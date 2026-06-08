using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using NtbSoft.ERP.Model.KeHoach;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.KeHoach
{
    [RoutePrefix("api/KHDongThung_Description")]
    public class KHDongThung_DescriptionController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para1, string para2,string para3)
        {
            return new KHDongThungDescriptionModel().Get(action, para1, para2,para3);
        }       
        [HttpPost]
        [Route("Post")]
        public string PostData(string action, DataTable dt)
        {
            return new KHDongThungDescriptionModel().Post(action, dt);
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
                string uri = HttpContext.Current.Server.MapPath(@"\Content\UrlImgDongThung");
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

        string sourceFile = "";
        string outputfile = "";
        [HttpPost]
        [Route("UploadPDF")]
        public async Task<bool> UploadFilePDFV2()
        {

            var file = await Request.Content.ReadAsByteArrayAsync();
            var folderName = Request.Headers.GetValues("folderName").FirstOrDefault();
            var fileName = Request.Headers.GetValues("fileName").FirstOrDefault();
            string uri = HttpContext.Current.Server.MapPath(@"\Content\UrlPDFDongThung\");
            string dir = uri + @"\" + fileName;

            try
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                else
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(uri + @"\" + fileName);

                    foreach (FileInfo fileDelete in di.GetFiles())
                    {
                        fileDelete.Delete();
                    }
                }
                sourceFile = uri + @"\" + fileName + ".pdf";
                File.WriteAllBytes(sourceFile, file);
                outputfile = uri + @"\" + fileName;               
                ConvertSplitPdfPagesToImages(sourceFile, outputfile);

                return true;
            }
            catch (Exception ex)
            {
               
                return false;
            }
        }
        //string uri_gh = HttpContext.Current.Server.MapPath(@"\bin\gsdll32.dll");
        public void ConvertSplitPdfPagesToImages(string inputPdfPath, string outputDirectory)
        {
            bool flagExit = false;          
            string dllName = Environment.Is64BitProcess ? "gsdll64.dll" : "gsdll32.dll";
            string uri_gh = HttpContext.Current.Server.MapPath($@"\bin\{dllName}");

            var version = new GhostscriptVersionInfo(
                new Version(10, 04, 0),
                uri_gh,
                string.Empty,
                GhostscriptLicense.GPL);
            try
            {
                using (var rasterizer = new GhostscriptRasterizer())
                {
                    rasterizer.Open(inputPdfPath, version, false);

                    for (int pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                    {
                        string outputImagePath = Path.Combine(outputDirectory, $"{pageNumber}.png");

                        using (var image = rasterizer.GetPage(150, pageNumber)) // DPI thấp để tránh out memory
                        {
                            image.Save(outputImagePath, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {                
            }
            finally
            {
                //rasterizer.Close();
            }
        }
    }
}