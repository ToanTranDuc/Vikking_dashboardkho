using System.Web.Http;
using System.Threading.Tasks;
using System.Data;
using NtbSoft.ERP.Model.POMH;
using NtbSoft.ERP.Entity.POMH;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace NtbSoft.ERP.Web.Api.POMH
{
    [RoutePrefix("api/ERP_POMH_PhieuMuaHangMMTB_Import_Seri")]
    public class POMH_PhieuMuaHangMMTB_Import_SeriController : ApiController
    {
        POMH_PhieuMuaHangMMTB_Import_SeriModel _model = new POMH_PhieuMuaHangMMTB_Import_SeriModel();

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
        [Route("UpdateXNMuaMMTB")]
        public async Task<string> UpdateXNMuaMMTB(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            return await _model.Post(action, para1, para2, para3, para4, para5);
        }
        [HttpPost]
        [Route("Post")]
        public async Task<string> Post(string action,List<ERP_POMH_PhieuMuaHang_MMTB_SoSeriEntity> lstSave)
        {
            if (lstSave == null) return "false";
            string json = JsonConvert.SerializeObject(lstSave);
            DataTable tblSave = JsonConvert.DeserializeObject<DataTable>(json);
            return await _model.Post(action, tblSave);
        }


    }
}