using log4net;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania
{
    public sealed class JoystickGraph : Panel
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        private int timeGraphOffset;
        private Bitmap timeGraphBitmap;
        private Graphics timeGraphBitmapGraphics;

        private Bitmap xyGraphBitmap;
        private Graphics xyGraphBitmapGraphics;
        private Point previousJoystickPos;
        private Pen xyGraphPlotPen;
        private Brush xyGraphPlotBrush;
        private const int xyGraphPlotThickness = 4;
        
        private Brush backgroundBrush;
        private Brush foregroundBrush;
        private Brush axis1Brush;
        private Brush axis2Brush;
        private Brush joystickPressBrush;
        private SolidBrush fadeBrush;

        private Pen backgroundPen;
        private Pen foregroundPen;
        private Pen subdivision1Pen;
        private Pen subdivision2Pen;

        private Pen axis1Pen;
        private Pen axis2Pen;

        private String identifier;

        private int previousAxis1;
        private int previousAxis2;
    
        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the graph.
        /// </summary>
        public JoystickGraph()
        {
            logger.Debug("Create: JoystickGraph");

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
            foregroundPen = new Pen(new SolidBrush(ColorScheme.Instance.GraphForeground));
            subdivision1Pen = new Pen(new SolidBrush(ColorScheme.Instance.GraphSubdivisionLine1));
            subdivision2Pen = new Pen(new SolidBrush(ColorScheme.Instance.GraphSubdivisionLine2));

            axis1Pen = new Pen(new SolidBrush(ColorScheme.Instance.XyGraphAxis1Color));
            axis2Pen = new Pen(new SolidBrush(ColorScheme.Instance.XyGraphAxis2Color));
            axis1Brush = new SolidBrush(ColorScheme.Instance.XyGraphAxis1Color);
            axis2Brush = new SolidBrush(ColorScheme.Instance.XyGraphAxis2Color);

            fadeBrush = new SolidBrush(Color.FromArgb(50,0,0,0));
            xyGraphPlotPen = new Pen(new SolidBrush(ColorScheme.Instance.XyGraphDualPlotColor), xyGraphPlotThickness);
            xyGraphPlotBrush = new SolidBrush(ColorScheme.Instance.XyGraphDualPlotColor);
            
            backgroundBrush = new SolidBrush(ColorScheme.Instance.GraphBackground);
            foregroundBrush = new SolidBrush(ColorScheme.Instance.GraphForeground);
            joystickPressBrush = new SolidBrush(ColorScheme.Instance.GraphJoystickPress);

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            Layout += JoystickGraph_Layout;
            Paint += JoystickGraph_Paint;

            identifier = "Joystick";

            ResumeLayout();
        }

        #endregion

        #region Event Handlers
        
        /// <summary>
        /// Updates the GUI with the current information from the motion controller.
        /// </summary>
        public void updateTimer_Tick(double jX, double jY, bool trigger)
        {
            double pixelsPerDeg;
            double centerY;
            double axis1;
            double axis2;
            int newAxis1;
            int newAxis2;

            centerY = timeGraphBitmap.Height / 2.0;
            pixelsPerDeg = timeGraphBitmap.Height / 2.0;

            // Move the graph one pixel to the left
            timeGraphBitmapGraphics.DrawImage((Bitmap)timeGraphBitmap.Clone(), new Point(-1, 0));

            // Clear the column of pixels that is drawn into
            timeGraphBitmapGraphics.DrawLine(backgroundPen, timeGraphBitmap.Width - 2, 0, timeGraphBitmap.Width - 2, timeGraphBitmap.Height);

            // Draw axis1
            axis1 = jX;
            newAxis1 = (int)(centerY - (pixelsPerDeg * axis1));
            timeGraphBitmapGraphics.DrawLine(axis1Pen, timeGraphBitmap.Width - 3, previousAxis1, timeGraphBitmap.Width - 2, newAxis1);
            previousAxis1 = newAxis1;
          
            // Draw axis2
            axis2 = jY;
            newAxis2 = (int)(centerY + (pixelsPerDeg * axis2));
            timeGraphBitmapGraphics.DrawLine(axis2Pen, timeGraphBitmap.Width - 3, previousAxis2, timeGraphBitmap.Width - 2, newAxis2);
            previousAxis2 = newAxis2;

            // Draw the trigger presses
            if (trigger) {
                timeGraphBitmapGraphics.FillEllipse(joystickPressBrush, timeGraphBitmap.Width - 4, previousAxis1 - 2, 4, 4);
                timeGraphBitmapGraphics.FillEllipse(joystickPressBrush, timeGraphBitmap.Width - 4, previousAxis2 - 2, 4, 4);
            }

            // draw xy position
            xyGraphBitmapGraphics.FillRectangle(fadeBrush, 0, 0, xyGraphBitmap.Width, xyGraphBitmap.Height);
            int ellipseDiameter = xyGraphPlotThickness;
            int x = (int)(xyGraphBitmap.Width * 0.5f + axis1 * xyGraphBitmap.Width * 0.5f );
            int y = (int)(xyGraphBitmap.Height * 0.5f + axis2 * xyGraphBitmap.Height * 0.5f );

            xyGraphBitmapGraphics.DrawLine(xyGraphPlotPen, previousJoystickPos.X + (int)ellipseDiameter / 2, previousJoystickPos.Y + (int)ellipseDiameter / 2, x , y );
            xyGraphBitmapGraphics.FillEllipse(xyGraphPlotBrush, x - ellipseDiameter / 2, y - ellipseDiameter / 2, ellipseDiameter, ellipseDiameter);

            previousJoystickPos = new Point(x, y);
        }

        /// <summary>
        /// Called each time the panel is painted.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void JoystickGraph_Paint(object sender, PaintEventArgs e)
        {
            double distBetweenLines;

            distBetweenLines = DisplayRectangle.Height / 6.0;

            // Background
            e.Graphics.FillRectangle(backgroundBrush, DisplayRectangle.Left + 50 + timeGraphOffset, DisplayRectangle.Top,
                DisplayRectangle.Width - 50 - timeGraphOffset, DisplayRectangle.Height);


            // Time Graph data
            e.Graphics.DrawImage(timeGraphBitmap, DisplayRectangle.Left + 50 + timeGraphOffset, DisplayRectangle.Top);

            // Horizontal lines (-1, -0.5, 0, 0.5, 1 degrees)
            e.Graphics.DrawLine(subdivision2Pen, DisplayRectangle.Left + 50 + timeGraphOffset, (int)distBetweenLines + DisplayRectangle.Top,
                DisplayRectangle.Right, (int)distBetweenLines + DisplayRectangle.Top);
            e.Graphics.DrawLine(subdivision2Pen, DisplayRectangle.Left + 50 + timeGraphOffset, (int)(distBetweenLines * 2) + DisplayRectangle.Top,
                DisplayRectangle.Right, (int)(distBetweenLines * 2) + DisplayRectangle.Top);
            e.Graphics.DrawLine(subdivision1Pen, DisplayRectangle.Left + 50 + timeGraphOffset, (int)(distBetweenLines * 3) + DisplayRectangle.Top,
                DisplayRectangle.Right, (int)(distBetweenLines * 3) + DisplayRectangle.Top);
            e.Graphics.DrawLine(subdivision2Pen, DisplayRectangle.Left + 50 + timeGraphOffset, (int)(distBetweenLines * 4) + DisplayRectangle.Top,
                DisplayRectangle.Right, (int)(distBetweenLines * 4) + DisplayRectangle.Top);
            e.Graphics.DrawLine(subdivision2Pen, DisplayRectangle.Left + 50 + timeGraphOffset, (int)(distBetweenLines * 5) + DisplayRectangle.Top,
                DisplayRectangle.Right, (int)(distBetweenLines * 5) + DisplayRectangle.Top);

            // Y axis
            e.Graphics.DrawLine(subdivision1Pen, DisplayRectangle.Left + 50 + timeGraphOffset, DisplayRectangle.Top,
                DisplayRectangle.Left + 50 + timeGraphOffset, DisplayRectangle.Bottom);

            e.Graphics.DrawString("0.66", SystemFonts.DefaultFont, foregroundBrush,
                new PointF(20 + timeGraphOffset, (float)(distBetweenLines + DisplayRectangle.Top - 6)));
            e.Graphics.DrawString("0.33", SystemFonts.DefaultFont, foregroundBrush,
                new PointF(20 + timeGraphOffset, (float)((distBetweenLines * 2) + DisplayRectangle.Top - 6)));
            e.Graphics.DrawString("0", SystemFonts.DefaultFont, foregroundBrush,
                new PointF(35 + timeGraphOffset, (float)((distBetweenLines * 3) + DisplayRectangle.Top - 6)));
            e.Graphics.DrawString("-0.33", SystemFonts.DefaultFont, foregroundBrush,
                new PointF(15 + timeGraphOffset, (float)((distBetweenLines * 4) + DisplayRectangle.Top - 6)));
            e.Graphics.DrawString("-0.66", SystemFonts.DefaultFont, foregroundBrush,
                new PointF(15 + timeGraphOffset, (float)((distBetweenLines * 5) + DisplayRectangle.Top - 6)));

            // Joystick axes
            e.Graphics.DrawString("RIGHT", SystemFonts.DefaultFont, axis1Brush, new PointF(60 + timeGraphOffset, (float)((distBetweenLines * 3) + DisplayRectangle.Top - 6) - 10));
            e.Graphics.DrawString("/", SystemFonts.DefaultFont, foregroundBrush, new PointF(100 + timeGraphOffset, (float)((distBetweenLines * 3) + DisplayRectangle.Top - 6) - 10));
            e.Graphics.DrawString("FORWARD", SystemFonts.DefaultFont, axis2Brush, new PointF(110 + timeGraphOffset, (float)((distBetweenLines * 3) + DisplayRectangle.Top - 6) - 10));

            e.Graphics.DrawString("LEFT", SystemFonts.DefaultFont, axis1Brush, new PointF(60 + timeGraphOffset, (float)((distBetweenLines * 3) + DisplayRectangle.Top - 6) + 10));
            e.Graphics.DrawString("/", SystemFonts.DefaultFont, foregroundBrush, new PointF(100 + timeGraphOffset, (float)((distBetweenLines * 3) + DisplayRectangle.Top - 6) + 10));
            e.Graphics.DrawString("BACKWARD", SystemFonts.DefaultFont, axis2Brush, new PointF(110 + timeGraphOffset, (float)((distBetweenLines * 3) + DisplayRectangle.Top - 6) + 10));



            // XY graph data
            e.Graphics.DrawImage(xyGraphBitmap, DisplayRectangle.Left, DisplayRectangle.Top);

            // Axis ticks
            int numTicks = 6;
            double distBetweenTicks = DisplayRectangle.Height / numTicks;

            for(int i=0 ; i<numTicks; i++){
                e.Graphics.DrawLine(subdivision2Pen, DisplayRectangle.Left, (int)distBetweenTicks * i + DisplayRectangle.Top, timeGraphOffset, (int)distBetweenTicks * i + DisplayRectangle.Top);
                e.Graphics.DrawLine(subdivision2Pen, (int)distBetweenTicks * i + DisplayRectangle.Left, DisplayRectangle.Top, (int)distBetweenTicks * i + DisplayRectangle.Left, timeGraphOffset );
            }
            e.Graphics.DrawRectangle(subdivision1Pen, DisplayRectangle.Left, DisplayRectangle.Top, timeGraphOffset, timeGraphOffset);
            e.Graphics.DrawLine(subdivision1Pen, DisplayRectangle.Left, DisplayRectangle.Top + (int)DisplayRectangle.Height / 2, DisplayRectangle.Left + timeGraphOffset, DisplayRectangle.Top + (int)DisplayRectangle.Height / 2);
            e.Graphics.DrawLine(subdivision1Pen, DisplayRectangle.Left + (int)timeGraphOffset / 2, DisplayRectangle.Top,
                                                 DisplayRectangle.Left + (int)timeGraphOffset / 2, DisplayRectangle.Top + DisplayRectangle.Height);
            
            // Identifier
            e.Graphics.DrawString(identifier, SystemFonts.DefaultFont, foregroundBrush, new PointF(70, 10));

            // Separator
            e.Graphics.DrawLine(subdivision1Pen, 0, 0, Width, 0);
        }
        
        /// <summary>
        /// Called when the graph is resized.
        /// </summary>
        /// <param name="sender">Not used</param>
        /// <param name="e">Not used</param>
        private void JoystickGraph_Layout(object sender, EventArgs e)
        {
            logger.Debug("Enter: JoystickGraph_Layout(object, EventArgs)");

            if (DisplayRectangle.Width != 0) {
                timeGraphOffset = DisplayRectangle.Height;

                timeGraphBitmap = new Bitmap(DisplayRectangle.Width - 50 - timeGraphOffset, DisplayRectangle.Height);
                timeGraphBitmapGraphics = Graphics.FromImage(timeGraphBitmap);

                xyGraphBitmap = new Bitmap(timeGraphOffset, timeGraphOffset);
                xyGraphBitmapGraphics = Graphics.FromImage(xyGraphBitmap);

                previousJoystickPos = new Point((int)timeGraphOffset / 2, (int)timeGraphOffset/2);

            }
        }

        #endregion
    }
}
