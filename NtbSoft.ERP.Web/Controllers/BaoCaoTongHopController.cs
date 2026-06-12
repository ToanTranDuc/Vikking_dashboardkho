using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaoCaoTongHop.Controllers
{
    public class BaoCaoTongHopController : Controller
    {
        public ActionResult NangSuatDoanhThu()
        {
            return View();
        }
        public ActionResult BaoCaoTienDo(string username, string dvsx)
        {
            ViewBag.Username = username == null ? "web" : username;
            ViewBag.Dvsx = dvsx == null ? "web" : dvsx;
            return View();
        }
        public ActionResult ChatLuongMaHangTrenTungChuyen()
        {
            return View();
        }
        public ActionResult ChatLuongMaHangSauUi()
        {
            return View();
        }
        public ActionResult BaoCaoDongThung(string username, string dvsx)
        {
            ViewBag.Username = username == null ? "web" : username;
            ViewBag.Dvsx = dvsx == null ? "web" : dvsx;
            return View();
        }
        public ActionResult BaoCaoNhapKho()
        {
            return View();
        }

        public ActionResult KeHoachCatNgay()
        {
            return View();
        }
        public ActionResult MHLoiTrenTungChuyenMay()
        {
            return View();
        }
        public ActionResult BCThoiGianHaoPhi()
        {
            return View();
        }

        public ActionResult BCKeHoachCatNgay()
        {
            return View();
        }
        public ActionResult BCNguonNhanLuc()
        {
            return View();
        }
        public ActionResult AllBaoCao()
        {
            return View();
        }

        public ActionResult BCCapThemVaThuHoiNPL()
        {
            return View();
        }
       

    }
}