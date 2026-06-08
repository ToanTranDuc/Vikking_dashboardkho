using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Qty;
using NtbSoft.ERP.Model.Qty;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.Qty
{
    [RoutePrefix("api/KiemVai")]
    public class KiemVaiController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public dynamic Get(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "")
        {
            try
            {
                var ds = new QTYMaHangPhuLieuModel().GetVai(action, para1, para2, para3, para4, para5, para6, para7, para8);
                return new { dt1 = ds.Tables[0], dt2 = ds.Tables.Count > 1 ? ds.Tables[1] : new DataTable(), dt3 = ds.Tables.Count > 2 ? ds.Tables[2] : new DataTable(), datetime = DateTime.Now.ToString("dd-MM-yyyy") };
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
                return new QTYMaHangPhuLieuModel().SaveGopMH_KiemVai("Save", dt);
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
                List<QtyMaHangKiemVaiEntity> lstData = new List<QtyMaHangKiemVaiEntity>();
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
                                string uploadPath = HttpContext.Current.Server.MapPath("~/Images/KiemVai");
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
                    lstData.Add(new QtyMaHangKiemVaiEntity()
                    {
                        MaDH = data.MaDH.ToString(),
                        MaHang = data.MaHang.ToString(),
                        MaVai = data.MaVai.ToString(),
                        MaVTMau = data.MaVTMau.ToString(),
                        Dot = data.Dot,
                        SoCay = data.SoCay,
                        //GhiChu = data.NoiDung.ToString(),
                        MaLoi = item["Code"].ToString(),
                        MaVTri = item["ViTri"] ?? "",
                        DiemLoi = (int)item["Diem"],
                        NVKiem = data.NVKiem
                    });
                }
                var json = JsonConvert.SerializeObject(lstData);
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                new QTYMaHangPhuLieuModel().PostVai("SaveError", dt);
                return Ok(new { Status = "OK" });
            }
            catch (Exception ex)
            {
                string Message = $"Function: NguyenPhuLieuController/SaveSLKiem \nMessage: {ex.Message.ToString()}";
                // new WriteLogModel().WriteFileLog("InLine", Message);
                return Ok(new { Status = "Fail" });
            }
        }
        [HttpPost]
        [Route("SaveVai")]
        public IHttpActionResult SaveVai(dynamic data)
        {
            try
            {
                if (data is null) return null;
                List<QtyMaHangKiemVaiEntity> lstData = new List<QtyMaHangKiemVaiEntity>();

                lstData.Add(new QtyMaHangKiemVaiEntity()
                {
                    MaDH = data.MaDH.ToString(),
                    MaHang = data.MaHang.ToString(),
                    MaVai = data.MaVai.ToString(),
                    MaVTMau = data.MaVTMau ?? "",
                    SoLuong = data.SoLuong ?? 0,
                    SoCay = data.SoCay ?? "",
                    Kho = data.Kho ?? "",
                    Dot = data.Dot,
                    GhiChu = data.GhiChu ?? "",
                    NVKiem = data.NVKiem.ToString(),
                });

                var json = JsonConvert.SerializeObject(lstData);
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                new QTYMaHangPhuLieuModel().PostVai(data.Action.ToString(), dt);
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
        [Route("SaveSLTT")]
        public IHttpActionResult SaveSLTT(dynamic data)
        {
            try
            {
                if (data is null) return null;
                List<QtyMaHangKiemVaiEntity> lstData = new List<QtyMaHangKiemVaiEntity>();

                lstData.Add(new QtyMaHangKiemVaiEntity()
                {
                    Dot = data.Dot,
                    MaVai = data.MaVai.ToString(),
                    MaVTMau = data.MaVTMau ?? "",
                    SoCay = data.SoCay ?? "",
                    SoLuongTT = data.SoLuongTT,
                    KhoTT = data.KhoTT,
                    DiemLoi = data.DiemLoi,
                    GhiChu = data.GhiChu
                });

                var json = JsonConvert.SerializeObject(lstData);
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                new QTYMaHangPhuLieuModel().PostVai(data.Action.ToString(), dt);
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

        [Route("PostKiemVaiV2")]
        public string PostKiemVaiV2(string action, string type, dynamic data)
        {
            var json = JsonConvert.SerializeObject(data);

            // biến chung để return
            object listData;

            if (action == "Post" || action == "UpdateResult")
            {
                // Deserialize về đúng model
                var list = JsonConvert.DeserializeObject<List<QTY_KiemVaiV2Entity>>(json);

                string datetime = DateTime.Now.ToString("ddMMyyyyHHss");
                string maPhieu = ReplaceSpecialCharacterssize($"{data[0].LOT}{data[0].Batch}{data[0].SoCay}");

                // Text base64
                string ImgWeightText = data[0].ImgWeight;
                string ImgShrinkageText = data[0].ImgShrinkage;
                string ImgWaterproofText = data[0].ImgWaterproof;
                string ImgFaceSideText = data[0].ImgFaceSide;
                string ImgBlackSideText = data[0].ImgBlackSide;
                string ImgColorShadingText = data[0].ImgColorShading;
                string ImgColorText = data[0].ColorImg;
                string ImgPercentText = data[0].PercentImg;

                // Lưu ảnh → trả tên file
                string ImgWeight = SaveSignatureImage(ImgWeightText, "ImgWeight", maPhieu, datetime);
                string ImgShrinkage = SaveSignatureImage(ImgShrinkageText, "ImgShrinkage", maPhieu, datetime);
                string ImgWaterproof = SaveSignatureImage(ImgWaterproofText, "ImgWaterproof", maPhieu, datetime);
                string ImgFaceSide = SaveSignatureImage(ImgFaceSideText, "ImgFaceSide", maPhieu, datetime);
                string ImgBlackSide = SaveSignatureImage(ImgBlackSideText, "ImgBlackSide", maPhieu, datetime);
                string ImgColorShading = SaveSignatureImage(ImgColorShadingText, "ImgColorShading", maPhieu, datetime);
                string ImgColor = SaveSignatureImage(ImgColorText, "ColorImg", maPhieu, datetime);
                string ImgPercent = SaveSignatureImage(ImgPercentText, "PercentImg", maPhieu, datetime);

                // Gán lại cho list
                list[0].ImgWeight = ImgWeight;
                list[0].ImgShrinkage = ImgShrinkage;
                list[0].ImgWaterproof = ImgWaterproof;
                list[0].ImgFaceSide = ImgFaceSide;
                list[0].ImgBlackSide = ImgBlackSide;
                list[0].ImgColorShading = ImgColorShading;
                list[0].ColorImg = ImgColor;
                list[0].PercentImg = ImgPercent;

                if (list == null && list.Count == 0)
                {
                    return "";
                }

                listData = list;
            }
            else
            {
                // Khi action != Post hoặc != UpdateResult → dùng ErrorPointEntity
                listData = JsonConvert.DeserializeObject<List<QTY_KiemVaiV2_ErrorPointEntity>>(json);
            }

            // serialize lại và convert sang DataTable
            string jsonL = JsonConvert.SerializeObject(listData);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonL);

            // gọi xử lý DB
            return new QTYMaHangPhuLieuModel().PostVaiV2(action, dt, type);
        }
        [Route("PostXacNhanLo")]
        public string PostXacNhanLo(string action, string type, dynamic data)
        {
            var json = JsonConvert.SerializeObject(data);

            object listData;

            listData = JsonConvert.DeserializeObject<List<QTY_KiemVaiV2_XacNhanEntity>>(json);
            // serialize lại và convert sang DataTable
            string jsonL = JsonConvert.SerializeObject(listData);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonL);

            // gọi xử lý DB
            return new QTYMaHangPhuLieuModel().PostVaiV2(action, dt, type);
        }
        private string SaveSignatureImage(string base64Data, string role, string module, string datetime)
        {
            if (string.IsNullOrEmpty(base64Data)) return "";

            string[] parts = base64Data.Split(',');
            if (parts.Length < 2) return base64Data;

            byte[] bytes = Convert.FromBase64String(parts[1]);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Image image = Image.FromStream(ms);
                string fileName = $"{role}-{module}-{datetime}.png";
                string uploadPath = HttpContext.Current.Server.MapPath("~/Images/SignKiemVai");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                string fullPath = Path.Combine(uploadPath, fileName);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                image.Save(fullPath);
                return fileName;
            }
        }
        [HttpPost]
        [Route("PostSign")]
        public string PostSign(string action, string type, dynamic data)
        {
            string datetime = DateTime.Now.ToString("ddMMyyyyHHss");
            string maPhieu = ReplaceSpecialCharacterssize($"{data[0].SoLoID}{data[0].LoaiVai}{data[0].MaVTID}");

            string imageDataA = data[0].Sign;
            string imageDataB = data[0].SignManager;
            string imageDataC = data[0].SignReceive;
            string imageName = SaveSignatureImage(imageDataA, $"SignNgaykiem", maPhieu, datetime);
            string imageNameB = SaveSignatureImage(imageDataB, $"SignManager", maPhieu, datetime);
            string imageNameC = SaveSignatureImage(imageDataC, $"SignNhan", maPhieu, datetime);

            List<QTY_KiemVaiV2_SignEntity> lstData = new List<QTY_KiemVaiV2_SignEntity>();
            lstData.Add(new QTY_KiemVaiV2_SignEntity
            {
                SoLoID = data[0].SoLoID,
                Sign = imageName,
                LoaiVai = data[0].LoaiVai,
                MaVTID = data[0].MaVTID,
                MauVTID = data[0].MauVTID,
                SignManager = imageNameB,
                SignReceive = imageNameC,
                MaNPL = data[0].MaNPL
            });

            string jsonL = JsonConvert.SerializeObject(lstData);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonL);
            return new QTYMaHangPhuLieuModel().PostVaiV2(action, dt, type);
        }
        static string ReplaceSpecialCharacterssize(string input)
        {
            // Pattern để tìm khoảng trắng và các kí tự đặc biệt
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            // Thay thế bằng dấu _
            string replacement = "_";
            // Tạo một Regex và thực hiện thay thế
            Regex regex = new Regex(pattern);
            var value1 = regex.Replace(input, replacement);
            string value = RemoveVietnameseTone(value1);
            return value;
        }
        public static string RemoveVietnameseTone(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            string result = stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToUpper();
            result = result.Replace('đ', 'd');
            return result;
        }
        [HttpPost]
        [Route("SaveSign")]
        public IHttpActionResult SaveSign(dynamic data)
        {
            if (data is null) return null;
            string maHang = data?.MaHang?.ToString() ?? "";
            string imageName = ReplaceSpecialCharacterssize(maHang) + "_" + data.IdNguoiKy.ToString();
            string idNguoiKy = data.IdNguoiKy;
            string imageData = data.Image.ToString();
            List<QtyMaHangKiemVaiEntity> lstData = new List<QtyMaHangKiemVaiEntity>();
            if (!string.IsNullOrEmpty(imageData))
            {
                if (!imageData.Contains(".png"))
                {
                    byte[] bytes = Convert.FromBase64String(imageData.Split(',')[1]);

                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        Image image = Image.FromStream(ms);
                        string uploadPath = HttpContext.Current.Server.MapPath("~/Images/SignKiemVai");
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

            lstData.Add(new QtyMaHangKiemVaiEntity()
            {
                MaDH = data.MaDH.ToString(),
                MaHang = data.MaHang.ToString(),
                Dot = data.Dot,
                MaVai = data.MaVai.ToString(),
                Image = imageName,
                NVKiem = data.NVKiem.ToString()
            });

            var json = JsonConvert.SerializeObject(lstData);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            new QTYMaHangPhuLieuModel().PostVai("SaveSign", dt, idNguoiKy);
            return Ok(new { Status = "OK" });
        }
        [HttpPost]
        [Route("EXBCKTVaiV2")]
        public HttpResponseMessage EXBCKTVai(dynamic postData)
        {
            var foder = HttpContext.Current.Server.MapPath("~/Images/SignKiemVai");

            string jsonData = JsonConvert.SerializeObject(postData.ArrBody);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonData);
            string jsonDataError = JsonConvert.SerializeObject(postData.ArrBodyErrorPoint);
            DataTable tblError = JsonConvert.DeserializeObject<DataTable>(jsonDataError);

            string imagePathRelative = "/Content/Templates/BCKiemVaiV2.xlsx";
            string imagePathPhysical = HttpContext.Current.Server.MapPath("~" + imagePathRelative);

            FileInfo templateFile = new FileInfo(imagePathPhysical);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            using (ExcelPackage package = new ExcelPackage(templateFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                ExcelRange range = worksheet.Cells;
                worksheet.Cells["c4"].Value = postData.ArrThongSo[0].ToString();
                worksheet.Cells["m4"].Value = postData.ArrThongSo[2].ToString();
                worksheet.Cells["c5"].Value = postData.ArrThongSo[1].ToString();
                int startCol = 4;
                int row = 7;

                int currentCol = startCol;
               
                foreach (DataRow item in tbl.Rows)
                {
                    int col = currentCol;

                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    range.Value = item["MaVT"];

                    // Row 2: Màu VT
                    row++;
                    var statusColor = item["ColorStatus"] != DBNull.Value ? Convert.ToInt32(item["ColorStatus"]) : -1;
                    var colorStatus = statusColor == 1 ? "PASS" : statusColor == 0 ? "FAIL" : statusColor == 2 ? "NO" : "";
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    //range.Value = $"{weightStatus} : {item["Weight"]}";
                    string colorVal = item["ColorText"]?.ToString();
                    range.Value = string.IsNullOrWhiteSpace(colorVal) ? colorStatus : $"{colorStatus} : {colorVal}";

                    // Row 2: Màu VT
                    row++;
                    var statusLoiVai = item["PercentStatus"] != DBNull.Value ? Convert.ToInt32(item["PercentStatus"]) : -1;
                    var LoiVaiStatus = statusLoiVai == 1 ? "PASS" : statusLoiVai == 0 ? "FAIL" : statusLoiVai == 2 ? "NO" : "";
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    string loiVaiVal = item["PercentText"]?.ToString();
                    range.Value = string.IsNullOrWhiteSpace(colorVal) ? LoiVaiStatus : $"{LoiVaiStatus} : {loiVaiVal}";

                    // Row 3: Batch/LOT
                    row++;
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    range.Value = $"{item["Batch"]} / {item["LOT"]} / {item["SoCay"]}";

                    // Row 4: Weight (Pass/Fail + Value)
                    row++;
                    var statusWeight = item["StatusWeight"] != DBNull.Value ? Convert.ToInt32(item["StatusWeight"]) : -1;
                    var weightStatus = statusWeight == 1 ? "PASS" : statusWeight == 0 ? "FAIL" : statusWeight == 2 ? "NO" : "";
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    //range.Value = $"{weightStatus} : {item["Weight"]}";
                    string weightVal = item["Weight"]?.ToString();
                    range.Value = string.IsNullOrWhiteSpace(weightVal) ? weightStatus : $"{weightStatus} : {weightVal}";

                    // Row 5: Shrinkage (Pass/Fail + Value)
                    row++;
                    var statusShrinkage = item["StatusShrinkage"] != DBNull.Value ? Convert.ToInt32(item["StatusShrinkage"]) : -1;
                    var shrinkageStatus = statusShrinkage == 1 ? "PASS" : statusShrinkage == 0 ? "FAIL" : statusShrinkage == 2 ? "NO" : "";
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    //range.Value = $"{shrinkageStatus} : {item["Shrinkage"]}";
                    string shrinkageVal = item["Shrinkage"]?.ToString();
                    range.Value = string.IsNullOrWhiteSpace(shrinkageVal) ? shrinkageStatus : $"{shrinkageStatus} : {shrinkageVal}";

                    // Row 6: Waterproof (Pass/Fail + Value)
                    row++;
                    var statusWaterproof = item["StatusWaterproof"] != DBNull.Value ? Convert.ToInt32(item["StatusWaterproof"]) : -1;
                    var waterproofStatus = statusWaterproof == 1 ? "PASS" : statusWaterproof == 0 ? "FAIL" : statusWaterproof == 2 ? "NO" : "";
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    //range.Value = $"{waterproofStatus} : {item["Waterproof"]}";
                    string waterproofVal = item["Waterproof"]?.ToString();
                    range.Value = string.IsNullOrWhiteSpace(waterproofVal) ? waterproofStatus : $"{waterproofStatus} : {waterproofVal}";

                    // Row 7: Receiving Quantity
                    row++;
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    range.Value = item["ReceivingQuantity"];

                    // Row 8: Checking Quantity
                    row++;
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    range.Value = item["CheckingQuantity"];

                    // Row 9: Face Side (Checkbox)
                    row++;
                    var statusFaceSide = item["StatusFaceSide"] != DBNull.Value ? Convert.ToInt32(item["StatusFaceSide"]) : -1;
                    var FaceSideStatus = statusFaceSide == 1 ? "Yes" : statusFaceSide == 0 ? "No" : statusFaceSide == 2 ? "NO" : "";
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    //range.Value = $"{FaceSideStatus} : {item["FaceSide"]}";
                    string faceSideVal = item["FaceSide"]?.ToString();
                    range.Value = string.IsNullOrWhiteSpace(faceSideVal) ? FaceSideStatus : $"{FaceSideStatus} : {faceSideVal}";

                    // Row 10: Back Side (Checkbox)
                    row++;
                    var statusBlackSide = item["StatusBlackSide"] != DBNull.Value ? Convert.ToInt32(item["StatusBlackSide"]) : -1;
                    var BlackSideStatus = statusBlackSide == 1 ? "Yes" : statusBlackSide == 0 ? "No" : statusBlackSide == 2 ? "NO" : "";
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    //range.Value = $"{BlackSideStatus} : {item["BlackSide"]}";
                    string blackSideVal = item["BlackSide"]?.ToString();
                    range.Value = string.IsNullOrWhiteSpace(blackSideVal) ? BlackSideStatus : $"{BlackSideStatus} : {blackSideVal}";

                    // Row 11: Roll Label
                    row++;
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    range.Value = $"{item["RollLabel"]} / {item["KhoVai"]}";

                    // Row 12: Roll Actual
                    row++;
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    range.Value = $"{item["RollActual"]} / {item["KhoVaiActual"]}";

                    // Row 13: Cut Protect & Color Thread (2 cột)
                    row++;
                    range = worksheet.Cells[row, col, row, col + 1];
                    range.Merge = true;
                    range.Value = item["CutProctect"];

                    range = worksheet.Cells[row, col + 2, row, col + 3];
                    range.Merge = true;
                    range.Value = item["ColorThread"];

                    // Row 14: Color Shading
                    row++;
                    var statusColorShading = item["StatusColorShading"] != DBNull.Value ? Convert.ToInt32(item["StatusColorShading"]) : -1;
                    var ColorShadingStatus = statusColorShading == 1 ? "PASS" : statusColorShading == 0 ? "FAIL" : statusColorShading == 2 ? "NO" : "";
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    //range.Value = $"{ColorShadingStatus} : {item["ColorShading"]}";
                    string colorShadingVal = item["ColorShading"]?.ToString();
                    range.Value = string.IsNullOrWhiteSpace(colorShadingVal) ? ColorShadingStatus : $"{ColorShadingStatus} : {colorShadingVal}";

                    // Row 15: Joining Points
                    row++;
                    range = worksheet.Cells[row, col, row, col + 3];
                    range.Merge = true;
                    range.Value = item["JoiningPointsRoll"];
                    row++;
                    for (int i = 0; i < 4; i++)
                    {
                        range = worksheet.Cells[row, col + i];
                        range.Value = i + 1;
                    }

                    // Reset row về đầu cho item tiếp theo
                    row = 7;
                    currentCol += 4;
                }

                // Xử lý Error Points (defects) - bắt đầu từ row 16

                if (tblError != null && tblError.Rows.Count > 0)
                {
                    int defectStartRow = 23;
                    var defectTypes = tblError.AsEnumerable()
                        .Select(r => r["ErrorType"].ToString())
                        .Distinct()
                        .ToList();

                    currentCol = startCol;
                    foreach (DataRow item in tbl.Rows)
                    {
                        int defectRow = defectStartRow;
                        string idErrorType = item["IDErrorType"].ToString();

                        foreach (var defectType in defectTypes)
                        {
                            var errorData = tblError.AsEnumerable()
                                .FirstOrDefault(r => r["IDErrorType"].ToString() == idErrorType.ToString()
                                                  && r["ErrorType"].ToString() == defectType);

                            if (errorData != null && errorData["ErrorPoint"] != null)
                            {
                                string[] points = errorData["ErrorPoint"].ToString().Split('@');

                                // Ghi 4 điểm vào 4 cột
                                for (int i = 0; i < Math.Min(points.Length, 4); i++)
                                {
                                    string value = points[i];

                                    if (string.IsNullOrWhiteSpace(value) || value == "0")
                                    {
                                        worksheet.Cells[defectRow, currentCol + i].Value = "";
                                        continue;
                                    }

                                    if (int.TryParse(value, out int number))
                                    {
                                        worksheet.Cells[defectRow, currentCol + i].Value = number;
                                    }
                                    else
                                    {
                                        worksheet.Cells[defectRow, currentCol + i].Value = ""; // hoặc ghi value nếu bạn muốn
                                    }
                                }
                            }

                            defectRow++;
                        }

                        currentCol += 4;
                    }

                    int noteRow = defectStartRow + defectTypes.Count;
                    currentCol = startCol;
                }
                row = 30;
                foreach (DataRow item in tbl.Rows)
                {
                    range = worksheet.Cells[row, currentCol, row + 2, currentCol + 3];
                    range.Merge = true;
                    range.Value = item["Note"];
                    range.Style.WrapText = true;
                    currentCol += 4;
                }

                if (postData.ArrThongSo[4].ToString() != "") Addpicutre2(worksheet, postData.ArrThongSo[4].ToString(), 34, 6, 40, foder, 110, 55);
                if (postData.ArrThongSo[5].ToString() != "") Addpicutre2(worksheet, postData.ArrThongSo[5].ToString(), 37, 1, 100, foder, 110, 55);
                if (postData.ArrThongSo[5].ToString() != "") Addpicutre2(worksheet, postData.ArrThongSo[5].ToString(), 37, 6, 40, foder, 110, 55);
                range = worksheet.Cells[7, 4, 33, 3 + tbl.Rows.Count * 4];
                BorderEx(range);
                range = worksheet.Cells["A36"]; range.Value = Convert.ToBoolean(postData.ArrThongSo[6].ToString()) ? "Pass" : "";
                range = worksheet.Cells["F36"]; range.Value = Convert.ToBoolean(postData.ArrThongSo[7].ToString()) ? "Fail" : "";
                string checkedValue = tbl.Rows[0]["KetLuan_Mer"].ToString();
                string ketQua = checkedValue == "1" ? "Pass" : checkedValue == "2" ? "Fail" : "";
                string valueMer = tbl.Rows[0]["GhiChu_Mer"].ToString();
                string template = worksheet.Cells["J36"].Value?.ToString();
             

                string result = template.Replace("{0}", ketQua).Replace("{1}", valueMer);

                worksheet.Cells["J36"].Value = result;

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