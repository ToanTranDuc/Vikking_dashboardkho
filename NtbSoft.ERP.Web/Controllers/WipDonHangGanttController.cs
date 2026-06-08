using System;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class WipDonHangGanttController : Controller
    {
        public ActionResult Index(string userName)
        {
            ViewBag.userName = userName;
            ViewBag.IsWinHost = string.Equals(Request.QueryString["host"], "win", StringComparison.OrdinalIgnoreCase);
            ViewBag.SkipLayoutHighcharts = true;
            return View();
        }
    }
}
