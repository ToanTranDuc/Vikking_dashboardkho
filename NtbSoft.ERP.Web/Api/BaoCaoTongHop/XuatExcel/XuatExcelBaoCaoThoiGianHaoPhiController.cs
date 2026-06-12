using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
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
    [RoutePrefix("api/XuatExcelBaoCaoThoiGianHaoPhi")]
    public class XuatExcelBaoCaoThoiGianHaoPhiController : ApiController
    {
        [HttpPost]
        [Route("ExportExcel")]
        public HttpResponseMessage XuatExcelBaoCaoChatLuongMaHangTrenChuyen(BaoCaoHaoPhi_Request obj_request)
        {
            try
            {
                string Path = HttpContext.Current.Server.MapPath(@"\Templates\TemplateBaoCaoThoiGianHaoPhi.xlsx");
                FileInfo file = new FileInfo(Path);

                string fileName = $"Báo cáo chất lượng mã hàng từng chuyền {DateTime.Now.ToString("dd-MM-yyyy-hhmmss")}";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                int index = 0;
                int row = 7;
                int CTM_Tong = 0;
                int CKT_Tong = 0;
                int CNRN_Tong = 0;
                int count_CTM = 0;
                int count_CKT = 0;
                int count_CNRN = 0;


                using (ExcelPackage package = new ExcelPackage(file))
                {

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