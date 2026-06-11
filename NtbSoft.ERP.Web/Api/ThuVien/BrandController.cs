using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Model.ThuVien;
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
    [RoutePrefix("api/Brand")]
    public class BrandController : ApiController
    {
        BrandModel _model = new BrandModel();
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action = "",string para = "", string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "")
        {
            return _model.Get(action,para, para1, para2, para3, para4, para5);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(List<BrandEntity> ojB)
        {
            if (ojB == null) return "false";
            string json = JsonConvert.SerializeObject(ojB);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return _model.Post(tbl);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string parameter)
        {
            return _model.Delete(parameter);
        }
    }
}