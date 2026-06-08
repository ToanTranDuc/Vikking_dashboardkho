using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmThungXuat : DevExpress.XtraEditors.XtraForm
    {
        public static int tuthung;
        public static int denthung;
        int tuthungbd = 0;
        int denthungKT = 0;
        public frmThungXuat(int tuthung, int denthung)
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.tuthungbd = tuthung;
            this.denthungKT = denthung;
        }
        private void textEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }


        private void textEdit2_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textEdit1.EditValue == null || textEdit1.EditValue.ToString() == "" || textEdit2.EditValue == null || textEdit2.EditValue.ToString() == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ từ thùng hoặc đến thùng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            tuthung = Convert.ToInt32(textEdit1.EditValue);
            denthung = Convert.ToInt32(textEdit2.EditValue);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tuthung = 0;
            denthung = 0;
            this.Close();
        }



        private void textEdit1_Validated(object sender, EventArgs e)
        {
            try
            {
                if (textEdit1.EditValue == null || textEdit1.EditValue.ToString() == "") return;
                if (Convert.ToInt32(textEdit1.EditValue) < tuthungbd)
                {
                    MessageBox.Show("SL Thùng nhỏ hơn SL Thùng cho phép ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textEdit1.EditValue = "";
                    return;
                }
                if (Convert.ToInt32(textEdit1.EditValue) > denthungKT)
                {
                    MessageBox.Show("SL Thùng lớn hơn SL Thùng cho phép", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textEdit1.EditValue = "";
                    return;
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void textEdit2_Validated(object sender, EventArgs e)
        {
            try
            {

                if (textEdit2.EditValue == null) return;
                if (Convert.ToInt32(textEdit2.EditValue) < tuthungbd)
                {
                    MessageBox.Show("SL Thùng nhỏ hơn SL Thùng cho phép ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textEdit2.EditValue = null;
                    return;
                }
                if (Convert.ToInt32(textEdit2.EditValue) > denthungKT)
                {
                    MessageBox.Show("SL Thùng lớn hơn SL Thùng cho phép", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textEdit2.EditValue = null;
                    return;
                }
                if (Convert.ToInt32(textEdit2.EditValue) < Convert.ToInt32(textEdit2.EditValue))
                {
                    MessageBox.Show("SL đến thùng không được nhỏ hơn SL từ thùng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textEdit1.EditValue = null;
                    return;
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}
