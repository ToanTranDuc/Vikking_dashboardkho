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
    public partial class frmNhapViTri : DevExpress.XtraEditors.XtraForm
    {
        private string _madhdt = string.Empty, _poiddt = string.Empty, _mapkldt = string.Empty, _tendvsxdt = string.Empty,
        _malenhdt = string.Empty, _tenlenhdt = string.Empty, _mapkl = string.Empty, _po = string.Empty;
        private int _sttthung = 0, _slkhdt = 0, _slthdt = 0, _slcldt = 0, _kiemtra = 0, _tuThung = 0, _denThung = 0, _dt = 0;
        private bool _IsDongThung = false;

        public int TuThung { get; private set; }
        public int DenThung { get; private set; }
        public string NgayNhapKho { get; private set; }
        public frmNhapViTri()
        {
            InitializeComponent();

        }

        public frmNhapViTri(List<DataRow> lst, string Size, bool IsDongThung, string mapkl, string tenlenhsx, int dt, string po)
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
            dateEditNgayNK.EditValue = DateTime.Now;
            IntitTitle(lst, Size, IsDongThung);
        }
        private void IntitTitle(List<DataRow> lstDongThung, string Size, bool IsDongThung)
        {
            txtPKL.Text = _mapkl;
            txtLenhSX.Text = _tenlenhdt.ToString();
            txtCartonNumber.Text = lstDongThung[0]["MinSttThung"].ToString() + "->" + lstDongThung[0]["MaxSttThung"].ToString();
            txtLenhSX.Text = _tenlenhdt.ToString();
            txtPOID.Text = _po;//lstDongThung[0]["PO"].ToString();
            txtDonViSX.Text = lstDongThung[0]["TenDVSX"].ToString();
            txtDauSize.Text = string.Join(";", lstDongThung.Select(row => row["DauSize"].ToString()));
            txtMau.Text = string.Join(";", lstDongThung.Select(row => row["TenMau"].ToString()));
            txtSize.Text = Size;
            txtSLNhap.Text = lstDongThung[0]["SLNhap"].ToString();
            if (IsDongThung)
            {
                layoutControlItem13.Text = "Số thùng đã đóng/Tổng thùng";
            }
            else
            {
                layoutControlItem13.Text = "Số thùng đã nhập/Tổng thùng";
            }
            _tuThung = Convert.ToInt32(lstDongThung[0]["MinSttThung"]);
            _denThung = Convert.ToInt32(lstDongThung[0]["MaxSttThung"]);
        }
        private void btnHuy_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnXacNhan_Click_1(object sender, EventArgs e)
        {
            if (_dt != 0)
            {
                if (dateEditNgayNK.EditValue == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày nhập kho.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            if (txtTuThung.Text.ToString() == ""|| txtDenThung.Text.ToString() == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin Từ Thùng và Đến Thùng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }    
            TuThung = Convert.ToInt32(txtTuThung.Text);
            DenThung = Convert.ToInt32(txtDenThung.Text);
            if(TuThung < _tuThung)
            {
                MessageBox.Show("Từ Thùng không được nhỏ hơn " + _tuThung+ ".!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;

            }
            if (DenThung < _tuThung)
            {
                MessageBox.Show("Đến Thùng phải lớn hơn Từ Thùng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;

            }
            if (DenThung > _denThung)
            {
                MessageBox.Show("Đến Thùng không được vướt quá " + _denThung + ".!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (_dt != 0)
            {
                if (Convert.ToDateTime(dateEditNgayNK.EditValue).Date > Convert.ToDateTime(DateTime.Now).Date)
                {
                    MessageBox.Show("Ngày nhập kho không được lớn hơn ngày hiện tại. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                NgayNhapKho = Convert.ToDateTime(dateEditNgayNK.EditValue).ToString("yyyy-MM-dd hh:mm:ss");
            }
            DialogResult = DialogResult.OK;
            this.Close();

        }
        private void spinEditSL_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

    }
}
