using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Web.Repository.R.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.SYSTEM
{
    [RoutePrefix("api/PhanQuyenExcelTS")]
    public class SysUserExcelTSController : ApiController
    {
        ISystemUserExcelTSRepository _repo = new SystemUserExcelTSRepository();
        [HttpGet]
        [Route("Get")]
        public List<SystemUserExcelTSModel> Get(string userID)
        {
            return _repo.Get(userID);
        }
        [HttpGet]
        [Route("GetUser")]
        public List<SystemUserExcelTSConfigViewModel> GetUser(string userID)
        {
            return _repo.GetUser(userID);
        }
        [HttpPost]
        [Route("Post")]
        public string PostNX(List<SystemUserExcelTSConfigViewModel> items)
        {
            return _repo.Post(items);
        }
    }
}