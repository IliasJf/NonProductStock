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
    public partial class Stock : Form
    {
        NonProductDataContext db = new NonProductDataContext();
        public Stock()
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

        public void vider()
        {
            txtCompoCode.SelectedIndex = -1;
            txtDescr.Text = "";
            txtMU.Text = "";
            txtQuanIN.Text = "";
            txtQuanOUT.Text = "";
            txtSTOCK.Text = "";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Home h = new Home();
            h.ShowDialog();
        }

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
        private void btnCompo_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
            Component c = new Component();
            c.ShowDialog();
        }
        public void afficher()
        {
            var req = db.Sto.Select(x => new { ComponentsCode = x.codeComponents, x.sumOfQuantityIN, x.sumOfQuantityOUT, x.stock, x.uniteMeasure});
            dataGridView1.DataSource = req;
            var cb = db.Compon.Select(x => new { x.codeComponents });
            txtCompoCode.DataSource = cb;
            txtCompoCode.DisplayMember = "codeComponents";
            txtCompoCode.ValueMember= "codeComponents";
            var cbb = db.Compon.Select(x => new { x.codeComponents });
            comboBox1.DataSource = cbb;
            comboBox1.DisplayMember = "codeComponents";
            comboBox1.ValueMember = "codeComponents";
            comboBox1.SelectedIndex = -1;
            txtCompoCode.SelectedIndex = -1;
        }

        private void btnNewStock_Click(object sender, EventArgs e)
        {
            afficher();
            enabled(true);
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

        private void Stock_Load(object sender, EventArgs e)
        {
            enabled(false);
            afficher();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            enabled(false);
            int i = e.RowIndex;
            DataGridViewRow row = dataGridView1.Rows[i];
            txtCompoCode.SelectedValue = row.Cells[0].Value.ToString();
            var req = db.Sto.FirstOrDefault(x => x.codeComponents == txtCompoCode.Text);
            txtCompoCode.Text = req.codeComponents;
            txtDescr.Text = req.Description;
            txtMU.Text = req.uniteMeasure;
            txtQuanIN.Text = req.sumOfQuantityIN.ToString();
            txtQuanOUT.Text = req.sumOfQuantityOUT.ToString();
            txtSTOCK.Text = req.stock.ToString();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var req = db.Sto.Where(y => y.codeComponents == comboBox1.SelectedValue.ToString()).Select(x => new { ComponentsCode = x.codeComponents, x.sumOfQuantityIN, x.sumOfQuantityOUT, x.stock, x.uniteMeasure});
            dataGridView1.DataSource = req;
        }
    }
}
