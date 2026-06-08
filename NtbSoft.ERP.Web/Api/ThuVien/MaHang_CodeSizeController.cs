using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
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
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/MaHang_CodeSize")]
    public class MaHang_CodeSizeController : ApiController
    {

        IMaHangRepository _repo = new MaHang_CodeSizeRepository();
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string Action,string Para1,string Para2)
        {
            return _repo.Get(Action,Para1,Para2);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(List<MaHang_CodeSizeEntity> objMaHang)
        {
            if (objMaHang == null) return "false";
            string json = JsonConvert.SerializeObject(objMaHang);
            DataTable tb = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tb);
        }

        [HttpDelete]
        [Route("Delete")]
        public string Delete(string Para1,string Para2, string Para3)
        {
            return _repo.Delete(Para1,Para2, Para3);
        }
    }
}