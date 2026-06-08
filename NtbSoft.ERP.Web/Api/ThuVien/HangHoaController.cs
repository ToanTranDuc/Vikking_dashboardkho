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
    [RoutePrefix("api/HangHoa")]
    public class HangHoaController : ApiController
    {
        IHangHoaRepository _repo = new HangHoaRepository();
        [HttpGet]
        [Route("GetHangHoa")]
        public DataTable GetHangHoa()
        {
            return _repo.GetHangHoa();
        }
        [HttpPost]
        [Route("PostHangHoa")]
        public string PostHangHoa(List<HangHoaEntity> ojHangHoa)
        {
            if (ojHangHoa == null) return "false";
            string json = JsonConvert.SerializeObject(ojHangHoa);
            DataTable tbHangHoa = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tbHangHoa);
        }
        [HttpDelete]
        [Route("DeleteHangHoa")]
        public string DeleteHangHoa(int parameter)
        {
            return _repo.DeleteHangHoa(parameter);
        }
        [HttpGet]
        [Route("GetHangHoaWithDVSX")]
        public DataTable GetHangHoaWithDVSX()
        {
            return _repo.GetHangHoaWithDVSX();
        }
        [HttpGet]
        [Route("GetHangHoaCheckKeoVe")]
        public DataTable GetHangHoaCheckKeoVe()
        {
            return _repo.GetHangHoaCheckKeoVe();
        }
        [HttpGet]
        [Route("GetKiemTraHH")]
        public DataTable GetKiemTraHH()
        {
            return _repo.GetKiemTraHH();
        }

        #region quan
        [HttpGet]
        [Route("GetHangHoaByKH")]
        public DataTable GetHangHoabykh(string makh, string mahang)
        {
            return new HangHoaModel().GetHangHoabykh(makh, mahang);
        }
        #endregion
    }
}