using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Win.Utils
{
   
    public class Status
    {
        public Status(int id, string name) { _Id = id; _Name = name; }

        private int _Id;
        private string _Name;

        public int Id { get { return _Id; } set { _Id = value; } }
        public string Name { get { return _Name; } set { _Name = value; } }

        /// <summary>
        /// Now used to return the Name.
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return Name; }
    }
   
    public class StatusList : List<Status>
    {
    }
}
