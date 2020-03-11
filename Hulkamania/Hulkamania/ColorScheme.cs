using System.Drawing;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// Color scheme for the application.
    /// </summary>
    public abstract class ColorScheme
    {
         #region GUI-Mapped Colors

        public abstract Color XyGraphAxis1Color{ get; }
        public abstract Color XyGraphAxis2Color{ get; }
        public abstract Color XyGraphDualPlotColor{ get; }
        
        public abstract Color FormBackground{ get; }

        public abstract Color AudioPanelMarker{ get; }
        public abstract Color ControlPanelMarker{ get; }
        public abstract Color GraphsPanelMarker{ get; }
        public abstract Color EventLogPanelMarker{ get; }
        public abstract Color StatusPanelMarker{ get; }
        public abstract Color TaskPanelMarker{ get; }

        public abstract Color ButtonBackground{ get; }
        public abstract Color ButtonFlatBorder{ get; }
        public abstract Color ButtonForeground{ get; }
        public abstract Color ButtonMouseDown{ get; }
        public abstract Color ButtonMouseOver{ get; }

        public abstract Color GraphBackground{ get; }
        public abstract Color GraphForeground{ get; }
        public abstract Color GraphJoystickPress{ get; }
        public abstract Color GraphPositionLine{ get; }
        public abstract Color GraphSubdivisionLine1{ get; }
        public abstract Color GraphSubdivisionLine2{ get; }
        public abstract Color GraphVelocityLine{ get; }

        public abstract Color GridBackground{ get; }
        public abstract Color GridColumnHeaderBackground{ get; }
        public abstract Color GridColumnHeaderForeground{ get; }
        public abstract Color GridForeground{ get; }

        public abstract Color LabelForeground{ get; }

        public abstract Color ListBoxBackground{ get; }
        public abstract Color ListBoxForeground{ get; }

        public abstract Color MenuBarBackground{ get; }
        public abstract Color MenuBarForeground{ get; }
        public abstract Color MenuBarSelected{ get; }

        public abstract Color LogPanelBackground{ get; }
        public abstract Color LogPanelForeground{ get; }

        public abstract Color MenuItemBackground{ get; }
        public abstract Color MenuItemForeground{ get; }
        public abstract Color MenuItemSelected{ get; }
        
        public abstract Color PanelBackground{ get; }
        public abstract Color PanelEdge{ get; }
        public abstract Color PanelForeground{ get; }
        public abstract Color PanelTitleBar{ get; }
   
        public abstract Color StatusBarBackground{ get; }
        public abstract Color StatusBarForeground{ get; }

        public abstract Color StatusLightOK{ get; }
        public abstract Color StatusLightError{ get; }

        public abstract Color TextBoxBackground{ get; }
        public abstract Color TextBoxDisabledBackground{ get; }
        public abstract Color TextBoxForeground{ get; }

        public abstract Color AudioPanelActiveChannelColor{ get; }
        #endregion

        private static ColorScheme instance = null;

        public static ColorScheme Instance
        {
            get
            {
                if (instance == null)
                {
                    if (AppMain.UseDummyMotionController)
                    {
                        instance = new ColorSchemeDummyController();
                    }
                    else
                    {
                        instance = new ColorSchemeHulkController();
                    }
                }
                return instance;
            }
        }
    }
}
