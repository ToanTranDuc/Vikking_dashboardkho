using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class NhanHangGiaCongController : Controller
    {
       
        public ActionResult Index(string userName)
        {
            var model = new
            {
                userName = userName,
            };
            return View(model);
        }
        public ActionResult SoKien(string userName)
        {
            var model = new
            {
                userName = userName,
            };
            return View(model);
        }
    }
}
