using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.Kho;
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
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Spreadsheet;
using DevExpress.XtraPrinting;
using System.Web;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmPhanTichBom_XinNPL : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _checkUserName = false;
        bool _allowDuyet = false, _allowHuyDuyet = false, _allowActive = false;
        private int _rowhandle = 0, _stt = 0;
        private bool isAdd = false;
        DataTable _dtSize;
        private string _madot = string.Empty;
        DataTable _tblmhvt;
        private bool isReset = false;
        private bool _isDuyet = false;
        private bool _isActiveDot = false;
        DataTable _tblMauSP;
        DataTable tblMauVT = new DataTable();
        DataTable tblChungLoaiChiTiet = new DataTable();
        DataTable tblSizeChung = new DataTable();
        DataTable _tblKhoSize = new DataTable();
        DataTable tblAllSize = new DataTable();
        DataTable tblSizeKTTK = new DataTable();
        int _sttVT = 1;
        int _sttVTPL = 1;
        int maxSTTPhieu = 1;
        string _maphieu = string.Empty;
        string _mamauchung = string.Empty;

        string _makhTQ = string.Empty, _mahangTQ = string.Empty, _maDotTQ = string.Empty, _maPhieuTQ = string.Empty;
        bool _PQXacNhan = false;
        bool _PQSua = false;
        bool _PQXoa = false;

        bool _PQSoatXet = false;
        bool _PQDuyet = false;
        private List<DataRow> selectedRows = new List<DataRow>();
        private List<DataRow> selectedRowsSize = new List<DataRow>();
        private Dictionary<string, bool> cellsToHighlight = new Dictionary<string, bool>();
        private DataTable tblMaLenh;
        public frmPhanTichBom_XinNPL()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CheckPerminsion();
            _dtSize = new DataTable();
            _tblMauSP = new DataTable();
            tblMaLenh = new DataTable();
        }

        public frmPhanTichBom_XinNPL(string makh, string mahang, string MaDot, string MaPhieu)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CheckPerminsion();

            _dtSize = new DataTable();
            _tblMauSP = new DataTable();
            tblMaLenh = new DataTable();
            this._makhTQ = makh;
            this._mahangTQ = mahang;
            this._maDotTQ = MaDot;
            this._maPhieuTQ = MaPhieu;
            //bandedGridView1.RowCellStyle += bandedGridView1_RowCellStyle;
            Dictionary<string, bool> cellsToHighlight = new Dictionary<string, bool>();

        }
        protected override void OnLoad(EventArgs e)
        {
            //CheckUserXetDuyet();
            CreateRepoSearchLookUpChungLoaiCT();
            loadKhoSizeBOM();
            CheckPerminsionEdit();
            LoadTVMauVT();
            CreateSearchLookup();

            CreateRepoSearchLookUpMaLenh();

            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Delete))
            {
                deleteCell();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CheckPerminsionEdit()
        {

            string url = string.Format("{0}?userID={1}", URL + "PhanQuyenYCNPL/Get", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                    return;

                _PQXacNhan = Convert.ToBoolean(tbl.Rows[0]["AllowAdd"].ToString());

                _PQSua = Convert.ToBoolean(tbl.Rows[0]["AllowEdit"].ToString());

                _PQXoa = Convert.ToBoolean(tbl.Rows[0]["AllowDelete"].ToString());

                _PQSoatXet = Convert.ToBoolean(tbl.Rows[0]["AllowSoatXet"].ToString());

                _PQDuyet = Convert.ToBoolean(tbl.Rows[0]["AllowDuyet"].ToString());
            }
            if (!_PQSua)
            {
                bandedGridView1.OptionsBehavior.Editable = false;
                bandedGridView1.OptionsBehavior.ReadOnly = true;
            }


        }
        private void CreateRepoSearchLookUpChungLoaiCT()
        {
            try
            {
                repoSearchCLCT.DisplayMember = "TenNhomChiTiet";
                repoSearchCLCT.ValueMember = "MaNhomChiTiet";

                string url = $"{URL}XinNPLMayMau/Get?action=GETCHUNGLOAICHITIET";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                tblChungLoaiChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
                repoSearchCLCT.DataSource = tblChungLoaiChiTiet;


            }
            catch (Exception ex)
            {
            }
        }
        private void CreateSearchLookup()
        {
            try
            {
                string urlKH = string.Format("{0}?", URL + "PhanTichBom/GetKH");
                string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
                DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
                searchLookUpEditKH.Properties.DataSource = tblKH;
                searchLookUpEditKH.Properties.ValueMember = "MaKH";
                searchLookUpEditKH.Properties.DisplayMember = "TenKH";

                if (!string.IsNullOrEmpty(_makhTQ))
                {
                    searchLookUpEditKH.EditValue = _makhTQ;
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void CreateSearchLookUpDot(bool isSua = false)
        {
            searchLookUpEditDot.Properties.DisplayMember = "Dot";
            searchLookUpEditDot.Properties.ValueMember = "MaDot";

            string url = string.Format("{0}?makh={1}&&mahang={2}", URL + "KhoiTaoDM/GetThongSoDot", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblDot = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditDot.Properties.DataSource = tblDot;
            if (tblDot == null || tblDot.Rows.Count == 0)
            {

                searchLookUpEditDot.EditValue = "";
                gridControl1.DataSource = new DataTable();

            }
            else
            {
                if (isSua) return;

                if (!string.IsNullOrEmpty(_maDotTQ))
                {
                    searchLookUpEditDot.EditValue = _maDotTQ;
                    return;
                }
                else
                {
                    searchLookUpEditDot.EditValue = tblDot.AsEnumerable()
                   .OrderByDescending(row =>
                   {
                       int.TryParse(row["MaDot"].ToString().Split('|').Last(), out int value);
                       return value;
                   })
                   .First()["MaDot"];
                }

                //btnCopyBOM.Enabled = true;
            }

        }
        //private void CheckUserXetDuyet()
        //{
        //    string url = $"{URL}XinNPLMayMau/Get?Action=CheckUserXetDuyet&para1={GlobleData.UserName}";
        //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //    if (json == "[]")
        //    {
        //        btnDuyetAll.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        //        btnHuyDuyetAll.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

        //    }
        //    else
        //    {
        //        DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
        //        if (tbl == null || tbl.Rows.Count == 0)
        //        {
        //            btnDuyetAll.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        //            btnHuyDuyetAll.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

        //            return;
        //        }
        //        _allowActive = tbl.Rows[0]["AllowActive"].ToString() == "True" ? true : false;
        //        _allowDuyet = tbl.Rows[0]["AllowAdd"].ToString() == "True" ? true : false;
        //        _allowHuyDuyet = tbl.Rows[0]["AllowEdit"].ToString() == "True" ? true : false;

        //        btnDuyetAll.Visibility = _allowDuyet ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
        //        btnHuyDuyetAll.Visibility = _allowHuyDuyet ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;

        //    }
        //}
        //private void LoadData()
        //{
        //    try
        //    {

        //        SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
        //        SplashScreenManager.Default.SetWaitFormCaption("Đang tải dữ liệu");
        //        //BestFitCol(bandedGridView1);

        //        string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
        //        string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
        //        string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
        //        string url = $"{URL}XinNPLMayMau/Get?action=GetKTBom&para1={makh.ToString()}&para2={mahang.ToString()}&para3={madot.ToString()}";
        //        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //        DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);


        //        loadAllSize(searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString());

        //        if (tbl.Rows.Count > 0)
        //        {
        //            //if (gridBandMauSP != null)
        //            //    AppendColumnsFromBand(tbl, gridBandMauSP);
        //            ProcessSizeData(tbl);
        //            gridControl1.DataSource = tbl;
        //            bandedGridView1.FocusedRowHandle = _rowhandle;
        //            _isDuyet = tbl.AsEnumerable()
        //       .All(row => row["IsXetDuyet"]?.ToString() == "Đã duyệt");


        //            _sttVT = tbl.AsEnumerable()
        //              .Where(row => row.Field<bool>("NPL"))
        //              .Select(row => (int)row.Field<long>("STT"))
        //              .DefaultIfEmpty(0)
        //              .Max() + 1;

        //            _sttVTPL = tbl.AsEnumerable()
        //           .Where(row => !row.Field<bool>("NPL"))
        //           .Select(row => (int)row.Field<long>("STT"))
        //           .DefaultIfEmpty(0)
        //           .Max() + 1;


        //        }
        //        else
        //        {
        //            gridControl1.DataSource = null;

        //        }
        //        SplashScreenManager.CloseForm(false);
        //    }
        //    catch (Exception ex)
        //    {

        //        SplashScreenManager.CloseForm(false);
        //    }


        //}
        private void LoadData()
        {
            try
            {
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang tải dữ liệu");
                string maKH = searchLookUpEditKH.EditValue?.ToString() ?? "";
                string maHang = searchLookUpEditMH.EditValue?.ToString() ?? "";
                string maDot = searchLookUpEditDot.EditValue?.ToString() ?? "";

                loadAllSize(searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString());
                string urlMain = $"{URL}KhoiTaoBOMV1/Get?action=GETVTSP_V1&para1={maKH.ToString()}&para2={maHang.ToString()}&para3={maDot.ToString()}";

                string jsonMain = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMain); }).Result;
                DataTable dtMain = JsonConvert.DeserializeObject<DataTable>(jsonMain);

                if (dtMain.Rows.Count == 0)
                {
                    gridControl1.DataSource = null;
                    SplashScreenManager.CloseForm(false);
                    return;
                }

                // Query 2: Size data

                string urlSize = $"{URL}KhoiTaoBOMV1/Get?action=GETSIZESP_V1&para1={maKH.ToString()}&para2={maHang.ToString()}&para3={maDot.ToString()}";
                string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);

                // Query 3: Color mapping

                string urlColorMap = $"{URL}KhoiTaoBOMV1/Get?action=GETMAUSP_V1&para1={maKH.ToString()}&para2={maHang.ToString()}&para3={maDot.ToString()}";

                string jsonColorMap = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlColorMap); }).Result;
                DataTable dtColorMap = JsonConvert.DeserializeObject<DataTable>(jsonColorMap);

                // Query 4: All colors (for dynamic columns)

                string urlColors = $"{URL}KhoiTaoBOMV1/Get?action=GETBANGMAU_V1&para1={maKH.ToString()}&para2={maHang.ToString()}&para3={maDot.ToString()}";

                string jsonColors = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlColors); }).Result;
                DataTable dtColors = JsonConvert.DeserializeObject<DataTable>(jsonColors);

                DataTable dtResult = new DataTable();



                // Add ALL fixed columns
                dtResult.Columns.Add("ID", typeof(int));
                dtResult.Columns.Add("MaHang", typeof(string));
                dtResult.Columns.Add("MaKH", typeof(string));
                dtResult.Columns.Add("MaNhom", typeof(string));
                dtResult.Columns.Add("TenNhom", typeof(string));
                dtResult.Columns.Add("NPL", typeof(bool));
                dtResult.Columns.Add("Sort", typeof(int));
                dtResult.Columns.Add("MaVTID", typeof(string));
                dtResult.Columns.Add("MaVT", typeof(string));
                dtResult.Columns.Add("ChiTiet", typeof(string));
                dtResult.Columns.Add("MaDVVT", typeof(string));
                dtResult.Columns.Add("TenDVVT", typeof(string));
                dtResult.Columns.Add("MaMauVT", typeof(string));
                dtResult.Columns.Add("MauVT", typeof(string));
                dtResult.Columns.Add("GhiChu", typeof(string));
                dtResult.Columns.Add("STT", typeof(long));
                dtResult.Columns.Add("KhoVaiID", typeof(string));
                dtResult.Columns.Add("KhoVai", typeof(string));
                dtResult.Columns.Add("TachMau", typeof(bool));
                dtResult.Columns.Add("DinhMucHaoHut", typeof(decimal));
                dtResult.Columns.Add("DinhMucChung", typeof(decimal));
                dtResult.Columns.Add("IsXetDuyet", typeof(string));
                dtResult.Columns.Add("MaCode", typeof(string));
                dtResult.Columns.Add("STTCode", typeof(int));
                dtResult.Columns.Add("IsActive", typeof(bool));
                dtResult.Columns.Add("MaNhomChiTiet", typeof(string));
                dtResult.Columns.Add("MauVTIDChung", typeof(string));
                dtResult.Columns.Add("MaSizeChung", typeof(string));
                dtResult.Columns.Add("Size", typeof(string));
                dtResult.Columns.Add("IsNew", typeof(int));

                // Add dynamic color columns
                var colorColumnMapping = new Dictionary<string, string>();
                foreach (DataRow colorRow in dtColors.Rows)
                {
                    string maMau = colorRow["MaMau"].ToString();
                    string tenMau = colorRow["TenMau"].ToString();
                    string columnName = $"{tenMau}@Mau@{maMau}";

                    if (!dtResult.Columns.Contains(columnName))
                    {
                        dtResult.Columns.Add(columnName, typeof(string));
                        colorColumnMapping.Add(columnName, maMau);
                    }
                }

                // ============ BƯỚC 3: GROUP DATA THEO MaNhom, MaVTID, KhoVaiID, STT ============

                // QUAN TRỌNG: Group theo MaNhom, MaVTID, KhoVaiID, STT
                // Các row có cùng 4 field này sẽ gộp lại thành 1 row
                // MauVTID khác nhau sẽ được nối thành chuỗi trong MauVTIDChung
                var groupedData = dtMain.AsEnumerable()
     .GroupBy(r => new
     {
         MaHang = r["MaHang"]?.ToString() ?? "",
         MaKH = r["MaKH"]?.ToString() ?? "",
         MaNhom = r["MaNhom"]?.ToString() ?? "",
         TenNhom = r["TenNhom"]?.ToString() ?? "",
         NPL = r["NPL"] != DBNull.Value ? Convert.ToBoolean(r["NPL"]) : false,
         Sort = r["Sort"] != DBNull.Value ? Convert.ToInt32(r["Sort"]) : 0,
         MaVTID = r["MaVTID"]?.ToString() ?? "",
         MaVT = r["MaVT"]?.ToString() ?? "",
         ChiTiet = r["ChiTiet"]?.ToString() ?? "",
         MaDVVT = r["MaDVVT"]?.ToString() ?? "",
         TenDVVT = r["TenDVVT"]?.ToString() ?? "",
         GhiChu = r["GhiChu"]?.ToString() ?? "",
         STT = r["STT"] != DBNull.Value ? Convert.ToInt64(r["STT"]) : 0L,
         KhoVaiID = r["KhoVaiID"]?.ToString() ?? "",
         IsKV = r["IsKV"] != DBNull.Value ? Convert.ToBoolean(r["IsKV"]) : false,
         KhoVai = r["KhoVai"]?.ToString() ?? "",
         MaTheSize = r["MaTheSize"]?.ToString() ?? "",
         TachMau = r["TachMau"] != DBNull.Value ? Convert.ToBoolean(r["TachMau"]) : false,
         DinhMucHaoHut = r["DinhMucHaoHut"] != DBNull.Value ? Convert.ToDecimal(r["DinhMucHaoHut"]) : 0m,
         DinhMucChung = r["DinhMucChung"] != DBNull.Value ? Convert.ToDecimal(r["DinhMucChung"]) : 0m,
         IsXetDuyet = r["IsXetDuyet"]?.ToString() ?? "",
         MaCode = r["MaCode"]?.ToString() ?? "",
         STTCode = r["STTCode"] != DBNull.Value ? Convert.ToInt32(r["STTCode"]) : 0,
         IsActive = r["IsActive"] != DBNull.Value ? Convert.ToBoolean(r["IsActive"]) : false,
         MaNhomChiTiet = r["MaNhomChiTiet"]?.ToString() ?? "",
         MaDot = r["MaDot"]?.ToString() ?? ""
     })
     .OrderBy(g => g.Key.Sort)
     .ThenBy(g => g.Key.STT);

                int id = 1;
                foreach (var group in groupedData)
                {
                    DataRow resultRow = dtResult.NewRow();

                    // Fill fixed columns từ group key
                    resultRow["ID"] = id++;
                    resultRow["MaHang"] = group.Key.MaHang;
                    resultRow["MaKH"] = group.Key.MaKH;
                    resultRow["MaNhom"] = group.Key.MaNhom;
                    resultRow["TenNhom"] = group.Key.TenNhom;
                    resultRow["NPL"] = group.Key.NPL;
                    resultRow["Sort"] = group.Key.Sort;
                    resultRow["MaVTID"] = group.Key.MaVTID;
                    resultRow["MaVT"] = group.Key.MaVT;
                    resultRow["ChiTiet"] = group.Key.ChiTiet;
                    resultRow["MaDVVT"] = group.Key.MaDVVT;
                    resultRow["TenDVVT"] = group.Key.TenDVVT;
                    //resultRow["MauVTIDChung"] = group.Key.MauVTIDChung;// tôi mới bổ sung
                    resultRow["MauVTIDChung"] = group.First()["MauVTIDChung"]?.ToString() ?? "";
                    //resultRow["MauVT"] = group.Key.MauVT;
                    resultRow["GhiChu"] = group.Key.GhiChu;
                    resultRow["STT"] = group.Key.STT;
                    resultRow["KhoVaiID"] = group.Key.KhoVaiID;
                    resultRow["KhoVai"] = group.Key.KhoVai;
                    resultRow["TachMau"] = group.Key.TachMau;
                    resultRow["DinhMucHaoHut"] = group.Key.DinhMucHaoHut;
                    resultRow["DinhMucChung"] = group.Key.DinhMucChung;
                    resultRow["IsXetDuyet"] = group.Key.IsXetDuyet;
                    resultRow["MaCode"] = group.Key.MaCode;
                    resultRow["STTCode"] = group.Key.STTCode;
                    resultRow["IsActive"] = group.Key.IsActive;
                    resultRow["MaNhomChiTiet"] = group.Key.MaNhomChiTiet;
                    resultRow["IsNew"] = 0;

                    // -------- XỬ LÝ MauVTIDChung: STUFF tất cả MauVTID trong group --------
                    var mauVTIDs = group
                        .Select(r => r["MauVTID"].ToString())
                        .Distinct()
                        .OrderBy(x => x);
                    //resultRow["MauVTIDChung"] = string.Join(", ", mauVTIDs);

                    // -------- XỬ LÝ Size Data --------
                    var sizeGroups = dtSize.AsEnumerable()
                        .Where(r => r["MaVTID"].ToString() == group.Key.MaVTID
                            && r["MaNhom"].ToString() == group.Key.MaNhom
                            && r["KhoVaiID"].ToString() == group.Key.KhoVaiID
                              && r["MaCode"].ToString() == group.Key.MaCode)
                        .GroupBy(r => new
                        {
                            MaNhomSize = r["MaNhomSize"].ToString(),
                            NhomSize = r["NhomSize"] != DBNull.Value ? r["NhomSize"].ToString() : r["MaNhomSize"].ToString()
                        })
                        .OrderBy(g => g.Key.MaNhomSize);

                    var maSizeChungParts = new List<string>();
                    var sizeChungParts = new List<string>();

                    foreach (var sizeGroup in sizeGroups)
                    {
                        var sizes = sizeGroup
                            .OrderBy(r => Convert.ToInt32(r["Sort"]))
                            .ThenBy(r => r["MaSize"].ToString())
                            .Select(r => r["MaSize"].ToString())
                            .Distinct();

                        var tenSizes = sizeGroup
                            .OrderBy(r => Convert.ToInt32(r["Sort"]))
                            .ThenBy(r => r["TenSize"] != DBNull.Value ? r["TenSize"].ToString() : r["MaSize"].ToString())
                            .Select(r => r["TenSize"] != DBNull.Value ? r["TenSize"].ToString() : r["MaSize"].ToString())
                            .Distinct();

                        maSizeChungParts.Add($"{sizeGroup.Key.MaNhomSize}: {string.Join(", ", sizes)}");
                        sizeChungParts.Add($"{sizeGroup.Key.NhomSize}: {string.Join(", ", tenSizes)}");
                    }

                    resultRow["MaSizeChung"] = string.Join("; ", maSizeChungParts);
                    resultRow["Size"] = string.Join("; ", sizeChungParts);

                    // -------- XỬ LÝ KhoVai/Size based on IsKV --------
                    if (!group.Key.IsKV)
                    {
                        var sizeInfo = dtSize.AsEnumerable()
                            .FirstOrDefault(r => r["MaSize"].ToString() == group.Key.KhoVaiID);

                        if (sizeInfo != null)
                        {
                            resultRow["KhoVaiID"] = sizeInfo["MaSize"].ToString();
                            resultRow["KhoVai"] = sizeInfo["TenSize"] != DBNull.Value
                                ? sizeInfo["TenSize"].ToString()
                                : group.Key.KhoVai;
                        }
                    }

                    // -------- XỬ LÝ PIVOT COLOR COLUMNS --------
                    // Initialize tất cả color columns = empty
                    foreach (var kvp in colorColumnMapping)
                    {
                        resultRow[kvp.Key] = "";
                    }

                    // Duyệt qua tất cả rows trong group để fill MauVTID vào đúng color column
                    // LẤY DANH SÁCH MauVTID của group này từ dtColorMap
                    var colorMappingsForGroup = dtColorMap.AsEnumerable()
                        .Where(r => r["MaVTID"].ToString() == group.Key.MaVTID
                            && r["MaNhom"].ToString() == group.Key.MaNhom
                            && r["KhoVaiID"].ToString() == group.Key.KhoVaiID
                             && r["MaCode"].ToString() == group.Key.MaCode)
                        .ToList();

                    // Duyệt qua từng mapping để fill vào đúng cột màu
                    foreach (DataRow colorMapping in colorMappingsForGroup)
                    {
                        string mauVTID = colorMapping["MauVTID"].ToString();
                        string maMau = colorMapping["MaMau"].ToString();

                        // Tìm column name tương ứng với MaMau này
                        var matchedColumn = colorColumnMapping.FirstOrDefault(x => x.Value == maMau);

                        if (!string.IsNullOrEmpty(matchedColumn.Key))
                        {
                            // Fill MauVTID vào column tương ứng
                            string currentValue = resultRow[matchedColumn.Key].ToString();
                            if (string.IsNullOrEmpty(currentValue))
                            {
                                resultRow[matchedColumn.Key] = mauVTID;
                            }
                            //else
                            //{
                            //    // Nếu đã có giá trị thì append (trường hợp 1 màu có nhiều MauVTID)
                            //    resultRow[matchedColumn.Key] = currentValue + ", " + mauVTID;
                            //}
                            // tôi mới comneent lại 
                        }
                    }

                    dtResult.Rows.Add(resultRow);
                }

                // ============ BƯỚC 4: LOAD VÀO GRID ============

                loadAllSize(maKH, maHang);
                ProcessSizeData(dtResult);
                gridControl1.DataSource = dtResult;
                bandedGridView1.FocusedRowHandle = _rowhandle;

                // ============ BƯỚC 5: XỬ LÝ CÁC GIÁ TRỊ KHÁC ============

                _isDuyet = dtResult.AsEnumerable()
                    .All(row => row["IsXetDuyet"]?.ToString() == "Đã xác nhận");


                _sttVT = dtResult.AsEnumerable()
                    .Where(row => row.Field<bool>("NPL"))
                    .Select(row => (int)row.Field<long>("STT"))
                    .DefaultIfEmpty(0)
                    .Max() + 1;

                _sttVTPL = dtResult.AsEnumerable()
                    .Where(row => !row.Field<bool>("NPL"))
                    .Select(row => (int)row.Field<long>("STT"))
                    .DefaultIfEmpty(0)
                    .Max() + 1;

                SplashScreenManager.CloseForm(false);
            }
            catch (Exception ex)
            {

                SplashScreenManager.CloseForm(false);
            }


        }

        private void loadAllSize(string _makh, string _mahang)
        {
            string url = $"{URL}XinNPLMayMau/Get?action=GETSIZESP&para1={_makh.ToString()}&para2={_mahang.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                tblAllSize = JsonConvert.DeserializeObject<DataTable>(json);
            }

        }
        private void ProcessSizeData(DataTable tbl)
        {

            HashSet<string> allMaSizeSet = new HashSet<string>();


            foreach (DataRow row in tblAllSize.Rows)
            {
                string maSize = row["MaSize"].ToString().Trim();
                if (!string.IsNullOrEmpty(maSize))
                {
                    allMaSizeSet.Add(maSize);
                    Console.WriteLine($"  - '{maSize}'");
                }
            }

            // Duyệt qua từng dòng trong tbl
            foreach (DataRow row in tbl.Rows)
            {
                string maSizeChung = row["MaSizeChung"].ToString();

                // Tách chuỗi MaSizeChung
                HashSet<string> maSizeSet = new HashSet<string>();

                // Tách theo dấu ";"
                string[] nhomSizes = maSizeChung.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string nhomSize in nhomSizes)
                {
                    // Tách theo dấu ":"
                    string[] parts = nhomSize.Split(':');
                    if (parts.Length == 2)
                    {
                        // Lấy phần sau dấu ":" và tách theo dấu ","
                        string[] maSizes = parts[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string maSize in maSizes)
                        {
                            string trimmedMaSize = maSize.Trim();
                            if (!string.IsNullOrEmpty(trimmedMaSize))
                            {
                                maSizeSet.Add(trimmedMaSize);
                                Console.WriteLine($"  + Thêm: '{trimmedMaSize}'");
                            }
                        }
                    }
                }

                Console.WriteLine($"\nTổng số size tìm được: {maSizeSet.Count}");

                // Tìm size thiếu
                var missingSizes = allMaSizeSet.Except(maSizeSet).ToList();
                if (missingSizes.Any())
                {
                    Console.WriteLine("Size thiếu trong MaSizeChung:");
                    foreach (var missing in missingSizes)
                    {
                        Console.WriteLine($"  - '{missing}'");
                    }
                }

                // Tìm size thừa
                var extraSizes = maSizeSet.Except(allMaSizeSet).ToList();
                if (extraSizes.Any())
                {
                    Console.WriteLine("Size thừa trong MaSizeChung (không có trong tblAllSize):");
                    foreach (var extra in extraSizes)
                    {
                        Console.WriteLine($"  - '{extra}'");
                    }
                }

                // So sánh với allMaSizeSet
                bool isAllSize = allMaSizeSet.SetEquals(maSizeSet);
                Console.WriteLine($"\nKết quả: IsAllSize = {isAllSize}");

                if (isAllSize)
                {
                    row["Size"] = "All Size";
                    Console.WriteLine("=> Đã cập nhật Size = 'All Size'");
                }
                Console.WriteLine("===============================\n");
            }
        }
        private void CreateDotSTT()
        {
            string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string url = $"{URL}XinNPLMayMau/Get?action=GETSTTDOT&para1={kh.ToString()}&para2={mh.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            _stt = Convert.ToInt32(tbl.Rows[0][0]) + 1;

            _madot = kh.ToString() + '_' + mh.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "|" + _stt;
        }

        private void searchLookUpEditKH_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string urlMH = string.Format("{0}?makh={1}", URL + "PhanTichBom/GetMH", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
                string jsonMH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMH); }).Result;
                DataTable tblMH = JsonConvert.DeserializeObject<DataTable>(jsonMH);
                searchLookUpEditMH.Properties.DataSource = tblMH;
                searchLookUpEditMH.Properties.ValueMember = "MaHang";
                searchLookUpEditMH.Properties.DisplayMember = "TenHang";


                if (!string.IsNullOrEmpty(_mahangTQ))
                {
                    searchLookUpEditMH.EditValue = _mahangTQ;
                    return;
                }



                if (tblMH.Rows.Count == 1)
                {
                    searchLookUpEditMH.EditValue = tblMH.Rows[0]["MaHang"];
                }
                else
                    searchLookUpEditMH.EditValue = null;





                //if (isAdd)
                //    CreateSearchLookUpVT();
            }
            catch (Exception ex)
            {

            }
        }

        private void searchLookUpEditMH_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                tgXacNhan.Toggled -= tgDuyet_Toggled;
                tgXacNhan.EditValueChanging -= tgDuyet_EditValueChanging;
                tgXacNhan.EditValue = false;
                tgXacNhan.Properties.Appearance.ForeColor = tgXacNhan.IsOn ? Color.ForestGreen : Color.DimGray;
                tgXacNhan.Toggled += tgDuyet_Toggled;
                tgXacNhan.EditValueChanging += tgDuyet_EditValueChanging;
                layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                emptySpaceItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                //emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                btnTaoPhieuXinNPL.Enabled = true;

                loadSizeChung();

                CreateSearchLookUpDot();

                CreateDotSTT();

                CreateMauSPColumns();

                loadSize();

            }
            catch (Exception ex)
            {

            }
        }






        private void searchLookUpEditKH_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            if (string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString()))
            {
                e.DisplayText = "Chọn khách hàng";
            }

        }

        private void NapLai(bool isSua = false)
        {
            isAdd = false;
            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

            emptySpaceItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            btnTaoPhieuXinNPL.Enabled = true;
            CreateSearchLookUpDot(isSua);
            CreateDotSTT();
            loadPhieuXinNPL();

            LoadData();
            loadMauPhieu();

            loadDataPhieu();

            CreateMauSPColumns();

        }
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLai(!isAdd);
        }

        private DataTable createTablesave()
        {
            DataTable _tblSave = new DataTable();
            _tblSave.Columns.Add("ID", typeof(int));
            _tblSave.Columns.Add("MaKH", typeof(string));
            _tblSave.Columns.Add("MaHang", typeof(string));
            _tblSave.Columns.Add("MaVTID", typeof(string));
            _tblSave.Columns.Add("MauID", typeof(string));
            _tblSave.Columns.Add("KhoVaiID", typeof(string));
            _tblSave.Columns.Add("MaDot", typeof(string));
            _tblSave.Columns.Add("Dot", typeof(string));
            _tblSave.Columns.Add("NguoiTao", typeof(string));
            _tblSave.Columns.Add("NgayTao", typeof(DateTime));
            _tblSave.Columns.Add("NguoiXet", typeof(string));
            _tblSave.Columns.Add("NgayXet", typeof(DateTime));
            _tblSave.Columns.Add("NguoiSua", typeof(string));
            _tblSave.Columns.Add("NgaySua", typeof(DateTime));
            _tblSave.Columns.Add("IsXetDuyet", typeof(int));
            _tblSave.Columns.Add("NguoiHuy", typeof(string));
            _tblSave.Columns.Add("NgayHuy", typeof(DateTime));
            return _tblSave;
        }





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
                barButtonItem7.Enabled = false;
            }
            if (!_allowEdit)
            {
            }

        }



        private void bandedGridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            if (info.Column != null && info.Column.FieldName == "NPL" && info.EditValue.ToString() != "")
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);

                if (isChecked)
                {
                    info.GroupText = "Nguyên liệu";
                }
                else
                {
                    info.GroupText = "Phụ liệu";
                }
            }

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            //if (info.Column == gridColumn24)
            //{
            //    info.GroupText = string.Format("{0}", info.GroupValueText);
            //}
            if (info.Column == gridColumn5)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column == gridColumn10)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (bandedGridView1.IsGroupRow(e.RowHandle))
            {


                int groupIndex = bandedGridView1.GetRowLevel(e.RowHandle);
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
                else if (groupIndex == 2)
                {
                    textColor = Color.Maroon;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }
                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }

        #region excel
        private void searchLookUpEditDot_EditValueChanged(object sender, EventArgs e)
        {
            tgXacNhan.Toggled -= tgDuyet_Toggled;
            tgXacNhan.EditValueChanging -= tgDuyet_EditValueChanging;
            tgXacNhan.EditValue = false;
            tgXacNhan.Properties.Appearance.ForeColor = tgXacNhan.IsOn ? Color.ForestGreen : Color.DimGray;
            tgXacNhan.Toggled += tgDuyet_Toggled;
            tgXacNhan.EditValueChanging += tgDuyet_EditValueChanging;


            //LoadSizeDM();
            CreatePhieuSTT();
            LoadData();
            loadPhieuXinNPL();
        }
        #endregion
        #region duyệt/ hủy duyệt all
        private void btnDuyetAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                return;
            if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                return;
            string url = string.Format("{0}?username={1}", URL + "KhoiTaoDM/KiemTraDuyet", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                XtraMessageBox.Show("User không có quyền Duyệt BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            duyetAll(1);
        }
        private DataTable createTableSaveEdit()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("TenNhom", typeof(string));
            tbl.Columns.Add("NPL", typeof(string));
            tbl.Columns.Add("Sort", typeof(int));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaVT", typeof(string));
            tbl.Columns.Add("ChiTiet", typeof(string));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("TenDVVT", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("MaMauVT", typeof(string));
            tbl.Columns.Add("MauVT", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("KhoVai", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("DinhMucChung", typeof(Decimal));
            tbl.Columns.Add("DinhMucHaoHut", typeof(Decimal));
            tbl.Columns.Add("TachMau", typeof(bool));
            tbl.Columns.Add("MaVTGhep", typeof(string));
            tbl.Columns.Add("IsNew", typeof(int));
            tbl.Columns.Add("STTCode", typeof(int));
            tbl.Columns.Add("MaCode", typeof(string));
            tbl.Columns.Add("IsActive", typeof(bool));
            tbl.Columns.Add("MaNhomChiTiet", typeof(string));
            tbl.Columns.Add("MauVTIDChung", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("MaSizeChung", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("STT", typeof(int));
            if (gridBandMauSP != null)
                AppendColumnsFromBand(tbl, gridBandMauSP);
            return tbl;
        }
        private void AppendColumnsFromBand(DataTable tbl, GridBand band)
        {
            foreach (BandedGridColumn column in band.Columns)
            {
                string fieldName = column.FieldName;
                if (!string.IsNullOrEmpty(fieldName) && !tbl.Columns.Contains(fieldName))
                    tbl.Columns.Add(fieldName, typeof(string));
            }
            foreach (GridBand childBand in band.Children)
                AppendColumnsFromBand(tbl, childBand);
        }





        private void btnHuyDuyetAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                return;
            if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                return;
            string url1 = string.Format("{0}?madot={1}", URL + "KhoiTaoDM/KiemTraDot", searchLookUpEditDot.EditValue.ToString());
            string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
            if (json1 != "[]")
            {
                //XtraMessageBox.Show("Đợt đã được cấp phát. Vui lòng kiểm tra lại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                //return;
            }
            string url = string.Format("{0}?username={1}", URL + "KhoiTaoDM/KiemTraHuy", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                XtraMessageBox.Show("User không có quyền hủy BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }

            duyetAll(0);
            NapLai();
        }
        private void duyetAll(int isduyet)
        {
            try
            {
                if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                    return;
                if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                    return;


                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang xử lý yêu cầu");
                string url = $"{URL}KhoiTaoDM/DuyetAll?makh={searchLookUpEditKH.EditValue.ToString()}&mahang={searchLookUpEditMH.EditValue.ToString()}&isduyet={isduyet}&username={GlobleData.UserName}&madot={searchLookUpEditDot?.EditValue?.ToString()}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "True")
                {
                    string userName = GlobleData.UserName ?? "";
                    string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                    string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                    string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                    string urlTS = $"{URL}XinNPLMayMau/Get?action=GET_TSNEW&para1={makh}&para2={mahang}&para6={userName}&para7={madot}";
                    string jsonTS = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTS); }).Result;


                    clsWaitForm.ShowSuccessForm(this, 2000);
                    if (SplashScreenManager.Default != null)
                    {
                        SplashScreenManager.CloseForm(false);
                    }

                    //LoadData();
                    NapLai(true);

                }
            }
            catch (Exception ex)
            {
                if (SplashScreenManager.Default != null)
                {
                    SplashScreenManager.CloseForm(false);
                }

            }

        }
        #endregion




        private void LoadSizeDM()
        {
            try
            {
                _dtSize = new DataTable();
                string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string url = $"{URL}XinNPLMayMau/Get?action=GETSIZEDM&para1={kh.ToString()}&para2={mh.ToString()}&para7={0}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _dtSize = JsonConvert.DeserializeObject<DataTable>(json);

                if (!_dtSize.Columns.Contains("AllSize"))
                {
                    _dtSize.Columns.Add("AllSize", typeof(float));
                }
                if (!_dtSize.Columns.Contains("CheckAll"))
                {
                    _dtSize.Columns.Add("CheckAll", typeof(bool)).DefaultValue = true;
                }
            }
            catch (Exception ex) { }
        }
        private void LoadSizeDMNew(DataRow _dr)
        {
            try
            {
                if (_dr == null) return;
                string maVTID = _dr["MaVTID"].ToString();
                string maMauVT = _dr["MauVTID"].ToString();
                string khoVaiID = _dr["KhoVaiID"].ToString();
                string MaNhom = _dr["MaNhom"].ToString();
                string MaCode = _dr["MaCode"].ToString();

                string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string url = $"{URL}XinNPLMayMau/Get?action=GETSIZEDM&para1={kh.ToString()}&para2={mh.ToString()}&para7={0}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable _dt = JsonConvert.DeserializeObject<DataTable>(json);
                foreach (DataRow dr in _dt.Rows)
                {


                    dr["MaMau"] = _dr["MaMau"].ToString();
                    dr["TenMau"] = _dr["TenMau"].ToString();

                }
                if (!_dt.Columns.Contains("AllSize"))
                {
                    _dt.Columns.Add("AllSize", typeof(float));
                }
                if (!_dt.Columns.Contains("CheckAll"))
                {
                    _dt.Columns.Add("CheckAll", typeof(bool)).DefaultValue = true;
                }
                if (_dr["TachMau"].ToString() != "True")
                {

                    foreach (DataRow dr in _dtSize.Rows)
                    {

                        if (dr["MaVTID"].ToString() == maVTID &&
                             dr["MauVTID"].ToString() == maMauVT &&
                             dr["KhoVaiID"].ToString() == khoVaiID &&
                             dr["MaNhom"].ToString() == MaNhom &&
                              dr["MaCode"].ToString() == MaCode)
                        {
                            dr["MaMau"] = _dr["MaMau"].ToString();
                            dr["TenMau"] = _dr["TenMau"].ToString();
                        }
                    }

                    foreach (DataRow row in _dt.Rows)
                    {

                        string maMau = row["MaMau"].ToString();
                        if (string.IsNullOrEmpty(maMau)) continue;
                        string manhomsize = row["MaNhomSize"].ToString();
                        // Gán giá trị vào row
                        row["MaVTID"] = maVTID;
                        row["MauVTID"] = maMauVT;
                        row["KhoVaiID"] = khoVaiID;
                        row["MaNhom"] = MaNhom;
                        row["MaCode"] = MaCode;

                        // Kiểm tra trùng
                        bool isDuplicate = _dtSize.AsEnumerable().Any(r =>
                            r["MaVTID"].ToString() == maVTID &&
                            r["MauVTID"].ToString() == maMauVT &&
                            r["KhoVaiID"].ToString() == khoVaiID &&
                            r["MaNhom"].ToString() == MaNhom &&
                            r["MaMau"].ToString() == maMau &&
                            r["MaNhomSize"].ToString() == manhomsize &&
                            r["MaCode"].ToString() == MaCode
                        );

                        if (!isDuplicate)
                        {
                            _dtSize.ImportRow(row);
                        }

                    }
                }
                else
                {
                    DataRow drF = bandedGridView1.GetFocusedDataRow();
                    string[] maMauArr = drF["MaMau"].ToString().Replace(" ", "").Split('|');
                    string[] tenMauArr = drF["TenMau"].ToString().Replace(" ", "").Split('|');
                    var rowsToDelete = _dtSize.AsEnumerable()
                           .Where(dr => string.IsNullOrEmpty(dr["MaMau"]?.ToString()) || !maMauArr.Contains(dr["MaMau"].ToString().Replace(" ", ""))
                                        && dr["MaVTID"].ToString() == drF["MaVTID"].ToString()
                                         && dr["MaUVTID"].ToString() == drF["MaUVTID"].ToString()
                                          && dr["KhoVaiID"].ToString() == drF["KhoVaiID"].ToString()
                                           && dr["MaNhom"].ToString() == drF["MaNhom"].ToString()
                                             && dr["MaCode"].ToString() == drF["MaCode"].ToString()
                                        )


                           .ToList();

                    foreach (var row in rowsToDelete)
                    {
                        _dtSize.Rows.Remove(row);
                    }
                    int indexTenMau = 0;
                    foreach (string maMau in maMauArr)
                    {
                        string mm = maMau.Trim();


                        foreach (DataRow row in _dt.Rows)
                        {
                            DataRow newRow = _dtSize.NewRow();

                            newRow.ItemArray = row.ItemArray.Clone() as object[];
                            newRow["MaVTID"] = drF["MaVTID"].ToString();
                            newRow["MaUVTID"] = drF["MaUVTID"].ToString();
                            newRow["KhoVaiID"] = drF["KhoVaiID"].ToString();
                            newRow["MaNhom"] = drF["MaNhom"].ToString();
                            newRow["MaMau"] = mm;
                            newRow["TenMau"] = tenMauArr[indexTenMau];
                            newRow["MaCode"] = drF["MaCode"].ToString();
                            newRow["MaNhomChiTiet"] = drF["MaNhomChiTiet"].ToString();
                            newRow["IsActive"] = drF["IsActive"];
                            bool exists = _dtSize.AsEnumerable().Any(dr =>
                               dr["MaMau"].ToString().Replace(" ", "") == mm &&
                               dr["MaVTID"].ToString() == newRow["MaVTID"].ToString() &&
                               dr["MaUVTID"].ToString() == newRow["MaUVTID"].ToString() &&
                               dr["KhoVaiID"].ToString() == newRow["KhoVaiID"].ToString() &&
                               dr["MaNhom"].ToString() == newRow["MaNhom"].ToString() &&
                                dr["MaNhomSize"].ToString() == newRow["MaNhomSize"].ToString() &&
                                dr["MaCode"].ToString() == newRow["MaCode"].ToString()
                           );
                            if (!exists)
                                _dtSize.Rows.Add(newRow);

                        }




                        indexTenMau++;
                    }

                }





                DataTable tblSizeFiltered = _dtSize.Clone();
                var filteredRows = _dtSize.AsEnumerable()
                .Where(row => row.Field<string>("MaVTID") == maVTID &&
                              row.Field<string>("MauVTID") == maMauVT &&
                              row.Field<string>("KhoVaiID") == khoVaiID &&
                              row.Field<string>("MaNhom") == MaNhom &&
                              row.Field<string>("MaCode") == MaCode);
                foreach (var row in filteredRows)
                {
                    tblSizeFiltered.ImportRow(row);
                }



            }

            catch (Exception ex)
            {

            }
        }


        private void repositoryItemButtonEditMauSP_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DataRow row = bandedGridView1.GetFocusedDataRow();
                if (row == null) return;
                if (row.Table.Columns.Contains("IsXetDuyet"))
                {
                    bool IsXetDuyet = false;
                    IsXetDuyet = row["IsXetDuyet"]?.ToString().ToLower() == "đã duyệt";
                    if (IsXetDuyet)
                    {
                        XtraMessageBox.Show("Vật tư đã được xét duyêt. Không thể chỉnh sửa màu sản phẩm. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }

                }
                frmMauSanPhamV1 frm = new frmMauSanPhamV1(searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), row["MaMau"].ToString(), row["TenMau"].ToString());
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.DialogResult != DialogResult.OK) return;
                List<KeyValuePair<string, string>> result = frm.GetSelectedDataAsList();

                if (result != null && result.Count > 0)
                {
                    row["MaMau"] = string.Join("|", result.Select(x => x.Value));
                    row["TenMau"] = string.Join("|", result.Select(x => x.Key));

                }
                else
                {
                    row["MaMau"] = "";
                    row["TenMau"] = "";
                }
                row["TachMau"] = true;
                this.ActiveControl = button1;
                LoadSizeDMNew(row);


            }
            catch (Exception ex)
            {


            }
        }






        private async void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Lưu phiếu


            try
            {
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang xử lý yêu cầu");
                SplashScreenManager.Default.SetWaitFormDescription("Đang chuẩn bị dữ liệu...");

                // Truy cập UI an toàn ở đây
                _rowhandle = bandedGridView1.FocusedRowHandle;
                string maPhieuPost = "";
                string tenPhieuPost = "";
                int IsSoatXet = 0;
                if (isAdd)
                {
                    if (string.IsNullOrEmpty(_maphieu))
                    {
                        string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                        string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                        string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                        string url = $"{URL}XinNPLMayMau/Get?action=GETMAXSTTPHIEU&para1={kh.ToString()}&para2={mh.ToString()}&para3={madot.ToString()}";
                        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                        if (json == "[]")
                        {
                            txtPhieuXinNPL.Text = "";
                            return;
                        }
                        DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                        maxSTTPhieu = Convert.ToInt32(tbl.Rows[0][0]) + 1;
                        txtPhieuXinNPL.Text = "Phiếu " + maxSTTPhieu;
                        _maphieu = kh.ToString() + '_' + mh.ToString() + '_' + madot + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "|" + maxSTTPhieu;

                    }
                    maPhieuPost = _maphieu;

                }
                else
                {
                    IsSoatXet = 1;
                    maPhieuPost = searchLookUpEditPhieuXinNPL.EditValue?.ToString() ?? "";
                    tenPhieuPost = searchLookUpEditPhieuXinNPL.Text.ToString();
                    if (string.IsNullOrEmpty(maPhieuPost))
                        maPhieuPost = _maphieu;
                }

                this.ActiveControl = button1;

                if (string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue?.ToString()) ||
                    string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue?.ToString()))
                {
                    SplashScreenManager.CloseForm();
                    return;
                }

                var tblGC1 = gridControl1.DataSource as DataTable;

                if (tblGC1 == null || tblGC1.Rows.Count == 0)
                {
                    SplashScreenManager.CloseForm();
                    return;
                }

                string _makh = searchLookUpEditKH.EditValue.ToString();
                string _mahang = searchLookUpEditMH.EditValue.ToString();
                string _malenh = searchLookUpEditMaLenh.EditValue?.ToString() ?? "";
                string dateStr = dateEditNgayYC.EditValue?.ToString() == null ? "" : dateEditNgayYC.DateTime.ToString("dd/MM/yyyy");

                // Thực thi logic nặng trên thread nền
                bool success = await Task.Run(async () =>
                {


                    ////SplashScreenManager.Default.SetWaitFormDescription("Đang lưu vật tư... (90%)");
                    //string url3 = $"{URL}XinNPLMayMau/Post3?Action=POSTVTSP&para1={GlobleData.UserName}&para2={madotpost}";
                    //string result3 = await _clientExtension.PostAsync(url3, dtVatTuSP);
                    //if (result3 != "True") return false;
                    //SplashScreenManager.Default.SetWaitFormDescription("Hoàn tất... (100%)");
                    var roomDisplayNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                                                                                            {
                                                                                                                { "MA_P1", "KTTK" },
                                                                                                                { "MA_P2", "TESTING" },
                                                                                                                { "MA_P3", "BẢNG MÀU" },
                                                                                                                { "MA_P4", "CƠ ĐIỆN" }
                                                                                                            };
                    DataTable dtMain = CreateTableSaveERPNPLMayMau();
                    DataTable dtChiTiet = CreateTableSaveERPNPLMayMauChiTiet();

                    // Lấy danh sách các cột phòng và màu (P1, P2, P3, P4)
                    //var colorRoomColumns = tblGC1.Columns.Cast<DataColumn>()
                    //    .Where(c => c.ColumnName.Contains("@MA_P") && c.ColumnName.Split('@').Length >= 2)
                    //    .Select(c => {
                    //        var parts = c.ColumnName.Split('@');
                    //        return new
                    //        {
                    //            Color = parts[2],           // ANTHRACITE, BLACK, ...
                    //            Room = parts[1],            // P1, P2, P3, P4
                    //            ColumnName = c.ColumnName
                    //        };
                    //    })
                    //    .ToList();

                    // Foreach từng dòng trong dtSource
                    foreach (DataRow row in tblGC1.Rows)
                    {

                        //var colorRoomColumns = tblGC1.Columns.Cast<DataColumn>()
                        //  // Lọc lấy các cột có định dạng @MA_P
                        //  .Where(c => c.ColumnName.Contains("@MA_P") && c.ColumnName.Split('@').Length >= 3)
                        //  .Select(c => {
                        //      var parts = c.ColumnName.Split('@');


                        //        string mauVTID = string.Empty;

                        //      if (row != null)
                        //      {
                        //            // 2. Tạo tên cột tham chiếu theo quy luật: {Prefix}@Mau@{Suffix}
                        //            // Ví dụ: BLACK@Mau@Mau_V8
                        //            string refColumnNamePattern = $"{parts[0]}@Mau@{parts[2]}";

                        //            // 3. Tìm cột thực tế trong DataTable (Bỏ qua hoa thường để khớp BLACK với Black)
                        //            var refColumn = tblGC1.Columns.Cast<DataColumn>()
                        //                          .FirstOrDefault(col => col.ColumnName.Equals(refColumnNamePattern, StringComparison.OrdinalIgnoreCase));

                        //            // 4. Nếu tìm thấy cột, lấy giá trị từ dòng targetRow
                        //            if (refColumn != null)
                        //          {
                        //              mauVTID = row[refColumn]?.ToString(); // Kết quả: Mau_293
                        //            }
                        //      }

                        //      return new
                        //      {
                        //          Color = parts[2],
                        //          Room = parts[1],
                        //          ColumnName = c.ColumnName,
                        //          MauVTID = mauVTID // Thuộc tính mới đã được ghép vào
                        //        };
                        //  })
                        //  .ToList();
                        var colorRoomColumns = tblGC1.Columns.Cast<DataColumn>()
                           // Lọc lấy các cột có định dạng @MA_P
                           .Where(c => c.ColumnName.Contains("@MA_P") && c.ColumnName.Split('@').Length >= 3)
                           .Select(c =>
                           {
                               var parts = c.ColumnName.Split('@');
                               string refColumnNamePattern = $"{parts[0]}@Mau@{parts[2]}";

                               // Kiểm tra column có trong grid không
                               var gridColumn = bandedGridView1.Columns
                                    .FirstOrDefault(col => col.FieldName.Equals(refColumnNamePattern, StringComparison.OrdinalIgnoreCase));

                               string mauVTID = string.Empty;
                               if (row != null && gridColumn != null && gridColumn.Visible)
                               {
                                   var refColumn = tblGC1.Columns.Cast<DataColumn>()
                                       .FirstOrDefault(col => col.ColumnName.Equals(refColumnNamePattern, StringComparison.OrdinalIgnoreCase));

                                   if (refColumn != null)
                                   {
                                       mauVTID = row[refColumn]?.ToString();
                                   }
                               }

                               return new
                               {
                                   Color = parts[2],
                                   Room = parts[1],
                                   ColumnName = c.ColumnName,
                                   MauVTID = mauVTID,
                                   IsVisible = gridColumn != null && gridColumn.Visible // Thêm flag để biết
                               };
                           })
                           .Where(x => !string.IsNullOrEmpty(x.MauVTID)) // Chỉ lấy những cái có MauVTID
                           .ToList();

                        // Xử lý Chi Tiết - foreach từng cột phòng và màu
                        foreach (var colInfo in colorRoomColumns)
                        {

                            //dtMain.Rows.Add(mainRow);
                            string maKH = row["MaKH"] == DBNull.Value ? "" : row["MaKH"].ToString();
                            string maHang = row["MaHang"] == DBNull.Value ? "" : row["MaHang"].ToString();
                            string maNhom = row["MaNhom"] == DBNull.Value ? "" : row["MaNhom"].ToString();
                            string maVTID = row["MaVTID"] == DBNull.Value ? "" : row["MaVTID"].ToString();
                            string mauVTID = colInfo.MauVTID == null ? "" : colInfo.MauVTID.ToString();
                            string khoVaiID = row["KhoVaiID"] == DBNull.Value ? "" : row["KhoVaiID"].ToString();
                            string maDot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                            string maCode = row["MaCode"] == DBNull.Value ? "" : row["MaCode"].ToString();
                            string maNhomChiTiet = row["MaNhomChiTiet"] == DBNull.Value ? "" : row["MaNhomChiTiet"].ToString();

                            bool exists = dtMain.AsEnumerable().Any(r =>
                                r["MaKH"].ToString() == maKH &&
                                r["MaHang"].ToString() == maHang &&
                                r["MaNhom"].ToString() == maNhom &&
                                r["MaVTID"].ToString() == maVTID &&
                                r["MauVTID"].ToString() == mauVTID &&
                                r["KhoVaiID"].ToString() == khoVaiID &&
                                r["MaDot"].ToString() == maDot &&
                                r["MaCode"].ToString() == maCode &&
                                r["MaNhomChiTiet"].ToString() == maNhomChiTiet
                            );

                            if (!exists)
                            {
                                DataRow mainRow = dtMain.NewRow();
                                mainRow["ID"] = 0;
                                mainRow["MaKH"] = maKH;
                                mainRow["MaHang"] = maHang;
                                mainRow["MaNhom"] = maNhom;
                                mainRow["MaVTID"] = maVTID;
                                mainRow["MauVTID"] = mauVTID;
                                mainRow["KhoVaiID"] = khoVaiID;
                                mainRow["MaDot"] = maDot;
                                mainRow["NguoiTao"] = Environment.UserName;
                                mainRow["NgayTao"] = DateTime.Now;
                                mainRow["STT"] = row["STT"];

                                double dinhMucChung = 0;
                                double dinhMucHaoHut = 0;
                                if (row["DinhMucChung"] != DBNull.Value)
                                    double.TryParse(row["DinhMucChung"].ToString(), out dinhMucChung);
                                if (row["DinhMucHaoHut"] != DBNull.Value)
                                    double.TryParse(row["DinhMucHaoHut"].ToString(), out dinhMucHaoHut);

                                mainRow["DinhMucChung"] = dinhMucChung;
                                mainRow["DinhMucHaoHut"] = dinhMucHaoHut;
                                mainRow["NguoiDuyet"] = "";
                                mainRow["NgayDuyet"] = DBNull.Value;
                                mainRow["IsDuyet"] = false;
                                mainRow["MaVTGhep"] = "";
                                mainRow["MaCode"] = maCode;
                                mainRow["STTCode"] = row["STTCode"];
                                mainRow["MaNhomChiTiet"] = maNhomChiTiet;
                                mainRow["IsActive"] = 0;

                                mainRow["GhiChu"] = row["GhiChu"];
                                mainRow["MaSizeChung"] = row["MaSizeChung"];

                                string size = "";
                                if (tblGC1.Columns.Contains("Size") && row["Size"] != DBNull.Value)
                                    size = row["Size"].ToString();
                                mainRow["Size"] = size;

                                mainRow["MaPhieu"] = maPhieuPost;
                                mainRow["STTPhieu"] = maxSTTPhieu;

                                dtMain.Rows.Add(mainRow);
                            }



                            decimal soLuong = 0;


                            // Xử lý số lượng: nếu null hoặc trống hoặc không parse được thì = 0
                            if (row[colInfo.ColumnName] != DBNull.Value)
                            {
                                string soLuongStr = row[colInfo.ColumnName].ToString().Trim();
                                if (!string.IsNullOrEmpty(soLuongStr))
                                {
                                    decimal.TryParse(soLuongStr, out soLuong);
                                }
                            }

                            var tenPhong = roomDisplayNames.TryGetValue(colInfo.Room, out var displayName)
                                ? displayName
                                : $"Phòng {colInfo.Room}";
                            // Luôn luôn lưu dòng, kể cả khi số lượng = 0
                            DataRow detailRow = dtChiTiet.NewRow();
                            detailRow["ID"] = 0;
                            detailRow["MaPhieu"] = maPhieuPost;
                            detailRow["MaVTID"] = row["MaVTID"];
                            detailRow["MauVTID"] = colInfo.MauVTID;
                            detailRow["KhoVaiID"] = row["KhoVaiID"];
                            detailRow["MaNhom"] = row["MaNhom"];
                            detailRow["MaCode"] = row["MaCode"];
                            detailRow["MaPhong"] = colInfo.Room;  // P1, P2, P3, P4
                            detailRow["TenPhong"] = tenPhong;
                            detailRow["MaMau"] = colInfo.Color;   // ANTHRACITE, BLACK, ...
                            detailRow["SoLuong"] = soLuong;  // Có thể = 0
                            detailRow["GhiChu"] = "";

                            dtChiTiet.Rows.Add(detailRow);
                        }
                    }
                    string maSizeKTTK = BuildMaNhomSizeString(selectedRows);

                    //SplashScreenManager.Default.SetWaitFormDescription("Đang lưu màu sản phẩm... (40%)");



                    string url1 = $"{URL}XinNPLMayMau/Post?Action=POST&para1={GlobleData.UserName}&para2={maSizeKTTK}&para3={IsSoatXet}&para4={txtGhiChu.Text.ToString()}&para5={_malenh}&para6={dateStr}";
                    string result1 = await _clientExtension.PostAsync(url1, dtMain);
                    if (result1 != "True") return false;
                    //SplashScreenManager.Default.SetWaitFormDescription("Đang lưu định mức size... (70%)");
                    string url2 = $"{URL}XinNPLMayMau/PostT2?Action=POSTCHITIET&para1={GlobleData.UserName}&para2={maSizeKTTK}&para3={IsSoatXet}&para4={txtGhiChu.Text.ToString()}";
                    string result2 = await _clientExtension.PostAsync(url2, dtChiTiet);
                    if (result2 != "True") return false;

                    return true;
                });

                SplashScreenManager.CloseForm(false);

                if (success)
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    NapLai(!isAdd);
                    bandedGridView1.FocusedRowHandle = _rowhandle;
                }

            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);
                //MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }


        }
        public string BuildMaNhomSizeString(List<DataRow> selectedRows)
        {
            // Nhóm các MaSize theo MaNhomSize
            var grouped = selectedRows
                .Where(row => row["MaNhomSize"] != DBNull.Value && row["MaSize"] != DBNull.Value)
                .GroupBy(row => row["MaNhomSize"].ToString())
                .Select(g => new
                {
                    MaNhomSize = g.Key,
                    MaSizes = string.Join(",", g.Select(r => r["MaSize"].ToString()).Distinct())
                })
                .Where(x => !string.IsNullOrEmpty(x.MaSizes));

            // Ghép chuỗi theo định dạng
            string result = string.Join(";", grouped.Select(g => $"{g.MaNhomSize}:{g.MaSizes}"));

            return result;
        }
        public List<DataRow> GetSelectedRowsFromResult(string result, DataTable dataTable)
        {
            var selectedRows = new List<DataRow>();

            if (string.IsNullOrEmpty(result) || dataTable == null || dataTable.Rows.Count == 0)
                return selectedRows;

            // Phân tích chuỗi result thành dictionary
            // Format: "MaNhomSize1:MaSize1,MaSize2;MaNhomSize2:MaSize3"
            var nhomSizeDict = new Dictionary<string, List<string>>();

            var nhomGroups = result.Split(';');
            foreach (var group in nhomGroups)
            {
                if (string.IsNullOrEmpty(group)) continue;

                var parts = group.Split(':');
                if (parts.Length != 2) continue;

                var maNhomSize = parts[0].Trim();
                var maSizeList = parts[1].Split(',')
                                         .Select(s => s.Trim())
                                         .Where(s => !string.IsNullOrEmpty(s))
                                         .ToList();

                if (!string.IsNullOrEmpty(maNhomSize) && maSizeList.Any())
                {
                    nhomSizeDict[maNhomSize] = maSizeList;
                }
            }

            // Tìm các DataRow phù hợp
            foreach (DataRow row in dataTable.Rows)
            {
                var maNhomSize = row["MaNhomSize"]?.ToString() ?? "";
                var maSize = row["MaSize"]?.ToString() ?? "";

                if (nhomSizeDict.ContainsKey(maNhomSize))
                {
                    var allowedMaSizes = nhomSizeDict[maNhomSize];
                    if (allowedMaSizes.Contains(maSize))
                    {
                        selectedRows.Add(row);
                    }
                }
            }

            return selectedRows;
        }
        private DataTable CreateTableSaveERPNPLMayMau()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("MaNhom", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("MaDot", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("DinhMucChung", typeof(double));
            dt.Columns.Add("DinhMucHaoHut", typeof(double));
            dt.Columns.Add("IsDuyet", typeof(bool));
            dt.Columns.Add("NguoiDuyet", typeof(string));
            dt.Columns.Add("NgayDuyet", typeof(string));
            dt.Columns.Add("MaVTGhep", typeof(string));
            dt.Columns.Add("MaCode", typeof(string));
            dt.Columns.Add("STTCode", typeof(int));
            dt.Columns.Add("MaNhomChiTiet", typeof(string));
            dt.Columns.Add("IsActive", typeof(int));
            dt.Columns.Add("GhiChu", typeof(string));
            dt.Columns.Add("MaSizeChung", typeof(string));
            dt.Columns.Add("Size", typeof(string));
            dt.Columns.Add("MaPhieu", typeof(string));
            dt.Columns.Add("STTPhieu", typeof(int));
            return dt;
        }

        private DataTable CreateTableSaveERPNPLMayMauChiTiet()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("MaPhieu", typeof(string));
            dt.Columns.Add("MaPhong", typeof(string));
            dt.Columns.Add("TenPhong", typeof(string));
            dt.Columns.Add("MaNhom", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("MaCode", typeof(string));

            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("SoLuong", typeof(decimal));
            dt.Columns.Add("GhiChu", typeof(string));
            return dt;
        }
        private void setTypeTxtEdit(object sender, KeyPressEventArgs e)
        {
            TextEdit textEdit = sender as TextEdit;
            string currentText = textEdit.Text;
            int cursorPosition = textEdit.SelectionStart;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && currentText.Contains("."))
            {
                e.Handled = true;
                return;
            }
            if (char.IsDigit(e.KeyChar))
            {
                string newText = currentText.Insert(cursorPosition, e.KeyChar.ToString());
                if (newText.Contains("."))
                {
                    int indexOfDot = newText.IndexOf('.');
                    string decimalPart = newText.Substring(indexOfDot + 1);
                    if (decimalPart.Length > 4)
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
        }

        private void bandedGridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {


            try
            {
                DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
                if (bandedGridView1.RowCount > 0)
                {
                    if (e.Menu == null)
                        return;
                    e.Menu.Items.Clear();

                    if (e.HitInfo.InRow)
                    {
                        if (e.HitInfo.Column != null)
                        {

                            bool IsXetDuyet = false;
                            DataRow row_focused = bandedGridView1.GetFocusedDataRow();
                            if (row_focused == null) return;
                            if (row_focused.Table.Columns.Contains("IsXetDuyet"))
                            {
                                IsXetDuyet = row_focused["IsXetDuyet"]?.ToString().ToLower() == "đã duyệt";
                            }
                            bool IsCopyMauDM = false;
                            if (!IsXetDuyet)
                            {

                                DevExpress.Utils.Menu.DXMenuItem menuThemDongItem = new DevExpress.Utils.Menu.DXMenuItem("Thêm dòng", ThemDong);
                                e.Menu.Items.Add(menuThemDongItem);

                                DevExpress.Utils.Menu.DXMenuItem menuCopyCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy dòng", CopyDong);
                                e.Menu.Items.Add(menuCopyCopyItem);
                                //if (!string.IsNullOrEmpty(row_focused["MaMau"]?.ToString()) && IsCopyMauDM)
                                //{
                                //    DevExpress.Utils.Menu.DXMenuItem menuCopyMauItem = new DevExpress.Utils.Menu.DXMenuItem("Copy định mức màu", CopyDinhMuc_Mau);
                                //    e.Menu.Items.Add(menuCopyMauItem);

                                //}
                                DevExpress.Utils.Menu.DXMenuItem menuRemoveVatTuItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", RemoveVatTu);
                                e.Menu.Items.Add(menuRemoveVatTuItem);

                            }

                        }
                        //if (!isAdd)
                        //{
                        //    DevExpress.Utils.Menu.DXMenuItem menuXetduyetdong = new DevExpress.Utils.Menu.DXMenuItem("Xét duyệt dòng", xetduyetdong);
                        //    e.Menu.Items.Add(menuXetduyetdong);
                        //    DevExpress.Utils.Menu.DXMenuItem menuhuyduyetdong = new DevExpress.Utils.Menu.DXMenuItem("Hủy duyệt dòng", huyduyetdong);
                        //    e.Menu.Items.Add(menuhuyduyetdong);
                        //}

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        //private void xetduyetdong(object sender, EventArgs e)
        //{
        //    _rowhandle = bandedGridView1.FocusedRowHandle;
        //    DataRow currentRow = bandedGridView1.GetDataRow(bandedGridView1.FocusedRowHandle);
        //    if (currentRow == null) return;
        //    string url = string.Format("{0}?username={1}", URL + "KhoiTaoDM/KiemTraHuy", GlobleData.UserName);
        //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //    if (json == "[]")
        //    {
        //        XtraMessageBox.Show("User không có quyền hủy BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
        //        return;
        //    }
        //    string urls = $"{URL}XinNPLMayMau/XetDuyetDong?Action=XETDUYETDONG&para1={searchLookUpEditKH.EditValue.ToString()}&para2={searchLookUpEditMH.EditValue.ToString()}" +
        //        $"&para3={searchLookUpEditDot.EditValue.ToString()} &para4={currentRow["MaVTID"].ToString()} &para5={currentRow["MauVTID"].ToString()} &para6={currentRow["KhoVaiID"].ToString()}" +
        //        $"&para7={currentRow["MaCode"].ToString()} &para8={1}";
        //    string jsons = Task.Run(async () => { return await _clientExtension.GetAsnyc(urls); }).Result;
        //    if (jsons == "True")
        //    {
        //        clsWaitForm.ShowSuccessForm(this, 2000);
        //        LoadData();
        //        LoadChiTiet();
        //    }


        //}
        //private void huyduyetdong(object sender, EventArgs e)
        //{
        //    _rowhandle = bandedGridView1.FocusedRowHandle;
        //    DataRow currentRow = bandedGridView1.GetDataRow(bandedGridView1.FocusedRowHandle);
        //    if (currentRow == null) return;
        //    string url = string.Format("{0}?username={1}", URL + "KhoiTaoDM/KiemTraHuy", GlobleData.UserName);
        //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //    if (json == "[]")
        //    {
        //        XtraMessageBox.Show("User không có quyền hủy BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
        //        return;
        //    }
        //    if (toggleIsActive.IsOn)
        //    {
        //        XtraMessageBox.Show("Đợt này đang đucợ active. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
        //        return;
        //    }
        //    string urls = $"{URL}XinNPLMayMau/XetDuyetDong?Action=HUYDUYETDONG&para1={searchLookUpEditKH.EditValue.ToString()}&para2={searchLookUpEditMH.EditValue.ToString()}" +
        //        $"&para3={searchLookUpEditDot.EditValue.ToString()} &para4={currentRow["MaVTID"].ToString()} &para5={currentRow["MauVTID"].ToString()} &para6={currentRow["KhoVaiID"].ToString()}" +
        //        $"&para7={currentRow["MaCode"].ToString()} &para8={0}";
        //    string jsons = Task.Run(async () => { return await _clientExtension.GetAsnyc(urls); }).Result;
        //    if (jsons == "True")
        //    {
        //        clsWaitForm.ShowSuccessForm(this, 2000);
        //        LoadData();
        //        LoadChiTiet();
        //    }
        //}
        private void CopyDong(object sender, EventArgs e)
        {

            GridView view = bandedGridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                DataTable table = gridControl1.DataSource as DataTable;
                if (table == null) return;

                DataRow currentRow = view.GetDataRow(rowHandle);
                if (currentRow == null) return;

                string currentMaVTID = currentRow["MaVTID"].ToString();
                string currentMaNhom = currentRow["MaNhom"].ToString();
                string currentKhoVaiID = currentRow["KhoVaiID"].ToString();
                bool isNL = currentRow["NPL"].ToString() == "True" ? true : false;
                int currentSTT = Convert.ToInt32(currentRow["STT"]);
                int newSTTP = currentSTT + 1;
                // Tìm STT lớn nhất với MaVTID hiện tại
                var maxSTT = table.AsEnumerable()
                                  .Where(r => r["MaVTID"].ToString() == currentMaVTID && int.TryParse(r["STTCode"].ToString(), out _))
                                  .Select(r => Convert.ToInt32(r["STT"]))
                                  .DefaultIfEmpty(0)
                                  .Max();

                int newSTT = maxSTT + 1;

                // Tạo dòng mới
                DataRow newRow = table.NewRow();
                foreach (DataColumn col in table.Columns)
                {
                    newRow[col.ColumnName] = currentRow[col.ColumnName];
                }

                newRow["STTCode"] = newSTT;
                newRow["MaCode"] = $"{currentMaVTID}|{newSTT}";

                newRow["DinhMucHaoHut"] = 0;
                newRow["DinhMucChung"] = 0;
                newRow["STT"] = newSTTP;
                var rowsToUpdate = table.AsEnumerable()
           .Where(r => r["NPL"].ToString() == isNL.ToString() && Convert.ToInt32(r["STT"]) >= newSTTP)
           .ToList();

                // Tăng STT của các dòng phía sau lên 1
                foreach (DataRow row in rowsToUpdate)
                {
                    row["STT"] = Convert.ToInt32(row["STT"]) + 1;
                }
                if (isNL)
                {

                    _sttVT++;
                }
                else
                {

                    _sttVTPL++;

                }

                // Chèn dòng vào bảng
                int insertIndex = view.GetDataSourceRowIndex(rowHandle) + 1;
                table.Rows.InsertAt(newRow, insertIndex);

                // Làm mới lưới và focus dòng mới
                view.RefreshData();
                int newRowHandle = view.GetRowHandle(insertIndex);
                view.FocusedRowHandle = newRowHandle;
                view.MakeRowVisible(newRowHandle);
            }
        }
        private void RemoveVatTu(object sender, EventArgs e)
        {
            try
            {
                string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                int Rowfocus_Idx = bandedGridView1.FocusedRowHandle;
                if (Rowfocus_Idx < 0) return;
                DataRow rowFocus = bandedGridView1.GetFocusedDataRow();
                if (rowFocus != null)
                {

                    DialogResult result = XtraMessageBox.Show(
                     "Bạn có chắc chắn muốn xóa không?",
                     "XÁC NHẬN XÓA",
                     MessageBoxButtons.YesNo,
                     MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                    {
                        return;
                    }
                    else
                    {
                        object MaVTID = rowFocus["MaVTID"];
                        //object MauVatID = rowFocus["MauVTID"];
                        object MaNhom = rowFocus["MaNhom"];
                        object KhoVaiID = rowFocus["KhoVaiID"];
                        object MaCode = rowFocus["MaCode"];

                        string[] mauvtidchung = rowFocus["MauVTIDChung"].ToString().Split(',');
                        foreach (string mauvtid in mauvtidchung)
                        {
                            dynamic JPara = new
                            {
                                MaVTID = MaVTID,
                                MauVTID = mauvtid,
                                MaNhom = MaNhom,
                                KhoVaiID = KhoVaiID,
                                MaCode = MaCode,
                                MaKH = makh,
                                MaHang = mahang,
                                MaDot = madot,
                                UserID = GlobleData.UserName
                            };



                            string urlRemove = $"{URL}DeleteBOM_DM/Delete";
                            string resultRemove = Task.Run(async () => { return await _clientExtension.PostAsync(urlRemove, JPara); }).Result;
                            if (string.Compare(resultRemove?.ToLower(), "true") != 0)
                            {
                                XtraMessageBox.Show($"Xóa đã xảy ra lỗi.Vui lòng thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            RemoveRowDM(rowFocus, Rowfocus_Idx);
                        }



                    }
                }
            }
            catch (Exception ex)
            {

            }


        }


        private void RemoveRowDM(DataRow rowRm, int Rowfocus_Idx)
        {
            if (rowRm != null)
            {
                bool isNL = rowRm["NPL"].ToString() == "True" ? true : false;
                int currentSTT = Convert.ToInt32(rowRm["STT"]);

                DataTable table = gridControl1.DataSource as DataTable;
                var rowsToUpdate = table.AsEnumerable()
          .Where(r => r["NPL"].ToString() == isNL.ToString() && Convert.ToInt32(r["STT"]) >= currentSTT)
          .ToList();

                // Tăng STT của các dòng phía sau lên 1
                foreach (DataRow row in rowsToUpdate)
                {
                    row["STT"] = Convert.ToInt32(row["STT"]) - 1;
                }

                bandedGridView1.DeleteRow(Rowfocus_Idx);
                bandedGridView1.RefreshData();
                if (isNL)
                {

                    _sttVT--;
                }
                else
                {

                    _sttVTPL--;

                }


            }
        }


        private string GetLocalIPAddress()
        {
            string localIP = "Không xác định";
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
        private void WriteLogRemove_DM(object MaVTID, object MauVatID, object MaNhom, object KhoVaiID, object MaCode)
        {
            try
            {
                string IpAdress = GetLocalIPAddress();
                List<Log_Erp_KhoiTaoBom_Delete> lstLogSave = new List<Log_Erp_KhoiTaoBom_Delete>();
                lstLogSave.Add(new Log_Erp_KhoiTaoBom_Delete
                {
                    Action = "Xóa BOM",
                    //TableName = "ERPVatTuSP",
                    Modules = this.Name,
                    MaVTID = MaVTID?.ToString(),
                    MauVTID = MauVatID?.ToString(),
                    MaNhom = MaNhom?.ToString(),
                    KhoVaiID = KhoVaiID?.ToString(),
                    MaCode = MaCode?.ToString(),
                    Content = "",
                    IP = IpAdress,
                    UserID = GlobleData.UserName,
                    CreatedDate = DateTime.Now

                });


                clsWriteLogThuVienLib.WriteLog_BOM_DM(URL, _clientExtension, lstLogSave);
            }
            catch (Exception ex)
            {

            }

        }
        #region Phú sửa BOM
        private void repositoryItemSearchLookUpEdit1View_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            DataRow dr = bandedGridView1.GetFocusedDataRow();
            if (dr == null)
            {
                return;
            }
            string tenNhom = dr["TenNhom"].ToString();
            if (string.IsNullOrEmpty(tenNhom))
                return; // Không có điều kiện ⇒ hiển thị toàn bộ

            var view = sender as DevExpress.XtraGrid.Views.Base.ColumnView;
            var value = Convert.ToString(
                view.GetListSourceRowCellValue(e.ListSourceRow, "TenNhom")
            );

            if (!string.Equals(value, tenNhom, StringComparison.OrdinalIgnoreCase))
            {
                e.Visible = false;
                e.Handled = true;
            }
        }
        private void createTableCT(DataTable tab, GridBand gridBand, string bandGoc, string bandName, string bandNameParent)
        {
            if (tab == null || tab.Rows.Count == 0)
                return;

            bandedGridView1.BeginUpdate();
            try
            {
                GridBand parentBand = bandedGridView1.Bands[bandGoc];
                if (parentBand == null)
                    return;

                // Xóa các cột cũ khỏi bandedGridView1.Columns TRƯỚC KHI xóa band
                List<BandedGridColumn> columnsToRemove = new List<BandedGridColumn>();
                foreach (GridBand child in parentBand.Children)
                {
                    if (child.Name.StartsWith(bandNameParent, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (BandedGridColumn col in child.Columns)
                        {
                            columnsToRemove.Add(col);
                        }
                    }
                }

                // Xóa các column khỏi view
                foreach (var col in columnsToRemove)
                {
                    bandedGridView1.Columns.Remove(col);
                }

                // Xóa các band con cũ
                foreach (GridBand child in parentBand.Children.ToArray())
                {
                    if (child.Name.StartsWith(bandNameParent, StringComparison.OrdinalIgnoreCase))
                        parentBand.Children.Remove(child);
                }

                parentBand.Columns.Clear();

                DataTable tbl = gridControl1.DataSource as DataTable ?? new DataTable();

                foreach (DataRow row in tab.Rows)
                {
                    string caption = row[4].ToString();
                    string fieldName = $"{row[4]}{bandName}{row[1]}";
                    string columnName = $"col{row[4]}";
                    int estimatedWidth = Math.Max(50, caption.Length * 8 + 20);
                    // Luôn tạo column mới
                    BandedGridColumn column = new BandedGridColumn
                    {
                        Caption = caption,
                        FieldName = fieldName,
                        Name = columnName,
                        Visible = true,
                        MinWidth = estimatedWidth,
                        Width = estimatedWidth
                    };
                    column.OptionsColumn.FixedWidth = true;
                    column.AppearanceCell.Options.UseTextOptions = true;
                    column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    column.AppearanceHeader.Options.UseTextOptions = true;
                    column.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    column.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

                    if (bandGoc == "gridBandMauSP")
                        column = CreateSearchLookUpMauVT(column);
                    else
                    {
                        column = CreateTextSL(column);
                    }

                    // Add column vào view
                    bandedGridView1.Columns.Add(column);

                    GridBand childBand = new GridBand
                    {
                        Caption = caption,
                        Name = $"{bandNameParent}{columnName}",
                        Width = estimatedWidth   // Đặt cố định
                    };
                    childBand.OptionsBand.FixedWidth = true;
                    childBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    childBand.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    childBand.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                    childBand.AppearanceHeader.Options.UseTextOptions = true;

                    // CHỈ add column vào childBand, KHÔNG add vào parentBand.Columns
                    childBand.Columns.Add(column);
                    parentBand.Children.Add(childBand);



                    // XÓA DÒNG NÀY: parentBand.Columns.Add(column);

                    if (!tbl.Columns.Contains(fieldName))
                        tbl.Columns.Add(fieldName, typeof(string));
                }

                gridControl1.DataSource = tbl;
                bandedGridView1.OptionsView.ColumnAutoWidth = false;
            }
            finally
            {
                bandedGridView1.EndUpdate();
            }
        }

        private BandedGridColumn CreateSearchLookUpMauVT(BandedGridColumn col)
        {
            try
            {
                // KIỂM TRA DATA SOURCE
                if (tblMauVT == null || tblMauVT.Rows.Count == 0)
                {
                    MessageBox.Show("tblMauVT chưa có dữ liệu!");
                    return col;
                }

                RepositoryItemButtonEdit btnEdit = new RepositoryItemButtonEdit();
                btnEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

                btnEdit.Buttons.Clear();
                DevExpress.XtraEditors.Controls.EditorButton button =
            new DevExpress.XtraEditors.Controls.EditorButton(
                DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph);

                button.ImageOptions.Image = Properties.Resources.group_16x16;


                btnEdit.Buttons.Add(button);


                gridControl1.RepositoryItems.Add(btnEdit);

                btnEdit.ButtonClick += (s, e) =>
                {
                    DataRow dr = bandedGridView1.GetFocusedDataRow();
                    if (dr == null) return;
                    frmPhanTichBOM_ChonMau frm = new frmPhanTichBOM_ChonMau(dr);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        dr["MauVTIDChung"] = frm._mauvtidchung;
                        LoadTVMauVT();
                        DataTable tbl = frm.tblGrid;
                        if (tbl == null || tbl.Rows.Count == 0) return;
                        foreach (DataRow r in tbl.Rows)
                        {
                            string columnName = r["MauSPID"].ToString();
                            string mauVTID = r["MauVTID"].ToString();
                            foreach (DataColumn dc in dr.Table.Columns)
                            {
                                if (dc.ColumnName == columnName)
                                {
                                    dr[columnName] = mauVTID;
                                }
                            }

                        }

                    }
                    this.ActiveControl = button1;
                };

                col.ColumnEdit = btnEdit;

                return col;
            }
            catch (Exception ex)
            {

                return col;
            }
        }

        private BandedGridColumn CreateTextSL(BandedGridColumn col)
        {
            try
            {
                RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();

                gridControl1.RepositoryItems.Add(textEdit);

                textEdit.KeyPress += (s, e) =>
                {
                    setTypeTxtEditSL(s, e);
                };

                col.ColumnEdit = textEdit;

                return col;
            }
            catch (Exception ex)
            {

                return col;
            }
        }

        private void CreateMauSPColumns()
        {
            if (searchLookUpEditKH.EditValue == null || searchLookUpEditKH.EditValue.ToString() == "" || searchLookUpEditMH.EditValue == null || searchLookUpEditMH.EditValue.ToString() == "")
            {
                return;
            }
            string makh = searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue.ToString();
            string url = $"{URL}XinNPLMayMau/Get?Action=GETMAUSPCOLUMNS&para1={makh}&para2={mahang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblMauSP = JsonConvert.DeserializeObject<DataTable>(json);
            if (_tblMauSP.Rows.Count == 1)
            {
                createTableCT(_tblMauSP, gridBandMauSP, "gridBandMauSP", "@Mau@", "gridBandMauSPa");
                createTableCT(_tblMauSP, gridBandP1, "gridBandP1", "@MA_P1@", "gridBandP1a");
                createTableCT(_tblMauSP, gridBandP2, "gridBandP2", "@MA_P2@", "gridBandP2a");
                createTableCT(_tblMauSP, gridBandP3, "gridBandP3", "@MA_P3@", "gridBandP3a");
                createTableCT(_tblMauSP, gridBandP4, "gridBandP4", "@MA_P4@", "gridBandP4a");
            }

        }


        private void LoadTVMauVT()
        {
            string url = $"{URL}XinNPLMayMau/Get?Action=GETMAUVTTV";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblMauVT = JsonConvert.DeserializeObject<DataTable>(json);
        }

        private void bandedGridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Contains("@Mau@"))
            {
                if (e.Value != null && e.Value != DBNull.Value)
                {
                    string mauVTID = e.Value.ToString();


                    DataRow[] rows = tblMauVT.Select($"MauVTID = '{mauVTID}'");
                    if (rows.Length > 0)
                    {
                        e.DisplayText = rows[0]["MaMauVT"].ToString();
                    }
                }
            }
        }

        private void btnChonSize_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DataRow row = bandedGridView1.GetFocusedDataRow();
                if (row == null) return;
                DataTable tbl = gridControl1.DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0) return;
                if (row.Table.Columns.Contains("IsXetDuyet"))
                {
                    bool IsXetDuyet = false;
                    IsXetDuyet = row["IsXetDuyet"]?.ToString().ToLower() == "đã duyệt";
                    if (IsXetDuyet)
                    {
                        XtraMessageBox.Show("Vật tư đã được xét duyêt. Không thể chỉnh sửa Size sản phẩm. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }

                }
                frmPhanTichBOM_ChonSize frm = new frmPhanTichBOM_ChonSize(searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), row["MaSizeChung"].ToString());
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.DialogResult != DialogResult.OK) return;
                string result = frm.SelectedResult;
                row["Size"] = frm.SelectedResultName;
                row["MaSizeChung"] = frm.SelectedResult;
                ProcessSizeData(tbl);
                this.ActiveControl = button1;
            }
            catch (Exception ex)
            {


            }
        }
        private void btnNapLaiCLCT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLai(!isAdd);
        }
        private void bandedGridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;

            // Tô màu cả dòng khi MaVT rỗng hoặc chỉ toàn khoảng trắng
            var maVT = view.GetRowCellValue(e.RowHandle, "MaVT") as string;
            if (string.IsNullOrWhiteSpace(maVT))
            {
                e.Appearance.BackColor = Color.MistyRose;
                e.Appearance.ForeColor = Color.DarkRed;
                //e.HighPriority = true; // đảm bảo màu này không bị override bởi rule khác
                return;
            }
            if (e.Column.FieldName == "IsXetDuyet")
            {
                // Lấy giá trị của ô hiện tại trong cột đang xét
                object cellValue = e.CellValue;

                if (cellValue != null)
                {
                    if (cellValue.ToString() == "Đã duyệt")
                    {

                        e.Appearance.ForeColor = Color.Green;
                    }
                    else
                    {

                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }

            try
            {
                // Chỉ xử lý các cột phòng+màu (chứa @MA_P)
                if (!e.Column.FieldName.Contains("@MA_P")) return;

                DataRow row = bandedGridView1.GetDataRow(e.RowHandle);
                if (row == null) return;

                string maPhieu = row["MaPhieu"]?.ToString() ?? "";
                string maVTID = row["MaVTID"]?.ToString() ?? "";
                string khoVaiID = row["KhoVaiID"]?.ToString() ?? "";
                string maNhom = row["MaNhom"]?.ToString() ?? "";
                string maCode = row["MaCode"]?.ToString() ?? "";

                // Parse column name: TenMau@MA_PX@MaMau
                string[] parts = e.Column.FieldName.Split('@');
                if (parts.Length != 3) return;

                string maPhong = parts[1]; // MA_P1, MA_P2, etc.
                string maMau = parts[2];

                // Lấy MauVTID từ MauVTIDChung
                string mauVTIDChung = row["MauVTIDChung"]?.ToString() ?? "";
                string[] mauVTIDs = mauVTIDChung.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                // Kiểm tra từng MauVTID
                foreach (string mauVTID in mauVTIDs)
                {
                    string key = $"{maPhieu}_{maVTID}_{mauVTID}_{khoVaiID}_{maNhom}_{maCode}_{maMau}_{maPhong}";

                    if (cellsToHighlight.ContainsKey(key) && cellsToHighlight[key])
                    {
                        e.Appearance.BackColor = Color.Yellow;
                        e.Appearance.BackColor2 = Color.Yellow;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Silent error
            }
        }
        private void bandedGridView1_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                if (bandedGridView1.FocusedColumn == null)
                    return;
                DataRow row = bandedGridView1.GetFocusedDataRow();
                if (row == null) return;

                if ((bandedGridView1.FocusedColumn.FieldName == "MaVT" || bandedGridView1.FocusedColumn.FieldName == "Size" || bandedGridView1.FocusedColumn.FieldName.Contains("@Mau@")) && bandedGridView1.ActiveEditor is ButtonEdit buttonEdit)
                {
                    gridControl1.BeginInvoke(new Action(() =>
                    {
                        buttonEdit.PerformClick(buttonEdit.Properties.Buttons[0]);
                    }));
                }
                else if (bandedGridView1.FocusedColumn.FieldName == "MaNhomChiTiet")
                {

                    gridControl1.BeginInvoke(new Action(() =>
                    {
                        if (bandedGridView1.ActiveEditor is SearchLookUpEdit searchLookUpEdit)
                        {
                            if (!searchLookUpEdit.IsPopupOpen)
                            {
                                searchLookUpEdit.ShowPopup();
                            }
                        }
                    }));

                }
                else if (bandedGridView1.FocusedColumn.FieldName.Contains("@MA_P1") || bandedGridView1.FocusedColumn.FieldName.Contains("@MA_P2") ||
                    bandedGridView1.FocusedColumn.FieldName.Contains("@MA_P3") || bandedGridView1.FocusedColumn.FieldName.Contains("@MA_P4"))
                {
                    if (tgXacNhan.IsOn && !_PQSoatXet)
                    {
                        XtraMessageBox.Show("Phiếu đã được xác nhận. Không thể chỉnh sửa Số lượng. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        this.ActiveControl = button1;
                        return;
                    }
                    if (tgSoatXet.IsOn)
                    {
                        XtraMessageBox.Show("Phiếu đã được soát xét. Không thể chỉnh sửa Số lượng. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        this.ActiveControl = button1;
                        return;
                    }
                }

            }
            catch (Exception ex)
            {


            }
        }
        private void loadSizeChung()
        {
            string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string url = $"{URL}XinNPLMayMau/Get?action=GETSIZESP&para1={makh.ToString()}&para2={mahang.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                tblSizeChung = JsonConvert.DeserializeObject<DataTable>(json);

            }
        }
        #endregion
        private void addRowEmpty(List<DataRow> lst, int isNPL, DataTable tbl)
        {
            DataRow newRow = bandedGridView1.GetFocusedDataRow();
            if (newRow == null) return;
            DataRow firstRow = lst?.FirstOrDefault();
            if (firstRow == null)
                return;

            newRow["MaNhom"] = firstRow["MaNhom"];
            newRow["TenNhom"] = firstRow["TenNhom"];
            newRow["NPL"] = firstRow["NPL"];
            newRow["Sort"] = firstRow["Sort"];
            newRow["MaVTID"] = firstRow["MaVTID"];
            newRow["MaVT"] = firstRow["MaVT"];
            newRow["ChiTiet"] = firstRow["VatTu"];
            newRow["MaDVVT"] = firstRow["MaDVVT"];
            newRow["TenDVVT"] = firstRow["TenDVVT"];
            newRow["KhoVaiID"] = firstRow["KhoVaiID"];
            newRow["KhoVai"] = firstRow["KhoVai"];
            //newRow["STT"] = isNL ? _sttVT : _sttVTPL;
            newRow["DinhMucChung"] = 0;
            newRow["DinhMucHaoHut"] = 0;
            newRow["TachMau"] = false;
            newRow["IsNew"] = 1;

            newRow["STTCode"] = 0;
            newRow["MaCode"] = firstRow["MaVTID"].ToString() + "|" + "0";
            newRow["IsActive"] = false;

            // Logic tìm kiếm và gán MaNhomChiTiet
            string maNhomChiTiet = "";
            bool nplValue = Convert.ToBoolean(firstRow["NPL"]);

            if (nplValue)
            {
                // NPL = true: Tìm theo MaNhom
                string filterChungLoai = string.Format("MaNhom = '{0}'", firstRow["MaNhom"]);
                DataRow[] foundRows = tblChungLoaiChiTiet.Select(filterChungLoai);

                // Chỉ gán nếu tìm được đúng 1 dòng
                if (foundRows.Length == 1)
                {
                    maNhomChiTiet = foundRows[0]["MaNhomChiTiet"].ToString();
                }
            }
            else
            {
                // NPL = false: Tìm theo TenNhom = TenNhomChiTiet
                string filterChungLoai = string.Format("TenNhomChiTiet = '{0}'", firstRow["TenNhom"].ToString().Replace("'", "''"));
                DataRow[] foundRows = tblChungLoaiChiTiet.Select(filterChungLoai);

                // Gán nếu tìm được ít nhất 1 dòng (lấy dòng đầu tiên)
                if (foundRows.Length > 0)
                {
                    maNhomChiTiet = foundRows[0]["MaNhomChiTiet"].ToString();
                }
            }

            newRow["MaNhomChiTiet"] = maNhomChiTiet;
            newRow["MauVTIDChung"] = firstRow["MauVTIDChung"].ToString();
            newRow["Size"] = "";
            foreach (DataColumn col in newRow.Table.Columns)
            {
                if (col.ColumnName.Contains("@Mau@"))
                {
                    newRow[col.ColumnName] = "";
                }
            }
            //tbl.Rows.Add(newRow);

            // gridControl1.DataSource = tbl;

        }


        private void exportFileExcel(BandedGridView bandedView, string filePath)
        {
            try
            {
                XlsxExportOptionsEx options = new XlsxExportOptionsEx()
                {
                    ExportType = DevExpress.Export.ExportType.WYSIWYG,

                    AllowGrouping = DevExpress.Utils.DefaultBoolean.True,
                    ShowBandHeaders = DevExpress.Utils.DefaultBoolean.False,
                    ShowGridLines = true,
                    SheetName = "Danh sách BOM"
                };
                gridControl1.ExportToXlsx(filePath, options);
                Workbook wb = new Workbook();
                wb.LoadDocument(filePath);
                Worksheet sheet = wb.Worksheets[0];
                int headerTableIndex = 6;
                int startStaticHeader = 11;

                sheet.Rows.Insert(0, headerTableIndex);
                sheet.Columns[startStaticHeader - 1].WidthInCharacters = 20;
                sheet.Columns[startStaticHeader - 1].Alignment.WrapText = true;


                int rangeLeft = sheet.GetUsedRange().LeftColumnIndex;
                int rangeRight = sheet.GetUsedRange().RightColumnIndex;
                int rangeBottom = sheet.GetUsedRange().BottomRowIndex;
                int rangeTop = headerTableIndex;

                int currentCol = 0;
                int currentRow = 0;

                Range headerRange = sheet.Rows[headerTableIndex];
                Range beginHeaderRange = sheet.Range.FromLTRB(rangeLeft, rangeTop, rangeLeft + startStaticHeader - 1, rangeTop);
                Range colorRange = sheet.Range.FromLTRB(rangeLeft + startStaticHeader, rangeTop, rangeRight - 2, rangeTop + 1);
                Range gridRange = sheet.Range.FromLTRB(rangeLeft, rangeTop, rangeRight, rangeBottom);

                beginHeaderRange.FillColor = Color.FromArgb(255, 228, 179);
                colorRange.FillColor = Color.FromArgb(0, 218, 218);
                sheet.Cells[rangeTop, rangeRight].FillColor = Color.FromArgb(255, 228, 179);
                sheet.Cells[rangeTop, rangeRight - 1].FillColor = Color.FromArgb(255, 228, 179);

                sheet.Range["A2:D2"].Merge();
                sheet.Range["A3:D3"].Merge();
                sheet.Range["A4:D4"].Merge();
                sheet.Range["A5:D5"].Merge();
                sheet.Range["A6:D6"].Merge();

                sheet.Cells["A2"].Value = "Khách hàng";
                sheet.Cells["E2"].Value = searchLookUpEditKH.Text;
                sheet.Cells["A3"].Value = "Mã hàng";
                sheet.Cells["E3"].Value = searchLookUpEditMH.Text;
                sheet.Cells["A4"].Value = "Đợt";
                if (!isAdd)
                    sheet.Cells["E4"].Value = searchLookUpEditDot.Text;

                //sheet.Cells["A5"].Value = "Chủng loại CT";
                //sheet.Cells["E5"].Value = searchLookUpEditCLCT.Text;

                sheet.Range["A1:A5"].Font.Bold = true;
                headerRange.Font.Bold = true;

                currentRow = headerTableIndex + 2;
                currentCol = startStaticHeader - 1;
                while (currentRow <= rangeBottom + 1)
                {
                    Cell cell = sheet.Cells[currentRow, currentCol];
                    string txt = cell.DisplayText;
                    if (string.IsNullOrEmpty(txt))
                    {
                        currentRow++;
                        continue;
                    }

                    // xử lý: xuống dòng tại dấu phẩy gần giới hạn
                    StringBuilder sb = new StringBuilder();
                    int lastBreak = 0;
                    for (int i = 0; i < txt.Length; i++)
                    {
                        if (i - lastBreak >= 20)
                        {
                            int commaPos = sb.ToString().LastIndexOf(',', i);
                            if (commaPos > lastBreak && commaPos < sb.Length - 1)
                            {
                                sb.Insert(commaPos + 1, Environment.NewLine);
                                lastBreak = commaPos + 2; // +2 để bỏ qua newline
                            }
                        }
                    }

                    string newText = sb.Length > 0 ? sb.ToString() : txt;
                    cell.Value = newText;
                    cell.Alignment.WrapText = true;
                    currentRow++;
                }

                currentRow = headerTableIndex + 2;
                currentCol = 0;
                bool wroteNL = false;
                for (int i = sheet.Pictures.Count - 1; i >= 0; i--)
                {
                    Picture pic = sheet.Pictures[i];
                    if (pic.TopLeftCell.ColumnIndex == 0) // cột A = 0
                    {
                        sheet.Pictures.RemoveAt(i);
                    }
                }
                while (currentRow <= rangeBottom + 1)
                {
                    Color NPLCell = sheet.Cells[currentRow, currentCol].FillColor;
                    Color nhomCell = sheet.Cells[currentRow, currentCol + 1].FillColor;
                    Color vattuCell = sheet.Cells[currentRow, currentCol + 2].FillColor;
                    Cell sttCell = sheet.Cells[currentRow, currentCol + 3];
                    if (NPLCell != Color.Empty && !wroteNL)
                    {
                        sheet.Cells[currentRow, currentCol].Value = "Nguyên liệu";
                        sheet.Cells[currentRow, currentCol].Font.Color = Color.Blue;
                        sheet.Cells[currentRow, currentCol].Font.Bold = true;
                        wroteNL = true;
                    }
                    else if (NPLCell != Color.Empty && wroteNL)
                    {
                        sheet.Cells[currentRow, currentCol].Value = "Phụ liệu";
                        sheet.Cells[currentRow, currentCol].Font.Color = Color.Blue;
                        sheet.Cells[currentRow, currentCol].Font.Bold = true;
                    }
                    else if (nhomCell != Color.Empty)
                    {
                        sheet.Cells[currentRow, currentCol].Font.Color = Color.Red;
                        sheet.Cells[currentRow, currentCol].Font.Bold = true;
                    }
                    else if (vattuCell != Color.Empty)
                    {
                        sheet.Cells[currentRow, currentCol].Font.Color = Color.IndianRed;
                        sheet.Cells[currentRow, currentCol].Font.Bold = true;
                    }
                    else if (sttCell.Value.Type == CellValueType.Numeric)
                    {
                        sheet.Cells[currentRow, currentCol].Font.Color = Color.Black;
                        sheet.Cells[currentRow, currentCol].Font.Bold = true;
                    }
                    currentRow++;
                }

                currentCol = rangeLeft + startStaticHeader;
                foreach (GridBand band in bandedView.Bands)
                {
                    if (band.Caption != null && band.Caption.Contains("Màu sản phẩm"))
                    {
                        foreach (GridBand colorCol in band.Children)
                        {
                            if (colorCol.Columns.Count > 0)
                            {
                                var col = colorCol.Columns[0];
                                string fieldName = col.FieldName;
                                string headerText = col.Caption;
                                currentRow = headerTableIndex + 2;
                                for (int i = 0; i < bandedView.RowCount; i++)
                                {
                                    int rowHandle = bandedView.GetVisibleRowHandle(i);
                                    if (!bandedView.IsDataRow(rowHandle)) continue;
                                    object id = bandedView.GetRowCellValue(rowHandle, col);
                                    string display = bandedView.GetRowCellDisplayText(rowHandle, col);
                                    //while (currentRow < 50 && (string.IsNullOrEmpty(sheet.Cells[currentRow, currentCol].DisplayText?.Trim()) || sheet.Cells[currentRow, currentCol].IsMerged))
                                    while (currentRow <= rangeBottom + 1 && sheet.Cells[currentRow, currentCol].DisplayText?.Trim() != id.ToString())
                                    {
                                        currentRow++;
                                    }
                                    sheet.Cells[currentRow, currentCol].Value = display;
                                    currentRow++;
                                }
                            }
                            currentCol++;
                        }
                    }

                }

                gridRange.AutoFitColumns();
                gridRange.AutoFitRows();
                for (int i = gridRange.TopRowIndex; i <= gridRange.BottomRowIndex; i++)
                {
                    Row row = sheet.Rows[i];
                    row.Height *= 1.3;
                }
                for (int j = gridRange.LeftColumnIndex; j <= gridRange.RightColumnIndex; j++)
                {
                    Column col = sheet.Columns[j];
                    col.Width += 5;
                }
                wb.SaveDocument(filePath);

                //XtraMessageBox.Show("Đã xuất file thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi khi xuất file:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(ex.Message);
            }

        }

        private void loadKhoSizeBOM()
        {
            string url = $"{URL}XinNPLMayMau/Get?Action=GETKHOSIZEBOM";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblKhoSize = JsonConvert.DeserializeObject<DataTable>(json);
            repoKhoSize.DisplayMember = "KhoVai";
            repoKhoSize.ValueMember = "KhoVaiID";
            repoKhoSize.DataSource = _tblKhoSize;
        }
        private void repoItemcode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string dot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            DataRow dr = bandedGridView1.GetFocusedDataRow();
            if (dr == null) return;
            int isNPLDisplay = dr["NPL"].ToString() == "True" ? 1 : 2;
            DataTable tbl = gridControl1.DataSource as DataTable;
            frmPhanTichBOM_ChonVatTu frm = new frmPhanTichBOM_ChonVatTu(kh, mh, dot, tbl, isNPLDisplay);
            if (frm.ShowDialog() == DialogResult.OK)
            {

                //List<DataRow> lstSelect = frm.lstSelect.OrderBy(row => Convert.ToInt32(row["STT"])).ToList();
                addRowEmpty(frm.lstSelect, isNPLDisplay, tbl);
            }
            this.ActiveControl = button1;
        }



        private void ThemDong(object sender, EventArgs e)
        {
            GridView view = bandedGridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                DataTable table = gridControl1.DataSource as DataTable;
                if (table == null) return;

                DataRow currentRow = view.GetDataRow(rowHandle);
                if (currentRow == null) return;

                string currentMaVTID = currentRow["MaVTID"].ToString();

                string currentKhoVaiID = currentRow["KhoVaiID"].ToString();
                bool isNL = currentRow["NPL"].ToString() == "True" ? true : false;
                int currentSTT = Convert.ToInt32(currentRow["STT"]);
                int newSTTP = currentSTT + 1;
                // Tìm STT lớn nhất với MaVTID hiện tại


                // Tạo dòng mới
                DataRow newRow = table.NewRow();


                newRow["DinhMucHaoHut"] = 0;
                newRow["DinhMucChung"] = 0;
                newRow["STT"] = newSTTP;
                newRow["NPL"] = currentRow["NPL"];
                var rowsToUpdate = table.AsEnumerable()
           .Where(r => r["NPL"].ToString() == isNL.ToString() && Convert.ToInt32(r["STT"]) >= newSTTP)
           .ToList();

                // Tăng STT của các dòng phía sau lên 1
                foreach (DataRow row in rowsToUpdate)
                {
                    row["STT"] = Convert.ToInt32(row["STT"]) + 1;
                }
                if (isNL)
                {

                    _sttVT++;
                }
                else
                {

                    _sttVTPL++;

                }
                int insertIndex = view.GetDataSourceRowIndex(rowHandle) + 1;
                table.Rows.InsertAt(newRow, insertIndex);
                view.RefreshData();

            }
            this.ActiveControl = button1;
        }


        private void gridView1_CustomRowFilter(object sender, RowFilterEventArgs e)
        {
            DataRow dr = bandedGridView1.GetFocusedDataRow();
            if (dr == null)
                return;
            string mavtid = dr["MaVTID"].ToString();
            string manhom = dr["MaNhom"].ToString();

            if (string.IsNullOrEmpty(manhom) || string.IsNullOrEmpty(mavtid))
                return;
            var view = sender as DevExpress.XtraGrid.Views.Base.ColumnView;
            string listMaNhom = Convert.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "MaNhom"));
            string listMaVTID = Convert.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "MaVTID"));
            if (!string.Equals(listMaNhom, manhom, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(listMaVTID, mavtid, StringComparison.OrdinalIgnoreCase))
            {
                e.Visible = false;
                e.Handled = true;
            }
        }
        private void repoKhoSize_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            DataRow dr = bandedGridView1.GetFocusedDataRow();
            if (dr == null) return;
            var oldValue = e.OldValue;
            var newValue = e.NewValue;
            if (oldValue == newValue)
                return;
            string maNhom = dr["MaNhom"].ToString();
            string maVTID = dr["MaVTID"].ToString();
            string khovaiid = newValue.ToString();
            string url = $"{URL}XinNPLMayMau/Get?action=GETCHECKKHOSIZE&para1={maNhom}&para2={maVTID}&para3={khovaiid}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblMauVTIDNew = JsonConvert.DeserializeObject<DataTable>(json);
            DataRow checkMau = tblMauVTIDNew.AsEnumerable().FirstOrDefault();
            if (checkMau == null) return;
            string mauVTIDChung = checkMau["MauVTIDChung"]?.ToString() ?? "";
            bool isMau = true;
            foreach (DataColumn col in dr.Table.Columns)
            {
                if (col.ColumnName.Contains("@Mau@"))
                {
                    string giaTriCot = dr[col.ColumnName]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(giaTriCot) && !mauVTIDChung.Contains(giaTriCot))
                    {
                        isMau = false;
                        break;
                    }
                }
            }

            var maxSTT = (gridControl1.DataSource as DataTable).AsEnumerable()
                                  .Where(r => r["MaNhom"].ToString() == maNhom && r["KhoVaiID"].ToString() == khovaiid && r["MaVTID"].ToString() == maVTID && int.TryParse(r["STTCode"].ToString(), out _))
                                  .Select(r => Convert.ToInt32(r["STT"]))
                                  .DefaultIfEmpty(0)
                                  .Max();

            int newSTT = maxSTT + 1;
            dr["STTCode"] = newSTT;
            if (isMau)
            {
                return;
            }
            else
            {
                DialogResult result = MessageBox.Show(
              "Lưu ý: màu vật tư bạn chọn sẽ đc làm mới. Bạn có chắc chắn muốn thay đổi không?",
              "Xác nhận",
              MessageBoxButtons.YesNo,
              MessageBoxIcon.Question
          );

                // Nếu chọn No thì hủy thay đổi
                if (result == DialogResult.No)
                {
                    e.Cancel = true; // Hủy việc thay đổi giá trị
                }
                else if (result == DialogResult.Yes)
                {
                    foreach (DataColumn col in dr.Table.Columns)
                    {
                        if (col.ColumnName.Contains("@Mau@"))
                        {
                            dr[col.ColumnName] = "";
                        }
                    }

                }
            }

        }
        private void btnTaoPhieuXinNPL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            isAdd = true;
            tgXacNhan.Toggled -= tgDuyet_Toggled;
            tgXacNhan.EditValueChanging -= tgDuyet_EditValueChanging;
            tgXacNhan.EditValue = false;
            tgXacNhan.Properties.Appearance.ForeColor = tgXacNhan.IsOn ? Color.ForestGreen : Color.DimGray;
            tgXacNhan.Toggled += tgDuyet_Toggled;
            tgXacNhan.EditValueChanging += tgDuyet_EditValueChanging;
            layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            // Xóa các cột thuộc nhóm Mẫu SP
            ClearTableCT("gridBandMauSP", "gridBandMauSPa");

            // Xóa các cột thuộc nhóm P1
            ClearTableCT("gridBandP1", "gridBandP1a");

            // Xóa các cột thuộc nhóm P2
            ClearTableCT("gridBandP2", "gridBandP2a");

            // Xóa các cột thuộc nhóm P3
            ClearTableCT("gridBandP3", "gridBandP3a");

            // Xóa các cột thuộc nhóm P4
            ClearTableCT("gridBandP4", "gridBandP4a");
            CreateMauSPColumns();
            CreatePhieuSTT();
            LoadData();
            btnTaoPhieuXinNPL.Enabled = false;


        }

        private void CreatePhieuSTT()
        {
            string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            string url = $"{URL}XinNPLMayMau/Get?action=GETMAXSTTPHIEU&para1={kh.ToString()}&para2={mh.ToString()}&para3={madot.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                txtPhieuXinNPL.Text = "";
                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            maxSTTPhieu = Convert.ToInt32(tbl.Rows[0][0]) + 1;
            txtPhieuXinNPL.Text = searchLookUpEditMH.Text.ToString() + "|" + maxSTTPhieu;
            _maphieu = kh.ToString() + '_' + mh.ToString() + '_' + madot + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "|" + maxSTTPhieu;
        }

        private void loadPhieuXinNPL()
        {
            searchLookUpEditPhieuXinNPL.Properties.DisplayMember = "TenPhieu";
            searchLookUpEditPhieuXinNPL.Properties.ValueMember = "MaPhieu";

            string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            string url = $"{URL}XinNPLMayMau/Get?action=GETPHIEU&para1={makh.ToString()}&para2={mahang.ToString()}&para3={madot.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblDot = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditPhieuXinNPL.Properties.DataSource = tblDot;
            if (tblDot == null || tblDot.Rows.Count == 0)
            {

                searchLookUpEditPhieuXinNPL.EditValue = "";
                ClearTableCT("gridBandMauSP", "gridBandMauSPa");

                // Xóa các cột thuộc nhóm P1
                ClearTableCT("gridBandP1", "gridBandP1a");

                // Xóa các cột thuộc nhóm P2
                ClearTableCT("gridBandP2", "gridBandP2a");

                // Xóa các cột thuộc nhóm P3
                ClearTableCT("gridBandP3", "gridBandP3a");

                // Xóa các cột thuộc nhóm P4
                ClearTableCT("gridBandP4", "gridBandP4a");

            }
            else
            {
                if (!string.IsNullOrEmpty(_maPhieuTQ))
                {
                    searchLookUpEditPhieuXinNPL.EditValue = _maPhieuTQ;
                    return;
                }
                else
                {
                    searchLookUpEditPhieuXinNPL.EditValue = tblDot.Rows[0]["MaPhieu"];
                    return;
                }
            }
        }



        private void btnChonMauSP_Click(object sender, EventArgs e)
        {
            if (tgXacNhan.IsOn)
            {
                XtraMessageBox.Show("Phiếu đã được xét duyêt. Không thể chỉnh sửa màu sản phẩm. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            DataTable tbl = gridControl1.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0) return;
            frmPhanTichBOM_XinNPLChonMau frm = new frmPhanTichBOM_XinNPLChonMau(makh, mahang, _mamauchung, tbl, madot);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                DataTable tblMau = frm.selectedTable;
                createTableCT(tblMau, gridBandMauSP, "gridBandMauSP", "@Mau@", "gridBandMauSPa");
                createTableCT(tblMau, gridBandP1, "gridBandP1", "@MA_P1@", "gridBandP1a");
                createTableCT(tblMau, gridBandP2, "gridBandP2", "@MA_P2@", "gridBandP2a");
                createTableCT(tblMau, gridBandP3, "gridBandP3", "@MA_P3@", "gridBandP3a");
                createTableCT(tblMau, gridBandP4, "gridBandP4", "@MA_P4@", "gridBandP4a");


            }
        }



        private void searchLookUpEditPhieuXinNPL_EditValueChanged(object sender, EventArgs e)
        {
            //load size 
            //loadSize();
            //load màu
            loadMauPhieu();
            //load dữ liệu phiếu
            loadDataPhieu();


        }
        private void loadMauPhieu()
        {
            try
            {
                string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string maphieu = searchLookUpEditPhieuXinNPL.EditValue == null ? "" : searchLookUpEditPhieuXinNPL.EditValue.ToString();
                string url = $"{URL}XinNPLMayMau/Get?action=GETMAUPHIEU&para1={maphieu.ToString()}&para2={makh.ToString()}&para3={mahang.ToString()}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0) return;
                DataTable dtP1 = tbl.AsEnumerable()
                                .Where(r => r.Field<string>("MaPhong") == "MA_P1")
                                .CopyToDataTable();

                DataTable dtP2 = tbl.AsEnumerable()
                                 .Where(r => r.Field<string>("MaPhong") == "MA_P2")
                                 .CopyToDataTable();

                DataTable dtP3 = tbl.AsEnumerable()
                                 .Where(r => r.Field<string>("MaPhong") == "MA_P3")
                                 .CopyToDataTable();

                DataTable dtP4 = tbl.AsEnumerable()
                                 .Where(r => r.Field<string>("MaPhong") == "MA_P4")
                                 .CopyToDataTable();
                DataTable distinctColors = tbl.AsEnumerable()
                                                                .GroupBy(r => new
                                                                {
                                                                    MaMau = r.Field<string>("MaMau"),
                                                                    CodeMau = r.Field<string>("CodeMau"),
                                                                    TenMau = r.Field<string>("TenMau")
                                                                })
                                                                .Select(g => g.First())
                                                                .CopyToDataTable();

                createTableCT(distinctColors, gridBandMauSP, "gridBandMauSP", "@Mau@", "gridBandMauSPa");
                createTableCT(dtP1, gridBandP1, "gridBandP1", "@MA_P1@", "gridBandP1a");
                createTableCT(dtP2, gridBandP2, "gridBandP2", "@MA_P2@", "gridBandP2a");
                createTableCT(dtP3, gridBandP3, "gridBandP3", "@MA_P3@", "gridBandP3a");
                createTableCT(dtP4, gridBandP4, "gridBandP4", "@MA_P4@", "gridBandP4a");
            }
            catch (Exception ex)
            {

            }




        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                if (searchLookUpEditKH.EditValue == null || searchLookUpEditDot.EditValue == null || searchLookUpEditMH.EditValue == null || searchLookUpEditPhieuXinNPL.EditValue == null) return;
                if ((bool)tgXacNhan.EditValue)
                {
                    XtraMessageBox.Show("Phiếu đã được duyệt nên không thể xóa.Vui lòng kiểm tra lại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!_PQXoa)
                {
                    XtraMessageBox.Show("User không có quyền xóa.Vui lòng kiểm tra lại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                string maphieu = searchLookUpEditPhieuXinNPL.EditValue == null ? "" : searchLookUpEditPhieuXinNPL.EditValue.ToString();
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa Phiếu này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    string url = $"{URL}XinNPLMayMau/Delete?action=DELETE&para1={makh.ToString()}&para2={mahang.ToString()}&para3={madot.ToString()}&para4={maphieu.ToString()}&para5={GlobleData.UserName}";
                    string json = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;

                    if (json.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this);
                        string Title = "BOM NPL may mẫu đã xóa";
                        SendNotify(Title, "PKH");
                        //SendNotify(Title, "KHO");
                        loadPhieuXinNPL();
                        LoadData();
                        loadDataPhieu();

                    }

                }
            }
            catch (Exception ex) { }
        }


        private void loadDataPhieu2()
        {
            try
            {
                //string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                //string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                //string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                //string maphieu = searchLookUpEditPhieuXinNPL.EditValue == null ? "" : searchLookUpEditPhieuXinNPL.EditValue.ToString();
                //string url = $"{URL}XinNPLMayMau/Get?action=GETNPLMAYMAU&para1={makh.ToString()}&para2={mahang.ToString()}&para3={madot.ToString()}&para4={maphieu.ToString()}";
                //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                //if (json == "[]") return;
                //DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                //if (tbl == null || tbl.Rows.Count == 0) return;
                //gridControl1.DataSource = tbl;
                //tgDuyet.Toggled -= tgDuyet_Toggled;
                ////toggleIsActive.EditValueChanging -= toggleIsActive_EditValueChanging;
                ////_isActiveDot = Convert.ToBoolean(tbl.Rows[0]["IsDuyet"]);
                //tgDuyet.EditValue = tbl.Rows[0]["IsDuyet"].ToString().ToUpper() == "TRUE" ? true : false;
                //tgDuyet.Properties.Appearance.ForeColor = tgDuyet.IsOn ? Color.ForestGreen : Color.DimGray;
                //tgDuyet.Toggled += tgDuyet_Toggled;
                string makh = searchLookUpEditKH.EditValue?.ToString() ?? "";
                string mahang = searchLookUpEditMH.EditValue?.ToString() ?? "";
                string madot = searchLookUpEditDot.EditValue?.ToString() ?? "";
                string maphieu = searchLookUpEditPhieuXinNPL.EditValue?.ToString() ?? "";

                if (string.IsNullOrEmpty(makh) || string.IsNullOrEmpty(mahang) ||
                    string.IsNullOrEmpty(madot) || string.IsNullOrEmpty(maphieu))
                    return;

                // BƯỚC 1: Lấy 3 bảng dữ liệu song song
                var url1 = $"{URL}XinNPLMayMau/Get?action=GETERP_NPLMayMau_V1&para1={makh}&para2={mahang}&para3={madot}&para4={maphieu}";
                var url2 = $"{URL}XinNPLMayMau/Get?action=GETBangMau_V1&para1={makh}&para2={mahang}&para4={maphieu}";
                var url3 = $"{URL}XinNPLMayMau/Get?action=GETERP_NPLMayMauChiTiet_V1&para4={maphieu}";



                string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                string json2 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
                string json3 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url3); }).Result;

                if (json1 == "[]") return;

                DataTable dtMain = JsonConvert.DeserializeObject<DataTable>(json1);
                DataTable dtMau = json2 == "[]" ? null : JsonConvert.DeserializeObject<DataTable>(json2);
                DataTable dtChiTiet = json3 == "[]" ? null : JsonConvert.DeserializeObject<DataTable>(json3);

                if (dtMain == null || dtMain.Rows.Count == 0) return;

                // BƯỚC 2: Lấy danh sách màu và phòng
                var mauMaList = new List<string>();
                var mauTenList = new List<string>();
                if (dtMau != null)
                {
                    foreach (DataRow r in dtMau.Rows)
                    {
                        mauMaList.Add(r["MaMau"].ToString());
                        mauTenList.Add(r["TenMau"].ToString());
                    }
                }

                var phongList = new List<string>();
                if (dtChiTiet != null)
                {
                    var tempPhong = new HashSet<string>();
                    foreach (DataRow r in dtChiTiet.Rows)
                    {
                        tempPhong.Add(r["MaPhong"].ToString());
                    }
                    phongList = tempPhong.OrderBy(x => x).ToList();
                }

                // BƯỚC 3: Tạo DataTable kết quả với cột động
                DataTable result = dtMain.Clone();

                // Thêm cột màu: TenMau@Mau@MaMau
                for (int i = 0; i < mauMaList.Count; i++)
                {
                    string colName = string.Format("{0}@Mau@{1}", mauTenList[i], mauMaList[i]);
                    if (!result.Columns.Contains(colName))
                        result.Columns.Add(colName, typeof(string));
                }

                // Thêm cột phòng+màu: TenMau@MaPhong@MaMau
                foreach (var phong in phongList)
                {
                    for (int i = 0; i < mauMaList.Count; i++)
                    {
                        string colName = string.Format("{0}@{1}@{2}", mauTenList[i], phong, mauMaList[i]);
                        if (!result.Columns.Contains(colName))
                            result.Columns.Add(colName, typeof(string));
                    }
                }

                // Thêm các cột bổ sung
                if (!result.Columns.Contains("IsXetDuyet"))
                    result.Columns.Add("IsXetDuyet", typeof(string));
                if (!result.Columns.Contains("TachMau"))
                    result.Columns.Add("TachMau", typeof(string));
                if (!result.Columns.Contains("IsNew"))
                    result.Columns.Add("IsNew", typeof(int));

                // BƯỚC 4: Tạo dictionary để tra cứu nhanh chi tiết
                var chiTietDict = new Dictionary<string, decimal>();
                if (dtChiTiet != null)
                {
                    foreach (DataRow row in dtChiTiet.Rows)
                    {
                        string key = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}",
                            row["MaPhieu"], row["MaVTID"], row["MauVTID"], row["KhoVaiID"],
                            row["MaNhom"], row["MaCode"], row["MaMau"], row["MaPhong"]);
                        decimal soLuong = row["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(row["SoLuong"]);
                        chiTietDict[key] = soLuong;
                    }
                }

                // BƯỚC 5: Fill dữ liệu vào result
                foreach (DataRow mainRow in dtMain.Rows)
                {
                    DataRow newRow = result.NewRow();

                    // Copy dữ liệu cơ bản
                    foreach (DataColumn col in dtMain.Columns)
                    {
                        if (result.Columns.Contains(col.ColumnName))
                            newRow[col.ColumnName] = mainRow[col.ColumnName];
                    }

                    // Set các cột bổ sung
                    bool isDuyet = mainRow["IsDuyet"] != DBNull.Value && Convert.ToBoolean(mainRow["IsDuyet"]);
                    newRow["IsXetDuyet"] = isDuyet ? "Đã duyệt" : "Chưa duyệt";
                    newRow["IsNew"] = 0;
                    newRow["TachMau"] = "";

                    string maPhieu = mainRow["MaPhieu"].ToString();
                    string maVTID = mainRow["MaVTID"].ToString();
                    string mauVTID = mainRow["MauVTID"].ToString();
                    string khoVaiID = mainRow["KhoVaiID"].ToString();
                    string maNhom = mainRow["MaNhom"].ToString();
                    string maCode = mainRow["MaCode"].ToString();

                    // Fill cột màu (hiển thị MauVTID)
                    for (int i = 0; i < mauMaList.Count; i++)
                    {
                        string colName = string.Format("{0}@Mau@{1}", mauTenList[i], mauMaList[i]);
                        newRow[colName] = mauVTID;
                    }

                    // Fill cột phòng+màu (hiển thị số lượng)
                    foreach (var phong in phongList)
                    {
                        for (int i = 0; i < mauMaList.Count; i++)
                        {
                            string colName = string.Format("{0}@{1}@{2}", mauTenList[i], phong, mauMaList[i]);
                            string key = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}",
                                maPhieu, maVTID, mauVTID, khoVaiID, maNhom, maCode, mauMaList[i], phong);

                            if (chiTietDict.ContainsKey(key))
                            {
                                decimal soLuong = chiTietDict[key];
                                if (soLuong != 0)
                                    newRow[colName] = Math.Round(soLuong, 2).ToString("0.##");
                                else
                                    newRow[colName] = "";
                            }
                            else
                            {
                                newRow[colName] = "";
                            }
                        }
                    }

                    result.Rows.Add(newRow);
                }

                // BƯỚC 6: Gán vào grid
                gridControl1.DataSource = result;

                // BƯỚC 7: Xử lý toggle
                tgXacNhan.EditValueChanging -= tgDuyet_EditValueChanging;
                tgXacNhan.Toggled -= tgDuyet_Toggled;
                tgXacNhan.EditValue = result.Rows[0]["IsDuyet"].ToString().ToUpper() == "TRUE";
                tgXacNhan.Properties.Appearance.ForeColor = tgXacNhan.IsOn ? Color.ForestGreen : Color.DimGray;
                tgXacNhan.Toggled += tgDuyet_Toggled;
                tgXacNhan.EditValueChanging += tgDuyet_EditValueChanging;

            }
            catch (Exception ex)
            {
            }


        }

        private void loadDataPhieu()
        {
            try
            {
                string makh = searchLookUpEditKH.EditValue?.ToString() ?? "";
                string mahang = searchLookUpEditMH.EditValue?.ToString() ?? "";
                string madot = searchLookUpEditDot.EditValue?.ToString() ?? "";
                string maphieu = searchLookUpEditPhieuXinNPL.EditValue?.ToString() ?? "";

                if (string.IsNullOrEmpty(makh) || string.IsNullOrEmpty(mahang) ||
                    string.IsNullOrEmpty(madot) || string.IsNullOrEmpty(maphieu))
                    return;

                // BƯỚC 1: Lấy 3 bảng dữ liệu
                var url1 = string.Format("{0}XinNPLMayMau/Get?action=GETERP_NPLMayMau_V1&para1={1}&para2={2}&para3={3}&para4={4}", URL, makh, mahang, madot, maphieu);
                var url2 = string.Format("{0}XinNPLMayMau/Get?action=GETBangMau_V1&para1={1}&para2={2}&para4={3}", URL, makh, mahang, maphieu);
                var url3 = string.Format("{0}XinNPLMayMau/Get?action=GETERP_NPLMayMauChiTiet_V1&para4={1}", URL, maphieu);

                var url4 = string.Format("{0}XinNPLMayMau/Get?action=GETERP_NPLMayMauChiTiet_V1EditHis&para4={1}", URL, maphieu);

                string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                string json2 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
                string json3 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url3); }).Result;

                string json4 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url4); }).Result;

                if (json1 == "[]") return;

                DataTable dtMain = JsonConvert.DeserializeObject<DataTable>(json1);
                DataTable dtMau = json2 == "[]" ? null : JsonConvert.DeserializeObject<DataTable>(json2);
                DataTable dtChiTiet = json3 == "[]" ? null : JsonConvert.DeserializeObject<DataTable>(json3);
                DataTable dtChiTietHistory = json4 == "[]" ? null : JsonConvert.DeserializeObject<DataTable>(json4);


                cellsToHighlight.Clear();
                cellsToHighlight.Clear();
                if (dtChiTiet != null && dtChiTietHistory != null && dtMain.Rows[0]["IsDuyet"].ToString().ToUpper() == "TRUE")
                {
                    // Tạo dictionary cho dtChiTietHistory để tra cứu nhanh
                    var historyDict = new Dictionary<string, decimal>();
                    foreach (DataRow rowHistory in dtChiTietHistory.Rows)
                    {
                        string key = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}",
                            rowHistory["MaPhieu"],
                            rowHistory["MaVTID"],
                            rowHistory["MauVTID"],
                            rowHistory["KhoVaiID"],
                            rowHistory["MaNhom"],
                            rowHistory["MaCode"],
                            rowHistory["MaMau"],
                            rowHistory["MaPhong"]);

                        decimal soLuongHistory = rowHistory["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(rowHistory["SoLuong"]);
                        historyDict[key] = soLuongHistory;
                    }

                    // So sánh dtChiTiet với dictionary
                    foreach (DataRow rowOriginal in dtChiTiet.Rows)
                    {
                        string maPhieu = rowOriginal["MaPhieu"].ToString();
                        string maVTID = rowOriginal["MaVTID"].ToString();
                        string mauVTID = rowOriginal["MauVTID"].ToString();
                        string khoVaiID = rowOriginal["KhoVaiID"].ToString();
                        string maNhom = rowOriginal["MaNhom"].ToString();
                        string maCode = rowOriginal["MaCode"].ToString();
                        string maMau = rowOriginal["MaMau"].ToString();
                        string maPhong = rowOriginal["MaPhong"].ToString();

                        string key = $"{maPhieu}_{maVTID}_{mauVTID}_{khoVaiID}_{maNhom}_{maCode}_{maMau}_{maPhong}";

                        decimal soLuongOriginal = rowOriginal["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(rowOriginal["SoLuong"]);

                        // Kiểm tra xem key có tồn tại trong history không
                        if (historyDict.ContainsKey(key))
                        {
                            decimal soLuongHistory = historyDict[key];

                            // Nếu số lượng khác nhau thì đánh dấu cần tô màu
                            if (soLuongOriginal != soLuongHistory)
                            {
                                cellsToHighlight[key] = true;
                            }
                        }
                        else
                        {
                            // Key không tồn tại trong history (dòng mới) - cũng tô màu
                            cellsToHighlight[key] = true;
                        }
                    }
                }

                if (dtMain == null || dtMain.Rows.Count == 0) return;


                // BƯỚC 2: Lấy danh sách màu
                var mauMaList = new List<string>();
                var mauTenList = new List<string>();
                if (dtMau != null)
                {
                    foreach (DataRow r in dtMau.Rows)
                    {
                        mauMaList.Add(r["MaMau"].ToString());
                        mauTenList.Add(r["TenMau"].ToString());
                    }
                }

                // LẤY CÁC CẶP (MaMau, MaPhong) THỰC SỰ TỒN TẠI TRONG dtChiTiet
                var mauPhongPairs = new HashSet<string>(); // Dùng HashSet để tránh trùng lặp
                if (dtChiTiet != null)
                {
                    foreach (DataRow r in dtChiTiet.Rows)
                    {
                        string maMau = r["MaMau"].ToString();
                        string maPhong = r["MaPhong"].ToString();
                        string pair = $"{maMau}|{maPhong}"; // VD: "MAU_M8|MA_P3"
                        mauPhongPairs.Add(pair);
                    }
                }

                // BƯỚC 3: Tạo DataTable kết quả
                DataTable result = dtMain.Clone();

                if (result.Columns.Contains("MauVTID"))
                    result.Columns.Remove("MauVTID");
                if (!result.Columns.Contains("MauVTIDChung"))
                    result.Columns.Add("MauVTIDChung", typeof(string));

                // Thêm cột màu thông tin (giữ nguyên)
                for (int i = 0; i < mauMaList.Count; i++)
                {
                    string colName = string.Format("{0}@Mau@{1}", mauTenList[i], mauMaList[i]);
                    if (!result.Columns.Contains(colName))
                        result.Columns.Add(colName, typeof(string));
                }

                // CHỈ THÊM CỘT CHO CÁC CẶP (MaMau, MaPhong) CÓ TRONG dtChiTiet
                foreach (var pair in mauPhongPairs)
                {
                    string[] parts = pair.Split('|');
                    string maMau = parts[0];
                    string maPhong = parts[1];

                    // Tìm tên màu tương ứng
                    int mauIndex = mauMaList.IndexOf(maMau);
                    if (mauIndex >= 0)
                    {
                        string tenMau = mauTenList[mauIndex];
                        string colName = string.Format("{0}@{1}@{2}", tenMau, maPhong, maMau);

                        if (!result.Columns.Contains(colName))
                            result.Columns.Add(colName, typeof(string));
                    }
                }
                if (!result.Columns.Contains("IsXetDuyet"))
                    result.Columns.Add("IsXetDuyet", typeof(string));
                if (!result.Columns.Contains("TachMau"))
                    result.Columns.Add("TachMau", typeof(string));
                if (!result.Columns.Contains("IsNew"))
                    result.Columns.Add("IsNew", typeof(int));

                // BƯỚC 4: Tạo dictionary chi tiết
                var chiTietDict = new Dictionary<string, decimal>();
                if (dtChiTiet != null)
                {
                    foreach (DataRow row in dtChiTiet.Rows)
                    {
                        string key = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}",
                            row["MaPhieu"], row["MaVTID"], row["MauVTID"], row["KhoVaiID"],
                            row["MaNhom"], row["MaCode"], row["MaMau"], row["MaPhong"]);
                        decimal soLuong = row["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(row["SoLuong"]);
                        chiTietDict[key] = soLuong;
                    }
                }

                // BƯỚC 5: Group dữ liệu
                var groupedData = new Dictionary<string, List<DataRow>>();
                foreach (DataRow mainRow in dtMain.Rows)
                {
                    string groupKey = string.Format("{0}_{1}_{2}_{3}_{4}",
                        mainRow["MaPhieu"], mainRow["MaVTID"], mainRow["KhoVaiID"],
                        mainRow["MaNhom"], mainRow["MaCode"]);

                    if (!groupedData.ContainsKey(groupKey))
                        groupedData[groupKey] = new List<DataRow>();

                    groupedData[groupKey].Add(mainRow);
                }

                // BƯỚC 6: Xử lý từng nhóm
                foreach (var group in groupedData)
                {
                    DataRow firstRow = group.Value[0];
                    DataRow newRow = result.NewRow();

                    // Copy dữ liệu cơ bản
                    foreach (DataColumn col in dtMain.Columns)
                    {
                        if (col.ColumnName == "MauVTID") continue;
                        if (result.Columns.Contains(col.ColumnName))
                            newRow[col.ColumnName] = firstRow[col.ColumnName];
                    }

                    // Gộp MauVTID
                    var mauVTIDList = new List<string>();
                    foreach (DataRow row in group.Value)
                    {
                        string mauVTID = row["MauVTID"].ToString();
                        if (!string.IsNullOrEmpty(mauVTID) && !mauVTIDList.Contains(mauVTID))
                            mauVTIDList.Add(mauVTID);
                    }
                    newRow["MauVTIDChung"] = string.Join(", ", mauVTIDList);

                    // Tạo map MaMau -> MauVTID từ chi tiết
                    var mauVTIDByMaMau = new Dictionary<string, string>();
                    if (dtChiTiet != null)
                    {
                        string searchMaPhieu = firstRow["MaPhieu"].ToString();
                        string searchMaVTID = firstRow["MaVTID"].ToString();
                        string searchKhoVaiID = firstRow["KhoVaiID"].ToString();
                        string searchMaNhom = firstRow["MaNhom"].ToString();
                        string searchMaCode = firstRow["MaCode"].ToString();

                        foreach (DataRow ctRow in dtChiTiet.Rows)
                        {
                            if (ctRow["MaPhieu"].ToString() == searchMaPhieu &&
                                ctRow["MaVTID"].ToString() == searchMaVTID &&
                                ctRow["KhoVaiID"].ToString() == searchKhoVaiID &&
                                ctRow["MaNhom"].ToString() == searchMaNhom &&
                                ctRow["MaCode"].ToString() == searchMaCode)
                            {
                                string maMau = ctRow["MaMau"].ToString();
                                string mauVTID = ctRow["MauVTID"].ToString();

                                if (!mauVTIDByMaMau.ContainsKey(maMau))
                                    mauVTIDByMaMau[maMau] = mauVTID;
                            }
                        }
                    }

                    bool isDuyet = firstRow["IsDuyet"] != DBNull.Value && Convert.ToBoolean(firstRow["IsDuyet"]);
                    newRow["IsXetDuyet"] = isDuyet ? "Đã duyệt" : "Chưa duyệt";
                    newRow["IsNew"] = 0;
                    newRow["TachMau"] = "";

                    string maPhieu = firstRow["MaPhieu"].ToString();
                    string maVTID = firstRow["MaVTID"].ToString();
                    string khoVaiID = firstRow["KhoVaiID"].ToString();
                    string maNhom = firstRow["MaNhom"].ToString();
                    string maCode = firstRow["MaCode"].ToString();

                    // Fill cột màu @Mau@
                    for (int i = 0; i < mauMaList.Count; i++)
                    {
                        string colName = string.Format("{0}@Mau@{1}", mauTenList[i], mauMaList[i]);
                        if (mauVTIDByMaMau.ContainsKey(mauMaList[i]))
                            newRow[colName] = mauVTIDByMaMau[mauMaList[i]];
                        else
                            newRow[colName] = "";
                    }

                    // Fill cột phòng+màu (số lượng)
                    // Fill cột phòng+màu (số lượng)
                    foreach (DataRow mainRow in group.Value)
                    {
                        string mauVTID = mainRow["MauVTID"].ToString();

                        // DUYỆT QUA CÁC CẶP (MaMau, MaPhong) THỰC SỰ TỒN TẠI
                        foreach (var pair in mauPhongPairs)
                        {
                            string[] parts = pair.Split('|');
                            string maMau = parts[0];
                            string maPhong = parts[1];

                            // Tìm tên màu tương ứng
                            int mauIndex = mauMaList.IndexOf(maMau);
                            if (mauIndex >= 0)
                            {
                                string tenMau = mauTenList[mauIndex];
                                string colName = string.Format("{0}@{1}@{2}", tenMau, maPhong, maMau);

                                // Tạo key để tìm trong chiTietDict
                                string key = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}",
                                    maPhieu, maVTID, mauVTID, khoVaiID, maNhom, maCode, maMau, maPhong);

                                if (chiTietDict.ContainsKey(key))
                                {
                                    decimal soLuong = chiTietDict[key];
                                    if (soLuong != 0)
                                    {
                                        decimal existing = 0;
                                        if (newRow[colName] != DBNull.Value && !string.IsNullOrEmpty(newRow[colName].ToString()))
                                            decimal.TryParse(newRow[colName].ToString(), out existing);

                                        newRow[colName] = Math.Round(existing + soLuong, 4).ToString("0.####");
                                    }
                                }
                            }
                        }
                    }



                    result.Rows.Add(newRow);
                }

                gridControl1.DataSource = result;

                tgXacNhan.EditValueChanging -= tgDuyet_EditValueChanging;
                tgXacNhan.Toggled -= tgDuyet_Toggled;
                tgXacNhan.EditValue = result.Rows[0]["IsDuyet"].ToString().ToUpper() == "TRUE";
                tgXacNhan.Properties.Appearance.ForeColor = tgXacNhan.IsOn ? Color.ForestGreen : Color.DimGray;
                tgXacNhan.Toggled += tgDuyet_Toggled;
                tgXacNhan.EditValueChanging += tgDuyet_EditValueChanging;


                tgSoatXet.EditValueChanging -= tgSoatXet_EditValueChanging;
                tgSoatXet.Toggled -= tgSoatXet_Toggled;
                tgSoatXet.EditValue = result.Rows[0]["IsSoatXet"].ToString().ToUpper() == "TRUE";
                tgSoatXet.Properties.Appearance.ForeColor = tgSoatXet.IsOn ? Color.ForestGreen : Color.DimGray;
                tgSoatXet.Toggled += tgSoatXet_Toggled;
                tgSoatXet.EditValueChanging += tgSoatXet_EditValueChanging;



                tgDuyet.EditValueChanging -= tgDuyet_EditValueChanging_1;
                tgDuyet.Toggled -= tgDuyet_Toggled_1;
                tgDuyet.EditValue = result.Rows[0]["IsDuyetTong"].ToString().ToUpper() == "TRUE";
                tgDuyet.Properties.Appearance.ForeColor = tgDuyet.IsOn ? Color.ForestGreen : Color.DimGray;
                tgDuyet.Toggled += tgDuyet_Toggled_1;
                tgDuyet.EditValueChanging += tgDuyet_EditValueChanging_1;

                txtGhiChu.Text = result.Rows[0]["GhiChuTong"].ToString();

                if (DateTime.TryParse(result.Rows[0]["NgayYC"].ToString(), out DateTime ngayYC))
                {
                    string ngayYCStr = ngayYC.ToString("dd/MM/yyyy");
                    dateEditNgayYC.EditValue = ngayYCStr;
                }
                searchLookUpEditMaLenh.EditValue = result.Rows[0]["MaLenh"].ToString();


                if (tblSizeKTTK == null || tblSizeKTTK.Rows.Count == 0)
                    loadSize();
                selectedRowsSize = GetSelectedRowsFromResult(result.Rows[0]["MaSizeKTTK"].ToString(), tblSizeKTTK);

                searchLookUpEditSizeKTTK.ShowPopup();

                checkSize();
                searchLookUpEditSizeKTTK.ClosePopup();

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Lỗi: {0}", ex.Message), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void checkSize()
        {
            //gVSize.SelectionChanged -= gVSize_SelectionChanged;
            GridView gridView = searchLookUpEditSizeKTTK.Properties.View as GridView;
            if (gridView != null)
            {
                gridView.ClearSelection();

                // Tạo dictionary để tìm kiếm nhanh hơn
                var selectedDict = new Dictionary<string, HashSet<string>>();
                foreach (DataRow row in selectedRowsSize)
                {
                    string maNhomSize = row["MaNhomSize"].ToString();
                    string maSize = row["MaSize"].ToString();

                    if (!selectedDict.ContainsKey(maNhomSize))
                        selectedDict[maNhomSize] = new HashSet<string>();

                    selectedDict[maNhomSize].Add(maSize);
                }

                // Duyệt qua GridView và chọn các dòng phù hợp
                for (int i = 0; i < gridView.RowCount; i++)
                {
                    var rowView = gridView.GetRow(i) as DataRowView;
                    if (rowView != null)
                    {
                        DataRow currentRow = rowView.Row;
                        string currentMaNhomSize = currentRow["MaNhomSize"].ToString();
                        string currentMaSize = currentRow["MaSize"].ToString();

                        if (selectedDict.ContainsKey(currentMaNhomSize) &&
                            selectedDict[currentMaNhomSize].Contains(currentMaSize))
                        {
                            gridView.SelectRow(i);
                        }
                    }
                }
            }
            //gVSize.SelectionChanged += gVSize_SelectionChanged;
        }


        private void setTypeTxtEditSL(object sender, KeyPressEventArgs e)
        {
            TextEdit textEdit = sender as TextEdit;
            string currentText = textEdit.Text;
            int cursorPosition = textEdit.SelectionStart;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && currentText.Contains("."))
            {
                e.Handled = true;
                return;
            }
            if (char.IsDigit(e.KeyChar))
            {
                string newText = currentText.Insert(cursorPosition, e.KeyChar.ToString());
                if (newText.Contains("."))
                {
                    int indexOfDot = newText.IndexOf('.');
                    string decimalPart = newText.Substring(indexOfDot + 1);
                    if (decimalPart.Length > 4)
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
        }

        private void gVSize_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            selectedRows.Clear();


            int[] selectedRowHandles = gVSize.GetSelectedRows();

            foreach (int rowHandle in selectedRowHandles)
            {
                if (rowHandle >= 0)
                {
                    DataRow row = gVSize.GetDataRow(rowHandle);
                    if (row != null)
                    {
                        selectedRows.Add(row);
                    }
                }
            }

            // Cập nhật display text
            searchLookUpEditSizeKTTK.Refresh();
        }



        private void ClearTableCT(string bandGoc, string bandNameParent)
        {
            if (bandedGridView1 == null) return;

            bandedGridView1.BeginUpdate();
            try
            {
                GridBand parentBand = bandedGridView1.Bands[bandGoc];
                if (parentBand == null) return;

                List<GridBand> bandsToRemove = new List<GridBand>();
                List<BandedGridColumn> columnsToRemove = new List<BandedGridColumn>();

                foreach (GridBand child in parentBand.Children)
                {
                    if (!string.IsNullOrEmpty(child.Name) &&
                        child.Name.StartsWith(bandNameParent, StringComparison.OrdinalIgnoreCase))
                    {
                        bandsToRemove.Add(child);

                        foreach (BandedGridColumn col in child.Columns)
                        {
                            columnsToRemove.Add(col);
                        }
                    }
                }

                foreach (BandedGridColumn col in columnsToRemove)
                {
                    if (bandedGridView1.Columns.Contains(col))
                    {
                        bandedGridView1.Columns.Remove(col);
                    }
                }

                foreach (GridBand band in bandsToRemove)
                {
                    parentBand.Children.Remove(band);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                bandedGridView1.EndUpdate();
            }
        }



        private void btnChonMauPhong_Click(object sender, EventArgs e)
        {
            try
            {
                if (tgXacNhan.IsOn)
                {
                    XtraMessageBox.Show("Phiếu đã được xác nhận. Không thể chỉnh sửa màu sản phẩm. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                DataTable tbl = gridControl1.DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0) return;
                frmPhanTichBOM_XinNPLMauPhong frm = new frmPhanTichBOM_XinNPLMauPhong(makh, mahang, _mamauchung, tbl, madot);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.DialogResult == DialogResult.OK)
                {
                    DataTable tblMau = frm.selectedTable;
                    if (tblMau == null || tblMau.Rows.Count == 0) return;

                    ClearTableCT("gridBandMauSP", "gridBandMauSPa");

                    // Xóa các cột thuộc nhóm P1
                    ClearTableCT("gridBandP1", "gridBandP1a");

                    // Xóa các cột thuộc nhóm P2
                    ClearTableCT("gridBandP2", "gridBandP2a");

                    // Xóa các cột thuộc nhóm P3
                    ClearTableCT("gridBandP3", "gridBandP3a");

                    // Xóa các cột thuộc nhóm P4
                    ClearTableCT("gridBandP4", "gridBandP4a");
                    DataTable dtP1 = tblMau.AsEnumerable()
                     .Where(r => r.Field<string>("MaPhong") == "MA_P1")
                     .CopyToDataTable();

                    DataTable dtP2 = tblMau.AsEnumerable()
                                     .Where(r => r.Field<string>("MaPhong") == "MA_P2")
                                     .CopyToDataTable();

                    DataTable dtP3 = tblMau.AsEnumerable()
                                     .Where(r => r.Field<string>("MaPhong") == "MA_P3")
                                     .CopyToDataTable();

                    DataTable dtP4 = tblMau.AsEnumerable()
                                     .Where(r => r.Field<string>("MaPhong") == "MA_P4")
                                     .CopyToDataTable();
                    DataTable distinctColors = tblMau.AsEnumerable()
                                                                    .GroupBy(r => new
                                                                    {
                                                                        MaMau = r.Field<string>("MaMau"),
                                                                        CodeMau = r.Field<string>("CodeMau"),
                                                                        TenMau = r.Field<string>("TenMau")
                                                                    })
                                                                    .Select(g => g.First())
                                                                    .CopyToDataTable();

                    createTableCT(distinctColors, gridBandMauSP, "gridBandMauSP", "@Mau@", "gridBandMauSPa");
                    createTableCT(dtP1, gridBandP1, "gridBandP1", "@MA_P1@", "gridBandP1a");
                    createTableCT(dtP2, gridBandP2, "gridBandP2", "@MA_P2@", "gridBandP2a");
                    createTableCT(dtP3, gridBandP3, "gridBandP3", "@MA_P3@", "gridBandP3a");
                    createTableCT(dtP4, gridBandP4, "gridBandP4", "@MA_P4@", "gridBandP4a");
                    RemoveUnusedColumnsFromTbl(tbl, tblMau);

                }
            }
            catch (Exception ex)
            {

            }

        }
        private void RemoveUnusedColumnsFromTbl(DataTable tbl, DataTable tblMau)
        {
            try
            {
                // Tạo danh sách các cột hợp lệ từ tblMau
                HashSet<string> validColumns = new HashSet<string>();

                foreach (DataRow row in tblMau.Rows)
                {
                    string tenMau = row["TenMau"].ToString();
                    string maPhong = row["MaPhong"].ToString();
                    string maMau = row["MaMau"].ToString();

                    // Tạo chuỗi theo format: TenMau@MaPhong@MaMau
                    string columnName = $"{tenMau}@{maPhong}@{maMau}";
                    validColumns.Add(columnName);
                }

                // Lấy danh sách các cột cần xóa (các cột chứa @MA_P mà không có trong validColumns)
                List<DataColumn> columnsToRemove = new List<DataColumn>();

                foreach (DataColumn col in tbl.Columns)
                {
                    if (col.ColumnName.Contains("@MA_P") && !validColumns.Contains(col.ColumnName))
                    {
                        columnsToRemove.Add(col);
                    }
                }

                // Xóa các cột
                foreach (DataColumn col in columnsToRemove)
                {
                    tbl.Columns.Remove(col);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa cột dư thừa: " + ex.Message);
            }
        }




        private void btnThuVienSL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmPhanTichBOM_XinNPLQuyTac frm = new frmPhanTichBOM_XinNPLQuyTac();
            frm.ShowDialog();


        }



        //private void btnLaySL_Click(object sender, EventArgs e)
        //{
        //    DataTable tbl = getTVSL();
        //    if (tbl == null || tbl.Rows.Count==0)
        //    {
        //        return;
        //    }

        //    DataTable tblNPL = tbl.AsEnumerable()
        //                                         .Where(r => r["TypeID"].ToString() == "1")
        //                                         .CopyToDataTable();
        //    DataTable tblCLVT = tbl.AsEnumerable()
        //                                        .Where(r => r["TypeID"].ToString() == "2")
        //                                        .CopyToDataTable();


        //    DataTable tblGrid = gridControl1.DataSource as DataTable;
        //    if ((bool)checkTinhDM.EditValue)
        //    {
        //        foreach (DataRow gridRow in tblGrid.Rows)
        //        {
        //            string nplValue = gridRow["NPL"]?.ToString();
        //            string maDVVT = gridRow["MaDVVT"]?.ToString();
        //            string maclvtValue = gridRow["MaNhom"]?.ToString();
        //            if (string.IsNullOrEmpty(nplValue)) continue;

        //            var mauValues = gridRow.Table.Columns
        //  .Cast<DataColumn>()
        //  .Where(c => c.ColumnName.Contains("@Mau@"))
        //  .Select(c => gridRow[c.ColumnName])
        //  .Distinct()
        //  .ToList();

        //            bool isOne = mauValues.Count == 1;
        //            bool stopAssignColumn = false;
        //            // Duyệt qua các column trong tblGrid (bỏ qua các cột cố định)
        //            foreach (DataColumn col in tblGrid.Columns)
        //            {

        //                if (stopAssignColumn)
        //                    break;
        //                string columnName = col.ColumnName;

        //                int nplIndex = (nplValue == "True" )? 1 : 0;
        //                // Bỏ qua các cột không phải là cột phòng (cột có format @MA_Px@)
        //                if (!columnName.Contains("@") || !columnName.Contains("MA_P"))
        //                    continue;


        //                // Tìm các dòng trong tblNPL thỏa điều kiện
        //                DataRow[] matchingRows = tblNPL.Select(
        //                    $"(MaQT = '{nplIndex}' OR MAQT=2) AND " +
        //                    $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
        //                );

        //                foreach (DataRow nplRow in matchingRows)
        //                {
        //                    string maPhong = nplRow["MaPhong"]?.ToString();

        //                    // Kiểm tra nếu tên cột chứa MaPhong
        //                    if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
        //                    {
        //                        // Gán SoLuong vào cell
        //                        decimal soLuong = nplRow["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(nplRow["SoLuong"]);
        //                        decimal dinhMuc = gridRow["DinhMucChung"] == DBNull.Value ? 0 : Convert.ToDecimal(gridRow["DinhMucChung"]);

        //                        gridRow[columnName] = soLuong * dinhMuc;
        //                        if (isOne)
        //                        {
        //                            stopAssignColumn = true;
        //                            break;
        //                        }

        //                    }
        //                }


        //                DataRow[] matchingRows2 = tblCLVT.Select(
        //                   $"(MaQT = '{maclvtValue}') AND " +
        //                   $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
        //               );

        //                foreach (DataRow nplRow in matchingRows2)
        //                {
        //                    string maPhong = nplRow["MaPhong"]?.ToString();

        //                    // Kiểm tra nếu tên cột chứa MaPhong
        //                    if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
        //                    {
        //                        // Gán SoLuong vào cell
        //                        decimal soLuong = nplRow["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(nplRow["SoLuong"]);
        //                        decimal dinhMuc = gridRow["DinhMucChung"] == DBNull.Value ? 0 : Convert.ToDecimal(gridRow["DinhMucChung"]);

        //                        gridRow[columnName] = soLuong * dinhMuc;
        //                        if (isOne)
        //                        {
        //                            stopAssignColumn = true;
        //                            break;
        //                        }

        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (DataRow gridRow in tblGrid.Rows)
        //        {
        //            string nplValue = gridRow["NPL"]?.ToString();
        //            string maDVVT = gridRow["MaDVVT"]?.ToString();
        //            string maclvtValue = gridRow["MaNhom"]?.ToString();
        //            if (string.IsNullOrEmpty(nplValue)) continue;


        //            var mauValues = gridRow.Table.Columns
        //            .Cast<DataColumn>()
        //            .Where(c => c.ColumnName.Contains("@Mau@"))
        //            .Select(c => gridRow[c.ColumnName])
        //            .Distinct()
        //            .ToList();

        //            bool isOne = mauValues.Count == 1;
        //            bool stopAssignColumn = false;
        //            // Duyệt qua các column trong tblGrid (bỏ qua các cột cố định)
        //            foreach (DataColumn col in tblGrid.Columns)
        //            {

        //                if (stopAssignColumn)
        //                    break;

        //                string columnName = col.ColumnName;

        //                int nplIndex = (nplValue == "True") ? 1 : 0;
        //                // Bỏ qua các cột không phải là cột phòng (cột có format @MA_Px@)
        //                if (!columnName.Contains("@") || !columnName.Contains("MA_P"))
        //                    continue;


        //                // Tìm các dòng trong tblNPL thỏa điều kiện
        //                DataRow[] matchingRows = tblNPL.Select(
        //                    $"(MaQT = '{nplIndex}' OR MAQT=2) AND " +
        //                    $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
        //                );

        //                foreach (DataRow nplRow in matchingRows)
        //                {
        //                    string maPhong = nplRow["MaPhong"]?.ToString();

        //                    // Kiểm tra nếu tên cột chứa MaPhong
        //                    if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
        //                    {
        //                        // Gán SoLuong vào cell
        //                        object soLuong = nplRow["SoLuong"];

        //                        gridRow[columnName] = soLuong;

        //                        if (isOne)
        //                        {
        //                            stopAssignColumn = true;
        //                            break;
        //                        }    

        //                    }
        //                }

        //                DataRow[] matchingRows2 = tblCLVT.Select(
        //                   $"(MaQT = '{maclvtValue}') AND " +
        //                   $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
        //               );

        //                foreach (DataRow nplRow in matchingRows2)
        //                {
        //                    string maPhong = nplRow["MaPhong"]?.ToString();

        //                    // Kiểm tra nếu tên cột chứa MaPhong
        //                    if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
        //                    {
        //                        // Gán SoLuong vào cell

        //                        object soLuong = nplRow["SoLuong"];

        //                        gridRow[columnName] = soLuong;
        //                        if (isOne)
        //                        {
        //                            stopAssignColumn = true;
        //                            break;
        //                        }

        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //private void btnLaySL_Click(object sender, EventArgs e)
        //{
        //    this.ActiveControl = button1;
        //    DataTable tbl = getTVSL();
        //    if (tbl == null || tbl.Rows.Count == 0)
        //    {
        //        return;
        //    }

        //    DataTable tblNPL = tbl.AsEnumerable()
        //                                         .Where(r => r["TypeID"].ToString() == "1")
        //                                         .CopyToDataTable();
        //    DataTable tblCLVT = tbl.AsEnumerable()
        //                                        .Where(r => r["TypeID"].ToString() == "2")
        //                                        .CopyToDataTable();

        //    DataTable tblGrid = gridControl1.DataSource as DataTable;

        //    if ((bool)checkTinhDM.EditValue)
        //    {
        //        foreach (DataRow gridRow in tblGrid.Rows)
        //        {
        //            string nplValue = gridRow["NPL"]?.ToString();
        //            string maDVVT = gridRow["MaDVVT"]?.ToString();
        //            string maclvtValue = gridRow["MaNhom"]?.ToString();
        //            if (string.IsNullOrEmpty(nplValue)) continue;

        //            var mauValues = gridRow.Table.Columns
        //                .Cast<DataColumn>()
        //                .Where(c => c.ColumnName.Contains("@Mau@"))
        //                .Select(c => gridRow[c.ColumnName])
        //                .Distinct()
        //                .ToList();

        //            bool isOne = mauValues.Count == 1;
        //            HashSet<string> assignedMaPhong = new HashSet<string>(); // Track MaPhong đã gán

        //            foreach (DataColumn col in tblGrid.Columns)
        //            {
        //                string columnName = col.ColumnName;
        //                int nplIndex = (nplValue == "True") ? 1 : 0;

        //                if (!columnName.Contains("@") || !columnName.Contains("MA_P"))
        //                    continue;

        //                // Tìm các dòng trong tblNPL
        //                DataRow[] matchingRows = tblNPL.Select(
        //                    $"(MaQT = '{nplIndex}' OR MAQT=2) AND " +
        //                    $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
        //                );

        //                foreach (DataRow nplRow in matchingRows)
        //                {
        //                    string maPhong = nplRow["MaPhong"]?.ToString();

        //                    if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
        //                    {
        //                        // Nếu isOne và MaPhong này đã gán rồi thì bỏ qua
        //                        if (isOne && assignedMaPhong.Contains(maPhong))
        //                            continue;

        //                        decimal soLuong = nplRow["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(nplRow["SoLuong"]);
        //                        decimal dinhMuc = gridRow["DinhMucChung"] == DBNull.Value ? 0 : Convert.ToDecimal(gridRow["DinhMucChung"]);

        //                        gridRow[columnName] = soLuong * dinhMuc;

        //                        if (isOne)
        //                        {
        //                            assignedMaPhong.Add(maPhong); // Đánh dấu MaPhong này đã gán
        //                        }
        //                        break;
        //                    }
        //                }

        //                // Tìm các dòng trong tblCLVT
        //                DataRow[] matchingRows2 = tblCLVT.Select(
        //                    $"(MaQT = '{maclvtValue}') AND " +
        //                    $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
        //                );

        //                foreach (DataRow nplRow in matchingRows2)
        //                {
        //                    string maPhong = nplRow["MaPhong"]?.ToString();

        //                    if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
        //                    {
        //                        // Nếu isOne và MaPhong này đã gán rồi thì bỏ qua
        //                        if (isOne && assignedMaPhong.Contains(maPhong))
        //                            continue;

        //                        decimal soLuong = nplRow["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(nplRow["SoLuong"]);
        //                        decimal dinhMuc = gridRow["DinhMucChung"] == DBNull.Value ? 0 : Convert.ToDecimal(gridRow["DinhMucChung"]);

        //                        gridRow[columnName] = soLuong * dinhMuc;

        //                        if (isOne)
        //                        {
        //                            assignedMaPhong.Add(maPhong); // Đánh dấu MaPhong này đã gán
        //                        }
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (DataRow gridRow in tblGrid.Rows)
        //        {
        //            string nplValue = gridRow["NPL"]?.ToString();
        //            string maDVVT = gridRow["MaDVVT"]?.ToString();
        //            string maclvtValue = gridRow["MaNhom"]?.ToString();
        //            if (string.IsNullOrEmpty(nplValue)) continue;

        //            var mauValues = gridRow.Table.Columns
        //                .Cast<DataColumn>()
        //                .Where(c => c.ColumnName.Contains("@Mau@"))
        //                .Select(c => gridRow[c.ColumnName])
        //                .Distinct()
        //                .ToList();

        //            bool isOne = mauValues.Count == 1;
        //            HashSet<string> assignedMaPhong = new HashSet<string>(); // Track MaPhong đã gán

        //            foreach (DataColumn col in tblGrid.Columns)
        //            {
        //                string columnName = col.ColumnName;
        //                int nplIndex = (nplValue == "True") ? 1 : 0;

        //                if (!columnName.Contains("@") || !columnName.Contains("MA_P"))
        //                    continue;

        //                DataRow[] matchingRows = tblNPL.Select(
        //                    $"(MaQT = '{nplIndex}' OR MAQT=2) AND " +
        //                    $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
        //                );

        //                foreach (DataRow nplRow in matchingRows)
        //                {
        //                    string maPhong = nplRow["MaPhong"]?.ToString();

        //                    if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
        //                    {
        //                        // Nếu isOne và MaPhong này đã gán rồi thì bỏ qua
        //                        if (isOne && assignedMaPhong.Contains(maPhong))
        //                            continue;

        //                        object soLuong = nplRow["SoLuong"];
        //                        gridRow[columnName] = soLuong;

        //                        if (isOne)
        //                        {
        //                            assignedMaPhong.Add(maPhong);
        //                        }
        //                        break;
        //                    }
        //                }

        //                DataRow[] matchingRows2 = tblCLVT.Select(
        //                    $"(MaQT = '{maclvtValue}') AND " +
        //                    $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
        //                );

        //                foreach (DataRow nplRow in matchingRows2)
        //                {
        //                    string maPhong = nplRow["MaPhong"]?.ToString();

        //                    if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
        //                    {
        //                        // Nếu isOne và MaPhong này đã gán rồi thì bỏ qua
        //                        if (isOne && assignedMaPhong.Contains(maPhong))
        //                            continue;

        //                        object soLuong = nplRow["SoLuong"];
        //                        gridRow[columnName] = soLuong;

        //                        if (isOne)
        //                        {
        //                            assignedMaPhong.Add(maPhong);
        //                        }
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        private void btnLaySL_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActiveControl = button1;
                DataTable tbl = getTVSL();
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    return;
                }

                DataTable tblNPL = tbl.AsEnumerable()
                                                     .Where(r => r["TypeID"].ToString() == "1")
                                                     .CopyToDataTable();
                DataTable tblCLVT = tbl.AsEnumerable()
                                                    .Where(r => r["TypeID"].ToString() == "2")
                                                    .CopyToDataTable();

                DataTable tblGrid = gridControl1.DataSource as DataTable;
                foreach (DataRow gridRow in tblGrid.Rows)
                {
                    string nplValue = gridRow["NPL"]?.ToString();
                    string maDVVT = gridRow["MaDVVT"]?.ToString();
                    string maclvtValue = gridRow["MaNhom"]?.ToString();
                    string maSizeChung = gridRow["MaSizeChung"]?.ToString();

                    if (string.IsNullOrEmpty(nplValue)) continue;

                    var mauValues = gridRow.Table.Columns
                        .Cast<DataColumn>()
                        .Where(c => c.ColumnName.Contains("@Mau@"))
                        .Select(c => gridRow[c.ColumnName])
                        .Distinct()
                        .ToList();

                    bool isOne = mauValues.Count == 1;
                    HashSet<string> assignedMaPhong = new HashSet<string>();

                    foreach (DataColumn col in tblGrid.Columns)
                    {
                        string columnName = col.ColumnName;
                        int nplIndex = (nplValue == "True") ? 1 : 0;

                        if (!columnName.Contains("@") || !columnName.Contains("MA_P"))
                            continue;

                        // Kiểm tra nếu là MA_P1
                        bool isMAP1 = columnName.Contains("MA_P1");

                        if (isMAP1)
                        {
                            if (!IsMatchingSizeChung(maSizeChung, selectedRows))
                            {
                                gridRow[columnName] = "";
                                continue;
                            }
                        }

                        // Tìm các dòng trong tblNPL
                        DataRow[] matchingRows = tblNPL.Select(
                            $"(MaQT = '{nplIndex}' OR MAQT=2) AND " +
                            $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
                        );

                        foreach (DataRow nplRow in matchingRows)
                        {
                            string maPhong = nplRow["MaPhong"]?.ToString();

                            if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
                            {
                                if (isOne && assignedMaPhong.Contains(maPhong))
                                    continue;

                                decimal soLuong = nplRow["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(nplRow["SoLuong"]);

                                // MA_P1 nhân với định mức, các cột khác chỉ lấy SoLuong
                                if (isMAP1)
                                {
                                    decimal dinhMuc = gridRow["DinhMucChung"] == DBNull.Value ? 0 : Convert.ToDecimal(gridRow["DinhMucChung"]);
                                    gridRow[columnName] = soLuong * dinhMuc;
                                }
                                else
                                {
                                    gridRow[columnName] = soLuong;
                                }

                                if (isOne)
                                {
                                    assignedMaPhong.Add(maPhong);
                                }
                                break;
                            }
                        }

                        // Tìm các dòng trong tblCLVT
                        DataRow[] matchingRows2 = tblCLVT.Select(
                            $"(MaQT = '{maclvtValue}') AND " +
                            $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
                        );

                        foreach (DataRow nplRow in matchingRows2)
                        {
                            string maPhong = nplRow["MaPhong"]?.ToString();

                            if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
                            {
                                if (isOne && assignedMaPhong.Contains(maPhong))
                                    continue;

                                decimal soLuong = nplRow["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(nplRow["SoLuong"]);

                                // MA_P1 nhân với định mức, các cột khác chỉ lấy SoLuong
                                if (isMAP1)
                                {
                                    decimal dinhMuc = gridRow["DinhMucChung"] == DBNull.Value ? 0 : Convert.ToDecimal(gridRow["DinhMucChung"]);
                                    gridRow[columnName] = soLuong * dinhMuc;
                                }
                                else
                                {
                                    gridRow[columnName] = soLuong;
                                }

                                if (isOne)
                                {
                                    assignedMaPhong.Add(maPhong);
                                }
                                break;
                            }
                        }
                    }
                }
                //if ((bool)checkTinhDM.EditValue)
                //{
                //    foreach (DataRow gridRow in tblGrid.Rows)
                //    {
                //        string nplValue = gridRow["NPL"]?.ToString();
                //        string maDVVT = gridRow["MaDVVT"]?.ToString();
                //        string maclvtValue = gridRow["MaNhom"]?.ToString();
                //        string maSizeChung = gridRow["MaSizeChung"]?.ToString(); // Lấy MaSizeChung

                //        if (string.IsNullOrEmpty(nplValue)) continue;

                //        var mauValues = gridRow.Table.Columns
                //            .Cast<DataColumn>()
                //            .Where(c => c.ColumnName.Contains("@Mau@"))
                //            .Select(c => gridRow[c.ColumnName])
                //            .Distinct()
                //            .ToList();

                //        bool isOne = mauValues.Count == 1;
                //        HashSet<string> assignedMaPhong = new HashSet<string>();

                //        foreach (DataColumn col in tblGrid.Columns)
                //        {
                //            string columnName = col.ColumnName;
                //            int nplIndex = (nplValue == "True") ? 1 : 0;

                //            if (!columnName.Contains("@") || !columnName.Contains("MA_P"))
                //                continue;

                //            // Kiểm tra nếu là MA_P1 thì phải check MaSizeChung
                //            if (columnName.Contains("MA_P1"))
                //            {
                //                if (!IsMatchingSizeChung(maSizeChung, selectedRows))
                //                {
                //                    continue; // Bỏ qua nếu không match
                //                }
                //            }

                //            // Tìm các dòng trong tblNPL
                //            DataRow[] matchingRows = tblNPL.Select(
                //                $"(MaQT = '{nplIndex}' OR MAQT=2) AND " +
                //                $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
                //            );

                //            foreach (DataRow nplRow in matchingRows)
                //            {
                //                string maPhong = nplRow["MaPhong"]?.ToString();

                //                if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
                //                {
                //                    if (isOne && assignedMaPhong.Contains(maPhong))
                //                        continue;

                //                    decimal soLuong = nplRow["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(nplRow["SoLuong"]);
                //                    decimal dinhMuc = gridRow["DinhMucChung"] == DBNull.Value ? 0 : Convert.ToDecimal(gridRow["DinhMucChung"]);

                //                    gridRow[columnName] = soLuong * dinhMuc;

                //                    if (isOne)
                //                    {
                //                        assignedMaPhong.Add(maPhong);
                //                    }
                //                    break;
                //                }
                //            }

                //            // Tìm các dòng trong tblCLVT
                //            DataRow[] matchingRows2 = tblCLVT.Select(
                //                $"(MaQT = '{maclvtValue}') AND " +
                //                $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
                //            );

                //            foreach (DataRow nplRow in matchingRows2)
                //            {
                //                string maPhong = nplRow["MaPhong"]?.ToString();

                //                if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
                //                {
                //                    if (isOne && assignedMaPhong.Contains(maPhong))
                //                        continue;

                //                    decimal soLuong = nplRow["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(nplRow["SoLuong"]);
                //                    decimal dinhMuc = gridRow["DinhMucChung"] == DBNull.Value ? 0 : Convert.ToDecimal(gridRow["DinhMucChung"]);

                //                    gridRow[columnName] = soLuong * dinhMuc;

                //                    if (isOne)
                //                    {
                //                        assignedMaPhong.Add(maPhong);
                //                    }
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    foreach (DataRow gridRow in tblGrid.Rows)
                //    {
                //        string nplValue = gridRow["NPL"]?.ToString();
                //        string maDVVT = gridRow["MaDVVT"]?.ToString();
                //        string maclvtValue = gridRow["MaNhom"]?.ToString();
                //        string maSizeChung = gridRow["MaSizeChung"]?.ToString(); // Lấy MaSizeChung

                //        if (string.IsNullOrEmpty(nplValue)) continue;

                //        var mauValues = gridRow.Table.Columns
                //            .Cast<DataColumn>()
                //            .Where(c => c.ColumnName.Contains("@Mau@"))
                //            .Select(c => gridRow[c.ColumnName])
                //            .Distinct()
                //            .ToList();

                //        bool isOne = mauValues.Count == 1;
                //        HashSet<string> assignedMaPhong = new HashSet<string>();

                //        foreach (DataColumn col in tblGrid.Columns)
                //        {
                //            string columnName = col.ColumnName;
                //            int nplIndex = (nplValue == "True") ? 1 : 0;

                //            if (!columnName.Contains("@") || !columnName.Contains("MA_P"))
                //                continue;

                //            // Kiểm tra nếu là MA_P1 thì phải check MaSizeChung
                //            if (columnName.Contains("MA_P1"))
                //            {
                //                if (!IsMatchingSizeChung(maSizeChung, selectedRows))
                //                {
                //                    continue; // Bỏ qua nếu không match
                //                }
                //            }

                //            DataRow[] matchingRows = tblNPL.Select(
                //                $"(MaQT = '{nplIndex}' OR MAQT=2) AND " +
                //                $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
                //            );

                //            foreach (DataRow nplRow in matchingRows)
                //            {
                //                string maPhong = nplRow["MaPhong"]?.ToString();

                //                if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
                //                {
                //                    if (isOne && assignedMaPhong.Contains(maPhong))
                //                        continue;

                //                    object soLuong = nplRow["SoLuong"];
                //                    gridRow[columnName] = soLuong;

                //                    if (isOne)
                //                    {
                //                        assignedMaPhong.Add(maPhong);
                //                    }
                //                    break;
                //                }
                //            }

                //            DataRow[] matchingRows2 = tblCLVT.Select(
                //                $"(MaQT = '{maclvtValue}') AND " +
                //                $"(MaDVVT = 'ALL' OR MaDVVT = '{maDVVT}')"
                //            );

                //            foreach (DataRow nplRow in matchingRows2)
                //            {
                //                string maPhong = nplRow["MaPhong"]?.ToString();

                //                if (!string.IsNullOrEmpty(maPhong) && columnName.Contains(maPhong))
                //                {
                //                    if (isOne && assignedMaPhong.Contains(maPhong))
                //                        continue;

                //                    object soLuong = nplRow["SoLuong"];
                //                    gridRow[columnName] = soLuong;

                //                    if (isOne)
                //                    {
                //                        assignedMaPhong.Add(maPhong);
                //                    }
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {


            }

        }


        // Hàm kiểm tra xem có size nào trong MaSizeChung trùng với danh sách đã chọn không
        private bool IsMatchingSizeChung(string maSizeChung, List<DataRow> selectedSizeRows)
        {

            if (selectedSizeRows == null || selectedSizeRows.Count == 0)
                return true;

            if (string.IsNullOrEmpty(maSizeChung))
                return false;
            // Parse MaSizeChung: {MaNhomSize1:MaSize1,MaSize2;MaNhomSize2:MaSize3,MaSize4}
            var sizeChungGroups = maSizeChung.Split(';');

            foreach (var group in sizeChungGroups)
            {
                var parts = group.Split(':');
                if (parts.Length != 2) continue;

                string maNhomSize = parts[0].Trim();
                var maSizes = parts[1].Split(',').Select(s => s.Trim()).ToList();

                // Kiểm tra xem có size nào trùng với selectedSizeRows không
                foreach (var selectedRow in selectedSizeRows)
                {
                    string selectedMaNhomSize = selectedRow["MaNhomSize"]?.ToString();
                    string selectedMaSize = selectedRow["MaSize"]?.ToString();

                    // Nếu trùng cả MaNhomSize và MaSize thì return true
                    if (maNhomSize == selectedMaNhomSize && maSizes.Contains(selectedMaSize))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private DataTable getTVSL()
        {
            string url = $"{URL}XinNPLMayMau/Get?action=GETALLQUITAC";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return null;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0) return null;

            return tbl;
        }
        private void loadSize()
        {
            string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            selectedRows.Clear();
            searchLookUpEditSizeKTTK.Properties.DisplayMember = "TenSize";
            searchLookUpEditSizeKTTK.Properties.ValueMember = "MaSize";
            string url = $"{URL}XinNPLMayMau/Get?action=GETSIZESP&para1={makh}&para2={mahang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblSizeKTTK = JsonConvert.DeserializeObject<DataTable>(json);
            if (tblSizeKTTK == null || tblSizeKTTK.Rows.Count == 0) return;

            searchLookUpEditSizeKTTK.Properties.DataSource = tblSizeKTTK;
            if (selectedRowsSize.Count > 0)
            {
                searchLookUpEditSizeKTTK.ShowPopup();

                checkSize();
                searchLookUpEditSizeKTTK.ClosePopup();
            }



        }
        private void searchLookUpEdit1_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            if (selectedRows.Count == 0)
            {
                e.DisplayText = string.Empty;
                return;
            }

            // Nhóm các size theo NhomSize
            var groupedByNhomSize = selectedRows
                .GroupBy(row => new
                {
                    MaNhomSize = row["MaNhomSize"],
                    NhomSize = row["NhomSize"]
                })
                .OrderBy(g => g.Key.MaNhomSize);

            List<string> nhomSizeTexts = new List<string>();

            foreach (var group in groupedByNhomSize)
            {
                // Lấy danh sách TenSize trong nhóm
                var tenSizes = group.Select(row => row["TenSize"].ToString()).Distinct();

                // Format: NhomSize:TenSize1,TenSize2
                string nhomText = $"{group.Key.NhomSize}:{string.Join(",", tenSizes)}";
                nhomSizeTexts.Add(nhomText);
            }

            // Kết hợp các nhóm bằng dấu ;
            e.DisplayText = string.Join(";", nhomSizeTexts);
        }
        //Xử lý xuất excel
        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("{0}.YeuCauNPL-{1}", searchLookUpEditMH.EditValue.ToString().Replace("_", ""), DateTime.Now.ToString("ddMMyyyy"));

            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "TemplateYeuCauNPL.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                ExportGridView(TemplateFileName, ExportFileName);

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
        public void ExportGridView(string TemplateFileName, string ExportFileName)
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
                        excelPackage.Workbook.Properties.Title = "ThongSoVatTu";

                        string templateFilePath = TemplateFileName;
                        string resultFilePath = ExportFileName;
                        FileInfo templateFile = new FileInfo(templateFilePath);
                        ExcelPackage templatePackage = new ExcelPackage(templateFile);
                        FileInfo resultFile = new FileInfo(resultFilePath);
                        templatePackage.SaveAs(resultFile);
                    }
                }

                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    DataTable tblNPL = (gridControl1.DataSource as DataTable).Copy();
                    if (tblNPL == null || tblNPL.Rows.Count == 0)
                    {
                        excelPackage.Save();
                        return;
                    }

                    int soCotMau = tblNPL.Columns
                        .Cast<DataColumn>()
                        .Count(c => c.ColumnName.Contains("@Mau@"));

                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    worksheet.Cells["C4"].Value = searchLookUpEditKH.Text.ToString();
                    worksheet.Cells["C4"].Style.Font.Bold = true;
                    worksheet.Cells["C4"].Style.Font.Size = 12;


                    worksheet.Cells["C5"].Value = searchLookUpEditMH.Text.ToString();
                    worksheet.Cells["C5"].Style.Font.Bold = true;
                    worksheet.Cells["C5"].Style.Font.Size = 12;



                    worksheet.Cells["E4"].Value = searchLookUpEditDot.Text.ToString();
                    worksheet.Cells["E4"].Style.Font.Bold = true;
                    worksheet.Cells["E4"].Style.Font.Size = 12;




                    worksheet.Cells["E5"].Value = searchLookUpEditPhieuXinNPL.Text.ToString();
                    worksheet.Cells["E5"].Style.Font.Bold = true;
                    worksheet.Cells["E5"].Style.Font.Size = 12;
                    //worksheet.Cells["A1"].Value = "CÔNG TY TNHH VIKING VIỆT NAM";
                    //worksheet.Cells["A1"].Style.Font.Bold = true;
                    //worksheet.Cells["A1"].Style.Font.Size = 12;

                    //worksheet.Cells["A1"].Value = "CÔNG TY TNHH VIKING VIỆT NAM";
                    //worksheet.Cells["A1"].Style.Font.Bold = true;
                    //worksheet.Cells["A1"].Style.Font.Size = 12;



                    int headerRow = 7;
                    int headerRow2 = 8;

                    // Header column 1-8
                    worksheet.Cells[headerRow, 1, headerRow2, 1].Merge = true;
                    worksheet.Cells[headerRow, 1].Value = "STT";
                    worksheet.Cells[headerRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    worksheet.Cells[headerRow, 2, headerRow2, 2].Merge = true;
                    worksheet.Cells[headerRow, 2].Value = "CHỦNG LOẠI CHI TIẾT";
                    worksheet.Cells[headerRow, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    worksheet.Cells[headerRow, 3, headerRow2, 3].Merge = true;
                    worksheet.Cells[headerRow, 3].Value = "ITEMCODE";
                    worksheet.Cells[headerRow, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, 3].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    worksheet.Cells[headerRow, 4, headerRow2, 4].Merge = true;
                    worksheet.Cells[headerRow, 4].Value = "MÔ TẢ";
                    worksheet.Cells[headerRow, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, 4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    worksheet.Cells[headerRow, 5, headerRow2, 5].Merge = true;
                    worksheet.Cells[headerRow, 5].Value = "KHỔ/SIZE";
                    worksheet.Cells[headerRow, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, 5].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    worksheet.Cells[headerRow, 6, headerRow2, 6].Merge = true;
                    worksheet.Cells[headerRow, 6].Value = "ĐƠN VỊ";
                    worksheet.Cells[headerRow, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    worksheet.Cells[headerRow, 7, headerRow2, 7].Merge = true;
                    worksheet.Cells[headerRow, 7].Value = "ĐỊNH MỨC";
                    worksheet.Cells[headerRow, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, 7].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    worksheet.Cells[headerRow, 8, headerRow2, 8].Merge = true;
                    worksheet.Cells[headerRow, 8].Value = "Size";
                    worksheet.Cells[headerRow, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, 8].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    var headerRange = worksheet.Cells[headerRow, 1, headerRow2, 8];

                    // Màu nền cam nhạt
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightSalmon);
                    // hoặc thử màu khác:
                    // System.Drawing.Color.Orange
                    // System.Drawing.Color.FromArgb(255, 230, 153) // cam nhạt đẹp kiểu Excel

                    // (tuỳ chọn) border cho đẹp hơn
                    headerRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    headerRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    headerRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    headerRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    int columnNow = 9;

                    // MÀU SẢN PHẨM
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + soCotMau - 1].Merge = true;
                    worksheet.Cells[headerRow, columnNow].Value = "MÀU SP";
                    worksheet.Cells[headerRow, columnNow].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, columnNow].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


                    // Màu nền cam nhạt
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + soCotMau - 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + soCotMau - 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Cyan);


                    int index = 0;
                    foreach (DataColumn dc in tblNPL.Columns)
                    {
                        if (dc.ColumnName.Contains("@Mau@"))
                        {
                            string[] parts = dc.ColumnName.Split('@');
                            string tenMau = parts[0];
                            worksheet.Cells[headerRow2, columnNow + index].Value = tenMau;
                            worksheet.Cells[headerRow2, columnNow + index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            worksheet.Cells[headerRow2, columnNow + index].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            index++;
                        }
                    }

                    columnNow += soCotMau;

                    // KĨ THUẬT TRIỂN KHAI
                    index = 0;
                    foreach (DataColumn dc in tblNPL.Columns)
                    {
                        if (dc.ColumnName.Contains("@Mau@"))
                        {
                            string[] parts = dc.ColumnName.Split('@');
                            string tenMau = parts[0]; // Ví dụ: "BLACK"


                            bool hasMA_P1 = tblNPL.Columns
                                .Cast<DataColumn>()
                                .Any(col => col.ColumnName.Contains("@MA_P1@")
                                         && col.ColumnName.Split('@')[0] == tenMau);

                            if (hasMA_P1)
                            {
                                worksheet.Cells[headerRow2, columnNow + index].Value = tenMau;
                                worksheet.Cells[headerRow2, columnNow + index].Style.HorizontalAlignment =
                                    OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                worksheet.Cells[headerRow2, columnNow + index].Style.VerticalAlignment =
                                    OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                index++;
                            }
                        }
                    }
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + index - 1].Merge = true;
                    worksheet.Cells[headerRow, columnNow].Value = "KTTK";
                    worksheet.Cells[headerRow, columnNow].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, columnNow].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + index - 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + index - 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.YellowGreen);
                    columnNow += index;

                    // TESTING

                    index = 0;
                    foreach (DataColumn dc in tblNPL.Columns)
                    {
                        if (dc.ColumnName.Contains("@Mau@"))
                        {
                            string[] parts = dc.ColumnName.Split('@');
                            string tenMau = parts[0];
                            bool hasMA_P1 = tblNPL.Columns
                                .Cast<DataColumn>()
                                .Any(col => col.ColumnName.Contains("@MA_P2@")
                                         && col.ColumnName.Split('@')[0] == tenMau);

                            if (hasMA_P1)
                            {
                                worksheet.Cells[headerRow2, columnNow + index].Value = tenMau;
                                worksheet.Cells[headerRow2, columnNow + index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                worksheet.Cells[headerRow2, columnNow + index].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                index++;
                            }

                        }
                    }
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + index - 1].Merge = true;
                    worksheet.Cells[headerRow, columnNow].Value = "TESTING";
                    worksheet.Cells[headerRow, columnNow].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, columnNow].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + index - 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + index - 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                    columnNow += index;

                    // BẢNG MÀU

                    index = 0;
                    foreach (DataColumn dc in tblNPL.Columns)
                    {
                        if (dc.ColumnName.Contains("@Mau@"))
                        {
                            string[] parts = dc.ColumnName.Split('@');
                            string tenMau = parts[0];
                            bool hasMA_P1 = tblNPL.Columns
                                .Cast<DataColumn>()
                                .Any(col => col.ColumnName.Contains("@MA_P3@")
                                         && col.ColumnName.Split('@')[0] == tenMau);

                            if (hasMA_P1)
                            {
                                worksheet.Cells[headerRow2, columnNow + index].Value = tenMau;
                                worksheet.Cells[headerRow2, columnNow + index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                worksheet.Cells[headerRow2, columnNow + index].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                index++;
                            }

                        }
                    }
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + index - 1].Merge = true;
                    worksheet.Cells[headerRow, columnNow].Value = "BẢNG MÀU";
                    worksheet.Cells[headerRow, columnNow].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, columnNow].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + index - 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + index - 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.MediumPurple);
                    columnNow += index;

                    // CƠ ĐIỆN

                    index = 0;
                    foreach (DataColumn dc in tblNPL.Columns)
                    {
                        if (dc.ColumnName.Contains("@Mau@"))
                        {
                            string[] parts = dc.ColumnName.Split('@');
                            string tenMau = parts[0];
                            bool hasMA_P1 = tblNPL.Columns
                                .Cast<DataColumn>()
                                .Any(col => col.ColumnName.Contains("@MA_P4@")
                                         && col.ColumnName.Split('@')[0] == tenMau);

                            if (hasMA_P1)
                            {
                                worksheet.Cells[headerRow2, columnNow + index].Value = tenMau;
                                worksheet.Cells[headerRow2, columnNow + index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                worksheet.Cells[headerRow2, columnNow + index].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                index++;
                            }

                        }
                    }

                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + index - 1].Merge = true;
                    worksheet.Cells[headerRow, columnNow].Value = "CƠ ĐIỆN";
                    worksheet.Cells[headerRow, columnNow].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells[headerRow, columnNow].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + index - 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[headerRow, columnNow, headerRow, columnNow + index - 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.OrangeRed);

                    var rangeTop = worksheet.Cells[1, 9, 2, columnNow + index - 1];

                    // Merge
                    rangeTop.Merge = true;
                    rangeTop.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    rangeTop.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    rangeTop.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    rangeTop.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;


                    int currentRow = headerRow2 + 1;
                    DataView dv = tblNPL.DefaultView;
                    dv.Sort = "NPL DESC, STT ASC";
                    DataTable sortedTable = dv.ToTable();
                    bool? previousNPL = null;

                    foreach (DataRow row in sortedTable.Rows)
                    {
                        bool currentNPL = Convert.ToBoolean(row["NPL"]);
                        if (previousNPL == null || previousNPL != currentNPL)
                        {
                            string tieuDe = currentNPL ? "NGUYÊN LIỆU" : "PHỤ LIỆU";
                            worksheet.Cells[currentRow, 1].Value = tieuDe;
                            worksheet.Cells[currentRow, 1, currentRow, 8].Merge = true;
                            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                            worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            worksheet.Cells[currentRow, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                            currentRow++;
                            previousNPL = currentNPL;
                        }

                        worksheet.Cells[currentRow, 1].Value = row["STT"];
                        string maNhomCanTim = row["MaNhomChiTiet"].ToString();

                        DataRow rowCLCT = tblChungLoaiChiTiet.AsEnumerable()
                                                         .FirstOrDefault(r => r.Field<string>("MaNhomChiTiet") == maNhomCanTim);

                        worksheet.Cells[currentRow, 2].Value = rowCLCT == null ? "" : rowCLCT["TenNhomChiTiet"].ToString();
                        worksheet.Cells[currentRow, 3].Value = row["MaVT"];
                        worksheet.Cells[currentRow, 4].Value = row["ChiTiet"];
                        worksheet.Cells[currentRow, 5].Value = row["KhoVai"];
                        worksheet.Cells[currentRow, 6].Value = row["TenDVVT"];
                        worksheet.Cells[currentRow, 7].Value = row["DinhMucChung"];
                        worksheet.Cells[currentRow, 8].Value = row["Size"];

                        int columnIndex = 9;
                        foreach (DataColumn dc in row.Table.Columns)
                        {
                            if (dc.ColumnName.Contains("@"))
                            {
                                if (dc.ColumnName.Contains("@Mau@"))
                                {
                                    DataRow mauvt = tblMauVT.AsEnumerable()
                                                       .FirstOrDefault(r => r["MauVTID"].ToString() == row[dc.ColumnName].ToString());
                                    worksheet.Cells[currentRow, columnIndex].Value = mauvt == null ? "" : mauvt["MaMauVT"].ToString();
                                }
                                else
                                {
                                    worksheet.Cells[currentRow, columnIndex].Value = row[dc.ColumnName];
                                }
                                columnIndex++;
                            }
                        }
                        currentRow++;
                    }

                    // Thêm border cho toàn bộ bảng (từ header đến dòng cuối cùng)
                    int lastColumn = columnNow + soCotMau - 1;
                    int lastRow = currentRow - 1;

                    using (var range = worksheet.Cells[headerRow, 1, lastRow, lastColumn])
                    {
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    // Border cho header (in đậm)
                    using (var range = worksheet.Cells[headerRow, 1, headerRow2, lastColumn])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                    //worksheet.Column(1).Width = 6;
                    //worksheet.Column(2).Width = 18;
                    //worksheet.Column(3).Width = 20;
                    worksheet.Column(4).Width = 50;
                    worksheet.Column(5).Width = 15;
                    worksheet.Column(6).Width = 10;
                    worksheet.Column(7).Width = 12;
                    worksheet.Column(8).Width = 25;
                    worksheet.Cells[headerRow, 9, headerRow2, lastColumn].AutoFitColumns();
                    excelPackage.Save();
                }
            }
            catch (Exception ex)
            {
            }

        }


        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string makh = searchLookUpEditKH.EditValue?.ToString() ?? "";
            string mahang = searchLookUpEditMH.EditValue?.ToString() ?? "";
            string madot = searchLookUpEditDot.EditValue?.ToString() ?? "";
            string maphieu = searchLookUpEditPhieuXinNPL.EditValue?.ToString() ?? "";
            string tenPhieu = searchLookUpEditPhieuXinNPL.Text?.ToString() ?? "";
            frmPhanTichBom_XinNPL_History frm = new frmPhanTichBom_XinNPL_History(makh, mahang, maphieu, madot, tenPhieu);
            frm.ShowDialog();

        }




        //xác nhận
        private void tgDuyet_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {

            if (!_PQXacNhan)
            {
                XtraMessageBox.Show("User không có quyền xác nhận phiếu. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                e.Cancel = true;
                return;
            }

            bool isSoatXet = Convert.ToBoolean(tgSoatXet.EditValue);
            bool isDuyet = Convert.ToBoolean(tgDuyet.EditValue);

            bool isOn = Convert.ToBoolean(e.NewValue);

            if (isOn)
            {

                DialogResult result = XtraMessageBox.Show(
                    "Bạn có chắc chắn muốn xác nhận phiếu không?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
            else if (isDuyet)
            {
                XtraMessageBox.Show("Phiếu đã được duyệt. Không thể hủy xác nhận. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                e.Cancel = true;
                return;
            }
            else if (isSoatXet)
            {
                XtraMessageBox.Show("Phiếu đã được soát xét. Không thể hủy xác nhận. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                e.Cancel = true;
                return;
            }
        }
        private void tgDuyet_Toggled(object sender, EventArgs e)
        {
            tgXacNhan.Properties.Appearance.ForeColor = tgXacNhan.IsOn ? Color.ForestGreen : Color.DimGray;
            bool isDuyet = tgXacNhan.IsOn;
            int _isDuyet = isDuyet ? 1 : 0;
            Duyet(_isDuyet);
            this.ActiveControl = button1;
        }
        private void Duyet(int _isDuyet)
        {
            string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            string maphieu = searchLookUpEditPhieuXinNPL.EditValue == null ? "" : searchLookUpEditPhieuXinNPL.EditValue.ToString();
            if (maphieu.ToString() == "") return;
            string url = $"{URL}XinNPLMayMau/Post?action=DUYETPHIEU&para1={makh.ToString()}&para2={mahang.ToString()}&para3={madot.ToString()}&para4={maphieu.ToString()}&para5={_isDuyet}&para6={GlobleData.UserName}";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
            if (msResult.ToLower() != "true") return;
            else
            {
                string Title = _isDuyet == 1 ? "BOM NPL may mẫu cần soát xét" : "BOM NPL may mẫu đã hủy xác nhận";
                string SendTo = "PKH";
                SendNotify(Title, SendTo);

                clsWaitForm.ShowSuccessForm(this, 3000);
            }
        }


        //soát xét
        private void tgSoatXet_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (!_PQSoatXet)
            {
                XtraMessageBox.Show("User không có quyền soát xét phiếu. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                e.Cancel = true;
                return;
            }


            bool isXacNhan = Convert.ToBoolean(tgXacNhan.EditValue);
            if (!isXacNhan)
            {
                XtraMessageBox.Show("Phiếu chưa được xác nhận. Không thể soát xét. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                e.Cancel = true;
                return;
            }
            bool isDuyet = Convert.ToBoolean(tgDuyet.EditValue);

            bool isOn = Convert.ToBoolean(e.NewValue);

            if (isOn)
            {

                DialogResult result = XtraMessageBox.Show(
                    "Bạn có chắc chắn muốn soát xét phiếu không?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
            else if (isDuyet)
            {
                XtraMessageBox.Show("Phiếu đã được duyệt. Không thể hủy xác nhận. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                e.Cancel = true;
                return;
            }
        }
        private void tgSoatXet_Toggled(object sender, EventArgs e)
        {
            tgSoatXet.Properties.Appearance.ForeColor = tgSoatXet.IsOn ? Color.ForestGreen : Color.DimGray;
            bool isDuyet = tgSoatXet.IsOn;
            int _isDuyet = isDuyet ? 1 : 0;

            SoatXet(_isDuyet);
            this.ActiveControl = button1;
        }
        private void SoatXet(int _isDuyet)
        {
            string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            string maphieu = searchLookUpEditPhieuXinNPL.EditValue == null ? "" : searchLookUpEditPhieuXinNPL.EditValue.ToString();
            if (maphieu.ToString() == "") return;
            string url = $"{URL}XinNPLMayMau/Post?action=SOATXETPHIEU&para1={makh.ToString()}&para2={mahang.ToString()}&para3={madot.ToString()}&para4={maphieu.ToString()}&para5={_isDuyet}&para6={GlobleData.UserName}";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
            if (msResult.ToLower() != "true") return;
            else
            {
                string Title = _isDuyet == 1 ? "BOM NPL may mẫu đã soát xét" : "BOM NPL may mẫu đã hủy soát xét";
                string SendTo = "PKH";
                SendNotify(Title, SendTo, "ALL", 1);
                clsWaitForm.ShowSuccessForm(this, 3000);
            }
        }

        //duyệt
        private void tgDuyet_EditValueChanging_1(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (!_PQDuyet)
            {
                XtraMessageBox.Show("User không có quyền duyệt phiếu. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                e.Cancel = true;
                return;
            }
            bool isSoatXet = Convert.ToBoolean(tgSoatXet.EditValue);
            bool isXacNhan = Convert.ToBoolean(tgXacNhan.EditValue);
            if (!isXacNhan)
            {
                XtraMessageBox.Show("Phiếu chưa được xác nhận. Không thể duyệt phiếu. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                e.Cancel = true;
                return;
            }
            if (!isSoatXet)
            {
                XtraMessageBox.Show("Phiếu chưa được soát xét. Không thể duyệt phiếu. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                e.Cancel = true;
                return;
            }
            bool isOn = Convert.ToBoolean(e.NewValue);

            if (isOn)
            {

                DialogResult result = XtraMessageBox.Show(
                    "Bạn có chắc chắn muốn duyệt phiếu không?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
        private void tgDuyet_Toggled_1(object sender, EventArgs e)
        {
            tgDuyet.Properties.Appearance.ForeColor = tgDuyet.IsOn ? Color.ForestGreen : Color.DimGray;
            bool isDuyet = tgDuyet.IsOn;
            int _isDuyet = isDuyet ? 1 : 0;
            DuyetTong(_isDuyet);
            this.ActiveControl = button1;
        }
        private void DuyetTong(int _isDuyet)
        {
            string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            string maphieu = searchLookUpEditPhieuXinNPL.EditValue == null ? "" : searchLookUpEditPhieuXinNPL.EditValue.ToString();
            if (maphieu.ToString() == "") return;
            string url = $"{URL}XinNPLMayMau/Post?action=DUYETTONGPHIEU&para1={makh.ToString()}&para2={mahang.ToString()}&para3={madot.ToString()}&para4={maphieu.ToString()}&para5={_isDuyet}&para6={GlobleData.UserName}";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
            if (msResult.ToLower() != "true") return;
            else
            {
                string Title = _isDuyet == 1 ? "BOM NPL may mẫu đã duyệt" : "BOM NPL may mẫu đã hủy duyệt";
                string SendTo = "KHO";
                SendNotify(Title, SendTo);

                clsWaitForm.ShowSuccessForm(this, 3000);
            }
        }
        private void deleteCell()
        {
            GridCell[] selectedCells = bandedGridView1.GetSelectedCells();

            // Duyệt qua từng cell được chọn
            foreach (GridCell cell in selectedCells)
            {
                // Lấy tên cột (FieldName) của cell hiện tại
                string columnName = cell.Column.FieldName;

                // Kiểm tra xem tên cột có chứa "@Ma_P" không
                if (columnName.Contains("@MA_P"))
                {
                    // Gán giá trị của cell = trống
                    bandedGridView1.SetRowCellValue(cell.RowHandle, cell.Column, string.Empty);
                }
            }
            bandedGridView1.RefreshData();
        }

        #region Send To Notify
        private void SendNotify(string Title, string SendTo, string BoPhan = "ALL", int Status = -1)
        {
            bool isDuyetXN = tgXacNhan.IsOn;
            bool isDuyetSoatXet = tgSoatXet.IsOn;

            string KhachHang = searchLookUpEditKH.Text == null ? "" : searchLookUpEditKH.Text.ToString();
            string MaHang = searchLookUpEditMH.Text == null ? "" : searchLookUpEditMH.Text.ToString();
            string Dot = searchLookUpEditDot.Text == null ? "" : searchLookUpEditDot.Text.ToString();
            string TenPhieu = searchLookUpEditPhieuXinNPL.Text == null ? "" : searchLookUpEditPhieuXinNPL.Text.ToString();


            string Detail = $"Phiếu: {TenPhieu}\nĐợt: {Dot}\nMã Hàng: {MaHang}\nKhách hàng: {KhachHang}";


            try
            {
                string url = $"{URL}SendToNotification/PushNotificationFrm?" +
                $"UserIDTao={HttpUtility.UrlEncode(GlobleData.UserName)}&" +
                $"FrmName={HttpUtility.UrlEncode("frmPhanTichBOM_XinNPL_TONG")}&" +
                $"Title={HttpUtility.UrlEncode(Title)}&" +
                $"Detail={HttpUtility.UrlEncode(Detail)}&" +
                $"SendTo={HttpUtility.UrlEncode(SendTo)}&" +
                $"BoPhan={HttpUtility.UrlEncode(BoPhan)}&" +
                $"Status={Status}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;

            }
            catch (Exception e)
            {

            }

        }
        #endregion
        private void CreateRepoSearchLookUpMaLenh()
        {
            try
            {
                searchLookUpEditMaLenh.Properties.DisplayMember = "Display";
                searchLookUpEditMaLenh.Properties.ValueMember = "MaLenh";

                string url = $"{URL}XinNPLMayMau/Get?action=GETMALENH";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                tblMaLenh = JsonConvert.DeserializeObject<DataTable>(json);
                searchLookUpEditMaLenh.Properties.DataSource = tblMaLenh;


            }
            catch (Exception ex)
            {
            }
        }
    }
}
