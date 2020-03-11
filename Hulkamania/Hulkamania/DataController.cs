using System;
using System.Text;
using System.Configuration;

namespace Brandeis.AGSOL.Hulkamania
{
    abstract class DataController
    {
        #region Fields
        private static DataController instance = null;
        #endregion

        #region Singleton
        internal static DataController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = AppMain.UseDummyMotionController ? (new DataControllerDummy() as DataController) : (new DataControllerHulk() as DataController);
                }
                return instance;
            }
        }
        #endregion


        #region Properties

        /// <summary>
        ///  True if the safety light is green.
        /// </summary>
        internal virtual bool IsLightGreen
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// True if the mayday button on the HULK has been pressed.
        /// </summary>
        internal virtual bool IsMaydayPressed
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// True if the PC is ready to send commands.
        /// Should be set to true only after Hulkamania is fully initialized.
        /// </summary>
        internal virtual bool PCReady
        {
            get
            {
                return true;
            }
            set
            {

            }
        }

        /// <summary>
        /// True if the PLC board reports that it is ready to send and receive commands.
        /// </summary>
        internal virtual bool PLCReady
        {
            get
            {
                return true;
            }
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Starts the data I/O tasks.
        /// </summary>
        public abstract void StartTasks();

        /// <summary>
        /// Stops the data I/O tasks.
        /// </summary>
        public abstract void StopTasks();

        #endregion
  
    }
}
