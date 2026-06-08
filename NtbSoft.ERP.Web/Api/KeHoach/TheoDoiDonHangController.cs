using Newtonsoft.Json;
using NtbSoft.ERP.Entity.KeHoach;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Model.KeHoach;
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

namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [RoutePrefix("api/TheoDoiDonHang")]
    public class TheoDoiDonHangController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1, string Para2, string Para3)
        {
            return new TheoDoiDonHangModel().Get(action, Para1, Para2, Para3);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable dt)
        {
            return new TheoDoiDonHangModel().Post(dt);
        }
        [HttpPost]
        [Route("PostImage")]
        public string PostImage(dynamic item)
        {
            List<ImageOtherEntity> lstData = new List<ImageOtherEntity>();
            foreach (var dt in item)
            {
                string imageDataA = string.Empty;
                imageDataA = dt.Image.ToString();
                string imageName = "";
                int currentMinute = DateTime.Now.Minute;
                int currentSecond = DateTime.Now.Second;
                string[] imageDataParts = imageDataA.Split(',');
                if (!string.IsNullOrEmpty(imageDataA))
                {
                    byte[] bytes = Convert.FromBase64String(imageDataParts[1]);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        string tenhinh = dt.MaBienBan + "_" + dt.Position + "_" + currentSecond + currentMinute + currentSecond;
                        Image image = Image.FromStream(ms);
                        string uploadPath = HttpContext.Current.Server.MapPath("~/Images/Other");
                        if (!System.IO.Directory.Exists(uploadPath))
                            System.IO.Directory.CreateDirectory(uploadPath);
                        imageName = tenhinh + ".png";
                        var mPath = string.Format(@"{0}\{1}", uploadPath, imageName);

                        image.Save(mPath);
                    }
                }
                lstData.Add(new ImageOtherEntity()
                {
                    MaBienBan = (int)dt.MaBienBan,
                    NoiDung = dt.NoiDung.ToString(),
                    Image = imageName,
                    Position = (int)dt.Position,
                });
            }
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new TheoDoiDonHangModel().PostImage(tbl);
        }
        [HttpPost]
        [Route("PostImageNoiBo")]
        public string PostImageNoiBo(dynamic data)
        {
            List<ImageNoiBoEntity> lstData = new List<ImageNoiBoEntity>();
            foreach (var item in data)
            {
                string imageDataA = string.Empty;
                imageDataA = item.Image.ToString();
                string imageName = "";
                int currentMinute = DateTime.Now.Minute;
                int currentSecond = DateTime.Now.Second;
                string[] imageDataParts = imageDataA.Split(',');
                if (!string.IsNullOrEmpty(imageDataA))
                {
                    byte[] bytes = Convert.FromBase64String(imageDataParts[1]);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        string tenhinh = item.MaPKL + "_" + item.Position + "_" + currentSecond + currentMinute + currentSecond;
                        Image image = Image.FromStream(ms);
                        string uploadPath = HttpContext.Current.Server.MapPath("~/Images/ImageNoiBo");
                        if (!System.IO.Directory.Exists(uploadPath))
                            System.IO.Directory.CreateDirectory(uploadPath);
                        imageName = tenhinh + ".png";
                        var mPath = string.Format(@"{0}\{1}", uploadPath, imageName);

                        image.Save(mPath);
                    }
                }
                lstData.Add(new ImageNoiBoEntity()
                {
                    MaPKL = item.MaPKL.ToString(),
                    NoiDung = item.NoiDung.ToString(),
                    Image = imageName,
                    Position = (int)item.Position,
                    Cont = item.Cont.ToString(),
                });
            }
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new TheoDoiDonHangModel().PostNoiBo(tbl, "PostImageNoiBo", "@TypeImageNoiBo");
        }
        [HttpPost]
        [Route("PostDL")]
        public string PostDL(dynamic data)
        {
            List<BienBanChuyenKho> lstData = new List<BienBanChuyenKho>();
            foreach (var item in data)
            {
                lstData.Add(new BienBanChuyenKho()
                {
                    MaPKL_XH = item.MaPKL_XH.ToString(),
                    Cont = item.Cont.ToString(),
                    MaTuKho = item.MaTuKho.ToString(),
                    MaDenKho = item.MaDenKho.ToString(),
                    TuKho = item.TuKho.ToString() ?? "",
                });
            }

            var json = JsonConvert.SerializeObject(lstData);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            return new TheoDoiDonHangModel().PostNoiBo(dt, "SaveGhiChuChuyenKho", "@TypeBBChuyenKho");
        }
        [HttpPost]
        [Route("PostKyTenNoiBo")]
        public string PostKyTenNoiBo(dynamic data)
        {
            List<KiTenNoiBoEntity> lstData = new List<KiTenNoiBoEntity>();
            foreach (var item in data)
            {
                string imageDataA = string.Empty;
                imageDataA = item.ChuKy;
                string imageName = "";
                int currentMinute = DateTime.Now.Minute;
                int currentSecond = DateTime.Now.Second;
                if(imageDataA != null)
                {
                    string[] imageDataParts = imageDataA.Split(',');
                    if (!string.IsNullOrEmpty(imageDataA))
                    {
                        byte[] bytes = Convert.FromBase64String(imageDataParts[1]);
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            string tenhinh = item.MaPKL + "_" + item.Position + "_" + currentSecond + currentMinute + currentSecond;
                            Image image = Image.FromStream(ms);
                            string uploadPath = HttpContext.Current.Server.MapPath("~/Images/KyTenNoiBo");
                            if (!System.IO.Directory.Exists(uploadPath))
                                System.IO.Directory.CreateDirectory(uploadPath);
                            imageName = tenhinh + ".png";
                            var mPath = string.Format(@"{0}\{1}", uploadPath, imageName);

                            image.Save(mPath);
                        }
                    }
                }
              
                lstData.Add(new KiTenNoiBoEntity()
                {
                    MaPKL = item.MaPKL.ToString(),
                    MaHoTen = item.MaHoTen.ToString(),
                    HoTen = item.HoTen.ToString(),
                    BoPhan = item.BoPhan.ToString(),
                    ChuKy = imageName,
                    Cont = item.Cont.ToString(),
                    Position = (int)item.Position,
                });
            }
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new TheoDoiDonHangModel().PostNoiBo(tbl, "PostKyTenNoiBo", "@TypeKyTenNoiBo");
        }
        [HttpPost]
        [Route("UpdateChuyenKho")]
        public string UpdateChuyenKho(dynamic data)
        {
            List<PackageListChuyenKhoEntity> lstData = new List<PackageListChuyenKhoEntity>();
            lstData.Add(new PackageListChuyenKhoEntity()
            {
                MaPKL_XH = data.MaPKL_XH.ToString(),
                VCCont = data.VCCont.ToString(),
                VCXeTai = data.VCXeTai.ToString(),
                Romoc = data.Romoc.ToString(),
                Cont = data.Cont.ToString(),
                TenTaiXe = data.TenTaiXe.ToString(),
                CMND = data.CMND.ToString(),
                Seal = data.MaSeal.ToString(),
               
                FinishDate = DateTime.Parse(data.FinishDate.ToString()),
                NVienXuat = data.NVienXuat.ToString(),
            });

            if (lstData.Count == 0) return null;
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new TheoDoiDonHangModel().PostNoiBo(tbl, "UpdateChuyenKho", "@TypeChuyenKho");
        }
        [HttpPost]
        [Route("UpdateInputImage")]
        public string UpdateInputImage(dynamic data)
        {
            List<ImageNoiBoEntity> lstData = new List<ImageNoiBoEntity>();
            foreach (var item in data)
            {
                lstData.Add(new ImageNoiBoEntity()
                {
                    MaPKL = item.MaPKL.ToString(),
                    NoiDung = item.NoiDung.ToString(),
                    Position = (int)item.Position,
                    Cont = item.Cont.ToString(),
                });
            }
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new TheoDoiDonHangModel().PostNoiBo(tbl, "UpdateInputImage", "@TypeImageNoiBo");
        }
        [HttpPost]
        [Route("UpdateInputImageOther")]
        public string UpdateInputImageOther(dynamic data)
        {
            List<ImageOtherEntity> lstData = new List<ImageOtherEntity>();
            foreach (var dt in data)
            {
                lstData.Add(new ImageOtherEntity()
                {
                    MaBienBan = (int)dt.MaBienBan,
                    NoiDung = dt.NoiDung.ToString(),
                    Position = (int)dt.Position,
                });

            }
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new TheoDoiDonHangModel().PostNoiBo(tbl, "UpdateInputImageOther", "@TypeImage");
        }


        [HttpPost]
        [Route("PostImageKyTen")]
        public string PostImageKyTen(dynamic items)
        {
            if (items is null) return null;
            List<KyTenBienBan> lstData = new List<KyTenBienBan>();
            int index = 1;
            foreach (var item in items)
            {

                string imageDataA = string.Empty;
                imageDataA = item.KyTen;
                string imageName = "";
                int currentMinute = DateTime.Now.Minute;
                int currentSecond = DateTime.Now.Second;
                if (imageDataA != null)
                {
                    string[] imageDataParts = imageDataA.Split(',');
                    if (!string.IsNullOrEmpty(imageDataA))
                    {
                        byte[] bytes = Convert.FromBase64String(imageDataParts[1]);
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            string tenhinh = item.MaBBXuatHang.ToString() + "_" + currentSecond + currentMinute + currentSecond + index;
                            Image image = Image.FromStream(ms);
                            string uploadPath = HttpContext.Current.Server.MapPath("~/Images/KytenXuatHang");
                            if (!System.IO.Directory.Exists(uploadPath))
                                System.IO.Directory.CreateDirectory(uploadPath);
                            imageName = tenhinh + ".png";
                            var mPath = string.Format(@"{0}\{1}", uploadPath, imageName);

                            image.Save(mPath);
                        }
                    }
                }
                lstData.Add(new KyTenBienBan()
                {
                    MaBBXuatHang = item.MaBBXuatHang.ToString(),
                    MaHoTen = item.MaHoTen.ToString(),
                    HoTen = item.HoTen.ToString(),
                    BoPhan = item.BoPhan.ToString(),
                    KyTen = imageName,
                });
                index++;

            }

            var json = JsonConvert.SerializeObject(lstData);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            return new TheoDoiDonHangModel().PostNoiBo(dt, "PostImageKyTen", "@TypeKyTenXuatHang");
        }

        [HttpPost]
        [Route("DeleteNoiBo")]
        public string DeleteNoiBo(dynamic data)
        {
            string mapkl = data.MaPKL.ToString();
            string cont = data.Cont.ToString();
            return new TheoDoiDonHangModel().Delete("DeleteNoiBo", mapkl, cont);
        }
        [HttpPost]
        [Route("DeleteImage")]
        public string DeleteImage(dynamic data)
        {
            return new TheoDoiDonHangModel().DeleteImage("DeleteImageOther", (int)data.MaBienBan, (int)data.Position);
        }
        [HttpPost]
        [Route("DeleteImageOtherNoiBo")]
        public string DeleteImageOtherNoiBo(dynamic data)
        {
            return new TheoDoiDonHangModel().DeleteImageOtherNoiBo("DeleteImageOtherNoiBo", data.MaPKL.ToString(), (int)data.Position, data.Cont.ToString());
        }
        [HttpPost]
        [Route("UpdateInputKyTen")]
        public string UpdateInputKyTen(dynamic data)
        {
            return new TheoDoiDonHangModel().UpdateInputKyTen(data.Action.ToString(), data.Column.ToString(), data.Value.ToString(), data.MaHoTen.ToString());
        }
        [HttpPost]
        [Route("PostBienBanChuyenKho")]
        public string PostBienBanChuyenKho(dynamic data)
        {
            List<BienBanChuyenKho> lstData = new List<BienBanChuyenKho>();

            lstData.Add(new BienBanChuyenKho()
            {
                MaPKL_XH = data.MaPKL_XH.ToString(),
                TenTaiXe = data.TenTaiXe.ToString(),
                TuKho = data.TuKho.ToString(),
                DenKho = data.DenKho.ToString(),
                CMND = data.CMND.ToString(),
                SoXe = data.SoXe.ToString(),
                SoRoMoc =data.SoRoMoc.ToString(),
                NgayXeDI = Convert.ToDateTime(data.NgayXeDI.ToString()).ToString("yyyy-MM-dd hh:mm"),
                NgayXeDen = Convert.ToDateTime(data.NgayXeDen.ToString()).ToString("yyyy-MM-dd hh:mm"),
                SoSeal = Convert.ToDateTime(data.SoSeal.ToString()).ToString("yyyy-MM-dd hh:mm"),
                MaSeal =data.MaSeal.ToString(),
                Cont = data.Cont.ToString(),
                TenCont = data.TenCont.ToString(),
                TenBienBan = data.TenBienBan.ToString(),
                VCCont =(int)data.VCCont,
                VCRoMoc =(int)data.VCRoMoc,
                VCXetai =(int)data.VCXetai,

            });
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new TheoDoiDonHangModel().PostNoiBo(tbl, "SaveBBChuyenKho", "@TypeBBChuyenKho");
        }
        public static string FormatDateTimeAddNgay(DateTime datetime)
        {
            string day = datetime.Day.ToString("00");
            string month = datetime.Month.ToString("00");
            string year = datetime.Year.ToString();

            return $"Ngày {day} Tháng {month} Năm {year}";
        }
        [HttpGet]
        [Route("BCEXNoiBo")]
        public HttpResponseMessage BCEXKhacPhucCaiTien(string action, string para1, string para2, string para3)
        {
            try
            {
                DataTable dtTable = new TheoDoiDonHangModel().Get(action, para1, para2, para3);

                string imagePathRelative = "/Content/Templates/PhieuXuatKho.xlsx";
                string imagePathPhysical = HttpContext.Current.Server.MapPath("~" + imagePathRelative);

                FileInfo templateFile = new FileInfo(imagePathPhysical);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                using (ExcelPackage package = new ExcelPackage(templateFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                    ExcelRange range = worksheet.Cells;

                    string tukho = dtTable.Rows[0]["TuKho"].ToString();
                    string denKho = dtTable.Rows[0]["DenKho"].ToString();
                    string ngaythang = FormatDateTimeAddNgay( Convert.ToDateTime(para3));
                    worksheet.Cells["C4"].Value = ngaythang;
                    worksheet.Cells["i14"].Value = ngaythang;
                    worksheet.Cells["j6"].Value = denKho;
                    worksheet.Cells["j8"].Value = tukho;
                    int row = 11;
                    int index = 1;
                    foreach (DataRow item in dtTable.Rows)
                    {
                        worksheet.InsertRow(row, 1);
                        worksheet.Cells[row, 1].Value = index;
                        range = worksheet.Cells[row, 2, row, 4]; range.Merge = true; ; range.Value = item["Display"].ToString();
                        worksheet.Cells[row, 6].Value = "Kiện";
                        worksheet.Cells[row, 8].Value = Convert.ToInt32(item["Carton"]);
                        index++;
                        row++;
                    }
                    var borderData = worksheet.Cells[11, 1, row -1, 10].Style.Border;
                    borderData.Bottom.Style =
                        borderData.Top.Style =
                        borderData.Left.Style =
                        borderData.Right.Style = ExcelBorderStyle.Thin;
                    byte[] fileBytes = package.GetAsByteArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string fileName = $"PhieuXuatKho.xlsx";


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
        
    }
    public class BienBanChuyenKho
    {
        public string MaPKL_XH { get; set; }
        public string TenTaiXe { get; set; }
        public string MaTuKho { get; set; }
        public string TuKho { get; set; }
        public string MaDenKho { get; set; }
        public string DenKho { get; set; }
        public string CMND { get; set; }
        public string SoXe { get; set; }
        public string SoRoMoc { get; set; }
        public int VCCont { get; set; }
        public int VCRoMoc { get; set; }
        public int VCXetai { get; set; }
        public string NgayXeDI { get; set; }
        public string NgayXeDen { get; set; }
        public string SoSeal { get; set; }
        public string MaSeal { get; set; }
        public string Cont { get; set; }
        public string TenCont { get; set; }
        public string TenBienBan { get; set; }

    }
}