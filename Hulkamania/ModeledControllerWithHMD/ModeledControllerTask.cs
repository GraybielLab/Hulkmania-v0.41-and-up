using log4net;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using Brandeis.AGSOL.Network;

namespace Brandeis.AGSOL.Hulkamania.Tasks.ModeledControllerWithHmd
{
    /// <summary>
    /// A task that simulates balancing an inverted pendulum.
    /// </summary>
    public class ModeledControllerWithHmdTask : HulkTask
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
        private ControllerType activeController = ControllerType.Bayes;
        private Stopwatch logStopwatch;
        private Stopwatch trialStopwatch;

        private ControlInput previousControlnput;

        private Trial trial;

        private double previousMillis;
        
        private bool previousTrigger;
        private bool queueResetEndSound;
        #endregion

        #region Enums
        public enum ControllerType
        {
            Bayes = 0,
            Vivek
        };
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets which of the supported controller types is currently active
        /// </summary>
        public ControllerType ActiveController { get { return activeController; } set { activeController = value; } }

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
        /// The last generated control input 
        /// </summary>
        public ControlInput ModeledControlInput { get { return previousControlnput; } }

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
        public ModeledControllerWithHmdTask()
        {
            logger.Debug("Create: BalanceTask");
            panel = new ModeledControllerWithHmdPanel(this);

            Priors.Read();
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
            
            ((ModeledControllerWithHmdPanel)panel).CleanUp();

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
            // joystick presses are not modeled
            return;
        }

        /// <summary>
        /// Returns the joystick input, modified according to protocol specifications.
        /// </summary>
        /// <returns>The modified joystick input</returns>
        private ControlInput GetInput()
        {
            // calculate and return control input based on our computational model
            previousControlnput =  GetModeledControlInput();
            return previousControlnput;
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
            // set the current joystick deflection to 0
            curDeflection = 0;

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

#region Controller modeling

        #region Bayesian
        /// <summary>
        /// Randomizer that generates uniform numbers from the 0..1 range
        /// </summary>
        private Random random = new Random();
        private long prevPeriodPassed = 0;
        private double curDeflection = 0;
        private double curJoystickFrequency = 1;

        /// <summary>
        /// Generate a random number drawn from a normal distribution
        /// </summary>
        /// <param name="mean">Mean of the distribution</param>
        /// <param name="stdDev">Standard deviation of the distribution</param>
        /// <returns></returns>
        private double RandomNormal(double mean, double stdDev)
        {
            // code taken from here: http://stackoverflow.com/questions/218060/random-gaussian-variables

            double u1 = random.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;
        }

        /// <summary>
        /// Given a PDF of joystick deflections, choose a deflection to apply
        /// </summary>
        /// <param name="pdf"></param>
        /// <returns></returns>
        private double RandomDeflectionFromPDF(double[] pdf)
        {
            double retval = 0;
            
            // first figure out which deflection bin to use
            double randProb = random.NextDouble();    // uniform 0..1 double
            double cdf = 0;
            for (int i = 0; i < pdf.Length; i++){
                cdf = cdf + pdf[i];
                if (cdf >= randProb){
                    // we now know what the bin is. Now determine what the actual deflection will be
                    // by drawing a random number that falls within that bin
                    retval = Priors.deflectionBins[i] - Priors.deflectionBinSize / 2 + random.NextDouble() * Priors.deflectionBinSize;
                    return retval;
                }
            }

            // this point can be reached if there is no prior, if that happens, do nothing
            return 0;
        }

        /// <summary>
        /// Given a PDF of joystick deflections, return the deflection with maximum a-priori probability. If there are multiple, return an average deflection.
        /// </summary>
        /// <param name="pdf"></param>
        /// <returns></returns>
        private double MAPDeflectionFromPDF(double[] pdf)
        {
            double retval = 0;

            List<double> probs = new List<double>();
            double maxProb = 0;
            
            for (int i = 0; i < pdf.Length; i++)
            {
                if (pdf[i] == maxProb)
                {
                    probs.Add(Priors.deflectionBins[i]);
                }

                if (pdf[i] > maxProb)
                {
                    probs.Clear();
                    probs.Add(Priors.deflectionBins[i]);
                    maxProb = pdf[i];
                }
            }

            foreach (double d in probs)
            {
                // draw random deflection from bin, add to return value, compute mean
                retval += (d - Priors.deflectionBinSize / 2 + random.NextDouble() * Priors.deflectionBinSize) / probs.Count;
            }

            return retval;
        }


        /// <summary>
        /// Computes and returns the control (joystick) input based on a computational model of the human as a controller
        /// </summary>
        /// <returns>ControlInput a structure that holds the control input properties</returns>
        private ControlInput GetModeledControlInputBayes(){

            double joystickX = 0; // joystick deflection on a scale of -1, 1

            // determine where we are in the current period
            int periodTime = (int)(1000.0 / curJoystickFrequency);
            long periodPassed = logStopwatch.ElapsedMilliseconds % periodTime;

            // scale to -1..1 range
            double xmen = periodPassed / (0.5 * periodTime) - 1;
            double sig = 0.2;  // parameter in the gaussian
            double Joyscale = (1 / (sig * 2.5) * Math.Exp(-1 * Math.Pow(xmen, 2) / (2 * Math.Pow(sig, 2)))) * 0.5;

            // uncomment this line to disable the gaussian joystick deflection 
            // Joyscale = 1;

            // joystick deflection should be recalculated if a period has ended, and we're in a new one
            bool recalculateDeflection = (periodPassed < prevPeriodPassed);
            prevPeriodPassed = periodPassed;

            // uncomment this line to disable frequency of joystick inputs (ie turn it into a continuous controller)
            //recalculateDeflection = true;

            if (recalculateDeflection)
            {
                // this is a structure that holds the current MARS axis position/velocity/acceleration
                MotionCommand currentMotionCommand = Hulk.CurrentMotion;

                // for now these properties are assigned by hand, they should be extracted from the data somehow
                const double JoystickFrequencyMean = 1; // Hz
                const double JoystickFrequencyStd = 0.25; // Hz;

                double WeightPosition = 0;              // weight of the position prior. all weights should sum to 1
                double WeightVelocity = 0;              // weight of the velocity prior. all weights should sum to 1
                double WeightPositionAndVelocity = 1;   // weight of the position*velocity prior. all weights should sum to 1

                // get the new joystick frequency (period time) for the next period. Make sure that the new frequency is within 2 std's away from the mean frequency
                curJoystickFrequency = RandomNormal(JoystickFrequencyMean, JoystickFrequencyStd);
                if (curJoystickFrequency < (JoystickFrequencyMean - 2 * JoystickFrequencyStd))
                {
                    curJoystickFrequency = (JoystickFrequencyMean - 2 * JoystickFrequencyStd);
                }
                if (curJoystickFrequency > (JoystickFrequencyMean + 2 * JoystickFrequencyStd))
                {
                    curJoystickFrequency = (JoystickFrequencyMean + 2 * JoystickFrequencyStd);
                }
              

                // get position, velocity, position*velocity PDFs            
                double[] deflectionGivenPosition = Priors.getDeflectionPDFForPosition(currentMotionCommand.innerPosition);
                double[] deflectionGivenVelocity = Priors.getDeflectionPDFForVelocity(currentMotionCommand.innerVelocity);
                double[] deflectionGivenPositionAndVelocity = Priors.getDeflectionPDFForPositionAndVelocity(currentMotionCommand.innerPosition, currentMotionCommand.innerVelocity);
               
                // choose a defelection from each of these PDFs
//                double deflectionPos = RandomDeflectionFromPDF(deflectionGivenPosition);
//                double deflectionVel = RandomDeflectionFromPDF(deflectionGivenVelocity);
//                double deflectionPosVel = RandomDeflectionFromPDF(deflectionGivenPositionAndVelocity);

                double deflectionPos = MAPDeflectionFromPDF(deflectionGivenPosition);
                double deflectionVel = MAPDeflectionFromPDF(deflectionGivenVelocity);
                double deflectionPosVel = MAPDeflectionFromPDF(deflectionGivenPositionAndVelocity);

                // weighted average of position, velocity, position*velocity PDF
                double deflectionFinal = WeightPosition * deflectionPos + WeightVelocity * deflectionVel + WeightPositionAndVelocity * deflectionPosVel;
          //      double deflectionFinal = Math.Max(deflectionPos, Math.Max(deflectionVel, deflectionPosVel));

                // clamp deflection to the range -1..1
                // set current deflection for this time period
                curDeflection = Math.Max(-1.0, Math.Min(deflectionFinal, 1.0));

                // output to debug console
                Debug.WriteLine("Position: " + currentMotionCommand.innerPosition + ", Velocity: " + currentMotionCommand.innerVelocity + ", Joystick frequency: " + curJoystickFrequency + ", deflection: " + curDeflection);
            }

            joystickX = curDeflection * Joyscale;

            // these control input properties should be left untouched
            double joystickY = 0; // joystick deflection on a scale of -1, 1
            bool joystickBlanked = false;
            bool joystickTrigger = false; 

            // create a new ControlInput structure based on the computational algorithm
            return new ControlInput(joystickX, joystickY, joystickTrigger, joystickBlanked);     
        }
        #endregion

        #region Vivek

        double TimeCounter = 0;

        /// <summary>
        /// Computes and returns the control (joystick) input based on a computational model of the human as a controller
        /// </summary>
        /// <returns>ControlInput a structure that holds the control input properties</returns>
        private ControlInput GetModeledControlInputVivek()
        {

            // this is a structure that holds the current MARS axis position/velocity/acceleration
            MotionCommand currentMotionCommand = Hulk.CurrentMotion;

            double joystickX = 0; // joystick deflection on a scale of -1, 1
            // -------------------------------------------------------

            //
            // calculate and update the joystick X deflection here
            //
            // Example: joystickX = currentMotionCommand.innerPosition * 12;

            // at the end of your code, you should assign a value to joystickX, e.g.
            // joystickX = myModeledValue;

            // if you need to know the elapsed time since the start of the trial, use
            // float elapsedTime = logStopwatch.ElapsedMilliseconds / 1000.0;
            // logStopwatch.ElapsedTicks
            // -------------------------------------------------------



            // gatekeeper = logStopwatch.ElapsedTicks % 25;
            // Console.WriteLine(logStopwatch.ElapsedTicks);
            // Console.WriteLine(logStopwatch.Elapsed);

            double diff = logStopwatch.ElapsedMilliseconds - TimeCounter;
            Console.WriteLine(diff);
            if (diff >= 500)
            {
                //Console.WriteLine(diff);
                //Console.WriteLine(logStopwatch.ElapsedMilliseconds);
                diff = 0;
                TimeCounter = logStopwatch.ElapsedMilliseconds;
            }

            //This makes sure that the input of the Gaussian is between -1 and 1
            double xmen = diff / 250 - 1;
            double sig = 0.2;  // parameter in the gaussian
            int ConstScale = 4;
            double Joyscale = (1 / (sig * 2.5) * Math.Exp(-1 * Math.Pow(xmen, 2) / (2 * Math.Pow(sig, 2)))) * 0.5;


            // Console.WriteLine(logStopwatch.ElapsedMilliseconds);
            //Console.WriteLine(previousMillis);

            //private double previousMillis;
            //private double HerpesCounter = 0;
            // Console.ReadKey(true);
            //joystickX = currentMotionCommand.innerPosition / 60;

            //***************************Uncomment the line below for position and velocity control
            //joystickX = ConstScale*Joyscale * (currentMotionCommand.innerPosition / 60 + currentMotionCommand.innerVelocity / 100) / 2;

            //***************************Uncomment the line below for velocity control
            joystickX = ConstScale * Joyscale * (currentMotionCommand.innerVelocity / 100);

            double joystickY = 0; // joystick deflection on a scale of -1, 1
            bool joystickBlanked = false;
            bool joystickTrigger = false;

            // create a new ControlInput structure based on the computational algorithm
            return new ControlInput(joystickX, joystickY, joystickTrigger, joystickBlanked);
        }

        #endregion

        /// <summary>
        /// Computes and returns the control (joystick) input based on a computational model of the human as a controller
        /// </summary>
        /// <returns>ControlInput a structure that holds the control input properties</returns>
        private ControlInput GetModeledControlInput(){

            // default control input: do nothing
            ControlInput retval = new ControlInput(0, 0, false, false);

            // figure out which controller should be active, and get the appropriate control input
            switch (activeController)
            {
                case ControllerType.Bayes:
                    retval = GetModeledControlInputBayes();
                    break;
                case ControllerType.Vivek:
                    retval = GetModeledControlInputVivek();
                    break;
            }
            return retval;
        }


#endregion

    }
}
