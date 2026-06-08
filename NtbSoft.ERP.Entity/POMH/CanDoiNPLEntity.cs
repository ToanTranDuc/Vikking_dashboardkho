using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.POMuaHang
{
    public class CanDoiNPL
    {
        public string TrangThai { get; set; }
        public bool IsDuyet { get; set; }
        public string MaPhieu { get; set; }
        public string TenPhieu { get; set; }
        public string MaDH { get; set; }
        public int STTDH { get; set; }
        public string MaLenhSanXuat { get; set; }
        public string MaDot { get; set; }
	    public string MaVTID { get; set; }
	    public string MaNhomVT { get; set; }
        public string MaMauVT { get; set; }
        public string MaKhoVT { get; set; }
        public string MaDVVT { get; set; }
	    public decimal SLCanDoiKho { get; set; }
	    public decimal SLTonKhoSauCanDoi { get; set; }
	    public decimal SLMuaThem { get; set; }
        public decimal TyLeMuaThem { get; set; }
        public string GhiChu { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }       
    }
    public class CanDoiNPLTable
    {
        public static DataTable create()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TrangThai", typeof(string));
            dt.Columns.Add("IsDuyet", typeof(bool));
            dt.Columns.Add("MaPhieu", typeof(string));
            dt.Columns.Add("TenPhieu", typeof(string));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("STTDH", typeof(int));
            dt.Columns.Add("MaLenhSanXuat", typeof(string));
            dt.Columns.Add("MaDot", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
	        dt.Columns.Add("MaNhomVT", typeof(string));
            dt.Columns.Add("MaMauVT", typeof(string));
            dt.Columns.Add("MaKhoVT", typeof(string));
            dt.Columns.Add("MaDVVT", typeof(string));
            dt.Columns.Add("SLCanDoiKho", typeof(decimal));
            dt.Columns.Add("SLTonKhoSauCanDoi", typeof(decimal));
            dt.Columns.Add("SLMuaThem", typeof(decimal));
            dt.Columns.Add("TyLeMuaThem", typeof(decimal));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            dt.Columns.Add("Sort", typeof(int));
            dt.Columns.Add("IsDHKhGui", typeof(bool));
            return dt;
        }
    }
}