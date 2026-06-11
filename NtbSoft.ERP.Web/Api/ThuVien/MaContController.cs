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
    [RoutePrefix("api/MaCont")]
    public class MaContController : ApiController
    {

        IMaContRepository _repo = new MaContRepository();
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string Action)
        {
            return _repo.Get(Action);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(List<MaContEntity> ojMaCont)
        {
            if (ojMaCont == null) return "false";
            string json = JsonConvert.SerializeObject(ojMaCont);
            DataTable tbMaCont = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tbMaCont);
        }

        [HttpDelete]
        [Route("Delete")]
        public string Delete(string parameter)
        {
            return _repo.Delete(parameter);
        }
    }
}