using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Model.QuanLyDonHang;
using NtbSoft.ERP.Web.Api.Hubs;
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
    [RoutePrefix("api/DonHangTong")]
    public class DonHangTongController : ApiController
    {
        IDonHangTongRepository _repo = new DonHangTongRepository();
        [HttpGet]
        [Route("GetDonHangTong")]
        public DataTable GetDonHangTong(int pageIndex, int pageSize)
        {
            return _repo.GetDonHangTong(pageIndex, pageSize);
        }
        [HttpGet]
        [Route("GetKT")]
        public DataTable GetKT(string madh)
        {
            return _repo.GetKT(madh);
        }
        [HttpPost]
        [Route("PostDonHangTong")]
        public string PostDonHangTong(List<DonHangTongSaveEntity> ojDonHangTong)
        {
            if (ojDonHangTong == null) return "false";
            string json = JsonConvert.SerializeObject(ojDonHangTong);
            DataTable tbDonHangTong = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDonHangTong(tbDonHangTong);
        }
        [HttpPost]
        [Route("PostDonHangTongV1")]
        public string PostDonHangTongV1(List<DonHangTongSaveEntityV1> ojDonHangTong)
        {
            if (ojDonHangTong == null) return "false";
            string json = JsonConvert.SerializeObject(ojDonHangTong);
            DataTable tbDonHangTong = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDonHangTongV1(tbDonHangTong);
        }
        [HttpPost]
        [Route("PostAutoImportBangSize")]
        public string PostAutoImportBangSize(DataTable ojBangSize)
        {
            if (ojBangSize == null) return "false";
            string json = JsonConvert.SerializeObject(ojBangSize);
            DataTable tbDonHangTong = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostAutoImportBangSize(tbDonHangTong);
        }
        [HttpPost]
        [Route("PostDonHangTongPO")]
        public string PostDonHangTongPO(List<DonHangTongPOEntity> ojDonHangTongPO)
        {
            if (ojDonHangTongPO == null) return "false";
            string json = JsonConvert.SerializeObject(ojDonHangTongPO);
            DataTable tbDonHangTongPO = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDonHangTongPO(tbDonHangTongPO);
        }
        [HttpPost]
        [Route("PostDonHangTongPOUser")]
        public string PostDonHangTongPOUser(List<DonHangTongPOUSEREntity> ojDonHangTongPO)
        {
            if (ojDonHangTongPO == null) return "false";
            string json = JsonConvert.SerializeObject(ojDonHangTongPO);
            DataTable tbDonHangTongPO = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDonHangTongPOUser(tbDonHangTongPO);
        }
        // DonHangTongPOChiTiet
        [HttpGet]
        [Route("GetPivotDonHangTongPOChiTiet")]
        public DataTable GetChiTietDonHangTongPO(string maDH)
        {
            return _repo.GetChiTietDonHangTongPO(maDH);
        }
        [HttpPost]
        [Route("PostDonHangTongPOChiTiet")]
        public string PostDonHangTongPOChiTiet(List<DonHangTongPOChiTietEntity> ojDonHangTongPOChiTiet)
        {
            if (ojDonHangTongPOChiTiet == null) return "false";
            string json = JsonConvert.SerializeObject(ojDonHangTongPOChiTiet);
            DataTable tbDonHangTongPOChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
            return _repo.PostDonHangTongPOChiTiet(tbDonHangTongPOChiTiet);

        }
        //SignalR
        private void NotifyOrderCreated(string maDH)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<WipHub>();
            hub.Clients.All.orderCreated(new { maDH = maDH });
        }
        [HttpPost]
        [Route("PostDonHangTongPOChiTietUser")]
        public string PostDonHangTongPOChiTiet(List<DonHangTongPOChiTietUSEREntity> ojDonHangTongPOChiTiet)
        {
            if (ojDonHangTongPOChiTiet == null) return "false";
            string json = JsonConvert.SerializeObject(ojDonHangTongPOChiTiet);
            DataTable tbDonHangTongPOChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
            //return _repo.PostDonHangTongPOChiTietUser(tbDonHangTongPOChiTiet);
            var result = _repo.PostDonHangTongPOChiTietUser(tbDonHangTongPOChiTiet); // "True" / throw

            if (string.Equals(result, "True", StringComparison.OrdinalIgnoreCase))
            {
                // Lấy MaDH từ payload (bạn chọn cách nào tiện nhất)
                // Ví dụ: item đầu tiên có POID "DH_xxx|..."
                var maDH = ojDonHangTongPOChiTiet[0]?.POID?.Split('|')[0]; // hoặc truyền thẳng MaDH trong entity

                if (!string.IsNullOrEmpty(maDH))
                    NotifyOrderCreated(maDH);
            }

            return result;
        }
        [HttpPost]
        [Route("UpdateSLDonHangTongPOChiTiet")]
        public string UpdateSLDonHangTongPOChiTiet(DataTable dataTable)
        {
            if (dataTable == null) return "false";
            return _repo.UpdateSLDonHangTongPOChiTiet(dataTable);
        }
        [HttpDelete]
        [Route("Delete")]
        public string Delete(string madonhang, string username)
        {
            return new DonHangTongModel().Delete(madonhang, username);
        }
        [HttpDelete]
        [Route("DeleteNPL")]
        public string DeleteNPL(string madonhang)
        {
            return new DonHangTongModel().DeleteNPL(madonhang);
        }
        [HttpGet]
        [Route("GetMaHangCT")]
        public DataTable GetMaHangCT(string donhang)
        {
            return new DonHangTongModel().GetMaHangCT(donhang);
        }
        [HttpGet]
        [Route("GetDonHangTongID")]
        public DataTable GetDonHangTongID()
        {
            return _repo.GetDonHangTongID();
        }
        [Route("GetDonHangTongMaHang")]
        public DataTable GetDonHangTongMaHang(string mahang)
        {
            return _repo.GetDonHangTongMaHang(mahang);
        }
        [Route("GetDsBaoCao")]
        public DataTable GetDsBaoCao(string maDH)
        {
            return _repo.GetDsBaoCao(maDH);
        }
        [HttpPost]
        [Route("DeleteDonHangTongPOChiTiet")]
        public string DeleteDonHangTongPOChiTiet(DataTable dt)
        {
            return _repo.DeleteDonHangTongPOChiTiet(dt);
        }
        [HttpPost]
        [Route("DeleteDonHangTongPO")]
        public string DeleteDonHangTongPO(DataTable dt)
        {
            return _repo.DeleteDonHangTongPO(dt);
        }
        [HttpPost]
        [Route("DeleteDonHangTong")]
        public string DeleteDonHangTong(DataTable dt)
        {
            return _repo.DeleteDonHangTong(dt);
        }
        [HttpPost]
        [Route("GetPOCanDoi")]
        public DataTable GetPOCanDoi(DataTable dt)
        {
            return _repo.GetPOCanDoi(dt);
        }
        [HttpGet]
        [Route("CheckCanDoi")]
        public DataTable CheckCanDoi(string maDH)
        {
            return _repo.CheckCanDoi(maDH);
        }
        [HttpGet]
        [Route("GetKiemTraDH")]
        public string GetKiemTraDH(string madh)
        {
            return _repo.GetKiemTraDH(madh);
        }
        [HttpGet]
        [Route("SearchDH")]
        public DataTable SearchDH(string maHang, string khachHang, string chungLoai, string dot, int soLuong, string maLenh, string dvsx, string po)
        {
            return _repo.SearchDH(maHang, khachHang, chungLoai, dot, soLuong, maLenh, dvsx, po);
        }
        [HttpGet]
        [Route("GetBangSizeKT")]
        public DataTable GetBangSizeKT(string mahang, string makh)
        {
            return _repo.GetBangSizeKT(mahang, makh);
        }
        [HttpGet]
        [Route("GetKhachHang")]
        public DataTable GetKhachHang()
        {
            return new DonHangTongModel().GetKhachHang();
        }
        [HttpGet]
        [Route("GetHangHoaKH")]
        public DataTable GetHangHoaKH(string makh)
        {
            return new DonHangTongModel().GetHangHoaKH(makh);
        }
        [HttpGet]
        [Route("GetSize")]
        public DataTable GetSize(string mahang, string makh)
        {
            return new DonHangTongModel().GetSize(mahang, makh);
        }
        [HttpGet]
        [Route("GetMau")]
        public DataTable GetMau(string mahang, string makh)
        {
            return new DonHangTongModel().GetMau(mahang, makh);
        }
        [HttpGet]
        [Route("GetChungLoai")]
        public DataTable GetChungLoai(string mahang, string makh)
        {
            return new DonHangTongModel().GetChungLoai(mahang, makh);
        }

        [HttpGet]
        [Route("GetChungLoai1")]
        public DataTable GetChungLoai(string chungloai)
        {
            return new DonHangTongModel().GetChungLoai(chungloai);
        }
        [HttpGet]
        [Route("GetDauSize")]
        public DataTable GetDauSize(string mahang, string makh)
        {
            return new DonHangTongModel().GetDauSize(mahang, makh);
        }
        [HttpGet]
        [Route("GetDauSizeV1")]
        public DataTable GetDauSizeV1(string mahang, string makh)
        {
            return new DonHangTongModel().GetDauSizeV1(mahang, makh);
        }
        [HttpGet]
        [Route("GetSizeEdit")]
        public DataTable GetSizeEdit(string mahang, string makh, string dausize)
        {
            return new DonHangTongModel().GetSizeEdit(mahang, makh, dausize);
        }
        [HttpGet]
        [Route("GetSizeEditV1")]
        public DataTable GetSizeEditV1(string mahang, string makh, string dausize)
        {
            return new DonHangTongModel().GetSizeEditV1(mahang, makh, dausize);
        }
        [HttpPost]
        [Route("PostThuVienMau")]
        public string PostThuVienMau(DataTable tblThuVienMau)
        {
            return new DonHangTongModel().PostThuVienMau(tblThuVienMau);
        }
        #region Phú
        [HttpGet]
        [Route("GetCapPhatThongSo")]
        public DataTable GetCapPhatThongSo(string makh, string mahang, string madot, string magop, string malenh, string madh)
        {
            return new DonHangTongModel().GetCapPhatThongSo(makh, mahang, madot, magop, malenh, madh);
        }
        [HttpGet]
        [Route("GetCapPhatThongSoChiTiet")]
        public DataTable GetCapPhatThongSoChiTiet(string makh, string mahang, string mavtid, string mauid, string malsx, string madot)
        {
            return new DonHangTongModel().GetCapPhatThongSoChiTiet(makh, mahang, mavtid, mauid, malsx, madot);
        }
        [HttpGet]
        [Route("GetCapPhatThongSoDot")]
        public DataTable GetCapPhatThongSoDot(string makh, string mahang)
        {
            return new DonHangTongModel().GetCapPhatThongSoDot(makh, mahang);
        }
        [HttpGet]
        [Route("GetChiTietDM")]
        public DataTable GetChiTietDM(string makh, string mahang, string mavtID, string madot, string mauID, string tachmau, string manhom, string khovaiID, string macode)
        {
            return new DonHangTongModel().GetChiTietDM(makh, mahang, mavtID, madot, mauID, tachmau, manhom, khovaiID, macode);
        }
        [HttpGet]
        [Route("GetChiTietLenhSX")]
        public DataTable GetChiTietLenhSX(string magop, string malenhsanxuat)
        {
            return new DonHangTongModel().GetChiTietLenhSX(magop, malenhsanxuat);
        }
        [HttpPost]
        [Route("PostCapPhatLSX")]
        public string PostCapPhatLSX(DataTable tblSave)
        {
            return new DonHangTongModel().PostCapPhatLSX(tblSave);
        }
        [HttpPost]
        [Route("PostCapThemLSX")]
        public string PostCapThemLSX(DataTable tblSave)
        {
            return new DonHangTongModel().PostCapThemLSX(tblSave);
        }
        [HttpPost]
        [Route("PostCapThemLSXSua")]
        public string PostCapThemLSXSua(DataTable tblSave)
        {
            return new DonHangTongModel().PostCapThemLSXSua(tblSave);
        }
        [HttpGet]
        [Route("GetCapPhatCanDoi")]
        public DataTable GetCapPhatCanDoi(string magop, string malenhsanxuat)
        {
            return new DonHangTongModel().GetCapPhatCanDoi(magop, malenhsanxuat);
        }
        [HttpGet]
        [Route("GetCapPhatCanDoiCT")]
        public DataTable GetCapPhatCanDoiCT(string magop, string malenhsanxuat, string madot, string makh, string mahang)
        {
            return new DonHangTongModel().GetCapPhatCanDoiCT(magop, malenhsanxuat, madot, makh, mahang);
        }
        [HttpGet]
        [Route("GetCapThemLichSu")]
        public DataTable GetCapThemLichSu(string magop, string malenhsanxuat, string madot, string manpl)
        {
            return new DonHangTongModel().GetCapThemLichSu(magop, malenhsanxuat, madot, manpl);
        }
        [HttpGet]
        [Route("GetCapPhatDotCanDoi")]
        public DataTable GetCapPhatDotCanDoi(string magop, string malenhsanxuat)
        {
            return new DonHangTongModel().GetCapPhatDotCanDoi(magop, malenhsanxuat);
        }
        [HttpGet]
        [Route("GetCapPhatCanDoi")]
        public DataTable GetCapPhatCanDoi(string magop, string malenhsanxuat, string madot)
        {
            return new DonHangTongModel().GetCapPhatCanDoi(magop, malenhsanxuat, madot);
        }
        #endregion
        [HttpGet]
        [Route("GetCD")]
        public DataTable GetCD(string maDH)
        {
            return new DonHangTongModel().GetCD(maDH);
        }
        [HttpDelete]
        [Route("DeleteCapThem")]
        public string DeleteCapThem(string madh, string malenhsanxuat, string manpl, string madot, string dot)
        {
            return new DonHangTongModel().DeleteCapThem(madh, malenhsanxuat, manpl, madot, dot);
        }

        #region SuaDonHangTong_GetSort
        [HttpGet]
        [Route("GetSort")]
        public DataTable GetSort(string madh)
        {
            return new DonHangTongModel().GetSort(madh);
        }
        #endregion

        #region TanSuat
        [HttpGet]
        [Route("GetMHCount")]
        public DataTable GetMHCount(string mahang)
        {
            return new DonHangTongModel().GetMaHangCount(mahang);
        }
        #endregion

        [HttpPost]
        [Route("PostMau")]
        public string PostMau(DataTable tblmau)
        {
            return new DonHangTongModel().PostMau(tblmau);
        }

        [HttpPost]
        [Route("PostSize")]
        public string PostSize(DataTable tblsize)
        {
            return new DonHangTongModel().PostSize(tblsize);
        }

        #region coppy mahang
        [HttpGet]
        [Route("Getdhcoppy")]
        public DataTable Getdhcoppy(string makh)
        {
            return new DonHangTongModel().GetDonHangTongCoppy(makh);
        }

        [HttpGet]
        [Route("Getctdhcoppy")]
        public DataTable Getctdhcoppy(string madh)
        {
            return new DonHangTongModel().GetChiTietDonHangTongPOCoppy(madh);
        }
        #endregion
        [HttpGet]
        [Route("GetALLDH")]
        public DataTable GetALLDH()
        {
            return new DonHangTongModel().GetAllDH();
        }
        [HttpGet]
        [Route("GetDeleteSize")]
        public DataTable GetDeleteSize(string madh)
        {
            return new DonHangTongModel().GetDeleteSize(madh);
        }
        [HttpGet]
        [Route("GetKTSize")]
        public DataTable GetKTSize(string madh, string sizeid)
        {
            return new DonHangTongModel().GetKTSize(madh, sizeid);
        }
        [HttpDelete]
        [Route("DeleteSize")]
        public string DeleteSize(string madh, string sizeid, string username)
        {
            return new DonHangTongModel().DeleteSize(madh, sizeid, username);
        }
        [HttpGet]
        [Route("GhiLogSuaDH")]
        public string GhiLogSuaDH(string mahang, string makh, string madh, string username)
        {
            return new DonHangTongModel().GhiLogSuaDH(mahang, makh, madh, username);
        }
    }
}