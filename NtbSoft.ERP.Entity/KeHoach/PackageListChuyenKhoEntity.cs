using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.KeHoach
{
    public class PackageListChuyenKhoEntity
    {
        public int Id { get; set; }
        public string MaPKL_XH { get; set; }
        public string MaDH { get; set; }
        public string KhachHang { get; set; }
        public string MaHang { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public string TuKho { get; set; }
        public string DenKho { get; set; }
        public int Carton { get; set; }
        public int SoLuong { get; set; }
        public string VCCont { get; set; }
        public string VCXeTai { get; set; }
        public string Cont { get; set; }
        public string Romoc { get; set; }
        public string Seal { get; set; }
        public string Consignee { get; set; }
        public string InVoice { get; set; }
        public string TenTaiXe { get; set; }
        public string CMND { get; set; }
        public string CongTy { get; set; }
        public int Shipper { get; set; }
        public string Image { get; set; }
        public string Video { get; set; }
        public DateTime? PackDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? ImportDate { get; set; }
        public DateTime? ExportDate { get; set; }
        public int Status { get; set; }
        public int Dot { get; set; }
        public string NVienNhap { get; set; }
        public string NVienXuat { get; set; }

    }
    public class ImageNoiBoEntity
    {
        public string MaPKL { get; set; }
        public string NoiDung { get; set; }
        public string Image { get; set; }
        public int Position { get; set; }
        public string Cont { get; set; }
    }
    public class KiTenNoiBoEntity
    {
        public string MaPKL { get; set; }
        public string MaHoTen { get; set; }
        public string HoTen { get; set; }
        public string BoPhan { get; set; }
        public string ChuKy { get; set; }
        public string Cont { get; set; }
        public int Position { get; set; }
    }
}
