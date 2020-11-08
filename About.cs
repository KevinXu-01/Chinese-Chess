using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chinese_Chess
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Click(object sender, EventArgs e)
        {
            MouseEventArgs mouse = (MouseEventArgs)e;
            if (mouse.X >= 761 && mouse.Y <= 38)
                Close();
        }

        private void About_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X >= 761 && e.Y <= 38)
                Cursor = Cursors.Hand;
            else
                Cursor = Cursors.Default;
        }
    }
}
