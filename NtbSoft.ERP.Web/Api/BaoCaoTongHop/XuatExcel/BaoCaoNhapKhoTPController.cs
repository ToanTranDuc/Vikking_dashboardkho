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
    [RoutePrefix("api/BCNhapKhoTP")]
    public class BaoCaoNhapKhoTPController : ApiController
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
                int Row = 6; int StartRow = 6;
                object MergeMH = null;DateTime parsedDate = new DateTime();
                int rowSpan = 0; int idx = 0;

                string Path = HttpContext.Current.Server.MapPath(@"\Templates\TemplateBaoCaoNhapKhoTP.xlsx");
                FileInfo file = new FileInfo(Path);

                string fileName = $"Báo cáo nhân lực {DateTime.Now.ToString("dd-MM-yyyy-hhmmss")}";
                string strDate = string.Empty;
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                string SV_api = ConfigurationManager.AppSettings["DH"].ToString();
                //string SV_api = ConfigurationManager.AppSettings["main"];
                sql = $"EXEC SP_DB_BaoCaoNhapKhoTP @Action = 'Get', @Para1 = '{Param["FromDate"]?.ToString()}', @Para2= '{Param["ToDate"]?.ToString()}', @Para3 = '{Param["BranchID"]?.ToString()}', @Para4 = '', @Para5 = ''";
                DataTable dt = ShortSqlQueryController.getDBFromOtherServer(SV_api, sql);

              
                using (ExcelPackage package = new ExcelPackage(file))
                {

                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                    worksheet.Cells.Style.Font.Name = "Times New Roman";

         

                    if (dt.Rows.Count > 0)
                    {

                        try
                        {
                            worksheet.Cells["D3"].Value = DateTime.ParseExact(Param["FromDate"]?.ToString(), "yyyy-MM-dd", null).ToString("dd-MM-yyyy");
                            worksheet.Cells["D4"].Value = DateTime.ParseExact(Param["ToDate"]?.ToString(), "yyyy-MM-dd", null).ToString("dd-MM-yyyy");

                        }
                        catch (Exception ex)
                        {
                            throw new Exception();
                        }


                        foreach (DataRow row in dt.Rows)
                        {
                            Row++;
                            object currentMH = row["MaHang"];
                            MergeMH = (idx == dt.Rows.Count-1) ? "" : dt.Rows[idx + 1]["MaHang"].ToString();
                            bool isMHMearge = currentMH.ToString() != MergeMH.ToString();

                            if (!isMHMearge)
                            {
                                worksheet.Cells[Row - rowSpan, 1, Row, 1].Merge = true;
                                worksheet.Cells[Row - rowSpan, 1, Row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[Row - rowSpan, 1, Row, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                               
                                rowSpan = 0;
                            }
                            else
                            {
                                rowSpan++;
                            }

                            if (DateTime.TryParse(row["NgayNhapKho"]?.ToString(), out parsedDate))
                            {


                                strDate=parsedDate.ToString("dd-MM-yyyy");

                            }
                            else
                            {
                                strDate = "";
                            }

                            worksheet.Cells[Row, 1].Value = row["MaHang"]?.ToString();
                            worksheet.Cells[Row, 2].Value = row["Dot"]?.ToString();
                            worksheet.Cells[Row, 3].Value = strDate;
                            worksheet.Cells[Row, 4].Value = Convert.ToInt32(row["SLKH"]?.ToString());
                            worksheet.Cells[Row, 5].Value = Convert.ToInt32(row["SLSPNK"]?.ToString());
                            worksheet.Cells[Row, 6].Value = Convert.ToInt32(row["SoThung"]?.ToString());

                            //worksheet.Cells[Row, 2].Style.WrapText = true;
                            idx++;
                        }
                    }
                

                    var range = worksheet.Cells[$"A7:F{Row}"];
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells[$"B7:C{Row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[$"D7:F{Row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;


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

    }
}