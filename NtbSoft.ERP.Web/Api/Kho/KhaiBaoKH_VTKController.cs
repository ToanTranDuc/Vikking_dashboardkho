using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Model.Kho;
using NtbSoft.ERP.Web.Filter;
using NtbSoft.ERP.Web.Repository.R.Kho;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.Kho
{
    //[HMACAuthentication]
    [RoutePrefix("api/KhaiBaoKH_VTK")]
    public class KhaiBaoKH_VTKController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1, string Para2, string Para3, string Para4, string Para5, string Para6)
        {
            return new KhaiBaoKH_VTKModel().Get(action, Para1, Para2, Para3, Para4, Para5, Para6);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable data)
        {
            return new KhaiBaoKH_VTKModel().Post(data, "Post", "@TypeTable");
        }
    }
    
}
