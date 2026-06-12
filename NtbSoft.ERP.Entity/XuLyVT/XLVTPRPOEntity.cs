using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.XuLyVT
{
    public class XLVTPRPO
    {
        public string ID { get; set; }
        public string Ten { get; set; }
        public string indentID { get; set; }
        public string indentName { get; set; }
        public string supplyID { get; set; }
        public string supplyName { get; set; }
        public string supplierID { get; set; }
        public string supplierName { get; set; }
        public string TrangThai { get; set; }
        public string ColorID { get; set; }
        public string TenMau { get; set; }
        public string SizeID { get; set; }
        public string Size { get; set; }
        public string DonGia { get; set; }
        public float SoLuong { get; set; }
        public float TongChiTieu { get; set; }
        public string TacVuGiaCong { get; set; }
        public DateTime? TGGH { get; set; }
        public DateTime? TGHTDuKien { get; set; }
        public DateTime? TGHT { get; set; }
        public string PhieuBaoGia { get; set; }
        public string PhieuNhapKho { get; set; }
        public string ViTriLuuKho { get; set; }
        public string GhiChu { get; set; }
        public DateTime? createdAt { get; set; }
    }
    public class XLVTPRPOTable
    {
        public static DataTable create()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Ten", typeof(string));
            dt.Columns.Add("indentID", typeof(string));
            dt.Columns.Add("indentName", typeof(string));
            dt.Columns.Add("supplyID", typeof(string));
            dt.Columns.Add("supplyName", typeof(string));
            dt.Columns.Add("supplierID", typeof(string));
            dt.Columns.Add("supplierName", typeof(string));
            dt.Columns.Add("TrangThai", typeof(string));
            dt.Columns.Add("ColorID", typeof(string));
            dt.Columns.Add("TenMau", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("Size", typeof(string));
            dt.Columns.Add("DonGia", typeof(string));
            dt.Columns.Add("SoLuong", typeof(float));
            dt.Columns.Add("TongChiTieu", typeof(float));
            dt.Columns.Add("TacVuGiaCong", typeof(string));
            dt.Columns.Add("TGGH", typeof(DateTime));
            dt.Columns.Add("TGHTDuKien", typeof(DateTime));
            dt.Columns.Add("TGHT", typeof(DateTime));
            dt.Columns.Add("PhieuBaoGia", typeof(string));
            dt.Columns.Add("PhieuNhapKho", typeof(string));
            dt.Columns.Add("ViTriLuuKho", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("createdAt", typeof(DateTime));
            return dt;
        }
    }
}