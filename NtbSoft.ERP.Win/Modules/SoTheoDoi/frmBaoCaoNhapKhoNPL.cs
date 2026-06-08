using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.SoTheoDoi
{
    public partial class frmBaoCaoNhapKhoNPL : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        public frmBaoCaoNhapKhoNPL()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            CreateSearchLookUpSoLo();
            CreateSearchLookUpItems();

            colXuatKho.OptionsColumn.ReadOnly = true;
            colXuatKho.OptionsColumn.AllowEdit = false;
            colXuatKho.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;

            colTonKho_CT.OptionsColumn.ReadOnly = true;
            colTonKho_CT.OptionsColumn.AllowEdit = false;
            colTonKho_CT.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;

            this.gridView1.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView_CustomUnboundColumnData);
            this.gridView2.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView_CustomUnboundColumnData);

        }
        private void CreateSearchLookUpSoLo()
        {
            string url = string.Format("{0}?", URL + "BaoCaoKhoNPL/GetSoLo");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                repositoryItemSearchLookUpEdit2.DataSource = null;
                barEditItemSoLo.EditValue = null;
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            repositoryItemSearchLookUpEdit2.DataSource = tbl;
            repositoryItemSearchLookUpEdit2.DisplayMember = "SoLo";
            repositoryItemSearchLookUpEdit2.ValueMember = "SoLoID";
            barEditItemSoLo.Refresh();
        }
        private void CreateSearchLookUpItems()
        {
            string url = string.Format("{0}?soloid={1}", URL + "BaoCaoKhoNPL/GetCayVai", barEditItemSoLo.EditValue == null ? "" : barEditItemSoLo.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                repositoryItemSearchLookUpEdit1.DataSource = null;
                barEditItemItems.EditValue = null;
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            repositoryItemSearchLookUpEdit1.DataSource = tbl;
            repositoryItemSearchLookUpEdit1.DisplayMember = "MaVTGhep";
            repositoryItemSearchLookUpEdit1.ValueMember = "MaVTID";
            barEditItemItems.Refresh();
        }
        private void LoadData()
        {
            string url = string.Format("{0}?soloid={1}&&items={2}", URL + "BaoCaoKhoNPL/Get", barEditItemSoLo.EditValue == null ? "" : barEditItemSoLo.EditValue.ToString(),
               barEditItemItems.EditValue == null ? "" : barEditItemItems.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                gridControl1.DataSource = null;
                gridControl2.DataSource = null;
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            gridControl1.DataSource = tbl;
        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barEditItemSoLo.EditValue == null && barEditItemItems.EditValue == null) return;

            var editValue = barEditItemItems.EditValue;
            var dataSource = repositoryItemSearchLookUpEdit1.DataSource as DataTable;
            string npl = string.Empty;
            if (dataSource != null && editValue != null)
            {
                DataRow[] selectedRows = dataSource.Select($"[MaVTID] = '{editValue}'");
                if (selectedRows.Length > 0)
                {
                    npl = selectedRows[0]["NPL"]?.ToString();
                    if (npl == "False")
                    {
                        colXuatCT.Visible = false;
                        colTonKho_CT.Visible = false;
                    }
                    else 
                    {
                        colXuatCT.Visible = true;
                        colTonKho_CT.Visible = true;
                    }
                }
            }

            LoadData();
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow focusedRow = gridView1.GetDataRow(e.FocusedRowHandle);

            if (focusedRow != null)
            {
                string url = string.Format("{0}?soloid={1}&&manpl={2}", URL + "BaoCaoKhoNPL/GetChiTiet", focusedRow["SoLoID"].ToString() == "" ? "" : focusedRow["SoLoID"].ToString(),
                focusedRow["MaNPL"].ToString() == "" ? "" : focusedRow["MaNPL"].ToString());
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    gridControl2.DataSource = null;
                    return;
                }
                bool.TryParse(focusedRow["NPL"]?.ToString(), out bool IsNPL);
                ToogleCol(IsNPL);
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                gridControl2.DataSource = tbl;

            }
        }

        private void gridView1_DataSourceChanged(object sender, EventArgs e)
        {
            DataRow focusedRow = gridView1.GetDataRow(gridView1.FocusedRowHandle);

            if (focusedRow != null)
            {
                string url = string.Format("{0}?soloid={1}&&manpl={2}", URL + "BaoCaoKhoNPL/GetChiTiet", focusedRow["SoLoID"].ToString() == "" ? "" : focusedRow["SoLoID"].ToString(),
                focusedRow["MaNPL"].ToString() == "" ? "" : focusedRow["MaNPL"].ToString());
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    gridControl2.DataSource = null;
                    return;
                }
                bool.TryParse(focusedRow["NPL"]?.ToString(), out bool IsNPL);
                ToogleCol(IsNPL);
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                gridControl2.DataSource = tbl;
            }
        }

        private void gridView1_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gridView1_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {

            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }

        private void gridView2_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gridView2_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {

            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }

        private void btnExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable tbl = gridControl1.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0)
            {
               
                return;
            }
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("BaoCaoNhapKhoNPL{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "BaoCaoNhapKhoNPL.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                Export(TemplateFileName, ExportFileName, tbl);

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
        public void Export(string TemplateFileName, string ExportFileName, DataTable tbl)
        {
            try
            {
                FileInfo file = new FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    excelPackage.Workbook.Properties.Author = "Cty NTB";
                    excelPackage.Workbook.Properties.Title = "";

                    FileInfo templateFile = new FileInfo(TemplateFileName);
                    string tenSheet = $"Báo cáo nhập kho NPL";
                    //if (barEditItemItems.EditValue==null || string.IsNullOrWhiteSpace( barEditItemItems.EditValue.ToString()))
                    //{
                    //    tenSheet = $"{ barEditItemItems.EditValue.ToString()}_{ repositoryItemSearchLookUpEdit1.Text}";
                    //}                   
                     
                     
                        var newSheet = excelPackage.Workbook.Worksheets.Add(tenSheet);
                        // Lấy dữ liệu của nhóm dưới dạng DataTable (nếu cần)
                      

                        newSheet.Cells["A1"].Value = "BÁO CÁO NHẬP KHO NPL";
                        newSheet.Cells["A1:K1"].Merge = true;
                        newSheet.Cells["A1"].Style.Font.Size = 16;
                        newSheet.Cells["A1"].Style.Font.Bold = true;
                        newSheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                       
                        int headerRow = 3;
                        newSheet.Cells[headerRow, 1].Value = "Số lô";
                        newSheet.Cells[headerRow, 2].Value = "Nhóm";
                        newSheet.Cells[headerRow, 3].Value = "Mã vật tư";
                        newSheet.Cells[headerRow, 4].Value = "Chi tiết";
                        newSheet.Cells[headerRow, 5].Value = "Màu vật tư";
                        newSheet.Cells[headerRow, 6].Value = "Khổ/Size";
                        newSheet.Cells[headerRow, 7].Value = "Đơn vị";
                        newSheet.Cells[headerRow, 8].Value = "Theo CT";
                        newSheet.Cells[headerRow, 9].Value = "Thực nhập";
                        newSheet.Cells[headerRow, 10].Value = "Xuất";
                        newSheet.Cells[headerRow, 11].Value = "Thu hồi";
                        newSheet.Cells[headerRow, 12].Value = "Tồn kho";
               
                        newSheet.Cells[headerRow, 1, headerRow, 12].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        newSheet.Cells[headerRow, 1, headerRow, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        newSheet.Cells[headerRow, 1, headerRow, 12].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        newSheet.Cells[headerRow, 1, headerRow, 12].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        newSheet.Cells[headerRow, 1, headerRow, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        newSheet.Cells[headerRow, 1, headerRow, 12].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        newSheet.Cells[headerRow, 1, headerRow, 12].Style.Font.Bold = true;
                        int startRow = headerRow + 1;

                        // Thêm số dòng mới tương ứng số dòng của datatable (nếu template chưa có sẵn)
                        newSheet.InsertRow(startRow, tbl.Rows.Count);
                            int row = 0;
                        // Ghi dữ liệu từng dòng vào sheet
                        for (int i = 0; i < tbl.Rows.Count; i++)
                        {
                            var dr = tbl.Rows[i];
                            row = startRow + i;

                            newSheet.Cells[row, 1].Value = dr["SoLo"];    
                            newSheet.Cells[row, 2].Value = dr["TenNhom"];    
                            newSheet.Cells[row, 3].Value = dr["MaVatTu"];      
                            newSheet.Cells[row, 4].Value = dr["ChiTiet"];    
                            newSheet.Cells[row, 5].Value = dr["MauVT"];  
                            newSheet.Cells[row, 6].Value = dr["KhoVai"];
                            newSheet.Cells[row, 7].Value = dr["TenDV"];
                            newSheet.Cells[row, 8].Value = dr["TheoCT"];
                            newSheet.Cells[row, 9].Value = dr["ThucNhap"];
                            newSheet.Cells[row, 10].Value = dr["SLXuat"];
                            newSheet.Cells[row, 11].Value = dr["SLThuHoi"];
                            newSheet.Cells[row, 12].Formula =$"(I{row}-J{row}) + K{row}";


                    }


                        var borderData = newSheet.Cells[startRow, 1, startRow + tbl.Rows.Count - 1, 11].Style.Border;
                        borderData.Bottom.Style =
                        borderData.Top.Style =
                        borderData.Left.Style =
                        borderData.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


                        // Sau vòng for fill dữ liệu
                        // Tính tổng
                    //    decimal sumTheoCT = 0, sumTN = 0, SumSLXuat = 0,sumSLTon=0;
                    //foreach (DataRow dr in tbl.Rows)
                    //{
                    //    decimal.TryParse(dr["TheoCT"]?.ToString(), out decimal val1);
                    //    decimal.TryParse(dr["ThucNhap"]?.ToString(), out decimal val2);
                    //    decimal.TryParse(dr["SLXuat"]?.ToString(), out decimal val3);
                    //    //decimal.TryParse(dr["SLTon"]?.ToString(), out decimal val4);
                    //    sumTheoCT += val1;
                    //    sumTN += val2;
                    //    SumSLXuat += val3;
                    //    //sumSLTon += val4;
                    //}

                    // Thêm dòng tổng
                    int sumRow = startRow + tbl.Rows.Count;

                    newSheet.Cells[sumRow, 1].Value = "TỔNG ";
                    newSheet.Cells[sumRow, 1, sumRow, 7].Merge = true;
                    newSheet.Cells[sumRow, 1, sumRow, 11].Style.Font.Bold = true;
                    newSheet.Cells[sumRow, 1, sumRow, 11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    newSheet.Cells[sumRow, 1, sumRow, 11].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                    newSheet.Cells[sumRow, 1, sumRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    // Giả sử: startRow và row đã được khai báo đúng
                   
                    newSheet.Cells[sumRow, 8].Formula = string.Format("SUM(H{0}:H{1})", startRow, row);
                    newSheet.Cells[sumRow, 9].Formula = string.Format("SUM(I{0}:I{1})", startRow, row);
                    newSheet.Cells[sumRow, 10].Formula = string.Format("SUM(J{0}:J{1})", startRow, row);
                    newSheet.Cells[sumRow, 11].Formula = string.Format("SUM(K{0}:K{1})", startRow, row);
                    newSheet.Cells[sumRow, 12].Formula = string.Format("SUM(L{0}:L{1})", startRow, row);


                    newSheet.Cells[sumRow, 8].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    newSheet.Cells[sumRow, 9].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    newSheet.Cells[sumRow, 10].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                    newSheet.Cells[sumRow, 11].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                    // Dòng ngày tháng chuyển xuống 1 hàng sau dòng tổng
                    newSheet.Cells[sumRow + 1, 11].Value = $"Ngày: {DateTime.Now:dd/MM/yyyy}";
                    newSheet.Cells[newSheet.Dimension.Address].AutoFitColumns();
                    

                    // Lưu file Excel
                    excelPackage.SaveAs(file);
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc throw để debug
                Console.WriteLine(ex.Message);
            }
        }

        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn2)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column == gridColumn6)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gridView1.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gridView1.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        

        }

        private void gridView2_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn18)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column == gridColumn22)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gridView1.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gridView1.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }

        private void repositoryItemSearchLookUpEdit1View_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {

            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            if (info.Column != null && info.Column.FieldName == "NPL" && info.EditValue.ToString() != "")
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);

                if (isChecked)
                {
                    info.GroupText = "Nguyên liệu";
                }
                else
                {
                    info.GroupText = "Phụ liệu";
                }
            }

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();

            if (info.Column == gridColumn49)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column == gridColumn48)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (repositoryItemSearchLookUpEdit1View.IsGroupRow(e.RowHandle))
            {
                int groupIndex = repositoryItemSearchLookUpEdit1View.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }

        /*Tính Tồn Kho*/

       
        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if ((e.Column == colXuatKho || e.Column == colTonKho_CT) && e.IsGetData)
            {
                double thucNhap = 0, slXuat = 0, slThuHoi = 0;

                object valThucNhap = view.GetRowCellValue(e.ListSourceRowIndex, "ThucNhap");
                object valSLXuat = view.GetRowCellValue(e.ListSourceRowIndex, "SLXuat");
                object valSLThuHoi = view.GetRowCellValue(e.ListSourceRowIndex, "SLThuHoi");


                double.TryParse(Convert.ToString(valThucNhap), out thucNhap);
                double.TryParse(Convert.ToString(valSLXuat), out slXuat);
                double.TryParse(Convert.ToString(valSLThuHoi), out slThuHoi);


                double result = (thucNhap - slXuat) + slThuHoi;


                e.Value = result;
            }
        }
        private void ToogleCol(bool IsNPL)
        {
            colTonKho_CT.Visible = IsNPL;
            colXuatCT.Visible = IsNPL;
            colThuHoi_CT.Visible = IsNPL;

        }

    }
}
