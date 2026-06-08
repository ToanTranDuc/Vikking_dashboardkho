using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
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
    public partial class frmXuLyVatTu : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        KeyDownControlHandler keyDownControlHandler;

        private DataTable gridControl1Table;
        private DataTable gridControl2Table;
        public frmXuLyVatTu()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            loadGridControl1();            
        }
        private void addNewIndent_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using(frmTaoPhieuXLVT frmAddXLVT = new frmTaoPhieuXLVT())
            {
                if(frmAddXLVT.ShowDialog() == DialogResult.OK)
                {
                    loadGridControl1();
                }
            }
                
        }
        private void loadGridControl1(string action = null, Dictionary<string, string> param = null, DataTable typetable = null)
        {
            getDataGridControl1();
            DataTable dt = gridControl1Table.Copy();
            foreach(DataRow row in dt.Rows)
            {
                string tenNguon = row["TenNguon"].ToString();
                if (!string.IsNullOrWhiteSpace(tenNguon))
                {
                    var parts = tenNguon.Split(new string[] { "||" }, StringSplitOptions.None);
                    int takeCount = Math.Min(3, parts.Length);
                    tenNguon = string.Join(" / ", parts.Take(takeCount));
                }
                else
                {
                    tenNguon = "";
                }
                row["TenNguon"] = tenNguon;
            }
            gridControl1.DataSource = dt;
        }
        private void getDataGridControl1(string action = null, Dictionary<string, string> param = null, DataTable typetable = null)
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = action ?? "getall";
            string urlGetListDataTable = URL + "XLVT/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            gridControl1Table = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

        }       
        private void loadGridControl2(string action = null, Dictionary<string, string> param = null, DataTable typetable = null)
        {
            gridControl2.DataSource = getDataGridControl2(action, param, typetable);
            gridView3.ExpandAllGroups();
        }
        private DataTable getDataGridControl2(string action = null, Dictionary<string, string> param = null, DataTable typetable = null)
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = action ?? "getall";
            if (action == "getbyindent")
            {
                request.Parameter = param["MaPXLVT"];
            }
            string urlGetListDataTable = URL + "XLVTVT/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            gridControl2Table = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
            return gridControl2Table;
        }
        private void gridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo hitInfo = e.HitInfo;
            if (hitInfo.InRow && hitInfo.RowHandle >= 0)
            {
                e.Menu.Items.Clear();
                int rowHandle = e.HitInfo.RowHandle;
                view.FocusedRowHandle = rowHandle;
                string fieldName = view.FocusedColumn.FieldName;
                DataRow parentRow = view.GetDataRow(hitInfo.RowHandle);
                if (parentRow == null) return;
                DataRow childRow = null;
                if (fieldName == "SoDonMuaHang")
                {
                    e.Allow = false;
                    using (frmMuaHang newRMPO = new frmMuaHang(childRow, parentRow, 1))
                    {
                        var result = newRMPO.ShowDialog();
                        if (result == DialogResult.Cancel)
                        {
                            loadGridControl1();
                        }

                    }
                }
                else
                {
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Sửa", (s, ev) =>
                    {
                        editRowGridView1(view, rowHandle);
                    }));
                    e.Menu.Items.Add(new DevExpress.Utils.Menu.DXMenuItem("Xóa", (s, ev) =>
                    {
                        deleteRowGridView1(view, rowHandle);
                    }));
                }
                    
            }
        }
        private void editRowGridView1(GridView view, int rowHandle)
        {
            if(rowHandle < 0) return;
            DataRow row = view.GetDataRow(rowHandle);
            if (row == null) return;

            using (frmTaoPhieuXLVT editXLVT = new frmTaoPhieuXLVT(row))
            {
                if (editXLVT.ShowDialog() == DialogResult.OK)
                {
                    loadGridControl1();
                }
            }
        }
        private void deleteRowGridView1(GridView view, int rowHandle)
        {
            if (rowHandle < 0) return;
            DataRow row = view.GetDataRow(rowHandle);
            if (row == null) return;
        }
        private void gridView2_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            var childView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (childView == null || e.RowHandle < 0) return;

            DataRow childRow = childView.GetDataRow(e.RowHandle);
            if (childRow == null) return;
            DataRow parentRow = childRow.GetParentRow("Danh sách vật tư");
            if (parentRow == null) return;
            string columnName = e.Column?.FieldName;
            object cellValue = childView.GetRowCellValue(e.RowHandle, e.Column);

            //MessageBox.Show($"Parent ID: {parentRow?["ID"]}\nChild ID: {childRow["ID"]}\nCột: {columnName}, Giá trị: {cellValue}");
            if(columnName == "SLMuaHang")
            {
                using (frmMuaHang newRMPO = new frmMuaHang(childRow, parentRow))
                {
                    var result = newRMPO.ShowDialog();
                    if (result == DialogResult.Cancel)
                    {
                        loadGridControl1();
                    }
                    
                }
            }
            // Mở form chỉnh sửa chẳng hạn:
            // var frm = new frmAddXLVT(childRow, parentRow);
            // frm.ShowDialog();
        }
        private void gridControl1_ViewRegistered(object sender, DevExpress.XtraGrid.ViewOperationEventArgs e)
        {
            GridView view = e.View as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view != null && view.LevelName == "Danh sách vật tư")
            {
                // Gắn sự kiện cho detail view instance
                view.RowCellClick -= gridView2_RowCellClick;
                view.RowCellClick += gridView2_RowCellClick;
            }
        }
        private void addNewIndent_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (frmTaoPhieuXLVT frmAddXLVT = new frmTaoPhieuXLVT())
            {
                if (frmAddXLVT.ShowDialog() == DialogResult.OK)
                {
                    loadGridControl1();
                }
            }
        }
        private void gridView1_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            //var childView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            //if (childView == null || e.RowHandle < 0) return;

            //DataRow parentRow = childView.GetDataRow(e.RowHandle);
            //if (parentRow == null) return;
            //DataRow childRow = null;
            //string columnName = e.Column?.FieldName;
            //object cellValue = childView.GetRowCellValue(e.RowHandle, e.Column);

            ////MessageBox.Show($"Parent ID: {parentRow?["ID"]}\nChild ID: {childRow["ID"]}\nCột: {columnName}, Giá trị: {cellValue}");
            //if (columnName == "SoDonMuaHang")
            //{
            //    using (frmMuaHang newRMPO = new frmMuaHang(childRow, parentRow, 1))
            //    {
            //        var result = newRMPO.ShowDialog();
            //        if (result == DialogResult.Cancel)
            //        {
            //            loadGridControl1();
            //        }

            //    }
            //}
            GridView view = sender as GridView;
            DataRow row = view.GetDataRow(e.RowHandle);
            Dictionary<string, string> para = new Dictionary<string, string>
            {
                ["MaPXLVT"] = row["MaPXLVT"].ToString()
            };
            loadGridControl2("getbyindent", para);
        }
        private void gridView2_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 204, 102), Color.FromArgb(255, 204, 102), e.Column.AppearanceHeader.GradientMode);
                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);

                // Thiết lập font và màu chữ
                Font font = new Font(e.Appearance.Font, FontStyle.Bold);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                SolidBrush textColorBrush = new SolidBrush(Color.FromArgb(0, 17, 102)); // Màu chữ #001166

                // Vẽ chữ
                e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);

                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

            }
        }
        private void openFrmRMPOBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (frmMuaHang frm = new frmMuaHang())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {

                }
            }
        }
        private void gridView3_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
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