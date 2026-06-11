using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data;
using NtbSoft.ERP.Model.Kho;

namespace NtbSoft.ERP.Web.Api.Kho
{
    [RoutePrefix("api/ERPVatTuCBM")]
    public class ERPVatTuCBMController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para = "", string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", 
            string para6 = "",string para7="", string para8 = "", string para9 = "", string para10 = "", string para11 = "")
        {
            return new ERPVatTuCBMModel().Get(action, para,para1, para2, para3, para4, para5, para6, para7,para8,para9,para10,para11);
        }
       
        [HttpPost]
        [Route("Post")]
        public string Post(string action, [FromBody] DataTable tbl, string para = "", string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", 
            string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "", string para11 = "")
        {
            if (!tbl.Columns.Contains("Status"))
            {
                DataColumn statusCol = new DataColumn("Status", typeof(int));
                statusCol.DefaultValue = 1;
                tbl.Columns.Add(statusCol);

                foreach (DataRow row in tbl.Rows)
                {
                    row["Status"] = 1;
                }
            }
            return new ERPVatTuCBMModel().Post(action, para, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10, para11, tbl);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string action, string para = "", string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
            string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "", string para11 = "")
        {
            return new ERPVatTuCBMModel().Delete(action, para, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10, para11);
        }
    }
}