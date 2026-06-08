using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.SYSTEM
{
    public class SystemGroupReportEntity
    {
        public string ReportID { get; set; }
        public string ReportName { get; set; }
        public string ParentID { get; set; }
        public string ModuleID { get; set; }
        public bool IsActive { get; set; }
    }
    public class SystemGroupReportConfig
    {
        public int GroupID { get; set; }
        public string ReportID { get; set; }
        public bool IsActive { get; set; }
    }
}
