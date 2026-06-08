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
    [RoutePrefix("api/VatTuChiTiet")]
    public class VatTuChiTietController : ApiController
    {
        IVatTuChiTietRepository _repo = new VatTuChiTietRepository();

        // GET: api/VatTuChiTiet/Get
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return _repo.Get();
        }

        // GET: api/VatTuChiTiet/GetByMaVT
        [HttpGet]
        [Route("GetByMaVT")]
        public DataTable GetByMaVT(string MaVT)
        {
            return _repo.GetByMaVT(MaVT);
        }

        // POST: api/VatTuChiTiet/Post
        [HttpPost]
        [Route("Post")]
        public string Post(List<VatTuChiTietEntity> objVatTuChiTiet)
        {
            if (objVatTuChiTiet == null) return "false";

            // Chuyển đổi danh sách đối tượng VatTuChiTietEntity sang DataTable
            string json = JsonConvert.SerializeObject(objVatTuChiTiet);
            DataTable tbVatTuChiTiet = JsonConvert.DeserializeObject<DataTable>(json);

            return _repo.Post(tbVatTuChiTiet);
        }

        // DELETE: api/VatTuChiTiet/Delete
        [HttpDelete]
        [Route("Delete")]
        public string Delete(int ID)
        {
            return _repo.Delete(ID);
        }
    }

}