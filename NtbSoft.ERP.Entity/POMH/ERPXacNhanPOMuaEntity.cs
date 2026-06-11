using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Entity.POMuaHang
{
    public class ERPXacNhanPOMuaEntity
    {
        public string Action { get; set; } = string.Empty;
        public string MaPhieu { get; set; } = string.Empty;
        public string MaVTID { get; set; } = string.Empty;
        public string MaCLVT { get; set; } = string.Empty;
        public string MauVTID { get; set; } = string.Empty;
        public string KhoVaiID { get; set; } = string.Empty;
        public string NgayXN { get; set; } = string.Empty;
        public bool IsXacNhan { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string NguoiXN { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;

    }


    public class ERPXacNhanPOMuaMMTBEntity
    {
        public string Action { get; set; } = string.Empty;
        public string MaPhieu { get; set; } = string.Empty;
        public string MaHang { get; set; } = string.Empty;
        public string MaCL { get; set; } = string.Empty;
        public string MaNhom { get; set; } = string.Empty;
        public string NgayXN { get; set; } = string.Empty;
        public bool IsXacNhan { get; set; }
        public bool IsTB { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;

    }
}