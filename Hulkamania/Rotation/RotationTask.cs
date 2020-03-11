using log4net;
using System;
using System.Reflection;
using System.Threading;

namespace Brandeis.AGSOL.Hulkamania.Tasks.Rotation
{
    /// <summary>
    /// A task that allows the chair to be rotated by the operator with a given
    /// velocity and acceleration, optionally to a given position.
    /// </summary>
    public class RotationTask : HulkTask
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private DateTime stopTime;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the task.
        /// </summary>
        public RotationTask()
        {
            logger.Debug("Create: RotationTask");

            stopTime = DateTime.MinValue;

            panel = new RotationPanel(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called from the main control loop whenever the task is running.
        /// DO NOT call this method directly from RotationTask or RotationPanel.
        /// </summary>
        public override void ContinueTask()
        {
            if ((stopTime != DateTime.MinValue) && (DateTime.Now.CompareTo(stopTime) >= 0)) {
                // Duration limit reached.
                Hulk.StopTask();
            } else {
                Hulk.ContinueTask();
            }
        }

        /// <summary>
        /// Called from the main control loop whenever the task should be stopped.
        /// DO NOT call this method directly from RotationTask or RotationPanel.
        /// </summary>
        public override void StopTask()
        {
            logger.Debug("Enter: StopTask()");

            logger.Info("Stopping rotation task.");

            Hulk.Halt();

            ((RotationPanel)panel).CleanUp();
        }

        #endregion

        #region Internal Methods
        
        /// <summary>
        /// Starts the task. Called when the operator clicks the Go button.
        /// </summary>
        /// <param name="command">The movement command (velocity, acceleration)</param>
        /// <param name="duration">The duration of movement</param>
        internal void Go(MotionCommand command, double duration)
        {
            logger.Debug("Enter: Go(MotionCommand, double)");

            if (command.innerPosition == Hulk.ANY_MOTION) {

                // Moving with given velocity/acceleration and (optionally) duration.
                
                logger.Info("Starting rotation task with given velocity, acceleration, and duration.");

                Hulk.SetCommandType(Hulk.CommandType.Velocity);

                if (duration == -1.0) {
                    stopTime = DateTime.MinValue;
                } else {
                    stopTime = DateTime.Now.AddSeconds(duration);
                }

                Hulk.SetCommand(command);
                Hulk.StartTask();

            } else {

                // Moving to the given position.
                
                logger.Info("Starting rotation task to given position.");

                Hulk.SetCommandType(Hulk.CommandType.ModulusPosition);
                Hulk.SetCommand(command);
                Hulk.StartDefinedMove(false);
            }
        }

        /// <summary>
        /// Gradually stops any rotation of the HULK, using the currently specified 
        /// deceleration parameters. Called when the operator clicks the Soft Stop button.
        /// </summary>
        /// <param name="stateInfo">Not used. Here so that the method can be called by a worker thread.</param>
        internal void SoftStop(Object stateInfo)
        {
            MotionCommand newCommand;

            logger.Debug("Enter: SoftStop(Object)");

            logger.Info("Decelerating.");

            Hulk.SetCommandType(Hulk.CommandType.Velocity);

            newCommand = new MotionCommand();
            newCommand.innerVelocity = 0;
            newCommand.outerVelocity = 0;
            newCommand.innerAcceleration = Hulk.NORMAL_ACCELERATION;
            newCommand.outerAcceleration = Hulk.NORMAL_ACCELERATION;

            Hulk.SetCommand(newCommand);

            while ((Hulk.CurrentMotion.innerVelocity > 0.1) || (Hulk.CurrentMotion.outerVelocity > 0.1)) {
                Thread.Sleep(100);
            }

            Hulk.StopTask();
        }

        #endregion
    }
}
