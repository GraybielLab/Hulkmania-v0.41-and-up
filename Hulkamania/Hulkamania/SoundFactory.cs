using System;
using System.Collections.Generic;
using System.Text;

namespace Brandeis.AGSOL.Hulkamania
{
    static public class SoundFactory
    {
        #region Fields
        private static List<SoundPlayer> mSoundPlayers = new List<SoundPlayer>();
        private static bool mPushToTalkActive = false;
        private static bool mAllVolumesDimmed = false;
        #endregion

        #region Properties
        public static bool PushToTalk { get { return mPushToTalkActive; } set { mPushToTalkActive = value; dimAllVolumes(mAllVolumesDimmed, null); } }
        #endregion
        
        #region Public functions
        /// <summary>
        /// Dims/restores the volume on all the soundplayer instances
        /// </summary>
        /// <param name="dimmed">True if volume should be dimmed, false if it needs to be restored</param>
        /// <param name="solo">Optional soundplayer instance of which the volume should remain unaffected</param>
        public static void dimAllVolumes(bool dimmed, SoundPlayer solo)
        {
            // if push to talk is active, all sounds are dimmed
            SoundPlayer soloPlayer = mPushToTalkActive ? null : solo;

            foreach (SoundPlayer p in mSoundPlayers)
            {
                if (solo != p)
                {
                    p.DimVolume = dimmed | mPushToTalkActive;
                }
            }

            mAllVolumesDimmed = dimmed;
        }

        /// <summary>
        /// Removes a sound player from the tracked list of soundplayer instances
        /// </summary>
        /// <param name="thePlayer">the sound player instance to remove</param>
        public static void destroySoundPlayer(SoundPlayer thePlayer)
        {
            mSoundPlayers.Remove(thePlayer);
        }

        /// <summary>
        /// Create a sound player object
        /// </summary>
        /// <param name="fileName">the name of the wave file</param>
        /// <returns>the created soundplayer object</returns>
        public static SoundPlayer createSoundPlayer(string fileName)
        {
            SoundPlayer retval = null;

            foreach (SoundPlayer p in mSoundPlayers)
            {
                if (p.FileName == fileName)
                {
                    retval = p;
                    break;
                }
            }

            if (retval == null)
            {
                retval = new SoundPlayer(fileName);
                mSoundPlayers.Add(retval);
            }

            return retval;
        }
        #endregion
    }
}
