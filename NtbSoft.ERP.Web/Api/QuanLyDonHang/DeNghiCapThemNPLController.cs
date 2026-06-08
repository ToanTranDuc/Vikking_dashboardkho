using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using NtbSoft.ERP.Model.QuanLyDonHang;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;


namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/DeNghiCapThemNPL")]
    public class DeNghiCapThemNPLController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1 = "", string Para2 = "", string Para3 = "", string Para4 = "", string Para5 = "", string Para6 = "", string Para7 = "", string Para8 = "", string Para9 = "")
        {
            return new DeNghiCapThemNPLModel().Get(action, Para1, Para2, Para3, Para4, Para5, Para6,Para7,Para8,Para9);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(string action, List<TYPE_ERP_PhieuDeNghiCapThemVT> lstData, string Para1 = "", string Para2 = "", string Para3 = "", string Para4 = "", string Para5 = "", string Para6 = "", string Para7 = "", string Para8 = "", string Para9 = "")
        {
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            return new DeNghiCapThemNPLModel().Post(action, tbl, Para1, Para2, Para3, Para4, Para5, Para6, Para7, Para8, Para9);
        }

        [HttpPost]
        [Route("PostDNCT")]
        public string PostDNCT(string action, List<TYPE_ERP_PhieuDeNghiCapThemVT> lstData, string Para1 = "", string Para2 = "", string Para3 = "", string Para4 = "", string Para5 = "", string Para6 = "", string Para7 = "", string Para8 = "", string Para9 = "")
        {
            if (lstData == null || lstData.Count == 0)
                return "False";
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

            return new DeNghiCapThemNPLModel().Post(action, tbl, Para1, Para2, Para3, Para4, Para5, Para6, Para7, Para8, Para9);
        }


        [HttpPost]
        [Route("PostChiTiet")]
        public string PostChiTiet(string action, List<Type_ERP_PhieuDeNghiCapThemVTChiTiet> lstData, string Para1 = "", string Para2 = "", string Para3 = "", string Para4 = "", string Para5 = "", string Para6 = "", string Para7 = "", string Para8 = "", string Para9 = "")
        {
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            return new DeNghiCapThemNPLModel().PostChiTiet(action, tbl, Para1, Para2, Para3, Para4, Para5, Para6, Para7, Para8, Para9);
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
                string uploadPath = HttpContext.Current.Server.MapPath("~/Images/SignDeNghiCapThemNPL");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                string fullPath = Path.Combine(uploadPath, fileName);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                image.Save(fullPath);
                return fileName;
            }
        }

       
    }

}


public class TYPE_ERP_PhieuDeNghiCapThemVT
{
    public string PhieuTH { get; set; }
    public int? Dot { get; set; }
    public string MaLenhSX { get; set; }
    public string MaDH { get; set; }
    public string MaLenh { get; set; }
    public string MaNPL { get; set; }
    public double? SLDK { get; set; }
    public double? SLDKSP { get; set; }
    public string MaCode { get; set; }       
    public string GhiChu { get; set; }
    public string MaNhom { get; set; }
    public string MaVTID { get; set; }
    public string MauVTID { get; set; }
    public string KhoVaiID { get; set; }
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
    public int? IsTV { get; set; }
    public double? DinhMuc { get; set; }
    public double? CapPhat { get; set; }
    public double? SoLuong { get; set; }
    public string MaNhomChiTiet { get; set; }
    public bool? PhatSinhChiPhi { get; set; }
    public int? TrongDinhMuc { get; set; }
    public int? NgoaiDinhMuc { get; set; }
}

public class Type_ERP_PhieuDeNghiCapThemVTChiTiet
{
    public string PhieuTH { get; set; }
    public int? Dot { get; set; }
    public string MaLenhSX { get; set; }
    public string MaDH { get; set; }
    public string MaLenh { get; set; }
    public string MaNPL { get; set; }
    public string MaNhom { get; set; }
    public string MaVTID { get; set; }
    public string MauVTID { get; set; }
    public string KhoVaiID { get; set; }
    public int? IsTV { get; set; }
    public string MaMauSP { get; set; }
    public string TenMauSP { get; set; }
    public string MaNhomSize { get; set; }
    public string NhomSize { get; set; }
    public string MaSize { get; set; }
    public string TenSize { get; set; }
    public double? SoLuong { get; set; }
}