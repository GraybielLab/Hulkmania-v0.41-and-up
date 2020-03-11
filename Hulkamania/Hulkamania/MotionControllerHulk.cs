using Brandeis.AGSOL.FlexMotion;
using log4net;
using System;
using System.Reflection;
using System.Text;
using System.Configuration;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// The motion controller board.
    /// </summary>
    internal class MotionControllerHulk : MotionController
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private const ushort UNUSED_U16_PARAMETER = 0;

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
        
        private MotionCommand currentMotion;
        
        #endregion

        #region Properties
        /// <summary>
        /// The board ID of the motion controller board as it appears in NI motion and automation explorer
        /// </summary>
        internal byte BoardId
        {
            get
            {
                return byte.Parse(ConfigurationManager.AppSettings["MotionControllerBoardId"]);
            }
        }

        /// <summary>
        /// Current motion of the HULK. Read from the board each control loop cycle.
        /// </summary>
        internal override MotionCommand CurrentMotion
        {
            get {
                return currentMotion;
            }
        }
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the motion controller.
        /// </summary>
        internal MotionControllerHulk()
        {
             logger.Info("Create: MotionControllerHulk");
            
            currentMotion = new MotionCommand();

            InitializeBoard();
       }

        #endregion

        #region Internal Methods

        #region Motion Commands

        /// <summary>
        /// Starts the homing process. The HULK will seek out the reference locations for each axis.
        /// </summary>
        internal override void FindReference()
        {
            int error;

            error = Motion.flex_find_reference(BoardId, Motion.NIMC_AXIS_CTRL, AXIS_BITMAP, Motion.NIMC_FIND_HOME_REFERENCE);
            CheckNonmodalErrors(error);
        }

        /// <summary>
        /// Checks whether the homing reference locations have been found.
        /// </summary>
        /// <returns>True if references have been found</returns>
        internal override bool HasFoundReference()
        {
            int error;
            ushort found;
            ushort finding;
            
            error = Motion.flex_check_reference(BoardId, Motion.NIMC_AXIS_CTRL, AXIS_BITMAP, out found, out finding);
            CheckNonmodalErrors(error);

            return ((found != 0) && (finding == 0));
        }

        /// <summary>
        /// Checks whether the ongoing move command has finished.
        /// </summary>
        /// <returns>True if move is finished, false otherwise</returns>
        internal override bool IsMoveComplete()
        {
            int error;
            ushort moveComplete;

            error = Motion.flex_check_move_complete_status(BoardId, Motion.NIMC_AXIS_CTRL, AXIS_BITMAP, out moveComplete);
            CheckNonmodalErrors(error);

            return (moveComplete == 1);
        }

        /// <summary>
        /// Sets the desired acceleration for the inner and outer axes.
        /// </summary>
        /// <param name="command">The motion command, containing the accelerations</param>
        internal override void SetAcceleration(MotionCommand command)
        {
            int error;

            error = Motion.flex_load_rpsps(BoardId, OUTER_AXIS, Motion.NIMC_BOTH, command.outerAcceleration / 360.0,
                IMMEDIATE_INPUT_VECTOR);
            CheckNonmodalErrors(error);
            error = Motion.flex_load_rpsps(BoardId, INNER_AXIS, Motion.NIMC_BOTH, command.innerAcceleration / 360.0,
                IMMEDIATE_INPUT_VECTOR);
            CheckNonmodalErrors(error);
        }

        /// <summary>
        /// Sets the desired position for the inner and outer axes.
        /// </summary>
        /// <param name="command">The motion command, containing the positions</param>
        internal override void SetPosition(MotionCommand command)
        {
            int error;

            error = Motion.flex_load_target_pos(BoardId, OUTER_AXIS, (int)(CONTROLLER_STEPS_OUTER *
                command.outerPosition / 360.0), IMMEDIATE_INPUT_VECTOR);
            CheckNonmodalErrors(error);
            error = Motion.flex_load_target_pos(BoardId, INNER_AXIS, (int)(CONTROLLER_STEPS_INNER *
                command.innerPosition / 360.0), IMMEDIATE_INPUT_VECTOR);
            CheckNonmodalErrors(error);
        }

        /// <summary>
        /// Sets the desired velocity for the inner and outer axes.
        /// </summary>
        /// <param name="command">The motion command, containing the velocities</param>
        internal override void SetVelocity(MotionCommand command)
        {
            int error;

            error = Motion.flex_load_rpm(BoardId, OUTER_AXIS, command.outerVelocity / 6.0,
                IMMEDIATE_INPUT_VECTOR);
            CheckNonmodalErrors(error);
            error = Motion.flex_load_rpm(BoardId, INNER_AXIS, command.innerVelocity / 6.0,
                IMMEDIATE_INPUT_VECTOR);
            CheckNonmodalErrors(error);
        }

        /// <summary>
        /// Starts the HULK moving. Motion must be pre-programmed using SetCommandType and SetCommand.
        /// </summary>
        internal override void StartMotion()
        {
            int error;

            error = Motion.flex_start(BoardId, Motion.NIMC_AXIS_CTRL, AXIS_BITMAP);
            CheckNonmodalErrors(error);
        }

        /// <summary>
        /// Stops all motors.
        /// </summary>
        internal override void StopMotion()
        {
            int error;

            error = Motion.flex_stop_motion(BoardId, Motion.NIMC_AXIS_CTRL, Motion.NIMC_HALT_STOP, AXIS_BITMAP);
            CheckNonmodalErrors(error);
        }

        /// <summary>
        /// Sets up the HULK to use position commands in a vector space.
        /// </summary>
        internal override void UseModulusPositionCommands()
        {
            int error;

            logger.Debug("Enter: UseModulusPositionCommands()");

            error = Motion.flex_set_op_mode(BoardId, OUTER_AXIS, Motion.NIMC_MODULUS_POSITION);
            CheckNonmodalErrors(error);
            error = Motion.flex_set_op_mode(BoardId, INNER_AXIS, Motion.NIMC_MODULUS_POSITION);
            CheckNonmodalErrors(error);
            
            error = Motion.flex_load_pos_modulus(BoardId, OUTER_AXIS, (uint)Math.Abs(CONTROLLER_STEPS_OUTER),
                IMMEDIATE_INPUT_VECTOR);
            CheckNonmodalErrors(error);
            error = Motion.flex_load_pos_modulus(BoardId, INNER_AXIS, (uint)Math.Abs(CONTROLLER_STEPS_INNER),
                IMMEDIATE_INPUT_VECTOR);
            CheckNonmodalErrors(error);
        }

        /// <summary>
        /// Sets up the HULK to use velocity commands.
        /// </summary>
        internal override void UseVelocityCommands()
        {
            int error;

            logger.Debug("Enter: UseVelocityCommands()");

            error = Motion.flex_set_op_mode(BoardId, OUTER_AXIS, Motion.NIMC_VELOCITY);
            CheckNonmodalErrors(error);
            error = Motion.flex_set_op_mode(BoardId, INNER_AXIS, Motion.NIMC_VELOCITY);
            CheckNonmodalErrors(error);
        }

        /// <summary>
        /// Use the current position as the zero reference for all future position coordinates.
        /// </summary>
        internal override void ZeroReference()
        {
            MotionCommand motionCommand;
            int error;
            int positionOuter;
            int positionInner;

            positionOuter = (int)(((int)(CONTROLLER_STEPS_OUTER * HOME_OUTER / 360.0)) % CONTROLLER_STEPS_OUTER);
            positionInner = (int)(((int)(CONTROLLER_STEPS_INNER * HOME_INNER / 360.0)) % CONTROLLER_STEPS_INNER);

            error = Motion.flex_reset_pos(BoardId, OUTER_AXIS, positionOuter, positionOuter, IMMEDIATE_INPUT_VECTOR);
            CheckNonmodalErrors(error);
            error = Motion.flex_reset_pos(BoardId, INNER_AXIS, positionInner, positionInner, IMMEDIATE_INPUT_VECTOR);
            CheckNonmodalErrors(error);

            motionCommand = new MotionCommand {
                innerPosition = HOME_INNER,
                outerPosition = HOME_OUTER
            };
            Hulk.LastCommand = motionCommand;
        }

        #endregion

        #region Status Updates

        /// <summary>
        /// Reads the current motion of the Hulk from its encoders.
        /// </summary>
        /// <param name="updateRate">The number of motion reads in the past second</param>
        internal override void ReadCurrentMotion(int updateRate)
        {
            MotionCommand motion;
            double velocityOuter;
            double velocityInner;
            int positionOuter;
            int positionInner;
            int error;

            motion = new MotionCommand();

            // Read position
            error = Motion.flex_read_encoder_rtn(BoardId, PRIMARY_FEEDBACK_OUTER, out positionOuter);
            CheckNonmodalErrors(error);
            error = Motion.flex_read_encoder_rtn(BoardId, PRIMARY_FEEDBACK_INNER, out positionInner);
            CheckNonmodalErrors(error);
            motion.outerPosition = (360.0 * positionOuter / ENCODER_STEPS_OUTER) % 360.0;
            motion.innerPosition = (360.0 * positionInner / ENCODER_STEPS_INNER) % 360.0;

            // Read velocity
            error = Motion.flex_read_rpm_rtn(BoardId, OUTER_AXIS, out velocityOuter);
            CheckNonmodalErrors(error);
            error = Motion.flex_read_rpm_rtn(BoardId, INNER_AXIS, out velocityInner);
            CheckNonmodalErrors(error);
            motion.outerVelocity = velocityOuter * 6.0;
            motion.innerVelocity = velocityInner * 6.0;

            // NI-Motion has no way to read current acceleration, so estimate it.
            motion.outerAcceleration = (motion.outerVelocity - currentMotion.outerVelocity) / (1.0 / (double)updateRate);
            motion.innerAcceleration = (motion.innerVelocity - currentMotion.innerVelocity) / (1.0 / (double)updateRate);

            currentMotion = motion;
        }

        #endregion

        #region Error Checking

        /// <summary>
        /// Checks whether the motion board has reported any modal (runtime) errors. If there is an error, 
        /// the motion board is immediately stopped and the application is signaled to cleanup and exit.
        /// </summary>
        internal override void CheckModalErrors()
        {
            int errorCode;
            string newModalMessage;
            string allModalMessages;
            ushort communicationStatus;
            ushort commandID;
            ushort resourceID;

            allModalMessages = "";

            Motion.flex_read_csr_rtn(BoardId, out communicationStatus);

            while ((communicationStatus & Motion.NIMC_MODAL_ERROR_MSG) != 0) {

                // There is at least one modal error. Loop through them.

                // Get the modal error from the motion board's error stack.
                Motion.flex_read_error_msg_rtn(BoardId, out commandID, out resourceID, out errorCode);

                // Display the error.
                newModalMessage = GetErrorMessage(errorCode, commandID, resourceID);
                if (newModalMessage != null) {
                    allModalMessages += "[MotionControllerHulk] " + newModalMessage;
                }

                // Look for the next error.
                Motion.flex_read_csr_rtn(BoardId, out communicationStatus);
            }

            if (allModalMessages == "") {
                return;
            }

            logger.Error(allModalMessages);

            if (Hulk.SystemStatus == Hulk.Status.Initializing) {

                // Program was still being initialized when this error was found. Signal the main program to terminate.
                throw new Exception();
            }

            // Program was already initialized. Halt the motors and exit gracefully.
            Hulk.Halt();
            MainForm.ExitApplication(allModalMessages);
        }

        #endregion

        #endregion

        #region Private Methods

        #region Initialization

        /// <summary>
        /// Initialize the motion control board.
        /// This must be called once before any movement commands are given.
        /// </summary>
        private void InitializeBoard()
        {
            int error;
            sbyte[] settingName;
            byte[] settingNameBytes;

            logger.Debug("Enter: InitializeBoard()");

            // Reset controller 

            error = Motion.nimcResetController(BoardId);
            CheckNonmodalErrors(error);

            // Read the motion board settings from MAX

            settingNameBytes = Encoding.ASCII.GetBytes("HULK");
            settingName = new sbyte[settingNameBytes.Length];
            for (int i = 0; i < settingNameBytes.Length; i++) {
                settingName[i] = (sbyte)settingNameBytes[i];
            }
            error = Motion.flex_initialize_controller(BoardId, settingName);
            CheckNonmodalErrors(error);

            // Configure vector space (X and Y defined, Z axis undefined)

            error = Motion.flex_config_vect_spc(BoardId, VECTOR_SPACE, OUTER_AXIS, INNER_AXIS, Motion.NIMC_NOAXIS);
            CheckNonmodalErrors(error);

            // Check for any modal (runtime) errors that occurred during initialization

            CheckModalErrors();
            
            logger.Info("Motion controller initialized.");
        }

        #endregion

        #region Error Checking

        /// <summary>
        /// Checks any errors reported by the motion board. If there is an error, the motion board is immediately stopped
        /// and the application is signaled to cleanup and exit.
        /// </summary>
        /// <param name="errorCode">The error code returned by the board during the last motion call</param>
        private void CheckNonmodalErrors(int errorCode)
        {
            string errorMessage;

            if (errorCode == 0) {
                return;
            }

            errorMessage = GetErrorMessage(errorCode, 0, 0);

            if (errorMessage == null) {
                return;
            }

            logger.Error("[MotionControllerHulk] " + errorMessage);

            if (Hulk.SystemStatus == Hulk.Status.Initializing) {

                // Program was still being initialized when this error was found. Signal the main program to terminate.
                throw new Exception();
            }

            // Program was already initialized. Halt the motors and exit gracefully.
            Hulk.Halt();
            MainForm.ExitApplication(errorMessage);
        }

        /// <summary>
        /// Queries the motion board for a full description of a given error.
        /// </summary>
        /// <param name="errorCode">The error code to find a description of</param>
        /// <param name="commandID">Command that caused the error (for modal errors) or 0 (for nonmodal errors)</param>
        /// <param name="resourceID">Resource that caused the error (for modal errors) or 0 (for nonmodal errors)</param>
        /// <returns>The string description of the error, or null if no description could be found</returns>
        private string GetErrorMessage(int errorCode, ushort commandID, ushort resourceID)
        {
            byte[] bytes;
            sbyte[] errorDescription;
            string error;
            uint sizeOfArray;
            ushort descriptionType;

            errorDescription = new sbyte[0];

            descriptionType = (commandID == 0) ? Motion.NIMC_ERROR_ONLY : Motion.NIMC_COMBINED_DESCRIPTION;

            // First get the size of the error description
            Motion.flex_get_error_description(descriptionType, errorCode, commandID, resourceID, errorDescription, out sizeOfArray);

            // Allocate memory on the heap for the description
            errorDescription = new sbyte[sizeOfArray + 1];

            // Get the error description
            Motion.flex_get_error_description(descriptionType, errorCode, commandID, resourceID, errorDescription, out sizeOfArray);

            if (errorDescription.Length > 0) {

                bytes = new byte[errorDescription.Length];
                Buffer.BlockCopy(errorDescription, 0, bytes, 0, errorDescription.Length);
                error = (new UTF7Encoding()).GetString(bytes);

                return error;
            }
            return null;
        }

        #endregion

        #endregion
    }
}
