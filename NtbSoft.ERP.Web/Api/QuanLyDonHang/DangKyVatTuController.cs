using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using NtbSoft.ERP.Model.NguyenPhuLieu;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;


namespace NtbSoft.ERP.Web.Api.NguyenPhuLieu
{
    [RoutePrefix("api/DangkyVatTu")]
    public class DangKyVatTuController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1 = "", string Para2 = "", string Para3 = "", string Para4 = "", string Para5 = "", string Para6 = "")
        {
            return new DangKyVatTuModel().Get(action, Para1, Para2, Para3, Para4, Para5, Para6);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(string action, List<PhieuDangKyXuatVT> lstData, string Para1 = "")
        {
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            return new DangKyVatTuModel().Post(action, Para1, tbl);
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
                string uploadPath = HttpContext.Current.Server.MapPath("~/Images/SignDeNghiThuHoi");

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
        [Route("PostDNTH")]
        public string PostDNTH(string action, List<TYPE_ERP_PhieuDeNghiTHVT> lstData, string Para1 = "")
        {
            string imageDataA = lstData[0].SignNgDK;
            string imageDataB = lstData[0].SignTBPNgDK;
            string imageDataC = lstData[0].SignMer;
            string imageDataD = lstData[0].SignTBPMer;
            string datetime = DateTime.Now.ToString("ddMMyyyyHHss");
            string imageNameA = SaveSignatureImage(imageDataA, $"SignNgDK", "", datetime);
            string imageNameB = SaveSignatureImage(imageDataB, $"SignTBPNgDK", "", datetime);
            string imageNameC = SaveSignatureImage(imageDataC, $"SignMer", "", datetime);
            string imageNameD = SaveSignatureImage(imageDataD, $"SignTBPMer", "", datetime);



            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            foreach (DataRow item in tbl.Rows)
            {
                item["SignNgDK"] = imageNameA;
                item["SignTBPNgDK"] = imageNameB;
                item["SignMer"] = imageNameC;
                item["SignTBPMer"] = imageNameD;
            }

            return new DangKyVatTuModel().Post(action, Para1, tbl);
        }

        [HttpPost]
        [Route("PostXNKK")]
        public string PostXNKK(string action, List<DanhSachVatTuKiemKeDto> lstData, string Para1 = "")
        {
            string imageDataA = lstData[0].GhiChu;
            string datetime = DateTime.Now.ToString("ddMMyyyyHHss");
            string imageNameA = SaveSignatureImage(imageDataA, $"SignNgXNKK", "", datetime);



            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            foreach (DataRow item in tbl.Rows)
            {
                item["GhiChu"] = imageNameA;
            }

            return new DangKyVatTuModel().Post(action, Para1, tbl);
        }

        [HttpGet]
        [Route("GetXNHX")]
        public DataTable GetXNHX(string action, string Para1 = "", string Para2 = "", string Para3 = "", string Para4 = "", string Para5 = "", string Para6 = "")
        {
            return new DangKyVatTuModel().GetXNHX(action, Para1, Para2, Para3, Para4, Para5, Para6);
        }

        [HttpPost]
        [Route("PostXNHX")]
        public string PostXNHX(string action, List<XacNhanHangXuatDto> lstData)
        {
            string imageDataA = lstData[0].KyTen;
            string[] imageDataParts = imageDataA.Split(',');
            string imageName = "";
            string datetime = DateTime.Now.ToString("ddMMyyyyHHss");
            if (!string.IsNullOrEmpty(imageDataA))
            {
                byte[] bytes = Convert.FromBase64String(imageDataParts[1]);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    string tenhinh = $"{lstData[0].NguoiKT}-{datetime}";
                    Image image = Image.FromStream(ms);
                    string uploadPath = HttpContext.Current.Server.MapPath("~/Images/XuatHangNPL/XacNhanKT");
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
            lstData[0].KyTen = imageName;
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            return new DangKyVatTuModel().PostXNHX(action, tbl);
        }

        [HttpPost]
        [Route("PostKiemKe")]
        public string PostKiemKe(string action, List<DanhSachVatTuKiemKeDto> lstData, string Para1 = "")
        {
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            return new DangKyVatTuModel().Post(action, Para1, tbl);
        }

    }

}

public class PhieuDangKyXuatVT
{
    public string PhieuDK { get; set; }
    public int? Dot { get; set; }
    public string MaLenhSX { get; set; }
    public string MaDH { get; set; }
    public string MaLenh { get; set; }
    public string MaNPL { get; set; }
    public double? SLDK { get; set; }
    public string GhiChu { get; set; }
    public string MaVT { get; set; }
    public string MauVT { get; set; }
    public string KhoVai { get; set; }
    public string MaDVVT { get; set; }
    public DateTime? NgayDK { get; set; }
    public DateTime? NgayCap { get; set; }
    public TimeSpan? GioCap { get; set; }
    public DateTime? NgayTao { get; set; }
    public int? IsNPL { get; set; }
    public string NguoiDK { get; set; }
    public string Status { get; set; }
}

public class XacNhanHangXuatDto
{
    public string MaLenh { get; set; }
    public string PhieuXH { get; set; }
    public string KyTen { get; set; }
    public string NguoiKT { get; set; }
    public int IsNPL { get; set; }
}
public class DanhSachVatTuKiemKeDto
{
    public string PhieuVatTuKK { get; set; }
    public string MaNPL { get; set; }
    public string MaVT { get; set; }
    public string MauVT { get; set; }
    public string KhoVai { get; set; }
    public string MaDVVT { get; set; }
    public string NguoiTao { get; set; }
    public int? IsLog { get; set; }
    public int? IsNPL { get; set; }
    public string GhiChu { get; set; }
    public int? Dot { get; set; }
    public double? TonDKy { get; set; }
    public double? TonCKy { get; set; }
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public double? SLNhapKho { get; set; }
    public string ChiTiet { get; set; }
}

public class TYPE_ERP_PhieuDeNghiTHVT
{
    public string PhieuTH { get; set; }
    public int? Dot { get; set; }
    public string MaLenhSX { get; set; }
    public string MaDH { get; set; }
    public string MaLenh { get; set; }
    public string MaNPL { get; set; }
    public double? SLDK { get; set; }
    public string GhiChu { get; set; }
    public string MaVT { get; set; }
    public string MauVT { get; set; }
    public string KhoVai { get; set; }
    public string MaDVVT { get; set; }
    public DateTime? NgayDK { get; set; }
    public DateTime? NgayTH { get; set; }
    public DateTime? NgayTao { get; set; }
    public int? IsNPL { get; set; }
    public string NguoiTH { get; set; }
    public string PhieuDK { get; set; }
    public string SignNgDK { get; set; }
    public string NgayKi { get; set; }
    public string SignTBPNgDK { get; set; }
    public string NgayKiTBPNgDK { get; set; }
    public string SignMer { get; set; }
    public string NgaySignMer { get; set; }
    public string SignTBPMer { get; set; }
    public string NgaySignTBPMer { get; set; }
    public int?  Status { get; set; }
}