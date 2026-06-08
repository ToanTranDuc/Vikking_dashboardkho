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


namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/Dic_ShippingXuatHang")]
    public class Dic_ShippingXuatHangController : ApiController
    {
        Dic_ShippingModel _model = new Dic_ShippingModel();
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action,string para1,string para2)
        {
            return _model.Get(action, para1, para2);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable tblSave)
        {
            return _model.Post(tblSave);
        }
        [HttpGet]
        [Route("Delete")]
        public string Delete(string para1)
        {
            return _model.Delete(para1);
        }
    }
}
