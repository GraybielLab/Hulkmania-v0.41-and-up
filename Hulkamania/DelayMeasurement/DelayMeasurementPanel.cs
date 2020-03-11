using log4net;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania.Tasks.DelayMeasurement
{
    /// <summary>
    /// GUI panel for running delay measurement trials.
    /// </summary>
    public sealed class DelayMeasurementPanel : HulkTaskPanel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private Button goButton;
        private Button loadButton; 
        private Button stopButton;

        private Label amplitudeLabel;
        private Label offsetLabel;

        private TextBox amplitudeTextBox;
        private TextBox offsetTextBox;

        private String forcingFunctionsFilename;
              
        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public DelayMeasurementPanel(DelayMeasurementTask parentTask)
        {
            logger.Debug("Create: DelayMeasurementPanel(DelayMeasurementTask)");

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

            loadButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                Enabled = true,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Text = "Load Func"
            };
            loadButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            loadButton.FlatAppearance.BorderSize = 2;
            loadButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            loadButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            loadButton.Click += loadButton_Click;

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

            amplitudeLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Amplitude",
                TextAlign = ContentAlignment.MiddleLeft
            };
     
            offsetLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Offset (Roll): ",
                TextAlign = ContentAlignment.MiddleLeft
            };

            #endregion

            #region TextBoxes

            amplitudeTextBox = new TextBox() {
                BackColor = ColorScheme.Instance.TextBoxBackground,
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "2"
            };
            amplitudeTextBox.KeyPress += keyPress_CheckForNumeric;

            offsetTextBox = new TextBox() {
                BackColor = ColorScheme.Instance.TextBoxBackground,
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "0"
            };
            offsetTextBox.KeyPress += keyPress_CheckForNumeric;

            #endregion

            Controls.Add(amplitudeLabel);
            Controls.Add(amplitudeTextBox);
            Controls.Add(offsetLabel);
            Controls.Add(offsetTextBox);
            Controls.Add(goButton);
            Controls.Add(stopButton);
            Controls.Add(loadButton);
          
            Layout += DelayMeasurementPanel_Layout;
                        
            ResumeLayout();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Perform post-task clean-up of GUI.
        /// Called from control loop through DelayMeasurementTask.StopTask().
        /// </summary>
        internal void CleanUp()
        {
            if (InvokeRequired) {
                Invoke(new EventHandlerDelegate(CleanUp));
            } else {

                logger.Debug("Enter: CleanUp()");

                loadButton.Enabled = true;
                goButton.Enabled = true;
                stopButton.Enabled = false;

                amplitudeTextBox.Enabled = true;
                amplitudeTextBox.BackColor = ColorScheme.Instance.TextBoxBackground;
                offsetTextBox.Enabled = true;
                offsetTextBox.BackColor = ColorScheme.Instance.TextBoxBackground;
            }
        }

        #endregion

        #region Event Handlers

        #region Controls

        /// <summary>
        /// Ensures that only numeric values can be entered in the textbox.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void keyPress_CheckForNumeric(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && 
                (e.KeyChar != '.') && (e.KeyChar != '-')) {
                
                e.Handled = true;
            }

            // Only allow one decimal point
            if ((e.KeyChar == '.') && (((TextBox)sender).Text.IndexOf('.') > -1)) {
                e.Handled = true;
            }

            // Only allow one minus
            if (e.KeyChar == '-') {

                // Put minus sign at start of text
                ((TextBox)sender).Select(0, 0);

                if (((TextBox)sender).Text.IndexOf('-') > -1) {

                    e.Handled = true;
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
            
                loadButton.Enabled = false;
                goButton.Enabled = false;
                stopButton.Enabled = true;

                amplitudeTextBox.Enabled = false;
                amplitudeTextBox.BackColor = ColorScheme.Instance.TextBoxDisabledBackground;
                offsetTextBox.Enabled = false;
                offsetTextBox.BackColor = ColorScheme.Instance.TextBoxDisabledBackground;

                ((DelayMeasurementTask)task).Go(forcingFunctionsFilename, Double.Parse(amplitudeTextBox.Text),
                    Double.Parse(offsetTextBox.Text));
            }
        }

        /// <summary>
        /// Stops the task.
        /// This method should ONLY contain a call to Hulk.StopTask().
        /// Any other post-task cleanup should be handled in DelayMeasurementTask.StopTask() or DelayMeasurementPanel.CleanUp().
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: stopButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed()) {
                Hulk.StopTask();
            }
        }

        /// <summary>
        /// Loads a forcing function file.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog;
            
            logger.Debug("Enter: loadButton_Click(object, EventArgs)");
            
            dialog = new OpenFileDialog();
            dialog.DefaultExt = "csv";
            dialog.Filter = "Forcing function files (*.csv)|*.csv";
            dialog.Multiselect = false;
            dialog.InitialDirectory = Environment.CurrentDirectory;

            ForcingFunctions.Clear();

            if (dialog.ShowDialog() == DialogResult.OK) {
                try {
                    forcingFunctionsFilename = dialog.FileName;
                    ForcingFunctions.Read(Path.GetFileName(dialog.FileName));
                } catch (Exception) {
                    MessageBox.Show("Error: Could not open file.");
                }
            }

            goButton.Enabled = true;
        }

        #endregion

        /// <summary>
        /// Called when the panel is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void DelayMeasurementPanel_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: DelayMeasurementPanel_Layout(object, EventArgs)");

            goButton.Location = new Point((Width / 2) - 5 - goButton.Width, (Height - goButton.Height) / 2);
            stopButton.Location = new Point((Width / 2) + 5, (Height - goButton.Height) / 2);
            loadButton.Location = new Point(stopButton.Location.X + stopButton.Width + 5, stopButton.Location.Y);

            amplitudeLabel.Location = new Point((Width / 2) - 5 - amplitudeLabel.Width, goButton.Location.Y + goButton.Height + 5);
            amplitudeTextBox.Location = new Point((Width / 2) + 5, amplitudeLabel.Location.Y);

            offsetLabel.Location = new Point((Width / 2) - 5 - offsetLabel.Width, amplitudeTextBox.Location.Y + amplitudeTextBox.Height + 5);
            offsetTextBox.Location = new Point((Width / 2) + 5, offsetLabel.Location.Y);

            if (Parent != null) {
                ((NamedPanel)Parent).ChildPanelName = "Task : Delay Measurement";
            }
        }

        #endregion
    }
}
