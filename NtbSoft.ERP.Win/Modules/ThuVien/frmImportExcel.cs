using DevExpress.DataAccess.Excel;
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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmImportExcel : DevExpress.XtraEditors.XtraForm
    {

        public DataTable dt = new DataTable();
        public string SheetName { get; set; } = "";
        public string Path { get; set; } = string.Empty;
        string conString = "";
        public frmImportExcel()
        {
            InitializeComponent();
            searchLookUpEdit1.EditValueChanged += SearchLookUpEdit1_EditValueChanged;
        }
        protected override void OnLoad(EventArgs e)
        {
            LoadData();
        }

        private void SearchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            if(searchLookUpEdit1.EditValue is null)
            {
                SheetName = "";
                return;
            }
            SheetName = searchLookUpEdit1.EditValue.ToString();
        }

        private void btnChonFile_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        public void LoadData()
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";
            DataTable dtSheets = new DataTable();
            dtSheets.Columns.Add("SheetName", typeof(string));
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                string FilePath = Sfd.FileName;
                txtDuongDan.Text = FilePath;
                string worksheetName = string.Empty;
                using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(FilePath))
                {
                    DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                    foreach (var sheet in worksheetCollection)
                    {
                        dtSheets.Rows.Add(sheet.Name);
                    }
                    //worksheetName = worksheetCollection[0].Name;
                }
                // Bind vào SearchLookUpEdit
                searchLookUpEdit1.Properties.DataSource = dtSheets;
                searchLookUpEdit1.Properties.DisplayMember = "SheetName";
                searchLookUpEdit1.Properties.ValueMember = "SheetName";
                searchLookUpEdit1.Properties.NullText = "Chọn sheet...";

                // Tuỳ chọn: chọn sheet đầu tiên mặc định
                if (dtSheets.Rows.Count > 0)
                {
                    searchLookUpEdit1.EditValue = null;
                    searchLookUpEdit1.EditValue = dtSheets.Rows[0]["SheetName"];
                }
                  
                //var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                //source.FileName = FilePath;
                //var worksheetSettings = new ExcelWorksheetSettings(worksheetName);
                //source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                //source.Fill();
                //DataTable dtSource = new DataTable();
                //dtSource = source.ToDataTable();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (SheetName == "")
                {
                    MessageBox.Show("Vui lòng chọn sheet muốn import");
                    return;
                }
                var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                source.FileName = txtDuongDan.Text;
                var worksheetSettings = new ExcelWorksheetSettings(SheetName);
                source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                source.Fill();                
                dt = source.ToDataTable();
                this.Path = txtDuongDan.Text;
                this.Close();
            }
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
