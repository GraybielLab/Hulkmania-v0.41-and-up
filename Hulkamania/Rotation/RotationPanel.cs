using log4net;
using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania.Tasks.Rotation
{
    /// <summary>
    /// GUI panel for rotating the chair.
    /// </summary>
    public sealed class RotationPanel : HulkTaskPanel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);
        
        #endregion

        #region Fields

        private TableLayoutPanel startStopPanel;
        private TableLayoutPanel parametersPanel;

        private Button goButton;
        private Button softStopButton;
        private Button hardStopButton;

        private CheckBox durationCheckBox;
        private CheckBox positionCheckBox;

        private Label innerAxisLabel;
        private Label outerAxisLabel;

        private Label accelerationCommandLabel;
        private Label velocityCommandLabel;

        private TextBox accelerationCommandInnerTextBox;
        private TextBox accelerationCommandOuterTextBox;
        private TextBox durationCommandTextBox;
        private TextBox positionCommandInnerTextBox;
        private TextBox positionCommandOuterTextBox;
        private TextBox velocityCommandInnerTextBox;
        private TextBox velocityCommandOuterTextBox;

        #endregion
            
        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public RotationPanel(RotationTask parentTask)
        {
             logger.Debug("Create: RotationPanel(RotationTask)");

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

            #region Panels

            startStopPanel = new TableLayoutPanel();
            startStopPanel.Margin = new Padding(10, 10, 10, 5);

            parametersPanel = new TableLayoutPanel();
            parametersPanel.Margin = new Padding(10, 5, 10, 10);

            #endregion

            #region Buttons

            goButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Text = "Go",
            };
            goButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            goButton.FlatAppearance.BorderSize = 2;
            goButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            goButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            goButton.Click += goButton_Click;

            softStopButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                Enabled = false,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Text = "Stop (Soft)",
            };
            softStopButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            softStopButton.FlatAppearance.BorderSize = 2;
            softStopButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            softStopButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            softStopButton.Click += softStopButton_Click;

            hardStopButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                Enabled = false,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Text = "Stop (Hard)",
            };
            hardStopButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            hardStopButton.FlatAppearance.BorderSize = 2;
            hardStopButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            hardStopButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            hardStopButton.Click += hardStopButton_Click;
            hardStopButton.Enabled = false;

            #endregion

            #region Labels

            accelerationCommandLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Accel (dpsps)",
                TextAlign = ContentAlignment.MiddleLeft
            };

            innerAxisLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Inner (" + Hulk.InnerAxis + ")"
            };

            outerAxisLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Outer (" + Hulk.OuterAxis + ")",
            };

            velocityCommandLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Velocity (dps)",
                TextAlign = ContentAlignment.MiddleLeft
            };

            #endregion

            #region Textboxes
            
            accelerationCommandInnerTextBox = new TextBox() {
                BackColor = ColorScheme.Instance.TextBoxBackground,
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "15"
            };
            accelerationCommandInnerTextBox.KeyPress += keyPress_CheckForNumeric;

            accelerationCommandOuterTextBox = new TextBox() {
                BackColor = ColorScheme.Instance.TextBoxBackground,
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "15"
            };
            accelerationCommandOuterTextBox.KeyPress += keyPress_CheckForNumeric;

            durationCommandTextBox = new TextBox() {
                BackColor = ColorScheme.Instance.TextBoxDisabledBackground,
                Enabled = false,
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "2"
            };
            durationCommandTextBox.KeyPress += keyPress_CheckForNumeric;

            positionCommandInnerTextBox = new TextBox() {
                BackColor = ColorScheme.Instance.TextBoxDisabledBackground,
                Enabled = false,
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "0"
            };
            positionCommandInnerTextBox.KeyPress += keyPress_CheckForNumeric;

            positionCommandOuterTextBox = new TextBox() {
                BackColor = ColorScheme.Instance.TextBoxDisabledBackground,
                Enabled = false,
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "0"
            };
            positionCommandOuterTextBox.KeyPress += keyPress_CheckForNumeric;

            velocityCommandInnerTextBox = new TextBox() {
                BackColor = ColorScheme.Instance.TextBoxBackground,
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "0"
            };
            velocityCommandInnerTextBox.KeyPress += keyPress_CheckForNumeric;

            velocityCommandOuterTextBox = new TextBox() {
                BackColor = ColorScheme.Instance.TextBoxBackground,
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "0"
            };
            velocityCommandOuterTextBox.KeyPress += keyPress_CheckForNumeric;

            #endregion

            #region Checkboxes

            durationCheckBox = new CheckBox();
            durationCheckBox.ForeColor = ColorScheme.Instance.LabelForeground;
            durationCheckBox.Text = "Duration (s)";
            durationCheckBox.CheckedChanged += durationCheckBox_CheckedChanged;

            positionCheckBox = new CheckBox();
            positionCheckBox.ForeColor = ColorScheme.Instance.LabelForeground;
            positionCheckBox.Text = "Position (deg)";
            positionCheckBox.CheckedChanged += positionCheckBox_CheckedChanged;

            #endregion

            startStopPanel.Controls.Add(goButton, 0, 0);
            startStopPanel.Controls.Add(softStopButton, 1, 0);
            startStopPanel.Controls.Add(hardStopButton, 2, 0);

            parametersPanel.Controls.Add(outerAxisLabel, 1, 0);
            parametersPanel.Controls.Add(innerAxisLabel, 2, 0);

            parametersPanel.Controls.Add(velocityCommandLabel, 0, 1);
            parametersPanel.Controls.Add(velocityCommandOuterTextBox, 1, 1);
            parametersPanel.Controls.Add(velocityCommandInnerTextBox, 2, 1);

            parametersPanel.Controls.Add(accelerationCommandLabel, 0, 2);
            parametersPanel.Controls.Add(accelerationCommandOuterTextBox, 1, 2);
            parametersPanel.Controls.Add(accelerationCommandInnerTextBox, 2, 2);

            parametersPanel.Controls.Add(new Label(), 0, 3);

            parametersPanel.Controls.Add(positionCheckBox, 0, 4);
            parametersPanel.Controls.Add(positionCommandOuterTextBox, 1, 4);
            parametersPanel.Controls.Add(positionCommandInnerTextBox, 2, 4);

            parametersPanel.Controls.Add(new Label(), 0, 5);

            parametersPanel.Controls.Add(durationCheckBox, 0, 6);
            parametersPanel.Controls.Add(durationCommandTextBox, 1, 6);

            Controls.Add(startStopPanel);
            Controls.Add(parametersPanel);

            Layout += RotationPanel_Layout;

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

                softStopButton.Enabled = false;
                hardStopButton.Enabled = false;
            }
        }
        
        #endregion

        #region Event Handlers

        #region Controls

        #region Textboxes

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

        #endregion

        #region Checkboxes

        /// <summary>
        /// Enable/disable duration command entry based on duration checkbox state.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void durationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            logger.Debug("Enter: durationCheckBox_CheckedChanged(object, EventArgs)");

            if (durationCheckBox.Checked) {

                durationCommandTextBox.Enabled = true;
                durationCommandTextBox.BackColor = ColorScheme.Instance.TextBoxBackground;

                positionCheckBox.Checked = false;

            } else {
                durationCommandTextBox.Enabled = false;
                durationCommandTextBox.BackColor = ColorScheme.Instance.TextBoxDisabledBackground;
            }
        }

        /// <summary>
        /// Enable/disable position command entry based on position checkbox state.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void positionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            logger.Debug("Enter: positionCheckBox_CheckedChanged(object, EventArgs)");

            if (positionCheckBox.Checked) {
                
                positionCommandInnerTextBox.Enabled = true;
                positionCommandInnerTextBox.BackColor = ColorScheme.Instance.TextBoxBackground;
                positionCommandOuterTextBox.Enabled = true;
                positionCommandOuterTextBox.BackColor = ColorScheme.Instance.TextBoxBackground;

                durationCheckBox.Checked = false;

            } else {
                
                positionCommandInnerTextBox.Enabled = false;
                positionCommandInnerTextBox.BackColor = ColorScheme.Instance.TextBoxDisabledBackground;
                positionCommandOuterTextBox.Enabled = false;
                positionCommandOuterTextBox.BackColor = ColorScheme.Instance.TextBoxDisabledBackground;
            }
        }

        #endregion

        #region Buttons

        /// <summary>
        /// Starts the movement.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void goButton_Click(object sender, EventArgs e)
        {
            MotionCommand newMotionCommand;
            double duration;

            logger.Debug("Enter: goButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed()) {

                newMotionCommand = new MotionCommand();
                newMotionCommand.innerAcceleration = Double.Parse(accelerationCommandInnerTextBox.Text);
                newMotionCommand.outerAcceleration = Double.Parse(accelerationCommandOuterTextBox.Text);
                newMotionCommand.innerVelocity = Double.Parse(velocityCommandInnerTextBox.Text);
                newMotionCommand.outerVelocity = Double.Parse(velocityCommandOuterTextBox.Text);

                if (positionCheckBox.Checked) {
                    newMotionCommand.innerPosition = Double.Parse(positionCommandInnerTextBox.Text);
                    newMotionCommand.outerPosition = Double.Parse(positionCommandOuterTextBox.Text);
                    softStopButton.Enabled = false;
                    hardStopButton.Enabled = false;
                } else {
                    softStopButton.Enabled = true;
                    hardStopButton.Enabled = true;
                }

                duration = -1.0;
                if (durationCheckBox.Checked) {
                    duration = Double.Parse(durationCommandTextBox.Text);
                }

                ((RotationTask)task).Go(newMotionCommand, duration);
            }
        }

        /// <summary>
        /// Stops the task.
        /// This method should ONLY contain a call to Hulk.StopTask().
        /// Any other post-task cleanup should be handled in RotationTask.StopTask() or RotationPanel.CleanUp().
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void hardStopButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: hardStopButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed()) {
                Hulk.StopTask();
            }
        }

        /// <summary>
        /// Gradually stops any rotation of the HULK, using the currently specified deceleration parameters.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void softStopButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: softStopButton_Click(object, EventArgs)");

            ThreadPool.QueueUserWorkItem(new WaitCallback(((RotationTask)task).SoftStop));
        }

        #endregion

        #endregion

        /// <summary>
        /// Called when the panel is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void RotationPanel_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: RotationPanel_Layout(object, EventArgs)");

            startStopPanel.Width = base.Width;
            startStopPanel.Height = goButton.Height + startStopPanel.Margin.Top + startStopPanel.Margin.Bottom;

            parametersPanel.Location = new Point(0, startStopPanel.Height);
            parametersPanel.Width = base.Width;
            parametersPanel.Height = base.Height - startStopPanel.Height;

            if (Parent != null) {
                ((NamedPanel)Parent).ChildPanelName = "Task : Rotation";
            }
        }

        #endregion
    }
}
