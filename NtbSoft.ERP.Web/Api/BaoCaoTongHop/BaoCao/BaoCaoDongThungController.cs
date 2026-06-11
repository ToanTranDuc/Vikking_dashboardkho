using BaoCaoTongHop.Models.GetData;
using BaoCaoTongHop.Service;
using Newtonsoft.Json;
using NtbSoft.ERP.Web.Models;
using OfficeOpenXml;
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
    [RoutePrefix("api/sqlQuerryBCTD")]
    public class BaoCaoDongThungController : ApiController
    {
        public static HttpClientExtension _clientExtension = new HttpClientExtension();
        [HttpPost]
        [Route("getTongQuan")]
        public DataTable getTongQuan(RunStoredViewModel objView)
        {
            string SV_api = ConfigurationManager.AppSettings[objView.server].ToString();
            string SV_DH = ConfigurationManager.AppSettings["DH"].ToString();

            string querryGDT_client = createQuerry(objView.stored, objView.paramStr);
            DataTable dbClient = getDBFromOtherServer(SV_api, querryGDT_client);
            string sqlQuery = createQuerryDH("GetDataDongThungTongQuan", objView.server, objView.paramStr.Replace("@Action = 'GetDSTongQuan', @para0 = '", "").Replace("', @para1 = '',@para2 = '',@para3 = '',@para4 = ''", ""));
            DataTable dbServer = getDBFromOtherServer(SV_DH, sqlQuery);
            if (dbServer != null)
                for (int i = 0; i < dbClient.Rows.Count; i++)
                {
                    int tongDT = 0, tongNK = 0;
                    for (int j = 0; j < dbServer.Rows.Count; j++)
                    {
                        if (dbServer.Rows[j]["SPOID"].ToString() == dbClient.Rows[i]["SPOID"].ToString())
                        {
                            tongDT += Convert.ToInt32(dbServer.Rows[j]["NhanDT"].ToString());
                            tongNK += Convert.ToInt32(dbServer.Rows[j]["NhanNK"].ToString());
                        }
                    }
                    dbClient.Rows[i]["NhanDT"] = tongDT;
                    dbClient.Rows[i]["NhanNK"] = tongNK;
                }
            return dbClient;
        }
        [HttpPost]
        [Route("getResult")]
        public DataTable getResult(RunStoredViewModel objView)
        {
            string SV_api = ConfigurationManager.AppSettings[objView.server].ToString();
            string SV_DH = ConfigurationManager.AppSettings["DH"].ToString();

            string querryGDT_client = createQuerry(objView.stored, objView.paramStr);
            DataTable dbClient = getDBFromOtherServer(SV_api, querryGDT_client);
            var sqlpara = objView.paramStr.Replace("@Action = 'GetDS_Result', @para0 = '", "").Replace("'", "").Replace("@para1 = ", "").Replace("@para2 = ", "").Replace("@para3 = ", "").Replace("@para4 = ", "").Trim().Replace(",","@").Split('@');
            string sqlQuery = createQuerryDH("GetDataDongThungResult", objView.server, string.Format(@"{0}@{1}@{2}@{3}@", sqlpara[0], sqlpara[1], sqlpara[3], sqlpara[4])); 
            DataTable dbServer = getDBFromOtherServer(SV_DH, sqlQuery);
            if (dbServer != null)
                for (int i = 0; i < dbClient.Rows.Count; i++)
                {
                    int tongDT = 0, tongNK = 0;
                    for (int j = 0; j < dbServer.Rows.Count; j++)
                    {
                        if (dbServer.Rows[j]["MaLenh"].ToString() == dbClient.Rows[i]["MaLenh"].ToString() &&
                            dbServer.Rows[j]["POID"].ToString() == dbClient.Rows[i]["POID"].ToString() &&
                            dbServer.Rows[j]["DauSizeID"].ToString() == dbClient.Rows[i]["SizeTypeID"].ToString() &&
                            dbServer.Rows[j]["ColorID"].ToString() == dbClient.Rows[i]["ColorID"].ToString() &&
                            dbServer.Rows[j]["SizeID"].ToString() == dbClient.Rows[i]["SizeID"].ToString()
                            )
                        {
                            tongDT += Convert.ToInt32(dbServer.Rows[j]["NhanDT"].ToString());
                            tongNK += Convert.ToInt32(dbServer.Rows[j]["NhanNK"].ToString());
                        }
                    }
                    dbClient.Rows[i]["NhanDT"] = tongDT;
                    dbClient.Rows[i]["NhanNK"] = tongNK;
                }
            return dbClient;
        }
        [HttpPost]
        [Route("ExportExcel")]
        public HttpResponseMessage ExportExcelReportTH(RunStoredViewModel objView)
        {
            try
            {
                string SV_api = ConfigurationManager.AppSettings[objView.server].ToString();
                string SV_DH = ConfigurationManager.AppSettings["DH"].ToString();

                string querryGDT_client = createQuerry(objView.stored, objView.paramStr);
                DataTable dbClient = getDBFromOtherServer(SV_api, querryGDT_client);
                string sqlQuery = createQuerryDH("GetDataDongThungTongQuan", objView.server, objView.paramStr.Replace("@Action = 'GetDSTongQuan', @para0 = '", "").Replace("', @para1 = '',@para2 = '',@para3 = '',@para4 = ''", ""));
                DataTable dbServer = getDBFromOtherServer(SV_DH, sqlQuery);
                if (dbServer != null)
                    for (int i = 0; i < dbClient.Rows.Count; i++)
                    {
                        int tongDT = 0, tongNK = 0;
                        for (int j = 0; j < dbServer.Rows.Count; j++)
                        {
                            if (dbServer.Rows[j]["SPOID"].ToString() == dbClient.Rows[i]["SPOID"].ToString())
                            {
                                tongDT += Convert.ToInt32(dbServer.Rows[j]["NhanDT"].ToString());
                                tongNK += Convert.ToInt32(dbServer.Rows[j]["NhanNK"].ToString());
                            }
                        }
                        dbClient.Rows[i]["NhanDT"] = tongDT;
                        dbClient.Rows[i]["NhanNK"] = tongNK;
                    }
                DataTable db = dbClient;

                int slLenh = db.Rows.Count;

                db.Columns.Add("ConLaiTP");
                db.Columns.Add("ConLaiDT");
                db.Columns.Add("ConLaiNK");
                for (int i = 0; i < db.Rows.Count; i++)
                {
                    db.Rows[i]["ConLaiTP"] = Convert.ToInt32(db.Rows[i]["SoLuong"].ToString()) - Convert.ToInt32(db.Rows[i]["NhanTP"].ToString());
                    db.Rows[i]["ConLaiDT"] = Convert.ToInt32(db.Rows[i]["SoLuong"].ToString()) - Convert.ToInt32(db.Rows[i]["NhanDT"].ToString());
                    db.Rows[i]["ConLaiNK"] = Convert.ToInt32(db.Rows[i]["SoLuong"].ToString()) - Convert.ToInt32(db.Rows[i]["NhanNK"].ToString());
                }
                string malenhSS = db.Rows[0]["MaLenh"].ToString();
                for (int i = 0; i < db.Rows.Count; i++)
                {
                    string maLenhIndex = db.Rows[i]["MaLenh"].ToString();
                    if (malenhSS != maLenhIndex)
                    {
                        DataRow row = db.NewRow();
                        row["MaLenh"] = malenhSS;
                        row["NgayGH"] = "Sum";

                        int SLTong = SumColumn(db, malenhSS, "SoLuong");
                        int SLNhanTP = SumColumn(db, malenhSS, "NhanTP");
                        int SLNhanDT = SumColumn(db, malenhSS, "NhanDT");
                        int SLNhanNK = SumColumn(db, malenhSS, "NhanNK");

                        row["SoLuong"] = SLTong;

                        row["NhanTP"] = SLNhanTP;
                        row["NhanDT"] = SLNhanDT;
                        row["NhanNK"] = SLNhanNK;

                        row["ConLaiTP"] = SLTong - SLNhanTP;
                        row["ConLaiDT"] = SLTong - SLNhanDT;
                        row["ConLaiNK"] = SLTong - SLNhanNK;

                        db.Rows.InsertAt(row, i);

                        malenhSS = maLenhIndex;
                    }
                }
                DataRow _row = db.NewRow();
                _row["MaLenh"] = malenhSS;
                _row["NgayGH"] = "Sum";

                int _SLTong = SumColumn(db, malenhSS, "SoLuong");
                int _SLNhanTP = SumColumn(db, malenhSS, "NhanTP");
                int _SLNhanDT = SumColumn(db, malenhSS, "NhanDT");
                int _SLNhanNK = SumColumn(db, malenhSS, "NhanNK");

                _row["SoLuong"] = _SLTong;

                _row["NhanTP"] = _SLNhanTP;
                _row["NhanDT"] = _SLNhanDT;
                _row["NhanNK"] = _SLNhanNK;

                _row["ConLaiTP"] = _SLTong - _SLNhanTP;
                _row["ConLaiDT"] = _SLTong - _SLNhanDT;
                _row["ConLaiNK"] = _SLTong - _SLNhanNK;

                db.Rows.InsertAt(_row, db.Rows.Count);

                string Path = HttpContext.Current.Server.MapPath(@"\Templates\TemplateBaoCaoDongThung.xlsx");
                FileInfo file = new FileInfo(Path);

                string fileName = $"Báo cáo nhận TP-ĐT-NK {DateTime.Now.ToString("dd-MM-yyyy-hhmmss")}";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage package = new ExcelPackage(file))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Report"];
                    worksheet.Cells.Style.Font.Name = "Times New Roman";

                    worksheet.Cells["D2"].Value = slLenh;
                    int index = 5;
                    for (int i = 0; i < db.Rows.Count; i++)// +6
                    {
                        index++;
                        if (db.Rows[i]["NgayGH"].ToString().ToLower() == "sum")
                        {
                            worksheet.Cells["A" + index + ":F" + index].Merge = true;
                            worksheet.Cells["A" + index + ":F" + index].Value = string.Format(@"Tổng lệnh {0} (Mã hàng: {1})",
                                 db.Rows[i]["MaLenh"].ToString(), db.Rows[i - 1]["MaHang"].ToString()
                                );
                            worksheet.Cells["G" + index].Value = db.Rows[i]["SoLuong"].ToString();
                            worksheet.Cells["H" + index].Value = db.Rows[i]["NhanTP"].ToString();
                            worksheet.Cells["I" + index].Value = db.Rows[i]["ConLaiTP"].ToString();
                            worksheet.Cells["J" + index].Value = db.Rows[i]["NhanDT"].ToString();
                            worksheet.Cells["K" + index].Value = db.Rows[i]["ConLaiDT"].ToString();
                            worksheet.Cells["L" + index].Value = db.Rows[i]["NhanNK"].ToString();
                            worksheet.Cells["M" + index].Value = db.Rows[i]["ConLaiNK"].ToString();
                        }
                        else
                        {
                            worksheet.Cells["A" + index].Value = db.Rows[i]["MaLenh"].ToString();
                            worksheet.Cells["B" + index].Value = db.Rows[i]["PO"].ToString();
                            worksheet.Cells["C" + index].Value = db.Rows[i]["MaHang"].ToString();
                            worksheet.Cells["D" + index].Value = db.Rows[i]["KhachHang"].ToString();
                            worksheet.Cells["E" + index].Value = db.Rows[i]["QuocGia"].ToString();
                            worksheet.Cells["F" + index].Value = db.Rows[i]["NgayGH"].ToString();
                            worksheet.Cells["G" + index].Value = db.Rows[i]["SoLuong"].ToString();
                            worksheet.Cells["H" + index].Value = db.Rows[i]["NhanTP"].ToString();
                            worksheet.Cells["I" + index].Value = db.Rows[i]["ConLaiTP"].ToString();
                            worksheet.Cells["J" + index].Value = db.Rows[i]["NhanDT"].ToString();
                            worksheet.Cells["K" + index].Value = db.Rows[i]["ConLaiDT"].ToString();
                            worksheet.Cells["L" + index].Value = db.Rows[i]["NhanNK"].ToString();
                            worksheet.Cells["M" + index].Value = db.Rows[i]["ConLaiNK"].ToString();
                        }
                    }
                    // VẼ LẠI GIAO DIỆN
                    worksheet.Cells["A4:M" + index].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A4:M" + index].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A4:M" + index].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells["A4:M" + index].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    // G H I J K L M
                    worksheet.Cells["H6:M" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["H6:M" + index].Style.Fill.BackgroundColor.SetColor(Color.White); // Màu nền
                    worksheet.Cells["H6:M" + index].Style.Font.Color.SetColor(Color.DarkGreen); // Màu chữ

                    worksheet.Cells["G6:G" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["G6:G" + index].Style.Fill.BackgroundColor.SetColor(Color.White);
                    worksheet.Cells["G6:G" + index].Style.Font.Bold = true; // Màu chữ

                    worksheet.Cells["I6:I" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["I6:I" + index].Style.Fill.BackgroundColor.SetColor(Color.White);// Màu nền
                    worksheet.Cells["I6:I" + index].Style.Font.Color.SetColor(Color.Red); // Màu chữ
                    worksheet.Cells["K6:K" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["K6:K" + index].Style.Fill.BackgroundColor.SetColor(Color.White);
                    worksheet.Cells["K6:K" + index].Style.Font.Color.SetColor(Color.Red); // Màu chữ
                    worksheet.Cells["M6:M" + index].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells["M6:M" + index].Style.Fill.BackgroundColor.SetColor(Color.White);
                    worksheet.Cells["M6:M" + index].Style.Font.Color.SetColor(Color.Red); // Màu chữ

                    for (int i = 6; i <= index; i++)
                    {
                        if (worksheet.Cells[string.Format(@"A{0}:F{0}", i)].Merge == true)
                        {
                            worksheet.Cells[string.Format(@"A{0}:M{0}", i)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[string.Format(@"A{0}:M{0}", i)].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 242, 204)); // Màu nền
                            worksheet.Cells[string.Format(@"A{0}:M{0}", i)].Style.Font.Color.SetColor(Color.Red); // Màu chữ
                            worksheet.Cells[string.Format(@"A{0}:M{0}", i)].Style.Font.Bold = true;
                        }
                    }
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
        public string createQuerry(string stored, string sqlPara)
        {
            string result = string.Format(@"EXEC {0} {1}", stored, sqlPara);
            return result;
        }
        public string createQuerryDH(string action, string dvsx, string para)
        {
            string result = string.Format(@"EXEC SP_SendDataDongThungToClient @Action = '{0}', @maDVSX = '{1}' , @sqlPara = '{2}'", action, dvsx, para);
            return result;
        }
        //===================================================================
        private int SumColumn(DataTable db, string maLenh, string fieldName)
        {
            int result = 0;
            foreach (DataRow row in db.Rows)
            {
                if (row["MaLenh"].ToString() == maLenh)
                    result += Convert.ToInt32(row[fieldName].ToString());
            }
            return result;
        }
    }
}