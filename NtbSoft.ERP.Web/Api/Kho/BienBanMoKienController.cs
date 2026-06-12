using NtbSoft.ERP.Model.Kho;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.Kho
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/BienBanMoKien")]
    public class BienBanMoKienController : ApiController
    {
        [HttpGet]
        [Route("GetSoLo")]
        public DataTable GetSoLo()
        {
            return new BienBanMoKienModel().GetSoLo();
        }
        [HttpGet]
        [Route("GetSoLot")]
        public DataTable GetSoLot()
        {
            return new BienBanMoKienModel().GetSoLot();
        }
        [HttpGet]
        [Route("GetCay")]
        public DataTable GetCay()
        {
            return new BienBanMoKienModel().GetCay();
        }
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string fromDate, string toDate, string solo, string solot)
        {
            return new BienBanMoKienModel().Get(fromDate, toDate, solo, solot);
        }
        [HttpGet]
        [Route("GetNoDate")]
        public DataTable GetNoDate( string solo, string solot)
        {
            return new BienBanMoKienModel().GetNoDate(solo, solot);
        }
        [HttpGet]
        [Route("GetSoLoExcel")]
        public DataTable GetSoLoExcel(int npl)
        {
            return new BienBanMoKienModel().GetSoLoExcel(npl);
        }
        [HttpGet]
        [Route("GetMaHangExcel")]
        public DataTable GetMaHangExcel(string solo, int npl)
        {
            return new BienBanMoKienModel().GetMaHangExcel(solo, npl);
        }
        [HttpGet]
        [Route("GetExcel")]
        public DataTable GetExcel(string solo, string mahang, int npl)
        {
            return new BienBanMoKienModel().GetExcel(solo, mahang, npl);
        }
    }
}