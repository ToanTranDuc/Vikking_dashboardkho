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
    [RoutePrefix("api/ThuVienKhoSize")]
    public class ThuVienKhoSizeController:ApiController
    {
        IThuVienKhoSizeRepository _repo = new ThuVienKhoSizeRepository();

        // GET: api/ThuVienKhoSize/Get
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return _repo.Get();
        }
        [HttpGet]
        [Route("GetByMaVT")]
        public DataTable GetByMaVT(string MaVT)
        {
            return _repo.GetByMaVT(MaVT);
        }

        // POST: api/ThuVienKhoSize/Post
        [HttpPost]
        [Route("Post")]
        public string Post(List<ThuVienKhoSizeEntity> objThuVienKhoSize)
        {
            if (objThuVienKhoSize == null) return "false";

            // Chuyển đổi danh sách đối tượng VatTuChiTietEntity sang DataTable
            string json = JsonConvert.SerializeObject(objThuVienKhoSize);
            DataTable tbThuVienKhoSize = JsonConvert.DeserializeObject<DataTable>(json);

            return _repo.Post(tbThuVienKhoSize);
        }

        // DELETE: api/ThuVienKhoSize/Delete
        [HttpDelete]
        [Route("Delete")]
        public string Delete(int ID)
        {
            return _repo.Delete(ID);
        }
    }
}