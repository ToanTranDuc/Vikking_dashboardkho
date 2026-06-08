using System.Data;
using System.Web.Http;
using Newtonsoft.Json;
using NtbSoft.ERP.Model.Kho;

namespace NtbSoft.ERP.Web.Api.Kho
{
    [RoutePrefix("api/WriteLogThuVien")]
    public class WriteLogThuVienController : ApiController
    {
       
        WriteLogThuVienModel _model = new WriteLogThuVienModel();
        [HttpPost]
        [Route("WriteLog")]
        public string WriteLog(object tblSave,string TableName,string KeyColumn)
        {
            if (tblSave == null) return "false";
            string json = JsonConvert.SerializeObject(tblSave);
            DataTable objectSave = JsonConvert.DeserializeObject<DataTable>(json);
            return _model.Post(objectSave, TableName, KeyColumn);
        }
    }
}