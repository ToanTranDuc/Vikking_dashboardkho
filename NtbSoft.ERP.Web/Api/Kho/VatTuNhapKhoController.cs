using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NtbSoft.ERP.Web.Repository.R.Kho;
using System.Data;
using NtbSoft.ERP.Entity.Kho;
using Newtonsoft.Json;
using System.Web.Http;
namespace NtbSoft.ERP.Web.Api.Kho
{
    [RoutePrefix("api/VatTuNhapKho")]
    public class VatTuNhapKhoController : ApiController
    {
        IVatTuNhapKhoRepository _repo = new VatTuNhapKhoRepository();

        // GET: api/KhoVatTuNhap/Get
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string para2 = null)
        {
            return _repo.Get(para2);
        }
        [HttpGet]
        [Route("GetMaKho")]
        public DataTable GetMaKho()
        {
            return _repo.GetMaKho();
        }
        [HttpGet]
        [Route("GetViTri")]
        public DataTable GetViTri()
        {
            return _repo.GetViTri();
        }
        [HttpGet]
        [Route("GetTimKiem")]
        public DataTable GetTimKiem(string para2)
        {
            return _repo.GetTimKiem(para2);
        }
        [HttpGet]
        [Route("GetDonHang")]
        public DataTable GetDonHang(string para2 = null)
        {
            return _repo.GetDonHang(para2);
        }

        // POST: api/KhoVatTuNhap/Post
        [HttpPost]
        [Route("Post")]
        public string Post(List<VatTuNhapKhoEntity> objVatTu)
        {
            if (objVatTu == null) return "false";

            // Chuyển đổi danh sách đối tượng VatTuEntity sang DataTable
            string json = JsonConvert.SerializeObject(objVatTu);
            DataTable tbVatTu = JsonConvert.DeserializeObject<DataTable>(json);

            return _repo.Post(tbVatTu);
        }

        // DELETE: api/KhoVatTuNhap/Delete
        [HttpDelete]
        [Route("Delete")]
        public string Delete(int id)
        {
            return _repo.Delete(id);
        }

        // GET: api/KhoVatTuNhap/GetDotNhap
        [HttpGet]
        [Route("GetDotNhap")]
        public DataTable GetDotNhap(string para2 = null)
        {
            return _repo.GetDotNhap(para2);
        }

        // GET: api/KhoVatTuNhap/GetNgayNhap
        [HttpGet]
        [Route("GetNgayNhap")]
        public DataTable GetNgayNhap(string para2 = null)
        {
            return _repo.GetNgayNhap(para2);
        }

        // GET: api/KhoVatTuNhap/GetNguoiNhap
        [HttpGet]
        [Route("GetNguoiNhap")]
        public DataTable GetNguoiNhap(string para2 = null)
        {
            return _repo.GetNguoiNhap(para2);
        }
    }
}