using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.QuanLyDonHang
{
    public class DonHangTongSaveEntityV1
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
        public float FOB { get; set; }
        public float HeSoDH { get; set; }
        public string HinhThucXuatHang { get; set; }

    }

    public class DonHangTongEntityV1
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
        public float FOB { get; set; }
        public float HeSoDH { get; set; }
        public string HinhThucXuatHang { get; set; }
    }
}
