using NtbSoft.ERP.Model.ThuVien;
using System.Data;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/ERPHangHoa")]
    public class ERPHangHoaController : ApiController
    {
        #region HangHoa
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action,string para ="", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "")
        {
            return new ERPHangHoaModel().Get(action,para, para2, para3, para4, para5, para6);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action,string type,DataTable tbl)
        {           
            return new ERPHangHoaModel().Post(action, type, tbl);
        }
        [HttpPost]
        [Route("PostEdit")]
        public string PostEdit(string action, string type, DataTable tbl)
        {
            return new ERPHangHoaModel().PostEdit(action, type, tbl);
        }
        [HttpPost]
        [Route("DeleteSize")]
        public string DeleteSize(string action, string type, DataTable tbl)
        {
            return new ERPHangHoaModel().DeleteSize(action, type, tbl);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string action,string id, string para2 = "", string para3 = "", string para4 = "")
        {
            return new ERPHangHoaModel().Delete(action, id,para2, para3, para4);
        }
        [HttpPost]
        [Route("GetKiemTra")]
        public DataTable GetKiemTra(string action, string type, DataTable tbl)
        {
            return new ERPHangHoaModel().GetKiemTra(action, type, tbl);
        }
        #endregion
    }
}