using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using NtbSoft.ERP.Model.Qty;
using OfficeOpenXml;
using System.Net.Http;
using System.Web.Hosting;
using OfficeOpenXml.Drawing;
using System.Net;
using System.Linq;
using NtbSoft.ERP.Model.QuanLyDonHang;
using NtbSoft.ERP.Web.Api.ThuVien;
using NtbSoft.ERP.Model.POMuaHang;
using NtbSoft.ERP.Entity.POMuaHang;
using System.Web;

namespace NtbSoft.ERP.Web.Api.POMuaHang
{
    [RoutePrefix("api/ERPCanDoiNguyenPhuLieu")]
    public class ERPCanDoiNguyenPhuLieuController : ApiController
    {
        private ERPCanDoiNguyenPhuLieuModel _model = new ERPCanDoiNguyenPhuLieuModel();

        [HttpGet]
        [Route("GET")]
        public async Task<DataTable> GET(string action, string para1 = null, string para2 = null, string para3 = null, string para4 = null, string para5 = null)
        {
            para1 = para1 ?? "NONE";
            para2 = para2 ?? "NONE";
            para3 = para3 ?? "NONE";
            para4 = para4 ?? "NONE";
            para5 = para5 ?? "NONE";
            return await _model.Get(action, para1, para2, para3, para4, para5);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<string> Delete(string action, string Para1, string Para2 = null)
        {
            Para1 = Para1 ?? "NONE";
            Para2 = Para2 ?? "NONE";
            return await _model.Delete(action, Para1, Para2);
        }
        [HttpPost]
        [Route("Post")]
        public async Task<string> Post(ERPXacNhanPOMuaEntity objSave)
        {
         
            if (objSave == null) return "false";
            List<ERPXacNhanPOMuaEntity> lstXacNhan = new List<ERPXacNhanPOMuaEntity>();
            lstXacNhan.Add(objSave); 
            string json = JsonConvert.SerializeObject(lstXacNhan);
            DataTable tblSaveXN = JsonConvert.DeserializeObject<DataTable>(json);
            return await _model.Post(objSave.Action, tblSaveXN);
        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<IHttpActionResult> UploadFile()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return BadRequest("Invalid content");

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                var file = provider.Contents.FirstOrDefault();
                if (file == null)
                    return BadRequest("No file");

                string fileName = file.Headers.ContentDisposition.FileName?.Trim('"');
                fileName = Path.GetFileName(fileName);

                string ext = Path.GetExtension(fileName).ToLower();
                string[] allowedExtensions =
                {
                    ".pdf", ".doc", ".docx",
                    ".xls", ".xlsx",
                    ".jpg", ".jpeg", ".png", ".bmp"
                };

                if (!allowedExtensions.Contains(ext))
                    return BadRequest("File type not allowed");

                byte[] data = await file.ReadAsByteArrayAsync();

                string folderPath = HttpContext.Current.Server.MapPath("~/PMH/VatTu");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fullPath = Path.Combine(folderPath, fileName);
                File.WriteAllBytes(fullPath, data);

                return Ok(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    $"UploadFile Error: {ex.Message}\n{ex.StackTrace}"
                );
                return InternalServerError(ex);
            }

        }

        [HttpGet]
        [Route("DownloadFile")]
        public HttpResponseMessage DownloadFile(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid file name");

                string folderPath = HttpContext.Current.Server.MapPath("~/PMH/VatTu");
                string fullPath = Path.Combine(folderPath, fileName);

                if (!File.Exists(fullPath))
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                byte[] fileBytes = File.ReadAllBytes(fullPath);
                string contentType = MimeMapping.GetMimeMapping(fileName);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(fileBytes);
                response.Content.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                response.Content.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };

                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

    }
}