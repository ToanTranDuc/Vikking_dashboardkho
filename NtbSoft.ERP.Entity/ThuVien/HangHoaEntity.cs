using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class HangHoaEntity
    {
        public HangHoaEntity(string _tenHang, string _maKH, string _loaiHH, string _nhomHang, string _ghiChu, string _maHang)
        {
            TenHang = _tenHang;
            MaKH = _maKH;
            MaLHH = _loaiHH;
            NhomHang = _nhomHang;
            GhiChu = _ghiChu;
            MaHang = _maHang;
        }
        public HangHoaEntity()
        {}
        public int ID { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string NhomHang { get; set; }
        public string GhiChu { get; set; }  
        public string MaLHH { get; set; }
        public string TenLHH { get; set; }
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string MaCL { get; set; }
        public string TenCL { get; set; }
        public int CheckMaHang { get; set; }
        public int TrangThai { get; set; }
        public bool InTheu { get; set; }
        public bool InTheuCT { get; set; }

        public bool HutAm { get; set; }
        public bool DoKim { get; set; }
    }
}
