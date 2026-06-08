using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.Kho
{
    public class KhoNPLEntity
    {
        public string ID { set; get; }
        public string MaNhom { set; get; }
        public string TenNhom { set; get; }


    }
    public class PhuLieuEntity
    {
        public int ID { get; set; }
        public string MaVatTu { get; set; }
        public string TenVatTu { get; set; }
        public string CodeMau { get; set; }
        public string TenCodeMau { get; set; }
        public string DonViTinh { get; set; }
        public string Nhom { get; set; }
        public string MaKeToan { get; set; }
    }

    public class NhomNguyenPhuLieuEntity
    {
        public int ID { get; set; }
        public string MaNhom { get; set; }
        public string TenNhom { get; set; }
    }
}
