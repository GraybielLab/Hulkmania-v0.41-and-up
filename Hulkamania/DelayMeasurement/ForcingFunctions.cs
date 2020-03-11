using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Brandeis.AGSOL.Hulkamania.Tasks.DelayMeasurement
{
    /// <summary>
    /// Reads function timepoints and provides interpolated function values.
    /// </summary>
    internal static class ForcingFunctions
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly List<FunctionTimepoint> list;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates the list.
        /// </summary>
        static ForcingFunctions()
        {
            logger.Debug("Static create: ForcingFunctions");

            list = new List<FunctionTimepoint>();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Clears the forcing function data.
        /// </summary>
        internal static void Clear()
        {
            logger.Debug("Enter: Clear()");

            list.Clear();
        }

        /// <summary>
        /// Returns the value of the function at a given time.
        /// </summary>
        /// <param name="trialNumber">The trial number (a function must be defined for this trial number)</param>
        /// <param name="secondsIntoTrial">The value is calculated for this number of seconds into the trial</param>
        internal static FunctionTimepoint GetForcingFunction(double secondsIntoTrial)
        {
            FunctionTimepoint lower;
            FunctionTimepoint upper;
            FunctionTimepoint result;

            // If no forcing function was loaded, return empty timepoint
            if (list.Count == 0) {
                
                result = new FunctionTimepoint();
                result.Input.pitch = 0;
                result.Input.roll = 0;
                result.Input.yaw = 0;
                result.Time = 0;

                return result;
            }

            // If there is only one timepoint, return it
            if (list.Count == 1) {
                return list[0];
            }

            // If the forcing function data is requested for a time in the past, return the first function timepoint
            if (secondsIntoTrial <= list[0].Time) {
                return list[0];
            }

            // If the forcing function data is requested for a time in the future, return the last function timepoint
            if (secondsIntoTrial >= list[list.Count - 1].Time) {
                return list[list.Count - 1];
            }

            // Find the two timepoints to interpolate between
            lower = null;
            upper = null;

            for (int i = 0; i < list.Count; i++) {
                
                if ((list[i].Time > secondsIntoTrial) ||
                    (list[i + 1].Time < secondsIntoTrial)) {
                    continue;
                }

                lower = list[i];
                upper = list[i + 1];
                break;
            }

            if ((lower == null) || (upper == null)) {
                
                result = new FunctionTimepoint();
                result.Input.pitch = 0;
                result.Input.roll = 0;
                result.Input.yaw = 0;
                result.Time = 0;

                return result;
            }

            // Interpolate
            result = new FunctionTimepoint {
                Time = secondsIntoTrial,
                Input = {
                    roll = Interpolate(lower.Input.roll, upper.Input.roll,
                        lower.Time, upper.Time, secondsIntoTrial),
                    pitch = Interpolate(lower.Input.pitch, upper.Input.pitch,
                        lower.Time, upper.Time, secondsIntoTrial),
                    yaw = Interpolate(lower.Input.yaw, upper.Input.yaw,
                        lower.Time, upper.Time, secondsIntoTrial)
                }
            };

            return result;
        }

        /// <summary>
        /// Returns the maximum time point for which a forcing function value is defined.
        /// </summary>
        internal static double GetForcingFunctionMaxTime()
        {
            if (list.Count == 0) {
                return 0;
            }

            return list[list.Count - 1].Time;
        }

        /// <summary>
        /// Checks whether there is a forcing function.
        /// </summary>
        /// <returns>True if there is a forcing function</returns>
        internal static bool HasFunction()
        {
            return (list.Count != 0);
        }

        /// <summary>
        /// Create a handler for the given function timepoints file.
        /// </summary>
        /// <param name="filename">Filename of the CSV file containing the function timepoints</param>
        internal static void Read(String filename)
        {
            FunctionTimepoint timepoint;
            StreamReader reader;
            String line;

            logger.Debug("Enter: Read(String)");

            // Read in the file

            if (!File.Exists(filename)) {
                logger.Debug("Forcing function file " + filename + " not found.");
                return;
            }

            logger.Debug("Reading forcing function file: " + filename);

            reader = new StreamReader(filename);

            while ((line = reader.ReadLine()) != null) {

                // Ignore comment lines
                if (line.StartsWith(";")) {
                    continue;
                }
                if (line.StartsWith("s")) {
                    continue;
                }

                // Parse the line into a set of function timepoints
                timepoint = new FunctionTimepoint(line);
                list.Add(timepoint);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Linear interpolation between two values.
        /// </summary>
        /// <param name="lower">The value at the lower time</param>
        /// <param name="upper">The value at the higher time</param>
        /// <param name="lowerTime">The lower time</param>
        /// <param name="upperTime">The higher time</param>
        /// <param name="currentTime">The current time (in between lowerTime and higherTime)</param>
        /// <returns>The interpolated value</returns>
        private static double Interpolate(double lower, double upper, double lowerTime, double upperTime, double currentTime)
        {
            return lower + (currentTime - lowerTime) / (upperTime - lowerTime) * (upper - lower);
        }

        #endregion
    }
}
