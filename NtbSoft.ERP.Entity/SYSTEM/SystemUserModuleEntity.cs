using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.SYSTEM
{
    public class SystemUserModuleEntity
    {
        
        public string ModuleID { get; set; }
        public string Alias { get; set; }
        public string Title { get; set; }
        public string ParentID { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string IsAction { get; set; }
        public string Link { get; set; }
        public string Procedured { get; set; }
        public string FormShow { get; set; }
        public string Class { get; set; }
        public string Image { get; set; }
        public string ImageUrl { get; set; }
        public bool IsBarBelow { get; set; }
        public bool AllowView { get; set; }
        public bool AllowAdd { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowDelete { get; set; }
        public string Title_English { get; set; }

    }
    public class SystemUserModuleConfig
    {
        public string UserID { get; set; }
        public string ModuleID { get; set; }
        public bool AllowView { get; set; }
        public bool AllowAdd { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowDelete { get; set; }
    }

    public class SystemUserModuleDVSX
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string TenDVSX { get; set; }
        public string MaDVSX { get; set; }
        public bool IsCheck { get; set; }
    }
    public class SystemUserWeb
    {
        public string Module{ get; set; }
        public string Title { get; set; }
        public bool IsCheckXem { get; set; }
        public bool IsCheckThem { get; set; }
        public bool IsCheckSua { get; set; }
        public bool IsCheckXoa { get; set; }
    }
}
