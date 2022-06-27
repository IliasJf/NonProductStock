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
    public partial class Login : Form
    {
        NonProductDataContext db = new NonProductDataContext();

        public Login()
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

        private void btnLog_Click(object sender, EventArgs e)
        {
            try
            {
                var req = db.Users.FirstOrDefault(x => x.username == txtUSERNAME.Text&&x.password==txtPASS.Text);
                if (req == null) throw new Exception("Username/Password is incorrect.");
                this.Hide();
                Home h = new Home();
                h.ShowDialog();
                txtUSERNAME.Text = "";
                txtPASS.Text = "";
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Erreur");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
