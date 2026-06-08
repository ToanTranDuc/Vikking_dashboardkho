using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Data;
using NtbSoft.ERP.Web.Repository.R.ThuVien;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/KhachHang")]
    public class KhachHangController : ApiController
    {
        IKhachHangRepository _repo = new KhachHangRepository();
        [HttpGet]
        [Route("GetKhachHang")]
        public DataTable GetKhachHang()
        {
            return _repo.GetKhachHang();
        }
        [HttpPost]
        [Route("PostKhachHang")]
        public string PostKhachHang(List<KhachHangEntity> ojKhachHang)
        {
            if (ojKhachHang == null) return "false";
            string json = JsonConvert.SerializeObject(ojKhachHang);
            DataTable tbKhachHang = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tbKhachHang);
        }
        [HttpPost]
        [Route("PostAutoKH")]
        public string PostAutoKH(List<KhachHangEntity> ojKhachHang)
        {
            if (ojKhachHang == null) return "false";
            string json = JsonConvert.SerializeObject(ojKhachHang);
            DataTable tbKhachHang = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostAutoKH(tbKhachHang);
        }
        [HttpDelete]
        [Route("DeleteKhachHang")]
        public string DeleteKhachHang(int parameter)
        {
            return _repo.DeleteKhachHang(parameter);
        }
    }
}