using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Utils;
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
    public partial class frmNhapKhoBarCode : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private string _madhdt = string.Empty, _poiddt = string.Empty, _mapkldt = string.Empty, _tendvsxdt = string.Empty,
        _malenhdt = string.Empty, _tenlenhdt = string.Empty, _mapkl = string.Empty;
        private int _sttthung = 0, _slkhdt = 0, _slthdt = 0, _slcldt = 0, _kiemtra = 0, _dt = 0;
        private HttpClientExtension _clientExtension;
        private bool _IsDongThung = false;
        public int SoLuong { get; private set; }
        public int TuThung { get; private set; }
        public int DenThung { get; private set; }
        public string NgayNhapKho { get; private set; }

        public DataTable DtBarCode { get; private set; }

        private DataTable _dtNhapKho;
        private DataRow selectedRows;
        private bool _sms = false, _chkNhap = false;
        private int _lcsx = 0;
        public string _madvsx = string.Empty, _kho = string.Empty, _maday = string.Empty, _make = string.Empty, _matang = string.Empty, _mao = string.Empty;
        private bool _isNhapKho = false;
        private decimal _cbm = 0;
        public frmNhapKhoBarCode(DataTable dt, bool sms, bool lcsx, string kho, bool isNhapKho, bool chkNhap, string maday, string make, string matang, string mao, decimal cbm, string madvsx)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _dtNhapKho = dt;
            this.ActiveControl = txtBarCode;
            dateEdit1.EditValue = DateTime.Now;
            this._sms = sms;
            this._kho = kho.ToString();
            this._isNhapKho = isNhapKho;
            this._chkNhap = chkNhap;
            this._maday = maday;
            this._make = make;
            this._matang = matang;
            this._mao = mao;
            this._cbm = cbm;
            this._madvsx = madvsx;
            if (lcsx)
            {
                _lcsx = 1;
            }

        }
        private void LoadCBM()
        {
            string url = string.Format("{0}?madvsx={1}&&makho={2}&&maday={3}&&make={4}&&matang={5}&&mao={6}", URL + "NhapKho/GetCBM", _madvsx, _kho == null ? "" : _kho.ToString(),
               _maday == null ? "" : _maday.ToString(),
               _make == null ? "" : _make.ToString(),
               _matang == null ? "" : _matang.ToString(),
               _mao == null ? "" : _mao.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _dtCBM = JsonConvert.DeserializeObject<DataTable>(json);
            if (_dtCBM.Rows.Count > 0)
            {
                _cbm = Convert.ToInt32(_dtCBM.Rows[0][1]);
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
            if (_dt != 0)
            {
                if (dateEdit1.EditValue == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày nhập kho.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            if (txtSoLuong.Text.ToString() == "")
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
            if (_isNhapKho)
            {
                if (DtBarCode == null) return;
                DataTable _dtVTK = new DataTable();
                int i = 0;
                if (SoLuong <= 0) return;
                DataTable dtSave = new DataTable();
                dtSave.Columns.Add("MaDH", typeof(string));
                dtSave.Columns.Add("MaPKL", typeof(string));
                dtSave.Columns.Add("MaDVSX", typeof(string));
                dtSave.Columns.Add("MaLenh", typeof(string));
                dtSave.Columns.Add("POID", typeof(string));
                dtSave.Columns.Add("MaKho", typeof(string));
                dtSave.Columns.Add("MaKhoDen", typeof(string));
                dtSave.Columns.Add("TuThung", typeof(int));
                dtSave.Columns.Add("DenThung", typeof(int));
                dtSave.Columns.Add("MinSttThung", typeof(int));
                dtSave.Columns.Add("MaxSttThung", typeof(int));
                dtSave.Columns.Add("SttThungDecat", typeof(int));
                dtSave.Columns.Add("NgayNhapKho_TC", typeof(string));
                if (!_sms)
                {
                    foreach (DataRow row in DtBarCode.Rows)
                    {
                        if (i == SoLuong)
                            break;
                        if (Convert.ToInt32(row["CheckDT"]) == 0)
                        {
                            DataRow _dr = dtSave.NewRow();
                            _dr["MaDH"] = _dr["MaDH"];
                            _dr["MaPKL"] = row["MaPKL"];
                            _dr["MaLenh"] = row["MaLenh"];
                            _dr["MaDVSX"] = row["MaDVSX"];
                            _dr["POID"] = row["POID"];
                            _dr["MaKho"] = row["MaKho"] == "" ? _kho.ToString() : row["MaKho"].ToString();
                            _dr["MaKhoDen"] = _kho.ToString();
                            _dr["TuThung"] = TuThung;
                            _dr["DenThung"] = DenThung;
                            _dr["MinSttThung"] = 0;
                            _dr["MaxSttThung"] = 0;
                            _dr["SttThungDecat"] = row["SttDT"];
                            _dr["NgayNhapKho_TC"] = NgayNhapKho;
                            dtSave.Rows.Add(_dr);
                            i++;
                        }
                    }
                }
                else
                {
                    foreach (DataRow row in DtBarCode.Rows)
                    {
                        if (Convert.ToInt32(row["CheckDT"]) == 0)
                        {
                            DataRow _dr = dtSave.NewRow();
                            _dr["MaDH"] = _dr["MaDH"];
                            _dr["MaPKL"] = row["MaPKL"];
                            _dr["MaLenh"] = row["MaLenh"];
                            _dr["MaDVSX"] = row["MaDVSX"];
                            _dr["POID"] = row["POID"];
                            _dr["MaKho"] = row["MaKho"] == "" ? _kho.ToString() : row["MaKho"].ToString();
                            _dr["MaKhoDen"] = _kho.ToString();
                            _dr["TuThung"] = TuThung;
                            _dr["DenThung"] = DenThung;
                            _dr["MinSttThung"] = 0;
                            _dr["MaxSttThung"] = 0;
                            _dr["SttThungDecat"] = row["SttDT"];
                            _dr["NgayNhapKho_TC"] = NgayNhapKho;
                            dtSave.Rows.Add(_dr);
                        }
                    }
                }
                if (dtSave == null || dtSave.Rows.Count == 0)
                {
                    MessageBox.Show("Không có thùng nào để nhập kho. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (_chkNhap)
                {
                    string urlVTK = string.Format("{0}?", URL + "NhapKho/GetVTKV1");
                    string jsonVTK = Task.Run(async () => { return await _clientExtension.PostAsync(urlVTK, dtSave); }).Result;
                    _dtVTK = JsonConvert.DeserializeObject<DataTable>(jsonVTK);
                    bool isEmpty = !_dtVTK.AsEnumerable()
                              .Any(row => row["CBM"] != null &&
                                           row["CBM"] != "" &&
                                           row["CBM"] != DBNull.Value.ToString());

                    if (isEmpty)
                    {
                        XtraMessageBox.Show("Vui lòng khai báo CBM cho thùng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    decimal total = Math.Round(_dtVTK.AsEnumerable().Sum(row => Convert.ToDecimal(row["CBM"])), 4);
                    if (total > _cbm)
                    {
                        XtraMessageBox.Show("CBM các thùng vượt quá sức chứa trong Ô. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    foreach (DataRow _dr in _dtVTK.Rows)
                    {
                        _dr["MaKhuVuc"] = _maday.ToString();
                        _dr["MaKe"] = _make.ToString();
                        _dr["MaTang"] = _matang.ToString();
                        _dr["MaO"] = _mao.ToString();
                        _dr["MaKho"] = _kho.ToString();
                        _dr["NVNhap"] = GlobleData.UserName;
                    }

                    string urlsaveVTK = string.Format("{0}?", URL + "NhapKho/PostVTK");
                    string resultsaveVTK = Task.Run(async () => { return await _clientExtension.PostAsync(urlsaveVTK, _dtVTK); }).Result;
                    if (resultsaveVTK.ToLower() != "true")
                        XtraMessageBox.Show(resultsaveVTK);
                }
                string urlSave = "";
                if (!_sms)
                    urlSave = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}", URL + "NhapKho/PostDT", _isNhapKho, _lcsx, 2);
                else
                    urlSave = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}", URL + "NhapKho/PostDTSMS", _isNhapKho, _lcsx, 2);
                string resultSave = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, dtSave); }).Result;
                if (resultSave.ToLower() != "true")
                    XtraMessageBox.Show(resultSave);
                else
                {
                    if (_chkNhap)
                    {
                        if (_dtVTK != null && _dtVTK.Rows.Count > 0)
                        {
                            string urlsaveVTK = string.Format("{0}?", URL + "NhapKho/PostVTK");
                            string resultsaveVTK = Task.Run(async () => { return await _clientExtension.PostAsync(urlsaveVTK, _dtVTK); }).Result;
                            if (resultsaveVTK.ToLower() != "true")
                                XtraMessageBox.Show(resultsaveVTK);
                        }
                    }
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    this.ActiveControl = txtBarCode;
                    txtDisplayBarCode.Text = "";
                    txtPKL.Text = "";
                    txtLenhSX.Text = "";
                    txtCartonNumber.Text = "";
                    txtLenhSX.Text = "";
                    txtPOID.Text = "";
                    txtDonViSX.Text = "";
                    txtDauSize.Text = "";
                    txtMau.Text = "";
                    txtSize.Text = "";
                    txtSLNhap.Text = "";
                    dateEdit1.EditValue = DateTime.Now;
                    txtSoLuong.Text = "";
                    LoadCBM();
                }
            }
            else
            {
                int i = 0;
                if (SoLuong <= 0) return;
                DataTable dtSave = new DataTable();
                dtSave.Columns.Add("MaDH", typeof(string));
                dtSave.Columns.Add("MaPKL", typeof(string));
                dtSave.Columns.Add("MaDVSX", typeof(string));
                dtSave.Columns.Add("MaLenh", typeof(string));
                dtSave.Columns.Add("POID", typeof(string));
                dtSave.Columns.Add("MaKho", typeof(string));
                dtSave.Columns.Add("MaKhoDen", typeof(string));
                dtSave.Columns.Add("TuThung", typeof(int));
                dtSave.Columns.Add("DenThung", typeof(int));
                dtSave.Columns.Add("MinSttThung", typeof(int));
                dtSave.Columns.Add("MaxSttThung", typeof(int));
                dtSave.Columns.Add("SttThungDecat", typeof(int));
                dtSave.Columns.Add("NgayNhapKho_TC", typeof(string));
                if (!_sms)
                {
                    foreach (DataRow row in DtBarCode.Rows)
                    {
                        if (i == SoLuong)
                            break;
                        if (Convert.ToInt32(row["CheckDT"]) == 1)
                        {
                            DataRow _dr = dtSave.NewRow();
                            _dr["MaDH"] = row["MaDH"];
                            _dr["MaPKL"] = row["MaPKL"];
                            _dr["MaLenh"] = row["MaLenh"];
                            _dr["MaDVSX"] = row["MaDVSX"];
                            _dr["POID"] = row["POID"];
                            _dr["MaKho"] = "";
                            _dr["MaKhoDen"] = "";
                            _dr["TuThung"] = TuThung;
                            _dr["DenThung"] = DenThung;
                            _dr["MinSttThung"] = 0;
                            _dr["MaxSttThung"] = 0;
                            _dr["SttThungDecat"] = row["SttDT"];
                            _dr["NgayNhapKho_TC"] = "";
                            dtSave.Rows.Add(_dr);
                            i++;
                        }
                    }
                }
                else
                {
                    foreach (DataRow row in DtBarCode.Rows)
                    {
                        if (Convert.ToInt32(row["CheckDT"]) == 1)
                        {
                            DataRow _dr = dtSave.NewRow();
                            _dr["MaDH"] = row["MaDH"];
                            _dr["MaPKL"] = row["MaPKL"];
                            _dr["MaLenh"] = row["MaLenh"];
                            _dr["MaDVSX"] = row["MaDVSX"];
                            _dr["POID"] = row["POID"];
                            _dr["MaKho"] = "";
                            _dr["MaKhoDen"] = "";
                            _dr["TuThung"] = TuThung;
                            _dr["DenThung"] = DenThung;
                            _dr["MinSttThung"] = 0;
                            _dr["MaxSttThung"] = 0;
                            _dr["SttThungDecat"] = row["SttDT"];
                            _dr["NgayNhapKho_TC"] = "";
                            dtSave.Rows.Add(_dr);
                        }
                    }

                }
                if (dtSave == null || dtSave.Rows.Count == 0)
                {
                    MessageBox.Show("Chưa có nhập không, không có thùng nào để hủy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
               
                string urlSave = "";
                if (!_sms)
                    urlSave = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}", URL + "NhapKho/PostHuy", _isNhapKho, _lcsx, 1);
                else
                    urlSave = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}", URL + "NhapKho/PostHuySMS", _isNhapKho, _lcsx, 1);
                string resultSave = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, dtSave); }).Result;
                if (resultSave.ToLower() != "true")
                    XtraMessageBox.Show(resultSave);
                else
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    this.ActiveControl = txtBarCode;
                    txtDisplayBarCode.Text = "";
                    txtPKL.Text = "";
                    txtLenhSX.Text = "";
                    txtCartonNumber.Text = "";
                    txtLenhSX.Text = "";
                    txtPOID.Text = "";
                    txtDonViSX.Text = "";
                    txtDauSize.Text = "";
                    txtMau.Text = "";
                    txtSize.Text = "";
                    txtSLNhap.Text = "";
                    dateEdit1.EditValue = DateTime.Now;
                    txtSoLuong.Text = "";
                }
            }
        }
        private void btnHuy_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
        private void tbnLamMoi_Click(object sender, EventArgs e)
        {
            this.ActiveControl = txtBarCode;
            txtDisplayBarCode.Text = "";
            txtPKL.Text = "";
            txtLenhSX.Text = "";
            txtCartonNumber.Text = "";
            txtLenhSX.Text = "";
            txtPOID.Text = "";
            txtDonViSX.Text = "";
            txtDauSize.Text = "";
            txtMau.Text = "";
            txtSize.Text = "";
            txtSLNhap.Text = "";
            dateEdit1.EditValue = DateTime.Now;
        }
        private void txtBarCode_TextChanged(object sender, EventArgs e)
        {
            if (txtBarCode.Text == "") return;
            string filter = $"Barcode = '{txtBarCode.Text.ToString()}'";
            selectedRows = _dtNhapKho.Select(filter).FirstOrDefault();
            if (selectedRows == null) return;
            if (!_sms)
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&lcsx={9}&&minsttthung={10}&&maxsttthung={11}", URL + "NhapKho/GetThongTinChiTiet",
                    selectedRows["MaPKL"].ToString(), selectedRows["MaLenh"].ToString(), selectedRows["MaDVSX"].ToString(), selectedRows["POID"].ToString(), selectedRows["DauSizeID"].ToString(), selectedRows["MaMau"].ToString(), selectedRows["TuThung"], selectedRows["DenThung"], _lcsx, selectedRows["MinSttThung"], selectedRows["MaxSttThung"]);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DtBarCode = JsonConvert.DeserializeObject<DataTable>(json);
            }
            else
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&lcsx={9}&&minsttthung={10}&&maxsttthung={11}", URL + "NhapKho/GetThongTinChiTietSMS",
                    selectedRows["MaPKL"].ToString(), selectedRows["MaLenh"].ToString(), selectedRows["MaDVSX"].ToString(), selectedRows["POID"].ToString(), selectedRows["DauSizeID"].ToString(), selectedRows["MaMau"].ToString(), selectedRows["TuThung"], selectedRows["DenThung"], _lcsx, selectedRows["MinSttThung"], selectedRows["MaxSttThung"]);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DtBarCode = JsonConvert.DeserializeObject<DataTable>(json);
            }
            List<string> lstSize = DtBarCode.AsEnumerable()
                      .Select(x => x["Size"].ToString())
                      .ToList();
            txtPKL.Text = selectedRows["MaPKL"].ToString();
            txtLenhSX.Text = selectedRows["MaLenh"].ToString();
            txtCartonNumber.Text = selectedRows["TuThung"].ToString() + "->" + selectedRows["DenThung"].ToString();
            txtLenhSX.Text = selectedRows["MaLenh"].ToString();
            txtPOID.Text = selectedRows["PO"].ToString();
            txtDonViSX.Text = selectedRows["TenDVSX"].ToString();

            List<DataRow> lstDongThung = _dtNhapKho.AsEnumerable()
                .Where(x => x["SttThung"].ToString() == selectedRows["SttThung"].ToString()
                            && x["POID"].ToString() == selectedRows["POID"].ToString()
                            && x["MaHang"].ToString() == selectedRows["MaHang"].ToString())
                .ToList();
            txtDauSize.Text = string.Join(";", lstDongThung.Select(row => row["DauSize"].ToString()));
            txtMau.Text = string.Join(";", lstDongThung.AsEnumerable().Select(row => row["TenMau"].ToString()));
            txtSize.Text = string.Join(";", lstSize.Distinct());
            txtSLNhap.Text = selectedRows["SLNhap"].ToString();
            txtDisplayBarCode.Text = txtBarCode.Text;
            TuThung = Convert.ToInt32(selectedRows["TuThung"]);
            DenThung = Convert.ToInt32(selectedRows["DenThung"]);
            txtSoLuong.Text = "";
            txtBarCode.Text = "";
        }
        private void timerResetTXTQrcode_Tick(object sender, EventArgs e)
        {
            txtBarCode.Text = "";
        }

        private void timerFocusScan_Tick(object sender, EventArgs e)
        {
            this.ActiveControl = txtBarCode;
        }
    }
}
