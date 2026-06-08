using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BaoCaoTongHop.Api.BaoCao
{
    [RoutePrefix("api/BaoCaoNhapKho")]
    public class NKTPController : ApiController
    {

        [HttpPost]
        [Route("ExPost")]
        public HttpResponseMessage Expost([FromBody] ExPostRequest postData)
        {
            try
            {
                if (postData == null) return null;

                List<ArrBodyItem> arrBody = postData.ArrBody;
                List<ArrBodyItemDate> copyArrHeader = postData.CopyArrHeader;

                string imagePathRelative = "/Templates/TemplateBaoCaoNKTP.xlsx";
                string imagePathPhysical = HttpContext.Current.Server.MapPath("~" + imagePathRelative);

                FileInfo templateFile = new FileInfo(imagePathPhysical);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    ExcelRange range = worksheet.Cells;
                    int row = 6;
                    int index = 0;
                    string tungay = Convert.ToDateTime(copyArrHeader[0].TuNgay).ToString("dd/MM/yyyy");
                    string denngay = Convert.ToDateTime(copyArrHeader[0].DenNgay).ToString("dd/MM/yyyy");
                    worksheet.Cells["B3"].Value = $"Từ ngày: {tungay}";
                    worksheet.Cells["D3"].Value = $"Tới ngày: {denngay}";
                    worksheet.Column(2).AutoFit();  // Cột B (B3)
                    worksheet.Column(4).AutoFit();  // Cột D (D3)
                    /*                    worksheet.Column(5).AutoFit();
                    */                    /*worksheet.Cells["E5:E6"].Merge = true;
                                        worksheet.Cells["E5"].Value = $"Báo Cáo từ ngày: {tungay} tới ngày: {denngay}";
                                        worksheet.Cells["E5:E6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        worksheet.Cells["E5:E6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    */
                    int indexMerge = 0;
                    foreach (var item in arrBody)
                    {
                        string currentMaHang = item.MaHang;
                        string previosMaHang = (index + 1 < arrBody.Count) ? arrBody[index + 1].MaHang : "abcea";
                        worksheet.Cells[row, 1].Value = item.MaHang;
                        worksheet.Cells[row, 2].Value = Convert.ToDateTime(item.NgayNhapKho).ToString("dd/MM/yyyy");
                        worksheet.Cells[row, 3].Value = item.SoThung;
                        range = worksheet.Cells[row, 4]; range.Value = item.SLSPNK; range.Style.Font.Bold = false;

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
                    worksheet.Column(1).AutoFit();
                    var borderData = worksheet.Cells[4, 1, row - 1, 4].Style.Border;
                    borderData.Bottom.Style =
                        borderData.Top.Style =
                        borderData.Left.Style =
                        borderData.Right.Style = ExcelBorderStyle.Thin;
                    // ===========================
                    // Thêm biểu đồ tại đây
                    // ===========================
                    var chart = worksheet.Drawings.AddChart("chart", eChartType.ColumnClustered) as ExcelBarChart;
                    chart.Title.Text = "Biểu đồ sản lượng nhập kho";
                    chart.SetPosition(4, 0, 5, 0);  // Vị trí bắt đầu từ ô E5 (row 5, column 5)
                    chart.SetSize(800, 600);  // Điều chỉnh kích thước biểu đồ

                    // Chọn dữ liệu cho biểu đồ (dùng các cột từ A đến D)
                    var series = chart.Series.Add(worksheet.Cells[4, 4, row - 1, 4], worksheet.Cells[4, 1, row - 1, 1]);
                    series.Header = "Sản lượng nhập kho";

                    // ===========================
                    // Kết thúc chèn biểu đồ
                    // ===========================
                    byte[] fileBytes = package.GetAsByteArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"BCDoThongSo.xlsx";


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
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
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

    }
    public class ArrBodyItem
    {
        public string MaHang { get; set; }
        public string Dot { get; set; }
        public string MaHangDisplay { get; set; }
        public DateTime NgayNhapKho { get; set; }
        public string TenDVSX { get; set; }
        public int SLSPNK { get; set; }
        public int SoThung { get; set; }
    }

    public class ArrBodyItemDate
    {
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }

    }
    public class ExPostRequest
    {
        public List<ArrBodyItem> ArrBody { get; set; }
        public List<ArrBodyItemDate> CopyArrHeader { get; set; }
    }
}