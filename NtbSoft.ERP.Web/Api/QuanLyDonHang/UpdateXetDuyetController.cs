using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Model.ThuVien;
using NtbSoft.ERP.Web.Repository.R.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/UpdateXetDuyet")]
    public class UpdateXetDuyetController : ApiController
    {
   
        [HttpGet]
        [Route("Get")]
        public DataTable GetBangMau(string action, string para = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "")
        {
            return new UpdateXetDuyetModel().Get(action, para, para2, para3, para4, para5, para6);
        }
        [HttpGet]
        [Route("UpdateXetDuyet")]
        public string UpdateXetDuyet(string action, string para = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "")
        {
            return new UpdateXetDuyetModel().UpdateXetDuyet(action, para, para2, para3, para4, para5, para6);
        }
        [HttpPost]
        [Route("PostCopy")]
        public string PostCopy(string action, string para = "", string para2 = "", string para3 = "", string para4 = "")
        {
            return new UpdateXetDuyetModel().PostCopy(action, para, para2, para3, para4);
        }
        [HttpDelete]
        [Route("XoaDot")]
        public string XoaDot(string action, string para = "", string para2 = "", string para3 = "",string para4="", string para5 = "")
        {
            return new UpdateXetDuyetModel().XoaDot(action, para, para2, para3,para4, para5);
        }
        [HttpGet]
        [Route("GetSS")]
        public DataTable GetBangGetSSMau(string action, string para = "", string para2 = "")
        {
            return new UpdateXetDuyetModel().GetSS(action, para, para2);
        }
    }
}