using System;


namespace NtbSoft.ERP.Entity.NguyenPhuLieu
{
    public class KiemPLEntity
    {
        public string MaPhuLuc { get; set; }
        public string MaPhieuKiem { get; set; }
        public string SoLoID { get; set; }
        public DateTime? NgayKiem { get; set; } = new DateTime();
        public string Content { get; set; }
        public int? StatusKiem { get; set; }
        public string Img { get; set; }
        public string GhiChu { get; set; } = string.Empty;
        public string SoLot { get; set; }
        public string SoBatch { get; set; }
        public string MaVTID { get; set; }
        public string MauVTID { get; set; }
        public string MaCLVTID { get; set; }
        public string MaKhoVai { get; set; }
        public string MaNPL { get; set; }
        public string SoRoll { get; set; }
        public string NguoiKiem { get; set; }
        public string MaDonVi { get; set; }
        public float? SoMetThucTe { get; set; } = 0;
        public float? SoCuon { get; set; } = 0;       
        public bool Result { get; set; }
        public int? IsWash { get; set; }
        public string Dot { get; set; }
    }

    public class KiemPL_DanhGiaEntity
    {
        public string MaPhieuKiem { get; set; }
        public string SoLoID { get; set; }
        public DateTime? NgayKiem { get; set; }
        public string KL_QC_Pass { get; set; }
        public DateTime? NgayKiem_Pass { get; set; }
        public string Sign_Pass { get; set; }
        public string KQ_Fail { get; set; }
        public DateTime? NgayKiem_Fail { get; set; }
        public string Sign_Fail { get; set; }
        public DateTime? TPCL_ComfirmDate { get; set; }
        public string TPCL_ComfirmSign { get; set; }
        public string KQ_GiaiQuyet { get; set; }
        public string KQ_GiaiQuyet_Sign { get; set; }
        public DateTime? ReceivedInfo_Date { get; set; }
        public string ReceivedInfo_Sign { get; set; }
        public bool? Result { get; set; }
        public string MaNPL { get; set; }
        public bool? Result_Mer { get; set; }
    }
}
