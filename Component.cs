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
    public partial class Component : Form
    {
        NonProductDataContext db = new NonProductDataContext();
        public Component()
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

        private void Component_Load(object sender, EventArgs e)
        {
            enabled(false);
            vider();
            afficher();
            RemplirCb();
        }
        public void afficher()
        {
            var req = db.Compon.Select(x => new {x.codeComponents,x.SupplierName,x.UnitPrice,x.Type,x.MeasureUnit,x.EmgStock,x.PoStatus,x.Status,x.Description });
            dataGridView1.DataSource = req;
        }

        private void btnNEW_Click(object sender, EventArgs e)
        {
            try
            {
                enabled(true);
                afficher();
                vider();
                btnCANCEL.Visible = true;
                btnOK.Visible = false;
                btnDELETE.Visible = false;
                btnEdit.Visible = false;
                btnHome.Visible = false;
                btnSAVE.Visible = true;
                btnNEW.Visible = false;
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }
        }

        private void btnSAVE_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCompoCode.Enabled == false) throw new Exception("Please click on button NEW to add new component");
                if (txtCompoCode.Text == "" || txtDescription.Text == "" ||txtEmgStock.Text == "" ||txtUnitePrice.Text == "" ||cbMeasureUnit.SelectedIndex == -1 ||cbPOstatus.SelectedIndex == -1 ||cbStatus.SelectedIndex == -1 ||cbSuppName.SelectedIndex == -1 ||cbType.SelectedIndex == -1) throw new Exception("All fields are requiered");
                var d = db.Compon.FirstOrDefault(x => x.codeComponents == txtCompoCode.Text);
                if (d != null) throw new Exception("Components code already exist.");
                Compon c = new Compon();
                c.Commentary = txtCommentary.Text;
                c.codeComponents = txtCompoCode.Text;
                c.Description = txtDescription.Text;
                c.EmgStock = int.Parse(txtEmgStock.Text);
                c.UnitPrice = Convert.ToDecimal(txtUnitePrice.Text);
                c.MeasureUnit = cbMeasureUnit.SelectedItem.ToString();
                c.PoStatus = cbPOstatus.SelectedItem.ToString();
                c.Status = cbStatus.SelectedItem.ToString();
                c.SupplierName = cbSuppName.SelectedItem.ToString();
                c.Type = cbType.SelectedItem.ToString();
                db.Compon.InsertOnSubmit(c);
                db.SubmitChanges();

                var s = db.Sto.FirstOrDefault(y => y.codeComponents == txtCompoCode.Text);
                Sto st = new Sto();
                if (s == null)
                {
                    st.codeComponents = txtCompoCode.Text;
                    st.sumOfQuantityIN = 0;
                    st.sumOfQuantityOUT = 0;
                    st.stock = 0;
                    st.uniteMeasure = cbMeasureUnit.SelectedItem.ToString();
                    st.Description = txtDescription.Text;
                    db.Sto.InsertOnSubmit(st);
                    db.SubmitChanges();
                }
                enabled(false);
                btnCANCEL.Visible = false;
                btnOK.Visible = false;
                btnDELETE.Visible = false;
                btnEdit.Visible = false;
                btnHome.Visible = true;
                btnSAVE.Visible = false;
                btnNEW.Visible = true;
                afficher();
                RemplirCb();
                MessageBox.Show("Added successfully");
                vider();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }
        }
        public void enabled(bool f)
        {
            txtCommentary.Enabled = f;
            txtCompoCode.Enabled = f;
            txtDescription.Enabled = f;
            txtEmgStock.Enabled = f;
            txtUnitePrice.Enabled = f;
            cbMeasureUnit.Enabled = f;
            cbPOstatus.Enabled = f;
            cbStatus.Enabled = f;
            cbSuppName.Enabled = f;
            cbType.Enabled = f;
        }
        public void vider()
        {
            txtCommentary.Text = "";
            txtCompoCode.Text = "";
            txtDescription.Text = "";
            txtEmgStock.Text = "";
            txtUnitePrice.Text = "";
            cbMeasureUnit.SelectedIndex = -1;
            cbPOstatus.SelectedIndex = -1;
            cbSearch.SelectedIndex = -1;
            cbStatus.SelectedIndex = -1;
            cbSuppName.SelectedIndex = -1;
            cbType.SelectedIndex = -1;
            btnCANCEL.Visible = false;
            btnOK.Visible = false;
            btnDELETE.Visible = false;
            btnEdit.Visible = false;
            btnHome.Visible = true;
            btnSAVE.Visible = false;
            btnNEW.Visible = true;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            vider();
            enabled(false);
            btnOK.Visible = false;
            btnCANCEL.Visible = true;
            btnDELETE.Visible = true;
            btnEdit.Visible = true;
            btnHome.Visible = false;
            btnSAVE.Visible = false;
            btnNEW.Visible = false;
            int i = e.RowIndex;
            DataGridViewRow row = dataGridView1.Rows[i];
            txtCompoCode.Text = row.Cells[0].Value.ToString();
            var req= db.Compon.FirstOrDefault(x => x.codeComponents == txtCompoCode.Text);
            txtCommentary.Text = req.Commentary;
            txtCompoCode.Text = req.codeComponents;
            txtDescription.Text = req.Description;
            txtEmgStock.Text = req.EmgStock.ToString();
            txtUnitePrice.Text = req.UnitPrice.ToString();
            cbMeasureUnit.SelectedItem = req.MeasureUnit;
            cbPOstatus.SelectedItem = req.PoStatus;
            cbStatus.SelectedItem = req.Status;
            cbSuppName.SelectedItem = req.SupplierName;
            cbType.SelectedItem = req.Type;
        }

        private void btnDELETE_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCompoCode.Text == "") throw new Exception("Selected component from table is required.");
                var req = db.Compon.FirstOrDefault(x => x.codeComponents == txtCompoCode.Text);
                db.Compon.DeleteOnSubmit(req);
                db.SubmitChanges();
                vider();
                afficher();
                RemplirCb();
                enabled(false);
                btnOK.Visible = false;
                btnCANCEL.Visible = false;
                btnDELETE.Visible = false;
                btnEdit.Visible = false;
                btnHome.Visible = true;
                btnSAVE.Visible = false;
                btnNEW.Visible = true;
                MessageBox.Show("Deleted successfully");

            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }
        }

        private void cbSearch_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var req = db.Compon.Where(y => y.codeComponents == cbSearch.Text).Select(x => new { x.codeComponents, x.SupplierName, x.UnitPrice, x.Type, x.MeasureUnit, x.EmgStock, x.PoStatus, x.Status, x.Description });
            dataGridView1.DataSource = req;
        }
        public void RemplirCb()
        {
            var cb = db.Compon.Select(x=>new {x.codeComponents}).Distinct();
            cbSearch.DataSource = cb;
            cbSearch.DisplayMember = "codeComponents";
            cbSearch.SelectedIndex = -1;
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
                btnDELETE.Visible = false;
                btnEdit.Visible = false;
                btnHome.Visible = false;
                btnSAVE.Visible = false;
                btnNEW.Visible = false;
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                var c = db.Compon.FirstOrDefault(x => x.codeComponents == txtCompoCode.Text);
                if (txtCompoCode.Text == "" || txtDescription.Text == "" || txtEmgStock.Text == "" || txtUnitePrice.Text == "" || cbMeasureUnit.SelectedIndex == -1 || cbPOstatus.SelectedIndex == -1 || cbStatus.SelectedIndex == -1 || cbSuppName.SelectedIndex == -1 || cbType.SelectedIndex == -1) throw new Exception("All fields are requiered");
                c.Commentary = txtCommentary.Text;
                c.codeComponents = txtCompoCode.Text;
                c.Description = txtDescription.Text;
                c.EmgStock = int.Parse(txtEmgStock.Text);
                c.UnitPrice = Convert.ToDecimal(txtUnitePrice.Text);
                c.MeasureUnit = cbMeasureUnit.SelectedItem.ToString();
                c.PoStatus = cbPOstatus.SelectedItem.ToString();
                c.Status = cbStatus.SelectedItem.ToString();
                c.SupplierName = cbSuppName.SelectedItem.ToString();
                c.Type = cbType.SelectedItem.ToString();
                db.SubmitChanges();
                vider();
                afficher();
                enabled(false);
                btnOK.Visible = false;
                btnCANCEL.Visible = false;
                btnDELETE.Visible = false;
                btnEdit.Visible = false;
                btnHome.Visible = true;
                btnSAVE.Visible = false;
                btnNEW.Visible = true;
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
                vider();
                afficher();
                enabled(false);
                btnOK.Visible = false;
                btnCANCEL.Visible = false;
                btnDELETE.Visible = false;
                btnEdit.Visible = false;
                btnHome.Visible = true;
                btnSAVE.Visible = false;
                btnNEW.Visible = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }

        }
    }
}
