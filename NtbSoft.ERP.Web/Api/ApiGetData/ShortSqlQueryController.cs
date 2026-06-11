using BaoCaoTongHop.Models.GetData;
using BaoCaoTongHop.Service;
using Newtonsoft.Json;
using NtbSoft.ERP.Libs;
using NtbSoft.ERP.Model;
using NtbSoft.ERP.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api
{
    [RoutePrefix("api/sqlQ")]
    public class ShortSqlQueryController : ApiController
    {
        public static HttpClientExtension _clientExtension = new HttpClientExtension();
        [HttpPost]
        [Route("getObj")]// sqlQ/getObj
        public DataTable getObj(QueryObjViewModel objView)
        {
            string query = objView.query;
            return GetDataTable(query);
        }
        [HttpPost]
        [Route("RunStored")]// sqlQ/getObj
        public DataTable RunStored(RunStoredViewModel objView)
        {
            string SV_api = ConfigurationManager.AppSettings[objView.server].ToString();
            string sqlQuery_Str = string.Format(@"EXEC {0} {1}", objView.stored, objView.paramStr);
            DataTable result = getDBFromOtherServer(SV_api, sqlQuery_Str); //getData( "select * from Product").Result;
            return result;
        }
        [HttpPost]
        [Route("RunStoredDH")]// sqlQ/getObj
        public DataTable RunStoredDH(RunStoredViewModel objView)
        {
            string SV_api = ConfigurationManager.AppSettings["DH"].ToString();
            string sqlQuery_Str = string.Format(@"EXEC {0} {1}", objView.stored, objView.paramStr);
            DataTable result = getDBFromOtherServer(SV_api, sqlQuery_Str); //getData( "select * from Product").Result;
            return result;
        }
        public DataTable GetDataTable(string query)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataTable dt = new DataTable();
            try
            {
                conn = SqlHelper.GetConnection();
                cmd = new SqlCommand(query, conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                return dt;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        //==================================================================================
        public static DataTable getDBFromOtherServer(string server_api, string queryString)
        {
            QueryObjViewModel objViewM = new QueryObjViewModel { query = queryString };
            string json = Task.Run(async () => { return await _clientExtension.PostAsync(server_api, objViewM); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null)
                return null;
            if (tbl.Rows.Count == 0)
                return null;
            return tbl;
        }
    }
}