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
    public partial class frmERPMua : DevExpress.XtraEditors.XtraForm
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
        public frmERPMua()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            tenNVHienThi = GetTenNhanVien(); //Khởi tạo ds tên nv
            LoadMua();
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
        private void LoadMua()
        {
            string url = string.Format("{0}", URL + $"Mua/Get?action=Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            #region Thoai
            if (dt != null)
            {
                dt.AcceptChanges();
            }
            #endregion
            int focusRowHandle = grvMua.FocusedRowHandle;
            grcMua.DataSource = dt.Rows.Count == 0 ? CreateTblMua() : dt;
            grvMua.FocusedRowHandle = focusRowHandle;
        }
        #region Thoai
        private void SaveMua(DataTable dtSource)
        {
            grvMua.CloseEditor();
            grvMua.UpdateCurrentRow();

            if (dtSource == null || dtSource.Rows.Count == 0) return;
            DataTable dtSend = CreateTblMua();

            for (int i = 0; i < grvMua.RowCount; i++)
            {
                DataRow row = grvMua.GetDataRow(i);
                if (row == null) continue;
                if (row.RowState != DataRowState.Added && row.RowState != DataRowState.Modified)
                    continue;

                DataRow newR = dtSend.NewRow();

                string maMua = row["MaMua"]?.ToString();
                if (string.IsNullOrEmpty(maMua))
                    maMua = ReplaceSpecialCharacterssize(row["Mua"]?.ToString() ?? "");

                newR["ID"] = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]);
                newR["MaMua"] = maMua;
                newR["Mua"] = row["Mua"]?.ToString() ?? "";
                newR["GhiChu"] = row["GhiChu"]?.ToString() ?? "";

                if (row.RowState == DataRowState.Added)
                {
                    newR["NguoiTao"] = GlobleData.UserName;
                    newR["NgayTao"] = DateTime.Now;
                    newR["NguoiSua"] = DBNull.Value;
                    newR["NgaySua"] = DBNull.Value;
                }
                else if (row.RowState == DataRowState.Modified)
                {
                    newR["NguoiTao"] = row["NguoiTao", DataRowVersion.Original];
                    newR["NgayTao"] = row["NgayTao", DataRowVersion.Original];
                    newR["NguoiSua"] = GlobleData.UserName;
                    newR["NgaySua"] = DateTime.Now;
                }
                dtSend.Rows.Add(newR);
            }


            if (dtSend.Rows.Count == 0)
            {
                clsWaitForm.ShowSuccessFormCustom(this, 2000, "Không có dữ liệu thay đổi!");
                return;
            }

            string url = string.Format("{0}", URL + $"Mua/Post?action=Post");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSend); }).Result;

            if (msResult != null && msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadMua();
            }
            else
            {
                clsWaitForm.ShowErrorForm(this, 2000);
            }
        }
        #endregion

        /*private void SaveMua(DataTable dtSource)
        {
            if (dtSource.Rows.Count == 0) return;
            var dtSave = CreateTblMua();
            foreach (DataRow dr in dtSource.Rows)
            {
                if (dr["Mua"].ToString() == "") continue;
                var drNew = dtSave.NewRow();
                drNew["ID"] = 0;
                drNew["MaMua"] = dr["MaMua"].ToString() != "" ? dr["MaMua"] : ReplaceSpecialCharacterssize(dr["Mua"].ToString());
                drNew["Mua"] = dr["Mua"];
                drNew["GhiChu"] = dr["GhiChu"];
                dtSave.Rows.Add(drNew);
            }
            string url = string.Format("{0}", URL + $"Mua/Post?action=Post");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadMua();
            }
        }*/
        private void DeleteMua()
        {
            var drFocus = grvMua.GetFocusedDataRow();
            if (drFocus == null) return;
            var IDDelete = drFocus["ID"];
            string url = string.Format("{0}", URL + $"Mua/Delete?action=Delete&parameter={IDDelete}");
            var msResult = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadMua();
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
        private DataTable CreateTblMua()
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaMua", typeof(string));
            dt.Columns.Add("Mua", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("NguoiSua", typeof(string));
            dt.Columns.Add("NgaySua", typeof(DateTime));
            return dt;
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var dtSource = grcMua.DataSource as DataTable;
            if (dtSource == null) return;

            DataRow drNew = dtSource.NewRow();
            //Thoai
            drNew["NguoiTao"] = GlobleData.UserName;
            drNew["NgayTao"] = DateTime.Now;

            dtSource.Rows.Add(drNew);
            grvMua.FocusedRowHandle = grvMua.RowCount - 1;
            grvMua.FocusedColumn = grvMua.Columns["Mua"];
            grvMua.ShowEditor();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = button1;
            var dtSource = grcMua.DataSource as DataTable;
            SaveMua(dtSource);
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa mùa này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
                DeleteMua();
        }
        #region Thoai thêm hiển thị username -> Tên
        private void grvMua_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colNguoiTao || e.Column == colNguoiSua)
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

        private void grvMua_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            try
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view == null) return;

                var currentUser = GlobleData.UserName;

                if (!string.IsNullOrEmpty(currentUser))
                {
                    view.SetRowCellValue(e.RowHandle, "NguoiTao", currentUser);
                    view.SetRowCellValue(e.RowHandle, "NgayTao", DateTime.Now);
                }
                view.UpdateCurrentRow();
                _rowAdd = e.RowHandle;
                _status = ResourceURL.EventStatus.Add;
            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 1500);
            }
        }

        private void grvMua_CellValueChanged(object sender, CellValueChangedEventArgs e)
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
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadMua();
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
