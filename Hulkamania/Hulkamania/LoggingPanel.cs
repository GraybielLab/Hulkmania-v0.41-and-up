using log4net;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania
{
    internal sealed class LoggingPanel : Panel
    {
        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, Int32 wMsg, Int32 wParam, ref Point lParam);
        
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        private const int LOGPANEL_MAX_LINES = 100;

        private const int SB_PAGEBOTTOM = 7;
        private const int WM_SETREDRAW = 0x000B;
        private const int WM_USER = 0x400;
        private const int EM_GETEVENTMASK = (WM_USER + 59);
        private const int EM_SETEVENTMASK = (WM_USER + 69);

        private const int WM_VSCROLL = 0x115;
        private const int SB_VERT = 1;
        private const int EM_SETSCROLLPOS = WM_USER + 222;
        private const int EM_GETSCROLLPOS = WM_USER + 221;

        #endregion

        #region Fields
        
        private IntPtr eventMask = IntPtr.Zero;

        private RichTextBox logTextbox;

        private long lastReadBytes = 0;

        FileStream logFileStream = null;
        
        StreamReader logFileStreamReader = null;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the panel.
        /// </summary>
        public LoggingPanel()
        {
            logger.Debug("Create: LoggingPanel");

            // Open the main log file as read only. The log process (log4net) can write to the file at the same time.
            logFileStream = new FileStream(AppMain.BaseDirectory + "\\log\\main-log.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            // Limit stream reading to the last section of the log file.
            logFileStreamReader = new StreamReader(logFileStream);

            InitializeGUI();
        }

        #endregion

        #region GUI Creation

        /// <summary>
        /// Constructs the form GUI.
        /// </summary>
        private void InitializeGUI()
        {
            logger.Debug("Enter: InitializeGUI()");

            SuspendLayout();

            logTextbox = new RichTextBox {
                Multiline = true,
                ReadOnly = true,
                BackColor = ColorScheme.Instance.LogPanelBackground,
                ForeColor = ColorScheme.Instance.LogPanelForeground,
                HideSelection = true,
                SelectionProtected = true,
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                ScrollBars = RichTextBoxScrollBars.Both
            };

            this.SetStyle(  ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
                      
            Controls.Add(logTextbox);

            ResumeLayout();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Checks for new log messages to display.
        /// Called by the GUI during the GUI update loop.
        /// </summary>
        public void updateTimer_Tick()
        {
            // check if any new text has been added to the log file. If not, do nothing
            if (logFileStreamReader.BaseStream.Length > lastReadBytes)
            {
                logFileStreamReader.BaseStream.Seek(lastReadBytes, SeekOrigin.Begin);
                lastReadBytes = logFileStreamReader.BaseStream.Length;
            }
            else
            {
                return;
            }

            // figure out what the vertical scroll bar position is
            float verticalScrollBarPos = _verticalScrollBarPercentage();

            string line;
            char[] trimChars = (Environment.NewLine + " ").ToCharArray();
               
            try
            {
                // Stop redrawing and events
                SendMessage(logTextbox.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
                eventMask = SendMessage(logTextbox.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero);

                // Append the new text in the panel.
                while ((line = logFileStreamReader.ReadLine()) != null)
                {
                    // add line of text
                    int index = line.LastIndexOf("Brandeis");
                    string lin = line.Substring(0, index != -1 ? index : line.Length);
                    lin = lin.TrimEnd(trimChars);
                    if (lin.Length > 0) {
                        _appendLogTextboxText(lin + Environment.NewLine);
                    }
                }

                // limit the number of lines of text in the panel to only the last LOGPANEL_MAX_LINES number of lines
                if(logTextbox.Lines.Length > LOGPANEL_MAX_LINES)
                {
                    string[] newLines = new string[LOGPANEL_MAX_LINES];
                    Array.Copy(logTextbox.Lines, logTextbox.Lines.Length - newLines.Length, newLines, 0, newLines.Length);
                    // replace all lines
                    logTextbox.Clear();
                    foreach (string txt in newLines)
                    {
                        string lin = txt.TrimEnd(trimChars);
                        if (lin.Length > 0) {
                            _appendLogTextboxText(lin + Environment.NewLine);
                        }
                    }
                }

                // unless control key was pressed, scroll to bottom
                if ((Control.ModifierKeys & Keys.Shift) == 0)
                {
                    _scrollToEnd();
                }
                else
                {
                    _scrollToPos(verticalScrollBarPos);
                }

            } finally {
  
                // Turn on events and redraw
                SendMessage(logTextbox.Handle, EM_SETEVENTMASK, 0, eventMask);
                SendMessage(logTextbox.Handle, WM_SETREDRAW, 1, IntPtr.Zero);

                Refresh();
            }

        }

        #endregion

        #region Private functions

        /// <summary>
        /// Appends a line of text to the log textbox. Color is applied depending on the content of the text
        /// </summary>
        /// <param name="line">The line of text to append</param>
        private void _appendLogTextboxText(string line)
        {
            Color c = Color.FromArgb(128,128,128);

            if (line.LastIndexOf(" ERROR [") != -1)
            {
                c = Color.Red;
            }

            if (line.LastIndexOf(" Written data to file:") != -1)
            {
                c = Color.Yellow;
            }

            if (line.LastIndexOf(" WARN  [") != -1)
            {
                c = Color.Cyan;
            }

            if (line.LastIndexOf("Saving data to the following location: ") != -1)
            {
                c = Color.Yellow;
            }

            logTextbox.SelectionStart = logTextbox.TextLength;
            logTextbox.SelectionLength = 0;
            logTextbox.SelectionColor = c;
            logTextbox.AppendText(line);
            logTextbox.SelectionColor = logTextbox.ForeColor;
        }

        /// <summary>
        /// Determines the percentage that the vertical scroll bar is scrolled
        /// </summary>
        /// <returns>The percentage that the vertical scroll bar is scrolled, in the range 0-1</returns>
        private float _verticalScrollBarPercentage()
        {
            int minScroll;
            int maxScroll;

            GetScrollRange(logTextbox.Handle, SB_VERT, out minScroll, out maxScroll);

            if (maxScroll == 0)
            {
                return 0;
            }
            Point rtfPoint = Point.Empty;
            SendMessage(logTextbox.Handle, EM_GETSCROLLPOS, 0, ref rtfPoint);

            return (rtfPoint.Y * 1.0f/ (maxScroll - logTextbox.Height));
        }

        /// <summary>
        /// Scrolls the logTextbox to the very end. Done this way due to a bug in C# 'ScrollToCaret' function
        /// </summary>
        private void _scrollToEnd()
        {
            SendMessage(logTextbox.Handle, WM_VSCROLL, SB_PAGEBOTTOM, IntPtr.Zero);
        }

        /// <summary>
        /// Scrolls the logTextbox to the a relative position (percentage)
        /// </summary>
        private void _scrollToPos(float percentage)
        {
            int minScroll;
            int maxScroll;

            GetScrollRange(logTextbox.Handle, SB_VERT, out minScroll, out maxScroll);

            if (maxScroll == 0)
            {
                return;
            }

            Point rtfPoint = new Point();
            rtfPoint.X = 0;
            rtfPoint.Y = (int)((maxScroll - logTextbox.Height) * percentage);
            SendMessage(logTextbox.Handle, EM_SETSCROLLPOS, 0, ref rtfPoint);
        }
        #endregion
    }
}
