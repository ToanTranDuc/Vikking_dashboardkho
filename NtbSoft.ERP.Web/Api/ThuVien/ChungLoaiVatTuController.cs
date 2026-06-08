using NtbSoft.ERP.Model.ThuVien;
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
    [RoutePrefix("api/ChungLoaiVatTu")]
    public class ChungLoaiVatTuController : ApiController
    {
       
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return new ChungLoaiVatTuModel().Get();
        }
        [HttpGet]
        [Route("CheckChungLoaiVT")]
        public DataTable CheckChungLoaiVT(string MaCLVT)
        {
            return new ChungLoaiVatTuModel().Get(MaCLVT);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable dt)
        {
            return new ChungLoaiVatTuModel().Post(dt);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(int id)
        {
            return new ChungLoaiVatTuModel().Delete(id);
        }
    }
}