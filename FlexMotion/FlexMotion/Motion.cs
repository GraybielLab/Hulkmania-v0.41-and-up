using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Brandeis.AGSOL.FlexMotion
{
    /// <summary>
    /// C# wrapper for FlexMotion32.dll
    /// 
    /// Based on MotnCnst.h, (c) 2005 National Instruments.
    /// </summary>
    public class Motion
    {
        #region Constants

        public const Byte MAXBOARDS = 0x20;                 // 32 boards allowed in this version of the DLL
        public const Byte NIMC_MAXBOARDS = 32;              // Obsolete NI-Motion=7.2

        public const UInt16 NIMC_MAX_FILENAME_LEN = 256;    // Max string length, Obsolete NI-Motion=7.2

        #region Boolean

        public const Byte NIMC_FALSE = 0;
        public const Byte NIMC_TRUE = 1;

        #endregion

        #region Polarity

        public const Byte NON_INVERTING = 0;
        public const Byte INVERTING = 1;
        public const Byte NIMC_ACTIVE_HIGH = 0;
        public const Byte NIMC_ACTIVE_LOW = 1;

        #endregion

        #region Output Drive Mode

        public const Byte NIMC_OPEN_COLLECTOR = 0;
        public const Byte NIMC_TOTEM_POLE = 1;

        #endregion

        #region Board Information

        // These public constant values are used to represent various board information. Refer to GetMotionBoardInfo API.

        #region Board Attributes
        
        public const UInt32 NIMC_BOARD_FAMILY = 1100;
        public const UInt32 NIMC_BOARD_TYPE = 1120;
        public const UInt32 NIMC_BUS_TYPE = 1130;
        public const UInt32 NIMC_NOT_APPLICABLE = 1500;
        public const UInt32 NIMC_NUM_AXES = 1510;
        public const UInt32 NIMC_NUM_LICENSED_AXES = 1520;
        public const UInt32 NIMC_BOOT_VERSION = 3010;
        public const UInt32 NIMC_FIRMWARE_VERSION = 3020;
        public const UInt32 NIMC_DSP_VERSION = 3030;
        public const UInt32 NIMC_FPGA_VERSION = 3040;
        public const UInt32 NIMC_FPGA1_VERSION = 3040;
        public const UInt32 NIMC_FPGA2_VERSION = 3050;
        public const UInt32 NIMC_CONTROLLER_SERIAL_NUMBER = 2040;
        public const UInt32 NIMC_CONTROLLER_LOCAL_OR_REMOTE = 2050;
        public const UInt32 NIMC_DRIVER_VERSION = 2060;
        public const UInt32 NIMC_NUMBER_OF_COORDINATE_SPACES = 2070;
        public const UInt32 NIMC_LICENSE_TYPE = 2080;
        public const UInt32 NIMC_CLOSED_LOOP_CAPABLE = 1150;            // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_FLEXMOTION_BOARD_CLASS = 2030;         // Obsolete NI-Motion=7.2

        #endregion

        #region Board Type

        public const Byte PCI_7330 = 63;
        public const Byte PXI_7330 = 64;
        public const Byte PCI_7334 = 32;
        public const Byte PXI_7334 = 25;
        public const Byte PCI_7340 = 61;
        public const Byte PXI_7340 = 62;
        public const Byte PCI_7342 = 37;
        public const Byte PXI_7342 = 36;
        public const Byte PCI_7344 = 28;
        public const Byte PXI_7344 = 27;
        public const Byte PCI_7350 = 34;
        public const Byte PXI_7350 = 35;
        public const Byte PCI_7390 = 71;
        public const Byte SOFTMOTION = 99;
        public const Byte FW_7344 = 31;         // Obsolete NI-Motion=7.2
        public const Byte ENET_7344 = 41;       // Obsolete NI-Motion=7.2
        public const Byte SER_7340 = 51;        // Obsolete NI-Motion=7.2
        
        #endregion

        #region Board Family

        public const Byte NIMC_NI_MOTION = 0;
        public const Byte NIMC_FLEX_MOTION = 0;           // Obsolete NI-Motion=7.2
        public const Byte NIMC_VALUE_MOTION = 1;          // Obsolete NI-Motion=7.2

        #endregion

        #region Bus Type
        
        public const Byte NIMC_ISA_BUS = 0;             // Obsolete NI-Motion=7.2
        public const Byte NIMC_PCI_BUS = 1;
        public const Byte NIMC_PXI_BUS = 2;
        public const Byte NIMC_UNKNOWN_BUS = 3;
        public const Byte NIMC_1394_BUS = 4;
        public const Byte NIMC_ENET_BUS = 5;            // Obsolete NI-Motion=7.2 
        public const Byte NIMC_SERIAL_BUS = 6;          // Obsolete NI-Motion=7.2
        public const Byte NIMC_VIRTUAL_BUS = 7;
        public const Byte NIMC_CAN_BUS = 8;

        #endregion

        #region Number of Axes
        
        public const Byte TWO_AXIS = 2;         // Obsolete NI-Motion=7.2
        public const Byte THREE_AXIS = 3;       // Obsolete NI-Motion=7.2
        public const Byte FOUR_AXIS = 4;        // Obsolete NI-Motion=7.2
        public const Byte SIX_AXIS = 6;         // Obsolete NI-Motion=7.2
        public const Byte EIGHT_AXIS = 8;       // Obsolete NI-Motion=7.2

        #endregion

        #region Motor System Type
        
        public const Byte SERVO = 0;            // Obsolete NI-Motion=7.2
        public const Byte STEPPER = 1;          // Obsolete NI-Motion=7.2
        public const Byte SERVO_STEPPER = 2;    // Obsolete NI-Motion=7.2
        
        #endregion

        #region Stepper Motor Type
        
        public const Byte OPEN_LOOP = 0;        // Obsolete NI-Motion=7.2
        public const Byte CLOSED_LOOP = 1;      // Obsolete NI-Motion=7.2
        
        #endregion

        #region Operating System
        
        public const UInt32 NIMC_UNKNOWN_OS = 0xFFFF;   // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_WIN95 = 0;             // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_WIN98 = 2;             // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_WINNT = 3;             // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_WIN2000 = 4;           // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_PHARLAP = 5;           // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_LINUX = 6;             // Obsolete NI-Motion=7.2

        #endregion

        #region License Type
        
        public const UInt32 NIMC_NOTACTIVATED = 0;
        public const UInt32 NIMC_NI_7300 = 4000;
        public const UInt32 NIMC_CANOPEN = 3000;
        public const UInt32 NIMC_NI_7400 = 2000;
        public const UInt32 NIMC_ORMEC = 1000;
        
        #endregion

        #region Board Class
        
        public const Byte NIMC_FLEX_7344 = 1;           // Obsolete NI-Motion=7.2
        public const Byte NIMC_FLEX_7334 = 2;           // Obsolete NI-Motion=7.2
        public const Byte NIMC_FLEX_7348 = 3;           // Obsolete NI-Motion=7.2
        public const Byte NIMC_FLEX_7342 = 4;           // Obsolete NI-Motion=7.2
        public const Byte NIMC_NI_MOTION_7344 = 1;      // Obsolete NI-Motion=7.2
        public const Byte NIMC_NI_MOTION_7334 = 2;      // Obsolete NI-Motion=7.2
        public const Byte NIMC_NI_MOTION_7350 = 3;      // Obsolete NI-Motion=7.2
        public const Byte NIMC_NI_MOTION_7342 = 4;      // Obsolete NI-Motion=7.2
        public const Byte NIMC_NI_MOTION_7340 = 5;      // Obsolete NI-Motion=7.2
        public const Byte NIMC_NI_MOTION_7330 = 6;      // Obsolete NI-Motion=7.2
        public const Byte NIMC_NI_MOTION_7390 = 7;      // Obsolete NI-Motion=7.2

        #endregion

        #region Board Location
        
        public const Byte NIMC_LOCAL_CONTROLLER = 0;
        public const Byte NIMC_REMOTE_CONTROLLER = 1;

        #endregion

        #endregion

        #region Softmotion Axis Information

        // These public constant values are used to represent various NI SoftMotion axis information.
        // These values are reserved for internal used only.

        #region Axis Type
        
        public const UInt32 NIMC_NI_7451 = 2001;
        public const UInt32 NIMC_NI_7431 = 2100;
        public const UInt32 NIMC_ORMEC_SMS_DRIVE = 1001;
        public const UInt32 NIMC_CANOPEN_COPLEY_ACCELNET_MODULE = 3001;
        public const UInt32 NIMC_CANOPEN_COPLEY_XENUS = 3002;
        public const UInt32 NIMC_CANOPEN_COPLEY_ACCELNET_PANEL = 3004;
        public const UInt32 NIMC_CANOPEN_COPLEY_STEPNET_MODULE = 3005;
        public const UInt32 NIMC_CANOPEN_COPLEY_STEPNET_PANEL = 3006;
        public const UInt32 NIMC_CANOPEN_COPLEY_STEPNET_PANEL2 = 3007;
        public const UInt32 NIMC_CANOPEN_COPLEY_STEPNET_MICRO_MODULE = 3008;
        public const UInt32 NIMC_CANOPEN_COPLEY_XENUS_RESOLVER = 3009;
        public const UInt32 NIMC_CANOPEN_COPLEY_XENUS2 = 3010;
        public const UInt32 NIMC_CANOPEN_COPLEY_ACCELNET_PANEL2 = 3011;
        public const UInt32 NIMC_CANOPEN_COPLEY_XENUS3 = 3012;
        public const UInt32 NIMC_CANOPEN_COPLEY_XENUS_RESOLVER2 = 3013;

        #endregion

        #region Axis Presence Status
        
        public const Byte NIMC_DEVICE_ADDED = 0;
        public const Byte NIMC_DEVICE_FOUND = 1;
        public const Byte NIMC_DEVICE_ALL = 2;

        #endregion

        #endregion

        #region Axis Configuration

        // These public constant values are used during axis configuration. These attributes indicates the
        // axis system setup and resources used for the axis. These attributes are used by multiple APIs.

        #region Control Loop Update Rate
        
        public const Byte NIMC_PID_RATE_62_5 = 0;
        public const Byte NIMC_PID_RATE_125 = 1;
        public const Byte NIMC_PID_RATE_188 = 2;
        public const Byte NIMC_PID_RATE_250 = 3;
        public const Byte NIMC_PID_RATE_313 = 4;
        public const Byte NIMC_PID_RATE_375 = 5;
        public const Byte NIMC_PID_RATE_438 = 6;
        public const Byte NIMC_PID_RATE_500 = 7;

        #endregion

        #endregion

        #region Stepper

        #region Stepper Operation Mode
        
        public const Byte NIMC_OPEN_LOOP = 0;
        public const Byte NIMC_CLOSED_LOOP = 1;
        public const Byte NIMC_P_COMMAND = 2;

        #endregion

        #region Stepper Pulse Mode
        
        public const Byte NIMC_CLOCKWISE_COUNTERCLOCKWISE = 0;
        public const Byte NIMC_STEP_AND_DIRECTION = 1;

        #endregion

        #region Units Per Revolution
        
        public const Byte NIMC_COUNTS = 0;
        public const Byte NIMC_STEPS = 1;

        #endregion

        #endregion

        #region Servo Tuning

        // These public constant values are used to configure various tuning parameter on servo axes. 
        // Refer to LoadAdvancedControlParameter and LoadPIDParameters APIs.

        #region Servo Operation Mode
        
        public const Byte NIMC_EXTERNAL_COMMUTATION = 0;
        public const Byte NIMC_ONBOARD_COMMUTATION = 1;

        #endregion

        #region PID Attributes
        
        public const Byte NIMC_KP = 0;
        public const Byte NIMC_KI = 1;
        public const Byte NIMC_IL = 2;
        public const Byte NIMC_KD = 3;
        public const Byte NIMC_TD = 4;
        public const Byte NIMC_KV = 5;
        public const Byte NIMC_AFF = 6;
        public const Byte NIMC_VFF = 7;

        #endregion

        #region Advanced Control Attribute Types
        
        public const Byte NIMC_STATIC_FRICTION_MODE = 0;
        public const Byte NIMC_STATIC_FRICTION_MAX_DEADZONE = 3;
        public const Byte NIMC_STATIC_FRICTION_MIN_DEADZONE = 4;
        public const Byte NIMC_STATIC_FRICTION_ITERM_OFFSET_FWD = 5;
        public const Byte NIMC_STATIC_FRICTION_ITERM_OFFSET_REV = 6;
        public const Byte NIMC_PID_RATE_MULTIPLIER = 7;
        public const Byte NIMC_NOTCH_FILTER_FREQUENCY = 8;
        public const Byte NIMC_NOTCH_FILTER_BANDWIDTH = 9;
        public const Byte NIMC_NOTCH_FILTER_ENABLE = 10;
        public const Byte NIMC_LOWPASS_FILTER_CUTOFF_FREQUENCY = 11;
        public const Byte NIMC_LOWPASS_FILTER_ENABLE = 12;
        public const Byte NIMC_SECONDARY_PID_MODE = 13;
        public const Byte NIMC_CONTROL_LOOP_RATE = 14;

        #endregion

        #region Secondary PID Mode
        
        public const Byte NIMC_PID_DISABLED = 0;
        public const Byte NIMC_PID_CHANGE_IN_FEEDBACK = 1;
        public const Byte NIMC_PID_ACCELERATING = 2;
        public const Byte NIMC_PID_MOVING = 3;
        public const Byte NIMC_PID_MOVING_REVERSE = 4;
        
        #endregion

        #region PID Rate Multiplier
        
        public const Byte NIMC_MAX_PID_RATE_MULTIPLIER = 100;

        #endregion

        #region Static Friction Mode
        
        public const Byte NIMC_STICTION_OFF = 0;
        public const Byte NIMC_STICTION_ZERO_DAC = 1;
        public const Byte NIMC_STICTION_KILL = 2;

        #endregion

        #region External Commutation Attributes
        
        public const Byte NIMC_COMM_INITIALIZATION_TYPE = 0;
        public const Byte NIMC_COMM_FIND_ZERO_VOLT = 1;
        public const Byte NIMC_COMM_FIND_ZERO_TIME = 2;
        public const Byte NIMC_COMM_HALL_SENSOR_OFFSET = 3;      // Obsolete NI-Motion=7.2
        public const Byte NIMC_COMM_DIRECT_SET_PHASE = 4;
        public const Byte NIMC_COMM_ELECTRICAL_CYCLE_COUNTS = 5;
        public const Byte NIMC_COMM_HALL_SENSOR_TYPE = 6;
        public const Byte NIMC_COMM_INITIALIZE_ON_BOOT = 7;      // Obsolete NI-Motion=7.2
        public const Byte NIMC_COMM_MODE = 8;

        #endregion

        #region External Commutation Mode

        public const Byte NIMC_HALL_SENSOR = 0;
        public const Byte NIMC_SHAKE_AND_WAKE = 1;
        public const Byte NIMC_DIRECT_SET = 2;
        
        #endregion

        #region Hall Sensor Type
        
        public const Byte NIMC_HALL_SENSOR_TYPE_1 = 0;
        public const Byte NIMC_HALL_SENSOR_TYPE_2 = 1;

        #endregion

        #region PID Loop Rate
        
        public const UInt32 MAX_PIDLOOPRATE = 16000;         // Obsolete NI-Motion=7.2

        #endregion

        #endregion

        #region Trajectory

        // These public constant values are used to configure parameters for the motion control profile generation engine.
        // These attributes are used by multiple APIs.

        #region Acceleration
        
        public const byte NIMC_BOTH = 0;
        public const byte NIMC_ACCELERATION = 1;
        public const byte NIMC_DECELERATION = 2;
        public const byte NIMC_ACCEL = NIMC_ACCELERATION;                              // Obsolete NI-Motion=7.2
        public const byte NIMC_DECEL = NIMC_DECELERATION;                              // Obsolete NI-Motion=7.2

        #endregion

        #region Operation Modes

        public const Byte NIMC_ABSOLUTE_POSITION = 0;
        public const Byte NIMC_RELATIVE_POSITION = 1;
        public const Byte NIMC_VELOCITY = 2;
        public const Byte NIMC_RELATIVE_TO_CAPTURE = 3;
        public const Byte NIMC_MODULUS_POSITION = 4;
        public const Byte NIMC_ABSOLUTE_CONTOURING = 5;
        public const Byte NIMC_RELATIVE_CONTOURING = 6;
        
        #endregion

        #region Stop Control Modes
        
        public const ushort NIMC_DECEL_STOP = 0;
        public const ushort NIMC_HALT_STOP = 1;
        public const ushort NIMC_KILL_STOP = 2;

        #endregion

        #region Trajectory Status
        
        public const Byte NIMC_RUN_STOP_STATUS = 0;
        public const Byte NIMC_MOTOR_OFF_STATUS = 1;
        public const Byte NIMC_VELOCITY_THRESHOLD_STATUS = 2;
        public const Byte NIMC_MOVE_COMPLETE_STATUS = 3;

        #endregion

        #region Axis Status Bitmap

        public const UInt32 NIMC_RUN_STOP_BIT = 0x0001;
        public const UInt32 NIMC_PROFILE_COMPLETE_BIT = 0x0002;
        public const UInt32 NIMC_AXIS_OFF_BIT = 0x0004;
        public const UInt32 NIMC_FOLLOWING_ERROR_BIT = 0x0008;
        public const UInt32 NIMC_LIMIT_SWITCH_BIT = 0x0010;
        public const UInt32 NIMC_HOME_SWITCH_BIT = 0x0020;
        public const UInt32 NIMC_SW_LIMIT_BIT = 0x0040;
        public const UInt32 NIMC_AXIS_COMM_WATCHDOG_BIT = 0x0080;
        public const UInt32 NIMC_VELOCITY_THRESHOLD_BIT = 0x0100;
        public const UInt32 NIMC_POS_BREAKPOINT_BIT = 0x0200;
        public const UInt32 NIMC_HOME_FOUND_BIT = 0x0400;
        public const UInt32 NIMC_INDEX_FOUND_BIT = 0x0800;
        public const UInt32 NIMC_HIGH_SPEED_CAPTURE_BIT = 0x1000;
        public const UInt32 NIMC_DIRECTION_BIT = 0x2000;
        public const UInt32 NIMC_BLEND_STATUS_BIT = 0x4000;
        public const UInt32 NIMC_MOVE_COMPLETE_BIT = 0x8000;
        
        #endregion

        #region Velocity Filter Parameters

        public const UInt32 MAX_VELOCITY_FILTER_CONSTANT = 1000;    // Obsolete NI-Motion=7.2        
        public const UInt32 MAX_VELOCITY_UPDATE_INTERVAL = 2500;    // Obsolete NI-Motion=7.2
        
        #endregion

        #endregion

        #region Buffer Operation

        // Used to configure onboard memory buffer for various operations

        #region Buffered Operations Mode

        public const Byte NIMC_GENERAL_PURPOSE_INPUT                      =0;
        public const Byte NIMC_GENERAL_PURPOSE_OUTPUT                     =1;
        public const Byte NIMC_POSITION_DATA                              =2;
        public const Byte NIMC_BREAKPOINT_DATA                            =3;
        public const Byte NIMC_HS_CAPTURE_READBACK                        =4;
        public const Byte NIMC_CAMMING_POSITION                           =5;

        #endregion

        #region Buffer Status

        public const Byte NIMC_BUFFER_NOT_EXIST      =0;
        public const Byte NIMC_BUFFER_READY          =1;
        public const Byte NIMC_BUFFER_ACTIVE         =2;
        public const Byte NIMC_BUFFER_DONE           =3;
        public const Byte NIMC_BUFFER_OLDDATASTOP    =4;

        #endregion

        #region Write Regeneration Mode
        
        public const Byte NIMC_REGENERATION_NO_CHANGE                     =0;
        public const Byte NIMC_REGENERATION_LAST_WRITE                    =1;

        #endregion

        #region Contouring

        public const Byte NIMC_MIN_CONTOURING_INTERVAL                    =10;      // Obsolete NI-Motion=7.2
        public const Double NIMC_MAX_CONTOURING_INTERVAL                    =90.5;    // Obsolete NI-Motion=7.2
        public const Byte NIMC_MAX_SPLINE_POINTS                          =181;     // Obsolete NI-Motion=7.2

        #endregion

        #endregion

        #region Onboard Programming

        // Used during onboard programming. The attributes related to program control and configuration.
       
        #region Object Memory Control

        public const Byte NIMC_OBJECT_SAVE        =0;
        public const Byte NIMC_OBJECT_DELETE      =1;
        public const Byte NIMC_OBJECT_FREE        =2;

        #endregion

        #region Object Type

        public const Byte NIMC_OBJECT_TYPE_PROGRAM=1;
        public const Byte NIMC_OBJECT_TYPE_BUFFER =2;

        #endregion

        #region Program Wait and Program Jump Condition

        public const Byte NIMC_CONDITION_LESS_THAN                        =0; 
        public const Byte NIMC_CONDITION_EQUAL                            =1;
        public const Byte NIMC_CONDITION_LESS_THAN_OR_EQUAL               =2; 
        public const Byte NIMC_CONDITION_GREATER_THAN                     =3; 
        public const Byte NIMC_CONDITION_NOT_EQUAL                        =4; 
        public const Byte NIMC_CONDITION_GREATER_THAN_OR_EQUAL            =5; 
        public const Byte NIMC_CONDITION_TRUE                             =6; 
        public const Byte NIMC_CONDITION_HOME_FOUND                       =7; 
        public const Byte NIMC_CONDITION_INDEX_FOUND                      =8; 
        public const Byte NIMC_CONDITION_HIGH_SPEED_CAPTURE               =9; 
        public const Byte NIMC_CONDITION_POSITION_BREAKPOINT              =10;
        public const Byte NIMC_CONDITION_ANTICIPATION_TIME_BREAKPOINT     =11;
        public const Byte NIMC_CONDITION_VELOCITY_THRESHOLD               =12;
        public const Byte NIMC_CONDITION_MOVE_COMPLETE                    =13;
        public const Byte NIMC_CONDITION_PROFILE_COMPLETE                 =14;
        public const Byte NIMC_CONDITION_BLEND_COMPLETE                   =15;
        public const Byte NIMC_CONDITION_MOTOR_OFF                        =16;
        public const Byte NIMC_CONDITION_HOME_INPUT_ACTIVE                =17;
        public const Byte NIMC_CONDITION_LIMIT_INPUT_ACTIVE               =18;
        public const Byte NIMC_CONDITION_SOFTWARE_LIMIT_ACTIVE            =19;
        public const Byte NIMC_CONDITION_PROGRAM_COMPLETE                 =20;
        public const Byte NIMC_CONDITION_IO_PORT_MATCH                    =21;
        public const Byte NIMC_CONDITION_CENTER_FOUND                     =22;
        public const Byte NIMC_CONDITION_FORWARD_LIMIT_FOUND              =23;
        public const Byte NIMC_CONDITION_REVERSE_LIMIT_FOUND              =24;

        #endregion

        #region Program Wait and Program Jump Condition Matching Type

        public const Byte NIMC_MATCH_ALL          =0;
        public const Byte NIMC_MATCH_ANY          =1;

        #endregion

        #region Program Wait Condition Chaining

        public const Byte NIMC_WAIT               =0;
        public const Byte NIMC_WAIT_OR            =1;

        #endregion

        #region Program Status

        public const Byte NIMC_PROGRAM_DONE       =0;
        public const Byte NIMC_PROGRAM_PLAYING    =1;
        public const Byte NIMC_PROGRAM_PAUSED     =2;
        public const Byte NIMC_PROGRAM_STORING    =3;

        #endregion

        #region Program Download

        public const Byte NIMC_PROGRAM_DLOAD_STOP                         =0;        // Obsolete NI-Motion=7.2
        public const Byte NIMC_PROGRAM_DLOAD_START                        =1;       // Obsolete NI-Motion=7.2
        public const Byte NIMC_PROGRAM_DLOAD_PROGRESS                     =2;        // Obsolete NI-Motion=7.2

        #endregion

        #region Program Timeslice

        public const Byte NIMC_MAX_TIMESLICE      =20;             // Obsolete NI-Motion=7.2

        #endregion

        #endregion

        #region Position Compare and Input Capture

        // These public constant values are used to configure position compare and input capture behavior. 
        // These attributes are used by multiple APIs, refer to position compare (breakpoint) and 
        // input capture (high-speed capture) related APIs.

        #region Position Compare and Input Capture Buffer Operation

        public const Byte NIMC_OPERATION_SINGLE                           =0;
        public const Byte NIMC_OPERATION_BUFFERED                         =1;

        #endregion

        #region Position Compare Mode

        public const Byte NIMC_BREAKPOINT_OFF                             =0;
        public const Byte NIMC_ABSOLUTE_BREAKPOINT                        =1;
        public const Byte NIMC_RELATIVE_BREAKPOINT                        =2;
        public const Byte NIMC_MODULO_BREAKPOINT                          =3;
        public const Byte NIMC_PERIODIC_BREAKPOINT                        =4;

        #endregion

        #region Position Compare Output Action

        public const Byte NIMC_NO_CHANGE                                  =0;
        public const Byte NIMC_RESET_BREAKPOINT                           =1;
        public const Byte NIMC_SET_BREAKPOINT                             =2;
        public const Byte NIMC_TOGGLE_BREAKPOINT                          =3;
        public const Byte NIMC_PULSE_BREAKPOINT                           =4;

        #endregion

        #region Position Compare Type
        
        public const Byte NIMC_POSITION_BREAKPOINT                        =0;
        public const Byte NIMC_ANTICIPATION_TIME_BREAKPOINT               =1;       // Obsolete NI-Motion=7.2

        #endregion

        #region Input Capture Mode
        
        public const Byte NIMC_HS_NON_INVERTING_LEVEL                     =0;
        public const Byte NIMC_HS_INVERTING_LEVEL                         =1;
        public const Byte NIMC_HS_LOW_TO_HIGH_EDGE                        =2;
        public const Byte NIMC_HS_HIGH_TO_LOW_EDGE                        =3;
        public const Byte NIMC_HS_NON_INVERTING_DI                        =4;
        public const Byte NIMC_HS_INVERTING_DI                            =5;

        #endregion

        #endregion

        #region Analog IO Attributes

        // These public constant values are used to configure the analog input and analog output resources. 

        #region ADC Input Range

        public const Byte NIMC_ADC_UNIPOLAR_5     =0;
        public const Byte NIMC_ADC_BIPOLAR_5      =1;
        public const Byte NIMC_ADC_UNIPOLAR_10    =2;
        public const Byte NIMC_ADC_BIPOLAR_10     =3;

        #endregion

        #endregion

        #region Motion IO Attributes

        // These public constant values are used to configure and control the motion IO line such as limits and inhibits.

        #region Limit Input Attribute

        public const Byte  NIMC_LIMIT_INPUTS      =0;
        public const Byte  NIMC_SOFTWARE_LIMITS   =1;

        #endregion

        #region Home Input Attribute

        public const Byte  NIMC_HOME_INPUTS       =2;             // Obsolete NI-Motion=7.2

        #endregion

        #region Input Capture Lines

        public const Byte NIMC_HSC_TRIGGER1       =0;
        public const Byte NIMC_HSC_TRIGGER2       =1;
        public const Byte NIMC_HSC_TRIGGER3       =2;
        public const Byte NIMC_HSC_TRIGGER4       =3;
        public const Byte NIMC_HSC_TRIGGER5       =4;
        public const Byte NIMC_HSC_TRIGGER6       =5;
        public const Byte NIMC_HSC_TRIGGER7       =6;
        public const Byte NIMC_HSC_TRIGGER8       =7;

        #endregion

        #region Encoder Filter Frequency

        public const Byte NIMC_ENCODER_FILTER_25_6MHz                     =0;
        public const Byte NIMC_ENCODER_FILTER_12_8MHz                     =1;
        public const Byte NIMC_ENCODER_FILTER_6_4MHz                      =2;
        public const Byte NIMC_ENCODER_FILTER_3_2MHz                      =3;
        public const Byte NIMC_ENCODER_FILTER_1_6MHz                      =4;
        public const Byte NIMC_ENCODER_FILTER_800KHz                      =5;
        public const Byte NIMC_ENCODER_FILTER_400KHz                      =6;
        public const Byte NIMC_ENCODER_FILTER_200KHz                      =7;
        public const Byte NIMC_ENCODER_FILTER_100KHz                      =8;
        public const Byte NIMC_ENCODER_FILTER_50KHz                       =9;
        public const Byte NIMC_ENCODER_FILTER_25KHz                       =10;
        public const Byte NIMC_ENCODER_FILTER_DISABLED                    =11;

        #endregion

        #endregion

        #region Digital IO Attributes

        // These public constant values are used to configure and control the digital IO lines.

        #region Digital IO Lines

        public const Byte NIMC_DIO_BIT0           =0;
        public const Byte NIMC_DIO_BIT1           =1;
        public const Byte NIMC_DIO_BIT2           =2;
        public const Byte NIMC_DIO_BIT3           =3;
        public const Byte NIMC_DIO_BIT4           =4;
        public const Byte NIMC_DIO_BIT5           =5;
        public const Byte NIMC_DIO_BIT6           =6;
        public const Byte NIMC_DIO_BIT7           =7;

        #endregion

        #region RTSI IO Lines

        public const Byte NIMC_RTSI_BIT0          =0;             // Obsolete NI-Motion=7.2
        public const Byte NIMC_RTSI_BIT1          =1;             // Obsolete NI-Motion=7.2
        public const Byte NIMC_RTSI_BIT2          =2;             // Obsolete NI-Motion=7.2
        public const Byte NIMC_RTSI_BIT3          =3;             // Obsolete NI-Motion=7.2
        public const Byte NIMC_RTSI_BIT4          =4;             // Obsolete NI-Motion=7.2
        public const Byte NIMC_RTSI_BIT5          =5;             // Obsolete NI-Motion=7.2
        public const Byte NIMC_RTSI_BIT6          =6;             // Obsolete NI-Motion=7.2
        public const Byte NIMC_RTSI_BIT7          =7;             // Obsolete NI-Motion=7.2

        #endregion

        #region IO Line Direction

        public const Byte NIMC_OUTPUT             =0;
        public const Byte NIMC_INPUT              =1;

        #endregion

        #region PWM Pulse Generation Mode

        public const Byte NIMC_PWM_FREQ_SCALE_512                         =0;
        public const Byte NIMC_PWM_FREQ_SCALE_1K                          =1;
        public const Byte NIMC_PWM_FREQ_SCALE_2K                          =2;
        public const Byte NIMC_PWM_FREQ_SCALE_4K                          =3;
        public const Byte NIMC_PWM_FREQ_SCALE_8K                          =4;
        public const Byte NIMC_PWM_FREQ_SCALE_16K                         =5;
        public const Byte NIMC_PWM_FREQ_SCALE_33K                         =6;
        public const Byte NIMC_PWM_FREQ_EXT_CLK_256                       =7;
        public const Byte NIMC_PWM_FREQ_SCALE_65K                         =8;
        public const Byte NIMC_PWM_FREQ_SCALE_131K                        =9;
        public const Byte NIMC_PWM_FREQ_SCALE_262K                        =10;
        public const Byte NIMC_PWM_FREQ_SCALE_524K                        =11;
        public const Byte NIMC_PWM_FREQ_SCALE_1048K                       =12;
        public const Byte NIMC_PWM_FREQ_SCALE_2097K                       =13;
        public const Byte NIMC_PWM_FREQ_SCALE_4194K                       =14;
        public const Byte NIMC_PWM_FREQ_EXT_CLK_33K                       =15;

        #endregion

        #endregion

        #region I/O Routing Attributes

        // These public constant values are used to configure IO lines routing. Refer to SelectSignal API.

        #region Source/Destination Lines

        public const Byte NIMC_RTSI0              =0;
        public const Byte NIMC_RTSI1              =1;
        public const Byte NIMC_RTSI2              =2;
        public const Byte NIMC_RTSI3              =3;
        public const Byte NIMC_RTSI4              =4;
        public const Byte NIMC_RTSI5              =5;
        public const Byte NIMC_RTSI6              =6;
        public const Byte NIMC_RTSI7              =7;

        #endregion

        #region Source Only Lines

        public const Byte NIMC_HS_CAPTURE1        =8;
        public const Byte NIMC_HS_CAPTURE2        =9;
        public const Byte NIMC_HS_CAPTURE3        =10;
        public const Byte NIMC_HS_CAPTURE4        =11;
        public const Byte NIMC_HS_CAPTURE5        =12;
        public const Byte NIMC_HS_CAPTURE6        =13;
        public const Byte NIMC_HS_CAPTURE7        =14;
        public const Byte NIMC_HS_CAPTURE8        =15;

        #endregion

        #region Destination Only Lines

        public const Byte NIMC_TRIGGER_INPUT      =8;
        public const Byte NIMC_BREAKPOINT1        =9;
        public const Byte NIMC_BREAKPOINT2        =10;
        public const Byte NIMC_BREAKPOINT3        =11;
        public const Byte NIMC_BREAKPOINT4        =12;
        public const Byte NIMC_BREAKPOINT5        =15;
        public const Byte NIMC_BREAKPOINT6        =16;
        public const Byte NIMC_BREAKPOINT7        =49;
        public const Byte NIMC_BREAKPOINT8        =50;
        public const Byte NIMC_PHASE_A1           =17;
        public const Byte NIMC_PHASE_A2           =18;
        public const Byte NIMC_PHASE_A3           =19;
        public const Byte NIMC_PHASE_A4           =20;
        public const Byte NIMC_PHASE_A5           =21;
        public const Byte NIMC_PHASE_A6           =22;
        public const Byte NIMC_PHASE_A7           =51;
        public const Byte NIMC_PHASE_A8           =52;
        public const Byte NIMC_PHASE_B1           =23;
        public const Byte NIMC_PHASE_B2           =24;
        public const Byte NIMC_PHASE_B3           =25;
        public const Byte NIMC_PHASE_B4           =26;
        public const Byte NIMC_PHASE_B5           =27;
        public const Byte NIMC_PHASE_B6           =28;
        public const Byte NIMC_PHASE_B7           =53;
        public const Byte NIMC_PHASE_B8           =54;
        public const Byte NIMC_INDEX1             =29;
        public const Byte NIMC_INDEX2             =30;
        public const Byte NIMC_INDEX3             =31;
        public const Byte NIMC_INDEX4             =32;
        public const Byte NIMC_INDEX5             =33;
        public const Byte NIMC_INDEX6             =34;
        public const Byte NIMC_INDEX7             =55;
        public const Byte NIMC_INDEX8             =56;
        public const Byte NIMC_RTSI_SOFTWARE_PORT =13;
        public const Byte NIMC_DONT_DRIVE         =14;
        public const Byte NIMC_PXI_STAR_TRIGGER   =60;

        #endregion

        #region Drive Signal Attributes

        public const Byte NIMC_DRIVE_SIGNAL_CLEAR_ALL                     =0;
        public const Byte NIMC_IN_POSITION_MODE                           =1;
        public const Byte NIMC_DRIVE_FAULT_MODE                           =2;

        #endregion

        #endregion

        #region Find Reference Attributes

        // These public constant values are used to configure find reference move. Refer to ReferenceMove related APIs.
        
        #region Reference Move Type

        public const Byte NIMC_FIND_HOME_REFERENCE                        =0x00;
        public const Byte NIMC_FIND_INDEX_REFERENCE                       =0x01;
        public const Byte NIMC_FIND_CENTER_REFERENCE                      =0x02;
        public const Byte NIMC_FIND_FORWARD_LIMIT_REFERENCE               =0x03;
        public const Byte NIMC_FIND_REVERSE_LIMIT_REFERENCE               =0x04;
        public const Byte NIMC_FIND_SEQUENCE_REFERENCE                    =0x05;
        public const Byte NIMC_MAX_FIND_TYPES                             =5;

        #endregion

        #region Reference Attributes

        public const Byte NIMC_INITIAL_SEARCH_DIRECTION                   =0x0;
        public const Byte NIMC_FINAL_APPROACH_DIRECTION                   =0x1;
        public const Byte NIMC_EDGE_TO_STOP_ON                            =0x2;
        public const Byte NIMC_SMART_ENABLE                               =0x3;
        public const Byte NIMC_ENABLE_RESET_POSITION                      =0x4;
        public const Byte NIMC_OFFSET_POSITION                            =0x5;
        public const Byte NIMC_PRIMARY_RESET_POSITION                     =0x6;
        public const Byte NIMC_SECONDARY_RESET_POSITION                   =0x7;
        public const Byte NIMC_APPROACH_VELOCITY_PERCENT                  =0x8;
        public const Byte NIMC_SEQUENCE_SEARCH_ORDER                      =0x9;
        public const Byte NIMC_ENABLE_SEARCH_DISTANCE                     =0xA;
        public const Byte NIMC_SEARCH_DISTANCE                            =0xB;
        public const Byte NIMC_PHASEA_REFERENCE_STATE                     =0xC;
        public const Byte NIMC_PHASEB_REFERENCE_STATE                     =0xD;

        #endregion

        #region Reference Status

        public const Byte NIMC_HOME_FOUND                                 =0x0;
        public const Byte NIMC_INDEX_FOUND                                =0x1;
        public const Byte NIMC_CENTER_FOUND                               =0x2;
        public const Byte NIMC_FORWARD_LIMIT_FOUND                        =0x3;
        public const Byte NIMC_REVERSE_LIMIT_FOUND                        =0x4;
        public const Byte NIMC_REFERENCE_FOUND                            =0x5;
        public const Byte NIMC_CURRENT_SEQUENCE_PHASE                     =0x6;
        public const Byte NIMC_FINDING_REFERENCE                          =0x7;

        #endregion

        #region Reference Search Direction

        public const Byte NIMC_FORWARD_DIRECTION  =0;
        public const Byte NIMC_REVERSE_DIRECTION  =1;

        #endregion

        #region Reference Approach Direction

        public const Byte NIMC_INTO_LIMIT         =0;
        public const Byte NIMC_AWAY_FROM_LIMIT    =1;

        #endregion

        #endregion

        #region Gearing Operation Mode

        public const Byte NIMC_ABSOLUTE_GEARING   =0; 
        public const Byte NIMC_RELATIVE_GEARING   =1;

        #endregion

        #endregion

        ///////////////////////////////////////////////////////////////////////////////
        // ADVANCED AND UTILITY ATTRIBUTES
        //    These public constant values are used to configure advanced feature and control
        //    utility attributes.
        ///////////////////////////////////////////////////////////////////////////////
        // Board Communication Status
        public const Byte NIMC_READY_TO_RECEIVE                           =0x01;    
        public const Byte NIMC_DATA_IN_RDB                                =0x02;    
        public const Byte NIMC_PACKET_ERROR                               =0x10;   
        public const Byte NIMC_E_STOP                                     =0x10;  
        public const Byte NIMC_POWER_UP_RESET                             =0x20;     
        public const Byte NIMC_MODAL_ERROR_MSG                            =0x40;     
        public const Byte NIMC_HARDWARE_FAIL                              =0x80;     

        // Error Retrieval Mode
        public const Byte NIMC_ERROR_ONLY                                 =0;
        public const Byte NIMC_FUNCTION_NAME_ONLY                         =1;
        public const Byte NIMC_RESOURCE_NAME_ONLY                         =2;
        public const Byte NIMC_COMBINED_DESCRIPTION                       =3;

        // Attributes To Write
        public const UInt32 NIMC_BP_WINDOW                                  =0x0200;      
        public const UInt32 NIMC_PULL_IN_WINDOW                             =0x0400;      
        public const UInt32 NIMC_PULL_IN_TRIES                              =0x0401;      
        public const UInt32 NIMC_STOP_TYPE_ON_SWITCH                        =0x0403; 
        public const UInt32 NIMC_STEP_DUTY_CYCLE                            =0x0600;      
        public const UInt32 NIMC_DECEL_STOP_JERK                            =0x0500;      
        public const UInt32 NIMC_HOST_THROTTLE                              =0x04D2;   // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_RANGE_CHECK                                =0x1A85;   // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_BOUNCY_LIMIT_DETECTION                     =0x0402;   // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_FE_HALT_COMPENSATION                       =0x0900;   // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_HOST_LOOP_TIME                             =0x0800;   // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_BP_PULSE_WIDTH                             =0x0201;   // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_STICTION_MODE                              =0x0406;   // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_STICTION_MAX_DEADBAND                      =0x0407;   // Obsolete NI-Motion=7.2  
        public const UInt32 NIMC_STICTION_MIN_DEADBAND                      =0x0408;   // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_STICTION_ITERM_OFFSET_FWD                  =0x0409;   // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_STICTION_ITERM_OFFSET_REV                  =0x040A;   // Obsolete NI-Motion=7.2

        // Attributes To Read
        public const UInt32 NIMC_PROGRAM_AUTOSTART                          =0x0300;      
        public const UInt32 NIMC_GEARING_ENABLED_STATUS                     =0x0301;
        public const UInt32 NIMC_BUS_TIMEOUT                                =0x0100;   // Obsolete NI-Motion=7.2

        // Stepper Output Duty Cycle
        public const Byte NIMC_STEP_DUTY_CYCLE_25                         =25;
        public const Byte NIMC_STEP_DUTY_CYCLE_50                         =50;

        // Pull In Window Attribute
        public const UInt32 NIMC_MAX_PULL_IN_WINDOW =32767;          // Obsolete NI-Motion=7.2
        public const UInt32 NIMC_MAX_PULL_IN_TRIES = 32767;         // Obsolete NI-Motion=7.2

        // Decel Stop Jerk Constants
        public const Byte  NIMC_MAX_DECEL_STOP_JERK                       =1;       // Obsolete NI-Motion=7.2

        // Communication Attribute
        public const Byte NIMC_SEND_COMMAND       =0;             // Obsolete NI-Motion=7.2 
        public const Byte NIMC_SEND_AND_READ      =1;             // Obsolete NI-Motion=7.2
        public const Byte NIMC_READ_RDB           =2;             // Obsolete NI-Motion=7.2

        //=1394 Communication Watchdog Parameter
        public const Byte NIMC_ENABLE_1394_WATCHDOG                       =1;       // Obsolete NI-Motion=7.2
        public const Byte NIMC_DISABLE_1394_WATCHDOG                      =0;       // Obsolete NI-Motion=7.2

        // Performance Options Attributes
        public const Byte NIMC_EXTENDED_WATCHDOG                          =1;
        public const Byte NIMC_EXTENDED_ARC_INTERVAL                      =2;


        ///////////////////////////////////////////////////////////////////////////////
        // RESOURCE IDS
        //    These public constant values are used to indicate valid resource ID to be used 
        //    with NI-Motion APIs.
        ///////////////////////////////////////////////////////////////////////////////
        // Multi-Resource Controls
        public const Byte NIMC_AXIS_CTRL                                  =0x00;
        public const Byte NIMC_VECTOR_SPACE_CTRL                          =0x10;
        public const Byte NIMC_ENCODER_CTRL                               =0x20;
        public const Byte NIMC_DAC_CTRL                                   =0x30;
        public const Byte NIMC_STEP_OUTPUT_CTRL                           =0x40;
        public const Byte NIMC_ADC_CTRL                                   =0x50;
        public const Byte NIMC_ALTERNATE_EX_CTRL                          =0x60;
        public const Byte NIMC_IO_PORT_CTRL                               =0x00;
        public const Byte NIMC_PWM_CTRL                                   =0x00;
        public const Byte NIMC_PROGRAM_CTRL                               =0x00;
        public const Byte NIMC_SECONDARY_ENCODER_CTRL                     =0x70;
        public const Byte NIMC_SECONDARY_DAC_CTRL                         =0x80;
        public const Byte NIMC_SECONDARY_ADC_CTRL                         =0x90;
        public const Byte NIMC_ALTERNATE_CTRL                             =0xA0;
        public const Byte NIMC_AXIS_EX_CTRL                               =0xB0;
        public const Byte NIMC_ENCODER_EX_CTRL                            =0xC0;
        public const Byte NIMC_DAC_EX_CTRL                                =0xD0;
        public const Byte NIMC_STEP_OUTPUT_EX_CTRL                        =0xE0;
        public const Byte NIMC_ADC_EX_CTRL                                =0xF0;

        // Axis Resource IDs
        public const byte NIMC_NOAXIS             =0x00;
        public const byte NIMC_AXIS1              =0x01;
        public const byte NIMC_AXIS2              =0x02;
        public const byte NIMC_AXIS3              =0x03;
        public const byte NIMC_AXIS4              =0x04;
        public const byte NIMC_AXIS5              =0x05;
        public const byte NIMC_AXIS6              =0x06;
        public const byte NIMC_AXIS7              =0x07;
        public const byte NIMC_AXIS8              =0x08;
        public const byte NIMC_AXIS9              =0x09;
        public const byte NIMC_AXIS10             =0x0A;
        public const byte NIMC_AXIS11             =0x0B;
        public const byte NIMC_AXIS12             =0x0C;
        public const byte NIMC_AXIS13             =0x0D;
        public const byte NIMC_AXIS14             =0x0E;
        public const byte NIMC_AXIS15             =0x0F;
        public const byte NIMC_AXIS16             =0xB1;
        public const byte NIMC_AXIS17             =0xB2;
        public const byte NIMC_AXIS18             =0xB3;
        public const byte NIMC_AXIS19             =0xB4;
        public const byte NIMC_AXIS20             =0xB5;
        public const byte NIMC_AXIS21             =0xB6;
        public const byte NIMC_AXIS22             =0xB7;
        public const byte NIMC_AXIS23             =0xB8;
        public const byte NIMC_AXIS24             =0xB9;
        public const byte NIMC_AXIS25             =0xBA;
        public const byte NIMC_AXIS26             =0xBB;
        public const byte NIMC_AXIS27             =0xBC;
        public const byte NIMC_AXIS28             =0xBD;
        public const byte NIMC_AXIS29             =0xBE;
        public const byte NIMC_AXIS30             =0xBF;

        // Coordinate (Vector Space) Resource IDs
        public const byte NIMC_VECTOR_SPACE1      =0x11;
        public const byte NIMC_VECTOR_SPACE2      =0x12;
        public const byte NIMC_VECTOR_SPACE3      =0x13;
        public const byte NIMC_VECTOR_SPACE4      =0x14;
        public const byte NIMC_VECTOR_SPACE5      =0x15;
        public const byte NIMC_VECTOR_SPACE6      =0x16;
        public const byte NIMC_VECTOR_SPACE7      =0x17;
        public const byte NIMC_VECTOR_SPACE8      =0x18;
        public const byte NIMC_VECTOR_SPACE9      =0x19;
        public const byte NIMC_VECTOR_SPACE10     =0x1A;
        public const byte NIMC_VECTOR_SPACE11     =0x1B;
        public const byte NIMC_VECTOR_SPACE12     =0x1C;
        public const byte NIMC_VECTOR_SPACE13     =0x1D;
        public const byte NIMC_VECTOR_SPACE14     =0x1E;
        public const byte NIMC_VECTOR_SPACE15     =0x1F;

        // Encoder Resource IDs
        public const byte NIMC_ENCODER1           =0x21;
        public const byte NIMC_ENCODER2           =0x22;
        public const byte NIMC_ENCODER3           =0x23;
        public const byte NIMC_ENCODER4           =0x24;
        public const byte NIMC_ENCODER5           =0x25;
        public const byte NIMC_ENCODER6           =0x26;
        public const byte NIMC_ENCODER7           =0x27;
        public const byte NIMC_ENCODER8           =0x28;
        public const byte NIMC_ENCODER9           =0x29;
        public const byte NIMC_ENCODER10          =0x2A;
        public const byte NIMC_ENCODER11          =0x2B;
        public const byte NIMC_ENCODER12          =0x2C;
        public const byte NIMC_ENCODER13          =0x2D;
        public const byte NIMC_ENCODER14          =0x2E;
        public const byte NIMC_ENCODER15          =0x2F;
        public const byte NIMC_ENCODER16          =0xC1;
        public const byte NIMC_ENCODER17          =0xC2;
        public const byte NIMC_ENCODER18          =0xC3;
        public const byte NIMC_ENCODER19          =0xC4;
        public const byte NIMC_ENCODER20          =0xC5;
        public const byte NIMC_ENCODER21          =0xC6;
        public const byte NIMC_ENCODER22          =0xC7;
        public const byte NIMC_ENCODER23          =0xC8;
        public const byte NIMC_ENCODER24          =0xC9;
        public const byte NIMC_ENCODER25          =0xCA;
        public const byte NIMC_ENCODER26          =0xCB;
        public const byte NIMC_ENCODER27          =0xCC;
        public const byte NIMC_ENCODER28          =0xCD;
        public const byte NIMC_ENCODER29          =0xCE;
        public const byte NIMC_ENCODER30          =0xCF;

        // Secondary Encoder Resource IDs
        public const Byte NIMC_SECONDARY_ENCODER1                               =0x71;
        public const Byte NIMC_SECONDARY_ENCODER2                               =0x72;
        public const Byte NIMC_SECONDARY_ENCODER3                               =0x73;
        public const Byte NIMC_SECONDARY_ENCODER4                               =0x74;
        public const Byte NIMC_SECONDARY_ENCODER5                               =0x75;
        public const Byte NIMC_SECONDARY_ENCODER6                               =0x76;
        public const Byte NIMC_SECONDARY_ENCODER7                               =0x77;
        public const Byte NIMC_SECONDARY_ENCODER8                               =0x78;
        public const Byte NIMC_SECONDARY_ENCODER9                               =0x79;
        public const Byte NIMC_SECONDARY_ENCODER10                              =0x7A;
        public const Byte NIMC_SECONDARY_ENCODER11                              =0x7B;
        public const Byte NIMC_SECONDARY_ENCODER12                              =0x7C;
        public const Byte NIMC_SECONDARY_ENCODER13                              =0x7D;
        public const Byte NIMC_SECONDARY_ENCODER14                              =0x7E;
        public const Byte NIMC_SECONDARY_ENCODER15                              =0x7F;

        // DAC Output Resource IDs
        public const Byte NIMC_DAC1               =0x31;
        public const Byte NIMC_DAC2               =0x32;
        public const Byte NIMC_DAC3               =0x33;
        public const Byte NIMC_DAC4               =0x34;
        public const Byte NIMC_DAC5               =0x35;
        public const Byte NIMC_DAC6               =0x36;
        public const Byte NIMC_DAC7               =0x37;
        public const Byte NIMC_DAC8               =0x38;
        public const Byte NIMC_DAC9               =0x39;
        public const Byte NIMC_DAC10              =0x3A;
        public const Byte NIMC_DAC11              =0x3B;
        public const Byte NIMC_DAC12              =0x3C;
        public const Byte NIMC_DAC13              =0x3D;
        public const Byte NIMC_DAC14              =0x3E;
        public const Byte NIMC_DAC15              =0x3F;
        public const Byte NIMC_DAC16              =0xD1;
        public const Byte NIMC_DAC17              =0xD2;
        public const Byte NIMC_DAC18              =0xD3;
        public const Byte NIMC_DAC19              =0xD4;
        public const Byte NIMC_DAC20              =0xD5;
        public const Byte NIMC_DAC21              =0xD6;
        public const Byte NIMC_DAC22              =0xD7;
        public const Byte NIMC_DAC23              =0xD8;
        public const Byte NIMC_DAC24              =0xD9;
        public const Byte NIMC_DAC25              =0xDA;
        public const Byte NIMC_DAC26              =0xDB;
        public const Byte NIMC_DAC27              =0xDC;
        public const Byte NIMC_DAC28              =0xDD;
        public const Byte NIMC_DAC29              =0xDE;
        public const Byte NIMC_DAC30              =0xDF;

        // Secondary DAC Output Resource IDs
        public const Byte NIMC_SECONDARY_DAC1     =0x91;
        public const Byte NIMC_SECONDARY_DAC2     =0x92;
        public const Byte NIMC_SECONDARY_DAC3     =0x93;
        public const Byte NIMC_SECONDARY_DAC4     =0x94;
        public const Byte NIMC_SECONDARY_DAC5     =0x95;
        public const Byte NIMC_SECONDARY_DAC6     =0x96;
        public const Byte NIMC_SECONDARY_DAC7     =0x97;
        public const Byte NIMC_SECONDARY_DAC8     =0x98;
        public const Byte NIMC_SECONDARY_DAC9     =0x99;
        public const Byte NIMC_SECONDARY_DAC10    =0x9A;
        public const Byte NIMC_SECONDARY_DAC11    =0x9B;
        public const Byte NIMC_SECONDARY_DAC12    =0x9C;
        public const Byte NIMC_SECONDARY_DAC13    =0x9D;
        public const Byte NIMC_SECONDARY_DAC14    =0x9E;
        public const Byte NIMC_SECONDARY_DAC15    =0x9F;

        // Stepper Output Resource IDs
        public const Byte NIMC_STEP_OUTPUT1       =0x41;
        public const Byte NIMC_STEP_OUTPUT2       =0x42;
        public const Byte NIMC_STEP_OUTPUT3       =0x43;
        public const Byte NIMC_STEP_OUTPUT4       =0x44;
        public const Byte NIMC_STEP_OUTPUT5       =0x45;
        public const Byte NIMC_STEP_OUTPUT6       =0x46;
        public const Byte NIMC_STEP_OUTPUT7       =0x47;
        public const Byte NIMC_STEP_OUTPUT8       =0x48;
        public const Byte NIMC_STEP_OUTPUT9       =0x49;
        public const Byte NIMC_STEP_OUTPUT10      =0x4A;
        public const Byte NIMC_STEP_OUTPUT11      =0x4B;
        public const Byte NIMC_STEP_OUTPUT12      =0x4C;
        public const Byte NIMC_STEP_OUTPUT13      =0x4D;
        public const Byte NIMC_STEP_OUTPUT14      =0x4E;
        public const Byte NIMC_STEP_OUTPUT15      =0x4F;
        public const Byte NIMC_STEP_OUTPUT16      =0xE1;
        public const Byte NIMC_STEP_OUTPUT17      =0xE2;
        public const Byte NIMC_STEP_OUTPUT18      =0xE3;
        public const Byte NIMC_STEP_OUTPUT19      =0xE4;
        public const Byte NIMC_STEP_OUTPUT20      =0xE5;
        public const Byte NIMC_STEP_OUTPUT21      =0xE6;
        public const Byte NIMC_STEP_OUTPUT22      =0xE7;
        public const Byte NIMC_STEP_OUTPUT23      =0xE8;
        public const Byte NIMC_STEP_OUTPUT24      =0xE9;
        public const Byte NIMC_STEP_OUTPUT25      =0xEA;
        public const Byte NIMC_STEP_OUTPUT26      =0xEB;
        public const Byte NIMC_STEP_OUTPUT27      =0xEC;
        public const Byte NIMC_STEP_OUTPUT28      =0xED;
        public const Byte NIMC_STEP_OUTPUT29      =0xEE;
        public const Byte NIMC_STEP_OUTPUT30      =0xEF;

        // ADC Input Resource IDs
        public const Byte NIMC_ADC1               =0x51;
        public const Byte NIMC_ADC2               =0x52;
        public const Byte NIMC_ADC3               =0x53;
        public const Byte NIMC_ADC4               =0x54;
        public const Byte NIMC_ADC5               =0x55;
        public const Byte NIMC_ADC6               =0x56;
        public const Byte NIMC_ADC7               =0x57;
        public const Byte NIMC_ADC8               =0x58;
        public const Byte NIMC_ADC9               =0x59;
        public const Byte NIMC_ADC10              =0x5A;
        public const Byte NIMC_ADC11              =0x5B;
        public const Byte NIMC_ADC12              =0x5C;
        public const Byte NIMC_ADC13              =0x5D;
        public const Byte NIMC_ADC14              =0x5E;
        public const Byte NIMC_ADC15              =0x5F;
        public const Byte NIMC_ADC16              =0xF1;
        public const Byte NIMC_ADC17              =0xF2;
        public const Byte NIMC_ADC18              =0xF3;
        public const Byte NIMC_ADC19              =0xF4;
        public const Byte NIMC_ADC20              =0xF5;
        public const Byte NIMC_ADC21              =0xF6;
        public const Byte NIMC_ADC22              =0xF7;
        public const Byte NIMC_ADC23              =0xF8;
        public const Byte NIMC_ADC24              =0xF9;
        public const Byte NIMC_ADC25              =0xFA;
        public const Byte NIMC_ADC26              =0xFB;
        public const Byte NIMC_ADC27              =0xFC;
        public const Byte NIMC_ADC28              =0xFD;
        public const Byte NIMC_ADC29              =0xFE;
        public const Byte NIMC_ADC30              =0xFF;

        // Secondary ADC Input Resource IDs
        public const Byte NIMC_SECONDARY_ADC1     =0x81;
        public const Byte NIMC_SECONDARY_ADC2     =0x82;
        public const Byte NIMC_SECONDARY_ADC3     =0x83;
        public const Byte NIMC_SECONDARY_ADC4     =0x84;
        public const Byte NIMC_SECONDARY_ADC5     =0x85;
        public const Byte NIMC_SECONDARY_ADC6     =0x86;
        public const Byte NIMC_SECONDARY_ADC7     =0x87;
        public const Byte NIMC_SECONDARY_ADC8     =0x88;
        public const Byte NIMC_SECONDARY_ADC9     =0x89;
        public const Byte NIMC_SECONDARY_ADC10    =0x8A;
        public const Byte NIMC_SECONDARY_ADC11    =0x8B;
        public const Byte NIMC_SECONDARY_ADC12    =0x8C;
        public const Byte NIMC_SECONDARY_ADC13    =0x8D;
        public const Byte NIMC_SECONDARY_ADC14    =0x8E;
        public const Byte NIMC_SECONDARY_ADC15    =0x8F;

        // Bidirectional Digital IO Port Resource IDs
        public const Byte NIMC_IO_PORT1           =0x01;
        public const Byte NIMC_IO_PORT2           =0x02;
        public const Byte NIMC_IO_PORT3           =0x03;
        public const Byte NIMC_IO_PORT4           =0x04;
        public const Byte NIMC_IO_PORT5           =0x05;
        public const Byte NIMC_IO_PORT6           =0x06;
        public const Byte NIMC_IO_PORT7           =0x07;
        public const Byte NIMC_IO_PORT8           =0x08;
        public const Byte NIMC_IO_PORT9           =0x09;
        public const Byte NIMC_IO_PORT10          =0x0A;
        public const Byte NIMC_IO_PORT11          =0x0B;
        public const Byte NIMC_IO_PORT12          =0x0C;
        public const Byte NIMC_IO_PORT13          =0x0D;
        public const Byte NIMC_IO_PORT14          =0x0E;
        public const Byte NIMC_IO_PORT15          =0x0F;
        public const Byte NIMC_RTSI_PORT          =0x09;
        public const Byte NIMC_HSC_PORT           =0x0A;
        public const Byte NIMC_IO_PORT16          =0x11;
        public const Byte NIMC_IO_PORT17          =0x12;
        public const Byte NIMC_IO_PORT18          =0x13;
        public const Byte NIMC_IO_PORT19          =0x14;
        public const Byte NIMC_IO_PORT20          =0x15;
        public const Byte NIMC_IO_PORT21          =0x16;
        public const Byte NIMC_IO_PORT22          =0x17;
        public const Byte NIMC_IO_PORT23          =0x18;
        public const Byte NIMC_IO_PORT24          =0x19;
        public const Byte NIMC_IO_PORT25          =0x1A;
        public const Byte NIMC_IO_PORT26          =0x1B;
        public const Byte NIMC_IO_PORT27          =0x1C;
        public const Byte NIMC_IO_PORT28          =0x1D;
        public const Byte NIMC_IO_PORT29          =0x1E;
        public const Byte NIMC_IO_PORT30          =0x1F;

        // Digital Input Port Resource IDs
        public const Byte NIMC_DIGITAL_INPUT_PORT                         =0;
        public const Byte NIMC_DIGITAL_INPUT_PORT1                         =(1  | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT2                         =(2  | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT3                         =(3  | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT4                         =(4  | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT5                         =(5  | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT6                         =(6  | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT7                         =(7  | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT8                         =(8  | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT9                         =(9  | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT10                        =(10 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT11                        =(11 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT12                        =(12 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT13                        =(13 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT14                        =(14 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT15                        =(15 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT16                        =(16 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT17                        =(17 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT18                        =(18 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT19                        =(19 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT20                        =(20 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT21                        =(21 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT22                        =(22 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT23                        =(23 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT24                        =(24 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT25                        =(25 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT26                        =(26 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT27                        =(27 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT28                        =(28 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT29                        =(29 | NIMC_DIGITAL_INPUT_PORT);
        public const Byte NIMC_DIGITAL_INPUT_PORT30                        =(30 | NIMC_DIGITAL_INPUT_PORT);

        // Digital Output Port Resource IDs
        public const Byte NIMC_DIGITAL_OUTPUT_PORT                        =0x80;
        public const Byte NIMC_DIGITAL_OUTPUT_PORT1                        =(1  | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT2                        =(2  | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT3                        =(3  | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT4                        =(4  | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT5                        =(5  | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT6                        =(6  | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT7                        =(7  | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT8                        =(8  | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT9                        =(9  | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT10                       =(10 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT11                       =(11 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT12                       =(12 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT13                       =(13 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT14                       =(14 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT15                       =(15 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT16                       =(16 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT17                       =(17 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT18                       =(18 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT19                       =(19 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT20                       =(20 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT21                       =(21 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT22                       =(22 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT23                       =(23 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT24                       =(24 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT25                       =(25 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT26                       =(26 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT27                       =(27 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT28                       =(28 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT29                       =(29 | NIMC_DIGITAL_OUTPUT_PORT);
        public const Byte NIMC_DIGITAL_OUTPUT_PORT30                       =(30 | NIMC_DIGITAL_OUTPUT_PORT);

        // Secondary Input Capture Resource IDs
        public const Byte NIMC_SECOND_HS_CAPTURE                          =0xA0;
        public const Byte NIMC_SECOND_HS_CAPTURE_EX                       =0x60;
        public const Byte NIMC_SECOND_HS_CAPTURE1                          =(0x01 | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE2                          =(0x02 | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE3                          =(0x03 | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE4                          =(0x04 | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE5                          =(0x05 | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE6                          =(0x06 | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE7                          =(0x07 | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE8                          =(0x08 | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE9                          =(0x09 | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE10                         =(0x0A | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE11                         =(0x0B | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE12                         =(0x0C | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE13                         =(0x0D | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE14                         =(0x0E | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE15                         =(0x0F | NIMC_SECOND_HS_CAPTURE);
        public const Byte NIMC_SECOND_HS_CAPTURE16                         =(0x01 | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE17                         =(0x02 | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE18                         =(0x03 | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE19                         =(0x04 | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE20                         =(0x05 | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE21                         =(0x06 | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE22                         =(0x07 | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE23                         =(0x08 | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE24                         =(0x09 | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE25                         =(0x0A | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE26                         =(0x0B | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE27                         =(0x0C | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE28                         =(0x0D | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE29                         =(0x0E | NIMC_SECOND_HS_CAPTURE_EX);
        public const Byte NIMC_SECOND_HS_CAPTURE30                         =(0x0F | NIMC_SECOND_HS_CAPTURE_EX);

        // Secondary Position Compare Resource IDs
        public const Byte NIMC_SECOND_BREAKPOINT                          =0xA0;
        public const Byte NIMC_SECOND_BREAKPOINT_EX                       =0x60;
        public const Byte NIMC_SECOND_BREAKPOINT1                          =(0x01 | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT2                          =(0x02 | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT3                          =(0x03 | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT4                          =(0x04 | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT5                          =(0x05 | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT6                          =(0x06 | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT7                          =(0x07 | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT8                          =(0x08 | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT9                          =(0x09 | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT10                         =(0x0A | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT11                         =(0x0B | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT12                         =(0x0C | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT13                         =(0x0D | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT14                         =(0x0E | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT15                         =(0x0F | NIMC_SECOND_BREAKPOINT);
        public const Byte NIMC_SECOND_BREAKPOINT16                         =(0x01 | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT17                         =(0x02 | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT18                         =(0x03 | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT19                         =(0x04 | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT20                         =(0x05 | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT21                         =(0x06 | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT22                         =(0x07 | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT23                         =(0x08 | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT24                         =(0x09 | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT25                         =(0x0A | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT26                         =(0x0B | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT27                         =(0x0C | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT28                         =(0x0D | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT29                         =(0x0E | NIMC_SECOND_BREAKPOINT_EX);
        public const Byte NIMC_SECOND_BREAKPOINT30                         =(0x0F | NIMC_SECOND_BREAKPOINT_EX);

        // Secondary PID Control Resource IDs
        public const Byte NIMC_SECOND_PID1                                 =(0x01 | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID2                                 =(0x02 | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID3                                 =(0x03 | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID4                                 =(0x04 | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID5                                 =(0x05 | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID6                                 =(0x06 | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID7                                 =(0x07 | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID8                                 =(0x08 | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID9                                 =(0x09 | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID10                                =(0x0A | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID11                                =(0x0B | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID12                                =(0x0C | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID13                                =(0x0D | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID14                                =(0x0E | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID15                                =(0x0F | NIMC_ALTERNATE_CTRL);
        public const Byte NIMC_SECOND_PID16                                =(0x01 | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID17                                =(0x02 | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID18                                =(0x03 | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID19                                =(0x04 | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID20                                =(0x05 | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID21                                =(0x06 | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID22                                =(0x07 | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID23                                =(0x08 | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID24                                =(0x09 | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID25                                =(0x0A | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID26                                =(0x0B | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID27                                =(0x0C | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID28                                =(0x0D | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID29                                =(0x0E | NIMC_ALTERNATE_EX_CTRL);
        public const Byte NIMC_SECOND_PID30                                =(0x0F | NIMC_ALTERNATE_EX_CTRL);

        // PWM Output Resource IDs
        public const Byte NIMC_PWM1               =0x01;
        public const Byte NIMC_PWM2               =0x02;

        #region Enums

        // Motion IO Attributes (from motncnst.h)
        public enum TnimcMotionIOParameter
        {
           TnimcMotionIOParameterForwardLimitEnable = 0,
           TnimcMotionIOParameterReverseLimitEnable,
           TnimcMotionIOParameterForwardSoftwareLimitEnable,
           TnimcMotionIOParameterReverseSoftwareLimitEnable,
           TnimcMotionIOParameterHomeInputEnable,
           TnimcMotionIOParameterForwardLimitPolarity,
           TnimcMotionIOParameterReverseLimitPolarity,
           TnimcMotionIOParameterHomeInputPolarity,
           TnimcMotionIOParameterForwardSoftwareLimitPosition,
           TnimcMotionIOParameterReverseSoftwareLimitPosition,
           TnimcMotionIOParameterInhibitInEnable,
           TnimcMotionIOParameterInhibitInPolarity,
           TnimcMotionIOParameterInPositionEnable,
           TnimcMotionIOParameterInPositionPolarity,
        } ;

        // Motion IO Status (from motncnst.h)
        public enum TnimcMotionIOExecution
        {
           TnimcMotionIOExecutionForwardLimitStatus = 0,
           TnimcMotionIOExecutionReverseLimitStatus,
           TnimcMotionIOExecutionForwardSoftwareLimitStatus,
           TnimcMotionIOExecutionReverseSoftwareLimitStatus,
           TnimcMotionIOExecutionHomeInputStatus,
           TnimcMotionIOExecutionInhibitInStatus,
           TnimcMotionIOExecutionInPositionStatus,
        } ;

        // Axis Configuration Attributes (from motncnst.h)
        public enum TnimcAxisConfigurationParameter
        {
           TnimcAxisConfigurationParameterEnable = 0,
        };
        
        // Move Constraint Attributes (from motncnst.h)
        public enum TnimcMoveConstraint
        {
           TnimcMoveConstraintVelocity = 0,
           TnimcMoveConstraintAcceleration,
           TnimcMoveConstraintDeceleration,
           TnimcMoveConstraintAccelerationJerk,
           TnimcMoveConstraintDecelerationJerk,
        } ;

        
        // Camming Attributes (from motncnst.h)
        public enum TnimcCammingParameter
        {
           TnimcCammingParameterMasterCycle = 0,
           TnimcCammingParameterMasterOffset,
           TnimcCammingParameterSlaveOffset,
        } ;

        #endregion

        #region Structs

        /* PID 
        **    This structure is passed to flex_load_pid_parameters() function
        **    to configure all the control gains at the same time.
        */
        public struct PID
        {
            public UInt16 kp;
            public UInt16 ki;
            public UInt16 ilim;
            public UInt16 kd;
            public UInt16 td;
            public UInt16 kv;
            public UInt16 aff;
            public UInt16 vff;
        };

        /* REGISTRY 
        **   This structure is passed to flex_read_registry() function in order to
        **   return info about the object registry entry. 
        */
        public struct REGISTRY
        {
            public UInt16 device;
            public UInt16 type;
            public UInt32 pstart;
            public UInt32 size;
        };

        /* NIMC_CAMMING_ENABLE_DATA
        **    This structure is used to enable/disable camming operation for
        **    multiple axis at the same time.
        */
        public struct NIMC_CAMMING_ENABLE_DATA
        {
            public Int32 axisIndex;
            public Byte enable;
            public Double position;        
        };

        /* NIMC_DATA
        **    This structure is used for various configurtion functions to 
        **    load parameter with different data types.
        */
        public struct NIMC_DATA
        {
            public Int32 longData;
            public Byte boolData;
            public Double doubleData;
        };

        #endregion

        #region Functions

        #region Initialization

        // Initializes the specified controller according to the settings as configured in Measurement & Automation Explorer.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_initialize_controller (byte BoardID, sbyte[] settingsName);

        #endregion

        #region Axis & Resource Configuration

        // Configures an axis by defining its feedback and output resources.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_config_axis(byte BoardID, byte axis, byte primaryFeedback, byte secondaryFeedback,
            byte primaryOutput, byte secondaryOutput);

        // Configures the criteria for the Move Complete status to be True.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_config_mc_criteria(byte BoardID, byte axis, ushort criteria, ushort deadband, 
            byte delay, byte minPulse);

        // Configures the drive mode, output mode, and polarity of a stepper output.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_configure_stepper_output(byte BoardID, byte axisOrStepperOutput, ushort outputMode, 
            ushort polarity, ushort driveMode);

        // Defines the axes that are associated with a vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_config_vect_spc(byte BoardID, byte vectorSpace, byte xAxis, byte yAxis, byte zAxis);

        // Enables the operating axes and defines the PID and trajectory update rate.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_enable_axis(byte BoardID, byte reserved, byte PIDRate, ushort axisMap);

        // Sets an advanced control loop parameter for a given axis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_advanced_control_parameter(byte BoardID, byte axis, ushort parameterType, 
            uint value, byte inputVector);

        // Sets a commutation parameter for a given axis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_commutation_parameter(byte BoardID, byte axis, ushort attribute, double value);

        // Loads the quadrature counts or steps per revolution for an axis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_counts_steps_rev(byte BoardID, byte axis, ushort unitType, uint countsOrSteps);

        // Loads all 8 PID control loop parameters for an axis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_pid_parameters(byte BoardID, byte axis, PID PIDValues, byte inputVector);

        // Loads a single PID control loop parameter for an axis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_single_pid_parameter(byte BoardID, byte axis, ushort parameterType, 
            ushort PIDValue, byte inputVector);

        // Sets a stepper axis to operate in either open-loop, closed-loop, or P-Command mode.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_set_stepper_loop_mode(byte BoardID, byte axis, ushort loopMode);

        // Loads the velocity filter parameters and sets the velocity threshold, above which an axis is considered running.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_velocity_filter_parameter(byte BoardID, byte axis, ushort filterDistance, 
            ushort filterTime, byte inputVector);

        #endregion

        #region Trajectory

        // Checks the blend complete status for an axis, vector space, group of axes, or group of vector spaces.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_check_blend_complete_status(byte BoardID, byte axisOrVectorSpace,
            ushort axisOrVSMap, out ushort blendComplete);

        // Checks the move complete status for an axis, vector space, group of axes, or group of vector spaces.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_check_move_complete_status(byte BoardID, byte axisOrVectorSpace,
            ushort axisOrVSMap, out ushort moveComplete);

        // Loads the maximum acceleration and/or deceleration value for an axis or vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_acceleration(byte BoardID, byte axisOrVectorSpace, ushort accelerationType, 
            uint acceleration, byte inputVector);

        // Loads the target position for the next axis move.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_target_pos(byte BoardID, byte axis, int targetPosition, byte inputVector);

        // Loads the maximum velocity for an axis or vector space. 
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_velocity(byte BoardID, byte axisOrVectorSpace, int velocity, byte inputVector);

        // Loads the axis target positions for the next vector space move.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_vs_pos(byte BoardID, byte vectorSpace, int xPosition, int yPosition, 
            int zPosition, byte inputVector);

        // Reads the motion status on a per-axis basis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_axis_status(byte BoardID, byte axis, byte returnVector);

        // Reads the motion status on a per-axis basis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_axis_status_rtn(byte BoardID, byte axis, out ushort axisStatus);

        // Reads the position of an axis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_pos(byte BoardID, byte axis, byte returnVector);

        // Reads the position of an axis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_pos_rtn(byte BoardID, byte axis, out int position);

        // Reads the filtered velocity of an axis or vector space in RPM.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_rpm(byte BoardID, byte axisOrVectorSpace, byte returnVector);

        // Reads the filtered velocity of an axis or vector space in RPM.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_rpm_rtn(byte BoardID, byte axisOrVectorSpace, out double RPM);

        // Reads the filtered velocity of an axis or vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_velocity(byte BoardID, byte axisOrVectorSpace, byte returnVector);

        // Reads the filtered velocity of an axis or vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_velocity_rtn(byte BoardID, byte axisOrVectorSpace, out int velocity);

        // Reads the position of all axes in a vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_vs_pos(byte BoardID, byte vectorSpace, byte returnVector);

        // Reads the position of all axes in a vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_vs_pos_rtn(byte BoardID, byte vectorSpace, out int xPosition, 
            out int yPosition, out int zPosition);

        // Resets the axis position to the specified position, taking following error into account.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_reset_pos(byte BoardID, byte axis, int position1, int position2, byte inputVector);

        // Sets the operation mode for an axis or vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_set_op_mode(byte BoardID, byte axisOrVectorSpace, ushort operationMode);

        // Waits up to the specified period of time for a blend to complete on an axis, vector space, 
        // group of axes, or group of vector spaces.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_wait_for_blend_complete(byte BoardID, byte axisOrVectorSpace, ushort axisOrVSMap,
            uint timeout, int pollInterval, out ushort blendComplete);

        // Waits up to the specified period of time for a move to complete on an axis, vector space, group of axes, 
        // or group of vector spaces.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_wait_for_move_complete(byte BoardID, byte axisOrVectorSpace, ushort axisOrVSMap,
            uint timeout, int pollInterval, out ushort moveComplete);

        # region Arcs

        // Loads parameters for making a circular arc move in a 2D or 3D vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_circular_arc(byte BoardID, byte vectorSpace, uint radius, double startAngle,
            double travelAngle, byte inputVector);

        // Loads parameters for making a helical arc move in a 3D vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_helical_arc(byte BoardID, byte vectorSpace, uint radius, double startAngle,
            double travelAngle, int linearTravel, byte inputVector);

        // Loads parameters for making a spherical arc move in a 3D vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_spherical_arc(byte BoardID, byte vectorSpace, uint radius, double planePitch,
            double planeYaw, double startAngle, double travelAngle, byte inputVector);

        #endregion

        #region Buffered Operations

        // Returns information about the current state of the buffer.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_check_buffer(byte BoardID, byte buffer, byte returnVector);

        // Returns information about the current state of the buffer.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_check_buffer_rtn(byte BoardID, byte buffer, out uint backlog, out ushort bufferState,
            out uint pointsDone);

        // Clears the previously configured buffer and clears any associations between resources and the specified buffer.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_clear_buffer(byte BoardID, byte buffer);

        // Configures a buffer for use in buffered operations.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_configure_buffer(byte BoardID, byte buffer, byte resource, ushort bufferType, 
            int bufferSize, uint totalPoints, ushort oldDataStop, double requestedInterval, out double actualInterval);

        // Reads data from a previously configured buffer.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_buffer(byte BoardID, byte buffer, uint numberOfPoints, byte returnVector);

        // Reads data from a previously configured buffer.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_buffer_rtn(byte BoardID, byte buffer, uint numberOfPoints, 
            [MarshalAs(UnmanagedType.LPArray)] out int data);

        // Writes data to a previously configured buffer.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_write_buffer(byte boardID, byte buffer, uint numberOfPoints, ushort regenerationMode,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] int[] data, byte inputVector);

        #endregion

        #region Gearing

        // Assigns a master axis, encoder, or ADC channel for master-slave gearing.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_config_gear_master(byte BoardID, byte axis, byte masterAxisOrEncoderOrADC);

        // Enables slave axes for master-slave gearing.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_enable_gearing(byte BoardID, ushort gearMap);

        // Enables a slave axis for master-slave gearing.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_enable_gearing_single_axis(byte BoardID, byte axis, ushort enable);

        // Loads the gear ratio for master-slave gearing.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_gear_ratio(byte BoardID, byte axis, ushort absoluteOrRelative,
            short ratioNumerator, ushort ratioDenominator, byte inputVector);

        #endregion

        #region Advanced Trajectory

        // Acquires time-sampled position and velocity data on multiple axes.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_acquire_trajectory_data(byte BoardID, ushort axisMap, ushort numberOfSamples, 
            ushort timePeriod);

        // Sets the base velocity used by the trajectory control loop for the axis specified.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_base_vel(byte BoardID, byte axis, ushort baseVelocity);

        // Loads the blend factor for an axis or vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_blend_fact(byte BoardID, byte axisOrVectorSpace, short blendFactor, byte inputVector);

        // Loads the following error trip point.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_follow_err(byte BoardID, byte axis, ushort followingError, byte inputVector);

        // Loads move constraints in user units.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_move_constraint(int boardID, int axisID, TnimcMoveConstraint attribute, 
            out NIMC_DATA value);

        // Loads the position modulus for an axis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_pos_modulus(byte BoardID, byte axis, uint positionModulus, byte inputVector);

        // Loads velocity for an axis or vector space in RPM.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_rpm(byte BoardID, byte axisOrVectorSpace, double RPM, byte inputVector);

        // Loads a velocity threshold for an axis in RPM.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_rpm_thresh(byte BoardID, byte axis, double threshold, byte inputVector);

        // Loads the maximum acceleration and/or deceleration value for an axis or vector space in RPS/s.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_rpsps(byte BoardID, byte axisOrVectorSpace, ushort accelerationType, 
            double RPSPS, byte inputVector);

        // Sets the Run/Stop threshold, which affects the run/stop status returned by Read Trajectory Status.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_run_stop_threshold(byte BoardID, byte axis, ushort runStopThreshold, byte inputVector);

        // Loads the s-curve time for an axis or vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_scurve_time(byte BoardID, byte axisOrVectorSpace, ushort sCurveTime, byte inputVector);

        // Loads primary and secondary DAC torque limits for an axis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_torque_lim(byte BoardID, byte axis, short primaryPositiveLimit, 
            short primaryNegativeLimit, short secondaryPositiveLimit, short secondaryNegativeLimit, byte inputVector);

        // Loads primary and secondary DAC torque offsets for an axis. 
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_torque_offset(byte BoardID, byte axis, short primaryOffset, short secondaryOffset, 
            byte inputVector);

        // Loads an instantaneous velocity override for an axis or vector space. 
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_velocity_override(byte BoardID, byte axisOrVectorSpace, float overridePercentage, 
            byte inputVector);

        // Loads a velocity threshold for an axis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_velocity_threshold(byte BoardID, byte axis, uint threshold, byte inputVector);

        // Reads the Blend Complete status for all axes or vector spaces.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_blend_status(byte BoardID, byte axisOrVectorSpace, byte returnVector);

        // Reads the Blend Complete status for all axes or vector spaces.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_blend_status_rtn(byte BoardID, byte axisOrVectorSpace, out ushort blendStatus);

        // Reads the commanded DAC output value for an axis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_dac(byte BoardID, byte axisOrDAC, byte returnVector);

        // Reads the status of the DAC limits.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_dac_output_limit_status(byte BoardID, byte returnVector);

        // Reads the status of the DAC limits.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_dac_output_limit_status_rtn(byte BoardID, out ushort positiveStatus, 
            out ushort negativeStatus);

        // Reads the commanded DAC output value for an axis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_dac_rtn(byte BoardID, byte axisOrDAC, out short DACValue);

        // Reads the instantaneous following error for an axis or vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_follow_err(byte BoardID, byte axisOrVectorSpace, byte returnVector);

        // Reads the instantaneous following error for an axis or vector space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_follow_err_rtn(byte BoardID, byte axisOrVectorSpace, out short followingError);

        // Reads the Move Complete Status register.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_mcs_rtn(byte BoardID, out ushort moveCompleteStatus);

        // Reads the number of steps generated by a stepper output.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_steps_gen(byte BoardID, byte axisOrStepperOutput, byte returnVector);

        // Reads the number of steps generated by a stepper output.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_steps_gen_rtn(byte BoardID, byte axisOrStepperOutput, out int steps);

        // Reads the destination position of the current motion trajectory.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_target_pos(byte BoardID, byte axis, byte returnVector);

        // Reads the destination position of the current motion trajectory.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_target_pos_rtn(byte BoardID, byte axis, out int targetPosition);

        // Reads a sample of acquired data from the samples buffer. 
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_trajectory_data(byte BoardID, byte returnVector);

        // Reads a sample of acquired data from the samples buffer.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_trajectory_data_rtn(byte BoardID, [MarshalAs(UnmanagedType.LPArray)] out int returnData);

        // Reads the selected motion trajectory status of all axes or vector spaces.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_trajectory_status(byte BoardID, byte axisOrVectorSpace, ushort statusType, 
            byte returnVector);

        // Reads the selected motion trajectory status of all axes or vector spaces.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_trajectory_status_rtn(byte BoardID, byte axisOrVectorSpace, ushort statusType, 
            out ushort status);

        #endregion

        #endregion

        #region Start and Stop Motion

        // Blends motion on a single axis, single vector space, multiple axes, or multiple vector spaces.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_blend(byte BoardID, byte axisOrVectorSpace, ushort axisOrVSMap);

        // Starts motion on a single axis, single vector space, multiple axes, or multiple vector spaces.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_start(byte BoardID, byte axisOrVectorSpace, ushort axisOrVSMap);

        // Stops motion on a single axis, single vector space, multiple axes, or multiple vector spaces. 
        // Three types of stops can be executed: decelerate to stop, halt stop, and kill stop.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_stop_motion(byte BoardID, byte axisOrVectorSpace, ushort stopType, ushort axisOrVSMap);

        #endregion

        #region Motion I/O

        // Configures polarity and enables the per-axis inhibit outputs.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_config_inhibit_output(byte BoardID, ushort axis, ushort enableInhibit, 
            ushort polarity, ushort driveMode);

        // Enables/disables the home inputs.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_enable_home_inputs(byte BoardID, ushort homeMap);

        // Enables/disables either the forward and reverse limit inputs or the forward and reverse software position limits.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_enable_axis_limit(byte BoardID, ushort limitType, ushort forwardLimitMap, 
            ushort reverseLimitMap);

        // Loads the forward and reverse software limit positions for an axis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_sw_lim_pos(byte BoardID, byte axis, int forwardLimit, int reverseLimit, 
            byte inputVector);

        // Reads the status of the drive signal when an active signal is detected.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_drive_signal_status(byte BoardID, byte axis, byte returnVector);

        // Reads the status of the drive signal when an active signal is detected.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_drive_signal_status_rtn(byte BoardID, byte axis, out ushort driveStatus);

        // Reads the instantaneous status of the home inputs. 
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_home_input_status(byte BoardID, byte returnVector);

        // Configures the drive signal.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_configure_drive_signal(byte BoardID, byte axis, ushort port, ushort pin, ushort mode,
            ushort polarity);

        // Reads the instantaneous status of the home inputs.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_home_input_status_rtn(byte BoardID, out ushort homeStatus);

        // Reads the instantaneous state of either the hardware limit inputs or the software limits.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_axis_limit_status(byte BoardID, ushort limitType, byte returnVector);

        // Reads the instantaneous state of either the hardware limit inputs or the software limits.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_axis_limit_status_rtn(byte BoardID, ushort limitType, 
            out ushort forwardLimitStatus, out ushort reverseLimitStatus);

        // Sets the polarity of the home inputs as either active low/active open or active high/active closed.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_set_home_polarity(byte BoardID, ushort homePolarityMap);

        // Sets the inhibit outputs using the MustOn/MustOff protocol.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_set_inhibit_output_momo(byte BoardID, ushort mustOn, ushort mustOff);

        // Sets the polarity of the forward and reverse limit inputs as either active low/active open or active high/active closed.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_set_limit_input_polarity(byte BoardID, ushort forwardPolarityMap, ushort reversePolarityMap);

        #region Breakpoint

        // Configure a position breakpoint on an axis or encoder.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_configure_breakpoint(byte BoardID, byte axisOrEncoder, ushort enableMode, 
            ushort actionOnBreakpoint, ushort operation);

        // Configures the drive mode and polarity of the breakpoint output.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_configure_breakpoint_output(byte BoardID, byte axisOrEncoder, ushort polarity, 
            ushort driveMode);

        // Enables and disables a position breakpoint on an axis or encoder.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_enable_breakpoint(byte BoardID, byte axisOrEncoder, byte enable);

        // Load the breakpoint modulus for a position breakpoint.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_bp_modulus(byte BoardID, byte axisOrEncoder, uint breakpointModulus, byte inputVector);

        // Loads the breakpoint position for an axis or encoder in counts.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_pos_bp(byte BoardID, byte axisOrEncoder, int breakpointPosition, byte inputVector);

        // Reads the breakpoint status for all axes or encoders.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_breakpoint_status(byte BoardID, byte axisOrEncoder, ushort breakpointType, 
            byte returnVector);

        // Reads the breakpoint status for all axes or encoders.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_breakpoint_status_rtn(byte BoardID, byte axisOrEncoder, ushort breakpointType, 
            out ushort breakpointStatus);

        // Sets the breakpoint outputs using the mustOn/mustOff protocol.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_set_breakpoint_output_momo(byte BoardID, byte axisOrEncoder, ushort mustOn, 
            ushort mustOff, byte inputVector);

        #endregion

        #region High Speed Capture

        // Configures the high-speed capture input for the specified signal behavior.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_configure_hs_capture(byte BoardID, byte axisOrEncoder, ushort captureMode, ushort operation);

        // Enables or disables the specified high-speed capture input.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_enable_hs_capture(byte BoardID, byte axisOrEncoder, ushort enable);

        // Reads a captured position value from an axis or encoder.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_cap_pos(byte BoardID, byte axisOrEncoder, byte returnVector);

        // Reads a captured position value from an axis or encoder.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_cap_pos_rtn(byte BoardID, byte axisOrEncoder, out int capturedPosition);

        // Reads the high-speed position capture status for all axes or encoders.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_hs_cap_status(byte BoardID, byte axisOrEncoder, byte returnVector);

        // Reads the high-speed position capture status for all axes or encoders.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_hs_cap_status_rtn(byte BoardID, byte axisOrEncoder, out ushort highSpeedCaptureStatus);

        #endregion

        #endregion

        #region Find Reference

        // Executes a search operation to find a reference position: home, index, center, forward limit, 
        // reverse limit, or run sequence.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_find_reference(byte BoardID, byte axisOrVectorSpace, ushort axisOrVSMap, byte findType);

        // Waits for a search sequence initiated by Find Reference to complete and returns the status. 
        // Wait Reference also can be used to query the status of a search. 
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_wait_reference(byte BoardID, byte axisOrVectorSpace, ushort axisOrVSMap, uint timeout,
            uint pollingInterval, out ushort found);

        // Checks the status of a search sequence initiated by Find Reference.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_check_reference(byte BoardID, byte axisOrVectorSpace, ushort axisOrVSMap, 
            out ushort found, out ushort finding);

        // Reads the currently selected reference status for the given set of axes or coordinate (vector) space.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_reference_status(byte BoardID, byte axisOrVectorSpace, ushort axisOrVSMap, 
            ushort attribute, byte returnVector);

        // Reads the currently selected reference status for the given set of axes or coordinate (vector) space. 
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_reference_status_rtn(byte BoardID, byte axisOrVectorSpace, ushort axisOrVSMap, 
            ushort attribute, out ushort value);

        // Loads the value for the specified find reference parameter.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_reference_parameter(byte BoardID, byte axis, byte findType, ushort attribute, 
            double value);

        // Gets the value for the specified find reference parameter.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_get_reference_parameter(byte BoardID, byte axis, byte findType, ushort attribute, 
            out double value);

        #endregion

        #region Analog and Digital I/O

        // Selects the maximum count frequency for an encoder channel by configuring its digital filter.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_configure_encoder_filter(byte BoardID, byte axisOrEncoder, ushort frequency);

        // Configures the encoder Phase A, Phase B, and Index line polarities.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_configure_encoder_polarity(byte BoardID, ushort indexPolarity, ushort phaseAPolarity,
            ushort phaseBPolarity);

        // Enables and disables PWM outputs, and sets the PWM clock frequency.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_configure_pwm_output(byte BoardID, byte PWMOutput, ushort enable, ushort clock);

        // Enables one or more of the unmapped ADC channels.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_enable_adcs(byte BoardID, byte reserved, ushort ADCMap);

        // Enables one or more of the unmapped encoder resources.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_enable_encoders(byte BoardID, ushort encoderMap);

        // Loads an output value to an unmapped DAC resource.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_dac(byte BoardID, byte DAC, short outputValue, byte inputVector);

        // Sets the duty cycle for a PWM output.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_pwm_duty(byte BoardID, byte PWMOutput, ushort dutycycle, byte inputVector);

        // Reads the converted value from an ADC input channel.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_adc16(byte BoardID, byte ADC, byte returnVector);

        // Reads the converted value from an ADC input channel.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_adc16_rtn(byte BoardID, byte ADC, out int ADCValue);

        // Reads the position of an encoder.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_encoder(byte BoardID, byte axisOrEncoder, byte returnVector);

        // Reads the position of an encoder.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_encoder_rtn(byte BoardID, byte axisOrEncoder, out int encoderCounts);

        // Reads the logical state of the bits in an I/O port.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_port(byte BoardID, byte port, byte returnVector);

        // Reads the logical state of the bits in an I/O port.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_port_rtn(byte BoardID, byte port, out ushort portData);

        // Resets the position of an unmapped encoder to the specified value. 
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_reset_encoder(byte BoardID, byte encoder, int position, byte inputVector);

        // Specifies the source and destination for various motion signals, including trigger inputs, 
        // high-speed capture circuits, breakpoint outputs, RTSI lines, and RTSI software ports.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_select_signal(byte BoardID, ushort destination, ushort source);

        // Sets the voltage range for the analog to digital converters, on a per-channel basis.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_set_adc_range(byte BoardID, byte ADC, ushort range);

        // Sets the direction of a general-purpose I/O port as input or output.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_set_port_direction(byte BoardID, byte port, ushort directionMap);

        // Sets an I/O port value using the MustOn/MustOff protocol.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_set_port(byte BoardID, byte port, byte mustOn, byte mustOff, byte inputVector);

        // Sets the bit polarity in a general-purpose I/O port.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_set_port_pol(byte BoardID, byte port, ushort portPolarityMap);

        #endregion

        #region Error and Utility

        // Gets an error, command, and/or resource description string as an ASCII character array.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_get_error_description(ushort descriptionType, int errorCode, ushort commandID,
            ushort resourceID, [MarshalAs(UnmanagedType.LPArray)] sbyte[] charArray, out uint sizeOfArray);

        // Gets detailed information about the last error generated by a high-level NI-Motion function in the 
        // course of executing other NI-Motion functions.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_get_last_error(byte BoardID, out ushort commandID, out ushort resourceID, out int errorCode);

        // Gets information about the properties and features of the motion controller.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_get_motion_board_info(byte BoardID, uint informationType, out uint informationValue);

        // Gets the motion controller name as an ASCII character array.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_get_motion_board_name(byte BoardID, [MarshalAs(UnmanagedType.LPArray)] out sbyte[] charArray, out uint sizeOfArray);

        // Gets the general software settings.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_getu32(byte BoardID, byte resource, ushort attribute, out uint value);

        // Reads the most recent modal error from the Error Message Stack.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_error_msg_rtn(byte BoardID, out ushort commandID, out ushort resourceID, 
            out int errorCode);

        // Sets the general software settings.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_setu32(byte BoardID, byte resource, ushort attribute, uint value);

        #endregion

        #region Onboard Programming

        // Begins a program storage session.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_begin_store(byte BoardID, byte program);

        // Ends a program storage session.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_end_store(byte BoardID, byte program);

        // Inserts a label in a program.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_insert_program_label(byte BoardID, ushort labelNumber);

        // Inserts a conditional jump in a program.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_jump_on_event(byte BoardID, byte resource, ushort condition, ushort mustOn,
            ushort mustOff, ushort matchType, ushort labelNumber);

        // Loads a delay into a program sequence.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_delay(byte BoardID, uint delayTime);

        // Specifies the minimum time an onboard program has to run per watchdog period. 
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_program_time_slice(byte BoardID, byte program, ushort timeSlice, byte inputVector);

        // Pauses a running program or resumes execution of a paused program.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_pause_prog(byte BoardID, byte program);

        // Reads the status of an onboard program.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_program_status(byte BoardID, byte program, byte returnVector);

        // Reads the status of an onboard program.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_program_status_rtn(byte BoardID, byte program, out ushort programStatus);

        // Runs a previously stored program.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_run_prog(byte BoardID, byte program);

        // Controls the user status bits in the Move Complete Status (MCS) register.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_set_status_momo(byte BoardID, byte mustOn, byte mustOff);

        // Stops a running program.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_stop_prog(byte BoardID, byte program);

        // Inserts a conditional wait in a program.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_wait_on_event(byte BoardID, byte resource, ushort waitType, ushort condition,
            ushort mustOn, ushort mustOff, ushort matchType, ushort timeOut, byte returnVector);

        #region Object Management

        // Loads a ASCII text description for a program or buffer object.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_description(byte BoardID, byte objectID, 
            [MarshalAs(UnmanagedType.LPArray)] out sbyte[] description);

        // Saves, deletes, or frees programs or buffers in RAM and ROM.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_object_mem_manage(byte BoardID, byte objectID, ushort operation);

        // Reads the ASCII text description for a program or buffer object.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_description_rtn(byte BoardID, byte objectID, 
            [MarshalAs(UnmanagedType.LPArray)] out sbyte[] description);

        // Reads a data record for a memory object from the Object Registry.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_registry_rtn(byte BoardID, byte index, out REGISTRY registryRecord);

        #endregion

        #region Data Operations

        // Adds the values in the two variables and returns the result.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_add_vars(byte BoardID, byte variable1, byte variable2, byte returnVector);

        // Performs a bitwise AND of the values in the two variables and returns the result.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_and_vars(byte BoardID, byte variable1, byte variable2, byte returnVector);

        // Divides the value in the first variable by the value in the second variable and returns the result.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_div_vars(byte BoardID, byte variable1, byte variable2, byte returnVector);

        // Loads a constant value into a variable.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_var(byte BoardID, int value, byte variable1);

        // Performs a logical shift on the value in a variable and returns the result.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_lshift_var(byte BoardID, byte variable1, sbyte logicalShift, byte returnVector);

        // Multiplies the values in the two variables and returns the result.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_mult_vars(byte BoardID, byte variable1, byte variable2, byte returnVector);

        // Performs a bitwise inversion (NOT) on the value in a variable and returns the result.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_not_var(byte BoardID, byte variable1, byte returnVector);

        // Performs a bitwise OR of the values in the two variables and returns the result.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_or_vars(byte BoardID, byte variable1, byte variable2, byte returnVector);

        // Reads the value of a variable and returns the result.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_var(byte BoardID, byte variable1, byte returnVector);

        // Reads the value of a variable and returns the result.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_var_rtn(byte BoardID, byte variable1, out int value);

        // Subtracts the value of second variable from the value of the first variable and returns the result.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_sub_vars(byte BoardID, byte variable1, byte variable2, byte returnVector);

        // Performs a bitwise Exclusive OR (XOR) of the values in the two variables and returns the result.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_xor_vars(byte BoardID, byte variable1, byte variable2, byte returnVector);
        
        #endregion

        #endregion

        #region Advanced

        // Clears the Power-Up status bit and boots up the controller, making it ready to accept functions.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_clear_pu_status(byte BoardID);

        // Allows you to automatically run a program when the controller powers up.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_enable_auto_start(byte BoardID, byte enableOrDisable, byte programToExecute);

        // Enables the emergency shutdown functionality of the controller.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_enable_shutdown(byte BoardID);

        // Clears the Return Data Buffer by deleting all of the buffered data.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_flush_rdb(byte BoardID);

        // Reads the temperature from the motion controller. 
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_board_temperature(byte BoardID, out double temperature);

        // Reads the Communication Status Register (CSR).
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_csr_rtn(byte BoardID, out ushort csr);

        // Resets the power-up defaults to the factory-default settings.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_reset_defaults(byte BoardID);

        // Reads the Return Data Buffer.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_read_rdb(byte BoardID, out ushort number, 
            [MarshalAs(UnmanagedType.LPArray)] out ushort[] wordCount, [MarshalAs(UnmanagedType.LPArray)] out byte[] resource,
            [MarshalAs(UnmanagedType.LPArray)] out ushort[] command, [MarshalAs(UnmanagedType.LPArray)] ushort[] commandData);

        // Saves the current operating parameters as defaults.
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_save_defaults(byte BoardID);

        #endregion

        #endregion

        #region Unused Functions

        // Load Axis Configuration Parameter  (ASP: note undocumented in ni-motion help)                                  
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_load_axis_configuration_parameter(Int32 boardID, Int32 axisID, TnimcAxisConfigurationParameter attribute, out NIMC_DATA data);

        //Configure Camming Master
        [DllImport("FlexMotion32.dll")]
		public static extern int flex_configure_camming_master(Int32 boardID, Int32 axisID, Int32 masterResource, Double masterCycle);

        //Load Camming Parameter (ASP: note not documented in ni-motion help)
        [DllImport("FlexMotion32.dll")]
		public static extern int flex_load_camming_parameter(Int32 boardID, Int32 axisID, TnimcCammingParameter attribute, out NIMC_DATA data);

        //Enable Camming (Multiaxes) (ASP: note not documented in ni-motion help)
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_enable_camming(Int32 boardID, UInt32 arraySize, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] out NIMC_CAMMING_ENABLE_DATA[] dataArray);

        //Enable Camming Single Axis
        [DllImport("FlexMotion32.dll")]
		public static extern int flex_enable_camming_single_axis(Int32 boardID, Int32 axisID, UInt16 enable, Double position);

        // Load Motion IO Parameter(ASP: note according to ni-motion help this is no longer supported
        [DllImport("FlexMotion32.dll")]
		public static extern int flex_load_motion_io_parameter(Int32 boardID, Int32 axisID, TnimcMotionIOParameter attribute, out NIMC_DATA data);

        // Read Motion IO Execution Data (ASP: note according to ni-motion help this is no longer supported
        [DllImport("FlexMotion32.dll")]
		public static extern int flex_read_motion_io_execution_data(Int32 boardID, Int32 axisID, TnimcMotionIOExecution attribute, out NIMC_DATA data);

        //Find Home
        [DllImport("FlexMotion32.dll")]
		public static extern int flex_find_home(Byte BoardID, Byte axis, UInt16 directionMap);

        //Find Index
        [DllImport("FlexMotion32.dll")]
		public static extern int flex_find_index(Byte BoardID, Byte axis, UInt16 direction, Int16 offset);

        // Get Motion Board Information (ASP: note not documented in ni-motion help
        [DllImport("FlexMotion32.dll")]
        public static extern int flex_get_motion_board_info_string(Byte BoardID, UInt32 informationType, [MarshalAs(UnmanagedType.LPArray)] out sbyte[] charArray, out Int32 sizeOfArray);

        //Read Error Message Detail (ASP: note not documented in ni-motion help)
        [DllImport("FlexMotion32.dll")]
		public static extern int flex_read_error_msg_detail_rtn(Byte BoardID, out UInt16 commandID, out UInt16 resourceID, out Int32 errorCode, out UInt16 lineNumber, out UInt16 fileNumber);

        // Get Double 
        [DllImport("FlexMotion32.dll")]
		public static extern int flex_getDouble (Byte BoardID, Byte resource, UInt16 attribute, out Double value);

        // Set Double 
        [DllImport("FlexMotion32.dll")]
		public static extern int flex_setDouble (Byte BoardID, Byte resource, UInt16 attribute, Double value);

        //Communicate (ASP: note that this method is not documented in ni-motion help)
        [DllImport("FlexMotion32.dll")]
		public static extern int flex_communicate(Byte BoardID, Byte mode, Byte wordCount, [MarshalAs(UnmanagedType.LPArray)] out Byte[] resource,
                       [MarshalAs(UnmanagedType.LPArray)] out UInt16[] command, [MarshalAs(UnmanagedType.LPArray)] out UInt16[] data, Byte vector);

        //Enable 1394 Watchdog
        [DllImport("FlexMotion32.dll")]
		public static extern int flex_enable_1394_watchdog(Byte BoardID, UInt16 enableOrDisable);

        //Set Interrupt Event Mask
        [DllImport("FlexMotion32.dll")]
		public static extern int flex_set_irq_mask(Byte BoardID, UInt16 mask);

        #endregion

        #region Error Checks

        /// <summary>
        /// Checks whether a FlexMotion error occurred that should trigger the application to close.
        /// </summary>
        /// <param name="boardID">The board to check (board numbers are defined in MAX)</param>
        /// <param name="error">???</param>
        /// <returns>The error string, or null if there was no error</returns>
        public static string CheckForError(byte boardID, int error)
        {
            int errorCode;
            ushort communicationStatusRegister;
            ushort commandID;
            ushort resourceID;

            if (error == 0) {
                return null;
            }

            // Read the communication status register
            communicationStatusRegister = 0;
            flex_read_csr_rtn(boardID, out communicationStatusRegister);

            if ((communicationStatusRegister & NIMC_MODAL_ERROR_MSG) != 0) {

                // There is an error waiting on the board's error stack.

                commandID = 0;
                resourceID = 0;
                errorCode = 0;

                // Retrieve the error from the stack.
                flex_read_error_msg_rtn(boardID, out commandID, out resourceID, out errorCode);

                return GetErrorString(errorCode, commandID, resourceID);
            }      
            return GetErrorString(error, 0, 0);
        }

        /// <summary>
        /// Converts the board's error code into a human-readable description.
        /// </summary>
        /// <param name="errorCode">Error code given by the board</param>
        /// <param name="commandID">Command ID given by the board</param>
        /// <param name="resourceID">Resource ID given by the board</param>
        /// <returns>An error string that can be displayed to the user</returns>
        private static string GetErrorString(int errorCode, ushort commandID, ushort resourceID)
        {
            sbyte[] errorDescription;
            ushort descriptionType;
            uint sizeOfArray;
            byte[] errorDescriptionBytes;       // Used as an intermediate step in decoding the errorDescription

            errorDescription = new sbyte[0];
            descriptionType = 0;
            sizeOfArray = 0;

            if (commandID == 0) {
                descriptionType = NIMC_ERROR_ONLY;
            } else {
                descriptionType = NIMC_COMBINED_DESCRIPTION;
            }

            // Get the size of the description
            flex_get_error_description(descriptionType, errorCode, commandID, resourceID, errorDescription, out sizeOfArray);

            // Size of the array for holding the description should be the length of the description plus one for a NULL character.
            sizeOfArray++;

            // Allocate memory on the heap for the description
            errorDescription = new sbyte[sizeOfArray];

            // Get the error description
            flex_get_error_description(descriptionType, errorCode, commandID, resourceID, errorDescription, out sizeOfArray);

            if (errorDescription.Length > 0) {

                errorDescriptionBytes = new byte[errorDescription.Length];
                Buffer.BlockCopy(errorDescription, 0, errorDescriptionBytes, 0, errorDescription.Length);
                return Encoding.UTF7.GetString(errorDescriptionBytes);
            }
            return null;
        }

        #endregion

        #region NIMotion.dll Functions

        // Write Functions
        //[DllImport("nimotion.dll")]
        //public static extern int nimcWriteMotionIOData(int deviceHandle, int axisHandle, TnimcMotionIOData attribute, TnimcData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcWriteDigitalIOData(int deviceHandle, int axisHandle, uint line, TnimcDigitalIOData attribute, TnimcData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcWriteTrajectoryData(int deviceHandle, int axisHandle, TnimcTrajectoryData attribute, TnimcData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcWriteCaptureCompareData(int deviceHandle, int axisHandle, int index, TnimcCaptureCompareData attribute, TnimcData* data);

        // Read Functions
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadMotionIOData(int deviceHandle, int axisHandle, TnimcMotionIOData attribute, TnimcData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadAxisData(int deviceHandle, int axisHandle, TnimcAxisData attribute, TnimcData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadAxisStatus(int deviceHandle, int axisHandle, TnimcAxisStatus attribute, TnimcData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadCoordinateData(int deviceHandle, int coordinateHandle, TnimcCoordinateData attribute, TnimcData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadCoordinateStatus(int deviceHandle, int coordinateHandle, TnimcCoordinateStatus attribute, TnimcData* data);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcReadCoordinatePosition(int deviceHandle, int coordinateHandle, out double position, uint lengthPosition, out uint fetched);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadEncoderData(int deviceHandle, int axisHandle, int index, TnimcEncoderData attribute, TnimcData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadCaptureCompareData(int deviceHandle, int axisHandle, int index, TnimcCaptureCompareData attribute, TnimcData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadAllAxisData(int deviceHandle, int axisHandle, TnimcAllAxisData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadAllAxisStatus(int deviceHandle, int axisHandle, TnimcAllAxisStatus* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadDigitalIOData(int deviceHandle, int axisHandle, uint line, TnimcDigitalIOData attribute, TnimcData* data);

        // Methods
        [DllImport("nimotion.dll")]
        public static extern int nimcResetController(int deviceHandle);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcSelfTest(int deviceHandle);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcClearFaults(int deviceHandle);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcConfigureMotionIOMap(int deviceHandle, int axisHandle, TnimcMotionIOMap attribute, int ioAxis, TnimcMotionIOMapLineType lineType, uint line);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcAxisStraightLineMove(int deviceHandle, int axisHandle, TnimcAxisStraightLineMoveData* data, TnimcMoveConstraints* moveConstraints);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcDumdum();

        // Create, Destroy, Open & Close Interface Functions
        //[DllImport("nimotion.dll")]
        //public static extern int nimcCreateControlInterface(out int handle, TnimcNode node);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcDestroyControlInterface(int handle);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcOpenResourceInterface(out int handle, sbyte[] resource);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcCloseResourceInterface(int handle);

        // Interface Read, Write, Set & Get Functions
        [DllImport("nimotion.dll")]
        public static extern int nimcSetResource(int handle, sbyte[] resource);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcSetProperty(int handle, TnimcSetProperty property, TnimcData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcSetPropertyF64Array(int handle, TnimcSetProperty property, out double dataArray, uint sizeOfDataArray);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcGetProperty(int handle, TnimcGetProperty property, TnimcData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcGetPropertyF64Array(int handle, TnimcGetProperty property, out double dataArray, uint sizeOfDataArray, out uint fetched);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcWriteData(int handle, TnimcWriteData property, TnimcData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcWriteDataF64Array(int handle, TnimcWriteData property, out double dataArray, uint sizeOfDataArray);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadData(int handle, TnimcReadData property, TnimcData* data);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadDataF64Array(int handle, TnimcReadData property, out double dataArray, uint sizeOfDataArray, out uint fetched);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadDataI32Array(int handle, TnimcReadData property, out int dataArray, uint sizeOfDataArray, out uint fetched);
        
        //[DllImport("nimotion.dll")]
        //public static extern int nimcReadStatus(int handle, TnimcReadStatus property, TnimcData* data);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcGetLastError(int handle, out int errorCode, sbyte[] description, uint sizeOfDescriptionString, sbyte[] location, uint sizeOfLocationString);

        // Interface Methods
        [DllImport("nimotion.dll")]
        public static extern int nimcExecute(int handle);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcStop(int handle);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcCommit(int handle);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcRefresh(int handle);

        // Specific Interface Methods
        [DllImport("nimotion.dll")]
        public static extern int nimcControllerInitialize(int handle);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcAxisClearFaults(int handle);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcAxisPower(int handle, bool powerEnableAxis, bool powerEnableDrive);
        
        [DllImport("nimotion.dll")]
        public static extern int nimcAxisResetPosition(int handle, double position);

        // Non-Interface Methods
        [DllImport("nimotion.dll")]
        public static extern int nimcGetErrorDescription(int errorCode, sbyte[] description, uint sizeOfDescriptionString);

        #endregion
    }
}





