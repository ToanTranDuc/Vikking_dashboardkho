using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class BangMauEntity
    {
        public BangMauEntity()
        {

        }
        public BangMauEntity(string _maHang, string _tenMau, string _maKH, string _maMau, string _codeMau)
        {
            MaHang = _maHang;
            MaKH = _maKH;
            TenMau = _tenMau;
            CodeMau = _codeMau;
            MaMau = _maMau;
        }
        public BangMauEntity(string _maHang, string _tenMau, string _maKH, string _maMau)
        {
            MaHang = _maHang;
            MaKH = _maKH;
            TenMau = _tenMau;
            CodeMau = _tenMau;
            MaMau = _maMau;
        }
        public int ID { get; set; }
        public string MaMau { get; set; }
        public string MaHang { get; set; }
        public string CodeMau { get; set; }
        public string TenMau { get; set; }
        public string GhiChu { get; set; }
        public string MaKH { get; set; }
        //Thoai them 4 tham số
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }
}
