using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Model.Kho;
using NtbSoft.ERP.Web.Repository.R.Kho;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.Kho
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/NhapKho")]
    public class NhapKhoController : ApiController
    {
        INhapKhoRepository _repo = new NhapKhoRepository();
        [HttpGet]
        [Route("GetDS")]
        public DataTable GetDS()
        {
            return _repo.GetDS();
        }
        [HttpGet]
        [Route("GetThongTin")]
        public DataTable GetThongTin(int lcsx, string pageIndex, string pageSize, string madvsx)
        {
            return _repo.GetThongTin(lcsx, pageIndex, pageSize, madvsx);
        }
        [HttpGet]
        [Route("GetThongTinSMS")]
        public DataTable GetThongTinSMS(int lcsx, string pageIndex, string pageSize, string madvsx)
        {
            return _repo.GetThongTinSMS(lcsx, pageIndex, pageSize, madvsx);
        }
        [HttpGet]
        [Route("GetThongTinPO")]
        public DataTable GetThongTinPO(string madh, int lcsx, string madvsx)
        {
            return _repo.GetThongTinPO(madh, lcsx, madvsx);
        }
        [HttpGet]
        [Route("GetThongTinPOSMS")]
        public DataTable GetThongTinPOSMS(string madh, int lcsx, string madvsx)
        {
            return _repo.GetThongTinPOSMS(madh, lcsx, madvsx);
        }
        [HttpGet]
        [Route("GetThongTinPOCT")]
        public DataTable GetThongTinPOCT(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat, int lcsx)
        {
            return _repo.GetThongTinPOCT(mapkl, madvsx, malenh, poid, madh, malenhsanxuat, lcsx);
        }
        [HttpGet]
        [Route("GetThongTinPOCTSMS")]
        public DataTable GetThongTinPOCTSMS(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat, int lcsx)
        {
            return _repo.GetThongTinPOCTSMS(mapkl, madvsx, malenh, poid, madh, malenhsanxuat, lcsx);
        }
        [HttpGet]
        [Route("GetThongTinChiTiet")]
        public DataTable GetThongTinChiTiet(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int lcsx, int minsttthung, int maxsttthung)
        {
            return _repo.GetThongTinChiTiet(mapkl, malenh, madvsx, poid, dausizeid, mau, tuthung, denthung, lcsx, minsttthung, maxsttthung);
        }
        [HttpGet]
        [Route("GetThongTinChiTietSMS")]
        public DataTable GetThongTinChiTietSMS(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int lcsx, int minsttthung, int maxsttthung)
        {
            return _repo.GetThongTinChiTietSMS(mapkl, malenh, madvsx, poid, dausizeid, mau, tuthung, denthung, lcsx, minsttthung, maxsttthung);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            return _repo.Post(tblNhapKho, isdongthung, lcsx, statuschuyen);
        }
        [HttpPost]
        [Route("PostSMS")]
        public string PostSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            return _repo.PostSMS(tblNhapKho, isdongthung, lcsx, statuschuyen);
        }
        [HttpPost]
        [Route("PostALL")]
        public string PostALL(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, string mahang, string po)
        {
            return _repo.PostALL(tblNhapKho, isdongthung, lcsx, statuschuyen, mahang, po);
        }
        [HttpPost]
        [Route("PostALLSMS")]
        public string PostALLSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, string mahang, string po)
        {
            return _repo.PostALLSMS(tblNhapKho, isdongthung, lcsx, statuschuyen, mahang, po);
        }
        [HttpPost]
        [Route("PostDT")]
        public string PostDT(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            return _repo.PostDT(tblNhapKho, isdongthung, lcsx, statuschuyen);
        }
        [HttpPost]
        [Route("PostDTSMS")]
        public string PostDTSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            return _repo.PostDTSMS(tblNhapKho, isdongthung, lcsx, statuschuyen);
        }
        [HttpPost]
        [Route("PostHuy")]
        public string PostHuy(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            return _repo.PostHuy(tblNhapKho, isdongthung, lcsx, statuschuyen);
        }
        [HttpPost]
        [Route("PostHuySMS")]
        public string PostHuySMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            return _repo.PostHuySMS(tblNhapKho, isdongthung, lcsx, statuschuyen);
        }
        [HttpPost]
        [Route("PostChiTiet")]
        public string PostChiTiet(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, int isTonKho)
        {
            return _repo.PostChiTiet(tblNhapKho, isdongthung, lcsx, statuschuyen, isTonKho);
        }
        [HttpPost]
        [Route("PostChiTietSMS")]
        public string PostChiTietSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, int isTonKho)
        {
            return _repo.PostChiTietSMS(tblNhapKho, isdongthung, lcsx, statuschuyen, isTonKho);
        }
        [HttpPost]
        [Route("PostViTri")]
        public string PostViTri(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            return _repo.PostViTri(tblNhapKho, isdongthung, lcsx, statuschuyen);
        }
        [HttpPost]
        [Route("PostViTriSMS")]
        public string PostViTriSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            return _repo.PostViTriSMS(tblNhapKho, isdongthung, lcsx, statuschuyen);
        }
        [HttpGet]
        [Route("GetMaKho")]
        public DataTable GetMaKho(string madvsx, int lcsx)
        {
            return _repo.GetMaKho(madvsx, lcsx);
        }
        [HttpGet]
        [Route("GetDay")]
        public DataTable GetDay(string madvsx, string makho, string makh)
        {
            return new NhapKhoModel().GetDay(madvsx, makho, makh);
        }
        [HttpGet]
        [Route("GetKe")]
        public DataTable GetKe(string madvsx, string makho, string maday)
        {
            return new NhapKhoModel().GetKe(madvsx, makho, maday);
        }
        [HttpGet]
        [Route("GetTang")]
        public DataTable GetTang(string madvsx, string makho, string maday, string make)
        {
            return new NhapKhoModel().GetTang(madvsx, makho, maday, make);
        }
        [HttpGet]
        [Route("GetO")]
        public DataTable GetO(string madvsx, string makho, string maday, string make, string matang)
        {
            return new NhapKhoModel().GetO(madvsx, makho, maday, make, matang);
        }
        [HttpGet]
        [Route("GetCBM")]
        public DataTable GetO(string madvsx, string makho, string maday, string make, string matang, string mao, string madh, string mapkl, string poid)
        {
            return new NhapKhoModel().GetCBM(madvsx, makho, maday, make, matang, mao, madh, mapkl, poid);
        }
        [HttpPost]
        [Route("GetVTK")]
        public DataTable GetVTK(DataTable data)
        {
            return new NhapKhoModel().GetVTK(data);
        }
        [HttpPost]
        [Route("GetVTKV1")]
        public DataTable GetVTKV1(DataTable data)
        {
            return new NhapKhoModel().GetVTKV1(data);
        }
        [HttpPost]
        [Route("PostVTK")]
        public string PostVTK(DataTable tblNhapKho)
        {
            return new NhapKhoModel().PostVTK(tblNhapKho);
        }
        [HttpPost]
        [Route("GetVTKV2")]
        public DataTable GetVTKV2(DataTable data)
        {
            return new NhapKhoModel().GetVTKV2(data);
        }
        //coding by Dat
        //start
        [HttpGet]
        [Route("GetLenhSX")]
        public DataTable GetLenhSX(string statuschuyen, string username, string status)
        {
            return _repo.GetLenhSX(statuschuyen, username, status);
        }

        [HttpGet]
        [Route("GetLenhSXLuuChuyen")]
        public DataTable GetLenhSXLuuChuyen(string username)
        {
            return _repo.GetLenhSXLuuChuyen(username);
        }

        [HttpGet]
        [Route("GetDS_KHDT")]
        public DataTable GetDS_KHDT(string maplk, string mahang, string poid, string statuschuyen, string madvsx, string isDecat, string dotsx)
        {
            return _repo.GetDS_KHDT(maplk, mahang, poid, statuschuyen, madvsx, isDecat, dotsx);
        }

        [HttpGet]
        [Route("GetDS_KHDTLuanChuyen")]
        public DataTable GetDS_KHDTLuanChuyen(string maplk, string mahang, string poid, string mapklchuyen, string madvsx, string isDecat, string dotsx)
        {
            return _repo.GetDS_KHDTLuanChuyen(maplk, mahang, poid, mapklchuyen, madvsx, isDecat, dotsx);
        }

        [HttpGet]
        [Route("GetDSThung")]
        public DataTable GetDSThung(string maplk, string poid, string mahang, string tuthung, string denthung, string madvsx, string isDeCat, string dotsx)
        {
            return _repo.GetDSThung(maplk, poid, mahang, tuthung, denthung, madvsx, isDeCat, dotsx);
        }

        [HttpGet]
        [Route("GetDSThungLuanChuyen")]
        public DataTable GetDSThungLuanChuyen(string maplk, string poid, string mahang, string tuthung, string denthung, string madvsx, string isDeCat, string dotsx)
        {
            return _repo.GetDSThungLuanChuyen(maplk, poid, mahang, tuthung, denthung, madvsx, isDeCat, dotsx);
        }

        [HttpPost]
        [Route("PostNhapKho")]
        public string PostNhapKho(DataTable tblDongThung)
        {
            return _repo.PostNhapKho(tblDongThung);
        }

        [HttpPost]
        [Route("PostNhapKhoLuuChuyen")]
        public string PostNhapKhoLuuChuyen(string isTonKho, DataTable tblDongThung)
        {
            return _repo.PostNhapKhoLuuChuyen(isTonKho, tblDongThung);
        }

        [HttpGet]
        [Route("Search")]
        public DataTable Search(string malenh, string para)
        {
            return _repo.Search(malenh, para);
        }

        [HttpGet]
        [Route("GetKho")]
        public DataTable GetKho(string madvsx)
        {
            return _repo.GetKho(madvsx);
        }
        [HttpGet]
        [Route("GetNhapKhoWeb")]
        public DataTable GetNhapKhoWeb(string action, string para, string para1, string para2, string para3 = "", string para4 = "", string para5 = "",
                                    string para6 = "", string para7 = "", string para8 = "", string para9 = "")
        {
            return new NhapKhoModel().GetNhapKhoWeb(action, para, para1, para2, para3, para4, para5, para6, para7, para8, para9);
        }
        //end
        [HttpPost]
        [Route("GetKiemTra")]
        public DataTable GetKiemTra(DataTable data)
        {
            return new NhapKhoModel().GetKiemTra(data);
        }
        [HttpPost]
        [Route("GetKiemTraAll")]
        public DataTable GetKiemTraAll(DataTable data)
        {
            return new NhapKhoModel().GetKiemTraAll(data);
        }
        [HttpPost]
        [Route("GetKiemTraViTri")]
        public DataTable GetKiemTraViTri(DataTable data)
        {
            return new NhapKhoModel().GetKiemTraViTri(data);
        }
        [HttpGet]
        [Route("GetDSThungV2")]
        public DataTable GetDSThungV2(string maplk, string poid, string mahang, string tuthung, string isDongThung, string denthung, string isDeCat)
        {
            return new NhapKhoModel().GetDSThungV2(maplk, poid, mahang, tuthung, isDongThung, denthung, isDeCat);
        }
        [HttpGet]
        [Route("GetDate")]
        public string GetDate()
        {
            return Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
        }


        [HttpPost]
        [Route("UpdateDongThungNhapKho")]
        public string UpdateDongThungNhapKho(string action, dynamic data)
        {
            string json = JsonConvert.SerializeObject(data);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            return new NhapKhoModel().UpdateDongThungNhapKho(action, tbl);
        }
    }
}