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
    public partial class frmNhapKhoSoLuong : DevExpress.XtraEditors.XtraForm
    {
        private string _madhdt = string.Empty, _poiddt = string.Empty, _mapkldt = string.Empty, _tendvsxdt = string.Empty, 
        _malenhdt = string.Empty, _tenlenhdt = string.Empty, _mapkl = string.Empty, _po = string.Empty;
        private int _sttthung = 0, _slkhdt = 0, _slthdt = 0, _slcldt = 0, _kiemtra = 0, _dt = 0;
        private bool _IsDongThung = false;
        public int SoLuong { get; private set; }
        public string NgayNhapKho { get; private set; }
        public frmNhapKhoSoLuong()
        {
            InitializeComponent();
           
        }

        public frmNhapKhoSoLuong( List<DataRow> lst,string Size,bool IsDongThung, string mapkl, string tenlenhsx, int dt, string po)
        {
            InitializeComponent();
            this._IsDongThung = IsDongThung;
            this._tenlenhdt = tenlenhsx;
            this._mapkl = mapkl;
            this._dt = dt;
            this._po = po;
            if (_dt == 0)
                layoutControlItem14.Visibility = LayoutVisibility.Never;
            else
                layoutControlItem14.Visibility = LayoutVisibility.Always;
            dateEdit1.EditValue = DateTime.Now;
            IntitTitle(lst, Size, IsDongThung);
        }

        private void IntitTitle(List<DataRow> lstDongThung,string Size,bool IsDongThung)
        {
            txtPKL.Text = _mapkl;
            txtLenhSX.Text = _tenlenhdt.ToString();
            txtCartonNumber.Text = lstDongThung[0]["TuThung"].ToString() + "->" + lstDongThung[0]["DenThung"].ToString();
            txtLenhSX.Text = _tenlenhdt.ToString();
            txtPOID.Text = _po;//lstDongThung[0]["PO"].ToString();
            txtDonViSX.Text = lstDongThung[0]["TenDVSX"].ToString();
            txtDauSize.Text = string.Join(";", lstDongThung.Select(row => row["DauSize"].ToString()));
            txtMau.Text = string.Join(";", lstDongThung.Select(row => row["TenMau"].ToString()));
            txtSize.Text = Size;
            txtSLNhap.Text = lstDongThung[0]["SLNhap"].ToString();
            if(IsDongThung)
            {
                layoutControlItem12.Text = "Số thùng đã đóng/Tổng thùng";
            }    
            else
            {
                layoutControlItem12.Text = "Số thùng đã nhập/Tổng thùng";
            }    
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
            if(_dt != 0)
            {
                if (dateEdit1.EditValue == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày nhập kho.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }    

            if(txtSoLuong.Text.ToString() == "")
            {
                MessageBox.Show("Vui lòng nhập số lượng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }    
            SoLuong = Convert.ToInt32(txtSoLuong.Text);
            if (_dt != 0)
            {
                if (Convert.ToDateTime(dateEdit1.EditValue).Date > Convert.ToDateTime(DateTime.Now).Date)
                {
                    MessageBox.Show("Ngày nhập kho không được lớn hơn ngày hiện tại. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                NgayNhapKho = Convert.ToDateTime(dateEdit1.EditValue).ToString("yyyy-MM-dd hh:mm:ss");
              
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btnHuy_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
