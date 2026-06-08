using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Model.QuanLyDonHang;
using NtbSoft.ERP.Web.Repository.R.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    //[HMACAuthentication]
    [RoutePrefix("api/BOM")]
    public class BomNguyenPhuLieuController : ApiController
    {
        IBomNguyenPhuLieuRepository _repo = new BomNguyenPhuLieuRepository();
        [HttpGet]
        [Route("Get_TikKiem")]
        public DataTable Get_TikKiem(string _mahang)
        {
            return _repo.Get_TikKiem(_mahang);
        }
        [HttpGet]
        [Route("GET_VatTu")]
        public DataTable GET_VatTu(string mavt)
        {
            return _repo.GET_VatTu(mavt);
        }
        [HttpGet]
        [Route("GET_TenVatTu")]
        public DataTable GET_TenVatTu()
        {
            return _repo.GET_TenVatTu();
        }
        [HttpGet]
        [Route("GETMH")]
        public DataTable GETMH()
        {
            return _repo.GETMH();
        }
        [HttpGet]
        [Route("GET_Mau")]
        public DataTable GET_Mau(string _mahang)
        {
            return _repo.GET_Mau(_mahang);
        }
        [HttpGet]
        [Route("GET_InSeam")]
        public DataTable GET_InSeam(string _mahang)
        {
            return _repo.GET_InSeam(_mahang);
        }
        [HttpGet]
        [Route("GET_Size")]
        public DataTable GET_Size(string _mahang/*, string _inseam*/)
        {
            return _repo.GET_Size(_mahang/*, _inseam*/);
        }
        [HttpGet]
        [Route("GET_MauVatTu")]
        public DataTable GET_MauVatTu(string _mavt)
        {
            //_mavt = HttpUtility.UrlDecode(_mavt);//
            return _repo.GET_MauVatTu(_mavt);
        }
        [HttpGet]
        [Route("GET_KhoSizebyMau")]
        public DataTable GET_KhoSizebyMau(string _mavt,string _mau)
        {
            return _repo.GET_KhoSizebyMau(_mavt,_mau);
        }
        [HttpGet]
        [Route("GET_MaubyKhoSize")]
        public DataTable GET_MaubyKhoSize(string _mavt, string _khosize)
        {
            //_mavt = HttpUtility.UrlDecode(_mavt);//
            return _repo.GET_MaubyKhoSize(_mavt, _khosize);
        }
        [HttpGet]
        [Route("GET_KhoSizeVatTu")]
        public DataTable GET_KhoSizeVatTu(string _mavt)
        {
            return _repo.GET_KhoSizeVatTu(_mavt);
        }
        ////Post
        [HttpPost]
        [Route("PostBomVT")]
        public string PostBomVT(DataTable tblBOM)
        {
            return _repo.PostBomVT(tblBOM);
        }
        //DETELE
        [HttpDelete]
        [Route("DeleteBomNPL")]
        public string DeleteBomVT(string _mahang, string _mavt)
        {
            return _repo.DeleteBomVT(_mahang, _mavt);
        }
        //[HttpDelete]
        //[Route("DeleteBomDongVT")]
        //public string DeleteBomDongVT([FromBody] List<BomNguyenPhuLieuEntity> obj, string MaBom)
        //{
        //    if (obj == null) return "false";
        //    string json = JsonConvert.SerializeObject(obj);
        //    DataTable tbBomNL = JsonConvert.DeserializeObject<DataTable>(json);
        //    return _repo.DeleteBomDongVT(tbBomNL,MaBom);
        //}
        [HttpPost]
        [Route("PostBomVTCopy")]
        public string PostBomVTCopy(DataTable tblBOM)
        {
            return new BomNguyenPhuLieuModel().PostBomVTCopy(tblBOM);
        }
        [HttpGet]
        [Route("GET_DinhMucNPL")]
        public DataTable GET_DinhMucNPL(string _mahang)
        {
            return _repo.GET_DinhMucNPL(_mahang);
        }
        [HttpGet]
        [Route("Update")]
        public string UpdateXacNhan(string action,string para,string para2)
        {
            return new BomNguyenPhuLieuModel().UpdateXacNhan(action, para, para2);
        }
    }
}