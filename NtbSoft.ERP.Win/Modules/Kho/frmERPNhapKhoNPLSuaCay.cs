using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.Erp.Kho;
using NtbSoft.ERP.Win.Modules.ThuVien;
using NtbSoft.ERP.Win.Modules.ViTriKho;
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
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPNhapKhoNPLSuaCay : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private DataTable tblNL, tblPL, tblMau, tblNhapKho, tblPhieu, tblSearchNL, tblSearchPL;
        private DataTable tblNhapKhoPL, tblPhieuPL;
        private HashSet<DataRow> selectedRows = new HashSet<DataRow>();
        private HashSet<DataRow> selectedRowsPL = new HashSet<DataRow>();
        private DataTable tblTxt, tblChiTiet, tblChiTietPL;
        bool _isNPL = true;
        string _NL = "", _NLDisplay = "";
        string _PL = "", _PLDisplay = "";
        string _lo;
        HashSet<string> _lstMaHQ, _lstMaKT;
        private int _rowFocus;
        bool ischeckNL = true, ischeckPL = true;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _allowView = false;
        bool _allowAddPL = false, _allowEditPL = false, _allowDeletePL = false, _allowViewPL = false;
        bool _isTabNPL = true;
        string _soloid = string.Empty;
        public frmERPNhapKhoNPLSuaCay(string lo = "", bool isNPL = true)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _lo = lo;
            _isTabNPL = isNPL;

        }
        protected override void OnLoad(EventArgs e)
        {
            CheckPerminsion();
            Init();

            createSearchLookUpVatTu();
            //loadMauVT();
            CreateTableNhapKho();
            CreateTableNhapKhoPL();
            CreateTablePhieu();
            CreateTablePhieuPL();
            CreateTableChiTiet();
            CreateTableChiTietPL();

            CreateSearchLookUpTV();
            this.ActiveControl = simpleButton1;
            if (!_isTabNPL)
            {
                xtraTabControl1.SelectedTabPageIndex = 1;
            }
            int selectedTab = xtraTabControl1.SelectedTabPageIndex;
            if (selectedTab == 0)
            {
                CheckPerminsionNL();
            }
            else
            {
                CheckPerminsionPL();
            }

            if (!string.IsNullOrWhiteSpace(_lo))
            {
                if (_isTabNPL && tabNL.PageVisible)
                { searchLookUpEditLoNL.EditValue = _lo; }
                if (!_isTabNPL && tabPL.PageVisible)
                {
                    searchLookUpEditLoPL.EditValue = _lo;

                }

            }

            LoadIPAdress();
           
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}?userID={1}", URL + "PhanQuyenXacNhanBOM/GetUser", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<SystemUserXacNhanBomModel> listXN = JsonConvert.DeserializeObject<List<SystemUserXacNhanBomModel>>(json);
            if (listXN.Count == 0)
            {
                ischeckNL = false;
                ischeckPL = false;
                return;
            }
            foreach (SystemUserXacNhanBomModel item in listXN)
            {
                if (Convert.ToBoolean(item.isNPL))
                {
                    _allowAdd = item.AllowAdd;
                    _allowEdit = item.AllowEdit;
                    _allowDelete = item.AllowDelete;
                    _allowView = item.AllowView;
                }
                else
                {
                    _allowAddPL = item.AllowAdd;
                    _allowEditPL = item.AllowEdit;
                    _allowDeletePL = item.AllowDelete;
                    _allowViewPL = item.AllowView;
                }

            }
            CheckPerminsionNL();
            CheckPerminsionPL();

        }
        private void CheckPerminsionNL()
        {
            if (ischeckNL)
            {
                if (_allowView)
                {
                    tabNL.PageVisible = true;
                }
                else
                {
                    tabNL.PageVisible = false;
                }
                if (!_allowAdd)
                {

                    //searchLookUpEditNL.Properties.ReadOnly = true;
                }
                else
                {

                    //searchLookUpEditNL.Properties.ReadOnly = false;
                }


            }

        }
        private void CheckPerminsionPL()
        {
            if (ischeckPL)
            {
                if (_allowViewPL)
                {
                    tabPL.PageVisible = true;
                }
                else
                {
                    tabPL.PageVisible = false;
                }
                if (!_allowAddPL)
                {
                    //searchLookUpEditPL.Properties.ReadOnly = true;
                }
                else
                {
                    //searchLookUpEditPL.Properties.ReadOnly = false;
                }

            }

        }
        private void Init()
        {
            tblNL = new DataTable();
            tblPL = new DataTable();
            tblMau = new DataTable();
            tblNhapKho = new DataTable();
            tblPhieu = new DataTable();
            tblNhapKhoPL = new DataTable();
            tblPhieuPL = new DataTable();
            tblSearchNL = new DataTable();
            tblSearchPL = new DataTable();
            tblTxt = new DataTable();
            tblChiTiet = new DataTable();
            tblChiTietPL = new DataTable();
            _lstMaHQ = new HashSet<string>();
            _lstMaKT = new HashSet<string>();
            repoSearchLookUpEditMau.DisplayMember = "MauVT";
            repoSearchLookUpEditMau.ValueMember = "MaMauVT";


            searchLookUpEditVatTu.Properties.ValueMember = "ValueMember";
            searchLookUpEditVatTu.Properties.DisplayMember = "DisPlayMember";

            searchLookUpEditVatTuPL.Properties.ValueMember = "ValueMember";
            searchLookUpEditVatTuPL.Properties.DisplayMember = "DisPlayMember";

            initSearchTV();
        }
        private void initSearchTV()
        {
            searchLookUpEditNCC.Properties.ValueMember = "MaNhaCC";
            searchLookUpEditNCC.Properties.DisplayMember = "TenKH";

            searchLookUpEditNCCPL.Properties.ValueMember = "MaNhaCC";
            searchLookUpEditNCCPL.Properties.DisplayMember = "TenKH";

            searchLookUpEditCang.Properties.ValueMember = "MaCang";
            searchLookUpEditCang.Properties.DisplayMember = "TenCang";

            searchLookUpEditCangPL.Properties.ValueMember = "MaCang";
            searchLookUpEditCangPL.Properties.DisplayMember = "TenCang";

            searchLookUpEditMHCT.Properties.ValueMember = "MaHang";
            searchLookUpEditMHCT.Properties.DisplayMember = "TenHang";
            searchLookUpEditMHCTPL.Properties.ValueMember = "MaHang";
            searchLookUpEditMHCTPL.Properties.DisplayMember = "TenHang";
            searchLookUpEditKHCT.Properties.ValueMember = "MaKH";
            searchLookUpEditKHCT.Properties.DisplayMember = "TenKH";
            searchLookUpEditKHCTPL.Properties.ValueMember = "MaKH";
            searchLookUpEditKHCTPL.Properties.DisplayMember = "TenKH";

            repoSearchLookUpTienTe.ValueMember = "TienTeID";
            repoSearchLookUpTienTe.DisplayMember = "MaTienTe";
            repoSearchLookUpTienTePL.ValueMember = "TienTeID";
            repoSearchLookUpTienTePL.DisplayMember = "MaTienTe";
        }
        private void CreateSearchLookUpTV()
        {
            CreateSearchLookUpNCC();
            CreateSearchLookUpCang();
            CreateSearchLookUpTau();
            CreateSearchLookUpKHCT();
            CreateSearchLookUpTienTe();
        }
        private void CreateSearchLookUpTau()
        {
            string urlTau = $"{URL}NhaCC/GePMH?action=GETTAU&para1=&para2=&para3=&para4=&para5=";
            string jsonTau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTau); }).Result;
            tblTau = JsonConvert.DeserializeObject<DataTable>(jsonTau);

            SearchlookupTau.Properties.DataSource = tblTau;
            SearchlookupTauPL.Properties.DataSource = tblTau;
        }
        private void CreateSearchLookUpNCC()
        {
            try
            {
                string urlNCC = $"{URL}ERPNhapKhoNPL/Get?Action=GETNCC";
                string jsonNCC = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNCC); }).Result;
                if (jsonNCC == "[]")
                {
                    searchLookUpEditNCC.Properties.DataSource = null;
                    searchLookUpEditNCCPL.Properties.DataSource = null;
                }
                else
                {
                    DataTable tblNCC = JsonConvert.DeserializeObject<DataTable>(jsonNCC);
                    searchLookUpEditNCC.Properties.DataSource = tblNCC;
                    searchLookUpEditNCCPL.Properties.DataSource = tblNCC;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void CreateSearchLookUpKHCT()
        {
            try
            {
                string urlKH = $"{URL}ERPNhapKhoNPL/Get?Action=GETKH";
                string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
                if (jsonKH == "[]")
                {
                    searchLookUpEditKHCT.Properties.DataSource = null;
                    searchLookUpEditKHCTPL.Properties.DataSource = null;
                }
                else
                {
                    DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
                    searchLookUpEditKHCT.Properties.DataSource = tblKH;
                    searchLookUpEditKHCTPL.Properties.DataSource = tblKH;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void CreateSearchLookUpCang()
        {
            try
            {
                string urlCang = $"{URL}ERPThuVienNK/Get?Action=GETCANG";
                string jsonCang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCang); }).Result;
                if (jsonCang == "[]")
                {
                    searchLookUpEditCang.Properties.DataSource = null;
                    searchLookUpEditCangPL.Properties.DataSource = null;
                }
                else
                {
                    DataTable tblCang = JsonConvert.DeserializeObject<DataTable>(jsonCang);
                    searchLookUpEditCang.Properties.DataSource = tblCang;
                    searchLookUpEditCangPL.Properties.DataSource = tblCang;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void CreateSearchLookUpTienTe()
        {
            try
            {
                string urlTT = $"{URL}ERPNhapKhoNPL/Get?Action=GETTIENTE";
                string jsonTT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTT); }).Result;
                if (jsonTT == "[]")
                {
                    repoSearchLookUpTienTe.DataSource = null;
                    repoSearchLookUpTienTePL.DataSource = null;
                }
                else
                {
                    DataTable tblTT = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                    repoSearchLookUpTienTe.DataSource = tblTT;
                    repoSearchLookUpTienTePL.DataSource = tblTT;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void createSearchLookUpVatTu()
        {

            string url = $"{URL}ERPNhapKhoNPL/Get?Action=GETVTNL";
            if (!_isNPL)
                url = $"{URL}ERPNhapKhoNPL/Get?Action=GETVTPL";

            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                searchLookUpEditVatTu.Properties.DataSource = null;
                searchLookUpEditVatTuPL.Properties.DataSource = null;
                return;
            }


            DataTable tblMH = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditVatTu.Properties.DataSource = tblMH;
            searchLookUpEditVatTuPL.Properties.DataSource = tblMH;
            searchLookUpEditVatTu.EditValue = null;
            searchLookUpEditVatTuPL.EditValue = null;

        }
        #region tạo bảng 
        private DataTable CreateTableVatTu()
        {
            DataTable tbl = new DataTable();

            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaVT", typeof(string));
            tbl.Columns.Add("ChiTiet", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("MaMauVT", typeof(string));
            tbl.Columns.Add("MauVT", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("KhoVai", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("TenNhom", typeof(string));
            tbl.Columns.Add("Sort", typeof(string));
            tbl.Columns.Add("TongKien", typeof(int));
            tbl.Columns.Add("MaHaiQuan", typeof(string));
            tbl.Columns.Add("MaKeToan", typeof(string));
            tbl.Columns.Add("SoKienParent", typeof(string));
            tbl.Columns.Add("DonGia", typeof(decimal));
            tbl.Columns.Add("ThanhTien", typeof(decimal));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("TenDVVT", typeof(string));
            tbl.Columns.Add("SoLoID", typeof(string));
            tbl.Columns.Add("SoLo", typeof(string));
            tbl.Columns.Add("TienTe", typeof(string));
            tbl.Columns.Add("QuyDoiID", typeof(string));
            if (!tbl.Columns.Contains("MaVTGhep"))
            {
                tbl.Columns.Add("MaVTGhep", typeof(string));
            }
            tbl.Columns.Add("POMua", typeof(string));
            tbl.Columns.Add("Batch", typeof(string));
            tbl.Columns.Add("SLTong", typeof(decimal));
            tbl.Columns.Add("STTChonVT", typeof(int));
            tbl.Columns.Add("IsKiemKe", typeof(bool));
            tbl.Columns.Add("NgayKiemKe", typeof(DateTime));
            tbl.Columns.Add("TuoiTonKho", typeof(decimal));
            return tbl;

        }
        private void CreateTableChiTiet()
        {
            tblChiTiet = new DataTable("tblChiTiet");

            tblChiTiet.Columns.Add("ID", typeof(int));
            tblChiTiet.Columns.Add("SoLoID", typeof(string));
            tblChiTiet.Columns.Add("SoLo", typeof(string));
            tblChiTiet.Columns.Add("MaNPL", typeof(string));
            tblChiTiet.Columns.Add("MaVTID", typeof(string));
            tblChiTiet.Columns.Add("MaVT", typeof(string));
            tblChiTiet.Columns.Add("ChiTiet", typeof(string));
            tblChiTiet.Columns.Add("MauVTID", typeof(string));
            tblChiTiet.Columns.Add("MaMauVT", typeof(string));
            tblChiTiet.Columns.Add("MauVT", typeof(string));
            tblChiTiet.Columns.Add("SoKien", typeof(string));
            tblChiTiet.Columns.Add("SoLoT", typeof(string));
            tblChiTiet.Columns.Add("MaHaiQuan", typeof(string));
            tblChiTiet.Columns.Add("MaKeToan", typeof(string));
            tblChiTiet.Columns.Add("SoGhiDauCay", typeof(decimal));
            tblChiTiet.Columns.Add("NW", typeof(decimal));
            tblChiTiet.Columns.Add("GW", typeof(decimal));
            tblChiTiet.Columns.Add("BarCode", typeof(string));
            tblChiTiet.Columns.Add("GhiChu", typeof(string));
            tblChiTiet.Columns.Add("isNPL", typeof(bool));
            tblChiTiet.Columns.Add("KhoVaiID", typeof(string));
            tblChiTiet.Columns.Add("KhoVai", typeof(string));
            tblChiTiet.Columns.Add("SoKienParent", typeof(string));
            tblChiTiet.Columns.Add("SoLuongThucTe", typeof(decimal));
            tblChiTiet.Columns.Add("DonGia", typeof(decimal));
            tblChiTiet.Columns.Add("ThanhTien", typeof(decimal));
            tblChiTiet.Columns.Add("Pallet", typeof(string));
            tblChiTiet.Columns.Add("MaDVVT", typeof(string));
            tblChiTiet.Columns.Add("TenDVVT", typeof(string));
            tblChiTiet.Columns.Add("TrangThai", typeof(string));
            tblChiTiet.Columns.Add("IsNK", typeof(bool));
            tblChiTiet.Columns.Add("SoKienHienThi", typeof(string));
            tblChiTiet.Columns.Add("STT", typeof(string));
            tblChiTiet.Columns.Add("NgayNKDuKien", typeof(DateTime));
            tblChiTiet.Columns.Add("tileNW", typeof(decimal));
            tblChiTiet.Columns.Add("tileGW", typeof(decimal));
            tblChiTiet.Columns.Add("TienTe", typeof(string));
            tblChiTiet.Columns.Add("QuyDoiID", typeof(string));
            tblChiTiet.Columns.Add("POMua", typeof(string));
            tblChiTiet.Columns.Add("Batch", typeof(string));
            tblChiTiet.Columns.Add("SLTong", typeof(decimal));
            tblChiTiet.Columns.Add("STTChonVT", typeof(int));
            tblChiTiet.Columns.Add("IsKiemKe", typeof(bool));
            tblChiTiet.Columns.Add("NgayKiemKe", typeof(DateTime));
            tblChiTiet.Columns.Add("TuoiTonKho", typeof(decimal));
        }
        private void CreateTableChiTietPL()
        {
            tblChiTietPL = new DataTable("tblChiTietPL");

            tblChiTietPL.Columns.Add("ID", typeof(int));
            tblChiTietPL.Columns.Add("SoLoID", typeof(string));
            tblChiTietPL.Columns.Add("SoLo", typeof(string));
            tblChiTietPL.Columns.Add("MaNPL", typeof(string));
            tblChiTietPL.Columns.Add("MaVTID", typeof(string));
            tblChiTietPL.Columns.Add("MaVT", typeof(string));
            tblChiTietPL.Columns.Add("ChiTiet", typeof(string));
            tblChiTietPL.Columns.Add("MauVTID", typeof(string));
            tblChiTietPL.Columns.Add("MaMauVT", typeof(string));
            tblChiTietPL.Columns.Add("MauVT", typeof(string));
            tblChiTietPL.Columns.Add("SoKien", typeof(string));
            tblChiTietPL.Columns.Add("SoLoT", typeof(string));
            tblChiTietPL.Columns.Add("MaHaiQuan", typeof(string));
            tblChiTietPL.Columns.Add("MaKeToan", typeof(string));
            tblChiTietPL.Columns.Add("SoGhiDauCay", typeof(decimal));
            tblChiTietPL.Columns.Add("NW", typeof(decimal));
            tblChiTietPL.Columns.Add("GW", typeof(decimal));
            tblChiTietPL.Columns.Add("BarCode", typeof(string));
            tblChiTietPL.Columns.Add("GhiChu", typeof(string));
            tblChiTietPL.Columns.Add("isNPL", typeof(bool));
            tblChiTietPL.Columns.Add("KhoVaiID", typeof(string));
            tblChiTietPL.Columns.Add("KhoVai", typeof(string));
            tblChiTietPL.Columns.Add("SoKienParent", typeof(string));
            tblChiTietPL.Columns.Add("SoLuongThucTe", typeof(decimal));
            tblChiTietPL.Columns.Add("DonGia", typeof(decimal));
            tblChiTietPL.Columns.Add("ThanhTien", typeof(decimal));
            tblChiTietPL.Columns.Add("Pallet", typeof(string));
            tblChiTietPL.Columns.Add("MaDVVT", typeof(string));
            tblChiTietPL.Columns.Add("TenDVVT", typeof(string));
            tblChiTietPL.Columns.Add("TrangThai", typeof(string));
            tblChiTietPL.Columns.Add("IsNK", typeof(bool));
            tblChiTietPL.Columns.Add("SoKienHienThi", typeof(string));
            tblChiTietPL.Columns.Add("STT", typeof(string));
            tblChiTietPL.Columns.Add("NgayNKDuKien", typeof(DateTime));
            tblChiTietPL.Columns.Add("tileNW", typeof(decimal));
            tblChiTietPL.Columns.Add("tileGW", typeof(decimal));
            tblChiTietPL.Columns.Add("TienTe", typeof(string));
            tblChiTietPL.Columns.Add("QuyDoiID", typeof(string));
            tblChiTietPL.Columns.Add("POMua", typeof(string));
            tblChiTietPL.Columns.Add("Batch", typeof(string));
            tblChiTietPL.Columns.Add("SLTong", typeof(decimal));
            tblChiTietPL.Columns.Add("STTChonVT", typeof(int));
            tblChiTietPL.Columns.Add("IsKiemKe", typeof(bool));
            tblChiTietPL.Columns.Add("NgayKiemKe", typeof(DateTime));
            tblChiTietPL.Columns.Add("TuoiTonKho", typeof(decimal));
        }
        private void CreateTableNhapKho()
        {
            tblNhapKho = new DataTable("tblNhapKho");
            tblNhapKho.Columns.Add("ID", typeof(long));
            tblNhapKho.Columns.Add("SoLoID", typeof(string));
            tblNhapKho.Columns.Add("SoLo", typeof(string));
            tblNhapKho.Columns.Add("MaNPL", typeof(string));
            tblNhapKho.Columns.Add("MaVTID", typeof(string));
            tblNhapKho.Columns.Add("MaMauVT", typeof(string));
            tblNhapKho.Columns.Add("MauVT", typeof(string));
            tblNhapKho.Columns.Add("SoKien", typeof(string));
            tblNhapKho.Columns.Add("SoLoT", typeof(string));
            tblNhapKho.Columns.Add("MaHaiQuan", typeof(string));
            tblNhapKho.Columns.Add("MaKeToan", typeof(string));
            tblNhapKho.Columns.Add("SoGhiDauCay", typeof(decimal));
            tblNhapKho.Columns.Add("NW", typeof(decimal));
            tblNhapKho.Columns.Add("GW", typeof(decimal));
            tblNhapKho.Columns.Add("BarCode", typeof(string));
            tblNhapKho.Columns.Add("GhiChu", typeof(string));
            tblNhapKho.Columns.Add("IsNPL", typeof(bool));
            tblNhapKho.Columns.Add("KhoVaiID", typeof(string));
            tblNhapKho.Columns.Add("SoKienParent", typeof(string));
            tblNhapKho.Columns.Add("SoLuongThucTe", typeof(decimal));
            tblNhapKho.Columns.Add("DonGia", typeof(decimal));
            tblNhapKho.Columns.Add("ThanhTien", typeof(decimal));
            tblNhapKho.Columns.Add("Pallet", typeof(string));
            tblNhapKho.Columns.Add("MaDVVT", typeof(string));
            tblNhapKho.Columns.Add("MauVTID", typeof(string));
            tblNhapKho.Columns.Add("TrangThai", typeof(string));
            tblNhapKho.Columns.Add("IsNK", typeof(bool));
            tblNhapKho.Columns.Add("STT", typeof(int));
            tblNhapKho.Columns.Add("SoKienHienThi", typeof(string));
            tblNhapKho.Columns.Add("NgayNKDuKien", typeof(DateTime));
            tblNhapKho.Columns.Add("MaDVCD", typeof(string));
            tblNhapKho.Columns.Add("TenDVCD", typeof(string));
            tblNhapKho.Columns.Add("tileNW", typeof(decimal));
            tblNhapKho.Columns.Add("tileGW", typeof(decimal));
            tblNhapKho.Columns.Add("TienTe", typeof(string));
            tblNhapKho.Columns.Add("QuyDoiID", typeof(string));
            if (!tblNhapKho.Columns.Contains("MaVTGhep"))
            {
                tblNhapKho.Columns.Add("MaVTGhep", typeof(string));
            }
            if (!tblNhapKho.Columns.Contains("MaNhom"))
            {
                tblNhapKho.Columns.Add("MaNhom", typeof(string));
            }
            tblNhapKho.Columns.Add("POMua", typeof(string));
            tblNhapKho.Columns.Add("Batch", typeof(string));
            tblNhapKho.Columns.Add("SLTong", typeof(decimal));
            tblNhapKho.Columns.Add("STTChonVT", typeof(int));
            tblNhapKho.Columns.Add("IsKiemKe", typeof(bool));
            tblNhapKho.Columns.Add("NgayKiemKe", typeof(DateTime));
            tblNhapKho.Columns.Add("TuoiTonKho", typeof(decimal));

        }
        private void CreateTableNhapKhoPL()
        {
            tblNhapKhoPL = new DataTable("tblNhapKhoPL");
            tblNhapKhoPL.Columns.Add("ID", typeof(int));
            tblNhapKhoPL.Columns.Add("SoLoID", typeof(string));
            tblNhapKhoPL.Columns.Add("SoLo", typeof(string));
            tblNhapKhoPL.Columns.Add("MaNPL", typeof(string));
            tblNhapKhoPL.Columns.Add("MaVTID", typeof(string));
            tblNhapKhoPL.Columns.Add("MaMauVT", typeof(string));
            tblNhapKhoPL.Columns.Add("MauVT", typeof(string));
            tblNhapKhoPL.Columns.Add("SoKien", typeof(string));
            tblNhapKhoPL.Columns.Add("SoLot", typeof(string));
            tblNhapKhoPL.Columns.Add("MaHaiQuan", typeof(string));
            tblNhapKhoPL.Columns.Add("MaKeToan", typeof(string));
            tblNhapKhoPL.Columns.Add("SoGhiDauCay", typeof(decimal));
            tblNhapKhoPL.Columns.Add("NW", typeof(decimal));
            tblNhapKhoPL.Columns.Add("GW", typeof(decimal));
            tblNhapKhoPL.Columns.Add("BarCode", typeof(string));
            tblNhapKhoPL.Columns.Add("GhiChu", typeof(string));
            tblNhapKhoPL.Columns.Add("IsNPL", typeof(bool));
            tblNhapKhoPL.Columns.Add("KhoVaiID", typeof(string));
            tblNhapKhoPL.Columns.Add("SoKienParent", typeof(string));
            tblNhapKhoPL.Columns.Add("SoLuongThucTe", typeof(decimal));
            tblNhapKhoPL.Columns.Add("DonGia", typeof(decimal));
            tblNhapKhoPL.Columns.Add("ThanhTien", typeof(decimal));
            tblNhapKhoPL.Columns.Add("Pallet", typeof(string));
            tblNhapKhoPL.Columns.Add("MaDVVT", typeof(string));
            tblNhapKhoPL.Columns.Add("MauVTID", typeof(string));
            tblNhapKhoPL.Columns.Add("TrangThai", typeof(string));
            tblNhapKhoPL.Columns.Add("IsNK", typeof(bool));
            tblNhapKhoPL.Columns.Add("SoKienHienThi", typeof(string));
            tblNhapKhoPL.Columns.Add("STT", typeof(string));
            tblNhapKhoPL.Columns.Add("NgayNKDuKien", typeof(DateTime));
            tblNhapKhoPL.Columns.Add("MaDVCD", typeof(string));
            tblNhapKhoPL.Columns.Add("TenDVCD", typeof(string));

            tblNhapKhoPL.Columns.Add("tileNW", typeof(decimal));
            tblNhapKhoPL.Columns.Add("tileGW", typeof(decimal));
            tblNhapKhoPL.Columns.Add("TienTe", typeof(string));
            tblNhapKhoPL.Columns.Add("QuyDoiID", typeof(string));
            if (!tblNhapKhoPL.Columns.Contains("MaVTGhep"))
            {
                tblNhapKhoPL.Columns.Add("MaVTGhep", typeof(string));
            }
            if (!tblNhapKhoPL.Columns.Contains("MaNhom"))
            {
                tblNhapKhoPL.Columns.Add("MaNhom", typeof(string));
            }
            tblNhapKhoPL.Columns.Add("POMua", typeof(string));
            tblNhapKhoPL.Columns.Add("Batch", typeof(string));
            tblNhapKhoPL.Columns.Add("SLTong", typeof(decimal));
            tblNhapKhoPL.Columns.Add("STTChonVT", typeof(int));
            tblNhapKhoPL.Columns.Add("IsKiemKe", typeof(bool));
            tblNhapKhoPL.Columns.Add("NgayKiemKe", typeof(DateTime));
            tblNhapKhoPL.Columns.Add("TuoiTonKho", typeof(decimal));
        }

        private void CreateTablePhieu()
        {
            tblPhieu = new DataTable("tblPhieu");
            tblPhieu.Columns.Add("ID", typeof(long));
            tblPhieu.Columns.Add("SoLoID", typeof(string));
            tblPhieu.Columns.Add("SoLo", typeof(string));
            tblPhieu.Columns.Add("NhaCungCap", typeof(string));
            tblPhieu.Columns.Add("SoChungTu", typeof(string));
            tblPhieu.Columns.Add("NgayChungTu", typeof(DateTime));
            tblPhieu.Columns.Add("SoBienBan", typeof(string));
            tblPhieu.Columns.Add("NgayBienBan", typeof(DateTime));
            tblPhieu.Columns.Add("SoHopDong", typeof(string));
            tblPhieu.Columns.Add("NguoiNhap", typeof(string));
            tblPhieu.Columns.Add("NgayNhap", typeof(DateTime));
            tblPhieu.Columns.Add("IsNPL", typeof(bool));
            tblPhieu.Columns.Add("MaHaiQuan", typeof(string));
            tblPhieu.Columns.Add("MaKeToan", typeof(string));
            tblPhieu.Columns.Add("NguoiGui", typeof(string));
            tblPhieu.Columns.Add("NgayTao", typeof(DateTime));
            tblPhieu.Columns.Add("SoHoaDon", typeof(string));
            tblPhieu.Columns.Add("Tau", typeof(string));
            tblPhieu.Columns.Add("NgayGui", typeof(DateTime));
            tblPhieu.Columns.Add("NoiGui", typeof(string));
            tblPhieu.Columns.Add("DiemDen", typeof(string));
            tblPhieu.Columns.Add("NguoiNhan", typeof(string));
            tblPhieu.Columns.Add("DiaChiNhan", typeof(string));
            tblPhieu.Columns.Add("MaSoThue", typeof(string));
            tblPhieu.Columns.Add("SoDienThoai", typeof(string));
            tblPhieu.Columns.Add("Cang", typeof(string));
            tblPhieu.Columns.Add("KhachHang", typeof(string));
            tblPhieu.Columns.Add("NgayKyHD", typeof(DateTime));
            tblPhieu.Columns.Add("SoToKhai", typeof(string));
            tblPhieu.Columns.Add("NgayMoTK", typeof(DateTime));
            tblPhieu.Columns.Add("SoVanDon", typeof(string));
            tblPhieu.Columns.Add("NgayNKDuKien", typeof(DateTime));
            tblPhieu.Columns.Add("MaKH", typeof(string));
            tblPhieu.Columns.Add("NgayHoaDon", typeof(DateTime));
            tblPhieu.Columns.Add("MaHang", typeof(string));
            tblPhieu.Columns.Add("POMua", typeof(string));
            tblPhieu.Columns.Add("NW", typeof(decimal));
            tblPhieu.Columns.Add("GW", typeof(decimal));
            tblPhieu.Columns.Add("QCKiem", typeof(int));
            tblPhieu.Columns.Add("IsKiemKe", typeof(bool));
            tblPhieu.Columns.Add("NgayKiemKe", typeof(DateTime));
            tblPhieu.Columns.Add("PDF", typeof(string));
            tblPhieu.Columns.Add("E_Way_Bill", typeof(string));
        }
        private void CreateTablePhieuPL()
        {
            tblPhieuPL = new DataTable("tblPhieuPL");
            tblPhieuPL.Columns.Add("ID", typeof(long));
            tblPhieuPL.Columns.Add("SoLoID", typeof(string));
            tblPhieuPL.Columns.Add("SoLo", typeof(string));
            tblPhieuPL.Columns.Add("NhaCungCap", typeof(string));
            tblPhieuPL.Columns.Add("SoChungTu", typeof(string));
            tblPhieuPL.Columns.Add("NgayChungTu", typeof(DateTime));
            tblPhieuPL.Columns.Add("SoBienBan", typeof(string));
            tblPhieuPL.Columns.Add("NgayBienBan", typeof(DateTime));
            tblPhieuPL.Columns.Add("SoHopDong", typeof(string));
            tblPhieuPL.Columns.Add("NguoiNhap", typeof(string));
            tblPhieuPL.Columns.Add("NgayNhap", typeof(DateTime));
            tblPhieuPL.Columns.Add("IsNPL", typeof(bool));
            tblPhieuPL.Columns.Add("MaHaiQuan", typeof(string));
            tblPhieuPL.Columns.Add("MaKeToan", typeof(string));
            tblPhieuPL.Columns.Add("NguoiGui", typeof(string));
            tblPhieuPL.Columns.Add("NgayTao", typeof(DateTime));
            tblPhieuPL.Columns.Add("SoHoaDon", typeof(string));
            tblPhieuPL.Columns.Add("Tau", typeof(string));
            tblPhieuPL.Columns.Add("NgayGui", typeof(DateTime));
            tblPhieuPL.Columns.Add("NoiGui", typeof(string));
            tblPhieuPL.Columns.Add("DiemDen", typeof(string));
            tblPhieuPL.Columns.Add("NguoiNhan", typeof(string));
            tblPhieuPL.Columns.Add("DiaChiNhan", typeof(string));
            tblPhieuPL.Columns.Add("MaSoThue", typeof(string));
            tblPhieuPL.Columns.Add("SoDienThoai", typeof(string));
            tblPhieuPL.Columns.Add("Cang", typeof(string));
            tblPhieuPL.Columns.Add("KhachHang", typeof(string));
            tblPhieuPL.Columns.Add("NgayKyHD", typeof(DateTime));
            tblPhieuPL.Columns.Add("SoToKhai", typeof(string));
            tblPhieuPL.Columns.Add("NgayMoTK", typeof(DateTime));
            tblPhieuPL.Columns.Add("SoVanDon", typeof(string));
            tblPhieuPL.Columns.Add("NgayNKDuKien", typeof(DateTime));
            tblPhieuPL.Columns.Add("MaKH", typeof(string));
            tblPhieuPL.Columns.Add("NgayHoaDon", typeof(DateTime));
            tblPhieuPL.Columns.Add("MaHang", typeof(string));
            tblPhieuPL.Columns.Add("POMua", typeof(string));
            tblPhieuPL.Columns.Add("NW", typeof(decimal));
            tblPhieuPL.Columns.Add("GW", typeof(decimal));
            tblPhieuPL.Columns.Add("QCKiem", typeof(int));
            tblPhieuPL.Columns.Add("IsKiemKe", typeof(bool));
            tblPhieuPL.Columns.Add("NgayKiemKe", typeof(DateTime));
            tblPhieuPL.Columns.Add("PDF", typeof(string));
            tblPhieuPL.Columns.Add("E_Way_Bill", typeof(string));

        }
        #endregion



        private void loadTxt(bool isNL)
        {
            if ((isNL ? searchLookUpEditLoNL : searchLookUpEditLoPL).EditValue == null) return;
            string url = $"{URL}ERPNhapKhoNPL/Get?Action=GETNK&para={(isNL ? searchLookUpEditLoNL : searchLookUpEditLoPL).EditValue.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblTxt = JsonConvert.DeserializeObject<DataTable>(json);
            if (tblTxt == null || tblTxt.Rows.Count == 0) return;
            _soloid = tblTxt.Rows[0]["SoLoID"].ToString();
            if (isNL)
            {
                searchLookUpEditNCC.EditValue = tblTxt.Rows[0]["NhaCungCap"].ToString();
                txtSoToKhai.Text = tblTxt.Rows[0]["SoToKhai"].ToString();
                dateNgayToKhai.EditValue = tblTxt.Rows[0]["NgayMoTK"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayMoTK"]) : null;
                txtSoHopDong.Text = tblTxt.Rows[0]["SoHopDong"].ToString();
                dateSoHopDong.EditValue = tblTxt.Rows[0]["NgayKyHD"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayKyHD"]) : null;
                txtMaSoThue.Text = tblTxt.Rows[0]["MaSoThue"].ToString();
                txtSoHoaDon.Text = tblTxt.Rows[0]["SoHoaDon"].ToString();
                searchLookUpEditCang.EditValue = tblTxt.Rows[0]["Cang"].ToString();
                SearchlookupTau.EditValue = tblTxt.Rows[0]["MaTau"]?.ToString();
                dateNgayGui.EditValue = tblTxt.Rows[0]["NgayGui"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayGui"]) : null;
                txtNoiGui.Text = tblTxt.Rows[0]["NoiGui"].ToString();
                txtPOMua.Text = tblTxt.Rows[0]["POMua"].ToString();
                txtQCKiem.Text = tblTxt.Rows[0]["QCKiemDisPlay"].ToString();
                if (txtQCKiem.Text == "PASS")
                {
                    txtQCKiem.ForeColor = Color.Green;
                }
                else if (txtQCKiem.Text == "FAIL")
                {
                    txtQCKiem.ForeColor = Color.Red;
                }
                else
                {
                    txtQCKiem.ForeColor = Color.Black;
                }
                txtNW.Text = tblTxt.Rows[0]["NW"].ToString();
                txtGW.Text = tblTxt.Rows[0]["GW"].ToString();
                txtSoVanDon.Text = tblTxt.Rows[0]["SoVanDon"].ToString();
                dateNgayNK.EditValue = tblTxt.Rows[0]["NgayNKDuKien"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayNKDuKien"]) : null;
            }
            else
            {
                searchLookUpEditNCCPL.EditValue = tblTxt.Rows[0]["NhaCungCap"].ToString();
                txtSoToKhaiPL.Text = tblTxt.Rows[0]["SoToKhai"].ToString();
                dateNgayToKhaiPL.EditValue = tblTxt.Rows[0]["NgayMoTK"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayMoTK"]) : null;
                txtSoHopDongPL.Text = tblTxt.Rows[0]["SoHopDong"].ToString();
                dateSoHopDongPL.EditValue = tblTxt.Rows[0]["NgayKyHD"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayKyHD"]) : null;
                txtMaSoThuePL.Text = tblTxt.Rows[0]["MaSoThue"].ToString();
                txtSoHoaDonPL.Text = tblTxt.Rows[0]["SoHoaDon"].ToString();
                searchLookUpEditCangPL.EditValue = tblTxt.Rows[0]["Cang"].ToString();
                SearchlookupTauPL.EditValue = tblTxt.Rows[0]["MaTau"].ToString();
                dateNgayGuiPL.EditValue = tblTxt.Rows[0]["NgayGui"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayGui"]) : null;
                txtNoiGuiPL.Text = tblTxt.Rows[0]["NoiGui"].ToString();
                txtPOMuaPL.Text = tblTxt.Rows[0]["POMua"].ToString();
                txtNWPL.Text = tblTxt.Rows[0]["NW"].ToString();
                txtGWPL.Text = tblTxt.Rows[0]["GW"].ToString();
                txtQCKiemPL.Text = tblTxt.Rows[0]["QCKiemDisPlay"].ToString();
                if (txtQCKiemPL.Text == "PASS")
                {
                    txtQCKiemPL.ForeColor = Color.Green;
                }
                else if (txtQCKiemPL.Text == "FAIL")
                {
                    txtQCKiemPL.ForeColor = Color.Red;
                }
                else
                {
                    txtQCKiemPL.ForeColor = Color.Black;
                }
                txtSoVanDonPL.Text = tblTxt.Rows[0]["SoVanDon"].ToString();
                dateNgayNKPL.EditValue = tblTxt.Rows[0]["NgayNKDuKien"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayNKDuKien"]) : null;
            }
        }

        private void loadGVVatTu(bool isNL)
        {
            try
            {
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang tải dữ liệu");
                DataRow _r = (isNL ? gVSearchVT : gVSearchVTPL).GetFocusedDataRow();
                if (_r == null)
                {
                    SplashScreenManager.CloseForm(false);
                    return;
                }

                var MaVTID = _r["MaVTID"]?.ToString();
                var MauVTID = _r["MauVTID"]?.ToString();
                var KhoVaiID = _r["KhoVaiID"]?.ToString();
                var MaNhom = _r["MaNhom"]?.ToString(); // Lấy MaNhom từ dòng focus
                var MaVTGhep = _r["MaVTGhep"]?.ToString();

                // Xây dựng URL với thêm MaNhom
                string url = $"{URL}ERPNhapKhoNPL/Get?Action=GETSUACAYVAI&para={MaVTID}&para2={MauVTID}&para3={KhoVaiID}&para4={MaNhom}&para5={MaVTGhep}";

                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    (isNL ? gCNL : gCPL).DataSource = null;
                    (isNL ? gCNLNhapKho : gCPLNhapKho).DataSource = null;
                    SplashScreenManager.CloseForm(false);
                    return;
                }

                tblChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblChiTiet == null || tblChiTiet.Rows.Count == 0)
                {
                    (isNL ? gCNL : gCPL).DataSource = null;
                    (isNL ? gCNLNhapKho : gCPLNhapKho).DataSource = null;
                    SplashScreenManager.CloseForm(false);
                    return;
                }

                // Khởi tạo các bảng nếu chưa có

                if (isNL) CreateTableNhapKho();
                else CreateTableNhapKhoPL();

                if ((isNL ? tblNL : tblPL) == null || (isNL ? tblNL : tblPL).Rows.Count == 0)
                {
                    if (isNL) tblNL = CreateTableVatTu();
                    else tblPL = CreateTableVatTu();
                }

                // Xóa dữ liệu cũ
                (isNL ? tblNL : tblPL).Clear();
                (isNL ? tblNhapKho : tblNhapKhoPL).Clear();

                // Sử dụng LINQ để nhóm dữ liệu và gán STT theo nhóm
                var groupedData = tblChiTiet.AsEnumerable()
                    .GroupBy(dr => new
                    {
                        MaVTID = dr["MaVTID"],
                        MaNhom = dr["MaNhom"],
                        MauVTID = dr["MauVTID"],
                        KhoVaiID = dr["KhoVaiID"],
                        SoLoID = dr["SoLoID"]
                    })
                    .ToList();

                var currentTblNLPL = isNL ? tblNL : tblPL;
                var currentTblNhapKho = isNL ? tblNhapKho : tblNhapKhoPL;

                foreach (var group in groupedData)
                {
                    int sttTrongNhom = 1; // Reset STT cho mỗi nhóm

                    DataRow firstRowInGroup = group.First();
                    object groupMaVTID = firstRowInGroup["MaVTID"];
                    object groupMaNhom = firstRowInGroup["MaNhom"];
                    object groupMauVTID = firstRowInGroup["MauVTID"];
                    object groupKhoVaiID = firstRowInGroup["KhoVaiID"];
                    object groupSoLoID = firstRowInGroup["SoLoID"];

                    // Kiểm tra trùng lắp cho tblNL/tblPL với tất cả các trường nhóm
                    bool isDuplicate = (isNL ? tblNL : tblPL).AsEnumerable().Any(r =>
                        (r.Field<object>("MaVTID") ?? DBNull.Value).Equals(groupMaVTID ?? DBNull.Value) &&
                        (r.Field<object>("MaNhom") ?? DBNull.Value).Equals(groupMaNhom ?? DBNull.Value) &&
                        (r.Field<object>("MauVTID") ?? DBNull.Value).Equals(groupMauVTID ?? DBNull.Value) &&
                        (r.Field<object>("KhoVaiID") ?? DBNull.Value).Equals(groupKhoVaiID ?? DBNull.Value) &&
                        (r.Field<object>("SoLoID") ?? DBNull.Value).Equals(groupSoLoID ?? DBNull.Value)
                    );

                    if (!isDuplicate)
                    {
                        DataRow row1 = (isNL ? tblNL : tblPL).NewRow();

                        row1["MaVTID"] = firstRowInGroup["MaVTID"];
                        row1["MaVT"] = firstRowInGroup["MaVT"];
                        row1["ChiTiet"] = firstRowInGroup["ChiTiet"];
                        row1["MaNhom"] = firstRowInGroup["MaNhom"];
                        row1["TenNhom"] = firstRowInGroup["TenNhom"];
                        row1["Sort"] = firstRowInGroup["Sort"];
                        row1["MaMauVT"] = firstRowInGroup["MaMauVT"];
                        row1["MauVT"] = firstRowInGroup["MauVT"];
                        row1["KhoVaiID"] = firstRowInGroup["KhoVaiID"];
                        row1["KhoVai"] = firstRowInGroup["KhoVai"];
                        row1["MaHaiQuan"] = firstRowInGroup["MaHaiQuan"];
                        row1["POMua"] = firstRowInGroup["POMua"];
                        row1["MaKeToan"] = firstRowInGroup["MaKeToan"];
                        row1["DonGia"] = firstRowInGroup["DonGia"];
                        row1["MaDVVT"] = firstRowInGroup["MaDVVT"];
                        row1["TenDVVT"] = firstRowInGroup["TenDVVT"];
                        row1["MauVTID"] = firstRowInGroup["MauVTID"];
                        row1["SoLoID"] = firstRowInGroup["SoLoID"];
                        row1["SoLo"] = firstRowInGroup["SoLo"];
                        row1["TienTe"] = firstRowInGroup["TienTe"];
                        row1["QuyDoiID"] = firstRowInGroup["QuyDoiID"];
                        if (!_lstMaHQ.Contains(firstRowInGroup["MaHaiQuan"]?.ToString()))
                        {
                            _lstMaHQ.Add(firstRowInGroup["MaHaiQuan"].ToString());
                        }
                        if (!_lstMaKT.Contains(firstRowInGroup["MaKeToan"]?.ToString()))
                        {
                            _lstMaKT.Add(firstRowInGroup["MaKeToan"].ToString());
                        }

                        row1["MaVTGhep"] = firstRowInGroup["MaVTGhep"];
                        row1["SLTong"] = firstRowInGroup["SLTong"];
                        row1["IsKiemKe"] = firstRowInGroup["IsKiemKe"];
                        row1["NgayKiemKe"] = firstRowInGroup["NgayKiemKe"];
                        row1["STTChonVT"] = firstRowInGroup["STTChonVT"];
                        row1["TuoiTonKho"] = firstRowInGroup["TuoiTonKho"];
                        (isNL ? tblNL : tblPL).Rows.Add(row1);
                    }

                    foreach (DataRow dr in group)
                    {
                        DataRow row2 = (isNL ? tblNhapKho : tblNhapKhoPL).NewRow();

                        row2["ID"] = dr["ID"];
                        row2["SoLoID"] = dr["SoLoID"];
                        row2["MaNPL"] = dr["MaNPL"];
                        row2["MaVTID"] = dr["MaVTID"];
                        row2["MaMauVT"] = dr["MaMauVT"];
                        row2["MauVT"] = dr["MauVT"];
                        row2["SoKien"] = dr["SoKien"];
                        row2["SoLot"] = dr["SoLot"];
                        row2["MaHaiQuan"] = dr["MaHaiQuan"];
                        row2["Batch"] = dr["Batch"];
                        row2["MaKeToan"] = dr["MaKeToan"];
                        row2["SoGhiDauCay"] = dr["SoGhiDauCay"];
                        row2["NW"] = dr["NW"];
                        row2["GW"] = dr["GW"];
                        row2["BarCode"] = dr["BarCode"];
                        row2["GhiChu"] = dr["GhiChu"];
                        row2["IsNPL"] = dr["IsNPL"];
                        row2["KhoVaiID"] = dr["KhoVaiID"];
                        row2["SoKienParent"] = dr["SoKienParent"];
                        row2["SoLuongThucTe"] = dr["SoLuongThucTe"];
                        row2["DonGia"] = dr["DonGia"];
                        row2["ThanhTien"] = dr["ThanhTien"];
                        row2["Pallet"] = dr["Pallet"];
                        row2["MaDVVT"] = dr["MaDVVT"];
                        row2["MauVTID"] = dr["MauVTID"];
                        row2["TrangThai"] = dr["TrangThai"];
                        row2["IsNK"] = dr["IsNK"];
                        row2["SoKienHienThi"] = dr["SoKienHienThi"];
                        row2["STT"] = dr["STTChonVT"];
                        row2["STTChonVT"] = dr["STTChonVT"];
                        //row2["NgayNKDuKien"] = Convert.ToDateTime(dr["NgayNKDuKien"]).ToString("dd-MM-yyyy") == "" ? Convert.ToDateTime(DateTime.Now).ToString("dd-MM-yyyy") : Convert.ToDateTime(dr["NgayNKDuKien"]).ToString("dd-MM-yyyy");
                        DateTime ngay;
                        if (dr["NgayNKDuKien"] == DBNull.Value ||
                            !DateTime.TryParseExact(dr["NgayNKDuKien"].ToString(),
                                                    "dd-MM-yyyy",
                                                    System.Globalization.CultureInfo.InvariantCulture,
                                                    System.Globalization.DateTimeStyles.None,
                                                    out ngay))
                        {
                            ngay = DateTime.Now;
                        }
                        row2["NgayNKDuKien"] = dr["NgayNKDuKien"];
                        row2["tileNW"] = dr["tileNW"];
                        row2["tileGW"] = dr["tileGW"];
                        row2["MaDVCD"] = dr["MaDVCD"];
                        row2["TenDVCD"] = dr["TenDVCD"];
                        row2["MaVTGhep"] = dr["MaVTGhep"];
                        row2["MaNhom"] = dr["MaNhom"];
                        row2["QuyDoiID"] = dr["QuyDoiID"];
                        row2["SLTong"] = dr["SLTong"];
                        row2["IsKiemKe"] = dr["IsKiemKe"];
                        row2["NgayKiemKe"] = dr["NgayKiemKe"];
                        row2["TuoiTonKho"] = dr["TuoiTonKho"];
                        row2["SoLo"] = dr["SoLo"];
                        (isNL ? tblNhapKho : tblNhapKhoPL).Rows.Add(row2);
                        sttTrongNhom++; // Tăng STT cho dòng tiếp theo trong cùng nhóm
                    }
                }

               // Gán DataSource cho GridControl
               (isNL ? gCNL : gCPL).DataSource = (isNL ? tblNL : tblPL);
                (isNL ? gCNLNhapKho : gCPLNhapKho).DataSource = (isNL ? tblNhapKho : tblNhapKhoPL);

                // Tối ưu hóa BestFitColumns
                BestFitCol(isNL ? gVNL : gVPL);

                checkSearchLookUpVatTu(isNL);
                setTongKien(isNL);

                var gridView = isNL ? gVNL : gVPL;
                if (gridView.DataRowCount > 1)
                {
                    gridView.FocusedRowHandle = 1;
                    gridView.FocusedRowHandle = 0;
                }
                else if (gridView.DataRowCount > 0)
                {
                    gridView.FocusedRowHandle = 0;
                }
                SplashScreenManager.CloseForm(false);
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);

            }


        }

        private void checkSearchLookUpVatTu(bool isNL)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }

        }
        private void setTongKien(bool isNL)
        {
            try
            {
                DataTable tbl = (isNL ? gCNL : gCPL).DataSource as DataTable;
                foreach (DataRow row in tbl.Rows)
                {
                    int _tk = 0;
                    decimal _tt = 0;
                    foreach (DataRow dr in (isNL ? tblNhapKho : tblNhapKhoPL).Rows)
                    {
                        //#region Thoai
                        //bool isEmptyRow = string.IsNullOrWhiteSpace(dr["SoLoT"]?.ToString());

                        //if (isEmptyRow) continue;
                        //#endregion
                        if (row["MaVTID"].ToString() == dr["MaVTID"].ToString() && row["SoLoID"].ToString() == dr["SoLoID"].ToString() && row["MaNhom"].ToString() == dr["MaNhom"].ToString() &&
                            row["MauVTID"].ToString() == dr["MauVTID"].ToString() && row["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString()

                            )
                        {
                            var barCode = dr.Field<string>("BarCode");
                            if (!string.IsNullOrWhiteSpace(barCode))
                            {
                                var segments = barCode.Split('|');
                                if (segments.Length > 0 && !string.IsNullOrWhiteSpace(segments[segments.Length - 1]))
                                {
                                    _tk++;
                                }
                            }
                            decimal thanhTien = 0m;
                            decimal.TryParse(dr["ThanhTien"]?.ToString(), out thanhTien);
                            _tt += thanhTien;
                        }

                    }
                    row["TongKien"] = _tk;
                    row["ThanhTien"] = _tt;
                }
            }
            catch (Exception ex)
            {


            }
        }
        private bool checkTxt()
        {
            var controla = (_isNPL ? searchLookUpEditLoNL : searchLookUpEditLoPL);
            if (controla.EditValue == null || string.IsNullOrWhiteSpace(controla.Text))
            {
                MessageBox.Show("Vui lòng chọn PI NCC.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                (_isNPL ? searchLookUpEditLoNL : searchLookUpEditLoPL).Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace((_isNPL ? txtPOMua : txtPOMuaPL).Text))
            {
                MessageBox.Show("Vui lòng nhập PO Mua.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                (_isNPL ? txtPOMua : txtPOMuaPL).Focus();
                return false;
            }
            //var control = _isNPL ? searchLookUpEditKHCT : searchLookUpEditKHCTPL;
            //if (control.EditValue == null || string.IsNullOrWhiteSpace(control.Text))
            //{
            //    MessageBox.Show("Vui lòng chọn khách hàng CT.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    control.Focus();
            //    return false;
            //}

            if (string.IsNullOrWhiteSpace((_isNPL ? dateNgayNK : dateNgayNKPL).Text))
            {
                MessageBox.Show("Vui lòng chọn Ngày nhâp.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                (_isNPL ? dateNgayNK : dateNgayNKPL).Focus();
                return false;
            }
            return true;
        }
        #region sự kiện lưới
        private void gVNL_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "DonGia")
            {
                tinhThanhTien(true);
                ganLaiThanhTien(true);
                ganLaiDonGia(true);
            }
            else if (e.Column.FieldName == "TongKien")
            {
                object value = gVNL.GetRowCellValue(e.RowHandle, e.Column);

                int tongKien;
                if (value != null && int.TryParse(value.ToString(), out tongKien))
                {
                    DataTable _dtSK = (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource as DataTable;
                    int count = _dtSK.Rows.Count;
                    if (tongKien < count)
                    {
                        MessageBox.Show($"Tổng kiện phải lớn hơn hoặc bằng {count}!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                }
            }
            else if (e.Column.FieldName == "MaHaiQuan")
            {
                object value = gVNL.GetRowCellValue(e.RowHandle, e.Column);
                DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (dr == null) return;
                string maVTID = dr["MaVTID"].ToString();
                string MauVTID = dr["MauVTID"].ToString();
                string khoVaiID = dr["KhoVaiID"].ToString();
                string MaNhom = dr["MaNhom"].ToString();
                string SoLoID = dr["SoLoID"].ToString();
                foreach (DataRow _dr in tblNhapKho.Rows)
                {
                    if (_dr["MaVTID"].ToString() == maVTID && _dr["MauVTID"].ToString() == MauVTID && _dr["KhoVaiID"].ToString() == khoVaiID && _dr["MaNhom"].ToString() == MaNhom && _dr["SoLoID"].ToString() == SoLoID)
                    {
                        _dr["MaHaiQuan"] = value.ToString();
                    }
                }
                DataTable tblNhapKhoFiltered = (_isNPL ? tblNhapKho : tblNhapKhoPL).Clone();
                var filteredRows = (_isNPL ? tblNhapKho : tblNhapKhoPL).AsEnumerable()
                    .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                  row.Field<string>("MauVTID") == MauVTID &&
                                  row.Field<string>("KhoVaiID") == khoVaiID &&
                                  row.Field<string>("MaNhom") == MaNhom);
                foreach (var row in filteredRows)
                {
                    tblNhapKhoFiltered.ImportRow(row);
                }

                (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource = tblNhapKhoFiltered;
            }
            else if (e.Column.FieldName == "MaKeToan")
            {
                object value = gVNL.GetRowCellValue(e.RowHandle, e.Column);
                DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (dr == null) return;
                string maVTID = dr["MaVTID"].ToString();
                string MauVTID = dr["MauVTID"].ToString();
                string khoVaiID = dr["KhoVaiID"].ToString();
                string MaNhom = dr["MaNhom"].ToString();
                string SoLoID = dr["SoLoID"].ToString();
                foreach (DataRow _dr in tblNhapKho.Rows)
                {
                    if (_dr["MaVTID"].ToString() == maVTID && _dr["MauVTID"].ToString() == MauVTID && _dr["KhoVaiID"].ToString() == khoVaiID && _dr["MaNhom"].ToString() == MaNhom && _dr["SoLoID"].ToString() == SoLoID)
                    {
                        _dr["MaKeToan"] = value.ToString();
                    }
                }
                DataTable tblNhapKhoFiltered = (_isNPL ? tblNhapKho : tblNhapKhoPL).Clone();
                var filteredRows = (_isNPL ? tblNhapKho : tblNhapKhoPL).AsEnumerable()
                    .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                  row.Field<string>("MauVTID") == MauVTID &&
                                  row.Field<string>("KhoVaiID") == khoVaiID &&
                                  row.Field<string>("MaNhom") == MaNhom);
                foreach (var row in filteredRows)
                {
                    tblNhapKhoFiltered.ImportRow(row);
                }

                (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource = tblNhapKhoFiltered;
            }
            else if (e.Column.FieldName == "TienTe")
            {
                DataRow drF = gVNL.GetFocusedDataRow();
                if (drF == null) return;
                string maVTID = drF["MaVTID"].ToString();
                string MauVTID = drF["MauVTID"].ToString();
                string khoVaiID = drF["KhoVaiID"].ToString();
                string MaNhom = drF["MaNhom"].ToString();
                string SoLoID = drF["SoLoID"].ToString();
                foreach (DataRow _dr in tblNhapKho.Rows)
                {
                    if (_dr["MaVTID"].ToString() == maVTID && _dr["MauVTID"].ToString() == MauVTID && _dr["KhoVaiID"].ToString() == khoVaiID && _dr["MaNhom"].ToString() == MaNhom && _dr["SoLoID"].ToString() == SoLoID)
                    {
                        _dr["TienTe"] = drF["TienTe"].ToString();
                    }
                }


                DataTable tbl = gCNLNhapKho.DataSource as DataTable;
                if (tbl != null && tbl.Rows.Count != 0)
                {
                    foreach (DataRow dr in tbl.Rows)
                    {
                        dr["TienTe"] = drF["TienTe"].ToString();

                    }
                }


            }
            else if (e.Column.FieldName == "SLTong")
            {
                {
                    DataRow drF = gVNL.GetFocusedDataRow();
                    if (drF == null) return;
                    DataTable tbl = gCNLNhapKho.DataSource as DataTable;
                    if (tbl == null || tbl.Rows.Count == 0) return;
                    foreach (DataRow dr in tbl.Rows)
                    {
                        dr["SLTong"] = drF["SLTong"].ToString();

                    }

                    string maVTID_F = drF["MaVTID"].ToString();
                    string mauVTID_F = drF["MauVTID"].ToString();
                    string khoVaiID_F = drF["KhoVaiID"].ToString();
                    string maNhom_F = drF["MaNhom"].ToString();
                    string maVTGhep_F = drF["MaVTGhep"].ToString();
                    string SoLoID = drF["SoLoID"].ToString();
                    string SLTong_F = drF["SLTong"].ToString();


                    var rowsToUpdate = tblNhapKho.AsEnumerable()
                         .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                        dr["MauVTID"].ToString() == mauVTID_F &&
                                        dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                        dr["MaNhom"].ToString() == maNhom_F &&
                                        dr["SoLoID"].ToString() == SoLoID);

                    foreach (var row in rowsToUpdate)
                    {
                        row["SLTong"] = SLTong_F;
                    }

                }

            }
            else if (e.Column.FieldName == "NgayKiemKe")
            {
                {
                    DataRow drF = gVNL.GetFocusedDataRow();
                    if (drF == null) return;
                    DataTable tbl = gCNLNhapKho.DataSource as DataTable;
                    if (tbl == null || tbl.Rows.Count == 0) return;
                    foreach (DataRow dr in tbl.Rows)
                    {
                        dr["NgayKiemKe"] = drF["NgayKiemKe"].ToString();

                    }

                    string maVTID_F = drF["MaVTID"].ToString();
                    string mauVTID_F = drF["MauVTID"].ToString();
                    string khoVaiID_F = drF["KhoVaiID"].ToString();
                    string maNhom_F = drF["MaNhom"].ToString();
                    string maVTGhep_F = drF["MaVTGhep"].ToString();
                    string solo_F = drF["SoLoID"].ToString();
                    string NgayKiemKe_F = drF["NgayKiemKe"].ToString();


                    var rowsToUpdate = tblNhapKho.AsEnumerable()
                         .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                        dr["MauVTID"].ToString() == mauVTID_F &&
                                        dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                        dr["MaNhom"].ToString() == maNhom_F &&
                                        dr["SoLoID"].ToString() == solo_F);

                    foreach (var row in rowsToUpdate)
                    {
                        row["NgayKiemKe"] = NgayKiemKe_F;
                    }
                }
            }
            else if (e.Column.FieldName == "TuoiTonKho")
            {
                DataRow drF = gVNL.GetFocusedDataRow();
                if (drF == null) return;
                DataTable tbl = gCNLNhapKho.DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0) return;
                string maVTID_F = drF["MaVTID"].ToString();
                string mauVTID_F = drF["MauVTID"].ToString();
                string khoVaiID_F = drF["KhoVaiID"].ToString();
                string maNhom_F = drF["MaNhom"].ToString();
                string solo_F = drF["SoLoID"].ToString();
                string maVTGhep_F = drF["MaVTGhep"].ToString();
                string tuoi_F = drF["TuoiTonKho"].ToString();

                var rowsToUpdate = tblNhapKho.AsEnumerable()
                     .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                  dr["MauVTID"].ToString() == mauVTID_F &&
                                  dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                  dr["MaNhom"].ToString() == maNhom_F &&
                                        dr["SoLoID"].ToString() == solo_F);

                foreach (var row in rowsToUpdate)
                {
                    row["TuoiTonKho"] = tuoi_F;
                }
            }
        }
        private void gVPL_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "DonGia")
            {
                tinhThanhTien(false);
                ganLaiThanhTien(false);
                ganLaiDonGia(false);

            }
            else if (e.Column.FieldName == "TongKien")
            {
                object value = gVPL.GetRowCellValue(e.RowHandle, e.Column);

                int tongKien;
                if (value != null && int.TryParse(value.ToString(), out tongKien))
                {
                    DataTable _dtSK = (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource as DataTable;
                    int count = _dtSK.Rows.Count;
                    if (tongKien < count)
                    {
                        MessageBox.Show($"Tổng kiện phải lớn hơn hoặc bằng {count}!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                }
            }
            else if (e.Column.FieldName == "MaHaiQuan")
            {
                object value = gVPL.GetRowCellValue(e.RowHandle, e.Column);
                DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (dr == null) return;
                string maVTID = dr["MaVTID"].ToString();
                string MauVTID = dr["MauVTID"].ToString();
                string khoVaiID = dr["KhoVaiID"].ToString();
                string MaNhom = dr["MaNhom"].ToString();
                string SoLoID = dr["SoLoID"].ToString();
                foreach (DataRow _dr in tblNhapKhoPL.Rows)
                {
                    if (_dr["MaVTID"].ToString() == maVTID && _dr["MauVTID"].ToString() == MauVTID && _dr["KhoVaiID"].ToString() == khoVaiID && _dr["MaNhom"].ToString() == MaNhom && _dr["SoLoID"].ToString() == SoLoID)
                    {
                        _dr["MaHaiQuan"] = value.ToString();
                    }
                }
                DataTable tblNhapKhoFiltered = (_isNPL ? tblNhapKho : tblNhapKhoPL).Clone();
                var filteredRows = (_isNPL ? tblNhapKho : tblNhapKhoPL).AsEnumerable()
                    .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                  row.Field<string>("MauVTID") == MauVTID &&
                                  row.Field<string>("KhoVaiID") == khoVaiID &&
                                  row.Field<string>("MaNhom") == MaNhom);
                foreach (var row in filteredRows)
                {
                    tblNhapKhoFiltered.ImportRow(row);
                }

              (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource = tblNhapKhoFiltered;
            }
            else if (e.Column.FieldName == "MaKeToan")
            {
                object value = gVPL.GetRowCellValue(e.RowHandle, e.Column);
                DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (dr == null) return;
                string maVTID = dr["MaVTID"].ToString();
                string MauVTID = dr["MauVTID"].ToString();
                string khoVaiID = dr["KhoVaiID"].ToString();
                string MaNhom = dr["MaNhom"].ToString();
                string SoLoID = dr["SoLoID"].ToString();
                foreach (DataRow _dr in tblNhapKhoPL.Rows)
                {
                    if (_dr["MaVTID"].ToString() == maVTID && _dr["MauVTID"].ToString() == MauVTID && _dr["KhoVaiID"].ToString() == khoVaiID && _dr["MaNhom"].ToString() == MaNhom && _dr["SoLoID"].ToString() == SoLoID)
                    {
                        _dr["MaKeToan"] = value.ToString();
                    }
                }
                DataTable tblNhapKhoFiltered = (_isNPL ? tblNhapKho : tblNhapKhoPL).Clone();
                var filteredRows = (_isNPL ? tblNhapKho : tblNhapKhoPL).AsEnumerable()
                    .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                  row.Field<string>("MauVTID") == MauVTID &&
                                  row.Field<string>("KhoVaiID") == khoVaiID &&
                                  row.Field<string>("MaNhom") == MaNhom);
                foreach (var row in filteredRows)
                {
                    tblNhapKhoFiltered.ImportRow(row);
                }

                (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource = tblNhapKhoFiltered;
            }
            else if (e.Column.FieldName == "TienTe")
            {
                DataRow drF = gVPL.GetFocusedDataRow();
                if (drF == null) return;

                string maVTID = drF["MaVTID"].ToString();
                string MauVTID = drF["MauVTID"].ToString();
                string khoVaiID = drF["KhoVaiID"].ToString();
                string MaNhom = drF["MaNhom"].ToString();
                string SoLoID = drF["SoLoID"].ToString();
                foreach (DataRow _dr in tblNhapKhoPL.Rows)
                {
                    if (_dr["MaVTID"].ToString() == maVTID && _dr["MauVTID"].ToString() == MauVTID && _dr["KhoVaiID"].ToString() == khoVaiID && _dr["MaNhom"].ToString() == MaNhom && _dr["SoLoID"].ToString() == SoLoID)
                    {
                        _dr["TienTe"] = drF["TienTe"].ToString();
                    }
                }


                DataTable tbl = gCPLNhapKho.DataSource as DataTable;
                if (tbl != null && tbl.Rows.Count != 0)
                {
                    foreach (DataRow dr in tbl.Rows)
                    {
                        dr["TienTe"] = drF["TienTe"].ToString();

                    }
                }


            }
            else if (e.Column.FieldName == "TuoiTonKho")
            {
                DataRow drF = gVPL.GetFocusedDataRow();
                if (drF == null) return;
                DataTable tbl = gCPLNhapKho.DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0) return;
                string maVTID_F = drF["MaVTID"].ToString();
                string mauVTID_F = drF["MauVTID"].ToString();
                string khoVaiID_F = drF["KhoVaiID"].ToString();
                string maNhom_F = drF["MaNhom"].ToString();
                string maVTGhep_F = drF["MaVTGhep"].ToString();
                string tuoi_F = drF["TuoiTonKho"].ToString();
                string SoLoID = drF["SoLoID"].ToString();
                var rowsToUpdate = tblNhapKhoPL.AsEnumerable()
                     .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                  dr["MauVTID"].ToString() == mauVTID_F &&
                                  dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                  dr["MaNhom"].ToString() == maNhom_F &&
                                  dr["SoLoID"].ToString() == SoLoID);

                foreach (var row in rowsToUpdate)
                {
                    row["TuoiTonKho"] = tuoi_F;
                }
            }
            #region Thoai
            else if (e.Column.FieldName == "SLTong")
            {
                {
                    DataRow drF = gVPL.GetFocusedDataRow();
                    if (drF == null) return;
                    DataTable tbl = gCPLNhapKho.DataSource as DataTable;
                    if (tbl == null || tbl.Rows.Count == 0) return;
                    foreach (DataRow dr in tbl.Rows)
                    {
                        dr["SLTong"] = drF["SLTong"].ToString();

                    }

                    string maVTID_F = drF["MaVTID"].ToString();
                    string mauVTID_F = drF["MauVTID"].ToString();
                    string khoVaiID_F = drF["KhoVaiID"].ToString();
                    string maNhom_F = drF["MaNhom"].ToString();
                    string maVTGhep_F = drF["MaVTGhep"].ToString();
                    string SLTong_F = drF["SLTong"].ToString();
                    string SoLoID = drF["SoLoID"].ToString();

                    var rowsToUpdate = tblNhapKhoPL.AsEnumerable()
                         .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                        dr["MauVTID"].ToString() == mauVTID_F &&
                                        dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                        dr["MaNhom"].ToString() == maNhom_F &&
                                        dr["SoLoID"].ToString() == SoLoID);

                    foreach (var row in rowsToUpdate)
                    {
                        row["SLTong"] = SLTong_F;
                    }

                }

            }
            else if (e.Column.FieldName == "NgayKiemKe")
            {
                {
                    DataRow drF = gVPL.GetFocusedDataRow();
                    if (drF == null) return;
                    DataTable tbl = gCPLNhapKho.DataSource as DataTable;
                    if (tbl == null || tbl.Rows.Count == 0) return;
                    foreach (DataRow dr in tbl.Rows)
                    {
                        dr["NgayKiemKe"] = drF["NgayKiemKe"].ToString();

                    }

                    string maVTID_F = drF["MaVTID"].ToString();
                    string mauVTID_F = drF["MauVTID"].ToString();
                    string khoVaiID_F = drF["KhoVaiID"].ToString();
                    string maNhom_F = drF["MaNhom"].ToString();
                    string maVTGhep_F = drF["MaVTGhep"].ToString();
                    string NgayKiemKe_F = drF["NgayKiemKe"].ToString();


                    var rowsToUpdate = tblNhapKho.AsEnumerable()
                         .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                        dr["MauVTID"].ToString() == mauVTID_F &&
                                        dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                        dr["MaNhom"].ToString() == maNhom_F &&
                                        dr["MaVTGhep"].ToString() == maVTGhep_F);

                    foreach (var row in rowsToUpdate)
                    {
                        row["NgayKiemKe"] = NgayKiemKe_F;
                    }
                }
            }
            #endregion
        }
        private void ganLaiDonGia(bool isNL)
        {
            if (isNL)
            {
                DataRow dr = gVNL.GetFocusedDataRow();
                foreach (DataRow _row in tblNhapKho.Rows)
                {
                    if (dr["MaVTID"].ToString() == _row["MaVTID"].ToString() && dr["MauVTID"].ToString() == _row["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == _row["KhoVaiID"].ToString() && dr["MaNhom"].ToString() == _row["MaNhom"].ToString())
                    {
                        _row["DonGia"] = dr["DonGia"];
                    }
                }
            }
            else
            {
                DataRow dr = gVPL.GetFocusedDataRow();
                foreach (DataRow _row in tblNhapKhoPL.Rows)
                {
                    if (dr["MaVTID"].ToString() == _row["MaVTID"].ToString() && dr["MauVTID"].ToString() == _row["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == _row["KhoVaiID"].ToString() && dr["MaNhom"].ToString() == _row["MaNhom"].ToString())
                    {
                        _row["DonGia"] = dr["DonGia"];
                    }
                }
            }
        }
        private void ganLaiThanhTien(bool isNL)
        {
            try
            {
                if (isNL)
                {
                    DataRow row = gVNL.GetFocusedDataRow();
                    decimal _dg = Convert.ToDecimal(row["DonGia"].ToString());
                    if (_dg <= 0) return;
                    foreach (DataRow dr in tblNhapKho.Rows)
                    {

                        if (dr["MaVTID"].ToString() == row["MaVTID"].ToString() && dr["MauVTID"].ToString() == row["MauVTID"].ToString() && dr["MaNhom"].ToString() == row["MaNhom"].ToString() && dr["KhoVaiID"].ToString() == row["KhoVaiID"].ToString())
                        {
                            decimal _sltt = Convert.ToDecimal(dr["SoLuongThucTe"].ToString());
                            decimal _tt = _dg * _sltt;
                            dr["ThanhTien"] = _tt;
                        }

                    }



                }
                else
                {
                    DataRow row = gVPL.GetFocusedDataRow();
                    decimal _dg = Convert.ToDecimal(row["DonGia"].ToString());
                    if (_dg <= 0) return;
                    foreach (DataRow dr in tblNhapKhoPL.Rows)
                    {


                        if (dr["MaVTID"].ToString() == row["MaVTID"].ToString() && dr["MauVTID"].ToString() == row["MauVTID"].ToString() && dr["MaNhom"].ToString() == row["MaNhom"].ToString() && dr["KhoVaiID"].ToString() == row["KhoVaiID"].ToString())
                        {
                            decimal _sltt = Convert.ToDecimal(dr["SoLuongThucTe"].ToString());
                            decimal _tt = _dg * _sltt;
                            dr["ThanhTien"] = _tt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {


            }
        }
        private void gVNLNhapKho_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gVNLNhapKho.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {
                        DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", XoaDongNK);
                        e.Menu.Items.Add(menuDeleteItem);
                        var selectedRows = gVNLNhapKho.GetSelectedRows();
                        foreach (var rowHandle in selectedRows)
                        {
                            DataRow row = gVNLNhapKho.GetDataRow(rowHandle);
                            bool _isEdit = checkEdit(row);
                            if (!_isEdit)
                            {
                                return;
                            }

                        }
                        DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy", CopyCell);
                        e.Menu.Items.Add(menuCopyItem);
                        DevExpress.Utils.Menu.DXMenuItem menuPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Paste", PasteCell);
                        e.Menu.Items.Add(menuPasteItem);

                        //DevExpress.Utils.Menu.DXMenuItem menuThemLoTItem = new DevExpress.Utils.Menu.DXMenuItem("Thêm LOT", ThemLoT);
                        //e.Menu.Items.Add(menuThemLoTItem);

                        //DevExpress.Utils.Menu.DXMenuItem menuDoiDVItem = new DevExpress.Utils.Menu.DXMenuItem("Đổi đơn vị", DoiDV);
                        //e.Menu.Items.Add(menuDoiDVItem);

                        //DevExpress.Utils.Menu.DXMenuItem menunhapKLItem = new DevExpress.Utils.Menu.DXMenuItem("Nhập khối lượng", nhapKhoiLuong);
                        //e.Menu.Items.Add(menunhapKLItem);

                    }

                }
            }
        }



        private void XoaDongNK(object sender, EventArgs e)
        {
            try
            {
                DataRow drF = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (drF == null) return;
                string maVTID = drF["MaVTID"].ToString();
                string mauVTID = drF["MauVTID"].ToString();
                string khoVaiID = drF["KhoVaiID"].ToString();
                string MaNhom = drF["MaNhom"].ToString();
                string SoLoID = drF["SoLoID"].ToString();
                _rowFocus = (_isNPL ? gVNL : gVPL).FocusedRowHandle;
                gVNL.FocusedRowChanged -= gVNL_FocusedRowChanged;
                gVPL.FocusedRowChanged -= gVPL_FocusedRowChanged;

                // Lấy GridView và DataTable tương ứng theo loại
                DevExpress.XtraGrid.Views.Grid.GridView gridView = _isNPL ? gVNLNhapKho : gVPLNhapKho;
                DataTable tbl = (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource as DataTable;
                DataTable tblSource = _isNPL ? tblNhapKho : tblNhapKhoPL;

                // Lấy các dòng được chọn
                int[] selectedRowHandles = gridView.GetSelectedRows();
                if (selectedRowHandles.Length == 0) return;
                DialogResult dialog = XtraMessageBox.Show(
                    $"Nếu xóa có thể gây sai sót dữ liệu!. Bạn có chắc chắn muốn xóa {selectedRowHandles.Length} dòng đã chọn không?\nLưu ý: người dùng hoàn toàn chịu trách nhiệm về hành vi này!!!",
                    "Cảnh báo trước khi xóa dữ liệu",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (dialog != DialogResult.Yes)
                {
                    return;
                }
                // Danh sách dòng sẽ xóa
                var rowsToDeleteFromTbl = new List<DataRow>();

                // Thu thập các dòng cần xóa
                foreach (int rowHandle in selectedRowHandles)
                {
                    if (!gridView.IsValidRowHandle(rowHandle)) continue;
                    DataRow dr = gridView.GetDataRow(rowHandle);
                    if (dr == null) continue;
                    rowsToDeleteFromTbl.Add(dr);
                }

                string ids = string.Join(";",
rowsToDeleteFromTbl.Select(r => r["BarCode"].ToString())
);
                string urlXH = $"{URL}ERPNhapKhoNPL/Get?Action=GETXHDELETE&para={ids.ToString()}&para2={GlobleData.UserName}";
                string resultXH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlXH); }).Result;
                if (resultXH != "[]")
                {
                    MessageBox.Show("Vật tư đã được xuất hàng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Thực hiện xóa trên nguồn dữ liệu
                foreach (DataRow dr in rowsToDeleteFromTbl)
                {
                    // Xóa trên nguồn tblSource
                    DataRow[] rowsToDelete = tblSource.AsEnumerable()
                        .Where(row =>
                            row.Field<string>("MaVTID") == dr["MaVTID"].ToString() &&
                            row.Field<string>("MauVTID") == dr["MauVTID"].ToString() &&
                            row.Field<string>("KhoVaiID") == dr["KhoVaiID"].ToString() &&
                            row.Field<string>("SoKienHienThi") == dr["SoKienHienThi"].ToString() &&
                            row.Field<string>("MaNhom") == dr["MaNhom"].ToString() &&
                            row.Field<string>("SoLoID") == dr["SoLoID"].ToString())
                        .ToArray();

                    foreach (DataRow row in rowsToDelete)
                    {
                        if (row["ID"] != "0")
                        {
                            string url = $"{URL}ERPNhapKhoNPL/Delete?Action=DELETECT&para={row["ID"].ToString()}&para2={GlobleData.UserName}";
                            string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                            if (result.ToLower() == "true")
                                continue;
                        }
                        tblSource.Rows.Remove(row);
                    }

                    // Xóa trên bảng đang hiển thị
                    tbl.Rows.Remove(dr);
                }

                // Cập nhật lại DataSource và refresh
                (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource = tbl;
                gridView.RefreshData();
                clsWaitForm.ShowSuccessForm(this, 1000);

                // Load lại grid vật tư, gắn lại sự kiện
                if (_isNPL)
                {
                    loadGVVatTu(true);
                    gVNL.FocusedRowChanged += gVNL_FocusedRowChanged;
                    //if (_rowFocus > 0)
                    //    gVNL.FocusedRowHandle = _rowFocus;

                    if (drF == null && tblNL.Rows.Count > 0)
                    {
                        drF = tblNL.Rows[0];
                    }
                    if (tblNhapKho == null || tblNhapKho.Rows.Count == 0) CreateTableNhapKho();

                    string filter = $"MaVTID = {maVTID} AND MauVTID = {mauVTID} AND KhoVaiID = {khoVaiID} AND MaNhom = {MaNhom}";
                    DataRow[] foundRows = tblNhapKho.Select(filter);

                    if (foundRows.Length > 0)
                    {

                        DataTable tblNhapKhoFiltered = tblNhapKho.Clone();
                        var filteredRows = tblNhapKho.AsEnumerable()
                        .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                      row.Field<string>("MauVTID") == mauVTID &&
                                      row.Field<string>("KhoVaiID") == khoVaiID &&
                                      row.Field<string>("MaNhom") == MaNhom);
                        foreach (var row in filteredRows)
                        {
                            tblNhapKhoFiltered.ImportRow(row);
                        }


                        gCNLNhapKho.DataSource = tblNhapKhoFiltered;
                    }
                    else
                    {
                        DataRow dr = tblNL.Rows[0];

                        DataTable tblNhapKhoFiltered = tblNhapKho.Clone();
                        var filteredRows = tblNhapKho.AsEnumerable()
                        .Where(row => row.Field<string>("MaVTID") == dr["MaVTID"].ToString() &&
                                      row.Field<string>("MauVTID") == dr["MauVTID"].ToString() &&
                                      row.Field<string>("KhoVaiID") == dr["KhoVaiID"].ToString() &&
                                      row.Field<string>("MaNhom") == dr["MaNhom"].ToString());
                        foreach (var row in filteredRows)
                        {
                            tblNhapKhoFiltered.ImportRow(row);
                        }
                        gCNLNhapKho.DataSource = tblNhapKhoFiltered;
                    }
                }
                else
                {
                    loadGVVatTu(false);
                    gVPL.FocusedRowChanged += gVPL_FocusedRowChanged;
                    if (drF == null && tblPL.Rows.Count > 0)
                    {
                        drF = tblPL.Rows[0];
                    }
                    if (tblNhapKhoPL == null || tblNhapKhoPL.Rows.Count == 0) CreateTableNhapKhoPL();
                    string filter = $"MaVTID = {maVTID} AND MauVTID = {mauVTID} AND KhoVaiID = {khoVaiID} AND MaNhom ={MaNhom}";
                    DataRow[] foundRows = tblNhapKhoPL.Select(filter);

                    if (foundRows.Length > 0)
                    {

                        DataTable tblNhapKhoFiltered = tblNhapKhoPL.Clone();
                        var filteredRows = tblNhapKhoPL.AsEnumerable()
                        .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                      row.Field<string>("MauVTID") == mauVTID &&
                                      row.Field<string>("KhoVaiID") == khoVaiID &&
                                      row.Field<string>("MaNhom") == MaNhom);
                        foreach (var row in filteredRows)
                        {
                            tblNhapKhoFiltered.ImportRow(row);
                        }


                        gCPLNhapKho.DataSource = tblNhapKhoFiltered;
                    }
                    else
                    {
                        DataRow dr = tblPL.Rows[0];

                        DataTable tblNhapKhoFiltered = tblNhapKhoPL.Clone();
                        var filteredRows = tblNhapKhoPL.AsEnumerable()
                        .Where(row => row.Field<string>("MaVTID") == dr["MaVTID"].ToString() &&
                                      row.Field<string>("MauVTID") == dr["MauVTID"].ToString() &&
                                      row.Field<string>("KhoVaiID") == dr["KhoVaiID"].ToString() &&
                                      row.Field<string>("MaNhom") == dr["MaNhom"].ToString());
                        foreach (var row in filteredRows)
                        {
                            tblNhapKhoFiltered.ImportRow(row);
                        }


                        gCPLNhapKho.DataSource = tblNhapKhoFiltered;
                    }

                }

                resetTongKien();
            }
            catch (Exception ex)
            {

            }
        }

        private void XoaDongVT(object sender, EventArgs e)
        {



        }
        private void gVNLNhapKho_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            var gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (gridView == null) return;
            gridView.BeginUpdate();
            try
            {
                DataRowView changedRowView = gridView.GetRow(e.RowHandle) as DataRowView;
                if (changedRowView != null)
                {
                    DataRow changedDataRow = changedRowView.Row;
                    var targetRow = tblNhapKho.AsEnumerable().FirstOrDefault(row =>
                        row["MaVTID"].ToString() == changedDataRow["MaVTID"].ToString() &&
                        row["MaMauVT"].ToString() == changedDataRow["MaMauVT"].ToString() &&
                        row["KhoVaiID"].ToString() == changedDataRow["KhoVaiID"].ToString() &&
                        row["STT"].ToString() == changedDataRow["STT"].ToString() &&
                        row["MaNhom"].ToString() == changedDataRow["MaNhom"].ToString() &&
                        row["SoLoID"].ToString() == changedDataRow["SoLoID"].ToString()
                    );

                    if (targetRow != null)
                    {
                        if (changedDataRow.Table.Columns.Contains(e.Column.FieldName))
                        {
                            targetRow[e.Column.FieldName] = changedDataRow[e.Column.FieldName];
                        }
                    }
                }

                gridView.CellValueChanged -= gVNLNhapKho_CellValueChanged;

                if (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
                {
                    if (e.Column.FieldName == "SoGhiDauCay" || e.Column.FieldName == "NW" || e.Column.FieldName == "GW")
                    {
                        gridView.SetRowCellValue(e.RowHandle, e.Column, 0);
                    }
                }

                if (e.Column.FieldName == "SoLuongThucTe")
                {
                    tinhThanhTien(true);
                    ganThanhTien(true);
                }
                else if (e.Column.FieldName == "SoGhiDauCay")
                {
                    //decimal soGhiDauCay = Convert.ToDecimal(gridView.GetRowCellValue(e.RowHandle, "SoGhiDauCay") ?? 0);
                    // decimal tileNW = Convert.ToDecimal(gridView.GetRowCellValue(e.RowHandle, "tileNW") ?? 0);
                    //decimal tileGW = Convert.ToDecimal(gridView.GetRowCellValue(e.RowHandle, "tileGW") ?? 0);
                    decimal soGhiDauCay = 0;
                    decimal.TryParse(gridView.GetRowCellValue(e.RowHandle, "SoGhiDauCay")?.ToString(), out soGhiDauCay);
                    decimal tileNW = 0m;
                    tileNW = decimal.TryParse(gridView.GetRowCellValue(e.RowHandle, "tileGW")?.ToString(), out tileNW) ? tileNW : 0m;
                    decimal tileGW = 0m;
                    tileGW = decimal.TryParse(gridView.GetRowCellValue(e.RowHandle, "tileGW")?.ToString(), out tileGW) ? tileGW : 0m;
                    decimal khoiLuongMoi = soGhiDauCay * tileNW;
                    decimal trongLuongMoi = khoiLuongMoi + tileGW;

                    gridView.SetRowCellValue(e.RowHandle, "NW", khoiLuongMoi);
                    gridView.SetRowCellValue(e.RowHandle, "GW", trongLuongMoi);
                }
                /*else if (e.Column.FieldName == "NW")
                {
                    //decimal khoiLuongMoi = Convert.ToDecimal(gridView.GetRowCellValue(e.RowHandle, "NW") ?? 0);
                    //decimal tileGW = Convert.ToDecimal(gridView.GetRowCellValue(e.RowHandle, "tileGW") ?? 0);
                    decimal khoiLuongMoi = 0;
                    decimal.TryParse(gridView.GetRowCellValue(e.RowHandle, "NW")?.ToString(), out khoiLuongMoi);
                    decimal tileGW = 0m;
                    tileGW = decimal.TryParse(gridView.GetRowCellValue(e.RowHandle, "tileGW")?.ToString(), out tileGW) ? tileGW : 0m;
                    decimal trongLuongMoi = khoiLuongMoi + tileGW;

                    gridView.SetRowCellValue(e.RowHandle, "GW", trongLuongMoi);
                }*/
            }
            finally
            {
                // Bật lại sự kiện
                gridView.CellValueChanged += gVNLNhapKho_CellValueChanged;
                // Cho phép GridView cập nhật lại giao diện một lần duy nhất
                gridView.EndUpdate();
            }

        }
        private void ganThanhTien(bool isNL)
        {
            try
            {
                if (isNL)
                {
                    DataRow dr = gVNLNhapKho.GetFocusedDataRow();
                    DataRow row = gVNL.GetFocusedDataRow();
                    decimal _dg = Convert.ToDecimal(row["DonGia"].ToString());
                    if (_dg <= 0) return;
                    decimal _sltt = Convert.ToDecimal(dr["SoLuongThucTe"].ToString());
                    decimal _tt = _dg * _sltt;
                    var rows = tblNhapKho.AsEnumerable().Where(row2 =>
                    row2["MaVTID"].Equals(dr["MaVTID"]) &&
                    row2["MauVTID"].Equals(dr["MauVTID"]) &&
                    row2["KhoVaiID"].Equals(dr["KhoVaiID"]) &&
                     row2["SoKienHienThi"].Equals(dr["SoKienHienThi"]) &&
                     row2["MaNhom"].Equals(dr["MaNhom"]) &&
                     row2["SoLoID"].Equals(dr["SoLoID"])
                    );
                    foreach (var row3 in rows)
                    {
                        row3["ThanhTien"] = _tt;
                    }
                }
                else
                {
                    DataRow dr = gVPLNhapKho.GetFocusedDataRow();
                    DataRow row = gVPL.GetFocusedDataRow();
                    decimal _dg = Convert.ToDecimal(row["DonGia"].ToString());
                    if (_dg <= 0) return;
                    decimal _sltt = Convert.ToDecimal(dr["SoLuongThucTe"].ToString());
                    decimal _tt = _dg * _sltt;
                    var rows = tblNhapKhoPL.AsEnumerable().Where(row2 =>
                    row2["MaVTID"].Equals(dr["MaVTID"]) &&
                    row2["MauVTID"].Equals(dr["MauVTID"]) &&
                    row2["KhoVaiID"].Equals(dr["KhoVaiID"]) &&
                     row2["SoKienHienThi"].Equals(dr["SoKienHienThi"]) &&
                     row2["MaNhom"].Equals(dr["MaNhom"]) &&
                     row2["SoLoID"].Equals(dr["SoLoID"])
                    );
                    foreach (var row3 in rows)
                    {
                        row3["ThanhTien"] = _tt;
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }
        private void gVNL_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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
        private void gVNLNhapKho_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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



        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            var selectedTabPage = e.Page;
            if (selectedTabPage == null) return;
            if (selectedTabPage.Name == "tabNL")
            {
                _isNPL = true;
                CheckPerminsionNL();
                //CopyFromTxtPLToTxt();
                //loadTxt(true);
                loadGVVatTu(true);
                createSearchLookUpVatTu();
                setTxtNull();
                setGVNull();
            }
            else if (selectedTabPage.Name == "tabPL")
            {
                _isNPL = false;
                CheckPerminsionPL();
                //CopyFromTxtToTxtPL();
                //loadTxt(false);
                loadGVVatTu(false);
                createSearchLookUpVatTu();
                setTxtNull();
                setGVNull();
            }
        }
        private void gVPLNhapKho_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gVPLNhapKho.RowCount > 0)
            {
               
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {
                        DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", XoaDongNK);
                        e.Menu.Items.Add(menuDeleteItem);
                        var selectedRows = gVPLNhapKho.GetSelectedRows();
                        foreach (var rowHandle in selectedRows)
                        {
                            DataRow row = gVPLNhapKho.GetDataRow(rowHandle);
                            bool _isEdit = checkEdit(row);
                            if (!_isEdit)
                            {
                                return;
                            }

                        }
                        DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy", CopyCell);
                        e.Menu.Items.Add(menuCopyItem);
                        DevExpress.Utils.Menu.DXMenuItem menuPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Paste", PasteCell);
                        e.Menu.Items.Add(menuPasteItem);

                       

                        //DevExpress.Utils.Menu.DXMenuItem menuThemLoTItem = new DevExpress.Utils.Menu.DXMenuItem("Thêm LOT", ThemLoT);
                        //e.Menu.Items.Add(menuThemLoTItem);

                        //DevExpress.Utils.Menu.DXMenuItem menuDoiDVItem = new DevExpress.Utils.Menu.DXMenuItem("Đổi đơn vị", DoiDV);
                        //e.Menu.Items.Add(menuDoiDVItem);

                        //DevExpress.Utils.Menu.DXMenuItem menunhapKLItem = new DevExpress.Utils.Menu.DXMenuItem("Nhập khối lượng", nhapKhoiLuong);
                        //e.Menu.Items.Add(menunhapKLItem);


                    }

                }
            }
        }
        private void CopyCell(object sender, EventArgs e)
        {
            GridView view = (_isNPL ? gCNLNhapKho.MainView : gCPLNhapKho.MainView) as GridView;
            if (view == null) return;
            view.CopyToClipboard();
        }
        private void PasteCell(object sender, EventArgs e)
        {
            try
            {
                GridView view = (_isNPL ? gCNLNhapKho.MainView : gCPLNhapKho.MainView) as GridView;
                string clipboardData = Clipboard.GetText();
                byte[] bytes = Encoding.UTF8.GetBytes(clipboardData);
                string decodedClipboardData = Encoding.UTF8.GetString(bytes);
                string[] data = decodedClipboardData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 0) return;
                string checkDataRow = "Số Roll\tLOT\tBatch\tSL theo CT\tSL thực tế\tKhối lượng\tTrọng lượng\tPallet\tGhi chú\tBarCode";
                if (checkDataRow.Contains(data[0]))
                {
                    data = data.Skip(1).ToArray();
                }
                int startRow = view.FocusedRowHandle;
                foreach (string row in data)
                {
                    AddRow(row, startRow++, view);
                    if (!view.IsValidRowHandle(startRow)) break;
                }
                view.CloseEditor();


            }
            catch (Exception ex)
            {
            }
        }
        private void AddRow(string data, int rowHandle, GridView view)
        {
            if (string.IsNullOrEmpty(data)) return;

            string[] rowData = data.Split('\t');
            int columnIndex = view.FocusedColumn.VisibleIndex;

            int originalRowHandle = view.GetDataSourceRowIndex(rowHandle);

            for (int i = 0; i < rowData.Length; i++)
            {
                if (i >= view.VisibleColumns.Count) break;

                try
                {
                    GridColumn targetColumn = view.VisibleColumns[columnIndex + i];

                    if (targetColumn.ToString() == "Kiện" || targetColumn.ToString() == "BarCode") break;
                    Type fieldType = view.Columns[targetColumn.FieldName].ColumnType;

                    if (fieldType == typeof(int))
                    {
                        if (int.TryParse(rowData[i].Replace(",", ""), out int intValue))
                        {
                            view.SetRowCellValue(rowHandle, targetColumn, intValue);
                        }
                        else
                        {

                            this.ActiveControl = simpleButton1;
                        }
                    }
                    else if (fieldType == typeof(float))
                    {
                        if (float.TryParse(rowData[i].Replace(",", ""), out float floatValue))
                        {
                            view.SetRowCellValue(rowHandle, targetColumn, floatValue);
                        }
                        else
                        {

                            this.ActiveControl = simpleButton1;
                        }
                    }
                    else
                    {

                        view.SetRowCellValue(rowHandle, targetColumn, rowData[i]);
                        this.ActiveControl = simpleButton1;
                    }
                }


                catch (Exception ex)
                {
                    //MessageBox.Show($"Đã xảy ra lỗi khi dán dữ liệu vào cột '{view.VisibleColumns[columnIndex + i].FieldName}': {ex.Message}",
                    //                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }
            }
        }

        private void ThemLoT(object sender, EventArgs e)
        {
            try
            {
                DataTable tblNK = (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource as DataTable;

                GridView view = (_isNPL ? gVNLNhapKho : gVPLNhapKho);

                int rowCount = view.RowCount;
                int minSTT = rowCount > 0 ? 1 : 0;
                int maxSTT = rowCount;


                //var maxSoKien = tblNK.AsEnumerable().Max(row => Convert.ToDouble(row["SoKien"]));
                //var minSoKien = tblNK.AsEnumerable().Min(row => Convert.ToDouble(row["SoKien"]));

                frmERPNhapKhoNPL_ThemLot frm = new frmERPNhapKhoNPL_ThemLot(minSTT.ToString(), maxSTT.ToString());
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                List<string> _lst = frm.GetValues();
                if (_lst.Count < 3) return;
                string _tukien = _lst[0];
                string _denkien = _lst[1];
                string _lot = _lst[2];

                DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (dr == null) return;

                int startIndex = Convert.ToInt32(_tukien) - 1;
                int endIndex = Convert.ToInt32(_denkien) - 1;


                foreach (DataRow _dr in (_isNPL ? tblNhapKho : tblNhapKhoPL).Rows)
                {
                    if (dr["MaVTID"].ToString() == _dr["MaVTID"].ToString() && dr["MauVTID"].ToString() == _dr["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == _dr["KhoVaiID"].ToString() && dr["MaNhom"].ToString() == _dr["MaNhom"].ToString() && dr["SoLoID"].ToString() == _dr["SoLoID"].ToString()
                       && Convert.ToInt32(_dr["STT"]) >= Convert.ToInt32(_tukien) && Convert.ToInt32(_dr["STT"]) <= Convert.ToInt32(_denkien))
                    {
                        _dr["SoLot"] = _lot;
                    }
                }
                foreach (DataRow _dr in tblNK.Rows)
                {
                    if (dr["MaVTID"].ToString() == _dr["MaVTID"].ToString() && dr["MauVTID"].ToString() == _dr["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == _dr["KhoVaiID"].ToString() && dr["MaNhom"].ToString() == _dr["MaNhom"].ToString() && dr["SoLoID"].ToString() == _dr["SoLoID"].ToString()
                      && Convert.ToInt32(_dr["STT"]) >= Convert.ToInt32(_tukien) && Convert.ToInt32(_dr["STT"]) <= Convert.ToInt32(_denkien))
                    {
                        _dr["SoLot"] = _lot;
                    }
                }

            }
            catch (Exception ex)
            {


            }
        }



        private void gVPLNhapKho_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            var gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (gridView == null) return;

            gridView.BeginUpdate();
            try
            {

                DataRowView changedRowView = gridView.GetRow(e.RowHandle) as DataRowView;
                if (changedRowView != null)
                {
                    DataRow changedDataRow = changedRowView.Row;


                    var targetRow = tblNhapKhoPL.AsEnumerable().FirstOrDefault(row =>
                        row.Field<string>("MaVTID") == changedDataRow.Field<string>("MaVTID") &&
                        row.Field<string>("MauVTID") == changedDataRow.Field<string>("MauVTID") &&
                        row.Field<string>("KhoVaiID") == changedDataRow.Field<string>("KhoVaiID") &&
                        row["STT"].ToString() == changedDataRow["STT"].ToString() &&
                        row.Field<string>("MaNhom") == changedDataRow.Field<string>("MaNhom") &&
                        row.Field<string>("SoLoID") == changedDataRow.Field<string>("SoLoID")
                    );

                    // Nếu tìm thấy, cập nhật tất cả các trường cần thiết một lần
                    if (targetRow != null)
                    {
                        targetRow.BeginEdit(); // Bắt đầu chỉnh sửa dòng
                        targetRow["SoLot"] = changedDataRow["SoLot"];
                        targetRow["SoGhiDauCay"] = changedDataRow["SoGhiDauCay"];
                        targetRow["SoLuongThucTe"] = changedDataRow["SoLuongThucTe"];
                        targetRow["Pallet"] = changedDataRow["Pallet"];
                        targetRow["NW"] = changedDataRow["NW"];
                        targetRow["GW"] = changedDataRow["GW"];
                        targetRow["GhiChu"] = changedDataRow["GhiChu"];
                        targetRow["SoKienHienThi"] = changedDataRow["SoKienHienThi"];
                        targetRow["Batch"] = changedDataRow["Batch"];
                        targetRow.EndEdit(); // Kết thúc và chấp nhận các thay đổi
                    }
                }

                // === PHẦN 2: Tối ưu hóa logic tính toán và cập nhật GridView ===

                // Tạm thời hủy đăng ký sự kiện để tránh gọi đệ quy khi dùng SetRowCellValue
                gridView.CellValueChanged -= gVPLNhapKho_CellValueChanged;
                try
                {
                    // Đặt giá trị mặc định cho ô trống (nếu có)
                    if (e.Value == null || string.IsNullOrWhiteSpace(e.Value.ToString()))
                    {
                        if (e.Column.FieldName == "SoGhiDauCay" || e.Column.FieldName == "NW" || e.Column.FieldName == "GW")
                        {
                            gridView.SetRowCellValue(e.RowHandle, e.Column, 0);
                        }
                    }

                    // Xử lý logic nghiệp vụ cho từng cột
                    if (e.Column.FieldName == "SoLuongThucTe")
                    {
                        tinhThanhTien(false);
                        ganThanhTien(false);
                    }
                    else if (e.Column.FieldName == "SoGhiDauCay")
                    {
                        // Lấy giá trị sau khi đã được chuẩn hóa
                        //decimal soGhiDauCay = Convert.ToDecimal(gridView.GetRowCellValue(e.RowHandle, "SoGhiDauCay"));
                        //decimal tileNW = Convert.ToDecimal(gridView.GetRowCellValue(e.RowHandle, "tileNW") ?? 0m);
                        //decimal tileGW = Convert.ToDecimal(gridView.GetRowCellValue(e.RowHandle, "tileGW") ?? 0m);

                        decimal soGhiDauCay = 0;
                        decimal.TryParse(gridView.GetRowCellValue(e.RowHandle, "SoGhiDauCay")?.ToString(), out soGhiDauCay);
                        decimal tileNW = 0m;
                        tileNW = decimal.TryParse(gridView.GetRowCellValue(e.RowHandle, "tileGW")?.ToString(), out tileNW) ? tileNW : 0m;
                        decimal tileGW = 0m;
                        tileGW = decimal.TryParse(gridView.GetRowCellValue(e.RowHandle, "tileGW")?.ToString(), out tileGW) ? tileGW : 0m;
                        decimal khoiLuongMoi = soGhiDauCay * tileNW;
                        decimal trongLuongMoi = khoiLuongMoi + tileGW;

                        // Cập nhật các ô liên quan
                        gridView.SetRowCellValue(e.RowHandle, "NW", khoiLuongMoi);
                        gridView.SetRowCellValue(e.RowHandle, "GW", trongLuongMoi);
                    }
                    else if (e.Column.FieldName == "NW")
                    {
                        //decimal khoiLuongMoi = Convert.ToDecimal(gridView.GetRowCellValue(e.RowHandle, "NW"));
                        //decimal tileGW = Convert.ToDecimal(gridView.GetRowCellValue(e.RowHandle, "tileGW") ?? 0m);
                        decimal khoiLuongMoi = 0;
                        decimal.TryParse(gridView.GetRowCellValue(e.RowHandle, "NW")?.ToString(), out khoiLuongMoi);
                        decimal tileGW = 0m;
                        tileGW = decimal.TryParse(gridView.GetRowCellValue(e.RowHandle, "tileGW")?.ToString(), out tileGW) ? tileGW : 0m;

                        decimal trongLuongMoi = khoiLuongMoi + tileGW;

                        gridView.SetRowCellValue(e.RowHandle, "GW", trongLuongMoi);
                    }
                }
                finally
                {
                    // Đăng ký lại sự kiện sau khi hoàn tất
                    gridView.CellValueChanged += gVPLNhapKho_CellValueChanged;
                }
            }
            finally
            {
                // Cho phép GridView cập nhật lại giao diện một cách hiệu quả
                gridView.EndUpdate();
            }
        }


        private void gVPL_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gVPL.GetFocusedDataRow();
                if (dr == null) return;
                if (tblNhapKhoPL == null || tblNhapKhoPL.Rows.Count == 0) CreateTableNhapKhoPL();
                string maVTID = dr["MaVTID"].ToString();
                string maMauVT = dr["MauVTID"].ToString();
                string khoVaiID = dr["KhoVaiID"].ToString();
                string SoLoID = dr["SoLoID"].ToString();
                string MaNhom = dr["MaNhom"].ToString();
                DataTable tblNhapKhoFiltered = tblNhapKhoPL.Clone();
                var filteredRows = tblNhapKhoPL.AsEnumerable()
                .Where(row => row.Field<string>("MaVTID") == maVTID &&
                              row.Field<string>("MauVTID") == maMauVT &&
                              row.Field<string>("KhoVaiID") == khoVaiID &&
                              row.Field<string>("SoLoID") == SoLoID &&
                              row.Field<string>("MaNhom") == MaNhom
                              );
                foreach (var row in filteredRows)
                {
                    var barCode = row.Field<string>("BarCode");
                    if (!string.IsNullOrWhiteSpace(barCode))
                    {
                        var segments = barCode.Split('|');
                        if (segments.Length > 0 && !string.IsNullOrWhiteSpace(segments[segments.Length - 1]))
                        {
                            tblNhapKhoFiltered.ImportRow(row);
                        }
                    }
                }


                gCPLNhapKho.DataSource = tblNhapKhoFiltered;
                _rowFocus = e.FocusedRowHandle;
                loadTxtV2(false, SoLoID);
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
        private void searchLookUpEditNL_Popup(object sender, EventArgs e)
        {
            var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

            var layout = popupForm.Controls
                .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                .FirstOrDefault()
                ?.Controls
                .OfType<LayoutControl>()
                .FirstOrDefault();

            if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnOK"))
            {
                // Tạo nhóm layout dạng bảng
                var buttonGroup = new LayoutControlGroup()
                {
                    Name = "buttonGroup",
                    TextVisible = false,
                    LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                    Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                    Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                    GroupBordersVisible = false
                };

                // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                // Thêm nhóm vào layout chính
                layout.BeginUpdate();
                layout.Root.AddItem(buttonGroup);

                // 1. Empty space bên trái
                var emptySpaceItemLeft = new EmptySpaceItem();
                emptySpaceItemLeft.AllowHotTrack = false;
                emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(emptySpaceItemLeft);

                // 2. Nút "Khai báo Khổ/Size"
                var khaiBaoButton = new SimpleButton() { Name = "btnOK", Text = "OK" };
                khaiBaoButton.Click += searchOKButton_Click;
                var layoutItemKhaiBao = new LayoutControlItem()
                {
                    Control = khaiBaoButton,
                    TextVisible = false,
                    SizeConstraintsType = SizeConstraintsType.Custom,
                    MinSize = new Size(140, 30),
                    MaxSize = new Size(140, 30)
                };
                layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(layoutItemKhaiBao);

                // 3. Empty space trên và dưới để căn giữa
                var emptySpaceItemTop = new EmptySpaceItem();
                emptySpaceItemTop.AllowHotTrack = false;
                emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemTop);

                var emptySpaceItemBottom = new EmptySpaceItem();
                emptySpaceItemBottom.AllowHotTrack = false;
                emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemBottom);

                layout.EndUpdate();
            }
        }
        private void searchOKButton_Click(object sender, EventArgs e)
        {
            //searchLookUpEditNL.ClosePopup();
        }

        private void searchLookUpEditPL_Popup(object sender, EventArgs e)
        {
            var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

            var layout = popupForm.Controls
                .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                .FirstOrDefault()
                ?.Controls
                .OfType<LayoutControl>()
                .FirstOrDefault();

            if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnOK"))
            {
                // Tạo nhóm layout dạng bảng
                var buttonGroup = new LayoutControlGroup()
                {
                    Name = "buttonGroup",
                    TextVisible = false,
                    LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                    Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                    Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                    GroupBordersVisible = false
                };

                // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                // Thêm nhóm vào layout chính
                layout.BeginUpdate();
                layout.Root.AddItem(buttonGroup);

                // 1. Empty space bên trái
                var emptySpaceItemLeft = new EmptySpaceItem();
                emptySpaceItemLeft.AllowHotTrack = false;
                emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(emptySpaceItemLeft);

                // 2. Nút "Khai báo Khổ/Size"
                var khaiBaoButton = new SimpleButton() { Name = "btnOK", Text = "OK" };
                khaiBaoButton.Click += searchOKPLButton_Click;
                var layoutItemKhaiBao = new LayoutControlItem()
                {
                    Control = khaiBaoButton,
                    TextVisible = false,
                    SizeConstraintsType = SizeConstraintsType.Custom,
                    MinSize = new Size(140, 30),
                    MaxSize = new Size(140, 30)
                };
                layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(layoutItemKhaiBao);

                // 3. Empty space trên và dưới để căn giữa
                var emptySpaceItemTop = new EmptySpaceItem();
                emptySpaceItemTop.AllowHotTrack = false;
                emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemTop);

                var emptySpaceItemBottom = new EmptySpaceItem();
                emptySpaceItemBottom.AllowHotTrack = false;
                emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemBottom);

                layout.EndUpdate();
            }
        }
        private void searchOKPLButton_Click(object sender, EventArgs e)
        {
            //searchLookUpEditPL.ClosePopup();
        }

        #region barbutton
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_isNPL)
            {
                if (!CheckSLThucTeSLCT(tblNhapKho, gVNLNhapKho)) return;
                if (!luuPhieu()) return;
                if (!luuNhapKho()) return;
                loadGVVatTu(true);
            }
            else
            {
                if (!CheckSLThucTeSLCT(tblNhapKhoPL, gVPLNhapKho)) return;
                if (!luuPhieuPL()) return;
                if (!luuNhapKhoPL()) return;
                loadGVVatTu(false);
            }
            Save_E_Invoice();

        }

        #endregion

        #region RepositoryItem
        private void btnThem_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                DataRow dr = gVNL.GetFocusedDataRow();
                if (dr == null) return;
                _soloid = dr["SoLoID"].ToString();
                if (searchLookUpEditLoNL.EditValue == null) return;
                if (string.IsNullOrWhiteSpace(searchLookUpEditLoNL.Text))
                {
                    MessageBox.Show("Vui lòng chọn PI NCC.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    searchLookUpEditLoNL.Focus();
                    return;
                }

                List<string> currentList = new List<string>();
                if (tblNhapKho == null || tblNhapKho.Rows.Count == 0) CreateTableNhapKho();
                DataRow _dr = gVNLNhapKho.GetFocusedDataRow();
                DataTable dt = gCNLNhapKho.DataSource as DataTable;
                decimal tileNW = 0m, tileGW = 0m;
                int sttChonVT = 1;
                if (dr.Table.Columns.Contains("STTChonVT") && dr["STTChonVT"] != DBNull.Value)
                {
                    sttChonVT = Convert.ToInt32(dr["STTChonVT"]);
                }
                string madvcd = string.Empty, tendvcd = string.Empty, batch = string.Empty;
                if (_dr != null)
                {
                    tileNW = decimal.TryParse(_dr["tileNW"].ToString(), out var val) ? val : 0m;
                    tileGW = decimal.TryParse(_dr["tileGW"].ToString(), out var val2) ? val2 : 0m;
                    madvcd = _dr["MaDVCD"].ToString();
                    tendvcd = _dr["TenDVCD"].ToString();
                    batch = _dr["Batch"].ToString();
                }
                frmERPNhapKhoNPL_KhaiBao frm = new frmERPNhapKhoNPL_KhaiBao(dr, dt, searchLookUpEditLoNL.Text.ToString(), true, tileNW, tileGW, madvcd, tendvcd, _soloid, batch);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ThongSoKienChanged += (list) =>
                {

                    tblNhapKho = frmERPNhapKhoNPL_KhaiBao.dt;
                    string maVTID = dr["MaVTID"].ToString();
                    string maMauVT = dr["MauVTID"].ToString();
                    string khoVaiID = dr["KhoVaiID"].ToString();
                    string manhom = dr["MaNhom"].ToString();
                    //string soloID = dr["SoLoID"].ToString();
                    DataTable tblNhapKhoFiltered = tblNhapKho.Clone();
                    var filteredRows = tblNhapKho.AsEnumerable()
                        .Where(row => row["MaVTID"].ToString() == maVTID &&
                                      row["MauVTID"].ToString() == maMauVT &&
                                      row["KhoVaiID"].ToString() == khoVaiID);
                    foreach (var row in filteredRows)
                    {
                        row["STT"] = sttChonVT;
                        row["sttChonVT"] = sttChonVT;
                    }
                    foreach (var row in filteredRows)
                    {
                        tblNhapKhoFiltered.ImportRow(row);
                    }

                    gCNLNhapKho.DataSource = tblNhapKhoFiltered;
                };
                frm.ShowDialog();
                resetTongKien();


            }
            catch (Exception ex)
            {

            }
        }
        private void btnThemPL_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DataRow dr = gVPL.GetFocusedDataRow();
                if (dr == null) return;
                _soloid = dr["SoLoID"].ToString();
                if (searchLookUpEditLoPL.EditValue == null) return;
                if (string.IsNullOrWhiteSpace(searchLookUpEditLoPL.Text))
                {
                    MessageBox.Show("Vui lòng chọn PI NCC.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    searchLookUpEditLoPL.Focus();
                    return;
                }


                List<string> currentList = new List<string>();
                if (tblNhapKhoPL == null || tblNhapKhoPL.Rows.Count == 0) CreateTableNhapKhoPL();
                DataRow _dr = gVPLNhapKho.GetFocusedDataRow();
                DataTable dt = gCPLNhapKho.DataSource as DataTable;
                decimal tileNW = 0m, tileGW = 0m;
                string madvcd = string.Empty, tendvcd = string.Empty, batch = string.Empty;
                int sttChonVT = 1;
                if (dr.Table.Columns.Contains("STTChonVT") && dr["STTChonVT"] != DBNull.Value)
                {
                    sttChonVT = Convert.ToInt32(dr["STTChonVT"]);
                }
                if (_dr != null)
                {
                    tileNW = decimal.TryParse(_dr["tileNW"].ToString(), out var val) ? val : 0m;
                    tileGW = decimal.TryParse(_dr["tileGW"].ToString(), out var val2) ? val2 : 0m;
                    madvcd = _dr["MaDVCD"].ToString();
                    tendvcd = _dr["TenDVCD"].ToString();
                    batch = _dr["Batch"].ToString();
                }
                frmERPNhapKhoNPL_KhaiBao frm = new frmERPNhapKhoNPL_KhaiBao(dr, dt, searchLookUpEditLoPL.Text.ToString(), false, tileNW, tileGW, madvcd, tendvcd, _soloid, batch);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ThongSoKienChanged += (list) =>
                {
                    tblNhapKhoPL = frmERPNhapKhoNPL_KhaiBao.dt;
                    DataTable tblNhapKhoFiltered = tblNhapKhoPL.Clone();
                    string maVTID = dr["MaVTID"].ToString();
                    string maMauVT = dr["MauVTID"].ToString();
                    string khoVaiID = dr["KhoVaiID"].ToString();
                    var filteredRows = tblNhapKhoPL.AsEnumerable()
                        .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                      row.Field<string>("MauVTID") == maMauVT &&
                                      row.Field<string>("KhoVaiID") == khoVaiID);
                    foreach (var row in filteredRows)
                    {
                        row["STT"] = sttChonVT;
                        row["sttChonVT"] = sttChonVT;
                    }
                    foreach (var row in filteredRows)
                    {
                        tblNhapKhoFiltered.ImportRow(row);
                    }

                    gCPLNhapKho.DataSource = tblNhapKhoFiltered;
                };
                frm.ShowDialog();
                resetTongKien();

            }
            catch (Exception ex)
            {

            }

        }
        private void repoTxtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repoTxtNW_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repoTxtGW_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repoTxtQtyPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repoTxtNWPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repoTxtGWPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repoTxtSLThucTe_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repoTxtDonGia_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repoTxtThanhTien_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repoTxtDonGiaNL_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repoTxtThanhTienNL_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repoTxtSLThucTeNL_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }



        private void gVSearchPL_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRowsPL.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }
        private void repoSearchLookUpEditMau_EditValueChanged(object sender, EventArgs e)
        {
            if (sender is SearchLookUpEdit searchLookUpEdit)
            {
                DataRowView selectedRowView = searchLookUpEdit.Properties.View.GetFocusedRow() as DataRowView;

                if (selectedRowView == null) return;

                int focusedRowHandle2 = gVNL.FocusedRowHandle;
                if (focusedRowHandle2 >= 0)
                {
                    gVNL.SetRowCellValue(focusedRowHandle2, "MaMauVT", selectedRowView["MaMauVT"]);
                }
            }
        }


        private void repoTxtTongKienPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtMaSoThue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtMaSoThuePL_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSoChungTuPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            //{
            //    e.Handled = true;
            //}
        }

        private void txtSoChungTu_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            //{
            //    e.Handled = true;
            //}
        }



        private void repoTxtTongKien_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void gCNLNhapKho_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteCell(sender, e);
            }
        }

        private void gVNL_ShowingEditor(object sender, CancelEventArgs e)
        {
            DataRow dr = gVNL.GetFocusedDataRow();
            if (dr == null) return;
            GridView view = sender as GridView;
            //if (view.FocusedColumn.FieldName == "MaHaiQuan" && !string.IsNullOrWhiteSpace(dr["MaHaiQuan"].ToString())) // Kiểm tra cột hiện tại
            //{
            //    if (_lstMaHQ.Contains(dr["MaHaiQuan"].ToString()))
            //    {
            //        e.Cancel = true;
            //    }
            //}
            //else if (view.FocusedColumn.FieldName == "MaKeToan" && !string.IsNullOrWhiteSpace(dr["MaKeToan"].ToString())) // Kiểm tra cột hiện tại
            //{
            //    if (_lstMaKT.Contains(dr["MaKeToan"].ToString()))
            //    {
            //        e.Cancel = true;
            //    }
            //}

            var column = view.FocusedColumn;

            if (column.FieldName == "DonGia")
            {
                var value = view.GetFocusedValue();

                if (value != null &&
                    ((value is int && (int)value == 0) ||
                     (value is decimal && (decimal)value == 0) ||
                     (value is double && (double)value == 0) ||
                     (value is float && (float)value == 0)))
                {
                    view.SetFocusedValue(null);
                }
            }

        }

        private void gVPL_ShowingEditor(object sender, CancelEventArgs e)
        {
            DataRow dr = gVPL.GetFocusedDataRow();
            if (dr == null) return;
            GridView view = sender as GridView;
            //if (view.FocusedColumn.FieldName == "MaHaiQuan") // Kiểm tra cột hiện tại
            //{
            //    if (_lstMaHQ.Contains(dr["MaHaiQuan"].ToString()))
            //    {
            //        e.Cancel = true;
            //    }
            //}
            //else if (view.FocusedColumn.FieldName == "MaKeToan") // Kiểm tra cột hiện tại
            //{
            //    if (_lstMaKT.Contains(dr["MaKeToan"].ToString()))
            //    {
            //        e.Cancel = true;
            //    }
            //}
            var column = view.FocusedColumn;

            if (column.FieldName == "DonGia")
            {
                var value = view.GetFocusedValue();

                if (value != null &&
                    ((value is int && (int)value == 0) ||
                     (value is decimal && (decimal)value == 0) ||
                     (value is double && (double)value == 0) ||
                     (value is float && (float)value == 0)))
                {
                    view.SetFocusedValue(null);
                }
            }
        }



        private void gCPLNhapKho_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteCell(sender, e);
            }
        }



        private void gVNLNhapKho_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var column = view.FocusedColumn;
            DataRow dr = gVNLNhapKho.GetFocusedDataRow();
            if (dr == null) return;
            bool _isEdit = checkEdit(dr);
            if (_isEdit)
            {
                if (column.FieldName == "SoLuongThucTe")
                {
                    bool isKiemKeChecked = dr["IsKiemKe"] != DBNull.Value && Convert.ToBoolean(dr["IsKiemKe"]);
                    bool isQCPass = (txtQCKiem.Text == "PASS");
                    if (!(isKiemKeChecked && isQCPass))
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                if (column.FieldName == "SoGhiDauCay" || column.FieldName == "NW" || column.FieldName == "GW")
                {
                    var value = view.GetFocusedValue();

                    if (value != null &&
                        ((value is int && (int)value == 0) ||
                         (value is decimal && (decimal)value == 0) ||
                         (value is double && (double)value == 0) ||
                         (value is float && (float)value == 0)))
                    {

                        view.SetFocusedValue(null);
                    }
                }

            }
            else
            {

                e.Cancel = true;
                MessageBox.Show("Nguyên liệu đã được nhập kho. Không thể chỉnh sửa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void gVPLNhapKho_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var column = view.FocusedColumn;
            DataRow dr = gVPLNhapKho.GetFocusedDataRow();
            if (dr == null) return;
            bool _isEdit = checkEdit(dr);

            if (_isEdit)
            {
                if (column.FieldName == "SoLuongThucTe")
                {
                    bool isKiemKeChecked = dr["IsKiemKe"] != DBNull.Value && Convert.ToBoolean(dr["IsKiemKe"]);
                    bool isQCPass = (txtQCKiemPL.Text == "PASS");
                    if (!(isKiemKeChecked && isQCPass))
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                if (column.FieldName == "SoGhiDauCay" || column.FieldName == "NW" || column.FieldName == "GW")
                {
                    var value = view.GetFocusedValue();

                    if (value != null &&
                        ((value is int && (int)value == 0) ||
                         (value is decimal && (decimal)value == 0) ||
                         (value is double && (double)value == 0) ||
                         (value is float && (float)value == 0)))
                    {

                        view.SetFocusedValue(null);
                    }
                }


            }
            else
            {

                e.Cancel = true;
                MessageBox.Show("Phụ liệu đã được nhập kho. Không thể chỉnh sửa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void gVNLNhapKho_CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            if (e.Column.FieldName == "SoKien")
            {
                // Lấy giá trị của hai ô cần so sánh
                string val1 = e.Value1?.ToString() ?? "";
                string val2 = e.Value2?.ToString() ?? "";

                // Tách giá trị thành các phần bằng dấu chấm
                var parts1 = val1.Split('.');
                var parts2 = val2.Split('.');

                // Kiểm tra và lấy phần số nguyên
                if (parts1.Length > 0 && parts2.Length > 0 &&
                    int.TryParse(parts1[0], out int int1) && int.TryParse(parts2[0], out int int2))
                {
                    // So sánh phần số nguyên trước
                    if (int1 < int2)
                    {
                        e.Result = -1;
                    }
                    else if (int1 > int2)
                    {
                        e.Result = 1;
                    }
                    else
                    {
                        // Nếu phần số nguyên bằng nhau, so sánh phần thập phân
                        int dec1 = parts1.Length > 1 && int.TryParse(parts1[1], out int d1) ? d1 : 0;
                        int dec2 = parts2.Length > 1 && int.TryParse(parts2[1], out int d2) ? d2 : 0;
                        e.Result = dec1.CompareTo(dec2);
                    }
                }
                else
                {
                    // Nếu không thể parse, so sánh như chuỗi
                    e.Result = string.Compare(val1, val2);
                }

                e.Handled = true;
            }
            gVNL.FocusedRowHandle = 0;
        }

        private void gVPLNhapKho_CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            if (e.Column.FieldName == "SoKien")
            {
                // Lấy giá trị của hai ô cần so sánh
                string val1 = e.Value1?.ToString() ?? "";
                string val2 = e.Value2?.ToString() ?? "";

                // Tách giá trị thành các phần bằng dấu chấm
                var parts1 = val1.Split('.');
                var parts2 = val2.Split('.');

                // Kiểm tra và lấy phần số nguyên
                if (parts1.Length > 0 && parts2.Length > 0 &&
                    int.TryParse(parts1[0], out int int1) && int.TryParse(parts2[0], out int int2))
                {
                    // So sánh phần số nguyên trước
                    if (int1 < int2)
                    {
                        e.Result = -1;
                    }
                    else if (int1 > int2)
                    {
                        e.Result = 1;
                    }
                    else
                    {
                        // Nếu phần số nguyên bằng nhau, so sánh phần thập phân
                        int dec1 = parts1.Length > 1 && int.TryParse(parts1[1], out int d1) ? d1 : 0;
                        int dec2 = parts2.Length > 1 && int.TryParse(parts2[1], out int d2) ? d2 : 0;
                        e.Result = dec1.CompareTo(dec2);
                    }
                }
                else
                {
                    // Nếu không thể parse, so sánh như chuỗi
                    e.Result = string.Compare(val1, val2);
                }

                e.Handled = true;
            }
            gVPL.FocusedRowHandle = 0;
        }

        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_isNPL)
            {
                loadTxt(true);

                loadGVVatTu(true);

            }
            else
            {
                loadTxt(false);

                loadGVVatTu(false);

            }

        }

        private void splitContainerControl1_Paint(object sender, PaintEventArgs e)
        {
            splitContainerControl1.SplitterPosition = (int)(splitContainerControl1.Width * 0.6);
        }

        private void splitContainerControl2_Paint(object sender, PaintEventArgs e)
        {
            splitContainerControl2.SplitterPosition = (int)(splitContainerControl2.Width * 0.6);
        }

        private void gVNLNhapKho_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {

            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT1" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void gVPLNhapKho_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {

            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT1" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }
        #endregion

        #region hàm phụ
        private void tinhThanhTien(bool isNL)
        {
            try
            {
                if (isNL)
                {
                    DataTable tbl = gCNLNhapKho.DataSource as DataTable;
                    DataRow dr = gVNL.GetFocusedDataRow();
                    decimal _dg = Convert.ToDecimal(dr["DonGia"].ToString());
                    if (_dg <= 0) return;
                    decimal _sum = 0;
                    foreach (DataRow row in tbl.Rows)
                    {
                        if (row["MaVTID"].ToString() == dr["MaVTID"].ToString() && row["MaMauVT"].ToString() == dr["MaMauVT"].ToString() && row["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString())
                        {
                            decimal _sltt = 0;
                            decimal temp;

                            if (row["SoLuongThucTe"] != null &&
                                decimal.TryParse(row["SoLuongThucTe"].ToString(), out temp) &&
                                temp != 0)
                            {
                                _sltt = temp;
                            }
                            else
                            {
                                _sltt = 0;
                            }
                            if (_sltt > 0)
                            {
                                _sum += (_dg * _sltt);
                            }
                        }

                    }

                    int rowHandle = gVNL.FocusedRowHandle;
                    gVNL.SetRowCellValue(rowHandle, "ThanhTien", _sum);
                }
                else
                {
                    DataTable tbl = gCPLNhapKho.DataSource as DataTable;
                    DataRow dr = gVPL.GetFocusedDataRow();
                    decimal _dg = Convert.ToDecimal(dr["DonGia"].ToString());
                    if (_dg <= 0) return;
                    decimal _sum = 0;
                    foreach (DataRow row in tbl.Rows)
                    {
                        if (row["MaVTID"].ToString() == dr["MaVTID"].ToString() && row["MaMauVT"].ToString() == dr["MaMauVT"].ToString() && row["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString())
                        {
                            decimal _sltt = 0;
                            decimal temp;

                            if (row["SoLuongThucTe"] != null &&
                                decimal.TryParse(row["SoLuongThucTe"].ToString(), out temp) &&
                                temp != 0)
                            {
                                _sltt = temp;
                            }
                            else
                            {
                                _sltt = 0;
                            }
                            if (_sltt > 0)
                            {
                                _sum += (_dg * _sltt);
                            }
                        }

                    }

                    int rowHandle = gVPL.FocusedRowHandle;
                    gVPL.SetRowCellValue(rowHandle, "ThanhTien", _sum);
                }
            }
            catch (Exception ex)
            {

            }



        }
        public static void HandleGridViewKey(KeyEventArgs e, GridView gridView, string _fieldName)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int RowHandle = gridView.FocusedRowHandle;
                if (RowHandle == gridView.RowCount - 1) RowHandle = -1;
                gridView.FocusedRowHandle = RowHandle + 1;
                gridView.FocusedColumn = gridView.Columns[_fieldName];
                gridView.ShowEditor();
            }
            else if (e.KeyCode == Keys.Down)
            {
                gridView.FocusedColumn = gridView.Columns[_fieldName];
                gridView.ShowEditor();
            }
            else if (e.KeyCode == Keys.Up)
            {
                gridView.FocusedColumn = gridView.Columns[_fieldName];
                gridView.ShowEditor();
            }
        }

        private void setTxtNull()
        {
            if (_isNPL)
            {
                if (searchLookUpEditLoNL.EditValue != null)
                    searchLookUpEditLoNL.EditValue = null;
                searchLookUpEditNCC.EditValue = null;
                txtSoToKhai.Text = "";
                dateNgayToKhai.DateTime = DateTime.Now;
                txtSoHopDong.Text = "";
                dateSoHopDong.DateTime = DateTime.Now;
                txtMaSoThue.Text = "";
                txtSoHoaDon.Text = "";
                searchLookUpEditCang.EditValue = null;
                SearchlookupTau.EditValue = null;
                dateNgayGui.DateTime = DateTime.Now;
                txtNoiGui.Text = "";
                txtPOMua.Text = "";
                txtSoVanDon.Text = "";
                txtQCKiem.Text = "";
                txtNW.Text = "";
                txtGW.Text = "";
                searchLookUpEditKHCT.EditValue = null;
                searchLookUpEditMHCT.EditValue = null;
            }
            else
            {
                if (searchLookUpEditLoPL.EditValue != null)
                    searchLookUpEditLoPL.EditValue = null;
                searchLookUpEditNCCPL.EditValue = null;
                txtSoToKhaiPL.Text = "";
                dateNgayToKhaiPL.DateTime = DateTime.Now;
                txtSoHopDongPL.Text = "";
                dateSoHopDongPL.DateTime = DateTime.Now;
                txtMaSoThuePL.Text = "";
                txtSoHoaDonPL.Text = "";
                searchLookUpEditCangPL.EditValue = null;
                SearchlookupTauPL.EditValue = null;
                dateNgayGuiPL.DateTime = DateTime.Now;
                txtNoiGuiPL.Text = "";
                txtPOMuaPL.Text = "";
                txtSoVanDonPL.Text = "";
                txtQCKiemPL.Text = "";
                txtNWPL.Text = "";
                txtGWPL.Text = "";
                searchLookUpEditKHCTPL.EditValue = null;
                searchLookUpEditMHCTPL.EditValue = null;
            }

        }
        private void setGVNull()
        {
            if (_isNPL)
            {
                tblNL.Clear();
                tblChiTiet.Clear();
                tblNhapKho.Clear();
                gCNL.DataSource = null;
                gCNLNhapKho.DataSource = null;

            }
            else
            {
                tblPL.Clear();
                tblChiTietPL.Clear();
                tblNhapKhoPL.Clear();
                gCPL.DataSource = null;
                gCPLNhapKho.DataSource = null;
            }
        }

        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\”|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }
        private string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result.ToUpper();
        }
        private bool luuNhapKho()
        {
            try
            {
                //DataTable tblV2 = gCNLNhapKho.DataSource as DataTable;





                //foreach (DataRow item in tblV2.Rows)
                //{
                //    DataRow dataRow = tblNhapKho.NewRow();

                //    dataRow["ID"] = Convert.ToInt64(item["ID"]);
                //    dataRow["SoLoID"] = item["SoLoID"];
                //    dataRow["MaNPL"] = item["MaNPL"];
                //    dataRow["MaVTID"] = item["MaVTID"];
                //    dataRow["MaMauVT"] = item["MaMauVT"];
                //    dataRow["SoKien"] = item["SoKien"];
                //    dataRow["SoLoT"] = item["SoLoT"];
                //    dataRow["MaHaiQuan"] = item["MaHaiQuan"];
                //    dataRow["MaKeToan"] = item["MaKeToan"];
                //    dataRow["SoGhiDauCay"] = item["SoGhiDauCay"];
                //    dataRow["NW"] = item["NW"];
                //    dataRow["GW"] = item["GW"];
                //    dataRow["BarCode"] = item["BarCode"];
                //    dataRow["GhiChu"] = item["GhiChu"];
                //    dataRow["IsNPL"] = item["IsNPL"];
                //    dataRow["KhoVaiID"] = item["KhoVaiID"];
                //    dataRow["SoKienParent"] = item["SoKienParent"];
                //    dataRow["SoLuongThucTe"] = item["SoLuongThucTe"];
                //    dataRow["DonGia"] = item["DonGia"];
                //    dataRow["ThanhTien"] = item["ThanhTien"];
                //    dataRow["Pallet"] = item["Pallet"];
                //    dataRow["MaDVVT"] = item["MaDVVT"];
                //    dataRow["MauVTID"] = item["MauVTID"];
                //    dataRow["SoKienHienThi"] = item["SoKienHienThi"];
                //    dataRow["NgayNKDuKien"] = item["NgayNKDuKien"];
                //    dataRow["MaDVCD"] = item["MaDVCD"];
                //    dataRow["TenDVCD"] = item["TenDVCD"];

                //    tblNhapKho.Rows.Add(dataRow);
                //}

                if (tblNhapKho == null || tblNhapKho.Rows.Count == 0) return false;
                #region Thoai ràng buộc số lượng > 0
                var rowsAm = tblNhapKho.AsEnumerable()
                              .Where(r => r.Table.Columns.Contains("SLTong")
                                          && decimal.TryParse(r["SLTong"].ToString(), out decimal val)
                                          && val < 0)
                              .ToList();

                if (rowsAm.Any())
                {
                    MessageBox.Show("Số lượng tổng không hợp lệ. Vui lòng nhập lại!",
                                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                #endregion
                foreach (DataRow dr in tblNhapKho.Rows)
                {
                    /*if (Convert.ToDouble(dr["SoGhiDauCay"].ToString()) <= 0)
                    {
                        XtraMessageBox.Show("Vui lòng nhập Số lượng theo CT lớn hơn 0", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }*/
                    var barCode = dr.Field<string>("BarCode");
                    if (!string.IsNullOrWhiteSpace(barCode))
                    {
                        var segments = barCode.Split('|');
                        if (segments.Length > 0 && !string.IsNullOrWhiteSpace(segments[segments.Length - 1]))
                        {
                            if (dr["SoKienHienThi"].ToString() != "" && Convert.ToDouble(dr["SoGhiDauCay"].ToString()) <= 0)
                            {
                                XtraMessageBox.Show("Vui lòng nhập Số lượng theo CT lớn hơn 0", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }
                    /*if (tblNhapKho.Columns.Contains("SoLot"))
                    {
                        var soLotValue = dr["SoLot"];
                        if (soLotValue == DBNull.Value || string.IsNullOrWhiteSpace(soLotValue.ToString()))
                        {
                            XtraMessageBox.Show("Vui lòng nhập đầy đủ LoT.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }*/
                }
                //if (KiemTraTrungSoKienHienThi(tblNhapKho))
                //{

                //    XtraMessageBox.Show("Trùng số roll. Vui lòng kiểm tra lại", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return false;
                //}
                var dsSoLuongSai = LayDanhSachSoLuongKhongKhop(tblNhapKho, tblNL);
                if (dsSoLuongSai.Any())
                {
                    var danhSachSai = dsSoLuongSai
                        .Select(row => string.Format(
                            "Chủng loại: {0}, ItemCode: {1}, Mô tả: {2}, Màu: {3}, Khổ/Size: {4}\n" +
                            "  → SL Tổng: {5:N2}, Tổng các Roll: {6:N2}, Chênh lệch: {7:N2} ({8})",
                            row.TenNhom,
                            row.MaVT,
                            row.ChiTiet,
                            row.MauVT,
                            row.KhoVai,
                            row.SLTong,
                            row.TongSoGhi,
                            Math.Abs(row.ChenhLech),
                            row.ChenhLech > 0 ? "Thừa" : "Thiếu"))
                        .Distinct()
                        .ToList();

                    string thongBao = "Các dòng có số lượng không khớp:\n\n" + string.Join("\n\n", danhSachSai);

                    var dialogResult = MessageBox.Show(
                        thongBao + "\n\nBạn có muốn tiếp tục?",
                        "Cảnh báo số lượng không khớp",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (dialogResult == DialogResult.No)
                        return false;
                }
                string allErrors = CheckTrungRollLotBatch(tblNhapKho);
                if (!string.IsNullOrEmpty(allErrors))
                {
                    MessageBox.Show(allErrors, "Thông báo dòng trùng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                //var dsTrung = LayDanhSachTrungTheoID(tblNhapKho, tblNL);

                //if (dsTrung.Any())
                //{
                //    var danhSachTrung = dsTrung
                //     .Select(row => string.Format(
                //         "PI NCC: {0}, ItemCode: {1}, Mô tả: {2}, Màu: {3}, Khổ/Size: {4}, Số Roll: {5}",
                //         row.SoLo, row.MaVT, row.ChiTiet, row.MauVT, row.KhoVai, row.SoKienHienThi))
                //     .Distinct() // Loại bỏ trùng, vì 1 nhóm trùng có thể có >2 dòng giống hệt nhau
                //     .ToList();

                //    string thongBao = "Các dòng trùng:\n\n" + string.Join("\n", danhSachTrung);
                //    MessageBox.Show(thongBao, "Thông báo dòng trùng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return false;
                //}
                foreach (DataRow dr in tblNhapKho.Rows)
                {

                    //dr["SoloID"] = searchLookUpEditLoNL.EditValue.ToString();
                    //dr["NgayNKDuKien"] = dateNgayNK.EditValue == null ? (object)DBNull.Value : dateNgayNK.EditValue;
                    if (string.IsNullOrWhiteSpace(dr["DonGia"].ToString()))
                    {
                        dr["DonGia"] = 0;
                    }
                    dr["POMua"] = txtPOMua.Text.ToString();
                }
                foreach (DataRow dr in tblNhapKho.Rows)
                {
                    if (dr["STTChonVT"] != DBNull.Value)
                    {
                        dr["STTChonVT"] = dr["STTChonVT"];
                    }
                }
                if (tblNhapKho.Columns.Contains("MauVT"))
                {
                    tblNhapKho.Columns.Remove("MauVT");
                }
                if (tblNhapKho.Columns.Contains("TrangThai"))
                {
                    tblNhapKho.Columns.Remove("TrangThai");
                }
                if (tblNhapKho.Columns.Contains("IsNK"))
                {
                    tblNhapKho.Columns.Remove("IsNK");
                }
                if (tblNhapKho.Columns.Contains("STT"))
                {
                    tblNhapKho.Columns.Remove("STT");
                }
                if (tblNhapKho.Columns.Contains("QuyDoiID"))
                {
                    tblNhapKho.Columns.Remove("QuyDoiID");
                }
                if (tblNhapKho.Columns.Contains("SoLo"))
                {
                    tblNhapKho.Columns.Remove("SoLo");
                }
                string url = $"{URL}ERPNhapKhoNPL/Post?Action=POSTCT&para={GlobleData.UserName}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblNhapKho); }).Result;
                if (msResult.ToLower() == "true")
                {
                    loadGVVatTu(true);
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        private bool KiemTraTrungSoKienHienThi(DataTable tblNhapKho)
        {
            var keyList = tblNhapKho.AsEnumerable()
                .Select(row => new
                {
                    MaVTID = row["MaVTID"],
                    MauVTID = row["MauVTID"],
                    KhoVaiID = row["KhoVaiID"],
                    SoKienHienThi = row["SoKienHienThi"]
                }).ToList();

            return keyList.Count != keyList.Distinct().Count();
        }
        private List<dynamic> LayDanhSachSoLuongKhongKhop(DataTable tblNhapKho, DataTable tblNL)
        {
            var nhomSai = tblNhapKho.AsEnumerable()
                .Where(r => !string.IsNullOrWhiteSpace(r.Field<string>("SoLoT")))
                .GroupBy(r => new
                {
                    MaNhom = r["MaNhom"],
                    MaVTID = r["MaVTID"],
                    MauVTID = r["MauVTID"],
                    KhoVaiID = r["KhoVaiID"]
                })
                .Where(g =>
                {
                    decimal slTong = g.First().Field<decimal?>("SLTong") ?? 0m;
                    decimal tongSoGhi = g.Sum(r => r.Field<decimal?>("SoGhiDauCay") ?? 0m);
                    return tongSoGhi != slTong;
                })
                .SelectMany(g =>
                {
                    decimal slTong = g.First().Field<decimal?>("SLTong") ?? 0m;
                    decimal tongSoGhi = g.Sum(r => r.Field<decimal?>("SoGhiDauCay") ?? 0m);

                    return g.Select(row => new
                    {
                        MaNhom = g.Key.MaNhom,
                        MaVTID = g.Key.MaVTID,
                        MauVTID = g.Key.MauVTID,
                        KhoVaiID = g.Key.KhoVaiID,
                        SLTong = slTong,
                        TongSoGhi = tongSoGhi,
                        ChenhLech = tongSoGhi - slTong,
                        DataRowNhapKho = row
                    });
                })
                .ToList<dynamic>();

            // Join với tblNL để lấy thêm thông tin
            var mergedList = (
                from rowSai in nhomSai
                join rowNL in tblNL.AsEnumerable()
                    on new
                    {
                        MaNhom = rowSai.MaNhom,
                        MaVTID = rowSai.MaVTID,
                        MauVTID = rowSai.MauVTID,
                        KhoVaiID = rowSai.KhoVaiID
                    }
                    equals new
                    {
                        MaNhom = rowNL["MaNhom"],
                        MaVTID = rowNL["MaVTID"],
                        MauVTID = rowNL["MauVTID"],
                        KhoVaiID = rowNL["KhoVaiID"]
                    }
                select new
                {
                    MaNhom = rowSai.MaNhom,
                    MaVTID = rowSai.MaVTID,
                    MauVTID = rowSai.MauVTID,
                    KhoVaiID = rowSai.KhoVaiID,
                    SLTong = rowSai.SLTong,
                    TongSoGhi = rowSai.TongSoGhi,
                    ChenhLech = rowSai.ChenhLech,
                    // Thông tin từ tblNL
                    TenNhom = rowNL["TenNhom"],
                    MaVT = rowNL["MaVT"],
                    MaVTGhep = rowNL["MaVTGhep"],
                    ChiTiet = rowNL["ChiTiet"],
                    MauVT = rowNL["MauVT"],
                    KhoVai = rowNL["KhoVai"],
                    DataRowNhapKho = rowSai.DataRowNhapKho
                }
            ).ToList<dynamic>();

            return mergedList;
        }
        private void gVNLNhapKho_KeyDown(object sender, KeyEventArgs e)
        {


            string _fieldName = gVNLNhapKho.FocusedColumn.FieldName;
            if (_fieldName == "SoKien") return;
            HandleGridViewKey(e, gVNLNhapKho, _fieldName);
        }

        private void gVNL_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gVNL_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }

        private void gVNLNhapKho_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gVNLNhapKho_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }

        private void gVPL_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gVPL_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }

        private void gVPLNhapKho_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gVPLNhapKho_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }

        private void gVPLNhapKho_KeyDown(object sender, KeyEventArgs e)
        {
            string _fieldName = gVPLNhapKho.FocusedColumn.FieldName;
            if (_fieldName == "SoKien") return;
            HandleGridViewKey(e, gVPLNhapKho, _fieldName);
        }

        private void gVNLNhapKho_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "TrangThai")
            {
                string value = gVNLNhapKho.GetRowCellValue(e.RowHandle, e.Column)?.ToString();
                if (value == "Đã nhập kho")
                {
                    e.Appearance.ForeColor = Color.Green;
                }
                else if (value == "Chưa nhập kho")
                {
                    e.Appearance.ForeColor = Color.Red;
                }
            }
            if (e.Column.FieldName == "SoGhiDauCay" || e.Column.FieldName == "SoLuongThucTe")
            {
                GridView view = sender as GridView;
                if (view == null || !view.IsValidRowHandle(e.RowHandle)) return;

                object objSoGhiDauCay = view.GetRowCellValue(e.RowHandle, "SoGhiDauCay");
                object objSoLuongThucTe = view.GetRowCellValue(e.RowHandle, "SoLuongThucTe");

                decimal soGhiDauCay = 0m;
                decimal soLuongThucTe = 0m;

                decimal.TryParse(objSoGhiDauCay?.ToString(), out soGhiDauCay);
                decimal.TryParse(objSoLuongThucTe?.ToString(), out soLuongThucTe);

                if (soGhiDauCay != soLuongThucTe)
                {
                    e.Appearance.BackColor = Color.LightBlue;
                    e.Appearance.ForeColor = Color.Red;
                }
                else
                {
                    if (e.Column.FieldName == "SoGhiDauCay")
                    {
                        e.Appearance.BackColor = Color.FromArgb(255, 224, 192);
                        e.Appearance.ForeColor = Color.Black;
                    }
                    else if (e.Column.FieldName == "SoLuongThucTe")
                    {
                        e.Appearance.BackColor = Color.FromArgb(255, 192, 192);
                        e.Appearance.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void gVPLNhapKho_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "TrangThai")
            {
                string value = gVPLNhapKho.GetRowCellValue(e.RowHandle, e.Column)?.ToString();
                if (value == "Đã nhập kho")
                {
                    e.Appearance.ForeColor = Color.Green;
                }
                else if (value == "Chưa nhập kho")
                {
                    e.Appearance.ForeColor = Color.Red;
                }
                if (e.Column.FieldName == "SoGhiDauCay" || e.Column.FieldName == "SoLuongThucTe")
                {
                    GridView view = sender as GridView;
                    if (view == null || !view.IsValidRowHandle(e.RowHandle)) return;

                    object objSoGhiDauCay = view.GetRowCellValue(e.RowHandle, "SoGhiDauCay");
                    object objSoLuongThucTe = view.GetRowCellValue(e.RowHandle, "SoLuongThucTe");

                    decimal soGhiDauCay = 0m;
                    decimal soLuongThucTe = 0m;

                    decimal.TryParse(objSoGhiDauCay?.ToString(), out soGhiDauCay);
                    decimal.TryParse(objSoLuongThucTe?.ToString(), out soLuongThucTe);

                    if (soGhiDauCay != soLuongThucTe)
                    {
                        e.Appearance.BackColor = Color.LightBlue;
                        e.Appearance.ForeColor = Color.Red;
                    }
                    else
                    {
                        if (e.Column.FieldName == "SoGhiDauCay")
                        {
                            e.Appearance.BackColor = Color.FromArgb(255, 224, 192);
                            e.Appearance.ForeColor = Color.Black;
                        }
                        else if (e.Column.FieldName == "SoLuongThucTe")
                        {
                            e.Appearance.BackColor = Color.FromArgb(255, 192, 192);
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                }
            }
        }

        private void gVNL_KeyDown(object sender, KeyEventArgs e)
        {
            string _fieldName = gVNL.FocusedColumn.FieldName;
            if (_fieldName == "MaHaiQuan" || _fieldName == "MaKeToan" || _fieldName == "TongKien")
            {
                HandleGridViewKey(e, gVNL, _fieldName);
            }

        }


        private void gVPL_KeyDown(object sender, KeyEventArgs e)
        {
            string _fieldName = gVPL.FocusedColumn.FieldName;
            if (_fieldName == "MaHaiQuan" || _fieldName == "MaKeToan" || _fieldName == "TongKien")
            {
                HandleGridViewKey(e, gVPL, _fieldName);
            }
        }

        private bool luuPhieu()
        {
            try
            {
                if (tblNhapKho == null || tblNhapKho.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có kiện vật tư được thêm. Vui lòng kiểm tra lại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (!checkTxt()) return false;
                if (tblPhieu == null || tblPhieu.Rows.Count == 0) CreateTablePhieu();
                if (tblPhieu.Rows.Count > 0) tblPhieu.Clear();
                DataRow drVT = gVNL.GetFocusedDataRow();
                if (drVT == null) return false;
                DataRow dr = tblPhieu.NewRow();

                dr["ID"] = 0;
                dr["SoLoID"] = drVT["SoLoID"].ToString();
                dr["SoLo"] = drVT["SoLo"].ToString();


                dr["NhaCungCap"] = searchLookUpEditNCC.EditValue == null ? "" : searchLookUpEditNCC.EditValue.ToString();
                dr["SoChungTu"] = DBNull.Value;
                dr["NgayChungTu"] = DBNull.Value;
                dr["SoBienBan"] = DBNull.Value;
                dr["NgayBienBan"] = DBNull.Value;
                dr["SoHopDong"] = txtSoHopDong.Text.ToString();
                dr["NguoiNhap"] = GlobleData.UserName;
                dr["NgayNhap"] = DBNull.Value;
                dr["IsNPL"] = true;
                dr["MaHaiQuan"] = "";
                dr["MaKeToan"] = "";
                dr["NguoiGui"] = DBNull.Value;
                dr["NgayTao"] = DBNull.Value;
                dr["SoHoaDon"] = txtSoHoaDon.Text.ToString();
                dr["Tau"] =SearchlookupTau.EditValue?.ToString();
                dr["NgayGui"] = dateNgayGui.EditValue == null ? (object)DBNull.Value : dateNgayGui.EditValue.ToString();
                dr["NoiGui"] = txtNoiGui.Text.ToString();
                dr["DiemDen"] = "";
                dr["NguoiNhan"] = DBNull.Value;
                dr["DiaChiNhan"] = DBNull.Value;
                dr["MaSoThue"] = txtMaSoThue.Text.ToString();
                dr["SoDienThoai"] = DBNull.Value;

                dr["Cang"] = searchLookUpEditCang.EditValue == null ? "" : searchLookUpEditCang.EditValue.ToString();
                dr["KhachHang"] = DBNull.Value;
                dr["NgayKyHD"] = dateSoHopDong.EditValue == null ? (object)DBNull.Value : dateSoHopDong.EditValue.ToString();
                dr["SoToKhai"] = txtSoToKhai.Text.ToString();
                dr["NgayMoTK"] = dateNgayToKhai.EditValue == null ? (object)DBNull.Value : dateNgayToKhai.EditValue.ToString();
                dr["SoVanDon"] = txtSoVanDon.Text.ToString();
                dr["NgayNKDuKien"] = dateNgayNK.EditValue == null ? (object)DBNull.Value : dateNgayNK.EditValue.ToString();
                dr["NgayHoaDon"] = dateHoaDon.EditValue == null ? (object)DBNull.Value : dateHoaDon.EditValue.ToString();
                dr["MaKH"] = searchLookUpEditKHCT.EditValue == null ? "" : searchLookUpEditKHCT.EditValue.ToString();
                dr["MaHang"] = searchLookUpEditMHCT.EditValue == null ? "" : searchLookUpEditMHCT.EditValue.ToString();
                dr["POMua"] = txtPOMua.Text.ToString();
                dr["NW"] = string.IsNullOrWhiteSpace(txtNW.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtNW.Text);
                dr["GW"] = string.IsNullOrWhiteSpace(txtGW.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtGW.Text);
                dr["E_Way_Bill"] = txtEWayBill_NL.EditValue;
                tblPhieu.Rows.Add(dr);
                string url = $"{URL}ERPNhapKhoNPL/PostNK?Action=POSTNK&para={GlobleData.UserName}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblPhieu); }).Result;
                if (msResult.ToLower() == "true")
                {
                    return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                return false;

            }

        }
        private bool luuNhapKhoPL()
        {
            try
            {
                if (tblNhapKhoPL == null || tblNhapKhoPL.Rows.Count == 0) return false;
                #region Thoai ràng buộc số lượng > 0
                var rowsAm = tblNhapKhoPL.AsEnumerable()
                              .Where(r => r.Table.Columns.Contains("SLTong")
                                          && decimal.TryParse(r["SLTong"].ToString(), out decimal val)
                                          && val < 0)
                              .ToList();

                if (rowsAm.Any())
                {
                    MessageBox.Show("Số lượng tổng không hợp lệ. Vui lòng nhập lại!",
                                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                #endregion
                foreach (DataRow dr in tblNhapKhoPL.Rows)
                {
                    var barCode = dr.Field<string>("BarCode");
                    if (!string.IsNullOrWhiteSpace(barCode))
                    {
                        var segments = barCode.Split('|');
                        if (segments.Length > 0 && !string.IsNullOrWhiteSpace(segments[segments.Length - 1]))
                        {
                            if (dr["SoKienHienThi"].ToString() != "" && Convert.ToDouble(dr["SoGhiDauCay"].ToString()) <= 0)
                            {
                                XtraMessageBox.Show("Vui lòng nhập Số lượng theo CT lớn hơn 0", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }
                    /* if (Convert.ToDouble(dr["SoGhiDauCay"].ToString()) <= 0)
                     {
                         XtraMessageBox.Show("Vui lòng nhập Số lượng theo CT lớn hơn 0", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                         return false;
                     }*/
                    //if (tblNhapKhoPL.Columns.Contains("SoLot"))
                    //{
                    //    var soLotValue = dr["SoLot"];
                    //    if (soLotValue == DBNull.Value || string.IsNullOrWhiteSpace(soLotValue.ToString()))
                    //    {
                    //        XtraMessageBox.Show("Vui lòng nhập đầy đủ LoT.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //        return false;
                    //    }
                    //}
                }
                //if (KiemTraTrungSoKienHienThi(tblNhapKhoPL))
                //{

                //    XtraMessageBox.Show("Trùng số roll. Vui lòng kiểm tra lại", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return false;
                //}
                var dsSoLuongSai = LayDanhSachSoLuongKhongKhop(tblNhapKhoPL, tblPL);
                if (dsSoLuongSai.Any())
                {
                    var danhSachSai = dsSoLuongSai
                        .Select(row => string.Format(
                            "Chủng loại: {0}, ItemCode: {1}, Mô tả: {2}, Màu: {3}, Khổ/Size: {4}\n" +
                            "  → SL Tổng: {5:N2}, Tổng các Roll: {6:N2}, Chênh lệch: {7:N2} ({8})",
                            row.TenNhom,
                            row.MaVT,
                            row.ChiTiet,
                            row.MauVT,
                            row.KhoVai,
                            row.SLTong,
                            row.TongSoGhi,
                            Math.Abs(row.ChenhLech),
                            row.ChenhLech > 0 ? "Thừa" : "Thiếu"))
                        .Distinct()
                        .ToList();

                    string thongBao = "Các dòng có số lượng không khớp:\n\n" + string.Join("\n\n", danhSachSai);

                    var dialogResult = MessageBox.Show(
                        thongBao + "\n\nBạn có muốn tiếp tục?",
                        "Cảnh báo số lượng không khớp",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (dialogResult == DialogResult.No)
                        return false;
                }
                string allErrors = CheckTrungRollLotBatch(tblNhapKhoPL);
                if (!string.IsNullOrEmpty(allErrors))
                {
                    MessageBox.Show(allErrors, "Thông báo dòng trùng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                //var dsTrung = LayDanhSachTrungTheoID(tblNhapKhoPL, tblPL);

                //if (dsTrung.Any())
                //{
                //    var danhSachTrung = dsTrung
                //     .Select(row => string.Format(
                //         "PI NCC: {0}, Mã vật tư: {1}, Chi tiết: {2}, Màu: {3}, Khổ/Size: {4}, Số Roll: {5}",
                //         row.SoLo, row.MaVT, row.ChiTiet, row.MauVT, row.KhoVai, row.SoKienHienThi))
                //     .Distinct() // Loại bỏ trùng, vì 1 nhóm trùng có thể có >2 dòng giống hệt nhau
                //     .ToList();

                //    string thongBao = "Các dòng trùng:\n\n" + string.Join("\n", danhSachTrung);
                //    MessageBox.Show(thongBao, "Thông báo dòng trùng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return false;
                //}
                foreach (DataRow dr in tblNhapKhoPL.Rows)
                {
                    if (dr["STTChonVT"] != DBNull.Value)
                    {
                        dr["STTChonVT"] = dr["STTChonVT"];
                    }
                }
                if (tblNhapKhoPL.Columns.Contains("MauVT"))
                {
                    tblNhapKhoPL.Columns.Remove("MauVT");
                }
                if (tblNhapKhoPL.Columns.Contains("TrangThai"))
                {
                    tblNhapKhoPL.Columns.Remove("TrangThai");
                }
                if (tblNhapKhoPL.Columns.Contains("IsNK"))
                {
                    tblNhapKhoPL.Columns.Remove("IsNK");
                }

                if (tblNhapKhoPL.Columns.Contains("STT"))
                {
                    tblNhapKhoPL.Columns.Remove("STT");
                }
                if (tblNhapKhoPL.Columns.Contains("QuyDoiID"))
                {
                    tblNhapKhoPL.Columns.Remove("QuyDoiID");
                }
                if (tblNhapKhoPL.Columns.Contains("SoLo"))
                {
                    tblNhapKhoPL.Columns.Remove("SoLo");
                }

                foreach (DataRow dr in tblNhapKhoPL.Rows)
                {

                    //dr["SoloID"] = searchLookUpEditLoPL.EditValue.ToString();
                    //dr["NgayNKDuKien"] = dateNgayNKPL.EditValue == null ? (object)DBNull.Value : dateNgayNKPL.EditValue;
                    if (string.IsNullOrWhiteSpace(dr["DonGia"].ToString()))
                    {
                        dr["DonGia"] = 0;
                    }
                    dr["POMua"] = txtPOMua.Text.ToString();
                }
                string url = $"{URL}ERPNhapKhoNPL/Post?Action=POSTCT&para={GlobleData.UserName}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblNhapKhoPL); }).Result;
                if (msResult.ToLower() == "true")
                {
                    loadGVVatTu(false);
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;

            }

        }
        private bool luuPhieuPL()
        {
            try
            {
                if (tblNhapKhoPL == null || tblNhapKhoPL.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có kiện vật tư được thêm. Vui lòng kiểm tra lại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (!checkTxt()) return false;
                if (tblPhieuPL == null || tblPhieuPL.Rows.Count == 0) CreateTablePhieuPL();
                if (tblPhieuPL.Rows.Count > 0) tblPhieuPL.Clear();
                DataRow drVT = gVPL.GetFocusedDataRow();
                if (drVT == null) return false;
                DataRow dr = tblPhieuPL.NewRow();

                dr["ID"] = 0;
                dr["SoLoID"] = drVT["SoLoID"].ToString();
                dr["SoLo"] = drVT["SoLo"].ToString();
                dr["NhaCungCap"] = searchLookUpEditNCCPL.EditValue == null ? "" : searchLookUpEditNCCPL.EditValue.ToString();
                dr["SoChungTu"] = DBNull.Value;
                dr["NgayChungTu"] = DBNull.Value;
                dr["SoBienBan"] = DBNull.Value;
                dr["NgayBienBan"] = DBNull.Value;
                dr["SoHopDong"] = txtSoHopDongPL.Text.ToString();
                dr["NguoiNhap"] = GlobleData.UserName;
                dr["NgayNhap"] = DBNull.Value;
                dr["IsNPL"] = true;
                dr["MaHaiQuan"] = "";
                dr["MaKeToan"] = "";
                dr["NguoiGui"] = DBNull.Value;
                dr["NgayTao"] = DBNull.Value;
                dr["SoHoaDon"] = txtSoHoaDonPL.Text.ToString();
                dr["Tau"] = SearchlookupTauPL.EditValue?.ToString();
                dr["NgayGui"] = dateNgayGuiPL.EditValue == null ? (object)DBNull.Value : dateNgayGuiPL.EditValue.ToString();
                dr["NoiGui"] = txtNoiGuiPL.Text.ToString();
                dr["DiemDen"] = "";
                dr["NguoiNhan"] = DBNull.Value;
                dr["DiaChiNhan"] = DBNull.Value;
                dr["MaSoThue"] = txtMaSoThuePL.Text.ToString();
                dr["SoDienThoai"] = DBNull.Value;

                dr["Cang"] = searchLookUpEditCangPL.EditValue == null ? "" : searchLookUpEditCangPL.EditValue.ToString();
                dr["KhachHang"] = DBNull.Value;
                dr["NgayKyHD"] = dateSoHopDongPL.EditValue == null ? (object)DBNull.Value : dateSoHopDongPL.EditValue.ToString();
                dr["SoToKhai"] = txtSoToKhaiPL.Text.ToString();
                dr["NgayMoTK"] = dateNgayToKhaiPL.EditValue == null ? (object)DBNull.Value : dateNgayToKhaiPL.EditValue.ToString();
                dr["SoVanDon"] = txtSoVanDonPL.Text.ToString();
                dr["NgayNKDuKien"] = dateNgayNKPL.EditValue == null ? (object)DBNull.Value : dateNgayNKPL.EditValue.ToString();
                dr["NgayHoaDon"] = dateHoaDonPL.EditValue == null ? (object)DBNull.Value : dateHoaDonPL.EditValue.ToString();
                dr["MaKH"] = searchLookUpEditKHCTPL.EditValue == null ? "" : searchLookUpEditKHCTPL.EditValue.ToString();
                dr["MaHang"] = searchLookUpEditMHCTPL.EditValue == null ? "" : searchLookUpEditMHCTPL.EditValue.ToString();
                dr["POMua"] = txtPOMuaPL.Text.ToString();
                dr["NW"] = string.IsNullOrWhiteSpace(txtNWPL.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtNWPL.Text);
                dr["GW"] = string.IsNullOrWhiteSpace(txtGWPL.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtGWPL.Text);
                dr["E_Way_Bill"] = txtEWayBill_PL.EditValue;
                tblPhieuPL.Rows.Add(dr);
                string url = $"{URL}ERPNhapKhoNPL/PostNK?Action=POSTNK&para={GlobleData.UserName}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblPhieuPL); }).Result;
                if (msResult.ToLower() == "true")
                {
                    return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                return false;

            }

        }

        private void txtNW_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void txtGW_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void txtNWPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void txtGWPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void setTypeTxtEdit(object sender, KeyPressEventArgs e)
        {

            TextEdit textEdit = sender as TextEdit;
            string currentText = textEdit.Text;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (currentText.Contains("."))
            {
                int indexOfDot = currentText.IndexOf('.');
                string decimalPart = currentText.Substring(indexOfDot + 1);
                if (decimalPart.Length >= 2 && e.KeyChar != '\b')
                {
                    e.Handled = true;
                    return;
                }
            }
            if (e.KeyChar == '.' && currentText.Contains("."))
            {
                e.Handled = true;
            }
        }
        private bool checkEdit(DataRow dr)
        {
            try
            {

                DataRow _row = (_isNPL ? gVNLNhapKho : gVPLNhapKho).GetFocusedDataRow();
                if (_row["IsNK"].ToString().ToLower() == "false")
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {

                return false;
            }

        }


        #endregion

        #region popupSearchLookUpEdit
        private void searchLookUpEditNCC_Popup(object sender, EventArgs e)
        {
            var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

            var layout = popupForm.Controls
                .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                .FirstOrDefault()
                ?.Controls
                .OfType<LayoutControl>()
                .FirstOrDefault();

            if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
            {
                // Tạo nhóm layout dạng bảng
                var buttonGroup = new LayoutControlGroup()
                {
                    Name = "buttonGroup",
                    TextVisible = false,
                    LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                    Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                    Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                    GroupBordersVisible = false
                };

                // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                // Thêm nhóm vào layout chính
                layout.BeginUpdate();
                layout.Root.AddItem(buttonGroup);

                // 1. Empty space bên trái
                var emptySpaceItemLeft = new EmptySpaceItem();
                emptySpaceItemLeft.AllowHotTrack = false;
                emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(emptySpaceItemLeft);

                // 2. Nút "Khai báo Khổ/Size"
                var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo Nhà cung cấp" };
                khaiBaoButton.Click += khaiBaoNCCButton_Click;
                var layoutItemKhaiBao = new LayoutControlItem()
                {
                    Control = khaiBaoButton,
                    TextVisible = false,
                    SizeConstraintsType = SizeConstraintsType.Custom,
                    MinSize = new Size(140, 30),
                    MaxSize = new Size(140, 30)
                };
                layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(layoutItemKhaiBao);

                // 3. Empty space trên và dưới để căn giữa
                var emptySpaceItemTop = new EmptySpaceItem();
                emptySpaceItemTop.AllowHotTrack = false;
                emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemTop);

                var emptySpaceItemBottom = new EmptySpaceItem();
                emptySpaceItemBottom.AllowHotTrack = false;
                emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemBottom);

                layout.EndUpdate();
            }
        }



        private void searchLookUpEditNCCPL_Popup(object sender, EventArgs e)
        {
            var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

            var layout = popupForm.Controls
                .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                .FirstOrDefault()
                ?.Controls
                .OfType<LayoutControl>()
                .FirstOrDefault();

            if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
            {
                // Tạo nhóm layout dạng bảng
                var buttonGroup = new LayoutControlGroup()
                {
                    Name = "buttonGroup",
                    TextVisible = false,
                    LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                    Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                    Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                    GroupBordersVisible = false
                };

                // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                // Thêm nhóm vào layout chính
                layout.BeginUpdate();
                layout.Root.AddItem(buttonGroup);

                // 1. Empty space bên trái
                var emptySpaceItemLeft = new EmptySpaceItem();
                emptySpaceItemLeft.AllowHotTrack = false;
                emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(emptySpaceItemLeft);

                // 2. Nút "Khai báo Khổ/Size"
                var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo Nhà cung cấp" };
                khaiBaoButton.Click += khaiBaoNCCButton_Click;
                var layoutItemKhaiBao = new LayoutControlItem()
                {
                    Control = khaiBaoButton,
                    TextVisible = false,
                    SizeConstraintsType = SizeConstraintsType.Custom,
                    MinSize = new Size(140, 30),
                    MaxSize = new Size(140, 30)
                };
                layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(layoutItemKhaiBao);

                // 3. Empty space trên và dưới để căn giữa
                var emptySpaceItemTop = new EmptySpaceItem();
                emptySpaceItemTop.AllowHotTrack = false;
                emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemTop);

                var emptySpaceItemBottom = new EmptySpaceItem();
                emptySpaceItemBottom.AllowHotTrack = false;
                emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemBottom);

                layout.EndUpdate();
            }
        }


        private void khaiBaoNCCButton_Click(object sender, EventArgs e)
        {
            frmERPNhaCungCapNK frm = new frmERPNhaCungCapNK();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            CreateSearchLookUpNCC();
        }
        private void searchLookUpEditKH_Popup(object sender, EventArgs e)
        {
            var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

            var layout = popupForm.Controls
                .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                .FirstOrDefault()
                ?.Controls
                .OfType<LayoutControl>()
                .FirstOrDefault();

            if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
            {
                // Tạo nhóm layout dạng bảng
                var buttonGroup = new LayoutControlGroup()
                {
                    Name = "buttonGroup",
                    TextVisible = false,
                    LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                    Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                    Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                    GroupBordersVisible = false
                };

                // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                // Thêm nhóm vào layout chính
                layout.BeginUpdate();
                layout.Root.AddItem(buttonGroup);

                // 1. Empty space bên trái
                var emptySpaceItemLeft = new EmptySpaceItem();
                emptySpaceItemLeft.AllowHotTrack = false;
                emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(emptySpaceItemLeft);

                // 2. Nút "Khai báo Khổ/Size"
                var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo Khách hàng" };
                khaiBaoButton.Click += khaiBaoKHButton_Click;
                var layoutItemKhaiBao = new LayoutControlItem()
                {
                    Control = khaiBaoButton,
                    TextVisible = false,
                    SizeConstraintsType = SizeConstraintsType.Custom,
                    MinSize = new Size(140, 30),
                    MaxSize = new Size(140, 30)
                };
                layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(layoutItemKhaiBao);

                // 3. Empty space trên và dưới để căn giữa
                var emptySpaceItemTop = new EmptySpaceItem();
                emptySpaceItemTop.AllowHotTrack = false;
                emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemTop);

                var emptySpaceItemBottom = new EmptySpaceItem();
                emptySpaceItemBottom.AllowHotTrack = false;
                emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemBottom);

                layout.EndUpdate();
            }
        }


        private void searchLookUpEditKHPL_Popup(object sender, EventArgs e)
        {
            var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

            var layout = popupForm.Controls
                .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                .FirstOrDefault()
                ?.Controls
                .OfType<LayoutControl>()
                .FirstOrDefault();

            if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
            {
                // Tạo nhóm layout dạng bảng
                var buttonGroup = new LayoutControlGroup()
                {
                    Name = "buttonGroup",
                    TextVisible = false,
                    LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                    Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                    Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                    GroupBordersVisible = false
                };

                // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                // Thêm nhóm vào layout chính
                layout.BeginUpdate();
                layout.Root.AddItem(buttonGroup);

                // 1. Empty space bên trái
                var emptySpaceItemLeft = new EmptySpaceItem();
                emptySpaceItemLeft.AllowHotTrack = false;
                emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(emptySpaceItemLeft);

                // 2. Nút "Khai báo Khổ/Size"
                var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo Khách hàng" };
                khaiBaoButton.Click += khaiBaoKHButton_Click;
                var layoutItemKhaiBao = new LayoutControlItem()
                {
                    Control = khaiBaoButton,
                    TextVisible = false,
                    SizeConstraintsType = SizeConstraintsType.Custom,
                    MinSize = new Size(140, 30),
                    MaxSize = new Size(140, 30)
                };
                layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(layoutItemKhaiBao);

                // 3. Empty space trên và dưới để căn giữa
                var emptySpaceItemTop = new EmptySpaceItem();
                emptySpaceItemTop.AllowHotTrack = false;
                emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemTop);

                var emptySpaceItemBottom = new EmptySpaceItem();
                emptySpaceItemBottom.AllowHotTrack = false;
                emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemBottom);

                layout.EndUpdate();
            }
        }



        private void khaiBaoKHButton_Click(object sender, EventArgs e)
        {
            frmERPKhachHangNK frm = new frmERPKhachHangNK();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
        }


        private void searchLookUpEditCang_Popup(object sender, EventArgs e)
        {
            var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

            var layout = popupForm.Controls
                .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                .FirstOrDefault()
                ?.Controls
                .OfType<LayoutControl>()
                .FirstOrDefault();

            if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
            {
                // Tạo nhóm layout dạng bảng
                var buttonGroup = new LayoutControlGroup()
                {
                    Name = "buttonGroup",
                    TextVisible = false,
                    LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                    Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                    Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                    GroupBordersVisible = false
                };

                // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                // Thêm nhóm vào layout chính
                layout.BeginUpdate();
                layout.Root.AddItem(buttonGroup);

                // 1. Empty space bên trái
                var emptySpaceItemLeft = new EmptySpaceItem();
                emptySpaceItemLeft.AllowHotTrack = false;
                emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(emptySpaceItemLeft);

                // 2. Nút "Khai báo Khổ/Size"
                var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo Cảng" };
                khaiBaoButton.Click += khaiBaoCangButton_Click;
                var layoutItemKhaiBao = new LayoutControlItem()
                {
                    Control = khaiBaoButton,
                    TextVisible = false,
                    SizeConstraintsType = SizeConstraintsType.Custom,
                    MinSize = new Size(140, 30),
                    MaxSize = new Size(140, 30)
                };
                layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(layoutItemKhaiBao);

                // 3. Empty space trên và dưới để căn giữa
                var emptySpaceItemTop = new EmptySpaceItem();
                emptySpaceItemTop.AllowHotTrack = false;
                emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemTop);

                var emptySpaceItemBottom = new EmptySpaceItem();
                emptySpaceItemBottom.AllowHotTrack = false;
                emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemBottom);

                layout.EndUpdate();
            }
        }



        private void searchLookUpEditCangPL_Popup(object sender, EventArgs e)
        {
            var popupForm = (sender as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;

            var layout = popupForm.Controls
                .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                .FirstOrDefault()
                ?.Controls
                .OfType<LayoutControl>()
                .FirstOrDefault();

            if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnKhaiBao"))
            {
                // Tạo nhóm layout dạng bảng
                var buttonGroup = new LayoutControlGroup()
                {
                    Name = "buttonGroup",
                    TextVisible = false,
                    LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                    Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                    Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                    GroupBordersVisible = false
                };

                // Thiết lập 3 cột: EmptySpace - btnKhaiBao
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 }); // cột trống đẩy nút sang phải
                buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                // Thiết lập 3 hàng: EmptySpace (trên) - buttonGroup - EmptySpace (dưới)
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
                buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

                // Thêm nhóm vào layout chính
                layout.BeginUpdate();
                layout.Root.AddItem(buttonGroup);

                // 1. Empty space bên trái
                var emptySpaceItemLeft = new EmptySpaceItem();
                emptySpaceItemLeft.AllowHotTrack = false;
                emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
                emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(emptySpaceItemLeft);

                // 2. Nút "Khai báo Khổ/Size"
                var khaiBaoButton = new SimpleButton() { Name = "btnKhaiBao", Text = "Khai báo Cảng" };
                khaiBaoButton.Click += khaiBaoCangButton_Click;
                var layoutItemKhaiBao = new LayoutControlItem()
                {
                    Control = khaiBaoButton,
                    TextVisible = false,
                    SizeConstraintsType = SizeConstraintsType.Custom,
                    MinSize = new Size(140, 30),
                    MaxSize = new Size(140, 30)
                };
                layoutItemKhaiBao.OptionsTableLayoutItem.ColumnIndex = 1;
                layoutItemKhaiBao.OptionsTableLayoutItem.RowIndex = 1; // Đặt ở hàng giữa
                buttonGroup.AddItem(layoutItemKhaiBao);

                // 3. Empty space trên và dưới để căn giữa
                var emptySpaceItemTop = new EmptySpaceItem();
                emptySpaceItemTop.AllowHotTrack = false;
                emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0; // Hàng trên
                emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemTop);

                var emptySpaceItemBottom = new EmptySpaceItem();
                emptySpaceItemBottom.AllowHotTrack = false;
                emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2; // Hàng dưới
                emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                buttonGroup.AddItem(emptySpaceItemBottom);

                layout.EndUpdate();
            }
        }



        private void khaiBaoCangButton_Click(object sender, EventArgs e)
        {
            frmERPCangNK frm = new frmERPCangNK();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            CreateSearchLookUpCang();
        }

        #endregion



        private void btnTau_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            frmERPNhapKhoNPL_ChonTau frm = new frmERPNhapKhoNPL_ChonTau();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            string tentau = frm.getTau();
            //btnTau.Text = tentau;
        }



        private void searchLookUpEditTauPL_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            frmERPNhapKhoNPL_ChonTau frm = new frmERPNhapKhoNPL_ChonTau();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            string tentau = frm.getTau();
            //btnTauPL.Text = tentau;
        }



        private void btnQRCode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow drMain = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
            DataTable tblDetail = (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource as DataTable;
            if (drMain == null || tblDetail == null || tblDetail.Rows.Count == 0) return;
            List<QrcodeVatTuEntity> lst = new List<QrcodeVatTuEntity>();
            string SoLo = (_isNPL ? searchLookUpEditLoNL.EditValue : searchLookUpEditLoPL.EditValue) == null ? "" : (_isNPL ? searchLookUpEditLoNL.Text : searchLookUpEditLoPL.Text).ToString();
            string POMua = _isNPL ? txtPOMua.Text : txtPOMuaPL.Text;
            var selectedRow = (_isNPL ? searchLookUpEditVatTu.Properties.GetRowByKeyValue(searchLookUpEditVatTu.EditValue) : searchLookUpEditVatTuPL.Properties.GetRowByKeyValue(searchLookUpEditVatTuPL.EditValue));
            if (selectedRow != null)
            {
                var drv = selectedRow as DataRowView;
                SoLo = drv["DisplayMember"].ToString();
            }

            //string mahang = (_isNPL ? searchLookUpEditMHCT.EditValue : searchLookUpEditMHCTPL.EditValue) == null ? "" : (_isNPL ? searchLookUpEditMHCT.Text : searchLookUpEditMHCTPL.Text).ToString();
            //string soloGhep = SoLo + mahang == "" ? "" : "|" + mahang;
            foreach (DataRow dr in tblDetail.Rows)
            {
                if (dr["Barcode"].ToString() == "" || dr["SoKienHienThi"].ToString() == "") continue;
                string maMauVT = dr["MaMauVT"]?.ToString() ?? "";
                string tenMauVT = dr["MauVT"]?.ToString() ?? "";
                string soLot = dr["SoLot"].ToString();
                string batch = dr["Batch"].ToString();
                QrcodeVatTuEntity itemQR = new QrcodeVatTuEntity();
                itemQR.SoKien = dr["SoKienHienThi"].ToString();
                itemQR.IsKK = dr["IsKiemKe"] != DBNull.Value && Convert.ToBoolean(dr["IsKiemKe"]);
                string _solotBatch = soLot + (string.IsNullOrEmpty(batch) ? "" : $"/{batch}");
                itemQR.SoLot = _solotBatch;
                itemQR.GhiChu = POMua;
                itemQR.MauVT = $"{maMauVT}/{tenMauVT}";
                itemQR.SoLuongThucTe = dr["SoGhiDauCay"].ToString();
                itemQR.BarCode = dr["BarCode"].ToString();
                itemQR.TenNhom = drMain["MaVT"].ToString();
                itemQR.KhoVai = drMain["KhoVai"].ToString();
                itemQR.ChiTiet = $"{drMain["ChiTiet"].ToString()}-{itemQR.KhoVai}";
                itemQR.Para1 = dateNgayNK.EditValue != null ? Convert.ToDateTime(dateNgayNK.EditValue).ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");


                itemQR.Para2 = dr["NgayKiemKe"] != DBNull.Value ? Convert.ToDateTime(dr["NgayKiemKe"]).ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy");
                itemQR.TenDVVT = drMain["TenDVVT"].ToString();
                lst.Add(itemQR);
            }
            if (lst.Count == 0)
            {
                XtraMessageBox.Show("Không có vật tư để xuất QRCode.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            frmQRCodeViewer frm = new frmQRCodeViewer("QrcodeVT", lst, _isNPL);
            frm.ShowDialog();
        }
        #region Excel
        private void btnMauExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("NhapKhoNPL{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateNhapKhoNPL.xlsx";
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
                if ((_isNPL ? gCNL : gCPL).ContainsFocus || (_isNPL ? gCNL : gCPL).FocusedView == (_isNPL ? gVNL : gVPL))
                {
                    DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                    if (dr == null) return;
                    FileInfo resultFile = new FileInfo(ExportFileName);
                    FileInfo templateFile = new FileInfo(TemplateFileName);
                    OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                    using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(templateFile))
                    {
                        var worksheet = excelPackage.Workbook.Worksheets[0];
                        int startRowInExcel = 1;

                        int maxRow = 5000;
                        for (int r = 1; r < maxRow; r++)
                        {
                            worksheet.Cells[startRowInExcel + r, 1].Value = dr["TenNhom"];
                            worksheet.Cells[startRowInExcel + r, 2].Value = dr["MaVT"];
                            worksheet.Cells[startRowInExcel + r, 3].Value = dr["ChiTiet"];
                            worksheet.Cells[startRowInExcel + r, 4].Value = dr["MauVT"];
                            worksheet.Cells[startRowInExcel + r, 5].Value = dr["KhoVai"];
                        }
                        excelPackage.SaveAs(resultFile);
                    }
                }

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

                        excelPackage.Workbook.Properties.Author = "NTB";
                        excelPackage.Workbook.Properties.Title = "NhapKhoNPL";

                        string templateFilePath = TemplateFileName;
                        string resultFilePath = ExportFileName;
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);
                        FileInfo resultFile = new FileInfo(resultFilePath);
                        templatePackage.SaveAs(resultFile);
                    }
                }
            }
            catch (Exception ex)
            {
                //DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                //object misValue = System.Reflection.Missing.Value;
                //throw new Exception(ex.Message);
            }


        }
        private void btnExcel_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            btnExcelImport();

        }
        private void btnExcelPL_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btnExcelImport();
        }

        private void gVNL_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn24)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gVNL.IsGroupRow(e.RowHandle))
            {
                int groupIndex = gVNL.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }

        private void gVPL_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn63)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gVPL.IsGroupRow(e.RowHandle))
            {
                int groupIndex = gVPL.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }

        private void gVSearchVT_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn90)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

        }

        private void gVSearchVTPL_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {

            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn150)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

        }

        private void btnExcelImport()
        {
            DataTable dt = (_isNPL ? gCNL : gCPL).DataSource as DataTable;
            if (dt == null | dt.Rows.Count == 0) return;
            DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
            if (dr == null) return;


            //if (string.IsNullOrWhiteSpace(dr["DonGia"].ToString()) || dr["DonGia"].ToString() == "0")
            //{
            //    MessageBox.Show("Vui lòng nhập đơn giá.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            decimal _sk = 0;
            DataTable _dtSK = (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource as DataTable;
            if (_dtSK != null | _dtSK.Rows.Count != 0)
            {
                _sk = _dtSK.AsEnumerable()
                .Where(row => decimal.TryParse(row["SoKien"].ToString(), out _))
                .Select(row => decimal.Parse(row["SoKien"].ToString()))
                .DefaultIfEmpty(0)
                .Max();
            }
            _sk += 1;
            frmERPNhapKhoNPL_Excel frm = new frmERPNhapKhoNPL_Excel(dr, dt);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.WindowState = FormWindowState.Normal;
            frm.ShowDialog();
            if (frm.DialogResult != DialogResult.OK) return;
            DataTable tbl = frm.getData();
            if (tbl == null || tbl.Rows.Count == 0) return;
            bool isAllInt = true;
            List<int> errorRows = new List<int>();

            //for (int i = 0; i < tbl.Rows.Count; i++)
            //{
            //    var value = tbl.Rows[i][0]?.ToString();
            //    int temp;
            //    if (!int.TryParse(value, out temp))
            //    {
            //        isAllInt = false;
            //        errorRows.Add(i + 2);
            //    }

            //}

            //if (!isAllInt)
            //{
            //    string errorMsg = "Có giá trị không hợp lệ trong cột Số kiện ở dòng: " + string.Join(", ", errorRows);

            //    MessageBox.Show(errorMsg, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            //if (!KiemTraCacCotDecimal(tbl))
            //{
            //    XtraMessageBox.Show("File import có cột không đúng kiểu số. Vui lòng kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            if (!checkTrung(_dtSK, tbl)) return;
            int tongkienExcel = tbl.Rows.Count;
            if (string.IsNullOrWhiteSpace(dr["TongKien"].ToString()))
            {
                dr["TongKien"] = tongkienExcel;
            }
            else
            {
                dr["TongKien"] = Convert.ToInt32(dr["TongKien"].ToString()) + tongkienExcel;

            }
            (_isNPL ? gVNL : gVPL).RefreshData();
            string maVTID = dr["MaVTID"].ToString();
            string mauVTID = dr["MauVTID"].ToString();
            string maMauVT = dr["MaMauVT"].ToString();
            string khoVaiID = dr["KhoVaiID"].ToString();
            string MaNhom = dr["MaNhom"].ToString();
            string _soloID = dr["SoLoID"].ToString();
            string _manpl = RemoveVietnameseTone(ReplaceSpecialCharacters(dr["MaNhom"].ToString()) + "@" + ReplaceSpecialCharacters(dr["MaVTID"].ToString()) + "@" + ReplaceSpecialCharacters(dr["MauVTID"].ToString()) + "@" + ReplaceSpecialCharacters(dr["KhoVaiID"].ToString()));
            foreach (DataRow _drr in tbl.Rows)
            {
                DataRow _nr = (_isNPL ? tblNhapKho : tblNhapKhoPL).NewRow();

                _nr["ID"] = 0;
                _nr["SoLoID"] = _soloID;
                _nr["MaNPL"] = _manpl;
                _nr["MaVTID"] = dr["MaVTID"];
                _nr["MaMauVT"] = dr["MaMauVT"];
                _nr["MauVT"] = dr["MauVT"];
                _nr["SoKien"] = _sk;
                _nr["Batch"] = _drr[6];
                _nr["SoLot"] = _drr[7];
                _nr["MaHaiQuan"] = dr["MaHaiQuan"];
                _nr["MaKeToan"] = dr["MaKeToan"];
                _nr["SoGhiDauCay"] = _drr[8];
                _nr["NW"] = _drr[9].ToString() == "" ? 0 : _drr[9];
                _nr["GW"] = _drr[10].ToString() == "" ? 0 : _drr[10];
                _nr["BarCode"] = "";
                _nr["GhiChu"] = _drr[11];
                _nr["IsNPL"] = _isNPL;
                _nr["KhoVaiID"] = dr["KhoVaiID"];
                _nr["SoKienParent"] = "";
                _nr["DonGia"] = dr["DonGia"];
                _nr["ThanhTien"] = 0;
                _nr["SoLuongThucTe"] = 0;
                _nr["Pallet"] = "";
                _nr["MaDVVT"] = dr["MaDVVT"];
                _nr["MauVTID"] = dr["MauVTID"];
                _nr["SoKienHienThi"] = _drr[5];
                _nr["IsNK"] = false;
                _nr["MaDVCD"] = dr["MaDVVT"];
                _nr["TenDVCD"] = dr["TenDVVT"];
                _nr["TienTe"] = dr["TienTe"];
                _nr["MaNhom"] = MaNhom;

                _sk++;
                (_isNPL ? tblNhapKho : tblNhapKhoPL).Rows.Add(_nr);
            }
            DataTable tblNhapKhoFiltered = (_isNPL ? tblNhapKho : tblNhapKhoPL).Clone();
            var filteredRows = (_isNPL ? tblNhapKho : tblNhapKhoPL).AsEnumerable()
                .Where(row => row.Field<string>("MaVTID") == maVTID &&
                               row.Field<string>("MauVTID") == mauVTID &&
                              row.Field<string>("KhoVaiID") == khoVaiID &&
                              row.Field<string>("MaNhom") == MaNhom);
            foreach (var row in filteredRows)
            {
                tblNhapKhoFiltered.ImportRow(row);
            }

            (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource = tblNhapKhoFiltered;
        }

        #region Thoai viết check dòng excel trùng với dữ liệu trên lưới
        private bool checkTrung(DataTable _dtSK, DataTable tbl)
        {
            var allKeys = new HashSet<string>(
                _dtSK.AsEnumerable()
                     .Where(row => !string.IsNullOrWhiteSpace(row["SoLot"]?.ToString()) &&
                                   !string.IsNullOrWhiteSpace(row["SoKienHienThi"]?.ToString()))
                     .Select(row =>
                     {
                         string lot = row["SoLot"].ToString().Trim().ToUpper();
                         string roll = row["SoKienHienThi"].ToString().Trim().ToUpper();
                         return $"{lot}|{roll}";
                     })
            );

            List<int> errorRows = new List<int>();

            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                DataRow row = tbl.Rows[i];

                string excelLot = row[7]?.ToString()?.Trim().ToUpper() ?? "";
                string excelRoll = row[5]?.ToString()?.Trim().ToUpper() ?? "";

                if (string.IsNullOrWhiteSpace(excelLot) && string.IsNullOrWhiteSpace(excelRoll))
                {
                    continue;
                }

                string excelKey = $"{excelLot}|{excelRoll}";
                int excelRowNumber = i + 2;

                if (!allKeys.Add(excelKey))
                {
                    errorRows.Add(excelRowNumber);
                }
            }
            if (errorRows.Count > 0)
            {
                string msg = "Dữ liệu (LOT và Số Roll) bị trùng lặp.\n" +
                             "Vui lòng xem lại các dòng sau trong file excel: " +
                             string.Join(", ", errorRows.Distinct());
                XtraMessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
        #endregion

        //Thoai cmt
        /*private bool checkTrung(DataTable _dtSK, DataTable tbl)
        {
            HashSet<string> soKienSet = new HashSet<string>(
                                                            _dtSK.AsEnumerable()
                                                                 .Select(row => row["SoKienHienThi"]?.ToString())
                                                        );
            bool isDuplicated = false;
            List<int> duplicatedRows = new List<int>();

            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                var value = tbl.Rows[i][4]?.ToString();
                if (soKienSet.Contains(value))
                {
                    isDuplicated = true;
                    duplicatedRows.Add(i + 1); // lưu lại số thứ tự dòng
                }
            }
            if (isDuplicated)
            {
                string msg = "Các giá trị ở cột Số kiện bị trùng với Số kiện (có sẵn) ở các dòng: " + string.Join(", ", duplicatedRows);
                MessageBox.Show(msg, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }*/

        public bool KiemTraCacCotDecimal(DataTable tbl)
        {

            int[] columnIndexes = { 8, 9, 10 }; //các cột theoCT, Trọng lượng, khối lượng
            foreach (DataRow row in tbl.Rows)
            {
                foreach (int colIdx in columnIndexes)
                {
                    object value = row[colIdx];
                    if (value != DBNull.Value && !string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        decimal tmp;
                        if (!decimal.TryParse(value.ToString(), out tmp))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        #endregion
        private void resetTongKien()
        {
            DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
            DataTable tbl = (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource as DataTable;
            int _tk = 0;
            if (tbl == null || tbl.Rows.Count == 0)
            {
                dr["TongKien"] = 0;
                return;
            }

            int count = tbl.AsEnumerable().Count(r => !string.IsNullOrWhiteSpace(r.Field<string>("SoLoT")));
            dr["TongKien"] = count;

        }


        private void txtTienTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.ToUpper(e.KeyChar);
        }
        private void txtTienToPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Char.ToUpper(e.KeyChar);
        }
        private void txtTuKien_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }


        private void txtDenKien_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void txtTuKienPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }



        private void txtDenKienPL_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }



        private void searchLookUpEditVatTu_EditValueChanged(object sender, EventArgs e)
        {
            DataRow dr = gVSearchVT.GetFocusedDataRow();
            if (dr == null) return;
            loadGVVatTu(true);
            //loadSuaVatTu(dr);
        }

        private void gVNLNhapKho_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            var view = sender as GridView;
            if (view == null) return;
            string soKien = Convert.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "SoKienHienThi"));
            string barcode = Convert.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "Barcode"));
            if (string.IsNullOrWhiteSpace(soKien) && string.IsNullOrWhiteSpace(barcode))
            {
                e.Visible = false;
                e.Handled = true;
            }
        }

        private void gVPLNhapKho_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            var view = sender as GridView;
            if (view == null) return;
            string soKien = Convert.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "SoKienHienThi"));
            string barcode = Convert.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "Barcode"));
            if (string.IsNullOrWhiteSpace(soKien) && string.IsNullOrWhiteSpace(barcode))
            {
                e.Visible = false;
                e.Handled = true;
            }
        }

        private void gVNL_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "NgayKK")
            {
                try
                {
                    GridView view = sender as GridView;
                    bool isKiemKe = Convert.ToBoolean(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "IsKiemKe"));

                    if (!isKiemKe)
                    {
                        e.DisplayText = "";
                    }
                    else if (e.Value != null && e.Value != DBNull.Value)
                    {
                        e.DisplayText = Convert.ToDateTime(e.Value).ToString("dd/MM/yyyy");
                    }
                }
                catch (Exception)
                {
                    e.DisplayText = "";
                }
            }
        }

        private void gVPL_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "NgayKKPL")
            {
                try
                {
                    GridView view = sender as GridView;
                    bool isKiemKe = Convert.ToBoolean(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "IsKiemKe"));

                    if (!isKiemKe)
                    {
                        e.DisplayText = "";
                    }
                    else if (e.Value != null && e.Value != DBNull.Value)
                    {
                        e.DisplayText = Convert.ToDateTime(e.Value).ToString("dd/MM/yyyy");
                    }
                }
                catch (Exception)
                {
                    e.DisplayText = "";
                }
            }
        }

        private void searchLookUpEditVatTuPL_EditValueChanged(object sender, EventArgs e)
        {
            DataRow dr = gVSearchVTPL.GetFocusedDataRow();
            if (dr == null) return;
            loadGVVatTu(false);
            //loadSuaVatTu(dr);
        }

        private void gVNL_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                try
                {
                    DataRow dr = gVNL.GetFocusedDataRow();
                    if (dr == null) return;
                    if (tblNhapKho == null || tblNhapKho.Rows.Count == 0) CreateTableNhapKhoPL();
                    string maVTID = dr["MaVTID"].ToString();
                    string maMauVT = dr["MauVTID"].ToString();
                    string khoVaiID = dr["KhoVaiID"].ToString();
                    string SoLoID = dr["SoLoID"].ToString();
                    string MaNhom = dr["MaNhom"].ToString();
                    DataTable tblNhapKhoFiltered = tblNhapKho.Clone();
                    var filteredRows = tblNhapKho.AsEnumerable()
                    .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                  row.Field<string>("MauVTID") == maMauVT &&
                                  row.Field<string>("KhoVaiID") == khoVaiID &&
                                  row.Field<string>("SoLoID") == SoLoID &&
                                row.Field<string>("MaNhom") == MaNhom
                                  );
                    foreach (var row in filteredRows)
                    {
                        var barCode = row.Field<string>("BarCode");
                        if (!string.IsNullOrWhiteSpace(barCode))
                        {
                            var segments = barCode.Split('|');
                            if (segments.Length > 0 && !string.IsNullOrWhiteSpace(segments[segments.Length - 1]))
                            {
                                tblNhapKhoFiltered.ImportRow(row);
                            }
                        }
                    }


                    gCNLNhapKho.DataSource = tblNhapKhoFiltered;
                    _rowFocus = e.FocusedRowHandle;
                    loadTxtV2(true, SoLoID);
                }
                catch (Exception ex)
                {
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void xtraTabControl1_Click(object sender, EventArgs e)
        {

        }

        private void loadTxtV2(bool isNL, string SoLoID)
        {
            string url = $"{URL}ERPNhapKhoNPL/Get?Action=GETNK&para={SoLoID}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblTxt = JsonConvert.DeserializeObject<DataTable>(json);
            if (tblTxt == null || tblTxt.Rows.Count == 0) return;
            if (isNL)
            {
                searchLookUpEditLoNL.Text = tblTxt.Rows[0]["SoLo"].ToString();
                searchLookUpEditNCC.EditValue = tblTxt.Rows[0]["NhaCungCap"].ToString();
                txtSoToKhai.Text = tblTxt.Rows[0]["SoToKhai"].ToString();
                dateNgayToKhai.EditValue = tblTxt.Rows[0]["NgayMoTK"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayMoTK"]) : null;
                txtSoHopDong.Text = tblTxt.Rows[0]["SoHopDong"].ToString();
                dateSoHopDong.EditValue = tblTxt.Rows[0]["NgayKyHD"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayKyHD"]) : null;
                txtMaSoThue.Text = tblTxt.Rows[0]["MaSoThue"].ToString();
                txtSoHoaDon.Text = tblTxt.Rows[0]["SoHoaDon"].ToString();
                searchLookUpEditCang.EditValue = tblTxt.Rows[0]["Cang"].ToString();
                SearchlookupTauPL.EditValue = tblTxt.Rows[0]["Tau"]?.ToString();
                dateNgayGui.EditValue = tblTxt.Rows[0]["NgayGui"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayGui"]) : null;
                txtNoiGui.Text = tblTxt.Rows[0]["NoiGui"].ToString();
                txtPOMua.Text = tblTxt.Rows[0]["POMua"].ToString();
                txtSoVanDon.Text = tblTxt.Rows[0]["SoVanDon"].ToString();
                txtQCKiem.Text = tblTxt.Rows[0]["QCKiemDisPlay"].ToString();
                if (txtQCKiem.Text == "PASS")
                {
                    txtQCKiem.ForeColor = Color.Green;
                }
                else if (txtQCKiem.Text == "FAIL")
                {
                    txtQCKiem.ForeColor = Color.Red;
                }
                else
                {
                    txtQCKiem.ForeColor = Color.Black;
                }
                txtNW.Text = tblTxt.Rows[0]["NW"].ToString();
                txtGW.Text = tblTxt.Rows[0]["GW"].ToString();
                dateNgayNK.EditValue = tblTxt.Rows[0]["NgayNKDuKien"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayNKDuKien"]) : null;
                searchLookUpEditKHCT.EditValue = tblTxt.Rows[0]["MaKH"].ToString();
                dateHoaDon.EditValue = tblTxt.Rows[0]["NgayHoaDon"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayHoaDon"]) : null;
                searchLookUpEditMHCT.EditValue = tblTxt.Rows[0]["MaHang"].ToString();
            }
            else
            {
                searchLookUpEditLoPL.Text = tblTxt.Rows[0]["SoLo"].ToString();
                searchLookUpEditNCCPL.EditValue = tblTxt.Rows[0]["NhaCungCap"].ToString();
                txtSoToKhaiPL.Text = tblTxt.Rows[0]["SoToKhai"].ToString();
                dateNgayToKhaiPL.EditValue = tblTxt.Rows[0]["NgayMoTK"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayMoTK"]) : null;
                txtSoHopDongPL.Text = tblTxt.Rows[0]["SoHopDong"].ToString();
                dateSoHopDongPL.EditValue = tblTxt.Rows[0]["NgayKyHD"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayKyHD"]) : null;
                txtMaSoThuePL.Text = tblTxt.Rows[0]["MaSoThue"].ToString();
                txtSoHoaDonPL.Text = tblTxt.Rows[0]["SoHoaDon"].ToString();
                searchLookUpEditCangPL.EditValue = tblTxt.Rows[0]["Cang"].ToString();
                SearchlookupTau.EditValue = tblTxt.Rows[0]["MaTau"]?.ToString();
                dateNgayGuiPL.EditValue = tblTxt.Rows[0]["NgayGui"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayGui"]) : null;
                txtNoiGuiPL.Text = tblTxt.Rows[0]["NoiGui"].ToString();
                txtPOMuaPL.Text = tblTxt.Rows[0]["POMua"].ToString();
                txtSoVanDonPL.Text = tblTxt.Rows[0]["SoVanDon"].ToString();
                txtQCKiemPL.Text = tblTxt.Rows[0]["QCKiemDisPlay"].ToString();
                if (txtQCKiemPL.Text == "PASS")
                {
                    txtQCKiemPL.ForeColor = Color.Green;
                }
                else if (txtQCKiemPL.Text == "FAIL")
                {
                    txtQCKiemPL.ForeColor = Color.Red;
                }
                else
                {
                    txtQCKiemPL.ForeColor = Color.Black;
                }
                txtNWPL.Text = tblTxt.Rows[0]["NW"].ToString();
                txtGWPL.Text = tblTxt.Rows[0]["GW"].ToString();
                dateNgayNKPL.EditValue = tblTxt.Rows[0]["NgayNKDuKien"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayNKDuKien"]) : null;
                searchLookUpEditKHCTPL.EditValue = tblTxt.Rows[0]["MaKH"].ToString();
                dateHoaDonPL.EditValue = tblTxt.Rows[0]["NgayHoaDon"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayHoaDon"]) : null;
                searchLookUpEditMHCTPL.EditValue = tblTxt.Rows[0]["MaHang"].ToString();
            }
        }
        private void DoiDV(object sender, EventArgs e)
        {
            DataRow dr = (_isNPL ? gVNLNhapKho : gVPLNhapKho).GetFocusedDataRow();
            if (dr == null) return;
            string _mdv = dr["MaDVCD"].ToString();
            string _dv = dr["TenDVCD"].ToString();
            loadDoiDV(_mdv, _dv);
        }
        private void loadDoiDV(string _mdv, string _dv)
        {
            frmERPNhapKhoNPL_DVCD frm = new frmERPNhapKhoNPL_DVCD(_mdv, _dv);
            //frm.WindowState = FormWindowState.Normal;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                List<string> _lst = frm.GetDVCD();
                DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                DataTable tbl = (_isNPL ? tblNhapKho : tblNhapKhoPL);
                GridView gV = (_isNPL ? gVNLNhapKho : gVPLNhapKho);
                for (int i = 0; i < gV.RowCount; i++)
                {

                    gV.SetRowCellValue(i, "MaDVCD", _lst[0]);
                    gV.SetRowCellValue(i, "TenDVCD", _lst[1]);
                }
                foreach (DataRow _dr in tbl.Rows)
                {

                    if (dr["MaNhom"].ToString() == _dr["MaNhom"].ToString() && dr["MaVTID"].ToString() == _dr["MaVTID"].ToString() && dr["MauVTID"].ToString() == _dr["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == _dr["KhoVaiID"].ToString()
                      )
                    {
                        _dr["MaDVCD"] = _lst[0];
                        _dr["TenDVCD"] = _lst[1];
                    }

                }
            }
        }
        private void nhapKhoiLuong(object sender, EventArgs e)
        {
            //frmERPNhapKhoNPL_KhoiLuong
            DataRow dr = (_isNPL ? gVNLNhapKho : gVPLNhapKho).GetFocusedDataRow();
            if (dr == null) return;
            string _mdv = dr["MaDVCD"].ToString();
            string _dv = dr["TenDVCD"].ToString();
            string _tileNW = dr["tileNW"].ToString();
            string _tileGW = dr["tileGW"].ToString();

            loadKhoiLuong(_mdv, _dv, _tileNW, _tileGW);
        }
        private void loadKhoiLuong(string _mdv, string _dv, string _tileNW, string _tileGW)
        {
            frmERPNhapKhoNPL_KhoiLuong frm = new frmERPNhapKhoNPL_KhoiLuong(_mdv, _dv, _tileNW, _tileGW);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                List<decimal> _lst = frm.GetKhoiLuong();
                DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                DataTable tbl = (_isNPL ? gCNLNhapKho.DataSource as DataTable : gCPLNhapKho.DataSource as DataTable);
                DataTable tblNK = (_isNPL ? tblNhapKho : tblNhapKhoPL);

                foreach (DataRow drA in tbl.AsEnumerable())
                {
                    decimal soGhiDauCay = decimal.TryParse(drA["SoGhiDauCay"]?.ToString(), out var val) ? val : 0m;
                    decimal _nw = _lst[0] * soGhiDauCay;
                    decimal _gw = _nw + _lst[1];
                    drA["tileNW"] = _lst[0];
                    drA["tileGW"] = _lst[1];
                    drA["NW"] = _nw;
                    drA["GW"] = _gw;
                }
                foreach (DataRow _dr in tblNK.Rows)
                {

                    if (dr["MaNhom"].ToString() == _dr["MaNhom"].ToString() && dr["MaVTID"].ToString() == _dr["MaVTID"].ToString() && dr["MauVTID"].ToString() == _dr["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == _dr["KhoVaiID"].ToString()
                      )
                    {
                        decimal soGhiDauCay = decimal.TryParse(_dr["SoGhiDauCay"].ToString(), out var val) ? val : 0m;
                        decimal _nw = _lst[0] * soGhiDauCay;
                        decimal _gw = _nw + _lst[1];
                        _dr["tileNW"] = _lst[0];
                        _dr["tileGW"] = _lst[1];
                        _dr["NW"] = _nw;
                        _dr["GW"] = _gw;
                    }
                }
            }
        }
        private bool CheckSLThucTeSLCT(DataTable tbl, GridView view)
        {
            try
            {
                if (tbl == null || tbl.Rows.Count == 0) return true;
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    DataRow dr = tbl.Rows[i];
                    if (dr.RowState == DataRowState.Deleted) continue;
                    decimal soGhiDauCay = 0;
                    decimal soLuongThucTe = 0;

                    decimal.TryParse(dr["SoGhiDauCay"].ToString(), out soGhiDauCay);
                    decimal.TryParse(dr["SoLuongThucTe"].ToString(), out soLuongThucTe);

                    if (soGhiDauCay != soLuongThucTe)
                    {
                        string soKien = dr["SoKienHienThi"]?.ToString();
                        string batch = dr.Table.Columns.Contains("Batch") ? dr["Batch"]?.ToString() : "";

                        string msg = $"Số lượng theo CT và số lượng thực tế không khớp!!!." +
                             $"\n- Số ghi đầu cây: {soGhiDauCay:N2}" +
                             $"\n- Số lượng thực tế: {soLuongThucTe:N2}" +
                             $"\nBạn có muốn tiếp tục không?";
                        DialogResult result = XtraMessageBox.Show(msg, "Xác nhận chênh lệch", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        { return true; }
                        else
                        {
                            int rowHandle = view.GetRowHandle(i);
                            if (view.IsValidRowHandle(rowHandle))
                            {
                                view.FocusedRowHandle = rowHandle;
                                view.FocusedColumn = view.Columns["SoLuongThucTe"];
                                view.ShowEditor();
                            }
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi kiểm tra số lượng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private string CheckTrungRollLotBatch(DataTable tblChiTiet)
        {
            //string searchLookUpLo = _isNPL ? searchLookUpEditLoNL.Text.ToString() : searchLookUpEditLoPL.Text.ToString();
            const string sep = "^|^";
            var rolllotbatchTracking = new Dictionary<string, List<int>>();
            var itemInfoForDuplicates = new Dictionary<string, string>();
            var duplicatesMsg = new List<string>();
            DataTable tblVatTu = _isNPL ? tblNL : tblPL;
            for (int i = 0; i < tblChiTiet.Rows.Count; i++)
            {
                DataRow row = tblChiTiet.Rows[i];
                int line = i + 1;
                string soloID = row["SoLoID"]?.ToString().Trim().ToUpper() ?? "";
                string solo = row["SoLo"]?.ToString().Trim().ToUpper() ?? "";
                string maVTID = row["MaVTID"]?.ToString().Trim().ToUpper() ?? "";
                string mauVTID = row["MauVTID"]?.ToString().Trim().ToUpper() ?? "";
                string khoVaiID = row["KhoVaiID"]?.ToString().Trim().ToUpper() ?? "";
                string maNhom = row["MaNhom"]?.ToString().Trim().ToUpper() ?? "";
                string maVTGhep = row["MaVTGhep"]?.ToString().Trim().ToUpper() ?? "";
                string roll = row["SoKienHienThi"].ToString().Trim().ToUpper() ?? "";
                string lot = row["SoLoT"].ToString().Trim().ToUpper() ?? "";
                string batch = row["Batch"].ToString().Trim().ToUpper() ?? "";

                if (string.IsNullOrEmpty(roll) && string.IsNullOrEmpty(lot) && string.IsNullOrEmpty(batch)) continue;
                string keyVatTu = $"{soloID}{sep}{maVTID}{sep}{mauVTID}{sep}{khoVaiID}{sep}{maNhom}{sep}{maVTGhep}{sep}{solo}";
                string fullKey = $"{keyVatTu}{sep}{roll}{sep}{lot}{sep}{batch}";
                if (!rolllotbatchTracking.ContainsKey(fullKey))
                    rolllotbatchTracking[fullKey] = new List<int>();
                rolllotbatchTracking[fullKey].Add(line);
                if (!itemInfoForDuplicates.ContainsKey(keyVatTu) && tblVatTu != null)
                {
                    var infoRow = tblVatTu.AsEnumerable().FirstOrDefault(r =>
                        (r.Field<object>("MaVTID") ?? "").ToString().Equals(maVTID, StringComparison.OrdinalIgnoreCase) &&
                        (r.Field<object>("MauVTID") ?? "").ToString().Equals(mauVTID, StringComparison.OrdinalIgnoreCase) &&
                        (r.Field<object>("KhoVaiID") ?? "").ToString().Equals(khoVaiID, StringComparison.OrdinalIgnoreCase) &&
                        (r.Field<object>("MaNhom") ?? "").ToString().Equals(maNhom, StringComparison.OrdinalIgnoreCase) &&
                        (r.Field<object>("MaVTGhep") ?? "").ToString().Equals(maVTGhep, StringComparison.OrdinalIgnoreCase)
                    );
                    string info = infoRow != null
                       ? $"{infoRow.Field<string>("TenNhom")}, ItemCode: {infoRow.Field<string>("MaVT")}, Chi tiết: {infoRow.Field<string>("ChiTiet")}, Màu: {infoRow.Field<string>("MauVT")}, Khổ: {infoRow.Field<string>("KhoVai")}"
                       : maVTGhep;
                    itemInfoForDuplicates.Add(keyVatTu, info);
                }
            }
            foreach (var kvp in rolllotbatchTracking)
            {
                if (kvp.Value.Count <= 1) continue;
                string[] parts = kvp.Key.Split(new[] { sep }, StringSplitOptions.None);
                string keyVatTu = $"{parts[1]}{sep}{parts[2]}{sep}{parts[3]}{sep}{parts[4]}{sep}{parts[5]}{sep}{parts[6]}";
                string actualRoll = string.IsNullOrEmpty(parts[7]) ? "(trống)" : parts[7];
                string actualLot = string.IsNullOrEmpty(parts[8]) ? "(trống)" : parts[8];
                string actualBatch = string.IsNullOrEmpty(parts[9]) ? "(trống)" : parts[9];
                string solo = string.IsNullOrEmpty(parts[6]) ? "(trống)" : parts[6];
                string infoVatTu = itemInfoForDuplicates.ContainsKey(keyVatTu)
                    ? itemInfoForDuplicates[keyVatTu]
                    : keyVatTu;

                duplicatesMsg.Add($"PI NCC: {solo}\n" +
                    $"Roll/Lot/Batch bị trùng: {actualRoll}/{actualLot}/{actualBatch}"
                );

            }
            if (duplicatesMsg.Count > 0)
            {
                return "Roll và Lot/Batch bị trùng lặp:\n" + string.Join("\n", duplicatesMsg);
            }
            return string.Empty;
        }
        private List<dynamic> LayDanhSachTrungTheoID(DataTable tblNhapKho, DataTable tblNL)
        {
            var duplicateGroups = tblNhapKho.AsEnumerable()
                .GroupBy(row => new
                {
                    SoLoID = row["SoLoID"],

                    MaVTID = row["MaVTID"],
                    MauVTID = row["MauVTID"],
                    KhoVaiID = row["KhoVaiID"],
                    SoKienHienThi = row["SoKienHienThi"]
                })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g) // Đưa về từng DataRow riêng lẻ
                .ToList();

            // 2. JOIN với tblNL để lấy thêm thông tin MaVT, ChiTiet
            var mergedList = (
                from rowNK in duplicateGroups
                join rowNL in tblNL.AsEnumerable()
                    on new
                    {
                        SoLoID = rowNK["SoLoID"],
                        MaVTID = rowNK["MaVTID"],

                        MauVTID = rowNK["MauVTID"],
                        KhoVaiID = rowNK["KhoVaiID"]
                    }
                    equals new
                    {
                        SoLoID = rowNL["SoLoID"],
                        MaVTID = rowNL["MaVTID"],

                        MauVTID = rowNL["MauVTID"],
                        KhoVaiID = rowNL["KhoVaiID"]
                    }
                select new
                {
                    SoLo = rowNL["SoLo"],
                    MaVTID = rowNK["MaVTID"],
                    MauVTID = rowNK["MauVTID"],
                    KhoVaiID = rowNK["KhoVaiID"],
                    SoKienHienThi = rowNK["SoKienHienThi"],

                    MaVT = rowNL["MaVT"],
                    ChiTiet = rowNL["ChiTiet"],
                    MauVT = rowNL["MauVT"],
                    KhoVai = rowNL["KhoVai"],
                    DataRowNHapKho = rowNK
                }
            ).ToList<dynamic>();

            return mergedList;
        }



        /*auto with và scroll*/

        private void BestFitCol(GridView view)
        {
            if (view == null || view.VisibleColumns.Count < 3) return;

            view.BeginUpdate();
            try
            {
                view.OptionsView.ColumnAutoWidth = false;
                view.BestFitColumns();

                for (int i = 0; i < 4; i++)
                {
                    var col = view.VisibleColumns[i];
                    if (col.MaxWidth != 100 || col.Fixed != DevExpress.XtraGrid.Columns.FixedStyle.Left)
                    {
                        col.MaxWidth = 100;
                        col.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
                    }
                }
            }
            finally
            {
                view.EndUpdate();
            }
        }
        #region tiền tệ
        private void repoSearchLookUpTienTe_Popup(object sender, EventArgs e)
        {
            var popupControl = sender as DevExpress.Utils.Win.IPopupControl;
            if (popupControl == null) return;
            var popupForm = popupControl.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
            if (popupForm == null) return;
            var searchLookUpPopup = popupForm.Controls.OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>().FirstOrDefault();
            if (searchLookUpPopup == null) return;
            var layout = searchLookUpPopup.Controls.OfType<LayoutControl>().FirstOrDefault();
            if (layout == null) return;
            AddCustomButtonToPopupLayout(layout, "btnKhaiBao", "Khai báo Tiền tệ", khaiBaoTTButton_Click);
        }
        private void repoSearchLookUpTienTePL_Popup(object sender, EventArgs e)
        {

            var popupControl = sender as DevExpress.Utils.Win.IPopupControl;
            if (popupControl == null) return;
            var popupForm = popupControl.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
            if (popupForm == null) return;
            var searchLookUpPopup = popupForm.Controls.OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>().FirstOrDefault();
            if (searchLookUpPopup == null) return;
            var layout = searchLookUpPopup.Controls.OfType<LayoutControl>().FirstOrDefault();
            if (layout == null) return;
            AddCustomButtonToPopupLayout(layout, "btnKhaiBao", "Khai báo Tiền tệ", khaiBaoTTButton_Click);
        }
        private void khaiBaoTTButton_Click(object sender, EventArgs e)
        {
            frmTienTe frm = new frmTienTe();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            CreateSearchLookUpTienTe();
        }
        #endregion
        private void AddCustomButtonToPopupLayout(LayoutControl layout, string buttonName, string buttonText, EventHandler clickHandler)
        {

            if (layout.Controls.OfType<Control>().Any(c => c.Name == buttonName))
                return;

            var buttonGroup = new LayoutControlGroup()
            {
                Name = "buttonGroup_" + buttonName,
                TextVisible = false,
                LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                GroupBordersVisible = false
            };

            // Cột layout: 2 cột (cột trống đẩy nút sang phải + cột nút)
            buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
            buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 });
            buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

            // Hàng layout: 3 hàng (empty space trên, nút, empty space dưới)
            buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
            buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });
            buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });
            buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });

            layout.BeginUpdate();
            layout.Root.AddItem(buttonGroup);

            // Empty space bên trái (đẩy nút sang phải)
            var emptySpaceItemLeft = new EmptySpaceItem()
            {
                AllowHotTrack = false
            };
            emptySpaceItemLeft.OptionsTableLayoutItem.ColumnIndex = 0;
            emptySpaceItemLeft.OptionsTableLayoutItem.RowIndex = 1;
            buttonGroup.AddItem(emptySpaceItemLeft);

            // Nút tùy chỉnh
            var button = new SimpleButton()
            {
                Name = buttonName,
                Text = buttonText
            };
            button.Click += clickHandler;

            var layoutItemButton = new LayoutControlItem()
            {
                Control = button,
                TextVisible = false,
                SizeConstraintsType = SizeConstraintsType.Custom,
                MinSize = new Size(140, 30),
                MaxSize = new Size(140, 30)
            };
            layoutItemButton.OptionsTableLayoutItem.ColumnIndex = 1;
            layoutItemButton.OptionsTableLayoutItem.RowIndex = 1;
            buttonGroup.AddItem(layoutItemButton);

            // Empty space trên
            var emptySpaceItemTop = new EmptySpaceItem()
            {
                AllowHotTrack = false
            };
            emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0;
            emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
            buttonGroup.AddItem(emptySpaceItemTop);

            // Empty space dưới
            var emptySpaceItemBottom = new EmptySpaceItem()
            {
                AllowHotTrack = false
            };
            emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 2;
            emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
            buttonGroup.AddItem(emptySpaceItemBottom);

            layout.EndUpdate();
        }
        private void gVPL_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gVPL.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {

                        DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xem giá tiền tệ", XemTienTe);
                        e.Menu.Items.Add(menuDeleteItem);



                    }

                }
            }
        }

        private void gVNL_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gVNL.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {

                        DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xem giá tiền tệ", XemTienTe);
                        e.Menu.Items.Add(menuDeleteItem);



                    }

                }
            }
        }
        private void XemTienTe(object sender, EventArgs e)
        {

            DataTable tbl = (_isNPL ? tblNL : tblPL);
            if (tbl == null || tbl.Rows.Count == 0) return;
            frmERPNhapKhoNPL_TienTe frm = new frmERPNhapKhoNPL_TienTe(tbl);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();


        }
        private void searchLookUpEditKHCTPL_EditValueChanged(object sender, EventArgs e)
        {
            loadMaHangCT();
        }
        private void searchLookUpEditKHCT_EditValueChanged(object sender, EventArgs e)
        {
            loadMaHangCT();
        }
        private void loadMaHangCT()
        {
            try
            {
                if (_isNPL)
                {
                    if (searchLookUpEditKHCT.EditValue == null || searchLookUpEditKHCT.EditValue.ToString() == "") return;
                    string makh = searchLookUpEditKHCT.EditValue.ToString();
                    string url = $"{URL}ERPNhapKhoNPL/Get?Action=GETMH&para={makh}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (json == "[]")
                    {
                        searchLookUpEditMHCT.Properties.DataSource = null;

                    }
                    else
                    {
                        DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                        searchLookUpEditMHCT.Properties.DataSource = tbl;
                    }


                }
                else
                {
                    if (searchLookUpEditKHCTPL.EditValue == null || searchLookUpEditKHCTPL.EditValue.ToString() == "") return;
                    string makh = searchLookUpEditKHCTPL.EditValue.ToString();
                    string url = $"{URL}ERPNhapKhoNPL/Get?Action=GETMH&para={makh}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (json == "[]")
                    {
                        searchLookUpEditMHCTPL.Properties.DataSource = null;

                    }
                    else
                    {
                        DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                        searchLookUpEditMHCTPL.Properties.DataSource = tbl;
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }

        #region E Invoice number

        #region Save

        private DataTable CreateTable_E_Invoice()
        {
            DataTable tbl = new DataTable("tbl_E_Invoice");
            tbl.Columns.Add("ID", typeof(long));
            tbl.Columns.Add("MaPhieuMH", typeof(string));
            tbl.Columns.Add("MaKhachHang", typeof(string));
            tbl.Columns.Add("SoHoaDon", typeof(string));
            tbl.Columns.Add("SoToKhai", typeof(string));
            tbl.Columns.Add("SoHopDong", typeof(string));
            tbl.Columns.Add("NgayHoaDon", typeof(string));
            tbl.Columns.Add("NgayMoToKhai", typeof(string));
            tbl.Columns.Add("NgayKyHD", typeof(string));
            tbl.Columns.Add("MaSoThue", typeof(string));
            tbl.Columns.Add("SoVanDon", typeof(string));
            tbl.Columns.Add("E_Way_Bill", typeof(string));
            tbl.Columns.Add("NoiGui", typeof(string));
            tbl.Columns.Add("NgayGui", typeof(string));
            tbl.Columns.Add("Cang", typeof(string));
            tbl.Columns.Add("Tau", typeof(string));
            tbl.Columns.Add("UserName", typeof(string));
            tbl.Columns.Add("CreaDate", typeof(string));
            tbl.Columns.Add("Mac", typeof(string));
            tbl.Columns.Add("IPAdress", typeof(string));
            tbl.Columns.Add("MachineName", typeof(string));

            return tbl;
        }
        public void Save_E_Invoice()
        {
            try
            {
                DataRow drVT = gVNL.GetFocusedDataRow();
                DataRow drVT_PL = gVPL.GetFocusedDataRow();
           
                DataTable tblSaveEInvoice = CreateTable_E_Invoice();

                DataRow rowEInvoice = tblSaveEInvoice.NewRow();

                if (xtraTabControl1.SelectedTabPage == tabNL)
                {
                    if (drVT == null) return;
                    rowEInvoice["ID"] = 0;
                    rowEInvoice["MaPhieuMH"] =  drVT["SoLoID"].ToString(); 
                    rowEInvoice["MaKhachHang"] = searchLookUpEditKHCT.EditValue;
                    rowEInvoice["SoHoaDon"] = txtSoHoaDon.EditValue;
                    rowEInvoice["SoToKhai"] = txtSoToKhai.EditValue;
                    rowEInvoice["SoHopDong"] = txtSoHoaDon.EditValue;
                    DateTime InvoiceDate = dateHoaDon.EditValue == null ? DateTime.MinValue : (DateTime)dateHoaDon.EditValue;
                    rowEInvoice["NgayHoaDon"] = InvoiceDate == DateTime.MinValue ? null : InvoiceDate.ToString("yyyy-MM-dd 00:00:00");
                    DateTime DeclarationDate = dateNgayToKhai.EditValue == null ? DateTime.MinValue : (DateTime)dateNgayToKhai.EditValue;
                    rowEInvoice["NgayMoToKhai"] = DeclarationDate == DateTime.MinValue ? null : DeclarationDate.ToString("yyyy-MM-dd 00:00:00");
                    DateTime ContractDate = dateSoHopDong.EditValue == null ? DateTime.MinValue : (DateTime)dateSoHopDong.EditValue;
                    rowEInvoice["NgayKyHD"] = ContractDate == DateTime.MinValue ? null : ContractDate.ToString("yyyy-MM-dd 00:00:00");
                    rowEInvoice["MaSoThue"] = txtMaSoThue.EditValue;
                    rowEInvoice["SoVanDon"] = txtSoVanDon.EditValue;
                    rowEInvoice["E_Way_Bill"] = txtEWayBill_NL.EditValue;
                    rowEInvoice["NoiGui"] = txtNoiGui.EditValue;
                    DateTime ShippingDate = dateNgayGui.EditValue == null ? DateTime.MinValue : (DateTime)dateNgayGui.EditValue;
                    rowEInvoice["NgayGui"] = ShippingDate == DateTime.MinValue ? null : ShippingDate.ToString("yyyy-MM-dd 00:00:00");
                    rowEInvoice["Cang"] = searchLookUpEditCang.EditValue;
                    rowEInvoice["Tau"] = SearchlookupTau.EditValue;
                    rowEInvoice["UserName"] = GlobleData.UserName;
                    rowEInvoice["CreaDate"] = DateTime.Now;
                    rowEInvoice["Mac"] = macAddress;
                    rowEInvoice["IPAdress"] = ipAddress;
                    rowEInvoice["MachineName"] = Environment.MachineName;
                }
                else if (xtraTabControl1.SelectedTabPage == tabPL)
                {
                    if (drVT_PL == null) return;
                    rowEInvoice["ID"] = 0;
                    rowEInvoice["MaPhieuMH"] = drVT_PL["SoLoID"].ToString(); ;
                    rowEInvoice["MaKhachHang"] = searchLookUpEditKHCTPL.EditValue;
                    rowEInvoice["SoHoaDon"] = txtSoHoaDonPL.EditValue;
                    rowEInvoice["SoToKhai"] = txtSoToKhaiPL.EditValue;
                    rowEInvoice["SoHopDong"] = txtSoHoaDonPL.EditValue;
                    DateTime InvoiceDate = dateHoaDonPL.EditValue == null ? DateTime.MinValue : (DateTime)dateHoaDonPL.EditValue;
                    rowEInvoice["NgayHoaDon"] = InvoiceDate == DateTime.MinValue ? null : InvoiceDate.ToString("yyyy-MM-dd 00:00:00");
                    DateTime DeclarationDate = dateNgayToKhaiPL.EditValue == null ? DateTime.MinValue : (DateTime)dateNgayToKhaiPL.EditValue;
                    rowEInvoice["NgayMoToKhai"] = DeclarationDate == DateTime.MinValue ? null : DeclarationDate.ToString("yyyy-MM-dd 00:00:00");
                    DateTime ContractDate = dateSoHopDongPL.EditValue == null ? DateTime.MinValue : (DateTime)dateSoHopDongPL.EditValue;
                    rowEInvoice["NgayKyHD"] = ContractDate == DateTime.MinValue ? null : ContractDate.ToString("yyyy-MM-dd 00:00:00");
                    rowEInvoice["MaSoThue"] = txtMaSoThuePL.EditValue;
                    rowEInvoice["SoVanDon"] = txtSoVanDonPL.EditValue;
                    rowEInvoice["E_Way_Bill"] = txtEWayBill_PL.EditValue;
                    rowEInvoice["NoiGui"] = txtNoiGuiPL.EditValue;
                    DateTime ShippingDate = dateNgayGuiPL.EditValue == null ? DateTime.MinValue : (DateTime)dateNgayGuiPL.EditValue;
                    rowEInvoice["NgayGui"] = ShippingDate == DateTime.MinValue ? null : ShippingDate.ToString("yyyy-MM-dd 00:00:00");
                    rowEInvoice["Cang"] = searchLookUpEditCangPL.EditValue;
                    rowEInvoice["Tau"] = SearchlookupTauPL.EditValue;
                    rowEInvoice["UserName"] = GlobleData.UserName;
                    rowEInvoice["CreaDate"] = DateTime.Now;
                    rowEInvoice["Mac"] = macAddress;
                    rowEInvoice["IPAdress"] = ipAddress;
                    rowEInvoice["MachineName"] = Environment.MachineName;
                }


                tblSaveEInvoice.Rows.Add(rowEInvoice);
                string url = string.Format("{0}?", URL + "NhaCC/PostEInvoice");
                string mss = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, tblSaveEInvoice);
                }).Result;


            }
            catch (Exception ex)
            {

            }


        }


        #endregion

        DataTable tblCang = new DataTable();
        DataTable tblTau = new DataTable();
        string macAddress = "";
        string ipAddress = "";
        //Environment.MachineName;

        /*Sự kiện chọn or nhập cảng*/
        private void ItemEditCang_Popup(object sender, EventArgs e)
        {
            try
            {
                var popupEdit = sender as SearchLookUpEdit;
                var view = popupEdit.Properties.View;

                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl)?.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
                var ownerEdit = popupForm?.OwnerEdit as DevExpress.XtraEditors.SearchLookUpEdit;

                var layout = popupForm?.Controls
                            .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                            .FirstOrDefault()?
                            .Controls
                            .OfType<LayoutControl>()
                            .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnAddCang"))
                {
                    layout.BeginUpdate();

                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroup",
                        TextVisible = false,
                        LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                        Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                        GroupBordersVisible = false
                    };

                    // 2 cột: TextEdit (%) + Button (AutoSize)
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 });
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                    // 4 hàng:
                    // Row 0 - empty top
                    // Row 1 - Tên cảng + Button Thêm
                    // Row 2 - Địa chỉ (span 2 cột)
                    // Row 3 - empty bottom
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 }); // Row 0: top empty
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });             // Row 1: tên cảng + button
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });             // Row 2: địa chỉ
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 }); // Row 3: bottom empty

                    layout.Root.AddItem(buttonGroup);

                    // ── ROW 1, COL 0: TextEdit Tên cảng ──
                    var txtCangNew = new TextEdit()
                    {
                        Name = "txtCangNew",
                        Properties =
                {
                    NullText = "Nhập tên cảng mới...",
                    NullValuePrompt = "Nhập tên cảng mới..."
                }
                    };
                    txtCangNew.Tag = ownerEdit;

                    var layoutItemTextEdit = new LayoutControlItem()
                    {
                        Control = txtCangNew,
                        Text = "Tên cảng:",
                        TextVisible = true,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(200, 30),
                        MaxSize = new Size(0, 30)
                    };
                    layoutItemTextEdit.OptionsTableLayoutItem.ColumnIndex = 0;
                    layoutItemTextEdit.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemTextEdit);

                    // ── ROW 1, COL 1: Button Thêm cảng ──
                    var btnAddCang = new SimpleButton()
                    {
                        Name = "btnAddCang",
                        Text = "Thêm cảng"
                    };
                    btnAddCang.Tag = txtCangNew;
                    btnAddCang.Click += btnAddCang_Click;

                    var layoutItemButton = new LayoutControlItem()
                    {
                        Control = btnAddCang,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(100, 30),
                        MaxSize = new Size(100, 30)
                    };
                    layoutItemButton.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemButton.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemButton);

                    // ── ROW 2, COL 0-1: TextEdit Địa chỉ (span 2 cột) ──
                    var txtDiaChi = new TextEdit()
                    {
                        Name = "txtDiaChi",
                        Properties =
                {
                    NullText = "Nhập địa chỉ...",
                    NullValuePrompt = "Nhập địa chỉ..."
                }
                    };

                    var layoutItemDiaChi = new LayoutControlItem()
                    {
                        Control = txtDiaChi,
                        Text = "Địa chỉ:",
                        TextVisible = true,
                        TextLocation = DevExpress.Utils.Locations.Left,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(200, 30),
                        MaxSize = new Size(0, 30)
                    };
                    layoutItemDiaChi.OptionsTableLayoutItem.ColumnIndex = 0;
                    layoutItemDiaChi.OptionsTableLayoutItem.RowIndex = 2;
                    layoutItemDiaChi.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(layoutItemDiaChi);

                    // ── ROW 0: Empty space top ──
                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemTop);

                    // ── ROW 3: Empty space bottom ──
                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 3;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemBottom);

                    // Enter trên txtCangNew → click button
                    txtCangNew.KeyDown += (s, ev) =>
                    {
                        if (ev.KeyCode == Keys.Enter)
                        {
                            btnAddCang.PerformClick();
                            ev.Handled = true;
                        }
                    };

                    layout.EndUpdate();
                }
                else
                {
                    var ControlTextEdit = layout?.GetControlByName("txtCangNew");
                    if (ControlTextEdit != null) ControlTextEdit.Text = "";

                    var ControlDiaChi = layout?.GetControlByName("txtDiaChi");
                    if (ControlDiaChi != null) ControlDiaChi.Text = "";
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddCang_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as SimpleButton;
                var txtCangNew = button?.Tag as TextEdit;
                var searchLookUpEdit = txtCangNew?.Tag as SearchLookUpEdit;

                if (txtCangNew != null && searchLookUpEdit != null)
                {
                    string tenCang = txtCangNew.Text.Trim();
                    string maCang = string.Empty;

                    if (string.IsNullOrWhiteSpace(tenCang))
                    {
                        XtraMessageBox.Show("Vui lòng nhập tên cảng!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCangNew.Focus();
                        return;
                    }

                    // Lấy txtDiaChi từ cùng parent với txtCangNew
                    var txtDiaChi = txtCangNew.Parent?.Controls
                        .OfType<TextEdit>()
                        .FirstOrDefault(c => c.Name == "txtDiaChi");

                    string diaChi = txtDiaChi?.Text.Trim() ?? string.Empty;

                    if (tblCang != null && tblCang.Rows.Count > 0)
                    {
                        var existingRow = tblCang.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("TenCang")) &&
                                r.Field<string>("TenCang").Trim().Equals(tenCang, StringComparison.OrdinalIgnoreCase));

                        if (existingRow != null)
                        {
                            maCang = existingRow.Field<string>("MaCang");
                        }
                        else
                        {
                            maCang = LuuCangMoi(tenCang, diaChi); // truyền thêm diaChi
                        }
                    }

                    if (!string.IsNullOrEmpty(maCang))
                    {
                        if (xtraTabControl1.SelectedTabPage == tabNL)
                        {
                            searchLookUpEditCang.Properties.DataSource = tblCang;
                            searchLookUpEditCang.EditValue = maCang;
                        }
                        else if (xtraTabControl1.SelectedTabPage == tabPL)
                        {
                            searchLookUpEditCangPL.Properties.DataSource = tblCang;
                            searchLookUpEditCangPL.EditValue = maCang;
                        }


                        searchLookUpEdit.ClosePopup();
                    }
                    else
                    {
                        XtraMessageBox.Show("Lỗi khi lưu cảng!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string LuuCangMoi(string tenCang, string diaChi)
        {
            try
            {

                string url = string.Format("{0}", URL + "NhaCC/PostCang");
                DataTable tblThuVien = new DataTable("tblThuVien");
                tblThuVien.Columns.Add("ID", typeof(int));
                tblThuVien.Columns.Add("MaCang", typeof(string));
                tblThuVien.Columns.Add("TenCang", typeof(string));
                tblThuVien.Columns.Add("DiaChi", typeof(string));
                tblThuVien.Columns.Add("GhiChu", typeof(string));

                DataRow row = tblThuVien.NewRow();
                row["ID"] = 0;
                row["TenCang"] = tenCang.Trim();
                row["MaCang"] = "";
                row["DiaChi"] = diaChi;
                row["GhiChu"] = "";
                tblThuVien.Rows.Add(row);

                string result = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, tblThuVien);
                }).Result;

                if (result?.ToLower() == "true")
                {

                    string urlCang = $"{URL}NhaCC/GePMH?action=GETCANG&para1=&para2=&para3=&para4=&para5=";
                    string jsonCang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCang); }).Result;
                    tblCang = JsonConvert.DeserializeObject<DataTable>(jsonCang);

                    if (tblCang != null && tblCang.Rows.Count > 0)
                    {
                        var newRow = tblCang.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("TenCang")) &&
                                r.Field<string>("TenCang").Trim().Equals(tenCang.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (newRow != null)
                        {
                            return newRow.Field<string>("MaCang");
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi lưu cảng: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /*Sự kiện chọn or nhập tàu */

        private void ItemEdit_Tau_Popup(object sender, EventArgs e)
        {
            try
            {
                var popupEdit = sender as SearchLookUpEdit;
                var view = popupEdit.Properties.View;

                var popupForm = (sender as DevExpress.Utils.Win.IPopupControl)?.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
                var ownerEdit = popupForm?.OwnerEdit as DevExpress.XtraEditors.SearchLookUpEdit;

                var layout = popupForm?.Controls
                            .OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>()
                            .FirstOrDefault()?
                            .Controls
                            .OfType<LayoutControl>()
                            .FirstOrDefault();

                if (layout != null && layout.Controls.OfType<Control>().All(c => c.Name != "btnAddTau"))
                {
                    layout.BeginUpdate();

                    var buttonGroup = new LayoutControlGroup()
                    {
                        Name = "buttonGroup",
                        TextVisible = false,
                        LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table,
                        Padding = new DevExpress.XtraLayout.Utils.Padding(0),
                        Spacing = new DevExpress.XtraLayout.Utils.Padding(0),
                        GroupBordersVisible = false
                    };

                    // 2 cột: TextEdit (%) + Button (AutoSize)
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.Percent, Width = 100 });
                    buttonGroup.OptionsTableLayoutGroup.ColumnDefinitions.Add(new ColumnDefinition() { SizeType = SizeType.AutoSize });

                    // 5 hàng:
                    // Row 0 - empty top
                    // Row 1 - Tên tàu + Button Thêm
                    // Row 2 - Ghi chú (span 2 cột)
                    // Row 3 - empty bottom
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Clear();
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 });  // Row 0: top empty
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });              // Row 1: tên tàu + button
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.AutoSize });              // Row 2: ghi chú
                    buttonGroup.OptionsTableLayoutGroup.RowDefinitions.Add(new RowDefinition() { SizeType = SizeType.Percent, Height = 50 }); // Row 3: bottom empty

                    layout.Root.AddItem(buttonGroup);

                    // ── ROW 1, COL 0: TextEdit Tên tàu ──
                    var txtTauNew = new TextEdit()
                    {
                        Name = "txtTauNew",
                        Properties =
                {
                    NullText = "Nhập tên tàu mới...",
                    NullValuePrompt = "Nhập tên tàu mới..."
                }
                    };
                    txtTauNew.Tag = ownerEdit;

                    var layoutItemTextEdit = new LayoutControlItem()
                    {
                        Control = txtTauNew,
                        Text = "Tên tàu:",
                        TextVisible = true,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(200, 30),
                        MaxSize = new Size(0, 30)
                    };
                    layoutItemTextEdit.OptionsTableLayoutItem.ColumnIndex = 0;
                    layoutItemTextEdit.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemTextEdit);

                    // ── ROW 1, COL 1: Button Thêm tàu ──
                    var btnAddTau = new SimpleButton()
                    {
                        Name = "btnAddTau",
                        Text = "Thêm tàu"
                    };
                    btnAddTau.Tag = txtTauNew;
                    btnAddTau.Click += btnAddTau_Click;

                    var layoutItemButton = new LayoutControlItem()
                    {
                        Control = btnAddTau,
                        TextVisible = false,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(100, 30),
                        MaxSize = new Size(100, 30)
                    };
                    layoutItemButton.OptionsTableLayoutItem.ColumnIndex = 1;
                    layoutItemButton.OptionsTableLayoutItem.RowIndex = 1;
                    buttonGroup.AddItem(layoutItemButton);


                    var txtDiaChi = new TextEdit()
                    {
                        Name = "txtDiaChi",
                        Properties =
                {
                    NullText = "Nhập địa chỉ...",
                    NullValuePrompt = "Nhập  địa chỉ..."
                }
                    };

                    var layoutItemGhiChu = new LayoutControlItem()
                    {
                        Control = txtDiaChi,
                        Text = "Địa chỉ:",
                        TextVisible = true,
                        TextLocation = DevExpress.Utils.Locations.Left,
                        SizeConstraintsType = SizeConstraintsType.Custom,
                        MinSize = new Size(200, 30),
                        MaxSize = new Size(0, 30)
                    };
                    layoutItemGhiChu.OptionsTableLayoutItem.ColumnIndex = 0;
                    layoutItemGhiChu.OptionsTableLayoutItem.RowIndex = 2;
                    layoutItemGhiChu.OptionsTableLayoutItem.ColumnSpan = 2; // Span cả 2 cột
                    buttonGroup.AddItem(layoutItemGhiChu);


                    var emptySpaceItemTop = new EmptySpaceItem();
                    emptySpaceItemTop.AllowHotTrack = false;
                    emptySpaceItemTop.OptionsTableLayoutItem.RowIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemTop.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemTop);


                    var emptySpaceItemBottom = new EmptySpaceItem();
                    emptySpaceItemBottom.AllowHotTrack = false;
                    emptySpaceItemBottom.OptionsTableLayoutItem.RowIndex = 3;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnIndex = 0;
                    emptySpaceItemBottom.OptionsTableLayoutItem.ColumnSpan = 2;
                    buttonGroup.AddItem(emptySpaceItemBottom);


                    txtTauNew.KeyDown += (s, ev) =>
                    {
                        if (ev.KeyCode == Keys.Enter)
                        {
                            btnAddTau.PerformClick();
                            ev.Handled = true;
                        }
                    };

                    layout.EndUpdate();
                }
                else
                {
                    var ControlTextEdit = layout?.GetControlByName("txtTauNew");
                    if (ControlTextEdit != null) ControlTextEdit.Text = "";

                    var ControlGhiChu = layout?.GetControlByName("txtDiaChi");
                    if (ControlGhiChu != null) ControlGhiChu.Text = "";
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddTau_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as SimpleButton;
                var txtTauNew = button?.Tag as TextEdit;
                var searchLookUpEdit = txtTauNew?.Tag as SearchLookUpEdit;

                if (txtTauNew != null && searchLookUpEdit != null)
                {
                    string tenTau = txtTauNew.Text.Trim();
                    string maTau = string.Empty;

                    if (string.IsNullOrWhiteSpace(tenTau))
                    {
                        XtraMessageBox.Show("Vui lòng nhập tên tàu!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTauNew.Focus();
                        return;
                    }


                    var txtDiaChi = txtTauNew.Parent?.Controls
                        .OfType<TextEdit>()
                        .FirstOrDefault(c => c.Name == "txtDiaChi");

                    string DiaChi = txtDiaChi?.Text.Trim() ?? string.Empty;

                    if (tblTau != null && tblTau.Rows.Count > 0)
                    {
                        var existingRow = tblTau.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("TenTau")) &&
                                r.Field<string>("TenTau").Trim().Equals(tenTau, StringComparison.OrdinalIgnoreCase));

                        if (existingRow != null)
                        {
                            maTau = existingRow.Field<string>("MaTau");
                        }
                        else
                        {
                            maTau = LuuTauMoi(tenTau, DiaChi);
                        }
                    }

                    if (!string.IsNullOrEmpty(maTau))
                    {
                        if (xtraTabControl1.SelectedTabPage == tabNL)
                        {
                            SearchlookupTau.Properties.DataSource = tblTau;
                            SearchlookupTau.EditValue = maTau;
                        }
                        else if (xtraTabControl1.SelectedTabPage == tabPL)
                        {
                            SearchlookupTauPL.Properties.DataSource = tblTau;
                            SearchlookupTauPL.EditValue = maTau;
                        }

                        searchLookUpEdit.ClosePopup();
                    }
                    else
                    {
                        XtraMessageBox.Show("Lỗi khi lưu tàu!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private string LuuTauMoi(string Tentau, string DiaChi)
        {
            try
            {


                // Bước 2: Gọi API lưu thuế mới
                string url = string.Format("{0}", URL + "NhaCC/PostTau");
                DataTable tblThuVien = new DataTable();
                tblThuVien = new DataTable("tblThuVien");
                tblThuVien.Columns.Add("ID", typeof(int));
                tblThuVien.Columns.Add("MaTau", typeof(string));
                tblThuVien.Columns.Add("TenTau", typeof(string));
                tblThuVien.Columns.Add("DiaChi", typeof(string));
                tblThuVien.Columns.Add("GhiChu", typeof(string));

                DataRow row = tblThuVien.NewRow();
                row["ID"] = 0;
                row["TenTau"] = Tentau.Trim();
                row["MaTau"] = "";
                row["DiaChi"] = DiaChi;
                row["GhiChu"] = "";
                tblThuVien.Rows.Add(row);
                string result = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url, tblThuVien);
                }).Result;


                if (result?.ToLower() == "true")
                {

                    string urlTau = $"{URL}NhaCC/GePMH?action=GETTAU&para1=&para2=&para3=&para4=&para5=";
                    string jsonTau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTau); }).Result;
                    tblTau = JsonConvert.DeserializeObject<DataTable>(jsonTau);

                    if (tblTau != null && tblTau.Rows.Count > 0)
                    {
                        var newRow = tblTau.AsEnumerable()
                            .FirstOrDefault(r =>
                                !string.IsNullOrEmpty(r.Field<string>("TenTau")) &&
                                r.Field<string>("TenTau").Trim().Equals(Tentau.Trim(), StringComparison.OrdinalIgnoreCase));

                        if (newRow != null)
                        {
                            return newRow.Field<string>("MaTau");
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        private void LoadIPAdress()
        {
            macAddress = "";
            ipAddress = "";

            // Lấy MAC + IP từ card mạng đang hoạt động (không loopback, không ảo)
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                             ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                             ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                .OrderByDescending(ni => ni.Speed)
                .FirstOrDefault();

            if (networkInterface != null)
            {

                PhysicalAddress pa = networkInterface.GetPhysicalAddress();
                macAddress = string.Join("-", pa.GetAddressBytes().Select(b => b.ToString("X2")));


                var ipProps = networkInterface.GetIPProperties();
                var ipv4 = ipProps.UnicastAddresses
                    .FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork);

                if (ipv4 != null)
                {
                    ipAddress = ipv4.Address.ToString();
                }
            }
        }

        #endregion
    }
}