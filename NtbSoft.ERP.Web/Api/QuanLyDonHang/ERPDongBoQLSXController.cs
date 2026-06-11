using Newtonsoft.Json;
using NtbSoft.ERP.Model.ThuVien;
using System.Data;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/ERPDongBoQLSX")]
    public class ERPDongBoQLSXController : ApiController
    {
        #region Đồng bộ
       
        [HttpGet]
        [Route("DongBo")]
        public string DongBo(string action, string para = "")
        {
            return new ERPDongBoQLSXModel().DongBo(action, para);
        }
       
        #endregion
    }
}