using Newtonsoft.Json;
using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static NtbSoft.ERP.Entity.QuanLyDonHang.CapThemNgoaiDH;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/capthemngoaidh")]
    public class BC_CapThemNgoaiDHController : ApiController
    {
        [HttpGet]
        [Route("get")]
        public DataTable Get(string action, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null,
                        string param6 = null, string param7 = null, string param8 = null, string param9 = null)
        {
            return new CapThemNgoaiDH().Get(action, param1, param2, param3, param4, param5, param6, param7, param8, param9);
        }

        [HttpPost]
        [Route("Post")]
        public string Post(string action, List<TYPE_ERP_PhieuDeNghiCapThemVT> lstData, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null,
                        string param6 = null, string param7 = null, string param8 = null, string param9 = null)
        {
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            return new CapThemNgoaiDH().Post(action, tbl, param1, param2, param3, param4, param5, param6, param7, param8, param9);
        }

        [HttpPost]
        [Route("PostDNCT")]
        public string PostDNCT(string action, List<CapThem_NgoaiDH> lstData, string Para1 = "", string Para2 = "", string Para3 = "", string Para4 = "", string Para5 = "", string Para6 = "", string Para7 = "", string Para8 = "", string Para9 = "")
        {
            string imageDataA = lstData[0].SignNgDK;
            string imageDataB = lstData[0].SignTBPNgDK;
            string imageDataC = lstData[0].SignMer;
            string imageDataD = lstData[0].SignTBPMer;
            string datetime = DateTime.Now.ToString("ddMMyyyyHHss");
            string imageNameA = SaveSignatureImage(imageDataA, $"SignNgDK", "", datetime);
            string imageNameB = SaveSignatureImage(imageDataB, $"SignTBPNgDK", "", datetime);
            string imageNameC = SaveSignatureImage(imageDataC, $"SignMer", "", datetime);
            string imageNameD = SaveSignatureImage(imageDataD, $"SignTBPMer", "", datetime);



            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            foreach (DataRow item in tbl.Rows)
            {
                item["SignNgDK"] = imageNameA;
                item["SignTBPNgDK"] = imageNameB;
                item["SignMer"] = imageNameC;
                item["SignTBPMer"] = imageNameD;
            }

            return new CapThemNgoaiDH().Post(action, tbl, Para1, Para2, Para3, Para4, Para5, Para6, Para7, Para8, Para9);
        }

        private string SaveSignatureImage(string base64Data, string role, string module, string datetime)
        {
            if (string.IsNullOrEmpty(base64Data)) return "";

            string[] parts = base64Data.Split(',');
            if (parts.Length < 2) return base64Data;

            byte[] bytes = Convert.FromBase64String(parts[1]);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Image image = Image.FromStream(ms);
                string fileName = $"{role}-{module}-{datetime}.png";
                string uploadPath = HttpContext.Current.Server.MapPath("~/Images/SignDeNghiCapThemNPL_NgoaiDH");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                string fullPath = Path.Combine(uploadPath, fileName);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                image.Save(fullPath);
                return fileName;
            }
        }
        [HttpPost]
        [Route("PostGetTonKho")]
        public DataTable PostGetTonKho([FromBody] TonKhoRequest req)
        {
            var param1 = string.Join(";", req.MaNPLList);
            return new CapThemNgoaiDH().Get("GetTonKhoByNPL", param1);
        }

        public class TonKhoRequest
        {
            public List<string> MaNPLList { get; set; }
        }

        [HttpPost]
        [Route("PostChiTiet")]
        public string PostChiTiet(string action, List<Type_ERP_PhieuDeNghiCapThemVTChiTiet> lstData, string Para1 = "", string Para2 = "", string Para3 = "", string Para4 = "", string Para5 = "", string Para6 = "", string Para7 = "", string Para8 = "", string Para9 = "")
        {
            var json = JsonConvert.SerializeObject(lstData);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            return new CapThemNgoaiDH().PostChiTiet(action, tbl, Para1, Para2, Para3, Para4, Para5, Para6, Para7, Para8, Para9);
        }
    }
}
