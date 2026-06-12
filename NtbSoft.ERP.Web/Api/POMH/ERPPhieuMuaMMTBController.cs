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
    [RoutePrefix("api/ERPPOMuaMMTB")]
    public class ERPPhieuMuaMMTBController : ApiController
    {
        private ERPPOMuaMMTBModel _model = new ERPPOMuaMMTBModel();

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
        public async Task<string> Post(ERPXacNhanPOMuaMMTBEntity objSave)
        {

            if (objSave == null) return "false";
            List<ERPXacNhanPOMuaMMTBEntity> lstXacNhan = new List<ERPXacNhanPOMuaMMTBEntity>();
            lstXacNhan.Add(objSave);
            string json = JsonConvert.SerializeObject(lstXacNhan);
            DataTable tblSaveXN = JsonConvert.DeserializeObject<DataTable>(json);
            return await _model.Post(objSave.Action, tblSaveXN);
        }

        [HttpPost]
        [Route("UploadPDF")]
        public async Task<IHttpActionResult> UploadPDF()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    return BadRequest("Invalid content");

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                var file = provider.Contents.First();
                if (file == null)
                    return BadRequest("No file");

                string fileName = file.Headers.ContentDisposition.FileName?.Trim('"');
                fileName = Path.GetFileName(fileName);

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
                System.Diagnostics.Trace.TraceError($"UploadPDF Error: {ex.Message}\nStackTrace: {ex.StackTrace}");

                // Trả về thông báo lỗi cho client
                return InternalServerError(ex);
            }

        }
    }
}