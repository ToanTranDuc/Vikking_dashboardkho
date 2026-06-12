using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Model.KeHoach;
using NtbSoft.ERP.Model.Kho;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [RoutePrefix("api/NHGiaCong")]
    public class NhanHangGiaCongController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action,string para1, string para2, string para3, string para4, string para5, string para6, string para7 ="", string para8 = "")
        {
            return new NHGiaCongModel().Get(action,para1,para2,para3, para4, para5, para6, para7, para8);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(dynamic dt)
        {
            List<NhanHangGiaoCongEntity> lstData = new List<NhanHangGiaoCongEntity>();
            foreach (var item in dt)
            {
                lstData.Add(new NhanHangGiaoCongEntity()
                {
                    MaLenh = item.MaLenh.ToString(),
                    MaDH = item.MaDH.ToString(),
                    MaDVSX = item.MaDVSX.ToString(),
                    POID = item.POID.ToString(),
                    PO = item.PO.ToString(),
                    MaMau = item.MaMau.ToString(),
                    TenMau = item.TenMau.ToString(),
                    DauSizeID = item.DauSizeID.ToString(),
                    DauSize = item.DauSize.ToString(),
                    SizeID = item.SizeID.ToString(),
                    Size = item.Size.ToString(),
                    SoLuong = (int)item.SoLuong,
                    LuyKe = (int)item.LuyKe,
                    SoLuongKH = (int)item.SoLuongKH,
                    Dot = (int)item.Dot,
                    NhanVien = item.NhanVien.ToString(),
                    Status = (int)item.Status,
                    IsHoanThanh =(int)item.IsHoanThanh,
                    SoKien = (int)item.SoKien
                });
            }
            
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new NHGiaCongModel().Post(tbl,"Post", "@TypeTable");
        }     
        [HttpPost]
        [Route("PostDT")]
        public string PostDT(dynamic data)
        {
            List<DongThungList> lstData = new List<DongThungList>();
            foreach (var item in data)
            {
                string NgayGio = item.NgayNhapKho;
                DateTime NgayGioNK = DateTime.Parse(NgayGio);
                lstData.Add(new DongThungList()
                {
                    MaDH = item.MaDH.ToString(),
                    MaLenh = item.MaLenh.ToString(),
                    MaDVSX = item.MaDVSX.ToString(),
                    POID = item.POID.ToString(),
                    MaPKL = item.MaPKL.ToString(),
                    MinSttThung = (int)item.MinSttThung,
                    MaxSttThung = (int)item.MaxSttThung,
                    TuThung = (int)item.TuThung,
                    DenThung = (int)item.DenThung,
                    MaKho = item.MaKho.ToString(),
                    NgayNhapKho_TC = NgayGioNK
                }) ;
            }
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new NHGiaCongModel().Post(tbl, "UpdateErpDongThung", "@TypeUpDateDT");
        }
    }
    public class DongThungList
    {
        public  string MaDH { get; set; }
        public string MaPKL { get; set; }
        public string MaDVSX { get; set; }
        public string MaLenh { get; set; }
        public string POID { get; set; }
        public string MaKho { get; set; }
        public string MaKhoDen { get; set; }
        public int TuThung { get; set; }
        public int DenThung { get; set; }
        public int MinSttThung { get; set; }
        public int MaxSttThung { get; set; }
        public int SttThungDecat { get; set; }
        public DateTime? NgayNhapKho_TC { get; set; }
    }
}