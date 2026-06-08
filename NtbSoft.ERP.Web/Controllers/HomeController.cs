using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class HomeController : Controller
    {
        //public ActionResult Index()
        //{
        //    ViewBag.Title = "Home Page";
        //    return View();
        //}
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return RedirectToAction("Index", "Login");
            //ViewBag.Title = "Home Page";
            //return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult TongQuan()
        {
            return View();
        }
        public ActionResult ReportList()
        {
            return View();
        }
    }
}
