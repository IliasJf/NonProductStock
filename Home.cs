using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NonProductStock
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 30, 30));
        }
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn
        (
           int nLeftRect,
           int nTopRect,
           int nRightRect,
           int nBottomRect,
           int nWidthEllipse,
           int nHeightEllipse
        );

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult res= MessageBox.Show("Are you sure?", "Exit", MessageBoxButtons.YesNo);
            if (res==DialogResult.Yes)
            {
                this.Dispose();
                Login lg = new Login();
                lg.ShowDialog();
            }
        }

        private void btnComponents_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Component c = new Component();
            c.ShowDialog();
        }

        private void btnOperations_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Operations o = new Operations();
            o.ShowDialog();
        }

        private void btnStock_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Stock s = new Stock();
            s.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Shortage sh = new Shortage();
            sh.ShowDialog();
        }

        private void btnOp_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Operations o = new Operations();
            o.ShowDialog();
        }
    }
}
