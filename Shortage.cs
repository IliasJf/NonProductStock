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
    public partial class Shortage : Form
    {
        NonProductDataContext db = new NonProductDataContext();
        public Shortage()
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

        private void btHome_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Home h = new Home();
            h.ShowDialog();
        }

        private void btnOperations_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Operations o = new Operations();
            o.ShowDialog();
        }

        private void btnCompo_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Component c = new Component();
            c.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Home h = new Home();
            h.ShowDialog();
        }

        private void Shortage_Load(object sender, EventArgs e)
        {
            enabled(false);
            var shor = db.Short.Select(x => new { ComponentsCode = x.codeComponents, x.sumOfQuantityIN, x.sumOfQuantityOUT, x.stock,x.emgStock});
            dataGridView1.DataSource = shor;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            DataGridViewRow row = dataGridView1.Rows[i];
            txtCompoCode.Text = row.Cells[0].Value.ToString();
            var req = db.Sto.FirstOrDefault(x => x.codeComponents == txtCompoCode.Text);
            txtCompoCode.Text = req.codeComponents;
            txtDescr.Text = req.Description;
            txtMU.Text = req.uniteMeasure;
            txtQuanIN.Text = req.sumOfQuantityIN.ToString();
            txtQuanOUT.Text = req.sumOfQuantityOUT.ToString();
            txtSTOCK.Text = req.stock.ToString();
        }
        public void enabled(bool f)
        {
            txtCompoCode.Enabled = f;
            txtDescr.Enabled = f;
            txtMU.Enabled = f;
            txtQuanIN.Enabled = f;
            txtQuanOUT.Enabled = f;
            txtSTOCK.Enabled = f;
        }
    }
}
