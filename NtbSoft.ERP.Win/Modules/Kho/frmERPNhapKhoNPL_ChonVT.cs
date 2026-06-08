using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPNhapKhoNPL_ChonVT : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        public Action<DataRow> OnCheck;
        public Action<DataRow> OnUncheck;


        DataTable _tbl = new DataTable();
        DataTable tblMau = new DataTable();

        DataTable tblMauTV = new DataTable();
        public DataTable tblGrid = new DataTable();
        public List<DataRow> lstSelect = new List<DataRow>();
        private int _stt = 1;
        private bool isNPLDisplay = false;
        public bool IsEditMode { get; set; } = false;
        public HashSet<string> OriginalCheckedKeys { get; set; } = new HashSet<string>();
        private Dictionary<string, int> OriginalSTTMapping { get; set; } = new Dictionary<string, int>();
        public frmERPNhapKhoNPL_ChonVT(DataTable tbl, bool isNPL = false)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._tbl = tbl;
            this.isNPLDisplay = isNPL;
            this.ActiveControl = button1;

        }
        protected override void OnLoad(EventArgs e)
        {
            loadGridVatTu();
        }

        private void gV_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (view == null || info == null) return;
            int groupLevel = view.GetRowLevel(e.RowHandle);
            GridColumn groupColumn = info.Column;
            if (groupColumn == gridColumn11)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (groupColumn == gridColumn1)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (view.IsGroupRow(e.RowHandle))
            {
                Color textColor = Color.Black;
                switch (groupLevel)
                {
                    case 0: textColor = Color.MediumBlue; break;
                    case 1: textColor = Color.Maroon; break;
                }
                e.Appearance.ForeColor = textColor;
                e.DefaultDraw();
                e.Handled = true;
            }
        }

        private void gV_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = sender as GridView;
            if (e.Column.FieldName == "IsCheck")
            {
                view.UpdateCurrentRow();
            }
        }

        private void gV_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.Column.FieldName != "IsCheck") return;
            DataRow row = view.GetDataRow(e.RowHandle);
            if (row == null) return;
            string key = $"{row["MaVTID"]}_{row["MauVTID"]}_{row["MaNhom"]}_{row["KhoVaiID"]}";
            bool isCheck = Convert.ToBoolean(e.Value);
            if (isCheck)
            {
                if (!lstSelect.Contains(row))
                    lstSelect.Add(row);
                if (OriginalSTTMapping.ContainsKey(key))
                {
                    row["STTChonVT"] = OriginalSTTMapping[key];
                }
                else
                {
                    int maxSTT = 0;
                    if (OriginalSTTMapping.Count > 0)
                    {
                        maxSTT = OriginalSTTMapping.Values.Max();
                    }
                    int newSTT = maxSTT + 1;
                    row["STTChonVT"] = newSTT;
                    OriginalSTTMapping[key] = newSTT;
                }

                OnCheck?.Invoke(row);
            }
            else
            {
                lstSelect.Remove(row);
                int deletedSTT = 0;
                if (row["STTChonVT"] != DBNull.Value)
                {
                    deletedSTT = Convert.ToInt32(row["STTChonVT"]);
                }

                // Gọi OnUncheck trước khi xóa STT
                OnUncheck?.Invoke(row);

                // Xóa STT và mapping
                row["STTChonVT"] = DBNull.Value;
                row["IsCheck"] = false;
                if (OriginalSTTMapping.ContainsKey(key))
                {
                    OriginalSTTMapping.Remove(key);
                }
                if (deletedSTT > 0)
                {
                    var keysToUpdate = OriginalSTTMapping
                        .Where(kvp => kvp.Value > deletedSTT)
                        .Select(kvp => kvp.Key)
                        .ToList();

                    foreach (var keyToUpdate in keysToUpdate)
                    {
                        OriginalSTTMapping[keyToUpdate] = OriginalSTTMapping[keyToUpdate] - 1;
                    }
                    for (int i = 0; i < view.DataRowCount; i++)
                    {
                        DataRow gridRow = view.GetDataRow(i);
                        if (gridRow == null || gridRow["STTChonVT"] == DBNull.Value) continue;

                        int currentSTT = Convert.ToInt32(gridRow["STTChonVT"]);
                        if (currentSTT > deletedSTT)
                        {
                            gridRow["STTChonVT"] = currentSTT - 1;
                        }
                    }
                }
            }
            view.RefreshData();
        }

        private void loadGridVatTu()
        {
            string action = isNPLDisplay ? "GETNL" : "GETPL";
            string urlVT = $"{URL}ERPNhapKhoNPL/Get?Action={action}";
            string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
            if (string.IsNullOrEmpty(jsonVT) || jsonVT == "[]")
            {
                gC.DataSource = null;
                return;
            }
            tblGrid = JsonConvert.DeserializeObject<DataTable>(jsonVT);
            if (tblGrid == null || tblGrid.Rows.Count == 0)
            {
                gC.DataSource = null;
                return;
            }
            if (!tblGrid.Columns.Contains("STTChonVT"))
            {
                tblGrid.Columns.Add("STTChonVT", typeof(int));
            }

            if (tblGrid.Columns.Contains("NPL") && tblGrid.Columns["NPL"].DataType == typeof(bool))
            {
                bool wantNPL = isNPLDisplay;
                var filteredRows = tblGrid.AsEnumerable()
                    .Where(r => !r.IsNull("NPL") && r.Field<bool>("NPL") == wantNPL)
                    .ToList();

                tblGrid = filteredRows.Any() ? filteredRows.CopyToDataTable() : tblGrid.Clone();

           
            }

            gC.DataSource = tblGrid;
            if (_tbl != null && _tbl.Rows.Count > 0)
            {
                OriginalSTTMapping.Clear();
                var gridLookup = new Dictionary<string, DataRow>();
                foreach (DataRow row in tblGrid.Rows)
                {
                    string key = $"{row["MaVTID"]}_{row["MauVTID"]}_{row["MaNhom"]}_{row["KhoVaiID"]}";
                    if (!gridLookup.ContainsKey(key))
                    {
                        gridLookup.Add(key, row);
                    }
                }

                foreach (DataRow selectedRow in _tbl.Rows)
                {
                    string key = $"{selectedRow["MaVTID"]}_{selectedRow["MauVTID"]}_{selectedRow["MaNhom"]}_{selectedRow["KhoVaiID"]}";
                    if (selectedRow.Table.Columns.Contains("STTChonVT") && selectedRow["STTChonVT"] != DBNull.Value)
                    {
                        int originalSTT = Convert.ToInt32(selectedRow["STTChonVT"]);
                        OriginalSTTMapping[key] = originalSTT;
                        if (gridLookup.TryGetValue(key, out DataRow rowInGrid))
                        {
                            rowInGrid["STTChonVT"] = originalSTT;
                        }
                    }
                    if (gridLookup.TryGetValue(key, out DataRow rowInGrid2))
                    {
                        bool isChecked = true;
                        rowInGrid2["IsCheck"] = isChecked;
                        //rowInGrid2["STT"] = Convert.ToInt32(selectedRow["STTChonVT"]);
                        if (!lstSelect.Contains(rowInGrid2))
                        {
                            lstSelect.Add(rowInGrid2);
                        }
                    }
                }
            }
        }
        private void gV_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (IsEditMode)
            {
                GridView view = sender as GridView;
                if (view.FocusedColumn.FieldName == "IsCheck")
                {
                    object currentValue = view.GetFocusedValue();
                    bool wasChecked = (currentValue != DBNull.Value) && Convert.ToBoolean(currentValue);
                    if (wasChecked)
                    {
                        DataRow row = view.GetDataRow(view.FocusedRowHandle);
                        string currentKey = $"{row["MaVTID"]}_{row["MauVTID"]}_{row["MaNhom"]}_{row["KhoVaiID"]}";
                        if (OriginalCheckedKeys.Contains(currentKey))
                        {
                            e.Cancel = true;
                        }
                    }
                }
            }
        }
        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = button1;
            lstSelect.Clear();
            lstSelect.AddRange(tblGrid.AsEnumerable()
                  .Where(r => !r.IsNull("IsCheck") && r.Field<bool>("IsCheck")));
            this.DialogResult = DialogResult.OK;
        }
    }
}

