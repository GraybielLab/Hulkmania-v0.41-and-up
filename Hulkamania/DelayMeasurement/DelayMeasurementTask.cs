using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Brandeis.AGSOL.Hulkamania.Tasks.DelayMeasurement
{
    /// <summary>
    /// A task that measures the delay between movement commands and the begin of movement.
    /// </summary>
    public class DelayMeasurementTask : HulkTask
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private const double POSITIONING_VELOCITY = 270.0;
        private const double POSITIONING_ACCELERATION = 450.0;
      
        private static readonly String dataLogHeader = ";seconds,trialPhase," +
                "currentPosRoll,currentPosPitch,currentPosYaw," +
                "currentVelRoll,currentVelPitch,currentVelYaw," +
                "calcPosRoll,calcPosPitch,calcPosYaw," +
                "calcVelRoll,calcVelPitch,calcVelYaw," +
                "calcAccRoll,calcAccPitch,calcAccYaw," +
                "joystickX,joystickY\n";

        #endregion

        #region Fields

        private Stopwatch runningStopwatch;

        private double amplitude;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the task.
        /// </summary>
        public DelayMeasurementTask()
        {
            logger.Debug("Create: DelayMeasurementTask");

            runningStopwatch = new Stopwatch();

            panel = new DelayMeasurementPanel(this);
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Called from the main control loop whenever the task is running.
        /// DO NOT call this method directly from DelayMeasurementTask or DelayMeasurementPanel.
        /// </summary>
        public override void ContinueTask()
        {
            ControlInput controlInput;
            FunctionTimepoint function;
            MotionCommand command;
            double elapsedTime;
            
            elapsedTime = runningStopwatch.ElapsedMilliseconds * 1.0 / 1000;
           
            function = ForcingFunctions.GetForcingFunction(elapsedTime);
                
            controlInput = (Hulk.InnerAxis == Hulk.Axis.Roll) ?
                new ControlInput(function.Input.roll, function.Input.pitch, false, false) :
                new ControlInput(function.Input.yaw, function.Input.pitch, false, false);

            command = new MotionCommand();
            command.innerVelocity = controlInput.x * amplitude;
            command.outerVelocity = controlInput.y * amplitude;
            command.innerAcceleration = 300;
            command.outerAcceleration = 300;

            Hulk.SetCommand(command);
            Hulk.ContinueTask();

            LogData(elapsedTime, Hulk.CurrentMotion, command, controlInput);

            if (elapsedTime >= ForcingFunctions.GetForcingFunctionMaxTime()) {

                Hulk.StopTask();

                return;
            }
        }

        /// <summary>
        /// Called from the main control loop whenever the task should be stopped.
        /// DO NOT call this method directly from DelayMeasurementTask or DelayMeasurementPanel.
        /// </summary>
        public override void StopTask()
        {
            logger.Debug("Enter: StopTask()");

            logger.Info("Stopping delay measurement task.");

            Hulk.Halt();

            ((DelayMeasurementPanel)panel).CleanUp();
            runningStopwatch.Stop();            
            
            DataLogger.CloseDataLog();
        }
        
        #endregion

        #region Internal Methods

        /// <summary>
        /// Starts the task. Called when the operator clicks the Go button.
        /// </summary>
        /// <param name="functionFilename">The filename of the CSV file containing the forcing function</param>
        /// <param name="selectedAmplitude">The amplitude of the forcing function</param>
        /// <param name="selectedOffset">???</param>
        internal void Go(string functionFilename, double selectedAmplitude, double selectedOffset)
        {
            MotionCommand newMotionCommand;

            logger.Debug("Enter: Go()");

            logger.Info("Starting delay measurement task.");

            amplitude = selectedAmplitude;

            // Move to the start location

            Hulk.SetCommandType(Hulk.CommandType.ModulusPosition);

            newMotionCommand = new MotionCommand();
            newMotionCommand.innerPosition = selectedOffset;
            newMotionCommand.outerPosition = 0.0;
            newMotionCommand.outerVelocity = Hulk.NORMAL_VELOCITY;
            newMotionCommand.innerVelocity = Hulk.NORMAL_VELOCITY;
            newMotionCommand.outerAcceleration = Hulk.NORMAL_ACCELERATION;
            newMotionCommand.innerAcceleration = Hulk.NORMAL_ACCELERATION;

            Hulk.SetCommand(newMotionCommand);
            Hulk.StartDefinedMove(false);
            
            while (Hulk.SystemStatus != Hulk.Status.Idling) {
                Thread.Sleep(100);
            }

            // Prepare for trial

            DataLogger.AcquireDataLog(Path.GetFileNameWithoutExtension(functionFilename) +
                "_Amplitude" + amplitude.ToString("F1") + "_Offset" + selectedOffset.ToString("F1") + "_data.csv", dataLogHeader);

            runningStopwatch.Reset();
            runningStopwatch.Start();

            // Start task

            Hulk.SetCommandType(Hulk.CommandType.Velocity);
            Hulk.StartTask();
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Logs the data from the round of calculations.
        /// </summary>
        /// <param name="time">Seconds since start of trial</param>
        /// <param name="currentMotion">Current motion</param>
        /// <param name="controlInput">Current forcing function</param>
        private void LogData(double time, MotionCommand currentMotion, MotionCommand calculatedMotion, ControlInput controlInput)
        {
            StringBuilder builder;
            int phase;

            builder = new StringBuilder();

            phase = 0;  
            
            if (Hulk.InnerAxis == Hulk.Axis.Roll) {
                builder.AppendFormat(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}\n",
                    time,
                    phase,
                    currentMotion.innerPosition.ToString("F6"),
                    currentMotion.outerPosition.ToString("F6"),
                    0,
                    currentMotion.innerVelocity.ToString("F6"),
                    currentMotion.outerVelocity.ToString("F6"),
                    0,
                    ((calculatedMotion != null) ? calculatedMotion.innerPosition.ToString("F6") : "0"),
                    ((calculatedMotion != null) ? calculatedMotion.outerPosition.ToString("F6") : "0"),
                    0,
                    ((calculatedMotion != null) ? calculatedMotion.innerVelocity.ToString("F6") : "0"),
                    ((calculatedMotion != null) ? calculatedMotion.outerVelocity.ToString("F6") : "0"),
                    0,
                    ((calculatedMotion != null) ? calculatedMotion.innerAcceleration.ToString("F6") : "0"),
                    ((calculatedMotion != null) ? calculatedMotion.outerAcceleration.ToString("F6") : "0"),
                    0,
                    controlInput.x.ToString("F6"),
                    controlInput.y.ToString("F6")
               );
            } else {
                builder.AppendFormat(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}\n",
                    time,
                    phase,
                    0,
                    currentMotion.outerPosition.ToString("F6"),
                    currentMotion.innerPosition.ToString("F6"),
                    0,
                    currentMotion.outerVelocity.ToString("F6"),
                    currentMotion.innerVelocity.ToString("F6"),
                    0,
                    ((calculatedMotion != null) ? calculatedMotion.outerPosition.ToString("F6") : "0"),
                    ((calculatedMotion != null) ? calculatedMotion.innerPosition.ToString("F6") : "0"),
                    0,
                    ((calculatedMotion != null) ? calculatedMotion.outerVelocity.ToString("F6") : "0"),
                    ((calculatedMotion != null) ? calculatedMotion.innerVelocity.ToString("F6") : "0"),
                    0,
                    ((calculatedMotion != null) ? calculatedMotion.outerAcceleration.ToString("F6") : "0"),
                    ((calculatedMotion != null) ? calculatedMotion.innerAcceleration.ToString("F6") : "0"),
                    controlInput.x.ToString("F6"),
                    controlInput.y.ToString("F6")
               );
            }

            DataLogger.AppendData(builder.ToString());
        }
        
        #endregion
    }
}
