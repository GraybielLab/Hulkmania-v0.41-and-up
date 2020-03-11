//using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Brandeis.AGSOL.Hulkamania.Tasks.ModeledControllerWithHmd
{
    /// <summary>
    /// Reads trial parameters and maintains the list of balance trials.
    /// </summary>
    internal static class Trials
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly List<Trial> list;

        private static String protocolFilename;

        #endregion

        #region Fields

        private static int currentTrialIndex;

        #endregion

        #region Properties

        /// <summary>
        /// The trial that is scheduled to run (if idle) or is currently being run.
        /// </summary>
        internal static Trial CurrentTrial
        {
            get {
                if ((list == null) || (list.Count == 0)) {
                    return null;
                }
                return list[currentTrialIndex];
            }
        }

        /// <summary>
        /// Index of the trial that is scheduled to run (if idle) or is currently being run.
        /// </summary>
        internal static int CurrentTrialIndex
        {
            get {
                return currentTrialIndex;
            }
            set {
                currentTrialIndex = value;
            }
        }

        /// <summary>
        /// The list of trials.
        /// </summary>
        internal static List<Trial> List
        {
            get {
                return list;
            }
        }

        /// <summary>
        /// The trial that was previously run.
        /// </summary>
        internal static Trial PreviousTrial
        {
            get {
                if (currentTrialIndex > 0) {
                    return list[currentTrialIndex - 1];
                }
                return null;
            }
        }

        /// <summary>
        /// The name of the protocol file that was read to produce this list of trials.
        /// </summary>
        internal static String ProtocolFilename
        {
            get {
                return protocolFilename;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates the dynamics simulator.
        /// </summary>
        static Trials()
        {
            logger.Debug("Static create: Trials");

            list = new List<Trial>();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Create a handler for the given trial parameters file.
        /// </summary>
        /// <param name="filename">Filename of the CSV file containing the trial parameters</param>
        internal static bool Read(String filename)
        {
            CsvReader csvReader = new CsvReader();
            csvReader.readFile(filename);

            bool readSuccessful = true;
            list.Clear();
            for (int i = 0; i < csvReader.NumLinesRead; i++)
            {
                Trial t = new Trial(csvReader, i);
                if (t.SetNoiseProfile())
                {
                    list.Add(t);
                }
                else
                {
                    readSuccessful = false;
                }
            }

            if (readSuccessful)
            {
                String filenameWithoutDirectory;
                filenameWithoutDirectory = filename.Substring(filename.LastIndexOf('\\'));
                protocolFilename = filenameWithoutDirectory.Substring(1, filenameWithoutDirectory.Length - 5);
            }
            else
            {
                list.Clear();
            }

            return readSuccessful;
        }


        #endregion
    }
}
