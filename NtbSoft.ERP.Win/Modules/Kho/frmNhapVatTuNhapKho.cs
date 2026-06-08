using DevExpress.Data;
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
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
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
using ZXing;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmNhapVatTuNhapKho : DevExpress.XtraEditors.XtraForm
    {
        SearchCheckSelection gridCheckMarks_VatTu;

        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();

        DataTable tblBOM, tblVT, tblSave, tblTenVT,tblNhapKho,tblVatTuChiTiet;
        DataTable tblImportExcel,tblKhachHang,tblMaKho,tblViTri;
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        private string _Mahang = string.Empty, nhomsize = string.Empty, _MaVT = string.Empty
        , _TenVatTu = string.Empty, _Mau = string.Empty, _KhoVai = string.Empty, _DonViTinh = string.Empty,_MaVTMau=string.Empty;

        bool indicatorIcon = true;
         
        int dotnhap = -1;
        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        private bool isUpdating = false;
        string _nguoiTao = string.Empty;
        private object oldValue;
        List<string> _lstBarcode=new List<string>();
        Random _random = new Random();
        public frmNhapVatTuNhapKho()
        {
            InitializeComponent();
            this.KeyPreview = true;
            //this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);

            _clientExtension = new HttpClientExtension();
            tblBOM = new DataTable();
            tblVT = new DataTable();
            tblSave = new DataTable();
            tblTenVT = new DataTable();
            tblNhapKho = new DataTable();
            tblVatTuChiTiet = new DataTable();
            tblImportExcel = new DataTable();
            tblKhachHang = new DataTable();
            tblMaKho = new DataTable();
            tblViTri = new DataTable();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
           
        }
        public frmNhapVatTuNhapKho(DataTable dtPost,string _mahang,string _mavt)
        {
            InitializeComponent();
            this.KeyPreview = true;
            //this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);

            _clientExtension = new HttpClientExtension();
            tblBOM = new DataTable();
            tblVT = new DataTable();
            tblSave = new DataTable();
            tblTenVT = new DataTable();
            tblNhapKho = new DataTable();
            tblKhachHang=new DataTable();
            tblMaKho = new DataTable();
            tblViTri = new DataTable();
            CreateNewDataTable();
            tblVatTuChiTiet = new DataTable();
            tblImportExcel = new DataTable();
            tblImportExcel = dtPost.Copy();
            _Mahang = _mahang;
            _MaVT = _mavt;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            txtMaDH.Text = _mahang;
            txtMaVT.Text = _mavt;
            SaveTable(dtPost);
        }
        public frmNhapVatTuNhapKho(DataTable dtPost, DataRow row)
        {
            InitializeComponent();
            this.KeyPreview = true;
            //this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);

            _clientExtension = new HttpClientExtension();
            tblBOM = new DataTable();
            tblVT = new DataTable();
            tblSave = new DataTable();
            tblTenVT = new DataTable();
            tblNhapKho = new DataTable();
            tblKhachHang = new DataTable();
            tblMaKho = new DataTable();
            tblViTri = new DataTable();
            CreateNewDataTable();
            tblVatTuChiTiet = new DataTable();
            tblImportExcel = new DataTable();
            tblImportExcel = dtPost.Copy();
            _Mahang = row["MaDH"].ToString();
            _MaVT = row["MaVatTu"].ToString();
            _TenVatTu = row["TenVatTu"].ToString();
            _Mau = row["Mau"].ToString();
            _KhoVai = row["KhoVai"].ToString();
            _DonViTinh = row["DonViTinh"].ToString();
            _MaVTMau = row["MaVTMau"].ToString();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            txtMaDH.Text = _Mahang;
            txtMaVT.Text = _MaVT;
            txtTenVT.Text = _TenVatTu;
            txtTenMau.Text = _Mau;
            txtKhoSize.Text = _KhoVai;
            txtDVTinh.Text = _KhoVai;
            SaveTable(dtPost);
        }
      
        private void CreateSearchLookupViTri()
        {

            string urlViTri = string.Format("{0}?", URL + "VatTuNhapKho/GetViTri");
            string jsonViTri = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlViTri); }).Result;
            tblViTri = JsonConvert.DeserializeObject<DataTable>(jsonViTri);


            searchLookUpEditViTri.Properties.DataSource = tblViTri;
            searchLookUpEditViTri.Properties.ValueMember = "MaDisplay";
            searchLookUpEditViTri.Properties.DisplayMember = "Display";


            RepositoryItemSearchLookUpEdit repositorySearchLookupViTri = new RepositoryItemSearchLookUpEdit();
            repositorySearchLookupViTri.DataSource = tblViTri;
            repositorySearchLookupViTri.DisplayMember = "TenO";
            repositorySearchLookupViTri.ValueMember = "MaDisplay";
            repositorySearchLookupViTri.ShowClearButton = false;
            repositorySearchLookupViTri.NullText = "[Chọn giá trị]";

            GridView dvView = repositorySearchLookupViTri.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "TenO", Caption = "Tên ô", Name = "colTenOSearch", Visible = true, Width = 50 });
                dvView.Columns.Add(new GridColumn { FieldName = "MaDisplay", Caption = "Mã display", Name = "colMaDisplaySearch", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "Display", Caption = "Vị trí", Name = "colDisplaySearch", Visible = true });

            }
            colMaViTriNK.ColumnEdit = repositorySearchLookupViTri;
        }
        private void CreateSearchLookupMaKho()
        {

            string urlMaKho = string.Format("{0}?", URL + "VatTuNhapKho/GetMaKho");
            string jsonMaKho = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMaKho); }).Result;
            tblMaKho = JsonConvert.DeserializeObject<DataTable>(jsonMaKho);


            searchLookUpEditMaKho.Properties.DataSource = tblMaKho;
            searchLookUpEditMaKho.Properties.ValueMember = "MaKho";
            searchLookUpEditMaKho.Properties.DisplayMember = "TenDVSX";


            RepositoryItemSearchLookUpEdit repositorySearchLookupMakho = new RepositoryItemSearchLookUpEdit();
            repositorySearchLookupMakho.DataSource = tblMaKho;
            repositorySearchLookupMakho.DisplayMember = "TenDVSX";
            repositorySearchLookupMakho.ValueMember = "MaKho";
            repositorySearchLookupMakho.ShowClearButton = false;
            repositorySearchLookupMakho.NullText = "[Chọn giá trị]";

            GridView dvView = repositorySearchLookupMakho.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaKho", Caption = "Mã kho", Name = "colMaKhoSearch", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenDVSX", Caption = "Tên kho", Name = "colTenKhoSearch", Visible = true });

            }
            colMaKhoNK.ColumnEdit = repositorySearchLookupMakho;
        }
        private void CreateSearchLookupKhachHang()
        {
            string url = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblKhachHang = JsonConvert.DeserializeObject<DataTable>(json);
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditKH.Properties.DataSource = tbl;
            searchLookUpEditKH.Properties.ValueMember = "MaKH";
            searchLookUpEditKH.Properties.DisplayMember = "TenKH";
            RepositoryItemSearchLookUpEdit repositorySearchLookupKhachHang = new RepositoryItemSearchLookUpEdit();
            repositorySearchLookupKhachHang.DataSource = tbl;
            repositorySearchLookupKhachHang.DisplayMember = "TenKH";
            repositorySearchLookupKhachHang.ValueMember = "MaKH";
            repositorySearchLookupKhachHang.ShowClearButton = false;
            repositorySearchLookupKhachHang.NullText = "[Chọn giá trị]";

            GridView dvView = repositorySearchLookupKhachHang.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaKH", Caption = "Mã khách hàng", Name = "colMaKHSearch", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Tên khách hàng", Name = "colTenKHSearch", Visible = true });

            }
            colMaDTKHNK.ColumnEdit = repositorySearchLookupKhachHang;
        }
        private void SaveTable(DataTable dtPost)
        {
            if (dtPost.Rows.Count == 0)
            {
                XtraMessageBox.Show("File Excel rỗng! Vui lòng thử lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Xoa.Enabled = false;
               
                btn_Save.Enabled = false;
                this.DialogResult = DialogResult.OK;
                return ;
            }
            if (!dtPost.Columns.Contains("XacNhan"))
            {
                DataColumn col = new DataColumn("XacNhan", typeof(bool));
                dtPost.Columns.Add(col);

                // Đánh dấu tất cả các hàng trong cột 'XacNhan' là true
                foreach (DataRow row in dtPost.Rows)
                {
                    row["XacNhan"] = false; // Hoặc điều kiện logic để xác định giá trị
                }
            }
            if (!dtPost.Columns.Contains("IsEdit"))
            {
                DataColumn col = new DataColumn("IsEdit", typeof(int));
                dtPost.Columns.Add(col);

                // Đánh dấu tất cả các hàng trong cột 'XacNhan' là true
                foreach (DataRow row in dtPost.Rows)
                {
                    row["IsEdit"] = 0; // Hoặc điều kiện logic để xác định giá trị
                }
            }


            tblNhapKho = dtPost;
            gCNhapKho.DataSource = tblNhapKho;

            for (int i = 0; i < gVNhapKho.RowCount; i++)
            {
                gVNhapKho.SetRowCellValue(i, colMaDTKHNK, dtPost.Rows[i]["MaDTKH"].ToString());
            }
        }    

        private void BtLoad()
        {
            try
            {
                CreateSearchLookup();
                CreateSearchLookupKhachHang();
                CreateSearchLookupMaKho();
                CreateSearchLookupViTri();


                CreateTableSave();
                //creatTable_BOM();
                loadtblVatTuChiTiet();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại1.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        protected override void OnLoad(EventArgs e)
        {
            BtLoad();
            //CheckPerminsion();
            //keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }
        private void CreateNewDataTable()
        {
            tblNhapKho = new DataTable("tblNhapKho");

            tblNhapKho.Columns.Add("ID", typeof(int));
            tblNhapKho.Columns.Add("MaVTNhapKho", typeof(string));
            tblNhapKho.Columns.Add("MaDH", typeof(string));
            tblNhapKho.Columns.Add("Barcode", typeof(string));
            tblNhapKho.Columns.Add("MaVT", typeof(string));
            tblNhapKho.Columns.Add("TenVT", typeof(string));
            tblNhapKho.Columns.Add("MaVTMau", typeof(string));
            tblNhapKho.Columns.Add("SoLuong", typeof(float));
            tblNhapKho.Columns.Add("MaMau", typeof(string));
            tblNhapKho.Columns.Add("TenMau", typeof(string));
            tblNhapKho.Columns.Add("CodeMau", typeof(string));
            tblNhapKho.Columns.Add("MaKhoSize", typeof(string));
            tblNhapKho.Columns.Add("KhoSize", typeof(string));
            tblNhapKho.Columns.Add("TongKien", typeof(string));
            tblNhapKho.Columns.Add("Kien", typeof(string));
            tblNhapKho.Columns.Add("MaVTSP", typeof(string));
            tblNhapKho.Columns.Add("MaViTri", typeof(string));
            tblNhapKho.Columns.Add("MaKho", typeof(string));
            tblNhapKho.Columns.Add("MaDTKH", typeof(string));
            tblNhapKho.Columns.Add("Lot", typeof(string));
            tblNhapKho.Columns.Add("AnhMau", typeof(string));
            tblNhapKho.Columns.Add("LoangMau", typeof(string));
            tblNhapKho.Columns.Add("GhiChu", typeof(string));
            tblNhapKho.Columns.Add("NgayNhap", typeof(string));
            tblNhapKho.Columns.Add("NguoiNhap", typeof(string));
            tblNhapKho.Columns.Add("DotNhap", typeof(string));
            tblNhapKho.Columns.Add("IsEdit", typeof(int));
        }

        private void CreateTableSave()
        {
            tblSave = new DataTable("tblSave");

            tblSave.Columns.Add("ID", typeof(int));
            tblSave.Columns.Add("MaVTNhapKho", typeof(string));
            tblSave.Columns.Add("MaDH", typeof(string));
            tblSave.Columns.Add("Barcode", typeof(string));
            tblSave.Columns.Add("MaVT", typeof(string));
            tblSave.Columns.Add("TenVT", typeof(string));
            tblSave.Columns.Add("MaVTMau", typeof(string));
            tblSave.Columns.Add("SoLuong", typeof(float));
            tblSave.Columns.Add("MaMau", typeof(string));
            tblSave.Columns.Add("TenMau", typeof(string));
            tblSave.Columns.Add("CodeMau", typeof(string));
            tblSave.Columns.Add("MaKhoSize", typeof(string));
            tblSave.Columns.Add("KhoSize", typeof(string));
            tblSave.Columns.Add("TongKien", typeof(string));
            tblSave.Columns.Add("Kien", typeof(string));
            tblSave.Columns.Add("MaVTSP", typeof(string));
            tblSave.Columns.Add("MaViTri", typeof(string));
            tblSave.Columns.Add("MaKho", typeof(string));
            tblSave.Columns.Add("MaDTKH", typeof(string));
            tblSave.Columns.Add("Lot", typeof(string));
            tblSave.Columns.Add("AnhMau", typeof(string));
            tblSave.Columns.Add("LoangMau", typeof(string));
            tblSave.Columns.Add("GhiChu", typeof(string));
            tblSave.Columns.Add("NgayNhap", typeof(string));
            tblSave.Columns.Add("NguoiNhap", typeof(string));
            tblSave.Columns.Add("DotNhap", typeof(string));
        }
        private void loadtblVatTuChiTiet()
        {
            string url = string.Format("{0}?", URL + "KhoVatTu/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tblVatTuChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
        }



        private void LoadDataSource()
        {
            gCNhapKho.DataSource = tblNhapKho;


        }
        //private void frm_KeyDown(object sender, KeyEventArgs e)
        //{
        //    keyDownControlHandler.PressKeyDown(e);
        //}
        //private List<ActionControl> InitActionKeyDown()
        //{
        //    actionControlAdd = new ActionControl(BtThem, _allowAdd, ActionType.Add, this.Them.Enabled);

        //    actionControlRefresh = new ActionControl(BtNapLai, true, ActionType.Refresh, this.Naplai.Enabled);

        //    lstActionControls = new List<ActionControl> {
        //        actionControlAdd, actionControlSave,actionControlRefresh };
        //    return lstActionControls;
        //}
        
        private void CheckPerminsion()
        {
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
        private void CreateSearchLookup()
        {
            
            string urlVT = string.Format("{0}?", URL + "BOM/GET_TenVatTu");
            string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
            tblTenVT = JsonConvert.DeserializeObject<DataTable>(jsonVT);
       



            string urlHH = string.Format("{0}?", URL + "BOM/GETMH");
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            DataTable tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
           



        }

  

        private void BtSave()
        {
            try
            {

                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                if (tblNhapKho.Rows.Count > 0)
                {
                    tblSave.Clear();
                   
                    

                    loadDotNhap(_MaVT);
                    //DataTable tblSource = gCNhapKho.DataSource as DataTable;
                    for (int i = 0; i < tblNhapKho.Rows.Count; i++)
                    {
                        if (tblNhapKho.Rows[i]["XacNhan"].ToString() != "True")
                        {
                            continue;
                        }
                        Random random = new Random();
                        string uniqueCode = /*DateTime.UtcNow.Ticks.ToString() +*/"|" + random.Next(0, 99999);


                        DataRow _dr = tblSave.NewRow();
                       
                            _dr["ID"] = 0;
                        Random random1 = new Random();
                        _dr["MaVTNhapKho"] = _MaVT + DateTime.Now.ToString("yyyy-MM-dd_HHmmss")+random1.Next(0,99) ;

                        _dr["MaDH"] = tblNhapKho.Rows[i]["MaDH"];
                        _dr["Barcode"] = tblNhapKho.Rows[i]["Barcode"];
                        //_dr["MaVT"] = tblNhapKho.Rows[i]["MaVT"];
                        _dr["MaVT"] = _MaVT;
                        _dr["TenVT"] = tblNhapKho.Rows[i]["TenVT"];
                        _dr["MaVTMau"] = tblNhapKho.Rows[i]["MaVTMau"];
                        _dr["SoLuong"] = tblNhapKho.Rows[i]["SoLuong"];
                        _dr["MaMau"] = tblNhapKho.Rows[i]["MaMau"];
                        _dr["TenMau"] = tblNhapKho.Rows[i]["TenMau"];
                        _dr["CodeMau"] = tblNhapKho.Rows[i]["CodeMau"];
                        _dr["MaKhoSize"] = tblNhapKho.Rows[i]["MaKhoSize"];

                        _dr["KhoSize"] = tblNhapKho.Rows[i]["KhoSize"];
                        _dr["TongKien"] = tblNhapKho.Rows[i]["TongKien"];
                        _dr["Kien"] = tblNhapKho.Rows[i]["Kien"];
                        _dr["MaVTSP"] = tblNhapKho.Rows[i]["MaVTSP"];
                        _dr["MaViTri"] = tblNhapKho.Rows[i]["MaViTri"];
                        _dr["MaKho"] = tblNhapKho.Rows[i]["MaKho"];
                        _dr["MaDTKH"] = tblNhapKho.Rows[i]["MaDTKH"];
                        _dr["Lot"] = tblNhapKho.Rows[i]["Lot"];
                        _dr["AnhMau"] = tblNhapKho.Rows[i]["AnhMau"];
                        _dr["LoangMau"] = tblNhapKho.Rows[i]["LoangMau"];
                        _dr["GhiChu"] = tblNhapKho.Rows[i]["GhiChu"];
                        _dr["NgayNhap"] = tblNhapKho.Rows[i]["NgayNhap"];
                        _dr["NguoiNhap"] = tblNhapKho.Rows[i]["NguoiNhap"];
                        _dr["DotNhap"] = tblNhapKho.Rows[i]["DotNhap"];

                        // Thêm dòng vào DataTable
                        tblSave.Rows.Add(_dr);


                    }
                    if(tblSave.Rows.Count==0)
                    {
                        XtraMessageBox.Show("Vui lòng xác nhân nhập kho ít nhất một vật tư!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }    

                    string urlSaveBOMDH = string.Format("{0}?", URL + "VatTuNhapKho/Post");
                    string mssSaveBOMDH = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveBOMDH, tblSave); }).Result;
                    if (mssSaveBOMDH.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        tblSave.Clear();
                        tblNhapKho.Clear();
                        this.DialogResult = DialogResult.OK;

                    }
                    else
                    {
                        tblSave.Clear();
                        tblNhapKho.Clear();
                        this.DialogResult = DialogResult.OK;
                        return;
                    }
                    //XtraMessageBox.Show("Luu thành công");


                    /* if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                         DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);*/
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!" + ex, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadBarcode()
        {

            string urlGet = string.Format("{0}", URL + "VatTuNhapKho/Get");
            string jsonGet = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGet); }).Result;
            if (jsonGet == "[]") return;

            DataTable tblBarcode = JsonConvert.DeserializeObject<DataTable>(jsonGet);
            foreach (DataRow row in tblBarcode.Rows)
            {
                // Lấy giá trị từ cột "Barcode" và thêm vào danh sách
                string barcode = row["Barcode"].ToString();
                _lstBarcode.Add(barcode);
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

        private void txtSoVoice_KeyPress(object sender, KeyPressEventArgs e)
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
        //searchLookUpEdit_VatTu_EditValueChanged
    
        void searchLookUpEdit_VatTu_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            foreach (DataRowView rv in gridCheckMark.Selection)
            {
                if (sb.ToString().Length > 0) { sb.Append("; "); }
                sb.Append(rv["TenVT"].ToString());
            }
            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chưa chọn Màu----";
            }
            else
            {
                e.DisplayText = sb.ToString();
            }
            //e.DisplayText = sb.ToString();
        }
        void searchLookUpEdit_VatTu_SelectionChanged(object sender, EventArgs e)
        {
            //Control c = this.ActiveControl;
            Control c = FindSearchLookUpEditInLayout(this.ActiveControl);

            //if (c is DevExpress.XtraLayout.LayoutControl)
            //{
            //    if (!(((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl == null))
            //    {
            //        c = ((DevExpress.XtraLayout.LayoutControl)ActiveControl).ActiveControl;
            //    }
            //}
            if (c is DevExpress.XtraEditors.SearchLookUpEdit)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataRowView rv in (sender as SearchCheckSelection).Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append("; "); }
                    sb.Append(rv["MaVT"].ToString());
                }
                (c as DevExpress.XtraEditors.SearchLookUpEdit).Text = sb.ToString();
            }
        }
        private Control FindSearchLookUpEditInLayout(Control control)
        {
            // Kiểm tra null
            if (control == null)
                return null;

            // Nếu control hiện tại là SearchLookUpEdit, trả về nó
            if (control is DevExpress.XtraEditors.SearchLookUpEdit)
                return control;

            // Nếu control là LayoutControl, lặp qua các item của nó
            if (control is DevExpress.XtraLayout.LayoutControl layoutControl)
            {
                foreach (var item in layoutControl.Items)
                {
                    if (item is DevExpress.XtraLayout.LayoutControlItem layoutItem && layoutItem.Control != null)
                    {
                        // Tìm SearchLookUpEdit trong LayoutControlItem
                        var result = FindSearchLookUpEditInLayout(layoutItem.Control);
                        if (result != null)
                            return result;
                    }
                }
            }

            // Nếu control có ActiveControl, tiếp tục tìm
            if (control is ContainerControl containerControl && containerControl.ActiveControl != null)
            {
                return FindSearchLookUpEditInLayout(containerControl.ActiveControl);
            }

            // Không tìm thấy
            return null;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {

            BtSave();

        }

      
        private void btn_xoa_Click(object sender, EventArgs e)
        {
            DELETE();
        }
        private void DELETE()
        {
            DialogResult messResult = MessageBox.Show("Bạn có muốn xóa vật tư nhập kho này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messResult == DialogResult.Yes)
            {
                // Lấy ra dòng đang được focus
                int focusedRowIndex = gVNhapKho.FocusedRowHandle;

                if (focusedRowIndex >= 0)
                {
                    DataTable a = tblNhapKho;

                    // Lấy ra DataRow tương ứng với chỉ số dòng đang focus
                    DataRow row = gVNhapKho.GetDataRow(focusedRowIndex);

                    if (row != null)
                    {
                        // Xóa dòng này khỏi DataTable
                        tblNhapKho.Rows.Remove(row);
                    }
                }

                LoadDataSource();
            }
        }

        private void colBtn_Xoa_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DataRow row = gVNhapKho.GetFocusedDataRow();

            string _idDeleteRow = row["ID_DM"].ToString();
            DELETE();
        }
        //gridView1_CustomRowCellEdit
        
        private int FindRowIndexByName(DataTable _dtTemp, string name)
        {
            // Sử dụng LINQ để tìm index của hàng
            var rowIndex = _dtTemp.AsEnumerable()
                .Select((row, index) => new { Row = row, Index = index })
                .FirstOrDefault(item => item.Row["MaVtMau"].ToString() == name)?.Index ?? -1;

            return rowIndex;
        }

        private void gVNhapKho_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Location);

            // Kiểm tra nếu nhấp vào header của cột
            if (hitInfo.InColumn && hitInfo.Column != null && hitInfo.Column.FieldName == "XacNhan")
            {
                if (!tblNhapKho.Columns.Contains("XacNhan"))
                {
                    DataColumn col = new DataColumn("XacNhan", typeof(bool));
                    tblNhapKho.Columns.Add(col);
                    
                }
                if (!tblNhapKho.Columns.Contains("IsEdit"))
                {
                    DataColumn col = new DataColumn("IsEdit", typeof(int));
                    tblNhapKho.Columns.Add(col);

                }
                foreach (DataRow row in tblNhapKho.Rows)
                {
                    row["XacNhan"] = true;
                    row["IsEdit"] = 0;
                }

                gCNhapKho.DataSource = tblNhapKho;
               
            }

        }

        private void btnNapLai_Click(object sender, EventArgs e)
        {
            gCNhapKho.DataSource = tblImportExcel.Copy();
            //gCNhapKho.RefreshDataSource();
        }

        private void gVNhapKho_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            //GridView view = sender as GridView;

            //// Kiểm tra nếu cột không phải là "IsEdit"
            //if (e.Column.FieldName != "IsEdit" && oldValue != null)
            //{
            //    // Lấy giá trị cũ của hàng
            //    DataRow oldRow = oldValue as DataRow;

            //    // Kiểm tra sự khác biệt giữa giá trị cũ và mới của hàng hiện tại
            //    bool hasChanged = false;

            //    foreach (DataColumn column in oldRow.Table.Columns)
            //    {
            //        var oldCellValue = oldRow[column.ColumnName];
            //        var newCellValue = view.GetRowCellValue(e.RowHandle, column.ColumnName);

            //        if (!Equals(oldCellValue, newCellValue))
            //        {
            //            hasChanged = true;
            //            break;
            //        }
            //    }

            //    // Nếu có sự khác biệt, gán giá trị IsEdit = 2
            //    if (hasChanged)
            //    {
            //        view.SetRowCellValue(e.RowHandle, "IsEdit", 2);
            //    }
            //}
        }

        private void gVNhapKho_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;

            // Kiểm tra nếu hàng hiện tại là hàng dữ liệu
            if (e.RowHandle >= 0)
            {
                // Lấy giá trị của cột IsEdit
                var isEditValue = view.GetRowCellValue(e.RowHandle, "IsEdit");

                // Kiểm tra giá trị của IsEdit và thay đổi màu sắc của hàng
                if (isEditValue != null)
                {
                    int isEdit = Convert.ToInt32(isEditValue);

                    if (isEdit == 1)
                    {
                        e.Appearance.BackColor = Color.Green; // Màu xanh
                    }
                    else if (isEdit == 2)
                    {
                        e.Appearance.BackColor = Color.Yellow; // Màu vàng
                    }
                }
            }
        }

        private void gVNhapKho_RowUpdated(object sender, RowObjectEventArgs e)
        {
            GridView view = sender as GridView;
            // Duyệt qua từng dòng trong DataSource và so sánh với tblImportExcel
            for (int i = 0; i < tblImportExcel.Rows.Count; i++)
            {
                DataRow dataSourceRow = tblNhapKho.Rows[i];
                DataRow importExcelRow = tblImportExcel.Rows[i];

                // So sánh từng cột giá trị
                bool isDifferent = false;
                foreach (DataColumn column in tblImportExcel.Columns)
                {
                    if (!object.Equals(dataSourceRow[column.ColumnName], importExcelRow[column.ColumnName]))
                    {
                        isDifferent = true;
                        break;
                    }
                }

                // Nếu có sự khác biệt, set IsEdit của hàng đó thành 2
                if (isDifferent)
                {
                    view.SetRowCellValue(i, "isEdit", 2);
                    tblNhapKho.Rows[i]["IsEdit"] = 2;
                }
            }
            //GridView view = sender as GridView;

            //// Lấy hàng đã được cập nhật
            //DataRowView updatedRowView = e.Row as DataRowView;
            //if (updatedRowView == null) return;

            //DataRow updatedRow = updatedRowView.Row;

            //// Kiểm tra giá trị cũ
            //DataRow oldRow = oldValue as DataRow;
            //if (oldRow == null) return;

            //// Kiểm tra sự khác biệt giữa giá trị cũ và mới của hàng hiện tại
            //bool hasChanged = false;

            //foreach (DataColumn column in oldRow.Table.Columns)
            //{
            //    var oldCellValue = oldRow[column.ColumnName];
            //    var newCellValue = updatedRow[column.ColumnName];

            //    if (!Equals(oldCellValue, newCellValue))
            //    {
            //        hasChanged = true;
            //        break;
            //    }
            //}

            //// Nếu có sự khác biệt, gán giá trị IsEdit = 2
            //if (hasChanged)
            //{
            //    view.SetRowCellValue(view.FocusedRowHandle, "IsEdit", 2);
            //}
        }

        private void gVNhapKho_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;


            // Kiểm tra nếu có hàng nào được chọn
            if (view.FocusedRowHandle >= 0)
            {
                DataRow row = gVNhapKho.GetFocusedDataRow();
                oldValue = row;


            }
            DataRow row1 = gVNhapKho.GetFocusedDataRow();
            if (row1 == null) return;

            setTxtValueNhapKho(row1);


        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DialogResult messResult = MessageBox.Show("Bạn có muốn xóa lệnh này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messResult == DialogResult.Yes)
            {
                //gridView1.SetRowCellValue(0, colTesst, "Giá trị mới");

            }
        }

      
       
        private void gVNhapKho_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

      

     
       
    
       

      
       
        private void loadVatTuNhapKho(string _maVT)
        {
            string urlGet = string.Format("{0}?para2={1}", URL + "VatTuNhapKho/Get", _maVT);
            string jsonGet = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGet); }).Result;

            tblNhapKho = JsonConvert.DeserializeObject<DataTable>(jsonGet);
            if(tblNhapKho.Rows.Count==0)
            {
                CreateNewDataTable();
            }    

            if (!tblNhapKho.Columns.Contains("XacNhan"))
            {
                DataColumn col = new DataColumn("XacNhan", typeof(bool));
                tblNhapKho.Columns.Add(col);

                // Đánh dấu tất cả các hàng trong cột 'XacNhan' là true
                foreach (DataRow row in tblNhapKho.Rows)
                {
                    row["XacNhan"] = true; // Hoặc điều kiện logic để xác định giá trị
                }
            }
            gCNhapKho.DataSource = tblNhapKho;
        }

        private void loadDotNhap(string _maVT)
        {
            string urlDotNhap = string.Format("{0}?para2={1}", URL + "VatTuNhapKho/GetDotNhap", _maVT);
            string jsonDotNhap = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDotNhap); }).Result;
           
            if (jsonDotNhap != "[]")
            {
                DataTable tblDotNhap = JsonConvert.DeserializeObject<DataTable>(jsonDotNhap);
                dotnhap = int.TryParse(tblDotNhap.Rows[0]["DotNhap"].ToString(), out int dotNhapValue) ? dotNhapValue + 1 : dotnhap;

            }
        }
        //searchLookUpEdit_MauDH_EditValueChanged

      

        private void txtDot_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void Xoa_ItemClick(object sender, ItemClickEventArgs e)
        {
        }

        private void btn_Them_Click(object sender, EventArgs e)
        {
            try
            {


                int count = int.TryParse(txtTongKien.Text, out int temp) ? temp : 0;
                if (count == 0) count = 1;
                ThemDongHangLoat(count);


            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!" + ex, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ThemDongHangLoat(int count)
        {
            DataRow _rowFocus = gVNhapKho.GetFocusedDataRow();

            for (int i = 0; i < count; i++)
            {
                
               
                DataRow newRow = tblNhapKho.NewRow();

              

                loadDotNhap(_MaVT);
                string _makho = "", _maKH = "", _vitri = "";
                if (searchLookUpEditMaKho.EditValue != null)

                {
                    _makho = searchLookUpEditMaKho.EditValue.ToString();
                }
                if (searchLookUpEditKH.EditValue != null)
                {
                    _maKH = searchLookUpEditKH.EditValue.ToString();
                }
                if (searchLookUpEditViTri.EditValue != null)
                {
                    _vitri = searchLookUpEditViTri.EditValue.ToString();
                }
                newRow["ID"] = 0;
                newRow["MaVTNhapKho"] = _MaVT + DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
                newRow["MaDH"] = _Mahang;
                newRow["Barcode"] = CreateValueBarcode(_makho, txtTongKien.Text);
                newRow["MaVT"] = _MaVT;//cái này là mã phhair tự tìm
                newRow["TenVT"] = _TenVatTu;
                newRow["MaVTMau"] = _MaVTMau;
                newRow["SoLuong"] = txtSL.Text;
                newRow["MaMau"] = "";
                newRow["TenMau"] = _Mau;
                newRow["MaDVTinh"] = "";
                newRow["DVTinh"] = _DonViTinh;
                newRow["MaKhoSize"] = "";
                newRow["KhoSize"] =_KhoVai;
                newRow["TongKien"] = txtTongKien.Text;
                newRow["Kien"] = txtKien.Text + i;
                newRow["MaVTSP"] = _MaVT;
                newRow["MaViTri"] = _vitri;
                newRow["MaKho"] = _makho != "" ? _makho : "KH00";
                newRow["MaDTKH"] = _maKH;
                newRow["Lot"] = txtLot.Text;
                newRow["AnhMau"] = txtAnhMau.Text;
                newRow["LoangMau"] = txtLoangMau.Text; // Nhập vào loang màu
                newRow["GhiChu"] = txtGhiChu.Text; // Nhập vào ghi chú
                newRow["NgayNhap"] = dateNgayNhap.EditValue != null ? dateNgayNhap.EditValue.ToString() : DateTime.Now.ToString("dd-MM-yyyy"); // Tự tạo ngày nhập
                newRow["NguoiNhap"] = GlobleData.UserName; // Tự tạo người nhập từ môi trường hệ thống
                newRow["DotNhap"] = dotnhap;
                if (tblVatTuChiTiet.Rows.Count > 0)
                {
                    foreach (DataRow chiTietRow in tblVatTuChiTiet.Rows)
                    {
                        if (chiTietRow["MaVtMau"].ToString() == _MaVT.ToString())
                        {
                            //newRow["MaVT"] = chiTietRow["CodeVT"];
                            newRow["MaMau"] = chiTietRow["MaMau"];
                            newRow["MaKhoSize"] = chiTietRow["MaSizeKho"];

                            break; // Thoát vòng lặp nếu chỉ cần tìm dòng đầu tiên phù hợp
                        }
                    }
                }
                /* if(_rowFocus!=null)
                 {
                     newRow["SoLuong"] = _rowFocus["SoLuong"];

                     newRow["MaViTri"] = _rowFocus["MaViTri"];         
                     newRow["MaDTKH"] = _rowFocus["MaDTKH"];
                     newRow["Lot"] = _rowFocus["Lot"];
                     newRow["AnhMau"] = _rowFocus["AnhMau"];
                     newRow["LoangMau"] = _rowFocus["LoangMau"];
                     newRow["GhiChu"] = _rowFocus["GhiChu"];
                 }    */
                // Thêm dòng mới vào DataTable
                tblNhapKho.Rows.Add(newRow);

                // Làm mới và cập nhật lại nguồn dữ liệu cho GridView
                gCNhapKho.DataSource = tblNhapKho;
                gCNhapKho.RefreshDataSource();
            }
        }

        #region hàm phụ
        private string ReplaceSpecialCharactersAndRemoveSpaces(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            input = Regex.Replace(input, @"\s+", "");
            string result = Regex.Replace(input, @"[^a-zA-Z0-9]", "_");
            return result;
        }
        /*private void setTxtVatTuNull()
        {
            txtMaVT.Text = "";
            txtTenVT.Text = "";
            txtTenMau.Text = "";
            txtKhoSize.Text = "";

        }*/
        private void setTxtNhapKhoNull()
        {
            txtMaVTSP.Text = "";
            txtDVTinh.Text = "";
            searchLookUpEditKH.EditValue = "";
            txtTongKien.Text = "";
            searchLookUpEditViTri.EditValue = "";
            searchLookUpEditMaKho.EditValue = "";

            txtKien.Text = "";
            txtSL.Text = "";
            txtLot.Text = "";
            txtAnhMau.Text = "";
            txtLoangMau.Text = "";
            txtDotNhap.Text = "";
            txtMaVach.Text = "";
            picBarcode.Image = null;
        }

       
        private void setTxtValueNhapKho(DataRow row)
        {
            txtMaVT.Text = row["MaVT"].ToString();
            txtTenVT.Text = row["TenVT"].ToString();
            txtMaVTSP.Text = row["MaVTSP"].ToString();
            txtTenMau.Text = row["TenMau"].ToString();
            txtDVTinh.Text = row["DVTinh"].ToString();
            txtKhoSize.Text = row["KhoSize"].ToString();
            searchLookUpEditKH.EditValue = row["MaDTKH"].ToString();

            txtTongKien.Text = row["TongKien"].ToString();
            searchLookUpEditViTri.EditValue = row["MaViTri"].ToString();
            searchLookUpEditMaKho.EditValue = row["MaKho"].ToString();
            
            txtKien.Text = row["Kien"].ToString();
            txtSL.Text = row["SoLuong"].ToString();
            txtLot.Text = row["Lot"].ToString();
            txtAnhMau.Text = row["AnhMau"].ToString();
            txtLoangMau.Text = row["LoangMau"].ToString();
            txtDotNhap.Text = row["DotNhap"].ToString();
            txtMaVach.Text = row["BarCode"].ToString();
            //gán barcode

            // Gán hình ảnh cho PictureBox
            picBarcode.Image = CreateBarcodeBitmap(row["BarCode"].ToString());

        }
        private string CreateValueBarcode(string _makho, string _tongkien)
        {

            string _mavach;
            if (_lstBarcode.Count == 0)
            {
                _mavach = DateTime.Now.ToString("ddMMyy") + "0" + _tongkien + _makho +
                          _random.Next(0, 9) + _random.Next(0, 9) + _random.Next(0, 9) +
                          _random.Next(0, 9) + _random.Next(0, 9);
                _lstBarcode.Add(_mavach);
                return _mavach;
            }
            do
            {
                _mavach = DateTime.Now.ToString("ddMMyy") + "0" + _tongkien + _makho +
                          _random.Next(0, 9) + _random.Next(0, 9) + _random.Next(0, 9) +
                          _random.Next(0, 9) + _random.Next(0, 9);
            } while (_lstBarcode.Contains(_mavach));
            _lstBarcode.Add(_mavach);
            return _mavach;
        }
        private Bitmap CreateBarcodeBitmap(string barcode)
        {
            string barcodeValue = barcode;
            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions { Width = 60, Height = 60, Margin = 0, PureBarcode = true }
            };
            Bitmap barcodeBitmap = barcodeWriter.Write(barcodeValue);
            return barcodeBitmap;
        }
        #endregion

        private void BtNapLai()
        {

            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }

        private void Naplai_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            BtNapLai();
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }
    }
}
