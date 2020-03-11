using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Brandeis.AGSOL.Hulkamania.Tasks.PassiveMovement
{
    /// <summary>
    /// Reads recording timepoints and provides interpolated values.
    /// </summary>
    internal static class Recordings
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Dictionary<int, int> trialStartIndices;

        private static readonly List<RecordingTimepoint> list;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates the list.
        /// </summary>
        static Recordings()
        {
            logger.Debug("Static create: Recordings");

            list = new List<RecordingTimepoint>();
            trialStartIndices = new Dictionary<int, int>();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Clears the recording data.
        /// </summary>
        internal static void Clear()
        {
            logger.Debug("Enter: Clear()");

            list.Clear();
            trialStartIndices.Clear();
        }

        /// <summary>
        /// Checks whether a given time is included in a series of trial recordings.
        /// </summary>
        /// <param name="trialNumber">The trial to check</param>
        /// <param name="secondsIntoTrial">The time to check, in seconds</param>
        /// <returns>True if the given time is at or past the end of the entire series of trials</returns>
        internal static bool IsEndOfRecordingSeries(int trialNumber, double secondsIntoTrial)
        {
            if (!trialStartIndices.ContainsKey(trialNumber + 1)) {
                if (secondsIntoTrial >= list[list.Count - 1].Time) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether a given time is included in a given trial's recording.
        /// </summary>
        /// <param name="trialNumber">The trial to check</param>
        /// <param name="secondsIntoTrial">The time to check, in seconds</param>
        /// <returns>True if the given time is at or past the end of the given trial</returns>
        internal static bool IsEndOfRecordingTrial(int trialNumber, double secondsIntoTrial)
        {
            if (trialStartIndices.ContainsKey(trialNumber + 1)) {
                return (secondsIntoTrial >= list[trialStartIndices[trialNumber + 1] - 1].Time);
            }

            return (secondsIntoTrial >= list[list.Count - 1].Time);
        }

        /// <summary>
        /// Returns the trial number of the first trial in a series.
        /// </summary>
        /// <returns>The trial number</returns>
        internal static int GetFirstTrialNumber()
        {
            if (list.Count > 0) {
                return list[0].TrialNumber;
            }
            return -1;
        }

        /// <summary>
        /// Returns the value of the recording at a given time.
        /// </summary>
        /// <param name="trialNumber">The trial number to search data for</param>
        /// <param name="secondsIntoTrial">The value is calculated for this number of seconds into the trial</param>
        internal static RecordingTimepoint GetRecording(int trialNumber, double secondsIntoTrial)
        {
            RecordingTimepoint lower;
            RecordingTimepoint upper;
            RecordingTimepoint result;
            int trialStartIndex;
            int trialEndIndex;
            
            // Get the starting index.
            if (trialStartIndices.ContainsKey(trialNumber)) {
                trialStartIndex = trialStartIndices[trialNumber];
            } else {
                return new RecordingTimepoint();
            }

            // Get the ending index for the trial.
            if (trialStartIndices.ContainsKey(trialNumber + 1)) {
                trialEndIndex = trialStartIndices[trialNumber + 1] - 1;
            } else {
                trialEndIndex = list.Count - 1;
            }

            // If there is only one timepoint, return it
            if (trialStartIndex == trialEndIndex) {
                return list[trialStartIndex];
            }

            // If the recording data is requested for a time in the past, return the recording timepoint
            if (secondsIntoTrial <= list[trialStartIndex].Time)  {
                return list[trialStartIndex];
            }

            // If the recording data is requested for a time in the future, return the last recording timepoint
            if (secondsIntoTrial >= list[trialEndIndex].Time) {
                return list[trialEndIndex];
            }

            // Find the two timepoints to interpolate between
            
            lower = null;
            upper = null;
            for (int i = trialStartIndex; i <= trialEndIndex; i++) {

                if ((list[i].Time > secondsIntoTrial) || (list[i + 1].Time < secondsIntoTrial)) {
                    continue;
                }

                lower = list[i];
                upper = list[i + 1];
                break;
            }

            if ((lower == null) || (upper == null)) {

                // Something went wrong. Return an empty timepoint.
                return new RecordingTimepoint();
            }

            // Interpolate
            result = new RecordingTimepoint();
            result.Angles.pitch = Interpolate(lower.Angles.pitch, upper.Angles.pitch, lower.Time, upper.Time, secondsIntoTrial);
            result.Angles.roll = Interpolate(lower.Angles.roll, upper.Angles.roll, lower.Time, upper.Time, secondsIntoTrial);
            result.Angles.yaw = Interpolate(lower.Angles.yaw, upper.Angles.yaw, lower.Time, upper.Time, secondsIntoTrial);
            result.DirectionOfBalance.pitch = lower.DirectionOfBalance.pitch;
            result.DirectionOfBalance.roll = lower.DirectionOfBalance.roll;
            result.DirectionOfBalance.yaw = lower.DirectionOfBalance.yaw;
            result.MovingDirectionOfBalance.pitch = lower.MovingDirectionOfBalance.pitch;
            result.MovingDirectionOfBalance.roll = lower.MovingDirectionOfBalance.roll;
            result.MovingDirectionOfBalance.yaw = lower.MovingDirectionOfBalance.yaw;
            result.TrialNumber = lower.TrialNumber;
            result.TrialPhase = lower.TrialPhase;
            result.Time = secondsIntoTrial;

            return result;
        }

        /// <summary>
        /// Checks whether there is a recording available.
        /// </summary>
        /// <returns>True if a recording has been loaded</returns>
        internal static bool HasRecording()
        {
            return (list.Count != 0);
        }

        /// <summary>
        /// Create a handler for the given recording timepoints file.
        /// </summary>
        /// <param name="filename">Filename of the CSV file containing the recording timepoints</param>
        internal static void Read(String filename)
        {
            RecordingTimepoint timepoint;
            StreamReader reader;
            String line;

            logger.Debug("Enter: Read(String)");

            // Read in the file

            if (!File.Exists(filename)) {
                logger.Debug("Recording file " + filename + " not found.");
                return;
            }

            logger.Debug("Reading recording file: " + filename);

            reader = new StreamReader(filename);

            while ((line = reader.ReadLine()) != null)  {

                // Ignore comment lines
                if (line.StartsWith(";")) {
                    continue;
                }
                if (line.StartsWith("s")) {
                    continue;
                }

                // Parse the line into a set of recording timepoints
                timepoint = new RecordingTimepoint(line);
                if (!trialStartIndices.ContainsKey(timepoint.TrialNumber)) {
                    trialStartIndices[timepoint.TrialNumber] = list.Count;
                }
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
