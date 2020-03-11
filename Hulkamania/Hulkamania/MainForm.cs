using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using Brandeis.AGSOL.Network;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// The main GUI form.
    /// </summary>
    public sealed class MainForm : Form
    {
       
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);
        
        #endregion

        #region Fields

        // Static

        private static MainForm mainForm;

        // Instance

        private List<ToolStripMenuItem> taskMenuItems;

        private MenuStrip menuStrip;

        private NamedPanel audioPanel;
        private NamedPanel controlPanel;
        private NamedPanel graphPanel;
        private NamedPanel loggingPanel;
        private NamedPanel statusPanel;
        private NamedPanel taskPanel;

        private StatusStrip statusBar;

        private ToolStripMenuItem exitMenuItem;
        private ToolStripMenuItem fileMenu;
        private ToolStripMenuItem tasksMenu;

        // network / VR environment related
        private ToolStripMenuItem networkMenu;
        private ToolStripMenuItem listNetworkConnectionsMenuItem;
        private ToolStripMenuItem sendProtocolMenuItem;
        private ToolStripMenuItem startServerMenuItem;
        private ToolStripMenuItem stopServerMenuItem;
        private ToolStripMenuItem scenesMenuItem;
        private ToolStripMenuItem startAlignmentSceneMenuItem;
        private ToolStripMenuItem stopAlignmentSceneMenuItem;

        private ToolStripMenuItem resetOculusViewDirectionMenuItem;
        private ToolStripMenuItem resetTrackerOriginMenuItem;
        private ToolStripMenuItem sendReconnectCommandToNetworkClientsMenuItem;

        private ToolStripSeparator statusTaskSeparator;
        private ToolStripSeparator taskJoystickSeparator;
        private ToolStripSeparator joystickNetworkSeparator;
        private ToolStripSeparator lightsPlcSeparator;
        private ToolStripSeparator networkPcSeparator;
        private ToolStripSeparator plcUpdateSeparator;
        private ToolStripSeparator pcLightsSeparator;

        private ToolStripStatusLabel statusLabel;
        private ToolStripStatusLabel joystickLabel;
        private ToolStripStatusLabel lightsLabel;
        private ToolStripStatusLabel networkLabel;
        private ToolStripStatusLabel pcLabel;
        private ToolStripStatusLabel plcLabel;
        private ToolStripStatusLabel taskLabel;
        private ToolStripStatusLabel updateLabel;

        private bool layoutComplete;

        private int numberOfScenesInNetworkClients = 0;

        #endregion

        #region Properties
        /// <summary>
        /// The number of scenes that are available in any connected network client. It is assumed that all connected network clients run
        /// the same VR environment.
        /// </summary>
        public int NumberOfScenesInNetworkClients { get { return numberOfScenesInNetworkClients; } set { numberOfScenesInNetworkClients = value; _updateScenesMenuItem(); } }

        /// <summary>
        /// The status bar label that contains the status of the task.
        /// </summary>
        public ToolStripStatusLabel TaskStatusLabel
        {
            get {
                return taskLabel;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the form. Called through the singleton method.
        /// </summary>
        private MainForm()
        {
             logger.Debug("Create: MainForm");

            layoutComplete = false;

            InitializeGUI();
        }

        #endregion

        #region Delegates

        private delegate void UpdateScenesMenuItemDelegate();

        #endregion

        #region GUI Creation
     
        /// <summary>
        /// Updates the 'Network/Scenes...' menu item with the appropriate scene activation sub menu items for the currently connected network clients
        /// </summary>
        private void _updateScenesMenuItem()
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateScenesMenuItemDelegate(_updateScenesMenuItem));
            }
            else
            {

                if (scenesMenuItem != null)
                {
                    scenesMenuItem.DropDownItems.Clear();

                    scenesMenuItem.DropDownItems.Add(startAlignmentSceneMenuItem);
                    scenesMenuItem.DropDownItems.Add(stopAlignmentSceneMenuItem);
                    scenesMenuItem.DropDownItems.Add(new ToolStripSeparator());

                    for (int i = 1; i < numberOfScenesInNetworkClients; i++)
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem("Activate scene " + i.ToString());
                        item.Tag = i;
                        scenesMenuItem.DropDownItems.Add(item);
                        item.Click += new EventHandler(activateSceneMenuItem_Click);
                    }
                }
            }
        }
      
        /// <summary>
        /// Constructs the form GUI.
        /// </summary>
        private void InitializeGUI()
        {
            ControlPanel childControlPanel;
            AudioPanel childAudioPanel;
            GraphPanel childGraphPanel;
            LoggingPanel childLogPanel;
            StatusPanel childStatusPanel;

            logger.Debug("Enter: InitializeGUI()");

            SuspendLayout();

            WindowState = FormWindowState.Maximized;
            CheckForIllegalCrossThreadCalls = true;
            DoubleBuffered = true;

            Icon = new Icon(AppMain.BaseDirectory + "\\hulk.ico");
            Text = "HULKamania [v " + ConfigurationManager.AppSettings["Version"] + "]";

            BackColor = ColorScheme.Instance.FormBackground;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            
            #region Menu Strip

            exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += exitMenuItem_Click;

            fileMenu = new ToolStripMenuItem("File");
            fileMenu.Tag = "PARENT";
            fileMenu.DropDownItems.Add(exitMenuItem);

            taskMenuItems = new List<ToolStripMenuItem>();
            foreach (Type task in AppMain.AvailableTasks) {
                taskMenuItems.Add(new ToolStripMenuItem(task.Name));
                taskMenuItems[taskMenuItems.Count - 1].Tag = task;
                taskMenuItems[taskMenuItems.Count - 1].Click += taskMenuItem_Click;
            }

            tasksMenu = new ToolStripMenuItem("Tasks");
            tasksMenu.Tag = "PARENT";
            foreach (ToolStripMenuItem item in taskMenuItems)
            {
                tasksMenu.DropDownItems.Add(item);
            }

            startServerMenuItem = new ToolStripMenuItem("Start network server");
            startServerMenuItem.Click += new EventHandler(startServerMenuItem_Click);
            startServerMenuItem.Enabled = !AppMain.ServerHandler.isServerStarted;

            stopServerMenuItem = new ToolStripMenuItem("Stop network server");
            stopServerMenuItem.Click += new EventHandler(stopServerMenuItem_Click);
            stopServerMenuItem.Enabled = AppMain.ServerHandler.isServerStarted;

            sendProtocolMenuItem = new ToolStripMenuItem("Send protocol");
            sendProtocolMenuItem.Click += new EventHandler(sendProtocolMenuItemMenuItem_Click);

            listNetworkConnectionsMenuItem = new ToolStripMenuItem("List network connections");
            listNetworkConnectionsMenuItem.Click += new EventHandler(listNetworkConnectionsMenuItem_Click);

            resetOculusViewDirectionMenuItem = new ToolStripMenuItem("(Oculus Rift) Reset view direction");
            resetOculusViewDirectionMenuItem.Click += resetOculusViewDirectionMenuItem_Click;

            resetTrackerOriginMenuItem = new ToolStripMenuItem("(VRPN Tracker) Reset tracker origin");
            resetTrackerOriginMenuItem.Click += resetTrackerOriginMenuItem_Click;

            sendReconnectCommandToNetworkClientsMenuItem = new ToolStripMenuItem("Reconnect client(s) to server ...");
            sendReconnectCommandToNetworkClientsMenuItem.Click += new EventHandler(sendReconnectCommandToNetworkClientsMenuItem_Click);

            scenesMenuItem = new ToolStripMenuItem("Scenes");

            startAlignmentSceneMenuItem = new ToolStripMenuItem("Start alignment scene");
            startAlignmentSceneMenuItem.Click += new EventHandler(startAlignmentSceneMenuItem_Click);

            stopAlignmentSceneMenuItem = new ToolStripMenuItem("Stop alignment scene");
            stopAlignmentSceneMenuItem.Click += new EventHandler(stopAlignmentSceneMenuItem_Click);

             _updateScenesMenuItem();

            networkMenu = new ToolStripMenuItem("Network");
            networkMenu.Tag = "PARENT";
            networkMenu.DropDownItems.Add(startServerMenuItem);
            networkMenu.DropDownItems.Add(stopServerMenuItem);
            networkMenu.DropDownItems.Add(listNetworkConnectionsMenuItem);
            networkMenu.DropDownItems.Add(new ToolStripSeparator());
            networkMenu.DropDownItems.Add(sendProtocolMenuItem);
            networkMenu.DropDownItems.Add(new ToolStripSeparator());
            networkMenu.DropDownItems.Add(resetTrackerOriginMenuItem);
            networkMenu.DropDownItems.Add(resetOculusViewDirectionMenuItem);
            networkMenu.DropDownItems.Add(sendReconnectCommandToNetworkClientsMenuItem);
            networkMenu.DropDownItems.Add(new ToolStripSeparator());
            networkMenu.DropDownItems.Add(scenesMenuItem);

            menuStrip = new MenuStrip {
                Renderer = new MenuRenderer()
            };
            
            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(tasksMenu);
            menuStrip.Items.Add(networkMenu);

            #endregion

            #region Status Bar

            statusLabel = new ToolStripStatusLabel {
                TextAlign = ContentAlignment.MiddleLeft,
                Spring = true
            };

            statusTaskSeparator = new ToolStripSeparator();

            taskLabel = new ToolStripStatusLabel {
                TextAlign = ContentAlignment.MiddleLeft,
                Spring = true
            };

            taskJoystickSeparator = new ToolStripSeparator();

            joystickLabel = new ToolStripStatusLabel {
                TextAlign = ContentAlignment.MiddleRight,
                Spring = false,
                Width = 150
            };

            joystickNetworkSeparator = new ToolStripSeparator();

            networkLabel = new ToolStripStatusLabel {
                TextAlign = ContentAlignment.MiddleRight,
                Spring = false,
                Width = 150
            };

            networkPcSeparator = new ToolStripSeparator();

            pcLabel = new ToolStripStatusLabel {
                TextAlign = ContentAlignment.MiddleRight,
                Spring = false,
                Width = 150
            };

            pcLightsSeparator = new ToolStripSeparator();

            lightsLabel = new ToolStripStatusLabel {
                TextAlign = ContentAlignment.MiddleRight,
                Spring = false,
                Width = 150
            };

            lightsPlcSeparator = new ToolStripSeparator();

            plcLabel = new ToolStripStatusLabel {
                TextAlign = ContentAlignment.MiddleRight,
                Spring = false,
                Width = 150
            };

            plcUpdateSeparator = new ToolStripSeparator();

            updateLabel = new ToolStripStatusLabel {
                TextAlign = ContentAlignment.MiddleLeft,
                Spring = false,
                Width = 110
            };

            statusBar = new StatusStrip {
                BackColor = ColorScheme.Instance.StatusBarBackground,
                ForeColor = ColorScheme.Instance.StatusBarForeground
            };

            statusBar.Items.Add(statusLabel);
            statusBar.Items.Add(statusTaskSeparator);
            statusBar.Items.Add(taskLabel);
            statusBar.Items.Add(taskJoystickSeparator);
            statusBar.Items.Add(joystickLabel);
            statusBar.Items.Add(joystickNetworkSeparator);
            statusBar.Items.Add(networkLabel);
            statusBar.Items.Add(networkPcSeparator);
            statusBar.Items.Add(pcLabel);
            statusBar.Items.Add(pcLightsSeparator);
            statusBar.Items.Add(lightsLabel);
            statusBar.Items.Add(lightsPlcSeparator);
            statusBar.Items.Add(plcLabel);
            statusBar.Items.Add(plcUpdateSeparator);
            statusBar.Items.Add(updateLabel);

            #endregion

            #region Panels

            childControlPanel = new ControlPanel();
            controlPanel = new NamedPanel(childControlPanel, "Control", ColorScheme.Instance.ControlPanelMarker)
            {
                Margin = new Padding(3, 3, 3, 0)
            };

            childAudioPanel = new AudioPanel();
            audioPanel = new NamedPanel(childAudioPanel, "Audio", ColorScheme.Instance.AudioPanelMarker)
            {
                Margin = new Padding(3, 3, 3, 0)
            };

            childGraphPanel = new GraphPanel();
            graphPanel = new NamedPanel(childGraphPanel, "Graphs", ColorScheme.Instance.GraphsPanelMarker) {
                Margin = new Padding(3, 0, 0, 3)
            };

            childLogPanel = new LoggingPanel();
            loggingPanel = new NamedPanel(childLogPanel, "Event Log", ColorScheme.Instance.EventLogPanelMarker) {
                Margin = new Padding(3, 3, 0, 0)
            };

            childStatusPanel = new StatusPanel();
            statusPanel = new NamedPanel(childStatusPanel, "Status", ColorScheme.Instance.StatusPanelMarker) {
                Margin = new Padding(0, 3, 3, 0)
            };

            taskPanel = new NamedPanel(new HulkTaskPanel(), "Task", ColorScheme.Instance.TaskPanelMarker) {
                Margin = new Padding(0, 0, 3, 3)
            };

            #endregion

            Controls.Add(audioPanel);
            Controls.Add(controlPanel);
            Controls.Add(graphPanel);
            Controls.Add(loggingPanel);
            Controls.Add(menuStrip);
            Controls.Add(statusBar);
            Controls.Add(statusPanel);
            Controls.Add(taskPanel);

            Closing += MainForm_Closing;
            Layout += MainForm_Layout;
            Load += MainForm_Load;

            ResumeLayout();

            layoutComplete = true;
        }


        #endregion

        #region Internal Methods

        /// <summary>
        /// Exits the application.
        /// </summary>
        /// <param name="errorMessage">Optional error message to show the user</param>
        internal static void ExitApplication(string errorMessage)
        {
            logger.Debug("Enter: ExitApplication(string)");

            if (errorMessage != null) {
                MessageBox.Show(errorMessage, "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Application.Exit();
        }

        /// <summary>
        /// Returns the singleton main form.
        /// </summary>
        /// <returns>The singleton main form</returns>
        public static MainForm GetMainForm()
        {
            return mainForm ?? (mainForm = new MainForm());
        }

        #endregion

        #region Event Handlers

        #region Menu Bar
        /// <summary>
        /// Called when the user selects the Network/Send reconnect to server menu item
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        void sendReconnectCommandToNetworkClientsMenuItem_Click(object sender, EventArgs e)
        {
            SendReconnectCommandToNetworkClients dlg = new SendReconnectCommandToNetworkClients();
            dlg.ShowDialog();
        }

        /// <summary>
        /// Called when the user selects the Network/Reset oculus view direction menu item
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        void resetTrackerOriginMenuItem_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: resetTrackerOriginMenuItem_Click(object, EventArgs)");
        
            ICommand c = new ICommand();
            c.CommandType = (int)eCommands.User + 1;
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);

            logger.Info("Reset VRPN Tracker origin on all network clients");
        }


        /// <summary>
        /// Called when the user selects the Network/Reset oculus view direction menu item
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        void resetOculusViewDirectionMenuItem_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: resetOculusViewDirectionMenuItem_Click(object, EventArgs)");

            ICommand c = new ICommand();
            c.CommandType = (int)eCommands.User+1;
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);

            logger.Info("Reset Oculus Rift view direction on all network clients");
        }


        /// <summary>
        /// Called when the user selects the Network/Scenes/Activate scene N menu item.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="args">Not used</param>
        void activateSceneMenuItem_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: activateSceneMenuItem_Click(object, EventArgs)");

            ToolStripMenuItem item = sender as ToolStripMenuItem;

            int sceneNumber = (int)item.Tag;
            ICommand c = new ICommand();
            c.CommandType = (int)eCommands.SelectScene;
            c.addParameter((int)eSelectSceneCommandParameters.SceneNumber, sceneNumber.ToString());
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);

            logger.Info("Activating scene " + sceneNumber.ToString() + " on all network clients.");
        }


        /// <summary>
        /// Called when the user selects the Network/start alignment screen menu item.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="args">Not used</param>
        void startAlignmentSceneMenuItem_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: startAlignmentSceneMenuItem_Click(object, EventArgs)");
 
            ICommand c = new ICommand();
            
            c.CommandType = (int)eCommands.FadeOut;
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);

            c.CommandType = (int)eCommands.SelectScene;
            c.addParameter((int)eSelectSceneCommandParameters.SceneNumber, "0");    // alignment scene
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);

            c = new ICommand();
            c.CommandType = (int)eCommands.Start;
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);
        }

        /// <summary>
        /// Called when the user selects the Network/stop alignment screen menu item.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="args">Not used</param>
        void stopAlignmentSceneMenuItem_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: stopAlignmentSceneMenuItem_Click(object, EventArgs)");
            
            ICommand c = new ICommand();
            c.CommandType = (int)eCommands.FadeOut;
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);

            c.CommandType = (int)eCommands.SelectScene;
            c.addParameter((int)eSelectSceneCommandParameters.SceneNumber, "1");    // first experiment scene
            AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);
        }

        /// <summary>
        /// Starts the network server
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        void startServerMenuItem_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: startServerMenuItemMenuItem_Click(object, EventArgs)");

            // start the network server
            if (AppMain.startServer())
            {
                startServerMenuItem.Enabled = false;
                stopServerMenuItem.Enabled = true;
            }

        }

        /// <summary>
        /// Stops the network server
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        void stopServerMenuItem_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: stopServerMenuItemMenuItem_Click(object, EventArgs)");

            AppMain.stopServer();

            startServerMenuItem.Enabled = true;
            stopServerMenuItem.Enabled = false;
        }

        /// <summary>
        /// Called when the user selects the Network/send protocol menu item.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="args">Not used</param>
        void sendProtocolMenuItemMenuItem_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: sendProtocolMenuItemMenuItem_Click(object, EventArgs)");

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.Filter = "Protocol files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.InitialDirectory = ConfigurationManager.AppSettings["TrialParametersDirectory"];

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string str = File.ReadAllText(openFileDialog1.FileName);
                    ICommand cmd = new ICommand();
                    cmd.CommandType = (int)eCommands.Protocol;
                    cmd.addParameter((int)eProtocolCommandParameters.Protocol, str);
                    AppMain.ServerHandler.sendCommandToRegisteredClients(cmd, null);
                }
                catch (Exception ex)
                {
                    logger.Error("[MainForm] Could not read protocol file " + openFileDialog1.FileName + ". Error: " + ex.Message);
                }
            }
        }
        

        /// <summary>
        /// Called when the user selects the Network/List network connections menu item.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="args">Not used</param>
        void listNetworkConnectionsMenuItem_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: listNetworkConnectionsMenuItem_Click(object, EventArgs)");
            
            AppMain.listActiveConnections();
        }

        /// <summary>
        /// Called when the user selects the File / Exit menu item.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="args">Not used</param>
        private static void exitMenuItem_Click(object sender, EventArgs args)
        {
            logger.Debug("Enter: exitMenuItem_Click(object, EventArgs)");

            ExitApplication(null);
        }

        /// <summary>
        /// Called when the user selects a task from the task menu.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="args">Not used</param>
        private void taskMenuItem_Click(object sender, EventArgs args)
        {
            HulkTask task;

            logger.Debug("Enter: taskMenuItem_Click(object, EventArgs)");

            task = (HulkTask)Activator.CreateInstance((Type)((ToolStripMenuItem)sender).Tag);
            AppMain.CurrentTask = task;

            Controls.Remove(taskPanel);
            taskPanel = new NamedPanel(task.TaskPanel, "Task", ColorScheme.Instance.TaskPanelMarker) {
                Margin = new Padding(0, 0, 3, 3)
            };
            Controls.Add(taskPanel);

            GraphPanel gp = graphPanel.ChildPanel as GraphPanel;
            task.AxesCentersChanged +=new HulkTask.AxesCentersChangedHandler(gp.axisCenters_Changed);
        }

        #endregion

        #region Update Timer

        /// <summary>
        /// Updates the GUI with the current information from the motion controller.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="args">Not used</param>
        private void updateTimer_Tick(object sender, EventArgs args)
        {
            bool hulkIsIdling = (Hulk.SystemStatus == Hulk.Status.Idling);
            bool haveNetworkClients = (AppMain.ServerHandler.getConnectedClientCount()!=0);

            scenesMenuItem.Enabled = hulkIsIdling && haveNetworkClients;
            sendProtocolMenuItem.Enabled = hulkIsIdling && haveNetworkClients;
            resetTrackerOriginMenuItem.Enabled = hulkIsIdling && haveNetworkClients;
            resetOculusViewDirectionMenuItem.Enabled = hulkIsIdling && haveNetworkClients;
            sendReconnectCommandToNetworkClientsMenuItem.Enabled = hulkIsIdling && haveNetworkClients;
            
            statusLabel.Text = Hulk.SystemStatus.ToString();
            joystickLabel.Text = InputController.IsJoystickConnected ? "Joystick: CONNECTED" : "Joystick: NONE";

            int numConnections = AppMain.ServerHandler.getConnectedClientCount();
            networkLabel.Text = AppMain.ServerHandler.isServerStarted ? "Network: " + numConnections.ToString() + " CLIENT(S)" : "Network: NONE";

            pcLabel.Text = DataController.Instance.PCReady ? "PC: OK" : "PC: ---";

            lightsLabel.Text = "Lights";
            lightsLabel.BackColor = DataController.Instance.IsLightGreen ? ColorScheme.Instance.StatusLightOK : ColorScheme.Instance.StatusLightError;

            plcLabel.Text = DataController.Instance.PLCReady ? "PLC: OK" : "PLC: ---";
            updateLabel.Text = "Updates / sec: " + Hulk.UpdateRate;

            ((LoggingPanel)loggingPanel.ChildPanel).updateTimer_Tick();

            taskPanel.Enabled = ((Hulk.SystemStatus == Hulk.Status.Idling) || (Hulk.SystemStatus == Hulk.Status.Task));
            tasksMenu.Enabled = (Hulk.SystemStatus == Hulk.Status.Idling);

            ((HulkTaskPanel)taskPanel.ChildPanel).updateTimer_Tick();
            ((ControlPanel)controlPanel.ChildPanel).updateTimer_Tick();
            ((GraphPanel)graphPanel.ChildPanel).updateTimer_Tick();
            ((StatusPanel)statusPanel.ChildPanel).updateTimer_Tick();

            // the refresh below causes flickering and is not needed. instead the GraphPanel is responsible to refresh itself whenever necessary
            // the rest of the control only refreshes areas that have been invalidated.
            //       mainForm.Refresh();
        }

        #endregion

        #region MainForm

        /// <summary>
        /// Called when the form exits using the red "X" button.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private static void MainForm_Closing(object sender, EventArgs e)
        {
            logger.Debug("Enter: MainForm_Closing(object, EventArgs)");

            ExitApplication(null);
        }

        /// <summary>
        /// Called when the form is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void MainForm_Layout(object sender, EventArgs e)
        {
            double yGrid;

            logger.Debug("Enter: MainForm_Layout(object, EventArgs)");

            if (!layoutComplete) {
                return;
            }

            yGrid = (ClientSize.Height - menuStrip.Height - statusBar.Height) / 5.0;

            int topRowHeight = (int)(yGrid * 3.0);

            // top row
            taskPanel.Location = new Point(0, menuStrip.Height);
            taskPanel.Size = new Size((int)(ClientSize.Width / 2.0), topRowHeight);

            graphPanel.Location = new Point(taskPanel.Right, menuStrip.Height);
            graphPanel.Size = new Size(ClientSize.Width - taskPanel.Width, topRowHeight);

            // bottom row

            int bottomRowY = menuStrip.Height + topRowHeight;
            int bottomRowHeight = ClientSize.Height - topRowHeight - statusBar.Height- menuStrip.Height;

            statusPanel.Location = new Point(0, bottomRowY);
            statusPanel.Size = new Size((int)(ClientSize.Width / 4.0), bottomRowHeight);

            controlPanel.Location = new Point(statusPanel.Right, bottomRowY);
            controlPanel.Size = new Size((int)(ClientSize.Width / 4.0), bottomRowHeight);

            audioPanel.Location = new Point(controlPanel.Right, bottomRowY);
            audioPanel.Size = new Size((int)(ClientSize.Width / 6.0), bottomRowHeight);

            loggingPanel.Location = new Point(audioPanel.Right, bottomRowY);
            loggingPanel.Size = new Size(ClientSize.Width - statusPanel.Width - controlPanel.Width - audioPanel.Width, bottomRowHeight);

        }

        /// <summary>
        /// Called when the form is loaded for the first time.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Timer updateTimer;

            logger.Debug("Enter: MainForm_Load(object, EventArgs)");

            // Start a background thread that will periodically update the GUI with info from the HULK controller

            updateTimer = new System.Windows.Forms.Timer {
                Interval = 100
            };
            updateTimer.Tick += updateTimer_Tick;
            updateTimer.Start();
        }

        #endregion

        #endregion
    }



    /// <summary>
    /// Custom coloring of toolstrip.
    /// </summary>
    internal class MenuRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            if (e.Vertical || (e.Item as ToolStripSeparator) == null)
                base.OnRenderSeparator(e);
            else
            {
                Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
                e.Graphics.FillRectangle(new SolidBrush(ColorScheme.Instance.MenuItemBackground), bounds);

                Pen pen = new Pen(ColorScheme.Instance.MenuBarBackground);
                int y = bounds.Height / 2;
                e.Graphics.DrawLine(pen, 0, y, bounds.Width, y );
                pen.Dispose();
                pen = new Pen(ColorScheme.Instance.MenuBarForeground);
                e.Graphics.DrawLine(pen, 0, y+1, bounds.Width, y+1);
                pen.Dispose();
            }
        }
        
        /// <summary>
        /// Paints the border on the left side of a menu item.
        /// </summary>
        /// <param name="e">The coordinates and graphics context for the drawing operation.</param>
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(ColorScheme.Instance.MenuBarBackground), e.AffectedBounds);
        }

        /// <summary>
        /// Paints the text on a menu item.
        /// </summary>
        /// <param name="e">The coordinates and graphics context for the drawing operation.</param>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if ((e.Item.Tag != null) && e.Item.Tag.Equals("PARENT")) {
                e.TextColor = ColorScheme.Instance.MenuBarForeground;
            } else {
                e.TextColor = (e.Item.Selected ? ColorScheme.Instance.MenuItemForeground : ColorScheme.Instance.MenuBarForeground);
            }
            base.OnRenderItemText(e);
        }

        /// <summary>
        /// Paints the background of a menu item.
        /// </summary>
        /// <param name="e">The coordinates and graphics context for the drawing operation.</param>
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            SolidBrush brush;

            if ((e.Item.Tag != null) && e.Item.Tag.Equals("PARENT")) {
                brush = new SolidBrush(e.Item.Selected ? ColorScheme.Instance.MenuBarSelected : ColorScheme.Instance.MenuBarBackground);
            } else {
                brush = new SolidBrush(e.Item.Selected ? ColorScheme.Instance.MenuItemSelected : ColorScheme.Instance.MenuItemBackground);
            }
            e.Graphics.FillRectangle(brush, new Rectangle(Point.Empty, e.Item.Size));
        }

        /// <summary>
        /// Paints the border on the right side of a menu item.
        /// </summary>
        /// <param name="e">The coordinates and graphics context for the drawing operation.</param>
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(ColorScheme.Instance.MenuBarBackground), e.AffectedBounds);
        }
    } 
}
