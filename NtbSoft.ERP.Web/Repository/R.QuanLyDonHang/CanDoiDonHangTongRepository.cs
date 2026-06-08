using NtbSoft.ERP.Model.QuanLyDonHang;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Repository.R.QuanLyDonHang
{
    public interface ICanDoiDonHangTongRepository
    {
        DataTable GetDonHangTong(int pageIndex, int pageSize);
        DataTable GetDonHangTong_MaDH(string maDH);
        DataTable GetDonHangTongView(int pageIndex, int pageSize, string madvsx);
        DataTable GetDonHangTongViewLoc(int pageIndex, int pageSize, string madvsx, string tungay, string denngay, string mahang, string dot, string sl);
        DataTable Get();
        DataTable GetDauSize(string madh, string poid);
        DataTable GetPO(string madh);
        DataTable GetMau(string madh, string dausizeID);
        DataTable GetSizeID(string madh, string dausizeID, string mamau);
        DataTable GetListSize(string madh);
        DataTable GetSoLuongDH(string madh, string dausizeID, string mamau, string poid);
        string GetMaxDH();
        DataTable GetNPL(string madh, string malenhsanxuat);
        DataTable GetGomNPL(string parameter);
        string PostDVSX(object ojDVSX);
        string PostDVSXV2(object ojDVSX);
        string PostIsKeoVe(object ojDVSX);
        string PostUDDVSX(object ojDVSX);
        string PostNPL(object ojNPL);
        DataTable GetLenhSX(string madh, string poid);
        DataTable GetChiTietCanDoiNPL(string madh, string malenh); 
        DataTable GetChiTietCanDoiDVSX(string malenhsanxuat);//(string madh, string malenh, string malenhsanxuat, string poid);
        DataTable GetChiTietLenhSX(string madh, string madvsx);
        DataTable GetCTTotal(string madh);
        DataTable GetCTTotalDetail(string madh);
        string PostUpdateDVSX(object ojDVSX);
        string PostUpdateNPL(object ojNPL);
        string Delete(string madh, string malenh, string malenhsanxuat, string username);
        string DeleteDM(string madh, string malenh, string malenhsanxuat, string manpl, string username);
        string DeleteNPL(string madh, string malenhsanxuat, string username);
        DataTable GetDSHangHoa();
        DataTable GetDSThongTinLenhSX(int pageIndex, int pageSize);
        DataTable GetDSChiTietLenhSX(string id, int pageIndex, int pageSize);
        DataTable GetKTCanDoiNPL(string madh, string malenh, string poid, string mamau, string dausizeid);
        DataTable GetDsCanDoiDonHang(string maDH, string poID, string dauSizeID, string maMau);
        DataTable GetDsBaoCao(string maLenhSX);
        DataTable GetKiemTraDinhMuc(string madh, string malenhsanxuat);
        string PostDinhMuc(DataTable ojDinhMuc);
        string PostDinhMucImport(DataTable ojDinhMuc);
        string PostGopDH(DataTable ojGopDH);
        DataTable GetKiemTra(string madh);
        DataTable GetGopID(string madh);
        string DeleteItemCanDoi(DataTable _tblDeleteItemCanDoi);
        string PostDinhMucNhap(DataTable ojDinhMuc);
        DataTable GetCapPhat(string madh, string _madh, string _malenhsx, string _magop);
        DataTable GetVatTuNPL(string madh, string malenhsx);
        string DeleteNPLNhap(int id);
        DataTable GetSoLuongNhap(string para, string para1);
        DataTable GetSoBookingNhap(string para);
        DataTable GetCDNPL(string madh, string malenh);
        DataTable GetTenVTGoiY();
        DataTable GetMauGoiY();
        DataTable GetKhoSizeGoiY();
        DataTable GetDVTinhGoiY();
        DataTable GetNhomGoiY();
        DataTable GetSoLuongByMauDH(string para, string para1,string para2);
        string PostDot(DataTable ojDinhMuc);
    }
    public class CanDoiDonHangTongRepository : ICanDoiDonHangTongRepository
    {
        CanDoiDonHangModel candoidhmodel = new CanDoiDonHangModel();
        public DataTable GetDonHangTong(int pageIndex,int pageSize)
        {
            var dt = candoidhmodel.GetDonHangTong(pageIndex,pageSize);
            return dt;
        }
        public DataTable GetDonHangTong_MaDH(string maDH)
        {
            var dt = candoidhmodel.GetDonHangTong_MaDH(maDH);
            return dt;
        }
        public DataTable GetDonHangTongView(int pageIndex, int pageSize, string madvsx)
        {
            var dt = candoidhmodel.GetDonHangTongView(pageIndex, pageSize, madvsx);
            return dt;
        }
        public DataTable GetDonHangTongViewLoc(int pageIndex, int pageSize, string madvsx, string tungay, string denngay, string mahang, string dot, string sl)
        {
            var dt = candoidhmodel.GetDonHangTongViewLoc(pageIndex, pageSize, madvsx, tungay, denngay, mahang, dot, sl);
            return dt;
        }
        public DataTable Get()
        {
            var dt = candoidhmodel.Get();
            return dt;
        }
        public DataTable GetPO(string madh)
        {
            var dt = candoidhmodel.GetPO(madh);
            return dt;
        }
        public DataTable GetDauSize(string madh, string poid)
        {
            var dt = candoidhmodel.GetDauSize(madh, poid);
            return dt;
        }
        public DataTable GetMau(string madh, string dausizeID)
        {
            var dt = candoidhmodel.GetMau(madh, dausizeID);
            return dt;
        }
        public DataTable GetSizeID(string madh, string dausizeID, string mamau)
        {
            var dt = candoidhmodel.GetSizeID(madh, dausizeID, mamau);
            return dt;
        }
        public DataTable GetListSize(string madh)
        {
            var dt = candoidhmodel.GetListSize(madh);
            return dt;
        }
        public DataTable GetSoLuongDH(string madh, string dausizeID, string mamau, string poid)
        {
            var dt = candoidhmodel.GetSoLuongDH(madh, dausizeID, mamau, poid);
            return dt;
        }
        public string GetMaxDH()
        {
            return candoidhmodel.GetMax();
        }
        public DataTable GetNPL(string madh, string malenhsanxuat)
        {
            var dt = candoidhmodel.GetNPL(madh, malenhsanxuat);
            return dt;
        }
        public DataTable GetGomNPL(string parameter)
        {
            var dt = candoidhmodel.GetGomNPL(parameter);
            return dt;
        } 
        public string PostDVSX(object ojDVSX)
        {
            var result = candoidhmodel.PostDVSX(ojDVSX);
            return result;
        }
        public string PostDVSXV2(object ojDVSX)
        {
            var result = candoidhmodel.PostDVSXV2(ojDVSX);
            return result;
        }
        public string PostIsKeoVe(object ojDVSX)
        {
            var result = candoidhmodel.PostIsKeoVe(ojDVSX);
            return result;
        }
        public string PostUDDVSX(object ojDVSX)
        {
            var result = candoidhmodel.PostUDDVSX(ojDVSX);
            return result;
        }
        public string PostNPL(object ojNPL)
        {
            var result = candoidhmodel.PostNPL(ojNPL);
            return result;
        }
        public DataTable GetLenhSX(string madh, string poid)
        {
            var dt = candoidhmodel.GetLenhSX(madh, poid);
            return dt;
        }
        public DataTable GetChiTietCanDoiNPL(string madh, string malenh)
        {
            var dt = candoidhmodel.GetChiTietCanDoiNPL(madh, malenh);
            return dt;
        }
        public DataTable GetChiTietCanDoiDVSX(string malenhsanxuat)//(string madh, string malenh, string malenhsanxuat, string poid)
        {
            var dt = candoidhmodel.GetChiTietCanDoiDVSX(malenhsanxuat);
            return dt;
        }
        public DataTable GetChiTietLenhSX(string madh, string madvsx)
        {
            var dt = candoidhmodel.GetChiTietLenhSX(madh, madvsx);
            return dt;
        }
        public DataTable GetCTTotal(string madh)
        {
            var dt = candoidhmodel.GetCTTotal(madh);
            return dt;
        }
        public DataTable GetCTTotalDetail(string madh)
        {
            var dt = candoidhmodel.GetCTTotalDetail(madh);
            return dt;
        }
        public string PostUpdateDVSX(object ojDVSX)
        {
            var result = candoidhmodel.PostUpdateDVSX(ojDVSX);
            return result;
        }
        public string PostUpdateNPL(object ojNPL)
        {
            var result = candoidhmodel.PostUpdateNPL(ojNPL);
            return result;
        }
        public string Delete(string madh, string malenh, string malenhsanxuat, string username)
        {
            var result = candoidhmodel.Delete(madh, malenh, malenhsanxuat, username);
            return result;
        }
        public string DeleteDM(string madh, string malenh, string malenhsanxuat, string manpl, string username)
        {
            var result = candoidhmodel.DeleteDM(madh, malenh, malenhsanxuat, manpl, username);
            return result;
        }
        public string DeleteNPL(string madh, string malenhsanxuat, string username)
        {
            var result = candoidhmodel.DeleteNPL(madh, malenhsanxuat, username);
            return result;
        }
        
        public DataTable GetDSThongTinLenhSX(int pageIndex, int pageSize)
        {
            var dt = candoidhmodel.GetDSThongTinLenhSX(pageIndex, pageSize);
            return dt;
        }
        public DataTable GetDSHangHoa()
        {
            var dt = candoidhmodel.GetDSHangHoa();
            return dt;
        }
        public DataTable GetDSChiTietLenhSX(string id, int pageIndex, int pageSize)
        {
            var dt = candoidhmodel.GetDSChiTietLenhSX(id, pageIndex, pageSize);
            return dt;
        }
        public DataTable GetKTCanDoiNPL(string madh, string malenh, string poid, string mamau, string dausizeid)
        {
            var dt = candoidhmodel.GetKTCanDoiNPL(madh, malenh, poid, mamau, dausizeid);
            return dt;
        }
        public DataTable GetDsCanDoiDonHang(string maDH, string poID, string dauSizeID, string maMau)
        {
            var dt = candoidhmodel.GetDsCanDoiDonHang(maDH, poID, dauSizeID, maMau);
            return dt;
        }
        public DataTable GetDsBaoCao(string maLenhSX)
        {
            var dt = candoidhmodel.GetDsBaoCao(maLenhSX);
            return dt;
        }
        public DataTable GetKiemTraDinhMuc(string madh, string malenhsanxuat)
        {
            var dt = candoidhmodel.GetKiemTraDinhMuc(madh, malenhsanxuat);
            return dt;
        }
        public string PostDinhMuc(DataTable ojDinhMuc)
        {
            var result = candoidhmodel.PostDinhMuc(ojDinhMuc);
            return result;
        }
        public string PostDinhMucImport(DataTable ojDinhMuc)
        {
            var result = candoidhmodel.PostDinhMucImport(ojDinhMuc);
            return result;
        }
        public string PostGopDH(DataTable ojGopDH)
        {
            var result = candoidhmodel.PostGopDH(ojGopDH);
            return result;
        }
        public DataTable GetKiemTra(string madh)
        {
            var dt = candoidhmodel.GetKiemTra(madh);
            return dt;
        }
        public DataTable GetGopID(string madh)
        {
            var dt = candoidhmodel.GetGopID(madh);
            return dt;
        }
        public string DeleteItemCanDoi(DataTable _tblDeleteItemCanDoi)
        {
            var result = candoidhmodel.DeleteItemCanDoi(_tblDeleteItemCanDoi);
            return result;
        }
        public string PostDinhMucNhap(DataTable ojDinhMuc)
        {
            var result = candoidhmodel.PostDinhMucNhap(ojDinhMuc);
            return result;
        }
        public DataTable GetCapPhat(string madh, string _madh, string _malenhsx, string _magop)
        {
            var dt = candoidhmodel.GetCapPhat(madh,_madh,_malenhsx,_magop);
            return dt;
        }
        public DataTable GetVatTuNPL(string madh,string malenhsx)
        {
            var dt = candoidhmodel.GetVatTuNPL(madh, malenhsx);
            return dt;
        }
        public string DeleteNPLNhap(int id)
        {
            var dt = candoidhmodel.DeleteNPLNhap(id);
            return dt;
        }
        public DataTable GetSoLuongNhap(string para, string para1)
        {

            var dt = candoidhmodel.GetSoLuongNhap(para, para1);
            return dt;
        }
        public DataTable GetSoBookingNhap(string para)
        {
            var dt = candoidhmodel.GetSoBookingNhap(para);
            return dt;
        }
        public DataTable GetCDNPL(string madh, string malenh)
        {
            var dt = candoidhmodel.GetCDNPL(madh, malenh);
            return dt;
        }
        public DataTable GetTenVTGoiY()
        {
            var dt = candoidhmodel.GetTenVTGoiY();
            return dt;
        }
        public DataTable GetMauGoiY()
        {
            var dt = candoidhmodel.GetMauGoiY();
            return dt;
        }
        public DataTable GetKhoSizeGoiY()
        {
            var dt = candoidhmodel.GetKhoSizeGoiY();
            return dt;
        }
        public DataTable GetDVTinhGoiY()
        {
            var dt = candoidhmodel.GetDVTinhGoiY();
            return dt;
        }
        public DataTable GetNhomGoiY()
        {
            var dt = candoidhmodel.GetNhomGoiY();
            return dt;
        }
        public DataTable GetSoLuongByMauDH(string para, string para1, string para2)
        {
            var dt = candoidhmodel.GetSoLuongByMauDH(para, para1, para2);
            return dt;
        }
        public string PostDot(DataTable ojDinhMuc)
        {
            var result = candoidhmodel.PostDot(ojDinhMuc);
            return result;
        }
    }
}