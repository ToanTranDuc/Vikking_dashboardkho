using NtbSoft.ERP.Model.KeHoach;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/GhepDonHangBo")]
    public class GhepDHBoController : ApiController
    {
        [HttpGet]
        [Route("GET_DH")]
        public DataTable GET_DH()
        {
            return GhepDHBoModel.GET_DH();
        }
        [HttpGet]
        [Route("GET_DH_GHEP")]
        public DataTable GET_DH_GHEP(string maDH)
        {
            return GhepDHBoModel.GET_DH_GHEP(maDH);
        }
        [HttpPost]
        [Route("POST")]
        public string POST(string maDH, DataTable tb)
        {
            return GhepDHBoModel.POST(maDH, tb);
        }
    }
}