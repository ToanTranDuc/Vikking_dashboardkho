using NtbSoft.ERP.Model.ThuVien;
using System.Data;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/NhomLoiPL")]
    public class NhomLoiPLController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1 = "", string Para2 = "")
        {
            return new NhomLoiPLModel().Get(action, Para1, Para2);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action, DataTable dt)
        {
            return new NhomLoiPLModel().Post(action, dt);
        }
        [HttpPost]
        [Route("Delete")]
        public string Delete(string action, string Id)
        {
            return new NhomLoiPLModel().Delete(action, Id);
        }
    }
}