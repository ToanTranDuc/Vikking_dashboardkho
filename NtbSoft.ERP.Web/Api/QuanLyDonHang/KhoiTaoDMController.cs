using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using NtbSoft.ERP.Model.QuanLyDonHang;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/KhoiTaoDM")]
    public class KhoiTaoDMController : ApiController
    {
        KhoiTaoDMModel _model = new KhoiTaoDMModel();
        [HttpGet]
        [Route("GetKH")]
        public DataTable GetKH()
        {
            return _model.GetKH();
        }
        [HttpGet]
        [Route("GetMH")]
        public DataTable GetMH(string makh)
        {
            return _model.GetMH(makh);
        }
        [HttpGet]
        [Route("GetSttDot")]
        public DataTable GetSttDot(string makh, string mahang)
        {
            return _model.GetSttDot(makh,mahang);
        }
        [HttpGet]
        [Route("GetThongSoDot")]
        public DataTable GetThongSoDot(string makh, string mahang)
        {
            return _model.GetThongSoDot(makh, mahang);
        }
        [HttpGet]
        [Route("GetSizeDMCT")]
        public DataTable GetSizeDMCT(string makh, string mahang)
        {
            return _model.GetSizeDMCT(makh, mahang);
        }
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string makh,string mahang)
        {
            return _model.Get(makh,mahang);
        }
        [HttpGet]
        [Route("GetKTBom")]
        public DataTable GetKTBom(string makh, string mahang, string madot)
        {
            return _model.GetKTBom(makh, mahang, madot);
        }
        [HttpGet]
        [Route("GetDM")]
        public DataTable GetDM(string makh,string mahang, string madot)
        {
            return _model.GetDM(makh, mahang,madot);
        }
        [HttpGet]
        [Route("GetDMGN")]
        public DataTable GetDMGN(string makh, string mahang, string madot)
        {
            return _model.GetDMGN(makh, mahang, madot);
        }
        [HttpGet]
        [Route("GetEditDinhMuc")]
        public DataTable GetEditDinhMuc(string makh, string mahang, string madot)
        {
            return _model.GetEditDinhMuc(makh, mahang, madot);
        }
        [HttpGet]
        [Route("GetSizeDinhMucChung")]
        public DataTable GetSizeDinhMucChung(string makh, string mahang, string manhom, string mavtid, string mauvtid, string khovaiid, string madot, int isnew)
        {
            return _model.GetSizeDinhMucChung(makh, mahang, manhom, mavtid, mauvtid, khovaiid, madot, isnew);
        }
        [HttpGet]
        [Route("GetDot")]
        public DataTable GGetDotet(string makh, string mahang)
        {
            return _model.GetDot(makh, mahang);
        }
        [HttpGet]
        [Route("GetSuaDM")]
        public DataTable GetSuaDM(string makh, string mahang, string madot)
        {
            return _model.GetSuaDM(makh, mahang, madot);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable tbl)
        {
            return _model.Post(tbl);
        }
        [HttpGet]
        [Route("GetDMChiTiet")]
        public DataTable GetDinhMucChiTiet(string makh, string mahang, string manhom, string mavtid, string mauvtid, string khovaiid, string madot)
        {
            return _model.GetDinhMucChiTiet(makh, mahang,manhom,mavtid,mauvtid,khovaiid,madot);
        }
        [HttpGet]
        [Route("GetCheckSave")]
        public DataTable GetCheckSave(string makh, string mahang, string manhom, string mavtid, string mauvtid, string khovaiid, string madot)
        {
            return _model.GetCheckSize(makh, mahang, manhom, mavtid, mauvtid, khovaiid, madot);
        }
        [HttpPost]
        [Route("SaveSizeSP")]
        public string SaveSizeSP(string para, DataTable tbl)
        {
            return _model.PostSaveSize(para, tbl);
        }
        [HttpPost]
        [Route("PostDMChiTiet")]
        public string PostDMChiTiet(string para,DataTable tbl)
        {
            return _model.PostDMChiTiet(para,tbl);
        }
        [HttpPost]
        [Route("PostDMChungNULL")]
        public string PostDMChungNULL(DataTable tbl)
        {
            return _model.PostDMChungNULL(tbl);
        }
        [HttpDelete]
        [Route("DeleteDM")]
        public DataTable DeleteDM(string makh, string mahang, string manhom, string mavtid, string mauvtid, string khovaiid, string madot)
        {
            return _model.DeleteDM(makh, mahang, manhom, mavtid, mauvtid, khovaiid, madot);
        }
        [HttpGet]
        [Route("CheckCanDoi")]
        public DataTable CheckCanDoi(string para, string para1,string para2, string para3, string para4)
        {
            return _model.CheckCanDoi(para, para1, para2, para3, para4);
        }
        [HttpGet]
        [Route("CheckVTCreate")]
        public DataTable CheckVTCreate(string para, string para1, string para2, string para3)
        {
            return _model.CheckVTCreate(para, para1, para2, para3);
        }
        [HttpGet]
        [Route("GetMaDot")]
        public DataTable GetMaDot(string para, string para1, string para2)
        {
            return _model.GetMaDot(para, para1, para2);
        }
        [HttpGet]
        [Route("PostCopy")]
        public string PostCopy(string makh, string mahang, string madotcopy, string madot, string dot, string username)
        {
            return _model.PostCopy(makh, mahang, madotcopy, madot,dot,username);
        }
        [HttpGet]
        [Route("DuyetAll")]
        public string DuyetAll(string makh, string mahang, string isduyet, string username, string madot)
        {
            return _model.DuyetAll(makh, mahang, isduyet, username, madot);
        }

        [HttpPost]
        [Route("PostCopyVT")]
        public string PostCopyVT(string makh, string mahang, string manhom, string mavtid, string mauvtid, string khovaiid, string madot,string mamauid,[FromBody]DataTable tbl)
        {
            return _model.PostCopyVT(makh, mahang, manhom, mavtid, mauvtid, khovaiid, madot, mamauid, tbl);
        }
        [HttpGet]
        [Route("KiemTraDuyet")]
        public DataTable KiemTraDuyet(string username)
        {
            return _model.KiemTraDuyet(username);
        }
        [HttpGet]
        [Route("KiemTraHuy")]
        public DataTable KiemTraHuy(string username)
        {
            return _model.KiemTraHuy(username);
        }
        [HttpGet]
        [Route("KiemTraDot")]
        public DataTable KiemTraDot(string madot)
        {
            return _model.KiemTraDot(madot);
        }
        [HttpGet]
        [Route("GETVATTUEXCEL")]
        public DataTable GETVATTUEXCEL(string para, string para1, string para2)
        {
            return _model.GETVATTUEXCEL(para, para1, para2);
        }

        [HttpPost]
        [Route("PostNew")]
        public string PostNew(DataTable tbl, string para, string para1)
        {
            return _model.PostNew(tbl, para,para1);
        }
        [HttpPost]
        [Route("GetKiemTra")]
        public DataTable GetKiemTra(DataTable tbl, string makh, string mahang)
        {
            return new KhoiTaoDMModel().GetKiemTra(tbl, makh, mahang);
        }
        [HttpGet]
        [Route("GetDonVi")]
        public DataTable GetDonVi()
        {
            return new KhoiTaoDMModel().GetDonVi();
        }
    }
}