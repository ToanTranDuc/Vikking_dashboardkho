using Newtonsoft.Json;
using NtbSoft.ERP.Web.Repository.R.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/Duyet")]
    public class DuyetController : ApiController
    {
        IDuyetRepository _repo = new DuyetRepository();

        [HttpGet]
        [Route("GetEx")]
        public DataTable GetEx(string madh, string malenh) 
        {

            return _repo.GetEx(madh,malenh);
        }
        [HttpPost]
        [Route("PostDuyet")]
        public string PostDuyet(DataTable ojDinhMuc) 
        {
            if (ojDinhMuc == null) return "false";
            string json = JsonConvert.SerializeObject(ojDinhMuc);
            DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDuyet(ojDinhMuc);
        }

        [HttpPost]
        [Route("PostHuy")]
        public string PostHuy(DataTable ojDinhMuc) 
        {
            if (ojDinhMuc == null) return "false";
            string json = JsonConvert.SerializeObject(ojDinhMuc);
            DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostHuy(ojDinhMuc);
        }
    }
}