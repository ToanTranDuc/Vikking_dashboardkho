using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using NtbSoft.ERP.Win.Utils;
using NtbSoft.ERP.Win.Modules.QuanLyDonHang;
using DevExpress.SpreadsheetSource;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using DevExpress.Spreadsheet;
using System.Data.OleDb;
using DevExpress.DataAccess.Excel;
using NtbSoft.ERP.Win.Modules.Kho;
using NtbSoft.ERP.Win.Modules.ThuVien;
using System.Globalization;
using NtbSoft.ERP.Win.Properties;

namespace NtbSoft.ERP.Win.Modules.Erp.Kho
{
    public partial class frmERP_VatTuExcel : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                              new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        CustomImportDataExcel _customImport;

        public event EventHandler<string> DataClosed;
        System.Data.DataTable tbl_sheet;
        private HttpClientExtension _clientExtension;
        DataTable tblNhapKho = new DataTable();

        string _manpl = string.Empty;
        DataTable tbl = new DataTable();
        DataTable dtbcreate;
        DataTable _tblNhom = new DataTable(), tblKV = new DataTable(), tblDV = new DataTable(), tblVatTu = new DataTable(), tblMauVT = new DataTable(), tblTheKhoVai = new DataTable();
        string _kh = string.Empty, _khvt = string.Empty, _makh = string.Empty;
        string _mahang = string.Empty;
        bool isDinhMuc = true;
        string _tenhang = string.Empty;

        public frmERP_VatTuExcel(string kh, string khviettat, string makh, bool isDM)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));

            _kh = kh;
            _khvt = khviettat;
            _makh = makh;
            _customImport = new CustomImportDataExcel();
            textEdit1.Enabled = false;
            _clientExtension = new HttpClientExtension();

            dtbcreate = CreateTable();
            txtKH.Text = _kh;
            isDinhMuc = isDM;
            loadNhom();
            loadKhoVai();
            loadTheKhoVai();
            loadDonVi();
            loadVatTu();
            loadMauVT();
        }
        private void btGetLink_Click(object sender, EventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                tbl_sheet = new DataTable();
                AddColumn_tbl_Sheet();
                textEditLink.Text = Sfd.FileName;
                CreateDefaultLookUp(textEditLink.Text);
                textEdit1.Enabled = true;
            }
        }

        private DataTable CreateTable()
        {
            try
            {
                DataTable dtbcreate = new DataTable("dtbcreate");
                dtbcreate.Columns.Add("ID", typeof(int));
                dtbcreate.Columns.Add("MaHang", typeof(string));
                dtbcreate.Columns.Add("Makh", typeof(string));
                dtbcreate.Columns.Add("MaNhom", typeof(string));
                dtbcreate.Columns.Add("MaVTID", typeof(string));
                dtbcreate.Columns.Add("MaVT", typeof(string));
                dtbcreate.Columns.Add("ChiTiet", typeof(string));
                dtbcreate.Columns.Add("MaDVVT", typeof(string));
                dtbcreate.Columns.Add("MauVTID", typeof(string));
                dtbcreate.Columns.Add("MaMauVT", typeof(string));
                dtbcreate.Columns.Add("MauVT", typeof(string));
                dtbcreate.Columns.Add("KhoVaiID", typeof(string));
                dtbcreate.Columns.Add("MaMau", typeof(string));
                dtbcreate.Columns.Add("NPL", typeof(bool));
                dtbcreate.Columns.Add("TenMau", typeof(string));
                dtbcreate.Columns.Add("TenNhom", typeof(string));
                dtbcreate.Columns.Add("Sort", typeof(int));
                dtbcreate.Columns.Add("GhiChu", typeof(string));
                dtbcreate.Columns.Add("IsNew", typeof(int));
                dtbcreate.Columns.Add("STT", typeof(int));
                dtbcreate.Columns.Add("MaVTGhep", typeof(string));
                dtbcreate.Columns.Add("STTCode", typeof(int));
                dtbcreate.Columns.Add("STTIndex", typeof(int));
                dtbcreate.Columns.Add("IsActive", typeof(bool));
                dtbcreate.Columns.Add("MaNhomChiTiet", typeof(string));
                dtbcreate.Columns.Add("TenDVKV", typeof(string));
                dtbcreate.Columns.Add("KhoVaiHienThi", typeof(string));
                return dtbcreate;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private void loadNhom()
        {
            try
            {


                string url = $"{URL}ERPVatTuBOM/GetChung?Action=GETNHOMEXCEL";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                _tblNhom = JsonConvert.DeserializeObject<DataTable>(json);
                if (_tblNhom == null || _tblNhom.Rows.Count == 0) return;

            }
            catch (Exception ex)
            {


            }

        }
        private void loadKhoVai()
        {
            try
            {

                string url = $"{URL}ERPVatTuBOM/GetChung?Action=GETKHOVAIEXCEL";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]") return;
                tblKV = JsonConvert.DeserializeObject<DataTable>(json);



            }
            catch (Exception ex)
            {
            }

        }

        private void loadTheKhoVai()
        {
            try
            {

                string url = $"{URL}ERPVatTuBOM/GetChung?Action=GetTheKhoVai";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                if (json == "[]") return;
                tblTheKhoVai = JsonConvert.DeserializeObject<DataTable>(json);



            }
            catch (Exception ex)
            {
            }

        }
        private void loadDonVi()
        {
            try
            {

                string url = string.Format("{0}?", URL + "ERPVatTuBOM/GetDonVi");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                tblDV = JsonConvert.DeserializeObject<DataTable>(json);


            }
            catch (Exception ex)
            {
            }
        }
        private void loadVatTu()
        {
            try
            {
                string url = $"{URL}ERPVatTuTV/Get?Action=GET";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {

                    return;
                }
                tblVatTu = JsonConvert.DeserializeObject<DataTable>(json);


            }
            catch (Exception ex)
            {
            }
        }
        private void loadMauVT()
        {
            try
            {
                string url = $"{URL}ERPVatTuBom/GetChung?Action=GETMAUVTEXCEL";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {

                    return;
                }
                tblMauVT = JsonConvert.DeserializeObject<DataTable>(json);




            }
            catch (Exception ex)
            {
            }
        }
        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\”|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
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

        private void AddColumn_tbl_Sheet()
        {
            tbl_sheet.Columns.Add("Name");
        }

        private void CreateDefaultLookUp(string pathExecel)
        {
            ISpreadsheetSource spreadsheetSource = SpreadsheetSourceFactory.CreateSource(pathExecel);
            IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
            int worksheetcount = worksheetCollection.Count();
            for (int i = 0; i < worksheetcount; i++)
            {
                DataRow r = tbl_sheet.NewRow();
                r["Name"] = worksheetCollection[i].Name;
                tbl_sheet.Rows.Add(r);
            }
            textEdit1.Properties.DataSource = tbl_sheet;
            textEdit1.Properties.ValueMember = "Name";
            textEdit1.Properties.DisplayMember = "Name";

        }
        DataTable tblKhoVai;
        private void CreateTableKhoVai()
        {
            tblKhoVai = new DataTable("tblKhoVai");
            tblKhoVai.Columns.Add("ID", typeof(int));
            tblKhoVai.Columns.Add("KhoVaiID", typeof(string));
            tblKhoVai.Columns.Add("KhoVai", typeof(string));
            tblKhoVai.Columns.Add("NPL", typeof(bool));
            tblKhoVai.Columns.Add("MaNhom", typeof(string));
            tblKhoVai.Columns.Add("GhiChu", typeof(string));
            tblKhoVai.Columns.Add("MaDVVT", typeof(string));
            tblKhoVai.Columns.Add("KhoVaiMet", typeof(decimal));
            tblKhoVai.Columns.Add("MaTheKhoVai", typeof(string));
        }
        private void btImport_Click(object sender, EventArgs e)
        {
            try
            {
                dtbcreate.Clear();
                var lstKV_Save = new List<string>();
                if (string.IsNullOrEmpty(textEditLink.Text))
                {
                    XtraMessageBox.Show("Vui lòng chọn file Excel.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (textEdit1.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn sheet.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                CreateTableKhoVai();
                string worksheetName = textEdit1.EditValue.ToString();
                string filePath = textEditLink.Text;
                if (isDinhMuc)
                {
                    using (var source = new ExcelDataSource())
                    {
                        source.FileName = filePath;
                        source.SourceOptions = new ExcelSourceOptions(new ExcelWorksheetSettings(worksheetName, "C3:C4"));
                        source.Fill();

                        DataTable validationTable = source.ToDataTable();
                        string rangeExcel = "A8:Q5000";
                        if (validationTable != null && validationTable.Columns.Count != 0)
                        {
                            string khachHangFromFile = validationTable.Columns[0].ColumnName.ToString().Trim().ToUpper();

                            string makh = loadKhachHang(khachHangFromFile);

                            if (string.IsNullOrEmpty(makh))
                            {
                                XtraMessageBox.Show("Khách hàng không khớp. Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            string tenhang = validationTable.Rows[0][0].ToString();

                            _mahang = getMaHang(tenhang);
                            if (string.IsNullOrEmpty(_mahang))
                            {
                                XtraMessageBox.Show("Mã hàng không trùng khớp. Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                        }

                        source.SourceOptions = new ExcelSourceOptions(new ExcelWorksheetSettings(worksheetName, rangeExcel));
                        source.Fill();
                        tbl = source.ToDataTable();


                        DataRow[] rowsToDelete = tbl.AsEnumerable()
                    .Where(row =>
                        (row.IsNull(1) || string.IsNullOrWhiteSpace(row[1].ToString())) &&
                        (row.IsNull(3) || string.IsNullOrWhiteSpace(row[3].ToString())) &&
                        (row.IsNull(4) || string.IsNullOrWhiteSpace(row[4].ToString()))
                    ).ToArray();

                        foreach (DataRow row in rowsToDelete)
                        {
                            tbl.Rows.Remove(row);
                        }

                        if (tbl == null || tbl.Rows.Count == 0)
                        {
                            XtraMessageBox.Show("Không có dữ liệu. Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        tbl = FillForward(tbl);
                        DataTable tblMau = CreateTableMau(_makh, _mahang);
                        DataTable tblSize = CreateTableSize(_makh, _mahang);
                        if (!KiemTraVaXuatTatCaLoi(tbl)) return;
                        if (isDinhMuc)
                        {
                            if (!KiemTraDuLieuDinhMuc(tbl)) return;
                            if (!KiemTraHopLeHaoHut(tbl)) return;
                            if (!KiemTraChungLoaiChiTiet(tbl, _tblNhom)) return;
                            if (!KiemTraDuLieuToanBo(tbl, tblMau, tblSize))
                            { isDinhMuc = false; return; }

                        }


                        if (!KiemTraDuLieuMauVT(tbl, 5, 6, 7)) return;

                        //check nhóm
                        if (!dtbcreate.Columns.Contains("TenDVVT"))
                            dtbcreate.Columns.Add("TenDVVT", typeof(string));
                        if (!dtbcreate.Columns.Contains("KhoVai"))
                            dtbcreate.Columns.Add("KhoVai", typeof(string));

                        bool hopLe = KiemTraVaThemNhomMoi();
                        if (!hopLe)
                        {
                            return;
                        }

                        //if (!KiemTraMauHopLe(tbl, tblMauVT)) return;

                        DataTable tblDinhMuc = CreateTableDinhMuc();

                        //isDinhMuc = true;


                        int index = 0;
                        Func<string, string> chuanHoaTenNhom = s =>
                        {
                            if (string.IsNullOrEmpty(s))
                            {
                                return "";
                            }
                            // 1. Chuẩn hóa Unicode về dạng C (phổ biến nhất, tổ hợp sẵn)
                            string normalized = s.Normalize(System.Text.NormalizationForm.FormC);
                            // 2. Loại bỏ khoảng trắng
                            string noWhitespace = Regex.Replace(normalized, @"\s+", "");
                            // 3. Chuyển sang chữ hoa bất biến
                            return noWhitespace.ToUpperInvariant();
                        };
                        foreach (DataRow dr in tbl.Rows)
                        {

                            #region thông số
                            DataRow nr = dtbcreate.NewRow();
                            nr["ID"] = 0;
                            nr["MaKH"] = _makh;
                            string tennhom = NormalizeString(dr[1].ToString());
                            string chungloaichittiet = NormalizeString(dr[2].ToString());
                            string mavt = NormalizeString(dr[3].ToString());
                            string chitiet = NormalizeString(dr[4].ToString());
                            string themau = NormalizeString(dr[5].ToString());
                            string mamauvt = NormalizeString(dr[6].ToString());
                            string mauvt = NormalizeString(dr[7].ToString());
                            string khovai = NormalizeString(dr[8].ToString());
                            string dvtKhoVai = NormalizeString(dr[9].ToString());
                            string tendvvt = NormalizeString(dr[10].ToString());
                            // 1. Chuẩn hóa chuỗi đầu vào `tennhom` TRƯỚC khi truy vấn
                            string tenNhomChuanHoa = chuanHoaTenNhom(tennhom);

                            // 2. Tìm dòng dữ liệu bằng cách so sánh các giá trị đã được chuẩn hóa
                            DataRow targetRow = _tblNhom.AsEnumerable()
                                                        .FirstOrDefault(row => chuanHoaTenNhom(row.Field<string>("ChungLoaiVatTu")) == tenNhomChuanHoa);

                            if (targetRow == null)
                            {
                                XtraMessageBox.Show("Vui lòng xử lý chủng loại không hợp lệ trước khi thực hiện Import!!!.", "Đã hủy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }


                            DataRow targetRowChungLoaiCT = _tblNhom.AsEnumerable()
                                .FirstOrDefault(row =>
                                    System.Text.RegularExpressions.Regex.Replace(Convert.ToString(row["ChungLoaiVatTu"]), @"\s+", "").ToUpper() == tenNhomChuanHoa &&
                                    System.Text.RegularExpressions.Regex.Replace(Convert.ToString(row["TenNhom"]), @"\s+", "").ToUpper() == chungloaichittiet
                                );
                            DataRow targetRowMaVT = tblVatTu.AsEnumerable()
                                .FirstOrDefault(row =>
                                    System.Text.RegularExpressions.Regex.Replace(Convert.ToString(row["MaVT"]), @"\s+", "").ToUpper() == mavt &&
                                    System.Text.RegularExpressions.Regex.Replace(Convert.ToString(row["ChiTiet"]), @"\s+", "").ToUpper() == chitiet
                                );

                            DataRow targetRowMauVT = tblMauVT.AsEnumerable()
                                .FirstOrDefault(row =>
                                    System.Text.RegularExpressions.Regex.Replace(Convert.ToString(row["TheMau"]), @"\s+", "").ToUpper() == themau &&
                                    System.Text.RegularExpressions.Regex.Replace(Convert.ToString(row["MaMauVT"]), @"\s+", "").ToUpper() == mamauvt &&
                                    System.Text.RegularExpressions.Regex.Replace(Convert.ToString(row["MauVT"]), @"\s+", "").ToUpper() == mauvt
                                );

                            DataRow targetRowKV = tblKV.AsEnumerable()
                                .FirstOrDefault(row => row["NPL"].ToString() == targetRow["NPL"].ToString() &&
                                    System.Text.RegularExpressions.Regex.Replace(Convert.ToString(row["KhoVai"]), @"\s+", "").ToUpper() == khovai &&
                                    System.Text.RegularExpressions.Regex.Replace(Convert.ToString(row["TenDVVT"]), @"\s+", "").ToUpper() == dvtKhoVai
                                );

                            DataRow targetRowDV = tblDV.AsEnumerable()
                                .FirstOrDefault(row =>
                                    System.Text.RegularExpressions.Regex.Replace(Convert.ToString(row["TenDVVT"]), @"\s+", "").ToUpper() == tendvvt
                                );
                            nr["MaNhom"] = targetRow["MaCLVT"].ToString();
                            nr["TenNhom"] = targetRow["ChungLoaiVatTu"].ToString();
                            nr["Sort"] = targetRow["Sort"].ToString();
                            nr["NPL"] = targetRow["NPL"];
                            if (targetRow["NPL"].ToString() == "True" && string.IsNullOrEmpty(dr[7].ToString().Replace("\n", "").Replace("\r", "")))
                            {
                                XtraMessageBox.Show("Cần nhập đẩy đủ Màu vật tư đối với nguyên liệu ở dòng " + dr[0].ToString(), "Đã hủy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (targetRow["NPL"].ToString() == "True" && string.IsNullOrEmpty(dr[8].ToString().Replace("\n", "").Replace("\r", "")))
                            {
                                XtraMessageBox.Show("Cần nhập đẩy đủ Khổ/Size đối với nguyên liệu ở dòng " + dr[0].ToString(), "Đã hủy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            nr["IsNew"] = 2;
                            nr["MaVT"] = RemoveVietnameseTone(dr[3].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper());
                            nr["ChiTiet"] = dr[4].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper();
                            nr["MaMauVT"] = RemoveVietnameseTone(dr[6].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper());
                            nr["MauVT"] = dr[7].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper();
                            nr["KhoVai"] = dr[8].ToString().Replace("\n", "").Replace("\r", "");
                            nr["KhoVaiHienThi"] = dr[8].ToString().Replace("\n", "").Replace("\r", "") + " " + dr[9].ToString().Replace("\n", "").Replace("\r", "");
                            nr["TenDVKV"] = dr[9].ToString().Replace("\n", "").Replace("\r", "");
                            nr["TenDVVT"] = dr[10].ToString().Replace("\n", "").Replace("\r", "");
                            if (targetRowChungLoaiCT != null)
                            {
                                nr["MaNhomChiTiet"] = targetRowChungLoaiCT["MaNhom"];
                            }
                            if (targetRowMaVT != null)
                            {
                                nr["MaVTID"] = targetRowMaVT["MaVTID"];
                            }
                            if (targetRowMauVT != null)
                            {
                                nr["MauVTID"] = targetRowMauVT["MauVTID"];
                            }
                            if (targetRowKV != null)
                                nr["KhoVaiID"] = targetRowKV["KhoVaiID"];
                            if (targetRowDV != null)
                            {
                                nr["MaDVVT"] = targetRowDV["MaDVVT"];

                            }
                            nr["MaVTGhep"] = targetRow["VietTat"] + "|" + RemoveVietnameseTone(dr[3].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper()) + "|" + (nr["MaMauVT"].ToString() == "" ? "00" : RemoveVietnameseTone(dr[6].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper())) + "|" + (nr["KhoVai"].ToString() == "" ? "00" : dr[8].ToString().Replace("\n", "").Replace("\r", ""));
                            nr["STTIndex"] = index;
                            bool isDuplicate = dtbcreate.AsEnumerable().Any(row =>
                                                    row["MaNhom"].ToString().Replace(" ", "").ToUpper() == nr["MaNhom"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["MaVT"].ToString().Replace(" ", "").ToUpper() == nr["MaVT"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["ChiTiet"].ToString().Replace(" ", "").ToUpper() == nr["ChiTiet"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["MaMauVT"].ToString().Replace(" ", "").ToUpper() == nr["MaMauVT"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["MauVT"].ToString().Replace(" ", "").ToUpper() == nr["MauVT"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["KhoVai"].ToString().Replace(" ", "").ToUpper() == nr["KhoVai"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["TenDVKV"].ToString().Replace(" ", "").ToUpper() == nr["TenDVKV"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["TenDVVT"].ToString().Replace(" ", "").ToUpper() == nr["TenDVVT"].ToString().Replace(" ", "").ToUpper()
                                                    );

                            if (!isDuplicate)
                                dtbcreate.Rows.Add(nr);
                            #endregion
                            if (!isDinhMuc) continue;
                            #region Định mức
                            string mauSanPhamInput = dr[11]?.ToString().ToUpper().Trim();
                            string inseamInput = dr[12]?.ToString();
                            string sizeInput = dr[13]?.ToString();


                            var colorsToProcess = Enumerable.Empty<DataRow>();
                            if (string.IsNullOrEmpty(mauSanPhamInput))
                            {
                                colorsToProcess = tblMau.AsEnumerable();
                            }
                            else
                            {
                                List<string> mauList = mauSanPhamInput.Split(',')
                                                                      .Select(m => m.Trim())
                                                                      .Where(m => !string.IsNullOrEmpty(m))
                                                                      .ToList();
                                colorsToProcess = tblMau.AsEnumerable()
                                 .Where(r => mauList.Contains(r["TenMau"].ToString().ToUpper()) ||
                                             mauList.Contains(r["CodeMau"].ToString().ToUpper()));
                            }
                            var inseamsToProcess = Enumerable.Empty<DataRow>();
                            if (string.IsNullOrEmpty(inseamInput))
                            {

                                inseamsToProcess = tblSize.AsEnumerable()
                                                          .GroupBy(r => r["NhomSize"].ToString().ToUpper())
                                                          .Select(g => g.First());
                            }
                            else
                            {
                                List<string> inseamList = inseamInput.Split(',')
                                                                     .Select(i => i.Trim())
                                                                     .Where(i => !string.IsNullOrEmpty(i))
                                                                     .ToList();

                                inseamsToProcess = tblSize.AsEnumerable()
                                                          .Where(r => inseamList.Contains(r["NhomSize"].ToString().ToUpper()))
                                                          .GroupBy(r => r.Field<string>("NhomSize"))
                                                          .Select(g => g.First());
                            }



                            var sizesToProcess = Enumerable.Empty<DataRow>();
                            if (string.IsNullOrEmpty(sizeInput))
                            {

                                sizesToProcess = tblSize.AsEnumerable();
                            }
                            else
                            {
                                List<string> sizeList = sizeInput.Split(',')
                                                                 .Select(s => s.Trim())
                                                                 .Where(s => !string.IsNullOrEmpty(s))
                                                                 .ToList();
                                sizesToProcess = tblSize.AsEnumerable()
                                                        .Where(r => sizeList.Contains(r["TenSize"].ToString().ToUpper()));
                            }

                            if (!colorsToProcess.Any() || !inseamsToProcess.Any() || !sizesToProcess.Any())
                            {

                                continue;
                            }
                            var combinations = from colorRow in colorsToProcess
                                               from inseamRow in inseamsToProcess
                                               from sizeRow in sizesToProcess
                                               where sizeRow.Field<string>("NhomSize") == inseamRow.Field<string>("NhomSize")
                                               select new { colorRow, inseamRow, sizeRow };
                            foreach (var combo in combinations)
                            {
                                DataRow _dm = tblDinhMuc.NewRow();
                                _dm["ID"] = 0;
                                _dm["MaKH"] = _makh;
                                _dm["MaHang"] = _mahang;
                                _dm["MaNhom"] = targetRow["MaCLVT"].ToString();
                                _dm["TenNhom"] = targetRow["ChungLoaiVatTu"].ToString();
                                _dm["Sort"] = targetRow["Sort"].ToString();
                                _dm["NPL"] = targetRow["NPL"];
                                if (targetRowChungLoaiCT != null)
                                {
                                    _dm["MaNhomChiTiet"] = targetRowChungLoaiCT["MaNhom"];
                                }
                                if (targetRowDV != null)
                                {
                                    _dm["MaDVVT"] = targetRowDV["MaDVVT"];

                                }
                                _dm["TenDVVT"] = dr[10].ToString().Replace(Environment.NewLine, "");
                                if (targetRowMaVT != null)
                                {
                                    _dm["MaVTID"] = targetRowMaVT["MaVTID"];
                                }
                                _dm["MaVT"] = RemoveVietnameseTone(dr[3].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper());
                                _dm["ChiTiet"] = dr[4].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper();

                                if (targetRowMauVT != null)
                                {
                                    _dm["MauVTID"] = targetRowMauVT["MauVTID"];
                                }
                                _dm["MaMauVT"] = RemoveVietnameseTone(dr[6].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper());
                                _dm["MauVT"] = dr[7].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper();
                                if (targetRowKV != null)
                                    _dm["KhoVaiID"] = targetRowKV["KhoVaiID"];
                                _dm["KhoVai"] = dr[8].ToString().Replace("\n", "").Replace("\r", "");
                                _dm["KhoVaiHienThi"] = dr[8].ToString().Replace("\n", "").Replace("\r", "") + " " + dr[9].ToString().Replace("\n", "").Replace("\r", "");
                                _dm["TenDVKV"] = dr[9].ToString().Replace("\n", "").Replace("\r", "");
                                _dm["MaMau"] = combo.colorRow["MaMau"];
                                _dm["TenMau"] = combo.colorRow["TenMau"];
                                _dm["MaNhomSize"] = combo.sizeRow["MaNhomSize"];
                                _dm["NhomSize"] = combo.sizeRow["NhomSize"];
                                _dm["MaSize"] = combo.sizeRow["MaSize"];
                                _dm["TenSize"] = combo.sizeRow["TenSize"];
                                _dm["DinhMuc"] = Math.Round(float.TryParse(dr[14]?.ToString(), out float dinhMuc) ? dinhMuc : 0f, 4);
                                _dm["HaoHut"] = Math.Round(float.TryParse(dr[15]?.ToString(), out float haohut) ? haohut : 0f, 4);
                                _dm["MaVTGhep"] = targetRow["VietTat"] + "|" + RemoveVietnameseTone(dr[3].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper()) + "|" + (nr["MaMauVT"].ToString() == "" ? "00" : RemoveVietnameseTone(dr[6].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper())) + "|" + (nr["KhoVai"].ToString() == "" ? "00" : dr[8].ToString().Replace("\n", "").Replace("\r", ""));
                                _dm["STTIndex"] = index;
                                tblDinhMuc.Rows.Add(_dm);
                            }
                            index++;

                            #endregion

                        }
                        frmThuVienTongExcel frm = new frmThuVienTongExcel(dtbcreate, tblDinhMuc, tblMau, tblSize, _makh, _kh, _mahang, _tenhang, isDinhMuc);
                        frm.ShowDialog();
                        if (frm.DialogResult == DialogResult.OK)
                            this.DialogResult = DialogResult.OK;



                    }
                }
                else
                {

                    using (var source = new ExcelDataSource())
                    {
                        source.FileName = filePath;

                        string rangeExcel = "A6:J5000";


                        var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "A6:J5000");
                        var sourceOptions = new ExcelSourceOptions(worksheetSettings)
                        {
                            UseFirstRowAsHeader = true
                        };
                        source.SourceOptions = sourceOptions;

                        FieldInfo[] fields = new FieldInfo[10];
                        fields[0] = new FieldInfo { Name = "ColumnA", Type = typeof(string) };
                        fields[1] = new FieldInfo { Name = "ColumnB", Type = typeof(string) };
                        fields[2] = new FieldInfo { Name = "ColumnC", Type = typeof(string) };
                        fields[3] = new FieldInfo { Name = "ColumnD", Type = typeof(string) };
                        fields[4] = new FieldInfo { Name = "ColumnE", Type = typeof(string) };
                        fields[5] = new FieldInfo { Name = "ColumnF", Type = typeof(string) };
                        fields[6] = new FieldInfo { Name = "ColumnG", Type = typeof(string) };
                        fields[7] = new FieldInfo { Name = "ColumnH", Type = typeof(string) };
                        fields[8] = new FieldInfo { Name = "ColumnI", Type = typeof(string) };
                        fields[9] = new FieldInfo { Name = "ColumnJ", Type = typeof(string) };
                        source.Schema.AddRange(fields);
                        source.Fill();
                        tbl = source.ToDataTable();
                        DataRow[] rowsToDelete = tbl.AsEnumerable()
                                          .Where(row =>
                                              (row.IsNull(1) || string.IsNullOrWhiteSpace(row[1].ToString())) &&
                                              (row.IsNull(2) || string.IsNullOrWhiteSpace(row[2].ToString())) &&
                                              (row.IsNull(3) || string.IsNullOrWhiteSpace(row[3].ToString()))
                                          ).ToArray();

                        foreach (DataRow row in rowsToDelete)
                        {
                            tbl.Rows.Remove(row);
                        }

                        if (tbl == null || tbl.Rows.Count == 0)
                        {
                            XtraMessageBox.Show("Không có dữ liệu. Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        tbl = FillForwardThongSo(tbl);

                        if (!KiemTraVaXuatTatCaLoiThongSo(tbl)) return;



                        //if (!KiemTraDuLieuMauVT(tbl, 5, 6, 7)) return;

                        //check nhóm
                        if (!dtbcreate.Columns.Contains("TenDVVT"))
                            dtbcreate.Columns.Add("TenDVVT", typeof(string));
                        if (!dtbcreate.Columns.Contains("KhoVai"))
                            dtbcreate.Columns.Add("KhoVai", typeof(string));
                       
                        bool hopLe = KiemTraVaThemNhomMoi();
                        if (!hopLe)
                        {
                            return;
                        }

                        //if (!KiemTraMauHopLe(tbl, tblMauVT)) return;

                        DataTable tblDinhMuc = CreateTableDinhMuc();

                        //isDinhMuc = true;


                        int index = 0;
                        Func<string, string> chuanHoaTenNhom = s =>
                        {
                            if (string.IsNullOrEmpty(s))
                            {
                                return "";
                            }
                            // 1. Chuẩn hóa Unicode về dạng C (phổ biến nhất, tổ hợp sẵn)
                            string normalized = s.Normalize(System.Text.NormalizationForm.FormC);
                            // 2. Loại bỏ khoảng trắng
                            string noWhitespace = Regex.Replace(normalized, @"\s+", "");
                            // 3. Chuyển sang chữ hoa bất biến
                            return NormalizeString(noWhitespace.ToUpperInvariant());
                        };
                        foreach (DataRow dr in tbl.Rows)
                        {

                            #region thông số
                            DataRow nr = dtbcreate.NewRow();
                            nr["ID"] = 0;
                            nr["MaKH"] = _makh;
                            string tennhom = NormalizeString(dr[1].ToString());

                            string mavt = NormalizeString(dr[2].ToString());
                            string chitiet = NormalizeString(dr[3].ToString());
                            //string themau = NormalizeString(dr[5].ToString());
                            string mamauvt = NormalizeString(dr[4].ToString());
                            string mauvt = NormalizeString(dr[5].ToString());
                            string khovai = NormalizeString(dr[6].ToString());
                            string dvtKhoVai = NormalizeString(dr[7].ToString());
                            string tendvvt = NormalizeString(dr[8].ToString());
                            // 1. Chuẩn hóa chuỗi đầu vào `tennhom` TRƯỚC khi truy vấn
                            string tenNhomChuanHoa = chuanHoaTenNhom(tennhom);

                            // 2. Tìm dòng dữ liệu bằng cách so sánh các giá trị đã được chuẩn hóa
                            DataRow targetRow = _tblNhom.AsEnumerable()
                                                        .FirstOrDefault(row => chuanHoaTenNhom(row.Field<string>("ChungLoaiVatTu")) == tenNhomChuanHoa);
                            if(targetRow.ItemArray[0] != "NHOM_2")
                            {

                            }
                            if (targetRow == null)
                            {
                                XtraMessageBox.Show("Vui lòng xử lý chủng loại không hợp lệ trước khi thực hiện Import!!!.", "Đã hủy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (targetRow["NPL"].ToString() == "True" && string.IsNullOrEmpty(dr[6].ToString().Replace("\n", "").Replace("\r", "")))
                            {
                                XtraMessageBox.Show("Cần nhập đẩy đủ Khổ/Size đối với nguyên liệu ở dòng " + dr[0].ToString(), "Đã hủy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (targetRow["NPL"].ToString() == "True" && string.IsNullOrEmpty(dr[5].ToString().Replace("\n", "").Replace("\r", "")))
                            {
                                XtraMessageBox.Show("Cần nhập đẩy đủ màu vật tư đối với nguyên liệu ở dòng " + dr[0].ToString(), "Đã hủy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            DataRow targetRowMaVT = tblVatTu.AsEnumerable()
                                .FirstOrDefault(row =>
                                   NormalizeString(row["MaVT"]?.ToString()) == mavt &&
                                    NormalizeString(row["ChiTiet"]?.ToString()).ToUpper() == chitiet
                                );

                            DataRow targetRowMauVT = tblMauVT.AsEnumerable()
                                .FirstOrDefault(row =>
                                  NormalizeString(row["MaMauVT"]?.ToString()) == mamauvt &&
                                   NormalizeString(row["MauVT"]?.ToString()) == mauvt
                                );
                            if (Convert.ToInt16(dr[0]) >= 98)
                            {

                            }
                            string KhoVaiView = khovai == dvtKhoVai ? khovai : khovai + dvtKhoVai;
                            KhoVaiView = NormalizeString(KhoVaiView);
                            DataRow targetRowKV = null;
                            if (targetRow["NPL"].ToString() == "True")
                            {
                                targetRowKV = tblKV.AsEnumerable()
                                .FirstOrDefault(row => NormalizeString(row["TenNhom"]?.ToString()) == tennhom &&
                                    NormalizeString(row["KhoVai"]?.ToString()).ToUpper().Replace(" ", "") == khovai
                                  && NormalizeString(row["TenDVVT"]?.ToString()).ToUpper() == dvtKhoVai
                                );
                            }
                            else
                            {
                                targetRowKV = tblKV.AsEnumerable()
                               .FirstOrDefault(row => NormalizeString(row["TenNhom"]?.ToString()).ToUpper() == tennhom &&
                                  NormalizeString(row["KhoVaiView"]?.ToString()).ToUpper().Replace(" ", "") == KhoVaiView

                               );
                            }


                            DataRow targetRowDV = tblDV.AsEnumerable()
                                .FirstOrDefault(row =>
                                   NormalizeString(row["TenDVVT"]?.ToString()).ToUpper() == tendvvt
                                );
                            nr["MaNhom"] = targetRow["MaCLVT"].ToString();
                            nr["TenNhom"] = targetRow["ChungLoaiVatTu"].ToString();
                            nr["Sort"] = targetRow["Sort"].ToString();
                            nr["NPL"] = targetRow["NPL"];

                            nr["IsNew"] = 2;
                            nr["MaVT"] = RemoveVietnameseTone(dr[2].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper());
                            nr["ChiTiet"] = dr[3].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper();
                            nr["MaMauVT"] = RemoveVietnameseTone(dr[4].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper());
                            nr["MauVT"] = dr[5].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper();
                            nr["KhoVai"] = dr[6].ToString().Replace("\n", "").Replace("\r", "");
                            nr["KhoVaiHienThi"] = dr[6].ToString().Replace("\n", "").Replace("\r", "") + " " + dr[7].ToString().Replace("\n", "").Replace("\r", "");
                            nr["TenDVKV"] = dr[7].ToString().Replace("\n", "").Replace("\r", "");
                            nr["TenDVVT"] = dr[8].ToString().Replace("\n", "").Replace("\r", "");
                            nr["GhiChu"] = dr[9].ToString();
                            if (targetRowMaVT != null)
                            {
                                nr["MaVTID"] = targetRowMaVT["MaVTID"];
                            }
                            if (targetRowMauVT != null)
                            {
                                nr["MauVTID"] = targetRowMauVT["MauVTID"];
                            }

                            if (targetRowDV != null)
                            {
                                nr["MaDVVT"] = targetRowDV["MaDVVT"];

                            }
                            if (targetRowKV != null)
                            {
                                nr["KhoVaiID"] = targetRowKV["KhoVaiID"];
                            }
                            else
                            {

                                float KhoVaiMet = 0.0f;
                                if (targetRow["NPL"]?.ToString()?.ToLower() == "true")
                                {
                                    float.TryParse(targetRowDV["Tile"]?.ToString(), out float TiLe);
                                    if (float.TryParse(dr[6].ToString().Replace("\n", "").Replace("\r", ""), out float SoKV))
                                    {
                                        KhoVaiMet = SoKV * TiLe;
                                    }
                                }
                                DataRow targetRowDVKV = tblDV.AsEnumerable()
                              .FirstOrDefault(row =>
                                  NormalizeString(row["TenDVVT"]?.ToString()).ToUpper() == dvtKhoVai
                              );
                                DataRow targetRowTheKV = tblTheKhoVai.AsEnumerable()
                                 .FirstOrDefault(row =>
                                    NormalizeString(row["MaCLVT"]?.ToString()) == NormalizeString(targetRow["MaCLVT"]?.ToString()) &&
                                     NormalizeString(row["ChungLoaiVatTu"]?.ToString()).ToUpper() == NormalizeString(targetRow["ChungLoaiVatTu"]?.ToString())
                                 );
                                if (targetRowTheKV == null)
                                {
                                    XtraMessageBox.Show($"Lỗi không tìm thấy thẻ Size/Khổ của chủng loại {targetRow["ChungLoaiVatTu"]} ở dòng " + dr[0].ToString(), "Đã hủy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                if (targetRowDVKV == null && targetRow["NPL"]?.ToString()?.ToLower() == "true")
                                {
                                    XtraMessageBox.Show($"Lỗi không tìm thấy đơn vị của khổ vải {KhoVaiView} ở dòng " + dr[0].ToString(), "Đã hủy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                bool IsAddKV = tblKhoVai.AsEnumerable().Any(x =>
                                    NormalizeString(x["KhoVai"]?.ToString()) == NormalizeString(khovai) &&
                                    NormalizeString(x["MaNhom"]?.ToString()) == NormalizeString(targetRow["MaCLVT"]?.ToString()) &&
                                    NormalizeString(x["MaTheKhoVai"]?.ToString()) == NormalizeString(targetRowTheKV["MaTheKhoVai"]?.ToString())
                                );

                                if (!IsAddKV)
                                {
                                    DataRow rowKV = tblKhoVai.NewRow();
                                    rowKV["ID"] = 0;
                                    if (targetRow["NPL"]?.ToString()?.ToLower() == "true")
                                    {
                                        rowKV["KhoVai"] = dr[6].ToString().Replace("\n", "").Replace("\r", "");
                                        rowKV["MaDVVT"] = targetRowDVKV["MaDVVT"];
                                    }
                                    else
                                    {
                                        rowKV["KhoVai"] = dr[6].ToString().Replace("\n", "").Replace("\r", "") + " " + dr[7].ToString().Replace("\n", "").Replace("\r", "");
                                        rowKV["MaDVVT"] = null;
                                    }
                                    //nr["KhoVaiID"] =;

                                    rowKV["NPL"] = targetRow["NPL"];
                                    rowKV["MaNhom"] = targetRow["MaCLVT"];
                                    rowKV["GhiChu"] = null;
                                    rowKV["KhoVaiMet"] = KhoVaiMet;
                                    rowKV["MaTheKhoVai"] = targetRowTheKV["MaTheKhoVai"];
                                    tblKhoVai.Rows.Add(rowKV);
                                    if (!string.IsNullOrEmpty(khovai))
                                    {
                                        lstKV_Save.Add($"• <color=blue>Dòng {dr[0].ToString()}</color> Khổ/Size:<b><color=red>{KhoVaiView}</color></b> - Chủng Loại :<b><color=red>{System.Text.RegularExpressions.Regex.Replace(Convert.ToString(targetRow["ChungLoaiVatTu"]), @"\s+", "").ToUpper()}</color></b>\n");

                                    }

                                }

                            }

                            nr["MaVTGhep"] = targetRow["VietTat"] + "|" + RemoveVietnameseTone(dr[3].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper()) + "|" + (nr["MaMauVT"].ToString() == "" ? "00" : RemoveVietnameseTone(dr[6].ToString().Replace("\n", "").Replace("\r", "").Trim().ToUpper())) + "|" + (nr["KhoVai"].ToString() == "" ? "00" : dr[8].ToString().Replace("\n", "").Replace("\r", ""));
                            nr["STTIndex"] = index;
                            bool isDuplicate = dtbcreate.AsEnumerable().Any(row =>
                                                    row["MaNhom"].ToString().Replace(" ", "").ToUpper() == nr["MaNhom"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["MaVT"].ToString().Replace(" ", "").ToUpper() == nr["MaVT"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["ChiTiet"].ToString().Replace(" ", "").ToUpper() == nr["ChiTiet"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["MaMauVT"].ToString().Replace(" ", "").ToUpper() == nr["MaMauVT"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["MauVT"].ToString().Replace(" ", "").ToUpper() == nr["MauVT"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["KhoVai"].ToString().Replace(" ", "").ToUpper() == nr["KhoVai"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["TenDVKV"].ToString().Replace(" ", "").ToUpper() == nr["TenDVKV"].ToString().Replace(" ", "").ToUpper() &&
                                                    row["TenDVVT"].ToString().Replace(" ", "").ToUpper() == nr["TenDVVT"].ToString().Replace(" ", "").ToUpper()
                                                    );

                            if (!isDuplicate)
                                dtbcreate.Rows.Add(nr);
                            else
                            {
                                Console.WriteLine("Dòng này bị double lại: " + dr[0].ToString());
                            }
                            #endregion
                        }

                        if (tblKhoVai.Rows.Count > 0)
                        {
                            if (lstKV_Save.Count > 0)
                            {
                                string htmlMessage = "<b><color=red>Bạn có muốn lưu các khổ/size chưa có trong thư viện này không?</color></b>\n\n" +
                                                     string.Join("\n", lstKV_Save);

                                DialogResult messResult = ShowYesNoWarning(htmlMessage);

                                if (messResult != DialogResult.Yes)
                                    return;
                            }

                            string url = $"{URL}ERPThuVienVT/PostKV?action=Post";
                            string msResult = Task.Run(async () =>
                            {
                                return await _clientExtension.PostAsync(url, tblKhoVai);
                            }).Result;

                            if (msResult.ToLower() == "true")
                            {
                                clsWaitForm.ShowSuccessForm(this, 3000);
                                loadKhoVai();
                                return;
                            }
                        }
                        frmThuVienTongExcel frm = new frmThuVienTongExcel(dtbcreate, tblDinhMuc, null, null, _makh, _kh, _mahang, _tenhang, isDinhMuc);
                        frm.ShowDialog();
                        if (frm.DialogResult == DialogResult.OK)
                            this.DialogResult = DialogResult.OK;





                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
        private DialogResult ShowYesNoWarning(string message)
        {
            XtraMessageBoxArgs args = new XtraMessageBoxArgs
            {
                Caption = Resources.Warning,
                AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                Text = message,
                Buttons = new DialogResult[] { DialogResult.Yes, DialogResult.No },
                Icon = System.Drawing.SystemIcons.Warning,
                MessageBeepSound = MessageBeepSound.Warning,
                DefaultButtonIndex = 1 // Mặc định là "No"
            };

            return XtraMessageBox.Show(args);
        }
        public bool KiemTraDuLieuMauVT(DataTable dataTable, int colIndex1, int colIndex2, int colIndex3)
        {
            if (colIndex1 < 0 || colIndex1 >= dataTable.Columns.Count ||
                colIndex2 < 0 || colIndex2 >= dataTable.Columns.Count)
            {

                return false;
            }

            List<int> dongLoiIndexes = new List<int>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DataRow row = dataTable.Rows[i];
                bool cot1CoDuLieu = row[colIndex1] != null && !string.IsNullOrWhiteSpace(row[colIndex1].ToString());
                bool cot2CoDuLieu = row[colIndex2] != null && !string.IsNullOrWhiteSpace(row[colIndex2].ToString());

                bool cot3CoDuLieu = row[colIndex3] != null && !string.IsNullOrWhiteSpace(row[colIndex3].ToString());

                if (!cot1CoDuLieu && !cot2CoDuLieu && !cot3CoDuLieu)
                {
                    dongLoiIndexes.Add(i + 1);
                }
            }


            if (dongLoiIndexes.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Thiếu dữ liệu cột Mã màu vật tư và Màu vật tư:");
                sb.AppendLine(string.Join(", ", dongLoiIndexes));

                sb.AppendLine("\nVui lòng kiểm tra lại các dòng: ");
                XtraMessageBox.Show(
                    sb.ToString(),
                    "Lỗi Dữ Liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }
        private DataTable FillForward(DataTable dt)
        {
            if (dt == null || dt.Rows.Count < 2)
            {
                return dt;
            }
            foreach (DataColumn col in dt.Columns)
            {

                for (int i = 1; i < dt.Rows.Count; i++)
                {

                    string currentValue = dt.Rows[i][col].ToString().Trim();


                    if (currentValue == "\"\"")
                    {
                        object previousValue = dt.Rows[i - 1][col];
                        dt.Rows[i][col] = previousValue;
                    }
                }
            }
            //gán cột mã màu vt = mà mãu
            if (dt.Columns.Count >= 8)
            {
                DataColumn col6 = dt.Columns[6];
                DataColumn col7 = dt.Columns[7];

                foreach (DataRow row in dt.Rows)
                {
                    string col6Value = row[col6].ToString().Trim();
                    if (string.IsNullOrWhiteSpace(col6Value))
                    {
                        row[col6] = row[col7];
                    }
                }
            }
            return dt;

        }
        private DataTable FillForwardThongSo(DataTable dt)
        {
            if (dt == null || dt.Rows.Count < 2)
            {
                return dt;
            }
            foreach (DataColumn col in dt.Columns)
            {

                for (int i = 1; i < dt.Rows.Count; i++)
                {

                    string currentValue = dt.Rows[i][col].ToString().Trim();


                    if (currentValue == "\"\"")
                    {
                        object previousValue = dt.Rows[i - 1][col];
                        dt.Rows[i][col] = previousValue;
                    }
                }
            }
            //gán cột mã màu vt = mà mãu
            if (dt.Columns.Count >= 5)
            {
                DataColumn col4 = dt.Columns[4];
                DataColumn col5 = dt.Columns[5];

                foreach (DataRow row in dt.Rows)
                {
                    string col4Value = row[col4].ToString().Trim();
                    if (string.IsNullOrWhiteSpace(col4Value))
                    {
                        row[col4] = row[col5];
                    }
                }
            }
            return dt;

        }
        public bool KiemTraVaXuatTatCaLoi(DataTable tbl)
        {

            List<int> dongLoiIndexes = new List<int>();
            foreach (DataRow dr in tbl.Rows)
            {

                if (string.IsNullOrWhiteSpace(dr[1]?.ToString()) ||
                    (string.IsNullOrWhiteSpace(dr[2]?.ToString()) && isDinhMuc) ||
                    string.IsNullOrWhiteSpace(dr[3]?.ToString()) ||
                    string.IsNullOrWhiteSpace(dr[4]?.ToString()))
                {

                    int rowIndex = tbl.Rows.IndexOf(dr) + 1;
                    dongLoiIndexes.Add(rowIndex);
                }
            }
            if (dongLoiIndexes.Count > 0)
            {

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Phát hiện dữ liệu trống (Các cột 1" + (isDinhMuc ? ",2" : "") + ",3,4) ở các dòng sau:");
                sb.AppendLine(string.Join(", ", dongLoiIndexes));

                sb.AppendLine("\nVui lòng kiểm tra và điền đầy đủ thông tin.");
                XtraMessageBox.Show(
                    sb.ToString(),
                    "Lỗi Dữ Liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }
        public bool KiemTraVaXuatTatCaLoiThongSo(DataTable tbl)
        {

            List<int> dongLoiIndexes = new List<int>();
            foreach (DataRow dr in tbl.Rows)
            {

                if (string.IsNullOrWhiteSpace(dr[1]?.ToString()) ||

                    string.IsNullOrWhiteSpace(dr[2]?.ToString()) ||
                    string.IsNullOrWhiteSpace(dr[3]?.ToString()))
                {

                    int rowIndex = tbl.Rows.IndexOf(dr) + 1;
                    dongLoiIndexes.Add(rowIndex);
                }
            }
            if (dongLoiIndexes.Count > 0)
            {

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Phát hiện dữ liệu trống (Các cột 1,2,3) ở các dòng sau:");
                sb.AppendLine(string.Join(", ", dongLoiIndexes));

                sb.AppendLine("\nVui lòng kiểm tra và điền đầy đủ thông tin.");
                XtraMessageBox.Show(
                    sb.ToString(),
                    "Lỗi Dữ Liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }
        public static bool KiemTraHopLeHaoHut(DataTable tbl)
        {

            List<int> dongLoiIndexes = new List<int>();


            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                DataRow dr = tbl.Rows[i];
                object valueObject = dr[15];
                if (valueObject != null && !string.IsNullOrEmpty(valueObject.ToString()))
                {
                    string valueString = valueObject.ToString().Trim();
                    if (!float.TryParse(valueString, out float _))
                    {
                        dongLoiIndexes.Add(i + 1);
                    }
                }
            }
            if (dongLoiIndexes.Count > 0)
            {

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Lỗi định dạng dữ liệu: Giá trị ở cột % Hao hụt không phải là số. Lỗi xảy ra tại các dòng sau:");

                sb.AppendLine(string.Join(", ", dongLoiIndexes));

                sb.AppendLine("\nVui lòng kiểm tra và sửa lại dữ liệu.");

                XtraMessageBox.Show(
                    sb.ToString(),
                    "Lỗi Định Dạng Dữ Liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false; // Trả về false vì phát hiện lỗi
            }

            // Nếu không có lỗi nào được tìm thấy
            return true; // Trả về true vì tất cả dữ liệu đều hợp lệ
        }
        //public bool KiemTraDuLieuSoLuong(DataTable tbl)
        //{

        //    List<int> dongLoiIndexes = new List<int>();

        //    // Bắt đầu duyệt từ dòng đầu tiên (index 0)
        //    for (int i = 0; i < tbl.Rows.Count; i++)
        //    {
        //        DataRow dr = tbl.Rows[i];
        //        float soLuong;
        //        if (!float.TryParse(dr[11].ToString(), out soLuong) || soLuong <= 0)
        //        {
        //            dongLoiIndexes.Add(i + 1);
        //        }
        //    }

        //    if (dongLoiIndexes.Count > 0)
        //    {
        //        StringBuilder sb = new StringBuilder();

        //        sb.AppendLine("Cột định mức phải là kiểu số và lớn hơn 0 (Ví dụ: 0.99).");
        //        sb.AppendLine("\nPhát hiện lỗi cột định mức ở các dòng sau:");
        //        sb.AppendLine(string.Join(", ", dongLoiIndexes));

        //        sb.AppendLine("\nVui lòng kiểm tra và sửa lại dữ liệu.");
        //        XtraMessageBox.Show(
        //            sb.ToString(),
        //            "Lỗi Dữ Liệu",
        //            MessageBoxButtons.OK,
        //            MessageBoxIcon.Warning);

        //        return false; // Báo hiệu kiểm tra thất bại
        //    }

        //    return true; // Báo hiệu tất cả dữ liệu đều hợp lệ
        //}
        public bool KiemTraDuLieuDinhMuc(DataTable tbl)
        {
            int soDongDaNhap = 0;
            List<int> dongLoiSoAm = new List<int>();
            List<int> dongLoiKhongPhaiSo = new List<int>();
            List<int> dongBoTrong = new List<int>();
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                DataRow dr = tbl.Rows[i];
                object value = dr[14];
                if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    dongBoTrong.Add(i + 1);
                    continue;
                }
                soDongDaNhap++;
                float dinhMuc;
                if (!float.TryParse(value.ToString(), out dinhMuc))
                {
                    dongLoiKhongPhaiSo.Add(i + 1);
                }
                else if (dinhMuc <= 0)
                {
                    dongLoiSoAm.Add(i + 1);
                }
            }
            if (soDongDaNhap == 0)
            {
                isDinhMuc = false;
                return true;
            }


            if (soDongDaNhap > 0 && dongBoTrong.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Lỗi thiếu dữ liệu định mức.");
                sb.AppendLine("Nếu đã nhập định mức cho một dòng, tất cả các dòng còn lại cũng phải được nhập.");
                sb.AppendLine("\nPhát hiện các dòng sau bị bỏ trống:");
                sb.AppendLine(string.Join(", ", dongBoTrong));

                XtraMessageBox.Show(sb.ToString(), "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false; // Dữ liệu không hợp lệ
            }

            // Trường hợp 3: Tất cả các dòng đều đã được nhập, nhưng có lỗi định dạng hoặc giá trị
            bool coLoi = dongLoiKhongPhaiSo.Count > 0 || dongLoiSoAm.Count > 0;
            if (coLoi)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Dữ liệu định mức không hợp lệ.");

                if (dongLoiKhongPhaiSo.Count > 0)
                {
                    sb.AppendLine("\nCột định mức phải là kiểu số. Lỗi tại các dòng:");
                    sb.AppendLine(string.Join(", ", dongLoiKhongPhaiSo));
                }

                if (dongLoiSoAm.Count > 0)
                {
                    sb.AppendLine("\nGiá trị định mức phải lớn hơn 0. Lỗi tại các dòng:");
                    sb.AppendLine(string.Join(", ", dongLoiSoAm));
                }

                XtraMessageBox.Show(sb.ToString(), "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false; // Dữ liệu không hợp lệ
            }



            return true; // Dữ liệu hoàn toàn hợp lệ
        }
        private bool KiemTraDuLieuToanBo(DataTable sourceTable, DataTable tblMau, DataTable tblSize)
        {

            var validMau = new HashSet<string>(
        tblMau.AsEnumerable().Select(row => row.Field<string>("TenMau").Trim())
              .Concat(tblMau.AsEnumerable().Select(row => row.Field<string>("CodeMau").Trim())),
        StringComparer.OrdinalIgnoreCase
    );
            var validNhomSize = new HashSet<string>(
                tblSize.AsEnumerable().Select(row => row.Field<string>("NhomSize").Trim()),
                StringComparer.OrdinalIgnoreCase
            );

            var validSize = new HashSet<string>(
                tblSize.AsEnumerable().Select(row => row.Field<string>("TenSize").Trim()),
                StringComparer.OrdinalIgnoreCase
            );
            var invalidMauSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var invalidInseamSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var invalidSizeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow dr in sourceTable.Rows)
            {
                string mauSanPhamInput = dr[11]?.ToString();
                string inseamInput = dr[12]?.ToString();
                string sizeInput = dr[13]?.ToString();
                if (!string.IsNullOrWhiteSpace(mauSanPhamInput))
                {
                    foreach (string mau in mauSanPhamInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string trimmedMau = mau.Trim();
                        if (!validMau.Contains(trimmedMau))
                        {
                            invalidMauSet.Add(trimmedMau);
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(inseamInput))
                {
                    foreach (string inseam in inseamInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string trimmedInseam = inseam.Trim();
                        if (!validNhomSize.Contains(trimmedInseam))
                        {
                            invalidInseamSet.Add(trimmedInseam);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(sizeInput))
                {
                    foreach (string size in sizeInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string trimmedSize = size.Trim();
                        if (!validSize.Contains(trimmedSize))
                        {
                            invalidSizeSet.Add(trimmedSize);
                        }
                    }
                }
            }
            var errorMessages = new List<string>();

            if (invalidMauSet.Any())
            {
                string mauErrors = string.Join(", ", invalidMauSet);
                errorMessages.Add($"Các màu sau không tồn tại trong mã hàng: {mauErrors}.");
            }

            if (invalidInseamSet.Any())
            {
                string inseamErrors = string.Join(", ", invalidInseamSet);
                errorMessages.Add($"Các inseam sau không tồn tại trong mã hàng: {inseamErrors}.");
            }

            if (invalidSizeSet.Any())
            {
                string sizeErrors = string.Join(", ", invalidSizeSet);
                errorMessages.Add($"Các size sau không tồn tại trong mã hàng: {sizeErrors}.");
            }
            if (errorMessages.Any())
            {
                string fullErrorMessage = string.Join("\n", errorMessages);
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    fullErrorMessage,
                    "Dữ Liệu Không Hợp Lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }
            return true;
        }
        public bool KiemTraChungLoaiChiTiet(DataTable tbl, DataTable tblNhom)
        {

            Func<string, string> chuanHoaTenNhom = s =>
            {
                if (string.IsNullOrEmpty(s))
                {
                    return "";
                }
                string normalized = s.Normalize(System.Text.NormalizationForm.FormC);
                string noWhitespace = Regex.Replace(normalized, @"\s+", "");
                return noWhitespace.ToUpperInvariant();
            };
            var nhomSet = new HashSet<string>(
                tblNhom.AsEnumerable()
                       .Select(r => $"Chủng loại:{chuanHoaTenNhom(r[1].ToString())}|Chi Tiết:{chuanHoaTenNhom(r[2].ToString())}")
            );

            var loi = new List<string>();

            foreach (DataRow row in tbl.Rows)
            {
                var key = $"Chủng loại:{chuanHoaTenNhom(row[1].ToString())}|Chi Tiết:{chuanHoaTenNhom(row[2].ToString())}";
                if (!nhomSet.Contains(key))
                    loi.Add(key);
            }

            if (loi.Any())
            {
                string fullErrorMessage = string.Join("\n", loi);
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    fullErrorMessage,
                    "Dữ Liệu Chủng loại chi tiết không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }
            return true;
        }
        public bool KiemTraMauHopLe(DataTable tbl, DataTable tblMau)
        {

            Func<string, string> chuanHoaTenNhom = s =>
            {
                if (string.IsNullOrEmpty(s))
                {
                    return "";
                }
                string normalized = s.Normalize(System.Text.NormalizationForm.FormC);
                string noWhitespace = Regex.Replace(normalized, @"\s+", "");
                return noWhitespace.ToUpperInvariant();
            };
            var nhomSet = new HashSet<string>(
                tblMau.AsEnumerable()
                       .Select(r => $"Code màu:{chuanHoaTenNhom(r[1].ToString())}|Màu:{chuanHoaTenNhom(r[2].ToString())}")
            );

            var loi = new List<string>();

            foreach (DataRow row in tbl.Rows)
            {
                var key = $"Code màu:{chuanHoaTenNhom(row[4].ToString())}|Màu:{chuanHoaTenNhom(row[5].ToString())}";
                if (!nhomSet.Contains(key))
                    loi.Add(key);
            }

            if (loi.Any())
            {
                string fullErrorMessage = string.Join("\n", loi);
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    fullErrorMessage,
                    "Dữ Liệu Màu vật tư không hợp lệ (không có trong thư viện màu vật tư)",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }
            return true;
        }
        private bool KiemTraVaThemNhomMoi()
        {


            Func<string, string> chuanHoaTenNhom = s =>
            {
                if (string.IsNullOrEmpty(s))
                {
                    return "";
                }
                string normalized = s.Normalize(System.Text.NormalizationForm.FormC);
                string noWhitespace = Regex.Replace(normalized, @"\s+", "");
                return noWhitespace.ToUpperInvariant();
            };
            var nhomDaCo = new HashSet<string>(_tblNhom.AsEnumerable()
                                                       .Select(r => chuanHoaTenNhom(r.Field<string>(1)))
                                                       .Where(n => !string.IsNullOrEmpty(n)));
            var nhomMoiCanThem = tbl.AsEnumerable()
                                     .Select(r => r.Field<string>(1))
                                     .Where(n => !string.IsNullOrEmpty(n))
                                     .Select(n => new { TenGoc = n, TenChuanHoa = chuanHoaTenNhom(n) })
                                     .Where(x => !nhomDaCo.Contains(x.TenChuanHoa))
                                     .GroupBy(x => x.TenChuanHoa)
                                     .Select(g => g.First().TenGoc) // Lấy lại tên gốc đầu tiên của nhóm mới
                                     .ToList();
            if (nhomMoiCanThem.Count == 0)
            {
                return true;
            }
            string danhSachNhomMoi = string.Join("|", nhomMoiCanThem);
            string thongBao = $"Các chủng loại sau chưa tồn tại trong hệ thống:\n\n" +
                              $"[{danhSachNhomMoi}]\n\n" +
                              $"Bạn có muốn thêm các chủng loại này không?";

            DialogResult result = XtraMessageBox.Show(this, thongBao, "Phát hiện chủng loại mới",
                                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);


            if (result == DialogResult.Yes)
            {
                frmChungLoaiVatTu frm = new frmChungLoaiVatTu(danhSachNhomMoi);
                //frmChungLoaiVatTu frm = new frmChungLoaiVatTu();
                frm.WindowState = FormWindowState.Maximized;
                frm.ShowDialog();
                loadNhom();
                loadTheKhoVai();
                if (frm.Result != true)
                {
                    XtraMessageBox.Show("Vui lòng xử lý chủng loại không hợp lệ trước khi thực hiện Import!!!.", "Đã hủy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                return true;
            }
            else
            {

                XtraMessageBox.Show("Vui lòng xử lý nhóm không hợp lệ trước khi thực hiện Import!!!.", "Đã hủy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
        public DataTable getData()
        {
            return dtbcreate;
        }

        private string NormalizeString(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            string normalized = input.Normalize(NormalizationForm.FormKD);

            var sb = new System.Text.StringBuilder();
            foreach (char c in normalized)
            {
                if (char.IsLetterOrDigit(c) || "xX*-/".Contains(c))
                    sb.Append(char.ToUpperInvariant(c));
            }

            return System.Text.RegularExpressions.Regex.Replace(sb.ToString(), @"\s+", "").ToUpper();
        }
        private DataTable CreateTableDinhMuc()
        {

            DataTable tblDinhMuc = new DataTable("tblDinhMuc");
            tblDinhMuc.Columns.Add("ID", typeof(int));
            tblDinhMuc.Columns.Add("MaHang", typeof(string));
            tblDinhMuc.Columns.Add("Makh", typeof(string));
            tblDinhMuc.Columns.Add("MaNhom", typeof(string));
            tblDinhMuc.Columns.Add("TenNhom", typeof(string));
            tblDinhMuc.Columns.Add("NPL", typeof(bool));
            tblDinhMuc.Columns.Add("Sort", typeof(int));
            tblDinhMuc.Columns.Add("MaVTID", typeof(string));
            tblDinhMuc.Columns.Add("MaVT", typeof(string));
            tblDinhMuc.Columns.Add("ChiTiet", typeof(string));
            tblDinhMuc.Columns.Add("MaDVVT", typeof(string));
            tblDinhMuc.Columns.Add("TenDVVT", typeof(string));
            tblDinhMuc.Columns.Add("MauVTID", typeof(string));
            tblDinhMuc.Columns.Add("MaMauVT", typeof(string));
            tblDinhMuc.Columns.Add("MauVT", typeof(string));
            tblDinhMuc.Columns.Add("KhoVaiID", typeof(string));
            tblDinhMuc.Columns.Add("KhoVai", typeof(string));
            tblDinhMuc.Columns.Add("MaMau", typeof(string));
            tblDinhMuc.Columns.Add("TenMau", typeof(string));
            tblDinhMuc.Columns.Add("MaVTGhep", typeof(string));
            tblDinhMuc.Columns.Add("MaNhomSize", typeof(string));
            tblDinhMuc.Columns.Add("NhomSize", typeof(string));
            tblDinhMuc.Columns.Add("MaSize", typeof(string));
            tblDinhMuc.Columns.Add("TenSize", typeof(string));
            tblDinhMuc.Columns.Add("DinhMuc", typeof(float));
            tblDinhMuc.Columns.Add("HaoHut", typeof(float));
            tblDinhMuc.Columns.Add("STTCode", typeof(int));
            tblDinhMuc.Columns.Add("STTIndex", typeof(int));
            tblDinhMuc.Columns.Add("IsActive", typeof(bool));
            tblDinhMuc.Columns.Add("MaNhomChiTiet", typeof(string));
            tblDinhMuc.Columns.Add("TenDVKV", typeof(string));
            tblDinhMuc.Columns.Add("KhoVaiHienThi", typeof(string));
            return tblDinhMuc;
        }
        private DataTable CreateTableMau(string makh, string mahang)
        {
            string url = $"{URL}ERPVatTuBOM/GetChung?Action=GETBANGMAU&para={makh}&para1={mahang}";

            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return null;
            DataTable tblMau = JsonConvert.DeserializeObject<DataTable>(json);
            return tblMau;
        }
        private DataTable CreateTableSize(string makh, string mahang)
        {
            string url = $"{URL}ERPVatTuBOM/GetChung?Action=GETSIZE&para={makh}&para1={mahang}";

            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return null;
            DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(json);
            return tblSize;
        }
        private string getMaHang(string tenhang)
        {
            try
            {
                string url = $"{URL}ERPVatTuBOM/GetChung?Action=GETMAHANG&para={_makh}";

                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return "";
                DataTable tblMaHang = JsonConvert.DeserializeObject<DataTable>(json);

                if (tblMaHang == null || tblMaHang.Rows.Count == 0) return "";
                string _mh = string.Empty;
                _mh = tblMaHang.AsEnumerable()
                         .Where(row => row["TenHang"].ToString().Trim().Replace(" ", "") == tenhang.ToString().Trim().Replace(" ", ""))
                         .Select(row => row["MaHang"].ToString())
                         .FirstOrDefault();
                _tenhang = tblMaHang.AsEnumerable()
                         .Where(row => row["TenHang"].ToString().Trim().Replace(" ", "") == tenhang.ToString().Trim().Replace(" ", ""))
                         .Select(row => row["TenHang"].ToString())
                         .FirstOrDefault();
                return _mh;
            }
            catch (Exception ex)
            {
                return "";

            }

        }
        private string loadKhachHang(string tenkh)
        {

            try
            {
                string url = $"{URL}ERPVatTuBOM/GetChung?Action=GETKHACHHANGEXCEL";

                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return "";
                DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(json);

                if (tblKH == null || tblKH.Rows.Count == 0) return "";
                _makh = tblKH.AsEnumerable()
                         .Where(row => row["TenKH"].ToString().Trim().Replace(" ", "") == tenkh.ToString().Trim().Replace(" ", ""))
                         .Select(row => row["MaKH"].ToString())
                         .FirstOrDefault();
                _kh = tblKH.AsEnumerable()
                         .Where(row => row["TenKH"].ToString().Trim().Replace(" ", "") == tenkh.ToString().Trim().Replace(" ", ""))
                         .Select(row => row["TenKH"].ToString())
                         .FirstOrDefault();
                txtKH.Text = _kh;

                return _makh;
            }
            catch (Exception ex)
            {
                return "";

            }
        }
    }
}