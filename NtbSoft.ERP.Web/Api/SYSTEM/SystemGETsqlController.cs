using NtbSoft.ERP.Model.SYSTEM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.SYSTEM
{
    [RoutePrefix("api/SystemGETsql")]
    public class SystemGETsqlController : ApiController
    {
        [HttpGet]
        [Route("GETsqlPara")]
        public DataTable GetSQL(string columns, string tableName, string whereClause)
        {
            SystemGETsqlModel _model = new SystemGETsqlModel();
            return _model.Get(columns, tableName, whereClause);
        }

        [HttpGet]
        [Route("GETsqlQuery")]
        public DataTable GetSQL(string query)
        {
            SystemGETsqlModel _model = new SystemGETsqlModel();
            return _model.Get(query);
        }

        [HttpGet]
        [Route("GETIP")]
        public DataTable GETIP()
        {
            SystemGETsqlModel _model = new SystemGETsqlModel();
            return _model.GetIP();
        }
    }
}
