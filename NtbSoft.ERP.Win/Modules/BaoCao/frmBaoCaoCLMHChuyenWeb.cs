using CefSharp;
using CefSharp.Handler;
using CefSharp.WinForms;
using Newtonsoft.Json;
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
    public partial class frmBaoCaoCLMHChuyenWeb : Form
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

        public frmBaoCaoCLMHChuyenWeb()
        {
            useNameLogin = GlobleData.UserName;
            urlMain = settingsReader.GetValue("main", typeof(string)).ToString();
            urlWeb = settingsReader.GetValue("web_BCCLMHChuyenUrl", typeof(string)).ToString();
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
        public void ExportExcel(BaoCaoChatLuong_Request json)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";

            Sfd.FileName = string.Format("BaoCAoChatLuongMaHangTrenTungChuyen_{0}", DateTime.Now.Millisecond.ToString());

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateBaoCaoChatLuongMaHangTrenChuyen.xlsx";

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
        public void Export(string TemplateFileName, string ExportFileName, BaoCaoChatLuong_Request obj_request)
        {
            try
            {
                string fileName = $"BaoCaoChatLuongMaHangTrenTungChuyen";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                int index = 0;
                int row = 6;
                int indexMerge = 0;

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
                        package.Workbook.Properties.Title = "Bao-cao-chat-luong-ma-hang-tren-tung-chuyen";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;
                        /// coppy here
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Report"];
                        List<BaoCaoChatLuongMaHangTrenChuyenViewModel> Lst_obj = obj_request.BaoCaoBC;
                        foreach (BaoCaoChatLuongMaHangTrenChuyenViewModel item in Lst_obj)
                        {
                            string currentMaHang = item.MaHang;
                            string previosMaHang = (index + 1 < Lst_obj.Count) ? Lst_obj[index + 1].To : "abcea";
                            worksheet.Cells[row, 1].Value = index + 1;
                            worksheet.Cells[row, 2].Value = item.To;
                            worksheet.Cells[row, 3].Value = item.MaLenh;
                            worksheet.Cells[row, 4].Value = item.MaHang;
                            worksheet.Cells[row, 5].Value = double.TryParse(item.TruocUi, out double truocUiValue) ? truocUiValue : 0;
                            worksheet.Cells[row, 6].Value = double.TryParse(item.SauUi, out double sauUiValue) ? sauUiValue : 0;
                            worksheet.Cells[row, 7].Value = item.LoiHuNoiCom;
                            worksheet.Cells[row, 8].Value = item.KetQua;
                            for (int col = 1; col <= 8; col++)
                            {
                                var cell = worksheet.Cells[row, col];
                                cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;


                                if (col != 7)
                                {
                                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                }
                            }

                            if (currentMaHang == previosMaHang)
                            {
                                indexMerge++;
                            }
                            else
                            {
                                MergeCellsByRow(worksheet, 1, row - indexMerge, row, "center");
                                indexMerge = 0;
                            }
                            index++;
                            row++;
                        }
                        // Ghi giá trị ChatLuong, TuNgay, DenNgay vào các ô D2, D3, D4
                        worksheet.Cells["D2"].Value = obj_request.ChatLuong; // Ghi giá trị ChatLuong vào ô D2
                        worksheet.Cells["D3"].Value = obj_request.TuNgay;    // Ghi giá trị TuNgay vào ô D3
                        worksheet.Cells["D4"].Value = obj_request.DenNgay;   // Ghi giá trị DenNgay vào ô D4

                        // Có thể thêm căn giữa cho các ô này
                        worksheet.Cells["D2:D4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells["D2:D4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        worksheet.Column(7).Style.WrapText = true;
                        // ===========================
                        // Thêm biểu đồ tại đây
                        // ===========================
                        // Tạo biểu đồ cột cho "Sau ủi"
                        var chart = worksheet.Drawings.AddChart("BaoCaoChart", eChartType.ColumnClustered) as ExcelBarChart;
                        chart.Title.Text = "Biểu đồ chất lượng mã hàng";
                        chart.SetPosition(4, 0, 9, 0);  // Đặt vị trí biểu đồ tại cột J
                        chart.SetSize(800, 400);        // Kích thước biểu đồ

                        // Thiết lập dữ liệu cho biểu đồ (từ cột 5 đến cột 8)
                        var rangeLabel = worksheet.Cells[6, 4, row - 1, 4]; // Lấy dữ liệu từ cột Mã hàng (cột 4)
                        var rangeData2 = worksheet.Cells[6, 6, row - 1, 6]; // SauUi (cột 6)

                        // Thêm biểu đồ cột cho dữ liệu "Sau ủi"
                        var series2 = chart.Series.Add(rangeData2, rangeLabel);
                        series2.Header = "Sau ủi";    // Dữ liệu sau ủi (cột 6)

                        // Định dạng thêm cho biểu đồ cột
                        chart.Legend.Position = eLegendPosition.Right;
                        chart.YAxis.Title.Text = "Giá trị";
                        chart.XAxis.Title.Text = "Mã hàng";

                        // ===========================
                        // Thêm biểu đồ đường cho "Trước ủi"
                        // ===========================
                        var lineChart = (ExcelLineChart)chart.PlotArea.ChartTypes.Add(eChartType.Line); // Thêm biểu đồ đường vào biểu đồ cột
                        var rangeData1 = worksheet.Cells[6, 5, row - 1, 5];  // Trước ủi (cột 5)
                        lineChart.Series.Add(rangeData1, rangeLabel);        // Dữ liệu từ cột "Trước ủi" (cột 5)
                        lineChart.Series[0].Header = "Trước ủi";             // Tiêu đề của biểu đồ đường

                        // Định dạng thêm cho biểu đồ đường (nếu cần)
                        lineChart.YAxis.Title.Text = "Giá trị Trước ủi";
                        lineChart.UseSecondaryAxis = true;  // Sử dụng trục Y phụ để biểu đồ đường không ảnh hưởng đến cột

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
                    BaoCaoChatLuong_Request json = JsonConvert.DeserializeObject<BaoCaoChatLuong_Request>(e.Message.Replace("ExportExcel@", ""));
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
        private static void MergeCellsByRow(ExcelWorksheet worksheet, int colum, int startrow, int endrow, string align)
        {
            worksheet.Cells[startrow, colum, endrow, colum].Merge = true;

            using (var range = worksheet.Cells[startrow, colum, endrow, colum])
            {
                if (align == "center")
                {
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                range.Style.WrapText = true;
            }
        }
    }
    public class BaoCaoChatLuongMaHangTrenChuyenViewModel
    {
        public string To { get; set; }
        public string MaLenh { get; set; }
        public string MaHang { get; set; }
        public string TruocUi { get; set; }
        public string SauUi { get; set; }
        public string LoiHuNoiCom { get; set; }
        public string KetQua { get; set; }
    }
    public class BaoCaoChatLuong_Request
    {
        public string ChatLuong { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public List<BaoCaoChatLuongMaHangTrenChuyenViewModel> BaoCaoBC { get; set; }
    }
}
