using log4net;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// A panel that contains two horizontal axis graphs, one each for outer and inner axes.
    /// </summary>
    internal sealed class GraphPanel : Panel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);
        
        #endregion

        #region Fields

        private HorizontalAxisGraph innerGraph;
        private HorizontalAxisGraph outerGraph;
        private JoystickGraph joystickGraph;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public GraphPanel()
        {
            logger.Debug("Create: GraphPanel");

            InitializeGUI();
        }

        #endregion

        #region GUI Creation

        /// <summary>
        /// Constructs the panel GUI.
        /// </summary>
        private void InitializeGUI()
        {
            logger.Debug("Enter: InitializeGUI()");

            SuspendLayout();

            innerGraph = new HorizontalAxisGraph {
                InnerAxis = true
            };

            outerGraph = new HorizontalAxisGraph {
                InnerAxis = false
            };

            joystickGraph = new JoystickGraph();

            Controls.Add(innerGraph);
            Controls.Add(outerGraph);
            Controls.Add(joystickGraph);

            Layout += GraphPanel_Layout;
            
            ResumeLayout();
        }

        #endregion

        #region Event Handlers
        /// <summary>
        /// Informs the inner and outer axis graphs on where the center of the scale should be
        /// </summary>
        /// <param name="inner">The center for the inner axis graph</param>
        /// <param name="outer">The center for the outer axis graph</param>
        public void axisCenters_Changed(double inner, double outer)
        {
            innerGraph.AxisCenter = inner;
            outerGraph.AxisCenter = outer;
        }

        /// <summary>
        /// Updates the GUI with the current information from the motion controller.
        /// </summary>
        public void updateTimer_Tick()
        {
            innerGraph.updateTimer_Tick();
            outerGraph.updateTimer_Tick();
            joystickGraph.updateTimer_Tick(InputController.JoystickInputRaw.x, InputController.JoystickInputRaw.y, InputController.JoystickInputRaw.trigger);

            Refresh();
        }
        
        /// <summary>
        /// Called when the panel is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void GraphPanel_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: GraphPanel_Layout(object, EventArgs)");

            outerGraph.Location = new Point(0, 0);
            outerGraph.Size = new Size(Width, Height / 3);

            innerGraph.Location = new Point(0, Height / 3);
            innerGraph.Size = new Size(Width, Height / 3);

            joystickGraph.Location = new Point(0, 2 * Height / 3);
            joystickGraph.Size = new Size(Width, Height / 3);
        }

        #endregion
    }
}
