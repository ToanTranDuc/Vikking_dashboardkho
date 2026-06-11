using NtbSoft.ERP.Model.Kho;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.Kho
{
    public interface INhapKhoRepository
    {
        DataTable GetDS();
        DataTable GetLenhSX(string statuschuyen, string username,string status);
        DataTable GetLenhSXLuuChuyen(string username);
        DataTable GetDS_KHDT(string maplk, string mahang, string poid, string statuschuyen, string madvsx, string isDeCat, string dotsx);
        DataTable GetDS_KHDTLuanChuyen(string maplk, string mahang, string poid, string mapklchuyen, string madvsx, string isDecat, string dotsx);
        DataTable GetDSThung(string maplk, string poid, string mahang, string tuthung, string denthung, string madvsx, string isDeCat, string dotsx);
        DataTable GetDSThungLuanChuyen(string maplk, string poid, string mahang, string tuthung, string denthung, string madvsx, string isDeCat, string dotsx);
        DataTable Search(string malenh, string para);
        DataTable GetKho(string madvsx);
        DataTable GetThongTin(int lcsx, string pageIndex, string pageSize, string madvsx);
        DataTable GetThongTinSMS(int lcsx, string pageIndex, string pageSize, string madvsx);
        DataTable GetThongTinPO(string madh, int lcsx, string madvsx);
        DataTable GetThongTinPOSMS(string madh, int lcsx, string madvsx);
        DataTable GetMaKho(string madvsx, int lcsx);
        DataTable GetThongTinPOCT(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat, int lcsx);
        DataTable GetThongTinPOCTSMS(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat, int lcsx);

        DataTable GetThongTinChiTiet(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int lcsx, int minsttthung, int maxsttthung);
        DataTable GetThongTinChiTietSMS(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int lcsx, int minsttthung, int maxsttthung);

        string Post(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen);
        string PostALL(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, string mahang, string po);
        string PostDT(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen);
        string PostHuy(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen);
        string PostChiTiet(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, int _isTonKho);
        string PostNhapKho(DataTable tblDongThung);
        string PostNhapKhoLuuChuyen(string isTonKho, DataTable tblDongThung);
        string PostViTri(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen);
        string PostViTriSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen);
        string PostSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen);
        string PostALLSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, string mahang, string po);
        string PostDTSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen);
        string PostHuySMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen);
        string PostChiTietSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, int _isTonKho);
    }
    public class NhapKhoRepository : INhapKhoRepository
    {
        NhapKhoModel nhapkhomodel = new NhapKhoModel();
        public DataTable GetDS()
        {
            var dt = nhapkhomodel.GetDS();
            return dt;
        }
        public DataTable GetThongTin(int lcsx, string pageIndex, string pageSize, string madvsx)
        {
            var dt = nhapkhomodel.GetThongTin(lcsx, pageIndex, pageSize, madvsx);
            return dt;
        }
        public DataTable GetThongTinSMS(int lcsx, string pageIndex, string pageSize, string madvsx)
        {
            var dt = nhapkhomodel.GetThongTinSMS(lcsx, pageIndex, pageSize, madvsx);
            return dt;
        }
        public DataTable GetThongTinPO(string madh, int lcsx, string madvsx)
        {
            var dt = nhapkhomodel.GetThongTinPO(madh, lcsx, madvsx);
            return dt;
        }
        public DataTable GetThongTinPOSMS(string madh, int lcsx, string madvsx)
        {
            var dt = nhapkhomodel.GetThongTinPOSMS(madh, lcsx, madvsx);
            return dt;
        }
        public DataTable GetMaKho(string madvsx, int lcsx)
        {
            var dt = nhapkhomodel.GetMaKho(madvsx, lcsx);
            return dt;
        }
        public DataTable GetThongTinPOCT(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat, int lcsx)
        {
            var dt = nhapkhomodel.GetThongTinPOCT(mapkl, madvsx, malenh, poid, madh, malenhsanxuat, lcsx);
            return dt;
        }
        public DataTable GetThongTinPOCTSMS(string mapkl, string madvsx, string malenh, string poid, string madh, string malenhsanxuat, int lcsx)
        {
            var dt = nhapkhomodel.GetThongTinPOCTSMS(mapkl, madvsx, malenh, poid, madh, malenhsanxuat, lcsx);
            return dt;
        }
        public DataTable GetThongTinChiTiet(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int lcsx, int minsttthung, int maxsttthung)
        {
            var dt = nhapkhomodel.GetThongTinChiTiet(mapkl, malenh, madvsx, poid, dausizeid, mau, tuthung, denthung, lcsx, minsttthung, maxsttthung);
            return dt;
        }
        public DataTable GetThongTinChiTietSMS(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int lcsx, int minsttthung, int maxsttthung)
        {
            var dt = nhapkhomodel.GetThongTinChiTietSMS(mapkl, malenh, madvsx, poid, dausizeid, mau, tuthung, denthung, lcsx, minsttthung, maxsttthung);
            return dt;
        }
        public string Post(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            string result = nhapkhomodel.Post(tblNhapKho, isdongthung, lcsx, statuschuyen);
            return result;
        }
        public string PostSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            string result = nhapkhomodel.PostSMS(tblNhapKho, isdongthung, lcsx, statuschuyen);
            return result;
        }
        public string PostALL(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, string mahang, string po)
        {
            string result = nhapkhomodel.PostALL(tblNhapKho, isdongthung, lcsx, statuschuyen, mahang, po);
            return result;
        }
        public string PostALLSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, string mahang, string po)
        {
            string result = nhapkhomodel.PostALLSMS(tblNhapKho, isdongthung, lcsx, statuschuyen, mahang, po);
            return result;
        }
        public string PostDT(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            string result = nhapkhomodel.PostDT(tblNhapKho, isdongthung, lcsx, statuschuyen);
            return result;
        }
        public string PostDTSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            string result = nhapkhomodel.PostDTSMS(tblNhapKho, isdongthung, lcsx, statuschuyen);
            return result;
        }
        public string PostHuy(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            string result = nhapkhomodel.PostHuy(tblNhapKho, isdongthung, lcsx, statuschuyen);
            return result;
        }
        public string PostHuySMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            string result = nhapkhomodel.PostHuySMS(tblNhapKho, isdongthung, lcsx, statuschuyen);
            return result;
        }
        public string PostChiTiet(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, int _isTonKho)
        {
            string result = nhapkhomodel.PostChiTiet(tblNhapKho, isdongthung, lcsx, statuschuyen, _isTonKho);
            return result;
        }
        public string PostChiTietSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen, int _isTonKho)
        {
            string result = nhapkhomodel.PostChiTietSMS(tblNhapKho, isdongthung, lcsx, statuschuyen, _isTonKho);
            return result;
        }
        public string PostViTri(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            string result = nhapkhomodel.PostViTri(tblNhapKho, isdongthung, lcsx, statuschuyen);
            return result;
        }
        public string PostViTriSMS(DataTable tblNhapKho, bool isdongthung, int lcsx, int statuschuyen)
        {
            string result = nhapkhomodel.PostViTriSMS(tblNhapKho, isdongthung, lcsx, statuschuyen);
            return result;
        }
        //coding by Dat
        //start
        public DataTable GetLenhSX(string statuschuyen, string username, string status)
        {
            var dt = nhapkhomodel.GetLenhSX(statuschuyen, username, status);
            return dt;
        }

        public DataTable GetLenhSXLuuChuyen(string username)
        {
            var dt = nhapkhomodel.GetLenhSXLuuChuyen(username);
            return dt;
        }

        public DataTable GetDS_KHDT(string maplk, string mahang, string poid, string statuschuyen, string madvsx, string isDeCat, string dotsx)
        {
            var dt = nhapkhomodel.GetDS_KHDT(maplk, mahang, poid, statuschuyen, madvsx, isDeCat, dotsx);
            return dt;
        }

        public DataTable GetDS_KHDTLuanChuyen(string maplk, string mahang, string poid, string mapklchuyen, string madvsx, string isDeCat, string dotsx)
        {
            var dt = nhapkhomodel.GetDS_KHDTLuanChuyen(maplk, mahang, poid, mapklchuyen, madvsx, isDeCat, dotsx);
            return dt;
        }

        public DataTable GetDSThung(string maplk, string poid, string mahang, string tuthung, string denthung, string madvsx, string isDeCat, string dotsx)
        {
            var dt = nhapkhomodel.GetDSThung(maplk, poid, mahang, tuthung, denthung, madvsx, isDeCat, dotsx);
            return dt;
        }
        public DataTable GetDSThungLuanChuyen(string maplk, string poid, string mahang, string tuthung, string denthung, string madvsx, string isDeCat, string dotsx)
        {
            var dt = nhapkhomodel.GetDSThungLuanChuyen(maplk, poid, mahang, tuthung, denthung, madvsx, isDeCat, dotsx);
            return dt;
        }

        public string PostNhapKho(DataTable tblDongThung)
        {
            string result = nhapkhomodel.PostNhapKho(tblDongThung);
            return result;
        }
        public string PostNhapKhoLuuChuyen(string isTonKho, DataTable tblDongThung)
        {
            string result = nhapkhomodel.PostNhapKhoLuuChuyen(isTonKho, tblDongThung);
            return result;
        }
        public DataTable Search(string malenh, string para)
        {
            var dt = nhapkhomodel.Search(malenh, para);
            return dt;
        }
        public DataTable GetKho(string madvsx)
        {
            var dt = nhapkhomodel.GetKho(madvsx);
            return dt;
        }
        //end
    }
}