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
    [RoutePrefix("api/KHDT_DaKieuLap")]
    public class KHDT_DaKieuLapController : ApiController
    {        
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string Action, string Para1,string Para2)
        {
            return new KHDT_DaKieuLapModel().Get(Action, Para1, Para2);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(DataTable objSave)
        {
            if (objSave == null) return "false";            
            return new KHDT_DaKieuLapModel().Post(objSave);
        }

        [HttpDelete]
        [Route("Delete")]
        public string Delete(string Para1, string Para2, string Para3)
        {
            return new KHDT_DaKieuLapModel().Delete(Para1, Para2,Para3);
        }
    }
}