using DevExpress.XtraEditors;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
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
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using NtbSoft.ERP.Entity.ThuVien;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Repository;
using DevExpress.DataAccess.Excel;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmNguyenPhuLieuNew : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        private HttpClientExtension _clientExtension;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        string URL = string.Empty;
        private ResourceURL.EventStatus _status;
        List<VatTuEntity> listNguyenlieuEntity;
        List<VatTuEntity> listPhulieuEntity;
        List<DonViChungLoaiEntity> listDVT;
        List<NhomNguyenPhuLieuEntity> listNhomNPL;
        int FocusedIndex = 0;
        KeyDownControlHandler keyDownControlHandler;
        DataTable tbNL;
        DataTable tbPL;
        DataTable tbMau;
        DataTable tbKhoSize;
        DataTable tbVTMau;
        DataTable _dt = new DataTable();
        bool _isPost = false;
        bool _isNL = true;
        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        SearchCheckSelection gridCheckMarksMau;
        SearchCheckSelection gridCheckMarksKhoSize;
        private ToolTip toolTip = new ToolTip();
        string selectedValuesKhoSize = "";
        string KhoSizeID = "";
        string selectedValuesColor = "";
        string colorID = "";
        string listKhoSize = "";
        string listMau = "";
        Random random = new Random();
        public frmNguyenPhuLieuNew()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            listNguyenlieuEntity = new List<VatTuEntity>();
            listPhulieuEntity = new List<VatTuEntity>();
            listDVT = new List<DonViChungLoaiEntity>();
            listNhomNPL = new List<NhomNguyenPhuLieuEntity>();
            tbNL = new DataTable();
            tbPL = new DataTable();
            tbMau = new DataTable();
            tbKhoSize = new DataTable();
            tbVTMau = new DataTable();

        }

        private void frmNguyenLieu_Load(object sender, EventArgs e)
        {
            if(gVNguyenLieu.RowCount>0)
            {
            
                gVNguyenLieu.FocusedRowHandle = 0;
            }
            //gVNguyenLieu_RowClick(null, new DevExpress.XtraGrid.Views.Grid.RowClickEventArgs(DevExpress.XtraGrid.Views.Grid.RowClickEventArgs.MouseButtons.Left, 0));
        }
        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(ThemDong, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlEdit = new ActionControl(SuaDong, _allowEdit, ActionType.Edit, this.Sua.Enabled);
            actionControlDelete = new ActionControl(XoaDong, _allowDelete, ActionType.Delete, this.Xoa.Enabled);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(NapLaiDong, true, ActionType.Refresh, this.Naplai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd, actionControlEdit,
                actionControlDelete, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }
        protected override void OnLoad(EventArgs e)
        {

            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            Init();
            searchLookUpEdit4.Properties.ValueMember = "MaVT";
            searchLookUpEdit4.Properties.DisplayMember = "MaVT";
            searchLookUpEdit4.Properties.NullText = "[Chọn Vật tư]";
            SetSearchLookUpEditPopupState(searchLookUpEdit1, false);
            SetSearchLookUpEditPopupState(searchLookUpEdit2, false);
            SetSearchLookUpEditPopupState(searchLookUpEdit4, false);

            LoadDSPhuLieu(false);
            LoadDSNguyenLieu(false);
        }
        #region hàm load
        private void LoadDSNguyenLieu(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "KhoVatTu/GetNL");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _dt = JsonConvert.DeserializeObject<DataTable>(json);
                List<VatTuEntity> _lstTemp = new List<VatTuEntity>();
                if (json == "[]")
                {
                    tbNL = null;
                    listNguyenlieuEntity = new List<VatTuEntity>();
                    //gCNguyenLieu.DataSource = listNguyenlieuEntity;
                    gCNguyenLieu.DataSource = null;
                    gCNguyenLieu.Refresh();
                    Xoa.Enabled = false;
                    Sua.Enabled = false;

                    btnXuatExcel.Enabled = false;


                    setTxtNull();

                    return;
                }
                if (!string.IsNullOrEmpty(json))
                {
                    tbNL = JsonConvert.DeserializeObject<DataTable>(json);

                    listNguyenlieuEntity = JsonConvert.DeserializeObject<List<VatTuEntity>>(json);

                    if (listNguyenlieuEntity.Count == 0)
                    {
                        gCNguyenLieu.DataSource = null;
                        gCNguyenLieu.Refresh();
                        return;
                    }
                    _lstTemp = listNguyenlieuEntity;
                    foreach (var vt in _lstTemp)
                    {
                        listDVT = reloadDVCL();
                        listNhomNPL = reloadNhomNPL();
                        NhomNguyenPhuLieuEntity result1 = listNhomNPL.FirstOrDefault(n => RemoveVietnameseTone(n.MaNhom).Replace(" ", string.Empty) == RemoveVietnameseTone(vt.MaNhomVT).Replace(" ", string.Empty));
                        DonViChungLoaiEntity result2 = listDVT.FirstOrDefault(n => RemoveVietnameseTone(n.MaDVCL).Replace(" ", string.Empty) == RemoveVietnameseTone(vt.DVTinh).Replace(" ", string.Empty));
                        if (result1 != null)
                        {
                            vt.MaNhomVT = result1.TenNhom;
                        }
                        if (result2 != null)
                        {
                            vt.DVTinh = result2.TenDVCL;
                        }


                    }


                    if (!_isPost && _isNL)
                    {
                        loadThongTinNguyenLieu(listNguyenlieuEntity[0].ID);
                    }

                    Xoa.Enabled = true;
                    Sua.Enabled = true;
                    btnXuatExcel.Enabled = true;
                }

                gCNguyenLieu.DataSource = _lstTemp;

                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gVNguyenLieu.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gCNguyenLieu;
                }
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi khi tải danh sách nguyên liệu.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadDSPhuLieu(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "KhoVatTu/GetPL");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                List<VatTuEntity> _lstTemp = new List<VatTuEntity>();
                if (json == "[]")
                {

                    tbPL = null;
                    listPhulieuEntity = new List<VatTuEntity>();
                    // gCPhuLieu.DataSource = listNguyenlieuEntity;
                    gCPhuLieu.DataSource = null;
                    gCPhuLieu.Refresh();
                    Xoa.Enabled = false;
                    Sua.Enabled = false;
                    btnXuatExcel.Enabled = false;
                    setTxtNull();

                    return;
                }
                if (!string.IsNullOrEmpty(json))
                {
                    tbPL = JsonConvert.DeserializeObject<DataTable>(json);

                    listPhulieuEntity = JsonConvert.DeserializeObject<List<VatTuEntity>>(json);

                    if (listPhulieuEntity.Count == 0)
                    {
                        gCPhuLieu.DataSource = null;
                        gCPhuLieu.Refresh();
                        return;
                    }
                    _lstTemp = listPhulieuEntity;
                    foreach (var vt in _lstTemp)
                    {
                        listDVT = reloadDVCL();
                        listNhomNPL = reloadNhomNPL();
                        NhomNguyenPhuLieuEntity result1 = listNhomNPL.FirstOrDefault(n => RemoveVietnameseTone(n.MaNhom).Replace(" ", string.Empty) == RemoveVietnameseTone(vt.MaNhomVT).Replace(" ", string.Empty));
                        DonViChungLoaiEntity result2 = listDVT.FirstOrDefault(n => RemoveVietnameseTone(n.MaDVCL).Replace(" ", string.Empty) == RemoveVietnameseTone(vt.DVTinh).Replace(" ", string.Empty));
                        if (result1 != null)
                        {
                            vt.MaNhomVT = result1.TenNhom;
                        }
                        if (result2 != null)
                        {
                            vt.DVTinh = result2.TenDVCL;
                        }
                    }

                    if (!_isPost && !_isNL)
                    {
                        loadThongTinPhuLieu(listPhulieuEntity[0].ID);
                    }

                    Xoa.Enabled = true;
                    Sua.Enabled = true;
                    btnXuatExcel.Enabled = true;
                }

                gCPhuLieu.DataSource = _lstTemp;

                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gVPhuLieu.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gCPhuLieu;
                }
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi khi tải tính năng phụ liệu.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadThongTinNguyenLieu(int ID)
        {
            VatTuEntity result = listNguyenlieuEntity.First(nl => nl.ID == ID);
            if (result != null)
            {
                setTxtEdit(true);
                setTxtValue(result);

            }
            else
            {
                setTxtEdit(true);
                setTxtNull();

            }
        }
        private void loadThongTinPhuLieu(int ID)
        {

            VatTuEntity result = listPhulieuEntity.First(nl => nl.ID == ID);
            if (result != null)
            {
                setTxtValue(result);
                setTxtEdit(true);
            }
            else
            {
                setTxtNull();
                setTxtEdit(true);
            }
        }
        private void LoadKhoSizeVT()
        {
            try
            {
                string url = $"{URL}ThuVienKhoSize/Get";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                tbKhoSize = JsonConvert.DeserializeObject<DataTable>(json);
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                searchLookUpEdit2.Properties.DataSource = tbl;
                searchLookUpEdit1.RefreshEditValue();
                searchLookUpEdit1.Refresh();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }


        }
        private void LoadVatTuMau()
        {
            try
            {
                string url = $"{URL}VatTuChiTiet/Get";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                tbVTMau = JsonConvert.DeserializeObject<DataTable>(json);
                searchLookUpEdit3.Properties.DataSource = tbl;
                searchLookUpEdit4.Properties.DataSource = tbl;
                searchLookUpEdit3.RefreshEditValue();
                searchLookUpEdit3.Refresh();

                if (searchLookUpEdit3.Properties.View == null)
                {
                    Console.WriteLine("searchLookUpEdit3.Properties.View chưa được khởi tạo.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }


        }
        private void LoadMaMauKH()
        {
            try
            {
                string url = $"{URL}ThuVienMau/Get";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                tbMau = JsonConvert.DeserializeObject<DataTable>(json);
                searchLookUpEdit1.Properties.DataSource = tbl;
                //searchLookUpEdit1.Properties.View.PopulateColumns();
                //searchLookUpEdit1.Properties.View.RefreshData();
                searchLookUpEdit1.RefreshEditValue();
                searchLookUpEdit1.Refresh();
                if (searchLookUpEdit1.Properties.View == null)
                {
                    Console.WriteLine("searchLookUpEdit1.Properties.View chưa được khởi tạo.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
            }


        }
        private void Init()
        {

            searchLookUpEdit1.Properties.ValueMember = "MaMauKH";
            searchLookUpEdit1.Properties.DisplayMember = "TenMauKH";
            searchLookUpEdit1.Properties.NullText = "[Chọn màu]";
            searchLookUpEdit1.Properties.PopupView.OptionsSelection.MultiSelect = true;
            var gridView = searchLookUpEdit1.Properties.PopupView as GridView;
            if (gridView != null)
            {
                // Xóa cột checkbox
                var checkColumn = gridView.Columns["CheckMarkSelection"];
                if (checkColumn != null)
                    gridView.Columns.Remove(checkColumn);

                // Làm sạch sự kiện đã gắn trước đó

            }
            gridCheckMarksMau = new SearchCheckSelection(searchLookUpEdit1.Properties);

            gridCheckMarksMau.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEdit1_SelectionChanged);
            searchLookUpEdit1.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEditMau_CustomDisplayText);
            searchLookUpEdit1.Tag = gridCheckMarksMau;
            LoadMaMauKH();

            searchLookUpEdit2.Properties.ValueMember = "MaKhoSize";
            searchLookUpEdit2.Properties.DisplayMember = "KhoSize";
            searchLookUpEdit2.Properties.NullText = "[Chọn Khổ/Size]";
            searchLookUpEdit2.Properties.PopupView.OptionsSelection.MultiSelect = true;
            var gridView2 = searchLookUpEdit2.Properties.PopupView as GridView;
            if (gridView2 != null)
            {
                // Xóa cột checkbox
                var checkColumn = gridView2.Columns["CheckMarkSelection"];
                if (checkColumn != null)
                    gridView2.Columns.Remove(checkColumn);



            }
            gridCheckMarksKhoSize = new SearchCheckSelection(searchLookUpEdit2.Properties);

            gridCheckMarksKhoSize.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEdit2_SelectionChanged);
            searchLookUpEdit2.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEditKhoSize_CustomDisplayText);
            searchLookUpEdit2.Tag = gridCheckMarksKhoSize;
            LoadKhoSizeVT();

            LoadVatTuMau();
        }
        private void reinit()
        {
            // Chuyển searchLookUpEdit1 sang chế độ SingleSelect
            if (searchLookUpEdit1.Tag is SearchCheckSelection gridCheckMarksMau)
            {
                var gridView = searchLookUpEdit1.Properties.PopupView as GridView;
                if (gridView != null)
                {
                    // Xóa cột checkbox
                    var checkColumn = gridView.Columns["CheckMarkSelection"];
                    if (checkColumn != null)
                        gridView.Columns.Remove(checkColumn);

                    // Làm sạch sự kiện đã gắn trước đó

                }

                searchLookUpEdit1.Tag = null;
            }

            // Cấu hình lại chế độ SingleSelect cho searchLookUpEdit1
            searchLookUpEdit1.Properties.ValueMember = "MaMauKH";
            searchLookUpEdit1.Properties.DisplayMember = "TenMauKH";
            searchLookUpEdit1.Properties.NullText = "[Chọn màu]";
            searchLookUpEdit1.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard; // Cho phép nhập văn bản
            searchLookUpEdit1.Properties.PopupView.OptionsSelection.MultiSelect = false; // Tắt MultiSelect

            // Gỡ bỏ sự kiện cũ
            searchLookUpEdit1.CustomDisplayText -= searchLookUpEditMau_CustomDisplayText;
            searchLookUpEdit1.EditValueChanged -= searchLookUpEdit1_EditValueChanged;
            searchLookUpEdit1.EditValueChanged += searchLookUpEdit1_EditValueChangedSingle;
            // Gắn lại dữ liệu
            LoadMaMauKH();

            // Chuyển searchLookUpEdit2 sang chế độ SingleSelect
            if (searchLookUpEdit2.Tag is SearchCheckSelection gridCheckMarksKhoSize)
            {
                var gridView = searchLookUpEdit2.Properties.PopupView as GridView;
                if (gridView != null)
                {
                    // Xóa cột checkbox
                    var checkColumn = gridView.Columns["CheckMarkSelection"];
                    if (checkColumn != null)
                        gridView.Columns.Remove(checkColumn);

                    // Làm sạch sự kiện đã gắn trước đó

                }

                searchLookUpEdit2.Tag = null;
            }

            // Cấu hình lại chế độ SingleSelect cho searchLookUpEdit2
            searchLookUpEdit2.Properties.ValueMember = "MaKhoSize";
            searchLookUpEdit2.Properties.DisplayMember = "KhoSize";
            searchLookUpEdit2.Properties.NullText = "[Chọn Khổ/Size]";
            searchLookUpEdit2.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard; // Cho phép nhập văn bản
            searchLookUpEdit2.Properties.PopupView.OptionsSelection.MultiSelect = false; // Tắt MultiSelect

            // Gỡ bỏ sự kiện cũ
            searchLookUpEdit2.CustomDisplayText -= searchLookUpEditKhoSize_CustomDisplayText;

            // Gắn lại dữ liệu
            LoadKhoSizeVT();
        }

        #endregion

        private void searchLookUpEdit1_EditValueChangedSingle(object sender, EventArgs e)
        {
            if (_isPost)
            {
                var selectedValue = searchLookUpEdit1.EditValue;
                if (selectedValue != null)
                {
                    // Lấy DataRow của dòng được chọn
                    var selectedRow = searchLookUpEdit1.Properties.View.GetFocusedDataRow();

                    if (selectedRow != null)
                    {
                        // Gán MaVT vào ô SearchLookUpEdit (đã thực hiện qua ValueMember)
                        searchLookUpEdit1.EditValue = selectedRow["MaMauKH"];

                        // Gán TenVT vào ô TextEdit
                        txtCodeMau.Text = selectedRow["CodeMauKH"].ToString();
                    }
                }
            }
            else
            {

            }
           
            
        }

        void searchLookUpEditMau_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
        }
        void searchLookUpEditKhoSize_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
        }
        void searchLookUpEdit1_SelectionChanged(object sender, EventArgs e)
        {
            selectedValuesColor = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEdit1.Properties.ValueMember)));
            searchLookUpEdit1.EditValue = selectedValuesColor;
            if (searchLookUpEdit1.EditValue is null) return;
            colorID = searchLookUpEdit1.EditValue.ToString();
        }
        void searchLookUpEdit2_SelectionChanged(object sender, EventArgs e)
        {
            selectedValuesKhoSize = string.Join(";", searchLookUpEdit2View.GetSelectedRows().Select(rowHandle => searchLookUpEdit2View.GetRowCellValue(rowHandle, searchLookUpEdit2.Properties.ValueMember)));
            searchLookUpEdit2.EditValue = selectedValuesKhoSize;
            if (searchLookUpEdit2.EditValue is null) return;
            KhoSizeID = searchLookUpEdit2.EditValue.ToString();
        }
       
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);//SystemUser/GetPer/userID=...
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
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
                Luu.Enabled = false;
            }
            else
            {
                Luu.Enabled = false;
            }
            if (!_allowDelete)
                Xoa.Enabled = false;

        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ThemDong();
            Them.Enabled = false;
            Sua.Enabled = false;
            Luu.Enabled = true;
        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gVNguyenLieu.OptionsBehavior.Editable = false;
                    gVPhuLieu.OptionsBehavior.Editable = false;
                    if (_allowAdd)
                    {
                        Them.Enabled = true;
                        actionControlAdd.Enabled = true;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = true;
                        actionControlEdit.Enabled = true;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = true;
                        actionControlDelete.Enabled = true;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = false;
                        actionControlSave.Enabled = false;
                    }

                    break;
                case ResourceURL.EventStatus.Edit:
                    gVNguyenLieu.OptionsBehavior.Editable = true;
                    gVPhuLieu.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }
                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }

                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }

                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }
                    break;
                case ResourceURL.EventStatus.Add:
                    gVNguyenLieu.OptionsBehavior.Editable = true;
                    gVPhuLieu.OptionsBehavior.Editable = true;
                    if (_allowAdd)
                    {
                        Them.Enabled = false;
                        actionControlAdd.Enabled = false;
                    }

                    if (_allowEdit)
                    {
                        Sua.Enabled = false;
                        actionControlEdit.Enabled = false;
                    }
                    if (_allowDelete)
                    {
                        Xoa.Enabled = false;
                        actionControlDelete.Enabled = false;
                    }
                    Sua.Enabled = false;
                    if (_allowAdd || _allowEdit)
                    {
                        Luu.Enabled = true;
                        actionControlSave.Enabled = true;
                    }

                    break;
            }
        }
 
      
        private void ThemDong()
        {
            Init();
            gridCheckMarksMau.ClearSelection(searchLookUpEdit1View);
            searchLookUpEdit1.EditValue = null;
            searchLookUpEdit1.Refresh();

            gridCheckMarksKhoSize.ClearSelection(searchLookUpEdit2View);
            searchLookUpEdit2.EditValue = null;
            searchLookUpEdit2.Refresh();
            _isPost = true;
            SetSearchLookUpEditPopupState(searchLookUpEdit1, true);
            SetSearchLookUpEditPopupState(searchLookUpEdit2, true);
          
            SetSearchLookUpEditPopupState(searchLookUpEdit4, true);
            setTxtNull();
            setTxtEdit(false);
            Xoa.Enabled = false;
            Sua.Enabled = false;
            LoadNhomNPL();
            LoadDVCL();
            LoadisNL();
        }
        private void NapLaiDong()
        {
            /* LoadDSKhachHang(false);
             _status = ResourceURL.EventStatus.View;
             GridViewUpdateStatus(_status);
             gridViewKhachHang.OptionsView.NewItemRowPosition = NewItemRowPosition.None;*/
            Init();
            gridCheckMarksMau.ClearSelection(searchLookUpEdit1View);
            searchLookUpEdit1.EditValue = null;
            searchLookUpEdit1.Refresh();

            gridCheckMarksKhoSize.ClearSelection(searchLookUpEdit2View);
            searchLookUpEdit2.EditValue = null;
            searchLookUpEdit2.Refresh();

            _isPost = false;
            LoadMaMauKH();
            LoadKhoSizeVT();
            LoadDSPhuLieu(false);
            Them.Enabled = true;
            SetSearchLookUpEditPopupState(searchLookUpEdit1, false);
            SetSearchLookUpEditPopupState(searchLookUpEdit2, false);
         
            SetSearchLookUpEditPopupState(searchLookUpEdit4, false);
            setTxtEdit(true);
            LoadDSNguyenLieu(false);
           
        }
        private void SuaDong()
        {
            reinit();
            SetSearchLookUpEditPopupState(searchLookUpEdit1, true);
            SetSearchLookUpEditPopupState(searchLookUpEdit2, true);
          
            SetSearchLookUpEditPopupState(searchLookUpEdit4, true);
            VatTuEntity row=null;
            if (_isNL)
            {
                row = gVNguyenLieu.GetRow(gVNguyenLieu.FocusedRowHandle) as VatTuEntity;
            }
            else
            {
                row = gVPhuLieu.GetRow(gVPhuLieu.FocusedRowHandle) as VatTuEntity;
            }
            if (row == null) return;
            LoadNhomNPL(row.MaNhomVT);
            LoadDVCL(row.DVTinh);
            searchLookUpEdit1.EditValue = row.MaMau;
            searchLookUpEdit2.EditValue = row.MaSizeKho;
            _isPost = true;
            setTxtEdit(false);
            Xoa.Enabled = false;
            Sua.Enabled = false;
            Luu.Enabled = true;
            Them.Enabled = false;
        }

        private void Sua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaDong();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuDong();
          
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLaiDong();
           
        }


        private void gVNguyenLieu_Click(object sender, EventArgs e)
        {


            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            if (view != null)
            {

                int rowHandle = view.FocusedRowHandle;


                if (rowHandle >= 0)
                {

                    if (!_isPost)
                    {
                        Them.Enabled = true;
                        Sua.Enabled = true;
                        Xoa.Enabled = true;
                        Luu.Enabled = false;
                    }
                    int idValue = int.Parse(view.GetRowCellValue(rowHandle, "ID").ToString());
                    if (!_isPost)
                    { loadThongTinNguyenLieu(idValue); }


                }
            }
        }

        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    VatTuEntity row;
                    if (_isNL)
                    {
                        row = gVNguyenLieu.GetRow(gVNguyenLieu.FocusedRowHandle) as VatTuEntity;
                    }
                    else
                    {
                        row = gVPhuLieu.GetRow(gVPhuLieu.FocusedRowHandle) as VatTuEntity;
                    }

                    string url = string.Format("{0}?ID={1}", URL + "KhoVatTu/Delete", row.ID);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                    {

                        LoadDSNguyenLieu(false);
                        LoadDSPhuLieu(false);
                        if (gVNguyenLieu.RowCount == 0 || gVPhuLieu.RowCount == 0)
                        {
                            setTxtNull();
                        }
                    }
                    else XtraMessageBox.Show(" Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(" Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void btnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable _excel = ImportExcel();
            if(_excel!=null)
            {
                frmNhapNguyenPhuLieuNew frm = new frmNhapNguyenPhuLieuNew(_excel);
                frm.WindowState = FormWindowState.Maximized;
                frm.ShowDialog();
            }
           
           
            LoadDSNguyenLieu(false);
            LoadDSPhuLieu(false);

        }
  
        private DataTable ImportExcel()
        {
            try
            {
                // Hiển thị hộp thoại để chọn file Excel
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Excel File|*.xlsx;*.xls";
                openFileDialog1.Title = "Import Excel";
                openFileDialog1.Multiselect = false;
                DialogResult dialogResult = openFileDialog1.ShowDialog();
                if (dialogResult != DialogResult.OK) return null;

                // Lấy đường dẫn file và kiểm tra
                string filePath = openFileDialog1.FileName;
                if (string.IsNullOrEmpty(filePath)) return null;
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                string firstSheetName;
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    firstSheetName = package.Workbook.Worksheets[0].Name;
                }

                string range = "$A2:ZZ500";      // Vùng dữ liệu cần đọc

                // Khởi tạo ExcelDataSource
                var source = new DevExpress.DataAccess.Excel.ExcelDataSource();
                source.FileName = filePath;
                var worksheetSettings = new ExcelWorksheetSettings(firstSheetName, range);
                source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                source.Fill(); // Nạp dữ liệu từ file Excel

                // Chuyển dữ liệu từ ExcelDataSource sang DataTable
                DataTable dt = source.ToDataTable();

                // Tạo DataTable để lưu dữ liệu đã xử lý
                DataTable dtSave = CreateTblSave();

                // Duyệt qua từng dòng trong DataTable và kiểm tra dữ liệu
                for (int i = 0; i < dt.Rows.Count; i++) // Không cần bỏ qua dòng đầu tiên vì range đã bắt đầu từ A2
                {
                    bool isValidRow =
                        !string.IsNullOrWhiteSpace(dt.Rows[i][1]?.ToString()) && // MaKeToan
                        !string.IsNullOrWhiteSpace(dt.Rows[i][2]?.ToString()) && // MaKho
                        !string.IsNullOrWhiteSpace(dt.Rows[i][3]?.ToString()) && // MaVT
                        !string.IsNullOrWhiteSpace(dt.Rows[i][4]?.ToString()) && // CodeVT
                        !string.IsNullOrWhiteSpace(dt.Rows[i][5]?.ToString()) && // TenVT
                        !string.IsNullOrWhiteSpace(dt.Rows[i][6]?.ToString()) && // CodeMau
                        !string.IsNullOrWhiteSpace(dt.Rows[i][8]?.ToString()) && // SizeKho
                        !string.IsNullOrWhiteSpace(dt.Rows[i][10]?.ToString());  // DVTinh

                    // Nếu dòng không hợp lệ, bỏ qua
                    if (!isValidRow)
                    {
                        continue;
                    }

                    // Tạo dòng mới trong DataTable dtSave
                    var drnew = dtSave.NewRow();
                    drnew["ID"] = 0;
                    drnew["MaVTMau"] = ReplaceSpecialCharactersAndRemoveSpaces(RemoveVietnameseTone(dt.Rows[i][4].ToString()).Replace(" ", string.Empty)) + random.Next(0, 999) + DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
                    drnew["MaKeToan"] = dt.Rows[i][1].ToString();
                    drnew["MaKho"] = dt.Rows[i][2].ToString();
                    drnew["MaVT"] = dt.Rows[i][3].ToString();
                    drnew["CodeVT"] = "VT" + dt.Rows[i][4].ToString();
                    drnew["TenVT"] = dt.Rows[i][5].ToString();
                    drnew["MaMau"] = "";
                    drnew["Mau"] = "";
                    drnew["CodeMau"] = dt.Rows[i][6].ToString();
                    drnew["ItemCode"] = dt.Rows[i][7].ToString();
                    drnew["MaSizeKho"] = "";
                    drnew["SizeKho"] = dt.Rows[i][8].ToString();
                    drnew["TenGD"] = dt.Rows[i][9].ToString();
                    drnew["MaDVTinh"] = "";
                    drnew["DVTinh"] = dt.Rows[i][10].ToString();
                    drnew["DV1"] = float.Parse("0.9144");
                    drnew["DV2"] = float.Parse("1.09361");
                    drnew["MaNhomVT"] = dt.Rows[i][11].ToString();
                    drnew["NhaCungCap"] = dt.Rows[i][12].ToString();
                    drnew["MaVTHaiQuan"] = dt.Rows[i][13].ToString();
                    drnew["TenHaiQuan"] = dt.Rows[i][14].ToString();
                    drnew["XuatXu"] = dt.Rows[i][15].ToString();
                    drnew["SLTonToiThieu"] = dt.Rows[i][16].ToString() == "" ? 0 : float.Parse(dt.Rows[i][16].ToString());
                    drnew["SLTonToiDa"] = dt.Rows[i][17].ToString() == "" ? 0 : float.Parse(dt.Rows[i][17].ToString());
                    drnew["TKVT"] = dt.Rows[i][18].ToString();
                    drnew["TKGiaVon"] = dt.Rows[i][19].ToString();
                    drnew["TKDoanhThu"] = dt.Rows[i][20].ToString();
                    drnew["TKHangBanTraLai"] = dt.Rows[i][21].ToString();
                    drnew["ThueSuat"] = dt.Rows[i][22].ToString() == "" ? 0 : float.Parse(dt.Rows[i][22].ToString());
                    drnew["GhiChu"] = dt.Rows[i][23].ToString();
                    drnew["IsNL"] = dt.Rows[i][24].ToString().Trim() == "Nguyên liệu" ? true : false;
                    drnew["IsPL"] = dt.Rows[i][24].ToString().Trim() == "Nguyên liệu" ? false : true;

                    // Thêm dòng vào DataTable dtSave
                    dtSave.Rows.Add(drnew);
                }

                // Lưu dữ liệu đã xử lý
                return SaveData(dtSave);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                XtraMessageBox.Show("Đã xảy ra lỗi khi xử lý file Excel. Vui lòng kiểm tra lại!\n" + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private DataTable SaveData(DataTable dtSave)
        {

            List<NhomNguyenPhuLieuEntity> listNhomNPL = reloadNhomNPL();
            List<DonViChungLoaiEntity> listDVCL = reloadDVCL();

           /* foreach (DataRow row in dtSave.Rows)
            {

                string _nhomNPLValue = row["MaNhomVT"].ToString();
                string _donvitinhValue = row["DVTinh"].ToString();
                if (_nhomNPLValue == "" || _donvitinhValue == "")
                {
                    continue;
                }
                row["MaNhomVT"] = addValueNhomNPL(_nhomNPLValue);
                row["MaDVTinh"]= addValueDVCL(_donvitinhValue);
                
                listNhomNPL = reloadNhomNPL();
                listDVCL = reloadDVCL();
            }*/
            if (dtSave.Rows.Count == 0)
            {
                XtraMessageBox.Show("File Excel rỗng! Vui lòng thử lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return dtSave;
        }
      


        private DataTable GetSchemaTable(string connectionString)
        {
            using (OleDbConnection connection = new
                       OleDbConnection(connectionString))
            {
                connection.Open();
                DataTable schemaTable = connection.GetOleDbSchemaTable(
                    OleDbSchemaGuid.Tables,
                    new object[] { null, null, null, "TABLE" });
                return schemaTable;
            }
        }
        private DataTable CreateTblSave()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaVTMau", typeof(string));
            dt.Columns.Add("MaKeToan", typeof(string));
            dt.Columns.Add("MaVT", typeof(string));
            dt.Columns.Add("TenVT", typeof(string));
            dt.Columns.Add("CodeVT", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("Mau", typeof(string));
            dt.Columns.Add("CodeMau", typeof(string));
            dt.Columns.Add("ItemCode", typeof(string));
            dt.Columns.Add("MaSizeKho", typeof(string));
            dt.Columns.Add("SizeKho", typeof(string));
            dt.Columns.Add("TenGD", typeof(string));
            dt.Columns.Add("MaKho", typeof(string));
            dt.Columns.Add("MaDVTinh", typeof(string));
            dt.Columns.Add("DVTinh", typeof(string));
            dt.Columns.Add("DV1", typeof(float));
            dt.Columns.Add("DV2", typeof(float));
            dt.Columns.Add("MaNhomVT", typeof(string));
            dt.Columns.Add("NhaCungCap", typeof(string));
            dt.Columns.Add("MaVTHaiQuan", typeof(string));
            dt.Columns.Add("TenHaiQuan", typeof(string));
            dt.Columns.Add("XuatXu", typeof(string));
            dt.Columns.Add("SLTonToiThieu", typeof(float));
            dt.Columns.Add("SLTonToiDa", typeof(float));
            dt.Columns.Add("TKVT", typeof(string));
            dt.Columns.Add("TKGiaVon", typeof(string));
            dt.Columns.Add("TKDoanhThu", typeof(string));
            dt.Columns.Add("TKHangBanTraLai", typeof(string));
            dt.Columns.Add("ThueSuat", typeof(float));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("IsNL", typeof(bool));
            dt.Columns.Add("IsPL", typeof(bool));
            return dt;
        }
        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input.Trim(), replacement);
        }
        private void gvNguyenLieu_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadDSNguyenLieu(false);
            LoadDSPhuLieu(false);
            /* string urlNL = string.Format("{0}?", URL + "KhoVatTu/GetNL");
             string jsonNL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNL); }).Result;
             string urlPL = string.Format("{0}?", URL + "KhoVatTu/GetNPLL");
             string jsonPL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlPL); }).Result;
             if(jsonNL!="[]")
             {
                 tbNL = JsonConvert.DeserializeObject<DataTable>(jsonNL);
             }    
             if(jsonPL!="[]")
             {
                 tbPL = JsonConvert.DeserializeObject<DataTable>(jsonPL);
             }    
             */
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            if (tbNL != null && tbPL != null)
            {
                tbNL.Merge(tbPL);
            }
            else if (tbNL == null && tbPL != null)
            {
                tbNL = tbPL;
            }

            if (tbNL is null || tbNL.Rows.Count == 0) return;
            bool isTableEmpty = tbNL.AsEnumerable().All(row =>
    row.ItemArray.All(value => value is string str && string.IsNullOrEmpty(str)));

            if (isTableEmpty)
            {
                return;
            }
            DataTable dtTbl = tbNL;
            if (!dtTbl.Columns.Contains("STT"))
            {
                dtTbl.Columns.Add("STT", typeof(int));
            }
            if (!dtTbl.Columns.Contains("LoaiVatTu"))
            {
                dtTbl.Columns.Add("LoaiVatTu", typeof(string));
            }

            for (int i = 0; i < dtTbl.Rows.Count; i++)
            {
                dtTbl.Rows[i]["STT"] = i + 1;
                dtTbl.Rows[i]["LoaiVatTu"] = bool.Parse(dtTbl.Rows[i]["IsNL"]?.ToString()) == true ? "Nguyên liệu" : "Phụ liệu";
            }
            dtTbl.Columns["STT"].SetOrdinal(0);
            dtTbl.Columns["MaKeToan"].SetOrdinal(1);
            dtTbl.Columns["MaKho"].SetOrdinal(2);
            dtTbl.Columns["MaVT"].SetOrdinal(3);
            dtTbl.Columns["CodeVT"].SetOrdinal(4);
            dtTbl.Columns["TenVT"].SetOrdinal(5);
            dtTbl.Columns["Mau"].SetOrdinal(6);
            dtTbl.Columns["CodeMau"].SetOrdinal(7);
            dtTbl.Columns["ItemCode"].SetOrdinal(8);
            /*dtTbl.Columns["MaKho"].SetOrdinal(2);
            dtTbl.Columns["MaKho"].SetOrdinal(2);
            dtTbl.Columns["MaKho"].SetOrdinal(2);*/
            dtTbl.Columns.Remove("ID");
            dtTbl.Columns.Remove("MaVTMau");
            dtTbl.Columns.Remove("MaMau");
            dtTbl.Columns.Remove("MaSizeKho");
            dtTbl.Columns.Remove("MaDVTinh");
            dtTbl.Columns.Remove("DV1");
            dtTbl.Columns.Remove("DV2");
            dtTbl.Columns.Remove("IsNL");
            dtTbl.Columns.Remove("IsPL");
            Sfd.FileName = string.Format("TongHopVatTu");
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Nguyên liệu");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
                    op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                    op.ShowGridLines = true;
                    op.SheetName = string.Format("NguyenLieu");
                    ExportExcel(Sfd.FileName, dtTbl);
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn mở tài liệu này không?", "Thông báo",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                }
            }
        }

        private void ExportExcel(string path, DataTable dtNguyenLieu)
        {
            try
            {
                FileInfo file = new FileInfo(path);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("VatTu");
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells.Style.Font.Name = "Arial";
                    worksheet.Cells.Style.Font.Size = 12;

                    // Tiêu đề chính
                    range = worksheet.Cells["C1:L1"];
                    range.Merge = true;
                    range.Value = "CÔNG TY TNHH PHÁT TRIỂN CÔNG NGHỆ NAM THANH BÌNH";
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 14;

                    range = worksheet.Cells["C2:L3"];
                    range.Merge = true;
                    range.Value = "Vật tư";
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 14;

                    worksheet.Cells.Style.Font.Size = 12;

                    // Thiết lập tiêu đề bảng
                    for (int i = 0; i < dtNguyenLieu.Columns.Count; i++)
                    {
                        worksheet.Cells[4, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Kiểu nền
                        worksheet.Cells[4, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 112, 192)); // Màu xanh dương đậm
                        worksheet.Cells[4, i + 1].Style.Font.Bold = true; // Đặt tiêu đề in đậm
                        worksheet.Cells[4, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White); // Chữ màu trắng
                        worksheet.Cells[4, i + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                    }

                    // Đặt giá trị tiêu đề cho từng cột
                    worksheet.Cells[4, 1].Value = "STT";
                    worksheet.Cells[4, 2].Value = "Mã kế toán";
                    worksheet.Cells[4, 3].Value = "Mã kho";
                    worksheet.Cells[4, 4].Value = "Mã vật tư";
                    worksheet.Cells[4, 5].Value = "Code vật tư";
                    worksheet.Cells[4, 6].Value = "Tên vật tư";
                    worksheet.Cells[4, 7].Value = "Màu";
                    worksheet.Cells[4, 8].Value = "Code Màu";
                    worksheet.Cells[4, 9].Value = "ItemCode";
                    worksheet.Cells[4, 10].Value = "Size khổ";
                    worksheet.Cells[4, 11].Value = "Tên giao dịch";
                    worksheet.Cells[4, 12].Value = "Đơn vị tính";
                    worksheet.Cells[4, 13].Value = "Nhóm Vật tư";
                    worksheet.Cells[4, 14].Value = "Nhà cung cấp";
                    worksheet.Cells[4, 15].Value = "Mã hải quan";
                    worksheet.Cells[4, 16].Value = "Tên hải quan";
                    worksheet.Cells[4, 17].Value = "Xuất xứ";
                    worksheet.Cells[4, 18].Value = "SL tồn tối thiểu";
                    worksheet.Cells[4, 19].Value = "SL tồn tối đa";
                    worksheet.Cells[4, 20].Value = "TK vật tư";
                    worksheet.Cells[4, 21].Value = "TK giá vốn";
                    worksheet.Cells[4, 22].Value = "TK doanh thu";
                    worksheet.Cells[4, 23].Value = "TK hàng bán trả lại";
                    worksheet.Cells[4, 24].Value = "Thuế suất";
                    worksheet.Cells[4, 25].Value = "Ghi chú";
                    worksheet.Cells[4, 26].Value = "Loại vật tư";

                    // Thêm dữ liệu vào worksheet và tô màu dòng chẵn/lẻ
                    for (int i = 0; i < dtNguyenLieu.Rows.Count; i++)
                    {
                        for (int j = 0; j < dtNguyenLieu.Columns.Count; j++)
                        {
                            worksheet.Cells[i + 5, j + 1].Value = dtNguyenLieu.Rows[i][j];

                            // Tô màu dòng chẵn/lẻ
                            if ((i + 1) % 2 == 0) // Dòng chẵn
                            {
                                worksheet.Cells[i + 5, j + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                worksheet.Cells[i + 5, j + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242)); // Màu xám nhạt
                            }
                            else // Dòng lẻ
                            {
                                worksheet.Cells[i + 5, j + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                worksheet.Cells[i + 5, j + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 255)); // Màu trắng
                            }

                            // Căn chỉnh dữ liệu
                            worksheet.Cells[i + 5, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                            // Căn phải cho các cột đặc biệt
                            if ((j > 16 && j < 24) || j == 0)
                            {
                                worksheet.Cells[i + 5, j + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            }
                            worksheet.Cells[i + 5, j + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, System.Drawing.Color.Black);
                        }
                    }

                    // Tự động điều chỉnh độ rộng cột 
                    worksheet.Cells.AutoFitColumns();

                    // Lưu file
                    excelPackage.SaveAs(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi xuất excel: " + ex.StackTrace);
                throw;
            }
        }


        private async void LuuDong()
        {
            // Validate dữ liệu
            addValueDVCL(txtDVTinh.Text);
            addValueNhomNPL(txtMaNhomVT.Text);
            string[] arrMau = listMau.Split(';').Where(m => !string.IsNullOrWhiteSpace(m)).ToArray();
            string[] arrKhoSize = listKhoSize.Split(';').Where(k => !string.IsNullOrWhiteSpace(k)).ToArray();
            string[] arrCodeMau = txtCodeMau.Text.Split(';').Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
            int isAdd = int.TryParse(txtID.Text, out int idadd) ? idadd : 0;
            // Trường hợp chỉnh sửa 1 màu, 1 khổ size
          
            if (searchLookUpEdit4.EditValue == null)
            {
                XtraMessageBox.Show("Vui lòng chọn vật tư!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
           
            if (string.IsNullOrWhiteSpace(txtTenVatTu.Text))
            {
                XtraMessageBox.Show("Vui lòng nhập tên vật tư!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //Sửa dòng 
            if (isAdd != 0 && arrMau.Length == 0 && arrKhoSize.Length == 0)
            {
                string _mau = searchLookUpEdit1.EditValue.ToString();
                string _KhoSize = searchLookUpEdit2.EditValue.ToString();

                string _temmau = tbMau.Select($"MaMauKH = '{_mau}'").FirstOrDefault()?["TenMauKH"]?.ToString();
                string _tenKhoSize = tbKhoSize.Select($"MaKhoSize = '{_KhoSize}'").FirstOrDefault()?["KhoSize"]?.ToString();
                
                VatTuEntity vattu = new VatTuEntity
                {
                    ID = idadd, // Chỉnh sửa dòng hiện tại
                    MaVTMau = ReplaceSpecialCharactersAndRemoveSpaces(RemoveVietnameseTone(txtMaVatTu.Text).Replace(" ", string.Empty))+random.Next(0,999)+ DateTime.Now.ToString("yyyy_MM_dd_HHmmss"),
                    MaKeToan = RemoveVietnameseTone(txtMaKeToan.Text).Replace(" ", string.Empty),
                    MaVT = RemoveVietnameseTone(searchLookUpEdit4.EditValue.ToString()).Replace(" ", string.Empty),
                    TenVT = txtTenVatTu.Text,
                    CodeVT = RemoveVietnameseTone(txtCodeVatTu.Text).Replace(" ", string.Empty),
                    MaMau = _mau,
                    Mau = _temmau,
                    CodeMau = RemoveVietnameseTone(txtCodeMau.Text).Replace(" ", string.Empty),
                    ItemCode = txtItemCode.Text,
                    MaSizeKho = _KhoSize,
                    SizeKho = _tenKhoSize,
                    TenGD = txtTenGD.Text,
                    MaKho = RemoveVietnameseTone(txtMaKho.Text).Replace(" ", string.Empty),
                    MaDVTinh = addValueDVCL(txtDVTinh.Text),
                    DVTinh = txtDVTinh.Text,
                    DV1 = float.Parse("0.9144"),
                    DV2 = float.Parse("1.09361"),
                    MaNhomVT = addValueNhomNPL(txtMaNhomVT.Text),
                    NhaCungCap = txtNhaCungCap.Text,
                    MaVTHaiQuan = RemoveVietnameseTone(txtMaHaiQuan.Text).Replace(" ", string.Empty),
                    TenHaiQuan = txtTenHaiQuan.Text,
                    XuatXu = txtXuatXu.Text,
                    SLTonToiThieu = float.TryParse(txtSLTonToiThieu.Text, out float slTonToiThieu) ? slTonToiThieu : 0,
                    SLTonToiDa = float.TryParse(txtSLTonToiDa.Text, out float slTonToiDa) ? slTonToiDa : 0,
                    TKVT = txtTKVT.Text,
                    TKGiaVon = txtTKGiaVon.Text,
                    TKDoanhThu = txtTKDoanhThu.Text,
                    TKHangBanTraLai = txtTKHangBanTraLai.Text,
                    ThueSuat = float.TryParse(txtThueSuat.Text.Replace("%", string.Empty), out float thueSuat) ? thueSuat : 0,
                    GhiChu = txtGhiChu.Text,
                    IsNL = _isNL,
                    IsPL = !_isNL
                };

                List<VatTuEntity> lstPost = new List<VatTuEntity> { vattu };
                string urlPostNL = string.Format("{0}?", URL + "KhoVatTu/Post");
                string msResult = await _clientExtension.PostAsync(urlPostNL, lstPost);

                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                }
                else
                {
                    XtraMessageBox.Show(msResult);
                    _isPost = false;
                    return;
                }

                // Load lại dữ liệu
                _isPost = false;
                LoadDSNguyenLieu(false);
                LoadDSPhuLieu(false);
                setTxtEdit(true);
                SetSearchLookUpEditPopupState(searchLookUpEdit1, false);
                SetSearchLookUpEditPopupState(searchLookUpEdit2, false);
                SetSearchLookUpEditPopupState(searchLookUpEdit4, false);          
                Init();
                return;
            }

            // Trường hợp thêm mới (nhiều màu hoặc nhiều khổ size)
            int index = 0;
            // Thêm mới các dòng
            foreach (string _KhoSize in arrKhoSize)
            {
                index = 0;
                if (arrMau.Length==0)
                {
                    string _mamau = "";
                    string _maKhoSize = tbKhoSize.Select($"KhoSize = '{_KhoSize}'").FirstOrDefault()?["MaKhoSize"]?.ToString();

                    VatTuEntity vattu = new VatTuEntity
                    {
                        ID = 0, // Thêm mới
                        MaVTMau = ReplaceSpecialCharactersAndRemoveSpaces(RemoveVietnameseTone(txtCodeVatTu.Text).Replace(" ", string.Empty)) + random.Next(0, 999) + DateTime.Now.ToString("yyyy_MM_dd_HHmmss"),
                        MaKeToan = RemoveVietnameseTone(txtMaKeToan.Text).Replace(" ", string.Empty),
                        MaVT = RemoveVietnameseTone(searchLookUpEdit4.EditValue.ToString()).Replace(" ", string.Empty),
                        TenVT = txtTenVatTu.Text,
                        CodeVT = RemoveVietnameseTone(txtCodeVatTu.Text).Replace(" ", string.Empty),
                        MaMau = _mamau,
                        Mau = "",
                        CodeMau = "",
                        ItemCode = txtItemCode.Text,
                        MaSizeKho = _maKhoSize,
                        SizeKho = _KhoSize,
                        TenGD = txtTenGD.Text,
                        MaKho = RemoveVietnameseTone(txtMaKho.Text).Replace(" ", string.Empty),
                        MaDVTinh = addValueDVCL(txtDVTinh.Text),
                        DVTinh = txtDVTinh.Text,
                        DV1 = float.Parse("0.9144"),
                        DV2 = float.Parse("1.09361"),
                        MaNhomVT = addValueNhomNPL(txtMaNhomVT.Text),
                        NhaCungCap = txtNhaCungCap.Text,
                        MaVTHaiQuan = RemoveVietnameseTone(txtMaHaiQuan.Text).Replace(" ", string.Empty),
                        TenHaiQuan = txtTenHaiQuan.Text,
                        XuatXu = txtXuatXu.Text,
                        SLTonToiThieu = float.TryParse(txtSLTonToiThieu.Text, out float slTonToiThieu) ? slTonToiThieu : 0,
                        SLTonToiDa = float.TryParse(txtSLTonToiDa.Text, out float slTonToiDa) ? slTonToiDa : 0,
                        TKVT = txtTKVT.Text,
                        TKGiaVon = txtTKGiaVon.Text,
                        TKDoanhThu = txtTKDoanhThu.Text,
                        TKHangBanTraLai = txtTKHangBanTraLai.Text,
                        ThueSuat = float.TryParse(txtThueSuat.Text.Replace("%", string.Empty), out float thueSuat) ? thueSuat : 0,
                        GhiChu = txtGhiChu.Text,
                        IsNL = _isNL,
                        IsPL = !_isNL
                    };
                    if (!checkDuplicate(vattu))
                    {
                        XtraMessageBox.Show("Vật tư bị trùng, vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    List<VatTuEntity> lstPost = new List<VatTuEntity> { vattu };
                    string urlPostNL = string.Format("{0}?", URL + "KhoVatTu/Post");
                    string msResult = await _clientExtension.PostAsync(urlPostNL, lstPost);

                    if (msResult.ToLower() == "true")
                    {
                        _isPost = false;
                        clsWaitForm.ShowSuccessForm(this, 2000);
                    }
                    else
                    {
                        _isPost = false;
                        XtraMessageBox.Show(msResult);
                        return;
                    }
                }
                else
                { 
                    foreach (string _mau in arrMau)
                    {

                        string _mamau = tbMau.Select($"TenMauKH = '{_mau}'").FirstOrDefault()?["MaMauKH"]?.ToString();
                        string _maKhoSize = tbKhoSize.Select($"KhoSize = '{_KhoSize}'").FirstOrDefault()?["MaKhoSize"]?.ToString();

                        VatTuEntity vattu = new VatTuEntity
                        {
                            ID = 0, // Thêm mới
                            MaVTMau = ReplaceSpecialCharactersAndRemoveSpaces(RemoveVietnameseTone(txtCodeVatTu.Text).Replace(" ", string.Empty)) + random.Next(0, 999) + DateTime.Now.ToString("yyyy_MM_dd_HHmmss"),
                            MaKeToan = RemoveVietnameseTone(txtMaKeToan.Text).Replace(" ", string.Empty),
                            MaVT = RemoveVietnameseTone(searchLookUpEdit4.EditValue.ToString()).Replace(" ", string.Empty),
                            TenVT = txtTenVatTu.Text,
                            CodeVT = RemoveVietnameseTone(txtCodeVatTu.Text).Replace(" ", string.Empty),
                            MaMau = _mamau,
                            Mau = _mau,
                            CodeMau = arrCodeMau[index],
                            ItemCode = txtItemCode.Text,
                            MaSizeKho = _maKhoSize,
                            SizeKho = _KhoSize,
                            TenGD = txtTenGD.Text,
                            MaKho = RemoveVietnameseTone(txtMaKho.Text).Replace(" ", string.Empty),
                            MaDVTinh = addValueDVCL(txtDVTinh.Text),
                            DVTinh = txtDVTinh.Text,
                            DV1 = float.Parse("0.9144"),
                            DV2 = float.Parse("1.09361"),
                            MaNhomVT = addValueNhomNPL(txtMaNhomVT.Text),
                            NhaCungCap = txtNhaCungCap.Text,
                            MaVTHaiQuan = RemoveVietnameseTone(txtMaHaiQuan.Text).Replace(" ", string.Empty),
                            TenHaiQuan = txtTenHaiQuan.Text,
                            XuatXu = txtXuatXu.Text,
                            SLTonToiThieu = float.TryParse(txtSLTonToiThieu.Text, out float slTonToiThieu) ? slTonToiThieu : 0,
                            SLTonToiDa = float.TryParse(txtSLTonToiDa.Text, out float slTonToiDa) ? slTonToiDa : 0,
                            TKVT = txtTKVT.Text,
                            TKGiaVon = txtTKGiaVon.Text,
                            TKDoanhThu = txtTKDoanhThu.Text,
                            TKHangBanTraLai = txtTKHangBanTraLai.Text,
                            ThueSuat = float.TryParse(txtThueSuat.Text.Replace("%", string.Empty), out float thueSuat) ? thueSuat : 0,
                            GhiChu = txtGhiChu.Text,
                            IsNL = _isNL,
                            IsPL = !_isNL
                        };
                        if (!checkDuplicate(vattu))
                        {
                            XtraMessageBox.Show("Vật tư bị trùng, vui lòng kiểm tra lại!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        List<VatTuEntity> lstPost = new List<VatTuEntity> { vattu };
                        string urlPostNL = string.Format("{0}?", URL + "KhoVatTu/Post");
                        string msResult = await _clientExtension.PostAsync(urlPostNL, lstPost);

                        if (msResult.ToLower() == "true")
                        {
                            _isPost = false;
                            clsWaitForm.ShowSuccessForm(this, 2000);
                        }
                        else
                        {
                            _isPost = false;
                            XtraMessageBox.Show(msResult);
                            return;
                        }
                      
                    }
                    index++;
                }
               
                
            }

            // Load lại dữ liệu
            _isPost = false;
            LoadDSPhuLieu(false);
            LoadDSNguyenLieu(false);
           
            setTxtEdit(true);
            SetSearchLookUpEditPopupState(searchLookUpEdit1, false);
            SetSearchLookUpEditPopupState(searchLookUpEdit2, false);
           SetSearchLookUpEditPopupState(searchLookUpEdit4, false);
       

            // cuộn xuống chỗ đó
            Init();
          /*  if (_isNL)
            {
                int lastRowHandleNow = gVNguyenLieu.RowCount - 1;
                if (lastRowHandleNow >= 0)
                {
                    gVNguyenLieu.FocusedRowHandle = lastRowHandleNow;
                    gVNguyenLieu.MakeRowVisible(lastRowHandleNow);
                }

            }
            else
            {
                int lastRowHandleNow = gVPhuLieu.RowCount - 1;
                if (lastRowHandleNow >= 0)
                {
                    gVPhuLieu.FocusedRowHandle = lastRowHandleNow;
                    gVPhuLieu.MakeRowVisible(lastRowHandleNow);
                }

            }
           */
        }


        private void btnXuatMauExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("TemplateNguyenLieu{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateVatTu.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                Export(TemplateFileName, ExportFileName);

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
        public void Export(string TemplateFileName, string ExportFileName)
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
                        excelPackage.Workbook.Properties.Title = "NguyenLieu";

                        // Đường dẫn tới file Excel mẫu
                        string templateFilePath = TemplateFileName;

                        // Đường dẫn tới file kết quả (file Excel chứa dữ liệu đã được xuất)
                        string resultFilePath = ExportFileName;

                        // Mở file Excel mẫu
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);


                        FileInfo resultFile = new FileInfo(resultFilePath);
                        templatePackage.SaveAs(resultFile);
                    }
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                object misValue = System.Reflection.Missing.Value;
                throw new Exception(ex.Message);
            }


        }
        private void LoadNhomNPL(string NhomNPL = "")
        {
            try
            {
                // Định nghĩa URL API
                string url = string.Format("{0}?", URL + "KhoNPL/GetNhomNPL");

                // Gọi API để lấy danh sách NhomNPLEntity
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                // Kiểm tra nếu API trả về dữ liệu
                if (!string.IsNullOrEmpty(json) && json != "[]")
                {
                    // Deserialize JSON thành danh sách NhomNPLEntity
                    listNhomNPL = JsonConvert.DeserializeObject<List<NhomNguyenPhuLieuEntity>>(json);
                    // Xóa sạch các mục hiện tại trong combo box
                    txtMaNhomVT.Properties.Items.Clear();

                    // Thêm từng mục vào combo box
                    foreach (var item in listNhomNPL)
                    {
                        txtMaNhomVT.Properties.Items.Add(item.TenNhom.ToString());

                    }

                    // Tìm phần tử có ID bằng với ID truyền vào
                    NhomNguyenPhuLieuEntity nhomNPL = listNhomNPL.FirstOrDefault(n => n.TenNhom == NhomNPL);

                    // Kiểm tra nếu tìm thấy phần tử
                    if (nhomNPL != null)
                    {
                        // Gán giá trị cho txtMaNhom và txtTenNhom
                        txtMaNhomVT.SelectedItem = nhomNPL.TenNhom.ToString();

                    }
                    else
                    {
                        if (listNhomNPL != null && listNhomNPL.Count > 0)
                        {
                            var firstNhom = listNhomNPL[0]; // Lấy phần tử đầu tiên
                            txtMaNhomVT.SelectedItem = firstNhom.TenNhom.ToString(); // Gán ID của phần tử đầu tiên

                        }
                        else
                        {
                            // Nếu danh sách trống, set giá trị mặc định trống
                            txtMaNhomVT.SelectedItem = null;

                        }
                    }
                }
                else
                {
                    // Nếu API không trả về dữ liệu, gán giá trị mặc định
                    txtMaNhomVT.SelectedItem = "";

                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu xảy ra
                XtraMessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu nhóm NPL. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadDVCL(string donvitinh = "")
        {
            try
            {
                // Định nghĩa URL API
                string url = string.Format("{0}?", URL + "DonViChungLoai/Get");

                // Gọi API để lấy danh sách NhomNPLEntity
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                // Kiểm tra nếu API trả về dữ liệu
                if (!string.IsNullOrEmpty(json) && json != "[]")
                {
                    // Deserialize JSON thành danh sách NhomNPLEntity
                    listDVT = JsonConvert.DeserializeObject<List<DonViChungLoaiEntity>>(json);
                    // Xóa sạch các mục hiện tại trong combo box
                    txtDVTinh.Properties.Items.Clear();

                    // Thêm từng mục vào combo box
                    foreach (var item in listDVT)
                    {
                        txtDVTinh.Properties.Items.Add(item.TenDVCL.ToString());

                    }

                    // Tìm phần tử có ID bằng với ID truyền vào
                    DonViChungLoaiEntity nhomNPL = listDVT.FirstOrDefault(n => n.MaDVCL == donvitinh);

                    // Kiểm tra nếu tìm thấy phần tử
                    if (nhomNPL != null)
                    {
                        // Gán giá trị cho txtMaNhom và txtTenNhom
                        txtDVTinh.SelectedItem = nhomNPL.ID.ToString();

                    }
                    else
                    {
                        if (listDVT != null && listDVT.Count > 0)
                        {
                            var firstNhom = listDVT[0]; // Lấy phần tử đầu tiên
                            txtDVTinh.SelectedItem = firstNhom.TenDVCL.ToString(); // Gán ID của phần tử đầu tiên

                        }
                        else
                        {
                            // Nếu danh sách trống, set giá trị mặc định trống
                            txtDVTinh.SelectedItem = null;

                        }
                    }
                }
                else
                {
                    // Nếu API không trả về dữ liệu, gán giá trị mặc định
                    txtDVTinh.SelectedItem = "";

                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu xảy ra
                XtraMessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu nhóm NPL. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadisNL()
        {
            txtLoaiVatTu.Properties.Items.Clear();

            // Thêm các item vào ComboBoxEdit
            txtLoaiVatTu.Properties.Items.Add("Nguyên liệu");
            txtLoaiVatTu.Properties.Items.Add("Phụ liệu");

            XtraTabPage activeTab = xtraTabControl1.SelectedTabPage;
            bool _isNLorPL = activeTab.Text == "Nguyên liệu";
            // Set SelectedItem dựa trên giá trị của _isNL
            if (_isNLorPL)
            {
                txtLoaiVatTu.SelectedItem = "Nguyên liệu";
            }
            else
            {
                txtLoaiVatTu.SelectedItem = "Phụ liệu";
            }
        }
      
        private void gVPhuLieu_Click(object sender, EventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            if (view != null)
            {

                int rowHandle = view.FocusedRowHandle;


                if (rowHandle >= 0)
                {
                    if (!_isPost)
                    {
                        Them.Enabled = true;
                        Sua.Enabled = true;
                        Xoa.Enabled = true;
                        Luu.Enabled = false;
                    }

                    int idValue = int.Parse(view.GetRowCellValue(rowHandle, "ID").ToString());
                    if (!_isPost)
                    { loadThongTinPhuLieu(idValue); }


                }
            }
        }


        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            var selectedTabPage = e.Page;
            _isPost = false;
            if (selectedTabPage.Name == "tabNguyenLieu") 
            {
                searchKhoSize.Text = "Size/Khổ";
                LoadDSNguyenLieu(false);
                _isNL = true;
                if (listNguyenlieuEntity.Count > 0)
                {
                    if (!_isPost)
                    { loadThongTinNguyenLieu(listNguyenlieuEntity[0].ID); }
                }
                else
                {
                    if (!_isPost)
                    { setTxtNull(); }

                }
            }
            else if (selectedTabPage.Name == "tabPhuLieu") 
            {
                LoadDSPhuLieu(false);
                searchKhoSize.Text = "Size";
                _isNL = false;
                if (listPhulieuEntity.Count > 0)

                {
                    if (!_isPost)
                    { loadThongTinPhuLieu(listPhulieuEntity[0].ID); }
                }
                else
                {
                    if (!_isPost)
                    { setTxtNull(); }

                }
            }
        }

        private void txtLoaiVatTu_Properties_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEdit comboBoxEdit = sender as ComboBoxEdit;

            // Kiểm tra item được chọn
            if (comboBoxEdit.SelectedItem != null)
            {
                string selectedItem = comboBoxEdit.SelectedItem.ToString();

                // Cập nhật giá trị _isNL dựa trên lựa chọn
                if (selectedItem == "Nguyên liệu")
                {
                    _isNL = true;
                }
                else if (selectedItem == "Phụ liệu")
                {
                    _isNL = false;
                }


            }
        }
  
        private void gVNguyenLieu_CustomDrawColumnHeader_1(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void gVPhuLieu_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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
        
        private void txtThueSuat_Properties_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void txtSLTonToiThieu_Properties_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void txtSLTonToiDa_Properties_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void txtDV1_Properties_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void txtDV2_Properties_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void txtCodeVatTu_Properties_Validating(object sender, CancelEventArgs e)
        {
            if (_isPost)
            {
                string pattern = @"^[a-zA-Z0-9]*$";
                if (!string.IsNullOrWhiteSpace(txtCodeVatTu.Text) && !Regex.IsMatch(txtCodeVatTu.Text, pattern))
                {
                    toolTip.ToolTipTitle = "Lỗi nhập liệu";
                    toolTip.ToolTipIcon = ToolTipIcon.Warning;
                    toolTip.Show("Không thể nhập ký tự đặc biệt, tiếng Việt! Vui lòng sửa lỗi.", txtCodeVatTu, txtCodeVatTu.Width, txtCodeVatTu.Height, 2000);
                    e.Cancel = true;
                }
                else
                {

                    toolTip.Hide(txtCodeVatTu);
                }
            }

        }

        private void txtMaKho_Properties_Validating(object sender, CancelEventArgs e)
        {
            if (_isPost)
            {
                string pattern = @"^[a-zA-Z0-9_]*$";
                if (!string.IsNullOrWhiteSpace(txtMaKho.Text) && !Regex.IsMatch(txtMaKho.Text, pattern))
                {

                    toolTip.ToolTipTitle = "Lỗi nhập liệu";
                    toolTip.ToolTipIcon = ToolTipIcon.Warning;
                    toolTip.Show("Không thể nhập ký tự đặc biệt, tiếng Việt! Vui lòng sửa lỗi.", txtMaKho, txtMaKho.Width, txtMaKho.Height, 2000);

                    e.Cancel = true;
                }
                else
                {
                    toolTip.Hide(txtMaKho);
                }
            }

        }

        private void txtCodeMau_Properties_Validating(object sender, CancelEventArgs e)
        {
        }

        private void txtItemCode_Properties_Validating(object sender, CancelEventArgs e)
        {
            if (_isPost)
            {
                string pattern = @"^[a-zA-Z0-9_]*$";
                if (!string.IsNullOrWhiteSpace(txtItemCode.Text) && !Regex.IsMatch(txtItemCode.Text, pattern))
                {

                    toolTip.ToolTipTitle = "Lỗi nhập liệu";
                    toolTip.ToolTipIcon = ToolTipIcon.Warning;
                    toolTip.Show("Không thể nhập ký tự đặc biệt, tiếng Việt! Vui lòng sửa lỗi.", txtItemCode, txtItemCode.Width, txtItemCode.Height, 2000);

                    e.Cancel = true;
                }
                else
                {
                    toolTip.Hide(txtItemCode);
                }
            }

        }

        private void txtMaHaiQuan_Properties_Validating(object sender, CancelEventArgs e)
        {
            if (_isPost)
            {
                string pattern = @"^[a-zA-Z0-9_]*$";
                if (!string.IsNullOrWhiteSpace(txtMaHaiQuan.Text) && !Regex.IsMatch(txtMaHaiQuan.Text, pattern))
                {

                    toolTip.ToolTipTitle = "Lỗi nhập liệu";
                    toolTip.ToolTipIcon = ToolTipIcon.Warning;
                    toolTip.Show("Không thể nhập ký tự đặc biệt, tiếng Việt! Vui lòng sửa lỗi.", txtMaHaiQuan, txtMaHaiQuan.Width, txtMaHaiQuan.Height, 2000);

                    e.Cancel = true;
                }
                else
                {
                    toolTip.Hide(txtMaHaiQuan);
                }
            }

        }

       
     
        private void gVNguyenLieu_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void gVPhuLieu_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }
     
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            //// var selectedValue = searchLookUpEdit1.EditValue; // Lấy giá trị hiện tại
            //if (searchLookUpEdit1.EditValue is DataRowView)
            //{
            //    gridCheckMarksMau.Selection.Add(((DataRowView)searchLookUpEdit1.EditValue));
            //}

            //StringBuilder sb = new StringBuilder();
            //foreach (DataRowView rv in gridCheckMarksMau.Selection)
            //{
            //    if (sb.ToString().Length > 0) { sb.Append(";"); }
            //    sb.Append(rv["TenMauKH"].ToString());
            //}
            //searchLookUpEdit1.Text = sb.ToString();

        }

        private void searchLookUpEdit1_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            listMau = "";
            if (searchLookUpEdit1.EditValue is DataRowView)
            {
                gridCheckMarksMau.Selection.Add(((DataRowView)searchLookUpEdit1.EditValue));
                //listMau += searchLookUpEdit1.EditValue.ToString()+";";
            }
            if (selectedValuesColor.ToString() == "") return;
            StringBuilder sb = new StringBuilder();
            foreach (DataRowView rv in gridCheckMarksMau.Selection)
            {
                if (sb.ToString().Length > 0) { sb.Append(";"); }
                sb.Append(rv["TenMauKH"].ToString());
               
            }
            if (sb.ToString() != "")
            {
                listMau = sb.ToString();
                e.DisplayText = sb.ToString();
            }    
               


            StringBuilder sbCodeMau = new StringBuilder();
            foreach (DataRowView rv in gridCheckMarksMau.Selection)
            {
                if (sbCodeMau.Length > 0) sbCodeMau.Append("; "); // Thêm dấu phân cách
                sbCodeMau.Append(rv["CodeMauKH"].ToString()); // Lấy Mã Màu
            }

            // Gán giá trị tổng hợp vào txtCodeMau
            txtCodeMau.Text = sbCodeMau.ToString();
        }

        private void searchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

            //string selectedValuesColor = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEdit1.Properties.ValueMember)));
            //searchLookUpEdit1.EditValue = selectedValuesColor;
            //if (searchLookUpEdit1.EditValue is null) return;
            //colorID = searchLookUpEdit1.EditValue.ToString();
        }

        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {
        }

        private void searchLookUpEdit2_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
           
            listKhoSize = "";
            if (searchLookUpEdit2.EditValue is DataRowView)
            {
                gridCheckMarksKhoSize.Selection.Add(((DataRowView)searchLookUpEdit2.EditValue));
                //listKhoSize += searchLookUpEdit2.EditValue.ToString() + ";";
            }
            if (selectedValuesKhoSize.ToString() == "") return;
            StringBuilder sb = new StringBuilder();
            foreach (DataRowView rv in gridCheckMarksKhoSize.Selection)
            {
                if (sb.ToString().Length > 0) { sb.Append(";"); }
                sb.Append(rv["KhoSize"].ToString());
                
            }
            if (sb.ToString() != "")
            {
                listKhoSize += sb;
                e.DisplayText = sb.ToString();
            }    
               
        }

        private void searchLookUpEdit2View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
           /* selectedValuesKhoSize = string.Join(";", searchLookUpEdit2View.GetSelectedRows().Select(rowHandle => searchLookUpEdit2View.GetRowCellValue(rowHandle, searchLookUpEdit2.Properties.ValueMember)));
            searchLookUpEdit2.EditValue = selectedValuesKhoSize;
            if (searchLookUpEdit2.EditValue is null) return;
            KhoSizeID = searchLookUpEdit2.EditValue.ToString();*/
        }

   
        private void searchLookUpEdit4View_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void searchLookUpEdit4_EditValueChanged(object sender, EventArgs e)
        {
            var selectedValue = searchLookUpEdit4.EditValue;
            if (selectedValue != null)
            {

                var selectedRow = searchLookUpEdit4.Properties.View.GetFocusedDataRow();

                if (selectedRow != null)
                {

                    txtTenVatTu.Text = selectedRow["TenVT"].ToString();
                }
            }
        }

        #region hàm phụ
        public string RemoveVietnameseTone(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            string result = text.Trim().ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ", "y");
            result = Regex.Replace(result, "đ", "d");
            result = result.ToUpper();
            return result;
        }

        private string addValueDVCL(string tenDVCL)
        {
            try
            {
                if (tenDVCL == "") return "";
                string url = string.Format("{0}?", URL + "DonViChungLoai/Get");


                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;


                if (!string.IsNullOrEmpty(json) && json != "[]")
                {

                    listDVT = JsonConvert.DeserializeObject<List<DonViChungLoaiEntity>>(json);
                    var isExist = listDVT.Any(n => RemoveVietnameseTone(n.TenDVCL).Replace(" ", string.Empty) == RemoveVietnameseTone(tenDVCL).Replace(" ", string.Empty));
                    if (!isExist)
                    {
                        // Nếu không tồn tại, tạo mới đối tượng DonViChungLoaiEntity
                        DonViChungLoaiEntity DVCK = new DonViChungLoaiEntity
                        {
                            ID = 0,
                            MaDVCL = "",
                            TenDVCL = tenDVCL,
                            GhiChu = ""

                        };

                        List<DonViChungLoaiEntity> _lstPost = new List<DonViChungLoaiEntity> { DVCK };

                        string urlPost = string.Format("{0}", URL + "DonViChungLoai/Post");


                        // Gửi dữ liệu thông qua API Post
                        var postResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlPost, _lstPost); }).Result;
                    }

                }
                listDVT = reloadDVCL();
                DonViChungLoaiEntity result = listDVT.FirstOrDefault(n => RemoveVietnameseTone(n.TenDVCL).Replace(" ", string.Empty) == RemoveVietnameseTone(tenDVCL).Replace(" ", string.Empty));
                return result.MaDVCL;

            }
            catch (Exception ex)
            {
                return "";
                XtraMessageBox.Show("Đã xảy ra lỗi khi cố gắng thêm dữ liệu vào Đơn Vị Tính. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string addValueNhomNPL(string tenNhomNPL = "")
        {
            try
            {
                if (tenNhomNPL == "") return "";
                //listNhomNPL.Any(item => RemoveVietnameseTone(item.TenNhom) == RemoveVietnameseTone(nhomNPLValue));
                // API URL để lấy danh sách Nhóm NPL
                string url = string.Format("{0}?", URL + "KhoNPL/GetNhomNPL");

                // Gọi API GET để lấy danh sách Nhóm Nguyên Phụ Liệu
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;


                // Kiểm tra xem dữ liệu có tồn tại không
                if (!string.IsNullOrEmpty(json) && json != "[]")
                {
                    // Deserialize JSON thành danh sách đối tượng
                    var listNPL = JsonConvert.DeserializeObject<List<NhomNguyenPhuLieuEntity>>(json);
                    listNhomNPL = JsonConvert.DeserializeObject<List<NhomNguyenPhuLieuEntity>>(json);
                    // Kiểm tra xem Nhóm NPL đã tồn tại hay chưa
                    var isExist = listNPL.Any(n => RemoveVietnameseTone(n.TenNhom).Replace(" ", string.Empty) == RemoveVietnameseTone(tenNhomNPL).Replace(" ", string.Empty));
                    if (!isExist)
                    {
                        // Nếu không tồn tại, tạo mới đối tượng NhomNguyenPhuLieuEntity
                        NhomNguyenPhuLieuEntity nhomNPL = new NhomNguyenPhuLieuEntity
                        {
                            ID = 0,
                            MaNhom = RemoveVietnameseTone(tenNhomNPL).Replace(" ", string.Empty),
                            TenNhom = tenNhomNPL
                        };

                        // Tạo danh sách để gửi API POST
                        List<NhomNguyenPhuLieuEntity> _lstPost = new List<NhomNguyenPhuLieuEntity> { nhomNPL };

                        // URL API để thêm Nhóm NPL mới
                        string urlPost = string.Format("{0}", URL + "KhoNPL/PostNNPL");

                        var postResult = Task.Run(async () => { return await _clientExtension.PostAsync(urlPost, _lstPost); }).Result;

                    }

                }
                listNhomNPL = reloadNhomNPL();
                NhomNguyenPhuLieuEntity result = listNhomNPL.FirstOrDefault(n => RemoveVietnameseTone(n.TenNhom).Replace(" ", string.Empty) == RemoveVietnameseTone(tenNhomNPL).Replace(" ", string.Empty));
                return result.MaNhom;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu xảy ra
                return "";
                XtraMessageBox.Show("Đã xảy ra lỗi khi cố gắng thêm dữ liệu vào Nhóm Nguyên Phụ Liệu. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void setTypeTxtEdit(object sender, KeyPressEventArgs e)
        {

            TextEdit textEdit = sender as TextEdit;


            string currentText = textEdit.Text;

            // Kiểm tra nếu ký tự không phải là số và cũng không phải dấu chấm
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ngăn không cho nhập ký tự không hợp lệ
                return;
            }

            // Kiểm tra nếu dấu chấm đã tồn tại
            if (currentText.Contains("."))
            {
                // Chỉ cho phép nhập 2 chữ số ở phần thập phân
                int indexOfDot = currentText.IndexOf('.');
                string decimalPart = currentText.Substring(indexOfDot + 1);

                // Nếu phần thập phân đã có 2 số thì ngăn không cho nhập thêm
                if (decimalPart.Length >= 2 && e.KeyChar != '\b') // '\b' là phím Backspace
                {
                    e.Handled = true;
                    return;
                }
            }

            // Kiểm tra nếu là dấu chấm và đã tồn tại trong chuỗi
            if (e.KeyChar == '.' && currentText.Contains("."))
            {
                e.Handled = true; // Ngăn không cho nhập thêm dấu chấm
            }
        }

        private void SetSearchLookUpEditPopupState(DevExpress.XtraEditors.SearchLookUpEdit searchLookUpEdit, bool allowPopup)
        {
            if (allowPopup)
            {
                searchLookUpEdit.Properties.QueryPopUp -= PreventPopup; 
            }
            else
            {
                // Ngăn không cho mở popup - Gán sự kiện QueryPopUp
                searchLookUpEdit.Properties.QueryPopUp -= PreventPopup; 
                searchLookUpEdit.Properties.QueryPopUp += PreventPopup; 
            }
        }

        // Hàm ngăn mở popup
        private void PreventPopup(object sender, CancelEventArgs e)
        {
            e.Cancel = true; // Hủy mở popup
        }
        private string ReplaceSpecialCharactersAndRemoveSpaces(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Loại bỏ khoảng trắng trước
            input = Regex.Replace(input, @"\s+", "");

            // Thay thế tất cả các ký tự không phải chữ cái và số bằng ký tự '_'
            string result = Regex.Replace(input, @"[^a-zA-Z0-9]", "_");
            return result;
        }




        private bool checkDuplicate(VatTuEntity vt)
        {
            string url = string.Format("{0}?", URL + "KhoVatTu/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return true;
            List<VatTuEntity> tbDup = JsonConvert.DeserializeObject<List<VatTuEntity>>(json);
            bool isDuplicate = tbDup.Any(item =>
            item.MaVT == vt.MaVT &&
            item.MaMau == vt.MaMau &&
            item.MaSizeKho == vt.MaSizeKho);
            return !isDuplicate;
        }

        private void setTxtNull()
        {
            txtID.Text = "";
            txtMaKeToan.Text = "";
            searchLookUpEdit4.EditValue = null;
            txtMaVatTu.Text = "";
            txtCodeVatTu.Text = "";
            txtTenVatTu.Text = "";
            txtMau.Text = "";
            txtCodeMau.Text = "";
            txtItemCode.Text = "";
            txtSizeKho.Text = "";
            txtTenGD.Text = "";
            txtMaKho.Text = "";
            txtMaNhomVT.Text = "";
            txtDVTinh.Text = "";
            txtDV1.Text = "";
            txtDV2.Text = "";
            txtNhaCungCap.Text = "";
            txtMaHaiQuan.Text = "";
            txtTenHaiQuan.Text = "";
            txtXuatXu.Text = "";
            txtLoaiVatTu.Text = "";
            txtSLTonToiThieu.Text = "";
            txtSLTonToiDa.Text = "";
            txtThueSuat.Text = "";
            txtGhiChu.Text = "";
            txtTKVT.Text = "";
            txtTKGiaVon.Text = "";
            txtTKDoanhThu.Text = "";
            txtTKHangBanTraLai.Text = "";
        }
        private void setTxtValue(VatTuEntity vt)
        {
            if (searchLookUpEdit1View != null && gridCheckMarksMau != null)
            {
                gridCheckMarksMau.ClearSelection(searchLookUpEdit1View);
                searchLookUpEdit1.EditValue = null;
                searchLookUpEdit1.Refresh();
            }
            if (searchLookUpEdit2View != null && gridCheckMarksKhoSize != null)
            {

                gridCheckMarksKhoSize.ClearSelection(searchLookUpEdit2View);
                searchLookUpEdit2.EditValue = null;
                searchLookUpEdit2.Refresh();
            }

            listDVT = reloadDVCL();
            listNhomNPL = reloadNhomNPL();
            NhomNguyenPhuLieuEntity result1 = null;
            DonViChungLoaiEntity result2 = null;
            if (listDVT != null)
            {
                result2 = listDVT.FirstOrDefault(n => RemoveVietnameseTone(n.TenDVCL).Replace(" ", string.Empty) == RemoveVietnameseTone(vt.DVTinh).Replace(" ", string.Empty));
            }
            if (listNhomNPL != null)
            {
                result1 = listNhomNPL.FirstOrDefault(n => RemoveVietnameseTone(n.MaNhom).Replace(" ", string.Empty) == RemoveVietnameseTone(vt.MaNhomVT).Replace(" ", string.Empty));
            }

            txtID.Text = vt.ID.ToString();
            txtMaKeToan.Text = vt.MaKeToan;
            txtMaVatTu.Text = vt.MaVT;
            searchLookUpEdit4.EditValue = vt.MaVT.ToString();
            txtCodeVatTu.Text = vt.CodeVT;
            txtTenVatTu.Text = vt.TenVT;
            searchLookUpEdit1.EditValue = vt.MaMau;
            searchLookUpEdit2.EditValue = vt.MaSizeKho;
            listMau = vt.Mau;
            listKhoSize = vt.SizeKho;
            txtCodeMau.Text = vt.CodeMau;
            txtItemCode.Text = vt.ItemCode;
            txtSizeKho.Text = vt.SizeKho;
            txtTenGD.Text = vt.TenGD;
            txtMaKho.Text = vt.MaKho;
            txtMaNhomVT.Text = result1 != null ? result1.TenNhom : "";
            txtDVTinh.Text = result2 != null ? result2.TenDVCL : "";
            txtDV1.Text = vt.DV1.ToString();
            txtDV2.Text = vt.DV2.ToString();
            txtNhaCungCap.Text = vt.NhaCungCap;
            txtMaHaiQuan.Text = vt.MaVTHaiQuan;
            txtTenHaiQuan.Text = vt.TenHaiQuan;
            txtXuatXu.Text = vt.XuatXu;

            txtLoaiVatTu.Properties.Items.Clear();
            txtLoaiVatTu.Properties.Items.Add("Nguyên liệu");
            txtLoaiVatTu.Properties.Items.Add("Phụ liệu");
            if (bool.Parse(vt.IsNL.ToString()))
            {
                txtLoaiVatTu.SelectedItem = "Nguyên liệu";
            }
            else
            {
                txtLoaiVatTu.SelectedItem = "Phụ liệu";
            }
            txtSLTonToiThieu.Text = vt.SLTonToiThieu.ToString();
            txtSLTonToiDa.Text = vt.SLTonToiDa.ToString();
            txtThueSuat.Text = vt.ThueSuat.ToString() + "%";
            txtGhiChu.Text = vt.GhiChu;
            txtTKVT.Text = vt.TKVT;
            txtTKGiaVon.Text = vt.TKGiaVon;
            txtTKDoanhThu.Text = vt.TKDoanhThu;
            txtTKHangBanTraLai.Text = vt.TKHangBanTraLai;
            this.Refresh();
        }
        private void setTxtEdit(bool IsVi)
        {
            txtID.Properties.ReadOnly = IsVi;
            txtMaVatTu.Properties.ReadOnly = IsVi;
            txtMaKeToan.Properties.ReadOnly = IsVi;
            txtCodeVatTu.Properties.ReadOnly = IsVi;
            txtTenVatTu.Properties.ReadOnly = true;
            txtMau.Properties.ReadOnly = IsVi;
            txtCodeMau.Properties.ReadOnly = true;
            txtItemCode.Properties.ReadOnly = IsVi;
            txtSizeKho.Properties.ReadOnly = IsVi;
            txtTenGD.Properties.ReadOnly = IsVi;
            txtMaKho.Properties.ReadOnly = IsVi;
            txtMaNhomVT.Properties.ReadOnly = IsVi;
            txtDVTinh.Properties.ReadOnly = IsVi;
            txtDV1.Properties.ReadOnly = IsVi;
            txtDV2.Properties.ReadOnly = IsVi;
            txtNhaCungCap.Properties.ReadOnly = IsVi;
            txtMaHaiQuan.Properties.ReadOnly = IsVi;
            txtTenHaiQuan.Properties.ReadOnly = IsVi;
            txtXuatXu.Properties.ReadOnly = IsVi;
            txtLoaiVatTu.Properties.ReadOnly = IsVi;
            txtSLTonToiThieu.Properties.ReadOnly = IsVi;
            txtSLTonToiDa.Properties.ReadOnly = IsVi;
            txtThueSuat.Properties.ReadOnly = IsVi;
            txtGhiChu.Properties.ReadOnly = IsVi;
            txtTKVT.Properties.ReadOnly = IsVi;
            txtTKGiaVon.Properties.ReadOnly = IsVi;
            txtTKDoanhThu.Properties.ReadOnly = IsVi;
            txtTKHangBanTraLai.Properties.ReadOnly = IsVi;
        }
        private List<NhomNguyenPhuLieuEntity> reloadNhomNPL()
        {
            string urlNhomNPL = string.Format("{0}?", URL + "KhoNPL/GetNhomNPL");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNhomNPL); }).Result;
            List<NhomNguyenPhuLieuEntity> listNhomNPL = new List<NhomNguyenPhuLieuEntity>();
            if (!string.IsNullOrEmpty(json) && json != "[]")
            {
                listNhomNPL = JsonConvert.DeserializeObject<List<NhomNguyenPhuLieuEntity>>(json);
                return listNhomNPL;
            }

            return listNhomNPL;
        }
        private List<DonViChungLoaiEntity> reloadDVCL()
        {
            string urlNhomNPL = string.Format("{0}?", URL + "DonViChungLoai/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNhomNPL); }).Result;
            List<DonViChungLoaiEntity> listDVCL = new List<DonViChungLoaiEntity>();
            if (!string.IsNullOrEmpty(json) && json != "[]")
            {
                listDVCL = JsonConvert.DeserializeObject<List<DonViChungLoaiEntity>>(json);
                return listDVCL;
            }

            return listDVCL;
        }
        #endregion
    }
}