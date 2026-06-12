using NtbSoft.ERP.Model.Kho;
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
    [RoutePrefix("api/PhieuNhapKho")]
    public class PhieuNhapKhoV1Controller : ApiController
    {
        [HttpGet]
        [Route("GetSoLo")]
        public DataTable GetSoLo()
        {
            return new PhieuNhapKhoV1Model().GetSoLo();
        }
        [HttpGet]
        [Route("GetSoLoLS")]
        public DataTable GetSoLoLS()
        {
            return new PhieuNhapKhoV1Model().GetSoLoLS();
        }
        [HttpGet]
        [Route("GetSoKien")]
        public DataTable GetSoKien(string soloid, string manpl)
        {
            return new PhieuNhapKhoV1Model().GetSoKien(soloid, manpl);
        }
        [HttpGet]
        [Route("GetCayVai")]
        public DataTable GetCayVai(string soloid)
        {
            return new PhieuNhapKhoV1Model().GetCayVai(soloid);
        }
        [HttpGet]
        [Route("GetDanhSach")]
        public DataTable GetDanhSach(string soloid, string manpl)
        {
            return new PhieuNhapKhoV1Model().GetDanhSach(soloid, manpl);
        }
        [HttpGet]
        [Route("GetThongTinSoLo")]
        public DataTable GetThongTinSoLo(string soloid)
        {
            return new PhieuNhapKhoV1Model().GetThongTinSoLo(soloid);
        }
        [HttpGet]
        [Route("GetMaxSTT")]
        public DataTable GetMaxSTT()
        {
            return new PhieuNhapKhoV1Model().GetMaxSTT();
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable tbl)
        {
            return new PhieuNhapKhoV1Model().Post(tbl);
        }
        [HttpPost]
        [Route("PostIsNK")]
        public string PostIsNK(DataTable tbl)
        {
            return new PhieuNhapKhoV1Model().PostIsNK(tbl);
        }

        [HttpGet]
        [Route("GetLichSu")]
        public DataTable GetLichSu(string fromDate, string toDate)
        {
            return new PhieuNhapKhoV1Model().GetLichSu(fromDate, toDate);
        }
        [HttpGet]
        [Route("GetChiTietPhieu")]
        public DataTable GetChiTietPhieu(string sophieu)
        {
            return new PhieuNhapKhoV1Model().GetChiTietPhieu(sophieu);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string sophieu)
        {
            return new PhieuNhapKhoV1Model().Delete(sophieu);
        }
        [HttpGet]
        [Route("GetMV1")]
        public DataTable Get(string action, string para = "", string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "")
        {
            return new PhieuNhapKhoV1Model().GetMV1(action, para, para1, para2, para3, para4, para5);
        }
        [HttpPost]
        [Route("PostIsNKSua")]
        public string PostIsNKSua(DataTable tbl)
        {
            return new PhieuNhapKhoV1Model().PostIsNKSua(tbl);
        }
    }
}