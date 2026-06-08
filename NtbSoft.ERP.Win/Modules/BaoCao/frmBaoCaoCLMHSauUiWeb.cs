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
    public partial class frmBaoCaoCLMHSauUiWeb : Form
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

        public frmBaoCaoCLMHSauUiWeb()
        {
            useNameLogin = GlobleData.UserName;
            urlMain = settingsReader.GetValue("main", typeof(string)).ToString();
            urlWeb = settingsReader.GetValue("web_BCCLMHSauUiUrl", typeof(string)).ToString();
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
        public void ExportExcel(BaoCaoSauUi_Request json)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";

            Sfd.FileName = string.Format("BaoCaoChatLuongMaHangSauUi{0}", DateTime.Now.Millisecond.ToString());

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateBaoCaoChatLuongMaHangSauUi.xlsx";

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
        public void Export(string TemplateFileName, string ExportFileName, BaoCaoSauUi_Request postData)
        {
            try
            {
                string fileName = $"BaoCaoChatLuongMaHangSauUi";
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
                        package.Workbook.Properties.Title = "Bao-cao-chat-luong-ma-hang-sau-ui";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;
                        /// coppy here
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                        worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        worksheet.Cells.Style.Font.Name = "Times New Roman";
                        ExcelRange range = worksheet.Cells;


                        worksheet.Cells["D3"].Value = postData.FromDate;

                        worksheet.Cells["G3"].Value = postData.ToDate;

                        worksheet.Cells["D2"].Value = postData.Chuyen;



                        int row = 7;
                        int index = 0;
                        int indexMerge = 0;

                        List<BaoCaoSauUi> arrBody = postData.DataBC;
                        for (int i = 0; i < arrBody.Count; i++)
                        {
                            if (string.IsNullOrEmpty(arrBody[i].MaHang.ToString()) && string.IsNullOrEmpty(arrBody[i].NguoiKiemTra.ToString()))
                            {
                                arrBody[i].MaHang = arrBody[i - 1].MaHang;
                                arrBody[i].NguoiKiemTra = arrBody[i - 1].NguoiKiemTra;
                            }
                        }
                        foreach (var item in arrBody)
                        {
                            string currentMaHang = item.MaHang;
                            string previosMaHang = (index + 1 < arrBody.Count) ? arrBody[index + 1].MaHang : "";

                            // Ghi dữ liệu vào các ô tương ứng
                            worksheet.Cells[row, 1].Value = item.MaHang ?? "";
                            worksheet.Cells[row, 2].Value = item.NgayKiemDisplay;
                            worksheet.Cells[row, 3].Value = double.TryParse(item.TongKiem, out double tongKiemValue) ? tongKiemValue : 0;
                            worksheet.Cells[row, 4].Value = double.TryParse(item.SPDAT, out double spdatValue) ? spdatValue : 0;
                            worksheet.Cells[row, 5].Value = item.Loi1;
                            worksheet.Cells[row, 6].Value = item.Loi2;
                            worksheet.Cells[row, 7].Value = item.Loi3;
                            worksheet.Cells[row, 8].Value = item.Loi4;
                            worksheet.Cells[row, 9].Value = item.Loi5;
                            worksheet.Cells[row, 10].Value = item.Loi6;
                            worksheet.Cells[row, 11].Value = item.Loi7;
                            worksheet.Cells[row, 12].Value = item.Loi8;
                            worksheet.Cells[row, 13].Value = item.Loi9;
                            worksheet.Cells[row, 14].Value = item.Loi10;
                            worksheet.Cells[row, 15].Value = item.Loi11;
                            worksheet.Cells[row, 16].Value = item.Loi12;
                            worksheet.Cells[row, 17].Value = double.TryParse(item.TyLeDat, out double tyLeDatValue) ? tyLeDatValue : 0;
                            worksheet.Cells[row, 18].Value = double.TryParse(item.TiLeLoi, out double tiLeLoiValue) ? tiLeLoiValue : 0;
                            worksheet.Cells[row, 19].Value = item.NguoiKiemTra;
                            worksheet.Cells[row, 20].Value = item.GhiNhanLoi ?? "";

                            range = worksheet.Cells[row, 4]; range.Value = item.SPDAT; range.Style.Font.Bold = true;
                            range.Style.Font.Bold = true;

                            // Kiểm tra nếu currentMaHang bằng previosMaHang hoặc nếu currentMaHang là null
                            if (currentMaHang == previosMaHang)
                            {
                                indexMerge++;
                            }
                            else
                            {

                                MergeCellsByRow(worksheet, 1, row - indexMerge, row, "center");
                                MergeCellsByRow(worksheet, 19, row - indexMerge, row, "center");
                                indexMerge = 0;
                            }

                            index++;
                            row++;
                        }
                        worksheet.Column(1).AutoFit();
                        var borderData = worksheet.Cells[7, 1, row - 1, 20].Style.Border;
                        borderData.Bottom.Style =
                            borderData.Top.Style =
                            borderData.Left.Style =
                            borderData.Right.Style = ExcelBorderStyle.Thin;


                        for (int i = 7; i < row; i++)
                        {
                            worksheet.Cells[i, 4].Value = double.TryParse(worksheet.Cells[i, 4].Text, out double spdatValue) ? spdatValue : 0;
                            worksheet.Cells[i, 4].Style.Numberformat.Format = "0";
                        }

                        double maxTongKiem = arrBody.Max(item => double.TryParse(item.TongKiem.ToString(), out double value) ? value : 0);
                        // ===========================
                        // ===========================
                        // Thêm biểu đồ tại đây
                        // ===========================
                        var chart = worksheet.Drawings.AddChart("chart", eChartType.ColumnClustered) as ExcelBarChart;
                        chart.Title.Text = "Biểu đồ chất lượng số lượng mã hàng sau ủi";
                        chart.SetPosition(row, 0, 2, 0);
                        chart.SetSize(1000, 700);

                        // Dữ liệu cho biểu đồ
                        var TongKiem = chart.Series.Add(worksheet.Cells[7, 3, row - 1, 3], worksheet.Cells[7, 2, row - 1, 2]); // Cột TongKiem
                        TongKiem.Header = "Tổng Kiểm";

                        var SPDat = chart.Series.Add(worksheet.Cells[7, 4, row - 1, 4], worksheet.Cells[7, 2, row - 1, 2]); // Cột SPDAT
                        SPDat.Header = "Sản phẩm Đạt";


                        // Cấu hình biểu đồ
                        chart.Legend.Position = eLegendPosition.Bottom;
                        chart.YAxis.MaxValue = maxTongKiem;
                        chart.YAxis.MinValue = 0;
                        chart.DataLabel.ShowValue = true;

                        // Thêm biểu đồ tại đây
                        // ===========================
                        var chartTyLe = worksheet.Drawings.AddChart("chartTyLe", eChartType.ColumnClustered) as ExcelBarChart;
                        chartTyLe.Title.Text = "Biểu đồ tỷ lệ chất lượng mã hàng sau ủi";
                        chartTyLe.SetPosition(row + 26, 0, 2, 0);
                        chartTyLe.SetSize(1000, 700);

                        var TyLeDat = chartTyLe.Series.Add(worksheet.Cells[7, 17, row - 1, 17], worksheet.Cells[7, 2, row - 1, 2]); // Cột TyLeDat
                        TyLeDat.Header = "Tỷ lệ Đạt";

                        var TyLeLoi = chartTyLe.Series.Add(worksheet.Cells[7, 18, row - 1, 18], worksheet.Cells[7, 2, row - 1, 2]); // Cột TiLeLoi
                        TyLeLoi.Header = "Tỷ lệ Lỗi";

                        // Cấu hình biểu đồ
                        chartTyLe.Legend.Position = eLegendPosition.Bottom;
                        chartTyLe.YAxis.MaxValue = 100;
                        chartTyLe.YAxis.MinValue = 0;
                        chartTyLe.DataLabel.ShowValue = true;

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
                    BaoCaoSauUi_Request json = JsonConvert.DeserializeObject<BaoCaoSauUi_Request>(e.Message.Replace("ExportExcel@", ""));
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
        private static void MergeCellsByRow(ExcelWorksheet worksheet, int column, int startRow, int endRow, string align)
        {
            worksheet.Cells[startRow, column, endRow, column].Merge = true;

            using (var range = worksheet.Cells[startRow, column, endRow, column])
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
    public class BaoCaoSauUi
    {
        public string MaHang { get; set; }
        public string NgayKiemDisplay { get; set; }
        public string TongKiem { get; set; }
        public string SPDAT { get; set; }
        public string NguoiKiemTra { get; set; }
        public string GhiNhanLoi { get; set; }
        public string TyLeDat { get; set; }
        public string TiLeLoi { get; set; }

        public string Loi1 { get; set; }
        public string Loi2 { get; set; }
        public string Loi3 { get; set; }
        public string Loi4 { get; set; }
        public string Loi5 { get; set; }
        public string Loi6 { get; set; }
        public string Loi7 { get; set; }
        public string Loi8 { get; set; }
        public string Loi9 { get; set; }
        public string Loi10 { get; set; }
        public string Loi11 { get; set; }
        public string Loi12 { get; set; }

    }

    public class BaoCaoSauUi_Request
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Chuyen { get; set; }
        public List<BaoCaoSauUi> DataBC { get; set; }
    }
}
