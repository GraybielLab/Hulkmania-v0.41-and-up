using log4net;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania.Tasks.PassiveMovement
{
    /// <summary>
    /// GUI panel for running passive movement trials.
    /// </summary>
    public sealed class PassiveMovementPanel : HulkTaskPanel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private Button fileLoadButton;
        private Button goButton;
        private Button stopButton;

        private FlickerFreeListBox recordingFilenamesListBox;
        
        private Label fileLabel;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public PassiveMovementPanel(PassiveMovementTask parentTask)
        {
            logger.Debug("Create: PassiveMovementPanel(PassiveMovementTask)");

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
                Text = "Go",
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
                Text = "Stop",
            };
            stopButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            stopButton.FlatAppearance.BorderSize = 2;
            stopButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            stopButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            stopButton.Click += stopButton_Click;

            #endregion

            #region Labels
            
            fileLabel = new Label {
                Text = "Trials: ",
                Width = 40
            };
            
            #endregion

            #region ListBox

            recordingFilenamesListBox = new FlickerFreeListBox {
                BackColor = ColorScheme.Instance.ListBoxBackground,           
                Enabled = false,
                ForeColor = ColorScheme.Instance.ListBoxForeground,
                Height = 250,
                Width = 300,
            };

            #endregion

            Controls.Add(fileLabel);
            Controls.Add(fileLoadButton);
            Controls.Add(goButton);
            Controls.Add(recordingFilenamesListBox);
            Controls.Add(stopButton);

            Layout += PassiveMovementPanel_Layout;

            ResumeLayout();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Perform post-task clean-up of GUI.
        /// Called from control loop through RotationTask.StopTask().
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

        /// <summary>
        /// Updates the listbox to show which trial is currently being replayed.
        /// Placed in a separate function so that it can be invoked on the GUI thread.
        /// </summary>
        internal void UpdateListbox()
        {
            int selectedNow;

            if (InvokeRequired) {
                Invoke(new EventHandlerDelegate(UpdateListbox));
            } else {
                
                logger.Debug("Enter: UpdateListbox()");

                selectedNow = recordingFilenamesListBox.SelectedIndex;
                if (recordingFilenamesListBox.Items.Count >= (selectedNow + 2)) {
                    recordingFilenamesListBox.ClearSelected();
                    recordingFilenamesListBox.SetSelected(selectedNow + 1, true);
                }
            }
        }

        #endregion

        #region Event Handlers

        #region Buttons

        /// <summary>
        /// Opens the file load dialog.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void fileLoadButton_Click(object sender, EventArgs e)
        {
            DialogResult result;
            OpenFileDialog dialog;
            Stream stream;
            String[] filenames;

            logger.Debug("Enter: fileLoadButton_Click(object, EventArgs)");

            dialog = new OpenFileDialog();
            dialog.DefaultExt = "csv";
            dialog.Filter = "Recording files (*.csv)|*.csv";
            dialog.Multiselect = true;
            dialog.InitialDirectory = ConfigurationManager.AppSettings["TrialRecordingsDirectory"];

            result = dialog.ShowDialog();
            if (result == DialogResult.OK) {

                // Operator selected one or more files. Loop through them and add to the list of timepoints.

                Recordings.Clear();
                recordingFilenamesListBox.Items.Clear();

                try {
                    filenames = dialog.FileNames;
                    Array.Sort(filenames);
                    
                    foreach (String s in filenames) {
                    
                        stream = File.OpenRead(s);
                        if (stream != null) {
                            recordingFilenamesListBox.Items.Add(s);
                            Recordings.Read(Path.GetFileName(s));
                        }
                    }

                    goButton.Enabled = true;
                } catch (Exception) {
                    MessageBox.Show("Error: Could not open file(s).");
                }
            }
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

                recordingFilenamesListBox.ClearSelected();
                recordingFilenamesListBox.SetSelected(0, true);

                fileLoadButton.Enabled = false;
                goButton.Enabled = false;
                stopButton.Enabled = true;

                ((PassiveMovementTask)task).Go();
            }
        }

        /// <summary>
        /// Stops the task.
        /// This method should ONLY contain a call to Hulk.StopTask().
        /// Any other post-task cleanup should be handled in PassiveMovementTask.StopTask() or PassiveMovementPanel.CleanUp().
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        internal void stopButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: stopButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed()) {
                Hulk.StopTask();
            }
        }

        #endregion
        
        /// <summary>
        /// Updates the GUI with the current information from the motion controller.
        /// </summary>
        public override void updateTimer_Tick()
        {
            String statusMessage;
            long trialTime;

            statusMessage = String.Format("Trial {0:000} : ", ((PassiveMovementTask)task).CurrentTrialNumber);
            
            trialTime = ((PassiveMovementTask)task).TrialTime;
            if (trialTime != -1) {
                statusMessage += trialTime + " sec";
            } else {
                statusMessage += "[No timer found]";
            }

            statusMessage += "   " + ((PassiveMovementTask)task).NumClicks + " clicks";

            MainForm.GetMainForm().TaskStatusLabel.Text = statusMessage;
        }

        /// <summary>
        /// Called when the panel is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void PassiveMovementPanel_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: PassiveMovementPanel_Layout(object, EventArgs)");
            
            if (Parent != null) {
                ((NamedPanel)Parent).ChildPanelName = "Task : Passive Movement";
            }

            goButton.Location = new Point(10, 10);
            stopButton.Location = new Point(goButton.Right + 5, 10);

            fileLabel.Location = new Point(stopButton.Right + 50, 14);
            recordingFilenamesListBox.Location = new Point(fileLabel.Right + 10, 10);
            fileLoadButton.Location = new Point(recordingFilenamesListBox.Right + 10, 10);
        }

        #endregion
    }
}
