using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class DongThungController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NhapKho()
        {
            return View();
        }
    }
}
