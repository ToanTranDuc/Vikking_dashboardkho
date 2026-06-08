using DevExpress.XtraReports.UI;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Win.Modules.Kho;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ViTriKho
{
    public partial class frmQRCodeViewer : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        HttpClientExtension _clientExtension;
        List<ViTriKhoEntity> _lstVTK = new List<ViTriKhoEntity>();
        List<ViTriKhoEntity> _lstAll = new List<ViTriKhoEntity>();
        private bool _isNPL;

        public frmQRCodeViewer(List<ViTriKhoEntity> lst)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _lstVTK = lst.Where(x => x.MaVT.Contains("00") == false).ToList();
            _lstAll = lst;
            //_lstVTK = lst;
            LoadData();
           
        }
        public frmQRCodeViewer(string action, List<QrcodeVatTuEntity> lst, bool isNPL)
        {
            InitializeComponent();
            _isNPL = isNPL;
            LoadDataVT(lst);
        }

        private void LoadDataVT(List<QrcodeVatTuEntity> lst)
        {
            XtraReport rpt;

            if (_isNPL)
                rpt = new XtraReportQrCodeNPL();
            else
                rpt = new XtraReportQrCodePL();
            rpt.DataSource = lst;
            documentViewer1.PrintingSystem = rpt.PrintingSystem;
            rpt.CreateDocument();
        }

        private void LoadData()
        {
            if (_lstVTK == null) return;
            PrepareTenVT();
            XtraReport_QrCode_ViTriKho rpt = new XtraReport_QrCode_ViTriKho();
            rpt.DataSource = _lstVTK;
            documentViewer1.PrintingSystem = rpt.PrintingSystem;
            rpt.CreateDocument();
        }

        private void PrepareTenVT()
        {
            foreach(ViTriKhoEntity o in _lstVTK)
            {
                List<string> arr = new List<string>();
                arr.Add(o.TenVT);
                ViTriKhoEntity tang = _lstAll.Where(x => x.MaVT == o.ParentMaVT).FirstOrDefault();
                arr.Add(tang.TenVT);
                ViTriKhoEntity ke = _lstAll.Where(x => x.MaVT == tang.ParentMaVT).FirstOrDefault();
                arr.Add(ke.TenVT);
                o.TenVT = string.Format("{0} - {1} - {2}", arr[2], arr[1], arr[0]);
            }
        }
    }
}
