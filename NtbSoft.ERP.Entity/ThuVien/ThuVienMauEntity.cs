using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class ThuVienMauEntity
    {
        public int ID { get; set; }
        public string MaMauKH { get; set; }
        public string MaKH { get; set; }
        public string CodeMauKH { get; set; }
        public string TenMauKH { get; set; }
        public string CodeKhac { get; set; }

        public string GhiChu { get; set; }

        public bool isNew { get; set; } = false;

        public string TenKH { get; set; }
        #region Thoai
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
        #endregion
    }
}
