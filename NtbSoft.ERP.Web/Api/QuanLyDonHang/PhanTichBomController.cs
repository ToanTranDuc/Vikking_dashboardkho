using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/PhanTichBom")]
    public class PhanTichBomController : ApiController
    {
        [HttpGet]
        [Route("GetKH")]
        public DataTable GetKH()
        {
            return new PhanTichBomModel().GetKH();
        }
        [HttpGet]
        [Route("GetMH")]
        public DataTable GetMH(string makh)
        {
            return new PhanTichBomModel().GetMH(makh);
        }
        [HttpGet]
        [Route("GetSttDot")]
        public DataTable GetSttDot()
        {
            return new PhanTichBomModel().GetSttDot();
        }
        [HttpGet]
        [Route("GetMauSanPham")]
        public DataTable GetMauSanPham(string makh, string mahang, string mavtID, string mauID, string dot)
        {
            return new PhanTichBomModel().GetMauSanPham(makh, mahang, mavtID, mauID, dot);
        }
        [HttpGet]
        [Route("GetMauSanPhamNew")]
        public DataTable GetMauSanPhamNew(string makh, string mahang, string mavtID, string mauID, string dot)
        {
            return new PhanTichBomModel().GetMauSanPhamNew(makh, mahang, mavtID, mauID, dot);
        }
        [HttpGet]
        [Route("GetMauSanPhamNull")]
        public DataTable GetMauSanPhamNull(string makh, string mahang)
        {
            return new PhanTichBomModel().GetMauSanPhamNull(makh, mahang);
        }
        [HttpGet]
        [Route("GetDauSize")]
        public DataTable GetDauSize(string makh, string mahang, string mavtID)
        {
            return new PhanTichBomModel().GetDauSize(makh, mahang, mavtID);
        }
        [HttpGet]
        [Route("GetDauSizeNull")]
        public DataTable GetDauSizeNull(string makh, string mahang)
        {
            return new PhanTichBomModel().GetDauSizeNull(makh, mahang);
        }
        [HttpGet]
        [Route("GetSize")]
        public DataTable GetSize(string makh, string mahang, string dausize, string mavtID)
        {
            return new PhanTichBomModel().GetSize(makh, mahang, dausize, mavtID);
        }
        [HttpGet]
        [Route("GetSizeNull")]
        public DataTable GetSizeNull(string makh, string mahang, string dausize)
        {
            return new PhanTichBomModel().GetSizeNull(makh, mahang, dausize);
        }
        [HttpGet]
        [Route("GetChiTietSize")]
        public DataTable GetChiTietSize(string makh, string mahang, string mavtID, string mauID, string dot)
        {
            return new PhanTichBomModel().GetChiTietSize(makh, mahang, mavtID, mauID, dot);
        }
        [HttpGet]
        [Route("GetChiTietSizeNew")]
        public DataTable GetChiTietSizeNew(string makh, string mahang, string mavtID, string mauID, string dot)
        {
            return new PhanTichBomModel().GetChiTietSizeNew(makh, mahang, mavtID, mauID, dot);
        }
        [HttpGet]
        [Route("GetChiTietVT")]
        public DataTable GetChiTietVT(string makh, string mahang, string mavtID, string mauID)
        {
            return new PhanTichBomModel().GetChiTietVT(makh, mahang, mavtID, mauID);
        }
        [HttpGet]
        [Route("GetVatTu")]
        public DataTable GetVatTu(string makh, string mahang)
        {
            return new PhanTichBomModel().GetVatTu(makh, mahang);
        }
        [HttpGet]
        [Route("GetVatTuNull")]
        public DataTable GetVatTuNull(string makh, string mahang)
        {
            return new PhanTichBomModel().GetVatTuNull(makh, mahang);
        }
        [HttpGet]
        [Route("GetKTVatTu")]
        public DataTable GetKTVatTu(string makh, string mahang, string madot)
        {
            return new PhanTichBomModel().GetKTVatTu(makh, mahang, madot);
        }
        [HttpGet]
        [Route("GetVatTuSP")]
        public DataTable GetVatTuSP(string makh, string mahang)
        {
            return new PhanTichBomModel().GetVatTuSP(makh, mahang);
        }
        [HttpGet]
        [Route("GetMauVT")]
        public DataTable GetMauVT(string makh, string mahang, string mavtID, string madot)
        {
            return new PhanTichBomModel().GetMauVT(makh, mahang, mavtID, madot);
        }
        [HttpGet]
        [Route("GetMauVTNew")]
        public DataTable GetMauVTNew(string makh, string mahang, string mavtID, string madot)
        {
            return new PhanTichBomModel().GetMauVTNew(makh, mahang, mavtID, madot);
        }
        [HttpGet]
        [Route("GetMauVTNull")]
        public DataTable GetMauVTNull(string makh, string mahang)
        {
            return new PhanTichBomModel().GetMauVTNull(makh, mahang);
        }
        [HttpGet]
        [Route("GetKhoVai")]
        public DataTable GetKhoVai(string makh, string mahang)
        {
            return new PhanTichBomModel().GetKhoVai(makh, mahang);
        }
        [HttpGet]
        [Route("GetDot")]
        public DataTable GetDot(string makh, string mahang)
        {
            return new PhanTichBomModel().GetDot(makh, mahang);
        }
        [HttpPost]
        [Route("PostMauSP")]
        public string PostMauSP(DataTable tbl)
        {
            return new PhanTichBomModel().PostMauSP(tbl);
        }
        [HttpPost]
        [Route("PostMauSPNull")]
        public string PostMauSPNull(DataTable tbl)
        {
            return new PhanTichBomModel().PostMauSPNull(tbl);
        }
        [HttpPost]
        [Route("PostSizeSP")]
        public string PostSizeSP(DataTable tbl)
        {
            return new PhanTichBomModel().PostSizeSP(tbl);
        }
        [HttpPost]
        [Route("PostSizeSPNull")]
        public string PostSizeSPNull(DataTable tbl)
        {
            return new PhanTichBomModel().PostSizeSPNull(tbl);
        }
        [HttpPost]
        [Route("PostMauVT")]
        public string PostMauVT(DataTable tbl)
        {
            return new PhanTichBomModel().PostMauVT(tbl);
        }
        [HttpPost]
        [Route("PostMauVTNull")]
        public string PostMauVTNull(DataTable tbl)
        {
            return new PhanTichBomModel().PostMauVTNull(tbl);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string makh, string mahang, string mavtID, string mauID, string dausize, string madot)
        {
            return new PhanTichBomModel().Delete(makh, mahang, mavtID, mauID, dausize, madot);
        }
        [HttpDelete]
        [Route("DeleteDot")]
        public string DeleteDot(string makh, string mahang, string mavtID, string mauID, string madot)
        {
            return new PhanTichBomModel().DeleteDot(makh, mahang, mavtID, mauID, madot);
        }
        #region quan 
        [HttpGet]
        [Route("GetKHDotDM")]
        public DataTable GetKHDotDM()
        {
            return new PhanTichBomModel().GetKHDotDM();
        }
        [HttpGet]
        [Route("GetMHDotDM")]
        public DataTable GetMHDotDM(string makh)
        {
            return new PhanTichBomModel().GetMHDotDM(makh);
        }
        [HttpGet]
        [Route("GetDotDM")]
        public DataTable GetDotDM(string makh, string mahang, string mavtID)
        {
            return new PhanTichBomModel().GetDotDM(makh, mahang, mavtID);
        }
        [HttpGet]
        [Route("GetChiTietVTDotDM")]
        public DataTable GetChiTietVTDotDM(string makh, string mahang, string mavtID, string mauID, string makhoID, string madot, string tachmau, string manhom, string macode)
        {
            return new PhanTichBomModel().GetChiTietVTDotDM(makh, mahang, mavtID, mauID, makhoID, madot, tachmau, manhom, macode);
        }
        [HttpGet]
        [Route("GetVatTuDotDM")]
        public DataTable GetVatTuDotDM(string makh, string mahang)
        {
            return new PhanTichBomModel().GetVatTuDotDM(makh, mahang);
        }
        [HttpPost]
        [Route("PostDotDM")]
        public string PostDotDM(DataTable tbl)
        {
            return new PhanTichBomModel().PostDotDM(tbl);
        }
        [HttpGet]
        [Route("GetVTSPDotDM")]
        public DataTable GetVTSPDotDM(string makh, string mahang, string mavtID)
        {
            return new PhanTichBomModel().GetVTSPDotDM(makh, mahang, mavtID);
        }
        [HttpPost]
        [Route("PostDotDMHUY")]
        public string PostDotDMHuy(DataTable tbl)
        {
            return new PhanTichBomModel().PostDotDMHuy(tbl);
        }

        [HttpPost]
        [Route("POSTDUYETALL")]
        public string POSTDUYETALL(DataTable tbl)
        {
            return new PhanTichBomModel().PostDotDMDUYETALL(tbl);
        }
        [HttpPost]
        [Route("POSTHUYALL")]
        public string POSTHUYALL(DataTable tbl)
        {
            return new PhanTichBomModel().PostDotDMHUYALL(tbl);
        }
        [HttpPost]
        [Route("PostDM")]
        public string PostDM(DataTable tbl)
        {
            return new PhanTichBomModel().PostDM(tbl);
        }
        [HttpPost]
        [Route("PostDMChung")]
        public string PostDMChung(DataTable tbl)
        {
            return new PhanTichBomModel().PostDMChung(tbl);
        }
        #endregion
        [HttpGet]
        [Route("GetExcel")]
        public DataTable GetExcel(string makh, string mahang)
        {
            return new PhanTichBomModel().GetExcel(makh, mahang);
        }
        [HttpGet]
        [Route("GetExcel2")]
        public DataTable GetExcel2(string makh, string mahang)
        {
            return new PhanTichBomModel().GetExcel2(makh, mahang);
        }
        [HttpDelete]
        [Route("DeleteDotBOM")]
        public string DeleteDotBOM(string makh, string mahang, string madot, string userid)
        {
            return new PhanTichBomModel().DeleteDotBOM(makh, mahang, madot,userid);
        }
        [HttpGet]
        [Route("CopyBOM")]
        public string CopyBOM(string maKH, string maHang, string maKH_copy, string maHang_copy, string maDot_copy, string dot_copy)
        {
            return new PhanTichBomModel().CopyBOM(maKH, maHang, maKH_copy, maHang_copy, maDot_copy, dot_copy);
        }
    }
}