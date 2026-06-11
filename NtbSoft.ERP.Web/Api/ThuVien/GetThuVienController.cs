using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/GetThuVien")]
    public class GetThuVienController : ApiController
    {
        GetThuVienModel _model = new GetThuVienModel();
        [HttpGet]
        [Route("GetThuVienDaDung")]
        public List<DataTable> GetThuVienDaDung()
        {
            return _model.GetThuVienDaDung();
        }
    }
}