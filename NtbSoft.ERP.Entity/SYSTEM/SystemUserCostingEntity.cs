using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.SYSTEM
{
    public class SystemUserCostingEntity
    {
        public string FuncName { get; set; }
        public bool AllowView { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowAdd { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowCreateApproval { get; set; }
        public bool AllowSupplierApproval { get; set; }
        public bool AllowSMVApproval { get; set; }
    }
    public class SystemUserCostingConfigViewEntity
    {
        public int Pid { get; set; }
        public string FuncName { get; set; }
        public string UserID { get; set; }
        public bool AllowView { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowAdd { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowCreateApproval { get; set; }
        public bool AllowSupplierApproval { get; set; }
        public bool AllowSMVApproval { get; set; }
    }
}
