using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.POMH
{
    public class ERP_POMH_PhieuMuaHang_MMTB_SoSeriEntity
    {
        public long? ID { get; set; }                    
        public string PhieuMH { get; set; }             
        public string PhieuYC { get; set; }           
        public string POMH { get; set; }                  
        public string MaCL { get; set; }                 
        public string CodeVatTuMua { get; set; }         
        public string SoSeri { get; set; }               
        public DateTime? NgayKetThucBH { get; set; }     
        public DateTime? NgayBatDauBH { get; set; }      
        public int? LoaiPhieu { get; set; }              
        public string TenVatTu { get; set; }              
        public string MauMa { get; set; }                 
        public string XuatXu { get; set; }                
        public string HangSX { get; set; }               
        public string NamSX { get; set; }                 
        public string ChuKiBaoTri { get; set; }          
        public string MaNhaCC { get; set; }              
        public float? DonGia { get; set; }             
        public float? SoLuong { get; set; }              
        public string DonVi { get; set; }                
        public string KhauHao { get; set; }              
        public string GhiChu { get; set; }                
        public DateTime? NgayXacNhan { get; set; }        
        public string NguoiXacNhan { get; set; }
        public bool? IsGet { get; set; }
        public bool? IsThietBi { get; set;}
        public DateTime? NgayMua { get; set; }
    }
}
