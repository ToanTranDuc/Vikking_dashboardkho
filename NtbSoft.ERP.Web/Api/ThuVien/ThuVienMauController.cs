using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Web.Http;
using System.Data;
using NtbSoft.ERP.Web.Repository.R.ThuVien;
using NtbSoft.ERP.Entity.ThuVien;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/ThuVienMau")]
    public class ThuVienMauController:ApiController
    {
        IThuVienMauRepository _repo = new ThuVienMauRepository();

        // GET: api/ThuVienMau/Get
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return _repo.Get();
        }

        [HttpGet]
        [Route("GetByMaKH")]
        public DataTable GetByMaKH(string MaKH)
        {
            return _repo.GetByMaKH(MaKH);
        }

        // POST: api/ThuVienMau/Post
        [HttpPost]
        [Route("Post")]
        public string Post(List<ThuVienMauEntity> objThuVienMau)
        {
            if (objThuVienMau == null) return "false";
            var lstToPost = objThuVienMau.Select(item => new
            {
                item.ID,
                item.MaMauKH,
                item.MaKH,
                item.CodeMauKH,

                item.TenMauKH,
                item.CodeKhac,
                item.GhiChu,
                item.TenKH

            }).ToList();
            // Chuyển đổi danh sách đối tượng ThuVienMauEntity sang DataTable
            string json = JsonConvert.SerializeObject(lstToPost);
            DataTable tbThuVienMau = JsonConvert.DeserializeObject<DataTable>(json);

            return _repo.Post(tbThuVienMau);
        }

        [HttpPost]
        [Route("POSTEXCEL")]
        public string POSTEXCEL(List<ThuVienMauEntity> objThuVienMau)
        {
            if (objThuVienMau == null) return "false";
            var lstToPost = objThuVienMau.Select(item => new
            {
                item.ID,
                item.MaMauKH,
                item.MaKH,
                item.CodeMauKH,

                item.TenMauKH,
                item.CodeKhac,
                item.GhiChu,
                item.TenKH

            }).ToList();
            // Chuyển đổi danh sách đối tượng ThuVienMauEntity sang DataTable
            string json = JsonConvert.SerializeObject(lstToPost);
            DataTable tbThuVienMau = JsonConvert.DeserializeObject<DataTable>(json);

            return _repo.POSTEXCEL(tbThuVienMau);
        }

        // DELETE: api/ThuVienMau/Delete
        [HttpDelete]
        [Route("Delete")]
        public string Delete(int ID)
        {
            return _repo.Delete(ID);
        }

    }
}