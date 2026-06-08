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
    [RoutePrefix("api/NguyenPL")]
    public class NguyenPhuLieuController : ApiController
    {
        INguyenPLRepository _repo = new NguyenPLRepository();
        [HttpGet]
        [Route("GetNPL")]
        public DataTable GetNPLet()
        {
            return new NguyenPLModel().GetNPL();
        }
        [HttpPost]
        [Route("PostNPL")]
        public string PostNPL(DataTable item)
        {
            return _repo.Post(item);
        }
        [HttpDelete]
        [Route("DeleteNPL")]
        public string DeleteNPL(string manpl)
        {
            return new NguyenPLModel().DeleteNPL(manpl);
        }

    }
}