using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Model.ThuVien;
using NtbSoft.ERP.Web.Repository.R.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/PhapDanhCty")]
    public class PhapDanhCtyController : ApiController
    {
        IPhapDanhCtyRepository _repo = new PhapDanhCtyRepository();
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return _repo.Get();
        }

        [HttpPost]
        [Route("Post")]
        public string Post(List<PhapDanhCtyEntity> objSave)
        {
            if (objSave == null) return "false";
            string json = JsonConvert.SerializeObject(objSave);
            DataTable tblSave = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.Post(tblSave);
        }

        [HttpDelete]
        [Route("Delete")]
        public string Delete(string parameter)
        {
            return _repo.Delete(parameter);
        }
        [HttpPost]
        [Route("PostImageInFolder")]
        public string PostFolder(dynamic data)
        {
            string TenHinh = data.TenHinh.ToString();

            return new PhapDanhCtyModel().UpdateImage("PostImage", TenHinh);
        }
        [HttpGet]
        [Route("GetImage")]
        public string GetImage()
        {
            DataTable tbl = new PhapDanhCtyModel().GetImage();
            string tenImage = tbl.Rows[0]["ImageLoGo"].ToString();
            return tenImage;
        }
    }
}