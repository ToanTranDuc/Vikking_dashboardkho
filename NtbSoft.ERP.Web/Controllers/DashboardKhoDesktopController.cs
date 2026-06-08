using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class DashboardKhoDesktopController : Controller
    {
        /// <summary>
        /// Desktop Dashboard View
        /// Route: /DashboardKhoDesktop/Index
        /// View: Views/DashboardKhoDesktop/DashboardKhoDesktop.cshtml
        /// </summary>
        public ActionResult Index()
        {
            return View("DashboardKhoDesktop");
        }
    }
}
