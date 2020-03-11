using System.Drawing;
using System.Windows.Forms;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// Drop-in replacement for standard ListBox that reduces flickering on refresh.
    /// </summary>
    public class FlickerFreeListBox : ListBox
    {
        /// <summary>
        /// Constructs the list box.
        /// </summary>
        public FlickerFreeListBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        /// <summary>
        /// Draws a single list item.
        /// </summary>
        /// <param name="e">Information on the item</param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (Items.Count > 0) {
                e.DrawBackground();
                e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, new SolidBrush(ForeColor), new PointF(e.Bounds.X, e.Bounds.Y));
            }
            base.OnDrawItem(e);
        }

        /// <summary>
        /// Paints the control.
        /// </summary>
        /// <param name="e">Information on how to paint</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle irect;
            Region iRegion;
            
            iRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(BackColor), iRegion);

            if (Items.Count > 0) {
                
                for (int i = 0; i < Items.Count; ++i) {
                    
                    irect = GetItemRectangle(i);
                    if (e.ClipRectangle.IntersectsWith(irect)) {

                        if ((SelectionMode == SelectionMode.One && SelectedIndex == i) || 
                            (SelectionMode == SelectionMode.MultiSimple && SelectedIndices.Contains(i)) || 
                            (SelectionMode == SelectionMode.MultiExtended && SelectedIndices.Contains(i))) {

                            OnDrawItem(new DrawItemEventArgs(e.Graphics, Font, irect, i, DrawItemState.Selected, ForeColor, BackColor));
                        } else {
                            
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, Font, irect, i, DrawItemState.Default, ForeColor, BackColor));
                        }
                        iRegion.Complement(irect);
                    }
                }
            }
            base.OnPaint(e);
        }
    }
}
