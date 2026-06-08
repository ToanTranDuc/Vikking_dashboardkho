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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmChonQuocGia : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        public List<string> SelectedMaQG { get; private set; } = new List<string>();
        private List<string> _selectedOld;
        private List<string> _manualQG = new List<string>();
        private bool _isInternalSelect = false;
        public frmChonQuocGia(string factoryValue)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _selectedOld = string.IsNullOrEmpty(factoryValue)
            ? new List<string>()
            : factoryValue.Split(';').ToList();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadQuocGia();
        }

        private void LoadQuocGia()
        {
            string url = $"{URL}NhaCC/Get?action=GetQG&para==&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null) return;

            if (!tbl.Columns.Contains("IsSelected"))
                tbl.Columns.Add("IsSelected", typeof(int));

            HashSet<string> apiQGSet = new HashSet<string>(
                tbl.AsEnumerable()
                   .Select(r => r["TenQG"]?.ToString())
                   .Where(x => !string.IsNullOrEmpty(x))
            );

            _manualQG = _selectedOld
                .Where(x => !apiQGSet.Contains(x))
                .ToList();

            foreach (DataRow row in tbl.Rows)
            {
                string maQG = row["TenQG"]?.ToString();
                row["IsSelected"] = _selectedOld.Contains(maQG) ? 1 : 0;
            }

            gridControl1.DataSource = tbl;
            gridView1.SortInfo.Clear();
            gridView1.SortInfo.Add(new DevExpress.XtraGrid.Columns.GridColumnSortInfo(
                gridView1.Columns["IsSelected"],
                DevExpress.Data.ColumnSortOrder.Descending
            ));
            _isInternalSelect = true;
            gridView1.BeginSelection();
            gridView1.ClearSelection();

            //for (int i = 0; i < gridView1.RowCount; i++)
            //{
            //    int rowHandle = gridView1.GetRowHandle(i);
            //    if (rowHandle < 0) continue;

            //    DataRow row = gridView1.GetDataRow(rowHandle);
            //    if (row != null && Convert.ToInt32(row["IsSelected"]) == 1)
            //    {
            //        gridView1.SelectRow(rowHandle);
            //    }
            //}

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
            SelectedMaQG.Clear();
            SelectedMaQG.AddRange(_manualQG);

            DataTable tbl = gridControl1.DataSource as DataTable;
            //if (tbl == null) return;

            //foreach (DataRow row in tbl.Rows)
            //{
            //    if (Convert.ToInt32(row["IsSelected"]) == 1)
            //    {
            //        string maQG = row["TenQG"]?.ToString();
            //        if (!string.IsNullOrEmpty(maQG))
            //            SelectedMaQG.Add(maQG);
            //    }
            //}

            if (tbl != null)
            {
                // 2. Thêm các quốc gia được chọn trong grid
                foreach (DataRow row in tbl.Rows)
                {
                    if (Convert.ToInt32(row["IsSelected"]) == 1)
                    {
                        string maQG = row["TenQG"]?.ToString();
                        if (!string.IsNullOrEmpty(maQG))
                            SelectedMaQG.Add(maQG);
                    }
                }
            }
            SelectedMaQG = SelectedMaQG
            .Distinct()
            .ToList();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (_isInternalSelect) return;

            int rowHandle = e.ControllerRow;
            if (rowHandle < 0) return;

            DataRow row = gridView1.GetDataRow(rowHandle);
            if (row == null) return;

            if (e.Action == System.ComponentModel.CollectionChangeAction.Add)
            {
                row["IsSelected"] = 1;
            }
            else if (e.Action == System.ComponentModel.CollectionChangeAction.Remove)
            {
                row["IsSelected"] = 0;
            }
        }
    }
}