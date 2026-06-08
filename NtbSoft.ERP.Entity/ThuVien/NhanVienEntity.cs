using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class NhanVienEntity
    {
        public NhanVienEntity(string _manv, string _tennv, string _chucvu, string _phongban,
            string _ghichu, string _ten, string _nguoitao, DateTime? _ngaytao, string _nguoisua, DateTime? _ngaysua)
        {
            MaNV = _manv;
            TenNV = _tennv;
            ChucVu = _chucvu;
            PhongBan = _phongban;
            GhiChu = _ghichu;
            Ten = _ten;
            NguoiTao = _nguoitao;
            NgayTao = _ngaytao;
            NguoiSua = _nguoisua;
            NgaySua = _ngaysua;
        }
        public NhanVienEntity()
        {

        }

        public int ID { get; set; }
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public string ChucVu { get; set; }
        public string PhongBan { get; set; }
        public string UserID { get; set; }
        public string GhiChu { get; set; }
        public string HinhAnh { get; set; }
        public string Ten { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }
}
