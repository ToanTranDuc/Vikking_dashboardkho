using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaoCaoTongHop.Controllers
{
    public class ViTriKhoController : Controller
    {
        public ActionResult ViTriKho(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult NhapViTriThanhPham(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
        public ActionResult TongQuanKho()
        {
            return View();
        }
        public ActionResult ChiTietKho()
        {
            return View();
        }

        public ActionResult TongQuanKhoNPL()
        {
            return View();
        }
        public ActionResult NhapViTri(string userName)
        {
            ViewBag.userName = userName;
            return View();
        }
    }
}