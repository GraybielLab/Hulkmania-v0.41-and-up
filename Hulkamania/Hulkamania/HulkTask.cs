using Brandeis.AGSOL.Network;
using System.Net.Sockets;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// An experimenter-defined task to run on the Hulk.
    /// </summary>
    public class HulkTask
    {
        #region Fields

        protected HulkTaskPanel panel;

        #endregion

        #region Delegates

        public delegate void AxesCentersChangedHandler(double inner, double outer);

        #endregion

        #region Events
        
        public event AxesCentersChangedHandler AxesCentersChanged;
        
        #endregion

        #region Properties
        /// <summary>
        /// Any displayable value that is associated with the task and the inner axis.
        /// </summary>
        public virtual DisplayableData DataInnerAxis
        {
            get {
                return null;
            }
        }

        /// <summary>
        /// Any displayable value that is associated with the task and the outer axis.
        /// </summary>
        public virtual DisplayableData DataOuterAxis
        {
            get {
                return null;
            }
        }

        /// <summary>
        /// Called from the main control loop whenever the task is running.
        /// </summary>
        public HulkTaskPanel TaskPanel
        {
            get {
                return panel;
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Called whenever a network command is received that the task should deal with
        /// </summary>
        public virtual void ProcessNetworkCommand(ICommand c, INetworkServer server, Socket handler)
        {
        }

        /// <summary>
        /// Called from the main control loop regardless of whether a task is running or not. Send data to network clients in this method
        /// </summary>
        public virtual void UpdateNetworkClients()
        {

        }

        /// <summary>
        /// Called from the main control loop whenever the task is running.
        /// </summary>
        public virtual void ContinueTask()
        {
        }

        /// <summary>
        /// Called from the main control loop, even if the task is not running.
        /// Allows the task panel to log data during preparatory / cleanup activities
        /// before or after a task is run.
        /// </summary>
        public virtual void LogBasicData()
        {
        }

        /// <summary>
        /// Called from the main control loop whenever the task should be stopped.
        /// </summary>
        public virtual void StopTask()
        {
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Informs listeners about where the center of axes should be for the current trial
        /// </summary>
        protected virtual void SendAxesCentersChangedEvent(double innerCenter, double outerCenter)
        {
            AxesCentersChangedHandler handler = AxesCentersChanged;
            if (handler != null)
            {
                 handler(innerCenter, outerCenter);
            }
        }
        #endregion

    }
}
