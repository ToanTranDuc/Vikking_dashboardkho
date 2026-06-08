using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.XuLyVT;
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

namespace NtbSoft.ERP.Win.Modules.MuaHang
{
    public partial class frmTaoPhieuMuaHang : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        KeyDownControlHandler keyDownControlHandler;

        private DataTable gridControl4Table;
        private DataTable gridControl2StartTable;

        public frmTaoPhieuMuaHang()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SplashScreenManager.ShowForm(this, typeof(frmLoading), true, true, false);
            try
            {
                this.WindowState = FormWindowState.Maximized;
                loadSearchLookupEdit1();
                loadSearchLookupEdit2();
                TenPMH.Text = loadTenPMH("Phiếu");
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }
        private void loadGridControl4()
        {
            gridControl4.DataSource = getDataGridControl4();
            gridView4.ExpandAllGroups();
        }
        private DataTable getDataGridControl4()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getpvt";
            request.Parameter = searchLookUpEdit1.EditValue.ToString();
            request.Parameter1 = searchLookUpEdit2.EditValue == null ? "" : searchLookUpEdit2.EditValue.ToString();
            request.Parameter2 = searchLookUpEdit3.EditValue == null ? "" : searchLookUpEdit3.EditValue.ToString();
            string urlGetListDataTable = URL + "XLVTMUAHANG/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            gridControl4Table = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            RepositoryItemCheckEdit checkEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            gridControl4.RepositoryItems.Add(checkEdit);
            if (!gridControl4Table.Columns.Contains("IsSelected"))
                gridControl4Table.Columns.Add("IsSelected", typeof(bool));
            foreach (DataRow row in gridControl4Table.Rows)
            {
                row["IsSelected"] = false;
            }
            GridColumn colCheck = gridView4.Columns["IsSelected"];
            colCheck.ColumnEdit = checkEdit;
            return gridControl4Table;

        }
        private void loadGridControl2()
        {

        }      
        private string loadTenPMH(string tenPMH)
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getall";
            request.Parameter = tenPMH + ' ';
            string urlGetListDataTable = URL + "XLVTMUAHANG/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            string indentName = string.Empty;
            if (dt.Columns.Count == 0 || dt.Rows.Count == 0) return tenPMH + " 1";
            indentName = dt.Rows[0]["TenPMH"].ToString() ?? "";
            if (indentName == "") return tenPMH + " 1";
            int stt = XuLyVTUnits.SmartTryParse<int>(indentName.Split(' ')[1]) + 1;
            return tenPMH + ' ' + stt;
        }
        private void loadSearchLookupEdit1()
        {
            DataTable dt = getNhaCungCap();
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0) return;
            searchLookUpEdit1.Properties.DataSource = dt;
            searchLookUpEdit1.Properties.DisplayMember = "TenKH";
            searchLookUpEdit1.Properties.ValueMember = "MaKH";
            searchLookUpEdit1.Properties.PopulateViewColumns();

            GridView view = searchLookUpEdit1.Properties.View;
            view.Columns["MaKH"].Visible = false;
            view.Columns["ID"].Visible = false;
            view.Columns["NguoiDaiDien"].Visible = false;
            view.Columns["MaSoThue"].Visible = false;
            view.Columns["LoaiDT"].Visible = false;
            view.Columns["TenKH"].Caption = "Nhà cung cấp";
            view.Columns["DiaChi"].Caption = "Địa chỉ";
            view.Columns["SoDienThoai"].Caption = "Số điện thoại";

            view.OptionsFind.AlwaysVisible = true;
        }
        private DataTable getNhaCungCap()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getnhacungcap";

            string urlGetListDataTable = URL + "XLVTMUAHANG/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DataTable donHangTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            return donHangTable;
        }
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            loadGridControl4();
            gridControl2.DataSource = null;
        }
        private void loadSearchLookupEdit2()
        {
            DataTable dt = getKhachHang();
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0) return;
            searchLookUpEdit2.Properties.DataSource = dt;
            searchLookUpEdit2.Properties.DisplayMember = "TenKH";
            searchLookUpEdit2.Properties.ValueMember = "MaKH";
            searchLookUpEdit2.Properties.PopulateViewColumns();

            GridView view = searchLookUpEdit2.Properties.View;
            view.Columns["MaKH"].Visible = false;            
            view.Columns["TenKH"].Caption = "Tên khách hàng";

            view.OptionsFind.AlwaysVisible = true;
        }
        private DataTable getKhachHang()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getkhachhang";

            string urlGetListDataTable = URL + "XLVTMUAHANG/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DataTable donHangTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            return donHangTable;
        }
        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            searchLookUpEdit3.EditValue = null;
            gridControl2.DataSource = null;
            loadSearchLookupEdit3();
            loadGridControl4();           
        }
        private void loadSearchLookupEdit3()
        {
            DataTable dt = getMaHang();
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0) return;
            searchLookUpEdit3.Properties.DataSource = dt;
            searchLookUpEdit3.Properties.DisplayMember = "TenHang";
            searchLookUpEdit3.Properties.ValueMember = "MaHang";
            searchLookUpEdit3.Properties.PopulateViewColumns();

            GridView view = searchLookUpEdit3.Properties.View;
            view.Columns["MaHang"].Visible = false;
            view.Columns["TenHang"].Caption = "Tên hàng";

            view.OptionsFind.AlwaysVisible = true;
        }
        private DataTable getMaHang()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getmahang";
            request.Parameter = searchLookUpEdit2.EditValue.ToString();
            string urlGetListDataTable = URL + "XLVTMUAHANG/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DataTable donHangTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            return donHangTable;
        }
        private void searchLookUpEdit3_EditValueChanged(object sender, EventArgs e)
        {
            gridControl2.DataSource = null;
            loadGridControl4();           
        }
        private void selectBtn_Click(object sender, EventArgs e)
        {
            gridView4.CloseEditor();
            gridView4.UpdateCurrentRow();
            addToGridcontrol2();
        }
        private void addToGridcontrol2()
        {
            DataTable dt = gridControl4.DataSource as DataTable;
            DataRow[] checkedRows = dt.Select("IsSelected = True");
            DataTable dt_target = gridControl2.DataSource as DataTable;
            if (dt_target == null || dt_target.Columns.Count == 0 || dt_target.Rows.Count == 0)
            {
                dt_target = dt.Clone();
            }
            foreach (DataRow row in checkedRows)
            {
                DataRow newRow = dt_target.NewRow();
                foreach (var col in dt.Columns.Cast<DataColumn>())
                {
                    newRow[col.ColumnName] = row[col.ColumnName];
                }
                dt_target.Rows.Add(newRow);
                
            }
            gridControl2StartTable = dt_target.Copy();
            var groupColumns = new[] { "IsSelected", "LoaiVT", "MaVT", "TenVT", "ChiTietVT", "MaMauVT", "TenMauVT", "MaSizeVT", "TenSizeVT", "MaDVVT", "TenDVVT" };   // cột để group
            var sumColumns = new[] { "SLTong" };          // cột để sum

            var grouped = dt_target.AsEnumerable()
                .GroupBy(r => string.Join("|", groupColumns.Select(c => r[c])))
                .Select(g =>
                {
                    var dict = new Dictionary<string, object>();
                    var first = g.First();

                    // Giữ lại các cột group
                    foreach (var col in groupColumns)
                        dict[col] = first[col];

                    // Tính tổng cho các cột sum
                    foreach (var col in sumColumns)
                        dict[col] = g.Sum(r => XuLyVTUnits.SmartTryParse<float>(r[col].ToString()));

                    // Với các cột khác (không nằm trong groupColumns và sumColumns) thì cộng dồn chuỗi
                    var otherColumns = dt_target.Columns.Cast<DataColumn>()
                            .Select(c => c.ColumnName)
                            .Where(c => !groupColumns.Contains(c) && !sumColumns.Contains(c));

                    foreach (var col in otherColumns)
                    {
                        dict[col] = string.Join(",", g.Select(r => r[col]?.ToString()).Where(s => !string.IsNullOrEmpty(s)));
                    }

                    return dict;
                })
                .ToList();

            // Tạo DataTable kết quả
            DataTable result = new DataTable();

            // Tạo cột group
            foreach (var col in groupColumns)
            {
                if (col == "IsSelected")
                    result.Columns.Add(col, typeof(bool)); // bắt buộc bool
                else
                    result.Columns.Add(col);
            }

            // Tạo cột sum
            foreach (var col in sumColumns)
                result.Columns.Add(col, typeof(float)); // dùng float vì bạn đang SmartTryParse<float>

            // Tạo cột khác (chuỗi)
            var otherCols = dt_target.Columns.Cast<DataColumn>()
                            .Select(c => c.ColumnName)
                            .Where(c => !groupColumns.Contains(c) && !sumColumns.Contains(c));

            foreach (var col in otherCols)
                result.Columns.Add(col, typeof(string));

            foreach (var item in grouped)
            {
                var row = result.NewRow();
                foreach (var col in groupColumns)
                    row[col] = item[col];
                foreach (var col in sumColumns)
                    row[col] = item[col];
                foreach (var col in otherCols)
                    row[col] = item[col];
                result.Rows.Add(row);
            }
            // Bind lại vào gridControl2
            RepositoryItemCheckEdit checkEdit = new RepositoryItemCheckEdit();
            gridControl2.RepositoryItems.Add(checkEdit);
            GridColumn colCheck = gridView2.Columns["IsSelected"];
            colCheck.ColumnEdit = checkEdit;
            gridControl2.DataSource = result;
            gridView2.ExpandAllGroups();
        }
        private void deSelectBtn_Click(object sender, EventArgs e)
        {
            removeFromGridControl2();
            gridControl2.RefreshDataSource();
        }
        private void removeFromGridControl2()
        {
            DataTable dt = gridControl2.DataSource as DataTable;
            DataRow[] checkedRows = dt.Select("IsSelected = True");
            foreach (DataRow row in checkedRows)
            {
                row.Delete(); // hoặc gridControl2Table.Rows.Remove(row);
            }

            // Nếu dùng Delete thì cần AcceptChanges để commit
            dt.AcceptChanges();
            DataTable dt_target = gridControl2.DataSource as DataTable;
            gridControl2StartTable = dt_target.Copy();
        }
        private void checkAllBtn_Click(object sender, EventArgs e)
        {
            DataTable dt = gridControl4.DataSource as DataTable;
            foreach (DataRow row in dt.Rows)
            {
                row["IsSelected"] = true;
            }
        }

        private void resetGridControl4Btn_Click(object sender, EventArgs e)
        {
            DataTable dt = gridControl4.DataSource as DataTable;
            foreach (DataRow row in dt.Rows)
            {
                row["IsSelected"] = false;
            }
        }
        private void saveBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridControl2StartTable == null || gridControl2StartTable.Columns.Count == 0 || gridControl2StartTable.Rows.Count == 0)
            {
                XtraMessageBox.Show("Chưa chọn vật tư cần mua");
                return;
            }
            if(searchLookUpEdit1.EditValue == null || searchLookUpEdit1.Text == "" || TenPMH.Text == "")
            {
                XtraMessageBox.Show("Thiếu dữ liệu");
                return;
            }
            saveToXLVTMUAHANG();
            this.DialogResult = DialogResult.OK;
            this.Close();
            XtraMessageBox.Show("Lưu phiếu xử lý vật tư thành công!");
        }
        private void saveToXLVTMUAHANG()
        {
            try
            {
                var request = XuLyVTRequestPost.createDefault("XLVTMUAHANG");
                request.Action = "create";

                string newID = XuLyVTUnits.generatedTimeKey("PMH");
                foreach (DataRow gridRow in gridControl2StartTable.Rows)
                {
                    DataRow reqRow = request.TypeTable.NewRow();
                    reqRow["TrangThai"] = "Đang xử lý";
                    reqRow["MaPMH"] = newID;
                    reqRow["TenPMH"] = TenPMH.EditValue ?? DBNull.Value;
                    reqRow["MaPXLVT"] = gridRow["MaPXLVT"] ?? DBNull.Value;
                    reqRow["TenPXLVT"] = gridRow["TenPXLVT"] ?? DBNull.Value;
                    reqRow["MaPVT"] = gridRow["MaPVT"] ?? DBNull.Value;
                    reqRow["TenPVT"] = gridRow["TenPVT"] ?? DBNull.Value;
                    reqRow["MaVT"] = gridRow["MaVT"] ?? DBNull.Value;
                    reqRow["TenVT"] = gridRow["TenVT"] ?? DBNull.Value;
                    reqRow["ChiTietVT"] = gridRow["ChiTietVT"] ?? DBNull.Value;
                    reqRow["LoaiVT"] = gridRow["LoaiVT"] ?? DBNull.Value;
                    reqRow["InSeamMauVT"] = gridRow["InSeamMauVT"] ?? DBNull.Value;
                    reqRow["MaMauVT"] = gridRow["MaMauVT"] ?? DBNull.Value;
                    reqRow["TenMauVT"] = gridRow["TenMauVT"] ?? DBNull.Value;
                    reqRow["MaSizeVT"] = gridRow["MaSizeVT"] ?? DBNull.Value;
                    reqRow["TenSizeVT"] = gridRow["TenSizeVT"] ?? DBNull.Value;
                    reqRow["MaDVVT"] = gridRow["MaDVVT"] ?? DBNull.Value;
                    reqRow["TenDVVT"] = gridRow["TenDVVT"] ?? DBNull.Value;
                    reqRow["MaNhaCungCap"] = searchLookUpEdit1.EditValue ?? DBNull.Value;
                    reqRow["TenNhaCungCap"] = searchLookUpEdit1.Text != "" ? (object)searchLookUpEdit1.Text : DBNull.Value;
                    reqRow["SL"] = gridRow["SLTong"] ?? DBNull.Value;
                    reqRow["DonGia"] = gridRow["DonGiaMM"] ?? DBNull.Value;
                    reqRow["ChiPhiTong"] = XuLyVTUnits.SmartTryParse<float>(gridRow["SLTong"].ToString()) * XuLyVTUnits.SmartTryParse<float>(gridRow["DonGiaMM"].ToString());
                   // reqRow["PYCBaoGia"] = gridRow["MaPhieu"] ?? DBNull.Value;
                    reqRow["TGHTDuKien"] = TGHTDuKienTime.EditValue ?? DBNull.Value;
                    reqRow["NgayTao"] = DateTime.Now;
                    request.TypeTable.Rows.Add(reqRow);
                }

                string urlGetListDataTable = URL + "XLVTMUAHANG/Post";
                string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            }
            catch (Exception ex)
            {

            }
        }

        private void resetBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridControl4Table = null;
            gridControl2StartTable = null;
            gridControl4.DataSource = null;
            gridControl2.DataSource = null;
            searchLookUpEdit1.EditValue = null;
            TGHTDuKienTime.EditValue = null;
        }

        private void gridView2_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                GridGroupRowInfo info = e.Info as GridGroupRowInfo;

                if (view == null || info == null) return;
                GridColumn groupColumn = info.Column;
                int groupLevel = view.GetRowLevel(e.RowHandle);
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            catch (Exception ex)
            {

            }
        }

        private void gridView4_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                GridGroupRowInfo info = e.Info as GridGroupRowInfo;

                if (view == null || info == null) return;
                GridColumn groupColumn = info.Column;
                int groupLevel = view.GetRowLevel(e.RowHandle);
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            catch (Exception ex)
            {

            }
        }


       
    }
}