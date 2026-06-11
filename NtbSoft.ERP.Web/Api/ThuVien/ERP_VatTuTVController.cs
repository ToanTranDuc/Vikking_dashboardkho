using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data;
using NtbSoft.ERP.Model.ThuVien;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/ERPVatTuTV")]
    public class ERP_VatTuTVController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "")
        {
            return new ERP_VatTuTVModel().Get(action, para, para2, para3, para4, para5, para6, para7);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(string action, [FromBody] DataTable tbl, string para = "", string para2 = "", string para3 = "", string para4 = "")
        {
            return new ERP_VatTuTVModel().Post(action, para, para2, para3, para4, tbl);
        }
     

        [HttpDelete]
        [Route("Delete")]
        public string Delete(string action, string para = "", string para2 = "", string para3 = "", string para4 = "")
        {
            return new ERP_VatTuTVModel().Delete(action, para, para2, para3, para4);
        }
    }
}