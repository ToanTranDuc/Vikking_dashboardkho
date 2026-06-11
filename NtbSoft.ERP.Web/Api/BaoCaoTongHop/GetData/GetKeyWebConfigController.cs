using NtbSoft.ERP.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.GetData
{
    [RoutePrefix("api/webconfig")]
    public class GetKeyWebConfigController : ApiController
    {
        [HttpPost]
        [Route("GetKey")]// sqlQ/getObj
        public string getObj(QueryObjViewModel keyName)
        {
            return ConfigurationManager.AppSettings[keyName.query].ToString();
        }
    }
}
