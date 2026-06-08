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
    [RoutePrefix("api/QuocGia")]
    public class QuocGiaController : ApiController
    {
        IQuocGiaRepository _repo = new QuocGiaRepository();
        QuocGiaModel _model = new QuocGiaModel();
        [HttpGet]
        [Route("GetQuocGia")]
        public DataTable GetQuocGia()
        {
            return _repo.GetQuocGia();
        }
        [HttpPost]
        [Route("PostQuocGia")]
        public string PostQuocGia(List<QuocGiaEntity> ojQuocGia)
        {
            if (ojQuocGia == null) return "false";
            string json = JsonConvert.SerializeObject(ojQuocGia);
            DataTable tbQuocGia = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tbQuocGia);
        }
        [HttpPost]
        [Route("PostAutoQG")]
        public string PostAutoQG(List<QuocGiaEntity> ojQuocGia)
        {
            if (ojQuocGia == null) return "false";
            string json = JsonConvert.SerializeObject(ojQuocGia);
            DataTable tbQuocGia = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostAutoQG(tbQuocGia);
        }
        [HttpDelete]
        [Route("DeleteQuocGia")]
        public string DeleteQuocGia(string parameter)
        {
            return _repo.DeleteQuocGia(parameter);
        }

        [HttpPost]
        [Route("PostCang")]
        public string PostCang(List<QuocGiaEntity> ojQuocGia)
        {
            if (ojQuocGia == null) return "false";
            string json = JsonConvert.SerializeObject(ojQuocGia);
            DataTable tbQuocGia = JsonConvert.DeserializeObject<DataTable>(json);
            return _model.PostCang(tbQuocGia);
        }

        [HttpDelete]
        [Route("DeleteCangQuocGia")]
        public string DeleteCangQG(string parameter, string parameter1)
        {
            return _model.DeleteCangQG(parameter, parameter1);
        }

    }
}