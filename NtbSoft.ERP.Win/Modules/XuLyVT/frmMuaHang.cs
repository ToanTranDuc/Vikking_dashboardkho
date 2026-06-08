using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.XuLyVT;
using NtbSoft.ERP.Win.Modules.ThuVien;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.MuaHang
{
    public partial class frmMuaHang : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        KeyDownControlHandler keyDownControlHandler;

        private XtraUserControl currentUC;
        private DataRow supplyRow;
        private DataRow indentRow;
        private int clickIndent;
        private bool isLoad;
        private DataTable gridControl1CopyTable;
        public frmMuaHang(DataRow row = null, DataRow rt = null, int clickindent = 0)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            supplyRow = row;
            indentRow = rt;
            clickIndent = clickindent;
            isLoad = row != null;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            SplashScreenManager.ShowForm(this, typeof(frmLoading), true, true, false);
            try
            {
                this.WindowState = FormWindowState.Maximized;
                loadGridControl1();
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }
        private void loadGridControl1()
        {            
            //gridView1.OptionsBehavior.Editable = true;
            //gridView1.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;

            //gridView1.Columns["TrangThai"].OptionsColumn.AllowEdit = false;
            //gridView1.Columns["TenMau"].OptionsColumn.AllowEdit = false;
            //gridView1.Columns["Size"].OptionsColumn.AllowEdit = false;
            //gridView1.Columns["supplyName"].OptionsColumn.AllowEdit = false;
            //gridView1.Columns["indentName"].OptionsColumn.AllowEdit = false;

            DataTable dt_target = getDataGridControl1();
            gridControl1CopyTable = dt_target.Copy();
            //var groupColumns = new[] { "LoaiVT", "MaVT", "TenVT", "ChiTietVT",
            //    "MaMauVT", "TenMauVT", "MaSizeVT", "TenSizeVT", "MaDVVT", "TenDVVT",
            //    "DonGia", "TenNhaCungCap", "InSeamMauVT","TrangThai", "MaPMH", "TenPMH",
            //    "TGHTDuKien", "TGHT"
            //};   // cột để group
            //var sumColumns = new[] { "SL", "ChiPhiTong" };          // cột để sum

            //var grouped = dt_target.AsEnumerable()
            //    .GroupBy(r => string.Join("|", groupColumns.Select(c => r[c])))
            //    .Select(g =>
            //    {
            //        var dict = new Dictionary<string, object>();
            //        var first = g.First();

            //        // Giữ lại các cột group
            //        foreach (var col in groupColumns)
            //            dict[col] = first[col];

            //        // Tính tổng cho các cột sum
            //        foreach (var col in sumColumns)
            //            dict[col] = g.Sum(r => XuLyVTUnits.SmartTryParse<float>(r[col].ToString()));

            //        // Với các cột khác (không nằm trong groupColumns và sumColumns) thì cộng dồn chuỗi
            //        var otherColumns = dt_target.Columns.Cast<DataColumn>()
            //                .Select(c => c.ColumnName)
            //                .Where(c => !groupColumns.Contains(c) && !sumColumns.Contains(c));

            //        foreach (var col in otherColumns)
            //        {
            //            dict[col] = string.Join(",", g.Select(r => r[col]?.ToString()).Where(s => !string.IsNullOrEmpty(s)));
            //        }

            //        return dict;
            //    })
            //    .ToList();

            // Tạo DataTable kết quả
            //DataTable result = new DataTable();

            //// Tạo cột group
            //foreach (var col in groupColumns)
            //{
            //    if (col == "TGHTDuKien" || col == "TGHT")
            //        result.Columns.Add(col, typeof(DateTime));
            //    else
            //        result.Columns.Add(col);
            //}

            //// Tạo cột sum
            //foreach (var col in sumColumns)
            //    result.Columns.Add(col, typeof(float)); // dùng float vì bạn đang SmartTryParse<float>

            //// Tạo cột khác (chuỗi)
            //var otherCols = dt_target.Columns.Cast<DataColumn>()
            //                .Select(c => c.ColumnName)
            //                .Where(c => !groupColumns.Contains(c) && !sumColumns.Contains(c));

            //foreach (var col in otherCols)
            //    result.Columns.Add(col, typeof(string));

            //foreach (var item in grouped)
            //{
            //    var row = result.NewRow();
            //    foreach (var col in groupColumns)
            //    {
            //        if (item[col] == null || item[col] == DBNull.Value || string.IsNullOrWhiteSpace(item[col].ToString()))
            //        {
            //            row[col] = DBNull.Value;
            //        }
            //        else if (col == "TGHTDuKien" || col == "TGHT")
            //        {
            //            var ng = item[col];
                        
            //            DateTime dt;
            //            if (DateTime.TryParse(item[col].ToString(),
            //                CultureInfo.GetCultureInfo("en-US"),
            //                DateTimeStyles.None,
            //                out dt))
            //            {
            //                row[col] = dt;
            //            }
            //            else
            //            {
            //                row[col] = DBNull.Value; // fallback nếu chuỗi không hợp lệ
            //            }
            //        }
            //        else
            //        {
            //            row[col] = item[col];
            //        }
            //    }

            //    foreach (var col in sumColumns)
            //        row[col] = item[col];
            //    foreach (var col in otherCols)
            //        row[col] = item[col];
            //    result.Rows.Add(row);
            //}
            // Bind lại vào gridControl2
            gridControl1.DataSource = gridControl1CopyTable;
            gridView1.ExpandAllGroups();
        }
        private DataTable getDataGridControl1()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getall";
            //if (supplyRow == null) return new DataTable();
            //if (clickIndent == 0)
            //{
            //    request.Parameter = supplyRow["ID"].ToString();
            //    request.Action = "getbysupply";
            //}
            //else
            //{
            //    request.Parameter = indentRow["ID"].ToString();
            //    request.Action = "getbyindent";
            //}
            string urlGetListDataTable = URL + "XLVTMUAHANG/Get";
            string jsonRequest = JsonConvert.SerializeObject(request);
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            //if (dt == null || dt.Columns.Count == 0)
            //{
            //    DataTable newdt = XLVTMUAHANGTable.create();
            //    newdt.Columns.Add("canChange", typeof(bool));
            //    newdt.Columns.Add("isNew", typeof(bool));
            //    foreach (DataRow row in newdt.Rows)
            //    {
            //        row["canChange"] = false;
            //        row["isNew"] = false;
            //    }
            //    return newdt;
            //}
            //dt.Columns.Add("canChange", typeof(bool));
            //dt.Columns.Add("isNew", typeof(bool));
            //foreach (DataRow row in dt.Rows)
            //{
            //    row["canChange"] = false;
            //    row["isNew"] = false;
            //}
            return dt;
        }
        private void gridView1_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (e.RowHandle < 0) return; // bỏ qua new item row nếu chưa nhập

            if (e.Column.FieldName == "rmpoSupplier") // cột A bạn muốn
            {
                var row = gridView1.GetDataRow(e.RowHandle);
                using (var frm = new frmNhaCungCap())
                {
                    frm.ShowDialog();
                }
            }
        }
        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo hitInfo = e.HitInfo;
            
            if (hitInfo.InGroupRow)
            {
                e.Menu.Items.Clear();
                string groupValue = view.GetGroupRowValue(hitInfo.RowHandle)?.ToString();
                e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Thêm mới", (s, ev) =>
                {
                    addNewRow(groupValue, 1);
                }));
                e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Xoá", (s, ev) =>
                {
                    
                }));
            }
            else if (hitInfo.InColumnPanel)
            {
                if (view.DataRowCount == 0)
                {
                    e.Menu.Items.Clear();
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Thêm mới", (s, ev) =>
                    {
                        if (clickIndent == 0)
                        {
                            addNewRow(supplyRow["ID"].ToString(), 1);
                        }
                        else
                        {
                            addNewRowBySupply(indentRow["ID"].ToString());
                        }
                    }));
                }
                
            }
            else if (hitInfo.InRow && hitInfo.RowHandle >= 0)
            {
                e.Menu.Items.Clear();
                DataRow row = view.GetDataRow(hitInfo.RowHandle);
                bool canChange = row != null && row["canChange"] != DBNull.Value  && (bool)row["canChange"];
                if (canChange)
                {
                    string fieldName = view.FocusedColumn.FieldName;
                    if(fieldName == "supplierName")
                    {
                        e.Allow = false;
                        using (frmNhaCungCap frmNC = new frmNhaCungCap())
                        {
                            if(frmNC.ShowDialog() == DialogResult.OK)
                            {

                            }
                        }
                        
                    }
                    else
                    {
                        //e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Lưu", (s, ev) =>
                        //{
                        //    saveChangeAndNewRow();
                        //}));
                        e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Xoá", (s, ev) =>
                        {

                        }));
                        //e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Xoá tất cả hàng vừa thêm mới", (s, ev) =>
                        //{

                        //}));
                    }
                }
                else
                {
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Sửa", (s, ev) =>
                    {
                        row["canChange"] = true;
                    }));
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Xoá", (s, ev) =>
                    {

                    }));
                }
            }
        }
        private void addNewRow(string groupValue, int useGlobal)
        {
            DataTable dt = gridControl1.DataSource as DataTable;

            // Tìm vị trí đầu tiên của group
            int insertIndex = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["ID"].ToString() == groupValue)
                {
                    insertIndex = i;
                    break;
                }
            }
            if (useGlobal == 0 || supplyRow == null)
            {
                var request = XuLyVTRequestGet.createDefault();
                request.Action = "getbyid";
                request.Parameter = groupValue;
                string urlGetListDataTable = URL + "XLVTItem/Get";
                string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
                DataTable supplyTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
                if (supplyTable == null || supplyTable.Columns.Count == 0 || supplyTable.Rows.Count == 0) return;
                supplyRow = supplyTable.Rows[0];
            }
            // Tạo dòng mới
            DataRow newRow = dt.NewRow();
            newRow["ID"] = XuLyVTUnits.generatedTimeKey("rmpo");
            //newRow["Ten"] = groupValue;
            newRow["indentID"] = supplyRow["indentID"] ?? DBNull.Value;
            newRow["indentName"] = supplyRow["indentName"] ?? DBNull.Value;
            newRow["supplyID"] = groupValue;
            newRow["supplyName"] = supplyRow["Ten"] ?? DBNull.Value;
            newRow["TrangThai"] = "Đang xử lý";
            newRow["ColorID"] = supplyRow["ColorID"] ?? DBNull.Value;
            newRow["TenMau"] = supplyRow["TenMau"] ?? DBNull.Value;
            newRow["SizeID"] = supplyRow["SizeID"] ?? DBNull.Value;
            newRow["Size"] = supplyRow["Size"] ?? DBNull.Value;
            newRow["createdAt"] = DateTime.Now;

            newRow["canChange"] = true;
            newRow["isNew"] = true;

            // Chèn vào vị trí đầu của group
            dt.Rows.InsertAt(newRow, insertIndex);
            gridView1.ExpandAllGroups();
        }
        private void addNewRowBySupply(string indentValue)
        {
            using(frmCheckVT frmCheck = new frmCheckVT(indentValue))
            {
                if(frmCheck.ShowDialog() == DialogResult.OK)
                {
                    DataTable selectedSupply = frmCheck.selectedSupply();
                    if (selectedSupply == null || selectedSupply.Columns.Count == 0 || selectedSupply.Rows.Count == 0) return;
                    foreach(DataRow row in selectedSupply.Rows)
                    {
                        addNewRow(row["ID"].ToString(), 0);
                    }
                }
            }
        }
        private void saveChangeAndNewRow()
        {
            DialogResult result = XtraMessageBox.Show(
                "Bạn có chắc chắn muốn lưu dữ liệu này không?",
                "Xác nhận lưu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (result != DialogResult.Yes)
                return;
            try
            {
                DataTable dt = gridControl1.DataSource as DataTable;
                var columnNames = dt.Columns.Cast<DataColumn>()
                            .Where(c => c.ColumnName != "isNew" || c.ColumnName != "canChange")
                            .Select(c => c.ColumnName)
                            .ToArray();

                var newRows = dt.AsEnumerable()
                .Where(r => r.Field<bool>("isNew") == true);
                var requestPost = XuLyVTRequestPost.createDefault("XLVTRMPO");
                foreach (var row in newRows)
                {
                    DataRow newRow = requestPost.TypeTable.NewRow();
                    foreach (var col in requestPost.TypeTable.Columns.Cast<DataColumn>())
                    {
                        newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    requestPost.TypeTable.Rows.Add(newRow);
                }
                requestPost.Action = "POST";
                string urlGetListDataTable = URL + "XLVTRMPO/Post";
                string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, requestPost); }).Result;

                var changeRows = dt.AsEnumerable()
                .Where(r => r.Field<bool>("isNew") == false && r.Field<bool>("canChange") == true);
                var requestUpdate = XuLyVTRequestPost.createDefault("XLVTRMPO");
                foreach (var row in changeRows)
                {
                    DataRow changeRow = requestUpdate.TypeTable.NewRow();
                    foreach (var col in requestUpdate.TypeTable.Columns.Cast<DataColumn>())
                    {
                        changeRow[col.ColumnName] = row[col.ColumnName];
                    }
                    requestUpdate.TypeTable.Rows.Add(changeRow);
                }
                requestUpdate.Action = "UPDATE";
                jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, requestUpdate); }).Result;
                
                XtraMessageBox.Show("Thêm đơn hàng thành công!");
                //this.DialogResult = DialogResult.OK;
                loadGridControl1();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            //GridView view = sender as GridView;
            //DataRow row = view.GetDataRow(view.FocusedRowHandle);

            //bool isNewRow = row != null && row["canChange"] != DBNull.Value && (bool)row["canChange"];
            //e.Cancel = !isNewRow;
        }
        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            //try
            //{
            //    if (e.Column == null) return;
            //    Rectangle rect = e.Bounds;
            //    ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            //    Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 204, 102), Color.FromArgb(255, 204, 102), e.Column.AppearanceHeader.GradientMode);
            //    rect.Inflate(-1, -1);
            //    e.Graphics.FillRectangle(brush, rect);

            //    // Thiết lập font và màu chữ
            //    Font font = new Font(e.Appearance.Font, FontStyle.Bold);
            //    StringFormat stringFormat = new StringFormat();
            //    stringFormat.Alignment = StringAlignment.Center;
            //    stringFormat.LineAlignment = StringAlignment.Center;
            //    SolidBrush textColorBrush = new SolidBrush(Color.FromArgb(0, 17, 102)); // Màu chữ #001166

            //    // Vẽ chữ
            //    e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);

            //    foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            //    {
            //        DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            //    }
            //    e.Handled = true;
            //}
            //catch (Exception ex)
            //{

            //}
        }
        private void gridView1_RowStyle(object sender, RowStyleEventArgs e)
        {
            //bool canChange = Convert.ToBoolean(gridView1.GetRowCellValue(e.RowHandle, "canChange")?.ToString());
            //e.Appearance.ForeColor = ColorTranslator.FromHtml("#1A0000");
            //e.HighPriority = true;
            //if (gridView1.FocusedRowHandle == e.RowHandle)
            //{
            //    e.Appearance.BackColor = ColorTranslator.FromHtml("#80EAFF");
            //}
            //else if (canChange)
            //{
            //    e.Appearance.BackColor = ColorTranslator.FromHtml("#FFFF99");

            //}
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
        private void createNewRMPOBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (frmTaoPhieuMuaHang frm = new frmTaoPhieuMuaHang())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    loadGridControl1();
                }
            }
        }
    }
}