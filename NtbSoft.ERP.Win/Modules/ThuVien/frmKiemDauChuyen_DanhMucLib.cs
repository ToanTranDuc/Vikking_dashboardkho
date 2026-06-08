using DevExpress.XtraGrid.Views.Base;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmKiemDauChuyen_DanhMucLib : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        int _rowAdd = -1;
        List<int> lstRowUpdate = new List<int>();
        //Thoai tạo biến hiển thị username -> tên
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();
        private ResourceURL.EventStatus _status;
        public frmKiemDauChuyen_DanhMucLib()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            grvDanhMuc.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
        }
        protected override void OnLoad(EventArgs e)
        {
            tenNVHienThi = GetTenNhanVien(); //Khởi tạo ds tên nv
            LoadDanhMuc();
        }
        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            KHDongThungLib.CustomDrawRowIndicator(sender, e);
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
            if (!_allowAdd)
            {
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Luu.Enabled = false;
            }
            else
            {
                Luu.Enabled = false;
            }
            if (!_allowDelete)
                Xoa.Enabled = false;

        }
        private void LoadDanhMuc()
        {
            string url = URL + $"QTY_KiemDauChuyen/Get?action=GetDanhMucLib";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            #region Thoai
            if (dt != null)
            {
                dt.AcceptChanges();
            }
            #endregion
            int focusRowHandle = grvDanhMuc.FocusedRowHandle;
            grcDanhMuc.DataSource = dt.Rows.Count == 0 ? CreateTblDanhMuc() : dt;
            grvDanhMuc.FocusedRowHandle = focusRowHandle;
        }
        #region Thoai
        private void SaveDanhMuc(DataTable dtSource)
        {
            grvDanhMuc.CloseEditor();
            grvDanhMuc.UpdateCurrentRow();

            if (dtSource == null || dtSource.Rows.Count == 0) return;
            DataTable dtSend = CreateTblDanhMuc();
            int SortIndex = 1;
            foreach (DataRow row in dtSource.Rows)
            {
                DataRow newR = dtSend.NewRow();
                newR["ID"] = row["ID"];
                newR["Code"] = row["Code"];
                newR["Sort"] = SortIndex;
                dtSend.Rows.Add(newR);
                SortIndex++;
            }

            if (dtSend.Rows.Count == 0)
            {
                clsWaitForm.ShowSuccessFormCustom(this, 2000, "Không có dữ liệu thay đổi!");
                return;
            }
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Post?action=PostDanhMucLib");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSend); }).Result;          

            if (msResult != null && msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadDanhMuc();
            }
            else
            {
                clsWaitForm.ShowErrorForm(this, 2000);
            }
        }
        #endregion
      
        private void DeleteDanhMuc()
        {
            var drFocus = grvDanhMuc.GetFocusedDataRow();
            if (drFocus == null) return;           

            var IDDelete = drFocus["ID"];
            DataTable dtSend = CreateTblDanhMuc();
            DataRow newR = dtSend.NewRow();
            newR["ID"] = drFocus["ID"];            
            dtSend.Rows.Add(newR);

            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Post?action=DeleteDanhMucLib");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSend); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadDanhMuc();
            }
        }
        private string ReplaceSpecialCharacterssize(string input)
        {
            string pattern = @"[^\w\s\(\)]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            var a = regex.Replace(input, replacement).Replace(" ", "");
            return regex.Replace(input, replacement).Replace(" ", "");
        }
        private DataTable CreateTblDanhMuc()
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Sort", typeof(string));
            return dt;
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var dtSource = grcDanhMuc.DataSource as DataTable;
            if (dtSource == null) return;
            var drFocus = grvDanhMuc.GetFocusedDataRow();

            DataRow drNew = dtSource.NewRow();
            drNew["ID"] = 0;
            var index = 0;
            if (dtSource.Rows.Count > 0 && drFocus != null)
                index = dtSource.Rows.IndexOf(drFocus);

            dtSource.Rows.InsertAt(drNew, index + 1);
            grvDanhMuc.FocusedRowHandle = index + 1; 
            grvDanhMuc.ShowEditor();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = button1;
            var dtSource = grcDanhMuc.DataSource as DataTable;
            SaveDanhMuc(dtSource);
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa mùa này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                DeleteDanhMuc();
        }
        #region Thoai thêm hiển thị username -> Tên
        private void grvMua_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            //if (e.Column == colNguoiTao || e.Column == colNguoiSua)
            //{
            //    string username = e.Value as string;
            //    if (!string.IsNullOrEmpty(username) && tenNVHienThi != null)
            //    {
            //        string upperUser = username.ToUpper();
            //        if (tenNVHienThi.ContainsKey(upperUser))
            //        {
            //            e.DisplayText = tenNVHienThi[upperUser];
            //        }
            //    }
            //}
        }

        private void grvMua_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            //try
            //{
            //    var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            //    if (view == null) return;

            //    var currentUser = GlobleData.UserName;

            //    if (!string.IsNullOrEmpty(currentUser))
            //    {
            //        view.SetRowCellValue(e.RowHandle, "NguoiTao", currentUser);
            //        view.SetRowCellValue(e.RowHandle, "NgayTao", DateTime.Now);
            //    }
            //    view.UpdateCurrentRow();
            //    _rowAdd = e.RowHandle;
            //    _status = ResourceURL.EventStatus.Add;
            //}
            //catch (Exception ex)
            //{
            //    clsWaitForm.ShowErrorForm(this, 1500);
            //}
        }

        private void grvMua_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            //try
            //{
            //    GridView view = sender as GridView;
            //    if (view == null || e.RowHandle < 0 || view.IsNewItemRow(e.RowHandle)) return;

            //    DataRow row = view.GetDataRow(e.RowHandle);
            //    if (row == null) return;

            //    if (row["ID"] != DBNull.Value && Convert.ToInt32(row["ID"]) > 0)
            //    {
            //        row["NguoiSua"] = GlobleData.UserName;
            //        row["NgaySua"] = DateTime.Now;
            //    }
            //    else if (string.IsNullOrEmpty(row["NguoiTao"]?.ToString()))
            //    {
            //        row["NguoiTao"] = GlobleData.UserName;
            //        row["NgayTao"] = DateTime.Now;
            //    }
            //}
            //catch
            //{
            //    clsWaitForm.ShowErrorForm(this, 1500);
            //}
        }
        #endregion
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadDanhMuc();
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
