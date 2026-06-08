using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NtbSoft.ERP.Entity.Kho
{
    public class ERPMaMauVTEntity
    {
        public int ID { get; set; }
        public string MauID { get; set; }
        public string MaMauVT { get; set; }
        public string MauVT { get; set; }
        public string MaKH { get; set; }
        public string MaHang { get; set; }

        public bool isNew { get; set; } = false;
    }
}
