using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/CanDoiDinhMucNPLLog")]
    public class CanDoiDinhMucNPLLogController : ApiController
    {

        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para = "", string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new CanDoiDinhMucNPL_LogModel().Get(action, para, para1, para2, para3, para4, para5, para6, para7,para8,para9,para10);
        }
        [HttpGet]
        [Route("GhiLogLenh")]
        public DataTable GhiLogLenh(string action, string para = "", string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new CanDoiDinhMucNPL_LogModel().GhiLogLenh(action, para, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action, [FromBody] DataTable tbl, string para = "", string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new CanDoiDinhMucNPL_LogModel().Post(action, para, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10, tbl);
        }
    }
}