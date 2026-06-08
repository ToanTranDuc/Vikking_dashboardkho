using Newtonsoft.Json;
using NtbSoft.ERP.Model.Kho;
using NtbSoft.ERP.Web.Repository.R.Kho;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.Kho
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/DongThung")]
    public class DongThungController : ApiController
    {
        IDongThungRepository _repo = new DongThungRepository();
        [HttpGet]
        [Route("GetDS")]
        public DataTable GetDS()
        {
            return _repo.GetDS();
        }
        [HttpGet]
        [Route("GetThongTin")]
        public DataTable GetThongTin(string madh, string pageIndex, string pageSize, string madvsx)
        {
            return _repo.GetThongTin(madh, pageIndex, pageSize, madvsx);
        }
        [HttpGet]
        [Route("GetThongTinSMS")]
        public DataTable GetThongTinSMS(string madh, string pageIndex, string pageSize, string madvsx)
        {
            return _repo.GetThongTinSMS(madh, pageIndex, pageSize, madvsx);
        }
        [HttpGet]
        [Route("GetThongTinPO")]
        public DataTable GetThongTinPO(string madh, string madvsx)
        {
            return _repo.GetThongTinPO(madh, madvsx);
        }
        [HttpGet]
        [Route("GetThongTinPOSMS")]
        public DataTable GetThongTinPOSMS(string madh, string madvsx)
        {
            return _repo.GetThongTinPOSMS(madh, madvsx);
        }
        [HttpGet]
        [Route("GetThongTinPOCT")]
        public DataTable GetThongTinPOCT(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat)
        {
            return _repo.GetThongTinPOCT(mapkl, madvsx, malenh, poid, madh, malenhsanxuat);
        }
        [HttpGet]
        [Route("GetThongTinPOCTB")]
        public DataTable GetThongTinPOCTB(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat)
        {
            return new DongThungModel().GetThongTinPOCTB(mapkl, madvsx, malenh, poid, madh, malenhsanxuat);
        }

        [HttpGet]
        [Route("GetThongTinPOCTSMS")]
        public DataTable GetThongTinPOCTSMS(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat)
        {
            return _repo.GetThongTinPOCTSMS(mapkl, madvsx, malenh, poid, madh, malenhsanxuat);
        }
        [HttpGet]
        [Route("GetThongTinChiTiet")]
        public DataTable GetThongTinChiTiet(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int minsttthung, int maxsttthung)
        {
            return _repo.GetThongTinChiTiet(mapkl, malenh, madvsx, poid, dausizeid, mau, tuthung, denthung, minsttthung, maxsttthung);
        }
        [HttpGet]
        [Route("GetThongTinChiTietSMS")]
        public DataTable GetThongTinChiTietSMS(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int minsttthung, int maxsttthung)
        {
            return _repo.GetThongTinChiTietSMS(mapkl, malenh, madvsx, poid, dausizeid, mau, tuthung, denthung, minsttthung, maxsttthung);
        }
        [HttpPost]
        [Route("Post")]
        public string Post(DataTable tblNhapKho, bool isdongthung)
        {
            return _repo.Post(tblNhapKho, isdongthung);
        }
        [HttpPost]
        [Route("PostSMS")]
        public string PostSMS(DataTable tblNhapKho, bool isdongthung)
        {
            return _repo.PostSMS(tblNhapKho, isdongthung);
        }
        [HttpPost]
        [Route("PostDT")]
        public string PostDT(DataTable tblNhapKho, bool isdongthung)
        {
            return _repo.PostDT(tblNhapKho, isdongthung);
        }
        [HttpPost]
        [Route("PostDTSMS")]
        public string PostDTSMS(DataTable tblNhapKho, bool isdongthung)
        {
            return _repo.PostDTSMS(tblNhapKho, isdongthung);
        }
        [HttpPost]
        [Route("PostHuy")]
        public string PostHuy(DataTable tblNhapKho, bool isdongthung)
        {
            return _repo.PostHuy(tblNhapKho, isdongthung);
        }
        [HttpPost]
        [Route("PostHuySMS")]
        public string PostHuySMS(DataTable tblNhapKho, bool isdongthung)
        {
            return _repo.PostHuySMS(tblNhapKho, isdongthung);
        }
        [HttpPost]
        [Route("PostChiTiet")]
        public string PostChiTiet(DataTable tblNhapKho, bool isdongthung)
        {
            return _repo.PostChiTiet(tblNhapKho, isdongthung);
        }
        [HttpPost]
        [Route("PostChiTietSMS")]
        public string PostChiTietSMS(DataTable tblNhapKho, bool isdongthung)
        {
            return _repo.PostChiTietSMS(tblNhapKho, isdongthung);
        }
        [HttpPost]
        [Route("PostALL")]
        public string PostALL(DataTable tblNhapKho, int value)
        {
            return _repo.PostALL(tblNhapKho, value);
        }
        [HttpPost]
        [Route("PostALLSMS")]
        public string PostALLSMS(DataTable tblNhapKho, int value)
        {
            return _repo.PostALLSMS(tblNhapKho, value);
        }
        [HttpPost]
        [Route("PostViTri")]
        public string PostViTri(DataTable tblNhapKho, bool isdongthung)
        {
            return _repo.PostViTri(tblNhapKho, isdongthung);
        }
        [HttpPost]
        [Route("PostViTriSMS")]
        public string PostViTriSMS(DataTable tblNhapKho, bool isdongthung)
        {
            return _repo.PostViTriSMS(tblNhapKho, isdongthung);
        }
        //coding by Dat
        //start
        [HttpGet]
        [Route("GetLenhSX")]
        public DataTable GetLenhSX(string statuschuyen, string username, string status)
        {
            return _repo.GetLenhSX(statuschuyen, username, status);
        }

        [HttpGet]
        [Route("GetDS_KHDT")]
        public DataTable GetDS_KHDT(string maplk, string mahang, string poid, string statuschuyen, string madvsx, string isDecat, string dotsx)
        {
            return _repo.GetDS_KHDT(maplk, mahang, poid, statuschuyen, madvsx, isDecat, dotsx);
        }

        [HttpGet]
        [Route("GetDSThung")]
        public DataTable GetDSThung(string maplk, string poid, string mahang, string tuthung, string denthung, string madvsx, string isDeCat, string dotsx)
        {
            return _repo.GetDSThung(maplk, poid, mahang, tuthung, denthung, madvsx, isDeCat, dotsx);
        }

        [HttpPost]
        [Route("PostDongThung")]
        public string PostDongThung(DataTable tblDongThung)
        {
            return _repo.PostDongThung(tblDongThung);
        }

        [HttpGet]
        [Route("Search")]
        public DataTable Search(string malenh, string para)
        {
            return _repo.Search(malenh, para);
        }
        //end
        [HttpGet]
        [Route("GetDsTongQuanDongThung")]
        public DataTable GetDsTongQuanDongThung(string maHang, string ngayDongThung, string maDVSX)
        {
            DateTime _ngayDongThung = new DateTime(1970,01,01);

            string[] formats1 = GetDateFormats();
            string[] formats2 = GetDateParten();

            if (ngayDongThung != null && !string.IsNullOrEmpty(ngayDongThung) && !DateTime.TryParseExact(ngayDongThung, formats1, CultureInfo.InvariantCulture, DateTimeStyles.None, out _ngayDongThung))
            {
                DateTime.TryParseExact(ngayDongThung, formats2, CultureInfo.InvariantCulture, DateTimeStyles.None, out _ngayDongThung);
            }

            return _repo.GetDsTongQuanDongThung(maHang, _ngayDongThung, maDVSX);
        }
        [HttpGet]
        [Route("GetDsXuatNhapTon")]
        public DataTable GetDsXuatNhapTon(string maHang, string maDVSX, int isChecked)
        {
            return _repo.GetDsXuatNhapTon(maHang, maDVSX,isChecked);
        }

        [HttpGet]
        [Route("GetDsXuatNhapTonLuanChuyen")]
        public DataTable GetDsXuatNhapTonLuanChuyen(string maHang, string maDVSX, int isChecked)
        {
            return _repo.GetDsXuatNhapTonLuanChuyen(maHang, maDVSX,isChecked);
        }

        [HttpPost]
        [Route("GetChiTietDongThung")]
        public DataTable GetChiTietDongThung(DataTable tblChiTiet)
        {
            return _repo.GetChiTietDongThung(tblChiTiet);
        }
        [HttpPost]
        [Route("GetKiemTra")]
        public DataTable GetKiemTra(DataTable data)
        {
            return new DongThungModel().GetKiemTra(data);
        }
        [HttpPost]
        [Route("GetKiemTraAll")]
        public DataTable GetKiemTraAll(DataTable data)
        {
            return new DongThungModel().GetKiemTraAll(data);
        }
        [HttpPost]
        [Route("GetKiemTraViTri")]
        public DataTable GetKiemTraViTri(DataTable data)
        {
            return new DongThungModel().GetKiemTraViTri(data);
        }
        private string[] GetDateFormats()
        {
            string baseFormat = "d/M/yyyy";
            return GenerateDateFormats(baseFormat);
        }
        private string[] GenerateDateFormats(string baseFormat)
        {
            List<string> formats = new List<string>();


            formats.Add(baseFormat);


            formats.Add(baseFormat.Replace("/", "-"));
            formats.Add(baseFormat.Replace("-", "/"));
            formats.Add(baseFormat.Replace("/", "."));


            string[] components = baseFormat.Split('/');
            if (components.Length == 3)
            {
                string reorderedFormat = $"{components[1]}/{components[0]}/{components[2]}";
                formats.Add(reorderedFormat);
                formats.Add(reorderedFormat.Replace("/", "-"));
                formats.Add(reorderedFormat.Replace("-", "/"));
                formats.Add(reorderedFormat.Replace("/", "."));
            }

            return formats.ToArray();
        }
        public string[] GetDateParten()
        {
            DateTimeFormatInfo formatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
            string[] datePatterns = formatInfo.GetAllDateTimePatterns();
            return datePatterns;
        }
        // Mạnh

        [HttpPost]
        [Route("UpdateDongThungNhapKho")]
        public string UpdateDongThungNhapKho(string action,dynamic data)
        {
            string json = JsonConvert.SerializeObject(data);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            return new DongThungModel().UpdateDongThungNhapKho(action, tbl);
        }
    }

}