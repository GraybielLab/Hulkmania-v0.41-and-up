using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Brandeis.AGSOL.Network;
using System.Net;
using System.Diagnostics;
using System.Net.Sockets;
using System.Configuration;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// This class is the entry point for the application.
    /// </summary>
    public class AppMain
    {
        // System parameters info. Used in disabling the screensaver.
        public enum SPI : uint {
            SPI_GETSCREENSAVEACTIVE = 0x0010,
            SPI_SETSCREENSAVEACTIVE = 0x0011
        }

        // System parameters info file. Used in disabling the screensaver.
        public enum SPIF : uint {
            None = 0x00,
            SPIF_UPDATEINIFILE = 0x01,
            SPIF_SENDCHANGE = 0x02,
            SPIF_SENDWININICHANGE = 0x02
        }

        // System function used to disable the screensaver.
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, uint pvParam, SPIF fWinIni);

        // System function used to set an existing application window as the foreground window
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        // System function used to set an existing application window as the foreground window
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        private const int SW_SHOWNORMAL = 1;
        private const int SW_MAXIMIZE = 3;
        
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);
             
        private const string LOGGING_CONFIG_FILE = "logging.config";

        private static bool mUseDummyMotionController = false;

        #endregion

        #region Fields

        private static HulkTask currentTask;

        private static Thread controllerThread;

        private static List<Type> availableTasks;

        private static string baseDirectory;

        #endregion

        #region Properties

        /// <summary>
        /// The list of available tasks that can be selected to run.
        /// </summary>
        public static List<Type> AvailableTasks
        {
            get {
                return availableTasks;
            }
        }

        /// <summary>
        /// The directory that the application begins in.
        /// </summary>
        public static string BaseDirectory
        {
            get {
                return baseDirectory;
            }
        }

        /// <summary>
        /// The user-specified task.
        /// </summary>
        public static HulkTask CurrentTask
        {
            get {
                return currentTask;
            }
            set {
                currentTask = value;
            }
        }

        public static bool UseDummyMotionController
        {
            get
            {
                return mUseDummyMotionController;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if there already is an instance of Hulkamania running and if so, makes that the foreground window
        /// </summary>
        /// <returns>true if there already is an instance of Hulkamania running</returns>
        private static bool IsAlreadyRunning()
        {
            bool retval = false;

            Process current = Process.GetCurrentProcess();

            foreach (Process process in Process.GetProcessesByName(current.ProcessName))
            {
                if ((process.Id != current.Id) && (process.MainModule.FileName == current.MainModule.FileName))
                {
                    ShowWindow(process.MainWindowHandle, SW_SHOWNORMAL| SW_MAXIMIZE);
                    SetForegroundWindow(process.MainWindowHandle);
                    retval = true;
                    break;
                }
            }
            
            return retval;
        }

        /// <summary>
        /// Handler for any exceptions not handled elsewhere in the application.
        /// </summary>
        /// <param name="obj">The exception</param>
        private static void HandleUnhandledException(object obj)
        {
            Exception e;

            e = obj as Exception;

            if (e != null) {
                logger.Error("[AppMain] Unhandled exception", e);
            } else {
                logger.Error("[AppMain] Unhandled exception: " + obj.GetType() + " - " + obj);
            }
        }

        /// <summary>
        /// Searches for available HulkTasks in the current directory.
        /// </summary>
        private static void LoadAvailableTasks()
        {
            Assembly assembly;
            DirectoryInfo directoryInfo;
            FileInfo[] files;
            Type[] types;

            availableTasks = new List<Type>();

            directoryInfo = new DirectoryInfo(baseDirectory);
            files = directoryInfo.GetFiles("*.dll");

            foreach (FileInfo file in files) {
                if (file.FullName.Contains(".Tasks.")) {
                    
                    // Found a DLL that may contain a HulkTask

                    assembly = Assembly.LoadFrom(file.FullName);
                    types = assembly.GetTypes();
                    foreach (Type type in types) {
                        if ((type.BaseType!=null) && (type.BaseType.Name.Equals("HulkTask")))
                        {

                            // Found a HulkTask. Add it to the list so that the user can select it.
                            availableTasks.Add(type);
                        }
                    }
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Catches unhandled CLR exceptions.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Information on the exception</param>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException(e.ExceptionObject);
        }

        /// <summary>
        /// Catches unhandled GUI exceptions.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Information on the exception</param>
        private static void OnUnhandledGuiException(object sender, ThreadExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception);
        }

        #endregion

        #region Network

        #region Fields
        private static ServerHandler serverHandler = new ServerHandler();
        private static StatusTransmitterMars statusTransmitter = new StatusTransmitterMars();
        #endregion

        #region Properties

        /// <summary>
        /// the network status transmitter
        /// </summary>
        public static StatusTransmitterMars StatusTransmitter { get { return statusTransmitter; } }

        /// <summary>
        /// returns true if the network server is started.
        /// </summary>
        public static bool isServerStarted { get { return (serverHandler == null) ? false : serverHandler.isServerStarted; } }

        /// <summary>
        /// the network server handler
        /// </summary>
        public static ServerHandler ServerHandler { get { return serverHandler; } }
        #endregion

        /// <summary>
        /// Starts the server that communicates with graphics clients
        /// </summary>
        public static bool startServer()
        {
            int portNum = -1;
            IPAddress address;

            if (!int.TryParse(ConfigurationManager.AppSettings["VRServerPort"], out portNum))
            {
                portNum = 11023;
                logger.Warn("Network: Could not read server port settings from appconfig file, using default port 11023");
            }
            if (!IPAddress.TryParse(ConfigurationManager.AppSettings["VRServerAddress"], out address))
            {
                address = IPAddress.Parse("127.0.0.1");
                logger.Warn("Network: Could not read server address settings from appconfig file, using default address 127.0.0.1");
            }

            IPEndPoint endPoint = new IPEndPoint(address, portNum);

            bool retval = false;
            if (serverHandler.startServer(endPoint, new ServerReceivedCommandDelegate(_serverReceivedCommand), new ServerReceivedConnectionDelegate(_serverReceivedConnection), new ServerReceivedDisconnectionDelegate(_serverReceivedDisconnection)))
            {
                logger.Info("Network: Successfully started server on: " + address + ":" + portNum.ToString());
                retval = true;
            }
            else
            {
                logger.Error("Network: Could not start server on: " + address + ":" + portNum.ToString());
            }

            return retval;
        }

        /// <summary>
        /// Stops the server that communicates with graphics clients
        /// </summary>
        public static void stopServer()
        {
            if (serverHandler != null)
            {
                serverHandler.stopServer();
                logger.Info("Network: Stopped server");
            }
        }

        // -------------------------------------------------------------------------------------------------------------------------
        // callback that gets notified whenever a network command has been received
        private static void _serverReceivedCommand(ICommand c, INetworkServer server, Socket handler)
        {
            eCommands command = (eCommands)c.CommandType;
            switch(command){
                case eCommands.Message:
                    {
                        string message = c.getParameterAsString((int)eMessageCommandParameters.Message);
                        logger.Info("Network: received message from " + IPAddress.Parse(((IPEndPoint)handler.RemoteEndPoint).Address.ToString()) + ": " + message);
                    }

                    break;
                case eCommands.RegisterClient:
                    {
                        int nc = 0;
                        string versionInfo = "UNDEFINED";
                        try {
                            versionInfo = c.getParameterAsString((int)eRegisterClientCommandParameters.VersionInfo);
                            nc = c.getParameterAsInt((int)eRegisterClientCommandParameters.NumScenes);
                        } catch (Exception) { 
                        }

                        logger.Info("Network: Client version: " + versionInfo);
                        logger.Info("Network: Client number of scenes: " + nc);
          
                        MainForm.GetMainForm().NumberOfScenesInNetworkClients = nc;
                    } 
                    break;
                default:
                    if (currentTask != null){
                        currentTask.ProcessNetworkCommand(c, server, handler);
                    }
                    break;
            }
        }

        // -------------------------------------------------------------------------------------------------------------------------
        // callback function that gets notified whenever a new connection has been received
        private static void _serverReceivedConnection(INetworkServer server, Socket client)
        {
            ServerHandler.SocketDescriptor s = serverHandler.getSocketDescriptor(client);
            logger.Info("Network: Received connection from:" + s.state.remoteEndpoint);
            listActiveConnections();
        }

        // -------------------------------------------------------------------------------------------------------------------------
        // callback function that gets notified whenever a disconnection has been received
        private static void _serverReceivedDisconnection(INetworkServer server, Socket client)
        {
            ServerHandler.SocketDescriptor s = serverHandler.getSocketDescriptor(client);
            logger.Info("Network: Received disconnection from client ");
            listActiveConnections();
        }

        // -------------------------------------------------------------------------------------------------------------------------
        public static void listActiveConnections()
        {
            int numConnections = serverHandler.getConnectedClientCount();

            if (numConnections == 0)
            {
                logger.Info("Network: No clients connected!");
            }
            else
            {
                for (int i = 0; i != numConnections; i++)
                {
                    ServerHandler.SocketDescriptor desc = serverHandler.getSocketDescriptor(i);
                    logger.Info("Network: Connection " + (i + 1).ToString() + " client: " + desc.state.remoteEndpoint + ", type: " + desc.clientType.ToString());
                }
            }
        }

        #endregion


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // if the app is already running, make the already running app window the foreground window and exit this instance
            bool singleInstanceOnly = false;
            bool.TryParse(ConfigurationManager.AppSettings["AllowSingleHulkamaniaInstanceOnly"], out singleInstanceOnly);

            if (singleInstanceOnly && IsAlreadyRunning())
            {
                System.Environment.Exit(0);
                return;
            }

            MainForm mainForm;

            try {
                // Set up logging
                XmlConfigurator.ConfigureAndWatch(new FileInfo(LOGGING_CONFIG_FILE));
                logger.Info("********** Starting Hulkamania **********");

                // parse command line
                mUseDummyMotionController = (Array.IndexOf(args, "UseDummyMotionControl") >= 0);
                logger.Info("Checking 'UseDummyMotionControl' command line argument: using " + (mUseDummyMotionController ? "Dummy" : "HULK") + " motion controller");
  
                // Disable screensaver
                SystemParametersInfo(SPI.SPI_SETSCREENSAVEACTIVE, 0, 0, SPIF.None);

                // Check if required data output folder exists 
                DataLogger.CheckIfRequiredFoldersExist(true);

                // Start the thread that will control the Hulk
                MotionController.Instance.CheckModalErrors();
                controllerThread = new Thread(Hulk.ControlHulk) {
                    Priority = ThreadPriority.Highest
                };
                controllerThread.Start();

                // Set up handlers to deal with any exceptions not caught elsewhere in the application
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                Application.ThreadException += OnUnhandledGuiException;

                // Find available tasks
                baseDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                LoadAvailableTasks();

                // Start the network server
                startServer();
                statusTransmitter.initialize(serverHandler);

                // Start the main form, which will run until it is closed by the user
                Application.EnableVisualStyles();
                mainForm = MainForm.GetMainForm();
                
                Application.Run(mainForm);

                statusTransmitter.shutdown();
                stopServer();

            } catch (Exception e) {
                HandleUnhandledException(e);
            }
                        
            try {
                // Turn the screensaver back on
                SystemParametersInfo(SPI.SPI_SETSCREENSAVEACTIVE, 1, 0, SPIF.None);
                
                // Close the main control loop thread.
                Hulk.KeepRunning = false;

                // Stop the network server
                stopServer();

            } catch (Exception e) {
                HandleUnhandledException(e);
            }

            logger.Info("********** Quitting Hulkamania **********");

            Application.Exit();
        }
    }
}
