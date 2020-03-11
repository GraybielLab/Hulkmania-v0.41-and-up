using log4net;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Text;

namespace Brandeis.AGSOL.Hulkamania.Tasks.PassiveMovement
{
    /// <summary>
    /// A task that moves the chair passively based on a recorded movement.
    /// </summary>
    public class PassiveMovementTask : HulkTask
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private const double POSITIONING_VELOCITY = 270.0;
        private const double POSITIONING_ACCELERATION = 450.0;
     
        private static readonly String dataLogHeader = "seconds,trialNumber,trialPhase," +
                "dirOfBalanceRoll,dirOfBalancePitch,dirOfBalanceYaw," +
                "movingDOBRoll,movingDOBPitch,movingDOBYaw," +
                "currentPosRoll,currentPosPitch,currentPosYaw," +
                "currentVelRoll,currentVelPitch,currentVelYaw," +
                "calcPosRoll,calcPosPitch,calcPosYaw," +
                "joystickPress\n";

        #endregion

        #region Fields
        
        private Stopwatch runningStopwatch;

        private double lastPositionOuter;
        private double lastPositionInner;

        private int currentTrialNumber;
        private int numClicks;
        
        private bool previousTrigger;

        #endregion

        #region Properties

        /// <summary>
        /// The number of the currently running trial.
        /// </summary>
        internal int CurrentTrialNumber
        {
            get {
                return currentTrialNumber;
            }
        }

        /// <summary>
        /// The number of times the joystick has been clicked this trial.
        /// </summary>
        internal int NumClicks
        {
            get {
                return numClicks;
            }
        }

        /// <summary>
        /// How many seconds have elapsed in the current trial.
        /// </summary>
        internal int TrialTime
        {
            get {
                if (runningStopwatch != null) {
                    return (int)(runningStopwatch.ElapsedMilliseconds / 1000);
                }
                return -1;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the task.
        /// </summary>
        public PassiveMovementTask()
        {
            logger.Debug("Create: PassiveMovementTask");

            runningStopwatch = new Stopwatch();

            panel = new PassiveMovementPanel(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called from the main control loop whenever the task is running.
        /// DO NOT call this method directly from PassiveMovementTask or PassiveMovementPanel.
        /// </summary>
        public override void ContinueTask()
        {
            MotionCommand command;
            RecordingTimepoint recording;
            double elapsedTime;
            double angleDiff;

            elapsedTime = runningStopwatch.ElapsedMilliseconds * 1.0 / 1000;
            
            // Check whether the entire block is complete
            if (Recordings.IsEndOfRecordingSeries(currentTrialNumber, elapsedTime)) {

                // Play the last recording timepoint
                recording = Recordings.GetRecording(currentTrialNumber, elapsedTime);

                command = new MotionCommand();
                command.outerPosition = recording.Angles.pitch; 
                if (Hulk.ChairMount == Hulk.MountType.Back) {
                    command.innerPosition = recording.Angles.roll;
                } else {
                    command.innerPosition = recording.Angles.yaw;
                }

                Hulk.SetCommand(command);
                Hulk.ContinueTask();

                // Stop the HULK and return to idling

                Hulk.StopTask();

                return;
            }

            // Check whether the current trial is complete
            if (Recordings.IsEndOfRecordingTrial(currentTrialNumber, elapsedTime)) {

                // Clean up from last trial
                DataLogger.CloseDataLog();
                runningStopwatch.Reset();

                // Prepare for next trial

                runningStopwatch.Start();
                
                currentTrialNumber++;
                numClicks = 0;

                ((PassiveMovementPanel)panel).UpdateListbox();

                DataLogger.AcquireDataLog(String.Format("recording{0:000}.csv", currentTrialNumber), dataLogHeader);

                return;
            }

            // Determine the control input for this simulation step
            if (Recordings.HasRecording()) {

                // Check whether the trigger has just been pressed
                if (!previousTrigger && InputController.JoystickInput.trigger) {
                    numClicks++;
                }
                previousTrigger = InputController.JoystickInput.trigger;

                recording = Recordings.GetRecording(currentTrialNumber, elapsedTime);

                command = new MotionCommand();               
                command.outerPosition = recording.Angles.pitch;
                if (Hulk.ChairMount == Hulk.MountType.Back) {
                    command.innerPosition = recording.Angles.roll;
                } else {
                    command.innerPosition = recording.Angles.yaw;
                }

                // Stop if a large angle change is commanded. A large angle change could be dangerous at these accelerations.
                angleDiff = Math.Abs(lastPositionInner - command.innerPosition);
                if ((angleDiff > 3.0) && (angleDiff < 357.0)) {

                    logger.Warn("SAFETY: Instantaneous INNER move >3 deg prevented. Current=" +
                    lastPositionInner.ToString("F2") + " New=" + command.innerPosition.ToString("F2"));

                    Hulk.StopTask();

                    return;
                }
                angleDiff = Math.Abs(lastPositionOuter - command.outerPosition);
                if ((angleDiff > 3.0) && (angleDiff < 357.0)) {

                    logger.Warn("SAFETY: Instantaneous OUTER move >3 deg prevented. Current=" +
                    lastPositionOuter.ToString("F2") + " New=" + command.outerPosition.ToString("F2"));

                    Hulk.StopTask();

                    return;
                }

                lastPositionOuter = command.outerPosition;
                lastPositionInner = command.innerPosition;

                LogData(elapsedTime, Hulk.CurrentMotion, command, recording, InputController.JoystickInput);

                Hulk.SetCommand(command);
                Hulk.ContinueTask();
            }
        }

        /// <summary>
        /// Called from the main control loop whenever the task should be stopped.
        /// DO NOT call this method directly from PassiveMovementTask or PassiveMovementPanel.
        /// </summary>
        public override void StopTask()
        {
            logger.Debug("Enter: StopTask()");

            logger.Info("Stopping passive movement task.");

            Hulk.Halt();

            ((PassiveMovementPanel)panel).CleanUp();
            runningStopwatch.Stop();
            DataLogger.CloseDataLog();
        }

        #endregion

        #region Internal Methods
        
        /// <summary>
        /// Starts the task. Called when the operator clicks the Go button.
        /// </summary>
        internal void Go()
        {
            MotionCommand newMotionCommand;

            logger.Debug("Enter: Go()");

            logger.Info("Starting passive movement task.");

            previousTrigger = false;
            numClicks = 0;

            lastPositionOuter = Hulk.CurrentMotion.outerPosition;
            lastPositionInner = Hulk.CurrentMotion.innerPosition;

            currentTrialNumber = Recordings.GetFirstTrialNumber();

            DataLogger.AcquireDataLog(String.Format("recording{0:000}.csv", currentTrialNumber), dataLogHeader);

            runningStopwatch.Reset();
            runningStopwatch.Start();

            Hulk.SetCommandType(Hulk.CommandType.ModulusPosition);

            newMotionCommand = new MotionCommand {
                outerVelocity = POSITIONING_VELOCITY,
                innerVelocity = POSITIONING_VELOCITY,
                outerAcceleration = POSITIONING_ACCELERATION,
                innerAcceleration = POSITIONING_ACCELERATION
            };

            Hulk.SetCommand(newMotionCommand);
            Hulk.StartTask();
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Logs the data from the round of calculations.
        /// </summary>
        /// <param name="time">Seconds since start of trial</param>
        /// <param name="currentMotion">Current motion</param>
        /// <param name="calculatedMotion">New motion, calculated by dynamics</param>
        /// <param name="recording">The c</param>
        /// <param name="controlInput">Current joystick position</param>
        internal void LogData(double time, MotionCommand currentMotion, MotionCommand calculatedMotion, RecordingTimepoint recording, ControlInput controlInput)
        {
            StringBuilder builder;

            builder = new StringBuilder();
            
            if (Hulk.InnerAxis == Hulk.Axis.Roll) {
                builder.AppendFormat(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}\n",
                    time,
                    recording.TrialNumber,
                    recording.TrialPhase,
                    recording.DirectionOfBalance.roll,
                    recording.DirectionOfBalance.pitch,
                    recording.DirectionOfBalance.yaw,
                    recording.MovingDirectionOfBalance.roll,
                    recording.MovingDirectionOfBalance.pitch,
                    recording.MovingDirectionOfBalance.yaw,
                    currentMotion.innerPosition.ToString("F6"),
                    currentMotion.outerPosition.ToString("F6"),
                    0,
                    currentMotion.innerVelocity.ToString("F6"),
                    currentMotion.outerVelocity.ToString("F6"),
                    0,
                    calculatedMotion.innerPosition.ToString("F6"),
                    calculatedMotion.outerPosition.ToString("F6"),
                    0,
                    (controlInput.trigger ? "1" : "0")
               );
            } else {
                builder.AppendFormat(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}\n",
                    time,
                    recording.TrialNumber,
                    recording.TrialPhase,
                    recording.DirectionOfBalance.roll,
                    recording.DirectionOfBalance.pitch,
                    recording.DirectionOfBalance.yaw,
                    recording.MovingDirectionOfBalance.roll,
                    recording.MovingDirectionOfBalance.pitch,
                    recording.MovingDirectionOfBalance.yaw,
                    0,
                    currentMotion.outerPosition.ToString("F6"),
                    currentMotion.innerPosition.ToString("F6"),
                    0,
                    currentMotion.outerVelocity.ToString("F6"),
                    currentMotion.innerVelocity.ToString("F6"),
                    0,
                    calculatedMotion.outerPosition.ToString("F6"),
                    calculatedMotion.innerPosition.ToString("F6"),
                    (controlInput.trigger ? "1" : "0")
               );
            }

            DataLogger.AppendData(builder.ToString());
        }

        #endregion
    }
}
