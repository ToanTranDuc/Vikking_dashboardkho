using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.Erp.Kho;
using NtbSoft.ERP.Win.Modules.ThuVien;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
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
    public partial class frmERPNhapKhoNPL : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private DataTable tblNL, tblPL, tblMau, tblNhapKho, tblPhieu, tblSearchNL, tblSearchPL, tblSearchTienTe;
        private DataTable tblNhapKhoPL, tblPhieuPL;

        private HashSet<DataRow> selectedRows = new HashSet<DataRow>();
        private HashSet<DataRow> selectedRowsPL = new HashSet<DataRow>();
        bool _isNPL = true;
        string _NL = "", _NLDisplay = "";
        string _PL = "", _PLDisplay = "";
        bool ischeckNL = true, ischeckPL = true;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _allowView = false;
        bool _allowAddPL = false, _allowEditPL = false, _allowDeletePL = false, _allowViewPL = false;
        string _matau;
        string _soloid = string.Empty;
        bool isNK = false;
        private DataRow _drPO;
        private bool _isPOMua = false;
        private int _sttPO = 1;
        public frmERPNhapKhoNPL()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

        }

        public frmERPNhapKhoNPL(DataRow dr)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._drPO = dr;


        }
        protected override void OnLoad(EventArgs e)
        {
            comboTK.EditValue = "Nhập NPL mới";
            CheckPerminsion();
            Init();
            //loadVatTu(true);
            //loadVatTu(false);
            //loadMauVT();
            CreateTableNhapKho();
            CreateTablePhieu();
            CreateSearchLookUpTV();
            this.ActiveControl = simpleButton1;
            int selectedTab = xtraTabControl1.SelectedTabPageIndex;
            if (selectedTab == 0)
            {
                CheckPerminsionNL();
            }
            else
            {
                CheckPerminsionPL();
            }
            if (_drPO != null)
            {
                _isPOMua = true;
                //loadDataPoMua(1);
                txtPOMua.Text = _drPO["POMua"].ToString();
                txtPOMua.Properties.ReadOnly = true;
                searchLookUpEditNCC.EditValue = _drPO["MaNCC"].ToString();
                txtMaSoThue.Text = _drPO["MaSoThue"].ToString();
                LoadIPAdress();
            }
        }
        private void loadDataPoMua(int IsNPL)
        {
            try
            {
                string url = $"{URL}ERPNhapKhoNPLPOMUA/Get?Action=GETKETHUA&para1={_drPO["POMua"].ToString()}&para2={IsNPL}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    return;
                }
                if (IsNPL == 1)
                {
                    if (tblNhapKho == null || tblNhapKho.Rows.Count == 0)
                        CreateTableNhapKho();
                    DataTable tblGCNL = gCNL.DataSource as DataTable;
                    if (tblGCNL == null || tblGCNL.Rows.Count == 0)
                    {
                        tblGCNL = CreateTableVatTu();
                    }

                    tblNhapKho.Clear();
                    tblGCNL.Clear();
                    foreach (DataRow dr in tbl.Rows)
                    {

                        //lưới bên trái

                        object maVTID = dr["MaVTID"];
                        object maNhom = dr["MaCLVT"];
                        object maMauVT = dr["MauVTID"];
                        object khoVaiID = dr["KhoVaiID"];
                        object maVTGhep = dr["MaVTGhep"];
                        bool isDuplicate = tblGCNL.AsEnumerable().Any(r =>
                            (r.Field<object>("MaVTID") ?? DBNull.Value).Equals(maVTID ?? DBNull.Value) &&
                            (r.Field<object>("MaNhom") ?? DBNull.Value).Equals(maNhom ?? DBNull.Value) &&
                            (r.Field<object>("MauVTID") ?? DBNull.Value).Equals(maMauVT ?? DBNull.Value) &&
                            (r.Field<object>("KhoVaiID") ?? DBNull.Value).Equals(khoVaiID ?? DBNull.Value) &&
                            (r.Field<object>("MaVTGhep") ?? DBNull.Value).Equals(maVTGhep ?? DBNull.Value)
                        );

                        if (!isDuplicate)
                        {
                            DataRow row1 = tblGCNL.NewRow();
                            row1["MaVTID"] = dr["MaVTID"];
                            row1["MaVT"] = dr["MaVT"];
                            row1["ChiTiet"] = dr["ChiTiet"];
                            row1["MaNhom"] = dr["MaCLVT"];
                            row1["TenNhom"] = dr["ChungLoaiVatTu"];
                            row1["Sort"] = dr["Sort"];
                            row1["MaMauVT"] = dr["MaMauVT"];
                            row1["MauVT"] = dr["MauVT"];
                            row1["KhoVaiID"] = dr["KhoVaiID"];
                            row1["KhoVai"] = dr["KhoVai"];
                            row1["MaHaiQuan"] = "";
                            row1["MaKeToan"] = "";
                            row1["DonGia"] = dr["DonGia"];
                            row1["ThanhTien"] = dr["ThanhTien"];
                            row1["MaDVVT"] = dr["MaDVVT"];
                            row1["TenDVVT"] = dr["TenDVVT"];
                            row1["MauVTID"] = dr["MauVTID"];
                            row1["TienTe"] = dr["TienTeID"];
                            //row1["QuyDoiID"] = "";

                            row1["MaVTGhep"] = dr["MaVTGhep"];
                            row1["SLTong"] = dr["SoLuongMuaThem"];
                            row1["STTChonVT"] = _sttPO;
                            row1["TuoiTonKho"] = dr["TuoiTonKho"];
                            tblGCNL.Rows.Add(row1);
                            _sttPO++;
                        }


                        DataRow _nr = tblNhapKho.NewRow();

                        string maNPL = RemoveVietnameseTone(ReplaceSpecialCharacters(dr["MaCLVT"].ToString()) + "@"
                    + ReplaceSpecialCharacters(dr["MaVTID"].ToString()) + "@"
                    + ReplaceSpecialCharacters(dr["MauVTID"].ToString()) + "@"
                    + ReplaceSpecialCharacters(dr["KhoVaiID"].ToString()));

                        var newRows = new List<DataRow>();
                        //DataTable tblNhap = _isNPL ? tblNhapKho : tblNhapKhoPL;
                        //gVNLNhapKho.AddNewRow();
                        bool exists = (_isNPL ? tblNhapKho : tblNhapKhoPL).AsEnumerable().Any(r =>
            (r.Field<string>("MaVTID") ?? "") == (dr["MaVTID"]?.ToString() ?? "") &&
            (r.Field<string>("MauVTID") ?? "") == (dr["MauVTID"]?.ToString() ?? "") &&
            (r.Field<string>("MaNhom") ?? "") == (dr["MaCLVT"]?.ToString() ?? "") &&
            (r.Field<string>("KhoVaiID") ?? "") == (dr["KhoVaiID"]?.ToString() ?? "") &&
            (r.Field<string>("MaVTGhep") ?? "") == (dr["MaVTGhep"]?.ToString() ?? "")
        );


                        if (!exists)
                        {
                            DataRow nr = tblNhapKho.NewRow();
                            nr["ID"] = 0;
                            nr["SoLoID"] = _soloid;
                            nr["MaNPL"] = maNPL;
                            nr["MaVTID"] = dr["MaVTID"].ToString();
                            nr["MaMauVT"] = dr["MaMauVT"].ToString();
                            nr["MauVT"] = dr["MauVT"].ToString();
                            nr["SoKien"] = "";
                            nr["SoLot"] = "";
                            nr["MaHaiQuan"] = "";
                            nr["MaKeToan"] = "";
                            nr["SoGhiDauCay"] = 0;
                            nr["NW"] = 0;
                            nr["GW"] = 0;
                            nr["BarCode"] = "";
                            nr["GhiChu"] = "";
                            nr["IsNPL"] = _isNPL;
                            nr["KhoVaiID"] = dr["KhoVaiID"];
                            nr["SoKienParent"] = "";
                            nr["DonGia"] = 0;
                            nr["ThanhTien"] = 0;
                            nr["SoLuongThucTe"] = 0;
                            nr["Pallet"] = "";
                            nr["MaDVVT"] = dr["MaDVVT"];
                            nr["MauVTID"] = dr["MauVTID"];
                            nr["SoKienHienThi"] = "";
                            if (dr.Table.Columns.Contains("STT") && dr["STT"] != DBNull.Value)
                            {
                                nr["STT"] = dr["STT"];
                            }
                            if (dr.Table.Columns.Contains("STTChonVT") && dr["STTChonVT"] != DBNull.Value)
                            {
                                nr["STTChonVT"] = dr["STTChonVT"];
                            }
                            else
                            {
                                int newSTT = (_isNPL ? tblNhapKho : tblNhapKhoPL).AsEnumerable()
                                   .Where(r => r["STTChonVT"] != DBNull.Value)
                                                  .Select(r => Convert.ToInt32(r["STTChonVT"]))
                                   .DefaultIfEmpty(0)
                                   .Max() + 1;
                                nr["STTChonVT"] = newSTT;
                            }
                            nr["IsNK"] = false;
                            nr["TienTe"] = dr["TienTeID"];
                            nr["MaVTGhep"] = dr["MaVTGhep"];
                            nr["MaNhom"] = dr["MaCLVT"];
                            nr["MaDVCD"] = "";
                            nr["TenDVCD"] = "";
                            nr["Batch"] = "";
                            nr["TuoiTonKho"] = dr["TuoiTonKho"];
                            nr["SLTong"] = dr["SoLuongMuaThem"];
                            tblNhapKho.Rows.Add(nr);
                        }
                        gCNL.RefreshDataSource();

                    }
                    gCNL.DataSource = tblGCNL;
                }
                else
                {
                    if (tblNhapKhoPL == null || tblNhapKhoPL.Rows.Count == 0)
                        CreateTableNhapKhoPL();
                    DataTable tblGCPL = gCPL.DataSource as DataTable;
                    if (tblGCPL == null || tblGCPL.Rows.Count == 0)
                    {
                        tblGCPL = CreateTableVatTu();
                    }

                    tblNhapKhoPL.Clear();
                    tblGCPL.Clear();
                    foreach (DataRow dr in tbl.Rows)
                    {

                        //lưới bên trái

                        object maVTID = dr["MaVTID"];
                        object maNhom = dr["MaCLVT"];
                        object maMauVT = dr["MauVTID"];
                        object khoVaiID = dr["KhoVaiID"];
                        object maVTGhep = dr["MaVTGhep"];
                        bool isDuplicate = tblGCPL.AsEnumerable().Any(r =>
                            (r.Field<object>("MaVTID") ?? DBNull.Value).Equals(maVTID ?? DBNull.Value) &&
                            (r.Field<object>("MaNhom") ?? DBNull.Value).Equals(maNhom ?? DBNull.Value) &&
                            (r.Field<object>("MauVTID") ?? DBNull.Value).Equals(maMauVT ?? DBNull.Value) &&
                            (r.Field<object>("KhoVaiID") ?? DBNull.Value).Equals(khoVaiID ?? DBNull.Value) &&
                            (r.Field<object>("MaVTGhep") ?? DBNull.Value).Equals(maVTGhep ?? DBNull.Value)
                        );

                        if (!isDuplicate)
                        {
                            DataRow row1 = tblGCPL.NewRow();
                            row1["MaVTID"] = dr["MaVTID"];
                            row1["MaVT"] = dr["MaVT"];
                            row1["ChiTiet"] = dr["ChiTiet"];
                            row1["MaNhom"] = dr["MaCLVT"];
                            row1["TenNhom"] = dr["ChungLoaiVatTu"];
                            row1["Sort"] = dr["Sort"];
                            row1["MaMauVT"] = dr["MaMauVT"];
                            row1["MauVT"] = dr["MauVT"];
                            row1["KhoVaiID"] = dr["KhoVaiID"];
                            row1["KhoVai"] = dr["KhoVai"];
                            row1["MaHaiQuan"] = "";
                            row1["MaKeToan"] = "";
                            row1["DonGia"] = dr["DonGia"];
                            row1["ThanhTien"] = dr["ThanhTien"];
                            row1["MaDVVT"] = dr["MaDVVT"];
                            row1["TenDVVT"] = dr["TenDVVT"];
                            row1["MauVTID"] = dr["MauVTID"];
                            row1["TienTe"] = dr["TienTeID"];
                            //row1["QuyDoiID"] = "";

                            row1["MaVTGhep"] = dr["MaVTGhep"];
                            row1["SLTong"] = dr["SoLuongMuaThem"];
                            row1["STTChonVT"] = _sttPO;
                            row1["TuoiTonKho"] = dr["TuoiTonKho"];
                            tblGCPL.Rows.Add(row1);
                            _sttPO++;
                        }


                        DataRow _nr = tblNhapKhoPL.NewRow();

                        string maNPL = RemoveVietnameseTone(ReplaceSpecialCharacters(dr["MaCLVT"].ToString()) + "@"
                    + ReplaceSpecialCharacters(dr["MaVTID"].ToString()) + "@"
                    + ReplaceSpecialCharacters(dr["MauVTID"].ToString()) + "@"
                    + ReplaceSpecialCharacters(dr["KhoVaiID"].ToString()));

                        var newRows = new List<DataRow>();
                        //DataTable tblNhap = _isNPL ? tblNhapKho : tblNhapKhoPL;
                        //gVNLNhapKho.AddNewRow();
                        bool exists = tblNhapKhoPL.AsEnumerable().Any(r =>
           (r.Field<string>("MaVTID") ?? "") == (dr["MaVTID"]?.ToString() ?? "") &&
           (r.Field<string>("MauVTID") ?? "") == (dr["MauVTID"]?.ToString() ?? "") &&
           (r.Field<string>("MaNhom") ?? "") == (dr["MaCLVT"]?.ToString() ?? "") &&
           (r.Field<string>("KhoVaiID") ?? "") == (dr["KhoVaiID"]?.ToString() ?? "") &&
           (r.Field<string>("MaVTGhep") ?? "") == (dr["MaVTGhep"]?.ToString() ?? "")
        );


                        if (!exists)
                        {
                            DataRow nr = tblNhapKhoPL.NewRow();
                            nr["ID"] = 0;
                            nr["SoLoID"] = _soloid;
                            nr["MaNPL"] = maNPL;
                            nr["MaVTID"] = dr["MaVTID"].ToString();
                            nr["MaMauVT"] = dr["MaMauVT"].ToString();
                            nr["MauVT"] = dr["MauVT"].ToString();
                            nr["SoKien"] = "";
                            nr["SoLot"] = "";
                            nr["MaHaiQuan"] = "";
                            nr["MaKeToan"] = "";
                            nr["SoGhiDauCay"] = 0;
                            nr["NW"] = 0;
                            nr["GW"] = 0;
                            nr["BarCode"] = "";
                            nr["GhiChu"] = "";
                            nr["IsNPL"] = _isNPL;
                            nr["KhoVaiID"] = dr["KhoVaiID"];
                            nr["SoKienParent"] = "";
                            nr["DonGia"] = 0;
                            nr["ThanhTien"] = 0;
                            nr["SoLuongThucTe"] = 0;
                            nr["Pallet"] = "";
                            nr["MaDVVT"] = dr["MaDVVT"];
                            nr["MauVTID"] = dr["MauVTID"];
                            nr["SoKienHienThi"] = "";
                            if (dr.Table.Columns.Contains("STT") && dr["STT"] != DBNull.Value)
                            {
                                nr["STT"] = dr["STT"];
                            }
                            if (dr.Table.Columns.Contains("STTChonVT") && dr["STTChonVT"] != DBNull.Value)
                            {
                                nr["STTChonVT"] = dr["STTChonVT"];
                            }
                            else
                            {
                                int newSTT = (_isNPL ? tblNhapKho : tblNhapKhoPL).AsEnumerable()
                                   .Where(r => r["STTChonVT"] != DBNull.Value)
                                                  .Select(r => Convert.ToInt32(r["STTChonVT"]))
                                   .DefaultIfEmpty(0)
                                   .Max() + 1;
                                nr["STTChonVT"] = newSTT;
                            }
                            nr["IsNK"] = false;
                            nr["TienTe"] = dr["TienTeID"];
                            nr["MaVTGhep"] = dr["MaVTGhep"];
                            nr["MaNhom"] = dr["MaCLVT"];
                            nr["MaDVCD"] = "";
                            nr["TenDVCD"] = "";
                            nr["Batch"] = "";
                            nr["TuoiTonKho"] = dr["TuoiTonKho"];
                            nr["SLTong"] = dr["SoLuongMuaThem"];
                            tblNhapKhoPL.Rows.Add(nr);
                        }
                        gCPL.RefreshDataSource();

                    }
                    gCPL.DataSource = tblGCPL;
                }


            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi rôi: " + ex, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }


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
                if (!_allowEdit)
                {

                    btnMenuSua.Enabled = false;
                }
                else
                {
                    btnMenuSua.Enabled = true;
                }
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
                    Luu.Enabled = false;
                    setTxtReadOnly(true);
                }
                else
                {
                    Luu.Enabled = true;
                    setTxtReadOnly(false);
                }
            }

        }
        private void CheckPerminsionPL()
        {
            if (ischeckPL)
            {

                if (!_allowEditPL)
                {

                    btnMenuSua.Enabled = false;
                }
                else
                {
                    btnMenuSua.Enabled = true;
                }
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
                    Luu.Enabled = false;
                    setTxtReadOnly(false);
                }
                else
                {
                    Luu.Enabled = true;
                    setTxtReadOnly(false);
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

            repoSearchLookUpEditMau.DisplayMember = "MauVT";
            repoSearchLookUpEditMau.ValueMember = "MaMauVT";
            searchLookUpEditNL.Properties.ValueMember = "MaVTGhep";
            searchLookUpEditNL.Properties.DisplayMember = "MaVTGhep";
            searchLookUpEditPL.Properties.ValueMember = "MaVTGhep";
            searchLookUpEditPL.Properties.DisplayMember = "MaVTGhep";
            //dateNgayChungTu.DateTime = DateTime.Now;
            //dateNgayChungTuPL.DateTime = DateTime.Now;
            //dateNgayBienBan.DateTime = DateTime.Now;
            //dateNgayBienBanPL.DateTime = DateTime.Now;
            //dateNgayToKhai.DateTime = DateTime.Now;
            //dateNgayToKhaiPL.DateTime = DateTime.Now;
            //dateNgayGui.DateTime = DateTime.Now;
            //dateNgayGuiPL.DateTime = DateTime.Now;
            //dateNgayTaoPKL.DateTime = DateTime.Now;
            //dateNgayTaoPKLPL.DateTime = DateTime.Now;
            //dateSoHopDong.DateTime = DateTime.Now;
            //dateSoHopDongPL.DateTime = DateTime.Now;
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
            tbl.Columns.Add("DonGia", typeof(decimal));
            tbl.Columns.Add("ThanhTien", typeof(decimal));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("TenDVVT", typeof(string));
            tbl.Columns.Add("TienTe", typeof(string));
            if (!tbl.Columns.Contains("MaVTGhep"))
            {
                tbl.Columns.Add("MaVTGhep", typeof(string));
            }
            tbl.Columns.Add("POMua", typeof(string));
            tbl.Columns.Add("Batch", typeof(string));
            tbl.Columns.Add("SLTong", typeof(decimal));
            tbl.Columns.Add("STTChonVT", typeof(int));
            tbl.Columns.Add("TuoiTonKho", typeof(decimal));
            tbl.Columns.Add("IsThemNhanh", typeof(bool));
            return tbl;
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
            tblNhapKho.Columns.Add("SoKienHienThi", typeof(string));
            tblNhapKho.Columns.Add("IsNK", typeof(bool));
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
            tblNhapKhoPL.Columns.Add("SoKienHienThi", typeof(string));
            tblNhapKhoPL.Columns.Add("IsNK", typeof(bool));
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
        private void loadVatTu(bool isNPL)
        {
            try
            {
                string url = $"{URL}ERPNhapKhoNPL/Get?Action=GETNL";
                if (!isNPL)
                    url = $"{URL}ERPNhapKhoNPL/Get?Action=GETPL";

                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                if (isNPL)
                {
                    tblSearchNL = JsonConvert.DeserializeObject<DataTable>(json);
                    searchLookUpEditNL.Properties.DataSource = tblSearchNL;
                }
                else
                {
                    tblSearchPL = JsonConvert.DeserializeObject<DataTable>(json);
                    searchLookUpEditPL.Properties.DataSource = tblSearchPL;
                }

            }
            catch (Exception ex)
            {


            }
        }
        private void loadMauVT()
        {
            try
            {
                string url = $"{URL}ERPNhapKhoNPL/Get?Action=GETMAUVT";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                tblMau = JsonConvert.DeserializeObject<DataTable>(json);
                repoSearchLookUpEditMau.DataSource = tblMau;
            }
            catch (Exception ex)
            {


            }


        }
        private bool checkTxt()
        {
            if (string.IsNullOrWhiteSpace((_isNPL ? txtPOMua : txtPOMuaPL).Text))
            {
                XtraMessageBox.Show("Vui lòng nhập PO Mua.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                (_isNPL ? txtPOMua : txtPOMuaPL).Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace((_isNPL ? txtLo : txtLoPL).Text))
            {
                XtraMessageBox.Show("Vui lòng nhập PI NCC.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                (_isNPL ? txtLo : txtLoPL).Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace((_isNPL ? dateNgayNK : dateNgayNKPL).Text))
            {
                MessageBox.Show("Vui lòng chọn Ngày nhập.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                (_isNPL ? dateNgayNK : dateNgayNKPL).Focus();
                return false;
            }
            //var control = _isNPL ? searchLookUpEditKHCT : searchLookUpEditKHCTPL;
            //if (control.EditValue == null || string.IsNullOrWhiteSpace(control.Text))
            //{
            //    XtraMessageBox.Show("Vui lòng chọn khách hàng.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    control.Focus();
            //    return false;
            //}


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
            if (e.Column.FieldName == "TienTe")
            {
                DataRow drF = gVNL.GetFocusedDataRow();
                if (drF == null) return;
                DataTable tbl = gCNLNhapKho.DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0) return;
                foreach (DataRow dr in tbl.Rows)
                {
                    dr["TienTe"] = drF["TienTe"].ToString();
                }

                string maVTID_F = drF["MaVTID"].ToString();
                string mauVTID_F = drF["MauVTID"].ToString();
                string khoVaiID_F = drF["KhoVaiID"].ToString();
                string maNhom_F = drF["MaNhom"].ToString();

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
            if (e.Column.FieldName == "TuoiTonKho")
            {
                DataRow drF = gVNL.GetFocusedDataRow();
                if (drF == null) return;
                DataTable tbl = gCNLNhapKho.DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0) return;
                string maVTID_F = drF["MaVTID"].ToString();
                string mauVTID_F = drF["MauVTID"].ToString();
                string khoVaiID_F = drF["KhoVaiID"].ToString();
                string maNhom_F = drF["MaNhom"].ToString();
                //string maVTGhep_F = drF["MaVTGhep"].ToString();
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
            #region Thoai
            if (e.Column.FieldName == "SLTong")
            {
                DataRow drF = gVNL.GetFocusedDataRow();
                if (drF == null) return;

                string maVTID_F = drF["MaVTID"].ToString();
                string mauVTID_F = drF["MauVTID"].ToString();
                string khoVaiID_F = drF["KhoVaiID"].ToString();
                string maNhom_F = drF["MaNhom"].ToString();
                //string maVTGhep_F = drF["MaVTGhep"].ToString();
                string sLTong_F = drF["SLTong"].ToString();
                var rowsToUpdate = tblNhapKho.AsEnumerable()
                     .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                  dr["MauVTID"].ToString() == mauVTID_F &&
                                  dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                  dr["MaNhom"].ToString() == maNhom_F);

                foreach (var row in rowsToUpdate)
                {
                    row["SLTong"] = sLTong_F;
                }
            }
            #endregion
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
            if (e.Column.FieldName == "TienTe")
            {
                DataRow drF = gVPL.GetFocusedDataRow();
                if (drF == null) return;
                DataTable tbl = gCPLNhapKho.DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0) return;
                foreach (DataRow dr in tbl.Rows)
                {
                    dr["TienTe"] = drF["TienTe"].ToString();
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
                                   dr["MaNhom"].ToString() == maNhom_F &&
                                    dr["MaVTGhep"].ToString() == maVTGhep_F);

                foreach (var row in rowsToUpdate)
                {
                    row["TienTe"] = tienTe_F;
                }
            }
            if (e.Column.FieldName == "TuoiTonKho")
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
            #region Thoai
            if (e.Column.FieldName == "SLTong")
            {
                DataRow drF = gVPL.GetFocusedDataRow();
                if (drF == null) return;
                //DataTable tbl = gCPLNhapKho.DataSource as DataTable;
                //if (tbl == null || tbl.Rows.Count == 0) return;
                string maVTID_F = drF["MaVTID"].ToString();
                string mauVTID_F = drF["MauVTID"].ToString();
                string khoVaiID_F = drF["KhoVaiID"].ToString();
                string maNhom_F = drF["MaNhom"].ToString();
                string maVTGhep_F = drF["MaVTGhep"].ToString();
                string sLTong_F = drF["SLTong"].ToString();

                var rowsToUpdate = tblNhapKhoPL.AsEnumerable()
                     .Where(dr => dr["MaVTID"].ToString() == maVTID_F &&
                                  dr["MauVTID"].ToString() == mauVTID_F &&
                                  dr["KhoVaiID"].ToString() == khoVaiID_F &&
                                  dr["MaNhom"].ToString() == maNhom_F);

                foreach (var row in rowsToUpdate)
                {
                    row["SLTong"] = sLTong_F;
                }
            }

            #endregion
        }
        private void ganLaiDonGia(bool isNL)
        {
            if (isNL)
            {
                DataRow dr = gVNL.GetFocusedDataRow();
                string maVTID = dr["MaVTID"].ToString();
                string mauVTID = dr["MauVTID"].ToString();
                string khoVaiID = dr["KhoVaiID"].ToString();
                string maNhom = dr["MaNhom"].ToString();
                string MaVTGhep = dr["MaVTGhep"].ToString();

                var rowsToUpdate = tblNhapKho.AsEnumerable()
                     .Where(dr2 => dr2["MaVTID"].ToString() == maVTID &&
                                    dr2["MauVTID"].ToString() == mauVTID &&
                                    dr2["KhoVaiID"].ToString() == khoVaiID &&
                                    dr2["MaNhom"].ToString() == maNhom);

                foreach (var row in rowsToUpdate)
                {
                    row.BeginEdit();
                    row["DonGia"] = dr["DonGia"]; // Gán trực tiếp, không cần ToString()
                    row.EndEdit();
                }
            }
            else
            {
                DataRow dr = gVPL.GetFocusedDataRow();
                string maVTID = dr["MaVTID"].ToString();
                string mauVTID = dr["MauVTID"].ToString();
                string khoVaiID = dr["KhoVaiID"].ToString();
                string maNhom = dr["MaNhom"].ToString();
                string MaVTGhep = dr["MaVTGhep"].ToString();

                var rowsToUpdate = tblNhapKhoPL.AsEnumerable()
                     .Where(dr2 => dr2["MaVTID"].ToString() == maVTID &&
                                    dr2["MauVTID"].ToString() == mauVTID &&
                                    dr2["KhoVaiID"].ToString() == khoVaiID &&
                                    dr2["MaNhom"].ToString() == maNhom &&
                                    dr2["MaVTGhep"].ToString() == MaVTGhep);

                foreach (var row in rowsToUpdate)
                {
                    row.BeginEdit();
                    row["DonGia"] = dr["DonGia"]; // Gán trực tiếp, không cần ToString()
                    row.EndEdit();
                }
            }
        }
        private void ganLaiThanhTien(bool isNL)
        {
            try
            {
                if (isNL)
                {
                    foreach (DataRow dr in tblNhapKho.Rows)
                    {
                        DataRow row = gVNL.GetFocusedDataRow();
                        decimal _dg = Convert.ToDecimal(row["DonGia"].ToString());
                        if (_dg <= 0) return;
                        decimal _sltt = Convert.ToDecimal(dr["SoLuongThucTe"].ToString());
                        decimal _tt = _dg * _sltt;
                        dr["ThanhTien"] = _tt;
                    }



                }
                else
                {
                    foreach (DataRow dr in tblNhapKhoPL.Rows)
                    {
                        DataRow row = gVPL.GetFocusedDataRow();
                        decimal _dg = Convert.ToDecimal(row["DonGia"].ToString());
                        if (_dg <= 0) return;
                        decimal _sltt = Convert.ToDecimal(dr["SoLuongThucTe"].ToString());
                        decimal _tt = _dg * _sltt;
                        dr["ThanhTien"] = _tt;
                    }
                }
            }
            catch (Exception ex)
            {


            }
        }
        private void gVNL_ShowingEditor(object sender, CancelEventArgs e)
        {

            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
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
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
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


        private void gVNLNhapKho_KeyDown(object sender, KeyEventArgs e)
        {


            string _fieldName = gVNLNhapKho.FocusedColumn.FieldName;
            if (_fieldName == "SoKien") return;
            HandleGridViewKey(e, gVNLNhapKho, _fieldName);
        }

        private void gVPLNhapKho_KeyDown(object sender, KeyEventArgs e)
        {
            string _fieldName = gVPLNhapKho.FocusedColumn.FieldName;
            if (_fieldName == "SoKien") return;
            HandleGridViewKey(e, gVPLNhapKho, _fieldName);
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

                        DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy", CopyCell);
                        e.Menu.Items.Add(menuCopyItem);
                        DevExpress.Utils.Menu.DXMenuItem menuPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Paste", PasteCell);
                        e.Menu.Items.Add(menuPasteItem);

                        DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", XoaDongNK);
                        e.Menu.Items.Add(menuDeleteItem);

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

                var gridView = _isNPL ? gVNLNhapKho : gVPLNhapKho;
                var tbl = (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource as DataTable;
                var tblSource = _isNPL ? tblNhapKho : tblNhapKhoPL;
                int[] selectedRowHandles = gridView.GetSelectedRows();
                if (selectedRowHandles.Length == 0) return;

                var rowsToDeleteFromTbl = new List<DataRow>();

                foreach (int rowHandle in selectedRowHandles)
                {
                    if (!gridView.IsValidRowHandle(rowHandle)) continue;
                    DataRow dr = gridView.GetDataRow(rowHandle);
                    if (dr != null)
                        rowsToDeleteFromTbl.Add(dr);
                }

                foreach (DataRow dr in rowsToDeleteFromTbl)
                {
                    // Xoá ở tblSource (nếu cần)
                    DataRow[] rowsToDelete = tblSource.AsEnumerable()
                        .Where(row => row.Field<string>("MaVTID") == dr["MaVTID"].ToString() &&
                                      row.Field<string>("MaMauVT") == dr["MaMauVT"].ToString() &&
                                      row.Field<string>("KhoVaiID") == dr["KhoVaiID"].ToString() &&
                                      row.Field<string>("SoKien") == dr["SoKien"].ToString() &&
                                      row.Field<string>("MaNhom") == dr["MaNhom"].ToString() &&
                                      row.Field<string>("MaVTGhep") == dr["MaVTGhep"].ToString())
                        .ToArray();

                    foreach (DataRow row in rowsToDelete)
                    {
                        tblSource.Rows.Remove(row);
                    }

                    // Xoá khỏi datatable bind cho grid
                    tbl.Rows.Remove(dr);
                }

                (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource = tbl;
                gridView.RefreshData();
                resetTongKien();
            }
            catch (Exception ex)
            {

            }
        }
        private void gVNL_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            //DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            //if (gVNL.RowCount > 0)
            //{
            //    if (e.Menu == null)
            //        return;
            //    e.Menu.Items.Clear();

            //    if (e.HitInfo.InRow)
            //    {
            //        if (e.HitInfo.Column != null)
            //        {

            //            DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", XoaDongVT);
            //            e.Menu.Items.Add(menuDeleteItem);



            //        }

            //    }
            //}
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
                        row["MauVTID"].ToString() == changedDataRow["MauVTID"].ToString() &&
                        row["KhoVaiID"].ToString() == changedDataRow["KhoVaiID"].ToString() &&
                        row["BarCode"].ToString() == changedDataRow["BarCode"].ToString() &&
                        row["MaNhom"].ToString() == changedDataRow["MaNhom"].ToString()
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
                    //decimal tileGW = Convert.ToDecimal(gridView.GetRowCellValue(e.RowHandle, "tileGW") ?? 0);
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

        private void gvSearchNL_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRows.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
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

            int[] selectedRowsgop = gVSearchNL.GetSelectedRows();
            if (selectedRowsgop.Length > 0)
            {
                foreach (int rowHandles in selectedRowsgop)
                {
                    DataRow drRow = gVSearchNL.GetDataRow(rowHandles);
                    int rowHandle = gVSearchNL.FocusedRowHandle;
                    AddRowGCNL(drRow);

                }
            }
            int totalRows = gVSearchNL.RowCount;
            for (int i = 0; i < totalRows; i++)
            {
                if (!Array.Exists(selectedRowsgop, selectedRow => selectedRow == i))
                {
                    DataRow drRow = gVSearchNL.GetDataRow(i);
                    Remove(drRow);
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
                string filter = string.Format("MaVTID = '{0}'  AND MauVTID = '{1}' AND MaNhom = '{2}' AND KhoVaiID='{3}' AND MaVTGhep='{4}'",
                                               rowAdd["MaVTID"], rowAdd["MauVTID"], rowAdd["MaNhom"], rowAdd["KhoVaiID"], rowAdd["MaVTGhep"]);
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
                    newRow["DonGia"] = 0;
                    newRow["ThanhTien"] = 0;
                    newRow["MaDVVT"] = rowAdd["MaDVVT"];
                    newRow["TenDVVT"] = rowAdd["TenDVVT"];
                    newRow["MauVTID"] = rowAdd["MauVTID"];
                    newRow["TienTe"] = matiente;
                    newRow["MaVTGhep"] = rowAdd["MaVTGhep"];
                    newRow["SLTong"] = 0;
                    newRow["TuoiTonKho"] = rowAdd["TuoiTonKho"];
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
                    if (rowAdd.Table.Columns.Contains("STTChonVT") && rowAdd["STTChonVT"] != DBNull.Value)
                    {
                        if (!tbl.Columns.Contains("STTChonVT"))
                        {
                            tbl.Columns.Add("STTChonVT", typeof(int));
                        }
                        newRow["STTChonVT"] = rowAdd["STTChonVT"];
                    }
                    tbl.Rows.Add(newRow);
                    (_isNPL ? gCNL : gCPL).DataSource = tbl;
                    BestFitCol(_isNPL ? gVNL : gVPL);
                }

                string maNPL = RemoveVietnameseTone(ReplaceSpecialCharacters(rowAdd["MaNhom"].ToString()) + "@"
                    + ReplaceSpecialCharacters(rowAdd["MaVTID"].ToString()) + "@"
                    + ReplaceSpecialCharacters(rowAdd["MauVTID"].ToString()) + "@"
                    + ReplaceSpecialCharacters(rowAdd["KhoVaiID"].ToString()));
                #region T
                var newRows = new List<DataRow>();
                //DataTable tblNhap = _isNPL ? tblNhapKho : tblNhapKhoPL;
                //gVNLNhapKho.AddNewRow();
                bool exists = (_isNPL ? tblNhapKho : tblNhapKhoPL).AsEnumerable().Any(r =>
    (r.Field<string>("MaVTID") ?? "") == (rowAdd["MaVTID"]?.ToString() ?? "") &&
    (r.Field<string>("MauVTID") ?? "") == (rowAdd["MauVTID"]?.ToString() ?? "") &&
    (r.Field<string>("MaNhom") ?? "") == (rowAdd["MaNhom"]?.ToString() ?? "") &&
    (r.Field<string>("KhoVaiID") ?? "") == (rowAdd["KhoVaiID"]?.ToString() ?? "") &&
    (r.Field<string>("MaVTGhep") ?? "") == (rowAdd["MaVTGhep"]?.ToString() ?? "")
);


                if (!exists)
                {
                    DataRow nr = (_isNPL ? tblNhapKho : tblNhapKhoPL).NewRow();
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
                    }
                    if (rowAdd.Table.Columns.Contains("STTChonVT") && rowAdd["STTChonVT"] != DBNull.Value)
                    {
                        nr["STTChonVT"] = rowAdd["STTChonVT"];
                    }
                    else
                    {
                        int newSTT = (_isNPL ? tblNhapKho : tblNhapKhoPL).AsEnumerable()
                           .Where(r => r["STTChonVT"] != DBNull.Value)
                                          .Select(r => Convert.ToInt32(r["STTChonVT"]))
                           .DefaultIfEmpty(0)
                           .Max() + 1;
                        nr["STTChonVT"] = newSTT;
                    }
                    nr["IsNK"] = false;
                    nr["TienTe"] = matiente;
                    nr["MaVTGhep"] = rowAdd["MaVTGhep"];
                    nr["MaNhom"] = rowAdd["MaNhom"];
                    nr["MaDVCD"] = "";
                    nr["TenDVCD"] = "";
                    nr["Batch"] = "";
                   
                    nr["TuoiTonKho"] = rowAdd["TuoiTonKho"];
                    (_isNPL ? tblNhapKho : tblNhapKhoPL).Rows.Add(nr);
                }
                (_isNPL ? gCNL : gCPL).RefreshDataSource();
                #endregion
            }
            catch (Exception ex)
            {

            }
        }
        //private void Remove(DataRow rowAdd)
        //{
        //    try
        //    {
        //        if (rowAdd == null) return;
        //        DataTable tbl = (_isNPL ? gCNL : gCPL).DataSource as DataTable;
        //        if (tbl == null) return;
        //        var row = tbl.AsEnumerable().Where(x => x["MaVTID"].ToString() == rowAdd["MaVTID"].ToString() && x["KhoVaiID"].ToString() == rowAdd["KhoVaiID"].ToString()
        //       && x["MaNhom"].ToString() == rowAdd["MaNhom"].ToString() && x["MauVTID"].ToString() == rowAdd["MauVTID"].ToString()&&x["MaVTGhep"].ToString() == rowAdd["MaVTGhep"].ToString()).ToList();
        //        foreach (var r in row)
        //        {
        //            tbl.Rows.Remove(r);
        //        }


        //        DataTable tbl2 = (_isNPL ? tblNhapKho : tblNhapKhoPL);
        //        if (tbl2 == null) return;
        //        var row2 = tbl2.AsEnumerable().Where(x => x["MaVTID"].ToString() == rowAdd["MaVTID"].ToString() && x["KhoVaiID"].ToString() == rowAdd["KhoVaiID"].ToString()
        //       && x["MauVTID"].ToString() == rowAdd["MauVTID"].ToString() && x["MaVTGhep"].ToString() == rowAdd["MaVTGhep"].ToString()).ToList();
        //        foreach (var r in row2)
        //        {
        //            tbl2.Rows.Remove(r);
        //        }



        //        DataTable tbl3 = (_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource as DataTable;
        //        if (tbl3 == null) return;
        //        var row3 = tbl3.AsEnumerable().Where(x => x["MaVTID"].ToString() == rowAdd["MaVTID"].ToString() && x["KhoVaiID"].ToString() == rowAdd["KhoVaiID"].ToString()
        //       && x["MauVTID"].ToString() == rowAdd["MauVTID"].ToString() && x["MaVTGhep"].ToString() == rowAdd["MaVTGhep"].ToString()).ToList();
        //        foreach (var r in row3)
        //        {
        //            tbl3.Rows.Remove(r);
        //        }

        //    }
        //    catch (Exception ex)
        //    {


        //    }
        //}
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
                //CopyFromTxtPLToTxt();
                CheckPerminsionNL();
                if (_isPOMua)
                {
                    txtPOMua.Text = _drPO["POMua"].ToString();
                    txtPOMua.Properties.ReadOnly = true;
                    searchLookUpEditNCC.EditValue = _drPO["MaNCC"].ToString();
                    txtMaSoThue.Text = _drPO["MaSoThue"].ToString();
                    //loadDataPoMua(1);
                }

            }
            else if (selectedTabPage.Name == "tabPL")
            {
                _isNPL = false;
                //CopyFromTxtToTxtPL();
                CheckPerminsionPL();
                if (_isPOMua)
                {
                    txtPOMuaPL.Text = _drPO["POMua"].ToString();
                    txtPOMuaPL.Properties.ReadOnly = true;
                    searchLookUpEditNCCPL.EditValue = _drPO["MaNCC"].ToString();
                    txtMaSoThuePL.Text = _drPO["MaSoThue"].ToString();
                    //loadDataPoMua(0);
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
                        DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy", CopyCell);
                        e.Menu.Items.Add(menuCopyItem);
                        DevExpress.Utils.Menu.DXMenuItem menuPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Paste", PasteCell);
                        e.Menu.Items.Add(menuPasteItem);

                        DevExpress.Utils.Menu.DXMenuItem menuDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", XoaDongNK);
                        e.Menu.Items.Add(menuDeleteItem);

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
                //SendKeys.SendWait("{ESC}");



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
                //var maxSoKien = tblNK.AsEnumerable().Max(row => Convert.ToInt32(row["SoKien"]));
                //var minSoKien = tblNK.AsEnumerable().Min(row => Convert.ToInt32(row["SoKien"]));

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

                foreach (DataRow _dr in tblNhapKho.Rows)
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

            gridView.BeginUpdate();
            try
            {
                // === PHẦN 1: Tối ưu hóa việc đồng bộ dữ liệu giữa hai DataTable ===
                DataRowView changedRowView = gridView.GetRow(e.RowHandle) as DataRowView;
                if (changedRowView != null)
                {
                    DataRow changedDataRow = changedRowView.Row;

                    // Tìm chính xác dòng cần cập nhật trong tblNhapKhoPL bằng LINQ, thay vì lặp
                    var targetRow = tblNhapKhoPL.AsEnumerable().FirstOrDefault(row =>
                        row.Field<string>("MaVTID") == changedDataRow.Field<string>("MaVTID") &&
                        row.Field<string>("MauVTID") == changedDataRow.Field<string>("MauVTID") &&
                        row.Field<string>("KhoVaiID") == changedDataRow.Field<string>("KhoVaiID") &&
                        row["BarCode"].ToString() == changedDataRow["BarCode"].ToString() &&
                        row.Field<string>("MaNhom") == changedDataRow.Field<string>("MaNhom")
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
                              row.Field<string>("MaNhom") == MaNhom &&
                              row.Field<string>("MaVTGhep") == MaVTGhep);
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
                              row.Field<string>("MaNhom") == MaNhom &&
                              row.Field<string>("MaVTGhep") == MaVTGhep);
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

            }
            catch (Exception ex)
            {
            }
        }

        #endregion
        private void searchLookUpEditNL_Popup(object sender, EventArgs e)
        {
            var popupControl = sender as DevExpress.Utils.Win.IPopupControl;
            if (popupControl == null) return;
            var popupForm = popupControl.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
            if (popupForm == null) return;
            var searchLookUpPopup = popupForm.Controls.OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>().FirstOrDefault();
            if (searchLookUpPopup == null) return;
            var layout = searchLookUpPopup.Controls.OfType<LayoutControl>().FirstOrDefault();
            if (layout == null) return;
            AddCustomButtonToPopupLayout(layout, "btnOK", "OK", searchOKButton_Click);
        }
        private void searchOKButton_Click(object sender, EventArgs e)
        {
            searchLookUpEditNL.ClosePopup();
        }

        private void searchLookUpEditPL_Popup(object sender, EventArgs e)
        {
            var popupControl = sender as DevExpress.Utils.Win.IPopupControl;
            if (popupControl == null) return;
            var popupForm = popupControl.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
            if (popupForm == null) return;
            var searchLookUpPopup = popupForm.Controls.OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>().FirstOrDefault();
            if (searchLookUpPopup == null) return;
            var layout = searchLookUpPopup.Controls.OfType<LayoutControl>().FirstOrDefault();
            if (layout == null) return;
            AddCustomButtonToPopupLayout(layout, "btnOK", "OK", searchOKPLButton_Click);

        }
        private void searchOKPLButton_Click(object sender, EventArgs e)
        {
            searchLookUpEditPL.ClosePopup();
        }

        #region barbutton
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = simpleButton1;
            if (comboTK.EditValue.ToString() == "Nhập NPL mới")
            {
                isNK = false;
            }
            else
            {
                isNK = true;
            }
            if (_isNPL)
            {
                if (!luuPhieu()) return;
                luuNhapKho();
            }
            else
            {
                if (!luuPhieuPL()) return;
                luuNhapKhoPL();
            }
            //Save_E_Invoice();
        }
        private void btnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string _lo = RemoveVietnameseTone(ReplaceSpecialCharacters((_isNPL ? txtLo : txtLoPL).Text.ToString()));
            frmERPNhapKhoNPLSua frm = new frmERPNhapKhoNPLSua(_lo, _isNPL);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
        }

        #endregion

        #region RepositoryItem
        private void btnThem_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                DataRow dr = gVNL.GetFocusedDataRow();
                if (dr == null) return;
                //if (string.IsNullOrWhiteSpace(txtLo.Text))
                //{
                //    MessageBox.Show("Vui lòng nhập Số Lô.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    txtLo.Focus();
                //    return;
                //}

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

                frmERPNhapKhoNPL_KhaiBao frm = new frmERPNhapKhoNPL_KhaiBao(dr, tblNhapKho, txtLo.Text.ToString(), true, tileNW, tileGW, madvcd, tendvcd, _soloid, batch);
                frm.StartPosition = FormStartPosition.CenterScreen;

                frm.ThongSoKienChanged += (list) =>
                {
                    tblNhapKho = frmERPNhapKhoNPL_KhaiBao.dt;
                    string maVTID = dr["MaVTID"].ToString();
                    string maMauVT = dr["MauVTID"].ToString();
                    string khoVaiID = dr["KhoVaiID"].ToString();
                    string MaNhom = dr["MaNhom"].ToString();
                    string MaVTGhep = dr["MaVTGhep"].ToString();
                    var filteredRows = tblNhapKho.AsEnumerable()
                        .Where(row => row["MaVTID"].ToString() == maVTID &&
                                      row["MauVTID"].ToString() == maMauVT &&
                                      row["KhoVaiID"].ToString() == khoVaiID &&
                                      row["MaNhom"].ToString() == MaNhom &&
                                      row["MaVTGhep"].ToString() == MaVTGhep);

                    foreach (var row in filteredRows)
                    {
                        row["STT"] = sttChonVT;
                        row["sttChonVT"] = sttChonVT;
                    }
                    if (filteredRows.Any())
                    {
                        // Chuyển đổi trực tiếp sang DataTable mới
                        DataTable tblNhapKhoFiltered = filteredRows.CopyToDataTable();
                        gCNLNhapKho.DataSource = tblNhapKhoFiltered;
                    }
                    else
                    {

                        gCNLNhapKho.DataSource = tblNhapKho.Clone();
                    }
                };
                frm.ShowDialog();



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
                if (string.IsNullOrWhiteSpace(txtLoPL.Text))
                {
                    MessageBox.Show("Vui lòng nhập PI NCC.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLoPL.Focus();
                    return;
                }


                List<string> currentList = new List<string>();
                if (tblNhapKhoPL == null || tblNhapKhoPL.Rows.Count == 0) CreateTableNhapKhoPL();
                int sttChonVT = 1;
                if (dr.Table.Columns.Contains("STTChonVT") && dr["STTChonVT"] != DBNull.Value)
                {
                    sttChonVT = Convert.ToInt32(dr["STTChonVT"]);
                }
                DataRow _dr = gVPLNhapKho.GetFocusedDataRow();
                decimal tileNW = 0m, tileGW = 0m;
                string madvcd = string.Empty, tendvcd = string.Empty, batch = string.Empty;
                if (_dr != null)
                {
                    tileNW = decimal.TryParse(_dr["tileNW"].ToString(), out var val) ? val : 0m;
                    tileGW = decimal.TryParse(_dr["tileGW"].ToString(), out var val2) ? val2 : 0m;
                    madvcd = _dr["MaDVCD"].ToString();
                    tendvcd = _dr["TenDVCD"].ToString();
                    batch = _dr["Batch"].ToString();
                }

                frmERPNhapKhoNPL_KhaiBao frm = new frmERPNhapKhoNPL_KhaiBao(dr, tblNhapKhoPL, txtLoPL.Text.ToString(), false, tileNW, tileGW, madvcd, tendvcd, _soloid, batch);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ThongSoKienChanged += (list) =>
                {
                    tblNhapKhoPL = frmERPNhapKhoNPL_KhaiBao.dt;
                    DataTable tblNhapKhoFiltered = tblNhapKhoPL.Clone();
                    string maVTID = dr["MaVTID"].ToString();
                    string maMauVT = dr["MauVTID"].ToString();
                    string khoVaiID = dr["KhoVaiID"].ToString();
                    string MaNhom = dr["MaNhom"].ToString();
                    string MaVTGhep = dr["MaVTGhep"].ToString();
                    var filteredRows = tblNhapKhoPL.AsEnumerable()
                        .Where(row => row.Field<string>("MaVTID") == maVTID &&
                                      row.Field<string>("MauVTID") == maMauVT &&
                                      row.Field<string>("KhoVaiID") == khoVaiID &&
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

                    gCPLNhapKho.DataSource = tblNhapKhoFiltered;
                };
                frm.ShowDialog();


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
        private void repoTxtSLTT_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repoTxtThanhTien_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }
        private void repoTxtDonGia_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void searchLookUpEdit1_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {

            var selectedRows = gVSearchPL.GetSelectedRows();
            var selectedValues = selectedRows
                .Select(rowHandle => gVSearchPL.GetRowCellValue(rowHandle, searchLookUpEditPL.Properties.DisplayMember)?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();

            _PLDisplay = selectedValues.Any() ? string.Join("; ", selectedValues) : "Chọn phụ liệu";
            e.DisplayText = _PLDisplay;
        }

        private void gVSearchPL_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            GridView view = sender as GridView;
            int rowHandle3 = e.ControllerRow;
            if (view == null) return;
            if (rowHandle3 >= 0)
            {
                DataRow row = view.GetDataRow(rowHandle3);

                if (view.IsRowSelected(rowHandle3))
                {
                    selectedRowsPL.Add(row);

                }
                else
                {
                    selectedRowsPL.Remove(row);

                }
            }


            string selectedValues = string.Join(";", gVSearchPL.GetSelectedRows().Select(rowHandle2 => gVSearchPL.GetRowCellValue(rowHandle2, searchLookUpEditPL.Properties.ValueMember)));
            searchLookUpEditPL.EditValue = selectedValues;
            if (searchLookUpEditPL.EditValue is null) return;
            _PL = searchLookUpEditPL.EditValue.ToString();

            int[] selectedRowsgop = gVSearchPL.GetSelectedRows();
            if (selectedRowsgop.Length > 0)
            {
                foreach (int rowHandles in selectedRowsgop)
                {
                    DataRow drRow = gVSearchPL.GetDataRow(rowHandles);
                    int rowHandle = gVSearchPL.FocusedRowHandle;


                    AddRowGCNL(drRow);


                }
            }
            int totalRows = gVSearchPL.RowCount;
            for (int i = 0; i < totalRows; i++)
            {
                if (!Array.Exists(selectedRowsgop, selectedRow => selectedRow == i))
                {
                    DataRow drRow = gVSearchPL.GetDataRow(i);
                    Remove(drRow);
                }
            }
            gVSearchPL.RefreshData();
        }


        private void searchLookUpEditNL_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {

            var selectedRows = gVSearchNL.GetSelectedRows();
            var selectedValues = selectedRows
                .Select(rowHandle => gVSearchNL.GetRowCellValue(rowHandle, searchLookUpEditNL.Properties.DisplayMember)?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value)) // Loại bỏ giá trị null, rỗng hoặc chỉ chứa ký tự trắng
                .ToList();

            _NLDisplay = selectedValues.Any() ? string.Join("; ", selectedValues) : "Chọn nguyên liệu";
            e.DisplayText = _NLDisplay;
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

        }

        private void txtMaSoThuePL_KeyPress(object sender, KeyPressEventArgs e)
        {

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

        private void gCPLNhapKho_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteCell(sender, e);
            }
        }



        private void btnLamSach_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            setTxtNull();
            setGVNull();
            huySelect();
        }

        private void setTxtNull()
        {
            if (_isNPL)
            {
                txtLo.Text = "";
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
                txtPOMuaPL.Text = "";
                txtPOMua.Text = "";
                txtSoVanDon.Text = "";
                txtQCKiem.Text = "";
                txtNW.Text = "";
                txtGW.Text = "";
                cbKiemKe.Checked = false;
                ngayKK.EditValue = null;
                dateNgayNK.EditValue = null;
                searchLookUpEditKHCT.EditValue = null;
                searchLookUpEditMHCT.EditValue = null;
                dateNgayNK.EditValue = null;
                btnPDF.Text = "";
            }
            else
            {
                txtLoPL.Text = "";
                searchLookUpEditNCCPL.EditValue = null;

                txtSoToKhaiPL.Text = "";
                dateNgayToKhaiPL.EditValue = null;
                txtSoHopDongPL.Text = "";
                dateSoHopDongPL.EditValue = null;
                txtMaSoThuePL.EditValue = "";
                txtSoHoaDonPL.Text = "";
                searchLookUpEditCangPL.EditValue = null;
                SearchlookupTauPL.EditValue = null;
                dateNgayGuiPL.EditValue = null;
                txtNoiGuiPL.Text = "";
                txtQCKiemPL.Text = "";
                txtNWPL.Text = "";
                txtGWPL.Text = "";
                cbKiemKePL.Checked = false;
                ngayKKPL.EditValue = null;
                dateNgayNKPL.EditValue = null;
                //txtDiaChiNhanPL.Text = "";
                txtPOMuaPL.Text = "";
                txtSoVanDonPL.Text = "";
                searchLookUpEditKHCTPL.EditValue = null;
                searchLookUpEditMHCTPL.EditValue = null;
                dateNgayNKPL.EditValue = null;
                btnPDFPL.Text = "";
            }

        }
        private void setGVNull()
        {
            if (_isNPL)
            {
                tblNL.Clear();

                tblNhapKho.Clear();
                gCNL.DataSource = null;
                gCNLNhapKho.DataSource = null;

            }
            else
            {
                tblPL.Clear();

                tblNhapKhoPL.Clear();
                gCPL.DataSource = null;
                gCPLNhapKho.DataSource = null;
            }
        }
        private void huySelect()
        {
            if (_isNPL)
            {
                selectedRows.Clear();
                gVSearchNL.ClearSelection();
            }
            else
            {
                selectedRowsPL.Clear();
                gVSearchPL.ClearSelection();
            }
        }

        private void gVNLNhapKho_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var column = view.FocusedColumn;

            if (column.FieldName == "SoGhiDauCay" || column.FieldName == "NW" || column.FieldName == "GW" || column.FieldName == "SoLuongThucTe")
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

        private void gVPLNhapKho_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var column = view.FocusedColumn;

            if (column.FieldName == "SoGhiDauCay" || column.FieldName == "NW" || column.FieldName == "GW" || column.FieldName == "SoLuongThucTe")
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

        private void splitContainerControl2_Paint(object sender, PaintEventArgs e)
        {
            splitContainerControl2.SplitterPosition = (int)(splitContainerControl2.Width * 0.69);

        }

        private void splitContainerControl1_Paint(object sender, PaintEventArgs e)
        {
            splitContainerControl1.SplitterPosition = (int)(splitContainerControl1.Width * 0.69);
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

        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            CreateSearchLookUpTV();
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
                        if (row["MaVTID"].ToString() == dr["MaVTID"].ToString() && row["MauVTID"].ToString() == dr["MauVTID"].ToString() && row["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString() && row["MaNhom"].ToString() == dr["MaNhom"].ToString())
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
                        if (row["MaVTID"].ToString() == dr["MaVTID"].ToString() && row["MauVTID"].ToString() == dr["MauVTID"].ToString() && row["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString() && row["MaNhom"].ToString() == dr["MaNhom"].ToString())
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
        private void ganThanhTien(bool isNL)
        {
            try
            {
                //if (isNL)
                //{
                //    DataRow dr = gVNLNhapKho.GetFocusedDataRow();
                //    DataRow row = gVNL.GetFocusedDataRow();
                //    decimal _dg = Convert.ToDecimal(row["DonGia"].ToString());
                //    if (_dg <= 0) return;
                //    decimal _sltt = Convert.ToDecimal(dr["SoLuongThucTe"].ToString());
                //    decimal _tt = _dg * _sltt;
                //    var rows = tblNhapKho.AsEnumerable().Where(row2 =>
                //    row2["MaVTID"].Equals(dr["MaVTID"]) &&
                //    row2["MauVTID"].Equals(dr["MauVTID"]) &&
                //    row2["KhoVaiID"].Equals(dr["KhoVaiID"]) &&
                //     row2["SoKien"].Equals(dr["SoKien"])
                //    );
                //    foreach (var row3 in rows)
                //    {
                //        row3["ThanhTien"] = _tt;
                //    }
                //}
                //else
                //{
                //    DataRow dr = gVPLNhapKho.GetFocusedDataRow();
                //    DataRow row = gVPL.GetFocusedDataRow();
                //    decimal _dg = Convert.ToDecimal(row["DonGia"].ToString());
                //    if (_dg <= 0) return;
                //    decimal _sltt = Convert.ToDecimal(dr["SoLuongThucTe"].ToString());
                //    decimal _tt = _dg * _sltt;
                //    var rows = tblNhapKhoPL.AsEnumerable().Where(row2 =>
                //    row2["MaVTID"].Equals(dr["MaVTID"]) &&
                //    row2["MauVTID"].Equals(dr["MauVTID"]) &&
                //    row2["KhoVaiID"].Equals(dr["KhoVaiID"]) &&
                //     row2["SoKien"].Equals(dr["SoKien"])
                //    );
                //    foreach (var row3 in rows)
                //    {
                //        row3["ThanhTien"] = _tt;
                //    }
                //}
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

        private string ReplaceSpecialCharacters(string input)
        {
            string pattern = @"[\s!@#$%^&*()\-+=\[\]{};:'""\\”|,.<>/?]+";
            string replacement = "_";
            Regex regex = new Regex(pattern);
            return regex.Replace(input, replacement);
        }
        private string ReplaceSpecialCharactersV2(string input)
        {
            // Loại bỏ @ khỏi pattern
            string pattern = @"[\s!#$%^&*()\-+=\[\]{};:'""\\”|,.<>/?]+";

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
        private void luuNhapKho()
        {
            try
            {

                if (tblNhapKho == null || tblNhapKho.Rows.Count == 0) return;
                //DataTable hehe = gCNLNhapKho.DataSource as DataTable;
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
                    return;
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
                                return;
                            }
                        }
                    }
                    //if (tblNhapKho.Columns.Contains("SoLot"))
                    //{
                    //    var soLotValue = dr["SoLot"];
                    //    if (dr["Barcode"].ToString() != "" && dr["SoKienHienThi"].ToString() != "" && (soLotValue == DBNull.Value || string.IsNullOrWhiteSpace(soLotValue.ToString())))
                    //    {
                    //        //XtraMessageBox.Show("Vui lòng nhập đầy đủ LoT.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //        //return;
                    //    }
                    //}
                }
                string allErrors = CheckTrungRollLotBatch(tblNhapKho);
                if (!string.IsNullOrEmpty(allErrors))
                {
                    MessageBox.Show(allErrors, "Thông báo dòng trùng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //var dsTrung = LayDanhSachTrungTheoID(tblNhapKho, tblNL);

                //if (dsTrung.Any())
                //{
                //    var danhSachTrung = dsTrung
                //     .Select(row => string.Format(
                //         "Mã nhóm: {0} ,Mã vật tư: {1} ,ItemCode: {2} ,Chi tiết: {3}, Màu: {4}, Khổ/Size: {5}, Số Roll: {6}",
                //         row.TenNhom, row.MaVTGhep, row.MaVT, row.ChiTiet, row.MauVT, row.KhoVai, row.SoKienHienThi))
                //     .Distinct() // Loại bỏ trùng, vì 1 nhóm trùng có thể có >2 dòng giống hệt nhau
                //     .ToList();

                //    string thongBao = "Các dòng trùng:\n\n" + string.Join("\n", danhSachTrung);
                //    MessageBox.Show(thongBao, "Thông báo dòng trùng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}
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
                if (tblNhapKho.Columns.Contains("STT"))
                {
                    tblNhapKho.Columns.Remove("STT");
                }
                if (tblNhapKho.Columns.Contains("IsNK"))
                {
                    tblNhapKho.Columns.Remove("IsNK");
                }

                foreach (DataRow dr in tblNhapKho.Rows)
                {
                    dr["SoloID"] = _soloid;
                    //dr["NgayNKDuKien"] = dateNgayNK.EditValue == null ? (object)DBNull.Value : dateNgayNK.EditValue;
                    if (string.IsNullOrWhiteSpace(dr["DonGia"].ToString()))
                    {
                        dr["DonGia"] = 0;
                    }
                    dr["POMua"] = txtPOMua.Text.ToString();
                    dr["IsKiemKe"] = cbKiemKe.Checked;
                    dr["NgayKiemKe"] = ngayKK.EditValue == null ? (object)DBNull.Value : (DateTime)ngayKK.EditValue;
                }
                string url = $"{URL}ERPNhapKhoNPL/Post?Action=POSTCT&para={GlobleData.UserName}&para2={isNK}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblNhapKho); }).Result;
                if (msResult.ToLower() == "true")
                {
                    setTxtNull();
                    setGVNull();
                    huySelect();
                    clsWaitForm.ShowSuccessForm(this, 3000);
                }
            }
            catch (Exception ex)
            {


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
                    XtraMessageBox.Show("Không có kiện vật tư được thêm. Vui lòng kiểm tra lại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                //DataTable hehe = gCNLNhapKho.DataSource as DataTable;

                foreach (DataRow dr2 in tblNhapKho.Rows)
                {
                    var barCode = dr2.Field<string>("BarCode");
                    if (!string.IsNullOrWhiteSpace(barCode))
                    {
                        var segments = barCode.Split('|');
                        if (segments.Length > 0 && !string.IsNullOrWhiteSpace(segments[segments.Length - 1]))
                        {
                            if (dr2["SoKienHienThi"].ToString() != "" && Convert.ToDouble(dr2["SoGhiDauCay"].ToString()) <= 0 && dr2["SoGhiDauCay"].ToString() == "")
                            {
                                XtraMessageBox.Show("Vui lòng nhập Số lượng theo CT lớn hơn 0", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }
                    //if (tblNhapKho.Columns.Contains("SoLot"))
                    //{
                    //    var soLotValue = dr2["SoLot"];
                    //    if ((soLotValue == DBNull.Value || string.IsNullOrWhiteSpace(soLotValue.ToString())) && dr2["Barcode"].ToString() != "" && dr2["SoKienHienThi"].ToString() != "")
                    //    {
                    //        XtraMessageBox.Show("Vui lòng nhập đầy đủ LoT.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //        return false;
                    //    }
                    //}
                }

                string ngayNhap = (dateNgayNK.EditValue is DateTime d ? d : (DateTime.TryParse(Convert.ToString(dateNgayNK.EditValue), out var x) ? x : DateTime.Today)).ToString("ddMMyyyy");
                string mhct = searchLookUpEditMHCT.EditValue == null ? "00" : searchLookUpEditMHCT.EditValue.ToString();
                Random random = new Random();
                string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // Tập hợp ký tự có thể sử dụng
                string randomString = new string(Enumerable.Repeat(characters, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                if (!checkTxt()) return false;
                string kh = searchLookUpEditKHCT.EditValue == null ? "" : searchLookUpEditKHCT.EditValue.ToString();
                _soloid = RemoveVietnameseTone(ReplaceSpecialCharacters(txtLo.Text.ToString())) + "|" + ngayNhap + "|" + kh + "|" + mhct + "|" + randomString;
                if (!checkTrungLo())
                {
                    XtraMessageBox.Show("PI NCC bị trùng. Vui lòng kiểm tra lại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }


                if (tblPhieu == null || tblPhieu.Rows.Count == 0) CreateTablePhieu();
                if (tblPhieu.Rows.Count > 0) tblPhieu.Clear();

                DataRow dr = tblPhieu.NewRow();

                dr["ID"] = 0;
                dr["SoLoID"] = _soloid;
                dr["SoLo"] = txtLo.Text.ToString();
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
                dr["Tau"] = SearchlookupTau.EditValue;
                dr["NgayGui"] = dateNgayGui.EditValue == null ? (object)DBNull.Value : (DateTime)dateNgayGui.EditValue;
                dr["NoiGui"] = txtNoiGui.Text.ToString();
                dr["DiemDen"] = "";
                dr["NguoiNhan"] = DBNull.Value;
                dr["DiaChiNhan"] = DBNull.Value;
                dr["MaSoThue"] = txtMaSoThue.Text.ToString();
                dr["SoDienThoai"] = DBNull.Value;
                dr["Cang"] = searchLookUpEditCang.EditValue == null ? "" : searchLookUpEditCang.EditValue.ToString();
                dr["KhachHang"] = DBNull.Value;
                dr["NgayKyHD"] = dateSoHopDong.EditValue == null ? (object)DBNull.Value : (DateTime)dateSoHopDong.EditValue;
                dr["SoToKhai"] = txtSoToKhai.Text.ToString();
                dr["NgayMoTK"] = dateNgayToKhai.EditValue == null ? (object)DBNull.Value : (DateTime)dateNgayToKhai.EditValue;
                dr["SoVanDon"] = txtSoVanDon.Text.ToString();
                dr["NgayNKDuKien"] = dateNgayNK.EditValue == null ? (object)DBNull.Value : (DateTime)dateNgayNK.EditValue;
                dr["MaKH"] = searchLookUpEditKHCT.EditValue == null ? "" : searchLookUpEditKHCT.EditValue.ToString();
                dr["NgayHoaDon"] = dateHoaDon.EditValue == null ? (object)DBNull.Value : (DateTime)dateHoaDon.EditValue;
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
        private bool checkTrungLo()
        {
            string url = $"{URL}ERPNhapKhoNPL/Get?Action=GETTRUNGLO&para={_soloid}";
            //if (!_isNPL)
            //    url = $"{URL}ERPNhapKhoNPL/Get?Action=GETTRUNGLOPL&para={_soloid}";

            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return true;
            return false;
        }
        private void luuNhapKhoPL()
        {
            try
            {
                if (tblNhapKhoPL == null || tblNhapKhoPL.Rows.Count == 0) return;
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
                    return;
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
                                return;
                            }
                        }
                    }
                    //if (tblNhapKhoPL.Columns.Contains("SoLot"))
                    //{
                    //    var soLotValue = dr["SoLot"];
                    //    if (soLotValue == DBNull.Value || string.IsNullOrWhiteSpace(soLotValue.ToString()))
                    //    {
                    //        XtraMessageBox.Show("Vui lòng nhập đầy đủ LoT.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //        return;
                    //    }
                    //}
                }
                //if (KiemTraTrungSoKienHienThi(tblNhapKhoPL))
                //{

                //    XtraMessageBox.Show("Trùng số roll. Vui lòng kiểm tra lại", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}

                string allErrors = CheckTrungRollLotBatch(tblNhapKhoPL);
                if (!string.IsNullOrEmpty(allErrors))
                {
                    MessageBox.Show(allErrors, "Thông báo dòng trùng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //var dsTrung = LayDanhSachTrungTheoID(tblNhapKhoPL, tblPL);

                //if (dsTrung.Any())
                //{
                //    var danhSachTrung = dsTrung
                //    .Select(row => string.Format(
                //        "Mã nhóm: {0} ,Mã vật tư: {1} ,ItemCode: {2} ,Chi tiết: {3}, Màu: {4}, Khổ/Size: {5}, Số Roll: {6}",
                //        row.TenNhom, row.MaVTGhep, row.MaVT, row.ChiTiet, row.MauVT, row.KhoVai, row.SoKienHienThi))
                //    .Distinct() // Loại bỏ trùng, vì 1 nhóm trùng có thể có >2 dòng giống hệt nhau
                //    .ToList();

                //    string thongBao = "Các dòng trùng:\n\n" + string.Join("\n", danhSachTrung);
                //    MessageBox.Show(thongBao, "Thông báo dòng trùng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
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
                if (tblNhapKhoPL.Columns.Contains("STT"))
                {
                    tblNhapKhoPL.Columns.Remove("STT");
                }
                if (tblNhapKhoPL.Columns.Contains("IsNK"))
                {
                    tblNhapKhoPL.Columns.Remove("IsNK");
                }

                foreach (DataRow dr in tblNhapKhoPL.Rows)
                {
                    dr["SoloID"] = _soloid;

                    dr["NgayNKDuKien"] = dateNgayNKPL.EditValue == null ? (object)DBNull.Value : dateNgayNKPL.EditValue;
                    if (string.IsNullOrWhiteSpace(dr["DonGia"].ToString()))
                    {
                        dr["DonGia"] = 0;
                    }
                    dr["POMua"] = txtPOMuaPL.Text.ToString();
                    dr["IsKiemKe"] = cbKiemKePL.Checked;
                    dr["NgayKiemKe"] = ngayKKPL.EditValue == null ? (object)DBNull.Value : (DateTime)ngayKKPL.EditValue;
                }
                string url = $"{URL}ERPNhapKhoNPL/Post?Action=POSTCT&para={GlobleData.UserName}&para2={isNK}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblNhapKhoPL); }).Result;
                if (msResult.ToLower() == "true")
                {
                    setTxtNull();
                    setGVNull();
                    huySelect();
                    clsWaitForm.ShowSuccessForm(this, 3000);
                }
            }
            catch (Exception ex)
            {


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
                    XtraMessageBox.Show("Không có kiện vật tư được thêm. Vui lòng kiểm tra lại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                foreach (DataRow dr2 in tblNhapKhoPL.Rows)
                {
                    var barCode = dr2.Field<string>("BarCode");
                    if (!string.IsNullOrWhiteSpace(barCode))
                    {
                        var segments = barCode.Split('|');
                        if (segments.Length > 0 && !string.IsNullOrWhiteSpace(segments[segments.Length - 1]))
                        {
                            if (dr2["SoKienHienThi"].ToString() != "" && Convert.ToDouble(dr2["SoGhiDauCay"].ToString()) <= 0)
                            {
                                XtraMessageBox.Show("Vui lòng nhập Số lượng theo CT lớn hơn 0", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }
                    //if (tblNhapKhoPL.Columns.Contains("SoLot"))
                    //{
                    //    var soLotValue = dr2["SoLot"];
                    //    if (soLotValue == DBNull.Value || string.IsNullOrWhiteSpace(soLotValue.ToString()))
                    //    {
                    //        XtraMessageBox.Show("Vui lòng nhập đầy đủ LoT.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //        return false;
                    //    }
                    //}
                }
                string ngayNhap = (dateNgayNKPL.EditValue is DateTime d ? d : (DateTime.TryParse(Convert.ToString(dateNgayNKPL.EditValue), out var x) ? x : DateTime.Today)).ToString("ddMMyyyy");
                string mhct = searchLookUpEditMHCTPL.EditValue == null ? "00" : searchLookUpEditMHCTPL.EditValue.ToString();
                Random random = new Random();
                string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // Tập hợp ký tự có thể sử dụng
                string randomString = new string(Enumerable.Repeat(characters, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                string kh = searchLookUpEditKHCTPL.EditValue == null ? "" : searchLookUpEditKHCTPL.EditValue.ToString();
                _soloid = RemoveVietnameseTone(ReplaceSpecialCharacters(txtLoPL.Text.ToString())) + "|" + ngayNhap + "|" + kh + "|" + mhct + "|" + randomString;
                if (!checkTrungLo())
                {
                    XtraMessageBox.Show("PI NCC bị trùng. Vui lòng kiểm tra lại!!!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (!checkTxt()) return false;
                if (tblPhieuPL == null || tblPhieuPL.Rows.Count == 0) CreateTablePhieuPL();
                if (tblPhieuPL.Rows.Count > 0) tblPhieuPL.Clear();

                DataRow dr = tblPhieuPL.NewRow();

                dr["ID"] = 0;
                dr["SoLoID"] = _soloid;
                dr["SoLo"] = txtLoPL.Text.ToString();
                dr["NhaCungCap"] = searchLookUpEditNCCPL.EditValue == null ? "" : searchLookUpEditNCCPL.EditValue.ToString();
                dr["SoChungTu"] = DBNull.Value;
                dr["NgayChungTu"] = DBNull.Value;
                dr["SoBienBan"] = DBNull.Value;
                dr["NgayBienBan"] = DBNull.Value;
                dr["SoHopDong"] = txtSoHopDongPL.Text.ToString();
                dr["NguoiNhap"] = GlobleData.UserName;
                dr["NgayNhap"] = DBNull.Value;
                dr["IsNPL"] = false;
                dr["MaHaiQuan"] = "";
                dr["MaKeToan"] = "";
                dr["NguoiGui"] = DBNull.Value;
                dr["NgayTao"] = DBNull.Value;
                dr["SoHoaDon"] = txtSoHoaDonPL.Text.ToString();
                dr["Tau"] = SearchlookupTauPL.EditValue;
                dr["NgayGui"] = dateNgayGuiPL.EditValue == null ? (object)DBNull.Value : (DateTime)dateNgayGuiPL.EditValue;
                dr["NoiGui"] = txtNoiGuiPL.Text.ToString();
                dr["DiemDen"] = "";
                dr["NguoiNhan"] = DBNull.Value;
                dr["DiaChiNhan"] = DBNull.Value;
                dr["MaSoThue"] = txtMaSoThuePL.Text.ToString();
                dr["SoDienThoai"] = DBNull.Value;
                dr["Cang"] = searchLookUpEditCangPL.EditValue == null ? "" : searchLookUpEditCangPL.EditValue.ToString();
                dr["KhachHang"] = DBNull.Value;
                dr["NgayKyHD"] = dateSoHopDongPL.EditValue == null ? (object)DBNull.Value : (DateTime)dateSoHopDongPL.EditValue;
                dr["SoToKhai"] = txtSoToKhaiPL.Text.ToString();
                dr["NgayMoTK"] = dateNgayToKhaiPL.EditValue == null ? (object)DBNull.Value : (DateTime)dateNgayToKhaiPL.EditValue;
                dr["SoVanDon"] = txtSoVanDonPL.Text.ToString();
                dr["NgayNKDuKien"] = dateNgayNKPL.EditValue == null ? (object)DBNull.Value : (DateTime)dateNgayNKPL.EditValue;
                dr["NgayHoaDon"] = dateHoaDonPL.EditValue == null ? (object)DBNull.Value : (DateTime)dateHoaDonPL.EditValue;
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
        private object ValidateDecimal(TextEdit txt, string fieldName)
        {
            string val = txt.Text.Trim();

            if (string.IsNullOrWhiteSpace(val))
                return DBNull.Value;

            if (!decimal.TryParse(val, out decimal number))
            {
                XtraMessageBox.Show($"{fieldName} không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            if (number < 0)
            {
                XtraMessageBox.Show($"{fieldName} phải >= 0.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return number;
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

                duplicatesMsg.Add(
                    $"Vật tư: {infoVatTu}\n" +
                    $"Roll/Lot/Batch bị trùng: {actualRoll}/{actualLot}/{actualBatch}"
                );

            }
            if (duplicatesMsg.Count > 0)
            {
                return "Roll và Lot/Batch bị trùng lặp:\n" + string.Join("\n", duplicatesMsg);
            }
            return string.Empty;
        }
        private void txtLoPL_Validating(object sender, CancelEventArgs e)
        {
            string url = $"{URL}ERPNhapKhoNPL/Get?Action=GETLO";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0) return;
            var textEdit = sender as DevExpress.XtraEditors.TextEdit;
            string inputValue = textEdit.Text.Trim();


            foreach (DataRow dr in tbl.Rows)
            {
                if (RemoveVietnameseTone(ReplaceSpecialCharacters(txtLoPL.Text.ToString())) == dr["SoLoID"].ToString())
                {
                    MessageBox.Show("PI NCC đã tồn tại, vui lòng nhập giá trị khác!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }
            }

        }

        private void setTypeTxtEdit(object sender, KeyPressEventArgs e)
        {

            TextEdit textEdit = sender as TextEdit;
            string currentText = textEdit.Text;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ngăn không cho nhập ký tự không hợp lệ
                return;
            }
            if (currentText.Contains("."))
            {
                int indexOfDot = currentText.IndexOf('.');
                string decimalPart = currentText.Substring(indexOfDot + 1);
                if (decimalPart.Length >= 2 && e.KeyChar != '\b') // '\b' là phím Backspace
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



        private void setTxtReadOnly(bool isReadOnly)
        {
            if (_isNPL)
            {
                txtLo.Properties.ReadOnly = isReadOnly;
                searchLookUpEditNCC.Properties.ReadOnly = isReadOnly;
                txtSoToKhai.Properties.ReadOnly = isReadOnly;
                dateNgayToKhai.Properties.ReadOnly = isReadOnly;
                txtSoHopDong.Properties.ReadOnly = isReadOnly;
                dateSoHopDong.Properties.ReadOnly = isReadOnly;

                txtSoHoaDon.Properties.ReadOnly = isReadOnly;
                searchLookUpEditCang.Properties.ReadOnly = isReadOnly;
                SearchlookupTau.Properties.ReadOnly = isReadOnly;
                dateNgayGui.Properties.ReadOnly = isReadOnly;
                txtNoiGui.Properties.ReadOnly = isReadOnly;
                txtPOMua.Properties.ReadOnly = isReadOnly;
                txtSoVanDon.Properties.ReadOnly = isReadOnly;
                searchLookUpEditNL.Properties.ReadOnly = isReadOnly;
            }
            else
            {
                txtLoPL.Properties.ReadOnly = isReadOnly;
                searchLookUpEditNCCPL.Properties.ReadOnly = isReadOnly;
                txtSoToKhaiPL.Properties.ReadOnly = isReadOnly;
                dateNgayToKhaiPL.Properties.ReadOnly = isReadOnly;
                txtSoHopDongPL.Properties.ReadOnly = isReadOnly;
                dateSoHopDongPL.Properties.ReadOnly = isReadOnly;
                txtSoHoaDonPL.Properties.ReadOnly = isReadOnly;
                searchLookUpEditCangPL.Properties.ReadOnly = isReadOnly;
                SearchlookupTauPL.Properties.ReadOnly = isReadOnly;
                dateNgayGuiPL.Properties.ReadOnly = isReadOnly;
                txtNoiGuiPL.Properties.ReadOnly = isReadOnly;
                txtSoVanDonPL.Properties.ReadOnly = isReadOnly;
                searchLookUpEditPL.Properties.ReadOnly = isReadOnly;
                txtPOMuaPL.Properties.ReadOnly = isReadOnly;
            }
        }

        #endregion

        #region popupSearchLookUpEdit
        private void searchLookUpEditNCC_Popup(object sender, EventArgs e)
        {
            var popupControl = sender as DevExpress.Utils.Win.IPopupControl;
            if (popupControl == null) return;
            var popupForm = popupControl.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
            if (popupForm == null) return;
            var searchLookUpPopup = popupForm.Controls.OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>().FirstOrDefault();
            if (searchLookUpPopup == null) return;
            var layout = searchLookUpPopup.Controls.OfType<LayoutControl>().FirstOrDefault();
            if (layout == null) return;
            AddCustomButtonToPopupLayout(layout, "btnKhaiBao", "Khai báo Nhà cung cấp", khaiBaoNCCButton_Click);
        }



        private void searchLookUpEditNCCPL_Popup(object sender, EventArgs e)
        {
            var popupControl = sender as DevExpress.Utils.Win.IPopupControl;
            if (popupControl == null) return;
            var popupForm = popupControl.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
            if (popupForm == null) return;
            var searchLookUpPopup = popupForm.Controls.OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>().FirstOrDefault();
            if (searchLookUpPopup == null) return;
            var layout = searchLookUpPopup.Controls.OfType<LayoutControl>().FirstOrDefault();
            if (layout == null) return;
            AddCustomButtonToPopupLayout(layout, "btnKhaiBao", "Khai báo Nhà cung cấp", khaiBaoNCCButton_Click);
        }



        private void khaiBaoNCCButton_Click(object sender, EventArgs e)
        {
            frmERPNhaCungCapNK frm = new frmERPNhaCungCapNK();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            CreateSearchLookUpNCC();
        }

        /*
        private void khaiBaoKHButton_Click(object sender, EventArgs e)
        {
            frmERPKhachHangNK frm = new frmERPKhachHangNK();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            CreateSearchLookUpKH();
        }*/
        private void searchLookUpEditCang_Popup(object sender, EventArgs e)
        {
            var popupControl = sender as DevExpress.Utils.Win.IPopupControl;
            if (popupControl == null) return;
            var popupForm = popupControl.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
            if (popupForm == null) return;
            var searchLookUpPopup = popupForm.Controls.OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>().FirstOrDefault();
            if (searchLookUpPopup == null) return;
            var layout = searchLookUpPopup.Controls.OfType<LayoutControl>().FirstOrDefault();
            if (layout == null) return;
            AddCustomButtonToPopupLayout(layout, "btnKhaiBao", "Khai báo Cảng", khaiBaoCangButton_Click);
        }


        private void searchLookUpEditCangPL_Popup(object sender, EventArgs e)
        {
            var popupControl = sender as DevExpress.Utils.Win.IPopupControl;
            if (popupControl == null) return;
            var popupForm = popupControl.PopupWindow as DevExpress.XtraEditors.Popup.PopupSearchLookUpEditForm;
            if (popupForm == null) return;
            var searchLookUpPopup = popupForm.Controls.OfType<DevExpress.XtraGrid.Editors.SearchEditLookUpPopup>().FirstOrDefault();
            if (searchLookUpPopup == null) return;
            var layout = searchLookUpPopup.Controls.OfType<LayoutControl>().FirstOrDefault();
            if (layout == null) return;
            AddCustomButtonToPopupLayout(layout, "btnKhaiBao", "Khai báo Cảng", khaiBaoCangButton_Click);
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
            if (frm.ShowDialog() == DialogResult.OK)
            {
                string tentau = frm.getTau();
                SearchlookupTau.Text = tentau;
            }
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



        private void btnTauPL_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            frmERPNhapKhoNPL_ChonTau frm = new frmERPNhapKhoNPL_ChonTau();
            frm.StartPosition = FormStartPosition.CenterScreen;

            if (frm.ShowDialog() == DialogResult.OK)
            {
                string tentau = frm.getTau();
                SearchlookupTauPL.Text = tentau;
            }
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

        private void gVSearchPL_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn37)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
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

            if (!KiemTraCacCotDecimal(tbl))
            {
                XtraMessageBox.Show("File import có cột không đúng kiểu số. Vui lòng kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //if (!checkTrung(_dtSK, tbl)) return;
            ImportExcelToNhapKho(tbl);
            //     int tongkienExcel = tbl.Rows.Count;
            //     if (string.IsNullOrWhiteSpace(dr["TongKien"].ToString()))
            //     {
            //         dr["TongKien"] = tongkienExcel;
            //     }
            //     else
            //     {
            //         dr["TongKien"] = Convert.ToInt32(dr["TongKien"].ToString()) + tongkienExcel;

            //     }
            //     (_isNPL ? gVNL : gVPL).RefreshData();
            //     var slueKHCT = _isNPL ? searchLookUpEditKHCT : searchLookUpEditKHCTPL;
            //     var slueMHCT = _isNPL ? searchLookUpEditMHCT : searchLookUpEditMHCTPL;

            //     slueKHCT?.DoValidate();
            //     slueMHCT?.DoValidate();

            //     string mhct = slueMHCT?.EditValue?.ToString() ?? "00";
            //     string khctValue = slueKHCT?.EditValue?.ToString() ?? "";

            //     string maVTID = dr["MaVTID"].ToString();
            //     string MauVTID = dr["MauVTID"].ToString();
            //     string khoVaiID = dr["KhoVaiID"].ToString();
            //     string manhom = dr["MaNhom"].ToString();
            //     string MaVTGhep = dr["MaVTGhep"].ToString();
            //     string ngayNhap = (dateNgayNK.EditValue is DateTime d ? d : (DateTime.TryParse(Convert.ToString(dateNgayNK.EditValue), out var x) ? x : DateTime.Today)).ToString("ddMMyyyy");
            //     //string mhct = searchLookUpEditMHCT.EditValue == null ? "00" : searchLookUpEditMHCT.EditValue.ToString();
            //     Random random = new Random();
            //     string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // Tập hợp ký tự có thể sử dụng
            //     string randomString = new string(Enumerable.Repeat(characters, 6)
            //         .Select(s => s[random.Next(s.Length)]).ToArray());
            //     // _soloid = RemoveVietnameseTone(ReplaceSpecialCharacters(txtLo.Text.ToString())) + "|" + ngayNhap + "|" + searchLookUpEditKHCT.EditValue.ToString() + "|" + mhct + "|" + randomString;
            //     _soloid = RemoveVietnameseTone(ReplaceSpecialCharacters(txtLo.Text.ToString())) + "|" + ngayNhap + "|" + khctValue + "|" + mhct + "|" + randomString;
            //     string _manpl = RemoveVietnameseTone(ReplaceSpecialCharacters(dr["MaNhom"].ToString()) + "@" + ReplaceSpecialCharacters(dr["MaVTID"].ToString()) + "@" + ReplaceSpecialCharacters(dr["MauVTID"].ToString()) + "@" + ReplaceSpecialCharacters(dr["KhoVaiID"].ToString()));
            //     int sttChonVT = 1;
            //     if (dr.Table.Columns.Contains("STT") && dr["STT"] != DBNull.Value)
            //     {
            //         sttChonVT = Convert.ToInt32(dr["STTChonVT"]);
            //     }

            //     foreach (DataRow _drr in tbl.Rows)
            //     {
            //         DataRow _nr = (_isNPL ? tblNhapKho : tblNhapKhoPL).NewRow();

            //         _nr["ID"] = 0;
            //         _nr["SoLoID"] = _soloid;
            //         _nr["MaNPL"] = _manpl;
            //         _nr["MaNhom"] = manhom;
            //         _nr["MaVTGhep"] = MaVTGhep;
            //         _nr["MaVTID"] = dr["MaVTID"];
            //         _nr["MaMauVT"] = dr["MaMauVT"];
            //         _nr["MauVT"] = dr["MauVT"];
            //         _nr["SoKien"] = _sk;

            //         _nr["Batch"] = _drr[6];

            //         _nr["SoLot"] = _drr[7];
            //         _nr["MaHaiQuan"] = dr["MaHaiQuan"];
            //         _nr["MaKeToan"] = dr["MaKeToan"];
            //         _nr["SoGhiDauCay"] = _drr[8];
            //         _nr["NW"] = 0;
            //         _nr["GW"] =0;
            //         _nr["BarCode"] = "";
            //         _nr["GhiChu"] = _drr[9];
            //         _nr["IsNPL"] = _isNPL;
            //         _nr["KhoVaiID"] = dr["KhoVaiID"];
            //         _nr["SoKienParent"] = "";
            //         _nr["DonGia"] = dr["DonGia"];
            //         _nr["ThanhTien"] = 0;
            //         _nr["SoLuongThucTe"] = 0;
            //         _nr["Pallet"] = "";
            //         _nr["MaDVVT"] = dr["MaDVVT"];
            //         _nr["MauVTID"] = dr["MauVTID"];
            //         _nr["SoKienHienThi"] = _drr[5];
            //         _nr["MaDVCD"] = dr["MaDVVT"];
            //         _nr["TenDVCD"] = dr["TenDVVT"];
            //         _nr["TienTe"] = dr["TienTe"];
            //         _nr["STTChonVT"] = sttChonVT;
            //         _sk++;
            //         (_isNPL ? tblNhapKho : tblNhapKhoPL).Rows.Add(_nr);
            //     }
            //     DataTable tblNhapKhoFiltered = (_isNPL ? tblNhapKho : tblNhapKhoPL).Clone();
            //     var filteredRows = (_isNPL ? tblNhapKho : tblNhapKhoPL).AsEnumerable()
            //         .Where(row => row.Field<string>("MaVTID") == maVTID &&
            //                       row.Field<string>("MauVTID") == MauVTID &&
            //                       row.Field<string>("KhoVaiID") == khoVaiID &&
            //                       row.Field<string>("MaNhom") == manhom &&
            //                       row.Field<string>("MaVTGhep") == MaVTGhep);
            //     foreach (var row in filteredRows)
            //     {
            //         tblNhapKhoFiltered.ImportRow(row);
            //     }

            //(_isNPL ? gCNLNhapKho : gCPLNhapKho).DataSource = tblNhapKhoFiltered;
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
                string message = "Các dòng bị trùng đã bỏ qua:\n" + string.Join("\n", duplicates);
                MessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            int[] columnIndexes = { 8 }; //các cột theoCT, Trọng lượng, khối lượng
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
            dr["TongKien"] = tbl.Rows.Count;

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

            frmERPNhapKhoNPL_ChonVT frm = new frmERPNhapKhoNPL_ChonVT(tblNhapKho, true);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.OnCheck = (row) =>
            {
                AddRowGCNL(row);
                string key = $"{row["MaVTID"]}_{row["MauVTID"]}_{row["MaNhom"]}_{row["KhoVaiID"]}";
                int stt = Convert.ToInt32(row["STTChonVT"]);

                var matchingRows = tblNhapKho.AsEnumerable()
                    .Where(r => r["MaVTID"].ToString() == row["MaVTID"].ToString() &&
                               r["MauVTID"].ToString() == row["MauVTID"].ToString() &&
                               r["MaNhom"].ToString() == row["MaNhom"].ToString() &&
                               r["KhoVaiID"].ToString() == row["KhoVaiID"].ToString());

                foreach (var r in matchingRows)
                {
                    //r["STT"] = stt;
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
                        int oldSTT = Convert.ToInt32(r["STTChonVT"]);
                        //r["STT"] = oldSTT - 1;
                        r["STTChonVT"] = oldSTT - 1;
                    }
                    DataTable tblLeft = gCNL.DataSource as DataTable;
                    if (tblLeft != null)
                    {
                        var rowsToUpdateLeft = tblLeft.AsEnumerable()
                         .Where(r => r["STTChonVT"] != DBNull.Value && Convert.ToInt32(r["STTChonVT"]) > deletedSTT);
                        foreach (var r in rowsToUpdateLeft)
                        {
                            r["STTChonVT"] = Convert.ToInt32(r["STTChonVT"]) - 1;
                        }
                    }
                }
                gCNLNhapKho.DataSource = tblNhapKho;
                gVNLNhapKho.RefreshData();
                gCNL.RefreshDataSource();
            };
            if (frm.ShowDialog() == DialogResult.OK)
            {
                foreach (DataRow selectedRow in frm.lstSelect)
                {
                    string key = $"{selectedRow["MaVTID"]}_{selectedRow["MauVTID"]}_{selectedRow["MaNhom"]}_{selectedRow["KhoVaiID"]}";

                    DataRow rowInOriginal = tblNhapKho.AsEnumerable().FirstOrDefault(r =>
                        $"{r["MaVTID"]}_{r["MauVTID"]}_{r["MaNhom"]}_{r["KhoVaiID"]}" == key);

                    if (rowInOriginal != null)
                    {
                        rowInOriginal["STTChonVT"] = selectedRow["STTChonVT"];
                    }
                }

                gCNLNhapKho.DataSource = tblNhapKho;
                gCNLNhapKho.RefreshDataSource();
            }

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

        private void cbKiemKe_CheckedChanged(object sender, EventArgs e)
        {
            if (cbKiemKe.Checked)
            {
                layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            else
            {
                layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                ngayKK.EditValue = DBNull.Value;
            }
        }
        private void cbKiemKePL_CheckedChanged(object sender, EventArgs e)
        {
            if (cbKiemKePL.Checked)
            {
                layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            else
            {
                layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
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
        //Moi
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

            if (!tblPL.Columns.Contains("STTChonVT"))
            {
                tblPL.Columns.Add("STTChonVT", typeof(int));
            }

            foreach (DataRow drTbl in tblPL.Rows)
            {
                var matchingRows = tblNhapKhoPL.AsEnumerable()
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

            frmERPNhapKhoNPL_ChonVT frm = new frmERPNhapKhoNPL_ChonVT(tblNhapKhoPL, false);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.OnCheck = (row) =>
            {
                AddRowGCNL(row);
                string key = $"{row["MaVTID"]}_{row["MauVTID"]}_{row["MaNhom"]}_{row["KhoVaiID"]}";
                int stt = Convert.ToInt32(row["STTChonVT"]);

                var matchingRows = tblNhapKhoPL.AsEnumerable()
                    .Where(r => r["MaVTID"].ToString() == row["MaVTID"].ToString() &&
                               r["MauVTID"].ToString() == row["MauVTID"].ToString() &&
                               r["MaNhom"].ToString() == row["MaNhom"].ToString() &&
                               r["KhoVaiID"].ToString() == row["KhoVaiID"].ToString());

                foreach (var r in matchingRows)
                {
                    //r["STT"] = stt;
                    r["STTChonVT"] = stt;
                }
                gCPLNhapKho.DataSource = tblNhapKhoPL;
                gVPLNhapKho.RefreshData();
            };
            frm.OnUncheck = (row) =>
            {
                int deletedSTT = Convert.ToInt32(row["STTChonVT"]);
                Remove(row);
                if (deletedSTT > 0)
                {
                    var rowsToUpdate = tblNhapKhoPL.AsEnumerable()
                      .Where(r => r["STTChonVT"] != DBNull.Value && Convert.ToInt32(r["STTChonVT"]) > deletedSTT);
                    foreach (var r in rowsToUpdate)
                    {
                        int oldSTT = Convert.ToInt32(r["STTChonVT"]);
                        //r["STT"] = oldSTT - 1;
                        r["STTChonVT"] = oldSTT - 1;
                    }
                    DataTable tblLeft = gCPL.DataSource as DataTable;
                    if (tblLeft != null)
                    {
                        var rowsToUpdateLeft = tblLeft.AsEnumerable()
                         .Where(r => r["STTChonVT"] != DBNull.Value && Convert.ToInt32(r["STTChonVT"]) > deletedSTT);
                        foreach (var r in rowsToUpdateLeft)
                        {
                            r["STTChonVT"] = Convert.ToInt32(r["STTChonVT"]) - 1;
                        }
                    }
                }
                gCPLNhapKho.DataSource = tblNhapKhoPL;
                gVPLNhapKho.RefreshData();
                gCPL.RefreshDataSource();
            };
            if (frm.ShowDialog() == DialogResult.OK)
            {
                foreach (DataRow selectedRow in frm.lstSelect)
                {
                    string key = $"{selectedRow["MaVTID"]}_{selectedRow["MauVTID"]}_{selectedRow["MaNhom"]}_{selectedRow["KhoVaiID"]}";

                    DataRow rowInOriginal = tblNhapKhoPL.AsEnumerable().FirstOrDefault(r =>
                        $"{r["MaVTID"]}_{r["MauVTID"]}_{r["MaNhom"]}_{r["KhoVaiID"]}" == key);

                    if (rowInOriginal != null)
                    {
                        rowInOriginal["STTChonVT"] = selectedRow["STTChonVT"];
                    }
                }

                gCPLNhapKho.DataSource = tblNhapKhoPL;
                gVPLNhapKho.RefreshData();
            }

        }


        #endregion
        /*#region Thêm kiện
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


        }
        #endregion*/
        private void btnSuaLo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string _lo = RemoveVietnameseTone(ReplaceSpecialCharacters((_isNPL ? txtLo : txtLoPL).Text.ToString()));
            frmERPNhapKhoNPLSua frm = new frmERPNhapKhoNPLSua(_lo, _isNPL);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
        }



        private void btnSuaCay_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string _lo = RemoveVietnameseTone(ReplaceSpecialCharacters((_isNPL ? txtLo : txtLoPL).Text.ToString()));
            frmERPNhapKhoNPLSuaCay frm = new frmERPNhapKhoNPLSuaCay(_lo, _isNPL);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
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

                    if (dr["MaVTID"].ToString() == _dr["MaVTID"].ToString() && dr["MauVTID"].ToString() == _dr["MauVTID"].ToString() && dr["MaNhom"].ToString() == _dr["MaNhom"].ToString() && dr["MaVTGhep"].ToString() == _dr["MaVTGhep"].ToString()
                            && dr["KhoVaiID"].ToString() == _dr["KhoVaiID"].ToString())
                    {
                        _dr["MaDVCD"] = _lst[0];
                        _dr["TenDVCD"] = _lst[1];
                    }

                }
            }
        }
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
            //frmERPNhapKhoNPL_KhoiLuong frm = new frmERPNhapKhoNPL_KhoiLuong(_mdv, _dv, _tileNW, _tileGW);
            //frm.StartPosition = FormStartPosition.CenterScreen;
            //frm.ShowDialog();
            //if (frm.DialogResult == DialogResult.OK)
            //{
            //    List<decimal> _lst = frm.GetKhoiLuong();
            //    DataRow dr = (_isNPL ? gVNL : gVPL).GetFocusedDataRow();
            //    DataTable tbl = (_isNPL ? gCNLNhapKho.DataSource as DataTable : gCPLNhapKho.DataSource as DataTable);
            //    DataTable tblNK = (_isNPL ? tblNhapKho : tblNhapKhoPL);

            //    foreach (DataRow drA in tbl.AsEnumerable())
            //    {
            //        decimal soGhiDauCay = decimal.TryParse(drA["SoGhiDauCay"]?.ToString(), out var val) ? val : 0m;
            //        decimal _nw = _lst[0] * soGhiDauCay;
            //        decimal _gw = _nw + _lst[1];
            //        drA["tileNW"] = _lst[0];
            //        drA["tileGW"] = _lst[1];
            //        drA["NW"] = _nw;
            //        drA["GW"] = _gw;
            //    }
            //    foreach (DataRow _dr in tblNK.Rows)
            //    {

            //        if (dr["MaVTID"].ToString() == _dr["MaVTID"].ToString() && dr["MauVTID"].ToString() == _dr["MauVTID"].ToString() && dr["KhoVaiID"].ToString() == _dr["KhoVaiID"].ToString()
            //          )
            //        {
            //            decimal soGhiDauCay = decimal.TryParse(_dr["SoGhiDauCay"].ToString(), out var val) ? val : 0m;
            //            decimal _nw = _lst[0] * soGhiDauCay;
            //            decimal _gw = _nw + _lst[1];
            //            _dr["tileNW"] = _lst[0];
            //            _dr["tileGW"] = _lst[1];
            //            _dr["NW"] = _nw;
            //            _dr["GW"] = _gw;
            //        }
            //    }
            //}
        }

   

        /*auto with và scroll*/

        private void BestFitCol(GridView view)
        {
            if (view == null || view.VisibleColumns.Count < 4) return;

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
            { }
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.PDF) | *.PDF";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sourceFileName = openFileDialog.FileName;
                var fileName = ReplaceSpecialCharacterssize(Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName));

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
        private async void btnPDF_DoubleClick(object sender, EventArgs e)
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
                    nr["SoLoID"] = _soloid.ToString() ?? "";
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
                    rowEInvoice["SoHoaDon"] =txtSoHoaDon.EditValue;
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
                        if(xtraTabControl1.SelectedTabPage == tabNL)
                        {
                            searchLookUpEditCang.Properties.DataSource = tblCang;
                            searchLookUpEditCang.EditValue = maCang;
                        }
                        else if(xtraTabControl1.SelectedTabPage == tabPL)
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