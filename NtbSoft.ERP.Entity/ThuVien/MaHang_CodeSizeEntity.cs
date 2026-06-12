using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class MaHang_CodeSizeEntity
    {
        public int ID { get; set; }
        public string MaHang { get; set; }
        public string MaMau { get; set; }
        public string CodeMau { get; set; }
        public string MaSize { get; set; }
        public string CodeSize { get; set; }
    }
    public class MaHangConfigEntity : MaHang_CodeSizeEntity
    {
        public string TenSize { get; set; }
        public string TenMau { get; set; }
     
    }
}
