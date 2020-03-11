using Brandeis.AGSOL.FlexMotion;
using log4net;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Diagnostics;
using System.Configuration;

namespace Brandeis.AGSOL.Hulkamania
{

    /// <summary>
    /// The motion controller board.
    /// </summary>
    internal  class MotionControllerDummy : MotionController
    {
        private enum CommandType
        {
            ModulusPosition,
            Velocity
        };

        private struct MotionData
        {
            public MotionCommand   command;
            public long milliSecondsElapsed;

            public MotionData(MotionCommand c, long t)
            {
                command = c;
                milliSecondsElapsed = t;
            }
        }

        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private const ushort UNUSED_U16_PARAMETER = 0;

        private const byte BOARD_ID = 2;

        private const byte OUTER_AXIS = Motion.NIMC_AXIS1;
        private const byte INNER_AXIS = Motion.NIMC_AXIS2;
        private const ushort AXIS_BITMAP = 0x6;

        private const byte VECTOR_SPACE = Motion.NIMC_VECTOR_SPACE2;

        private const byte PRIMARY_FEEDBACK_OUTER = Motion.NIMC_ENCODER1;
        private const byte PRIMARY_FEEDBACK_INNER = Motion.NIMC_ENCODER2;

        private const byte IMMEDIATE_INPUT_VECTOR = 0xFF;

        private const uint CONTROLLER_STEPS_OUTER = 4096;
        private const uint CONTROLLER_STEPS_INNER = 4096;

        private const uint ENCODER_STEPS_OUTER = 131072;
        private const uint ENCODER_STEPS_INNER = 131072;

        private const double HOME_OUTER = 172.5;
        private const double HOME_INNER = 101.0;

        #endregion

        #region Fields

        private int delayInMs = 0;
        private static long lastHulkStatusUpdate = 0;
           
        private List<MotionData> commandBuffer = new List<MotionData>();
        private Stopwatch stopwatch = new Stopwatch();

        private MotionCommand currentMotion;
        private  MotionCommand targetMotion;
        private  MotionCommand zeroReferenceMotion;

        private  CommandType currentCommandType;
        private  System.Timers.Timer updateTimer;

        private Object lockObject = new Object();
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the motion controller.
        /// </summary>
        internal MotionControllerDummy()
        {
            logger.Debug("Create: MotionControllerDummy");

            delayInMs = int.Parse(ConfigurationManager.AppSettings["DummyMotionControllerDelayInMs"]);
       
            stopwatch.Start();

            MotionCommand newMotion = new MotionCommand();
            newMotion.innerPosition = 0;
            newMotion.outerPosition = 0;
            newMotion.innerAcceleration = Hulk.NORMAL_ACCELERATION;
            newMotion.outerAcceleration = Hulk.NORMAL_ACCELERATION;
            newMotion.innerVelocity = 0;
            newMotion.outerVelocity = 0;

            _processCommand(newMotion);

            targetMotion = new MotionCommand();
            targetMotion.innerPosition = 0;
            targetMotion.outerPosition = 0;
            targetMotion.innerVelocity = 0;
            targetMotion.outerVelocity = 0;
            targetMotion.innerAcceleration = 0;
            targetMotion.outerAcceleration = 0;

            zeroReferenceMotion = new MotionCommand();
            zeroReferenceMotion.innerPosition = 0;
            zeroReferenceMotion.outerPosition = 0;
            zeroReferenceMotion.innerVelocity = 0;
            zeroReferenceMotion.outerVelocity = 0;
            zeroReferenceMotion.innerAcceleration = 0;
            zeroReferenceMotion.outerAcceleration = 0;

            currentCommandType = CommandType.Velocity;

            updateTimer = new System.Timers.Timer(1);
            updateTimer.Elapsed += new ElapsedEventHandler(updateTimer_Elapsed);
            updateTimer.Enabled = true;
            
            InitializeBoard();

            lastHulkStatusUpdate = stopwatch.ElapsedMilliseconds;
       }

        /// <summary>
        /// Tells the motion controller to update its internal state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _updateHulkState();
        }

        /// <summary>
        /// Updates the internal state of the motion controller
        /// </summary>
        /// <param name="interval"></param>


        private void _updateHulkState()
        {
            long interval = stopwatch.ElapsedMilliseconds - lastHulkStatusUpdate;
            lastHulkStatusUpdate = lastHulkStatusUpdate + interval;

            if (
                (Hulk.SystemStatus == Hulk.Status.Idling) ||
                (Hulk.SystemStatus == Hulk.Status.Homing) ||
                (Hulk.SystemStatus == Hulk.Status.Initializing) ||
                (Hulk.SystemStatus == Hulk.Status.NeedsHoming) ||
                (Hulk.SystemStatus == Hulk.Status.Offline) ||
                (Hulk.SystemStatus == Hulk.Status.Stopping)
                )
            {
                return;
            }

            // figure out what the HULK current motion is
            MotionCommand cm = currentMotion;
            if (delayInMs != 0)
            {
                lock (lockObject)
                {
                    if ((commandBuffer.Count == 0))
                    {
                        logger.Warn("_updateHulkState() empty commandbuffer detected, while the delayInMs is " + delayInMs.ToString() + ". Using currentMotion to determine newMotion.");
                    }
                    else
                    {
                        cm = commandBuffer[commandBuffer.Count - 1].command;
                    }
                }
            }

            MotionCommand newMotion = new MotionCommand(cm);

            switch (currentCommandType)
            {
                case CommandType.Velocity:
                    // velocity command; just apply the velocity
                    newMotion.innerVelocity = targetMotion.innerVelocity;
                    newMotion.outerVelocity = targetMotion.outerVelocity;

                    newMotion.innerPosition += newMotion.innerVelocity * interval * 1.0f / 1000;
                    newMotion.outerPosition += newMotion.outerVelocity * interval * 1.0f / 1000;

                    break;

                case CommandType.ModulusPosition:
                    {
                        // modulus position command; figure out what the new position should be

                        // what is the current difference in position
                        double deltaPosInner = targetMotion.innerPosition - newMotion.innerPosition;
                        double deltaPosOuter = targetMotion.outerPosition - newMotion.outerPosition;

                        // what should the velocity be
                        double deltaVelInner = Math.Sign(deltaPosInner) * targetMotion.innerVelocity * interval * 1.0f / 1000;
                        double deltaVelOuter = Math.Sign(deltaPosOuter) * targetMotion.outerVelocity * interval * 1.0f / 1000;

                        // apply velocity
                        if (Math.Abs(deltaPosInner) > Math.Abs(deltaVelInner))
                        {
                            newMotion.innerPosition += deltaVelInner;
                            newMotion.innerVelocity = targetMotion.innerVelocity;
                        }
                        else
                        {
                            newMotion.innerPosition = targetMotion.innerPosition;
                            targetMotion.innerVelocity = 0;
                            newMotion.innerVelocity = 0;
                        }

                        // apply velocity
                        if (Math.Abs(deltaPosOuter) > Math.Abs(deltaVelOuter))
                        {
                            newMotion.outerPosition += deltaVelOuter;
                            newMotion.outerVelocity = targetMotion.outerVelocity;
                        }
                        else
                        {
                            newMotion.outerPosition = targetMotion.outerPosition;
                            targetMotion.outerVelocity = 0;
                            newMotion.outerVelocity = 0;
                        }
                    }
                    break;
            }

            _processCommand(newMotion);

        }

        #endregion

        #region Properties
        /// <summary>
        /// Current motion of the HULK. Read from the board each control loop cycle.
        /// </summary>
        internal override MotionCommand CurrentMotion
        {
            get
            {
                return currentMotion;
            }
        }
        #endregion

        #region Internal Methods

        #region Motion Commands



        /// <summary>
        /// Starts the homing process. The HULK will seek out the reference locations for each axis.
        /// </summary>
        internal  override void FindReference()
        {
        }

        /// <summary>
        /// Checks whether the homing reference locations have been found.
        /// </summary>
        /// <returns>True if references have been found</returns>
        internal override bool HasFoundReference()
        {
            return true;
        }

        /// <summary>
        /// Checks whether the ongoing move command has finished.
        /// </summary>
        /// <returns>True if move is finished, false otherwise</returns>
        internal override bool IsMoveComplete()
        {
            switch (currentCommandType)
            {
                case CommandType.Velocity:
                    return true;
                case CommandType.ModulusPosition:
                    return ((Hulk.LastCommand.innerPosition == currentMotion.innerPosition) && (Hulk.LastCommand.outerPosition == currentMotion.outerPosition));
            }

            return false;
        }

        /// <summary>
        /// Sets the desired acceleration for the inner and outer axes.
        /// </summary>
        /// <param name="command">The motion command, containing the accelerations</param>
        internal override void SetAcceleration(MotionCommand command)
        {
            targetMotion.innerAcceleration = command.innerAcceleration;
            targetMotion.outerAcceleration = command.outerAcceleration;
        }

        /// <summary>
        /// Sets the desired position for the inner and outer axes.
        /// </summary>
        /// <param name="command">The motion command, containing the positions</param>
        internal override void SetPosition(MotionCommand command)
        {
            targetMotion.innerPosition = command.innerPosition;
            targetMotion.outerPosition = command.outerPosition;
        }

        /// <summary>
        /// Sets the desired velocity for the inner and outer axes.
        /// </summary>
        /// <param name="command">The motion command, containing the velocities</param>
        internal override void SetVelocity(MotionCommand command)
        {
            targetMotion.innerVelocity = command.innerVelocity;
            targetMotion.outerVelocity = command.outerVelocity;
        }

        /// <summary>
        /// Starts the HULK moving. Motion must be pre-programmed using SetCommandType and SetCommand.
        /// </summary>
        internal override void StartMotion()
        {
        }

        /// <summary>
        /// Stops all motors.
        /// </summary>
        internal override void StopMotion()
        {
            MotionCommand newMotion = new MotionCommand(CurrentMotion);
            newMotion.innerVelocity = 0;
            newMotion.outerVelocity = 0;
            newMotion.innerAcceleration = 0;
            newMotion.outerAcceleration = 0;

            _processCommand(newMotion);

            targetMotion.innerVelocity = 0;
            targetMotion.outerVelocity = 0;
            targetMotion.innerAcceleration = 0;
            targetMotion.outerAcceleration = 0;
        }

        /// <summary>
        /// Sets up the HULK to use position commands in a vector space.
        /// </summary>
        internal override void UseModulusPositionCommands()
        {
            currentCommandType = CommandType.ModulusPosition;
        }

        /// <summary>
        /// Sets up the HULK to use velocity commands.
        /// </summary>
        internal override void UseVelocityCommands()
        {
            currentCommandType = CommandType.Velocity;
        }

        /// <summary>
        /// Use the current position as the zero reference for all future position coordinates.
        /// </summary>
        internal override void ZeroReference()
        {
            MotionCommand currentMotion = CurrentMotion;
            zeroReferenceMotion.innerPosition = currentMotion.innerPosition;
            zeroReferenceMotion.outerPosition = currentMotion.outerPosition;
            zeroReferenceMotion.innerVelocity = currentMotion.innerVelocity;
            zeroReferenceMotion.outerVelocity = currentMotion.outerVelocity;
            zeroReferenceMotion.innerAcceleration = currentMotion.innerAcceleration;
            zeroReferenceMotion.outerAcceleration = currentMotion.outerAcceleration;
        }

        #endregion

        #region Status Updates

        
        /// <summary>
        /// Reads the current motion of the Hulk from its encoders.
        /// </summary>
        /// <param name="updateRate">The number of motion reads in the past second</param>
        internal override void ReadCurrentMotion(int updateRate)
        {
            Thread.Sleep(20);
        }

        #endregion

        #endregion

        #region Private Methods

        #region Initialization

        /// <summary>
        /// Initialize the motion control board.
        /// This must be called once before any movement commands are given.
        /// </summary>
        private  void InitializeBoard()
        {
            logger.Info("Motion controller: simulator initialized.");
            logger.Info("Motion controller: simulating a delay of " + delayInMs.ToString() + " ms");
        }

        #endregion

        #region Error Checking

        /// <summary>
        /// Checks whether the motion board has reported any modal (runtime) errors. If there is an error, 
        /// the motion board is immediately stopped and the application is signaled to cleanup and exit.
        /// </summary>
        internal override void CheckModalErrors()
        {
        }
        #endregion

        #region Helpers
        
        /// <summary>
        /// Make sure that the appropriate amount of commands is in the buffer, to simulate the HULK delay
        /// </summary>
        private void _processCommand(MotionCommand c)
        {
            lock (lockObject)
            {

                if (delayInMs == 0)
                {
                    commandBuffer.Clear();
                    currentMotion = c;
                }

                commandBuffer.Add(new MotionData(c, stopwatch.ElapsedMilliseconds));

                if (delayInMs != 0)
                {
                    bool doRemove = false;
                    int startIndex = 0;
                    while ((commandBuffer[commandBuffer.Count - 1].milliSecondsElapsed - commandBuffer[startIndex].milliSecondsElapsed) > delayInMs)
                    {
                        startIndex = startIndex + 1;
                        doRemove = true;
                    }
                    if (doRemove)
                    {
                        commandBuffer.RemoveRange(0, startIndex);
                    }

                    if (commandBuffer.Count != 0)
                    {
                        currentMotion = commandBuffer[0].command;
                    }
                    else
                    {
                        currentMotion = c;
                    }

                    // Debug code to double check that the delay implementation works as expected
                    //             double delay = (commandBuffer[commandBuffer.Count - 1].milliSecondsElapsed - commandBuffer[0].milliSecondsElapsed);
                    //             System.Diagnostics.Debug.WriteLine("Number of commands in buffer: " + commandBuffer.Count.ToString() + " delay " + delay.ToString());
                }
            }
        }

        #endregion
        #endregion
    }
}
