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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmNGCKetThucPMHSom : DevExpress.XtraEditors.XtraForm
    {
        public string GhiChu = string.Empty;
        public frmNGCKetThucPMHSom()
        {
            InitializeComponent();
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textEdit11.Text))
            {
                XtraMessageBox.Show(
                    "Vui lòng nhập lý do",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            GhiChu = textEdit11.Text.Trim();
            this.DialogResult = DialogResult.OK;
            //this.Close();
        }
    }
}