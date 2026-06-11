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
    [RoutePrefix("api/DonViChungLoai")]
    public class DonViChungLoaiController : ApiController
    {
        IDonViChungLoaiRepository _repo = new DonViChungLoaiRepository();
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return _repo.Get();
        }
        [HttpPost]
        [Route("Post")]
        public string Post(List<DonViChungLoaiEntity> ojDVCL)
        {
            if (ojDVCL == null) return "false";
            string json = JsonConvert.SerializeObject(ojDVCL);
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