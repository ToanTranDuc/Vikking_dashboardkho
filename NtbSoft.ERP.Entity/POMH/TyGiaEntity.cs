using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.POMuaHang
{
    public class TyGia
    {
       
    }
    public class TyGiaTable
    {
        public static DataTable create()
        {
            DataTable dt = new DataTable();;
            dt.Columns.Add("TienTe", typeof(string));
            dt.Columns.Add("MaTienTe", typeof(string));
            dt.Columns.Add("KyHieu", typeof(string));
            dt.Columns.Add("MuaTienMat", typeof(decimal));
            dt.Columns.Add("MuaChuyenKhoan", typeof(decimal));
            dt.Columns.Add("BanTienMat", typeof(decimal));
            dt.Columns.Add("BanChuyenKhoan", typeof(decimal));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            return dt;
        }
    }
}