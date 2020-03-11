using log4net;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania
{
    internal sealed class StatusPanel : Panel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private const int BOX_X = 80;
        private const int BOX_Y = 25;

        #endregion

        #region Fields

        private Label accelerationDescriptionLabel;
        private Label accelerationActualDescriptionLabel;
        private Label accelerationActualInnerLabel;
        private Label accelerationActualOuterLabel;
        private Label accelerationCommandDescriptionLabel;
        private Label accelerationCommandInnerLabel;
        private Label accelerationCommandOuterLabel; 
        private Label innerDescriptionLabel;
        private Label joystickDescriptionLabel;
        private Label joystickInnerLabel;
        private Label joystickOuterLabel;
        private Label outerDescriptionLabel;
        private Label positionDescriptionLabel;
        private Label positionActualDescriptionLabel;
        private Label positionActualInnerLabel;
        private Label positionActualOuterLabel;
        private Label positionCommandDescriptionLabel;
        private Label positionCommandInnerLabel;
        private Label positionCommandOuterLabel;
        private Label velocityDescriptionLabel;
        private Label velocityActualDescriptionLabel;
        private Label velocityActualInnerLabel;
        private Label velocityActualOuterLabel;
        private Label velocityCommandDescriptionLabel;
        private Label velocityCommandInnerLabel;
        private Label velocityCommandOuterLabel;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public StatusPanel()
        {
            logger.Debug("Create: StatusPanel");

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
            
            #region Labels
            
            accelerationDescriptionLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Accel (dpsps)"
            };

            accelerationActualDescriptionLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Actual:"
            };

            accelerationActualInnerLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            accelerationActualOuterLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            accelerationCommandDescriptionLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Command:"
            };

            accelerationCommandInnerLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            accelerationCommandOuterLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            innerDescriptionLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(BOX_X, BOX_Y),
                Text = "Inner (" + Hulk.InnerAxis + ")"
            };

            joystickDescriptionLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Joystick:"
            };

            joystickInnerLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            joystickOuterLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            outerDescriptionLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(BOX_X, BOX_Y),
                Text = "Outer (" + Hulk.OuterAxis + ")"
            };

            positionDescriptionLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(BOX_X, BOX_Y),
                Text = "Position (deg)"
            };

            positionActualDescriptionLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(BOX_X, BOX_Y),
                Text = "Actual:"
            };

            positionActualInnerLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(BOX_X, BOX_Y),
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            positionActualOuterLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(BOX_X, BOX_Y),
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            positionCommandDescriptionLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(BOX_X, BOX_Y),
                Text = "Command:"
            };

            positionCommandInnerLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(BOX_X, BOX_Y),
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            positionCommandOuterLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                TextAlign = ContentAlignment.MiddleLeft,
                Size = new Size(BOX_X, BOX_Y),
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            velocityDescriptionLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Velocity (dps)"
            };

            velocityActualDescriptionLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Actual:"
            };

            velocityActualInnerLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            velocityActualOuterLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            velocityCommandDescriptionLabel = new Label {
                ForeColor = ColorScheme.Instance.LabelForeground,
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Command:"
            };

            velocityCommandInnerLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            velocityCommandOuterLabel = new Label {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "",
                Size = new Size(BOX_X, BOX_Y),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            #endregion

            Controls.Add(accelerationDescriptionLabel);
            Controls.Add(accelerationActualDescriptionLabel);
            Controls.Add(accelerationActualInnerLabel);
            Controls.Add(accelerationActualOuterLabel);
            Controls.Add(accelerationCommandDescriptionLabel);
            Controls.Add(accelerationCommandInnerLabel);
            Controls.Add(accelerationCommandOuterLabel); 
            Controls.Add(innerDescriptionLabel);
            Controls.Add(joystickDescriptionLabel);
            Controls.Add(joystickInnerLabel);
            Controls.Add(joystickOuterLabel);
            Controls.Add(outerDescriptionLabel);
            Controls.Add(positionDescriptionLabel);
            Controls.Add(positionActualDescriptionLabel);
            Controls.Add(positionActualInnerLabel);
            Controls.Add(positionActualOuterLabel);
            Controls.Add(positionCommandDescriptionLabel);
            Controls.Add(positionCommandInnerLabel);
            Controls.Add(positionCommandOuterLabel);
            Controls.Add(velocityDescriptionLabel);
            Controls.Add(velocityActualDescriptionLabel);
            Controls.Add(velocityActualInnerLabel);
            Controls.Add(velocityActualOuterLabel);
            Controls.Add(velocityCommandDescriptionLabel);
            Controls.Add(velocityCommandInnerLabel);
            Controls.Add(velocityCommandOuterLabel);
            
            Layout += StatusPanel_Layout;

            ResumeLayout();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Updates the GUI with the current information from the motion controller.
        /// </summary>
        public void updateTimer_Tick()
        {
            ControlInput controlInput;
            
            // Show intended and actual position
            if (Hulk.LastCommand.outerPosition == Double.PositiveInfinity) {
                positionCommandOuterLabel.Text = "---";
                positionCommandInnerLabel.Text = "---";
            } else {
                positionCommandOuterLabel.Text = Hulk.LastCommand.outerPosition.ToString("F2");
                positionCommandInnerLabel.Text = Hulk.LastCommand.innerPosition.ToString("F2");
            }
            positionActualOuterLabel.Text = Hulk.CurrentMotion.outerPosition.ToString("F2");
            positionActualInnerLabel.Text = Hulk.CurrentMotion.innerPosition.ToString("F2");

            // Show intended and actual velocity
            if (Hulk.LastCommand.outerVelocity == Double.PositiveInfinity) {
                velocityCommandOuterLabel.Text = "---";
                velocityCommandInnerLabel.Text = "---";
            } else {
                velocityCommandOuterLabel.Text = Hulk.LastCommand.outerVelocity.ToString("F2");
                velocityCommandInnerLabel.Text = Hulk.LastCommand.innerVelocity.ToString("F2");
            }
            velocityActualOuterLabel.Text = Hulk.CurrentMotion.outerVelocity.ToString("F2");
            velocityActualInnerLabel.Text = Hulk.CurrentMotion.innerVelocity.ToString("F2");

            // Show intended and actual acceleration
            if (Hulk.LastCommand.outerAcceleration == Double.PositiveInfinity) {
                accelerationCommandOuterLabel.Text = "---";
                accelerationCommandInnerLabel.Text = "---";
            } else {
                accelerationCommandOuterLabel.Text = Hulk.LastCommand.outerAcceleration.ToString("F2");
                accelerationCommandInnerLabel.Text = Hulk.LastCommand.innerAcceleration.ToString("F2");
            }
            accelerationActualOuterLabel.Text = Hulk.CurrentMotion.outerAcceleration.ToString("F2");
            accelerationActualInnerLabel.Text = Hulk.CurrentMotion.innerAcceleration.ToString("F2");
            
            // Show joystick position
            if (InputController.IsJoystickConnected) {

                controlInput = InputController.JoystickInput;

                joystickOuterLabel.Text = controlInput.y.ToString("F2");
                joystickInnerLabel.Text = controlInput.x.ToString("F2");

            } else {
                joystickOuterLabel.Text = "NONE";
                joystickInnerLabel.Text = "NONE";
            }
        }

        /// <summary>
        /// Called when the panel is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void StatusPanel_Layout(object sender, EventArgs e)
        {
            int boxX;
            int boxY;

            logger.Debug("Enter: StatusPanel_Layout(object, EventArgs)");

            boxX = (Width - 20) / 4;
            boxY = (Height - 20) / 9;

            outerDescriptionLabel.Location = new Point(10 + boxX * 2, 10);
            innerDescriptionLabel.Location = new Point(10 + boxX * 3, 10);

            positionDescriptionLabel.Location = new Point(10, 10 + boxY);
            positionCommandDescriptionLabel.Location = new Point(10 + boxX, 10 + boxY);
            positionCommandOuterLabel.Location = new Point(10 + 2 * boxX, 10 + boxY);
            positionCommandInnerLabel.Location = new Point(10 + 3 * boxX, 10 + boxY);

            positionActualDescriptionLabel.Location = new Point(10 + boxX, 10 + 2 * boxY);
            positionActualOuterLabel.Location = new Point(10 + 2 * boxX, 10 + 2 * boxY);
            positionActualInnerLabel.Location = new Point(10 + 3 * boxX, 10 + 2 * boxY);

            velocityDescriptionLabel.Location = new Point(10, 10 + 3 * boxY);
            velocityCommandDescriptionLabel.Location = new Point(10 + boxX, 10 + 3 * boxY);
            velocityCommandOuterLabel.Location = new Point(10 + 2 * boxX, 10 + 3 * boxY);
            velocityCommandInnerLabel.Location = new Point(10 + 3 * boxX, 10 + 3 * boxY);

            velocityActualDescriptionLabel.Location = new Point(10 + boxX, 10 + 4 * boxY);
            velocityActualOuterLabel.Location = new Point(10 + 2 * boxX, 10 + 4 * boxY);
            velocityActualInnerLabel.Location = new Point(10 + 3 * boxX, 10 + 4 * boxY);

            accelerationDescriptionLabel.Location = new Point(10, 10 + 5 * boxY);
            accelerationCommandDescriptionLabel.Location = new Point(10 + boxX, 10 + 5 * boxY);
            accelerationCommandOuterLabel.Location = new Point(10 + 2 * boxX, 10 + 5 * boxY);
            accelerationCommandInnerLabel.Location = new Point(10 + 3 * boxX, 10 + 5 * boxY);

            accelerationActualDescriptionLabel.Location = new Point(10 + boxX, 10 + 6 * boxY);
            accelerationActualOuterLabel.Location = new Point(10 + 2 * boxX, 10 + 6 * boxY);
            accelerationActualInnerLabel.Location = new Point(10 + 3 * boxX, 10 + 6 * boxY);

            joystickDescriptionLabel.Location = new Point(10 + boxX, 10 + 8 * boxY);
            joystickOuterLabel.Location = new Point(10 + 2 * boxX, 10 + 8 * boxY);
            joystickInnerLabel.Location = new Point(10 + 3 * boxX, 10 + 8 * boxY);
        }

        #endregion
    }
}
