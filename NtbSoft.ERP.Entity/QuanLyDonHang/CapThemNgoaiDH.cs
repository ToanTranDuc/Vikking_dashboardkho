using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.QuanLyDonHang
{
    public class CapThemNgoaiDH
    {
        public class CapThem_NgoaiDH
        {
            public string PhieuCT_NgoaiDH { get; set; }
            public int? Dot { get; set; }
            public string MaLenhSX { get; set; }
            public string MaDH { get; set; }
            public string MaLenh { get; set; }
            public string MaNPL { get; set; }
            public double? SLDK { get; set; }
            public string GhiChu { get; set; }
            public string MaNhom { get; set; }
            public string MaVTID { get; set; }
            public string MaMauVT { get; set; }
            public string MauVTID { get; set; }
            public string KhoVaiID { get; set; }
            public string MaVT { get; set; }
            public string MauVT { get; set; }
            public string KhoVai { get; set; }
            public string MaDVVT { get; set; }
            public DateTime? NgayDK { get; set; }
            public DateTime? NgayTH { get; set; }
            public DateTime? NgayTao { get; set; }
            public int? IsNPL { get; set; }
            public string NguoiTH { get; set; }
            public string PhieuDK { get; set; }
            public string SignNgDK { get; set; }
            public string NgayKi { get; set; }
            public string SignTBPNgDK { get; set; }
            public string NgayKiTBPNgDK { get; set; }
            public string SignMer { get; set; }
            public string NgaySignMer { get; set; }
            public string SignTBPMer { get; set; }
            public string NgaySignTBPMer { get; set; }
            public int? IsTV { get; set; }
            public string MaNhomChiTiet { get; set; }
            public bool? PhatSinhChiPhi { get; set; }
            public string LiDo { get; set; }
        }
    }
}
