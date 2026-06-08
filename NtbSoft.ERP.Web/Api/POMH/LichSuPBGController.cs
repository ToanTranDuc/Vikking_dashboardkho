using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Model.POMuaHang;
using NtbSoft.ERP.Model.ThuVien;
using NtbSoft.ERP.Web.Repository.R.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.POMuaHang
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/LichSuPBG")]
    public class LichSuPBGController : ApiController
    {
        LichSuPhieuBGModel _model = new LichSuPhieuBGModel();
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para1, string para2, string para3, string para4, string para5)
        {
            return _model.Get(action, para1, para2, para3, para4, para5);
        }

        [HttpGet]
        [Route("GetNCP")]
        public DataTable GetNCP(string action, string para1, string para2, string para3, string para4, string para5)
        {
            return _model.GetNCP(action, para1, para2, para3, para4, para5);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(DataTable tbl)
        {
            return _model.Post(tbl);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Deleteloai(string para1)
        {
            return _model.Delete(para1);
        }

    }
}