using NtbSoft.ERP.Model.ThuVien;
using NtbSoft.ERP.Web.Repository.R.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/GetTenNV")] 
    public class GetTenNVController : ApiController
    {
        IGetTenNVRepository _repo = new GetTenNVRepository();

        [HttpGet]
        [Route("GetTenNV")] 
        public DataTable GetTenNV()
        {
            return _repo.GetTenNV();
        }
    }
}