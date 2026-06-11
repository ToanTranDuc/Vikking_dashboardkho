using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.POMuaHang
{
  public class ERPDanhGiaNCCEntity
    {
        public long ID { get; set; }
        public string MaCLVTID { get; set; }
        public string Quy { get; set; }
        public string Nam { get; set; }
        public string NgayDanhGia { get; set; }
        public string NguoiDanhGia { get; set; }
        public string GhiChu { get; set; }

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

        public string TieuChiID { get; set; }
        public string DiemID { get; set; }
        public string MaNhaCC { get; set; }
        public string ThuTuUuTien { get; set; }
        public string KetQua { get; set; }
        public string TenPhieu { get; set; }
        public string NgayDanhGiaPhieu { get; set; }
        public string NguoiDanhGiaPhieu { get; set; }
        public string NguoiSoatXet { get; set; }
    }

    public class PhieuDanhGiaNCCNoiLamViecEntity
    {
        public long ID { get; set; }              
        public string MaPhieuDG { get; set; }     
        public string TenPhieuDG { get; set; }     
        public bool? KetQuaDat { get; set; }            
        public string MaNCC { get; set; }          
        public string NgayDG { get; set; }          
        public string NguoiDanhGia { get; set; }    
        public string Version { get; set; }        
        public long? NhomDanhGia { get; set; }      
        public long? TieuChiNoiLVID { get; set; }  
        public bool? ApDung { get; set; }         
        public decimal? DiemDatDuoc { get; set; }  
        public string GhiChu { get; set; }         
    }
}
