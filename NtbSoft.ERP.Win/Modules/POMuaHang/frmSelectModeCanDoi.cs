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
    public partial class frmSelectModeCanDoi : DevExpress.XtraEditors.XtraForm
    {
        public bool IsCanDoiVatTu { get; private set; }
        public bool IsCanDoiDonHang { get; private set; }

        public frmSelectModeCanDoi()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!checkEdit1.Checked && !checkEdit2.Checked)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một phương thức cân đối.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            IsCanDoiVatTu = checkEdit1.Checked;
            IsCanDoiDonHang = checkEdit2.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (!checkEdit1.Checked && !checkEdit2.Checked)
            {
                MessageBox.Show("Vui lòng chọn một phương thức cân đối.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            IsCanDoiVatTu = checkEdit1.Checked;
            IsCanDoiDonHang = checkEdit2.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit1.Checked) checkEdit2.Checked = false;
        }

        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit2.Checked) checkEdit1.Checked = false;
        }
    }
}