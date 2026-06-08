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
    public class BaoCaoTienDoController : ApiController
    {
        public static HttpClientExtension _clientExtension = new HttpClientExtension();
        [HttpGet]
        [Route("BaoCaoTienDo_XuatExcel")]
        public HttpResponseMessage BCChatLuong(string branch, string storeName, string sqlPara)
        {
            try
            {
                string SV_api = ConfigurationManager.AppSettings[branch].ToString();
                string sqlQuery_Str = string.Format(@"EXEC {0} {1}", storeName, sqlPara);
                DataTable dtExcel = getDBFromOtherServer(SV_api, sqlQuery_Str);

                string imagePathRelative = "/Templates/BAOCAOTIENDO_SX.xlsx";
                string imagePathPhysical = HttpContext.Current.Server.MapPath("~" + imagePathRelative);

                DateTime currentTime = DateTime.Now;

                FileInfo templateFile = new FileInfo(imagePathPhysical);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["BAOCAOTIENDOSX"];
                    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";

                    int countRow = 11, _oldRow = 0, _row14_21 = 0, _rowMerge = 0, _rowwMerge_MH = 0;
                    string _lineName = string.Empty, _odlLineNameMerge = string.Empty, _oldmaHang = string.Empty; ;
                    for (int i = 0; i < dtExcel.Rows.Count; i++)
                    {
                        if (_oldRow == 0)
                        {
                            _oldRow = countRow;
                        }
                        _lineName = dtExcel.Rows[i]["Name"].ToString();
                        if (_odlLineNameMerge != dtExcel.Rows[i]["Name"].ToString())
                        {
                            _odlLineNameMerge = dtExcel.Rows[i]["Name"].ToString();
                            _rowMerge = countRow;
                            if (_oldmaHang != dtExcel.Rows[i]["MaHang"].ToString())
                            {
                                _rowwMerge_MH = countRow;
                                _oldmaHang = dtExcel.Rows[i]["MaHang"].ToString();
                            }
                        }
                        else
                        {
                            worksheet.Cells[_rowMerge, 1, countRow, 1].Merge = true;
                            worksheet.Cells[_rowMerge, 2, countRow, 2].Merge = true;
                            worksheet.Cells[_rowMerge, 3, countRow, 3].Merge = true;

                            if (_oldmaHang != dtExcel.Rows[i]["MaHang"].ToString())
                            {
                                _rowwMerge_MH = countRow;
                                _oldmaHang = dtExcel.Rows[i]["MaHang"].ToString();
                            }
                            else
                            {
                                worksheet.Cells[countRow, 4].Merge = true;
                            }

                        }
                        int cuLK = Convert.ToInt32(dtExcel.Rows[i]["BTP_LK"]);
                        worksheet.Cells[countRow, 1].Value = dtExcel.Rows[i]["Name"];
                        worksheet.Cells[countRow, 2].Value = dtExcel.Rows[i]["NoWorked"];
                        worksheet.Cells[countRow, 3].Value = dtExcel.Rows[i]["HC"];
                        worksheet.Cells[countRow, 4].Value = dtExcel.Rows[i]["MaHang"];
                        worksheet.Cells[countRow, 5].Value = dtExcel.Rows[i]["MaMau"];
                        worksheet.Cells[countRow, 6].Value = dtExcel.Rows[i]["SLKH"];
                        worksheet.Cells[countRow, 7].Value = dtExcel.Rows[i]["SLDLL"];
                        worksheet.Cells[countRow, 8].Value = dtExcel.Rows[i]["SLTT"];
                        worksheet.Cells[countRow, 9].Value = dtExcel.Rows[i]["NgayCat"];
                        worksheet.Cells[countRow, 10].Value = dtExcel.Rows[i]["SLCut_TH"];
                        worksheet.Cells[countRow, 11].Value = dtExcel.Rows[i]["SLCut_LK"];
                        worksheet.Cells[countRow, 12].Value = dtExcel.Rows[i]["NgayBTP"];
                        worksheet.Cells[countRow, 13].Value = dtExcel.Rows[i]["BTP_TH"];
                        worksheet.Cells[countRow, 14].Value = dtExcel.Rows[i]["BTP_LK"];
                        worksheet.Cells[countRow, 15].Value = Convert.ToInt32(dtExcel.Rows[i]["SLCut_LK"]) - Convert.ToInt32(dtExcel.Rows[i]["BTP_LK"]);
                        worksheet.Cells[countRow, 16].Value = dtExcel.Rows[i]["DonGia"];
                        worksheet.Cells[countRow, 17].Value = dtExcel.Rows[i]["RC_HDTH"];
                        worksheet.Cells[countRow, 18].Value = dtExcel.Rows[i]["RC_HDLK"];
                        worksheet.Cells[countRow, 19].Value = dtExcel.Rows[i]["DT_TH"];
                        worksheet.Cells[countRow, 20].Value = dtExcel.Rows[i]["DT_LK"];
                        worksheet.Cells[countRow, 21].Value = dtExcel.Rows[i]["KH_ConLai"];
                        worksheet.Cells[countRow, 22].Value = dtExcel.Rows[i]["NTP_TH"];
                        worksheet.Cells[countRow, 23].Value = dtExcel.Rows[i]["NTP_LK"];
                        worksheet.Cells[countRow, 24].Value = dtExcel.Rows[i]["Ton_TPChuyen"];
                        worksheet.Cells[countRow, 25].Value = dtExcel.Rows[i]["UI_TH"];
                        worksheet.Cells[countRow, 26].Value = dtExcel.Rows[i]["UI_LK"];
                        worksheet.Cells[countRow, 27].Value = dtExcel.Rows[i]["Ton_UI"];
                        worksheet.Cells[countRow, 28].Value = dtExcel.Rows[i]["SU_TH"];
                        worksheet.Cells[countRow, 29].Value = dtExcel.Rows[i]["SU_LK"];
                        worksheet.Cells[countRow, 30].Value = dtExcel.Rows[i]["LKHangDat"];
                        worksheet.Cells[countRow, 31].Value = dtExcel.Rows[i]["TonKiem"];
                        worksheet.Cells[countRow, 32].Value = dtExcel.Rows[i]["HangHu"];
                        worksheet.Cells[countRow, 33].Value = dtExcel.Rows[i]["GX_TH"];
                        worksheet.Cells[countRow, 34].Value = dtExcel.Rows[i]["GX_LK"];
                        worksheet.Cells[countRow, 35].Value = dtExcel.Rows[i]["Ton_TPHT"];
                        worksheet.Cells[countRow, 36].Value = dtExcel.Rows[i]["DT_THHT"];
                        worksheet.Cells[countRow, 37].Value = dtExcel.Rows[i]["DT_LKHT"];
                        worksheet.Cells[countRow, 38].Value = dtExcel.Rows[i]["ChenhLech"];

                        worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Size = 14;

                        countRow++;
                        if (i != dtExcel.Rows.Count - 1)
                        {
                            if (_lineName != dtExcel.Rows[i + 1]["Name"].ToString())
                            {
                                if (!string.IsNullOrEmpty(_lineName))
                                {
                                    worksheet.Cells[countRow, 1].Value = "TOTAL";
                                    worksheet.Cells[countRow, 1, countRow, 4].Merge = true;
                                    for (int col = 6; col < 39; col++)
                                    {
                                        if (col != 9 && col != 12)
                                        {
                                            worksheet.Cells[countRow, col].Formula = "=Sum(" + worksheet.Cells[_oldRow, col].Address + ":" + worksheet.Cells[countRow - 1, col].Address + ")";
                                        }
                                    }
                                    worksheet.Cells[countRow, 1, countRow, 38].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[countRow, 1, countRow, 38].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                    worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                    worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Bold = true;
                                    worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Size = 12;
                                    Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#FFFBE9");
                                    worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.BackgroundColor.SetColor(colFromHex);
                                    worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Size = 14;
                                    //worksheet.Cells[countRow, 1, countRow, 37].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);

                                    countRow++;


                                    _oldRow = 0;
                                }
                            }
                        }
                        else
                        {
                            worksheet.Cells[countRow, 1].Value = "TOTAL";
                            worksheet.Cells[countRow, 1, countRow, 4].Merge = true;
                            for (int col = 6; col < 39; col++)
                            {
                                if (col != 9 && col != 12)
                                {
                                    worksheet.Cells[countRow, col].Formula = "=Sum(" + worksheet.Cells[_oldRow, col].Address + ":" + worksheet.Cells[countRow - 1, col].Address + ")";
                                }
                            }
                            worksheet.Cells[countRow, 1, countRow, 38].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            worksheet.Cells[countRow, 1, countRow, 38].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Bold = true;
                            worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Size = 12;
                            Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#FFFBE9");
                            worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.BackgroundColor.SetColor(colFromHex);
                            worksheet.Cells[countRow, 1, countRow, 38].Style.Font.Size = 14;
                            countRow++;

                            Color colFromHex1 = System.Drawing.ColorTranslator.FromHtml("#C4DFDF");
                            worksheet.Cells[countRow, 1].Value = "TOTAL " + dtExcel.Rows[0]["Name"].ToString() + " -> " + dtExcel.Rows[i]["Name"].ToString();
                            worksheet.Cells[countRow, 1].Style.Font.Bold = true;
                            worksheet.Cells[countRow, 1].Style.Font.Size = 14;
                            worksheet.Cells[countRow, 1, countRow, 4].Merge = true;

                            worksheet.Cells[countRow, 5].Value = 1;
                            worksheet.Cells[countRow, 5].Style.Font.Color.SetColor(colFromHex1);
                            for (int col = 6; col < 39; col++)
                            {
                                if (col != 9 && col != 12)
                                {
                                    worksheet.Cells[countRow, col].Formula = "=SumIF(" + worksheet.Cells[11, 1].Address + ":" + worksheet.Cells[countRow - 1, 5].Address + ",\"TOTAL\"," + worksheet.Cells[11, col].Address + ":" + worksheet.Cells[countRow - 1, col].Address + ")";

                                    worksheet.Cells[countRow, col].Style.Font.Bold = true;
                                    worksheet.Cells[countRow, col].Style.Font.Size = 14;

                                    _row14_21 = countRow + 1;
                                }
                            }

                            worksheet.Cells[countRow, 1, countRow, 38].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            worksheet.Cells[countRow, 1, countRow, 38].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.BackgroundColor.SetColor(colFromHex1);
                            countRow++;
                            //
                            worksheet.Cells[countRow, 1].Value = "TOTAL " + dtExcel.Rows[0]["Name"].ToString() + " -> " + dtExcel.Rows[i]["Name"].ToString();
                            worksheet.Cells[countRow, 1].Style.Font.Bold = true;
                            worksheet.Cells[countRow, 1].Style.Font.Size = 14;
                            worksheet.Cells[countRow, 1, countRow, 4].Merge = true;
                            for (int col = 6; col < 39; col++)
                            {
                                if (col != 9 && col != 12)
                                {
                                    worksheet.Cells[countRow, col].Formula = "=SumIF(" + worksheet.Cells[11, 5].Address + ":" + worksheet.Cells[countRow - 1, 5].Address + ",\"1\"," + worksheet.Cells[11, col].Address + ":" + worksheet.Cells[countRow - 1, col].Address + ")";

                                    worksheet.Cells[countRow, col].Style.Font.Bold = true;
                                    worksheet.Cells[countRow, col].Style.Font.Size = 14;

                                }
                            }
                            worksheet.Cells[countRow, 1, countRow, 38].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            worksheet.Cells[countRow, 1, countRow, 38].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            Color colFromHex2 = System.Drawing.ColorTranslator.FromHtml("#B3C890");
                            worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[countRow, 1, countRow, 38].Style.Fill.BackgroundColor.SetColor(colFromHex2);

                            _oldRow = 0;
                            countRow++;
                        }

                    }

                    worksheet.Cells[4, 1].Value = "DAILY PRODUCTION REPORT - Date: ";// + fromDate.Day + "-" + fromDate.Month + "-" + fromDate.Year;
                    var border_gc = worksheet.Cells[11, 1, countRow - 1, 38].Style.Border;
                    border_gc.Bottom.Style =
                        border_gc.Top.Style =
                        border_gc.Left.Style =
                        border_gc.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;



                    byte[] fileBytes = package.GetAsByteArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"BCTienDoSX{currentTime.ToString("dd/MM/yyyy")}.xlsx";

                    // Create a response message with the file as content
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
                return null;
            }

        }


        //==================================================================================
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
    }
}