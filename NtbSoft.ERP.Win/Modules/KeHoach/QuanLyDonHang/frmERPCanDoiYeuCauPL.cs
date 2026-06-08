using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Erp.Kho
{
    public partial class frmERPCanDoiYeuCauPL : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable _dtData;
        string _lenhsx = string.Empty;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        List<DevExpress.XtraGrid.Columns.GridColumn> lstColGroupDraw = new List<DevExpress.XtraGrid.Columns.GridColumn>();
        public frmERPCanDoiYeuCauPL()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            CheckPerminsion();
            initSearchLookUp();
            CreateSearchLookUpLenh();


            //var sum = new DevExpress.XtraGrid.GridGroupSummaryItem()
            //{
            //    FieldName = "TotalCat",
            //    SummaryType = DevExpress.Data.SummaryItemType.Custom,
            //    ShowInGroupColumnFooter = gridColumn30,
            //    DisplayFormat = "{0:#,##0.00}"
            //};
            //gV.GroupSummary.Add(sum);

            lstColGroupDraw.AddRange(new[] { colVaTu, colTenNhom, colTenNhom_Tong, colVatTu_Tong });
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                barButtonItem2.Enabled = false;
            }
        }
        private void initSearchLookUp()
        {

            searchLookUpEditLenhSX.Properties.DisplayMember = "DisPlay";
            searchLookUpEditLenhSX.Properties.ValueMember = "MaLenh";

        }
        private void CreateSearchLookUpLenh()
        {
            string url = $"{URL}CanDoiYCNPL/Get?Action=GETLENH";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                searchLookUpEditLenhSX.Properties.DataSource = null;
                searchLookUpEditLenhSX.EditValue = null;
                return;
            }


            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditLenhSX.Properties.DataSource = tbl;

            searchLookUpEditLenhSX.RefreshEditValue();
            searchLookUpEditLenhSX.Refresh();
            if (!string.IsNullOrWhiteSpace(_lenhsx))
            {
                searchLookUpEditLenhSX.EditValue = _lenhsx;
            }
        }
        private void loadData()
        {
            //if (searchLookUpEditLenhSX.EditValue == null || searchLookUpEditLenhSX.EditValue.ToString() == "")
            //{
            //    MessageBox.Show("Vui lòng chọn Lệnh SX", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            DataRow dr = gV_SearchLookUpEditLenhSX.GetFocusedDataRow();
            if (dr == null) return;
            string _lenhsx = searchLookUpEditLenhSX.EditValue == null ? "" : searchLookUpEditLenhSX.EditValue.ToString();
            string url = $"{URL}CanDoiYCNPL/Get?Action=GETTONGPL&para1={_lenhsx}&para2={dr["MaKhachHang"]}&para3={dr["MaHang"]}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {

                gC.DataSource = null;
                return;
            }

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);


            gC.DataSource = tbl;
            Load_TQ_YC_PhuLieu();
        }

        private void searchLookUpEditLenhSX_EditValueChanged(object sender, EventArgs e)
        {
            loadData();
            _lenhsx = searchLookUpEditLenhSX.EditValue == null ? "" : searchLookUpEditLenhSX.EditValue.ToString();
        }

        private void gV_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            //if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
            //{
            //    //decimal sumSoMetYC = 0;
            //    //decimal sumDaNhan = 0;
            //    //for (int i = 0; i < gV.RowCount; i++)
            //    //{
            //    //    var soMetYC = Convert.ToDecimal(gV.GetRowCellValue(i, "SoMetYC"));
            //    //    var daNhan = Convert.ToDecimal(gV.GetRowCellValue(i, "SoMetDN"));
            //    //    sumSoMetYC += soMetYC;
            //    //    sumDaNhan += daNhan;
            //    //}
            //    //e.TotalValue = sumDaNhan - sumSoMetYC;
            //}
        }
        private void gV_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gV.RowCount > 0)
            {

                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {

                        DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Sửa", SuaPhieu);
                        e.Menu.Items.Add(menuDeleteItem);
                    }

                }
            }
        }
        private void SuaPhieu(object sender, EventArgs e)
        {
            try
            {
                //DataRow dr = gV.GetFocusedDataRow();
                //if (dr == null) return;
                //if (searchLookUpEditLenhSX.EditValue == null || searchLookUpEditLenhSX.EditValue.ToString() == "") return;
                //frmERPLichSuXuatKhoPL frm = new frmERPLichSuXuatKhoPL(searchLookUpEditLenhSX.EditValue.ToString(), dr["MaPhieu"].ToString(), _allowEdit, _allowDelete);
                //frm.WindowState = FormWindowState.Maximized;
                //frm.ShowDialog();
                //loadData();

            }
            catch (Exception ex)
            {

            }
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CreateSearchLookUpLenh();
            loadData();

        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ////if (searchLookUpEditLenhSX.EditValue == null || searchLookUpEditLenhSX.EditValue.ToString() == "") return;
            //frmPhieuYeuCauXuatKhoPhuLieu frm = new frmPhieuYeuCauXuatKhoPhuLieu(searchLookUpEditLenhSX.EditValue == null ? "" : searchLookUpEditLenhSX.EditValue.ToString());
            //frm.WindowState = FormWindowState.Maximized;
            //frm.ShowDialog();
            //CreateSearchLookUpLenh();
            //loadData();
        }

        private void btnExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable tbl = gC.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn Lệnh SX.");
                return;
            }
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("CanDoiYeuCauNPL{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "CanDoiYeuCauNPL.xlsx";
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

        private void gV_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gV_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }

        private void gV_CustomDrawRowFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gV_CustomDrawRowFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }

        private void gV_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            Pen headerBorderPen = new Pen(Color.FromArgb(100, 128, 128, 128), 3);
            e.Graphics.DrawRectangle(headerBorderPen, e.Bounds);
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


                    var nhomVTs = tbl.AsEnumerable()
                                 .GroupBy(x => new
                                 {
                                     TenNhom = x["TenNhom"].ToString(),
                                     MaVT = x["MaVT"].ToString(),
                                     TenVT = x["TenVT"].ToString()
                                 });
                    foreach (var group in nhomVTs)
                    {
                        // Lấy thông tin của nhóm
                        string tenSheet = $"{group.Key.TenNhom}_{group.Key.MaVT}";
                        // Tạo sheet mới (tên sheet không nên dài quá hoặc chứa ký tự đặc biệt)
                        var newSheet = excelPackage.Workbook.Worksheets.Add(tenSheet);

                     

                        // Lấy dữ liệu của nhóm dưới dạng DataTable (nếu cần)
                        DataTable groupTable = group.CopyToDataTable();

                        newSheet.Cells["A1"].Value = "CÂN ĐỐI YÊU CẦU NPL";
                        newSheet.Cells["A1:N1"].Merge = true;
                        newSheet.Cells["A1"].Style.Font.Size = 16;
                        newSheet.Cells["A1"].Style.Font.Bold = true;
                        newSheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        // 2. Tiêu đề cột
                        int headerRow = 3;
                        newSheet.Cells[headerRow, 1].Value = "Mã lệnh";
                        newSheet.Cells[headerRow, 2].Value = "Mã hàng";
                        newSheet.Cells[headerRow, 3].Value = "Khách hàng";
                        newSheet.Cells[headerRow, 4].Value = "Loại vải";
                        newSheet.Cells[headerRow, 5].Value = "Mã vật tư";
                        newSheet.Cells[headerRow, 6].Value = "Tên vật tư";
                        newSheet.Cells[headerRow, 7].Value = "Màu";
                        newSheet.Cells[headerRow, 8].Value = "Khổ/Size";
                        newSheet.Cells[headerRow, 9].Value = "Đơn vị";
                        newSheet.Cells[headerRow, 10].Value = "Mã phiếu";
                        newSheet.Cells[headerRow, 11].Value = "Ngày yêu cầu";
                        newSheet.Cells[headerRow, 12].Value = "Yêu cầu";
                        newSheet.Cells[headerRow, 13].Value = "Thực xuất";
                        newSheet.Cells[headerRow, 14].Value = "Cân đối";
                        // ... các cột khác
                        newSheet.Cells[headerRow, 1, headerRow, 14].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        newSheet.Cells[headerRow, 1, headerRow, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        newSheet.Cells[headerRow, 1, headerRow, 14].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        newSheet.Cells[headerRow, 1, headerRow, 14].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        newSheet.Cells[headerRow, 1, headerRow, 14].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        newSheet.Cells[headerRow, 1, headerRow, 14].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        newSheet.Cells[headerRow, 1, headerRow, 14].Style.Font.Bold = true;
                        int startRow = headerRow + 1;

                        // Thêm số dòng mới tương ứng số dòng của datatable (nếu template chưa có sẵn)
                        newSheet.InsertRow(startRow, tbl.Rows.Count);

                        // Ghi dữ liệu từng dòng vào sheet
                        for (int i = 0; i < groupTable.Rows.Count; i++)
                        {
                            var dr = tbl.Rows[i];
                            int row = startRow + i;

                            newSheet.Cells[row, 1].Value = dr["MaLenh"];     // Cột 1
                            newSheet.Cells[row, 2].Value = dr["MaHang"];      // Cột 2
                            newSheet.Cells[row, 3].Value = dr["KhachHang"];      // Cột 3
                            newSheet.Cells[row, 4].Value = dr["TenNhom"];     // Cột 4
                            newSheet.Cells[row, 5].Value = dr["MaVT"];   // Cột 5
                            newSheet.Cells[row, 6].Value = dr["TenVT"];
                            newSheet.Cells[row, 7].Value = dr["Mau"];
                            newSheet.Cells[row, 8].Value = dr["KhoVai"];
                            newSheet.Cells[row, 9].Value = dr["TenDVVT"];
                            newSheet.Cells[row, 10].Value = dr["MaPhieu"];
                            newSheet.Cells[row, 11].Value = Convert.ToDateTime(dr["NgayTao"].ToString()).ToString("dd/MM/yyyy");
                            newSheet.Cells[row, 12].Value = dr["SoMetYC"];
                            newSheet.Cells[row, 13].Value = dr["SoMetDN"];
                            newSheet.Cells[row, 14].Value = dr["ToTalCat"];

                        }


                        var borderData = newSheet.Cells[startRow, 1, startRow + groupTable.Rows.Count - 1, 14].Style.Border;
                        borderData.Bottom.Style =
                        borderData.Top.Style =
                        borderData.Left.Style =
                        borderData.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


                        // Sau vòng for fill dữ liệu
                        // Tính tổng
                        decimal sumSoMetYC = 0, sumSoMetDN = 0, sumToTalCat = 0;
                        foreach (DataRow dr in groupTable.Rows)
                        {
                            decimal.TryParse(dr["SoMetYC"]?.ToString(), out decimal val1);
                            decimal.TryParse(dr["SoMetDN"]?.ToString(), out decimal val2);
                            decimal.TryParse(dr["ToTalCat"]?.ToString(), out decimal val3);
                            sumSoMetYC += val1;
                            sumSoMetDN += val2;
                            sumToTalCat += val3;
                        }

                        // Thêm dòng tổng
                        int sumRow = startRow + groupTable.Rows.Count;

                        newSheet.Cells[sumRow, 1].Value = "TỔNG ";
                        newSheet.Cells[sumRow, 1, sumRow, 11].Merge = true;
                        newSheet.Cells[sumRow, 1, sumRow, 14].Style.Font.Bold = true;
                        newSheet.Cells[sumRow, 1, sumRow, 14].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        newSheet.Cells[sumRow, 1, sumRow, 14].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                        newSheet.Cells[sumRow, 1, sumRow, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        newSheet.Cells[sumRow, 12].Value = sumSoMetYC;
                        newSheet.Cells[sumRow, 13].Value = sumSoMetDN;
                        newSheet.Cells[sumRow, 14].Value = sumToTalCat;

                        newSheet.Cells[sumRow, 12].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        newSheet.Cells[sumRow, 13].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        newSheet.Cells[sumRow, 14].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                     

                        // Dòng ngày tháng chuyển xuống 1 hàng sau dòng tổng
                        newSheet.Cells[sumRow + 1, 14].Value = $"Ngày: {DateTime.Now:dd/MM/yyyy}";
                        newSheet.Cells[newSheet.Dimension.Address].AutoFitColumns();
                    }

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

  

        private void gV_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (lstColGroupDraw.Contains(info.Column))
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
         
            if (gV.IsGroupRow(e.RowHandle))
            {
                int groupIndex = gV.GetRowLevel(e.RowHandle);
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

        private void Load_TQ_YC_PhuLieu()
        {
            DataRow dr = gV_SearchLookUpEditLenhSX.GetFocusedDataRow();
            if (dr == null) return;
            string _lenhsx = searchLookUpEditLenhSX.EditValue == null ? "" : searchLookUpEditLenhSX.EditValue.ToString();
            string url = $"{URL}CanDoiYCNPL/Get?Action=Get_TQ_YC_PhuLieu&para1={_lenhsx}&para2={dr["MaKhachHang"]}&para3={dr["MaHang"]}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {

                grcTQ_YC_PhuLieu.DataSource = null;
                return;
            }

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);


            grcTQ_YC_PhuLieu.DataSource = tbl;
        }

        private void gV_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "ThucNhan" && e.RowHandle >= 0)
            {
                DataRow row = gV.GetDataRow(e.RowHandle);
                if (row != null)
                {
                    double.TryParse(row["ThucNhan"]?.ToString(), out double SLThucNhan);
                    double.TryParse(row["SoMetDN"]?.ToString(), out double SLXuat);
                    if (SLThucNhan < SLXuat)
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                        e.Appearance.ForeColor = Color.IndianRed; // Đỏ nhạt, không chói

                    }
                    else
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                        e.Appearance.ForeColor = Color.SeaGreen; // Xanh lá dịu
                    }
                }
            }
        }

        private void grvTQ_YC_PhuLieu_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "ThucNhan" && e.RowHandle >= 0)
            {
                DataRow row = grvTQ_YC_PhuLieu.GetDataRow(e.RowHandle);
                if (row != null)
                {
                    double.TryParse(row["ThucNhan"]?.ToString(), out double SLThucNhan);
                    double.TryParse(row["SoMetDN"]?.ToString(), out double SLXuat);
                    if (SLThucNhan < SLXuat)
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                        e.Appearance.ForeColor = Color.IndianRed; // Đỏ nhạt, không chói

                    }
                    else
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                        e.Appearance.ForeColor = Color.SeaGreen; // Xanh lá dịu
                    }
                }
            }
        }
    }

}