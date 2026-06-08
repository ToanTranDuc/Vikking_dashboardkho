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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmNhapSoNo : DevExpress.XtraEditors.XtraForm
    {
        public static int soToNo = 0;
        public static bool isCheckHuy = true;
        public frmNhapSoNo()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void labelControl2_Click(object sender, EventArgs e)
        {
            if(textEdit1.EditValue == null || textEdit1.EditValue == "")
            {
                MessageBox.Show("Vui lòng không để trong số tờ no", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            isCheckHuy = true;
            soToNo = Convert.ToInt32(textEdit1.EditValue);
            this.Close();
        }

        private void labelControl3_Click(object sender, EventArgs e)
        {
            isCheckHuy = false;
            this.Close();
        }

        private void textEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                // Nếu ký tự không phải là số và không phải là ký tự điều khiển (ví dụ như backspace), hủy bỏ ký tự đó
                e.Handled = true;
            }
        }
    }
}