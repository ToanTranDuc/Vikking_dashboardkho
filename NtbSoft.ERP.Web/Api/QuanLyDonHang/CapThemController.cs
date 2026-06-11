using Newtonsoft.Json;
using NtbSoft.ERP.Model.QuanLyDonHang;
using NtbSoft.ERP.Web.Repository.R.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/Dot")]
    public class CapThemController : ApiController
    {
        ICapThemRepository _repo = new CapThemRepository();
        
        [HttpGet]
        [Route("GetLichSu")]

        public DataTable GetLichSu(string madh, string malenh, string manpl, string mavt, string mamau, string dausize, string size, string mabom, string mavtmau)
        {
            return _repo.GetLichSu(madh, malenh, manpl, mavt, mamau, dausize, size, mabom, mavtmau);
        }

        [HttpGet]
        [Route("GetDinhMucCD")]
        public DataTable GetDinhMucCD(string madh, string maLenhSX)
        {
            CapThemModel capthemmodel = new CapThemModel();
            return capthemmodel.GetDinhMucCD(madh, maLenhSX);
        }

        [HttpPost]
        [Route("PostDotDinhMuc")]
        public string PostDotDinhMuc(DataTable ojDinhMuc)
        {
            if (ojDinhMuc == null) return "false";
            string json = JsonConvert.SerializeObject(ojDinhMuc);
            DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDotDinhMuc(ojDinhMuc);
        }

        [HttpDelete]
        [Route("DeleteDot")]
        public string DeleteDot(string madh, string malenhsanxuat, string dot, string manpl, string mavt, string mamau, string dausize, string size, string mabom, string mavtmau)
        {
            return _repo.DeleteDot(madh, malenhsanxuat, dot, manpl, mavt, mamau, dausize, size, mabom, mavtmau);
        }
    }
}