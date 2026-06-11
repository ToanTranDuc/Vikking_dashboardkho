using NtbSoft.ERP.Model.ThuVien;
using System.Data;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/ERPBangSize")]
    public class ERPBangSizeController : ApiController
    {
        #region TheSize
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action,string para1 ="")
        {
            return new ERPBangSizeModel().Get(action,para1);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action,DataTable tbl)
        {           
            return new ERPBangSizeModel().Post(action, tbl);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string action,int id)
        {
            return new ERPBangSizeModel().Delete(action, id);
        }
        #endregion
    }
}