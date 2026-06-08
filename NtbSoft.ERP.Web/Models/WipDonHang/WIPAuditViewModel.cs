using System;

namespace NtbSoft.ERP.Web.Models.WipDonHang
{
    public class WIPAuditViewModel
    {
        public int AuditId { get; set; }
        public int WipId { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string ModifiedByName { get; set; }
    }
}
