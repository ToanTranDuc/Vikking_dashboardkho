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
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPNhapKhoNPLSua : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private DataTable tblNL, tblPL, tblMau, tblNhapKho, tblPhieu, tblSearchNL, tblSearchPL, tblSearchTienTe;
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
        private HashSet<string> _originalKeysFromDB_NPL = new HashSet<string>();
        private HashSet<string> _originalKeysFromDB_PL = new HashSet<string>();
        DataRow _drPO = null;
        public frmERPNhapKhoNPLSua(string lo = "", bool isNPL = true)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _lo = lo;
            _isTabNPL = isNPL;
            //loadVatTu(true);
            //loadVatTu(false);
        }

        public frmERPNhapKhoNPLSua(DataRow dr, string lo = "", bool isNPL = true)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _lo = lo;
            _isTabNPL = isNPL;
            this._drPO = dr;
            //loadVatTu(true);
            //loadVatTu(false);
        }
        protected override void OnLoad(EventArgs e)
        {
            CheckPerminsion();
            Init();

            createSearchLookUpLo();
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

            createSearchLookUpPOMua(true);
            createSearchLookUpPOMua(false);

            if (_drPO != null)
            {
                searchLookUpEditLoNL.EditValue = _drPO["SoLoID"].ToString().Split('@')[0];
            }
            else if (!string.IsNullOrWhiteSpace(_lo))
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

                    searchLookUpEditNL.Properties.ReadOnly = true;
                }
                else
                {

                    searchLookUpEditNL.Properties.ReadOnly = false;
                }
                if (!_allowDelete)
                {
                    btnXoaLo.Enabled = false;
                }
                else
                {
                    btnXoaLo.Enabled = true;
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

                if (!_allowDeletePL)
                {
                    btnXoaLo.Enabled = false;


                }
                else
                {

                    btnXoaLo.Enabled = true;
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
            searchLookUpEditNL.Properties.ValueMember = "MaVTGhep";
            searchLookUpEditNL.Properties.DisplayMember = "MaVTGhep";

            searchLookUpEditLoNL.Properties.ValueMember = "SoLoID";
            searchLookUpEditLoNL.Properties.DisplayMember = "DisPlay";
            searchLookUpEditLoPL.Properties.ValueMember = "SoLoID";
            searchLookUpEditLoPL.Properties.DisplayMember = "DisPlay";

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


            SearchlookupTau.Properties.ValueMember = "MaTau";
            SearchlookupTau.Properties.DisplayMember = "TenTau";
            SearchlookupTauPL.Properties.ValueMember = "MaTau";
            SearchlookupTauPL.Properties.DisplayMember = "TenTau";


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


            txtPOMua.Properties.ValueMember = "POMua";
            txtPOMua.Properties.DisplayMember = "POMua";
            txtPOMuaPL.Properties.ValueMember = "POMua";
            txtPOMuaPL.Properties.DisplayMember = "POMua";
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
                    tblSearchTienTe = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                    repoSearchLookUpTienTe.DataSource = tblSearchTienTe;
                    repoSearchLookUpTienTePL.DataSource = tblSearchTienTe;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void createSearchLookUpLo(bool isSolo = false)
        {

            //string pomua = string.Empty;
            //if(_isNPL)
            //{
            //    if (txtPOMua.EditValue != null && txtPOMua.EditValue.ToString() != "")
            //        pomua = txtPOMua.EditValue.ToString();
            //}
            //else
            //{
            //    if (txtPOMuaPL.EditValue != null && txtPOMuaPL.EditValue.ToString() != "")
            //        pomua = txtPOMuaPL.EditValue.ToString();
            //}
            //string url = $"{URL}ERPNhapKhoNPL/Get?Action=GETLONL&para={pomua}";
            //if (!_isNPL)
            //    url = $"{URL}ERPNhapKhoNPL/Get?Action=GETLOPL&para={pomua}";
            gCNL.DataSource = null;
            gCPL.DataSource = null;
            gCNLNhapKho.DataSource = null;
            gCPLNhapKho.DataSource = null;
            string poValue = _isNPL ? Convert.ToString(txtPOMua.EditValue) : Convert.ToString(txtPOMuaPL.EditValue);
            string pomua = string.IsNullOrWhiteSpace(poValue) ? string.Empty : poValue;

            string action = _isNPL ? "GETLONL" : "GETLOPL";
            string url = $"{URL}ERPNhapKhoNPL/Get?Action={action}&para={pomua}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                searchLookUpEditLoNL.Properties.DataSource = null;
                searchLookUpEditLoPL.Properties.DataSource = null;
                return;
            }


            DataTable tblMH = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditLoNL.Properties.DataSource = tblMH;
            searchLookUpEditLoPL.Properties.DataSource = tblMH;
            if (tblMH != null && tblMH.Rows.Count != 0 && isSolo)
            {
                if (_isNPL)
                {
                    DataRow targetRow = tblMH.AsEnumerable()
                    .FirstOrDefault(r => r["POMua"].ToString() == pomua);
                    if (targetRow != null)
                        searchLookUpEditLoNL.EditValue = targetRow["SoLoID"];
                }
                else
                {
                    DataRow targetRow = tblMH.AsEnumerable()
                   .FirstOrDefault(r => r["POMua"].ToString() == pomua);
                    if (targetRow != null)
                        searchLookUpEditLoPL.EditValue = targetRow["SoLoID"];

                }
            }


        }
        private void createSearchLookUpPOMua(bool isNL)
        {

            string url = $"{URL}ERPNhapKhoNPL/Get?Action=GETPOMUA&para={isNL}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                txtPOMua.Properties.DataSource = null;
                txtPOMuaPL.Properties.DataSource = null;
                return;
            }


            DataTable tblMH = JsonConvert.DeserializeObject<DataTable>(json);
            if (isNL)
            {
                txtPOMua.Properties.DataSource = tblMH;
            }
            else
            {
                txtPOMuaPL.Properties.DataSource = tblMH;
            }



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
            tbl.Columns.Add("TienTe", typeof(string));
            tbl.Columns.Add("MaVTGhep", typeof(string));
            tbl.Columns.Add("QuyDoiID", typeof(string));
            tbl.Columns.Add("POMua", typeof(string));
            tbl.Columns.Add("Batch", typeof(string));
            tbl.Columns.Add("SLTong", typeof(decimal));
            tbl.Columns.Add("STTChonVT", typeof(int));
            tbl.Columns.Add("TuoiTonKho", typeof(decimal));
            tbl.Columns.Add("IsIn", typeof(bool));
            tbl.Columns.Add("IsThemNhanh", typeof(bool));
            return tbl;

        }
        private void CreateTableChiTiet()
        {
            tblChiTiet = new DataTable("tblChiTiet");

            tblChiTiet.Columns.Add("ID", typeof(int));
            tblChiTiet.Columns.Add("SoLoID", typeof(string));
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
            tblChiTiet.Columns.Add("STT", typeof(int));
            tblChiTiet.Columns.Add("NgayNKDuKien", typeof(DateTime));
            tblChiTiet.Columns.Add("MaDVCD", typeof(string));
            tblChiTiet.Columns.Add("TenDVCD", typeof(string));
            tblChiTiet.Columns.Add("tileNW", typeof(decimal));
            tblChiTiet.Columns.Add("tileGW", typeof(decimal));
            tblChiTiet.Columns.Add("TienTe", typeof(string));
            tblChiTiet.Columns.Add("QuyDoiID", typeof(string));
            tblChiTiet.Columns.Add("POMua", typeof(string));
            tblChiTiet.Columns.Add("Batch", typeof(string));
            tblChiTiet.Columns.Add("SLTong", typeof(decimal));
            tblChiTiet.Columns.Add("TuoiTonKho", typeof(decimal));
            tblChiTiet.Columns.Add("Dot", typeof(int));
        }
        private void CreateTableChiTietPL()
        {
            tblChiTietPL = new DataTable("tblChiTietPL");

            tblChiTietPL.Columns.Add("ID", typeof(int));
            tblChiTietPL.Columns.Add("SoLoID", typeof(string));
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
            tblChiTietPL.Columns.Add("STT", typeof(int));
            tblChiTietPL.Columns.Add("NgayNKDuKien", typeof(DateTime));
            tblChiTietPL.Columns.Add("MaDVCD", typeof(string));
            tblChiTietPL.Columns.Add("TenDVCD", typeof(string));
            tblChiTietPL.Columns.Add("tileNW", typeof(decimal));
            tblChiTietPL.Columns.Add("tileGW", typeof(decimal));
            tblChiTietPL.Columns.Add("TienTe", typeof(string));
            tblChiTietPL.Columns.Add("QuyDoiID", typeof(string));
            tblChiTietPL.Columns.Add("POMua", typeof(string));
            tblChiTietPL.Columns.Add("Batch", typeof(string));
            tblChiTietPL.Columns.Add("SLTong", typeof(decimal));
            tblChiTietPL.Columns.Add("TuoiTonKho", typeof(decimal));
            tblChiTietPL.Columns.Add("Dot", typeof(int));
        }
        private void CreateTableNhapKho()
        {
            tblNhapKho = new DataTable("tblNhapKho");
            tblNhapKho.Columns.Add("ID", typeof(int));
            tblNhapKho.Columns.Add("SoLoID", typeof(string));
            tblNhapKho.Columns.Add("MaNPL", typeof(string));
            tblNhapKho.Columns.Add("MaVTID", typeof(string));
            tblNhapKho.Columns.Add("MaMauVT", typeof(string));
            tblNhapKho.Columns.Add("MauVT", typeof(string));
            tblNhapKho.Columns.Add("SoKien", typeof(string));
            tblNhapKho.Columns.Add("SoLot", typeof(string));
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
            tblNhapKho.Columns.Add("SoKienHienThi", typeof(string));
            tblNhapKho.Columns.Add("STT", typeof(int));
            tblNhapKho.Columns.Add("NgayNKDuKien", typeof(DateTime));
            tblNhapKho.Columns.Add("MaDVCD", typeof(string));
            tblNhapKho.Columns.Add("TenDVCD", typeof(string));
            tblNhapKho.Columns.Add("tileNW", typeof(decimal));
            tblNhapKho.Columns.Add("tileGW", typeof(decimal));
            tblNhapKho.Columns.Add("TienTe", typeof(string));

            if (!tblNhapKho.Columns.Contains("MaVTGhep"))
            {
                tblNhapKho.Columns.Add("MaVTGhep", typeof(string));
            }
            if (!tblNhapKho.Columns.Contains("MaNhom"))
            {
                tblNhapKho.Columns.Add("MaNhom", typeof(string));
            }
            tblNhapKho.Columns.Add("QuyDoiID", typeof(string));
            tblNhapKho.Columns.Add("POMua", typeof(string));
            tblNhapKho.Columns.Add("Batch", typeof(string));
            tblNhapKho.Columns.Add("SLTong", typeof(decimal));
            tblNhapKho.Columns.Add("STTChonVT", typeof(int));
            tblNhapKho.Columns.Add("IsKiemKe", typeof(bool));
            tblNhapKho.Columns.Add("NgayKiemKe", typeof(DateTime));
            tblNhapKho.Columns.Add("TuoiTonKho", typeof(decimal));
            tblNhapKho.Columns.Add("IsIn", typeof(bool));
            tblNhapKho.Columns.Add("NgayNhapKho", typeof(DateTime));
            tblNhapKho.Columns.Add("Dot", typeof(int));

        }
        private void CreateTableNhapKhoPL()
        {
            tblNhapKhoPL = new DataTable("tblNhapKhoPL");
            tblNhapKhoPL.Columns.Add("ID", typeof(int));
            tblNhapKhoPL.Columns.Add("SoLoID", typeof(string));
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
            tblNhapKhoPL.Columns.Add("STT", typeof(int));
            tblNhapKhoPL.Columns.Add("NgayNKDuKien", typeof(DateTime));
            tblNhapKhoPL.Columns.Add("MaDVCD", typeof(string));
            tblNhapKhoPL.Columns.Add("TenDVCD", typeof(string));
            tblNhapKhoPL.Columns.Add("tileNW", typeof(decimal));
            tblNhapKhoPL.Columns.Add("tileGW", typeof(decimal));
            tblNhapKhoPL.Columns.Add("TienTe", typeof(string));

            if (!tblNhapKhoPL.Columns.Contains("MaVTGhep"))
            {
                tblNhapKhoPL.Columns.Add("MaVTGhep", typeof(string));
            }
            if (!tblNhapKhoPL.Columns.Contains("MaNhom"))
            {
                tblNhapKhoPL.Columns.Add("MaNhom", typeof(string));
            }
            tblNhapKhoPL.Columns.Add("QuyDoiID", typeof(string));
            tblNhapKhoPL.Columns.Add("POMua", typeof(string));
            tblNhapKhoPL.Columns.Add("Batch", typeof(string));
            tblNhapKhoPL.Columns.Add("SLTong", typeof(decimal));
            tblNhapKhoPL.Columns.Add("STTChonVT", typeof(int));
            tblNhapKhoPL.Columns.Add("IsKiemKe", typeof(bool));
            tblNhapKhoPL.Columns.Add("NgayKiemKe", typeof(DateTime));
            tblNhapKhoPL.Columns.Add("TuoiTonKho", typeof(decimal));
            tblNhapKhoPL.Columns.Add("IsIn", typeof(bool));
            tblNhapKhoPL.Columns.Add("NgayNhapKho", typeof(DateTime));
            tblNhapKhoPL.Columns.Add("Dot", typeof(int));

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
                SearchlookupTau.EditValue = tblTxt.Rows[0]["MaTau"].ToString();
                dateNgayGui.EditValue = tblTxt.Rows[0]["NgayGui"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayGui"]) : null;
                txtNoiGui.Text = tblTxt.Rows[0]["NoiGui"].ToString();
                txtPOMua.EditValueChanged -= txtPOMua_EditValueChanged;
                txtPOMua.Text = tblTxt.Rows[0]["POMua"].ToString();
                txtPOMua.EditValueChanged += txtPOMua_EditValueChanged;
                txtSoVanDon.Text = tblTxt.Rows[0]["SoVanDon"].ToString();
                txtQCKiem.Text = tblTxt.Rows[0]["QCKiemDisPlay"].ToString();
                txtEWayBill_NL.EditValue = tblTxt.Rows[0]["E_Way_Bill"]?.ToString();
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

                cbKiemKe.Checked = tblTxt.Rows[0]["IsKiemKe"] != DBNull.Value ? Convert.ToBoolean(tblTxt.Rows[0]["IsKiemKe"]) : false;
                ngayKK.EditValue = tblTxt.Rows[0]["NgayKiemKe"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayKiemKe"]) : null;
                dateNgayNK.EditValue = tblTxt.Rows[0]["NgayNKDuKien"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayNKDuKien"]) : null;
                searchLookUpEditKHCT.EditValue = tblTxt.Rows[0]["MaKH"].ToString();
                dateHoaDon.EditValue = tblTxt.Rows[0]["NgayHoaDon"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayHoaDon"]) : null;
                searchLookUpEditMHCT.EditValue = tblTxt.Rows[0]["MaHang"].ToString();
                btnPDF.Text = tblTxt.Rows[0]["PDF"].ToString();
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
                txtPOMuaPL.EditValueChanged -= txtPOMuaPL_EditValueChanged;
                txtPOMuaPL.Text = tblTxt.Rows[0]["POMua"].ToString();
                txtPOMuaPL.EditValueChanged += txtPOMuaPL_EditValueChanged;
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
                cbKiemKePL.Checked = tblTxt.Rows[0]["IsKiemKe"] != DBNull.Value ? Convert.ToBoolean(tblTxt.Rows[0]["IsKiemKe"]) : false;
                ngayKKPL.EditValue = tblTxt.Rows[0]["NgayKiemKe"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayKiemKe"]) : null;
                dateNgayNKPL.EditValue = tblTxt.Rows[0]["NgayNKDuKien"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayNKDuKien"]) : null;
                searchLookUpEditKHCTPL.EditValue = tblTxt.Rows[0]["MaKH"].ToString();
                dateHoaDonPL.EditValue = tblTxt.Rows[0]["NgayHoaDon"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayHoaDon"]) : null;
                searchLookUpEditMHCTPL.EditValue = tblTxt.Rows[0]["MaHang"].ToString();
                btnPDFPL.Text = tblTxt.Rows[0]["PDF"].ToString();
                txtEWayBill_PL.EditValue = tblTxt.Rows[0]["E_Way_Bill"]?.ToString();
            }
        }


        private void loadGVVatTu(bool isNL)
        {
            try
            {
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang tải dữ liệu");
                if ((isNL ? searchLookUpEditLoNL : searchLookUpEditLoPL).EditValue == null)
                {
                    SplashScreenManager.CloseForm(false);
                    return;
                }

                string url = $"{URL}ERPNhapKhoNPL/Get?Action={(isNL ? "GETCTNKNL" : "GETCTNKPL")}&para={(isNL ? searchLookUpEditLoNL : searchLookUpEditLoPL).EditValue?.ToString()}";

                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    SplashScreenManager.CloseForm(false);
                    return;
                }

                tblChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblChiTiet == null || tblChiTiet.Rows.Count == 0)
                {
                    SplashScreenManager.CloseForm(false);
                    return;
                }
                //
                if (isNL)
                    _originalKeysFromDB_NPL.Clear();
                else
                    _originalKeysFromDB_PL.Clear();

                foreach (DataRow dr in tblChiTiet.Rows)
                {
                    string key = $"{dr["MaVTID"]}_{dr["MauVTID"]}_{dr["MaNhom"]}_{dr["KhoVaiID"]}";
                    if (isNL)
                        _originalKeysFromDB_NPL.Add(key);
                    else
                        _originalKeysFromDB_PL.Add(key);
                }
                //
                if (isNL) CreateTableNhapKho();
                else CreateTableNhapKhoPL();

                if ((isNL ? tblNL : tblPL) == null || (isNL ? tblNL : tblPL).Rows.Count == 0)
                {
                    if (isNL) tblNL = CreateTableVatTu();
                    else tblPL = CreateTableVatTu();
                }
                (isNL ? tblNL : tblPL).Clear();
                (isNL ? tblNhapKho : tblNhapKhoPL).Clear();
                var groupedData = tblChiTiet.AsEnumerable()
                    .GroupBy(dr => new
                    {
                        MaVTID = dr["MaVTID"],
                        MaNhom = dr["MaNhom"],
                        MauVTID = dr["MauVTID"],
                        KhoVaiID = dr["KhoVaiID"]
                    })
                    .ToList();
                int sttTrongNhom = 1;
                foreach (var group in groupedData)
                {

                    // Thêm dòng vật tư (tblNL/tblPL) nếu chưa có
                    DataRow firstRowInGroup = group.First();
                    object maVTID = firstRowInGroup["MaVTID"];
                    object maNhom = firstRowInGroup["MaNhom"];
                    object maMauVT = firstRowInGroup["MauVTID"];
                    object khoVaiID = firstRowInGroup["KhoVaiID"];
                    object MaVTGhep = firstRowInGroup["MaVTGhep"];
                    bool isDuplicate = (isNL ? tblNL : tblPL).AsEnumerable().Any(r =>
                        (r.Field<object>("MaVTID") ?? DBNull.Value).Equals(maVTID ?? DBNull.Value) &&
                        (r.Field<object>("MaNhom") ?? DBNull.Value).Equals(maNhom ?? DBNull.Value) &&
                        (r.Field<object>("MauVTID") ?? DBNull.Value).Equals(maMauVT ?? DBNull.Value) &&
                        (r.Field<object>("KhoVaiID") ?? DBNull.Value).Equals(khoVaiID ?? DBNull.Value) &&
                        (r.Field<object>("MaVTGhep") ?? DBNull.Value).Equals(MaVTGhep ?? DBNull.Value)
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
                        row1["MaKeToan"] = firstRowInGroup["MaKeToan"];
                        row1["DonGia"] = firstRowInGroup["DonGia"];
                        row1["MaDVVT"] = firstRowInGroup["MaDVVT"];
                        row1["TenDVVT"] = firstRowInGroup["TenDVVT"];
                        row1["MauVTID"] = firstRowInGroup["MauVTID"];
                        row1["TienTe"] = firstRowInGroup["TienTe"];
                        row1["QuyDoiID"] = firstRowInGroup["QuyDoiID"];
                        row1["IsIn"] = false;
                        row1["IsThemNhanh"] = false;
                        if (!_lstMaHQ.Contains(firstRowInGroup["MaHaiQuan"].ToString()))
                        {
                            _lstMaHQ.Add(firstRowInGroup["MaHaiQuan"].ToString());
                        }
                        if (!_lstMaKT.Contains(firstRowInGroup["MaKeToan"].ToString()))
                        {
                            _lstMaKT.Add(firstRowInGroup["MaKeToan"].ToString());
                        }
                        row1["MaVTGhep"] = firstRowInGroup["MaVTGhep"];
                        row1["SLTong"] = firstRowInGroup["SLTong"];
                        row1["STTChonVT"] = firstRowInGroup["STTChonVT"];
                        row1["TuoiTonKho"] = firstRowInGroup["TuoiTonKho"];
                        (isNL ? tblNL : tblPL).Rows.Add(row1);
                    }

                    // Duyệt qua các dòng trong nhóm để thêm vào bảng nhập kho và gán STT
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
                        row2["Batch"] = dr["Batch"];
                        row2["DonGia"] = dr["DonGia"];
                        row2["ThanhTien"] = dr["ThanhTien"];
                        row2["TienTe"] = dr["TienTe"];
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
                        row2["NgayNKDuKien"] = ngay;
                        row2["tileNW"] = dr["tileNW"];
                        row2["tileGW"] = dr["tileGW"];
                        row2["MaDVCD"] = dr["MaDVCD"];
                        row2["TenDVCD"] = dr["TenDVCD"];
                        row2["MaVTGhep"] = dr["MaVTGhep"];
                        row2["MaNhom"] = dr["MaNhom"];
                        row2["SLTong"] = Convert.ToDecimal(dr["SLTong"] == DBNull.Value ? 0 : dr["SLTong"]);
                        if (row2["SoKienParent"] != DBNull.Value &&
                        !row2["SoKienParent"].Equals(row2["BarCode"]))
                        {
                            var tblTarget = isNL ? tblNhapKho : tblNhapKhoPL;
                            DataRow parentRow = tblTarget.AsEnumerable()
                                .FirstOrDefault(r => r["BarCode"].ToString() == row2["SoKienParent"].ToString());

                            if (parentRow != null)
                            {
                                decimal parentSL = parentRow["SoLuongThucTe"] != DBNull.Value ? Convert.ToDecimal(parentRow["SoLuongThucTe"]) : 0;
                                decimal childSL = row2["SoLuongThucTe"] != DBNull.Value ? Convert.ToDecimal(row2["SoLuongThucTe"]) : 0;
                                parentRow["SoLuongThucTe"] = parentSL - childSL;

                                decimal parentGhiDauCay = parentRow["SoGhiDauCay"] != DBNull.Value ? Convert.ToDecimal(parentRow["SoGhiDauCay"]) : 0;
                                decimal childGhiDauCay = row2["SoGhiDauCay"] != DBNull.Value ? Convert.ToDecimal(row2["SoGhiDauCay"]) : 0;
                                parentRow["SoGhiDauCay"] = parentGhiDauCay - childGhiDauCay;

                                decimal parentNW = parentRow["NW"] != DBNull.Value ? Convert.ToDecimal(parentRow["NW"]) : 0;
                                decimal childNW = row2["NW"] != DBNull.Value ? Convert.ToDecimal(row2["NW"]) : 0;
                                parentRow["NW"] = parentNW - childNW;

                                decimal parentGW = parentRow["GW"] != DBNull.Value ? Convert.ToDecimal(parentRow["GW"]) : 0;
                                decimal childGW = row2["GW"] != DBNull.Value ? Convert.ToDecimal(row2["GW"]) : 0;
                                parentRow["GW"] = parentGW - childGW;
                            }
                        }
                        row2["IsKiemKe"] = dr["IsKiemKe"];
                        row2["NgayKiemKe"] = dr["NgayKiemKe"];
                        row2["TuoiTonKho"] = dr["TuoiTonKho"];
                        row2["IsIn"] = false;
                        row2["NgayNhapKho"] = dr["NgayNhapKho"];
                        row2["Dot"] = dr["Dot"];
                        (isNL ? tblNhapKho : tblNhapKhoPL).Rows.Add(row2);
                        //sttTrongNhom++; // Tăng STT cho dòng tiếp theo trong cùng nhóm
                    }
                }

                // Gán DataSource cho GridControl
                (isNL ? gCNL : gCPL).DataSource = (isNL ? tblNL : tblPL);
                (isNL ? gCNLNhapKho : gCPLNhapKho).DataSource = (isNL ? tblNhapKho : tblNhapKhoPL);
                selectedRows.Clear();
                selectedRowsPL.Clear();
                //checkSearchLookUpVatTu(isNL);
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
                MessageBox.Show(
       $"Lỗi: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
       "Lỗi loadGVVatTu",
       MessageBoxButtons.OK,
       MessageBoxIcon.Error
   );
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

                        if (row["MaVTID"].ToString() == dr["MaVTID"].ToString() && dr["MaVTGhep"].ToString() == row["MaVTGhep"].ToString() &&
                            row["MauVTID"].ToString() == dr["MauVTID"].ToString() && row["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString() && row["MaNhom"].ToString() == dr["MaNhom"].ToString()
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
                MessageBox.Show("Vui lòng chọn Ngày nhập.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            else if (e.Column.FieldName == "MaHaiQuan")
            {
                object value = gVNL.GetRowCellValue(e.RowHandle, e.Column);
                DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (dr == null) return;
                string maVTID = dr["MaVTID"].ToString();
                string mauVTID = dr["MauVTID"].ToString();
                string khoVaiID = dr["KhoVaiID"].ToString();
                string maNhom = dr["MaNhom"].ToString();
                string MaVTGhep = dr["MaVTGhep"].ToString();
                foreach (DataRow _dr in tblNhapKho.Rows)
                {
                    if (_dr["MaVTID"].ToString() == maVTID && _dr["MauVTID"].ToString() == mauVTID && _dr["KhoVaiID"].ToString() == khoVaiID && _dr["MaNhom"].ToString() == maNhom)
                    {
                        _dr["MaHaiQuan"] = value.ToString();
                    }
                }
                DataTable tblNhapKhoFiltered = (_isNPL ? tblNhapKho : tblNhapKhoPL).Clone();
                var filteredRows = (_isNPL ? tblNhapKho : tblNhapKhoPL).AsEnumerable()
                    .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                  row.Field<string>("MauVTID") == mauVTID &&
                                  row.Field<string>("KhoVaiID") == khoVaiID &&
                                  row.Field<string>("MaNhom") == maNhom);
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
                string maNhom = dr["MaNhom"].ToString();
                string MaVTGhep = dr["MaVTGhep"].ToString();
                foreach (DataRow _dr in tblNhapKho.Rows)
                {
                    if (_dr["MaVTID"].ToString() == maVTID && _dr["MauVTID"].ToString() == MauVTID && _dr["KhoVaiID"].ToString() == khoVaiID && _dr["MaNhom"].ToString() == maNhom)
                    {
                        _dr["MaKeToan"] = value.ToString();
                    }
                }
                DataTable tblNhapKhoFiltered = (_isNPL ? tblNhapKho : tblNhapKhoPL).Clone();
                var filteredRows = (_isNPL ? tblNhapKho : tblNhapKhoPL).AsEnumerable()
                    .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                  row.Field<string>("MauVTID") == MauVTID &&
                                  row.Field<string>("KhoVaiID") == khoVaiID &&
                                  row.Field<string>("MaNhom") == maNhom &&
                                  row.Field<string>("MaVTGhep") == MaVTGhep);
                foreach (var row in filteredRows)
                {
                    tblNhapKhoFiltered.ImportRow(row);
                }

                (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource = tblNhapKhoFiltered;
            }
            else if (e.Column.FieldName == "TienTe")
            {
                {
                    DataRow drF = gVNL.GetFocusedDataRow();
                    if (drF == null) return;
                    DataTable tbl = gCNLNhapKho.DataSource as DataTable;
                    if (tbl != null && tbl.Rows.Count != 0)
                    {
                        foreach (DataRow dr in tbl.Rows)
                        {
                            dr["TienTe"] = drF["TienTe"].ToString();

                        }
                    }


                    string maVTID_F = drF["MaVTID"].ToString();
                    string mauVTID_F = drF["MauVTID"].ToString();
                    string khoVaiID_F = drF["KhoVaiID"].ToString();
                    string maNhom_F = drF["MaNhom"].ToString();
                    string maVTGhep_F = drF["MaVTGhep"].ToString();
                    string tienTe_F = drF["TienTe"].ToString();


                    var rowsToUpdate = tblNhapKho.AsEnumerable()
                         .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                        dr["MauVTID"].ToString() == mauVTID_F &&
                                        dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                        dr["MaNhom"].ToString() == maNhom_F);

                    foreach (var row in rowsToUpdate)
                    {
                        row["TienTe"] = tienTe_F;
                    }

                }

            }
            else if (e.Column.FieldName == "SLTong")
            {
                {
                    DataRow drF = gVNL.GetFocusedDataRow();
                    if (drF == null) return;
                    DataTable tbl = gCNLNhapKho.DataSource as DataTable;
                    if (tbl != null && tbl.Rows.Count != 0)
                    {
                        foreach (DataRow dr in tbl.Rows)
                        {
                            dr["SLTong"] = drF["SLTong"].ToString();

                        }
                    }



                    string maVTID_F = drF["MaVTID"].ToString();
                    string mauVTID_F = drF["MauVTID"].ToString();
                    string khoVaiID_F = drF["KhoVaiID"].ToString();
                    string maNhom_F = drF["MaNhom"].ToString();
                    string maVTGhep_F = drF["MaVTGhep"].ToString();
                    string SLTong_F = drF["SLTong"].ToString();


                    var rowsToUpdate = tblNhapKho.AsEnumerable()
                         .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                        dr["MauVTID"].ToString() == mauVTID_F &&
                                        dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                        dr["MaNhom"].ToString() == maNhom_F);

                    foreach (var row in rowsToUpdate)
                    {
                        row["SLTong"] = SLTong_F;
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
                string maVTGhep_F = drF["MaVTGhep"].ToString();
                string tuoi_F = drF["TuoiTonKho"].ToString();

                var rowsToUpdate = tblNhapKho.AsEnumerable()
                     .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                  dr["MauVTID"].ToString() == mauVTID_F &&
                                  dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                  dr["MaNhom"].ToString() == maNhom_F);

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

            else if (e.Column.FieldName == "MaHaiQuan")
            {
                object value = gVPL.GetRowCellValue(e.RowHandle, e.Column);
                DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
                if (dr == null) return;
                string maVTID = dr["MaVTID"].ToString();
                string MauVTID = dr["MauVTID"].ToString();
                string khoVaiID = dr["KhoVaiID"].ToString();
                string MaNhom = dr["MaNhom"].ToString();

                foreach (DataRow _dr in tblNhapKhoPL.Rows)
                {
                    if (_dr["MaVTID"].ToString() == maVTID && _dr["MauVTID"].ToString() == MauVTID && _dr["KhoVaiID"].ToString() == khoVaiID && _dr["MaNhom"].ToString() == MaNhom)
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
                //string MaVTGhep = dr["MaVTGhep"].ToString();
                foreach (DataRow _dr in tblNhapKhoPL.Rows)
                {
                    if (_dr["MaVTID"].ToString() == maVTID && _dr["MauVTID"].ToString() == MauVTID && _dr["KhoVaiID"].ToString() == khoVaiID && _dr["MaNhom"].ToString() == MaNhom)
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
                DataTable tbl = gCPLNhapKho.DataSource as DataTable;
                if (tbl != null && tbl.Rows.Count != 0)
                {
                    foreach (DataRow dr in tbl.Rows)
                    {
                        dr["TienTe"] = drF["TienTe"].ToString();

                    }
                }

                string maVTID_F = drF["MaVTID"].ToString();
                string mauVTID_F = drF["MauVTID"].ToString();
                string khoVaiID_F = drF["KhoVaiID"].ToString();
                string maNhom_F = drF["MaNhom"].ToString();
                string maVTGhep_F = drF["MaVTGhep"].ToString();
                string tienTe_F = drF["TienTe"].ToString();
                var rowsToUpdate = tblNhapKhoPL.AsEnumerable()
                    .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                   dr["MauVTID"].ToString() == mauVTID_F &&
                                   dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                   dr["MaNhom"].ToString() == maNhom_F);

                foreach (var row in rowsToUpdate)
                {
                    row["TienTe"] = tienTe_F;
                }

            }
            else if (e.Column.FieldName == "SLTong")
            {
                {
                    DataRow drF = gVPL.GetFocusedDataRow();
                    if (drF == null) return;
                    DataTable tbl = gCPLNhapKho.DataSource as DataTable;
                    if (tbl != null && tbl.Rows.Count != 0)
                    {
                        foreach (DataRow dr in tbl.Rows)
                        {
                            dr["SLTong"] = drF["SLTong"].ToString();

                        }
                    }


                    string maVTID_F = drF["MaVTID"].ToString();
                    string mauVTID_F = drF["MauVTID"].ToString();
                    string khoVaiID_F = drF["KhoVaiID"].ToString();
                    string maNhom_F = drF["MaNhom"].ToString();
                    string maVTGhep_F = drF["MaVTGhep"].ToString();
                    string SLTong_F = drF["SLTong"].ToString();


                    var rowsToUpdate = tblNhapKhoPL.AsEnumerable()
                         .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                        dr["MauVTID"].ToString() == mauVTID_F &&
                                        dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                        dr["MaNhom"].ToString() == maNhom_F &&
                                        dr["MaVTGhep"].ToString() == maVTGhep_F);

                    foreach (var row in rowsToUpdate)
                    {
                        row["SLTong"] = SLTong_F;
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

                var rowsToUpdate = tblNhapKhoPL.AsEnumerable()
                     .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                  dr["MauVTID"].ToString() == mauVTID_F &&
                                  dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                  dr["MaNhom"].ToString() == maNhom_F &&
                                  dr["MaVTGhep"].ToString() == maVTGhep_F);

                foreach (var row in rowsToUpdate)
                {
                    row["TuoiTonKho"] = tuoi_F;
                }
            }

        }
        private void ganLaiDonGia(bool isNL)
        {
            if (isNL)
            {
                DataRow dr = gVNL.GetFocusedDataRow();
                foreach (DataRow _row in tblNhapKho.Rows)
                {
                    if (dr["MaVTID"].ToString() == _row["MaVTID"].ToString() && dr["MauVTID"].ToString() == _row["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == _row["KhoVaiID"].ToString() && dr["MaNhom"].ToString() == _row["MaNhom"].ToString() && dr["MaVTGhep"].ToString() == _row["MaVTGhep"].ToString())
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
                    if (dr["MaVTID"].ToString() == _row["MaVTID"].ToString() && dr["MauVTID"].ToString() == _row["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == _row["KhoVaiID"].ToString() && dr["MaNhom"].ToString() == _row["MaNhom"].ToString() && dr["MaVTGhep"].ToString() == _row["MaVTGhep"].ToString())
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

                        if (dr["MaVTID"].ToString() == row["MaVTID"].ToString() && dr["MauVTID"].ToString() == row["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == row["KhoVaiID"].ToString() && dr["MaNhom"].ToString() == row["MaNhom"].ToString() && dr["MaVTGhep"].ToString() == row["MaVTGhep"].ToString())
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


                        if (dr["MaVTID"].ToString() == row["MaVTID"].ToString() && dr["MauVTID"].ToString() == row["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == row["KhoVaiID"].ToString() && dr["MaNhom"].ToString() == row["MaNhom"].ToString() && dr["MaVTGhep"].ToString() == row["MaVTGhep"].ToString())
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

                        ////DevExpress.Utils.Menu.DXMenuItem menuTachKienItem = new DevExpress.Utils.Menu.DXMenuItem("Tách kiện", TachKien);
                        ////e.Menu.Items.Add(menuTachKienItem);
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
                string MaVTGhep = drF["MaVTGhep"].ToString();
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
                             row["MaVTID"].ToString() == dr["MaVTID"].ToString() &&
                             row["MauVTID"].ToString() == dr["MauVTID"].ToString() &&
                            row["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString() &&
                             row["SoKienHienThi"].ToString() == dr["SoKienHienThi"].ToString() &&
                             row["MaNhom"].ToString() == dr["MaNhom"].ToString() &&
                            //row.Field<string>("MaVTGhep") == dr["MaVTGhep"].ToString() &&
                            row["SoLot"].ToString() == dr["SoLot"].ToString() &&
                             row["Batch"].ToString() == dr["Batch"].ToString()
                            )
                        .ToArray();

                    //                    string ids = string.Join(",",
                    //    rowsToDelete.Select(r => r["ID"].ToString())
                    //);
                    //                    string urlXH = $"{URL}ERPNhapKhoNPL/Get?Action=GETXHDELETE&para={ids.ToString()}&para2={GlobleData.UserName}";
                    //                    string resultXH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlXH); }).Result;
                    //                    if(resultXH!= "[]")
                    //                    {
                    //                        MessageBox.Show("Vật tư đã được xuất hàng. Không thể xóa!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //                        return;
                    //                    }    
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

                    string filter = $"MaVTID = {maVTID} AND MauVTID = {mauVTID} AND KhoVaiID = {khoVaiID} AND MaNhom = {MaNhom} AND MaVTGhep = {MaVTGhep}";
                    DataRow[] foundRows = tblNhapKho.Select(filter);

                    if (foundRows.Length > 0)
                    {

                        DataTable tblNhapKhoFiltered = tblNhapKho.Clone();
                        var filteredRows = tblNhapKho.AsEnumerable()
                        .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                      row.Field<string>("MauVTID") == mauVTID &&
                                      row.Field<string>("KhoVaiID") == khoVaiID &&
                                      row.Field<string>("MaNhom") == MaNhom &&
                                      row.Field<string>("MaVTGhep") == MaVTGhep);
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
                                      row.Field<string>("MaNhom") == dr["MaNhom"].ToString() &&
                                      row.Field<string>("MaVTGhep") == dr["MaVTGhep"].ToString());
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
                    string filter = $"MaVTID = {maVTID} AND MauVTID = {mauVTID} AND KhoVaiID = {khoVaiID} AND MaNhom = {MaNhom} AND MaVTGhep = {MaVTGhep}";
                    DataRow[] foundRows = tblNhapKhoPL.Select(filter);

                    if (foundRows.Length > 0)
                    {

                        DataTable tblNhapKhoFiltered = tblNhapKhoPL.Clone();
                        var filteredRows = tblNhapKhoPL.AsEnumerable()
                        .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                      row.Field<string>("MauVTID") == mauVTID &&
                                      row.Field<string>("KhoVaiID") == khoVaiID &&
                                      row.Field<string>("MaNhom") == MaNhom &&
                                      row.Field<string>("MaVTGhep") == MaVTGhep);
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
                                      row.Field<string>("MaNhom") == dr["MaNhom"].ToString() &&
                                      row.Field<string>("MaVTGhep") == dr["MaVTGhep"].ToString());
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


        private void gVNLNhapKho_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            var gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (gridView == null) return;
            gridView.BeginUpdate();
            try
            {
                DataRowView changedRowView = gridView.GetRow(e.RowHandle) as DataRowView;
                //if (changedRowView != null)
                //{
                //    DataRow changedDataRow = changedRowView.Row;
                //    var targetRow = tblNhapKho.AsEnumerable().FirstOrDefault(row =>
                //        row["MaVTID"].ToString() == changedDataRow["MaVTID"].ToString() &&
                //        row["MauVTID"].ToString() == changedDataRow["MauVTID"].ToString() &&
                //        row["KhoVaiID"].ToString() == changedDataRow["KhoVaiID"].ToString() &&
                //       row["SoKienHienThi"].ToString() == changedDataRow["SoKienHienThi"].ToString() &&
                //        row["SoLoT"].ToString() == changedDataRow["SoLoT"].ToString() &&
                //         row["Batch"].ToString() == changedDataRow["Batch"].ToString() &&
                //        row["MaNhom"].ToString() == changedDataRow["MaNhom"].ToString() 

                //    );

                //    if (targetRow != null)
                //    {
                //        if (changedDataRow.Table.Columns.Contains(e.Column.FieldName))
                //        {
                //            targetRow[e.Column.FieldName] = changedDataRow[e.Column.FieldName];
                //        }
                //    }
                //}
                if (changedRowView != null)
                {
                    DataRow changedDataRow = changedRowView.Row;

                    bool hasID = changedDataRow["ID"] != DBNull.Value && changedDataRow["ID"].ToString() != "0";

                    var targetRow = tblNhapKho.AsEnumerable().FirstOrDefault(row =>
                    {
                        bool baseCondition =
                            row.Field<string>("MaVTID") == changedDataRow.Field<string>("MaVTID") &&
                            row.Field<string>("MauVTID") == changedDataRow.Field<string>("MauVTID") &&
                            row.Field<string>("KhoVaiID") == changedDataRow.Field<string>("KhoVaiID") &&
                            row.Field<string>("MaNhom") == changedDataRow.Field<string>("MaNhom");

                        if (hasID)
                        {
                            return baseCondition &&
                                   row["ID"].ToString() == changedDataRow["ID"].ToString();
                        }
                        else
                        {
                            return baseCondition &&
                    row["Barcode"].ToString() == changedDataRow["Barcode"].ToString();
                        }
                    });

                    if (targetRow != null)
                    {
                        if (changedDataRow.Table.Columns.Contains(e.Column.FieldName))
                        {
                            targetRow[e.Column.FieldName] = changedDataRow[e.Column.FieldName];
                        }
                    }
                }

                gridView.CellValueChanged -= gVNLNhapKho_CellValueChanged;

                // Xử lý giá trị mặc định cho ô trống
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
                    //decimal tileNW = Convert.ToDecimal(gridView.GetRowCellValue(e.RowHandle, "tileNW") ?? 0);
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
                else if (e.Column.FieldName == "NW")
                {
                    //decimal khoiLuongMoi = Convert.ToDecimal(gridView.GetRowCellValue(e.RowHandle, "NW") ?? 0);
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
                    row2["MaVTGhep"].Equals(dr["MaVTGhep"])
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
                    row2["MaMauVT"].Equals(dr["MaMauVT"]) &&
                    row2["KhoVaiID"].Equals(dr["KhoVaiID"]) &&
                    row2["SoKienHienThi"].Equals(dr["SoKienHienThi"]) &&
                    row2["MaNhom"].Equals(dr["MaNhom"]) &&
                    row2["MaVTGhep"].Equals(dr["MaVTGhep"])
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

        private void gvSearchNL_ColumnFilterChanged(object sender, EventArgs e)
        {
            checkSearchLookUpVatTu2(true);
            //GridView view = sender as GridView;

            //for (int i = 0; i < view.RowCount; i++)
            //{
            //    DataRow row = view.GetDataRow(i);
            //    if (row != null && selectedRows.Contains(row))
            //    {
            //        view.SelectRow(i);
            //    }
            //}
        }

        private void gvSearchNL_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            int rowHandle3 = e.ControllerRow;
            if (view == null) return;
            if (rowHandle3 >= 0)
            {
                DataRow row = view.GetDataRow(rowHandle3);

                if (view.IsRowSelected(rowHandle3))
                {
                    selectedRows.Add(row);

                }
                else
                {
                    selectedRows.Remove(row);

                }
            }


            string selectedValues = string.Join(";", gVSearchNL.GetSelectedRows().Select(rowHandle2 => gVSearchNL.GetRowCellValue(rowHandle2, searchLookUpEditNL.Properties.ValueMember)));
            searchLookUpEditNL.EditValue = selectedValues;
            if (searchLookUpEditNL.EditValue is null) return;
            _NL = searchLookUpEditNL.EditValue.ToString();


            if (e.Action == CollectionChangeAction.Add)
            {
                // Khi người dùng chọn dòng
                int[] selectedRows = gVSearchNL.GetSelectedRows();
                foreach (int rowHandle in selectedRows)
                {
                    if (rowHandle < 0) continue;
                    DataRow drRow = gVSearchNL.GetDataRow(rowHandle);
                    AddRowGCNL(drRow);
                }
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                DataRow removedRow = gVSearchNL.GetDataRow(e.ControllerRow);
                Remove(removedRow);
            }
            if (e.Action == CollectionChangeAction.Refresh)
            {
                int selectedCount = gVSearchNL.SelectedRowsCount;
                int totalRowCount = gVSearchNL.RowCount;

                if (selectedCount == totalRowCount && selectedCount > 0)
                {
                    for (int i = 0; i < totalRowCount; i++)
                    {
                        if (!gVSearchNL.IsRowSelected(i)) continue;
                        DataRow drRow = gVSearchNL.GetDataRow(i);
                        AddRowGCNL(drRow);
                    }
                }
                else if (selectedCount == 0)
                {
                    selectedRows.Clear();
                }
            }
            gVSearchNL.RefreshData();
        }
        private void AddRowGCNL(DataRow rowAdd)
        {
            try
            {
                if (rowAdd == null) return;
                DataTable tbl = (_isNPL ? gCNL : gCPL).DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    tbl = CreateTableVatTu();
                }

                string matiente = string.Empty;
                string filterExpression = "MaTienTe = 'USD'";
                DataRow foundRows = tblSearchTienTe.Select(filterExpression).FirstOrDefault();
                if (foundRows != null)
                    matiente = foundRows["TienTeID"].ToString();



                //string filter = string.Format("MaVTID = '{0}' AND MauVTID = '{1}' AND MaNhom = '{2}' AND KhoVaiID = '{3}' AND MaVTGhep = '{4}'",
                //                 rowAdd["MaVTID"].ToString(),
                //                 rowAdd["MauVTID"].ToString(),
                //                 rowAdd["MaNhom"].ToString(),
                //                 rowAdd["KhoVaiID"].ToString(),
                //                 rowAdd["MaVTGhep"].ToString());
                string filter = string.Format("MaVTID = '{0}' AND MauVTID = '{1}' AND MaNhom = '{2}' AND KhoVaiID = '{3}' AND MaVTGhep = '{4}'",
                                 EscapeSingleQuotes(rowAdd["MaVTID"]),
                                 EscapeSingleQuotes(rowAdd["MauVTID"]),
                                 EscapeSingleQuotes(rowAdd["MaNhom"]),
                                 EscapeSingleQuotes(rowAdd["KhoVaiID"]),
                                 EscapeSingleQuotes(rowAdd["MaVTGhep"]));
                DataRow[] existingRows = tbl.Select(filter);
                if (existingRows.Length == 0)
                {
                    DataRow newRow = tbl.NewRow();
                    newRow["MaVTID"] = rowAdd["MaVTID"];
                    newRow["MaVT"] = rowAdd["MaVT"];
                    newRow["ChiTiet"] = rowAdd["ChiTiet"];
                    newRow["MaNhom"] = rowAdd["MaNhom"];
                    newRow["TenNhom"] = rowAdd["TenNhom"];
                    newRow["Sort"] = rowAdd["Sort"];
                    newRow["MaMauVT"] = rowAdd["MaMauVT"];
                    newRow["MauVT"] = rowAdd["MauVT"];
                    newRow["KhoVaiID"] = rowAdd["KhoVaiID"];
                    newRow["KhoVai"] = rowAdd["KhoVai"];
                    newRow["MaHaiQuan"] = "";
                    newRow["MaKeToan"] = "";
                    newRow["SoKienParent"] = "";
                    newRow["DonGia"] = 0;
                    newRow["ThanhTien"] = 0;
                    newRow["MaDVVT"] = rowAdd["MaDVVT"];
                    newRow["TenDVVT"] = rowAdd["TenDVVT"];
                    newRow["MauVTID"] = rowAdd["MauVTID"];
                    newRow["MaVTGhep"] = rowAdd["MaVTGhep"];
                    newRow["TienTe"] = matiente;
                    newRow["SLTong"] = 0;
                    newRow["MaHaiQuan"] = rowAdd["MaHaiQuan"];
                    newRow["IsThemNhanh"] = false;
                    if (rowAdd.Table.Columns.Contains("STT") && rowAdd["STT"] != DBNull.Value)
                    {
                        if (!tbl.Columns.Contains("STT"))
                        {
                            tbl.Columns.Add("STT", typeof(int));
                        }
                        newRow["STT"] = rowAdd["STT"];
                    }
                    tbl.Rows.Add(newRow);
                    (_isNPL ? gCNL : gCPL).DataSource = tbl;
                    BestFitCol(_isNPL ? gVNL : gVPL);

                }
                else
                {
                    if (rowAdd.Table.Columns.Contains("STT") && rowAdd["STT"] != DBNull.Value)
                    {
                        if (!tbl.Columns.Contains("STT"))
                        {
                            tbl.Columns.Add("STT", typeof(int));
                        }
                        existingRows[0]["STT"] = rowAdd["STT"];
                    }
                }
                string maNPL = RemoveVietnameseTone(ReplaceSpecialCharacters(rowAdd["MaNhom"].ToString()) + "@"
                    + ReplaceSpecialCharacters(rowAdd["MaVTID"].ToString()) + "@"
                    + ReplaceSpecialCharacters(rowAdd["MauVTID"].ToString()) + "@"
                    + ReplaceSpecialCharacters(rowAdd["KhoVaiID"].ToString()));
                #region T
                var newRows = new List<DataRow>();
                DataTable tblNhap = _isNPL ? tblNhapKho : tblNhapKhoPL;
                //gVNLNhapKho.AddNewRow();
                bool exists = tblNhap.AsEnumerable().Any(r =>
                    (r.Field<string>("MaVTID") ?? "") == (rowAdd["MaVTID"]?.ToString() ?? "") &&
                    (r.Field<string>("MauVTID") ?? "") == (rowAdd["MauVTID"]?.ToString() ?? "") &&
                    (r.Field<string>("MaNhom") ?? "") == (rowAdd["MaNhom"]?.ToString() ?? "") &&
                    (r.Field<string>("KhoVaiID") ?? "") == (rowAdd["KhoVaiID"]?.ToString() ?? "") &&
                    (r.Field<string>("MaVTGhep") ?? "") == (rowAdd["MaVTGhep"]?.ToString() ?? "")
                );


                if (!exists)
                {
                    DataRow nr = tblNhap.NewRow();
                    nr["ID"] = 0;
                    nr["SoLoID"] = _soloid;
                    nr["MaNPL"] = maNPL;
                    nr["MaVTID"] = rowAdd["MaVTID"].ToString();
                    nr["MaMauVT"] = rowAdd["MaMauVT"].ToString();
                    nr["MauVT"] = rowAdd["MauVT"].ToString();
                    nr["SoKien"] = "";
                    nr["SoLot"] = "";
                    nr["MaHaiQuan"] = rowAdd["MaHaiQuan"];
                    nr["MaKeToan"] = rowAdd["MaKeToan"];
                    nr["SoGhiDauCay"] = 0;
                    nr["NW"] = 0;
                    nr["GW"] = 0;
                    nr["BarCode"] = "";
                    nr["GhiChu"] = "";
                    nr["IsNPL"] = _isNPL;
                    nr["KhoVaiID"] = rowAdd["KhoVaiID"];
                    nr["SoKienParent"] = "";
                    nr["DonGia"] = 0;
                    nr["ThanhTien"] = 0;
                    nr["SoLuongThucTe"] = 0;
                    nr["Pallet"] = "";
                    nr["MaDVVT"] = rowAdd["MaDVVT"];
                    nr["MauVTID"] = rowAdd["MauVTID"];
                    nr["SoKienHienThi"] = "";
                    if (rowAdd.Table.Columns.Contains("STT") && rowAdd["STT"] != DBNull.Value)
                    {
                        nr["STT"] = rowAdd["STT"];
                        nr["STTChonVT"] = rowAdd["STT"];
                    }
                    else
                    {
                        int newSTT = tblNhap.AsEnumerable()
                           .Where(r => r["STT"] != DBNull.Value)
                                          .Select(r => Convert.ToInt32(r["STT"]))
                           .DefaultIfEmpty(0)
                           .Max() + 1;
                        nr["STT"] = 0;
                        nr["STTChonVT"] = newSTT;
                    }
                    nr["IsNK"] = false;
                    nr["TienTe"] = matiente;
                    nr["MaVTGhep"] = rowAdd["MaVTGhep"];
                    nr["MaNhom"] = rowAdd["MaNhom"];
                    nr["MaDVCD"] = "";
                    nr["TenDVCD"] = "";
                    nr["Batch"] = "";

                    tblNhap.Rows.Add(nr);
                }
                (_isNPL ? gCNL : gCPL).RefreshDataSource();
                #endregion
            }
            catch (Exception ex)
            {

            }
        }
        string EscapeSingleQuotes(object value)
        {
            if (value == null || value == DBNull.Value)
                return "";
            return value.ToString().Replace("'", "''");
        }
        private void Remove(DataRow rowAdd)
        {
            try
            {
                if (rowAdd == null) return;
                string maVTID = rowAdd["MaVTID"].ToString();
                string khoVaiID = rowAdd["KhoVaiID"].ToString();
                string maNhom = rowAdd["MaNhom"].ToString();
                string mauVTID = rowAdd["MauVTID"].ToString();
                string maVTGhep = rowAdd["MaVTGhep"].ToString();
                // Sử dụng DataTable.Select() thay vì LINQ - nhanh hơn
                string filter = $"MaVTID = '{maVTID}' AND KhoVaiID = '{khoVaiID}' AND MaNhom = '{maNhom}' AND MauVTID = '{mauVTID}' AND MaVTGhep = '{maVTGhep}'";

                DataTable tbl = (_isNPL ? gCNL : gCPL).DataSource as DataTable;
                if (tbl != null)
                {
                    DataRow[] rows = tbl.Select(filter);
                    foreach (DataRow row in rows)
                        tbl.Rows.Remove(row);
                }



                DataTable tbl2 = (_isNPL ? tblNhapKho : tblNhapKhoPL);
                if (tbl2 != null)
                {
                    DataRow[] rows2 = tbl2.Select(filter);
                    foreach (DataRow row in rows2)
                        tbl2.Rows.Remove(row);
                }



                DataTable tbl3 = (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource as DataTable;
                if (tbl3 != null)
                {
                    DataRow[] rows3 = tbl3.Select(filter);
                    foreach (DataRow row in rows3)
                        tbl3.Rows.Remove(row);
                }
            }
            catch (Exception ex)
            {


            }
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
                loadTxt(true);

                loadGVVatTu(true);

                createSearchLookUpLo();
                setTxtNull();
                if (_drPO != null)
                {
                    searchLookUpEditLoNL.EditValue = _drPO["SoLoID"].ToString().Split('@')[0];
                }

            }
            else if (selectedTabPage.Name == "tabPL")
            {
                _isNPL = false;
                CheckPerminsionPL();
                //CopyFromTxtToTxtPL();
                loadTxt(false);

                loadGVVatTu(false);
                createSearchLookUpLo();
                setTxtNull();
                if (_drPO != null)
                {
                    searchLookUpEditLoPL.EditValue = _drPO["SoLoID"].ToString().Split('@')[0];
                }
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

                        ////DevExpress.Utils.Menu.DXMenuItem menuTachKienItem = new DevExpress.Utils.Menu.DXMenuItem("Tách kiện", TachKien);
                        ////e.Menu.Items.Add(menuTachKienItem);
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
                    if (dr["MaVTID"].ToString() == _dr["MaVTID"].ToString() && dr["MauVTID"].ToString() == _dr["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == _dr["KhoVaiID"].ToString() && dr["MaNhom"].ToString() == _dr["MaNhom"].ToString() && dr["MaVTGhep"].ToString() == _dr["MaVTGhep"].ToString()
                       && Convert.ToInt32(_dr["STT"]) >= Convert.ToInt32(_tukien) && Convert.ToInt32(_dr["STT"]) <= Convert.ToInt32(_denkien))
                    {
                        _dr["SoLot"] = _lot;
                    }
                }
                foreach (DataRow _dr in tblNK.Rows)
                {
                    if (dr["MaVTID"].ToString() == _dr["MaVTID"].ToString() && dr["MauVTID"].ToString() == _dr["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == _dr["KhoVaiID"].ToString() && dr["MaNhom"].ToString() == _dr["MaNhom"].ToString() && dr["MaVTGhep"].ToString() == _dr["MaVTGhep"].ToString()
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

            // Tạm dừng mọi cập nhật giao diện và sự kiện để xử lý logic
            gridView.BeginUpdate();
            try
            {
                // === PHẦN 1: Tối ưu hóa việc đồng bộ dữ liệu giữa hai DataTable ===
                DataRowView changedRowView = gridView.GetRow(e.RowHandle) as DataRowView;
                //if (changedRowView != null)
                //{
                //    DataRow changedDataRow = changedRowView.Row;

                //    // Tìm chính xác dòng cần cập nhật trong tblNhapKhoPL bằng LINQ, thay vì lặp
                //    var targetRow = tblNhapKhoPL.AsEnumerable().FirstOrDefault(row =>
                //        row.Field<string>("MaVTID") == changedDataRow.Field<string>("MaVTID") &&
                //        row.Field<string>("MauVTID") == changedDataRow.Field<string>("MauVTID") &&
                //        row.Field<string>("KhoVaiID") == changedDataRow.Field<string>("KhoVaiID") &&
                //         row["SoKienHienThi"].ToString() == changedDataRow["SoKienHienThi"].ToString() &&
                //            row["SoLoT"].ToString() == changedDataRow["SoLoT"].ToString() &&
                //         row["Batch"].ToString() == changedDataRow["Batch"].ToString() &&
                //        row.Field<string>("MaNhom") == changedDataRow.Field<string>("MaNhom")
                //    );
                //    if (targetRow != null)
                //    {
                //        targetRow.BeginEdit();
                //        targetRow["SoLot"] = changedDataRow["SoLot"];
                //        targetRow["SoGhiDauCay"] = changedDataRow["SoGhiDauCay"];
                //        targetRow["SoLuongThucTe"] = changedDataRow["SoLuongThucTe"];
                //        targetRow["Pallet"] = changedDataRow["Pallet"];
                //        targetRow["NW"] = changedDataRow["NW"];
                //        targetRow["GW"] = changedDataRow["GW"];
                //        targetRow["GhiChu"] = changedDataRow["GhiChu"];
                //        targetRow["SoKienHienThi"] = changedDataRow["SoKienHienThi"];
                //        targetRow["Batch"] = changedDataRow["Batch"];
                //        targetRow["IsIn"] = changedDataRow["IsIn"];
                //        targetRow.EndEdit();
                //    }
                //}
                if (changedRowView != null)
                {
                    DataRow changedDataRow = changedRowView.Row;

                    bool hasID = changedDataRow["ID"] != DBNull.Value && changedDataRow["ID"].ToString() != "0";

                    var targetRow = tblNhapKhoPL.AsEnumerable().FirstOrDefault(row =>
                    {
                        bool baseCondition =
                            row.Field<string>("MaVTID") == changedDataRow.Field<string>("MaVTID") &&
                            row.Field<string>("MauVTID") == changedDataRow.Field<string>("MauVTID") &&
                            row.Field<string>("KhoVaiID") == changedDataRow.Field<string>("KhoVaiID") &&
                            row.Field<string>("MaNhom") == changedDataRow.Field<string>("MaNhom");

                        if (hasID)
                        {
                            return baseCondition &&
                                   row["ID"].ToString() == changedDataRow["ID"].ToString();
                        }
                        else
                        {
                            return baseCondition &&
                    row["Barcode"].ToString() == changedDataRow["Barcode"].ToString();
                        }
                    });

                    if (targetRow != null)
                    {
                        if (changedDataRow.Table.Columns.Contains(e.Column.FieldName))
                        {
                            targetRow[e.Column.FieldName] = changedDataRow[e.Column.FieldName];
                        }
                    }
                }
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

        private void gVNL_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                DataRow dr = gVNL.GetFocusedDataRow();
                if (dr == null && tblNL.Rows.Count > 0)
                {
                    dr = tblNL.Rows[0];
                }
                if (dr == null) return;
                if (tblNhapKho == null || tblNhapKho.Rows.Count == 0) CreateTableNhapKho();
                string maVTID = dr["MaVTID"].ToString();
                string maMauVT = dr["MauVTID"].ToString();
                string khoVaiID = dr["KhoVaiID"].ToString();
                string MaNhom = dr["MaNhom"].ToString();
                string MaVTGhep = dr["MaVTGhep"].ToString();
                DataTable tblNhapKhoFiltered = tblNhapKho.Clone();
                var filteredRows = tblNhapKho.AsEnumerable()
                .Where(row => row.Field<string>("MaVTID") == maVTID &&
                              row.Field<string>("MauVTID") == maMauVT &&
                              row.Field<string>("KhoVaiID") == khoVaiID &&
                              row.Field<string>("MaNhom") == MaNhom);
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
            }
            catch (Exception ex)
            {
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
                string MaNhom = dr["MaNhom"].ToString();
                string MaVTGhep = dr["MaVTGhep"].ToString();
                DataTable tblNhapKhoFiltered = tblNhapKhoPL.Clone();
                var filteredRows = tblNhapKhoPL.AsEnumerable()
                .Where(row => row.Field<string>("MaVTID") == maVTID &&
                              row.Field<string>("MauVTID") == maMauVT &&
                              row.Field<string>("KhoVaiID") == khoVaiID &&
                              row.Field<string>("MaNhom") == MaNhom);
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
            searchLookUpEditNL.ClosePopup();
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

        }

        #region barbutton
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = simpleButton1;
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
                if (searchLookUpEditLoNL.EditValue == null) return;
                if (string.IsNullOrWhiteSpace(searchLookUpEditLoNL.Text))
                {
                    MessageBox.Show("Vui lòng chọn PI NCC.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    searchLookUpEditLoNL.Focus();
                    return;
                }
                string pomua = txtPOMua.Text.ToString();
                List<string> currentList = new List<string>();
                if (tblNhapKho == null || tblNhapKho.Rows.Count == 0) CreateTableNhapKho();
                decimal tileNW = 0m, tileGW = 0m;
                string madvcd = string.Empty, tendvcd = string.Empty, batch = string.Empty;
                int sttChonVT = 1;
                //int sttTrongNhom = 1;
                if (dr.Table.Columns.Contains("STTChonVT") && dr["STTChonVT"] != DBNull.Value)
                {
                    sttChonVT = Convert.ToInt32(dr["STTChonVT"]);
                }
                DataRow _dr = gVNLNhapKho.GetFocusedDataRow();
                if (_dr != null)
                {
                    tileNW = decimal.TryParse(_dr["tileNW"].ToString(), out var val) ? val : 0m;
                    tileGW = decimal.TryParse(_dr["tileGW"].ToString(), out var val2) ? val2 : 0m;
                    madvcd = _dr["MaDVCD"].ToString();
                    tendvcd = _dr["TenDVCD"].ToString();
                    batch = _dr["Batch"].ToString();
                }
                frmERPNhapKhoNPL_KhaiBao frm = new frmERPNhapKhoNPL_KhaiBao(dr, tblNhapKho, searchLookUpEditLoNL.Text.ToString(), true, tileNW, tileGW, madvcd, tendvcd, _soloid, batch, pomua);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ThongSoKienChanged += (list) =>
                {

                    tblNhapKho = frmERPNhapKhoNPL_KhaiBao.dt;
                    string maVTID = dr["MaVTID"].ToString();
                    string MauVTID = dr["MauVTID"].ToString();
                    string khoVaiID = dr["KhoVaiID"].ToString();
                    string MaNhom = dr["MaNhom"].ToString();
                    string MaVTGhep = dr["MaVTGhep"].ToString();
                    DataTable tblNhapKhoFiltered = tblNhapKho.Clone();
                    var filteredRows = tblNhapKho.AsEnumerable()
                        .Where(row => row["MaVTID"].ToString() == maVTID &&
                                      row["MauVTID"].ToString() == MauVTID &&
                                      row["KhoVaiID"].ToString() == khoVaiID &&
                                      row["MaNhom"].ToString() == MaNhom &&
                                      row["MaVTGhep"].ToString() == MaVTGhep);
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
                if (searchLookUpEditLoPL.EditValue == null) return;
                if (string.IsNullOrWhiteSpace(searchLookUpEditLoPL.Text))
                {
                    MessageBox.Show("Vui lòng chọn PI NCC.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    searchLookUpEditLoPL.Focus();
                    return;
                }


                List<string> currentList = new List<string>();
                if (tblNhapKhoPL == null || tblNhapKhoPL.Rows.Count == 0) CreateTableNhapKhoPL();
                string pomua = txtPOMuaPL.Text.ToString();
                DataRow _dr = gVPLNhapKho.GetFocusedDataRow();
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
                frmERPNhapKhoNPL_KhaiBao frm = new frmERPNhapKhoNPL_KhaiBao(dr, tblNhapKhoPL, searchLookUpEditLoPL.Text.ToString(), false, tileNW, tileGW, madvcd, tendvcd, _soloid, batch, pomua);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ThongSoKienChanged += (list) =>
                {
                    tblNhapKhoPL = frmERPNhapKhoNPL_KhaiBao.dt;
                    DataTable tblNhapKhoFiltered = tblNhapKhoPL.Clone();
                    string maVTID = dr["MaVTID"].ToString();
                    string MauVTID = dr["MauVTID"].ToString();
                    string khoVaiID = dr["KhoVaiID"].ToString();
                    string MaNhom = dr["MaNhom"].ToString();
                    string MaVTGhep = dr["MaVTGhep"].ToString();
                    var filteredRows = tblNhapKhoPL.AsEnumerable()
                        .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                      row.Field<string>("MauVTID") == MauVTID &&
                                      row.Field<string>("KhoVaiID") == khoVaiID &&
                                      row["MaNhom"].ToString() == MaNhom &&
                                      row["MaVTGhep"].ToString() == MaVTGhep);
                    foreach (var row in filteredRows)
                    {
                        row["STT"] = sttChonVT;
                        row["STTChonVT"] = sttChonVT;
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
        private void searchLookUpEdit1_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {

            //var selectedRows = gVSearchPL.GetSelectedRows();
            //var selectedValues = selectedRows
            //    .Select(rowHandle => gVSearchPL.GetRowCellValue(rowHandle, searchLookUpEditPL.Properties.DisplayMember)?.ToString())
            //    .Where(value => !string.IsNullOrWhiteSpace(value)) // Loại bỏ giá trị null, rỗng hoặc chỉ chứa ký tự trắng
            //    .ToList();

            //_PLDisplay = selectedValues.Any() ? string.Join("; ", selectedValues) : "Chọn phụ liệu";
            //e.DisplayText = _PLDisplay;

            if (selectedRowsPL != null && selectedRowsPL.Count > 0)
            {
                // Lấy giá trị theo DisplayMember (ví dụ "TenPL")
                var selectedValues = selectedRowsPL
                    .Select(r => r["MaVTGhep"]?.ToString().Replace("\r", "").Replace("\n", ""))   // đổi "TenPL" thành DisplayMember thực tế
                    .Where(val => !string.IsNullOrWhiteSpace(val))
                    .ToList();

                e.DisplayText = string.Join("; ", selectedValues);
                //Console.WriteLine(string.Join("; ", selectedValues).Length + " hihi " + string.Join("; ", selectedValues).Replace("\r", "").Replace("\n", ""));
            }
            else
            {
                e.DisplayText = "Chọn phụ liệu";
            }
        }




        private void searchLookUpEditNL_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {

            if (selectedRows != null && selectedRows.Count > 0)
            {
                // Lấy giá trị theo DisplayMember (ví dụ "TenPL")
                var selectedValues = selectedRows
                    .Select(r => r["MaVTGhep"]?.ToString().Replace("\r", "").Replace("\n", ""))   // đổi "TenPL" thành DisplayMember thực tế
                    .Where(val => !string.IsNullOrWhiteSpace(val))
                    .ToList();

                e.DisplayText = string.Join("; ", selectedValues);
            }
            else
            {
                e.DisplayText = "Chọn phụ liệu";
            }


            //var selectedRows = gVSearchNL.GetSelectedRows();
            //var selectedValues = selectedRows
            //    .Select(rowHandle => gVSearchNL.GetRowCellValue(rowHandle, searchLookUpEditNL.Properties.DisplayMember)?.ToString())
            //    .Where(value => !string.IsNullOrWhiteSpace(value)) // Loại bỏ giá trị null, rỗng hoặc chỉ chứa ký tự trắng
            //    .ToList();

            //_NLDisplay = selectedValues.Any() ? string.Join("; ", selectedValues) : "Chọn nguyên liệu";
            //e.DisplayText = _NLDisplay;
        }
        private void gVSearchPL_ColumnFilterChanged(object sender, EventArgs e)
        {
            checkSearchLookUpVatTu2(false);

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
            //        e.Cancel = true;gVPLNhapKho_ShowingEditor
            //    }
            //}
            //else if (view.FocusedColumn.FieldName == "MaKeToan" && !string.IsNullOrWhiteSpace(dr["MaKeToan"].ToString())) // Kiểm tra cột hiện tại
            //{
            //    if (_lstMaKT.Contains(dr["MaKeToan"].ToString()))
            //    {
            //        e.Cancel = true;
            //    }
            //}
            if (view.FocusedColumn != null && view.FocusedColumn.FieldName == "IsIn")
            {
                e.Cancel = true;
            }
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

            var column = view.FocusedColumn;
            if (view.FocusedColumn != null && view.FocusedColumn.FieldName == "IsIn")
            {
                e.Cancel = true;
            }
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

        private void btnXoaLo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string url;
                string text = string.Empty;
                if (_isNPL)
                {
                    if (searchLookUpEditLoNL.EditValue == null || searchLookUpEditLoNL.EditValue.ToString() == "") return;
                    text = searchLookUpEditLoNL.Text.ToString();
                    url = $"{URL}ERPNhapKhoNPL/Delete?Action=DELETELO&para={ searchLookUpEditLoNL.EditValue.ToString()}&para2={GlobleData.UserName}";
                }
                else
                {
                    if (searchLookUpEditLoPL.EditValue == null || searchLookUpEditLoPL.EditValue.ToString() == "") return;
                    text = searchLookUpEditLoPL.Text.ToString();
                    url = $"{URL}ERPNhapKhoNPL/Delete?Action=DELETELO&para={ searchLookUpEditLoPL.EditValue.ToString()}&para2={GlobleData.UserName}";
                }
                DialogResult messResult = MessageBox.Show($"Bạn có muốn xóa PI NCC:{ text} không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 1000);
                        setTxtNull();
                        setGVNull();
                        huySelect();
                        createSearchLookUpLo();
                    }
                }
            }
            catch (Exception ex)
            {


            }




        }

        private void gCPLNhapKho_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteCell(sender, e);
            }
        }

        private void searchLookUpEditLoNL_EditValueChanged(object sender, EventArgs e)
        {
            loadTxt(true);

            loadGVVatTu(true);
        }

        private void searchLookUpEditLoPL_EditValueChanged(object sender, EventArgs e)
        {
            loadTxt(false);

            loadGVVatTu(false);
        }

        private void gVNLNhapKho_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var column = view.FocusedColumn;
            DataRow dr = gVNLNhapKho.GetFocusedDataRow();
            if (dr == null) return;
            bool _isEdit = checkEdit(dr);
            //if (column.FieldName == "IsIn")
            //{
            //    var value = view.GetFocusedValue();
            //    bool isin = !Convert.ToBoolean(value);
            //    view.SetFocusedValue(isin);
            //    return;
            //}
            if (_isEdit)
            {
                if (column.FieldName == "SoLuongThucTe")
                {
                    bool isKiemKeChecked = cbKiemKe.Checked;
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

            if (view.FocusedColumn != null && view.FocusedColumn.FieldName == "IsIn")
            {
                e.Cancel = true;
            }
            //if (column.FieldName == "IsIn")
            //{
            //    var value = view.GetFocusedValue();
            //    bool isin = !Convert.ToBoolean(value);
            //    view.SetFocusedValue(isin);
            //    return;
            //}
            if (_isEdit)
            {
                if (column.FieldName == "SoLuongThucTe")
                {
                    bool isKiemKeChecked = cbKiemKePL.Checked;
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
            splitContainerControl1.SplitterPosition = (int)(splitContainerControl1.Width * 0.60);
        }

        private void splitContainerControl2_Paint(object sender, PaintEventArgs e)
        {
            splitContainerControl2.SplitterPosition = (int)(splitContainerControl2.Width * 0.60);
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
                        if (row["MaVTID"].ToString() == dr["MaVTID"].ToString() && row["MauVTID"].ToString() == dr["MauVTID"].ToString() && row["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString() && row["MaNhom"].ToString() == dr["MaNhom"].ToString() && row["MaVTGhep"].ToString() == dr["MaVTGhep"].ToString())
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
                        if (row["MaVTID"].ToString() == dr["MaVTID"].ToString() && row["MauVTID"].ToString() == dr["MauVTID"].ToString() && row["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString() && row["MaNhom"].ToString() == dr["MaNhom"].ToString() && row["MaVTGhep"].ToString() == dr["MaVTGhep"].ToString())
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

                if (txtPOMua.EditValue != null)
                    txtPOMua.EditValue = null;
                searchLookUpEditNCC.EditValue = null;
                txtSoToKhai.Text = "";
                dateNgayToKhai.EditValue = null;
                txtSoHopDong.Text = "";
                dateSoHopDong.EditValue = null;
                txtMaSoThue.Text = "";
                txtSoHoaDon.Text = "";
                searchLookUpEditCang.EditValue = null;
                SearchlookupTau.EditValue = null;
                dateNgayGui.EditValue = null;
                txtNoiGui.Text = "";

                txtSoVanDon.Text = "";
                txtQCKiem.Text = "";
                txtNW.Text = "";
                txtGW.Text = "";
                cbKiemKe.Checked = false;
                ngayKK.EditValue = null;
                dateNgayNK.EditValue = null;
                searchLookUpEditKHCT.EditValue = null;
                searchLookUpEditMHCT.EditValue = null;
                btnPDF.Text = "";
            }
            else
            {
                if (searchLookUpEditLoPL.EditValue != null)
                    searchLookUpEditLoPL.EditValue = null;

                if (txtPOMuaPL.EditValue != null)
                    txtPOMuaPL.EditValue = null;
                searchLookUpEditNCCPL.EditValue = null;
                txtSoToKhaiPL.Text = "";
                dateNgayToKhaiPL.EditValue = null;
                txtSoHopDongPL.Text = "";
                dateSoHopDongPL.EditValue = null;
                txtMaSoThuePL.Text = "";
                txtSoHoaDonPL.Text = "";
                searchLookUpEditCangPL.EditValue = null;
                SearchlookupTauPL.EditValue = null;
                dateNgayGuiPL.EditValue = null;
                txtNoiGuiPL.Text = "";

                txtSoVanDonPL.Text = "";
                txtQCKiemPL.Text = "";
                txtNWPL.Text = "";
                txtGWPL.Text = "";
                cbKiemKePL.Checked = false;
                ngayKKPL.EditValue = null;
                dateNgayNKPL.EditValue = null;
                searchLookUpEditKHCT.EditValue = null;
                searchLookUpEditMHCT.EditValue = null;
                btnPDFPL.Text = "";
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
        private void huySelect()
        {

        }
        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\”|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
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
        private bool luuNhapKho()
        {
            try
            {
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

                }

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
                if (tblNhapKho.Columns.Contains("QuyDoiID"))
                {
                    tblNhapKho.Columns.Remove("QuyDoiID");
                }
                if (tblNhapKho.Columns.Contains("STT"))
                {
                    tblNhapKho.Columns.Remove("STT");
                }
                if (tblNhapKho.Columns.Contains("IsIn"))
                {
                    tblNhapKho.Columns.Remove("IsIn");
                }
                if (tblNhapKho.Columns.Contains("NgayNhapKho"))
                {
                    tblNhapKho.Columns.Remove("NgayNhapKho");
                }
                if (tblNhapKho.Columns.Contains("Dot"))
                {
                    tblNhapKho.Columns.Remove("Dot");
                }
                foreach (DataRow dr in tblNhapKho.Rows)
                {
                    //dr["SoloID"] = searchLookUpEditLoNL.EditValue.ToString();
                    //dr["NgayNKDuKien"] = dateNgayNK.EditValue == null ? (object)DBNull.Value : dateNgayNK.EditValue;
                    if (string.IsNullOrWhiteSpace(dr["DonGia"].ToString()))
                    {
                        dr["DonGia"] = 0;
                    }
                    dr["POMua"] = txtPOMua.Text.ToString();
                    dr["IsKiemKe"] = cbKiemKe.Checked;
                    dr["NgayKiemKe"] = ngayKK.EditValue == null ? (object)DBNull.Value : (DateTime)ngayKK.EditValue;
                    dr["SoLoID"] = searchLookUpEditLoNL.EditValue.ToString();
                }

                string url = $"{URL}ERPNhapKhoNPL/Post?Action=POSTCT&para={GlobleData.UserName}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblNhapKho); }).Result;
                if (msResult.ToLower() == "true")
                {
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
                if (view == null) return;

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
            }
            if (e.Column.FieldName == "SoGhiDauCay" || e.Column.FieldName == "SoLuongThucTe")
            {
                GridView view = sender as GridView;
                if (view == null) return;
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
                if (!ValidateKiemKe())
                {
                    return false;
                }
                if (tblNhapKho == null || tblNhapKho.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có kiện vật tư nào trong PI NCC. Vui lòng kiểm tra lại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (!checkTxt()) return false;
                if (tblPhieu == null || tblPhieu.Rows.Count == 0) CreateTablePhieu();
                if (tblPhieu.Rows.Count > 0) tblPhieu.Clear();
                DataRow dr = tblPhieu.NewRow();

                dr["ID"] = 0;
                dr["SoLoID"] = searchLookUpEditLoNL.EditValue.ToString();
                dr["SoLo"] = searchLookUpEditLoNL.Text.ToString();


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
                dr["Tau"] = SearchlookupTau.EditValue?.ToString();
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
                //dr["QCKiem"] = string.IsNullOrWhiteSpace(txtQCKiem.Text) ? (object)DBNull.Value : Convert.ToInt64(txtQCKiem.Text);
                dr["IsKiemKe"] = cbKiemKe.Checked;
                dr["PDF"] = btnPDF.Text;
                if (cbKiemKe.Checked)
                {
                    dr["NgayKiemKe"] = (DateTime)ngayKK.EditValue;
                }
                else
                {
                    dr["NgayKiemKe"] = DBNull.Value;
                }
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
                }
                var dsSoLuongSai = LayDanhSachSoLuongKhongKhop(tblNhapKhoPL, tblPL);
                if (dsSoLuongSai.Any())
                {
                    var danhSachSai = dsSoLuongSai
                        .Select(row => string.Format(
                            "Chủng loại: {0}, ItemCode VT: {1}, Mô tả: {2}, Màu: {3}, Khổ/Size: {4}\n" +
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
                //    .Select(row => string.Format(
                //        "Chủng loại: {0} ,Mã vật tư: {1} ,ItemCode: {2} ,Mô tả: {3}, Màu: {4}, Khổ/Size: {5}, Số Roll: {6}",
                //        row.TenNhom, row.MaVTGhep, row.MaVT, row.ChiTiet, row.MauVT, row.KhoVai, row.SoKienHienThi))
                //    .Distinct() // Loại bỏ trùng, vì 1 nhóm trùng có thể có >2 dòng giống hệt nhau
                //    .ToList();

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
                if (tblNhapKhoPL.Columns.Contains("TrangThai")) ;
                {
                    tblNhapKhoPL.Columns.Remove("TrangThai");
                }
                if (tblNhapKhoPL.Columns.Contains("QuyDoiID"))
                {
                    tblNhapKhoPL.Columns.Remove("QuyDoiID");
                }
                if (tblNhapKhoPL.Columns.Contains("IsNK"))
                {
                    tblNhapKhoPL.Columns.Remove("IsNK");
                }
                if (tblNhapKhoPL.Columns.Contains("STT"))
                {
                    tblNhapKhoPL.Columns.Remove("STT");
                }
                if (tblNhapKhoPL.Columns.Contains("IsIn"))
                {
                    tblNhapKhoPL.Columns.Remove("IsIn");
                }
                if (tblNhapKhoPL.Columns.Contains("NgayNhapKho"))
                {
                    tblNhapKhoPL.Columns.Remove("NgayNhapKho");
                }
                if (tblNhapKhoPL.Columns.Contains("Dot"))
                {
                    tblNhapKhoPL.Columns.Remove("Dot");
                }
                foreach (DataRow dr in tblNhapKhoPL.Rows)
                {

                    //dr["SoloID"] = searchLookUpEditLoPL.EditValue.ToString();
                    //dr["NgayNKDuKien"] = dateNgayNKPL.EditValue == null ? (object)DBNull.Value : dateNgayNKPL.EditValue;
                    if (string.IsNullOrWhiteSpace(dr["DonGia"].ToString()))
                    {
                        dr["DonGia"] = 0;
                    }
                    dr["POMua"] = txtPOMuaPL.Text.ToString();
                    dr["IsKiemKe"] = cbKiemKePL.Checked;
                    dr["NgayKiemKe"] = ngayKKPL.EditValue == null ? (object)DBNull.Value : (DateTime)ngayKKPL.EditValue;
                    dr["SoLoID"] = searchLookUpEditLoPL.EditValue.ToString();
                }
                string url = $"{URL}ERPNhapKhoNPL/Post?Action=POSTCT&para={GlobleData.UserName}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblNhapKhoPL); }).Result;
                if (msResult.ToLower() == "true")
                {
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
                if (!ValidateKiemKe())
                {
                    return false;
                }
                if (tblNhapKhoPL == null || tblNhapKhoPL.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có kiện vật tư nào trong PI NCC. Vui lòng kiểm tra lại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (!checkTxt()) return false;
                if (tblPhieuPL == null || tblPhieuPL.Rows.Count == 0) CreateTablePhieuPL();
                if (tblPhieuPL.Rows.Count > 0) tblPhieuPL.Clear();
                DataRow dr = tblPhieuPL.NewRow();

                dr["ID"] = 0;
                dr["SoLoID"] = searchLookUpEditLoPL.EditValue.ToString();
                dr["SoLo"] = searchLookUpEditLoPL.Text.ToString();
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
                //dr["QCKiem"] = string.IsNullOrWhiteSpace(txtQCKiemPL.Text) ? (object)DBNull.Value : Convert.ToInt64(txtQCKiemPL.Text);
                dr["IsKiemKe"] = cbKiemKePL.Checked;
                dr["PDF"] = btnPDFPL.Text;
                if (cbKiemKePL.Checked)
                {
                    dr["NgayKiemKe"] = (DateTime)ngayKKPL.EditValue;
                }
                else
                {
                    dr["NgayKiemKe"] = DBNull.Value;
                }
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
            // btnTauPL.Text = tentau;
        }

        private void btnQRCode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            this.ActiveControl = simpleButton1;

            if (_isNPL)
                XuLyQRCode_NPL();
            else
                XuLyQRCode_PL();

        }
        private void XuLyQRCode_NPL()
        {
            DataRow drMain = gVNL.GetFocusedDataRow();
            DataTable tblDetail = gCNLNhapKho.DataSource as DataTable;
            if (drMain == null || tblDetail == null || tblDetail.Rows.Count == 0) return;

            List<QrcodeVatTuEntity> lst = new List<QrcodeVatTuEntity>();

            string SoLo = searchLookUpEditLoNL.EditValue == null ? "" : searchLookUpEditLoNL.Text?.ToString();
            string POMua = txtPOMua.Text;

            var selectedRow = searchLookUpEditLoNL.Properties.GetRowByKeyValue(searchLookUpEditLoNL.EditValue);
            if (selectedRow is DataRowView drv)
                SoLo = drv["DisPlayQRCode"]?.ToString() ?? "";

            // Bước 1: Kiểm tra xem có dòng nào IsIn = true không
            bool hasIsInTrue = false;
            foreach (DataRow dr in tblDetail.Rows)
            {
                if (dr["IsIn"].ToString() == "True")
                {
                    hasIsInTrue = true;
                    break;
                }
            }

            // Bước 2: Lặp và áp dụng logic
            foreach (DataRow dr in tblDetail.Rows)
            {
                if (dr["Barcode"].ToString() == "" || dr["SoKienHienThi"].ToString() == "")
                    continue;

                if (hasIsInTrue && dr["IsIn"].ToString() == "False")
                    continue;

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
                itemQR.SoLuongThucTe = dr["tileNW"].ToString();
                itemQR.BarCode = dr["BarCode"].ToString();
                itemQR.TenNhom = drMain["MaVT"].ToString();
                itemQR.KhoVai = drMain["KhoVai"].ToString();
                itemQR.ChiTiet = $"{drMain["ChiTiet"].ToString()}";
                //itemQR.Para1 = dr["NgayNKDuKien"] != DBNull.Value
                //    ? Convert.ToDateTime(dateNgayNK.EditValue).ToString("dd/MM/yyyy")
                //    : DateTime.Now.ToString("dd/MM/yyyy");
                itemQR.Para1 = dr["NgayNhapKho"] != DBNull.Value
                        ? Convert.ToDateTime(dr["NgayNhapKho"]).ToString("dd/MM/yyyy")
                        : DateTime.Now.ToString("dd/MM/yyyy");

                itemQR.TenDVVT = drMain["TenDVVT"].ToString();

                itemQR.Para3 = searchLookUpEditKHCT.Text?.ToString();

                lst.Add(itemQR);
            }

            if (lst.Count == 0)
            {
                XtraMessageBox.Show("Không có vật tư để xuất QRCode.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            frmQRCodeViewer frm = new frmQRCodeViewer("QrcodeVT", lst, true);
            frm.ShowDialog();
        }

        //private void XuLyQRCode_PL()
        //{
        //    DataRow drMain = gVPL.GetFocusedDataRow();
        //    DataTable tblDetail = gCPLNhapKho.DataSource as DataTable;
        //    if (drMain == null || tblDetail == null || tblDetail.Rows.Count == 0) return;

        //    List<QrcodeVatTuEntity> lst = new List<QrcodeVatTuEntity>();

        //    string SoLo = searchLookUpEditLoPL.EditValue == null ? "" : searchLookUpEditLoPL.Text?.ToString();
        //    string POMua = txtPOMuaPL.Text;

        //    var selectedRow = searchLookUpEditLoPL.Properties.GetRowByKeyValue(searchLookUpEditLoPL.EditValue);
        //    if (selectedRow is DataRowView drv)
        //        SoLo = drv["DisPlayQRCode"]?.ToString() ?? "";

        //    // Bước 1: Kiểm tra xem có dòng nào IsIn = true không
        //    bool hasIsInTrue = false;
        //    foreach (DataRow dr in tblDetail.Rows)
        //    {
        //        if (dr["IsIn"].ToString() == "True")
        //        {
        //            hasIsInTrue = true;
        //            break;
        //        }
        //    }

        //    // Bước 2: Lặp và áp dụng logic
        //    foreach (DataRow dr in tblDetail.Rows)
        //    {
        //        if (dr["Barcode"].ToString() == "" || dr["SoKienHienThi"].ToString() == "")
        //            continue;

        //        if (hasIsInTrue && dr["IsIn"].ToString() == "False")
        //            continue;

        //        string maMauVT = dr["MaMauVT"]?.ToString() ?? "";
        //        string tenMauVT = dr["MauVT"]?.ToString() ?? "";
        //        string soLot = dr["SoLot"].ToString();
        //        string batch = dr["Batch"].ToString();

        //        QrcodeVatTuEntity itemQR = new QrcodeVatTuEntity();
        //        itemQR.SoKien = dr["SoKienHienThi"].ToString();
        //        itemQR.IsKK = dr["IsKiemKe"] != DBNull.Value && Convert.ToBoolean(dr["IsKiemKe"]);
        //        string _solotBatch = soLot + (string.IsNullOrEmpty(batch) ? "" : $"/{batch}");
        //        itemQR.SoLot = _solotBatch;
        //        itemQR.GhiChu = POMua;
        //        itemQR.MauVT = $"{maMauVT}/{tenMauVT}";
        //        itemQR.SoLuongThucTe = dr["SoGhiDauCay"].ToString();
        //        itemQR.BarCode = dr["BarCode"].ToString();
        //        itemQR.TenNhom = drMain["MaVT"].ToString();
        //        itemQR.KhoVai = drMain["KhoVai"].ToString();
        //        itemQR.ChiTiet = $"{drMain["ChiTiet"].ToString()}";
        //        itemQR.Para1 = dr["NgayNKDuKien"] != DBNull.Value
        //            ? Convert.ToDateTime(dr["NgayNKDuKien"]).ToString("dd/MM/yyyy")
        //            : DateTime.Now.ToString("dd/MM/yyyy");
        //        itemQR.Para2 = dr["NgayKiemKe"] != DBNull.Value
        //            ? Convert.ToDateTime(dr["NgayKiemKe"]).ToString("dd/MM/yyyy")
        //            : DateTime.Now.ToString("dd/MM/yyyy");
        //        itemQR.TenDVVT = drMain["TenDVVT"].ToString();

        //        lst.Add(itemQR);
        //    }

        //    if (lst.Count == 0)
        //    {
        //        XtraMessageBox.Show("Không có vật tư để xuất QRCode.", "Cảnh báo",
        //            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    frmQRCodeViewer frm = new frmQRCodeViewer("QrcodeVT", lst, false);
        //    frm.ShowDialog();
        //}
        private bool IsTrue(object v)
        {
            if (v == null || v == DBNull.Value) return false;
            if (v is bool b) return b;
            var s = v.ToString();
            return s.Equals("True", StringComparison.OrdinalIgnoreCase) || s == "1";
        }

        private void XuLyQRCode_PL()
        {
            if (gVPL.DataRowCount == 0) return;

            List<QrcodeVatTuEntity> lst = new List<QrcodeVatTuEntity>();

            string POMua = txtPOMuaPL.Text;

            string SoLo = searchLookUpEditLoPL.EditValue == null ? "" : searchLookUpEditLoPL.Text;
            var selectedRow = searchLookUpEditLoPL.Properties.GetRowByKeyValue(searchLookUpEditLoPL.EditValue);
            if (selectedRow is DataRowView drv)
                SoLo = drv["DisPlayQRCode"]?.ToString() ?? "";

            // Lấy tất cả rowHandle master có IsIn = true
            List<int> handles = new List<int>();
            for (int i = 0; i < gVPL.DataRowCount; i++)
            {
                var vIsIn = gVPL.GetRowCellValue(i, "IsIn");
                if (IsTrue(vIsIn))
                    handles.Add(i);
            }

            // Nếu không có dòng nào được check, lấy dòng đang focus
            if (handles.Count == 0)
            {
                int focusedHandle = gVPL.FocusedRowHandle;
                if (focusedHandle >= 0)
                {
                    handles.Add(focusedHandle);
                }
                else
                {
                    XtraMessageBox.Show("Không có dòng nào được chọn.", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            foreach (int handle in handles)
            {
                // set focus để detail (gCPLNhapKho) đổi theo dòng master này
                gVPL.FocusedRowHandle = handle;
                gVPL.UpdateCurrentRow();

                DataRow drMain = gVPL.GetFocusedDataRow();
                DataTable tblDetail = gCPLNhapKho.DataSource as DataTable;

                if (drMain == null || tblDetail == null || tblDetail.Rows.Count == 0)
                    continue;

                // --- giữ nguyên logic cũ của bạn, nhưng chạy theo từng drMain ---
                bool hasIsInTrue = false;
                foreach (DataRow dr in tblDetail.Rows)
                {
                    if (IsTrue(dr["IsIn"]))
                    {
                        hasIsInTrue = true;
                        break;
                    }
                }

                foreach (DataRow dr in tblDetail.Rows)
                {
                    if (string.IsNullOrWhiteSpace(dr["Barcode"]?.ToString()) ||
                        string.IsNullOrWhiteSpace(dr["SoKienHienThi"]?.ToString()))
                        continue;

                    if (hasIsInTrue && !IsTrue(dr["IsIn"]))
                        continue;

                    string maMauVT = dr["MaMauVT"]?.ToString() ?? "";
                    string tenMauVT = dr["MauVT"]?.ToString() ?? "";
                    string soLot = dr["SoLot"]?.ToString() ?? "";
                    string batch = dr["Batch"]?.ToString() ?? "";

                    QrcodeVatTuEntity itemQR = new QrcodeVatTuEntity();
                    itemQR.SoKien = dr["SoKienHienThi"]?.ToString();
                    itemQR.IsKK = IsTrue(dr["IsKiemKe"]);

                    string _solotBatch = soLot + (string.IsNullOrEmpty(batch) ? "" : $"/{batch}");
                    itemQR.SoLot = _solotBatch;

                    itemQR.GhiChu = POMua;
                    itemQR.MauVT = $"{maMauVT}/{tenMauVT}";
                    itemQR.SoLuongThucTe = dr["tileNW"]?.ToString();
                    itemQR.BarCode = dr["BarCode"]?.ToString();
                    itemQR.TenNhom = drMain["MaVT"]?.ToString();
                    itemQR.KhoVai = drMain["KhoVai"]?.ToString();
                    itemQR.ChiTiet = $"{drMain["ChiTiet"]}";

                    //itemQR.Para1 = dr["NgayNKDuKien"] != DBNull.Value
                    //    ? Convert.ToDateTime(dateNgayNKPL.EditValue).ToString("dd/MM/yyyy")
                    //    : DateTime.Now.ToString("dd/MM/yyyy");

                    itemQR.Para1 = dr["NgayNhapKho"] != DBNull.Value
                     ? Convert.ToDateTime(dr["NgayNhapKho"]).ToString("dd/MM/yyyy")
                     : DateTime.Now.ToString("dd/MM/yyyy");

                  
                    itemQR.TenDVVT = drMain["TenDVVT"].ToString();

                    itemQR.Para3 = searchLookUpEditKHCTPL.Text?.ToString();

                    lst.Add(itemQR);
                }
            }

            if (lst.Count == 0)
            {
                XtraMessageBox.Show("Không có vật tư để xuất QRCode.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            frmQRCodeViewer frm = new frmQRCodeViewer("QrcodeVT", lst, false);
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

            }
        }
        public void Export(string TemplateFileName, string ExportFileName)
        {
            try
            {
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                FileInfo templateFile = new FileInfo(TemplateFileName);
                FileInfo resultFile = new FileInfo(ExportFileName);

                using (ExcelPackage templatePackage = new ExcelPackage(templateFile))
                {
                    templatePackage.Workbook.Properties.Author = "NTB";
                    templatePackage.Workbook.Properties.Title = "NhapKhoNPL";
                    templatePackage.SaveAs(resultFile);
                }

                DataTable tbl = (_isNPL ? gCNL : gCPL).DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0)
                    return;

                using (ExcelPackage excelPackage = new ExcelPackage(resultFile))
                {
                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    int startRowInExcel = 2;

                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {
                        DataRow row = tbl.Rows[i];
                        int excelRow = startRowInExcel + i;

                        worksheet.Cells[excelRow, 1].Value = row["TenNhom"];
                        worksheet.Cells[excelRow, 2].Value = row["MaVT"];
                        worksheet.Cells[excelRow, 3].Value = row["ChiTiet"];
                        worksheet.Cells[excelRow, 4].Value = row["MauVT"];
                        worksheet.Cells[excelRow, 5].Value = row["KhoVai"];
                    }

                    excelPackage.Save();
                }
            }
            catch (Exception ex)
            {
            }
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
            if (info.Column == gridColumn31)
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
            if (info.Column == gridColumn54)
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

        private void gVSearchNL_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn44)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

        }





        private void btnExcel_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            btnExcelImport();

        }
        #region excel_TongHop_ChiTiet_CayVai
        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataTable tbl;
                if (_isNPL)
                {
                    if (tblNhapKho == null) return;

                    tbl = tblNhapKho;
                }
                else
                {
                    if (tblNhapKhoPL == null) return;
                    tbl = tblNhapKhoPL;
                }

                SaveFileDialog Sfd = new SaveFileDialog();
                Sfd.Title = "File To Save";
                Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
                Sfd.FileName = string.Format("TongHopNhapKho_{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
                if (Sfd.ShowDialog() == DialogResult.OK)
                {
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                    string fileName = "KhoiTaoPhieuNhapKhoNPLTemplate.xlsx";
                    string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                    string TemplateFileName = path;
                    string ExportFileName = Sfd.FileName;
                    ExportTH(TemplateFileName, ExportFileName, tbl);

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
            catch (Exception ex)
            {
            }
        }


        public void ExportTH(string TemplateFileName, string ExportFileName, DataTable tbl)
        {

            try
            {
                string urlKHcheck = $"{URL}ERPThuVienNK/Get?Action=GETKH";
                string jsonKHcheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKHcheck); }).Result;
                DataTable tblkhcheck = JsonConvert.DeserializeObject<DataTable>(jsonKHcheck);

                string urlNCCcheck = $"{URL}ERPThuVienNK/Get?Action=GETNCC";
                string jsonNCCcheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNCCcheck); }).Result;
                DataTable tblncccheck = JsonConvert.DeserializeObject<DataTable>(jsonNCCcheck);

                var rowkh = tblkhcheck.AsEnumerable().FirstOrDefault(r => r["MaKH"].ToString() == "");
                var rowncc = tblncccheck.AsEnumerable().FirstOrDefault(r => r["MaKH"].ToString() == tblTxt.Rows[0]["NhaCungCap"].ToString());
                DataTable tblct = tblChiTiet;
                FileInfo file = new FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    excelPackage.Workbook.Properties.Author = "Cty NTB";
                    excelPackage.Workbook.Properties.Title = "";

                    string[] dateTime = DateTime.Now.ToString("dd/MM/yyyy").Split('/');

                    FileInfo templateFile = new FileInfo(TemplateFileName);
                    int sheetIndex = 0;

                    using (ExcelPackage templatePackage = new ExcelPackage(templateFile))
                    {
                        sheetIndex++;


                        string sheetName = tblct.Rows[0]["MaVT"].ToString();
                        ExcelWorksheet templateSheetTH = templatePackage.Workbook.Worksheets["Sheet1"];
                        ExcelWorksheet newSheetTH = excelPackage.Workbook.Worksheets.Add(sheetName, templateSheetTH);
                        //newSheetTH.Cells[1, 1].Value = searchLookUpEditKH.EditValue?.ToString();

                        newSheetTH.Cells[1, 1].Value = "";
                        newSheetTH.Cells[1, 1].Style.Font.Size = 18;
                        newSheetTH.Cells[1, 1].Style.Font.Bold = true;
                        newSheetTH.Cells[2, 1].Value = rowkh != null ? rowkh["DiaChi"].ToString() : "";
                        newSheetTH.Cells[7, 1].Value = tblTxt.Rows[0]["NguoiGui"].ToString();

                        string dcnoigui = tblTxt.Rows[0]["NoiGui"].ToString();
                        int maxlength = 50;
                        int currentRownoigui = 8;
                        if (string.IsNullOrEmpty(dcnoigui))
                        {
                            newSheetTH.Cells[currentRownoigui, 1].Value = "";
                        }
                        else if (dcnoigui.Length <= maxlength)
                        {
                            newSheetTH.Cells[currentRownoigui, 1].Value = dcnoigui;
                        }
                        else
                        {
                            int startIndex = 0;
                            while (startIndex < dcnoigui.Length)
                            {
                                int length = Math.Min(maxlength, dcnoigui.Length - startIndex);
                                string substring = dcnoigui.Substring(startIndex, length);


                                int splitIndex = substring.LastIndexOf(',');
                                if (splitIndex == -1 || splitIndex == 0)
                                    splitIndex = length;

                                string part = dcnoigui.Substring(startIndex, splitIndex).Trim();
                                newSheetTH.Cells[currentRownoigui, 1].Value = part;

                                startIndex += splitIndex + (splitIndex < length ? 1 : 0);
                                currentRownoigui++;
                            }
                        }
                        //newSheetTH.Cells[8, 1].Value = tblTxt.Rows[0]["NoiGui"].ToString();
                        newSheetTH.Cells[13, 1].Value = "";

                        string dc = rowkh != null ? rowkh["DiaChi"].ToString() : "";
                        int currentRow = 14;
                        if (string.IsNullOrEmpty(dc))
                        {
                            newSheetTH.Cells[currentRow, 1].Value = "";
                        }
                        else if (dc.Length <= maxlength)
                        {
                            newSheetTH.Cells[currentRow, 1].Value = dc;
                        }
                        else
                        {
                            int startIndex = 0;
                            while (startIndex < dc.Length)
                            {
                                int length = Math.Min(maxlength, dc.Length - startIndex);
                                string substring = dc.Substring(startIndex, length);


                                int splitIndex = substring.LastIndexOf(',');
                                if (splitIndex == -1 || splitIndex == 0)
                                    splitIndex = length;

                                string part = dc.Substring(startIndex, splitIndex).Trim();
                                newSheetTH.Cells[currentRow, 1].Value = part;

                                startIndex += splitIndex + (splitIndex < length ? 1 : 0);
                                currentRow++;
                            }
                        }

                        //newSheetTH.Cells[14, 1].Value = rowkh != null ? rowkh["DiaChi"].ToString() : "";

                        newSheetTH.Cells[6, 9].Value = "DATE:" + (tblTxt.Rows[0]["NgayGui"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayGui"]) : null);
                        newSheetTH.Cells[7, 9].Value = "INVOICE NO:" + tblTxt.Rows[0]["SoHoaDon"].ToString();
                        newSheetTH.Cells[8, 9].Value = "SHIPPED PER S.S.:" + tblTxt.Rows[0]["Tau"].ToString();
                        newSheetTH.Cells[9, 9].Value = "'SAILING ON / ABOUT :" + (tblTxt.Rows[0]["NgayNKDuKien"] != DBNull.Value ? (object)Convert.ToDateTime(tblTxt.Rows[0]["NgayNKDuKien"]) : null);
                        newSheetTH.Cells[10, 9].Value = "FROM:" + tblTxt.Rows[0]["NoiGui"].ToString();
                        newSheetTH.Cells[11, 9].Value = "TO:" + tblTxt.Rows[0]["DiaChiNhan"].ToString();
                        newSheetTH.Cells[13, 9].Value = searchLookUpEditNCC.Text;

                        string dcncc = rowncc != null ? rowncc["DiaChi"].ToString() : "";
                        int currentRowncc = 14;
                        if (string.IsNullOrEmpty(dcncc))
                        {
                            newSheetTH.Cells[currentRowncc, 9].Value = "";
                        }
                        else if (dcncc.Length <= maxlength)
                        {
                            newSheetTH.Cells[currentRowncc, 9].Value = dcncc;
                        }
                        else
                        {
                            int startIndex = 0;
                            while (startIndex < dcncc.Length)
                            {
                                int length = Math.Min(maxlength, dcncc.Length - startIndex);
                                string substring = dcncc.Substring(startIndex, length);


                                int splitIndex = substring.LastIndexOf(',');
                                if (splitIndex == -1 || splitIndex == 0)
                                    splitIndex = length;

                                string part = dcncc.Substring(startIndex, splitIndex).Trim();
                                newSheetTH.Cells[currentRowncc, 9].Value = part;

                                startIndex += splitIndex + (splitIndex < length ? 1 : 0);
                                currentRowncc++;
                            }
                        }

                        //newSheetTH.Cells[14, 9].Value = rowncc != null ? rowncc["DiaChi"].ToString():"";
                        newSheetTH.Cells[16, 9].Value = "TAX CODE:" + (rowncc != null ? rowncc["MaSoThue"].ToString() : "");
                        newSheetTH.Cells[17, 9].Value = "TEL:" + (rowncc != null ? rowncc["SoDienThoai"].ToString() : "");


                        int row = 18;

                        var groupMaVTs = tblct.AsEnumerable()
                            .GroupBy(r => new
                            {
                                MaNPL = r["MaNPL"].ToString(),
                                MaVTID = r["MaVTID"].ToString(),
                                MauVTID = r["MauVTID"].ToString(),
                                MaVT = r["MaVT"].ToString(),
                                MaNhom = r["MaNhom"].ToString(),
                                KhoVaiID = r["KhoVaiID"].ToString(),
                                TenDVCD = r["TenDVCD"].ToString(),
                                KhoVai = r["KhoVai"].ToString()
                            });

                        foreach (var group in groupMaVTs)
                        {
                            string mavt = group.Key.MaVT;
                            string dv = group.Key.TenDVCD;
                            string khovai = group.Key.KhoVai;

                            var headerCell = newSheetTH.Cells[row, 1];
                            headerCell.Value = $"{mavt}-{khovai}";
                            headerCell.Style.Font.Bold = true;
                            headerCell.Style.Font.Color.SetColor(Color.White);
                            headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            headerCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 0, 0));
                            headerCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            newSheetTH.Cells[row, 1, row, 3].Merge = true;
                            row++;

                            int maxRowsPerBlock = 31;
                            int rowInBlock = 0;
                            int colOffset = 0;
                            int blocksPerRow = 2;
                            int blockIndex = 0;

                            int startRowForGroup = row;
                            int maxRowUsedInGroup = row;
                            int startRowForBlock = row;

                            int rollCountInBlock = 0;
                            decimal totalSoGhiDauInBlock = 0;
                            decimal totalNWInBlock = 0;
                            decimal totalGWInBlock = 0;

                            string[] subHeaders = { "R/NO", "COLOR", "LOT NO", "QTY", "", "NW", "GW" };

                            // Vẽ header ban đầu
                            for (int i = 0; i < subHeaders.Length; i++)
                            {
                                var cell = newSheetTH.Cells[row, i + 1];
                                cell.Value = subHeaders[i];
                                cell.Style.Font.Bold = true;
                                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }
                            row++;
                            startRowForGroup = row;

                            var chiTietList = tbl.AsEnumerable().Where(r =>
                                r["MaVTID"].ToString() == group.Key.MaVTID &&
                                r["MauVTID"].ToString() == group.Key.MauVTID &&
                                r["KhoVaiID"].ToString() == group.Key.KhoVaiID
                            //&& r["MaNhom"].ToString() == group.Key.MaNhom
                            );



                            foreach (var item in chiTietList)
                            {

                                if (rowInBlock >= maxRowsPerBlock)
                                {
                                    // Ghi dòng tổng cho block hiện tại

                                    newSheetTH.Cells[row, colOffset + 1].Value = $"{rollCountInBlock} \nROLLS";
                                    newSheetTH.Cells[row, colOffset + 1].Style.WrapText = true;
                                    newSheetTH.Cells[row, colOffset + 4].Value = $"{totalSoGhiDauInBlock:0.##} \n{dv}";
                                    newSheetTH.Cells[row, colOffset + 4].Style.WrapText = true;
                                    newSheetTH.Cells[row, colOffset + 6].Value = $"{totalNWInBlock:0.##} \nKGS";
                                    newSheetTH.Cells[row, colOffset + 6].Style.WrapText = true;
                                    newSheetTH.Cells[row, colOffset + 7].Value = $"{totalGWInBlock:0.##} \nKGS";
                                    newSheetTH.Cells[row, colOffset + 7].Style.WrapText = true;

                                    for (int i = 1; i <= 7; i++)
                                    {
                                        var cell = newSheetTH.Cells[row, colOffset + i];
                                        cell.Style.Font.Bold = true;
                                        cell.Style.Font.Color.SetColor(Color.DarkBlue);
                                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    }
                                    row++;

                                    rowInBlock = 0;

                                    rollCountInBlock = 0;
                                    totalSoGhiDauInBlock = 0;
                                    totalNWInBlock = 0;
                                    totalGWInBlock = 0;

                                    blockIndex++;

                                    if (blockIndex % blocksPerRow == 0)
                                    {
                                        // xuống dòng
                                        row = startRowForBlock + maxRowsPerBlock + 3;
                                        startRowForBlock = row;
                                        colOffset = 0;
                                    }
                                    else
                                    {
                                        // bảng bên phải
                                        row = startRowForBlock;
                                        colOffset += 8;
                                    }

                                    // Vẽ lại header con
                                    for (int i = 0; i < subHeaders.Length; i++)
                                    {
                                        var cell = newSheetTH.Cells[row, colOffset + i + 1];
                                        cell.Value = subHeaders[i];
                                        cell.Style.Font.Bold = true;
                                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        cell.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    }
                                    row++;
                                }


                                newSheetTH.Cells[row, colOffset + 1].Value = item["SoKienHienThi"].ToString();
                                newSheetTH.Cells[row, colOffset + 2].Value = item["MauVT"].ToString();
                                newSheetTH.Cells[row, colOffset + 3].Value = item["SoLot"].ToString();
                                newSheetTH.Cells[row, colOffset + 4].Value = item["SoGhiDauCay"].ToString();
                                newSheetTH.Cells[row, colOffset + 5].Value = "";
                                newSheetTH.Cells[row, colOffset + 6].Value = item["NW"].ToString();
                                newSheetTH.Cells[row, colOffset + 7].Value = item["GW"].ToString();

                                for (int i = 1; i <= 7; i++)
                                {
                                    var cell = newSheetTH.Cells[row, colOffset + i];
                                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                }

                                // Cộng dồn tổng
                                rollCountInBlock++;
                                decimal.TryParse(item["SoGhiDauCay"].ToString(), out decimal soghi);
                                decimal.TryParse(item["NW"].ToString(), out decimal nw);
                                decimal.TryParse(item["GW"].ToString(), out decimal gw);

                                totalSoGhiDauInBlock += soghi;
                                totalNWInBlock += nw;
                                totalGWInBlock += gw;

                                row++;
                                rowInBlock++;

                                // Cập nhật dòng cao nhất được sử dụng
                                if (row > maxRowUsedInGroup)
                                    maxRowUsedInGroup = row;

                            }

                            // Nếu block cuối cùng chưa đủ 31 dòng → chèn dòng trống
                            while (rowInBlock < maxRowsPerBlock && rowInBlock > 0)
                            {
                                for (int i = 1; i <= 7; i++)
                                {
                                    var cell = newSheetTH.Cells[row, colOffset + i];
                                    cell.Value = "";
                                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                }
                                row++;
                                rowInBlock++;
                                if (row > maxRowUsedInGroup)
                                    maxRowUsedInGroup = row;
                            }

                            if (rowInBlock > 0)
                            {
                                newSheetTH.Cells[row, colOffset + 1].Value = $"{rollCountInBlock} \nROLLS";
                                newSheetTH.Cells[row, colOffset + 1].Style.WrapText = true;
                                newSheetTH.Cells[row, colOffset + 4].Value = $"{totalSoGhiDauInBlock:0.##} \n{dv}";
                                newSheetTH.Cells[row, colOffset + 4].Style.WrapText = true;
                                newSheetTH.Cells[row, colOffset + 6].Value = $"{totalNWInBlock:0.##} \nKGS";
                                newSheetTH.Cells[row, colOffset + 6].Style.WrapText = true;
                                newSheetTH.Cells[row, colOffset + 7].Value = $"{totalGWInBlock:0.##} \nKGS";
                                newSheetTH.Cells[row, colOffset + 7].Style.WrapText = true;

                                var cellsWithBorder = new[] { colOffset + 1, colOffset + 6, colOffset + 7 };
                                foreach (var col in cellsWithBorder)
                                {
                                    var cell = newSheetTH.Cells[row, col];
                                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                }

                                for (int i = 1; i <= 7; i++)
                                {
                                    var cell = newSheetTH.Cells[row, colOffset + i];
                                    cell.Style.Font.Bold = true;
                                    cell.Style.Font.Color.SetColor(Color.DarkBlue);
                                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                }

                                row++;
                            }

                            // Cách 2 dòng cho cụm kế tiếp
                            row = maxRowUsedInGroup + 2;
                        }

                    }
                    excelPackage.SaveAs(file);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xuất Excel: {ex.Message}");

            }
        }
        #endregion


        private void btnExcelPL_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btnExcelImport();
        }
        private void btnExcelImport()
        {
            DataTable dt = (_isNPL ? gCNL : gCPL).DataSource as DataTable;
            if (dt == null | dt.Rows.Count == 0) return;
            DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
            if (dr == null) return;


            /*if (string.IsNullOrWhiteSpace(dr["DonGia"].ToString()) || dr["DonGia"].ToString() == "0")
             //{
             //    MessageBox.Show("Vui lòng nhập đơn giá.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
             //    return;
             //}*/
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
            ImportExcelToNhapKho(tbl);


        }
        private void ImportExcelToNhapKho(DataTable tblExcel)
        {
            if (tblExcel == null || tblExcel.Rows.Count == 0)
                return;

            DataTable dtVT = (_isNPL ? gCNL : gCPL).DataSource as DataTable;
            if (dtVT == null || dtVT.Rows.Count == 0)
                return;

            DataTable tblNKRoot = _isNPL ? tblNhapKho : tblNhapKhoPL;
            DataTable dtNKSource = (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource as DataTable;

            decimal soKienMax = 0;
            if (tblNKRoot.Rows.Count > 0)
            {
                soKienMax = tblNKRoot.AsEnumerable()
                    .Where(r => decimal.TryParse(Convert.ToString(r["SoKien"]), out _))
                    .Select(r => Convert.ToDecimal(r["SoKien"]))
                    .DefaultIfEmpty(0)
                    .Max();
            }

            List<string> duplicates = new List<string>();

            foreach (DataRow excelRow in tblExcel.Rows)
            {
                string tenNhom = excelRow[0].ToString().Trim();
                string maVT = excelRow[1].ToString().Trim();
                string moTa = excelRow[2].ToString().Trim();
                string mauVT = excelRow[3].ToString().Trim();
                string khoVai = excelRow[4].ToString().Trim();

                DataRow vtRow = dtVT.AsEnumerable()
                    .FirstOrDefault(r =>
                        r["TenNhom"].ToString().Trim() == tenNhom &&
                        r["MaVT"].ToString().Trim() == maVT &&
                        r["ChiTiet"].ToString().Trim() == moTa &&
                        r["MauVT"].ToString().Trim() == mauVT &&
                        r["KhoVai"].ToString().Trim() == khoVai
                    );

                if (vtRow == null)
                    continue;

                // Kiểm tra trùng lặp Roll, Lot, Batch
                string soKienHienThi = excelRow[5].ToString().Trim();
                string batch = excelRow[6].ToString().Trim();
                string soLot = excelRow[7].ToString().Trim();

                var existingRow = tblNKRoot.AsEnumerable()
                    .FirstOrDefault(r =>
                        r["MaVTID"].ToString() == vtRow["MaVTID"].ToString() &&
                        r["MaNhom"].ToString() == vtRow["MaNhom"].ToString() &&
                        r["MauVTID"].ToString() == vtRow["MauVTID"].ToString() &&
                        r["KhoVaiID"].ToString() == vtRow["KhoVaiID"].ToString() &&
                        r["SoKienHienThi"].ToString().Trim() == soKienHienThi &&
                        r["Batch"].ToString().Trim() == batch &&
                        r["SoLot"].ToString().Trim() == soLot
                    );

                if (existingRow != null)
                {
                    duplicates.Add($"Roll: {soKienHienThi}, Lot: {soLot}, Batch: {batch}");
                    continue; // Bỏ qua dòng trùng, không insert
                }

                int tongKien = vtRow["TongKien"] == DBNull.Value ? 0 : Convert.ToInt32(vtRow["TongKien"]);
                vtRow["TongKien"] = tongKien + 1;

                soKienMax++;

                DataRow nk = tblNKRoot.NewRow();

                nk["ID"] = 0;
                nk["SoKien"] = soKienMax;
                nk["SoKienHienThi"] = soKienHienThi;
                nk["Batch"] = batch;
                nk["SoLot"] = soLot;
                nk["SoGhiDauCay"] = excelRow[8];
                nk["GhiChu"] = excelRow[9].ToString().Trim();

                nk["MaNPL"] = RemoveVietnameseTone(
                    vtRow["MaNhom"] + "@" +
                    vtRow["MaVTID"] + "@" +
                    vtRow["MauVTID"] + "@" +
                    vtRow["KhoVaiID"]
                );

                nk["MaNhom"] = vtRow["MaNhom"];
                nk["MaVTGhep"] = vtRow["MaVTGhep"];
                nk["MaVTID"] = vtRow["MaVTID"];
                nk["MauVTID"] = vtRow["MauVTID"];
                nk["MaMauVT"] = vtRow["MaMauVT"];
                nk["MauVT"] = vtRow["MauVT"];
                nk["KhoVaiID"] = vtRow["KhoVaiID"];
                nk["MaDVVT"] = vtRow["MaDVVT"];
                nk["MaDVCD"] = vtRow["MaDVVT"];
                nk["TenDVCD"] = vtRow["TenDVVT"];
                nk["TienTe"] = vtRow["TienTe"];
                nk["DonGia"] = vtRow["DonGia"];
                nk["ThanhTien"] = 0;
                nk["SoLuongThucTe"] = 0;
                nk["NW"] = 0;
                nk["GW"] = 0;
                nk["BarCode"] = "";
                nk["Pallet"] = "";
                nk["SoKienParent"] = "";
                nk["IsNPL"] = _isNPL;
                nk["STTChonVT"] = vtRow["STTChonVT"];

                nk["BarCode"] = vtRow["STTChonVT"] + "|" + soKienHienThi;
                tblNKRoot.Rows.Add(nk);

            }

            // Hiển thị thông báo trùng lặp nếu có
            if (duplicates.Count > 0)
            {
                string detail;
                if (duplicates.Count > 5)
                {
                    detail = string.Join("\n", duplicates.Take(5)) +
                             $"\n... và {duplicates.Count - 5} dòng nữa";
                }
                else
                {
                    detail = string.Join("\n", duplicates);
                }

                string message = "Các dòng bị trùng đã bỏ qua:\n" + detail;
                MessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Tính lại SLTong cho vtRow nếu SLTong == 0 hoặc rỗng
            foreach (DataRow vtRow in dtVT.Rows)
            {
                decimal currentSLTong = 0;
                bool isEmpty = vtRow["SLTong"] == DBNull.Value ||
                               !decimal.TryParse(vtRow["SLTong"].ToString(), out currentSLTong);

                if (isEmpty || currentSLTong == 0)
                {
                    decimal sumSoGhiDauCay = tblNKRoot.AsEnumerable()
                        .Where(r =>
                            r["MaVTID"].ToString() == vtRow["MaVTID"].ToString() &&
                            r["MaNhom"].ToString() == vtRow["MaNhom"].ToString() &&
                            r["MauVTID"].ToString() == vtRow["MauVTID"].ToString() &&
                            r["KhoVaiID"].ToString() == vtRow["KhoVaiID"].ToString() &&
                            r["SoGhiDauCay"] != DBNull.Value &&
                            decimal.TryParse(r["SoGhiDauCay"].ToString(), out _)
                        )
                        .Sum(r => Convert.ToDecimal(r["SoGhiDauCay"]));

                    vtRow["SLTong"] = sumSoGhiDauCay;
                }
            }
            DataRow drCurrent = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
            if (drCurrent == null)
                return;

            string maVTID = drCurrent["MaVTID"].ToString();
            string mauVTID = drCurrent["MauVTID"].ToString();
            string khoVaiID = drCurrent["KhoVaiID"].ToString();
            string maNhom = drCurrent["MaNhom"].ToString();
            string maVTGhep = drCurrent["MaVTGhep"].ToString();

            DataTable tblNhapKhoFiltered = tblNKRoot.Clone();

            var filteredRows = tblNKRoot.AsEnumerable()
                .Where(r =>
                    r["MaVTID"].ToString() == maVTID &&
                    r["MauVTID"].ToString() == mauVTID &&
                    r["KhoVaiID"].ToString() == khoVaiID &&
                    r["MaNhom"].ToString() == maNhom &&
                    r["MaVTGhep"].ToString() == maVTGhep
                );

            foreach (var r in filteredRows)
                tblNhapKhoFiltered.ImportRow(r);

            (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource = tblNhapKhoFiltered;
            (_isNPL ? gVNL : gVPL).RefreshData();
        }
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
            int count = tbl.AsEnumerable()
        .Select(r => r.Field<string>("BarCode"))
        .Where(value => !string.IsNullOrWhiteSpace(value))
        .Select(value => value.Split('|'))
        .Count(parts => parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[parts.Length - 1]));
            dr["TongKien"] = count;
            dr["TongKien"] = count;

        }
        /*
        private void btnThemKien_Click(object sender, EventArgs e)
        {
            ThemKien();
        }
        private void btnThemKienPL_Click(object sender, EventArgs e)
        {
            ThemKien();
        }
        private void ThemKien()
        {
            GridView gV = (_isNPL ? gVNLNhapKho : gVPLNhapKho);
            if (tblNhapKho == null || tblNhapKho.Rows.Count == 0) return;
            DataTable tblNK = (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource as DataTable;
            if (tblNK == null || tblNK.Rows.Count == 0) return;
            DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
            if (dr == null) return;

            DataRow _r = (_isNPL ? gVNLNhapKho : gVPLNhapKho).GetFocusedDataRow();
            if (_r == null) return;


            decimal _sk = Convert.ToDecimal(_txtTuKien.Text.ToString());
            if (string.IsNullOrWhiteSpace(_txtTuKien.Text))
            {
                XtraMessageBox.Show("Vui lòng nhập Từ kiện.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(_txtDenKien.Text))
            {
                XtraMessageBox.Show("Vui lòng nhập Đến kiện.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int startIndex = Convert.ToInt32(_txtTuKien.Text) - 1;
            int endIndex = Convert.ToInt32(_txtDenKien.Text) - 1;

            for (int i = startIndex; i <= endIndex; i++)
            {
                if (i >= 0 && i < gV.RowCount)
                {
                    string newValue = _txtTienTo.Text + _sk.ToString();
                    gV.SetRowCellValue(i, "SoKienHienThi", newValue);
                    _sk++;
                }
            }


        }*/
        private void cbKiemKe_CheckedChanged(object sender, EventArgs e)
        {
            if (cbKiemKe.Checked)
            {
                layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            else
            {
                layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                ngayKK.EditValue = DBNull.Value;
            }
        }
        private void cbKiemKePL_CheckedChanged(object sender, EventArgs e)
        {
            if (cbKiemKePL.Checked)
            {
                layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            else
            {
                layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                ngayKKPL.EditValue = DBNull.Value;
            }
        }
        private bool ValidateKiemKe()
        {
            if (cbKiemKe.Checked && (ngayKK.EditValue == null || ngayKK.EditValue == DBNull.Value))
            {
                MessageBox.Show("Vui lòng nhập ngày kiểm kê!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ngayKK.Focus();
                return false;
            }

            if (cbKiemKePL.Checked && (ngayKKPL.EditValue == null || ngayKKPL.EditValue == DBNull.Value))
            {
                MessageBox.Show("Vui lòng nhập ngày kiểm kê!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ngayKKPL.Focus();
                return false;
            }

            return true;
        }
        #region Thoai them btn Chọn vật tư
        private void btnChonVT_Click(object sender, EventArgs e)
        {
            if (tblNhapKho == null)
                CreateTableNhapKho();
            DataTable tbl = gCNL.DataSource as DataTable;
            if (tbl == null)
            {
                tbl = CreateTableVatTu();
                gCNL.DataSource = tbl;
            }
            if (!tbl.Columns.Contains("STTChonVT"))
            {
                tbl.Columns.Add("STTChonVT", typeof(int));
            }
            foreach (DataRow drTbl in tbl.Rows)
            {
                var matchingRows = tblNhapKho.AsEnumerable()
                    .Where(r => r["MaVTID"].ToString() == drTbl["MaVTID"].ToString() &&
                               r["MauVTID"].ToString() == drTbl["MauVTID"].ToString() &&
                               r["MaNhom"].ToString() == drTbl["MaNhom"].ToString() &&
                               r["KhoVaiID"].ToString() == drTbl["KhoVaiID"].ToString())
                    .Where(r => r["STTChonVT"] != DBNull.Value);

                if (matchingRows.Any())
                {
                    var minSTT = matchingRows.Min(r => Convert.ToInt32(r["STTChonVT"]));
                    drTbl["STTChonVT"] = minSTT;
                }
            }

            frmERPNhapKhoNPL_ChonVT frm = new frmERPNhapKhoNPL_ChonVT(tbl, true);
            frm.IsEditMode = true;
            frm.OriginalCheckedKeys = _originalKeysFromDB_NPL;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.OnCheck = (row) =>
            {
                AddRowGCNL(row);
                string key = $"{row["MaVTID"]}_{row["MauVTID"]}_{row["MaNhom"]}_{row["KhoVaiID"]}";
                int stt = row["STTChonVT"] != DBNull.Value ? Convert.ToInt32(row["STTChonVT"]) : 0;
                var matchingRows = tblNhapKho.AsEnumerable()
                    .Where(r => r["MaVTID"].ToString() == row["MaVTID"].ToString() &&
                               r["MauVTID"].ToString() == row["MauVTID"].ToString() &&
                               r["MaNhom"].ToString() == row["MaNhom"].ToString() &&
                               r["KhoVaiID"].ToString() == row["KhoVaiID"].ToString());

                foreach (var r in matchingRows)
                {
            // r["STT"] = stt;
            r["STTChonVT"] = stt;
                }
                gCNLNhapKho.DataSource = tblNhapKho;
                gVNLNhapKho.RefreshData();
            };
            frm.OnUncheck = (row) =>
            {
                int deletedSTT = row["STTChonVT"] != DBNull.Value ? Convert.ToInt32(row["STTChonVT"]) : 0;
                Remove(row);
                if (deletedSTT > 0)
                {
                    var rowsToUpdate = tblNhapKho.AsEnumerable()
                        .Where(r => r["STTChonVT"] != DBNull.Value && Convert.ToInt32(r["STTChonVT"]) > deletedSTT);

                    foreach (var r in rowsToUpdate)
                    {
                        int newSTT = Convert.ToInt32(r["STT"]) - 1;
                //r["STT"] = newSTT;
                r["STTChonVT"] = newSTT;
                    }
                }
                gCNLNhapKho.DataSource = tblNhapKho;
                gVNLNhapKho.RefreshData();
            };
            frm.ShowDialog();
        }

        public void RemoveRowFromGrid(string maVTID)
        {
            var grid = _isNPL ? gCNLNhapKho : gCPLNhapKho;
            DataTable tbl = grid.DataSource as DataTable;
            if (tbl != null)
            {
                var rows = tbl.AsEnumerable()
                    .Where(r => r["MaVTID"] != DBNull.Value && r["MaVTID"].ToString() == maVTID)
                    .ToList();
                foreach (var r in rows)
                    tbl.Rows.Remove(r);
                grid.RefreshDataSource();
            }
        }

        private void btnChonVTPL_Click(object sender, EventArgs e)
        {
            if (tblNhapKhoPL == null)
                CreateTableNhapKhoPL();
            DataTable tblPL = gCPL.DataSource as DataTable;
            if (tblPL == null)
            {
                tblPL = CreateTableVatTu();
                gCPL.DataSource = tblPL;
            }

            //if (!tblPL.Columns.Contains("STT"))
            //{
            //    tblPL.Columns.Add("STT", typeof(int));
            //}

            //foreach (DataRow drTbl in tblPL.Rows)
            //{
            //    var matchingRows = tblNhapKhoPL.AsEnumerable()
            //        .Where(r => r["MaVTID"].ToString() == drTbl["MaVTID"].ToString() &&
            //                   r["MauVTID"].ToString() == drTbl["MauVTID"].ToString() &&
            //                   r["MaNhom"].ToString() == drTbl["MaNhom"].ToString() &&
            //                   r["KhoVaiID"].ToString() == drTbl["KhoVaiID"].ToString() &&
            //                   r["MaVTGhep"].ToString() == drTbl["MaVTGhep"].ToString())
            //        .Where(r => r["STT"] != DBNull.Value);

            //    if (matchingRows.Any())
            //    {
            //        var firstSTT = matchingRows.First()["STT"];
            //        drTbl["STT"] = firstSTT;
            //    }
            //}

            frmERPNhapKhoNPL_ChonVT frm = new frmERPNhapKhoNPL_ChonVT(tblPL, false);
            frm.IsEditMode = true;
            frm.OriginalCheckedKeys = _originalKeysFromDB_PL;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.OnCheck = (row) =>
            {
                AddRowGCNL(row);
                string key = $"{row["MaVTID"]}_{row["MauVTID"]}_{row["MaNhom"]}_{row["KhoVaiID"]}";
                int stt = row["STTChonVT"] != DBNull.Value ? Convert.ToInt32(row["STTChonVT"]) : 0;

                var matchingRows = tblNhapKhoPL.AsEnumerable()
                    .Where(r => r["MaVTID"].ToString() == row["MaVTID"].ToString() &&
                               r["MauVTID"].ToString() == row["MauVTID"].ToString() &&
                               r["MaNhom"].ToString() == row["MaNhom"].ToString() &&
                               r["KhoVaiID"].ToString() == row["KhoVaiID"].ToString());

                foreach (var r in matchingRows)
                {

                    r["STTChonVT"] = stt;
                }
                gCPLNhapKho.DataSource = tblNhapKhoPL;
                gVPLNhapKho.RefreshData();
            };
            frm.OnUncheck = (row) =>
            {
                int deletedSTT = row["STTChonVT"] != DBNull.Value ? Convert.ToInt32(row["STTChonVT"]) : 0;
                Remove(row);
                if (deletedSTT > 0)
                {
                    var rowsToUpdate = tblNhapKhoPL.AsEnumerable()
                        .Where(r => r["STTChonVT"] != DBNull.Value && Convert.ToInt32(r["STTChonVT"]) > deletedSTT);

                    foreach (var r in rowsToUpdate)
                    {
                        int newSTT = Convert.ToInt32(r["STTChonVT"]) - 1;
                //r["STT"] = newSTT;
                r["STTChonVT"] = newSTT;
                    }
                }
                gCPLNhapKho.DataSource = tblNhapKhoPL;
                gVPLNhapKho.RefreshData();
            };
            frm.Show();
        }
        #endregion


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

        private void searchLookUpEditPL_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            //if (selectedRowsPL != null && selectedRowsPL.Count > 0)
            //{
            //    // Lấy giá trị theo DisplayMember (ví dụ "TenPL")
            //    var selectedValues = selectedRowsPL
            //        .Select(r => r["MaVTGhep"]?.ToString())   // đổi "TenPL" thành DisplayMember thực tế
            //        .Where(val => !string.IsNullOrWhiteSpace(val))
            //        .ToList();

            //    e.DisplayText = string.Join("; ", selectedValues);
            //}
            //else
            //{
            //    e.DisplayText = "Chọn phụ liệu";
            //}
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

                    if (dr["MaNhom"].ToString() == _dr["MaNhom"].ToString() && dr["MaVTID"].ToString() == _dr["MaVTID"].ToString() && dr["MauVTID"].ToString() == _dr["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == _dr["KhoVaiID"].ToString() && dr["MaVTGhep"].ToString() == _dr["MaVTGhep"].ToString()
                      )
                    {
                        _dr["MaDVCD"] = _lst[0];
                        _dr["TenDVCD"] = _lst[1];
                    }

                }
            }
        }


        //private List<dynamic> LayDanhSachTrungTheoID(DataTable tblNhapKho, DataTable tblNL)
        //{
        //    // 1. Tìm dòng trùng trong tblNhapKho (như cũ):
        //    var duplicateGroups = tblNhapKho.AsEnumerable()
        //        .GroupBy(row => new
        //        {
        //            MaVTID = row["MaVTID"],
        //            MauVTID = row["MauVTID"],
        //            KhoVaiID = row["KhoVaiID"],
        //            SoKienHienThi = row["SoKienHienThi"]
        //        })
        //        .Where(g => g.Count() > 1)
        //        .SelectMany(g => g) // Đưa về từng DataRow riêng lẻ
        //        .ToList();

        //    // 2. JOIN với tblNL để lấy thêm thông tin MaVT, ChiTiet
        //    var mergedList = (
        //        from rowNK in duplicateGroups
        //        join rowNL in tblNL.AsEnumerable()
        //            on new
        //            {
        //                MaVTID = rowNK["MaVTID"],

        //                MauVTID = rowNK["MauVTID"],
        //                KhoVaiID = rowNK["KhoVaiID"]
        //            }
        //            equals new
        //            {
        //                MaVTID = rowNL["MaVTID"],

        //                MauVTID = rowNL["MauVTID"],
        //                KhoVaiID = rowNL["KhoVaiID"]
        //            }
        //        select new
        //        {
        //            MaVTID = rowNK["MaVTID"],
        //            MauVTID = rowNK["MauVTID"],
        //            KhoVaiID = rowNK["KhoVaiID"],
        //            SoKienHienThi = rowNK["SoKienHienThi"],

        //            MaVT = rowNL["MaVT"],
        //            ChiTiet = rowNL["ChiTiet"],
        //            MauVT = rowNL["MauVT"],
        //            KhoVai = rowNL["KhoVai"],
        //            DataRowNHapKho = rowNK
        //        }
        //    ).ToList<dynamic>();

        //    return mergedList;
        //}
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
                             $"\nBạn có muốn tiếp tục lưu không?";
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
            const string sep = "^|^";
            var rolllotbatchTracking = new Dictionary<string, List<int>>();
            var itemInfoForDuplicates = new Dictionary<string, string>();
            var duplicatesMsg = new List<string>();
            DataTable tblVatTu = _isNPL ? tblNL : tblPL;
            for (int i = 0; i < tblChiTiet.Rows.Count; i++)
            {
                DataRow row = tblChiTiet.Rows[i];
                int line = i + 1;
                string maVTID = row["MaVTID"]?.ToString().Trim().ToUpper() ?? "";
                string mauVTID = row["MauVTID"]?.ToString().Trim().ToUpper() ?? "";
                string khoVaiID = row["KhoVaiID"]?.ToString().Trim().ToUpper() ?? "";
                string maNhom = row["MaNhom"]?.ToString().Trim().ToUpper() ?? "";
                string maVTGhep = row["MaVTGhep"]?.ToString().Trim().ToUpper() ?? "";
                string roll = row["SoKienHienThi"].ToString().Trim().ToUpper() ?? "";
                string lot = row["SoLoT"].ToString().Trim().ToUpper() ?? "";
                string batch = row["Batch"].ToString().Trim().ToUpper() ?? "";

                if (string.IsNullOrEmpty(roll) && string.IsNullOrEmpty(lot) && string.IsNullOrEmpty(batch)) continue;
                string keyVatTu = $"{maVTID}{sep}{mauVTID}{sep}{khoVaiID}{sep}{maNhom}{sep}{maVTGhep}";
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
                string keyVatTu = $"{parts[0]}{sep}{parts[1]}{sep}{parts[2]}{sep}{parts[3]}{sep}{parts[4]}";
                string actualRoll = string.IsNullOrEmpty(parts[5]) ? "(trống)" : parts[5];
                string actualLot = string.IsNullOrEmpty(parts[6]) ? "(trống)" : parts[6];
                string actualBatch = string.IsNullOrEmpty(parts[7]) ? "(trống)" : parts[7];

                string infoVatTu = itemInfoForDuplicates.ContainsKey(keyVatTu)
                    ? itemInfoForDuplicates[keyVatTu]
                    : keyVatTu;

                duplicatesMsg.Add($"Vật tư: {infoVatTu}" +
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
                    MaVTGhep = row["MaVTGhep"],
                    MaNhom = row["MaNhom"],
                    MaVTID = row["MaVTID"],
                    MauVTID = row["MauVTID"],
                    KhoVaiID = row["KhoVaiID"],
                    SoKienHienThi = row["SoKienHienThi"]
                })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .ToList();

            var mergedList = (
                from rowNK in duplicateGroups
                join rowNL in tblNL.AsEnumerable()
                    on new
                    {
                        MaVTGhep = rowNK["MaVTGhep"],
                        MaNhom = rowNK["MaNhom"],
                        MaVTID = rowNK["MaVTID"],
                        MauVTID = rowNK["MauVTID"],
                        KhoVaiID = rowNK["KhoVaiID"]
                    }
                    equals new
                    {
                        MaVTGhep = rowNL["MaVTGhep"],
                        MaNhom = rowNL["MaNhom"],
                        MaVTID = rowNL["MaVTID"],
                        MauVTID = rowNL["MauVTID"],
                        KhoVaiID = rowNL["KhoVaiID"]
                    }
                select new
                {

                    MaNhom = rowNK["MaNhom"],
                    MaVTID = rowNK["MaVTID"],
                    MauVTID = rowNK["MauVTID"],
                    KhoVaiID = rowNK["KhoVaiID"],
                    SoKienHienThi = rowNK["SoKienHienThi"],
                    TenNhom = rowNL["TenNhom"],
                    MaVT = rowNL["MaVT"],
                    ChiTiet = rowNL["ChiTiet"],
                    MauVT = rowNL["MauVT"],
                    KhoVai = rowNL["KhoVai"],
                    MaVTGhep = rowNL["MaVTGhep"],
                    DataRowNHapKho = rowNK
                }
            ).ToList<dynamic>();

            return mergedList;
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


        private void searchLookUpEditKHCT_EditValueChanged(object sender, EventArgs e)
        {
            loadMaHangCT();
        }



        private void searchLookUpEditKHCTPL_EditValueChanged(object sender, EventArgs e)
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
        private void checkSearchLookUpVatTu2(bool isNL)
        {



        }
        private void searchLookUpEditNL_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e)
        {
            checkSearchLookUpVatTu2(true);
        }
        private void searchLookUpEditPL_CloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e)
        {
            checkSearchLookUpVatTu2(false);
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
        private void txtPOMua_EditValueChanged(object sender, EventArgs e)
        {
            createSearchLookUpLo();
        }
        private void txtPOMuaPL_EditValueChanged(object sender, EventArgs e)
        {
            createSearchLookUpLo();
        }
        private void gVNLNhapKho_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            var view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;

            if (e.Column != null && e.Column.FieldName == "IsIn")
            {
                var currentValue = view.GetRowCellValue(e.RowHandle, e.Column);
                bool newValue = !Convert.ToBoolean(currentValue);

                view.SetRowCellValue(e.RowHandle, e.Column, newValue);
                view.RefreshRow(e.RowHandle);

                this.ActiveControl = simpleButton1;
            }
        }
        private void gVPLNhapKho_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            var view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;

            if (e.Column != null && e.Column.FieldName == "IsIn")
            {
                var currentValue = view.GetRowCellValue(e.RowHandle, e.Column);
                bool newValue = !Convert.ToBoolean(currentValue);

                view.SetRowCellValue(e.RowHandle, e.Column, newValue);
                view.RefreshRow(e.RowHandle);

                this.ActiveControl = simpleButton1;
            }
        }
        private void gVPL_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            var view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;

            if (e.Column != null && e.Column.FieldName == "IsIn")
            {
                var currentValue = view.GetRowCellValue(e.RowHandle, e.Column);
                bool newValue = !Convert.ToBoolean(currentValue);

                view.SetRowCellValue(e.RowHandle, e.Column, newValue);
                view.RefreshRow(e.RowHandle);

                this.ActiveControl = simpleButton1;
            }
        }
        private void btnPDF_Click(object sender, EventArgs e)
        {
            try
            {

                var tailieu = "POMUA";
                var filename = btnPDF.Text.ToString();
                string objUrl = (string)settingsReader.GetValue("HostDH", typeof(String));

                if (!string.IsNullOrEmpty(filename))
                {
                    try
                    {
                        string url = $"{objUrl}/Content/Pdf/" + tailieu + "/" + filename;
                        System.Diagnostics.Process.Start(new ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không mở được ảnh: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }
        private async void btnPDF_DoubleClick(object sender, EventArgs e)
        {

        }


        private void btnPDFPL_Click(object sender, EventArgs e)
        {
            try
            {

                var tailieu = "POMUA";
                var filename = btnPDFPL.Text.ToString();
                string objUrl = (string)settingsReader.GetValue("HostDH", typeof(String));

                if (!string.IsNullOrEmpty(filename))
                {
                    try
                    {
                        string url = $"{objUrl}/Content/Pdf/" + tailieu + "/" + filename;
                        System.Diagnostics.Process.Start(new ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không mở được ảnh: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {


            }
        }
        private async void btnPDFPL_DoubleClick(object sender, EventArgs e)
        {

        }
        private string ReplaceSpecialCharacterssize(string input)
        {
            // Pattern để tìm khoảng trắng và các kí tự đặc biệt
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\|,.<>/?]+";
            // Thay thế bằng dấu _
            string replacement = "_";
            // Tạo một Regex và thực hiện thay thế
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement).ToUpper();
        }
        private async Task<bool> UploadPDF(string TenTaiLieu, string sourceFileName, string fileName)
        {
            string Url = "";

            var client = new WebClient();




            Url = string.Format($"{URL}ERPVatTuBOM/UploadFilePDF");
            var uri = new Uri(Url);
            try
            {
                client.Headers.Add("folderName", TenTaiLieu);
                client.Headers.Add("fileName", (fileName));
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
        private async void btnUploadPL_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.PDF) | *.PDF";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sourceFileName = openFileDialog.FileName;
                var fileName = RemoveVietnameseTone(ReplaceSpecialCharacterssize(Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName)));
                string folderTXT = "POMUA";
                Random rnd = new Random();
                int number = rnd.Next(100000, 1000000);
                var result = await UploadPDF(ReplaceSpecialCharacterssize(folderTXT), sourceFileName, fileName + number.ToString());
                if (result)
                {
                    //gVNL.SetRowCellValue(gVNL.FocusedRowHandle, fieldName, fileName + fieldName.ToString() + ".pdf");
                    btnPDFPL.Text = fileName + number.ToString() + ".pdf";
                }
            }
        }
        private async void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.PDF) | *.PDF";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sourceFileName = openFileDialog.FileName;
                var fileName = RemoveVietnameseTone(ReplaceSpecialCharacterssize(Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName)));

                string folderTXT = "POMUA";
                Random rnd = new Random();
                int number = rnd.Next(100000, 1000000);
                var result = await UploadPDF(ReplaceSpecialCharacterssize(folderTXT), sourceFileName, fileName + number.ToString());
                if (result)
                {
                    //gVNL.SetRowCellValue(gVNL.FocusedRowHandle, fieldName, fileName + fieldName.ToString() + ".pdf");
                    btnPDF.Text = fileName + number.ToString() + ".pdf";
                }
            }
        }
        private void btnThemNhanh_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable tbl = gCPL.DataSource as DataTable;

                if (tbl == null || tbl.Rows.Count == 0) return;
                var result = tbl.AsEnumerable()
                .Where(row => row.Field<bool>("IsThemNhanh") == true)
                .CopyToDataTable();
                if (result == null || result.Rows.Count == 0) return;
                Random rd = new Random();
                foreach (DataRow _dr in result.Rows)
                {
                    DataRow nr = tblNhapKhoPL.NewRow();

                    nr["ID"] = 0;
                    nr["SoLoID"] = searchLookUpEditLoPL.EditValue?.ToString() ?? "";
                    nr["MaNPL"] = _dr["MaNhom"].ToString() + "@" + _dr["MaVTID"].ToString() + "@" + _dr["MauVTID"].ToString() + "@" + _dr["KhoVaiID"].ToString();
                    nr["MaVTID"] = _dr["MaVTID"].ToString();
                    nr["MaMauVT"] = _dr["MaMauVT"].ToString();
                    nr["MauVT"] = _dr["MauVT"].ToString();
                    nr["SoKien"] = rd.Next(10000, 1000000);
                    nr["SoLot"] = "";

                    nr["MaHaiQuan"] = "";
                    nr["MaKeToan"] = "";
                    decimal val = 0;
                    decimal.TryParse(_dr["SLTong"]?.ToString(), out val);
                    nr["SoGhiDauCay"] = val;
                    nr["NW"] = 0;
                    nr["GW"] = 0;
                    string _pomua = txtPOMuaPL.EditValue?.ToString() ?? "";
                    nr["BarCode"] = $"{_pomua}|{_dr["MaVT"]}|{_dr["MaMauVT"]}|1";
                    nr["GhiChu"] = "";
                    nr["IsNPL"] = false;
                    nr["KhoVaiID"] = _dr["KhoVaiID"];
                    nr["SoKienParent"] = "";
                    nr["DonGia"] = _dr["DonGia"];
                    nr["ThanhTien"] = 0;
                    nr["SoLuongThucTe"] = 0;
                    nr["Pallet"] = "";
                    nr["MaDVVT"] = _dr["MaDVVT"];
                    nr["MauVTID"] = _dr["MauVTID"];
                    nr["SoKienHienThi"] = "1";

                    nr["STT"] = rd.Next(10000, 1000000);
                    nr["IsNK"] = false;
                    nr["TienTe"] = _dr["TienTe"];
                    nr["MaVTGhep"] = _dr["MaVTGhep"];
                    nr["MaNhom"] = _dr["MaNhom"];

                    nr["MaDVCD"] = _dr["MaDVVT"];
                    nr["TenDVCD"] = _dr["TenDVVT"];
                    nr["Batch"] = "";
                    nr["SLTong"] = _dr["SLTong"];
                    nr["TuoiTonKho"] = _dr["TuoiTonKho"];
                    // Lưu dòng mới vào List tạm thời
                    tblNhapKhoPL.Rows.Add(nr);
                }



                DataRow dr = gVPL.GetFocusedDataRow();
                if (dr == null) return;
                if (tblNhapKhoPL == null || tblNhapKhoPL.Rows.Count == 0) CreateTableNhapKhoPL();
                string maVTID = dr["MaVTID"].ToString();
                string maMauVT = dr["MauVTID"].ToString();
                string khoVaiID = dr["KhoVaiID"].ToString();
                string MaNhom = dr["MaNhom"].ToString();
                string MaVTGhep = dr["MaVTGhep"].ToString();
                DataTable tblNhapKhoFiltered = tblNhapKhoPL.Clone();
                var filteredRows = tblNhapKhoPL.AsEnumerable()
                .Where(row => row.Field<string>("MaVTID") == maVTID &&
                              row.Field<string>("MauVTID") == maMauVT &&
                              row.Field<string>("KhoVaiID") == khoVaiID &&
                              row.Field<string>("MaNhom") == MaNhom);
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
                foreach (DataRow row in tbl.Rows)
                {
                    if (row.Field<bool>("IsThemNhanh") == true)
                        row["IsThemNhanh"] = false;
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
                DataTable tblSaveEInvoice = CreateTable_E_Invoice();

                DataRow rowEInvoice = tblSaveEInvoice.NewRow();

                if (xtraTabControl1.SelectedTabPage == tabNL)
                {
                    rowEInvoice["ID"] = 0;
                    rowEInvoice["MaPhieuMH"] = _soloid;
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
                    rowEInvoice["ID"] = 0;
                    rowEInvoice["MaPhieuMH"] = _soloid;
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