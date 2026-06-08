using Newtonsoft.Json;
using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/TongQuanCapPhat")]
    public class Erp_TongQuanCapPhatController : ApiController
    {
        private readonly Erp_TongQuanCapPhatModel _model = new Erp_TongQuanCapPhatModel();
        [HttpGet]
        [Route("GET")]
        public DataTable GET(string action,string para1 = null,string para2 = null,string para3 = null,string para4 = null,string para5 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";

            return _model.Get(action, para1, para2, para3, para4, para5);
        }

    }
}