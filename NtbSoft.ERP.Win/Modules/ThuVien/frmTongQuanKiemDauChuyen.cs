using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmTongQuanKiemDauChuyen : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        public frmTongQuanKiemDauChuyen()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            grvTongQuan.RowCellStyle += grvTongQuan_RowCellStyle;
            grvTongQuan.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
            grvTongQuan.IndicatorWidth = 50;
        }
        protected override void OnLoad(EventArgs e)
        {
            LoadData();
        }
        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            KHDongThungLib.CustomDrawRowIndicator(sender, e);
        }
        private void LoadData()
        {
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Get?action=GetOverView");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(json);
            int focus = grvTongQuan.FocusedRowHandle;
            grcTongQuan.DataSource = dt;
            grvTongQuan.FocusedRowHandle = focus;
        }

        private void repoChiTietQC_Click(object sender, EventArgs e)
        {
            var drFocus = grvTongQuan.GetFocusedDataRow();
            if (drFocus is null) return;
            var line = drFocus["LineX"].ToString();
            var lenh = drFocus["LenhSX"].ToString();
            frmKiemMauDauChuyen frm = new frmKiemMauDauChuyen();
            frmKiemMauDauChuyen.gLine_Link = line;
            frmKiemMauDauChuyen.gLenhSX_Link = lenh;
            frm.ShowDialog();
            LoadData();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmKiemMauDauChuyen frm = new frmKiemMauDauChuyen();
            frm.ShowDialog();
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string templatePath = Application.StartupPath + @"\Templates\BCTQKiemDauChuyen.xlsx";

                if (!System.IO.File.Exists(templatePath))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file Template tại:\n" + templatePath, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                    saveDialog.FileName = "BCTQKiemDauChuyen_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        grvTongQuan.ShowLoadingPanel();

                        ExportDataToTemplate(templatePath, saveDialog.FileName);


                        System.Diagnostics.Process.Start(saveDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Lỗi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                grvTongQuan.HideLoadingPanel();
            }
        }
        private void ExportDataToTemplate(string templatePath, string outputPath)
        {
            string forderImageTable = System.IO.Path.Combine(Application.StartupPath, "Resources");

            using (DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook())
            {
                workbook.LoadDocument(templatePath);
                DevExpress.Spreadsheet.Worksheet worksheet = workbook.Worksheets[0];

                int headerRowIndex = 4;
                int startRowIndex = 5;

                var alignCenter = DevExpress.Spreadsheet.SpreadsheetHorizontalAlignment.Center;
                var alignLeft = DevExpress.Spreadsheet.SpreadsheetHorizontalAlignment.Left;
                var alignVcenter = DevExpress.Spreadsheet.SpreadsheetVerticalAlignment.Center;

                worksheet.Cells[headerRowIndex, 0].Value = "STT";
                worksheet.Cells[headerRowIndex, 0].Alignment.Horizontal = alignCenter;
                worksheet.Cells[headerRowIndex, 0].Alignment.Vertical = alignVcenter;
                worksheet.Cells[headerRowIndex, 0].Font.Bold = true;
                worksheet.Cells[headerRowIndex, 0].FillColor = System.Drawing.Color.FromArgb(255, 212, 128);
              
                SetMergedHeaderValue(worksheet, headerRowIndex, 1, 4, "Khách hàng", alignCenter, alignVcenter);
                SetMergedHeaderValue(worksheet, headerRowIndex, 5, 9, "Style", alignCenter, alignVcenter);
                SetMergedHeaderValue(worksheet, headerRowIndex, 10, 12, "Lệnh SX", alignCenter, alignVcenter);
                SetMergedHeaderValue(worksheet, headerRowIndex, 13, 14, "PO", alignCenter, alignVcenter);
                SetMergedHeaderValue(worksheet, headerRowIndex, 15, 16, "Chuyền", alignCenter, alignVcenter);
                SetMergedHeaderValue(worksheet, headerRowIndex, 17, 19, "Người tạo", alignCenter, alignVcenter);
                SetMergedHeaderValue(worksheet, headerRowIndex, 20, 21, "Ngày tạo", alignCenter, alignVcenter);
                SetMergedHeaderValue(worksheet, headerRowIndex, 22, 25, "Trạng thái", alignCenter, alignVcenter);
                SetMergedHeaderValue(worksheet, headerRowIndex, 26, 30, "Người khắc phục", alignCenter, alignVcenter);
                SetMergedHeaderValue(worksheet, headerRowIndex, 31, 34, "Ngày khắc phục", alignCenter, alignVcenter);

                for (int i = 0; i < grvTongQuan.RowCount; i++)
                {
                    int rowHandle = grvTongQuan.GetVisibleRowHandle(i);
                    if (grvTongQuan.IsGroupRow(rowHandle)) continue;

                    int currentRow = startRowIndex + i;

                    worksheet.Cells[currentRow, 0].Value = i + 1;
                    worksheet.Cells[currentRow, 0].Alignment.Horizontal = alignCenter;
                    worksheet.Cells[currentRow, 0].Alignment.Vertical = alignVcenter;

                    SetMergedCellValue(worksheet, currentRow, 15, 16, grvTongQuan.GetRowCellValue(rowHandle, "Name")?.ToString(), alignCenter, alignVcenter);
                    SetMergedCellValue(worksheet, currentRow, 10, 12, grvTongQuan.GetRowCellValue(rowHandle, "LenhSX")?.ToString(), alignCenter, alignVcenter);
                    SetMergedCellValue(worksheet, currentRow, 1, 4, grvTongQuan.GetRowCellValue(rowHandle, "KhachHang")?.ToString(), alignLeft, alignVcenter);
                    SetMergedCellValue(worksheet, currentRow, 5, 9, grvTongQuan.GetRowCellValue(rowHandle, "MaHang")?.ToString(), alignLeft, alignVcenter);
                    SetMergedCellValue(worksheet, currentRow, 13, 14, grvTongQuan.GetRowCellValue(rowHandle, "PO")?.ToString(), alignCenter, alignVcenter);
                    SetMergedCellValue(worksheet, currentRow, 17, 19, grvTongQuan.GetRowCellValue(rowHandle, "CreateUser")?.ToString(), alignLeft, alignVcenter);
                    SetMergedCellValue(worksheet, currentRow, 20, 21, grvTongQuan.GetRowCellValue(rowHandle, "CreateDate")?.ToString(), alignCenter, alignVcenter);
                    string statusKT = grvTongQuan.GetRowCellValue(rowHandle, "StatusKT")?.ToString();
                    string statusKT_Check = grvTongQuan.GetRowCellValue(rowHandle, "StatusKT_Check")?.ToString();
                    SetMergedCellValue(worksheet, currentRow, 22, 25, statusKT, alignCenter, alignVcenter);

                    if (statusKT_Check.Trim() == "2")
                    {
                        worksheet.Cells[currentRow, 22].Font.Color = System.Drawing.Color.ForestGreen;
                        worksheet.Cells[currentRow, 22].Font.Bold = true;
                    }
                    else if (statusKT_Check.Trim() == "1")
                    {
                        worksheet.Cells[currentRow, 22].Font.Color = System.Drawing.Color.Orange;
                        worksheet.Cells[currentRow, 22].Font.Bold = true;
                    }
                    SetMergedCellValue(worksheet, currentRow, 26, 30, grvTongQuan.GetRowCellValue(rowHandle, "QCInLine")?.ToString(), alignLeft, alignVcenter);
                    SetMergedCellValue(worksheet, currentRow, 31, 34, grvTongQuan.GetRowCellValue(rowHandle, "NgayTH")?.ToString(), alignCenter, alignVcenter);
                }

                int totalColumns = 34;
                var dataRange = worksheet.Range.FromLTRB(0, headerRowIndex, totalColumns, startRowIndex + grvTongQuan.RowCount - 1);
                dataRange.Borders.SetAllBorders(System.Drawing.Color.Black, DevExpress.Spreadsheet.BorderLineStyle.Thin);

                workbook.SaveDocument(outputPath);
            }

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            FileInfo fileSauKhiGhiText = new FileInfo(outputPath);

            using (var package = new OfficeOpenXml.ExcelPackage(fileSauKhiGhiText))
            {
                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws != null)
                {
                    Addpicutre(ws, "DongNai1.jpg", 0, 2, 25, 8, 60, 50, forderImageTable);

                    package.Save();
                }
            }
        }

        private void SetMergedHeaderValue(DevExpress.Spreadsheet.Worksheet sheet, int row, int colStart, int colEnd, string value,
                                                DevExpress.Spreadsheet.SpreadsheetHorizontalAlignment hAlign,
                                                DevExpress.Spreadsheet.SpreadsheetVerticalAlignment vAlign)
        {
            sheet.MergeCells(sheet.Range.FromLTRB(colStart, row, colEnd, row));
            DevExpress.Spreadsheet.Cell cell = sheet.Cells[row, colStart];
            cell.Value = value;
            cell.Alignment.Horizontal = hAlign;
            cell.Alignment.Vertical = vAlign;
            cell.Alignment.WrapText = true;
            cell.Font.Bold = true;
            cell.FillColor = System.Drawing.Color.FromArgb(255, 212, 128); 
        }

        private void SetMergedCellValue(DevExpress.Spreadsheet.Worksheet sheet, int row, int colStart, int colEnd, string value,
                                        DevExpress.Spreadsheet.SpreadsheetHorizontalAlignment hAlign,
                                        DevExpress.Spreadsheet.SpreadsheetVerticalAlignment vAlign)
        {
            sheet.MergeCells(sheet.Range.FromLTRB(colStart, row, colEnd, row));

            DevExpress.Spreadsheet.Cell cell = sheet.Cells[row, colStart];
            cell.Value = value;

            cell.Alignment.Horizontal = hAlign;
            cell.Alignment.Vertical = vAlign;
            cell.Alignment.WrapText = true; 
        }
        private void Addpicutre(ExcelWorksheet worksheet, string sign, int row, int col, int toadorow, int toado, int width, int height, string forderImageTable)
        {
            var pathFolder = forderImageTable;
            string imagePath = Path.Combine(pathFolder, sign);
            // Chèn ảnh vào worksheet
            if (File.Exists(imagePath))
            {
                FileInfo imageFile = new FileInfo(imagePath);
                ExcelPicture picture = worksheet.Drawings.AddPicture(Guid.NewGuid().ToString(), imageFile);
                picture.SetPosition(row, toadorow, col, toado);
                picture.SetSize(width, height);
            }
        }
        private void grvTongQuan_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "StatusKT")
            {
                string status = grvTongQuan.GetRowCellValue(e.RowHandle, "StatusKT_Check")?.ToString() ?? "";

                if (status.Trim() == "2")
                {
                    e.Appearance.ForeColor = Color.ForestGreen;
                    e.Appearance.FontStyleDelta = FontStyle.Bold;
                }
                else if (status.Trim() == "1")
                {
                    e.Appearance.ForeColor = Color.Orange;
                    e.Appearance.FontStyleDelta = FontStyle.Bold;
                }
            }
        }
    }
}
