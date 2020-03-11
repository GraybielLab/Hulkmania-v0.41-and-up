using log4net;
using NationalInstruments.DAQmx;
using System.Configuration;
using System.Reflection;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// The data acquisition board.
    /// </summary>
    internal class DataControllerHulk : DataController
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DigitalMultiChannelReader checkLightsReader;
        private readonly DigitalSingleChannelReader checkPLCReader;
        private readonly DigitalMultiChannelReader checkMaydayReader;
        private readonly DigitalSingleChannelWriter pcReadyWriter;
        
        private readonly Task checkLightsTask;
        private readonly Task checkPLCTask;
        private readonly Task checkMaydayTask;
        private readonly Task pcReadyTask;

        #endregion

        #region Fields

        private bool pcReady;

        #endregion

        #region Properties
        
        /// <summary>
        ///  True if the safety light is green.
        /// </summary>
        internal override bool IsLightGreen
        {
            get {

                bool[] checks;

                checks = checkLightsReader.ReadSingleSampleSingleLine();
                if (checks.Length == 2) {
                    return !(checks[0] & checks[1]);
                }

                return false;
            }
        }

        /// <summary>
        /// True if the mayday button on the HULK has been pressed.
        /// </summary>
        internal override bool IsMaydayPressed 
        {
            get {
                bool[] checks;

                checks = checkMaydayReader.ReadSingleSampleSingleLine();
                if (checks.Length == 2) {
                    return (checks[0] || checks[1]);
                }
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
                return pcReady;
            }
            set {
                pcReadyWriter.WriteSingleSampleSingleLine(true, value);
                pcReady = value;
            }
        }

        /// <summary>
        /// True if the PLC board reports that it is ready to send and receive commands.
        /// </summary>
        internal override bool PLCReady
        {
            get {
                bool check;

                check = checkPLCReader.ReadSingleSampleSingleLine();
                return check;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the data reader board.
        /// </summary>
        internal DataControllerHulk()
        {
            logger.Info("Create: DataControllerHulk");

            // Create a task to check whether the PLC is OK

            checkPLCTask = new Task("CheckPLCTask");
            checkPLCTask.DIChannels.CreateChannel(ConfigurationManager.AppSettings["InputLineCheckPLC"],
                "checkPLC", ChannelLineGrouping.OneChannelForEachLine);
            checkPLCTask.Timing.SampleTimingType = SampleTimingType.OnDemand;
            checkPLCTask.Timing.SampleQuantityMode = SampleQuantityMode.ContinuousSamples;

            checkPLCReader = new DigitalSingleChannelReader(checkPLCTask.Stream);

            // Create a task to check the status of the safety light

            checkLightsTask = new Task("CheckLightsTask");
            checkLightsTask.DIChannels.CreateChannel(ConfigurationManager.AppSettings["InputLineCheckLights1"],
                "checkLights1", ChannelLineGrouping.OneChannelForEachLine);
            checkLightsTask.DIChannels.CreateChannel(ConfigurationManager.AppSettings["InputLineCheckLights2"],
                "checkLights2", ChannelLineGrouping.OneChannelForEachLine);
            checkLightsTask.Timing.SampleTimingType = SampleTimingType.OnDemand;
            checkLightsTask.Timing.SampleQuantityMode = SampleQuantityMode.ContinuousSamples;

            checkLightsReader = new DigitalMultiChannelReader(checkLightsTask.Stream);

            // Create a task to check for MAYDAY signals

            checkMaydayTask = new Task("CheckMaydayTask");
            checkMaydayTask.DIChannels.CreateChannel(ConfigurationManager.AppSettings["InputLineCheckMayday1"],
                "checkMayday1", ChannelLineGrouping.OneChannelForEachLine);
            checkMaydayTask.DIChannels.CreateChannel(ConfigurationManager.AppSettings["InputLineCheckMayday2"],
                "checkMayday2", ChannelLineGrouping.OneChannelForEachLine);
            checkMaydayTask.Timing.SampleTimingType = SampleTimingType.OnDemand;
            checkMaydayTask.Timing.SampleQuantityMode = SampleQuantityMode.ContinuousSamples;

            checkMaydayReader = new DigitalMultiChannelReader(checkMaydayTask.Stream);

            // Create a task for sending 'PC Ready' signals

            pcReadyTask = new Task("PCReadyTask");
            pcReadyTask.DOChannels.CreateChannel(ConfigurationManager.AppSettings["OutputLinePCReady"],
                "pcReady", ChannelLineGrouping.OneChannelForEachLine);

            pcReadyWriter = new DigitalSingleChannelWriter(pcReadyTask.Stream);
            
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

            //checkLightsTask.Start();
            checkPLCTask.Start();
            checkMaydayTask.Start();
            pcReadyTask.Start();

            logger.Info("Data controller tasks started.");
        }
        
        /// <summary>
        /// Stops the data I/O tasks.
        /// </summary>
        public override void StopTasks()
        {
            logger.Debug("Enter: StopTasks");

            //checkLightsTask.Stop();
            checkPLCTask.Stop();
            checkMaydayTask.Stop();
            pcReadyTask.Stop();

            logger.Info("Data controller tasks stopped.");
        }

        #endregion
    }
}
