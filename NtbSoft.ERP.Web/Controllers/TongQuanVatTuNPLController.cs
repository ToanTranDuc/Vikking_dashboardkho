using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaoCaoTongHop.Controllers
{
    public class TongQuanVatTuNPLController : Controller
    {
       
        public ActionResult index(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult VatTuPhuLieu(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
    }
}