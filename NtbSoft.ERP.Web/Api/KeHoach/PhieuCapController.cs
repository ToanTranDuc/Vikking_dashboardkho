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
    [RoutePrefix("api/PhieuCap")]
    public class PhieuCapController : ApiController
    {
        [HttpGet]
        [Route("GetData")]
        public DataTable GetData(string action, string maphieu, string malenhsx)
        {
            return PhieuCapVatTu_Model.GetData(action, maphieu, malenhsx);
        }
        [HttpPost]
        [Route("GetDataDB")]
        public DataTable GetData(DataTable ChiTiet)
        {
            string action = ChiTiet.Rows[0]["action"].ToString();
            string maphieu = ChiTiet.Rows[0]["maphieu"].ToString();
            string malenhsx = ChiTiet.Rows[0]["malenhsx"].ToString();
            return PhieuCapVatTu_Model.GetData(action, maphieu, malenhsx);
        }
        [HttpPost]
        [Route("PostData")]
        public string PostData(string maphieu, string malenhsx, string nguoitao, string thietbi, string ghichu, DataTable ChiTiet)
        {
            return PhieuCapVatTu_Model.PostData(maphieu, malenhsx, nguoitao, thietbi, ghichu, ChiTiet);
        }
    }
}