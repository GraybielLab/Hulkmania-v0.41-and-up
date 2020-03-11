using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SlimDX.DirectSound;
using System.Threading;

namespace Brandeis.AGSOL.Hulkamania
{
    public class SoundPlayer
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Delegates
        public delegate void PlayingDone();
        #endregion

        #region Fields

        private SecondarySoundBuffer soundPlayer = null;
        private String fileName = "";
        private bool looped = false;
        private float volume = 1;
        private bool dimVolume = false;
        private Thread workerThread = null;
        private NotificationPosition notificationPosition = new NotificationPosition();
        private PlayingDone playingDoneDelegate = null;
        private float dimmedVolumeLevel = 0.1f;   
        #endregion

        #region Properties
        /// <summary>
        /// Get/set the sound volume in the range [0, 1]
        /// </summary>
        public float Volume { get { return volume; } set { volume = Math.Min(1, Math.Max(0, value)); _updateVolume(volume); } }

        /// <summary>
        /// Get/set the sound volume when dimmed, in the range [0, 1]
        /// </summary>
        public float DimmedVolumeLevel { get { return dimmedVolumeLevel; } set { dimmedVolumeLevel = Math.Min(1, Math.Max(0, value)); } }

        /// <summary>
        /// Get the filename of the sound file
        /// </summary>
        public string FileName { get { return fileName; } }

        /// <summary>
        /// Get/set whether the volume should be dimmed
        /// </summary>
        public bool DimVolume { get { return dimVolume; } set { dimVolume = value; _updateVolume(dimVolume ? dimmedVolumeLevel : volume); } }

        /// <summary>
        /// Get/set whether the sound should loop
        /// </summary>
        public bool Loop { get { return looped; } set { looped = value; } }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the soundplayer. Do not call this directly but use SoundFactory.createSoundPlayer instead!
        /// </summary>
        /// 
        internal SoundPlayer(string file)
        {
            logger.Debug("Create: SoundPlayer");
            _setupSoundPlayer(file);
        }

        /// <summary>
        /// Destroys the soundplayer
        /// </summary>
         ~SoundPlayer()
        {
            SoundFactory.destroySoundPlayer(this);
        }

        #endregion

        #region Public functions
        /// <summary>
        /// Plays the currently configured sound
        /// </summary>
        public void Play(PlayingDone delegateHandler)
        {
            switch (looped)
            {
                case true:
                    // do not create a worker thread
                    if (soundPlayer != null)
                    {
                        soundPlayer.CurrentPlayPosition = 0;
                        soundPlayer.Play(0, PlayFlags.Looping);
                    }
                    break;
                case false:
                    // create a worker thread that gives a notification when the playback has completed
                    if ((workerThread == null) || (!workerThread.IsAlive))
                    {
                        playingDoneDelegate = delegateHandler;
                        workerThread = new Thread(_playThread);
                        workerThread.Start();
                    }
                    break;
            }

        }

        /// <summary>
        /// Plays the currently configured sound
        /// </summary>
        public void Stop()
        {
            try
            {
                if (soundPlayer != null)
                {
                    soundPlayer.Stop();
                }

                if (workerThread != null) {
                    workerThread.Abort();
                    workerThread = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error stopping sound: " + fileName + ", message: " + ex.Message);
            }
        }


        #endregion

        #region Private functions
        /// <summary>
        /// Initializes a media player with the given file
        /// </summary>
        /// <param name="fileName">the file to play</param>
        /// <returns>true if successful</returns>
        private bool _setupSoundPlayer(string filename)
        {
            bool retval = false;
            soundPlayer = null;

            if ((filename.Length != 0))
            {
                try
                {
                    fileName = filename;
                    soundPlayer = DirectSoundWrapper.CreateSoundBufferFromWave(fileName);
                    SlimDX.Multimedia.WaveStream waveFile = new SlimDX.Multimedia.WaveStream(fileName);

                    List<NotificationPosition> pos = new List<NotificationPosition>();
                    notificationPosition.Offset =  (int)waveFile.Length-1;
                    notificationPosition.Event = new AutoResetEvent(false);

                    pos.Add(notificationPosition);
                    soundPlayer.SetNotificationPositions(pos.ToArray());

                }
                catch (Exception ex)
                {
                    logger.Error("[SoundPlayer] Error creating soundplayer: " + ex.Message);
                }

                _updateVolume(volume);

                retval = true;
            }

            return retval;
        }

        /// <summary>
        /// worker thread to play the sample and wait for completion
        /// </summary>
        private void _playThread()
        {
            try {
                if (soundPlayer != null) {
                    soundPlayer.CurrentPlayPosition = 0;
                    soundPlayer.Play(0, PlayFlags.None);
                    notificationPosition.Event.WaitOne();
                    _playCompletedCallback();
                }
            } catch (System.Threading.ThreadAbortException) {
                // do nothing
            } catch (Exception ex) {
                logger.Error("Error playing sound: " + fileName + ", message: " + ex.Message);
            }
        }

        /// <summary>
        /// Called when playback of the sample is completed
        /// </summary>
        private void _playCompletedCallback()
        {
            if (playingDoneDelegate != null)
            {
                playingDoneDelegate();
            }
        }

        /// <summary>
        /// Called whenever the trackbar value is changed
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        void _updateVolume(float lvl)
        {
            if (soundPlayer != null)
            {
                soundPlayer.Volume = (int)(-2000 * (1 - (double)lvl));
            }
        }

        #endregion

    }
}
