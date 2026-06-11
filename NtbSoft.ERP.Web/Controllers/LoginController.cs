
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Model.SYSTEM;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ActionResult Index(string User, string Password)
        {
            var model = new Account()
            {
                UserName = User ?? "",
                Password = Password ?? ""
            };
            Session.Clear();
            return View(model);
        }
        //[HttpPost]
        //public JsonResult Index(Account account)
        //{
        //    try
        //    {
        //        MD5StringCrypt Md5Crypt = new MD5StringCrypt();
        //        Session.Clear();
        //        var userName = account.UserName;
        //        var password = account.Password;

        //        if (userName == null || password == null)
        //            return Json(new { data = "" });

        //        DataTable lstUser = new SystemUserModel().GetUserModuleWeb("GetUser", userName);
        //        var tblUser = lstUser.AsEnumerable().Where(x => x["LoginID"].ToString().ToLower() == userName.Trim().ToLower());
        //        List<ModuleInfo> lstModule = new List<ModuleInfo>();
        //        foreach (DataRow dr in tblUser)
        //        {
        //            if (dr["ModuleQTYV2"] == null || dr["ModuleQTYV2"].ToString() == "") continue;
        //            lstModule.Add(new ModuleInfo() { 
        //                MaModule = Convert.ToInt16(dr["ModuleQTYV2"]),
        //                NhomModule = Convert.ToInt16(dr["NhomModule"])
        //            });
        //        }
        //        var user = lstUser.AsEnumerable().Where(x => x["LoginID"].ToString().ToLower() == userName.Trim().ToLower()).FirstOrDefault();
        //        if (user == null)
        //            return Json(new { data = "" });
        //        else
        //        {                   
        //            //CryptorEnginePro Md5Crypt = new CryptorEnginePro();
        //            string deCrypt = Md5Crypt.Decrypt(user["Password"].ToString(), true);
        //            if (deCrypt == password)
        //            {                       
        //                return Json(new { data = lstModule });
        //            }
        //            else
        //            {
        //                return Json(new { data = "" });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {               
        //        var model = new { };
        //        return Json(new { data = "" });
        //    }

        //}
        public class ModuleInfo
        {
            public int MaModule { get; set; }
            public int NhomModule { get; set; }
        }
    }
}