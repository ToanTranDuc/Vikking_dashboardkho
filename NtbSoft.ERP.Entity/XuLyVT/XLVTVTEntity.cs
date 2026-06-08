using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.XuLyVT
{
    public class XLVTVT
    {
        public string TrangThai { get; set; }
        public string MaPVT { get; set; }
        public string TenPVT { get; set; }
        public string MaPXLVT { get; set; }
        public string TenPXLVT { get; set; }
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
        public float SLTong { get; set; }
        public float SLTonKho { get; set; }
        public float SLTonKhoSuDung { get; set; }
        public int SoDonMuaHang { get; set; }
        public float SLMuaHang { get; set; }
        public float SLCanMuaConLai { get; set; }
        public string TVGiaCong { get; set; }
        public int SoDonGiaCong { get; set; }
        public string GhiChu { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }
    public class XLVTVTTable
    {
        public static DataTable create()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TrangThai", typeof(string));
            dt.Columns.Add("MaPVT", typeof(string));
            dt.Columns.Add("TenPVT", typeof(string));
            dt.Columns.Add("MaPXLVT", typeof(string));
            dt.Columns.Add("TenPXLVT", typeof(string));
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
            dt.Columns.Add("SLTong", typeof(float));
            dt.Columns.Add("SLTonKho", typeof(float));
            dt.Columns.Add("SLTonKhoSuDung", typeof(float));
            dt.Columns.Add("SoDonMuaHang", typeof(int));
            dt.Columns.Add("SLMuaHang", typeof(float));
            dt.Columns.Add("SLCanMuaConLai", typeof(float));
            dt.Columns.Add("TVGiaCong", typeof(string));
            dt.Columns.Add("SoDonGiaCong", typeof(int));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }
    }
}