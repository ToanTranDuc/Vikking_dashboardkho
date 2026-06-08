using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Web.Repository.R.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.SYSTEM
{
    [RoutePrefix("api/PhanQuyenCosting")]
    public class SysUserCostingController : ApiController
    {
        ISystemUserCostingRepository _repo = new SystemUserCostingRepository();
        [HttpGet]
        [Route("Get")]
        public List<SystemUserCostingEntity> Get(string userID)
        {
            return _repo.Get(userID);
        }
        [HttpGet]
        [Route("GetUser")]
        public List<SystemUserCostingConfigViewEntity> GetUser(string userID)
        {
            return _repo.GetUser(userID);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(List<SystemUserCostingConfigViewEntity> items)
        {
            return _repo.Post(items);
        }
    }
}