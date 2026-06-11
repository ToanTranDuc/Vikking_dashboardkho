using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class NguyenPhuLieuController : Controller
    {
        public ActionResult Monitoring(string userName = "")
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult BCPhuLieu(string userName = "")
        {
            return View();
        }
        public ActionResult KiemVai(string userName = "")
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult BCKiemVai(string userName = "")
        {
            return View();
        }
        public ActionResult KiemVaiV2(string userName = "")
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult BCKiemVaiV2(string userName = "", string soLoID = "", string soLo = "", string maNPL = "")
        {
            ViewBag.userName = userName;
            ViewBag.soLoID = soLoID;
            ViewBag.soLo = soLo;
            ViewBag.maNPL = maNPL;
            return View();
        }
        public ActionResult GhiNhanKiemKePhuLieu(string userName = "")
        {
            ViewBag.userName = userName;
            return View();
        }

        public ActionResult GhiNhanKiemKeNL(string userName = "")
        {
            ViewBag.userName = userName;
            return View();
        }
    }
}
