using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPNhapKhoNPL_KhaiBao : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataRow _drRow;
        public static List<string> ThongSoKien = new List<string>();
        public event Action<List<string>> ThongSoKienChanged;
        public static DataTable dt = new DataTable();
        private string _solo = string.Empty, _madvcd = string.Empty, _tendvcd = string.Empty, _soloid = string.Empty, _batch = string.Empty, _pomua = string.Empty;
        private bool _IsNPL = false;
        private decimal _tileNW = 0m, _tileGW = 0m;

        public frmERPNhapKhoNPL_KhaiBao(DataRow dr, DataTable _dt, string solo = "", bool IsNPL = false, decimal tileNW = 0m, decimal tileGW = 0m, string madvcd = "", string tendvcd = "", string soloid = "", string batch = "", string pomua = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._drRow = dr;
            dt = _dt;
            _IsNPL = IsNPL;
            _solo = solo;
            _tileNW = tileNW;
            _tileGW = tileGW;
            _madvcd = madvcd;
            _tendvcd = tendvcd;
            _soloid = soloid;
            _batch = batch;
            _pomua = pomua;
            txtDV.Text = dr["TenDVVT"].ToString();
            ThongSoKien = new List<string>();
            loadDonVi();
            searchLookUpEdit1.EditValue = madvcd;
            //if (string.IsNullOrWhiteSpace(tendvcd))
            //{
            //    layoutControlItem5.Text = "Khối lượng(kg)/" + dr["TenDVVT"];
            //}
            //else
            //{
            //    layoutControlItem5.Text = "Khối lượng(kg)/" + _tendvcd;
            //}

            txtKLDV.Text = tileNW.ToString();
            txtKLBaoBi.Text = tileGW.ToString();
            if (!IsNPL)
            {
                layoutControlItem1.Text = "Từ Roll/Túi";
                layoutControlItem2.Text = "Đến Roll/Túi";
            }
        }
        private void loadDonVi()
        {
            searchLookUpEdit1.Properties.DisplayMember = "TenDVVT";
            searchLookUpEdit1.Properties.ValueMember = "MaDVVT";
            string url = $"{URL}ERPNhapKhoNPL/GET?Action=GETDVCD&para={_drRow["TenDVVT"]}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;


            if (json == "[]")
            {
                searchLookUpEdit1.EditValue = null;
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit1.Properties.DataSource = tbl;


        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            XacNhan();
        }
      
        private bool XacNhan()
        {
            // --- Phần 1: Validate đầu vào (Giữ nguyên) ---
            if (string.IsNullOrWhiteSpace(txtTuKien.Text) || string.IsNullOrWhiteSpace(txtDenKien.Text))
            {
                XtraMessageBox.Show("Vui lòng nhập đầy đủ Từ Kiện và Đến Kiện.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtTuKien.Text, out int fromValue) || !int.TryParse(txtDenKien.Text, out int toValue))
            {
                XtraMessageBox.Show("Vui lòng nhập giá trị số hợp lệ cho Từ Kiện và Đến Kiện.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (toValue < fromValue)
            {
                XtraMessageBox.Show("Giá trị Đến Kiện phải lớn hơn hoặc bằng Từ Kiện.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            var tienTo = txtTienTo.Text;
            var soLOT = txtSoLot.Text;
            var batch = txtBatch.Text;
            // Chuyển đổi một lần, dùng lại nhiều lần
            decimal.TryParse(txtKLDV.Text, out decimal tileNW);
            decimal.TryParse(txtKLBaoBi.Text, out decimal tileGW);
            decimal.TryParse(txtSL.Text, out decimal soLuongTheoCT);
            decimal.TryParse(txtKhoiLuong.Text?.ToString(), out decimal klGoc);
            decimal.TryParse(txtTrongLuong.Text?.ToString(), out decimal tlGoc);

            var maDonVi = (searchLookUpEdit1.EditValue == null || searchLookUpEdit1.EditValue.ToString() == "") ? _drRow["MaDVVT"].ToString() : searchLookUpEdit1.EditValue.ToString();
            var donVi = string.IsNullOrEmpty(searchLookUpEdit1.Text.ToString()) ? _drRow["TenDVVT"].ToString() : searchLookUpEdit1.Text.ToString();

            string maVTID = _drRow["MaVTID"].ToString();
            string maMauVT = _drRow["MauVTID"].ToString();
            string khoVaiID = _drRow["KhoVaiID"].ToString();
            string maNhom = _drRow["MaNhom"].ToString();
            string maVatTu_Ghep = _drRow["MaVTGhep"].ToString();

            string soLoID = _soloid;

            string maNPL = RemoveVietnameseTone(ReplaceSpecialCharacters(maNhom) + "@" + ReplaceSpecialCharacters(maVTID) + "@" + ReplaceSpecialCharacters(maMauVT) + "@" + ReplaceSpecialCharacters(khoVaiID));

            var existingRowsWithSameCriteria = dt.AsEnumerable().Where(x =>
                                                x.Field<string>("MaVTID") == maVTID &&
                                                x.Field<string>("MauVTID") == maMauVT &&
                                                x.Field<string>("KhoVaiID") == khoVaiID &&
                                                x.Field<string>("MaNhom") == maNhom &&
                                                x.Field<string>("MaVTGhep") == maVatTu_Ghep
                                                && x["SoLoT"] != ""
                                                ).ToList();

            var setSoKienHienThi = new HashSet<string>(existingRowsWithSameCriteria.Select(r => r.Field<string>("SoKienHienThi")));

            //int maxSTT = existingRowsWithSameCriteria.Any() ? existingRowsWithSameCriteria.Max(x => x.Field<int>("STT")) : 0; //Thoaicmt

            //Thoai
            int maxSTT = existingRowsWithSameCriteria
                .Where(row => int.TryParse(row["SoKien"]?.ToString(), out _))
                .Select(row => int.Parse(row["SoKien"].ToString()))
                .DefaultIfEmpty(0)
                .Max();
            // Sử dụng một List để lưu các dòng mới, sau đó mới thêm vào DataTable
            var newRows = new List<DataRow>();
            int countKien = toValue - fromValue + 1;

            for (int i = 0; i < countKien; i++)
            {
                var soKienHienThiMoi = $"{tienTo}{fromValue + i}";

                // Kiểm tra trong HashSet thay vì truy vấn Linq
                if (setSoKienHienThi.Contains(soKienHienThiMoi))
                {
                    XtraMessageBox.Show($"Kiện {soKienHienThiMoi} đã tồn tại trong bảng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                DataRow nr = dt.NewRow();
                nr["ID"] = 0;
                nr["SoLoID"] = soLoID;
                nr["MaNPL"] = maNPL;
                nr["MaVTID"] = maVTID;
                nr["MaMauVT"] = maMauVT;
                nr["MauVT"] = _drRow["MauVT"];
                nr["SoKien"] = soKienHienThiMoi;
                nr["SoLot"] = soLOT;

                nr["MaHaiQuan"] = _drRow["MaHaiQuan"];
                nr["MaKeToan"] = _drRow["MaKeToan"];
                nr["SoGhiDauCay"] = soLuongTheoCT;
                nr["NW"] = klGoc;
                nr["GW"] = tlGoc;
                nr["BarCode"] =$"{_pomua}|{_drRow["MaVT"]}|{maMauVT}|{soKienHienThiMoi}";
                nr["GhiChu"] = "";
                nr["IsNPL"] = _IsNPL;
                nr["KhoVaiID"] = khoVaiID;
                nr["SoKienParent"] = "";
                nr["DonGia"] = _drRow["DonGia"];
                nr["ThanhTien"] = 0;
                nr["SoLuongThucTe"] = 0;
                nr["Pallet"] = "";
                nr["MaDVVT"] = _drRow["MaDVVT"];
                nr["MauVTID"] = maMauVT;
                nr["SoKienHienThi"] = soKienHienThiMoi;
                nr["STT"] = maxSTT + 1 + i;
                nr["IsNK"] = false;
                nr["TienTe"] = _drRow["TienTe"];
                nr["MaVTGhep"] = maVatTu_Ghep;
                nr["MaNhom"] = maNhom;

                nr["MaDVCD"] = maDonVi;
                nr["TenDVCD"] = donVi;
                nr["Batch"] = batch;
                nr["SLTong"] = _drRow["SLTong"];
                nr["TuoiTonKho"] = _drRow["TuoiTonKho"];
                // Lưu dòng mới vào List tạm thời
                newRows.Add(nr);
            }

            // Thêm tất cả các dòng mới vào DataTable
            foreach (var rowToAdd in newRows)
            {
                dt.Rows.Add(rowToAdd);
            }

            // --- Phần 4: Cập nhật tất cả các dòng liên quan một lần ---
            // Kết hợp các dòng đã tồn tại và các dòng mới để cập nhật
            var allRelevantRows = existingRowsWithSameCriteria.Concat(newRows);

            foreach (DataRow dr in allRelevantRows)
            {
                decimal soGhiDauCay = dr.Field<decimal?>("SoGhiDauCay") ?? 0m;

                // Cập nhật các giá trị tỷ lệ
                dr["tileNW"] = tileNW;
                dr["tileGW"] = tileGW;

                // Tính toán lại khối lượng
                decimal khoiLuongMoi = soGhiDauCay * tileNW;
                decimal trongLuongMoi = khoiLuongMoi + tileGW;

                dr["NW"] = khoiLuongMoi;
                dr["GW"] = trongLuongMoi;

                // Cập nhật đơn vị
                dr["MaDVCD"] = maDonVi;
                dr["TenDVCD"] = donVi;
            }

            // --- Phần 5: Kết thúc (Giữ nguyên) ---
            ThongSoKien.Clear();
            ThongSoKien.AddRange(new string[] { tienTo, txtTuKien.Text, txtDenKien.Text, soLOT, batch, txtKLDV.Text, txtKLBaoBi.Text, maDonVi, donVi });
            ThongSoKienChanged?.Invoke(ThongSoKien);

            return true;
        }
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            //layoutControlItem5.Text = "Khối lượng(kg)/" +searchLookUpEdit1.Text.ToString();
        }

        private void textEdit3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.ToUpper(e.KeyChar);
        }

        private void textEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textEdit2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void setTypeTxtEdit(object sender, KeyPressEventArgs e)
        {
            TextEdit textEdit = sender as TextEdit;
            string currentText = textEdit.Text;
            int cursorPosition = textEdit.SelectionStart;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && currentText.Contains("."))
            {
                e.Handled = true;
                return;
            }
            if (char.IsDigit(e.KeyChar))
            {
                string newText = currentText.Insert(cursorPosition, e.KeyChar.ToString());
                if (newText.Contains("."))
                {
                    int indexOfDot = newText.IndexOf('.');
                    string decimalPart = newText.Substring(indexOfDot + 1);
                    if (decimalPart.Length > 2)
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
        }
        private void txtSL_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void textEdit21_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void textEdit211_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void textEdit21_Enter(object sender, EventArgs e)
        {
            var textEdit = sender as DevExpress.XtraEditors.TextEdit;
            if (textEdit == null) return;

            decimal val;
            if (decimal.TryParse(textEdit.Text, out val) && val == 0)
            {
                textEdit.Text = "";
                //textEdit.SelectAll();
            }
            var edit = sender as DevExpress.XtraEditors.TextEdit;
            if (edit != null)
                edit.SelectAll();
        }

        private void textEdit211_Enter(object sender, EventArgs e)
        {
            var textEdit = sender as DevExpress.XtraEditors.TextEdit;
            if (textEdit == null) return;

            decimal val;
            if (decimal.TryParse(textEdit.Text, out val) && val == 0)
            {
                textEdit.Text = "";
                //textEdit.SelectAll();
            }
            var edit = sender as DevExpress.XtraEditors.TextEdit;
            if (edit != null)
                edit.SelectAll();
        }

        private void textEdit21_Leave(object sender, EventArgs e)
        {
            var textEdit = sender as DevExpress.XtraEditors.TextEdit;
            if (textEdit == null) return;

            if (string.IsNullOrWhiteSpace(textEdit.Text))
            {
                textEdit.Text = "0";
            }
        }

        private void textEdit211_Leave(object sender, EventArgs e)
        {
            var textEdit = sender as DevExpress.XtraEditors.TextEdit;
            if (textEdit == null) return;

            if (string.IsNullOrWhiteSpace(textEdit.Text))
            {
                textEdit.Text = "0";
            }
        }

        private void txt_Enter(object sender, EventArgs e)
        {
            var edit = sender as DevExpress.XtraEditors.TextEdit;
            if (edit != null)
                edit.SelectAll();
        }

        private void txtKhoiLuong_EditValueChanged(object sender, EventArgs e)
        {


            decimal khoiLuong, soLuong, khoiLuongBaoBi;
            if (decimal.TryParse(txtKhoiLuong.Text, out khoiLuong) && decimal.TryParse(txtSL.Text, out soLuong) && soLuong != 0)
            {
                decimal kldv = khoiLuong / soLuong;
                txtKLDV.Text = kldv.ToString("N2");
            }


            if (!decimal.TryParse(txtKLBaoBi.Text, out khoiLuongBaoBi)) return;
            decimal trongluong = khoiLuong + khoiLuongBaoBi;
            txtTrongLuong.Text = Math.Round(trongluong, 2).ToString("N2");

        }

        private void txtSL_EditValueChanged(object sender, EventArgs e)
        {
            decimal khoiLuong, soLuong;
            if (!decimal.TryParse(txtKhoiLuong.Text, out khoiLuong)) return;
            if (!decimal.TryParse(txtSL.Text, out soLuong)) return;
            if (soLuong == 0) return;
            decimal kldv = khoiLuong / soLuong;
            txtKLDV.Text = Math.Round(kldv, 2).ToString("N2");
        }

        private void txtKLDV_EditValueChanged(object sender, EventArgs e)
        {
            decimal soLuong, kldv;
            if (!decimal.TryParse(txtSL.Text, out soLuong)) return;
            if (!decimal.TryParse(txtKLDV.Text, out kldv)) return;
            if (soLuong == 0) return;
            decimal khoiLuong = kldv * soLuong;
            txtKhoiLuong.Text = Math.Round(khoiLuong, 2).ToString("N2");
        }

        private void txtTrongLuong_EditValueChanged(object sender, EventArgs e)
        {

            decimal khoiLuong, trongLuong;
            if (!decimal.TryParse(txtKhoiLuong.Text, out khoiLuong)) return;
            if (!decimal.TryParse(txtTrongLuong.Text, out trongLuong)) return;
            if (trongLuong == 0) return;
            decimal klBaoBi = trongLuong - khoiLuong;
            txtKLBaoBi.Text = Math.Round(klBaoBi, 2).ToString("N2");

        }

        private void txtKLBaoBi_EditValueChanged(object sender, EventArgs e)
        {

            decimal khoiLuong, khoiLuongBaoBi;
            if (!decimal.TryParse(txtKhoiLuong.Text, out khoiLuong)) return;
            if (!decimal.TryParse(txtKLBaoBi.Text, out khoiLuongBaoBi)) return;
            if (khoiLuongBaoBi == 0) return;
            decimal trongluong = khoiLuong + khoiLuongBaoBi;
            txtTrongLuong.Text = Math.Round(trongluong, 2).ToString("N2");

        }

        private void txtKhoiLuong_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void txtTrongLuong_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            //bool isXN= XacNhan();
            //if(isXN)  
            this.DialogResult = DialogResult.OK;
        }
        public string RemoveVietnameseTone(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in normalizedString)
            {

                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (c == 'Đ')
                    {
                        stringBuilder.Append('D');
                    }
                    else if (c == 'đ')
                    {
                        stringBuilder.Append('d');
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        private string ReplaceSpecialCharacters(string input)
        {
            try
            {
                string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\”|,.<>/?]+";
                string replacement = "_";
                Regex regex = new Regex(pattern);
                return regex.Replace(input, replacement);
            }
            catch (Exception ex)
            {

                return input;
            }

        }

    }
}