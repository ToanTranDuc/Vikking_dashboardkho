using NtbSoft.ERP.Model.ThuVien;
using System.Data;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/NhomLoiNL")]
    public class NhomLoiNLController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1 = "", string Para2 = "")
        {
            return new NhomLoiNLModel().Get(action, Para1, Para2);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action, DataTable dt)
        {
            return new NhomLoiNLModel().Post(action, dt);
        }
        [HttpPost]
        [Route("Delete")]
        public string Delete(string action, string Id)
        {
            return new NhomLoiNLModel().Delete(action, Id);
        }
    }
}