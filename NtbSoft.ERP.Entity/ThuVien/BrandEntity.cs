using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class BrandEntity
    {
        public long ID { get; set; }
        public string MaKH { get; set; }
        public string MaBrand { get; set; }
        public string TenBrand { get; set; }
        public string GhiChu { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }

}
