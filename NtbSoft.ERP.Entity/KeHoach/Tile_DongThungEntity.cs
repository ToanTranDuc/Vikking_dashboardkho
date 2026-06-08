using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.KeHoach
{
   public class Tile_DongThungEntity
    {
        public string SizeID { get; set; }
        public string Size { get; set; }
        public double SLDT { get; set; }
        public double SLConLai { get; set; }
        public double SLThung { get; set; }
        public int PCB { get; set; }
        public double NW { get; set; }
        public double GW { get; set; }
        public int PCB_Pack { get; set; }
        public int Pack_Ctn { get; set; }
        public bool IsThungLe { get; set; }
    }
    public class Store_DongThungEntity
    {
        public string SizeID { get; set; }
        public string Size { get; set; }
        public double SLDT { get; set; }
        public int SLConLai { get; set; }
        public double SLThung { get; set; }       
        public double NW { get; set; }
        public double GW { get; set; }       
    }
    public class DTStoreAlarm_Entity
    {
        public string PO { get; set; }
        public string Size { get; set; }
        public string Store { get; set; }
        public int SLuong { get; set; }
    }
    public class DauSizeEntity
    {
        public string MaDVSX { get; set; }
        public string TenDVSX { get; set; }
        public string DotSX { get; set; }
        public string MaLenh { get; set; }
        public string SizeTypeID { get; set; }
        public string SizeType { get; set; }
        public string Value { get; set; }
        public int SapXep { get; set; }
    }    
}
