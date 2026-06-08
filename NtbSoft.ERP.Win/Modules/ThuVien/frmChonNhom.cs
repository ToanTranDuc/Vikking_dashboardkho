using DevExpress.XtraEditors;
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
    public partial class frmChonNhom : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        public List<string> SelectedMaNhom { get; private set; } = new List<string>();
        private List<string> _selectedOld;
        private bool _isInternalSelect = false;
        private DataTable _externalData = null;

        public frmChonNhom(string currentValue)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _selectedOld = string.IsNullOrEmpty(currentValue)
                ? new List<string>()
                : currentValue.Split(';').ToList();
        }

        public frmChonNhom(string currentValue, DataTable dtNhom)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _selectedOld = string.IsNullOrEmpty(currentValue)
                ? new List<string>()
                : currentValue.Split(';').ToList();
            _externalData = dtNhom;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (_externalData != null)
                LoadFromExternal(_externalData);
            else
                LoadNhom();
        }

        private void LoadNhom()
        {
            string url = $"{URL}NhaCC/Get?action=GETNHOM&para==&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null) return;

            BindToGrid(tbl);
        }

        private void LoadFromExternal(DataTable dt)
        {
            if (dt == null) return;
            BindToGrid(dt);
        }

        private void BindToGrid(DataTable tbl)
        {
            if (!tbl.Columns.Contains("IsSelected"))
                tbl.Columns.Add("IsSelected", typeof(int));

            // Đánh dấu các dòng đã chọn trước đó
            foreach (DataRow row in tbl.Rows)
            {
                string maNhom = row["MaNhom"]?.ToString();
                row["IsSelected"] = _selectedOld.Contains(maNhom) ? 1 : 0;
            }

            gridControl1.DataSource = tbl;
            gridView1.SortInfo.Clear();
            _isInternalSelect = true;
            gridView1.BeginSelection();
            gridView1.ClearSelection();

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                int rowHandle = gridView1.GetRowHandle(i);
                if (rowHandle < 0) continue;

                DataRow row = gridView1.GetDataRow(rowHandle);
                if (row != null && Convert.ToInt32(row["IsSelected"]) == 1)
                {
                    gridView1.SelectRow(rowHandle);
                }
            }

            gridView1.EndSelection();
            _isInternalSelect = false;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SelectedMaNhom.Clear();

            int[] selectedHandles = gridView1.GetSelectedRows();

            foreach (int handle in selectedHandles)
            {
                if (handle >= 0)
                {
                    object val = gridView1.GetRowCellValue(handle, "MaNhom");
                    if (val != null && val != DBNull.Value)
                    {
                        SelectedMaNhom.Add(val.ToString());
                    }
                }
            }

            SelectedMaNhom = SelectedMaNhom.Distinct().ToList();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (_isInternalSelect) return;
            if (e.Action == System.ComponentModel.CollectionChangeAction.Refresh)
            {
                DataTable dt = gridControl1.DataSource as DataTable;
                if (dt == null) return;
                foreach (DataRow r in dt.Rows) r["IsSelected"] = 0;
                int[] selectedHandles = gridView1.GetSelectedRows();
                foreach (int handle in selectedHandles)
                {
                    DataRow row = gridView1.GetDataRow(handle);
                    if (row != null) row["IsSelected"] = 1;
                }
                return;
            }
            int rowHandle = e.ControllerRow;
            if (rowHandle < 0) return;

            DataRow rowSingle = gridView1.GetDataRow(rowHandle);
            if (rowSingle == null) return;

            if (e.Action == System.ComponentModel.CollectionChangeAction.Add)
            {
                rowSingle["IsSelected"] = 1;
            }
            else if (e.Action == System.ComponentModel.CollectionChangeAction.Remove)
            {
                rowSingle["IsSelected"] = 0;
            }
        }
    }
}