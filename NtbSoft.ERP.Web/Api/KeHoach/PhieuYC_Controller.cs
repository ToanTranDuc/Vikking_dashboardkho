using NtbSoft.ERP.Model.KeHoach;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [RoutePrefix("api/PhieuYeuCau")]
    public class PhieuYC_Controller : ApiController
    {
        [HttpGet]
        [Route("GetData")]
        public DataTable GetData(string action, string maphieu, string malenhsx)
        {
            return PhieuYC_Model.GetData(action, maphieu, malenhsx);
        }
        [HttpPost]
        [Route("PostData")]
        public string PostData(string maphieu, string malenhsx, string nguoitao,string thietbi, string ghichu, DataTable ChiTiet)
        {
            return PhieuYC_Model.PostData(maphieu, malenhsx, nguoitao, thietbi, ghichu, ChiTiet);
        }
    }
}