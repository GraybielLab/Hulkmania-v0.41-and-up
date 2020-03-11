using log4net;
using System;
using System.Reflection;
using System.Threading;

using System.Net.Sockets;
using Brandeis.AGSOL.Network;

namespace Brandeis.AGSOL.Hulkamania.Tasks.UnrealLandscapesDemo
{
    /// <summary>
    /// A task that allows the chair to be rotated by the operator with a given
    /// velocity and acceleration, optionally to a given position.
    /// </summary>
    public class UnrealLandscapesDemoTask : HulkTask
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private Vector3 unrealPosition;
        private Vector3 unrealVelocity;
        private Vector3 unrealAcceleration;
        private Vector3 unrealForce;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the task.
        /// </summary>
        public UnrealLandscapesDemoTask()
        {
            logger.Debug("Create: UnrealLandscapesDemoTask");

            panel = new UnrealLandscapesDemoPanel(this);
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Called from the main control loop whenever a network command is received
        /// </summary>
        public override void ProcessNetworkCommand(ICommand c, INetworkServer server, Socket handler)
        {
            // USER command == data from UE4
            if (c.CommandType == 1000)
            {
                // parameter 0 == position
                unrealPosition = c.getParameterAsVector3(0);
                unrealVelocity = c.getParameterAsVector3(1);
                unrealAcceleration = c.getParameterAsVector3(2);
                unrealForce = c.getParameterAsVector3(3);

                ((UnrealLandscapesDemoPanel)panel).setUnrealData(unrealPosition, unrealVelocity, unrealAcceleration, unrealForce);

            }
        }



        /// <summary>
        /// Called from the main control loop whenever the task is running.
        /// DO NOT call this method directly from UnrealLandscapesDemoTask or RotationPanel.
        /// </summary>
        public override void ContinueTask()
        {
             Hulk.ContinueTask();

            _transmitStatusToNetworkClients();
        }

        /// <summary>
        /// Called from the main control loop whenever the task should be stopped.
        /// DO NOT call this method directly from UnrealLandscapesDemoTask or RotationPanel.
        /// </summary>
        public override void StopTask()
        {
            logger.Debug("Enter: StopTask()");

            logger.Info("Stopping rotation task.");

            Hulk.Halt();

            ((UnrealLandscapesDemoPanel)panel).CleanUp();
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

                logger.Info("Starting UnrealLandscapesDemoTask with given duration.");

                Hulk.SetCommandType(Hulk.CommandType.Velocity);

                Hulk.SetCommand(command);
                Hulk.StartTask();

            } else {

                // Moving to the given position.

                logger.Info("Starting UnrealLandscapesDemoTask to given position.");

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

        #region Network
        // -------------------------------------------------------------------------------------------------------------------------
        private void _transmitStatusToNetworkClients()
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

        #endregion
    }
}
