using NtbSoft.ERP.Model.ThuVien;
using System.Data;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/QTY_MaHangHinh")]
    public class QTY_MaHangHinhController : ApiController
    { 

        #region Hình
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action,string para1 = "")
        {
            return new QTY_MaHang_HinhModel().Get(action,para1);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(string action, DataTable tbl)
        {
            return new QTY_MaHang_HinhModel().Post(action, tbl);
        }
        [HttpPost]
        [Route("PostCopy")]
        public string PostCopy(string action, DataTable tbl,string para1 ="")
        {
            return new QTY_MaHang_HinhModel().Post(action, tbl, para1);
        }
        #endregion
    }
}