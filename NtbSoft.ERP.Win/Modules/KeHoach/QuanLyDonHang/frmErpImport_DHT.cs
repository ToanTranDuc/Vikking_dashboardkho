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
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using OfficeOpenXml;

namespace NtbSoft.ERP.Win.Modules.Erp.Kehoach
{
    public partial class frmErpImport_DHT : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                              new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        string _maDH = string.Empty, _malenhsanxuat = string.Empty, _makh = string.Empty;
        bool _ChkCT = false, _ChkTH = false;
        CustomImportDataExcel _customImport;
        // Sự kiện để thông báo khi Form đóng và trả dữ liệu về
        public event EventHandler<string> DataClosed;
        System.Data.DataTable tbl_sheet;
        private HttpClientExtension _clientExtension;
        public string resultIndex;
        public string resultFilePath = string.Empty, _nameExcel = string.Empty;
        public bool result = false;
        public frmErpImport_DHT()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _customImport = new CustomImportDataExcel();
            textEdit1.Enabled = false;
            _clientExtension = new HttpClientExtension();
        }
        private void btGetLink_Click(object sender, EventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                tbl_sheet = new DataTable();
                AddColumn_tbl_Sheet();
                textEditLink.Text = Sfd.FileName;
                CreateDefaultLookUp(textEditLink.Text);
                textEdit1.Enabled = true;
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                FileInfo fileInfo = new FileInfo(Sfd.FileName);
                using (var package = new ExcelPackage(fileInfo))
                {
                    var workbook = package.Workbook;
                    if (workbook.Worksheets.Count == 1)
                    {
                        textEdit1.EditValue = workbook.Worksheets[0].Name; 
                    }
                }
            }
        }

        private void AddColumn_tbl_Sheet()
        {
            tbl_sheet.Columns.Add("Name");
        }

        private void CreateDefaultLookUp(string pathExecel)
        {
            ISpreadsheetSource spreadsheetSource = SpreadsheetSourceFactory.CreateSource(pathExecel);
            IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
            int worksheetcount = worksheetCollection.Count();
            for (int i = 0; i < worksheetcount; i++)
            {
                DataRow r = tbl_sheet.NewRow();
                r["Name"] = worksheetCollection[i].Name;
                tbl_sheet.Rows.Add(r);
            }
            textEdit1.Properties.DataSource = tbl_sheet;
            textEdit1.Properties.ValueMember = "Name";
            textEdit1.Properties.DisplayMember = "Name";
            //textEdit1.EditValue = 0;
        }
        private async void btImport_Click(object sender, EventArgs e)
        {
            try
            {
                btImport.Enabled = false;
                string newFilePath = string.Empty;
                if (string.IsNullOrEmpty(textEditLink.Text))
                {
                    XtraMessageBox.Show(Properties.Resources.DataValid, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    btImport.Enabled = true;
                    return;
                }
                if (textEdit1.EditValue == null)
                {
                    MessageBox.Show("Chưa chọn sheet ", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btImport.Enabled = true;
                    return;
                }
                if (string.IsNullOrEmpty(textEditLink.Text))
                {
                    XtraMessageBox.Show(Properties.Resources.DataValid, Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    btImport.Enabled = true;
                    return;
                }
                resultFilePath = textEditLink.Text;
                string sheetIndex = this.textEdit1.EditValue.ToString();
                resultIndex = sheetIndex;
                result = true;
                this.Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}