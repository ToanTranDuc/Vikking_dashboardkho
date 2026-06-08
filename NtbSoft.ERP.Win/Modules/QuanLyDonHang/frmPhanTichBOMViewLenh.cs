using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmPhanTichBOMViewLenh : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _makh, _mahang, _madot = string.Empty;
        public DataTable SelectedTable { get; private set; }
        public bool HasData { get; private set; }

        public frmPhanTichBOMViewLenh(string makh, string mahang, string madot)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _makh = makh;
            _mahang = mahang;
            _madot = madot;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            loadLenhTheoDot();
            if (!HasData)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
        private void loadLenhTheoDot()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GetLenhTheoDot&para1={_makh}&para2={_mahang}&para3={_madot}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                HasData = false;
                return;
            }
            HasData = true;
            gridControl1.DataSource = tbl;
            GridView view = gridControl1.MainView as GridView;
            if (view != null)
            {
                view.SelectAll();
            }
        }
        private void btnXacNhan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GridView view = gridControl1.MainView as GridView;
            if (view == null) return;
            int[] selectedRow = view.GetSelectedRows();
            if (selectedRow.Length == 0)
            {
                //MessageBox.Show("Vui lòng chọn ít nhất một lệnh!","Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataTable dt = gridControl1.DataSource as DataTable;
            SelectedTable = dt.Clone();
            foreach (int rowHandle in selectedRow)
            {
                if (rowHandle < 0) continue;
                DataRow dr = view.GetDataRow(rowHandle);
                if (dr != null)
                    SelectedTable.ImportRow(dr);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
