using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.QuanLyDonHang
{
    public class CanDoiDonHangTongEntity
    {
        public string MaDH { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public int SLTong { get; set; }
    }
    //public class DonHangEntity
    //{
    //    public string MaDH { get; set; }
    //    public string MaHang { get; set; }
    //    public string TenHang { get; set; }
    //}
    //public class DauSizeEntity
    //{
    //    public string DauSizeID { get; set; }
    //    public string DauSize { get; set; }
    //}
    //public class MauEntity
    //{
    //    public string MaMau { get; set; }
    //    public string TenMau { get; set; }
    //}
    //public class SizeEntity
    //{
    //    public string SizeID { get; set; }
    //    public string Size { get; set; }
    //}
    public class CanDoiDonViSanXuatSaveEntity
    {
        public int ID { get; set; }
        public string MaLenhSanXuat { get; set; }
        public string MaLenh { get; set; }
        public string TenLenh { get; set; }
        public string MaDH { get; set; }
        public string MaDVSX { get; set; }
        public string DotSX { get; set; }
        public string MaQG { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public string MaMau { get; set; }
        public string DauSizeID { get; set; }
        public string DauSize { get; set; }
        public string SizeID { get; set; }
        public string Size { get; set; }
        public int SoLuong { get; set; }
        public int TrangThai { get; set; }
        public int STTLenh { get; set; }
        public string MaGop { get; set; }
        public string POID_T { get; set; }
        public string MaCu { get; set; }
        public string Line { get; set; }
        public string GhiChu { get; set; }
    }

    public class CanDoiDonViSanXuatDuyetSaveEntity
    {
        public int ID { get; set; }
        public string MaLenhSanXuat { get; set; }
        public string MaLenh { get; set; }
        public string TenLenh { get; set; }
        public string MaDH { get; set; }
        public string MaDVSX { get; set; }
        public decimal DotSX { get; set; }
        public string MaQG { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public string MaMau { get; set; }
        public string DauSizeID { get; set; }
        public string DauSize { get; set; }
        public string SizeID { get; set; }
        public string Size { get; set; }
        public decimal SoLuong { get; set; }
        public decimal TrangThai { get; set; }
        public decimal STTLenh { get; set; }
        public string MaGop { get; set; }
        public string POID_T { get; set; }
        public string MaCu { get; set; }
        public string Line { get; set; }
        public string GhiChu { get; set; }
    }
    public class CanDoiNPLEntity
    {
        public int ID { get; set; }
        public string MaDH { get; set; }
        public string MaLenh { get; set; }
        public string MaNPL { get; set; }
        public string MaVT { get; set; }
        public string TenVT { get; set; }
        public string MaMau { get; set; }
        public string TenMau { get; set; }
        public string KhoVai { get; set; }
        public string MaDV { get; set; }
        public string TenDV { get; set; }
        public double DinhMuc { get; set; }
        public decimal SoLuongCapPhat { get; set; }
        public int TrangThai { get; set; }
        public string NguoiSua { get; set; }
        public string GhiChu { get; set; }
    }
    public class CanDoiNPLSaveEntity
    {
        public int ID { get; set; }
        public string MaDH { get; set; }
        public string MaLenhSanXuat { get; set; }
        public string MaLenh { get; set; }
        public string MaNPL { get; set; }
        public string MaVT { get; set; }
        public string TenVT { get; set; }
        public string MaMau { get; set; }
        public string KhoVai { get; set; }
        public string MaDV { get; set; }
        public double DinhMuc { get; set; }
        public decimal SoLuongCapPhat { get; set; }
        public int TrangThai { get; set; }
        public string NguoiSua { get; set; }
        public string GhiChu { get; set; }

    }
}
