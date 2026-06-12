using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
  public  class MaSealEntity
    {       
        public int ID { get; set; }
        public string MaSeal { get; set; }
        public string Seal { get; set; }
        public string Status{ get; set; }       
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public string NVTao { get; set; }
        public string LoaiSeal { get; set; }
        public string MaKho { get; set; }
        public bool IsEdit { get; set; }
        public bool IsChoose { get; set; }
        public string MaDVSX { get; set; }
        public bool IsUsed { get; set; }

    }
}
