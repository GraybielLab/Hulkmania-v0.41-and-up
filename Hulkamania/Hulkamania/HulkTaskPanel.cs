using System;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// Panel that contains the GUI for an experimental task.
    /// </summary>
    public class HulkTaskPanel : Panel
    {
        #region Delegates

        protected delegate void EventHandlerDelegate();

        #endregion


        #region Fields

        protected HulkTask task;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Updates the GUI with the current information from the motion controller.
        /// </summary>
        public virtual void updateTimer_Tick()
        {
        }

        #endregion
    }
}
