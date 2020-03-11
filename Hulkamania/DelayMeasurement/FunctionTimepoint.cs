using System;
using System.IO;

namespace Brandeis.AGSOL.Hulkamania.Tasks.DelayMeasurement
{
    /// <summary>
    /// The values of the forcing function at one time point.
    /// </summary>
    internal sealed class FunctionTimepoint
    {
        #region Fields

        private RotationAngles input;

        private double time;

        #endregion

        #region Properties

        /// <summary>
        /// The input.
        /// </summary>
        internal RotationAngles Input
        {
            get {
                return input;
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

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an empty function timepoint.
        /// </summary>
        internal FunctionTimepoint()
        {
            input = new RotationAngles();
        }

        /// <summary>
        /// Creates the function timepoint from a single line of a CSV file.
        /// </summary>
        /// <param name="line">The CSV file line</param>
        internal FunctionTimepoint(String line)
        {
            String[] param;

            input = new RotationAngles();

            param = line.Split(',');

            if (param.Length != 4) {
                throw new IOException("Incorrect number of values in function timepoint input string.");
            }

            try {
                Double.TryParse(param[0], out time);
                Double.TryParse(param[1], out input.yaw);
                Double.TryParse(param[2], out input.pitch);
                Double.TryParse(param[3], out input.roll);
            } catch (Exception) {
                throw new IOException("Parsing failed for function timepoint input string.");
            }
        }

        #endregion
    }
}
