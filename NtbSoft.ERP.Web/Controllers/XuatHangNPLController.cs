using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaoCaoTongHop.Controllers
{
    public class XuatHangNPLController : Controller
    {
       
        public ActionResult index(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult XuatHangPL(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
    }
}