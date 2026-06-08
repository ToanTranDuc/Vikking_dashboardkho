using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.SYSTEM
{
    public class SystemUserYeuCauNPLEntity
    {
        public string TenChucNang { get; set; }
        public bool AllowView { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowAdd { get; set; }
        public bool AllowDelete { get; set; }

        public bool AllowSoatXet { get; set; }

        public bool AllowDuyet { get; set; }

    }
    public class SystemUserYeuCauNPLConfigViewEntity
    {
        public int Pid { get; set; }
        public string UserID { get; set; }
        public bool AllowView { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowAdd { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowSoatXet { get; set; }

        public bool AllowDuyet { get; set; }
        public string TenChucNang { get; set; }

    }
}
