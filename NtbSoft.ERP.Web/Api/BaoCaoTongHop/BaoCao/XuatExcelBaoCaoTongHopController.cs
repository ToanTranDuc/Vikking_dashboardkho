using BaoCaoTongHop.Models.GetData;
using BaoCaoTongHop.Service;
using Newtonsoft.Json;
using NtbSoft.ERP.Web.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BaoCaoTongHop.Api.BaoCao
{
    [RoutePrefix("api/XuatExcelBaoCaoTongHop")]
    public class XuatExcelBaoCaoTongHopController : ApiController
    {
        public static HttpClientExtension _clientExtension = new HttpClientExtension();
        //[HttpPost]
        //[Route("BaoCaoChatLuong")]
        //public DataTable BCChatLuong(RunStoredViewModel objView)
        //{
        //    string SV_api = ConfigurationManager.AppSettings[objView.server].ToString();
        //    string sqlQuery_Str_tb1 = string.Format(@"EXEC {0} {1}", "sp_ReportChatLuongThang_getReport_tb1", objView.paramStr);
        //    string sqlQuery_Str_tb2 = string.Format(@"EXEC {0} {1}", "sp_ReportChatLuongThang_getReport_tb2", objView.paramStr);
        //    string sqlQuery_Str_tb3 = string.Format(@"EXEC {0} {1}", "sp_ReportChatLuongThang_getReport_tb3", objView.paramStr);
        //    DataTable dtView = getDBFromOtherServer(SV_api, sqlQuery_Str_tb1); //getData( "select * from Product").Result;
        //    DataTable dtTruocUi = getDBFromOtherServer(SV_api, sqlQuery_Str_tb2);
        //    DataTable dtSauUi = getDBFromOtherServer(SV_api, sqlQuery_Str_tb3);
        //    DataTable tbl = new DataTable();
        //    tbl = BindingData(dtView, dtTruocUi, dtSauUi);
        //    return tbl;
        //}
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
       
        [HttpPost]
        [Route("ExportExcel_ChatLuongSauUi")]
        public HttpResponseMessage XuatExcelBaoCaoChatLuongMaHangSauUi(RunStoredViewModel objView)
        {
            try
            {
                string SV_api = ConfigurationManager.AppSettings[objView.server].ToString();
                string SV_DH = ConfigurationManager.AppSettings["DH"].ToString();

                string Path = HttpContext.Current.Server.MapPath(@"\Content\ReportThemes\TemplateBaoCaoDongThung.xlsx");
                FileInfo file = new FileInfo(Path);

                string fileName = $"Báo cáo chất lượng mã hàng sau ủi {DateTime.Now.ToString("dd-MM-yyyy-hhmmss")}";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Report"];

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
        [HttpPost]
        [Route("ExportExcel_ChatLuongSauUi")]
        public HttpResponseMessage XuatExcelBaoCaoThoiGianHaoPhi(RunStoredViewModel objView)
        {
            try
            {
                string SV_api = ConfigurationManager.AppSettings[objView.server].ToString();
                string SV_DH = ConfigurationManager.AppSettings["DH"].ToString();

                string Path = HttpContext.Current.Server.MapPath(@"\Content\ReportThemes\TemplateBaoCaoDongThung.xlsx");
                FileInfo file = new FileInfo(Path);

                string fileName = $"Báo cáo thời gian hao phí trên chuyền {DateTime.Now.ToString("dd-MM-yyyy-hhmmss")}";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Report"];

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
}