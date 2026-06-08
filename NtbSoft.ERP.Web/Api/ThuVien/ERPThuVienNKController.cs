using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data;
using NtbSoft.ERP.Model.ThuVien;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/ERPThuVienNK")]
    public class ERPThuVienNKController:ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "")
        {
            return new ERPThuVienNKModel().Get(action, para, para2, para3, para4, para5, para6, para7);
        }

        [HttpPost]
        [Route("PostKH")]
        public string PostKH(string action, [FromBody] DataTable tbl, string para = "", string para2 = "", string para3 = "", string para4 = "")
        {
            return new ERPThuVienNKModel().PostKH(action, para, para2, para3, para4, tbl);
        }
        [HttpPost]
        [Route("PostCang")]
        public string PostCang(string action, [FromBody] DataTable tbl, string para = "", string para2 = "", string para3 = "", string para4 = "")
        {
            return new ERPThuVienNKModel().PostCang(action, para, para2, para3, para4, tbl);
        }
        [HttpPost]
        [Route("PostTau")]
        public string PostTau(string action, [FromBody] DataTable tbl, string para = "", string para2 = "", string para3 = "", string para4 = "")
        {
            return new ERPThuVienNKModel().PostTau(action, para, para2, para3, para4, tbl);
        }

        [HttpDelete]
        [Route("Delete")]
        public string Delete(string action, string para = "", string para2 = "", string para3 = "", string para4 = "")
        {
            return new ERPThuVienNKModel().Delete(action, para, para2, para3, para4);
        }
    }
}