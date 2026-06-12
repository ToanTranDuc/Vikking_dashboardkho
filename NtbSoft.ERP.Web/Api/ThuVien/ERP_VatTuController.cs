using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Model.ThuVien;
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
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/ERPVatTuBOM")]
    public class ERP_VatTuController : ApiController
    {
        ERP_VatTuModel _model = new ERP_VatTuModel();
        [HttpGet]
        [Route("GetKH")]
        public DataTable GetKH()
        {
            return _model.GetKH();
        }
        [HttpGet]
        [Route("GetMH")]
        public DataTable GetMH(string makh)
        {
            return _model.GetMH(makh);
        }
        [HttpGet]
        [Route("GetNhom")]
        public DataTable GetNhom()
        {
            return _model.GetNhom();
        }
        [HttpGet]
        [Route("GetDonVi")]
        public DataTable GetDonVi()
        {
            return _model.GetDonVi();
        }
        [HttpGet]
        [Route("GetKhoVai")]
        public DataTable GetKhoVai(string makh, string mahang)
        {
            return _model.GetKhoVai(makh, mahang);
        }
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string makh, string mahang)
        {
            return _model.Get(makh,mahang);
        }
        [HttpGet]
        [Route("GetMauVT")]
        public DataTable GetMauVT()
        {
            return _model.GetMauVT();
        }

        [HttpGet]
        [Route("GetMauSP")]
        public DataTable GetMauSP( string mavtid , string mauvtid)
        {
            return _model.GetMauSP(mavtid,mauvtid);
        }
        [HttpGet]
        [Route("GetMau")]
        public DataTable GetMau(string makh, string mahang)
        {
            return _model.GetMau(makh, mahang);
        }
        [HttpGet]
        [Route("GetBangMau")]
        public DataTable GetBangMau(string makh, string mahang)
        {
            return _model.GetBangMau(makh, mahang);
        }
        [HttpPost]
        [Route("GetPost")]
        public DataTable GetPost(DataTable tbl)
        {
            return new ERP_VatTuModel().GetPost(tbl);
        }
        [HttpPost]
        [Route("PostMVT")]
        public string PostMauVT(DataTable tbMauVT)
        {
           
            return _model.PostMauVT(tbMauVT);
        }

        [HttpPost]
        [Route("PostMSP")]
        public string PostMauSP(DataTable tbMauSP)
        {
            return _model.PostMauSP(tbMauSP);
        }
        [HttpPost]
        [Route("PostMVTCopy")]
        public string PostMVTCopy(DataTable tbMauVT)
        {

            return _model.PostMauVTCopy(tbMauVT);
        }

        [HttpPost]
        [Route("PostMSPCopy")]
        public string PostMSPCopy(DataTable tbMauSP)
        {
            return _model.PostMauSPCopy(tbMauSP);
        }
        [HttpPost]
        [Route("PostThongSo")]
        public string PostThongSo(DataTable tbTS)
        {
            
            return _model.PostThongSo(tbTS);
        }
        [HttpPost]
        [Route("PostThongSoCopy")]
        public string PostThongSoCopy(DataTable tbTS)
        {

            return _model.PostThongSoCopy(tbTS);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string id,string makh, string mahang, string mauvtid)
        {
            return _model.Delete(id,makh, mahang, mauvtid);
        }

        [HttpDelete]
        [Route("DeleteMau")]
        public string DeleteMau( string mavtid, string mauvtid, string mamau)
        {
            return _model.DeleteMau(mavtid, mauvtid, mamau);
        }
        [HttpGet]
        [Route("GetTenCT")]
        public DataTable GetTenCT()
        {
            return _model.GetTenCT();
        }

        [HttpGet]
        [Route("GetDoiNhom")]
        public DataTable GetDoiNhom(string npl, string manhom)
        {
            return _model.GetDoiNhom(npl, manhom);
        }
        [HttpPost]
        [Route("PostUpdateNhom")]
        public string PostUpdateNhom([FromBody] DataTable tbTS, string para = "")
        {

            return _model.PostUpdateNhom(tbTS, para);
        }
        [HttpPost]
        [Route("PostT1")]
        public string PostT1(string action, [FromBody] DataTable tbl, string para = "", string para2 = "", string para3 = "", string para4 = "")
        {
            return _model.PostT1(action, para, para2, para3, para4, tbl);
        }
        [HttpPost]
        [Route("PostT2")]
        public string PostT2(string action, [FromBody] DataTable tbl, string para = "", string para2 = "", string para3 = "", string para4 = "")
        {
            return _model.PostT2(action, para, para2, para3, para4, tbl);
        }
        [HttpPost]
        [Route("PostT3")]
        public string PostT3(string action, [FromBody] DataTable tbl, string para = "", string para2 = "", string para3 = "", string para4 = "")
        {
            return _model.PostT3(action, para, para2, para3, para4, tbl);
        }
        [HttpPost]
        [Route("PostT4")]
        public string PostT4(string action, [FromBody] DataTable tbl, string para = "", string para2 = "", string para3 = "", string para4 = "")
        {
            return _model.PostT4(action, para, para2, para3, para4, tbl);
        }
        [HttpGet]
        [Route("GetTimKiem")]
        public DataTable GetTimKiem()
        {
            return _model.GetTimKiem();
        }
        [HttpGet]
        [Route("GetChung")]
        public DataTable GetChung(string action, string para = "", string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "")
        {
            return _model.GetChung(action, para, para1, para2, para3, para4, para5);
        }
        string sourceFile = "";
        string outputfile = "";
        [HttpPost]
        [Route("UploadFilePDF")]
        public async Task<bool> UploadFilePDFV2()
        {

            var file = await Request.Content.ReadAsByteArrayAsync();
            var folderName = Request.Headers.GetValues("folderName").FirstOrDefault();
            var fileName = Request.Headers.GetValues("fileName").FirstOrDefault();
            string uri = HttpContext.Current.Server.MapPath(@"\Content\Pdf\" + folderName);
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
        public void ConvertSplitPdfPagesToImages(string inputPdfPath, string outputDirectory)
        {
            bool flagExit = false;
            //GhostscriptVersionInfo version = new GhostscriptVersionInfo(
            //new Version(10, 01, 1), // Specify the Ghostscript version (e.g., 9.53.3)
            //uri_gh, // Specify the path to the Ghostscript DLL file
            // string.Empty,
            // GhostscriptLicense.GPL); // Provide an empty string for the initialization parameter

            //GhostscriptRasterizer rasterizer = new GhostscriptRasterizer();
            //GhostscriptVersionInfo version1 = new GhostscriptVersionInfo()

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