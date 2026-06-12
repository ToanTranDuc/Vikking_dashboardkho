using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/QuyDoi")]
    public class QuyDoiController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return new QuyDoiModel().Get();
        }
        [HttpGet]
        [Route("GetQD")]
        public DataTable GetQD()
        {
            return new QuyDoiModel().GetQD();
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable tbl)
        {
            return new QuyDoiModel().Post(tbl);
        }
    }
}