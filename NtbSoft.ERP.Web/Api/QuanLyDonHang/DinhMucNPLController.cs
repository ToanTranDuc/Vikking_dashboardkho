using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Web.Repository.R.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/DinhMucNPL")]
    public class DinhMucNPLController : ApiController
    {
        //IHangHoaRepository _repo = new HangHoaRepository();
        IDinhMucNPLRepository _repo = new DinhMucNPLRepository();
        [HttpGet]
        [Route("GetDMNPL")]
        public DataTable GetDMNPL(string maDH)
        {
            return _repo.Get(maDH);
        }
        [HttpPost]
        [Route("PostDMNPL")]
        public string PostDMNPL(DataTable ojHangHoa)
        {
            if (ojHangHoa == null) return "false";
            //string json = JsonConvert.SerializeObject(ojHangHoa);
            //DataTable tbHangHoa = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(ojHangHoa);
        }
        [HttpDelete]
        [Route("DeleteDMNPL")]
        public string DeleteDMNPL(int id)
        {
            return _repo.Delete(id);
        }
    }
}