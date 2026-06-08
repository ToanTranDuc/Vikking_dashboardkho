using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/KeHoachXuatHangThanhPham")]
    public class KeHoachXuatHangThanhPhamController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "")
        {
            return new KeHoachXuatHangThanhPhamModel().Get(action, para1, para2, para3, para4, para5, para6);
        }
        [HttpGet]
        [Route("PostXH")]
        public string PostXH(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "")
        {
            return new KeHoachXuatHangThanhPhamModel().PostXH(action, para1, para2, para3, para4, para5, para6);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action, DataTable tbl)
        {
            return new KeHoachXuatHangThanhPhamModel().Post(action, tbl);
        }
        [HttpGet]
        [Route("GetThanhPham")]
        public DataSet GetThanhPham(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "", string para6 = "")
        {
            return new KeHoachXuatHangThanhPhamModel().GetThanhPham(action, para1, para2, para3, para4, para5, para6);
        }
        [HttpPost]
        [Route("PostThungThanhPham")]
        public string PostThungThanhPham(string action, [FromBody] DataTable tbl)
        {
            return new KeHoachXuatHangThanhPhamModel().PostThungThanhPham(action, tbl);
        }

    }
}