using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.XuLyVT
{
    public class XLVT
    {
        public string TrangThai { get; set; }
        public string MaPXLVT { get; set; }
        public string TenPXLVT { get; set; }
        public string LoaiXLVT { get; set; }
        public string MaNguon { get; set; }
        public string TenNguon { get; set; }
        public string NhuCau { get; set; }
        public int SLVT { get; set; }
        public int SoDonMuaHang { get; set; }
        public int SoDonGiaCong { get; set; }
        public float ChiPhiTong { get; set; }
        public DateTime? TGHTDuKien { get; set; }
        public DateTime? TGHT { get; set; }
        public int MucDoUuTien { get; set; }
        public string GhiChu { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }
    public class XLVTTable
    {
        public static DataTable create()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TrangThai", typeof(string));
            dt.Columns.Add("MaPXLVT", typeof(string));
            dt.Columns.Add("TenPXLVT", typeof(string));
            dt.Columns.Add("LoaiXLVT", typeof(string));
            dt.Columns.Add("MaNguon", typeof(string));
            dt.Columns.Add("TenNguon", typeof(string));
            dt.Columns.Add("NhuCau", typeof(string));
            dt.Columns.Add("SLVT", typeof(int));
            dt.Columns.Add("SoDonMuaHang", typeof(int));
            dt.Columns.Add("SoDonGiaCong", typeof(int));
            dt.Columns.Add("ChiPhiTong", typeof(float));
            dt.Columns.Add("TGHTDuKien", typeof(DateTime));
            dt.Columns.Add("TGHT", typeof(DateTime));
            dt.Columns.Add("MucDoUuTien", typeof(int));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }
    }
}