using NtbSoft.ERP.Model.CanDoiDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.DonHangTonKho
{
    public interface IDonHangTonKhoRepository
    {
        DataTable GetDonHangTonKho();
        DataTable GetDSDonHangTonKho(string maDH, string maHang);
        DataTable GetTonKhoChiTiet(string madh,string maHang);
        DataTable GetMaxDH();
        DataTable GetPivotSLKH_SLSX(string maDH);

        DataTable GetPivotSLKho(string maHang);

        string InsertDataTruTonKho(DataTable tblTruTonKho);

        string UpdateSLTonKho(DataTable tblTruTonKho, string nguoitao);
        DataTable GetLichSuTruTonKho(string maHang);
        DataTable GetBangMau(string mahang);
        DataTable GetBangSize(string mahang);
        string PostNhapTonKho(object ojNhapTonKho,string nguoiTao);
        DataTable GetTonKho(int pageIndex, int pageSize);
        DataTable GetKiemTra(string mahang, string poid, string mamau, string dausizeid, string sizeid);
        string DeleteTonKho(int parameter);
        string AddDonHangTonKho(string nguoiTao,DataTable tblTonKho);
        DataTable GetKHDTTonKho();
    }
    public class DonHangTonKhoRepository : IDonHangTonKhoRepository
    {
        DonHangTonKhoModel donhangTonKhoModel = new DonHangTonKhoModel();
        public DataTable GetDonHangTonKho()
        {
            var dt = donhangTonKhoModel.GetDonHangTonKho();
            return dt;
        }
        public DataTable GetDSDonHangTonKho(string maDH,string maHang)
        {
            var dt = donhangTonKhoModel.GetDSDonHangTonKho(maDH,maHang);
            return dt;
        }
        public DataTable GetTonKhoChiTiet(string madh,string maHang)
        {
            var dt = donhangTonKhoModel.GetTonKhoChiTiet(madh,maHang);
            return dt;
        }
        public DataTable GetMaxDH()
        {
            var dt = donhangTonKhoModel.GetMaxDH();
            return dt;
        }
        public DataTable GetPivotSLKH_SLSX(string maDH)
        {
            var dt = donhangTonKhoModel.GetPivotSLKH(maDH);
            return dt;
        }

        public DataTable GetPivotSLKho(string maHang)
        {
            var dt = donhangTonKhoModel.GetPivotSLKho(maHang);
            return dt;
        }

        public string InsertDataTruTonKho(DataTable tblTruTonKho)
        {
            string result = donhangTonKhoModel.InsertDataTruTonKho(tblTruTonKho);
            return result;
        }

        public string UpdateSLTonKho(DataTable tblTonKho, string nguoitao)
        {
            string result = donhangTonKhoModel.UpdateSLTonKho(tblTonKho, nguoitao);
            return result;
        }

        public DataTable GetLichSuTruTonKho(string maHang)
        {
            var dt = donhangTonKhoModel.GetLichSuTruTonKho(maHang);
            return dt;
        }

        public DataTable GetBangMau(string maHang)
        {
            var dt = donhangTonKhoModel.GetBangMau(maHang);
            return dt;
        }
        public DataTable GetBangSize(string maHang)
        {
            var dt = donhangTonKhoModel.GetBangSize(maHang);
            return dt;
        }
        public string PostNhapTonKho(object ojNhapTonKho,string nguoiTao)
        {
            var result = donhangTonKhoModel.PostNhapTonKho(ojNhapTonKho,nguoiTao);
            return result;
        }
        public DataTable GetTonKho(int pageIndex, int pageSize)
        {
            var dt = donhangTonKhoModel.GetTonKho(pageIndex, pageSize);
            return dt;
        }
        public DataTable GetKiemTra(string mahang, string poid, string mamau, string dausizeid, string sizeid)
        {
            var dt = donhangTonKhoModel.GetKiemTra(mahang, poid, mamau, dausizeid, sizeid);
            return dt;
        }
        public string DeleteTonKho(int parameter)
        {
            var dt = donhangTonKhoModel.DeleteTonKho(parameter);
            return dt;
        }
        public string AddDonHangTonKho(string nguoiTao,DataTable tblTonKho)
        {
            string result = donhangTonKhoModel.AddDonHangTonKho(nguoiTao,tblTonKho);
            return result;
        }

        public DataTable GetKHDTTonKho()
        {
            return donhangTonKhoModel.GetKHDTTonKho();
        }
    }
}