using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class VatTuGiaDto
    {
        public int ID { get; set; }

        public string MaNhom { get; set; }
        public string MaVTID { get; set; }
        public string MaVT { get; set; }
        public string ChiTiet { get; set; }
        public string MaDVVT { get; set; }
        public string TenDVVT { get; set; }
        public string NguoiTao { get; set; }
        public string MaMauVT { get; set; }
        public string MauVT { get; set; }
        public string KhoVai { get; set; }

        public string MaCLVTID { get; set; }
        public string MauVTID { get; set; }
        public string KhoVaiID { get; set; }
        public DateTime? NgayApDung { get; set; }
        public DateTime? NgayKetThuc { get; set; }

        public decimal? DonGia { get; set; }
        public string DonViTienTe { get; set; }

        public string GhiChu { get; set; }
    }
}
