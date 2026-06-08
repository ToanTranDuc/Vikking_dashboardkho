using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
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
    public partial class frmNhapKhoAll : DevExpress.XtraEditors.XtraForm
    {
        private string _madhdt = string.Empty, _poiddt = string.Empty, _mapkldt = string.Empty, _tendvsxdt = string.Empty, 
        _malenhdt = string.Empty, _tenlenhdt = string.Empty, _mapkl = string.Empty;
        private int _sttthung = 0, _slkhdt = 0, _slthdt = 0, _slcldt = 0, _kiemtra = 0;

        private bool _IsDongThung = false;

        public int SoLuong { get; private set; }
        public string NgayNhapKho { get; private set; }
        public frmNhapKhoAll()
        {
            InitializeComponent();
            dateEdit1.EditValue = DateTime.Now;
        }

        public frmNhapKhoAll( List<DataRow> lst,string Size,bool IsDongThung, string mapkl, string tenlenhsx)
        {
            InitializeComponent();
        }

        private void IntitTitle(List<DataRow> lstDongThung,string Size,bool IsDongThung)
        {
        }
        private void txtSoLuong_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; 
            }
        }
        private void btnXacNhan_Click_1(object sender, EventArgs e)
        {
            if(dateEdit1.EditValue == null)
            {
                MessageBox.Show("Vui lòng chọn ngày nhập kho.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (Convert.ToDateTime(dateEdit1.EditValue).Date > Convert.ToDateTime(DateTime.Now).Date)
            {
                MessageBox.Show("Ngày nhập kho không được lớn hơn ngày hiện tại. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            NgayNhapKho = Convert.ToDateTime(dateEdit1.EditValue).ToString("yyyy-MM-dd hh:mm:ss");
            
            DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btnHuy_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
