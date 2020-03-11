    using System;
    using System.Windows.Forms;
    using System.IO;
    using System.Collections.Generic;

    using SlimDX;
    using SlimDX.Multimedia;
    using SlimDX.DirectSound;

//    using OggVorbisDecoder;

    namespace Brandeis.AGSOL.Hulkamania
    {
        public sealed class DirectSoundWrapper
        {
            #region Public/Internal Properties
            public static DirectSound Device
            {
                get { return device; }
            }
            #endregion

            #region Private Members
            private static DirectSound device = null;
            private static List<SecondarySoundBuffer> soundBuffers = new List<SecondarySoundBuffer>();
            #endregion

            /// <summary>
            /// Initialize DirectSound for the specified Window
            /// </summary>
            /// <param name="HWnd">Handle of the window for which DirectSound is to be initialized</param>
            public static void Initialize(Control Parent)
            {
                device = new DirectSound();
                device.SetCooperativeLevel(Parent.Handle, CooperativeLevel.Priority);
                device.IsDefaultPool = false;
            }

            /// <summary>
            /// Disposes of any directsound resources
            /// </summary>
            public static void Shutdown()
            {
                foreach (SecondarySoundBuffer b in soundBuffers)
                {
                    b.Dispose();
                }
                soundBuffers.Clear();
                device.Dispose();
            }

            /// <summary>
            /// Create a playable sound buffer from a WAV file
            /// </summary>
            /// <param name="Filename">The WAV file to load</param>
            /// <returns>Playable sound buffer</returns>
            public static SecondarySoundBuffer CreateSoundBufferFromWave(string Filename)
            {
                // Load wave file
                using (WaveStream waveFile = new WaveStream(Filename))
                {
                    SoundBufferDescription description = new SoundBufferDescription();
                    description.Format = waveFile.Format;
                    description.SizeInBytes = (int)waveFile.Length;
                    description.Flags = BufferFlags.ControlVolume | BufferFlags.GlobalFocus | BufferFlags.ControlPositionNotify;

                    // Create the buffer.
                    SecondarySoundBuffer buffer = new SecondarySoundBuffer(device, description);
                    byte[] data = new byte[description.SizeInBytes];
                    waveFile.Read(data, 0, (int)waveFile.Length);
                    buffer.Write(data, 0, LockFlags.None);

                    soundBuffers.Add(buffer);

                    return buffer;
                }
            }
        }
    }

