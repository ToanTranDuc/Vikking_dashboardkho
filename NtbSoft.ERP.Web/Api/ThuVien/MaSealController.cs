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
    [RoutePrefix("api/MaSeal")]
    public class MaSealController : ApiController
    {
        IMaSealRepository _repo = new MaSealRepository();
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string username)
        {
            return _repo.Get(username);
        }

        [HttpGet]
        [Route("GetKho")]
        public DataTable GetKho(string username)
        {
            return _repo.GetKho(username);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(string Action,List<MaSealEntity> ojMaSeal)
        {
            if (ojMaSeal == null) return "false";
            string json = JsonConvert.SerializeObject(ojMaSeal);
            DataTable tbMaSeal = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(Action,tbMaSeal);
        }

        [HttpDelete]
        [Route("Delete")]
        public string Delete(string parameter)
        {
            return _repo.Delete(parameter);
        }

        [HttpGet]
        [Route("GetPKLMaSeal")]
        public DataTable Get(string action,string MaSeal)
        {
            return _repo.Get( action,MaSeal);
        }


        [HttpPost]
        [Route("checkDuplicateSeal")]
        public DataTable checkDuplicateSeal(List<MaSealEntity> ojMaSeal)
        {
            if (ojMaSeal == null) return new DataTable();
            string json = JsonConvert.SerializeObject(ojMaSeal);
            DataTable tbMaSeal = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.checkDuplicateSeal(tbMaSeal);
        }
    }
}