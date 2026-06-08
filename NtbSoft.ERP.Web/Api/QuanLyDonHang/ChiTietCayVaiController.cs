using Newtonsoft.Json;
using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/chitiet")]
    public class ChiTietCayVaiController : ApiController
    {
        ChiTietCayVai _model = new ChiTietCayVai();

        [HttpGet]
        [Route("get")]
        public DataTable Get(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null)
        {
            return _model.Get(action, para1, para2, para3, para4, para5);
        }

        [HttpPost]
        [Route("postcayvaicapphat")]
        public string PostCayVaiVaoCapPhat(DataTable objectCayVai)
        {
            if (objectCayVai == null) return "false";
            string json = JsonConvert.SerializeObject(objectCayVai);
            DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
            return _model.PostCayVaiVaoCapPhat(objectCayVai);
        }
    }
}
