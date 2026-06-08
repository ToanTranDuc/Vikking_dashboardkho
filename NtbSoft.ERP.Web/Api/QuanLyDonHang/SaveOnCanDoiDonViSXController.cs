using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data;
using NtbSoft.ERP.Model.QuanLyDonHang;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using Newtonsoft.Json;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/SaveOnCanDoiDonViSX")]
    public class SaveOnCanDoiDonViSXController : ApiController
    {
        private SaveOnCanDoiDonViSXModel _model = new SaveOnCanDoiDonViSXModel();
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "", string para7 = "")
        {
            return _model.Get(action, para, para2, para3, para4, para5, para6, para7);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(string action, [FromBody] List<EditCanDoiLenhSanXuatEntity> objSave, string para = "", string para2 = "", string para3 = "", string para4 = "")
        {
            if (objSave == null) return "false";
            string json = JsonConvert.SerializeObject(objSave);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return _model.Post(action, para, para2, para3, para4, tbl);
        }
        [HttpPost]
        [Route("DongBoQLSX")]
        public string Post(string action, string para = "")
        {
          
            return _model.Post(action, para);
        }

    }
}
