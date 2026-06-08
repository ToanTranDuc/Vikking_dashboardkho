using DevExpress.XtraReports.UI;
using NtbSoft.ERP.Entity.Kho;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class XtraReportQrCodePL : DevExpress.XtraReports.UI.XtraReport
    {
        private List<QrcodeVatTuEntity> _dataList;
        private int _currentIndex = 0;

        public XtraReportQrCodePL()
        {
            InitializeComponent();
            this.Detail.BeforePrint += Detail_BeforePrint;
        }

        private void XtraReportQrCodePL_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (this.DataSource is List<QrcodeVatTuEntity>)
            {
                _dataList = this.DataSource as List<QrcodeVatTuEntity>;
                _currentIndex = 0;


                if (_dataList != null && _dataList.Count == 1)
                {
                    xrPanel2.Visible = false;
                }
            }
            var data = this.GetCurrentRow() as QrcodeVatTuEntity;
            if (data != null)
            {
                bool isKiemKe = data.IsKK;
                if (!isKiemKe)
                {
                    xrLabel6.Text = "VIKING VIETNAM";
                    xrLabel6_Right.Text = "VIKING VIETNAM";
                }
            }
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (_dataList == null || _dataList.Count == 0) return;
            if (_currentIndex >= _dataList.Count)
            {
                e.Cancel = true; return;
            }
            var dataLeft = _dataList[_currentIndex];
            BindDataToPanel(xrPanel1, dataLeft, false);
            xrPanel1.Visible = true;

            if (_currentIndex + 1 < _dataList.Count)
            {
                var dataRight = _dataList[_currentIndex + 1];
                BindDataToPanel(xrPanel2, dataRight, true);
                xrPanel2.Visible = true;
            }
            else
            {
                xrPanel2.Visible = false;
            }
            _currentIndex += 2;
        }

        private void BindDataToPanel(XRPanel panel, QrcodeVatTuEntity data, bool isRight)
        {
            string suffix = isRight ? "_Right" : "";

            foreach (XRControl ctrl in panel.Controls)
            {
                if (ctrl is XRLabel)
                {
                    XRLabel label = ctrl as XRLabel;

                    // Xóa binding cũ
                    label.ExpressionBindings.Clear();

                    // Gán giá trị trực tiếp theo tên control
                    if (label.Name == "xrItemCode" + suffix)
                        label.Text = data.TenNhom;
                    else if (label.Name == "xrMoTa" + suffix)
                        label.Text = data.ChiTiet;
                    else if (label.Name == "xrMau" + suffix)
                        label.Text = data.MauVT;
                    else if (label.Name == "xrKho" + suffix)
                        label.Text = data.KhoVai;
                    else if (label.Name == "xrSoLuong" + suffix)
                        label.Text = data.SoLuongThucTe?.ToString();
                    else if (label.Name == "xrLabel16" + suffix)
                        label.Text = data.TenDVVT;
                    else if (label.Name == "xrLabel4" + suffix)
                        label.Text = data.BarCode;
                    else if (label.Name == "xrLabel2" + suffix)
                        label.Text = data.Para1;
                    else if (label.Name == "xrLabel5" + suffix)
                        label.Text = data.Para2;
                }
                else if (ctrl is XRBarCode)
                {
                    XRBarCode barcode = ctrl as XRBarCode;
                    barcode.ExpressionBindings.Clear();
                    barcode.Text = data.BarCode;
                }
            }
        }
    }
}
