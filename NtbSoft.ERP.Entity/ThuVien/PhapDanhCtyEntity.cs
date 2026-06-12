using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
  public class PhapDanhCtyEntity
    {
        public int ID { get; set; }
        public string MaCty { get; set; }
        public string TenCty { get; set; }
        public string TenCty_ENG { get; set; } = null;
        public string GhiChu { get; set; }
        public string ImageLoGo { get; set; }
    }
}
