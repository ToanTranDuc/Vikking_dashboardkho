using CefSharp;
using CefSharp.Handler;
using CefSharp.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
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

namespace NtbSoft.ERP.Win.Modules.BaoCao
{
    public partial class frmBaoCaoNhanLucWeb : Form
    {
        System.Configuration.AppSettingsReader settingsReader =
                                           new System.Configuration.AppSettingsReader();

        string webUrl = string.Empty;
        string branchID = string.Empty;
        string account = string.Empty;
        string useNameLogin = string.Empty;
        string urlMain = string.Empty;
        string urlWeb = string.Empty;
        DataTable userRoleDb = new DataTable();
        public static HttpClientExtension _clientExtension = new HttpClientExtension();
        private ChromiumWebBrowser chromiumWebBrowser;

        public frmBaoCaoNhanLucWeb()
        {
            useNameLogin = GlobleData.UserName;
            urlMain = settingsReader.GetValue("main", typeof(string)).ToString();
            urlWeb = settingsReader.GetValue("web_BCNhanSuUrl", typeof(string)).ToString();
            InitializeComponent();

            LoadQuyenTruyCapCacMayChu();
        }
        private void LoadQuyenTruyCapCacMayChu()
        {
            try
            {
                string sqlQuery = string.Format(@"select * from PhanQuyen_Users PQ_user 
                                                            left join DonViSanXuat dvsx on PQ_user.MaDVSX = dvsx.MaDVSX 
                                                            where Username = '{0}' and host is not null ", useNameLogin);
                userRoleDb = GetDataFromOtherServer.GetData(urlMain, sqlQuery);
                if (userRoleDb == null)
                    MessageBox.Show("Tài khoản người dùng chưa được cấp quyền!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    foreach (DataRow row in userRoleDb.Rows)
                        cbxChooseDVSX.Properties.Items.Add(row["TenDVSX"].ToString());
                    cbxChooseDVSX_Bar.EditValue = "Chọn đơn vị sản xuất";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void loadWebView(string BranchId, string brandName)
        {
            try
            {
                chromiumWebBrowser = new ChromiumWebBrowser();
                chromiumWebBrowser.Dock = DockStyle.Fill;
                Controls.Add(chromiumWebBrowser);
                chromiumWebBrowser.ConsoleMessage += ChromiumWebBrowser_ConsoleMessage;
                chromiumWebBrowser.Load(urlWeb);
                LoadUrlAsyncResponse resultStart = Task.Run(
                    async () => { return await chromiumWebBrowser.WaitForInitialLoadAsync(); }
                    ).Result;
                // Execute the script after the page has loaded
                var script = string.Format(@"setBranchIdAndUser('{0}','{1}','{2}');", BranchId, brandName, useNameLogin); // Replace with your actual script
                await chromiumWebBrowser.EvaluateScriptAsync(script);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void ExportExcel(JObject json)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";

            Sfd.FileName = string.Format("BaoCaoNguonNhanLuc{0}", DateTime.Now.Millisecond.ToString());

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateBaoCaoNhanLuc.xlsx";

                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;

                Export(TemplateFileName, ExportFileName, json);

                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

                DialogResult dr = MessageBox.Show("Mở file?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
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
        public void Export(string TemplateFileName, string ExportFileName, JObject Param)
        {
            try
            {
                string fileName = $"BaoCAoNguonNhanLuc";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                string server = string.Empty;
                string sql = string.Empty;
                int Row = 6; int StartRow = 6;
                string Option = Param["Option"]?.ToString();

                string SV_api = settingsReader.GetValue($"{Param["BrandID"]?.ToString()}", typeof(string)).ToString(); 
                sql = $"EXEC SP_SEW_NHANLUC @Action='GetNgay', @Ngay='{Param["LaborOfDate"]?.ToString()}', @LineX='', @fromDate='', @toDate='', @year=''";
                DataTable dt_LaborOfDate = GetDataFromOtherServer.GetData(SV_api, sql);
                sql = $"EXEC SP_SEW_NHANLUC @Action='GetSLAll', @Ngay='', @LineX='', @fromDate='{Param["FromDate"]?.ToString()}', @toDate='{Param["ToDate"]?.ToString()}', @year=''";
                DataTable dt_Labor = GetDataFromOtherServer.GetData(SV_api, sql);
                sql = $"EXEC SP_SEW_NHANLUC @Action = '{(Option?.ToLower() == "all year" ? "GetYear" : "GetMonth")}', @Ngay = '', @LineX = '', @fromDate = '', @toDate = '', @year = '{(Option?.ToLower() == "all year" ? "" : Option)}'";
                DataTable dt_LaborYear = GetDataFromOtherServer.GetData(SV_api, sql);

                System.IO.FileInfo file = new System.IO.FileInfo(TemplateFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage package = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {
                        // đặt tên người tạo file
                        package.Workbook.Properties.Author = "Cty NTB";

                        // đặt tiêu đề cho file
                        package.Workbook.Properties.Title = "Bao-cao-tien-so-san-xuat";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;
                        /// coppy here
                        if (dt_LaborOfDate?.Rows?.Count > 0)
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                            worksheet.Cells["B3"].Value = Convert.ToDateTime(Param["LaborOfDate"]?.ToString()).ToString("dd-MM-yyyy");
                            worksheet.Cells.Style.Font.Name = "Times New Roman";
                            DrawChartSheet1(worksheet, dt_LaborOfDate);
                        }
                        if (dt_Labor?.Rows?.Count > 0)
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet2"];
                            worksheet.Cells.Style.Font.Name = "Times New Roman";
                            DrawChartSheet2(worksheet, dt_Labor);
                        }
                        if (dt_LaborYear?.Rows?.Count > 0)
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet3"];
                            worksheet.Cells.Style.Font.Name = "Times New Roman";
                            DrawChartSheet3(worksheet, dt_LaborYear, Option);
                        }
                        ///
                        FileInfo resultFile = new FileInfo(resultFilePath);
                        package.SaveAs(resultFile);
                    }

                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                object misValue = System.Reflection.Missing.Value;
                throw new Exception(ex.Message);
            }

        }
        private void ChromiumWebBrowser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            // e.Message: Nội dung của thông báo
            // e.Source: Nguồn gốc của thông báo (ví dụ: script, console-api)
            // e.Line: Số dòng trong file script
            // e.SourceId: ID của nguồn

            // Ví dụ: nếu bắt được log "ExportExcel"
            Console.WriteLine(e.Message);
            if (e.Message.Contains("ExportExcel") == true)
            {
                try
                {
                    JObject json = JsonConvert.DeserializeObject<JObject>(e.Message.Replace("ExportExcel@", ""));
                    ExportExcel(json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            else if (e.Message == "FULL_RELOAD")
            {
                chromiumWebBrowser.Reload();
            }
        }

        private void cbxChooseDVSX_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void cbxChooseDVSX_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            Controls.Remove(chromiumWebBrowser);
            string maDVSXRequest = "";
            string tenDVSXRequest = "";
            foreach (DataRow row in userRoleDb.Rows)
            {
                if (e.NewValue == row["TenDVSX"].ToString())
                {
                    maDVSXRequest = row["MaDVSX"].ToString();
                    tenDVSXRequest = row["TenDVSX"].ToString();
                }
            }
            loadWebView(maDVSXRequest, tenDVSXRequest);
            cbxChooseDVSX_Bar.EditValue = tenDVSXRequest;
        }
        private void DrawChartSheet1(ExcelWorksheet worksheet, DataTable dt_LaborOfDate)
        {
            int Row = 6;
            //worksheet.Cells[Row, 1].Value = "Chuyền";
            //worksheet.Cells[Row, 2].Value = "Số Lao Động";
            //worksheet.Cells[Row, 3].Value = "Số Lao Động Vắng";

            foreach (DataRow row in dt_LaborOfDate.Rows)
            {
                Row++;
                worksheet.Cells[Row, 1].Value = row["DepID"]?.ToString();
                worksheet.Cells[Row, 2].Value = Convert.ToInt32(row["SoLaoDong"]?.ToString());
                worksheet.Cells[Row, 3].Value = Convert.ToInt32(row["SoLaoDongVang"]?.ToString());

            }
            //// Tạo biểu đồ tròn
            var pieChart = worksheet.Drawings.AddChart("ChiPhiChart", eChartType.Pie) as ExcelPieChart;
            pieChart.Title.Text = "Biểu Đồ Lao Động Trong Ngày";
            pieChart.SetPosition(20, 0, 6, 0);
            pieChart.SetSize(600, 300);

            // Thêm dữ liệu cho biểu đồ tròn
            var series2 = pieChart.Series.Add(worksheet.Cells[$"B7:B{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            series2.Header = "Chuyền";


            // Tạo biểu đồ cột
            var columnChart = worksheet.Drawings.AddChart("LaoDongTheoNgayChart", eChartType.ColumnClustered) as ExcelBarChart;
            columnChart.Title.Text = "Biểu Đồ Lao Động Vắng Trong Ngày";
            columnChart.Style = OfficeOpenXml.Drawing.Chart.eChartStyle.Style16;
            columnChart.SetPosition(5, 0, 6, 0);
            columnChart.SetSize(600, 300);

            // Thêm dữ liệu cho biểu đồ cột
            var chartSerieCol = columnChart.Series.Add(worksheet.Cells[$"C7:C{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            chartSerieCol.Header = "Chuyền";

            columnChart.Legend.Position = eLegendPosition.Bottom;
            columnChart.YAxis.Title.Text = "Số Lượng";
            columnChart.XAxis.Title.Text = "Chuyền";

            var range = worksheet.Cells[$"A7:C{Row}"];
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        }

        private void DrawChartSheet2(ExcelWorksheet worksheet, DataTable dt_Labor)
        {
            int Row = 6;
            //worksheet.Cells[Row, 1].Value = "Chuyền";
            //worksheet.Cells[Row, 2].Value = "Tổng Lao Động";
            //worksheet.Cells[Row, 3].Value = "Tổng Lao Động Vắng";


            foreach (DataRow row in dt_Labor.Rows)
            {
                Row++;
                worksheet.Cells[Row, 1].Value = row["DepID"]?.ToString();
                worksheet.Cells[Row, 2].Value = Convert.ToInt32(row["TongLD"]);
                worksheet.Cells[Row, 3].Value = Convert.ToInt32(row["LDV"]);
            }


            var columnChart = worksheet.Drawings.AddChart("LaoDongTheoNgayChart", eChartType.ColumnStacked) as ExcelBarChart;
            columnChart.Title.Text = $"Biểu Đồ Số Lao Động Theo Từng Chuyền";
            columnChart.Style = OfficeOpenXml.Drawing.Chart.eChartStyle.Style11;
            columnChart.SetPosition(5, 0, 6, 0);
            columnChart.SetSize(600, 300);

            var totalLaborSeries = columnChart.Series.Add(worksheet.Cells[$"B7:B{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            totalLaborSeries.Header = "Tổng Lao Động";


            var absentLaborSeries = columnChart.Series.Add(worksheet.Cells[$"C7:C{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            absentLaborSeries.Header = "Tổng Lao Động Vắng";


            columnChart.Legend.Position = eLegendPosition.Bottom;
            columnChart.XAxis.Title.Text = "Chuyền";




            var range = worksheet.Cells["A7:C25"];
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        }


        private void DrawChartSheet3(ExcelWorksheet worksheet, DataTable dt_LaborOfYear, string Option)
        {
            int Row = 6;
            worksheet.Cells[Row, 1].Value = $"{(Option?.ToLower() == "all year" ? "Năm" : "Tháng")}";
            //worksheet.Cells[Row, 2].Value = "Số Lao Động ";
            //worksheet.Cells[Row, 3].Value = "Số Lao Động Vắng";
            //worksheet.Cells[Row, 4].Value = "Số Lao Động Thêm";
            //worksheet.Cells[Row, 5].Value = "Số Lao Động Nghỉ Việc";

            foreach (DataRow row in dt_LaborOfYear.Rows)
            {
                Row++;
                worksheet.Cells[Row, 1].Value = $"{(Option?.ToLower() == "all year" ? "" : "Tháng ")}" + row[$"{(Option?.ToLower() == "all year" ? "Nam" : "thang")}"]?.ToString();
                worksheet.Cells[Row, 2].Value = Convert.ToInt32(row["SLD"]?.ToString());
                worksheet.Cells[Row, 3].Value = Convert.ToInt32(row["SLDV"]?.ToString());
                worksheet.Cells[Row, 4].Value = Convert.ToInt32(row["SoLaoDongThem"]?.ToString());
                worksheet.Cells[Row, 5].Value = Convert.ToInt32(row["SoLaoDongNghiViec"]?.ToString());

            }


            //// Tạo biểu đồ đường (line chart)
            var lineChartIncrease = worksheet.Drawings.AddChart("ChiPhiChart", eChartType.LineStacked) as ExcelLineChart;
            lineChartIncrease.Title.Text = "Biểu Đồ Biến Động Lao Động ";
            lineChartIncrease.SetPosition(20, 0, 6, 0);
            lineChartIncrease.SetSize(800, 300);

            var lineChartReduced = worksheet.Drawings.AddChart("ChiPhiChart1", eChartType.LineStacked) as ExcelLineChart;
            lineChartReduced.Title.Text = "Biểu Đồ Biến Động Lao Động";
            lineChartReduced.SetPosition(20, 0, 8, 0);
            lineChartReduced.SetSize(800, 300);

            // Thêm dữ liệu cho biểu đồ đường
            var series2 = lineChartReduced.Series.Add(worksheet.Cells[$"E7:E{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            series2.Header = "Số LĐ Giảm";

            var series1 = lineChartIncrease.Series.Add(worksheet.Cells[$"D7:D{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            series1.Header = "Số LĐ Tăng";




            //lineChart.Legend.Position = eLegendPosition.Bottom;
            ////lineChart.YAxis.Title.Text = "Số Lượng";
            //lineChart.XAxis.Title.Text = "Thời Gian";

            // Tạo biểu đồ cột
            var columnChart = worksheet.Drawings.AddChart("LaoDongTheoNgayChart", eChartType.ColumnClustered) as ExcelBarChart;
            columnChart.Title.Text = $"Biểu Đồ Số Lao Động Vắng Theo Thời Gian";
            columnChart.SetPosition(5, 0, 6, 0);
            columnChart.SetSize(600, 300);

            // Thêm dữ liệu cho biểu đồ cột
            var chartSerieCol = columnChart.Series.Add(worksheet.Cells[$"B7:B{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            chartSerieCol.Header = "Chuyền";

            worksheet.Cells.Style.Font.Name = "Times New Roman";


            var range = worksheet.Cells[$"A7:E{Row}"];
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        }
    }
}
