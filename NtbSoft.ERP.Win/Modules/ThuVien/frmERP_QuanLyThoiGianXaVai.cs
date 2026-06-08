
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.Erp.Kho;
using NtbSoft.ERP.Win.Modules.Kho;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERP_QuanLyThoiGianXaVai : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;

        private HttpClientExtension _clientExtension;
        DataTable tblVatTu;
        SearchCheckSelection gridCheckMarksColor;
        string mau;
        DataTable _dttable;
        DataTable tblCNL;
        DataTable tblCPL;
        DataTable dttbmh;
        DataTable rowselectmh;

        private bool _allowAdd = false;
        private bool _allowEdit = false;
        private bool _allowDelete = false;

        private List<GridCell> savedSelectedCells = new List<GridCell>();
        private SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _isNPL = true;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string _makh = string.Empty, _mahang = string.Empty, _nhom = string.Empty, _madvvt = string.Empty, _mamausp = string.Empty, _khovaiid = string.Empty;
        string Host = string.Empty; bool indicatorIcon = true;
        public frmERP_QuanLyThoiGianXaVai()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            tblVatTu = new DataTable();
            Host = (string)settingsReader.GetValue("HostDH", typeof(String));
            gVNL.CellValueChanged += gVNL_CellValueChanged;
    
        }
        public frmERP_QuanLyThoiGianXaVai(string makh = "", string mahang = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            tblVatTu = new DataTable();
            _makh = makh;
            _mahang = mahang;
        }
        private DataTable CreateTable()
        {
            try
            {
                DataTable dtbcreate = new DataTable("dtbcreate");
                dtbcreate.Columns.Add("ID", typeof(int));
                dtbcreate.Columns.Add("MaHang", typeof(string));
                dtbcreate.Columns.Add("Makh", typeof(string));
                dtbcreate.Columns.Add("MaNhom", typeof(string));
                dtbcreate.Columns.Add("MaVTID", typeof(string));
                dtbcreate.Columns.Add("MaVT", typeof(string));
                dtbcreate.Columns.Add("ChiTiet", typeof(string));
                dtbcreate.Columns.Add("MaDVVT", typeof(string));
                dtbcreate.Columns.Add("TenDVVT", typeof(string));
                dtbcreate.Columns.Add("MauVTID", typeof(string));
                dtbcreate.Columns.Add("MaMauVT", typeof(string));
                dtbcreate.Columns.Add("MauVT", typeof(string));
                dtbcreate.Columns.Add("KhoVaiID", typeof(string));
                dtbcreate.Columns.Add("KhoVai", typeof(string));
                dtbcreate.Columns.Add("MaMau", typeof(string));
                dtbcreate.Columns.Add("NPL", typeof(bool));
                dtbcreate.Columns.Add("TenMau", typeof(string));
                dtbcreate.Columns.Add("TenNhom", typeof(string));
                dtbcreate.Columns.Add("Sort", typeof(int));
                dtbcreate.Columns.Add("GhiChu", typeof(string));
                dtbcreate.Columns.Add("ThoiGian", typeof(string));
                dtbcreate.Columns.Add("IsNew", typeof(int));
                dtbcreate.Columns.Add("STT", typeof(int));
                dtbcreate.Columns.Add("MaVTGhep", typeof(string));
                dtbcreate.Columns.Add("urlAnh", typeof(string));
                dtbcreate.Columns.Add("IsKV", typeof(bool));
                dtbcreate.Columns.Add("MaTheSize", typeof(string));
                return dtbcreate;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private void loadVatTu()
        {
            try
            {

                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang tải dữ liệu");
                string manhom = searchLookUpEditLocCL.EditValue == null ? "" : searchLookUpEditLocCL.EditValue.ToString();
                string url = $"{URL}ERPThoiGianXaVai/GetChung?Action=Get&para={manhom}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;



                if (json == "[]")
                {
                    _dttable = CreateTable();
                    if (tblCNL != null && tblCNL.Rows.Count != 0)
                        tblCNL.Clear();
                    if (tblCPL != null && tblCPL.Rows.Count != 0)
                        tblCPL.Clear();
                    tblCNL = _dttable.Clone();
                    tblCPL = _dttable.Clone();
                    gCNL.DataSource = tblCNL;
                  
                    SplashScreenManager.CloseForm(false);
                    return;
                }

                if (_dttable != null && _dttable.Rows.Count != 0)
                {
                    _dttable.Clear();
                }

                if (string.IsNullOrWhiteSpace(json) || json == "[]")
                {
                    _dttable = CreateTable();
                }
                else
                {
                    _dttable = JsonConvert.DeserializeObject<DataTable>(json);

                    if (_dttable == null)
                        _dttable = CreateTable();
                }

                if (_dttable != null && !_dttable.Columns.Contains("IsNew"))
                {
                    _dttable.Columns.Add("IsNew", typeof(int));
                }
                if (!_dttable.Columns.Contains("STT"))
                {
                    _dttable.Columns.Add("STT", typeof(int));
                }
                if (!_dttable.Columns.Contains("IsEdit"))
                {
                    _dttable.Columns.Add("IsEdit", typeof(int));
                }

                if (_dttable == null || _dttable.Rows.Count == 0)
                {
                    gCNL.DataSource = null;
                    gCNL.DataSource = null;
                    SplashScreenManager.CloseForm(false);
                    return;
                }


                tblCNL = _dttable.Copy();
                gCNL.DataSource = tblCNL;


                if (tblCNL.Rows.Count == 0) tblCNL = _dttable.Clone();
   

                //RefreshSTTColumn(tblCNL);
                //RefreshSTTColumn(tblCPL);
                gVNL.SelectionChanged -= gVNL_SelectionChanged;
             
                gCNL.DataSource = tblCNL;
         
                gVNL.ClearSelection();
        
                if (_isNPL)
                {
                    RestoreSelectedCells(gVNL);
                }
                else
                {
               
                }
                gVNL.SelectionChanged += gVNL_SelectionChanged;
            
                SplashScreenManager.CloseForm(false);
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);  //MessageBox.Show("loadVatTu " + ex, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
            }

        }

        private void gVNL_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            savedSelectedCells.Clear();
            var selectedCells = gVNL.GetSelectedCells();
            foreach (GridCell cell in selectedCells)
            {
                savedSelectedCells.Add(cell);
            }
        }

        private void gVNL_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            DataRow row = gVNL.GetDataRow(e.RowHandle);
            if (row != null)
                row["IsEdit"] = 1;
        }


        private DataTable GetDataSave()
        {
            GridView view = gVNL;

            view.PostEditor();
            view.CloseEditor();
            view.UpdateCurrentRow();

            DataTable source = null;

            if (view.GridControl.DataSource is DataTable)
            {
                source = (DataTable)view.GridControl.DataSource;
            }
            else if (view.GridControl.DataSource is BindingSource)
            {
                BindingSource bs = (BindingSource)view.GridControl.DataSource;
                source = bs.DataSource as DataTable;
            }

            if (source == null)
                return null;

            DataTable dtSave = new DataTable();

            dtSave.Columns.Add("MaNhom", typeof(string));
            dtSave.Columns.Add("MaDVVT", typeof(string));
            dtSave.Columns.Add("MaVTID", typeof(string));
            dtSave.Columns.Add("MauVTID", typeof(string));
            dtSave.Columns.Add("KhoVaiID", typeof(string));
            dtSave.Columns.Add("ThoiGian", typeof(string));
            dtSave.Columns.Add("NguoiTao", typeof(string));
            dtSave.Columns.Add("NgayTao", typeof(DateTime));
            dtSave.Columns.Add("NguoiSua", typeof(string));
            dtSave.Columns.Add("NgaySua", typeof(DateTime));

            DataRow[] rowsEdit = source.Select("IsEdit = 1");

            foreach (DataRow row in rowsEdit)
            {
                string maNhom = row["MaNhom"]?.ToString().Trim();
                string maDVVT = row["MaDVVT"]?.ToString().Trim();
                string maVTID = row["MaVTID"]?.ToString().Trim();
                string mauVTID = row["MauVTID"]?.ToString().Trim();
                string khoVaiID = row["KhoVaiID"]?.ToString().Trim();
                string thoiGian = row["ThoiGian"]?.ToString()
                    .Replace("h", "")
                    .Replace("Khác", "")
                    .Trim();

                if (string.IsNullOrWhiteSpace(maNhom) ||
                    string.IsNullOrWhiteSpace(maDVVT) ||
                    string.IsNullOrWhiteSpace(thoiGian))
                {
                    continue;
                }

                DataRow dr = dtSave.NewRow();
                dr["MaNhom"] = maNhom;
                dr["MaDVVT"] = maDVVT;
                dr["MaVTID"] = maVTID;
                dr["MauVTID"] = mauVTID;
                dr["KhoVaiID"] = khoVaiID;
                dr["ThoiGian"] = thoiGian;
                dr["NguoiTao"] = GlobleData.UserName;
                dr["NgayTao"] = DateTime.Now;
                dr["NguoiSua"] = GlobleData.UserName;
                dr["NgaySua"] = DateTime.Now;

                dtSave.Rows.Add(dr);
            }

            return dtSave;
        }



        private void _cboTG_ParseEditValue(object sender, ConvertEditValueEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value)
            {
                e.Value = "";
                e.Handled = true;
                return;
            }

            string text = e.Value.ToString().Trim();

            if (text.EndsWith("h") && text != "Khác")
            {
                text = text.Substring(0, text.Length - 1);
            }

            e.Value = text;
            e.Handled = true;
        }

        private void _cboTG_FormatEditValue(object sender, ConvertEditValueEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value)
            {
                e.Value = "";
                e.Handled = true;
                return;
            }

            string text = e.Value.ToString().Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                e.Value = "";
            }
            else if (text == "Khác")
            {
                e.Value = "Khác";
            }
            else if (!text.EndsWith("h"))
            {
                e.Value = text + "h";
            }
            else
            {
                e.Value = text;
            }

            e.Handled = true;
        }

        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataTable dt = GetDataSave();

                if (dt == null || dt.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu để lưu.");
                    return;
                }

                string url = $"{URL}ERPThoiGianXaVai/PostThoiGianXaVai";
                string json = JsonConvert.SerializeObject(dt);

                string result = Task.Run(async () =>
                {
                    using (HttpClient client = new HttpClient())
                    {
                        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync(url, content);
                        return await response.Content.ReadAsStringAsync();
                    }
                }).Result;

                if (result.Replace("\"", "").Trim().ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    loadVatTu();
                }
                else
                {
                    XtraMessageBox.Show("Lỗi từ Server: " + result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void RestoreSelectedCells(GridView view)
        {
            view.ClearSelection();
            foreach (GridCell cell in savedSelectedCells)
            {

                if (view.IsValidRowHandle(cell.RowHandle) && cell.Column != null)
                {
                    view.SelectCell(cell.RowHandle, cell.Column);
                }
            }
        }




        private void gVNL_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            var view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (e.Column.FieldName == "STT1" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void gVPL_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            var view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (e.Column.FieldName == "STT1" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }
        private RepositoryItemComboBox _cboTG;

        private void CreateThoiGianCombo()
        {
            _cboTG = new RepositoryItemComboBox();

            _cboTG.TextEditStyle = TextEditStyles.DisableTextEditor;

            InitializeThoiGianItems();

            _cboTG.SelectedIndexChanged += CboTG_SelectedIndexChanged;
            _cboTG.ParseEditValue += _cboTG_ParseEditValue;
            _cboTG.FormatEditValue += _cboTG_FormatEditValue;

            colThoiGianNgL.ColumnEdit = _cboTG;

        }

        private void InitializeThoiGianItems()
        {
            _cboTG.Items.Clear();

            for (int i = 24; i <= 72; i += 24)
            {
                _cboTG.Items.Add(i + "h");
            }

            _cboTG.Items.Add("Khác");
        }

        private void CboTG_QueryPopUp(object sender, CancelEventArgs e)
        {
            ExtendComboItemsIfNeeded();
        }

        private void CboTG_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEdit combo = sender as ComboBoxEdit;
            if (combo == null) return;

            string value = combo.SelectedItem?.ToString();

            if (value == "Khác")
            {
                string input = XtraInputBox.Show("Nhập số giờ:", "Thời Gian Xả Vải", "");


                if (string.IsNullOrWhiteSpace(input))
                {
                    combo.EditValue = null;

                    GridControl grid = combo.Parent as GridControl;
                    if (grid != null)
                    {
                        GridView view = grid.FocusedView as GridView;
                        if (view != null)
                        {
                            view.SetFocusedRowCellValue(view.FocusedColumn, DBNull.Value);
                        }
                    }

                    return;
                }

                int hour;
                if (int.TryParse(input, out hour) && hour > 0)
                {

                    combo.EditValue = hour + "h";

                    GridControl grid = combo.Parent as GridControl;
                    if (grid != null)
                    {
                        GridView view = grid.FocusedView as GridView;
                        if (view != null)
                        {
                            view.SetFocusedRowCellValue(view.FocusedColumn, hour + "h");
                        }
                    }
                }
                else
                {
                    combo.EditValue = null;
                }
            }
        }
        private void ExtendComboItemsIfNeeded()
        {
            if (_cboTG.Items.Count == 0)
            {
                _cboTG.Items.Add("24");
                return;
            }

            string lastValue = _cboTG.Items[_cboTG.Items.Count - 1].ToString();
            int lastHour = 24;


        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                searchLookUpEditLocCL.EditValue = null;
                txtFind.Text = string.Empty;
                gVNL.ActiveFilterString = string.Empty;
                savedSelectedCells.Clear();
                gVNL.ClearSelection();

                if (_dttable != null) _dttable.Clear();
                if (tblCNL != null) tblCNL.Clear();

                loadVatTu();

               
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {

                gVNL.CustomUnboundColumnData += gVNL_CustomUnboundColumnData;
   
                gVNL.ShowingEditor += gVNL_ShowingEditor;
                CheckPerminsion();
                CreateThoiGianCombo();
                loadLocChungLoai();
                loadVatTu();
                gVNL.OptionsView.ColumnAutoWidth = true;
                gVNL.BeginUpdate();
                try
                {
                    gVNL.ClearGrouping();

       

                    // group level 1
              

                    // group level 2
                    gVNL.Columns["TenNhom"].GroupIndex = 1;

                    gVNL.ExpandAllGroups();
                }
                finally
                {
                    gVNL.EndUpdate();
                }
            }
            catch { }
        }
        private void gVNL_ShowingEditor(object sender, CancelEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            if (view.FocusedColumn != colThoiGianNgL)
            {
                e.Cancel = true;
            }
        }
        private void btnHistory_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                int rowHandle = gVNL.FocusedRowHandle;

                if (rowHandle < 0)
                    return;

                DataRow row = gVNL.GetDataRow(rowHandle);

                if (row == null)
                    return;

                string maNhom = row["MaNhom"]?.ToString();
                string maDVVT = row["MaDVVT"]?.ToString();
                string maVTID = row["MaVTID"]?.ToString();
                string mauVTID = row["MauVTID"]?.ToString();
                string khoVaiID = row["KhoVaiID"]?.ToString();

                string url =
                    $"{URL}ERPThoiGianXaVai/GetChung?" +
                    $"Action=GETHISTORY" +
                    $"&para={maNhom}" +
                    $"&para1={maDVVT}" +
                    $"&para2={maVTID}" +
                    $"&para3={mauVTID}" +
                    $"&para4={khoVaiID}";

                string json = Task.Run(async () =>
                {
                    return await _clientExtension.GetAsnyc(url);
                }).Result;

                if (string.IsNullOrWhiteSpace(json) || json == "[]")
                {
                    XtraMessageBox.Show("Không có lịch sử thay đổi.");
                    return;
                }

                DataTable dt =
                    JsonConvert.DeserializeObject<DataTable>(json);

                XtraForm frm = new XtraForm();

                frm.Text = "Lịch Sử Thay Đổi Thời Gian Xả Vải";

                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Width = 1600;
                frm.Height = 500;

                GridControl gc = new GridControl();
                GridView gv = new GridView();

                gc.Dock = DockStyle.Fill;

                frm.Controls.Add(gc);

                gc.MainView = gv;
                gc.ViewCollection.Add(gv);

                gc.DataSource = dt;

                gv.BeginUpdate();

                try
                {
                    gv.OptionsBehavior.Editable = false;
                    gv.OptionsBehavior.ReadOnly = true;

                    gv.OptionsSelection.EnableAppearanceFocusedCell = false;

                    gv.FocusRectStyle =
                        DrawFocusRectStyle.RowFullFocus;

                    gv.OptionsView.ShowGroupPanel = false;
                    gv.OptionsView.ShowIndicator = false;
                    gv.OptionsView.ShowAutoFilterRow = false;

                    // IMPORTANT
                    gv.OptionsView.ColumnAutoWidth = true;

                    gv.OptionsView.RowAutoHeight = true;

                    gv.Appearance.HeaderPanel.Font =
                        new Font("Tahoma", 9F, FontStyle.Bold);

                    gv.Appearance.HeaderPanel.TextOptions.HAlignment =
                        DevExpress.Utils.HorzAlignment.Center;

                    gv.Appearance.Row.Font =
                        new Font("Tahoma", 9F);

                    gv.Appearance.Row.TextOptions.VAlignment =
                        DevExpress.Utils.VertAlignment.Center;

                    gv.RowHeight = 28;

                    gc.ForceInitialize();

                    gv.PopulateColumns();

                    foreach (GridColumn col in gv.Columns)
                    {
                        col.OptionsColumn.AllowEdit = false;
                        col.OptionsColumn.AllowFocus = false;
                    }

                    gv.BestFitColumns();

                    if (gv.Columns["STT"] != null)
                    {
                        gv.Columns["STT"].MaxWidth = 50;
                        gv.Columns["STT"].MinWidth = 50;
                    }

                    if (gv.Columns["Item Code"] != null)
                    {
                        gv.Columns["Item Code"].MinWidth = 120;
                    }

                    if (gv.Columns["Mô Tả"] != null)
                    {
                        gv.Columns["Mô Tả"].MinWidth = 250;
                    }
                    if (gv.Columns["Đơn Vị"] != null)
                    {
                        gv.Columns["Đơn Vị"].MinWidth = 100;
                    }
                    if (gv.Columns["Khổ / Size"] != null)
                    {
                        gv.Columns["Khổ / Size"].MinWidth = 90;
                    }
                    if (gv.Columns["Mã Màu"] != null)
                    {
                        gv.Columns["Mã Màu"].MinWidth = 120;
                    }

                    if (gv.Columns["Màu Vật Tư"] != null)
                    {
                        gv.Columns["Màu Vật Tư"].MinWidth = 140;
                    }

                  
                    if (gv.Columns["Ghi Chú"] != null)
                    {
                        gv.Columns["Ghi Chú"].MinWidth = 250;
                    }

                    if (gv.Columns["Thời Gian"] != null)
                    {
                        gv.Columns["Thời Gian"].MinWidth = 90;
                    }

                    if (gv.Columns["Người Thực Hiện"] != null)
                    {
                        gv.Columns["Người Thực Hiện"].MinWidth = 140;
                    }

                    if (gv.Columns["Ngày Thực Hiện"] != null)
                    {
                        gv.Columns["Ngày Thực Hiện"].MinWidth = 130;
                    }

                    gv.BestFitColumns();
                }
                finally
                {
                    gv.EndUpdate();
                }

                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }
        private void searchLookUpEditLocCL_EditValueChanged(object sender, EventArgs e)
        {
            loadVatTu();
        }
        private void loadLocChungLoai()
        {
            try
            {
                string url = $"{URL}ERPThoiGianXaVai/GetChung?action=GETLOCCHUNGLOAI";

                string json = Task.Run(async () =>
                {
                    return await _clientExtension.GetAsnyc(url);
                }).Result;

                if (string.IsNullOrWhiteSpace(json) || json == "[]")
                    return;

                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);

                if (dt == null || dt.Rows.Count == 0)
                    return;

                searchLookUpEditLocCL.Properties.DataSource = dt;
                searchLookUpEditLocCL.Properties.ValueMember = "MaCLVT";
                searchLookUpEditLocCL.Properties.DisplayMember = "ChungLoaiVatTu";
                searchLookUpEditLocCL.Properties.NullText = "Lọc chủng loại";

                GridView view = searchLookUpEditLocCL.Properties.View as GridView;
                if (view != null && view.Columns.Count == 0)
                {
                    view.Columns.AddVisible("MaCLVT", "Mã");
                    view.Columns.AddVisible("ChungLoaiVatTu", "Chủng loại");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void txtFind_EditValueChanged(object sender, EventArgs e)
        {
            string searchText = txtFind.Text.Trim();

            FilterGridView(gVNL, searchText);

        }
        private void FilterGridView(DevExpress.XtraGrid.Views.Grid.GridView gridView, string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                gridView.ActiveFilterString = "";
                return;
            }

            List<string> conditions = new List<string>();

            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridView.Columns)
            {
                if (col.Visible)
                {

                    string escapedText = searchText.Replace("'", "''");
                    conditions.Add($"Contains([{col.FieldName}], '{escapedText}')");
                }
            }

            if (conditions.Count > 0)
            {
                gridView.ActiveFilterString = string.Join(" OR ", conditions);
            }
        }
        private void CheckPerminsion()
        {
            try
            {
                string url = string.Format("{0}/GetPer?userID={1}",
                    URL + ResourceURL.UrlUserModule,
                    GlobleData.UserName);

                List<SystemUserModuleEntity> list =
                    Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;

                SystemUserModuleEntity obj = list
                    .Where(m => m.FormShow == this.Name)
                    .FirstOrDefault();

                if (obj == null) return;

                _allowAdd = obj.AllowAdd;
                _allowEdit = obj.AllowEdit;
                _allowDelete = obj.AllowDelete;




                if (!_allowAdd)
                {
                    btnLuu.Enabled = false;
                }

                if (!_allowEdit)
                {
                    btnSua.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi phân quyền: " + ex.Message);
            }
        }



    }
}