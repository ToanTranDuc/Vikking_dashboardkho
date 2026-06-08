using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout.Utils;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Libs;
using NtbSoft.ERP.Win.Modules.ResourceForm;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmPhieuGiaoNhan : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        string _minDateValue = "01/01/1753";
        int rowHandle = 0;
        string jsonDataPGN = string.Empty;

        const string _filterTime = "THOIGIAN";
        const string _filterDH = "DONHANG";
        string _strDaXacNhan = "DAXACNHAN";
        string _strChuaXacNhan = "CHUAXACNHAN";
        KeyDownControlHandler keyDownControlHandler;
        ActionControl actionControlRefresh;
        ActionControl actionControlPrintExcelSelect;
        ActionControl actionControlPrintExcel;
        ActionControl actionControlSave;
        List<ActionControl> lstActionControls;
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();

        List<int> _lstRowSave;

        bool isInit;
        public frmPhieuGiaoNhan()
        {
            InitializeComponent();
            this.bandedGridViewPhieuGiaoNhan.OptionsView.ShowColumnHeaders = false;
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _lstRowSave = new List<int>();
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            isInit = true;

            UpdateView(RemoveVietnameseDiacritics(cbxFilter.EditValue.ToString()));
            //LoadDataInit();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            dateTuNgay.EditValue = DateTime.Now.Date;
            dateDenNgay.EditValue = DateTime.Now.Date;
            CheckPermission();
            CreateSearchLookup();
            MapDataCBXFilter();
        }

        private void CheckPermission()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity SysUserModule = (from m in List
                                                    where m.FormShow == this.Name
                                                    select m).FirstOrDefault();
            // Nút xác nhận
            layoutControlItem2.Visibility = SysUserModule.AllowAdd ? LayoutVisibility.Always : LayoutVisibility.Never;
            // Nút hủy xác nhận
            layoutControlItem4.Visibility = SysUserModule.AllowEdit ? LayoutVisibility.Always : LayoutVisibility.Never;
            // Check permission cho thay đổi cột Ngày Nhập Kho và nút Lưu
            gridColNgayNhapKho.OptionsColumn.AllowEdit = SysUserModule.AllowEdit;
            layoutControlItem4.Visibility = SysUserModule.AllowEdit ? LayoutVisibility.Always : LayoutVisibility.Never;
            this.barButtonItem1.Visibility = SysUserModule.AllowEdit ? BarItemVisibility.Always : BarItemVisibility.Never;
        }

        private List<ActionControl> InitActionKeyDown()
        {
            actionControlRefresh = new ActionControl(NapLai, true, ActionType.Refresh, true);
            actionControlPrintExcel = new ActionControl(XuatExcelPGN, true, ActionType.ExCel, true);
            actionControlPrintExcelSelect = new ActionControl(XuatExcelPGNDongChon, true, ActionType.ExcelSelect, true);
            actionControlSave = new ActionControl(Luu, true, ActionType.Save, true);

            lstActionControls = new List<ActionControl> {
                actionControlRefresh, actionControlPrintExcel,actionControlPrintExcelSelect,actionControlSave};
            return lstActionControls;
        }

        private void splitContainerControl1_SizeChanged(object sender, EventArgs e)
        {
            //splitContainerControl1.SizeChanged -= splitContainerControl1_SizeChanged;
            splitContainerControl1.SplitterPosition = splitContainerControl1.Height / 2;
        }

        private void CreateSearchLookup()
        {
            try
            {
                string url = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblDVSX = JsonConvert.DeserializeObject<DataTable>(json);
                repositoryItemSearchLookUpEdit1.DataSource = tblDVSX;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadData(bool isLoading = false)
        {
            isInit = false;
            if (isLoading)
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                }
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang lấy dữ liệu...");
            }
            try
            {
                if (dateTuNgay.EditValue != null && !string.IsNullOrEmpty(dateTuNgay.EditValue.ToString())
                    && dateDenNgay.EditValue != null && !string.IsNullOrEmpty(dateDenNgay.EditValue.ToString()))
                {
                    DateTime tuNgay = DateTime.Parse(dateTuNgay.EditValue.ToString());
                    DateTime denNgay = DateTime.Parse(dateDenNgay.EditValue.ToString());
                    string urlPGN = string.Format("{0}?tuNgay={1}&&denNgay={2}", URL + "PhieuGiaoNhan/GetPGN", tuNgay.ToString("yyyy-MM-dd"), denNgay.ToString("yyyy-MM-dd"));
                    jsonDataPGN = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlPGN); }).Result;
                    DataTable tblPGN = JsonConvert.DeserializeObject<DataTable>(jsonDataPGN);
                    gridControlPhieuGiaoNhan.DataSource = tblPGN;
                    //Task.Run(async () => {
                    //    await FilterMaDH();
                    //});
                    MapDataCBXDH(this.bandedGridViewPhieuGiaoNhan);
                    FilterMaDH();
                    MapDataCBXPhieuXH(this.bandedGridViewPhieuGiaoNhan);
                    FilterPhieuXH();
                }
                else
                {
                    // Vui lòng chọn từ ngày đến ngày
                    XtraMessageBox.Show("Vui lòng chọn từ ngày đến ngày.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }

        private void MapDataCBXDH(GridView view)
        {
            Console.WriteLine("Test_MapDataCBXDH");
            if (view.DataSource != null)
            {
                DataTable _tblPGN = (view.DataSource as DataView).Table;
                DataTable _tblFilterDH = CreateDataTableFilterDH();
                foreach (DataRow row in _tblPGN.Rows)
                {
                    var _maDH = row[this.gridColMaDH.FieldName];
                    var _maHang = row[this.gridColMaHang.FieldName];
                    if (_maDH != null && !string.IsNullOrEmpty(_maDH.ToString())
                        && _maHang != null && !string.IsNullOrEmpty(_maHang.ToString()))
                    {
                        DataRow _rowTblPGN = _tblFilterDH.NewRow();
                        _rowTblPGN["MaDH"] = _maDH.ToString();
                        _rowTblPGN["MaHang"] = _maHang.ToString();
                        if (CheckRowAlReady(_tblFilterDH, "MaDH", _maDH.ToString()))
                        {
                            _tblFilterDH.Rows.Add(_rowTblPGN);
                        }
                    }
                }
                searchLookUpFilterDH.Properties.DataSource = _tblFilterDH;
            }
            else
            {
                searchLookUpFilterDH.Properties.DataSource = null;
            }

        }

        private bool CheckRowAlReady(DataTable tbl, string fieldName, string value)
        {
            foreach (DataRow row in tbl.Rows)
            {
                if (row[fieldName].Equals(value))
                {
                    return false;
                }
            }
            return true;
        }

        private DataTable CreateDataTableFilterDH()
        {
            DataTable _tbl = new DataTable();
            _tbl.Columns.Add("MaDH", typeof(string));
            _tbl.Columns.Add("MaHang", typeof(string));
            return _tbl;
        }

        private DataTable CreateDataTableFilterPXH()
        {
            DataTable _tbl = new DataTable();
            _tbl.Columns.Add("PhieuXH", typeof(string));
            return _tbl;
        }

        private void MapDataCBXPhieuXH(GridView view)
        {
            Console.WriteLine("Test_MapDataCBXPhieuXH");
            if (view.DataSource != null)
            {
                DataTable _tblPGN = (view.DataSource as DataView).Table;
                DataTable _tblFilterPXH = CreateDataTableFilterPXH();
                foreach (DataRow row in _tblPGN.Rows)
                {
                    // Thêm phiếu XH vào Filter
                    var _phieuXH = row[this.gridColPhieuXH.FieldName];
                    if (_phieuXH != null && !string.IsNullOrEmpty(_phieuXH.ToString().Trim()))
                    {
                        DataRow _rowTblPXH = _tblFilterPXH.NewRow();
                        _rowTblPXH["PhieuXH"] = _phieuXH;
                        if (CheckRowAlReady(_tblFilterPXH, "PhieuXH", _phieuXH.ToString()))
                        {
                            _tblFilterPXH.Rows.Add(_rowTblPXH);
                        }
                    }

                    // Thêm phiếu CK vào Filter
                    var _phieuCK = row[this.bandedGridColBienBanChuyenKho.FieldName];
                    if (_phieuCK != null && !string.IsNullOrEmpty(_phieuCK.ToString().Trim()))
                    {
                        DataRow _rowTblPXH = _tblFilterPXH.NewRow();
                        _rowTblPXH["PhieuXH"] = _phieuCK;
                        if (CheckRowAlReady(_tblFilterPXH, "PhieuXH", _phieuCK.ToString()))
                        {
                            _tblFilterPXH.Rows.Add(_rowTblPXH);
                        }
                    }
                }
                searchLookUpEditFilterPXH.Properties.DataSource = _tblFilterPXH;
            }
            else
            {
                searchLookUpEditFilterPXH.Properties.DataSource = null;
            }
        }

        private void gridViewPGN_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        public void LoadDataChiTiet(GridView viewPGN)
        {
            DataRow focusedRow;

            //if (view.IsGroupRow(view.FocusedRowHandle))
            //{
            //    int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
            //    _objMaDH = view.GetRowCellValue(childHandle, colNMaDH);
            //}
            //else
            //{
            //    _objMaDH = view.GetFocusedRowCellValue(colNMaDH);
            //}

            if (viewPGN.IsGroupRow(viewPGN.FocusedRowHandle))
            {
                int childHandle = viewPGN.GetChildRowHandle(viewPGN.FocusedRowHandle, 0);
                focusedRow = (viewPGN.GetRow(childHandle) as DataRowView).Row;
            }
            else
            {
                focusedRow = viewPGN.GetFocusedDataRow();
            }
            if (focusedRow != null)
            {
                // Tìm dòng đang focus của gridViewPhieuGiaoNhan do các dòng đang được group
                DataTable _focusedItemTblPGN = focusedRow.Table.Clone();
                _focusedItemTblPGN.ImportRow(focusedRow);

                _focusedItemTblPGN = DeleteColumn(_focusedItemTblPGN);

                if (_focusedItemTblPGN != null && _focusedItemTblPGN.Rows.Count > 0)
                {
                    string urlChiTietPGN = string.Format("{0}?", URL + "PhieuGiaoNhan/GetChiTietPGN");
                    string jsonChiTietPGN = Task.Run(async () => { return await _clientExtension.PostAsync(urlChiTietPGN, _focusedItemTblPGN); }).Result;
                    DataTable tblChiTietPGN = JsonConvert.DeserializeObject<DataTable>(jsonChiTietPGN);
                    bool isGopPO = tblChiTietPGN.AsEnumerable().Where(x => bool.Parse(x["IsGop"].ToString()) == true).FirstOrDefault() != null;
                    bandedGridColPO.OptionsColumn.AllowMerge = isGopPO? DevExpress.Utils.DefaultBoolean.False : DevExpress.Utils.DefaultBoolean.True;
                    gridControlChiTietGiaoNhan.DataSource = tblChiTietPGN;
                    createColBandsSizeSLTonKho(tblChiTietPGN);
                }
            }
        }

        public DataTable LoadDataChiTietTheoRow(DataRow focusedRow)
        {
            //DataRow focusedRow;
            //if (viewPGN.IsGroupRow(viewPGN.FocusedRowHandle))
            //{
            //    int childHandle = viewPGN.GetChildRowHandle(viewPGN.FocusedRowHandle, 0);
            //    focusedRow = (viewPGN.GetRow(childHandle) as DataRowView).Row;
            //}
            //else
            //{
            //    focusedRow = viewPGN.GetFocusedDataRow();
            //}
            if (focusedRow != null)
            {
                // Tìm dòng đang focus của gridViewPhieuGiaoNhan do các dòng đang được group
                DataTable _focusedItemTblPGN = focusedRow.Table.Clone();
                _focusedItemTblPGN.ImportRow(focusedRow);

                _focusedItemTblPGN = DeleteColumn(_focusedItemTblPGN);

                if (_focusedItemTblPGN != null && _focusedItemTblPGN.Rows.Count > 0)
                {
                    string urlChiTietPGN = string.Format("{0}?", URL + "PhieuGiaoNhan/GetChiTietPGNExcel");
                    string jsonChiTietPGN = Task.Run(async () => { return await _clientExtension.PostAsync(urlChiTietPGN, _focusedItemTblPGN); }).Result;
                    //DataTable tblChiTietPGN = JsonConvert.DeserializeObject<DataTable>(jsonChiTietPGN);
                    DataSet tblChiTietPGN = JsonConvert.DeserializeObject<DataSet>(jsonChiTietPGN);
                    //gridControlChiTietGiaoNhan.DataSource = tblChiTietPGN;
                    //createColBandsSizeSLTonKho(tblChiTietPGN);
                    DataTable tblReportPGNNotCombine = MapDataNotNull(tblChiTietPGN.Tables[0]);
                    DataTable tblReportPGNCombine = MapDataNotNull(tblChiTietPGN.Tables[1]);

                    // Tìm các thùng trong tblReportPGNCombine không phải gộp màu
                    //foreach (DataRow rowCombine in tblReportPGNCombine.Rows)
                    for (int i = 0; i < tblReportPGNCombine.Rows.Count;)
                    {
                        DataRow rowCombine = tblReportPGNCombine.Rows[i];
                        // Tìm những thùng gộp nhưng không phải là gộp màu
                        if (ExtractUniqueSequence(rowCombine["TenMau"].ToString()) != null)
                        {
                            // Không phải gộp màu
                            if (tblReportPGNNotCombine != null && tblReportPGNNotCombine.Rows.Count > 0)
                            {
                                // Nếu không phải gộp màu -> Thêm row này vào tblReportPGNNotCombine
                                // Tìm trong tblReportPGNNotCombine có row nào trùng Ngay_XN; POID; TenMau ? true -> Thì cộng dồn số lượng vào row đó và SoThung+=1
                                // else -> Thêm một row mới
                                DataRow rowDefault = tblReportPGNNotCombine.AsEnumerable().Where(
                                    x => x["Ngay_XN"].Equals(rowCombine["Ngay_XN"])
                                    //&& x["POID"].Equals(rowCombine["POID"])
                                    && x["TenMau"].Equals(rowCombine["TenMau"])
                                    ).FirstOrDefault();
                                if (rowDefault != null)
                                {
                                    foreach (DataColumn column in tblReportPGNNotCombine.Columns)
                                    {
                                        if (column.ColumnName.Contains("Size@") && rowCombine.Table.Columns.Contains(column.ColumnName))
                                        {
                                            rowDefault[column] = int.Parse(rowDefault[column].ToString()) + int.Parse(rowCombine[column.ColumnName].ToString());
                                        }
                                        else if (column.ColumnName.Equals("SoThung"))
                                        {
                                            rowDefault[column] = int.Parse(rowDefault[column].ToString()) + 1;
                                        }
                                    }
                                    tblReportPGNCombine.Rows.RemoveAt(i);
                                }
                                else
                                {
                                    // Thêm 1 row mới vào tblReportPGNNotCombine
                                    DataRow rowAdd = tblReportPGNNotCombine.NewRow();
                                    // rowAdd[""]
                                    // rowAdd.ItemArray = rowCombine.ItemArray;
                                    foreach (DataColumn column in tblReportPGNNotCombine.Columns)
                                    {
                                        if (rowCombine.Table.Columns.Contains(column.ColumnName))
                                        {
                                            rowAdd[column] = rowCombine[column.ColumnName];
                                        }
                                    }
                                    rowAdd["SoThung"] = 1;
                                    tblReportPGNNotCombine.Rows.Add(rowAdd);
                                    tblReportPGNCombine.Rows.RemoveAt(i);
                                }
                                Console.WriteLine("row_default");
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            i += 1;
                        }
                    }

                    return CombineDataExecl(tblReportPGNNotCombine, tblReportPGNCombine);
                }
            }
            return new DataTable();
        }

        private DataTable DeleteColumn(DataTable tbl)
        {
            List<string> lstNameColumn = new List<string> { "NgayNhapKho","NgayNhapKho_Update","Ngay_XN","NVien_XN","MaPKL","MaDH"
                ,"MaPKL_XH","Cont_XH","Cont_CK","MaDVSX","MaLenh","MaHang","POID","PO","SoLuongThung","SoLuongSP"};
            for (int i = 0; i < tbl.Columns.Count;)
            {
                DataColumn column = tbl.Columns[i];
                if (!lstNameColumn.Contains(column.ColumnName))
                {
                    tbl.Columns.RemoveAt(i);
                }
                else
                {
                    i += 1;
                }
            }
            return tbl;
        }

        List<string> lstSize = new List<string>();
        private BandedGridView createColBandsSizeSLTonKho(DataTable _thongTinPGN)
        {
            BandedGridView bandedGridview = new BandedGridView();
            bandedGridview = bandedGridViewChiTiet;

            if (lstSize != null && lstSize.Count > 0)
            {
                lstSize.Clear();
            }
            gridBandSize.Children.Clear();

            foreach (DataColumn dc in _thongTinPGN.Columns)
            {
                if (dc.ColumnName.Contains("Size@"))
                {
                    string colName = dc.ColumnName;
                    lstSize.Add(colName.Replace("Size@", ""));

                    BandedGridColumn colSize = new BandedGridColumn();
                    colSize.AppearanceHeader.Options.UseTextOptions = true;
                    colSize.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    colSize.AppearanceCell.Options.UseTextOptions = true;
                    colSize.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    colSize.FieldName = colName;
                    colSize.Name = colName;
                    colSize.OptionsColumn.AllowEdit = false;
                    colSize.Visible = true;
                    colSize.Width = 60;
                    colSize.Caption = colName;
                    bandedGridview.Columns.AddRange(new BandedGridColumn[] { colSize });
                    colSize.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    colSize.DisplayFormat.FormatString = "{0:##,0}";
                    colSize.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

                    GridBand gb = new GridBand();
                    gb.Caption = colName.Replace("Size@", "");
                    gb.Name = "gb";
                    gb.VisibleIndex = 2;
                    gb.Width = 75;

                    gb.Columns.Add(colSize);
                    gridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }

            return bandedGridview;
            return new BandedGridView();
        }

        private void dateTuNgay_EditValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("tu_ngay");
            var newValue = (e as DevExpress.XtraEditors.Controls.ChangingEventArgs).NewValue;
            if (newValue != null && !string.IsNullOrEmpty(newValue.ToString()))
            {
                //tuNgay = DateTime.Parse(newValue.ToString()).Date;
                //LoadData(tuNgay, denNgay);
                dateDenNgay.EditValue = "";
            }
        }

        private void dateDenNgay_EditValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("den_ngay");
            var newValue = (e as DevExpress.XtraEditors.Controls.ChangingEventArgs).NewValue;
            if (newValue != null && !string.IsNullOrEmpty(newValue.ToString()))
            {
                //denNgay = DateTime.Parse(newValue.ToString()).Date;
                this.checkEdit1.EditValue = false;
                searchLookUpFilterDH.EditValue = null;
                searchLookUpEditFilterPXH.EditValue = null;
                LoadData(!isInit);
            }
        }

        private void gridViewPhieuGiaoNhan_RowClick(object sender, RowClickEventArgs e)
        {
            //String tenKH = 
            DataRow focusedRow;
            if (bandedGridViewPhieuGiaoNhan.IsGroupRow(bandedGridViewPhieuGiaoNhan.FocusedRowHandle))
            {
                int childHandle = bandedGridViewPhieuGiaoNhan.GetChildRowHandle(bandedGridViewPhieuGiaoNhan.FocusedRowHandle, 0);
                focusedRow = (bandedGridViewPhieuGiaoNhan.GetRow(childHandle) as DataRowView).Row;
            }
            else
            {
                focusedRow = bandedGridViewPhieuGiaoNhan.GetFocusedDataRow();
            }
            LoadDataChiTiet(this.bandedGridViewPhieuGiaoNhan);
        }

        private void bandedGridViewPhieuGiaoNhan_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            //if (gridViewThongTinDonHang.RowCount > 0)
            //{
            if (e.Menu == null)
                return;
            e.Menu.Items.Clear();

            if (e.HitInfo.InRow && layoutControlItem4.Visibility.Equals(LayoutVisibility.Always))
            {

                DevExpress.Utils.Menu.DXMenuItem menuHuyXacNhan = new DevExpress.Utils.Menu.DXMenuItem("Hủy xác nhận", HuyXacNhanItem);
                menuHuyXacNhan.Appearance.Font = new Font("Tahoma", 8.0F, System.Drawing.FontStyle.Bold);
                e.Menu.Items.Add(menuHuyXacNhan);
            }
            //}
        }

        private void HuyXacNhanItem(object sender, EventArgs e)
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn hủy xác nhận không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    // DataTable _tblXacNhan = (this.bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table;
                    if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    {
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    }
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang hủy xác nhận...");

                    DataRow row = bandedGridViewPhieuGiaoNhan.GetFocusedDataRow();
                    Console.WriteLine("btn_xac_nhan");
                    bool _isXuathang = (bool)row["IsXuatHang"];
                    bool _isChuyenKho = (bool)row["IsChuyenKho"];
                    int _isXacNhan = int.Parse(row["IsXacNhan"] != null ? (RemoveVietnameseDiacritics(row["IsXacNhan"].ToString()).ToUpper().Equals(_strDaXacNhan) ? "1" : "0") : "0");
                    if (_isXuathang)
                    {
                        XtraMessageBox.Show("Phiếu này đã Xuất Hàng. Không thể hủy xác nhận.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (_isChuyenKho)
                    {
                        XtraMessageBox.Show("Phiếu này đã Chuyển Kho. Không thể hủy xác nhận.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (_isXacNhan.Equals(0))
                    {
                        XtraMessageBox.Show("Phiếu này chưa được Xác Nhận.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    // Hủy xác nhận
                    DataTable _tblHuyXacNhan = row.Table.Clone();

                    DataRow _rowHuyXacNhan = _tblHuyXacNhan.NewRow();
                    _rowHuyXacNhan = row;
                    _tblHuyXacNhan.ImportRow(_rowHuyXacNhan);
                    _tblHuyXacNhan = DeleteColumn(_tblHuyXacNhan);
                    string urlChiTietPGN = string.Format("{0}?nVien={1}", URL + "PhieuGiaoNhan/HuyIsXacNhan", GlobleData.UserName);
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlChiTietPGN, _tblHuyXacNhan); }).Result;
                    // Load lại data
                    // LoadData();
                    UpdateUI();

                    if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bandedGridViewChiTiet_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (e.Band == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Band.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void gridViewPhieuGiaoNhan_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            Console.WriteLine("custom_display_column_text");
            if (e.Column.Equals(this.bandedGridColNgayXuatHang) && e.ListSourceRowIndex >= 0
               && e.Value != null && !string.IsNullOrEmpty(e.Value.ToString()))
            {
                Console.WriteLine("bandedGridColNgayXuatHang");
                DateTime _dateTime = (DateTime)e.Value;
                if (_dateTime.Equals(new DateTime(2500, 1, 1)))
                {
                    e.DisplayText = "";
                }
            }
        }

        private void gridViewPhieuGiaoNhan_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            Console.WriteLine("custom_draw_cell");
            if (e.Column.Equals(this.gridColTrangThai) && e.RowHandle >= 0
                && e.CellValue != null && !string.IsNullOrEmpty(e.CellValue.ToString()))
            {
                GridCellInfo cellInfo = e.Cell as GridCellInfo;

                if (RemoveVietnameseDiacritics(e.CellValue.ToString()).ToUpper().Equals(_strDaXacNhan))
                {
                    cellInfo.Appearance.ForeColor = Color.Green;
                }
                else
                {
                    cellInfo.Appearance.ForeColor = Color.Red;
                }
            }
        }

        private void cbxDH_EditValueChanged(object sender, EventArgs e)
        {
            //Console.WriteLine("cbxDH_EditValueChanged");
            FilterMaDH();
            searchLookUpEditFilterPXH.EditValue = null;
            MapDataCBXPhieuXH(bandedGridViewPhieuGiaoNhan);
            this.ActiveControl = this.gridControlPhieuGiaoNhan;
        }

        // Hàm FilterMaDH() sẽ luôn được gọi sau khi LoadData()
        // Hàm FilterMaDH() được gọi sau khi chọn Đơn hàng trong combobox
        private void FilterMaDH(bool isLoadDetail = false)
        {
            try
            {
                DataTable _tblFilter = JsonConvert.DeserializeObject<DataTable>(jsonDataPGN);

                // Filter data theo mã đơn hàng
                if (this.searchLookUpFilterDH.EditValue != null && !string.IsNullOrEmpty(this.searchLookUpFilterDH.EditValue.ToString()))
                {
                    string _maDH = this.searchLookUpFilterDH.EditValue.ToString();
                    DataRow row = _tblFilter.AsEnumerable().FirstOrDefault(x => x["MaDH"].Equals(_maDH));
                    if (row != null)
                    {
                        _tblFilter = _tblFilter.AsEnumerable().Where(x => x["MaDH"].Equals(_maDH)).CopyToDataTable();
                    }
                    else
                    {
                        _tblFilter = null;
                    }
                    Console.WriteLine("Test_FilterMaDH");
                }

                this.gridControlPhieuGiaoNhan.DataSource = _tblFilter;
                if (!isLoadDetail)
                    return;
                // Get chi tiết
                if (_tblFilter != null && _tblFilter.Rows.Count > 0)
                {
                    LoadDataChiTiet(this.bandedGridViewPhieuGiaoNhan);
                }
                else
                {
                    gridControlChiTietGiaoNhan.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Hàm FilterPhieuXH() sẽ luôn được gọi sau khi LoadData()
        // Hàm FilterPhieuXH() được gọi sau khi chọn Phiếu xuất hàng trong combobox
        private void FilterPhieuXH()
        {
            try
            {
                if (bandedGridViewPhieuGiaoNhan.DataSource != null)
                {

                    DataTable _tblFilter = (bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table;
                    // Filter data theo mã đơn hàng
                    if (this.searchLookUpEditFilterPXH.EditValue != null && !string.IsNullOrEmpty(this.searchLookUpEditFilterPXH.EditValue.ToString()))
                    {
                        string _maPKL_XH = this.searchLookUpEditFilterPXH.EditValue.ToString();
                        DataRow row = _tblFilter.AsEnumerable().FirstOrDefault(x => x["BienBanXuatHang"].Equals(_maPKL_XH) || x["BienBanChuyenKho"].Equals(_maPKL_XH));
                        if (row != null)
                        {
                            _tblFilter = _tblFilter.AsEnumerable().Where(x => x["BienBanXuatHang"].Equals(_maPKL_XH) || x["BienBanChuyenKho"].Equals(_maPKL_XH)).CopyToDataTable();
                        }
                        else
                        {
                            _tblFilter = null;
                        }
                        Console.WriteLine("Test_FilterPhieuXH");
                    }

                    this.gridControlPhieuGiaoNhan.DataSource = _tblFilter;

                    // Get chi tiết
                    if (_tblFilter != null && _tblFilter.Rows.Count > 0)
                    {
                        LoadDataChiTiet(this.bandedGridViewPhieuGiaoNhan);
                    }
                    else
                    {
                        gridControlChiTietGiaoNhan.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bandedGridViewChiTiet_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            Console.WriteLine("bandedGridViewChiTiet_CustomColumnDisplayText");
            if (e.Column.FieldName.Contains("Size@") && e.ListSourceRowIndex >= 0)
            {
                if (e.Value is null || (!(e.Value is null) && string.IsNullOrEmpty(e.Value.ToString())))
                {
                    e.DisplayText = "-";
                }
            }
        }

        // ĐỔi type Ngay_XN sang DateTime
        private DataTable ConvertTypeNgay_XN(DataTable _tblXacNhan)
        {
            // Đổi type của cột Ngay_XN
            _tblXacNhan.Columns.Add("addcolumn", typeof(DateTime));

            // Sao chép data từ cột Ngay_XN -> cột addcolumn
            for (int i = 0; i < _tblXacNhan.Columns.Count;)
            {
                DataColumn column = _tblXacNhan.Columns[i];
                if (column.ColumnName.Equals("Ngay_XN"))
                {
                    foreach (DataRow row in _tblXacNhan.Rows)
                    {
                        row["addcolumn"] = row["Ngay_XN"];
                    }

                    // Xóa cột Ngay_XN
                    _tblXacNhan.Columns.Remove(column);

                    // Đổi tên cột addcolumn -> Ngay_XN
                }
                else
                {
                    i += 1;
                }
            }
            _tblXacNhan.Columns["addcolumn"].ColumnName = "Ngay_XN";
            return _tblXacNhan;
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xác nhận không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    {
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    }
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang xác nhận...");

                    DataTable _tblXacNhan = (this.bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table;
                    //_tblXacNhan = ConvertTypeNgay_XN(_tblXacNhan);
                    Console.WriteLine("btn_xac_nhan");
                    DataRow row = _tblXacNhan.AsEnumerable().FirstOrDefault(x => RemoveVietnameseDiacritics(x["IsXacNhan"].ToString()).ToUpper().Equals(_strChuaXacNhan));
                    if (row != null)
                    {
                        _tblXacNhan = _tblXacNhan.AsEnumerable().Where(x => RemoveVietnameseDiacritics(x["IsXacNhan"].ToString()).ToUpper().Equals(_strChuaXacNhan)).CopyToDataTable();

                        // Cập nhật dữ liệu xác nhận
                        foreach (DataRow _rowUpdate in _tblXacNhan.Rows)
                        {
                            foreach (DataColumn _columnUpdate in _tblXacNhan.Columns)
                            {
                                if (_columnUpdate.ColumnName.Equals("IsXacNhan"))
                                {
                                    _rowUpdate[_columnUpdate] = 1;
                                }
                                else if (_columnUpdate.ColumnName.Equals("NVien_XN"))
                                {
                                    _rowUpdate[_columnUpdate] = GlobleData.UserName;
                                }
                            }
                        }

                        _tblXacNhan = DeleteColumn(_tblXacNhan);

                        string urlChiTietPGN = string.Format("{0}?", URL + "PhieuGiaoNhan/UpdateIsXacNhan");
                        string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlChiTietPGN, _tblXacNhan); }).Result;
                        UpdateUI();
                        if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                        if (result.ToLower().Equals("true"))
                        {
                            clsWaitForm.ShowSuccessForm(this, 1500);
                        }
                        else
                        {
                            XtraMessageBox.Show(result);
                        }
                        // Load lại data

                        // LoadData();

                    }
                    else
                    {
                        XtraMessageBox.Show("Tất cả đơn hàng đã được xác nhận. Vui lòng chọn đơn hàng khác.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void NapLai()
        {
            // LoadData();
            UpdateUI();
        }

        private void UpdateUI()
        {
            this.checkEdit1.EditValue = false;
            string _cbxFilter = RemoveVietnameseDiacritics(cbxFilter.EditValue.ToString());
            switch (_cbxFilter.ToUpper())
            {
                case _filterTime:
                    LoadData();
                    break;
                case _filterDH:
                    if (searchLookUpEditMaDH.EditValue != null && !string.IsNullOrEmpty(searchLookUpEditMaDH.EditValue.ToString()))
                    {
                        GetPhieuGiaoNhanAllowDonHang(searchLookUpEditMaDH.EditValue.ToString());
                    }

                    break;

            }
        }

        private void btRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLai();
        }

        private void btnExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // GetReportPGNNotCombine();
            XuatExcelPGN();
        }

        private DataTable GetReportPGN(DataRow row)
        {
            DataTable tblItemReport = row.Table.Clone();
            DataRow rowItemReport = tblItemReport.NewRow();
            rowItemReport = row;
            tblItemReport.ImportRow(rowItemReport);
            tblItemReport = DeleteColumn(tblItemReport);

            //
            string urlReportPGN = string.Format("{0}?", URL + "PhieuGiaoNhan/GetReportPGN");
            string jsonReportPGN = Task.Run(async () => { return await _clientExtension.PostAsync(urlReportPGN, tblItemReport); }).Result;
            DataSet dtsReportPGN = JsonConvert.DeserializeObject<DataSet>(jsonReportPGN);
            DataTable tblReportPGNNotCombine = MapDataNotNull(dtsReportPGN.Tables[0]);
            DataTable tblReportPGNCombine = MapDataNotNull(dtsReportPGN.Tables[1]);

            // Tìm các thùng trong tblReportPGNCombine không phải gộp màu
            //foreach (DataRow rowCombine in tblReportPGNCombine.Rows)
            for (int i = 0; i < tblReportPGNCombine.Rows.Count;)
            {
                DataRow rowCombine = tblReportPGNCombine.Rows[i];
                // Tìm những thùng gộp nhưng không phải là gộp màu
                if (ExtractUniqueSequence(rowCombine["TenMau"].ToString()) != null)
                {
                    // Không phải gộp màu
                    if (tblReportPGNNotCombine != null && tblReportPGNNotCombine.Rows.Count > 0)
                    {
                        // Nếu không phải gộp màu -> Thêm row này vào tblReportPGNNotCombine
                        // Tìm trong tblReportPGNNotCombine có row nào trùng Ngay_XN; POID; TenMau ? true -> Thì cộng dồn số lượng vào row đó và SoThung+=1
                        // else -> Thêm một row mới
                        DataRow rowDefault = tblReportPGNNotCombine.AsEnumerable().Where(
                            x => x["Ngay_XN"].Equals(rowCombine["Ngay_XN"])
                            //&& x["POID"].Equals(rowCombine["POID"])
                            && x["TenMau"].Equals(rowCombine["TenMau"])
                            ).FirstOrDefault();
                        if (rowDefault != null)
                        {
                            foreach (DataColumn column in tblReportPGNNotCombine.Columns)
                            {
                                if (column.ColumnName.Contains("Size@") && rowCombine.Table.Columns.Contains(column.ColumnName))
                                {
                                    rowDefault[column] = int.Parse(rowDefault[column].ToString()) + int.Parse(rowCombine[column.ColumnName].ToString());
                                }
                                else if (column.ColumnName.Equals("SoThung"))
                                {
                                    rowDefault[column] = int.Parse(rowDefault[column].ToString()) + 1;
                                }
                            }
                            tblReportPGNCombine.Rows.RemoveAt(i);
                        }
                        else
                        {
                            // Thêm 1 row mới vào tblReportPGNNotCombine
                            DataRow rowAdd = tblReportPGNNotCombine.NewRow();
                            // rowAdd[""]
                            // rowAdd.ItemArray = rowCombine.ItemArray;
                            foreach (DataColumn column in tblReportPGNNotCombine.Columns)
                            {
                                if (rowCombine.Table.Columns.Contains(column.ColumnName))
                                {
                                    rowAdd[column] = rowCombine[column.ColumnName];
                                }
                            }
                            rowAdd["SoThung"] = 1;
                            tblReportPGNNotCombine.Rows.Add(rowAdd);
                            tblReportPGNCombine.Rows.RemoveAt(i);
                        }
                        Console.WriteLine("row_default");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    i += 1;
                }
            }

            return CombineDataExecl(tblReportPGNNotCombine, tblReportPGNCombine);
        }

        // Map giá trị 0 vào cho các cell của cột size có giá trị null
        private DataTable MapDataNotNull(DataTable tbl)
        {
            if (tbl != null && tbl.Rows.Count > 0)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    foreach (DataColumn column in tbl.Columns)
                    {
                        if (column.ColumnName.Contains("Size@"))
                        {
                            if (!(row[column] != null && !string.IsNullOrEmpty(row[column].ToString())))
                            {
                                row[column] = 0;
                            }
                        }
                        else if (column.ColumnName.Contains("TenMau"))
                        {
                            string uniqueSequence = ExtractUniqueSequence(row[column].ToString());

                            if (uniqueSequence != null)
                            {
                                row[column] = uniqueSequence;
                                //Console.WriteLine($"Chuỗi duy nhất: {uniqueSequence}");
                            }
                            else
                            {
                                //Console.WriteLine("Chuỗi không chứa các chuỗi con giống nhau");
                            }

                        }
                    }
                }

            }
            return tbl;
        }

        private string ExtractUniqueSequence(string s)
        {
            // Tách chuỗi bằng dấu gạch nối
            string[] parts = s.Split('-');

            // Kiểm tra nếu tất cả các phần đều giống nhau
            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i] != parts[0])
                {
                    return null;
                }
            }

            return parts[0];
        }

        private DataTable CombineDataExecl(DataTable tblNotCombine, DataTable tblCombine)
        {
            // Gộp 2 datatable
            if (tblNotCombine != null && tblNotCombine.Rows.Count > 0)
            {
                DataTable tblPGNExcel = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(tblNotCombine)).AsEnumerable()
          .OrderBy(row => row.Field<DateTime>("Ngay_XN")).CopyToDataTable();
                if (tblCombine != null && tblCombine.Rows.Count > 0)
                {
                    // Thêm cột số thùng cho tblCombine
                    tblCombine.Columns.Add("SoThung", typeof(int));
                    foreach (DataRow row in tblCombine.Rows)
                    {
                        row["SoThung"] = 1;
                    }

                    tblCombine = tblCombine.AsEnumerable()
                              .OrderBy(row => row.Field<DateTime>("Ngay_XN")).CopyToDataTable();
                    foreach (DataRow row in tblCombine.Rows)
                    {
                        DataRow rowPGNExcel = tblPGNExcel.NewRow();
                        foreach (DataColumn column in tblCombine.Columns)
                        {
                            rowPGNExcel[column.ColumnName] = row[column];
                        }
                        tblPGNExcel.Rows.Add(rowPGNExcel);
                    }
                    Console.WriteLine("complete_gop");
                }


                return tblPGNExcel;
            }
            else if (tblCombine != null && tblCombine.Rows.Count > 0)
            {
                tblCombine.Columns.Add("SoThung", typeof(int));
                foreach (DataRow row in tblCombine.Rows)
                {
                    row["SoThung"] = 1;
                }
                return JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(tblCombine)).AsEnumerable()
                    .OrderBy(row => row.Field<DateTime>("Ngay_XN")).CopyToDataTable();
            }
            return new DataTable();
        }
        private void XuatExcelPGN()
        {
            //List<DongThungBaoCaoEntity> lstExport = gridView1.DataSource as List<DongThungBaoCaoEntity>;
            //DataTable _dtExport = (bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table;
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("baocaophieugiaonhan{0}", DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "baocaophieugiaonhan.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                //gridView1.ExportToXlsx(Sfd.FileName);
                bool isExport = Export(TemplateFileName, ExportFileName);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                if (isExport)
                {
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
                }
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            //}
            //else
            //{
            //    XtraMessageBox.Show("Vui lòng lọc dữ liệu theo Mã DVSX, Mã Hàng và Ngày Lập Phiếu để xuất Excel.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            //    return;
            //}
        }

        // isAll la bien de check xuatExcel theo dong chon hay tat ca
        public bool Export(string TemplateFileName, string ExportFileName, bool isAll = true)
        {
            try
            {
                System.IO.FileInfo file = new System.IO.FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {
                        // đặt tên người tạo file
                        excelPackage.Workbook.Properties.Author = "Cty NTB";

                        // đặt tiêu đề cho file
                        excelPackage.Workbook.Properties.Title = "Bao-cao-dong-thung";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;

                        // Mở file Excel mẫu
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);

                        // Lấy bảng tính trong file Excel mẫu (Sheet1 là tên của bảng tính)
                        ExcelWorksheet worksheet = templatePackage.Workbook.Worksheets["CL-TT02-BM16"];

                        //// countRow là hàng bắt đầu thêm dữ liệu của gridview
                        int countRow = 9;

                        // Thêm các cột size, cột đầu tiên ở ô thứ 5
                        int columnOfSize = 4;

                        DataTable tblPhieuGN = (bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table;
                        if (tblPhieuGN != null && tblPhieuGN.Rows.Count > 0)
                        {
                            DataRow defaultRow = tblPhieuGN.AsEnumerable().Where(x => x["IsXacNhan"] != null
                            // && (x["IsXacNhan"] is int || x["IsXacNhan"] is long)
                            && RemoveVietnameseDiacritics(x["IsXacNhan"].ToString()).ToUpper().Equals(_strDaXacNhan)).FirstOrDefault();
                            if (defaultRow != null)
                            {
                                DataTable tblPhieuGNXacNhan = tblPhieuGN.AsEnumerable().Where(x => x["IsXacNhan"] != null
                                //&& (x["IsXacNhan"] is int || x["IsXacNhan"] is long)
                                && RemoveVietnameseDiacritics(x["IsXacNhan"].ToString()).ToUpper().Equals(_strDaXacNhan)).CopyToDataTable();

                                if (isAll)
                                {
                                    for (int i = 0; i < tblPhieuGNXacNhan.Rows.Count; i++)
                                    {
                                        // countRow là hàng bắt đầu thêm dữ liệu của gridview
                                        countRow = 9;


                                        DataRow row = tblPhieuGNXacNhan.Rows[i];

                                        DataTable _dtExport = new DataTable();
                                        _dtExport = GetReportPGN(row);


                                        string _maHang = string.Empty;
                                        string _tenKH = string.Empty;
                                        string _donHang = string.Empty;
                                        if (row.Table.Columns.Contains("MaHang")
                                            && row["MaHang"] != null)
                                        {
                                            _maHang = row["MaHang"].ToString();
                                        }

                                        if (row.Table.Columns.Contains("MaDH")
                                            && row["MaDH"] != null)
                                        {
                                            _donHang = row["MaDH"].ToString();
                                        }

                                        if (row.Table.Columns.Contains("TenKH")
                                            && row["TenKH"] != null)
                                        {
                                            _tenKH = row["TenKH"].ToString();
                                        }
                                        string _cbxFilter = RemoveVietnameseDiacritics(cbxFilter.EditValue.ToString());
                                        bool _isDate = _cbxFilter.ToUpper() == _filterTime;
                                        if (_isDate)
                                        {
                                            if (!(templatePackage.Workbook.Worksheets[_donHang] != null))
                                            {
                                                PrintDataExcel(worksheet, _dtExport, columnOfSize, countRow, _donHang, _tenKH, _maHang);
                                            }
                                        }
                                        else
                                        {
                                            if (!(templatePackage.Workbook.Worksheets[string.Format("{0} - {1}", _donHang, i + 1)] != null))
                                            {
                                                PrintDataExcel(worksheet, _dtExport, columnOfSize, countRow, string.Format("{0} - {1}", _donHang, i + 1), _tenKH, _maHang);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    DataTable dtPGNSelect = (bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table.AsEnumerable()
                                    .CopyToDataTable().AsEnumerable()
                                    .Where((x) => bool.Parse(x["MultiSelect"].ToString()) == true && RemoveVietnameseDiacritics(x["IsXacNhan"].ToString()).ToUpper().Equals(_strDaXacNhan)).CopyToDataTable();

                                    string _maHang = string.Empty;
                                    string _tenKH = string.Empty;
                                    string _donHang = string.Empty;
                                    string _po = string.Empty;


                                    DataTable _dtExport = new DataTable();
                                    int i = 0;
                                    foreach (DataRow row in dtPGNSelect.Rows)
                                    {
                                        i += 1;
                                        _dtExport = LoadDataChiTietTheoRow(row);
                                        if (row.Table.Columns.Contains("MaHang") && row["MaHang"] != null)
                                        {
                                            _maHang = row["MaHang"].ToString();
                                        }

                                        if (row.Table.Columns.Contains("MaDH")
                                            && row["MaDH"] != null)
                                        {
                                            _donHang = row["MaDH"].ToString();
                                        }

                                        if (row.Table.Columns.Contains("TenKH")
                                            && row["TenKH"] != null)
                                        {
                                            _tenKH = row["TenKH"].ToString();
                                        }
                                        if (row.Table.Columns.Contains("PO") && row["PO"] != null)
                                        {
                                            _po = row["PO"].ToString();
                                        }

                                        string _cbxFilter = RemoveVietnameseDiacritics(cbxFilter.EditValue.ToString());
                                        bool _isDate = _cbxFilter.ToUpper() == _filterTime;
                                        if (_isDate)
                                        {
                                            if (!(templatePackage.Workbook.Worksheets[string.Format("{0} - {1}", _donHang, i + 1)] != null))
                                            {
                                                PrintDataExcel(worksheet, _dtExport, columnOfSize, countRow, string.Format("{0} - {1}", _donHang, i), _tenKH, _maHang);
                                            }
                                        }
                                        else
                                        {
                                            if (!(templatePackage.Workbook.Worksheets[string.Format("{0} - {1} - {2}", _donHang, _po, i + 1)] != null))
                                            {
                                                PrintDataExcel(worksheet, _dtExport, columnOfSize, countRow, string.Format("{0} - {1} - {2}", _donHang, _po, i), _tenKH, _maHang);
                                            }
                                        }
                                    }
                                }
                            }

                            // Xóa sheet có tên là "Sheet2"
                            if (templatePackage.Workbook.Worksheets["CL-TT02-BM16"] != null)
                            {
                                templatePackage.Workbook.Worksheets.Delete("CL-TT02-BM16");
                            }
                        }
                        if (templatePackage.Workbook.Worksheets.Count > 0)
                        {
                            FileInfo resultFile = new FileInfo(resultFilePath);
                            templatePackage.SaveAs(resultFile);
                        }
                        else
                        {
                            XtraMessageBox.Show("Không có dữ liệu để xuất Excel vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                object misValue = System.Reflection.Missing.Value;
                throw new Exception(ex.Message);
                return false;
            }
        }

        private void PrintDataExcel(ExcelWorksheet worksheet, DataTable _dtExport, int columnOfSize, int countRow, String _nameSheet, String _tenKH, String _maHang)
        {
            bool flagDrawSize = true;
            string colName_Ngay_XN = "Ngay_XN";
            string colName_POID = "POID";
            string colName_PO = "PO";
            string colName_TenMau = "TenMau";
            string colName_SoThung = "SoThung";

            ExcelWorksheet ws = worksheet.Workbook.Worksheets.Add(_nameSheet ?? "", worksheet);
            int SumAllSLSP = 0;
            if (_dtExport != null && _dtExport.Rows.Count > 0)
            {
                foreach (DataRow rowExport in _dtExport.Rows)
                {

                    columnOfSize = 4;
                    int rowOfSize = 8;

                    // Để insert cột size
                    if (flagDrawSize)
                    {
                        flagDrawSize = false;

                        foreach (DataColumn column in _dtExport.Columns)
                        {
                            if (column.ColumnName.Contains("Size@"))
                            {

                                columnOfSize += 1;
                                if (columnOfSize > 14)
                                {
                                    ws.InsertColumn(columnOfSize, 1);
                                }
                                ws.Column(columnOfSize).Width = 6;
                                ws.Cells[rowOfSize, columnOfSize].Value = column.ColumnName.Replace("Size@", "");

                                // ws.Cells[countRow, columnOfSize].Value = rowExport[column];
                            }
                        }
                        if (columnOfSize > 14)
                        {
                            // Merge ô Size lại
                            ws.Cells[6, 5, 7, columnOfSize].Merge = true;
                        }
                    }
                    columnOfSize = 4;

                    // Gán số lượng của từng size
                    int sumSLSP = 0;
                    foreach (DataColumn column in _dtExport.Columns)
                    {
                        if (column.ColumnName.Contains("Size@"))
                        {
                            columnOfSize += 1;

                            ws.Cells[countRow, columnOfSize].Value = rowExport[column];
                            sumSLSP += int.Parse(rowExport[column].ToString());
                        }

                        // Insert dữ liệu vào cột Số lượng sản phẩm
                    }
                    if (columnOfSize > 14)
                    {
                        ws.Cells[countRow, columnOfSize + 1].Value = sumSLSP;
                    }
                    else
                    {
                        // 15 là cột mặc định của Số lượng sản phẩm
                        ws.Cells[countRow, 15].Value = sumSLSP;
                    }
                    SumAllSLSP += sumSLSP;

                    // merge lại cho ô Size

                    ws.Row(countRow).Height = 20;

                    // Cột ngày nhập kho
                    ws.Cells[countRow, 1, countRow, 2].Merge = true;
                    if (_dtExport.Columns.Contains(colName_Ngay_XN) && rowExport[colName_Ngay_XN] != null)
                    {
                        if (rowExport[colName_Ngay_XN] is DateTime)
                        {
                            ws.Cells[countRow, 1].Value = (DateTime.Parse(rowExport[colName_Ngay_XN].ToString())).ToString("dd/MM/yyyy");
                        }
                    }

                    // Check data gridColSoLuongThung
                    if (_dtExport.Columns.Contains(colName_SoThung) && rowExport[colName_SoThung] != null)
                    {
                        // ws.Cells[countRow, 1].Value = row[this.gridColTrangThai.Name].NgayDongThung.ToString("dd-MM-yyyy");
                        ws.Cells[countRow, 3].Value = rowExport[colName_SoThung];
                    }

                    // Check Màu
                    if (_dtExport.Columns.Contains(colName_TenMau) && rowExport[colName_TenMau] != null)
                    {
                        ws.Cells[countRow, 4].Value = rowExport[colName_TenMau];
                    }


                    countRow++;
                }

                // 19: Là tổng số cột
                // 8: Là Row bắt đầu draw style
                // columnOfSize > 14 ? columnOfSize + 5 : 19: Mặc định số cột của size là 10 cột, Khi số lượng size > 10.
                // insert thêm column vào -> Nên drawstyle cho các column này
                // drawstyle từ column 1 vì khi insert column vào, các row phía trên chưa được border
                // -> Sẽ draw style từ row 1 đến cuối
                DrawStyleCellExcel(ws, columnOfSize > 14 ? columnOfSize + 5 : 19, 1, _dtExport.Rows.Count + 9);
            }

            // Điền thông tin Khách Hàng; Mã Hàng; Tông Sản lượng
            string rowInfoDH = string.Format("Khách Hàng/Customer: {0}          Mã Hàng/Style: {1}          Tổng Sản Lượng/Quantity: {2}", _tenKH, _maHang, SumAllSLSP);
            ws.Cells[3, 1].Value = rowInfoDH;
            ws.Cells[3, 1].Style.Font.Size = 12;
            ws.Cells[3, 1].Style.Font.SetFromFont(new Font("Times News Roman", 13.0F));
        }

        private void DrawStyleCellExcel(ExcelWorksheet ws, int countColumn, int startRow, int endRow)
        {
            //ws.Cells[countRow, countColumn].Value = lstSoTheoDoi[i].NgayThangNam.ToString("dd/MM/yyyy");
            for (int row = startRow; row < endRow + startRow; row++)
            {
                for (int column = 1; column <= countColumn; column++)
                {
                    if (row >= 8)
                    {
                        ws.Cells[row, column].Style.Font.Size = 12;
                        ws.Cells[row, column].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ws.Cells[row, column].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }
                    ws.Cells[row, column].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    ws.Cells[row, column].Style.Border.Left.Style = ExcelBorderStyle.Thin;

                    if (column == countColumn)
                    {
                        ws.Cells[row, column].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                }
            }
        }

        private void cbxPhieuXH_EditValueChanged(object sender, EventArgs e)
        {
            FilterMaDH();
            FilterPhieuXH();
            this.ActiveControl = this.gridControlPhieuGiaoNhan;
        }

        private void MainView_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void MainView_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red; // Màu đỏ
        }

        private void MainView_CustomDrawRowFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            // Kiểm tra xem dòng này có phải là footer của group không
            if (view.IsGroupRow(e.RowHandle))
            {
                // Đổi màu nền
                e.Appearance.BackColor = Color.Yellow; // Thay đổi thành màu bạn muốn
            }
        }
        int sum = 0;
        private void bandedGridviewPGN_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                sum = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "SoLuongThung")
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
            }
        }

        private void btnHuyXacNhan(object sender, EventArgs e)
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn hủy xác nhận tất cả không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    {
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    }
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang hủy xác nhận...");

                    DataTable _tbl = (this.bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table;
                    Console.WriteLine("btn_huy_xac_nhan");
                    DataRow _IsXuatHang = _tbl.AsEnumerable().Where(x => (bool)x["IsXuatHang"]).FirstOrDefault();
                    DataRow _IsChuyenKho = _tbl.AsEnumerable().Where(x => (bool)x["IsChuyenKho"]).FirstOrDefault();
                    if (_IsXuatHang != null)
                    {
                        XtraMessageBox.Show("Danh sách hủy có phiếu đã Xuất Hàng. Không thể hủy xác nhận.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (_IsChuyenKho != null)
                    {
                        XtraMessageBox.Show("Danh sách hủy có phiếu đã Chuyển Kho. Không thể hủy xác nhận.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    // Lấy ra những row chưa (xuất hàng || chuyển kho) và đã xác nhận
                    DataRow row = _tbl.AsEnumerable().FirstOrDefault(x => RemoveVietnameseDiacritics(x["IsXacNhan"].ToString()).ToUpper().Equals(_strDaXacNhan));
                    if (row != null)
                    {
                        DataTable _tblHuyXacNhan = _tbl.AsEnumerable().Where(x => RemoveVietnameseDiacritics(x["IsXacNhan"].ToString()).ToUpper().Equals(_strDaXacNhan)).CopyToDataTable();
                        _tblHuyXacNhan = DeleteColumn(_tblHuyXacNhan);
                        string urlChiTietPGN = string.Format("{0}?nVien={1}", URL + "PhieuGiaoNhan/HuyIsXacNhan", GlobleData.UserName);
                        string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlChiTietPGN, _tblHuyXacNhan); }).Result;
                        // Load lại data
                        //LoadData();
                        UpdateUI();
                        if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bandedGridViewChiTiet_CellMerge(object sender, CellMergeEventArgs e)
        {
            // Truy cập đến GridView
            var gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            // bandedGridColCarton: Carton

            // Kiểm tra nếu hai ô thuộc cột cần kiểm tra
            if (e.Column.Equals(this.bandedGridColCarton))
            {
                // Lấy giá trị của hai ô đang được xem xét

                // So sánh giá trị để quyết định có hợp nhất hay không
                e.Merge = CheckMergeRow(bandedGridViewChiTiet, e.RowHandle1, e.RowHandle2);
                // Đặt Handled thành true để không cần kiểm tra thêm
                e.Handled = true;
            }
            //else
            //{
            //    // Đối với các cột khác, không cho phép hợp nhất
            //    e.Merge = false;
            //    e.Handled = true;
            //}
        }

        private bool CheckMergeRow(GridView gridView, int rowHandle, int prevRow)
        {
            try
            {
                // POID
                DataRow entityHandle = gridView.GetDataRow(rowHandle);
                DataRow entityPrev = gridView.GetDataRow(prevRow);
                int tuThungHandle = -1;
                int denThungHandle = -1;
                int sttThungHandle = -1;

                int tuThungPrev = -1;
                int denThungPrev = -1;
                int sttThungPrev = -1;

                if (entityHandle != null && entityPrev != null)
                {


                    if (entityHandle.Table.Columns.Contains("TuThung") && entityHandle.Table.Columns.Contains("DenThung"))
                    {
                        int.TryParse(entityHandle["TuThung"].ToString(), out tuThungHandle);
                        int.TryParse(entityHandle["DenThung"].ToString(), out denThungHandle);
                        int.TryParse(entityHandle["SttThung"].ToString(), out sttThungHandle);
                    }

                    if (entityPrev.Table.Columns.Contains("TuThung") && entityPrev.Table.Columns.Contains("DenThung"))
                    {
                        int.TryParse(entityPrev["TuThung"].ToString(), out tuThungPrev);
                        int.TryParse(entityPrev["DenThung"].ToString(), out denThungPrev);
                        int.TryParse(entityPrev["SttThung"].ToString(), out sttThungPrev);
                    }

                    if (tuThungHandle > -1 && denThungPrev > -1
                        && tuThungPrev > -1 && denThungPrev > -1
                        )
                    {
                        return (tuThungHandle.Equals(tuThungPrev) && denThungHandle.Equals(denThungPrev) && sttThungHandle.Equals(sttThungPrev));
                    }


                    ////if (poIDValue1.Equals(poIDValue2) && poValue1.Equals(poValue2) &&
                    ////        dauSizeIDValue1.Equals(dauSizeIDValue2) && dauSizeValue1.Equals(dauSizeValue2) &&
                    ////        maDVSXValue1.Equals(maDVSXValue2) && maHangValue1.Equals(maHangValue2) &&
                    ////        maMauValue1.Equals(maMauValue2) && tenMauValue1.Equals(tenMauValue2))
                    ////{
                    ////    Console.WriteLine("True_CheckMergeRow - dòng: " + rowHandle);
                    ////}
                    ////else
                    ////{
                    ////    Console.WriteLine("False_CheckMergeRow - dòng: " + rowHandle);
                    ////}
                    //return (poIDValue1.Equals(poIDValue2) && poValue1.Equals(poValue2) &&
                    //        maDVSXValue1.Equals(maDVSXValue2) && maHangValue1.Equals(maHangValue2));
                }
                return false;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        private void bandedGridViewChiTiet_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            int sum = 0;
            GridView view = (GridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                sum = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                //Console.WriteLine("gridView_CustomSummaryCalculate");
                string fieldname = (e.Item as GridColumnSummaryItem).FieldName;
                if ((fieldname.Equals(this.bandedGridColCarton.FieldName)))
                {
                    DataTable tblChiTietPGN = (view.DataSource as DataView).Table;
                    for (int i = 0; i < tblChiTietPGN.Rows.Count; i++)
                    {
                        DataRow dataRow = tblChiTietPGN.Rows[i];
                        //Console.WriteLine("i: " + i);
                        if (!IsFirstRowInGroup(i))
                        {
                            //if (fieldname.Equals(this.gridColumn11.FieldName))
                            //{
                            //    Console.WriteLine("sum_chua_dong: " + lstDsDongThungEntity[i].SoThungChuaDong);
                            //}
                            //else
                            //{
                            //    Console.WriteLine("sum_tong: " + lstDsDongThungEntity[i].TongSoThung);
                            //}
                            //sum += fieldname.Equals(this.gridColumn11.FieldName) ? lstDsDongThungEntity[i].SoThungChuaDong : lstDsDongThungEntity[i].TongSoThung;
                            if (dataRow["Carton"] is long || dataRow["Carton"] is int)
                            {
                                sum += int.Parse(dataRow["Carton"].ToString());
                            }
                        }
                        else
                        {
                            //if (fieldname.Equals(this.gridColumn11.FieldName))
                            //{
                            //    Console.WriteLine("not_sum_chua_dong: " + lstDsDongThungEntity[i].SoThungChuaDong);
                            //}
                            //else
                            //{
                            //    Console.WriteLine("not_sum_tong: " + lstDsDongThungEntity[i].TongSoThung);
                            //}
                        }

                    }
                }
                e.TotalValue = sum;
            }
        }
        private bool IsFirstRowInGroup(int rowHandle)
        {
            int prevRowHandle = rowHandle - 1;
            if (prevRowHandle == GridControl.InvalidRowHandle || prevRowHandle < 0)
                return false; ;
            return CheckMergeRow(bandedGridViewChiTiet, rowHandle, prevRowHandle);
        }

        private void MapDataCBXFilter()
        {
            cbxFilter.Properties.Items.Clear();
            cbxFilter.Properties.Items.Add("Thời gian");
            cbxFilter.Properties.Items.Add("Đơn hàng");
        }

        private void cbxFilter_EditValueChanged(object sender, EventArgs e)
        {
            if (cbxFilter.EditValue != null)
            {
                string _cbxFilter = RemoveVietnameseDiacritics(cbxFilter.EditValue.ToString());
                UpdateView(_cbxFilter);
                UpdateUI();
            }
        }

        private void UpdateView(string _cbxFilter)
        {
            switch (_cbxFilter.ToUpper())
            {
                case _filterTime:
                    this.layoutTuNgay.Visibility = LayoutVisibility.Always;
                    this.layoutDenNgay.Visibility = LayoutVisibility.Always;
                    // filter Đơn hàng
                    this.layoutControlItem1.Visibility = LayoutVisibility.Always;
                    // filter Phiếu xuất hàng
                    this.layoutControlItem7.Visibility = LayoutVisibility.Always;
                    // filter Đơn hàng all
                    this.layoutControlItem6.Visibility = LayoutVisibility.Never;


                    emptySpaceItem2.Visibility = LayoutVisibility.Always;
                    emptySpaceItem4.Visibility = LayoutVisibility.Always;
                    //emptySpaceItem6.Visibility = LayoutVisibility.Always;
                    //emptySpaceItem5.Visibility = LayoutVisibility.Always;


                    //emptySpaceItem5.Width = 30;
                    break;
                case _filterDH:
                    this.layoutTuNgay.Visibility = LayoutVisibility.Never;
                    this.layoutDenNgay.Visibility = LayoutVisibility.Never;
                    // filter Đơn hàng
                    this.layoutControlItem1.Visibility = LayoutVisibility.Never;
                    // filter Phiếu xuất hàng
                    this.layoutControlItem7.Visibility = LayoutVisibility.Never;
                    // filter Đơn hàng all
                    this.layoutControlItem6.Visibility = LayoutVisibility.Always;
                    // this.layoutControlItem6.Width = emptySpaceItem8.Width + layoutTuNgay.Width + emptySpaceItem2.Width + layoutDenNgay.Width + emptySpaceItem4.Width + layoutControlItem1.Width + emptySpaceItem6.Width + layoutControlItem3.Width;



                    emptySpaceItem2.Visibility = LayoutVisibility.Never;
                    emptySpaceItem4.Visibility = LayoutVisibility.Never;
                    //emptySpaceItem6.Visibility = LayoutVisibility.Never;
                    //emptySpaceItem5.Visibility = LayoutVisibility.Never;
                    GetDSDonHangNhapKho();
                    break;
            }
        }

        public void GetDSDonHangNhapKho()
        {
            string urlDsDHNhapKho = string.Format("{0}?", URL + "PhieuGiaoNhan/GetDSDonHangNhapKho");
            string jsonDsDHNhapKho = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDsDHNhapKho); }).Result;
            DataTable dtDsDHNhapKho = JsonConvert.DeserializeObject<DataTable>(jsonDsDHNhapKho);
            this.searchLookUpEditMaDH.Properties.DataSource = dtDsDHNhapKho;
        }

        public string RemoveVietnameseDiacritics(string text)
        {
            // Normalize the text to decompose combined characters into their base characters and diacritics
            string normalizedString = text.Normalize(NormalizationForm.FormD);

            // Use regex to remove all non-spacing marks (diacritics)
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string result = regex.Replace(normalizedString, string.Empty);

            // Replace special Vietnamese characters
            result = result.Replace('đ', 'd').Replace('Đ', 'D');

            return result.Replace(" ", "");
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            checkEdit1.EditValue = false;
            Console.WriteLine("searchLookUpEdit1_EditValueChanged");
            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            if (changingEventArgs != null && changingEventArgs.NewValue != null && !string.IsNullOrEmpty(changingEventArgs.NewValue.ToString()))
            {
                string _maDH = changingEventArgs.NewValue.ToString();
                GetPhieuGiaoNhanAllowDonHang(_maDH);
            }
            else
            {
                gridControlPhieuGiaoNhan.DataSource = null;
                gridControlChiTietGiaoNhan.DataSource = null;
                XtraMessageBox.Show("Vui lòng chọn đơn hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void GetPhieuGiaoNhanAllowDonHang(string maDH)
        {
            string urlPGN = string.Format("{0}?maDH={1}", URL + "PhieuGiaoNhan/GetPGNDonHang", maDH);
            jsonDataPGN = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlPGN); }).Result;
            DataTable tblPGN = JsonConvert.DeserializeObject<DataTable>(jsonDataPGN);
            gridControlPhieuGiaoNhan.DataSource = tblPGN;

            LoadDataChiTiet(this.bandedGridViewPhieuGiaoNhan);
        }

        private void searchLookUpEditMaDH_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            Console.WriteLine("searchLookUpEditMaDH_CustomDisplayText");
        }

        private void searchLookUpFilterDH_EditValueChanged(object sender, EventArgs e)
        {
            FilterMaDH(true);
            searchLookUpEditFilterPXH.EditValue = null;
            MapDataCBXPhieuXH(bandedGridViewPhieuGiaoNhan);
            this.checkEdit1.EditValue = false;
            this.ActiveControl = this.gridControlPhieuGiaoNhan;
        }

        private void searchLookUpEditFilterPXH_EditValueChanged(object sender, EventArgs e)
        {
            ChangingEventArgs changing = e as ChangingEventArgs;
            if (e != null && changing.NewValue != null)
            {
                this.checkEdit1.EditValue = false;
                FilterMaDH();
                FilterPhieuXH();
                this.ActiveControl = this.gridControlPhieuGiaoNhan;
            }
        }

        private void bandedGridViewPhieuGiaoNhan_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                if (e.Column.Equals(this.gridColPhieuXH) && e.CellValue != null && !string.IsNullOrEmpty(e.CellValue.ToString().Trim()))
                {
                    bool _isXuatHang = !string.IsNullOrEmpty(e.CellValue.ToString());
                    if (_isXuatHang)
                    {
                        Console.WriteLine("bandedGridViewPhieuGiaoNhan_RowCellStyle");
                        e.Appearance.BackColor = Color.FromArgb(245, 215, 213);
                    }
                }
                else if (e.Column.Equals(this.bandedGridColBienBanChuyenKho) && e.CellValue != null && !string.IsNullOrEmpty(e.CellValue.ToString().Trim()))
                {
                    bool _isChuyenKho = !string.IsNullOrEmpty(e.CellValue.ToString());
                    if (_isChuyenKho)
                    {
                        Console.WriteLine("bandedGridViewPhieuGiaoNhan_RowCellStyle");
                        e.Appearance.BackColor = Color.FromArgb(144, 181, 240);
                    }
                }
                //e.Appearance.ForeColor = Color.White;
            }
        }

        private void bandedGridViewPhieuGiaoNhan_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            if (bandedGridViewPhieuGiaoNhan.FocusedRowHandle >= 0 && bandedGridViewPhieuGiaoNhan.FocusedColumn.Equals(this.gridColNgayNhapKho))
            {
                if (e.Value != null)
                {
                    Console.WriteLine("bandedGridViewPhieuGiaoNhan_ValidatingEditor");
                    DateTime _time_XH = (DateTime)bandedGridViewPhieuGiaoNhan.GetFocusedRowCellValue(this.bandedGridColNgayXuatHang);
                    DateTime _time_NK = (DateTime)e.Value;
                    DateTime _time_NK_BF = (DateTime)bandedGridViewPhieuGiaoNhan.GetFocusedRowCellValue(this.gridColNgayNhapKho);

                    // Check _time_XH nếu là ngày 1-1-2500: Được xem là chưa xuất hàng
                    // Nếu chưa xuất hàng thì Ngày Nhập Kho không được sửa vượt quá ngày hiện tại
                    // Nếu đã xuất hàng thì ngày Nhập Kho không được sửa vượt quá ngày Xuất Hàng
                    if (_time_XH.CompareTo(new DateTime(2500, 1, 1)) >= 0)
                    {
                        if (_time_NK.CompareTo(DateTime.Now).Equals(1))
                        {
                            XtraMessageBox.Show("Ngày Nhập Kho không được vượt qua ngày hiện tại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Value = _time_NK_BF;
                            e.Valid = false;
                            e.ErrorText = "Ngày Nhập Kho không được vượt qua ngày hiện tại";
                            return;
                        }
                    }
                    else
                    {
                        if (_time_NK.CompareTo(_time_XH).Equals(1))
                        {
                            XtraMessageBox.Show("Ngày Nhập Kho không được sau ngày Xuất Hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Value = _time_NK_BF;
                            e.Valid = false;
                            e.ErrorText = "Ngày Nhập Kho không được sau ngày xuất hàng";
                            return;
                        }
                    }

                }
                else
                {
                    e.Valid = false;
                    e.ErrorText = "Dữ liệu không được để trống";
                }
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Luu();
        }

        private void Luu()
        {
            this.ActiveControl = gridControlChiTietGiaoNhan;

            DataTable _tblSave = new DataTable();

            if (_lstRowSave.Count > 0)
            {
                _lstRowSave = _lstRowSave.Distinct().OrderBy(x => x).ToList();
                DataTable _tblPhieuGiaoNhan = (bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table.AsEnumerable().CopyToDataTable();
                _tblPhieuGiaoNhan = _tblPhieuGiaoNhan.AsEnumerable().OrderBy(row => row.Field<string>("StrMaPKL")).CopyToDataTable();
                _tblSave = _lstRowSave.Select(index => _tblPhieuGiaoNhan.Rows[index]).AsEnumerable().CopyToDataTable();
                Console.WriteLine("bandedGridViewPhieuGiaoNhan_CellValueChanged");
                _lstRowSave.Clear();
            }
            else
            {
                return;
            }

            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }

            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
            _tblSave = DeleteColumn(_tblSave);
            string urlSave = string.Format("{0}?", URL + "PhieuGiaoNhan/UpdateNgayNhapKho");
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, _tblSave); }).Result;
            if (result.ToLower().Equals("true"))
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                clsWaitForm.ShowSuccessForm(this, 1500);

                // Xử lý xóa những dòng không nằm trong khoảng Từ Ngày -> Đến Ngày ra khỏi View
                UpdateUIOffline();
            }
            else
            {
                XtraMessageBox.Show(result);
            }
        }

        private void UpdateUIOffline()
        {
            if (RemoveVietnameseDiacritics(cbxFilter.EditValue.ToString()).ToUpper().Equals(_filterTime))
            {
                if (dateTuNgay.EditValue != null && !string.IsNullOrEmpty(dateTuNgay.EditValue.ToString())
                    && dateDenNgay.EditValue != null && !string.IsNullOrEmpty(dateDenNgay.EditValue.ToString()))
                {
                    DataTable _tblPhieuGiaoNhan = (bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table;
                    DataRow row_tblPhieuGiaoNhan = _tblPhieuGiaoNhan.AsEnumerable().Where(x =>
                    (DateTime)x["NgayNhapKho_Update"] >= (DateTime)dateTuNgay.EditValue
                    && (DateTime)x["NgayNhapKho_Update"] <= (DateTime)dateDenNgay.EditValue
                        ).FirstOrDefault();
                    if (row_tblPhieuGiaoNhan != null)
                    {
                        _tblPhieuGiaoNhan = _tblPhieuGiaoNhan.AsEnumerable().Where(x =>
                            (DateTime)x["NgayNhapKho_Update"] >= (DateTime)dateTuNgay.EditValue
                            && (DateTime)x["NgayNhapKho_Update"] <= (DateTime)dateDenNgay.EditValue).CopyToDataTable();
                    }
                    else
                    {
                        _tblPhieuGiaoNhan = null;
                    }

                    gridControlPhieuGiaoNhan.DataSource = _tblPhieuGiaoNhan;
                    Console.WriteLine("UpdateUIOffline");
                }
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.ActiveControl = this.gridControlChiTietGiaoNhan;
            XuatExcelPGNDongChon();
        }

        private void XuatExcelPGNDongChon()
        {

            //DataTable dtPGNSelect = (bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table.AsEnumerable()
            //    .Where((x) => bool.Parse(x["MultiSelect"].ToString()) == true && int.Parse(x["IsXacNhan"].ToString()) == 1).CopyToDataTable();

            DataRow rowCheck = (bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table.AsEnumerable()
                .Where((x) => bool.Parse(x["MultiSelect"].ToString()) == true && int.Parse(x["IsXacNhan"] != null ? (RemoveVietnameseDiacritics(x["IsXacNhan"].ToString()).ToUpper().Equals(_strDaXacNhan) ? "1" : "0") : "0") == 1).FirstOrDefault();

            //int _isXacNhan = int.Parse(row["IsXacNhan"] != null ? (RemoveVietnameseDiacritics(row["IsXacNhan"].ToString()).ToUpper().Equals(_strDaXacNhan) ? "1" : "0") : "0");

            if (rowCheck == null)
            {
                Console.WriteLine("rowCheck");
                XtraMessageBox.Show("Vui lòng chọn dòng cần xuất excel trước khi thực hiện.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            //List<DongThungBaoCaoEntity> lstExport = gridView1.DataSource as List<DongThungBaoCaoEntity>;
            //DataTable _dtExport = (bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table;
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("baocaophieugiaonhan{0}", DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "baocaophieugiaonhan.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                //gridView1.ExportToXlsx(Sfd.FileName);
                bool isExport = Export(TemplateFileName, ExportFileName, isAll: false);
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                if (isExport)
                {
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
                }
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            //}
            //else
            //{
            //    XtraMessageBox.Show("Vui lòng lọc dữ liệu theo Mã DVSX, Mã Hàng và Ngày Lập Phiếu để xuất Excel.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            //    return;
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ActiveControl = this.gridControlChiTietGiaoNhan;
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            Console.WriteLine("check_changed");
            FilterMaDH();

            DataTable _tblNotPXH = (bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table;
            bool _isCheckNotPXH = (bool)checkEdit1.EditValue;
            if (_isCheckNotPXH)
            {
                searchLookUpEditFilterPXH.EditValue = null;
                DataRow _row = _tblNotPXH.AsEnumerable().FirstOrDefault((x) => (x["BienBanXuatHang"] is null) || (string.IsNullOrEmpty(x["BienBanXuatHang"].ToString())));
                if (_row != null)
                {
                    _tblNotPXH = _tblNotPXH.AsEnumerable().Where((x) => (x["BienBanXuatHang"] is null) || (string.IsNullOrEmpty(x["BienBanXuatHang"].ToString()))).CopyToDataTable();
                    this.gridControlPhieuGiaoNhan.DataSource = _tblNotPXH;
                }
                else
                {
                    this.gridControlPhieuGiaoNhan.DataSource = null;
                }
            }
        }

        private void bandedGridViewPhieuGiaoNhan_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //if (e != null && e.RowHandle >= 0)
            //{
            //    _lstRowSave.Add(e.RowHandle);
            //}

            // Tìm index của dòng đang focus trong thứ tự của all danh sách bảng size( Không filter)
            DataTable _lstPGN = (bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table as DataTable;
            List<DataRow> _lstDataRows = _lstPGN.AsEnumerable().OrderBy(row => row.Field<string>("StrMaPKL")).ToList();
            DataRow _itemPGN = bandedGridViewPhieuGiaoNhan.GetFocusedDataRow();
            int index = _lstDataRows.IndexOf(_itemPGN);

            if (index >=0 ) {
                _lstRowSave.Add(index);
            }
            //lstRowUpdate.Add(index);
        }

        //private DataTable GetPGNDongChon()
        //{
        //    DataTable _dtExport;
        //    // Lấy ra những dòng được chọn
        //    DataTable _tblPhieuGiaoNhan = (bandedGridViewPhieuGiaoNhan.DataSource as DataView).Table.AsEnumerable()
        //    .CopyToDataTable().AsEnumerable()
        //    .Where((x) => bool.Parse(x["MultiSelect"].ToString()) == true && int.Parse(x["IsXacNhan"].ToString()) == 1).CopyToDataTable();
        //    Console.WriteLine("_tblPhieuGiaoNhan: " + _tblPhieuGiaoNhan);
        //    // Get chi tiet tung row

        //    foreach (DataRow row in _tblPhieuGiaoNhan.Rows)
        //    {

        //    }
        //    return _dtExport;
        //}
    }
}