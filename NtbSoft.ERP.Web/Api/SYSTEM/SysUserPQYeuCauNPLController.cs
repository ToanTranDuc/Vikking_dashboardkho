using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Web.Repository.R.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.SYSTEM
{
    [RoutePrefix("api/PhanQuyenYCNPL")]
    public class SysUserPQYeuCauNPLController : ApiController
    {
        ISystemUserPQYeuCauNPLRepository _repo = new SystemUserPQYeuCauNPLRepository();
        [HttpGet]
        [Route("Get")]
        public List<SystemUserYeuCauNPLEntity> Get(string userID)
        {
            return _repo.Get(userID);
        }
        [HttpGet]
        [Route("GetUser")]
        public List<SystemUserYeuCauNPLConfigViewEntity> GetUser(string userID)
        {
            return _repo.GetUser(userID);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(List<SystemUserYeuCauNPLConfigViewEntity> items)
        {
            return _repo.Post(items);
        }
    }
}