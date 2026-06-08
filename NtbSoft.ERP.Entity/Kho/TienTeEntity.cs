using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.Kho
{
    public class TienTeEntity
    {
        public int ID { get; set; }
        public string TienTeID { get; set; }
        public string MaTienTe { get; set; }
        public string TenTienTe { get; set; }
    }
    public class QuyDoiTienTeEntity
    {
        public int ID { get; set; }
        public string QuyDoiID { get; set; }
        public string MaTienTe { get; set; }
        public double Gia { get; set; }
        public DateTime Ngay { get; set; }
        public string UserID { get; set; }
        public string NguoiSua { get; set; }
        public string MaTT { get; set; }
    }
}
