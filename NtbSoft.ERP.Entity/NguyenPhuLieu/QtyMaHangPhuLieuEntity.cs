using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.Qty
{
    public class QtyMaHangPhuLieuEntity
    {
        public int ID { get; set; }
        public string MaDH { get; set; }
        public string MaHang { get; set; }
        public string MaVatTu { get; set; }
        public string QuiCach { get; set; }
        public float SoLuong { get; set; }
        public float SoLuongThuc { get; set; }
        public float SoLuongKiem { get; set; }
        public float SoLuongLoi { get; set; }
        public string MaLoi { get; set; }
        public float Balance { get; set; }
        public string GhiChu { get; set; }
        public string Image { get; set; }
        public string NgayKiem { get; set; }
        public string Dot { get; set; }
        public string NVKiem { get; set; }
    }
    public class QtyMaHangKiemVaiEntity
    {
        public int ID { get; set; }
        public string MaDH { get; set; }
        public string MaHang { get; set; }
        public string MaVai { get; set; }
        public string MaVTMau { get; set; }
        public string Dot { get; set; }
        public string SoCayID { get; set; }
        public string SoCay { get; set; }
        public float SoLuong { get; set; }
        public float SoLuongTT { get; set; }
        public string Kho { get; set; }
        public string KhoTT { get; set; }
        public string MaVTri { get; set; }
        public string MaLoi { get; set; }
        public int DiemLoi { get; set; }
        public int LoaiKiem { get; set; }
        public string GhiChu { get; set; }
        public string Image { get; set; }
        public string NgayKiem { get; set; }
        public string NVKiem { get; set; }
    }
    public class QTY_KiemVaiV2Entity
    {
        public int ID { get; set; }
        public string SoLoID { get; set; }
        public string SoLo { get; set; }
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string MaHang { get; set; }
        public string TenHang { get; set; }
        public string LoaiVai { get; set; }
        public string TenLoaiVai { get; set; }
        public string MaVTID { get; set; }
        public string MaVT { get; set; }
        public string MauVTID { get; set; }
        public string LOT { get; set; }
        public string Batch { get; set; }
        public string SoCay { get; set; }
        public string Weight { get; set; }
        public int? StatusWeight { get; set; }
        public string ImgWeight { get; set; }
        public string Shrinkage { get; set; }
        public int? StatusShrinkage { get; set; }
        public string ImgShrinkage { get; set; }
        public string Waterproof { get; set; }
        public int? StatusWaterproof { get; set; }
        public string ImgWaterproof { get; set; }
        public string ReceivingQuantity { get; set; }
        public string CheckingQuantity { get; set; }
        public string FaceSide { get; set; }
        public int? StatusFaceSide { get; set; }
        public string ImgFaceSide { get; set; }
        public string BlackSide { get; set; }
        public int? StatusBlackSide { get; set; }
        public string ImgBlackSide { get; set; }
        public string RollLabel { get; set; }
        public string RollActual { get; set; }
        public string CutProctect { get; set; }
        public string ColorThread { get; set; }
        public string ColorShading { get; set; }
        public int? StatusColorShading { get; set; }
        public string ImgColorShading { get; set; }
        public string JoiningPointsRoll { get; set; }
        public string IDErrorType { get; set; }
        public string Note { get; set; }
        public string ResultQC { get; set; }
        public int? StatusResultQC { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string NVKiem { get; set; }
        public string KhoVai { get; set; }
        public string KhoVaiActual { get; set; }
        public int ColorStatus { get; set; }
        public string ColorText { get; set; }
        public string ColorImg { get; set; }
        public int PercentStatus { get; set; }
        public string PercentText { get; set; }
        public string PercentImg { get; set; }
        public int ErrorPoint { get; set; }
        public int StatusResult { get; set; }
        public int IsXacNhan { get; set; }
        public string MaNPL { get; set; }
        public int RollStatus { get; set; }
        public int KhoVaiStatus { get; set; }
        public int ResultQCStatus { get; set; }
    }
    public class QTY_KiemVaiV2_ErrorPointEntity
    {
        public int ID { get; set; }
        public string ErrorTypeID { get; set; }
        public string ErrorType { get; set; }
        public int? OnePoint { get; set; }
        public int? TwoPoint { get; set; }
        public int? ThreePoint { get; set; }
        public int? FourPoint { get; set; }
    }
    public class QTY_KiemVaiV2_SignEntity
    {
        public int ID { get; set; }
        public string SoLoID { get; set; }
        public string LoaiVai { get; set; }
        public string MaVTID { get; set; }
        public string MauVTID { get; set; }
        public DateTime? NgayKiemVai { get; set; }
        public DateTime? NgaySign { get; set; }
        public string Sign { get; set; }
        public string SignManager { get; set; }
        public string SignReceive { get; set; }
        public string MaNPL { get; set; }
    }
    public class QTY_KiemVaiV2_XacNhanEntity
    {
        public string SoLoID { get; set; }
        public string MaNPL { get; set; }
        public string UserXN { get; set; }
        public DateTime? NgayNhapKho { get; set; }
        public int Dot { get; set; }
    }
}
