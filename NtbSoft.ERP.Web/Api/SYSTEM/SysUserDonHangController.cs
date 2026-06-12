using Newtonsoft.Json;
using NtbSoft.ERP.Model.SYSTEM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.SYSTEM
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/PhanQuyenDH")]
    public class SysUserDonHangController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string userID)
        {
            return new SysUserDonHangModel().Get(userID);
        }
        [HttpGet]
        [Route("GetUser")]
        public DataTable GetUser(string userId)
        {
            return new SysUserDonHangModel().GetUser(userId);
        }
        [HttpGet]
        [Route("GetUserNV")]
        public DataTable GetUserNV()
        {
            return new SysUserDonHangModel().GetUserNV();
        }
        //[HttpGet]
        //[Route("GetNhanVien")]
        //public DataTable GetNhanVien()
        //{
        //    return new SysUserDonHangModel().GetNhanVien();
        //}
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable ojData, string userId, string ModuleId, bool allow, string trangthai)
        {
            return new SysUserDonHangModel().Post(ojData, userId, ModuleId, allow, trangthai);
        }
        [HttpPost]
        [Route("PostNV")]
        public string PostNV(DataTable ojData, string userId, string manv)
        {
            return new SysUserDonHangModel().PostNV(ojData, userId, manv);
        }
        [HttpPost]
        [Route("GONV")]
        public string GONV(object ojData, string userId, string manv)
        {
            string json = JsonConvert.SerializeObject(ojData);
            DataTable tbls = JsonConvert.DeserializeObject<DataTable>(json);
            return new SysUserDonHangModel().GONV(tbls, userId, manv);
        }
    }
}