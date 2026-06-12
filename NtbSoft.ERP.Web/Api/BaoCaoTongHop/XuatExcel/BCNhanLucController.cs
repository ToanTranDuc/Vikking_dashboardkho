using BaoCaoTongHop.Models.GetData;
using BaoCaoTongHop.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Web.Api;
using NtbSoft.ERP.Web.Models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
namespace BaoCaoTongHop.Api.BaoCao
{
    [RoutePrefix("api/BCNhanLuc")]
    public class BCNhanLucController : ApiController
    {
        public static HttpClientExtension _clientExtension = new HttpClientExtension();
  


        [HttpPost]
        [Route("ExportExcel")]
        public HttpResponseMessage ExportExcelReportTH(JObject Param)
        {
            try
            {        
                string server = string.Empty;
                string sql = string.Empty;
                int Row = 6;int StartRow = 6;
                string Path = HttpContext.Current.Server.MapPath(@"\Templates\TemplateBaoCaoNhanLuc.xlsx");
                FileInfo file = new FileInfo(Path);

                string fileName = $"Báo cáo nhân lực {DateTime.Now.ToString("dd-MM-yyyy-hhmmss")}";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                string Option = Param["Option"]?.ToString();

                string SV_api = ConfigurationManager.AppSettings[$"{Param["BrandID"]?.ToString()}"].ToString();
                sql = $"EXEC SP_SEW_NHANLUC @Action='GetNgay', @Ngay='{Param["LaborOfDate"]?.ToString()}', @LineX='', @fromDate='', @toDate='', @year=''";
                DataTable dt_LaborOfDate = ShortSqlQueryController.getDBFromOtherServer(SV_api, sql);
                sql = $"EXEC SP_SEW_NHANLUC @Action='GetSLAll', @Ngay='', @LineX='', @fromDate='{Param["FromDate"]?.ToString()}', @toDate='{Param["ToDate"]?.ToString()}', @year=''";
                DataTable dt_Labor = ShortSqlQueryController.getDBFromOtherServer(SV_api, sql);
                sql = $"EXEC SP_SEW_NHANLUC @Action = '{(Option?.ToLower() == "all year" ? "GetYear" : "GetMonth")}', @Ngay = '', @LineX = '', @fromDate = '', @toDate = '', @year = '{(Option?.ToLower() == "all year" ? "" : Option)}'";
                DataTable dt_LaborYear = ShortSqlQueryController.getDBFromOtherServer(SV_api, sql);

                using (ExcelPackage package = new ExcelPackage(file))
                {

                    if (dt_LaborOfDate?.Rows?.Count > 0)
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                        worksheet.Cells["B3"].Value = Convert.ToDateTime(Param["LaborOfDate"]?.ToString()).ToString("dd-MM-yyyy");
                        worksheet.Cells.Style.Font.Name = "Times New Roman";
                        DrawChartSheet1(worksheet, dt_LaborOfDate);
                    }
                    if(dt_Labor?.Rows?.Count > 0)
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet2"];
                        worksheet.Cells.Style.Font.Name = "Times New Roman";
                        DrawChartSheet2(worksheet, dt_Labor);
                    }
                    if (dt_LaborYear?.Rows?.Count > 0)
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet3"];
                        worksheet.Cells.Style.Font.Name = "Times New Roman";
                        DrawChartSheet3(worksheet, dt_LaborYear,Option);
                    }
                                        
                    // Send the file to the server
                    byte[] fileBytes = package.GetAsByteArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };

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


        private void DrawChartSheet1(ExcelWorksheet worksheet,DataTable dt_LaborOfDate)
        {
            int Row = 6;          
            //worksheet.Cells[Row, 1].Value = "Chuyền";
            //worksheet.Cells[Row, 2].Value = "Số Lao Động";
            //worksheet.Cells[Row, 3].Value = "Số Lao Động Vắng";

            foreach (DataRow row in dt_LaborOfDate.Rows)
            {
                Row++;
                worksheet.Cells[Row, 1].Value = row["DepID"]?.ToString();
                worksheet.Cells[Row, 2].Value = Convert.ToInt32(row["SoLaoDong"]?.ToString());
                worksheet.Cells[Row, 3].Value = Convert.ToInt32(row["SoLaoDongVang"]?.ToString());

            }
            //// Tạo biểu đồ tròn
            var pieChart = worksheet.Drawings.AddChart("ChiPhiChart", eChartType.Pie) as ExcelPieChart;
            pieChart.Title.Text = "Biểu Đồ Lao Động Trong Ngày";
            pieChart.SetPosition(20, 0, 6, 0);
            pieChart.SetSize(600, 300);

            // Thêm dữ liệu cho biểu đồ tròn
            var series2 = pieChart.Series.Add(worksheet.Cells[$"B7:B{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            series2.Header = "Chuyền";


            // Tạo biểu đồ cột
            var columnChart = worksheet.Drawings.AddChart("LaoDongTheoNgayChart", eChartType.ColumnClustered) as ExcelBarChart;
            columnChart.Title.Text = "Biểu Đồ Lao Động Vắng Trong Ngày";
            columnChart.Style = OfficeOpenXml.Drawing.Chart.eChartStyle.Style16;
            columnChart.SetPosition(5, 0, 6, 0);
            columnChart.SetSize(600, 300);

            // Thêm dữ liệu cho biểu đồ cột
            var chartSerieCol = columnChart.Series.Add(worksheet.Cells[$"C7:C{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            chartSerieCol.Header = "Chuyền";

            columnChart.Legend.Position = eLegendPosition.Bottom;
            columnChart.YAxis.Title.Text = "Số Lượng";
            columnChart.XAxis.Title.Text = "Chuyền";

            var range = worksheet.Cells[$"A7:C{Row}"];
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        }

        private void DrawChartSheet2(ExcelWorksheet worksheet, DataTable dt_Labor)
        {
            int Row = 6;         
            //worksheet.Cells[Row, 1].Value = "Chuyền";
            //worksheet.Cells[Row, 2].Value = "Tổng Lao Động";
            //worksheet.Cells[Row, 3].Value = "Tổng Lao Động Vắng";

           
            foreach (DataRow row in dt_Labor.Rows)
            {
                Row++;
                worksheet.Cells[Row, 1].Value = row["DepID"]?.ToString();               
                worksheet.Cells[Row, 2].Value = Convert.ToInt32(row["TongLD"]);          
                worksheet.Cells[Row, 3].Value = Convert.ToInt32(row["LDV"]);            
            }

           
            var columnChart = worksheet.Drawings.AddChart("LaoDongTheoNgayChart", eChartType.ColumnStacked) as ExcelBarChart;
            columnChart.Title.Text = $"Biểu Đồ Số Lao Động Theo Từng Chuyền";
            columnChart.Style = OfficeOpenXml.Drawing.Chart.eChartStyle.Style11;            
            columnChart.SetPosition(5, 0, 6, 0);
            columnChart.SetSize(600, 300);       

            var totalLaborSeries = columnChart.Series.Add(worksheet.Cells[$"B7:B{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            totalLaborSeries.Header = "Tổng Lao Động";

            
            var absentLaborSeries = columnChart.Series.Add(worksheet.Cells[$"C7:C{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            absentLaborSeries.Header = "Tổng Lao Động Vắng";


            columnChart.Legend.Position = eLegendPosition.Bottom;                        
            columnChart.XAxis.Title.Text = "Chuyền";                

            
           

            var range = worksheet.Cells["A7:C25"];
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        }


        private void DrawChartSheet3(ExcelWorksheet worksheet, DataTable dt_LaborOfYear,string Option)
        {
            int Row = 6;          
            worksheet.Cells[Row, 1].Value = $"{(Option?.ToLower() == "all year" ? "Năm" : "Tháng")}";        
            //worksheet.Cells[Row, 2].Value = "Số Lao Động ";
            //worksheet.Cells[Row, 3].Value = "Số Lao Động Vắng";
            //worksheet.Cells[Row, 4].Value = "Số Lao Động Thêm";
            //worksheet.Cells[Row, 5].Value = "Số Lao Động Nghỉ Việc";

            foreach (DataRow row in dt_LaborOfYear.Rows)
            {
                Row++;
                worksheet.Cells[Row, 1].Value = $"{(Option?.ToLower() == "all year" ? "" : "Tháng ")}" + row[$"{(Option?.ToLower() == "all year" ? "Nam" : "thang")}"]?.ToString();
                worksheet.Cells[Row, 2].Value = Convert.ToInt32(row["SLD"]?.ToString());
                worksheet.Cells[Row, 3].Value = Convert.ToInt32(row["SLDV"]?.ToString());
                worksheet.Cells[Row, 4].Value = Convert.ToInt32(row["SoLaoDongThem"]?.ToString());
                worksheet.Cells[Row, 5].Value = Convert.ToInt32(row["SoLaoDongNghiViec"]?.ToString());

            }
           

            //// Tạo biểu đồ đường (line chart)
            var lineChartIncrease = worksheet.Drawings.AddChart("ChiPhiChart", eChartType.LineStacked) as ExcelLineChart;
            lineChartIncrease.Title.Text = "Biểu Đồ Biến Động Lao Động ";
            lineChartIncrease.SetPosition(20, 0, 6, 0);
            lineChartIncrease.SetSize(800, 300);

            var lineChartReduced = worksheet.Drawings.AddChart("ChiPhiChart1", eChartType.LineStacked) as ExcelLineChart;
            lineChartReduced.Title.Text = "Biểu Đồ Biến Động Lao Động";
            lineChartReduced.SetPosition(20, 0, 8, 0);
            lineChartReduced.SetSize(800, 300);

            // Thêm dữ liệu cho biểu đồ đường
            var series2 = lineChartReduced.Series.Add(worksheet.Cells[$"E7:E{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            series2.Header = "Số LĐ Giảm";

            var series1 = lineChartIncrease.Series.Add(worksheet.Cells[$"D7:D{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            series1.Header = "Số LĐ Tăng";

          


            //lineChart.Legend.Position = eLegendPosition.Bottom;
            ////lineChart.YAxis.Title.Text = "Số Lượng";
            //lineChart.XAxis.Title.Text = "Thời Gian";

            // Tạo biểu đồ cột
            var columnChart = worksheet.Drawings.AddChart("LaoDongTheoNgayChart", eChartType.ColumnClustered) as ExcelBarChart;
            columnChart.Title.Text = $"Biểu Đồ Số Lao Động Vắng Theo Thời Gian";
            columnChart.SetPosition(5, 0, 6, 0);
            columnChart.SetSize(600, 300);

            // Thêm dữ liệu cho biểu đồ cột
            var chartSerieCol = columnChart.Series.Add(worksheet.Cells[$"B7:B{Row}"], worksheet.Cells[$"A7:A{Row}"]);
            chartSerieCol.Header = "Chuyền";

            worksheet.Cells.Style.Font.Name = "Times New Roman";


            var range = worksheet.Cells[$"A7:E{Row}"];
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        }
    }
}