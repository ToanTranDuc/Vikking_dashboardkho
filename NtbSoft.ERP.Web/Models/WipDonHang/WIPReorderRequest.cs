using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Models.WipDonHang
{
    public class WIPReorderRequest
    {
        public int LineX { get; set; }
        public int FromThuTu { get; set; }
        public int ToThuTu { get; set; }
        public bool IsKetThuc { get; set; } = false;
    }
}