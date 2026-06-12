using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SoTheoDoi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using NtbSoft.ERP.Model.Kho;

namespace NtbSoft.ERP.Web.Api.Kho
{
    [RoutePrefix("api/TheKhoThanhPham")]
    public class TheKhoThanhPhamController: ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable GetDSTheKhoThanhPham(string action,string para = "",string para2 = "",string para3 = "",string para4 = "",string para5 = "", string para6 = "", string para7 = "", string para8 = "")
        {
            return new TheKhoThanhPhamModel().Get(action, para, para2, para3, para4, para5, para6, para7, para8);
        }
    }
}