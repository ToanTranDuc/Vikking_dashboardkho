using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data;
using NtbSoft.ERP.Model.QuanLyDonHang;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/XinNPLMayMau")]
    public class XinNPLMayMauController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new XinNPLMayMauModel().Get(action, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }


        [HttpPost]
        [Route("Post")]
        public string Post(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new XinNPLMayMauModel().Post(action, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10, tbl);
        }
        [HttpPost]
        [Route("PostT2")]
        public string PostT2(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new XinNPLMayMauModel().PostT2(action, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10, tbl);
        }
        [HttpPost]
        [Route("PostT3")]
        public string PostT3(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new XinNPLMayMauModel().PostT3(action, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10, tbl);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new XinNPLMayMauModel().Delete(action, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
    }
}