using NtbSoft.ERP.Model.ThuVien;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.ThuVien
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/ERPBangMau")]
    public class ERPBangMauController : ApiController
    {
        #region TheMau
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action,string para1 ="")
        {
            return new ERPBangMauModel().Get(action,para1);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action,DataTable tbl)
        {           
            return new ERPBangMauModel().Post(action, tbl);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string action,int id)
        {
            return new ERPBangMauModel().Delete(action, id);
        }
        [HttpPost]
        [Route("UploadImage")]
        public async Task<object> UploadImageKCS()
        {
            try
            {
                var file = await Request.Content.ReadAsByteArrayAsync();
                var action = Request.Headers.GetValues("action").FirstOrDefault();
                var fileName = Request.Headers.GetValues("fileName").FirstOrDefault();
                string uri = HttpContext.Current.Server.MapPath(@"\Images\BangMau");
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
        #endregion


    }
}