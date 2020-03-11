using log4net;
using System;
using System.Drawing;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// Panel containing the control mode buttons.
    /// </summary>
    public sealed class ControlPanel : Panel, IMessageFilter
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private Button backButton; 
        private Button hangButton;
        private Button holdButton;
        private Button homeButton;
        private Button killButton;
        private Button loadingButton;
        private Button maydayButton;
        private Button frontButton;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public ControlPanel()
        {
             logger.Debug("Create: ControlPanel");

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

            frontButton = new Button
            {
                BackColor = ColorScheme.Instance.ButtonBackground,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Enabled = false,
                Font = new Font(FontFamily.GenericSansSerif, 20.0f),
                Text = "Front",
            };
            frontButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            frontButton.FlatAppearance.BorderSize = 2;
            frontButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            frontButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            frontButton.Click += frontButton_Click;

            backButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Enabled = false,
                Font = new Font(FontFamily.GenericSansSerif, 20.0f),
                Text = "Back",
            };
            backButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            backButton.FlatAppearance.BorderSize = 2;
            backButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            backButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            backButton.Click += backButton_Click;

            hangButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Enabled = false,
                Font = new Font(FontFamily.GenericSansSerif, 20.0f),
                Text = "Hang",
            };
            hangButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            hangButton.FlatAppearance.BorderSize = 2;
            hangButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            hangButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            hangButton.Click += hangButton_Click;

            holdButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Enabled = false,
                Font = new Font(FontFamily.GenericSansSerif, 20.0f),
                Text = "Hold",
            };
            holdButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            holdButton.FlatAppearance.BorderSize = 2;
            holdButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            holdButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            holdButton.Click += holdButton_Click;

            homeButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Enabled = false,
                Font = new Font(FontFamily.GenericSansSerif, 20.0f),
                Text = "Home",
            };
            homeButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            homeButton.FlatAppearance.BorderSize = 2;
            homeButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            homeButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            homeButton.Click += homeButton_Click;

            killButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Enabled = false,
                Font = new Font(FontFamily.GenericSansSerif, 20.0f),
                Text = "Kill (F12)",
            };
            killButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            killButton.FlatAppearance.BorderSize = 2;
            killButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            killButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            killButton.Click += killButton_Click;

            loadingButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Enabled = false,
                Font = new Font(FontFamily.GenericSansSerif, 20.0f),
                Text = "Loading",
            };
            loadingButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            loadingButton.FlatAppearance.BorderSize = 2;
            loadingButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            loadingButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            loadingButton.Click += loadingButton_Click;

            maydayButton = new Button {
                BackColor = ColorScheme.Instance.ButtonBackground,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Enabled = false,
                Font = new Font(FontFamily.GenericSansSerif, 20.0f),
                Text = "Mayday (F1)",
            };
            maydayButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            maydayButton.FlatAppearance.BorderSize = 2;
            maydayButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            maydayButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            maydayButton.Click += maydayButton_Click;

            #endregion

            Controls.Add(backButton);
            Controls.Add(hangButton);
            Controls.Add(holdButton);
            Controls.Add(homeButton);
            Controls.Add(killButton);
            Controls.Add(loadingButton);
            Controls.Add(maydayButton);
            Controls.Add(frontButton);

            Layout += ControlPanel_Layout;

            // Grab keyboard input to look for function key shortcuts
            Application.AddMessageFilter(this);

            ResumeLayout();
        }

        #endregion

        #region Event Handlers

        #region Keyboard

        /// <summary>
        /// Processes function key shortcuts.
        /// </summary>
        /// <param name="message">The keyboard message</param>
        /// <returns>True if the shortcut is handled by this control, or false if other controls should try</returns>
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public bool PreFilterMessage(ref Message message)
        {
            if (message.Msg == 0x100) {
                Keys keyCode = (Keys)message.WParam & Keys.KeyCode;

                switch (keyCode) {
                    case Keys.F1:
                        maydayButton.PerformClick();
                        return true;
                    case Keys.F12:
                        killButton.PerformClick();
                        return true;
                }
            }
            return false;
        }

        #endregion

        #region Mode Buttons

        /// <summary>
        /// Instructs the Hulk to move to the BACK position.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private static void backButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: backButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed())
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Hulk.Back));
            }
        }     
        
        /// <summary>
        /// Instructs the Hulk to move to the FRONT position.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private static void frontButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: frontButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed())
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Hulk.Front));
            }
        }

        /// <summary>
        /// Instructs the Hulk to move to the HANG position.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private static void hangButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: hangButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed()) {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Hulk.Hang));
            }
        }

        /// <summary>
        /// Instructs the Hulk to hold position during initialization.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private static void holdButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: holdButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed()) {
                Hulk.Hold();
            }
        }

        /// <summary>
        /// Instructs the Hulk to begin the homing procedure.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private static void homeButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: homeButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed()) {
                Hulk.Home();
            }
        }

        /// <summary>
        /// Instructs the Hulk to take the motors offline.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private static void killButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: killButton_Click(object, EventArgs)");

            Hulk.Kill();
        }

        /// <summary>
        /// Instructs the Hulk to move to the LOADING position.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private static void loadingButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: loadingButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed()) {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Hulk.Loading));
            }
        }

        /// <summary>
        /// Instructs the Hulk to move to the MAYDAY position.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private static void maydayButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: maydayButton_Click(object, EventArgs)");

            if (Hulk.CheckCommandAllowed()) {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Hulk.Mayday));
            }
        }

        #endregion

        /// <summary>
        /// Updates the GUI with the current information from the motion controller.
        /// </summary>
        public void updateTimer_Tick()
        {
            // Enable or disable the mode buttons based on the current Hulk status.
            // This should prevent homing errors.

            switch (Hulk.SystemStatus) {
                
                case Hulk.Status.Initializing:
                    backButton.Enabled = false;
                    frontButton.Enabled = false;
                    hangButton.Enabled = false;
                    holdButton.Enabled = true;
                    homeButton.Enabled = false;
                    killButton.Enabled = false;
                    loadingButton.Enabled = false;
                    maydayButton.Enabled = false;
                    break;
            
                case Hulk.Status.NeedsHoming:
                    backButton.Enabled = false;
                    frontButton.Enabled = false;
                    hangButton.Enabled = false;
                    holdButton.Enabled = false;
                    homeButton.Enabled = true;
                    killButton.Enabled = true;
                    loadingButton.Enabled = false;
                    maydayButton.Enabled = false;
                    break;

                case Hulk.Status.Homing:
                    backButton.Enabled = false;
                    frontButton.Enabled = false;
                    hangButton.Enabled = false;
                    holdButton.Enabled = true;
                    homeButton.Enabled = false;
                    killButton.Enabled = true;
                    loadingButton.Enabled = false;
                    maydayButton.Enabled = false;
                    break;

                case Hulk.Status.Offline:
                    backButton.Enabled = false;
                    frontButton.Enabled = false;
                    hangButton.Enabled = false;
                    holdButton.Enabled = true;
                    homeButton.Enabled = false;
                    killButton.Enabled = false;
                    loadingButton.Enabled = false;
                    maydayButton.Enabled = false;
                    break;

                case Hulk.Status.Idling:
                    backButton.Enabled = true;
                    frontButton.Enabled = true;
                    hangButton.Enabled = true;
                    holdButton.Enabled = true;
                    homeButton.Enabled = false;
                    killButton.Enabled = true;
                    loadingButton.Enabled = true;
                    maydayButton.Enabled = true;
                    break;

                case Hulk.Status.Executing :
                    backButton.Enabled = true;
                    frontButton.Enabled = true;
                    hangButton.Enabled = true;
                    holdButton.Enabled = true;
                    homeButton.Enabled = false;
                    killButton.Enabled = true;
                    loadingButton.Enabled = true;
                    maydayButton.Enabled = true;
                    break;

                case Hulk.Status.Task:
                    backButton.Enabled = false;
                    frontButton.Enabled = false;
                    hangButton.Enabled = false;
                    holdButton.Enabled = true;
                    homeButton.Enabled = false;
                    killButton.Enabled = true;
                    loadingButton.Enabled = false;
                    maydayButton.Enabled = true;
                    break;

                case Hulk.Status.TaskExecuting:
                    backButton.Enabled = false;
                    frontButton.Enabled = false;
                    hangButton.Enabled = false;
                    holdButton.Enabled = true;
                    homeButton.Enabled = false;
                    killButton.Enabled = true;
                    loadingButton.Enabled = false;
                    maydayButton.Enabled = true;
                    break;

                case Hulk.Status.Stopping:
                    backButton.Enabled = false;
                    frontButton.Enabled = false;
                    hangButton.Enabled = false;
                    holdButton.Enabled = true;
                    homeButton.Enabled = false;
                    killButton.Enabled = true;
                    loadingButton.Enabled = false;
                    maydayButton.Enabled = true;
                    break;
            }
        }

        /// <summary>
        /// Called when the panel is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void ControlPanel_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: ControlPanel_Layout(object, EventArgs)");

            Size buttonSize = new Size(ClientRectangle.Width / 2, ClientRectangle.Height / 5);

            // row 1: mayday button
            maydayButton.Location = new Point(ClientRectangle.X, ClientRectangle.Y);
            maydayButton.Size = new Size(ClientRectangle.Width, buttonSize.Height);

            // row 2: hold / home buttons
            holdButton.Location = new Point(ClientRectangle.X, maydayButton.Location.Y + buttonSize.Height);
            holdButton.Size = buttonSize;

            homeButton.Location = new Point(ClientRectangle.X + buttonSize.Width, maydayButton.Location.Y + buttonSize.Height);
            homeButton.Size = buttonSize;

            // row 3: loading / hang buttons
            loadingButton.Location = new Point(ClientRectangle.X, holdButton.Location.Y + buttonSize.Height);
            loadingButton.Size = buttonSize;

            hangButton.Location = new Point(ClientRectangle.X + buttonSize.Width, holdButton.Location.Y + buttonSize.Height);
            hangButton.Size = buttonSize;

            // row 4: front / back buttons
            frontButton.Location = new Point(ClientRectangle.X, hangButton.Location.Y + buttonSize.Height);
            frontButton.Size = buttonSize;

            backButton.Location = new Point(ClientRectangle.X + buttonSize.Width, hangButton.Location.Y + buttonSize.Height);
            backButton.Size = buttonSize;

            // row 5: kill button
            killButton.Location = new Point(ClientRectangle.X, backButton.Location.Y + buttonSize.Height);
            killButton.Size = new Size(ClientRectangle.Width, buttonSize.Height);
        }

        #endregion
    }
}
