using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleMusicPlayer.CustomControls
{
    /// <summary>
    /// Credit goes to https://github.com/daszat for his nice implementation to make listbox to be like windows multi select
    /// </summary>
    public class ListViewMultiSelect : ListView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListViewItemEx();
        }

        public class ListViewItemEx : ListViewItem
        {
            private bool checkmultiselect = false;

            protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
            {

                // If it is not a double click and the item is already selected.  We will wait to process OnMouseLeftButtonDown
                // Since when your drag its mouse down drag then mouse up this allows us to keep the selection
                if (e.ClickCount == 1 && IsSelected)                
                    checkmultiselect = true;                
                else                
                    // Do normal ListViewItem stuff
                    base.OnMouseLeftButtonDown(e);                
            }

            protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
            {
                // If we do have a multiselection and do a MouseUp that means the user is trying to select that one item so we do a proper listviewitem mouse down then mouse up.
                if (checkmultiselect)
                {
                    try
                    {
                        base.OnMouseLeftButtonDown(e);
                    }
                    finally
                    {
                        checkmultiselect = false;
                    }
                }

                base.OnMouseLeftButtonUp(e);
            }

            protected override void OnMouseLeave(MouseEventArgs e)
            {
                // We have left the listview area so reset our flag.  This means if we drag away we keep our current selection which is what we want.
                checkmultiselect = false;
                base.OnMouseLeave(e);
            }
        }
    }
}
