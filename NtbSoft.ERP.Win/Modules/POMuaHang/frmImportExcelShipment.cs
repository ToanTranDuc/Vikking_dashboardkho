using DevExpress.DataAccess.Excel;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmImportExcelShipment : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;


        public DataTable dt = new DataTable();
        public string SheetName { get; set; } = "";
        public string Path { get; set; } = string.Empty;
        public int sttDot { get; set; } = 1;
        
        string conString = "";

        string _phieuMH = string.Empty;
        DataTable tblDot = new DataTable();
        public frmImportExcelShipment(int sttDot = 1,bool HasDot = false,string PhieuMH = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this.sttDot = sttDot;
            _phieuMH = PhieuMH;

            tblDot = new DataTable();
            tblDot.Columns.Add("Dot", typeof(string));
            tblDot.Columns.Add("STTDot", typeof(string));

            if (HasDot)
            {
                layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            else
            {
                layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            //LoadData();
            this.StartPosition = FormStartPosition.CenterScreen;
            InitSearchLookupDot();
        }
        private void InitSearchLookupDot()
        {
            try
            {

                string urlNCC = $"{URL}SimpleShipment/Get?action=GetDot&para1={_phieuMH}";
                string jsonNCC = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNCC); }).Result;
                if (jsonNCC == "[]")
                {
                  

         
                    DataRow rowDot = tblDot.NewRow();
                    rowDot["Dot"] = 1;
                    rowDot["STTDot"] = 1;
                    tblDot.Rows.Add(rowDot);
                }
                else
                {
                    tblDot = JsonConvert.DeserializeObject<DataTable>(jsonNCC);
                }
                SearchLookUpDotShipment.Properties.ValueMember = "STTDot";
                SearchLookUpDotShipment.Properties.DisplayMember = "Dot";
                SearchLookUpDotShipment.Properties.DataSource = tblDot;

                SearchLookUpDotShipment.EditValue = tblDot.Rows[0]["STTDot"];
            }
            catch (Exception ex)
            {

            }
        }
        private void SearchLookUpDotShipment_EditValueChanged(object sender, EventArgs e)
        {
            var view = SearchLookUpDotShipment.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null) return;

             DataRow selectedRow = view.GetFocusedDataRow() as DataRow;
             if (selectedRow == null) return;

            int.TryParse(selectedRow["STTDot"]?.ToString(),out int dot);

            sttDot = dot == 0 ? 1 : dot;
            

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
         
                searchLookUpEdit1.Properties.DataSource = dtSheets;
                searchLookUpEdit1.Properties.DisplayMember = "SheetName";
                searchLookUpEdit1.Properties.ValueMember = "SheetName";
                searchLookUpEdit1.Properties.NullText = "Chọn sheet...";

                
                if (dtSheets.Rows.Count > 0)
                {
                    searchLookUpEdit1.EditValue = null;
                    searchLookUpEdit1.EditValue = dtSheets.Rows[0]["SheetName"];
                }
                  
                
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (SheetName == "")
                {
                    XtraMessageBox.Show("Vui lòng chọn sheet muốn import");
                    return;
                }
                if (string.IsNullOrEmpty(SearchLookUpDotShipment.Text))
                {
                    XtraMessageBox.Show("Vui lòng nhập tên version");
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
                XtraMessageBox.Show(ex.Message);
            }
            
        }
    }
}
