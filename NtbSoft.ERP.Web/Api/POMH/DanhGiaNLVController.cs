using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
using NtbSoft.ERP.Model.POMuaHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.POMuaHang
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/DanhGiaNLV")]
    public class DanhGiaNLVController : ApiController
    {

        [HttpPost]
        [Route("Get")]
        public DataTable Get(XuLyVTRequestGet req)

        {
            return new DanhGiaNLVModel().Get(req);
        }
        [HttpPost]
        [Route("GetByTypeTable")]
        public DataTable GetByTypeTable(XuLyVTRequestPost req)

        {
            return new DanhGiaNLVModel().GetByTypeTable(req);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(XuLyVTRequestPost req)

        {
            return new DanhGiaNLVModel().Post(req);
        }

        ////THU VIEN DANH GIA TQ
        ///
        [HttpGet]
        [Route("GetTQ")]
        public DataTable Get(string action, string para1, string para2, string para3, string para4, string para5)
        {
            return new DanhGiaNLVModel().GetTQ(action, para1, para2, para3, para4, para5);
        }

        [HttpPost]
        [Route("PostTQ")]
        public string Post([FromBody] PostTQRequest req)

        {
            return new DanhGiaNLVModel().PostTQ(
                    req.Action,
                    req.Tbl,
                    req.Tbl2,
                    req.Tbl3);
        }

        [HttpDelete]
        [Route("Delete")]
        public string Deleteloai(string action, string para1, string para2, string para3, string para4, string para5)
        {
            return new DanhGiaNLVModel().Delete(action, para1, para2, para3, para4, para5);
        }
    }
}