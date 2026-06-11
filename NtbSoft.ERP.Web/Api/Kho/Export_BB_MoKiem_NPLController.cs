using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Web.Hosting;
using NtbSoft.ERP.Model.Kho;

namespace NtbSoft.ERP.Web.Api.Kho
{
    [RoutePrefix("api/BBMoKienNPL_Export")]
    public class Export_BB_MoKiem_NPLController : ApiController
    {
        private readonly Erp_Export_BBMoKien_NPLModel _model = new Erp_Export_BBMoKien_NPLModel();
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para1 = "NONE", string para2 = "NONE", string para3 = "NONE", string para4 = "NONE")
        {
            return _model.Get(action, para1, para2, para3, para4);
        }
        [HttpPost]
        [Route("ExportBB")]
        public async Task<HttpResponseMessage> ExportBB(JObject ObjExport)
        {
            try
            {
                

                int rowStart = 10; int rowIdx = 10;
                string SoLo = ObjExport["SoLo"]?.ToString();
                bool IsNL = ObjExport["IsNL"]?.ToString()?.ToLower() == "true";
                bool IsHaiQuan = ObjExport["IsHaiQuan"]?.ToString()?.ToLower() == "true";

                //string templatePath = HttpContext.Current.Server.MapPath(@"\Content\Templates\TemplatePhieuTestNut.xlsx");

                string strPathLoGo = "/img/DONAGAMEX_Logo.webp";

              
                DateTime.TryParse(ObjExport["toDate"]?.ToString(), out DateTime toDate);
                DateTime.TryParse(ObjExport["fromDate"]?.ToString(), out DateTime fromDate);
                DataTable tbl_BBMoKien = _model.Get("Get", SoLo, IsNL.ToString(), toDate.ToString("yyyy-MM-dd"), fromDate.ToString("yyyy-MM-dd"));
                //FileInfo templateFile = new FileInfo(templatePath);


                string fileName = $"`BB-MoKien-NPL-{DateTime.Now.ToString("ddMMyyyyhhmmss")} " + ".xlsx";


                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage package = new ExcelPackage())
                {
                    await Task.Run(() =>
                    {
                        if (tbl_BBMoKien != null && tbl_BBMoKien.Rows.Count > 0)
                        {
                            var groupedResult = tbl_BBMoKien.AsEnumerable()
                                .GroupBy(row => new
                                {
                                    NgayNhapKho = DateTime.TryParse(row["NgayNhapKho"]?.ToString(), out var dt) ? dt.Date : DateTime.MinValue,
                                    SoLoID = row["SoLoID"]?.ToString(),
                                    SoLo = row["SoLo"]?.ToString()
                                })
                                .Select(g => new
                                {
                                    NgayNhapKho = g.Key.NgayNhapKho,
                                    SoLoID = g.Key.SoLoID,
                                    SoLo = g.Key.SoLo,
                                    DsNhapKho = g.ToList()
                                })
                                .ToList();

                            foreach (var group in groupedResult)
                            {
                                string sheetName = $"{group.SoLo}_{group.NgayNhapKho:ddMMyy}".Trim();
                                if (sheetName.Length > 31) sheetName = sheetName.Substring(0, 31);

                                string finalSheetName = sheetName;
                                int duplicateIndex = 1;
                                while (package.Workbook.Worksheets.Any(ws => ws.Name == finalSheetName))
                                {
                                    finalSheetName = $"{sheetName}_{duplicateIndex++}";
                                }

                                var worksheet = package.Workbook.Worksheets.Add(finalSheetName);
                                worksheet.Cells.Style.Font.Name = "Times New Roman";
                                worksheet.Cells.Style.Font.Size = 12;


                                // Tiêu đề                                
                                worksheet.Cells["A1:F1"].Merge = true;
                                worksheet.Cells["A1"].Value = "";
                                worksheet.Cells["A1"].Style.Font.Bold = true;
                                worksheet.Cells["A1"].Style.Font.Size = 13;
                                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                //worksheet.Cells["A2:B2"].Merge = true;                            
                               // Addpicutre(worksheet, strPathLoGo, 1, 0,0, 100, 50);


                                //worksheet.Cells["H2:M2"].Merge = true;
                                worksheet.Cells["H2"].Value = "PHIẾU NHẬP KHO";
                                worksheet.Cells["H2"].Style.Font.Bold = true;
                                worksheet.Cells["H2"].Style.Font.Size = 13;
                                worksheet.Cells["H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["H2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                worksheet.Cells["H3"].Value = "Ngày  tháng  năm  ";
                                if (group.NgayNhapKho != DateTime.MinValue)
                                {
                                    worksheet.Cells["H3"].Value = $"Ngày {group.NgayNhapKho.Day:D2} tháng {group.NgayNhapKho.Month:D2} năm {group.NgayNhapKho.Year}"; ;
                                }
                              
                          /*      worksheet.Cells["H3"].Style.Font.Bold = true;*/
                                worksheet.Cells["H3"].Style.Font.Size = 12;
                                worksheet.Cells["H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                //worksheet.Cells["M2:P2"].Merge = true;
                                worksheet.Cells["M2"].Value = "Mẫu số 1-VT";
                                worksheet.Cells["M2"].Style.Font.Italic = true;
                                worksheet.Cells["M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells["M2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                worksheet.Cells["A4:B4"].Merge = true;
                                worksheet.Cells["A4"].Value = "Nhà cung cấp:";


                                worksheet.Cells["A5:B5"].Merge = true;
                                worksheet.Cells["A5"].Value = "Chứng từ số:";
                                worksheet.Cells["C5"].Value = group.DsNhapKho[0]["SoChungTu"]?.ToString();
                               

                                worksheet.Cells["H5"].Value = "Ngày  tháng  năm  ";
                                if (group.DsNhapKho[0]["NgayChungTu"]?.ToString() != null)
                                {
                                    DateTime.TryParse(group.DsNhapKho[0]["NgayChungTu"]?.ToString(), out DateTime NgayChungTu);
                                    worksheet.Cells["H5"].Value = NgayChungTu ==  DateTime.MinValue ? "" : $"Ngày {NgayChungTu.Day:D2} tháng {NgayChungTu.Month:D2} năm {NgayChungTu.Year}";
                                }
                                worksheet.Cells["H5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["H5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                worksheet.Cells["A6:B6"].Merge = true;
                                worksheet.Cells["A6"].Value = "Biên bản kiểm nghiệm số:";
                                worksheet.Cells["C6"].Value = group.DsNhapKho[0]["SoBienBan"]?.ToString();

                                worksheet.Cells["H6"].Value = "Ngày  tháng  năm  ";
                                if (group.DsNhapKho[0]["NgayBienBan"]?.ToString() != null)
                                {
                                    DateTime.TryParse(group.DsNhapKho[0]["NgayBienBan"]?.ToString(), out DateTime NgayBienBan);
                                    worksheet.Cells["H6"].Value = NgayBienBan == DateTime.MinValue  ? "" :$"Ngày {NgayBienBan.Day:D2} tháng {NgayBienBan.Month:D2} năm {NgayBienBan.Year}";
                                }
                                worksheet.Cells["H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["H6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                worksheet.Cells["A7:B7"].Merge = true;
                                worksheet.Cells["A7"].Value = "Số Hợp đồng:";
                                worksheet.Cells["C7"].Value = group.DsNhapKho[0]["SoHopDong"]?.ToString();

                                worksheet.Cells["A8:B8"].Merge = true;
                                worksheet.Cells["A8"].Value = "PI NCC:";

                                worksheet.Cells["C8"].Value = group.SoLo;
                                worksheet.Cells["C8"].Style.Font.Bold = true;
                                worksheet.Cells["C8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["C8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                //worksheet.Cells["H4:J4"].Merge = true;
                                worksheet.Cells["H4"].Style.Font.Size = 13;
                                worksheet.Cells["H4"].Value = group.DsNhapKho[0]["TenNCC"]?.ToString();
                                worksheet.Cells["H4"].Style.Font.Bold = true;
                                worksheet.Cells["H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["H4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                                worksheet.Cells["M4:P4"].Merge = true;
                                worksheet.Cells["M4"].Value = "Số: 58/NLNĐ";
                                worksheet.Cells["M4"].Style.Font.Bold = true;

                                worksheet.Cells["M5"].Value = "Nợ";
                                worksheet.Cells["M6"].Value = "Có";

                                worksheet.Cells["H8"].Value = string.Format("Nhập vào kho : {0}", IsNL ? "NPL" : "PL");
                                worksheet.Cells["H8"].Style.Font.Bold = true;
                                worksheet.Cells["H8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells["H8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                // Tiêu đề bảng
                                worksheet.Cells["A9"].Value = "STT";
                                worksheet.Cells["B9"].Value = "Tên hàng hóa";
                                worksheet.Cells["B9:F9"].Merge = true;

                                worksheet.Cells["G9"].Value = "ĐVT";
                                worksheet.Cells["H9"].Value = "Theo CT";
                                worksheet.Cells["I9"].Value = "Thực nhập";
                                worksheet.Cells["J9"].Value = "Đơn giá";
                                worksheet.Cells["K9"].Value = "Thành tiền";
                                worksheet.Cells["L9"].Value = "Mã HQ";
                                worksheet.Cells["M9"].Value = "Ghi chú";


                                // Canh giữa và đậm
                                using (var range = worksheet.Cells["A9:M9"])
                                {
                                    range.Style.Font.Bold = true;
                                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                }


                                worksheet.Column(11).AutoFit();
                                worksheet.Column(11).Width += 10;

                              
                            
                                worksheet.Column(9).Width= 15;
                                worksheet.Column(8).Width = 15;


                                worksheet.Column(1).Width = 12;
                                worksheet.Column(2).Width = 12; // B
                                worksheet.Column(3).Width = 12; // C
                                worksheet.Column(4).Width = 12; // D
                                worksheet.Column(5).Width = 12; // E
                                worksheet.Column(6).Width = 12;
                     

                                // Duyệt dòng dữ liệu nhóm
                                int Stt = 0; rowIdx = 10;
                                foreach (DataRow row in group.DsNhapKho)
                                {
                                    Stt++;
                               


                                    worksheet.Cells[rowIdx, 1].Value = Stt;

                                    worksheet.Cells[rowIdx, 2].Value = row["TenHH"]?.ToString();
                                    var cell = worksheet.Cells[$"B{rowIdx}:F{rowIdx}"];
                                    cell.Merge = true;
                                    cell.Style.WrapText = true;

                                
                               

                                    worksheet.Cells[rowIdx, 2].Style.WrapText = true;

                                    worksheet.Cells[rowIdx, 7].Value = row["TenDVVT"]?.ToString();

                             
                                    float.TryParse(row["SoLuongChungTu"]?.ToString(), out float SLChungTu);
                                    float.TryParse(row["SoLuongThucTe"]?.ToString(), out float SLThucTe);

                                
                                    worksheet.Cells[rowIdx, 8].Value = SLChungTu;
                                    worksheet.Cells[rowIdx, 8].Style.Numberformat.Format =
                                        (SLChungTu % 1 == 0) ? "#,##0" : "#,##0.00"; // Nếu số nguyên thì không có phần thập phân

                                   
                                    float value = IsHaiQuan ? SLChungTu : SLThucTe;
                                    worksheet.Cells[rowIdx, 9].Value = value;
                                    worksheet.Cells[rowIdx, 9].Style.Numberformat.Format =
                                        (value % 1 == 0) ? "#,##0" : "#,##0.00"; // Format tùy theo phần thập phân




                                
                                    float.TryParse(row["DonGia"]?.ToString(), out float donGia);

                                
                                    worksheet.Cells[rowIdx, 10].Value = donGia;
                                    worksheet.Cells[rowIdx, 10].Style.Numberformat.Format =
                                        (donGia % 1 == 0) ? "#,##0" : "#,##0.00";


                                    worksheet.Cells[rowIdx, 11].Formula = $"=I{rowIdx}*J{rowIdx}";
                                    worksheet.Cells[rowIdx, 11].Style.Numberformat.Format = 
                                    ((value* donGia) % 1 == 0) ? "#,##0" : "#,##0.00";

                                    worksheet.Cells[rowIdx, 12].Value = row["MaHaiQuan"]?.ToString();
                                    worksheet.Cells[rowIdx, 13].Value = row["GhiChu"]?.ToString();

                                    worksheet.Row(rowIdx).CustomHeight = true;
                                    worksheet.Row(rowIdx).Height = 30;
                                    rowIdx++;
                                }
                        
                                worksheet.Cells[$"B{rowIdx}:F{rowIdx}"].Merge = true;
                                worksheet.Cells[rowIdx, 2].Value = "Tổng cộng";
                                worksheet.Cells[$"J{rowIdx}"].Value = "$";
                               

                                worksheet.Cells[$"K{rowIdx}"].Formula = $"=Sum(K{rowStart}:K{rowIdx-1})";
                                worksheet.Calculate();
                                var sumValue = worksheet.Cells[$"K{rowIdx}"].Value;
                                if (sumValue != null && double.TryParse(sumValue.ToString(), out double result))
                                {
                                    string format = (result % 1 == 0) ? "#,##0" : "#,##0.00";
                                    worksheet.Cells[$"K{rowIdx}"].Style.Numberformat.Format = format;
                                }




                                //worksheet.Cells[$"K{rowIdx}"].Style.Numberformat.Format = "#,##0.00";

                                worksheet.Cells[rowIdx + 1, 1].Value = "Cộng thành tiền (viết bằng chữ) :";

                                worksheet.Cells[rowIdx + 2, 1, rowIdx + 2, 2].Merge = true;
                                worksheet.Cells[rowIdx + 2, 1].Value = "TỔNG GIÁM ĐỐC";
                                worksheet.Cells[rowIdx + 2, 1].Style.Font.Size = 12;
                                worksheet.Cells[rowIdx + 2, 1].Style.Font.Bold = true;
                                worksheet.Cells[rowIdx + 2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                worksheet.Cells[rowIdx + 2, 4, rowIdx + 2, 5].Merge = true;
                               worksheet.Cells[rowIdx + 2, 4].Value = "PHÒNG KẾ HOẠCH";
                                worksheet.Cells[rowIdx + 2, 4].Style.Font.Size = 12;
                                worksheet.Cells[rowIdx + 2, 4].Style.Font.Bold = true;
                                worksheet.Cells[rowIdx + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                                worksheet.Cells[rowIdx + 2, 7, rowIdx + 2, 8].Merge = true;
                                worksheet.Cells[rowIdx + 2, 7].Value = "THỦ KHO";
                                worksheet.Cells[rowIdx + 2, 7].Style.Font.Size = 12;
                                worksheet.Cells[rowIdx + 2, 7].Style.Font.Bold = true;
                                worksheet.Cells[rowIdx + 2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                                worksheet.Cells[rowIdx + 2, 10, rowIdx + 2, 11].Merge = true;
                                worksheet.Cells[rowIdx + 2, 10].Value = "NGƯỜI GIAO";
                                worksheet.Cells[rowIdx + 2, 10].Style.Font.Size = 12;
                                worksheet.Cells[rowIdx + 2, 10].Style.Font.Bold = true;
                                worksheet.Cells[rowIdx + 2, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                                var borderRange = worksheet.Cells[rowStart, 1, rowIdx, 13];
                                borderRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                borderRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                borderRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                borderRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                borderRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                borderRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                               
                            }
                        }
                    });

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

                Console.WriteLine("An error occurred: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        private void Addpicutre(ExcelWorksheet worksheet, string sign, int row, int col, int toado, int width, int height)
        {
            if (!string.IsNullOrEmpty(sign))
            {
                worksheet.Row(row + 1).Height = 50;
                //worksheet.Column(col).Width = 30;S

                string imagePath = HostingEnvironment.MapPath(sign);
                // Chèn ảnh vào worksheet
                if (File.Exists(imagePath))
                {
                    FileInfo imageFile = new FileInfo(imagePath);
                    ExcelPicture picture = worksheet.Drawings.AddPicture(Guid.NewGuid().ToString(), imageFile);
                    picture.SetPosition(row, 5, col, toado);
                    picture.SetSize(width, height);
                }
            }

        }

    }
}