using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.QuanLyDonHang
{
    public class BomNguyenPhuLieuEntity
    {
        public int? ID { get; set; }
        public string MaBom { set; get; }
        public string MaDH { set; get; }
        public string MaVatTu { set; get; }
        public string TenVatTu { set; get; }
        public string Mau { set; get; }
        public string KhoVai { set; get; }
        public string DonViTinh { set; get; }
        public float? DinhMucMuaHang { set; get; }
        public float? DinhMucThucTe { set; get; }
        public float? DinhMucKH { set; get; }
        public string GhiChu { set; get; }
        public string NgayLap { get; set; }
        public string MaSP { get; set; } 
        public string MaTiLe { get; set; }
        public int? SLChungTu { get; set; }
        public string PhanLoaiVT { get; set; }
        public string NhaCungCap { get; set; }
        public string POID { get; set; }
        public string MaMau { get; set; }
        public string DauSizeID { get; set; }
        public string SizeID { get; set; }
        public int? SLSanPham { get; set; }
        public int? SLDinhMuc { get; set; }
        public float? HaoHut { get; set; }
        public int? SL { get; set; }
        public string SoChungTu { get; set; }  
        public int? SanPham { get; set; } 
        public float? ChieuDaiCuon { get; set; }
        public int? SoLuongCan { get; set; }
        public float? SLCanBangMau { get; set; }
        public float? DonGia { get; set; }
        public float? ThanhTien { get; set; }
        public int? SLThucTe { get; set; }
        public int? SLDat { get; set; }
        public int? Chon { get; set; }


    }
}
