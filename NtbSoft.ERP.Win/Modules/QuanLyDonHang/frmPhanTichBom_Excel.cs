using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.Kho;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmPhanTichBom_Excel : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tblGC, tblAllSize;
        bool isCT = false;
        string makh = string.Empty, mahang = string.Empty, dot = string.Empty;
        private HashSet<DataRow> selectedRows = new HashSet<DataRow>();
        private bool isLoadingDefault = false;
        private bool _suspendSelectionChanged = false;
        public frmPhanTichBom_Excel(DataTable tbl, DataTable tblSize, string _kh="", string _mh="", string _dot="")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            tblGC = tbl;
            tblAllSize = tblSize;
            makh = _kh;
            mahang = _mh;
            dot = _dot;
            loadDot();
            LoadCol();
            LoadSize();
        }
        private void loadDot()
        {

            string url = $"{URL}KhoiTaoDM/GetDot?makh={makh}&mahang={mahang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0) return;
            searchDot.Properties.ValueMember = "MaDot";
            searchDot.Properties.DisplayMember = "Dot";
            searchDot.Properties.DataSource = tbl;

            //searchDot.Popup += searchDot_Popup;
            //this.ActiveControl = searchDot;
            //searchDot.Focus();
            //SendKeys.Send("{F4}");
            //SendKeys.Send("{F4}");
        }

        private void searchDot_Popup(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dot)) return;

            GridView view = searchDot.Properties.View as GridView;
            if (view == null) return;

            string[] dotList = dot.Split(';');

            // Delay để chắc chắn data đã load xong
            BeginInvoke((Action)(() =>
            {
                view.BeginSelection();
                view.ClearSelection();

                for (int i = 0; i < view.RowCount; i++)
                {
                    object val = view.GetRowCellValue(i, searchDot.Properties.ValueMember);
                    if (val != null && dotList.Contains(val.ToString()))
                    {
                        view.SelectRow(i);
                    }
                }

                view.EndSelection();

                // Set text hiển thị
                string displayText = string.Join(";", dotList.Select(d =>
                {
                    DataRow[] rows = ((DataTable)searchDot.Properties.DataSource).Select($"MaDot = '{d}'");
                    return rows.Length > 0 ? rows[0]["Dot"].ToString() : "";
                }));
                searchDot.Text = displayText;
            }));

            // Chỉ thực hiện 1 lần
            searchDot.Popup -= searchDot_Popup;
        }

        private void checkCT_CheckedChanged(object sender, EventArgs e)
        {
            var barItem = sender as DevExpress.XtraBars.BarCheckItem;
            if (barItem != null)
            {
                if (barItem.Checked)
                {
                    isCT = true;
                }
                else
                {
                    isCT = false;
                }
            }
        }
        private DataTable loadTongHop(string madot)
        {
            string url = $"{URL}KhoiTaoDM/GetDM?makh={makh}&mahang={mahang}&madot={madot}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                return null;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0) return null;
            return tbl;
        }

        private void searchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            if (view.SelectedRowsCount == view.RowCount)
            {
                selectedRows.Clear();
                for (int i = 0; i < view.RowCount; i++)
                {
                    DataRow row = view.GetDataRow(i);
                    if (row != null)
                        selectedRows.Add(row);
                }
            }
            else if (view.SelectedRowsCount == 0)
            {
                selectedRows.Clear();
            }
            else
            {
                int rowHandle = e.ControllerRow;
                if (rowHandle >= 0)
                {
                    DataRow row = view.GetDataRow(rowHandle);

                    if (view.IsRowSelected(rowHandle))
                        selectedRows.Add(row);
                    else
                        selectedRows.Remove(row);
                }
            }
            string selectedValues = string.Join(";", gVDot.GetSelectedRows().Select(rowHandle2 => gVDot.GetRowCellValue(rowHandle2, searchDot.Properties.ValueMember)));
            searchDot.EditValue = selectedValues;
            if (searchDot.EditValue is null) return;
        }

        private void searchLookUpEdit1View_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRows.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }

        private void searchDot_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedRows = gVDot.GetSelectedRows();
            var selectedValues = selectedRows
                .Select(rowHandle => gVDot.GetRowCellValue(rowHandle, searchDot.Properties.DisplayMember)?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value)) // Loại bỏ giá trị null, rỗng hoặc chỉ chứa ký tự trắng
                .ToList();

            e.DisplayText = selectedValues.Any() ? string.Join("; ", selectedValues) : "";
           
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (tblGC == null || tblGC.Rows.Count == 0) return;
                if (tblAllSize == null || tblAllSize.Rows.Count == 0) return;
                if (selectedRows.Count == 0) return;

                SaveFileDialog Sfd = new SaveFileDialog();
                Sfd.Title = "File To Save";
                Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
                Sfd.FileName = string.Format("KhoiTaoBom{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
                if (Sfd.ShowDialog() == DialogResult.OK)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    string fileName = "KhoiTaoBomTemplate.xlsx";
                    string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                    string TemplateFileName = path;
                    string ExportFileName = Sfd.FileName;
                    Export(TemplateFileName, ExportFileName, tblGC, tblAllSize);

                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Mở File", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            if (File.Exists(Sfd.FileName))
                                System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                        }
                        catch
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Error..");
                        }
                    }
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

                }

            }
            catch (Exception ex)
            {
            }
        }
        public void Export(string TemplateFileName, string ExportFileName, DataTable tbl1, DataTable tblSize)
        {

            try
            {
                List<string> selectedColumns = GetSelectedColumns();
                List<string> selectedSizes = GetSelectedSizes();

                if (selectedColumns.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một cột để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string selectedDot = string.Join(";", selectedRows.Select(r => r["MaDot"].ToString()));

                string[] dotList = selectedDot.Split(';');

                if (selectedDot == null) return;


                FileInfo file = new FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    excelPackage.Workbook.Properties.Author = "Cty NTB";
                    excelPackage.Workbook.Properties.Title = "";

                    string[] dateTime = DateTime.Now.ToString("dd/MM/yyyy").Split('/');

                    FileInfo templateFile = new FileInfo(TemplateFileName);
                    int sheetIndex = 0;

                    using (ExcelPackage templatePackage = new ExcelPackage(templateFile))
                    {
                        sheetIndex++;
                        foreach (string madotthonghop in dotList)
                        {
                            int index = 1;
                            int rowTH = 1;
                            // ======================== SHEET TỔNG HỢP ========================
                            string url = string.Format("{0}?para={1}&&para1={2}&&para2={3}", URL + "KhoiTaoDM/GETVATTUEXCEL", makh ?? "", mahang ?? "", madotthonghop);
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                            DataTable tblGC2 = JsonConvert.DeserializeObject<DataTable>(json);

                            if (tblGC2 == null || tblGC2.Rows.Count == 0) continue;

                            DataTable tbl = tblGC2;
                            string sheetNameTongHop = $"{mahang}-TongHop-DOT{madotthonghop.Split('_').Last()}";
                            ExcelWorksheet templateSheetTH = templatePackage.Workbook.Worksheets["Sheet1"];
                            ExcelWorksheet newSheetTH = excelPackage.Workbook.Worksheets.Add(sheetNameTongHop, templateSheetTH);

                            //// Header cố định
                            //string[] headers = { "STT", "Mã Vật Tư","ItemCode", "Vật Tư", "Đơn Vị", "Khổ/Size", "Mã Màu Vật Tư", "Màu Vật Tư", "Màu Sản Phẩm", "Định Mức Tạm Tính", "% Hao Hụt" };


                            // Tạo mapping giữa tên cột hiển thị và tên cột trong database
                            Dictionary<string, string> columnMapping = new Dictionary<string, string>
                            {
                                {"STT", "STT"},
                                {"Mã Vật Tư", "MaVTGhep"},
                                {"ItemCode", "MaVT"},
                                {"Vật Tư", "ChiTiet"},
                                {"Đơn Vị", "TenDVVT"},
                                {"Khổ/Size", "KhoVai"},
                                {"Mã Màu Vật Tư", "MaMauVT"},
                                {"Màu Vật Tư", "MauVT"},
                                {"Màu Sản Phẩm", "TenMau"},
                                {"Định Mức Tạm Tính", "DinhMucChung"},
                                {"% Hao Hụt", "DinhMucHaoHut"}
                            };

                            // Tạo header chỉ với các cột được chọn (luôn có STT ở đầu)
                            List<string> headers = new List<string> { "STT" };
                            var filteredColumns = selectedColumns.Where(col => col != "STT").ToList();
                            headers.AddRange(filteredColumns);
                            //for (int i = 0; i < headers.Length; i++)
                            //    newSheetTH.Cells[rowTH, i + 1].Value = headers[i];
                            for (int i = 0; i < headers.Count; i++)
                                newSheetTH.Cells[rowTH, i + 1].Value = headers[i];

                            using (var range = newSheetTH.Cells[rowTH, 1, rowTH, headers.Count])
                            {
                                range.Style.Font.Bold = true;
                                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(Color.Orange);
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                range.Style.WrapText = true;
                            }

                            //for (int i = 0; i < headers.Length; i++)
                            for (int i = 0; i < headers.Count; i++)
                            {
                                int headerLength = headers[i].Length;
                                newSheetTH.Column(i + 1).Width = headerLength + 4;
                            }
                            rowTH++;

                            var groupByNPL = tbl.AsEnumerable()
                                .GroupBy(r => r.Field<bool>("NPL") ? "Nguyên Liệu" : "Phụ Liệu");

                            foreach (var nplGroup in groupByNPL)
                            {
                                var nplCell = newSheetTH.Cells[rowTH, 1, rowTH, headers.Count];
                                nplCell.Merge = true;
                                nplCell.Value = nplGroup.Key;
                                nplCell.Style.Font.Bold = true;
                                nplCell.Style.Font.Color.SetColor(Color.Blue);
                                nplCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                nplCell.Style.Fill.BackgroundColor.SetColor(Color.White);
                                nplCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                nplCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                nplCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                nplCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                rowTH++;

                                var groupByNhom = nplGroup.GroupBy(r => r["TenNhom"].ToString());
                                foreach (var nhomGroup in groupByNhom)
                                {
                                    var nhomCell = newSheetTH.Cells[rowTH, 1, rowTH, headers.Count];
                                    nhomCell.Merge = true;
                                    nhomCell.Value = nhomGroup.Key;
                                    nhomCell.Style.Font.Bold = true;
                                    nhomCell.Style.Font.Color.SetColor(Color.Red);
                                    nhomCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    nhomCell.Style.Fill.BackgroundColor.SetColor(Color.White);
                                    nhomCell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    nhomCell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    nhomCell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    nhomCell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    rowTH++;

                                    foreach (var items in nhomGroup)
                                    {
                                        //newSheetTH.Cells[rowTH, 1].Value = index++;
                                        //newSheetTH.Cells[rowTH, 2].Value = items["MaVTGhep"];
                                        //newSheetTH.Cells[rowTH, 3].Value = items["MaVT"];
                                        //newSheetTH.Cells[rowTH, 4].Value = items["ChiTiet"];
                                        //newSheetTH.Cells[rowTH, 5].Value = items["TenDVVT"];
                                        //newSheetTH.Cells[rowTH, 6].Value = items["KhoVai"];
                                        //newSheetTH.Cells[rowTH, 7].Value = items["MaMauVT"];
                                        //newSheetTH.Cells[rowTH, 8].Value = items["MauVT"];
                                        //newSheetTH.Cells[rowTH, 9].Value = items["TenMau"];
                                        //newSheetTH.Cells[rowTH, 10].Value = items["DinhMucChung"];
                                        //newSheetTH.Cells[rowTH, 11].Value = items["DinhMucHaoHut"];
                                        for (int i = 0; i < headers.Count; i++)
                                        {
                                            string headerName = headers[i];
                                            object cellValue = null;

                                            if (headerName == "STT")
                                            {
                                                cellValue = index++;
                                                newSheetTH.Cells[rowTH, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            }
                                            else if (columnMapping.ContainsKey(headerName))
                                            {
                                                string dbColumnName = columnMapping[headerName];
                                                cellValue = items[dbColumnName];
                                            }

                                            newSheetTH.Cells[rowTH, i + 1].Value = cellValue;
                                        }

                                        newSheetTH.Cells[rowTH, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        var range = newSheetTH.Cells[rowTH, 1, rowTH, headers.Count];
                                        range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        rowTH++;
                                    }
                                }
                            }

                            // ======================== SHEET CHI TIẾT ========================
                            if (checkCT.Checked)
                            {
                                var ctRows = tblSize.AsEnumerable().Where(r => r["MaDot"].ToString() == madotthonghop).ToList();

                                ctRows = ctRows.OrderBy(r =>
                                {
                                    var maNhom = r["MaNhom"].ToString();
                                    var matchingTblRow = tbl.AsEnumerable()
                                        .FirstOrDefault(tr => tr["MaNhom"].ToString() == maNhom);
                                    return matchingTblRow != null ? Convert.ToInt32(matchingTblRow["Sort"]) : int.MaxValue;
                                }).ToList();
                                //if (selectedSizes.Count > 0)
                                //{
                                //    ctRows = ctRows.Where(r =>
                                //    {
                                //        string rowSize = r["Size"].ToString();
                                //        return selectedSizes.Contains(rowSize);
                                //    }).ToList();
                                //}
                                if (ctRows.Count == 0) continue;
                                var dotGroup = ctRows.GroupBy(r => r["MaDot"].ToString()).First();
                                string sheetNameChiTiet = $"{mahang}-ChiTiet-DOT{madotthonghop.Split('_').Last()}";
                                ExcelWorksheet templateSheet = templatePackage.Workbook.Worksheets["Sheet1"];
                                ExcelWorksheet newSheet = excelPackage.Workbook.Worksheets.Add(sheetNameChiTiet, templateSheet);
                                int row = 1;

                                var groupByVatTu = dotGroup.GroupBy(r => new
                                {
                                    MaVTID = r["MaVTID"].ToString(),
                                    MauVTID = r["MauVTID"].ToString(),
                                    KhoVaiID = r["KhoVaiID"].ToString(),
                                    MaMau = r["MaMau"].ToString(),
                                    TenMau = r["TenMau"].ToString(),
                                    MaNhom = r["MaNhom"].ToString()
                                });

                                //foreach (var vtGroup in groupByVatTu)
                                foreach (var vtGroup in groupByVatTu)
                                {
                                    var dtGroup = vtGroup.CopyToDataTable();
                                    var listMauSize = SplitTenMau(vtGroup.Key.TenMau.ToString());

                                    foreach (var mauSize in listMauSize) 
                                    {
                                        var matchedRow = tbl.AsEnumerable().FirstOrDefault(r =>
                                        {
                                            if (r["MaVTID"].ToString() != vtGroup.Key.MaVTID) return false;
                                            if (r["MauVTID"].ToString() != vtGroup.Key.MauVTID) return false;
                                            if (r["KhoVaiID"].ToString() != vtGroup.Key.KhoVaiID) return false;
                                            if (r["MaNhom"].ToString() != vtGroup.Key.MaNhom) return false;

                                            var listMauTbl = SplitTenMau(r["TenMau"].ToString());
                                            return listMauTbl.Any(mauTbl => string.Equals(mauTbl, mauSize, StringComparison.OrdinalIgnoreCase));
                                        });

                                        if (matchedRow == null) continue;

                                        string tennhomhd = matchedRow["TenNhom"].ToString();
                                        string mavthd = matchedRow["MaVT"].ToString();
                                        string khovaihd = matchedRow["KhoVai"].ToString();
                                        string tenmau = matchedRow["TenMau"].ToString();

                                        var dotCell = newSheet.Cells[row, 1];
                                        dotCell.Value = $"{tennhomhd}-{mavthd}-{mauSize}-{khovaihd}";
                                        dotCell.Style.Font.Color.SetColor(Color.White);
                                        dotCell.Style.Font.Bold = true;
                                        dotCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        dotCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 0, 0));
                                        dotCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        dotCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                        var colsWithAt = dtGroup.Columns.Cast<DataColumn>().Where(c => c.ColumnName.Contains("@")).ToList();
                                        if (selectedSizes.Count > 0)
                                        {
                                            colsWithAt = colsWithAt.Where(c =>
                                            {
                                                string colSize = c.ColumnName.Split('@').First();
                                                //return selectedSizes.Contains(colSize);
                                                return selectedSizes.Any(s=>s.Split('|')[0] == colSize);
                                            }).ToList();
                                        }

                                        int colI = 2;

                                        foreach (DataColumn col in colsWithAt)
                                        {
                                            var cell = newSheet.Cells[row, colI++];
                                            cell.Value = col.ColumnName.Split('@').First();
                                            cell.Style.Font.Color.SetColor(Color.White);
                                            cell.Style.Font.Bold = true;
                                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(46, 117, 182));
                                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                        }

                                        int duyetCol = colsWithAt.Count + 2;
                                        var duyetHeader = newSheet.Cells[row, duyetCol];
                                        duyetHeader.Value = "Xét Duyệt";
                                        duyetHeader.Style.Font.Bold = true;
                                        duyetHeader.Style.Font.Color.SetColor(Color.White);
                                        duyetHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        duyetHeader.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(46, 117, 182));
                                        duyetHeader.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                        duyetHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        row++;

                                        var filteredRows = dtGroup.AsEnumerable()
                                                                    .Where(dr =>
                                                                    {
                                                                        var colors = SplitTenMau(dr["TenMau"].ToString());
                                                                        return colors.Any(m => string.Equals(m, mauSize, StringComparison.OrdinalIgnoreCase));
                                                                    }).ToList();

                                        if (filteredRows.Count == 0) continue;

                                        int startMergeRow = row;
                                        string currentDuyetValue = null;
                                        //foreach (DataRow dr in dtGroup.Rows)
                                        foreach (var dr in filteredRows)
                                        {
                                            colI = 1;
                                            //newSheet.Cells[row, colI++].Value = dr["DauSize"].ToString();
                                            var dauSizeCell = newSheet.Cells[row, colI++];
                                            dauSizeCell.Value = dr["NhomSize"].ToString();
                                            dauSizeCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            dauSizeCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                            dauSizeCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                                            foreach (var col in colsWithAt)
                                            {
                                                var cell = newSheet.Cells[row, colI++];
                                                string colSizeSelected = col.ColumnName.Split('@').First();
                                                string mnSize = dr["MaNhomSize"].ToString();
                                                bool isSelected = selectedSizes.Any(s =>
                                                {
                                                    var parts = s.Split('|');
                                                    return parts[0] == colSizeSelected && parts[1] == mnSize;
                                                });
                                                if (isSelected && dr[col] != DBNull.Value && Convert.ToDecimal(dr[col]) > 0)
                                                {
                                                    cell.Value = dr[col];
                                                }
                                                else 
                                                {
                                                    cell.Value = '-';
                                                }
                                                
                                                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                                cell.Style.Font.Color.SetColor(Color.FromArgb(0, 112, 192));
                                            }

                                            var duyetCell = newSheet.Cells[row, duyetCol];
                                            bool isDuyet = dr["IsXetDuyet"].ToString().ToLower() == "true" || dr["IsXetDuyet"].ToString() == "1";
                                            string duyetText = isDuyet ? "Đã duyệt" : "Chưa duyệt";
                                            duyetCell.Value = duyetText;
                                            duyetCell.Style.Font.Bold = true;
                                            duyetCell.Style.Font.Color.SetColor(isDuyet ? Color.Green : Color.Red);
                                            duyetCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                            duyetCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                            // Kiểm tra block merge
                                            if (currentDuyetValue == null)
                                            {
                                                currentDuyetValue = duyetText;
                                                startMergeRow = row;
                                            }
                                            else if (currentDuyetValue != duyetText)
                                            {
                                                // Merge block trước đó
                                                if (startMergeRow < row - 1)
                                                {
                                                    newSheet.Cells[startMergeRow, duyetCol, row - 1, duyetCol].Merge = true;
                                                    newSheet.Cells[startMergeRow, duyetCol, row - 1, duyetCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                                }
                                                // Bắt đầu block mới
                                                currentDuyetValue = duyetText;
                                                startMergeRow = row;
                                            }

                                            row++;

                                        }

                                        if (startMergeRow < row - 1)
                                        {
                                            newSheet.Cells[startMergeRow, duyetCol, row - 1, duyetCol].Merge = true;
                                            newSheet.Cells[startMergeRow, duyetCol, row - 1, duyetCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        }

                                        int dauSizeCol = 1;
                                        int totalRow = row;
                                        int dataStartRow = row - filteredRows.Count;
                                        int dataEndRow = row - 1;

                                        // Ghi "Total" vào cột đầu tiên
                                        var labelCell = newSheet.Cells[totalRow, dauSizeCol];
                                        labelCell.Value = "Định Mức Gợi Ý";
                                        labelCell.Style.Font.Bold = true;
                                        labelCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        labelCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        labelCell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                        labelCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        labelCell.Style.Fill.BackgroundColor.SetColor(Color.White);

                                        // Xác định vùng cần merge: từ cột 2 → cột cuối cùng của dữ liệu
                                        int startMergeCol = dauSizeCol + 1;
                                        int endMergeCol = startMergeCol + colsWithAt.Count - 1;

                                        // Tạo công thức AVERAGE cho tất cả các cột dữ liệu
                                        List<string> ranges = new List<string>();
                                        int colIndex = startMergeCol;

                                        foreach (var col in colsWithAt)
                                        {
                                            // Bỏ qua cột duyệt nếu nằm trong vùng
                                            if (colIndex == duyetCol) { colIndex++; continue; }

                                            string colLetter = ExcelCellAddress.GetColumnLetter(colIndex);
                                            ranges.Add($"{colLetter}{dataStartRow}:{colLetter}{dataEndRow}");
                                            colIndex++;
                                        }

                                        // Merge các cột lại và đặt công thức AVERAGE vào ô merged
                                        var mergeRange = newSheet.Cells[totalRow, startMergeCol, totalRow, endMergeCol];
                                        mergeRange.Merge = true;
                                        mergeRange.Formula = $"AVERAGE({string.Join(",", ranges)})";
                                        mergeRange.Style.Font.Bold = true;
                                        mergeRange.Style.Numberformat.Format = "#,##0.00";
                                        mergeRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        mergeRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        mergeRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                        mergeRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        mergeRange.Style.Fill.BackgroundColor.SetColor(Color.White);
                                        row +=2;
                                    }

                                    //row ++; // cách dòng
                                }

                                newSheet.Column(1).AutoFit();
                                newSheet.Column(3).AutoFit();
                                if (newSheet.Column(1).Width < 20) newSheet.Column(1).Width = 20;

                                //for (int i = 2; i <= newSheet.Dimension.End.Column; i++)
                                //    if (newSheet.Column(i).Width < 8) newSheet.Column(i).Width = 8;

                                //newSheet.View.FreezePanes(3, 2);
                                newSheet.PrinterSettings.FitToPage = true;
                                newSheet.PrinterSettings.FitToWidth = 1;
                                newSheet.PrinterSettings.Orientation = eOrientation.Landscape;
                                newSheet.PrinterSettings.LeftMargin = 0.5M;
                                newSheet.PrinterSettings.RightMargin = 0.5M;
                                newSheet.PrinterSettings.TopMargin = 0.75M;
                                newSheet.PrinterSettings.BottomMargin = 0.75M;
                            }
                        }
                    }
                    excelPackage.SaveAs(file);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xuất Excel: {ex.Message}");

            }
        }

        List<string> SplitTenMau(string tenMauRaw)
        {
            return tenMauRaw.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .ToList();
        }

        private void LoadCol() 
        {
            DataTable dttbcol = new DataTable();

            dttbcol.Columns.Add("Col", typeof(string));
            string[] fixedValues = {"Mã Vật Tư","ItemCode","Vật Tư","Đơn Vị","Khổ/Size","Mã Màu Vật Tư","Màu Vật Tư","Màu Sản Phẩm","Định Mức Tạm Tính","% Hao Hụt"};

            foreach (string value in fixedValues)
            {
                DataRow row = dttbcol.NewRow();
                row["Col"] = value;
                dttbcol.Rows.Add(row);
            }

            gridControl2.DataSource = dttbcol;
            gridView2.SelectAll();
        }


        private void LoadSize()
        {
            DataTable dttbSize = new DataTable();
            string url = $"{URL}BangSize/GetBangSizebom?para={mahang}&para1={makh}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dttbSize = JsonConvert.DeserializeObject<DataTable>(json);
            gridControl1.DataSource = dttbSize;
            gridView1.ExpandAllGroups();
            gridView1.SelectAll();
        }

        private List<string> GetSelectedColumns() 
        {
            List<string> selectedCol = new List<string>();
            int[] selectedRowhandle = gridView2.GetSelectedRows();
            foreach (int rowhandle in selectedRowhandle) 
            {
                if (rowhandle > 0 ) 
                {
                    DataRowView row = (DataRowView)gridView2.GetRow(rowhandle);
                    string columnName = row["Col"].ToString();
                    selectedCol.Add(columnName);
                }
            }
            return selectedCol;
        }

        private List<string> GetSelectedSizes()
        {
            List<string> selectedSizes = new List<string>();

            // Lấy các row được select trong gridControl1 (sizes)
            int[] selectedRowHandles = gridView1.GetSelectedRows();
            foreach (int rowHandle in selectedRowHandles)
            {
                if (rowHandle >= 0)
                {
                    DataRowView row = (DataRowView)gridView1.GetRow(rowHandle);
                    string sizeName = row["SizeSanXuat"].ToString();
                    string maNhomSize = row["MaNhomSize"].ToString();
                    selectedSizes.Add($"{sizeName}|{maNhomSize}");
                }
            }

            return selectedSizes;
        }
    }
}