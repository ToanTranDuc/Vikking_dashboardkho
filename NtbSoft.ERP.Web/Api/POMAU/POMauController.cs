using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using NtbSoft.ERP.Model.POMau;

namespace NtbSoft.ERP.Web.Api.POMau
{
    [RoutePrefix("api/POMau")]
    public class POMauController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                            string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new POMauModel().Get(action, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                           string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new POMauModel().Post(action, tbl, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpPost]
        [Route("PostCT")]
        public string PostCT(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                          string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new POMauModel().PostCT(action, tbl, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpPost]
        [Route("PostNCC")]
        public string PostNCC(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                     string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new POMauModel().PostNCC(action, tbl, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpPost]
        [Route("PostNCCCT")]
        public string PostNCCCT(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                          string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new POMauModel().PostNCCCT(action, tbl, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }
        [HttpPost]
        [Route("PostTV")]
        public string PostTV(string action, [FromBody] DataTable tbl, string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "",
                          string para6 = "", string para7 = "", string para8 = "", string para9 = "", string para10 = "")
        {
            return new POMauModel().PostTV(action, tbl, para1, para2, para3, para4, para5, para6, para7, para8, para9, para10);
        }

        [HttpPost]
        [Route("UploadImage")]
        public string UploadImage()
        {
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count == 0)
                return "No file";

            var file = httpRequest.Files[0];

        
            string folder = HttpContext.Current.Server.MapPath("~/Content/Image/POMau/");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // 🔥 tạo tên file unique
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            string fullPath = Path.Combine(folder, fileName);

            // 🔥 LƯU FILE
            file.SaveAs(fullPath);

            return fileName;
        }
    }
}