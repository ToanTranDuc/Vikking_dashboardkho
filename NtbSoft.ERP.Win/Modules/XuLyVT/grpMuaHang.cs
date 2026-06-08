using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
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
    public partial class grpMuaHang : DevExpress.XtraEditors.XtraUserControl
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        KeyDownControlHandler keyDownControlHandler;

        private DataRow currentRow;
        private bool isLoad;
        private bool isCheckAll;
        private int tsl;

        private DataTable gridControl2Table;
        private DataTable gridControl1Table;
        private DataTable getToGridControl2Table;

        public grpMuaHang(DataRow row = null)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            currentRow = row;
            isLoad = row != null;
            isCheckAll = false;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            loadIndentTypeOption();
            loadIndentSrcTypeOption();
            loadProductPerposeTypeOption();
            loadSearchLookupEdit3();
            loadGridControl1();

            layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (isLoad)
            {
                loadToUpdate();
            }
        }
        public Dictionary<string, object> shareData()
        {
            return new Dictionary<string, object>
            {
                ["LoaiHang"] = LoaiHang.EditValue?.ToString(),
                ["MaKH"] = searchLookUpEdit3.EditValue?.ToString(),
                ["TenKH"] = searchLookUpEdit3.Text,
                ["MaHang"] = searchLookUpEdit4.EditValue?.ToString(),
                ["TenHang"] = searchLookUpEdit4.Text,
                ["MaDot"] = searchLookUpEdit2.EditValue?.ToString(),
                ["Dot"] = searchLookUpEdit2.Text,
                ["MaDH"] = searchLookUpEdit1.EditValue?.ToString(),
                ["TenDH"] = searchLookUpEdit1.Text,
                //["srcIndentType"] = srcIndentType.EditValue,
                //["TongSoLuong"] = tsl,
            };
        }
        public DataTable shareVTTable()
        {
            return gridControl1.DataSource as DataTable;
        }
        private void loadIndentTypeOption()
        {
            LoaiHang.Properties.Items.AddRange(new string[] { "NPL Sản xuất" });//, "NPL", "Máy móc thiết bị", "Văn phòng phẩm" });
        }
        private void loadIndentSrcTypeOption()
        {
            srcIndentType.Properties.Items.AddRange(new string[] { "Khách hàng" });//, "Mã hàng", "Lệnh SX" });
        }
        private void loadProductPerposeTypeOption()
        {
            mucDich.Properties.Items.AddRange(new string[] { "Sản xuất đại trà", "Sản xuất hàng mẫu" });
        }
        private void loadToUpdate()
        {
            string srcInfo = currentRow["SrcName"]?.ToString() ?? "--";
            var srcParts = srcInfo.Split(new string[] { "--" }, StringSplitOptions.None);
            searchLookUpEdit1.Text = srcParts.Length > 0 ? srcParts[1] : "";
            srcIndentType.EditValue = srcParts.Length > 0 ? srcParts[0] : null;

            string LoaiHangInfo = currentRow["LoaiHang"] == DBNull.Value ? "--" : currentRow["LoaiHang"].ToString();
            var LoaiHangParts = LoaiHangInfo.Split(new string[] { "--" }, StringSplitOptions.None);
            LoaiHang.EditValue = LoaiHangParts.Length > 0 ? LoaiHangParts[0] : null;
            mucDich.EditValue = LoaiHangParts.Length > 0 ? LoaiHangParts[1] : null;
        }
        private void loadGridControl1()
        {
            gridView1.Columns["TVGiaCong"].OptionsColumn.AllowEdit = true;
        }       
        private void loadGridControl2()
        {
            getDataGridControl2();
            gridControl2Table = getToGridControl2Table.Copy();
            gridControl2.DataSource = gridControl2Table;
            gridView2.ExpandAllGroups();
        }
        private void getDataGridControl2()
        {
            getToGridControl2Table = null;
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getdinhmucvattu";
            request.Parameter = searchLookUpEdit1.EditValue.ToString();
            request.Parameter1 = searchLookUpEdit2.EditValue.ToString();
            string urlGetListDataTable = URL + "XLVT/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            getToGridControl2Table = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            RepositoryItemCheckEdit checkEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            gridControl2.RepositoryItems.Add(checkEdit);
            if (!getToGridControl2Table.Columns.Contains("IsSelected"))
                getToGridControl2Table.Columns.Add("IsSelected", typeof(bool));
            if (!getToGridControl2Table.Columns.Contains("LoaiVT"))
                getToGridControl2Table.Columns.Add("LoaiVT");
            if (!getToGridControl2Table.Columns.Contains("TVGiaCong"))
                getToGridControl2Table.Columns.Add("TVGiaCong");
            if (!getToGridControl2Table.Columns.Contains("GridControl2"))
                getToGridControl2Table.Columns.Add("GridControl2", typeof(bool));
            foreach (DataRow row in getToGridControl2Table.Rows)
            {
                row["GridControl2"] = true;
                row["IsSelected"] = false;
                if (bool.Parse(row["NPL"].ToString()))
                {
                    row["LoaiVT"] = "Nguyên liệu";
                }
                else
                {
                    row["LoaiVT"] = "Phụ liệu";
                }
            }
            GridColumn colCheck = gridView2.Columns["IsSelected"];
            colCheck.ColumnEdit = checkEdit;
        }
        private void loadSearchLookupEdit1()
        {
            DataTable dt = getDonHang();
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0) return;
            searchLookUpEdit1.Properties.DataSource = dt;
            searchLookUpEdit1.Properties.DisplayMember = "MaDH";
            searchLookUpEdit1.Properties.ValueMember = "MaDH";
            searchLookUpEdit1.Properties.PopulateViewColumns();

            GridView view = searchLookUpEdit1.Properties.View;
            view.Columns["MaDH"].Caption = "Đơn hàng";
            view.Columns["TenHang"].Caption = "Tên hàng";
            view.Columns["Dot"].Caption = "Season";
            view.Columns["SoLuong"].Caption = "Số lượng";

            view.OptionsFind.AlwaysVisible = true;
        }
        private DataTable getDonHang()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getdonhang";
            request.Parameter = searchLookUpEdit3.EditValue.ToString();
            request.Parameter1 = searchLookUpEdit4.EditValue.ToString();
            string urlGetListDataTable = URL + "XLVT/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DataTable donHangTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            return donHangTable;
        }
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            loadSearchLookupEdit2();
            gridControl1.DataSource = null;
        }
        private void loadSearchLookupEdit2()
        {
            DataTable dt = getDot();
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0) return;
            searchLookUpEdit2.Properties.DataSource = dt;
            searchLookUpEdit2.Properties.DisplayMember = "Dot";
            searchLookUpEdit2.Properties.ValueMember = "MaDot";
            searchLookUpEdit2.Properties.PopulateViewColumns();

            GridView view = searchLookUpEdit2.Properties.View;
            view.Columns["MaDot"].Visible = false;
            view.Columns["Dot"].Caption = "Đợt";

            // bật Find Panel để search nhanh
            view.OptionsFind.AlwaysVisible = true;
        }
        private DataTable getDot()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getdot";
            request.Parameter = searchLookUpEdit1.EditValue.ToString();
            string urlGetListDataTable = URL + "XLVT/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DataTable dotTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            return dotTable;
        }
        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            loadGridControl2();
            gridControl1.DataSource = null;
        }
        private void loadSearchLookupEdit3()
        {
            DataTable dt = getKH();
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0) return;
            searchLookUpEdit3.Properties.DataSource = dt;
            searchLookUpEdit3.Properties.DisplayMember = "TenKH";
            searchLookUpEdit3.Properties.ValueMember = "MaKH";
            searchLookUpEdit3.Properties.PopulateViewColumns();

            GridView view = searchLookUpEdit3.Properties.View;
            view.Columns["MaKH"].Visible = false;
            view.Columns["TenKH"].Caption = "Tên khách hàng";

            // bật Find Panel để search nhanh
            view.OptionsFind.AlwaysVisible = true;
        }
        private DataTable getKH()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getkh";
            request.Parameter = searchLookUpEdit1.EditValue.ToString();
            string urlGetListDataTable = URL + "XLVT/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DataTable dotTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            return dotTable;
        }
        private void searchLookUpEdit3_EditValueChanged(object sender, EventArgs e)
        {
            loadSearchLookupEdit4();
            gridControl1.DataSource = null;
        }
        private void loadSearchLookupEdit4()
        {
            DataTable dt = getMH();
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0) return;
            searchLookUpEdit4.Properties.DataSource = dt;
            searchLookUpEdit4.Properties.DisplayMember = "TenHang";
            searchLookUpEdit4.Properties.ValueMember = "MaHang";
            searchLookUpEdit4.Properties.PopulateViewColumns();

            GridView view = searchLookUpEdit4.Properties.View;
            view.Columns["MaHang"].Visible = false;;
            view.Columns["TenHang"].Caption = "Tên hàng";

            // bật Find Panel để search nhanh
            view.OptionsFind.AlwaysVisible = true;
        }
        private DataTable getMH()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getmh";
            request.Parameter = searchLookUpEdit3.EditValue.ToString();
            string urlGetListDataTable = URL + "XLVT/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DataTable dotTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            return dotTable;
        }
        private void searchLookUpEdit4_EditValueChanged(object sender, EventArgs e)
        {
            loadSearchLookupEdit1();
            gridControl1.DataSource = null;
        }
        private void mucDich_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
        private void gridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            //GridView view = sender as GridView;
            //GridHitInfo hitInfo = e.HitInfo;
            //if (hitInfo.InColumnPanel)
            //{
            //    e.Menu.Items.Clear();
            //}
            //else if (hitInfo.InRow && hitInfo.RowHandle >= 0)
            //{
            //    e.Menu.Items.Clear();
            //}
        }              
        private void gridView2_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            //GridView view = sender as GridView;
            //GridHitInfo hitInfo = e.HitInfo;

            //if (hitInfo.InGroupRow)
            //{
            //    e.Menu.Items.Clear();
            //    int level = view.GetRowLevel(hitInfo.RowHandle);

            //    if (level == 1) // chỉ xử lý group con "TenSize"
            //    {
            //        string tenSizeValue = view.GetGroupRowValue(hitInfo.RowHandle, view.Columns["TenSize"])?.ToString();
            //        int parentHandle = view.GetParentRowHandle(hitInfo.RowHandle);
            //        string dotValue = view.GetGroupRowValue(parentHandle, view.Columns["Dot"])?.ToString();
            //        e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Chọn", (s, ev) =>
            //        {
            //            selectDinhMuc(tenSizeValue, dotValue);
            //        }));
            //    }

            //}
            //else if (hitInfo.InRow && hitInfo.RowHandle >= 0)
            //{
            //    e.Menu.Items.Clear();
            //    DataRow row = view.GetDataRow(hitInfo.RowHandle);
            //    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Chọn", (s, ev) =>
            //    {
            //        selectDinhMuc("", "", row);
            //    }));
            //}
        }
        private void selectDinhMuc(string groupValue, string parent, DataRow selectOneRow = null)
        {
            DataTable dt = gridControl2.DataSource as DataTable;
            DataTable dt_target = gridControl1.DataSource as DataTable;
            if (dt_target == null || dt_target.Columns.Count == 0 || dt_target.Rows.Count == 0)
            {
                // nếu chưa có thì clone từ dt
                dt_target = dt.Clone();
                if (!dt_target.Columns.Contains("TVGiaCong"))
                    dt_target.Columns.Add("TVGiaCong");
            }

            if (selectOneRow == null)
            {
                var selectedRows = dt.AsEnumerable()
                .Where(r => r.Field<string>("TenSize") == groupValue &&
                        r.Field<string>("Dot") == parent);
                foreach (var row in selectedRows)
                {
                    DataRow newRow = dt_target.NewRow();
                    foreach (var col in dt.Columns.Cast<DataColumn>())
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    dt_target.Rows.Add(newRow);
                    tsl = XuLyVTUnits.SmartTryParse<int>(row["SoLuongTong"]);
                }
            }
            else
            {
                DataRow newRow = dt_target.NewRow();
                foreach (var col in dt.Columns.Cast<DataColumn>())
                {
                    newRow[col.ColumnName] = selectOneRow[col.ColumnName];
                }
                dt_target.Rows.Add(newRow);
                tsl = XuLyVTUnits.SmartTryParse<int>(selectOneRow["SoLuongTong"]);
            }
            
            foreach (DataRow row in dt_target.Rows)
            {
                row["LoaiVatTu"] = "NPL";
            }

            gridControl1.DataSource = dt_target;
            gridView1.ExpandAllGroups();
        }       
        private void LoaiHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (LoaiHang.SelectedIndex)
            {
                case 0:
                    //layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    break;

                case 1:
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    break;

                case 2:
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    break;
                case 3:
                    layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    break;
            }
        }
        private void resetGridControl1Btn_Click(object sender, EventArgs e)
        {
            gridControl1.DataSource = null;
        }
        private void selectBtn_Click(object sender, EventArgs e)
        {
            gridView2.CloseEditor();
            gridView2.UpdateCurrentRow();
            addToGridcontrol1();
        }
        private void addToGridcontrol1()
        {
            DataTable dt = gridControl2.DataSource as DataTable;
            if (dt == null || dt.Rows.Count <= 0) return;
            DataRow[] checkedRows = dt.Select("IsSelected = True");
            DataTable dt_target = gridControl1.DataSource as DataTable;
            if (dt_target == null || dt_target.Columns.Count == 0 || dt_target.Rows.Count == 0)
            {
                // nếu chưa có thì clone từ dt
                dt_target = dt.Clone();
                if (!dt_target.Columns.Contains("LoaiVT"))
                    dt_target.Columns.Add("LoaiVT");
                if (!dt_target.Columns.Contains("TVGiaCong"))
                    dt_target.Columns.Add("TVGiaCong");
            }
            foreach (DataRow row in checkedRows)
            {
                DataRow newRow = dt_target.NewRow();
                foreach (var col in dt.Columns.Cast<DataColumn>())
                {
                    newRow[col.ColumnName] = row[col.ColumnName];
                }
                dt_target.Rows.Add(newRow);
                tsl = XuLyVTUnits.SmartTryParse<int>(row["SoLuongTong"]);
            }
            gridControl1.DataSource = dt_target;
            gridView1.ExpandAllGroups();
        }
        private void deSelectBtn_Click(object sender, EventArgs e)
        {
            removeFromGridControl1();
            gridControl1.RefreshDataSource();
        }
        private void removeFromGridControl1()
        {
            DataTable dt = gridControl1.DataSource as DataTable;
            DataRow[] checkedRows = dt.Select("IsSelected = True");
            foreach (DataRow row in checkedRows)
            {
                row.Delete(); // hoặc gridControl2Table.Rows.Remove(row);
            }

            // Nếu dùng Delete thì cần AcceptChanges để commit
            dt.AcceptChanges();
        }
        private void checkAllBtn_Click(object sender, EventArgs e)
        {
            DataTable dt = gridControl2.DataSource as DataTable;
            if (!isCheckAll)
            {
                foreach (DataRow row in dt.Rows)
                {
                    row["IsSelected"] = true;
                }
            }
        }
        private void resetGridControl2Btn_Click(object sender, EventArgs e)
        {
            DataTable dt = gridControl2.DataSource as DataTable;
            if (!isCheckAll)
            {
                foreach (DataRow row in dt.Rows)
                {
                    row["IsSelected"] = false;
                }
            }
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
        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
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
