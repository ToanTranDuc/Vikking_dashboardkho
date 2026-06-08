using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Web.Repository.R.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.SYSTEM
{
    [RoutePrefix("api/PhanQuyenXacNhanBOM")]
    public class SysUserXacNhanBOMController : ApiController
    {
        ISystemUserXacNhanRepository _repo = new SystemUserXacNhanRepository();
        [HttpGet]
        [Route("Get")]
        public List<SystemUserXacNhanBomModel> Get(string userID)
        {
            return _repo.Get(userID);
        }
        [HttpGet]
        [Route("GetUser")]
        public List<SystemUserXacNhanBomConfigViewModel> GetUser(string userID)
        {
            return _repo.GetUser(userID);
        }
        [HttpPost]
        [Route("Post")]
        public string PostNX(List<SystemUserXacNhanBomConfigViewModel> items)
        {
            return _repo.Post(items);
        }
    }
}