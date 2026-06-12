using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/DicQCDongThung")]
    public class DicQCDongThungController : ApiController
    {        
        public DicQCDongThungController()
        {
           
        }
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action)
        {
            return new DicQCDongThungModel().Get(action);
        }

        [HttpPost]
        [Route("Insert")]
        public string Insert(DataTable items)
        {
            return new DicQCDongThungModel().Insert(items);
        }
        [HttpPost]
        [Route("Update")]
        public string Update(DataTable items)
        {
            return new DicQCDongThungModel().Update(items);
        }

        [HttpDelete]
        [Route("{maCongDoan}")]
        public string Delete(string maQuiCach)
        {
            Console.WriteLine("/Delete");
            return new DicQCDongThungModel().Delete(maQuiCach);
        }
        [HttpGet]
        [Route("GetPKL")]
        public DataTable GetPKL(string action, string para1)
        {
            return new DicQCDongThungModel().GetPKH(action, para1);
        }
        [HttpPost]
        [Route("PostPKH")]
        public string PostPKH(string action, DataTable dt)
        {
            return new DicQCDongThungModel().PostPKH(action, dt);
        }
    }
}