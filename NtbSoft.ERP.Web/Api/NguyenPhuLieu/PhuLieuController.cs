using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Qty;
using NtbSoft.ERP.Model.Qty;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.Qty
{
    [RoutePrefix("api/PhuLieu")]
    public class PhuLieuController : ApiController
    {
        [HttpGet]
        [Route("GetDate")]
        public dynamic GetDate()
        {
            return DateTime.Now.ToString("dd-MM-yyyy");
        }
        [HttpGet]
        [Route("GetPhuLieu")]
        public dynamic GetPhuLieu(string action, string para1 = "", string para2 = "", string para3 = "")
        {
            try
            {
                var ds = new QTYMaHangPhuLieuModel().Get(action, para1, para2, para3);
                return new { dt1 = ds.Tables[0], dt2 = ds.Tables.Count > 1 ? ds.Tables[1] : new DataTable(), dt3 = ds.Tables.Count > 2 ? ds.Tables[2] : new DataTable(),datetime = DateTime.Now.ToString("dd-MM-yyyy") };
            }
            catch (Exception ex)
            {
                string Message = $"Function: NguyenPhuLieuController/GetLine \nMessage: {ex.Message.ToString()}";
                //new WriteLogModel().WriteFileLog("Active", Message);
                return null;
            }
        }
        [HttpPost]
        [Route("SaveGopMH")]
        public string SaveGopMH(dynamic data)
        {
            try
            {                
                string json = JsonConvert.SerializeObject(data);
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                return new QTYMaHangPhuLieuModel().SaveGopMH("Save", dt);
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        [HttpPost]
        [Route("SaveError")]
        public dynamic Save(dynamic data)
        {
            try
            {
                if (data is null) return null;
                List<QtyMaHangPhuLieuEntity> lstData = new List<QtyMaHangPhuLieuEntity>();
                string imageName = "";
                var lstImg = data.Image ?? new List<string>();
                int i = 0;
                foreach (var item in lstImg)
                {
                    string imageData = item;
                    if (!string.IsNullOrEmpty(imageData))
                    {
                        if (!imageData.Contains(".png"))
                        {
                            byte[] bytes = Convert.FromBase64String(imageData.Split(',')[1]);

                            using (MemoryStream ms = new MemoryStream(bytes))
                            {
                                Image image = Image.FromStream(ms);
                                string uploadPath = HttpContext.Current.Server.MapPath("~/Images/PhuLieu");
                                if (!System.IO.Directory.Exists(uploadPath))
                                    System.IO.Directory.CreateDirectory(uploadPath);
                                var _imageName = $"{data.MaHang.ToString()}_{DateTime.Now.ToString("ddMMyyyy_HHmmssfff")}_{i.ToString()}";
                                string except = " _";
                                _imageName = Regex.Replace(_imageName, @"[^a-zA-Z0-9" + except + "]+", string.Empty) + ".png";
                                var mPath = string.Format(@"{0}\{1}", uploadPath, _imageName);
                                image.Save(mPath);
                                imageName += "|" + _imageName;
                            }
                        }
                        else
                        {
                            imageName += "|" + imageData.Split('/').Last();
                        }
                    }
                    i++;
                }
                imageName = imageName.TrimStart('|');
                foreach (var item in data.ArrayXY)
                {
                    lstData.Add(new QtyMaHangPhuLieuEntity()
                    {
                        MaDH = data.MaDH.ToString(),
                        MaHang = data.MaHang.ToString(),
                        MaVatTu = data.MaVatTu.ToString(),
                        Dot = data.Dot,
                        // GhiChu = data.NoiDung.ToString(),
                        MaLoi = item["Code"].ToString(),
                        NVKiem = data.NVKiem
                    });
                }

                var json = JsonConvert.SerializeObject(lstData);
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                new QTYMaHangPhuLieuModel().Post("SaveError", dt);
                return Ok(new { Status = "OK" });
            }
            catch (Exception ex)
            {
                string Message = $"Function: NguyenPhuLieuController/SaveSLKiem \nMessage: {ex.Message.ToString()}";
                //new WriteLogModel().WriteFileLog("InLine", Message);
                return Ok(new { Status = "Fail" });
            }

        }
        [HttpPost]
        [Route("SavePhuLieu")]
        public IHttpActionResult SavePhuLieu(dynamic data)
        {
            try
            {
                if (data is null) return null;
                List<QtyMaHangPhuLieuEntity> lstData = new List<QtyMaHangPhuLieuEntity>();
                var a = data.QuiCach;
                lstData.Add(new QtyMaHangPhuLieuEntity()
                {
                    MaDH = data.MaDH.ToString(),
                    MaHang = data.MaHang.ToString(),
                    MaVatTu = data.MaVatTu.ToString(),
                    QuiCach = data.QuiCach ?? "",
                    SoLuong = (float)(data.SoLuong ?? 0),
                    SoLuongThuc = (float)(data.SoLuongThuc ?? 0),
                    SoLuongKiem = (float)(data.SoLuongKiem ?? 0),
                    Balance = (float)(data.Balance ?? 0),
                    Dot = data.Dot,
                    GhiChu = data.GhiChu ?? "",
                    NVKiem = data.NVKiem ?? "",
                });

                var json = JsonConvert.SerializeObject(lstData);
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                new QTYMaHangPhuLieuModel().Post(data.Action.ToString(), dt);
                return Ok(new { Status = "OK" });
            }
            catch (Exception ex)
            {
                string Message = $"Function: QtyPhuLieuController/SavePhuLieu \nMessage: {ex.Message.ToString()}";
                //new WriteLogModel().WriteFileLog("InLine", Message);
                return Ok(new { Status = "Fail" });
            }

        }
        [HttpPost]
        [Route("SaveSign")]
        public IHttpActionResult SaveSign(dynamic data)
        {
            if (data is null) return null;
            string imageName = data.MaHang + "_" + data.IdNguoiKy.ToString();
            string idNguoiKy = data.IdNguoiKy;
            string imageData = data.Image.ToString();
            List<QtyMaHangPhuLieuEntity> lstData = new List<QtyMaHangPhuLieuEntity>();
            if (!string.IsNullOrEmpty(imageData))
            {
                if (!imageData.Contains(".png"))
                {
                    byte[] bytes = Convert.FromBase64String(imageData.Split(',')[1]);

                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        Image image = Image.FromStream(ms);
                        string uploadPath = HttpContext.Current.Server.MapPath("~/Images/SignNguyenPhuLieu");
                        if (!System.IO.Directory.Exists(uploadPath))
                            System.IO.Directory.CreateDirectory(uploadPath);
                        imageName += $"{DateTime.Now.ToString("ddMMyyyy_HHmmssfff")}" + ".png";
                        var mPath = string.Format(@"{0}\{1}", uploadPath, imageName);
                        image.Save(mPath);
                    }
                }
                else
                {
                    imageName = imageData.Split('/').Last();
                }
            }

            lstData.Add(new QtyMaHangPhuLieuEntity()
            {
                MaDH = data.MaDH.ToString(),
                MaHang = data.MaHang.ToString(),
                Dot = data.Dot,
                Image = imageName,
                NVKiem = data.NVKiem.ToString()
            });

            var json = JsonConvert.SerializeObject(lstData);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            new QTYMaHangPhuLieuModel().Post("SaveSign", dt, idNguoiKy);
            return Ok(new { Status = "OK" });
        }


        [HttpPost]
        [Route("EXBCKTVai")]
        public HttpResponseMessage EXBCKTVai(dynamic postData)
        {
            var foder = HttpContext.Current.Server.MapPath("~/Images/SignKiemVai");
            string jsonData = JsonConvert.SerializeObject(postData.ArrBody);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonData);


            var ds = new QTYMaHangPhuLieuModel().GetVai("GetNhomLoiChiTiet", "", "", "","","","", "", "");

            DataTable NL1 = ds.Tables[0].Copy();
            DataTable NL2 = ds.Tables[1].Copy();

            string imagePathRelative = "/Content/Templates/BaoCaoKTChatLuongVai.xlsx";
            string imagePathPhysical = HttpContext.Current.Server.MapPath("~" + imagePathRelative);

            FileInfo templateFile = new FileInfo(imagePathPhysical);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            using (ExcelPackage package = new ExcelPackage(templateFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                ExcelRange range = worksheet.Cells;
                worksheet.Cells["b6"].Value = postData.ArrThongSo[0].ToString();
                worksheet.Cells["f6"].Value = postData.ArrThongSo[1].ToString();
                worksheet.Cells["j6"].Value = postData.ArrThongSo[2].ToString();
                int rowNL = 9, rowKT = 9;
                int rowInsert = Math.Max(NL1.Rows.Count + NL2.Rows.Count, tbl.Rows.Count * 3);
                worksheet.InsertRow(rowNL, rowInsert);
                foreach (DataRow item in NL1.Rows)
                {
                    range = worksheet.Cells[rowNL, 2, rowNL, 4]; range.Merge = true; range.Value = item["Name"]; range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    rowNL++;
                    DataTable NLNew = NL2.AsEnumerable().Where(x => x["NhomLoi"].ToString() == item["ID"].ToString()).CopyToDataTable();
                    foreach (DataRow dr in NLNew.Rows)
                    {

                        worksheet.Cells[rowNL, 1].Value = dr["MaLoi"];
                        range = worksheet.Cells[rowNL, 2, rowNL, 4]; range.Merge = true; range.Value = dr["TenLoi"];
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowNL++;
                    }
                }
                range = worksheet.Cells[9, 1, rowNL - 1, 4];
                BorderEx(range);

                List<string> columnNamesWithSize = tbl.Columns.Cast<DataColumn>()
                     .Where(column => column.ColumnName.Contains("@"))
                     .Select(column => column.ColumnName)
                     .Distinct()
                     .ToList();


                int columnDL = (columnNamesWithSize.Count == 0 ? 0 : columnNamesWithSize.Count - 1) + 10;
                range = worksheet.Cells[7, 10, 8, columnDL];
                range.Value = "Kết quả kiểm tra ( Result )"; range.Merge = true; range.Style.Font.Bold = true;
                BorderEx(range);
                columnDL++;
                range = worksheet.Cells[7, columnDL, 8, columnDL]; range.Merge = true; range.Style.Font.Bold = true; range.Value = "Tổng số điểm"; range.Style.WrapText = true;
                BorderEx(range);
                columnDL++;
                range = worksheet.Cells[7, columnDL, 8, columnDL]; range.Merge = true; range.Style.Font.Bold = true; range.Value = "Ghi chú \n (Notes)"; range.Style.WrapText = true;

                BorderEx(range);
                range = worksheet.Cells[3, 1, 3, columnDL]; range.Merge = true; range.Value = "BẢNG BÁO CÁO KIỂM TRA CHẤT LƯỢNG VẢI"; range.Style.Font.Bold = true; range.Style.Font.Size = 16;
                range = worksheet.Cells[1, columnDL - 1, 2, columnDL]; range.Merge = true; range.Value = "BM-KH-26 \n LSD: 02"; range.Style.WrapText = true;
                BorderEx(range);
                range = worksheet.Cells[1, columnDL - 3]; range.Value = "HỆ THỐNG CHẤT LƯỢNG"; range.Style.Font.Bold = true;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                foreach (DataRow item in tbl.Rows)
                {
                    int columnKT = 10;
                    int tongSD = 0;
                    range = worksheet.Cells[rowKT, 5, rowKT + 2, 5]; range.Value = item["TenMau"]; range.Merge = true;
                    range = worksheet.Cells[rowKT, 6, rowKT + 2, 6]; range.Value = item["SoCay"]; range.Merge = true;
                    range = worksheet.Cells[rowKT, 7, rowKT + 2, 7]; range.Value = $"{item["SoLuong"]}\n {item["SoLuongTT"]}"; range.Merge = true;range.Style.WrapText = true;
                     range = worksheet.Cells[rowKT, 8, rowKT + 2, 8]; range.Value = $"{item["Kho"]}\n {item["KhoTT"]}"; range.Merge = true; range.Style.WrapText = true;

                    foreach (var dr in columnNamesWithSize)
                    {
                        var valueLoi = item[dr].ToString().Split('@');
                        var DiemLoi = Convert.ToInt16(valueLoi[0]);
                        worksheet.Cells[rowKT, columnKT, rowKT, columnKT].Value = valueLoi[1].ToString();
                        worksheet.Cells[rowKT + 1, columnKT, rowKT + 1, columnKT].Value = dr.Split('@')[0];
                        worksheet.Cells[rowKT + 2, columnKT, rowKT + 2, columnKT].Value = DiemLoi;
                        columnKT++;                        
                        tongSD += DiemLoi;
                    }

                    range = worksheet.Cells[rowKT, columnDL - 1, rowKT + 2, columnDL - 1]; range.Value = $"{item["GhiChu"]}\n{tongSD}"; range.Merge = true; range.Style.WrapText = true;
                    range = worksheet.Cells[rowKT, columnDL, rowKT + 2, columnDL]; range.Value = item["KetQua"]; range.Merge = true;
                    rowKT += 3;
                }
                int rowMax = Math.Max(rowNL, rowKT);
                string[] values = { "Vị trí lỗi", "Loại lỗi", "Điểm lỗi" };
                int startRow = 9;
                int endRow = rowMax - 1;
                for (int i = startRow; i <= endRow; i++)
                {
                    string value = values[(i - startRow) % values.Length];
                    worksheet.Cells[i, 9].Value = value;
                }

                range = worksheet.Cells[9, 5, rowMax - 1, columnDL];
                BorderEx(range);

                range = worksheet.Cells[rowMax, 2]; range.Value = postData.ArrThongSo[3].ToString();
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                if (postData.ArrThongSo[4].ToString() != "") Addpicutre2(worksheet, postData.ArrThongSo[4].ToString(), rowMax + 1, 14, 10, foder, 110,85);
                if (postData.ArrThongSo[5].ToString() != "") Addpicutre2(worksheet, postData.ArrThongSo[5].ToString(), rowMax + 1, 17, 10, foder, 110, 85);

                byte[] fileBytes = package.GetAsByteArray();
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = $"BaoCaoKTCLVai.xlsx";


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

        [HttpPost]
        [Route("EXBCNPhuLieu")]
        public HttpResponseMessage EXBCNPhuLieu(dynamic postData)
        {
            var foder = HttpContext.Current.Server.MapPath("~/Images/SignNguyenPhuLieu");
            string jsonData = JsonConvert.SerializeObject(postData.ArrBody);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonData);

            string imagePathRelative = "/Content/Templates/BaoCaoKTPhuLieu.xlsx";
            string imagePathPhysical = HttpContext.Current.Server.MapPath("~" + imagePathRelative);

            FileInfo templateFile = new FileInfo(imagePathPhysical);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            using (ExcelPackage package = new ExcelPackage(templateFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                ExcelRange range = worksheet.Cells;
                worksheet.Cells["c5"].Value = postData.ArrThongSo[0].ToString();
                worksheet.Cells["C6"].Value = postData.ArrThongSo[1].ToString();
                worksheet.Cells["H6"].Value = postData.ArrThongSo[5].ToString();
                worksheet.Cells["H7"].Value = postData.ArrThongSo[6].ToString();
                int row = 10;
                int rowInsert = tbl.Rows.Count + 1;
                worksheet.InsertRow(row, rowInsert);
                int index = 1;
                foreach (DataRow item in tbl.Rows)
                {
                    worksheet.Cells[row, 1].Value = index;
                    worksheet.Cells[row, 2].Value = item["TenVT"];
                    worksheet.Cells[row, 3].Value = "";
                    worksheet.Cells[row, 4].Value = item["DVTinh"];
                    worksheet.Cells[row, 5].Value = item["SoLuong"];
                    worksheet.Cells[row, 6].Value = item["SoLuongThuc"];
                    worksheet.Cells[row, 7].Value = item["Balance"];
                    worksheet.Cells[row, 8].Value = item["SoLuongKiem"];
                    worksheet.Cells[row, 9].Value = item["SoLuongLoi"];
                    worksheet.Cells[row, 10].Value = item["TLLoi"];
                    worksheet.Cells[row, 11].Value = item["TenLoi"];
                    worksheet.Cells[row, 12].Value = item["KetQua"];
                    row++;

                }
                worksheet.Column(11).AutoFit();
                range = worksheet.Cells[10, 1, row - 1, 12];

                BorderEx(range);
                range = worksheet.Cells[row + 1, 1]; range.Value = $"Comments (góp ý): {postData.ArrThongSo[2].ToString()}";
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                if (postData.ArrThongSo[3].ToString() != "") Addpicutre2(worksheet, postData.ArrThongSo[3].ToString(), row + 5, 1, 15, foder, 105, 90);
                if (postData.ArrThongSo[4].ToString() != "") Addpicutre2(worksheet, postData.ArrThongSo[4].ToString(), row + 5, 8, 20, foder, 105, 90);

                byte[] fileBytes = package.GetAsByteArray();
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = $"BaoCaoNPhuLieu.xlsx";


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
        public static void BorderEx(ExcelRange range)
        {
            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        }
        public static void Addpicutre2(ExcelWorksheet worksheet, string sign, int row, int col, int toado, string foder, int chieudai, int chieurong)
        {
            var pathFolder = foder;
            string imagePath = Path.Combine(pathFolder, sign);
            // Chèn ảnh vào worksheet
            if (File.Exists(imagePath))
            {
                FileInfo imageFile = new FileInfo(imagePath);
                ExcelPicture picture = worksheet.Drawings.AddPicture(Guid.NewGuid().ToString(), imageFile);
                picture.SetPosition(row, 5, col, toado);
                picture.SetSize(chieudai, chieurong);
            }
        }
    }
}