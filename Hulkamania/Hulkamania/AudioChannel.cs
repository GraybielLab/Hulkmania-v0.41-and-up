using log4net;
using System;
using System.Drawing;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// Panel containing the control mode buttons.
    /// </summary>
    public sealed class AudioChannel : Panel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private Button playStopButton;
        private bool isPlaying = false;
        private Button loopButton;
        private bool looped = false;
        private Label fileLabel;
        private Label volumeLabel;
        private TrackBar volumeTrackbar;
        private int borderThickness = 1;

        private SoundPlayer soundPlayer = null;

        #endregion

        #region Properties 
        /// <summary>
        /// Get/set the sound volume in the range [0, 1]
        /// </summary>
        public double Volume { get { return volumeTrackbar.Value / (double)volumeTrackbar.Maximum; } set { volumeTrackbar.Value = (int) value * volumeTrackbar.Maximum; } }

        /// <summary>
        /// Get/set whether the sound should loop
        /// </summary>
        public bool Loop { get { return looped; } set { looped = value; } }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public AudioChannel(string file, string name, bool showLoopCheckbox)
        {
            logger.Debug("Create: AudioChannel");

            InitializeGUI(showLoopCheckbox);

            soundPlayer = SoundFactory.createSoundPlayer(file);
            fileLabel.Text = name;

        }
        #endregion

        #region GUI Creation

        /// <summary>
        /// Constructs the panel GUI.
        /// </summary>
        private void InitializeGUI(bool showLoopCheckbox)
        {
            logger.Debug("Enter: InitializeGUI()");

            SuspendLayout();

            BackColor = ColorScheme.Instance.TextBoxBackground;
            
            #region Controls

            playStopButton = new Button
            {
                BackColor = ColorScheme.Instance.ButtonBackground,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Enabled = true,
            };
            playStopButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            // playButton.FlatAppearance.BorderSize = 2;
            playStopButton.FlatAppearance.BorderSize = 0;
            playStopButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            playStopButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            playStopButton.Click += playStopButton_Click;

            playStopButton.Image = Image.FromFile(AppMain.BaseDirectory + "//Images//play.png");

            loopButton = new Button
            {
                BackColor = ColorScheme.Instance.ButtonBackground,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Enabled = true,
            };
            loopButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            // playButton.FlatAppearance.BorderSize = 2;
            loopButton.FlatAppearance.BorderSize = 0;
            loopButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            loopButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            loopButton.Click += loopButton_Click;

            loopButton.Image = Image.FromFile(AppMain.BaseDirectory + "//Images//repeat_inactive.png");
       
            fileLabel = new Label
            {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "N/A",
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            volumeLabel = new Label
            {
                ForeColor = ColorScheme.Instance.TextBoxForeground,
                Text = "Volume",
                TextAlign = ContentAlignment.TopLeft,
                BackColor = ColorScheme.Instance.TextBoxBackground
            };

            volumeTrackbar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 80,
                AutoSize = false,
                BackColor = ColorScheme.Instance.ButtonBackground,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                TickFrequency = 10
            };
            volumeTrackbar.ValueChanged += new EventHandler(volumeTrackbar_ValueChanged);

            #endregion

            Controls.Add(playStopButton);
            Controls.Add(loopButton);
            Controls.Add(volumeLabel);
            Controls.Add(fileLabel);
            Controls.Add(volumeTrackbar);
            Layout += AudioChannel_Layout;

            ResumeLayout();

        }


        #endregion

        #region Event Handlers

        /// <summary>
        /// Called whenever the trackbar value is changed
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        void volumeTrackbar_ValueChanged(object sender, EventArgs e)
        {
            if (soundPlayer != null)
            {
                soundPlayer.Volume = (float) (volumeTrackbar.Value / (volumeTrackbar.Maximum * 1.0));
            }
        }


        /// <summary>
        /// Paints a border around the panel
        /// </summary>
        /// <param name="e">not used</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            int thickness = borderThickness;
            int halfThickness = thickness / 2;
            using (System.Drawing.Pen p = new System.Drawing.Pen(ColorScheme.Instance.ButtonFlatBorder, thickness))
            {
                e.Graphics.DrawRectangle(p, new Rectangle(halfThickness,
                                                            halfThickness,
                                                            ClientSize.Width - thickness,
                                                            ClientSize.Height - thickness));
            }
        }


        /// <summary>
        /// Plays the sound
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void playStopButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: playButton_Click(object, EventArgs)");

            switch (isPlaying)
            {
                case false:
                    isPlaying = true;
                    soundPlayer.Stop();
                    soundPlayer.Loop = looped;
                    soundPlayer.Play(playbackCompleted);
                    fileLabel.BackColor = ColorScheme.Instance.AudioPanelActiveChannelColor;
                    playStopButton.Image = Image.FromFile(AppMain.BaseDirectory + "//Images//stop.png");
                    break;
                case true:
                    isPlaying = false;
                    soundPlayer.Stop();
                    fileLabel.BackColor = ColorScheme.Instance.TextBoxBackground;
                    playStopButton.Image = Image.FromFile(AppMain.BaseDirectory + "//Images//play.png");
                    break;
            }

        }

        private void playbackCompleted()
        {
            fileLabel.BackColor = ColorScheme.Instance.TextBoxBackground;
            playStopButton.Image = Image.FromFile(AppMain.BaseDirectory + "//Images//play.png");
        }

        /// <summary>
        /// toggles loop on or off
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void loopButton_Click(object sender, EventArgs e)
        {
            logger.Debug("Enter: loopButton_Click(object, EventArgs)");

            looped = !looped;
            loopButton.Image = Image.FromFile(AppMain.BaseDirectory + (looped ? "//Images//repeat_active.png" : "//Images//repeat_inactive.png"));
        }

        /// <summary>
        /// Called when the panel is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void AudioChannel_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: AudioChannel_Layout(object, EventArgs)");
            
            int clientRectangleWidth = ClientRectangle.Width - borderThickness * 2;
            int clientRectangleHeight = ClientRectangle.Height - borderThickness ;

            int buttonWidth = 25; // pixels
            playStopButton.Location = new Point(clientRectangleWidth - buttonWidth - borderThickness, borderThickness);
            playStopButton.Size = new Size(buttonWidth, (int)(clientRectangleHeight / 2.0));

            loopButton.Location = new Point(playStopButton.Left - buttonWidth, borderThickness);
            loopButton.Size = new Size(buttonWidth, (int)(clientRectangleHeight / 2.0));
            
            fileLabel.Location = new Point(borderThickness, borderThickness);
            fileLabel.Size = new Size((int)(clientRectangleWidth -  buttonWidth * 2), (int)(clientRectangleHeight / 2.0));

      
            volumeLabel.Location = new Point(borderThickness, (int)(clientRectangleHeight / 2.0)+1);
            volumeLabel.Size = new Size(50, (int)(clientRectangleHeight / 2.0));

            volumeTrackbar.Location = new Point(volumeLabel.Right, (int)(clientRectangleHeight / 2.0));
            volumeTrackbar.Size = new Size(clientRectangleWidth - volumeLabel.Width, (int)(clientRectangleHeight / 2.0));
        }

        #endregion
    }
}
