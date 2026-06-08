using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.KeHoach
{
    public class PackageListXuatHangEntity
    {
        public int Id { get; set; }
        public string MaPKL_XH { get; set; }
        public string MaDH { get; set; }
        public string KhachHang { get; set; }
        public string MaHang { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public int Carton { get; set; }
        public int SoLuong { get; set; }
        public string VCCont { get; set; }
        public string VCXeTai { get; set; }
        public string Soxe { get; set; }
        public string Cont { get; set; }
        public string Romoc { get; set; }
        public string MaSeal { get; set; }
        public string Consignee { get; set; }
        public string InVoice { get; set; }
        public string TenTaiXe { get; set; }
        public string CMND { get; set; }
        public string CongTy { get; set; }
        public int Shipper { get; set; }
        public string Image { get; set; }
        public string Video { get; set; }
        public DateTime? PackDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ExportDate { get; set; }
        public int IsExport { get; set; }
        public int Dot { get; set; }
        public string NhanVien { get; set; }
        public string Image0 { get; set; }
        public string Image25 { get; set; }
        public string Image50 { get; set; }
        public string Image75 { get; set; }
        public string Image100 { get; set; }
        public string ImageDongCua { get; set; }
        public string ImageDoAm { get; set; }
        public string ImageTruocChot { get; set; }
        public string ImageChotAT { get; set; }
        public string NhanVienKiem { get; set; }
        public string GhiChu { get; set; }
        public string MaCont { get; set; }
        public string SoBooking { get; set; }
        public string MaBooking { get; set; }

    }
    public class BienBanXuatHang
    {
        public string MaPhieuBB { get; set; }
        public DateTime? NgayLap { get; set; }
        public int? VanChuyen { get; set; }
        public string LoaiFeets { get; set; }
        public string LoaiTan { get; set; }
        public string SoXe { get; set; }
        public string SoCont { get; set; }
        public string SoRoMoc { get; set; }
        public string TenTaiXe { get; set; }
        public string CMND { get; set; }
        public string CongTy { get; set; }
        public DateTime? NgayXeDen { get; set; }
        public DateTime? NgayGioCheckXeRoMoc { get; set; }
        public DateTime? NgayGioDongHang { get; set; }
        public DateTime? NgayGioKTDongHang { get; set; }
        public string MaSeal { get; set; }
        public DateTime? NgayGioXeDi { get; set; }
        public string Image0 { get; set; }
        public string Image25 { get; set; }
        public string Image50 { get; set; }
        public string Image75 { get; set; }
        public string Image100 { get; set; }
        public string ImageDongCua { get; set; }
        public string ImageDoAm { get; set; }
        public string ImageTruocChot { get; set; }
        public string ImageChotAT { get; set; }
        public string Ma_PKL { get; set; }
        public int XeRomoc { get; set; }
        public string MaCongTy {  get; set; }
        public string NhanVienKiem { get; set; }
        public string TenBienBan { get; set; }
        public string MaSoCont { get; set; }
        public string ChungTu { get; set; }
        public string DuKienTG { get; set; }
        public DateTime? TGBVCheckBD { get; set; }
        public DateTime? TGBVCheckKT { get; set; }
        public DateTime? TGNVCheckBD { get; set; }
        public DateTime? TGNVCheckKT { get; set; }

    }
    public class KyTenBienBan
    {
        public string MaBBXuatHang { get; set; }
        public string MaHoTen { get; set; }
        public string HoTen { get; set; }
        public string BoPhan { get; set; }
        public string KyTen { get; set; }

    }
    public class NoiDungKiemTra
    {
        public int MaBBPhieu { get; set; }
        public string MaNoiDung { get; set; }
        public string NoiDung { get; set; }
        public int StatusCont { get; set; }
        public int StatusRoMoc { get; set; }
        public int StatusXeTai { get; set; }
        public int StatusKL { get; set; }

    }
    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
       

    }
}
