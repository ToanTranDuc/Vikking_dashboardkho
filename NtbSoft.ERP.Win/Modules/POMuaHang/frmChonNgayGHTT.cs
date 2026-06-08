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
    public partial class frmChonNgayGHTT : DevExpress.XtraEditors.XtraForm
    {
        public string Ngay = string.Empty;
        public frmChonNgayGHTT()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(dateEdit1.Text))
            {
                XtraMessageBox.Show(
                    "Vui lòng nhập lý do kết thúc đơn sớm.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            DateTime ngayChon = dateEdit1.DateTime;
            Ngay = ngayChon.ToString("dd/MM/yyyy");
            this.DialogResult = DialogResult.OK;
            //this.Close();
        }
    }
}