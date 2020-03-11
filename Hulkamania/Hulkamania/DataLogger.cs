using log4net;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// Logs experimental data.
    /// </summary>
    public static class DataLogger
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private static StreamWriter logWriter;

        private static StringBuilder data;
       
        #endregion

        #region Properties

        /// <summary>
        /// Returns true if the data logger is ready to use.
        /// </summary>
        public static bool Ready
        {
            get {
                return (logWriter != null);
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Performs a check if the required output folders exist, and logs an error message if they dont
        /// </summary>
        /// <returns>True if all required folders exist, false if one or more required folders does not exist</returns>
        public static bool CheckIfRequiredFoldersExist(bool writeStatusTolog)
        {
            // Set up the log file for the results
            if (!Directory.Exists(ConfigurationManager.AppSettings["DataDirectory"])) {
                try {
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["DataDirectory"]);
                } catch (Exception e) {
                    logger.Error("[DataLogger] Error creating directory: " + ConfigurationManager.AppSettings["DataDirectory"]);
                    logger.Error("[DataLogger] Error message: " + e.Message);
                    return false;
                }
            }

            if (writeStatusTolog) {
                logger.Info("Saving data to the following location: " + ConfigurationManager.AppSettings["DataDirectory"]);
            }
            return true;
        }

        /// <summary>
        /// Acquires the data log resources.
        /// </summary>
        /// <param name="logName">The log filename</param>
        /// <param name="header">The header of the data file. Usually contains text descriptions of columns.</param>
        public static void AcquireDataLog(String logName, String header)
        {
             logger.Debug("Enter: AcquireDataLog()");

            // Flush if the log was not closed correctly last time.
            if (logWriter != null) {
                if (data != null) {
                    logWriter.Write(data);
                }
                logWriter.Flush();
                logWriter.Close();
            }

            // Set up data logging for approximately 30 seconds of data.
            // More will be allocated if required.
            data = new StringBuilder(5242880);

            // Add the column headers to the data file
            data.Append(header);

            // make sure that the required data output folder exists
            CheckIfRequiredFoldersExist(false);

            try
            {
                logWriter = new StreamWriter(ConfigurationManager.AppSettings["DataDirectory"] + logName);
            }
            catch (Exception e)
            {
                logger.Error("[DataLogger] Error creating file: " + ConfigurationManager.AppSettings["DataDirectory"] + logName);
                logger.Error("[DataLogger] Error message: " + e.Message);
            }
            
        }

        /// <summary>
        /// Appends data to the ongoing log of a trial. The log will be written to a file at the end of the trial.
        /// </summary>
        /// <param name="logMessage">The log message to append</param>
        public static void AppendData(String logMessage)
        {
            if (data != null)
            {
                data.Append(logMessage);
            }
        }

        /// <summary>
        /// Closes the data log. Called at the end of a trial.
        /// </summary>
        public static void CloseDataLog()
        {
             logger.Debug("Enter: CloseDataLog()");

            // Finish logging the trial data
            if (data == null) {
                return;
            }

            // Note: logWrite == null can happen when no trial is currently active (e.g. all trials in protocol has been completed)
            // and a new protocol is loaded. This triggers a CloseDataLog() call without a matching call to AcquireDataLog. 
            // When this happens no error message should be generated here. Furthermore if AcquireDataLog fails an error is generated, making this one
            // redundant.
            if (logWriter == null) {
                return;
            }

            try
            {
                logWriter.Write(data);
                logWriter.Flush();
                logger.Info("Written data to file: " + ((FileStream)(logWriter.BaseStream)).Name);
                logWriter.Close();
                logWriter = null;
            }
            catch (IOException ex)
            {
                logger.Error("DataLogger: error writing data to file: " + ((FileStream)(logWriter.BaseStream)).Name);
                logger.Error("DataLogger: description: " + ex.Message);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Releases the data log resources. Usually called on application exit. 
        /// </summary>
        internal static void ReleaseDataLog()
        {
            logger.Debug("Enter: ReleaseDataLog()");

            if (logWriter == null) {
                return;
            }

            logWriter.Close();
            logWriter.Dispose();
        }

        #endregion
    }
}
