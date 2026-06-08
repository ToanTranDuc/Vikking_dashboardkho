using NtbSoft.ERP.Model.KeHoach;
using System.Data;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [RoutePrefix("api/ChuyenKho")]
    public class PackageListChuyenKhoController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1, string Para2,string Para3 = "")
        {
            return new PackageListChuyenKhoModel().Get(action, Para1, Para2,Para3);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action, DataSet ds)
        {
            return new PackageListChuyenKhoModel().Post(action, ds.Tables[0], ds.Tables[1],ds.Tables[2]);
        }
        [HttpPost]
        [Route("Delete")]
        public string Delete(string action, string Para, DataSet ds)
        {
            return new PackageListChuyenKhoModel().Delete(action, Para, ds.Tables[0]);
        }
    }
}