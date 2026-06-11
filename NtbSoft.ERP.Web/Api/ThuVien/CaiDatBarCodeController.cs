using NtbSoft.ERP.Model.ThuVien;
using System.Data;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/CaiDatBarCode")]
    public class CaiDatBarCodeController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1, string Para2, string Para3)
        {
            return new CaiDatBarCodeModel().Get(action, Para1, Para2, Para3);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(string action, string Para1, DataTable dt)
        {
            return new CaiDatBarCodeModel().Post(action, Para1, dt);
        }
        [HttpPost]
        [Route("Delete")]
        public string Delete(string action, string Para1, DataTable items)
        {
            return new CaiDatBarCodeModel().Post(action, Para1, items);
        }

        [HttpGet]
        [Route("GetNew")]
        public DataTable GetNew(string Para1, string Para2, string Para3)
        {
            return new CaiDatBarCodeModel().GetNew(Para1, Para2, Para3);
        }

        [HttpPost]
        [Route("PostNew")]
        public string PostNew(string action,string Para1, DataTable dt)
        {
            return new CaiDatBarCodeModel().PostNew(action,Para1, dt);
        }

    }
}