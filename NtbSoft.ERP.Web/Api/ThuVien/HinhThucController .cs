using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Data;
using NtbSoft.ERP.Web.Repository.R.ThuVien;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Model.ThuVien;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/HinhThuc")]
    public class HinhThucController : ApiController
    {
        HinhThucModel _model = new HinhThucModel();
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return _model.Get();
        }
        [HttpPost]
        [Route("Post")]
        public string PostDonVi(DataTable tbl)
        {
            return _model.Post(tbl);
        }
        [HttpDelete]
        [Route("Delete")]
        public string DeleteDonVi(int parameter)
        {
            return _model.Delete(parameter);
        }
    }
}