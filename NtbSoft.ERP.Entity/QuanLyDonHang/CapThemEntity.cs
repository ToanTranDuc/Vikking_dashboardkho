using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.QuanLyDonHang
{
    class CapThemEntity
    {
        public int ID { get; set; }
        public string MaDH { get; set; }
        public string MaLenhSanXuat { get; set; }
        public string MaNPL { get; set; }
        public string MaVT { get; set; }
        public string TenVT { get; set; }
        public string MaMau { get; set; }
        public string KhoVai { get; set; }
        public string MaDV { get; set; }
        public float DinhMuc { get; set; }
        public int SoLuong { get; set; }
        public float CapPhat { get; set; }
        public float CapThem { get; set; }
        public float ThuHoi { get; set; }
        public int TrangThai { get; set; }
        public string GhiChu { get; set; }
        public string MaMauLenh { get; set; }
        public string DauSizeLenh { get; set; }
        public string SizeLenh { get; set; }
        public string MaBom { get; set; }
        public string NguoiCap { get; set; }
        public DateTime NgayCap { get; set; }
        public int Dot { get; set; }

    }
}
