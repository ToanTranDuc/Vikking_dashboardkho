using Newtonsoft.Json;
using NtbSoft.ERP.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Win.Utils
{
    public class GetDataFromOtherServer
    {
        public static HttpClientExtension _clientExtension = new HttpClientExtension();
        public static DataTable GetData(string server_api, string queryString)
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
