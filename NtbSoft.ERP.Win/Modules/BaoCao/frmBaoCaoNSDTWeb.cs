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
    public partial class frmBaoCaoNSDTWeb : Form
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

        public frmBaoCaoNSDTWeb()
        {
            useNameLogin = GlobleData.UserName;
            urlMain = settingsReader.GetValue("main", typeof(string)).ToString();
            urlWeb = settingsReader.GetValue("web_BCNSDTUrl", typeof(string)).ToString();
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
        public void ExportExcel(DataReponse json)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";

            Sfd.FileName = string.Format("BaoCaoNangSuatDoanhThu_{0}", DateTime.Now.Millisecond.ToString());

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateBaoCaoNangSuatDoanhThu.xlsx";

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
        public void Export(string TemplateFileName, string ExportFileName, DataReponse obj)
        {
            try
            {
                string fileName = $"BaoCaoNangSuatDoanhThu";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;


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
                        package.Workbook.Properties.Title = "Bao-cao-nang-suat-doanh-thu";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;
                        /// coppy here
                        List<BaoCaoNangXuat_DoanhThu_ViewModel> lst_obj = obj.DataBC;
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                        // Đặt giá trị và định dạng cho ô D3
                        worksheet.Cells["D3"].Value = DateTime.TryParse(obj.TuNgay, out DateTime tuNgay) ? tuNgay : (DateTime?)null;
                        worksheet.Cells["D3"].Style.Numberformat.Format = "dd/MM/yyyy"; // Định dạng ngày tháng năm

                        // Đặt giá trị và định dạng cho ô D4
                        worksheet.Cells["D4"].Value = DateTime.TryParse(obj.DenNgay, out DateTime denNgay) ? denNgay : (DateTime?)null;
                        worksheet.Cells["D4"].Style.Numberformat.Format = "dd/MM/yyyy"; // Định dạng ngày tháng

                        int row = 7;
                        int index = 0;
                        double totalDT_LK = 0;
                        int count = 0;
                        double maxDT_LK_LessThanZero = double.MinValue;

                        foreach (BaoCaoNangXuat_DoanhThu_ViewModel item in lst_obj)
                        {
                            worksheet.Cells[row, 1].Value = item.Chuyen;
                            worksheet.Cells[row, 2].Value = double.TryParse(item.SLDat, out double slDatValue) ? slDatValue : 0; ;
                            worksheet.Cells[row, 3].Value = double.TryParse(item.SLLoi.ToString(), out double slLoiValue) ? slLoiValue : 0;
                            worksheet.Cells[row, 4].Value = double.TryParse(item.TiLeLoi.ToString(), out double tiLeLoiValue) ? tiLeLoiValue : 0;
                            worksheet.Cells[row, 5].Value = double.TryParse(item.Ui_TH.ToString(), out double uiTHValue) ? uiTHValue : 0;
                            worksheet.Cells[row, 6].Value = double.TryParse(item.Ui_LK.ToString(), out double uiLKValue) ? uiLKValue : 0;
                            worksheet.Cells[row, 7].Value = double.TryParse(item.Ui_UUi.ToString(), out double uiUUiValue) ? uiUUiValue : 0;
                            worksheet.Cells[row, 8].Value = double.TryParse(item.DG_TH.ToString(), out double dgTHValue) ? dgTHValue : 0;
                            worksheet.Cells[row, 9].Value = double.TryParse(item.DG_LK.ToString(), out double dgLKValue) ? dgLKValue : 0;
                            worksheet.Cells[row, 10].Value = double.TryParse(item.DG_TonTPHT.ToString(), out double dgTonTPHTValue) ? dgTonTPHTValue : 0;
                            worksheet.Cells[row, 11].Value = double.TryParse(item.DT_TH.ToString(), out double dtTHValue) ? dtTHValue : 0;
                            worksheet.Cells[row, 12].Value = double.TryParse(item.DT_LK.ToString(), out double dtLKValue) ? dtLKValue : 0;

                            if (dtLKValue > 0)
                            {
                                totalDT_LK += dtLKValue;
                                count++;
                            }
                            if (dtLKValue < 0 && dtLKValue > maxDT_LK_LessThanZero)
                            {
                                maxDT_LK_LessThanZero = dtLKValue;
                            }

                            index++;
                            row++;
                        }

                        worksheet.Column(1).AutoFit();
                        var borderData = worksheet.Cells[7, 1, row - 1, 12].Style.Border;
                        borderData.Bottom.Style =
                            borderData.Top.Style =
                            borderData.Left.Style =
                            borderData.Right.Style = ExcelBorderStyle.Thin;


                        double averageDT_LK = count > 0 ? totalDT_LK / count : 0;
                        double maxTiLeLoi = lst_obj.Max(item => double.TryParse(item.TiLeLoi.ToString(), out double value) ? value : 0);
                        double minDT_LK = lst_obj.Min(item => double.TryParse(item.DT_LK.ToString(), out double value) ? value : 0);
                        double maxDT_LK = lst_obj.Max(item => double.TryParse(item.DT_LK.ToString(), out double value) ? value : 0);

                        // ===========================
                        // Thêm biểu đồ tại đây
                        // ===========================
                        var chart1 = worksheet.Drawings.AddChart("chart1", eChartType.ColumnClustered) as ExcelBarChart;
                        chart1.Title.Text = "Biểu đồ tỉ lệ lỗi";
                        chart1.SetPosition(row, 0, 0, 0);
                        chart1.SetSize(600, 400);
                        // Dữ liệu cho biểu đồ 1
                        var series1 = chart1.Series.Add(worksheet.Cells[7, 4, row - 1, 4], worksheet.Cells[7, 1, row - 1, 1]); // Cột TongKiem
                        series1.Header = "Tỉ lệ lỗi";

                        // Cấu hình biểu đồ 1
                        chart1.Legend.Position = eLegendPosition.Bottom;
                        chart1.YAxis.MaxValue = maxTiLeLoi;
                        chart1.YAxis.MinValue = 0;
                        chart1.DataLabel.ShowValue = true;

                        // ===========================
                        // Thêm biểu đồ mới cho DT_LK
                        // ===========================
                        var chart2 = worksheet.Drawings.AddChart("chart2", eChartType.ColumnClustered) as ExcelBarChart;
                        chart2.Title.Text = "Biểu đồ doanh thu lũy kế";
                        chart2.SetPosition(row, 0, 6, 0);
                        chart2.SetSize(600, 400);

                        var series2 = chart2.Series.Add(worksheet.Cells[7, 12, row - 1, 12], worksheet.Cells[7, 1, row - 1, 1]);
                        series2.Header = "DT_LK";

                        series2.DataLabel.ShowValue = true;
                        //series2.DataLabel.Position = eLabelPosition.Bottom;

                        chart2.Legend.Position = eLegendPosition.Bottom;
                        chart2.YAxis.MaxValue = maxDT_LK;
                        if (minDT_LK < 0)
                        {
                            chart2.YAxis.MinValue = minDT_LK + (averageDT_LK + maxDT_LK_LessThanZero);
                        }
                        else
                        {
                            chart2.YAxis.MinValue = minDT_LK;
                        }
                        chart2.DataLabel.ShowValue = true;

                        chart2.XAxis.TickLabelPosition = eTickLabelPosition.Low;
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
                    int line = e.Line;
                    DataReponse json = JsonConvert.DeserializeObject<DataReponse>(e.Message.Replace("ExportExcel@", ""));
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
    }
    public class DataReponse
    {
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public List<BaoCaoNangXuat_DoanhThu_ViewModel> DataBC { get; set; }
    }
    public class BaoCaoNangXuat_DoanhThu_ViewModel
    {
        public string Chuyen { get; set; }
        public string SLDat { get; set; }
        public string SLLoi { get; set; }
        public string TiLeLoi { get; set; }
        public string Ui_TH { get; set; }
        public string Ui_LK { get; set; }
        public string Ui_UUi { get; set; }
        public string DG_TH { get; set; }
        public string DG_LK { get; set; }
        public string DG_TonTPHT { get; set; }
        public string DT_TH { get; set; }
        public string DT_LK { get; set; }
    }
}
