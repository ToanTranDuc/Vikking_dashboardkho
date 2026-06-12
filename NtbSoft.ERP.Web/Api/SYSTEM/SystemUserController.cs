using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Api.Models.SYSTEM;
using NtbSoft.ERP.Api.Repository.Interface.SYSTEM;
using NtbSoft.ERP.Api.Repository.R.SYSTEM;
using NtbSoft.ERP.Web.Filter;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.SYSTEM
{
    //[HMACAuthentication]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/SystemUser")]
    public class SystemUserController : ApiController
    {
        ISystemUserRepository _repo = new SystemUserRepository();

        [HttpGet]
        [Route("Get")]
        public IEnumerable<SystemUserViewModel> Get()
        {
            return _repo.Get();
        }
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action)
        {
            return _repo.Get(action);
        }
        [HttpGet]
        [Route("GetKhoUser")]
        public DataTable GetKhoUser()
        {
            return _repo.GetKhoUser();
        }
        [HttpPost]
        [Route("")]
        public string Post(SystemUserViewModel item)
        {
            return _repo.Post(item);
        }
        [HttpDelete]
        [Route("{id}")]
        public string Delete(string id)
        {
            return _repo.Delete(id);
        }

        [HttpGet]
        [Route("Login")]
        public string Login(string userName, string password)
        {
            if (userName == null || password == null)
                return "null";
            List<SystemUserViewModel> lstUser = _repo.Get();
            SystemUserViewModel user = lstUser.Where(x => x.UserName.ToLower() == userName.Trim().ToLower()).FirstOrDefault();
            if (user == null)
                return "Tên đăng nhập hoặc mật khẩu không đúng";
            else
            {
                MD5StringCrypt Md5Crypt = new MD5StringCrypt();
                string deCrypt = Md5Crypt.Decrypt(user.Password, true);
                if (deCrypt == password) return "true";
                else return "Tên đăng nhập hoặc mật khẩu không đúng";
            }
        }
    }
}
