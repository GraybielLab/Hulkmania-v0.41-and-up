using log4net;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using Brandeis.AGSOL.Network;

namespace Brandeis.AGSOL.Hulkamania.Tasks.BalanceWithHmd
{
    /// <summary>
    /// A task that simulates balancing an inverted pendulum.
    /// </summary>
    public class BalanceWithHmdTask : HulkTask
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly String dataLogHeader = ";seconds,trialNumber,trialPhase," +
                "dirOfBalanceRoll,dirOfBalancePitch,dirOfBalanceYaw," +
                "movingDOBRoll,movingDOBPitch,movingDOBYaw," +
                "currentPosRoll,currentPosPitch,currentPosYaw," +
                "currentVelRoll,currentVelPitch,currentVelYaw," +
                "calcPosRoll,calcPosPitch,calcPosYaw," +
                "calcVelRoll,calcVelPitch,calcVelYaw," +
                "calcAccRoll,calcAccPitch,calcAccYaw," +
                "joystickX,joystickY,joystickBlanked,joystickPress," +
                "noiseRoll,noisePitch,noiseYaw\n";

        private enum MovementDirection
        {
            Forward,
            Backward,
            Left,
            Right,
            Up,
            Down
        };

        #endregion

        #region Fields
        private Stopwatch logStopwatch;
        private Stopwatch trialStopwatch;

        private Trial trial;

        private double previousMillis;
        
        private bool previousTrigger;
        private bool queueResetEndSound;
        #endregion

        #region Properties
        
        /// <summary>
        /// Any displayable value that is associated with the task and the inner axis.
        /// </summary>
        public override DisplayableData DataInnerAxis
        {
            get {

                DisplayableData data;

                if (trial != null) {

                    data = new DisplayableData();
                    data.pen = Pens.Green;
                    data.value = (Hulk.ChairMount == Hulk.MountType.Back) ? trial.MovingDirectionOfBalance.roll : 
                        trial.MovingDirectionOfBalance.yaw;

                    return data;
                }
                return null;
            }
        }

        /// <summary>
        /// Any displayable value that is associated with the task and the outer axis.
        /// </summary>
        public override DisplayableData DataOuterAxis
        {
            get {

                DisplayableData data;

                if (trial != null) {

                    data = new DisplayableData();
                    data.pen = Pens.Green;
                    data.value = trial.MovingDirectionOfBalance.pitch;

                    return data;
                }
                return null;
            }
        }

        /// <summary>
        /// Stopwatch that counts the time since the start of the first attempt at the trial.
        /// Used for logging.
        /// </summary>
        internal Stopwatch LogStopwatch
        {
            get {
                return logStopwatch;
            }
        }

        /// <summary>
        /// Stopwatch that counts the time since the start of the most recent attempt at the trial.
        /// Used to determine when the trial is complete.
        /// </summary>
        internal Stopwatch TrialStopwatch
        {
            get {
                return trialStopwatch;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the task.
        /// </summary>
        public BalanceWithHmdTask()
        {
            logger.Debug("Create: BalanceTask");
            
            panel = new BalanceWithHmdPanel(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called from the main control loop regardless of whether a task is running or not. Send data to network clients in this method
        /// </summary>
        public override void UpdateNetworkClients()
        {
            MotionCommand cmd = Hulk.CurrentMotion;

            double yaw = (Hulk.InnerAxis == Hulk.Axis.Roll) ? 0 : cmd.innerPosition;
            double pitch = cmd.outerPosition;
            double roll = (Hulk.InnerAxis == Hulk.Axis.Roll) ? cmd.innerPosition : 0;

            double yawv = (Hulk.InnerAxis == Hulk.Axis.Roll) ? 0 : cmd.innerVelocity;
            double pitchv = cmd.outerVelocity;
            double rollv = (Hulk.InnerAxis == Hulk.Axis.Roll) ? cmd.innerVelocity : 0;

            double yawa = (Hulk.InnerAxis == Hulk.Axis.Roll) ? 0 : cmd.innerAcceleration;
            double pitcha = cmd.outerAcceleration;
            double rolla = (Hulk.InnerAxis == Hulk.Axis.Roll) ? cmd.innerAcceleration : 0;


            AppMain.StatusTransmitter.Orientation.x = (float)pitch;
            AppMain.StatusTransmitter.Orientation.y = (float)yaw;
            AppMain.StatusTransmitter.Orientation.z = (float)roll;

            AppMain.StatusTransmitter.Velocity.x = (float)pitchv;
            AppMain.StatusTransmitter.Velocity.y = (float)yawv;
            AppMain.StatusTransmitter.Velocity.z = (float)rollv;

            AppMain.StatusTransmitter.Acceleration.x = (float)pitcha;
            AppMain.StatusTransmitter.Acceleration.y = (float)yawa;
            AppMain.StatusTransmitter.Acceleration.z = (float)rolla;

            AppMain.StatusTransmitter.transmitStatus();
        }

        /// <summary>
        /// Called whenever a network command is received that the task should deal with
        /// </summary>
        /// <param name="c">The command that was received</param>
        /// <param name="server">The server that received the command</param>
        /// <param name="handler">The client that sent the command. Use the client and server object if you need to send a reply</param>
        public override void ProcessNetworkCommand(ICommand c, INetworkServer server, Socket client)
        {
            eCommands command = (eCommands)c.CommandType;
            switch (command)
            {
                case eCommands.Message:
                    string msg = c.getParameterAsString((int)eMessageCommandParameters.Message);
                    logger.Info("Received message from:" + client.RemoteEndPoint.ToString() + ", " + msg);
                    break;

                case eCommands.TaskCompleted:
                    {
                        logger.Info("Received task completed command from:" + client.RemoteEndPoint.ToString());

                        _sendCommandStopTrial();
                        _sendCommandFadeOut();
                    }
                    break;
            
//                default:
//                    logger.Info("Received unknown command " + c.ToString() + " from:" + client.RemoteEndPoint.ToString() + "\r\n" + c.ToString());
//                    break;
            }
        }

        /// <summary>
        /// Called from the main control loop whenever the task is running.
        /// DO NOT call this method directly from BalanceTask or BalancePanel.
        /// </summary>
        public override void ContinueTask()
        {
            if (!InputController.IsJoystickConnected) {
                logger.Warn("No joystick found. Stopping balance task.");
                Hulk.StopTask();
                return;
            }

            switch (trial.TrialStatus) {
                case Trial.Status.Moving:
                    HandleStateMoving();
                    break;
                case Trial.Status.BalancingDOBChanging:     // DO NOT add a break here
                case Trial.Status.BalancingDOBStable:
                    HandleStateBalancing();
                    break;
                case Trial.Status.Resetting:
                    HandleResetting();
                    break;
                case Trial.Status.Complete:
                    HandleStateComplete();
                    break;
            }
        }

        /// <summary>
        /// Called from the main control loop. Can be used to log data before or after a trial is run.
        /// </summary>
        public override void LogBasicData()
        {
            if (logStopwatch == null) {
                return;
            }

            if (!InputController.IsJoystickConnected) {
                return;
            }

            if ((trial.TrialStatus == Trial.Status.BalancingDOBChanging) || (trial.TrialStatus == Trial.Status.BalancingDOBStable) ||
                (trial.TrialStatus == Trial.Status.Complete)) {
                return;
            }

            LogData(logStopwatch.ElapsedMilliseconds / 1000.0, trial, Hulk.CurrentMotion, null, InputController.JoystickInput, new RotationAngles());
        }

        /// <summary>
        /// Called from the main control loop whenever the task should be stopped.
        /// DO NOT call this method directly from BalanceTask or BalancePanel.
        /// </summary>
        public override void StopTask()
        {
             logger.Debug("Enter: StopTask()");

            if (trial.TrialStatus == Trial.Status.Complete) {
                logger.Info("Completed set of balance trials.");
            } else {
                logger.Info("Aborting balance trial: " + trial.TrialNumber);
                trial.TrialStatus = Trial.Status.Complete;
            }

            DataLogger.CloseDataLog();

            Hulk.Halt();
            
            ((BalanceWithHmdPanel)panel).CleanUp();

            logStopwatch = null;
            trialStopwatch = null;

            _sendCommandStopTrial();
            _sendCommandFadeOut();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Starts the task. Called when the operator clicks the Go button.
        /// </summary>
        /// <param name="stateInfo">Not used. Here so that the method can be called by a worker thread.</param>
        internal void Go(Object stateInfo)
        {
            MotionCommand moveCommand;

             logger.Debug("Enter: Go(Object)");

            Trials.CurrentTrialIndex = 0;
            trial = Trials.CurrentTrial;

            SendPlottingAxesCenterChange();

            queueResetEndSound = false;

            StartLogging();
            
            // Begin moving to the starting location of the first trial

             logger.Info("Moving to location for beginning of balance trial: " + trial.TrialNumber);

            trial.MovingDirectionOfBalance.yaw = trial.DirectionOfBalance.yaw;
            trial.MovingDirectionOfBalance.pitch = trial.DirectionOfBalance.pitch;
            trial.MovingDirectionOfBalance.roll = trial.DirectionOfBalance.roll;

            trial.TrialStatus = Trial.Status.Moving;
            trial.PlayMoveSound();
            
            moveCommand = new MotionCommand {
                innerVelocity = Hulk.NORMAL_VELOCITY,
                outerVelocity = Hulk.NORMAL_VELOCITY,
                innerAcceleration = Hulk.NORMAL_ACCELERATION,
                outerAcceleration = Hulk.NORMAL_ACCELERATION
            };
            if (trial.BeginAt == null) {
                moveCommand.innerPosition = (Hulk.InnerAxis == Hulk.Axis.Roll) ?
                    trial.DirectionOfBalance.roll : trial.DirectionOfBalance.yaw;
                moveCommand.outerPosition = trial.DirectionOfBalance.pitch;
            } else {
                moveCommand.innerPosition = (Hulk.InnerAxis == Hulk.Axis.Roll) ?
                    trial.BeginAt.roll : trial.BeginAt.yaw;
                moveCommand.outerPosition = trial.BeginAt.pitch;
            }

            Hulk.SetCommandType(Hulk.CommandType.ModulusPosition);
            Hulk.SetCommand(moveCommand);
            Hulk.StartDefinedMove(true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks whether the participant has indicated a position.
        /// </summary>
        /// <param name="controlInput">The joystick input</param>
        private void CheckJoystickPresses(ControlInput controlInput)
        {
            if (trial.TrialStatus != Trial.Status.BalancingDOBStable) {
                return;
            }

            if (!previousTrigger && controlInput.trigger) {
                trial.NumberIndications++;
            }
            previousTrigger = controlInput.trigger;

            // Optionally, give a reminder if the trigger has not been pressed this trial
            if (((trialStopwatch.ElapsedMilliseconds / 1000.0) >= 20.0) &&
                (trial.NumberIndications == 0) && (!trial.ReminderGiven) && trial.JoystickIndicationsMandatory) {

                trial.PlayReminderSound();
                trial.ReminderGiven = true;
            }
        }
              
        /// <summary>
        /// Returns the joystick input, modified according to protocol specifications.
        /// </summary>
        /// <returns>The modified joystick input</returns>
        private ControlInput GetInput()
        {
            ControlInput controlInput;
            ControlInput joystickInput;

            joystickInput = InputController.JoystickInput;
            
            // Rotate or mirror joystick input according to protocol file
            if (trial.JoystickControlType == Trial.JoystickControl.Clockwise) {
                controlInput = new ControlInput(-joystickInput.y, joystickInput.x, joystickInput.trigger, joystickInput.blanked);
            } else if (trial.JoystickControlType == Trial.JoystickControl.Counterclockwise) {
                controlInput = new ControlInput(joystickInput.y, -joystickInput.x, joystickInput.trigger, joystickInput.blanked);
            } else if (trial.JoystickControlType == Trial.JoystickControl.Mirrored) {
                controlInput = new ControlInput(-joystickInput.x, -joystickInput.y, joystickInput.trigger, joystickInput.blanked);
            } else {
                controlInput = joystickInput;
            }

            return controlInput;
        }

        /// <summary>
        /// Sets up logging for the start of a new trial.
        /// </summary>
        private void StartLogging()
        {
             logger.Debug("Enter: StartLogging()");

            DataLogger.AcquireDataLog(String.Format(Trials.ProtocolFilename + "_trial_{0}.csv",
                Trials.CurrentTrial.TrialNumber.ToString("000")), dataLogHeader);
            logStopwatch = new Stopwatch();
            logStopwatch.Start();
        }

        /// <summary>
        /// Sets up timing and movement commands for the start of a new trial.
        /// </summary>
        private void StartTrial()
        {
            logger.Debug("Enter: StartTrial()");
            logger.Info("Starting balance trial: " + trial.TrialNumber);
            
            ICommand c = new ICommand();
            c.CommandType = (int)eCommands.SelectTrial;
            c.addParameter((int)eSelectTrialCommandParameters.TrialNumber, trial.TrialNumber.ToString());   
   
            trial.PlayStartSound();

            trialStopwatch = new Stopwatch();
            trialStopwatch.Start();
            previousMillis = trialStopwatch.ElapsedMilliseconds;

            trial.TrialStatus = Trial.Status.BalancingDOBChanging;

            Hulk.SetCommandType(Hulk.CommandType.Velocity);
        }

        /// <summary>
        /// Moves the DOB closer to its final position for this trial.
        /// </summary>
        private void UpdateMovingDOB()
        {
            bool checkInner;    // Flags for whether the DOB has reached its final value for this trial
            bool checkOuter;

            checkInner = false;
            checkOuter = false;

            if (Hulk.ChairMount == Hulk.MountType.Back) {
                if ((trial.MovingDirectionOfBalance.roll - trial.DirectionOfBalance.roll) > 0.05) {
                    trial.MovingDirectionOfBalance.roll -= 0.05;
                } else if ((trial.MovingDirectionOfBalance.roll - trial.DirectionOfBalance.roll) < -0.05) {
                    trial.MovingDirectionOfBalance.roll += 0.05;
                } else {
                    trial.MovingDirectionOfBalance.roll = trial.DirectionOfBalance.roll;
                    checkInner = true;
                }
            } else {
                if ((trial.MovingDirectionOfBalance.yaw - trial.DirectionOfBalance.yaw) > 0.05) {
                    trial.MovingDirectionOfBalance.yaw -= 0.05;
                } else if ((trial.MovingDirectionOfBalance.yaw - trial.DirectionOfBalance.yaw) < -0.05) {
                    trial.MovingDirectionOfBalance.yaw += 0.05;
                } else {
                    trial.MovingDirectionOfBalance.yaw = trial.DirectionOfBalance.yaw;
                    checkInner = true;
                }
            }

            if ((trial.MovingDirectionOfBalance.pitch - trial.DirectionOfBalance.pitch) > 0.05) {
                trial.MovingDirectionOfBalance.pitch -= 0.05;
            } else if ((trial.MovingDirectionOfBalance.pitch - trial.DirectionOfBalance.pitch) < -0.05) {
                trial.MovingDirectionOfBalance.pitch += 0.05;
            } else {
                trial.MovingDirectionOfBalance.pitch = trial.DirectionOfBalance.pitch;
                checkOuter = true;
            }

            if (checkInner && checkOuter) {

                previousTrigger = false;
                trial.NumberIndications = 0;
                
                trialStopwatch = new Stopwatch();
                trialStopwatch.Start();
                previousMillis = trialStopwatch.ElapsedMilliseconds;

                trial.TrialStatus = Trial.Status.BalancingDOBStable;
            }
        }

        #region State Handling

        /// <summary>
        /// Called by the control loop when the participant is actively balancing.
        /// </summary>
        private void HandleStateBalancing()
        {
            ControlInput controlInput;
            MotionCommand calculatedMotion;
            MotionCommand newMotion;
            RotationAngles noise;
            double time;
            double dt;

            if (queueResetEndSound) {
                trial.PlayResetEndSound();
                queueResetEndSound = false;
            }

    
            // Calculate how much time elapsed since the previous call
            time = trialStopwatch.ElapsedMilliseconds;
            dt = (time - previousMillis) / 1000.0;
            
            // Remember the time for the next call
            previousMillis = time;

            // Move the DOB slowly towards its value for the current trial
            if (trial.TrialStatus == Trial.Status.BalancingDOBChanging) {
                UpdateMovingDOB();
            }

            if ((trial.TrialStatus == Trial.Status.BalancingDOBStable) && (trialStopwatch.ElapsedMilliseconds / 1000.0) >= trial.TimeLimit)
            {
                // Trial time limit was reached
                trial.TrialStatus = Trial.Status.Complete;

            } else {

                // Trial time limit has not been reached. Keep balancing.
                controlInput = GetInput();
                
                CheckJoystickPresses(controlInput);

                // Determine new motion based on current position and joystick input
                MotionCommand lastKnownHulkPosition = Hulk.CurrentMotion;
                newMotion = new MotionCommand();

                // Determine the angle between the current location and the desired location
                if (Hulk.InnerAxis == Hulk.Axis.Roll)
                {
                    newMotion.innerPosition = lastKnownHulkPosition.innerPosition - trial.MovingDirectionOfBalance.roll;
                }
                else
                {
                    newMotion.innerPosition = lastKnownHulkPosition.innerPosition - trial.MovingDirectionOfBalance.yaw;
                }
                newMotion.outerPosition = lastKnownHulkPosition.outerPosition - trial.MovingDirectionOfBalance.pitch;

                // Keep the angles between -180 and 180
                if (newMotion.innerPosition > 180.0)
                {
                    newMotion.innerPosition = newMotion.innerPosition - 360.0;
                }
                else if (newMotion.innerPosition < -180.0)
                {
                    newMotion.innerPosition = newMotion.innerPosition + 360.0;
                }
                if (newMotion.outerPosition > 180.0)
                {
                    newMotion.outerPosition = newMotion.outerPosition - 360.0;
                }
                else if (newMotion.outerPosition < -180.0)
                {
                    newMotion.outerPosition = newMotion.outerPosition + 360.0;
                }

                // Get the current velocity
                newMotion.innerVelocity = lastKnownHulkPosition.innerVelocity;
                newMotion.outerVelocity = lastKnownHulkPosition.outerVelocity;

                // Update new motion based on IVP dynamics
                noise = trial.GetNextNoise();

                calculatedMotion = Dynamics.CalculateDynamics(trial, newMotion, controlInput, noise, dt);
                
                LogData(logStopwatch.ElapsedMilliseconds / 1000.0, trial, newMotion, calculatedMotion, controlInput, noise);

                // Check whether the max angle was exceeded
                if (trial.ExceedsMaxAngle(calculatedMotion))
                {
                    // Participant went too far away from the balance point.
                    trial.TrialStatus = Trial.Status.Resetting;

                } else {

                    // Participant is still balancing

                    // Maximize ability of motors to follow the commanded velocities
                    calculatedMotion.outerAcceleration = trial.MaxAcceleration;
                    calculatedMotion.innerAcceleration = trial.MaxAcceleration;
                    calculatedMotion.outerPosition = Hulk.ANY_MOTION;
                    calculatedMotion.innerPosition = Hulk.ANY_MOTION;

                    // Cancel movement commands for unused axes
                    if ((Hulk.InnerAxis == Hulk.Axis.Roll) && !trial.UseRoll) {
                        calculatedMotion.innerVelocity = 0.0;
                    } else if ((Hulk.InnerAxis == Hulk.Axis.Yaw) && !trial.UseYaw) {
                        calculatedMotion.innerVelocity = 0.0;
                    }
                    if (!trial.UsePitch) {
                        calculatedMotion.outerVelocity = 0.0;
                    }

                    Hulk.SetCommand(calculatedMotion);
                    Hulk.ContinueTask();
                }
            }

        }

        /// <summary>
        /// Calculates a random value between [min, max], and randomizes the sign as well
        /// </summary>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximum value</param>
        /// <returns></returns>
        private static double _calculateDOBOffset(double min, double max)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            double dobOffset = min + rnd.NextDouble() * (max - min);
            if (rnd.NextDouble() >= 0.5f)
            {
                dobOffset = dobOffset * -1;
            }
            return dobOffset;
        }

        /// <summary>
        /// Called by the control loop when the participant exceeds the max allowable angle from the DOB.
        /// </summary>
        private void HandleResetting()
        {
            MotionCommand moveCommand;

            // Restart with balancing once reset is complete
            trial.TrialStatus = Trial.Status.Moving;

            Hulk.Halt();

            trial.PlayResetStartSound();

            queueResetEndSound = true;

            // Move back to the DOB

            moveCommand = new MotionCommand {
                innerVelocity = Hulk.NORMAL_VELOCITY,
                outerVelocity = Hulk.NORMAL_VELOCITY,
                innerAcceleration = Hulk.NORMAL_ACCELERATION,
                outerAcceleration = Hulk.NORMAL_ACCELERATION
            };

            // calculate random offset from DOB to return the chair to

            double dobOffsetY = _calculateDOBOffset(trial.RestartDOBOffsetMinYaw, trial.RestartDOBOffsetMaxYaw);
            double dobOffsetP = _calculateDOBOffset(trial.RestartDOBOffsetMinPitch, trial.RestartDOBOffsetMaxPitch);
            double dobOffsetR = _calculateDOBOffset(trial.RestartDOBOffsetMinRoll, trial.RestartDOBOffsetMaxRoll);

            if (Math.Abs(dobOffsetY) > trial.MaxAngle)
            {
                logger.Warn("The calculated random offset for the DOB yaw return position is outside of the maximum angle specified for this trial. Using a random offset of 0!");
                dobOffsetY = 0;
            }
            if (Math.Abs(dobOffsetP) > trial.MaxAngle)
            {
                logger.Warn("The calculated random offset for the DOB pitch return position is outside of the maximum angle specified for this trial. Using a random offset of 0!");
                dobOffsetP = 0;
            }
            if (Math.Abs(dobOffsetR) > trial.MaxAngle)
            {
                logger.Warn("The calculated random offset for the DOB roll return position is outside of the maximum angle specified for this trial. Using a random offset of 0!");
                dobOffsetR = 0;
            }

            double resetDobY = trial.MovingDirectionOfBalance.yaw + dobOffsetY;
            double resetDobP = trial.MovingDirectionOfBalance.pitch + dobOffsetP;
            double resetDobR = trial.MovingDirectionOfBalance.roll + dobOffsetR;
            
            moveCommand.innerPosition = (Hulk.InnerAxis == Hulk.Axis.Roll) ? resetDobR : resetDobY;
            moveCommand.outerPosition = resetDobP;

            Hulk.SetCommandType(Hulk.CommandType.ModulusPosition);
            Hulk.SetCommand(moveCommand);

            Hulk.StartDefinedMove(true);

            logger.Info("Participant lost control. Resetting to orientation: yaw " + resetDobY.ToString("0.##") + " pitch " + resetDobP.ToString("0.##") + " roll " + resetDobR.ToString("0.##"));

            _sendCommandStopTrial();
        }

        /// <summary>
        /// Called by the control loop when the chair has moved (at start of block of trials, or after a reset).
        /// </summary>
        private void HandleStateMoving()
        {
            StartTrial();

            _sendCommandStartTrial((uint)trial.TrialNumber);
        }

        /// <summary>
        /// Called by th control loop when the trial's time limit has been reached.
        /// </summary>
        private void HandleStateComplete()
        {
             logger.Info("Completed balance trial: " + trial.TrialNumber);

            DataLogger.CloseDataLog();
            
            bool advanceToNextTrial = true;
            if ((trial.NumberIndications == 0) && trial.JoystickIndicationsMandatory)
            {
                logger.Warn("Participant did not indicate during this trial - restarting trial");
            
                StartLogging();
                
                StartTrial();

                advanceToNextTrial = false;
            }

            if (advanceToNextTrial)
            {
                trial.PlayEndSound();
                
                if (Trials.CurrentTrialIndex == (Trials.List.Count - 1))
                {

                    // Entire protocol has been completed
                    Hulk.StopTask();

                }
                else
                {

                    // Continue to following trial
                    Trials.CurrentTrialIndex++;

                    trial = Trials.CurrentTrial;

                    StartLogging();

                    trial.MovingDirectionOfBalance.roll = Trials.PreviousTrial.MovingDirectionOfBalance.roll;
                    trial.MovingDirectionOfBalance.pitch = Trials.PreviousTrial.MovingDirectionOfBalance.pitch;
                    trial.MovingDirectionOfBalance.yaw = Trials.PreviousTrial.MovingDirectionOfBalance.yaw;

                    StartTrial();
                }
            }

        }

        /// <summary>
        /// Informs listeners about where the center of axes should be for the current protocol. This function is only called when the 'Go' button is clicked. 
        /// It is assumed that the DOB of the 1st trial in any given protocol indicates where the 'upright' is. This could be either (0, 0) when the chair is facing the desk,
        /// or (180, 180) when it is facing the wall
        /// </summary>
        private void SendPlottingAxesCenterChange()
        {
            double innerCenter = (Hulk.InnerAxis == Hulk.Axis.Roll) ? trial.DirectionOfBalance.roll : trial.DirectionOfBalance.yaw;
            double outerCenter = trial.DirectionOfBalance.pitch;
            base.SendAxesCentersChangedEvent(innerCenter, outerCenter);
        }

        #endregion

        /// <summary>
        /// Logs the data from the round of calculations.
        /// </summary>
        /// <param name="time">Seconds since start of trial</param>
        /// <param name="trial">The trial being run</param>
        /// <param name="currentMotion">Current motion</param>
        /// <param name="calculatedMotion">New motion, calculated by dynamics</param>
        /// <param name="controlInput">Current joystick position</param>
        /// <param name="noise">Noise input to position</param>
        internal void LogData(double time, Trial trial, MotionCommand currentMotion, MotionCommand calculatedMotion,
            ControlInput controlInput, RotationAngles noise)
        {
            StringBuilder builder;
            RotationAngles position;
            RotationAngles calcPosition;
            int phase;

            position = new RotationAngles();
            calcPosition = new RotationAngles();
            
            builder = new StringBuilder();
            switch (trial.TrialStatus) {
                case Trial.Status.Initializing:
                    phase = 0;
                    break;
                case Trial.Status.Moving:
                    phase = 1;
                    if (Hulk.InnerAxis == Hulk.Axis.Roll) {
                        position.roll = currentMotion.innerPosition;
                    } else {
                        position.yaw = currentMotion.innerPosition;
                    }
                    position.pitch = currentMotion.outerPosition;
                    break;
                case Trial.Status.BalancingDOBChanging:
                    phase = 2;
                    if (Hulk.InnerAxis == Hulk.Axis.Roll) {
                        position.roll = currentMotion.innerPosition + trial.MovingDirectionOfBalance.roll;
                        calcPosition.roll = calculatedMotion.innerPosition + trial.MovingDirectionOfBalance.roll;
                    } else {
                        position.yaw = currentMotion.innerPosition + trial.MovingDirectionOfBalance.yaw;
                        calcPosition.yaw = calculatedMotion.innerPosition + trial.MovingDirectionOfBalance.yaw;
                    }
                    position.pitch = currentMotion.outerPosition + trial.MovingDirectionOfBalance.pitch;
                    calcPosition.pitch = calculatedMotion.outerPosition + trial.MovingDirectionOfBalance.pitch;
                    break;
                case Trial.Status.BalancingDOBStable:
                    phase = 3;
                    if (Hulk.InnerAxis == Hulk.Axis.Roll) {
                        position.roll = currentMotion.innerPosition + trial.MovingDirectionOfBalance.roll;
                        calcPosition.roll = calculatedMotion.innerPosition + trial.MovingDirectionOfBalance.roll;
                    } else {
                        position.yaw = currentMotion.innerPosition + trial.MovingDirectionOfBalance.yaw;
                        calcPosition.yaw = calculatedMotion.innerPosition + trial.MovingDirectionOfBalance.yaw;
                    }
                    position.pitch = currentMotion.outerPosition + trial.MovingDirectionOfBalance.pitch;
                    calcPosition.pitch = calculatedMotion.outerPosition + trial.MovingDirectionOfBalance.pitch;
                    break;
                case Trial.Status.Resetting:
                    phase = 4;
                    if (Hulk.InnerAxis == Hulk.Axis.Roll) {
                        position.roll = currentMotion.innerPosition;
                    } else {
                        position.yaw = currentMotion.innerPosition;
                    }
                    position.pitch = currentMotion.outerPosition;
                    break;
                case Trial.Status.Complete:
                    phase = 5;
                    break;
                default:
                    phase = -1;
                    break;
            }

            if (Hulk.InnerAxis == Hulk.Axis.Roll) {
                builder.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30}\n",
                   time,
                   trial.TrialNumber,
                   phase,
                   trial.DirectionOfBalance.roll,
                   trial.DirectionOfBalance.pitch,
                   trial.DirectionOfBalance.yaw,
                   trial.MovingDirectionOfBalance.roll,
                   trial.MovingDirectionOfBalance.pitch,
                   trial.MovingDirectionOfBalance.yaw,
                   position.roll,
                   position.pitch,
                   0,
                   currentMotion.innerVelocity.ToString("F6"),
                   currentMotion.outerVelocity.ToString("F6"),
                   0,
                   ((calculatedMotion != null) ? calcPosition.roll.ToString("F6") : "0"),
                   ((calculatedMotion != null) ? calcPosition.pitch.ToString("F6") : "0"),
                   0,
                   ((calculatedMotion != null) ? calculatedMotion.innerVelocity.ToString("F6") : "0"),
                   ((calculatedMotion != null) ? calculatedMotion.outerVelocity.ToString("F6") : "0"),
                   0,
                   ((calculatedMotion != null) ? calculatedMotion.innerAcceleration.ToString("F6") : "0"),
                   ((calculatedMotion != null) ? calculatedMotion.outerAcceleration.ToString("F6") : "0"),
                   0,
                   controlInput.x.ToString("F6"),
                   controlInput.y.ToString("F6"),
                   (controlInput.blanked ? "1" : "0"),
                   (controlInput.trigger ? "1" : "0"),
                   noise.roll,
                   noise.pitch,
                   noise.yaw
               );
            } else {
                builder.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30}\n",
                   time,
                   trial.TrialNumber,
                   phase,
                   trial.DirectionOfBalance.roll,
                   trial.DirectionOfBalance.pitch,
                   trial.DirectionOfBalance.yaw,
                   trial.MovingDirectionOfBalance.roll,
                   trial.MovingDirectionOfBalance.pitch,
                   trial.MovingDirectionOfBalance.yaw,
                   0,
                   position.pitch,
                   position.yaw,
                   0,
                   currentMotion.outerVelocity.ToString("F6"),
                   currentMotion.innerVelocity.ToString("F6"),
                   0,
                   ((calculatedMotion != null) ? calcPosition.pitch.ToString("F6") : "0"),
                   ((calculatedMotion != null) ? calcPosition.yaw.ToString("F6") : "0"),
                   0,
                   ((calculatedMotion != null) ? calculatedMotion.outerVelocity.ToString("F6") : "0"),
                   ((calculatedMotion != null) ? calculatedMotion.innerVelocity.ToString("F6") : "0"),
                   0,
                   ((calculatedMotion != null) ? calculatedMotion.outerAcceleration.ToString("F6") : "0"),
                   ((calculatedMotion != null) ? calculatedMotion.innerAcceleration.ToString("F6") : "0"),
                   controlInput.x.ToString("F6"),
                   controlInput.y.ToString("F6"),
                   (controlInput.blanked ? "1" : "0"),
                   (controlInput.trigger ? "1" : "0"),
                   noise.roll,
                   noise.pitch,
                   noise.yaw
               );
            }

    
            DataLogger.AppendData(builder.ToString());

        }

        #endregion
        
        #region Network

        // -------------------------------------------------------------------------------------------------------------------------
        private float _constrainEulerAngleForHulk(float angle)
        {
            float retval = angle;
            if (angle > 180) {
                angle = angle - 360;
            }

            if (angle < -180) {
                angle = angle + 360;
            }

            return angle;
        }
        
        // -------------------------------------------------------------------------------------------------------------------------
        private void _sendCommandStartTrial(uint trialNumber)
        {
            _sendCommandSetupTrial(trialNumber-1);

            ICommand c = new ICommand();
            c.CommandType = (int)eCommands.Start;
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);
        }

        // -------------------------------------------------------------------------------------------------------------------------
        // send command to client(s) to  stop executing the current trial
        private void _sendCommandStopTrial()
        {
            ICommand c = new ICommand();
            c.CommandType = (int)eCommands.Stop;
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);
        }

        // -------------------------------------------------------------------------------------------------------------------------
        // send command to client(s) to select the trial no
        private void _sendCommandSetupTrial(uint trialNumber){
            ICommand c = new ICommand();
            c.CommandType = (int)eCommands.SelectTrial;
            c.addParameter((int)eSelectTrialCommandParameters.TrialNumber, trialNumber.ToString());
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);
        }

      
        // -------------------------------------------------------------------------------------------------------------------------
        // send command to client(s) to fade out to the waiting screen
        private void _sendCommandFadeOut()
        {
            ICommand c = new ICommand();
            c.CommandType = (int)eCommands.FadeOut;
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);
        }

        // -------------------------------------------------------------------------------------------------------------------------
        private void _sendCommandSelectScene(int sceneNum)
        {
            ICommand c = new ICommand();
            c.CommandType = (int)eCommands.SelectScene;
            c.addParameter((int)eSelectSceneCommandParameters.SceneNumber, sceneNum.ToString());
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);
        }

        #endregion

    }
}
