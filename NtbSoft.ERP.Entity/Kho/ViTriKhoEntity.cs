using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.Kho
{
    public class ViTriKhoEntity
    {
        public int ID { get; set; }
        public string MaKho { get; set; }
        public string MaVT { get; set; }
        public string TenVT { get; set; }
        public string ParentMaVT { get; set; }
        public double CBM { get; set; }
        public double Dai { get; set; }
        public double Rong { get; set; }
        public double Cao { get; set; }
        public bool NguyenLieu { get; set; }
        public bool PhuLieu { get; set; }
        public string GhiChu { get; set; }

    }

    public class ChiTietPhieuNhap_VTKEntity
    {
        public int ID { get; set; }
        public string MaVT { get; set; }
        public string MaKho { get; set; }
        public string MaPhieuNhap { get; set; }
        public string MaCTVT { get; set; }
        public double SoLuong { get; set; }
        public double KhoiLuong { get; set; }
        public double TrongLuong { get; set; }
        public DateTime NgayNhapKho { get; set; }
        public string UserName { get; set; }

    }
}
