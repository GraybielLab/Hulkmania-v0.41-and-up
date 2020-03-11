using log4net;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania
{
    public sealed class NamedPanel : Panel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Panel childPanel;

        private readonly Pen colorPen;

        private readonly SolidBrush colorBrush;

        #endregion

        #region Fields

        private String childPanelName;

        #endregion

        #region Properties

        /// <summary>
        /// The child panel nested inside this panel.
        /// </summary>
        internal Panel ChildPanel
        {
            get {
                return childPanel;
            }
        }

        /// <summary>
        /// The name of the child panel.
        /// </summary>
        public String ChildPanelName
        {
            set {
                childPanelName = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a panel with a visible name that contains the given child panel.
        /// </summary>
        /// <param name="panel">The panel to display</param>
        /// <param name="name">The name of the child panel</param>
        /// <param name="nameColor">The background color of the name bar</param>
        internal NamedPanel(Panel panel, String name, Color nameColor)
        {
            logger.Debug("Create: NamedPanel");

            childPanel = panel;
            childPanelName = name;

            colorBrush = new SolidBrush(nameColor);
            colorPen = new Pen(colorBrush);

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
            
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            childPanel.BackColor = ColorScheme.Instance.PanelBackground;

            Controls.Add(childPanel);

            Layout += NamedPanel_Layout;
            Paint += NamedPanel_Paint;

            ResumeLayout();
        }

        #endregion

        #region Event Handlers
        
        /// <summary>
        /// Called when the panel is repainted.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void NamedPanel_Paint(object sender, PaintEventArgs e)
        {
            Pen edgePen;
            Pen fillPen;
            Pen titlePen;

            edgePen = new Pen(ColorScheme.Instance.PanelEdge);
            fillPen = new Pen(ColorScheme.Instance.PanelBackground);
            titlePen = new Pen(ColorScheme.Instance.PanelTitleBar);

            // Left line
            e.Graphics.DrawLine(edgePen, Margin.Left, Margin.Top + 5, 
                Margin.Left, Height - Margin.Bottom - 5);

            // Right line
            e.Graphics.DrawLine(edgePen, Width - Margin.Right - 1, Margin.Top + 5, 
                Width - Margin.Right - 1, Height - Margin.Bottom - 5);

            // Top line
            e.Graphics.DrawLine(edgePen, Margin.Left + 5, Margin.Top, 
                Width - Margin.Right - 5, Margin.Top);

            // Bottom line
            e.Graphics.DrawLine(edgePen, Margin.Left + 5, Height - Margin.Bottom - 1,
                Width - Margin.Right - 5, Height - Margin.Bottom - 1);

            // Top left arc
            e.Graphics.DrawArc(edgePen, Margin.Left, Margin.Top, 10, 10, 180, 90);

            // Top right arc
            e.Graphics.DrawArc(edgePen, Width - Margin.Right - 11, Margin.Top, 10, 10, 270, 90);

            // Bottom left arc
            e.Graphics.DrawArc(edgePen, Margin.Left, Height - Margin.Bottom - 11, 10, 10, 90, 90);

            // Bottom right arc
            e.Graphics.DrawArc(edgePen, Width - Margin.Right - 11, Height - Margin.Bottom - 11, 10, 10, 0, 90);

            // Left fill line
            e.Graphics.DrawLine(fillPen, Margin.Left + 1, Margin.Top + 18, Margin.Left + 1, Height - Margin.Bottom - 8);

            // Right fill line
            e.Graphics.DrawLine(fillPen, Width - Margin.Right - 2, Margin.Top + 18,
                Width - Margin.Right - 2, Height - Margin.Bottom - 8);

            // Bottom fill line
            e.Graphics.DrawLine(fillPen, Margin.Left + 8, Height - Margin.Bottom - 2,
                Width - Margin.Right - 8, Height - Margin.Bottom - 2);

            // Right part of label
            e.Graphics.FillRectangle(new SolidBrush(ColorScheme.Instance.PanelTitleBar), Margin.Left + 3, Margin.Top + 1, 
                Width - Margin.Right - Margin.Left - 6, 18);
            e.Graphics.DrawLine(titlePen, Width - Margin.Right - 3, Margin.Top + 2, Width - Margin.Right - 3, Margin.Top + 19);
            e.Graphics.DrawLine(titlePen, Width - Margin.Right - 2, Margin.Top + 3, Width - Margin.Right - 2, Margin.Top + 18);

            // Left part of label
            e.Graphics.FillRectangle(colorBrush, Margin.Left + 3, Margin.Top + 1, 20, 18);
            e.Graphics.DrawLine(colorPen, Margin.Left + 1, Margin.Top + 3, Margin.Left + 1, Margin.Top + 18);
            e.Graphics.DrawLine(colorPen, Margin.Left + 2, Margin.Top + 2, Margin.Left + 2, Margin.Top + 19);
            
            // Label text
            e.Graphics.DrawString(childPanelName, SystemFonts.CaptionFont, new SolidBrush(ColorScheme.Instance.PanelForeground), 
                new PointF(Margin.Left + 25, Margin.Top + 1));
        }

        /// <summary>
        /// Called when the panel is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void NamedPanel_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: NamedPanel_Layout(object, EventArgs)");

            childPanel.Location = new Point(Margin.Left + 2, Margin.Top + 19);
            childPanel.Size = new Size(Width - Margin.Left - Margin.Right - 4, Height - Margin.Top - Margin.Bottom - 21);
        }

        #endregion
    }
}
