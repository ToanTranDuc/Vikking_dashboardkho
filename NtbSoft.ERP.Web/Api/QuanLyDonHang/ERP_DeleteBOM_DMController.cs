using System.Data;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Model.QuanLyDonHang;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/DeleteBOM_DM")]
    public class ERP_DeleteBOM_DMController : ApiController
    {
        ERP_DeleteBOM_DMModel _model = new ERP_DeleteBOM_DMModel();

        [HttpPost]
        [Route("WriteLog")]
        public string WriteLog(object tblSave)
        {
            if (tblSave == null) return "false";
            string json = JsonConvert.SerializeObject(tblSave);
            DataTable objectSave = JsonConvert.DeserializeObject<DataTable>(json);
            return _model.Post(objectSave);
        }
        [HttpPost]
        [Route("Delete")]
        public string Delete(JObject JPara)
        {                   
            return _model.Delete(JPara["MaVTID"]?.ToString(), JPara["MauVTID"]?.ToString(), JPara["MaNhom"]?.ToString(), JPara["KhoVaiID"]?.ToString(), JPara["MaCode"]?.ToString(), JPara["MaKH"]?.ToString(), JPara["MaHang"]?.ToString(), JPara["MaDot"]?.ToString(), JPara["UserID"]?.ToString());
        }

    }
}