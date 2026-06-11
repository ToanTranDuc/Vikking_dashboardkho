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
    [RoutePrefix("api/LenghtUnitConvert")]
    public class LenghtUnitConvertController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return LenghtUnitConvertModel.getHeQuyChieu();
        }
    }
}