using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SoTheoDoi;
using NtbSoft.ERP.Web.Repository.R.SoTheoDoi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/SoTheoDoiSeal")]
    public class SoTheoDoiController : ApiController
    {
        ISoTheoDoiSealNoiBoRepository _repo = new SoTheoDoiSealNoiBoRepository();
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return _repo.Get();
        }
        [HttpPost]
        [Route("Post")]
        public string Post(List<SoTheoDoiSealNoiBoEntity> ojSoTheoDoiSeal)
        {
            if (ojSoTheoDoiSeal == null) return "false";
            string json = JsonConvert.SerializeObject(ojSoTheoDoiSeal);
            DataTable tbSoTheoDoiSeal = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tbSoTheoDoiSeal);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(int parameter)
        {
            return _repo.Delete(parameter);
        }
    }
}