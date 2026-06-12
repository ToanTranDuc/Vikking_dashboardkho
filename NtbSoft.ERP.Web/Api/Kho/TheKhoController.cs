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

namespace NtbSoft.ERP.Web.Api.SoTheoDoi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/TheKho")]
    public class TheKhoController: ApiController
    {
        ITheKhoRepository _repo = new TheKhoRepository();
        [HttpGet]
        [Route("GetDSTheKho")]
        public DataTable GetDSTheKho()
        {
            return _repo.GetDSTheKho();
        }
        [HttpPost]
        [Route("Post")]
        public string Post(List<TheKhoEntity> objTheKho)
        {
            if (objTheKho == null) return "false";
            string json = JsonConvert.SerializeObject(objTheKho);
            DataTable tblTheKho = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tblTheKho);
        }

        [HttpDelete]
        [Route("Delete")]
        public string Delete(int parameter)
        {
            return _repo.Delete(parameter);
        }
    }
}