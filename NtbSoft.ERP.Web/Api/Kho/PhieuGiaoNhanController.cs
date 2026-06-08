using NtbSoft.ERP.Model.KeHoach;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [RoutePrefix("api/PhieuGiaoNhan")]
    public class PhieuGiaoNhanController: ApiController
    {
        PhieuGiaoNhanModel _phieuGiaoNhanModel = new PhieuGiaoNhanModel();

        [HttpGet]
        [Route("GetPGN")]
        public DataTable Get(DateTime tuNgay, DateTime denNgay)
        {
            return _phieuGiaoNhanModel.GetPhieuGiaoNhan(tuNgay, denNgay);
        }


        [HttpPost]
        [Route("GetChiTietPGN")]
        public DataTable GetChiTietPGN(DataTable tblPGN)
        {
            return _phieuGiaoNhanModel.GetChiTietPhieuGiaoNhan(tblPGN);
        }

        [HttpPost]
        [Route("GetChiTietPGNExcel")]
        public DataSet GetChiTietPGNExcel(DataTable tblPGN)
        {
            return _phieuGiaoNhanModel.GetChiTietPhieuGiaoNhanExcel(tblPGN);
        }

        [HttpPost]
        [Route("UpdateIsXacNhan")]
        public string UpdateIsXacNhan(DataTable tblPGN)
        {
            return _phieuGiaoNhanModel.UpdateIsXacNhan(tblPGN);
        }

        [HttpPost]
        [Route("HuyIsXacNhan")]
        public string HuyIsXacNhan(string nVien,DataTable tblPGN)
        {
            return _phieuGiaoNhanModel.HuyIsXacNhan(nVien,tblPGN);
        }
        [HttpPost]
        [Route("GetReportPGN")]
        public DataSet GetReportPGN(DataTable tblPGN)
        {
            return _phieuGiaoNhanModel.GetReportPGN(tblPGN);
        }
        [HttpGet]
        [Route("GetDSDonHangNhapKho")]
        public DataTable GetDSDonHangNhapKho()
        {
            return _phieuGiaoNhanModel.GetDSDonHangNhapKho();
        }
        [HttpGet]
        [Route("GetPGNDonHang")]
        public DataTable GetPGNDonHang(string maDH)
        {
            return _phieuGiaoNhanModel.GetPhieuGiaoNhanDonHang(maDH);
        }
        [HttpPost]
        [Route("UpdateNgayNhapKho")]
        public string UpdateNgayNhapKho(DataTable tblPGN)
        {
            return _phieuGiaoNhanModel.UpdateNgayNhapKho(tblPGN);
        }
    }
}