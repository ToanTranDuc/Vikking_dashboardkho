using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using NtbSoft.ERP.Model.POMuaHang;
using System.Collections.Generic;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Entity.POMuaHang;
using System.Net.Http;
using System.Net;
using OfficeOpenXml;
using System.Web.Hosting;
using System.Linq;
using NtbSoft.ERP.Utils;
using OfficeOpenXml.Style;
using System.Net.Http.Headers;
using System.Drawing;
using System.Web;

namespace NtbSoft.ERP.Web.Api.POMuaHang
{
    [RoutePrefix("api/PhieuDanhGiaNhaCC")]
    public class ERPDanhGiaNhaCCController : ApiController
    {
        private ERPDanhGiaNhaCCModel _model = new ERPDanhGiaNhaCCModel();
        [HttpGet]
        [Route("GET")]
        public async Task<DataTable> GET(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            return await _model.Get(action, para1, para2, para3, para4, para5);
        }


        [HttpDelete]
        [Route("Delete")]
        public async Task<string> Delete(string action, string Para1, string Para2 = null, string Para3 = null)
        {
            Para1 = Para1 ?? "NONE";
            Para2 = Para2 ?? "NONE";
            Para3 = Para3 ?? "NONE";
            return await _model.Delete(action, Para1, Para2, Para3);
        }

        [HttpPost]
        [Route("ImportTCNoiLV")]
        public async Task<string> ImportTCNoiLV([FromBody] string Json)
        {

            DataTable dsSave = JsonConvert.DeserializeObject<DataTable>(Json);
            return await _model.ImportTCNoiLV(dsSave);
        }

        [HttpPost]
        [Route("UploadSignature")]
        public async Task<List<dynamic>> UploadSignature([FromBody] JArray imageDatas, string getFileName)
        {
            var results = new List<dynamic>();

            try
            {
                string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "DanhGiaNhaCC");
                string formattedDate = DateTime.Now.ToString("ddMMyyyyHHmmss");
                string folderName = getFileName.Replace("|", "-").Replace(" ", "-");
                string newFolderPath = Path.Combine(baseDirectory, folderName);

                if (!Directory.Exists(newFolderPath))
                {
                    Directory.CreateDirectory(newFolderPath);
                }

                var tasks = new List<Task>();

                foreach (var imageData in imageDatas)
                {
                    string imgString = imageData["img"]?.ToString();
                    string imgName = imageData["name"]?.ToString();

                    if (!string.IsNullOrEmpty(imgString) && !string.IsNullOrEmpty(imgName))
                    {
                        string[] imageDataParts = imgString.Split(',');

                        if (imageDataParts.Length == 2)
                        {
                            string base64Data = imageDataParts[1];
                            byte[] imageBytes = Convert.FromBase64String(base64Data);
                            string sanitizedFileName = imgName.Replace(" ", "").Replace("|", "-") + "_" + formattedDate + ".png";
                            string fullImagePath = Path.Combine(newFolderPath, sanitizedFileName);
                            string publicPath = Path.Combine("/Images/DanhGiaNhaCC/", folderName, sanitizedFileName).Replace("\\", "/");

                            tasks.Add(Task.Run(() =>
                            {
                                System.IO.File.WriteAllBytes(fullImagePath, imageBytes);
                                lock (results)
                                {
                                    results.Add(new
                                    {
                                        name = imgName,
                                        url = publicPath
                                    });
                                }
                            }));
                        }
                        else
                        {
                            Console.WriteLine("Invalid image data format.");
                        }
                    }
                }
                await Task.WhenAll(tasks);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading images: {ex.Message}");
            }

            return results;
        }

        [HttpPost]
        [Route("PostPhieuDG")]
        public async Task<string> PostPhieuDG(List<ERPDanhGiaNCCEntity> lstDanhGia)
        {
            if (lstDanhGia == null) return "false";
            string json = JsonConvert.SerializeObject(lstDanhGia);
            DataTable tblSave = JsonConvert.DeserializeObject<DataTable>(json);
            return await _model.PostPhieuDG(tblSave);
        }


        /**/
        [HttpPost]
        [Route("PostPhieuDGNoiLV")]
        public async Task<string> PostPhieuDGNoiLV(List<PhieuDanhGiaNCCNoiLamViecEntity> lstDanhGia)
        {

            if (lstDanhGia == null) return "false";
            string json = JsonConvert.SerializeObject(lstDanhGia);
            DataTable tblSave = JsonConvert.DeserializeObject<DataTable>(json);
            return await _model.PostPhieuDGNoiLV(tblSave);

        }

        [HttpPost]
        [Route("UpdateKetLuan")]
        public async Task<string> UpdateKetLuan(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            return await _model.UpdateKetLuan(action, para1, para2, para3, para4, para5);
        }


        [HttpPost]
        [Route("ExportPhieuTheoDoi")]
        public async Task<HttpResponseMessage> ExportPhieuTheoDoi(string para1, string para2, string para3)
        {
            try
            {
                int rowStart = 9;
                int rowIdx = rowStart;
                string templatePath = System.Web.HttpContext.Current.Server.MapPath(@"\Content\Templates\TemplatePhieuTheoDoi.xlsx");
                int colIdx = 0;
                DataTable tblBC = await _model.Get("GetPhieuDG_TQ", para2, para1, "edit", "TheoDoi", para3);
                DataTable tblNhaCC = await _model.Get("GetNCC", para2, "NONE", "NONE", "NONE", "NONE");
                DataTable tblChungLoai = await _model.Get("GetChungLoaiVT", "NONE", "NONE", "NONE", "NONE", "NONE");
                DataTable tblDiem = await _model.Get("GetDiemDG", "NONE", "NONE", "NONE", "NONE", "NONE");
                FileInfo templateFile = new FileInfo(templatePath);
                string fileName = $"BM04_QT13_CL01_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["BM04"];
                    worksheet.Cells.Style.Font.Name = "Times New Roman";

                    if (tblBC != null && tblBC.Rows.Count > 0)
                    {
                        DateTime ngayDGPhieu = clsForrmatUtils.ConvertDate(tblBC?.Rows[0]["NgayDanhGiaPhieu"]?.ToString());
                        var queryCL = tblChungLoai.AsEnumerable().FirstOrDefault(x => x["MaCLVT"]?.ToString() == (tblBC?.Rows[0]["MaChungLoai"]?.ToString()));
                        if(queryCL!= null)
                        {
                            worksheet.Cells["A4"].Value = $"Tên hàng hoá, dịch vụ: {queryCL["ChungLoaiVatTu"]?.ToString()}";
                        }
                        if (ngayDGPhieu == DateTime.MinValue)
                        {
                            worksheet.Cells["A5"].Value = $"Năm :";
                        }
                        else
                        {
                            worksheet.Cells["A5"].Value = $"Năm : {ngayDGPhieu.Year}";
                        }
                        foreach (DataRow row in tblBC.Rows)
                        {
             

                            // Ngày đánh giá
                            DateTime ngayDG = clsForrmatUtils.ConvertDate(row["NgayDanhGia"]?.ToString());
                            worksheet.Cells[rowIdx, 1].Value = ngayDG == DateTime.MinValue ? "" : ngayDG.ToString("dd/MM/yyyy");


                            // Nhà cung cấp: lấy từ TenPhieu → tra tên NCC bằng LINQ
                            string maPhieu = row["TenPhieu"]?.ToString();
                            string tenNCC = tblNhaCC.AsEnumerable()
                                .Where(r => r["MaNhaCC"].ToString() == row["MaNhaCC"]?.ToString())
                                .Select(r => r["TenNCC"].ToString())
                                .FirstOrDefault();
                            worksheet.Cells[rowIdx, 2].Value = tenNCC;
                            colIdx = 2;

                            for (int i = 1; i <= 6; i++)
                            {
                                string prefix = $"{i}@TieuChi@";
                                DataColumn matchedCol = null;
                                foreach (DataColumn col in tblBC.Columns)
                                {
                                    if (col.ColumnName.StartsWith(prefix))
                                    {
                                        colIdx++;
                                        matchedCol = col;
                                        break;
                                    }
                                }

                       
                                if (matchedCol != null)
                                {
                                    string raw = row[matchedCol.ColumnName]?.ToString();
                                    if (!string.IsNullOrEmpty(raw) && raw.StartsWith("Diem_"))
                                    {
                                       if(tblDiem  != null && tblDiem?.Rows?.Count > 0)
                                        {
                                            var Query = tblDiem.AsEnumerable().FirstOrDefault(x=>x["MaDiemDanhGia"]?.ToString() == raw);
                                            if(Query!= null)
                                            {
                                                worksheet.Cells[rowIdx, 2 + i].Value = Query["DiemDanhGia"]?.ToString();
                                            }
                                        }
                                    }
                                }
                                
                            }

                            
                            
                            worksheet.Cells[rowIdx, colIdx+1].Value = row["TongDiemUuTien"]?.ToString();

                            // Ghi chú
                            worksheet.Cells[rowIdx, colIdx + 2].Value = row["GhiChu"]?.ToString();

                            // Kết quả: đánh dấu ☑ nếu true
                            bool ketQuaPass = Convert.ToBoolean(row["KetQuaPass"]);
                            bool ketQuaFail = Convert.ToBoolean(row["KetQuaFail"]);
                            worksheet.Cells[rowIdx, colIdx + 3].Value = ketQuaPass ? "✓" : "";
                            worksheet.Cells[rowIdx, colIdx + 4].Value = ketQuaFail ? "✓" : "";

                            worksheet.Cells[rowIdx, colIdx + 5].Value = row["NguoiDanhGiaPhieu"]?.ToString();

                            worksheet.Row(rowIdx).Height = 35;
                            rowIdx++;
                        }


                        worksheet.Cells[rowIdx + 1, 1].Value = "Ghi chú:";
                        worksheet.Cells[rowIdx + 1, 2].Value = "*Thang điểm: 1: Rất kém, 3: Đạt, 4: Tốt, 5: Rất tốt";
                        worksheet.Cells[rowIdx + 1 + 1, 2].Value = "*Điểm đạt tối thiểu ở mỗi mục là 3; đánh giá theo PL02/QT15/KH02";
                        worksheet.Cells[rowIdx + 1 + 2, 2].Value = "*Kiểm soát định kỳ tối thiểu 1 lần/quý";


                        var borderRange = worksheet.Cells[9, 1, rowIdx - 1, colIdx + 5];
                        borderRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        borderRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        borderRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        borderRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        borderRange.Style.WrapText = true;


                        borderRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        borderRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    }

                    // Xuất file Excel
                    byte[] fileBytes = package.GetAsByteArray();
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExportPhieuTheoDoi error: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Route("ExportPhieuDGNoiLV")]
        public async Task<HttpResponseMessage> ExportPhieuDGNoiLV(string para1, string para2, string para3,string para4)
        {
            try
            {
                int rowStart = 8;
                int rowIdx = rowStart;
                string templatePath = HttpContext.Current.Server.MapPath(@"\Content\Templates\TemplatePhieuDanhGiaNoiLV.xlsx");
                FileInfo templateFile = new FileInfo(templatePath);
                string fileName = $"BM04_QT13_CL01_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";

                DataTable tblBC = await _model.Get("GetPhieuDGNoiLV", para1, para2, "edit", "TheoDoi", para3);
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
                double sumTongDiemToiDa = 0;
                double sumTongDiemDatDuoc = 0;
                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["BM13"];
                    worksheet.Cells.Style.Font.Name = "Times New Roman";

                    if (tblBC != null && tblBC.Rows.Count > 0)
                    {
                        var rows = tblBC.AsEnumerable()
                            .OrderBy(r => Convert.ToInt32(r["Sort"]))
                            .ThenBy(r => r["TenNhomDanhGia"].ToString())
                            .ThenBy(r => NormalizeNo(r["No"]?.ToString()))
                            .ToList();

                    
                        double tongDiemToiDa = 0;
                        double tongDiemDatDuoc = 0;
                        string lastGroupName = string.Empty;
                        var LastGroup = rows.AsEnumerable().LastOrDefault();
                        string currentGroup = "";
                        if (LastGroup != null)
                        {
                            lastGroupName = LastGroup["TenNhomDanhGia"]?.ToString();
                        }
                        worksheet.Cells["A5"].Value = $"Supplier/Nhà cung cấp-nhà thầu:  {para3}";
                        DateTime ngayDGPhieu = clsForrmatUtils.ConvertDate(tblBC?.Rows[0]["NgayDG"]?.ToString());
                       
                        if (ngayDGPhieu == DateTime.MinValue)
                        {
                            worksheet.Cells["A6"].Value = "Date of verification/ Ngày đánh giá: ";
                        }
                        else
                        {
                            worksheet.Cells["A6"].Value = $"Date of verification/ Ngày đánh giá:  {ngayDGPhieu.ToString("dd-MM-yyyy")}";
                        }

                        worksheet.Cells["D6"].Value = $"Người đánh giá: {para4}";
                        foreach (var row in rows)
                        {
                            
                            string groupName = row["TenNhomDanhGia"]?.ToString();
                            if (groupName != currentGroup)
                            {
                                
                                if (!string.IsNullOrEmpty(currentGroup))
                                {
                                    worksheet.Cells[rowIdx, 1].Value = "Total :";
                                    worksheet.Cells[rowIdx, 1, rowIdx, 3].Merge = true;
                                    worksheet.Cells[rowIdx, 1].Style.Font.Bold = true;
                                    worksheet.Cells[rowIdx, 5].Value = tongDiemToiDa;
                                    worksheet.Cells[rowIdx, 6].Value = tongDiemDatDuoc;
                                    worksheet.Cells[rowIdx, 4, rowIdx, 6].Style.Font.Bold = true;
                                    worksheet.Cells[rowIdx, 4, rowIdx, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[rowIdx, 4, rowIdx, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    rowIdx++;

                                    tongDiemToiDa = 0;
                                    tongDiemDatDuoc = 0;
                                }

                                // Tiêu đề nhóm
                                worksheet.Cells[rowIdx, 1].Value = groupName;
                                worksheet.Cells[rowIdx, 1, rowIdx, 6].Merge = true;
                                worksheet.Cells[rowIdx, 1].Style.Font.Bold = true;
                                
                                worksheet.Cells[rowIdx, 1, rowIdx, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[rowIdx, 1, rowIdx, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                worksheet.Cells[rowIdx, 1, rowIdx, 6].Style.WrapText = true;
                                worksheet.Row(rowIdx).Height = 35;
                                rowIdx++;
                                currentGroup = groupName;
                            }

                            worksheet.Cells[rowIdx, 1].Value = row["No"]?.ToString();
                            worksheet.Cells[rowIdx, 2].Value = row["TieuChi"]?.ToString();

                            string apDung = row["ApDung"]?.ToString();
                            worksheet.Cells[rowIdx, 3].Value = apDung == "X" ? "X" : (apDung == "V" ? "✓" : "");

                            double diemToiDa = Convert.ToDouble(row["DiemToiDa"] ?? 0);
                            double diemDatDuoc = Convert.ToDouble(row["DiemDatDuoc"] ?? 0);

                            worksheet.Cells[rowIdx, 5].Value = diemToiDa;
                            worksheet.Cells[rowIdx, 6].Value = diemDatDuoc;
                            worksheet.Cells[rowIdx, 4].Value = row["GhiChu"]?.ToString();

                            sumTongDiemToiDa += diemToiDa;
                            sumTongDiemDatDuoc += diemDatDuoc;
                            tongDiemToiDa += diemToiDa;
                            tongDiemDatDuoc += diemDatDuoc;
                          

                            var borderRange = worksheet.Cells[rowIdx, 1, rowIdx, 6];
                            borderRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            borderRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            borderRange.Style.WrapText = true;


                            var col2Range = worksheet.Cells[rowIdx, 2, rowIdx, 2];
                            col2Range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            col2Range.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                            worksheet.Row(rowIdx).Height = 55;
                            rowIdx++;

                            if(currentGroup== lastGroupName && LastGroup["No"]?.ToString() == row["No"]?.ToString())
                            {
                                worksheet.Cells[rowIdx, 1].Value = "Total :";
                                worksheet.Cells[rowIdx, 1, rowIdx, 3].Merge = true;
                                worksheet.Cells[rowIdx, 1].Style.Font.Bold = true;
                                worksheet.Cells[rowIdx, 5].Value = tongDiemToiDa;
                                worksheet.Cells[rowIdx, 6].Value = tongDiemDatDuoc;
                                worksheet.Cells[rowIdx, 4, rowIdx, 6].Style.Font.Bold = true;

                                worksheet.Cells[rowIdx, 4, rowIdx, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIdx, 4, rowIdx, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            }
                        }

                        // Tổng cuối cùng cho nhóm cuối
                        worksheet.Cells[rowIdx + 1, 1].Value = "TOTAL -TỔNG CỘNG";
                        worksheet.Cells[rowIdx + 1, 1, rowIdx + 1, 3].Merge = true;
                        worksheet.Cells[rowIdx + 1, 1].Style.Font.Bold = true;
                        worksheet.Cells[rowIdx + 1, 5].Value = sumTongDiemToiDa;
                        worksheet.Cells[rowIdx + 1, 6].Value = sumTongDiemDatDuoc;
                        worksheet.Cells[rowIdx + 1, 4, rowIdx + 1, 6].Style.Font.Bold = true;
                        worksheet.Cells[rowIdx + 1, 5,rowIdx + 1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[rowIdx + 1, 5,rowIdx + 1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        rowIdx++;




                        // Range chung cho viền từ cột 1 đến 6
                        var borderRangeAll = worksheet.Cells[rowStart, 1, rowIdx +1, 6];
                        borderRangeAll.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        borderRangeAll.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        borderRangeAll.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        borderRangeAll.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        rowIdx++;

                        // Giải thích kết luận
                        worksheet.Cells[rowIdx, 1].Value = "** Tổng điểm đạt ít nhất 70%/ tổng số điểm: đạt";
                        worksheet.Cells[rowIdx, 1, rowIdx, 6].Merge = true;
                        worksheet.Cells[rowIdx, 1].Style.Font.Bold = true;
                        worksheet.Cells[rowIdx, 1].Style.WrapText = true;
                        worksheet.Cells[rowIdx, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIdx, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        worksheet.Row(rowIdx).Height = 45;
                        rowIdx++;

                        worksheet.Cells[rowIdx, 1].Value = "** Tổng điểm đạt thấp hơn 70%/ tổng số điểm: xem xét lại khả năng hợp tác với NCC";
                        worksheet.Cells[rowIdx, 1, rowIdx, 6].Merge = true;
                        worksheet.Cells[rowIdx, 1].Style.Font.Bold = true;
                        worksheet.Cells[rowIdx, 1].Style.WrapText = true;
                        worksheet.Cells[rowIdx, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIdx, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        worksheet.Row(rowIdx).Height = 45;
                        rowIdx++;

                        // Dòng kết quả tổng thể
                        bool.TryParse(tblBC?.Rows[0]["KetQuaDat"]?.ToString(), out bool KetQuaDat);

                        // Cột 1–3: "Kết quả đánh giá" (merge)
                        worksheet.Cells[rowIdx, 1].Value = "Kết quả đánh giá: " + (KetQuaDat ? "Đạt" : "Không đạt");
                        worksheet.Cells[rowIdx, 1, rowIdx, 3].Merge = true;
                        worksheet.Cells[rowIdx, 1].Style.Font.Bold = true;
                        worksheet.Cells[rowIdx, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[rowIdx, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[rowIdx, 1].Style.WrapText = true;

                                    }


                    byte[] fileBytes = package.GetAsByteArray();
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExportPhieuDGNoiLV error: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

  
        private static string NormalizeNo(string no)
        {
            if (string.IsNullOrEmpty(no)) return "";
            var parts = no.Split('.').Select(p => p.PadLeft(4, '0'));
            return string.Join(".", parts);
        }

        [HttpPost]
        [Route("ExportPhieuDanhGia")]
        public async Task<HttpResponseMessage> ExportPhieuDanhGia(string para1, string para2, string para3)
        {
            try
            {
                int rowStart = 9;
                int rowIdx = rowStart;
                string templatePath = System.Web.HttpContext.Current.Server.MapPath(@"\Content\Templates\TemplatePhieuDanhGia.xlsx");
                int colIdx = 0;
                DataTable tblBC = await _model.Get("GetPhieuDG_TQ", para2, para1, "edit", "DanhGia", para3);
                DataTable tblNhaCC = await _model.Get("GetNCC", para2, "NONE", "NONE", "NONE", "NONE");
                DataTable tblChungLoai = await _model.Get("GetChungLoaiVT", "NONE", "NONE", "NONE", "NONE", "NONE");
                DataTable tblDiem = await _model.Get("GetDiemDG", "NONE", "NONE", "NONE", "NONE", "NONE");
                DataTable tblKetQua = await _model.Get("GetKetQuaDG", para1, "DanhGia", "NONE", "NONE", "NONE");
                FileInfo templateFile = new FileInfo(templatePath);
                string fileName = $"BM04_QT13_CL01_{DateTime.Now:ddMMyyyyHHmmss}.xlsx";
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["BM03"];
                    worksheet.Cells.Style.Font.Name = "Times New Roman";

                    if (tblBC != null && tblBC.Rows.Count > 0)
                    {
                        DateTime ngayDGPhieu = clsForrmatUtils.ConvertDate(tblBC?.Rows[0]["NgayDanhGiaPhieu"]?.ToString());
                        var queryCL = tblChungLoai.AsEnumerable().FirstOrDefault(x => x["MaCLVT"]?.ToString() == (tblBC?.Rows[0]["MaChungLoai"]?.ToString()));
                        if (queryCL != null)
                        {
                            worksheet.Cells["A4"].Value = $"Tên hàng hoá, dịch vụ: {queryCL["ChungLoaiVatTu"]?.ToString()}";
                        }
                        if (ngayDGPhieu == DateTime.MinValue)
                        {
                            worksheet.Cells["A5"].Value = $"Năm :";
                        }
                        else
                        {
                            worksheet.Cells["A5"].Value = $"Năm : {ngayDGPhieu.Year}";
                        }

                        int stt = 0;
                        foreach (DataRow row in tblBC.Rows)
                        {

                            stt++;
                            // Ngày đánh giá

                            worksheet.Cells[rowIdx, 1].Value = stt;


                            // Nhà cung cấp: lấy từ TenPhieu → tra tên NCC bằng LINQ
                            string maPhieu = row["TenPhieu"]?.ToString();
                            string tenNCC = tblNhaCC.AsEnumerable()
                                .Where(r => r["MaNhaCC"].ToString() == row["MaNhaCC"]?.ToString())
                                .Select(r => r["TenNCC"].ToString())
                                .FirstOrDefault();
                            worksheet.Cells[rowIdx, 2].Value = tenNCC;
                            colIdx = 2;

                            for (int i = 1; i <= 6; i++)
                            {
                                string prefix = $"{i}@TieuChi@";
                                DataColumn matchedCol = null;
                                foreach (DataColumn col in tblBC.Columns)
                                {
                                    if (col.ColumnName.StartsWith(prefix))
                                    {
                                        colIdx++;
                                        matchedCol = col;
                                        break;
                                    }
                                }

                                if (matchedCol != null)
                                {
                                    string raw = row[matchedCol.ColumnName]?.ToString();
                                    if (!string.IsNullOrEmpty(raw) && raw.StartsWith("Diem_"))
                                    {
                                        if (tblDiem != null && tblDiem?.Rows?.Count > 0)
                                        {
                                            var Query = tblDiem.AsEnumerable().FirstOrDefault(x => x["MaDiemDanhGia"]?.ToString() == raw);
                                            if (Query != null)
                                            {
                                                worksheet.Cells[rowIdx, 2 + i].Value = Query["DiemDanhGia"]?.ToString();
                                            }
                                        }
                                    }
                                }

                            }



                            worksheet.Cells[rowIdx, colIdx + 1].Value = row["TongDiemUuTien"]?.ToString();

                            // Ghi chú
                            worksheet.Cells[rowIdx, colIdx + 2].Value = row["ThuTuUuTien"]?.ToString();
                            worksheet.Row(rowIdx).Height = 35;

                            rowIdx++;
                        }


                        var borderRangeGrid = worksheet.Cells[9, 1, rowIdx - 1, colIdx + 2];
                        borderRangeGrid.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        borderRangeGrid.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        borderRangeGrid.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        borderRangeGrid.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        borderRangeGrid.Style.WrapText =true;


                        borderRangeGrid.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        borderRangeGrid.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        if (tblKetQua != null && tblKetQua.Rows.Count > 0)
                        {
                            DataRow row = tblKetQua.Rows[0];

                            // Cách bảng chính 2 dòng
                            rowIdx += 2;

                            // === Tiêu đề đánh giá ===
                            worksheet.Cells[rowIdx, 1].Value = "Đánh giá";
                            worksheet.Cells[rowIdx, 1, rowIdx + 1, 1].Merge = true;

                            worksheet.Cells[rowIdx, 2, rowIdx + 1, 3].Merge = true;
                            worksheet.Cells[rowIdx, 2].Value = "Người đề xuất";
                            worksheet.Cells[rowIdx, 4, rowIdx + 1, 5].Merge = true;
                            worksheet.Cells[rowIdx, 4].Value = "Soát xét";
                            worksheet.Cells[rowIdx, 6, rowIdx + 1, 7].Merge = true;
                            worksheet.Cells[rowIdx, 6].Value = "Xét duyệt\nTổng giám đốc";

                            worksheet.Cells[rowIdx, 1, rowIdx + 1, 7].Style.Font.Bold = true;
                            worksheet.Cells[rowIdx, 1, rowIdx + 1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIdx, 1, rowIdx + 1, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[rowIdx, 6].Style.WrapText = true;
                            worksheet.Row(rowIdx).Height = 25;
                            worksheet.Row(rowIdx + 1).Height = 25;
                            rowIdx += 2;

                            // === Đồng ý ===
                            worksheet.Cells[rowIdx, 1].Value = "Đồng ý";

                            worksheet.Cells[rowIdx, 2, rowIdx, 3].Merge = true;
                            if (!string.IsNullOrEmpty(row["KQ_DG_NgDeXuat"]?.ToString()))
                            {
                                worksheet.Cells[rowIdx, 2].Value = Convert.ToBoolean(row["KQ_DG_NgDeXuat"]) ? "" : "✓";
                            }
                            //worksheet.Cells[rowIdx, 2].Value = Convert.ToBoolean(row["KQ_DG_NgDeXuat"]) ? "✓" : "";
                            worksheet.Cells[rowIdx, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            worksheet.Cells[rowIdx, 4, rowIdx, 5].Merge = true;
                            if (!string.IsNullOrEmpty(row["KQ_DG_SoatXet"]?.ToString()))
                            {
                                worksheet.Cells[rowIdx, 4].Value = Convert.ToBoolean(row["KQ_DG_SoatXet"]) ? "" : "✓";
                            }
                            //worksheet.Cells[rowIdx, 4].Value = Convert.ToBoolean(row["KQ_DG_SoatXet"]) ? "✓" : "";
                            worksheet.Cells[rowIdx, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            worksheet.Cells[rowIdx, 6, rowIdx, 7].Merge = true;
                            if (!string.IsNullOrEmpty(row["KQ_DG_TongGiamDoc"]?.ToString()))
                            {
                                worksheet.Cells[rowIdx, 6].Value = Convert.ToBoolean(row["KQ_DG_TongGiamDoc"]) ? "" : "✓";
                            }
                            //worksheet.Cells[rowIdx, 6].Value = Convert.ToBoolean(row["KQ_DG_TongGiamDoc"]) ? "✓" : "";
                            worksheet.Cells[rowIdx, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            worksheet.Row(rowIdx).Height = 35;
                            rowIdx++;

                            // === Không đồng ý ===
                            worksheet.Cells[rowIdx, 1].Value = "Không đồng ý";

                            worksheet.Cells[rowIdx, 2, rowIdx, 3].Merge = true;
                            if (!string.IsNullOrEmpty(row["KQ_DG_NgDeXuat"]?.ToString()))
                            {
                                worksheet.Cells[rowIdx, 2].Value = Convert.ToBoolean(row["KQ_DG_NgDeXuat"]) ? "" : "✓";
                            }
                          
                            worksheet.Cells[rowIdx, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            worksheet.Cells[rowIdx, 4, rowIdx, 5].Merge = true;
                            if (!string.IsNullOrEmpty(row["KQ_DG_SoatXet"]?.ToString()))
                            {
                                worksheet.Cells[rowIdx, 4].Value = Convert.ToBoolean(row["KQ_DG_SoatXet"]) ? "" : "✓";
                            }
                        
                            worksheet.Cells[rowIdx, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            worksheet.Cells[rowIdx, 6, rowIdx, 7].Merge = true;
                            worksheet.Cells[rowIdx, 4, rowIdx, 5].Merge = true;
                            if (!string.IsNullOrEmpty(row["KQ_DG_TongGiamDoc"]?.ToString()))
                            {
                                worksheet.Cells[rowIdx, 6].Value = Convert.ToBoolean(row["KQ_DG_TongGiamDoc"]) ? "" : "✓";
                            }

         
                            worksheet.Cells[rowIdx, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            worksheet.Row(rowIdx).Height = 35;
                            rowIdx++;

                            // === Ý kiến khác ===
                            worksheet.Cells[rowIdx, 1].Value = "Ý kiến khác";

                            worksheet.Cells[rowIdx, 2, rowIdx, 3].Merge = true;
                            worksheet.Cells[rowIdx, 2].Value = row["YKien_NgDeXuat"]?.ToString();
                            worksheet.Cells[rowIdx, 2].Style.WrapText = true;
                            worksheet.Cells[rowIdx, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIdx, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                            worksheet.Cells[rowIdx, 4, rowIdx, 5].Merge = true;
                            worksheet.Cells[rowIdx, 4].Value = row["YKien_SoatXet"]?.ToString();
                            worksheet.Cells[rowIdx, 4].Style.WrapText = true;
                            worksheet.Cells[rowIdx, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIdx, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                            worksheet.Cells[rowIdx, 6, rowIdx, 7].Merge = true;
                            worksheet.Cells[rowIdx, 6].Value = row["YKien_TongGiamDoc"]?.ToString();
                            worksheet.Cells[rowIdx, 6].Style.WrapText = true;
                            worksheet.Cells[rowIdx, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[rowIdx, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                            worksheet.Row(rowIdx).Height = 45;

                          
                            rowIdx++;

                            // === Ngày tháng năm và ký tên ===
                            DateTime NgayKy_NgDeXuat = clsForrmatUtils.ConvertDate(row["NgayKy_NgDeXuat"]?.ToString());                    
                            worksheet.Cells[rowIdx, 2].Value = NgayKy_NgDeXuat == DateTime.MinValue ? "Ngày   tháng   năm" :
                                $"Ngày {NgayKy_NgDeXuat.Day:D2} tháng {NgayKy_NgDeXuat.Month:D2} năm {NgayKy_NgDeXuat.Year}"; 
                            worksheet.Cells[rowIdx, 2, rowIdx,3].Merge = true;

                            worksheet.Cells[rowIdx + 1, 2].Value = "Ký tên";
                            worksheet.Cells[rowIdx + 1, 2, rowIdx + 4, 3].Merge = true;
                            DateTime NgayKy_SoatXet = clsForrmatUtils.ConvertDate(row["NgayKy_SoatXet"]?.ToString());
                            worksheet.Cells[rowIdx, 4].Value = NgayKy_SoatXet == DateTime.MinValue ? "Ngày   tháng   năm" :
                                $"Ngày {NgayKy_SoatXet.Day:D2} tháng {NgayKy_SoatXet.Month:D2} năm {NgayKy_SoatXet.Year}";
                            worksheet.Cells[rowIdx, 4, rowIdx, 5].Merge = true;

                            worksheet.Cells[rowIdx + 1, 4].Value = "Ký tên";
                            worksheet.Cells[rowIdx+ 1, 4, rowIdx + 4, 5].Merge = true;
                            DateTime NgayKy_TongGiamDoc = clsForrmatUtils.ConvertDate(row["NgayKy_TongGiamDoc"]?.ToString());
                            worksheet.Cells[rowIdx, 6].Value = NgayKy_TongGiamDoc == DateTime.MinValue ? "Ngày   tháng   năm":
                              $"Ngày {NgayKy_TongGiamDoc.Day:D2} tháng {NgayKy_TongGiamDoc.Month:D2} năm {NgayKy_TongGiamDoc.Year}";
                            worksheet.Cells[rowIdx, 6, rowIdx, 7].Merge = true;

                            worksheet.Cells[rowIdx, 2, rowIdx, 3].Merge = true;

                            worksheet.Cells[rowIdx + 1, 6].Value = "Ký tên";

                            worksheet.Cells[rowIdx+ 1, 6, rowIdx + 4, 7].Merge = true;


                            worksheet.Cells[rowIdx, 1, rowIdx + 1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rowIdx, 1, rowIdx + 1, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            // === Chữ ký hình ảnh ===
                            AddImageToCell(worksheet, row["SignDG_NgDeXuat"]?.ToString(), rowIdx + 2, 2, 60, 120, 50);
                            AddImageToCell(worksheet, row["SignDG_SoatXet"]?.ToString(), rowIdx + 2, 4, 60, 120, 50);
                            AddImageToCell(worksheet, row["SignDG_TongGiamDoc"]?.ToString(), rowIdx + 2, 6, 60, 120,50);

                            // === Border toàn vùng đánh giá ===
                            var borderRange = worksheet.Cells[rowIdx - 5, 1, rowIdx + 4, 7];
                            borderRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            borderRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            borderRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            borderRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            borderRange.Style.WrapText = true;
                            worksheet.Cells[rowIdx, 1, rowIdx + 4, 1].Merge = true;
                            // Cập nhật rowIdx để tiếp tục xuất sau
                            rowIdx += 6;
                          
                        }


                        worksheet.Cells[rowIdx + 1, 1].Value = "Ghi chú:";
                        worksheet.Cells[rowIdx + 1, 2].Value = "*Thang điểm: 1: Rất kém, 3: Đạt, 4: Tốt, 5: Rất tốt";
                        worksheet.Cells[rowIdx + 1 + 1, 2].Value = "*Điểm đạt tối thiểu ở mỗi mục là 3; đánh giá theo PL02/QT15/KH02";
                        worksheet.Cells[rowIdx + 1 + 2, 2].Value = "*Kiểm soát định kỳ tối thiểu 1 lần/quý";

                    }

                    // Xuất file Excel
                    byte[] fileBytes = package.GetAsByteArray();
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExportPhieuTheoDoi error: " + ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        private void AddImageToCell(ExcelWorksheet worksheet, string imagePath, int row, int col, int offsetX, int width, int height)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) return;

            string fullPath = HostingEnvironment.MapPath(imagePath);
            if (!File.Exists(fullPath)) return;

            var imageFile = new FileInfo(fullPath);
            var picture = worksheet.Drawings.AddPicture(Guid.NewGuid().ToString(), imageFile);


            picture.SetPosition(row - 1, 17, col - 1, offsetX);
            picture.SetSize(width, height);
        }
    }
}