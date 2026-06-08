using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.QuanLyDonHang
{
    public class DinhMucEntity
    {
        public int ID { get; set; }
        public string MaDH { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string Dot { get; set; }
        public string MaNPL { get; set; }
        public string MaVT { get; set; }
        public string TenVT { get; set; }
        public string MaMau { get; set; }
        public string KhoVai { get; set; }
        public string MaDV { get; set; }
        public double DinhMuc { get; set; }
        public int SoLuong { get; set; }
        public double CapPhat { get; set; }
        public double CapThem { get; set; }
        public double ThuHoi { get; set; }
        public int TrangThai { get; set; }
        public string GhiChu { get; set; }
    }
    public class DinhMucSaveEntity
    {
        public int ID { get; set; }
        public string MaDH { get; set; }
        public string MaLenhSanXuat { get; set; }
        public string MaNPL { get; set; }
        public string MaVT { get; set; }
        public string TenVT { get; set; }
        public string MaMau { get; set; }
        public string KhoVai { get; set; }
        public string MaDV { get; set; }
        public double DinhMuc { get; set; }
        public int SoLuong { get; set; }
        public double CapPhat { get; set; }
        public double CapThem { get; set; }
        public double ThuHoi { get; set; }
        public int TrangThai { get; set; }
        public string GhiChu { get; set; }
        public string NguoiTao { get; set; }
        public string NguoiSua { get; set; }
        public string MaMauLenh { get; set; }
        public string DauSizeLenh { get; set; }
        public string SizeLenh { get; set; }
        public string MaBom { get; set; }
        public bool IsXetDuyet { get; set; }
        public string NguoiXet { get; set; }
        public string NguoiHuy { get; set; }
        public double QtyES { get; set; }
        public double CostES { get; set; }
        public double NeedES { get; set; }
        public double ToTalRec { get; set; }

    }

    public class DinhMucSaveImportEntity
    {
        public int ID { get; set; }
        public string MaDH { get; set; }
        public string MaLenhSanXuat { get; set; }
        public string MaNPL { get; set; }
        public string MaVT { get; set; }
        public string TenVT { get; set; }
        public string MaMau { get; set; }
        public string KhoVai { get; set; }
        public string MaDV { get; set; }
        public double DinhMuc { get; set; }
        public int SoLuong { get; set; }
        public double CapPhat { get; set; }
        public double CapThem { get; set; }
        public double ThuHoi { get; set; }
        public int TrangThai { get; set; }
        public string GhiChu { get; set; }
        public string NguoiTao { get; set; }
        public string NguoiSua { get; set; }
        public string MaMauLenh { get; set; }
        public string DauSizeLenh { get; set; }
        public string SizeLenh { get; set; }
        public string MaBom { get; set; }
        public string MaVTMau { get; set; }

        public double QtyES { get; set; }
        public double CostES { get; set; }
        public double NeedES { get; set; }
        public double ToTalRec { get; set; }
        public int? IsNL { get; set; }
        public string MaKH { get; set; }
        public double ThucNhan { get; set; }
        public string TenTAVT { get; set; }
        public string NhomNPL { get; set; }

        public double? DinhMucHaoHut { get; set; }
        public string TenLoaiVT { get; set; }
        public string MauSP { get; set; }
    }
}
