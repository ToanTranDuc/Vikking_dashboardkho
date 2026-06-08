using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class ErpKHDongThung_PCBEntity
    {
        public int ID { get; set; }
        public string MaLenh { get; set; }
        public string StyleID { get; set; }
        public string MaHang { get; set; }
        public string MakH { get; set; }
        public string SPOID { get; set; }
        public string PO { get; set; }
        public string SizeID { get; set; }
        public string Size { get; set; }
        public int SL_PCS_Thung { get; set; }
        public string GhiChu { get; set; }
        public string ColorID { get; set; }
        public string Tenmau { get; set; }
        public string Ma_QC_DongThung { get; set; }
    }
}
