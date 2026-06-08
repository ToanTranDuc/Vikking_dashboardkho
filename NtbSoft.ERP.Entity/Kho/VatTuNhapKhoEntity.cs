using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.Kho
{
    public class VatTuNhapKhoEntity
    {
        public int ID { get; set; }
        public string MaVTNhapKho { get; set; }
        public string MaDH { get; set; }
        public string Barcode { get; set; }
        public string MaVT { get; set; }
        public string TenVT { get; set; }
        public string MaVTMau { get; set; }
        public float? SoLuong { get; set; }
        public string MaMau { get; set; }
        public string TenMau { get; set; }
        public string CodeMau { get; set; }

        public string MaDVTinh { get; set; }
        public string DVTinh { get; set; }
        public string MaKhoSize { get; set; }
        public string KhoSize { get; set; }
        public string TongKien { get; set; }
        public string Kien { get; set; }
        public string MaVTSP { get; set; }
        public string MaViTri { get; set; }
        public string MaKho { get; set; }
        public string MaDTKH { get; set; }
        public string Lot { get; set; }
        public string AnhMau { get; set; }
        public string LoangMau { get; set; }
        public string GhiChu { get; set; }
        public string NgayNhap { get; set; }
        public string NguoiNhap { get; set; }
        public string DotNhap { get; set; }

      
    }
    public class VTKhoEntity
    {
        public string MaKho { get; set; }
        public string TenDVSX { get; set; }

    }
}
