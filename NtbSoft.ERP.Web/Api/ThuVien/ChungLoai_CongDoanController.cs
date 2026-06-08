using NtbSoft.ERP.Model.ThuVien;
using System.Data;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/ChungLoai_CongDoan")]
    public class ChungLoai_CongDoanController : ApiController
    {
        #region Chủng loại
        [HttpGet]
        [Route("GetChungLoai")]
        public DataTable GetChungLoai(string action,string para1 ="")
        {
            return new ChungLoai_CongDoanModel().GetChungLoai(action,para1);
        }
        [HttpPost]
        [Route("PostChungLoai")]
        public string PostChungLoai(string action,DataTable tbl)
        {           
            return new ChungLoai_CongDoanModel().PostChungLoai(action, tbl);
        }
        [HttpDelete]
        [Route("DeleteChungLoai")]
        public string DeleteChungLoai(string action,int id)
        {
            return new ChungLoai_CongDoanModel().DeleteChungLoai(action, id);
        }
        #endregion

        #region Công đoạn
        [HttpGet]
        [Route("GetCongDoan")]
        public DataTable GetCongDoan(string action,string para1 = "")
        {
            return new ChungLoai_CongDoanModel().GetCongDoan(action,para1);
        }
        [HttpPost]
        [Route("PostCongDoan")]
        public string PostCongDoan(string action, DataTable tbl)
        {
            return new ChungLoai_CongDoanModel().PostCongDoan(action, tbl);
        }
        [HttpDelete]
        [Route("DeleteCongDoan")]
        public string DeleteCongDoan(int id)
        {
            return new ChungLoai_CongDoanModel().DeleteCongDoan(id);
        }
        #endregion
    }
}