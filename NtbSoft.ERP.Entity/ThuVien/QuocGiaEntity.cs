using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.ThuVien
{
    public class QuocGiaEntity
    {

        public QuocGiaEntity()
        {

        }
        public QuocGiaEntity(string tenQG)
        {
            TenQG = tenQG;
        }
        public int ID { get; set; }
        public string MaQG { get; set; }
        public string TenQG { get; set; }
        public string GhiChu { get; set; }

        public string MaBuuChinh { get; set; }
        public string MaDienThoai { get; set; }
        public string MaQuocKy { get; set; }
        public string TenTiengAnh { get; set; }
        public string MaCang { get; set; }
        public string TenCang { get; set; }
        #region Thoai
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
        #endregion
    }

    public class QuocKy
    {
        public Bitmap image;
        public object ma;
        public QuocKy(Bitmap _image, string _ma)
        {
            image = _image;
            ma = _ma;
        }
    }

}
