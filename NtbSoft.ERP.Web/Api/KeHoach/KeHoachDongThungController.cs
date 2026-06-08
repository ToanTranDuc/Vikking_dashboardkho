using NtbSoft.ERP.Model.KeHoach;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [RoutePrefix("api/KeHoachDongThung")]
    public class KeHoachDongThungController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string Action, string MaDH, string MaDVSX, string DotSX, string POID, string SizeTypeID, string ColorID, string ProductID, string SizeID, string MaPKL = "")
        {
            return new KeHoachDongThungModel().Get(Action, MaDH, MaDVSX, DotSX, POID, SizeTypeID, ColorID, ProductID, SizeID, MaPKL);
        }
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string Action, string MaDH, string POID, string SizeTypeID, string ColorID, string MaPKL = "")
        {
            return new KeHoachDongThungModel().GetV2(Action, MaDH, POID, SizeTypeID, ColorID, MaPKL);
        }
        [HttpPost]
        [Route("PostGetV2")]
        public DataTable PostGetV2(DataTable dt)
        {
            string Action = dt.Rows[0]["Action"].ToString();
            string MaDH = dt.Rows[0]["MaDH"].ToString();
            string POID = dt.Rows[0]["POID"].ToString();
            string SizeTypeID = dt.Rows[0]["SizeTypeID"].ToString();
            string ColorID = dt.Rows[0]["ColorID"].ToString();
            string MaPKL = dt.Rows[0]["MaPKL"].ToString();
            return new KeHoachDongThungModel().GetV2(Action, MaDH, POID, SizeTypeID, ColorID, MaPKL);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action, DataTable dt)
        {
            return new KeHoachDongThungModel().Post(action, dt);
        }
        [HttpPost]
        [Route("Delete")]
        public string Delete(string action, DataTable dt)
        {
            return new KeHoachDongThungModel().Post(action, dt);
        }

        [HttpPost]
        [Route("CopyPaste")]
        public string CopyPaste(string action, string MaDHDes, string MaPKL, DataSet ds)
        {
            return new KeHoachDongThungModel().CopyPaste(action, MaDHDes, MaPKL, ds.Tables[0], ds.Tables[1]);
        }
        [HttpPost]
        [Route("CancelCopyPaste")]
        public string CopyPaste(string action, string MaDH, string POID)
        {
            return new KeHoachDongThungModel().CancelCopyPaste(action, MaDH, POID);
        }
        [HttpGet]
        [Route("GetOption")]
        public async Task<DataTable> GetOption(string Action, string Para1 = "", string Para2 = "", string Para3 = "", string Para4 = "", string UserName = "",
            string status = "", string IsNhapKho = "", string IsLuanChuyen = "")
        {


            return await new KeHoachDongThungModel().Get(Action, Para1, Para2, Para3, Para4, UserName, status, IsNhapKho, IsLuanChuyen);
        }
        [HttpPost]
        [Route("PostGopPO")]
        public string PostGopPO(string action, DataTable dt)
        {
            return new KeHoachDongThungModel().PostGopPO(action, dt);
        }

    }
}