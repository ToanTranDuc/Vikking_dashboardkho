using NtbSoft.ERP.Web.Repository.R.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/CongDoan")]
    public class CongDoanController: ApiController
    {
        ICongDoanRepository _repo = new CongDoanRepository();
        [HttpGet]
        [Route("GetCongDoan")]
        public DataTable GetCongDoan()
        {
            return _repo.GetCongDoan();
        }
        [HttpPost]
        [Route("PostCongDoan")]
        public string Post(DataTable tblCongDoan)
        {
            if (tblCongDoan == null) return "false";
            return _repo.Post(tblCongDoan);
        }
        [HttpGet]
        [Route("DeleteCongDoan")]
        public string Delete(int id)
        {
            return _repo.Delete(id);
        }
        [HttpGet]
        [Route("GetPivotCongDoan")]
        public DataTable GetPivotCongDoan()
        {
            return _repo.GetPivotCongDoan();
        }
        [HttpPost]
        [Route("PostChiTietCongDoan")]
        public string PostChiTietCongDoan(DataTable tblChiTietCongDoan)
        {
            if (tblChiTietCongDoan == null) return "false";
            return _repo.PostChitietCongDoan(tblChiTietCongDoan);
        }
        [HttpGet]
        [Route("GetCongDoanAllowCondition")]
        public DataTable GetCongDoanAllowCondition(string maDH, string maLenhSX)
        {
            return _repo.GetCongDoanAllowCondition(maDH,maLenhSX);
        }
    }
}