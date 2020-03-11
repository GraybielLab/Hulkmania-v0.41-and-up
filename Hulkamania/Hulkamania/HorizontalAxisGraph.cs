using log4net;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania
{
    internal sealed class HorizontalAxisGraph : Panel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly int paddingLeft = 100;
        private static readonly int labelsOffset = 25;
        private static readonly int labelPadding = 5;

        #endregion

        #region Fields

        private Bitmap bitmap;

        private Brush backgroundBrush;
        private Brush foregroundBrush;
        private Brush joystickPressBrush;

        private Brush velocityBrush;
        private Brush positionBrush;

        private Graphics bitmapGraphics;

        private Pen backgroundPen;
        private Pen balanceLinePen;
        private Pen positionPen;
        private Pen subdivision1Pen;
        private Pen subdivision2Pen;
        private Pen velocityPen;

        private String identifier;

        private bool innerAxis;

        private int previousPosition;
        private int previousVelocity;
        private int previousData;

        private double axisCenter = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the center of the Y axis for this graph.
        /// </summary>
        public double AxisCenter { 
            get { 
                return axisCenter; 
            } 
            set { 
                axisCenter = value;
            } 
        }

        /// <summary>
        /// Set to true to display the inner axis, or false for the outer axis.
        /// </summary>
        public bool InnerAxis
        {
            set {
                innerAxis = value;

                if (value) {
                    identifier = "Inner Axis (" + Hulk.InnerAxis + ")";
                } else {
                    identifier = "Outer Axis (" + Hulk.OuterAxis + ")";
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the graph.
        /// </summary>
        public HorizontalAxisGraph()
        {
            logger.Debug("Create: HorizontalAxisGraph");

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

            backgroundPen = new Pen(new SolidBrush(ColorScheme.Instance.GraphBackground));
            positionPen = new Pen(new SolidBrush(ColorScheme.Instance.GraphPositionLine));
            subdivision1Pen = new Pen(new SolidBrush(ColorScheme.Instance.GraphSubdivisionLine1));
            subdivision2Pen = new Pen(new SolidBrush(ColorScheme.Instance.GraphSubdivisionLine2));
            velocityPen = new Pen(new SolidBrush(ColorScheme.Instance.GraphVelocityLine));

            velocityBrush = new SolidBrush(ColorScheme.Instance.GraphVelocityLine);
            positionBrush = new SolidBrush(ColorScheme.Instance.GraphPositionLine);

            backgroundBrush = new SolidBrush(ColorScheme.Instance.GraphBackground);
            foregroundBrush = new SolidBrush(ColorScheme.Instance.GraphForeground);
            joystickPressBrush = new SolidBrush(ColorScheme.Instance.GraphJoystickPress);

            balanceLinePen = new Pen(Brushes.Red) {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
            };

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            Layout += HorizontalAxisGraph_Layout;
            Paint += HorizontalAxisGraph_Paint;

            ResumeLayout();
        }

        #endregion

        #region Event Handlers
        
        /// <summary>
        /// Updates the GUI with the current information from the motion controller.
        /// </summary>
        public void updateTimer_Tick()
        {
            DisplayableData data;
            double pixelsPerDeg;
            double centerY;
            double position;
            double velocity;
            int newPosition;
            int newVelocity;
            int newData;

            centerY = bitmap.Height / 2.0;
            pixelsPerDeg = bitmap.Height / 360.0;

            // Move the graph one pixel to the left
            bitmapGraphics.DrawImage((Bitmap)bitmap.Clone(), new Point(-1, 0));

            // Clear the column of pixels that is drawn into
            bitmapGraphics.DrawLine(backgroundPen, bitmap.Width - 2, 0, bitmap.Width - 2, bitmap.Height);

            // Draw the position
            position = (innerAxis ? Hulk.CurrentMotion.innerPosition : Hulk.CurrentMotion.outerPosition);
            if (position != Hulk.ANY_MOTION) {
                newPosition = (int)(centerY - (pixelsPerDeg * (position-axisCenter)));
                bitmapGraphics.DrawLine(positionPen, bitmap.Width - 3, previousPosition, bitmap.Width - 2, newPosition);
                previousPosition = newPosition;
            }

            // Draw the velocity
            velocity = (innerAxis ? Hulk.CurrentMotion.innerVelocity : Hulk.CurrentMotion.outerVelocity);
            if (velocity == Hulk.ANY_MOTION) {
                return;
            }
            newVelocity = (int)(centerY - (pixelsPerDeg * velocity));
            bitmapGraphics.DrawLine(velocityPen, bitmap.Width - 3, previousVelocity, bitmap.Width - 2, newVelocity);
            previousVelocity = newVelocity;

            // Draw the trigger presses
            if (InputController.JoystickInput.trigger) {
                bitmapGraphics.FillEllipse(joystickPressBrush, bitmap.Width - 4, previousPosition - 2, 4, 4);
            }

            // Draw task-specifics
            data = null;
            if (innerAxis) {
                if (AppMain.CurrentTask != null) {
                    data = AppMain.CurrentTask.DataInnerAxis;
                }
            } else {
                if (AppMain.CurrentTask != null) {
                    data = AppMain.CurrentTask.DataOuterAxis;
                }
            }
            if (data != null) {
                double dataLimited = data.value - axisCenter;
                newData = (int)(centerY - (pixelsPerDeg * dataLimited));
                bitmapGraphics.DrawLine(data.pen, bitmap.Width - 3, previousData, bitmap.Width - 2, newData);
                previousData = newData;
            }
        }

        /// <summary>
        /// Called each time the panel is painted.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void HorizontalAxisGraph_Paint(object sender, PaintEventArgs e)
        {
            double distBetweenLines;

       
            distBetweenLines = DisplayRectangle.Height / 6.0;

            // Bakckground
            e.Graphics.FillRectangle(backgroundBrush, DisplayRectangle.Left + paddingLeft, DisplayRectangle.Top, 
                DisplayRectangle.Width - paddingLeft, DisplayRectangle.Height);

            // Graph data
            e.Graphics.DrawImage(bitmap, DisplayRectangle.Left + paddingLeft, DisplayRectangle.Top);

            // Horizontal lines 
            e.Graphics.DrawLine(subdivision2Pen, DisplayRectangle.Left + paddingLeft, (int)distBetweenLines + DisplayRectangle.Top,
                DisplayRectangle.Right, (int)distBetweenLines + DisplayRectangle.Top);
            e.Graphics.DrawLine(subdivision2Pen, DisplayRectangle.Left + paddingLeft, (int)(distBetweenLines * 2) + DisplayRectangle.Top,
                DisplayRectangle.Right, (int)(distBetweenLines * 2) + DisplayRectangle.Top);
            e.Graphics.DrawLine(subdivision1Pen, DisplayRectangle.Left + paddingLeft, (int)(distBetweenLines * 3) + DisplayRectangle.Top,
                DisplayRectangle.Right, (int)(distBetweenLines * 3) + DisplayRectangle.Top);
            e.Graphics.DrawLine(subdivision2Pen, DisplayRectangle.Left + paddingLeft, (int)(distBetweenLines * 4) + DisplayRectangle.Top,
                DisplayRectangle.Right, (int)(distBetweenLines * 4) + DisplayRectangle.Top);
            e.Graphics.DrawLine(subdivision2Pen, DisplayRectangle.Left + paddingLeft, (int)(distBetweenLines * 5) + DisplayRectangle.Top,
                DisplayRectangle.Right, (int)(distBetweenLines * 5) + DisplayRectangle.Top);

            // Y axis (angle)
            e.Graphics.DrawLine(subdivision1Pen, DisplayRectangle.Left + paddingLeft, DisplayRectangle.Top,
                DisplayRectangle.Left + paddingLeft, DisplayRectangle.Bottom);

            e.Graphics.DrawString((axisCenter + 120).ToString(), SystemFonts.DefaultFont, positionBrush,
                new PointF(labelsOffset, (float)(distBetweenLines + DisplayRectangle.Top - 6)));
            e.Graphics.DrawString((axisCenter + 60).ToString(), SystemFonts.DefaultFont, positionBrush,
                new PointF(labelsOffset, (float)((distBetweenLines * 2) + DisplayRectangle.Top - 6)));
            e.Graphics.DrawString(axisCenter.ToString(), SystemFonts.DefaultFont, positionBrush,
                new PointF(labelsOffset, (float)((distBetweenLines * 3) + DisplayRectangle.Top - 6)));
            e.Graphics.DrawString((axisCenter - 60).ToString(), SystemFonts.DefaultFont, positionBrush,
                new PointF(labelsOffset, (float)((distBetweenLines * 4) + DisplayRectangle.Top - 6)));
            e.Graphics.DrawString((axisCenter - 120).ToString(), SystemFonts.DefaultFont, positionBrush,
                new PointF(labelsOffset, (float)((distBetweenLines * 5) + DisplayRectangle.Top - 6)));

            // Y axis (velocity)
            e.Graphics.DrawString("120", SystemFonts.DefaultFont, velocityBrush,
                new PointF(labelsOffset * 2 + labelPadding, (float)(distBetweenLines + DisplayRectangle.Top - 6)));
            e.Graphics.DrawString("60", SystemFonts.DefaultFont, velocityBrush,
                new PointF(labelsOffset * 2 + labelPadding, (float)((distBetweenLines * 2) + DisplayRectangle.Top - 6)));
            e.Graphics.DrawString("0", SystemFonts.DefaultFont, velocityBrush,
                new PointF(labelsOffset * 2 + labelPadding, (float)((distBetweenLines * 3) + DisplayRectangle.Top - 6)));
            e.Graphics.DrawString("-60".ToString(), SystemFonts.DefaultFont, velocityBrush,
                new PointF(labelsOffset * 2 + labelPadding, (float)((distBetweenLines * 4) + DisplayRectangle.Top - 6)));
            e.Graphics.DrawString("-120", SystemFonts.DefaultFont, velocityBrush,
                new PointF(labelsOffset * 2 + labelPadding, (float)((distBetweenLines * 5) + DisplayRectangle.Top - 6)));

            // Identifier
            e.Graphics.DrawString(identifier, SystemFonts.DefaultFont, foregroundBrush, new PointF(paddingLeft+10, 10));

            //
            e.Graphics.DrawString("POSITION", SystemFonts.DefaultFont, positionBrush, new PointF(10 + paddingLeft, (float)((distBetweenLines * 3) + DisplayRectangle.Top - 6 - 10)));
            e.Graphics.DrawString("/", SystemFonts.DefaultFont, foregroundBrush, new PointF(70 + paddingLeft, (float)((distBetweenLines * 3) + DisplayRectangle.Top - 6 - 10)));
            e.Graphics.DrawString("VELOCITY", SystemFonts.DefaultFont, velocityBrush, new PointF(80 + paddingLeft, (float)((distBetweenLines * 3) + DisplayRectangle.Top - 6 - 10)));

            // Separator
            e.Graphics.DrawLine(subdivision1Pen, 0, 0, Width, 0);
        }
        
        /// <summary>
        /// Called when the graph is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void HorizontalAxisGraph_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: HorizontalAxisGraph_Layout(object, EventArgs)");

            if (DisplayRectangle.Width != 0) {
                bitmap = new Bitmap(DisplayRectangle.Width - paddingLeft, DisplayRectangle.Height);
                bitmapGraphics = Graphics.FromImage(bitmap);
            }
        }

        #endregion
    }
}
