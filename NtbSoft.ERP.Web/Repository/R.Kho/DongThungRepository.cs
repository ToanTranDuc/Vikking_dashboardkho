using NtbSoft.ERP.Model.Kho;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.Kho
{
    public interface IDongThungRepository
    {
        DataTable GetDS();
        DataTable GetLenhSX(string statuschuyen, string username,string status);
        DataTable GetDS_KHDT(string maplk, string mahang, string po, string statuschuyen, string madvsx, string isDecat, string dotsx);
        DataTable GetDSThung(string maplk, string po, string mahang, string tuthung, string denthung, string madvsx, string isDeCat, string dotsx);
        DataTable Search(string malenh, string para);
        DataTable GetThongTin(string madh, string pageIndex, string pageSize, string madvsx);
        DataTable GetThongTinSMS(string madh, string pageIndex, string pageSize, string madvsx);
        DataTable GetThongTinPO(string madh, string madvsx);
        DataTable GetThongTinPOSMS(string madh, string madvsx);
        DataTable GetThongTinPOCT(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat);
        DataTable GetThongTinPOCTSMS(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat);
        DataTable GetThongTinChiTiet(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int minsttthung, int maxsttthung);
        DataTable GetThongTinChiTietSMS(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int minsttthung, int maxsttthung);
        string Post(DataTable tblNhapKho, bool isdongthung);
        string PostDT(DataTable tblNhapKho, bool isdongthung);
        string PostHuy(DataTable tblNhapKho, bool isdongthung);
        string PostChiTiet(DataTable tblNhapKho, bool isdongthung);
        string PostALL(DataTable tblNhapKho, int value);
        string PostViTri(DataTable tblNhapKho, bool isdongthung);
        string PostSMS(DataTable tblNhapKho, bool isdongthung);
        string PostDTSMS(DataTable tblNhapKho, bool isdongthung);
        string PostHuySMS(DataTable tblNhapKho, bool isdongthung);
        string PostChiTietSMS(DataTable tblNhapKho, bool isdongthung);
        string PostALLSMS(DataTable tblNhapKho, int value);
        string PostViTriSMS(DataTable tblNhapKho, bool isdongthung);
        string PostDongThung(DataTable tblDongThung);
        DataTable GetDsTongQuanDongThung(string maHang, DateTime ngayDongThung, string maDVSX);
        DataTable GetDsXuatNhapTon(string maHang, string maDVSX, int isChecked);
        DataTable GetDsXuatNhapTonLuanChuyen(string maHang, string maDVSX, int isChecked);
        DataTable GetChiTietDongThung(DataTable tblChiTiet);
    }
    public class DongThungRepository : IDongThungRepository
    {
        DongThungModel nhapkhomodel = new DongThungModel();
        public DataTable GetDS()
        {
            var dt = nhapkhomodel.GetDS();
            return dt;
        }
        public DataTable GetThongTin(string madh, string pageIndex, string pageSize, string madvsx)
        {
            var dt = nhapkhomodel.GetThongTin(madh, pageIndex, pageSize, madvsx);
            return dt;
        }
        public DataTable GetThongTinSMS(string madh, string pageIndex, string pageSize, string madvsx)
        {
            var dt = nhapkhomodel.GetThongTinSMS(madh, pageIndex, pageSize, madvsx);
            return dt;
        }
        public DataTable GetThongTinPO(string madh, string madvsx)
        {
            var dt = nhapkhomodel.GetThongTinPO(madh, madvsx);
            return dt;
        }
        public DataTable GetThongTinPOSMS(string madh, string madvsx)
        {
            var dt = nhapkhomodel.GetThongTinPOSMS(madh, madvsx);
            return dt;
        }
        public DataTable GetThongTinPOCT(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat)
        {
            var dt = nhapkhomodel.GetThongTinPOCT(mapkl, madvsx, malenh, poid, madh, malenhsanxuat);
            return dt;
        }
        public DataTable GetThongTinPOCTSMS(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat)
        {
            var dt = nhapkhomodel.GetThongTinPOCTSMS(mapkl, madvsx, malenh, poid, madh, malenhsanxuat);
            return dt;
        }
        public DataTable GetThongTinChiTiet(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int minsttthung, int maxsttthung)
        {
            var dt = nhapkhomodel.GetThongTinChiTiet(mapkl, malenh, madvsx, poid, dausizeid, mau, tuthung, denthung, minsttthung, maxsttthung);
            return dt;
        }
        public DataTable GetThongTinChiTietSMS(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int minsttthung, int maxsttthung)
        {
            var dt = nhapkhomodel.GetThongTinChiTietSMS(mapkl, malenh, madvsx, poid, dausizeid, mau, tuthung, denthung, minsttthung, maxsttthung);
            return dt;
        }
        public string Post(DataTable tblNhapKho,  bool isdongthung)
        {
            string result = nhapkhomodel.Post(tblNhapKho, isdongthung);
            return result;
        }
        public string PostSMS(DataTable tblNhapKho, bool isdongthung)
        {
            string result = nhapkhomodel.PostSMS(tblNhapKho, isdongthung);
            return result;
        }
        public string PostDT(DataTable tblNhapKho, bool isdongthung)
        {
            string result = nhapkhomodel.PostDT(tblNhapKho, isdongthung);
            return result;
        }
        public string PostDTSMS(DataTable tblNhapKho, bool isdongthung)
        {
            string result = nhapkhomodel.PostDTSMS(tblNhapKho, isdongthung);
            return result;
        }
        public string PostHuy(DataTable tblNhapKho, bool isdongthung)
        {
            string result = nhapkhomodel.PostHuy(tblNhapKho, isdongthung);
            return result;
        }
        public string PostHuySMS(DataTable tblNhapKho, bool isdongthung)
        {
            string result = nhapkhomodel.PostHuySMS(tblNhapKho, isdongthung);
            return result;
        }
        public string PostChiTiet(DataTable tblNhapKho, bool isdongthung)
        {
            string result = nhapkhomodel.PostChiTiet(tblNhapKho, isdongthung);
            return result;
        }
        public string PostChiTietSMS(DataTable tblNhapKho, bool isdongthung)
        {
            string result = nhapkhomodel.PostChiTietSMS(tblNhapKho, isdongthung);
            return result;
        }
        public string PostALL(DataTable tblNhapKho, int value)
        {
            string result = nhapkhomodel.PostALL(tblNhapKho, value);
            return result;
        }
        public string PostALLSMS(DataTable tblNhapKho, int value)
        {
            string result = nhapkhomodel.PostALLSMS(tblNhapKho, value);
            return result;
        }
        public string PostViTri(DataTable tblNhapKho, bool isdongthung)
        {
            string result = nhapkhomodel.PostViTri(tblNhapKho, isdongthung);
            return result;
        }
        public string PostViTriSMS(DataTable tblNhapKho, bool isdongthung)
        {
            string result = nhapkhomodel.PostViTriSMS(tblNhapKho, isdongthung);
            return result;
        }
        //coding by Dat
        //start
        public DataTable GetLenhSX(string statuschuyen, string username, string status)
        {
            var dt = nhapkhomodel.GetLenhSX(statuschuyen, username, status);
            return dt;
        }

        public DataTable GetDS_KHDT(string maplk, string mahang, string po, string statuschuyen, string madvsx, string isDecat, string dotsx)
        {
            var dt = nhapkhomodel.GetDS_KHDT(maplk, mahang, po, statuschuyen, madvsx, isDecat, dotsx);
            return dt;
        }

        public DataTable GetDSThung(string maplk, string po, string mahang, string tuthung, string denthung, string madvsx, string isDeCat, string dotsx)
        {
            var dt = nhapkhomodel.GetDSThung(maplk, po, mahang, tuthung, denthung, madvsx, isDeCat, dotsx);
            return dt;
        }

        public string PostDongThung(DataTable tblDongThung)
        {
            string result = nhapkhomodel.PostDongThung(tblDongThung);
            return result;
        }

        public DataTable Search(string malenh, string para)
        {
            var dt = nhapkhomodel.Search(malenh, para);
            return dt;
        }
        //end
        public DataTable GetDsTongQuanDongThung(string maHang, DateTime ngayDongThung, string maDVSX)
        {
            var dt = nhapkhomodel.GetDsTongQuanDongThung(maHang, ngayDongThung, maDVSX);
            return dt;
        }

        public DataTable GetDsXuatNhapTon(string maHang, string maDVSX, int isChecked)
        {
            var dt = nhapkhomodel.GetDsXuatNhapTon(maHang, maDVSX,isChecked);
            return dt;
        }

        public DataTable GetDsXuatNhapTonLuanChuyen(string maHang, string maDVSX,int isChecked)
        {
            var dt = nhapkhomodel.GetDsXuatNhapTonLuanChuyen(maHang, maDVSX,isChecked);
            return dt;
        }

        // BaoCaoDongThung
        public DataTable GetChiTietDongThung(DataTable tblChiTiet)
        {
            DataTable result = nhapkhomodel.GetChiTietDongThung(tblChiTiet);
            return result;
        }
    }
}