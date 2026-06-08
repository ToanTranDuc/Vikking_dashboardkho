using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SoTheoDoi;
using NtbSoft.ERP.Model.SoTheoDoi;
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
    [RoutePrefix("api/SoTheoDoiContainer")]
    public class SoTheoDoiContainerController: ApiController
    {
        ISoTheoDoiContainerRepository _repo = new SoTheoDoiContainerRepository();
        [HttpGet]
        [Route("GetDSSoTheoDoiContainer")]
        public DataTable GetDSSoTheoDoiContainer(string action,string tungay,string denngay,string madvsx)

        {
            return new SoTheoDoiContainerModel().GetDSSoTheoDoiContainer(action, tungay, denngay, madvsx);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(List<SoTheoDoiContainerEntity> ojBangSize)
        {
            if (ojBangSize == null) return "false";
            string json = JsonConvert.SerializeObject(ojBangSize);
            DataTable tbBangSize = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tbBangSize);
        }

        [HttpDelete]
        [Route("DeleteDSSoTheoDoiContainer")]
        public string DeleteDSSoTheoDoiContainer(int parameter)
        {
            return _repo.DeleteSoTheoDoiContainer(parameter);
        }
    }
}