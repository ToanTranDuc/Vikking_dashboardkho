using DevExpress.DataAccess.Excel;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERP_BangInSeam : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        //Thoai thêm biến lấy tên nv
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();
        public frmERP_BangInSeam()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            grvTheInSeam.FocusedRowChanged += GrvTheInSeam_FocusedRowChanged;
            grvTheInSeam.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
            grvTheInSeam.RowCountChanged += gridView_RowCountChanged;
            grvInSeam.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
            grvInSeam.RowCountChanged += gridView_RowCountChanged;

            grvInSeam.OptionsSelection.MultiSelect = true;
            grvInSeam.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F1))
            {
                btnAddTheInSeam.PerformClick();
                return true;
            }
            if (keyData == (Keys.F2))
            {
                btnAddInSeam.PerformClick();
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

        protected override void OnLoad(EventArgs e)
        {
            tenNVHienThi = GetTenNhanVien(); //khởi tạo lấy ds tên nv
            LoadTheInSeam();
            CheckPerminsion();
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
                btnAddInSeam.Enabled = false;
                btnAddInSeam.Visible = true;

                btnImport_InSeam.Enabled = true;
                btnImport_InSeam.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                btnAddTheInSeam.Enabled = false;

                btnSaveTheInSeam.Enabled = false;
                btnSaveInSeam.Enabled = false;
                btnDeleteTheInSeam.Enabled = false;
                btnDeleteInSeam.Enabled = false;

            }

        }
        #region Thẻ inseam
        private void LoadTheInSeam()
        {
            string url = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetTheInSeam");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt != null)
            {
                dt.AcceptChanges(); // Để theo dõi RowState (Added, Modified)
            }
            int focusRowHandle = grvTheInSeam.FocusedRowHandle;
            grcTheInSeam.DataSource = dt.Rows.Count == 0 ? CreateTblTheInSeam() : dt;
            if (focusRowHandle == 0)
            {
                LoadInSeam();
            }
            else
            {
                grvTheInSeam.FocusedRowHandle = focusRowHandle;
                grvTheInSeam.ClearSelection();
                grvTheInSeam.SelectRow(focusRowHandle);
            }
        }
        private void SaveTheInSeam(DataTable dtSource, bool showSuccessForm = true)
        {
            grvTheInSeam.CloseEditor();
            grvTheInSeam.UpdateCurrentRow();
            if (dtSource == null || dtSource.Rows.Count == 0) return;
            #region T
            foreach (DataRow dr in dtSource.Rows)
            {
                if (dr.RowState == DataRowState.Added || dr.RowState == DataRowState.Modified)
                {
                    if (string.IsNullOrWhiteSpace(dr["TheInSeam"]?.ToString()))
                    {
                        MessageBox.Show("Vui lòng nhập 'Thẻ InSeam' cho tất cả các dòng.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            #endregion
            var dtSave = CreateTblTheInSeam();
            foreach (DataRow dr in dtSource.Rows)
            {
                if (dr.RowState != DataRowState.Added && dr.RowState != DataRowState.Modified)
                    continue;
                if (dr["TheInSeam"].ToString() == "") continue;
                var drNew = dtSave.NewRow();
                var TheInSeam = dr["TheInSeam"].ToString().Trim().ToUpper();
                drNew["ID"] = 0;
                drNew["MaTheInSeam"] = ReplaceSpecialCharacterssize(TheInSeam);
                drNew["TheInSeam"] = TheInSeam;
                drNew["GhiChu"] = dr["GhiChu"];
                drNew["NguoiTao"] = dr["NguoiTao"];
                drNew["NgayTao"] = dr["NgayTao"];
                drNew["NguoiSua"] = dr["NguoiSua"];
                drNew["NgaySua"] = dr["NgaySua"];
                dtSave.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"ERPBangInSeam/Post?action=PostTheInSeam");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                if (showSuccessForm)
                    clsWaitForm.ShowSuccessForm(this, 2000);
                LoadTheInSeam();
            }
        }
        private void DeleteTheInSeam()
        {

            var dtDelete = CreateTblTheInSeam();
            int[] selectedRows = grvTheInSeam.GetSelectedRows();
            if (selectedRows.Length == 0) return;
            foreach (int rowHandle in selectedRows)
            {
                DataRow dr = grvTheInSeam.GetDataRow(rowHandle);
                var drNew = dtDelete.NewRow();
                drNew["ID"] = dr["ID"];
                dtDelete.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"ERPBangInSeam/Post?action=DeleteTheInSeam");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtDelete); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadTheInSeam();
            }


        }
        private void GrvTheInSeam_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadInSeam();
        }
        private void btnAddTheInSeam_Click(object sender, EventArgs e)
        {
            var dtSource = grcTheInSeam.DataSource as DataTable;
            var drNew = dtSource.NewRow();
            drNew["NguoiTao"] = GlobleData.UserName;
            drNew["NgayTao"] = DateTime.Now;
            dtSource.Rows.InsertAt(drNew, dtSource.Rows.Count);
        }

        private void btnSaveTheInSeam_Click(object sender, EventArgs e)
        {
            var dtSource = grcTheInSeam.DataSource as DataTable;
            SaveTheInSeam(dtSource);

        }

        private void btnDeleteTheInSeam_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa thẻ inseam này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                DeleteTheInSeam();
        }
        #endregion Thẻ màu

        #region  Chi tiết màu
        private void LoadInSeam()
        {
            var drFocus = grvTheInSeam.GetFocusedDataRow();
            if (drFocus == null) return;
            var _MaTheInSeam = drFocus["MaTheInSeam"].ToString();
            if (drFocus == null || _MaTheInSeam == "")
            {
                grcInSeam.DataSource = CreateTblInSeam();
                return;
            }
            string url = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeam&para1={_MaTheInSeam}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt != null)
            {
                dt.AcceptChanges();// Để theo dõi RowState (Added, Modified)
            }
            int focusRowHandle = grvInSeam.FocusedRowHandle;
            grcInSeam.DataSource = dt.Rows.Count == 0 ? CreateTblInSeam() : dt;
            grvInSeam.FocusedRowHandle = focusRowHandle;
            grvInSeam.ClearSelection();
            grvInSeam.SelectRow(focusRowHandle);
        }
        private void SaveInSeam(DataTable dtSource, bool showSuccessForm = true)
        {
            grvInSeam.CloseEditor();
            grvInSeam.UpdateCurrentRow();
            if (dtSource == null || dtSource.Rows.Count == 0) return;
            #region T
            foreach (DataRow dr in dtSource.Rows)
            {
                if (dr.RowState == DataRowState.Added || dr.RowState == DataRowState.Modified)
                {
                    if (string.IsNullOrWhiteSpace(dr["InSeam"]?.ToString()))
                    {
                        MessageBox.Show("Vui lòng nhập 'InSeam' cho tất cả các dòng chi tiết.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            #endregion
            var drFocus = grvTheInSeam.GetFocusedDataRow();
            var _MaTheInSeam = drFocus["MaTheInSeam"].ToString();
            var dtSave = CreateTblInSeam();
            foreach (DataRow dr in dtSource.Rows)
            {
                if (dr.RowState != DataRowState.Added && dr.RowState != DataRowState.Modified)
                    continue;
                if (dr["InSeam"].ToString() == "") continue;
                var drNew = dtSave.NewRow();
                var InSeam = dr["InSeam"].ToString().Trim().ToUpper();
                drNew["ID"] = 0;
                drNew["MaTheInSeam"] = _MaTheInSeam;
                drNew["MaInSeam"] = ReplaceSpecialCharacterssize(InSeam);
                drNew["InSeam"] = InSeam;
                drNew["GhiChu"] = dr["GhiChu"];
                drNew["NguoiTao"] = dr["NguoiTao"];
                drNew["NgayTao"] = dr["NgayTao"];
                drNew["NguoiSua"] = dr["NguoiSua"];
                drNew["NgaySua"] = dr["NgaySua"];
                dtSave.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"ERPBangInSeam/Post?action=PostInSeam");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                if (showSuccessForm)
                    clsWaitForm.ShowSuccessForm(this, 2000);
                LoadInSeam();
            }
        }
        private void DeleteInSeam()
        {
            var dtDelete = CreateTblInSeam();
            int[] selectedRows = grvInSeam.GetSelectedRows();
            foreach (int rowHandle in selectedRows)
            {
                DataRow dr = grvInSeam.GetDataRow(rowHandle);
                var drNew = dtDelete.NewRow();
                drNew["ID"] = dr["ID"];
                dtDelete.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"ERPBangInSeam/Post?action=DeleteInSeam");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtDelete); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadInSeam();
            }
        }
        private void btnAddInSeam_Click(object sender, EventArgs e)
        {
            var drFocus = grvTheInSeam.GetFocusedDataRow();
            if (drFocus == null)
            {
                MessageBox.Show("Vui lòng chọn thẻ inseam");
                return;
            }
            else if (drFocus["MaTheInSeam"].ToString() == "")
            {
                var dtSourceThe = grcTheInSeam.DataSource as DataTable;
                SaveTheInSeam(dtSourceThe, false);
            }
            var dtSource = grcInSeam.DataSource as DataTable;
            var drNew = dtSource.NewRow();
            drNew["NguoiTao"] = GlobleData.UserName;
            drNew["NgayTao"] = DateTime.Now;
            dtSource.Rows.InsertAt(drNew, dtSource.Rows.Count);
        }

        private void btnSaveInSeam_Click(object sender, EventArgs e)
        {
            var dtSource = grcInSeam.DataSource as DataTable;
            SaveInSeam(dtSource);
        }

        private void btnDeleteInSeam_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa inseam này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                DeleteInSeam();
        }
        #endregion


        private void btnImport_InSeam_Click(object sender, EventArgs e)
        {


        }
        private DataTable CreateTblTheInSeam()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaTheInSeam", typeof(string));
            dt.Columns.Add("TheInSeam", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }
        private DataTable CreateTblInSeam()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaTheInSeam", typeof(string));
            dt.Columns.Add("MaInSeam", typeof(string));
            dt.Columns.Add("InSeam", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }
        private void grvInSeam_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                var fieldName = grvInSeam.FocusedColumn.FieldName;
                if (fieldName != "InSeam")
                {
                    e.Cancel = false;
                    return;
                }
                if ((bool)grvInSeam.GetFocusedRowCellValue("IsSave") == true) //**** lỗi lúc click sau khi ấn thêm
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = false;
                }
            }
            catch (Exception ex) { }

        }

        private void btnImport_InSeam_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                frmImportExcel fr = new frmImportExcel();
                fr.FormClosed += Fr_FormClosed;
                fr.ShowDialog();
            }
            catch (Exception ex) { }
        }
        private void Fr_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmImportExcel form2 = (frmImportExcel)sender;
            DataTable dtSource = form2.dt;
            var dtTheInSeam = dtSource.AsEnumerable().Select(x => x["TheInSeam"].ToString()).Distinct();
            var dtTheInSeamSave = CreateTblTheInSeam();
            foreach (var item in dtTheInSeam)
            {
                if (string.IsNullOrEmpty(item)) continue;
                var drTheInSeam = dtTheInSeamSave.NewRow();
                drTheInSeam["ID"] = 0;
                drTheInSeam["MaTheInSeam"] = ReplaceSpecialCharacterssize(item.Trim().ToUpper());
                drTheInSeam["TheInSeam"] = item.Trim().ToUpper();
                drTheInSeam["GhiChu"] = "";
                drTheInSeam["NguoiTao"] = GlobleData.UserName;
                drTheInSeam["NgayTao"] = DateTime.Now;
                drTheInSeam["NguoiSua"] = DBNull.Value;
                drTheInSeam["NgaySua"] = DBNull.Value;
                dtTheInSeamSave.Rows.Add(drTheInSeam);
            }
            if (dtTheInSeamSave.Rows.Count == 0) return;
            string url = string.Format("{0}", URL + $"ERPBangInSeam/Post?action=PostTheInSeam");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtTheInSeamSave); }).Result;


            if (msResult.ToUpper() == "TRUE")
            {
                url = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeam&para1=All");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable dtSourceInSeam = JsonConvert.DeserializeObject<DataTable>(json);
                var dtInSeamSave = CreateTblInSeam();
                foreach (var item in dtTheInSeam)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    var dtTempInSeam = dtSource.AsEnumerable().Where(x => x["TheInSeam"].ToString() == item.Trim().ToUpper());
                    foreach (DataRow dr in dtTempInSeam)
                    {
                        var _maInSeam = ReplaceSpecialCharacterssize(dr["InSeam"].ToString().Trim().ToUpper());
                        var drCheckInSeam = dtSourceInSeam.AsEnumerable().Where(x => x["MaTheInSeam"].ToString().Trim().ToUpper() == item.Trim().ToUpper()
                                                                             & x["MaInSeam"].ToString().Trim().ToUpper() == _maInSeam).FirstOrDefault();
                        if (drCheckInSeam != null) continue;
                        var drExist = dtInSeamSave.AsEnumerable().Where(x => x["MaTheInSeam"].ToString().Trim().ToUpper() == item.Trim().ToUpper()
                                                                             & x["MaInSeam"].ToString().Trim().ToUpper() == _maInSeam).FirstOrDefault();
                        if (drExist != null) continue;
                        var drNew = dtInSeamSave.NewRow();
                        drNew["ID"] = 0;
                        drNew["MaTheInSeam"] = ReplaceSpecialCharacterssize(item.Trim().ToUpper());
                        drNew["MaInSeam"] = _maInSeam;
                        drNew["InSeam"] = dr["InSeam"].ToString().Trim().ToUpper();
                        drNew["GhiChu"] = dr["GhiChu"];
                        drNew["NguoiTao"] = GlobleData.UserName;
                        drNew["NgayTao"] = DateTime.Now;
                        drNew["NguoiSua"] = DBNull.Value;
                        drNew["NgaySua"] = DBNull.Value;
                        dtInSeamSave.Rows.Add(drNew);
                    }
                }
                if (dtInSeamSave.Rows.Count == 0) return;
                url = string.Format("{0}", URL + $"ERPBangInSeam/Post?action=PostInSeam");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtInSeamSave); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    LoadTheInSeam();
                }
            }
        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadTheInSeam();
        }


        //private string ReplaceSpecialCharacterssize(string input)
        //{
        //    string pattern = @"[^\w\s\(\)]+";
        //    string replacement = "_";
        //    Regex regex = new Regex(pattern);
        //    var a = regex.Replace(input, replacement).Replace(" ", "");
        //    return regex.Replace(input, replacement).Replace(" ", "");
        //}
        static string ReplaceSpecialCharacterssize(string input)
        {

            // Pattern để tìm khoảng trắng và các kí tự đặc biệt
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            // Thay thế bằng dấu _
            string replacement = "_";
            // Tạo một Regex và thực hiện thay thế
            Regex regex = new Regex(pattern);
            var value1 = regex.Replace(input, replacement);
            string value = RemoveVietnameseTone(value1);
            return value;
        }
        #region Thoai
        private void grvTheInSeam_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colNguoiTaoTInSeam || e.Column == colNguoiSuaTInSeam)
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

        private void grvTheInSeam_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
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

        private void grvInSeam_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
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

        private void grvInSeam_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colNguoiTaoInSeam || e.Column == colNguoiSuaInSeam)
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
        public static string RemoveVietnameseTone(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            string result = stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToUpper();
            result = result.Replace('đ', 'd');
            return result;
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
