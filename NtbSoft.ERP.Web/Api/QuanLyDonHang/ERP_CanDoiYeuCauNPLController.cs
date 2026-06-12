using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using System.Web.Http;
using NtbSoft.ERP.Model.QuanLyDonHang;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/CanDoiYCNPL")]
    public class ERP_CanDoiYeuCauNPLController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "")
        {
            return new ERP_CanDoiYeuCauNPLModel().Get(action, para1, para2, para3, para4, para5);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(string action, DataTable data)
        {


            return new ERP_CanDoiYeuCauNPLModel().Post(action, data);
        }
    }
}