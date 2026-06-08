using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Models.SYSTEM
{
    public class SystemUserDuyetViewModel
    {
        public string Name { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowAdd { get; set; }
        public bool AllowActive { get; set; }
    }
    public class SystemUserDuyetConfigViewModel
    {
        public int Pid { get; set; }
        public string UserID { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowAdd { get; set; }

        public bool AllowActive { get; set; }
    }
}