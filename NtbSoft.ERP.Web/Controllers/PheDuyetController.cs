using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class PheDuyetController : Controller
    {
        // GET: PheDuyet
        public ActionResult DuyetModule(string userName = "")
        {
            ViewBag.userName = userName;
            return View();
        }
    }
}