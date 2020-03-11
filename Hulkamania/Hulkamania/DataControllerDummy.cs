using log4net;
using NationalInstruments.DAQmx;
using System.Configuration;
using System.Reflection;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// The data acquisition board.
    /// </summary>
    internal class DataControllerDummy : DataController
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private static bool pcReady;

        #endregion

        #region Properties
        
        /// <summary>
        ///  True if the safety light is green.
        /// </summary>
        internal override bool IsLightGreen
        {
            get {
                return false;
            }
        }

        /// <summary>
        /// True if the mayday button on the HULK has been pressed.
        /// </summary>
        internal override bool IsMaydayPressed 
        {
            get {
                return false;
            }
        }

        /// <summary>
        /// True if the PC is ready to send commands.
        /// Should be set to true only after Hulkamania is fully initialized.
        /// </summary>
        internal override bool PCReady 
        {
            get {
                return true;
            }
            set {
                pcReady = value;
            }
        }

        /// <summary>
        /// True if the PLC board reports that it is ready to send and receive commands.
        /// </summary>
        internal override bool PLCReady
        {
            get {
                return true;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the data reader board.
        /// </summary>
        static DataControllerDummy()
        {
             logger.Debug("Create: DataControllerDummy");
             logger.Info("Data tasks initialized.");
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// Starts the data I/O tasks.
        /// </summary>
        public override void StartTasks()
        {
             logger.Debug("Enter: StartTasks");
             logger.Info("Data controller tasks started.");
        }
        
        /// <summary>
        /// Stops the data I/O tasks.
        /// </summary>
        public override void StopTasks()
        {
             logger.Debug("Enter: StopTasks");
             logger.Info("Data controller tasks stopped.");
        }

        #endregion
    }
}
