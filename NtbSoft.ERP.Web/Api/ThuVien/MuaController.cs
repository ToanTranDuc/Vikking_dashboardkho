using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Model.ThuVien;
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
    [RoutePrefix("api/Mua")]
    public class MuaController : ApiController
    {
       
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return new MuaModel().Get();
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable tbl)
        {           
            return new MuaModel().Post(tbl);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(int parameter)
        {
            return new MuaModel().Delete(parameter);
        }
    }
}