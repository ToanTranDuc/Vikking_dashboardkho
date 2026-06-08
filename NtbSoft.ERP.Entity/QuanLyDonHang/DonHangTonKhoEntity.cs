using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.QuanLyDonHang
{
    public class DonHangTonKhoEntity
    {
        public int ID { get; set; }
        public string MaHang { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public string MaMau { get; set; }
        public string DauSizeID { get; set; }
        public string DauSize { get; set; }
        public string SizeID { get; set; }
        public string Size { get; set; }
        public int SoLuong { get; set; }
        public int TrangThai { get; set; }
        public string GhiChu { get; set; }
    }
    public class DonHangTonKhoDSEntity
    {
        public int ID { get; set; }
        public string MaDH { get; set; }
        public string MaHang { get; set; }
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string MaCL { get; set; }
        public string TenCL { get; set; }
        public string MaDVSX { get; set; }
        public string TenDVSX { get; set; }
        public string MaHS { get; set; }
        public string SoVoice { get; set; }
        public int TrangThai { get; set; }
    }
}
