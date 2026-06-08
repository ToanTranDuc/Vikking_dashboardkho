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
using System.Globalization;

namespace NtbSoft.ERP.Win.Modules.Erp.Kho
{
    public partial class frmERPNhapKhoNPL_Excel : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                              new System.Configuration.AppSettingsReader();
        string URL = string.Empty;


        CustomImportDataExcel _customImport;

        public event EventHandler<string> DataClosed;
        System.Data.DataTable tbl_sheet;
        private HttpClientExtension _clientExtension;
        DataRow _r;
        DataTable _dt = new DataTable();
        DataTable tblNhapKho = new DataTable();

        string _manpl = string.Empty;
        DataTable tbl = new DataTable();
        public frmERPNhapKhoNPL_Excel()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));

            _customImport = new CustomImportDataExcel();
            textEdit1.Enabled = false;
            _clientExtension = new HttpClientExtension();
        }
        public frmERPNhapKhoNPL_Excel(DataRow r, DataTable dt)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _r = r;
            _dt = dt;

            _customImport = new CustomImportDataExcel();
            textEdit1.Enabled = false;
            _clientExtension = new HttpClientExtension();
            CreateSearchLookUpCayVai();
            CreateTableNhapKho();
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

        private void CreateTableNhapKho()
        {
            tblNhapKho = new DataTable("tblNhapKho");
            tblNhapKho.Columns.Add("ID", typeof(int));
            tblNhapKho.Columns.Add("SoLoID", typeof(string));
            tblNhapKho.Columns.Add("MaNPL", typeof(string));
            tblNhapKho.Columns.Add("MaVTID", typeof(string));
            tblNhapKho.Columns.Add("MaMauVT", typeof(string));
            tblNhapKho.Columns.Add("MauVT", typeof(string));
            tblNhapKho.Columns.Add("SoKien", typeof(string));
            tblNhapKho.Columns.Add("SoLot", typeof(string));
            tblNhapKho.Columns.Add("MaHaiQuan", typeof(string));
            tblNhapKho.Columns.Add("MaKeToan", typeof(string));
            tblNhapKho.Columns.Add("SoGhiDauCay", typeof(decimal));
            tblNhapKho.Columns.Add("NW", typeof(decimal));
            tblNhapKho.Columns.Add("GW", typeof(decimal));
            tblNhapKho.Columns.Add("BarCode", typeof(string));
            tblNhapKho.Columns.Add("GhiChu", typeof(string));
            tblNhapKho.Columns.Add("IsNPL", typeof(bool));
            tblNhapKho.Columns.Add("KhoVaiID", typeof(string));
            tblNhapKho.Columns.Add("SoKienParent", typeof(string));
            tblNhapKho.Columns.Add("SoLuongThucTe", typeof(decimal));
            tblNhapKho.Columns.Add("DonGia", typeof(decimal));
            tblNhapKho.Columns.Add("ThanhTien", typeof(decimal));
            tblNhapKho.Columns.Add("Pallet", typeof(string));
            tblNhapKho.Columns.Add("MaDVVT", typeof(string));
            tblNhapKho.Columns.Add("MauVTID", typeof(string));
            //Thoai
            tblNhapKho.Columns.Add("Batch", typeof(string));
        }
        private void CreateSearchLookUpCayVai()
        {

            if (_dt == null || _dt.Rows.Count == 0) return;
            if (!_dt.Columns.Contains("DisPlay"))
            {
                _dt.Columns.Add("DisPlay", typeof(string));
            }
            if (!_dt.Columns.Contains("MaNPL"))
            {
                _dt.Columns.Add("MaNPL", typeof(string));
            }
            foreach (DataRow dr in _dt.Rows)
            {
                dr["DisPlay"] = dr["MaVT"].ToString() + "|" + dr["ChiTiet"].ToString() + "|" + dr["MauVT"].ToString();
                dr["MaNPL"] = RemoveVietnameseTone(ReplaceSpecialCharacters(dr["MaVTID"].ToString() + dr["MaMauVT"].ToString() + dr["KhoVaiID"].ToString()));
            }
            searchLookUpEditCayVai.Properties.DisplayMember = "DisPlay";
            searchLookUpEditCayVai.Properties.ValueMember = "MaNPL";
            searchLookUpEditCayVai.Properties.DataSource = _dt;
            if (_r != null)
            {
                _manpl = RemoveVietnameseTone(ReplaceSpecialCharacters(_r["MaVTID"].ToString() + _r["MaMauVT"].ToString() + _r["KhoVaiID"].ToString()));
                searchLookUpEditCayVai.EditValue = _manpl;
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
            if (worksheetcount == 1)
            {
                textEdit1.EditValue = worksheetCollection[0].Name;
            }
        }
        private async void btImport_Click(object sender, EventArgs e)
        {
            try
            {
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
                if (searchLookUpEditCayVai.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn cây vải.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string worksheetName = textEdit1.EditValue.ToString();

                var source = new ExcelDataSource();
                source.FileName = textEditLink.Text;


                var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "A1:BZ500");



                var options = new ExcelSourceOptions(worksheetSettings);
                source.SourceOptions = options;
                source.Fill();
                tbl = source.ToDataTable();
                if (tbl == null || tbl.Rows.Count == 0) return;
                if (tbl.Columns.Count < 10)
                {
                    XtraMessageBox.Show("File import không đúng mẫu. Vui lòng kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DataRow[] rowsToDelete = tbl.AsEnumerable()
              .Where(row =>
                  (row.IsNull(0) || string.IsNullOrWhiteSpace(row[0].ToString())) &&
                  (row.IsNull(1) || string.IsNullOrWhiteSpace(row[1].ToString())) &&
                  (row.IsNull(2) || string.IsNullOrWhiteSpace(row[2].ToString())) &&
                  (row.IsNull(3) || string.IsNullOrWhiteSpace(row[3].ToString()))
              ).ToArray();

                foreach (DataRow row in rowsToDelete)
                {
                    tbl.Rows.Remove(row);
                }
                /*DataRow[] rowsToDelete2 = tbl.AsEnumerable()
            .Where(row =>
                (row.IsNull(5) || string.IsNullOrWhiteSpace(row[5].ToString())) &&
                (row.IsNull(7) || string.IsNullOrWhiteSpace(row[7].ToString()))
            ).ToArray();

                foreach (DataRow row in rowsToDelete2)
                {
                    tbl.Rows.Remove(row);
                }*/
                //foreach (DataRow dr in tbl.Rows)
                //{
                //    if (
                //         Regex.Replace(dr[0].ToString(), @"\s+", "").ToUpper() != Regex.Replace(_r["TenNhom"].ToString(), @"\s+", "").ToUpper() ||
                //         Regex.Replace(dr[1].ToString(), @"\s+", "").ToUpper() != Regex.Replace(_r["MaVT"].ToString(), @"\s+", "").ToUpper() ||
                //         Regex.Replace(dr[2].ToString(), @"\s+", "").ToUpper() != Regex.Replace(_r["ChiTiet"].ToString(), @"\s+", "").ToUpper() ||
                //         Regex.Replace(dr[3].ToString(), @"\s+", "").ToUpper() != Regex.Replace(_r["MauVT"].ToString(), @"\s+", "").ToUpper() ||
                //         Regex.Replace(dr[4].ToString(), @"\s+", "").ToUpper() != Regex.Replace(_r["KhoVai"].ToString(), @"\s+", "").ToUpper()
                //    )
                //    {
                //        XtraMessageBox.Show("Vật tư import không trùng khớp. Vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //        return;
                //    }
                //}
                #region Thoai thông báo các ràng buộc khi import excel
                List<int> errorSoLuong = new List<int>();

                // Dictionary để track: "Roll|Lot|Batch" -> List<Số dòng>
                Dictionary<string, List<int>> rollLotBatchTracking = new Dictionary<string, List<int>>();

                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    string col0 = tbl.Rows[i][0]?.ToString().Trim() ?? "";
                    string col1 = tbl.Rows[i][1]?.ToString().Trim() ?? "";
                    string col2 = tbl.Rows[i][2]?.ToString().Trim() ?? "";
                    string col3 = tbl.Rows[i][3]?.ToString().Trim() ?? "";
                    string col4 = tbl.Rows[i][4]?.ToString().Trim() ?? "";

                    string roll = tbl.Rows[i][5]?.ToString().Trim() ?? "";
                    string lot = tbl.Rows[i][7]?.ToString().Trim() ?? "";
                    string batch = tbl.Rows[i][6]?.ToString().Trim() ?? "";
                    string sltt = tbl.Rows[i][8]?.ToString().Trim() ?? "";

                    bool hasRollLotBatch =
                        !string.IsNullOrWhiteSpace(roll) ||
                        !string.IsNullOrWhiteSpace(lot) ||
                        !string.IsNullOrWhiteSpace(batch);

                    if (!hasRollLotBatch)
                        continue;

                    var slct = tbl.Rows[i][8];
                    var value = slct?.ToString().Trim();

                    if (string.IsNullOrWhiteSpace(value) ||
                        !double.TryParse(value, out double num) ||
                        num < 0)
                    {
                        errorSoLuong.Add(i + 1);
                    }
                    // Tạo key duy nhất từ Roll + Lot + Batch
                    if (sltt == "" && batch == "" && lot == "" && roll == "") continue;
                    string key = $"{col0}|{col1}|{col2}|{col3}|{col4}|{roll}|{lot}|{batch}";

                    if (!rollLotBatchTracking.ContainsKey(key))
                    {
                        rollLotBatchTracking[key] = new List<int>();
                    }
                    rollLotBatchTracking[key].Add(i + 1);

                }

                // Kiểm tra các nhóm trùng lặp
                string allErrors = "";

                if (errorSoLuong.Count > 0)
                    allErrors += $"Dòng {string.Join(", ", errorSoLuong)}: Số lượng không hợp lệ.\n";

                List<string> duplicateMessages = new List<string>();
                foreach (var kvp in rollLotBatchTracking)
                {
                    if (kvp.Value.Count > 1)
                    {
                        string[] parts = kvp.Key.Split('|');
                        string displayCol0 = string.IsNullOrEmpty(parts[0]) ? "(trống)" : parts[0];
                        string displayCol1 = string.IsNullOrEmpty(parts[1]) ? "(trống)" : parts[1];
                        string displayCol2 = string.IsNullOrEmpty(parts[2]) ? "(trống)" : parts[2];
                        string displayCol3 = string.IsNullOrEmpty(parts[3]) ? "(trống)" : parts[3];
                        string displayCol4 = string.IsNullOrEmpty(parts[4]) ? "(trống)" : parts[4];
                        string displayRoll = string.IsNullOrEmpty(parts[5]) ? "(trống)" : parts[5];
                        string displayLot = string.IsNullOrEmpty(parts[6]) ? "(trống)" : parts[6];
                        string displayBatch = string.IsNullOrEmpty(parts[7]) ? "(trống)" : parts[7];

                        duplicateMessages.Add(
                            $"Dòng {string.Join(", ", kvp.Value)}: " +
                            //$"Col0={displayCol0}, Col1={displayCol1}, Col2={displayCol2}, Col3={displayCol3}, Col4={displayCol4}, " +
                            $"Roll={displayRoll}, Lot={displayLot}, Batch={displayBatch} bị trùng lặp.\n" +
                            $"Vui lòng kiểm tra lại file excel.");
                    }
                }

                if (duplicateMessages.Count > 0)
                {
                    allErrors += "Roll và Lot/Batch bị trùng lặp:\n";
                    allErrors += string.Join("\n", duplicateMessages) + "\n";
                }

                if (!string.IsNullOrEmpty(allErrors))
                {
                    XtraMessageBox.Show(allErrors, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.DialogResult = DialogResult.OK;
                #endregion
            }
            catch (Exception ex)
            { }
        }

        private void searchLookUpEditCayVai_EditValueChanged(object sender, EventArgs e)
        {

            _manpl = searchLookUpEditCayVai.EditValue.ToString();

        }
        public DataTable getData()
        {
            return tbl;
        }
    }
}