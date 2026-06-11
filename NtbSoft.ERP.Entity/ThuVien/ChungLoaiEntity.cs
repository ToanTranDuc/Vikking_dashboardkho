using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class ChungLoaiEntity
    {
        public ChungLoaiEntity()
        {

        }
        public ChungLoaiEntity(string _TenCL)
        {
            TenCL = _TenCL;
        }
        public int ID { get; set; }
        public string MaCL { get; set; }
        public string TenCL { get; set; }
        public string MaNhomCL { get; set; }
        public string DonViTinh { get; set; }
        public string GhiChu { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }
}
