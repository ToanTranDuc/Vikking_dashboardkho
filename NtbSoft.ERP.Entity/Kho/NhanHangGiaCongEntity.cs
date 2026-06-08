using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.Kho
{
    public class NhanHangGiaoCongEntity
    {
        public string MaLenh { get; set; }
        public string MaDH { get; set; }
        public string MaDVSX { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public string MaMau { get; set; }
        public string TenMau { get; set; }
        public string DauSizeID { get; set; }
        public string DauSize { get; set; }
        public string SizeID { get; set; }
        public string Size { get; set; }       
        public int SoLuong { get; set; }
        public int LuyKe { get; set; }
        public int SoLuongKH { get; set; }
        public int Dot { get; set; }
        public  string NgayKiem { get; set; }
        public string NhanVien { get; set; }
        public int Status { get; set; }
        public int IsHoanThanh { get; set; }
        public int SoKien { get; set; }
    }
    public class ImageOtherEntity
    {
        public int MaBienBan { get; set; }
        public string NoiDung { get; set; }
        public string Image {  get; set; }
        public int Position { get; set; }
    } 
}
