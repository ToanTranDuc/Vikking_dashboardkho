using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.QuanLyDonHang
{
    public interface IDonHangTongRepository
    {
        DataTable GetDonHangTong(int pageIndex, int pageSize);
        DataTable GetKT(string madh);
        string PostDonHangTong(object ojDonHangTong);
        string PostDonHangTongV1(object ojDonHangTong);
        string PostDonHangTongPO(object ojDonHangTongPO);
        string PostDonHangTongPOUser(object ojDonHangTongPO);
        string PostAutoImportBangSize(object ojBangSize);
        DataTable GetChiTietDonHangTongPO(string maDH);
        string PostDonHangTongPOChiTiet(object ojDonHangTongPOChiTiet);
        string PostDonHangTongPOChiTietUser(object ojDonHangTongPOChiTiet);
        DataTable GetDonHangTongID();
        DataTable GetDonHangTongMaHang(string mahang);
        string UpdateSLDonHangTongPOChiTiet(DataTable dataTable);
        DataTable GetDsBaoCao(string maDH);

        string DeleteDonHangTongPOChiTiet(DataTable dataTable);
        string DeleteDonHangTongPO(DataTable dataTable);
        string DeleteDonHangTong(DataTable dataTable);
        DataTable GetPOCanDoi(DataTable dataTable);
        DataTable CheckCanDoi(string _maDH);
        string GetKiemTraDH(string madh);
        DataTable SearchDH(string maHang, string khachHang, string chungLoai, string dot, int soLuong, string malenh, string dvsx, string po);
        DataTable GetBangSizeKT(string mahang, string makh);
    }
    public class DonHangTongRepository : IDonHangTongRepository
    {
        DonHangTongModel donhangtongmodel = new DonHangTongModel();
        public DataTable GetDonHangTong(int pageIndex, int pageSize)
        {
            var dt = donhangtongmodel.GetDonHangTong(pageIndex, pageSize);
            return dt;
        }
        public DataTable GetKT(string madh)
        {
            var dt = donhangtongmodel.GetKT(madh);
            return dt;
        }
        public string PostDonHangTong(object ojDonHangTong)
        {
            var result = donhangtongmodel.PostDonHangTong(ojDonHangTong);
            return result;
        }
        public string PostDonHangTongV1(object ojDonHangTong)
        {
            var result = donhangtongmodel.PostDonHangTongV1(ojDonHangTong);
            return result;
        }
        public string PostAutoImportBangSize(object ojBangSize)
        {
            var result = donhangtongmodel.PostAutoImportBangSize(ojBangSize);
            return result;
        }
        public string PostDonHangTongPO(object ojDonHangTongPO)
        {
            var result = donhangtongmodel.PostDonHangTongPO(ojDonHangTongPO);
            return result;
        }
        public string PostDonHangTongPOUser(object ojDonHangTongPO)
        {
            var result = donhangtongmodel.PostDonHangTongPOUser(ojDonHangTongPO);
            return result;
        }
        public DataTable GetChiTietDonHangTongPO(string maDH)
        {
            var dt = donhangtongmodel.GetChiTietDonHangTongPO(maDH);
            return dt;
        }
        public string PostDonHangTongPOChiTiet(object ojDonHangTongPOChiTiet)
        {
            var result = donhangtongmodel.PostDonHangTongPOChiTiet(ojDonHangTongPOChiTiet);
            return result;
        }
        public string PostDonHangTongPOChiTietUser(object ojDonHangTongPOChiTiet)
        {
            var result = donhangtongmodel.PostDonHangTongPOChiTietUser(ojDonHangTongPOChiTiet);
            return result;
        }
        public DataTable GetDonHangTongID()
        {
            var dt = donhangtongmodel.GetDonHangTongID();
            return dt;
        }
        public DataTable GetDonHangTongMaHang(string mahang)
        {
            var dt = donhangtongmodel.GetDonHangTongMaHang(mahang);
            return dt;
        }
        public string UpdateSLDonHangTongPOChiTiet(DataTable dataTable)
        {
            string dt = donhangtongmodel.UpdateSLDonHangTongPOChiTiet(dataTable);
            return dt;
        }
        public DataTable GetDsBaoCao(string maDH)
        {
            var dt = donhangtongmodel.GetDsBaoCao(maDH);
            return dt;
        }
        public string DeleteDonHangTongPOChiTiet(DataTable dataTable)
        {
            string dt = donhangtongmodel.DeleteDonHangTongPOChiTiet(dataTable);
            return dt;
        }
        public string DeleteDonHangTongPO(DataTable dataTable)
        {
            string dt = donhangtongmodel.DeleteDonHangTongPO(dataTable);
            return dt;
        }

        public string DeleteDonHangTong(DataTable dataTable)
        {
            string dt = donhangtongmodel.DeleteDonHangTong(dataTable);
            return dt;
        }
        public DataTable GetPOCanDoi(DataTable dataTable)
        {
            DataTable dt = donhangtongmodel.GetPOCanDoi(dataTable);
            return dt;
        }
        public DataTable CheckCanDoi(string _maDH)
        {
            DataTable dt = donhangtongmodel.CheckCanDoi(_maDH);
            return dt;
        }
        public string GetKiemTraDH(string madh)
        {
            var result = donhangtongmodel.GetKiemTraDH(madh);
            return result;
        }
        public DataTable SearchDH(string maHang, string khachHang, string chungLoai, string dot, int soLuong, string malenh, string dvsx, string po)
        {
            var result = donhangtongmodel.SearchDH(maHang, khachHang, chungLoai, dot, soLuong, malenh, dvsx, po);
            return result;
        }
        public DataTable GetBangSizeKT(string mahang, string makh)
        {
            var dt = donhangtongmodel.GetBangSizeKT(mahang, makh);
            return dt;
        }
    }

}