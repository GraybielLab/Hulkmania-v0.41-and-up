using log4net;
using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using Brandeis.AGSOL.Network;

namespace Brandeis.AGSOL.Hulkamania.Tasks.UnrealLandscapesDemo
{
    /// <summary>
    /// GUI panel for rotating the chair.
    /// </summary>
    public sealed class UnrealLandscapesDemoPanel : HulkTaskPanel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);
        
        #endregion

        #region Fields
        private Label unrealPositionXLabel;
        private Label unrealPositionYLabel;
        private Label unrealPositionZLabel;
        private Label unrealVelocityXLabel;
        private Label unrealVelocityYLabel;
        private Label unrealVelocityZLabel;
        private Label unrealAccelerationXLabel;
        private Label unrealAccelerationYLabel;
        private Label unrealAccelerationZLabel;
        private Label unrealForceXLabel;
        private Label unrealForceYLabel;
        private Label unrealForceZLabel;

        private Label unrealXLabel;
        private Label unrealYLabel;
        private Label unrealZLabel;

        private Label unrealPositionLabel;
        private Label unrealVelocityLabel;
        private Label unrealAccelerationLabel;
        private Label unrealForceLabel;
        
        private TableLayoutPanel unrealDataPanel;
        
        private TableLayoutPanel startStopPanel;
        private TableLayoutPanel parametersPanel;

        private Button goButton;
        private Button softStopButton;
        private Button hardStopButton;

        private CheckBox orientationCheckBox;

        private Label innerAxisLabel;
        private Label outerAxisLabel;

        private Label maxAngleLabel;

        private TextBox maxAngleInnerTextBox;
        private TextBox maxAngleOuterTextBox;

        #endregion
            
        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public UnrealLandscapesDemoPanel(UnrealLandscapesDemoTask parentTask)
        {
             logger.Debug("Create: UnrealLandscapesDemoPanel(RotationTask)");

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

            unrealDataPanel = new TableLayoutPanel();
            unrealDataPanel.Margin = new Padding(10, 10, 10, 5);

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

            unrealPositionLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Unreal Pos",
                TextAlign = ContentAlignment.MiddleLeft
            };
            unrealPositionXLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "---",
                TextAlign = ContentAlignment.MiddleCenter
            };
            unrealPositionYLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "---",
                TextAlign = ContentAlignment.MiddleCenter
            };
            unrealPositionZLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "---",
                TextAlign = ContentAlignment.MiddleCenter
            };

            unrealVelocityLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Unreal Vel",
                TextAlign = ContentAlignment.MiddleLeft
            };
            unrealVelocityXLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "---",
                TextAlign = ContentAlignment.MiddleCenter
            };
            unrealVelocityYLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "---",
                TextAlign = ContentAlignment.MiddleCenter
            };
            unrealVelocityZLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "---",
                TextAlign = ContentAlignment.MiddleCenter
            };

            unrealAccelerationLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Unreal Acc",
                TextAlign = ContentAlignment.MiddleLeft
            };
            unrealAccelerationXLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "---",
                TextAlign = ContentAlignment.MiddleCenter
            };
            unrealAccelerationYLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "---",
                TextAlign = ContentAlignment.MiddleCenter
            };
            unrealAccelerationZLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "---",
                TextAlign = ContentAlignment.MiddleCenter
            };


            unrealForceLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Unreal For",
                TextAlign = ContentAlignment.MiddleLeft
            };
            unrealForceXLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "---",
                TextAlign = ContentAlignment.MiddleCenter
            };
            unrealForceYLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "---",
                TextAlign = ContentAlignment.MiddleCenter
            };
            unrealForceZLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "---",
                TextAlign = ContentAlignment.MiddleCenter
            };


            unrealXLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "X",
                TextAlign = ContentAlignment.MiddleCenter
            };
            unrealYLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Y",
                TextAlign = ContentAlignment.MiddleCenter
            };
            unrealZLabel = new Label
            {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Z",
                TextAlign = ContentAlignment.MiddleCenter
            };






            innerAxisLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Inner (" + Hulk.InnerAxis + ")"
            };

            outerAxisLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Outer (" + Hulk.OuterAxis + ")",
            };

            maxAngleLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Text = "Max Angle",
                TextAlign = ContentAlignment.MiddleLeft
            };

            #endregion

            #region Textboxes
            
            maxAngleInnerTextBox = new TextBox() {
                BackColor = ColorScheme.Instance.TextBoxBackground,
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "0",
                Enabled = false
            };
            maxAngleInnerTextBox.KeyPress += keyPress_CheckForNumeric;

            maxAngleOuterTextBox = new TextBox() {
                BackColor = ColorScheme.Instance.TextBoxBackground,
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "0",
                Enabled = false
            };
            maxAngleOuterTextBox.KeyPress += keyPress_CheckForNumeric;

            #endregion

            #region Checkboxes

            orientationCheckBox = new CheckBox();
            orientationCheckBox.ForeColor = ColorScheme.Instance.LabelForeground;
            orientationCheckBox.Text = "Use orientation";
            orientationCheckBox.CheckedChanged += orientationCheckBox_CheckedChanged;
            orientationCheckBox.Checked = true;

            #endregion


            unrealDataPanel.Controls.Add(unrealXLabel, 2, 0);
            unrealDataPanel.Controls.Add(unrealYLabel, 3, 0);
            unrealDataPanel.Controls.Add(unrealZLabel, 4, 0);

            unrealDataPanel.Controls.Add(unrealPositionLabel, 1, 1);
            unrealDataPanel.Controls.Add(unrealPositionXLabel, 2, 1);
            unrealDataPanel.Controls.Add(unrealPositionYLabel, 3, 1);
            unrealDataPanel.Controls.Add(unrealPositionZLabel, 4, 1);

            unrealDataPanel.Controls.Add(unrealVelocityLabel, 1, 2);
            unrealDataPanel.Controls.Add(unrealVelocityXLabel, 2, 2);
            unrealDataPanel.Controls.Add(unrealVelocityYLabel, 3, 2);
            unrealDataPanel.Controls.Add(unrealVelocityZLabel, 4, 2);

            unrealDataPanel.Controls.Add(unrealAccelerationLabel, 1, 3);
            unrealDataPanel.Controls.Add(unrealAccelerationXLabel, 2, 3);
            unrealDataPanel.Controls.Add(unrealAccelerationYLabel, 3, 3);
            unrealDataPanel.Controls.Add(unrealAccelerationZLabel, 4, 3);

            unrealDataPanel.Controls.Add(unrealForceLabel, 1, 4);
            unrealDataPanel.Controls.Add(unrealForceXLabel, 2, 4);
            unrealDataPanel.Controls.Add(unrealForceYLabel, 3, 4);
            unrealDataPanel.Controls.Add(unrealForceZLabel, 4, 4);

            startStopPanel.Controls.Add(goButton, 0, 0);
            startStopPanel.Controls.Add(softStopButton, 1, 0);
            startStopPanel.Controls.Add(hardStopButton, 2, 0);

            parametersPanel.Controls.Add(outerAxisLabel, 1, 0);
            parametersPanel.Controls.Add(innerAxisLabel, 2, 0);

            parametersPanel.Controls.Add(maxAngleLabel, 0, 1);
            parametersPanel.Controls.Add(maxAngleOuterTextBox, 1, 1);
            parametersPanel.Controls.Add(maxAngleInnerTextBox, 2, 1);


            parametersPanel.Controls.Add(new Label(), 0, 5);

            parametersPanel.Controls.Add(orientationCheckBox, 0, 6);

            Controls.Add(unrealDataPanel);
            Controls.Add(startStopPanel);
            Controls.Add(parametersPanel);

            Layout += UnrealLandscapesDemoPanel_Layout;

            ResumeLayout();
        }

        #endregion

        #region Internal Methods
        /// <summary>
        /// Called by the task whenever data from unreal is received
        /// </summary>
        internal void setUnrealData(Vector3 Position, Vector3 Velocity, Vector3 Acceleration, Vector3 Force)
        {
            SetControlPropertyThreadSafe(unrealPositionXLabel, "Text", Position.x.ToString());
            SetControlPropertyThreadSafe(unrealPositionYLabel, "Text", Position.z.ToString());
            SetControlPropertyThreadSafe(unrealPositionZLabel, "Text", Position.y.ToString());

            SetControlPropertyThreadSafe(unrealVelocityXLabel, "Text", Velocity.x.ToString());
            SetControlPropertyThreadSafe(unrealVelocityYLabel, "Text", Velocity.z.ToString());
            SetControlPropertyThreadSafe(unrealVelocityZLabel, "Text", Velocity.y.ToString());

            SetControlPropertyThreadSafe(unrealAccelerationXLabel, "Text", Acceleration.x.ToString());
            SetControlPropertyThreadSafe(unrealAccelerationYLabel, "Text", Acceleration.z.ToString());
            SetControlPropertyThreadSafe(unrealAccelerationZLabel, "Text", Acceleration.y.ToString());

            SetControlPropertyThreadSafe(unrealForceXLabel, "Text", Force.x.ToString());
            SetControlPropertyThreadSafe(unrealForceYLabel, "Text", Force.z.ToString());
            SetControlPropertyThreadSafe(unrealForceZLabel, "Text", Force.y.ToString());

        }


        // -------------------------------------------------------------------------------------------------------------------------
        // Threadsafe way of changing control properties.
        private delegate void SetControlPropertyThreadSafeDelegate(Control control, string propertyName, object propertyValue);
        private void SetControlPropertyThreadSafe(Control control, string propertyName, object propertyValue)
        {
            // do nothing if the form is closed/closing
            if (this.Visible == false)
            {
                return;
            }

            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlPropertyThreadSafeDelegate(SetControlPropertyThreadSafe), new object[] { control, propertyName, propertyValue });
            }
            else
            {
                control.GetType().InvokeMember(propertyName, BindingFlags.SetProperty, null, control, new object[] { propertyValue });
            }
        }


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
        private void orientationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            logger.Debug("Enter: orientationCheckBox_CheckedChanged(object, EventArgs)");

            maxAngleInnerTextBox.Enabled = !orientationCheckBox.Checked;
            maxAngleOuterTextBox.Enabled = !orientationCheckBox.Checked;

            // does not do anything yet
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

                MessageBox.Show("NOT IMPLEMENTED YET!");

                //newMotionCommand = new MotionCommand();
                //newMotionCommand.innerAcceleration = Double.Parse(accelerationCommandInnerTextBox.Text);
                //newMotionCommand.outerAcceleration = Double.Parse(accelerationCommandOuterTextBox.Text);
                //newMotionCommand.innerVelocity = Double.Parse(velocityCommandInnerTextBox.Text);
                //newMotionCommand.outerVelocity = Double.Parse(velocityCommandOuterTextBox.Text);

                //softStopButton.Enabled = true;
                //hardStopButton.Enabled = true;

                //((UnrealLandscapesDemoTask)task).Go(newMotionCommand, 0);
            }
        }

        /// <summary>
        /// Stops the task.
        /// This method should ONLY contain a call to Hulk.StopTask().
        /// Any other post-task cleanup should be handled in RotationTask.StopTask() or UnrealLandscapesDemoPanel.CleanUp().
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

            ThreadPool.QueueUserWorkItem(new WaitCallback(((UnrealLandscapesDemoTask)task).SoftStop));
        }

        #endregion

        #endregion

        /// <summary>
        /// Called when the panel is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void UnrealLandscapesDemoPanel_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: UnrealLandscapesDemoPanel_Layout(object, EventArgs)");

            startStopPanel.Location = new Point(0, 0);
            startStopPanel.Width = base.Width;
            startStopPanel.Height = goButton.Height + startStopPanel.Margin.Top + startStopPanel.Margin.Bottom ;

            unrealDataPanel.Location = new Point(0, startStopPanel.Height);
            unrealDataPanel.Width = base.Width;
         
            parametersPanel.Location = new Point(0, startStopPanel.Height + unrealDataPanel.Height);
            parametersPanel.Width = base.Width;
      //      parametersPanel.Height = base.Height - startStopPanel.Height - unrealDataPanel.Height;

            if (Parent != null) {
                ((NamedPanel)Parent).ChildPanelName = "Task : UnrealLandscapesDemo";
            }
        }

        #endregion
    }
}
