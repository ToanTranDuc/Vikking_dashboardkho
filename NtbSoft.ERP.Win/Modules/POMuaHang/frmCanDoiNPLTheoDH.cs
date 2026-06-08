using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmCanDoiNPLTheoDH : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        KeyDownControlHandler keyDownControlHandler;

        private string maDH;
        private string lenhSX;
        private string maLenh;

        private DataTable gridControl1Table;
        private DataTable gridControl2Table;
        private DataTable gridControl2TableCopy;
        private DataTable lenhTable;
        private DataTable vattuTable;
        private DataTable ngayDBTable;
        private DataTable gridControl2SnapshotKhGui;
        private Point startPoint;
        private bool isDragging = false;
        private GridColumn dragColumn;
        private GridColumn startColumn = null;
        private int startRowHandle = GridControl.InvalidRowHandle;

        public frmCanDoiNPLTheoDH(string donhangText = "", string lenhsxText = "", string malenhText = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            this.maDH = donhangText;
            this.lenhSX = lenhsxText;
            this.maLenh = malenhText;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.WindowState = FormWindowState.Maximized;

            getDonHang();
            calcLenhSX();
            loadGridControl1();
            getNgayDongBoDK();
            loadNgayDongBoDK();
            if (gridView1.RowCount > 0)
            {
                gridView1.FocusedRowHandle = 0;
                handleRowAction(0);
                getVatTu();
                calcVatTu();
                loadGridControl2();
                LoadDHKHGui();
                autoCanDoiNPL();
            }
        }


        private void loadGridControl1()
        {
            gridControl1.DataSource = gridControl1Table;
            foreach (GridColumn col in gridView1.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
        }
        private void getDonHang()
        {
            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "getthongtindonhang";
            request.Parameter = maDH;
            string urlGetListDataTable = URL + "CanDoiNPL/GetByTypeTable";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            lenhTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!lenhTable.Columns.Contains("generatedID"))
                lenhTable.Columns.Add("generatedID");
            foreach (DataRow row in lenhTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");
            }
        }
        private void calcLenhSX()
        {
            DataTable dt = lenhTable.Copy();
            gridControl1Table = lenhTable.Clone();
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count == 0) return;
            //foreach (DataRow row in dt.Rows)
            //{

            //}
            gridControl1Table = dt.Copy();
        }
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            handleRowAction(e.FocusedRowHandle);
            getVatTu();
            calcVatTu();
            loadGridControl2();
            LoadDHKHGui();
            autoCanDoiNPL();
        }
        private void handleRowAction(int rowHandle)
        {
            var row = gridView1.GetDataRow(rowHandle);
            if (row == null) return;
            //lenhSX = row["MaLenh"]?.ToString();
        }


        private void loadGridControl2()
        {
            gridControl2.DataSource = gridControl2Table;
            foreach (GridColumn col in gridView2.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            gridView2.Columns["SLCanDoiKho"].OptionsColumn.AllowEdit = true;
            gridView2.Columns["SLMuaThem"].OptionsColumn.AllowEdit = true;
            gridView2.Columns["TyLeMuaThem"].OptionsColumn.AllowEdit = true;
            RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();
            textEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            textEdit.Mask.EditMask = "n2"; // 2 số thập phân
            textEdit.Mask.UseMaskAsDisplayFormat = true;
            textEdit.KeyPress += (s, e) =>
            {
                if (e.KeyChar == '-') // chặn dấu trừ
                    e.Handled = true;
            };
            gridView2.Columns["SLNhuCau"].ColumnEdit = textEdit;
            gridView2.Columns["TonKho"].ColumnEdit = textEdit;
            gridView2.Columns["SLTonKhoSauCanDoi"].ColumnEdit = textEdit;
            gridView2.Columns["SLCanDoiKho"].ColumnEdit = textEdit;
            gridView2.Columns["SLMuaThem"].ColumnEdit = textEdit;
            gridView2.Columns["TyLeMuaThem"].ColumnEdit = textEdit;
            gridView2.Columns["SLMuaThemTong"].ColumnEdit = textEdit;

        }
        private void getVatTu()
        {
            SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
            try
            {
                var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
                request.Action = "getvattucancandoi";
                DataRow newRow = request.TypeTable.NewRow();
                newRow["MaDH"] = maDH;
                request.TypeTable.Rows.Add(newRow);
                string urlGetListDataTable = URL + "CanDoiNPL/GetByTypeTable";
                string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
                vattuTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

                if (!vattuTable.Columns.Contains("generatedID"))
                    vattuTable.Columns.Add("generatedID");
                if (!vattuTable.Columns.Contains("SLNhuCau"))
                    vattuTable.Columns.Add("SLNhuCau", typeof(decimal));

                if (!vattuTable.Columns.Contains("TyLeMuaThem"))
                    vattuTable.Columns.Add("TyLeMuaThem", typeof(decimal));
                if (!vattuTable.Columns.Contains("TyLeDongBo"))
                    vattuTable.Columns.Add("TyLeDongBo", typeof(float));
                if (!vattuTable.Columns.Contains("IsNPLText"))
                    vattuTable.Columns.Add("IsNPLText", typeof(string));
                if (!vattuTable.Columns.Contains("SLTonKhoSauCanDoi"))
                    vattuTable.Columns.Add("SLTonKhoSauCanDoi", typeof(decimal));
                int asckey = 1;
                foreach (DataRow row in vattuTable.Rows)
                {
                    if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                    else row["IsNPLText"] = "Phụ liệu";
                    row["generatedID"] = XuLyVTUnits.generatedTimeKey("id") + asckey.ToString();
                    asckey++;
                }
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseDefaultWaitForm();
            }
            finally
            {
                SplashScreenManager.CloseDefaultWaitForm();
            }
        }
        private void calcVatTu()
        {
            DataTable dt = vattuTable.Copy();
            gridControl2Table = vattuTable.Clone();
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count == 0) return;
            //foreach (DataRow row in dt.Rows)
            //{
            //if (row["DinhMucHaoHut"] == DBNull.Value || row["DinhMucHaoHut"].ToString() == "") row["DinhMucHaoHut"] = 0;
            //if (row["DinhMucChung"] == DBNull.Value || row["DinhMucChung"].ToString() == "") row["DinhMucChung"] = 0;
            //float slCapPhat = XuLyVTUnits.SmartTryParse<float>(row["DinhMucChung"].ToString()) *
            //    XuLyVTUnits.SmartTryParse<float>(row["SoLuong"].ToString()) *
            //    (1 + XuLyVTUnits.SmartTryParse<float>(row["DinhMucHaoHut"].ToString()) / 100);
            //float slCanDoi = 0;
            //slCapPhat = (float)Math.Round(slCapPhat, 2, MidpointRounding.AwayFromZero);
            //row["SLNhuCau"] = slCapPhat;

            //float slMuaThemTong = XuLyVTUnits.SmartTryParse<float>(row["SLMuaThem"].ToString()) *
            //    (1 + XuLyVTUnits.SmartTryParse<float>(row["TyLeMuaThem"].ToString()) / 100);
            //row["SLMuaThemTong"] = slMuaThemTong;
            //}
            gridControl2Table = dt.Copy();
        }
        private void gridView2_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
            if (e.Column.FieldName == "SLMuaThemTong" && e.IsGetData)
            {
                DataRow row = ((DataRowView)e.Row).Row;
                decimal slMuaThemTong = XuLyVTUnits.SmartTryParse<decimal>(row["SLMuaThem"].ToString()) *
                    (1 + XuLyVTUnits.SmartTryParse<decimal>(row["TyLeMuaThem"].ToString()) / 100m);
                e.Value = slMuaThemTong;//a * (1 + b / 100m);
            }
        }
        private void gridView2_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.IsGroupRow(e.RowHandle))
            {
                // Luôn tô màu cho group row
                e.Appearance.BackColor = Color.WhiteSmoke;
                e.Appearance.ForeColor = Color.Black;
                e.HighPriority = true; // đảm bảo override
            }
            if (e.RowHandle >= 0) // chỉ áp dụng cho data row, không áp dụng cho group row
            {
                string haveMatch = Convert.ToString(view.GetRowCellValue(e.RowHandle, "Havematch"));
                string maPhieu = Convert.ToString(view.GetRowCellValue(e.RowHandle, "MaPhieu"));

                if (haveMatch == "yes")
                {
                    e.Appearance.BackColor = Color.LemonChiffon;   // màu nền
                    //e.Appearance.ForeColor = Color.Black;       // màu chữ
                }
            }
        }
        private void gridView2_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.FocusedRowHandle >= 0) // chỉ áp dụng cho data row
            {
                string haveMatch = Convert.ToString(view.GetRowCellValue(view.FocusedRowHandle, "Havematch"));
                string maPhieu = Convert.ToString(view.GetRowCellValue(view.FocusedRowHandle, "MaPhieu"));

                if (haveMatch == "yes" && maPhieu.Contains("group"))
                {
                    e.Cancel = true; // chặn không cho edit
                }
            }
        }
        private void gridView2_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.CellValue == null || e.CellValue == DBNull.Value) return;

            GridView view = sender as GridView;
            if (e.Column.FieldName == "TyLeMuaThem")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "TonKho")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "SLMuaThemTong")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "SLCanDoiKho")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "SLMuaThem")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }

            }
        }
        private void gridView2_MouseDown(object sender, MouseEventArgs e)
        {
            //var view = sender as GridView;
            //var hi = view.CalcHitInfo(e.Location);

            //isDragging = false;
            //startPoint = e.Location;
            //startColumn = null;
            //startRowHandle = GridControl.InvalidRowHandle;

            //// Chỉ quan tâm khi nhấn vào cell dữ liệu (không phải group/header)
            //if (hi.InRowCell && hi.Column != null && hi.RowHandle >= 0)
            //{
            //    if (e.Button == MouseButtons.Left)
            //    {
            //        startColumn = hi.Column;
            //        startRowHandle = hi.RowHandle;
            //    }
            //    else if (e.Button == MouseButtons.Right)
            //    {
            //        startColumn = hi.Column;
            //        startRowHandle = hi.RowHandle;
            //        // Đặt focus vào cell đó để người dùng thấy vị trí click
            //        view.FocusedRowHandle = hi.RowHandle;
            //        view.FocusedColumn = hi.Column;
            //    }
            //}
        }
        private void gridView2_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left && startColumn != null && startRowHandle >= 0)
            //{
            //    if (Math.Abs(e.X - startPoint.X) > SystemInformation.DoubleClickSize.Width ||
            //        Math.Abs(e.Y - startPoint.Y) > SystemInformation.DoubleClickSize.Height)
            //    {
            //        isDragging = true;
            //    }
            //}
        }
        private void gridView2_MouseUp(object sender, MouseEventArgs e)
        {
            //var view = sender as GridView;

            //try
            //{
            //    if (isDragging && startColumn != null && startRowHandle >= 0)
            //    {
            //        // Chỉ cho phép ở cột TyLeMuaThem
            //        if (startColumn.FieldName != "TyLeMuaThem")
            //            return;

            //        // Xác định cell kết thúc bằng hit info tại MouseUp
            //        var endHi = view.CalcHitInfo(e.Location);
            //        if (!(endHi.InRowCell && endHi.Column == startColumn && endHi.RowHandle >= 0))
            //            return; // kéo sang cột khác thì bỏ

            //        int endRowHandle = endHi.RowHandle;

            //        // Lấy giá trị từ ô bắt đầu kéo
            //        object startValue = view.GetRowCellValue(startRowHandle, startColumn);

            //        // Fill từ min..max (kéo lên hay kéo xuống đều xử lý)
            //        int minRow = Math.Min(startRowHandle, endRowHandle);
            //        int maxRow = Math.Max(startRowHandle, endRowHandle);

            //        view.BeginUpdate();
            //        for (int rh = minRow; rh <= maxRow; rh++)
            //        {
            //            // Bỏ qua group row
            //            if (!view.IsDataRow(rh)) continue;
            //            view.SetRowCellValue(rh, startColumn, startValue);
            //        }
            //    }
            //}
            //finally
            //{
            //    if (isDragging) (sender as GridView).EndUpdate();
            //    // Reset trạng thái
            //    isDragging = false;
            //    startColumn = null;
            //    startRowHandle = GridControl.InvalidRowHandle;
            //}
        }
        private void gridView2_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            var view = sender as GridView;

            // Chỉ hiện menu khi chuột phải trên cell của cột TyLeMuaThem
            if (e.HitInfo.InRowCell && e.HitInfo.Column != null && e.HitInfo.Column.FieldName == "TyLeMuaThem")
            {
                e.Menu.Items.Clear(); // bỏ menu mặc định nếu muốn

                // Fill tất cả
                var fillAllItem = new DXMenuItem("Áp dụng toàn cột", (o, args) =>
                {
                    int rowHandle = e.HitInfo.RowHandle;
                    var col = e.HitInfo.Column;
                    object value = view.GetRowCellValue(rowHandle, col);

                    view.BeginUpdate();
                    try
                    {
                        for (int i = 0; i < view.DataRowCount; i++)
                        {
                            int rh = view.GetRowHandle(i);
                            view.SetRowCellValue(rh, col, value);
                        }
                    }
                    finally { view.EndUpdate(); }
                });

                // Fill group
                var fillGroupItem = new DXMenuItem("Áp dụng theo nhóm", (o, args) =>
                {
                    int rowHandle = e.HitInfo.RowHandle;
                    var col = e.HitInfo.Column;
                    object value = view.GetRowCellValue(rowHandle, col);

                    // Xác định group row của row hiện tại
                    int groupRowHandle = view.GetParentRowHandle(rowHandle);

                    view.BeginUpdate();
                    try
                    {
                        // Duyệt tất cả row trong group đó
                        for (int i = 0; i < view.GetChildRowCount(groupRowHandle); i++)
                        {
                            int childHandle = view.GetChildRowHandle(groupRowHandle, i);
                            if (view.IsDataRow(childHandle))
                                view.SetRowCellValue(childHandle, col, value);
                        }
                    }
                    finally { view.EndUpdate(); }
                });

                e.Menu.Items.Add(fillAllItem);
                e.Menu.Items.Add(fillGroupItem);
            }
        }
        private void saveBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!checkCanDoiNPL()) return;
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
                saveCanDoiNPL();
                updateCanDoiNPL();
                clsWaitForm.ShowSuccessForm(this, 2000);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void saveCanDoiNPL()
        {
            gridView2.CloseEditor();
            gridView2.UpdateCurrentRow();
            DataTable dt = gridControl2.DataSource as DataTable;
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) return;
            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "createtophieu";
            string key = XuLyVTUnits.generatedTimeKey("one");
            foreach (DataRow row in dt.Rows)
            {
                decimal slCanDoiKho = XuLyVTUnits.SmartTryParse<decimal>(row["SLCanDoiKho"].ToString());
                decimal slMuaThem = XuLyVTUnits.SmartTryParse<decimal>(row["SLMuaThem"].ToString());
                string match = row["Havematch"].ToString();

                if (slCanDoiKho + slMuaThem > 0 && match == "no")
                {
                    DataRow reqRow = request.TypeTable.NewRow();

                    foreach (DataColumn col in request.TypeTable.Columns)
                    {
                        if (dt.Columns.Contains(col.ColumnName))
                        {
                            reqRow[col.ColumnName] = row[col.ColumnName];
                        }
                        else
                        {
                            reqRow[col.ColumnName] = DBNull.Value;
                        }
                    }
                    reqRow["MaPhieu"] = key;
                    reqRow["NguoiTao"] = GlobleData.UserName;
                    reqRow["NgayTao"] = DateTime.Now;
                    reqRow["STTDH"] = 1;
                    reqRow["IsDHKhGui"] = ckDHKhGui.Checked ? 1 : 0;
                    request.TypeTable.Rows.Add(reqRow);
                }
            }
            string urlGetListDataTable = URL + "CanDoiNPL/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            if (response != "OK")
            {
                throw new ApplicationException(response);
            }
            DateTime? selectedDateTime = ngayDKDBEdit.EditValue as DateTime?;
            if (selectedDateTime.HasValue)
            {
                var update = XuLyVTRequestPost.createDefault("CanDoiNPL");
                update.Action = "updatengaydongbo";
                DataRow row = update.TypeTable.NewRow();
                row["MaDH"] = maDH;
                update.TypeTable.Rows.Add(row);
                update.Parameter6 = selectedDateTime.Value;
                response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, update); }).Result;
            }

        }
        private void updateCanDoiNPL()
        {
            gridView2.CloseEditor();
            gridView2.UpdateCurrentRow();
            DataTable dt = gridControl2.DataSource as DataTable;
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) return;
            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "updatephieucandoi";
            string key = XuLyVTUnits.generatedTimeKey("one");
            foreach (DataRow row in dt.Rows)
            {
                decimal slCanDoiKho = XuLyVTUnits.SmartTryParse<decimal>(row["SLCanDoiKho"].ToString());
                decimal slMuaThem = XuLyVTUnits.SmartTryParse<decimal>(row["SLMuaThem"].ToString());
                string match = row["Havematch"].ToString();

                if (slCanDoiKho + slMuaThem > 0 && match == "yes")
                {
                    DataRow reqRow = request.TypeTable.NewRow();

                    foreach (DataColumn col in request.TypeTable.Columns)
                    {
                        if (dt.Columns.Contains(col.ColumnName))
                        {
                            reqRow[col.ColumnName] = row[col.ColumnName];
                        }
                        else
                        {
                            // Nếu muốn set default cho cột không có ở bảng A
                            reqRow[col.ColumnName] = DBNull.Value;
                        }
                    }
                    reqRow["MaPhieu"] = key;
                    reqRow["NguoiTao"] = GlobleData.UserName;
                    reqRow["NgayTao"] = DateTime.Now;
                    reqRow["STTDH"] = 1;
                    reqRow["IsDHKhGui"] = ckDHKhGui.Checked ? 1 : 0;
                    request.TypeTable.Rows.Add(reqRow);
                }
            }
            string urlGetListDataTable = URL + "CanDoiNPL/Post";
            string response = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            if (response != "OK")
            {
                throw new ApplicationException(response);
            }
        }
        private void autoCanDoiNPL()
        {
            DataTable dt = gridControl2.DataSource as DataTable;
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) return;

            foreach (DataRow row in dt.Rows)
            {
                decimal slTonKho = XuLyVTUnits.SmartTryParse<decimal>(row["TonKho"].ToString());
                decimal slNhuCau = XuLyVTUnits.SmartTryParse<decimal>(row["SLNhuCau"].ToString());
                decimal slCanDoiKho;
                if (slNhuCau >= slTonKho) slCanDoiKho = slTonKho;
                else slCanDoiKho = slNhuCau;
                row["SLCanDoiKho"] = slCanDoiKho;
                row["SLMuaThem"] = slNhuCau - slCanDoiKho;
                row["SLTonKhoSauCanDoi"] = slTonKho - slCanDoiKho;
                //if (row["Havematch"].ToString() == "no")
                //{
                //    decimal slTonKho = XuLyVTUnits.SmartTryParse<decimal>(row["TonKho"].ToString());
                //    decimal slNhuCau = XuLyVTUnits.SmartTryParse<decimal>(row["SLNhuCau"].ToString());
                //    decimal slCanDoiKho;
                //    if (slNhuCau >= slTonKho) slCanDoiKho = slTonKho;
                //    else slCanDoiKho = slNhuCau;
                //    row["SLCanDoiKho"] = slCanDoiKho;
                //    row["SLMuaThem"] = slNhuCau - slCanDoiKho;
                //    row["SLTonKhoSauCanDoi"] = slTonKho - slCanDoiKho;
                //}
            }
            gridControl2TableCopy = gridControl2Table.Copy();
        }
        private bool checkCanDoiNPL()
        {
            gridView2.CloseEditor();
            gridView2.UpdateCurrentRow();
            DataTable dt = gridControl2.DataSource as DataTable;
            DataTable dtcheck = gridControl2TableCopy.Copy();

            try
            {
                dt.PrimaryKey = new DataColumn[] { dt.Columns["generatedID"] };
                dtcheck.PrimaryKey = new DataColumn[] { dtcheck.Columns["generatedID"] };

                foreach (DataRow rowNew in dt.Rows)
                {
                    var id = rowNew["generatedID"];
                    DataRow rowOld = dtcheck.Rows.Find(id);
                    if (rowOld == null) continue; // không có trong bảng cũ thì bỏ qua hoặc xử lý riêng

                    // lấy số tồn kho và cân đối kho mới
                    decimal slTonKhoMoi = XuLyVTUnits.SmartTryParse<decimal>(rowNew["TonKho"].ToString());
                    decimal slCanDoiMoi = XuLyVTUnits.SmartTryParse<decimal>(rowNew["SLCanDoiKho"].ToString());

                    // lấy số tồn kho và cân đối kho cũ
                    decimal slTonKhoCu = XuLyVTUnits.SmartTryParse<decimal>(rowOld["TonKho"].ToString());
                    decimal slCanDoiCu = XuLyVTUnits.SmartTryParse<decimal>(rowOld["SLCanDoiKho"].ToString());

                    // so sánh
                    if (slTonKhoMoi + slCanDoiMoi > slTonKhoCu + slCanDoiCu)
                    {
                        XtraMessageBox.Show("Số lượng cân đối từ tồn kho không hợp lệ!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi trùng key, vui lòng nạp lại!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        private void candoiBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //autoCanDoiNPL();
        }
        private void resetBtn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridControl2Table = gridControl2TableCopy.Copy();
            loadGridControl2();
        }

        private void loadNgayDongBoDK()
        {
            if (ngayDBTable == null || ngayDBTable.Columns.Count <= 0 || ngayDBTable.Rows.Count <= 0) return;
            string ngayStr = ngayDBTable.Rows[0]["NgayDKDBo"]?.ToString(); // lấy từ DataTable
            DateTime? ngayValue = null;

            if (!string.IsNullOrWhiteSpace(ngayStr))
            {
                // Parse theo format dd/MM/yyyy
                if (DateTime.TryParseExact(
                    ngayStr,
                    "dd/MM/yyyy", 
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDate))
                {
                    ngayValue = parsedDate;
                }
            }
            ngayDKDBEdit.EditValue = ngayValue;
        }
        private void getNgayDongBoDK()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getngaydongbo";
            request.Parameter = maDH;
            string urlGetListDataTable = URL + "CanDoiNPL/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            ngayDBTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
        }
        private void ckDHKhGui_CheckedChanged(object sender, EventArgs e)
        {
            DataTable dt = gridControl2.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                XtraMessageBox.Show("Vui lòng chọn vật tư trước khi tick!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                ckDHKhGui.CheckedChanged -= ckDHKhGui_CheckedChanged;
                ckDHKhGui.Checked = false;
                ckDHKhGui.CheckedChanged += ckDHKhGui_CheckedChanged;
                return;
            }

            gridView2.BeginUpdate();
            try
            {
                if (ckDHKhGui.Checked)
                {
                    gridControl2SnapshotKhGui = dt.Copy();

                    foreach (DataRow row in dt.Rows)
                    {
                        decimal slNhuCau = XuLyVTUnits.SmartTryParse<decimal>(row["SLNhuCau"].ToString());
                        decimal tonKho = XuLyVTUnits.SmartTryParse<decimal>(row["TonKho"].ToString());
                        row["SLTonKhoSauCanDoi"] = tonKho;
                        row["SLCanDoiKho"] = 0;
                        row["SLMuaThem"] = slNhuCau;
                    }
                }
                else
                {
                    if (gridControl2SnapshotKhGui == null) return;
                    gridControl2Table = gridControl2SnapshotKhGui.Copy();
                    gridControl2.DataSource = gridControl2Table;
                    gridControl2SnapshotKhGui = null;
                }
                gridView2.RefreshData();
            }
            finally
            {
                gridView2.EndUpdate();
            }
        }  
        private void LoadDHKHGui()
        {
            if (gridControl2Table != null && gridControl2Table.Columns.Contains("IsDHKhGui"))
            {
                bool isDHKhGui = gridControl2Table.AsEnumerable()
                    .Any(r => r["IsDHKhGui"] is true || r["IsDHKhGui"]?.ToString() == "1");

                ckDHKhGui.CheckedChanged -= ckDHKhGui_CheckedChanged;
                ckDHKhGui.Checked = isDHKhGui;
                ckDHKhGui.CheckedChanged += ckDHKhGui_CheckedChanged;
                if (isDHKhGui)
                    ckDHKhGui.Enabled = false;
            }
        }

        private void gridView2_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 192, 128), Color.FromArgb(255, 192, 128), e.Column.AppearanceHeader.GradientMode);
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

        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 192, 128), Color.FromArgb(255, 192, 128), e.Column.AppearanceHeader.GradientMode);
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
    }
}