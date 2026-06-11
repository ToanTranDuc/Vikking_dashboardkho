using NtbSoft.ERP.Entity.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.POMuaHang
{
    public class PostTQRequest
    {
        public string Action { get; set; }
        public DataTable Tbl { get; set; }
        public DataTable Tbl2 { get; set; }
        public DataTable Tbl3 { get; set; }
    }
}

