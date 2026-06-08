using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmSettingTiLeLoi : DevExpress.XtraEditors.XtraForm
    {
        private readonly AppSettingsReader _settingsReader = new AppSettingsReader();
        private readonly HttpClientExtension _client = new HttpClientExtension();
        private string _url = string.Empty;

        private string _selectedLine = string.Empty;
        private string _selectedLenh = string.Empty;
        private string _selectedMaHang = string.Empty;

        public frmSettingTiLeLoi()
        {
            InitializeComponent();
            _url = (string)_settingsReader.GetValue("URL", typeof(string));

            // Mở khóa sự kiện tạo dòng mới
            gridView1.InitNewRow += gvTiLeLoi_InitNewRow;
            searchLookUpEdit1View.CustomDrawCell += searchLookUpEdit1View_CustomDrawCell;
            gridView1.OptionsSelection.MultiSelect = true;
            gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
        }

        private void searchLookUpEdit1View_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column == gridColumnSTT && e.RowHandle >= 0)
            {
                e.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadLineX();
        }

        private void LoadLineX()
        {
            try
            {
                string url = _url + "SettingTiLeLoi/GetLineX";
                string json = Task.Run(async () => await _client.GetAsnyc(url)).Result;
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                searchLookUpEdit1.Properties.DataSource = dt;
                searchLookUpEdit1.Properties.ValueMember = "LineX";
                searchLookUpEdit1.Properties.DisplayMember = "Name";

                gridColumnLine.FieldName = "Name";
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Lỗi tải danh sách Chuyền!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            _selectedLine = searchLookUpEdit1.EditValue?.ToString() ?? string.Empty;
            searchLookUpEdit2.EditValue = null;
            textEditViewSL.Text = string.Empty;
            _selectedLenh = string.Empty;
            _selectedMaHang = string.Empty;

            LoadLenh(_selectedLine);
            LoadSettingTiLeLoi();
        }

        private void LoadLenh(string lineX)
        {
            try
            {
                string url = string.Format("{0}SettingTiLeLoi/GetLenh?lineX={1}", _url, lineX);
                string json = Task.Run(async () => await _client.GetAsnyc(url)).Result;
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                searchLookUpEdit2.Properties.DataSource = dt;
                searchLookUpEdit2.Properties.ValueMember = "MaLenh";
                searchLookUpEdit2.Properties.DisplayMember = "LenhDisplay";
                gridColumnLenh.FieldName = "MaLenh";
                gridColumnLenhDisplay.FieldName = "LenhDisplay";
                gridColumnSL.FieldName = "SLKH";
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Lỗi tải danh sách Lệnh!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            _selectedLenh = searchLookUpEdit2.EditValue?.ToString() ?? string.Empty;

            DataRowView row = searchLookUpEdit2.GetSelectedDataRow() as DataRowView;
            if (row != null)
            {
                _selectedMaHang = row["MaHang"]?.ToString() ?? string.Empty;
                textEditViewSL.Text = row["SLKH"]?.ToString() ?? string.Empty;
            }
            else
            {
                _selectedMaHang = string.Empty;
                textEditViewSL.Text = string.Empty;
            }

            LoadSettingTiLeLoi();
        }

        private void LoadSettingTiLeLoi()
        {
            if (string.IsNullOrEmpty(_selectedLine) || string.IsNullOrEmpty(_selectedLenh))
            {
                DataTable dtEmpty = new DataTable();
                dtEmpty.Columns.Add("ID", typeof(int));
                dtEmpty.Columns.Add("TiLeLoi", typeof(string));
                dtEmpty.Columns.Add("NgayCaiDat", typeof(DateTime));

                // Đã mở khóa
                gridView1.GridControl.DataSource = dtEmpty;
                return;
            }

            try
            {
                string url = string.Format("{0}SettingTiLeLoi/GetSettingTiLeLoi?line={1}&lenh={2}&maHang={3}",
                    _url, _selectedLine, _selectedLenh, _selectedMaHang);
                string json = Task.Run(async () => await _client.GetAsnyc(url)).Result;

                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                if (dt == null || dt.Columns.Count == 0)
                {
                    dt = new DataTable();
                    dt.Columns.Add("ID", typeof(int));
                    dt.Columns.Add("TiLeLoi", typeof(string));
                    dt.Columns.Add("NgayCaiDat", typeof(DateTime));
                }

                // Đã mở khóa
                gridView1.GridControl.DataSource = dt;
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Lỗi tải dữ liệu Tỉ Lệ Lỗi!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gvTiLeLoi_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view != null)
            {
                view.SetRowCellValue(e.RowHandle, "ID", 0);
                view.SetRowCellValue(e.RowHandle, "TiLeLoi", "0");
                view.SetRowCellValue(e.RowHandle, "NgayCaiDat", DateTime.Now);
            }
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadSettingTiLeLoi();
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveRow();
        }

        // Sự kiện cho nút Thêm Dòng trên BarManager (Bạn nhớ đảm bảo Name của nút là btnAddRow)
        private void btnAddRow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedLine) || string.IsNullOrEmpty(_selectedLenh))
            {
                XtraMessageBox.Show("Vui lòng chọn Chuyền và Lệnh trước!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            gridView1.AddNewRow();
        }

        private void SaveRow()
        {
            if (string.IsNullOrEmpty(_selectedLine))
            {
                XtraMessageBox.Show("Vui lòng chọn Chuyền trước khi lưu!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(_selectedLenh))
            {
                XtraMessageBox.Show("Vui lòng chọn Lệnh trước khi lưu!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();

            bool hasError = false;
            int successCount = 0;

            for (int i = 0; i < gridView1.RowCount; i++)
            {
                var tiLeLoi = gridView1.GetRowCellValue(i, "TiLeLoi")?.ToString() ?? "";
                var ngayCaiDat = gridView1.GetRowCellValue(i, "NgayCaiDat");
                var id = gridView1.GetRowCellValue(i, "ID")?.ToString() ?? "0";

                string ngayStr = ngayCaiDat != null && ngayCaiDat != DBNull.Value
                    ? Convert.ToDateTime(ngayCaiDat).ToString("yyyy-MM-dd")
                    : string.Empty;

                try
                {
                    string url = string.Format(
                        "{0}SettingTiLeLoi/SaveSettingTiLeLoi?line={1}&lenh={2}&maHang={3}&tiLeLoi={4}&ngayCaiDat={5}&id={6}",
                        _url, _selectedLine, _selectedLenh, _selectedMaHang, tiLeLoi, ngayStr, id);

                    string json = Task.Run(async () => await _client.GetAsnyc(url)).Result;
                    DataTable dtResult = JsonConvert.DeserializeObject<DataTable>(json);

                    if (dtResult != null && dtResult.Rows.Count > 0)
                    {
                        string resultCode = dtResult.Rows[0]["ResultCode"]?.ToString();
                        string message = dtResult.Rows[0]["Message"]?.ToString();

                        if (resultCode == "Success")
                            successCount++;
                        else
                        {
                            XtraMessageBox.Show($"Dòng {i + 1}: {message}", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            hasError = true;
                        }
                    }
                }
                catch (Exception)
                {
                    XtraMessageBox.Show($"Lỗi khi lưu dòng {i + 1}!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    hasError = true;
                }
            }

            if (successCount > 0 && !hasError)
            {
                clsWaitForm.ShowSuccessForm(this, 1000);
                LoadSettingTiLeLoi();
            }
            else if (successCount > 0)
            {
                LoadSettingTiLeLoi();
            }
        }

        private void barButtonDeleteRow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int[] selectedRows = gridView1.GetSelectedRows();

            if (selectedRows.Length == 0)
            {
                XtraMessageBox.Show("Vui lòng chọn ít nhất một dòng để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (XtraMessageBox.Show($"Bạn có chắc chắn muốn xóa {selectedRows.Length} dòng đã chọn?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            System.Collections.Generic.List<int> idsToDelete = new System.Collections.Generic.List<int>();

            foreach (int rowHandle in selectedRows)
            {
                var idValue = gridView1.GetRowCellValue(rowHandle, "ID");
                if (idValue != null && int.TryParse(idValue.ToString(), out int id))
                {
                    if (id > 0) 
                    {
                        idsToDelete.Add(id);
                    }
                }
            }

            if (idsToDelete.Count > 0)
            {
                try
                {
                    string listIds = string.Join(",", idsToDelete);

                    string url = string.Format("{0}SettingTiLeLoi/DeleteSettingTiLeLoi?id=&jsonIds={1}", _url, listIds);

                    string jsonResult = Task.Run(async () => await _client.GetAsnyc(url)).Result;
                    DataTable dtResult = JsonConvert.DeserializeObject<DataTable>(jsonResult);
                    if (dtResult != null && dtResult.Rows.Count > 0)
                    {
                        string resultCode = dtResult.Rows[0]["ResultCode"]?.ToString();
                        string message = dtResult.Rows[0]["Message"]?.ToString();

                        if (resultCode == "Success")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                        else
                        {
                            XtraMessageBox.Show(message, "Thông báo lỗi từ Server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; 
                        }
                    }
                }
                catch (Exception)
                {
                    XtraMessageBox.Show("Lỗi kết nối khi xóa dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            gridView1.DeleteSelectedRows();

            // LoadSettingTiLeLoi();
        }
    }
}