using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.QuanLyDonHang
{
    public class DinhMucNPLEntity
    {
        public int ID { get; set; }

        public string MaDH { get; set; }
        public string MaNPL { get; set; }

        public string MaVT { get; set; }
        public string TenVT { get; set; }
        // Mã màu
        public string MaMau { get; set; }
        public string TenMau { get; set; }
        public string KhoVai { get; set; }
        // mã đơn vị
        public string MaDV { get; set; }
        public double DinhMuc { get; set; }
        public int SoLuong { get; set; }
        public double CapPhat { get; set; }
        public double CapThem { get; set; }
        public double ThuHoi { get; set; }
        public int TrangThai { get; set; }
        public string GhiChu { get; set; }
    }
}