using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/baocao")]
    public class BCCapThem_ThuHoi_NPL_ApiController : ApiController
    {
        [HttpGet]
        [Route("get")]
        public DataTable Get(string action, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null)
        {
            return new BCCapThem_ThuHoi_NPL().Get(action, param1, param2, param3, param4, param5);
        }
    }
}
