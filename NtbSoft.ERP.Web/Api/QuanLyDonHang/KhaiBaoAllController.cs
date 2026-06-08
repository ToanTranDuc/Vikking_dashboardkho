using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/KhaiBaoAll")]
    public class KhaiBaoAllController : ApiController
    {       
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action,string para1 = "",string para2="",string para3 = "",string para4="",string para5="")
        {
            return new KhaiBaoAllModel().Get(action, para1, para2, para3, para4, para5);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(string action,string para1,string para2,string para3,string para4, DataTable tbl)
        {
            return new KhaiBaoAllModel().Post(action, para1, para2, para3, para4, tbl);
        }

    }
}