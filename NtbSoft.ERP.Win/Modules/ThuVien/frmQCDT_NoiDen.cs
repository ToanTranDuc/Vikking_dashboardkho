using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Utils;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmQCDT_NoiDen : DevExpress.XtraEditors.XtraForm
    {
        public string _styleId = "";
        public string _maHang = "";
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable _dtNoiDen;
        public frmQCDT_NoiDen()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _dtNoiDen = new DataTable();
        }
        public frmQCDT_NoiDen(string styleId, string maHang) : this()
        {
            _styleId = styleId;
            _maHang = maHang;
            gridControl1.DataSource = null;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            gridControl1.DataSource = null;
            InitializeGrid();
            LoadData();
        }
        private void InitializeGrid()
        {
            gridView1.OptionsBehavior.Editable = true;
            gridView1.OptionsBehavior.EditingMode = GridEditingMode.Default;
            gridView1.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
        }
        private async void LoadData()
        {
            try
            {
                gridView1.PostEditor();
                gridView1.UpdateCurrentRow();
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(ResourceForm.frmWait));

                string url = URL + $"CaiDatThongSoKHDT/Get?action=GetNoiDen&Para1={_styleId}";
                string json = await _clientExtension.GetAsnyc(url);
                if (!string.IsNullOrWhiteSpace(json) && json.Trim() != "[]" && json.Trim() != "null")
                {
                    _dtNoiDen = JsonConvert.DeserializeObject<DataTable>(json);
                    if (_dtNoiDen.Rows.Count > 0)
                    {
                        _maHang = _dtNoiDen.Rows[0]["MaHang"]?.ToString() ?? _maHang;
                    }
                }
                else
                {
                    _dtNoiDen = new DataTable();
                    _dtNoiDen.Columns.Add("StyleID", typeof(string));
                    _dtNoiDen.Columns.Add("MaHang", typeof(string));
                    _dtNoiDen.Columns.Add("MaNoiDen", typeof(string));
                    _dtNoiDen.Columns.Add("NoiDen", typeof(string));
                }
                gridControl1.DataSource = null;
                gridControl1.RefreshDataSource();
                gridControl1.DataSource = _dtNoiDen;
                EnsureAllDesRow();
                if (gridView1.Columns["MaHang"] != null)
                {
                    gridView1.Columns["MaHang"].OptionsColumn.AllowEdit = false;
                    gridView1.Columns["MaHang"].OptionsColumn.ReadOnly = true;
                }
                if (gridView1.RowCount > 0)
                {
                    gridView1.FocusedRowHandle = 0;
                    gridView1.FocusedColumn = gridView1.Columns["NoiDen"];
                    gridView1.ShowEditor();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                }
                catch { }
            }
        }
        private void btAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridView1.PostEditor();
            gridView1.UpdateCurrentRow();
            DataRow newRow = _dtNoiDen.NewRow();
            newRow["StyleID"] = _styleId;
            newRow["MaHang"] = _dtNoiDen.Rows.Count > 0
                ? _dtNoiDen.Rows[0]["MaHang"]?.ToString()
                : _maHang;
            newRow["MaNoiDen"] = "";
            newRow["NoiDen"] = "";
            _dtNoiDen.Rows.InsertAt(newRow, 0);
            gridControl1.RefreshDataSource();
            gridView1.FocusedRowHandle = gridView1.RowCount - 1;
            gridView1.FocusedColumn = gridView1.Columns["NoiDen"];
        }
        private void btSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveRows();
        }
        private async void SaveRows()
        {
            try
            {
                gridView1.PostEditor();
                gridView1.UpdateCurrentRow();

                DataTable dtSave = _dtNoiDen.Clone();
                bool isRenameChung = false;
                string tenMoi = "";
                foreach (DataRow row in _dtNoiDen.Rows)
                {
                    string maNoiDen = row["MaNoiDen"]?.ToString();
                    string noiDen = row["NoiDen"]?.ToString()?.Trim();
                    if (string.IsNullOrWhiteSpace(noiDen)) continue;
                    DataRow newRow = dtSave.NewRow();
                    newRow["MaHang"] = _styleId;
                    newRow["MaNoiDen"] = maNoiDen;
                    newRow["NoiDen"] = noiDen;
                    dtSave.Rows.Add(newRow);
                }
                if (dtSave.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu để lưu!");
                    return;
                }
                if (dtSave.Columns.Contains("StyleID"))
                    dtSave.Columns.Remove("StyleID");

                string url = URL + "CaiDatThongSoKHDT/PostNoiDen";
                string json = await _clientExtension.PostAsync(url, dtSave);
                if (json.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 1500);
                }
                else
                {
                    XtraMessageBox.Show(json);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void btDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DeleteRow();
        }
        private async void DeleteRow()
        {
            if (gridView1.FocusedRowHandle < 0)
            {
                XtraMessageBox.Show("Vui lòng chọn dòng cần xóa!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (row == null)
                return;
            string maNoiDen = row["MaNoiDen"]?.ToString();
            string noiDen = row["NoiDen"]?.ToString();
            if (string.IsNullOrWhiteSpace(maNoiDen) || maNoiDen == "0")
            {
                if (XtraMessageBox.Show($"Xóa nơi đến: {noiDen}\nMã hàng: {_maHang}\n\nBạn có chắc chắn?",
                   "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                gridView1.DeleteRow(gridView1.FocusedRowHandle);
                return;
            }
            try
            {
                string urlCheck = $"{URL}CaiDatThongSoKHDT/Get?Action=GetCheckNoiDen&Para1={maNoiDen}";
                string jsonCheck = await _clientExtension.GetAsnyc(urlCheck);
                if (jsonCheck != "[]")
                {
                    if (XtraMessageBox.Show($"Xóa nơi đến: {noiDen}\nMã hàng: {_maHang} đã được cài Thông số/Qui cách\nBạn có chắc chắn?",
                     "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;
                }
                else
                {
                    if (XtraMessageBox.Show($"Xóa nơi đến: {noiDen}\nMã hàng: {_maHang}\n\nBạn có chắc chắn?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;
                }
                string url = $"{URL}CaiDatThongSoKHDT/DeleteNoiDen?Para1={_styleId}&Para2={maNoiDen}";
                string result = await _clientExtension.DeletedAsync(url);
                if (result.ToUpper() == "TRUE")
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    LoadData();
                }
                else
                {
                    XtraMessageBox.Show("Có lỗi xảy ra khi xóa nơi đến!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }
        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.N)
            {
                btAdd_ItemClick(null, null);
                e.Handled = true;
                return;
            }
            if (e.Control && e.KeyCode == Keys.S)
            {
                btSave_ItemClick(null, null);
                e.Handled = true;
                return;
            }
            switch (e.KeyCode)
            {
                case Keys.F1:
                    btAdd_ItemClick(null, null);
                    e.Handled = true;
                    break;
                case Keys.Delete:
                    DeleteRow();
                    e.Handled = true;
                    break;
                case Keys.F4:
                    btSave_ItemClick(null, null);
                    e.Handled = true;
                    break;
                case Keys.F5:
                    LoadData();
                    e.Handled = true;
                    break;
            }
        }
        private void gridView1_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            GridView view = sender as GridView;
            string noiDen = view.GetRowCellValue(e.RowHandle, "NoiDen")?.ToString().Trim();
            if (!string.IsNullOrEmpty(noiDen))
            {
                for (int i = 0; i < view.RowCount; i++)
                {
                    if (i == e.RowHandle) continue;
                    string existingNoiDen = view.GetRowCellValue(i, "NoiDen")?.ToString().Trim();
                    if (!string.IsNullOrEmpty(existingNoiDen) &&
                        string.Equals(noiDen, existingNoiDen, StringComparison.OrdinalIgnoreCase))
                    {
                        e.Valid = false;
                        e.ErrorText = $"Nơi đến '{noiDen}' đã tồn tại!";
                        return;
                    }
                }
            }
        }
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = e.ListSourceRowIndex + 1;
            }
        }
        private void EnsureAllDesRow()
        {
            DataRow[] allRows = _dtNoiDen.Select("MaNoiDen = '0'");
            foreach (DataRow row in allRows)
            {
                row.Delete();
            }
            if (_dtNoiDen.Rows.Count == 0 || _dtNoiDen.Select("MaNoiDen <> '0'").Length == 0)
            {
                DataRow row = _dtNoiDen.NewRow();
                row["StyleID"] = _styleId;
                row["MaHang"] = _maHang;
                row["MaNoiDen"] = "0";
                row["NoiDen"] = "Chung";
                _dtNoiDen.Rows.InsertAt(row, 0);
            }
            _dtNoiDen.AcceptChanges();
        }
        //private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        //{
        //    if (e.Column.FieldName == "NoiDen")
        //    {
        //        DataRow row = gridView1.GetDataRow(e.RowHandle);
        //        if (row != null)
        //        {
        //            string maNoiDen = row["MaNoiDen"]?.ToString();
        //            if (maNoiDen == "0")
        //            {
        //                _oldNoiDenName = row["NoiDen"]?.ToString() ?? "";
        //            }
        //        }
        //    }
        //}
    }
}