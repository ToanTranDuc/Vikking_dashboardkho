using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.SoTheoDoi
{
    public class TheKhoEntity
    {
        public TheKhoEntity()
        {

        }
        public TheKhoEntity(string _MaKho, string _MaHang, string _ToSo, DateTime _NgayNhapKho, string _MaKH)
        {
            this.MaKho = _MaKho;
            this.MaHang = _MaHang;
            this.ToSo = _ToSo;
            this.NgayNhapKho = _NgayNhapKho;
            this.MaKhachHang = _MaKH;

            this.NgayChungTu = DateTime.Now;
        }
        public int ID { get; set; }
        public string MaTheKho { get; set; }
        public string MaKho { get; set; }
        public DateTime NgayNhapKho { get; set; }
        public string ToSo { get; set; }
        public string MaHang { get; set; }
        public string MaKhachHang { get; set; }
        public DateTime NgayChungTu { get; set; }
        public string SoChungTu { get; set; }
        public string MoTa { get; set; }
        public int SLNhap { get; set; }
        public int SLXuat { get; set; }
        public int SLTon { get; set; }
        public string MaDV { get; set; }
    }
}
