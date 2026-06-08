using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.POMH
{
  public  class PheDuyetPhieuEntity
    {
        public string Action { get; set; } = string.Empty;
        public string MaPhieu { get; set; } = string.Empty;
        public bool IsDuyet { get; set; }
        public string NguoiPheDuyet { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;

    }
  public class XacNhanDanhGiaNhaCCEntiy
    {
        public string KQ_DG_NgDeXuat { get; set; }
        public string SignDG_NgDeXuat { get; set; }
        public string YKien_NgDeXuat { get; set; }
        public string NgayKy_NgDeXuat { get; set; }

        public string KQ_DG_SoatXet { get; set; }
        public string SignDG_SoatXet { get; set; }
        public string YKien_SoatXet { get; set; }
        public string NgayKy_SoatXet { get; set; }

        public string KQ_DG_TongGiamDoc { get; set; }
        public string SignDG_TongGiamDoc { get; set; }
        public string YKien_TongGiamDoc { get; set; }
        public string NgayKy_TongGiamDoc { get; set; }

        public string MaPhieuDG { get; set; }
        public string LoaiDanhGia { get; set; }

        public string NguoiSoatXet { get; set; }
    }
    
}
