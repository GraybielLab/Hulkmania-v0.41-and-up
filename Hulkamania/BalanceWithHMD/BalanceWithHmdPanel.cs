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

namespace Brandeis.AGSOL.Hulkamania.Tasks.BalanceWithHmd
{
    /// <summary>
    /// GUI panel for running balance trials.
    /// </summary>
    public sealed class BalanceWithHmdPanel : HulkTaskPanel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private Button fileLoadButton;
        private Button goButton;
        private Button stopButton;
      
        private Label fileLabel;
        private Label filenameLabel;
        
        private TrialGrid trialsGrid;

        private Stopwatch protocolStopwatch;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public BalanceWithHmdPanel(BalanceWithHmdTask parentTask)
        {
            logger.Debug("Create: BalanceWithHmdPanel(BalanceTask)");

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

            Controls.Add(fileLabel);
            Controls.Add(fileLoadButton);
            Controls.Add(filenameLabel);
            Controls.Add(goButton);
            Controls.Add(stopButton);
            Controls.Add(trialsGrid);

            Layout += BalanceWithHmdPanel_Layout;

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
                goButton.Enabled = true;
                stopButton.Enabled = false;
            }
        }

        #endregion

        #region Event Handlers

        #region Controls

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

            goButton.Enabled = true;
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

                ThreadPool.QueueUserWorkItem(new WaitCallback(((BalanceWithHmdTask)task).Go));
            }
        }

        /// <summary>
        /// Stops the task.
        /// This method should ONLY contain a call to Hulk.StopTask().
        /// Any other post-task cleanup should be handled in BalanceTask.StopTask() or BalanceWithHmdPanel.CleanUp().
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

            trialsGrid.updateTimer_Tick();

            statusMessage = "Protocol: " + (protocolStopwatch.ElapsedMilliseconds / 1000) + " sec";

            statusMessage += "     Trial: ";
            if (((BalanceWithHmdTask)task).LogStopwatch != null) {
                statusMessage += (((BalanceWithHmdTask)task).LogStopwatch.ElapsedMilliseconds / 1000) + " sec";
            } else {
                statusMessage += "0 sec";
            }

            statusMessage += "     Balancing: ";
            if (((BalanceWithHmdTask)task).TrialStopwatch != null) {
                statusMessage += (((BalanceWithHmdTask)task).TrialStopwatch.ElapsedMilliseconds / 1000) + " sec";
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
        private void BalanceWithHmdPanel_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: BalanceWithHmdPanel_Layout(object, EventArgs)");

            if (Parent != null) {
                ((NamedPanel)Parent).ChildPanelName = "Task : Balance (HMD)";
            }

            fileLabel.Location = new Point(5, 10);
            filenameLabel.Location = new Point(fileLabel.Right + 10, 5);
            fileLoadButton.Location = new Point(filenameLabel.Right + 10, 5);

            goButton.Size = stopButton.Size;
            goButton.Location = new Point(fileLoadButton.Right + 30, 5);
            stopButton.Location = new Point(goButton.Right + 5, 5);

            trialsGrid.Location = new Point(0, stopButton.Bottom + 5);
            trialsGrid.Size = new Size(Width, Height - trialsGrid.Location.Y);
        }

        #endregion


    }
}
