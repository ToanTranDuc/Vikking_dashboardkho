using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class SoDoKhoController : Controller
    {
        // GET: SoDoKho
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SoDoTong(string dvsx, string tenkho, string tenkhuvuc, string username)
        {
            var model = new
            {
                dvsx = dvsx,
                tenkho = tenkho,
                tenkhuvuc = tenkhuvuc,
                username = username,
            };
            return View(model);
        }
        public ActionResult SoDo(string dvsx, string tenkho, string tenkhuvuc,string tenke,string tentang,string teno,string indexvitri,string username)
        {
            var model = new
            {
                dvsx = dvsx,
                tenkho = tenkho,
                tenkhuvuc = tenkhuvuc,
                tenke= tenke,
                tentang = tentang,
                teno = teno,
                indexvitri = indexvitri,
                username = username
            };
            return View(model);
        }
        public ActionResult SoDoGiaCong(string dvsx, string tenkho, string tenkhuvuc, string tenke, string tentang, string teno, string indexvitri, string username)
        {
            var model = new
            {
                dvsx = dvsx,
                tenkho = tenkho,
                tenkhuvuc = tenkhuvuc,
                tenke = tenke,
                tentang = tentang,
                teno = teno,
                indexvitri = indexvitri,
                username = username
            };
            return View(model);
        }
        public ActionResult QRcode()
        {
            return View();
        }
       
    }
    
}
    