using NtbSoft.ERP.Model.KeHoach;
using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/KeHoachDongThungTonKho")]
    public class KeHoachDongThungTonKhoController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string Action, string MaDH, string MaDVSX, string DotSX, string POID, string SizeTypeID, string ColorID, string ProductID, string SizeID,string MaPKL = "")
        {
            return new KeHoachDongThungTonKhoModel().Get(Action, MaDH, MaDVSX, DotSX, POID, SizeTypeID, ColorID, ProductID, SizeID, MaPKL);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action,DataTable dt)
        {
            return new KeHoachDongThungTonKhoModel().Post(action, dt);
        }
        [HttpPost]
        [Route("Delete")]
        public string Delete(string action, DataTable dt)
        {
            return new KeHoachDongThungTonKhoModel().Post(action, dt);
        }

    }
}