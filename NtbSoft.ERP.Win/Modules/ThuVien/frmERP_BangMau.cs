using DevExpress.DataAccess.Excel;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
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
    public partial class frmERP_BangMau : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        //Thoai thêm biến lấy tên nv
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();
        public frmERP_BangMau()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            grvTheMau.FocusedRowChanged += GrvTheMau_FocusedRowChanged;
            grvMau.DoubleClick += GrvMau_DoubleClick;
            grvMau.CustomUnboundColumnData += GrvMau_CustomUnboundColumnData;
            grvMau.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
            grvMau.RowCountChanged += gridView_RowCountChanged;
            grvTheMau.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
            grvTheMau.RowCountChanged += gridView_RowCountChanged;
            grvMau.OptionsSelection.MultiSelect = true;
            grvMau.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
        }

      
        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            KHDongThungLib.CustomDrawRowIndicator(sender, e);
        }

        private void gridView_RowCountChanged(object sender, EventArgs e)
        {
            KHDongThungLib.RowCountChanged(sender, e);
        }
       
      
        private void grvTheMau_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
        }
        private void GrvMau_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;               
                if (e.Column.FieldName == "STT" && e.IsGetData)
                    e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
                var urlHost = (string)settingsReader.GetValue("URLV2", typeof(String));
                //if (e.Column.FieldName == "STT" && e.IsGetData)
                //    e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
                if ((e.Column.FieldName == "UrlAnh") && e.IsGetData)
                {
                    DataRow dr = grvMau.GetDataRow(e.ListSourceRowIndex);
                    if (dr == null) return;
                    string url = urlHost + "/Images/BangMau/" + dr["HinhAnh"].ToString();
                    if (!string.IsNullOrEmpty(url))
                    {
                        try
                        {
                            using (var wc = new System.Net.WebClient())
                            {
                                byte[] data = wc.DownloadData(url);
                                using (var ms = new MemoryStream(data))
                                    e.Value = Image.FromStream(ms);
                            }
                        }
                        catch
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        private async void GrvMau_DoubleClick(object sender, EventArgs e)
        {
            var fieldName = grvMau.FocusedColumn.FieldName;
            if (fieldName == "HinhAnh")
            {
                DataRow drFocus = grvMau.GetFocusedDataRow();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string sourceFileName = openFileDialog.FileName;
                    var result = await UploadImage("", sourceFileName, openFileDialog.SafeFileName);
                    if (result)
                    {
                        grvMau.SetRowCellValue(grvMau.FocusedRowHandle, fieldName, openFileDialog.SafeFileName);
                    }
                }
            }
        }
        private async Task<bool> UploadImage(string action, string sourceFileName, string fileName)
        {
            string UrlUpload = "";
            var client = new WebClient();
            //object objUrl = _regedit.get_RegistryKey(QtyUrlKeyName);
            //objUrl = "localhost:5480";

            UrlUpload = string.Format("{0}/ERPBangMau/UploadImage", URL);
            var uri = new Uri(UrlUpload);
            try
            {
                client.Headers.Add("action", action);
                client.Headers.Add("fileName", fileName);
                var data = System.IO.File.ReadAllBytes(sourceFileName);
                var responseBytes = await client.UploadDataTaskAsync(uri, data);
                string result = Encoding.UTF8.GetString(responseBytes);
                if (result.ToUpper() == "TRUE")
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            tenNVHienThi = GetTenNhanVien(); //khởi tạo lấy ds tên nv
            LoadTheMau();
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
            //if (!_allowAdd)
            //{
            //    btnAddMau.Enabled = false;
            //    btnAddTheMau.Enabled = false;

            //}
            //if (!_allowEdit)
            //{
            //    btnSaveMau.Enabled = false;
            //    btnSaveTheMau.Enabled = false;

            //}
            //else
            //{
            //    btnSaveMau.Enabled = true;
            //    btnSaveTheMau.Enabled = true;

            //}
            //if (!_allowDelete)
            //{
            //    btnDeleteMau.Enabled = false;
            //    btnDeleteTheMau.Enabled = false;

            //}
            if (!_allowAdd || !_allowEdit || !_allowDelete)
            {
                btnAddMau.Enabled = false;

                btnImport_Mau.Enabled = true;
                btnImport_Mau.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                btnAddTheMau.Enabled = false;

                btnSaveTheMau.Enabled = false;
                btnSaveMau.Enabled = false;
                btnDeleteTheMau.Enabled = false;
                btnDeleteMau.Enabled = false;

            }

        }
        #region Thẻ màu
        private void LoadTheMau()
        {
            string url = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetTheMau");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt != null)
            {
                dt.AcceptChanges(); // Để theo dõi RowState (Added, Modified)
            }
            int focusRowHandle = grvTheMau.FocusedRowHandle;

            grcTheMau.DataSource = (dt == null || dt.Rows.Count == 0) ? CreateTblTheMau() : dt;
            if (focusRowHandle == 0)
            {
                LoadMau();
            }
            else
            {
                grvTheMau.FocusedRowHandle = focusRowHandle;
                grvTheMau.ClearSelection();
                grvTheMau.SelectRow(focusRowHandle);
            }
        }
        private void SaveTheMau(DataTable dtSource, bool showSuccessForm = true)
        {
            grvTheMau.CloseEditor();
            grvTheMau.UpdateCurrentRow();
            if (dtSource == null || dtSource.Rows.Count == 0) return;
            var dtSave = CreateTblTheMau();
            foreach (DataRow dr in dtSource.Rows)
            {
                if (dr.RowState != DataRowState.Added && dr.RowState != DataRowState.Modified)
                    continue;
                var drNew = dtSave.NewRow();
                var TheMau = dr["TheMau"].ToString().Trim().ToUpper();
                if (TheMau == "") continue;
                drNew["ID"] = 0;
                drNew["MaTheMau"] = ReplaceSpecialCharacterssize(TheMau);
                drNew["TheMau"] = TheMau;
                drNew["GhiChu"] = dr["GhiChu"];
                drNew["NguoiTao"] = dr["NguoiTao"];
                drNew["NgayTao"] = dr["NgayTao"];
                drNew["NguoiSua"] = dr["NguoiSua"];
                drNew["NgaySua"] = dr["NgaySua"];
                dtSave.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"ERPBangMau/Post?action=PostTheMau");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                if (showSuccessForm)
                    clsWaitForm.ShowSuccessForm(this, 2000);
                LoadTheMau();
            }
        }
        private void DeleteTheMau()
        {
            var dtDelete = CreateTblTheMau();
            int[] selectedRows = grvTheMau.GetSelectedRows();
            if (selectedRows.Length == 0) return;
            foreach (int rowHandle in selectedRows)
            {
                DataRow dr = grvTheMau.GetDataRow(rowHandle);
                var drNew = dtDelete.NewRow();
                drNew["ID"] = dr["ID"];
                dtDelete.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"ERPBangMau/Post?action=DeleteTheMau");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtDelete); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadTheMau();
            }
        }
        private void GrvTheMau_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadMau();
        }
        private void btnAddTheMau_Click(object sender, EventArgs e)
        {
            ThemTheMau();
        }
        private void ThemTheMau()
        {
            var dtSource = grcTheMau.DataSource as DataTable;
            var drNew = dtSource.NewRow();
            drNew["NguoiTao"] = GlobleData.UserName;
            drNew["NgayTao"] = DateTime.Now;
            dtSource.Rows.InsertAt(drNew, dtSource.Rows.Count);
        }

        private void btnSaveTheMau_Click(object sender, EventArgs e)
        {
            var dtSource = grcTheMau.DataSource as DataTable;
            SaveTheMau(dtSource);

        }

        private void btnDeleteTheMau_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa thẻ màu này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                DeleteTheMau();
        }
        #endregion Thẻ màu

        #region  Chi tiết màu
        private void LoadMau()
        {
            var drFocus = grvTheMau.GetFocusedDataRow();
            if (drFocus == null || drFocus["MaTheMau"].ToString() == "")
            {
                grcMau.DataSource = CreateTblMau();
                return;
            }
            var _maTheMau = drFocus["MaTheMau"].ToString();
            string url = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMau&para1={_maTheMau}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt != null)
            {
                dt.AcceptChanges();// Để theo dõi RowState (Added, Modified)
            }
            int focusRowHandle = grvMau.FocusedRowHandle;
            grcMau.DataSource = dt.Rows.Count == 0 ? CreateTblMau() : dt;
            grvMau.FocusedRowHandle = focusRowHandle;
            grvMau.ClearSelection();
            grvMau.SelectRow(focusRowHandle);
        }
        private void SaveMau(DataTable dtSource)
        {
            try
            {
                grvMau.CloseEditor();
                grvMau.UpdateCurrentRow();
                if (dtSource == null || dtSource.Rows.Count == 0) return;
                if (!CheckDupicateMaMau(dtSource, "CodeMau", "CodeMau"))
                {
                    return;
                }
                var drFocus = grvTheMau.GetFocusedDataRow();
                var _maTheMau = drFocus["MaTheMau"].ToString();
                var dtSave = CreateTblMau();
                foreach (DataRow dr in dtSource.Rows)
                {
                    if (dr.RowState != DataRowState.Added && dr.RowState != DataRowState.Modified)
                        continue;
                    if (dr["CodeMau"].ToString() == "") continue;
                    var drNew = dtSave.NewRow();
                    var TenMau = dr["TenMau"].ToString().Trim().ToUpper();
                    var CodeMau = dr["CodeMau"].ToString().Trim().ToUpper();
                    drNew["ID"] = 0;
                    drNew["MaTheMau"] = _maTheMau;
                    drNew["MaMau"] = "MAU_" + ReplaceSpecialCharacterssize(CodeMau);
                    drNew["TenMau"] = TenMau;
                    drNew["CodeMau"] = CodeMau;
                    drNew["GhiChu"] = dr["GhiChu"];
                    drNew["HinhAnh"] = dr["HinhAnh"];
                    drNew["NguoiTao"] = dr["NguoiTao"];
                    drNew["NgayTao"] = dr["NgayTao"];
                    drNew["NguoiSua"] = dr["NguoiSua"];
                    drNew["NgaySua"] = dr["NgaySua"];
                    dtSave.Rows.Add(drNew);
                }
                string url = string.Format("{0}", URL + $"ERPBangMau/Post?action=PostMau");
                var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadMau();
                }
            }
            catch(Exception ex) { }
            
        }
        private bool CheckDupicateMaMau(DataTable dtCheck, string fieldName,string fieldNameOutput)
        {
            string valuePopup = "";
            var duplicateGroups = dtCheck.AsEnumerable().Where(x=>x["CodeMau"].ToString() != "")
                                 .GroupBy(r => r.Field<string>(fieldName).Trim().ToUpper())
                                 .Where(g => g.Count() > 1);
            foreach (var group in duplicateGroups)
            {                
                foreach (var row in group)
                {
                    valuePopup = string.Join(", ", row[fieldNameOutput]);                    
                }
            }
            if(duplicateGroups.Count() > 0)
            {
                MessageBox.Show($"Trùng dữ liệu: {valuePopup}  \r\nVui lòng kiểm tra lại!", "Thông báo");
                return false;
            }
            return true ;

        }
        private void DeleteMau()
        {
            var dtDelete = CreateTblMau();
            int[] selectedRows = grvMau.GetSelectedRows();
            foreach (int rowHandle in selectedRows)
            {
                DataRow dr = grvMau.GetDataRow(rowHandle);
                var drNew = dtDelete.NewRow();
                drNew["ID"] = dr["ID"];
                dtDelete.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"ERPBangMau/Post?action=DeleteMau");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtDelete); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadMau();
            }
        }
        private void barThemMau_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemMau();
        }
        private void btnAddMau_Click(object sender, EventArgs e)
        {
            ThemMau();
        }
        private void ThemMau()
        {
            var drFocus = grvTheMau.GetFocusedDataRow();
            if (drFocus == null)
            {
                MessageBox.Show("Vui lòng chọn thẻ màu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string theMau = drFocus["TheMau"].ToString().Trim();
            string maTheMau = drFocus["MaTheMau"].ToString().Trim();
            if (string.IsNullOrEmpty(theMau))
            {
                MessageBox.Show("Vui lòng nhập tên Thẻ màu trước khi thêm màu chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                grvTheMau.FocusedColumn = grvTheMau.Columns["TheMau"];
                grvTheMau.ShowEditor();
                return;
            }
            if (string.IsNullOrEmpty(maTheMau))
            {
                var dtSourceThe = grcTheMau.DataSource as DataTable;
                SaveTheMau(dtSourceThe, false);
                drFocus = grvTheMau.GetFocusedDataRow();
                if (drFocus == null || string.IsNullOrEmpty(drFocus["MaTheMau"].ToString()))
                {
                    MessageBox.Show("Lưu thẻ màu không thành công. Vui lòng thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            var dtSource = grcMau.DataSource as DataTable;

            var drNew = dtSource.NewRow();
            drNew["ID"] = 0;
            drNew["NguoiTao"] = GlobleData.UserName;
            drNew["NgayTao"] = DateTime.Now;
            dtSource.Rows.InsertAt(drNew, dtSource.Rows.Count);

            int newRowHandle = grvMau.GetRowHandle(dtSource.Rows.Count - 1);
            grvMau.FocusedRowHandle = newRowHandle;
            grvMau.FocusedColumn = grvMau.Columns["CodeMau"];
            grvMau.ShowEditor();
        }
        /*private void ThemMau()
        {
            var drFocus = grvTheMau.GetFocusedDataRow();
            if (drFocus == null)
            {
                MessageBox.Show("Vui lòng chọn thẻ màu");
                return;
            }
            else if (drFocus["MaTheMau"].ToString() == "")
            {
                var dtSourceThe = grcTheMau.DataSource as DataTable;
                SaveTheMau(dtSourceThe, false);
            }
            string theMau = drFocus["TheMau"].ToString().Trim();
            string maTheMau = drFocus["MaTheMau"].ToString().Trim();
            if (string.IsNullOrEmpty(theMau))
            {
                MessageBox.Show("Vui lòng nhập tên Thẻ màu trước khi thêm màu chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                grvTheMau.FocusedColumn = grvTheMau.Columns["TheMau"];
                grvTheMau.ShowEditor();
                return;
            }
            if (string.IsNullOrEmpty(maTheMau))
            {
                var dtSourceTMau = grcTheMau.DataSource as DataTable;
                SaveTheMau(dtSourceTMau, false);
                drFocus = grvTheMau.GetFocusedDataRow();
                if(drFocus == null || string.IsNullOrEmpty(drFocus["MaTheMau"].ToString().Trim()))
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                }
            }
            var dtSource = grcMau.DataSource as DataTable;
            var drNew = dtSource.NewRow();
            drNew["ID"] = 0;
            drNew["NguoiTao"] = GlobleData.UserName;
            drNew["NgayTao"] = DateTime.Now;
            dtSource.Rows.InsertAt(drNew, dtSource.Rows.Count);
            int newRowHandle = grvMau.GetRowHandle(dtSource.Rows.Count - 1);
            grvMau.FocusedRowHandle = newRowHandle;
            grvMau.FocusedColumn = grvMau.Columns["CodeMau"];
            grvMau.ShowEditor();
        }*/

        private void btnSaveMau_Click(object sender, EventArgs e)
        {
            var dtSource = grcMau.DataSource as DataTable;
            SaveMau(dtSource);
        }

        private void btnDeleteMau_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa màu này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                DeleteMau();
        }
        #endregion

        private DataTable CreateTblTheMau()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaTheMau", typeof(string));
            dt.Columns.Add("TheMau", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }
        private DataTable CreateTblMau()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaTheMau", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("TenMau", typeof(string));
            dt.Columns.Add("CodeMau", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("MaMauTemp", typeof(string));
            dt.Columns.Add("HinhAnh", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }
        private void grvMau_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                var fieldName = grvMau.FocusedColumn.FieldName;
                if (fieldName != "CodeMau")
                {
                    e.Cancel = false;
                    return;
                }
                /*if ((bool)grvMau.GetFocusedRowCellValue("IsSave") == true) //Lỗi lúc Focus
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = false;
                }*/
                var val = grvMau.GetFocusedRowCellValue("IsSave");
                bool isSave = val != null && val != DBNull.Value && Convert.ToBoolean(val);

                if (isSave)
                    e.Cancel = true;
                else
                    e.Cancel = false;
            }
            catch (Exception ex) { }
        }



        private void btnImport_Mau_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
            var dtTheMau = dtSource.AsEnumerable().Select(x => x["TheMau"].ToString()).Distinct();
            var dtTheMauSave = CreateTblTheMau();
            foreach (var item in dtTheMau)
            {
                if (string.IsNullOrEmpty(item)) continue;
                var drTheMau = dtTheMauSave.NewRow();
                drTheMau["ID"] = 0;
                drTheMau["MaTheMau"] = ReplaceSpecialCharacterssize(item.Trim().ToUpper());
                drTheMau["TheMau"] = item.Trim().ToUpper();
                drTheMau["GhiChu"] = "";
                drTheMau["NguoiTao"] = GlobleData.UserName;
                drTheMau["NgayTao"] = DateTime.Now;
                drTheMau["NguoiSua"] = DBNull.Value;
                drTheMau["NgaySua"] = DBNull.Value;
                dtTheMauSave.Rows.Add(drTheMau);
            }
            if (dtTheMauSave.Rows.Count == 0) return;
            string url = string.Format("{0}", URL + $"ERPBangMau/Post?action=PostTheMau");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtTheMauSave); }).Result;

            if (msResult.ToUpper() == "TRUE")
            {
                url = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMau&para1=All");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable dtSourceMau = JsonConvert.DeserializeObject<DataTable>(json);
                var dtMauSave = CreateTblMau();
                foreach (var item in dtTheMau)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    var dtTempMau = dtSource.AsEnumerable().Where(x => x["TheMau"].ToString() == item.Trim().ToUpper());
                    foreach (DataRow dr in dtTempMau)
                    {
                        var _maMau = "MAU_" + ReplaceSpecialCharacterssize(dr["CodeMau"].ToString().Trim().ToUpper());
                        var drCheckMau = dtSourceMau.AsEnumerable().Where(x => x["MaTheMau"].ToString().ToUpper() == item.Trim().ToUpper()
                                                                             & x["MaMau"].ToString().ToUpper() == _maMau).FirstOrDefault();
                        if (drCheckMau != null) continue;
                        var drExist = dtMauSave.AsEnumerable().Where(x => x["MaTheMau"].ToString().ToUpper() == item.Trim().ToUpper()
                                                                             & x["MaMau"].ToString().ToUpper() == _maMau).FirstOrDefault();
                        if (drExist != null) continue;
                        var drNew = dtMauSave.NewRow();
                        drNew["ID"] = 0;
                        drNew["MaTheMau"] = ReplaceSpecialCharacterssize(item.Trim().ToUpper());
                        drNew["MaMau"] = _maMau;
                        drNew["TenMau"] = dr["Mau"].ToString().Trim().ToUpper();
                        drNew["CodeMau"] = dr["CodeMau"].ToString().Trim().ToUpper();
                        drNew["GhiChu"] = dr["GhiChu"];
                        drNew["NguoiTao"] = GlobleData.UserName;
                        drNew["NgayTao"] = DateTime.Now;
                        drNew["NguoiSua"] = DBNull.Value;
                        drNew["NgaySua"] = DBNull.Value;
                        dtMauSave.Rows.Add(drNew);
                    }
                }
                if (dtMauSave.Rows.Count == 0) return;
                url = string.Format("{0}", URL + $"ERPBangMau/Post?action=PostMau");               
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtMauSave); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    LoadTheMau();
                }
            }

        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadTheMau();
        }

        private void frmERP_BangMau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.N)
            {
                e.Handled = true;
                ThemMau();               
            }
            else if (e.KeyCode == Keys.F1)
            {
                e.Handled = true;
                ThemTheMau();              
            }
            else if(e.KeyCode == Keys.F2)
            {
                e.Handled = true;
                ThemMau();
            }
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
        private void grvMau_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == gridColNguoiTaoMau || e.Column == gridColNguoiSuaMau)
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

        private void grvTheMau_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == gridColNguoiTaoTMau || e.Column == gridColNguoiSuaTMau)
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

        private void grvMau_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
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

        private void grvTheMau_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
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
