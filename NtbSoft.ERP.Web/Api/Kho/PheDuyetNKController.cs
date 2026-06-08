using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Model.ERP.Kho;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace NtbSoft.ERP.Web.Api.ERP.Kho
{
    [RoutePrefix("api/PheDuyetNK")]
    public class PheDuyetNKController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(
            string parameter = null,
            string parameter1 = null,
            string parameter2 = null,
            string parameter3 = null,
            string parameter4 = null,
            string fromDate = null,
            string toDate = null)
        {
            string effectiveFromDate = string.IsNullOrWhiteSpace(fromDate) ? parameter3 : fromDate;
            string effectiveToDate = string.IsNullOrWhiteSpace(toDate) ? parameter4 : toDate;
            return new ErpPheDuyetNKModel().Get("GET", parameter, parameter1, parameter2, effectiveFromDate, effectiveToDate);
        }

        [HttpGet]
        [Route("Refresh")]
        public DataTable Refresh(
            string parameter = null,
            string parameter1 = null,
            string parameter2 = null,
            string parameter3 = null,
            string parameter4 = null,
            string fromDate = null,
            string toDate = null)
        {
            string effectiveFromDate = string.IsNullOrWhiteSpace(fromDate) ? parameter3 : fromDate;
            string effectiveToDate = string.IsNullOrWhiteSpace(toDate) ? parameter4 : toDate;
            return new ErpPheDuyetNKModel().Get("REFRESH", parameter, parameter1, parameter2, effectiveFromDate, effectiveToDate);
        }

        [HttpGet]
        [Route("Detail")]
        public DataTable Detail(string parameter = null, string parameter2 = null, string parameter3 = null)
        {
            return new ErpPheDuyetNKModel().GetDetail(parameter, parameter2, parameter3);
        }



        [HttpPost]
        [Route("Approve")]
        public string Approve(dynamic data, string parameter = null, string parameter1 = null, string parameter2 = null)
        {
            DataTable dt = ParseTypeTable(data);
            return new ErpPheDuyetNKModel().Post("DUYET", dt, parameter, parameter1, parameter2);
        }

        [HttpPost]
        [Route("Cancel")]
        public string Cancel(dynamic data, string parameter = null, string parameter1 = null, string parameter2 = null)
        {
            DataTable dt = ParseTypeTable(data);
            return new ErpPheDuyetNKModel().Post("HUY", dt, parameter, parameter1, parameter2);
        }

        [HttpPost]
        [Route("ExportExcel")]
        public HttpResponseMessage ExportExcel(JObject payload)
        {
            try
            {
                var rows = payload?["Rows"] as JArray;
                if (rows == null || rows.Count == 0)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                string templatePath = HttpContext.Current.Server.MapPath("~/Content/Templates/PXNK.xlsx");
                FileInfo file = new FileInfo(templatePath);
                if (!file.Exists)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    if (worksheet == null)
                    {
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }

                    // Header alignment (center both horizontally & vertically) + compact height
                    //using (var headerRange = worksheet.Cells["A8:J10"])
                    //{
                    //    headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //    headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //    headerRange.Style.WrapText = true;
                    //    headerRange.Style.ShrinkToFit = true;
                    //}
                    //worksheet.Row(8).Height = 18;
                    //worksheet.Row(9).Height = 20;
                    //worksheet.Row(10).Height = 18;

                    int firstDataRow = 11;
                    int styleTemplateRow = 11;
                    int maxCols = 10;

                    // Column widths (tune for "Tên, quy cách, phẩm chất sản phẩm (Items)")
                    worksheet.Column(4).Width = 60;
                    worksheet.Column(3).Width = 20;
                    worksheet.Column(6).Width = 14;
                    worksheet.Column(7).Width = 14;
                    worksheet.Column(8).Width = 14;
                    worksheet.Column(9).Width = 12;

                    for (int idx = 0; idx < rows.Count; idx++)
                    {
                        int rowNum = firstDataRow + idx;
                        if (rowNum > worksheet.Dimension.End.Row)
                        {
                            worksheet.InsertRow(rowNum, 1, styleTemplateRow);
                        }

                        var row = rows[idx] as JObject;
                        if (row == null) continue;

                        worksheet.Cells[rowNum, 1].Value = idx + 1;
                        worksheet.Cells[rowNum, 2].Value = "";
                        worksheet.Cells[rowNum, 3].Value = row["MaVT"]?.ToString() ?? "";
                        worksheet.Cells[rowNum, 4].Value = row["ChiTiet"]?.ToString() ?? "";
                        worksheet.Cells[rowNum, 5].Value = "";
                        worksheet.Cells[rowNum, 6].Value = row["TenDVVT"]?.ToString() ?? "";

                        double slNhap = ParseNumber(row["SLNhap"]);
                        double slKiem = ParseNumber(row["SLKiem"]);
                        worksheet.Cells[rowNum, 7].Value = slNhap;
                        worksheet.Cells[rowNum, 8].Value = slKiem;

                        string qcText = NormalizeQcStatus(row["TrangThaiQC"]);
                        if (string.IsNullOrWhiteSpace(qcText))
                        {
                            qcText = Math.Abs(slNhap - slKiem) < 0.00001 ? "Pass" : "Fail";
                        }
                        worksheet.Cells[rowNum, 9].Value = qcText;
                        worksheet.Cells[rowNum, 10].Value = "";

                        // Color Pass/Fail
                        var qcCell = worksheet.Cells[rowNum, 9];
                        if (qcText.Equals("Pass", StringComparison.OrdinalIgnoreCase))
                        {
                            qcCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(0, 176, 80)); // green
                            qcCell.Style.Font.Bold = true;
                        }
                        else if (qcText.Equals("Fail", StringComparison.OrdinalIgnoreCase))
                        {
                            qcCell.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(255, 0, 0)); // red
                            qcCell.Style.Font.Bold = true;
                        }
                    }

                    var range = worksheet.Cells[firstDataRow, 1, firstDataRow + rows.Count - 1, maxCols];
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    byte[] fileBytes = package.GetAsByteArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };

                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "PXNK-da-duyet.xlsx"
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    return response;
                }
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        private static double ParseNumber(JToken token)
        {
            if (token == null) return 0;
            if (double.TryParse(token.ToString(), out double value)) return value;
            return 0;
        }

        private static string NormalizeQcStatus(JToken token)
        {
            if (token == null) return "";
            string raw = token.ToString().Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(raw)) return "";
            if (raw.Contains("PASS") || raw == "1" || raw == "TRUE" || raw == "Y") return "Pass";
            if (raw.Contains("FAIL") || raw == "0" || raw == "FALSE" || raw == "N") return "Fail";
            return token.ToString();
        }

        private static DataTable ParseTypeTable(dynamic data)
        {
            if (data == null)
            {
                return null;
            }

            var json = JsonConvert.SerializeObject(data);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<DataTable>(json);
            }
            catch
            {
                try
                {
                    var wrapper = JsonConvert.DeserializeObject<PheDuyetTypeTableWrapper>(json);
                    return wrapper?.TypeTable;
                }
                catch
                {
                    return null;
                }
            }
        }

        private class PheDuyetTypeTableWrapper
        {
            public DataTable TypeTable { get; set; }
        }
    }
}
