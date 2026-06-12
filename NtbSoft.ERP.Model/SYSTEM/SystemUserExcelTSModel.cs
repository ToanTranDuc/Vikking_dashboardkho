using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.SYSTEM
{
    public class SystemUserExcelTSModel
    {
        public string TenChucNang { get; set; }
        public bool AllowExcel { get; set; }
    }
    public class SystemUserExcelTSConfigViewModel
    {
        public int Pid { get; set; }
        public string UserID { get; set; }

        public bool AllowExcel { get; set; }
     
    }
}
