using Newtonsoft.Json;
using NtbSoft.ERP.Model.KeHoach;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [RoutePrefix("api/ScanBarcodeDT_NK")]
    public class ScanBarcodeDT_NKController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataSet Get(string Action, string Para1="",string Para2="",string Para3="")
        {
            return new ScanBarCodeDT_NKModel().Get(Action, Para1, Para2, Para3);
        }
        [HttpPost]
        [Route("Post")]
        public int Post(string action, DataTable dt)
        {
            return new ScanBarCodeDT_NKModel().Post(action, dt);
        }
        [HttpPost]
        [Route("PostWeb")]
        public int PostWeb(string action, dynamic data)
        {
            string json = JsonConvert.SerializeObject(data);

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            return new ScanBarCodeDT_NKModel().Post(action, tbl);
        }
    }
}