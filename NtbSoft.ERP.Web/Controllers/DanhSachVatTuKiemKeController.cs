using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class DanhSachVatTuKiemKeController : Controller
    {
        public ActionResult Index(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }

        public ActionResult DanhSachVatTuKiemKePL(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult XacNhanKiemKeNL(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }

        public ActionResult XacNhanKiemKePL(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
    }
}