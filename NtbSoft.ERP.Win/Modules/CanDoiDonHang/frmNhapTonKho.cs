using DevExpress.Data;
using DevExpress.DataAccess.Excel;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout.Utils;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Modules.Erp.Kehoach;
using NtbSoft.ERP.Win.Modules.QuanLyDonHang;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.CanDoiDonHang
{
    public partial class frmNhapTonKho : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        //DataTable _dtTable;
        List<int> lstRowUpdate;
        bool indicatorIcon = true;
        bool isEdit = false;
        string maDH = string.Empty, maHang = string.Empty, maKH = string.Empty, maCL = string.Empty, dot = string.Empty;
        int check = 0;
        List<string> lstSize;
        int sum = 0;
        int _rowAdd = -1;
        string mau; string size; string dausize;
        DataTable tblChiTiet;
        DataTable tblMaHang;
        DataTable tblMau;
        DataTable tbl;
        DataTable _dtTable;
        DataTable _dtSizeNhom;
        DataTable _grid;
        DataTable tblChiTietold;
        DataTable tblKHDTTonKho;
        SearchCheckSelection gridCheckMarksSize;

        KeyDownControlHandler keyDownControlHandler;

        SearchCheckSelection gridCheckMarksColor;
        SearchCheckSelection gridCheckMarksDauSize;
        private List<string> _unsavedSelectedSizes = new List<string>();
        public frmNhapTonKho(bool _isEdit, DataTable _tbl, DataTable _tblChiTiet, DataTable _tblMaHang, DataTable _tblMau)
        {
            
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            _status = ResourceURL.EventStatus.View;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            //_dtTable = new DataTable();
            lstSize = new List<string>();
            isEdit = _isEdit;
            lstRowUpdate = new List<int>();
            tblChiTiet = _tblChiTiet.Copy();
            tblChiTietold = _tblChiTiet.Copy();
            tblMaHang = _tblMaHang;
            tblMau = _tblMau;
            tbl = _tbl;
            checkEdit1.Checked = true;
            checkEditTV.Checked = true;
            CreateDefaultSearchLookUpHH();
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }


        private void createSearchLookup()
        {
            string urlKhachHang = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string jsonKhachHang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKhachHang); }).Result;
            DataTable _tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKhachHang);
            searchLookUpEditKH.Properties.DataSource = _tblKH;

            //string urlDH = string.Format("{0}", URL + "DonHangTonKho/GetDonHangTonKho");
            //string jsonDH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDH); }).Result;
            //DataTable tblDH = JsonConvert.DeserializeObject<DataTable>(jsonDH);
            //searchLookUpEditDH.Properties.DataSource = tblDH;
            string urlMua = string.Format("{0}?", URL + "Mua/Get");
            string jsonMua = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMua); }).Result;
            DataTable tblMua = JsonConvert.DeserializeObject<DataTable>(jsonMua);
            textDot.Properties.TextEditStyle = TextEditStyles.Standard;
            textDot.Properties.NullText = "";
            textDot.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            textDot.Properties.DataSource = tblMua;
            textDot.Properties.ValueMember = "MaMua";
            textDot.Properties.DisplayMember = "Mua";
            textDot.ProcessNewValue += (s, e) =>
            {
                if (e.DisplayValue == null)
                    return;

                string txt = e.DisplayValue.ToString();

                // Thêm nếu chưa có trong datasource
                DataTable tbl = textDot.Properties.DataSource as DataTable;
                DataRow[] found = tbl.Select($"Mua = '{txt.Replace("'", "''")}'");

                if (found.Length == 0)
                {
                    DataRow r = tbl.NewRow();
                    r["MaMua"] = txt;
                    r["Mua"] = txt;
                    tbl.Rows.Add(r);
                }

                // Gán lại giá trị mới nhập
                textDot.EditValue = txt;
                e.Handled = true;
            };

            // DonViSanXuat
            string urlDVSX = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
            string jsonDVSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDVSX); }).Result;
            DataTable tblDVSX = JsonConvert.DeserializeObject<DataTable>(jsonDVSX);
            searchLookUpEditDVSX.Properties.DataSource = tblDVSX;
            if (tblDVSX != null && tblDVSX.Rows.Count > 0)
            {
                searchLookUpEditDVSX.EditValue = tblDVSX.Rows[0]["MaDVSX"];
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            
            createSearchLookup();
            //CreateDatatable();
            CreateSearchLookupHT();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            dateEditNgayTao.DateTime = DateTime.Now;
            InitForm(tblChiTiet, tblMau, tbl);
        }

        private void InitForm(DataTable _tblMaHang, DataTable _tblMau, DataTable tbl)
        {

            string urlNV = string.Format("{0}?", URL + "NhanVien/GetAllNV");
            string jsonNV = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNV); }).Result;
            DataTable tblnv = JsonConvert.DeserializeObject<DataTable>(jsonNV);
            if (isEdit)
            {
                this.Text = "Sửa tồn kho";
                //this.Xoa.Visibility = BarItemVisibility.Never;
                //this.Luu.Enabled = false;
                //this.Them.Visibility = BarItemVisibility.Never;
            }
            else
            {
                this.Text = "Nhập tồn kho";
                //this.Them.Visibility = BarItemVisibility.Never;
                this.Xoa.Visibility = BarItemVisibility.Never;
                this.Naplai.Visibility = BarItemVisibility.Never;
                //this.Luu.Enabled = false;
                this.barButtonItem2.Visibility = BarItemVisibility.Never;
            }

            //searchLookUpEditMH.Properties.DataSource = _tblMaHang;
            repositoryMauSearchLookUpEdit.DataSource = _tblMau;
            createSearchLookup();

            if (isEdit)
            {

                layoutControlItem2.Visibility = LayoutVisibility.Never;
                layoutControlItem18.Visibility = LayoutVisibility.Never;
                layoutControlItem22.Visibility = LayoutVisibility.Never;
                layoutControlItem19.Visibility = LayoutVisibility.Never;
                layoutControlItem8.Visibility = LayoutVisibility.Never;

                emptySpaceItem6.Visibility = LayoutVisibility.Never;
                emptySpaceItem7.Visibility = LayoutVisibility.Never;

                layoutControlItem20.Visibility = LayoutVisibility.Never;
                //emptySpaceItem5.Visibility = LayoutVisibility.Never;

                layoutControlItem14.Visibility = LayoutVisibility.Never;
                layoutControlItem15.Visibility = LayoutVisibility.Never;

                layoutControlItem11.Visibility = LayoutVisibility.Never;
                searchLookUpEditDVSX.Properties.ReadOnly = true;
                //searchLookUpEditDH.Properties.ReadOnly = true;
                searchLookUpEditMH.Properties.ReadOnly = true;
                searchLookUpEditCL.Properties.ReadOnly = true;
                searchLookUpEditKH.Properties.ReadOnly = true;
                //txtSoVoice.Properties.ReadOnly = true;
                txtGhiChu.Properties.ReadOnly = true;
                textDot.Properties.ReadOnly = true;
                txtSoBooking.Properties.ReadOnly = true;

                this.gridColPO.OptionsColumn.AllowEdit = false;
                this.gridColMau.OptionsColumn.AllowEdit = false;
                this.gridColDauSize.OptionsColumn.AllowEdit = false;

                //this.txtSoVoice.Enabled = false;
                this.txtGhiChu.Enabled = false;
                this.txtNguoiTao.Enabled = true;
                barButtonItem1.Visibility = BarItemVisibility.Never;


                if (tbl != null && tbl.Rows.Count > 0)
                {
                    DataRow dataRow = tbl.Rows[0];
                    foreach (DataColumn column in tbl.Columns)
                    {


                        if (column.ColumnName.Equals("MaDVSX", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataRow["MaDVSX"].ToString()))
                        {
                            searchLookUpEditDVSX.EditValue = dataRow["MaDVSX"];
                        }
                        //else if (column.ColumnName.Contains("MaDH") && !string.IsNullOrEmpty(dataRow["MaDH"].ToString()))
                        //{
                        //    searchLookUpEditDH.EditValue = dataRow["MaDH"];
                        //}
                        else if (column.ColumnName.Equals("MaKH", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataRow["MaKH"].ToString()))
                        {
                            searchLookUpEditKH.EditValue = dataRow["MaKH"];
                        }
                        else if (column.ColumnName.Equals("MaHang", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataRow["MaHang"].ToString()))
                        {
                            searchLookUpEditMH.EditValue = dataRow["MaHang"];
                        }
                        else if (column.ColumnName.Equals("MaCL", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataRow["MaCL"].ToString()))
                        {
                            searchLookUpEditCL.EditValue = dataRow["MaCL"];
                        }
                        else if (column.ColumnName.Equals("SoVoice", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataRow["SoVoice"].ToString()))
                        {
                            //txtSoVoice.Text = dataRow["SoVoice"].ToString();
                        }
                        else if (column.ColumnName.Equals("GhiChu", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataRow["GhiChu"].ToString()))
                        {
                            txtGhiChu.Text = dataRow["GhiChu"].ToString();
                        }
                        else if (column.ColumnName.Equals("NguoiTao", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataRow["NguoiTao"].ToString()))
                        {
                            txtNguoiTao.Text = dataRow["NguoiTao"].ToString();
                        }
                        else if (column.ColumnName.Equals("Dot", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataRow["Dot"].ToString()))
                        {
                            string val = dataRow["Dot"].ToString();
                            DataTable data = textDot.Properties.DataSource as DataTable;

                            DataRow[] found = data.Select($"MaMua = '{val.Replace("'", "''")}'");
                            if (found.Length == 0)
                            {
                                DataRow r = data.NewRow();
                                r["MaMua"] = val;
                                r["Mua"] = val;
                                data.Rows.Add(r);
                            }

                            textDot.EditValue = val;
                        }
                        else if (column.ColumnName.Equals("BookingMaHang", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataRow["BookingMaHang"].ToString()))
                        {
                            txtSoBooking.Text = dataRow["BookingMaHang"].ToString();
                        }
                        else if (column.ColumnName.Equals("FOB", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataRow["FOB"].ToString()))
                        {
                            spinEditFOB.EditValue = dataRow["FOB"].ToString();
                        }
                        else if (column.ColumnName.Equals("HeSoDH", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataRow["HeSoDH"].ToString()))
                        {
                            spinEditHSDH.EditValue = dataRow["HeSoDH"].ToString();
                        }
                        else if (column.ColumnName.Equals("CM", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataRow["CM"].ToString()))
                        {
                            spinEditUSD.EditValue = dataRow["CM"].ToString();
                        }
                        else if (column.ColumnName.Equals("HinhThucXuatHang", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dataRow["CM"].ToString()))
                        {
                            searchlookupEditHT.EditValue = dataRow["HinhThucXuatHang"].ToString();
                        }
                    }
                }
                tblChiTiet = RemoveColumn(_tblMaHang);
                CreateSearchLookupQuocGia();
                //CreateSearchLookupMaMau();
                CreateSearchLookupDauSize();
                gridControl1.MainView = CreateBandSize(tblChiTiet);
                //BandedGridView mainView = (BandedGridView)gridControl1.MainView;
                BandedGridView mainView = (BandedGridView)gridControl1.MainView;

                //mainView.CustomUnboundColumnData += mainView_CustomUnboundColumnData;
                mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                mainView.CustomDrawCell += MainView_CustomDrawCell;
                //mainView.CustomDrawFooter += BandedView_CustomDrawFooter;

                gridControl1.DataSource = tblChiTiet;
                //searchLookUpEditDVSX.EditValue=tbl[""]

                DataTable dataSource = gridControl1.DataSource as DataTable;
                if (!dataSource.Columns.Contains("CheckColumn"))
                {
                    dataSource.Columns.Add("CheckColumn", typeof(bool));
                }

                // Gán giá trị mặc định true cho cột "CheckColumn" cho tất cả các dòng trong DataTable
                foreach (DataRow row in dataSource.Rows)
                {
                    row["CheckColumn"] = true;
                }
            }
            else
            {
                string username = GlobleData.UserName.ToString();
                DataRow ten = tblnv.AsEnumerable().FirstOrDefault(r => string.Equals(r.Field<string>("UserID")?.Trim(), username.Trim(), StringComparison.OrdinalIgnoreCase));
                txtNguoiTao.Text = ten != null ? ten["Ten"].ToString() : username.ToString();
            }
        }

        private void focused(object sender)
        {

            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (view.FocusedColumn == null)
            {
                return;
            }
            if (isEdit)
            {
                //if (view.FocusedColumn.FieldName.Contains("@SIZE@") || view.FocusedColumn.FieldName.Equals("MaMau"))
                //    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                //else
                //    view.FocusedColumn.OptionsColumn.AllowEdit = false;

                bool isSIZEColumn = view.FocusedColumn.FieldName.Contains("@SIZE@");
                bool isSizeColumn = view.FocusedColumn.FieldName.Contains("@Size@");

                DataRow row = view.GetDataRow(view.FocusedRowHandle);
                bool isNewRow = false;

                if (row != null && row.Table.Columns.Contains("IsNew"))
                {
                    var val = row["IsNew"];

                    if (val != DBNull.Value && val != null && val.ToString() != "")
                        isNewRow = Convert.ToBoolean(val);
                }

                if (isSizeColumn || isNewRow || isSIZEColumn)
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                else
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
            }
            else
            {
                // && view.FocusedRowHandle.Equals("") == true
                if (_status == ResourceURL.EventStatus.Add)
                {
                    if (_rowAdd != -1 && view.FocusedRowHandle == _rowAdd || view.FocusedColumn.FieldName.Contains("IsNew"))
                    {
                        bool isNew = Convert.ToBoolean(view.GetFocusedRowCellValue("IsNew"));

                        if (isNew)
                        {
                            if (!view.FocusedColumn.FieldName.Equals("Amount"))
                            {
                                view.FocusedColumn.OptionsColumn.AllowEdit = true;
                            }
                        }

                    }
                    else
                    {
                        foreach (GridColumn column in view.Columns)
                        {
                            column.OptionsColumn.AllowEdit = !column.FieldName.Equals("PO");
                        }
                    }

                }
                else
                {
                    if (_status == ResourceURL.EventStatus.View)
                    {
                        if (view.FocusedColumn.FieldName.Equals("PO"))
                            view.FocusedColumn.OptionsColumn.AllowEdit = false;
                        else
                            view.FocusedColumn.OptionsColumn.AllowEdit = true;
                    }
                }
            }

        }

        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, BtLuu, true, ActionType.Save);
            AddActionControl(_lstActionControl, BtNapLai, true, ActionType.Refresh);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }

        private void BandedView_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;
            //e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            e.Appearance.ForeColor = Color.Red;
        }

        private void BandedView_CustomDrawFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void bandedView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (e.Band == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Band.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);

            // Chỉnh font chữ cho bandHeader
            e.Info.Appearance.Options.UseTextOptions = true;
            e.Info.Appearance.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            // Chỉnh alignment cho bandHeader
            e.Info.Appearance.Options.UseTextOptions = true;
            e.Info.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            e.Info.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void BtLuu()
        {
            //DataTable tblunit =
            //string msSave = string.Empty;
            DataTable tblunit = UnPivot(gridControl1.DataSource as DataTable);
            string SoBooking = txtSoBooking.Text?.ToString();

            if (isEdit)
            {
                bool checkDuplicateData = tblunit.AsEnumerable().GroupBy(row => new
                {
                    DauSize = row.Field<string>("DauSize"),
                    POID = row.Field<string>("POID"),
                    PO = row.Field<string>("PO"),
                    MaMau = row.Field<string>("MaMau"),
                    Size = row.Field<string>("Size"),
                    SizeID = row.Field<string>("SizeID")
                }).Any(g => g.Count() > 1);
                if (checkDuplicateData)
                {
                    XtraMessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
                if (checkEditTV.Checked == true)
                {
                    if (dtSize == null && dtSize.Rows.Count < 0)
                    {
                        MessageBox.Show("Size thuộc mã hàng  " + searchLookUpEditMH.EditValue.ToString().ToString() + " chưa được khai báo trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                        return;
                    }
                    var missingRows = from row1 in tblunit.AsEnumerable()
                                      where !dtSize.AsEnumerable().Any(row2 =>
                                      row1["MaHang"].ToString() == row2["MaHang"].ToString() &&
                                      row1["Size"].ToString() == row2["SizeSanXuat"].ToString() &&
                                      row1["DauSize"].ToString() == row2["NhomSize"].ToString())
                                      select row1;
                    if (missingRows.Any())
                    {
                        // Nhóm các dòng bị thiếu theo NhomSize
                        var groupedMissing = missingRows
                            .GroupBy(row => row["DauSize"].ToString())
                            .Select(group => new
                            {
                                NhomSize = group.Key, // Tên nhóm size
                            Sizes = group.Select(row => row["Size"].ToString()).Distinct() // Các size bị thiếu
                        });

                        // Tạo chuỗi thông báo
                        var message = string.Join("\n", groupedMissing.Select(group =>
                            $"Nhóm {group.NhomSize} : thiếu các size:  {string.Join(", ", group.Sizes)}  thuộc mã hàng {searchLookUpEditMH.EditValue.ToString()}"));

                        // Hiển thị thông báo
                        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

            }
            else
            {
                if (searchLookUpEditKH.EditValue == null || searchLookUpEditKH.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Khách Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (searchLookUpEditMH.EditValue == null || searchLookUpEditMH.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Mã Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                //if (searchLookUpEditCL.EditValue == null || searchLookUpEditCL.EditValue == "")
                //{
                //    MessageBox.Show("Vui lòng chọn Chủng Loại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //    return;
                //}
                if (textDot.Text == "" || textDot.Text == null)
                {
                    MessageBox.Show("Vui lòng nhập Đợt.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                //if (txtSoBooking.Text == null || txtSoBooking.Text == "")
                //{
                //    MessageBox.Show("Vui lòng nhập số Booking.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //    return;
                //}
                if (gridControl1.DataSource == null)
                {
                    MessageBox.Show("Chưa nhập danh sách chi tiết, Không thể lưu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                bool checkDuplicateData = _dtTable.AsEnumerable().GroupBy(row => new
                {
                    DauSize = row.Field<string>("DauSize"),
                    POID = row.Field<string>("POID"),
                    PO = row.Field<string>("PO"),
                    MaMau = row.Field<string>("MaMau"),
                    MaQG = row.Field<string>("MaQG")
                }).Any(g => g.Count() > 1);
                if (checkDuplicateData)
                {
                    XtraMessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
                if (checkEditTV.Checked == true)
                {
                    if (dtSize == null && dtSize.Rows.Count < 0)
                    {
                        MessageBox.Show("Size thuộc mã hàng  " + searchLookUpEditMH.EditValue.ToString().ToString() + " chưa được khai báo trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                        return;
                    }
                    var missingRows = from row1 in _dtSizeNhom.AsEnumerable()
                                      where !dtSize.AsEnumerable().Any(row2 =>
                                          row1["MaHang"].ToString() == row2["MaHang"].ToString() &&
                                          row1["SizeSanXuat"].ToString() == row2["SizeSanXuat"].ToString() &&
                                          row1["NhomSize"].ToString() == row2["NhomSize"].ToString())
                                      select row1;
                    if (missingRows.Any())
                    {
                        // Nhóm các dòng bị thiếu theo NhomSize
                        var groupedMissing = missingRows
                            .GroupBy(row => row["NhomSize"].ToString())
                            .Select(group => new
                            {
                                NhomSize = group.Key, // Tên nhóm size
                                Sizes = group.Select(row => row["SizeSanXuat"].ToString()).Distinct() // Các size bị thiếu
                            });

                        // Tạo chuỗi thông báo
                        var message = string.Join("\n", groupedMissing.Select(group =>
                            $"InSeam {group.NhomSize} : thiếu các size:  {string.Join(", ", group.Sizes)}  thuộc mã hàng {searchLookUpEditMH.EditValue.ToString()}"));

                        // Hiển thị thông báo
                        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }

            if (_dtTable.Columns.Contains("CheckColumn"))
            {
                _dtTable.Columns.Remove("CheckColumn");
            }

            if (_dtTable.Columns.Contains("IsNew"))
            {
                _dtTable.Columns.Remove("IsNew");
            }

            if (tblunit.Columns.Contains("CheckColumn"))
            {
                tblunit.Columns.Remove("CheckColumn");
            }

            if (tblunit.Columns.Contains("IsNew"))
            {
                tblunit.Columns.Remove("IsNew");
            }
            this.ActiveControl = this.searchLookUpEditMH;
            string msSave = string.Empty;
            string str_fail_save_input = "fail_input";
            DataTable tblSave;
            if (isEdit)
            {
                if (GlobleData.UserName.ToString().ToUpper() == txtNguoiTao.Text.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
                {
                    if (lstRowUpdate != null && lstRowUpdate.Count > 0)
                    {
                        tblSave = Update();
                        if (tblSave != null && tblSave.Rows.Count > 0)
                        {
                            if (checkEditTV.Checked == false)
                            {
                                var MauRows = tblSave.AsEnumerable()
                                        .GroupBy(row => new
                                        {
                                            MaMau = row["MaMau"],
                                            ColorCode = row["ColorCode"]
                                        })
                                        .Select(g => g.First())
                                        .ToList();

                                var distinctSizeRows = tblSave.AsEnumerable()
                                        .GroupBy(row => new
                                        {
                                            SizeID = row["SizeID"],
                                            DauSizeID = row["DauSizeID"]
                                        })
                                    .Select(g => g.First())
                                    .ToList();


                                string urlDSTheMauct = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                                string jsonDSTheMauct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSTheMauct); }).Result;
                                DataTable themau = JsonConvert.DeserializeObject<DataTable>(jsonDSTheMauct);

                                string urlsizecheck = $"{URL}BangSize/GetBangSizebom?para={searchLookUpEditMH.EditValue.ToString()}&para1={searchLookUpEditKH.EditValue.ToString()}";
                                string jsonsizecheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlsizecheck); }).Result;
                                DataTable tblSizecheck = JsonConvert.DeserializeObject<DataTable>(jsonsizecheck);

                                string urlmaucheck = $"{URL}BangMau/GetBangMau3?para={searchLookUpEditMH.EditValue.ToString()}&para2={searchLookUpEditKH.EditValue.ToString()}";
                                string jsonmaucheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlmaucheck); }).Result;
                                DataTable tblmaucheck = JsonConvert.DeserializeObject<DataTable>(jsonmaucheck);

                                DataTable tbmauSave = CreateDatatableTheMau();
                                //bangmau
                                foreach (var rowm in MauRows)
                                {
                                    var tenmau = themau.AsEnumerable().FirstOrDefault(r => r["MaMau"].ToString() == rowm["MaMau"].ToString());
                                    string mamau = rowm["MaMau"].ToString();
                                    string colorCode = rowm["ColorCode"]?.ToString().Trim() ?? mamau;

                                    bool existsm = tblmaucheck.AsEnumerable().Any(r =>
                                        r["MaMau"].ToString() == mamau
                                    );
                                    if (!existsm)
                                    {
                                        DataRow newRow = tbmauSave.NewRow();
                                        newRow["ID"] = 0;
                                        newRow["MaMau"] = mamau;
                                        newRow["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                                        newRow["TenMau"] = tenmau["TenMau"];
                                        newRow["CodeMau"] = tenmau["CodeMau"];
                                        newRow["GhiChu"] = "";
                                        newRow["MaKH"] = searchLookUpEditKH.EditValue.ToString();

                                        var thuVienMauRow = themau.AsEnumerable()
                                            .FirstOrDefault(r => (r["MaMau"]?.ToString() ?? "").Trim()
                                                .Equals(mamau, StringComparison.OrdinalIgnoreCase));

                                        if (thuVienMauRow != null)
                                        {
                                            newRow["MaTheMau"] =
                                                thuVienMauRow["MaTheMau"]?.ToString() ??
                                                thuVienMauRow["TheMau"]?.ToString() ??
                                                "";
                                        }
                                        else
                                        {
                                            newRow["MaTheMau"] = ""; // nếu không có dữ liệu mapping
                                        }
                                        tbmauSave.Rows.Add(newRow);
                                    }

                                }
                                //bangsize
                                string urlDsTheSizect = string.Format("{0}", URL + $"ERPBangSize/Get?action=Getizev1");
                                string jsonDsTheSizect = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDsTheSizect); }).Result;
                                DataTable TheSize = JsonConvert.DeserializeObject<DataTable>(jsonDsTheSizect);

                                string urlDSTheInSeamct = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeamv1");
                                string jsonDSTheInSeamct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSTheInSeamct); }).Result;
                                DataTable TheInSeam = JsonConvert.DeserializeObject<DataTable>(jsonDSTheInSeamct);

                                var dictTheSize = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);
                                if (TheSize != null)
                                {
                                    foreach (DataRow r in TheSize.Rows)
                                    {
                                        // cố gắng map bằng SizeSanXuat (hoặc MaSize nếu bạn có)
                                        string key = (r["SizeSanXuat"]?.ToString() ?? "").Trim();
                                        if (!dictTheSize.ContainsKey(key)) dictTheSize[key] = r;
                                    }
                                }
                                DataTable tblsizeSave = CreateDatatableTheSize();
                                foreach (var rows in distinctSizeRows)
                                {
                                    string sizeID = rows["SizeID"].ToString();
                                    string dauSizeID = rows["DauSizeID"].ToString();
                                    string sizeText = rows["Size"]?.ToString() ?? "";
                                    string dauSizeText = rows["DauSize"]?.ToString() ?? "";
                                    bool exists = tblSizecheck.AsEnumerable().Any(r =>
                                        r["MaSize"].ToString() == sizeID &&
                                        r["MaNhomSize"].ToString() == dauSizeID
                                    );
                                    if (!exists)
                                    {

                                        DataRow rowSize = tblsizeSave.NewRow();
                                        rowSize["ID"] = 0;
                                        rowSize["MaSize"] = sizeID;
                                        rowSize["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                                        rowSize["CodeSize"] = sizeText;
                                        rowSize["TenSize"] = sizeText;
                                        rowSize["SizeSanXuat"] = sizeText;
                                        rowSize["GhiChu"] = "";
                                        rowSize["XacNhan"] = "";
                                        rowSize["SizeXacNhan"] = 0;
                                        rowSize["MaNhomSize"] = dauSizeID;
                                        rowSize["NhomSize"] = dauSizeText;
                                        rowSize["MaKH"] = searchLookUpEditKH.EditValue.ToString();

                                        // ✅ lấy MaTheSize từ thư viện size
                                        var rsTheSize = TheSize.AsEnumerable().FirstOrDefault(r =>
                                            (r["SizeSanXuat"]?.ToString() ?? "").Trim().Equals(sizeText.Trim(), StringComparison.OrdinalIgnoreCase)
                                        );
                                        rowSize["Sort"] = rsTheSize["Sort"].ToString();
                                        rowSize["MaTheSize"] = rsTheSize?["MaTheSize"]?.ToString() ?? "";

                                        // ✅ lấy MaTheInSeam từ thư viện inseam (ưu tiên MaInSeam → InSeam)
                                        var rsInSeam = TheInSeam.AsEnumerable().FirstOrDefault(r =>
                                            (r["MaInSeam"]?.ToString() ?? "").Trim().Equals(dauSizeID.Trim(), StringComparison.OrdinalIgnoreCase)
                                            || (r["InSeam"]?.ToString() ?? "").Trim().Equals(dauSizeText.Trim(), StringComparison.OrdinalIgnoreCase)
                                        );

                                        rowSize["MaTheInSeam"] = rsInSeam?["MaTheInSeam"]?.ToString() ?? "";

                                        tblsizeSave.Rows.Add(rowSize);
                                    }
                                }

                                if (tbmauSave != null && tbmauSave.Rows.Count > 0)
                                {
                                    string urlpostmau = string.Format("{0}?", URL + "DonHangTong/PostMau");
                                    string msResultmau = Task.Run(async () => { return await _clientExtension.PostAsync(urlpostmau, tbmauSave); }).Result;
                                    if (msResultmau.ToLower() != "true")
                                        XtraMessageBox.Show(msResultmau);
                                }


                                if (tblsizeSave != null && tblsizeSave.Rows.Count > 0)
                                {
                                    string urlpostsize = string.Format("{0}", URL + "DonHangTong/PostSize");
                                    string msResultsize = Task.Run(async () => { return await _clientExtension.PostAsync(urlpostsize, tblsizeSave); }).Result;
                                    if (msResultsize.ToLower() != "true")
                                        XtraMessageBox.Show(msResultsize);
                                }

                            }

                            //tblSave = RemoveColumn(tblSave);
                            //if (_status == ResourceURL.EventStatus.View)
                            //{
                            //    string url = string.Format("{0}?nguoiTao={1}", URL + "DonHangTonKho/UpdateSLTonKho",GlobleData.UserName);
                            //    msSave = string.Empty;
                            //    msSave = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
                            //}
                            string url = string.Format("{0}?nguoiTao={1}", URL + "DonHangTonKho/UpdateSLTonKho", GlobleData.UserName);
                            msSave = string.Empty;
                            msSave = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblSave); }).Result;
                        }
                        else
                        {
                            msSave = str_fail_save_input;
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Vui lòng thay đổi thông tin trước khi lưu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    XtraMessageBox.Show("User này không phải là người tạo, không có quyền sửa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            else
            {
                tblSave = Save();
                if (tblSave != null && tblSave.Rows.Count > 0)
                {
                    if (checkEditTV.Checked == false)
                    {
                        var MauRows = tblSave.AsEnumerable()
                                .GroupBy(row => new
                                {
                                    MaMau = row["MaMau"],
                                    ColorCode = row["ColorCode"]
                                })
                                .Select(g => g.First())
                                .ToList();

                        var distinctSizeRows = tblSave.AsEnumerable()
                                .GroupBy(row => new
                                {
                                    SizeID = row["SizeID"],
                                    DauSizeID = row["DauSizeID"]
                                })
                            .Select(g => g.First())
                            .ToList();


                        string urlDSTheMauct = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                        string jsonDSTheMauct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSTheMauct); }).Result;
                        DataTable themau = JsonConvert.DeserializeObject<DataTable>(jsonDSTheMauct);

                        string urlsizecheck = $"{URL}BangSize/GetBangSizebom?para={searchLookUpEditMH.EditValue.ToString()}&para1={searchLookUpEditKH.EditValue.ToString()}";
                        string jsonsizecheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlsizecheck); }).Result;
                        DataTable tblSizecheck = JsonConvert.DeserializeObject<DataTable>(jsonsizecheck);

                        string urlmaucheck = $"{URL}BangMau/GetBangMau3?para={searchLookUpEditMH.EditValue.ToString()}&para2={searchLookUpEditKH.EditValue.ToString()}";
                        string jsonmaucheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlmaucheck); }).Result;
                        DataTable tblmaucheck = JsonConvert.DeserializeObject<DataTable>(jsonmaucheck);

                        DataTable tbmauSave = CreateDatatableTheMau();
                        //bangmau
                        foreach (var rowm in MauRows)
                        {
                            var tenmau = themau.AsEnumerable().FirstOrDefault(r => r["MaMau"].ToString() == rowm["MaMau"].ToString());
                            string mamau = rowm["MaMau"].ToString();
                            string colorCode = rowm["ColorCode"]?.ToString().Trim() ?? mamau;

                            bool existsm = tblmaucheck.AsEnumerable().Any(r =>
                                r["MaMau"].ToString() == mamau
                            );
                            if (!existsm)
                            {
                                DataRow newRow = tbmauSave.NewRow();
                                newRow["ID"] = 0;
                                newRow["MaMau"] = mamau;
                                newRow["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                                newRow["TenMau"] = tenmau["TenMau"];
                                newRow["CodeMau"] = tenmau["CodeMau"];
                                newRow["GhiChu"] = "";
                                newRow["MaKH"] = searchLookUpEditKH.EditValue.ToString();

                                var thuVienMauRow = themau.AsEnumerable()
                                    .FirstOrDefault(r => (r["MaMau"]?.ToString() ?? "").Trim()
                                        .Equals(mamau, StringComparison.OrdinalIgnoreCase));

                                if (thuVienMauRow != null)
                                {
                                    newRow["MaTheMau"] =
                                        thuVienMauRow["MaTheMau"]?.ToString() ??
                                        thuVienMauRow["TheMau"]?.ToString() ??
                                        "";
                                }
                                else
                                {
                                    newRow["MaTheMau"] = ""; // nếu không có dữ liệu mapping
                                }
                                tbmauSave.Rows.Add(newRow);
                            }

                        }
                        //bangsize
                        string urlDsTheSizect = string.Format("{0}", URL + $"ERPBangSize/Get?action=Getizev1");
                        string jsonDsTheSizect = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDsTheSizect); }).Result;
                        DataTable TheSize = JsonConvert.DeserializeObject<DataTable>(jsonDsTheSizect);

                        string urlDSTheInSeamct = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeamv1");
                        string jsonDSTheInSeamct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSTheInSeamct); }).Result;
                        DataTable TheInSeam = JsonConvert.DeserializeObject<DataTable>(jsonDSTheInSeamct);

                        var dictTheSize = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);
                        if (TheSize != null)
                        {
                            foreach (DataRow r in TheSize.Rows)
                            {
                                // cố gắng map bằng SizeSanXuat (hoặc MaSize nếu bạn có)
                                string key = (r["SizeSanXuat"]?.ToString() ?? "").Trim();
                                if (!dictTheSize.ContainsKey(key)) dictTheSize[key] = r;
                            }
                        }
                        DataTable tblsizeSave = CreateDatatableTheSize();
                        foreach (var rows in distinctSizeRows)
                        {
                            string sizeID = rows["SizeID"].ToString();
                            string dauSizeID = rows["DauSizeID"].ToString();
                            string sizeText = rows["Size"]?.ToString() ?? "";
                            string dauSizeText = rows["DauSize"]?.ToString() ?? "";
                            bool exists = tblSizecheck.AsEnumerable().Any(r =>
                                r["MaSize"].ToString() == sizeID &&
                                r["MaNhomSize"].ToString() == dauSizeID
                            );
                            if (!exists)
                            {

                                DataRow rowSize = tblsizeSave.NewRow();
                                rowSize["ID"] = 0;
                                rowSize["MaSize"] = sizeID;
                                rowSize["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                                rowSize["CodeSize"] = sizeText;
                                rowSize["TenSize"] = sizeText;
                                rowSize["SizeSanXuat"] = sizeText;
                                rowSize["GhiChu"] = "";
                                rowSize["XacNhan"] = "";
                                rowSize["SizeXacNhan"] = 0;
                                rowSize["MaNhomSize"] = dauSizeID;
                                rowSize["NhomSize"] = dauSizeText;
                                rowSize["MaKH"] = searchLookUpEditKH.EditValue.ToString();
                                //rowSize["Sort"] = rows["Sort"].ToString();


                                // ✅ lấy MaTheSize từ thư viện size
                                var rsTheSize = TheSize.AsEnumerable().FirstOrDefault(r =>
                                    (r["SizeSanXuat"]?.ToString() ?? "").Trim().Equals(sizeText.Trim(), StringComparison.OrdinalIgnoreCase)
                                );
                                rowSize["Sort"] = rsTheSize["Sort"].ToString();

                                rowSize["MaTheSize"] = rsTheSize?["MaTheSize"]?.ToString() ?? "";

                                // ✅ lấy MaTheInSeam từ thư viện inseam (ưu tiên MaInSeam → InSeam)
                                var rsInSeam = TheInSeam.AsEnumerable().FirstOrDefault(r =>
                                    (r["MaInSeam"]?.ToString() ?? "").Trim().Equals(dauSizeID.Trim(), StringComparison.OrdinalIgnoreCase)
                                    || (r["InSeam"]?.ToString() ?? "").Trim().Equals(dauSizeText.Trim(), StringComparison.OrdinalIgnoreCase)
                                );

                                rowSize["MaTheInSeam"] = rsInSeam?["MaTheInSeam"]?.ToString() ?? "";

                                tblsizeSave.Rows.Add(rowSize);
                            }
                        }

                        if (tbmauSave != null && tbmauSave.Rows.Count > 0)
                        {
                            string urlpostmau = string.Format("{0}?", URL + "DonHangTong/PostMau");
                            string msResultmau = Task.Run(async () => { return await _clientExtension.PostAsync(urlpostmau, tbmauSave); }).Result;
                            if (msResultmau.ToLower() != "true")
                                XtraMessageBox.Show(msResultmau);
                        }


                        if (tblsizeSave != null && tblsizeSave.Rows.Count > 0)
                        {
                            string urlpostsize = string.Format("{0}", URL + "DonHangTong/PostSize");
                            string msResultsize = Task.Run(async () => { return await _clientExtension.PostAsync(urlpostsize, tblsizeSave); }).Result;
                            if (msResultsize.ToLower() != "true")
                                XtraMessageBox.Show(msResultsize);
                        }

                    }
                    //tblSave = RemoveColumn(tblSave);
                    string urlSave = string.Format("{0}?nguoiTao={1}", URL + "DonHangTonKho/PostNhapTonKho", GlobleData.UserName);
                    msSave = string.Empty;
                    msSave = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, tblSave); }).Result;
                }
            }

            if (msSave.ToLower().Equals("true"))
            {

                clsWaitForm.ShowSuccessForm(this, 3000);
                this.Close();
            }
            else if (msSave.Equals(str_fail_save_input))
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtLuu();
        }

        // Dùng cho trường hợp cập nhật lại số lượng
        private DataTable Update()
        {
            // this.ActiveControl = this.searchLookUpEditMH;
            DataTable tbl = (bandedGridView1.DataSource as DataView).Table;

            if (tbl.Columns.Contains("CheckColumn"))
            {
                tbl.Columns.Remove("CheckColumn");
            }
            if (_status == ResourceURL.EventStatus.View)
            {
                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();
                if (gridControl1.DataSource != null)
                {
                    DataTable tblUpdate = tbl.Clone();

                    for (int i = 0; i < lstRowUpdate.Count; i++)
                    {
                        DataRow dataRow = tbl.Rows[lstRowUpdate[i]];

                        // Tạo thêm biến rowUpdate vì không
                        // thể add trực tiếp biến dataRow vào tblUpdate
                        DataRow rowUpdate = tblUpdate.NewRow();
                        rowUpdate.ItemArray = tbl.Rows[lstRowUpdate[i]].ItemArray;

                        tblUpdate.Rows.Add(rowUpdate);
                    }
                    Console.WriteLine("Update_CLick");
                    tblUpdate = UnPivotEdit(tblUpdate);
                    return tblUpdate;
                }
            }
            else 
            {
                lstRowUpdate = lstRowUpdate.Distinct().OrderBy(x => x).ToList();
                if (gridControl1.DataSource != null)
                {
                    DataTable tblUpdate = tbl.Clone();

                    for (int i = 0; i < lstRowUpdate.Count; i++)
                    {
                        DataRow dataRow = tbl.Rows[lstRowUpdate[i]];

                        // Tạo thêm biến rowUpdate vì không
                        // thể add trực tiếp biến dataRow vào tblUpdate
                        DataRow rowUpdate = tblUpdate.NewRow();
                        rowUpdate.ItemArray = tbl.Rows[lstRowUpdate[i]].ItemArray;

                        tblUpdate.Rows.Add(rowUpdate);
                    }
                    Console.WriteLine("Update_CLick");
                    tblUpdate = UnPivotEdit(tblUpdate);
                    return tblUpdate;
                }
            }
            // Trường hợp này sử dụng khi thêm một 
            //else if (_status == ResourceURL.EventStatus.Add)
            //{
            //    // Check các field: PO, Màu, DauSize của dòng thêm vào ở cuối
            //    DataRow dataRow = tbl.Rows[tbl.Rows.Count - 1];
            //    if (dataRow["PO"] != null && string.IsNullOrEmpty(dataRow["PO"].ToString()))
            //    {
            //        XtraMessageBox.Show("Chưa nhập PO. Vui lòng nhập đầy đủ thông tin trước khi lưu dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return new DataTable();
            //    }

            //    if (dataRow["DauSize"] != null && string.IsNullOrEmpty(dataRow["DauSize"].ToString()))
            //    {
            //        XtraMessageBox.Show("Chưa nhập Đầu size. Vui lòng nhập đầy đủ thông tin trước khi lưu dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return new DataTable();
            //    }

            //    if (dataRow["MaMau"] != null && string.IsNullOrEmpty(dataRow["MaMau"].ToString()))
            //    {
            //        XtraMessageBox.Show("Chưa chọn Màu. Vui lòng nhập đầy đủ thông tin trước khi lưu dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return new DataTable();
            //    }

            //    DataTable tblInsert = tbl.Clone();
            //    // Lấy row cuối cùng để insert
            //    //DataRow newRow = tbl.Rows[tbl.Rows.Count - 1];
            //    DataRow newRow = tblInsert.NewRow();
            //    newRow.ItemArray = tbl.Rows[tbl.Rows.Count - 1].ItemArray;

            //    // check duplicate
            //    List<DataRow> dataRowCheckDupliCate = tbl.AsEnumerable().Where(x => (x["PO"].ToString().Equals(newRow["PO"]))
            //    && (x["DauSize"].ToString().Equals(newRow["DauSize"]))
            //    && (x["MaMau"].ToString().Equals(newRow["MaMau"]))).ToList();
            //    if (dataRowCheckDupliCate != null && dataRowCheckDupliCate.Count > 1)
            //    {
            //        XtraMessageBox.Show("PO, Màu, Đầu Size đã tồn tại. Vui lòng kiểm tra thông tin trước khi lưu dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return new DataTable();
            //    }

            //    if (tbl != null && tbl.Rows.Count > 0)
            //    {
            //        newRow["MaDH"] = tbl.Rows[0]["MaDH"];
            //    }
            //    tblInsert.Rows.Add(newRow);
            //    tblInsert = UnPivotEdit(tblInsert);
            //    return tblInsert;
            //}
            return new DataTable();

        }

        private DataTable Save()
        {
            //if (searchLookUpEditMH.EditValue == null || searchLookUpEditKH.EditValue == "" || searchLookUpEditCL.EditValue == "" || searchLookUpEditDVSX.EditValue == null || textDot.EditValue == null || txtSoBooking.EditValue == null)
            //{
            //    XtraMessageBox.Show("Vui lòng nhập đầy đủ thông tin trước khi lưu dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return new DataTable();
            //}


            if (searchLookUpEditMH.EditValue == null || searchLookUpEditMH.EditValue == "")
            {
                XtraMessageBox.Show("Vui lòng nhập đầy đủ Mã Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataTable();
            }
            if (searchLookUpEditKH.EditValue == null || searchLookUpEditKH.EditValue == "")
            {
                XtraMessageBox.Show("Vui lòng nhập đầy đủ Khách Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataTable();
            }
            if (searchLookUpEditCL.EditValue == null || searchLookUpEditCL.EditValue == "")
            {
                XtraMessageBox.Show("Vui lòng chọn Chủng Loại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataTable();
            }
            if (searchLookUpEditDVSX.EditValue == null || searchLookUpEditDVSX.EditValue == "")
            {
                XtraMessageBox.Show("Vui lòng Chọn Đơn Vị Sản Xuất.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataTable();
            }
            if (textDot.EditValue == null || textDot.EditValue == "")
            {
                XtraMessageBox.Show("Vui lòng nhập Đợt.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataTable();
            }
            if (txtSoBooking.EditValue == null || txtSoBooking.EditValue == "")
            {
                XtraMessageBox.Show("Vui lòng nhập số Booking.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataTable();
            }
            DataTable _dt = gridControl1.DataSource as DataTable;
            if (_dt.Columns.Contains("CheckColumn"))
            {
                _dt.Columns.Remove("CheckColumn");
            }
            if (_dt.Columns.Contains("IsNew"))
            {
                _dt.Columns.Remove("IsNew");
            }
            _dt = UnPivotAdd(_dt);
            return _dt;
        }

        private DataTable createTableTonKho()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(string));
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaCL", typeof(string));
            tbl.Columns.Add("MaDVSX", typeof(string));
            tbl.Columns.Add("MaHS", typeof(string));
            tbl.Columns.Add("SoVoice", typeof(string));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("SoLuong", typeof(string));
            tbl.Columns.Add("TrangThai", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("POID_T", typeof(string));
            tbl.Columns.Add("Dot", typeof(string));
            tbl.Columns.Add("BookingMaHang", typeof(string));
            tbl.Columns.Add("MaQG", typeof(string));
            tbl.Columns.Add("NgayGH", typeof(DateTime));
            tbl.Columns.Add("ColorCode", typeof(string));
            tbl.Columns.Add("FOB", typeof(float));
            tbl.Columns.Add("HeSoDH", typeof(float));
            tbl.Columns.Add("CM", typeof(float));
            tbl.Columns.Add("HinhThucXuatHang", typeof(string));
            tbl.Columns.Add("NgayDuKienVC", typeof(DateTime));
            tbl.Columns.Add("NgayThucTeVC", typeof(DateTime));
            tbl.Columns.Add("NgayXuatHang", typeof(DateTime));
            return tbl;
        }

        private DataTable UnPivotEdit(DataTable tblDSChiTiet)
        {

            DataTable tblSave = createTableTonKho();
            if (tblDSChiTiet == null || tblDSChiTiet.Rows.Count == 0) return null;
            //DataRow tbl_kho = tblKho.Rows[0];
            int indexStartSize = 7;
            //foreach (DataRow dataRows in tblDSChiTiet.Rows)
            //{
            //    for (int i = indexStartSize; i < dataRows.ItemArray.Length; i++)
            //    {
            //        Console.WriteLine("dataRows");
            //        //tblSave[""]
            //        DataRow dataRowSave = tblSave.NewRow();
            //        //dataRowSave["DauSizeID"];
            //        //dataRowSave["POID"];
            //        //dataRowSave["MaDVSX"];
            //        dataRowSave["MaDVSX"] = searchLookUpEditDVSX.EditValue;
            //        //dataRowSave["MaDH"] = searchLookUpEditDH.EditValue;
            //        dataRowSave["MaHang"] = searchLookUpEditMH.EditValue;
            //        dataRowSave["MaKH"] = searchLookUpEditKH.EditValue;
            //        dataRowSave["MaCL"] = searchLookUpEditCL.EditValue;
            //        dataRowSave["MaHS"] = txtMaHS.Text;

            //        //dataRowSave["NguoiTao"] = txtNguoiTao.Text;
            //        //dataRowSave["NgayTao"] = Convert.ToDateTime(dateEditNgayTao.EditValue.ToString());
            //        dataRowSave["SoVoice"] = txtSoVoice.Text;
            //        dataRowSave["POID"] = string.Format("{0}|{1}", searchLookUpEditMH.EditValue, dataRows["PO"]);
            //        dataRowSave["PO"] = dataRows["PO"];
            //        dataRowSave["MaMau"] = dataRows["MaMau"];
            //        dataRowSave["DauSize"] = dataRows["DauSize"];
            //        dataRowSave["MaDH"] = dataRows["MaDH"];

            //        string doidausize = Regex.Replace(dataRows["DauSize"].ToString(), @"[^\w\d]+", "_");
            //        string doidausizeid = RemoveOneUnderscore(doidausize);
            //        if (!string.IsNullOrEmpty(doidausizeid))
            //        {
            //            dataRowSave["DauSizeID"] = doidausizeid;
            //        }
            //        dataRowSave["GhiChu"] = dataRows["GhiChu"];
            //        dataRowSave["Size"] = lstSize[i - indexStartSize];
            //        dataRowSave["SoLuong"] = dataRows.ItemArray[i];
            //        //dataRowSave["Dot"] = textDot.EditValue;
            //        //dataRowSave["BookingMaHang"] = txtSoBooking.EditValue;
            //        tblSave.Rows.Add(dataRowSave);
            //    }
            //}
            try
            {
                foreach (DataRow dataRows in tblDSChiTiet.Rows)
                {
                    foreach (DataColumn column in tblDSChiTiet.Columns)
                    {
                        if (column.ColumnName.Contains("@SIZE@"))
                        {
                            string[] arrNewHeader = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                            string sizesx = arrNewHeader[0];
                            string sizeid = arrNewHeader[2];
                            Console.WriteLine("dataRows");
                            //tblSave[""]
                            DataRow dataRowSave = tblSave.NewRow();
                            //dataRowSave["DauSizeID"];
                            //dataRowSave["POID"];
                            //dataRowSave["MaDVSX"];
                            dataRowSave["MaDH"] = dataRows["MaDH"];
                            dataRowSave["MaDVSX"] = searchLookUpEditDVSX.EditValue;
                            dataRowSave["MaHang"] = searchLookUpEditMH.EditValue;
                            dataRowSave["MaKH"] = searchLookUpEditKH.EditValue;
                            dataRowSave["MaCL"] = searchLookUpEditCL.EditValue;
                            dataRowSave["SoVoice"] = "";
                            dataRowSave["POID"] = string.Format("{0}|{1}", searchLookUpEditMH.EditValue, ReplaceSpecialCharacters(RemoveVietnameseTone(dataRows["PO"].ToString())));
                            dataRowSave["PO"] = dataRows["PO"];
                            dataRowSave["MaMau"] = dataRows["MaMau"];
                            dataRowSave["DauSize"] = dataRows["DauSize"];
                            dataRowSave["MaQG"] = dataRows["MaQG"];
                            dataRowSave["NgayGH"] = dataRows["NgayGH"];
                            dataRowSave["ColorCode"] = dataRows["ColorCode"] == "" ? dataRows["MaMau"] : dataRows["ColorCode"];

                            string doidausize = Regex.Replace(dataRows["DauSize"].ToString(), @"[^\w\d]+", "_");
                            string doidausizeid = RemoveOneUnderscore(doidausize);
                            if (!string.IsNullOrEmpty(doidausizeid))
                            {
                                dataRowSave["DauSizeID"] = doidausizeid;
                            }
                            dataRowSave["GhiChu"] = dataRows["GhiChu"];
                            dataRowSave["SizeID"] = sizeid;
                            dataRowSave["Size"] = sizesx;
                            dataRowSave["SoLuong"] = dataRows[column];
                            dataRowSave["POID_T"] = ReplaceSpecialCharacters(RemoveVietnameseTone(dataRows["PO"].ToString()));
                            dataRowSave["Dot"] = textDot.EditValue;
                            dataRowSave["BookingMaHang"] = txtSoBooking.EditValue;
                            dataRowSave["FOB"] = (float)Convert.ToDouble(spinEditFOB.EditValue);
                            dataRowSave["HeSoDH"] = (float)Convert.ToDouble(spinEditFOB.EditValue);
                            dataRowSave["CM"] = (float)Convert.ToDouble(spinEditHSDH.EditValue);
                            dataRowSave["HinhThucXuatHang"] = searchlookupEditHT.EditValue.ToString().Trim();
                            //dataRowSave["NgayDuKienVC"] = Convert.ToDateTime(dataRows["NgayDuKienVC"]);
                            //dataRowSave["NgayThucTeVC"] = Convert.ToDateTime(dataRows["NgayThucTeVC"]);
                            //dataRowSave["NgayXuatHang"] = Convert.ToDateTime(dataRows["NgayXuatHang"]);
                            dataRowSave["NgayDuKienVC"] = string.IsNullOrEmpty(dataRows["NgayDuKienVC"].ToString()) ? (object)DBNull.Value : Convert.ToDateTime(dataRows["NgayDuKienVC"].ToString());
                            dataRowSave["NgayThucTeVC"] = string.IsNullOrEmpty(dataRows["NgayThucTeVC"].ToString()) ? (object)DBNull.Value : Convert.ToDateTime(dataRows["NgayThucTeVC"]);
                            dataRowSave["NgayXuatHang"] = string.IsNullOrEmpty(dataRows["NgayXuatHang"].ToString()) ? (object)DBNull.Value : Convert.ToDateTime(dataRows["NgayXuatHang"]);
                            tblSave.Rows.Add(dataRowSave);
                        }

                    }

                }
            } 
            catch (Exception e) 
            {

            }
            
            Console.WriteLine("end for");
            return tblSave;
        }

        private DataTable UnPivotAdd(DataTable tblDSChiTiet)
        {
            DataTable tblSave = createTableTonKho();
            if (tblDSChiTiet == null || tblDSChiTiet.Rows.Count == 0) return null;
            //DataRow tbl_kho = tblKho.Rows[0];
            int indexStartSize = 5;
            try
            {

                foreach (DataRow dataRows in tblDSChiTiet.Rows)
                {
                    foreach (DataColumn column in tblDSChiTiet.Columns)
                    {
                        if (column.ColumnName.Contains("@Size@"))
                        {
                            string[] arrNewHeader = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                            string sizeID = arrNewHeader[0];
                            string sizeSX = arrNewHeader[2];
                            Console.WriteLine("dataRows");
                            string mau = dataRows["MaMau"].ToString();
                            DataRow dataRowSave = tblSave.NewRow();
                            dataRowSave["MaDH"] = "";
                            dataRowSave["MaDVSX"] = searchLookUpEditDVSX.EditValue;
                            dataRowSave["MaHang"] = searchLookUpEditMH.EditValue.ToString().ToUpper();
                            dataRowSave["MaKH"] = searchLookUpEditKH.EditValue;
                            dataRowSave["MaCL"] = searchLookUpEditCL.EditValue;
                            dataRowSave["SoVoice"] = "";
                            dataRowSave["POID"] = string.Format("{0}|{1}", searchLookUpEditMH.EditValue, ReplaceSpecialCharacters(RemoveVietnameseTone(dataRows["PO"].ToString())));
                            dataRowSave["PO"] = dataRows["PO"].ToString();
                            dataRowSave["MaMau"] = mau.ToString().ToUpper().Trim();
                            dataRowSave["DauSize"] = dataRows["DauSize"];
                            dataRowSave["MaQG"] = SearchLookUpEditQG.EditValue;
                            dataRowSave["NgayGH"] = dataRows["NgayGH"];
                            dataRowSave["ColorCode"] = string.IsNullOrEmpty(dataRows["ColorCode"].ToString()) ? mau : dataRows["ColorCode"];

                            string doidausize = Regex.Replace(dataRows["DauSize"].ToString(), @"[^\w\d]+", "_");
                            string doidausizeid = RemoveOneUnderscore(doidausize);
                            if (!string.IsNullOrEmpty(doidausizeid))
                            {
                                dataRowSave["DauSizeID"] = doidausizeid;
                            }
                            dataRowSave["GhiChu"] = dataRows["GhiChu"];
                            dataRowSave["SizeID"] = sizeID;
                            dataRowSave["Size"] = sizeSX;
                            if (dataRows[column] == null || (dataRows[column] != null && string.IsNullOrEmpty(dataRows[column].ToString())))
                            {
                                dataRowSave["SoLuong"] = 0;
                            }
                            else
                            {
                                dataRowSave["SoLuong"] = dataRows[column];
                            }
                            dataRowSave["POID_T"] = ReplaceSpecialCharacters(RemoveVietnameseTone(dataRows["PO"].ToString()));
                            dataRowSave["Dot"] = textDot.EditValue;
                            dataRowSave["BookingMaHang"] = txtSoBooking.EditValue;
                            dataRowSave["MaQG"] = dataRows["MaQG"];
                            dataRowSave["NgayGH"] = dataRows["NgayGH"];
                            dataRowSave["ColorCode"] = string.IsNullOrEmpty(dataRows["ColorCode"].ToString()) ? dataRows["MaMau"] : dataRows["ColorCode"];
                            dataRowSave["FOB"] = (float)Convert.ToDouble(spinEditFOB.EditValue);
                            dataRowSave["HeSoDH"] = (float)Convert.ToDouble(spinEditHSDH.EditValue);
                            dataRowSave["CM"] = (float)Convert.ToDouble(spinEditUSD.EditValue); 
                            dataRowSave["HinhThucXuatHang"] = searchlookupEditHT.EditValue.ToString().Trim();
                            dataRowSave["NgayDuKienVC"] = string.IsNullOrEmpty(dataRows["NgayDuKienVC"].ToString()) ? (object)DBNull.Value : Convert.ToDateTime(dataRows["NgayDuKienVC"].ToString());
                            dataRowSave["NgayThucTeVC"] = string.IsNullOrEmpty(dataRows["NgayThucTeVC"].ToString()) ? (object)DBNull.Value : Convert.ToDateTime(dataRows["NgayThucTeVC"]);
                            dataRowSave["NgayXuatHang"] = string.IsNullOrEmpty(dataRows["NgayXuatHang"].ToString()) ? (object)DBNull.Value : Convert.ToDateTime(dataRows["NgayXuatHang"]);
                            tblSave.Rows.Add(dataRowSave);
                        }
                    }

                }

            }
            catch (Exception e) 
            {

            }
            Console.WriteLine("end for");
            return tblSave;
        }

        private DataTable UnPivot(DataTable tblDSChiTiet)
        {
            DataTable tblSave = createTableTonKho();
            if (tblDSChiTiet == null || tblDSChiTiet.Rows.Count == 0) return null;
            foreach (DataRow dataRows in tblDSChiTiet.Rows)
            {
                foreach (DataColumn column in tblDSChiTiet.Columns)
                {
                    if (column.ColumnName.Contains("@SIZE@"))
                    {
                        Console.WriteLine("dataRows");
                        //tblSave[""]
                        string[] arrNewHeader = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        string size = arrNewHeader[0];
                        string sizeID = arrNewHeader[2];
                        DataRow dataRowSave = tblSave.NewRow();
                        dataRowSave["MaDVSX"] = searchLookUpEditDVSX.EditValue;
                        dataRowSave["MaHang"] = searchLookUpEditMH.EditValue.ToString().ToUpper();
                        dataRowSave["MaKH"] = searchLookUpEditKH.EditValue;
                        dataRowSave["MaCL"] = searchLookUpEditCL.EditValue;
                        dataRowSave["SoVoice"] = "";
                        dataRowSave["POID"] = string.Format("{0}|{1}", searchLookUpEditMH.EditValue, dataRows["PO"]);
                        dataRowSave["PO"] = dataRows["PO"];
                        dataRowSave["MaMau"] = dataRows["MaMau"];
                        dataRowSave["DauSize"] = dataRows["DauSize"];
                        dataRowSave["MaQG"] = SearchLookUpEditQG.EditValue;
                        dataRowSave["NgayGH"] = dataRows["NgayGH"];
                        dataRowSave["GhiChu"] = dataRows["GhiChu"];
                        dataRowSave["ColorCode"] = dataRows["ColorCode"] == "" ? dataRows["MaMau"] : dataRows["ColorCode"];
                        dataRowSave["SizeID"] = sizeID;
                        dataRowSave["Size"] = size;
                        tblSave.Rows.Add(dataRowSave);
                    }
                }
            }
            Console.WriteLine("end for");
            return tblSave;
        }
        static string RemoveOneUnderscore(string input)
        {
            return Regex.Replace(input, @"_+", "_");
        }

        private void SearchLookupeditMH_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            maHang = changingEventArgs.NewValue.ToString();
            //CheckBtnLuu();
            //DevExpress.XtraEditors.Controls.ChangingEventArgs ValueEditChanged = e as DevExpress.XtraEditors.Controls.ChangingEventArgs;
            //if (!string.IsNullOrEmpty(ValueEditChanged.NewValue?.ToString()))
            //{

            //}
            InitSearInitSearchLookupMaMau();
            //InitSearchLookupSize();
            InitSearchLookupDauSize();
            CreateSearchLookupCL();

            string urlCount = string.Format("{0}?mahang={1}", URL + "DonHangTonKho/GetMHCount", searchLookUpEditMH.EditValue.ToString());
            string jsonCount = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCount); }).Result;
            DataTable tblCount = JsonConvert.DeserializeObject<DataTable>(jsonCount);

            int currentYear = DateTime.Now.Year;
            int countCurrentYear = 0;
            if (tblCount != null && tblCount.Rows.Count > 0)
            {
                countCurrentYear = tblCount.AsEnumerable()
                    .Count(r =>
                    {
                        DateTime ngayTao;
                        return DateTime.TryParse(r["NgayTao"]?.ToString(), out ngayTao) && ngayTao.Year == currentYear;
                    });
            }

            txtSoBooking.EditValue = currentYear + "-" + (countCurrentYear + 1);
        }

        private void SearchLookupeditKH_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            maKH = changingEventArgs.NewValue.ToString();
            //CheckBtnLuu();
        }

        // Hàm CheckDupLicateDataStorage: Check mã đơn hàng và mã hàng nhập vào đã có trong db chưa
        private bool CheckDupLicateDataStorage()
        {
            string urlktDonHangTK = string.Format("{0}?maDH={1}&&maHang={2}", URL + "DonHangTonKho/GetDSDonHangTonKho", searchLookUpEditDVSX.EditValue, searchLookUpEditMH.EditValue);
            string jsonktDonHangTK = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlktDonHangTK); }).Result;
            DataTable _tblDonHangTK = JsonConvert.DeserializeObject<DataTable>(jsonktDonHangTK);
            if (_tblDonHangTK != null && _tblDonHangTK.Rows.Count > 0)
            {
                XtraMessageBox.Show("Mã hàng này đã có trong kho. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
            return false;
        }
        // Hàm CheckDuplicateData: Check data nhập vào có bị trùng không
        private bool CheckDuplicateData()
        {
            // Check PO, Mau, DauSize, Size: Nếu có 1 phần tử trùng lại => Thông báo lỗi
            bool checkPO = txtBoxPO.Text.Split(new char[] { ':' }).ToList().GroupBy(x => x).Any(g => g.Count() > 1);
            int countPONullValue = txtBoxPO.Text.Split(new char[] { ':' }).ToList().Where(x => string.IsNullOrEmpty(x)).ToList().Count;
            //if (checkPO)
            //{
            //    MessageBox.Show("Dữ liệu PO nhập vào bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return true;
            //}
            //else if (countPONullValue > 0)
            //{
            //    MessageBox.Show("Dữ liệu PO nhập vào không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return true;
            //}

            bool checkMau = txtBoxSize.Text.Split(new char[] { ':' }).ToList().GroupBy(x => x).Any(g => g.Count() > 1);
            int countMauNullValue = txtBoxSize.Text.Split(new char[] { ':' }).ToList().Where(x => string.IsNullOrEmpty(x)).ToList().Count;
            if (checkMau)
            {
                MessageBox.Show("Dữ liệu Màu nhập vào bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            else if (countMauNullValue > 0)
            {
                MessageBox.Show("Dữ liệu Màu nhập vào không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            if(txtBoxDauSize.Text.ToString() != "")
            {
                bool checkDauSize = txtBoxDauSize.Text.Split(new char[] { ':' }).ToList().GroupBy(x => x).Any(g => g.Count() > 1);
                int countDauSizeNullValue = txtBoxDauSize.Text.Split(new char[] { ':' }).ToList().Where(x => string.IsNullOrEmpty(x)).ToList().Count;
                if (checkDauSize)
                {
                    MessageBox.Show("Dữ liệu Đầu Size nhập vào bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }
                else if (countDauSizeNullValue > 0)
                {

                    MessageBox.Show("Dữ liệu Đầu Size nhập vào không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }
            }    
           

            bool checkSize = txtBoxSize.Text.Split(new char[] { ':' }).ToList().GroupBy(x => x).Any(g => g.Count() > 1);
            int countSizeNullValue = txtBoxSize.Text.Split(new char[] { ':' }).ToList().Where(x => string.IsNullOrEmpty(x)).ToList().Count;
            if (checkSize)
            {
                MessageBox.Show("Dữ liệu Size nhập vào bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            else if (countSizeNullValue > 0)
            {
                MessageBox.Show("Dữ liệu Size nhập vào không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            return false;
        }
        //private DataTable MapData(DataTable _dtTable, DataTable tblChanged)
        //{
        //    // Tạo một DataTable tạm từ tblChanged. Chỉ giữ lại các cột số lượng size, xóa các cột còn lại
        //    // Dùng để map số lượng theo từng size cho bảng _dtTable
        //    // Sau khi xóa số lượng cột của _tblTempSLSize sẽ bằng với số lượng cột size của _dtTable
        //    DataTable _tblTempSLSize = tblChanged.Copy();
        //    for (int i = 0; i < _tblTempSLSize.Columns.Count;)
        //    {
        //        DataColumn column = _tblTempSLSize.Columns[i];
        //        List<string> _lstTemp = column.ColumnName.Split(new char[] { ' ' }).ToList();
        //        if (!_lstTemp[0].ToUpper().Equals("SIZE"))
        //        {
        //            _tblTempSLSize.Columns.RemoveAt(i);
        //        }
        //        else
        //        {
        //            i++;
        //        }
        //    }

        //    // POID, PO, MaMau, DauSize, MaQG, NgayGH, GhiChu
        //    foreach (DataRow row in tblChanged.Rows)
        //    {
        //        if (row[0].ToString() == "***")
        //        {
        //            break;
        //        }
        //        DataRow _rowAdd = _dtTable.NewRow();
        //        //_rowAdd["POID"] = txtMaDH.Text.ToString() + '|' + row[0].ToString();
        //        _rowAdd["PO"] = row[0].ToString();
        //        _rowAdd["MaMau"] = row[1];
        //        _rowAdd["DauSize"] = row[2].ToString();
        //        //_rowAdd["MaQG"] = "";
        //        //_rowAdd["NgayGH"] = DateTime.Now.Date;
        //        _rowAdd["GhiChu"] = txtGhiChu.Text;
        //        _dtTable.Rows.Add(_rowAdd);
        //    }

        //    // Map danh sách số lượng của từng size
        //    int index = 0;
        //    foreach (DataColumn column in _dtTable.Columns)
        //    {
        //        if (column.ColumnName.Contains("@Size@"))
        //        {
        //            for (int i = 0; i < _dtTable.Rows.Count; i++)
        //            {
        //                _dtTable.Rows[i][column] = _tblTempSLSize.Rows[i][index];
        //            }
        //            index += 1;
        //        }
        //    }

        //    return _dtTable;
        //}

        private void txtNhapPO_KeyPress(object sender, KeyPressEventArgs e)
        {
            int ascii = Convert.ToInt32(e.KeyChar);
            if (!(ascii == 58))
            {
                if (char.IsWhiteSpace(e.KeyChar) || !char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true; // Chặn ký tự nhập vào
                }
            }

            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }

        }


        private void txtNhapPO_TextChanged(object sender, EventArgs e)
        {
            txtBoxPO.Text = txtBoxPO.Text.ToString().ToUpper();
            txtBoxPO.SelectionStart = txtBoxPO.Text.Length;
        }


        private void txtNhapPO_Click(object sender, EventArgs e)
        {
            if (this.txtBoxPO.Text.Trim().ToUpper() == "[P1:P2]")
            {
                this.txtBoxPO.Text = "";
            }
        }

        private void txtNhapSize_Click(object sender, EventArgs e)
        {
            if (this.txtBoxSize.Text.Trim() == "[S1:S2]")
            {
                this.txtBoxSize.Text = "";
            }
        }


        private void txtNhapSize_TextChanged(object sender, EventArgs e)
        {
            txtBoxSize.Text = txtBoxSize.Text.ToString().ToUpper();
            txtBoxSize.SelectionStart = txtBoxSize.Text.Length;
        }



        private void txtNhapSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ':' && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }


        private void txtMau_TextChanged(object sender, EventArgs e)
        {
            txtBoxMau.Text = txtBoxMau.Text.ToString().ToUpper();
            txtBoxMau.SelectionStart = txtBoxMau.Text.Length;
        }

        private void txtMau_Click(object sender, EventArgs e)
        {
            if (this.txtBoxMau.Text.Trim().ToUpper() == "[M1:M2]")
            {
                this.txtBoxMau.Text = "";
            }
        }

        private void txtDauSize_TextChanged(object sender, EventArgs e)
        {
            txtBoxDauSize.Text = txtBoxDauSize.Text.ToString().ToUpper();
            txtBoxDauSize.SelectionStart = txtBoxDauSize.Text.Length;
        }

        private void txtDauSize_Click(object sender, EventArgs e)
        {
            if (this.txtBoxDauSize.Text.Trim().ToUpper() == "[D1:D2]")
            {
                this.txtBoxDauSize.Text = "";
            }
        }
        static string ReplaceSpecialCharacters(string input)
        {
            // Pattern để tìm khoảng trắng và các kí tự đặc biệt
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            // Thay thế bằng dấu _
            string replacement = "_";
            // Tạo một Regex và thực hiện thay thế
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }

        private void btNhap_Click(object sender, EventArgs e)
        {
            _grid = gridControl1.DataSource as DataTable;
            try
            {
                check = 0;
                if (_dtTable.Columns.Contains("CheckColumn"))
                {
                    _dtTable.Columns.Remove("CheckColumn");
                }
                if (searchLookUpEditKH.EditValue == null || searchLookUpEditKH.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Khách Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (searchLookUpEditMH.EditValue == null || searchLookUpEditMH.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Mã Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (searchLookUpEditCL.EditValue == null || searchLookUpEditCL.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Chủng Loại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (textDot.Text == "" || textDot.Text == null)
                {
                    MessageBox.Show("Vui lòng nhập Contract.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (txtSoBooking.Text == null || txtSoBooking.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập tần suất.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (mau == null || mau == "")
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin Màu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (dausize == null || dausize == "")
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin InSeam.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (size == null || size == "")
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin Size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (SearchLookUpEditQG.EditValue == null || SearchLookUpEditQG.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn Quốc gia.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (searchlookupEditHT.EditValue == null || searchlookupEditHT.EditValue == "")
                {
                    MessageBox.Show("Vui lòng chọn hình thức.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                //if (txtSoVoice.Text == null || txtSoVoice.Text == "")
                //{
                //    MessageBox.Show("Vui lòng nhập số invoice.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //    return;
                //}
                if (spinEditHSDH.Text == null || spinEditHSDH.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập hệ số đơn hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }

                if (spinEditFOB.Text == null || spinEditFOB.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập FOB.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                if (spinEditUSD.Text == null || spinEditUSD.Text == "")
                {
                    MessageBox.Show("Vui lòng nhập hệ số đơn hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                // Check Duplicate dữ liệu nhập: PO, Màu, DauSize, Size
                if (CheckDuplicateData())
                {
                    return;
                }

                // CheckDuplicate trên db: MaDH, MaHang
                //if (CheckDupLicateDataStorage())
                //{
                //    return;
                //}

                string _maHang = searchLookUpEditMH.EditValue.ToString();
                #region //
                //CreateDatatable();

                //// Check tồn tại size
                //// Nếu chưa có thì autoInport
                //if (txtBoxSize.Text != "")
                //{
                //    string urlsize = string.Format("{0}?", URL + "BangSize/GetBangSize");
                //    string jsonsize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlsize); }).Result;
                //    List<BangSizeEntity> lstBangSize = JsonConvert.DeserializeObject<List<BangSizeEntity>>(jsonsize);

                //    string textValueKTSize = txtBoxSize.Text;
                //    string[] valuesKTSize = textValueKTSize.Split(':');
                //    List<string> lstSizeSanXuat = new List<string>();
                //    foreach (var valueKT in valuesKTSize)
                //    {
                //        string KTSize = RemoveVietnameseTone(valueKT).ToUpper().Trim();
                //        BangSizeEntity bangSize = lstBangSize.Where(item => RemoveVietnameseTone(item.SizeSanXuat + item.MaHang).ToUpper().Trim() == KTSize.ToString() + _maHang).FirstOrDefault();
                //        if (bangSize == null && !string.IsNullOrEmpty(KTSize))
                //        {
                //            lstSizeSanXuat.Add(KTSize);
                //        }
                //    }
                //    AutoImportSize(lstSizeSanXuat, searchLookUpEditMH.EditValue.ToString().Trim(), false);
                //}

                //// Check và AutoImport màu
                //if (txtBoxMau.Text != "")
                //{
                //    string url = string.Format("{0}?", URL + "BangMau/GetBangMau");
                //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                //    List<BangMauEntity> lstBangMau = JsonConvert.DeserializeObject<List<BangMauEntity>>(json);

                //    List<string> lstSaveMau = new List<string>();
                //    string textValueKTMau = txtBoxMau.Text;
                //    string[] valuesKTMau = textValueKTMau.Split(':');
                //    foreach (var valueKT in valuesKTMau)
                //    {
                //        string KTMau = RemoveVietnameseTone(valueKT).ToUpper().Trim();
                //        BangMauEntity bangMau = lstBangMau.Where(item => RemoveVietnameseTone(item.TenMau + item.MaHang).ToUpper().Trim() == KTMau.ToString() + _maHang).FirstOrDefault();
                //        if (bangMau == null)
                //        {
                //            lstSaveMau.Add(KTMau);
                //        }
                //    }

                //    if (lstSaveMau.Count > 0)
                //    {
                //        AutoImportMau(lstSaveMau, searchLookUpEditMH.EditValue.ToString().Trim());
                //    }
                //}
                #endregion
                List<string> _lstSize = txtBoxSize.Text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> _lstDauSize = txtBoxDauSize.Text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                DataTable tblNhap = CreateDatatable();
                //tblNhap = AddColumnSize(tblNhap, _lstSize);

                // Thêm các dòng vào _dtTable từ PO, DauSize, Mau nhập vào
                List<string> _lstPo = txtBoxPO.Text.Split(new char[] { ':' }).ToList();
                List<string> _lstMau = txtBoxMau.Text.Split(new char[] { ':' }).ToList();
               //tblNhap = GenerateRowNhap(tblNhap, _lstPo, _lstDauSize, _lstMau);
                //DataTable _tblThongTin = CreateDataTableFromNhap();
                //tblNhap = ChangeTenMauToMaMau(tblNhap, _maHang);
                DataTable _tblSizeNhom = CreateDatatableSizeNhom();
                //gridControlThongTinDonHang.MainView = GetBandGridViewAmount(_dtTable);
                //BandedGridView mainView = (BandedGridView)gridControlThongTinDonHang.MainView;
                //mainView.CustomUnboundColumnData += mainView_CustomUnboundColumnData;
                //mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                //mainView.CellValueChanging += mainView_CellValueChanging;
                //gridControlThongTinDonHang.DataSource = _dtTable;
                //foreach (DataRow row in tblNhap.Rows)
                //{
                //    if (row["DauSize"].ToString() == "")
                //        row["DauSize"] = 0;
                //}
                //gridBandColSize.Children.Clear();
                //gridBandColSize.Columns.Clear();
                _dtTable = CreateDatatable();
                _dtSizeNhom = CreateDatatableSizeNhom();
                string jsonKtSize = string.Empty;
                string jsonMau = string.Empty;
                if (checkEditTV.Checked == false)
                {
                    string urlKtSize = string.Format("{0}", URL + $"ERPBangSize/Get?action=Getizev1");
                    jsonKtSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKtSize); }).Result;
                }
                else
                {
                    string urlKtSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetBangSizeKT", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                    jsonKtSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKtSize); }).Result;
                }
                DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(jsonKtSize);
                foreach (string Dausize in _lstDauSize)
                {
                    foreach (string size in _lstSize)
                    {
                        DataRow _dr = _dtSizeNhom.NewRow();
                        _dr["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                        _dr["SizeSanXuat"] = size.ToString().Trim();
                        _dr["MaNhomSize"] = Dausize.ToString().Replace(" ", "").Trim().ToUpper();
                        _dr["NhomSize"] = Dausize.ToString().Trim();
                        _dtSizeNhom.Rows.Add(_dr);
                    }
                }
                if (checkEditTV.Checked == true)
                {
                    var missingRows = from row1 in _dtSizeNhom.AsEnumerable()
                                      where !tblSize.AsEnumerable().Any(row2 =>
                                          row1["MaHang"].ToString() == row2["MaHang"].ToString() &&
                                          row1["SizeSanXuat"].ToString() == row2["SizeSanXuat"].ToString() &&
                                          row1["NhomSize"].ToString() == row2["NhomSize"].ToString())
                                      select row1;
                    if (missingRows.Any())
                    {
                        // Nhóm các dòng bị thiếu theo NhomSize
                        var groupedMissing = missingRows
                            .GroupBy(row => row["NhomSize"].ToString())
                            .Select(group => new
                            {
                                NhomSize = group.Key, // Tên nhóm size
                                Sizes = group.Select(row => row["SizeSanXuat"].ToString()).Distinct() // Các size bị thiếu
                            });

                        // Tạo chuỗi thông báo
                        var message = string.Join("\n", groupedMissing.Select(group =>
                            $"InSeam {group.NhomSize} : thiếu các size:  {string.Join(", ", group.Sizes)}  thuộc mã hàng {searchLookUpEditMH.EditValue.ToString()}"));

                        // Hiển thị thông báo
                        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                //if (missingRows.Any())
                //{
                //    string sizesanxuatMissing = string.Join("; ", missingRows.Select(r => r["SizeSanXuat"].ToString().Distinct().ToArray()));
                //    string nhomsizeMissing = string.Join("; ", missingRows.Select(r => r["NhomSize"].ToString()).Distinct().ToArray());
                //    MessageBox.Show("Size " + sizesanxuatMissing + " không có trong nhóm  " + nhomsizeMissing + " thuộc mã hàng  " + searchLookUpEditMH.EditValue.ToString() + " trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                //    return;
                //}
                var uniqueValues = _dtSizeNhom.AsEnumerable().Select(x => new { MaHang = x["MaHang"], SizeSanXuat = x["SizeSanXuat"] }).Distinct().ToList();
                foreach (var dr in uniqueValues)
                {
                    var rowmasize = tblSize.AsEnumerable().FirstOrDefault(r => r["SizeSanXuat"].ToString().Trim() == dr.SizeSanXuat.ToString().Trim());
                    if (rowmasize != null)
                        _dtTable.Columns.Add(rowmasize["MaSize"].ToString() + "@Size@" + dr.SizeSanXuat.ToString(), typeof(string));
                }
                DataTable _tblThongTin = CreateDataTableFromNhap();
                if (checkEditTV.Checked == false)
                {
                    string urlMau = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                    jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                }
                else
                {
                    string urlMau = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                    jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                }

                DataTable dtMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);

                if (checkEditTV.Checked == true)
                {
                    var RowsMau = from row1 in _tblThongTin.AsEnumerable()
                                  where !dtMau.AsEnumerable().Any(row2 =>
                                      searchLookUpEditMH.EditValue.ToString() == row2["MaHang"].ToString() &&
                                      row1["MauPO"].ToString() == row2["MaMau"].ToString())
                                  select row1;
                    if (RowsMau.Any())
                    {
                        string mauMissing = string.Join("; ", RowsMau.Select(r => r["MauPO"].ToString().Distinct().ToArray()));
                        MessageBox.Show("Màu " + mauMissing + " không có trong nhóm mã hàng " + searchLookUpEditMH.EditValue.ToString() + " trong thư viện màu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                        return;
                    }
                }

                foreach (DataRow _drmau in _tblThongTin.Rows)
                {
                    var rowmau = dtMau.AsEnumerable().FirstOrDefault(r => r["TenMau"].ToString().Trim() == _drmau["MauPO"].ToString().Trim());
                    if (rowmau != null)
                        _drmau["MauPO"] = rowmau["MaMau"];

                }

                if (_tblThongTin != null && _tblThongTin.Rows.Count > 0)
                {
                    _dtTable = MapData(_dtTable, _tblThongTin);
                }
                gridBandColSize.Children.Clear();
                //createSearchLookup();
                CreateSearchLookupQG();
                CreateSearchLookupQuocGia();
                CreateSearchLookupMaMau();
                CreateSearchLookupDauSize();
                gridControl1.MainView = CreateBandSize(_dtTable);
                BandedGridView mainView = (BandedGridView)gridControl1.MainView;
                mainView.CellValueChanging += mainView_CellValueChanging;
                //mainView.CustomUnboundColumnData += mainView_CustomUnboundColumnData;
                mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                mainView.CustomDrawCell += MainView_CustomDrawCell;
                gridControl1.DataSource = _dtTable;
                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thông tin nhập vào bị lỗi. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            DataTable dataSource = gridControl1.DataSource as DataTable;
            if (!dataSource.Columns.Contains("CheckColumn"))
            {
                dataSource.Columns.Add("CheckColumn", typeof(bool));
            }

            // Gán giá trị mặc định true cho cột "CheckColumn" cho tất cả các dòng trong DataTable
            foreach (DataRow row in dataSource.Rows)
            {
                row["CheckColumn"] = true;
            }
        }

        private DevExpress.XtraGrid.Views.Base.BaseView GetBandGridViewAmount(DataTable tab)
        {
            BandedGridView bandedView = bandedGridView1;
            bandedView.OptionsView.ShowColumnHeaders = false;
            bandedView.OptionsView.ShowGroupPanel = false;
            bandedView.OptionsView.ShowFooter = true;
            bandedView.OptionsView.AllowCellMerge = true;
            bandedView.OptionsCustomization.AllowBandMoving = false;
            if (tab.Columns.Count == 0) return bandedView;

            bandedView.CustomDrawBandHeader += bandedView_CustomDrawBandHeader;
            bandedView.CustomColumnDisplayText += bandedGridView1_CustomColumnDisplayText;
            bandedView.CustomDrawFooter += BandedView_CustomDrawFooter;
            bandedView.CustomDrawFooterCell += BandedView_CustomDrawFooterCell;
            //bandedView.PopupMenuShowing += BandedView_PopupMenuShowing;
            bandedView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            bandedView.RowCountChanged += BandedView_RowCountChanged;
            bandedView.CustomDrawRowIndicator += BandedView_CustomDrawRowIndicator;
            bandedView.KeyPress += bandedGridView1_KeyPress;
            bandedView.ValidatingEditor += bandedGridView1_ValidatingEditor;
            bandedView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bandedView.OptionsNavigation.EnterMoveNextColumn = true;
            return bandedView;
        }
        private void BandedView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {

            try
            {
                GridView view = (GridView)sender;
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {
                    string sText = (e.RowHandle + 1).ToString();
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString(sText, e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }
                if (!indicatorIcon)
                    e.Info.ImageIndex = -1;

                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void BandedView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }
        // Import: Đổi tên màu sang mã màu
        public DataTable ChangeTenMauToMaMau(DataTable tbl, string _maHang)
        {
            DataTable tblChange = tbl.Copy();
            // lấy danh sách bảng màu
            string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
            List<BangMauEntity> _lstBangMau = JsonConvert.DeserializeObject<List<BangMauEntity>>(json);
            repositoryMauSearchLookUpEdit.DataSource = _lstBangMau;
            string columnNameMauPO = "MAMAU";

            // Cập nhật field Màu PO
            for (int i = 0; i < tblChange.Rows.Count; i++)
            {
                foreach (DataColumn dataColumn in tblChange.Columns)
                {
                    if (RemoveDiacritics(dataColumn.ColumnName).Replace(" ", "").ToUpper().Equals(columnNameMauPO))
                    {
                        Console.WriteLine("ChangedTenMauToMaMau");
                        if (tblChange.Rows[i][dataColumn] != null)
                        {
                            string tenMau = tblChange.Rows[i][dataColumn].ToString();
                            // Lấy ra mã màu từ _lstBangMau bằng field TenMau và MaHang
                            BangMauEntity bangMauEntity = _lstBangMau.Where(x => x.TenMau == tenMau && x.MaHang == _maHang).FirstOrDefault();

                            if (bangMauEntity != null)
                            {
                                // Đổi DataTable
                                tblChange.Rows[i][dataColumn] = bangMauEntity.MaMau;
                            }
                            else
                            {
                                Console.WriteLine("Import màu không thành công => Không lấy được mã màu sau khi import");
                            }
                        }
                        break;
                    }
                }
            }

            Console.WriteLine("Change PO complete");
            return tblChange;
        }

        static string RemoveDiacritics(string text)
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

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        // Hàm CreateDataTableFromNhap() tạo ra DataTable chi tiết từ 4 field được nhập vào: PO, DauSize, Mau, SizeSanXuat
        // Chỉ dùng khi nhập, không dùng khi import
        private DataTable CreateDataTableFromNhap()
        {
            DataTable _dataTable = CreateColumnNhap();
            Console.WriteLine("CreateDataTableFromNhap");

            List<string> _lstPo = txtBoxPO.Text.Split(new char[] { ':' }).ToList();
            List<string> _lstDauSize = txtBoxDauSize.Text.ToString() == "" ? new List<string> { "0" } : txtBoxDauSize.Text.Split(new char[] { ':' }).ToList();
            List<string> _lstMau = mau.Split(new char[] { ':' }).ToList();
            _dataTable = GenerateRowNhap(_dataTable, _lstPo, _lstDauSize, _lstMau);
            return _dataTable;
        }

        // Hàm GenerateRowNhap tạo ra các Row dựa vào PO; Màu; Đầu Size; Size Sản Xuất nhập vào
        private DataTable GenerateRowNhap(DataTable _dtTable, List<string> _lstPO, List<string> _lstDauSize, List<string> _lstMau)
        {
            Console.WriteLine("GenerateRowNhap");
            foreach (string _po in _lstPO)
            {
                foreach (string _dauSize in _lstDauSize)
                {
                    foreach (string _mau in _lstMau)
                    {
                        DataRow row = _dtTable.NewRow();
                        row["ChungTuPO"] = _po;
                        row["MauPO"] = _mau;
                        row["MaNhomSize"] = _dauSize;
                        row["MaQG"] = SearchLookUpEditQG.EditValue;
                        _dtTable.Rows.Add(row);
                    }
                }
            }
            return _dtTable;
        }

        private DataTable CreateColumnNhap()
        {
            List<string> _lstSize = txtBoxSize.Text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            DataTable _dataTable = new DataTable();
            _dataTable.Columns.Add("ChungTuPo", typeof(string));
            _dataTable.Columns.Add("MauPO", typeof(string));
            _dataTable.Columns.Add("MaNhomSize", typeof(string));
            _dataTable.Columns.Add("MaQG", typeof(string));
            for (int i = 0; i < _lstSize.Count; i++)
            {
                _dataTable.Columns.Add(string.Format("Size {0}", _lstSize[i]), typeof(int));
            }
            return _dataTable;
        }
        private DataTable AddColumnSize(DataTable _tbl, List<string> _lstSize)
        {

            //foreach (string size in _lstSize)
            //{

            //    //bandedGridView1.Columns.Clear();
            //    _tbl.Columns.Add(string.Format("Size@{0}", size), typeof(double));
            //}
            //return _tbl;

            foreach (string size in _lstSize)
            {
                string columnName = string.Format("Size@{0}", size);
                if (!_tbl.Columns.Contains(columnName))
                {
                    _tbl.Columns.Add(columnName, typeof(double));
                }
            }
            return _tbl;
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

        private DataTable CreateDatatable()
        {
            DataTable tbl = new DataTable("dtTable");
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("MaQG", typeof(string));
            tbl.Columns.Add("NgayGH", typeof(DateTime));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("ColorCode", typeof(string));
            tbl.Columns.Add("NgayDuKienVC", typeof(DateTime));
            tbl.Columns.Add("NgayThucTeVC", typeof(DateTime));
            tbl.Columns.Add("NgayXuatHang", typeof(DateTime));
            return tbl;
        }
        private DataTable CreateDatatableSize()
        {
            DataTable tbl = new DataTable("dtSize");
            tbl.Columns.Add("MaSize", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("CodeSize", typeof(string));
            tbl.Columns.Add("TenSize", typeof(string));
            tbl.Columns.Add("SizeSanXuat", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("XacNhan", typeof(int));
            tbl.Columns.Add("SizeXacNhan", typeof(string));
            return tbl;
        }
        private void AutoImportSize(List<string> lstColumnName, string maHang, bool isFromImport)
        {
            List<BangSizeEntity> lstAutoImportSize = new List<BangSizeEntity>();
            BangSizeEntity autoImportSizeItem;
            //string maHang = searchLookUpEditMH.EditValue.ToString().Trim();
            for (int i = 0; i < lstColumnName.Count; i++)
            {
                string columnName = lstColumnName[i];

                // Khi import list column chứa tên size
                if (isFromImport)
                {
                    if (columnName.Contains("Size"))
                    {
                        string tenSize = columnName;
                        string sizeSX = string.Empty;
                        string[] nameSize = columnName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (nameSize != null && nameSize.Length > 1)
                        {
                            sizeSX = nameSize[1];
                            sizeSX = Regex.Replace(RemoveDiacritics(sizeSX), @"[^\w\d]+", "_");
                            autoImportSizeItem = new BangSizeEntity(sizeSX, tenSize, maHang);
                            lstAutoImportSize.Add(autoImportSizeItem);
                        }
                    }
                }
                else // Khi nhập list column chứa size sản xuất
                {
                    string sizeSX = columnName;
                    string tenSize = string.Format("Size {0}", columnName);
                    //string[] nameSize = columnName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    //sizeSX = nameSize[1];
                    autoImportSizeItem = new BangSizeEntity(sizeSX, tenSize, maHang);
                    lstAutoImportSize.Add(autoImportSizeItem);
                }
            }
            if (lstAutoImportSize != null && lstAutoImportSize.Count > 0)
            {
                string urlImportBangSize = string.Format("{0}", URL + "BangSize/PostAutoImportBangSize");
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlImportBangSize, lstAutoImportSize); }).Result;
            }
        }

        private void AutoImportMau(List<string> lstSaveMau, string maHang)
        {
            List<BangMauEntity> lstAutoImportMau = new List<BangMauEntity>();
            BangSizeEntity autoImportSizeItem;
            //string maHang = searchLookUpEditMH.EditValue.ToString().Trim();

            if (lstSaveMau != null && lstSaveMau.Count > 0)
            {
                lstSaveMau = lstSaveMau.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstSaveMau.Count; i++)
                {
                    BangMauEntity itemBangMau = new BangMauEntity(maHang, lstSaveMau[i],"","");
                    lstAutoImportMau.Add(itemBangMau);
                }
                if (lstAutoImportMau.Count > 0)
                {
                    string urlAutoImportMau = string.Format("{0}?", URL + "BangMau/PostAutoImportBangMau");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlAutoImportMau, lstAutoImportMau); }).Result;
                }
            }
        }

        private void btImport_Click(object sender, EventArgs e)
        {
            try
            {
                check = 1;
                if (searchLookUpEditMH.EditValue == null || (searchLookUpEditMH.EditValue != null && string.IsNullOrEmpty(searchLookUpEditMH.EditValue.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Mã Hàng trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (searchLookUpEditKH.EditValue == null || (searchLookUpEditKH.EditValue != null && string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Khách Hàng trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (searchLookUpEditCL.EditValue == null || (searchLookUpEditCL.EditValue != null && string.IsNullOrEmpty(searchLookUpEditCL.EditValue.ToString())))
                {
                    MessageBox.Show("Vui lòng nhập Chủng Loại trước khi import vào.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // CheckDuplicate trên db: MaDH, MaHang
                if (CheckDupLicateDataStorage())
                {
                    return;
                }
                //_dtTable = CreateDatatable();

                // Import excel
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Select an Excel File";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pathExecel = openFileDialog.FileName;
                    string conString = "", sheet1 = string.Empty;
                    //int totalAmount = 0, Amount = 0;
                    using (OleDbConnection connection = new OleDbConnection(conString))
                    {
                        var source = new ExcelDataSource();
                        source.FileName = pathExecel;
                        string worksheetName = "";
                        using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(pathExecel))
                        {
                            DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                            worksheetName = worksheetCollection[0].Name;
                        }
                        var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A5:BZ500");
                        source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                        source.Fill();
                        DataTable tbl_ThongTinSize = new DataTable();
                        tbl_ThongTinSize = source.ToDataTable();

                        const string namePOExcel = "CHUNGTUPO";
                        const string namePO = "PO";
                        const string nameMauExcel = "MAUPO";
                        const string nameMau = "MaMau";
                        const string nameDauSizeExcel = "MANHOMSIZE";
                        const string nameDauSize = "DauSize";

                        if (tbl_ThongTinSize != null && tbl_ThongTinSize.Rows.Count > 0)
                        {

                            for (int j = 0; j < tbl_ThongTinSize.Rows.Count - 1; j++)
                            {
                                //
                                foreach (DataColumn column in tbl_ThongTinSize.Columns)
                                {
                                    string removeDiacrColumnName = RemoveDiacritics(column.ColumnName).Replace(" ", "").ToUpper();
                                    if (removeDiacrColumnName.Equals(namePOExcel) || removeDiacrColumnName.Equals(nameMauExcel))
                                    {
                                        string value = string.Empty;
                                        if (tbl_ThongTinSize.Rows[j][column] != null)
                                        {
                                            value = tbl_ThongTinSize.Rows[j][column].ToString().Replace(" ", "");
                                        }
                                        if (string.IsNullOrEmpty(value))
                                        {
                                            switch (removeDiacrColumnName)
                                            {
                                                case namePOExcel:
                                                    XtraMessageBox.Show("PO không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    return;
                                                    break;
                                                case nameMauExcel:
                                                    XtraMessageBox.Show("Màu không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    return;
                                                    break;
                                                //case nameDauSizeExcel:
                                                //    XtraMessageBox.Show("Đầu Size không được để trống và không nên chứa các kí tự đặc biệt. Có thể phát sinh do dư cột hãy kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                //    return;
                                                //    break;
                                            }

                                        }
                                    }
                                }
                            }
                            tbl_ThongTinSize = RefactorColumnImport(tbl_ThongTinSize, namePOExcel, namePO, nameMauExcel, nameMau, nameDauSizeExcel, nameDauSize);

                            ////
                            //for (int col = 3; col < tbl_ThongTinSize.Columns.Count; col++)
                            //{
                            //    string ColName = tbl_ThongTinSize.Columns[col].ColumnName.ToString();
                            //    string[] mang_gia_tri = ColName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            //    if (ProcessString(RemoveDiacritics(ColName.ToString())) == "SOLUONG")
                            //        break;
                            //    foreach (string gia_tri in mang_gia_tri)
                            //    {
                            //        if (ProcessString(RemoveDiacritics(gia_tri.ToString())) != "SIZE")
                            //            _dtTable.Columns.Add(gia_tri + "@Size@" + gia_tri, typeof(double));
                            //    }
                            //}

                            // Check và import Size
                            List<string> lstImportSize = new List<string>();
                            string urlsize = string.Format("{0}?", URL + "BangSize/GetBangSize");
                            string jsonsize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlsize); }).Result;
                            List<BangSizeEntity> lstBangSize = JsonConvert.DeserializeObject<List<BangSizeEntity>>(jsonsize);
                            string _maHang = searchLookUpEditMH.EditValue.ToString().Trim();

                            if (tbl_ThongTinSize != null && tbl_ThongTinSize.Rows.Count > 0)
                            {
                                //
                                foreach (DataColumn column in tbl_ThongTinSize.Columns)
                                {
                                    if (column.ColumnName.Contains("Size@"))
                                    {
                                        List<string> lstNameSize = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        if (lstNameSize != null && lstNameSize.Count > 1)
                                        {
                                            {
                                                string sizeSX = lstNameSize[1];
                                                BangSizeEntity bangSize = lstBangSize.Where(item => RemoveVietnameseTone(item.SizeSanXuat + item.MaHang).ToString().ToUpper().Trim().Replace(" ", "")
                                                == (sizeSX + _maHang).ToString().ToUpper().Trim().Replace(" ", "")).FirstOrDefault();
                                                if (bangSize == null && column != null && !string.IsNullOrEmpty(column.ColumnName))
                                                {
                                                    lstImportSize.Add(column.ColumnName);
                                                }
                                            }
                                        }
                                    }
                                }
                                //if (lstImportSize.Count > 0)
                                //{
                                //    AutoImportSize(lstImportSize, searchLookUpEditMH.EditValue.ToString().Trim(), true);
                                //}
                                // Size

                                // Check và import màu
                                string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
                                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                                List<BangMauEntity> lstBangMau = JsonConvert.DeserializeObject<List<BangMauEntity>>(json);

                                List<string> lstSaveMau = new List<string>();

                                for (int j = 0; j < tbl_ThongTinSize.Rows.Count; j++)
                                {
                                    foreach (DataColumn column in tbl_ThongTinSize.Columns)
                                    {
                                        if (column.ColumnName.Contains(nameMau))
                                        {
                                            BangMauEntity bangMau = lstBangMau.Where(item => RemoveVietnameseTone(item.TenMau + item.MaHang).ToString().ToUpper().Trim().Replace(" ", "") == (tbl_ThongTinSize.Rows[j][column].ToString().Trim() + searchLookUpEditMH.EditValue.ToString().Trim()).ToString().ToUpper().Trim().Replace(" ", "")).FirstOrDefault();
                                            if (bangMau == null)
                                            {
                                                lstSaveMau.Add(tbl_ThongTinSize.Rows[j][column].ToString());
                                            }
                                            break;
                                        }
                                    }
                                }

                                //if (lstSaveMau.Count > 0)
                                //{
                                //    AutoImportMau(lstSaveMau, searchLookUpEditMH.EditValue.ToString().Trim());
                                //}

                                tbl_ThongTinSize = ChangeTenMauToMaMau(tbl_ThongTinSize, _maHang);
                                foreach(DataRow row in tbl_ThongTinSize.Rows)
                                {
                                    if (row["DauSize"].ToString() == "")
                                        row["DauSize"] = 0;
                                }    
                                gridControl1.MainView = CreateBandSize(tbl_ThongTinSize);
                                BandedGridView mainView = (BandedGridView)gridControl1.MainView;
                                //mainView.CellValueChanging += mainView_CellValueChanging;
                                //mainView.CustomUnboundColumnData += mainView_CustomUnboundColumnData;
                                mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                                mainView.CustomDrawCell += MainView_CustomDrawCell;
                                gridControl1.DataSource = tbl_ThongTinSize;

                            }
                            else
                            {
                                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file excel và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file excel và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        // Đổi tên các cột khi import từ excel
        // Chứng từ PO -> PO
        // Màu PO -> MaMau
        // Mã nhóm size -> DauSize
        private DataTable RefactorColumnImport(DataTable tblImport, string columnNamePOExcel, string columnNamePO, string columnNameMauExcel, string columnNameMaMau, string columnNameDauSizeExcel, string columnNameDauSize)
        {
            // Xóa dòng *** ở dòng cuối cùng
            // *** nằm ở vị trí cột số 0
            for (int i=0;i<tblImport.Rows.Count;)
            {
                if (tblImport.Rows[i][0] != null&&tblImport.Rows[i][0].Equals("***"))
                {
                    DataRow dataRow = tblImport.Rows[i];
                    tblImport.Rows.Remove(dataRow);
                }
                else
                {
                    i++;
                }
            }
            for (int i = 0; i < tblImport.Columns.Count;)
            {
                DataColumn column = tblImport.Columns[i];
                string removeDiacrColumnName = RemoveDiacritics(column.ColumnName).Replace(" ", "").ToUpper();

                if (removeDiacrColumnName.Equals(columnNamePOExcel))
                {
                    column.ColumnName = columnNamePO;
                }
                else if (removeDiacrColumnName.Equals(columnNameMauExcel))
                {
                    column.ColumnName = columnNameMaMau;
                }
                else if (removeDiacrColumnName.Equals(columnNameDauSizeExcel))
                {
                    column.ColumnName = columnNameDauSize;
                }
                else if (removeDiacrColumnName.Contains("SIZE"))
                {
                    column.ColumnName = column.ColumnName.Replace(" ", "").Replace("Size", "Size@");
                }
                // Xóa cột số lượng khi import từ excel
                if (removeDiacrColumnName.Contains("SOLUONG"))
                {
                    tblImport.Columns.Remove(column);
                }
                else
                {
                    i++;
                }
            }
            // Thêm cột POID vào index 0
            // Cột GhiChu vào index 
            tblImport.Columns.Add("POID", typeof(string));
            tblImport.Columns["POID"].SetOrdinal(0);

            tblImport.Columns.Add("GhiChu", typeof(string));
            tblImport.Columns["GhiChu"].SetOrdinal(4);
            return tblImport;
        }

        private void mainView_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if (e.RowHandle >= 0 && (e.Column.FieldName == "MaQG" || e.Column.FieldName == "NgayGH" || e.Column.FieldName == "NgayDuKienVC" || e.Column.FieldName == "NgayThucTeVC" ||
                e.Column.FieldName == "NgayXuatHang"))
            {
                GridView gridView = sender as GridView;

                DataRow rowFocused = (gridView.GetFocusedRow() as DataRowView).Row;
                DataTable dataTable = (gridView.DataSource as DataView).Table as DataTable;
                if (rowFocused != null && rowFocused["PO"] != null && dataTable != null && dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if (i != e.RowHandle && dataTable.Rows[i]["PO"] != null && (rowFocused["PO"].Equals(dataTable.Rows[i]["PO"])))
                        {
                            gridView.SetRowCellValue(i, e.Column.FieldName, e.Value);
                        }
                    }
                }
            }
        }

        private void MainView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            //BandedGridView view = (BandedGridView)sender;

            //// Kiểm tra xem dòng này có được chọn không
            //if (view.IsRowSelected(e.RowHandle))
            //{
            //    // Thiết lập màu sắc cho dòng được chọn
            //    e.Appearance.BackColor = Color.FromArgb(255, 251, 209);
            //}
        }

        //void mainView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        //{
        //    BandedGridView view = (BandedGridView)sender;
        //    if (e.IsGetData)
        //    {
        //        int sum = 0;
        //        bool isEmpty = true;
        //        DataRow row = (e.Row as DataRowView).Row;
        //        DataTable dt = row.Table;
        //        foreach (BandedGridColumn col in view.Columns)
        //        {
        //            if (col.FieldName != "ID" && col.FieldName != "MaDH" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "ColorCode" && col.FieldName != "MaMau" && col.FieldName != "DauSizeID" && col.FieldName != "DauSize" && col.FieldName != "MaQG" && col.FieldName != "NgayGH" && col.FieldName != "NgayDuKienVC" && col.FieldName != "NgayThucTeVC" && col.FieldName != "NgayXuatHang" && col.FieldName != "GhiChu" && col.UnboundType == UnboundColumnType.Bound)
        //            {
        //                try
        //                {
        //                    if (!dt.Columns.Contains(col.FieldName))
        //                        continue;

        //                    if (row[col.FieldName] != null && !string.IsNullOrEmpty(row[col.FieldName].ToString()))
        //                    {
        //                        int value = Convert.ToInt32(row[col.FieldName]);
        //                        sum += value;
        //                    }
        //                    isEmpty = false;
        //                }
        //                catch (Exception ex)
        //                {
        //                    throw new Exception(ex.Message);
        //                }
        //            }
        //        }
        //        if (!isEmpty)
        //            e.Value = sum;
        //    }
        //}


        //private void mainView_CellValueChanging(object sender, CellValueChangedEventArgs e)
        //{
        //    if (e.RowHandle >= 0 && (e.Column.FieldName == "MaQG" || e.Column.FieldName == "NgayGH"))
        //    {
        //        GridView gridView = sender as GridView;

        //        DataRow rowFocused = (gridView.GetFocusedRow() as DataRowView).Row;
        //        DataTable dataTable = (gridView.DataSource as DataView).Table as DataTable;
        //        if (rowFocused != null && rowFocused["PO"] != null && dataTable != null && dataTable.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < dataTable.Rows.Count; i++)
        //            {
        //                if (dataTable.Rows[i]["PO"] != null && (rowFocused["PO"].Equals(dataTable.Rows[i]["PO"])))
        //                {
        //                    gridView.SetRowCellValue(i, e.Column.FieldName, e.Value);
        //                }
        //            }
        //        }
        //    }
        //}
        void mainView_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
            {
                sum = 0; // Đã có sẵn nhưng đảm bảo nó chạy
            }
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                sum = 0;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "ID" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "MaQG" && col.FieldName != "NgayGH" && col.FieldName != "GhiChu" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
            }

        }

        private BandedGridView CreateBandSize(DataTable tblDS)
        {
            BandedGridView bandedGridview = new BandedGridView();
            bandedGridview = bandedGridView1;

            gridBandColSize.Children.Clear();
            gridBandColSize.Columns.Clear();
            RemoveColumnSize();

            GridBand parentBand = bandedGridView1.Bands["gridBandColSize"];

            if (isEdit)
            {
                if (parentBand != null)
                {
                    parentBand.Children.Clear();
                    parentBand.Columns.Clear();
                    for (int i = 0; i < bandedGridView1.Columns.Count;)
                    {
                        if (bandedGridView1.Columns[i].FieldName.Contains("@Size@"))
                        {
                            bandedGridView1.Columns.RemoveAt(i);
                        }
                        else
                        {
                            i += 1;
                        }
                    }
                }
                int demColIndex = -1;
                foreach (DataColumn dc in tblDS.Columns)
                {
                    demColIndex++;
                    if (demColIndex > 12 && demColIndex < tblDS.Columns.Count)
                    {
                        if (tblDS.Columns[demColIndex].ColumnName == "CheckColumn" || tblDS.Columns[demColIndex].ColumnName == "IsNew")
                        {
                            continue;
                        }
                        string[] arrName = dc.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        string colName = arrName[0];
                        String colName1 = arrName[2];
                        BandedGridColumn col = new BandedGridColumn();
                        col.AppearanceCell.Options.UseTextOptions = true;
                        col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        col.AppearanceHeader.Options.UseTextOptions = true;
                        col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        col.Caption = colName;
                        col.FieldName = dc.ColumnName;
                        col.Name = "col" + colName;
                        col.OptionsColumn.AllowEdit = true;
                        col.Visible = true;
                        col.Width = 65;
                        col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                        col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        col.DisplayFormat.FormatString = "{0:##,0}";
                        col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                        bandedGridView1.Columns.AddRange(new BandedGridColumn[] { col });
                        GridBand gb = new GridBand();
                        gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                        gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                        gb.AppearanceHeader.Options.UseTextOptions = true;
                        gb.AppearanceHeader.Options.UseForeColor = true;
                        gb.Caption = colName;
                        gb.Columns.Add(col);
                        gb.Name = "gbColBandSize" + col;
                        gb.VisibleIndex = 0;
                        gb.Width = 65;

                        gridBandColSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                    }
                }
            }
            else 
            {
                try
                {
                    if (parentBand != null)
                    {
                        parentBand.Children.Clear();
                        parentBand.Columns.Clear();
                        for (int i = 0; i < bandedGridView1.Columns.Count;)
                        {
                            if (bandedGridView1.Columns[i].FieldName.Contains("@Size@"))
                            {
                                bandedGridView1.Columns.RemoveAt(i);
                            }
                            else
                            {
                                i += 1;
                            }
                        }
                    }
                    int demColIndex = -1;
                    foreach (DataColumn dc in tblDS.Columns)
                    {
                        demColIndex++;
                        if (demColIndex > 10 && demColIndex < tblDS.Columns.Count)
                        {
                            string[] arrName = dc.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                            string colName = arrName[2];
                            String colName1 = arrName[2];
                            BandedGridColumn col = new BandedGridColumn();
                            col.AppearanceCell.Options.UseTextOptions = true;
                            col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            col.AppearanceHeader.Options.UseTextOptions = true;
                            col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            col.Caption = colName;
                            col.FieldName = dc.ColumnName;
                            col.Name = "col" + colName;
                            col.OptionsColumn.AllowEdit = true;
                            col.Visible = true;
                            col.Width = 65;
                            col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                            col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                            col.DisplayFormat.FormatString = "{0:##,0}";
                            col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                            bandedGridView1.Columns.AddRange(new BandedGridColumn[] { col });
                            GridBand gb = new GridBand();
                            gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                            gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                            gb.AppearanceHeader.Options.UseTextOptions = true;
                            gb.AppearanceHeader.Options.UseForeColor = true;
                            gb.Caption = colName;
                            gb.Columns.Add(col);
                            gb.Name = "gbColBandSize" + col;
                            gb.VisibleIndex = 0;
                            gb.Width = 65;

                            gridBandColSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            return bandedGridview;
        }

        static string ProcessString(string text)
        {
            string trimmedString = text.Trim();
            string uppercaseString = trimmedString.ToUpper();
            string stringWithoutSpaces = uppercaseString.Replace(" ", "");
            return stringWithoutSpaces;
        }

        private void TextEditDot_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            dot = changingEventArgs.NewValue.ToString();
            //CheckBtnLuu();
        }

        private void BtThem()
        {
            if (isEdit) 
            {
                 if (GlobleData.UserName.ToString().ToUpper() == txtNguoiTao.Text.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
                  {
                    DataTable tblthem = gridControl1.DataSource as DataTable;
                    if (gridControl1.DataSource == null)
                    {
                        return;
                    }
                    if (tblthem.Rows.Count > 0)
                    {
                        if (searchLookUpEditKH.EditValue == null || searchLookUpEditKH.EditValue == "")
                        {
                            MessageBox.Show("Vui lòng chọn Khách Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            return;
                        }
                        if (searchLookUpEditMH.EditValue == null || searchLookUpEditMH.EditValue == "")
                        {
                            MessageBox.Show("Vui lòng chọn Mã Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            return;
                        }
                        if (searchLookUpEditCL.EditValue == null || searchLookUpEditCL.EditValue == "")
                        {
                            MessageBox.Show("Vui lòng chọn Chủng Loại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            return;
                        }
                        if (textDot.Text == "" || textDot.Text == null)
                        {
                            MessageBox.Show("Vui lòng nhập Đợt.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            return;
                        }
                        if (txtSoBooking.Text == null || txtSoBooking.Text == "")
                        {
                            MessageBox.Show("Vui lòng nhập tần suất.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            return;
                        }
                        if (!tblthem.Columns.Contains("IsNew"))
                        {
                            tblthem.Columns.Add("IsNew", typeof(bool));
                        }
                        CreateSearchLookupMaMau();
                        //this.Them.Enabled = false;
                        //Console.WriteLine("Them_ItemClick");
                        _status = ResourceURL.EventStatus.Add;
                        DataTable tbl = gridControl1.DataSource as DataTable;
                        DataRow newDataRow = tbl.Rows.Add();

                        newDataRow["IsNew"] = true;
                        _rowAdd = tbl.Rows.Count - 1;
                        //tbl.Rows.Add();
                        //gridControl1.DataSource = tbl;
                        if (isEdit)
                        {
                            object maDh = tblthem.Rows[0]["MaDH"];

                            newDataRow["MaDH"] = maDh;
                            newDataRow["PO"] = "KIEMKE";
                        }
                    }
                }
                else
                {
                    XtraMessageBox.Show("User này không phải là người tạo, không có quyền sửa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                DataTable tblthem = gridControl1.DataSource as DataTable;
                if (gridControl1.DataSource == null)
                {
                    return;
                }
                if (tblthem.Rows.Count > 0)
                {
                    if (searchLookUpEditKH.EditValue == null || searchLookUpEditKH.EditValue == "")
                    {
                        MessageBox.Show("Vui lòng chọn Khách Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    if (searchLookUpEditMH.EditValue == null || searchLookUpEditMH.EditValue == "")
                    {
                        MessageBox.Show("Vui lòng chọn Mã Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    if (searchLookUpEditCL.EditValue == null || searchLookUpEditCL.EditValue == "")
                    {
                        MessageBox.Show("Vui lòng chọn Chủng Loại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    if (textDot.Text == "" || textDot.Text == null)
                    {
                        MessageBox.Show("Vui lòng nhập Đợt.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    if (txtSoBooking.Text == null || txtSoBooking.Text == "")
                    {
                        MessageBox.Show("Vui lòng nhập tần suất.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
                    if (!tblthem.Columns.Contains("IsNew"))
                    {
                        tblthem.Columns.Add("IsNew", typeof(bool));
                    }
                    CreateSearchLookupMaMau();
                    //this.Them.Enabled = false;
                    //Console.WriteLine("Them_ItemClick");
                    _status = ResourceURL.EventStatus.Add;
                    DataTable tbl = gridControl1.DataSource as DataTable;
                    DataRow newDataRow = tbl.Rows.Add();

                    newDataRow["IsNew"] = true;
                    _rowAdd = tbl.Rows.Count - 1;
                    //tbl.Rows.Add();
                    //gridControl1.DataSource = tbl;
                    if (isEdit)
                    {
                        object maDh = tblthem.Rows[0]["MaDH"];

                        newDataRow["MaDH"] = maDh;
                        newDataRow["PO"] = "KIEMKE";
                    }
                }
            }


        }

        private void Them_ItemClick(object sender, ItemClickEventArgs e)
        {
            BtThem();
        }

        private void bandedGridView1_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void bandedGridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }
        private void BtNapLai()
        {
            InitForm(tblChiTietold, tblMau, tbl);
        }
        private void Naplai_ItemClick(object sender, ItemClickEventArgs e)
        {
            BtNapLai();
        }

        private void txtBoxPO_KeyPress(object sender, KeyPressEventArgs e)
        {
            int ascii = Convert.ToInt32(e.KeyChar);
            if (!(ascii == 58 || ascii == 32))
            {
                if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true; // Chặn ký tự nhập vào
                }
            }

            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtBoxMau_KeyPress(object sender, KeyPressEventArgs e)
        {
            int ascii = Convert.ToInt32(e.KeyChar);
            if (!(ascii == 58))
            {
                if (char.IsWhiteSpace(e.KeyChar) || !char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true; // Chặn ký tự nhập vào
                }
            }

            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtBoxSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            int ascii = Convert.ToInt32(e.KeyChar);
            if (!(ascii == 58))
            {
                if (char.IsWhiteSpace(e.KeyChar) || !char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true; // Chặn ký tự nhập vào
                }
            }

            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtBoxDauSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            int ascii = Convert.ToInt32(e.KeyChar);
            if (!(ascii == 58))
            {
                if (char.IsWhiteSpace(e.KeyChar) || !char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true; // Chặn ký tự nhập vào
                }
            }

            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtSoVoice_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtGhiChu_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtMaHS_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void bandedGridView1_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            GridView view = (GridView)sender;
            if (view.FocusedColumn.FieldName.Contains("@"))
            {
                if (Convert.ToInt32(e.Value) < 0)
                {
                    e.Valid = false;
                    e.ErrorText = "Số lượng phải lớn hơn 0";
                }
            }
        }

        private void searchLookUpEditKH_EditValueChanged_1(object sender, EventArgs e)
        {
            string urlHH = string.Format("{0}?makh={1}", URL + "DonHangTong/GetHangHoaKH", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            DataTable tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
            searchLookUpEditMH.Properties.DataSource = tblHH;
            searchLookUpEditMH.Properties.ValueMember = "MaHang";
            searchLookUpEditMH.Properties.DisplayMember = "TenHang";
        }

        private void SearchLookupeditCL_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            maCL = changingEventArgs.NewValue.ToString();
            //CheckBtnLuu();
        }

        private void checkEditImport_CheckedChanged(object sender, EventArgs e)
        {
            CheckEdit checkEdit = sender as CheckEdit;
            //_dtSizeNhom = CreateDatatableSizeNhom();
            _dtTable = CreateDatatable();
            gridBandColSize.Children.Clear();
            if (checkEdit != null)
            {
                //gridCheckMarksColor.Selection.Clear();
                //gridCheckMarksDauSize.Selection.Clear();
                //gridCheckMarksSize.Selection.Clear();
                if (checkEdit.Checked)
                {

                    //gridCheckMarksColor.Selection.Clear();
                    //gridCheckMarksDauSize.Selection.Clear();
                    //gridCheckMarksSize.Selection.Clear();
                    barButtonItem1.Enabled = true;
                    btNhap.Enabled = false;
                    txtBoxPO.Enabled = false;
                    txtBoxSize.Enabled = false;
                    txtBoxDauSize.Enabled = false;
                    txtBoxMau.Enabled = false;
                    labelMoTa.Text = "";
                    txtBoxPO.Text = "";
                    txtBoxSize.Text = "";
                    txtBoxMau.Text = "";
                    txtBoxDauSize.Text = "";
                    //string[] po = { "[p1:p2]" };
                    //txtBoxPO.Lines = po;
                    //string[] size = { "[s1:s2]" };
                    //txtBoxSize.Lines = size;
                    //string[] dausize = { "[d1:d2]" };
                    //txtBoxDauSize.Lines = dausize;
                    //string[] mau = { "[m1:m2]" };
                    //txtBoxMau.Lines = mau;
                    System.Data.DataTable dtbNull = new DataTable();
                    //gridControlThongTinDonHang.MainView = GetBandGridViewAmount(dtbNull);
                    //gridControlThongTinDonHang.DataSource = dtbNull;

                }
                else
                {
                    //gridBandColSize.Children.Clear();
                    gridCheckMarksColor.Selection.Clear();
                    gridCheckMarksDauSize.Selection.Clear();
                    gridCheckMarksSize.Selection.Clear();
                    barButtonItem1.Enabled = false;
                    btNhap.Enabled = true;
                    txtBoxPO.Enabled = true;
                    txtBoxSize.Enabled = true;
                    txtBoxDauSize.Enabled = true;
                    txtBoxMau.Enabled = true;
                    labelMoTa.Text = "Vui lòng nhập PO, Màu, Nhóm Size, Size và Quốc gia. Mỗi đối tượng phải cách nhau bằng dấu : ";
                    System.Data.DataTable dtbNull = new DataTable();
                    //gridControlThongTinDonHang.MainView = GetBandGridViewAmount(dtbNull);
                    //gridControlThongTinDonHang.DataSource = dtbNull;
                    gridControl1.DataSource = _dtTable;
                    //gridControl1.DataSource = null;
                    //gridControl1.Refresh();
                    //bandedGridView1.RefreshData();
                    //bandedGridView1.RefreshData();
                    gridControl1.RefreshDataSource();
                    gridControl1.Refresh();
                    CreateSearchLookupQuocGia();
                    CreateSearchLookupMaMau();
                    CreateSearchLookupDauSize();
                }
            }
            CreateSearchLookupQG();
            InitSearInitSearchLookupMaMau();
            InitSearchLookupDauSize();
        }

        private void gridViewThongTinTonKho_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            DataTable checkszie = UnPivot(gridControl1.DataSource as DataTable);
            if (isEdit)
            {
                lstRowUpdate.Add(e.RowHandle);
            }
            //if (e.Column.FieldName == "DauSize")
            //{
            //    if (isEdit)
            //    {
            //        foreach (DataRow dr in checkszie.Rows)
            //        {
            //            dr["DauSize"] = e.Value?.ToString();
            //        }
            //        string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
            //        string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
            //        DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
            //        if (dtSize == null && dtSize.Rows.Count < 0)
            //        {
            //            MessageBox.Show("Size thuộc mã hàng  " + searchLookUpEditMH.EditValue.ToString().ToString() + " chưa được khai báo trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
            //            return;
            //        }
            //        var missingRows = from row1 in checkszie.AsEnumerable()
            //                          where !dtSize.AsEnumerable().Any(row2 =>
            //                      row1["MaHang"].ToString() == row2["MaHang"].ToString() &&
            //                      row1["Size"].ToString() == row2["SizeSanXuat"].ToString() &&
            //                      row1["DauSize"].ToString() == row2["NhomSize"].ToString())
            //                          select row1;
            //        if (missingRows.Any())
            //        {
            //            // Nhóm các dòng bị thiếu theo NhomSize
            //            var groupedMissing = missingRows
            //                .GroupBy(row => row["DauSize"].ToString())
            //                .Select(group => new
            //                {
            //                    NhomSize = group.Key, // Tên nhóm size
            //            Sizes = group.Select(row => row["Size"].ToString()).Distinct() // Các size bị thiếu
            //        });

            //            // Tạo chuỗi thông báo
            //            var message = string.Join("\n", groupedMissing.Select(group =>
            //                $"Nhóm {group.NhomSize} : thiếu các size:  {string.Join(", ", group.Sizes)}  thuộc mã hàng {searchLookUpEditMH.EditValue.ToString()}"));

            //            // Hiển thị thông báo
            //            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //            return;
            //        }
            //    }
            //    else 
            //    {
            //        foreach (DataRow dr in _dtSizeNhom.Rows)
            //        {
            //            dr["NhomSize"] = e.Value?.ToString();
            //        }
            //        string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
            //        string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
            //        DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
            //        if (dtSize == null && dtSize.Rows.Count < 0)
            //        {
            //            MessageBox.Show("Size thuộc mã hàng  " + searchLookUpEditMH.EditValue.ToString().ToString() + " chưa được khai báo trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
            //            return;
            //        }
            //        var missingRows = from row1 in _dtSizeNhom.AsEnumerable()
            //                          where !dtSize.AsEnumerable().Any(row2 =>
            //                              row1["MaHang"].ToString() == row2["MaHang"].ToString() &&
            //                              row1["SizeSanXuat"].ToString() == row2["SizeSanXuat"].ToString() &&
            //                              row1["NhomSize"].ToString() == row2["NhomSize"].ToString())
            //                          select row1;
            //        if (missingRows.Any())
            //        {
            //            // Nhóm các dòng bị thiếu theo NhomSize
            //            var groupedMissing = missingRows
            //                .GroupBy(row => row["NhomSize"].ToString())
            //                .Select(group => new
            //                {
            //                    NhomSize = group.Key, // Tên nhóm size
            //                Sizes = group.Select(row => row["SizeSanXuat"].ToString()).Distinct() // Các size bị thiếu
            //            });

            //            // Tạo chuỗi thông báo
            //            var message = string.Join("\n", groupedMissing.Select(group =>
            //                $"Nhóm {group.NhomSize} : thiếu các size:  {string.Join(", ", group.Sizes)}  thuộc mã hàng {searchLookUpEditMH.EditValue.ToString()}"));

            //            // Hiển thị thông báo
            //            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //            return;

            //        }
            //    }

            //}

        }
        private DataTable RemoveColumn(DataTable tblRemoveColumn)
        {
            // Xóa Cột POID và cột DauSizeID khỏi DataTable
            for (int i = 0; i < tblRemoveColumn.Columns.Count;)
            {
                DataColumn dataColumn = tblRemoveColumn.Columns[i];
                if (dataColumn.ColumnName.Equals("POID") || dataColumn.ColumnName.Equals("DauSizeID"))
                {
                    tblRemoveColumn.Columns.Remove(dataColumn);
                }
                else
                {
                    i += 1;
                }
            }
            return tblRemoveColumn;
        }

        private void RemoveColumnSize()
        {
            for (int i = 0; i < bandedGridView1.Columns.Count;)
            {
                if (bandedGridView1.Columns[i].FieldName.Contains("@SIZE@"))
                {
                    bandedGridView1.Columns.RemoveAt(i);
                }
                else
                {
                    i += 1;
                }
            }

        }
        //private DataTable AddColumnSize(DataTable _tbl, List<string> _lstSize)
        //{
        //    foreach (string size in _lstSize)
        //    {
        //        _tbl.Columns.Add(string.Format("Size@{0}", size), typeof(double));
        //    }
        //    return _tbl;
        //}

        private void CreateDefaultSearchLookUpHH()
        {

            //Mau
            txtBoxMau.Properties.ValueMember = "MaMau";
            txtBoxMau.Properties.DisplayMember = "TenMau";
            txtBoxMau.Properties.ShowClearButton = false;
            txtBoxMau.Properties.Appearance.ForeColor = Color.Red;
            txtBoxMau.Properties.NullText = "[Chọn Màu]";
            txtBoxMau.Properties.View.OptionsSelection.MultiSelect = true;
            txtBoxMau.Properties.PopulateViewColumns();
            gridCheckMarksColor = new SearchCheckSelection(txtBoxMau.Properties);
            gridCheckMarksColor.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEdit_Mau_SelectionChanged);
            txtBoxMau.Properties.Tag = gridCheckMarksColor;

            //Size
            txtBoxSize.Properties.ValueMember = "SizeSanXuat";
            txtBoxSize.Properties.DisplayMember = "SizeSanXuat";
            txtBoxSize.Properties.ShowClearButton = false;
            txtBoxSize.Properties.Appearance.ForeColor = Color.Red;
            txtBoxSize.Properties.View.OptionsSelection.MultiSelect = true;
            txtBoxSize.Properties.NullText = "[Chọn Size]";
            txtBoxSize.Properties.PopulateViewColumns();
            gridCheckMarksSize = new SearchCheckSelection(txtBoxSize.Properties);
            gridCheckMarksSize.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEditSize_SelectionChanged);
            txtBoxSize.Properties.Tag = gridCheckMarksSize;
            // Đăng ký sự kiện Popup để sắp xếp ngược lại
            txtBoxSize.Properties.Popup += new EventHandler(txtBoxSize_Popup);

            //DauSize
            txtBoxDauSize.Properties.ValueMember = "DauSizeID";
            txtBoxDauSize.Properties.DisplayMember = "DauSize";
            txtBoxDauSize.Properties.ShowClearButton = false;
            txtBoxDauSize.Properties.Appearance.ForeColor = Color.Red;
            txtBoxDauSize.Properties.View.OptionsSelection.MultiSelect = true;
            txtBoxDauSize.Properties.NullText = "[Chọn nhóm size]";
            txtBoxDauSize.Properties.PopulateViewColumns();
            gridCheckMarksDauSize = new SearchCheckSelection(txtBoxDauSize.Properties);
            gridCheckMarksDauSize.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEditDauSize_SelectionChanged);
            txtBoxDauSize.Properties.Tag = gridCheckMarksDauSize;


            InitSearInitSearchLookupMaMau();
            ////InitSearchLookupSize();
            InitSearchLookupDauSize();
        }

        private void txtBoxSize_Popup(object sender, EventArgs e)
        {
            // Lấy SearchLookUpEdit
            SearchLookUpEdit searchLookUpEdit = sender as SearchLookUpEdit;
            if (searchLookUpEdit != null)
            {
                // Lấy GridView từ SearchLookUpEdit
                GridView gridView = searchLookUpEdit.Properties.View as GridView;

                if (gridView != null)
                {
                    // Đảm bảo cột "SizeSanXuat" tồn tại
                    if (gridView.Columns["SizeSanXuat"] != null)
                    {
                        // Xóa các sắp xếp hiện tại
                        //gridView.SortInfo.Clear();

                        // Thêm sắp xếp ngược lại theo cột "SizeSanXuat"
                        //gridView.SortInfo.Add(new GridColumnSortInfo(gridView.Columns["SizeSanXuat"], ColumnSortOrder.Descending));

                        // Áp dụng sắp xếp
                        gridView.RefreshData();
                    }
                }
            }
        }

        private void searchLookUpEditSize_SelectionChanged(object sender, EventArgs e)
        {

            Control c = this.txtBoxSize;

            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(":"); }
                    sb.Append(rv["SizeSanXuat"].ToString());
                }

                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();
                size = sb.ToString();
            }
        }

        private void searchLookUpEdit_Mau_SelectionChanged(object sender, EventArgs e)
        {
            Control c = this.txtBoxMau;
            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(":"); }
                    sb.Append(rv["MaMau"].ToString());
                }

                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();
                mau = sb.ToString();
            }

        }

        private void txtBoxMau_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            
        }

        private void txtBoxSize_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {

        }

        private void txtBoxDauSize_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {

        }

        private void txtBoxMau_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            foreach (DataRowView rv in gridCheckMark.Selection)
            {

                if (sb.ToString().Length > 0) { sb.Append(":"); }
                sb.Append(rv["TenMau"].ToString());
            }
            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chọn Màu----";
            }
            else
            {
                e.DisplayText = sb.ToString();
            }
        }

        private void txtBoxSize_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            foreach (DataRowView rv in gridCheckMark.Selection)
            {

                if (sb.ToString().Length > 0) { sb.Append(":"); }
                sb.Append(rv["SizeSanXuat"].ToString());
            }
            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chọn Size----";
            }
            else
            {
                e.DisplayText = sb.ToString();
            }
        }

        private void txtBoxDauSize_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            if (checkEditTV.Checked == false)
            {
                foreach (DataRowView rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(":"); }
                    sb.Append(rv["MaInSeam"].ToString());
                }
            }
            else
            {
                foreach (DataRowView rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(":"); }
                    sb.Append(rv["DauSize"].ToString());
                }
            }


            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chọn InSeam----";
            }
            else
            {
                e.DisplayText = sb.ToString();
            }
        }

        private DataTable CreateDatatableSizeNhom()
        {
            DataTable tbl = new DataTable("dtSizeNhom");
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("SizeSanXuat", typeof(string));
            tbl.Columns.Add("MaNhomSize", typeof(string));
            tbl.Columns.Add("NhomSize", typeof(string));
            return tbl;
        }

        static bool KiemTraChuoiLaChu(string chuoi)
        {
            if (string.IsNullOrEmpty(chuoi))
            {
                return false; // Chuỗi rỗng không được coi là chứa toàn ký tự chữ
            }

            foreach (char kyTu in chuoi)
            {
                if (!char.IsLetter(kyTu))
                {
                    return false; // Nếu có ít nhất một ký tự không phải là chữ, trả về false
                }
            }

            return true; // Nếu không có ký tự nào không phải là chữ, trả về true
        }
        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                check = 1;
                string filePath = string.Empty;
                string sheetIndex = string.Empty;
                bool resultFrm = false;
                // btnXacNhan.BackColor = Color.Navy;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Select an Excel File";
                frmErpImport_DHT frm = new frmErpImport_DHT();
                frm.ShowDialog();
                filePath = frm.resultFilePath;
                sheetIndex = frm.resultIndex;
                resultFrm = frm.result;
                if (resultFrm == false) return;
                string pathExecel = filePath;
                string conString = "", sheet1 = string.Empty;
                int totalAmount = 0, Amount = 0;
                using (OleDbConnection connection = new OleDbConnection(conString))
                {
                    var source = new ExcelDataSource();
                    source.FileName = pathExecel;
                    string worksheetName = "";
                    using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(pathExecel))
                    {
                        DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                        worksheetName = worksheetCollection[sheetIndex].Name;
                    }
                    var worksheetSettingss = new ExcelWorksheetSettings(worksheetName, "$A1:D9");
                    source.SourceOptions = new ExcelSourceOptions(worksheetSettingss);
                    source.Fill();
                    DataTable tbl_Styles = new DataTable();
                    tbl_Styles = source.ToDataTable();

                    string pp = string.Empty;
                    string style = tbl_Styles.Rows[1][2].ToString().TrimStart().TrimEnd();
                    var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A11:BZ500");
                    source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                    source.Fill();
                    DataTable tbColorPO = new DataTable();
                    tbColorPO = source.ToDataTable();
                    _dtTable = CreateDatatable();
                    _dtSizeNhom = CreateDatatableSizeNhom();
                    string styleID = "";
                    string cusName = "";
                    string chungloai = "";
                    Dictionary<string, string> mauChuaCo = new Dictionary<string, string>();
                    if (style.Length > 100)
                    {
                        XtraMessageBox.Show("Mã hàng không được dài quá 100 kí tự!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string _styleID = style.ToString().TrimStart().TrimEnd();
                    if (string.IsNullOrEmpty(_styleID))
                    {
                        XtraMessageBox.Show("Mã hàng không được bỏ trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string urlmhang = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
                    string jsonmhang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlmhang); }).Result;
                    DataTable _dtMaHang = JsonConvert.DeserializeObject<DataTable>(jsonmhang);
                    if (_dtMaHang == null)
                    {
                        XtraMessageBox.Show("Mã hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var rowmh = _dtMaHang.AsEnumerable().FirstOrDefault(r => r["TenHang"].ToString().Trim() == _styleID.ToString().Trim());
                    if (rowmh != null)
                    {
                        styleID = rowmh["MaHang"].ToString();
                    }
                    else
                    {
                        XtraMessageBox.Show("Mã hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // kiểm tra khách hàng
                    string _cusName = tbl_Styles.Rows[0][2].ToString().TrimStart().TrimEnd();
                    if (string.IsNullOrEmpty(_cusName))
                    {
                        XtraMessageBox.Show("Không được bỏ trống khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string urlKH = string.Format("{0}?", URL + "DonHangTong/GetKhachHang");
                    string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
                    DataTable _dtKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
                    if (_dtKH == null)
                    {
                        XtraMessageBox.Show("Khách hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var rowkh = _dtKH.AsEnumerable().FirstOrDefault(r => r["TenKH"].ToString().Trim() == _cusName.ToString().Trim());
                    if (rowkh != null)
                    {
                        cusName = rowkh["MaKH"].ToString();
                    }
                    else
                    {
                        XtraMessageBox.Show("Khách hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    //if (string.IsNullOrEmpty(tbl_Styles.Rows[4][2].ToString().TrimStart().TrimEnd()))
                    //{
                    //    XtraMessageBox.Show("Không được bỏ trống chủng loại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return;
                    //}
                    string urlCL = string.Format("{0}?chungloai={1}", URL + "DonHangTong/GetChungLoai1", tbl_Styles.Rows[4][2].ToString().TrimStart().TrimEnd());
                    string jsonCL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCL); }).Result;
                    DataTable _dtCL = JsonConvert.DeserializeObject<DataTable>(jsonCL);
                    if (_dtCL != null && _dtCL.Rows.Count > 0)
                    {
                        var rowcl = _dtCL.AsEnumerable().FirstOrDefault(r => r["TenCL"].ToString().Trim().ToUpper() == tbl_Styles.Rows[4][2].ToString().TrimStart().TrimEnd().ToUpper());
                        if (rowcl != null)
                        {
                            chungloai = rowcl["MaCL"].ToString();
                        }

                    }
                    string jsonSize = string.Empty, jsonMau = string.Empty;

                    if (checkEditTV.Checked == false)
                    {
                        //size
                        string urlSize = string.Format("{0}", URL + $"ERPBangSize/Get?action=Getizev1");
                        jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;

                        //Mau
                        string urlMau = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                        jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                    }
                    else
                    {
                        //size
                        string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", styleID.ToString(), cusName.ToString());
                        jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;

                        //Mau
                        string urlMau = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", styleID.ToString(), cusName.ToString());
                        jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                    }

                    DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);

                    DataTable dtMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);

                    int Grand = 0;
                    for (int j = 1; j < tbColorPO.Rows.Count; j++)
                    {
                        if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("TOTAL"))
                        {
                            if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                                break;
                            continue;
                        }
                        else
                        {
                            for (int c = 6; c < tbColorPO.Columns.Count - 1; c++)
                            {
                                if (tbColorPO.Columns[c].ToString().Trim().ToUpper().Contains("GRAND"))
                                    break;

                                if (KiemTraChuoiLaChu(tbColorPO.Rows[j][c].ToString().Trim()))
                                {
                                    MessageBox.Show("Số lượng size không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                    return;
                                }
                                else
                                {
                                    if (tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "") != "")
                                        totalAmount = totalAmount + (tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "") == "" ? 0 : Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", ""), "[^0-9a-zA-Z]+", "")));//row[col].ToString().Replace());
                                }

                            }
                        }
                    }
                    string urlktdonhang = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetDonHangTongMaHang", styleID);
                    string jsonktdonhang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlktdonhang); }).Result;
                    List<DonHangTongEntity> dohanglist = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(jsonktdonhang);
                    bool isMessageShow = false;
                    foreach (DonHangTongEntity donhang in dohanglist)
                    {

                        if (donhang.MaKH == cusName.ToString() && donhang.MaHang == styleID.ToString() && RemoveVietnameseTone(donhang.Dot).Trim().ToUpper().ToString().Replace(" ", "") == RemoveVietnameseTone(tbl_Styles.Rows[1][2].ToString().TrimStart().TrimEnd()).Trim().ToUpper().ToString().Replace(" ", "")
                           && donhang.SoLuong == totalAmount)
                        {
                            if (!isMessageShow)
                            {
                                DialogResult messResult = MessageBox.Show("Đã có đơn hàng cho mã hàng và khách hàng này. Vui lòng kiểm tra lại. Nếu muốn tiếp tục hãy đổi Đợt!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (messResult == DialogResult.Yes)
                                {
                                    return;
                                }
                            }
                            isMessageShow = true;
                        }
                    }

                    string sizetrung = "", _inseam = "";
                    for (int j = 1; j < tbColorPO.Columns.Count - 1; j++)
                    {
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][2].ToString()))
                        {
                            //_inseam = tbColorPO.Rows[j][3].ToString().Trim();
                            _inseam = Regex.Replace(tbColorPO.Rows[j][3].ToString().Trim().ToUpper(), @"[^\w\d]+", "_");
                        }
                        //if (_inseam == "" && !_inseam.ToUpper().Contains("GRAND"))
                        //{
                        //    MessageBox.Show("InSeam không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                        //    return;
                        //}
                        if (_inseam == "" && !_inseam.ToUpper().Contains("GRAND"))
                        {
                            _inseam = "0";
                        }
                        if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("TOTAL"))
                        {
                            if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                                break;
                        }
                        HashSet<string> sizeSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        for (int i = 6; i < tbColorPO.Columns.Count - 1; i++)
                        {

                            if (!string.IsNullOrEmpty(tbColorPO.Columns[i].ColumnName.ToString()))
                            {
                                pp = tbColorPO.Columns[i].ColumnName;
                            }

                            if (pp.Trim().ToUpper().Contains("TOTAL"))
                            {
                                if (pp.Trim().ToUpper().Contains("GRAND"))
                                    break;
                                continue;
                            }

                            string sizeID = tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper() + "@Size@" + tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper();
                            string rawSize = tbColorPO.Rows[0][i].ToString().Trim().Replace(" ", "").ToUpper();
                            if (sizeID.ToString() == "")
                            {
                                XtraMessageBox.Show("Size không được bỏ trống. Vui lòng kiểm tra lai.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            if (sizeSet.Contains(rawSize))
                            {
                                XtraMessageBox.Show($"File Excel có 2 cột size giống nhau: '{rawSize}'. Vui lòng kiểm tra lại!",
                                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            else
                            {
                                sizeSet.Add(rawSize);
                            }

                            var rowsize = _dtSizeNhom.AsEnumerable().FirstOrDefault(r => r["SizeSanXuat"].ToString().Trim() == tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper()
                            && r["MaNhomSize"].ToString().Trim() == _inseam.ToString().Trim());
                            if (rowsize != null) break;
                            DataRow _dr = _dtSizeNhom.NewRow();
                            _dr["MaHang"] = styleID;
                            _dr["SizeSanXuat"] = tbColorPO.Rows[0][i].ToString().Trim().ToUpper();
                            _dr["MaNhomSize"] = _inseam.ToString().Replace(" ", "").Trim().ToUpper();
                            _dr["NhomSize"] = string.IsNullOrEmpty(tbColorPO.Rows[j][3].ToString()) ? "0" : tbColorPO.Rows[j][3].ToString().Trim().ToUpper();
                            _dtSizeNhom.Rows.Add(_dr);
                            sizetrung = sizeID;

                        }
                    }  // Đối chiếu dữ liệu và kiểm tra
                    if (checkEditTV.Checked == false)
                    {
                        string urlDSTheInSeamct = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeamv1");
                        string jsonDSTheInSeamct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSTheInSeamct); }).Result;
                        DataTable theinseam = JsonConvert.DeserializeObject<DataTable>(jsonDSTheInSeamct);

                        var missingSizeSanXuat = _dtSizeNhom.AsEnumerable()
                            .Where(row1 =>
                                !dtSize.AsEnumerable().Any(row2 =>
                                    row1["SizeSanXuat"].ToString().Trim()
                                        .Equals(row2["SizeSanXuat"].ToString().Trim(),
                                                StringComparison.OrdinalIgnoreCase)))
                            .Select(row => row["SizeSanXuat"].ToString())
                            .Distinct()
                            .ToList();
                        if (missingSizeSanXuat.Any())
                        {
                            string msgSize = "Các Size chưa khai báo trong Thẻ Size:\n" +
                                             string.Join(", ", missingSizeSanXuat);

                            MessageBox.Show(msgSize, "Thiếu Size", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var missingNhomSize = _dtSizeNhom.AsEnumerable()
                            .Where(row1 =>
                                !theinseam.AsEnumerable().Any(row2 =>
                                    row1["NhomSize"].ToString().Trim()
                                        .Equals((row2["InSeam"]?.ToString() ?? "").Trim(),
                                                StringComparison.OrdinalIgnoreCase)))
                            .Select(row => row["NhomSize"].ToString())
                            .Distinct()
                            .ToList();
                        if (missingNhomSize.Any())
                        {
                            string msgInSeam = "Các InSeam chưa khai báo trong Thẻ InSeam:\n" +
                                               string.Join(", ", missingNhomSize);

                            MessageBox.Show(msgInSeam, "Thiếu InSeam", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        // Đối chiếu dữ liệu và kiểm tra
                        var missingRows = from row1 in _dtSizeNhom.AsEnumerable()
                                          where !dtSize.AsEnumerable().Any(row2 =>
                                              row1["MaHang"].ToString() == row2["MaHang"].ToString() &&
                                              row1["SizeSanXuat"].ToString() == row2["SizeSanXuat"].ToString() &&
                                              row1["NhomSize"].ToString() == row2["NhomSize"].ToString())
                                          select row1;
                        //if (missingRows.Any())
                        //{
                        //    string sizesanxuatMissing = string.Join("; ", missingRows.Select(r => r["SizeSanXuat"].ToString()));
                        //    string nhomsizeMissing = string.Join("; ", missingRows.Select(r => r["NhomSize"].ToString()).Distinct().ToArray());
                        //    MessageBox.Show("Size " + sizesanxuatMissing + " không có trong nhóm  " + nhomsizeMissing + " thuộc mã hàng  " + styleID.ToString() + " trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                        //    return;
                        //}
                        if (missingRows.Any())
                        {
                            // Nhóm các dòng bị thiếu theo NhomSize
                            var groupedMissing = missingRows
                                .GroupBy(row => row["NhomSize"].ToString())
                                .Select(group => new
                                {
                                    NhomSize = group.Key, // Tên nhóm size
                                        Sizes = group.Select(row => row["SizeSanXuat"].ToString()).Distinct() // Các size bị thiếu
                                    });

                            // Tạo chuỗi thông báo
                            var message = string.Join("\n", groupedMissing.Select(group =>
                                $"InSeam {group.NhomSize} : thiếu các size:  {string.Join(", ", group.Sizes)}  thuộc mã hàng {styleID.ToString()}"));

                            // Hiển thị thông báo
                            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    var uniqueValues = _dtSizeNhom.AsEnumerable().Select(x => new { MaHang = x["MaHang"], SizeSanXuat = x["SizeSanXuat"] }).Distinct().ToList();
                    foreach (var dr in uniqueValues)
                    {
                        var rowmasize = dtSize.AsEnumerable().FirstOrDefault(r => r["SizeSanXuat"].ToString().Trim() == dr.SizeSanXuat.ToString().Trim());
                        if (rowmasize != null)
                            _dtTable.Columns.Add(rowmasize["MaSize"].ToString() + "@Size@" + dr.SizeSanXuat.ToString(), typeof(string));
                    }
                    string _strdausize = string.Empty, _sizetrung = string.Empty;
                    string newHeader = string.Empty;
                    string dausize = string.Empty;
                    string _colorID = string.Empty, _colorName = string.Empty, _po = string.Empty, _maQG = string.Empty, _dausize = string.Empty, _colorCode = string.Empty;
                    DateTime _ngayGH;
                    string error = string.Empty, _potrung = string.Empty;

                    string ngayBHStr = string.Empty;
                    for (int j = 1; j < tbColorPO.Rows.Count; j++)
                    {
                        if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("TOTAL"))
                        {
                            if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                                break;
                            continue;
                        }
                        _colorCode = tbColorPO.Rows[j][0].ToString();
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][0].ToString()))
                        {
                            _colorCode = tbColorPO.Rows[j][0].ToString().Trim();
                        }
                        if (_colorCode == "" && !tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                        {
                            _colorCode = "";
                        }

                        _colorID = tbColorPO.Rows[j][1].ToString().Trim();
                        var _rowcolorID = dtMau.AsEnumerable().FirstOrDefault(x => Regex.Replace(x["TenMau"].ToString().Trim(), @"\s+", "").ToUpper() == Regex.Replace(tbColorPO.Rows[j][1].ToString().Trim(), @"\s+", "").ToUpper()
                        && x["CodeMau"].ToString().Trim() == tbColorPO.Rows[j][0].ToString().Trim());
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][1].ToString().ToUpper()))
                        {
                            if (_rowcolorID != null)
                            {
                                _colorName = _rowcolorID == null ? "" : _rowcolorID["MaMau"].ToString();
                            }
                            else
                            {
                                if (_rowcolorID == null)
                                    mauChuaCo.Add(tbColorPO.Rows[j][0].ToString().Trim(), tbColorPO.Rows[j][1].ToString().Trim());
                            }


                        }
                        //if (_colorName == "" && !tbColorPO.Rows[j][1].ToString().Trim().ToUpper().Contains("GRAND"))
                        //{
                        //    MessageBox.Show("Màu không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    return;
                        //}
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][2].ToString()))
                        {
                            _po = "KIEMKE";//tbColorPO.Rows[j][2].ToString().Trim();
                        }
                        //if (_po == "" && !tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                        //{
                        //    MessageBox.Show("PO không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    return;
                        //}
                        //if (!string.IsNullOrEmpty(tbColorPO.Rows[j][3].ToString()))
                        //{
                        //    _dausize = tbColorPO.Rows[j][3].ToString().Trim();
                        //}
                        _dausize = string.IsNullOrEmpty(tbColorPO.Rows[j][3].ToString()) ? "0" : tbColorPO.Rows[j][3].ToString().Trim();
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][4].ToString()))
                        {
                            _maQG = tbColorPO.Rows[j][4].ToString().Trim();
                        }


                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][5].ToString()))
                        {
                            DateTime date_ngayBH;
                            if (DateTime.TryParseExact(tbColorPO.Rows[j][5].ToString().Trim().Replace("-", "/"), new[]
                            { "MM/dd/yyyy", "MM/dd/yyyy h:mm:ss", "MM/dd/yyyy h:mm:ss tt", "dd/MM/yyyy h:mm:ss", "dd/MM/yyyy h:mm:ss tt", "dd/MM/yyyy", "M/d/yyyy h:mm:ss tt", "M/d/yyyy", "M/d/yy", "yy/MM/dd", "yyyy-MM-dd", "dd-MMM-yy", "MM/yyyy/dd h:mm:ss","M/d/yyyy h:mm:ss","M/d/yy h:mm:ss","MM/dd/yy h:mm:ss", "yy/MM/dd h:mm:ss", "yyyy/MM/dd h:mm:ss","dd-MMM-yy h:mm:ss","yyyy-MM-dd h:mm:ss", "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy h:mm:ss tt", "dd\\MM\\yyyy", "dd\\MM\\yyyy HH:mm:ss","dd\\MM\\yyyy h:mm:ss tt","dd-MMM-yyyy","dd-MMMM-yyyy","dd-MMM-yyyy HH:mm:ss","dd-MMMM-yyyy HH:mm:ss","yyyy/MM/dd","yyyy-MM-dd","yyyy.MM.dd","yyyy\\MM\\dd","yyyyMMdd","dd/MM/yyyy HH:mm:ss","dd.MM.yyyy HH:mm:ss","dd\\MM\\yyyy HH:mm:ss","dd/MM/yyyy hh:mm:ss tt","dd-MM-yyyy hh:mm:ss tt","dd.MM.yyyy hh:mm:ss tt","dd\\MM\\yyyy hh:mm:ss tt","yyyy-MM-ddTHH:mm:ssZ","yyyy-MM-ddTHH:mm:ss.fffZ","yyyy-MM-ddTHH:mm:sszzz","yyyy-MM-ddTHH:mm:ss.fffzzz","dd/MM/yy","dd-MM-yy","dd.MM.yy","dd\\MM\\yy","MM/dd/yy","MM-dd-yy","MM.dd.yy","MM\\dd\\yy","yyyy/MM/dd HH:mm:ss","yyyy-MM-dd HH:mm:ss","yyyy.MM.dd HH:mm:ss","yyyy\\MM\\dd HH:mm:ss","yyyy/MM/dd hh:mm:ss tt","yyyy-MM-dd hh:mm:ss tt","yyyy.MM.dd hh:mm:ss tt","yyyy\\MM\\dd hh:mm:ss tt"
                                }, CultureInfo.InvariantCulture, DateTimeStyles.None, out date_ngayBH))
                            {
                                ngayBHStr = date_ngayBH.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                MessageBox.Show("Kiểm tra lại Ngày Giao Hàng chưa đúng định dạng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                        }
                        if (ngayBHStr == "" && !tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                        {
                            MessageBox.Show("Ngày giao hàng không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (_colorID.Trim().ToUpper().Contains("TOTAL"))
                        {
                            _po = "";
                            _colorName = "";
                            ngayBHStr = "";
                            _maQG = "";
                            if (_colorID.Trim().ToUpper().Contains("GRAND"))
                                break;
                            continue;
                        }
                        else
                        {
                            DataRow _dr = _dtTable.NewRow();
                            int n = 11;
                            for (int c = 0; c < tbColorPO.Columns.Count - 1; c++)
                            {
                                if (tbColorPO.Columns[c].ToString().Trim().ToUpper().Contains("GRAND"))
                                    break;
                                if (c <= 5)
                                {
                                    _dr[0] = "";
                                    _dr[1] = "KIEMKE";
                                    _dr[2] = _colorName.Trim();
                                    _dr[3] = _dausize.ToString().Trim();
                                    _dr[4] = _maQG.Trim();
                                    DateTime dateValue;

                                    try
                                    {
                                        dateValue = DateTime.ParseExact(ngayBHStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                        _dr[5] = dateValue;
                                        _dr[7] = _colorCode.Trim() == "" ? "" : _colorCode;
                                        _dr[8] = DBNull.Value;
                                        _dr[9] = DBNull.Value;
                                        _dr[10] = DBNull.Value;
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine("Chuỗi không có định dạng hợp lệ.");
                                    }

                                }
                                else
                                {
                                    if (KiemTraChuoiLaChu(tbColorPO.Rows[j][c].ToString().Trim()))
                                    {
                                        MessageBox.Show("Số lượng size không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                        return;
                                    }
                                    else
                                    {
                                        _dr[n] = tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "");
                                        n++;
                                    }



                                }

                            }

                            _dtTable.Rows.Add(_dr);
                        }

                    }
                    if (mauChuaCo.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Các màu sau chưa được khai báo trong thẻ màu:");

                        foreach (var kv in mauChuaCo)
                        {
                            string code = string.IsNullOrEmpty(kv.Key) ? "(Không có mã)" : kv.Key;
                            string ten = string.IsNullOrEmpty(kv.Value) ? "(Không có tên màu)" : kv.Value;
                            sb.AppendLine(string.Format("- Code Màu: {0} | Màu: {1}", code, ten));
                        }

                        XtraMessageBox.Show(sb.ToString(), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    //if (!string.IsNullOrEmpty(_dtTable.Rows[0]["MaMau"].ToString())) 
                    //{
                    //    var RowsMau = from row1 in _dtTable.AsEnumerable()
                    //                  where !dtMau.AsEnumerable().Any(row2 =>
                    //                      styleID.ToString().ToUpper().Trim() == row2["MaHang"].ToString().ToUpper().Trim() &&
                    //                      row1["MaMau"].ToString().ToUpper().Trim() == row2["MaMau"].ToString().ToUpper().Trim())
                    //                  select row1;
                    //    if (RowsMau.Any())
                    //    {
                    //        string mauMissing = string.Join("; ", RowsMau.Select(r => r["MaMau"].ToString()).Distinct().ToArray());
                    //        MessageBox.Show("Màu " + mauMissing + " không có trong nhóm mã hàng " + styleID.ToString() + " trong thư viện màu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                    //        return;
                    //    }
                    //}
                    foreach (DataRow _drmau in _dtTable.Rows)
                    {
                        var rowmau = dtMau.AsEnumerable().FirstOrDefault(r => r["TenMau"].ToString().Trim().ToUpper() == _drmau["MaMau"].ToString().Trim().ToUpper());
                        if (rowmau != null)
                            _drmau["MaMau"] = rowmau["TenMau"];
                    }

                    string urlQG = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                    string jsonQG = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlQG); }).Result;
                    List<QuocGiaEntity> lstQG = JsonConvert.DeserializeObject<List<QuocGiaEntity>>(jsonQG);

                    List<string> lstSaveQG = new List<string>();

                    for (int j = 0; j < _dtTable.Rows.Count; j++)
                    {

                        string KTQG = string.Empty;
                        QuocGiaEntity qg = lstQG.Where(item => RemoveVietnameseTone(item.TenQG).ToString().ToUpper().Trim() == RemoveVietnameseTone(_dtTable.Rows[j][4].ToString().ToUpper().Trim())).FirstOrDefault();
                        if (qg == null)
                        {
                            string hasDuplicates = lstSaveQG.Where(x => x.Equals(_dtTable.Rows[j][4].ToString())).FirstOrDefault();
                            if (string.IsNullOrEmpty(hasDuplicates))
                            {
                                if (_dtTable.Rows[j][4].ToString() != "")
                                    lstSaveQG.Add(_dtTable.Rows[j][4].ToString());
                            }
                        }
                    }
                    if (lstSaveQG.Count > 0)
                    {
                        AutoImportQGNew(lstSaveQG);
                    }
                    _dtTable = ChangeTenQGToMaQGNew(_dtTable);
                    _dtTable = _dtTable.AsEnumerable().OrderBy(x => x["PO"]).CopyToDataTable();
                    searchLookUpEditKH.EditValue = cusName;
                    searchLookUpEditMH.EditValue = styleID;
                    searchLookUpEditCL.EditValue = chungloai;
                    textDot.Text = tbl_Styles.Rows[2][2].ToString().TrimStart().TrimEnd();
                    //txtSoBooking.Text = tbl_Styles?.Rows?.Count < 4 ? string.Empty : tbl_Styles.Rows[5][2]?.ToString()?.TrimStart()?.TrimEnd();
                    createSearchLookup();
                    CreateSearchLookupQG();
                    CreateSearchLookupQuocGia();
                    CreateSearchLookupMaMau();
                    CreateSearchLookupDauSize();
                    CreateSearchLookupCL();
                    bool checkDuplicateData = _dtTable.AsEnumerable().GroupBy(row => new
                    {
                        DauSize = row.Field<string>("DauSize"),
                        PO = row.Field<string>("PO"),
                        MaMau = row.Field<string>("MaMau")
                    }).Any(g => g.Count() > 1);
                    //if (checkDuplicateData)
                    //{
                    //    XtraMessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    _dtTable.Clear();
                    //    searchLookUpEditMH.EditValue = null;
                    //    searchLookUpEditKH.EditValue = null;
                    //    textDot.Text = "";
                    //    searchLookUpEditCL.EditValue = null;
                    //    txtSoBooking.Text = string.Empty;
                    //    return;
                    //}
                    gridBandColSize.Children.Clear();
                    gridControl1.MainView = CreateBandSize(_dtTable);
                    BandedGridView mainView = (BandedGridView)gridControl1.MainView;
                    mainView.CellValueChanging += mainView_CellValueChanging;
                    //mainView.CustomUnboundColumnData += mainView_CustomUnboundColumnData;
                    mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                    mainView.CustomDrawCell += MainView_CustomDrawCell;
                    gridControl1.DataSource = _dtTable;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file excel và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable dataSource = gridControl1.DataSource as DataTable;
            if (dataSource != null) 
            {
                if (!dataSource.Columns.Contains("CheckColumn"))
                {
                    dataSource.Columns.Add("CheckColumn", typeof(bool));
                }

                // Gán giá trị mặc định true cho cột "CheckColumn" cho tất cả các dòng trong DataTable
                foreach (DataRow row in dataSource.Rows)
                {
                    row["CheckColumn"] = true;
                }
            }

        }

        private void bandedGridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
            {
                e.DisplayText = "-";
            }
            else
            {
                string[] arr = e.Column.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Count() > 1)
                {
                    if (Convert.ToInt32(e.Value.ToString()) == 0)
                        e.DisplayText = "-";
                }
            }
        }

        private void searchLookUpEditDauSize_SelectionChanged(object sender, EventArgs e)
        {

            Control c = this.txtBoxDauSize;

            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {
                StringBuilder sb = new StringBuilder();
                if (checkEditTV.Checked == false)
                {
                    foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(":"); }
                        sb.Append(rv["MaInSeam"].ToString());
                    }
                }
                else
                {
                    foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(":"); }
                        sb.Append(rv["DauSizeID"].ToString());
                    }
                }


                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();
                dausize = sb.ToString();
                InitSearchLookupSize();
            }
        }

        private void InitSearInitSearchLookupMaMau()
        {
            try
            {

                if (txtBoxMau.Enabled == true)
                {
                    if (searchLookUpEditMH.EditValue != null)
                    {
                        gridCheckMarksColor.Selection.Clear();
                        if (checkEditTV.Checked == false)
                        {
                            string urlMau = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                            DataTable tblMau = JsonConvert.DeserializeObject<DataTable>(json);
                            txtBoxMau.Properties.DataSource = tblMau;
                        }
                        else
                        {
                            string urlMau = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                            DataTable tblMau = JsonConvert.DeserializeObject<DataTable>(json);
                            txtBoxMau.Properties.DataSource = tblMau;
                        }
                        GridView dvView = txtBoxMau.Properties.View;
                        if (dvView.Columns.Count == 1)
                        {
                            dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                            dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                            dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                            dvView.Appearance.HeaderPanel.Options.UseBackColor = true;
                            dvView.Columns.Add(new GridColumn { FieldName = "MaMau", Caption = "Mã Màu", Name = "bandedGridColumn3", Visible = false });
                            dvView.Columns.Add(new GridColumn { FieldName = "TenMau", Caption = "Tên Màu", Name = "bandedGridColumn8", Visible = true });
                            dvView.Columns.Add(new GridColumn { FieldName = "CodeMau", Caption = "Code Màu", Name = "bandedGridColumn14", Visible = true });
                            dvView.Columns.Add(new GridColumn { FieldName = "TheMau", Caption = "Thẻ Màu", Visible = true });

                            // Cấu hình giao diện header
                            dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                            dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                            dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                            dvView.Appearance.HeaderPanel.Options.UseBackColor = true;

                            // Group theo MaMau
                            dvView.Columns["TheMau"].GroupIndex = 0;

                            // Cho phép hiển thị nhóm
                            dvView.OptionsView.ShowGroupPanel = false;
                            dvView.ExpandAllGroups();
                            dvView.OptionsBehavior.AutoExpandAllGroups = true;
                            dvView.GroupFormat = "{1}";
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else { return; }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void InitSearchLookupSize()
        {
            try
            {
                if (txtBoxSize.Enabled == true) 
                {
                    if (searchLookUpEditMH.EditValue != null)
                    {
                        gridCheckMarksSize.Selection.Clear();

                        if (checkEditTV.Checked == false)
                        {
                            string urlSize = string.Format("{0}", URL + $"ERPBangSize/Get?action=Getizev1");
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                            DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(json);
                            txtBoxSize.Properties.DataSource = tblSize;
                        }
                        else
                        {
                            string urlSize = string.Format("{0}?mahang={1}&&makh={2}&&dausize={3}", URL + "DonHangTong/GetSizeEditV1", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString(), dausize.ToString());
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                            DataTable tblSize = JsonConvert.DeserializeObject<DataTable>(json);
                            txtBoxSize.Properties.DataSource = tblSize;
                        }
                        //rMauEdit.EditValueChanged += rMauEdit_EditValueChanged;
                        GridView dvView = txtBoxSize.Properties.View;

                        if (dvView.Columns.Count == 1)
                        {
                            dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                            dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                            dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                            //dvView.Appearance.HeaderPanel.Options.UseBackColor = true;
                            //dvView.Columns.Add(new GridColumn { FieldName = "Chon", Caption = "", Name = "colChonHH", Visible = true });
                            dvView.Columns.Add(new GridColumn { FieldName = "TenSize", Caption = " Size Sản Xuất", Name = "bandedGridColumn12", Visible = true });
                            //dvView.Columns.Add(new GridColumn { FieldName = "TenSize", Caption = "Tên Size", Name = "bandedGridColumn13", Visible = true });
                            dvView.Columns.Add(new GridColumn { FieldName = "TheSize", Caption = "Thẻ Size", Visible = true });

                            // Group theo MaMau
                            dvView.Columns["TheSize"].GroupIndex = 0;

                            // Cho phép hiển thị nhóm
                            dvView.OptionsView.ShowGroupPanel = false;
                            dvView.ExpandAllGroups();
                            dvView.OptionsBehavior.AutoExpandAllGroups = true;
                            dvView.GroupFormat = "{1}";
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else { return; }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitSearchLookupDauSize()
        {
            try
            {
                if (txtBoxDauSize.Enabled == true)
                {
                    if (searchLookUpEditMH.EditValue != null)
                    {
                        gridCheckMarksDauSize.Selection.Clear();
                        if (checkEditTV.Checked == false)
                        {
                            string urlDSize = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeamv1");
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSize); }).Result;
                            DataTable tblDSize = JsonConvert.DeserializeObject<DataTable>(json);
                            txtBoxDauSize.Properties.DataSource = tblDSize;
                        }
                        else
                        {
                            string urlDSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetDauSizeV1", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSize); }).Result;
                            DataTable tblDSize = JsonConvert.DeserializeObject<DataTable>(json);
                            txtBoxDauSize.Properties.DataSource = tblDSize;
                        }

                        //rMauEdit.EditValueChanged += rMauEdit_EditValueChanged;
                        GridView dvView = txtBoxDauSize.Properties.View;
                        string[] colNeedRemove = new string[] { "MaInSeam", "DauSizeID", "InSeam", "DauSize", "TheInSeam" };
                        foreach (GridColumn col in dvView.Columns.Cast<GridColumn>().ToList())
                        {
                            if (colNeedRemove.Contains(col.FieldName))
                                dvView.Columns.Remove(col);
                        }
                        //if (dvView.Columns.Count == 1)
                        //{
                        //    dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                        //    dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        //    dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                        //    dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                        //    //dvView.Appearance.HeaderPanel.Options.UseBackColor = true;
                        //    //dvView.Columns.Add(new GridColumn { FieldName = "Chon", Caption = "", Name = "colChonHH", Visible = true });
                        //    dvView.Columns.Add(new GridColumn { FieldName = checkEditTV.Checked == false ? "MaInSeam" : "DauSizeID", Caption = " Mã Đầu Size", Name = "", Visible = false });
                        //    dvView.Columns.Add(new GridColumn { FieldName = checkEditTV.Checked == false ? "InSeam" : "DauSize", Caption = "InSeam", Name = "bandedGridColumn4", Visible = true });
                        //    dvView.Columns.Add(new GridColumn { FieldName = "TheInSeam", Caption = "Nhóm thẻ InSeam", Visible = true });

                        //    // Group theo MaMau
                        //    dvView.Columns["TheInSeam"].GroupIndex = 0;

                        //    // Cho phép hiển thị nhóm
                        //    dvView.OptionsView.ShowGroupPanel = false;
                        //    dvView.ExpandAllGroups();
                        //    dvView.OptionsBehavior.AutoExpandAllGroups = true;
                        //    dvView.GroupFormat = "{1}";
                        //}
                        dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                        dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                        dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                        //dvView.Appearance.HeaderPanel.Options.UseBackColor = true;
                        //dvView.Columns.Add(new GridColumn { FieldName = "Chon", Caption = "", Name = "colChonHH", Visible = true });
                        dvView.Columns.Add(new GridColumn { FieldName = checkEditTV.Checked == false ? "MaInSeam" : "DauSizeID", Caption = " Mã Đầu Size", Name = "", Visible = false });
                        dvView.Columns.Add(new GridColumn { FieldName = checkEditTV.Checked == false ? "InSeam" : "DauSize", Caption = "InSeam", Name = "bandedGridColumn4", Visible = true });
                        dvView.Columns.Add(new GridColumn { FieldName = "TheInSeam", Caption = "Nhóm thẻ InSeam", Visible = true });

                        // Group theo MaMau
                        dvView.Columns["TheInSeam"].GroupIndex = 0;

                        // Cho phép hiển thị nhóm
                        dvView.OptionsView.ShowGroupPanel = false;
                        dvView.ExpandAllGroups();
                        dvView.OptionsBehavior.AutoExpandAllGroups = true;
                        dvView.GroupFormat = "{1}";
                    }
                    else
                    {
                        return;
                    }
                }
                else { return; }
                
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateSearchLookupQG()
        {
            string url = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            SearchLookUpEditQG.Properties.DataSource = tbl;
            SearchLookUpEditQG.Properties.DisplayMember = "TenQG";
            SearchLookUpEditQG.Properties.ValueMember = "MaQG";
            SearchLookUpEditQG.Properties.ShowClearButton = false;
            SearchLookUpEditQG.Properties.NullText = "Chọn QG";

            GridView dvView = SearchLookUpEditQG.Properties.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaQG", Caption = "Mã QG", Name = "colMaQG", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenQG", Caption = "Quốc gia", Name = "colTenQG", Visible = true });
            }
        }

        private void bandedGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void bandedGridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (bandedGridView1.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();
                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {

                        // Example of column names where you want the context menu to appear
                        List<string> allowedColumns = new List<string> { "ID", "MaDH", "POID", "PO", "MaQG", "DauSizeID", "DauSize", "NgayGH", "GhiChu", "MaMau" };

                        if (!allowedColumns.Contains(e.HitInfo.Column.FieldName))
                        {
                            // ColumnName.Contains("@Size@")
                            if (e.HitInfo.Column.FieldName.Contains("@Size@") || e.HitInfo.Column.FieldName.Contains("@SIZE@"))
                            {
                                // Add the "Copy" item to the context menu
                                DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy", ItemCopy_Click);
                                e.Menu.Items.Add(menuCopyItem);

                                // Add the "Paste" item to the context menu
                                DevExpress.Utils.Menu.DXMenuItem menuPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Paste", ItemPaste_Click);
                                e.Menu.Items.Add(menuPasteItem);
                            }
                        }
                        //if (isEdit) 
                        //{
                        //    return;
                        //}
                        DevExpress.Utils.Menu.DXMenuItem menuCoppyPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Copy dòng", ItemCoppyPaste_Click);
                        e.Menu.Items.Add(menuCoppyPasteItem);
                        DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng mới thêm", ItemDelete_Click);
                        e.Menu.Items.Add(menuDeleteItem);
                    }

                }
            }
        }

        private void ItemCopy_Click(object sender, EventArgs e)
        {
            BandedGridView view = gridControl1.MainView as BandedGridView;
            view.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
            view.CopyToClipboard();

        }
        private void ItemPaste_Click(object sender, EventArgs e)
        {
            //BandedGridView view = gridControl1.MainView as BandedGridView;
            //string[] data = ClipboardData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //if (data.Length < 1) return;
            //int startRow = view.FocusedRowHandle;
            //for(int i = 1; i < data.Length; i++)
            //{
            //    AddRow(data[i], startRow++);
            //    if (!view.IsValidRowHandle(startRow)) break;
            //}
            BandedGridView view = gridControl1.MainView as BandedGridView;
            string[] data = ClipboardData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length < 1) return;
            int startRow = view.FocusedRowHandle;
            foreach (string row in data)
            {
                AddRow(row, startRow++);
                if (!view.IsValidRowHandle(startRow)) break;
            }
        }

        private void ItemCoppyPaste_Click(object sender, EventArgs e)
        {
            // Lấy view từ GridControl
            BandedGridView view = gridControl1.MainView as BandedGridView;

            // Lấy vị trí của dòng được chọn
            //int focusedRowHandle = view.FocusedRowHandle;
            //if (focusedRowHandle < 0) return;
            int[] selectedRowHandles = view.GetSelectedRows();
            if (selectedRowHandles.Length == 0) return;
            // Tạo một từ điển chứa dữ liệu của dòng được chọn
            //var rowData = new Dictionary<string, object>();
            //foreach (DevExpress.XtraGrid.Columns.GridColumn column in view.VisibleColumns)
            //{
            //    string fieldName = column.FieldName;
            //    if (!string.IsNullOrEmpty(fieldName))
            //    {
            //        rowData[fieldName] = view.GetRowCellValue(focusedRowHandle, column);
            //    }
            //}


            DataTable dataSource = gridControl1.DataSource as DataTable;
            if (dataSource == null) return;

            //DataRow newRow = view.GetFocusedDataRow();


            //// Tạo một dòng mới và thêm vào dataSource
            //DataRow insertedRow = dataSource.NewRow();

            //// Gán giá trị cho các cột trong dòng mới 
            //foreach (DataColumn column in dataSource.Columns)
            //{
            //    if (newRow.Table.Columns.Contains(column.ColumnName))
            //    {
            //        insertedRow[column.ColumnName] = newRow[column.ColumnName];
            //    }
            //}

            //// Đặt giá trị mặc định cho "CheckColumn" trong dòng mới là false
            //insertedRow["CheckColumn"] = false;

            //// Chèn dòng mới vào DataTable (sau dòng hiện tại)
            ////dataSource.Rows.InsertAt(insertedRow, focusedRowHandle + 1);
            Array.Sort(selectedRowHandles);
            Array.Reverse(selectedRowHandles);

            foreach (int rowHandle in selectedRowHandles)
            {
                // Lấy dữ liệu của từng dòng được chọn
                DataRow selectedRow = view.GetDataRow(rowHandle);
                if (selectedRow == null) continue;

                // Tạo một dòng mới hoàn toàn
                DataRow newRow = dataSource.NewRow();

                // Sao chép dữ liệu từ dòng được chọn vào dòng mới
                foreach (DataColumn column in dataSource.Columns)
                {
                    if (selectedRow.Table.Columns.Contains(column.ColumnName))
                    {
                        newRow[column.ColumnName] = selectedRow[column.ColumnName];
                    }
                }

                // Đặt giá trị mặc định cho "CheckColumn" là false
                if (dataSource.Columns.Contains("CheckColumn"))
                {
                    newRow["CheckColumn"] = false;
                }

                // Xác định vị trí chèn
                int insertIndex = rowHandle + 1;
                if (insertIndex > dataSource.Rows.Count)
                {
                    insertIndex = dataSource.Rows.Count;
                }

                // Chèn dòng mới vào DataTable
                dataSource.Rows.InsertAt(newRow, insertIndex);
            }

            // Cập nhật lại GridControl
            view.RefreshData();

            // Cập nhật lại GridControl
            gridControl1.DataSource = dataSource;
            gridControl1.RefreshDataSource();
            view.RowStyle += (s, z) =>
            {
                var gridView = s as BandedGridView;
                if (gridView == null) return;

                // Lấy dữ liệu của dòng hiện tại
                DataRow row = gridView.GetDataRow(z.RowHandle);
                if (row == null) return;

                // Kiểm tra nếu dòng được đánh dấu là dòng mới
                if (row.Table.Columns.Contains("CheckColumn") && row["CheckColumn"] != DBNull.Value && (bool)row["CheckColumn"] == false)
                {
                    z.Appearance.BackColor = System.Drawing.Color.Yellow; // Tô màu nền
                                                                          //z.Appearance.BackColor2 = System.Drawing.Color.White;    // Hiệu ứng gradient
                }
            };

        }
        private void ItemDelete_Click(object sender, EventArgs e)
        {
            BandedGridView view = gridControl1.MainView as BandedGridView;
            if (view.SelectedRowsCount > 0)
            {
                int selectedIndex = view.GetSelectedRows()[0];
                DataRow row = view.GetDataRow(selectedIndex);
                if (row != null)
                {
                    ((DataTable)gridControl1.DataSource).Rows.Remove(row);
                }
                view.RefreshData();
            }
        }
        private void AddRow(string data, int rowHandle)
        {
            BandedGridView view = gridControl1.MainView as BandedGridView;
            if (data == string.Empty) return;
            string[] rowData = data.Split('\t');
            int column = view.FocusedColumn.VisibleIndex;
            for (int i = 0; i < rowData.Length; i++)
            {
                if (i >= view.VisibleColumns.Count) break;
                if (rowData[i] == "-")
                    rowData[i] = "0";
                view.SetRowCellValue(rowHandle, view.VisibleColumns[column + i], rowData[i].Replace(",", ""));
            }

        }
        private string ClipboardData
        {
            get
            {
                IDataObject iData = Clipboard.GetDataObject();
                if (iData == null) return "";

                if (iData.GetDataPresent(DataFormats.Text))
                    return (string)iData.GetData(DataFormats.Text);
                return "";
            }
            set
            {
                Clipboard.SetDataObject(value);
            }
        }

        private void gridControl1_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                BandedGridView view = gridControl1.MainView as BandedGridView;
                view.CopyToClipboard();
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                BandedGridView view = gridControl1.MainView as BandedGridView;
                string[] data = ClipboardData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length < 1) return;
                int startRow = view.FocusedRowHandle;
                for (int i = 1; i < data.Length; i++) // Bắt đầu từ phần tử thứ 2 trong mảng
                {
                    AddRow(data[i], startRow++);
                    if (!view.IsValidRowHandle(startRow)) break;
                }

            }
        }

        private void bandedGridView1_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            DataRow row = view.GetDataRow(e.RowHandle);
            if (row != null && row.Table.Columns.Contains("IsNew") && row["IsNew"] is bool isNew && isNew)
            {
                e.Appearance.BackColor = Color.LightGreen;
                e.Appearance.BackColor2 = Color.LightYellow;
            }

        }

        private void CreateSearchLookupQuocGia()
        {
            string url = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tbl;
            rCountryEdit.DisplayMember = "TenQG";
            rCountryEdit.ValueMember = "MaQG";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";
            rCountryEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
            rCountryEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaQG", Caption = "Mã quốc gia", Name = "colMaQG", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenQG", Caption = "Quốc gia", Name = "colTenQG", Visible = true });

            }
            gridcolQG.ColumnEdit = rCountryEdit;
        }

        private void CreateSearchLookupMaMau()
        {
            string json = string.Empty;
            if (checkEditTV.Checked == false)
            {
                string url = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            }
            else
            {
                string url = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            }
            DataTable tblmau = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tblmau;
            rCountryEdit.DisplayMember = "TenMau";
            rCountryEdit.ValueMember = "MaMau";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";
            rCountryEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
            rCountryEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
            GridView dvView = rCountryEdit.View;
            rCountryEdit.EditValueChanged += new System.EventHandler(this.searchLookUpEditMau_EditValueChanged);
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaMau", Caption = "Mã màu", Name = "colMaMau", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenMau", Caption = "Tên màu", Name = "colTenMau", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "CodeMau", Caption = "Code màu", Name = "colTenMau", Visible = true });

                GridColumn colGroup = new GridColumn
                {
                    FieldName = "TheMau",
                    Caption = "",
                    Name = "colTheMau",
                    Visible = true
                };
                dvView.Columns.Add(colGroup);

                colGroup.GroupIndex = 0;
                dvView.OptionsView.ShowGroupPanel = false;
                dvView.OptionsBehavior.AutoExpandAllGroups = true;
                dvView.GroupFormat = "{1}";

            }
            gridColMau.ColumnEdit = rCountryEdit;

        }

        private void searchLookUpEditMau_EditValueChanged(object sender, EventArgs e)
        {
            //SendKeys.SendWait("{ENTER}");
            //this.ActiveControl = this.txtMaDH;

            //string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
            string urlMau = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
            string jsonmau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
            DataTable tblmau = JsonConvert.DeserializeObject<DataTable>(jsonmau);
            var rCountryEdit = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (rCountryEdit == null) return;
            DevExpress.XtraGrid.Views.Grid.GridView gridView = (DevExpress.XtraGrid.Views.Grid.GridView)rCountryEdit.Properties.View;
            int focusedRowHandle = gridView.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                DevExpress.XtraGrid.Views.Grid.GridView gridViews = (DevExpress.XtraGrid.Views.Grid.GridView)gridControl1.MainView;
                DataRow focusedRow = gridView.GetDataRow(focusedRowHandle);
                if (focusedRow != null)
                {
                    gridViews.SetRowCellValue(gridViews.FocusedRowHandle, "MaMau", focusedRow["MaMau"].ToString());
                    var rowctmau = tblmau.AsEnumerable().FirstOrDefault(x => x["MaMau"].ToString() == focusedRow["MaMau"].ToString());
                    if (rowctmau != null)
                    {
                        gridViews.SetRowCellValue(gridViews.FocusedRowHandle, "ColorCode", rowctmau["CodeMau"].ToString());
                    }

                }
            }
        }

        private void bandedGridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (isEdit)
            {
                lstRowUpdate.Add(e.RowHandle);
            }
        }

        private void bandedGridView1_Click(object sender, EventArgs e)
        {

        }

        private void bandedGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            GridHitInfo hitInfo = bandedGridView1.CalcHitInfo(e.Location);

            // Kiểm tra nếu nhấn vào ô trong cột "DauSize"
            if (hitInfo.InRowCell == true) 
            {
                if (hitInfo.InRowCell && hitInfo.Column != null && hitInfo.Column.FieldName == "DauSize" || hitInfo.Column.FieldName == "MaQG" || hitInfo.Column.FieldName == "MaMau")
                {
                    bandedGridView1.FocusedColumn = hitInfo.Column;
                    bandedGridView1.FocusedRowHandle = hitInfo.RowHandle;
                    bandedGridView1.ShowEditor(); // Kích hoạt editor

                    // Kiểm tra nếu editor là SearchLookUpEdit thì hiển thị popup ngay lập tức
                    if (bandedGridView1.ActiveEditor is SearchLookUpEdit searchLookUpEdit)
                    {
                        searchLookUpEdit.ShowPopup();
                    }
                }
            }
            
        }

        private void bandedGridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (bandedGridView1.FocusedColumn != null && bandedGridView1.FocusedColumn.FieldName == "DauSize" || bandedGridView1.FocusedColumn.FieldName == "MaQG" || bandedGridView1.FocusedColumn.FieldName == "MaMau")
            {
                gridControl1.BeginInvoke(new Action(() =>
                {
                    if (bandedGridView1.ActiveEditor is SearchLookUpEdit searchLookUpEdit)
                    {
                        searchLookUpEdit.ShowPopup();
                    }
                }));
            }
        }

        private void Sua_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void CreateSearchLookupDauSize()
        {
            string json = string.Empty;
            if (checkEditTV.Checked == false)
            {
                string url = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeamv1");
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
                rCountryEdit.DataSource = tbl;
                rCountryEdit.DisplayMember = "InSeam";
                rCountryEdit.ValueMember = "MaInSeam";
                rCountryEdit.ShowClearButton = false;
                rCountryEdit.NullText = "[Chọn giá trị]";
                rCountryEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
                rCountryEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
                GridView dvView = rCountryEdit.View;
                if (dvView.Columns.Count == 0)
                {
                    dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    dvView.Columns.Add(new GridColumn { FieldName = "InSeam", Caption = "Mã Đầu Size", Name = "colMaQG", Visible = false });
                    dvView.Columns.Add(new GridColumn { FieldName = "MaInSeam", Caption = "InSeam", Name = "colTenQG", Visible = true });
                    GridColumn colGroup = new GridColumn
                    {
                        FieldName = "TheInSeam",
                        Caption = "",
                        Name = "colTheMau",
                        Visible = true
                    };
                    dvView.Columns.Add(colGroup);

                    colGroup.GroupIndex = 0;
                    dvView.OptionsView.ShowGroupPanel = false;
                    dvView.OptionsBehavior.AutoExpandAllGroups = true;
                    dvView.GroupFormat = "{1}";
                }
                gridColDauSize.ColumnEdit = rCountryEdit;
            }
            else
            {
                string url = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetDauSize", searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
                rCountryEdit.DataSource = tbl;
                rCountryEdit.DisplayMember = "DauSize";
                rCountryEdit.ValueMember = "DauSize";
                rCountryEdit.ShowClearButton = false;
                rCountryEdit.NullText = "[Chọn giá trị]";
                rCountryEdit.ImmediatePopup = true;  // Hiển thị popup ngay khi nhấp vào ô
                rCountryEdit.TextEditStyle = TextEditStyles.Standard;  // Đảm bảo popup luôn xuất hiện
                GridView dvView = rCountryEdit.View;
                if (dvView.Columns.Count == 0)
                {
                    dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                    dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                    dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                    dvView.Columns.Add(new GridColumn { FieldName = "DauSizeID", Caption = "Mã Đầu Size", Name = "colMaQG", Visible = false });
                    dvView.Columns.Add(new GridColumn { FieldName = "DauSize", Caption = "InSeam", Name = "colTenQG", Visible = true });
                    GridColumn colGroup = new GridColumn
                    {
                        FieldName = "TheInSeam",
                        Caption = "",
                        Name = "colTheMau",
                        Visible = true
                    };
                    dvView.Columns.Add(colGroup);

                    colGroup.GroupIndex = 0;
                    dvView.OptionsView.ShowGroupPanel = false;
                    dvView.OptionsBehavior.AutoExpandAllGroups = true;
                    dvView.GroupFormat = "{1}";
                }
                gridColDauSize.ColumnEdit = rCountryEdit;
            }

        }
        private void AutoImportQGNew(List<string> lstSaveQG)
        {
            List<QuocGiaEntity> lstAutoImportQG = new List<QuocGiaEntity>();
            QuocGiaEntity autoImportSizeItem;

            if (lstSaveQG != null && lstSaveQG.Count > 0)
            {
                lstSaveQG = lstSaveQG.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstSaveQG.Count; i++)
                {
                    QuocGiaEntity itemQG = new QuocGiaEntity(lstSaveQG[i]);
                    lstAutoImportQG.Add(itemQG);
                }
                if (lstAutoImportQG.Count > 0)
                {
                    string urlAutoImportQG = string.Format("{0}?", URL + "QuocGia/PostAutoQG");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlAutoImportQG, lstAutoImportQG); }).Result;
                }
            }
        }

        private void txtBoxSize_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void dateEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            var view = gridControl1.MainView as BandedGridView;
            if (view == null) return;
            if (dateEdit1.EditValue == null) return;

            // Lấy ngày chọn
            DateTime date = Convert.ToDateTime(dateEdit1.EditValue);

            // Lấy danh sách cell đang chọn
            var selectedCells = view.GetSelectedCells();

            string targetField = "NgayGH"; // cột muốn fill
            var targetCol = view.Columns[targetField];
            if (targetCol == null) return;

            view.BeginUpdate();
            try
            {
                bool hasSelectedCellInTargetColumn = false;

                // Nếu có cell được chọn
                if (selectedCells != null && selectedCells.Length > 0)
                {
                    foreach (GridCell cell in selectedCells)
                    {
                        if (cell.RowHandle < 0 || cell.Column == null) continue;

                        if (cell.Column.FieldName == targetField)
                        {
                            hasSelectedCellInTargetColumn = true;
                            view.SetRowCellValue(cell.RowHandle, cell.Column, date);
                        }
                    }
                }

                if (!hasSelectedCellInTargetColumn)
                {
                    for (int i = 0; i < view.RowCount; i++)
                    {
                        if (view.IsGroupRow(i)) continue;
                        view.SetRowCellValue(i, targetCol, date);
                    }
                }
            }
            finally
            {
                view.EndUpdate();
                view.RefreshData();
            }
        }

        private void dateEdit2_Properties_EditValueChanged(object sender, EventArgs e)
        {
            var view = gridControl1.MainView as BandedGridView;
            if (view == null) return;
            if (dateEdit2.EditValue == null) return;

            DateTime date = Convert.ToDateTime(dateEdit2.EditValue);

            var selectedCells = view.GetSelectedCells();

            string targetField = "NgayDuKienVC";

            var targetCol = view.Columns[targetField];
            if (targetCol == null) return;

            view.BeginUpdate();
            try
            {
                bool hasSelectedCellInTargetColumn = false;

                if (selectedCells != null && selectedCells.Length > 0)
                {
                    foreach (GridCell cell in selectedCells)
                    {
                        if (cell.RowHandle < 0 || cell.Column == null) continue;

                        if (cell.Column.FieldName == targetField)
                        {
                            hasSelectedCellInTargetColumn = true;
                            view.SetRowCellValue(cell.RowHandle, cell.Column, date);
                        }
                    }
                }

                if (!hasSelectedCellInTargetColumn)
                {
                    for (int i = 0; i < view.RowCount; i++)
                    {
                        if (view.IsGroupRow(i)) continue;
                        view.SetRowCellValue(i, targetCol, date);
                    }
                }
            }
            finally
            {
                view.EndUpdate();
                view.RefreshData();
            }
        }

        private void dateEdit4_Properties_EditValueChanged(object sender, EventArgs e)
        {
            var view = gridControl1.MainView as BandedGridView;
            if (view == null) return;
            if (dateEdit4.EditValue == null) return;

            DateTime date = Convert.ToDateTime(dateEdit4.EditValue);

            var selectedCells = view.GetSelectedCells();

            string targetField = "NgayThucTeVC";
            var targetCol = view.Columns[targetField];
            if (targetCol == null) return;

            view.BeginUpdate();
            try
            {
                bool hasSelectedCellInTargetColumn = false;

                if (selectedCells != null && selectedCells.Length > 0)
                {
                    foreach (GridCell cell in selectedCells)
                    {
                        if (cell.RowHandle < 0 || cell.Column == null) continue;

                        if (cell.Column.FieldName == targetField)
                        {
                            hasSelectedCellInTargetColumn = true;
                            view.SetRowCellValue(cell.RowHandle, cell.Column, date);
                        }
                    }
                }

                if (!hasSelectedCellInTargetColumn)
                {
                    for (int i = 0; i < view.RowCount; i++)
                    {
                        if (view.IsGroupRow(i)) continue;
                        view.SetRowCellValue(i, targetCol, date);
                    }
                }
            }
            finally
            {
                view.EndUpdate();
                view.RefreshData();
            }
        }

        private void dateEdit3_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void dateEdit3_Properties_EditValueChanged(object sender, EventArgs e)
        {
            var view = gridControl1.MainView as BandedGridView;
            if (view == null) return;
            if (dateEdit3.EditValue == null) return;

            DateTime date = Convert.ToDateTime(dateEdit3.EditValue);

            var selectedCells = view.GetSelectedCells();

            string targetField = "NgayXuatHang";
            var targetCol = view.Columns[targetField];
            if (targetCol == null) return;

            view.BeginUpdate();
            try
            {
                bool hasSelectedCellInTargetColumn = false;

                if (selectedCells != null && selectedCells.Length > 0)
                {
                    foreach (GridCell cell in selectedCells)
                    {
                        if (cell.RowHandle < 0 || cell.Column == null) continue;

                        if (cell.Column.FieldName == targetField)
                        {
                            hasSelectedCellInTargetColumn = true;
                            view.SetRowCellValue(cell.RowHandle, cell.Column, date);
                        }
                    }
                }

                if (!hasSelectedCellInTargetColumn)
                {
                    for (int i = 0; i < view.RowCount; i++)
                    {
                        if (view.IsGroupRow(i)) continue;
                        view.SetRowCellValue(i, targetCol, date);
                    }
                }
            }
            finally
            {
                view.EndUpdate();
                view.RefreshData();
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
             string mkh = searchLookUpEditKH.EditValue.ToString();
            string mh = searchLookUpEditMH.EditValue.ToString();
            var bandedView = gridControl1.MainView as BandedGridView;
            if (bandedView == null) return;
            var inseamList = new HashSet<string>();
            for (int i = 0; i < bandedView.RowCount; i++)
            {
                var value = bandedView.GetRowCellValue(i, "DauSize");
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    inseamList.Add(value.ToString().Trim());
                }
            }

            List<string> existedSizes = new List<string>();
            foreach (DataColumn col in tblChiTiet.Columns)
            {
                if (col.ColumnName.Contains("@"))
                {
                    string sizeName = col.ColumnName.Split('@')[2];
                    existedSizes.Add(sizeName);
                }
            }

            string inseam = string.Join(":", inseamList);
            using (frmImportSize frm = new frmImportSize(existedSizes, mkh, mh, inseam, _unsavedSelectedSizes))
            {
                frm.StartPosition = FormStartPosition.CenterScreen;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    string urlSizecheck = string.Format("{0}?mahang={1}&&makh={2}&&dausize={3}", URL + "DonHangTong/GetSizeEdit", mh, mkh, inseam);
                    string jsonSizecheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSizecheck); }).Result;
                    DataTable dtSizeCheck = JsonConvert.DeserializeObject<DataTable>(jsonSizecheck);

                    var newSelectedIds = frm.SelectedSizes.Select(s => s.MaSize).ToList();
                    var oldUnsaved = _unsavedSelectedSizes.ToList();
                    // Lấy danh sách size đã chọn
                    foreach (var size in frm.SelectedSizes)
                    {
                        // Tạo tên cột dạng Size@MaHang
                        string colName = size.SizeSanXuat + "@SIZE@" + size.MaSize;

                        if (!tblChiTiet.Columns.Contains(colName))
                        {
                            tblChiTiet.Columns.Add(colName, typeof(int));
                            foreach (DataRow row in tblChiTiet.Rows)
                            {
                                row[colName] = 0; // mặc định số lượng = 0
                            }
                        }

                        if (!_unsavedSelectedSizes.Contains(size.MaSize))
                            _unsavedSelectedSizes.Add(size.MaSize);

                    }


                    // lấy chuỗi X:L:M từ form con
                    //string selectedSizeString = frm.Tag?.ToString();
                    // (selectedSizeString có dạng "X:L:M")
                    var toRemove = oldUnsaved
                        .Where(id => !newSelectedIds.Contains(id))
                        .ToList();

                    foreach (var id in toRemove)
                    {
                        var colToRemove = tblChiTiet.Columns
                            .Cast<DataColumn>()
                            .FirstOrDefault(c => c.ColumnName.Contains("@SIZE@" + id));

                        if (colToRemove != null)
                        {
                            tblChiTiet.Columns.Remove(colToRemove);
                        }

                        _unsavedSelectedSizes.Remove(id);
                    }

                    if (dtSizeCheck != null && dtSizeCheck.Rows.Count > 0)
                    {
                        // B1: map MaSize -> Sort
                        //Dictionary<string, int> sizeSortMap = dtSizeCheck.AsEnumerable()
                        //    .ToDictionary(r => r["MaSize"].ToString(),
                        //                  r => r.Table.Columns.Contains("Sort") ? Convert.ToInt32(r["Sort"]) : 9999);
                        Dictionary<string, int> sizeSortMap = new Dictionary<string, int>();
                        HashSet<int> usedSorts = new HashSet<int>();

                        foreach (DataRow r in dtSizeCheck.Rows)
                        {
                            string maSize = r["MaSize"].ToString();
                            int sort = 9999;

                            if (r.Table.Columns.Contains("Sort") && int.TryParse(r["Sort"].ToString(), out int s))
                                sort = s;

                            // Nếu Sort bị trùng, tự động +1 cho tới khi không trùng
                            while (usedSorts.Contains(sort))
                            {
                                sort++;
                            }

                            usedSorts.Add(sort);
                            sizeSortMap[maSize] = sort;
                        }

                        // B2: Lấy cột size hiện có
                        var sizeCols = tblChiTiet.Columns.Cast<DataColumn>()
                            .Where(c => c.ColumnName.Contains("@SIZE@"))
                            .ToList();

                        // B3: Sắp xếp lại theo Sort
                        var orderedCols = sizeCols
                            .OrderBy(c =>
                            {
                                string maSize = c.ColumnName.Split('@').Last();
                                return sizeSortMap.ContainsKey(maSize) ? sizeSortMap[maSize] : int.MaxValue;
                            })
                            .ToList();

                        // B4: Tạo DataTable mới theo thứ tự cột mới
                        DataTable newTbl = new DataTable();
                        foreach (DataColumn col in tblChiTiet.Columns)
                        {
                            if (!col.ColumnName.Contains("@SIZE@"))
                                newTbl.Columns.Add(col.ColumnName, col.DataType);
                        }
                        foreach (DataColumn col in orderedCols)
                        {
                            newTbl.Columns.Add(col.ColumnName, col.DataType);
                        }

                        // B5: Copy dữ liệu từng dòng theo đúng cột
                        foreach (DataRow oldRow in tblChiTiet.Rows)
                        {
                            DataRow newRow = newTbl.NewRow();
                            foreach (DataColumn col in newTbl.Columns)
                            {
                                if (tblChiTiet.Columns.Contains(col.ColumnName))
                                    newRow[col.ColumnName] = oldRow[col.ColumnName];
                            }
                            newTbl.Rows.Add(newRow);
                        }

                        tblChiTiet = newTbl;
                    }

                    gridControl1.MainView = null;
                    gridControl1.ViewCollection.Clear();
                    gridControl1.MainView = CreateBandSize(tblChiTiet);

                    BandedGridView mainView = (BandedGridView)gridControl1.MainView;
                    //mainView.CustomUnboundColumnData += mainView_CustomUnboundColumnData;
                    mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                    mainView.CellValueChanging += mainView_CellValueChanging;
                    mainView.CustomDrawCell += MainView_CustomDrawCell;

                    gridControl1.DataSource = tblChiTiet;
                    gridControl1.RefreshDataSource();
                }
            }
        }

        private void bandedGridView1_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            if (isEdit)
            {
                lstRowUpdate.Add(e.RowHandle);
            }
        }



        private void bandedGridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.IsGetData)
            {
                int sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName.Contains("@Size@") && col.UnboundType == UnboundColumnType.Bound || col.FieldName.Contains("@SIZE@") && col.UnboundType == UnboundColumnType.Bound)
                    {
                        try
                        {
                            if (row.Table.Columns.Contains(col.FieldName) && row[col.FieldName] != null && !string.IsNullOrEmpty(row[col.FieldName].ToString()))
                            {
                                int value;
                                bool isValidNumber = Int32.TryParse(row[col.FieldName].ToString(), out value);

                                if (!isValidNumber)
                                {
                                    value = 0;
                                }
                                else
                                {
                                    Convert.ToInt32(row[col.FieldName]);
                                }
                                //int value = Convert.ToInt32(row[col.FieldName]);
                                sum += value;
                            }
                            isEmpty = false;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
                if (!isEmpty)
                    e.Value = sum;
            }
        }


        public DataTable ChangeTenQGToMaQGNew(DataTable tbl)
        {
            try
            {
                DataTable tblChange = tbl.Copy();
                // lấy danh sách bảng QG
                string urlQG = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlQG); }).Result;
                List<QuocGiaEntity> _lstQG = JsonConvert.DeserializeObject<List<QuocGiaEntity>>(json);
                string columnNameQG = "MAQG";

                // Cập nhật field Màu PO
                for (int i = 0; i < tblChange.Rows.Count; i++)
                {
                    foreach (DataColumn dataColumn in tblChange.Columns)
                    {
                        if (RemoveDiacritics(dataColumn.ColumnName).Replace(" ", "").ToUpper().Equals(columnNameQG))
                        {
                            //Console.WriteLine("ChangedTenMauToMaMau");
                            if (tblChange.Rows[i][dataColumn] != null)
                            {
                                string tenQG = tblChange.Rows[i][dataColumn].ToString();
                                // Lấy ra mã màu từ _lstBangMau bằng field TenMau và MaHang
                                QuocGiaEntity QGEntity = _lstQG.Where(x => x.TenQG.ToString().ToUpper().Trim() == tenQG.ToString().ToUpper().Trim()).FirstOrDefault();

                                if (QGEntity != null)
                                {
                                    // Đổi DataTable
                                    tblChange.Rows[i][dataColumn] = QGEntity.MaQG;
                                }
                                else
                                {
                                    Console.WriteLine("Import QG không thành công => Nên không lấy được mã QG sau khi import");
                                }
                            }
                        }
                    }
                }

                //Console.WriteLine("Change PO complete");
                return tblChange;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }

        }

        private DataTable MapData(DataTable _dtTable, DataTable tblChanged)
        {
            // Tạo một DataTable tạm từ tblChanged. Chỉ giữ lại các cột số lượng size, xóa các cột còn lại
            // Dùng để map số lượng theo từng size cho bảng _dtTable
            // Sau khi xóa số lượng cột của _tblTempSLSize sẽ bằng với số lượng cột size của _dtTable
            string json = string.Empty;
            DataTable _tblTempSLSize = tblChanged.Copy();
            if (checkEditTV.Checked == false)
            {
                string urlMau = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
            }
            else
            {
                string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
            }

            DataTable tblmau = JsonConvert.DeserializeObject<DataTable>(json);
            //for (int i = 0; i < _tblTempSLSize.Columns.Count;)
            //{
            //    DataColumn column = _tblTempSLSize.Columns[i];
            //    List<string> _lstTemp = column.ColumnName.Split(new char[] { ' ' }).ToList();
            //    if (!_lstTemp[0].ToUpper().Equals("SIZE"))
            //    {
            //        _tblTempSLSize.Columns.RemoveAt(i);
            //    }
            //    else
            //    {
            //        i++;
            //    }
            //}
            // Xóa 3 cột đầu tiên
            if ((bool)checkEdit1.Checked == true)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (_tblTempSLSize.Columns.Count > 0)
                    {
                        _tblTempSLSize.Columns.RemoveAt(0);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (_tblTempSLSize.Columns.Count > 0)
                    {
                        _tblTempSLSize.Columns.RemoveAt(0);
                    }
                }
            }


            // Xóa cột cuối cùng nếu có tên là "SỐ LƯỢNG"
            if (_tblTempSLSize.Columns.Count > 0)
            {
                DataColumn lastColumn = _tblTempSLSize.Columns[_tblTempSLSize.Columns.Count - 1];
                if (RemoveDiacritics(lastColumn.ColumnName.ToUpper()).Replace(" ", "").Trim() == "SOLUONG")
                {
                    _tblTempSLSize.Columns.Remove(lastColumn);
                }
            }

            // POID, PO, MaMau, DauSize, MaQG, NgayGH, GhiChu
            foreach (DataRow row in tblChanged.Rows)
            {
                if (row[0].ToString() == "***")
                {
                    break;
                }
                var rowmau = tblmau.AsEnumerable().FirstOrDefault(r => r["MaMau"].ToString() == row[1].ToString().ToUpper().Trim());
                DataRow _rowAdd = _dtTable.NewRow();
                //_rowAdd["POID"] = txtMaDH.Text.ToString() + '|' + row[0].ToString();
                _rowAdd["PO"] = "KIEMKE";
                _rowAdd["MaMau"] = row[1];
                _rowAdd["DauSize"] = row[2].ToString();
                _rowAdd["MaQG"] = SearchLookUpEditQG.EditValue;
                //_rowAdd["NgayGH"] = DateTime.Now;
                //_rowAdd["GhiChu"] = txtGhiChu.Text;
                if (rowmau != null && rowmau["CodeMau"] != DBNull.Value)
                {
                    _rowAdd["ColorCode"] = rowmau["CodeMau"];
                }
                else
                {
                    _rowAdd["ColorCode"] = ""; // Giá trị mặc định nếu có lỗi
                }
                //_rowAdd["ColorCode"] = "";
                _dtTable.Rows.Add(_rowAdd);

            }


            // Map danh sách số lượng của từng size
            int index = 0;
            foreach (DataColumn column in _dtTable.Columns)
            {
                if (column.ColumnName.Contains("@Size@"))
                {
                    for (int i = 0; i < _dtTable.Rows.Count; i++)
                    {
                        _dtTable.Rows[i][column] = _tblTempSLSize.Rows[i][index];
                    }
                    index += 1;
                }
            }

            if (_grid != null)
            {
                foreach (DataRow dtRow in _dtTable.Rows)
                {
                    //string poid = dtRow["POID"].ToString();
                    string po = dtRow["PO"].ToString();
                    string mamau = dtRow["MaMau"].ToString();
                    string dausize = dtRow["DauSize"].ToString();
                    DataRow[] matchingRows = _grid.Select($"PO ='{po}'  AND MaMau ='{mamau}' AND DauSize = '{dausize}'");

                    if (matchingRows.Length > 0)
                    {
                        // Cập nhật dữ liệu của dòng trùng lặp
                        DataRow gridRow = matchingRows[0];
                        dtRow.BeginEdit();

                        foreach (DataColumn column in _grid.Columns)
                        {
                            if (dtRow.Table.Columns.Contains(column.ColumnName))
                            {
                                // Ghi đè dữ liệu từ gridRow sang dtRow
                                dtRow[column.ColumnName] = gridRow[column.ColumnName];
                            }
                            //dtRow[column.ColumnName] = gridRow[column.ColumnName];
                        }
                        dtRow.EndEdit();
                    }
                }
            }
            return _dtTable;
        }


        private void CreateSearchLookupHT()
        {
            string url = string.Format("{0}?", URL + "HinhThuc/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbht = JsonConvert.DeserializeObject<DataTable>(json);
            tbht.Columns.Add("Chon", typeof(bool));
            searchlookupEditHT.Properties.DataSource = tbht;
            searchlookupEditHT.Properties.ValueMember = "MaHT";
            searchlookupEditHT.Properties.DisplayMember = "TenHT";
            if (tbht != null && tbht.Rows.Count > 0)
            {
                searchlookupEditHT.EditValue = tbht.Rows[0]["MaHT"];
            }
        }

        private void CreateSearchLookupCL()
        {
            //string urlCL = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetChungLoai", searchLookUpEditMH.EditValue == null ? "" :searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
            //string jsonCL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCL); }).Result;
            //DataTable tblCL = JsonConvert.DeserializeObject<DataTable>(jsonCL);
            string urlCLL = $"{URL}ChungLoai/GetChungLoai?";
            string jsonCLL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCLL); }).Result;
            DataTable tblcll = JsonConvert.DeserializeObject<DataTable>(jsonCLL);
            searchLookUpEditCL.Properties.DataSource = tblcll;
            //searchLookUpEditCL.Properties.DataSource = tblCL;
            searchLookUpEditCL.Properties.ValueMember = "MaCL";
            searchLookUpEditCL.Properties.DisplayMember = "TenCL";

            if (tblcll != null && tblcll.Rows.Count > 0)
            {
                string urlHH = string.Format("{0}?makh={1}", URL + "DonHangTong/GetHangHoaKH", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
                string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
                DataTable tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
                var rowcl = tblHH.AsEnumerable().FirstOrDefault(x => x["MaHang"].ToString() == searchLookUpEditMH.EditValue.ToString());
                if (rowcl != null)
                {
                    searchLookUpEditCL.EditValue = rowcl["MaCL"].ToString();
                }

            }
        }

        private DataTable CreateDatatableTheSize()
        {
            DataTable tbl = new DataTable("dtSizethe");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaSize", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("CodeSize", typeof(string));
            tbl.Columns.Add("TenSize", typeof(string));
            tbl.Columns.Add("SizeSanXuat", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("XacNhan", typeof(string));
            tbl.Columns.Add("SizeXacNhan", typeof(int));
            tbl.Columns.Add("MaNhomSize", typeof(string));
            tbl.Columns.Add("NhomSize", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("Sort", typeof(string));
            tbl.Columns.Add("MaTheSize", typeof(string));
            tbl.Columns.Add("MaTheInSeam", typeof(string));
            return tbl;
        }

        private DataTable CreateDatatableTheMau()
        {
            DataTable tbl = new DataTable("dtMauthe");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("CodeMau", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaTheMau", typeof(string));
            return tbl;
        }

        private void Xoa_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataTable tbl = gridControl1.DataSource as DataTable;
            BandedGridView view = gridControl1.MainView as BandedGridView;
            int foucsedDelete = view.FocusedRowHandle;
            if (foucsedDelete >= 0)
            {
                DataRow row = view.GetFocusedDataRow();
                if (row != null)
                {
                    //string urlKHDTTonKho = string.Format("{0}", URL + "DonHangTonKho/GetKHDTTonKho");
                    //string jsonKHDTTonKHo = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKHDTTonKho); }).Result;
                    //tblKHDTTonKho = JsonConvert.DeserializeObject<DataTable>(jsonKHDTTonKHo);

                    //DataRow rowFilter = tblKHDTTonKho.AsEnumerable().Where(
                    //x => x["MaDH"].Equals(row["MaDH"])
                    //&& x["MaHang"].Equals(searchLookUpEditMH.EditValue.ToString())).FirstOrDefault();
                    ////
                    //if (rowFilter != null)
                    //{
                    //    XtraMessageBox.Show("Đơn hàng này đã được lập kế hoạch đóng thùng. Vui lòng không xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    return;
                    //}

                    string _po = row["PO"].ToString();
                    string _maMau = row["MaMau"].ToString();
                    string _dauSizeID = row["DauSize"].ToString();
                    string madh = row["madh"].ToString();

                    string urlKHDTTonKho = string.Format("{0}?para={1}&para1={2}&para2={3}", URL + "DonHangTonKho/GetDongThung", madh, _dauSizeID, _maMau);
                    string jsonKHDTTonKHo = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKHDTTonKho); }).Result;
                    tblKHDTTonKho = JsonConvert.DeserializeObject<DataTable>(jsonKHDTTonKHo);
                    if (tblKHDTTonKho != null && tblKHDTTonKho.Rows.Count > 0)
                    {
                        XtraMessageBox.Show("Đã Được lập PKL. Vui lòng không xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    DialogResult result = MessageBox.Show("Bạn có muốn xóa dòng này ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        tbl.Rows.RemoveAt(foucsedDelete);
                        string url = string.Format("{0}?para={1}&para1={2}&para2={3}&para3={4}", URL + "DonHangTonKho/DeleteDongDHTonKho", madh, _maMau, _dauSizeID, _po);
                        string resultDelete = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (resultDelete.ToLower() == "true")
                        {
                            Console.WriteLine("delete_true");
                        }
                        //LoadDSHangHoa();
                        else XtraMessageBox.Show(resultDelete);
                    }

                }
            }
        }
    }
}
