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
    [RoutePrefix("api/donvi")]
    public class DonViController : ApiController
    {
        IDonViRepository _repo = new DonViRepository();
        [HttpGet]
        [Route("GetDonVi")]
        public DataTable GetDonVi()
        {
            return _repo.GetDonVi();
        }
        [HttpPost]
        [Route("PostDonVi")]
        public string PostDonVi(List<DonViEntity> ojDonVi)
        {
            if (ojDonVi == null) return "false";
            string json = JsonConvert.SerializeObject(ojDonVi);
            DataTable tbDonVi = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tbDonVi);
        }
        [HttpDelete]
        [Route("DeleteDonVi")]
        public string DeleteDonVi(int parameter)
        {
            return _repo.DeleteDonVi(parameter);
        }
    }
}