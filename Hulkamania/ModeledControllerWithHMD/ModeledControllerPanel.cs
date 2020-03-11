using log4net;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

using Brandeis.AGSOL.Network;
using Brandeis.AGSOL.Hulkamania;

namespace Brandeis.AGSOL.Hulkamania.Tasks.ModeledControllerWithHmd
{
    /// <summary>
    /// GUI panel for running balance trials.
    /// </summary>
    public sealed class ModeledControllerWithHmdPanel : HulkTaskPanel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields
        private JoystickGraph joystickGraph;

        private Button fileLoadButton;
        private Button goButton;
        private Button stopButton;
      
        private Label fileLabel;
        private Label filenameLabel;
        
        private TrialGrid trialsGrid;

        private Stopwatch protocolStopwatch;

        private Label controllerTypeLabel;
        private ComboBox controllerType;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public ModeledControllerWithHmdPanel(ModeledControllerWithHmdTask parentTask)
        {
            logger.Debug("Create: ModeledControllerWithHmdPanel(BalanceTask)");

            task = parentTask;

            InitializeGUI();
        }

        #endregion

        #region GUI Creation

        /// <summary>
        /// Constructs the panel GUI.
        /// </summary>
        private void InitializeGUI()
        {
            logger.Debug("Enter: InitializeGUI()");

            SuspendLayout();

            protocolStopwatch = new Stopwatch();
            protocolStopwatch.Stop();

            #region Buttons

            fileLoadButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Text = "..",
                Width = 40
            };
            fileLoadButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            fileLoadButton.FlatAppearance.BorderSize = 2;
            fileLoadButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            fileLoadButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            fileLoadButton.Click += fileLoadButton_Click;

            goButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                Enabled = false,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Text = "Go"
            };
            goButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            goButton.FlatAppearance.BorderSize = 2;
            goButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            goButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            goButton.Click += goButton_Click;

            stopButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                Enabled = false,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Text = "Stop"
            };
            stopButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            stopButton.FlatAppearance.BorderSize = 2;
            stopButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            stopButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            stopButton.Click += stopButton_Click;

            
            #endregion

            #region Labels

            fileLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Trials: ",
                Width = 40
            };

            controllerTypeLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Controller type:",
                TextAlign = ContentAlignment.MiddleLeft,
                Width = 90
            };

            filenameLabel = new Label {
                BackColor = ColorScheme.Instance.ListBoxBackground,
                ForeColor = ColorScheme.Instance.ListBoxForeground,
                TextAlign = ContentAlignment.MiddleLeft,
                Width = 200
            };

            #endregion

            #region Grid

            trialsGrid = new TrialGrid();
            
            #endregion

            controllerType = new ComboBox();
            controllerType.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (Object s in Enum.GetValues(typeof(ModeledControllerWithHmdTask.ControllerType)))
            {
                controllerType.Items.Add(s);
                if (((ModeledControllerWithHmdTask.ControllerType)s) == ((ModeledControllerWithHmdTask)task).ActiveController)
                {
                    controllerType.SelectedIndex = controllerType.Items.Count - 1;
                }
            }
            controllerType.SelectedIndexChanged += controllerType_SelectedIndexChanged;
            joystickGraph = new JoystickGraph();

            Controls.Add(joystickGraph);
            Controls.Add(fileLabel);
            Controls.Add(fileLoadButton);
            Controls.Add(filenameLabel);
            Controls.Add(goButton);
            Controls.Add(stopButton);
            Controls.Add(trialsGrid);
            Controls.Add(controllerType);
            Controls.Add(controllerTypeLabel);
            Layout += ModeledControllerWithHmdPanel_Layout;

            ResumeLayout();
        }


        #endregion

        #region Internal Methods


        /// <summary>
        /// Perform post-task clean-up of GUI.
        /// Called from control loop through BalanceTask.StopTask().
        /// </summary>
        internal void CleanUp()
        {
            if (InvokeRequired) {
                Invoke(new EventHandlerDelegate(CleanUp));
            } else {

                logger.Debug("Enter: CleanUp()");

                fileLoadButton.Enabled = true;
                goButton.Enabled = AppMain.UseDummyMotionController;
                stopButton.Enabled = false;
            }
        }

        #endregion

        #region Event Handlers

        #region Controls
        /// <summary>
        /// Called when the user selects a different controller type
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void controllerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModeledControllerWithHmdTask.ControllerType t = (ModeledControllerWithHmdTask.ControllerType) controllerType.SelectedItem;
            ((ModeledControllerWithHmdTask)task).ActiveController = t;
        }

        /// <summary>
        /// Opens the file load dialog.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void fileLoadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog;
            Stream stream;

            logger.Debug("Enter: fileLoadButton_Click(object, EventArgs)");

            dialog = new OpenFileDialog();
            dialog.DefaultExt = "csv";
            dialog.Filter = "Parameter files (*.csv)|*.csv";
            dialog.Multiselect = false;
            dialog.InitialDirectory = ConfigurationManager.AppSettings["TrialParametersDirectory"];

            if (dialog.ShowDialog() == DialogResult.OK) {
                bool readResult = true;

                try {
                    stream = dialog.OpenFile();
                    if (stream != null) {

                        filenameLabel.Text = dialog.SafeFileName;
                        readResult = Trials.Read(dialog.FileName);
                        Trials.CurrentTrialIndex = 0;

                        trialsGrid.PopulateList();
                        protocolStopwatch.Stop();
                        protocolStopwatch.Reset();
                    }
                } catch (Exception ex) {
                   MessageBox.Show(ex.Message, "Could not import protocol file");
                }

                if (readResult == true)
                {
                    logger.Info("Loaded protocol from file: " + dialog.FileName);
                }
                else
                {
                    logger.Warn("Could not import protocol file: " + dialog.FileName);
                }
            }

            goButton.Enabled = AppMain.UseDummyMotionController;
        }


        /// <summary>
        /// Starts the next trial.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void goButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: goButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed()) {
                protocolStopwatch.Start();

                fileLoadButton.Enabled = false;
                goButton.Enabled = false;
                stopButton.Enabled = true;

                ThreadPool.QueueUserWorkItem(new WaitCallback(((ModeledControllerWithHmdTask)task).Go));
            }
        }

        /// <summary>
        /// Stops the task.
        /// This method should ONLY contain a call to Hulk.StopTask().
        /// Any other post-task cleanup should be handled in BalanceTask.StopTask() or ModeledControllerWithHmdPanel.CleanUp().
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: stopButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed()) {
                protocolStopwatch.Stop();
                protocolStopwatch.Reset();

                Hulk.StopTask();

                ICommand c = new ICommand();
                c.CommandType = (int)eCommands.FadeOut;
                AppMain.ServerHandler.sendCommandToRegisteredClients(c, null);
            }
        }

        #endregion
      
        /// <summary>
        /// Updates the GUI with the current information from the motion controller.
        /// </summary>
        public override void updateTimer_Tick()
        {
            String statusMessage;

            ModeledControllerWithHmdTask mcTask = task as ModeledControllerWithHmdTask;
            joystickGraph.updateTimer_Tick(mcTask.ModeledControlInput.x, mcTask.ModeledControlInput.y, mcTask.ModeledControlInput.trigger);
            joystickGraph.Refresh();

            trialsGrid.updateTimer_Tick();

            statusMessage = "Protocol: " + (protocolStopwatch.ElapsedMilliseconds / 1000) + " sec";

            statusMessage += "     Trial: ";
            if (((ModeledControllerWithHmdTask)task).LogStopwatch != null) {
                statusMessage += (((ModeledControllerWithHmdTask)task).LogStopwatch.ElapsedMilliseconds / 1000) + " sec";
            } else {
                statusMessage += "0 sec";
            }

            statusMessage += "     Balancing: ";
            if (((ModeledControllerWithHmdTask)task).TrialStopwatch != null) {
                statusMessage += (((ModeledControllerWithHmdTask)task).TrialStopwatch.ElapsedMilliseconds / 1000) + " sec";
            } else {
                statusMessage += "0 sec";
            }

            statusMessage += "     Indications: ";
            if (Trials.CurrentTrial != null) {
                statusMessage += Trials.CurrentTrial.NumberIndications;
            }

            MainForm.GetMainForm().TaskStatusLabel.Text = statusMessage;
        }

        /// <summary>
        /// Called when the panel is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void ModeledControllerWithHmdPanel_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: ModeledControllerWithHmdPanel_Layout(object, EventArgs)");

            if (Parent != null) {
                ((NamedPanel)Parent).ChildPanelName = "Task : Modeled Controller (HMD)";
            }

            fileLabel.Location = new Point(5, 10);
            filenameLabel.Location = new Point(fileLabel.Right + 10, 5);
            fileLoadButton.Location = new Point(filenameLabel.Right + 10, 5);

            goButton.Size = stopButton.Size;
            goButton.Location = new Point(fileLoadButton.Right + 30, 5);
            stopButton.Location = new Point(goButton.Right + 5, 5);

            controllerTypeLabel.Location = new Point(stopButton.Right + 30, 5);

            controllerType.Location = new Point(controllerTypeLabel.Right + 5, 5);
            controllerType.Size = new System.Drawing.Size(100, stopButton.Height);

            trialsGrid.Location = new Point(0, stopButton.Bottom + 5);
            trialsGrid.Size = new Size(Width, Height - trialsGrid.Location.Y - Height/3);

            joystickGraph.Location = new Point(0, trialsGrid.Bottom);
            joystickGraph.Size = new Size(Width, Height - joystickGraph.Location.Y);
        }

        #endregion
    }
}
