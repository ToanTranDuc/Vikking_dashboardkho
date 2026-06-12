using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class KhachHangEntity
    {
        public int ID { get; set; }
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string GhiChu { get; set; }
        public string MaDT { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string MaSoThue { get; set; }

        public string NguoiDaiDien { get; set; }

        public int MaLoaiDT { get; set; }
        public string VietTat { get; set; }

        public string MaQG { get; set; }
        public string MaHoa { get; set; }
        #region Thoai
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
        #endregion
    }
}
