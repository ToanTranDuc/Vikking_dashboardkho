using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Model.Kho;
using NtbSoft.ERP.Web.Filter;
using NtbSoft.ERP.Web.Repository.R.Kho;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.Kho
{
    //[HMACAuthentication]
    [RoutePrefix("api/ViTriKhoTong")]
    public class ViTriKhoTongController : ApiController
    {
        [HttpGet]
        [Route("Get")]
        public DataTable Get(string action, string Para1, string Para2, string Para3, string Para4, string Para5, string Para6, string Para7, string Para8 = "", string Para9 = "")
        {
            return new ViTriKhoTongModel().Get(action, Para1, Para2, Para3, Para4, Para5, Para6, Para7, Para8, Para9);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(dynamic data, string Para1 = "", string Para2 = "")
        {
            string dateTime = Para2 == "" ? "" : Convert.ToDateTime(Para2).ToString("yyyy-MM-dd HH:mm");
            List<ViTriKhoDetail> lst = new List<ViTriKhoDetail>();
            foreach (var item in data)
            {
                lst.Add(new ViTriKhoDetail
                {
                    MaDH = item.MaDH.ToString(),
                    MaHang = item.MaHang.ToString(),
                    Ma_PKL = item.Ma_PKL.ToString(),
                    MaDVSX = item.MaDVSX.ToString(),
                    SizeID = item.SizeID.ToString(),
                    Size = item.Size.ToString(),
                    PoID = item.PoID.ToString(),
                    Po = item.Po.ToString(),
                    DauSizeID = item.DauSizeID.ToString(),
                    DauSize = item.DauSize.ToString(),
                    ColorID = item.ColorID.ToString(),
                    Color = item.Color.ToString(),
                    MaKhuVuc = item.MaKhuVuc.ToString(),
                    MaKho = item.MaKho.ToString(),
                    MaKe = item.MaKe.ToString(),
                    MaTang = item.MaTang.ToString(),
                    MaO = item.MaO.ToString(),
                    TenThung = (int)item.SttThungView,
                    SttThung = (int)item.SttThung,
                    CBM = (float)item.KhoiLuong,
                    NgayLuuKho = "",
                    NVNhap = item.NVNhap.ToString(),
                });
            }
            var json = JsonConvert.SerializeObject(lst);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            return new ViTriKhoTongModel().Post(tbl, "Post", "@typetable", Para1, dateTime);
        }
    }
    public class ViTriKhoDetail
    {
        public string MaDH { get; set; }
        public string MaHang { get; set; }
        public string Ma_PKL { get; set; }
        public string MaDVSX { get; set; }
        public string SizeID { get; set; }
        public string Size { get; set; }
        public string PoID { get; set; }
        public string Po { get; set; }
        public string DauSizeID { get; set; }
        public string DauSize { get; set; }
        public string ColorID { get; set; }
        public string Color { get; set; }
        public string MaKhuVuc { get; set; }
        public string MaKho { get; set; }
        public string MaKe { get; set; }
        public string MaTang { get; set; }
        public string MaO { get; set; }
        public int TenThung { get; set; }
        public int SttThung { get; set; }
        public float CBM { get; set; }
        public string NgayLuuKho { get; set; }
        public string NVNhap { get; set; }
    }
}
