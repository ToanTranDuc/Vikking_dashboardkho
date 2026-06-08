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
    public partial class frmBaoCaoKHCatNgayWeb : Form
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

        public frmBaoCaoKHCatNgayWeb()
        {
            useNameLogin = GlobleData.UserName;
            urlMain = settingsReader.GetValue("main", typeof(string)).ToString();
            urlWeb = settingsReader.GetValue("web_BCKHCatNgayUrl", typeof(string)).ToString();
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
        public void ExportExcel(ExPosterRequest json)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";

            Sfd.FileName = string.Format("BaoCaoKeHoachCatNgay{0}", DateTime.Now.Millisecond.ToString());

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateBaoCaoKeHoachCatNgay.xlsx";

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
        public void Export(string TemplateFileName, string ExportFileName, ExPosterRequest postData)
        {
            try
            {
                string fileName = $"BaoCaoKeHoachCatNgay";
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
                        package.Workbook.Properties.Title = "Bao-cao-ke-hoach-cat-ngay";

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
                        int row = 6;
                        int index = 0;
                        int slCutValue = 0;
                        int slLKValue = 0;
                        int slKHValue = 0;

                        int indexMerge = 0;
                        List<ArrBodyFile> arrBody = postData.ArrBody;
                        List<ArrBodyDate> copyArrHeader = postData.CopyArrHeader;
                        foreach (var item in arrBody)
                        {
                            string currentMaHang = item.MaHang;
                            string previosMaHang = (index + 1 < arrBody.Count) ? arrBody[index + 1].MaHang : "abcea";
                            worksheet.Cells[row, 2].Value = item.MaHang;
                            worksheet.Cells[row, 3].Value = item.SLCut_TH;
                            worksheet.Cells[row, 4].Value = item.SLCut_LK;
                            worksheet.Cells[row, 5].Value = item.SLKH;
                            worksheet.Cells[row, 1].Value = index + 1;
                            slCutValue += item.SLCut_TH;
                            slLKValue += item.SLCut_LK;
                            slKHValue += item.SLKH;
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
                        worksheet.Cells["D2"].Value = copyArrHeader[0].DenNgay;
                        worksheet.Cells[row, 1, row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 1, row, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                        worksheet.Cells[row, 1].Value = "Tổng";

                        worksheet.Cells[row, 3].Value = slCutValue; // Total CTM in column G
                        worksheet.Cells[row, 4].Value = slLKValue; // Total CKT in column K
                        worksheet.Cells[row, 5].Value = slKHValue; // Total CNRN in column O
                        var borderTotal = worksheet.Cells[row, 1, row, 5].Style.Border;
                        borderTotal.Bottom.Style =
                            borderTotal.Top.Style =
                            borderTotal.Left.Style =
                            borderTotal.Right.Style = ExcelBorderStyle.Thin;
                        var borderData = worksheet.Cells[6, 1, row - 1, 5].Style.Border;
                        borderData.Bottom.Style =
                            borderData.Top.Style =
                            borderData.Left.Style =
                            borderData.Right.Style = ExcelBorderStyle.Thin;
                        // ===========================
                        // Thêm biểu đồ tại đây
                        // ===========================
                        var chart = worksheet.Drawings.AddChart("chart", eChartType.ColumnClustered) as ExcelBarChart;
                        chart.Title.Text = "Biểu đồ sản lượng nhập kho";
                        chart.SetPosition(4, 0, 6, 0); // Đặt vị trí cho biểu đồ dưới bảng
                        chart.SetSize(800, 400);  // Điều chỉnh kích thước biểu đồ

                        // Chọn dữ liệu cho biểu đồ (dùng các cột từ B đến D)
                        var series1 = chart.Series.Add(worksheet.Cells[6, 3, row - 1, 3], worksheet.Cells[6, 2, row - 1, 2]);
                        series1.Header = "Sản lượng cắt "; // Tên cho dữ liệu cột SLCut_TH

                        var series2 = chart.Series.Add(worksheet.Cells[6, 4, row - 1, 4], worksheet.Cells[6, 2, row - 1, 2]);
                        series2.Header = "Lũy kế"; // Tên cho dữ liệu cột SLCut_LK

                        var series3 = chart.Series.Add(worksheet.Cells[6, 5, row - 1, 5], worksheet.Cells[6, 2, row - 1, 2]);
                        series3.Header = "Số lượng kế hoạch"; // Tên cho dữ liệu cột SLKH

                        // ===========================
                        // Kết thúc chèn biểu đồ
                        // ===========================
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
                    ExPosterRequest json = JsonConvert.DeserializeObject<ExPosterRequest>(e.Message.Replace("ExportExcel@", ""));
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
    public class ArrBodyFile
    {
        public string MaHang { get; set; }
        public int SLCut_TH { get; set; }
        public int SLCut_LK { get; set; }
        public DateTime NgayNhapKho { get; set; }
        public int SLKH { get; set; }
        public int slCut { get; set; }
        public int slLK { get; set; }
        public int slKH { get; set; }


    }
    public class ExPosterRequest
    {
        public List<ArrBodyFile> ArrBody { get; set; }
        public List<ArrBodyDate> CopyArrHeader { get; set; }


    }
    public class ArrBodyDate
    {
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }

    }
}
