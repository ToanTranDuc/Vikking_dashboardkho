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
    [RoutePrefix("api/DonViSanXuat")]
    public class DonViSanXuatController : ApiController
    {
        IDonViSanXuatRepository _repo = new DonViSanXuatRepository();
        [HttpGet]
        [Route("GetDonViSanXuat")]
        public DataTable GetDonViSanXuat()
        {
            return _repo.GetDonViSanXuat();
        }

        [HttpGet]
        [Route("GetDonViSanXuatDetail")]
        public DataTable GetDonViSanXuatDetail(string MaDVSX)
        {
            return _repo.GetDonViSanXuatDetail(MaDVSX);
        }

        [HttpPost]
        [Route("PostDonViSanXuat")]
        public string PostDonViSanXuat(List<DonViSanXuatEntity> ojDonViSanXuat)
        {
            if (ojDonViSanXuat == null) return "false";
            string json = JsonConvert.SerializeObject(ojDonViSanXuat);
            DataTable tbDonViSanXuat = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tbDonViSanXuat);
        }
        [HttpDelete]
        [Route("DeleteDonViSanXuat")]
        public string DeleteDonViSanXuat(int parameter)
        {
            return _repo.DeleteDonViSanXuat(parameter);
        }

        [HttpPost]
        [Route("PostDonViSanXuatDetail")]
        public string PostDonViSanXuatDetail(DataTable tbl)
        {
            return _repo.PostDonViSanXuatDetail(tbl);
        }

        [HttpGet]
        [Route("GetCaiDatKho")]
        public DataTable GetCaiDat_DVSX_Kho(string action,string para1)
        {
            return new DonViSanXuatModel().GetCaiDat_DVSX_Kho(action,para1);
        }
        [HttpPost]
        [Route("PostCaiDatKho")]
        public string PostCaiDatKho(string MaDVSX,DataTable tbl)
        {
            string action = "Post";
            if (tbl is null || tbl.Rows.Count == 0) action = "Delete";
            return new DonViSanXuatModel().PostCaiDat_DVSX_Kho(action,MaDVSX, tbl);
        }


        [HttpGet]
        [Route("GetKhoList")]
        public DataTable GetKhoList()
        {
            return _repo.GetKhoList();


        }
        [HttpGet]
        [Route("GetLookupDVSX")]
        public DataTable GetLookupDVSX()
        {
            return _repo.GetLookupDVSX();
        }
        [HttpDelete]
        [Route("DeleteKho")]
        public string DeleteKho(string maKho)
        {
            return _repo.DeleteKho(maKho);
        }
        [HttpPost]
        [Route("PostKho")]
        public string PostKho(DataTable tbl)
        {
            return _repo.PostKho(tbl);
        }
    }
}