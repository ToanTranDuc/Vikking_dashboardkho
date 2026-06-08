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
    [RoutePrefix("api/BaoCaoKhoNPL")]
    public class BaoCaoTongHopKhoNPLController : ApiController
    {
        [HttpGet]
        [Route("GetSoLo")]
        public DataTable GetSoLo()
        {
            return new BaoCaoTongHopKhoNPLModel().GetSoLo();
        }  
        [HttpGet]
        [Route("GetCayVai")]
        public DataTable GetCayVai(string soloid)
        {
            return new BaoCaoTongHopKhoNPLModel().GetCayVai(soloid);
        }
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string soloid, string items)
        {
            return new BaoCaoTongHopKhoNPLModel().Get(soloid, items);
        }
        [HttpGet]
        [Route("GetChiTiet")]
        public DataTable GetChiTiet(string soloid, string manpl)
        {
            return new BaoCaoTongHopKhoNPLModel().GetChiTiet(soloid, manpl);
        }
    }
}