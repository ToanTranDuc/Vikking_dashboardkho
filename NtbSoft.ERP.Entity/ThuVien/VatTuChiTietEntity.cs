using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class VatTuChiTietEntity
    {
        public int ID { get; set; }       // Khóa chính, tự động tăng
        public string MaVT { get; set; } // Mã vật tư
        public string TenVT { get; set; } // Tên vật tư

        public string MaHienThi { get; set; }
    }
}
