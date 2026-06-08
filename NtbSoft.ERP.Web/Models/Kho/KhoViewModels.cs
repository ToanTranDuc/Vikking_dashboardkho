using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtbSoft.ERP.Web.Models.THIETBI
{
    public class KhoViewModels
    {
        public string Id { get; set; }
        public string MaKho { get; set; }
        public string TenKho { get; set; }
        public string Ghichu { get; set; }
        public int Status_VT { get; set; }
        public string ParentMaKho { get; set; }
    }
    public class PhieuDieuDongViewModels
    {
        public int ID { get; set; }
        public string MaKho { get; set; }
        public string MaTB { get; set; }
        public string SoPhieu { get; set; }
        public DateTime NgayTao { get; set; }
    }
    public class KhoDetaiViewModels
    {
        public int ID_Row { get; set; }
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string TenKho { get; set; }
        public string Ghichu { get; set; }
        public int Status_VT { get; set; }
        public bool Status_NL { get; set; }
        public bool Status_PL { get; set; }
    }
}