using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers.Qty
{

    public class QtyKiemPLController : Controller
    {
        // GET: QtyKiemPL
        public ActionResult PhieuKiem(string userName = "", string soLoID = "", string soLo = "", string maNPL = "")
        {
            ViewBag.userName = userName;
            ViewBag.soLoID = soLoID;
            ViewBag.soLo = soLo;
            ViewBag.maNPL = maNPL;
            return View();
        }

        public ActionResult BCKiemPL(string userName = "")
        {
            return View();
        }

        public ActionResult BCKPLNgay(string userName = "")
        {
            return View();
        }
    }

  
}