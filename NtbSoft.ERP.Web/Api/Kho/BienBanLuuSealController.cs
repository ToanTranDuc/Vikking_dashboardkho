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
    [RoutePrefix("api/BienBanLuuSeal")]
    public class BienBanLuuSealController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para1, string para2, string para3, string para4, string para5)
        {
            return new BienBanLuuSealModel().Get(action, para1, para2, para3, para4, para5);
        }

        [HttpPost]
        [Route("PostSeal")]
        public string PostDT(dynamic data)
        {
            List<BienBanLuuSeal> lstData = new List<BienBanLuuSeal>();
            foreach (var item in data)
            {
                lstData.Add(new BienBanLuuSeal()
                {
                    MaPhieu = item.MaPhieu.ToString(),
                    MaCont = item.MaCont.ToString(),
                    MaSeal = item.MaSeal.ToString(),
                    TenSeal = item.TenSeal.ToString(),
                    MaPKL = item.MaPKL.ToString(),
                    SoXe = item.SoXe.ToString(),
                    Status = (int)item.Status,
                    NgayLuu = "",
                });
            }
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new BienBanLuuSealModel().Post(tbl, "PostSeal", "@TyTable");
        }
        [HttpPost]
        [Route("PostDSXe")]
        public string PostDSXe(dynamic data)
        {
            List<BienBanDSXe> lstData = new List<BienBanDSXe>();
            foreach (var item in data)
            {
                lstData.Add(new BienBanDSXe()
                {
                    MaCont = item.MaCont.ToString(),
                    MaSeal = item.MaSeal.ToString(),
                    TenSeal = item.TenSeal.ToString(),
                    MaPKL = item.MaPKL.ToString(),
                    TenTaiXe = item.TenTaiXe.ToString(),
                    CMND = item.CMND.ToString(),
                    SoXe = item.SoXe.ToString(),
                    SLSP = (int)item.SLSP,
                    SLThung = (int)item.SLThung,
                    Module = (int)item.Module,

                });
            }
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new BienBanLuuSealModel().Post(tbl, "PostListXe", "@TypeTableDSXe");
        }
    }
    public class BienBanLuuSeal
    {
        public string MaPhieu { get; set; }
        public string MaCont { get; set; }
        public string MaSeal { get; set; }
        public string TenSeal { get; set; }
        public string MaPKL { get; set; }
        public string SoXe { get; set; }
        public int Status { get; set; }
        public string NgayLuu { get; set; }
    }
    public class BienBanDSXe
    {
        public string MaCont { get; set; }
        public string MaSeal { get; set; }
        public string TenSeal { get; set; }
        public string MaPKL { get; set; }
        public string TenTaiXe { get; set; }
        public string CMND { get; set; }
        public string SoXe { get; set; }
        public int SLSP { get; set; }
        public int SLThung { get; set; }
        public int Module { get; set; }
    }
}