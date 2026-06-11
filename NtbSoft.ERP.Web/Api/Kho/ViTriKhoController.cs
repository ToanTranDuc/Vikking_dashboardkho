using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Model.Kho;
using NtbSoft.ERP.Web.Filter;
using NtbSoft.ERP.Web.Repository.R.Kho;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.Kho
{
    //[HMACAuthentication]
    [RoutePrefix("api/ViTriKho")]
    public class ViTriKhoController : ApiController
    {
        ViTriKhoRepository _repo = new ViTriKhoRepository();
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string parameter)
        {
            return _repo.GetDatatable(action, parameter);
        }

        [HttpPost]
        [Route("PostViTriKho")]
        public string PostViTriKho(string action, string parameter, List<ViTriKhoEntity> lstSave)
        {
            string json = JsonConvert.SerializeObject(lstSave);
            DataTable tblViTriKho = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(action, parameter, tblViTriKho);
        }

        [HttpGet]
        [Route("GetPN_VTK")]
        public DataTable GetPN_VTK(string action, string parameter)
        {
            return _repo.GetPN_VTK(action, parameter);
        }

        [HttpPost]
        [Route("PostPN_VTK")]
        public string PostPN_VTK(string action, string parameter, List<ChiTietPhieuNhap_VTKEntity> lstSave)
        {
            string json = JsonConvert.SerializeObject(lstSave);
            DataTable tblPN_VTK = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostPN_VTK(action, parameter, tblPN_VTK);
        }

        [HttpDelete]
        [Route("DeleteViTriKho")]
        public string DeleteViTriKho(string maViTri)
        {
            return _repo.DeleteViTriKho(maViTri);
        }
        [HttpGet]
        [Route("GetKhoVT_MK")]
        public DataTable GetKhoVT_MK(string nhamay)
        {
            return new ViTriKhoModel().GetKhoVT_MK(nhamay);
        }
        [HttpGet]
        [Route("GetShowVTK")]
        public DataTable GetShowVTK()
        {
            return new ViTriKhoModel().GetShowVTK();
        }
        [HttpGet]
        [Route("GetNhaMay")]
        public DataTable GetNhaMay(string para1)
        {
            return new ViTriKhoModel().GetNhaMay(para1);
        }
        [HttpGet]
        [Route("GetSearch")]
        public DataTable GetSearch(string action, string parameter, string parameter2)
        {
            return new ViTriKhoModel().search_vitri(action, parameter, parameter2);
        }
    }
}
