using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.SYSTEM
{
    public class SystemUserReportEntity : SystemUserReportConfig
    {
        public string ReportName { get; set; }
        public string ParentID { get; set; }
        public string ModuleID { get; set; }
        public string FormShow { get; set; }
    }
    public class SystemUserReportConfig
    {
        public string UserID { get; set; }
        public string ReportID { get; set; }
        public bool IsActive { get; set; }
    }
}
