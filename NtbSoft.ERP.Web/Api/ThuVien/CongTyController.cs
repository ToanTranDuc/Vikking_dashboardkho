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
    [RoutePrefix("api/CongTy")]
    public class CongTyController : ApiController
    {
        ICongTyRepository _repo = new CongTyRepository();
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return _repo.Get();
        }
       
        [HttpPost]
        [Route("Post")]
        public string Post(List<CongTyEntity> ojCongTy)
        {
            if (ojCongTy == null) return "false";
            string json = JsonConvert.SerializeObject(ojCongTy);
            DataTable tbCongTy= JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tbCongTy);
        }
      
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string parameter)
        {
            return _repo.Delete(parameter);
        }
    }
}