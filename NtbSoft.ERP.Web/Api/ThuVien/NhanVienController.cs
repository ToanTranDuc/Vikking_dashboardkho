using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Web.Repository.R.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [RoutePrefix("api/NhanVien")]
    public class NhanVienController : ApiController
    {
        INhanVienRepository _repo = new NhanVienRepository();
        [HttpGet]
        [Route("GetAllNV")]
        public DataTable GetAllNV()
        {
            return _repo.GetAllNV();
        }

        [HttpGet]
        [Route("GetNV")]
        public DataTable GetNV(string manv)
        {
            return _repo.GetNV(manv);
        }


        [HttpPost]
        [Route("PostNV")]
        public string PostNV(object ojBangSize)
        {
            if (ojBangSize == null) return "false";
            string json = JsonConvert.SerializeObject(ojBangSize);
            DataTable tbnv = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostNV(tbnv);
        }

        [HttpDelete]
        [Route("DeleteNV")]
        public string DeleteNV(string manv)
        {
            return _repo.DeleteNV(manv);
        }

        [HttpPost]
        [Route("UploadImage")]
        public async Task<object> UploadImage()
        {
            try
            {
                var file = await Request.Content.ReadAsByteArrayAsync();
                var action = Request.Headers.GetValues("action").FirstOrDefault();
                var fileName = Request.Headers.GetValues("fileName").FirstOrDefault();
                string uri = HttpContext.Current.Server.MapPath(@"\Images\NhanVien");
                if (!Directory.Exists(uri))
                    Directory.CreateDirectory(uri);

                // Ghép đường dẫn đầy đủ gồm thư mục + tên file
                string sourceImage = Path.Combine(uri, fileName);

                if (file != null)
                {
                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream(file))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(stream);


                        if (!System.IO.File.Exists(sourceImage))
                            image.Save(sourceImage);
                    }
                }
                // Lưu file
                //File.WriteAllBytes(sourceImage, file);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
