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
using OfficeOpenXml;
using System.IO;
using NtbSoft.ERP.Entity.ThuVien;
using DevExpress.XtraSplashScreen;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmDicQCDongThung_Excel : DevExpress.XtraEditors.XtraForm
    {
        private string _filePath = "";
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        public List<DicQCDongThungEntity> ImportedData { get; private set; } = new List<DicQCDongThungEntity>();
        public List<string> ErrorLines { get; private set; } = new List<string>();
        public frmDicQCDongThung_Excel()
        {
            InitializeComponent();
            _clientExtension = new HttpClientExtension();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        }

        private void btGetLink_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel Files|*.xlsx;*.xls";
                ofd.Title = "Chọn file quy cách đóng thùng";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _filePath = ofd.FileName;
                    textEditLink.Text = _filePath;
                    LoadSheets();
                }
            }
        }

        private void LoadSheets()
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo(_filePath)))
                {
                    var sheets = package.Workbook.Worksheets.Select(s => s.Name).ToList();
                    var dt = new DataTable();
                    dt.Columns.Add("Name");
                    foreach (var s in sheets) dt.Rows.Add(s);

                    textEdit1.Properties.DataSource = dt;
                    textEdit1.Properties.DisplayMember = textEdit1.Properties.ValueMember = "Name";

                    if (sheets.Count == 1)
                        textEdit1.EditValue = sheets[0];
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi đọc file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textEdit1.Properties.DataSource = null;
            }
        }

        private int FindDataStartRow(ExcelWorksheet ws)
        {
            int headerRow = -1;
            for (int r = 1; r <= Math.Min(10, ws.Dimension.End.Row); r++)
            {
                string cell5 = ws.Cells[r, 5].Text.ToLower().Trim();
                string cell2 = ws.Cells[r, 2].Text.ToLower().Trim();
                string cell3 = ws.Cells[r, 3].Text.ToLower().Trim();

                if ((cell5.Contains("kích") && cell5.Contains("thùng")) ||
                    (cell2.Contains("số") && cell2.Contains("lớp")) ||
                    (cell3.Contains("loại") && cell3.Contains("thùng")))
                {
                    headerRow = r;
                    //System.Diagnostics.Debug.WriteLine($"Tìm thấy header ở dòng: {r}");
                    break;
                }
            }

            int searchStart = headerRow > 0 ? headerRow + 1 : 1;
            for (int r = searchStart; r <= Math.Min(searchStart + 10, ws.Dimension.End.Row); r++)
            {
                string kichThung = ws.Cells[r, 5].Text.Trim();

                if (string.IsNullOrWhiteSpace(kichThung))
                    continue;

                if (kichThung.Contains("x") || kichThung.Contains("X") ||
                    kichThung.Contains("*") || kichThung.Contains("×"))
                {
                    //System.Diagnostics.Debug.WriteLine($"Tìm thấy dữ liệu ở dòng: {r}");
                    return r;
                }
            }
            return headerRow > 0 ? headerRow + 1 : 1;
        }
        private void btImport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                XtraMessageBox.Show("Vui lòng chọn file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (textEdit1.EditValue == null)
            {
                XtraMessageBox.Show("Vui lòng chọn sheet!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sheetName = textEdit1.EditValue.ToString();
            ImportedData.Clear();

            if (SplashScreenManager.Default == null || !SplashScreenManager.FormInPendingState)
            {
                SplashScreenManager.ShowForm(this, typeof(ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang import dữ liệu...");
                SplashScreenManager.Default.SetWaitFormDescription("Vui lòng chờ...");
            }

            try
            {
                string url = string.Format("{0}", URL + $"DicQCDongThung/Get?action=Get");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                List<DicQCDongThungEntity> dbData = JsonConvert.DeserializeObject<List<DicQCDongThungEntity>>(json) ?? new List<DicQCDongThungEntity>();

                using (var package = new ExcelPackage(new FileInfo(_filePath)))
                {
                    var ws = package.Workbook.Worksheets[sheetName];
                    int startRow = FindDataStartRow(ws);
                    var errorLines = new List<string>();
                    var tempDict = new Dictionary<string, DicQCDongThungEntity>();
                    int emptyRow = 0;
                    int maxEmptyRow = 10;

                    for (int r = startRow; r <= ws.Dimension.End.Row; r++)
                    {
                        string soLopText = ws.Cells[r, 2].Text.Trim();
                        string loaiThung = ws.Cells[r, 3].Text.Trim();
                        string kichThung = ws.Cells[r, 5].Text.Trim();
                        string ghiChu = ws.Cells[r, 9].Text.Trim();
                        //string ghiChu2 = ws.Cells[r, 10].Text.Trim();

                        if (string.IsNullOrWhiteSpace(kichThung))
                        {
                            emptyRow++;
                            if (emptyRow > maxEmptyRow)
                            {
                                break;
                            }
                            continue;
                        }
                        emptyRow = 0;

                        string maDV = "1";
                        if (kichThung.Contains("\"") || kichThung.Contains("\"")
                           || kichThung.Contains("\"") || kichThung.Contains("'")
                           || kichThung.Contains("'") || kichThung.Contains("′")
                           || kichThung.Contains("″") || kichThung.Contains("”"))
                            maDV = "2";

                        kichThung = kichThung.Replace("\"", "")
                                     .Replace("\"", "")
                                     .Replace("\"", "")
                                     .Replace("'", "")
                                     .Replace("'", "")
                                     .Replace("′", "")
                                     .Replace("″", "")
                                     .Replace("”", "")
                                     .Trim();

                        var parts = kichThung.Split(new char[] { '*', 'x', 'X', '×' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 3)
                        {
                            errorLines.Add($"Dòng {r}: Kích thùng không hợp lệ (phải có 3 giá trị)");
                            continue;
                        }

                        if (!double.TryParse(parts[0].Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double dai) ||
                            !double.TryParse(parts[1].Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double rong) ||
                            !double.TryParse(parts[2].Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double cao))
                        {
                            errorLines.Add($"Dòng {r}: Kích thùng chứa giá trị không phải số");
                            continue;
                        }

                        if (dai == 0 || rong == 0 || cao == 0)
                        {
                            errorLines.Add($"Dòng {r}: Kích thùng {kichThung} - Chiều dài, rộng, cao phải hợp lệ");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(soLopText))
                        {
                            errorLines.Add($"Dòng {r}: Số lớp để trống!");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(loaiThung))
                        {
                            errorLines.Add($"Dòng {r}: Loại thùng để trống!");
                            continue;
                        }

                        int soLop = 0;
                        int.TryParse(soLopText, out soLop);

                        string tenQuiCach = $"{dai}x{rong}x{cao}";
                        string loaiThungNormalized = loaiThung.Trim().ToLower();
                        //string ghiChuNormalized = string.IsNullOrWhiteSpace(ghiChu) ? "" : ghiChu.Trim().ToLower();
                        string key = $"{dai}|{rong}|{cao}|{soLop}|{loaiThungNormalized}";

                        if (tempDict.ContainsKey(key))
                        {
                            var existing = tempDict[key];
                            var ghiChuList = new List<string>();

                            if (!string.IsNullOrWhiteSpace(existing.GhiChu))
                                ghiChuList.AddRange(existing.GhiChu.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)));

                            if (!string.IsNullOrWhiteSpace(ghiChu))
                            {
                                string ghiChuTrim = ghiChu.Trim();
                                bool exists = ghiChuList.Any(x => x.Equals(ghiChuTrim, StringComparison.OrdinalIgnoreCase));

                                if (!exists)
                                    ghiChuList.Add(ghiChuTrim);
                            }
                            var distinctGhiChu = new List<string>();
                            foreach(var item in ghiChuList)
                            {
                                if (!distinctGhiChu.Any(x => x.Equals(item, StringComparison.OrdinalIgnoreCase)))
                                    distinctGhiChu.Add(item);
                            }
                            existing.GhiChu = string.Join(",", distinctGhiChu);
                        }
                        else
                        {
                            var newItem = new DicQCDongThungEntity
                            {
                                TenQuiCach = tenQuiCach,
                                ChieuDai = dai,
                                ChieuRong = rong,
                                ChieuCao = cao,
                                SoLop = soLop,
                                LoaiThung = loaiThung,
                                GhiChu = string.IsNullOrWhiteSpace(ghiChu) ? null : ghiChu.Trim(),
                                //GhiChu2 = string.IsNullOrWhiteSpace(ghiChu2) ? null : ghiChu2.Trim(),
                                MaDV = maDV,
                                CanNang = 0,
                                NguoiTao = GlobleData.UserName
                            };
                            tempDict.Add(key, newItem);
                        }
                    }
                    var duplicatedItems = new List<string>();

                    foreach (var kvp in tempDict)
                    {
                        var item = kvp.Value;
                        string key = kvp.Key;

                        var existingInDB = dbData.FirstOrDefault(x =>
                        {
                            string dbLoaiThung = (x.LoaiThung ?? "").Trim().ToLower();
                            string dbKey = $"{x.ChieuDai}|{x.ChieuRong}|{x.ChieuCao}|{x.SoLop}|{dbLoaiThung}";
                            return dbKey == key;
                        });

                        if (existingInDB != null)
                        {
                            duplicatedItems.Add($"{item.TenQuiCach} - Số lớp: {item.SoLop} - Loại thùng: {item.LoaiThung}");
                        }
                        else
                        {
                            ImportedData.Add(item);
                        }
                    }

                    if (ImportedData.Count == 0 && duplicatedItems.Count == 0) return;

                    string resultJson = "";

                    if (ImportedData.Count > 0)
                    {
                        string insertUrl = string.Format("{0}", URL + "DicQCDongThung/Insert");
                        resultJson = Task.Run(async () =>
                        {
                            return await _clientExtension.PostAsync(insertUrl, ImportedData);
                        }).Result;
                    }
                   
                    if (ImportedData.Count > 0)
                    {
                        string updateUrl = string.Format("{0}", URL + "DicQCDongThung/Update");
                        string updateResult = Task.Run(async () =>
                        {
                            return await _clientExtension.PostAsync(updateUrl, ImportedData);
                        }).Result;
                    }
                    if (duplicatedItems.Count > 0)
                    {
                        if (duplicatedItems.Count > 0)
                        {
                            errorLines.Insert(0, $"{duplicatedItems.Count} kích thùng đã tồn tại (bỏ qua)");
                            errorLines.AddRange(duplicatedItems);
                        }
                    }
                    this.ErrorLines = errorLines;
                    SplashScreenManager.CloseForm(false);

                    bool isSuccess = string.IsNullOrEmpty(resultJson) &&
                           resultJson.ToLower().Contains("true") ||
                            resultJson.ToLower().Contains("success");

                    if (isSuccess || ImportedData.Count > 0)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);
                return;
            }
        }    
    }
}
