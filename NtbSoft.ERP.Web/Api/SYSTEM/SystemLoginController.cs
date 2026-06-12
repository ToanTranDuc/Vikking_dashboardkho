using Newtonsoft.Json;
using NtbSoft.ERP.Entity.KeHoach;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Model.KeHoach;
using NtbSoft.ERP.Model.SYSTEM;
using NtbSoft.ERP.Web.Filter;
using NtbSoft.ERP.Web.Models.SYSTEM;
using NtbSoft.ERP.Web.Repository.R.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls.WebParts;

namespace NtbSoft.ERP.Web.Api.SYSTEM
{
    //[HMACAuthentication]
    [RoutePrefix("api/Login")]
    public class SystemLoginController : ApiController
    {
        ISystemLogRepository _repo = new SystemLogRepository();

        [HttpGet]
        [Route("Get")]
        public string Get(string para1 , string para2)
        {
            if (para1 == "" || para2 == "") return null;
            DataTable dtTable = new SystemLoginModel().Get();
            string json = JsonConvert.SerializeObject(dtTable);
            List<Login> login = JsonConvert.DeserializeObject<List<Login>>(json);
            Login user = login.Where(x => x.UserName.ToLower() == para1.Trim().ToLower()).FirstOrDefault();
            if (user == null) return "Tên đăng nhập không đúng";
            else
            {
                MD5StringCrypt Md5Crypt = new MD5StringCrypt();
                string deCrypt = Md5Crypt.Decrypt(user.Password,true);
                if (deCrypt == para2) return "true";
                else return null;
            }
        }
        [HttpGet]
        [Route("GetUser")]
        public DataTable GetUser(string para1)
        {
            return new SystemUserModuleModel().GetPhanQuyenUserWeb(para1);
        }
        [HttpGet]
        [Route("GetUserDashBoard")]
        public DataTable GetUserDashBoard(string para1)
        {
            return new SystemUserModuleModel().GetPhanQuyenUserWebDashBoard(para1);
        }
    }
}
