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
    [RoutePrefix("api/BangMau")]
    public class BangMauController : ApiController
    {
        IBangMauRepository _repo = new BangMauRepository();
        [HttpGet]
        [Route("GetBangMau")]
        public DataTable GetBangMau()
        {
            return _repo.GetBangMau();
        }
        [HttpGet]
        [Route("GetBangMauAllowMaHang")]
        public DataTable GetBangMau(string maHang)
        {
            return _repo.GetBangMauAllowMaHang(maHang);
        }
        [HttpPost]
        [Route("PostBangMau")]
        public string PostBangMau(List<BangMauEntity> ojBangMau)
        {
            if (ojBangMau == null) return "false";
            string json = JsonConvert.SerializeObject(ojBangMau);
            DataTable tbBangMau = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tbBangMau);
        }
        [HttpPost]
        [Route("PostAutoImportBangMau")]
        public string PostAutomImportBangMau(List<BangMauEntity> ojBangMau)
        {
            if (ojBangMau == null) return "false";
            string json = JsonConvert.SerializeObject(ojBangMau);
            DataTable tbBangMau = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostAutoImport(tbBangMau);
        }
        [HttpDelete]
        [Route("DeleteBangMau")]
        public string DeleteBangMau(int parameter)
        {
            return _repo.DeleteBangMau(parameter);
        }
        [HttpGet]
        [Route("GetBangMau2")]
        public DataTable GetBangMau(string para, string para2)

        {
            return new BangMauModel().GetBangMau2(para, para2);
        }

        [HttpGet]
        [Route("GetBangMau3")]
        public DataTable GetBangMau3(string para, string para2)

        {
            return new BangMauModel().GetBangMau3(para, para2);
        }
    }
}