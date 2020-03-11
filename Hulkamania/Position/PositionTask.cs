using log4net;
using System;
using System.Reflection;

namespace Brandeis.AGSOL.Hulkamania.Tasks.Position
{
    /// <summary>
    /// A task that allows the joystick to be used as a chair positioning device.
    /// </summary>
    public class PositionTask : HulkTask
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private const double POSITIONING_VELOCITY = 270.0;
        private const double POSITIONING_ACCELERATION = 450.0;

        #endregion

        #region Fields

        private double startInnerPosition;
        private double startOuterPosition;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the task.
        /// </summary>
        public PositionTask()
        {
            logger.Debug("Create: PositionTask");

            panel = new PositionPanel(this);
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Called from the main control loop whenever the task is running.
        /// DO NOT call this method directly from PositionTask or PositionPanel.
        /// </summary>
        public override void ContinueTask()
        {
            MotionCommand command;
            ControlInput joystickInput;

            // Continually command the chair to position itself according to joystick input

            joystickInput = InputController.JoystickInput;

            command = new MotionCommand {
                outerPosition = startOuterPosition + (joystickInput.y * 90.0),
                innerPosition = startInnerPosition + (joystickInput.x * 90.0),
            };

            Hulk.SetCommand(command);
            Hulk.ContinueTask();
        }

        /// <summary>
        /// Called from the main control loop whenever the task should be stopped.
        /// DO NOT call this method directly from PositionTask or PositionPanel.
        /// </summary>
        public override void StopTask()
        {
            logger.Debug("Enter: StopTask()");

            logger.Info("Stopping position task.");

            Hulk.Halt();

            ((PositionPanel)panel).CleanUp();
        }

        #endregion

        #region Internal Methods
        
        /// <summary>
        /// Starts the task. Called when the operator clicks the Go button.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        internal void Go()
        {
            MotionCommand newMotionCommand;

            logger.Debug("Enter: Go()");

            Hulk.Halt();

            startOuterPosition = Hulk.CurrentMotion.outerPosition;
            startInnerPosition = Hulk.CurrentMotion.innerPosition;

            logger.Info("Starting position task with center at OUTER=" + startOuterPosition.ToString("F1") + 
                    " INNER=" + startInnerPosition.ToString("F1") + ".");

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
    }
}
