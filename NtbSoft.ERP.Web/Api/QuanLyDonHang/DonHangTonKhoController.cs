using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Model.CanDoiDonHang;
using NtbSoft.ERP.Web.Repository.R.DonHangTonKho;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.CanDoiDonHang
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/DonHangTonKho")]
    public class DonHangTonKhoController:ApiController
    {
        IDonHangTonKhoRepository _repo = new DonHangTonKhoRepository();
        

        [HttpGet]
        [Route("GetDonHangTonKho")]
        public DataTable GetDonHangTonKho()
        {
            return _repo.GetDonHangTonKho();
        }

        [HttpGet]
        [Route("GetDSDonHangTonKho")]
        public DataTable GetDSDonHangTonKho(string maDH,string maHang)
        {
            return _repo.GetDSDonHangTonKho(maDH,maHang);
        }

        [HttpGet]
        [Route("GetTonKhoChiTiet")]
        public DataTable GetTonKhoChiTiet(string madh,string maHang)
        {
            return _repo.GetTonKhoChiTiet(madh,maHang);
        }

        [HttpGet]
        [Route("GetMaxDH")]
        public DataTable GetMaxDH()
        {
            return _repo.GetMaxDH();
        }
        [HttpGet]
        [Route("GetPivotSLKH_SLSX")]
        public DataTable GetPivotSLKH_SLSX(string maDH)
        {
            return _repo.GetPivotSLKH_SLSX(maDH);
        }

        [HttpGet]
        [Route("GetPivotSLKho")]
        public DataTable GetPivotSLKho(string maHang)
        {
            return _repo.GetPivotSLKho(maHang);
        }

        [HttpPost]
        [Route("InsertDataTruTonKho")]
        public string InsertDataTruTonKho(DataTable tblTruTonKho)
        {
            return _repo.InsertDataTruTonKho(tblTruTonKho);
        }

        [HttpPost]
        [Route("UpdateSLTonKho")]
        public string UpdateSLTonKho(DataTable tblTonKho, string nguoitao)
        {
            return _repo.UpdateSLTonKho(tblTonKho, nguoitao);
        }

        [HttpGet]
        [Route("GetLichSuTruTonKho")]
        public DataTable GetLichSuTruTonKho(string maHang)
        {
            return _repo.GetLichSuTruTonKho(maHang);
        }

        [HttpGet]
        [Route("GetBangMau")]
        public DataTable GetBangMau(string mahang)
        {
            return _repo.GetBangMau(mahang);
        }
        [HttpGet]
        [Route("GetBangSize")]
        public DataTable GetBangSize(string mahang)
        {
            return _repo.GetBangSize(mahang);
        }
        [HttpPost]
        [Route("PostNhapTonKho")]
        public string PostNhapTonKho(string nguoiTao, object ojNhapTonKho)
        {
            if (ojNhapTonKho == null) return "false";
            string json = JsonConvert.SerializeObject(ojNhapTonKho);
             DataTable dtbNhapTonKho = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostNhapTonKho(dtbNhapTonKho, nguoiTao);
        }
        [HttpGet]
        [Route("GetTonKho")]
        public DataTable GetTonKho(int pageIndex, int pageSize)
        {
            return _repo.GetTonKho(pageIndex,pageSize);
        }
        [HttpGet]
        [Route("GetKiemTra")]
        public DataTable GetKiemTra(string mahang, string poid, string mamau, string dausizeid, string sizeid)
        {
            return _repo.GetKiemTra(mahang, poid, mamau, dausizeid, sizeid);
        }
        [HttpDelete]
        [Route("DeleteTonKho")]
        public string DeleteTonKho(int parameter)
        {
            return _repo.DeleteTonKho(parameter);
        }
        [HttpPost]
        [Route("AddDonHangTonKho")]
        public string AddDonHangTonKho(string nguoiTao, DataTable tblTonKho)
        {
            return _repo.AddDonHangTonKho(nguoiTao,tblTonKho);
        }
        [HttpGet]
        [Route("GetKHDTTonKho")]
        public DataTable GetKHDTTonKho()
        {
            return _repo.GetKHDTTonKho();
        }

        [HttpGet]
        [Route("GetMHCount")]
        public DataTable GetMHCount(string mahang)
        {
            return new DonHangTonKhoModel().GetMHCount(mahang);
        }
        [HttpDelete]
        [Route("DeleteDongDHTonKho")]
        public string DeleteDongDHTonKho(string para, string para1, string para2, string para3)
        {
            return new DonHangTonKhoModel().DeleteDongDHTonKho(para, para1, para2, para3);
        }

        [HttpGet]
        [Route("GetDongThung")]
        public DataTable GetDongThung(string para, string para1, string para2)
        {
            return new DonHangTonKhoModel().GetKHDongThung(para, para1, para2);
        }
    }
}