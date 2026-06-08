using NtbSoft.ERP.Model.POMuaHang;
using NtbSoft.ERP.Model.SYSTEM;
using NtbSoft.ERP.Web.Models.SYSTEM;
using NtbSoft.ERP.Web.Repository.R.SYSTEM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.SYSTEM
{
    [RoutePrefix("api/PhanQuyenPOMH")]
    public class SysUserPOMHController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string userID)
        {
            return new PhanQuyenPOMHModel().Get(userID);
        }
        [HttpGet]
        [Route("GetUser")]
        public DataTable GetUser(string userId)
        {
            return new PhanQuyenPOMHModel().GetUser(userId);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable tbl)
        {
            return new PhanQuyenPOMHModel().Post(tbl) ;
        }

        [HttpGet]
        [Route("Getweb")]
        public DataTable GetWEB(string userID)
        {
            return new PhanQuyenPOMHModel().GetWEB(userID);
        }
        [HttpPost]
        [Route("Postweb")]
        public string PostWEB(DataTable tbl)
        {
            return new PhanQuyenPOMHModel().PostWEB(tbl);
        }
    }
}