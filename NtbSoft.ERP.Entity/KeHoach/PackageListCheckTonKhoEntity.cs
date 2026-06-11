using System;

namespace NtbSoft.ERP.Entity.KeHoach
{
    public class PackageListCheckTonKhoEntity
    {
        public int Id { get; set; }
        public string MaPKL_TK { get; set; }
        public string MaDH { get; set; }
        public string KhachHang { get; set; }
        public string MaHang { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public int Carton { get; set; }
        public int SoLuong { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ExportDate { get; set; }
        public int Status { get; set; }
        public int Dot { get; set; }
        public string NhanVien { get; set; }
    }
}
