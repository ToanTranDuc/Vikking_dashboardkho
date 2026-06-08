using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class DicQCDongThungEntity
    {
        public string MaQuiCach { get; set; }
        public string TenQuiCach { get; set; }
        public double ChieuDai { get; set; }
        public double ChieuRong { get; set; }
        public double ChieuCao { get; set; }
        public double CanNang { get; set; }
        public string MaDV { get; set; }
        public int? SoLop { get; set; }
        public string LoaiThung { get; set; }
        public string GhiChu { get; set; }
        public string GhiChu2 { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }
}
