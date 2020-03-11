using System.Drawing;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// Color scheme for the application.
    /// </summary>
    public sealed class ColorSchemeHulkController : ColorScheme
    {
        #region Base Colors

        // Color scheme "Solarized" by Ethan Schoonover.

        private static readonly Color Base03 = Color.FromArgb(0, 43, 54);
        private static readonly Color Base02 = Color.FromArgb(7, 54, 66);
        private static readonly Color Base01 = Color.FromArgb(88, 110, 117);
        private static readonly Color Base00 = Color.FromArgb(101, 123, 131);
        private static readonly Color Base0 = Color.FromArgb(131, 148, 150);
        private static readonly Color Base1 = Color.FromArgb(147, 161, 161);
        private static readonly Color Base2 = Color.FromArgb(238, 232, 213);
        private static readonly Color Base3 = Color.FromArgb(253, 246, 227);

        private static readonly Color Yellow = Color.FromArgb(181, 137, 0);
        private static readonly Color Orange = Color.FromArgb(203, 75, 22);
        private static readonly Color Red = Color.FromArgb(220, 50, 47);
        private static readonly Color Magenta = Color.FromArgb(211, 54, 130);
        private static readonly Color Violet = Color.FromArgb(108, 113, 196);
        private static readonly Color Blue = Color.FromArgb(38, 139, 210);
        private static readonly Color Cyan = Color.FromArgb(42, 161, 152);
        private static readonly Color Green = Color.FromArgb(133, 153, 0);

        #endregion

        #region GUI-Mapped Colors

        public override Color XyGraphAxis1Color { get { return Red; } }
        public override Color XyGraphAxis2Color { get { return Blue; } }
        public override Color XyGraphDualPlotColor { get { return Yellow; } }

        public override Color FormBackground { get { return Base03; } }

        public override Color AudioPanelMarker { get { return Green; } }
        public override Color ControlPanelMarker { get { return Magenta; } }
        public override Color GraphsPanelMarker { get { return Yellow; } }
        public override Color EventLogPanelMarker { get { return Red; } }
        public override Color StatusPanelMarker { get { return Violet; } }
        public override Color TaskPanelMarker { get { return Blue; } }

        public override Color ButtonBackground { get { return Base03; } }
        public override Color ButtonFlatBorder { get { return Color.Black; } }
        public override Color ButtonForeground { get { return Base3; } }
        public override Color ButtonMouseDown { get { return Yellow; } }
        public override Color ButtonMouseOver { get { return Base1; } }

        public override Color GraphBackground { get { return Color.Black; } }
        public override Color GraphForeground { get { return Base3; } }
        public override Color GraphJoystickPress { get { return Yellow; } }
        public override Color GraphPositionLine { get { return Red; } }
        public override Color GraphSubdivisionLine1 { get { return Base01; } }
        public override Color GraphSubdivisionLine2 { get { return Color.FromArgb(255, 32, 32, 32); } }
        public override Color GraphVelocityLine { get { return Blue; } }

        public override Color GridBackground { get { return Color.Black; } }
        public override Color GridColumnHeaderBackground { get { return Base02; } }
        public override Color GridColumnHeaderForeground { get { return Base3; } }
        public override Color GridForeground { get { return Base3; } }

        public override Color LabelForeground { get { return Base3; } }

        public override Color ListBoxBackground { get { return Base03; } }
        public override Color ListBoxForeground { get { return Base3; } }

        public override Color MenuBarBackground { get { return Base02; } }
        public override Color MenuBarForeground { get { return Base3; } }
        public override Color MenuBarSelected { get { return Base0; } }

        public override Color LogPanelBackground { get { return Color.Black; } }
        public override Color LogPanelForeground { get { return Base3; } }

        public override Color MenuItemBackground { get { return Base0; } }
        public override Color MenuItemForeground { get { return Base03; } }
        public override Color MenuItemSelected { get { return Base2; } }

        public override Color PanelBackground { get { return Color.Black; } }
        public override Color PanelEdge { get { return Base1; } }
        public override Color PanelForeground { get { return Base3; } }
        public override Color PanelTitleBar { get { return Base01; } }

        public override Color StatusBarBackground { get { return Base02; } }
        public override Color StatusBarForeground { get { return Base3; } }

        public override Color StatusLightOK { get { return Green; } }
        public override Color StatusLightError { get { return Red; } }

        public override Color TextBoxBackground { get { return Base03; } }
        public override Color TextBoxDisabledBackground { get { return Base00; } }
        public override Color TextBoxForeground { get { return Base3; } }

        public override Color AudioPanelActiveChannelColor { get { return Green; } }
        #endregion
    }
}
