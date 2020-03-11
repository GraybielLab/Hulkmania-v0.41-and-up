using log4net;
using System;
using System.Configuration;
using System.Reflection;
using System.Threading;

using Brandeis.AGSOL.Network;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// The HULK device.
    /// </summary>
    public static class Hulk
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        public const double ANY_MOTION = Double.PositiveInfinity;

        public const double NORMAL_VELOCITY = 15.0;
        public const double NORMAL_ACCELERATION = 15.0;

        private const double HOMING_VELOCITY = 30.0;
        private const double HOMING_ACCELERATION = 30.0;

        private const double MAYDAY_VELOCITY = 40.0;
        private const double MAYDAY_ACCELERATION = 30.0;

        private static readonly MountType mount;

        #endregion

        #region Enums

        /// <summary>
        /// Type of movement for a given axis.
        /// </summary>
        public enum Axis {
            Pitch,
            Roll,
            Yaw
        }
        
        /// <summary>
        /// Type of command to the HULK.
        /// </summary>
        public enum CommandType {
            //AbsolutePosition,   // Commands will be desired positions, using independent axes
            ModulusPosition,    // Commands will be desired positions, using vector space
            Velocity            // Commands will be desired velocities
        }

        /// <summary>
        /// Current homing phase.
        /// </summary>
        public enum HomingPhase {
            Unhomed,            // Homing process has not been started
            Seeking,            // Hulk is being rotated to find the home positioning markers
            Complete            // Homing process is complete
        }

        /// <summary>
        /// Current chair positioning.
        /// </summary>
        public enum MountType {
            Back,               // Inner axis attached to back of chair
            Seat,               // Inner axis attached to bottom of chair
        }

        /// <summary>
        /// Current status of the Hulk.
        /// </summary>
        public enum Status {
            Initializing,       // Still being initialized, and will not respond to commands
            NeedsHoming,        // Motors online but require homing before other commands can be given
            Homing,             // Calibrating the encoders
            Offline,            // Initialized but offline
            Idling,             // Waiting for a command
            Executing,          // Executing a basic movement command
            Task,               // Running a task (selected by the user in the GUI)
            TaskExecuting,      // Executing a basic movement command within a task (task will be resumed after move is complete)
            Stopping            // Stopping a task (selected by the user in the GUI)
        }

        #endregion

        #region Fields

        private static CommandType currentCommandType;

        private static DateTime frameTime;

        private static HomingPhase homingPhase;

        private static MotionCommand lastCommand;

        private static MotionCommand backCommand;
        private static MotionCommand frontCommand;
        private static MotionCommand hangCommand;
        private static MotionCommand loadingCommand;
        private static MotionCommand maydayCommand;
        private static MotionCommand uprightCommand;

        private static Status status;

        private static int updateCounter;
        private static int updateRate;

        private static bool keepRunning;

        #endregion

        #region Properties

        /// <summary>
        /// Motion command for moving to the backwards facing position.
        /// </summary>
        public static MotionCommand BackCommand
        {
            get
            {
                return backCommand;
            }
        }

        /// <summary>
        /// Motion command for moving to the front facing position.
        /// </summary>
        public static MotionCommand FrontCommand
        {
            get
            {
                return frontCommand;
            }
        }

        /// <summary>
        /// How the chair is mounted (back or seat).
        /// </summary>
        public static MountType ChairMount
        {
            get {
                return mount;
            }
        }
        
        /// <summary>
        /// Type of commands being given to the HULK.
        /// </summary>
        internal static CommandType CurrentCommandType
        {
            get {
                return currentCommandType;
            }
        }

        /// <summary>
        /// Current motion of the HULK. Read from the board each control loop cycle.
        /// </summary>
        public static MotionCommand CurrentMotion
        {
            get {
                return MotionController.Instance.CurrentMotion;
            }
        }

        /// <summary>
        /// The axis (yaw / pitch / roll) currently controlled by the inner axis.
        /// </summary>
        public static Axis InnerAxis
        {
            get {
                return (mount == MountType.Back) ? Axis.Roll : Axis.Yaw;
            }
        }
        
        /// <summary>
        /// Flag for whether the control loop should continue.
        /// Set to false to abort the movement.
        /// </summary>
        internal static bool KeepRunning
        {
            set {
                keepRunning = value;
            }
        }

        /// <summary>
        /// Last motion commands sent to the HULK.
        /// </summary>
        public static MotionCommand LastCommand
        {
            get {
                return lastCommand;
            }
            internal set {
                lastCommand = value;
            }
        }

        /// <summary>
        /// The axis (yaw / pitch / roll) currently controlled by the outer axis.
        /// </summary>
        public static Axis OuterAxis
        {
            get {
                return Axis.Pitch;
            }
        }
        
        /// <summary>
        /// What the HULK is currently doing (executing a move, homing, balancing).
        /// </summary>
        public static Status SystemStatus
        {
            get {
                return status;
            }
        }

        /// <summary>
        /// The number of times the control loop was executed during the previous second.
        /// </summary>
        public static int UpdateRate
        {
            get {
                return updateRate;
            }
        }

        /// <summary>
        /// Motion command for moving to the upright, facing operator position.
        /// </summary>
        public static MotionCommand UprightCommand
        {
            get {
                return uprightCommand;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the device settings.
        /// </summary>
        static Hulk()
        {
            logger.Debug("Static create: Hulk");

            DataController.Instance.PCReady = false;
            homingPhase = HomingPhase.Unhomed;

            mount = (ConfigurationManager.AppSettings["MountType"] == "Back") ? 
                MountType.Back : MountType.Seat;

            InitializeMotionStates();

            DataController.Instance.StartTasks();
            InputController.AcquireJoystick();

            status = Status.Initializing;
        }

        #endregion

        #region Public Methods

        #region Commands

        /// <summary>
        /// Moves the Hulk to the BACK position.
        /// </summary>
        /// <param name="stateInfo">Not used. Here so that the method can be called by a worker thread.</param>
        public static void Back(Object stateInfo)
        {
            logger.Debug("Enter: Back(Object)");

            // Safely stop any ongoing task.
            if ((status == Status.Task) || (status == Status.TaskExecuting))
            {
                status = Status.Stopping;
                while (status != Status.Idling)
                {
                    Thread.Sleep(100);
                }
            }

            Halt();

            logger.Info("Moving to BACK position.");

            SetCommandType(CommandType.ModulusPosition);
            SetCommand(backCommand);

            MotionController.Instance.StartMotion();
            status = Status.Executing;
        }

        /// <summary>
        /// Moves the Hulk to the FRONT position.
        /// </summary>
        /// <param name="stateInfo">Not used. Here so that the method can be called by a worker thread.</param>
        public static void Front(Object stateInfo)
        {
            logger.Debug("Enter: Back(Object)");

            // Safely stop any ongoing task.
            if ((status == Status.Task) || (status == Status.TaskExecuting))
            {
                status = Status.Stopping;
                while (status != Status.Idling)
                {
                    Thread.Sleep(100);
                }
            }

            Halt();

            logger.Info("Moving to FRONT position.");

            SetCommandType(CommandType.ModulusPosition);
            SetCommand(frontCommand);

            MotionController.Instance.StartMotion();
            status = Status.Executing;
        }

        /// <summary>
        /// Checks whether commands should be sent.
        /// If the safety system is on (green light) then no command should be sent.
        /// </summary>
        public static bool CheckCommandAllowed()
        {
            if (DataController.Instance.IsLightGreen) {
                logger.Warn("Light must be red to continue.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Continues running an experimenter-defined task.
        /// </summary>
        public static void ContinueTask()
        {
            MotionController.Instance.StartMotion();
        }

        /// <summary>
        /// Immediately halts any ongoing motion.
        /// </summary>
        public static void Halt()
        {
            MotionCommand newMotionCommand;

            logger.Debug("Enter: Halt()");

            MotionController.Instance.StopMotion();

            newMotionCommand = new MotionCommand {
                outerPosition = ANY_MOTION,
                innerPosition = ANY_MOTION,
                outerVelocity = 0,
                innerVelocity = 0,
                outerAcceleration = 0,
                innerAcceleration = 0
            };
            lastCommand = newMotionCommand;
        }

        /// <summary>
        /// Moves the Hulk to the HANG position. This position produces the least movement if the power is cut.
        /// </summary>
        /// <param name="stateInfo">Not used. Here so that the method can be called by a worker thread.</param>
        public static void Hang(Object stateInfo)
        {
            logger.Debug("Enter: Hang(Object)");

            // Safely stop any ongoing task.
            if ((status == Status.Task) || (status == Status.TaskExecuting)) {
                status = Status.Stopping;
                while (status != Status.Idling) {
                    Thread.Sleep(100);
                }
            }

            Halt();

            logger.Info("Moving to HANG position.");

            SetCommandType(CommandType.ModulusPosition);
            SetCommand(hangCommand);

            MotionController.Instance.StartMotion();
            status = Status.Executing;
        }

        /// <summary>
        /// Tells the HULK to hold position. 
        /// Used during initialization while computer-PLC communications are being established,
        /// and at any time for emergency stop.
        /// WARNING: Using as emergency stop will leave any ongoing tasks incomplete.
        /// </summary>
        public static void Hold()
        {
            logger.Debug("Enter: Hold()");

            DataController.Instance.PCReady = true;

            Halt();

            if (status == Status.Offline) {
                homingPhase = HomingPhase.Unhomed;
            }

            if (homingPhase == HomingPhase.Complete) {
                status = Status.Idling;
            } else {
                status = Status.NeedsHoming;
            }

            logger.Info("Motors online. Holding position.");
        }

        /// <summary>
        /// Sets up the homing task. The task will run during subsequent rounds of the control loop,
        /// until the calibration is complete.
        /// </summary>
        public static void Home()
        {
            MotionCommand newMotionCommand;

            logger.Debug("Enter: Home()");

            logger.Info("Homing.");

            homingPhase = HomingPhase.Seeking;

            newMotionCommand = new MotionCommand {
                outerAcceleration = HOMING_ACCELERATION,
                innerAcceleration = HOMING_ACCELERATION,
                outerVelocity = HOMING_VELOCITY,
                innerVelocity = HOMING_VELOCITY
            };

            SetCommand(newMotionCommand);

            MotionController.Instance.FindReference();

            status = Status.Homing;
        }

        /// <summary>
        /// Tells the HULK to take the motors offline.
        /// </summary>
        public static void Kill()
        {
            logger.Debug("Enter: Kill()");

            DataController.Instance.PCReady = false;
            status = Status.Offline;

            logger.Info("Motors offline.");
        }

        /// <summary>
        /// Moves the Hulk to the LOADING position. This position is the simplest for loading a participant into the chair.
        /// </summary>
        /// <param name="stateInfo">Not used. Here so that the method can be called by a worker thread.</param>
        public static void Loading(Object stateInfo)
        {
            logger.Debug("Enter: Loading(Object)");

            // Safely stop any ongoing task.
            if ((status == Status.Task) || (status == Status.TaskExecuting)) {
                status = Status.Stopping;
                while (status != Status.Idling) {
                    Thread.Sleep(100);
                }
            }

            Halt();

            logger.Info("Moving to LOADING position.");

            SetCommandType(CommandType.ModulusPosition);          
            SetCommand(loadingCommand);

            MotionController.Instance.StartMotion();
            status = Status.Executing;
        }

        /// <summary>
        /// Moves the Hulk to the MAYDAY position. Triggered by experimental participant in cases of motion sickness.
        /// </summary>
        /// <param name="stateInfo">Not used. Here so that the method can be called by a worker thread.</param>
        public static void Mayday(Object stateInfo)
        {
            logger.Debug("Enter: Mayday(Object)");

            // Safely stop any ongoing task.
            if ((status == Status.Task) || (status == Status.TaskExecuting)) {
                status = Status.Stopping;
                while (status != Status.Idling) {
                    Thread.Sleep(100);
                }
            }

            Halt();

            logger.Info("Moving to MAYDAY position.");

            SetCommandType(CommandType.ModulusPosition);
            SetCommand(maydayCommand);

            MotionController.Instance.StartMotion();
            status = Status.Executing;
        }

        /// <summary>
        /// Sets the velocity of the inner and outer axes of the HULK.
        /// </summary>
        /// <param name="command">Motion command, containing the velocities</param>
        public static void SetCommand(MotionCommand command)
        {
            if ((command.innerAcceleration != ANY_MOTION) || (command.outerAcceleration != ANY_MOTION)) {
                MotionController.Instance.SetAcceleration(command);
            }
            if ((command.innerVelocity != ANY_MOTION) || (command.outerVelocity != ANY_MOTION)) {
                MotionController.Instance.SetVelocity(command);
            }
            if ((command.innerPosition != ANY_MOTION) || (command.outerPosition != ANY_MOTION)) {
                MotionController.Instance.SetPosition(command);
            }
            lastCommand = command;
        }

        /// <summary>
        /// Sets the type of commands that will be sent to the HULK.
        /// </summary>
        /// <param name="commandType">Type of command (velocity, position, etc)</param>
        public static void SetCommandType(CommandType commandType)
        {
            logger.Debug("Enter: SetCommandType(CommandType)");

            currentCommandType = commandType;

            switch (commandType) {
                case CommandType.ModulusPosition:
                    MotionController.Instance.UseModulusPositionCommands();
                    break;
                case CommandType.Velocity:
                    MotionController.Instance.UseVelocityCommands();
                    break;
            }
        }

        /// <summary>
        /// Starts running a predefined move.
        /// </summary>
        /// <param name="returnToTask">True to tell the control loop to resume the task after completion of the move</param>
        public static void StartDefinedMove(bool returnToTask)
        {
            logger.Debug("Enter: StartDefinedMove(bool)");

            MotionController.Instance.StartMotion();
            if (returnToTask) {
                status = Status.TaskExecuting;
            } else {
                status = Status.Executing;
            }
        }

        /// <summary>
        /// Starts running an experimenter-defined task.
        /// </summary>
        public static void StartTask()
        {
            logger.Debug("Enter: StartTask()");

            status = Status.Task;
        }

        /// <summary>
        /// Stops running an experimenter-defined task.
        /// </summary>
        public static void StopTask()
        {
            logger.Debug("Enter: StopTask()");

            status = Status.Stopping;
        }

        #endregion

        #endregion

        #region Internal Methods

        #region Control Loop

        /// <summary>
        /// The main control loop.
        /// </summary>
        internal static void ControlHulk()
        {
            bool joystickMessageGiven;
            bool lightMessageGiven;

            logger.Debug("Enter: ControlHulk()");

            logger.Info("Control loop starting.");

            // Check frames-per-second once every second
            frameTime = DateTime.Now.AddSeconds(1.0);
            updateCounter = 0;
            updateRate = 0;

            joystickMessageGiven = false;
            lightMessageGiven = false;

            // Do the control loop until a signal is sent to stop
            keepRunning = true;
            do {

                // Record the frames-per-second
                if (DateTime.Now.CompareTo(frameTime) >= 0) {

                    updateRate = updateCounter;
                    frameTime = DateTime.Now.AddSeconds(1.0);
                    updateCounter = 1;

                } else {
                    updateCounter++;
                }

                // Make sure that joystick input is possible



                if (InputController.IsJoystickConnected) {
                    joystickMessageGiven = false;
                } else {
                    InputController.AcquireJoystick();
                    if (!InputController.IsJoystickConnected) {

                        if (!joystickMessageGiven)
                        {
                            // if no joystick connection is detected, the following code needs to be executed just once. Piggyback on the joystickMessageGiven functionality to do this, 
                            // since that message is logged once as well

                            // if a task is currently executing, halt execution 
                            switch (status)
                            {
                                case Status.Task:
                                case Status.TaskExecuting:
                                case Status.Executing:
                                    Hulk.Halt();
                                    // Stop the currently executing task to prevent the task from continuing where it left off once the joystick is plugged back in
                                    if (AppMain.CurrentTask != null)
                                    {
                                        AppMain.CurrentTask.StopTask();
                                    }
                                    status = Status.Idling;
                                    break;
                                default:
                                    break;
                            }

                            // put the HULK in the loading position
                            Hulk.Loading(null);

                            logger.Warn("Joystick must be plugged in to continue.");
                            joystickMessageGiven = true;
                        }

                           continue;
                    }
                }


                // See if safety lights show readiness for commands
                if (DataController.Instance.IsLightGreen) {

                    Hulk.Halt();

                    if (!lightMessageGiven) {
                        logger.Warn("Light must be red to continue.");
                        lightMessageGiven = true;
                    }

                    Thread.Sleep(250);

                    continue;

                } else {
                    if (lightMessageGiven) {
                        logger.Info("Red light detected.");
                        lightMessageGiven = false;
                    }
                }

                // Is this program ready to send commands?
                if (!DataController.Instance.PCReady) {
                    Thread.Sleep(250);
                    continue;
                }

                // See if PLC is still ready to receive commands
                if (!DataController.Instance.PLCReady) {
                    Thread.Sleep(250);
                    continue;
                }

                // If needed, initiate mayday
                if (DataController.Instance.IsMaydayPressed) {
                    //Mayday(null);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Mayday));
                }

                // Check if the motion control board is reporting an error
                MotionController.Instance.CheckModalErrors();

                // Notify programs of current HULK motion
                MotionController.Instance.ReadCurrentMotion(updateRate);

                if (AppMain.CurrentTask != null) {
                    AppMain.CurrentTask.LogBasicData();
                    AppMain.CurrentTask.UpdateNetworkClients();
                }

                switch (status) {
                    case Status.Task:
                        AppMain.CurrentTask.ContinueTask();
                        break;
                    case Status.TaskExecuting:
                        if (MotionController.Instance.IsMoveComplete()) {
                            status = Status.Task;
                        }
                        break;
                    case Status.Stopping:
                        AppMain.CurrentTask.StopTask();
                        status = Status.Idling;
                        break;
                    case Status.Homing:
                        ContinueHoming();
                        break;
                    case Status.Executing:
                        if (MotionController.Instance.IsMoveComplete()) {
                            status = Status.Idling;
                        }
                        break;
                }

            } while (keepRunning);

            // Program is exiting. Perform any necessary cleanup.

            DataLogger.ReleaseDataLog();
            InputController.ReleaseInputDevices();

            Halt();

            DataController.Instance.PCReady = false;
            DataController.Instance.StopTasks();

            logger.Info("Control loop has exited.");
        }

        #endregion

        #endregion
        
        #region Private Methods

        #region Motion

        /// <summary>
        /// Continues the process of searching for the reference locations.
        /// Called repeatedly by the control loop until the locations are found.
        /// </summary>
        private static void ContinueHoming()
        {
            if (!MotionController.Instance.HasFoundReference()) {
                return;
            }

            logger.Info("Home has been located. Zeroing references.");

            Thread.Sleep(250);
            MotionController.Instance.ZeroReference();

            homingPhase = HomingPhase.Complete;
            status = Status.Idling;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the set of motion commands for the predefined positions (Hang, Mayday, Loading, etc).
        /// </summary>
        private static void InitializeMotionStates()
        {
            logger.Debug("Enter: InitializeMotionStates()");

            lastCommand = new MotionCommand();

            backCommand = new MotionCommand();
            if (mount == MountType.Back)
            {
                backCommand.outerPosition = 180.0;
                backCommand.innerPosition = 180.0;
            }
            else
            {
                backCommand.outerPosition = 90.0;
                backCommand.innerPosition = 0.0;
            }
            backCommand.outerVelocity = NORMAL_VELOCITY;
            backCommand.innerVelocity = NORMAL_VELOCITY;
            backCommand.outerAcceleration = NORMAL_ACCELERATION;
            backCommand.innerAcceleration = NORMAL_ACCELERATION;

            frontCommand = new MotionCommand();
            if (mount == MountType.Back)
            {
                frontCommand.outerPosition = 0;
                frontCommand.innerPosition = 0;
            }
            else
            {
                frontCommand.outerPosition = 90.0;
                frontCommand.innerPosition = 180.0;
            }
            frontCommand.outerVelocity = NORMAL_VELOCITY;
            frontCommand.innerVelocity = NORMAL_VELOCITY;
            frontCommand.outerAcceleration = NORMAL_ACCELERATION;
            frontCommand.innerAcceleration = NORMAL_ACCELERATION;

            hangCommand = new MotionCommand {
                outerPosition = 90.0,
                innerPosition = 90.0,
                outerVelocity = NORMAL_VELOCITY,
                innerVelocity = NORMAL_VELOCITY,
                outerAcceleration = NORMAL_ACCELERATION,
                innerAcceleration = NORMAL_ACCELERATION
            };

            loadingCommand = new MotionCommand();
            if (mount == MountType.Back) {
                loadingCommand.outerPosition = 5.0;
                loadingCommand.innerPosition = 0.0;
            } else {
                loadingCommand.outerPosition = 95.0;
                loadingCommand.innerPosition = 180.0;
            }
            loadingCommand.outerVelocity = NORMAL_VELOCITY;
            loadingCommand.innerVelocity = NORMAL_VELOCITY;
            loadingCommand.outerAcceleration = NORMAL_ACCELERATION;
            loadingCommand.innerAcceleration = NORMAL_ACCELERATION;

            maydayCommand = new MotionCommand();
            if (mount == MountType.Back) {
                maydayCommand.outerPosition = 340.0;
                maydayCommand.innerPosition = 0.0;
            } else {
                maydayCommand.outerPosition = 60.0;
                maydayCommand.innerPosition = 180.0;
            }
            maydayCommand.outerVelocity = MAYDAY_VELOCITY;
            maydayCommand.innerVelocity = MAYDAY_VELOCITY;
            maydayCommand.outerAcceleration = MAYDAY_ACCELERATION;
            maydayCommand.innerAcceleration = MAYDAY_ACCELERATION;

            uprightCommand = new MotionCommand();
            if (mount == MountType.Back) {
                uprightCommand.outerPosition = 0.0;
                uprightCommand.innerPosition = 0.0;
            } else {
                uprightCommand.outerPosition = 90.0;
                uprightCommand.innerPosition = 180.0;
            }
            uprightCommand.outerVelocity = NORMAL_VELOCITY;
            uprightCommand.innerVelocity = NORMAL_VELOCITY;
            uprightCommand.outerAcceleration = NORMAL_ACCELERATION;
            uprightCommand.innerAcceleration = NORMAL_ACCELERATION;
        }

        #endregion

        #endregion
    }
}
