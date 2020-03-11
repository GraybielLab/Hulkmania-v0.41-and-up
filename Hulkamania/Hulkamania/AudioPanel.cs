using log4net;
using System;
using System.Drawing;
using System.Collections.Generic;
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
    public sealed class AudioPanel : Panel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private List<AudioChannel> audioChannels = new List<AudioChannel>();
        private Button pttButton = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public AudioPanel()
        {
            logger.Debug("Create: AudioPanel");

            
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

            BackColor = ColorScheme.Instance.PanelBackground;

            pttButton = new Button
            {
                BackColor = ColorScheme.Instance.ButtonBackground,
                FlatStyle = FlatStyle.Flat,
                ForeColor = ColorScheme.Instance.ButtonForeground,
                Enabled = true,
                Font = new Font(FontFamily.GenericSansSerif, 20.0f),
                Text = "Push to talk",
            };
            pttButton.FlatAppearance.BorderColor = ColorScheme.Instance.ButtonFlatBorder;
            pttButton.FlatAppearance.BorderSize = 2;
            pttButton.FlatAppearance.MouseOverBackColor = ColorScheme.Instance.ButtonMouseOver;
            pttButton.FlatAppearance.MouseDownBackColor = ColorScheme.Instance.ButtonMouseDown;
            pttButton.MouseDown += new MouseEventHandler(pttButton_MouseDown);
            pttButton.MouseUp += new MouseEventHandler(pttButton_MouseUp);
            pttButton.MouseLeave += new EventHandler(pttButton_MouseLeave);

            Controls.Add(pttButton);

            Layout += AudioPanel_Layout;
    
            ResumeLayout();
        }

        /// <summary>
        /// When the mouse leaves the push to talk button client area, deactivate push to talk on the soundfactory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pttButton_MouseLeave(object sender, EventArgs e)
        {
            SoundFactory.PushToTalk = false;
        }

        /// <summary>
        /// When the mouse button is released in the push to talk button client area, deactivate push to talk on the soundfactory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pttButton_MouseUp(object sender, MouseEventArgs e)
        {
            SoundFactory.PushToTalk = false;
        }

        /// <summary>
        /// When the mouse button is pressed in the push to talk button client area, deactivate push to talk on the soundfactory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pttButton_MouseDown(object sender, MouseEventArgs e)
        {
            SoundFactory.PushToTalk = true;
        }

        #endregion

        #region Event Handlers
        /// <summary>
        /// Handle panel creation - initialize slimdx directsound players
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            DirectSoundWrapper.Initialize(this);

            #region Create controls

            string soundFolder = Application.StartupPath + "\\AudioPanel";

            if (!Directory.Exists(soundFolder))
            {
                logger.Warn("AudioPanel: the sound folder '" + soundFolder + "' does not exist. No sounds have been loaded");
            }
            else
            {
                foreach (string file in Directory.GetFiles(soundFolder))
                {
                    AudioChannel ch = new AudioChannel(file, Path.GetFileName(file), true);
                    audioChannels.Add(ch);
                }
            }
            #endregion

            foreach (AudioChannel ac in audioChannels)
            {
                Controls.Add(ac);
            }
        }

        /// <summary>
        /// Handle panel destruction - clean up slimdx directsound
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            DirectSoundWrapper.Shutdown();
        }

        /// <summary>
        /// Called when the panel is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void AudioPanel_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: AudioPanel_Layout(object, EventArgs)");

            int count = 0;
            int channelHeight = (int)(ClientRectangle.Height * 1.0 / audioChannels.Count);
            channelHeight = 60;
            foreach (AudioChannel ac in audioChannels)
            {
                ac.Location = new Point(ClientRectangle.X, ClientRectangle.Y + count*channelHeight);
                ac.Size = new Size(ClientRectangle.Width, channelHeight);
                count += 1;
            }

            pttButton.Location = new Point(ClientRectangle.X, ClientRectangle.Height - 60);
            pttButton.Size = new Size(ClientRectangle.Width, 60);
        }

        #endregion
    }
}
