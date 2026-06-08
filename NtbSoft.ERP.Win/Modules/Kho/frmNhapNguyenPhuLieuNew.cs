using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmNhapNguyenPhuLieuNew : DevExpress.XtraEditors.XtraForm
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
        DataTable _tblImportExcel;
        int FocusedIndex = 0;
        KeyDownControlHandler keyDownControlHandler;
        DataTable tbNL;
        DataTable tbPL;
        DataTable tbMau;
        DataTable tbKhoSize;
        DataTable tbDVTinh;
        DataTable tbVTMau;
        bool _isPost = false;
        bool _isNL = true;
        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        private ToolTip toolTip = new ToolTip();
        private List<DataRow> rowsToHighlight = new List<DataRow>();//row trùng với vật tư có sẵn
        private List<DataRow> rowsToHighlightinTable = new List<DataRow>();
        private List<DataRow> rowsEdit = new List<DataRow>();
        private List<DataRow> rowsError = new List<DataRow>();
        private bool _isEditing = false; // Biến trạng thái để theo dõi chế độ

        SearchCheckSelection gridCheckMarksMau;
        SearchCheckSelection gridCheckMarksKhoSize;
        string selectedValuesKhoSize = "";
        string KhoSizeID = "";
        string selectedValuesColor = "";
        string colorID = "";
        string listKhoSize = "";
        string listMau = "";
        Random random = new Random();
        public frmNhapNguyenPhuLieuNew()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            listNguyenlieuEntity = new List<VatTuEntity>();
            listPhulieuEntity = new List<VatTuEntity>();
            listDVT = new List<DonViChungLoaiEntity>();
            listNhomNPL = new List<NhomNguyenPhuLieuEntity>();
            _tblImportExcel = new DataTable();
            tbNL = new DataTable();
            tbPL = new DataTable();
            tbMau = new DataTable();
            tbKhoSize = new DataTable();
            tbDVTinh = new DataTable();
            tbVTMau = new DataTable();
        }
        public frmNhapNguyenPhuLieuNew(DataTable tblNew)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            listNguyenlieuEntity = new List<VatTuEntity>();
            listPhulieuEntity = new List<VatTuEntity>();
            listDVT = new List<DonViChungLoaiEntity>();
            listNhomNPL = new List<NhomNguyenPhuLieuEntity>();
            _tblImportExcel = new DataTable();
            tbNL = new DataTable();
            tbPL = new DataTable();
            tbMau = new DataTable();
            tbKhoSize = new DataTable();
            tbVTMau = new DataTable();
            Init();
            SaveData(tblNew);
        }

        private void frmNguyenLieu_Load(object sender, EventArgs e)
        {

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
            //LoadDSNguyenLieu(false);
            //LoadDSPhuLieu(false);
            SetSearchLookUpEditPopupState(searchLookUpEdit1, false);
            SetSearchLookUpEditPopupState(searchLookUpEdit2, false);
            SetSearchLookUpEditPopupState(searchLookUpEdit3, false);
        }

        private void Init()
        {

            searchLookUpEdit1.Properties.ValueMember = "MaMauKH";
            searchLookUpEdit1.Properties.DisplayMember = "TenMauKH";
            searchLookUpEdit1.Properties.NullText = "[Chọn màu]";
            gridCheckMarksMau = new SearchCheckSelection(searchLookUpEdit1.Properties);
            gridCheckMarksMau.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEdit1_SelectionChanged);
            searchLookUpEdit1.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEditMau_CustomDisplayText);
            searchLookUpEdit1.Tag = gridCheckMarksMau;
            LoadMaMauKH();

            searchLookUpEdit2.Properties.ValueMember = "MaKhoSize";
            searchLookUpEdit2.Properties.DisplayMember = "KhoSize";
            searchLookUpEdit2.Properties.NullText = "[Chọn Khổ/Size]";
            gridCheckMarksKhoSize = new SearchCheckSelection(searchLookUpEdit2.Properties);
            gridCheckMarksKhoSize.SelectionChanged += new SearchCheckSelection.SelectionChangedEventHandler(searchLookUpEdit2_SelectionChanged);
            searchLookUpEdit2.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(searchLookUpEditKhoSize_CustomDisplayText);
            searchLookUpEdit2.Tag = gridCheckMarksKhoSize;
            LoadKhoSizeVT();

            searchLookUpEdit3.Properties.ValueMember = "MaVT";
            searchLookUpEdit3.Properties.DisplayMember = "MaVT";
            searchLookUpEdit3.Properties.NullText = "[Chọn Vật tư]";
            LoadVatTuMau();
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
                //searchLookUpEdit1.Properties.View.PopulateColumns();
                //searchLookUpEdit1.Properties.View.RefreshData();
                searchLookUpEdit3.RefreshEditValue();
                searchLookUpEdit3.Refresh();
                if (searchLookUpEdit3.Properties.View == null)
                {
                    Console.WriteLine("searchLookUpEdit3.Properties.View chưa được khởi tạo.");
                    return;
                }



                //searchLookUpEdit1.EditValue = "PUMACOWELLPantone 7541 C";

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



                //searchLookUpEdit1.EditValue = "PUMACOWELLPantone 7541 C";

            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi: " + ex.Message);
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
            btnLuu.Enabled = true;
        }

        private void GridViewUpdateStatus(ResourceURL.EventStatus status)
        {
            switch (status)
            {
                case ResourceURL.EventStatus.View:
                    gVNhapNguyenLieu.OptionsBehavior.Editable = false;

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
                    gVNhapNguyenLieu.OptionsBehavior.Editable = true;

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
                    gVNhapNguyenLieu.OptionsBehavior.Editable = true;

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
        private void LoadDSNguyenLieu(bool isFromSave)
        {
            try
            {
                _status = ResourceURL.EventStatus.View;
                string url = string.Format("{0}?", URL + "KhoVatTu/GetNL");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                List<VatTuEntity> _lstTemp = new List<VatTuEntity>();
                if (json == "[]")
                {
                    listNguyenlieuEntity = new List<VatTuEntity>();
                    //gCNguyenLieu.DataSource = listNguyenlieuEntity;
                    gCNhapNguyenLieu.DataSource = null;
                    gCNhapNguyenLieu.Refresh();
                    Xoa.Enabled = false;
                    Sua.Enabled = false;
                    btnXacNhanExcel.Enabled = false;
                    setTxtNull();

                    return;
                }
                if (!string.IsNullOrEmpty(json))
                {
                    tbNL = JsonConvert.DeserializeObject<DataTable>(json);

                    listNguyenlieuEntity = JsonConvert.DeserializeObject<List<VatTuEntity>>(json);
                    if (listNguyenlieuEntity.Count == 0)
                    {
                        gCNhapNguyenLieu.DataSource = null;
                        gCNhapNguyenLieu.Refresh();
                        return;
                    }
                    _lstTemp = listNguyenlieuEntity;
                    foreach (var vt in _lstTemp)
                    {
                        listDVT = reloadDVCL();
                        listNhomNPL = reloadNhomNPL();
                        NhomNguyenPhuLieuEntity result1 = listNhomNPL.FirstOrDefault(n => RemoveVietnameseTone(n.MaNhom).Replace(" ", string.Empty) == RemoveVietnameseTone(vt.MaNhomVT).Replace(" ", string.Empty));
                        DonViChungLoaiEntity result2 = listDVT.FirstOrDefault(n => RemoveVietnameseTone(n.MaDVCL).Replace(" ", string.Empty) == RemoveVietnameseTone(vt.DVTinh).Replace(" ", string.Empty));
                        vt.MaNhomVT = result1.TenNhom;
                        vt.DVTinh = result2.TenDVCL;
                    }


                    if (!_isPost && _isNL)
                    { loadThongTinNguyenLieu(); }

                    Xoa.Enabled = true;
                    Sua.Enabled = true;
                    btnXacNhanExcel.Enabled = true;
                }

                gCNhapNguyenLieu.DataSource = _lstTemp;

                GridViewUpdateStatus(_status);
                if (isFromSave && FocusedIndex >= 0)
                {
                    gVNhapNguyenLieu.FocusedRowHandle = FocusedIndex;
                    this.ActiveControl = gCNhapNguyenLieu;
                }
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi khi tải danh sách nguyên liệu.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void loadThongTinNguyenLieu()
        {
            if (gCNhapNguyenLieu.DataSource == null) return;
            DataRow id = gVNhapNguyenLieu.GetDataRow(gVNhapNguyenLieu.FocusedRowHandle) as DataRow;
            DataTable dataTable = _tblImportExcel;
            if (dataTable.Rows.Count == 0)
            {
                return;
            }
            //DataRow result = dataTable.AsEnumerable().FirstOrDefault(row => row.Field<int>("ID") == ID);
            if (id != null)
            {
                setTxtValue(id);
                setTxtEdit(true);
            }
            else
            {
                setTxtNull();
                setTxtEdit(true);
            }
        }


        private void ThemDong()
        {
            Init();
            _isEditing = false;
            gridCheckMarksMau.ClearSelection(searchLookUpEdit1View);
            searchLookUpEdit1.EditValue = null;
            searchLookUpEdit1.Refresh();

            gridCheckMarksKhoSize.ClearSelection(searchLookUpEdit2View);
            searchLookUpEdit2.EditValue = null;
            searchLookUpEdit2.Refresh();
            _isPost = true;
            SetSearchLookUpEditPopupState(searchLookUpEdit1, true);
            SetSearchLookUpEditPopupState(searchLookUpEdit2, true);
            SetSearchLookUpEditPopupState(searchLookUpEdit3, true);
            setTxtNull();
            setTxtEdit(false);
            Xoa.Enabled = false;
            Sua.Enabled = false;
            Them.Enabled = false;
            LoadNhomNPL();
            LoadDVCL();
            LoadisNL();
         
        }
        private void NapLaiDong()
        {
            Init();
            /* LoadDSKhachHang(false);
             _status = ResourceURL.EventStatus.View;
             GridViewUpdateStatus(_status);
             gridViewKhachHang.OptionsView.NewItemRowPosition = NewItemRowPosition.None;*/
            _isPost = false;
            //LoadDSNguyenLieu(false);
            //LoadDSPhuLieu(false);
            Them.Enabled = true;
            setTxtEdit(true);
            SetSearchLookUpEditPopupState(searchLookUpEdit1, false);
            SetSearchLookUpEditPopupState(searchLookUpEdit2, false);
            SetSearchLookUpEditPopupState(searchLookUpEdit3, false);
        }
        private void SuaDong()
        {
            reinit();
            _isEditing = true;
            SetSearchLookUpEditPopupState(searchLookUpEdit1, true);
            SetSearchLookUpEditPopupState(searchLookUpEdit2, true);
            SetSearchLookUpEditPopupState(searchLookUpEdit3, true);
            DataRow row = gVNhapNguyenLieu.GetDataRow(gVNhapNguyenLieu.FocusedRowHandle) as DataRow;
            if(searchLookUpEdit3.EditValue==null)
            {
                searchLookUpEdit3.EditValue = row["MaVT"].ToString();
            }    

            //LoadNhomNPL(row.MaNhomVT);
            //LoadDVCL(row.DVTinh);
            if(row==null)
            {
                return;
            }    


            _isPost = true;
            //NguyenLieuEntity row = gvNguyenLieu.GetRow(gvNguyenLieu.FocusedRowHandle) as NguyenLieuEntity;
            /* _status = ResourceURL.EventStatus.Edit;
             GridViewUpdateStatus(_status);
             focused(this.gridViewKhachHang);*/
            setTxtEdit(false);
            Sua.Enabled = false;
            Xoa.Enabled = false;
            btnLuu.Enabled = true;
            Them.Enabled = false;
            //txtLoaiVatTu.Properties.ReadOnly = true;
            LoadNhomNPL(row["MaNhomVT"]?.ToString() ?? "");
            LoadDVCL(row["DVTinh"]?.ToString() ?? "");
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

                    if (!_isPost)
                    { loadThongTinNguyenLieu(); }



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
                    int focusedRowHandle = gVNhapNguyenLieu.FocusedRowHandle;

                    // Kiểm tra xem có dòng nào được focus hay không
                    if (focusedRowHandle >= 0) // Nếu dòng lớn hơn hoặc bằng 0, nghĩa là có dòng đang được chọn
                    {
                        DataRow rowToDelete = gVNhapNguyenLieu.GetDataRow(focusedRowHandle);

                        // Kiểm tra xem dòng có khác null không
                        if (rowToDelete != null)
                        {
                            // Xóa dòng từ DataTable
                            _tblImportExcel.Rows.Remove(rowToDelete);

                            // Cập nhật DataSource cho GridView
                            gCNhapNguyenLieu.DataSource = _tblImportExcel;

                            // Gọi lại hàm refresh (nếu cần)
                            gCNhapNguyenLieu.RefreshDataSource();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(" Xóa đã xảy ra lỗi.Vui lòng kiểm tra lại và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void btnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImportExcel();
        }
        private void ImportExcel()
        {

            try
            {

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Excel File|*.xlsx;*.xls";
                openFileDialog1.Title = "Import Excel";
                openFileDialog1.Multiselect = false;
                DialogResult dialogResult = openFileDialog1.ShowDialog();
                if (dialogResult != DialogResult.OK) return;
                string pathExecel = openFileDialog1.FileName;
                if (string.IsNullOrEmpty(pathExecel)) return;
                string conString = "";
                string extension = System.IO.Path.GetExtension(pathExecel);
                switch (extension)
                {
                    case ".xls": //Excel 97-03
                        conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString, pathExecel);
                        //conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString, pathExecel);
                        break;
                    case ".xlsx": //Excel 07 or higher
                        conString = string.Format(System.Configuration.ConfigurationManager.ConnectionStrings["Excel07+ConString"].ConnectionString, pathExecel);
                        break;
                }
                using (OleDbConnection excel_con = new OleDbConnection(conString))
                {
                    if (excel_con.State == ConnectionState.Closed)
                    {
                        excel_con.Open();
                    }
                    //OleDbCommand _oleCmdSelect;
                    OleDbDataAdapter oleAdapter = new OleDbDataAdapter(); ;
                    DataTable sheets = GetSchemaTable(conString);


                    //OleDbDataAdapter _oleCmdSelect = new System.Data.OleDb.OleDbDataAdapter(
                    //                             @"SELECT * FROM [Sheet1$] ", excel_con);
                    OleDbDataAdapter _oleCmdSelect = new System.Data.OleDb.OleDbDataAdapter(
                                              @"SELECT * FROM [" + sheets.Rows[0]["TABLE_NAME"].ToString() + "] ", excel_con);
                    DataSet excelDataSet = new DataSet();
                    _oleCmdSelect.Fill(excelDataSet);
                    DataTable dt = new DataTable();
                    dt = excelDataSet.Tables[0];

                    DataTable dtSave = CreateTblSave();
                    for (int i = 1; i < dt.Rows.Count; i++)
                    {
                        bool hasData = false;
                        for (int j = 1; j < dt.Columns.Count; j++)
                        {
                            if (!string.IsNullOrWhiteSpace(dt.Rows[i][j]?.ToString()) || dt.Rows[i][j]?.ToString() != "")
                            {
                                hasData = true;
                                break;
                            }
                        }

                        if (!hasData)
                        {
                            continue;
                        }

                        var drnew = dtSave.NewRow();
                        drnew["ID"] = 0;
                        drnew["MaVT"] = dt.Rows[i][1].ToString();
                        drnew["TenVT"] = dt.Rows[i][3].ToString();
                        drnew["CodeVT"] = "NL" + dt.Rows[i][2].ToString();
                        drnew["Mau"] = dt.Rows[i][5].ToString();
                        drnew["CodeMau"] = dt.Rows[i][6].ToString();
                        drnew["ItemCode"] = dt.Rows[i][7].ToString();
                        drnew["SizeKho"] = dt.Rows[i][8].ToString();
                        drnew["TenGD"] = dt.Rows[i][9].ToString();
                        drnew["MaKho"] = dt.Rows[i][4].ToString();
                        drnew["DVTinh"] = dt.Rows[i][10].ToString();
                        drnew["DV1"] = dt.Rows[i][11].ToString() == "" ? 0 : float.Parse(dt.Rows[i][11].ToString());
                        drnew["DV2"] = dt.Rows[i][12].ToString() == "" ? 0 : float.Parse(dt.Rows[i][12].ToString());
                        drnew["MaNhomVT"] = dt.Rows[i][13].ToString();
                        drnew["NhaCungCap"] = dt.Rows[i][14].ToString();
                        drnew["MaVTHaiQuan"] = dt.Rows[i][15].ToString();
                        drnew["TenHaiQuan"] = dt.Rows[i][16].ToString();
                        drnew["XuatXu"] = dt.Rows[i][17].ToString();
                        drnew["SLTonToiThieu"] = dt.Rows[i][18].ToString() == "" ? 0 : float.Parse(dt.Rows[i][18].ToString());
                        drnew["SLTonToiDa"] = dt.Rows[i][19].ToString() == "" ? 0 : float.Parse(dt.Rows[i][19].ToString());
                        drnew["TKVT"] = dt.Rows[i][20].ToString();
                        drnew["TKGiaVon"] = dt.Rows[i][21].ToString();
                        drnew["TKDoanhThu"] = dt.Rows[i][22].ToString();
                        drnew["TKHangBanTraLai"] = dt.Rows[i][23].ToString();
                        drnew["ThueSuat"] = dt.Rows[i][24].ToString() == "" ? 0 : float.Parse(dt.Rows[i][24].ToString());
                        drnew["GhiChu"] = dt.Rows[i][25].ToString();
                        drnew["IsNL"] = dt.Rows[i][26].ToString().Trim() == "Nguyên liệu" ? true : false;
                        drnew["IsPL"] = dt.Rows[i][26].ToString().Trim() == "Nguyên liệu" ? false : true;
                        dtSave.Rows.Add(drnew);
                        string a = "";
                    }

                    string rs = SaveData(dtSave);
                    if (rs == "false")
                    {
                        return;
                    }
                }
                XtraMessageBox.Show("Đã nhập xong dữ liệu từ Excel", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                XtraMessageBox.Show("Đã xảy ra lỗi về kiểu dữ liệu trong file Excel. Vui lòng kiểm tra lại!" + ex, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }


        }
        private string SaveData(DataTable dtSave)
        {

            /* List<NhomNguyenPhuLieuEntity> listNhomNPL = reloadNhomNPL();
             List<DonViChungLoaiEntity> listDVCL = reloadDVCL();

             foreach (DataRow row in dtSave.Rows)
             {

                 string _nhomNPLValue = row["MaNhomVT"].ToString();
                 string _donvitinhValue = row["DVTinh"].ToString();
                 if(_nhomNPLValue == ""|| _donvitinhValue=="")
                 {
                     continue;
                 }
                 row["MaNhomVT"]=addValueNhomNPL(_nhomNPLValue);
                 row["DVTinh"]=addValueDVCL(_donvitinhValue);
                 listNhomNPL = reloadNhomNPL();
                 listDVCL = reloadDVCL();
             }*/
            if (!dtSave.Columns.Contains("IsEdit"))
            {
                dtSave.Columns.Add("IsEdit", typeof(int)); // Cột IsEdit kiểu int
            }
            // Gán giá trị mặc định là 0 cho tất cả các hàng
            foreach (DataRow row in dtSave.Rows)
            {
                if (row["IsEdit"] == DBNull.Value) // Đảm bảo không ghi đè giá trị cũ nếu cột đã tồn tại
                {
                    row["IsEdit"] = 0;
                }
            }
            if (dtSave.Rows.Count == 0)
            {
                XtraMessageBox.Show("File Excel rỗng! Vui lòng thử lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Xoa.Enabled = false;
                Sua.Enabled = false;
                btnXacNhanExcel.Enabled = false;
                return "false";
            }


            string url = string.Format("{0}?", URL + "KhoVatTu/GetNL");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<VatTuEntity> _lstTemp = new List<VatTuEntity>();
            if (!string.IsNullOrEmpty(json) || json != "[]")
            {
                tbNL = JsonConvert.DeserializeObject<DataTable>(json);
            }
            string urlPL = string.Format("{0}?", URL + "KhoVatTu/GetPL");
            string jsonPL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlPL); }).Result;
            List<VatTuEntity> _lstTempPL = new List<VatTuEntity>();
            if (!string.IsNullOrEmpty(jsonPL) || jsonPL != "[]")
            {
                tbPL = JsonConvert.DeserializeObject<DataTable>(jsonPL);

            }
            tbNL.Merge(tbPL);
            HighlightRows(dtSave, tbNL);
            HighlightRowsInTable(dtSave);
            /* string url = string.Format("{0}?", URL + "KhoVatTu/Post");
             string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;*/
            string urlMau = $"{URL}ThuVienMau/Get";
            string jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
            string urlKhoSize = $"{URL}ThuVienKhoSize/Get";
            string jsonKhoSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKhoSize); }).Result;
            string urlDVT = $"{URL}DonViChungLoai/Get";
            string jsonDVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDVT); }).Result;
            // Gán dữ liệu cho cột "Màu"
            foreach (DataRow row in dtSave.Rows)
            {
                bool isError = false;
                if (jsonMau != "[]")
                {
                    tbMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);
                    string _tenMau = tbMau.Select($"CodeMauKH = '{row["CodeMau"]}'").FirstOrDefault()?["TenMauKH"]?.ToString();
                    string _maMau = tbMau.Select($"CodeMauKH = '{row["CodeMau"]}'").FirstOrDefault()?["MaMauKH"]?.ToString();
                    if (string.IsNullOrEmpty(_tenMau) || string.IsNullOrEmpty(_maMau))
                    {
                        isError = true; // Đánh dấu dòng lỗi nếu không tìm thấy giá trị
                    }
                    row["Mau"] = _tenMau;
                    row["MaMau"] = _maMau;
                }
                if (jsonKhoSize != "[]")
                {
                    tbKhoSize = JsonConvert.DeserializeObject<DataTable>(jsonKhoSize);
                    string _maKhoSize = tbKhoSize.Select($"KhoSize = '{row["SizeKho"]}'").FirstOrDefault()?["MaKhoSize"]?.ToString();
                    if (string.IsNullOrEmpty(_maKhoSize))
                    {
                        isError = true; // Đánh dấu dòng lỗi nếu không tìm thấy giá trị
                    }
                    row["MaSizeKho"] = _maKhoSize;
                }
                if (jsonDVT != "[]")
                {
                    tbDVTinh = JsonConvert.DeserializeObject<DataTable>(jsonDVT);
                    string _maDVT = tbDVTinh.Select($"TenDVCL = '{row["DVTinh"]}'").FirstOrDefault()?["MaDVCL"]?.ToString();
                    if (string.IsNullOrEmpty(_maDVT))
                    {
                        isError = true; // Đánh dấu dòng lỗi nếu không tìm thấy giá trị
                    }
                    row["MaDVTinh"] = _maDVT;
                }
                if (isError)
                {
                    rowsError.Add(row);
                }
            }

            _tblImportExcel = dtSave;
            gCNhapNguyenLieu.DataSource = dtSave;
            return "true";
        }
        //so sánh bảng trong excel và bảng vật tư có sẵn 
        private void HighlightRows(DataTable dtSave, DataTable tbNL)
        {
            rowsToHighlight.Clear(); // Xóa danh sách trước khi thêm mới

            foreach (DataRow rowSave in dtSave.Rows)
            {
                string maVatTuValue = rowSave["MaVT"].ToString();
                string tenVatTuValue = rowSave["TenVT"].ToString();
                string codemauValue = rowSave["CodeMau"].ToString();

                // Kiểm tra từng dòng trong tbNL
                foreach (DataRow rowNL in tbNL.Rows)
                {
                    string maVatTuNLValue = rowNL["MaVT"].ToString();
                    string tenVatTuNLValue = rowNL["TenVT"].ToString();
                    string mauNLValue = rowNL["CodeMau"].ToString();

                    // So sánh các giá trị
                    if (maVatTuValue == maVatTuNLValue && tenVatTuValue == tenVatTuNLValue)
                    {
                        if (codemauValue == mauNLValue)
                        {
                            rowsToHighlight.Add(rowSave);
                            break; // Đã tìm thấy, không cần kiểm tra tiếp
                        }


                    }
                }
            }
        }
        //so sánh 1 row vừa chỉnh sữa với bản vật tư có sẵn
        private void Highlight(DataRow rowToCheck, DataTable comparisonTable)
        {
           
            // Lấy giá trị cụ thể từ rowToCheck
            string maVatTuValue = rowToCheck["MaVT"].ToString();
            string tenVatTuValue = rowToCheck["TenVT"].ToString();
            string mauValue = rowToCheck["Mau"].ToString();
            string khoSizeValue = rowToCheck["SizeKho"].ToString();

            // Kiểm tra từng dòng trong bảng so sánh
            foreach (DataRow rowComparison in comparisonTable.Rows)
            {
                string maVatTuComparisonValue = rowComparison["MaVT"].ToString();
                string tenVatTuNLValue = rowComparison["TenVT"].ToString();
                string mauComparisonValue = rowComparison["Mau"].ToString();
                string khoSizeComparisonValue = rowComparison["SizeKho"].ToString();
                // So sánh các giá trị
                if (maVatTuValue == maVatTuComparisonValue && mauValue == mauComparisonValue && tenVatTuValue == tenVatTuNLValue && khoSizeValue == khoSizeComparisonValue)
                {
                    // Nếu tìm thấy trùng khớp, thêm dòng vào danh sách cần tô màu
                    rowsToHighlight.Add(rowToCheck);
                    break; // Đã tìm thấy, không cần kiểm tra tiếp
                }
            }
        }
        //so sánh các dòng trong file excel
        private void HighlightRowsInTable(DataTable dtSave)
        {
            List<DataRow> duplicateRows = new List<DataRow>();

            // Tạo một HashSet để lưu các chỉ số dòng đã xử lý
            HashSet<int> processedRows = new HashSet<int>();

            // Vòng lặp chính để duyệt qua từng dòng trong DataTable
            for (int i = 0; i < dtSave.Rows.Count; i++)
            {
                // Kiểm tra xem dòng này đã được xử lý chưa
                if (processedRows.Contains(i))
                {
                    continue; // Bỏ qua nếu dòng đã xử lý
                }

                DataRow row1 = dtSave.Rows[i];
                bool hasDuplicate = false; // Đặt trạng thái mặc định là không tìm thấy trùng

                // Vòng lặp lồng để so sánh dòng hiện tại với những dòng còn lại
                for (int j = i + 1; j < dtSave.Rows.Count; j++)
                {
                    DataRow row2 = dtSave.Rows[j];

                    // Ví dụ so sánh: kiểm tra trùng dựa trên cột "MaVT" và "Mau"
                    if (row1["MaVT"].ToString() == row2["MaVT"].ToString() &&
                        row1["TenVT"].ToString() == row2["TenVT"].ToString() &&
                        row1["Mau"].ToString() == row2["Mau"].ToString() &&
                        row1["SizeKho"].ToString() == row2["SizeKho"].ToString()
                        )
                    {
                        // Nếu có dòng trùng, thêm dòng thứ 2 vào danh sách
                        duplicateRows.Add(row2);

                        // Đánh dấu đã xử lý dòng thứ 2
                        processedRows.Add(j);

                        // Đặt trạng thái là có dòng trùng
                        hasDuplicate = true;
                    }
                }

                // Nếu dòng đầu tiên cũng có trùng, thêm nó vào danh sách trùng
                if (hasDuplicate)
                {
                    duplicateRows.Add(row1);
                    processedRows.Add(i); // Đánh dấu dòng hiện tại đã xử lý
                }
            }

            rowsToHighlightinTable = new List<DataRow>(duplicateRows);
        }
        private DataRow checkRowError(DataRow row)
        {
            string urlMau = $"{URL}ThuVienMau/Get";
            string jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
            string urlKhoSize = $"{URL}ThuVienKhoSize/Get";
            string jsonKhoSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKhoSize); }).Result;
            string urlDVT = $"{URL}DonViChungLoai/Get";
            string jsonDVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDVT); }).Result;
            // Gán dữ liệu cho cột "Màu"

            bool isError = false;
            if (jsonMau != "[]")
            {
                tbMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);
                if (tbMau.Rows.Count == 0) return row;
                string _tenMau = tbMau.Select($"CodeMauKH = '{row["CodeMau"]}'").FirstOrDefault()?["TenMauKH"]?.ToString();
                string _maMau = tbMau.Select($"CodeMauKH = '{row["CodeMau"]}'").FirstOrDefault()?["MaMauKH"]?.ToString();
                if (string.IsNullOrEmpty(_tenMau) || string.IsNullOrEmpty(_maMau))
                {
                    isError = true; // Đánh dấu dòng lỗi nếu không tìm thấy giá trị
                }
               /* row["Mau"] = _tenMau;
                row["MaMau"] = _maMau;*/
            }
            if (jsonKhoSize != "[]")
            {
                tbKhoSize = JsonConvert.DeserializeObject<DataTable>(jsonKhoSize);
                if (tbKhoSize.Rows.Count == 0) return row;
                string _maKhoSize = tbKhoSize.Select($"KhoSize = '{row["SizeKho"]}'").FirstOrDefault()?["MaKhoSize"]?.ToString();
                if (string.IsNullOrEmpty(_maKhoSize))
                {
                    isError = true; // Đánh dấu dòng lỗi nếu không tìm thấy giá trị
                }
               /* row["MaSizeKho"] = _maKhoSize;*/
            }
            if (jsonDVT != "[]")
            {
                tbDVTinh = JsonConvert.DeserializeObject<DataTable>(jsonDVT);
                if (tbDVTinh.Rows.Count == 0) return row;
                string _maDVT = tbDVTinh.Select($"TenDVCL = '{row["DVTinh"]}'").FirstOrDefault()?["MaDVCL"]?.ToString();
                if (string.IsNullOrEmpty(_maDVT))
                {
                    isError = true; // Đánh dấu dòng lỗi nếu không tìm thấy giá trị
                }
               /* row["MaDVTinh"] = _maDVT;*/
            }
            if (isError)
            {
                rowsError.Add(row);
            }
            else
            {
                if (rowsError.Contains(row))
                {
                    rowsError.Remove(row);
                }
            }
            return row;
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
            dt.Columns.Add("MaVT", typeof(string));
            dt.Columns.Add("TenVT", typeof(string));
            dt.Columns.Add("CodeVT", typeof(string));
            dt.Columns.Add("Mau", typeof(string));
            dt.Columns.Add("CodeMau", typeof(string));
            dt.Columns.Add("ItemCode", typeof(string));
            dt.Columns.Add("SizeKho", typeof(string));
            dt.Columns.Add("TenGD", typeof(string));
            dt.Columns.Add("MaKho", typeof(string));
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
        /*private void EnsureTableColumnsExist(DataTable table)
        {
            // Danh sách các cột cần thiết cùng kiểu dữ liệu
            Dictionary<string, Type> requiredColumns = new Dictionary<string, Type>
    {
        {"MaVT", typeof(string)},
        {"TenVT", typeof(string)},
        {"CodeVT", typeof(string)},
        {"MaKho", typeof(string)},
        {"MaVTHaiQuan", typeof(string)},
        {"ItemCode", typeof(string)},
        {"CodeMau", typeof(string)},
        {"Mau", typeof(string)},
        {"SizeKho", typeof(string)},
        {"TenGD", typeof(string)},
        {"DVTinh", typeof(string)},
        {"DV1", typeof(float)},
        {"DV2", typeof(float)},
        {"MaNhomVT", typeof(string)},
        {"NhaCungCap", typeof(string)},
        {"TenHaiQuan", typeof(string)},
        {"XuatXu", typeof(string)},
        {"SLTonToiThieu", typeof(float)},
        {"SLTonToiDa", typeof(float)},
        {"TKVT", typeof(string)},
        {"TKGiaVon", typeof(string)},
        {"TKDoanhThu", typeof(string)},
        {"TKHangBanTraLai", typeof(string)},
        {"ThueSuat", typeof(float)},
        {"GhiChu", typeof(string)},
        {"IsNL", typeof(bool)},
        {"IsPL", typeof(bool)}
    };

            // Kiểm tra và thêm các cột còn thiếu vào DataTable hiện tại
            foreach (var column in requiredColumns)
            {
                if (!table.Columns.Contains(column.Key))
                {
                    table.Columns.Add(column.Key, column.Value);
                }
            }
        }*/

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
            //foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            //{
            //    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            //}
            e.Handled = true;


        }



        private async void LuuDong()
        {

            string[] arrMau = listMau.Split(';'); 
            string[] arrKhoSize = listKhoSize.Split(';');
            string[] lstmaColor = maColor.Split(';');
            string[] lstCodemau = CodeMau.Split(';');
            string[] lstMaKS = codeKhoSize.Split(';');
            if (searchLookUpEdit3.EditValue == null)
            {
                XtraMessageBox.Show("Vui lòng chọn vật tư!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
           
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTenVatTu.Text))
            {
                XtraMessageBox.Show("Vui lòng nhập tên vật tư!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenVatTu.Focus();
                return;
            }
            // Lấy danh sách nguyên liệu hiện có từ API (nếu cần thiết)
            string urlGetNL = string.Format("{0}?", URL + "KhoVatTu/GetNL");
            string jsonGetNL = await _clientExtension.GetAsnyc(urlGetNL);
            DataTable _NLLuuDong = JsonConvert.DeserializeObject<DataTable>(jsonGetNL);
            string urlGetPL = string.Format("{0}?", URL + "KhoVatTu/GetPL");
            string jsonGetPL = await _clientExtension.GetAsnyc(urlGetPL);
            DataTable _PLLuuDong = JsonConvert.DeserializeObject<DataTable>(jsonGetPL);
            //List<NguyenLieuEntity> existingNguyenLieuList = JsonConvert.DeserializeObject<List<NguyenLieuEntity>>(jsonGetNL);
            if (_PLLuuDong.Rows.Count > 0)
            {

                foreach (DataRow row in _PLLuuDong.Rows)
                {
                    _NLLuuDong.ImportRow(row);
                }
            }
            if (_tblImportExcel == null)
            {
                _tblImportExcel = CreateTblSave();
            }
           /* else
            {
                EnsureTableColumnsExist(_tblImportExcel); // Bổ sung cột còn thiếu
            }*/

            int focusedRowHandle = gVNhapNguyenLieu.FocusedRowHandle;
            DataRow baseRow = gVNhapNguyenLieu.GetDataRow(focusedRowHandle); // Dòng gốc (dòng đang chỉnh sửa hoặc dòng mới)

            
            if (arrMau.Length < 2 && arrKhoSize.Length < 2 )
            {
                DataRow dr = gVNhapNguyenLieu.GetFocusedDataRow();
                if (rowsToHighlight.Contains(dr))
                {
                    rowsToHighlight.Remove(dr);
                }
                dr["MaVT"] = searchLookUpEdit3.EditValue.ToString();
                    dr["TenVT"] = txtTenVatTu.Text;
                    dr["CodeVT"] = txtCodeVatTu.Text;
                    dr["MaKho"] = txtMaKho.Text;
                    dr["MaVTHaiQuan"] = txtMaHaiQuan.Text;
                    dr["ItemCode"] = txtItemCode.Text;
                    dr["MaMau"] = lstmaColor[0] == "" ? dr["MaMau"].ToString().Trim() : lstmaColor[0];
                    dr["Mau"] = arrMau[0] == "" ? dr["Mau"].ToString().Trim() : arrMau[0]; 
                    dr["CodeMau"] = lstCodemau[0] == "" ? dr["CodeMau"].ToString().Trim() : lstCodemau[0]; 

                    dr["SizeKho"] = arrKhoSize[0] == "" ? dr["SizeKho"].ToString().Trim() : arrKhoSize[0];
                    dr["TenGD"] = txtTenGD.Text;
                   dr["MaSizeKho"] = lstMaKS[0] == "" ? dr["MaSizeKho"].ToString().Trim() : lstMaKS[0]; 
                      dr["DVTinh"] = txtDVTinh.Text;
                    dr["DV1"] = float.Parse("0.9144");
                    dr["DV2"] = float.Parse("1.09361");
                    dr["MaNhomVT"] = txtMaNhomVT.Text;
                    dr["NhaCungCap"] = txtNhaCungCap.Text;
                    dr["TenHaiQuan"] = txtTenHaiQuan.Text;
                    dr["XuatXu"] = txtXuatXu.Text;
                    dr["SLTonToiThieu"] = float.TryParse(txtSLTonToiThieu.Text, out float slTonToiThieu) ? slTonToiThieu : 0;
                    dr["SLTonToiDa"] = float.TryParse(txtSLTonToiDa.Text, out float slTonToiDa) ? slTonToiDa : 0;
                    dr["TKVT"] = txtTKVT.Text;
                    dr["TKGiaVon"] = txtTKGiaVon.Text;
                    dr["TKDoanhThu"] = txtTKDoanhThu.Text;
                    dr["TKHangBanTraLai"] = txtTKHangBanTraLai.Text;
                    dr["ThueSuat"] = float.TryParse(txtThueSuat.Text, out float thueSuat) ? thueSuat : 0;
                    dr["GhiChu"] = txtGhiChu.Text;
                    dr["IsNL"] = txtLoaiVatTu.Text == "Nguyên liệu" ? true : false;
                    dr["IsPL"] = txtLoaiVatTu.Text == "Nguyên liệu" ? false : true;
                dr = checkRowError(dr); // Kiểm tra lỗi
                rowsEdit.Add(dr);
                Highlight(dr, _NLLuuDong);
            }
            else
            {

                int index = 0;
                foreach (string _mau in lstmaColor)
                { 
                    string maunew = _mau == "" ? baseRow["MaMau"].ToString() : _mau;
                    if (string.IsNullOrWhiteSpace(_mau)) continue;

                    foreach (string _KhoSize in arrKhoSize)
                    {
                        string maKhoSizenew = string.Empty;
                        string khosizenew = _KhoSize == "" ? baseRow["SizeKho"].ToString() : _KhoSize;
                        if(lstMaKS.Length<=index||lstMaKS[index] =="")
                        {
                            maKhoSizenew = baseRow["MaSizeKho"].ToString();
                        }
                        else
                        {
                            maKhoSizenew= lstMaKS[index];
                        }
                        if (khosizenew == "") continue;

                        DataRow newRow;
                        if (_isEditing && baseRow != null)
                        {
                            newRow = _tblImportExcel.NewRow();
                            newRow.ItemArray = baseRow.ItemArray.Clone() as object[]; // Sao chép dữ liệu từ baseRow
                        }
                        else
                        {
                            newRow = _tblImportExcel.NewRow();
                        }
                        if (rowsToHighlight.Contains(newRow))
                        {
                            rowsToHighlight.Remove(newRow);
                        }
                        // Cập nhật thông tin dòng mới
                        newRow["MaVT"] = searchLookUpEdit3.EditValue.ToString();
                        newRow["TenVT"] = txtTenVatTu.Text;
                        newRow["CodeVT"] = txtCodeVatTu.Text;
                        newRow["MaKho"] = txtMaKho.Text;
                        newRow["MaVTHaiQuan"] = txtMaHaiQuan.Text;
                        newRow["ItemCode"] = txtItemCode.Text;
                        newRow["MaMau"] = _mau.ToString().Trim();
                        newRow["Mau"] = arrMau[index].ToString().Trim();
                        newRow["CodeMau"] = lstCodemau[index].ToString().Trim();
                        newRow["SizeKho"] = khosizenew;
                        newRow["MaSizeKho"] = maKhoSizenew;
                        newRow["TenGD"] = txtTenGD.Text;
                        newRow["DVTinh"] = txtDVTinh.Text;
                        newRow["DV1"] = float.Parse("0.9144");
                        newRow["DV2"] = float.Parse("1.09361");
                        newRow["MaNhomVT"] = txtMaNhomVT.Text;
                        newRow["NhaCungCap"] = txtNhaCungCap.Text;
                        newRow["TenHaiQuan"] = txtTenHaiQuan.Text;
                        newRow["XuatXu"] = txtXuatXu.Text;
                        newRow["SLTonToiThieu"] = float.TryParse(txtSLTonToiThieu.Text, out float slTonToiThieu) ? slTonToiThieu : 0;
                        newRow["SLTonToiDa"] = float.TryParse(txtSLTonToiDa.Text, out float slTonToiDa) ? slTonToiDa : 0;
                        newRow["TKVT"] = txtTKVT.Text;
                        newRow["TKGiaVon"] = txtTKGiaVon.Text;
                        newRow["TKDoanhThu"] = txtTKDoanhThu.Text;
                        newRow["TKHangBanTraLai"] = txtTKHangBanTraLai.Text;
                        newRow["ThueSuat"] = float.TryParse(txtThueSuat.Text, out float thueSuat) ? thueSuat : 0;
                        newRow["GhiChu"] = txtGhiChu.Text;
                        newRow["IsNL"] = txtLoaiVatTu.Text == "Nguyên liệu" ? true : false;
                        newRow["IsPL"] = txtLoaiVatTu.Text == "Nguyên liệu" ? false : true;

                        // **4. Kiểm tra lỗi và áp dụng highlight**
                        newRow = checkRowError(newRow); // Kiểm tra lỗi
                        Highlight(newRow, _NLLuuDong); // Highlight dòng nếu cần
                        rowsEdit.Add(newRow);
                        // **5. Thêm dòng mới vào DataTable**
                        _tblImportExcel.Rows.Add(newRow);
                    }
                    index++;
                }
                _tblImportExcel.Rows.Remove(baseRow);
            }

            
         // Mảng khổ size

           

            // **6. Đánh dấu các dòng được highlight trong bảng**
            HighlightRowsInTable(_tblImportExcel);

            // **7. Cập nhật lại GridView**
            gCNhapNguyenLieu.DataSource = _tblImportExcel;
            gCNhapNguyenLieu.RefreshDataSource();

            // Cập nhật trạng thái các nút
            setTxtEdit(true);
            if (_tblImportExcel.Rows.Count > 0)
            {
                Xoa.Enabled = true;
                Sua.Enabled = true;
            }
            Them.Enabled = true;
            btnLuu.Enabled = false;

            _isPost = false;
            Init();
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

            txtLoaiVatTu.SelectedItem = "Nguyên liệu";

        }
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
        private string addValueDVCL(string tenDVCL)
        {
            try
            {
                listDVT = reloadDVCL();
                if (tenDVCL == "") return "";
                string url = string.Format("{0}?", URL + "DonViChungLoai/Get");


                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;



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
                listNhomNPL = reloadNhomNPL();
                if (tenNhomNPL == "") return "";
                //listNhomNPL.Any(item => RemoveVietnameseTone(item.TenNhom) == RemoveVietnameseTone(nhomNPLValue));
                // API URL để lấy danh sách Nhóm NPL
                string url = string.Format("{0}?", URL + "KhoNPL/GetNhomNPL");

                // Gọi API GET để lấy danh sách Nhóm Nguyên Phụ Liệu
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                var listNPL = reloadNhomNPL();

                // Kiểm tra xem dữ liệu có tồn tại không
                if (string.IsNullOrEmpty(json) && json == "[]")
                {

                    listNPL = null;
                    listNhomNPL = null;
                }
                else
                {
                    listNPL = JsonConvert.DeserializeObject<List<NhomNguyenPhuLieuEntity>>(json);
                    listNhomNPL = JsonConvert.DeserializeObject<List<NhomNguyenPhuLieuEntity>>(json);
                }
                // Deserialize JSON thành danh sách đối tượng

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

            // Lấy giá trị hiện tại trong TextEdit
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
                string pattern = @"^[a-zA-Z0-9_]*$";
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
           /* if (_isPost)
            {
                string pattern = @"^[a-zA-Z0-9]*$";
                if (!string.IsNullOrWhiteSpace(txtCodeMau.Text) && !Regex.IsMatch(txtCodeMau.Text, pattern))
                {

                    toolTip.ToolTipTitle = "Lỗi nhập liệu";
                    toolTip.ToolTipIcon = ToolTipIcon.Warning;
                    toolTip.Show("Không thể nhập ký tự đặc biệt, tiếng Việt! Vui lòng sửa lỗi.", txtCodeMau, txtCodeMau.Width, txtCodeMau.Height, 2000);

                    e.Cancel = true;
                }
                else
                {
                    toolTip.Hide(txtCodeMau);
                }
            }*/

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

        private void txtTenVatTu_Properties_Validating(object sender, CancelEventArgs e)
        {
            if (_isPost)
            {
                string pattern = @"^[a-zA-Z0-9\u00C0-\u01BF\u1EA0-\u1EF9\s\-\(\)]*$";


                if (!Regex.IsMatch(txtTenVatTu.Text, pattern))
                {

                    toolTip.ToolTipTitle = "Lỗi nhập liệu";
                    toolTip.ToolTipIcon = ToolTipIcon.Warning;
                    toolTip.Show("Không thể nhập ký tự đặc biệt! Vui lòng sửa lỗi.", txtTenVatTu, txtTenVatTu.Width, txtTenVatTu.Height, 2000);

                    e.Cancel = true;
                }
                else
                {
                    toolTip.Hide(txtTenVatTu);
                }
            }

        }

        private void txtMaVatTu_Properties_Validating(object sender, CancelEventArgs e)
        {
            if (_isPost)
            {
                /*  string pattern = @"^[a-zA-Z0-9]*$";
                  if (!string.IsNullOrWhiteSpace(txtMaVatTu.Text) && !Regex.IsMatch(txtMaVatTu.Text, pattern))
                  {

                      toolTip.ToolTipTitle = "Lỗi nhập liệu";
                      toolTip.ToolTipIcon = ToolTipIcon.Warning;
                      toolTip.Show("Không thể nhập ký tự đặc biệt, tiếng Việt! Vui lòng sửa lỗi.", txtMaVatTu, txtMaVatTu.Width, txtMaVatTu.Height, 2000);

                      e.Cancel = true;
                  }
                  else
                  {
                      toolTip.Hide(txtMaVatTu);
                  }*/
            }

        }



        private void gVNhapNguyenLieu_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
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

                    if (!_isPost)
                    {
                        loadThongTinNguyenLieu();
                    }
                }
            }
        }

        private void gVNhapNguyenLieu_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            DataRow row = view.GetDataRow(e.RowHandle);
          
            // Kiểm tra xem dòng hiện tại có trong danh sách dòng cần tô màu không
            if (view.FocusedRowHandle == e.RowHandle)
            {
                e.Appearance.BackColor = Color.LightBlue;// Màu bạn muốn hiển thị khi hàng được focus
                e.Appearance.ForeColor = Color.Black;       // Màu chữ khi hàng được focus
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold); // Ví dụ làm chữ đậm
            }
            else if (rowsToHighlight.Contains(view.GetDataRow(e.RowHandle)))
            {
                e.Appearance.BackColor = Color.LightCoral;
                e.Appearance.ForeColor = Color.Black; // Có thể thay đổi màu chữ nếu cần
            }
            else if (rowsToHighlightinTable.Contains(view.GetDataRow(e.RowHandle)))
            {
                e.Appearance.BackColor = Color.FromArgb(255, 251, 209);// Màu nền xanh nhạt
                e.Appearance.ForeColor = Color.Black;    // Màu chữ đen

            }
            else if (rowsEdit.Contains(view.GetDataRow(e.RowHandle)))
            {
                e.Appearance.BackColor = Color.LightGreen;// Màu nền xanh nhạt
                e.Appearance.ForeColor = Color.Black;    // Màu chữ đen

            }
            if (rowsError.Contains(view.GetDataRow(e.RowHandle)))
            {
                e.Appearance.BackColor = Color.Firebrick;
                e.Appearance.ForeColor = Color.White;    

            }

        }

        private void gVNhapNguyenLieu_MouseWheel(object sender, MouseEventArgs e)
        {
            if (gVNhapNguyenLieu.IsEditing)
            {
                gVNhapNguyenLieu.CloseEditor();     // Kết thúc chế độ chỉnh sửa (kết thúc việc nhập liệu)
                gVNhapNguyenLieu.UpdateCurrentRow(); // Lưu lại nếu có thay đổi
            }
        }

        private void btnXacNhanExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            for (int i = 0; i < gVNhapNguyenLieu.RowCount; i++)
            {
                DataRow row = gVNhapNguyenLieu.GetDataRow(i); // Lấy hàng hiện tại

                if (row != null)
                {

                    if (rowsToHighlight.Contains(row)) // Ví dụ: bạn lưu danh sách các hàng tô đỏ
                    {
                        XtraMessageBox.Show("Vật tư bạn muốn thêm trùng với vật tư có sẵn (Tô đỏ)",
                                            "Cảnh báo",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                        return; // Dừng kiểm tra tại đây
                    }


                    if (rowsToHighlightinTable != null && rowsToHighlightinTable.Contains(row)) // Danh sách hàng tô xanh (nếu có)
                    {
                        XtraMessageBox.Show("Vật tư bạn muốn thêm bị trùng nhau (Tô vàng)",
                                            "Cảnh báo",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                        return; // Dừng kiểm tra tại đây
                    }
                    if (rowsError != null && rowsError.Contains(row)) // Danh sách hàng tô xanh (nếu có)
                    {
                        XtraMessageBox.Show("Vật tư bạn muốn có trường bị sai thông tin (Tô đỏ sẫm)",
                                            "Cảnh báo",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                        return; // Dừng kiểm tra tại đây
                    }

                }
            }

            // Nếu không có hàng nào bị tô màu không hợp lệ, thực hiện POST API
            PostDataToAPI();
        }
        private void PostDataToAPI()
        {
            try
            {
                // Lấy nguồn dữ liệu từ lưới
                DataTable tablePost = (gCNhapNguyenLieu.DataSource as DataTable);
                if (tablePost == null || tablePost.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!tablePost.Columns.Contains("MaVTMau"))
                {
                    tablePost.Columns.Add("MaVTMau", typeof(string)); // Thêm cột nếu chưa tồn tại
                }

             
                foreach (DataRow row in tablePost.Rows)
                {
                    
                    row["MaVTMau"] = ReplaceSpecialCharactersAndRemoveSpaces(RemoveVietnameseTone(row["CodeVT"].ToString()).Replace(" ", string.Empty)) + random.Next(0, 999) + DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
                }
               

                // Gọi API
                string url = string.Format("{0}?", URL + "KhoVatTu/Post");
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tablePost); }).Result;

                // Kiểm tra kết quả trả về từ API
                if (msResult == "True")
                {
                    MessageBox.Show("Lưu thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Đóng form sau khi lưu thành công
                    this.DialogResult = DialogResult.OK; // Tự động đóng form
                }
                else
                {
                    MessageBox.Show("Lưu thất bại. Vui lòng kiểm tra lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi nếu xảy ra ngoại lệ
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void searchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

        }

        private void searchLookUpEdit2View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }
        string maColor = "";
        string codeKhoSize = "";
        string CodeMau = "";
        private void searchLookUpEdit1_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            listMau = "";
            maColor = "";
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
            CodeMau = sbCodeMau.ToString();
            txtCodeMau.Text = CodeMau;
            StringBuilder sbMaMau = new StringBuilder();
            foreach (DataRowView rv in gridCheckMarksMau.Selection)
            {
                if (sbMaMau.Length > 0) sbMaMau.Append("; "); // Thêm dấu phân cách
                sbMaMau.Append(rv["MaMauKH"].ToString()); // Lấy Mã Màu
            }

            maColor = sbMaMau.ToString();

        }

        private void searchLookUpEdit2_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void searchLookUpEdit2_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            listKhoSize = "";
            codeKhoSize = "";
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

            StringBuilder sbMaSize = new StringBuilder();
            foreach (DataRowView rv in gridCheckMarksKhoSize.Selection)
            {
                if (sbMaSize.ToString().Length > 0) { sbMaSize.Append(";"); }
                sbMaSize.Append(rv["MaKhoSize"].ToString());

            }
            codeKhoSize = sbMaSize.ToString();
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
        void searchLookUpEditMau_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            //StringBuilder sb = new StringBuilder();
            //SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            //if (gridCheckMark == null) return;
            //foreach (DataRowView rv in gridCheckMark.Selection)
            //{

            //    if (sb.ToString().Length > 0) { sb.Append("; "); }
            //    sb.Append(rv["TenMauKH"].ToString());
            //}
            //if (string.IsNullOrEmpty(sb.ToString()))
            //{
            //    e.DisplayText = "Chọn Màu";
            //}
            //else
            //{

            //    e.DisplayText = sb.ToString();
            //}

        }
        void searchLookUpEditKhoSize_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            //StringBuilder sb = new StringBuilder();
            //SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            //if (gridCheckMark == null) return;
            //foreach (DataRowView rv in gridCheckMark.Selection)
            //{

            //    if (sb.ToString().Length > 0) { sb.Append("; "); }
            //    sb.Append(rv["TenMauKH"].ToString());
            //}
            //if (string.IsNullOrEmpty(sb.ToString()))
            //{
            //    e.DisplayText = "Chọn Màu";
            //}
            //else
            //{

            //    e.DisplayText = sb.ToString();
            //}

        }
        // Hàm bật/tắt khả năng mở Popup
        private void SetSearchLookUpEditPopupState(DevExpress.XtraEditors.SearchLookUpEdit searchLookUpEdit, bool allowPopup)
        {
            if (allowPopup)
            {
                // Cho phép mở popup - Hủy bỏ sự kiện QueryPopUp
                searchLookUpEdit.Properties.QueryPopUp -= PreventPopup; // Xóa sự kiện nếu đã gán trước đó
            }
            else
            {
                // Ngăn không cho mở popup - Gán sự kiện QueryPopUp
                searchLookUpEdit.Properties.QueryPopUp -= PreventPopup; // Đảm bảo không bị gán trùng
                searchLookUpEdit.Properties.QueryPopUp += PreventPopup; // Gán sự kiện ngăn mở popup
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

        private void gVNhapNguyenLieu_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void searchLookUpEdit3View_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void searchLookUpEdit3_EditValueChanged(object sender, EventArgs e)
        {
            var selectedValue = searchLookUpEdit3.EditValue;
            if (selectedValue != null)
            {
                // Lấy DataRow của dòng được chọn
                var selectedRow = searchLookUpEdit3.Properties.View.GetFocusedDataRow();

                if (selectedRow != null)
                {
                    // Gán MaVT vào ô SearchLookUpEdit (đã thực hiện qua ValueMember)
                    searchLookUpEdit3.EditValue = selectedRow["MaVT"];

                    // Gán TenVT vào ô TextEdit
                    txtTenVatTu.Text = selectedRow["TenVT"].ToString();
                }
            }
        }

        private void setTxtNull()
        {
            txtID.Text = "";
            txtMaVatTu.Text = "";
            txtMaKeToan.Text = "";
            searchLookUpEdit3.EditValue = null;
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
        private void setTxtValue(DataRow row)
        {
            if (row["IsNL"].ToString() == "") return;
             if (searchLookUpEdit1View!=null&& gridCheckMarksMau!=null)
            {
                gridCheckMarksMau.ClearSelection(searchLookUpEdit1View);
                searchLookUpEdit1.EditValue = null;
                searchLookUpEdit1.Refresh();
            }    
            if(searchLookUpEdit2View!=null&& gridCheckMarksKhoSize!=null)
            {

                gridCheckMarksKhoSize.ClearSelection(searchLookUpEdit2View);
                searchLookUpEdit2.EditValue = null;
                searchLookUpEdit2.Refresh();
            }    
            string _maMau = tbMau.Select($"CodeMauKH = '{row["CodeMau"]}'").FirstOrDefault()?["MaMauKH"]?.ToString();
            string _maKhoSize = tbKhoSize.Select($"KhoSize = '{row["SizeKho"]}'").FirstOrDefault()?["MaKhoSize"]?.ToString();
            txtID.Text = row["ID"].ToString();

            searchLookUpEdit3.EditValue = row["MaVT"].ToString();
            txtCodeVatTu.Text = row["CodeVT"].ToString();
            txtTenVatTu.Text = row["TenVT"].ToString();
            txtMaKeToan.Text = row["MaKeToan"].ToString();
            searchLookUpEdit1.EditValue = _maMau;
            searchLookUpEdit2.EditValue = _maKhoSize;
            //txtMau.Text = _tenMau;
            txtCodeMau.Text = row["CodeMau"].ToString();
            txtItemCode.Text = row["ItemCode"].ToString();
            txtSizeKho.Text = row["SizeKho"].ToString();
            txtTenGD.Text = row["TenGD"].ToString();
            txtMaKho.Text = row["MaKho"].ToString();
            txtMaNhomVT.Text = row["MaNhomVT"].ToString();
            txtDVTinh.Text = row["DVTinh"].ToString();
            txtDV1.Text = row["DV1"].ToString();
            txtDV2.Text = row["DV2"].ToString();
            txtNhaCungCap.Text = row["NhaCungCap"].ToString();
            txtMaHaiQuan.Text = row["MaVTHaiQuan"].ToString();
            txtTenHaiQuan.Text = row["TenHaiQuan"].ToString();
            txtXuatXu.Text = row["XuatXu"].ToString();

            // Cập nhật loại vật tư
            txtLoaiVatTu.Properties.Items.Clear();
            txtLoaiVatTu.Properties.Items.Add("Nguyên liệu");
            txtLoaiVatTu.Properties.Items.Add("Phụ liệu");

            if (bool.Parse(row["IsNL"].ToString()))
            {
                txtLoaiVatTu.SelectedItem = "Nguyên liệu";
            }
            else
            {
                txtLoaiVatTu.SelectedItem = "Phụ liệu";
            }

            txtSLTonToiThieu.Text = row["SLTonToiThieu"].ToString();
            txtSLTonToiDa.Text = row["SLTonToiDa"].ToString();
            txtThueSuat.Text = row["ThueSuat"].ToString() + "%";
            txtGhiChu.Text = row["GhiChu"].ToString();
            txtTKVT.Text = row["TKVT"].ToString();
            txtTKGiaVon.Text = row["TKGiaVon"].ToString();
            txtTKDoanhThu.Text = row["TKDoanhThu"].ToString();
            txtTKHangBanTraLai.Text = row["TKHangBanTraLai"].ToString();
        }
        #region hàm phụ
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
        #endregion
    }
}