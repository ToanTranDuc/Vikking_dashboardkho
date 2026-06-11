using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.Kho
{
    public class DongThungEntity
    {
        public int ID { get; set; }
        public string MaDH { get; set; }
        public string MaDVSX { get; set; }
        public string MaPKL { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public string DauSizeID { get; set; }
        public string DauSize { get; set; }
        public string MaMau { get; set; }
        public string TenMau { get; set; }
        public string MaSX { get; set; }
        public string TenSX { get; set; }
        public string MaLenh { get; set; }
        public string TenLenh { get; set; }
        public string DotSX { get; set; }
        public string MaHang { get; set; }
        public string MaLenhSanXuat { get; set; }
        public int Thung { get; set; }
        public int SLCL { get; set; }
    }
    public class DongThungThongTinEntity
    {
        public string MaDH { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string MaCL { get; set; }
        public string TenCL { get; set; }
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public string Dot { get; set; }
        public int SoLuong { get; set; }
        public int GhiChu { get; set; }

    }
    public class DongThungThongTinPOCTEntity
    {
        public string MaPKL { get; set; }
        public string TenPKL { get; set; }
        public string MaDH { get; set; }
        public string MaSX { get; set; }
        public string TenSX { get; set; }
        public string MaLenh { get; set; }
        public string TenLenh { get; set; }
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
    public class DongThungBaoCaoEntity
    {
        public string MaDH { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string MaKho { get; set; }
        public string TenKho { get; set; }
        public string MaKhoTu { get; set; }
        public string TenKhoTu { get; set; }
        public string MaKhoDen { get; set; }
        public string TenKhoDen { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public string DauSizeID { get; set; }
        public string DauSize { get; set; }
        public string MaDVSX { get; set; }
        public string TenDVSX { get; set; }
        public string MaDVSXDen { get; set; }
        public string TenDVSXDen { get; set; }
        public string MaMau { get; set; }
        public string TenMau { get; set; }
        public DateTime NgayDongThung { get; set; }
        public int TongSoThung { get; set; }
        public int SoThungDaDong { get; set; }
        public int SoThungChuaDong { get; set; }
        public string MaPKL { get; set; }
        public string MaLenh { get; set; }
        public int SoSPNhap { get; set; }
        public int SoThungNhap { get; set; }

        public int SoSPXuat { get; set; }
        public int SoThungXuat { get; set; }
        public int SoSPTon { get; set; }
        public int SoThungTon { get; set; }
        public string DotSX { get; set; }
    }
}
