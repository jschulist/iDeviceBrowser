using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace iDeviceBrowser
{
    public class CustomListView : ListView
    {
        public CustomListView()
            : base()
        {
            // removes unnecessary flickering by painting off screen, when shifting items or other actions
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
