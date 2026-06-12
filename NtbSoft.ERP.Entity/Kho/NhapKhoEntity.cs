using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.Kho
{
    public class NhapKhoDSDHEntity
    {
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string MaCL { get; set; }
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public string Dot { get; set; }
        public string SoLuong { get; set; }
        public string GhiChu { get; set; }
        public string SLThung { get; set; }
        public string SLTH { get; set; }
        public string SLCL { get; set; }
        public string MaGop { get; set; }
    }

    public class NhapKhoEntity
    {
        public int ID { get; set; }
        public string MaPKL { get; set; }
        public string MaDH { get; set; }
        public string DotSX { get; set; }
        public string MaHang { get; set; }
        public string POID { get; set; }
    }
    public class NhapKhoThongTinEntity
    {
        public string MaDH { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string MaCL { get; set; }
        public string TenCL { get; set; }
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string Dot { get; set; }
        public int SoLuong { get; set; }
        public int GhiChu { get; set; }
    }
    public class NhapKhoThongTinPOCTEntity
    {
        public string MaPKL { get; set; }
        public string MaDH { get; set; }
        public string MaDVSX { get; set; }
        public string TenDVSX { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public string DauSizeID { get; set; }
        public string DauSize { get; set; }
        public string MaMau { get; set; }
        public string TenMau { get; set; }
        public int Dot { get; set; }
        public int SttThung { get; set; }
        public string TrangThai { get; set; }
    }
}
