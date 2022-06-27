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
    public partial class Operations : Form
    {
        NonProductDataContext db = new NonProductDataContext();
        public Operations()
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
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Home h = new Home();
            h.ShowDialog();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Home h = new Home();
            h.ShowDialog();
        }

        private void btnComponents_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Component c = new Component();
            c.ShowDialog();
        }

        private void Operations_Load(object sender, EventArgs e)
        {
            txtTime.Text = DateTime.Now.ToString();
            afficher();
            vider();
            enabled(false);
            btnOK.Visible = false;
            btnCANCEL.Visible = false;
            btnDelete.Visible = false;
            btnEdit.Visible = false;
            btnHome.Visible = true;
            btnSave.Visible = false;
            btnNewOP.Visible = true;
            btnStock.Visible = true;
            btnComponents.Visible = true;
        }

        private void btStock_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Stock s = new Stock();
            s.ShowDialog();
        }

        private void btShortage_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Shortage s = new Shortage();
            s.ShowDialog();
        }
        public void afficher()
        {
            var req = db.Opera.Select(x => new { x.idOperation, x.codeComponents, x.quantityIN, x.quantityOUT,x.currentStock, x.recipiendID, x.site, x.date, x.operationType }).OrderByDescending(y => y.idOperation);
            dataGridView1.DataSource = req;
            var cb = db.Compon.ToList();
            txtCompoCode.DataSource = cb;
            txtCompoCode.DisplayMember = "codeComponents";
            txtCompoCode.ValueMember = "codeComponents";
            var s = db.Sto.Select(y => new { ComponentCode = y.codeComponents, QuantityIN = y.sumOfQuantityIN, QuantityOUT = y.sumOfQuantityOUT });
            gridStock.DataSource = s;
            var shor = db.Short.Select(y => new { ComponentCode = y.codeComponents, y.stock, y.emgStock });
            gridShortage.DataSource = shor;

        }
        public void enabled(bool f)
        {
            txtCompoCode.Enabled = f;
            txtQuantityIN.Enabled = f;
            txtQuantityOUT.Enabled = f;
            txtRecipID.Enabled = f;
            cbOpType.Enabled = f;
            cbSite.Enabled = f;
        }
        public void vider()
        {
            txtCompoCode.SelectedIndex = -1;
            txtQuantityIN.Text = "0";
            txtQuantityOUT.Text = "0";
            txtRecipID.Text = "";
            txtTime.Text = DateTime.Now.ToString();
            cbOpType.SelectedIndex = -1;
            cbSite.SelectedIndex = -1;
            txtidOp.Text = "id";
            btnOK.Visible = false;
            btnCANCEL.Visible = false;
        }

        private void btnNewOP_Click(object sender, EventArgs e)
        {
            try
            {
                enabled(true);
                afficher();
                vider();
                btnOK.Visible = false;
                btnCANCEL.Visible = true;
                btnDelete.Visible = false;
                btnEdit.Visible = false;
                btnHome.Visible = false;
                btnSave.Visible = true;
                btnNewOP.Visible = false;
                btnStock.Visible = false;
                btnComponents.Visible = false;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCompoCode.Enabled == false) throw new Exception("Please click on button NEW to add new component");
                if (txtCompoCode.Text == "" || txtQuantityIN.Text == "" || txtQuantityOUT.Text == "" || txtRecipID.Text == "" || txtTime.Text == "" || cbOpType.SelectedIndex == -1 || cbSite.SelectedIndex == -1) throw new Exception("All fields are requiered");
                Opera o = new Opera();
                o.codeComponents = txtCompoCode.SelectedValue.ToString();
                o.quantityIN = int.Parse(txtQuantityIN.Text);
                o.quantityOUT = int.Parse(txtQuantityOUT.Text);
                o.currentStock = int.Parse(txtQuantityIN.Text) - int.Parse(txtQuantityOUT.Text);
                o.recipiendID = txtRecipID.Text;
                o.date = Convert.ToDateTime(txtTime.Text);
                o.operationType = cbOpType.SelectedItem.ToString();
                o.site = cbSite.SelectedItem.ToString();
                db.Opera.InsertOnSubmit(o);
                db.SubmitChanges();
                var s = db.Sto.FirstOrDefault(y => y.codeComponents == txtCompoCode.Text);
                var Qin = db.Opera.Where(t => t.codeComponents == txtCompoCode.Text).Sum(i => i.quantityIN).ToString();
                var Qout = db.Opera.Where(t => t.codeComponents == txtCompoCode.Text).Sum(i => i.quantityOUT).ToString();
                var unitM = db.Compon.Where(t => t.codeComponents == txtCompoCode.Text).Select(i => i.MeasureUnit).Single();
                var Des = db.Compon.Where(t => t.codeComponents == txtCompoCode.Text).Select(i => i.Description).Single();
                var emg=db.Compon.Where(t => t.codeComponents == txtCompoCode.Text).Select(i => i.EmgStock).Single();

                    s.sumOfQuantityIN = int.Parse(Qin);
                    s.sumOfQuantityOUT = int.Parse(Qout);
                    s.stock = int.Parse(Qin) - int.Parse(Qout);
                    s.uniteMeasure = unitM.ToString();
                    s.Description = Des.ToString();
                    db.SubmitChanges();

                var k = db.Short.FirstOrDefault(x => x.codeComponents == txtCompoCode.Text);
                if (k != null && s.stock < emg)
                {
                    k.sumOfQuantityIN = int.Parse(Qin);
                    k.sumOfQuantityOUT = int.Parse(Qout);
                    k.stock = int.Parse(Qin) - int.Parse(Qout);
                    k.emgStock = emg;
                    db.SubmitChanges();
                }
                else if (k != null && s.stock >= emg)
                {
                    db.Short.DeleteOnSubmit(k);
                    db.SubmitChanges();
                }
                else if (k == null && s.stock < emg)
                {
                    Short kk = new Short();
                    kk.codeComponents = txtCompoCode.Text;
                    kk.sumOfQuantityIN = int.Parse(Qin);
                    kk.sumOfQuantityOUT = int.Parse(Qout);
                    kk.stock = int.Parse(Qin) - int.Parse(Qout);
                    kk.emgStock = emg;
                    db.Short.InsertOnSubmit(kk);
                    db.SubmitChanges();
                }
                afficher();
                enabled(false);
                btnOK.Visible = false;
                btnCANCEL.Visible = false;
                btnDelete.Visible = false;
                btnEdit.Visible = false;
                btnHome.Visible = true;
                btnSave.Visible = false;
                btnNewOP.Visible = true;
                btnStock.Visible = true;
                btnComponents.Visible = true;
                MessageBox.Show("Added successfully");
                vider();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            vider();
            enabled(false);
            btnOK.Visible = false;
            btnCANCEL.Visible = true;
            btnDelete.Visible = true;
            btnEdit.Visible = true;
            btnHome.Visible = false;
            btnSave.Visible = false;
            btnNewOP.Visible = false;
            btnStock.Visible = false;
            btnComponents.Visible = false;
            int i = e.RowIndex;
            DataGridViewRow row = dataGridView1.Rows[i];
            txtidOp.Text = row.Cells[0].Value.ToString();
            var req = db.Opera.FirstOrDefault(x => x.idOperation == int.Parse(txtidOp.Text));
            txtCompoCode.SelectedValue = req.codeComponents;
            txtQuantityIN.Text = req.quantityIN.ToString();
            txtQuantityOUT.Text = req.quantityOUT.ToString();
            txtRecipID.Text = req.recipiendID;
            txtTime.Text = req.date.ToString();
            cbOpType.SelectedItem = req.operationType;
            cbSite.SelectedItem = req.site;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtidOp.Text == "id") throw new Exception("Selected component from table is required.");
                var req = db.Opera.FirstOrDefault(x => x.idOperation == int.Parse(txtidOp.Text));
                db.Opera.DeleteOnSubmit(req);
                db.SubmitChanges();

                var s = db.Sto.FirstOrDefault(y => y.codeComponents == txtCompoCode.Text);
                var Qin = db.Opera.Where(t => t.codeComponents == txtCompoCode.Text).Sum(i => i.quantityIN).ToString();
                var Qout = db.Opera.Where(t => t.codeComponents == txtCompoCode.Text).Sum(i => i.quantityOUT).ToString();
                var unitM = db.Compon.Where(t => t.codeComponents == txtCompoCode.Text).Select(i => i.MeasureUnit).Single();
                var Des = db.Compon.Where(t => t.codeComponents == txtCompoCode.Text).Select(i => i.Description).Single();
                var emg = db.Compon.Where(t => t.codeComponents == txtCompoCode.Text).Select(i => i.EmgStock).Single();

                s.sumOfQuantityIN = int.Parse(Qin);
                s.sumOfQuantityOUT = int.Parse(Qout);
                s.stock = int.Parse(Qin) - int.Parse(Qout);
                s.uniteMeasure = unitM.ToString();
                s.Description = Des.ToString();
                db.SubmitChanges();

                var k = db.Short.FirstOrDefault(x => x.codeComponents == txtCompoCode.Text);
                if (k != null && s.stock < emg)
                {
                    k.sumOfQuantityIN = int.Parse(Qin);
                    k.sumOfQuantityOUT = int.Parse(Qout);
                    k.stock = int.Parse(Qin) - int.Parse(Qout);
                    k.emgStock = emg;
                    db.SubmitChanges();
                }
                else if (k != null && s.stock >= emg)
                {
                    db.Short.DeleteOnSubmit(k);
                    db.SubmitChanges();
                }
                else if (k == null && s.stock < emg)
                {
                    Short kk = new Short();
                    kk.codeComponents = txtCompoCode.Text;
                    kk.sumOfQuantityIN = int.Parse(Qin);
                    kk.sumOfQuantityOUT = int.Parse(Qout);
                    kk.stock = int.Parse(Qin) - int.Parse(Qout);
                    kk.emgStock = emg;
                    db.Short.InsertOnSubmit(kk);
                    db.SubmitChanges();
                }

                afficher();
                vider();
                enabled(true);
                MessageBox.Show("Deleted successfully");
                btnOK.Visible = false;
                btnCANCEL.Visible = false;
                btnDelete.Visible = false;
                btnEdit.Visible = false;
                btnHome.Visible = true;
                btnSave.Visible = false;
                btnNewOP.Visible = true;
                btnStock.Visible = true;
                btnComponents.Visible = true;

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCompoCode.Text == "") throw new Exception("Selected component from table is required.");
                enabled(true);
                txtCompoCode.Enabled = false;
                btnOK.Visible = true;
                btnCANCEL.Visible = true;
                btnDelete.Visible = false;
                btnEdit.Visible = false;
                btnHome.Visible = false;
                btnSave.Visible = false;
                btnNewOP.Visible = false;
                btnStock.Visible = false;
                btnComponents.Visible = false;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                var o = db.Opera.FirstOrDefault(x => x.idOperation == int.Parse(txtidOp.Text));
                if (txtCompoCode.Text == "" || txtQuantityIN.Text == "" || txtQuantityOUT.Text == "" || txtRecipID.Text == "" || txtTime.Text == "" || cbOpType.SelectedIndex == -1 || cbSite.SelectedIndex == -1) throw new Exception("All fields are requiered");
                o.codeComponents = txtCompoCode.SelectedValue.ToString();
                o.quantityIN = int.Parse(txtQuantityIN.Text);
                o.quantityOUT = int.Parse(txtQuantityOUT.Text);
                o.currentStock = int.Parse(txtQuantityIN.Text) - int.Parse(txtQuantityOUT.Text);
                o.recipiendID = txtRecipID.Text;
                o.date = Convert.ToDateTime(txtTime.Text);
                o.operationType = cbOpType.SelectedItem.ToString();
                o.site = cbSite.SelectedItem.ToString();
                db.SubmitChanges();
                var s = db.Sto.FirstOrDefault(y => y.codeComponents == txtCompoCode.Text);
                var Qin = db.Opera.Where(t => t.codeComponents == txtCompoCode.Text).Sum(i => i.quantityIN).ToString();
                var Qout = db.Opera.Where(t => t.codeComponents == txtCompoCode.Text).Sum(i => i.quantityOUT).ToString();
                var unitM = db.Compon.Where(t => t.codeComponents == txtCompoCode.Text).Select(i => i.MeasureUnit).Single();
                var Des = db.Compon.Where(t => t.codeComponents == txtCompoCode.Text).Select(i => i.Description).Single();
                var emg = db.Compon.Where(t => t.codeComponents == txtCompoCode.Text).Select(i => i.EmgStock).Single();

                s.sumOfQuantityIN = int.Parse(Qin);
                    s.sumOfQuantityOUT = int.Parse(Qout);
                    s.stock = int.Parse(Qin) - int.Parse(Qout);
                    s.uniteMeasure = unitM.ToString();
                    s.Description = Des.ToString();
                    db.SubmitChanges();

                var k = db.Short.FirstOrDefault(x => x.codeComponents == txtCompoCode.Text);
                if (k != null && s.stock < emg)
                {
                    k.sumOfQuantityIN = int.Parse(Qin);
                    k.sumOfQuantityOUT = int.Parse(Qout);
                    k.stock = int.Parse(Qin) - int.Parse(Qout);
                    k.emgStock = emg;
                    db.SubmitChanges();
                }
                else if (k != null && s.stock >= emg)
                {
                    db.Short.DeleteOnSubmit(k);
                    db.SubmitChanges();
                }
                else if (k == null && s.stock < emg)
                {
                    Short kk = new Short();
                    kk.codeComponents = txtCompoCode.Text;
                    kk.sumOfQuantityIN = int.Parse(Qin);
                    kk.sumOfQuantityOUT = int.Parse(Qout);
                    kk.stock = int.Parse(Qin) - int.Parse(Qout);
                    kk.emgStock = emg;
                    db.Short.InsertOnSubmit(kk);
                    db.SubmitChanges();
                }

                afficher();
                vider();
                btnOK.Visible = false;
                btnCANCEL.Visible = false;
                btnDelete.Visible = false;
                btnEdit.Visible = false;
                btnHome.Visible = true;
                btnSave.Visible = false;
                btnNewOP.Visible = true;
                btnStock.Visible = true;
                btnComponents.Visible = true;
                btnComponents.Visible = true;
                MessageBox.Show("Edited successfully");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }
        }

        private void btnCANCEL_Click(object sender, EventArgs e)
        {
            try
            {                
                afficher();
                vider();
                enabled(true);
                txtCompoCode.Enabled = false;
                txtQuantityIN.Enabled = false;
                txtQuantityOUT.Enabled = false;
                txtRecipID.Enabled = false;
                cbOpType.Enabled = false;
                cbSite.Enabled = false;
                btnOK.Visible = false;
                btnCANCEL.Visible = false;
                btnDelete.Visible = false;
                btnEdit.Visible = false;
                btnHome.Visible = true;
                btnSave.Visible = false;
                btnNewOP.Visible = true;
                btnStock.Visible = true;
                btnComponents.Visible = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }
        }
        public void shortage()
        {
            
        }

        private void btnStock_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Stock s = new Stock();
            s.ShowDialog();
        }

        private void cbOpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbOpType.SelectedIndex == 0)
            {
                txtQuantityIN.Enabled = true;
                txtQuantityOUT.Enabled = false;
            }
            else if (cbOpType.SelectedIndex == 1)
            {
                txtQuantityIN.Enabled = false;
                txtQuantityOUT.Enabled = true;
            }
            else if(cbOpType.SelectedIndex == 2)
            {
                txtQuantityIN.Enabled = true;
                txtQuantityOUT.Enabled = true;
            }
        }
    }
}
