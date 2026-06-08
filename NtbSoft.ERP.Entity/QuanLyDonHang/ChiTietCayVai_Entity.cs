using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.QuanLyDonHang
{
    public class ChiTietCayVai_Entity
    {
        public class CayVaiPOMua
        {
            public string MaVT { get; set; }
            public string MauVT { get; set; }
            public string KhoVai { get; set; }
            public string TenDVVT { get; set; }
            public string ChiTiet { get; set; }
            public string POMua { get; set; }
            public string ChungLoaiVatTu { get; set; }
            public string SoKienHienThi { get; set; }
            public string SoLuongThucTe { get; set; }
            public DateTime NgayTaoNhapKho { get; set; }
            public string SoLoT { get; set; }
            public string Batch { get; set; }
            public string MaONPL { get; set; }
            public string MaVTID { get; set; }
            public string KhoVaiID { get; set; }
            public string TenCL { get; set; }
            public string SoLoID { get; set; }
            public string MaNPL { get; set; }
            public string BarCode { get; set; }
            public string MaNhom { get; set; }
            public string MaDVVT { get; set; }
            public string MauVTID { get; set; }
            public int IsNPL { get; set; }
        }
    }
}
