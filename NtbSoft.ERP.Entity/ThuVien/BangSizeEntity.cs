using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class BangSizeEntity
    {
        public BangSizeEntity()
        {

        }
        public BangSizeEntity(string _sizeSanXuat, string _tenSize, string _maHang)
        {
            SizeSanXuat = _sizeSanXuat;
            TenSize = _tenSize;
            MaHang = _maHang;
           
        }

        public BangSizeEntity(string _sizeSanXuat, string _tenSize,string _nhomSize, string _maHang, string _codesize, string _ghichu, string _maKH, string _maSize, string _maNhomSize,string _Sort)
        {
            SizeSanXuat = _sizeSanXuat;
            TenSize = _tenSize;
            NhomSize = _nhomSize;
            MaHang = _maHang;
            CodeSize = _codesize;
            GhiChu = _ghichu;
            MaKH = _maKH;
            MaSize = _maSize;
            MaNhomSize = _maNhomSize;
            Sort = _Sort;
        }
        public int ID { get; set; }
        public string MaSize { get; set; }
        public string MaHang { get; set; }
        public string CodeSize { get; set; }
        public string TenSize { get; set; }
        public string SizeSanXuat { get; set; }
        public string GhiChu { get; set; }
        public string NhomSize { get; set; }
        public string MaKH { get; set; }
        public string MaNhomSize { get; set; }
        public string Sort { get; set; }
        //Thoai them 4 tham số
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }
}
