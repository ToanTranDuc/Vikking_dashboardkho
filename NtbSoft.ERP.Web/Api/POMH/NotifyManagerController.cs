using Newtonsoft.Json;
using NtbSoft.ERP.Model.POMH;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.POMH
{
    [RoutePrefix("api/NotifyManger")]
    public class NotifyManagerController : ApiController
    {
        private NotifyManagerModel _model = new NotifyManagerModel();
        [HttpGet]
        [Route("GET")]
        public async Task<DataTable> GET(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null, string para6 = null,string para7 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            para6 = para6 ?? "NONE";
            para7 = para7 ?? "NONE";
            return await _model.Get(action, para1, para2, para3, para4, para5, para6, para7);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<string> Update(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null, string para6 = null, string para7 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            para6 = para6 ?? "NONE";
            para7 = para7 ?? "NONE";
            return await _model.Post(action, para1, para2, para3, para4, para5,para6, para7);
        }
        [HttpPost]
        [Route("POST")]
        public async Task<string> POST(string action, [FromBody] string Json)
        {

            DataTable tblSave = JsonConvert.DeserializeObject<DataTable>(Json);
            return await _model.Post(action, tblSave);

        }
    }
}