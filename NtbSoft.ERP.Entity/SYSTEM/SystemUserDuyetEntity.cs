using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.SYSTEM
{
    public class SystemUserDuyetEntity
    {
        public string Name { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowAdd { get; set; }
        public bool AllowActive { get; set; }
       
    }
    public class SystemUserDuyetConfigEntity
    {
        public int Pid { get; set; }
        public string UserID { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowAdd { get; set; }
        public bool AllowActive { get; set; }
      
    }
}
