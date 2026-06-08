using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmCanDoiDHChiaSXChiTiet : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _maDH = string.Empty, _maLenhSX = string.Empty, _lenhSX = string.Empty;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _checkCapPhat = false;
        private HttpClientExtension _clientExtension;
        public frmCanDoiDHChiaSXChiTiet(string maDH = "", string maLenhSX = "", bool allowAdd = false, bool allowEdit = false, bool allowDelete = false, bool checkCapPhat = false)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._maDH = maDH;
            this._maLenhSX = maLenhSX;
            bandedGridView1.RowCellStyle += bandedGridView1_RowCellStyle;
            bandedGridView1.ShowingEditor += bandedGridView1_ShowingEditor;

            bandedGridView1.Appearance.FocusedCell.Options.UseBackColor = true;
            bandedGridView1.Appearance.FocusedCell.BackColor = Color.FromArgb(234, 234, 234);

            bandedGridView1.CustomColumnDisplayText += bandedGridView1_CustomColumnDisplayText;
            bandedGridView1.CellValueChanged += bandedGridView1_CellValueChanged;

            bandedGridView1.GridControl.ProcessGridKey += GridControl_ProcessGridKey;
            grvLenhChia.FocusedRowChanged += grvLenhChia_FocusedRowChanged;
            grvLenhChia.ShowingEditor += grvLenhChia_ShowingEditor;
            grvLenhChia.RowCellStyle += grv_RowCellStyle;
            grvCanDoi.RowCellStyle += grvCanDoi_RowCellStyle;

            bandedGridView1.OptionsSelection.MultiSelect = true;
            bandedGridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            bandedGridView1.SelectionChanged += BandedGridView1_SelectionChanged;
            this._allowAdd = allowAdd;
            this._allowEdit = allowEdit;
            this._allowDelete = allowDelete;
            this._checkCapPhat = checkCapPhat;
            InIt();
            CheckPerminsion();
        }
        private void CheckPerminsion()
        {

            if (!_allowAdd || !_allowEdit || !_allowDelete)
            {
                simpleButton6.Enabled = false;
                btnXuatEX.Enabled = false;
                btnChiaChuyenItem.Enabled = false;
                btnDoiChuyenItem.Enabled = false;
                btnXoaChuyenItem.Enabled = false;
                btnDuyetItem.Enabled = false;
                btnSaveItem.Enabled = false;
            }

        }
        private void InIt()
        {
            searchLookUpEditPO.Properties.ValueMember = "POID";
            searchLookUpEditPO.Properties.DisplayMember = "PO";
            searchLookUpEditPO.Properties.NullValuePrompt = "Chọn PO";

            searchLookUpEditMau.Properties.ValueMember = "MaMau";
            searchLookUpEditMau.Properties.DisplayMember = "TenMau";
            searchLookUpEditMau.Properties.NullValuePrompt = "Chọn màu";

            searchLookUpEditLine.Properties.ValueMember = "Line";
            searchLookUpEditLine.Properties.DisplayMember = "Name";
            searchLookUpEditLine.Properties.NullValuePrompt = "Chọn chuyền";
            LoadLenhSanXuat();
            if (_checkCapPhat)
            {
                colPOMua.Visible = false;
                layoutControlGroup10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }


        }
        private void LoadLenhSanXuat()
        {
            string url = $"{URL}ERPDonHangTong/Get?action=GetLenhChiaSX";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                grcLenhChia.DataSource = null;
                return;
            }
            grcLenhChia.DataSource = tbl;
            _maDH = tbl.Rows[0]["MaDH"].ToString();
            _maLenhSX = tbl.Rows[0]["MaLenhSanXuat"].ToString();
            //AutoClickRow();
        }
        private void AutoClickRow()
        {
            if (!this.IsHandleCreated)
            {
                this.HandleCreated += (s, e) => AutoClickRow();
                return;
            }
            this.BeginInvoke(new Action(() =>
            {
                for (int i = 0; i < grvLenhChia.RowCount; i++)
                {
                    var row = grvLenhChia.GetDataRow(i);
                    if (row != null && row["MaDH"].ToString() == _maDH && row["MaLenhSanXuat"].ToString() == _maLenhSX)
                    {
                        grvLenhChia.FocusedRowHandle = i;
                        grvLenhChia.SelectRow(i);
                        break;
                    }
                }
            }));
        }
        private void LoadCanDoi()
        {
            string url = $"{URL}ERPDonHangTong/Get?action=GetNPL&para={_maDH}&para5={_maLenhSX}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                grcCanDoi.DataSource = null;
                return;
            }
            grcCanDoi.DataSource = tbl;
        }
        private void searchLookUpEditPO_Properties_EditValueChanged(object sender, EventArgs e)
        {
            GetChiTietCanDoi();
        }
        private void searchLookUpEditMau_Properties_EditValueChanged(object sender, EventArgs e)
        {
            GetChiTietCanDoi();
        }

        private void searchLookUpEditLine_Properties_EditValueChanged(object sender, EventArgs e)
        {
            GetChiTietCanDoi();
        }

        private void GetChiTietCanDoi()
        {
            try
            {
                string PO = (searchLookUpEditPO.EditValue as string) ?? "";
                string Mau = (searchLookUpEditMau.EditValue as string) ?? "";
                string Line = (searchLookUpEditLine.EditValue as string) ?? "";
                string url = $"{URL}ERPDonHangTong/Get?action=GetChiTietLenh&para={_maDH}&para2={PO}&para3={Mau}&para4={Line}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    grcChiTietCanDoi.DataSource = null;
                    return;
                }

                grcChiTietCanDoi.DataSource = tbl;
                CreateBandSize(tbl, bandedGridView1, gbSize, repotxtN0);
                AmoutRow(tbl);
            }
            catch (Exception ex)
            {

            }
        }
        private void GetPO()
        {
            try
            {
                string url = $"{URL}ERPDonHangTong/Get?action=GetPO&para={_maDH}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditPO.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditPO.Properties.DataSource = tbl;

            }
            catch (Exception ex)
            {

            }
        }
        private void GetMau()
        {
            try
            {
                string url = $"{URL}ERPDonHangTong/Get?action=GetMau&para={_maDH}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditMau.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditMau.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }
        private void GetLine()
        {
            try
            {
                string url = $"{URL}ERPDonHangTong/Get?action=GetLineCT&para={_maDH}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    searchLookUpEditLine.Properties.DataSource = null;
                    return;
                }
                searchLookUpEditLine.Properties.DataSource = tbl;


            }
            catch (Exception ex)
            {

            }
        }
        private void GetChiTietDH()
        {
            try
            {
                string url = $"{URL}ERPDonHangTong/Get?action=GetChiTiet&para={_maDH}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    txtDH.EditValue = null;
                    txtLenhSX.EditValue = null;
                    txtMaHang.EditValue = null;
                    txtChuyenGoc.EditValue = null;
                    txtDotSX.EditValue = null;
                    return;
                }

                txtDH.EditValue = tbl.Rows[0]["MaDH"];
                txtLenhSX.EditValue = tbl.Rows[0]["MaLenh"];
                txtMaHang.EditValue = tbl.Rows[0]["MaHang"];
                txtChuyenGoc.EditValue = tbl.Rows[0]["Name"];
                txtDotSX.EditValue = tbl.Rows[0]["DotSX"];

                return;

            }
            catch (Exception ex)
            {

            }
        }
        private void AmoutRow(DataTable _data)
        {
            if (!_data.Columns.Contains("Amount"))
                _data.Columns.Add("Amount", typeof(int));

            foreach (DataRow row in _data.Rows)
            {
                int total = 0;
                foreach (DataColumn column in _data.Columns)
                {
                    if (column.ColumnName.Contains("@"))
                    {
                        total += Convert.ToInt32(row[column]);
                    }
                }
                row["Amount"] = total;
            }

            // 🔸 Nếu cột Amount đã tồn tại trong grid → chỉnh lại thuộc tính
            BandedGridColumn colAmount = bandedGridView1.Columns["Amount"] as BandedGridColumn;
            if (colAmount == null)
            {
                // Nếu chưa có, tạo mới
                colAmount = new BandedGridColumn
                {
                    FieldName = "Amount",
                    Caption = "Tổng",
                    Visible = true,
                    Width = 80,
                    OptionsColumn = { AllowEdit = false }
                };
                bandedGridView1.Columns.Add(colAmount);
            }
            else
            {
                // Nếu có, chỉnh caption/format nếu cần
                colAmount.Caption = "Tổng số lượng";
                colAmount.Width = 100;
            }

            // 🔸 Format số
            colAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colAmount.DisplayFormat.FormatString = "n0";

            // 🔸 Xóa summary cũ nếu có
            colAmount.Summary.Clear();

            // 🔸 Thêm summary mới
            colAmount.Summary.Add(DevExpress.Data.SummaryItemType.Sum, "Amount", "{0:n0}");
            bandedGridView1.OptionsView.ShowFooter = true;
        }
        public static void CreateBandSize(DataTable dt, BandedGridView grvShared, GridBand gbSizeA, DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repotxtN0, bool IsCheckSumSize = true)
        {
            ClearBand(gbSizeA);
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[0];


                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName = colName;
                col.Name = "col" + _sizeID;
                col.OptionsColumn.AllowEdit = true;
                col.ColumnEdit = repotxtN0;
                col.Visible = true;
                col.Width = 60;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                if (IsCheckSumSize)
                {
                    if (col.FieldName.Contains("@"))
                    {
                        GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                        itemSize.FieldName = col.FieldName;
                        itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                        itemSize.DisplayFormat = "{0:n0}";
                        itemSize.ShowInGroupColumnFooter = col;
                        grvShared.GroupSummary.Add(itemSize);
                    }
                    string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrName.Length > 1)
                    {
                        col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    }
                }

                // Thêm cột vào grid
                grvShared.Columns.AddRange(new BandedGridColumn[] { col });

                // Tạo và thêm GridBand vào grid
                GridBand gb = new GridBand();
                gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                gb.AppearanceHeader.ForeColor = System.Drawing.Color.White;
                gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                gb.AppearanceHeader.Options.UseBackColor = true;
                gb.AppearanceHeader.Options.UseFont = true;
                gb.AppearanceHeader.Options.UseForeColor = true;
                gb.AppearanceHeader.Options.UseTextOptions = true;
                gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb.Caption = _size;
                gb.Columns.Add(col);
                gb.Name = "gb" + "Size@" + _sizeID;
                gb.VisibleIndex = 0;
                gb.Width = 60;
                gbSizeA.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }



        public static void ClearBand(GridBand gbSizeA)
        {
            gbSizeA.Children.Clear();
        }
        private void bandedGridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;

            DataRow row = null;
            try { row = view.GetDataRow(e.RowHandle); } catch { row = null; }
            if (row == null) return;

            int isLineGoc = 0;
            if (row.Table.Columns.Contains("IsLineGoc"))
                int.TryParse(row["IsLineGoc"].ToString(), out isLineGoc);

            if (e.Column.FieldName == "Name" && isLineGoc == 1)
            {

                e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#84e184");   // màu nền đỏ nhạt
                e.Appearance.ForeColor = Color.Black;

            }
            var selectedCells = view.GetSelectedCells();
            bool isCellSelected = selectedCells.Any(c =>
                c.RowHandle == e.RowHandle &&
                c.Column.FieldName == e.Column.FieldName
            );
            if (isCellSelected)
            {
                return; // Để GridView tự tô màu selection
            }
            if (e.Column == null || !e.Column.FieldName.Contains("@")) return;
            if (isLineGoc == 1)
            {
                // Màu nền xám nhạt #EAEAEA
                Color bg = Color.FromArgb(234, 234, 234);
                e.Appearance.BackColor = bg;
                e.Appearance.Options.UseBackColor = true;

                // Tùy chọn: đổi màu chữ cho rõ
                //e.Appearance.ForeColor = Color.Gray;
                e.Appearance.Options.UseForeColor = true;
            }
            else
            {
                // Trả lại mặc định (nếu bạn cần)
                // e.Appearance.Reset(); // hoặc đặt trắng, tuỳ bạn
            }
        }
        private void grv_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var view = sender as GridView; // hoặc BandedGridView nếu bạn dùng BandedGrid
            if (view == null) return;

            DataRow row = null;
            try { row = view.GetDataRow(e.RowHandle); } catch { row = null; }
            if (row == null) return;

            // Lấy giá trị CheckDuyet và Status
            int checkDuyet = 0;
            int status = 0;
            if (e.RowHandle == view.FocusedRowHandle)
            {
                e.Appearance.BackColor = Color.FromArgb(234, 234, 234);
                e.Appearance.ForeColor = Color.Black;
                return;
            }
            if (row.Table.Columns.Contains("CheckDuyet"))
                int.TryParse(row["CheckDuyet"]?.ToString(), out checkDuyet);

            if (row.Table.Columns.Contains("Status"))
            {
                if (row["Status"] is bool boolValue)
                    status = boolValue ? 1 : 0;
                else
                    int.TryParse(row["Status"]?.ToString(), out status);
            }

            // ✅ ÁP DỤNG MÀU CHO CẢ DÒNG DỰA TRÊN TRẠNG THÁI
            if (checkDuyet == 0)
            {
                // 🔸 Chưa duyệt (CheckDuyet = 0) → Màu trắng/mặc định
                e.Appearance.BackColor = Color.White;
                e.Appearance.ForeColor = Color.Black;
            }
            else if (checkDuyet == 1 && status == 0)
            {
                // 🟡 Đã duyệt nhưng chưa kích hoạt → Màu vàng
                e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffff99"); // Vàng nhạt
                e.Appearance.ForeColor = Color.Black;
            }
            else if (checkDuyet == 1 && status == 1)
            {
                // 🟢 Đã duyệt và đã kích hoạt → Màu xanh
                e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#84e184"); // Xanh lá nhạt
                e.Appearance.ForeColor = Color.Black;
            }

            e.Appearance.Options.UseBackColor = true;
            e.Appearance.Options.UseForeColor = true;
            if (e.Column.FieldName != "TrangThai")
                return;

            // Kiểm tra null và chuyển về chuỗi an toàn
            if (e.CellValue == null)
                return;
            if (e.CellValue.ToString() == "Đã sẵn sàng cấp phát")// Đã ss cấp phát
            {
                e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcad4");
                e.Appearance.ForeColor = Color.DarkGreen;
            }
            else if (e.CellValue.ToString() == "Đã cấp phát") // Đã cp
            {
                e.Appearance.BackColor = Color.LightYellow;
                e.Appearance.ForeColor = Color.OrangeRed;
            }
            else if (e.CellValue.ToString() == "Đã có BOM") // Đã có BOM
            {
                e.Appearance.BackColor = Color.LightSkyBlue;
                e.Appearance.ForeColor = Color.DarkBlue;
            }
        }
        private void grvCanDoi_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            //var view = sender as GridView; // hoặc BandedGridView nếu bạn dùng BandedGrid
            //if (view == null) return;

            //DataRow row = null;
            //try { row = view.GetDataRow(e.RowHandle); } catch { row = null; }
            //if (row == null) return;

            //// Lấy giá trị CheckDuyet và Status
            //int checkDuyet = row["Status"].ToString() == "Đã xác nhận" ? 1 : 0;
            //int status = 0;


            //// ✅ ÁP DỤNG MÀU CHO CẢ DÒNG DỰA TRÊN TRẠNG THÁI
            //if (checkDuyet == 0)
            //{
            //    // 🔸 Chưa duyệt (CheckDuyet = 0) → Màu trắng/mặc định
            //    e.Appearance.BackColor = Color.White;
            //    e.Appearance.ForeColor = Color.Black;
            //}
            //else if (checkDuyet == 1)
            //{
            //    // 🟡 Đã duyệt nhưng chưa kích hoạt → Màu vàng
            //    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#84e184"); // Vàng nhạt
            //    e.Appearance.ForeColor = Color.Black;
            //}
            //e.Appearance.Options.UseBackColor = true;
            //e.Appearance.Options.UseForeColor = true;
        }
        private void bandedGridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;
            if (view.FocusedColumn == null) return;

            // Chỉ quan tâm cột size động
            if (!view.FocusedColumn.FieldName.Contains("@")) return;

            DataRow row = null;
            try { row = view.GetDataRow(view.FocusedRowHandle); } catch { row = null; }
            if (row == null) return;

            int isLineGoc = 0;
            if (row.Table.Columns.Contains("IsLineGoc"))
                int.TryParse(row["IsLineGoc"].ToString(), out isLineGoc);

            if (isLineGoc == 1)
            {
                // Chặn mở editor → readonly trên từng dòng
                e.Cancel = true;
            }
        }
        private void bandedGridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;

            // Chỉ áp dụng cho cột size động
            if (e.Column == null || !e.Column.FieldName.Contains("@")) return;

            DataRow row = null;
            try { row = view.GetDataRow(e.ListSourceRowIndex); } catch { row = null; }
            if (row == null) return;

            int isLineGoc = 0;
            if (row.Table.Columns.Contains("IsLineGoc"))
                int.TryParse(row["IsLineGoc"].ToString(), out isLineGoc);

            int value;
            // Kiểm tra xem text hiện tại có phải là số không
            if (int.TryParse(e.DisplayText, out value))
            {
                if (value == 0)
                {
                    e.DisplayText = "-";
                }
            }
        }
        private void bandedGridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;

            // Chỉ tính lại khi thay đổi các cột size (có ký tự "@")
            if (e.Column.FieldName.Contains("@"))
            {
                DataRow row = view.GetDataRow(e.RowHandle);
                if (row == null) return;

                int total = 0;
                foreach (DataColumn column in row.Table.Columns)
                {
                    if (column.ColumnName.Contains("@"))
                    {
                        int val = 0;
                        int.TryParse(row[column].ToString(), out val);
                        total += val;
                    }
                }

                row["Amount"] = total;

                // Cập nhật lại giao diện ngay
                view.RefreshRow(e.RowHandle);
            }
        }
        private void BandedGridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                var view = sender as BandedGridView;
                if (view == null) return;

                var selectedCells = view.GetSelectedCells();
                if (selectedCells == null || selectedCells.Length == 0) return;

                // ✅ LẤY DANH SÁCH CÁC DÒNG KHÁC NHAU ĐƯỢC CHỌN
                var distinctRows = selectedCells.Select(c => c.RowHandle).Distinct().ToList();

                // ✅ NẾU CHỌN QUA NHIỀU HƠN 1 DÒNG → HỦY SELECTION
                if (distinctRows.Count > 1)
                {
                    // Lấy dòng đầu tiên được chọn
                    int firstRow = distinctRows.First();

                    // Xóa tất cả selection
                    view.ClearSelection();

                    // Chỉ giữ lại các ô trong dòng đầu tiên
                    var cellsInFirstRow = selectedCells.Where(c => c.RowHandle == firstRow).ToList();

                    foreach (var cell in cellsInFirstRow)
                    {
                        view.SelectCell(cell.RowHandle, cell.Column);
                    }

                    // Thông báo (tùy chọn - có thể bỏ nếu thấy phiền)
                    // XtraMessageBox.Show("Chỉ được bôi đen trong 1 dòng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void bandedGridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                var view = sender as BandedGridView;
                if (view == null || view.FocusedColumn == null) return;

                string fieldName = view.FocusedColumn.FieldName;
                if (!fieldName.Contains("@")) return; // chỉ áp dụng cho cột size động

                DataRow currentRow = view.GetDataRow(view.FocusedRowHandle);
                if (currentRow == null) return;

                // Nếu dòng hiện tại là chuyền gốc thì bỏ qua, không giới hạn
                if (currentRow["IsLineGoc"] != DBNull.Value && Convert.ToInt32(currentRow["IsLineGoc"]) == 1)
                    return;

                // Lấy giá trị người dùng nhập
                int newValue = 0;
                if (!int.TryParse(e.Value?.ToString(), out newValue))
                {
                    e.Valid = false;
                    e.ErrorText = "Vui lòng nhập số hợp lệ.";
                    return;
                }

                // Lấy giá trị cũ để tính delta
                int oldValue = 0;
                int.TryParse(currentRow[fieldName].ToString(), out oldValue);
                int delta = newValue - oldValue;  // số thay đổi so với trước

                // Xác định các khóa để tìm dòng gốc
                var poid = currentRow["POID"].ToString();
                var madh = currentRow["MaDH"].ToString();
                var mamau = currentRow["MaMau"].ToString();
                var dausizeid = currentRow["DauSizeID"].ToString();
                var codemau = currentRow["CodeMau"].ToString();

                // Lấy bảng nguồn
                DataTable tbl = view.GridControl.DataSource as DataTable;
                if (tbl == null) return;

                // Tìm dòng chuyền gốc
                var rowGoc = tbl.AsEnumerable().FirstOrDefault(r =>
                    r["POID"].ToString() == poid &&
                    r["MaDH"].ToString() == madh &&
                    r["MaMau"].ToString() == mamau &&
                    r["DauSizeID"].ToString() == dausizeid &&
                    r["CodeMau"].ToString() == codemau &&
                    Convert.ToInt32(r["IsLineGoc"]) == 1
                );

                if (rowGoc == null) return;

                int valueGoc = 0;
                int.TryParse(rowGoc[fieldName].ToString(), out valueGoc);

                // Kiểm tra không được vượt số gốc còn lại
                if (delta > valueGoc)
                {
                    e.Valid = false;
                    e.ErrorText = $"Không được nhập vượt quá số lượng còn lại ({valueGoc}).";
                    return;
                }

                // Nếu hợp lệ → cập nhật lại giá trị chuyền gốc
                rowGoc[fieldName] = valueGoc - delta;

                // Cập nhật lại Amount của cả dòng gốc và dòng hiện tại
                UpdateAmountForRow(currentRow);
                UpdateAmountForRow(rowGoc);
            }
            catch (Exception ex)
            {

            }
        }
        private void UpdateAmountForRow(DataRow row)
        {
            int total = 0;
            foreach (DataColumn col in row.Table.Columns)
            {
                if (col.ColumnName.Contains("@"))
                {
                    int val = 0;
                    int.TryParse(row[col.ColumnName].ToString(), out val);
                    total += val;
                }
            }
            row["Amount"] = total;
        }

        private void BuildSaveTable()
        {
            var view = bandedGridView1;
            var tblSource = view.GridControl.DataSource as DataTable;
            if (tblSource == null) return;

            // Tạo DataTable kết quả

            List<CanDoiDonViSanXuatSaveEntity> listSave = new List<CanDoiDonViSanXuatSaveEntity>();
            // Duyệt từng dòng trong Grid
            foreach (DataRow row in tblSource.Rows)
            {
                string poid = row["POID"].ToString();
                string madh = row["MaDH"].ToString();
                string mamau = row["MaMau"].ToString();
                string dausizeid = row["DauSizeID"].ToString();
                string codemau = row["CodeMau"].ToString();
                string line = row["Line"].ToString();


                // Duyệt các cột size động
                foreach (DataColumn col in tblSource.Columns)
                {
                    if (col.ColumnName.Contains("@"))
                    {
                        string[] parts = col.ColumnName.Split('@');
                        if (parts.Length == 0) continue;
                        string size = parts[1];

                        int soLuong = 0;
                        int.TryParse(row[col.ColumnName].ToString(), out soLuong);

                        CanDoiDonViSanXuatSaveEntity itemSave = new CanDoiDonViSanXuatSaveEntity();

                        itemSave.POID = poid;
                        itemSave.MaDH = madh;
                        itemSave.MaMau = mamau;
                        itemSave.DauSizeID = dausizeid;
                        itemSave.Line = line;
                        itemSave.SizeID = size;
                        itemSave.SoLuong = soLuong;
                        itemSave.MaLenhSanXuat = _maLenhSX;
                        listSave.Add(itemSave);
                    }
                }
            }
            if (listSave == null || listSave.Count == 0)
            {
                XtraMessageBox.Show("Không có dữ liệu để lưu.");
                return;
            }
            string url = string.Format("{0}", URL + $"ERPDonHangTong/Post?action=PostSoLuongChuyen&type=@TypeTable");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, listSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                GetChiTietCanDoi();
            }
        }
        private void GridControl_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopySizeCellsToClipboard();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteClipboardToSizeCells();
                e.Handled = true;
            }
        }
        private void CopySizeCellsToClipboard()
        {
            try
            {
                var view = bandedGridView1;

                // ✅ LẤY DANH SÁCH CÁC Ô ĐÃ CHỌN
                var selectedCells = view.GetSelectedCells();

                if (selectedCells == null || selectedCells.Length == 0)
                {
                    XtraMessageBox.Show("Vui lòng bôi đen các ô cần copy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ✅ LỌC CHỈ LẤY CÁC Ô SIZE (có ký tự @)
                var sizeCells = selectedCells.Where(c => c.Column.FieldName.Contains("@")).ToList();

                if (sizeCells.Count == 0)
                {
                    XtraMessageBox.Show("Chỉ có thể copy các cột Size!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ✅ NHÓM CÁC Ô THEO DÒNG
                var cellsByRow = sizeCells
                    .GroupBy(c => c.RowHandle)
                    .OrderBy(g => g.Key)
                    .ToList();

                StringBuilder clipboardText = new StringBuilder();

                // ✅ DUYỆT TỪNG DÒNG
                foreach (var rowGroup in cellsByRow)
                {
                    // Sắp xếp các ô trong dòng theo thứ tự cột
                    var cellsInRow = rowGroup.OrderBy(c => c.Column.VisibleIndex).ToList();

                    List<string> rowValues = new List<string>();

                    foreach (var cell in cellsInRow)
                    {
                        DataRow row = view.GetDataRow(cell.RowHandle);
                        if (row == null) continue;

                        object value = row[cell.Column.FieldName];

                        // Chuyển về chuỗi
                        string strValue = "";
                        if (value != null && value != DBNull.Value)
                        {
                            int intValue = 0;
                            if (int.TryParse(value.ToString(), out intValue))
                            {
                                strValue = intValue.ToString();
                            }
                        }
                        rowValues.Add(strValue);
                    }

                    // Nối các giá trị trong dòng bằng TAB
                    clipboardText.AppendLine(string.Join("\t", rowValues));
                }

                // ✅ ĐƯA VÀO CLIPBOARD
                if (clipboardText.Length > 0)
                {
                    Clipboard.SetText(clipboardText.ToString().TrimEnd());

                    // Thông báo (tùy chọn)
                    // XtraMessageBox.Show($"Đã copy {sizeCells.Count} ô!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void PasteClipboardToSizeCells()
        {
            try
            {
                string clipboardText = Clipboard.GetText();
                if (string.IsNullOrWhiteSpace(clipboardText)) return;

                var view = bandedGridView1;
                if (view.FocusedRowHandle < 0 || view.FocusedColumn == null) return;

                DataTable tbl = grcChiTietCanDoi.DataSource as DataTable;

                DataRow row = view.GetDataRow(view.FocusedRowHandle);
                if (row == null) return;

                // 🔸 Kiểm tra dòng hiện tại có phải chuyền gốc không
                if (row.Table.Columns.Contains("IsLineGoc") &&
                    int.TryParse(row["IsLineGoc"]?.ToString(), out int isLineGoc) &&
                    isLineGoc == 1)
                {
                    XtraMessageBox.Show("Không thể paste vào dòng chuyền gốc!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 🔸 Tách dữ liệu từ clipboard
                string[] values = clipboardText
                    .Split(new[] { '\t', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length == 0) return;

                // 🔸 Lấy danh sách cột Size động từ DataTable (theo đúng thứ tự bạn muốn)
                var sizeColumns = tbl.Columns.Cast<DataColumn>()
                    .Where(c => c.ColumnName.Contains("@"))
                    .ToList();

                // 🔸 Giới hạn tránh tràn chỉ số
                int colCount = Math.Min(values.Length, sizeColumns.Count);

                List<string> errors = new List<string>();
                int successCount = 0;

                for (int i = 0; i < colCount; i++)
                {
                    string valueText = values[i].Trim();
                    if (string.IsNullOrWhiteSpace(valueText)) continue;

                    var column = sizeColumns[i];
                    string fieldName = column.ColumnName; // ✅ Dùng ColumnName vì FieldName = ColumnName

                    // 🔸 Kiểm tra giá trị hợp lệ
                    if (!int.TryParse(valueText, out int newVal))
                    {
                        errors.Add($"Cột '{column.ColumnName}': giá trị '{valueText}' không hợp lệ");
                        continue;
                    }

                    // 🔸 Kiểm tra hợp lệ qua sự kiện ValidatingEditor
                    var gridCol = view.Columns
                        .Cast<BandedGridColumn>()
                        .FirstOrDefault(c => c.FieldName == fieldName);

                    if (gridCol != null)
                    {
                        view.FocusedColumn = gridCol;
                        var validateArgs = new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs(valueText);
                        bandedGridView1_ValidatingEditor(view, validateArgs);

                        if (!validateArgs.Valid)
                        {
                            errors.Add($"Cột '{gridCol.Caption}': {validateArgs.ErrorText}");
                            continue;
                        }
                    }

                    // 🔸 Gán giá trị
                    row[fieldName] = newVal;
                    successCount++;
                }

                // 🔸 Cập nhật Amount và refresh view
                if (successCount > 0)
                {
                    UpdateAmountForRow(row);
                    view.RefreshRow(view.FocusedRowHandle);
                }

                // 🔸 Hiển thị kết quả
                if (errors.Count > 0)
                {
                    string message = $"Đã paste thành công {successCount}/{values.Length} giá trị.\n\n";
                    message += "Các lỗi:\n" + string.Join("\n", errors.Take(10));
                    if (errors.Count > 10)
                        message += $"\n... và {errors.Count - 10} lỗi khác";

                    XtraMessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi paste dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnChiaChuyenItem_Click(object sender, EventArgs e)
        {

            using (var frm = new frmCanDoiDHChiaChuyen(_maDH, _maLenhSX))
            {

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    GetChiTietCanDoi();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
            }
        }

        private void btnDoiChuyenItem_Click(object sender, EventArgs e)
        {
            using (var frm = new frmCanDoiDHDoiChuyen(_maDH, _maLenhSX))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    GetChiTietCanDoi();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
            }
        }

        private void btnXoaChuyenItem_Click(object sender, EventArgs e)
        {
            DataRow dtRow = grvLenhChia.GetFocusedDataRow();
            if (dtRow == null) return;
            if (Convert.ToInt32(dtRow["CheckDuyet"]) == 1)
            {
                XtraMessageBox.Show("Lệnh này đã được duyệt trước đó nên không xóa được chuyền!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (var frm = new frmCanDoiDHXoaChuyen(_maDH, _maLenhSX))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    GetChiTietCanDoi();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
            }
        }

        private void btnSaveItem_Click(object sender, EventArgs e)
        {
            BuildSaveTable();
        }

        private void btnDuyetItem_Click(object sender, EventArgs e)
        {
            var dt = grcLenhChia.DataSource as DataTable;
            if (dt == null) return;

            // Tìm dòng có MaLenh = _lenhSX
            DataRow rowLenh = dt.AsEnumerable()
                                .FirstOrDefault(r => r["MaLenh"]?.ToString() == _lenhSX);

            if (rowLenh == null)
            {
                XtraMessageBox.Show("Không tìm thấy lệnh sản xuất trong danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //// Kiểm tra đã duyệt chưa
            //if (rowLenh["CheckDuyet"] != DBNull.Value && Convert.ToInt32(rowLenh["CheckDuyet"]) == 1)
            //{
            //    XtraMessageBox.Show("Lệnh này đã được duyệt trước đó!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            var confirm = XtraMessageBox.Show(
                   $"Bạn có chắc muốn duyệt lệnh sản xuất: {_lenhSX} này không?",
                   "Xác nhận duyệt lệnh",
                   MessageBoxButtons.YesNo,
                   MessageBoxIcon.Question
               );

            if (confirm != DialogResult.Yes)
                return;

            string userName = GlobleData.UserName ?? "";
            string makh = rowLenh["MaKH"].ToString() ?? "";
            string mahang = rowLenh["MaHang"].ToString() ?? "";
            string madot = string.Empty;
            string urlDot = $"{URL}ERPDonHangTong/Get?Action=GETMAXDOT&para={makh}&para2={mahang}";
            string jsonDot = Task.Run(async () => await _clientExtension.GetAsnyc(urlDot)).Result;

            DataTable tblDot = JsonConvert.DeserializeObject<DataTable>(jsonDot);

            if (tblDot != null && tblDot.Rows.Count != 0)
            {
                madot = tblDot.Rows[0]["MaDot"].ToString();

            }
            // Nếu chưa duyệt → gọi API để duyệt
            string urlTS = $"{URL}KhoiTaoBOMV1/Get?action=GET_TSNEW&para1={makh}&para2={mahang}&para6={userName}&para7={madot}";
            string jsonTS = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTS); }).Result;

            string url = $"{URL}ERPDongBoQLSX/DongBo?action=DongBo&para={_lenhSX}";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            string urlIsKeoVe = $"{URL}ERPDonHangTong/GetChuyen?action=PostIsKeoVe&para={_maDH}&para5={_maLenhSX}";
            string jsonKeoVe = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlIsKeoVe); }).Result;

            if (json.ToUpper() != "2")
            {
                rowLenh["CheckDuyet"] = 1;
                grcLenhChia.RefreshDataSource();
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
            else
            {
                XtraMessageBox.Show("Duyệt thất bại. Vui lòng thử lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadLenhSanXuat();
        }


        private void grvLenhChia_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow drRow = grvLenhChia.GetFocusedDataRow();
            if (drRow == null) return;
            _maDH = drRow["MaDH"].ToString();
            _maLenhSX = drRow["MaLenhSanXuat"].ToString();
            _lenhSX = drRow["MaLenh"].ToString();
            GetChiTietDH();
            GetChiTietCanDoi();
            GetPO();
            GetMau();
            GetLine();
            LoadCanDoi();
        }
        private void grvLenhChia_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as GridView;
            if (view == null || view.FocusedColumn == null) return;

            // Chỉ xử lý khi người dùng click vào cột Status (checkbox)
            if (view.FocusedColumn.FieldName != "Status") return;

            DataRow row = view.GetDataRow(view.FocusedRowHandle);
            if (row == null) return;

            // Lấy các giá trị liên quan
            int checkDuyet = 0;
            if (row.Table.Columns.Contains("CheckDuyet"))
                int.TryParse(row["CheckDuyet"]?.ToString(), out checkDuyet);

            int currentStatus = 0;
            if (row.Table.Columns.Contains("Status"))
            {
                if (row["Status"] is bool boolValue)
                    currentStatus = boolValue ? 1 : 0;
                else
                    int.TryParse(row["Status"]?.ToString(), out currentStatus);
            }

            int isCheckKichHoat = 0;
            if (row.Table.Columns.Contains("isCheckKichHoat"))
                int.TryParse(row["isCheckKichHoat"]?.ToString(), out isCheckKichHoat);

            string maLenh = row.Table.Columns.Contains("MaLenh") ? row["MaLenh"]?.ToString() : "";

            // ✅ 1. Nếu CheckDuyet = 0 và Status hiện tại = 0 → Không cho bật kích hoạt
            if (checkDuyet == 0 && currentStatus == 0)
            {
                XtraMessageBox.Show(
                    "Vui lòng duyệt trước khi Kích hoạt!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                e.Cancel = true;
                return;
            }

            // ✅ 2. Nếu đang bật (Status = 1) mà isCheckKichHoat = 1 → Không cho tắt
            if (currentStatus == 1 && isCheckKichHoat == 1)
            {
                XtraMessageBox.Show(
                    $"Mã lệnh {maLenh} đã được kích hoạt, không được phép hủy kích hoạt!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                e.Cancel = true;
                return;
            }
        }



        private void simpleButton6_Click(object sender, EventArgs e)
        {
            var dt = grcLenhChia.DataSource as DataTable;
            if (dt == null) return;

            // Lấy tất cả dòng có Status = true
            var selectedRows = dt.AsEnumerable()
                                 .Where(r => r["Status"] != DBNull.Value && Convert.ToBoolean(r["Status"]))
                                 .ToList();

            // Không có dòng nào được chọn
            if (selectedRows.Count == 0)
            {
                XtraMessageBox.Show("Vui lòng chọn lệnh để kích hoạt.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra xem tất cả dòng được chọn đã kích hoạt hay chưa
            bool allActivated = selectedRows.All(r => r["isCheckKichHoat"] != DBNull.Value && Convert.ToInt32(r["isCheckKichHoat"]) == 1);

            if (allActivated)
            {
                XtraMessageBox.Show("Tất cả các lệnh đã được kích hoạt trước đó.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            List<CanDoiDonViSanXuatSaveEntity> listKichHoat = new List<CanDoiDonViSanXuatSaveEntity>();

            // Nếu còn dòng chưa kích hoạt → kích hoạt chúng
            foreach (var row in selectedRows)
            {
                CanDoiDonViSanXuatSaveEntity itemKichHoat = new CanDoiDonViSanXuatSaveEntity();
                itemKichHoat.MaLenh = row["MaLenh"].ToString();

                row["isCheckKichHoat"] = 1;
                listKichHoat.Add(itemKichHoat);
            }

            string url = string.Format("{0}", URL + $"ERPDonHangTong/Post?action=PostKichHoat&type=@TypeTable");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, listKichHoat); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                grcLenhChia.RefreshDataSource();
                clsWaitForm.ShowSuccessForm(this, 2000);
            }

        }



        private void btnXuatEX_Click(object sender, EventArgs e)
        {
            DataTable dtTable = grcChiTietCanDoi.DataSource as DataTable;
            if (dtTable == null || dtTable.Rows.Count == 0)
            {
                MessageBox.Show("Dữ liệu rỗng");
                return;
            }
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("ChiaChuyenSanXuat{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "ChiaChuyenSanXuat.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                Export(TemplateFileName, ExportFileName, dtTable);

                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Mở File", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        if (File.Exists(Sfd.FileName))
                            System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                    }
                    catch
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Error..");
                    }
                }
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            }
        }

        private void grvCanDoi_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow dr = grvCanDoi.GetFocusedDataRow();
            if (dr == null)
                return;
            //DinhMucHaoHut
            if (e.Column.FieldName == "DinhMucHaoHut")
            {
                decimal soLuong = 0, dinhMuc = 0, dinhMucHaoHut = 0;

                // TryParse an toàn cho từng cột
                decimal.TryParse(Convert.ToString(dr["SoLuong"]), out soLuong);
                decimal.TryParse(Convert.ToString(dr["DinhMuc"]), out dinhMuc);
                decimal.TryParse(Convert.ToString(dr["DinhMucHaoHut"]), out dinhMucHaoHut);

                // Tính CapPhat
                decimal capPhat = Math.Round(soLuong * dinhMuc + soLuong * dinhMuc * (dinhMucHaoHut / 100),2);

                dr["CapPhat"] = capPhat;

                // Refresh hiển thị
                grvCanDoi.RefreshRow(e.RowHandle);
            }
        }

        public void Export(string TemplateFileName, string ExportFileName, DataTable dataTable)
        {
            try
            {
                FileInfo file = new FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    excelPackage.Workbook.Properties.Author = "Cty NTB";
                    excelPackage.Workbook.Properties.Title = "";

                    string[] dateTime = DateTime.Now.ToString("dd/MM/yyyy").Split('/');

                    FileInfo templateFile = new FileInfo(TemplateFileName);

                    var tbl = dataTable.AsEnumerable()
                           .GroupBy(x => new { Line = x["Line"]?.ToString(), Name = x["Name"]?.ToString() })
                           .Select(g => g.First())
                           .CopyToDataTable();
                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {
                        using (ExcelPackage templatePackage = new ExcelPackage(templateFile))
                        {
                            ExcelWorksheet templateSheet = templatePackage.Workbook.Worksheets["Sheet1"];

                            // Tạo worksheet mới từ template
                            ExcelWorksheet ws = excelPackage.Workbook.Worksheets.Add($"{tbl.Rows[i]["Name"]}", templateSheet);

                            ExcelRange range = ws.Cells;

                            DataTable tblDT = dataTable.AsEnumerable().Where(x => x["Line"].ToString() == tbl.Rows[i]["Line"].ToString()).CopyToDataTable();

                            List<string> columnNamesWithSize = tblDT.Columns.Cast<DataColumn>()
                                 .Where(column => column.ColumnName.Contains("@"))
                                 .Select(column => column.ColumnName)
                                 .Distinct()
                                 .ToList();
                            int colum = 6;
                            int countRow = 8;
                            range = ws.Cells[6, 6, 6, 6 + columnNamesWithSize.Count - 1]; range.Merge = true; range.Value = "Size";
                            range = ws.Cells[6, 6 + columnNamesWithSize.Count, 7, 6 + columnNamesWithSize.Count]; range.Merge = true;
                            range.Value = "Tổng";

                            foreach (var item in columnNamesWithSize)
                            {
                                var size = item.Split('@')[0];
                                ws.Cells[7, colum].Value = size;

                                colum++;
                            }
                            foreach (DataRow item in tblDT.Rows)
                            {
                                colum = 6;
                                ws.Cells[countRow, 1].Value = item["PO"];
                                ws.Column(1).AutoFit();
                                ws.Cells[countRow, 2].Value = item["DauSize"];
                                ws.Cells[countRow, 3].Value = item["TenMau"];
                                ws.Cells[countRow, 4].Value = item["CodeMau"];
                                ws.Cells[countRow, 5].Value = item["Name"];
                                ws.Column(2).AutoFit();
                                ws.Column(3).AutoFit();
                                ws.Column(4).AutoFit();
                                foreach (var size in columnNamesWithSize)
                                {
                                    var valueSize = item[size];
                                    ws.Cells[countRow, colum].Value = Convert.ToInt32(valueSize) == 0 ? "" : valueSize;
                                    colum++;
                                }
                                ws.Cells[countRow, colum].Value = item["Amount"];
                                countRow++;
                            }
                            int sumAmount = tblDT.AsEnumerable().Sum(x => Convert.ToInt32(x["Amount"]));
                            // Tạo list để lưu tổng từng cột size
                            List<decimal> sizeColumnSums = new List<decimal>();

                            // Duyệt qua từng cột size (tên trong columnNamesWithSize)
                            foreach (var columnName in columnNamesWithSize)
                            {
                                decimal sum = 0;
                                foreach (DataRow row in tblDT.Rows)
                                {
                                    if (row[columnName] != DBNull.Value)
                                    {
                                        // cố gắng parse thành decimal (hoặc double tùy bạn)
                                        decimal value;
                                        if (decimal.TryParse(row[columnName].ToString(), out value))
                                        {
                                            sum += value;
                                        }
                                    }
                                }
                                sizeColumnSums.Add(sum);
                            }
                            colum = 6;
                            range = ws.Cells[countRow, 1, countRow, 5]; range.Merge = true; range.Value = "Tổng";

                            foreach (var item in sizeColumnSums)
                            {
                                ws.Cells[countRow, colum].Value = item;
                                colum++;
                            }
                            ws.Cells[countRow, colum].Value = sumAmount;
                            ws.Cells[6, 1, countRow, colum].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[6, 1, countRow, colum].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            range = ws.Cells[1, 1, 2, colum]; range.Merge = true; range.Value = "CHIA CHUYỀN SẢN XUẤT";
                            range.Style.Font.Bold = true; range.Style.Font.Size = 18;
                            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            var dataRange = ws.Cells[6, 1, countRow, colum];
                            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            DataRow drRow = grvLenhChia.GetFocusedDataRow();
                            if (drRow == null) return;
                            ws.Cells["A4"].Value = $"Mã ĐH: {drRow["MaDH"].ToString()}";
                            ws.Cells["B4"].Value = $"LSX: {drRow["MaLenh"].ToString()}";
                            ws.Cells["D4"].Value = $"Mã hàng: {drRow["MaHang"].ToString()}";
                            ws.Cells["E4"].Value = $"Khách hàng: {drRow["TenKH"].ToString()}";
                            ws.Cells["H4"].Value = $"Chủng loại: {drRow["TenCL"].ToString()}";
                        }
                    }

                    // Save all sheets into one file
                    excelPackage.SaveAs(file);
                }
            }
            catch (Exception ex)
            {
                // Bạn nên log hoặc throw lỗi để dễ debug
                Console.WriteLine(ex.Message);
            }
        }
        private void btnXacNhanPOMua_Click(object sender, EventArgs e)
        {
            int[] selectedHandles = grvCanDoi.GetSelectedRows();
            string POMua = (txtPOMua.EditValue as string) ?? "";
            if (POMua == "") return;
            if (selectedHandles.Length == 0)
            {
                XtraMessageBox.Show("Vui lòng chọn ít nhất một dòng trước khi lưu!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (int handle in selectedHandles)
            {
                DataRow row = grvCanDoi.GetDataRow(handle);
                if (row != null)
                {
                    row["POMua"] = POMua;
                }
            }

            grvCanDoi.RefreshData();
            txtPOMua.EditValue = "";
        }
        private void btnSavePOMua_Click(object sender, EventArgs e)
        {
            SaveCanDoi("PostPOMua");
        }
        private void btnXacNhanCanDoi_Click(object sender, EventArgs e)
        {
            if(GlobleData.UserName =="admin" || GlobleData.UserName== "QLDH_01")
            {
                SaveCanDoi("XacNhanCanDoi");
            }
            else{
                
                XtraMessageBox.Show("Bạn không có quyền thực hiện thao tác này!!!",
                   "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
           
        }
        private void SaveCanDoi(string action)
        {
            List<CanDoiDonViSanXuatDuyetSaveEntity> listSave = new List<CanDoiDonViSanXuatDuyetSaveEntity>();
            var tblSource = grcCanDoi.DataSource as DataTable;
            if (tblSource == null) return;
            // Duyệt từng dòng trong Grid
            foreach (DataRow row in tblSource.Rows)
            {
                string poid = row["POMua"].ToString();
                int IdRow = Convert.ToInt32(row["ID"]);
                decimal dinhmuchaohut = 0;
                decimal.TryParse(Convert.ToString(row["DinhMucHaoHut"]), out dinhmuchaohut);
                decimal capphat = 0;
                decimal.TryParse(Convert.ToString(row["CapPhat"]), out capphat);
                CanDoiDonViSanXuatDuyetSaveEntity itemSave = new CanDoiDonViSanXuatDuyetSaveEntity();

                itemSave.POID = poid;
                itemSave.ID = IdRow;
                itemSave.SoLuong = dinhmuchaohut;
                itemSave.STTLenh = capphat;
                itemSave.Line = row["GhiChu"].ToString();
                listSave.Add(itemSave);
            }

            if (listSave == null || listSave.Count == 0)
            {
                XtraMessageBox.Show("Không có dữ liệu để lưu.");
                return;
            }
            string url = string.Format("{0}", URL + $"ERPDonHangTong/PostV1?action={action}&type=@TypeTable");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, listSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
            if (action == "XacNhanCanDoi")
            {
                LoadCanDoi();
            }
        }
        private void grvLenhChia_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {

        }
    }
}