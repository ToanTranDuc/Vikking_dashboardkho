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
    [RoutePrefix("api/LoaiHangHoa")]
    public class LoaiHangHoaController : ApiController
    {
        ILoaiHangHoaRepository _repo = new LoaiHangHoaRepository();
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return _repo.Get();
        }
        [HttpPost]
        [Route("Post")]
        public string Post(List<LoaiHangHoaEntity> ojLHH)
        {
            if (ojLHH == null) return "false";
            string json = JsonConvert.SerializeObject(ojLHH);
            DataTable tbDVCL = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tbDVCL);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(int parameter)
        {
            return _repo.Delete(parameter);
        }
    }
}