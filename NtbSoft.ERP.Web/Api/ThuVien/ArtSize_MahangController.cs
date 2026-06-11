using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/ArtSize_Mahang")]
    public class ArtSize_MahangController : ApiController
    {       
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action,string para1)
        {
            return new ArtSize_MaHangModel().Get(action,para1);
        }

        [HttpPost]
        [Route("Post")]
        public string Insert(string action,string para1,DataTable items)
        {
            return new ArtSize_MaHangModel().Post(action,para1,items);
        }

        [HttpDelete]
        [Route("Delete")]
        public string Delete(string action, string Para1)
        {
            return new ArtSize_MaHangModel().Delete(action, Para1);
        }

    }
}