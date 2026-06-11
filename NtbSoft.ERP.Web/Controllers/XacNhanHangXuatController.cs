using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaoCaoTongHop.Controllers
{
    public class XacNhanHangXuatController : Controller
    {

        public ActionResult index(string userName, string para1, string para2)
        {
            ViewBag.para1 = para1;
            ViewBag.para2 = para2;
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult XacNhanPhuLieu(string userName, string para1, string para2)
        {
            ViewBag.para1 = para1;
            ViewBag.para2 = para2;
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult XacNhanXuatHangCapThem(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult XacNhanCapThemPhuLieu(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
    }
}