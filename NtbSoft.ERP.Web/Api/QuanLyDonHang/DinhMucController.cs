using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Web.Repository.R.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/DinhMuc")]
    public class DinhMucController : ApiController
    {
        IDinhMucRepository _repo = new DinhMucRepository();
        [HttpGet]
        [Route("Get")]
        public DataTable Get(int pageIndex, int pageSize,string parameter)
        {
            return _repo.Get(pageIndex, pageSize,parameter);
         }
        [HttpGet]
        [Route("GetKiemTraDinhMuc")]
        public DataTable GetKiemTraDinhMuc(string donhang)
        {
            return _repo.GetKiemTraDinhMuc(donhang);
        }
        [HttpPost]
        [Route("PostDinhMuc")]
        public string PostDinhMuc(DataTable ojDinhMuc)
        {
            if (ojDinhMuc == null) return "false";
            string json = JsonConvert.SerializeObject(ojDinhMuc);
            DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDinhMuc(ojDinhMuc);
        }
        [HttpPost]
        [Route("UpdateNPL")]
        public string UpdateNPL(DataTable ojDinhMuc)
        {
            if (ojDinhMuc == null) return "false";
            string json = JsonConvert.SerializeObject(ojDinhMuc);
            DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.UpdateNPL(ojDinhMuc);
        }
    }
}