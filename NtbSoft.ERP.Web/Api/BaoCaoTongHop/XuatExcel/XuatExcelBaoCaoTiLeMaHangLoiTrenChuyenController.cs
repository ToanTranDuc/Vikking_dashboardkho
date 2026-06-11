using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BaoCaoTongHop.Api.XuatExcel
{
    [RoutePrefix("api/XuatExcelBaoCaoTiLeMaHangLoiTrenChuyen")]
    public class XuatExcelBaoCaoTiLeMaHangLoiTrenChuyenController : ApiController
    {
        [HttpPost]
        [Route("ExportExcel")]
        public HttpResponseMessage XuatExcelBaoCaoTiLeMaHangLoiTrenChuyen(BaoCaoTiLeMaHangLoiTrenChuyen_Request obj_request)
        {
            try
            {
                string Path = HttpContext.Current.Server.MapPath(@"\Templates\TemplateBCMHLoiTrenTungChuyenMay.xlsx");
                FileInfo file = new FileInfo(Path);

                string fileName = $"Báo cáo tỉ lệ mã hàng lỗi trên chuyền {DateTime.Now.ToString("dd-MM-yyyy-hhmmss")}";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                List<BaoCaoTiLeMaHangLoiTrenChuyen> lstData = obj_request.DataBC;

                using (ExcelPackage package = new ExcelPackage(file))
                {

                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    int startRow = 6; // Bắt đầu từ ô A6

                    foreach (BaoCaoTiLeMaHangLoiTrenChuyen item in lstData)
                    {
                        worksheet.Cells[startRow, 1].Value = item.Chuyen; // Cột A - Chuyền
                        worksheet.Cells[startRow, 2].Value = item.MaHang; // Cột B - Mã hàng
                        worksheet.Cells[startRow, 3].Value = item.Mau; // Cột C - Màu
                        worksheet.Cells[startRow, 4].Value = Convert.ToInt32(item.SoLuongKiem); // Cột D - Số lượng kiểm
                        worksheet.Cells[startRow, 5].Value = Convert.ToInt32(item.SoLuongDat); // Cột E - Số lượng đạt
                        worksheet.Cells[startRow, 6].Value = Convert.ToInt32(item.SoLuongLoi);
                        worksheet.Cells[startRow, 7].Value = double.TryParse(item.TiLeLoi.ToString(), out double loiValue) ? loiValue : 0; // Cột F - Số lượng lỗi
                        startRow++;
                    }
                    worksheet.Cells["D2"].Value = obj_request.TuNgay;
                    worksheet.Cells["D3"].Value = obj_request.DenNgay;
                    // ===========================
                    // Thêm biểu đồ tại đây
                    // ===========================
                    var chart = worksheet.Drawings.AddChart("chart", eChartType.ColumnClustered) as ExcelBarChart;
                    chart.Title.Text = "Biểu đồ mã hàng lỗi trên chuyền may";
                    chart.SetPosition(4, 0, 7, 0); // Đặt vị trí cho biểu đồ (ô H5)
                    chart.SetSize(800, 400); // Điều chỉnh kích thước biểu đồ

                    // Chọn dữ liệu cho biểu đồ
                    var xRange = worksheet.Cells[6, 2, startRow - 1, 2]; // Mã hàng (cột B)
                    var yRange2 = worksheet.Cells[6, 7, startRow - 1, 7];

                    var series2 = chart.Series.Add(yRange2, xRange);
                    series2.Header = "Tỉ lệ lỗi";



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
    }
    public class BaoCaoTiLeMaHangLoiTrenChuyen
    {
        public string Chuyen { get; set; }
        public string MaHang { get; set; }
        public string Mau { get; set; }
        public string SoLuongKiem { get; set; }
        public string SoLuongDat { get; set; }
        public string SoLuongLoi { get; set; }
        public string TiLeLoi { get; set; }
    }
    public class BaoCaoTiLeMaHangLoiTrenChuyen_Request
    {
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public List<BaoCaoTiLeMaHangLoiTrenChuyen> DataBC;
    }
}