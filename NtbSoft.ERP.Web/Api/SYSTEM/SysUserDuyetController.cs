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
    [RoutePrefix("api/PhanQuyenDUYET")]
    public class SysUserDuyetController : ApiController
    {
        ISystemUserDuyetRepository _repo = new SystemUserDuyetRepository();
        [HttpGet]
        [Route("Get")]
        public List<SystemUserDuyetViewModel> Get(string userID)
        {
            return _repo.Get(userID);
        }
        [HttpGet]
        [Route("GetUser")]
        public List<SystemUserDuyetConfigViewModel> GetUser(string userID)
        {
            return _repo.GetUser(userID);
        }
        [HttpPost]
        [Route("Post")]
        public string PostNX(List<SystemUserDuyetConfigViewModel> items)
        {
            return _repo.Post(items);
        }
    }
}