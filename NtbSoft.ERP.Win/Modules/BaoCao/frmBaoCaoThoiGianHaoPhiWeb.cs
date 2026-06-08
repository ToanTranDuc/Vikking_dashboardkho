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
    public partial class frmBaoCaoThoiGianHaoPhiWeb : Form
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

        public frmBaoCaoThoiGianHaoPhiWeb()
        {
            useNameLogin = GlobleData.UserName;
            urlMain = settingsReader.GetValue("main", typeof(string)).ToString();
            urlWeb = settingsReader.GetValue("web_BCThoiGianHaoPhiUrl", typeof(string)).ToString();
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
        public void ExportExcel(BaoCaoHaoPhi_Request json)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";

            Sfd.FileName = string.Format("BaoCaoThoiGianHaoPhi{0}", DateTime.Now.Millisecond.ToString());

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateBaoCaoThoiGianHaoPhi.xlsx";

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
        public void Export(string TemplateFileName, string ExportFileName, BaoCaoHaoPhi_Request obj_request)
        {
            try
            {
                string fileName = $"BaoCaoThoiGianHaoPhi";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;


                System.IO.FileInfo file = new System.IO.FileInfo(TemplateFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                int index = 0;
                int row = 7;
                int CTM_Tong = 0;
                int CKT_Tong = 0;
                int CNRN_Tong = 0;
                int count_CTM = 0;
                int count_CKT = 0;
                int count_CNRN = 0;
                using (OfficeOpenXml.ExcelPackage package = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {
                        // đặt tên người tạo file
                        package.Workbook.Properties.Author = "Cty NTB";

                        // đặt tiêu đề cho file
                        package.Workbook.Properties.Title = "Bao-cao-thoi-gian-hao-phi";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;
                        /// coppy here
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                        worksheet.Cells["E2"].Value = obj_request.Chuyen;
                        worksheet.Cells["E3"].Value = obj_request.TuNgay;
                        worksheet.Cells["H3"].Value = obj_request.DenNgay;
                        List<BaoCaoThoiGianHaoPhi> Lst_obj = obj_request.DataBC;
                        foreach (BaoCaoThoiGianHaoPhi item in Lst_obj)
                        {
                            string previosMaHang = (index + 1 < Lst_obj.Count) ? Lst_obj[index + 1].Ngay : "abcea";
                            worksheet.Cells[row, 1].Value = index + 1;
                            worksheet.Cells[row, 2].Value = item.Ngay;
                            worksheet.Cells[row, 3].Value = item.Nhom;
                            worksheet.Cells[row, 4].Value = item.CTM_BatDauBatDen;
                            worksheet.Cells[row, 5].Value = item.CTM_BatDauXuLi;
                            worksheet.Cells[row, 6].Value = item.CTM_KetThuc;
                            worksheet.Cells[row, 7].Value = item.CTM_ThoiGianXuLi;
                            worksheet.Cells[row, 8].Value = item.CKT_BatDauBatDen;
                            worksheet.Cells[row, 9].Value = item.CKT_BatDauXuLi;
                            worksheet.Cells[row, 10].Value = item.CKT_KetThuc;
                            worksheet.Cells[row, 11].Value = item.CKT_ThoiGianXuLi;
                            worksheet.Cells[row, 12].Value = item.CNRN_BatDauBatDen;
                            worksheet.Cells[row, 13].Value = item.CNRN_BatDauXuLi;
                            worksheet.Cells[row, 14].Value = item.CNRN_KetThuc;
                            worksheet.Cells[row, 15].Value = item.CNRN_ThoiGianXuLi;

                            CTM_Tong += int.TryParse(item.CTM_ThoiGianXuLi, out int ctmValue) ? ctmValue : 0;
                            CKT_Tong += int.TryParse(item.CKT_ThoiGianXuLi, out int cktValue) ? cktValue : 0;
                            CNRN_Tong += int.TryParse(item.CNRN_ThoiGianXuLi, out int cnrnValue) ? cnrnValue : 0;
                            if (!string.IsNullOrEmpty(item.CTM_ThoiGianXuLi)) count_CTM++;
                            if (!string.IsNullOrEmpty(item.CKT_ThoiGianXuLi)) count_CKT++;
                            if (!string.IsNullOrEmpty(item.CNRN_ThoiGianXuLi)) count_CNRN++;

                            for (int col = 1; col <= 19; col++)
                            {
                                var cell = worksheet.Cells[row, col];
                                cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;



                            }
                            worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            index++;
                            row++;
                        }
                        worksheet.Cells[row, 1, row, 19].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 1, row, 19].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                        worksheet.Cells[row, 1].Value = "Tổng";

                        worksheet.Cells[row, 7].Value = CTM_Tong; // Total CTM in column G
                        worksheet.Cells[row, 11].Value = CKT_Tong; // Total CKT in column K
                        worksheet.Cells[row, 15].Value = CNRN_Tong; // Total CNRN in column O
                        /*                    worksheet.Column(1).AutoFit();
                        *//*                    worksheet.Cells[row, 1].Style.WrapText = true;
                        */
                        for (int col = 1; col <= 19; col++)
                        {
                            var cell = worksheet.Cells[row, col];
                            cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        // Dữ liệu cho Biểu Đồ 1: Số Lượng Vai Trò Cần Thiết
                        // Dữ liệu cho Biểu Đồ 1: Số Lượng Vai Trò Cần Thiết
                        int chartDataRow = row + 2; // Giữ khoảng cách giữa bảng và biểu đồ 1
                        worksheet.Cells[chartDataRow, 1].Value = "Cần Thợ Máy";
                        worksheet.Cells[chartDataRow, 2].Value = count_CTM;

                        worksheet.Cells[chartDataRow + 1, 1].Value = "Cần Tổ Trưởng";
                        worksheet.Cells[chartDataRow + 1, 2].Value = count_CKT;

                        worksheet.Cells[chartDataRow + 2, 1].Value = "Cần Kỹ Thuật";
                        worksheet.Cells[chartDataRow + 2, 2].Value = count_CNRN;

                        // Biểu Đồ Hình Tròn Đầu Tiên cho Số Lượng Vai Trò
                        var pieChart1 = worksheet.Drawings.AddChart("PieChart1", eChartType.Pie);
                        pieChart1.Title.Text = "Biểu Đồ Số Lần Bấm Đèn";
                        pieChart1.SetPosition(chartDataRow, 0, 3, 0); // Vị trí của biểu đồ 1
                        pieChart1.SetSize(500, 400);
                        var series1 = pieChart1.Series.Add(worksheet.Cells[chartDataRow, 2, chartDataRow + 2, 2], worksheet.Cells[chartDataRow, 1, chartDataRow + 2, 1]);
                        series1.Header = "Số lần xử lý";

                        int newChartDataRow = chartDataRow; // Đặt hàng cho biểu đồ 2 ở cùng hàng
                        worksheet.Cells[newChartDataRow, 11].Value = "Cần Thợ Máy"; // Di chuyển cột dữ liệu sang bên phải 3 ô
                        worksheet.Cells[newChartDataRow, 12].Value = CTM_Tong;

                        worksheet.Cells[newChartDataRow + 1, 11].Value = "Cần Tổ Trưởng";
                        worksheet.Cells[newChartDataRow + 1, 12].Value = CKT_Tong;

                        worksheet.Cells[newChartDataRow + 2, 11].Value = "Cần Kỹ Thuật";
                        worksheet.Cells[newChartDataRow + 2, 12].Value = CNRN_Tong;

                        // Biểu Đồ Hình Tròn Thứ Hai cho Tổng Thời Gian Đã Dùng
                        var pieChart2 = worksheet.Drawings.AddChart("PieChart2", eChartType.Pie);
                        pieChart2.Title.Text = "Biểu Đồ Thời Gian Hao Phí";
                        pieChart2.SetPosition(chartDataRow, 0, 13, 0); // Di chuyển biểu đồ 2 sang phải 3 ô
                        pieChart2.SetSize(500, 400);
                        var series2 = pieChart2.Series.Add(worksheet.Cells[newChartDataRow, 12, newChartDataRow + 2, 12], worksheet.Cells[newChartDataRow, 8, newChartDataRow + 2, 8]);
                        series2.Header = "Tổng thời gian xử lý";
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
                    BaoCaoHaoPhi_Request json = JsonConvert.DeserializeObject<BaoCaoHaoPhi_Request>(e.Message.Replace("ExportExcel@", ""));
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
    public class BaoCaoThoiGianHaoPhi
    {
        public string STT { get; set; }
        public string Ngay { get; set; }
        public string Nhom { get; set; }
        public string CTM_BatDauBatDen { get; set; }
        public string CTM_BatDauXuLi { get; set; }
        public string CTM_KetThuc { get; set; }
        public string CTM_ThoiGianXuLi { get; set; }

        public string CKT_BatDauBatDen { get; set; }
        public string CKT_BatDauXuLi { get; set; }
        public string CKT_KetThuc { get; set; }
        public string CKT_ThoiGianXuLi { get; set; }

        public string CNRN_BatDauBatDen { get; set; }
        public string CNRN_BatDauXuLi { get; set; }
        public string CNRN_KetThuc { get; set; }
        public string CNRN_ThoiGianXuLi { get; set; }
    }
    public class BaoCaoHaoPhi_Request
    {
        public string Chuyen { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public List<BaoCaoThoiGianHaoPhi> DataBC { get; set; }
    }
}
