using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class VatTuMinMax
    {
        public string Nguon { get; set; }
        public string MaVTID { get; set; }
        public string MaNhomVT { get; set; }
        public string MaMauVT { get; set; }
        public string MaKhoVT { get; set; }
        public decimal TonToiThieu { get; set; }
        public decimal TonToiDa { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }
    public class VatTuMinMaxTable
    {
        public static DataTable create()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Nguon", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MaNhomVT", typeof(string));
            dt.Columns.Add("MaMauVT", typeof(string));
            dt.Columns.Add("MaKhoVT", typeof(string));
            dt.Columns.Add("TonToiThieu", typeof(decimal));
            dt.Columns.Add("TonToiDa", typeof(decimal));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }
    }
}