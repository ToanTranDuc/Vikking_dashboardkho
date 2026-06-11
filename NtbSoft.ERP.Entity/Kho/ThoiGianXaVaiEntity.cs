using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace NtbSoft.ERP.Entity.Kho
{
    public class ThoiGianXaVai
    {
        public string MaNhom { get; set; }
        public string MaDVVT { get; set; }
        public int ThoiGian { get; set; }

        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }

        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }
}
