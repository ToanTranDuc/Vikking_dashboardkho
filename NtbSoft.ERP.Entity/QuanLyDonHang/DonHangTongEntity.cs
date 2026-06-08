using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.QuanLyDonHang
{
    public class DonHangTongSaveEntity
    {
        public int ID { get; set; }
        public string MaDH { get; set; }
        public string MaKH { get; set; }
        public string MaHang { get; set; }
        public string MaCL { get; set; }
        public string MaHS { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public int SoLuong { get; set; }
        public decimal VND { get; set; }
        public decimal CM { get; set; }
        public string Dot { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; }
        public string NguoiSua { get; set; }
        public DateTime NgaySua { get; set; }
        public string SoVoice { get; set; }
        public int STT { get; set; }
        public string MaCty { get; set; }
        public string BookingMaHang { get; set; }

        public decimal? FOB { get; set; }
        public decimal? HeSoDH { get; set; }
        public string HinhThucXuatHang { get; set; }

    }
    public enum TypeBaoCao { CanDoi, PackageList, DongThung, NhapKho, NhapKhoLuanChuyen, XuatHang }
    public class BaoCaoEntity
    {
        public BaoCaoEntity()
        {

        }
        public BaoCaoEntity(TypeBaoCao _type, string _maDh, double _tiLe)
        {
            this.Type = _type;
            this.MaDH = _maDh;
            this.TiLe = _tiLe;
        }
        public TypeBaoCao Type;
        public string MaDH { get; set; }
        public double TiLe { get; set; }
    }
    public class DonHangTongEntity
    {
        public int ID { get; set; }
        public string MaDH { get; set; }
        public string MaKH { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string MaCL { get; set; }
        public string MaHS { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public int SoLuong { get; set; }
        public decimal VND { get; set; }
        public decimal CM { get; set; }
        public string Dot { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; }
        public string NguoiSua { get; set; }
        public DateTime NgaySua { get; set; }
        public string SoVoice { get; set; }
        public int STT { get; set; }
        public string MaCty { get; set; }
        public string BookingMaHang { get; set; }

        public decimal? FOB { get; set; }
        public decimal? HeSoDH { get; set; }
        public string HinhThucXuatHang { get; set; }
    }

    public class SoDonHangTongEntity
    {
        public int SoDonHang { get; set; }
    }

}
