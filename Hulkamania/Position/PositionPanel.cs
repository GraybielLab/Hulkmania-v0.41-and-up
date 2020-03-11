using log4net;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania.Tasks.Position
{
    /// <summary>
    /// GUI panel for running position trials.
    /// </summary>
    public sealed class PositionPanel : HulkTaskPanel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private Button goButton;
        private Button stopButton;

        private Label instructionLabel;
        private Label warningLabel;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public PositionPanel(PositionTask parentTask)
        {
             logger.Debug("Create: PositionPanel(PositionTask)");

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

            #region Label

            instructionLabel = new Label();
            instructionLabel.ForeColor = ColorScheme.Instance.LabelForeground;
            instructionLabel.Text = "Works from BACK position only";
            instructionLabel.TextAlign = ContentAlignment.MiddleCenter;

            warningLabel = new Label();
            warningLabel.Font = new Font(FontFamily.GenericSansSerif, 20);
            warningLabel.ForeColor = ColorScheme.Instance.LabelForeground;
            warningLabel.Text = "TEST USE ONLY - NO HUMANS ALLOWED IN CHAIR";
            warningLabel.TextAlign = ContentAlignment.MiddleCenter;

            #endregion

            Controls.Add(goButton);
            Controls.Add(stopButton);
            Controls.Add(instructionLabel);
            Controls.Add(warningLabel);
          
            Layout += PositionPanel_Layout;

            ResumeLayout();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Perform post-task clean-up of GUI.
        /// Called from control loop through PositionTask.StopTask().
        /// </summary>
        internal void CleanUp()
        {
            if (InvokeRequired) {
                Invoke(new EventHandlerDelegate(CleanUp));
            } else {

                logger.Debug("Enter: CleanUp()");

                goButton.Enabled = true;
                stopButton.Enabled = false;
            }
        }

        #endregion

        #region Event Handlers

        #region Controls

        /// <summary>
        /// Starts the next trial.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void goButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: goButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed()) {

                stopButton.Enabled = true;
                goButton.Enabled = false;

                ((PositionTask)task).Go();
            }
        }

        /// <summary>
        /// Stops the running of trials.
        /// This method should ONLY contain a call to Hulk.StopTask().
        /// Any other post-task cleanup should be handled in PositionTask.StopTask() or PositionPanel.CleanUp().
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
        
        #endregion

        /// <summary>
        /// Called when the panel is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void PositionPanel_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: PositionPanel_Layout(object, EventArgs)");

            warningLabel.Location = new Point(0, Height / 4);
            warningLabel.Width = Width;
            instructionLabel.Location = new Point(0, warningLabel.Location.Y + 50);
            instructionLabel.Width = Width;

            goButton.Location = new Point((Width / 2) - 5 - goButton.Width, (Height - goButton.Height) / 2);
            stopButton.Location = new Point((Width / 2) + 5, (Height - goButton.Height) / 2);
            
            if (Parent != null) {
                ((NamedPanel)Parent).ChildPanelName = "Task : Position";
            }
        }

        #endregion
    }
}
