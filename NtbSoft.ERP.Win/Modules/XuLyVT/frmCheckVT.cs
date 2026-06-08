using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.XuLyVT;
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

namespace NtbSoft.ERP.Win.Modules.MuaHang
{
    public partial class frmCheckVT : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        KeyDownControlHandler keyDownControlHandler;

        private string indentID;
        private DataTable supplyResultTable;
        public frmCheckVT(string id = null)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            indentID = id;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SplashScreenManager.ShowForm(this, typeof(frmLoading), true, true, false);
            try
            {
                //this.WindowState = FormWindowState.Maximized;
                loadGridControl1();
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }
        private void loadGridControl1()
        {
            RepositoryItemCheckEdit checkEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            gridControl1.RepositoryItems.Add(checkEdit);
            DataTable dts = getDataGridControl1();
            if (!dts.Columns.Contains("IsSelected"))
                dts.Columns.Add("IsSelected", typeof(bool));
            GridColumn colCheck = gridView1.Columns["IsSelected"];
            colCheck.ColumnEdit = checkEdit;

            gridControl1.DataSource = dts;
            gridView1.ExpandAllGroups();
        }
        private DataTable getDataGridControl1()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getbyindent";
            request.Parameter = indentID;
            string urlGetListDataTable = URL + "XLVTItem/Get";
            string jsonRequest = JsonConvert.SerializeObject(request);
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            if (dt == null || dt.Columns.Count == 0)
            {
                DataTable newdt = XLVTVTTable.create();
                return newdt;
            }
            return dt;
        }
        public DataTable selectedSupply()
        {           
            return supplyResultTable;
        }

        private void confirmBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();

            DataTable dt = gridControl1.DataSource as DataTable;
            DataRow[] checkedRows = dt.Select("IsSelected = True");

            // tạo DataTable mới từ các dòng đã chọn
            if(checkedRows.Length == 0)
            {
                XtraMessageBox.Show("Chưa vật tư nào được chọn");
                //this.Close();
                return;
            }
            supplyResultTable = dt.Clone();
            foreach (DataRow row in checkedRows)
            {
                supplyResultTable.ImportRow(row);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void checkAllBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                gridView1.SetRowCellValue(i, "IsSelected", true);
            }
        }

        private void resetBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            for (int i = 0; i < gridView1.RowCount; i++)
            {
                gridView1.SetRowCellValue(i, "IsSelected", false);
            }
        }
    }
}