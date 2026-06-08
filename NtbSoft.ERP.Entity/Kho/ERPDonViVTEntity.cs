using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NtbSoft.ERP.Entity.Kho
{
    public class ERPDonViVTEntity
    {
        public ERPDonViVTEntity()
        {

        }
        public ERPDonViVTEntity(string _tendvvt)
        {
            TenDVVT = _tendvvt;
        }
        public ERPDonViVTEntity(string _tendvvt, float TiLe)
        {
            TenDVVT = _tendvvt;
            this.Tile = TiLe;
        }
        public int ID { get; set; }
        public string MaDVVT { get; set; }
        public string TenDVVT { get; set; }
        public string GhiChu { get; set; }
        public float Tile { get; set; }
        public bool QDLamTronLe { get; set; }
        //Thoai them 4 tham số
        public string NguoiTao { get; set; }    
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }

    public class ERPKhoVatTuEntiy
    {
        public int ID { get; set; }
        public string MaVT { get; set; }
        public string TenVT { get; set; }
        public double SoLuong { get; set; }
        public string DonVi { get; set; }
        public bool IsUpdate { get; set; }
        public string MaCLVT { get; set; }
    }

    public class ERPPhieuNhapVTEntiy
    {
        public int ID { get; set; }
        public string MaPhieu { get; set; }
        public string MaVT { get; set; }
        public string TenVT { get; set; }
        public double SoLuong { get; set; }
        public double SoLuongTong { get; set; }
        public string NgayNhap { get; set; }
    }
    public class ERPPhieuXuatVTEntiy
    {
        public int ID { get; set; }
        public string MaPhieu { get; set; }
        public string MaVT { get; set; }
        public string TenVT { get; set; }
        public double SoLuong { get; set; }
        public double SoLuongTong { get; set; }
        public string NgayXuat { get; set; }
    }
    public class ERPChungLoaiVatTuEntiy
    {
        public int ID { get; set; }
        public string MaCLVT { get; set; }
        public string TenCLVT { get; set; }
    }

}
