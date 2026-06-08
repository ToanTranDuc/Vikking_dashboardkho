using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
     public class KHDT_StoreEntity
    {
        public int ID { get; set; }
        public string StyleID { get; set; }
        public string MaHang { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public string Store { get; set; }
        public string SizeID { get; set; }
        public string Size { get; set; }
        public string ColorID { get; set; }
        public string MaMau { get; set; }
        public int SLuong { get; set; }
        public int SLThung { get; set; }     
        public string QuiCach { get; set; }
        public int SapXep { get; set; }
    }
}
