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
    [RoutePrefix("api/QTY_MaHangTaiLieuKiThuat")]
    public class QTY_MaHangTaiLieuKiThuatController : ApiController
    { 

        #region Tài liệu
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action,string para1 = "")
        {
            return new QTY_MaHang_TaiLieuKiThuatModel().Get(action,para1);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(string action, DataTable tbl)
        {
            return new QTY_MaHang_TaiLieuKiThuatModel().Post(action, tbl);
        }        
        #endregion
    }
}