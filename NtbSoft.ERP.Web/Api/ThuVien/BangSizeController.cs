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
    [RoutePrefix("api/BangSize")]
    public class BangSizeController : ApiController
    {
        IBangSizeRepository _repo = new BangSizeRepository();
        [HttpGet]
        [Route("GetBangSize")]
        public DataTable GetBangSize()

        {
            return _repo.GetBangSize();
        }

        [HttpPost]
        [Route("PostBangSize")]
        public string PostBangSize(List<BangSizeEntity> ojBangSize)
        {
            if (ojBangSize == null) return "false";
            string json = JsonConvert.SerializeObject(ojBangSize);
            DataTable tbBangSize = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tbBangSize);
        }

        [HttpPost]
        [Route("PostAutoImportBangSize")]
        public string PostAutoImportBangSize(List<BangSizeEntity> ojBangSize)
        {
            if (ojBangSize == null) return "false";
            string json = JsonConvert.SerializeObject(ojBangSize);
            DataTable tbBangSize = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostAutoImport(tbBangSize);
        }
        [HttpDelete]
        [Route("DeleteBangSize")]
        public string DeleteBangSize(int parameter)
        {
            return _repo.DeleteBangSize(parameter);
        }

        [HttpGet]
        [Route("GetBangSize2")]
        public DataTable GetBangSize(string para, string para2)
        {
            return new BangSizeModel().GetBangSize2(para, para2);
        }
        #region quan
        [HttpDelete]
        [Route("DeleteNhomBangSize")]
        public string DeleteNhomBangSize(string manhomsize, string mahang, string makh)
        {
            return new BangSizeModel().DeleteNhomBangSize(manhomsize, mahang, makh);
        }
        #endregion
        [HttpGet]
        [Route("GetBangSizeMH")]
        public DataTable GetBangSizeMH(string makh)

        {
            return _repo.GetBangSizeMH(makh);
        }
        [HttpGet]
        [Route("GetBangSize3")]
        public DataTable GetBangSize2(string para, string para2, string para1)
        {
            return new BangSizeModel().GetBangSize3(para, para2, para1);
        }
        [HttpGet]
        [Route("GetBangSizebom")]
        public DataTable GetBangSizebom(string para, string para1)
        {
            return new BangSizeModel().GetBangSizeBom(para,para1);
        }
    }
}