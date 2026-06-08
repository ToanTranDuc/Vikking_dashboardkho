using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class BienBanHangXuatController : Controller
    {
       
        public ActionResult Index(string userName,string PhieuBB)
        {
            var model = new
            {
                userName = userName,
                phieuBB = PhieuBB

            };
            return View(model);
        }
        public ActionResult IndexV2(string userName, string PhieuBB)
        {
            var model = new
            {
                userName = userName,
                phieuBB = PhieuBB

            };
            return View(model);
        }

    }
}
