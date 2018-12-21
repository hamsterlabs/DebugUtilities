using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HamsterLabs.DebugUtilities
{
    public static class DataGridViewDumper
    {

        public static void dumpGridViewRow(DataGridViewRow row)
        {
            System.Diagnostics.Debug.WriteLine(Environment.NewLine);
            if (row.Tag != null)
                System.Diagnostics.Debug.WriteLine(String.Format("Tag: {0}", row.Tag));

            foreach (DataGridViewCell c in row.Cells)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("{0} {1}: {2}", c.ToString(), c.OwningColumn.HeaderText, c.FormattedValue.ToString()));
            }
        }
    }
}
