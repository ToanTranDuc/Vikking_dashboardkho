using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/KhoiTaoBOMV1")]
    public class KhoiTaoBOMV1Controller : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                            string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new KhoiTaoBOMV1Model().Get(action, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpPost]
        [Route("Post1")]
        public string Post1(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                            string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new KhoiTaoBOMV1Model().PostT1(action, tbl, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpPost]
        [Route("Post2")]
        public string Post2(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                          string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new KhoiTaoBOMV1Model().PostT2(action, tbl, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpPost]
        [Route("Post3")]
        public string Post3(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                          string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new KhoiTaoBOMV1Model().PostT3(action, tbl, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpPost]
        [Route("Post4")]
        public string Post4(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                           string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new KhoiTaoBOMV1Model().PostT4(action, tbl, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpPost]
        [Route("Post5")]
        public string Post5(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                          string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new KhoiTaoBOMV1Model().PostT5(action, tbl, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpPost]
        [Route("Post6")]
        public string Post6(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                          string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new KhoiTaoBOMV1Model().PostT6(action, tbl, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpPost]
        [Route("GetSD")]
        public DataTable GetSD(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                          string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new KhoiTaoBOMV1Model().GetSD(action, tbl, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpGet]
        [Route("XetDuyetDong")]
        public string XetDuyetDong(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                            string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new KhoiTaoBOMV1Model().XetDuyetDong(action, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                           string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new KhoiTaoBOMV1Model().Delete(action, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
    }
}