using NtbSoft.ERP.Model.ThuVien;
using NtbSoft.ERP.Web.Repository.R.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/ErpKHDongThung_PCB")]
    public class ErpKHDongThung_PCBController : ApiController
    {   
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action,string styleID)
        {
            return new ErpKHDongThung_PCBModel().Get(action,styleID);
        }        
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable dt)
        {
            return new ErpKHDongThung_PCBModel().Post("Post", dt); ;
        }
    }
}