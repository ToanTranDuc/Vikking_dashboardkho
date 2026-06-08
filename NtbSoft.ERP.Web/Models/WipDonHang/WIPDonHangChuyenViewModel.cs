using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Models.WipDonHang
{
    public class SaveWipRequest
    {
        public WIPDonHangChuyenViewModel Row { get; set; }
        public List<SaveWipChange> Changes { get; set; }
        public string ModifyBy { get; set; }
    }

    public class SaveWipDeltaRequest
    {
        public int? WipId { get; set; }
        public int? LineX { get; set; }
        public List<SaveWipDeltaChange> Changes { get; set; }
        public Dictionary<string, bool?> ManualPatch { get; set; }
        public string ModifyBy { get; set; }
        public bool IsKetThuc { get; set; } = false;
    }

    public class SaveWipDeltaChange
    {
        public string Field { get; set; }
        public object Value { get; set; }
    }

    public class WIPDonHangChuyenViewModel
    {
        public int WIPId { get; set; }
        public int ThuTuChuyen { get; set; }
        public int? ThuTuGiaCong { get; set; }
        public int? LineX { get; set; }
        public bool IsGiaCong { get; set; }
        public string LineName { get; set; }

        public int? LenhSX { get; set; }
        public string MaLenhSanXuat { get; set; }
        public string StyleId { get; set; }

        // ✅ NEW
        public string MaKH { get; set; }
        public string MaDH { get; set; }
        public string Season { get; set; }

        public string MaHang { get; set; }
        public string KhachHang { get; set; }
        public string PO { get; set; }
        public double? SLKH { get; set; }
        public bool? InTheu { get; set; }
        public bool? DoKim { get; set; }
        public bool? HutAm { get; set; }
        // ===== In/Thêu mở rộng =====
        public bool? InTheuTrong_PKH { get; set; }

        public DateTime? KH_GuiInTheu { get; set; }
        public DateTime? TT_GuiInTheu { get; set; }
        public DateTime? KH_NhanInTheu { get; set; }
        public DateTime? TT_NhanInTheu { get; set; }
        public double? NSGio { get; set; }
        public double? SoGioSX { get; set; }
        public double? SoNgay { get; set; }
        public double? BTP_DK { get; set; }
        public double? TP_KiemDat { get; set; }
        public double? TP_Nhan { get; set; }
        public double? Packing { get; set; }

        public DateTime? KHCat { get; set; }
        public DateTime? TTCat { get; set; }
        public DateTime? KHLapTrinh { get; set; }
        public DateTime? TTLapTrinh { get; set; }
        public DateTime? KHMay { get; set; }
        public DateTime? TTMay { get; set; }
        public DateTime? ThoatChuyen { get; set; }
        public DateTime? TTThoatChuyen { get; set; }

        public double? TT_Cat { get; set; }
        public double? RaChuyen { get; set; }

        public DateTime? KH_Top { get; set; }
        public double? SLSizeMau_Top { get; set; }
        public DateTime? TT_MauTop { get; set; }

        public DateTime? KH_Ship { get; set; }
        public double? SLSizeMau_Ship { get; set; }
        public DateTime? TT_Ship { get; set; }

        public DateTime? KH_Vai { get; set; }
        public string TT_Vai { get; set; }

        public DateTime? KH_PLInEp { get; set; }
        public string TT_PLInEp { get; set; }

        public DateTime? KH_PLMay { get; set; }
        public string TT_PLMay { get; set; }

        public DateTime? KH_PackingList { get; set; }
        public DateTime? TT_PackingList { get; set; }

        public DateTime? KH_PLDongGoi { get; set; }
        public string TT_PLDongGoi { get; set; }

        public DateTime? KH_LenhSX { get; set; }
        public DateTime? TT_LenhSX { get; set; }

        public double? SLCN { get; set; }
        public double? TGLV { get; set; }
        public double? OT { get; set; }
        public double? AllocatedRegularHours { get; set; }
        public double? AllocatedOTHours { get; set; }
        public double? HieuSuat { get; set; }

        public double? CostPerM { get; set; }
        public double? Profit { get; set; }

        public double? SMV_WIP { get; set; }
        public double? NSCost { get; set; }
        public double? NS_SMV { get; set; }

        public double? CM { get; set; }
        public double? HeSoCM { get; set; }
        public double? DoanhThuCM { get; set; }
        // ===== Phòng kỹ thuật =====
        public DateTime? NgayNhanTT_BOM { get; set; }
        public DateTime? KH_BOM { get; set; }
        public DateTime? TT_BOM { get; set; }
        public bool? TT_BOM_IsLateLocked { get; set; }
        public DateTime? TT_BOM_LateLockedAt { get; set; }
        public byte? TT_BOM_Status { get; set; }

        public DateTime? KH_NPL_BM_KTX_MayMau { get; set; }
        public DateTime? TT_NPL_BM_KTX_MayMau { get; set; }
        public bool? TT_NPL_BM_KTX_MayMau_NoData { get; set; }

        public DateTime? KH_TestKeoLogo_TapeLaser { get; set; }
        public DateTime? TT_TestKeoLogo_TapeLaser { get; set; }

        public DateTime? KH_Rap { get; set; }
        public DateTime? TT_Rap { get; set; }

        public DateTime? KH_TacNghiepCat { get; set; }
        public DateTime? TT_TacNghiepCat { get; set; }

        public DateTime? KH_SoDo { get; set; }
        public DateTime? TT_SoDo { get; set; }

        public DateTime? KH_BangMauVai_InEp { get; set; }
        public DateTime? TT_BangMauVai_InEp { get; set; }

        public DateTime? KH_BangMauPL_Full { get; set; }
        public DateTime? TT_BangMauPL_Full { get; set; }

        public DateTime? KH_BangDanhSo { get; set; }
        public DateTime? TT_BangDanhSo { get; set; }

        public DateTime? KH_TLieuInEp { get; set; }
        public DateTime? TT_TLieuInEp { get; set; }

        public DateTime? KH_TLieuLT { get; set; }
        public DateTime? TT_TLieuLT { get; set; }

        public DateTime? KH_TSCatPL { get; set; }
        public DateTime? TT_TSCatPL { get; set; }

        public DateTime? KH_CoiNut { get; set; }
        public DateTime? TT_CoiNut { get; set; }

        public DateTime? KH_Layout { get; set; }
        public DateTime? TT_Layout { get; set; }

        public DateTime? KH_NhanCare { get; set; }
        public DateTime? TT_NhanCare { get; set; }
        public DateTime? DateTrenNhanCare { get; set; }

        public DateTime? KH_CbiSXChoCoDien { get; set; }
        public DateTime? TT_CbiSXChoCoDien { get; set; }

        public DateTime? KH_TaiLieuHoanChinh { get; set; }
        public DateTime? TT_TaiLieuHoanChinh { get; set; }

        public DateTime? KH_KichThuoc { get; set; }
        public DateTime? TT_KichThuoc { get; set; }

        public DateTime? KH_QuyCachDongGoi { get; set; }
        public DateTime? TT_QuyCachDongGoi { get; set; }

        public DateTime? KH_HopTKSX { get; set; }
        public DateTime? TT_HopTKSX { get; set; }

        public DateTime? KH_NhanBaoThung { get; set; }
        public DateTime? TT_NhanBaoThung { get; set; }

        public DateTime? KH_MauDauChuyen { get; set; }
        public DateTime? TT_MauDauChuyen { get; set; }

        public DateTime? KH_DuyetMauDauChuyen { get; set; }
        public DateTime? TT_DuyetMauDauChuyen { get; set; }

        // ===== IE =====
        public DateTime? KH_NhuCauMayMoc { get; set; }
        public DateTime? TT_NhuCauMayMoc { get; set; }

        public DateTime? KH_Layout_IE { get; set; }
        public DateTime? TT_Layout_IE { get; set; }

        public DateTime? KH_QTMay { get; set; }
        public DateTime? TT_QTMay { get; set; }

        public DateTime? KH_DonGiaCongNhan { get; set; }
        public DateTime? TT_DonGiaCongNhan { get; set; }

        // ===== Cơ điện =====
        public DateTime? KH_ChuanBiMayMoc_KTX { get; set; }
        public DateTime? TT_ChuanBiMayMoc_KTX { get; set; }

        public DateTime? KH_ChuanBiMayMoc_Chuyen { get; set; }
        public DateTime? TT_ChuanBiMayMoc_Chuyen { get; set; }

        // ===== Kho =====
        public DateTime? KH_KhoVai { get; set; }
        public DateTime? TT_KhoVai { get; set; }

        public DateTime? KH_KhoPhuLieuMay { get; set; }
        public DateTime? TT_KhoPhuLieuMay { get; set; }

        public DateTime? KH_Thung { get; set; }
        public DateTime? TT_Thung { get; set; }
        public string GhiChuKho { get; set; }
        public string GhiChu { get; set; }
        public string TenCL { get; set; }
        public string ManualJsonPatch { get; set; }
        public string Mer { get; set; }
        public string NameMer { get; set; }
        public string ManualJson { get; set; }
        public int? SkipCalc { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CapStatus { get; set; }
        public double? CapMissingHours { get; set; }
        public DateTime? CapFirstMissingDate { get; set; }
        public DateTime? CapCheckedAt { get; set; }
        public string CapNote { get; set; }
        public string MaDVSX { get; set; }
        public string TenDVSX { get; set; }
        // ✅ NEW
        public DateTime? UpdatedAt { get; set; }

        // Step status map (key = LegacyTTColumn)
        public Dictionary<string, int?> StepStatusByTtField { get; set; }
        public Dictionary<string, bool?> StepLateLockedByTtField { get; set; }
        public Dictionary<string, bool?> StepMissingTTWarningByTtField { get; set; }
        public Dictionary<string, bool?> StepManualByField { get; set; }
    }
    public class SaveWipChange
    {
        public string Field { get; set; }
        public object NewValue { get; set; }
    }
}
