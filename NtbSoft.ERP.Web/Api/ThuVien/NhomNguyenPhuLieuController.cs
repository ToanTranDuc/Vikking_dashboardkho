using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Web.Http;
using System.Data;
using NtbSoft.ERP.Model.ThuVien;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/NhomNPL")]
    public class NhomNguyenPhuLieuController : ApiController
    {
        NhomNguyenPhuLieuModel _model = new NhomNguyenPhuLieuModel();
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return _model.Get();
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable tblSave, string para="")
        {
            return _model.Post(tblSave, para);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string id, string user)
        {
            return _model.Delete(id, user);
        }

        [HttpPost]
        [Route("UpdateSort")]
        public string UpdateSort(DataTable tblSave)
        {
            return _model.UpdateSort(tblSave);
        }
        [HttpGet]
        [Route("GetCheck")]
        public DataTable GetCheck(string manhom)
        {
            return _model.GetCheck(manhom);
        }
        [HttpGet]
        [Route("GetChung")]
        public DataTable GetChung(string action, string para = "", string para2 = "", string para3 = "")
        {
            return  _model.GetChung(action, para, para2, para3);
        }
    }
}