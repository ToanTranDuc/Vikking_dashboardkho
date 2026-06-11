using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/CaiDatThongSoKHDT")]
    public class CaiDatThongSoKHDTController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1, string Para2="", string Para3="", string Para4="", string Para5 = "")
        {
            return new CaiDatThongSoKHDTModel().Get(action, Para1, Para2, Para3, Para4, Para5);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action,DataTable dt)
        {
            return new CaiDatThongSoKHDTModel().Post(action,dt);
        }
        [HttpPost]
        [Route("PostQC")]
        public string PostQC(DataTable dt)
        {
            return new CaiDatThongSoKHDTModel().PostQC(dt);
        }

        [HttpPost]
        [Route("DeleteQC")]
        public string DeleteQC(DataTable dt)
        {
            return new CaiDatThongSoKHDTModel().DeleteQC(dt);
        }
        [HttpPost]
        [Route("PostNoiDen")]
        public string PostNoiDen(DataTable dt)
        {
            return new CaiDatThongSoKHDTModel().PostNoiDen(dt);
        }
        [HttpDelete]
        [Route("DeleteNoiDen")]
        public string DeleteNoiDen(string Para1, string Para2)
        {
            return new CaiDatThongSoKHDTModel().DeleteNoiDen(Para1, Para2);
        }
    }
}