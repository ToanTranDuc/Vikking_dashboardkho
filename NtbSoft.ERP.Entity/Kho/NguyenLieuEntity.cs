using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.Kho
{
    public class NguyenLieuEntity
    {
        public int ID { get; set; }

        public string MaDH { get; set; }
        public string MaVatTu { get; set; }
        public string TenVatTu { get; set; }
        public string MaHaiQuan { get; set; }
        public string NhaCungCap { get; set; }
        public string LoNhap { get; set; }
        public string ItemCode { get; set; }
        public string CodeMau { get; set; }
        public string TenCodeMau { get; set; }
        public string KhoVai { get; set; }
        public string TongKien { get; set; }
        public string Kien { get; set; }
        public string Lot { get; set; }
        public string AnhMau { get; set; }
        public string NhomNPL { get; set; }
        public string TenNhom { get; set; }
        public double SoLuong { get; set; }
        public string DonViTinh { get; set; }
        public string BarCode { get; set; }
        public string MaKeToan { get; set; }
        public string MaNguyenLieu { get; set; }

        public string TenDVCL { get; set; }
    }
}
