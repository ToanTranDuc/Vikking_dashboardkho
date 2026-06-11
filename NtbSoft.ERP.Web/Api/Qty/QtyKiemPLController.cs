using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using NtbSoft.ERP.Model.Qty;
using NtbSoft.ERP.Entity.NguyenPhuLieu;
using OfficeOpenXml;
using System.Net.Http;
using System.Web.Hosting;
using OfficeOpenXml.Drawing;
using System.Net;
using System.Linq;
using NtbSoft.ERP.Utils;
using OfficeOpenXml.Style;

namespace NtbSoft.ERP.Web.Api.Qty
{
    [RoutePrefix("api/QtyKiemPL")]
    public class QtyKiemPLController : ApiController
    {
        private QtyKiemPLModel _model = new QtyKiemPLModel();

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
        [HttpPost]
        [Route("Post")]
        public async Task<string> Post(List<KiemPLEntity> lstKiemPL)
        {
            if (lstKiemPL == null) return "false";
            string json = JsonConvert.SerializeObject(lstKiemPL);
            DataTable tblSave = JsonConvert.DeserializeObject<DataTable>(json);
            return await _model.Post(tblSave);
        }
        [HttpPost]
        [Route("PostXN_DanhGia")]
        public async Task<string> PostXN_DanhGia(List<KiemPL_DanhGiaEntity> lstDanhGia)
        {
            if (lstDanhGia == null) return "false";
            string json = JsonConvert.SerializeObject(lstDanhGia);
            DataTable tblSave = JsonConvert.DeserializeObject<DataTable>(json);
            return await _model.PostXN_DanhGia(tblSave);
        }
        [HttpPost]
        [Route("UploadImg")]
        public async Task<List<dynamic>> UploadImg([FromBody] JArray imageDatas, string getFileName)
        {
            var results = new List<dynamic>();

            try
            {
                string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "KiemPL");
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
                            string publicPath = Path.Combine("/Images/KiemPL/", folderName, sanitizedFileName).Replace("\\", "/");

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
                //await RemoveSignature(MaLenh);
                await Task.WhenAll(tasks);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading images: {ex.Message}");
            }

            return results;
        }
        [HttpPost]
        [Route("Delete")]
        public async Task<string> Delete(string action, string UserName, string Para1, string Para2 = null)
        {
            Para1 = Para1 ?? "NONE";
            Para2 = Para2 ?? "NONE";
            return await _model.Delete(action, UserName, Para1, Para2);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<string> Update(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            return await _model.Update(action, para1, para2, para3, para4, para5);
        }
        [HttpPost]
        [Route("ExportBC")]
        public async Task<HttpResponseMessage> ExportBC(JObject objSoLo)
        {
            try
            {
                int rowStart = 9;
                int rowIdx = 9;
                int colIdx = 3;
                string templatePath = System.Web.HttpContext.Current.Server.MapPath(@"\Content\Templates\TemplateBBKiemPL.xlsx");

                DataTable tblBC = await _model.Get("GetBC", objSoLo["SoLoID"]?.ToString(), "NONE", "NONE", "NONE", "NONE");
                DataTable tblDonViTinh = await _model.Get("GetDonViVT", "NONE", "NONE", "NONE", "NONE", "NONE");
                //DataTable tblDanhGia = await _model.Get("GetXN_PL", objSoLo["MaPhieuKiem"]?.ToString(), objSoLo["SoLoID"]?.ToString(),"NONE", "NONE", "NONE");

                FileInfo templateFile = new FileInfo(templatePath);
                string fileName = $"BM04/QT13/CL01-{DateTime.Now.ToString("ddMMyyyyhhmmss")}.xlsx";

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["BM04"];
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    

                    await Task.Run(() =>
                    {
                        if (tblBC != null && tblBC.Rows.Count > 0)
                        {
                            DataRow rowHeader = tblBC.Rows[0];
                            worksheet.Cells["A6"].Value = worksheet.Cells["A6"].Text + ": " + objSoLo["TenKH"]?.ToString();
                            worksheet.Cells["A7"].Value = worksheet.Cells["A7"].Text + ": " + objSoLo["TenNCC"]?.ToString();
                            worksheet.Cells["E6"].Value = worksheet.Cells["E6"].Text + ": " + objSoLo["SoDDH"]?.ToString();

                            string ngayNKFormatted = "";
                            if (!string.IsNullOrEmpty(objSoLo["NgayNK"]?.ToString()))
                            {
                                if (DateTime.TryParse(objSoLo["NgayNK"].ToString(), out DateTime ngayNK))
                                {
                                    ngayNKFormatted = ngayNK.ToString("dd/MM/yyyy");
                                }
                            }
                            worksheet.Cells["E7"].Value = worksheet.Cells["E7"].Text + ": " + ngayNKFormatted;

                            var boldTextRange = worksheet.Cells["A5:J6"];
                            boldTextRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            boldTextRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            boldTextRange.Style.Font.Bold = true;

                          
                            CreateDynamicHeaders(worksheet, tblBC);


                            foreach (DataRow row in tblBC.Rows)
                            {
                                int stt = row["STT"] != DBNull.Value ? Convert.ToInt32(row["STT"]) : 0;
                                string maPhuLuc = row["MaPhuLuc"]?.ToString() ?? "NONE";
                                worksheet.Cells[rowIdx, 2].Value = row["PhuLuc"]?.ToString();
                                worksheet.Cells[rowIdx, 1].Value = maPhuLuc == "NONE" || stt == 0 ? "" : (object)stt;
                                colIdx = 3;

                                foreach (DataColumn col in tblBC.Columns)
                                {
                                    if (col.ColumnName.Contains("@MaNPL@"))
                                    {
                                        string rawValue = row[col.ColumnName]?.ToString() ?? "";
                                        var parts = rawValue.Split('@').Select(s => s.Trim()).ToArray();
                                        string note = parts.Length > 0 ? parts[0].Replace("none", "").Trim() : "";
                                        string status = parts.Length > 1 ? parts[1].ToLower() : "";
                                        string img = parts.Length > 2 ? parts[2].Replace("none", "").Trim() : "";
                                        string soMet = parts.Length > 4 ? parts[4].Replace("none", "").Trim() : "";
                                        string cuon = parts.Length > 5 ? parts[5].Replace("none", "").Trim() : "";
                                        string isWash = parts.Length > 6 ? parts[6].Replace("none", "").Trim() : "";
                                        string maDvTinh = parts.Length > 3 ? parts[3].Replace("none", "").Trim() : "";
                                        string TenDV = string.Empty;
                                        if(tblDonViTinh!= null && tblDonViTinh?.Rows?.Count > 0)
                                        {
                                            var QueryDVTinh = tblDonViTinh.AsEnumerable().FirstOrDefault(x => x["MaDVVT"]?.ToString() == maDvTinh);
                                            if(QueryDVTinh != null)
                                            {
                                                TenDV = QueryDVTinh["TenDVVT"]?.ToString();
                                            }
                                        }
                                        if (status == "none") status = "";
                                        if (img == "none") img = "";

                                        bool isCheckType = new[] { "PhuLucPL_2", "PhuLucPL_3", "PhuLucPL_8", "PhuLucPL_9", "PhuLucPL_10", "PhuLucPL_10", "PhuLucPL_12", "PhuLucPL_13", "NONE", "PhuLucPL_15", "PhuLucPL_16" }.Contains(maPhuLuc);
                                        bool isImageType = new[] { "PhuLucPL_1", "PhuLucPL_8", "PhuLucPL_9", "PhuLucPL_10", "PhuLucPL_11", "PhuLucPL_12", "PhuLucPL_13", "PhuLucPL_15", "PhuLucPL_2", "PhuLucPL_3","NONE" }.Contains(maPhuLuc);
                                        bool HasUnit = new[] { "PhuLucPL_5", "PhuLucPL_6", "PhuLucPL_7" }.Contains(maPhuLuc);
                                        var cell = worksheet.Cells[rowIdx, colIdx];
                                        cell.Style.Font.Size = 12;
                                        if (isCheckType)
                                        {
                                            SetCellWithImageAndStatus(worksheet, rowIdx, colIdx, img, status, note);
                                        }
                                        else if (isImageType && maPhuLuc == "PhuLucPL_1")
                                        {
                                            SetCellWithImage(worksheet, rowIdx, colIdx, img, note);
                                        }
                                        else if (maPhuLuc == "NONE")
                                        {
                                                                                                                          
                                            worksheet.Cells[rowIdx, colIdx].Value = rawValue;
                                            worksheet.Cells[rowIdx, colIdx].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            worksheet.Cells[rowIdx, colIdx].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                            //if (!worksheet.Cells[rowIdx, colIdx, rowIdx + 1, colIdx].Merge)
                                            //{
                                            //    worksheet.Cells[rowIdx, colIdx, rowIdx + 1, colIdx].Merge = true;
                                            //}
                                           
                                        }
                                        else
                                        {
                                            
                                            if (HasUnit)
                                            {
                                                cell.Value =( string.IsNullOrEmpty(note) ? "" : note) + $" | {TenDV}" ;
                                            }
                                            else if(maPhuLuc== "PhuLucPL_14")
                                            {
                                                cell.Value = $"Tổng mét thực tế: {soMet} | Số cuộn: {cuon}  | {TenDV}";
                                            }
                                            else
                                            {
                                                cell.Value = string.IsNullOrEmpty(note) ? "" : note;
                                            }
                                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        }

                                        // Style cơ bản
                                        cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                        cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                        cell.Style.WrapText = true;

                                        if (maPhuLuc != "PhuLucPL_1")
                                        {
                                            worksheet.Row(rowIdx).Height = 60;
                                        }

                                        if (maPhuLuc == "PhuLucPL_2")
                                        {
                                          
                                            cell.Style.Font.Bold = true;
                                        }
                                        else
                                        {
                                            
                                            cell.Style.Font.Bold = false;
                                        }

                                        colIdx++;
                                    }
                                }

                                rowIdx++;
                            }

                            

                        }
                    });
                    package.Workbook.Calculate();
                    package.Workbook.Properties.Company = "NTB";
                    package.Workbook.Properties.Author = "NTB";

                    // Trả về file Excel
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


        private void SetCellWithImageAndStatus(ExcelWorksheet worksheet, int row, int col, string imgPath, string status, string note)
        {
            var cell = worksheet.Cells[row, col];

            // ✅ XÓA nội dung cũ trước khi thêm RichText
            cell.Value = null;
            cell.RichText.Clear();

            string statusText = "";
            if (status == "pass") statusText = "✓";
            else if (status == "fail") statusText = "X";
            else statusText = "*";

            // ✅ Thêm RichText an toàn
            if (!string.IsNullOrEmpty(statusText))
            {
                var richTextStatus = cell.RichText.Add(statusText);
                richTextStatus.Bold = true;
                richTextStatus.Size = 13;
            }

            if (!string.IsNullOrEmpty(note))
            {
                if (!string.IsNullOrEmpty(statusText))
                {
                    cell.RichText.Add("\n");
                }
                var richTextNote = cell.RichText.Add(note);
                richTextNote.Bold = false;
                richTextNote.Size = 12;
                richTextNote.Color = System.Drawing.Color.Black;
            }

            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cell.Style.WrapText = true;
        }

        private void SetCellWithImage(ExcelWorksheet worksheet, int row, int col, string imgPath, string note)
        {
            // Thêm ảnh nếu có
            //if (!string.IsNullOrEmpty(imgPath))
            //{
            //    AddImageToCell(worksheet, imgPath, row, col, 0, 80, 58);
            //}

            // Thêm ghi chú
            string displayText = string.IsNullOrEmpty(note) ? "" : note;
            worksheet.Cells[row, col].Value = displayText;
            worksheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells[row, col].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        }


        private void AddImageToCell(ExcelWorksheet worksheet, string imagePath, int row, int col, int offsetX, int width, int height)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) return;

            try
            {
                string fullPath = HostingEnvironment.MapPath(imagePath);
                if (!File.Exists(fullPath)) return;

                var imageFile = new FileInfo(fullPath);

                // ✅ Kiểm tra file size
                if (imageFile.Length > 5 * 1024 * 1024) // > 5MB
                {
                    return; // Skip ảnh quá lớn
                }

                var picture = worksheet.Drawings.AddPicture(Guid.NewGuid().ToString(), imageFile);
                picture.SetPosition(row - 1, 47, col - 1, offsetX);
                picture.SetSize(width, height);
            }
            catch (Exception ex)
            {
                // Log nhưng không throw
                System.Diagnostics.Debug.WriteLine($"Error adding image: {ex.Message}");
            }
        }
        private void CreateDynamicHeaders(ExcelWorksheet worksheet, DataTable tblBC)
        {
            int headerRow1 = 10;
            int headerRow2 = 11;

            var headerGroups = new Dictionary<string, List<string>>();

            foreach (DataColumn col in tblBC.Columns)
            {
                if (col.ColumnName.Contains("@MaNPL@"))
                {
                    string rollName = "";
                    var parts = col.ColumnName.Split(new[] { "@MaNPL@" }, StringSplitOptions.None);
                    if (parts.Length > 1)
                    {
                        rollName = !string.IsNullOrEmpty(parts[0]) ? $"Số Roll: {parts[0]}" : "";
                    }

                    if (!headerGroups.ContainsKey(parts[1]))
                    {
                        headerGroups[parts[1]] = new List<string>();
                    }
                    headerGroups[parts[1]].Add(rollName);
                }
            }

            int colIdx = 3;
            foreach (var group in headerGroups)
            {
                string noiDungKiem = group.Key;
                int colCount = group.Value.Count;
                int startCol = colIdx;
                int endCol = colIdx + colCount - 1;

                // ✅ KIỂM TRA trước khi merge
                if (startCol <= endCol && startCol > 0 && endCol <= 16384)
                {
                    // Chỉ merge nếu có nhiều hơn 1 cột
                    if (startCol < endCol)
                    {
                        worksheet.Cells[headerRow1, startCol, headerRow1, endCol].Merge = true;
                    }

                    worksheet.Cells[headerRow1, startCol].Value = noiDungKiem; // ✅ Thêm value
                    worksheet.Cells[headerRow1, startCol].Style.Font.Bold = true;
                    worksheet.Cells[headerRow1, startCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow1, startCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // ✅ Border cho từng cell thay vì BorderAround
                    for (int c = startCol; c <= endCol; c++)
                    {
                        worksheet.Cells[headerRow1, c].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[headerRow1, c].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[headerRow1, c].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[headerRow1, c].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                }

                for (int i = 0; i < group.Value.Count; i++)
                {
                    int currentCol = startCol + i;

                    worksheet.Cells[headerRow2, currentCol].Value = group.Value[i]; // ✅ Uncomment này
                    worksheet.Cells[headerRow2, currentCol].Style.Font.Bold = true;
                    worksheet.Cells[headerRow2, currentCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow2, currentCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // ✅ Border từng cell
                    worksheet.Cells[headerRow2, currentCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[headerRow2, currentCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[headerRow2, currentCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[headerRow2, currentCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    worksheet.Column(currentCol).Width = 20;
                }

                colIdx = endCol + 1;
            }
        }





    }
}