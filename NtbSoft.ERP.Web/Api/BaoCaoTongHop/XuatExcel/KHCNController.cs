

using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BaoCaoTongHop.Api.BaoCao
{
    [RoutePrefix("api/BaoCaoCatNgay")]
    public class KHCNController : ApiController
    {

        [HttpPost]
        [Route("ExPoster")]
        public HttpResponseMessage Exposter(string ngay,[FromBody] ExPosterRequest postData)
        {
            try
            {
                if (postData == null) return null;

                List<ArrBodyFile> arrBody = postData.ArrBody;
                List<ArrBodyDate> copyArrHeader = postData.CopyArrHeader;

                string imagePathRelative = "/Templates/TemplateBaoCaoKeHoachCatNgay.xlsx";
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
                    int slCutValue = 0;
                    int slLKValue = 0;
                    int slKHValue = 0;

                    int indexMerge = 0;
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
                    worksheet.Cells["D2"].Value = ngay;
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