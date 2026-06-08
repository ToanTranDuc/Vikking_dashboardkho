using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Model.ThuVien;
using NtbSoft.ERP.Web.Repository.R.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/QTY_KiemDauChuyen")]
    public class QTY_KiemDauChuyenController : ApiController
    {
       
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action,string para1 = "", string para2 = "", string para3 = "", string para4 = "")
        {
            return new QTY_KiemDauChuyenModel().Get(action,para1,para2,para3,para4);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action, [FromBody] Newtonsoft.Json.Linq.JArray body)
        {
            try
            {
                DataTable tbl = new DataTable();
                if (body != null && body.Count > 0)
                    tbl = JsonConvert.DeserializeObject<DataTable>(body.ToString());

                return new QTY_KiemDauChuyenModel().Post(action, tbl);
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        [HttpPost]
        [Route("Delete")]
        public string Delete(string action,string parameter)
        {
            return new QTY_KiemDauChuyenModel().Delete(action,parameter);
        }
        [HttpPost]
        [Route("PostDuyet")]
        public string PostDuyet(string action, string para1,string para2)
        {
            return new QTY_KiemDauChuyenModel().Post(action, para1,para2);
        }
    }
}