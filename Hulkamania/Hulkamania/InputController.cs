using log4net;
using SlimDX.DirectInput;
using System.Collections.Generic;
using System.Reflection;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// Handles reading input from the input devices.
    /// </summary>
    public static class InputController
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private const double JOYSTICK_UNITS_PER_DEG = 65536.0 / 180.0;

        #endregion

        #region Fields

        private static DirectInput directInput;
        
        private static Joystick joystick;

        private static Keyboard keyboard;

        private static bool joystickMessageGiven = false;

        #endregion

        #region Properties

        /// <summary>
        /// True if the joystick is currently connected.
        /// </summary>
        public static bool IsJoystickConnected
        {
            get {
                JoystickState state;
                
                state = new JoystickState();
                
                return ((joystick != null) && (!joystick.Poll().IsFailure) && (!joystick.GetCurrentState(ref state).IsFailure));
            }
        }

        /// <summary>
        /// Current joystick position, in range 0 to 1. Note that the joystick coordinate system is NOT altered
        /// </summary>
        public static ControlInput JoystickInputRaw
        {
            get
            {

                JoystickState state;
                double x;
                double y;

                if (joystick == null) {
                    return new ControlInput();
                }

                state = joystick.GetCurrentState();

                x = (state.X / JOYSTICK_UNITS_PER_DEG - 90.0) / 90.0;
                y = (state.Y / JOYSTICK_UNITS_PER_DEG - 90.0) / 90.0;

                if (keyboard != null) {
                    if (keyboard.GetCurrentState().PressedKeys.Contains(Key.RightControl)) {

                        // Override joystick input
                        return new ControlInput(x, y, state.IsPressed(0), true);
                    }
                }

                return new ControlInput(x, y, state.IsPressed(0), false);
            }
        }


        /// <summary>
        /// Current joystick position, in range 0 to 1. Note that the joystick coordinate system is altered depending on chair configuration and/or outer axis position
        /// As a result, pushing the joystick forward always has the desired effect on the task.
        /// </summary>
        public static ControlInput JoystickInput
        {
            get {

                JoystickState state;
                double x;
                double y;

                if (joystick == null) {
                    return new ControlInput();
                }

                state = joystick.GetCurrentState();

                x = (state.X / JOYSTICK_UNITS_PER_DEG - 90.0) / 90.0;
                y = (state.Y / JOYSTICK_UNITS_PER_DEG - 90.0) / 90.0;

                if ((Hulk.CurrentMotion.innerPosition < 90.0) && (Hulk.CurrentMotion.innerPosition > -90.0)) {
                    y = -y;
                }

                if (Hulk.ChairMount == Hulk.MountType.Seat) {
                    x = -x;
                    y = -y;
                }

                if (keyboard != null) {
                    if (keyboard.GetCurrentState().PressedKeys.Contains(Key.RightControl)) {

                        // Override joystick input
                        return new ControlInput(x, y, state.IsPressed(0), true);
                    }
                }

                return new ControlInput(x, y, state.IsPressed(0), false);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor. Called the first time this class is referenced.
        /// </summary>
        static InputController() 
        {
            logger.Debug("Static create: InputController");
            directInput = new DirectInput();
        }
        #endregion

  
        #region Internal Methods

        /// <summary>
        /// Acquires the communications connection to the joystick.
        /// .NET does not have joystick input by default, so SlimDX is used.
        /// </summary>
        internal static void AcquireJoystick()
        {
            DeviceInstance deviceInstance;
            IList<DeviceInstance> devices;

            logger.Debug("Enter: AcquireJoystick()");

            if (directInput == null)
                return;

            devices = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);

            if (devices.Count == 0) {
                if (!joystickMessageGiven) {
                    logger.Info("No joystick found.");
                    joystickMessageGiven = true;
                }
                joystick = null;
                return;
            }

            deviceInstance = null;
            foreach (var device in devices) {
                if ((device.Type == DeviceType.Joystick) || (device.Type == DeviceType.Gamepad))
                {
                    deviceInstance = device;
                }
                else
                {
                    continue;
                }
                
                break;
            }

            if (deviceInstance != null) {
                logger.Info("Joystick found: " + deviceInstance.ProductName);
                joystickMessageGiven = false;

                joystick = new Joystick(directInput, deviceInstance.InstanceGuid);
                joystick.Acquire();

            } else {
                if (!joystickMessageGiven) {
                    logger.Info("No joystick found.");
                    joystickMessageGiven = true;
                }
                joystick = null;
            }

            devices = directInput.GetDevices(DeviceClass.Keyboard, DeviceEnumerationFlags.AttachedOnly);

            deviceInstance = null;
            foreach (var device in devices) {
                if (device.Type != DeviceType.Keyboard) {
                    continue;
                }
                deviceInstance = device;
                break;
            }

            if (deviceInstance != null) {
                logger.Info("Keyboard found: " + deviceInstance.ProductName);

                keyboard = new Keyboard(directInput);
                keyboard.Acquire();

            } else {
                logger.Error("[InputController] No keyboard found.");

                keyboard = null;
            }
        }

        /// <summary>
        /// Releases the resources reserved for the input devices. Usually called on application exit. 
        /// </summary>
        internal static void ReleaseInputDevices()
        {
            logger.Debug("Enter: ReleaseInputDevices()");

            if (joystick != null) {
                joystick.Unacquire();
                joystick.Dispose();
                keyboard.Dispose();
                directInput.Dispose();
            }
        }

        #endregion
    }
}
