using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class POMuaHangController : Controller
    {
        /*Đánh giá nhà cung cấp BM03*/
        public ActionResult TongQuanDanhGia()
        {
            return View();
        }
        public ActionResult PhieuDanhGia(string ActionDG = "add", string MaPhieu = "NONE", string MaCLVTID = "")
        {
            ViewBag.ActionDG = ActionDG;
            ViewBag.MaPhieu = MaPhieu;
            ViewBag.MaCLVTID = MaCLVTID;
            return View();
        }
        /*Theo dõi nhà cung cấp BM04*/
        
        public ActionResult PhieuTheoDoi(string ActionTheoDoi = "add", string MaPhieu = "NONE", string MaCLVTID = "")
        {

            ViewBag.ActionTheoDoi = ActionTheoDoi;
            ViewBag.MaPhieu = MaPhieu;
            ViewBag.MaCLVTID = MaCLVTID;
            return View();
        }
        public ActionResult TongQuanPhieuTheoDoi()
        {

            return View();
        }
        /* Đánh giá nhà cung cấp tai nơi làm việc BM13*/
        public ActionResult TongQuanDanhGiaNoiLamViec()
        {
            return View();
        }
        public ActionResult PhieunDanhGiaNoiLamViec(
       string actionType = "add",
       string MaPhieu = "NONE",
       string MaNhaCC = "",
       string TenPhieu = "",
       string NguoiTao = "")
        {

            ViewBag.ActionType = actionType;
            ViewBag.MaPhieu = MaPhieu;
            ViewBag.MaNhaCC = MaNhaCC;
            ViewBag.TenPhieu = TenPhieu;
            ViewBag.NguoiTao = NguoiTao;
            return View();
        }


        public ActionResult BaoCaoTongHopNCC()
        {
            return View();
        }
    }
}