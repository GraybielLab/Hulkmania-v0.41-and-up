using System;
using System.IO;

namespace Brandeis.AGSOL.Hulkamania.Tasks.PassiveMovement
{
    /// <summary>
    /// The values of the recording at one time point.
    /// </summary>
    internal sealed class RecordingTimepoint
    {
        #region Fields

        private RotationAngles angles;
        private RotationAngles directionOfBalance;
        private RotationAngles movingDirectionOfBalance;

        private double time;

        private int trialNumber;
        private int trialPhase;

        #endregion

        #region Properties

        /// <summary>
        /// The recorded angles at this timepoint.
        /// </summary>
        internal RotationAngles Angles
        {
            get {
                return angles;
            }
        }

        /// <summary>
        /// The recorded direction of balance at this timepoint.
        /// </summary>
        internal RotationAngles DirectionOfBalance
        {
            get {
                return directionOfBalance;
            }
        }

        /// <summary>
        /// The recorded moving direction of balance at this timepoint.
        /// </summary>
        internal RotationAngles MovingDirectionOfBalance
        {
            get {
                return movingDirectionOfBalance;
            }
        }

        /// <summary>
        /// The time (in seconds after trial start) at which this motion should occur.
        /// </summary>
        internal double Time
        {
            get {
                return time;
            }
            set {
                time = value;
            }
        }

        /// <summary>
        /// The number of the original trial.
        /// </summary>
        internal int TrialNumber
        {
            get {
                return trialNumber;
            }
            set {
                trialNumber = value;
            }
        }

        /// <summary>
        /// The phase of the original trial at this timepoint.
        /// </summary>
        internal int TrialPhase
        {
            get {
                return trialPhase;
            }
            set {
                trialPhase = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an empty recording timepoint.
        /// </summary>
        internal RecordingTimepoint()
        {
            angles = new RotationAngles();
            directionOfBalance = new RotationAngles();
            movingDirectionOfBalance = new RotationAngles();
        }

        /// <summary>
        /// Creates the recording timepoint from a single line of a CSV file.
        /// </summary>
        /// <param name="line">The CSV file line</param>
        internal RecordingTimepoint(String line)
        {
            String[] param;

            angles = new RotationAngles();
            directionOfBalance = new RotationAngles();
            movingDirectionOfBalance = new RotationAngles();

            param = line.Split(',');

            if (param.Length < 12) {
                throw new IOException("Incorrect number of values in recording timepoint string.");
            }

            try {
                Double.TryParse(param[0], out time);
                Int32.TryParse(param[1], out trialNumber);
                Int32.TryParse(param[2], out trialPhase);
                Double.TryParse(param[3], out directionOfBalance.roll);
                Double.TryParse(param[4], out directionOfBalance.pitch);
                Double.TryParse(param[5], out directionOfBalance.yaw);
                Double.TryParse(param[6], out movingDirectionOfBalance.roll);
                Double.TryParse(param[7], out movingDirectionOfBalance.pitch);
                Double.TryParse(param[8], out movingDirectionOfBalance.yaw);
                Double.TryParse(param[9], out angles.roll);
                Double.TryParse(param[10], out angles.pitch);
                Double.TryParse(param[11], out angles.yaw);
             } catch (Exception) {
                throw new IOException("Parsing failed for recording timepoint string.");
            }
        }

        #endregion
    }
}
