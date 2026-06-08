using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaoCaoTongHop.Controllers
{
    public class BienBanMoKienController : Controller
    {
        public ActionResult index(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult BienBanMoKienPL(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult BienBanMoKienNLV2(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult BienBanMoKienPLV2(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
    }
}