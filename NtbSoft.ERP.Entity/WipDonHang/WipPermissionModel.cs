using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.WipDonHang
{
    public class WipPermissionModel
    {
        public string ColKey { get; set; }
        public string Title { get; set; }
        public string GroupName { get; set; }
        public bool IsCheckXem { get; set; }
        public bool IsCheckSua { get; set; }
        public int SortOrder { get; set; }
    }
    public class WipPermissionFeatureModel
    {
        public string FeatureKey { get; set; }
        public string FeatureName { get; set; }
        public bool IsAllow { get; set; }
    }
    public class WipPermNode
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }

        public string GroupName { get; set; } // text nhóm
        public string ColKey { get; set; }    // key cột
        public string ColName { get; set; }   // tên cột hiển thị

        public bool IsCheckXem { get; set; }
        public bool IsCheckSua { get; set; }

        public int SortOrder { get; set; }
        public bool IsGroup { get; set; }     // node nhóm
    }
}
