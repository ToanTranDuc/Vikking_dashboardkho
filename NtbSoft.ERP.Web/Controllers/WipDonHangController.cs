using NtbSoft.ERP.Model.WipDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NtbSoft.ERP.Web.Controllers
{
    public class WipDonHangController : Controller
    {
        // GET: WipDonHang
        public ActionResult Index(string userName)
        {
            DataTable dt = WipDonHangModel.ExecStoredProcedure("sp_wip_color", new SqlParameter[] { new SqlParameter("@Action", "get") });
            var colorDict = new Dictionary<string, string>();
            foreach (DataRow row in dt.Rows)
            {
                colorDict[row["CssClass"].ToString()] = row["ColorCode"].ToString();
            }

            // Quăng xuống View
            ViewBag.ColorConfig = Newtonsoft.Json.JsonConvert.SerializeObject(colorDict);
            ViewBag.userName = userName;
            ViewBag.IsWinHost = string.Equals(Request.QueryString["host"], "win", StringComparison.OrdinalIgnoreCase);
            return View();
        }
    }
}
