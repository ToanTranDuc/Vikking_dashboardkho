using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.Kho
{
    public class VatTuEntity
    {
        public int ID { get; set; } 

        public string MaVTMau { get; set; }

        public string MaKeToan { get; set; }
        public string MaVT { get; set; } 
        public string TenVT { get; set; } 
        public string CodeVT { get; set; }
        public string MaMau { get; set; }
        public string Mau { get; set; }
        public string CodeMau { get; set; }
        public string ItemCode { get; set; }

        public string MaSizeKho { get; set; }
        public string SizeKho { get; set; } 
        public string TenGD { get; set; } 
        public string MaKho { get; set; }
        public string MaDVTinh { get; set; }
        public string DVTinh { get; set; } 
        public float? DV1 { get; set; } 
        public float? DV2 { get; set; } 
        public string MaNhomVT { get; set; } 
        public string NhaCungCap { get; set; } 
        public string MaVTHaiQuan { get; set; } 
        public string TenHaiQuan { get; set; } 
        public string XuatXu { get; set; } 
        public float? SLTonToiThieu { get; set; } 
        public float? SLTonToiDa { get; set; } 
        public string TKVT { get; set; } 
        public string TKGiaVon { get; set; } 
        public string TKDoanhThu { get; set; } 
        public string TKHangBanTraLai { get; set; } 
        public float? ThueSuat { get; set; } 
        public string GhiChu { get; set; } 
        public bool IsNL { get; set; } 
        public bool IsPL { get; set; } 
    }
}
