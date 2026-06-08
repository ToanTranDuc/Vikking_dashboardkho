using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.QuanLyDonHang
{
    public class DonHangTongPOChiTietUSEREntity
    {
        public DonHangTongPOChiTietUSEREntity()
        {

        }

        public DonHangTongPOChiTietUSEREntity(string _poID, string _maMau,string _dauSizeID)
        {
            this.POID = _poID;
            this.MaMau = _maMau;
            this.DauSizeID = _dauSizeID;
        }
        public int ID { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public string MaMau { get; set; }
        public string DauSizeID { get; set; }
        public string DauSize { get; set; }
        public string SizeID { get; set; }
        public string Size { get; set; }
        public int SoLuong { get; set; }
        public int SoLuongSX { get; set; }
        public bool CheckTruTK { get; set; }
        public string GhiChu { get; set; }
        public int Sort { get; set; }
        public string MaKiemTra { get; set; }
        public string NguoiTao { get; set; }
    }
}
