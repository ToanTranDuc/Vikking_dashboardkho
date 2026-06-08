using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/ERPBangInSeam")]
    public class ERPBangInSeamController : ApiController
    {
        #region InSeam
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action,string para1 ="")
        {
            return new ERPBangInSeamModel().Get(action,para1);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action,DataTable tbl)
        {           
            return new ERPBangInSeamModel().Post(action, tbl);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string action,int id)
        {
            return new ERPBangInSeamModel().Delete(action, id);
        }       
        #endregion


    }
}