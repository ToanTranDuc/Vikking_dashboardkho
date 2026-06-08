using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
using NtbSoft.ERP.Model.POMuaHang;
using NtbSoft.ERP.Model.ThuVien;
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
    [RoutePrefix("api/VatTuMinMax")]
    public class VatTuMinMaxController : ApiController
    {

        [HttpPost]
        [Route("Get")]
        public DataTable Get(XuLyVTRequestGet req)

        {
            return new VatTuMinMaxModel().Get(req);
        }
        [HttpPost]
        [Route("GetByTypeTable")]
        public DataTable GetByTypeTable(XuLyVTRequestPost req)

        {
            return new VatTuMinMaxModel().GetByTypeTable(req);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(XuLyVTRequestPost req)

        {
            return new VatTuMinMaxModel().Post(req);
        }
    }
}