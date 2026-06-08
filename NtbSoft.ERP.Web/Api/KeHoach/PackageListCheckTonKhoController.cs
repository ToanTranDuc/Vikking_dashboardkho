using NtbSoft.ERP.Model.KeHoach;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [RoutePrefix("api/CheckTonKho")]
    public class PackageListCheckTonKhoController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1, string Para2)
        {
            return new PackageListCheckTonKhoModel().Get(action, Para1, Para2);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action, DataSet ds)
        {
            return new PackageListCheckTonKhoModel().Post(action, ds.Tables[0], ds.Tables[1]);
        }
        [HttpPost]
        [Route("Delete")]
        public string Delete(string action, string Para, DataSet ds)
        {
            return new PackageListCheckTonKhoModel().Delete(action, Para, ds.Tables[0]);
        }
    }
}