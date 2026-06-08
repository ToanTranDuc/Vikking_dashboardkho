using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Model.ThuVien;
using NtbSoft.ERP.Web.Repository.R.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/QTY_MaHangThongSoDo")]
    public class QTY_MaHangThongSoDoController : ApiController
    { 

        #region Thông Số
        [HttpGet]
        [Route("GetDH")]
        public DataTable GetDH(string action,string para1 = "",string para2="")
        {
            return new QTY_MaHang_ThongSoDoModel().GetDH(action,para1, para2);
        }
        [HttpGet]
        [Route("Get")]
        public DataTable GetDH(string action, string maLenh, string styleID, string status, string season, string sizeTypeID, string rap,string poid = "")
        {
            return new QTY_MaHang_ThongSoDoModel().Get(action, maLenh, styleID, status,season,sizeTypeID,rap, poid);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(string action, DataTable tbl)
        {
            return new QTY_MaHang_ThongSoDoModel().Post(action, tbl);
        }
        #endregion
        #region Công đoạn IE
        [HttpGet]
        [Route("GetCongDoanIE")]
        public DataTable GetCongDoanIE(string action, string para, string para1, string para2, string para3)
        {
            return new QTY_MaHang_ThongSoDoModel().GetCongDoanIE(action, para, para1, para2, para3);
        }

        [HttpPost]
        [Route("PostCongDoanIE")]
        public string PostCongDoanIE(string action, DataTable tbl)
        {
            return new QTY_MaHang_ThongSoDoModel().PostCongDoanIE(action, tbl);
        }
        #endregion
        #region Cài đặt thông số - season
        [HttpGet]
        [Route("GetMaHang_Season")]
        public DataTable GetMaHang_Season(string action, string maHang, string sizeTypeID, string season, string version)
        {
            return new QTY_MaHang_ThongSoDoModel().GetMaHang_Season(action, maHang, sizeTypeID, season, version);
        }

        [HttpPost]
        [Route("PostMaHang_Season")]
        public string PostMaHang_Season(string action, DataTable tbl)
        {
            return new QTY_MaHang_ThongSoDoModel().PostMaHang_Season(action, tbl);
        }
        #endregion
        #region Thông số - Ghi chú
        [HttpGet]
        [Route("GetGhiChu")]
        public DataTable GetGhiChu(string action, string para1 = "", string para2 = "", string para3 = "")
        {
            return new QTY_MaHang_ThongSoDoModel().GetGhiChu(action, para1, para2, para3);
        }
        [HttpPost]
        [Route("PostGhiChu")]
        public string PostGhiChu(string action, DataTable tbl)
        {
            return new QTY_MaHang_ThongSoDoModel().PostGhiChu(action, tbl);
        }
        #endregion
    }
}