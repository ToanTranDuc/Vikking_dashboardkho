using Newtonsoft.Json;
using NtbSoft.ERP.Model.ThuVien;
using System.Data;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/ERPDonHangTong")]
    public class ERPDonHangTongController : ApiController
    {
        #region HangHoa
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "")
        {
            return new ERPDonHangTongModel().Get(action, para, para2, para3, para4, para5, para6);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action, string type, object tbl)
        {
            string json = JsonConvert.SerializeObject(tbl);
            DataTable tbDVSX = JsonConvert.DeserializeObject<DataTable>(json);
            return new ERPDonHangTongModel().Post(action, type, tbDVSX);
        }
        [HttpPost]
        [Route("PostV1")]
        public string PostV1(string action, string type, object tbl, string para = "", string para2 = "", string para3 = "", string para4 = "")
        {
            string json = JsonConvert.SerializeObject(tbl);
            DataTable tbDVSX = JsonConvert.DeserializeObject<DataTable>(json);
            return new ERPDonHangTongModel().PostV1(action, type, tbDVSX, para, para2, para3, para4);
        }
        [HttpPost]
        [Route("PostV2")]
        public string PostV2(string action, string type, object tbl)
        {
            string json = JsonConvert.SerializeObject(tbl);
            DataTable tbDVSX = JsonConvert.DeserializeObject<DataTable>(json);
            return new ERPDonHangTongModel().PostV2(action, type, tbDVSX);
        }
        [HttpGet]
        [Route("GetChuyen")]
        public string GetChuyen(string action, string para = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "")
        {
            return new ERPDonHangTongModel().GetChuyen(action, para, para2, para3, para4, para5, para6);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string action, string id, string para2 = "", string para3 = "", string para4 = "")
        {
            return new ERPDonHangTongModel().Delete(action, id, para2, para3, para4);
        }
        #endregion
        [HttpGet]
        [Route("PostUpSLCP")]
        public string PostUpSLCP(string para = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "")
        {
            return new ERPDonHangTongModel().PostUpSLCP(para, para2, para3, para4, para5, para6);
        }
    }
}