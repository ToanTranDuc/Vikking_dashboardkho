using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.Notification
{
    public class NotificationEntity
    {
        public string UserIDTao { get; set; }
        public string FrmName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;

        public string SendTo { get; set; } = string.Empty;
        public string BoPhan { get; set; } = string.Empty;
        public int Status { get; set; } = 0;
        public int IsQLSX { get; set; } = 0;
        public string MaPhieu { get; set; } = string.Empty;
    }
}
