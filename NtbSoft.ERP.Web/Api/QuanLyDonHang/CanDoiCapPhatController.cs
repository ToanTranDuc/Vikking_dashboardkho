using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Model.QuanLyDonHang;
using NtbSoft.ERP.Model.ThuVien;
using NtbSoft.ERP.Web.Repository.R.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/CanDoiCapPhat")]
    public class CanDoiCapPhatController : ApiController
    {
   
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "",string para7="")
        {
            return new CanDoiCapPhatModel().Get(action, para, para2, para3, para4, para5, para6, para7);
        }
      
        //[HttpPost]
        //[Route("PostCopy")]
        //public string PostCopy(string action, string para = "", string para2 = "", string para3 = "", string para4 = "")
        //{
        //    return new UpdateXetDuyetModel().PostCopy(action, para, para2, para3, para4);
        //}
      
    }
}