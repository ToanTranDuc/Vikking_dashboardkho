using Newtonsoft.Json;
using NtbSoft.ERP.Model.ThuVien;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Net.Http.Headers;


namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/PhieuXuatHangNPL")]
    public class PhieuXuatHangNPLController : ApiController
    {
        private const ExcelBorderStyle thin = ExcelBorderStyle.Thin;

        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "")
        {
            return new PhieuXuatHangNPLModel().Get(action, para1, para2, para3, para4, para5, para6, para7, para8);
        }
        [HttpGet]
        [Route("GetTH")]
        public DataTable GetTH(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "")
        {
            return new PhieuXuatHangNPLModel().GetTH(action, para1, para2, para3, para4, para5, para6, para7, para8);
        }
        [HttpGet]
        [Route("GetDS")]
        public DataSet GetDS(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "")
        {
            return new PhieuXuatHangNPLModel().GetDTS(action, para1, para2, para3, para4, para5, para6, para7, para8);
        }
        [HttpGet]
        [Route("GetTheKhoNPL")]
        public DataTable GetTheKhoNPL(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "")
        {
            return new PhieuXuatHangNPLModel().GetTheKhoNPL(action, para1, para2, para3, para4, para5, para6);
        }
        [HttpGet]
        [Route("Delete")]
        public string Delete(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "")
        {
            return new PhieuXuatHangNPLModel().Delete(action, para1, para2, para3, para4, para5, para6, para7, para8);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action, string para1, dynamic data)
        {
            string json = JsonConvert.SerializeObject(data);
            var listData = JsonConvert.DeserializeObject<List<PhieuXuatHangNguyenPhuLIeu>>(json);
            string jsonL = JsonConvert.SerializeObject(listData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonL);

            return new PhieuXuatHangNPLModel().Post(action, tbl, para1);
        }
        [HttpPost]
        [Route("PostWeb")]
        public string PostWeb(string action, string para1, dynamic data)
        {
            DataTable tbl;
            string json;
            if (action == "PostVT")
            {
                json = JsonConvert.SerializeObject(data);
                var listData = JsonConvert.DeserializeObject<List<ChiTietNhapKhoNPL>>(json);
                string jsonL = JsonConvert.SerializeObject(listData);
                tbl = JsonConvert.DeserializeObject<DataTable>(jsonL);
            }
            else
            {
                json = JsonConvert.SerializeObject(data);
                tbl = JsonConvert.DeserializeObject<DataTable>(json);
            }

            return new PhieuXuatHangNPLModel().PostWeb(action, tbl, para1);
        }
        [HttpPost]
        [Route("PostTH")]
        public int PostTH(string action, string para1, dynamic data)
        {
            string json = JsonConvert.SerializeObject(data);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            return new PhieuXuatHangNPLModel().PostTH(action, tbl, para1);
        }
        [HttpPost]
        [Route("PostTHPL")]
        public int PostTHPL(string action, string para1, dynamic data)
        {
            string json = JsonConvert.SerializeObject(data);
            var listData = JsonConvert.DeserializeObject<List<PhieuThuHoiModel>>(json);
            string jsonL = JsonConvert.SerializeObject(listData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonL);

            return new PhieuXuatHangNPLModel().PostTHPL(action, tbl, para1);
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
        [Route("PostSoanHang")]
        public string PostSoanHang(string action, dynamic data)
        {
            string datetime = DateTime.Now.ToString("ddMMyyyyHHss");
            string maPhieu = $"{data[0].PhieuSH}{data[0].MaNPL}";
            string imageData = data[0].KyTen;
            string imageName = SaveSignatureImage(imageData, $"KiTen", maPhieu, datetime);
            string json = JsonConvert.SerializeObject(data);
            var listData = JsonConvert.DeserializeObject<List<ERP_PhieuSoanHangVatTu_Type>>(json);
            string jsonL = JsonConvert.SerializeObject(listData);

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonL);
            foreach (DataRow row in tbl.Rows)
            {
                row["KyTen"] = imageName;
            }
            return new PhieuXuatHangNPLModel().PostSoanHang(action, tbl);
        }

        [HttpPost]
        [Route("PostXHKT")]
        public string PostXHKT(string action, dynamic data)
        {
            string imageDataA = data[0].KyTen;
            string[] imageDataParts = imageDataA.Split(',');
            string imageName = "";
            string datetime = DateTime.Now.ToString("ddMMyyyyHHss");
            if (!string.IsNullOrEmpty(imageDataA))
            {
                byte[] bytes = Convert.FromBase64String(imageDataParts[1]);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    string tenhinh = $"{data[0].UserXH}-{datetime}";
                    Image image = Image.FromStream(ms);
                    string uploadPath = HttpContext.Current.Server.MapPath("~/Images/XuatHangNPL/KyTen");
                    if (!System.IO.Directory.Exists(uploadPath))
                        System.IO.Directory.CreateDirectory(uploadPath);
                    imageName = tenhinh + ".png";
                    var mPath = string.Format(@"{0}\{1}", uploadPath, imageName);
                    if (File.Exists(mPath))
                    {
                        File.Delete(mPath);
                    }
                    image.Save(mPath);
                }
            }
            List<PhieuXuatHangKT> lstData = new List<PhieuXuatHangKT>();
            foreach (var item in data)
            {
                lstData.Add(new PhieuXuatHangKT
                {
                    PhieuXH = item.PhieuXH,
                    SortXH = 1,
                    BarCode = item.BarCode,
                    Dot = item.Dot,
                    IsNPL = item.IsNPL,
                    KyTen = imageName,
                    PhieuYC = item.PhieuYC,
                    UserXH = item.UserXH
                });
            }
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new PhieuXuatHangNPLModel().PostXHKT(action, tbl);
        }
        [HttpPost]
        [Route("PostPhieuXHPL")]
        public string PostPhieuXHPL(string action, string para1, dynamic data)
        {
            string imageDataA = data[0].Sign;
            string[] imageDataParts = imageDataA.Split(',');
            string imageName = "";
            string datetime = DateTime.Now.ToString("ddMMyyyyHHss");
            if (!string.IsNullOrEmpty(imageDataA))
            {
                byte[] bytes = Convert.FromBase64String(imageDataParts[1]);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    string tenhinh = $"{para1}-{datetime}";
                    Image image = Image.FromStream(ms);
                    string uploadPath = HttpContext.Current.Server.MapPath("~/Images/XuatHangPhuLieu/KyTen");
                    if (!System.IO.Directory.Exists(uploadPath))
                        System.IO.Directory.CreateDirectory(uploadPath);
                    imageName = tenhinh + ".png";
                    var mPath = string.Format(@"{0}\{1}", uploadPath, imageName);
                    if (File.Exists(mPath))
                    {
                        File.Delete(mPath);
                    }
                    image.Save(mPath);
                }
            }
            List<PhieuXuatHangPL> lstData = new List<PhieuXuatHangPL>();
            foreach (var item in data)
            {
                lstData.Add(new PhieuXuatHangPL
                {
                    PhieuXHPL = item.PhieuXHPL,
                    PhieuYC = item.PhieuYC,
                    MaLenh = item.MaLenh,
                    MaGop = item.MaGop,
                    MaLenhSX = item.MaLenhSX,
                    MaNPL = item.MaNPL,
                    SLNhap = item.SLNhap ?? 1,
                    Module = item.Module ?? 1,
                    Sort = 1,               // gán cứng như bạn ví dụ
                    Sign = imageName,       // gán tên ảnh
                    NgayXuatHang = "",
                    Dot = item.Dot,
                    SoLoID = item.SoLoID
                });
            }
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new PhieuXuatHangNPLModel().PostXHPLPhieu(action, para1, tbl);
        }
        [HttpPost]
        [Route("DeletePhieu")]
        public string DeletePhieuXHPL(dynamic data)
        {

            List<PhieuXuatHangPL> lstData = new List<PhieuXuatHangPL>();
            foreach (var item in data)
            {
                lstData.Add(new PhieuXuatHangPL
                {
                    PhieuXHPL = item.PhieuXHPL,
                    PhieuYC = item.PhieuYC,
                    MaLenh = item.MaLenh,
                    MaGop = item.MaGop,
                    MaLenhSX = item.MaLenhSX,
                    MaNPL = item.MaNPL,
                    SLNhap = item.SLNhap,
                    Module = item.Module,
                    Sort = 1,               // gán cứng như bạn ví dụ
                    Sign = "",       // gán tên ảnh
                    NgayXuatHang = "",
                    Dot = item.Dot,
                    SoLoID = item.SoLoID
                });
            }
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new PhieuXuatHangNPLModel().DeleteXHPLPhieu(tbl);
        }
        [HttpGet]
        [Route("GetEX")]
        public HttpResponseMessage GetEX(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "")
        {
            try
            {
                DataTable dtTableT = new PhieuXuatHangNPLModel().GetTH("GetEXT", para1, para2, para3, para4, para5, para6, para7, para8);
                DataTable dtTable = new PhieuXuatHangNPLModel().GetTH("GetChiTietXH", para1, para2, para3, para4, para5, para6, para7, para8);

                string imagePathRelative = "/Content/Templates/PhieuXuatHang.xlsx";
                string imagePathPhysical = HttpContext.Current.Server.MapPath("~" + imagePathRelative);
                var foder = HttpContext.Current.Server.MapPath("~/Images/XuatHangNPL/KyTen");
                string datetime = DateTime.Now.ToString("HHmmss");

                FileInfo templateFile = new FileInfo(imagePathPhysical);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                var uniqueValues = dtTable.AsEnumerable().
                  Select(x => new
                  {
                      PhieuXH = x["PhieuXH"],
                      MaPhieu = x["MaPhieu"]
                  }).Distinct().ToList();

                using (ExcelPackage package = new ExcelPackage(templateFile))
                {

                    var templateWorksheet = package.Workbook.Worksheets[0];
                    HashSet<string> createdWorksheets = new HashSet<string>();
                    ExcelWorksheet worksheet = templateWorksheet;
                    ExcelRange range = worksheet.Cells;

                    worksheet = package.Workbook.Worksheets.Add("PhieuXK", templateWorksheet);
                    createdWorksheets.Add("PhieuXK");
                    string[] datatime = DateTime.Now.ToString("dd/MM/yyyy").Split('/');
                    range = worksheet.Cells["j5"];
                    range.Value = $"Ngày {datatime[0]} tháng {datatime[1]} năm {datatime[2]}";
                    range = worksheet.Cells["A12"];
                    range = worksheet.Cells["B12:P12"]; range.Merge = true; range.Value = dtTableT.Rows[0]["TenHang"];
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    range.Style.Font.Bold = true; range.Style.Font.Size = 14;
                    Ex(worksheet, range, dtTableT, foder, 13);

                    foreach (var item in uniqueValues)
                    {
                        string PhieuXH = item.PhieuXH.ToString();
                        string nameSheet = $"{item.PhieuXH} - {item.MaPhieu}";
                        DataTable tblPXH = dtTable.AsEnumerable().Where(x => x["PhieuXH"].ToString() == PhieuXH).CopyToDataTable();


                        if (!createdWorksheets.Contains(PhieuXH))
                        {
                            worksheet = package.Workbook.Worksheets.Add(nameSheet, templateWorksheet);
                            createdWorksheets.Add(nameSheet);
                        }
                        else
                        {
                            worksheet = package.Workbook.Worksheets[PhieuXH];
                        }
                        range = worksheet.Cells["j5"];
                        range.Value = $"Ngày {datatime[0]} tháng {datatime[1]} năm {datatime[2]}";
                        Ex(worksheet, range, tblPXH, foder, 12);
                    }
                    package.Workbook.Worksheets.Delete(0);
                    byte[] fileBytes = package.GetAsByteArray();


                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"PhieuXuatHang{datetime}.xlsx";

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
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }

        [HttpGet]
        [Route("GetEXPL")]
        public HttpResponseMessage GetEXPL(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "")
        {
            try
            {
                DataTable dtTable = new PhieuXuatHangNPLModel().GetTH("GetEXPL", para1, para2, para3, para4, para5, para6, para7, para8);

                string imagePathRelative = "/Content/Templates/PhieuXuatHang.xlsx";
                string imagePathPhysical = HttpContext.Current.Server.MapPath("~" + imagePathRelative);
                var foder = HttpContext.Current.Server.MapPath("~/Images/XuatHangNPL/KyTen");
                string datetime = DateTime.Now.ToString("HHmmss");

                FileInfo templateFile = new FileInfo(imagePathPhysical);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage package = new ExcelPackage(templateFile))
                {

                    var templateWorksheet = package.Workbook.Worksheets[0];
                    HashSet<string> createdWorksheets = new HashSet<string>();
                    ExcelWorksheet worksheet = templateWorksheet;
                    ExcelRange range = worksheet.Cells;

                    worksheet = package.Workbook.Worksheets.Add("PhieuXK", templateWorksheet);
                    createdWorksheets.Add("PhieuXK");
                    string[] datatime = DateTime.Now.ToString("dd/MM/yyyy").Split('/');
                    range = worksheet.Cells["j5"];
                    range.Value = $"Ngày {datatime[0]} tháng {datatime[1]} năm {datatime[2]}";
                    range = worksheet.Cells["A12"];
                    range = worksheet.Cells["B12:P12"]; range.Merge = true; range.Value = dtTable.Rows[0]["TenHang"];
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    range.Style.Font.Bold = true; range.Style.Font.Size = 14;

                    Ex(worksheet, range, dtTable, foder, 13);


                    package.Workbook.Worksheets.Delete(0);
                    byte[] fileBytes = package.GetAsByteArray();


                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"PhieuXuatHangPhuLieu{datetime}.xlsx";

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
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }

        private void Addpicutre(ExcelWorksheet worksheet, string sign, int row, int col, int toado, string foder, int chieudai, int chieurong)
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
        private void Ex(ExcelWorksheet worksheet, ExcelRange range, DataTable tbl, string forder, int row)
        {
            int index = 1;
            foreach (DataRow dr in tbl.Rows)
            {
                worksheet.InsertRow(row, 1);
                worksheet.Cells[row, 1].Value = index;
                range = worksheet.Cells[row, 2, row, 3]; range.Merge = true;
                range.Value = dr["MaVT"].ToString(); range.Style.WrapText = true;
                range = worksheet.Cells[row, 4, row, 8]; range.Merge = true;
                range.Value = dr["ChiTiet"].ToString(); range.Style.WrapText = true;
                worksheet.Cells[row, 9].Value = dr["MauVT"].ToString(); worksheet.Cells[row, 9].Style.WrapText = true;
                worksheet.Cells[row, 10].Value = dr["KhoVai"].ToString();
                worksheet.Cells[row, 11].Value = dr["SLYC"].ToString();
                worksheet.Cells[row, 12].Value = dr["SLXH"].ToString();
                worksheet.Cells[row, 13].Value = dr["DonGia"].ToString();
                worksheet.Cells[row, 14].Value = dr["ThanhTien"].ToString();
                range = worksheet.Cells[row, 15, row, 16]; range.Merge = true;
                range.Value = "";
                worksheet.Row(row).Height = -1;
                index++;
                row++;
            }
            if (tbl.Rows[0]["KyTen"].ToString() != "") Addpicutre(worksheet, tbl.Rows[0]["KyTen"].ToString(), row + 2, 10, -10, forder, 100, 100);

            var borderData = worksheet.Cells[12, 1, row - 1, 16].Style.Border;
            borderData.Bottom.Style =
                borderData.Top.Style =
                borderData.Left.Style =
                borderData.Right.Style = ExcelBorderStyle.Thin;
        }

        // Kiệt
        [HttpPost]
        [Route("PostKiemKeNPL")]
        public string PostKiemKeNPL(string action, PhieuKiemKeNPL data)
        {
            data.DateKiemKKe = DateTime.Now;
            data.SLKiemKeEdit = data.SLKiemKeEdit ?? null;
            data.GhiChu = data.GhiChu ?? null;
            data.IsXacNhan = data.IsXacNhan ?? null;

            var lstData = new List<PhieuKiemKeNPL> { data };

            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            return new PhieuXuatHangNPLModel().PostKiemKeNPL(action, tbl);
        }

        [HttpPost]
        [Route("PostChiTietKiemKeNPL")]
        public string PostChiTietKiemKeNPL(string action, dynamic data)
        {
            List<PhieuKiemKeNPL> lstData = new List<PhieuKiemKeNPL>();

            foreach (var item in data)
            {
                lstData.Add(new PhieuKiemKeNPL
                {
                    SoLoID = item.SoLoID,
                    SoLo = item.SoLo,
                    MaNPL = item.MaNPL,
                    MaVTID = item.MaVTID,
                    MauVTID = item.MauVTID,
                    IsNPL = item.IsNPL,
                    BarCode = item.BarCode,
                    SLKiemKe = item.SLKiemKe,
                    SLKiemKeEdit = item.SLKiemKeEdit,
                    GhiChu = item.GhiChu,
                    UserKK = item.UserKK,
                    DateKiemKKe = item.DateKiemKKe,
                    IsXacNhan = item.IsXacNhan,
                    PhieuKiemKe = item.PhieuKiemKe
                });
            }

            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            return new PhieuXuatHangNPLModel().PostKiemKeNPL(action, tbl);
        }

        [HttpGet]
        [Route("GetEXXH")]
        public HttpResponseMessage GetEXXH(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "")
        {
            try
            {
                // Lấy dữ liệu
                DataTable thongTinDonHang = new PhieuXuatHangNPLModel().Get("GetThongTinDonHang", para1, para2, para3, para4, para5, para6, para7, para8);
                DataTable dtTable1 = new PhieuXuatHangNPLModel().Get("GetChiTietLenh", para2, para2, para3, para4, para5, para6, para7, para8);
                DataTable dtTable2 = new PhieuXuatHangNPLModel().Get("GetEXTB3", para1, para2, para3, para4, para5, para6, para7, para8);

                string datetime = DateTime.Now.ToString("HHmmss");

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Xuất Hàng NL");

                    // Set default row height
                    worksheet.DefaultRowHeight = 22;

                    // Set column widths
                    worksheet.Column(1).Width = 14;   // A
                    worksheet.Column(2).Width = 20;   // B
                    worksheet.Column(3).Width = 30;   // C
                    worksheet.Column(4).Width = 24;   // D
                    worksheet.Column(5).Width = 40;   // E
                    worksheet.Column(6).Width = 24;   // F
                    worksheet.Column(7).Width = 18;   // G
                    worksheet.Column(8).Width = 18;   // H
                    worksheet.Column(9).Width = 18;   // I
                    worksheet.Column(10).Width = 18;  // J
                    worksheet.Column(11).Width = 18;  // K
                    worksheet.Column(12).Width = 18;  // L
                    worksheet.Column(13).Width = 18;  // M
                    worksheet.Column(14).Width = 18;  // N

                    // ====== THÊM HEADER CÔNG TY (ROW 1-3) ======

                    // ROW 1: CÔNG TY TNHH VIKING VIỆT NAM
                    worksheet.Cells["A1:C2"].Merge = true;
                    var companyCell = worksheet.Cells["A1:C2"];
                    companyCell.Value = "CÔNG TY TNHH VIKING VIỆT NAM";
                    companyCell.Style.Font.Bold = true;
                    companyCell.Style.Font.Size = 14;
                    companyCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    companyCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;



                    // ROW 3: TIÊU ĐỀ CHÍNH
                    worksheet.Cells["E3:G3"].Merge = true;
                    var titleMainCell = worksheet.Cells["E3:G3"];
                    var borderTitle = worksheet.Cells["A3:N3"];
                    titleMainCell.Value = "Lệnh Sản Xuất và Cấp Phát " + (int.Parse(para3) == 1 ? "Nguyên Liệu" : "Phụ Liệu");

                    titleMainCell.Style.Font.Bold = true;
                    titleMainCell.Style.Font.Size = 13;
                    titleMainCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    titleMainCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    borderTitle.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    borderTitle.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);

                    // ROW 4: Trống
                    worksheet.Row(4).Height = 10;

                    // ====== THÔNG TIN ĐƠN HÀNG (A5:C10) - Dịch xuống từ row 1 ======
                    var row = thongTinDonHang.Rows[0];

                    worksheet.Cells["A5"].Value = "MÃ HÀNG/STYLE";
                    worksheet.Cells["C5"].Value = row["MaHang"]?.ToString() ?? "";
                    worksheet.Cells["A6"].Value = "MÙA/SEASON";
                    worksheet.Cells["C6"].Value = row["Dot"]?.ToString() ?? "";
                    worksheet.Cells["A7"].Value = "FACTORY/XÍ NGHIỆP";
                    worksheet.Cells["C7"].Value = row["TenDVSX"]?.ToString() ?? "";
                    worksheet.Cells["A8"].Value = "CHỦNG LOẠI";
                    worksheet.Cells["C8"].Value = row["TenCL"]?.ToString() ?? "";
                    worksheet.Cells["A9"].Value = "SỐ BOOKING/BUY";
                    worksheet.Cells["C9"].Value = "";
                    worksheet.Cells["A10"].Value = "GHI CHÚ";
                    worksheet.Cells["C10"].Value = row["GhiChu"]?.ToString() ?? "";

                    // Format thông tin đơn hàng (rows 5-10)
                    for (int i = 5; i <= 10; i++)
                    {
                        // Merge A:B
                        worksheet.Cells[$"A{i}:B{i}"].Merge = true;
                        var cellA = worksheet.Cells[$"A{i}:B{i}"];
                        cellA.Style.Font.Bold = true;
                        cellA.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        cellA.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        cellA.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        cellA.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        cellA.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        cellA.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                        // Format value cells (C5-C10)
                        var cellC = worksheet.Cells[$"C{i}"];
                        cellC.Style.Font.Bold = true;
                        cellC.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        cellC.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        cellC.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        cellC.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        cellC.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        cellC.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        cellC.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }

                    // Lấy danh sách Size động
                    var sizeHeaders = dtTable1.AsEnumerable()
                        .Select(r => r["Size"]?.ToString())
                        .Distinct()
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList();

                    // Tính toán vị trí cột
                    int fixedColCount = 6;
                    int startSizeCol = fixedColCount + 1;
                    int endSizeCol = startSizeCol + sizeHeaders.Count - 1;
                    int grandTotalCol = endSizeCol + 1;

                    // ====== ROW 12: HEADER CHÍNH (Dịch xuống từ row 8) ======
                    worksheet.Cells["A12:A13"].Merge = true;
                    worksheet.Cells["A12:A13"].Value = "COLOR\nCODE";
                    ApplyBorder(worksheet.Cells["A12:A13"]);

                    worksheet.Cells["B12:B13"].Merge = true;
                    worksheet.Cells["B12:B13"].Value = "COLOR";
                    ApplyBorder(worksheet.Cells["B12:B13"]);

                    worksheet.Cells["C12:C13"].Merge = true;
                    worksheet.Cells["C12:C13"].Value = "PO";
                    ApplyBorder(worksheet.Cells["C12:C13"]);

                    worksheet.Cells["D12:D13"].Merge = true;
                    worksheet.Cells["D12:D13"].Value = "SIZE\n/INSEAM";
                    ApplyBorder(worksheet.Cells["D12:D13"]);

                    worksheet.Cells["E12:E13"].Merge = true;
                    worksheet.Cells["E12:E13"].Value = "COUNTRY";
                    ApplyBorder(worksheet.Cells["E12:E13"]);

                    worksheet.Cells["F12:F13"].Merge = true;
                    worksheet.Cells["F12:F13"].Value = "Ngày GH";
                    ApplyBorder(worksheet.Cells["F12:F13"]);

                    // Merge cột Size động
                    if (sizeHeaders.Count > 1)
                    {
                        worksheet.Cells[12, startSizeCol, 12, endSizeCol].Merge = true;
                    }
                    var sizeCell = worksheet.Cells[12, startSizeCol, 12, endSizeCol];
                    sizeCell.Value = "Size";
                    sizeCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sizeCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    sizeCell.Style.Font.Bold = true;
                    sizeCell.Style.Font.Size = 11;
                    sizeCell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    ApplyBorder(sizeCell);

                    // Grand Total
                    worksheet.Cells[12, grandTotalCol, 13, grandTotalCol].Merge = true;
                    var grandTotalCell = worksheet.Cells[12, grandTotalCol, 13, grandTotalCol];
                    grandTotalCell.Value = "Grand Total";
                    grandTotalCell.Style.Font.Bold = true;
                    grandTotalCell.Style.Font.Size = 11;
                    grandTotalCell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    grandTotalCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    grandTotalCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ApplyBorder(grandTotalCell);

                    // ====== ROW 13: SUB-HEADER SIZE ======
                    for (int i = 0; i < sizeHeaders.Count; i++)
                    {
                        var cell = worksheet.Cells[13, startSizeCol + i];
                        cell.Value = sizeHeaders[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.Size = 11;
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ApplyBorder(cell);
                    }

                    // Style cho header cố định (A12-F12)
                    string[] fixedHeaders = { "A12", "B12", "C12", "D12", "E12", "F12" };
                    foreach (var addr in fixedHeaders)
                    {
                        var cell = worksheet.Cells[addr];
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.Size = 11;
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        cell.Style.WrapText = true;
                        ApplyBorder(cell);
                    }

                    // GROUP DATA
                    var groupedData = new Dictionary<string, GroupedRow>();

                    foreach (DataRow item in dtTable1.Rows)
                    {
                        string po = item["PO"]?.ToString() ?? "";
                        string tenMau = item["TenMau"]?.ToString() ?? "";
                        string dauSize = item["DauSize"]?.ToString() ?? "0";
                        string key = $"{po}_{tenMau}_{dauSize}";

                        if (!groupedData.ContainsKey(key))
                        {
                            groupedData[key] = new GroupedRow
                            {
                                ColorCode = item["ColorCode"]?.ToString() ?? "",
                                Color = tenMau,
                                PO = po,
                                SizeInseam = dauSize,
                                Country = item["TenQG"]?.ToString() ?? "",
                                NgayGH = item["NgayGH"] != DBNull.Value
                                    ? Convert.ToDateTime(item["NgayGH"]).ToString("dd/MM/yyyy")
                                    : "",
                                Sizes = new Dictionary<string, int>()
                            };
                        }

                        string sizeName = item["Size"]?.ToString() ?? "";
                        int sl = item["SL"] != DBNull.Value ? Convert.ToInt32(item["SL"]) : 0;

                        if (!groupedData[key].Sizes.ContainsKey(sizeName))
                        {
                            groupedData[key].Sizes[sizeName] = 0;
                        }
                        groupedData[key].Sizes[sizeName] += sl;
                    }

                    var dataRows = groupedData.Values.ToList();

                    // GHI DỮ LIỆU VÀO EXCEL (Bắt đầu từ row 14)
                    int dataRow = 14;
                    var sizeTotals = new Dictionary<string, int>();
                    int grandTotalSum = 0;

                    foreach (var size in sizeHeaders)
                    {
                        sizeTotals[size] = 0;
                    }

                    foreach (var item in dataRows)
                    {
                        worksheet.Cells[dataRow, 1].Value = item.ColorCode;
                        worksheet.Cells[dataRow, 2].Value = item.Color;
                        worksheet.Cells[dataRow, 3].Value = item.PO;
                        worksheet.Cells[dataRow, 4].Value = item.SizeInseam;
                        worksheet.Cells[dataRow, 5].Value = item.Country;
                        worksheet.Cells[dataRow, 6].Value = item.NgayGH;

                        int rowTotal = 0;
                        for (int i = 0; i < sizeHeaders.Count; i++)
                        {
                            string size = sizeHeaders[i];
                            int value = item.Sizes.ContainsKey(size) ? item.Sizes[size] : 0;

                            var cell = worksheet.Cells[dataRow, startSizeCol + i];
                            cell.Value = value;

                            if (value == 0)
                            {
                                cell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            }
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ApplyBorder(cell);

                            rowTotal += value;
                            sizeTotals[size] += value;
                        }

                        var gtCell = worksheet.Cells[dataRow, grandTotalCol];
                        gtCell.Value = rowTotal;
                        gtCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        gtCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ApplyBorder(gtCell);

                        grandTotalSum += rowTotal;

                        for (int col = 1; col <= 6; col++)
                        {
                            var cell = worksheet.Cells[dataRow, col];
                            ApplyBorder(cell);
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }

                        dataRow++;
                    }

                    // GRAND TOTAL ROW
                    int totalRow = dataRow;
                    worksheet.Cells[totalRow, 1, totalRow, 6].Merge = true;
                    var totalCell = worksheet.Cells[totalRow, 1, totalRow, 6];
                    totalCell.Value = "Grand Total";
                    totalCell.Style.Font.Bold = true;
                    totalCell.Style.Font.Size = 11;
                    totalCell.Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    totalCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    totalCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFC7CE"));
                    totalCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    totalCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ApplyBorder(totalCell);

                    for (int i = 0; i < sizeHeaders.Count; i++)
                    {
                        string size = sizeHeaders[i];
                        var cell = worksheet.Cells[totalRow, startSizeCol + i];
                        cell.Value = sizeTotals[size];
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.Size = 11;
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFC7CE"));
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ApplyBorder(cell);
                    }

                    var gtTotalCell = worksheet.Cells[totalRow, grandTotalCol];
                    gtTotalCell.Value = grandTotalSum;
                    gtTotalCell.Style.Font.Bold = true;
                    gtTotalCell.Style.Font.Size = 11;
                    gtTotalCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    gtTotalCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFC7CE"));
                    gtTotalCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    gtTotalCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ApplyBorder(gtTotalCell);

                    // DANH SÁCH NGUYÊN PHỤ LIỆU
                    int nplStartRow = totalRow + 3;

                    worksheet.Cells[nplStartRow, 1, nplStartRow, 11].Merge = true;
                    var titleNplCell = worksheet.Cells[nplStartRow, 1, nplStartRow, 11];
                    titleNplCell.Value = "DANH SÁCH " + (int.Parse(para3) == 1 ? "NGUYÊN LIỆU" : "PHỤ LIỆU");
                    titleNplCell.Style.Font.Bold = true;
                    titleNplCell.Style.Font.Size = 14;
                    titleNplCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    titleNplCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    int nplHeaderRow = nplStartRow + 2;
                    string[] nplHeaders = { "STT", "MÃ NPL", "LOẠI VẬT TƯ", "", "MÔ TẢ VẬT TƯ",
                           "MÀU", "SIZE/KHỔ", "ĐVT", "Số Lượng", "DMSX", "HẠO HỤT", "CẤP PHÁT", "GHI CHÚ PKT", "GHI CHÚ PKH" };

                    for (int i = 0; i < nplHeaders.Length; i++)
                    {
                        var cell = worksheet.Cells[nplHeaderRow, i + 1];
                        cell.Value = nplHeaders[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.Size = 11;
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        cell.Style.WrapText = true;
                        ApplyBorder(cell);
                    }

                    int nplRow = nplHeaderRow + 1;
                    int sttCounter = 1;

                    foreach (DataRow item in dtTable2.Rows)
                    {
                        worksheet.Cells[nplRow, 1].Value = sttCounter++;
                        worksheet.Cells[nplRow, 2].Value = item["MaVt"]?.ToString() ?? "";
                        worksheet.Cells[nplRow, 3].Value = item["TenNhom"]?.ToString() ?? "";
                        worksheet.Cells[nplRow, 4].Value = item["TenNhom"]?.ToString() ?? "";
                        worksheet.Cells[nplRow, 5].Value = item["TenVT"]?.ToString() ?? "";
                        worksheet.Cells[nplRow, 6].Value = item["MaMau"]?.ToString() ?? "";
                        worksheet.Cells[nplRow, 7].Value = item["KhoVai"]?.ToString() ?? "";
                        worksheet.Cells[nplRow, 8].Value = item["MaDV"]?.ToString() ?? "";
                        worksheet.Cells[nplRow, 9].Value = item["SoLuong"] != DBNull.Value ? item["SoLuong"] : 0;
                        worksheet.Cells[nplRow, 10].Value = item["DinhMuc"] != DBNull.Value ? item["DinhMuc"] : 0;
                        worksheet.Cells[nplRow, 11].Value = item["DinhMucHaoHut"] != DBNull.Value ? item["DinhMucHaoHut"] : 0;
                        worksheet.Cells[nplRow, 12].Value = item["CapPhat"] != DBNull.Value ? item["CapPhat"] : 0;
                        worksheet.Cells[nplRow, 13].Value = item["GhiChuPKT"]?.ToString() ?? "";
                        worksheet.Cells[nplRow, 14].Value = item["GhiChu"]?.ToString() ?? "";

                        for (int col = 1; col <= 14; col++)
                        {
                            var cell = worksheet.Cells[nplRow, col];
                            ApplyBorder(cell);
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }

                        nplRow++;
                    }

                    // XUẤT FILE
                    byte[] fileBytes = package.GetAsByteArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"XuatHangNL_{datetime}.xlsx";

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
                Console.WriteLine("Lỗi xảy ra: " + ex.Message);
                Console.WriteLine("StackTrace: " + ex.StackTrace);
                throw;
            }
        }

        private void ApplyBorder(ExcelRange cell)
        {
            cell.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        [HttpPost]
        [Route("PostBarCode")]
        public string PostBarCode(string action, dynamic data)
        {
            string json = JsonConvert.SerializeObject(data);
            var listData = JsonConvert.DeserializeObject<List<ERP_SoanHangNPL_BarCode_TYPE>>(json);
            string jsonL = JsonConvert.SerializeObject(listData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonL);

            return new PhieuXuatHangNPLModel().PostBarCode(action, tbl);
        }

        [HttpGet]
        [Route("ExportFileGhiNhanKiemKeNL")]
        public HttpResponseMessage ExportFileGhiNhanKiemKeNL(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "")
        {
            try
            {
                DataTable dt = new PhieuXuatHangNPLModel().GetTH("GetTongQuanKiemKeV2", para1, para2, para3, para4, para5, para6, para7, para8);
                string path = HttpContext.Current.Server.MapPath("~/Content/Templates/Template_GhiNhanKiemKeNL_PL.xlsx");
                FileInfo file = new FileInfo(path);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    var ws = package.Workbook.Worksheets["KiemKe"];
                    if (ws == null) throw new Exception("Không tìm thấy Sheet");
                    string loaiNguyenLieu = para4 == "1" ? "NGUYÊN LIỆU" : "PHỤ LIỆU";

                    var titleCell = ws.Cells["B1"];
                    titleCell.Value = titleCell.Value?.ToString()
                        .Replace("{0}", loaiNguyenLieu.ToUpper());
                    // ===== TÍNH TỔNG =====
                    decimal totalSLNK = Convert.ToDecimal(dt.Compute("SUM(SLNK)", ""));
                    decimal totalChenhLechMet = Convert.ToDecimal(dt.Compute("SUM(ChenhLechMet)", ""));
                    decimal totalChenhLechTT = Convert.ToDecimal(dt.Compute("SUM(ChenhLechTT)", ""));
                    int totalVatTu = dt.Rows.Count;

                    decimal totalSLKiemKe = 0;
                    foreach (DataRow item in dt.Rows)
                    {
                        totalSLKiemKe += item["SLKiemKeEdit"] != DBNull.Value && Convert.ToDecimal(item["SLKiemKeEdit"]) != 0
                            ? Convert.ToDecimal(item["SLKiemKeEdit"])
                            : (item["SLKiemKe"] != DBNull.Value ? Convert.ToDecimal(item["SLKiemKe"]) : 0);
                    }

                    // ===== DATA TABLE =====
                    int startRow = 6;
                    int row = startRow;

                    foreach (DataRow item in dt.Rows)
                    {
                        var slKiemKe = item["SLKiemKeEdit"] != DBNull.Value && Convert.ToDecimal(item["SLKiemKeEdit"]) != 0
                            ? item["SLKiemKeEdit"]
                            : item["SLKiemKe"];

                        ws.Cells[row, 1].Value = item["PhieuVatTuKK"];
                        ws.Cells[row, 2].Value = item["MaVT"];
                        ws.Cells[row, 3].Value = item["ChiTiet"];

                        var status = Convert.ToInt32(item["IsLog"]) == 0 ? "Hoàn thành" : "Đang kiểm kê";
                        ws.Cells[row, 4].Value = status;
                        ws.Cells[row, 4].Style.Font.Color.SetColor(
                            status == "Đang kiểm kê" ? Color.Goldenrod : Color.Green
                        );

                        ws.Cells[row, 5].Value = item["MauVT"];
                        ws.Cells[row, 6].Value = item["KhoVai"];
                        ws.Cells[row, 7].Value = item["SLNK"] != DBNull.Value ? Convert.ToDecimal(item["SLNK"]) : 0;
                        ws.Cells[row, 8].Value = slKiemKe != DBNull.Value ? Convert.ToDecimal(slKiemKe) : 0;
                        ws.Cells[row, 9].Value = item["ChenhLechMet"] != DBNull.Value ? Convert.ToDecimal(item["ChenhLechMet"]) : 0;
                        ws.Cells[row, 10].Value = item["ChenhLechTT"] != DBNull.Value ? Convert.ToDecimal(item["ChenhLechTT"]) : 0;

                        row++;
                    }

                    int lastDataRow = row - 1;
                    int totalRow = row;

                    // ===== DÒNG TỔNG =====
                    ws.Cells[totalRow, 1].Value = "TỔNG";
                    ws.Cells[totalRow, 2].Value = $"{totalVatTu} Vật tư";
                    ws.Cells[totalRow, 7].Value = totalSLNK;
                    ws.Cells[totalRow, 8].Value = totalSLKiemKe;
                    ws.Cells[totalRow, 9].Value = totalChenhLechMet;
                    ws.Cells[totalRow, 10].Value = totalChenhLechTT;

                    // ===== BORDER toàn bộ table =====
                    int lastRow = totalRow;
                    using (var table = ws.Cells[startRow, 1, lastRow, 10])
                    {
                        var border = table.Style.Border;
                        border.Top.Style = ExcelBorderStyle.Thin;
                        border.Bottom.Style = ExcelBorderStyle.Thin;
                        border.Left.Style = ExcelBorderStyle.Thin;
                        border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    ws.Cells[startRow, 1, lastDataRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[startRow, 1, lastDataRow, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[startRow, 3, lastDataRow, 3].Style.WrapText = true;
                    // tự động tăng chiều cao theo nội dung
                    foreach (var r in Enumerable.Range(startRow, lastDataRow - startRow + 1))
                    {
                        ws.Row(r).CustomHeight = false;
                    }
                    // ===== STYLE dòng tổng =====
                    var footerRange = ws.Cells[totalRow, 1, totalRow, 10];
                    footerRange.Style.Font.Bold = true;
                    footerRange.Style.Font.Color.SetColor(Color.Red);
                    footerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    footerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    footerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    footerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    byte[] bytes = package.GetAsByteArray();

                    var result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new ByteArrayContent(bytes);
                    var fileName = para4 == "1" ? "NguyenLieu" : "PhuLieu";
                    var fullFileName = $"GhiNhanKiemKe_{fileName}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    result.Content.Headers.ContentDisposition =
                        new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                        {
                            FileName = fullFileName,
                            FileNameStar = fullFileName
                        };
                    result.Content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra: " + ex.Message);
                Console.WriteLine("StackTrace: " + ex.StackTrace);
                throw;
            }
        }

        [HttpGet]
        [Route("ExportFileXuatPhieuGhiNhanKiemKeNL")]
        public HttpResponseMessage ExportFileXuatPhieuGhiNhanKiemKeNL(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "")
        {
            try
            {
                DataTable dt = new PhieuXuatHangNPLModel().GetTH("GetTongQuanKiemKeXuatPhieuV2", para1, para2, para3, para4, para5, para6, para7, para8);
                string path = HttpContext.Current.Server.MapPath("~/Content/Templates/Template_GhiNhanKiemKeNL_PL.xlsx");
                FileInfo file = new FileInfo(path);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    var templateWorksheet = package.Workbook.Worksheets[0];
                    HashSet<string> createdWorksheets = new HashSet<string>();
                    ExcelWorksheet ws2;
                    foreach (DataRow dr in dt.Rows)
                    {
                        string phieuVTKK = dr["PhieuVatTuKK"].ToString();
                        if (!createdWorksheets.Contains(phieuVTKK))
                        {
                            ws2 = package.Workbook.Worksheets.Add(phieuVTKK, templateWorksheet);
                            createdWorksheets.Add(phieuVTKK);
                        }
                        else
                        {
                            ws2 = package.Workbook.Worksheets[phieuVTKK];
                            continue;
                        }

                        string loaiNguyenLieuXP = para4 == "1" ? "NGUYÊN LIỆU" : "PHỤ LIỆU";

                        var titleCellXP = ws2.Cells["B1"];
                        titleCellXP.Value = titleCellXP.Value?.ToString()
                            .Replace("{0}", loaiNguyenLieuXP.ToUpper());

                        var filteredRows = dt.Select($"PhieuVatTuKK = '{phieuVTKK}'");

                        // ===== TÍNH TỔNG (chỉ dòng của sheet này) =====
                        decimal totalSLNKXP = filteredRows
                            .Sum(r => r["SLNK"] != DBNull.Value ? Convert.ToDecimal(r["SLNK"]) : 0);
                        decimal totalChenhLechMetXP = filteredRows
                            .Sum(r => r["ChenhLechMet"] != DBNull.Value ? Convert.ToDecimal(r["ChenhLechMet"]) : 0);
                        decimal totalChenhLechTTXP = filteredRows
                            .Sum(r => r["ChenhLechTT"] != DBNull.Value ? Convert.ToDecimal(r["ChenhLechTT"]) : 0);
                        int totalVatTuXP = filteredRows.Length;

                        decimal totalSLKiemKeXP = filteredRows.Sum(item =>
                            item["SLKiemKeEdit"] != DBNull.Value && Convert.ToDecimal(item["SLKiemKeEdit"]) != 0
                                ? Convert.ToDecimal(item["SLKiemKeEdit"])
                                : (item["SLKiemKe"] != DBNull.Value ? Convert.ToDecimal(item["SLKiemKe"]) : 0));

                        // ===== DATA TABLE =====
                        int startRowXP = 6;
                        int rowXP = startRowXP;

                        foreach (DataRow item in filteredRows)
                        {
                            var slKiemKe = item["SLKiemKeEdit"] != DBNull.Value && Convert.ToDecimal(item["SLKiemKeEdit"]) != 0
                               ? item["SLKiemKeEdit"]
                               : item["SLKiemKe"];

                            ws2.Cells[rowXP, 1].Value = item["PhieuVatTuKK"];
                            ws2.Cells[rowXP, 2].Value = item["MaVT"];
                            ws2.Cells[rowXP, 3].Value = item["ChiTiet"];

                            var status = Convert.ToInt32(item["IsLog"]) == 0 ? "Hoàn thành" : "Đang kiểm kê";
                            ws2.Cells[rowXP, 4].Value = status;
                            ws2.Cells[rowXP, 4].Style.Font.Color.SetColor(
                                status == "Đang kiểm kê" ? Color.Goldenrod : Color.Green
                            );

                            ws2.Cells[rowXP, 5].Value = item["MauVT"];
                            ws2.Cells[rowXP, 6].Value = item["KhoVai"];
                            ws2.Cells[rowXP, 7].Value = item["SLNK"] != DBNull.Value ? Convert.ToDecimal(item["SLNK"]) : 0;
                            ws2.Cells[rowXP, 8].Value = slKiemKe != DBNull.Value ? Convert.ToDecimal(slKiemKe) : 0;
                            ws2.Cells[rowXP, 9].Value = item["ChenhLechMet"] != DBNull.Value ? Convert.ToDecimal(item["ChenhLechMet"]) : 0;
                            ws2.Cells[rowXP, 10].Value = item["ChenhLechTT"] != DBNull.Value ? Convert.ToDecimal(item["ChenhLechTT"]) : 0;

                            rowXP++;
                        }

                        int lastDataRowXP = rowXP - 1;
                        int totalRowXP = rowXP;

                        // ===== DÒNG TỔNG =====
                        ws2.Cells[totalRowXP, 1].Value = "TỔNG";
                        ws2.Cells[totalRowXP, 2].Value = $"{totalVatTuXP} Vật tư";
                        ws2.Cells[totalRowXP, 7].Value = totalSLNKXP;
                        ws2.Cells[totalRowXP, 8].Value = totalSLKiemKeXP;
                        ws2.Cells[totalRowXP, 9].Value = totalChenhLechMetXP;
                        ws2.Cells[totalRowXP, 10].Value = totalChenhLechTTXP;

                        // ===== BORDER toàn bộ table =====
                        int lastRowXP = totalRowXP;
                        using (var table = ws2.Cells[startRowXP, 1, lastRowXP, 10])
                        {
                            var border = table.Style.Border;
                            border.Top.Style = ExcelBorderStyle.Thin;
                            border.Bottom.Style = ExcelBorderStyle.Thin;
                            border.Left.Style = ExcelBorderStyle.Thin;
                            border.Right.Style = ExcelBorderStyle.Thin;
                        }

                        ws2.Cells[startRowXP, 1, lastDataRowXP, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws2.Cells[startRowXP, 1, lastDataRowXP, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws2.Cells[startRowXP, 3, lastDataRowXP, 3].Style.WrapText = true;
                        ws2.Cells[startRowXP, 1, lastRowXP, 10].Style.Font.Size = 11;
                        // tự động tăng chiều cao theo nội dung
                        foreach (var r in Enumerable.Range(startRowXP, lastDataRowXP - startRowXP + 1))
                        {
                            ws2.Row(r).CustomHeight = false;
                        }
                        // ===== STYLE dòng tổng =====
                        var footerRangeXP = ws2.Cells[totalRowXP, 1, totalRowXP, 10];
                        footerRangeXP.Style.Font.Bold = true;
                        footerRangeXP.Style.Font.Color.SetColor(Color.Red);
                        footerRangeXP.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        footerRangeXP.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        footerRangeXP.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        footerRangeXP.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    }

                    var ws = package.Workbook.Worksheets["KiemKe"];
                    if (ws == null) throw new Exception("Không tìm thấy Sheet");
                    string loaiNguyenLieu = para4 == "1" ? "NGUYÊN LIỆU" : "PHỤ LIỆU";

                    var titleCell = ws.Cells["B1"];
                    titleCell.Value = titleCell.Value?.ToString()
                        .Replace("{0}", loaiNguyenLieu.ToUpper());
                    // ===== TÍNH TỔNG =====
                    decimal totalSLNK = Convert.ToDecimal(dt.Compute("SUM(SLNK)", ""));
                    decimal totalChenhLechMet = Convert.ToDecimal(dt.Compute("SUM(ChenhLechMet)", ""));
                    decimal totalChenhLechTT = Convert.ToDecimal(dt.Compute("SUM(ChenhLechTT)", ""));
                    int totalVatTu = dt.Rows.Count;

                    decimal totalSLKiemKe = 0;
                    foreach (DataRow item in dt.Rows)
                    {
                        totalSLKiemKe += item["SLKiemKeEdit"] != DBNull.Value && Convert.ToDecimal(item["SLKiemKeEdit"]) != 0
                            ? Convert.ToDecimal(item["SLKiemKeEdit"])
                            : (item["SLKiemKe"] != DBNull.Value ? Convert.ToDecimal(item["SLKiemKe"]) : 0);
                    }

                    // ===== DATA TABLE =====
                    int startRow = 6;
                    int row = startRow;

                    foreach (DataRow item in dt.Rows)
                    {
                        var slKiemKe = item["SLKiemKeEdit"] != DBNull.Value && Convert.ToDecimal(item["SLKiemKeEdit"]) != 0
                            ? item["SLKiemKeEdit"]
                            : item["SLKiemKe"];

                        ws.Cells[row, 1].Value = item["PhieuVatTuKK"];
                        ws.Cells[row, 2].Value = item["MaVT"];
                        ws.Cells[row, 3].Value = item["ChiTiet"];

                        var status = Convert.ToInt32(item["IsLog"]) == 0 ? "Hoàn thành" : "Đang kiểm kê";
                        ws.Cells[row, 4].Value = status;
                        ws.Cells[row, 4].Style.Font.Color.SetColor(
                            status == "Đang kiểm kê" ? Color.Goldenrod : Color.Green
                        );

                        ws.Cells[row, 5].Value = item["MauVT"];
                        ws.Cells[row, 6].Value = item["KhoVai"].ToString(); ;
                        ws.Cells[row, 7].Value = item["SLNK"] != DBNull.Value ? Convert.ToDecimal(item["SLNK"]) : 0;
                        ws.Cells[row, 8].Value = slKiemKe != DBNull.Value ? Convert.ToDecimal(slKiemKe) : 0;
                        ws.Cells[row, 9].Value = item["ChenhLechMet"] != DBNull.Value ? Convert.ToDecimal(item["ChenhLechMet"]) : 0;
                        ws.Cells[row, 10].Value = item["ChenhLechTT"] != DBNull.Value ? Convert.ToDecimal(item["ChenhLechTT"]) : 0;

                        row++;
                    }

                    int lastDataRow = row - 1;
                    int totalRow = row;

                    // ===== DÒNG TỔNG =====
                    ws.Cells[totalRow, 1].Value = "TỔNG";
                    ws.Cells[totalRow, 2].Value = $"{totalVatTu} Vật tư";
                    ws.Cells[totalRow, 7].Value = totalSLNK;
                    ws.Cells[totalRow, 8].Value = totalSLKiemKe;
                    ws.Cells[totalRow, 9].Value = totalChenhLechMet;
                    ws.Cells[totalRow, 10].Value = totalChenhLechTT;

                    // ===== BORDER toàn bộ table =====
                    int lastRow = totalRow;
                    using (var table = ws.Cells[startRow, 1, lastRow, 10])
                    {
                        var border = table.Style.Border;
                        border.Top.Style = ExcelBorderStyle.Thin;
                        border.Bottom.Style = ExcelBorderStyle.Thin;
                        border.Left.Style = ExcelBorderStyle.Thin;
                        border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    ws.Cells[startRow, 1, lastDataRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[startRow, 1, lastDataRow, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[startRow, 3, lastDataRow, 3].Style.WrapText = true;
                    ws.Cells[startRow, 1, lastRow, 10].Style.Font.Size = 11;

                    // tự động tăng chiều cao theo nội dung
                    foreach (var r in Enumerable.Range(startRow, lastDataRow - startRow + 1))
                    {
                        ws.Row(r).CustomHeight = false;
                    }
                    // ===== STYLE dòng tổng =====
                    var footerRange = ws.Cells[totalRow, 1, totalRow, 10];
                    footerRange.Style.Font.Bold = true;
                    footerRange.Style.Font.Color.SetColor(Color.Red);
                    footerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    footerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    footerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    footerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    byte[] bytes = package.GetAsByteArray();

                    var result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new ByteArrayContent(bytes);
                    var fileName = para4 == "1" ? "NguyenLieu" : "PhuLieu";
                    var fullFileName = $"GhiNhanKiemKe_{fileName}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    result.Content.Headers.ContentDisposition =
                        new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                        {
                            FileName = fullFileName,
                            FileNameStar = fullFileName
                        };
                    result.Content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra: " + ex.Message);
                Console.WriteLine("StackTrace: " + ex.StackTrace);
                throw;
            }
        }


        [HttpPost]
        [Route("PostGiaoViec")]
        public string PostGiaoViec(string action, dynamic data)
        {
            string json = JsonConvert.SerializeObject(data);
            var listData = JsonConvert.DeserializeObject<List<ERP_KeHoachGiaoViecNL>>(json);
            string jsonL = JsonConvert.SerializeObject(listData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonL);

            return new PhieuXuatHangNPLModel().PostGiaoViec(action, null, tbl);
        }

        [HttpPost]
        [Route("PostHT")]
        public string PostHT(string action, dynamic data)
        {
            string json = JsonConvert.SerializeObject(data);
            var listData = JsonConvert.DeserializeObject<List<ERP_ChiTietMoKien_Dot_Type>>(json);
            string jsonL = JsonConvert.SerializeObject(listData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonL);

            return new PhieuXuatHangNPLModel().PostHT(action, tbl);
        }

    }
    public class GroupedRow
    {
        public string ColorCode { get; set; }
        public string Color { get; set; }
        public string PO { get; set; }
        public string SizeInseam { get; set; }
        public string Country { get; set; }
        public string NgayGH { get; set; }
        public Dictionary<string, int> Sizes { get; set; }
    }
    public class PhieuXuatHangNguyenPhuLIeu
    {
        public string SoLoID { get; set; }
        public string SoLo { get; set; }
        public string PhieuYC { get; set; }
        public string MaLenh { get; set; }
        public string MaLenhSX { get; set; }
        public string MaGop { get; set; }
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string MaNPL { get; set; }
        public string MaVTID { get; set; }
        public string MauVTID { get; set; }
        public string CayVai { get; set; }
        public string SoLot { get; set; }
        public string KhoVai { get; set; }
        public string KhoVaiID { get; set; }
        public string DonVi { get; set; }
        public string MaDonVi { get; set; }
        public string SoKien { get; set; }
        public string KienGoc { get; set; }
        public double? SLGoc { get; set; }
        public double? SLNhap { get; set; }
        public bool? isCheck { get; set; }
        public string GhiChu { get; set; }
        public string BarCodeGoc { get; set; }
        public string BarCode { get; set; }
        public DateTime? NgayXuatHang { get; set; }
        public string NguoiXuatHang { get; set; }
        public int? Moudule { get; set; }
        public int? Dot { get; set; }
        public string Line { get; set; }
    }
    public class PhieuXuatHangKT
    {
        public string PhieuXH { get; set; }
        public int SortXH { get; set; }
        public string BarCode { get; set; }
        public int? Dot { get; set; }
        public int? IsNPL { get; set; }
        public string KyTen { get; set; }
        public string PhieuYC { get; set; }
        public DateTime? NgayXH { get; set; }
        public string UserXH { get; set; }
    }
    public class PhieuThuHoiModel
    {
        public string MaPhieu { get; set; }
        public string MaNPL { get; set; }
        public string MaVTID { get; set; }
        public string MauVTID { get; set; }
        public string MauVT { get; set; }
        public string MaVT { get; set; }
        public string KhoVaiID { get; set; }
        public string KhoVai { get; set; }
        public double SLTH { get; set; }
        public string MaHang { get; set; }
        public string MaLenh { get; set; }
        public string TenKH { get; set; }
        public DateTime? NgayTH { get; set; }
        public string NguoiTH { get; set; }
        public DateTime? NgayCN { get; set; }
        public string NguoiCN { get; set; }
        public int Dot { get; set; }
        public int Sort { get; set; }
        public string Palet { get; set; }
        public int Module { get; set; }
        public string MaLenhSX { get; set; }
        public string MaGop { get; set; }
        public string PhieuYC { get; set; }
        public string SoLoID { get; set; }
        public int Status { get; set; }
        public string GhiChu { get; set; }
    }
    public class PhieuXuatHangPL
    {
        public string PhieuXHPL { get; set; }
        public string PhieuYC { get; set; }
        public string MaLenh { get; set; }
        public string MaGop { get; set; }
        public string MaLenhSX { get; set; }
        public string MaNPL { get; set; }
        public int? SLNhap { get; set; }
        public int? Module { get; set; }
        public int? Sort { get; set; }
        public string Sign { get; set; }
        public string NgayXuatHang { get; set; }
        public int? Dot { get; set; }
        public string SoLoID { get; set; }
    }

    public class PhieuKiemKeNPL
    {
        public string SoLoID { get; set; }
        public string SoLo { get; set; }
        public string MaNPL { get; set; }
        public string MaVTID { get; set; }
        public string MauVTID { get; set; }
        public int? IsNPL { get; set; }
        public string BarCode { get; set; }
        public double? SLKiemKe { get; set; }
        public double? SLKiemKeEdit { get; set; }
        public string GhiChu { get; set; }
        public string UserKK { get; set; }
        public DateTime? DateKiemKKe { get; set; }
        public int? IsXacNhan { get; set; }
        public string PhieuKiemKe { get; set; }
    }
    public class ERP_PhieuSoanHangVatTu_Type
    {
        public string PhieuSH { get; set; }
        public int? Dot { get; set; }
        public string MaLenh { get; set; }
        public string MaLenhSX { get; set; }
        public string MaNPL { get; set; }
        public string MaVT { get; set; }
        public string MauVT { get; set; }
        public string KhoSize { get; set; }
        public string DonVi { get; set; }
        public int? IsNPL { get; set; }
        public double? SLCapPhat { get; set; }
        public double? SLSoanHang { get; set; }
        public string NguoiSoanHang { get; set; }
        public DateTime? NgayDangKy { get; set; }
        public DateTime? NgaySoanHang { get; set; }
        public int? IsXN { get; set; }
        public string KyTen { get; set; }
        public DateTime? NgayXacNhan { get; set; }
        public string GhiChu { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string BarCode { get; set; }
        public string SoKienHienThi { get; set; }
    }


    public class ERP_SoanHangNPL_BarCode_TYPE
    {
        public string BarCode { get; set; }
        public string GhiChu { get; set; }
        public string PhieuSH { get; set; }
        public float SLSoanHang_BC { get; set; }
        public float SLSoanHang_TK { get; set; }
        public string MaNPL { get; set; }
    }


    public class ERP_KeHoachGiaoViecNL
    {
        public string PhieuGiaoViec { get; set; }
        public int? Dot { get; set; }
        public string MaLenhSX { get; set; }
        public string MaNPL { get; set; }
        public double SLCanXuat { get; set; }
        public string NguoiGiaoViec { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayGiaoViec { get; set; }
        public DateTime NgayXuatHang { get; set; }
        public string GhiChu { get; set; }
    }

    public class ERP_ChiTietMoKien_Dot_Type
    {
        public string SoLoID { get; set; }
        public string MaNPL { get; set; }
        public float SoGhiDauCay { get; set; }
        public string SoKienHienThi { get; set; }
        public string BarCode { get; set; }
        public int? isNK { get; set; }
        public int? IsNPL { get; set; }
        public int? Dot { get; set; }
        public DateTime? NgayNhapKho { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiTao { get; set; }
    }
    public class ChiTietNhapKhoNPL
    {
        public int ID { get; set; }

        public string SoLoID { get; set; }

        public string MaNPL { get; set; }

        public string MaVTID { get; set; }

        public string MaMauVT { get; set; }

        public string SoKien { get; set; }

        public string SoLoT { get; set; }

        public string MaHaiQuan { get; set; }

        public string MaKeToan { get; set; }

        public float SoGhiDauCay { get; set; }

        public float NW { get; set; }

        public float GW { get; set; }

        public string BarCode { get; set; }

        public string GhiChu { get; set; }

        public int IsNPL { get; set; }

        public string KhoVaiID { get; set; }

        public string SoKienParent { get; set; }

        public float SoLuongThucTe { get; set; }

        public decimal DonGia { get; set; }

        public decimal ThanhTien { get; set; }

        public DateTime? Pallet { get; set; }

        public string MaDVVT { get; set; }

        public string MauVTID { get; set; }
    }
}

