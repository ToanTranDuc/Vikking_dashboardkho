using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data;
using NtbSoft.ERP.Model.Kho;

namespace NtbSoft.ERP.Web.Api.Kho
{
    [RoutePrefix("api/ERPBaoCaoXuatNhapTon")]
    public class ERPBaoCaoXuatNhapTonController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "",string para7="", string para8="", string para9="", string para10="", string para11="")
        {
            return new ERPBaoCaoXuatNhapTonModel().Get(action, para, para2, para3, para4, para5, para6, para7,para8,para9,para10,para11);
        }
       
        //[HttpPost]
        //[Route("Post")]
        //public string Post(string action, [FromBody] DataTable tbl, string para = "", string para2 = "", string para3 = "", string para4 = "")
        //{
        //    return new ERPNhapKhoNPLModel().Post(action, para, para2, para3, para4,tbl);
        //}
        //[HttpPost]
        //[Route("PostNK")]
        //public string PostNK(string action, [FromBody] DataTable tbl, string para = "", string para2 = "", string para3 = "", string para4 = "")
        //{
        //    return new ERPNhapKhoNPLModel().PostNK(action, para, para2, para3, para4, tbl);
        //}
        //[HttpDelete]
        //[Route("Delete")]
        //public string Delete(string action, string para = "", string para2 = "", string para3 = "", string para4 = "")
        //{
        //    return new ERPNhapKhoNPLModel().Delete(action, para, para2, para3, para4);
        //}
    }
}