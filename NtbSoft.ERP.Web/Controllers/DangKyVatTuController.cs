using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class DangKyVatTuController : Controller
    {
        public ActionResult Index(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }

        public ActionResult DangKyVatTuPL(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult DangKyCapThemVT(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult DangKyCapThemVatTuPL(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
    }
}