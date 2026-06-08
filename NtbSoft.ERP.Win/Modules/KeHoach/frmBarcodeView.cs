using NtbSoft.ERP.Entity.KeHoach;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmBarcodeView : DevExpress.XtraEditors.XtraForm
    {
        List<DanhSachPhieuXHEntity> lstPhieuXH = new List<DanhSachPhieuXHEntity>();
        List<DanhSachPhieuXHEntity> _lstQrCode = new List<DanhSachPhieuXHEntity>();
        public frmBarcodeView(List<DanhSachPhieuXHEntity> lstPhieu)
        {
            InitializeComponent();
            lstPhieuXH = lstPhieu;
            LoadData();
        }
        public void LoadData()
        {
            grcPhieuXH.DataSource = lstPhieuXH;
        }

        private void grvPhieuXH_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            DanhSachPhieuXHEntity rowFocus = (DanhSachPhieuXHEntity)grvPhieuXH.GetFocusedRow();
            if (rowFocus == null) return;
            if (e.Action == CollectionChangeAction.Add)
            {
                if (_lstQrCode.Exists(x => x == rowFocus)) return;
                _lstQrCode.Add(rowFocus);
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                DanhSachPhieuXHEntity rowRemove = _lstQrCode.Where(x => x.MaPhieu == rowFocus.MaPhieu).FirstOrDefault();
                if (rowRemove != null) _lstQrCode.Remove(rowRemove);
            }
            XtraQRCodePhieuXH rpt = new XtraQRCodePhieuXH();
            rpt.DataSource = _lstQrCode;
            documentViewer1.PrintingSystem = rpt.PrintingSystem;
            rpt.CreateDocument();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            grvPhieuXH.ClearSelection();
            _lstQrCode = new List<DanhSachPhieuXHEntity>();
            XtraQRCodePhieuXH rpt = new XtraQRCodePhieuXH();
            rpt.DataSource = _lstQrCode;
            documentViewer1.PrintingSystem = rpt.PrintingSystem;
            rpt.CreateDocument();
        }
    }
}
