using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Model.QuanLyDonHang;
using NtbSoft.ERP.Web.Repository.R.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NtbSoft.ERP.Web.Api.QuanLyDonHang
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/CanDoiDonHangTong")]
    public class CanDoiDonHangTongController : ApiController
    {
        ICanDoiDonHangTongRepository _repo = new CanDoiDonHangTongRepository();
        [HttpGet]
        [Route("GetDonHangTong")]
        public DataTable GetDonHangTong(int pageIndex, int pageSize)
        {
            return _repo.GetDonHangTong(pageIndex, pageSize);
        }
        [HttpGet]
        [Route("GetDonHangTong_MaDH")]
        public DataTable GetDonHangTong(string maDH)
        {
            return _repo.GetDonHangTong_MaDH(maDH);
        }
        [HttpGet]
        [Route("GetDonHangTongView")]
        public DataTable GetDonHangTongView(int pageIndex, int pageSize, string madvsx)
        {
            return _repo.GetDonHangTongView(pageIndex, pageSize, madvsx);
        }
        [HttpGet]
        [Route("GetDonHangTongViewLoc")]
        public DataTable GetDonHangTongViewLoc(int pageIndex, int pageSize, string madvsx, string tungay, string denngay, string mahang, string dot, string sl)
        {
            return _repo.GetDonHangTongViewLoc(pageIndex, pageSize, madvsx, tungay, denngay, mahang, dot, sl);
        }
        [HttpGet]
        [Route("Get")]
        public DataTable Get()
        {
            return _repo.Get();
        }
        [HttpGet]
        [Route("GetPO")]
        public DataTable GetPO(string madh)
        {
            return _repo.GetPO(madh);
        }
        [HttpGet]
        [Route("GetDauSize")]
        public DataTable GetDauSize(string madh, string poid)
        {
            return _repo.GetDauSize(madh, poid);
        }
        [HttpGet]
        [Route("GetMau")]

        public DataTable GetMau(string madh, string dausizeID)
        {
            return _repo.GetMau(madh, dausizeID);
        }
        [HttpGet]
        [Route("GetSizeID")]

        public DataTable GetSizeID(string madh, string dausizeID, string mamau)
        {
            return _repo.GetSizeID(madh, dausizeID, mamau);
        }
        [HttpGet]
        [Route("GetListSize")]

        public DataTable GetListSize(string madh)
        {
            return _repo.GetListSize(madh);
        }
        [HttpGet]
        [Route("GetSoLuongDH")]

        public DataTable GetSoLuongDH(string madh, string dausizeID, string mamau, string poid)
        {
            return _repo.GetSoLuongDH(madh, dausizeID, mamau, poid);
        }
        [HttpGet]
        [Route("GetMaxDH")]
        public string GetMaxDH()
        {
            return _repo.GetMaxDH();
        }
        [HttpGet]
        [Route("GetNPL")]

        public DataTable GetNPL(string madh, string malenhsanxuat)
        {
            return _repo.GetNPL(madh, malenhsanxuat);
        }
        [HttpGet]
        [Route("GetGomNPL")]

        public DataTable GetGomNPL(string parameter)
        {
            return _repo.GetGomNPL(parameter);
        }
        [HttpPost]
        [Route("PostDVSX")]
        public string PostDVSX(List<CanDoiDonViSanXuatSaveEntity> ojDVSX)
        {
            if (ojDVSX == null) return "false";
            string json = JsonConvert.SerializeObject(ojDVSX);
            DataTable tbDVSX = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDVSX(tbDVSX);
        }

        [HttpPost]
        [Route("PostDVSXV2")]
        public string PostDVSXV2(List<CanDoiDonViSanXuatSaveEntity> ojDVSX)
        {
            if (ojDVSX == null) return "false";
            string json = JsonConvert.SerializeObject(ojDVSX);
            DataTable tbDVSX = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDVSXV2(tbDVSX);
        }
        [HttpPost]
        [Route("PostIsKeoVe")]
        public string PostIsKeoVe(List<CanDoiDonViSanXuatSaveEntity> ojDVSX)
        {
            if (ojDVSX == null) return "false";
            string json = JsonConvert.SerializeObject(ojDVSX);
            DataTable tbDVSX = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostIsKeoVe(tbDVSX);
        }
        [HttpPost]
        [Route("PostUDDVSX")]
        public string PostUDDVSX(List<CanDoiDonViSanXuatSaveEntity> ojDVSX)
        {
            if (ojDVSX == null) return "false";
            string json = JsonConvert.SerializeObject(ojDVSX);
            DataTable tbDVSX = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostUDDVSX(tbDVSX);
        }
        [HttpPost]
        [Route("PostNPL")]
        public string PostNPL(List<DinhMucSaveEntity> ojNPL)
        {
            if (ojNPL == null) return "false";
            string json = JsonConvert.SerializeObject(ojNPL);
            DataTable tbNPL = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostNPL(tbNPL);
        }
        [HttpGet]
        [Route("GetLenhSX")]
        public DataTable GetLenhSX(string madh, string poid)
        {
            return _repo.GetLenhSX(madh, poid);
        }
        [HttpGet]
        [Route("GetChiTietCanDoiNPL")]

        public DataTable GetChiTietCanDoiNPL(string madh, string malenh)
        {
            return _repo.GetChiTietCanDoiNPL(madh, malenh);
        }
        [HttpGet]
        [Route("GetChiTietCanDoiDVSX")]

        public DataTable GetChiTietCanDoiDVSX(string malenhsanxuat)//(string madh, string malenh, string malenhsanxuat, string poid)
        {
            return _repo.GetChiTietCanDoiDVSX(malenhsanxuat);
        }
        [HttpGet]
        [Route("GetChiTietLenhSX")]

        public DataTable GetChiTietLenhSX(string madh, string madvsx)
        {
            return _repo.GetChiTietLenhSX(madh, madvsx);
        }
        [HttpGet]
        [Route("GetCTTotal")]

        public DataTable GetCTTotal(string madh)
        {
            return _repo.GetCTTotal(madh);
        }
        [HttpGet]
        [Route("GetCTTotalDetail")]

        public DataTable GetCTTotalDetail(string madh)
        {
            return _repo.GetCTTotalDetail(madh);
        }
        [HttpPost]
        [Route("PostUpdateDVSX")]
        public string PostUpdateDVSX(List<EditCanDoiLenhSanXuatEntity> ojDVSX)
        {
            if (ojDVSX == null) return "false";
            string json = JsonConvert.SerializeObject(ojDVSX);
            DataTable tbDVSX = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostUpdateDVSX(tbDVSX);
        }
        [HttpPost]
        [Route("PostUpdateNPL")]
        public string PostUpdateNPL(List<DinhMucSaveEntity> ojNPL)
        {
            if (ojNPL == null) return "false";
            string json = JsonConvert.SerializeObject(ojNPL);
            DataTable tbNPL = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostUpdateNPL(tbNPL);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string madh, string malenh, string malenhsanxuat, string username)
        {
            return _repo.Delete(madh, malenh, malenhsanxuat, username);
        }
        [HttpDelete]
        [Route("DeleteDM")]
        public string DeleteDM(string madh, string malenh, string malenhsanxuat, string manpl, string username)
        {
            return _repo.DeleteDM(madh, malenh, malenhsanxuat, manpl, username);
        }
        [HttpDelete]
        [Route("DeleteNPL")]
        public string DeleteNPL(string madh, string malenhsanxuat, string username)
        {
            return _repo.DeleteNPL(madh, malenhsanxuat, username);
        }
        [HttpGet]
        [Route("GetDSHangHoa")]
        public DataTable GetDSHangHoa()
        {
            return _repo.GetDSHangHoa();
        }
        [HttpGet]
        [Route("GetDSThongTinLenhSX")]
        public DataTable GetDSThongTinLenhSX(int pageIndex, int pageSize)
        {
            return _repo.GetDSThongTinLenhSX(pageIndex, pageSize);
        }
        [HttpGet]
        [Route("GetDSChiTietLenhSX")]
        public DataTable GetDSChiTietLenhSX(string id, int pageIndex, int pageSize)
        {
            return _repo.GetDSChiTietLenhSX(id, pageIndex, pageSize);
        }
        [HttpGet]
        [Route("GetKTCanDoiNPL")]
        public DataTable GetKTCanDoiNPL(string madh, string malenh, string poid, string mamau, string dausizeid)
        {
            return _repo.GetKTCanDoiNPL(madh, malenh, poid, mamau, dausizeid);
        }
        [HttpGet]
        [Route("GetDsCanDoiDonHang")]
        public DataTable GetDsCanDoiDonHang(string maDH, string poID, string dauSizeID, string maMau)
        {
            return _repo.GetDsCanDoiDonHang(maDH, poID, dauSizeID, maMau);
        }
        [Route("GetDsBaoCao")]
        public DataTable GetDsBaoCao(string maLenhSX)
        {
            return _repo.GetDsBaoCao(maLenhSX);
        }
        [HttpGet]
        [Route("GetKiemTraDinhMuc")]
        public DataTable GetKiemTraDinhMuc(string madh, string malenhsanxuat)
        {
            return _repo.GetKiemTraDinhMuc(madh, malenhsanxuat);
        }
        [HttpPost]
        [Route("PostDinhMuc")]
        public string PostDinhMuc(DataTable ojDinhMuc)
        {
            if (ojDinhMuc == null) return "false";
            string json = JsonConvert.SerializeObject(ojDinhMuc);
            DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDinhMuc(ojDinhMuc);
        }
        [HttpPost]
        [Route("PostDinhMucImport")]
        public string PostDinhMucImport(DataTable ojDinhMuc)
        {
            if (ojDinhMuc == null) return "false";
            string json = JsonConvert.SerializeObject(ojDinhMuc);
            DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDinhMucImport(ojDinhMuc);
        }
        [HttpPost]
        [Route("PostGopDH")]
        public string PostGopDH(DataTable ojGopDH)
        {
            if (ojGopDH == null) return "false";
            return _repo.PostGopDH(ojGopDH);
        }
        [HttpGet]
        [Route("GetKiemTra")]
        public DataTable GetKiemTra(string madh)
        {
            return _repo.GetKiemTra(madh);
        }
        [HttpGet]
        [Route("GetGopID")]
        public DataTable GetGopID(string madh)
        {
            return _repo.GetGopID(madh);
        }
        [HttpPost]
        [Route("DeleteItemCanDoi")]
        public string DeleteItemCanDoi(DataTable _tblDeleteItemCanDoi)
        {
            if (_tblDeleteItemCanDoi == null) return "false";
            return _repo.DeleteItemCanDoi(_tblDeleteItemCanDoi);
        }
        [HttpGet]
        [Route("GetDHClient")]
        public string SearchOrder(string maDVSX, string maLenh)
        {
            CanDoiDonHangModel _model = new CanDoiDonHangModel();
            return _model.SearchOrder(maDVSX, maLenh);
        }

        [HttpPost]
        [Route("PostDinhMucNhap")]
        public string PostDinhMucNhap(DataTable ojDinhMuc)
        {
            if (ojDinhMuc == null) return "false";
            string json = JsonConvert.SerializeObject(ojDinhMuc);
            DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDinhMucNhap(ojDinhMuc);
        }
        [HttpGet]
        [Route("GetCapPhat")]
        public DataTable GetCapPhat(string madh, string _madh, string _malenhsx, string _magop)
        {
            return _repo.GetCapPhat(madh, _madh, _malenhsx, _magop);
        }
        [HttpGet]
        [Route("GetVatTuNPL")]
        public DataTable GetVatTuNPL(string madh, string malenhsx)
        {
            return _repo.GetVatTuNPL(madh, malenhsx);
        }
        [HttpDelete]
        [Route("DeleteNPLNhap")]
        public string DeleteNPLNhap(int id)
        {
            return _repo.DeleteNPLNhap(id);
        }
        [HttpGet]
        [Route("GetSoLuongNhap")]
        public DataTable GetSoLuongNhap(string para, string para1)
        {
            return _repo.GetSoLuongNhap(para, para1);
        }
        [HttpGet]
        [Route("GetSoBookingNhap")]
        public DataTable GetSoBookingNhap(string para)
        {
            return _repo.GetSoBookingNhap(para);
        }
        [HttpGet]
        [Route("GetCDNPL")]

        public DataTable GetCDNPL(string madh, string malenh)
        {
            return _repo.GetCDNPL(madh, malenh);
        }
        [HttpGet]
        [Route("GetTenVTGoiY")]
        public DataTable GetTenVTGoiY()
        {
            return _repo.GetTenVTGoiY();
        }
        [HttpGet]
        [Route("GetMauGoiY")]
        public DataTable GetMauGoiY()
        {
            return _repo.GetMauGoiY();
        }
        [HttpGet]
        [Route("GetKhoSizeGoiY")]
        public DataTable GetKhoSizeGoiY()
        {
            return _repo.GetKhoSizeGoiY();
        }
        [HttpGet]
        [Route("GetDVTinhGoiY")]
        public DataTable GetDVTinhGoiY()
        {
            return _repo.GetDVTinhGoiY();
        }
        [HttpGet]
        [Route("GetNhomGoiY")]
        public DataTable GetNhomGoiY()
        {
            return _repo.GetNhomGoiY();
        }
        [HttpGet]
        [Route("GetSoLuongByMauDH")]
        public DataTable GetSoLuongByMauDH(string para, string para1,string para2)
        {
            return _repo.GetSoLuongByMauDH(para, para1, para2);
        }
        [Route("GetVatTuNPLCapThem")]
        public DataTable GetVatTuNPLCapThem(string madh, string malenhsx)
        {
            return new CanDoiDonHangModel().GetVatTuCapThem(madh, malenhsx);
        }
        [HttpPost]
        [Route("PostDinhMucNhapCapThem")]
        public string PostDinhMucNhapCapThem(DataTable ojDinhMuc)
        {
            if (ojDinhMuc == null) return "false";
            string json = JsonConvert.SerializeObject(ojDinhMuc);
            DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
            return new CanDoiDonHangModel().PostDinhMucNhapCapThem(ojDinhMuc);
        }
        [Route("GetVatTuDotCapThem")]
        public DataTable GetVatTuDotCapThem(string para)
        {
            return new CanDoiDonHangModel().GetVatTuDotCapThem(para);
        }
        [HttpDelete]
        [Route("DeleteAllCapThem")]
        public string DeleteAllCapThem(string madh, string malenhsanxuat, string username)
        {
            return new CanDoiDonHangModel().DeleteAllCapThem(madh, malenhsanxuat, username);
        }
        [HttpDelete]
        [Route("DeleteCapThem")]
        public string DeleteCapThem(string para)
        {
            return new CanDoiDonHangModel().DeleteCapThem(para);
        }
        [HttpPost]
        [Route("PostDot")]
        public string PostDot(DataTable ojDinhMuc)
        {
            if (ojDinhMuc == null) return "false";
            string json = JsonConvert.SerializeObject(ojDinhMuc);
            DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDot(ojDinhMuc);
        }
        [HttpDelete]
        [Route("DeleteCapThemByDot")]
        public string DeleteCapThemByDot(string para, string para1)
        {
            return new CanDoiDonHangModel().DeleteCapThemByDot(para, para1);
        }

        [HttpPost]
        [Route("PostDinhMucNhapMoi")]
        public string PostDinhMucNhapMoi(DataTable ojDinhMuc)
        {
            if (ojDinhMuc == null) return "false";
            string json = JsonConvert.SerializeObject(ojDinhMuc);
            DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
            return new CanDoiDonHangModel().PostDinhMucNhapMoi(ojDinhMuc);
        }
        [HttpPost]
        [Route("PostDinhMucNPL")]
        public string PostDinhMucNPL(DataTable ojDinhMuc)
        {
            if (ojDinhMuc == null) return "false";
            string json = JsonConvert.SerializeObject(ojDinhMuc);
            DataTable tbDinhMuc = JsonConvert.DeserializeObject<DataTable>(json);
            return new CanDoiDonHangModel().PostDinhMucnpl(ojDinhMuc);
        }

        [Route("GetChuyenChinh")]
        public DataTable GetChuyenChinh(string madh)
        {
            return new CanDoiDonHangModel().GetChuyenChinh(madh);
        }

        #region xoa lenh sx
        [Route("GetIsLenChuyen")]
        public DataTable GetIsLenChuyen()
        {
            return new CanDoiDonHangModel().GetIsLenChuyen();
        }

        [HttpDelete]
        [Route("DeleteLenhSX")]
        public string DeleteLenhSX(string madh, string malenh, string malenhsanxuat, string username)
        {
            return new CanDoiDonHangModel().DeleteLenhSX(madh, malenh, malenhsanxuat, username);
        }
        #endregion

        [HttpGet]
        [Route("GetCheckEdiCanDoiSX")]
        public DataTable GetCheckEdiCanDoiSX(string madh, string malenh, string poid, string mamau, string dausizeid)
        {
            return new CanDoiDonHangModel().GetCheckEdiCanDoiSX(madh, malenh, poid, mamau, dausizeid);
        }

        [Route("GetIsLSX")]
        public DataTable GetIsLSX(string madh, string poid)
        {
            return new CanDoiDonHangModel().GetIsLSX(madh, poid);
        }
        [HttpDelete]
        [Route("DeletePOCD")]
        public string DeletePOCD(string madh, string poid)
        {
            return new CanDoiDonHangModel().DeletePOcd(madh, poid);
        }
        [HttpGet]
        [Route("GetGomNPLKH")]

        public DataTable GetGomNPLKH(string parameter, string parameter1, string parameter2)
        {
            return new CanDoiDonHangModel().GetGomNPLKH(parameter, parameter1, parameter2);
        }
        [HttpGet]
        [Route("UpdateGhiChuQLSX")]
        public string UpdateGhiChuQLSX(string maDH)
        {
            return new CanDoiDonHangModel().UpdateGhiChuQLSX(maDH);
        }
        [HttpGet]
        [Route("GetSize")]
        public DataTable GetSize(string parameter, string action)
        {
            return new CanDoiDonHangModel().GetSize(parameter, action);
        }
    }
}