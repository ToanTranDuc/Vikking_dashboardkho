using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class DonViSanXuatEntity
    {
        public int ID { get; set; }
        public string MaDVSX { get; set; }
        public string TenDVSX { get; set; }
        public bool GiaCong { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
        public int Sort { get; set; }
        public string MaHienThi { get; set; }

        public DonViSanXuatEntity(int _sort=0)
        {
            this.Sort = _sort;
        }
    }

    public class DonViSanXuatDetailEntity
    {
        public string MaDVSX { get; set; }
        public string DonViSanXuat { get; set; }
        public bool GiaCong { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
    }
}
