using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.POMuaHang
{
    public class DanhGiaNoiLamBViec
    {

    }
    public class DanhGiaNoiLamBViecTable
    {
        public static DataTable create()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("TenNhomDanhGia", typeof(string));
            dt.Columns.Add("GhiChuNhom", typeof(string));
            dt.Columns.Add("Version", typeof(int));
            dt.Columns.Add("TenVersion", typeof(string));
            dt.Columns.Add("Sort", typeof(int));
            dt.Columns.Add("IsActive", typeof(bool));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NhomDanhGia", typeof(int));
            dt.Columns.Add("No", typeof(string));
            dt.Columns.Add("TieuChi", typeof(string));
            dt.Columns.Add("DiemToiDa", typeof(decimal));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("IsUse", typeof(bool));

            return dt;
        }
    }
}