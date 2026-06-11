using System.Web.Http;
using System.Threading.Tasks;
using System.Data;
using NtbSoft.ERP.Model.POMH;
using NtbSoft.ERP.Entity.POMH;
using Newtonsoft.Json;
using System.Collections.Generic;
using NtbSoft.ERP.Model.QuanLyDonHang;
using System.Linq;
using System;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/ERP_ViewLenhSXChanged")]
    public class ERP_ViewLenhSXChangedController : ApiController
    {

        private ERP_ViewLenhSXChangedModel _model = new ERP_ViewLenhSXChangedModel();

        [HttpGet]
        [Route("GET")]
        public async Task<DataTable> GET(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            return await _model.Get(action, para1, para2, para3, para4, para5);
        }
       
    }
}