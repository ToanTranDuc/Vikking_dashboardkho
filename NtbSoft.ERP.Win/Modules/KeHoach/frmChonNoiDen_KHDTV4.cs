using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using NtbSoft.ERP.Win.Utils;
using NtbSoft.ERP.Win.Modules.QuanLyDonHang;
using DevExpress.SpreadsheetSource;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using DevExpress.Spreadsheet;
using System.Data.OleDb;
using DevExpress.DataAccess.Excel;
using System.Globalization;
using OfficeOpenXml;
using System.IO;
using NtbSoft.ERP.Entity.ThuVien;
using DevExpress.XtraSplashScreen;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmChonNoiDen_KHDTV4 : XtraForm
    {
        public string SelectedMaNoiDen { get; set; } = "0";

        public frmChonNoiDen_KHDTV4(DataTable dt)
        {
            InitializeComponent();

            searchLookUpEdit_NoiDen.Properties.DataSource = dt;
            searchLookUpEdit_NoiDen.Properties.DisplayMember = "NoiDen";
            searchLookUpEdit_NoiDen.Properties.ValueMember = "MaNoiDen";

            if (dt.Rows.Count > 1)
                searchLookUpEdit_NoiDen.EditValue = dt.Rows[1]["MaNoiDen"];
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedMaNoiDen = searchLookUpEdit_NoiDen.EditValue?.ToString() ?? "0";
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
