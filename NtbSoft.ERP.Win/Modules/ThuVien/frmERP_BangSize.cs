using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERP_BangSize : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        private string selectedFilePath = "";
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();
        public frmERP_BangSize()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            grvTheSize.FocusedRowChanged += GrvTheSize_FocusedRowChanged;

            grvTheSize.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
            grvTheSize.RowCountChanged += gridView_RowCountChanged;
            grvSize.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
            grvSize.RowCountChanged += gridView_RowCountChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            tenNVHienThi = GetTenNhanVien();
            LoadTheSize();
            CheckPerminsion();
        }
        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            KHDongThungLib.CustomDrawRowIndicator(sender, e);
        }

        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            KHDongThungLib.RowCountChanged(sender, e);
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd || !_allowEdit || !_allowDelete)
            {
                btnAddSize.Enabled = false;
                btnAddSize.Visible = true;

                barButtonItem3.Enabled = true;
                barButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                btnAddTheSize.Enabled = false;

                btnSaveTheSize.Enabled = false;
                btnSaveSize.Enabled = false;
                btnDeleteTheSize.Enabled = false;
                btnDeleteSize.Enabled = false;
                simpleButton2.Enabled = false;
                simpleButton1.Enabled = false;

            }
            //if (!_allowEdit)
            //{
            //    btnSaveSize.Enabled = false;
            //    btnSaveTheSize.Enabled = false;

            //}
            //else
            //{
            //    btnSaveSize.Enabled = true;
            //    btnSaveTheSize.Enabled = true;

            //}
            //if (!_allowDelete)
            //{
            //    btnDeleteSize.Enabled = false;
            //    btnDeleteTheSize.Enabled = false;

            //}


        }
        #region Thẻ size

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F1))
            {
                ThemTheSize();
                return true;
            }
            if (keyData == (Keys.F2))
            {
                ThemSize();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void LoadTheSize()
        {
            string url = string.Format("{0}", URL + $"ERPBangSize/Get?action=GetTheSize");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt != null)
            {
                dt.AcceptChanges(); // Để theo dõi RowState (Added, Modified)
            }
            int focusRowHandle = grvTheSize.FocusedRowHandle;
            grcTheSize.DataSource = dt.Rows.Count == 0 ? CreateTblTheSize() : dt;
            if (focusRowHandle == 0)
            {
                //Load();
                LoadSize();
            }
            else
            {
                grvTheSize.FocusedRowHandle = focusRowHandle;
                grvTheSize.ClearSelection();
                grvTheSize.SelectRow(focusRowHandle);
            }
        }
        private void SaveTheSize(DataTable dtSource)
        {
            grvTheSize.CloseEditor();
            grvTheSize.UpdateCurrentRow();
            if (dtSource == null || dtSource.Rows.Count == 0) return;
            var dtSave = CreateTblTheSize();
            foreach (DataRow dr in dtSource.Rows)
            {
                if (dr.RowState != DataRowState.Added && dr.RowState != DataRowState.Modified)
                    continue;
                var drNew = dtSave.NewRow();
                drNew["ID"] = dr["ID"].ToString() == "" ? 0 : dr["ID"];
                drNew["MaTheSize"] = NormalizeTheSize(dr["TheSize"].ToString().ToUpper());
                drNew["TheSize"] = dr["TheSize"].ToString().ToUpper();
                drNew["GhiChu"] = dr["GhiChu"];
                drNew["NguoiTao"] = dr["NguoiTao"];
                drNew["NgayTao"] = dr["NgayTao"];
                drNew["NguoiSua"] = dr["NguoiSua"];
                drNew["NgaySua"] = dr["NgaySua"];
                dtSave.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"ERPBangSize/Post?action=PostTheSize");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadTheSize();
            }
        }
        private void DeleteTheSize()
        {
            var dtDelete = CreateTblTheSize();
            int[] selectedRows = grvTheSize.GetSelectedRows();
            if (selectedRows.Length == 0) return;
            foreach (int rowHandle in selectedRows)
            {
                DataRow dr = grvTheSize.GetDataRow(rowHandle);
                var drNew = dtDelete.NewRow();
                drNew["ID"] = dr["ID"];
                dtDelete.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"ERPBangSize/Post?action=DeleteTheSize");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtDelete); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadTheSize();
            }

        }
        private void GrvTheSize_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadSize();
        }
        private void btnAddTheSize_Click(object sender, EventArgs e)
        {
            ThemTheSize();
        }
        private void ThemTheSize()
        {
            var dtSource = grcTheSize.DataSource as DataTable;
            var drNew = dtSource.NewRow();
            drNew["NguoiTao"] = GlobleData.UserName;
            drNew["NgayTao"] = DateTime.Now;
            dtSource.Rows.InsertAt(drNew, dtSource.Rows.Count);
        }
        private void btnSaveTheSize_Click(object sender, EventArgs e)
        {
            var dtSource = grcTheSize.DataSource as DataTable;
            SaveTheSize(dtSource);

        }

        private void btnDeleteTheSize_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa thẻ size này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                DeleteTheSize();
        }
        #endregion Thẻ size

        #region  Chi tiết size
        private void LoadSize()
        {
            var drFocus = grvTheSize.GetFocusedDataRow();
            if (drFocus == null)
            {
                grcSize.DataSource = CreateTblSize();
                return;
            }
            var _maTheSize = drFocus["MaTheSize"].ToString();
            string url = string.Format("{0}", URL + $"ERPBangSize/Get?action=GetSize&para1={_maTheSize}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt != null)
            {
                dt.AcceptChanges();
            }
            int focusRowHandle = grvSize.FocusedRowHandle;
            grcSize.DataSource = dt.Rows.Count == 0 ? CreateTblSize() : dt;
            grvSize.FocusedRowHandle = focusRowHandle;
            grvSize.ClearSelection();
            grvSize.SelectRow(focusRowHandle);

        }
        private void SaveSize(DataTable dtSource, bool checkImport = false)
        {
            if (dtSource.Rows.Count == 0) return;
            var drFocus = grvTheSize.GetFocusedDataRow();
            var _maTheSize = drFocus["MaTheSize"].ToString();
            var dtSave = CreateTblSize();
            int sort = 0;

            foreach (DataRow dr in dtSource.Rows)
            {
                //if (dr.RowState != DataRowState.Added && dr.RowState != DataRowState.Modified)
                //    continue;
                grvSize.CloseEditor();
                grvSize.UpdateCurrentRow();
                var mathesize = dr["MaTheSize"].ToString().ToUpper() == "" ? _maTheSize : dr["MaTheSize"].ToString().ToUpper();
                if (mathesize == "")
                {
                    MessageBox.Show("Vui lòng lưu thẻ size trước khi lưu size");
                    return;
                }
                var drNew = dtSave.NewRow();
                var masize = $"{ReplaceSpecialCharacterssize(dr["TenSize"].ToString().ToUpper())}";
                if (ReplaceSpecialCharacterssize(dr["TenSize"].ToString().ToUpper()) == "")
                    continue;
                drNew["ID"] = dr["ID"].ToString() == "" ? 0 : dr["ID"]; ;
                drNew["MaTheSize"] = mathesize;
                drNew["MaSize"] = $"SIZE_{masize}";
                drNew["TenSize"] = dr["TenSize"].ToString().ToUpper();
                drNew["Sort"] = checkImport ? Convert.ToInt32(dr["Sort"]) : sort;
                drNew["GhiChu"] = dr["GhiChu"];
                drNew["SizeSanXuat"] = ReplaceSpecialCharacterssize(dr["TenSize"].ToString().ToUpper());
                drNew["CodeSize"] = dr["CodeSize"];
                drNew["NguoiTao"] = dr["NguoiTao"];
                drNew["NgayTao"] = dr["NgayTao"];
                drNew["NguoiSua"] = dr["NguoiSua"];
                drNew["NgaySua"] = dr["NgaySua"];
                dtSave.Rows.Add(drNew);
                sort++;
            }
            string url = string.Format("{0}", URL + $"ERPBangSize/Post?action=PostSize");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadSize();
            }
        }
        private void DeleteSize()
        {
            var dtDelete = CreateTblSize();
            var drFocus = grvSize.GetFocusedDataRow();
            int[] selectedRows = grvSize.GetSelectedRows();
            if (drFocus == null) return;
            foreach (int rowHandle in selectedRows)
            {
                DataRow dr = grvSize.GetDataRow(rowHandle);
                var drNew = dtDelete.NewRow();
                drNew["ID"] = dr["ID"];
                dtDelete.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"ERPBangSize/Post?action=DeleteSize");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtDelete); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadSize();
            }


        }
        private void btnAddSize_Click(object sender, EventArgs e)
        {
            ThemSize();

        }
        private void ThemSize()
        {
            var drFocus = grvTheSize.GetFocusedDataRow();
            if (drFocus == null)
            {
                MessageBox.Show("Vui lòng chọn thẻ size");
                return;
            }
            if (drFocus["TheSize"].ToString() == "")
            {
                MessageBox.Show("Vui lòng nhập tên thẻ size");
                return;
            }

            var dtSource = grcSize.DataSource as DataTable;

            // Tìm giá trị Sort lớn nhất
            int maxSort = 0;
            if (dtSource.Rows.Count > 0)
            {
                foreach (DataRow row in dtSource.Rows)
                {
                    if (row["Sort"] != DBNull.Value && !string.IsNullOrEmpty(row["Sort"].ToString()))
                    {
                        int currentSort = Convert.ToInt32(row["Sort"]);
                        if (currentSort > maxSort)
                            maxSort = currentSort;
                    }
                }
            }

            // Tạo row mới với Sort = maxSort + 1
            var drNew = dtSource.NewRow();
            drNew["NguoiTao"] = GlobleData.UserName;
            drNew["NgayTao"] = DateTime.Now;
            drNew["Sort"] = maxSort + 1;
            dtSource.Rows.InsertAt(drNew, dtSource.Rows.Count);
        }
        private void btnSaveSize_Click(object sender, EventArgs e)
        {
            var dtSource = grcSize.DataSource as DataTable;
            SaveSize(dtSource);
        }

        private void btnDeleteSize_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa size này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                DeleteSize();
        }
        #endregion
        private DataTable CreateTblTheSize()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaTheSize", typeof(string));
            dt.Columns.Add("TheSize", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }


        private DataTable CreateTblSize()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaTheSize", typeof(string));
            dt.Columns.Add("MaSize", typeof(string));
            dt.Columns.Add("TenSize", typeof(string));
            dt.Columns.Add("Sort", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("SizeSanXuat", typeof(string));
            dt.Columns.Add("CodeSize", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }
        private string ReplaceSpecialCharacterssize(string input)
        {
            string pattern = @"[^\w\s\(\)]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);

            // Thay ký tự đặc biệt và khoảng trắng bằng "_"
            string result = regex.Replace(input, replacement).Replace(" ", replacement);

            // Nếu chuỗi gốc có dấu '+', thêm 1 dấu "_" ở cuối
            if (input.Contains("+"))
            {
                result += "_";
            }

            return result;
        }
        private string NormalizeTheSize(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // B1: Chuẩn hóa tiếng Việt
            string noVietnamese = ReplaceVietnameseChars(input);

            // B2: Thay ký tự đặc biệt + khoảng trắng bằng _
            string clean = ReplaceSpecialCharacterssize(noVietnamese);

            return clean;
        }

        private void grcSize_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            if (grvSize.FocusedColumn.FieldName == "TenSize")
            {
                var editor = grvSize.ActiveEditor as TextEdit;
                if (editor == null) return;

                string original = e.KeyChar.ToString();
                string replaced = ReplaceVietnameseChars(original);

                if (replaced != original)
                {
                    e.Handled = true;

                    int selectionStart = editor.SelectionStart;
                    string currentText = editor.Text;

                    // Xóa text đang được chọn (nếu có)
                    if (editor.SelectionLength > 0)
                    {
                        currentText = currentText.Remove(selectionStart, editor.SelectionLength);
                    }

                    // Chèn ký tự thay thế
                    editor.Text = currentText.Insert(selectionStart, replaced);

                    // Di chuyển con trỏ
                    editor.SelectionStart = selectionStart + replaced.Length;
                }
            }
        }

        private void grcTheSize_EditorKeyPress(object sender, KeyPressEventArgs e)
        {
            if (grvTheSize.FocusedColumn.FieldName == "TheSizeA")
            {
                var editor = grvTheSize.ActiveEditor as TextEdit;
                if (editor == null) return;

                string original = e.KeyChar.ToString();
                string replaced = ReplaceVietnameseChars(original);

                if (replaced != original)
                {
                    e.Handled = true;

                    int selectionStart = editor.SelectionStart;
                    string currentText = editor.Text;

                    // Xóa text đang được chọn (nếu có)
                    if (editor.SelectionLength > 0)
                    {
                        currentText = currentText.Remove(selectionStart, editor.SelectionLength);
                    }

                    // Chèn ký tự thay thế
                    editor.Text = currentText.Insert(selectionStart, replaced);

                    // Di chuyển con trỏ
                    editor.SelectionStart = selectionStart + replaced.Length;
                }
            }
        }


        private void MoveRowUp()
        {
            try
            {
                int currentRowHandle = grvSize.FocusedRowHandle;

                // Kiểm tra nếu đang ở hàng đầu tiên thì bỏ qua
                if (currentRowHandle <= 0)
                    return;

                var dtSource = grcSize.DataSource as DataTable;
                if (dtSource == null || dtSource.Rows.Count == 0)
                    return;

                // Lấy chỉ số hàng trên
                int previousRowHandle = currentRowHandle - 1;

                // Lấy giá trị Sort hiện tại
                var currentRow = grvSize.GetDataRow(currentRowHandle);
                var previousRow = grvSize.GetDataRow(previousRowHandle);

                if (currentRow == null || previousRow == null)
                    return;

                string currentTenSize = currentRow["TenSize"].ToString();

                // Hoán đổi giá trị Sort
                object tempSort = currentRow["Sort"];
                currentRow["Sort"] = previousRow["Sort"];
                previousRow["Sort"] = tempSort;

                // Sắp xếp lại DataTable theo Sort
                DataView dv = dtSource.DefaultView;
                dv.Sort = "Sort ASC";
                DataTable sortedDt = dv.ToTable();

                // Gán lại vào GridControl
                grcSize.DataSource = sortedDt;
                grvSize.RefreshData();

                // Focus lại vào row đã di chuyển (tìm theo ID hoặc TenSize)
                this.BeginInvoke(new Action(() =>
                {
                    for (int i = 0; i < grvSize.RowCount; i++)
                    {
                        var row = grvSize.GetDataRow(i);
                        if (row != null && row["TenSize"].ToString() == currentTenSize)
                        {
                            grvSize.FocusedRowHandle = i;
                            grvSize.SelectRow(i);
                            break;
                        }
                    }
                }));
                //// Tự động lưu sau khi di chuyển
                //SaveSize(sortedDt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi di chuyển lên: " + ex.Message);
            }
        }

        private void MoveRowDown()
        {
            try
            {
                int currentRowHandle = grvSize.FocusedRowHandle;
                var dtSource = grcSize.DataSource as DataTable;

                if (dtSource == null || dtSource.Rows.Count == 0)
                    return;

                // Kiểm tra nếu đang ở hàng cuối cùng thì bỏ qua
                if (currentRowHandle >= grvSize.RowCount - 1)
                    return;

                // Lấy chỉ số hàng dưới
                int nextRowHandle = currentRowHandle + 1;

                // Lấy giá trị Sort hiện tại
                var currentRow = grvSize.GetDataRow(currentRowHandle);
                var nextRow = grvSize.GetDataRow(nextRowHandle);

                if (currentRow == null || nextRow == null)
                    return;

                string currentTenSize = currentRow["TenSize"].ToString();

                // Hoán đổi giá trị Sort
                object tempSort = currentRow["Sort"];
                currentRow["Sort"] = nextRow["Sort"];
                nextRow["Sort"] = tempSort;

                // Sắp xếp lại DataTable theo Sort
                DataView dv = dtSource.DefaultView;
                dv.Sort = "Sort ASC";
                DataTable sortedDt = dv.ToTable();

                // Gán lại vào GridControl
                grcSize.DataSource = sortedDt;
                grvSize.RefreshData();

                // Focus lại vào row đã di chuyển (tìm theo ID hoặc TenSize)
                this.BeginInvoke(new Action(() =>
                {
                    for (int i = 0; i < grvSize.RowCount; i++)
                    {
                        var row = grvSize.GetDataRow(i);
                        if (row != null && row["TenSize"].ToString() == currentTenSize)
                        {
                            grvSize.FocusedRowHandle = i;
                            grvSize.SelectRow(i);
                            break;
                        }
                    }
                }));

                //// Tự động lưu sau khi di chuyển
                //SaveSize(sortedDt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi di chuyển xuống: " + ex.Message);
            }
        }

        private void grvTheSize_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            string valueTxt = e.Value?.ToString() ?? "";
            if (string.IsNullOrEmpty(valueTxt)) return;
            DataRow drow = grvTheSize.GetFocusedDataRow();
            DataTable tblTS = grcTheSize.DataSource as DataTable;
            if (tblTS == null) return;
            if (drow == null) return;
            if (grvTheSize.FocusedColumn.FieldName == "TheSize")
            {
                bool isExist = tblTS.AsEnumerable().Where(row => row != drow).Any(row =>
                  row["TheSize"].ToString() == valueTxt);

                if (isExist)
                {
                    e.ErrorText = $"Tên thẻ size {valueTxt} này đã tồn tại";
                    e.Valid = false;
                }
            }
        }

        private void grvSize_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            string valueTxt = e.Value?.ToString() ?? "";
            if (string.IsNullOrEmpty(valueTxt)) return;
            DataRow drow = grvSize.GetFocusedDataRow();
            DataTable tblS = grcSize.DataSource as DataTable;
            if (tblS == null) return;
            if (drow == null) return;
            if (grvSize.FocusedColumn.FieldName == "TenSize")
            {
                bool isExist = tblS.AsEnumerable().Where(row => row != drow).Any(row =>
                  row["TenSize"].ToString() == valueTxt);

                if (isExist)
                {
                    e.ErrorText = $"Tên size {valueTxt} này đã tồn tại";
                    e.Valid = false;
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            MoveRowDown();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            MoveRowUp();
        }
        public List<string> OpenExcelAndShowSheets()
        {
            var sheetNames = new List<string>();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All Files|*.*";
                openFileDialog.Title = "Chọn file Excel";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName; // LƯU ĐƯỜNG DẪN FILE

                    try
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        using (var package = new ExcelPackage(new FileInfo(selectedFilePath)))
                        {
                            foreach (var worksheet in package.Workbook.Worksheets)
                            {
                                sheetNames.Add(worksheet.Name);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show($"Lỗi khi đọc file Excel:\n{ex.Message}",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            return sheetNames;
        }

        private void btnImport_Size_Click(object sender, EventArgs e)
        {

        }

        private DataTable ReadExcelToDataTable(string filePath, string sheetName)
        {
            DataTable dt = new DataTable();

            // Tạo các cột cho DataTable
            dt.Columns.Add("TheSize", typeof(string));
            dt.Columns.Add("TenSize", typeof(string));
            dt.Columns.Add("CodeSize", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[sheetName];

                    if (worksheet == null)
                    {
                        XtraMessageBox.Show($"Không tìm thấy sheet '{sheetName}'", "Lỗi");
                        return dt;
                    }

                    int row = 2; // Bắt đầu từ row 2

                    while (true)
                    {
                        // Lấy giá trị từ cột A đến D
                        var colA = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? "";
                        var colB = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? "";
                        var colC = worksheet.Cells[row, 3].Value?.ToString()?.Trim() ?? "";
                        var colD = worksheet.Cells[row, 4].Value?.ToString()?.Trim() ?? "";

                        // Kiểm tra điều kiện dừng: cột A và D đều rỗng
                        if (string.IsNullOrEmpty(colA) && string.IsNullOrEmpty(colD))
                        {
                            break;
                        }

                        // Thêm dòng vào DataTable
                        DataRow dr = dt.NewRow();
                        dr["TheSize"] = colA;
                        dr["TenSize"] = colB;
                        dr["CodeSize"] = colC;
                        dr["GhiChu"] = colD;
                        dt.Rows.Add(dr);

                        row++;
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi đọc dữ liệu:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var sheetNames = OpenExcelAndShowSheets();

            if (sheetNames.Count > 0)
            {
                using (var frm = new frmERP_BangSizeImport(sheetNames))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        string selectedSheet = frm.SelectedSheet;
                        DataTable dt = ReadExcelToDataTable(selectedFilePath, selectedSheet);
                        var dtSource = grcTheSize.DataSource as DataTable;

                        var dtSave = CreateTblTheSize();

                        var dtTheSize = dt.AsEnumerable()
                            .Select(x => x["TheSize"].ToString().Trim().ToUpper())
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Distinct();
                        foreach (var currentSize in dtTheSize)
                        {
                            // Kiểm tra đã tồn tại trong dtSource chưa
                            bool isExist = dtSource.AsEnumerable()
                                .Any(r => r["TheSize"] != null
                                       && r["TheSize"].ToString().Trim().ToUpper() == currentSize);

                            if (!isExist)
                            {
                                // Tạo row cho dtSave
                                var drNewSave = dtSave.NewRow();
                                drNewSave["ID"] = 0;
                                drNewSave["MaTheSize"] = NormalizeTheSize(currentSize);
                                drNewSave["TheSize"] = currentSize;
                                drNewSave["GhiChu"] = "";
                                drNewSave["NguoiTao"] = GlobleData.UserName;
                                drNewSave["NgayTao"] = DateTime.Now;
                                drNewSave["NguoiSua"] = DBNull.Value;
                                drNewSave["NgaySua"] = DBNull.Value;
                                dtSave.Rows.Add(drNewSave);

                                // Tạo row mới cho dtSource (phải tạo riêng, không được add cùng 1 DataRow)
                                var drNewSource = dtSource.NewRow();
                                drNewSource["ID"] = 0;
                                drNewSource["MaTheSize"] = NormalizeTheSize(currentSize);
                                drNewSource["TheSize"] = currentSize;
                                drNewSource["GhiChu"] = "";
                                drNewSource["NguoiTao"] = GlobleData.UserName;
                                drNewSource["NgayTao"] = DateTime.Now;
                                drNewSource["NguoiSua"] = DBNull.Value;
                                drNewSource["NgaySua"] = DBNull.Value;
                                dtSource.Rows.Add(drNewSource);
                            }
                        }
                        // Nếu có dòng mới thì lưu
                        if (dtSave.Rows.Count > 0)
                        {
                            SaveTheSize(dtSave);
                            dtSource.AcceptChanges();
                        }
                        var dtSourceSize = grcSize.DataSource as DataTable;
                        var dtSaveSize = CreateTblSize();

                        foreach (var currentSize in dtTheSize)
                        {
                            // Lọc những dòng có TheSize hiện tại
                            var dtSize = dt.AsEnumerable()
                                .Where(x => x["TheSize"].ToString().Trim().ToUpper() == currentSize)
                                .CopyToDataTable();

                            // 👉 Lấy bảng Size hiện có từ API 1 lần cho mỗi MaTheSize
                            string url = string.Format("{0}", URL + $"ERPBangSize/Get?action=GetSize&para1={NormalizeTheSize(currentSize)}");
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                            DataTable dtSizeTable = JsonConvert.DeserializeObject<DataTable>(json);

                            // 👉 Lấy max sort hiện có cho MaTheSize này
                            int maxSort = 0;
                            if (dtSizeTable.Rows.Count > 0 && dtSizeTable.Columns.Contains("Sort"))
                            {
                                maxSort = dtSizeTable.AsEnumerable()
                                    .Where(r => r["Sort"] != DBNull.Value)
                                    .Select(r => Convert.ToInt32(r["Sort"]))
                                    .DefaultIfEmpty(0)
                                    .Max();
                            }

                            // 👉 Dùng biến sortCurrent để tăng dần trong nhóm
                            int sortCurrent = maxSort;

                            foreach (DataRow item in dtSize.Rows)
                            {
                                string tenSize = item["TenSize"].ToString().Trim().ToUpper();
                                string maSize = $"{ReplaceSpecialCharacterssize(tenSize)}";

                                // Kiểm tra tên size đã tồn tại trong dtSizeTable hay chưa
                                bool isExist = dtSizeTable.AsEnumerable().Any(r =>
                                    r["TenSize"] != null &&
                                    r["TenSize"].ToString().Trim().ToUpper() == tenSize
                                );

                                if (!isExist)
                                {
                                    sortCurrent++; // 👉 Mỗi lần thêm mới thì tăng sort lên

                                    var drNewSize = dtSaveSize.NewRow();
                                    drNewSize["ID"] = 0;
                                    drNewSize["MaTheSize"] = NormalizeTheSize(currentSize);
                                    drNewSize["MaSize"] = maSize;
                                    drNewSize["TenSize"] = tenSize;
                                    drNewSize["Sort"] = sortCurrent;
                                    drNewSize["GhiChu"] = item["GhiChu"];
                                    drNewSize["SizeSanXuat"] = ReplaceSpecialCharacterssize(tenSize);
                                    drNewSize["CodeSize"] = item["CodeSize"];

                                    dtSaveSize.Rows.Add(drNewSize);
                                }
                            }
                        }
                        //Nếu có dòng mới thì lưu
                        if (dtSaveSize.Rows.Count > 0)
                        {
                            SaveSize(dtSaveSize, true);
                            LoadSize();
                        }
                    }

                }
            }
            else
            {
                XtraMessageBox.Show("Không có sheet nào được chọn hoặc file rỗng.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadTheSize();
        }


        private static readonly Dictionary<char, string> vietCharMap = new Dictionary<char, string>
        {
            // a
            { 'á', "a" }, { 'à', "a" }, { 'ả', "a" }, { 'ã', "a" }, { 'ạ', "a" },
            { 'ă', "aa" }, { 'ắ', "aa" }, { 'ằ', "aa" }, { 'ẳ', "aa" }, { 'ẵ', "aa" }, { 'ặ', "aa" },
            { 'â', "aa" }, { 'ấ', "aa" }, { 'ầ', "aa" }, { 'ẩ', "aa" }, { 'ẫ', "aa" }, { 'ậ', "aa" },

            // e
            { 'é', "e" }, { 'è', "e" }, { 'ẻ', "e" }, { 'ẽ', "e" }, { 'ẹ', "e" },
            { 'ê', "ee" }, { 'ế', "ee" }, { 'ề', "ee" }, { 'ể', "ee" }, { 'ễ', "ee" }, { 'ệ', "ee" },

            // i
            { 'í', "i" }, { 'ì', "i" }, { 'ỉ', "i" }, { 'ĩ', "i" }, { 'ị', "i" },

            // o
            { 'ó', "o" }, { 'ò', "o" }, { 'ỏ', "o" }, { 'õ', "o" }, { 'ọ', "o" },
            { 'ô', "oo" }, { 'ố', "oo" }, { 'ồ', "oo" }, { 'ổ', "oo" }, { 'ỗ', "oo" }, { 'ộ', "oo" },
            { 'ơ', "oo" }, { 'ớ', "oo" }, { 'ờ', "oo" }, { 'ở', "oo" }, { 'ỡ', "oo" }, { 'ợ', "oo" },

            // u
            { 'ú', "u" }, { 'ù', "u" }, { 'ủ', "u" }, { 'ũ', "u" }, { 'ụ', "u" },
            { 'ư', "uu" }, { 'ứ', "uu" }, { 'ừ', "uu" }, { 'ử', "uu" }, { 'ữ', "uu" }, { 'ự', "uu" },

            // y
            { 'ý', "y" }, { 'ỳ', "y" }, { 'ỷ', "y" }, { 'ỹ', "y" }, { 'ỵ', "y" },

            // đ
            { 'đ', "dd" },

            // --- HOA ---
            { 'Á', "A" }, { 'À', "A" }, { 'Ả', "A" }, { 'Ã', "A" }, { 'Ạ', "A" },
            { 'Ă', "AA" }, { 'Ắ', "AA" }, { 'Ằ', "AA" }, { 'Ẳ', "AA" }, { 'Ẵ', "AA" }, { 'Ặ', "AA" },
            { 'Â', "AA" }, { 'Ấ', "AA" }, { 'Ầ', "AA" }, { 'Ẩ', "AA" }, { 'Ẫ', "AA" }, { 'Ậ', "AA" },

            { 'É', "E" }, { 'È', "E" }, { 'Ẻ', "E" }, { 'Ẽ', "E" }, { 'Ẹ', "E" },
            { 'Ê', "EE" }, { 'Ế', "EE" }, { 'Ề', "EE" }, { 'Ể', "EE" }, { 'Ễ', "EE" }, { 'Ệ', "EE" },

            { 'Í', "I" }, { 'Ì', "I" }, { 'Ỉ', "I" }, { 'Ĩ', "I" }, { 'Ị', "I" },

            { 'Ó', "O" }, { 'Ò', "O" }, { 'Ỏ', "O" }, { 'Õ', "O" }, { 'Ọ', "O" },
            { 'Ô', "OO" }, { 'Ố', "OO" }, { 'Ồ', "OO" }, { 'Ổ', "OO" }, { 'Ỗ', "OO" }, { 'Ộ', "OO" },
            { 'Ơ', "OO" }, { 'Ớ', "OO" }, { 'Ờ', "OO" }, { 'Ở', "OO" }, { 'Ỡ', "OO" }, { 'Ợ', "OO" },

            { 'Ú', "U" }, { 'Ù', "U" }, { 'Ủ', "U" }, { 'Ũ', "U" }, { 'Ụ', "U" },
            { 'Ư', "UU" }, { 'Ứ', "UU" }, { 'Ừ', "UU" }, { 'Ử', "UU" }, { 'Ữ', "UU" }, { 'Ự', "UU" },

            { 'Ý', "Y" }, { 'Ỳ', "Y" }, { 'Ỷ', "Y" }, { 'Ỹ', "Y" }, { 'Ỵ', "Y" },

            { 'Đ', "DD" }
        };
        #region Thoai
        private void grvTheSize_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colNguoiTaoTSize || e.Column == colNguoiSuaTSize)
            {
                string username = e.Value as string;
                if (!string.IsNullOrEmpty(username) && tenNVHienThi != null)
                {
                    string upperUser = username.ToUpper();
                    if (tenNVHienThi.ContainsKey(upperUser))
                    {
                        e.DisplayText = tenNVHienThi[upperUser];
                    }
                }
            }
        }

        private void grvSize_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colNguoiTaoSize || e.Column == colNguoiSuaSize)
            {
                string username = e.Value as string;
                if (!string.IsNullOrEmpty(username) && tenNVHienThi != null)
                {
                    string upperUser = username.ToUpper();
                    if (tenNVHienThi.ContainsKey(upperUser))
                    {
                        e.DisplayText = tenNVHienThi[upperUser];
                    }
                }
            }
        }

        private void CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null || e.RowHandle < 0 || view.IsNewItemRow(e.RowHandle)) return;

                DataRow row = view.GetDataRow(e.RowHandle);
                if (row == null) return;

                if (row["ID"] != DBNull.Value && Convert.ToInt32(row["ID"]) > 0)
                {
                    row["NguoiSua"] = GlobleData.UserName;
                    row["NgaySua"] = DateTime.Now;
                }
                else if (string.IsNullOrEmpty(row["NguoiTao"]?.ToString()))
                {
                    row["NguoiTao"] = GlobleData.UserName;
                    row["NgayTao"] = DateTime.Now;
                }
            }
            catch
            {
                clsWaitForm.ShowErrorForm(this, 1500);
            }
        }
        #endregion
        private string ReplaceVietnameseChars(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var sb = new StringBuilder();
            foreach (char ch in input)
            {
                if (vietCharMap.TryGetValue(ch, out string replacement))
                {
                    sb.Append(replacement);
                }
                else
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }
        #region Thoai
        private Dictionary<string, string> GetTenNhanVien()
        {
            try
            {
                string url = string.Format("{0}?", URL + "GetTenNV/GetTenNV");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    DataTable dtNhanVien = JsonConvert.DeserializeObject<DataTable>(json);
                    string userNameCol = "UserName";
                    string tenNVCol = "TenNV";
                    return dtNhanVien.AsEnumerable()
                        .Where(row => row[userNameCol] != DBNull.Value && row[userNameCol] != null && !string.IsNullOrWhiteSpace(row.Field<string>(userNameCol))) // Lọc row null/rỗng
                        .GroupBy(row => row.Field<string>(userNameCol).Trim().ToUpper())
                        .ToDictionary(
                            g => g.Key,
                            g => g.First().Field<string>(tenNVCol) ?? g.Key
                        );
                }

            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
            }
            return new Dictionary<string, string>();
        }
        #endregion
    }
}
