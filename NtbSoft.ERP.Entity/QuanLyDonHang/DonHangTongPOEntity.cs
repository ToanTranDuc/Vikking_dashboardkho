using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.QuanLyDonHang
{
    public class DonHangTongPOEntity
    {

        public DonHangTongPOEntity() { }

        public DonHangTongPOEntity(string _maDH, string _poID, string _maMau, string _dauSizeID)
        {
            this.MaDH = _maDH;
            this.POID = _poID;
            this.MaMau = _maMau;
            this.DauSizeID = _dauSizeID;
            this.NgayGH = DateTime.Now.Date;
        }
        public int ID { get; set; }
        public string MaDH { get; set; }
        public string MaQG { get; set; }
        public string POID { get; set; }
        public string PO { get; set; }
        public string MaMau { get; set; }
        public string DauSizeID { get; set; }
        public string DauSize { get; set; }
        public DateTime NgayGH { get; set; }
        public int SoLuong { get; set; }
        public string GhiChu { get; set; }
        public string ColorCode { get; set; }
        public DateTime? NgayDKVC { get; set; }
        public DateTime? NgayThucTeVC { get; set; }
        public DateTime? NgayXuatHang { get; set; }

      
    }
}
