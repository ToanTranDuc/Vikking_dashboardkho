using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class XtraQRCodePhieuXHV1_Kho : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraQRCodePhieuXHV1_Kho(bool flagKK = false)
        {
            InitializeComponent();
            if (flagKK)
            {
                lblTitle.Text = "VIKING VIETNAM - KIỂM KÊ";
                lblPCB.Visible = false;
                lblPCBV.Visible = false;
            }
            //this.PageHeight = 192;
            //this.PageWidth = 384;
            //panel1.SizeF.Width = 382;
            //panel1.SizeF.Height = 190;
            //this.PageHeight = 165;
            //this.PageWidth = 370;
        }

        private void xrBarCode1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //xrBarCode1.Text = "4521927667PKL@018";
        }
    }
}
