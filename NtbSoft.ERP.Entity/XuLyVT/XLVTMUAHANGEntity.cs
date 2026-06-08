using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.XuLyVT
{
    public class XLVTMUAHANG
    {
        public string TrangThai { get; set; }
        public string MaPMH { get; set; }
        public string TenPMH { get; set; }
        public string MaPXLVT { get; set; }
        public string TenPXLVT { get; set; }
        public string MaPVT { get; set; }
        public string TenPVT { get; set; }
        public string MaVT { get; set; }
        public string TenVT { get; set; }
        public string ChiTietVT { get; set; }
        public string LoaiVT { get; set; }
        public string InSeamMauVT { get; set; }
        public string MaMauVT { get; set; }
        public string TenMauVT { get; set; }
        public string MaSizeVT { get; set; }
        public string TenSizeVT { get; set; }
        public string MaDVVT { get; set; }
        public string TenDVVT { get; set; }
        public string MaNhaCungCap { get; set; }
        public string TenNhaCungCap { get; set; }
        public string DVTienTe { get; set; }
        public string DonGia { get; set; }
        public float SL { get; set; }
        public float ChiPhiTong { get; set; }
        public DateTime? TGHTDuKien { get; set; }
        public DateTime? TGHT { get; set; }
        public string PYCBaoGia { get; set; }
        public string PNhapKho { get; set; }
        public string ViTriLuuKho { get; set; }
        public string GhiChu { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }
    public class XLVTMUAHANGTable
    {
        public static DataTable create()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TrangThai", typeof(string));
            dt.Columns.Add("MaPMH", typeof(string));
            dt.Columns.Add("TenPMH", typeof(string));
            dt.Columns.Add("MaPXLVT", typeof(string));
            dt.Columns.Add("TenPXLVT", typeof(string));
            dt.Columns.Add("MaPVT", typeof(string));
            dt.Columns.Add("TenPVT", typeof(string));
            dt.Columns.Add("MaVT", typeof(string));
            dt.Columns.Add("TenVT", typeof(string));
            dt.Columns.Add("ChiTietVT", typeof(string));
            dt.Columns.Add("LoaiVT", typeof(string));
            dt.Columns.Add("InSeamMauVT", typeof(string));
            dt.Columns.Add("MaMauVT", typeof(string));
            dt.Columns.Add("TenMauVT", typeof(string));
            dt.Columns.Add("MaSizeVT", typeof(string));
            dt.Columns.Add("TenSizeVT", typeof(string));
            dt.Columns.Add("MaDVVT", typeof(string));
            dt.Columns.Add("TenDVVT", typeof(string));
            dt.Columns.Add("MaNhaCungCap", typeof(string));
            dt.Columns.Add("TenNhaCungCap", typeof(string));
            dt.Columns.Add("DVTienTe", typeof(string));
            dt.Columns.Add("DonGia", typeof(string));
            dt.Columns.Add("SL", typeof(float));
            dt.Columns.Add("ChiPhiTong", typeof(float));
            dt.Columns.Add("TGHTDuKien", typeof(DateTime));
            dt.Columns.Add("TGHT", typeof(DateTime));
            dt.Columns.Add("PYCBaoGia", typeof(string));
            dt.Columns.Add("PNhapKho", typeof(string));
            dt.Columns.Add("ViTriLuuKho", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }
    }
}