using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using OfficeOpenXml.Style;
using OfficeOpenXml.Drawing.Chart;


namespace BaoCaoTongHop.Api.XuatExcel
{
    [RoutePrefix("api/XuatExcelBaoCaoNangXuatDoanhThu")]
    public class XuatExcekBaoCaoNangXuat_DoanhThuController : ApiController
    {
        [HttpPost]
        [Route("ExportExcel")]
        public HttpResponseMessage XuatExcelBaoCaoChatLuongMaHangTrenChsuyen(DataReponse obj)
        {
            try
            {
                List<BaoCaoNangXuat_DoanhThu_ViewModel> lst_obj = obj.DataBC;
                string Path = HttpContext.Current.Server.MapPath(@"\Templates\TemplateBaoCaoNangSuatDoanhThu.xlsx");
                FileInfo file = new FileInfo(Path);

                string fileName = $"Báo cáo năng suất doanh thu {DateTime.Now.ToString("dd-MM-yyyy-hhmmss")}";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage package = new ExcelPackage(file))
                {
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

