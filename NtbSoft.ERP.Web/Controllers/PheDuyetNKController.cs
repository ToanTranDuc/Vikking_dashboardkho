using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class PheDuyetNKController : Controller
    {
        public ActionResult PheDuyetNK()
        {
            return View("PheDuyetNK");
        }

        public ActionResult DuyetModule()
        {
            return RedirectToAction("PheDuyetNK");
        }
    }

    
}
