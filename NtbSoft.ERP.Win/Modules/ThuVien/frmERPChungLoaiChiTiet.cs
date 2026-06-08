using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERPChungLoaiChiTiet : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tblThuVien;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        public frmERPChungLoaiChiTiet()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
          
            CheckPerminsion();
        }
        protected override void OnLoad(EventArgs e)
        {
            tblThuVien = new DataTable();
            CreateTableThuVien();
            loadData();
            CreateSearcLookUpCL();
            this.ActiveControl = button1;
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
                btnThem.Enabled = false;
            }
            if (!_allowEdit && !_allowAdd)
            {

                btnSave.Enabled = false;
                gV.OptionsBehavior.Editable = false;
            }

            if (!_allowDelete)
                btnDelete.Enabled = false;

        }
        private void loadData()
        {
            try
            {
                string url = $"{URL}NhomNPL/GetChung?Action=GET";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    
                    gC.DataSource = null;
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                gC.DataSource = tbl;
                this.ActiveControl = button1;
            }
            catch (Exception ex)
            {

            }

        }

       
        private void CreateTableThuVien()
        {
            tblThuVien = new DataTable("tblThuVien");
            tblThuVien.Columns.Add("ID", typeof(int));
            tblThuVien.Columns.Add("MaNhom", typeof(string));
            tblThuVien.Columns.Add("TenNhom", typeof(string));
            tblThuVien.Columns.Add("NPL", typeof(bool));
            tblThuVien.Columns.Add("TenTA", typeof(string));
            tblThuVien.Columns.Add("Sort", typeof(int));
            tblThuVien.Columns.Add("VietTat", typeof(string));
            tblThuVien.Columns.Add("MaCLVT", typeof(string));

        }
        private void CreateSearcLookUpCL()
        {
            searchLookUpEditCL.Properties.ValueMember = "MaCLVT";
            searchLookUpEditCL.Properties.DisplayMember = "Display";
            searchLookUpEditLocCL.Properties.ValueMember = "MaCLVT";
            searchLookUpEditLocCL.Properties.DisplayMember = "Display";

            string url = $"{URL}NhomNPL/GetChung?Action=GETCHUNGLOAIVATTU";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                searchLookUpEditCL.Properties.DataSource = null;
                searchLookUpEditLocCL.Properties.DataSource = null;
                return;
            }
            DataTable tblCL = JsonConvert.DeserializeObject<DataTable>(json);
            if (tblCL == null || tblCL.Rows.Count == 0)
            {
                searchLookUpEditCL.Properties.DataSource = null;
                searchLookUpEditLocCL.Properties.DataSource = null;
                return;
            }    
               
            searchLookUpEditCL.Properties.DataSource = tblCL;
            searchLookUpEditLocCL.Properties.DataSource = tblCL;

        }
        private void searchLookUpEdit1View_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn15)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
        }

       

        private void searchLookUpEdit2View_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn16)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
        }

      
        private void gV_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn4|| info.Column == gridColumn8)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gV.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gV.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
              
                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (searchLookUpEditCL.EditValue == null || searchLookUpEditCL.EditValue.ToString() == "")
                return;
            if (string.IsNullOrWhiteSpace(txtTenCLVT.Text.ToString()))
                return;
            DataTable tblGC = gC.DataSource as DataTable;
            DataRow dr = tblGC.NewRow();
            var dataRowSearch = searchLookUpEditCL.Properties.View.GetFocusedDataRow();
            if (dataRowSearch == null)
                return;
            string tenNhomMoi = txtTenCLVT.Text.Trim();
            string maCLVT = dataRowSearch["MaCLVT"].ToString();
            bool existed = tblGC.AsEnumerable().Any(r =>
                string.Equals(r.Field<string>("MaCLVT"), maCLVT, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(r.Field<string>("TenNhom"), tenNhomMoi, StringComparison.OrdinalIgnoreCase));
            if (existed)
            {
                MessageBox.Show("Trùng chủng loại. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }    
               
           
            int maxSort = tblGC.AsEnumerable()
                .Where(r => r.Field<string>("MaCLVT") == maCLVT && !r.IsNull("Sort"))
                .Select(r => Convert.ToInt32(r["Sort"]))
                .DefaultIfEmpty(0)
                .Max();
            dr["MaCLVT"] = maCLVT;
            dr["TenCLVT"] = dataRowSearch["TenCLVT"].ToString();
            dr["SortGroup"] = dataRowSearch["Sort"];
            dr["NPL"] = dataRowSearch["IsNPL"].ToString();
            dr["LoaiVT"] = dataRowSearch["LoaiVT"].ToString();
            dr["TenNhom"] = txtTenCLVT.Text.ToString();
            dr["VietTat"] = txtVietTat.Text.ToString();
            dr["Sort"] = maxSort + 1;
            tblGC.Rows.Add(dr);
            gC.DataSource = tblGC;
            searchLookUpEditCL.EditValue = null;
            txtTenCLVT.Text = "";
            txtVietTat.Text = "";
        }
        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = button1;
                DataTable tblGC = gC.DataSource as DataTable;
                if (tblGC == null || tblGC.Rows.Count == 0) return;
                bool coCanhBao = checkEmpty(tblGC);
                if (coCanhBao)
                {
                    XtraMessageBox.Show("Dữ liệu trống. Vui lòng kiểm tra lại tên chủng loại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var duplicateRows = GetDuplicateRows(tblGC);
                if (duplicateRows.Any())
                {

                    string message = "Các dòng sau bị trùng:\n";
                    foreach (var row in duplicateRows)
                    {
                        int rowIndex = tblThuVien.Rows.IndexOf(row) + 1;
                        message += $"- Dòng {rowIndex}: {row["TenCLVT"]}, {row["TenNhom"]}\n";
                    }
                    XtraMessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach(DataRow dr in tblGC.Rows)
                {
                    DataRow nr = tblThuVien.NewRow();
                    nr["ID"] = dr["ID"];
                    nr["MaNhom"] = dr["MaNhom"];
                    nr["TenNhom"] = dr["TenNhom"];
                    nr["NPL"] = dr["NPL"];
                    nr["TenTA"] = dr["TenTA"];
                    nr["Sort"] = dr["Sort"];
                    nr["VietTat"] = dr["VietTat"];
                    nr["MaCLVT"] = dr["MaCLVT"];
                    tblThuVien.Rows.Add(nr);
                }    
              
                string url = $"{URL}NhomNPL/Post?para={GlobleData.UserName}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblThuVien); }).Result;

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    loadData();

                }
            }
            catch (Exception ex)
            {


            }
        }
        public bool checkEmpty(DataTable dt)
        {
            bool empty = false;

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = dt.Rows[i];

                string tenCLVT = row["TenCLVT"]?.ToString().Trim();
                string tenNhom = row["TenNhom"]?.ToString().Trim();





                if (string.IsNullOrWhiteSpace(tenCLVT) || string.IsNullOrWhiteSpace(tenNhom))
                {
                    empty = true;
                }
            }

            return empty;
        }
        private List<DataRow> GetDuplicateRows(DataTable dt)
        {
            List<DataRow> duplicateRows = new List<DataRow>();

            if (dt == null || dt.Rows.Count == 0)
                return duplicateRows;

            var groups = dt.AsEnumerable()
              .GroupBy(row => new
              {
                  MaCLVT = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("MaCLVT"))),
                  TenNhom = ReplaceSpecialCharacters(RemoveVietnameseTone(row.Field<string>("TenNhom")))
              })
              .Where(g => g.Count() > 1);


            foreach (var group in groups)
            {
                duplicateRows.AddRange(group.Skip(1));
            }
            return duplicateRows;
        }

       

        public string RemoveVietnameseTone(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in normalizedString)
            {

                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (c == 'Đ')
                    {
                        stringBuilder.Append('D');
                    }
                    else if (c == 'đ')
                    {
                        stringBuilder.Append('d');
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private void btnUp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gV == null || gV.FocusedRowHandle < 0)
                return;

            var view = gV;
            view.BeginUpdate();
            view.BeginSort();
            try
            {
                var currentRow = view.GetDataRow(view.FocusedRowHandle);
                if (currentRow == null)
                    return;

                var maClvt = Convert.ToString(currentRow["MaCLVT"]);
                var currentSort = Convert.ToInt64(currentRow["Sort"]);
                var currentId = currentRow["ID"];

                DataTable table = null;
                if (view.DataSource is DataView)
                    table = ((DataView)view.DataSource).Table;
                else if (view.DataSource is DataTable)
                    table = (DataTable)view.DataSource;
                if (table == null)
                    return;

                var previousRow = table.AsEnumerable()
                    .Where(r => Equals(r["MaCLVT"], maClvt) && r.Field<long>("Sort") < currentSort)
                    .OrderByDescending(r => r.Field<long>("Sort"))
                    .FirstOrDefault();
                if (previousRow == null)
                    return;

                var previousSort = previousRow.Field<long>("Sort");
                var previousId = previousRow["ID"];

                var currentHandle = view.LocateByValue("ID", currentId);
                var previousHandle = view.LocateByValue("ID", previousId);

                if (currentHandle >= 0)
                    view.SetRowCellValue(currentHandle, "Sort", previousSort);
                else
                    currentRow["Sort"] = previousSort;

                if (previousHandle >= 0)
                    view.SetRowCellValue(previousHandle, "Sort", currentSort);
                else
                    previousRow["Sort"] = currentSort;

                var sortColumn = view.Columns["Sort"];
                if (sortColumn != null && sortColumn.SortOrder == DevExpress.Data.ColumnSortOrder.None)
                    view.SortInfo.AddRange(new[] { new DevExpress.XtraGrid.Columns.GridColumnSortInfo(sortColumn, DevExpress.Data.ColumnSortOrder.Ascending) });

                view.UpdateCurrentRow();
                view.RefreshData();

                var newHandle = view.LocateByValue("ID", currentId);
                if (newHandle >= 0)
                {
                    view.FocusedRowHandle = newHandle;
                    view.MakeRowVisible(newHandle);
                    view.ClearSelection();
                    view.SelectRow(newHandle);
                }
            }
            finally
            {
                view.EndSort();
                view.EndUpdate();
            }
        }

        private void btnDown_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gV == null || gV.FocusedRowHandle < 0)
                return;

            var view = gV;
            view.BeginUpdate();
            view.BeginSort();
            try
            {
                var currentRow = view.GetDataRow(view.FocusedRowHandle);
                if (currentRow == null)
                    return;

                var maClvt = Convert.ToString(currentRow["MaCLVT"]);
                var currentSort = Convert.ToInt64(currentRow["Sort"]);
                var currentId = currentRow["ID"];

                DataTable table = null;
                if (view.DataSource is DataView)
                    table = ((DataView)view.DataSource).Table;
                else if (view.DataSource is DataTable)
                    table = (DataTable)view.DataSource;
                if (table == null)
                    return;

                var nextRow = table.AsEnumerable()
                    .Where(r => Equals(r["MaCLVT"], maClvt) && r.Field<long>("Sort") > currentSort)
                    .OrderBy(r => r.Field<long>("Sort"))
                    .FirstOrDefault();
                if (nextRow == null)
                    return;

                var nextSort = nextRow.Field<long>("Sort");
                var nextId = nextRow["ID"];

                var currentHandle = view.LocateByValue("ID", currentId);
                var nextHandle = view.LocateByValue("ID", nextId);

                if (currentHandle >= 0)
                    view.SetRowCellValue(currentHandle, "Sort", nextSort);
                else
                    currentRow["Sort"] = nextSort;

                if (nextHandle >= 0)
                    view.SetRowCellValue(nextHandle, "Sort", currentSort);
                else
                    nextRow["Sort"] = currentSort;

                var sortColumn = view.Columns["Sort"];
                if (sortColumn != null && sortColumn.SortOrder == DevExpress.Data.ColumnSortOrder.None)
                    view.SortInfo.AddRange(new[] { new DevExpress.XtraGrid.Columns.GridColumnSortInfo(sortColumn, DevExpress.Data.ColumnSortOrder.Ascending) });

                view.UpdateCurrentRow();
                view.RefreshData();

                var newHandle = view.LocateByValue("ID", currentId);
                if (newHandle >= 0)
                {
                    view.FocusedRowHandle = newHandle;
                    view.MakeRowVisible(newHandle);
                    view.ClearSelection();
                    view.SelectRow(newHandle);
                }
            }
            finally
            {
                view.EndSort();
                view.EndUpdate();
            }
        }

        private void searchLookUpEditLocCL_EditValueChanged(object sender, EventArgs e)
        {
            if (gV == null) return;
            var filterValue = searchLookUpEditLocCL.EditValue as string;
            gV.ActiveFilter.Clear();
            if (string.IsNullOrEmpty(filterValue)) return;
            var escaped = filterValue.Replace("'", "''");
            gV.Columns["MaCLVT"].FilterInfo = new ColumnFilterInfo($"[MaCLVT] = '{escaped}'");
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            loadData();
        }

        static string ReplaceSpecialCharacters(string input)
        {
            try
            {
                string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\”|,.<>/?]+";
                string replacement = "_";
                Regex regex = new Regex(pattern);
                return regex.Replace(input, replacement);
            }
            catch (Exception ex)
            {
                return input;

            }

        }
        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataRow dr = gV.GetFocusedDataRow();
                if (dr == null) return;
                //string urlGET = $"{URL}ERPThuVienMau/Get?Action=GETDELETE&para={dr["MauVTID"].ToString()}";
                //string jsonGET = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                //if (jsonGET != "[]")
                //{
                //    XtraMessageBox.Show("Màu này đang được sử dụng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}
                DataTable tblGC = gC.DataSource as DataTable;
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {

                    if (dr["ID"].ToString() != "0")
                    {
                        string url = $"{URL}NhomNPL/Delete?id={dr["ID"]}&user={GlobleData.UserName}";
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 1000);
                        }
                    }
                    tblGC.Rows.Remove(dr);
                    gC.DataSource = tblGC;
                    this.ActiveControl = button1;

                }
            }
            catch (Exception ex)
            {


            }
        }
    }
}