using DevExpress.XtraReports.UI;
using NtbSoft.ERP.Entity.Kho;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class XtraReportQrCodeNPL : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraReportQrCodeNPL()
        {
            InitializeComponent();
        }

        private void XtraReportQrCodeNPL_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var data = this.GetCurrentRow() as QrcodeVatTuEntity;
            if (data == null) return;

            bool isKiemKe = data.IsKK;
            if(!isKiemKe)
                xrLabel1.Text = "VIKING VIETNAM";
        }
    }
}
