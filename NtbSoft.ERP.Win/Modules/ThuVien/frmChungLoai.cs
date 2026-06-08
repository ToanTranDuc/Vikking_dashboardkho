using DevExpress.DataAccess.Excel;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
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
    public partial class frmChungLoai : DevExpress.XtraEditors.XtraForm
    {
        //Thoai thêm biến lấy tên nv
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        public frmChungLoai()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            grvNhomCL.FocusedRowChanged += GrvNhomCL_FocusedRowChanged;
            grvChungLoai.FocusedRowChanged += GrvChungLoai_FocusedRowChanged;

            grvNhomCL.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
            grvNhomCL.RowCountChanged += gridView_RowCountChanged;
            grvChungLoai.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
            grvChungLoai.RowCountChanged += gridView_RowCountChanged;

            grvCongDoan.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
            grvCongDoan.RowCountChanged += gridView_RowCountChanged;
        }
        protected override void OnLoad(EventArgs e)
        {
            tenNVHienThi = GetTenNhanVien(); //khởi tạo biến lấy ds tên nhân viên
            LoadNhomCL();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F1))
            {
                btnAddNhomCL.PerformClick();
                return true;
            }
            if (keyData == (Keys.F2))
            {
                btnAddCL.PerformClick();
                return true;
            }
            if (keyData == (Keys.F3))
            {
                btnAddCongDoan.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            KHDongThungLib.CustomDrawRowIndicator(sender, e);
        }

        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            KHDongThungLib.RowCountChanged(sender, e);
        }
        #region Nhóm Chủng Loại
        private void LoadNhomCL()
        {
            string url = string.Format("{0}", URL + $"ChungLoai_CongDoan/GetChungLoai?action=GetNhomCL");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            #region Thoai
            if (dt != null)
            {
                dt.AcceptChanges(); // Để theo dõi RowState (Added, Modified)
            }
            #endregion
            int focusRowHandle = grvNhomCL.FocusedRowHandle;
            grcNhomCL.DataSource = dt.Rows.Count == 0 ? CreateTblNhomCL() : dt;
            if (focusRowHandle == 0)
            {
                LoadChungLoai();
            }
            else
            {
                grvNhomCL.FocusedRowHandle = focusRowHandle;
            }
        }
        private void SaveNhomCL(DataTable dtSource, bool showSuccessForm = true)
        {
            grvNhomCL.CloseEditor();
            grvNhomCL.UpdateCurrentRow();
            if (dtSource == null || dtSource.Rows.Count == 0) return;
            var dtSave = CreateTblNhomCL();
            foreach (DataRow dr in dtSource.Rows)
            {
                if (dr.RowState != DataRowState.Added && dr.RowState != DataRowState.Modified)
                    continue;
                if (dr["TenNhomCL"].ToString() == "") continue;
                var drNew = dtSave.NewRow();               
                drNew["ID"] = 0;
                drNew["MaNhomCL"] = dr["MaNhomCL"];
                drNew["TenNhomCL"] = dr["TenNhomCL"];
                drNew["GhiChu"] = dr["GhiChu"];
                drNew["NguoiTao"] = dr["NguoiTao"];
                drNew["NgayTao"] = dr["NgayTao"];
                drNew["NguoiSua"] = dr["NguoiSua"];
                drNew["NgaySua"] = dr["NgaySua"];
                dtSave.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"ChungLoai_CongDoan/PostChungLoai?action=PostNhomCL");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                if (showSuccessForm)
                    clsWaitForm.ShowSuccessForm(this, 2000);
                LoadNhomCL();
            }
        }
        private void DeleteNhomCL()
        {
            var drFocus = grvNhomCL.GetFocusedDataRow();
            if (drFocus == null) return;
            var IDDelete = drFocus["ID"];
            string url = string.Format("{0}", URL + $"ChungLoai_CongDoan/DeleteChungLoai?action=DeleteNhomCL&id={IDDelete}");
            var msResult = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadNhomCL();
            }
        }
        private void GrvNhomCL_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadChungLoai();
        }
        private void btnAddNhomCL_Click(object sender, EventArgs e)
        {
            var dtSource = grcNhomCL.DataSource as DataTable;
            var drNew = dtSource.NewRow();
            drNew["NguoiTao"] = GlobleData.UserName;
            drNew["NgayTao"] = DateTime.Now;
            dtSource.Rows.InsertAt(drNew, dtSource.Rows.Count);
        }
        private void btnSaveNhomCL_Click(object sender, EventArgs e)
        {
            var dtSource = grcNhomCL.DataSource as DataTable;
            SaveNhomCL(dtSource);
        }
        private void btnDeleteNhomCL_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa nhóm chủng loại này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                DeleteNhomCL();
        }
        #endregion
        #region Chủng Loại
        private void LoadChungLoai()
        {
            var drFocus = grvNhomCL.GetFocusedDataRow();
            if (drFocus == null)
            {
                grcChungLoai.DataSource = CreateTblChungLoai();
                return;
            }
            var nhomCL = drFocus["MaNhomCL"].ToString();
            string url = string.Format("{0}", URL + $"ChungLoai_CongDoan/GetChungLoai?action=GetChungLoai&para1={nhomCL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            #region Thoai
            if (dt != null)
            {
                dt.AcceptChanges();
            }
            #endregion
            int focusRowHandle = grvChungLoai.FocusedRowHandle;
            grcChungLoai.DataSource = dt.Rows.Count == 0 ? CreateTblChungLoai() : dt;
            if (focusRowHandle == 0)
            {
                LoadCongDoan();
            }
            else
            {
                grvChungLoai.FocusedRowHandle = focusRowHandle;
            }
            CreateSearchlookup();
        }
        private void SaveChungLoai(DataTable dtSource, bool showSuccessForm = true)
        {
            if (dtSource.Rows.Count == 0) return;
            var dtSave = CreateTblChungLoai();
            var drFocus = grvNhomCL.GetFocusedDataRow();
            var _maNhomCL = drFocus["MaNhomCL"].ToString();
            foreach (DataRow dr in dtSource.Rows)
            {
                if (dr.RowState != DataRowState.Added && dr.RowState != DataRowState.Modified)
                    continue; // Kiểm tra Add/Update tránh cập nhật hàng loạt
                if (dr["TenCL"].ToString() == "") continue;
                var drNew = dtSave.NewRow();
                drNew["ID"] = 0;
                drNew["MaCL"] = dr["MaCL"];
                drNew["TenCL"] = dr["TenCL"];
                drNew["MaNhomCL"] = _maNhomCL;
                drNew["DonViTinh"] = dr["DonViTinh"];
                drNew["GhiChu"] = dr["GhiChu"];
                drNew["NguoiTao"] = dr["NguoiTao"];
                drNew["NgayTao"] = dr["NgayTao"];
                drNew["NguoiSua"] = dr["NguoiSua"];
                drNew["NgaySua"] = dr["NgaySua"];
                dtSave.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"ChungLoai_CongDoan/PostChungLoai?action=PostChungLoai");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                if (showSuccessForm)
                    clsWaitForm.ShowSuccessForm(this, 2000);
                LoadChungLoai();
            }
        }
        private void DeleteChungLoai()
        {
            var drFocus = grvChungLoai.GetFocusedDataRow();
            if (drFocus == null) return;
            var IDDelete = drFocus["ID"];
            string url = string.Format("{0}", URL + $"ChungLoai_CongDoan/DeleteChungLoai?action=DeleteChungLoai&id={IDDelete}");
            var msResult = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadChungLoai();
            }
        }
        private void GrvChungLoai_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadCongDoan();
        }
        private void btnAddCL_Click(object sender, EventArgs e)
        {
            var drFocus = grvNhomCL.GetFocusedDataRow();
            if (drFocus == null)
            {
                MessageBox.Show("Vui lòng chọn nhóm chủng loại");
                return;
            }
            /*else if (drFocus["MaNhomCL"].ToString() == "")
            {
                var dtSourceThe = grcNhomCL.DataSource as DataTable;
                SaveNhomCL(dtSourceThe, false);
            }*/
            #region Thoai check có nhóm CL trước khi thêm chi tiết CL
            string nhomCLoai = drFocus["TenNhomCL"].ToString().Trim();
            string maNhomCL = drFocus["MaNhomCL"].ToString().Trim();
            if (string.IsNullOrEmpty(nhomCLoai))
            {
                MessageBox.Show("Vui lòng nhập nhóm chủng loại trước khi thêm chủng loại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                grvNhomCL.FocusedColumn = grvNhomCL.Columns["TheMau"];
                grvNhomCL.ShowEditor();
                return;
            }
            if (string.IsNullOrEmpty(maNhomCL))
            {
                var dtSourceThe = grcChungLoai.DataSource as DataTable;
                SaveNhomCL(dtSourceThe, false);
                drFocus = grvNhomCL.GetFocusedDataRow();
                if (drFocus == null || string.IsNullOrEmpty(drFocus["MaTheMau"].ToString()))
                {
                    MessageBox.Show("Lưu nhóm CL không thành công. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            #endregion
            var dtSource = grcChungLoai.DataSource as DataTable;
            var drNew = dtSource.NewRow();
            drNew["NguoiTao"] = GlobleData.UserName;
            drNew["NgayTao"] = DateTime.Now;
            dtSource.Rows.InsertAt(drNew, dtSource.Rows.Count);
        }
        private void btnSaveCL_Click(object sender, EventArgs e)
        {
            var dtSource = grcChungLoai.DataSource as DataTable;
            SaveChungLoai(dtSource);
        }
        private void btnDeleteCL_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa chủng loại này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                DeleteChungLoai();
        }
        #endregion
        #region Công đoạn
        private void LoadCongDoan()
        {
            var drFocus = grvChungLoai.GetFocusedDataRow();
            if (drFocus == null || drFocus["MaCL"] == null)
            {
                grcCongDoan.DataSource = CreateTblCongDoan();
                return;
            }
            var nhomCL = drFocus["MaCL"].ToString();
            string url = string.Format("{0}", URL + $"ChungLoai_CongDoan/GetCongDoan?action=GetCongDoan&para1={nhomCL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            #region Thoai
            if (dt != null)
            {
                dt.AcceptChanges();
            }
            #endregion
            grcCongDoan.DataSource = (dt == null || dt.Rows.Count == 0) ? CreateTblCongDoan() : dt;
        }
        private void SaveCongDoan(DataTable dtSource)
        {
            if (dtSource.Rows.Count == 0) return;
            var dtSave = CreateTblCongDoan();
            var drFocus = grvChungLoai.GetFocusedDataRow();
            var _maCL = drFocus["MaCL"].ToString();
            int sort = 1;
            foreach (DataRow dr in dtSource.Rows)
            {
                //if (dr.RowState != DataRowState.Added && dr.RowState != DataRowState.Modified)
                //    continue;
                var drNew = dtSave.NewRow();
                drNew["ID"] = 0;
                drNew["MaCongDoan"] = dr["MaCongDoan"];
                drNew["TenCongDoan"] = dr["TenCongDoan"];
                drNew["ChungLoai"] = _maCL;
                drNew["Sort"] = sort;
                drNew["NguoiTao"] = dr["NguoiTao"];
                drNew["NgayTao"] = dr["NgayTao"];
                drNew["NguoiSua"] = dr["NguoiSua"];
                drNew["NgaySua"] = dr["NgaySua"];
                dtSave.Rows.Add(drNew);
                sort++;
            }
            string url = string.Format("{0}", URL + $"ChungLoai_CongDoan/PostCongDoan?action=InsertOrUpdate");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadCongDoan();
            }
        }
        private void DeleteCongDoan()
        {
            var drFocus = grvCongDoan.GetFocusedDataRow();
            if (drFocus == null) return;
            var IDDelete = drFocus["ID"];
            string url = string.Format("{0}", URL + $"ChungLoai_CongDoan/DeleteCongDoan?action=DeleteCongDoan&id={IDDelete}");
            var msResult = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadCongDoan();
            }
        }
        private void btnAddCongDoan_Click(object sender, EventArgs e)
        {
            var drFocus = grvChungLoai.GetFocusedDataRow();
            if (drFocus == null)
            {
                MessageBox.Show("Vui lòng chọn chủng loại");
                return;
            }
            /*else if (drFocus["MaCL"].ToString() == "")
            {
                var dtSourceThe = grcChungLoai.DataSource as DataTable;
                SaveChungLoai(dtSourceThe, false);
            }*/
            #region Thoai check có chủng loại trước khi thêm công đoạn
            string chungLoai = drFocus["TenCL"].ToString().Trim();
            string maCL = drFocus["MaCL"].ToString().Trim();
            if (string.IsNullOrEmpty(chungLoai))
            {
                MessageBox.Show("Vui lòng nhập chủng loại trước khi thêm công đoạn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                grvChungLoai.FocusedColumn = grvChungLoai.Columns["TheMau"];
                grvChungLoai.ShowEditor();
                return;
            }
            if (string.IsNullOrEmpty(maCL))
            {
                var dtSourceThe = grcChungLoai.DataSource as DataTable;
                SaveChungLoai(dtSourceThe, false);
                drFocus = grvChungLoai.GetFocusedDataRow();
                if (drFocus == null || string.IsNullOrEmpty(drFocus["MaTheMau"].ToString()))
                {
                    MessageBox.Show("Lưu chủng loại không thành công. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            #endregion
            drFocus = grvChungLoai.GetFocusedDataRow();
            // Tính toán mã công đoạn
            string maCongDoan = "";
            int Sort = 1;
            var _maCL = drFocus["MaCL"].ToString().Split('_')[1];
            var dtSource = grcCongDoan.DataSource as DataTable;
            if (dtSource.Rows.Count > 0)
            {
                var lastCD = dtSource.Rows[dtSource.Rows.Count - 1]["MaCongDoan"].ToString().Split('.');
                maCongDoan = _maCL + "." + (Convert.ToInt16(lastCD[1]) + 1).ToString();
                Sort = Convert.ToInt16(lastCD[1]) + 1;
            }
            else
            {
                maCongDoan = _maCL + ".1";
                Sort = 1;
            }
            //////////////////////////
            var drNew = dtSource.NewRow();
            drNew["MaCongDoan"] = maCongDoan;
            drNew["NguoiTao"] = GlobleData.UserName;
            drNew["NgayTao"] = DateTime.Now;
            drNew["Sort"] = Sort;
            dtSource.Rows.InsertAt(drNew, dtSource.Rows.Count);
        }
        private void btnSaveCongDoan_Click(object sender, EventArgs e)
        {
            var dtSource = grcCongDoan.DataSource as DataTable;
            SaveCongDoan(dtSource);
        }
        private void btnDeleteCongDoan_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa công đoạn này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                DeleteCongDoan();
        }
        private void ImportExcel_CD_Click(object sender, EventArgs e)
        {
            OpenFileDialog Sfd = new OpenFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (*.xls;*.xlsx)|*.xls;*.xlsx";
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                string FilePath = Sfd.FileName;
                string worksheetName = string.Empty;
                using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(FilePath))
                {
                    DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                    worksheetName = worksheetCollection[0].Name;
                }
                var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                source.FileName = FilePath;
                var worksheetSettings = new ExcelWorksheetSettings(worksheetName);
                source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                source.Fill();
                DataTable dtSource = new DataTable();
                dtSource = source.ToDataTable();
                //ProcessExcel(dtSource);
            }
        }
        #endregion
        private DataTable CreateTblNhomCL()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaNhomCL", typeof(string));
            dt.Columns.Add("TenNhomCL", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }
        private DataTable CreateTblChungLoai()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaCL", typeof(string));
            dt.Columns.Add("TenCL", typeof(string));
            dt.Columns.Add("MaNhomCL", typeof(string));
            dt.Columns.Add("DonViTinh", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }
        private DataTable CreateTblCongDoan()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaCongDoan", typeof(string));
            dt.Columns.Add("TenCongDoan", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("ChungLoai", typeof(string));
            dt.Columns.Add("Sort", typeof(int));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }

        private void grcCongDoan_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                CopyPasteFor1Col(grvCongDoan, 0, "TenCongDoan");
            }
        }

        public void CopyPasteFor1Col(dynamic bandedGridView1, int totalRow, string _fieldName)
        {
            bandedGridView1.CloseEditor();
            bandedGridView1.UpdateCurrentRow();
            var columnFocus = bandedGridView1.FocusedColumn.FieldName;

            GridView view = bandedGridView1 as GridView;
            string clipboardData = Clipboard.GetText();
            byte[] bytes = Encoding.UTF8.GetBytes(clipboardData);
            string decodedClipboardData = Encoding.UTF8.GetString(bytes);
            string[] data = decodedClipboardData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length == 0) return;
            var rowHandle = bandedGridView1.FocusedRowHandle;
            foreach (var item in data)
            {
                if (rowHandle >= bandedGridView1.RowCount)
                {
                    bandedGridView1.AddNewRow();
                    bandedGridView1.UpdateCurrentRow();
                    //rowHandle = bandedGridView1.FocusedRowHandle;
                }

                var value = item.Split('\t')[0];
                bandedGridView1.SetRowCellValue(rowHandle, _fieldName, value);
                var drFocus = grvChungLoai.GetFocusedDataRow();
                string maCongDoan = "";
                var _maCL = drFocus["MaCL"].ToString().Split('_')[1];
                var dtSource = grcCongDoan.DataSource as DataTable;
                if (dtSource.Rows.Count != 1)
                {
                    if (dtSource.Rows.Count > 0)
                    {
                        var lastCD = dtSource.Rows[dtSource.Rows.Count - 2]["MaCongDoan"].ToString().Split('.');
                        maCongDoan = _maCL + "." + (Convert.ToInt16(lastCD[1]) + 1).ToString();
                    }
                    else
                    {
                        maCongDoan = _maCL + ".1";
                    }
                    bandedGridView1.SetRowCellValue(rowHandle, "MaCongDoan", maCongDoan);
                }



                rowHandle++;
                //if (rowHandle > totalRow - 1) return;
            }
        }

        private void btnImport_CL_Click(object sender, EventArgs e)
        {
            string thuVien = "ChungLoai";
            frmImPortExcelThuVien f = new frmImPortExcelThuVien(thuVien);
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog();
            LoadChungLoai();
        }


        private void CreateSearchlookup()
        {
            string url = string.Format("{0}?", URL + "DonViChungLoai/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tbl;
            rCountryEdit.DisplayMember = "TenDVCL";
            rCountryEdit.ValueMember = "MaDVCL";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";

            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaDVCL", Caption = "Mã Đơn Vị Chủng Loại", Name = "colMaDVCL", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenDVCL", Caption = "Tên Đơn Vị Chủng Loại", Name = "colTenDVCL", Visible = true });

            }
            gridColumn4.ColumnEdit = rCountryEdit;
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadNhomCL();
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

        private void grvNhomCL_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colNguoiTaoNhomCL || e.Column == colNguoiSuaNhomCL)
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

        private void grvChungLoai_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colNguoiTaoCL || e.Column == colNguoiSuaCL)
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

        private void grvCongDoan_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colNguoiTaoCD || e.Column == colNguoiSuaCD)
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
        #endregion

        private void btnUp_Click(object sender, EventArgs e)
        {
            var dtSource = grcCongDoan.DataSource as DataTable;
            DataRow drFocus = grvCongDoan.GetFocusedDataRow();
            if (dtSource == null || drFocus == null) return;
            var index = dtSource.Rows.IndexOf(drFocus);
            if (index <= 0) return;
            DataRow newRow = dtSource.NewRow();
            newRow.ItemArray = drFocus.ItemArray.Clone() as object[];
            dtSource.Rows.RemoveAt(index);
            newRow["Sort"] = index;
            dtSource.Rows.InsertAt(newRow, index - 1);
            grvCongDoan.FocusedRowHandle = index - 1;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var dtSource = grcCongDoan.DataSource as DataTable;
            DataRow drFocus = grvCongDoan.GetFocusedDataRow();
            if (dtSource == null || drFocus == null) return;
            var index = dtSource.Rows.IndexOf(drFocus);
            if (index == dtSource.Rows.Count - 1) return;
            DataRow newRow = dtSource.NewRow();
            newRow.ItemArray = drFocus.ItemArray.Clone() as object[];
            dtSource.Rows.RemoveAt(index);
            newRow["Sort"] = index + 2;
            dtSource.Rows.InsertAt(newRow, index + 1);
            grvCongDoan.FocusedRowHandle = index + 1;
        }

        private void grvCongDoan_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DataRow drFocus = grvChungLoai.GetFocusedDataRow();
            if (drFocus == null || e.Menu == null) return;
            DXMenuItem menuinsertAt = new DXMenuItem();
            menuinsertAt.Caption = "Tạo dòng mới phía dưới";
            menuinsertAt.Click += MenuinsertAt_Click;
            e.Menu.Items.Add(menuinsertAt);
        }

        private void MenuinsertAt_Click(object sender, EventArgs e)
        {
            try
            {
                var drFocusCL = grvChungLoai.GetFocusedDataRow();
                var _maCL = drFocusCL["MaCL"].ToString().Split('_')[1];
                var dtSource = grcCongDoan.DataSource as DataTable;
                var congdoanMax = dtSource.AsEnumerable().Max(x => Convert.ToInt32(x["MaCongDoan"].ToString().Split('.')[1])) + 1;
                var drfocusCD = grvCongDoan.GetFocusedDataRow();

                var index = dtSource.Rows.IndexOf(drfocusCD);
                var drNew = dtSource.NewRow();
                drNew["MaCongDoan"] = _maCL + "." + congdoanMax;
                drNew["NguoiTao"] = GlobleData.UserName;
                drNew["NgayTao"] = DateTime.Now;
                drNew["Sort"] = congdoanMax;
                dtSource.Rows.InsertAt(drNew, index + 1);
            }
            catch(Exception ex) { }
           
        }
    }
}
