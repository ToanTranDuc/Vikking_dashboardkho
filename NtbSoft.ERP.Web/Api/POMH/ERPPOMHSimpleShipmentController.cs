using System.Web.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data;
using NtbSoft.ERP.Model.POMuaHang;
using Microsoft.AspNet.SignalR;
using System;
using NtbSoft.ERP.Model.POMH;

namespace NtbSoft.ERP.Web.Api.POMH
{
    [RoutePrefix("api/SimpleShipment")]
    public class ERPPOMHSimpleShipmentController : ApiController
    {
        private ERPPOMHSimpleShipmentModel _model = new ERPPOMHSimpleShipmentModel();
        [HttpGet]
        [Route("GET")]
        public async Task<DataTable> GET(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            return await _model.Get(action, para1, para2, para3, para4, para5);
        }
        [HttpPost]
        [Route("POST")]
        public async Task<string> POST(string action, [FromBody] string Json)
        {

            DataTable tblSave = JsonConvert.DeserializeObject<DataTable>(Json);
            return await _model.Post(action, tblSave);

        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<string> Delete(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            return await _model.Delete(action, para1, para2, para3, para4, para5);
        }
    }
}