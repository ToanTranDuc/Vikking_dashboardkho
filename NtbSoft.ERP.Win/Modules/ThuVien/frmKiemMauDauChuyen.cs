using DevExpress.Spreadsheet;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraPrinting;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using LicenseContext = OfficeOpenXml.LicenseContext;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using NtbSoft.ERP.Win.Modules.ResourceForm;
using DevExpress.XtraEditors;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmKiemMauDauChuyen : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        Guid gGuid = Guid.Empty;
        string gDauSizeID = "", gStyleID = "", gSizeID = "";
        int gStopRow = 0;
        string gNhanVien = "";
        DataTable gdtNhomKhacPhuc = new DataTable();
        private DataTable dtThongSoMH = new DataTable();
        public static string gLine_Link = "";
        public static string gLenhSX_Link = "";
        string gIsDuyet = "0";
        public frmKiemMauDauChuyen()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            grvHinh.CustomUnboundColumnData += GrvHinh_CustomUnboundColumnData;
            grcMaHangHinh.AllowDrop = true;
            grcMaHangHinh.DragDrop += GrcMaHangHinh_DragDrop;
            grcMaHangHinh.DragEnter += GrcMaHangHinh_DragEnter;
            grcMaHangHinh.DragOver += GrcMaHangHinh_DragOver;
            searchLookUpEdit_Line.EditValueChanged += SearchLookUpEdit_Line_EditValueChanged;
            searchLookUpEdit_MaHang.EditValueChanged += SearchLookUpEdit_MaHang_EditValueChanged;

            grvDanhMuc.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;

            //grvDetail_GopY.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
            //grvHinh.CustomDrawRowIndicator += gridView_CustomDrawRowIndicator;
            grvDanhMuc.CellValueChanging += grvDanhMuc_CellValueChanging;
            grvDetail_GopY.RowStyle += grvDetail_GopY_RowStyle;
            searchLookUpEdit_PO.EditValueChanged += SearchLookUpEdit_PO_EditValueChanged;
            searchLookUpEdit_Inseam.EditValueChanged += SearchLookUpEdit_Inseam_EditValueChanged;
            searchLookUpEdit_Color.EditValueChanged += SearchLookUpEdit_Color_EditValueChanged;

            grvDetail_GopY.CustomDrawGroupRow += GrvDetail_GopY_CustomDrawGroupRow;
            LoadNhomKhacPhucToRepository();
            LoadMaLoiToRepository();

            grvDetail_GopY.CustomColumnSort += (s, e) =>
            {
                if (e.Column.FieldName == "TenNhom")
                {
                    var view = s as DevExpress.XtraGrid.Views.Grid.GridView;

                    var row1 = view.GetRow(e.ListSourceRowIndex1);
                    var row2 = view.GetRow(e.ListSourceRowIndex2);

                    int sort1 = Convert.ToInt32(view.GetListSourceRowCellValue(e.ListSourceRowIndex1, "MaNhom"));
                    int sort2 = Convert.ToInt32(view.GetListSourceRowCellValue(e.ListSourceRowIndex2, "MaNhom"));

                    e.Result = sort1.CompareTo(sort2);
                    e.Handled = true;
                }
            };
            grvDanhMuc.OptionsSelection.MultiSelect = true;
            grvDanhMuc.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;

            grvDetail_GopY.OptionsSelection.MultiSelect = true;
            grvDetail_GopY.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            repoDuyet.MouseDown += RepoDuyet_MouseDown;
            btnDuyet.ShowingEditor += BtnDuyet_ShowingEditor;

            grvDanhMuc.IndicatorWidth = 40;
        }

        private void BtnDuyet_ShowingEditor(object sender, DevExpress.XtraBars.ItemCancelEventArgs e)
        {
            if (gGuid == Guid.Empty)
            {
                e.Cancel = true;
                return;
            }
            if (_allowDelete || gIsDuyet == "0")
            {

            }
            else
            {
                e.Cancel = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Init();
            CheckPerminsion();
            LoadLine();
            //txtNguoiTao.Text = GlobleData.NhanVien;           
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == "frmTongQuanKiemDauChuyen"
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                Them.Enabled = false;
            }
            //if (!_allowEdit)
            //{
            //    btnSave.Enabled = false;
            //}
            //else
            //{
            //    btnSave.Enabled = false;
            //}
            if (!_allowDelete)
                btnDeleteAll.Enabled = false;

        }
        private void LockDuyet(bool IsLocked = false)
        {
            if (IsLocked)
            {
                btnAddDanhMuc.Enabled = false;
                btnAddDetail.Enabled = false;
                btnAddImage.Enabled = false;

                btnSave.Enabled = false;
                btnDeleteAll.Enabled = false;
                btnSaveImage.Enabled = false;
                btnDeleteDanhMuc.Enabled = false;
                btnDeleteDetail.Enabled = false;
                btnDeleteImage.Enabled = false;
                grvDanhMuc.OptionsBehavior.Editable = false;
                grvDetail_GopY.OptionsBehavior.Editable = false;
                bandedGridViewNhapThongSo.OptionsBehavior.Editable = false;
                searchLookUpEdit_Size.Enabled = false;
            }
            else
            {
                btnAddDanhMuc.Enabled = true;
                btnAddDetail.Enabled = true;
                btnAddImage.Enabled = true;

                btnSave.Enabled = true;
                btnDeleteAll.Enabled = true;
                btnSaveImage.Enabled = true;
                btnDeleteDanhMuc.Enabled = true;
                btnDeleteDetail.Enabled = true;
                btnDeleteImage.Enabled = true;
                grvDanhMuc.OptionsBehavior.Editable = true;
                grvDetail_GopY.OptionsBehavior.Editable = true;
                bandedGridViewNhapThongSo.OptionsBehavior.Editable = true;
                searchLookUpEdit_Size.Enabled = true;
            }

        }
        private void RepoDuyet_MouseDown(object sender, MouseEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                var editor = sender as ToggleSwitch;
                if (gGuid == Guid.Empty) return;
                bool current = editor.IsOn;
                bool next = !current;
                string mess = next
                    ? "Bạn có chắc muốn xác nhận duyệt mẫu không?"
                    : "Bạn có chắc muốn hủy duyệt mẫu không?";
                if (MessageBox.Show(mess, "Thông báo",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    editor.IsOn = next;
                    var statusDuyet = next ? "1" : "0";
                    string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/PostDuyet?action=PostDuyet&&para1={gGuid}&&para2={statusDuyet}");
                    var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
                    if (msResult.ToUpper() == "TRUE")
                    {
                        LockDuyet(next);
                        btnDuyet.EditValue = next;
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    }
                }
            }));

        }
        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            KHDongThungLib.CustomDrawRowIndicator(sender, e);
        }
        private void SearchLookUpEdit_Line_EditValueChanged(object sender, EventArgs e)
        {
            LoadLenhSX();
        }
        private void SearchLookUpEdit_MaHang_EditValueChanged(object sender, EventArgs e)
        {
            gGuid = Guid.Empty;
            // Reset các control phía sau
            searchLookUpEdit_PO.EditValue = null;
            searchLookUpEdit_Inseam.EditValue = null;
            searchLookUpEdit_Color.EditValue = null;
            //searchLookUpEdit_Size.Text = null;
            searchLookUpEdit_Size.EditValue = null;
            searchLookUpEdit_Size.Text = "";
            gSizeID = "";
            sizeDisplay = "";
            var view = searchLookUpEdit_Size.Properties.View;
            view.ClearSelection();
            searchLookUpEdit_Size.Properties.DataSource = null;
            txtbox_Soluong.Text = "";
            txtGhiChu.Text = "";
            dateEdit1.EditValue = DateTime.Now;
            var dtSourceMH = searchLookUpEdit_MaHang.Properties.DataSource as DataTable;
            var drKH = dtSourceMH.AsEnumerable().Where(x => x["LenhSX"].ToString() == searchLookUpEdit_MaHang.EditValue.ToString()).FirstOrDefault();
            txtKhachHang.Text = drKH["KhachHang"].ToString();
            LoadPO();
            LoadGuid();
        }
        private void SearchLookUpEdit_PO_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_PO.EditValue is null) return;
            searchLookUpEdit_Inseam.EditValue = null;
            searchLookUpEdit_Color.EditValue = null;

            LoadInseam();
        }
        private void SearchLookUpEdit_Inseam_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Inseam.EditValue is null) return;
            LoadColor();
        }
        private void SearchLookUpEdit_Color_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEdit_Color.EditValue is null) return;
            LoadSize();
        }
        private void CheckedComboBoxEdit_Size_EditValueChanged(object sender, EventArgs e)
        {
            var sizes = GetSelectedSizes();
            if (sizes.Count == 0) return;

            LoadSoLuong();

            if (gGuid == Guid.Empty)
                SaveKiemDauChuyen();
            else
                LoadGuid();
        }
        private List<string> GetSelectedSizes()
        {
            if (sizeTable == null) return new List<string>();
            return sizeTable.AsEnumerable()
                .Where(r => r["IsChon"] != DBNull.Value && (bool)r["IsChon"])
                .Select(r => r["SizeID"].ToString())
                .ToList();
        }
        private void LoadInseam()
        {
            if (searchLookUpEdit_PO.EditValue is null) return;
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Get?action=GetInseam&para1={searchLookUpEdit_MaHang.EditValue}&para2={searchLookUpEdit_PO.EditValue}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_Inseam.Properties.DataSource = dt;
            searchLookUpEdit_Inseam.Properties.DisplayMember = "DauSize";
            searchLookUpEdit_Inseam.Properties.ValueMember = "SizeTypeID";
            searchLookUpEdit_Inseam.EditValue = null;
            if (dt.Rows.Count == 1)
            {
                searchLookUpEdit_Inseam.EditValue = dt.Rows[0]["SizeTypeID"];
            }
        }
        private void LoadColor()
        {
            if (searchLookUpEdit_PO.EditValue is null) return;
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Get?action=GetColor&para1={searchLookUpEdit_MaHang.EditValue}&para2={searchLookUpEdit_PO.EditValue}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_Color.Properties.DataSource = dt;
            searchLookUpEdit_Color.Properties.DisplayMember = "TenMau";
            searchLookUpEdit_Color.Properties.ValueMember = "MaMau";
            searchLookUpEdit_Color.EditValue = null;
            if (dt.Rows.Count == 1)
            {
                searchLookUpEdit_Color.EditValue = dt.Rows[0]["MaMau"];
            }
        }
        private bool _preventCloseSize = false;
        private bool _suppressSizeChanged = false;
        private DataTable sizeTable;
        private string sizeDisplay = "";
        private void LoadSize()
        {
            if (searchLookUpEdit_Color.EditValue is null) return;
            string url = URL + $"QTY_KiemDauChuyen/Get?action=GetSize&para1={searchLookUpEdit_MaHang.EditValue}&para2={searchLookUpEdit_PO.EditValue}&para3={searchLookUpEdit_Color.EditValue}";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(json);

            // ✅ Thêm cột IsChon
            if (!dt.Columns.Contains("IsChon"))
                dt.Columns.Add("IsChon", typeof(bool));
            foreach (DataRow dr in dt.Rows)
                if (dr["IsChon"] == DBNull.Value) dr["IsChon"] = false;

            sizeTable = dt;

            // ✅ Setup SearchLookUpEdit_Size
            searchLookUpEdit_Size.Properties.DataSource = dt;
            searchLookUpEdit_Size.Properties.DisplayMember = "Size";
            searchLookUpEdit_Size.Properties.ValueMember = "SizeID";
            searchLookUpEdit_Size.Properties.PopulateViewColumns();

            var view = searchLookUpEdit_Size.Properties.View;
            view.OptionsBehavior.Editable = true;
            view.OptionsSelection.MultiSelect = false;
            foreach (GridColumn col in view.Columns)
                col.OptionsColumn.AllowEdit = false;

            // ✅ Cột checkbox IsChon
            var colIsChon = view.Columns["IsChon"];
            if (colIsChon != null)
            {
                colIsChon.Caption = "Chọn";
                colIsChon.Visible = true;
                colIsChon.VisibleIndex = 0;
                colIsChon.Width = 50;
                colIsChon.OptionsColumn.AllowEdit = true;
                colIsChon.OptionsColumn.FixedWidth = true;
                colIsChon.ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            }
            if (view.Columns["Size"] != null) { view.Columns["Size"].Caption = "Size"; view.Columns["Size"].VisibleIndex = 1; }
            if (view.Columns["SizeID"] != null) view.Columns["SizeID"].Visible = false;

            view.OptionsFind.AlwaysVisible = true;

            view.MouseDown -= SizeView_MouseDown;
            view.MouseDown += SizeView_MouseDown;

            searchLookUpEdit_Size.QueryCloseUp -= SearchLookUpEdit_Size_QueryCloseUp;
            searchLookUpEdit_Size.QueryCloseUp += SearchLookUpEdit_Size_QueryCloseUp;

            searchLookUpEdit_Size.CustomDisplayText -= SearchLookUpEdit_Size_CustomDisplayText;
            searchLookUpEdit_Size.CustomDisplayText += SearchLookUpEdit_Size_CustomDisplayText;

            searchLookUpEdit_Size.CloseUp -= searchLookUpEdit_Size_CloseUp;
            searchLookUpEdit_Size.CloseUp += searchLookUpEdit_Size_CloseUp;

            searchLookUpEdit_Size.EditValue = null;
            sizeDisplay = "";
        }
        private void SizeView_MouseDown(object sender, MouseEventArgs ex)
        {
            var view = sender as GridView;
            var hi = view.CalcHitInfo(ex.Location);
            if (ex.Button == MouseButtons.Left
                && hi.Column != null && hi.Column.FieldName == "IsChon"
                && hi.RowHandle >= 0)
            {
                _preventCloseSize = true;
                var row = view.GetDataRow(hi.RowHandle);
                if (row != null)
                {
                    bool current = row["IsChon"] != DBNull.Value && (bool)row["IsChon"];
                    row["IsChon"] = !current;
                    view.RefreshData();
                }
            }
        }
        private void SearchLookUpEdit_Size_QueryCloseUp(object sender,
        System.ComponentModel.CancelEventArgs ex)
        {
            if (_preventCloseSize) { ex.Cancel = true; _preventCloseSize = false; }
        }
        private void SearchLookUpEdit_Size_CustomDisplayText(object sender,
            DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs ex)
        {
            ex.DisplayText = sizeDisplay;
        }
        private void searchLookUpEdit_Size_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e)
        {
            if (sizeTable == null) return;

            var checkedRows = sizeTable.AsEnumerable()
                .Where(r => r["IsChon"] != DBNull.Value && (bool)r["IsChon"])
                .ToList();

            var sizeIDs = checkedRows.Select(r => r["SizeID"].ToString()).ToList();
            var sizeNames = checkedRows.Select(r => r["Size"].ToString()).ToList();

            sizeDisplay = string.Join(", ", sizeNames);
            gSizeID = string.Join(",", sizeIDs);

            _suppressSizeChanged = true;
            searchLookUpEdit_Size.EditValue = sizeDisplay;
            _suppressSizeChanged = false;

            if (sizeIDs.Count > 0)
            {
                LoadSoLuong();
                if (gGuid == Guid.Empty)
                    SaveKiemDauChuyen();
                else
                {
                    // ✅ Không gọi LoadGuid() vì nó sẽ ghi đè gSizeID
                    SaveHeaderInfo();   // lưu ngay với gSizeID mới
                    LoadAllData();      // reload data
                }
            }
        }
        private void LoadSoLuong()
        {
            //if (sizeTable == null || sizeTable.Rows.Count == 0)
            //{
            //    txtbox_Soluong.Text = "0";
            //    return;
            //}

            //try
            //{
            //    int totalQty = sizeTable.AsEnumerable()
            //        .Where(r => r.Table.Columns.Contains("IsChon") && r["IsChon"] != DBNull.Value && (bool)r["IsChon"])
            //        .Sum(r =>
            //        {
            //            if (!r.Table.Columns.Contains("SoLuong") || r["SoLuong"] == DBNull.Value)
            //                return 0;

            //            int qty = 0;
            //            int.TryParse(r["SoLuong"].ToString(), out qty);
            //            return qty;
            //        });

            //    txtbox_Soluong.Text = totalQty.ToString();
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("LoadSoLuong error: " + ex.Message);
            //    txtbox_Soluong.Text = "0";
            //}
        }

        private void Init()
        {
            searchLookUpEdit_Line.Properties.DisplayMember = "Name";
            searchLookUpEdit_Line.Properties.ValueMember = "LineX";

            searchLookUpEdit_MaHang.Properties.DisplayMember = "MaHangDisplay";
            searchLookUpEdit_MaHang.Properties.ValueMember = "LenhSX";


            var poView = searchLookUpEdit_PO.Properties.View;
            poView.Columns.Clear();
            //var colPOID = poView.Columns.AddField("POID");
            //colPOID.Caption = "POID"; colPOID.Visible = true; colPOID.VisibleIndex = 0;
            var colPO = poView.Columns.AddField("PO");
            colPO.Caption = "PO"; colPO.Visible = true; colPO.VisibleIndex = 0;

            gridColumn22.FieldName = "MaNhom";
            gridColumn22.Caption = "Nhóm";
            gridColumn22.ColumnEdit = repositoryItemSearchLookUpEdit1;

            // Trong Init()
            grvDetail_GopY.OptionsView.ShowGroupPanel = false;
            grvDetail_GopY.OptionsView.GroupFooterShowMode =
                DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
            grvDetail_GopY.OptionsView.ShowGroupedColumns = false;
            grvDetail_GopY.OptionsBehavior.AutoExpandAllGroups = false;
            grvDetail_GopY.OptionsCustomization.AllowGroup = true;
            grvDetail_GopY.GroupPanelText = "Kéo cột vào đây để nhóm";

            var inseamView = searchLookUpEdit_Inseam.Properties.View;
            inseamView.Columns.Clear();
            var colSizeTypeID = inseamView.Columns.AddField("SizeTypeID");
            colSizeTypeID.Visible = false;  // ✅ ẩn
            var colDauSize = inseamView.Columns.AddField("DauSize");
            colDauSize.Caption = "Đầu Size";
            colDauSize.Visible = true;
            colDauSize.VisibleIndex = 0;
            colDauSize.Width = 200;

            // ✅ Ẩn cột MaMau, chỉ hiện TenMau
            var colorView = searchLookUpEdit_Color.Properties.View;
            colorView.Columns.Clear();
            var colMaMau = colorView.Columns.AddField("MaMau");
            colMaMau.Visible = false;  // ✅ ẩn
            var colTenMau = colorView.Columns.AddField("TenMau");
            colTenMau.Caption = "Tên Màu";
            colTenMau.Visible = true;
            colTenMau.VisibleIndex = 0;
            colTenMau.Width = 200;
        }
        private void LoadLine()
        {
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Get?action=GetLine");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_Line.Properties.DataSource = dt;
            if (gLine_Link != "")
            {
                searchLookUpEdit_Line.EditValue = gLine_Link;
            }
        }
        private void LoadLenhSX()
        {
            if (searchLookUpEdit_Line.EditValue is null) return;
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Get?action=GetLenh&para1={searchLookUpEdit_Line.EditValue.ToString()}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_MaHang.Properties.DataSource = dt;
            if (gLenhSX_Link != "")
            {
                searchLookUpEdit_MaHang.EditValue = gLenhSX_Link;
            }
        }
        private void LoadPO()
        {
            if (searchLookUpEdit_MaHang.EditValue is null) return;
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Get?action=GetPO&para1={searchLookUpEdit_MaHang.EditValue.ToString()}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit_PO.Properties.DataSource = dt;

            searchLookUpEdit_PO.Properties.DisplayMember = "PO";
            searchLookUpEdit_PO.Properties.ValueMember = "POID";
            if (dt.Rows.Count == 1)
            {
                searchLookUpEdit_PO.EditValue = null;
                searchLookUpEdit_PO.EditValue = dt.Rows[0]["POID"];
            }
        }
        private void LoadGuid()
        {
            if (searchLookUpEdit_Line.EditValue is null) return;
            if (searchLookUpEdit_MaHang.EditValue is null) return;

            string url = URL + $"QTY_KiemDauChuyen/Get?action=GetGuid" +
                         $"&para1={searchLookUpEdit_Line.EditValue}" +
                         $"&para2={searchLookUpEdit_MaHang.EditValue}";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(json);

            if (dt != null && dt.Rows.Count > 0)
            {
                var dr = dt.Rows[0];
                gGuid = Guid.Parse(dr["Guid"].ToString());
                gDauSizeID = dr["DauSizeID"].ToString();
                gStyleID = dr["StyleID"].ToString();
                gSizeID = dr["SizeID"].ToString();
                txtbox_Soluong.Text = dr["SoLuongKiem"].ToString() == "0" ? "1" : dr["SoLuongKiem"].ToString();
                // ✅ Dùng đúng tên control trên form
                txtNguoiTao.Text = dr["CreateUser"]?.ToString() == "" ? GlobleData.NhanVien : dr["CreateUser"]?.ToString();
                txtKTTK.Text = dr["TechnicalStaff"]?.ToString() ?? "";
                txtQC.Text = dr["QCInLine"]?.ToString() ?? "";
                txtLineManager.Text = dr["LineManager"]?.ToString() ?? "";
                txtGhiChu.Text = dr["Note"]?.ToString() ?? "";
                if (dr["CreateDate"] != DBNull.Value && dr["CreateDate"]?.ToString() != "")
                    dateEdit1.DateTime = Convert.ToDateTime(dr["CreateDate"]);
                else
                    dateEdit1.DateTime = DateTime.Now;

                var StatusDuyet = dr["IsDuyet"]?.ToString() ?? "0";
                gIsDuyet = StatusDuyet;
                if (StatusDuyet == "1")
                {
                    LockDuyet(true);
                    btnDuyet.EditValue = true;
                }
                else
                {
                    btnDuyet.EditValue = false;
                }



                RestoreUIFromDB(dr);
                LoadAllData();
            }
            else
            {
                LockDuyet(false);
                txtbox_Soluong.Text = "1";
                txtNguoiTao.Text = GlobleData.NhanVien;
                txtNguoiTao.Text = "";
                txtKTTK.Text = "";
                txtQC.Text = "";
                txtLineManager.Text = "";
                dateEdit1.DateTime = DateTime.Now;

                grcDanhMuc.DataSource = CreateTblDanhMuc();
                dgrNhapThongSo.DataSource = null;
                grcDetail_GopYKhacPhuc.DataSource = CreateTblDetail();
                grcMaHangHinh.DataSource = CreateTblImage();
            }
        }
        private void LoadNhomKhacPhucToRepository()
        {
            try
            {
                string url = URL + "QTY_KiemDauChuyen/Get?action=GetNhomKhacPhuc";
                string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                var dt = JsonConvert.DeserializeObject<DataTable>(json);

                if (dt == null)
                {
                    dt = new DataTable();
                    dt.Columns.Add("ID", typeof(int));
                    dt.Columns.Add("MaNKP", typeof(string));
                    dt.Columns.Add("NhomKhacPhuc", typeof(string));
                    dt.Columns.Add("NguoiTao", typeof(string));
                    dt.Columns.Add("NguoiSua", typeof(string));
                    dt.Columns.Add("Sort", typeof(int));
                }

                if (!dt.Columns.Contains("ID"))
                    dt.Columns.Add("ID", typeof(int));
                if (!dt.Columns.Contains("NhomKhacPhuc"))
                    dt.Columns.Add("NhomKhacPhuc", typeof(string));

                // ✅ Setup cho SearchLookUpEdit thay vì LookUpEdit
                var view = repositoryItemSearchLookUpEdit1.View;
                view.Columns.Clear();

                var colID = view.Columns.AddField("ID");
                colID.Caption = "ID";
                colID.Visible = true;
                colID.VisibleIndex = 0;
                colID.Width = 60;

                var colNhom = view.Columns.AddField("NhomKhacPhuc");
                colNhom.Caption = "Nhóm KP";
                colNhom.Visible = true;
                colNhom.VisibleIndex = 1;
                colNhom.Width = 220;

                repositoryItemSearchLookUpEdit1.DataSource = dt;
                repositoryItemSearchLookUpEdit1.ValueMember = "ID";
                repositoryItemSearchLookUpEdit1.DisplayMember = "NhomKhacPhuc";
                repositoryItemSearchLookUpEdit1.NullText = "";
                repositoryItemSearchLookUpEdit1.View.OptionsView.ShowGroupPanel = false;
                gdtNhomKhacPhuc = dt;
                repositoryItemSearchLookUpEdit1.EditValueChanged -= RepositoryItemSearchLookUpEdit1_EditValueChanged;
                repositoryItemSearchLookUpEdit1.EditValueChanged += RepositoryItemSearchLookUpEdit1_EditValueChanged;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadNhomKhacPhucToRepository error: " + ex.Message);
            }
        }
        private void LoadMaLoiToRepository()
        {
            try
            {
                string url = URL + "QTY_KiemDauChuyen/Get?action=GetMaLoi";
                string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                var dt = JsonConvert.DeserializeObject<DataTable>(json);

                if (dt == null) dt = new DataTable();
                if (!dt.Columns.Contains("MaLoi")) dt.Columns.Add("MaLoi", typeof(string));
                if (!dt.Columns.Contains("TenLoi")) dt.Columns.Add("TenLoi", typeof(string));
                if (!dt.Columns.Contains("LoaiLoi")) dt.Columns.Add("LoaiLoi", typeof(int));

                var view = repositoryItemSearchLookUpEdit2.View;
                view.Columns.Clear();

                var colMa = view.Columns.AddField("MaLoi");
                colMa.Caption = "Mã Lỗi";
                colMa.Visible = true;
                colMa.VisibleIndex = 0;
                colMa.Width = 80;

                var colTen = view.Columns.AddField("TenLoi");
                colTen.Caption = "Tên Lỗi";
                colTen.Visible = true;
                colTen.VisibleIndex = 1;
                colTen.Width = 280;

                repositoryItemSearchLookUpEdit2.DataSource = dt;
                repositoryItemSearchLookUpEdit2.ValueMember = "MaLoi";
                repositoryItemSearchLookUpEdit2.DisplayMember = "TenLoi";
                repositoryItemSearchLookUpEdit2.NullText = "";
                repositoryItemSearchLookUpEdit2.View.OptionsView.ShowGroupPanel = false;

                repositoryItemSearchLookUpEdit2.EditValueChanged -= RepositoryItemSearchLookUpEdit2_EditValueChanged;
                repositoryItemSearchLookUpEdit2.EditValueChanged += RepositoryItemSearchLookUpEdit2_EditValueChanged;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadMaLoiToRepository error: " + ex.Message);
            }
        }

        private void RepositoryItemSearchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
            var editor = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (editor == null) return;

            var selectedMaLoi = editor.EditValue?.ToString();
            if (string.IsNullOrEmpty(selectedMaLoi)) return;

            var dt = repositoryItemSearchLookUpEdit2.DataSource as DataTable;
            if (dt == null) return;

            var dr = dt.AsEnumerable()
                       .FirstOrDefault(x => x["MaLoi"]?.ToString() == selectedMaLoi);
            if (dr == null) return;

            var focusedRow = grvDetail_GopY.GetFocusedDataRow();
            if (focusedRow == null) return;

            //focusedRow["MaLoi"] = selectedMaLoi;

            int loaiLoi = 0;
            int.TryParse(dr["LoaiLoi"]?.ToString(), out loaiLoi);

            if (loaiLoi == 3)
            {
                focusedRow["LoiNang"] = false;
                focusedRow["LoiNhe"] = true;
                focusedRow["MucDoLoi"] = 0;
            }
            else if (loaiLoi == 2)
            {
                focusedRow["LoiNang"] = true;
                focusedRow["LoiNhe"] = false;
                focusedRow["MucDoLoi"] = 1;
            }
            grvDetail_GopY.RefreshData();
        }

        private void RepositoryItemSearchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            var editor = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (editor == null) return;

            var selectedID = editor.EditValue;
            if (selectedID == null || selectedID == DBNull.Value) return;

            var dt = repositoryItemSearchLookUpEdit1.DataSource as DataTable;
            if (dt == null) return;

            var dr = dt.AsEnumerable()
                       .FirstOrDefault(x => x["ID"]?.ToString() == selectedID.ToString());
            if (dr == null) return;

            var focusedRow = grvDetail_GopY.GetFocusedDataRow();
            if (focusedRow != null)
            {
                focusedRow["MaNhom"] = Convert.ToInt32(selectedID);
                focusedRow["TenNhom"] = dr["NhomKhacPhuc"]?.ToString() ?? "";
            }
            grvDetail_GopY.PostEditor();
            grvDetail_GopY.UpdateCurrentRow();
            grvDetail_GopY.RefreshData();
            grvDetail_GopY.ExpandAllGroups();
        }
        private void GrvDetail_GopY_CustomDrawGroupRow(object sender,
       RowObjectCustomDrawEventArgs e)
        {
            var groupRow = e.Info as GridGroupRowInfo;
            if (groupRow == null) return;
            if (groupRow.Column == null) return;

            string fullText = groupRow.GroupText ?? "";
            int colonIndex = fullText.IndexOf(':');
            if (colonIndex >= 0)
                groupRow.GroupText = fullText.Substring(colonIndex + 1).Trim();
            else
                groupRow.GroupText = string.IsNullOrEmpty(fullText) ? "(Chưa phân nhóm)" : fullText;
        }
        private void RepositoryItemLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            var editor = sender as DevExpress.XtraEditors.LookUpEdit;
            if (editor == null) return;

            var dt = repositoryItemLookUpEdit1.DataSource as DataTable;
            if (dt == null) return;

            var selectedID = editor.EditValue;
            if (selectedID == null || selectedID == DBNull.Value) return;

            var dr = dt.AsEnumerable()
                       .FirstOrDefault(x => x["ID"]?.ToString() == selectedID.ToString());

            if (dr == null) return;
            var focusedRow = grvDetail_GopY.GetFocusedDataRow();
            if (focusedRow != null)
            {
                focusedRow["MaNhom"] = Convert.ToInt32(selectedID);
                focusedRow["TenNhom"] = dr["NhomKhacPhuc"]?.ToString() ?? "";
            }
        }
        private void RestoreUIFromDB(DataRow dr)
        {
            searchLookUpEdit_PO.EditValueChanged -= SearchLookUpEdit_PO_EditValueChanged;
            searchLookUpEdit_Inseam.EditValueChanged -= SearchLookUpEdit_Inseam_EditValueChanged;
            searchLookUpEdit_Color.EditValueChanged -= SearchLookUpEdit_Color_EditValueChanged;

            try
            {
                if (searchLookUpEdit_PO.EditValue == null)
                {
                    LoadPO();
                    string poid = dr["POID"]?.ToString();
                    if (!string.IsNullOrEmpty(poid))
                        searchLookUpEdit_PO.EditValue = poid;
                }
                if (searchLookUpEdit_Inseam.EditValue == null)
                {
                    LoadInseam();
                    string dauSizeID = dr["DauSizeID"]?.ToString();
                    if (!string.IsNullOrEmpty(dauSizeID))
                        searchLookUpEdit_Inseam.EditValue = dauSizeID;
                }
                if (searchLookUpEdit_Color.EditValue == null)
                {
                    LoadColor();
                    string maMau = dr["MaMau"]?.ToString();
                    if (!string.IsNullOrEmpty(maMau))
                        searchLookUpEdit_Color.EditValue = maMau;
                }

                //if (GetSelectedSizes().Count == 0)
                {
                    LoadSize();
                    string sizeID = dr["SizeID"]?.ToString();
                    if (!string.IsNullOrEmpty(sizeID) && sizeTable != null)
                    {
                        var sizeList = sizeID.Split(',').Select(s => s.Trim()).ToList();
                        var names = new List<string>();
                        foreach (DataRow r in sizeTable.Rows)
                        {
                            bool isChecked = sizeList.Contains(r["SizeID"].ToString());
                            r["IsChon"] = isChecked;
                            if (isChecked) names.Add(r["Size"].ToString());
                        }
                        sizeDisplay = string.Join(", ", names);
                        gSizeID = sizeID;
                        _suppressSizeChanged = true;
                        searchLookUpEdit_Size.EditValue = sizeDisplay;
                        _suppressSizeChanged = false;
                    }
                }
                LoadSoLuong();
            }
            finally
            {
                searchLookUpEdit_PO.EditValueChanged += SearchLookUpEdit_PO_EditValueChanged;
                searchLookUpEdit_Inseam.EditValueChanged += SearchLookUpEdit_Inseam_EditValueChanged;
                searchLookUpEdit_Color.EditValueChanged += SearchLookUpEdit_Color_EditValueChanged;

            }
        }
        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // ✅ Reload cả repository Nhóm KP
            LoadNhomKhacPhucToRepository();
            LoadMaLoiToRepository();
            LoadThongSoMH();
            LoadThongSo();
            LoadDetail_GopYKhacPhuc();
            LoadDanhMuc();
        }
        private void LoadAllData()
        {
            LoadThongSoMH();
            LoadThongSo();
            LoadDetail_GopYKhacPhuc();
            LoadDanhMuc();
        }
        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            SaveHeaderInfo();
            SaveDanhMuc_Button();
            SaveDetail_Button();
            SaveThongSo();
            SaveHinh();
        }
        private void SaveHeaderInfo()
        {
            if (gGuid == Guid.Empty) return;

            var dtSave = CreateTblKiemDauChuyen();
            var drSave = dtSave.NewRow();
            drSave["ID"] = 0;
            drSave["Line"] = searchLookUpEdit_Line.EditValue?.ToString() ?? "";
            drSave["LenhSX"] = searchLookUpEdit_MaHang.EditValue?.ToString() ?? "";
            drSave["MaHang"] = GlobleData.UserName;
            drSave["POID"] = searchLookUpEdit_PO.EditValue?.ToString() ?? "";
            drSave["PO"] = searchLookUpEdit_PO.Text ?? "";
            drSave["DauSizeID"] = searchLookUpEdit_Inseam.EditValue?.ToString() ?? "";
            drSave["DauSize"] = searchLookUpEdit_Inseam.Text ?? "";
            drSave["MaMau"] = searchLookUpEdit_Color.EditValue?.ToString() ?? "";
            drSave["TenMau"] = searchLookUpEdit_Color.Text ?? "";
            drSave["SizeID"] = gSizeID;
            drSave["Size"] = sizeDisplay;
            drSave["SoLuongKiem"] = txtbox_Soluong.Text;
            drSave["Guid"] = gGuid;
            drSave["CreateUser"] = txtNguoiTao.Text?.Trim();
            drSave["TechnicalStaff"] = txtKTTK.Text?.Trim();
            drSave["QCInLine"] = txtQC.Text?.Trim();
            drSave["LineManager"] = txtLineManager.Text?.Trim();
            drSave["CreateDate"] = dateEdit1.DateTime.Date;
            drSave["Note"] = txtGhiChu.Text;
            dtSave.Rows.Add(drSave);

            string url = URL + "QTY_KiemDauChuyen/Post?action=Post";
            Task.Run(async () => await _clientExtension.PostAsync(url, dtSave)).Wait();
        }

        #region Danh mục
        private void SaveDanhMuc_Button()
        {
            grvDanhMuc.CloseEditor();
            grvDanhMuc.UpdateCurrentRow();

            this.ActiveControl = grcInSeam;
            var dtSource = grcDanhMuc.DataSource as DataTable;
            if (dtSource == null || dtSource.Rows.Count == 0) return;

            var dtSave = CreateTblDanhMuc();
            if (dtSave.Columns.Count == 0) return;

            int indexDanhMuc = 1;
            foreach (DataRow dr in dtSource.Rows)
            {
                var drSave = dtSave.NewRow();
                drSave["ID"] = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                drSave["ID_Link"] = dr["ID_Link"]?.ToString() ?? gGuid.ToString();
                drSave["Code"] = dr["Code"]?.ToString() ?? "";
                drSave["Note"] = dr["Note"]?.ToString() ?? "";
                drSave["Value"] = dr["Value"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Value"]);
                drSave["Sort"] = indexDanhMuc;
                dtSave.Rows.Add(drSave);
                indexDanhMuc++;
            }

            string url = URL + "QTY_KiemDauChuyen/Post?action=PostDanhMucV2";
            var msResult = Task.Run(async () =>
                await _clientExtension.PostAsync(url, dtSave)).Result;

            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadDanhMuc();
            }
        }
        private bool _isUpdatingCheckbox = false;

        private void grvDanhMuc_CellValueChanging(object sender,
     DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (_isUpdatingCheckbox) return;

            var view = sender as GridView;

            // 🔥 QUAN TRỌNG: commit ngay
            //view.PostEditor();

            var checkCols = new[] { "Col_Star", "Col_Delta", "Col_Check", "Col_X" };
            if (!checkCols.Contains(e.Column.FieldName)) return;
            if (e.Value == null || !(bool)e.Value) return;

            _isUpdatingCheckbox = true;
            try
            {
                foreach (var col in checkCols)
                {
                    if (col != e.Column.FieldName)
                        view.SetRowCellValue(e.RowHandle, col, false);
                }

                int newValue = e.Column.FieldName == "Col_Star" ? 0 :
                               e.Column.FieldName == "Col_Delta" ? 1 :
                               e.Column.FieldName == "Col_Check" ? 2 :
                               e.Column.FieldName == "Col_X" ? 3 : -1;

                var dr = view.GetDataRow(e.RowHandle);
                if (dr != null)
                    dr["Value"] = newValue;
            }
            finally
            {
                _isUpdatingCheckbox = false;

                // 🔥 Force refresh UI
                //view.RefreshRow(e.RowHandle);
            }
        }
        private void grvDanhMuc_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            var view = sender as GridView;

            if (e.Column.FieldName == "Col_Check")
            {
                view.RefreshRow(e.RowHandle);
            }
        }
        private void grcDanhMuc_Click(object sender, EventArgs e)
        {

        }
        //private void btnAddDanhMuc_Click(object sender, EventArgs e)
        //{
        //    var dtSource = grcDanhMuc.DataSource as DataTable;
        //    if (gGuid == Guid.Empty)
        //    {
        //        return;
        //    }
        //    var drFocus = grvDanhMuc.GetFocusedDataRow();
        //    var drNew = dtSource.NewRow();
        //    drNew["ID"] = 0;
        //    drNew["ID_Link"] = gGuid;

        //    var index = 0;
        //    if (dtSource.Rows.Count > 0) index = dtSource.Rows.IndexOf(drFocus);

        //    dtSource.Rows.InsertAt(drNew, index + 1);
        //    grvDanhMuc.FocusedRowHandle = index + 1;
        //}

        private void SaveKiemDauChuyen()
        {
            if (searchLookUpEdit_Line.EditValue is null) return;
            if (searchLookUpEdit_MaHang.EditValue is null) return;

            var selectedSizes = GetSelectedSizes();
            if (selectedSizes.Count == 0) return;

            var sizeTong = string.Join(",", selectedSizes);
            gGuid = gGuid == Guid.Empty ? Guid.NewGuid() : gGuid;

            DataTable dtSave = CreateTblKiemDauChuyen();
            var drSave = dtSave.NewRow();
            drSave["ID"] = 0;
            drSave["Line"] = searchLookUpEdit_Line.EditValue?.ToString() ?? "";
            drSave["LenhSX"] = searchLookUpEdit_MaHang.EditValue;
            drSave["MaHang"] = "";
            drSave["POID"] = searchLookUpEdit_PO.EditValue?.ToString() ?? "";
            drSave["PO"] = searchLookUpEdit_PO.Text ?? "";
            drSave["DauSizeID"] = searchLookUpEdit_Inseam.EditValue?.ToString() ?? "";
            drSave["DauSize"] = searchLookUpEdit_Inseam.Text ?? "";
            drSave["MaMau"] = searchLookUpEdit_Color.EditValue?.ToString() ?? "";
            drSave["TenMau"] = searchLookUpEdit_Color.Text ?? "";
            drSave["SizeID"] = sizeTong;
            drSave["Size"] = sizeDisplay;
            drSave["SoLuongKiem"] = txtbox_Soluong.Text;
            drSave["Guid"] = gGuid;
            drSave["QCInLine"] = GlobleData.UserName;
            drSave["CreateUser"] = txtNguoiTao.Text?.Trim();
            drSave["CreateDate"] = dateEdit1.DateTime.Date;
            dtSave.Rows.Add(drSave);

            System.Diagnostics.Debug.WriteLine($"CreateUser: {drSave["CreateUser"]}, CreateDate: {drSave["CreateDate"]}");
            string url = URL + "QTY_KiemDauChuyen/Post?action=Post";
            var msResult = Task.Run(async () => await _clientExtension.PostAsync(url, dtSave)).Result;

            if (msResult.ToUpper() == "TRUE")
            {
                gDauSizeID = searchLookUpEdit_Inseam.EditValue?.ToString() ?? "";
                gSizeID = sizeTong;
                LoadGuid();
            }
            else
            {
                gGuid = Guid.Empty;
            }
        }
        private void btnAddDanhMuc_Click(object sender, EventArgs e)
        {
            if (gGuid == Guid.Empty)
            {
                SaveKiemDauChuyen();
                if (gGuid == Guid.Empty) return;
            }

            var dtSource = grcDanhMuc.DataSource as DataTable;

            if (dtSource == null || !dtSource.Columns.Contains("ID"))
            {
                dtSource = CreateTblDanhMuc();
                if (!dtSource.Columns.Contains("Col_Star")) dtSource.Columns.Add("Col_Star", typeof(bool));
                if (!dtSource.Columns.Contains("Col_Delta")) dtSource.Columns.Add("Col_Delta", typeof(bool));
                if (!dtSource.Columns.Contains("Col_Check")) dtSource.Columns.Add("Col_Check", typeof(bool));
                if (!dtSource.Columns.Contains("Col_X")) dtSource.Columns.Add("Col_X", typeof(bool));
                grcDanhMuc.DataSource = dtSource;
            }

            // ✅ Commit giá trị đang edit trước khi insert row mới
            grvDanhMuc.CloseEditor();
            grvDanhMuc.UpdateCurrentRow();

            grvDanhMuc.ActiveFilterString = string.Empty;

            var drFocus = grvDanhMuc.GetFocusedDataRow();
            var drNew = dtSource.NewRow();
            drNew["ID"] = 0;
            drNew["ID_Link"] = gGuid;
            if (dtSource.Columns.Contains("Col_Star")) drNew["Col_Star"] = false;
            if (dtSource.Columns.Contains("Col_Delta")) drNew["Col_Delta"] = false;
            if (dtSource.Columns.Contains("Col_Check")) drNew["Col_Check"] = false;
            if (dtSource.Columns.Contains("Col_X")) drNew["Col_X"] = false;

            var index = 0;
            if (dtSource.Rows.Count > 0 && drFocus != null)
                index = dtSource.Rows.IndexOf(drFocus);

            dtSource.Rows.InsertAt(drNew, index + 1);
            grvDanhMuc.FocusedRowHandle = index + 1;
        }
        private void btnDeleteDanhMuc_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa danh mục này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                int[] selectedRows = grvDanhMuc.GetSelectedRows();
                if (selectedRows.Length == 0) return;
                var dtSave = CreateTblDanhMuc();
                List<DataRow> rowsToDelete = new List<DataRow>();
                foreach (int rowHandle in selectedRows)
                {
                    DataRow dr = grvDanhMuc.GetDataRow(rowHandle);
                    var drNew = dtSave.NewRow();
                    if (dr["ID"].ToString() == "0")
                    {
                        rowsToDelete.Add(dr);
                        //var dtSource = grcDanhMuc.DataSource as DataTable;
                        //dtSource.Rows.Remove(dr);
                        continue;
                    }
                    drNew["ID"] = dr["ID"];
                    dtSave.Rows.Add(drNew);
                }
                foreach (var row in rowsToDelete)
                {
                    row.Delete(); // hoặc dtSource.Rows.Remove(row);
                }
                if (dtSave.Rows.Count == 0) return;

                string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Post?action=DeleteDanhMuc");
                var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadDanhMuc();
                }
            }
        }
        private void grvDanhMuc_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            var value = grvDanhMuc.GetRowCellValue(e.RowHandle, "Col_Check");
            var value1 = grvDanhMuc.GetRowCellValue(e.RowHandle, "Col_Star");
            var value2 = grvDanhMuc.GetRowCellValue(e.RowHandle, "Col_Delta");
            var value3 = grvDanhMuc.GetRowCellValue(e.RowHandle, "Col_X");

            bool isCheckedR = value != null && Convert.ToBoolean(value);
            bool isCheckedF = Convert.ToBoolean(value1) || Convert.ToBoolean(value2) || Convert.ToBoolean(value3);
            if (isCheckedR)
            {
                e.Appearance.ForeColor = Color.Black;
            }
            else if (isCheckedF)
            {
                e.Appearance.ForeColor = Color.Red;
            }

            e.HighPriority = true;

        }
        private void btnDanhMucLib_Click(object sender, EventArgs e)
        {
            frmKiemDauChuyen_DanhMucLib frm = new frmKiemDauChuyen_DanhMucLib();
            frm.ShowDialog();
        }
        #endregion

        #region Detail
        private void LoadDetail_GopYKhacPhuc()
        {
            string url = URL + "QTY_KiemDauChuyen/Get?action=GetDetail&para1=" + gGuid;
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(json);

            if (dt.Rows.Count == 0) dt = CreateTblDetail();
            AddUIColumns(dt);
            var forcusHandle = grvDetail_GopY.FocusedRowHandle;
            CheckGroupDetail(dt);
            grcDetail_GopYKhacPhuc.DataSource = dt;
            CalSTT();
            grvDetail_GopY.ExpandAllGroups();
            // ✅ Gọi sau khi đã bind xong
            //this.BeginInvoke(new Action(() => ApplyGrouping()));
            grvDetail_GopY.FocusedRowHandle = forcusHandle;
        }
        private void CheckGroupDetail(DataTable dt)
        {
            try
            {
                foreach (DataRow dr in gdtNhomKhacPhuc.Rows)
                {
                    var checkExist = dt.AsEnumerable().Any(x => x["MaNhom"].ToString() == dr["ID"].ToString());
                    if (!checkExist)
                    {
                        var drNew = dt.NewRow();
                        drNew["ID"] = 0;
                        drNew["ID_Link"] = gGuid.ToString();
                        drNew["STT"] = 1;
                        drNew["LoiNang"] = false;
                        drNew["LoiNhe"] = false;
                        drNew["BienPhap"] = "Điều chỉnh cho sản xuất";
                        drNew["MucDoLoi"] = 0;
                        drNew["Sort"] = 1;

                        drNew["MaNhom"] = dr["ID"];
                        drNew["TenNhom"] = dr["NhomKhacPhuc"];
                        drNew["MaLoi"] = "";
                        dt.Rows.Add(drNew);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void ApplyGrouping()
        {
            grvDetail_GopY.BeginUpdate();
            try
            {
                grvDetail_GopY.ClearGrouping();
                gridColumn22.Visible = true;
                gridColumn22.VisibleIndex = 1;
                gridColumn22.GroupIndex = 0;
                gridColumn22.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;

                grvDetail_GopY.OptionsView.ShowGroupedColumns = true;

                grvDetail_GopY.ExpandAllGroups();
            }
            finally
            {
                grvDetail_GopY.EndUpdate();
            }
        }
        private void SaveDetail_Button()
        {
            this.ActiveControl = grcInSeam;
            var dtSource = grcDetail_GopYKhacPhuc.DataSource as DataTable;
            if (dtSource == null || dtSource.Rows.Count == 0) return;

            var dtSave = CreateTblDetail();
            if (dtSave.Columns.Count == 0) return;

            int indexDetail = 1;
            foreach (DataRow dr in dtSource.Rows)
            {
                //if (dr["MaLoi"].ToString() == "") continue;
                var drSave = dtSave.NewRow();
                drSave["ID"] = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                drSave["ID_Link"] = dr["ID_Link"]?.ToString() ?? gGuid.ToString();
                drSave["STT"] = dr["STT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["STT"]);
                drSave["MoTa"] = dr["MoTa"]?.ToString() ?? "";
                drSave["BienPhap"] = dr["BienPhap"]?.ToString() ?? "";
                drSave["TheoDoi"] = dr["TheoDoi"]?.ToString() ?? "";
                drSave["Sign"] = dr["Sign"]?.ToString() ?? "";
                drSave["NgaySign"] = dr["NgaySign"] == DBNull.Value ? (object)DBNull.Value : dr["NgaySign"];
                drSave["NguoiSign"] = dr["NguoiSign"]?.ToString() ?? "";
                drSave["TenNhom"] = dr["TenNhom"]?.ToString() ?? "";
                drSave["MaLoi"] = dr.Table.Columns.Contains("MaLoi")
                    ? dr["MaLoi"]?.ToString() ?? ""
                    : "";
                bool loiNang = dtSource.Columns.Contains("LoiNang")
                               && dr["LoiNang"] != DBNull.Value
                               && Convert.ToBoolean(dr["LoiNang"]);
                drSave["MucDoLoi"] = loiNang ? 1 : 0;

                string maNhomStr = dr["MaNhom"]?.ToString();
                drSave["MaNhom"] = int.TryParse(maNhomStr, out int maNhomVal) ? maNhomVal : 0;

                drSave["Sort"] = indexDetail;
                dtSave.Rows.Add(drSave);
                indexDetail++;
            }

            string url = URL + $"QTY_KiemDauChuyen/Post?action=PostDetailV2";
            var msResult = Task.Run(async () => await _clientExtension.PostAsync(url, dtSave)).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadDetail_GopYKhacPhuc();
            }
        }
        private DataTable AddUIColumns(DataTable dt)
        {
            if (!dt.Columns.Contains("LoiNang"))
                dt.Columns.Add("LoiNang", typeof(bool));
            if (!dt.Columns.Contains("LoiNhe"))
                dt.Columns.Add("LoiNhe", typeof(bool));
            return dt;
        }
        private void btnAddDetail_Click(object sender, EventArgs e)
        {
            if (gGuid == Guid.Empty)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ thông tin trước!", "Thông báo");
                return;
            }

            var dtSource = grcDetail_GopYKhacPhuc.DataSource as DataTable;
            if (dtSource == null || !dtSource.Columns.Contains("ID"))
            {
                dtSource = CreateTblDetail();
                AddUIColumns(dtSource);
                grcDetail_GopYKhacPhuc.DataSource = dtSource;
            }
            else
            {
                AddUIColumns(dtSource);
            }

            int maxSTT = 0;
            foreach (DataRow r in dtSource.Rows)
            {
                if (r["STT"] != DBNull.Value)
                {
                    if (int.TryParse(r["STT"].ToString(), out int sttVal))
                        if (sttVal > maxSTT) maxSTT = sttVal;
                }
            }

            object maNhom = DBNull.Value;
            string tenNhom = "";
            int insertIndex = 0;

            int focusedRowHandle = grvDetail_GopY.FocusedRowHandle;

            if (focusedRowHandle >= 0)
            {
                var drFocus = grvDetail_GopY.GetDataRow(focusedRowHandle);
                if (drFocus != null)
                {
                    maNhom = drFocus["MaNhom"];
                    tenNhom = drFocus["TenNhom"]?.ToString() ?? "";

                    insertIndex = dtSource.Rows.IndexOf(drFocus) + 1;
                }
            }
            else if (focusedRowHandle < 0 && grvDetail_GopY.IsGroupRow(focusedRowHandle))
            {
                int firstChildRowHandle = grvDetail_GopY.GetChildRowHandle(focusedRowHandle, 0);
                if (firstChildRowHandle >= 0)
                {
                    var drFocus = grvDetail_GopY.GetDataRow(firstChildRowHandle);
                    if (drFocus != null)
                    {
                        maNhom = drFocus["MaNhom"];
                        tenNhom = drFocus["TenNhom"]?.ToString() ?? "";
                        insertIndex = dtSource.Rows.IndexOf(drFocus);
                    }
                }
            }

            var drNew = dtSource.NewRow();
            drNew["ID"] = 0;
            drNew["ID_Link"] = gGuid.ToString();
            drNew["STT"] = maxSTT + 1;
            drNew["LoiNang"] = false;
            drNew["LoiNhe"] = false;
            drNew["BienPhap"] = "Điều chỉnh cho sản xuất";
            drNew["MucDoLoi"] = 0;
            drNew["Sort"] = maxSTT + 1;

            drNew["MaNhom"] = maNhom;
            drNew["TenNhom"] = tenNhom;
            drNew["MaLoi"] = "";

            if (insertIndex > dtSource.Rows.Count) insertIndex = dtSource.Rows.Count;

            dtSource.Rows.InsertAt(drNew, insertIndex);
            grvDetail_GopY.FocusedRowHandle = insertIndex;
            CalSTT();
            //this.BeginInvoke(new Action(() =>
            //{
            //    //ApplyGrouping();

            //    int newRowHandle = grvDetail_GopY.LocateByValue("STT", maxSTT + 1);
            //    if (newRowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
            //    {
            //        grvDetail_GopY.FocusedRowHandle = newRowHandle;
            //        grvDetail_GopY.SelectRow(newRowHandle);
            //    }
            //}));
        }
        private void btnDeleteDetail_Click(object sender, EventArgs e)
        {
            if (!CheckDel())
            {
                DialogResult dialogResultDel = MessageBox.Show("Phiếu đã được kiểm. Bạn có chắc muốn xóa không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResultDel != DialogResult.Yes) return;

            }


            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa chi tiết này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {


                int[] selectedRows = grvDetail_GopY.GetSelectedRows();
                if (selectedRows.Length == 0) return;
                var dtSave = CreateTblDetail();
                List<DataRow> rowsToDelete = new List<DataRow>();
                foreach (int rowHandle in selectedRows)
                {
                    DataRow dr = grvDetail_GopY.GetDataRow(rowHandle);
                    var drNew = dtSave.NewRow();
                    if (dr["ID"].ToString() == "0")
                    {
                        rowsToDelete.Add(dr);
                        //var dtSource = grcDanhMuc.DataSource as DataTable;
                        //dtSource.Rows.Remove(dr);
                        continue;
                    }
                    drNew["ID"] = dr["ID"];
                    dtSave.Rows.Add(drNew);
                }
                foreach (var row in rowsToDelete)
                {
                    row.Delete(); // hoặc dtSource.Rows.Remove(row);
                }
                if (dtSave.Rows.Count == 0) return;


                //var drFocus = grvDetail_GopY.GetFocusedDataRow();
                //if (drFocus is null) return;

                //var dtSave = CreateTblDetail();
                //var drSave = dtSave.NewRow();
                //drSave["ID"] = drFocus["ID"];
                //dtSave.Rows.Add(drSave);

                string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Post?action=DeleteDetail");
                var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadDetail_GopYKhacPhuc();
                }
            }
        }
        private void grvDetail_GopY_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as GridView;

            if (view.FocusedColumn.FieldName == "LoiNang" || view.FocusedColumn.FieldName == "LoiNhe")
            {
                // Lấy row hiện tại
                var drFocus = grvDetail_GopY.GetFocusedDataRow();
                var row = view.GetFocusedRowCellValue("MaLoi"); // ví dụ field
                if (drFocus != null && drFocus["MaNhom"].ToString() == "2")
                {
                    e.Cancel = true;
                }
                // Điều kiện: nếu Status != "OK" thì không cho edit
                else if (drFocus != null && drFocus["MaLoi"].ToString() != "")
                {
                    e.Cancel = true; // ❌ chặn edit
                }
            }

        }
        private void CalSTT()
        {
            var dtSource = grcDetail_GopYKhacPhuc.DataSource as DataTable;
            if (dtSource == null || dtSource.Rows.Count == 0) return;

            var dtSave = CreateTblDetail();
            if (dtSave.Columns.Count == 0) return;

            int indexDetail = 1;
            foreach (DataRow dr in dtSource.Rows)
            {
                dr["STT"] = indexDetail;
                indexDetail++;
            }
        }

        #endregion

        #region Thông Số
        private void LoadThongSo()
        {
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Get?action=GetThongSo&para1={gGuid}&para2={gStyleID}&para3={gSizeID}&para4={gDauSizeID}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt.Rows.Count > 0)
            {
                dgrNhapThongSo.DataSource = dt;
                RenderColoumn(dt);
            }
            else
            {
                dgrNhapThongSo.DataSource = null;
            }
        }
        private void LoadThongSoMH()
        {
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Get?action=GetThongSoMH&para1={gStyleID}&para2={gDauSizeID}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtThongSoMH = JsonConvert.DeserializeObject<DataTable>(json);
        }
        private void SaveThongSo()
        {
            this.ActiveControl = grcInSeam;
            DataTable dtSource = dgrNhapThongSo.DataSource as DataTable;
            //if (dtSource.Rows.Count == 0) return;
            if (dtSource == null || dtSource.Rows.Count == 0) return;
            var dtSave = CreateTblThongSo();
            foreach (DataRow dr in dtSource.Rows)
            {
                foreach (DataColumn dc in dtSource.Columns)
                {
                    if (dc.ColumnName.Contains("@Do"))
                    {
                        var drSave = dtSave.NewRow();
                        var ID_ThongSo_MaHang = GetID_ThongSo_MaHang(dr["ID_cd"].ToString(), dc.ColumnName.ToString().Split('@')[0]);
                        if (ID_ThongSo_MaHang == "" || dr[dc].ToString() == "0") continue;
                        drSave["ID"] = 0;
                        drSave["ThongSoDo"] = dr[dc];
                        drSave["ID_ThongSo_MaHang"] = ID_ThongSo_MaHang;
                        drSave["ID_Link"] = gGuid;
                        dtSave.Rows.Add(drSave);
                    }
                }
            }
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Post?action=PostThongSo");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
        }
        private string GetID_ThongSo_MaHang(string ID_cd, string size)
        {
            var drCheck = dtThongSoMH.AsEnumerable().Where(x => x["ID_cd"].ToString() == ID_cd && x["Size"].ToString() == size).FirstOrDefault();
            if (drCheck != null)
                return drCheck["ID"].ToString();
            return "";
        }
        private DataTable CreateTblThongSo()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("ID_ThongSo_MaHang", typeof(int));
            dt.Columns.Add("ID_Link", typeof(string));
            dt.Columns.Add("ThongSoDo", typeof(float));
            dt.Columns.Add("Actions", typeof(string));
            return dt;
        }
        //private void bandedGridViewNhapThongSo_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        //{
        //    DataRow drfocus = bandedGridViewNhapThongSo.GetFocusedDataRow();
        //    if (drfocus is null) return;
        //    var fieldName = e.Column.FieldName;
        //    var parts = fieldName.Split('@');
        //    string size = parts[0];
        //    string type = parts[1];

        //    var value = Convert.ToDouble(e.Value);
        //    var TSChuan = Convert.ToDouble(drfocus[size + "@Chuan"]);
        //    var SaiSoAm = Convert.ToDouble(drfocus["SaiSoAm"]);
        //    var SaiSoDuong = Convert.ToDouble(drfocus["SaiSoDuong"]);

        //    drfocus[size + "@SaiSo"] = (value - TSChuan >= (SaiSoAm * -1) && value - TSChuan <= SaiSoDuong) ? "OK" : Convert.ToString(value - TSChuan);
        //}
        private void bandedGridViewNhapThongSo_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DataRow drfocus = bandedGridViewNhapThongSo.GetFocusedDataRow();
            if (drfocus is null) return;

            var fieldName = e.Column.FieldName;
            var parts = fieldName.Split('@');
            string size = parts[0];
            string type = parts[1];

            if (type != "Do") return;

            var value = Convert.ToDouble(e.Value);
            var TSChuan = Convert.ToDouble(drfocus[size + "@Chuan"]);

            double saiSo = value - TSChuan;

            if (saiSo == 0)
            {
                drfocus[size + "@SaiSo"] = "√";
            }
            else
            {
                drfocus[size + "@SaiSo"] = saiSo > 0 ? "+" + saiSo.ToString() : saiSo.ToString();
            }
        }
        private void RenderColoumn(DataTable dt)
        {
            bandedGridViewNhapThongSo.BeginUpdate();
            bandedGridViewNhapThongSo.Bands.Clear();
            bandedGridViewNhapThongSo.Columns.Clear();
            string[] fixedCols = { "NO@NO", "Name@Công đoạn", "SaiSoAm@Sai Số âm", "SaiSoDuong@Sai Số dương" };

            foreach (var colName in fixedCols)
            {
                if (colName.Split('@')[0] == "Name")
                    CreateBand(colName.Split('@')[1], colName.Split('@')[0], 200, 0);
                else if (colName.Split('@')[0] == "NO")
                    CreateBand(colName.Split('@')[1], colName.Split('@')[0], 50);
                else
                    CreateBand(colName.Split('@')[1], colName.Split('@')[0], 100);
                //GridBand band = new GridBand();
                //band.Caption = colName.Split('@')[1];

                //bandedGridViewNhapThongSo.Bands.Add(band);

                //BandedGridColumn col = bandedGridViewNhapThongSo.Columns.AddField(colName.Split('@')[0]);
                //col.Visible = true;

                //band.Columns.Add(col);
            }

            Dictionary<string, GridBand> sizeBands = new Dictionary<string, GridBand>();
            GridBand gridBandSize = new GridBand();
            List<string> lstColumnHeader = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName.Contains("@"))
                {
                    var value = dc.ColumnName.Split('@')[0];
                    if (!lstColumnHeader.Contains(value))
                        lstColumnHeader.Add(dc.ColumnName.Split('@')[0]);
                }
                else
                {
                    // Cột chung
                    GridColumn col = bandedGridViewNhapThongSo.Columns.Add();
                    col.FieldName = dc.ColumnName;
                    col.Caption = dc.ColumnName;
                    col.Visible = true;
                    continue;
                }
            }
            foreach (var itemSize in lstColumnHeader)
            {
                GridBand gridBandSizeA = new GridBand();
                gridBandSizeA.Caption = itemSize.Replace("SIZE_", "");
                gridBandSizeA.AppearanceHeader.Options.UseTextOptions = true;
                gridBandSizeA.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                bandedGridViewNhapThongSo.Bands.Add(gridBandSizeA);
                foreach (DataColumn dc in dt.Columns)
                {
                    if (itemSize == dc.ColumnName.Split('@')[0])
                    {
                        var type = dc.ColumnName.Split('@')[1];
                        BandedGridColumn bandedGridColumn = new BandedGridColumn();
                        bandedGridColumn.Caption = type;
                        bandedGridColumn.Name = "bandedGridColumnSize" + itemSize;
                        bandedGridColumn.FieldName = dc.ColumnName;
                        bandedGridColumn.Visible = true;
                        bandedGridColumn.Width = 70;
                        bandedGridColumn.ColumnEdit = repositoryTxtNumber;
                        bandedGridColumn.AppearanceCell.Options.UseTextOptions = true;
                        bandedGridColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        if (type == "Chuan")
                            bandedGridColumn.OptionsColumn.AllowEdit = false;
                        else if (type == "Do")
                            bandedGridColumn.OptionsColumn.AllowEdit = true;
                        else if (type == "SaiSo")
                            bandedGridColumn.OptionsColumn.AllowEdit = false;

                        GridBand gridBand = new GridBand();
                        gridBand.AppearanceHeader.Options.UseTextOptions = true;
                        gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                        if (type == "Chuan")
                            gridBand.Caption = "Chuẩn";
                        else if (type == "Do")
                            gridBand.Caption = "Đo";
                        else if (type == "SaiSo")
                            gridBand.Caption = "Sai số";
                        gridBand.Name = "gridBandSize" + itemSize;
                        gridBand.VisibleIndex = 0;
                        gridBand.Width = 70;

                        gridBandSizeA.Children.Add(gridBand);
                        gridBand.Columns.Add(bandedGridColumn);
                    }
                }
            }
            //foreach (DataColumn dc in dt.Columns)
            //{


            //    var parts = dc.ColumnName.Split('@');

            //    string size = parts[0];
            //    string type = parts[1];

            //    if (!sizeBands.ContainsKey(size))
            //    {
            //        GridBand band = new GridBand();
            //        band.Caption = size;
            //        band.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //        bandedGridViewNhapThongSo.Bands.Add(band);

            //        sizeBands.Add(size, band);
            //    }

            //    BandedGridColumn column = bandedGridViewNhapThongSo.Columns.Add();
            //    column.FieldName = dc.ColumnName;
            //    column.Visible = true;

            //    if (type == "Chuan")
            //        column.Caption = "Chuẩn";
            //    else if (type == "Do")
            //        column.Caption = "Đo";
            //    else if (type == "SaiSo")
            //        column.Caption = "Sai số";

            //    sizeBands[size].Columns.Add(column);
            //}

            bandedGridViewNhapThongSo.OptionsView.ShowBands = true;
            //bandedGridViewNhapThongSo.OptionsView.ShowColumnHeaders = true;

            bandedGridViewNhapThongSo.EndUpdate();


        }
        private void CreateBand(string caption, string fieldName, int width = 80, int textalign = 1)
        {
            BandedGridColumn bandedGridColumn = new BandedGridColumn();
            bandedGridColumn.Caption = caption;
            bandedGridColumn.Name = "bandedGridColumnSize_DB" + fieldName;
            bandedGridColumn.FieldName = fieldName;
            bandedGridColumn.Visible = true;
            bandedGridColumn.Width = width;
            bandedGridColumn.OptionsColumn.AllowEdit = false;
            bandedGridColumn.ColumnEdit = repositoryTxtNumber;
            bandedGridColumn.AppearanceCell.Options.UseTextOptions = true;
            bandedGridColumn.AppearanceCell.TextOptions.HAlignment = textalign == 1 ? DevExpress.Utils.HorzAlignment.Center : DevExpress.Utils.HorzAlignment.Near;

            GridBand gridBand = new GridBand();
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.Caption = caption;
            gridBand.Name = "gridBandSize_DB" + fieldName;
            gridBand.VisibleIndex = 0;
            gridBand.Width = width;

            bandedGridViewNhapThongSo.Bands.Add(gridBand);
            gridBand.Columns.Add(bandedGridColumn);
        }
        #endregion

        private bool CheckDel()
        {
            if (gGuid == Guid.Empty) return true;
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Get?action=CheckDelete&para1={gGuid}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt.Rows.Count > 0) return false;
            return true;
        }

        private void grvDetail_GopY_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadImage();
        }

        #region Hình ảnh
        private void GrvHinh_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            var urlHost = (string)settingsReader.GetValue("HostQty", typeof(String));
            if ((e.Column.FieldName == "UrlImage") && e.IsGetData)
            {
                DataRow dr = grvHinh.GetDataRow(e.ListSourceRowIndex);
                if (dr == null) return;
                string url = "http://" + urlHost + dr["Image_Link"].ToString();
                if (!string.IsNullOrEmpty(url))
                {
                    try
                    {
                        using (var wc = new System.Net.WebClient())
                        {
                            byte[] data = wc.DownloadData(url);
                            using (var ms = new MemoryStream(data))
                                e.Value = Image.FromStream(ms);
                        }
                    }
                    catch (Exception ex)
                    {
                        e.Value = null;
                    }
                }
            }
        }
        private void GrcMaHangHinh_DragOver(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string[] imageExts = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };
                bool hasImage = files.Any(f =>
                                    imageExts.Contains(Path.GetExtension(f).ToLower())
                                );
                e.Effect = hasImage ? DragDropEffects.Copy : DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void GrcMaHangHinh_DragEnter(object sender, DragEventArgs e)
        {
            // Kiểm tra xem có phải file đang được kéo không
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                string[] imageExts = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };
                bool hasPdf = files.Any(f =>
                                    imageExts.Contains(Path.GetExtension(f).ToLower())
                                );

                if (hasPdf)
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private async void GrcMaHangHinh_DragDrop(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DragDrop triggered");

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);


            Point screenPoint = new Point(e.X, e.Y);
            Point gridPoint = grcMaHangHinh.PointToClient(screenPoint);

            GridHitInfo hitInfo = grvHinh.CalcHitInfo(gridPoint);

            if (!hitInfo.InRowCell || hitInfo.Column == null)
                return;

            int rowHandle = hitInfo.RowHandle;
            string columnName = hitInfo.Column.FieldName;

            System.Diagnostics.Debug.WriteLine($"Drop vào Row: {rowHandle}, Column: {columnName}");
            //List<string> lstFieldNameHinh = new List<string>() { "Url", "FileName_TSo", "FileName_WaterTest" };
            //if (!lstFieldNameHinh.Contains(columnName)) return;

            DataRow drFocus = grvHinh.GetDataRow(rowHandle);
            if (drFocus == null) return;

            // Các định dạng ảnh cho phép
            string[] imageExts = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };

            foreach (string file in files)
            {
                string ext = Path.GetExtension(file)?.ToLower();

                if (string.IsNullOrEmpty(ext) || !imageExts.Contains(ext))
                    continue;

                FileInfo fileInfo = new FileInfo(file);
                string sourceFileName = file;

                // Tên file xử lý lại
                string fileName = Guid.NewGuid().ToString() + ".png";
                var result = await UploadImage("", sourceFileName, fileName);
                if (result)
                {
                    grvHinh.SetRowCellValue(grvHinh.FocusedRowHandle, "Image_Link", Path.Combine(@"\Uploads\KiemDauChuyen", fileName));
                    SaveHinh();
                }
                break; // chỉ nhận 1 file cho 1 cell
            }
        }
        private async Task<bool> UploadImage(string action, string sourceFileName, string fileName)
        {
            string Url = "";
            string objUrl = (string)settingsReader.GetValue("HostQty", typeof(String));
            var client = new WebClient();
            //object objUrl = _regedit.get_RegistryKey(QtyUrlKeyName);
            //objUrl = "localhost:5480";
            if (objUrl != null)
                Url = string.Format("http://{0}/api/QTY_KiemDauChuyenV2/UploadImage", objUrl);
            var uri = new Uri(Url);
            try
            {
                client.Headers.Add("action", action);
                client.Headers.Add("fileName", fileName);
                var data = System.IO.File.ReadAllBytes(sourceFileName);
                var responseBytes = await client.UploadDataTaskAsync(uri, data);
                string result = Encoding.UTF8.GetString(responseBytes);
                if (result.ToUpper() == "TRUE")
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private void btnAddImage_Click(object sender, EventArgs e)
        {
            var drFocus = grvDetail_GopY.GetFocusedDataRow();
            if (drFocus is null) return;
            if (drFocus["MaLoi"].ToString() == "" && drFocus["MaNhom"].ToString() == "2")
            {
                MessageBox.Show("Vui lòng chọn code lỗi!");
                return;
            }
            if (drFocus["ID"].ToString() == "0")
            {
                SaveDetail_Button();
            }
            var STT = drFocus["STT"].ToString();
            string STT_Image = "";
            DataTable dtSource = grcMaHangHinh.DataSource as DataTable;
            if (dtSource.Rows.Count > 0)
            {
                var lastCD = dtSource.Rows[dtSource.Rows.Count - 1]["STT"].ToString().Split('.');
                STT_Image = STT + "." + (Convert.ToInt16(lastCD[1]) + 1).ToString();

            }
            else
            {
                STT_Image = STT + ".1";
            }
            var drNew = dtSource.NewRow();
            drNew["ID"] = 0;
            drNew["STT"] = STT_Image;

            dtSource.Rows.Add(drNew);
            grvHinh.FocusedRowHandle = dtSource.Rows.Count - 1;
        }
        private void SaveHinh()
        {
            DataTable dtSource = grcMaHangHinh.DataSource as DataTable;
            if (dtSource.Rows.Count == 0) return;
            var drFocus = grvDetail_GopY.GetFocusedDataRow();
            if (drFocus is null) return;
            if (drFocus["MaLoi"].ToString() == "" && drFocus["MaNhom"].ToString() == "2")
            {
                MessageBox.Show("Vui lòng chọn lỗi!");
                return;
            }
            var dtSave = CreateTblImage();
            foreach (DataRow dr in dtSource.Rows)
            {
                var drSave = dtSave.NewRow();
                drSave["ID"] = dr["ID"];
                drSave["ID_Kiem"] = drFocus["ID"];
                drSave["Image_Link"] = dr["Image_Link"].ToString().Replace("\\", "/");
                drSave["Image_Type"] = 0;
                drSave["NoteImage"] = dr["NoteImage"];
                drSave["STT"] = dr["STT"];
                dtSave.Rows.Add(drSave);
            }
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Post?action=PostImage");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadImage();
            }
        }
        private void LoadImage()
        {
            var drFocus = grvDetail_GopY.GetFocusedDataRow();
            if (drFocus is null)
            {
                return;
            }
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Get?action=GetImage&para1={drFocus["ID"].ToString()}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt.Rows.Count > 0)
            {
                int rowHandle = grvHinh.FocusedRowHandle;
                HandleSTTHinhAnh(dt);
                grcMaHangHinh.DataSource = dt;
                grvHinh.FocusedRowHandle = rowHandle;
            }
            else
            {
                grcMaHangHinh.DataSource = CreateTblImage();
            }
        }
        private void HandleSTTHinhAnh(DataTable dtSource)
        {
            var drFocusDetail = grvDetail_GopY.GetFocusedDataRow();
            if (drFocusDetail is null) return;
            var STTDetail = drFocusDetail["STT"].ToString();
            int index = 1;
            foreach(DataRow dr in dtSource.Rows)
            {
                dr["STT"] = STTDetail +"."+ index.ToString();
                index++;
            }
        }
        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            SaveHinh();
        }
        private void btnDeleteImage_Click(object sender, EventArgs e)
        {
            var drFocus = grvHinh.GetFocusedDataRow();
            if (drFocus is null) return;
            DialogResult dialogResult = MessageBox.Show("Bạn có chắn muốn xóa ảnh này không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                var dtSave = CreateTblImage();
                var drSave = dtSave.NewRow();
                drSave["ID"] = drFocus["ID"];
                dtSave.Rows.Add(drSave);
                string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Post?action=DeleteImage");
                var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadImage();
                }
            }

        }


        private string ReplaceSpecialCharacterssize(string input)
        {
            if (input is null) return "";
            // Pattern để tìm khoảng trắng và các kí tự đặc biệt
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            // Thay thế bằng dấu _
            string replacement = "_";
            // Tạo một Regex và thực hiện thay thế
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement).ToUpper();
        }
        private DataTable CreateTblImage()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("ID_Kiem", typeof(int));
            dt.Columns.Add("Image_Link", typeof(string));
            dt.Columns.Add("Image_Type", typeof(string));
            dt.Columns.Add("NoteImage", typeof(string));
            dt.Columns.Add("STT", typeof(string));
            return dt;
        }
        #endregion
        private void btnDeleteAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            DialogResult dialogResultDel = MessageBox.Show("Bạn có chắc muốn xóa phiếu không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResultDel != DialogResult.Yes) return;
            if (gGuid == Guid.Empty) return;
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Delete?action=DeleteAll&&parameter={gGuid}");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, gGuid); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadAllData();
            }
        }
        #region Import excel
        //private void LoadDetail_GopYKhacPhuc()
        //{
        //    string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Get?action=GetDetail&para1={gGuid}");
        //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //    DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
        //    if (dt.Rows.Count > 0)
        //    {
        //        AddUIColumns(dt);
        //        grcDetail_GopYKhacPhuc.DataSource = dt;
        //    }
        //    else
        //    {
        //        var emptyDt = CreateTblDetail();
        //        AddUIColumns(emptyDt);
        //        grcDetail_GopYKhacPhuc.DataSource = emptyDt;
        //    }

        //    // ✅ Áp dụng group sau khi bind
        //    ApplyGrouping();
        //}

        private void LoadDanhMuc()
        {
            string url = URL + $"QTY_KiemDauChuyen/Get?action=GetDanhMuc&para1={gGuid}";
            string json = Task.Run(async () =>
                await _clientExtension.GetAsnyc(url)).Result;
            if (string.IsNullOrEmpty(json)) return;

            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt == null)
            {
                grcDanhMuc.DataSource = CreateTblDanhMuc();
                return;
            }
            if (!dt.Columns.Contains("Col_Star")) dt.Columns.Add("Col_Star", typeof(bool));
            if (!dt.Columns.Contains("Col_Delta")) dt.Columns.Add("Col_Delta", typeof(bool));
            if (!dt.Columns.Contains("Col_Check")) dt.Columns.Add("Col_Check", typeof(bool));
            if (!dt.Columns.Contains("Col_X")) dt.Columns.Add("Col_X", typeof(bool));

            foreach (DataRow dr in dt.Rows)
            {
                dr["Col_Star"] = false;
                dr["Col_Delta"] = false;
                dr["Col_Check"] = false;
                dr["Col_X"] = false;

                string val = dr["Value"] == DBNull.Value ? "" : dr["Value"]?.ToString().Trim();
                switch (val)
                {
                    case "0": dr["Col_Star"] = true; break;
                    case "1": dr["Col_Delta"] = true; break;
                    case "2": dr["Col_Check"] = true; break;
                    case "3": dr["Col_X"] = true; break;
                }
            }

            grcDanhMuc.DataSource = dt;
            grvDanhMuc.ActiveFilterString = string.Empty;
        }
        private void btnImportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (searchLookUpEdit_Line.EditValue is null || searchLookUpEdit_MaHang.EditValue is null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ ttin chuyền , mã hàng!");
                return;
            }
            if (!CheckDel())
            {
                MessageBox.Show("Phiếu đã được kiểm. Không thể import dữ liệu!", "Thông báo");
                return;

                //DialogResult dialogResult = MessageBox.Show("Phiếu đã được kiểm. Không thể import dữ liệu!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            OpenFileDialog of = new OpenFileDialog();

            of.Filter = "Excel File (*.xlsx)|*.xlsx";

            if (of.ShowDialog() != DialogResult.OK)
                return;

            DataTable dtExcel = ExcelQCReader.ReadExcel(of.FileName);
            ReadDanhMuc(dtExcel);
            ReadDetail(dtExcel);
            LoadGuid();
        }
        private void ReadDanhMuc(DataTable dt)
        {
            string lineX = null;
            string style = null;
            string size = null;
            string inseam = null;
            string po = null;

            int startRow = 7;
            int stopRow = dt.Rows.Count;

            // ===== SCAN HEADER =====

            for (int r = 0; r < dt.Rows.Count; r++)
            {
                //if(dt.Rows[r][0]?.ToString().Trim() == "")
                //{
                //    gStopRow = r + 3;
                //    stopRow = r;
                //}
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    string cell = dt.Rows[r][c]?.ToString().Trim();

                    if (string.IsNullOrEmpty(cell))
                        continue;
                    if (cell.Contains("Chuyền"))
                    {
                        if (c + 3 < dt.Columns.Count)
                        {
                            string value = dt.Rows[r][c + 3]?.ToString().Trim();

                            if (!string.IsNullOrEmpty(value))
                                lineX = value; // ANTIGONE
                        }
                    }

                    if (cell.Contains("Style"))
                        style = GetRightValue(dt, r, c);

                    if (cell.Contains("Size"))
                    {
                        string v = GetRightValue(dt, r, c);
                        if (string.IsNullOrEmpty(v))
                            v = GetDownValue(dt, r, c);

                        size = v; // M
                    }

                    if (cell.Contains("InSeam"))
                    {
                        string v = GetRightValue(dt, r, c);
                        if (string.IsNullOrEmpty(v))
                            v = GetDownValue(dt, r, c);

                        inseam = v; // REG
                    }

                    if (cell.Contains("Cut/PO"))
                    {
                        string v = GetRightValue(dt, r, c);
                        po = string.IsNullOrWhiteSpace(v) ? null : v;
                    }

                    //if (cell.Contains("I/CÁC MỤC KIỂM TRA"))
                    //    startRow = r + 2;

                    //if (cell.StartsWith("II/"))
                    //{
                    //    gStopRow = r + 2;
                    //    stopRow = r;
                    //}

                }
            }

            int codeIndex = 0;
            DataTable dtSave = CreateTblKiemDauChuyen();
            var drSave = dtSave.NewRow();
            gGuid = gGuid == Guid.Empty ? Guid.NewGuid() : gGuid;
            drSave["ID"] = 0;
            drSave["Line"] = searchLookUpEdit_Line.EditValue;
            drSave["LenhSX"] = searchLookUpEdit_MaHang.EditValue;
            drSave["MaHang"] = style;
            drSave["POID"] = ReplaceSpecialCharacterssize(po);
            drSave["PO"] = po;
            drSave["DauSizeID"] = ReplaceSpecialCharacterssize(inseam);
            drSave["DauSize"] = ReplaceSpecialCharacterssize(inseam);
            drSave["MaMau"] = searchLookUpEdit_Color.EditValue?.ToString() ?? "";
            drSave["TenMau"] = searchLookUpEdit_Color.Text ?? "";
            var splitSize = size.Split(',');
            var sizeTong = "";
            foreach (var item in splitSize)
            {
                var checkCumCharacter = item.Contains('+');
                sizeTong += ",SIZE_" + ReplaceSpecialCharacterssize(item.Trim()) + (checkCumCharacter ? "_" : "");
            }
            sizeTong = sizeTong.TrimStart(',');

            drSave["SizeID"] = sizeTong;
            drSave["Size"] = size;
            drSave["Guid"] = gGuid;
            drSave["QCInLine"] = GlobleData.UserName;
            dtSave.Rows.Add(drSave);
            // ===== SCAN QC ITEMS =====


            DataTable dtSaveDanhMuc = CreateTblDanhMuc();
            int indexDanhMuc = 1;
            for (int r = startRow; r < dt.Rows.Count; r++)
            {
                if (dt.Rows[r][0]?.ToString().Trim() == "")
                {
                    gStopRow = r + 2;
                    break;
                }
                for (int c = 0; c < dt.Columns.Count - 1; c++)
                {
                    string name = dt.Rows[r][c]?.ToString().Trim();

                    if (string.IsNullOrEmpty(name))
                        continue;

                    string mark = dt.Rows[r][c + 1]?.ToString().Trim();

                    int value = ConvertMark(mark);

                    if (value == -999)
                        continue;
                    var drSaveDanhMuc = dtSaveDanhMuc.NewRow();
                    drSaveDanhMuc["ID"] = 0;
                    drSaveDanhMuc["ID_Link"] = gGuid;
                    drSaveDanhMuc["Code"] = name;
                    drSaveDanhMuc["Value"] = value;
                    drSaveDanhMuc["Sort"] = indexDanhMuc;
                    dtSaveDanhMuc.Rows.Add(drSaveDanhMuc);
                    codeIndex++;
                    indexDanhMuc++;
                }
            }
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Post?action=Post");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                //searchLookUpEdit_PO.EditValue = po;
                //searchLookUpEdit_Inseam.EditValue = inseam;
                //searchLookUpEdit_Size.EditValue = size;
                clsWaitForm.ShowSuccessForm(this, 2000);
                url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Post?action=PostDanhMuc");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSaveDanhMuc); }).Result;
                if (msResult.ToUpper() == "TRUE")
                {
                    //LoadDanhMuc();
                }
            }

        }
        private void ReadDetail(DataTable dt)
        {
            int currentGroup = 0;
            string currentGroupName = "";

            int lastSTT = 0;
            int colLoiNang = -1;
            int colLoiNhe = -1;

            for (int r = gStopRow; r < dt.Rows.Count; r++)
            {
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    string v = dt.Rows[r][c]?.ToString().Trim().ToLower();

                    if (v.Contains("lỗi nặng"))
                        colLoiNang = c;

                    if (v.Contains("lỗi nhẹ"))
                        colLoiNhe = c;
                }

                if (colLoiNang != -1 && colLoiNhe != -1)
                    break;
            }
            DataTable dtSave = CreateTblDetail();
            int indexDetail = 1;
            for (int r = gStopRow; r < dt.Rows.Count; r++)
            {
                string sttText = dt.Rows[r][0]?.ToString().Trim();
                string mota = dt.Rows[r][1]?.ToString().Trim();

                // ========================
                // detect group
                // ========================

                string value = dt.Rows[r][0]?.ToString().Trim();

                if (string.IsNullOrEmpty(value))
                {
                    gStopRow = r + 3;
                    break;

                }


                if (value.StartsWith("I.")
                 || value.StartsWith("II.")
                 || value.StartsWith("III.")
                 || value.StartsWith("IV."))
                {
                    string roman = value.Split('.')[0];

                    currentGroup = RomanToInt(roman);
                    currentGroupName = value;

                    goto NEXTROW;
                }

                for (int c = 0; c < dt.Columns.Count; c++)
                {

                }

                // ========================
                // kiểm tra STT
                // ========================

                int stt;

                if (!int.TryParse(sttText, out stt))
                    goto NEXTROW;

                // nếu STT bị reset → stop scan
                if (stt < lastSTT)
                    break;

                lastSTT = stt;

                // ========================
                // detect tick lỗi
                // ========================

                int mucDoLoi = 0;

                string loiNang = colLoiNang >= 0 ? dt.Rows[r][colLoiNang]?.ToString().Trim() : "";
                string loiNhe = colLoiNhe >= 0 ? dt.Rows[r][colLoiNhe]?.ToString().Trim() : "";

                // check có tick
                bool tickNang = !string.IsNullOrEmpty(loiNang);
                bool tickNhe = !string.IsNullOrEmpty(loiNhe);

                if (tickNang)
                    mucDoLoi = 1;

                if (tickNhe)
                    mucDoLoi = 0;

                // ========================s
                // detect biện pháp
                // ========================

                string bienPhap = "";

                bienPhap = dt.Rows[r][8]?.ToString().Trim();

                //if (!string.IsNullOrEmpty(value)
                //    && value != "✓"
                //    && value != "√"
                //    && value != "ü")
                //{
                //    if (value.Contains("Điều chỉnh")
                //     || value.Contains("Cắt")
                //     || value.Contains("OK"))
                //    {
                //        bienPhap = value;
                //    }
                //}
                for (int c = 0; c < dt.Columns.Count; c++)
                {

                }
                var drSave = dtSave.NewRow();
                drSave["ID"] = 0;
                drSave["ID_Link"] = gGuid;
                drSave["STT"] = stt;
                drSave["MoTa"] = mota;
                drSave["BienPhap"] = bienPhap;
                drSave["MucDoLoi"] = mucDoLoi;
                drSave["MaNhom"] = currentGroup;
                drSave["TenNhom"] = currentGroupName;
                drSave["Sort"] = indexDetail;
                dtSave.Rows.Add(drSave);
                indexDetail++;
            NEXTROW:;
            }
            gNhanVien = dt.Rows[gStopRow - 1][4].ToString();
            string url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Post?action=PostDetail");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToUpper() == "TRUE")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                //Update NhanVien
                dtSave = CreateTblKiemDauChuyen();
                var drSave = dtSave.NewRow();
                drSave["ID"] = 0;
                drSave["Guid"] = gGuid;
                drSave["CreateUser"] = gNhanVien;
                dtSave.Rows.Add(drSave);
                url = string.Format("{0}", URL + $"QTY_KiemDauChuyen/Post?action=UpdateNVKT");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
                //LoadDetail_GopYKhacPhuc();
            }
        }
        private DataTable CreateTblKiemDauChuyen()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Line", typeof(string));
            dt.Columns.Add("LenhSX", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("PO", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("DauSize", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("Size", typeof(string));
            dt.Columns.Add("SoLuongKiem", typeof(int));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("TenMau", typeof(string));
            dt.Columns.Add("Note", typeof(string));
            dt.Columns.Add("CreateUser", typeof(string));
            dt.Columns.Add("CreateDate", typeof(DateTime));
            dt.Columns.Add("TechnicalStaff", typeof(string));
            dt.Columns.Add("QCInLine", typeof(string));
            dt.Columns.Add("LineManager", typeof(string));
            dt.Columns.Add("Guid", typeof(string));
            return dt;
        }
        private DataTable CreateTblDanhMuc()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("ID_Link", typeof(string));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Value", typeof(int));
            dt.Columns.Add("Note", typeof(string));
            dt.Columns.Add("Sort", typeof(int));
            return dt;
        }
        private DataTable CreateTblDetail()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("ID_Link", typeof(string));
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("MoTa", typeof(string));
            dt.Columns.Add("MucDoLoi", typeof(int));
            dt.Columns.Add("BienPhap", typeof(string));
            dt.Columns.Add("TheoDoi", typeof(string));
            dt.Columns.Add("MaNhom", typeof(int));
            dt.Columns.Add("TenNhom", typeof(string));
            dt.Columns.Add("MaLoi", typeof(string));
            dt.Columns.Add("Sign", typeof(string));
            dt.Columns.Add("NgaySign", typeof(string));
            dt.Columns.Add("NguoiSign", typeof(string));
            dt.Columns.Add("Sort", typeof(int));
            return dt;
        }
        private static string GetDownValue(DataTable dt, int row, int col)
        {
            // chỉ kiểm tra 3 dòng dưới label
            for (int r = row + 1; r <= row + 3 && r < dt.Rows.Count; r++)
            {
                for (int c = col; c < dt.Columns.Count; c++)
                {
                    string v = dt.Rows[r][c]?.ToString().Trim();

                    if (string.IsNullOrEmpty(v))
                        continue;

                    if (IsHeaderLabel(v))
                        continue;

                    if (int.TryParse(v, out _))
                        continue;

                    // bỏ qua text layout
                    if (v.Contains("Vị trí"))
                        continue;

                    return v;
                }
            }

            return null;
        }
        private DataTable LoadNhomKhacPhuc()
        {
            string url = URL + "QTY_KiemDauChuyen/Get?action=GetNhomKhacPhuc";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            var dt = JsonConvert.DeserializeObject<DataTable>(json);
            return dt ?? new DataTable();
        }

        private void grvDetail_GopY_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //var drFocus = grvDetail_GopY.GetFocusedDataRow();
            //if (e.Column.FieldName == "LoiNang")
            //{
            //    var value = e.Value;
            //    grvDetail_GopY.SetFocusedRowCellValue("LoiNhe", !Convert.ToBoolean(value));
            //    //if (Convert.ToBoolean(value)) 
            //    //drFocus["LoiNhe"] = !Convert.ToBoolean(value);
            //}
            //else if (e.Column.FieldName == "LoiNhe")
            //{
            //    var value = e.Value;
            //    grvDetail_GopY.SetFocusedRowCellValue("LoiNang", !Convert.ToBoolean(value));
            //    //drFocus["LoiNang"] = !Convert.ToBoolean(value);
            //}
            //grvDetail_GopY.PostEditor();
            //grvDetail_GopY.UpdateCurrentRow();
        }

        private void grvDetail_GopY_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            if (e.Column.FieldName == "LoiNang")
            {
                view.SetRowCellValue(e.RowHandle, "LoiNhe", !(bool)e.Value);
            }
            else if (e.Column.FieldName == "LoiNhe")
            {
                view.SetRowCellValue(e.RowHandle, "LoiNang", !(bool)e.Value);
            }
        }

        private void grcDanhMuc_FocusedViewChanged(object sender, DevExpress.XtraGrid.ViewFocusEventArgs e)
        {

        }

        //private void simpleButton1_MouseClick(object sender, MouseEventArgs e)
        //{
        //    MoveRowUp();
        //}


        //private void MoveRowDown()
        //{
        //    try
        //    {
        //        int currentRowHandle = grvDetail_GopY.FocusedRowHandle;
        //        var dtSource = grcDetail_GopYKhacPhuc.DataSource as DataTable; // ✅ lấy từ GridControl
        //        if (dtSource == null || dtSource.Rows.Count == 0) return;
        //        if (currentRowHandle >= grvDetail_GopY.RowCount - 1) return;

        //        int nextRowHandle = currentRowHandle + 1;
        //        var currentRow = grvDetail_GopY.GetDataRow(currentRowHandle);
        //        var nextRow = grvDetail_GopY.GetDataRow(nextRowHandle);
        //        if (currentRow == null || nextRow == null) return;

        //        int currentID = Convert.ToInt32(currentRow["ID"]);

        //        // Hoán đổi Sort
        //        object tempSort = currentRow["Sort"];
        //        currentRow["Sort"] = nextRow["Sort"];
        //        nextRow["Sort"] = tempSort;

        //        // Sắp xếp lại
        //        DataView dv = dtSource.DefaultView;
        //        dv.Sort = "Sort ASC";
        //        DataTable sortedDt = dv.ToTable();
        //        AddUIColumns(sortedDt);
        //        // ✅ Gán vào GridControl, không gán vào GridView
        //        grcDetail_GopYKhacPhuc.DataSource = sortedDt;
        //        grvDetail_GopY.RefreshData();


        //        this.BeginInvoke(new Action(() =>
        //        {
        //            for (int i = 0; i < grvDetail_GopY.RowCount; i++)
        //            {
        //                var row = grvDetail_GopY.GetDataRow(i);
        //                if (row != null && Convert.ToInt32(row["ID"]) == currentID)
        //                {
        //                    grvDetail_GopY.FocusedRowHandle = i;
        //                    grvDetail_GopY.SelectRow(i);
        //                    break;
        //                }
        //            }
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Lỗi khi di chuyển xuống: " + ex.Message);
        //    }
        //}

        //private void MoveRowUp()
        //{
        //    try
        //    {
        //        int currentRowHandle = grvDetail_GopY.FocusedRowHandle;
        //        if (currentRowHandle <= 0) return;

        //        var dtSource = grcDetail_GopYKhacPhuc.DataSource as DataTable; // ✅ lấy từ GridControl
        //        if (dtSource == null || dtSource.Rows.Count == 0) return;

        //        int previousRowHandle = currentRowHandle - 1;
        //        var currentRow = grvDetail_GopY.GetDataRow(currentRowHandle);
        //        var previousRow = grvDetail_GopY.GetDataRow(previousRowHandle);
        //        if (currentRow == null || previousRow == null) return;

        //        int currentID = Convert.ToInt32(currentRow["ID"]);

        //        // Hoán đổi Sort
        //        object tempSort = currentRow["Sort"];
        //        currentRow["Sort"] = previousRow["Sort"];
        //        previousRow["Sort"] = tempSort;

        //        // Sắp xếp lại
        //        DataView dv = dtSource.DefaultView;
        //        dv.Sort = "Sort ASC";
        //        DataTable sortedDt = dv.ToTable();
        //        AddUIColumns(sortedDt);
        //        // ✅ Gán vào GridControl, không gán vào GridView
        //        grcDetail_GopYKhacPhuc.DataSource = sortedDt;
        //        grvDetail_GopY.RefreshData();

        //        // Focus lại dòng vừa move (tìm theo ID)
        //        this.BeginInvoke(new Action(() =>
        //        {
        //            for (int i = 0; i < grvDetail_GopY.RowCount; i++)
        //            {
        //                var row = grvDetail_GopY.GetDataRow(i);
        //                if (row != null && Convert.ToInt32(row["ID"]) == currentID)
        //                {
        //                    grvDetail_GopY.FocusedRowHandle = i;
        //                    grvDetail_GopY.SelectRow(i);
        //                    break;
        //                }
        //            }
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Lỗi khi di chuyển lên: " + ex.Message);
        //    }
        //}
        //private void MoveRowUp()
        //{
        //    try
        //    {
        //        int currentRowHandle = grvDetail_GopY.FocusedRowHandle;
        //        if (currentRowHandle <= 0) return;

        //        var dtSource = grcDetail_GopYKhacPhuc.DataSource as DataTable;
        //        if (dtSource == null || dtSource.Rows.Count == 0) return;

        //        int previousRowHandle = currentRowHandle - 1;
        //        var previousRow = grvDetail_GopY.GetDataRow(previousRowHandle);
        //        while (previousRow == null && previousRowHandle > 0)
        //        {
        //            previousRowHandle--;
        //            previousRow = grvDetail_GopY.GetDataRow(previousRowHandle);
        //        }

        //        var currentRow = grvDetail_GopY.GetDataRow(currentRowHandle);
        //        if (currentRow == null || previousRow == null) return;

        //        // Lưu lại Sort mục tiêu để lát tìm lại focus cho chuẩn xác (kể cả dòng mới ID = 0)
        //        int targetSort = Convert.ToInt32(previousRow["Sort"]);

        //        // 1. Hoán đổi giá trị Sort
        //        object tempSort = currentRow["Sort"];
        //        currentRow["Sort"] = previousRow["Sort"];
        //        previousRow["Sort"] = tempSort;

        //        // 2. Xác định Index vật lý hiện tại
        //        int oldIndex = dtSource.Rows.IndexOf(currentRow);
        //        int newIndex = dtSource.Rows.IndexOf(previousRow);

        //        // 3. KHÓA UI DEVEXPRESS (Quan trọng: Giữ nguyên Group, không chớp giật)
        //        grvDetail_GopY.BeginDataUpdate();

        //        // 4. Cắt/Dán vật lý trong DataTable (Để hàm Save đọc đúng vòng lặp)
        //        DataRow newRow = dtSource.NewRow();
        //        newRow.ItemArray = currentRow.ItemArray; // Clone data
        //        dtSource.Rows.Remove(currentRow);        // Xóa dòng cũ
        //        dtSource.Rows.InsertAt(newRow, newIndex);// Chèn lên vị trí trên

        //        // 5. MỞ KHÓA UI
        //        grvDetail_GopY.EndDataUpdate();

        //        // 6. Focus lại mượt mà
        //        this.BeginInvoke(new Action(() =>
        //        {
        //            int rowHandle = grvDetail_GopY.LocateByValue("Sort", targetSort);
        //            if (rowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
        //            {
        //                grvDetail_GopY.FocusedRowHandle = rowHandle;
        //                grvDetail_GopY.SelectRow(rowHandle);
        //            }
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Lỗi khi di chuyển lên: " + ex.Message);
        //    }
        //}
        private void MoveRowUp()
        {
            try
            {
                int currentRowHandle = grvDetail_GopY.FocusedRowHandle;
                if (currentRowHandle <= 0) return;

                var dtSource = grcDetail_GopYKhacPhuc.DataSource as DataTable;
                if (dtSource == null || dtSource.Rows.Count == 0) return;

                int previousRowHandle = currentRowHandle - 1;
                var previousRow = grvDetail_GopY.GetDataRow(previousRowHandle);
                while (previousRow == null && previousRowHandle > 0)
                {
                    previousRowHandle--;
                    previousRow = grvDetail_GopY.GetDataRow(previousRowHandle);
                }

                var currentRow = grvDetail_GopY.GetDataRow(currentRowHandle);
                if (currentRow == null || previousRow == null) return;

                grvDetail_GopY.BeginDataUpdate();

                object[] tempArray = currentRow.ItemArray;
                currentRow.ItemArray = previousRow.ItemArray;
                previousRow.ItemArray = tempArray;

                grvDetail_GopY.EndDataUpdate();

                grvDetail_GopY.FocusedRowHandle = previousRowHandle;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi di chuyển lên: " + ex.Message);
            }
        }

        private void MoveRowDown()
        {
            try
            {
                int currentRowHandle = grvDetail_GopY.FocusedRowHandle;
                var dtSource = grcDetail_GopYKhacPhuc.DataSource as DataTable;
                if (dtSource == null || dtSource.Rows.Count == 0) return;

                // Dùng DataRowCount để tránh lỗi Out of Bounds với các dòng cuối
                if (currentRowHandle >= grvDetail_GopY.DataRowCount - 1) return;

                int nextRowHandle = currentRowHandle + 1;
                var nextRow = grvDetail_GopY.GetDataRow(nextRowHandle);
                while (nextRow == null && nextRowHandle < grvDetail_GopY.DataRowCount - 1)
                {
                    nextRowHandle++;
                    nextRow = grvDetail_GopY.GetDataRow(nextRowHandle);
                }

                var currentRow = grvDetail_GopY.GetDataRow(currentRowHandle);
                if (currentRow == null || nextRow == null) return;

                grvDetail_GopY.BeginDataUpdate();

                object[] tempArray = currentRow.ItemArray;
                currentRow.ItemArray = nextRow.ItemArray;
                nextRow.ItemArray = tempArray;

                grvDetail_GopY.EndDataUpdate();

                grvDetail_GopY.FocusedRowHandle = nextRowHandle;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi di chuyển xuống: " + ex.Message);
            }
        }
        // Helper 
        private List<string> SaveGroupExpandedState(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            List<string> expandedList = new List<string>();
            for (int i = -1; i >= int.MinValue; i--)
            {
                if (!view.IsValidRowHandle(i)) break;

                if (view.GetRowExpanded(i))
                {
                    var groupValue = view.GetGroupRowValue(i);
                    if (groupValue != null) expandedList.Add(groupValue.ToString());
                }
            }
            return expandedList;
        }

        private void RestoreGroupExpandedState(DevExpress.XtraGrid.Views.Grid.GridView view, List<string> expandedList)
        {
            for (int i = -1; i >= int.MinValue; i--)
            {
                if (!view.IsValidRowHandle(i)) break;

                var groupValue = view.GetGroupRowValue(i);
                if (groupValue != null && expandedList.Contains(groupValue.ToString()))
                {
                    view.SetRowExpanded(i, true);
                }
            }
        }

        //private void MoveRowDown()
        //{
        //    try
        //    {
        //        int currentRowHandle = grvDetail_GopY.FocusedRowHandle;
        //        var dtSource = grcDetail_GopYKhacPhuc.DataSource as DataTable;
        //        if (dtSource == null || dtSource.Rows.Count == 0) return;
        //        if (currentRowHandle >= grvDetail_GopY.RowCount - 1) return;

        //        int nextRowHandle = currentRowHandle + 1;
        //        var nextRow = grvDetail_GopY.GetDataRow(nextRowHandle);
        //        while (nextRow == null && nextRowHandle < grvDetail_GopY.RowCount - 1)
        //        {
        //            nextRowHandle++;
        //            nextRow = grvDetail_GopY.GetDataRow(nextRowHandle);
        //        }

        //        var currentRow = grvDetail_GopY.GetDataRow(currentRowHandle);
        //        if (currentRow == null || nextRow == null) return;

        //        // Lưu lại Sort mục tiêu để lát tìm lại focus cho chuẩn xác (kể cả dòng mới ID = 0)
        //        int targetSort = Convert.ToInt32(nextRow["Sort"]);

        //        // 1. Hoán đổi giá trị Sort
        //        object tempSort = currentRow["Sort"];
        //        currentRow["Sort"] = nextRow["Sort"];
        //        nextRow["Sort"] = tempSort;

        //        // 2. Xác định Index vật lý hiện tại
        //        int oldIndex = dtSource.Rows.IndexOf(currentRow);
        //        int newIndex = dtSource.Rows.IndexOf(nextRow);

        //        // 3. KHÓA UI DEVEXPRESS (Quan trọng: Giữ nguyên Group, không chớp giật)
        //        grvDetail_GopY.BeginDataUpdate();

        //        // 4. Cắt/Dán vật lý trong DataTable (Để hàm Save đọc đúng vòng lặp)
        //        DataRow newRow = dtSource.NewRow();
        //        newRow.ItemArray = currentRow.ItemArray; // Clone data
        //        dtSource.Rows.Remove(currentRow);        // Xóa dòng cũ
        //        dtSource.Rows.InsertAt(newRow, newIndex);// Chèn lên vị trí trên

        //        // 5. MỞ KHÓA UI
        //        grvDetail_GopY.EndDataUpdate();

        //        // 6. Focus lại mượt mà
        //        this.BeginInvoke(new Action(() =>
        //        {
        //            int rowHandle = grvDetail_GopY.LocateByValue("Sort", targetSort);
        //            if (rowHandle != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
        //            {
        //                grvDetail_GopY.FocusedRowHandle = rowHandle;
        //                grvDetail_GopY.SelectRow(rowHandle);
        //            }
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Lỗi khi di chuyển xuống: " + ex.Message);
        //    }
        //}
        private void simpleButton1_Click(object sender, EventArgs e)
        {

            MoveRowUp();
            CalSTT();
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            MoveRowDown();
            CalSTT();
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            MoveRowUp_DanhMuc();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            MoveRowDown_DanhMuc();
        }
        private void MoveRowUp_DanhMuc()
        {
            try
            {
                var dtSource = grcDanhMuc.DataSource as DataTable;
                DataRow drFocus = grvDanhMuc.GetFocusedDataRow();
                if (dtSource == null || drFocus == null) return;
                var index = dtSource.Rows.IndexOf(drFocus);
                if (index <= 0) return;
                DataRow newRow = dtSource.NewRow();
                newRow.ItemArray = drFocus.ItemArray.Clone() as object[];
                dtSource.Rows.RemoveAt(index);
                //newRow["Sort"] = index;
                dtSource.Rows.InsertAt(newRow, index - 1);
                grvDanhMuc.FocusedRowHandle = index - 1;
                //int currentRowHandle = grvDanhMuc.FocusedRowHandle;
                //if (currentRowHandle <= 0) return;

                //var dtSource = grcDanhMuc.DataSource as DataTable;
                //if (dtSource == null || dtSource.Rows.Count == 0) return;

                //int previousRowHandle = currentRowHandle - 1;
                //var currentRow = grvDanhMuc.GetDataRow(currentRowHandle);
                //var previousRow = grvDanhMuc.GetDataRow(previousRowHandle);
                //if (currentRow == null || previousRow == null) return;

                //int currentID = Convert.ToInt32(currentRow["ID"]);

                //object tempSort = currentRow["Sort"];
                //currentRow["Sort"] = previousRow["Sort"];
                //previousRow["Sort"] = tempSort;

                //DataView dv = dtSource.DefaultView;
                //dv.Sort = "Sort ASC";
                //DataTable sortedDt = dv.ToTable();

                //grcDanhMuc.DataSource = sortedDt;
                //grvDanhMuc.RefreshData();

                //this.BeginInvoke(new Action(() =>
                //{
                //    for (int i = 0; i < grvDanhMuc.RowCount; i++)
                //    {
                //        var row = grvDanhMuc.GetDataRow(i);
                //        if (row != null && Convert.ToInt32(row["ID"]) == currentID)
                //        {
                //            grvDanhMuc.FocusedRowHandle = i;
                //            grvDanhMuc.SelectRow(i);
                //            break;
                //        }
                //    }
                //}));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi di chuyển lên: " + ex.Message);
            }
        }

        private void MoveRowDown_DanhMuc()
        {
            try
            {
                var dtSource = grcDanhMuc.DataSource as DataTable;
                DataRow drFocus = grvDanhMuc.GetFocusedDataRow();
                if (dtSource == null || drFocus == null) return;
                var index = dtSource.Rows.IndexOf(drFocus);
                if (index == dtSource.Rows.Count - 1) return;
                DataRow newRow = dtSource.NewRow();
                newRow.ItemArray = drFocus.ItemArray.Clone() as object[];
                dtSource.Rows.RemoveAt(index);
                //newRow["Sort"] = index + 2;
                dtSource.Rows.InsertAt(newRow, index + 1);
                grvDanhMuc.FocusedRowHandle = index + 1;

                //int currentRowHandle = grvDanhMuc.FocusedRowHandle;
                //var dtSource = grcDanhMuc.DataSource as DataTable;
                //if (dtSource == null || dtSource.Rows.Count == 0) return;
                //if (currentRowHandle >= grvDanhMuc.RowCount - 1) return;

                //int nextRowHandle = currentRowHandle + 1;
                //var currentRow = grvDanhMuc.GetDataRow(currentRowHandle);
                //var nextRow = grvDanhMuc.GetDataRow(nextRowHandle);
                //if (currentRow == null || nextRow == null) return;

                //int currentID = Convert.ToInt32(currentRow["ID"]);

                //object tempSort = currentRow["Sort"];
                //currentRow["Sort"] = nextRow["Sort"];
                //nextRow["Sort"] = tempSort;

                //DataView dv = dtSource.DefaultView;
                //dv.Sort = "Sort ASC";
                //DataTable sortedDt = dv.ToTable();

                //grcDanhMuc.DataSource = sortedDt;
                //grvDanhMuc.RefreshData();

                //this.BeginInvoke(new Action(() =>
                //{
                //    for (int i = 0; i < grvDanhMuc.RowCount; i++)
                //    {
                //        var row = grvDanhMuc.GetDataRow(i);
                //        if (row != null && Convert.ToInt32(row["ID"]) == currentID)
                //        {
                //            grvDanhMuc.FocusedRowHandle = i;
                //            grvDanhMuc.SelectRow(i);
                //            break;
                //        }
                //    }
                //}));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi di chuyển xuống: " + ex.Message);
            }
        }

        private void grvDetail_GopY_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var dr = grvDetail_GopY.GetDataRow(e.RowHandle);
            if (dr == null) return;

            if (dr["LoiNang"] != DBNull.Value && Convert.ToBoolean(dr["LoiNang"]))
            {
                e.Appearance.ForeColor = Color.Red;

            }
        }

        private void frmKiemMauDauChuyen_Load(object sender, EventArgs e)
        {

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (searchLookUpEdit_Line.EditValue == null || searchLookUpEdit_MaHang.EditValue == null)
                {
                    MessageBox.Show("Vui lòng chọn Chuyền và Lệnh Sản Xuất trước khi xem chi tiết!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string line = searchLookUpEdit_Line.EditValue.ToString();
                string lenhSX = searchLookUpEdit_MaHang.EditValue.ToString();

                string hostQty = "";
                try
                {
                    hostQty = (string)settingsReader.GetValue("HostQty", typeof(String));
                }
                catch
                {
                    MessageBox.Show("Không tìm thấy cấu hình 'HostQty' trong file App.config!", "Lỗi cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string url = string.Format("http://{0}/InLine/BC_KiemMauDauChuyen?line={1}&lenhSX={2}", hostQty, line, lenhSX);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở liên kết web: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            var frm = new frmKiemMauDauChuyen_NhomKhacPhuc();
            frm.FormClosed += (s, args) => LoadNhomKhacPhucToRepository();
            frm.Show();
        }

        private void searchLookUpEdit1View_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {

        }

        private void searchLookUpEdit1View_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {

        }

        private void searchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

        }

        private async void grvHinh_DoubleClick(object sender, EventArgs e)
        {
            var fieldName = grvHinh.FocusedColumn.FieldName;
            if (fieldName == "UrlImage")
            {
                DataRow drFocus = grvHinh.GetFocusedDataRow();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string sourceFileName = openFileDialog.FileName;
                    var fileSave = ReplaceSpecialCharacterssize(Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName)) + ".png";
                    string fileName = Guid.NewGuid().ToString() + ".png";
                    var result = await UploadImage("", sourceFileName, fileName);
                    if (result)
                    {
                        grvHinh.SetRowCellValue(grvHinh.FocusedRowHandle, "Image_Link", Path.Combine(@"\Uploads\KiemDauChuyen", fileName));
                        SaveHinh();
                    }

                }
            }
        }
        int[] rowCopy;
        private void CopySelectedRowsToClipboard(bool includeHeader = false, GridView gridView = null)
        {
            var view = gridView;
            //if (view == null || view.SelectedRowsCount == 0) return;

            //var sb = new StringBuilder();

            //var visibleColumns = view.VisibleColumns.Cast<GridColumn>()
            //    .Where(c => c.Visible && c.FieldName != "STT")
            //    .OrderBy(c => c.VisibleIndex)
            //    .ToList();
            //if (includeHeader)
            //{
            //    var headers = visibleColumns.Select(c => c.Caption ?? c.FieldName);
            //    sb.AppendLine(string.Join("\t", headers));
            //}
            var selectedHandles = view.GetSelectedRows()
                .Where(h => h >= 0 && view.IsDataRow(h))
                .OrderBy(h => h)
                .ToArray();
            rowCopy = selectedHandles;
            //foreach (int rowHandle in selectedHandles)
            //{
            //    var rowValues = new List<string>();
            //    foreach (var col in visibleColumns)
            //    {
            //        object val = view.GetRowCellValue(rowHandle, col);
            //        string text = Convert.ToString(val ?? "");
            //        if (text.Contains("\t") || text.Contains("\r") || text.Contains("\n") || text.Contains("\""))
            //            text = "\"" + text.Replace("\"", "\"\"") + "\"";

            //        rowValues.Add(text);
            //    }
            //    sb.AppendLine(string.Join("\t", rowValues));
            //}

            //if (sb.Length > 0)
            //    Clipboard.SetText(sb.ToString().TrimEnd('\r', '\n'));
        }
        private void PasteSelectedRowsFromClipboard()
        {
            //string clipboardData = Clipboard.GetText();
            //if (string.IsNullOrWhiteSpace(clipboardData)) return;

            //string[] data = clipboardData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (rowCopy.Length == 0) return;


            var dtSource = grcDetail_GopYKhacPhuc.DataSource as DataTable;

            List<DataRow> rowsToAdd = new List<DataRow>();
            foreach (int rowHandle in rowCopy)
            {
                DataRow dr = grvDetail_GopY.GetDataRow(rowHandle);
                var drNew = dtSource.NewRow();
                KHDongThungLib.CopyDataRow(dr, drNew);
                drNew["ID"] = 0;
                dtSource.Rows.Add(drNew);
                rowsToAdd.Add(dr);
            }
            foreach (var row in rowsToAdd)
            {
                //dtSource.Rows.Add(row); // hoặc dtSource.Rows.Remove(row);
            }
            CalSTT();
            //foreach (var item in data)
            //{
            //    var value = item.Split('\t');
            //    var drNew = dtSource.NewRow();
            //    drNew["ID"] = 0;
            //    drNew["STT"] = 0;
            //    drNew["MaNhom"] = value[0];
            //    drNew["MaLoi"] = value[1];
            //    drNew["MoTa"] = value[2];
            //    drNew["LoiNang"] = value[3];
            //    drNew["LoiNhe"] = value[4];
            //    drNew["BienPhap"] = value[5];
            //    dtSource.Rows.Add(drNew);
            //}
        }

        private void grcDetail_GopYKhacPhuc_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (grvDetail_GopY.SelectedRowsCount > 1)
                {
                    CopySelectedRowsToClipboard(includeHeader: false, grvDetail_GopY);
                }
                else
                {
                    CopySelectedRowsToClipboard(includeHeader: false, grvDetail_GopY);
                }
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                e.Handled = true;   // chặn luôn
                e.SuppressKeyPress = true;
                PasteSelectedRowsFromClipboard();
            }
        }

        private void grcMaHangHinh_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                e.Handled = true;   // chặn luôn
                e.SuppressKeyPress = true;
                HandleCopyImage();
            }
        }
        private async Task HandleCopyImage()
        {
            try
            {
                var drFocus = grvHinh.GetFocusedDataRow();
                if (drFocus is null) return;
                if (Clipboard.ContainsImage())
                {
                    Image img = Clipboard.GetImage();

                    if (img != null)
                    {
                        // Convert sang byte[] nếu cần
                        byte[] imgBytes;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            imgBytes = ms.ToArray();
                        }
                        string Url = "";
                        string objUrl = (string)settingsReader.GetValue("HostQty", typeof(String));
                        var client = new WebClient();

                        if (objUrl != null)
                            Url = string.Format("http://{0}/api/QTY_KiemDauChuyenV2/UploadImage", objUrl);
                        var uri = new Uri(Url);
                        try
                        {
                            string fileName = Guid.NewGuid().ToString() + ".png";
                            client.Headers.Add("action", "");
                            client.Headers.Add("fileName", fileName);

                            var responseBytes = await client.UploadDataTaskAsync(uri, imgBytes);
                            string result = Encoding.UTF8.GetString(responseBytes);
                            if (result.ToUpper() == "TRUE")
                            {
                                drFocus["Image_Link"] = Path.Combine(@"\Uploads\KiemDauChuyen", fileName);
                                SaveHinh();
                            }
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message);

                        }
                    }
                }
            }
            catch (Exception ex) { }

        }

        //private void bandedGridViewNhapThongSo_RowStyle(object sender, RowStyleEventArgs e)
        //{
        //    if (e.RowHandle < 0) return;

        //    bool isOK = false;

        //    foreach (GridColumn col in bandedGridViewNhapThongSo.Columns)
        //    {
        //        if (col.FieldName.Contains("@SaiSo"))
        //        {
        //            var value = bandedGridViewNhapThongSo.GetRowCellValue(e.RowHandle, col);
        //            Console.WriteLine(value);
        //            if (value?.ToString() != "" && value?.ToString().Trim() != "OK")
        //            {
        //                isOK = true;
        //                break;
        //            }
        //        }
        //    }

        //    if (isOK)
        //    {
        //        //e.Appearance.BackColor = Color.Red;
        //        e.Appearance.ForeColor = Color.Red;
        //        e.HighPriority = true;
        //    }
        //}
        private void bandedGridViewNhapThongSo_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;

            bool hasError = false;

            var valAm = bandedGridViewNhapThongSo.GetRowCellValue(e.RowHandle, "SaiSoAm");
            var valDuong = bandedGridViewNhapThongSo.GetRowCellValue(e.RowHandle, "SaiSoDuong");

            double saiSoAm = valAm != DBNull.Value && valAm != null ? Convert.ToDouble(valAm) : 0;
            double saiSoDuong = valDuong != DBNull.Value && valDuong != null ? Convert.ToDouble(valDuong) : 0;

            foreach (GridColumn col in bandedGridViewNhapThongSo.Columns)
            {
                if (col.FieldName.Contains("@SaiSo"))
                {
                    var cellValue = bandedGridViewNhapThongSo.GetRowCellValue(e.RowHandle, col)?.ToString();

                    if (!string.IsNullOrEmpty(cellValue) && cellValue != "√")
                    {
                        double saiSoThucTe = 0;

                        if (double.TryParse(cellValue.Replace("+", ""), out saiSoThucTe))
                        {
                            if (saiSoThucTe < (saiSoAm * -1) || saiSoThucTe > saiSoDuong)
                            {
                                hasError = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (hasError)
            {
                e.Appearance.ForeColor = Color.Red;
                e.HighPriority = true;
            }
        }
        //-------------------------------------------------------------------------------XỬ lý xuấ Excel
        #region Export Excel 
        private void barButtonItemExporExcell_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gGuid == Guid.Empty)
            {
                MessageBox.Show("Vui lòng lưu phiếu hoặc chọn phiếu trước khi xuất Excel!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Files (*.xlsx)|*.xlsx";
            var dtSourceMH = searchLookUpEdit_MaHang.Properties.DataSource as DataTable;
            var drKH = dtSourceMH.AsEnumerable().Where(x => x["LenhSX"].ToString() == searchLookUpEdit_MaHang.EditValue.ToString()).FirstOrDefault();
            string defaultName = $"{drKH["MaHang"].ToString()}_{searchLookUpEdit_PO.Text}_{searchLookUpEdit_Line.Text}.xlsx";

            sfd.FileName = SanitizeFileName(defaultName);

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // Sử dụng con trỏ chuột hệ thống thay cho clsWaitForm
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    ExportToExcel(sfd.FileName);

                    Cursor.Current = Cursors.Default;
                    if (MessageBox.Show("Xuất file thành công! Bạn có muốn mở file không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show("Lỗi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string CleanXmlChars(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return Regex.Replace(input, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", string.Empty);
        }

        private string SanitizeFileName(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            string result = input;
            foreach (char c in invalid) result = result.Replace(c.ToString(), "");
            result = Regex.Replace(result.Trim(), @"\s+", " ");
            return result;
        }

        private void SetBorder(OfficeOpenXml.ExcelRange range)
        {
            range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            range.Style.Border.Top.Color.SetColor(Color.Gray);
            range.Style.Border.Bottom.Color.SetColor(Color.Gray);
            range.Style.Border.Left.Color.SetColor(Color.Gray);
            range.Style.Border.Right.Color.SetColor(Color.Gray);
        }

        private byte[] GetImageBytesFromServer(string relativeLink)
        {
            try
            {
                string urlHost = (string)settingsReader.GetValue("HostQty", typeof(String));
                string url = "http://" + urlHost + relativeLink.Replace("\\", "/");
                using (var wc = new System.Net.WebClient())
                {
                    return wc.DownloadData(url);
                }
            }
            catch { return null; }
        }
        #endregion
        private void ExportToExcel(string saveFilePath)
        {
            string templatePath = Application.StartupPath + @"\Templates\BCKiemDauChuyen.xlsx";
            FileInfo templateFile = new FileInfo(templatePath);
            string forderImageTable = System.IO.Path.Combine(Application.StartupPath, "Resources");
            if (!templateFile.Exists)
                throw new Exception("Không tìm thấy file Template tại: " + templatePath);

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new OfficeOpenXml.ExcelPackage(templateFile))
            {
                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws == null) ws = package.Workbook.Worksheets.Add("BienBanDuyetMau");

                ws.Cells.Style.Font.Name = "Times New Roman";
                ws.Cells.Style.Font.Size = 11;

                // ==========================================
                // 1. DATA NGUỒN TỪ WINFORM VÀ HELPER
                // ==========================================
                DataTable dtDanhMuc = grcDanhMuc.DataSource as DataTable;
                DataTable dtGopY = grcDetail_GopYKhacPhuc.DataSource as DataTable;
                DataTable dtThongSo = dgrNhapThongSo.DataSource as DataTable;

                Action<OfficeOpenXml.ExcelRange> SafeMerge = (rng) =>
                {
                    try { rng.Merge = false; } catch { }
                    try { rng.Merge = true; } catch { }
                };

                string slMaHang = "";
                string strMaHang = "";
                if (searchLookUpEdit_MaHang.Properties.DataSource is DataTable dtMaHang)
                {
                    var drKH = dtMaHang.AsEnumerable().FirstOrDefault(x => x["LenhSX"].ToString() == searchLookUpEdit_MaHang.EditValue?.ToString());
                    if (drKH != null)
                    {
                        if (dtMaHang.Columns.Contains("SoLuong")) slMaHang = drKH["SoLuong"]?.ToString();
                        if (dtMaHang.Columns.Contains("MaHang")) strMaHang = drKH["MaHang"]?.ToString();
                    }
                }

                // ==========================================
                // LẤY TOÀN BỘ ẢNH CHO TẤT CẢ CÁC DÒNG
                // ==========================================
                DataTable dtAllHinh = CreateTblImage();
                if (dtGopY != null && dtGopY.Rows.Count > 0)
                {
                    foreach (DataRow r in dtGopY.Rows)
                    {
                        string idKiem = r["ID"]?.ToString();
                        if (!string.IsNullOrEmpty(idKiem) && idKiem != "0")
                        {
                            try
                            {
                                string urlImg = URL + $"QTY_KiemDauChuyen/Get?action=GetImage&para1={idKiem}";
                                string jsonImg = Task.Run(async () => await _clientExtension.GetAsnyc(urlImg)).Result;
                                DataTable dtTemp = JsonConvert.DeserializeObject<DataTable>(jsonImg);

                                if (dtTemp != null && dtTemp.Rows.Count > 0)
                                {
                                    foreach (DataRow imgRow in dtTemp.Rows)
                                    {
                                        dtAllHinh.ImportRow(imgRow);
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }

                // ==========================================
                // TÍNH TOÁN ĐỘ RỘNG CỘT ĐỘNG DỰA TRÊN SỐ LƯỢNG ẢNH TỐI ĐA
                // ==========================================
                int maxTruocCount = 0, maxSauCount = 0;
                if (dtGopY != null && dtAllHinh != null)
                {
                    foreach (DataRow r in dtGopY.Rows)
                    {
                        string idKiem = r["ID"].ToString();
                        if (!string.IsNullOrEmpty(idKiem))
                        {
                            DataRow[] imgs = dtAllHinh.Select($"ID_Kiem = '{idKiem}'");
                            maxTruocCount = Math.Max(maxTruocCount, imgs.Count(x => x["Image_Type"]?.ToString() == "0"));
                            maxSauCount = Math.Max(maxSauCount, imgs.Count(x => x["Image_Type"]?.ToString() == "1"));
                        }
                    }
                }

                int colsPerImg = 2;
                int minImgCols = 4;
                int truocColCount = Math.Max(minImgCols, maxTruocCount * colsPerImg);
                int sauColCount = Math.Max(minImgCols, maxSauCount * colsPerImg);

                int truocColStart = 19;
                int truocColEnd = truocColStart + truocColCount - 1;
                int sauColStart = truocColEnd + 1;
                int sauColEnd = sauColStart + sauColCount - 1;
                int theodoiStart = sauColEnd + 1;
                int theodoiEnd = theodoiStart + 3;
                int kyStart = theodoiEnd + 1;
                int kyEnd = kyStart + 1;
                int ngaykyStart = kyEnd + 1;
                int ngaykyEnd = ngaykyStart + 1;
                if (ngaykyEnd < 36)
                {
                    int gap = 36 - ngaykyEnd;
                    theodoiEnd += gap;

                    kyStart = theodoiEnd + 1;
                    kyEnd = kyStart + 1;
                    ngaykyStart = kyEnd + 1;
                    ngaykyEnd = ngaykyStart + 1;
                }
                int totalCols = Math.Max(36, ngaykyEnd);
                int standardCols = 36;

                // ==========================================
                // 2. HEADER (Dòng 4 -> 7)
                // ==========================================
                for (int i = 1; i <= totalCols; i++) ws.Column(i).Width = 4;

                Action<string, string> SetMetaLabel = (range, text) =>
                {
                    var cell = ws.Cells[range]; SafeMerge(cell); cell.Value = text;
                    cell.Style.Font.UnderLine = true;
                    cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    SetBorder(cell);
                };
                Action<string, string> SetMetaValue = (range, text) =>
                {
                    var cell = ws.Cells[range]; SafeMerge(cell); cell.Value = text;
                    cell.Style.Font.Bold = true;
                    cell.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    SetBorder(cell);
                };

                SetMetaLabel("A4:D4", "Khách hàng:"); SetMetaValue("E4:P4", CleanXmlChars(txtKhachHang.Text));
                SetMetaLabel("Q4:U4", "Chuyền sản xuất:"); SetMetaValue("V4:AJ4", CleanXmlChars(searchLookUpEdit_Line.Text));

                SetMetaLabel("A5:D5", "Style:"); SetMetaValue("E5:P5", CleanXmlChars(strMaHang));

                string sizeAndSL = $"{CleanXmlChars(searchLookUpEdit_Size.Text)} / {CleanXmlChars(txtbox_Soluong.Text)}";
                SetMetaLabel("Q5:U5", "Size kiểm tra/SL:"); SetMetaValue("V5:AJ5", sizeAndSL);

                SetMetaLabel("A6:D6", "PO:"); SetMetaValue("E6:P6", CleanXmlChars(searchLookUpEdit_PO.Text));
                SetMetaLabel("Q6:U6", "InSeam/Màu:"); SetMetaValue("V6:AJ6", $"{CleanXmlChars(searchLookUpEdit_Inseam.Text)} / {CleanXmlChars(searchLookUpEdit_Color.Text)}");

                string cleanSl = CleanXmlChars(slMaHang);
                SetMetaLabel("A7:D7", "Số lượng:");
                SetMetaValue("E7:P7", $"{cleanSl} PCS");
                SetMetaLabel("Q7:U7", "Ngày kiểm tra:"); SetMetaValue("V7:AJ7", dateEdit1.DateTime.ToString("dd/MM/yyyy"));
                for (int i = 4; i <= 7; i++) ws.Row(i).Height = 20;

                // ==========================================
                // 3. MỤC I: CÁC MỤC KIỂM TRA (5 HÀNG TRÊN 1 CỘT)
                // ==========================================
                int rIndex = 9;
                int startRowMuc1 = rIndex;
                var title1 = ws.Cells[rIndex, 1, rIndex, standardCols]; SafeMerge(title1);
                title1.Value = "I/ CÁC MỤC KIỂM TRA";
                title1.Style.Font.Bold = true;
                title1.Style.Font.UnderLine = true;
                title1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                SetBorder(title1);
                ws.Row(rIndex).Height = 22;
                rIndex++;

                var title2 = ws.Cells[rIndex, 1, rIndex, standardCols]; SafeMerge(title2);
                title2.Value = "Lưu ý cách đánh dấu : xem thêm góp ý bên dưới (*) ; chưa thực hiện (Δ) ; đúng (√) ; không đúng (x).";
                title2.Style.Font.Italic = true;
                title2.Style.Font.UnderLine = true;
                SetBorder(title2);
                ws.Row(rIndex).Height = 20;
                rIndex++;

                int dataStartRow = rIndex;
                int maxRows = 5;
                int totalSlots = maxRows * 5;

                try { ws.Cells[dataStartRow, 1, dataStartRow + maxRows - 1, 36].Merge = false; } catch { }

                for (int i = 0; i < maxRows; i++) ws.Row(dataStartRow + i).Height = 20;

                int[][] mapLabel = { new[] { 1, 4 }, new[] { 8, 11 }, new[] { 15, 18 }, new[] { 22, 25 }, new[] { 29, 32 } };
                int[][] mapBox = { new[] { 5, 6 }, new[] { 12, 13 }, new[] { 19, 20 }, new[] { 26, 27 }, new[] { 33, 34 } };

                for (int i = 0; i < totalSlots; i++)
                {
                    int rOffset = i % maxRows;
                    int cBlock = i / maxRows;
                    int currentRow = dataStartRow + rOffset;

                    var lblRange = ws.Cells[currentRow, mapLabel[cBlock][0], currentRow, mapLabel[cBlock][1]];
                    var boxRange = ws.Cells[currentRow, mapBox[cBlock][0], currentRow, mapBox[cBlock][1]];
                    var lblFirstCell = ws.Cells[currentRow, mapLabel[cBlock][0]];
                    var boxFirstCell = ws.Cells[currentRow, mapBox[cBlock][0]];

                    if (dtDanhMuc != null && i < dtDanhMuc.Rows.Count)
                    {
                        DataRow row = dtDanhMuc.Rows[i];

                        string labelText = CleanXmlChars(row["Code"]?.ToString());
                        string valStr = row["Value"]?.ToString();
                        string symbol = valStr == "0" ? "*" : valStr == "1" ? "Δ" : valStr == "2" ? "√" : valStr == "3" ? "✗" : "";
                        bool isError = !string.IsNullOrEmpty(symbol) && symbol != "√";

                        lblFirstCell.IsRichText = true;
                        lblFirstCell.RichText.Clear();
                        var rtLabel = lblFirstCell.RichText.Add(labelText);
                        rtLabel.FontName = "Times New Roman";
                        rtLabel.Size = 11;

                        if (isError) rtLabel.Color = Color.FromArgb(220, 53, 69);

                        if (dtDanhMuc.Columns.Contains("Note") && !string.IsNullOrWhiteSpace(row["Note"]?.ToString()))
                        {
                            var rtNote = lblFirstCell.RichText.Add("\n" + CleanXmlChars(row["Note"].ToString()));
                            rtNote.FontName = "Times New Roman";
                            rtNote.Size = 9;
                            rtNote.Italic = true;
                            rtNote.Color = Color.FromArgb(108, 117, 125);
                            ws.Row(currentRow).Height = 37;
                        }

                        boxFirstCell.Value = symbol;
                        boxFirstCell.Style.Font.Bold = true;
                        if (isError) boxFirstCell.Style.Font.Color.SetColor(Color.FromArgb(220, 53, 69));
                    }
                    else
                    {
                        lblFirstCell.Value = ""; boxFirstCell.Value = "";
                    }

                    lblRange.Merge = true; boxRange.Merge = true;

                    lblRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    lblRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    lblRange.Style.WrapText = true; SetBorder(lblRange);

                    boxRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    boxRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; SetBorder(boxRange);
                }

                rIndex = dataStartRow + maxRows;

                int[][] gapCols = { new[] { 7, 7 }, new[] { 14, 14 }, new[] { 21, 21 }, new[] { 28, 28 }, new[] { 35, standardCols } };
                foreach (var gap in gapCols)
                {
                    var gapRng = ws.Cells[dataStartRow, gap[0], dataStartRow + maxRows - 1, gap[1]];
                    gapRng.Merge = true; SetBorder(gapRng);
                }

                var rngMuc1 = ws.Cells[startRowMuc1, 1, rIndex - 1, standardCols];
                rngMuc1.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                rngMuc1.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                rngMuc1.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                rngMuc1.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                // ==========================================
                // 4. MỤC II: GÓP Ý VÀ KHẮC PHỤC
                // ==========================================
                rIndex++;
                SafeMerge(ws.Cells[rIndex, 1, rIndex, totalCols]);
                ws.Cells[rIndex, 1].Value = "II/ CÁC GÓP Ý VÀ KHẮC PHỤC";
                ws.Cells[rIndex, 1].Style.Font.Bold = true;
                ws.Cells[rIndex, 1].Style.Font.UnderLine = true;
                ws.Cells[rIndex, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws.Row(rIndex).Height = 22; rIndex++;

                string[] headers = { "STT", "Mô tả lỗi", "Lỗi\nnặng", "Lỗi\nnhẹ", "Biện pháp xử lý", "Hình ảnh trước", "Hình ảnh sau", "Theo dõi thực hiện", "Ký", "Ngày ký" };
                int[][] hCols = {
            new[] { 1, 2 }, new[] { 3, 8 }, new[] { 9, 10 }, new[] { 11, 12 }, new[] { 13, 18 },
            new[] { truocColStart, truocColEnd }, new[] { sauColStart, sauColEnd }, new[] { theodoiStart, theodoiEnd }, new[] { kyStart, kyEnd }, new[] { ngaykyStart, ngaykyEnd }
        };

                ws.Row(rIndex).Height = 28;
                for (int hi = 0; hi < headers.Length; hi++)
                {
                    var rng = ws.Cells[rIndex, hCols[hi][0], rIndex, hCols[hi][1]]; SafeMerge(rng);
                    rng.Value = headers[hi]; rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(252, 228, 214));
                    rng.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    rng.Style.WrapText = true; SetBorder(rng);
                }
                rIndex++;

                if (dtGopY != null && dtGopY.Rows.Count > 0)
                {
                    DataView dv = dtGopY.DefaultView;
                    dv.Sort = "MaNhom ASC, STT ASC";
                    DataTable sortedDt = dv.ToTable();

                    string currentGroup = "";
                    foreach (DataRow row in sortedDt.Rows)
                    {
                        string tenNhomDB = CleanXmlChars(row["TenNhom"]?.ToString());
                        string maNhom = CleanXmlChars(row["MaNhom"]?.ToString());
                        string fullGroup = $"{maNhom}. {tenNhomDB}";
                        bool isLoiNang = row["MucDoLoi"]?.ToString() == "1";

                        if (fullGroup != currentGroup)
                        {
                            currentGroup = fullGroup;
                            var gRng = ws.Cells[rIndex, 1, rIndex, totalCols]; SafeMerge(gRng);
                            gRng.Value = currentGroup; gRng.Style.Font.Bold = true;
                            gRng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            SetBorder(gRng); ws.Row(rIndex).Height = 22; rIndex++;
                        }

                        // --- 1. LẤY DANH SÁCH ẢNH VÀ KIỂM TRA CÓ GHI CHÚ KHÔNG ---
                        List<DataRow> listTruoc = new List<DataRow>();
                        List<DataRow> listSau = new List<DataRow>();
                        if (dtAllHinh != null && row["ID"] != DBNull.Value)
                        {
                            string idKiem = row["ID"].ToString();
                            DataRow[] imgs = dtAllHinh.Select($"ID_Kiem = '{idKiem}'");
                            listTruoc = imgs.Where(x => x["Image_Type"]?.ToString() == "0").ToList();
                            listSau = imgs.Where(x => x["Image_Type"]?.ToString() == "1").ToList();
                        }

                        bool coGhiChuTruoc = listTruoc.Any(x => dtAllHinh.Columns.Contains("NoteImage") && !string.IsNullOrWhiteSpace(x["NoteImage"]?.ToString()));
                        bool coGhiChuSau = listSau.Any(x => dtAllHinh.Columns.Contains("NoteImage") && !string.IsNullOrWhiteSpace(x["NoteImage"]?.ToString()));
                        bool coGhiChu = coGhiChuTruoc || coGhiChuSau;

                        int totalRows = coGhiChu ? 2 : 1; // Nếu có ghi chú, chiếm 2 hàng

                        ws.Row(rIndex).Height = 40;
                        if (coGhiChu) ws.Row(rIndex + 1).Height = 18;

                        // Hàm Merge động theo số hàng (totalRows)
                        Action<int[], string, bool> SetMainCell = (cols, val, center) =>
                        {
                            var rng = ws.Cells[rIndex, cols[0], rIndex + totalRows - 1, cols[1]]; SafeMerge(rng);
                            rng.Value = val; rng.Style.WrapText = true; SetBorder(rng);
                            rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            if (center) rng.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            if (isLoiNang) rng.Style.Font.Color.SetColor(Color.FromArgb(180, 0, 0));
                        };

                        // Đổ các cột Text (sẽ tự động Merge 2 hàng dọc nếu có ghi chú)
                        SetMainCell(hCols[0], row["STT"]?.ToString(), true);
                        string maLoi = row.Table.Columns.Contains("MaLoi") ? row["MaLoi"]?.ToString().Trim() : "";
                        string tenLoi = "";

                        if (!string.IsNullOrEmpty(maLoi))
                        {
                            DataTable dtMaLoi = repositoryItemSearchLookUpEdit2.DataSource as DataTable;
                            if (dtMaLoi != null)
                            {
                                var drLoi = dtMaLoi.AsEnumerable().FirstOrDefault(x => x["MaLoi"]?.ToString() == maLoi);
                                if (drLoi != null && dtMaLoi.Columns.Contains("TenLoi"))
                                {
                                    tenLoi = drLoi["TenLoi"]?.ToString().Trim();
                                }
                            }
                        }

                        string moTa = row["MoTa"]?.ToString().Trim();

                        string prefixLoi = !string.IsNullOrEmpty(tenLoi) ? tenLoi : maLoi;
                        string hienThiMoTa = string.IsNullOrEmpty(prefixLoi) ? moTa : $"{prefixLoi}: {moTa}";

                        SetMainCell(hCols[1], CleanXmlChars(hienThiMoTa), false);

                        SetMainCell(hCols[1], CleanXmlChars(hienThiMoTa), false);
                        SetMainCell(hCols[2], isLoiNang ? "✓" : "", true);
                        SetMainCell(hCols[3], row["MucDoLoi"]?.ToString() == "0" ? "✓" : "", true);
                        SetMainCell(hCols[4], CleanXmlChars(row["BienPhap"]?.ToString()), false);
                        SetMainCell(hCols[7], CleanXmlChars(row["TheoDoi"]?.ToString()), false);

                        // --- 2. ĐÓNG KHUNG BỌC NGOÀI VÙNG HÌNH ẢNH TRƯỚC/SAU ---
                        var rngPicTruoc = ws.Cells[rIndex, hCols[5][0], rIndex + totalRows - 1, hCols[5][1]]; SetBorder(rngPicTruoc);
                        var rngPicSau = ws.Cells[rIndex, hCols[6][0], rIndex + totalRows - 1, hCols[6][1]]; SetBorder(rngPicSau);

                        // Xử lý Merge khung bên trong Hình ảnh Trước
                        if (coGhiChu && !coGhiChuTruoc)
                        {
                            SafeMerge(ws.Cells[rIndex, hCols[5][0], rIndex + 1, hCols[5][1]]);
                        }
                        else
                        {
                            SafeMerge(ws.Cells[rIndex, hCols[5][0], rIndex, hCols[5][1]]); // Merge dòng trên chứa ảnh
                            if (coGhiChuTruoc)
                            {
                                for (int ni = 0; ni < maxTruocCount; ni++)
                                {
                                    int ns = hCols[5][0] + ni * colsPerImg;
                                    int ne = ns + colsPerImg - 1;
                                    SafeMerge(ws.Cells[rIndex + 1, ns, rIndex + 1, ne]); // Merge các ô ghi chú bên dưới
                                }
                            }
                        }

                        // Xử lý Merge khung bên trong Hình ảnh Sau
                        if (coGhiChu && !coGhiChuSau)
                        {
                            SafeMerge(ws.Cells[rIndex, hCols[6][0], rIndex + 1, hCols[6][1]]);
                        }
                        else
                        {
                            SafeMerge(ws.Cells[rIndex, hCols[6][0], rIndex, hCols[6][1]]);
                            if (coGhiChuSau)
                            {
                                for (int ni = 0; ni < maxSauCount; ni++)
                                {
                                    int ns = hCols[6][0] + ni * colsPerImg;
                                    int ne = ns + colsPerImg - 1;
                                    SafeMerge(ws.Cells[rIndex + 1, ns, rIndex + 1, ne]);
                                }
                            }
                        }

                        int picHeightTruoc = (coGhiChu && !coGhiChuTruoc) ? 80 : 56;
                        int picHeightSau = (coGhiChu && !coGhiChuSau) ? 80 : 56;

                        // --- 3. ĐỔ ẢNH VÀ GHI CHÚ TRƯỚC ---
                        for (int imgIdx = 0; imgIdx < listTruoc.Count; imgIdx++)
                        {
                            byte[] imgBytes = GetImageBytesFromServer(listTruoc[imgIdx]["Image_Link"].ToString());
                            if (imgBytes != null)
                            {
                                MemoryStream ms = new MemoryStream(imgBytes);
                                Image img = Image.FromStream(ms);

                                var pic = ws.Drawings.AddPicture($"T_{Guid.NewGuid():N}", img);
                                int colIdx0 = (hCols[5][0] - 1) + (imgIdx * colsPerImg);
                                pic.SetPosition(rIndex - 1, 3, colIdx0, 4);
                                pic.SetSize(40, 40);
                                pic.Print = true;
                            }

                            if (coGhiChuTruoc && dtAllHinh.Columns.Contains("NoteImage"))
                            {
                                int ns = hCols[5][0] + imgIdx * colsPerImg;
                                int ne = ns + colsPerImg - 1;
                                var noteRng = ws.Cells[rIndex + 1, ns, rIndex + 1, ne];
                                noteRng.Value = CleanXmlChars(listTruoc[imgIdx]["NoteImage"]?.ToString()) ?? "";
                                noteRng.Style.Font.Size = 9;
                                noteRng.Style.Font.Italic = true;
                                noteRng.Style.Font.Color.SetColor(Color.FromArgb(45, 106, 53));
                                noteRng.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                noteRng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            }
                        }

                        // --- 4. ĐỔ ẢNH VÀ GHI CHÚ SAU ---
                        for (int imgIdx = 0; imgIdx < listSau.Count; imgIdx++)
                        {
                            byte[] imgBytes = GetImageBytesFromServer(listSau[imgIdx]["Image_Link"].ToString());
                            if (imgBytes != null)
                            {
                                MemoryStream ms = new MemoryStream(imgBytes);
                                Image img = Image.FromStream(ms);
                                var pic = ws.Drawings.AddPicture($"S_{Guid.NewGuid():N}", img);
                                int colIdx0 = (hCols[6][0] - 1) + (imgIdx * colsPerImg);
                                pic.SetPosition(rIndex - 1, 3, colIdx0, 4);
                                pic.SetSize(40, 40);
                                pic.Print = true;
                            }

                            if (coGhiChuSau && dtAllHinh.Columns.Contains("NoteImage"))
                            {
                                int ns = hCols[6][0] + imgIdx * colsPerImg;
                                int ne = ns + colsPerImg - 1;
                                var noteRng = ws.Cells[rIndex + 1, ns, rIndex + 1, ne];
                                noteRng.Value = CleanXmlChars(listSau[imgIdx]["NoteImage"]?.ToString()) ?? "";
                                noteRng.Style.Font.Size = 9;
                                noteRng.Style.Font.Italic = true;
                                noteRng.Style.Font.Color.SetColor(Color.FromArgb(45, 106, 53));
                                noteRng.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                noteRng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                            }
                        }

                        // Các cột Ký Tên (Cũng tự động Merge dọc theo totalRows)
                        var rngKy = ws.Cells[rIndex, hCols[8][0], rIndex + totalRows - 1, hCols[8][1]]; SafeMerge(rngKy); SetBorder(rngKy);
                        var rngNgayKy = ws.Cells[rIndex, hCols[9][0], rIndex + totalRows - 1, hCols[9][1]]; SafeMerge(rngNgayKy);

                        //if (row["NgaySign"] != DBNull.Value && !string.IsNullOrEmpty(row["NgaySign"].ToString()))
                        //    rngNgayKy.Value = Convert.ToDateTime(row["NgaySign"]).ToString("dd/MM/yyyy");

                        rngNgayKy.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        rngNgayKy.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        SetBorder(rngNgayKy);

                        rIndex += totalRows;
                    }
                }

                // ==========================================
                // 5. BẢNG CHỮ KÝ XÁC NHẬN
                // ==========================================
                rIndex++;
                var rngSignTitle = ws.Cells[rIndex, 1, rIndex, standardCols]; SafeMerge(rngSignTitle);
                rngSignTitle.Value = "Người kiểm tra & chịu trách nhiệm: (xác nhận rõ đã đọc & ghi ngày)";
                rngSignTitle.Style.Font.Bold = true; rngSignTitle.Style.Font.UnderLine = true;
                ws.Row(rIndex).Height = 22; rIndex++;

                var signRows = new[] {
            new { Label = "NV tiêu chuẩn kỹ thuật:", Value = txtNguoiTao.Text },
            new { Label = "Kỹ thuật triển khai:",     Value = txtKTTK.Text },
            new { Label = "QC inline:",               Value = txtQC.Text },
            new { Label = "Chuyền trưởng:",           Value = txtLineManager.Text }
        };

                int signStartRow = rIndex;
                foreach (var item in signRows)
                {
                    ws.Row(rIndex).Height = 25;
                    var lblRng = ws.Cells[rIndex, 1, rIndex, 6]; SafeMerge(lblRng);
                    lblRng.Value = item.Label; SetBorder(lblRng);
                    lblRng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    var valRng = ws.Cells[rIndex, 7, rIndex, 15]; SafeMerge(valRng);
                    valRng.Value = CleanXmlChars(item.Value); SetBorder(valRng);
                    valRng.Style.Font.Bold = true;
                    rIndex++;
                }

                var rngNvTcktLabel = ws.Cells[signStartRow, 17, signStartRow + 1, standardCols];
                SafeMerge(rngNvTcktLabel);
                rngNvTcktLabel.Value = "NV TCKT Chụp hình gửi lên group chuyền đang sản xuất";
                rngNvTcktLabel.Style.Font.Italic = true;
                rngNvTcktLabel.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                rngNvTcktLabel.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                SetBorder(rngNvTcktLabel);

                var rngGhiChuLabel = ws.Cells[signStartRow + 2, 17, signStartRow + 2, 22]; SafeMerge(rngGhiChuLabel);
                rngGhiChuLabel.Value = "Ghi chú:"; rngGhiChuLabel.Style.Font.Bold = true;
                rngGhiChuLabel.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                SetBorder(rngGhiChuLabel);

                var rngGhiChuVal = ws.Cells[signStartRow + 2, 23, signStartRow + 2, standardCols]; SafeMerge(rngGhiChuVal);
                rngGhiChuVal.Value = CleanXmlChars(txtGhiChu.Text);
                rngGhiChuVal.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                SetBorder(rngGhiChuVal);

                //var rngEmptyRight = ws.Cells[signStartRow + 3, 17, signStartRow + 3, totalCols];
                //SafeMerge(rngEmptyRight);
                //SetBorder(rngEmptyRight);

                // ==========================================
                // 6. MỤC III: THÔNG SỐ ĐO
                // ==========================================
                rIndex++;
                var rngTSTitle = ws.Cells[rIndex, 1, rIndex, standardCols]; SafeMerge(rngTSTitle);
                rngTSTitle.Value = "III/ THÔNG SỐ ĐO (Đơn vị: cm)";
                rngTSTitle.Style.Font.Bold = true; rngTSTitle.Style.Font.UnderLine = true;
                ws.Row(rIndex).Height = 22; rIndex++;

                string[] tsHeaders = { "NO", "Vị trí đo", "Dung sai (+/-)", "Size", "Mẫu", "Theo dõi xử lý" };
                int[][] tsCols = { new[] { 1, 2 }, new[] { 3, 15 }, new[] { 16, 19 }, new[] { 20, 23 }, new[] { 24, 28 }, new[] { 29, standardCols } };

                ws.Row(rIndex).Height = 28;
                for (int i = 0; i < tsHeaders.Length; i++)
                {
                    var rng = ws.Cells[rIndex, tsCols[i][0], rIndex, tsCols[i][1]]; SafeMerge(rng);
                    rng.Value = tsHeaders[i]; rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));
                    rng.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    SetBorder(rng);
                }
                rIndex++;

                var sizeNames = sizeDisplay.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                if (sizeNames.Count > 0 && dtThongSo != null)
                {
                    foreach (string currentSize in sizeNames)
                    {
                        var gSizeRng = ws.Cells[rIndex, 1, rIndex, standardCols]; SafeMerge(gSizeRng);
                        gSizeRng.Value = $"Size: {currentSize}";
                        gSizeRng.Style.Font.Bold = true; gSizeRng.Style.Font.Color.SetColor(Color.FromArgb(30, 64, 175));
                        gSizeRng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        gSizeRng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 234, 254));
                        SetBorder(gSizeRng); ws.Row(rIndex).Height = 22; rIndex++;

                        string chuanCol = $"SIZE_{currentSize}@Chuan";
                        string doCol = $"SIZE_{currentSize}@Do";
                        string saisoCol = $"SIZE_{currentSize}@SaiSo";

                        foreach (DataRow tsRow in dtThongSo.Rows)
                        {
                            ws.Row(rIndex).Height = 20;

                            var cSTT = ws.Cells[rIndex, 1, rIndex, 2]; SafeMerge(cSTT);
                            cSTT.Value = dtThongSo.Columns.Contains("NO") ? tsRow["NO"]?.ToString() : "";

                            var cViTri = ws.Cells[rIndex, 3, rIndex, 15]; SafeMerge(cViTri);
                            cViTri.Value = dtThongSo.Columns.Contains("Name") ? CleanXmlChars(tsRow["Name"]?.ToString()) : "";

                            var cDS = ws.Cells[rIndex, 16, rIndex, 19]; SafeMerge(cDS);
                            var parts = new List<string>();
                            string ssAm = dtThongSo.Columns.Contains("SaiSoAm") ? tsRow["SaiSoAm"]?.ToString() : "";
                            string ssDuong = dtThongSo.Columns.Contains("SaiSoDuong") ? tsRow["SaiSoDuong"]?.ToString() : "";
                            if (!string.IsNullOrWhiteSpace(ssAm) && ssAm != "0") parts.Add($"-{ssAm}");
                            if (!string.IsNullOrWhiteSpace(ssDuong) && ssDuong != "0") parts.Add($"+{ssDuong}");
                            cDS.Value = parts.Count > 0 ? string.Join(" / ", parts) : "0";

                            var cSize = ws.Cells[rIndex, 20, rIndex, 23]; SafeMerge(cSize);
                            cSize.Value = dtThongSo.Columns.Contains(chuanCol) ? tsRow[chuanCol]?.ToString() : "";

                            double ssAmN = 0;
                            double ssDuongN = 0;
                            if (dtThongSo.Columns.Contains("SaiSoAm") && tsRow["SaiSoAm"] != DBNull.Value)
                                double.TryParse(tsRow["SaiSoAm"].ToString(), out ssAmN);
                            if (dtThongSo.Columns.Contains("SaiSoDuong") && tsRow["SaiSoDuong"] != DBNull.Value)
                                double.TryParse(tsRow["SaiSoDuong"].ToString(), out ssDuongN);

                            bool isErrorRow = false; 

                            var cMau = ws.Cells[rIndex, 24, rIndex, 28];
                            SafeMerge(cMau);

                            string saiSoVal = dtThongSo.Columns.Contains(saisoCol) ? tsRow[saisoCol]?.ToString().Trim() : "";
                            cMau.Value = saiSoVal;

                            if (!string.IsNullOrEmpty(saiSoVal))
                            {
                                string lowerVal = saiSoVal.ToLower();
                                if (lowerVal == "v" || saiSoVal == "√" || saiSoVal == "OK")
                                {
                                    cMau.Style.Font.Color.SetColor(Color.FromArgb(25, 135, 84));
                                }
                                else
                                {
                                    string cleanVal = saiSoVal.Replace("+", "");
                                    if (double.TryParse(cleanVal, out double num))
                                    {
                                        if (num < (ssAmN * -1) || num > ssDuongN)
                                        {
                                            isErrorRow = true; 
                                            cMau.Style.Font.Color.SetColor(Color.FromArgb(220, 53, 69));
                                        }
                                    }
                                }
                            }

                            var cKetQua = ws.Cells[rIndex, 29, rIndex, standardCols];
                            SafeMerge(cKetQua);
                            cKetQua.Value = dtThongSo.Columns.Contains("Actions") ? CleanXmlChars(tsRow["Actions"]?.ToString()) : "";

                            // Format viền và căn lề cho các cột
                            for (int j = 0; j < tsCols.Length; j++)
                            {
                                var cellRng = ws.Cells[rIndex, tsCols[j][0], rIndex, tsCols[j][1]];
                                SetBorder(cellRng);
                                cellRng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                if (j != 1) cellRng.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            }

                            // 2. Bôi đỏ chữ nguyên dòng nếu cờ isErrorRow được bật
                            if (isErrorRow)
                            {
                                var rowRng = ws.Cells[rIndex, tsCols[0][0], rIndex, tsCols[tsCols.Length - 1][1]];
                                rowRng.Style.Font.Color.SetColor(Color.FromArgb(220, 53, 69));
                            }
                            rIndex++;
                        }
                        Addpicutre(ws, "DongNai1.jpg", 0, 2, 25, 8, 60, 50, forderImageTable);
                    }
                }
                ws.PrinterSettings.PrintArea = ws.Cells[1, 1, rIndex + 2, totalCols];
                ws.HeaderFooter.OddFooter.CenteredText =
    $"Trang {ExcelHeaderFooter.PageNumber} / {ExcelHeaderFooter.NumberOfPages}";
                //            // chỉnh margin
                ws.PrinterSettings.BottomMargin = 0.2M;
                ws.PrinterSettings.FooterMargin = 0.1M;
                // Lưu file
                package.SaveAs(new FileInfo(saveFilePath));
            }
        }


        private void txtbox_Soluong_TextChanged(object sender, EventArgs e)
        {

        }

        private void Addpicutre(ExcelWorksheet worksheet, string sign, int row, int col, int toadorow, int toado, int width, int height, string forderImageTable)
        {
            var pathFolder = forderImageTable;
            string imagePath = Path.Combine(pathFolder, sign);
            // Chèn ảnh vào worksheet
            if (File.Exists(imagePath))
            {
                FileInfo imageFile = new FileInfo(imagePath);
                ExcelPicture picture = worksheet.Drawings.AddPicture(Guid.NewGuid().ToString(), imageFile);
                picture.SetPosition(row, toadorow, col, toado);
                picture.SetSize(width, height);
                picture.Print = true;
            }
        }



        public void ConvertExcelToPDFA(string excelPath, string pdfPath)
        {
            Workbook workbook = new Workbook();
            workbook.LoadDocument(excelPath);

            // 👇 Quan trọng: cho phép in hình
            //workbook.Options.Print.PrintDrawingObjects = true;

            foreach (Worksheet sheet in workbook.Worksheets)
            {
                WorksheetView view = sheet.ActiveView;

                view.Orientation = PageOrientation.Landscape;
                view.PaperKind = PaperKind.A4;

                sheet.PrintOptions.PrintGridlines = false;
                sheet.PrintOptions.PrintHeadings = false;
            }

            PdfExportOptions options = new PdfExportOptions()
            {
                ImageQuality = PdfJpegImageQuality.High,
                ConvertImagesToJpeg = false
            };

            workbook.ExportToPdf(pdfPath, options);
        }
        public void ConvertExcelToPDFB(string excelPath, string pdfPath)
        {
            //excelPath = @"D:\1.Projects\DonHang\ViKing\QLDH_Viking_03022026\NtbSoft.ERP.Win\bin\Debug\TempExcel\BCDoThongSo.xlsx";
            Thread.Sleep(1000);
            foreach (var process in Process.GetProcessesByName("EXCEL"))
            {
                process.Kill();
            }
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbook = null;

            try
            {
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;
                workbook = excelApp.Workbooks.Open(excelPath);
                foreach (Microsoft.Office.Interop.Excel.Worksheet sheet in workbook.Worksheets)
                {
                    var ps = sheet.PageSetup;
                    ps.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;
                    try
                    {
                        ps.PaperSize = Microsoft.Office.Interop.Excel.XlPaperSize.xlPaperA4;
                    }
                    catch (Exception ex)
                    {
                        // fallback nếu lỗi printer
                    }
                }
                // Export PDF
                workbook.ExportAsFixedFormat(
                     Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF,
                    pdfPath,
                    Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityStandard,
                    true,   // include properties
                    false,  // ignore print areas
                    Type.Missing,
                    Type.Missing,
                    false
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Cleanup
                if (workbook != null)
                {
                    workbook.Close(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                }

                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                foreach (var process in Process.GetProcessesByName("EXCEL"))
                {
                    process.Kill();
                }
            }
        }
        public void ConvertExcelToPDF(string excelPath, string pdfPath)
        {
            // Kill Excel trước, rồi chờ chắc chắn đã thoát
            KillAllExcelProcesses();
            Thread.Sleep(1500); // tăng lên 1.5s cho chắc

            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;

            try
            {
                excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;
                excelApp.EnableEvents = false;       // Tắt events
                excelApp.ScreenUpdating = false;     // Tắt screen update
                excelApp.Interactive = false;        // Quan trọng: chặn user interrupt

                // Mở file với retry
                workbook = OpenWorkbookWithRetry(excelApp, excelPath, retryCount: 3);

                foreach (Microsoft.Office.Interop.Excel.Worksheet sheet in workbook.Worksheets)
                {
                    var ps = sheet.PageSetup;
                    ps.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;
                    ps.LeftMargin = excelApp.CentimetersToPoints(0.5);
                    ps.RightMargin = excelApp.CentimetersToPoints(0.4);
                    try
                    {
                        ps.PaperSize = Microsoft.Office.Interop.Excel.XlPaperSize.xlPaperA4;
                    }
                    catch { /* fallback nếu lỗi printer */ }
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            ps.Zoom = false;            // bắt buộc phải có
                            ps.FitToPagesWide = 1;      // Fit tất cả column vào 1 trang ngang
                            ps.FitToPagesTall = false;
                        }
                        catch (Exception ex)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    // không giới hạn chiều dọc
                }

                // Export với retry
                ExportPDFWithRetry(workbook, pdfPath, retryCount: 3);
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Lỗi convert PDF: {ex.Message}");
            }
            finally
            {
                try
                {
                    if (workbook != null)
                    {
                        workbook.Close(false);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                        workbook = null;
                    }
                }
                catch { }

                try
                {
                    if (excelApp != null)
                    {
                        excelApp.Interactive = true; // Restore trước khi Quit
                        excelApp.Quit();
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                        excelApp = null;
                    }
                }
                catch { }

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Thread.Sleep(500);
                KillAllExcelProcesses();
            }
        }

        /// <summary>
        /// Mở workbook với cơ chế retry khi gặp lỗi COM busy (0x800AC472)
        /// </summary>
        private Microsoft.Office.Interop.Excel.Workbook OpenWorkbookWithRetry(
            Microsoft.Office.Interop.Excel.Application excelApp,
            string path,
            int retryCount = 3)
        {
            Exception lastEx = null;
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    return excelApp.Workbooks.Open(
                        path,
                        UpdateLinks: false,
                        ReadOnly: true,        // Chỉ đọc, tránh lock file
                        IgnoreReadOnlyRecommended: true
                    );
                }
                catch (System.Runtime.InteropServices.COMException ex)
                    when ((uint)ex.HResult == 0x800AC472)
                {
                    lastEx = ex;
                    Thread.Sleep(1000 * (i + 1)); // Tăng dần: 1s, 2s, 3s
                }
            }
            return null;
            //throw new Exception($"Không thể mở file sau {retryCount} lần thử.", lastEx);
        }

        /// <summary>
        /// Export PDF với retry khi gặp lỗi COM busy
        /// </summary>
        private void ExportPDFWithRetry(
            Microsoft.Office.Interop.Excel.Workbook workbook,
            string pdfPath,
            int retryCount = 3)
        {
            Exception lastEx = null;
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    workbook.ExportAsFixedFormat(
                        Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF,
                        pdfPath,
                        Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityStandard,
                        true,
                        false,
                        Type.Missing,
                        Type.Missing,
                        false
                    );
                    return; // Thành công
                }
                catch (System.Runtime.InteropServices.COMException ex)
                    when ((uint)ex.HResult == 0x800AC472)
                {
                    lastEx = ex;
                    Thread.Sleep(1000 * (i + 1));
                }
            }
            //return null;
            //throw new Exception($"Không thể export PDF sau {retryCount} lần thử.", lastEx);
        }

        private void KillAllExcelProcesses()
        {
            foreach (var process in Process.GetProcessesByName("EXCEL"))
            {
                try { process.Kill(); process.WaitForExit(3000); }
                catch { }
            }
        }

        private void barButtonItem_PrinPDF_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gGuid == Guid.Empty)
            {
                MessageBox.Show("Vui lòng lưu phiếu hoặc chọn phiếu trước khi xuất PDF!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF Files (*.pdf)|*.pdf";
            var dtSourceMH = searchLookUpEdit_MaHang.Properties.DataSource as DataTable;
            var drMH = dtSourceMH.AsEnumerable().Where(x => x["LenhSX"].ToString() == searchLookUpEdit_MaHang.EditValue.ToString()).FirstOrDefault();

            string defaultName = $"{drMH["MaHang"].ToString()}_{searchLookUpEdit_PO.Text}_{searchLookUpEdit_Line.Text}.pdf";
            sfd.FileName = SanitizeFileName(defaultName);

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(frmWait));
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    string PathFile = Path.Combine(Application.StartupPath, "TempExcel");
                    if (!Directory.Exists(PathFile))
                    {
                        Directory.CreateDirectory(PathFile);
                    }
                    string tempExcelPath = Path.Combine(Application.StartupPath, "TempExcel", Guid.NewGuid().ToString() + ".xlsx");

                    ExportToExcel(tempExcelPath);

                    ConvertExcelToPDF(tempExcelPath, sfd.FileName);

                    if (File.Exists(tempExcelPath))
                    {
                        File.Delete(tempExcelPath);
                    }

                    Cursor.Current = Cursors.Default;
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    if (MessageBox.Show("Xuất file PDF thành công! Bạn có muốn mở file không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                    Cursor.Current = Cursors.Default;
                    //MessageBox.Show("Lỗi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //----------------------------------------------------------------------------------END
        private static string GetRightValue(DataTable dt, int row, int col)
        {
            for (int i = col + 1; i < dt.Columns.Count; i++)
            {
                string value = dt.Rows[row][i]?.ToString().Trim();

                if (string.IsNullOrEmpty(value))
                    continue;

                // Nếu ô đó là label khác thì dừng và trả null
                if (IsHeaderLabel(value))
                    return null;

                return value;
            }

            return null;
        }
        private static bool IsHeaderLabel(string text)
        {
            string t = text.ToLower();

            return t.Contains("inseam")
                || t.Contains("style")
                || t.Contains("size")
                || t.Contains("chuyền")
                || t.Contains("khách")
                || t.Contains("ngày")
                || t.Contains("số lượng")
                || t.Contains("cut/po");
        }
        private static int ConvertMark(string mark)
        {
            if (mark == "*")
                return 0;

            if (mark == "∆")
                return 1;

            if (mark == "√" || mark == "✓" || mark == "ü" || mark.ToUpper() == "V")
                return 2;

            if (mark.ToUpper() == "X")
                return 3;

            return -999;
        }
        private static int RomanToInt(string roman)
        {
            switch (roman)
            {
                case "I": return 1;
                case "II": return 2;
                case "III": return 3;
                case "IV": return 4;
                case "V": return 5;
            }

            return 0;
        }
        #endregion
    }
    public class ExcelQCReader
    {
        public static DataTable ReadExcel(string file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            DataTable dt = new DataTable();

            using (var package = new ExcelPackage(new FileInfo(file)))
            {
                var ws = package.Workbook.Worksheets[0];

                int colCount = ws.Dimension.End.Column;
                int rowCount = ws.Dimension.End.Row;

                for (int c = 1; c <= colCount; c++)
                {
                    dt.Columns.Add("Col" + c);
                }

                for (int r = 1; r <= rowCount; r++)
                {
                    var dr = dt.NewRow();

                    for (int c = 1; c <= colCount; c++)
                    {
                        dr[c - 1] = ws.Cells[r, c].Text;
                    }

                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }
    }
    public class QCParser
    {
        public static List<QCKiemDauChuyenEntity> ParseKiemDauChuyen(DataTable dt)
        {
            List<QCKiemDauChuyenEntity> list = new List<QCKiemDauChuyenEntity>();

            int currentGroup = 0;
            string currentGroupName = "";

            int lastSTT = 0;
            int colLoiNang = -1;
            int colLoiNhe = -1;

            for (int r = 0; r < dt.Rows.Count; r++)
            {
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    string v = dt.Rows[r][c]?.ToString().Trim().ToLower();

                    if (v.Contains("lỗi nặng"))
                        colLoiNang = c;

                    if (v.Contains("lỗi nhẹ"))
                        colLoiNhe = c;
                }

                if (colLoiNang != -1 && colLoiNhe != -1)
                    break;
            }
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                string sttText = dt.Rows[r][0]?.ToString().Trim();
                string mota = dt.Rows[r][1]?.ToString().Trim();

                // ========================
                // detect group
                // ========================

                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    string value = dt.Rows[r][c]?.ToString().Trim();

                    if (string.IsNullOrEmpty(value))
                        continue;

                    if (value.StartsWith("I.")
                     || value.StartsWith("II.")
                     || value.StartsWith("III.")
                     || value.StartsWith("IV."))
                    {
                        string roman = value.Split('.')[0];

                        currentGroup = RomanToInt(roman);
                        currentGroupName = value;

                        goto NEXTROW;
                    }
                }

                // ========================
                // kiểm tra STT
                // ========================

                int stt;

                if (!int.TryParse(sttText, out stt))
                    goto NEXTROW;

                // nếu STT bị reset → stop scan
                if (stt < lastSTT)
                    break;

                lastSTT = stt;

                // ========================
                // detect tick lỗi
                // ========================

                int mucDoLoi = 0;

                string loiNang = colLoiNang >= 0 ? dt.Rows[r][colLoiNang]?.ToString().Trim() : "";
                string loiNhe = colLoiNhe >= 0 ? dt.Rows[r][colLoiNhe]?.ToString().Trim() : "";

                // check có tick
                bool tickNang = !string.IsNullOrEmpty(loiNang);
                bool tickNhe = !string.IsNullOrEmpty(loiNhe);

                if (tickNang)
                    mucDoLoi = 1;

                if (tickNhe)
                    mucDoLoi = 0;

                // ========================s
                // detect biện pháp
                // ========================

                string bienPhap = "";

                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    string value = dt.Rows[r][c]?.ToString().Trim();

                    if (!string.IsNullOrEmpty(value)
                        && value != "✓"
                        && value != "√"
                        && value != "ü")
                    {
                        if (value.Contains("Điều chỉnh")
                         || value.Contains("Cắt")
                         || value.Contains("OK"))
                        {
                            bienPhap = value;
                        }
                    }
                }

                list.Add(new QCKiemDauChuyenEntity
                {
                    STT = stt,
                    MoTa = mota,
                    MucDoLoi = mucDoLoi,
                    BienPhap = bienPhap,
                    TheoDoi = null,
                    MaNhom = currentGroup,
                    TenNhom = currentGroupName
                });

            NEXTROW:;
            }

            return list;
        }

        public static List<QCScannerEntity> Parse(DataTable dt)
        {
            List<QCScannerEntity> list = new List<QCScannerEntity>();

            string lineX = null;
            string style = null;
            string size = null;
            string inseam = null;
            string po = null;

            int startRow = 0;
            int stopRow = dt.Rows.Count;

            // ===== SCAN HEADER =====

            for (int r = 0; r < dt.Rows.Count; r++)
            {
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    string cell = dt.Rows[r][c]?.ToString().Trim();

                    if (string.IsNullOrEmpty(cell))
                        continue;
                    if (cell.Contains("Chuyền"))
                    {
                        if (c + 3 < dt.Columns.Count)
                        {
                            string value = dt.Rows[r][c + 3]?.ToString().Trim();

                            if (!string.IsNullOrEmpty(value))
                                lineX = value; // ANTIGONE
                        }
                    }

                    if (cell.Contains("Style"))
                        style = GetRightValue(dt, r, c); // EA2-FV_PR2GG

                    if (cell.Contains("Size"))
                    {
                        string v = GetRightValue(dt, r, c);
                        if (string.IsNullOrEmpty(v))
                            v = GetDownValue(dt, r, c);

                        size = v; // M
                    }

                    if (cell.Contains("InSeam"))
                    {
                        string v = GetRightValue(dt, r, c);
                        if (string.IsNullOrEmpty(v))
                            v = GetDownValue(dt, r, c);

                        inseam = v; // REG
                    }

                    if (cell.Contains("Cut/PO"))
                    {
                        string v = GetRightValue(dt, r, c);
                        po = string.IsNullOrWhiteSpace(v) ? null : v;
                    }

                    if (cell.Contains("I/CÁC MỤC KIỂM TRA"))
                        startRow = r + 2;

                    if (cell.StartsWith("II/"))
                        stopRow = r;
                }
            }

            int codeIndex = 0;

            // ===== SCAN QC ITEMS =====

            for (int r = startRow; r < stopRow; r++)
            {
                for (int c = 0; c < dt.Columns.Count - 1; c++)
                {
                    string name = dt.Rows[r][c]?.ToString().Trim();

                    if (string.IsNullOrEmpty(name))
                        continue;

                    string mark = dt.Rows[r][c + 1]?.ToString().Trim();

                    int value = ConvertMark(mark);

                    if (value == -999)
                        continue;

                    list.Add(new QCScannerEntity
                    {
                        LineX = lineX,
                        MaHang = style,
                        SizeID = size,
                        DauSizeID = inseam, // REG
                        POID = po,
                        Code = name,
                        Value = value,
                        CreateUser = "admin"
                    });

                    codeIndex++;
                }
            }

            return list;
        }
        // ======================
        // CONVERT MARK
        // ======================
        private static string GetRightValue(DataTable dt, int row, int col)
        {
            for (int i = col + 1; i < dt.Columns.Count; i++)
            {
                string value = dt.Rows[row][i]?.ToString().Trim();

                if (string.IsNullOrEmpty(value))
                    continue;

                // Nếu ô đó là label khác thì dừng và trả null
                if (IsHeaderLabel(value))
                    return null;

                return value;
            }

            return null;
        }

        private DataTable CreateTypeTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("MoTa");
            dt.Columns.Add("MucDoLoi", typeof(int));
            dt.Columns.Add("BienPhap");
            dt.Columns.Add("TheoDoi");
            dt.Columns.Add("MaNhom", typeof(int));
            dt.Columns.Add("TenNhom");

            return dt;
        }
        private static string GetDownValue(DataTable dt, int row, int col)
        {
            // chỉ kiểm tra 3 dòng dưới label
            for (int r = row + 1; r <= row + 3 && r < dt.Rows.Count; r++)
            {
                for (int c = col; c < dt.Columns.Count; c++)
                {
                    string v = dt.Rows[r][c]?.ToString().Trim();

                    if (string.IsNullOrEmpty(v))
                        continue;

                    if (IsHeaderLabel(v))
                        continue;

                    if (int.TryParse(v, out _))
                        continue;

                    // bỏ qua text layout
                    if (v.Contains("Vị trí"))
                        continue;

                    return v;
                }
            }

            return null;
        }
        private static bool IsHeaderLabel(string text)
        {
            string t = text.ToLower();

            return t.Contains("inseam")
                || t.Contains("style")
                || t.Contains("size")
                || t.Contains("chuyền")
                || t.Contains("khách")
                || t.Contains("ngày")
                || t.Contains("số lượng")
                || t.Contains("cut/po");
        }
        private static int ConvertMark(string mark)
        {
            if (mark == "*")
                return 0;

            if (mark == "∆")
                return 1;

            if (mark == "√" || mark == "✓" || mark == "ü")
                return 2;

            if (mark == "x")
                return 3;

            return -999;
        }
        private static int RomanToInt(string roman)
        {
            switch (roman)
            {
                case "I": return 1;
                case "II": return 2;
                case "III": return 3;
                case "IV": return 4;
                case "V": return 5;
            }

            return 0;
        }

    }

}