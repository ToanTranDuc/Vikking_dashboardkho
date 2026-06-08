using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.SYSTEM
{
    public class SystemUserEntity
    {
       
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmplyeeID { get; set; }
        public int GroupID { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsLock { get; set; }
        public bool IsLeader { get; set; }
        public bool Active { get; set; }
        public DateTime LastView { get; set; }
        public DateTime AddDate { get; set; }
        public int ViewCount { get; set; }
        public string Descriptions { get; set; }
        public string MaKho { get; set; }
    }
}
