using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{
    [RoutePrefix("api/CanDoiDMNL")]
    public class CanDoiDMNLController: ApiController
    {

        [HttpGet]
        [Route("GetCanDoiBomVatTu")]
        public DataTable GetCanDoiBomVatTu(string madh, string maLenhSX)
        {
            CanDoiDMNLModel candoiModel = new CanDoiDMNLModel();
            return candoiModel.GetCanDoiBomVatTu(madh, maLenhSX);
        }
        [HttpGet]
        [Route("GetDinhMucCD")]
        public DataTable GetDinhMucCD(string madh, string maLenhSX)
        {
            CanDoiDMNLModel candoiModel = new CanDoiDMNLModel();
            return candoiModel.GetDinhMucCD(madh, maLenhSX);
        }
        [HttpGet]
        [Route("GetDinhMucBOM")]
        public DataTable GetDinhMucBOM(string mahang)
        {
            CanDoiDMNLModel candoiModel = new CanDoiDMNLModel();
            return candoiModel.GetDinhMucBOM(mahang);
        }

        [HttpGet]
        [Route("GetDinhMucNL")]
        public DataTable GetDinhMucNL(string mahang)
        {
            CanDoiDMNLModel candoiModel = new CanDoiDMNLModel();
            return candoiModel.GetDinhMucNL(mahang);
        }
    }
}