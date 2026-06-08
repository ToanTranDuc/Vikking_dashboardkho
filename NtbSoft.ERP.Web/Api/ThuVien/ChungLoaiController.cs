using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
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
    [RoutePrefix("api/ChungLoai")]
    public class ChungLoaiController : ApiController
    {
        IChungLoaiRepository _repo = new ChungLoaiRepository();
        [HttpGet]
        [Route("GetChungLoai")]
        public DataTable GetChungLoai()
        {
            return _repo.GetChungLoai();
        }
        [HttpPost]
        [Route("PostChungLoai")]
        public string PostChungLoai(List<ChungLoaiEntity> ojChungLoai)
        {
            if (ojChungLoai == null) return "false";
            string json = JsonConvert.SerializeObject(ojChungLoai);
            DataTable tbChungLoai = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tbChungLoai);
        }
        [HttpDelete]
        [Route("DeleteChungLoai")]
        public string DeleteChungLoai(int parameter)
        {
            return _repo.DeleteChungLoai(parameter);
        }
    }
}