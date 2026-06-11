using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NtbSoft.ERP.Entity.Kho
{
    public class ERPKhoVaiEntity
    {
        public ERPKhoVaiEntity()
        {

        }
        public ERPKhoVaiEntity( string _khovai,bool _NPL)
        {
          
            KhoVai = _khovai;
            NPL = _NPL;
        }
        public int ID { get; set; }
       public string KhoVaiID { get; set; }
       public string KhoVai { get; set; }
        public bool NPL { get; set; } = true;

    }
}
