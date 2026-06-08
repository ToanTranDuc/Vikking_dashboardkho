using CefSharp;
using CefSharp.Handler;
using CefSharp.WinForms;
using Newtonsoft.Json;
using NtbSoft.ERP.Web.Models;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.BaoCao
{
    public partial class frmBaoCaoTongQuanSXWeb : Form
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

        public frmBaoCaoTongQuanSXWeb()
        {
            useNameLogin = GlobleData.UserName;
            urlMain = settingsReader.GetValue("main", typeof(string)).ToString();
            urlWeb = settingsReader.GetValue("web_BCTongQuanUrl", typeof(string)).ToString();
            InitializeComponent();

            LoadQuyenTruyCapCacMayChu();
        }
        private void LoadQuyenTruyCapCacMayChu()
        {
            try
            {
                cbxChooseDVSX.Properties.Items.Add("Xuất báo cáo tổng quan");
                cbxChooseDVSX_Bar.EditValue = "Xuất báo cáo tổng quan";
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
                    string ngayBaoCao = e.Message.Split('☻')[1];
                    CTDHResult result = JsonConvert.DeserializeObject<CTDHResult>(e.Message.Split('☻')[2]);
                    ExportExcel(ngayBaoCao, result);
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
        public void ExportExcel(string ngayBaoCao, CTDHResult result)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";

            Sfd.FileName = string.Format("BaoCaoTienDoTongHopCacKhu_{0}", DateTime.Now.Millisecond.ToString());

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateBaoCaoTienDoTongHopCacKhu.xlsx";

                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;

                Export(TemplateFileName, ExportFileName, ngayBaoCao, result);

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
        public void Export(string TemplateFileName, string ExportFileName, string ngayBaoCao, CTDHResult resultDB)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime ngayBaoCaoDate = DateTime.ParseExact(ngayBaoCao, "yyyy-MM-dd", provider);
                DataTable dtTanHuong1 = resultDB.dt;
                DataTable dtTanHuong2 = resultDB.dt2;
                DataTable dtChoGao = resultDB.dt3;
                DataTable dtCaiLay = resultDB.dt4;


                string fileName = $"TemplatesBaoCaoTienDoTongHopCacKhu";
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
                        package.Workbook.Properties.Title = "Bao-cao-tong-quan-san-xuat-cac-khu";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Report"];
                        worksheet.Cells["B2"].Value = ngayBaoCao.Split('-')[2] + "/" + ngayBaoCao.Split('-')[1] + "/" + ngayBaoCao.Split('-')[0];
                        worksheet.Cells["B2"].Style.Numberformat.Format = "dd/MM/yyyy";
                        worksheet.Cells["B2"].Style.Font.Bold = true;
                        worksheet.Cells["B2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Optional: align center

                        worksheet.Cells["A7"].Value = "NM TH 1";
                        worksheet.Cells["A7"].Style.Font.Bold = true;
                        worksheet.Cells["A7"].Style.Font.Color.SetColor(System.Drawing.Color.Blue);

                        if (/*dtTanHuong1 != null && dtTanHuong2 != null && dtChoGao != null && dtCaiLay != null*/1 == 1)
                        {
                            int startRowTanHuong1 = 8;
                            int startCol = 1;
                            double totalDTChenhLech = 0;
                            double totalLaoDong1 = dtTanHuong1.AsEnumerable().Sum(row => Convert.ToDouble(row["LaoDong"]));
                            double totalDTMucTieu = 0;
                            double totalMucTieu = 0;
                            double totalLDCoMat1 = dtTanHuong1.AsEnumerable().Sum(row => Convert.ToDouble(row["LDCoMat"]));
                            double totalRC_Ngay1 = dtTanHuong1.AsEnumerable().Sum(row => Convert.ToDouble(row["RC_Ngay"]));
                            var yellowColor = System.Drawing.Color.Yellow;
                            double totalRC_DonGiaSum = 0;
                            if (dtTanHuong1.Rows.Count > 0)
                                foreach (DataRow row1 in dtTanHuong1.Rows)
                                {
                                    worksheet.Cells[startRowTanHuong1, startCol].Value = row1["Name"].ToString();
                                    worksheet.Cells[startRowTanHuong1, startCol + 1].Value = row1["LaoDong"].ToString();
                                    worksheet.Cells[startRowTanHuong1, startCol + 2].Value = row1["LDCoMat"].ToString();
                                    worksheet.Cells[startRowTanHuong1, startCol + 3].Value = row1["MaHang"].ToString();
                                    worksheet.Cells[startRowTanHuong1, startCol + 4].Value = row1["KhachHang"].ToString();
                                    worksheet.Cells[startRowTanHuong1, startCol + 5].Value = row1["TenHang"].ToString();
                                    worksheet.Cells[startRowTanHuong1, startCol + 6].Value = row1["SLKH"].ToString();
                                    DateTime ngayGH1 = Convert.ToDateTime(row1["NgayGH"]); // Chuyển đổi sang kiểu DateTime
                                    worksheet.Cells[startRowTanHuong1, startCol + 7].Value = ngayGH1.ToString("dd/MM/yyyy"); worksheet.Cells[startRowTanHuong1, startCol + 8].Value = row1["RC_Ngay"].ToString();
                                    worksheet.Cells[startRowTanHuong1, startCol + 9].Value = row1["DonGia"].ToString();
                                    var rcNgay = Convert.ToDouble(row1["RC_Ngay"]);
                                    var donGia = Convert.ToDouble(row1["DonGia"]);
                                    var totalRC_DonGia = rcNgay * donGia;
                                    worksheet.Cells[startRowTanHuong1, startCol + 10].Value = totalRC_DonGia;
                                    var ldCoMat = Convert.ToDouble(row1["LDCoMat"]);
                                    var result = ldCoMat != 0 ? totalRC_DonGia / ldCoMat : 0;
                                    worksheet.Cells[startRowTanHuong1, startCol + 11].Value = result;
                                    worksheet.Cells[startRowTanHuong1, startCol + 12].Value = row1["MucTieu"].ToString();
                                    worksheet.Cells[startRowTanHuong1, startCol + 13].Value = row1["DT_MucTieu"].ToString();
                                    worksheet.Cells[startRowTanHuong1, startCol + 14].Value = row1["DT_ChenhLech"].ToString();
                                    worksheet.Cells[startRowTanHuong1, startCol + 15].Value = row1["RC_LK"].ToString();
                                    var slkh = Convert.ToDouble(row1["SLKH"]);
                                    var rcLK = Convert.ToDouble(row1["RC_LK"]);
                                    worksheet.Cells[startRowTanHuong1, startCol + 16].Value = slkh - rcLK;
                                    worksheet.Cells[startRowTanHuong1, startCol + 17].Value = row1["SoNgayConLai"].ToString();
                                    int soNgayConLai = Convert.ToInt32(row1["SoNgayConLai"]);
                                    DateTime ngayKetQua = ngayBaoCaoDate.AddDays(+soNgayConLai);
                                    worksheet.Cells[startRowTanHuong1, startCol + 18].Value = ngayKetQua.ToString("dd/MM/yyyy");
                                    // Giả sử row1["NgayGH"] chứa chuỗi ngày
                                    DateTime ngayGH = DateTime.Parse(row1["NgayGH"].ToString()).Date;

                                    TimeSpan difference = ngayGH - ngayKetQua;
                                    int daysDifference = (int)difference.TotalDays;
                                    worksheet.Cells[startRowTanHuong1, startCol + 19].Value = daysDifference;
                                    worksheet.Cells[startRowTanHuong1, startCol + 20].Value = "";
                                    totalRC_DonGiaSum += totalRC_DonGia;
                                    var dtMucTieu = Convert.ToDouble(row1["DT_MucTieu"]);
                                    totalDTMucTieu += dtMucTieu;
                                    var mucTieu = Convert.ToDouble(row1["MucTieu"]);
                                    totalMucTieu += mucTieu;
                                    var dtChenhLech = Convert.ToDouble(row1["DT_ChenhLech"]);
                                    totalDTChenhLech += dtChenhLech;
                                    startRowTanHuong1++;
                                }
                            // Total row
                            double tongTienLDNgay = totalLDCoMat1 != 0 ? totalRC_DonGiaSum / totalLDCoMat1 : 0;
                            worksheet.Cells[startRowTanHuong1, startCol].Value = "Total";
                            worksheet.Cells[startRowTanHuong1, startCol + 1].Value = totalLaoDong1;
                            worksheet.Cells[startRowTanHuong1, startCol + 2].Value = totalLDCoMat1;
                            worksheet.Cells[startRowTanHuong1, startCol + 8].Value = totalRC_Ngay1;
                            worksheet.Cells[startRowTanHuong1, startCol + 10].Value = totalRC_DonGiaSum;
                            worksheet.Cells[startRowTanHuong1, startCol + 12].Value = totalMucTieu;
                            worksheet.Cells[startRowTanHuong1, startCol + 13].Value = totalDTMucTieu;
                            worksheet.Cells[startRowTanHuong1, startCol + 11].Formula = string.Format(@"IF(C{0} = 0, 0, ROUND(K{0}/C{0}, 2))", startRowTanHuong1);
                            worksheet.Cells[startRowTanHuong1, startCol + 11].Style.Numberformat.Format = "0.00";
                            worksheet.Cells[startRowTanHuong1, startCol + 14].Value = totalDTChenhLech;
                            worksheet.Cells[startRowTanHuong1, startCol + 20].Formula = string.Format(@"IF(N{0} = 0, 0, (K{0}/N{0}))", startRowTanHuong1);//totalDTMucTieu3 != 0 ? "" * 100 : 0;
                            worksheet.Cells[startRowTanHuong1, startCol + 20].Style.Numberformat.Format = "0.00%"; // Định dạng phần trăm
                            using (var range = worksheet.Cells[startRowTanHuong1, startCol, startRowTanHuong1, startCol + 20])
                            {
                                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(yellowColor);
                            }
                            var endRowTanHuong1 = startRowTanHuong1 - 0; // last row filled for NM TH 1
                            var borderRangeTanHuong1 = worksheet.Cells[8, 1, endRowTanHuong1, 21]; // adjust 21 if you have more columns
                            borderRangeTanHuong1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            borderRangeTanHuong1.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            borderRangeTanHuong1.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            borderRangeTanHuong1.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            startRowTanHuong1++;
                            startRowTanHuong1++;
                            worksheet.Cells[startRowTanHuong1, 1].Value = "NM TH 2";
                            worksheet.Cells[startRowTanHuong1, 1].Style.Font.Bold = true;
                            worksheet.Cells[startRowTanHuong1, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);

                            int startRowTanHuong2 = startRowTanHuong1 + 1;
                            double totalRC_DonGiaSum2 = 0;
                            double totalDTMucTieu2 = 0;
                            double totalMucTieu2 = 0;
                            double totalDTChenhLech2 = 0;
                            double totalLaoDong2 = dtTanHuong2.AsEnumerable().Sum(row => Convert.ToDouble(row["LaoDong"]));
                            double totalLDCoMat2 = dtTanHuong2.AsEnumerable().Sum(row => Convert.ToDouble(row["LDCoMat"]));
                            double totalRC_Ngay2 = dtTanHuong2.AsEnumerable().Sum(row => Convert.ToDouble(row["RC_Ngay"]));
                            if (dtTanHuong2.Rows.Count > 0)
                                foreach (DataRow row in dtTanHuong2.Rows) // Use consistent variable name
                                {
                                    worksheet.Cells[startRowTanHuong2, startCol].Value = row["Name"].ToString(); // Assuming "Name" exists
                                    worksheet.Cells[startRowTanHuong2, startCol + 1].Value = row["LaoDong"].ToString();
                                    worksheet.Cells[startRowTanHuong2, startCol + 2].Value = row["LDCoMat"].ToString();
                                    worksheet.Cells[startRowTanHuong2, startCol + 3].Value = row["MaHang"].ToString();
                                    worksheet.Cells[startRowTanHuong2, startCol + 4].Value = row["KhachHang"].ToString();
                                    worksheet.Cells[startRowTanHuong2, startCol + 5].Value = row["TenHang"].ToString();
                                    worksheet.Cells[startRowTanHuong2, startCol + 6].Value = row["SLKH"].ToString();
                                    DateTime ngayGH1 = Convert.ToDateTime(row["NgayGH"]); // Chuyển đổi sang kiểu DateTime
                                    worksheet.Cells[startRowTanHuong2, startCol + 7].Value = ngayGH1.ToString("dd/MM/yyyy");
                                    worksheet.Cells[startRowTanHuong2, startCol + 8].Value = row["RC_Ngay"].ToString();
                                    worksheet.Cells[startRowTanHuong2, startCol + 9].Value = row["DonGia"].ToString();
                                    var rcNgay = Convert.ToDouble(row["RC_Ngay"]);
                                    var donGia = Convert.ToDouble(row["DonGia"]);
                                    var totalRC_DonGia = rcNgay * donGia;
                                    worksheet.Cells[startRowTanHuong2, startCol + 10].Value = totalRC_DonGia;
                                    var ldCoMat = Convert.ToDouble(row["LDCoMat"]);
                                    var result = ldCoMat != 0 ? totalRC_DonGia / ldCoMat : 0; // Avoid division by zero
                                    worksheet.Cells[startRowTanHuong2, startCol + 11].Value = result;
                                    worksheet.Cells[startRowTanHuong2, startCol + 12].Value = row["MucTieu"].ToString();
                                    worksheet.Cells[startRowTanHuong2, startCol + 13].Value = row["DT_MucTieu"].ToString();
                                    worksheet.Cells[startRowTanHuong2, startCol + 14].Value = row["DT_ChenhLech"].ToString();
                                    worksheet.Cells[startRowTanHuong2, startCol + 15].Value = row["RC_LK"].ToString();
                                    var slkh = Convert.ToDouble(row["SLKH"]);
                                    var rcLK = Convert.ToDouble(row["RC_LK"]);
                                    worksheet.Cells[startRowTanHuong2, startCol + 16].Value = slkh - rcLK;
                                    worksheet.Cells[startRowTanHuong2, startCol + 17].Value = row["SoNgayConLai"].ToString();
                                    int soNgayConLai = Convert.ToInt32(row["SoNgayConLai"]);
                                    DateTime ngayKetQua = ngayBaoCaoDate.AddDays(+soNgayConLai);
                                    worksheet.Cells[startRowTanHuong2, startCol + 18].Value = ngayKetQua.ToString("dd/MM/yyyy");
                                    DateTime ngayGH = DateTime.Parse(row["NgayGH"].ToString()).Date;

                                    TimeSpan difference = ngayGH - ngayKetQua;
                                    int daysDifference = (int)difference.TotalDays;
                                    worksheet.Cells[startRowTanHuong2, startCol + 19].Value = daysDifference;
                                    worksheet.Cells[startRowTanHuong2, startCol + 20].Value = "";
                                    totalRC_DonGiaSum2 += totalRC_DonGia;
                                    var dtMucTieu = Convert.ToDouble(row["DT_MucTieu"]);
                                    totalDTMucTieu2 += dtMucTieu;
                                    var mucTieu = Convert.ToDouble(row["MucTieu"]);
                                    totalMucTieu2 += mucTieu;
                                    var dtChenhLech = Convert.ToDouble(row["DT_ChenhLech"]);
                                    totalDTChenhLech2 += dtChenhLech;
                                    startRowTanHuong2++;
                                }
                            double tongTienLDNgay2 = totalLDCoMat2 != 0 ? totalRC_DonGiaSum / totalLDCoMat2 : 0;
                            worksheet.Cells[startRowTanHuong2, startCol].Value = "Total";
                            worksheet.Cells[startRowTanHuong2, startCol + 1].Value = totalLaoDong2;
                            worksheet.Cells[startRowTanHuong2, startCol + 2].Value = totalLDCoMat2;
                            worksheet.Cells[startRowTanHuong2, startCol + 8].Value = totalRC_Ngay2;
                            worksheet.Cells[startRowTanHuong2, startCol + 10].Value = totalRC_DonGiaSum2;
                            worksheet.Cells[startRowTanHuong2, startCol + 12].Value = totalMucTieu2;
                            worksheet.Cells[startRowTanHuong2, startCol + 13].Value = totalDTMucTieu2;
                            worksheet.Cells[startRowTanHuong2, startCol + 11].Formula = string.Format(@"IF(C{0} = 0, 0, ROUND(K{0}/C{0}, 2))", startRowTanHuong2);
                            worksheet.Cells[startRowTanHuong2, startCol + 11].Style.Numberformat.Format = "0.00";
                            worksheet.Cells[startRowTanHuong2, startCol + 14].Value = totalDTChenhLech2;
                            worksheet.Cells[startRowTanHuong2, startCol + 20].Formula = string.Format(@"IF(N{0} = 0, 0, (K{0}/N{0}))", startRowTanHuong2);//totalDTMucTieu3 != 0 ? "" * 100 : 0;
                            worksheet.Cells[startRowTanHuong2, startCol + 20].Style.Numberformat.Format = "0.00%"; // Định dạng phần trăm
                            using (var range = worksheet.Cells[startRowTanHuong2, startCol, startRowTanHuong2, startCol + 20])
                            {
                                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(yellowColor);
                            }
                            var endRowTanHuong2 = startRowTanHuong2 - 1; // last row filled for NM TH 2
                            var borderRangeTanHuong2 = worksheet.Cells[startRowTanHuong1 + 1, 1, endRowTanHuong2, 21];
                            borderRangeTanHuong2.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            borderRangeTanHuong2.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            borderRangeTanHuong2.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            borderRangeTanHuong2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            int summaryRow = startRowTanHuong2 + 1;
                            worksheet.Cells[summaryRow, startCol].Value = "NM TH 1+2";
                            worksheet.Cells[summaryRow, startCol].Style.Font.Bold = true;
                            worksheet.Cells[summaryRow, startCol].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRow, startCol + 1].Value = totalLaoDong1 + totalLaoDong2; // Total of LaoDong1 + LaoDong2
                            worksheet.Cells[summaryRow, startCol + 1].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRow, startCol + 2].Value = totalLDCoMat1 + totalLDCoMat2; // Total of LDCoMat1 + LDCoMat2
                            worksheet.Cells[summaryRow, startCol + 2].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRow, startCol + 8].Value = totalRC_Ngay1 + totalRC_Ngay2; // Total RC_Ngay
                            worksheet.Cells[summaryRow, startCol + 8].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRow, startCol + 10].Value = totalRC_DonGiaSum + totalRC_DonGiaSum2; // Total RC_DonGia
                            worksheet.Cells[summaryRow, startCol + 10].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRow, startCol + 11].Formula = string.Format(@"IF(C{0} = 0, 0, ROUND(K{0}/C{0}, 2))", summaryRow);
                            worksheet.Cells[summaryRow, startCol + 11].Style.Numberformat.Format = "0.00";
                            worksheet.Cells[summaryRow, startCol + 11].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRow, startCol + 12].Value = totalMucTieu + totalMucTieu2; // Total MucTieu
                            worksheet.Cells[summaryRow, startCol + 12].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRow, startCol + 13].Value = totalDTMucTieu + totalDTMucTieu2; // Total DT_MucTieu
                            worksheet.Cells[summaryRow, startCol + 13].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRow, startCol + 14].Value = totalDTChenhLech + totalDTChenhLech2; // Total DT_ChenhLech
                            worksheet.Cells[summaryRow, startCol + 14].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRow, startCol + 20].Formula = string.Format(@"K{0}/N{0}", summaryRow);//totalDTMucTieu3 != 0 ? "" * 100 : 0;
                            worksheet.Cells[summaryRow, startCol + 20].Style.Numberformat.Format = "0.00%"; // Định dạng phần trăm



                            using (var range = worksheet.Cells[summaryRow, startCol, summaryRow, startCol + 20])
                            {
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            startRowTanHuong2++;
                            startRowTanHuong2++;
                            startRowTanHuong2++;

                            // NM  Cg
                            worksheet.Cells[startRowTanHuong2, 1].Value = "NM  CG";
                            worksheet.Cells[startRowTanHuong2, 1].Style.Font.Bold = true;
                            worksheet.Cells[startRowTanHuong2, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                            int startRowdtChoGao = startRowTanHuong2 + 1;
                            double totalRC_DonGiaSum3 = 0;
                            double totalDTMucTieu3 = 0;
                            double totalDTChenhLech3 = 0;
                            double totalMucTieu3 = 0;
                            double totalLaoDongChoGao = dtChoGao.AsEnumerable().Sum(row => Convert.ToDouble(row["LaoDong"]));
                            double totalLDCoMatChoGao = dtChoGao.AsEnumerable().Sum(row => Convert.ToDouble(row["LDCoMat"]));
                            double totalRC_NgayChoGao = dtChoGao.AsEnumerable().Sum(row => Convert.ToDouble(row["RC_Ngay"]));
                            if (dtChoGao.Rows.Count > 0)
                                foreach (DataRow row in dtChoGao.Rows) // Use consistent variable name
                                {

                                    worksheet.Cells[startRowdtChoGao, startCol].Value = row["Name"].ToString();
                                    worksheet.Cells[startRowdtChoGao, startCol + 1].Value = row["LaoDong"].ToString();
                                    worksheet.Cells[startRowdtChoGao, startCol + 2].Value = row["LDCoMat"].ToString();
                                    worksheet.Cells[startRowdtChoGao, startCol + 3].Value = row["MaHang"].ToString();
                                    worksheet.Cells[startRowdtChoGao, startCol + 4].Value = row["KhachHang"].ToString();
                                    worksheet.Cells[startRowdtChoGao, startCol + 5].Value = row["TenHang"].ToString();
                                    worksheet.Cells[startRowdtChoGao, startCol + 6].Value = row["SLKH"].ToString();
                                    DateTime ngayGH1 = Convert.ToDateTime(row["NgayGH"]); // Chuyển đổi sang kiểu DateTime
                                    worksheet.Cells[startRowdtChoGao, startCol + 7].Value = ngayGH1.ToString("dd/MM/yyyy");
                                    worksheet.Cells[startRowdtChoGao, startCol + 8].Value = row["RC_Ngay"].ToString();
                                    worksheet.Cells[startRowdtChoGao, startCol + 9].Value = row["DonGia"].ToString();
                                    var rcNgay = Convert.ToDouble(row["RC_Ngay"]);
                                    var donGia = Convert.ToDouble(row["DonGia"]);
                                    var totalRC_DonGia = rcNgay * donGia;
                                    worksheet.Cells[startRowdtChoGao, startCol + 10].Value = totalRC_DonGia;
                                    var ldCoMat = Convert.ToDouble(row["LDCoMat"]);
                                    var result = ldCoMat != 0 ? totalRC_DonGia / ldCoMat : 0; // Tránh chia cho 0
                                    worksheet.Cells[startRowdtChoGao, startCol + 11].Value = result;
                                    worksheet.Cells[startRowdtChoGao, startCol + 12].Value = row["MucTieu"].ToString();
                                    worksheet.Cells[startRowdtChoGao, startCol + 13].Value = row["DT_MucTieu"].ToString();
                                    worksheet.Cells[startRowdtChoGao, startCol + 14].Value = row["DT_ChenhLech"].ToString();
                                    worksheet.Cells[startRowdtChoGao, startCol + 15].Value = row["RC_LK"].ToString();
                                    var slkh = Convert.ToDouble(row["SLKH"]);
                                    var rcLK = Convert.ToDouble(row["RC_LK"]);
                                    worksheet.Cells[startRowdtChoGao, startCol + 16].Value = slkh - rcLK;
                                    worksheet.Cells[startRowdtChoGao, startCol + 17].Value = row["SoNgayConLai"].ToString();
                                    int soNgayConLai = Convert.ToInt32(row["SoNgayConLai"]);
                                    DateTime ngayKetQua = ngayBaoCaoDate.AddDays(+soNgayConLai);
                                    worksheet.Cells[startRowdtChoGao, startCol + 18].Value = ngayKetQua.ToString("dd/MM/yyyy");
                                    DateTime ngayGH = DateTime.Parse(row["NgayGH"].ToString()).Date;

                                    TimeSpan difference = ngayGH - ngayKetQua;
                                    int daysDifference = (int)difference.TotalDays;
                                    worksheet.Cells[startRowdtChoGao, startCol + 19].Value = daysDifference;
                                    worksheet.Cells[startRowdtChoGao, startCol + 20].Value = "";
                                    totalRC_DonGiaSum3 += totalRC_DonGia;
                                    var dtMucTieu = Convert.ToDouble(row["DT_MucTieu"]);
                                    totalDTMucTieu3 += dtMucTieu;
                                    var mucTieu = Convert.ToDouble(row["MucTieu"]);
                                    totalMucTieu3 += mucTieu;
                                    var dtChenhLech = Convert.ToDouble(row["DT_ChenhLech"]);
                                    totalDTChenhLech3 += dtChenhLech;
                                    startRowdtChoGao++;
                                }
                            double tongTienLDNgay3 = totalLDCoMatChoGao != 0 ? totalRC_DonGiaSum3 / totalLDCoMatChoGao : 0;
                            worksheet.Cells[startRowdtChoGao, startCol].Value = "Total";
                            worksheet.Cells[startRowdtChoGao, startCol + 1].Value = totalLaoDongChoGao;
                            worksheet.Cells[startRowdtChoGao, startCol + 2].Value = totalLDCoMatChoGao;
                            worksheet.Cells[startRowdtChoGao, startCol + 8].Value = totalRC_NgayChoGao;
                            worksheet.Cells[startRowdtChoGao, startCol + 10].Value = totalRC_DonGiaSum3;
                            worksheet.Cells[startRowdtChoGao, startCol + 12].Value = totalMucTieu3;
                            worksheet.Cells[startRowdtChoGao, startCol + 13].Value = totalDTMucTieu3;
                            worksheet.Cells[startRowdtChoGao, startCol + 11].Formula = string.Format(@"IF(C{0} = 0, 0, ROUND(K{0}/C{0}, 2))", startRowdtChoGao);
                            worksheet.Cells[startRowdtChoGao, startCol + 11].Style.Numberformat.Format = "0.00";
                            worksheet.Cells[startRowdtChoGao, startCol + 14].Value = totalDTChenhLech3;
                            worksheet.Cells[startRowdtChoGao, startCol + 20].Formula = string.Format(@"IF(N{0} = 0, 0, (K{0}/N{0}))", startRowdtChoGao);//totalDTMucTieu3 != 0 ? "" * 100 : 0;
                            worksheet.Cells[startRowdtChoGao, startCol + 20].Style.Numberformat.Format = "0.00%"; // Định dạng phần trăm

                            using (var range = worksheet.Cells[startRowdtChoGao, startCol, startRowdtChoGao, startCol + 20])
                            {
                                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(yellowColor);
                            }
                            var endRowChoGao = startRowdtChoGao - 0;
                            var borderRangeChoGao = worksheet.Cells[startRowTanHuong2 + 1, 1, endRowChoGao, 21];
                            borderRangeChoGao.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            borderRangeChoGao.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            borderRangeChoGao.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            borderRangeChoGao.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            startRowdtChoGao++;
                            startRowdtChoGao++;

                            // NM  Cl
                            worksheet.Cells[startRowdtChoGao, 1].Value = "NM  CL";
                            worksheet.Cells[startRowdtChoGao, 1].Style.Font.Bold = true;
                            worksheet.Cells[startRowdtChoGao, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                            int startRowdtCaiLay = startRowdtChoGao + 1;
                            double totalRC_DonGiaSum4 = 0;
                            /*double totalDTMucTieu4 = 0;*/
                            double totalDTChenhLech4 = 0;
                            double totalMucTieu4 = 0;
                            double totalDTMucTieu4 = 0;
                            double totalLaoDongCaiLay = dtCaiLay.AsEnumerable().Sum(row => Convert.ToDouble(row["LaoDong"]));
                            double totalLDCoMatCaiLay = dtCaiLay.AsEnumerable().Sum(row => Convert.ToDouble(row["LDCoMat"]));
                            double totalRC_NgayCaiLay = dtCaiLay.AsEnumerable().Sum(row => Convert.ToDouble(row["RC_Ngay"]));
                            if (dtCaiLay.Rows.Count > 0)
                                foreach (DataRow row in dtCaiLay.Rows) // Use consistent variable name
                                {
                                    worksheet.Cells[startRowdtCaiLay, startCol].Value = row["Name"].ToString();
                                    worksheet.Cells[startRowdtCaiLay, startCol + 1].Value = row["LaoDong"].ToString();
                                    worksheet.Cells[startRowdtCaiLay, startCol + 2].Value = row["LDCoMat"].ToString();
                                    worksheet.Cells[startRowdtCaiLay, startCol + 3].Value = row["MaHang"].ToString();
                                    worksheet.Cells[startRowdtCaiLay, startCol + 4].Value = row["KhachHang"].ToString();
                                    worksheet.Cells[startRowdtCaiLay, startCol + 5].Value = row["TenHang"].ToString();
                                    worksheet.Cells[startRowdtCaiLay, startCol + 6].Value = row["SLKH"].ToString();
                                    DateTime ngayGH1 = Convert.ToDateTime(row["NgayGH"]); // Chuyển đổi sang kiểu DateTime
                                    worksheet.Cells[startRowdtCaiLay, startCol + 7].Value = ngayGH1.ToString("dd/MM/yyyy");
                                    worksheet.Cells[startRowdtCaiLay, startCol + 8].Value = row["RC_Ngay"].ToString();
                                    worksheet.Cells[startRowdtCaiLay, startCol + 9].Value = row["DonGia"].ToString();
                                    var rcNgay = Convert.ToDouble(row["RC_Ngay"]);
                                    var donGia = Convert.ToDouble(row["DonGia"]);
                                    var totalRC_DonGia = rcNgay * donGia;
                                    worksheet.Cells[startRowdtCaiLay, startCol + 10].Value = totalRC_DonGia;

                                    var ldCoMat = Convert.ToDouble(row["LDCoMat"]);
                                    var result = ldCoMat != 0 ? totalRC_DonGia / ldCoMat : 0; // Tránh chia cho 0
                                    worksheet.Cells[startRowdtCaiLay, startCol + 11].Value = result;
                                    worksheet.Cells[startRowdtCaiLay, startCol + 12].Value = row["MucTieu"].ToString();
                                    worksheet.Cells[startRowdtCaiLay, startCol + 13].Value = row["DT_MucTieu"].ToString();
                                    worksheet.Cells[startRowdtCaiLay, startCol + 14].Value = row["DT_ChenhLech"].ToString();
                                    worksheet.Cells[startRowdtCaiLay, startCol + 15].Value = row["RC_LK"].ToString();

                                    var slkh = Convert.ToDouble(row["SLKH"]);
                                    var rcLK = Convert.ToDouble(row["RC_LK"]);
                                    worksheet.Cells[startRowdtCaiLay, startCol + 16].Value = slkh - rcLK;
                                    worksheet.Cells[startRowdtCaiLay, startCol + 17].Value = row["SoNgayConLai"].ToString();
                                    int soNgayConLai = Convert.ToInt32(row["SoNgayConLai"]);
                                    DateTime ngayKetQua = ngayBaoCaoDate.AddDays(+soNgayConLai);
                                    worksheet.Cells[startRowdtCaiLay, startCol + 18].Value = ngayKetQua.ToString("dd/MM/yyyy");

                                    DateTime ngayGH = DateTime.Parse(row["NgayGH"].ToString()).Date;

                                    TimeSpan difference = ngayGH - ngayKetQua;
                                    int daysDifference = (int)difference.TotalDays;
                                    worksheet.Cells[startRowdtCaiLay, startCol + 19].Value = daysDifference;
                                    worksheet.Cells[startRowdtCaiLay, startCol + 20].Value = "";

                                    totalRC_DonGiaSum4 += totalRC_DonGia;
                                    var dtMucTieu = Convert.ToDouble(row["DT_MucTieu"]);
                                    totalDTMucTieu4 += dtMucTieu;
                                    var mucTieu = Convert.ToDouble(row["MucTieu"]);
                                    totalMucTieu4 += mucTieu;
                                    var dtChenhLech = Convert.ToDouble(row["DT_ChenhLech"]);
                                    totalDTChenhLech4 += dtChenhLech;

                                    startRowdtCaiLay++;

                                }
                            double tongTienLDNgay4 = totalLDCoMatCaiLay != 0 ? totalRC_DonGiaSum / totalLDCoMatCaiLay : 0;

                            worksheet.Cells[startRowdtCaiLay, startCol].Value = "Total";
                            worksheet.Cells[startRowdtCaiLay, startCol + 1].Value = totalLaoDongCaiLay;
                            worksheet.Cells[startRowdtCaiLay, startCol + 2].Value = totalLDCoMatCaiLay;
                            worksheet.Cells[startRowdtCaiLay, startCol + 8].Value = totalRC_NgayCaiLay;
                            worksheet.Cells[startRowdtCaiLay, startCol + 10].Value = totalRC_DonGiaSum4;
                            worksheet.Cells[startRowdtCaiLay, startCol + 12].Value = totalMucTieu4;
                            worksheet.Cells[startRowdtCaiLay, startCol + 13].Value = totalDTMucTieu4;
                            worksheet.Cells[startRowdtCaiLay, startCol + 11].Formula = string.Format(@"IF(C{0} = 0, 0, ROUND(K{0}/C{0}, 2))", startRowdtCaiLay);
                            worksheet.Cells[startRowdtCaiLay, startCol + 11].Style.Numberformat.Format = "0.00";
                            worksheet.Cells[startRowdtCaiLay, startCol + 14].Value = totalDTChenhLech4;
                            worksheet.Cells[startRowdtCaiLay, startCol + 20].Formula = string.Format(@"IF(N{0} = 0, 0, (K{0}/N{0}))", startRowdtCaiLay);//totalDTMucTieu3 != 0 ? "" * 100 : 0;
                            worksheet.Cells[startRowdtCaiLay, startCol + 20].Style.Numberformat.Format = "0.00%"; // Định dạng phần trăm
                            using (var range = worksheet.Cells[startRowdtCaiLay, startCol, startRowdtCaiLay, startCol + 20])
                            {
                                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(yellowColor);
                            }
                            var endRowCaiLay = startRowdtCaiLay - 0; // last row filled for NM CG
                            var borderRangeCaiLay = worksheet.Cells[startRowdtChoGao + 1, 1, endRowCaiLay, 21];
                            borderRangeCaiLay.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            borderRangeCaiLay.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            borderRangeCaiLay.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            borderRangeCaiLay.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            int summaryRowCaiLay = startRowdtCaiLay + 1;
                            double tongTienLDNgayCaiLay = (totalLDCoMat1 + totalLDCoMat2 + totalLDCoMatChoGao + totalLDCoMatCaiLay) != 0 ? (totalRC_DonGiaSum + totalRC_DonGiaSum2 + totalRC_DonGiaSum3 + totalRC_DonGiaSum4) / (totalLDCoMat1 + totalLDCoMat2 + totalLDCoMatChoGao + totalLDCoMatCaiLay) : 0;
                            worksheet.Cells[summaryRowCaiLay, startCol].Value = "Tổng Nhà Máy";
                            worksheet.Cells[summaryRowCaiLay, startCol].Style.Font.Bold = true;
                            worksheet.Cells[summaryRowCaiLay, startCol].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRowCaiLay, startCol + 1].Value = totalLaoDong1 + totalLaoDong2 + totalLaoDongChoGao + totalLaoDongCaiLay;
                            worksheet.Cells[summaryRowCaiLay, startCol + 1].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRowCaiLay, startCol + 2].Value = totalLDCoMat1 + totalLDCoMat2 + totalLDCoMatChoGao + totalLDCoMatCaiLay;
                            worksheet.Cells[summaryRowCaiLay, startCol + 2].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRowCaiLay, startCol + 8].Value = totalRC_Ngay1 + totalRC_Ngay2 + totalRC_NgayChoGao + totalRC_NgayCaiLay;
                            worksheet.Cells[summaryRowCaiLay, startCol + 8].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRowCaiLay, startCol + 10].Value = totalRC_DonGiaSum + totalRC_DonGiaSum2 + totalRC_DonGiaSum3 + totalRC_DonGiaSum4;
                            worksheet.Cells[summaryRowCaiLay, startCol + 10].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRowCaiLay, startCol + 12].Value = totalMucTieu + totalMucTieu2 + totalMucTieu3 + totalMucTieu4;
                            worksheet.Cells[summaryRowCaiLay, startCol + 12].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRowCaiLay, startCol + 13].Value = totalDTMucTieu + totalDTMucTieu2 + totalDTMucTieu3 + totalDTMucTieu4;
                            worksheet.Cells[summaryRowCaiLay, startCol + 13].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRowCaiLay, startCol + 14].Value = totalDTChenhLech + totalDTChenhLech2 + totalDTChenhLech3 + totalDTChenhLech4;
                            worksheet.Cells[summaryRowCaiLay, startCol + 14].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[summaryRowCaiLay, startCol + 20].Formula = string.Format(@"IF(N{0} = 0, 0, (K{0}/N{0}))", summaryRowCaiLay);//totalDTMucTieu3 != 0 ? "" * 100 : 0;
                            worksheet.Cells[summaryRowCaiLay, startCol + 20].Style.Numberformat.Format = "0.00%"; // Định dạng phần trăm
                            double tongRC_DonGia = (double)(worksheet.Cells[summaryRowCaiLay, startCol + 10].Value ?? 0);
                            double tongLDCoMat = (double)(worksheet.Cells[summaryRowCaiLay, startCol + 2].Value ?? 0);

                            /*                    double tongTienLDNgayCaiLay = (totalLDCoMat1 + totalLDCoMat2 + totalLDCoMatChoGao + totalLDCoMatCaiLay) != 0 ? (totalRC_DonGiaSum + totalRC_DonGiaSum2 + totalRC_DonGiaSum3 + totalRC_DonGiaSum4) / (totalLDCoMat1 + totalLDCoMat2 + totalLDCoMatChoGao + totalLDCoMatCaiLay) : 0;
                            */
                            worksheet.Cells[summaryRowCaiLay, startCol + 11].Value = tongTienLDNgayCaiLay;
                            worksheet.Cells[startRowdtCaiLay, startCol + 11].Style.Numberformat.Format = "0.00";
                            worksheet.Cells[summaryRowCaiLay, startCol + 11].Style.Font.Color.SetColor(System.Drawing.Color.Red);

                            using (var range = worksheet.Cells[summaryRowCaiLay, startCol, summaryRowCaiLay, startCol + 20])
                            {
                                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            startRowdtCaiLay++;
                            startRowdtCaiLay++;
                        }
                        else
                        {
                            MessageBox.Show("Có máy chủ không phản hồi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        worksheet.Cells["D7:D3000"].Style.WrapText = true;
                        worksheet.Cells["E:F"].AutoFitColumns();
                        // gửi file qua server
                        FileInfo resultFile = new FileInfo(resultFilePath);
                        package.SaveAs(resultFile);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                object misValue = System.Reflection.Missing.Value;
                throw new Exception(ex.Message);
                return;
            }

        }
        public static DataTable getDBFromOtherServer(string server_api, string queryString)
        {

            QueryObjViewModel objViewM = new QueryObjViewModel { query = queryString };
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(server_api, objViewM); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null)
                return null;
            if (tbl.Rows.Count == 0)
                return null;
            return tbl;
        }

        private void frmBaoCaoTongQuanSXWeb_Load(object sender, EventArgs e)
        {
            loadWebView("DVSX_TGI", "TexGiang");
        }
    }
}
public class CTDHResult
{
    public DataTable dt { get; set; }
    public DataTable dt2 { get; set; }
    public DataTable dt3 { get; set; }
    public DataTable dt4 { get; set; }

}
