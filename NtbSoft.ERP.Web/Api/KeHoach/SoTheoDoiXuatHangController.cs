using Newtonsoft.Json;
using NtbSoft.ERP.Entity.KeHoach;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Model.KeHoach;
using NtbSoft.ERP.Web.Models.THIETBI;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [RoutePrefix("api/SoTheoDoiXuatHangController")]
    public class SoTheoDoiXuatHangController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1, string Para2, string Para3, string Para4)
        {
            return new SoTheoDoiXuatHangModel().Get(action, Para1, Para2, Para3, Para4);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable tbl)
        {
            return new SoTheoDoiXuatHangModel().Post(tbl);
        }
    }
}