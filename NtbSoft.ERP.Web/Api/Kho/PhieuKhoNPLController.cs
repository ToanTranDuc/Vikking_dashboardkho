using Newtonsoft.Json;
using NtbSoft.ERP.Model.Kho;
using NtbSoft.ERP.Model.ThuVien;

using OfficeOpenXml;

using OfficeOpenXml.Drawing;

using OfficeOpenXml.Style;

using System;

using System.Collections.Generic;

using System.Data;

using System.Drawing;

using System.IO;

using System.Linq;

using System.Net;

using System.Net.Http;

using System.Web;

using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.Kho

{

    [RoutePrefix("api/PhieuKhoNPL")]

    public class PhieuKhoNPLController : ApiController

    {

        [HttpGet]
        [Route("GetTheKhoNPL")]
        public DataTable GetTheKhoNPL(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "",string para7 ="", string para8 = "", string para9 = "")
        {
            return new PhieuKhoNPLModel().GetTheKhoNPL(action, para1, para2, para3, para4, para5, para6, para7,para8,para9);
        }



    }

}