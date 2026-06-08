using Newtonsoft.Json;
using NtbSoft.ERP.Entity.XuLyVT;
using NtbSoft.ERP.Model.XuLyVT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.XuLyVT
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/XLVTVT")]
    public class XLVTVTController : ApiController
    {

        [HttpPost]
        [Route("Get")]
        public DataTable Get(XuLyVTRequestGet req)

        {
            return new XLVTVTModel().Get(req);
        }
        [HttpPost]
        [Route("GetByTypeTable")]
        public DataTable GetByTypeTable(XuLyVTRequestPost req)

        {
            return new XLVTVTModel().GetByTypeTable(req);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(XuLyVTRequestPost req)

        {
            return new XLVTVTModel().Post(req);
        }
    }
}