using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
   public class MaContEntity
    {

        public int ID { get; set; }
        public string MaCont { get; set; }
        public string TenCont { get; set; }
        public DateTime? NgayTao { get; set; } = DateTime.Now;
        public string NVTao { get; set; }
  
    }
}
