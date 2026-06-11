
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.Kho
{
    public class KhoEntity
    {
        public int ID { get; set; }
        public string MaKho { get; set; }
        public string TenKho { get; set; }
        public string Ghichu { get; set; }
        public int Status_VT { get; set; }
        public string ParentMaKho { get; set; }
        public decimal CBM { get; set; }
        public string TenDVSX { get; set; }
        public KhoEntity() { }
    }
}
