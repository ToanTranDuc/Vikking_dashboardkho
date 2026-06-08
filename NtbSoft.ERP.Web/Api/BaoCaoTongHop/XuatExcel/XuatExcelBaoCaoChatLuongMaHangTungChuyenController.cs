using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BaoCaoTongHop.Api.XuatExcel
{
    [RoutePrefix("api/XuatExcelBaoCaoChatLuongMaHangTungChuyen")]
    public class XuatExcelBaoCaoChatLuongMaHangTungChuyenController : ApiController
    {
        [HttpPost]
        [Route("ExportExcel")]
        public HttpResponseMessage XuatExcelBaoCaoChatLuongMaHangTrenChsuyen(BaoCaoChatLuong_Request obj_request)
        {
            try
            {
                string Path = HttpContext.Current.Server.MapPath(@"\Templates\TemplateBaoCaoChatLuongMaHangTrenChuyen.xlsx");
                FileInfo file = new FileInfo(Path);

                int index = 0;
                int row = 6;
                int indexMerge = 0;

                string fileName = $"Báo cáo chất lượng mã hàng từng chuyền {DateTime.Now.ToString("dd-MM-yyyy-hhmmss")}";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage package = new ExcelPackage(file))
                {
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


                    // ===========================
                    // Kết thúc chèn biểu đồ
                    // ===========================

                    // gửi file qua server
                    byte[] fileBytes = package.GetAsByteArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(fileBytes);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
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
}