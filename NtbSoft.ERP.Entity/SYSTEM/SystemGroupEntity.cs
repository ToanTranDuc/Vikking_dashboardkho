using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.SYSTEM
{
    public class SystemGroupEntity
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public string Descriptions { get; set; }
        public bool Active { get; set; }
    }
}
