using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.Erp.Kehoach;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmDinhMucNguyenPhuLieu : DevExpress.XtraEditors.XtraForm
    {

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        string _maDonHang = string.Empty;
        List<DinhMucEntity> lstDinhMuc;
        List<BomNguyenPhuLieuEntity> lstBomNL;
        List<BomNguyenPhuLieuEntity> lstBOMVT;
        List<BomNguyenPhuLieuEntity> lstBOMVTDM;
        List<BomNguyenPhuLieuEntity> lstBOMVTMAU;
        List<BomNguyenPhuLieuEntity> lstBOMVTDMNL;
        List<BomNguyenPhuLieuEntity> lstBOMVTMAUNL;
        List<int> lstRowUpdate = new List<int>();
        int pageIndex = 1;
        int pageSize = 100;
        int _option = 0;
        int _rowAdd = -1;
        int _rowAdd2 = -1;
        int _rowAdd3 = -1;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _allowLapSoDoTNC = true, _ChkCT = false, _ChkTH = false, _NChkCT = false, _NChkTH = false, Chon = false, Ischecked = false;
        string _maHang = string.Empty, MaDH = string.Empty;
        int id = 0;
        bool isRowChanged = false;
        bool indicatorIcon = true;
        int rowhandel = 0, rowhandel1 = 0;
        int FocusedIndex = 0;
        string _maNLold;
        string _maPLOld = string.Empty;
        string _maDHPL = string.Empty;
        string _maBOMPL = string.Empty;
        string _maBOMDM = string.Empty;
        string _maVT = string.Empty;
        DataTable _dtSave;
        DataTable _lstNguyenLieu;
        DataTable tblPL;
        DataTable tblNL;
        DataTable DM;
        DataTable DMNL;
        DataTable MAU;
        DataTable MAUNL;
        DataTable DVT;
        KeyDownControlHandler keyDownControlHandler;
        // Import
        ActionControl actionControlImportCapPhat;
        ActionControl actionControlImportCapThem;
        ActionControl actionControlImportThuHoi;
        // Nhập
        ActionControl actionControlNhapCapPhat;
        ActionControl actionControlNhapCapThem;
        ActionControl actionControlNhapThuHoi;
        ActionControl actionControlRefresh;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        List<ActionControl> lstActionControls;
        RepositoryItemSearchLookUpEdit plEdit = new RepositoryItemSearchLookUpEdit();
        RepositoryItemSearchLookUpEdit nlEdit = new RepositoryItemSearchLookUpEdit();
        RepositoryItemSearchLookUpEdit dvtEdit = new RepositoryItemSearchLookUpEdit();
        
        //moi
        private int _newRowHandle = GridControl.InvalidRowHandle; // Khởi tạo giá trị không hợp lệ

        public frmDinhMucNguyenPhuLieu()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstDinhMuc = new List<DinhMucEntity>();
            //this.splitContainerControl1.SplitRatio = this.ClientSize.Height/2;
            _dtSave = new DataTable();
            

        }
        private void TaoTable()
        {
            _dtSave = new DataTable("dtSave");
            _dtSave.Columns.Add("ID", typeof(int));
            _dtSave.Columns.Add("MaDH", typeof(string));
            _dtSave.Columns.Add("MaNPL", typeof(string));
            _dtSave.Columns.Add("MaVT", typeof(string));
            _dtSave.Columns.Add("TenVT", typeof(string));
            _dtSave.Columns.Add("MaMau", typeof(string));
            _dtSave.Columns.Add("KhoVai", typeof(string));
            _dtSave.Columns.Add("MaDV", typeof(string));
            _dtSave.Columns.Add("DinhMuc", typeof(double));
            _dtSave.Columns.Add("SoLuong", typeof(int));
            _dtSave.Columns.Add("CapPhat", typeof(decimal));
            _dtSave.Columns.Add("CapThem", typeof(decimal));
            _dtSave.Columns.Add("ThuHoi", typeof(decimal));
            _dtSave.Columns.Add("TrangThai", typeof(int));
            _dtSave.Columns.Add("GhiChu", typeof(string));
        }
        private List<ActionControl> InitActionKeyDown()
        {
            // Import
            actionControlImportCapPhat = new ActionControl(BtImportCapPhat, _allowAdd, ActionType.Add, true);
            actionControlImportCapThem = new ActionControl(BtImportCapThem, _allowEdit, ActionType.Edit, true);
            actionControlImportThuHoi = new ActionControl(BtImportThuHoi, _allowDelete, ActionType.Recall, true);
            // Nhập
            actionControlNhapCapPhat = new ActionControl(BtNhapCapPhat, _allowAdd, ActionType.Add, false);
            actionControlNhapCapThem = new ActionControl(BtNhapCapThem, _allowEdit, ActionType.Edit, false);
            actionControlNhapThuHoi = new ActionControl(BtNhapThuHoi, _allowDelete, ActionType.Recall, false);

            // Save
            actionControlSave = new ActionControl(Luu, _allowEdit, ActionType.Save, true);
            //Delete
            actionControlDelete = new ActionControl(Xoa, _allowDelete, ActionType.Delete, true);

            // Refresh
            actionControlRefresh = new ActionControl(BtNapLai, true, ActionType.Refresh, this.btCancel.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlImportCapPhat, actionControlImportCapThem,
                actionControlImportThuHoi, actionControlNhapCapPhat,
                actionControlNhapCapThem,actionControlNhapThuHoi,actionControlRefresh,
                actionControlDelete,actionControlSave };
            return lstActionControls;
        }

        protected override void OnLoad(EventArgs e)
        {
            barEditItemSpinPageSize.EditValue = pageSize;
            barEditItemcbLoc.EditValue = repositoryItemComboBox1.Items[0].ToString();
            //LoadDSDinhMucDonHang();
            LoadBOMPL(false);
            CreateSearchLookUpCL();
            CreateSearchLookUp();
            //CreateSearchLookUpDVTNL();
            //CreateSearchLookUpDVTPL();
            LoadDSDMThongTin();
            CheckPerminsion();
         
            if (barCheckImport.Checked == true)
            {
                barButtonCapThem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barButtonCapPhat.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barButtonThuHoi.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnCapPhat.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                btnCapThem.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                btnThuHoi.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            TaoTable();
            int focusedRowHandle = gridViewDMThongTin.FocusedRowHandle;

            string cellValue = gridViewDMThongTin.GetRowCellValue(focusedRowHandle, "MaDH")?.ToString() ?? "";
            loadDSBomNL();
            _maDHPL = gridViewDMThongTin.GetRowCellValue(0, "MaDH").ToString();
            _maBOMPL = "BOM" + _maDHPL.Substring(2);
        }
        private void CreateSearchLookUpCL()
        {
            string url = string.Format("{0}?", URL + "KhoVatTu/GetPL");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tblPL = JsonConvert.DeserializeObject<DataTable>(json);
            plEdit.DataSource = tblPL;
            plEdit.DisplayMember = "MaVT";
            plEdit.ValueMember = "MaVT";
            plEdit.ShowClearButton = false;
            plEdit.NullText = "[Chọn giá trị]";
            GridView dvView = plEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaVT", Caption = "Mã Vật Tư", Name = "colMaVatTuPL", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TenVT", Caption = "Tên Vật Tư", Name = "colTenVatTuPL", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "Mau", Caption = "Tên Màu", Name = "colMauPL", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "SizeKho", Caption = "Khổ Vải", Name = "colKhoVaiPL", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "DTTinh", Caption = "Đơn Vị Tính", Name = "colDonViTinhPL", Visible = true });

            }
            colMaVatTuPL.ColumnEdit = plEdit;
            //colTenVatTuPL.ColumnEdit = plEdit ;
            //colMauPL.ColumnEdit = plEdit;
            plEdit.EditValueChanged += gridPLView_FocusedRowChanged;
        }

        private void CheckPerminsion()
        {
            SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;

            if (!_allowAdd)
            {
                barCheckImport.Enabled = false;
                barButtonCapThem.Enabled = false;
                barButtonCapPhat.Enabled = false;
                barButtonThuHoi.Enabled = false;
                btnCapPhat.Enabled = false;
                btnCapThem.Enabled = false;
                btnThuHoi.Enabled = false;
            }
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);

        }
        private void splitContainerControl1_SizeChanged(object sender, EventArgs e)
        {
            //splitContainerControl1.SizeChanged -= splitContainerControl1_SizeChanged;
            //splitContainerControl1.SplitterPosition = splitContainerControl1.Height / 2;
        }
        private void CreateSearchLookUp()
        {
            string url = string.Format("{0}?", URL + "KhoVatTu/GetNL");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tblNL = JsonConvert.DeserializeObject<DataTable>(json);
            nlEdit.DataSource = tblNL;
            nlEdit.DisplayMember = "MaVT";
            nlEdit.ValueMember = "MaVT";
            nlEdit.ShowClearButton = false;
            nlEdit.NullText = "[Chọn giá trị]";
            GridView dvView = nlEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaVT", Caption = "Mã Vật Tư", Name = "colMaVatTuNL", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TenVT", Caption = "Tên Vật Tư", Name = "colTenVatTuNL", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "Mau", Caption = "Tên Màu", Name = "colMauNL", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "SizeKho", Caption = "Khổ Vải", Name = "colKhoVaiNL", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "DVTinh", Caption = "Đơn Vị Tính", Name = "colDonViTinhNL", Visible = true });
            }
            colMaVatTuNL.ColumnEdit = nlEdit;
            //colTenVatTuPL.ColumnEdit = plEdit ;
            //colMauPL.ColumnEdit = plEdit;
            nlEdit.EditValueChanged += gridPLView2_FocusedRowChanged;
        }

        private void GridViewDMThongTin_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            _dtSave.Clear();
            isRowChanged = true;
            GridView view = sender as GridView;
            DataRowView dataRowView = view.GetFocusedRow() as DataRowView;
            if (dataRowView != null)
            {
                DataRow row = dataRowView.Row;
                string parameter = string.Format("{0}", row["MaGop"].ToString());
                LoadDSDinhMucDonHang(parameter);
                _maDonHang = row["MaGop"].ToString();
            }
        }
        

        private void LoadDSDMThongTin()
        {
            isRowChanged = false;
            //isFocusedRowChanged = false;
            string url = string.Format("{0}?pageIndex={1}&&pageSize={2}&&mahang={3}", URL + "DonHangTong/GetDonHangTong", pageIndex, pageSize, null);
            string jsonDHT = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonDHT);
            List<DonHangTongEntity> lstDonHangTong = new List<DonHangTongEntity>();
            if (tbl.Rows.Count > 0)
            {

                if (barEditItemcbLoc.EditValue != null)
                {
                    if (RemoveVietnameseTone(barEditItemcbLoc.EditValue.ToString().Trim().ToUpper().Replace(" ", "")).ToString().ToUpper() == "CHUACANDOISANXUAT")
                    {
                        var selectedRows = tbl.AsEnumerable().Where(row => Convert.ToInt32(row["SLTH"]) == 0 && Convert.ToInt32(row["SLCL"]) != 0);
                        if (selectedRows.Any())
                            tbl = selectedRows.CopyToDataTable();
                        else
                        {
                            MessageBox.Show("Dữ liệu lọc không tìm thấy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        var sortedRows = tbl.AsEnumerable().OrderBy(row => row["MaDH"].ToString().Substring(4, row["MaDH"].ToString().Length));

                    }
                    if (RemoveVietnameseTone(barEditItemcbLoc.EditValue.ToString().Trim().ToUpper().Replace(" ", "")).ToString().ToUpper() == "DACANDOISANXUATXONG")
                    {
                        var selectedRows = tbl.AsEnumerable().Where(row => Convert.ToInt32(row["SLCL"]) == 0);
                        if (selectedRows.Any())
                            tbl = selectedRows.CopyToDataTable();
                        else
                        {
                            MessageBox.Show("Dữ liệu lọc không tìm thấy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    if (RemoveVietnameseTone(barEditItemcbLoc.EditValue.ToString().Trim().ToUpper().Replace(" ", "")).ToString().ToUpper() == "CANDOISANXUATCHUAXONG")
                    {
                        var selectedRows = tbl.AsEnumerable().Where(row => Convert.ToInt32(row["SLTH"]) != 0 && Convert.ToInt32(row["SLCL"]) != 0);
                        if (selectedRows.Any())
                            tbl = selectedRows.CopyToDataTable();
                        else
                        {
                            MessageBox.Show("Dữ liệu lọc không tìm thấy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }                    LoadBOMMAUPL(false);
                gridControlDMThongTin.DataSource = tbl;
                if (!isRowChanged)
                {
                    string parameter = string.Format("{0}", tbl.Rows[0]["MaGop"].ToString());
                    LoadDSDinhMucDonHang(parameter);

                }
            }
            else
            {
                NtbSoft.ERP.Libs.clsConvert<DonHangTongEntity> convert = new Libs.clsConvert<DonHangTongEntity>();
                DataTable tblnull = convert.ToDataTable(lstDonHangTong);
                gridControlDMThongTin.DataSource = tblnull;
                //this.gridDinhMucNguyenPhuLieu.DataSource = null;
            }
            gridViewDMThongTin.FocusedRowHandle = rowhandel;

        }
        private void LoadBOMPL(bool IsFromSave)
        {
            try
            {
                clsWaitForm.ShowWaitForm(this, 1000);
                MaDH = gridViewDMThongTin.GetFocusedRowCellValue(colMaDHDMThongTin)?.ToString();
                string url = URL + $"BOMNPL/GetBomPLByMaDH?MaDH={MaDH}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    _dtSave = JsonConvert.DeserializeObject<DataTable>(json);
                    lstBOMVT = JsonConvert.DeserializeObject<List<BomNguyenPhuLieuEntity>>(json);
                }
                gridPLControl.DataSource = lstBOMVT.Count > 0 ? lstBOMVT : null;
                if (IsFromSave && FocusedIndex >= 0)
                {
                    gridPLView.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridPLControl;
                }
                gridPLControl.RefreshDataSource();
                //LoadBOMDMPL(false, lstBOMVT[0].MaVatTu);
                LoadBOMMAUPL(false);
            }
            catch
            {
                //XtraMessageBox.Show("Chưa có dữ liệu xin vui lòng nhập.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void LoadBOMDMPL(bool IsFromSave, string MaVatTu)
        {
            try
             {
                //MaVatTu = gridPLView.GetFocusedRowCellValue(colMaVatTuPL)?.ToString();
                string url = URL + $"BOMNPL/GetBomPLDMByMaDH?MaVatTu={MaVatTu}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (!string.IsNullOrEmpty(json))
                    {
                        DM = JsonConvert.DeserializeObject<DataTable>(json);
                    lstBOMVTDM = JsonConvert.DeserializeObject<List<BomNguyenPhuLieuEntity>>(json);
                }
                gridControlBOMPLDM.DataSource = lstBOMVTDM.Count > 0 ? lstBOMVTDM : null;
                    if (IsFromSave && FocusedIndex >= 0)
                    {
                        //gridBOMDMPLView.FocusedRowHandle = FocusedIndex;
                        this.ActiveControl = gridControlBOMPLDM;
                    }
                gridControlBOMPLDM.RefreshDataSource();

            }
            catch
            {
                XtraMessageBox.Show("Chưa có dữ liệu xin vui lòng nhập.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        private void LoadBOMMAUPL(bool IsFromSave)
        {
            try
            {
                MaDH = gridViewDMThongTin.GetFocusedRowCellValue(colMaDHDMThongTin)?.ToString();
                string _maVT = gridPLView.GetFocusedRowCellValue(colMaVatTuPL)?.ToString();

                    string url = URL + $"BOMNPL/GetPOChiTiet?MaDH={MaDH}&&MaVatTu={_maVT}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (!string.IsNullOrEmpty(json))
                    {
                        MAU = JsonConvert.DeserializeObject<DataTable>(json);
                        lstBOMVTMAU = JsonConvert.DeserializeObject<List<BomNguyenPhuLieuEntity>>(json);

                    }
                    CreateBandSize(MAU, bandedGridView1, gridBandSizePL);
                    gridMauPLControl.DataSource = MAU;
                    if (IsFromSave && FocusedIndex >= 0)
                    {
                        bandedGridView1.FocusedRowHandle = FocusedIndex;
                        this.ActiveControl = gridMauPLControl;
                    }
                    gridMauPLControl.RefreshDataSource();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        private void LoadBOMMAUNL(bool IsFromSave)
        {
            try
            {
                MaDH = gridViewDMThongTin.GetFocusedRowCellValue(colMaDHDMThongTin)?.ToString();
                string _maVT = gridNLView.GetFocusedRowCellValue(colMaVatTuNL)?.ToString();

                    string url = URL + $"BOMNPL/GetPOChiTiet?MaDH={MaDH}&&MaVatTu={_maVT}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (!string.IsNullOrEmpty(json))
                    {
                        MAUNL = JsonConvert.DeserializeObject<DataTable>(json);
                        lstBOMVTMAU = JsonConvert.DeserializeObject<List<BomNguyenPhuLieuEntity>>(json);
                        
                    }
                    CreateBandSize(MAUNL, bandedGridViewMau, gridBandSize);
                    gridControlNLBOMMAU.DataSource = MAUNL;
                    if (IsFromSave && FocusedIndex >= 0)
                    {
                        bandedGridViewMau.FocusedRowHandle = FocusedIndex;
                        this.ActiveControl = gridControlNLBOMMAU;
                    }
                    gridControlNLBOMMAU.RefreshDataSource();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void CreateBandSize(DataTable dt, BandedGridView grvShared, GridBand gridBandSizeA)
        {
            try {
                //DataRow rowPO = bandedGridViewMau.GetFocusedDataRow();

                ClearBand(gridBandSizeA);
                foreach (DataColumn dc in dt.Columns)
                {
                    RepositoryItemCheckEdit checkEdit = new RepositoryItemCheckEdit();
                    checkEdit.ValueChecked = true;
                    checkEdit.ValueUnchecked = false;
                    string colName = dc.ColumnName;
                    if (!colName.Contains('@')) continue;
                    var _sizeID = colName.Split('@')[0];
                    var _size = colName.Split('@')[1];
                    if (!CheckExistBand(_sizeID, gridBandSizeA)) continue;
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = _size;
                    col.FieldName = _sizeID + "@" + _size;
                    col.Name = "col" + _sizeID;
                    col.OptionsColumn.AllowEdit = true;
                    col.OptionsColumn.ReadOnly = false;
                    col.ColumnEdit = checkEdit;
                    //col.ColumnEdit = this.repotxtN0;
                    col.Visible = true;
                    col.Width = 40;
                    //col.OptionsColumn.AllowEdit = false;
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

                    grvShared.Columns.AddRange(new BandedGridColumn[] { col });

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
                    gb.Name = "gb" + "@" + _sizeID;
                    gb.VisibleIndex = 0;
                    gb.Width = 60;
                    grvShared.GridControl.RepositoryItems.Add(checkEdit);
                    gridBandSizeA.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });

                }

            }
            catch (Exception e) 
            {
            }
            
        }
        private void ClearBand(GridBand gbSizeA)
        {
            gbSizeA.Children.Clear();
        }
        private bool CheckExistBand(string size, GridBand gbSizeA)
        {

            GridBand gbCheck = gbSizeA.Children.Where(x => x.Name == "gb" + "@Size" + size).FirstOrDefault();
            if (gbCheck != null) return false;
            return true;
        }
        public string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }
        public void LoadDSDinhMucDonHang(string parameter)
        {
            
            //string parameter
            //string url = string.Format("{0}?pageIndex={1}&&pageSize={2}&&parameter={3}", URL + "DinhMuc/Get", pageIndex, pageSize, parameter);
            string url = string.Format("{0}?parameter={1}", URL + "CanDoiDonHangTong/GetGomNPL", parameter);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0 || tbl == null)
            {
                NtbSoft.ERP.Libs.clsConvert<DinhMucEntity> convert = new Libs.clsConvert<DinhMucEntity>();
                DataTable tblnull = convert.ToDataTable(lstDinhMuc);
                //gridDinhMucNguyenPhuLieu.DataSource = tblnull;
            }
            LoadBOMMAUPL(false);
            LoadBOMMAUNL(false);
            //else
            //gridDinhMucNguyenPhuLieu.DataSource = tbl;
        }

        private void BtImportCapPhat()
        {
            _ChkTH = false;
            _ChkCT = false;
            addEventClick(_ChkCT, _ChkTH);
        }
        private void btnCapPhat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtImportCapPhat();
        }




        private void BtImportCapThem()
        {
            _ChkCT = true;
            _ChkTH = false;
            string url = string.Format("{0}/?donhang={1}", URL + "DinhMuc/GetKiemTraDinhMuc", _maDonHang);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<DinhMucEntity> listDMNPL = JsonConvert.DeserializeObject<List<DinhMucEntity>>(json);
            if (listDMNPL.Count() == 0)
            {
                MessageBox.Show("Chưa có cấp phát", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                addEventClick(_ChkCT, _ChkTH);
            }
        }
        private void btnCapThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtImportCapThem();
        }

        private void frmErpImport_DMNL_Closed(object sender, string result)
        {
            Console.WriteLine("frmNhapTruTonKho_Closed");
            if (string.Compare(result, "True") == 0)
            {
                //LoadDSDinhMucDonHang();
                LoadDSDMThongTin();
            }
        }

        private void addEventClick(bool _ChkCT, bool _ChkTH)
        {
            if (_maDonHang.ToString() == "")
                return;
            if (!_ChkCT || !_ChkTH)
            {
                string url = string.Format("{0}/?donhang={1}", URL + "DinhMuc/GetKiemTraDinhMuc", _maDonHang);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                List<DinhMucEntity> listDMNPL = JsonConvert.DeserializeObject<List<DinhMucEntity>>(json);

                if (listDMNPL.Count() == 0)
                {
                    frmErpImport_DMNL frm = new frmErpImport_DMNL(_maDonHang, "", _ChkCT, _ChkTH,"");
                    frm.DataClosed += frmErpImport_DMNL_Closed;
                    frm.ShowDialog();
                    _status = ResourceURL.EventStatus.Add;
                }
                else
                {
                    if (_ChkCT || _ChkTH)
                    {
                        frmErpImport_DMNL frm = new frmErpImport_DMNL(_maDonHang, "", _ChkCT, _ChkTH,"");
                        frm.DataClosed += frmErpImport_DMNL_Closed;
                        frm.ShowDialog();
                        _status = ResourceURL.EventStatus.Add;
                    }
                    else
                    {
                        MessageBox.Show("Đơn hàng này đã có cấp phát, hãy xóa đi trước khi muốn thực hiện cấp phát! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Đơn hàng đã được sản xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        #region 
        //// Cấp phát_1
        //private void CapPhat_ItemClick(object sender, EventArgs e)
        //{
        //    int option = 1;
        //    frmNhapDMNL frmNhapDMNL = new frmNhapDMNL(_maDonHang, _maHang, option);
        //    frmNhapDMNL.StartPosition = FormStartPosition.CenterScreen;
        //    frmNhapDMNL.ShowInTaskbar = false;
        //    frmNhapDMNL.Size = new Size(1550, 800);
        //    frmNhapDMNL.ShowDialog();
        //}

        //// Cấp thêm_2
        //private void CapThem_ItemClick(object sender, EventArgs e)
        //{
        //    int option = 2;
        //    frmNhapDMNL frmNhapDMNL = new frmNhapDMNL(_maDonHang, _maHang, option);
        //    frmNhapDMNL.StartPosition = FormStartPosition.CenterScreen;
        //    frmNhapDMNL.ShowInTaskbar = false;
        //    frmNhapDMNL.Size = new Size(1550, 800);
        //    frmNhapDMNL.ShowDialog();
        //}

        //// Thu hồi_3
        //private void ThuHoi_ItemClick(object sender, EventArgs e)
        //{
        //    // frmDonHangChiTiet frm = new frmDonHangChiTiet(_maDonHang, _mahang, _soluong, _ngaytao, _chungloai);
        //    int option = 3;
        //    frmNhapDMNL frmNhapDMNL = new frmNhapDMNL(_maDonHang, _maHang, option);
        //    frmNhapDMNL.StartPosition = FormStartPosition.CenterScreen;
        //    frmNhapDMNL.ShowInTaskbar = false;
        //    frmNhapDMNL.Size = new Size(1550, 800);
        //    frmNhapDMNL.ShowDialog();
        //}
        #endregion

        private void BtNapLai()
        {
            LoadDSDMThongTin();
        }
        private void btCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //LoadDSDinhMucDonHang();
            barEditItemcbLoc.EditValue = repositoryItemComboBox1.Items[0].ToString();
            BtNapLai();
        }

        private void BandedView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
        }
        private void gridViewDinhMucNguyenPhuLieu_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void barCheckImport_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UpdateButton(barCheckImport.Checked);
        }

        // Dùng để cập nhật ẩn hiện nút Nhập và Import
        private void UpdateButton(bool _isImport)
        {
            // Nhập
            // CapThem
            barButtonCapThem.Visibility = _isImport ? DevExpress.XtraBars.BarItemVisibility.Never
                : DevExpress.XtraBars.BarItemVisibility.Always;
            actionControlNhapCapThem.Enabled = !_isImport;
            // CapPhat
            barButtonCapPhat.Visibility = _isImport ? DevExpress.XtraBars.BarItemVisibility.Never
                : DevExpress.XtraBars.BarItemVisibility.Always;
            actionControlNhapCapPhat.Enabled = !_isImport;
            // ThuHoi
            barButtonThuHoi.Visibility = _isImport ? DevExpress.XtraBars.BarItemVisibility.Never
                : DevExpress.XtraBars.BarItemVisibility.Always;
            actionControlNhapThuHoi.Enabled = !_isImport;

            // Import
            // CapThem
            btnCapThem.Visibility = _isImport ? DevExpress.XtraBars.BarItemVisibility.Always
                : DevExpress.XtraBars.BarItemVisibility.Never;
            actionControlImportCapThem.Enabled = _isImport;
            // CapPhat
            btnCapPhat.Visibility = _isImport ? DevExpress.XtraBars.BarItemVisibility.Always
                : DevExpress.XtraBars.BarItemVisibility.Never;
            actionControlImportCapPhat.Enabled = _isImport;
            // ThuHoi
            btnThuHoi.Visibility = _isImport ? DevExpress.XtraBars.BarItemVisibility.Always
                : DevExpress.XtraBars.BarItemVisibility.Never;
            actionControlImportThuHoi.Enabled = _isImport;
        }

        private void gridViewDinhMucNguyenPhuLieu_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void BtImportThuHoi()
        {
            _ChkCT = false;
            _ChkTH = true;
            string url = string.Format("{0}/?donhang={1}", URL + "DinhMuc/GetKiemTraDinhMuc", _maDonHang);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<DinhMucEntity> listDMNPL = JsonConvert.DeserializeObject<List<DinhMucEntity>>(json);
            if (listDMNPL.Count() == 0)
            {
                MessageBox.Show("Chưa có cấp phát", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                addEventClick(_ChkCT, _ChkTH);
            }
        }
        private void btnThuHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtImportThuHoi();
        }

        private void GridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                if (e.RowHandle < 0)
                {
                    e.DisplayText = "";
                }
                else
                {
                    e.DisplayText = Convert.ToString(e.RowHandle + 1);
                }
            }
        }

        private void gridViewDMThongTin_RowCountChanged(object sender, EventArgs e)
        {

            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void gridViewDinhMucNguyenPhuLieu_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridViewDMThongTin_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridViewDinhMucNguyenPhuLieu_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }


        private void repositoryItemSpinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            SpinEdit spinEdit = sender as SpinEdit;
            if (spinEdit != null && spinEdit.EditValue != null && Convert.ToInt32(spinEdit.EditValue) >= 0)
            {
                pageSize = Convert.ToInt32(spinEdit.EditValue);
                LoadDSDMThongTin();
            }
        }

        private void barButtonItemPrev_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (pageIndex > 1)
            {
                pageIndex = pageIndex - 1;
                barStaticItemPageIndex.Caption = pageIndex.ToString();
            }
            //LoadDSDinhMucDonHang();
            LoadDSDMThongTin();
        }

        private void barButtonItemNext_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pageIndex = pageIndex + 1;
            barStaticItemPageIndex.Caption = pageIndex.ToString();
            //LoadDSDinhMucDonHang();
            LoadDSDMThongTin();
        }


        private void BtNhapCapPhat()
        {
            _option = 1;
            _NChkTH = false;
            _NChkCT = false;
            addNEventClick(_ChkCT, _NChkTH, _maDonHang, _maHang, _option);
        }

        private void barButtonCapPhat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtNhapCapPhat();
        }
        private void BtNhapCapThem()
        {
            _option = 2;
            _NChkCT = true;
            _NChkTH = false;
            addNEventClick(_ChkCT, _NChkTH, _maDonHang, _maHang, _option);
        }

        private void barButtonCapThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtNhapCapThem();
        }


        private void BtNhapThuHoi()
        {
            _option = 3;
            _NChkCT = false;
            _NChkTH = true;
            addNEventClick(_ChkCT, _NChkTH, _maDonHang, _maHang, _option);
        }
        private void barButtonThuHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtNhapThuHoi();
        }

        private void addNEventClick(bool _ChkCT, bool _ChkTH, string madh, string mahang, int option)
        {
            if (!_NChkCT || !_NChkTH)
            {
                string url = string.Format("{0}/?donhang={1}", URL + "DinhMuc/GetKiemTraDinhMuc", _maDonHang);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                List<DinhMucEntity> listDMNPL = JsonConvert.DeserializeObject<List<DinhMucEntity>>(json);

                if (listDMNPL.Count() == 0)
                {
                    frmNhapDMNL frmNhapDMNL = new frmNhapDMNL(_maDonHang, _maHang, _option);
                    frmNhapDMNL.StartPosition = FormStartPosition.CenterScreen;
                    frmNhapDMNL.ShowInTaskbar = false;
                    //frmNhapDMNL.Size = new Size(1550, 800);
                    frmNhapDMNL.ShowDialog();
                }
                else
                {
                    if (_NChkCT || _NChkTH)
                    {
                        frmNhapDMNL frmNhapDMNL = new frmNhapDMNL(_maDonHang, _maHang, _option);
                        frmNhapDMNL.StartPosition = FormStartPosition.CenterScreen;
                        frmNhapDMNL.ShowInTaskbar = false;
                        //frmNhapDMNL.Size = new Size(1550, 800);
                        frmNhapDMNL.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Đơn hàng này đã có cấp phát, hãy xóa đi trước khi muốn thực hiện cấp phát! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                }
            }
            else
            {
                MessageBox.Show("Đơn hàng đã được sản xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void Xoa()
        {
            try
            {
                DialogResult result = MessageBox.Show("Bạn có muốn xóa định mức đơn hàng " + _maDonHang + " này không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string url = string.Format("{0}/?madonhang={1}", URL + "DonHangTong/DeleteNPL", _maDonHang);
                    string json = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                    LoadDSDMThongTin();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void barEditItemcbLoc_EditValueChanged(object sender, EventArgs e)
        {
            LoadDSDMThongTin();
        }

        
        private void frmDinhMucNguyenPhuLieu_Load(object sender, EventArgs e)
        {

        }

       

        private void xtraTabPage1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void xtraTabPage2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gridViewDMThongTin_Click(object sender, EventArgs e)
        {
            
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            if (view != null)
            {

                int rowHandle = view.FocusedRowHandle;


                if (rowHandle >= 0)
                {

                    string MaDH = view.GetRowCellValue(rowHandle, "MaDH").ToString();
                   
                    loadDSBomNL();
                    LoadBOMPL(false);
                }
            }
            
        }
        private void loadDSBomNL()
        {
            MaDH = gridViewDMThongTin.GetFocusedRowCellValue(colMaDHDMThongTin)?.ToString();
            string url = string.Format("{0}?MaDH={1}", URL + "BomNPL/GetBomNLByMaDH", MaDH);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json=="[]")
            {
                lstBomNL = new List<BomNguyenPhuLieuEntity>();
                gridNLConTrol.DataSource = lstBomNL;
                btnSuaNL.Enabled = false;
                btnXoaNL.Enabled = false;
                btnLuuNL.Enabled = false;
                return; 
            }    
            if (!string.IsNullOrEmpty(json))
            {
                /* DataTable tbBomNL = JsonConvert.DeserializeObject<DataTable>(json);*/
                lstBomNL = JsonConvert.DeserializeObject<List<BomNguyenPhuLieuEntity>>(json);
                btnSuaNL.Enabled = true;
                btnXoaNL.Enabled = true;
                btnLuuNL.Enabled = false;
            }

            gridNLConTrol.DataSource = lstBomNL;
            //LoadBOMDMNL(false, lstBomNL[0].MaVatTu);
            //LoadBOMMAUNL(false);
        }
        private void LoadBOMDMNL(bool IsFromSave, string MaVatTu)
        {
            try
            {
                //MaVatTu = gridPLView.GetFocusedRowCellValue(colMaVatTuPL)?.ToString();
                string url = URL + $"BOMNPL/GetBomNLDMByMaDH?MaVatTu={MaVatTu}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    DMNL = JsonConvert.DeserializeObject<DataTable>(json);
                    lstBOMVTDMNL = JsonConvert.DeserializeObject<List<BomNguyenPhuLieuEntity>>(json);
                }
                gridControlNLBOMDM.DataSource = lstBOMVTDMNL.Count > 0 ? lstBOMVTDMNL : null;
                if (IsFromSave && FocusedIndex >= 0)
                {
                    //gridBOMDMPLView.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gridControlNLBOMDM;
                }
                gridControlNLBOMDM.RefreshDataSource();

            }
            catch
            {
                XtraMessageBox.Show("Đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }


        private void btnXoaNL_Click(object sender, EventArgs e)
        {

            XoaBomNL();
        }
        private void XoaBomNL()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    if (gridNLView.FocusedRowHandle >= 0)
                    {
                        string MaBom  = gridNLView.GetFocusedRowCellValue(colMaBomNL)?.ToString();
                        string url = URL + $"BOMNPL/DeleteBomNPL?MaBom={MaBom}";
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true")
                        {
                            loadDSBomNL();
                            LoadBOMMAUNL(false);
                        }
                        else XtraMessageBox.Show(result);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Không có dữ liệu vui lòng nhập.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNapLaiNL_Click(object sender, EventArgs e)
        {
            NapLaiBomNL();
        }
        private void NapLaiBomNL()
        {
            string MaDH = gridViewDMThongTin.GetFocusedRowCellValue("MaDH").ToString();

            loadDSBomNL();
            LoadBOMMAUNL(false);
        }

      
        private void btnThemNL_Click(object sender, EventArgs e)
        {
            ThemDongNL();
            btnLuuNL.Enabled = true;
        }
        private void ThemDongNL()
        {
            BomNguyenPhuLieuEntity obj = new BomNguyenPhuLieuEntity();
            obj.MaDH = _maDHPL;
            //_bindingHangHoaEntity.Add(obj);
            lstBomNL.Add(obj);
            _rowAdd = gridNLView.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            gridNLView.FocusedRowHandle = _rowAdd;
            gridNLConTrol.DataSource = lstBomNL;
            gridNLConTrol.RefreshDataSource();
            //ThemDongDMNL();

        }
        private void ThemDongDMNL()
        {
            BomNguyenPhuLieuEntity obj = new BomNguyenPhuLieuEntity();
            obj.MaDH = _maDHPL;
            lstBOMVTDMNL.Add(obj);
            _rowAdd = gridBOMDMPLView.RowCount - 1;
            gridNLBOMDMView.FocusedRowHandle = _rowAdd;
            gridControlNLBOMDM.DataSource = lstBOMVTDMNL;
            gridControlNLBOMDM.RefreshDataSource();
        }
        //private void ThemDongMAUNL()
        //{
        //    BomNguyenPhuLieuEntity obj = new BomNguyenPhuLieuEntity();
        //    obj.MaDH = _maDHPL;
        //    lstBOMVTMAUNL.Add(obj);
        //    _rowAdd = gridBOMNLMAU.RowCount - 1;
        //    //gridMauViewPL.FocusedRowHandle = _rowAdd3;
        //    gridControlNLBOMMAU.DataSource = lstBOMVTMAUNL;
        //    gridControlNLBOMMAU.RefreshDataSource();
        //}

        private void searchLookUpEditBomNL_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void searchLookUpEditBomNL_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            //if (searchLookUpEditBomNL.EditValue != null)
            //{
            //    var row = (DataRowView)searchLookUpEditBomNL.GetSelectedDataRow();
            //    if (row != null)
            //    {
            //        e.DisplayText = $"{row["MaVatTu"]} - {row["TenVatTu"]} - {row["CodeMau"]}";
            //    }
            //}
        }

        private void btnHuyPopupNL_Click(object sender, EventArgs e)
        {
            //popupSearchNL.Hide();
        }

        private void btnXacNhanPopupNL_Click(object sender, EventArgs e)
        {
            //var selectedRow = searchLookUpEditBomNL.Properties.View.GetFocusedDataRow();
            //if (selectedRow != null)
            //{
            //    // Lấy dữ liệu từ DataRow
            //    string _maVatTu = selectedRow["MaVatTu"].ToString();
            //    DataRow[] filteredRows = _lstNguyenLieu.Select($"MaVatTu = '{_maVatTu}'");

            //    if (filteredRows.Length > 0)
            //    {
            //        DataRow filteredRow = filteredRows[0];
            //        int focusedRowHandle = gridNLView.FocusedRowHandle;

            //        if (focusedRowHandle >= 0) // Kiểm tra xem có dòng nào đang được focus
            //        {
            //            // Gán dữ liệu từ DataRow vào các cột của dòng đang focus
            //            gridNLView.SetRowCellValue(focusedRowHandle, "MaVatTu", filteredRow["MaVatTu"]);
            //            gridNLView.SetRowCellValue(focusedRowHandle, "TenVatTu", filteredRow["TenVatTu"]);
            //            gridNLView.SetRowCellValue(focusedRowHandle, "Mau", filteredRow["TenCodeMau"]);
            //            gridNLView.SetRowCellValue(focusedRowHandle, "KhoVai", filteredRow["KhoVai"]);
            //            gridNLView.SetRowCellValue(focusedRowHandle, "DonVi", filteredRow["DonViTinh"]);

            //            // Thông báo hoàn tất
            //            Console.WriteLine("Dữ liệu đã được gán vào dòng focus của GridView.");
            //        }

            //    }

            //    // Thêm dữ liệu vào GridView
               
            //}
           
            //popupSearchNL.Hide();
        }

        private void btnSuaNL_Click(object sender, EventArgs e)
        {

            int focusedRowHandle = gridNLView.FocusedRowHandle;
            _maNLold = gridNLView.GetRowCellValue(focusedRowHandle, "MaVatTu")?.ToString();
            colMaVatTuNL.OptionsColumn.ReadOnly = false; 
            colGhiChu.OptionsColumn.ReadOnly = false;
            btnLuuNL.Enabled = true;
            _newRowHandle = focusedRowHandle;
        }

        private void btnLuuNL_Click(object sender, EventArgs e)
        {
            LuuBomNL();
           
        }
        private async void LuuBomNL()
        {

            try
            {
                this.ActiveControl = this.button2;
                List<BomNguyenPhuLieuEntity> _lstUpdate = new List<BomNguyenPhuLieuEntity>();
                //kiểm tra null
                int focusedRowHandle = gridNLView.FocusedRowHandle;
                int focusedRowhandle2 = gridNLBOMDMView.FocusedRowHandle;
                int focusedRowhandle3 = bandedGridViewMau.FocusedRowHandle;
                DataTable po = gridControlNLBOMMAU.DataSource as DataTable;
                //var IsChon = po.AsEnumerable().Where(x => x["Chon"]?.ToString() == "True").ToList();

                foreach (DataRow row in po.Rows)
                {
                    foreach (DataColumn col in po.Columns) 
                    {
                        if (!col.ColumnName.Contains("@")) continue;
                        string SizeID = col.ColumnName.ToString().Split(new string[] { "@" }, StringSplitOptions.None)[0];
                        bool Chon = bool.TryParse(row[col.ColumnName]?.ToString(),out bool chon)&&chon;

                            _lstUpdate.Add(new BomNguyenPhuLieuEntity
                            {
                                ID = gridNLView.GetRowCellValue(focusedRowHandle, "ID") != null ? Int32.Parse(gridNLView.GetRowCellValue(focusedRowHandle, "ID").ToString()) : 0,
                                //MaBom = gridNLView.GetRowCellValue(focusedRowHandle, "MaBom")?.ToString() ?? string.Empty,
                                MaBom = "BOM" + MaDH + gridNLView.GetRowCellValue(focusedRowHandle, "MaVatTu")?.ToString() ?? string.Empty,
                                MaDH = MaDH,
                                MaVatTu = gridNLView.GetRowCellValue(focusedRowHandle, "MaVatTu")?.ToString() ?? string.Empty,
                                TenVatTu = gridNLView.GetRowCellValue(focusedRowHandle, "TenVatTu")?.ToString() ?? string.Empty,
                                Mau = gridNLView.GetRowCellValue(focusedRowHandle, "Mau")?.ToString() ?? string.Empty,
                                KhoVai = gridNLView.GetRowCellValue(focusedRowHandle, "KhoVai")?.ToString() ?? string.Empty,
                                DonViTinh = gridNLView.GetRowCellValue(focusedRowHandle, "DonViTinh")?.ToString() ?? string.Empty,
                                DinhMucMuaHang = float.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "DinhMucMuaHang")?.ToString(), out float dinhMucmuahang) ? dinhMucmuahang : 0,
                                DinhMucThucTe = float.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "DinhMucThucTe")?.ToString(), out float dinhmucThucte) ? dinhmucThucte : 0,
                                DinhMucKH = float.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "DinhMucKH")?.ToString(), out float dinhmucKh) ? dinhmucKh : 0,
                                GhiChu = gridNLView.GetRowCellValue(focusedRowHandle, "GhiChu")?.ToString() ?? string.Empty,
                                NgayLap = DateTime.Now.ToString("yyyy-MM-dd"),
                                MaSP = gridNLView.GetRowCellValue(focusedRowHandle, "MaSP")?.ToString() ?? string.Empty,
                                MaTiLe = gridNLView.GetRowCellValue(focusedRowHandle, "MaTiLe")?.ToString() ?? string.Empty,
                                SLChungTu = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SLChungTu")?.ToString(), out int slChungTu) ? slChungTu : 0,
                                PhanLoaiVT = "Nguyên Liệu",
                                NhaCungCap = gridNLView.GetRowCellValue(focusedRowHandle, "NhaCungCap")?.ToString() ?? string.Empty,
                                POID = row["POID"].ToString(),
                                MaMau = row["MaMau"].ToString(),
                                DauSizeID = row["DauSizeID"].ToString(),
                                SizeID = SizeID,
                                SLSanPham = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SLSanPham")?.ToString(), out int slsp) ? slsp : 0,
                                SLDinhMuc = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SLDinhMuc")?.ToString(), out int sldm) ? sldm : 0,
                                HaoHut = float.TryParse(gridBOMDMPLView.GetRowCellValue(focusedRowHandle, "HaoHut")?.ToString(), out float haohut) ? haohut : 0,
                                SL = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SL")?.ToString(), out int sl) ? sl : 0,
                                SoChungTu = gridNLView.GetRowCellValue(focusedRowHandle, "SoChungTu")?.ToString() ?? string.Empty,
                                SanPham = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SanPham")?.ToString(), out int sp) ? sp : 0,
                                ChieuDaiCuon = float.TryParse(gridBOMDMPLView.GetRowCellValue(focusedRowHandle, "ChieuDaiCuon")?.ToString(), out float chieuDaicuon) ? chieuDaicuon : 0,
                                SoLuongCan = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SoLuongCan")?.ToString(), out int slc) ? slc : 0,
                                SLCanBangMau = float.TryParse(gridBOMDMPLView.GetRowCellValue(focusedRowHandle, "SLCanBangMau")?.ToString(), out float slCanBanMau) ? slCanBanMau : 0,
                                DonGia = float.TryParse(gridBOMDMPLView.GetRowCellValue(focusedRowHandle, "DonGia")?.ToString(), out float donGia) ? donGia : 0,
                                ThanhTien = float.TryParse(gridBOMDMPLView.GetRowCellValue(focusedRowHandle, "ThanhTien")?.ToString(), out float thanhTien) ? thanhTien : 0,
                                SLThucTe = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SLThucTe")?.ToString(), out int sltt) ? sltt : 0,
                                SLDat = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SLDat")?.ToString(), out int sldat) ? sldat : 0,
                                Chon = Chon ? 1:0,
                            });
                    }

                }
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                {
                    return;
                }
                string url = string.Format("{0}", URL + "BomNPL/PostBomVT");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate.Distinct()); }).Result;

                if (msResult.ToLower() == "true")
                {
                    loadDSBomNL();
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                lstRowUpdate.Clear();
            }
            catch(Exception e)
            {
                
            }
            
        }

        private void btnNapLaiPL_Click(object sender, EventArgs e)
        {
            CreateSearchLookUp();
            LoadBOMPL(false);
            LoadBOMMAUPL(false);
        }

        private void btnXoaPL_Click(object sender, EventArgs e)
        {
            XoaVatTu();
        }
        private async void XoaVatTu()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    if (gridPLView.FocusedRowHandle >= 0)
                    {
                        string MaBom = gridPLView.GetFocusedRowCellValue(colMaBomPL)?.ToString();
                        string url = URL + $"BOMNPL/DeleteBomNPL?MaBom={MaBom}";
                        string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                        if (result.ToLower() == "true") {
                            LoadBOMPL(true);
                            LoadBOMMAUPL(true);
                        }
                        else XtraMessageBox.Show(result);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Không có dữ liệu vui lòng nhập.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThemPL_Click(object sender, EventArgs e)
        {
            ThemDong();

        }
        private void ThemDong()
        {
            BomNguyenPhuLieuEntity obj = new BomNguyenPhuLieuEntity();
            obj.MaDH = _maDHPL;
            //_bindingHangHoaEntity.Add(obj);
            lstBOMVT.Add(obj);
            _rowAdd = gridPLView.RowCount - 1;
            _status = ResourceURL.EventStatus.Add;
            gridPLView.FocusedRowHandle = _rowAdd;
            gridPLControl.DataSource = lstBOMVT;
            gridPLControl.RefreshDataSource();
            //ThemDongDMPL();
            //ThemDongMAUPL();


        }
        private void ThemDongDMPL() 
        {
            BomNguyenPhuLieuEntity obj = new BomNguyenPhuLieuEntity();
            obj.MaDH = _maDHPL;
            lstBOMVTDM.Add(obj);
            _rowAdd = gridBOMDMPLView.RowCount - 1;
            gridBOMDMPLView.FocusedRowHandle = _rowAdd;
            gridControlBOMPLDM.DataSource = lstBOMVTDM;
            gridControlBOMPLDM.RefreshDataSource();
        }
        //private void ThemDongMAUPL()
        //{
        //    BomNguyenPhuLieuEntity obj = new BomNguyenPhuLieuEntity();
        //    obj.MaDH = _maDHPL;
        //    lstBOMVTMAU.Add(obj);
        //    _rowAdd = gridMauViewPL.RowCount - 1;
        //    //gridMauViewPL.FocusedRowHandle = _rowAdd3;
        //    gridMauPLControl.DataSource = lstBOMVTMAU;
        //    gridMauPLControl.RefreshDataSource();
        //}


        private void btnLuuPL_Click(object sender, EventArgs e)
        {
            LuuDong();
        }
        private void LuuDong()
        {
            try
            {
                this.ActiveControl = this.button2;
                List<BomNguyenPhuLieuEntity> _lstUpdate = new List<BomNguyenPhuLieuEntity>();
                //kiểm tra null
                int focusedRowHandle = gridPLView.FocusedRowHandle;
                int focusedRowhandle2 = gridBOMDMPLView.FocusedRowHandle;
                int focusedRowhandle3 = bandedGridView1.FocusedRowHandle;
                DataTable po = gridMauPLControl.DataSource as DataTable;
                //var IsChon = po.AsEnumerable().Where(x => x["Chon"]?.ToString() == "True").ToList();

                foreach (DataRow row in po.Rows)
                {
                    foreach (DataColumn col in po.Columns)
                    {
                        if (!col.ColumnName.Contains("@")) continue;
                        string SizeID = col.ColumnName.ToString().Split(new string[] { "@" }, StringSplitOptions.None)[0];
                        bool Chon = bool.TryParse(row[col.ColumnName]?.ToString(), out bool chon) && chon;
                        _lstUpdate.Add(new BomNguyenPhuLieuEntity
                        {
                            ID = gridPLView.GetRowCellValue(focusedRowHandle, "ID") == null ? 0 :
                                Int32.Parse(gridPLView.GetRowCellValue(focusedRowHandle, "ID").ToString()),
                            //MaBom = gridPLView.GetRowCellValue(focusedRowHandle, "MaBom")?.ToString() ?? string.Empty,
                            MaBom = "BOM" + MaDH + gridPLView.GetRowCellValue(focusedRowHandle, "MaVatTu")?.ToString() ?? string.Empty,
                            MaDH = MaDH,
                            MaVatTu = gridPLView.GetRowCellValue(focusedRowHandle, "MaVatTu")?.ToString() ?? string.Empty,
                            TenVatTu = gridPLView.GetRowCellValue(focusedRowHandle, "TenVatTu")?.ToString() ?? string.Empty,
                            Mau = gridPLView.GetRowCellValue(focusedRowHandle, "Mau")?.ToString() ?? string.Empty,
                            KhoVai = gridPLView.GetRowCellValue(focusedRowHandle, "KhoVai")?.ToString() ?? string.Empty,
                            DonViTinh = gridPLView.GetRowCellValue(focusedRowHandle, "DonViTinh")?.ToString() ?? string.Empty,
                            DinhMucMuaHang = float.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "DinhMucMuaHang")?.ToString(), out float dinhMucmuahang) ? dinhMucmuahang : 0,
                            DinhMucThucTe = float.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "DinhMucThucTe")?.ToString(), out float dinhmucThucte) ? dinhmucThucte : 0,
                            DinhMucKH = float.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "DinhMucKH")?.ToString(), out float dinhmucKh) ? dinhmucKh : 0,
                            GhiChu = gridPLView.GetRowCellValue(focusedRowHandle, "GhiChu")?.ToString() ?? string.Empty,
                            NgayLap = DateTime.Now.ToString("yyyy-MM-dd"),
                            MaSP = gridPLView.GetRowCellValue(focusedRowHandle, "MaSP")?.ToString() ?? string.Empty,
                            MaTiLe = gridPLView.GetRowCellValue(focusedRowHandle, "MaTiLe")?.ToString() ?? string.Empty,
                            SLChungTu = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SLChungTu")?.ToString(), out int slChungTu) ? slChungTu : 0,
                            PhanLoaiVT = "Phụ Liệu",
                            NhaCungCap = gridPLView.GetRowCellValue(focusedRowHandle, "NhaCungCap")?.ToString() ?? string.Empty,
                            POID = row["POID"].ToString(),
                            MaMau = row["MaMau"].ToString(),
                            DauSizeID = row["DauSizeID"].ToString(),
                            SizeID = SizeID,
                            SLSanPham = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SLSanPham")?.ToString(), out int slsp) ? slsp : 0,
                            SLDinhMuc = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SLDinhMuc")?.ToString(), out int sldm) ? sldm : 0,
                            HaoHut = float.TryParse(gridBOMDMPLView.GetRowCellValue(focusedRowHandle, "HaoHut")?.ToString(), out float haohut) ? haohut : 0,
                            SL = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SL")?.ToString(), out int sl) ? sl : 0,
                            SoChungTu = gridPLView.GetRowCellValue(focusedRowHandle, "SoChungTu")?.ToString() ?? string.Empty,
                            SanPham = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SanPham")?.ToString(), out int sp) ? sp : 0,
                            ChieuDaiCuon = float.TryParse(gridBOMDMPLView.GetRowCellValue(focusedRowHandle, "ChieuDaiCuon")?.ToString(), out float chieuDaicuon) ? chieuDaicuon : 0,
                            SoLuongCan = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SoLuongCan")?.ToString(), out int slc) ? slc : 0,
                            SLCanBangMau = float.TryParse(gridBOMDMPLView.GetRowCellValue(focusedRowHandle, "SLCanBangMau")?.ToString(), out float slCanBanMau) ? slCanBanMau : 0,
                            DonGia = float.TryParse(gridBOMDMPLView.GetRowCellValue(focusedRowHandle, "DonGia")?.ToString(), out float donGia) ? donGia : 0,
                            ThanhTien = float.TryParse(gridBOMDMPLView.GetRowCellValue(focusedRowHandle, "ThanhTien")?.ToString(), out float thanhTien) ? thanhTien : 0,
                            SLThucTe = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SLThucTe")?.ToString(), out int sltt) ? sltt : 0,
                            SLDat = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SLDat")?.ToString(), out int sldat) ? sldat : 0,
                            Chon = Chon ? 1 : 0,
                        });
                    }

                }

                
                string msResult = "";
                if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
                {
                    return;
                }
                string url = string.Format("{0}", URL + "BomNPL/PostBomVT");
                msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;

                if (msResult.ToLower() == "true")
                {
                    LoadBOMPL(true);
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else XtraMessageBox.Show(msResult);
                lstRowUpdate.Clear();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!"+ex, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
       
        private void gridPLView_ColumnChanged(object sender, EventArgs e)
        {
           
        }

        private void gridPLView_Click(object sender, EventArgs e)
        {

        }

        private void btnSuaPL_Click(object sender, EventArgs e)
        {
            int focusedRowHandle = gridPLView.FocusedRowHandle;
            _maPLOld = gridPLView.GetRowCellValue(focusedRowHandle, "MaVatTu")?.ToString();
            LuuDong();
        }

        private void gridPLView_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            int focusedRowHandle = gridPLView.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                string MaVatTu = gridPLView.GetFocusedRowCellValue(colMaVatTuPL)?.ToString();
                //LoadBOMDMPL(false, MaVatTu);
                LoadBOMMAUPL(false);
            }
        }

        private void gridNLView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {

            int focusedRowHandle = gridPLView.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                string MaVatTu = gridPLView.GetFocusedRowCellValue(colMaVatTuPL)?.ToString();
                //LoadBOMDMPL(false, MaVatTu);
                LoadBOMMAUPL(false);
            }
        }
        private void gridNLView2_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {

            int focusedRowHandle = gridNLView.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                string MaVatTu = gridNLView.GetFocusedRowCellValue(colMaVatTuNL)?.ToString();
                LoadBOMDMNL(false, MaVatTu);
                LoadBOMMAUNL(false);
            }
        }
        private void gridPLView_FocusedRowChanged(object sender, EventArgs e)
        {
            //var selectedRow = plEdit.View.GetFocusedDataRow();
            DevExpress.XtraEditors.Controls.ChangingEventArgs changedVaule = e as DevExpress.XtraEditors.Controls.ChangingEventArgs;
            var PL = tblPL.AsEnumerable().FirstOrDefault(x => x["MaVT"].ToString() == changedVaule.NewValue);
            // Lấy dữ liệu từ DataRow
            if (PL != null)
            {
                int focusedRowHandle = gridPLView.FocusedRowHandle;
                if (focusedRowHandle >= 0)
                {
                    gridPLView.SetRowCellValue(focusedRowHandle, "MaVatTu", PL["MaVT"]);
                    gridPLView.SetRowCellValue(focusedRowHandle, "TenVatTu", PL["TenVT"]);
                    gridPLView.SetRowCellValue(focusedRowHandle, "Mau", PL["Mau"]);
                    gridPLView.SetRowCellValue(focusedRowHandle, "KhoVai", PL["SizeKho"]);
                    gridPLView.SetRowCellValue(focusedRowHandle, "DonViTinh", PL["DVTinh"]);
                }
            }
        }
        private void gridPLView2_FocusedRowChanged(object sender, EventArgs e)
        {
            //var selectedRow = plEdit.View.GetFocusedDataRow();
            DevExpress.XtraEditors.Controls.ChangingEventArgs changedVaule = e as DevExpress.XtraEditors.Controls.ChangingEventArgs;
            var NL = tblNL.AsEnumerable().FirstOrDefault(x => x["MaVT"].ToString() == changedVaule.NewValue);
            // Lấy dữ liệu từ DataRow
            if (NL != null)
            {
                int focusedRowHandle = gridNLView.FocusedRowHandle;
                if (focusedRowHandle >= 0)
                {
                    gridNLView.SetRowCellValue(focusedRowHandle, "MaVatTu", NL["MaVT"]);
                    gridNLView.SetRowCellValue(focusedRowHandle, "TenVatTu", NL["TenVT"]);
                    gridNLView.SetRowCellValue(focusedRowHandle, "Mau", NL["Mau"]);
                    gridNLView.SetRowCellValue(focusedRowHandle, "KhoVai", NL["SizeKho"]);
                    gridNLView.SetRowCellValue(focusedRowHandle, "DonViTinh", NL["DVTinh"]);

                }
            }

        }

        private void gridBOMDMView_FocusedRowChanged(object sender, EventArgs e)
        {


        }

        private void repositoryItemCheckEdit2_CheckedChanged(object sender, EventArgs e)
        {
            //List<BomNguyenPhuLieuEntity> _lstUpdate = new List<BomNguyenPhuLieuEntity>();
            //this.ActiveControl = this.button2;
            //int focusedRowHandle = gridNLView.FocusedRowHandle;
            //int focusedRowhandle2 = gridNLBOMDMView.FocusedRowHandle;
            //int focusedRowhandle3 = bandedGridViewMau.FocusedRowHandle;
            //DataRow rowPO = bandedGridViewMau.GetFocusedDataRow();
            //var UnChon = Int32.TryParse(rowPO["Chon"]?.ToString(), out int chon) ? chon : 0;
            //if (UnChon==0) 
            //{
            //    BomNguyenPhuLieuEntity _bom = new BomNguyenPhuLieuEntity
            //    {
            //        ID = gridNLView.GetRowCellValue(focusedRowHandle, "ID") != null ? Int32.Parse(gridNLView.GetRowCellValue(focusedRowHandle, "ID").ToString()) : 0,
            //        MaBom = gridNLView.GetRowCellValue(focusedRowHandle, "MaBom")?.ToString() ?? string.Empty,
            //        MaDH = gridNLView.GetRowCellValue(focusedRowHandle, "MaDH")?.ToString() ?? string.Empty,
            //        MaVatTu = gridNLView.GetRowCellValue(focusedRowHandle, "MaVatTu")?.ToString() ?? string.Empty,
            //        TenVatTu = gridNLView.GetRowCellValue(focusedRowHandle, "TenVatTu")?.ToString() ?? string.Empty,
            //        Mau = gridNLView.GetRowCellValue(focusedRowHandle, "Mau")?.ToString() ?? string.Empty,
            //        KhoVai = gridNLView.GetRowCellValue(focusedRowHandle, "KhoVai")?.ToString() ?? string.Empty,
            //        DonViTinh = gridNLView.GetRowCellValue(focusedRowHandle, "DonViTinh")?.ToString() ?? string.Empty,
            //        DinhMucMuaHang = float.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "DinhMucMuaHang")?.ToString(), out float dinhMucmuahang) ? dinhMucmuahang : 0,
            //        DinhMucThucTe = float.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "DinhMucThucTe")?.ToString(), out float dinhmucThucte) ? dinhmucThucte : 0,
            //        DinhMucKH = float.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "DinhMucKH")?.ToString(), out float dinhmucKh) ? dinhmucKh : 0,
            //        GhiChu = gridNLView.GetRowCellValue(focusedRowHandle, "GhiChu")?.ToString() ?? string.Empty,
            //        NgayLap = gridNLView.GetRowCellValue(focusedRowHandle, "NgayLap")?.ToString() ?? string.Empty,
            //        MaSP = gridNLView.GetRowCellValue(focusedRowHandle, "MaSP")?.ToString() ?? string.Empty,
            //        MaTiLe = gridNLView.GetRowCellValue(focusedRowHandle, "MaTiLe")?.ToString() ?? string.Empty,
            //        SLChungTu = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SLChungTu")?.ToString(), out int slChungTu) ? slChungTu : 0,
            //        PhanLoaiVT = "Nguyên Liệu",
            //        NhaCungCap = gridNLView.GetRowCellValue(focusedRowHandle, "NhaCungCap")?.ToString() ?? string.Empty,
            //        POID = rowPO["POID"].ToString(),
            //        MaMau = rowPO["MaMau"].ToString(),
            //        DauSizeID = rowPO["DauSizeID"].ToString(),
            //        SizeID = rowPO["SizeID"].ToString(),
            //        SLSanPham = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SLSanPham")?.ToString(), out int slsp) ? slsp : 0,
            //        SLDinhMuc = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SLDinhMuc")?.ToString(), out int sldm) ? sldm : 0,
            //        HaoHut = float.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "HaoHut")?.ToString(), out float haohut) ? haohut : 0,
            //        SL = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SL")?.ToString(), out int sl) ? sl : 0,
            //        SoChungTu = gridNLView.GetRowCellValue(focusedRowHandle, "SoChungTu")?.ToString() ?? string.Empty,
            //        SanPham = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SanPham")?.ToString(), out int sp) ? sp : 0,
            //        ChieuDaiCuon = float.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "ChieuDaiCuon")?.ToString(), out float chieuDaicuon) ? chieuDaicuon : 0,
            //        SoLuongCan = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SoLuongCan")?.ToString(), out int slc) ? slc : 0,
            //        SLCanBangMau = float.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SLCanBangMau")?.ToString(), out float slCanBanMau) ? slCanBanMau : 0,
            //        DonGia = float.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "DonGia")?.ToString(), out float donGia) ? donGia : 0,
            //        ThanhTien = float.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "ThanhTien")?.ToString(), out float thanhTien) ? thanhTien : 0,
            //        SLThucTe = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SLThucTe")?.ToString(), out int sltt) ? sltt : 0,
            //        SLDat = Int32.TryParse(gridNLView.GetRowCellValue(focusedRowHandle, "SLDat")?.ToString(), out int sldat) ? sldat : 0,
            //        Chon = UnChon,
            //    };
            //    _lstUpdate.Add(_bom);
            //    string msResult = "";
            //    if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
            //    {
            //        return;
            //    }
            //    string url = string.Format("{0}", URL + "BomNPL/PostBomVT");
            //    msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;

            //    if (msResult.ToLower() == "true")
            //    {
            //        LoadBOMPL(true);
            //        clsWaitForm.ShowSuccessForm(this, 2000);
            //    }
            //    else XtraMessageBox.Show(msResult);
            //    lstRowUpdate.Clear();
            //}

        }

        private void repositoryItemCheckEdit3_CheckedChanged(object sender, EventArgs e)
        {
            //List<BomNguyenPhuLieuEntity> _lstUpdate = new List<BomNguyenPhuLieuEntity>();
            //this.ActiveControl = this.button2;
            //int focusedRowHandle = gridPLView.FocusedRowHandle;
            //int focusedRowhandle2 = gridBOMDMPLView.FocusedRowHandle;
            //int focusedRowhandle3 = bandedGridView1.FocusedRowHandle;
            //DataRow rowPO = bandedGridView1.GetFocusedDataRow();
            //var UnChon = Int32.TryParse(rowPO["Chon"]?.ToString(), out int chon) ? chon : 0;
            //if (UnChon==0)
            //{
            //    BomNguyenPhuLieuEntity _bom = new BomNguyenPhuLieuEntity
            //    {
            //        ID = gridPLView.GetRowCellValue(focusedRowHandle, "ID") == null ? 0 :
            //                    Int32.Parse(gridPLView.GetRowCellValue(focusedRowHandle, "ID").ToString()),
            //        //MaBom = gridPLView.GetRowCellValue(focusedRowHandle, "MaBom")?.ToString() ?? string.Empty,
            //        MaBom = gridPLView.GetRowCellValue(focusedRowHandle, "MaBom")?.ToString() ?? string.Empty,
            //        MaDH = gridPLView.GetRowCellValue(focusedRowHandle, "MaDH")?.ToString() ?? string.Empty,
            //        MaVatTu = gridPLView.GetRowCellValue(focusedRowHandle, "MaVatTu")?.ToString() ?? string.Empty,
            //        TenVatTu = gridPLView.GetRowCellValue(focusedRowHandle, "TenVatTu")?.ToString() ?? string.Empty,
            //        Mau = gridPLView.GetRowCellValue(focusedRowHandle, "Mau")?.ToString() ?? string.Empty,
            //        KhoVai = gridPLView.GetRowCellValue(focusedRowHandle, "KhoVai")?.ToString() ?? string.Empty,
            //        DonViTinh = gridPLView.GetRowCellValue(focusedRowHandle, "DonViTinh")?.ToString() ?? string.Empty,
            //        DinhMucMuaHang = float.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "DinhMucMuaHang")?.ToString(), out float dinhMucmuahang) ? dinhMucmuahang : 0,
            //        DinhMucThucTe = float.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "DinhMucThucTe")?.ToString(), out float dinhmucThucte) ? dinhmucThucte : 0,
            //        DinhMucKH = float.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "DinhMucKH")?.ToString(), out float dinhmucKh) ? dinhmucKh : 0,
            //        GhiChu = gridPLView.GetRowCellValue(focusedRowHandle, "GhiChu")?.ToString() ?? string.Empty,
            //        NgayLap = gridPLView.GetRowCellValue(focusedRowHandle, "NgayLap")?.ToString() ?? string.Empty,
            //        MaSP = gridPLView.GetRowCellValue(focusedRowHandle, "MaSP")?.ToString() ?? string.Empty,
            //        MaTiLe = gridPLView.GetRowCellValue(focusedRowHandle, "MaTiLe")?.ToString() ?? string.Empty,
            //        SLChungTu = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SLChungTu")?.ToString(), out int slChungTu) ? slChungTu : 0,
            //        PhanLoaiVT = "Phụ Liệu",
            //        NhaCungCap = gridPLView.GetRowCellValue(focusedRowHandle, "NhaCungCap")?.ToString() ?? string.Empty,
            //        POID = rowPO["POID"].ToString(),
            //        MaMau = rowPO["MaMau"].ToString(),
            //        DauSizeID = rowPO["DauSizeID"].ToString(),
            //        SizeID = rowPO["SizeID"].ToString(),
            //        SLSanPham = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SLSanPham")?.ToString(), out int slsp) ? slsp : 0,
            //        SLDinhMuc = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SLDinhMuc")?.ToString(), out int sldm) ? sldm : 0,
            //        HaoHut = float.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "HaoHut")?.ToString(), out float haohut) ? haohut : 0,
            //        SL = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SL")?.ToString(), out int sl) ? sl : 0,
            //        SoChungTu = gridPLView.GetRowCellValue(focusedRowHandle, "SoChungTu")?.ToString() ?? string.Empty,
            //        SanPham = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SanPham")?.ToString(), out int sp) ? sp : 0,
            //        ChieuDaiCuon = float.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "ChieuDaiCuon")?.ToString(), out float chieuDaicuon) ? chieuDaicuon : 0,
            //        SoLuongCan = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SoLuongCan")?.ToString(), out int slc) ? slc : 0,
            //        SLCanBangMau = float.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SLCanBangMau")?.ToString(), out float slCanBanMau) ? slCanBanMau : 0,
            //        DonGia = float.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "DonGia")?.ToString(), out float donGia) ? donGia : 0,
            //        ThanhTien = float.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "ThanhTien")?.ToString(), out float thanhTien) ? thanhTien : 0,
            //        SLThucTe = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SLThucTe")?.ToString(), out int sltt) ? sltt : 0,
            //        SLDat = Int32.TryParse(gridPLView.GetRowCellValue(focusedRowHandle, "SLDat")?.ToString(), out int sldat) ? sldat : 0,
            //        Chon = UnChon,
            //    };
            //    _lstUpdate.Add(_bom);
            //    string msResult = "";
            //    if (_lstUpdate == null || (_lstUpdate != null && _lstUpdate.Count == 0))
            //    {
            //        return;
            //    }
            //    string url = string.Format("{0}", URL + "BomNPL/PostBomVT");
            //    msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstUpdate); }).Result;

            //    if (msResult.ToLower() == "true")
            //    {
            //        LoadBOMPL(true);
            //        clsWaitForm.ShowSuccessForm(this, 2000);
            //    }
            //    else XtraMessageBox.Show(msResult);
            //    lstRowUpdate.Clear();
            //}
        }

        private void xtraTabControl1_Click(object sender, EventArgs e)
        {

        }

        private void bandedGridViewMau_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
   
        }

        private void repositoryItemCheckEdit5_CheckedChanged(object sender, EventArgs e)
        {
            int focusedRowHandle = bandedGridView1.FocusedRowHandle;
            bool isChecked = (sender as DevExpress.XtraEditors.CheckEdit).Checked;
            DataTable po = gridMauPLControl.DataSource as DataTable;
            if (focusedRowHandle >= 0 && po != null)
            {
                foreach (DataColumn col in po.Columns)
                {
                    if (col.ColumnName.Contains("@"))
                    {
                        bandedGridView1.SetRowCellValue(focusedRowHandle, col.ColumnName.ToString(), isChecked ? 1 : 0);
                    }
                }

            }
        }

        private void gridViewDMThongTin_FocusedRowChanged_1(object sender, FocusedRowChangedEventArgs e)
        {
            LoadBOMMAUNL(false);
            LoadBOMMAUPL(false);
        }

        private void repositoryItemCheckEdit4_CheckedChanged(object sender, EventArgs e)
        {
            
            int focusedRowHandle = bandedGridViewMau.FocusedRowHandle;
            bool isChecked = (sender as DevExpress.XtraEditors.CheckEdit).Checked;
            DataTable po = gridControlNLBOMMAU.DataSource as DataTable;
            if (focusedRowHandle >= 0 && po != null)
            {
                foreach (DataColumn col in po.Columns)
                {
                   if (col.ColumnName.Contains("@"))
                   {
                      bandedGridViewMau.SetRowCellValue(focusedRowHandle, col.ColumnName.ToString(), isChecked? 1:0);
                   }
                }

            }

        }

        //private void XoaBomDongVTNL()
        //{
        //    try
        //    {
        //        DialogResult messResult = MessageBox.Show("Bạn có muốn uncheck không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //        if (messResult == DialogResult.Yes)
        //        {
        //            if (gridNLView.FocusedRowHandle >= 0)
        //            {
        //                string MaBom = gridNLView.GetFocusedRowCellValue(colMaBomNL)?.ToString();
        //                string url = URL + $"BOMNPL/DeleteBomDongVT?MaBom={MaBom}";
        //                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
        //                if (result.ToLower() == "true")
        //                    loadDSBomNL();
        //                else XtraMessageBox.Show(result);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show("Không có dữ liệu vui lòng nhập.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private void gridNLView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            lstRowUpdate.Add(e.RowHandle);
        }

        private void gridNLView_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            //GridView view = sender as GridView;
            
            //if (view == null || view.FocusedRowHandle < 0 || view.FocusedColumn == null)
            //    return;

            //if (e.FocusedColumn == colMaVatTuNL&& view.FocusedRowHandle == _newRowHandle)
            //{

            //    popupSearchNL.Show();

            //    // Trì hoãn việc hiển thị pop-up của SearchLookUpEdit
            //    this.BeginInvoke(new Action(() =>
            //    {
            //        searchLookUpEditBomNL.Focus();
            //        searchLookUpEditBomNL.ShowPopup();
            //    }));
            //}
               
        }
      
        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Xoa();
        }
        private void gridViewDinhMucNguyenPhuLieu_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "DinhMuc")
            {
                // Tạo một RepositoryItemTextEdit
                var repositoryItemTextEdit = new RepositoryItemTextEdit();
                repositoryItemTextEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                repositoryItemTextEdit.Mask.EditMask = "n4"; // Cho phép nhập số thập phân với 2 chữ số sau dấu phẩy

                e.RepositoryItem = repositoryItemTextEdit;
            }
        }

        private void gridViewDinhMucNguyenPhuLieu_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.FocusedColumn.FieldName == "DinhMuc") // Thay "ColumnName" bằng tên cột cụ thể của bạn
            {
                if (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
                {
                    e.Valid = false;
                    e.ErrorText = "Giá trị không được để trống!";
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            Luu();
        }
        private void Luu()
        {
            try
            {
                rowhandel = gridViewDMThongTin.FocusedRowHandle;
                this.ActiveControl = this.button1;
                if (_dtSave != null || _dtSave.Rows.Count > 0)
                { 
                    string urlPNPL = string.Format("{0}?", URL + "DinhMuc/UpdateNPL");
                    string msPNPL = Task.Run(async () => { return await _clientExtension.PostAsync(urlPNPL, _dtSave); }).Result;
                    if (msPNPL.ToLower() != "true")
                        XtraMessageBox.Show(msPNPL);
                }
                clsWaitForm.ShowSuccessForm(this, 3000);
                LoadDSDMThongTin();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridViewDMThongTin_DataSourceChanged(object sender, EventArgs e)
        {
            _dtSave.Clear();
            isRowChanged = true;
            GridView view = sender as GridView;
            DataRowView dataRowView = view.GetFocusedRow() as DataRowView;
            if (dataRowView != null)
            {
                DataRow row = dataRowView.Row;
                string parameter = string.Format("{0}", row["MaGop"].ToString());
                LoadDSDinhMucDonHang(parameter);
                _maDonHang = row["MaGop"].ToString();
            }

        }

        private void gridViewDMThongTin_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.RowHandle >= 0)
            {
                string Strflth = view.GetRowCellDisplayText(e.RowHandle, view.Columns["SLTH"]);
                string Strfcl = view.GetRowCellDisplayText(e.RowHandle, view.Columns["SLCL"]);
                if (Strfcl == "0")
                {
                    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#84e184");
                    e.HighPriority = true;
                }
                if (Strflth != "0" && Strfcl != "0")
                {
                    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffff99");
                    e.HighPriority = true;
                }
                if (e.RowHandle == gridViewDMThongTin.FocusedRowHandle)
                {
                    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ddd");
                    e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    e.HighPriority = true;
                }
            }
        }

    }
}
