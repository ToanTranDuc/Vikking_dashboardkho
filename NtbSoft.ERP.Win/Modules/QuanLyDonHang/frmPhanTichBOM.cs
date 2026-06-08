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
    public partial class frmPhanTichBOM : DevExpress.XtraEditors.XtraForm
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
        private bool _isHuyDot = false;
        DataTable _tblMauSP;
        DataTable tblMauVT = new DataTable();
        DataTable tblChungLoaiChiTiet = new DataTable();
        DataTable tblSizeChung = new DataTable();
        DataTable _tblKhoSize = new DataTable();
        DataTable tblAllSize = new DataTable();
        DataTable tblItemCode = new DataTable();
        DataTable tblMaVT = new DataTable();
        DataTable tblDV = new DataTable();
        private DataTable copiedData;
        int _sttVT = 1;
        int _sttVTPL = 1;
        string _makhTQ = string.Empty, _mahangTQ = string.Empty, _maDotTQ = string.Empty;
        int _isSua = 0;
        HashSet<DataRow> skippedRows = new HashSet<DataRow>();
        public frmPhanTichBOM()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CheckPerminsion();
            _dtSize = new DataTable();

            _tblMauSP = new DataTable();
        }

        public frmPhanTichBOM(string makh, string mahang, string MaDot)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CheckPerminsion();
            _dtSize = new DataTable();
            _tblMauSP = new DataTable();
            this._makhTQ = makh;
            this._mahangTQ = mahang;
            this._maDotTQ = MaDot;


        }
        protected override void OnLoad(EventArgs e)
        {
            loadSearchLookUpItemcode();
            CheckUserXetDuyet();
            LoadTVMauVT();
            CreateSearchLookup();
            loadKhoSizeBOM();
            CreateRepoSearchLookUpChungLoaiCT();
            loadDVChungLoai();
            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never; // ← THÊM DÒNG NÀY

            if (!string.IsNullOrEmpty(_makhTQ))
            {
                searchLookUpEditKH.EditValue = _makhTQ;
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
                btnCopyBOM.Enabled = false;
                searchLookUpEditDot.EditValue = "";
                gridControl1.DataSource = new DataTable();

                toggleIsActive.Toggled -= toggleIsActive_Toggled;
                toggleIsActive.EditValueChanging -= toggleIsActive_EditValueChanging;
                _isActiveDot = false;
                toggleIsActive.EditValue = _isActiveDot;
                toggleIsActive.Properties.Appearance.ForeColor = toggleIsActive.IsOn ? Color.ForestGreen : Color.DimGray;
                toggleIsActive.Toggled += toggleIsActive_Toggled;
                toggleIsActive.EditValueChanging += toggleIsActive_EditValueChanging;


                toggleIsHuy.Toggled -= toggleIsHuy_Toggled;
                toggleIsHuy.EditValueChanging -= toggleIsHuy_EditValueChanging;
                _isHuyDot = false;
                toggleIsHuy.EditValue = _isHuyDot;
                toggleIsHuy.Properties.Appearance.ForeColor = toggleIsActive.IsOn ? Color.ForestGreen : Color.DimGray;
                toggleIsHuy.Toggled += toggleIsHuy_Toggled;
                toggleIsHuy.EditValueChanging += toggleIsHuy_EditValueChanging;

            }
            else
            {
                if (isSua) return;


                if (string.IsNullOrEmpty(_maDotTQ))
                {
                    searchLookUpEditDot.EditValue = tblDot.AsEnumerable()
       .OrderByDescending(row =>
       {
           int.TryParse(row["MaDot"].ToString().Split('|').Last(), out int value);
           return value;
       })
       .First()["MaDot"];
                }
                else
                {
                    searchLookUpEditDot.EditValue = _maDotTQ;
                }

                //btnCopyBOM.Enabled = true;
            }

        }
        private void CheckUserXetDuyet()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=CheckUserXetDuyet&para1={GlobleData.UserName}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                btnDuyetAll.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnHuyDuyetAll.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barButtonItem10.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnHuyXNPL.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                toggleIsActive.Enabled = false;
            }
            else
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    btnDuyetAll.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnHuyDuyetAll.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    toggleIsActive.Enabled = false;
                    return;
                }
                _allowActive = tbl.Rows[0]["AllowActive"].ToString() == "True" ? true : false;
                _allowDuyet = tbl.Rows[0]["AllowAdd"].ToString() == "True" ? true : false;
                _allowHuyDuyet = tbl.Rows[0]["AllowEdit"].ToString() == "True" ? true : false;

                btnDuyetAll.Visibility = _allowDuyet ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                btnHuyDuyetAll.Visibility = _allowHuyDuyet ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;


                barButtonItem10.Visibility = _allowDuyet ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                btnHuyXNPL.Visibility = _allowHuyDuyet ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                toggleIsActive.Enabled = _allowActive;
            }
        }
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
                _isSua = dtMain.Rows.Count == 0 ? 0 : dtMain.AsEnumerable()
                        .Where(r => r["IsSua"] != DBNull.Value)
                        .Select(r => Convert.ToInt32(r["IsSua"]))
                        .DefaultIfEmpty(0)
                        .Max();
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
                dtResult.Columns.Add("IsActive", typeof(int));
                dtResult.Columns.Add("MaNhomChiTiet", typeof(string));
                dtResult.Columns.Add("MauVTIDChung", typeof(string));
                dtResult.Columns.Add("MaSizeChung", typeof(string));
                dtResult.Columns.Add("Size", typeof(string));
                dtResult.Columns.Add("IsNew", typeof(int));
                dtResult.Columns.Add("DinhMucKH", typeof(decimal));
                dtResult.Columns.Add("TenDVKV", typeof(string));
                dtResult.Columns.Add("MaDVCL", typeof(string));
                dtResult.Columns.Add("MaDVDM", typeof(string));
                dtResult.Columns.Add("IsPS", typeof(int));
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
                     IsActive = r["IsActive"] != DBNull.Value ? Convert.ToInt32(r["IsActive"]) : 0,
                     MaNhomChiTiet = r["MaNhomChiTiet"]?.ToString() ?? "",
                     MaDot = r["MaDot"]?.ToString() ?? "",
                     DinhMucKH = r["DinhMucKH"] != DBNull.Value ? Convert.ToDecimal(r["DinhMucKH"]) : 0m,
                     TenDVKV = r["TenDVKV"]?.ToString() ?? "",
                     MaDVCL = r["MaDVCL"]?.ToString() ?? "",
                     MaDVDM = r["MaDVDM"]?.ToString() ?? "",
                     IsPS = r["IsPS"]?.ToString() ?? "",
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
                    resultRow["DinhMucKH"] = group.Key.DinhMucKH;
                    resultRow["TenDVKV"] = group.Key.TenDVKV;
                    resultRow["MaDVCL"] = group.Key.MaDVCL;
                    resultRow["MaDVDM"] = group.Key.MaDVDM;
                    resultRow["IsPS"] = group.Key.IsPS;
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

                var rows = dtResult.AsEnumerable();

                // Nhóm theo NPL, kiểm tra từng nhóm có tồn tại thì tất cả phải "Đã xác nhận"
                bool nplTrueOk = !rows.Any(r => r["NPL"]?.ToString() == "True")
                                 || rows.Where(r => r["NPL"]?.ToString() == "True")
                                        .All(r => r["IsXetDuyet"]?.ToString() == "Đã xác nhận");

                bool nplFalseOk = !rows.Any(r => r["NPL"]?.ToString() == "False")
                                  || rows.Where(r => r["NPL"]?.ToString() == "False")
                                         .All(r => r["IsXetDuyet"]?.ToString() == "Đã xác nhận");

                _isDuyet = nplTrueOk || nplFalseOk;
                int isActiveValue = dtResult.Rows[0]["IsActive"] != DBNull.Value ? Convert.ToInt32(dtResult.Rows[0]["IsActive"]) : 0;

                toggleIsActive.Toggled -= toggleIsActive_Toggled;
                toggleIsActive.EditValueChanging -= toggleIsActive_EditValueChanging;
                _isActiveDot = (isActiveValue == 1);
                toggleIsActive.EditValue = _isActiveDot;
                toggleIsActive.Properties.Appearance.ForeColor = toggleIsActive.IsOn ? Color.ForestGreen : Color.DimGray;
                toggleIsActive.Toggled += toggleIsActive_Toggled;
                toggleIsActive.EditValueChanging += toggleIsActive_EditValueChanging;

                toggleIsHuy.Toggled -= toggleIsHuy_Toggled;
                toggleIsHuy.EditValueChanging -= toggleIsHuy_EditValueChanging;
                _isHuyDot = (isActiveValue == 2);
                toggleIsHuy.EditValue = _isHuyDot;
                toggleIsHuy.Properties.Appearance.ForeColor = _isHuyDot ? Color.Red : Color.DimGray;

                toggleIsHuy.Toggled += toggleIsHuy_Toggled;
                toggleIsHuy.EditValueChanging += toggleIsHuy_EditValueChanging;

                if (_allowActive)
                {
                    bool hasAnyConfirmed = dtResult.AsEnumerable()
                        .Any(r => r["IsXetDuyet"]?.ToString() == "Đã xác nhận");

                    bool showToggleHuy = hasAnyConfirmed || _isHuyDot;
                    layoutControlItem17.Visibility = showToggleHuy ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

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
                toggleIsActive.Toggled += toggleIsActive_Toggled;
                SplashScreenManager.CloseForm(false);
            }


        }


        private void loadAllSize(string _makh, string _mahang)
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETSIZESP&para1={_makh.ToString()}&para2={_mahang.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                tblAllSize = JsonConvert.DeserializeObject<DataTable>(json);
            }

        }
        private void ProcessSizeData(DataTable tbl)
        {
            // Lấy tất cả cặp MaNhomSize-MaSize từ tblAllSize
            HashSet<string> allSizePairs = new HashSet<string>();

            foreach (DataRow row in tblAllSize.Rows)
            {
                string maNhomSize = row["MaNhomSize"].ToString().Trim();
                string maSize = row["MaSize"].ToString().Trim();

                if (!string.IsNullOrEmpty(maNhomSize) && !string.IsNullOrEmpty(maSize))
                {
                    string pair = $"{maNhomSize}-{maSize}";
                    allSizePairs.Add(pair);
                    Console.WriteLine($"  - '{pair}'");
                }
            }

            Console.WriteLine($"\nTổng số cặp MaNhomSize-MaSize trong tblAllSize: {allSizePairs.Count}\n");

            // Duyệt qua từng dòng trong tbl
            foreach (DataRow row in tbl.Rows)
            {
                string maSizeChung = row["MaSizeChung"].ToString();

                HashSet<string> sizePairsInRow = new HashSet<string>();

                // Tách chuỗi MaSizeChung theo format: "MaNhomSize1:MaSize1,MaSize2;MaNhomSize2:MaSize3,MaSize4"
                string[] nhomSizes = maSizeChung.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string nhomSize in nhomSizes)
                {
                    string[] parts = nhomSize.Split(':');
                    if (parts.Length == 2)
                    {
                        string maNhomSize = parts[0].Trim();

                        // Lấy các MaSize và ghép với MaNhomSize
                        string[] maSizes = parts[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string maSize in maSizes)
                        {
                            string trimmedMaSize = maSize.Trim();
                            if (!string.IsNullOrEmpty(maNhomSize) && !string.IsNullOrEmpty(trimmedMaSize))
                            {
                                string pair = $"{maNhomSize}-{trimmedMaSize}";
                                sizePairsInRow.Add(pair);
                                Console.WriteLine($"  + Thêm: '{pair}'");
                            }
                        }
                    }
                }

                Console.WriteLine($"\nTổng số cặp tìm được: {sizePairsInRow.Count}");

                // Tìm cặp thiếu
                var missingPairs = allSizePairs.Except(sizePairsInRow).ToList();
                if (missingPairs.Any())
                {
                    Console.WriteLine("Cặp MaNhomSize-MaSize thiếu:");
                    foreach (var missing in missingPairs)
                    {
                        Console.WriteLine($"  - '{missing}'");
                    }
                }

                // Tìm cặp thừa
                var extraPairs = sizePairsInRow.Except(allSizePairs).ToList();
                if (extraPairs.Any())
                {
                    Console.WriteLine("Cặp MaNhomSize-MaSize thừa (không có trong tblAllSize):");
                    foreach (var extra in extraPairs)
                    {
                        Console.WriteLine($"  - '{extra}'");
                    }
                }

                // So sánh với allSizePairs
                bool isAllSize = allSizePairs.SetEquals(sizePairsInRow);
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
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETSTTDOT&para1={kh.ToString()}&para2={mh.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                textBox1.Text = "";

                return;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            _stt = Convert.ToInt32(tbl.Rows[0][0]) + 1;
            textBox1.Text = searchLookUpEditKH.Text.ToString() + '-' + searchLookUpEditMH.Text.ToString() + '-' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "|" + _stt;
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
                if (tblMH.Rows.Count == 1)
                {
                    searchLookUpEditMH.EditValue = tblMH.Rows[0]["MaHang"];
                }
                else
                    searchLookUpEditMH.EditValue = null;


                if (!string.IsNullOrEmpty(_mahangTQ))
                    searchLookUpEditMH.EditValue = _mahangTQ;
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

                loadSizeChung();
                if (!isAdd)
                    CreateSearchLookUpDot();
                else
                {
                    CreateDotSTT();
                    LoadSizeDM();
                }
                CreateMauSPColumns();

            }
            catch (Exception ex)
            {

            }
        }

        private void repositoryItemButtonEdit1_Click(object sender, EventArgs e)
        {
            GridView view = bandedGridView1;
            object MaVTID = null, MauID = null, TenVT = null, MauVT = null, NhomVT = null, Chitiet = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                MaVTID = view.GetRowCellValue(childHandle, gridColumn3);
                MauID = view.GetRowCellValue(childHandle, gridColumn8);
                TenVT = view.GetRowCellValue(childHandle, gridColumn5);
                MauVT = view.GetRowCellValue(childHandle, gridColumn10);
                NhomVT = view.GetRowCellValue(childHandle, gridColumn2);
                Chitiet = view.GetRowCellValue(childHandle, gridColumn6);
            }
            else
            {
                MaVTID = view.GetFocusedRowCellValue(gridColumn3);
                MauID = view.GetFocusedRowCellValue(gridColumn8);
                TenVT = view.GetFocusedRowCellValue(gridColumn5);
                MauVT = view.GetFocusedRowCellValue(gridColumn10);
                NhomVT = view.GetFocusedRowCellValue(gridColumn2);
                Chitiet = view.GetFocusedRowCellValue(gridColumn6);
            }
            //frmKhoiTaoDM frm = new frmKhoiTaoDM();
            //frm.ShowDialog();
            //LoadChiTiet();

        }


        private int FindFirstDataRowHandle(GridView view, int groupHandle)
        {
            int childCount = view.GetChildRowCount(groupHandle);
            for (int i = 0; i < childCount; i++)
            {
                int childHandle = view.GetChildRowHandle(groupHandle, i);
                if (view.IsGroupRow(childHandle))
                {
                    // Đệ quy vào group con
                    int result = FindFirstDataRowHandle(view, childHandle);
                    if (result >= 0)
                        return result;
                }
                else
                {
                    // Đây là data row thực sự
                    return childHandle;
                }
            }
            return -1;
        }
        private void LoadChiTiet()
        {

            GridView view = bandedGridView1;
            object MaVTID = null, MauID = null, MaKhoVT = null, TachMau = null, MaNhom = null, MaCode = null;

            int levelGroup = bandedGridView1.GetRowLevel(bandedGridView1.FocusedRowHandle);
            int focusedHandle = view.FocusedRowHandle;

            if (view.IsGroupRow(focusedHandle))
            {
                int dataHandle = FindFirstDataRowHandle(view, focusedHandle);
                if (dataHandle >= 0)
                {
                    MaVTID = view.GetRowCellValue(dataHandle, gridColumn8);
                    MauID = view.GetRowCellValue(dataHandle, gridColumn13);
                    MaKhoVT = view.GetRowCellValue(dataHandle, gridColumn18);
                    TachMau = view.GetRowCellValue(dataHandle, gridColumn33);
                    MaNhom = view.GetRowCellValue(dataHandle, gridColumn4);
                    MaCode = view.GetRowCellValue(dataHandle, gridColumn49);

                }
            }
            else
            {
                MaVTID = view.GetRowCellValue(focusedHandle, gridColumn8);
                MauID = view.GetRowCellValue(focusedHandle, gridColumn13);
                MaKhoVT = view.GetRowCellValue(focusedHandle, gridColumn18);
                TachMau = view.GetRowCellValue(focusedHandle, gridColumn33);
                MaNhom = view.GetRowCellValue(focusedHandle, gridColumn4);
                MaCode = view.GetRowCellValue(focusedHandle, gridColumn49);
            }
            if (MaVTID == null && MauID == null && MaKhoVT == null && TachMau == null && MaNhom == null && MaCode == null)
            {

                return;
            }
            int focusedRowHandle = view.FocusedRowHandle;
            string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&mauID={4}&&makhoID={5}&&madot={6}&&tachmau={7}&&manhom={8}&&macode={9}", URL + "PhanTichBom/GetChiTietVTDotDM", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(),
                           searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), MaVTID == null ? "" : MaVTID.ToString(), MauID == null ? "" : MauID.ToString(), MaKhoVT == null ? "" : MaKhoVT.ToString(),
                           searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(), TachMau.ToString() != "" ? Convert.ToBoolean(TachMau.ToString()) : false, MaNhom.ToString(), MaCode.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            if (tbl.Rows.Count > 0)
            {

                DataRow dr = bandedGridView1.GetFocusedDataRow();
                string[] mamauArr = dr["MaMau"].ToString().Split('|');

                HashSet<string> mamauSet = new HashSet<string>(mamauArr);

                // Duyệt ngược để tránh lỗi khi xóa dòng trong quá trình duyệt
                for (int i = tbl.Rows.Count - 1; i >= 0; i--)
                {
                    string maMau = tbl.Rows[i]["MaMau"].ToString();
                    if (!mamauSet.Contains(maMau))
                    {
                        tbl.Rows.RemoveAt(i);
                    }
                }
                //// Cuối cùng gọi AcceptChanges để cập nhật thay đổi
                tbl.AcceptChanges();
                var groupedRows = tbl.AsEnumerable()
                                         .GroupBy(row => row["MaMau"])
                                         .ToList();

                foreach (var group in groupedRows)
                {
                    Decimal totalForGroup = 0;
                    int countColumnsWithAt = 0;
                    int rowsInGroup = group.Count();

                    foreach (var row in group)
                    {
                        int columnCount = tbl.Columns.Count;

                        for (int i = 0; i < columnCount; i++)
                        {
                            string columnName = tbl.Columns[i].ColumnName;
                            if (columnName.Contains("@"))
                            {
                                var columnValue = row[columnName];
                                if (columnValue != DBNull.Value && Convert.ToDecimal(columnValue) != 0)
                                {
                                    totalForGroup += Convert.ToDecimal(columnValue) * 1;
                                    countColumnsWithAt++;
                                }
                            }
                            else if (i + 1 < columnCount && tbl.Columns[i + 1].ColumnName.Contains("@"))
                            {
                                var nextColumnValue = row[tbl.Columns[i + 1].ColumnName];
                                if (nextColumnValue != DBNull.Value && Convert.ToDecimal(nextColumnValue) != 0)
                                {
                                    totalForGroup += Convert.ToDecimal(nextColumnValue);
                                    countColumnsWithAt++;
                                }
                            }
                        }
                    }
                    if (countColumnsWithAt > 0)
                    {
                        Decimal average = Math.Round(totalForGroup / countColumnsWithAt, 4);

                        foreach (var row in group)
                        {
                            row["DinhMucGY"] = average;
                        }
                    }
                }
                DataTable newTbl = new DataTable();
                foreach (DataColumn col in tbl.Columns)
                {
                    if (col.ColumnName == "IsXetDuyet")
                        newTbl.Columns.Add(col.ColumnName, typeof(string));
                    else
                        newTbl.Columns.Add(col.ColumnName, col.DataType);
                }
                foreach (DataRow row in tbl.Rows)
                {
                    DataRow newRow = newTbl.NewRow();
                    foreach (DataColumn col in tbl.Columns)
                    {
                        if (col.ColumnName == "IsXetDuyet")
                            newRow[col.ColumnName] = row[col.ColumnName].ToString();
                        else
                            newRow[col.ColumnName] = row[col.ColumnName];
                    }
                    newTbl.Rows.Add(newRow);
                }

                foreach (DataRow row in newTbl.Rows)
                {

                    string maMau = row["MaMau"].ToString();
                    if (string.IsNullOrEmpty(maMau)) continue;
                    string manhomsize = row["MaNhomSize"].ToString();
                    row["MaVTID"] = dr["MaVTID"];
                    row["MauVTID"] = dr["MauVTID"];
                    row["KhoVaiID"] = dr["KhoVaiID"];
                    row["MaNhom"] = dr["MaNhom"];
                    row["MaCode"] = dr["MaCode"];

                    // Kiểm tra trùng
                    bool isDuplicate = _dtSize.AsEnumerable().Any(r =>
                        r["MaVTID"].ToString() == dr["MaVTID"].ToString() &&
                        r["MauVTID"].ToString() == dr["MauVTID"].ToString() &&
                        r["KhoVaiID"].ToString() == dr["KhoVaiID"].ToString() &&
                        r["MaNhom"].ToString() == dr["MaNhom"].ToString() &&
                        r["MaMau"].ToString() == maMau &&
                        r["MaNhomSize"].ToString() == manhomsize &&
                        r["MaCode"].ToString() == dr["MaCode"].ToString()
                    );

                    if (!isDuplicate)
                    {
                        _dtSize.ImportRow(row);
                    }



                }
                //foreach(DataRow dr1 in tbl.Rows)
                //{
                //    foreach(DataRow dr2 in _dtSize.Rows)
                //    {
                //        if(dr1["MaVTID"].ToString()==dr2["MaVTID"].ToString()&& dr1["MauVTID"].ToString() == dr2["MauVTID"].ToString()&& dr1["KhoVaiID"].ToString() == dr2["KhoVaiID"].ToString()
                //            && dr1["MaNhom"].ToString() == dr2["MaNhom"].ToString()&& dr1["MaCode"].ToString() == dr2["MaCode"].ToString())
                //        {
                //            dr2["IsXetDuyet"] = dr1["IsXetDuyet"].ToString();
                //        }    
                //    }    
                //}   
                var query = from dr1 in tbl.AsEnumerable()
                            join dr2 in _dtSize.AsEnumerable()
                            on new
                            {
                                MaVTID = dr1["MaVTID"].ToString(),
                                MauVTID = dr1["MauVTID"].ToString(),
                                KhoVaiID = dr1["KhoVaiID"].ToString(),
                                MaNhom = dr1["MaNhom"].ToString(),
                                MaCode = dr1["MaCode"].ToString()
                            }
                            equals new
                            {
                                MaVTID = dr2["MaVTID"].ToString(),
                                MauVTID = dr2["MauVTID"].ToString(),
                                KhoVaiID = dr2["KhoVaiID"].ToString(),
                                MaNhom = dr2["MaNhom"].ToString(),
                                MaCode = dr2["MaCode"].ToString()
                            }
                            select new { dr1, dr2 };

                foreach (var item in query)
                {
                    item.dr2["IsXetDuyet"] = item.dr1["IsXetDuyet"].ToString();
                }

                //createTable(_dtSize);


                DataTable tblSizeFiltered = _dtSize.Clone();
                var filteredRows = _dtSize.AsEnumerable()
                .Where(row => row.Field<string>("MaVTID") == dr["MaVTID"].ToString() &&
                              row.Field<string>("MauVTID") == dr["MauVTID"].ToString() &&
                              row.Field<string>("KhoVaiID") == dr["KhoVaiID"].ToString() &&
                              row.Field<string>("MaNhom") == dr["MaNhom"].ToString() &&
                              row.Field<string>("MaCode") == dr["MaCode"].ToString());
                foreach (var row in filteredRows)
                {
                    tblSizeFiltered.ImportRow(row);
                }







            }
            else
            {
                DataRow dr = bandedGridView1.GetFocusedDataRow();
                DataTable tblSizeFiltered = _dtSize.Clone();
                var filteredRows = _dtSize.AsEnumerable()
                .Where(row => row.Field<string>("MaVTID") == dr["MaVTID"].ToString() &&
                              row.Field<string>("MauVTID") == dr["MauVTID"].ToString() &&
                              row.Field<string>("KhoVaiID") == dr["KhoVaiID"].ToString() &&
                              row.Field<string>("MaNhom") == dr["MaNhom"].ToString() &&
                               row.Field<string>("MaCode") == dr["MaCode"].ToString());
                foreach (var row in filteredRows)
                {
                    tblSizeFiltered.ImportRow(row);
                }




            }

            //if ((bool)checkBox1.Checked == true)
            //{
            //    //layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //}
            //else
            //   // layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }


        //private void createTable(DataTable tab)
        //{
        //    bandedbandedGridView1.OptionsView.AllowCellMerge = true;
        //    GridBand parentBand = bandedbandedGridView1.Bands["gridBandSizeDM"];
        //    if (parentBand != null)
        //    {
        //        parentBand.Children.Clear();
        //        parentBand.Columns.Clear();
        //        for (int i = 0; i < bandedbandedGridView1.Columns.Count;)
        //        {
        //            if (bandedbandedGridView1.Columns[i].FieldName.Contains("@Size@"))
        //            {
        //                bandedbandedGridView1.Columns.RemoveAt(i);
        //            }
        //            else
        //            {
        //                i += 1;
        //            }
        //        }
        //    }
        //    int demColIndex = -1;

        //    foreach (DataColumn column in tab.Columns)
        //    {
        //        demColIndex++;
        //        if (demColIndex > 21 && demColIndex < tab.Columns.Count)
        //        {
        //            string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
        //            string colName = arrName[0];
        //            BandedGridColumn col = new BandedGridColumn();
        //            col.AppearanceCell.Options.UseTextOptions = true;
        //            col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //            col.AppearanceHeader.Options.UseTextOptions = true;
        //            col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //            col.Caption = colName;
        //            col.FieldName = column.ColumnName;
        //            col.Name = "col" + colName;
        //            //col.OptionsColumn.AllowEdit = false;
        //            col.Visible = true;
        //            col.Width = 85;
        //            //col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
        //            col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        //            //col.DisplayFormat.FormatString = "{0:##,0}";
        //            col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
        //            bandedbandedGridView1.Columns.AddRange(new BandedGridColumn[] { col });
        //            GridBand gb = new GridBand();
        //            gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //            gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
        //            gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
        //            gb.AppearanceHeader.Options.UseTextOptions = true;
        //            gb.Caption = colName;
        //            gb.Columns.Add(col);
        //            gb.Name = "gbColBandSize" + col;
        //            gb.VisibleIndex = 0;
        //            gb.Width = 85;
        //            gridBandSizeDM.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
        //        }
        //    }

        //}

        private void bandedGridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            Pen headerBorderPen = new Pen(Color.FromArgb(100, 128, 128, 128), 3);
            e.Graphics.DrawRectangle(headerBorderPen, e.Bounds);

        }



        private void splitContainerControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void searchLookUpEditKH_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            if (string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString()))
            {
                e.DisplayText = "Chọn khách hàng";
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmERPThongSoVatTu frm = new frmERPThongSoVatTu();
            frm.ShowDialog();
            LoadData();
            LoadChiTiet();


        }
        private void NapLai(bool isSua = false)
        {

            isAdd = false;
            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            CreateSearchLookUpDot(isSua);
            CreateDotSTT();

            LoadData();


        }
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NapLai(!isAdd);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            isAdd = true;
            _sttVT = 1;
            _sttVTPL = 1;
            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem20.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            emptySpaceItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            emptySpaceItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            btnCopyBOM.Enabled = true;
            _isActiveDot = false;
            _isHuyDot = false;
            CreateDotSTT();

            LoadSizeDM();

            gridControl1.DataSource = null;
            //gridControl2.DataSource = null;
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
        private void simpleButton11_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn khách hàng.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(searchLookUpEditMH.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn mã hàng.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Duyet();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn khách hàng.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(searchLookUpEditMH.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn mã hàng.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            HuyDuyet();
        }

        private void Duyet()
        {
            //this.ActiveControl = this.Button1;
            GridView view = bandedGridView1;
            object MaVTID = null, MauID = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                MaVTID = view.GetRowCellValue(childHandle, gridColumn3);
                MauID = view.GetRowCellValue(childHandle, gridColumn8);
            }
            else
            {
                MaVTID = view.GetFocusedRowCellValue(gridColumn3);
                MauID = view.GetFocusedRowCellValue(gridColumn8);
            }
            string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}", URL + "PhanTichBom/GetVTSPDotDM", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(),
                       searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), MaVTID == null ? "" : MaVTID.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblvt = JsonConvert.DeserializeObject<DataTable>(json);

            DataTable tblsave = createTablesave();

            foreach (DataRow row in tblvt.Rows)
            {
                DataRow _InsertRow = tblsave.NewRow();
                _InsertRow["ID"] = -1;
                _InsertRow["MaKH"] = searchLookUpEditKH.EditValue.ToString().Trim();
                _InsertRow["MaHang"] = searchLookUpEditMH.EditValue.ToString().Trim();
                _InsertRow["MaVTID"] = MaVTID;
                _InsertRow["MauID"] = row["MauID"].ToString() ?? "";
                _InsertRow["KhoVaiID"] = row["KhoVaiID"].ToString() ?? "";
                _InsertRow["MaDot"] = row["MaDot"].ToString() ?? "";
                _InsertRow["Dot"] = row["Dot"].ToString() ?? "";
                _InsertRow["NguoiTao"] = "";
                _InsertRow["NgayTao"] = (object)null ?? DBNull.Value;
                _InsertRow["NguoiXet"] = GlobleData.UserName;
                _InsertRow["NgayXet"] = DateTime.Now;
                _InsertRow["NguoiSua"] = "";
                _InsertRow["NgaySua"] = (object)null ?? DBNull.Value;
                _InsertRow["IsXetDuyet"] = 1;
                _InsertRow["NguoiHuy"] = "";
                _InsertRow["NgayHuy"] = (object)null ?? DBNull.Value;
                tblsave.Rows.Add(_InsertRow);
            }
            string urlSaveDuyet = string.Format("{0}", URL + "PhanTichBom/POSTDUYETALL");
            string savelistduyet = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveDuyet, tblsave); }).Result;

            if (string.Compare(savelistduyet, "True") != 0)
            {
                XtraMessageBox.Show("Lỗi vui lòng kiểm tra lại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            else
            {
                LoadChiTiet();
                return;

            }

        }
        private void HuyDuyet()
        {
            //this.ActiveControl = this.Button1;
            GridView view = bandedGridView1;
            object MaVTID = null, MauID = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                MaVTID = view.GetRowCellValue(childHandle, gridColumn3);
                MauID = view.GetRowCellValue(childHandle, gridColumn8);
            }
            else
            {
                MaVTID = view.GetFocusedRowCellValue(gridColumn3);
                MauID = view.GetFocusedRowCellValue(gridColumn8);
            }
            string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}", URL + "PhanTichBom/GetVTSPDotDM", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(),
                       searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), MaVTID == null ? "" : MaVTID.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblvt = JsonConvert.DeserializeObject<DataTable>(json);

            DataTable tblsave = createTablesave();

            foreach (DataRow row in tblvt.Rows)
            {
                DataRow _InsertRow = tblsave.NewRow();
                _InsertRow["ID"] = -1;
                _InsertRow["MaKH"] = searchLookUpEditKH.EditValue.ToString().Trim();
                _InsertRow["MaHang"] = searchLookUpEditMH.EditValue.ToString().Trim();
                _InsertRow["MaVTID"] = MaVTID;
                _InsertRow["MauID"] = row["MauID"].ToString() ?? "";
                _InsertRow["KhoVaiID"] = row["KhoVaiID"].ToString() ?? "";
                _InsertRow["MaDot"] = row["MaDot"].ToString() ?? "";
                _InsertRow["Dot"] = row["Dot"].ToString() ?? "";
                _InsertRow["NguoiTao"] = "";
                _InsertRow["NgayTao"] = (object)null ?? DBNull.Value;
                _InsertRow["NguoiXet"] = "";
                _InsertRow["NgayXet"] = (object)null ?? DBNull.Value;
                _InsertRow["NguoiSua"] = "";
                _InsertRow["NgaySua"] = (object)null ?? DBNull.Value;
                _InsertRow["IsXetDuyet"] = 0;
                _InsertRow["NguoiHuy"] = GlobleData.UserName;
                _InsertRow["NgayHuy"] = DateTime.Now;
                tblsave.Rows.Add(_InsertRow);
            }
            string urlSaveHuy = string.Format("{0}", URL + "PhanTichBom/POSTHUYALL");
            string savelistHuy = Task.Run(async () => { return await _clientExtension.PostAsync(urlSaveHuy, tblsave); }).Result;
            if (string.Compare(savelistHuy, "True") != 0)
            {
                XtraMessageBox.Show("Lỗi vui lòng kiểm tra lại.!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            else
            {
                LoadChiTiet();
                return;

            }

        }

        private void bandedbandedGridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            //GridView View = sender as GridView;
            //if (e.Column == bandedGridColumn11)
            //{
            //    object isXetDuyetValue = e.Value;
            //    if (isXetDuyetValue != null && bool.TryParse(isXetDuyetValue.ToString(), out bool isXetDuyet))
            //    {
            //        e.DisplayText = isXetDuyet ? "Đã Duyệt" : "Chưa Duyệt";

            //    }
            //    else
            //    {
            //        return;
            //    }

            //}
            if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
            {
                e.DisplayText = "-";
            }
            else
            {
                string[] arr = e.Column.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Count() > 1)
                {
                    if (Convert.ToDecimal(e.Value.ToString()) == 0)
                        e.DisplayText = "-";
                }
            }
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
                barButtonItem3.Enabled = false;
                barButtonItem7.Enabled = false;
            }
            if (!_allowEdit)
            {
                //barButtonItem5.Enabled = false;
            }

        }


        private void MenuDuyetDong(object sender, EventArgs e)
        {
            XetDuyet("duyệt", true, true);

        }


        private void MenuHuyDong(object sender, EventArgs e)
        {
            XetDuyet("hủy duyệt", false, true);

        }


        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            XetDuyet("duyệt", true, false);
        }
        private void XetDuyet(string title, bool xetduyet, bool checkduyet)
        {
            DataRow dr = bandedGridView1.GetFocusedDataRow();

            if (dr == null) return;
            string MauVTID = dr["MauVTID"].ToString();

            string MaKH = dr["MaKH"].ToString();
            string MaHang = dr["MaHang"].ToString();
            string checkDuyetDot = checkduyet ? "" : "";

            if (!xetduyet)
            {
                string url2 = $"{URL}UpdateXetDuyet/Get?Action=CheckVT&para={MauVTID}&para2={checkDuyetDot}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
                if (json != "[]")
                {
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    if (!checkduyet)
                        MessageBox.Show($"Có {tbl.Rows.Count} Đợt đã được xét duyện không thể hủy ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                    {
                        MessageBox.Show($"Đợt này đã được cấp phát không thể hủy ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            //var result = checkduyet ? DialogResult.Yes : MessageBox.Show($"Bạn muốn {title} cây vải này", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (result == DialogResult.Yes)
            //{
            string url = $"{URL}UpdateXetDuyet/UpdateXetDuyet?Action=UpdateXetDuyen&para={xetduyet.ToString()}&para2={GlobleData.UserName}&para3={MaKH}&para4={MaHang}&para5={checkDuyetDot}&para6={MauVTID}";
            string jsons = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            LoadChiTiet();
            clsWaitForm.ShowSuccessForm(this, 2000);
            //}
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            XetDuyet("hủy duyệt", false, false);
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
        private void btnXuatExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                string dot = string.Empty;
                if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                    return;
                if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                    return;

                if (textBox1.Text != null)
                {
                    dot = textBox1.Text;
                }
                if (searchLookUpEditDot.EditValue != null)
                {
                    dot = searchLookUpEditDot.EditValue.ToString();
                }
                if (dot == null || string.IsNullOrWhiteSpace(dot.ToString()))
                    return;

                DataTable tblGC = gridControl1.DataSource as DataTable;
                if (tblGC == null || tblGC.Rows.Count == 0)
                    return;

                DataTable _tblAllSize = loadDataExcel(searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString());
                if (_tblAllSize == null || _tblAllSize.Rows.Count == 0) return;

                frmPhanTichBom_Excel frm = new frmPhanTichBom_Excel(tblGC, _tblAllSize, searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString(), dot);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();

            }
            catch (Exception ex)
            {
            }
        }

        private DataTable loadDataExcel(string kh, string mh)
        {
            string url = $"{URL}PhanTichBom/GetExcel2?makh={kh}&mahang={mh}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                return null;
            }
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
                return null;
            return tbl;
        }

        private void searchLookUpEditDot_EditValueChanged(object sender, EventArgs e)
        {
            loadSearchLookUpItemcode();
            LoadSizeDM();
            LoadData();


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
                XtraMessageBox.Show("User không có quyền Xác nhận BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }

            duyetAll(1);
            if (!GetIsXacNhan())
                UpdateNguyenLieu();
            DoBOMHangLoat();

        }

        private void btnCopyBOM_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (isAdd)
            {
                string maKH = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string maHang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string maDot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                DataTable tblGrid = gridControl1.DataSource as DataTable;
                if (tblGrid == null)
                    tblGrid = createTableSaveEdit();
                frmPhanTichBOM_Copy frmCopy = new frmPhanTichBOM_Copy(maKH, maHang, maDot, tblGrid);

                if (frmCopy.ShowDialog() == DialogResult.OK)
                {
                    DataTable tbl = frmCopy.ResultTable;
                    if (tbl != null)
                    {
                        foreach (DataRow dr in tbl.Rows)
                        {
                            string[] mauvtarr = dr["MauVTIDChung"].ToString().Split(',');
                            if (mauvtarr.Length == 1)
                            {
                                //foreach (DataColumn dc in dr.Table.Columns)
                                //{
                                //    if (dc.ColumnName.Contains("@Mau@"))
                                //    {
                                //        dr[dc.ColumnName] = mauvtarr[0];
                                //    }
                                //}
                                bool allMauNull = dr.Table.Columns
                                .Cast<DataColumn>()
                                .Where(dc => dc.ColumnName.Contains("@Mau@"))
                                .All(dc => dr[dc.ColumnName] == DBNull.Value || string.IsNullOrEmpty(dr[dc.ColumnName].ToString()));

                                if (allMauNull)
                                {
                                    foreach (DataColumn dc in dr.Table.Columns)
                                    {
                                        if (dc.ColumnName.Contains("@Mau@"))
                                        {
                                            dr[dc.ColumnName] = mauvtarr[0];
                                        }
                                    }
                                }
                            }


                        }

                        gridControl1.DataSource = tbl;

                    }
                }
            }
            else
            {
                XtraMessageBox.Show("Chức năng sao chép BOM chỉ khả dụng khi bạn đang khai báo đợt mới. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
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
            tbl.Columns.Add("DinhMucKH", typeof(Decimal));
            tbl.Columns.Add("STT", typeof(int));
            tbl.Columns.Add("MaDVCL", typeof(string));
            tbl.Columns.Add("MaDVDM", typeof(string));
            tbl.Columns.Add("IsPS", typeof(int));
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

        private void RemoveRowGCNL(DataRow rowRemove)
        {
            try
            {
                if (rowRemove == null) return;
                DataTable tbl = gridControl1.DataSource as DataTable;
                if (tbl == null) return;




                string filter = string.Format("MaVTID = '{0}' AND KhoVaiID = '{1}' AND MaNhom = '{2}' AND MaDVVT = '{3}'",
                                              rowRemove["MaVTID"], rowRemove["KhoVaiID"], rowRemove["MaNhom"], rowRemove["MaDVVT"]);

                DataRow[] rowsToDelete = tbl.Select(filter);


                foreach (DataRow dr in rowsToDelete)
                {
                    tbl.Rows.Remove(dr);
                }



            }
            catch (Exception ex)
            { }


        }



        private void btnHuyDuyetAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                return;
            if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                return;

            string url = string.Format("{0}?username={1}", URL + "KhoiTaoDM/KiemTraHuy", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                XtraMessageBox.Show("User không có quyền hủy BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }


            duyetAll(0);
            LoadData();
        }
        private void duyetAll(int isduyet)
        {
            try
            {
                if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                    return;
                if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                    return;

                if (isduyet == 0 && toggleIsActive.IsOn)
                {

                    XtraMessageBox.Show("Đợt này đang được Active. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                DataTable tbl = gridControl1.DataSource as DataTable;

                var filteredRows = tbl.Rows
              .Cast<DataRow>()
              .Where(r => Convert.ToBoolean(r["NPL"]) == true)
              .ToList();

                if (filteredRows.Count == 0)
                    return;

                if (isduyet == 1)
                {
                    if (!KiemTraDuLieuTuDataTable(tbl, true))
                    {
                        XtraMessageBox.Show(
                            "Có dòng không hợp lệ (dòng đc tô màu).\nVui lòng kiểm tra lại!",
                            Properties.Resources.InfoTitle,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                }


               
              
                string _makh = searchLookUpEditKH.EditValue.ToString();
                string _mahang = searchLookUpEditMH.EditValue.ToString();
                string _madot = searchLookUpEditDot.EditValue.ToString();


                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang xử lý yêu cầu");
                string url = $"{URL}KhoiTaoBOMV1/Post1?Action=XacNhan&para1={_makh}&para2={_mahang}&para3={_madot}&para4={isduyet}&para5={GlobleData.UserName}&para6=1";
                string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
                if (json == "True")
                {


                    string title = "BOM NL đã bị hủy xác nhận";
                    if (isduyet == 1)
                    {
                        title = "BOM NL đã xác nhận";
                    }
                    SendNotify(title, "PKT", "ALL", 1);

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



        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (searchLookUpEditKH.EditValue == null || searchLookUpEditDot.EditValue == null || searchLookUpEditMH.EditValue == null) return;

                if (toggleIsActive.IsOn == true)
                {
                    XtraMessageBox.Show($"BOM đã được active.Không thể xóa đợt BOM này!!!"
                         , Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa Đợt này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {


                    string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                    string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                    string dot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();


                    string urlGET = $"{URL}KhoiTaoBOMV1/Get?action=GETCAPPHATDELETEBOM&para1={dot.ToString()}";
                    string jsonGET = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGET); }).Result;
                    if (jsonGET != "[]")
                    {
                        DataTable tblGet = JsonConvert.DeserializeObject<DataTable>(jsonGET);
                        if (tblGet != null && tblGet.Rows.Count != 0)
                        {

                            XtraMessageBox.Show($"BOM này đã được cấp phát đến lệnh {tblGet.Rows[0]["MaLenh"].ToString()}.Không thể xóa đợt BOM này!!!"
                           , Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                            return;

                        }

                    }

                    //string url = $"{URL}PhanTichBom/DeleteDotBOM?makh={searchLookUpEditKH.EditValue.ToString()}&mahang={searchLookUpEditMH.EditValue.ToString()}&madot={searchLookUpEditDot.EditValue.ToString()}&userid={GlobleData.UserName}";
                    string url = $"{URL}KhoiTaoBOMV1/Delete?action=DELETEDOTBOMV1&para1={kh.ToString()}&para2={mh.ToString()}&para3={dot}&para4={GlobleData.UserName}";
                    string json = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;

                    if (json.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this);
                        string title = "BOM NPL đã bị xóa";
                        SendNotify(title, "PKT", "ALL", 1);
                        SendNotify(title, "PKH", "PKH");
                        CreateSearchLookUpDot();
                        LoadData();
                        LoadChiTiet();


                    }

                }
            }
            catch (Exception ex) { }
        }


        private void LoadSizeDM()
        {
            try
            {
                _dtSize = new DataTable();
                string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string url = $"{URL}KhoiTaoBOMV1/Get?action=GETSIZEDM&para1={kh.ToString()}&para2={mh.ToString()}&para7={0}";
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
                string url = $"{URL}KhoiTaoBOMV1/Get?action=GETSIZEDM&para1={kh.ToString()}&para2={mh.ToString()}&para7={0}";
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
                    IsXetDuyet = row["IsXetDuyet"]?.ToString().ToLower() == "đã xác nhận";
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







        private void bandedGridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //if (!isAdd) return;
            DataRow drF = bandedGridView1.GetFocusedDataRow();

            if (e.Column.FieldName == "MaNhomChiTiet")
            {



                foreach (DataRow row in _dtSize.Rows)
                {
                    if (row["MaVTID"].ToString() == drF["MaVTID"].ToString() && row["MauVTID"].ToString() == drF["MauVTID"].ToString() && row["KhoVaiID"].ToString() == drF["KhoVaiID"].ToString() && row["MaNhom"].ToString() == drF["MaNhom"].ToString())
                    {
                        row["MaNhomChiTiet"] = drF["MaNhomChiTiet"];

                    }

                }


            }





        }
        //Lưu tổng

        private async void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang xử lý yêu cầu");
                SplashScreenManager.Default.SetWaitFormDescription("Đang chuẩn bị dữ liệu...");

                // Lấy thông tin và validate
                _rowhandle = bandedGridView1.FocusedRowHandle;
                this.ActiveControl = button1;

                string madotpost = isAdd ? _madot : (searchLookUpEditDot.EditValue?.ToString() ?? "");
                string tendotpost = isAdd ? textBox1.Text : searchLookUpEditDot.Text;

                var tblGC1 = gridControl1.DataSource as DataTable;

                if (string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue?.ToString()) ||
                    string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue?.ToString()) ||
                    tblGC1 == null || tblGC1.Rows.Count == 0)
                {
                    SplashScreenManager.CloseForm();
                    return;
                }

                string _makh = searchLookUpEditKH.EditValue.ToString();
                string _mahang = searchLookUpEditMH.EditValue.ToString();

                // Lấy danh sách màu và cột màu một lần
                var colorColumns = tblGC1.Columns.Cast<DataColumn>()
                    .Where(c => c.ColumnName.Contains("@Mau@"))
                    .ToList();

                List<string> listMaMau = colorColumns
                    .Select(c => c.ColumnName.Split('@')[2].Trim())
                    .Distinct()
                    .ToList();

                // Thực thi logic nặng trên thread nền
                bool success = await Task.Run(async () =>
                {
                    // Tạo DataTable MauSP
                    DataTable dtMauSP = createTypeMauSP();
                    dtMauSP.Clear();
                    foreach (DataRow _dr in tblGC1.Rows)
                    {
                        bool hasMau = false;
                        foreach (var _cl in colorColumns)
                        {
                            var cellValue = _dr[_cl.ColumnName]?.ToString();
                            if (!string.IsNullOrWhiteSpace(cellValue))
                            {
                                hasMau = true;
                                string mamau = _cl.ColumnName.Split('@')[2].Trim();
                                DataRow _nr = dtMauSP.NewRow();
                                _nr["ID"] = 0;
                                _nr["MaKH"] = _makh;
                                _nr["MaHang"] = _mahang;
                                _nr["MaNhom"] = _dr["MaNhom"];
                                _nr["MaVTID"] = _dr["MaVTID"];
                                _nr["MauVTID"] = cellValue;
                                _nr["KhoVaiID"] = _dr["KhoVaiID"];
                                _nr["MaMau"] = mamau;
                                _nr["MaDot"] = madotpost;
                                _nr["MaCode"] = _dr["MaCode"];
                                _nr["IsActive"] = _dr["IsActive"];
                                _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
                                dtMauSP.Rows.Add(_nr);
                            }
                        }

                        if (!hasMau)
                        {
                            foreach (string mamau in listMaMau)
                            {
                                DataRow _nr = dtMauSP.NewRow();
                                _nr["ID"] = 0;
                                _nr["MaKH"] = _makh;
                                _nr["MaHang"] = _mahang;
                                _nr["MaNhom"] = _dr["MaNhom"];
                                _nr["MaVTID"] = _dr["MaVTID"];
                                _nr["MauVTID"] = "MAUVT_0";
                                _nr["KhoVaiID"] = _dr["KhoVaiID"];
                                _nr["MaMau"] = mamau;
                                _nr["MaDot"] = madotpost;
                                _nr["MaCode"] = _dr["MaCode"];
                                _nr["IsActive"] = _dr["IsActive"];
                                _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
                                dtMauSP.Rows.Add(_nr);
                            }
                        }
                    }

                    // Tạo DataTable SizeSP
                    DataTable dtSizeSP = createTypeSizeSP();
                    dtSizeSP.Clear();
                    foreach (DataRow _dr in tblGC1.Rows)
                    {
                        string maSizeChung = Convert.ToString(_dr["MaSizeChung"]);
                        Dictionary<string, string[]> sizeMap;

                        if (string.IsNullOrEmpty(maSizeChung))
                        {
                            sizeMap = tblSizeChung.AsEnumerable()
                                .GroupBy(row => row.Field<string>("MaNhomSize"))
                                .ToDictionary(
                                    g => g.Key,
                                    g => g.Select(row => row.Field<string>("MaSize")).Distinct().ToArray()
                                );
                        }
                        else
                        {
                            sizeMap = maSizeChung.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(item => item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries))
                                .Where(parts => parts.Length == 2)
                                .ToDictionary(
                                    parts => parts[0].Trim(),
                                    parts => parts[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Trim())
                                        .ToArray()
                                );
                        }

                        bool hasMau = false;
                        foreach (var _cl in colorColumns)
                        {
                            var cellValue = _dr[_cl.ColumnName]?.ToString();
                            if (!string.IsNullOrWhiteSpace(cellValue))
                            {
                                hasMau = true;
                                string mamau = _cl.ColumnName.Split('@')[2].Trim();

                                foreach (var kv in sizeMap)
                                {
                                    foreach (string size in kv.Value)
                                    {
                                        DataRow _nr = dtSizeSP.NewRow();
                                        _nr["MaKH"] = _makh;
                                        _nr["MaHang"] = _mahang;
                                        _nr["MaNhom"] = _dr["MaNhom"];
                                        _nr["MaVTID"] = _dr["MaVTID"];
                                        _nr["MauVTID"] = cellValue;
                                        _nr["KhoVaiID"] = _dr["KhoVaiID"];
                                        _nr["MaNhomSize"] = kv.Key;
                                        _nr["MaSize"] = size;
                                        _nr["DinhMuc"] = _dr["DinhMucChung"];
                                        _nr["MaDot"] = madotpost;
                                        _nr["Dot"] = tendotpost;
                                        _nr["NguoiTao"] = GlobleData.UserName;
                                        _nr["MaMau"] = mamau;
                                        _nr["TachMau"] = _dr["TachMau"];
                                        _nr["MaCode"] = _dr["MaCode"];
                                        _nr["IsActive"] = _dr["IsActive"];
                                        _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
                                        dtSizeSP.Rows.Add(_nr);
                                    }
                                }
                            }
                        }

                        if (!hasMau)
                        {
                            foreach (string mamau in listMaMau)
                            {
                                foreach (var kv in sizeMap)
                                {
                                    foreach (string size in kv.Value)
                                    {
                                        DataRow _nr = dtSizeSP.NewRow();
                                        _nr["MaKH"] = _makh;
                                        _nr["MaHang"] = _mahang;
                                        _nr["MaNhom"] = _dr["MaNhom"];
                                        _nr["MaVTID"] = _dr["MaVTID"];
                                        _nr["MauVTID"] = "MAUVT_0";
                                        _nr["KhoVaiID"] = _dr["KhoVaiID"];
                                        _nr["MaNhomSize"] = kv.Key;
                                        _nr["MaSize"] = size;
                                        _nr["DinhMuc"] = _dr["DinhMucChung"];
                                        _nr["MaDot"] = madotpost;
                                        _nr["Dot"] = tendotpost;
                                        _nr["NguoiTao"] = GlobleData.UserName;
                                        _nr["MaMau"] = mamau;
                                        _nr["TachMau"] = _dr["TachMau"];
                                        _nr["MaCode"] = _dr["MaCode"];
                                        _nr["IsActive"] = _dr["IsActive"];
                                        _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
                                        dtSizeSP.Rows.Add(_nr);
                                    }
                                }
                            }
                        }
                    }

                    // Tạo DataTable VatTuSP
                    DataTable dtVatTuSP = createTypeVattuSP();
                    dtVatTuSP.Clear();
                    foreach (DataRow _dr in tblGC1.Rows)
                    {
                        bool hasMau = false;
                        foreach (var _cl in colorColumns)
                        {
                            var cellValue = _dr[_cl.ColumnName]?.ToString();
                            if (!string.IsNullOrWhiteSpace(cellValue))
                            {
                                hasMau = true;
                                DataRow _nr = dtVatTuSP.NewRow();
                                _nr["ID"] = 0;
                                _nr["MaKH"] = _makh;
                                _nr["MaHang"] = _mahang;
                                _nr["MaNhom"] = _dr["MaNhom"];
                                _nr["MaVTID"] = _dr["MaVTID"];
                                _nr["MauVTID"] = cellValue;
                                _nr["KhoVaiID"] = _dr["KhoVaiID"];
                                _nr["MaDot"] = madotpost;
                                _nr["Dot"] = tendotpost;
                                _nr["NguoiTao"] = GlobleData.UserName;
                                _nr["DinhMucChung"] = _dr["DinhMucChung"];
                                _nr["DinhMucChiTiet"] = _dr["DinhMucHaoHut"];
                                _nr["TachMau"] = _dr["TachMau"];
                                _nr["STT"] = _dr["STT"];
                                _nr["MaCode"] = _dr["MaCode"];
                                _nr["STTCode"] = _dr["STTCode"];
                                _nr["IsActive"] = _dr["IsActive"];
                                _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
                                _nr["MaVTGhep"] = _dr["GhiChu"];
                                _nr["MaDVCL"] = _dr["MaDVCL"];
                                _nr["MaDVDM"] = _dr["MaDVDM"];
                                _nr["IsPS"] = _dr["IsPS"];

                                if (string.IsNullOrEmpty(_dr["MaSizeChung"].ToString()))
                                    _nr["IsAllSize"] = true;
                                else
                                    _nr["IsAllSize"] = false;


                                if (string.IsNullOrEmpty(_dr["Size"].ToString()) || _dr["Size"].ToString() == "All Size")
                                {
                                    _nr["IsAllSize"] = true;
                                }
                                else
                                {
                                    _nr["IsAllSize"] = false;
                                }

                                _nr["DinhMucKH"] = _dr["DinhMucKH"];
                                dtVatTuSP.Rows.Add(_nr);
                            }
                        }

                        if (!hasMau)
                        {
                            DataRow _nr = dtVatTuSP.NewRow();
                            _nr["ID"] = 0;
                            _nr["MaKH"] = _makh;
                            _nr["MaHang"] = _mahang;
                            _nr["MaNhom"] = _dr["MaNhom"];
                            _nr["MaVTID"] = _dr["MaVTID"];
                            _nr["MauVTID"] = "MAUVT_0";
                            _nr["KhoVaiID"] = _dr["KhoVaiID"];
                            _nr["MaDot"] = madotpost;
                            _nr["Dot"] = tendotpost;
                            _nr["NguoiTao"] = GlobleData.UserName;
                            _nr["DinhMucChung"] = _dr["DinhMucChung"];
                            _nr["DinhMucChiTiet"] = _dr["DinhMucHaoHut"];
                            _nr["TachMau"] = false;
                            _nr["STT"] = _dr["STT"];
                            _nr["MaCode"] = _dr["MaCode"];
                            _nr["STTCode"] = _dr["STTCode"];
                            _nr["IsActive"] = _dr["IsActive"];
                            _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
                            _nr["MaVTGhep"] = _dr["GhiChu"];
                            _nr["MaDVCL"] = _dr["MaDVCL"];
                            _nr["MaDVDM"] = _dr["MaDVDM"];
                            _nr["IsPS"] = _dr["IsPS"];
                            if (string.IsNullOrEmpty(_dr["MaSizeChung"].ToString()))
                                _nr["IsAllSize"] = true;
                            else
                                _nr["IsAllSize"] = false;



                            if (string.IsNullOrEmpty(_dr["Size"].ToString()) || _dr["Size"].ToString() == "All Size")
                            {
                                _nr["IsAllSize"] = true;
                            }
                            else
                            {
                                _nr["IsAllSize"] = false;
                            }

                            _nr["DinhMucKH"] = _dr["DinhMucKH"];
                            dtVatTuSP.Rows.Add(_nr);

                        }
                    }

                    //SplashScreenManager.Default.SetWaitFormDescription("Đang lưu màu sản phẩm... (40%)");
                    string url1 = $"{URL}KhoiTaoBOMV1/Post1?Action=POSTMAUSP&para1={GlobleData.UserName}";
                    string result1 = await _clientExtension.PostAsync(url1, dtMauSP);
                    if (result1 != "True") return false;
                    //SplashScreenManager.Default.SetWaitFormDescription("Đang lưu định mức size... (70%)");
                    string url2 = $"{URL}KhoiTaoBOMV1/Post2?Action=POSTDMSIZE&para1={GlobleData.UserName}";
                    string result2 = await _clientExtension.PostAsync(url2, dtSizeSP);
                    if (result2 != "True") return false;
                    //SplashScreenManager.Default.SetWaitFormDescription("Đang lưu vật tư... (90%)");

                    string url3 = $"{URL}KhoiTaoBOMV1/Post3?Action=POSTVTSP&para1={GlobleData.UserName}&para2={madotpost}&para3={_isSua}";
                    string result3 = await _clientExtension.PostAsync(url3, dtVatTuSP);
                    if (result3 != "True") return false;
                    //SplashScreenManager.Default.SetWaitFormDescription("Hoàn tất... (100%)");
                    return true;
                });

                SplashScreenManager.CloseForm(false);

                if (success)
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    NapLai(!isAdd);
                    bandedGridView1.FocusedRowHandle = _rowhandle;
                    btnCopyBOM.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);
                //MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
        }
        private DataTable createTypeMauSP()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("MaNhom", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("MaDot", typeof(string));
            dt.Columns.Add("MaCode", typeof(string));
            dt.Columns.Add("MaNhomChiTiet", typeof(string));
            dt.Columns.Add("IsActive", typeof(bool));
            return dt;
        }

        private DataTable createTypeSizeSP()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("MaHang", typeof(string));
            dt.Columns.Add("MaNhom", typeof(string));
            dt.Columns.Add("MaVTID", typeof(string));
            dt.Columns.Add("MauVTID", typeof(string));
            dt.Columns.Add("KhoVaiID", typeof(string));
            dt.Columns.Add("MaNhomSize", typeof(string));
            dt.Columns.Add("MaSize", typeof(string));
            dt.Columns.Add("DinhMuc", typeof(double));
            dt.Columns.Add("MaDot", typeof(string));
            dt.Columns.Add("Dot", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("TachMau", typeof(bool));
            dt.Columns.Add("MaCode", typeof(string));
            dt.Columns.Add("MaNhomChiTiet", typeof(string));
            dt.Columns.Add("IsActive", typeof(bool));
            return dt;
        }

        private DataTable createTypeVattuSP()
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
            dt.Columns.Add("Dot", typeof(string));
            dt.Columns.Add("NguoiTao", typeof(string));
            dt.Columns.Add("NgayTao", typeof(DateTime));
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("DinhMucChung", typeof(double));
            dt.Columns.Add("DinhMucChiTiet", typeof(double));
            dt.Columns.Add("TachMau", typeof(bool));
            dt.Columns.Add("MaVTGhep", typeof(string));
            dt.Columns.Add("MaCode", typeof(string));
            dt.Columns.Add("STTCode", typeof(int));
            dt.Columns.Add("MaNhomChiTiet", typeof(string));
            dt.Columns.Add("IsActive", typeof(bool));
            dt.Columns.Add("IsAllSize", typeof(bool));
            dt.Columns.Add("DinhMucKH", typeof(double));
            dt.Columns.Add("MaDVCL", typeof(string));
            dt.Columns.Add("MaDVDM", typeof(string));
            dt.Columns.Add("IsPS", typeof(int));
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
                                IsXetDuyet = row_focused["IsXetDuyet"]?.ToString().ToLower() == "đã xác nhận";
                            }
                            bool IsCopyMauDM = false;
                            if (!IsXetDuyet)
                            {

                                DevExpress.Utils.Menu.DXMenuItem menuThemDongItem = new DevExpress.Utils.Menu.DXMenuItem("Thêm dòng", ThemDong);
                                e.Menu.Items.Add(menuThemDongItem);

                                DevExpress.Utils.Menu.DXMenuItem menuCopyCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy dòng", CopyDong);
                                e.Menu.Items.Add(menuCopyCopyItem);

                                DevExpress.Utils.Menu.DXMenuItem menuCopyNhieuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy nhiều dòng", CopyNhieuDong);
                                e.Menu.Items.Add(menuCopyNhieuCopyItem);

                                DevExpress.Utils.Menu.DXMenuItem menuCopyPasteCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Paste nhiều dòng", PasteNhieuDong);
                                e.Menu.Items.Add(menuCopyPasteCopyItem);
                                DevExpress.Utils.Menu.DXMenuItem menuCopyPasteExcelCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Paste Excel Itemcode", PasteExcel);
                                e.Menu.Items.Add(menuCopyPasteExcelCopyItem);

                                DevExpress.Utils.Menu.DXMenuItem menuCopyPasteExcelCopyKhoSize = new DevExpress.Utils.Menu.DXMenuItem("Paste Khổ/Size", PasteKhoSizeExcel);
                                e.Menu.Items.Add(menuCopyPasteExcelCopyKhoSize);

                                DevExpress.Utils.Menu.DXMenuItem menuCopyPasteDMExcelCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Paste Excel", PasteExcelDinhMuc);
                                e.Menu.Items.Add(menuCopyPasteDMExcelCopyItem);
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
        private void CopyNhieuDong(object sender, EventArgs e)
        {

            GridView view = bandedGridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                CopySelectedRows();
            }
        }
        private void CopySelectedRows()
        {
            try
            {
                int[] selectedRows = bandedGridView1.GetSelectedRows();
                if (selectedRows.Length == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn dòng cần copy!", "Thông báo");
                    return;
                }
                copiedData = (bandedGridView1.GridControl.DataSource as DataTable).Clone();

                foreach (int rowHandle in selectedRows)
                {
                    if (rowHandle >= 0)
                    {
                        DataRow sourceRow = bandedGridView1.GetDataRow(rowHandle);
                        DataRow newRow = copiedData.NewRow();

                        foreach (DataColumn col in (bandedGridView1.GridControl.DataSource as DataTable).Columns)
                        {
                            newRow[col.ColumnName] = sourceRow[col.ColumnName];
                        }
                        copiedData.Rows.Add(newRow);
                    }
                }


            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi copy: {ex.Message}", "Lỗi");
            }
        }
        private void PasteNhieuDong(object sender, EventArgs e)
        {

            GridView view = bandedGridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                PasteCopiedRows();
            }
        }

        private void PasteExcel(object sender, EventArgs e)
        {

            GridView view = bandedGridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                //bandedGridView1.BeforeLeaveRow -= bandedGridView1_BeforeLeaveRow;
                PasteDataExcel(view);
                //bandedGridView1.BeforeLeaveRow += bandedGridView1_BeforeLeaveRow;
            }
        }
        private void PasteKhoSizeExcel(object sender, EventArgs e)
        {

            GridView view = bandedGridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {

                PasteDataKhoVaiExcel(view);

            }
        }
        private void PasteDataKhoVaiExcel(GridView view)
        {
            try
            {
                string clipboardData = Clipboard.GetText();
                byte[] bytes = Encoding.UTF8.GetBytes(clipboardData);
                string decodedClipboardData = Encoding.UTF8.GetString(bytes);
                string[] data = decodedClipboardData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 0) return;

                int startRow = view.FocusedRowHandle;
                DataTable table = gridControl1.DataSource as DataTable;
                //_tblKhoSize






                view.CloseEditor();
                for (int i = 0; i < data.Length; i++)
                {
                    int currentRow = startRow + i;
                    DataRow foundRow = _tblKhoSize.AsEnumerable()
                        .FirstOrDefault(row => row["KhoVai"].ToString().Trim() == data[i].Trim());

                    if (foundRow != null)
                    {
                        table.Rows[currentRow]["KhoVaiID"] = foundRow["KhoVaiID"];
                    }
                    else
                    {

                        MessageBox.Show($"Không tìm thấy khổ vải: {data[i]}", "Cảnh báo");

                    }
                }

                view.RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }
        private void PasteDataExcel(GridView view)
        {
            try
            {
                string clipboardData = Clipboard.GetText();
                byte[] bytes = Encoding.UTF8.GetBytes(clipboardData);
                string decodedClipboardData = Encoding.UTF8.GetString(bytes);
                string[] data = decodedClipboardData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 0) return;

                int startRow = view.FocusedRowHandle;
                DataTable table = gridControl1.DataSource as DataTable;
                DataTable tblItemCodeGoc = tblItemCode;

                // LẤY VỊ TRÍ CHÍNH XÁC TRONG DATATABLE
                int insertIndex = table.Rows.Count;

                if (view.IsValidRowHandle(startRow))
                {
                    DataRow startDataRow = view.GetDataRow(startRow);
                    if (startDataRow != null)
                    {
                        insertIndex = table.Rows.IndexOf(startDataRow);
                    }
                }
                else
                {
                    var lastValidRow = table.AsEnumerable()
                        .Where(r => !string.IsNullOrWhiteSpace(r["MaVT"].ToString()))
                        .OrderByDescending(r => table.Rows.IndexOf(r))
                        .FirstOrDefault();

                    if (lastValidRow != null)
                    {
                        insertIndex = table.Rows.IndexOf(lastValidRow) + 1;
                    }
                    else
                    {
                        insertIndex = 0;
                    }
                }

                // ===== SỬA CHÍNH Ở ĐÂY =====
                // ĐẾM SỐ DÒNG TRỐNG CÓ SẴN TỪ VỊ TRÍ PASTE
                int existingEmptyRows = 0;
                for (int i = insertIndex; i < table.Rows.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(table.Rows[i]["MaVT"].ToString()))
                    {
                        existingEmptyRows++;
                    }
                    else
                    {
                        break; // Gặp dòng có dữ liệu thì dừng
                    }
                }

                // TÍNH SỐ DÒNG CẦN THÊM MỚI
                int rowsToAdd = data.Length - existingEmptyRows;
                if (rowsToAdd < 0) rowsToAdd = 0;

                // BƯỚC 1: Thêm các dòng mới nếu cần
                List<bool> nplList = new List<bool>();

                foreach (string rowData in data)
                {
                    DataRow foundRow = tblItemCodeGoc.AsEnumerable()
                        .FirstOrDefault(r => r["MaVT"].ToString().Trim() == rowData.Trim());

                    bool isNL = false;
                    if (foundRow != null)
                    {
                        isNL = foundRow["NPL"].ToString().ToUpper() == "TRUE";
                    }
                    nplList.Add(isNL);
                }

                // Thêm các dòng mới (chỉ thêm số lượng cần thiết)
                for (int i = 0; i < rowsToAdd; i++)
                {
                    // Lấy NPL từ dòng tương ứng (bắt đầu từ dòng sau các dòng trống có sẵn)
                    bool isNL = nplList[existingEmptyRows + i];

                    DataRow newRow = table.NewRow();
                    newRow["DinhMucHaoHut"] = 0;
                    newRow["DinhMucChung"] = 1;
                    newRow["DinhMucKH"] = 1;
                    newRow["STT"] = 0;
                    newRow["NPL"] = isNL;
                    newRow["MaDVCL"] = "DVVT_7";

                    newRow["MaDVDM"] = "DVVT_7";
                    if (isNL)
                        _sttVT++;
                    else
                        _sttVTPL++;

                    // Chèn vào cuối vùng paste
                    table.Rows.InsertAt(newRow, insertIndex + existingEmptyRows + i);
                }

                // REFRESH LƯỚI SAU KHI THÊM XONG
                view.RefreshData();
                gridControl1.Refresh();

                // BƯỚC 2: Điền dữ liệu vào các dòng (cả dòng cũ và mới)
                for (int i = 0; i < data.Length; i++)
                {
                    string rowData = data[i];
                    DataRow dr = table.Rows[insertIndex + i];

                    if (dr == null) continue;

                    DataRow foundRow = tblItemCodeGoc.AsEnumerable()
                        .FirstOrDefault(r => r["MaVT"].ToString().Trim() == rowData.Trim());

                    if (foundRow != null)
                    {
                        // Gán dữ liệu
                        dr["MaNhom"] = foundRow["MaNhom"].ToString();
                        dr["TenNhom"] = foundRow["TenNhom"].ToString();
                        dr["NPL"] = foundRow["NPL"].ToString();
                        dr["Sort"] = foundRow["Sort"].ToString();
                        dr["MaVTID"] = foundRow["MaVTID"].ToString();
                        dr["MaVT"] = foundRow["MaVT"].ToString();
                        dr["ChiTiet"] = foundRow["VatTu"].ToString();
                        dr["MaDVVT"] = foundRow["MaDVVT"].ToString();
                        dr["TenDVVT"] = foundRow["TenDVVT"].ToString();
                        dr["MaDVCL"] = "DVVT_7";

                        dr["MaDVDM"] = "DVVT_7";
                        bool isNL = dr["NPL"].ToString().ToUpper() == "TRUE" ? true : false;

                        string random6 = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
                        dr["MaCode"] = foundRow["MaVTID"].ToString()
                                         + "|" + random6
                                         + "|" + dr["STT"].ToString();

                        // Xử lý chung loại chi tiết tự chọn
                        string maNhomChiTiet = "";

                        if (isNL)
                        {
                            string filterChungLoai = string.Format("MaNhom = '{0}'", foundRow["MaNhom"]);
                            DataRow[] foundRows = tblChungLoaiChiTiet.Select(filterChungLoai);
                            if (foundRows.Length == 1)
                            {
                                maNhomChiTiet = foundRows[0]["MaNhomChiTiet"].ToString();
                            }
                        }
                        else
                        {
                            string filterChungLoai = string.Format("TenNhomChiTiet = '{0}'", foundRow["TenNhom"].ToString().Replace("'", "''"));
                            DataRow[] foundRows = tblChungLoaiChiTiet.Select(filterChungLoai);
                            if (foundRows.Length > 0)
                            {
                                maNhomChiTiet = foundRows[0]["MaNhomChiTiet"].ToString();
                            }
                        }

                        dr["MaNhomChiTiet"] = maNhomChiTiet;

                        // Xử lý màu vật tư
                        dr["MauVTIDChung"] = foundRow["MauVTIDChung"].ToString();
                        string[] arrmauvtid = foundRow["MauVTIDChung"].ToString().Split(',');
                        if (arrmauvtid.Length == 1)
                        {
                            foreach (DataColumn dc in dr.Table.Columns)
                            {
                                if (dc.ColumnName.Contains("@Mau@"))
                                {
                                    dr[dc.ColumnName] = arrmauvtid[0];
                                }
                            }
                        }
                        else
                        {
                            foreach (DataColumn dc in dr.Table.Columns)
                            {
                                if (dc.ColumnName.Contains("@Mau@"))
                                {
                                    dr[dc.ColumnName] = "";
                                }
                            }
                        }

                        // Xử lý khổ vải ID
                        string[] khovaiidarr = foundRow["KhoVaiIDChung"].ToString().Split(',');
                        if (khovaiidarr.Length == 1 || khovaiidarr.All(x => x == khovaiidarr[0]))
                        {
                            dr["KhoVaiID"] = khovaiidarr[0];
                        }
                        else
                        {
                            dr["KhoVaiID"] = "";
                        }
                    }
                }

                // BƯỚC 3: Cập nhật lại STT cho toàn bộ table
                RecalculateSTT(table);

                view.CloseEditor();
                view.RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        // HÀM MỚI: Thêm dòng liên tiếp tại vị trí chỉ định
        private void AddNewRowAt_Sequential(DataTable table, int position, bool isNL)
        {
            DataRow newRow = table.NewRow();
            newRow["DinhMucHaoHut"] = 0;
            newRow["DinhMucChung"] = 1;
            newRow["STT"] = 0; // Sẽ tính lại sau
            newRow["NPL"] = isNL;

            if (isNL)
                _sttVT++;
            else
                _sttVTPL++;

            // Chèn tại vị trí chính xác
            table.Rows.InsertAt(newRow, position);
        }

        // Hàm tính lại STT cho toàn bộ bảng
        private void RecalculateSTT(DataTable table)
        {
            // Tính lại STT cho NPL = True
            bool hasEmptySTT = table.AsEnumerable()
       .Any(r => string.IsNullOrEmpty(r["STT"].ToString()) || r["STT"].ToString() == "0");

            if (!hasEmptySTT)
                return;

            bandedGridView1.BeforeLeaveRow -= bandedGridView1_BeforeLeaveRow;
            var nlRows = table.AsEnumerable()
                .Where(r => r["NPL"].ToString() == "True")
                .OrderBy(r => table.Rows.IndexOf(r))
                .ToList();

            int sttNL = 1;
            foreach (DataRow row in nlRows)
            {
                row["STT"] = sttNL++;
            }

            // Tính lại STT cho NPL = False
            var plRows = table.AsEnumerable()
                .Where(r => r["NPL"].ToString() != "True")
                .OrderBy(r => table.Rows.IndexOf(r))
                .ToList();

            int sttPL = 1;
            foreach (DataRow row in plRows)
            {
                row["STT"] = sttPL++;
            }
            bandedGridView1.BeforeLeaveRow += bandedGridView1_BeforeLeaveRow;
        }
        private void PasteExcelDinhMuc(object sender, EventArgs e)
        {

            GridView view = bandedGridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                PasteDataExcelDinhMuc(view);
            }
        }
        private void PasteDataExcelDinhMuc(GridView view)
        {
            try
            {
                if (view.FocusedColumn == null) return;
                GridColumn focusedColumn = view.FocusedColumn;

                string clipboardData = Clipboard.GetText();
                byte[] bytes = Encoding.UTF8.GetBytes(clipboardData);
                string decodedClipboardData = Encoding.UTF8.GetString(bytes);
                string[] data = decodedClipboardData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                if (data.Length == 0) return;
                //string checkDataRow = "Số Roll\tLOT\tBatch\tSL theo CT\tSL thực tế\tKhối lượng\tTrọng lượng\tPallet\tGhi chú\tBarCode";
                //if (checkDataRow.Contains(data[0]))
                //{
                //    data = data.Skip(1).ToArray();
                //}
                int startRow = view.FocusedRowHandle;

                foreach (string row in data)
                {
                    if (!view.IsValidRowHandle(startRow)) break;


                    DataRow dr = bandedGridView1.GetDataRow(startRow);
                    if (dr == null) return;
                    if (focusedColumn.FieldName == "DinhMucChung")
                    {
                        dr["DinhMucChung"] = row;
                    }
                    else if (focusedColumn.FieldName == "DinhMucKH")
                    {
                        dr["DinhMucKH"] = row;
                        dr["DinhMucChung"] = row;
                    }
                    else if (focusedColumn.FieldName == "GhiChu")
                    {
                        dr["GhiChu"] = row;
                    }
                    else if (focusedColumn.FieldName == "Size")
                    {
                        string result = "";
                        string filteredRow = "";
                        string[] inseamGroups = row.Split(';');

                        foreach (string inseamGroup in inseamGroups)
                        {
                            if (string.IsNullOrEmpty(inseamGroup.Trim())) continue;

                            string[] parts = inseamGroup.Split(':');
                            if (parts.Length != 2) continue;

                            string inseam = parts[0].Trim(); // Inseam1, Inseam2...
                            string[] sizes = parts[1].Split(',');

                            // Tìm MaNhomSize từ TenSize = inseam
                            DataRow[] nhomSizeRows = tblSizeChung.Select($"NhomSize = '{inseam}'");
                            if (nhomSizeRows.Length == 0) continue;

                            string maNhomSize = nhomSizeRows[0]["NhomSize"].ToString();
                            string validSizes = "";
                            string validSizeNames = ""; // Lưu tên size hợp lệ

                            foreach (string size in sizes)
                            {
                                string trimmedSize = size.Trim();
                                // Kiểm tra size có tồn tại trong tblSizeChung với NhomSize tương ứng
                                DataRow[] sizeRows = tblSizeChung.Select($"NhomSize = '{maNhomSize}' AND TenSize = '{trimmedSize}'");

                                if (sizeRows.Length > 0)
                                {
                                    string maSize = sizeRows[0]["MaSize"].ToString();

                                    // Thêm vào validSizes (MaSize)
                                    if (!string.IsNullOrEmpty(validSizes))
                                        validSizes += ",";
                                    validSizes += maSize;

                                    // Thêm vào validSizeNames (TenSize)
                                    if (!string.IsNullOrEmpty(validSizeNames))
                                        validSizeNames += ",";
                                    validSizeNames += trimmedSize;
                                }
                            }

                            // Chỉ thêm vào result và filteredRow nếu có size hợp lệ
                            if (!string.IsNullOrEmpty(validSizes))
                            {
                                // Xử lý result
                                if (!string.IsNullOrEmpty(result))
                                    result += ";";
                                result += $"{maNhomSize}:{validSizes}";

                                // Xử lý filteredRow (giữ lại định dạng gốc nhưng chỉ với size hợp lệ)
                                if (!string.IsNullOrEmpty(filteredRow))
                                    filteredRow += ";";
                                filteredRow += $"{inseam}:{validSizeNames}";
                            }
                        }

                        dr["MaSizeChung"] = result;
                        dr["Size"] = filteredRow;
                    }



                    startRow++;
                }
                this.ActiveControl = button1;
                view.CloseEditor();

            }
            catch (Exception ex)
            {
            }
        }
        //private void PasteCopiedRows()
        //{
        //    int successCount = 0;
        //    int errorCount = 0;

        //    try
        //    {
        //        if (copiedData == null || copiedData.Rows.Count == 0)
        //        {
        //            return;
        //        }

        //        int currentRowHandle = bandedGridView1.FocusedRowHandle;

        //        // KIỂM TRA GROUP ROW
        //        if (bandedGridView1.IsGroupRow(currentRowHandle))
        //        {
        //            XtraMessageBox.Show("Vui lòng chọn một dòng dữ liệu, không phải dòng nhóm!", "Thông báo");
        //            return;
        //        }

        //        if (currentRowHandle < 0)
        //        {
        //            XtraMessageBox.Show("Vui lòng chọn vị trí để paste!", "Thông báo");
        //            return;
        //        }

        //        DataTable dt = bandedGridView1.GridControl.DataSource as DataTable;
        //        if (dt == null)
        //        {
        //            XtraMessageBox.Show("DataSource bị null!", "Lỗi");
        //            return;
        //        }

        //        bandedGridView1.BeginUpdate();

        //        try
        //        {
        //            int currentHandle = currentRowHandle;
        //            int copiedRowCount = 0;

        //            while (copiedRowCount < copiedData.Rows.Count)
        //            {
        //                // BỎ QUA GROUP ROWS
        //                while (currentHandle < bandedGridView1.RowCount &&
        //                       bandedGridView1.IsGroupRow(currentHandle))
        //                {
        //                    currentHandle++;
        //                }

        //                // Nếu hết dòng visible, thoát
        //                if (currentHandle >= bandedGridView1.RowCount)
        //                {
        //                    // Có thể thêm dòng mới hoặc dừng lại
        //                    break;
        //                }

        //                try
        //                {
        //                    DataRow copiedRow = copiedData.Rows[copiedRowCount];
        //                    DataRow targetRow = bandedGridView1.GetDataRow(currentHandle);

        //                    if (targetRow != null)
        //                    {
        //                        foreach (DataColumn col in dt.Columns)
        //                        {
        //                            // Bỏ qua cột STT
        //                            if (col.ColumnName.ToUpper() == "STT")
        //                            {
        //                                continue;
        //                            }
        //                            else if(col.ColumnName.ToString() == "MaCode")
        //                            {
        //                                string random6 = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

        //                                targetRow["MaCode"] = copiedRow["MaVTID"].ToString()
        //                                                 + "|" + random6
        //                                                 + "|" + targetRow["STT"].ToString();
        //                                continue;

        //                            }
        //                            else if(col.ColumnName.Contains("@Mau@"))
        //                            {
        //                                string[] arrmauvtid = copiedRow["MauVTIDChung"].ToString().Split(',');
        //                                if (arrmauvtid.Length == 1)
        //                                {
        //                                    foreach (DataColumn dc in targetRow.Table.Columns)
        //                                    {
        //                                        if (dc.ColumnName.Contains("@Mau@"))
        //                                        {
        //                                            targetRow[dc.ColumnName] = arrmauvtid[0];
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    foreach (DataColumn dc in targetRow.Table.Columns)
        //                                    {
        //                                        if (dc.ColumnName.Contains("@Mau@"))
        //                                        {
        //                                            targetRow[dc.ColumnName] = "";
        //                                        }
        //                                    }

        //                                }
        //                            }    
        //                            else if (copiedData.Columns.Contains(col.ColumnName))
        //                            {
        //                                targetRow[col.ColumnName] = copiedRow[col.ColumnName];
        //                            }

        //                        }

        //                        successCount++;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    errorCount++;
        //                }

        //                copiedRowCount++;
        //                currentHandle++;
        //            }
        //        }
        //        finally
        //        {
        //            bandedGridView1.EndUpdate();
        //            bandedGridView1.RefreshData();
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show($"Lỗi khi paste: {ex.Message}", "Lỗi");
        //    }
        //}
        private void PasteCopiedRows()
        {
            int successCount = 0;
            int errorCount = 0;

            try
            {
                if (copiedData == null || copiedData.Rows.Count == 0)
                {
                    return;
                }

                int currentRowHandle = bandedGridView1.FocusedRowHandle;

                // LẤY CỘT ĐANG ĐƯỢC FOCUS
                GridColumn focusedColumn = bandedGridView1.FocusedColumn;
                string focusedColumnName = focusedColumn?.FieldName;

                // KIỂM TRA GROUP ROW
                if (bandedGridView1.IsGroupRow(currentRowHandle))
                {
                    XtraMessageBox.Show("Vui lòng chọn một dòng dữ liệu, không phải dòng nhóm!", "Thông báo");
                    return;
                }

                if (currentRowHandle < 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn vị trí để paste!", "Thông báo");
                    return;
                }

                DataTable dt = bandedGridView1.GridControl.DataSource as DataTable;
                if (dt == null)
                {
                    XtraMessageBox.Show("DataSource bị null!", "Lỗi");
                    return;
                }

                bandedGridView1.BeginUpdate();

                try
                {
                    int currentHandle = currentRowHandle;
                    int copiedRowCount = 0;

                    // KIỂM TRA XEM CÓ PASTE CHỈ MỘT CỘT KHÔNG
                    bool pasteOnlyFocusedColumn = !string.IsNullOrEmpty(focusedColumnName) &&
                                                 focusedColumnName == "DinhMucChung";

                    while (copiedRowCount < copiedData.Rows.Count)
                    {
                        // BỎ QUA GROUP ROWS
                        while (currentHandle < bandedGridView1.RowCount &&
                               bandedGridView1.IsGroupRow(currentHandle))
                        {
                            currentHandle++;
                        }

                        // Nếu hết dòng visible, thoát
                        if (currentHandle >= bandedGridView1.RowCount)
                        {
                            break;
                        }

                        try
                        {
                            DataRow copiedRow = copiedData.Rows[copiedRowCount];
                            DataRow targetRow = bandedGridView1.GetDataRow(currentHandle);

                            if (targetRow != null)
                            {
                                // NẾU PASTE CHỈ CỘT DinhMucChung
                                if (pasteOnlyFocusedColumn)
                                {
                                    if (copiedData.Columns.Contains("DinhMucChung"))
                                    {
                                        targetRow["DinhMucChung"] = copiedRow["DinhMucChung"];
                                    }
                                }
                                else
                                {
                                    // PASTE TẤT CẢ CÁC CỘT (CODE GỐC)
                                    foreach (DataColumn col in dt.Columns)
                                    {
                                        // Bỏ qua cột STT
                                        if (col.ColumnName.ToUpper() == "STT")
                                        {
                                            continue;
                                        }
                                        else if (col.ColumnName.ToString() == "MaCode")
                                        {
                                            string random6 = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

                                            targetRow["MaCode"] = copiedRow["MaVTID"].ToString()
                                                             + "|" + random6
                                                             + "|" + targetRow["STT"].ToString();
                                            continue;
                                        }
                                        else if (col.ColumnName.Contains("@Mau@"))
                                        {
                                            string[] arrmauvtid = copiedRow["MauVTIDChung"].ToString().Split(',');
                                            if (arrmauvtid.Length == 1)
                                            {
                                                foreach (DataColumn dc in targetRow.Table.Columns)
                                                {
                                                    if (dc.ColumnName.Contains("@Mau@"))
                                                    {
                                                        targetRow[dc.ColumnName] = arrmauvtid[0];
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (DataColumn dc in targetRow.Table.Columns)
                                                {
                                                    if (dc.ColumnName.Contains("@Mau@"))
                                                    {
                                                        targetRow[dc.ColumnName] = "";
                                                    }
                                                }
                                            }
                                        }
                                        else if (copiedData.Columns.Contains(col.ColumnName))
                                        {
                                            targetRow[col.ColumnName] = copiedRow[col.ColumnName];
                                        }
                                    }
                                }

                                successCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                        }

                        copiedRowCount++;
                        currentHandle++;
                    }
                }
                finally
                {
                    bandedGridView1.EndUpdate();
                    bandedGridView1.RefreshData();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi paste: {ex.Message}", "Lỗi");
            }
        }
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
                if (isNL)
                {

                }
                else
                {

                }
                int currentSTT = Convert.ToInt32(currentRow["STT"]);
                int newSTTP = currentSTT + 1;
                // Tìm STT lớn nhất với MaVTID hiện tại
                var maxSTT = table.AsEnumerable()
                                  .Where(r => r["MaVTID"].ToString() == currentMaVTID && int.TryParse(r["STTCode"].ToString(), out _))
                                  .Select(r => Convert.ToInt32(r["STT"]))
                                  .DefaultIfEmpty(0)
                                  .Max();

                int newSTT = maxSTT + 1;
                int STTCode = table.Rows.Count + newSTT * 2;

                // Tạo dòng mới
                DataRow newRow = table.NewRow();
                foreach (DataColumn col in table.Columns)
                {
                    newRow[col.ColumnName] = currentRow[col.ColumnName];
                }

                newRow["STTCode"] = newSTT;
                newRow["MaCode"] = $"{currentMaVTID}|{STTCode}";

                newRow["DinhMucHaoHut"] = 0;
                newRow["DinhMucChung"] = 1;
                newRow["DinhMucKH"] = 1;
                newRow["MaDVCL"] = "DVVT_7";

                newRow["MaDVDM"] = "DVVT_7";
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
                //string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                //string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                //string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                //int Rowfocus_Idx = bandedGridView1.FocusedRowHandle;
                //if (Rowfocus_Idx < 0) return;
                //DataRow rowFocus = bandedGridView1.GetFocusedDataRow();
                //if (rowFocus != null)
                //{

                //    DialogResult result = XtraMessageBox.Show(
                //     "Bạn có chắc chắn muốn xóa không?",
                //     "XÁC NHẬN XÓA",
                //     MessageBoxButtons.YesNo,
                //     MessageBoxIcon.Warning);

                //    if (result == DialogResult.No)
                //    {
                //        return;
                //    }
                //    else
                //    {
                //        object MaVTID = rowFocus["MaVTID"];
                //        //object MauVatID = rowFocus["MauVTID"];
                //        object MaNhom = rowFocus["MaNhom"];
                //        object KhoVaiID = rowFocus["KhoVaiID"];
                //        object MaCode = rowFocus["MaCode"];

                //        string[] mauvtidchung = rowFocus["MauVTIDChung"].ToString().Split(',');
                //        foreach (string mauvtid in mauvtidchung)
                //        {
                //            dynamic JPara = new
                //            {
                //                MaVTID = MaVTID,
                //                MauVTID = mauvtid,
                //                MaNhom = MaNhom,
                //                KhoVaiID = KhoVaiID,
                //                MaCode = MaCode,
                //                MaKH = makh,
                //                MaHang = mahang,
                //                MaDot = madot,
                //                UserID = GlobleData.UserName
                //            };



                //            string urlRemove = $"{URL}DeleteBOM_DM/Delete";
                //            string resultRemove = Task.Run(async () => { return await _clientExtension.PostAsync(urlRemove, JPara); }).Result;
                //            if (string.Compare(resultRemove?.ToLower(), "true") != 0)
                //            {
                //                XtraMessageBox.Show($"Xóa đã xảy ra lỗi.Vui lòng thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //                return;
                //            }
                //            RemoveRowDM(rowFocus, Rowfocus_Idx);
                //        }



                //    }
                //}
                string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string madot = isAdd ? _madot : (searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString());

                // Lấy các dòng đã chọn
                int[] selectedRows = bandedGridView1.GetSelectedRows();

                if (selectedRows.Length == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn dòng cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = XtraMessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa {selectedRows.Length} dòng đã chọn?",
                    "XÁC NHẬN XÓA",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    return;
                }

                // Lưu danh sách các dòng cần xóa (xóa từ cuối lên đầu để tránh lỗi index)
                List<DataRow> rowsToDelete = new List<DataRow>();
                List<int> indexesToDelete = new List<int>();

                foreach (int rowHandle in selectedRows)
                {
                    if (rowHandle >= 0)
                    {
                        DataRow row = bandedGridView1.GetDataRow(rowHandle);
                        if (row != null)
                        {
                            rowsToDelete.Add(row);
                            indexesToDelete.Add(rowHandle);
                        }
                    }
                }
                var nlRowsToCheck = rowsToDelete.Where(r => r["NPL"].ToString().ToUpper() == "TRUE").ToList();

                if (nlRowsToCheck.Count > 0)
                {
                    DataTable dtCheck = frmPhanTichBOMViewSD.BuildRowsTable(makh, mahang, madot, nlRowsToCheck);
                    string url = $"{URL}KhoiTaoBOMV1/GetSD?action=CheckVTSD&para1={makh}&para2={mahang}&para3={madot}";
                    string jsonCheck = Task.Run(async () => await _clientExtension.PostAsync(url, dtCheck)).Result;
                    DataTable tblCheck = JsonConvert.DeserializeObject<DataTable>(jsonCheck);
                    if (tblCheck != null && tblCheck.Rows.Count > 0)
                    {
                        var frmSD = new frmPhanTichBOMViewSD(makh, mahang, madot, dtCheck, GlobleData.UserName);
                        if (frmSD.ShowDialog() != DialogResult.OK)
                        {
                            foreach (var r in nlRowsToCheck)
                                skippedRows.Add(r);
                        }
                    }
                }
                for (int i = 0; i < rowsToDelete.Count; i++)
                {
                    DataRow rowFocus = rowsToDelete[i];
                    if (skippedRows.Contains(rowFocus)) continue;
                    object MaVTID = rowFocus["MaVTID"];
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
                            XtraMessageBox.Show($"Xóa dòng {i + 1}/{rowsToDelete.Count} đã xảy ra lỗi. Vui lòng thực hiện lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                indexesToDelete.Sort();
                indexesToDelete.Reverse();

                for (int i = 0; i < indexesToDelete.Count; i++)
                {
                    //RemoveRowDM(rowsToDelete[rowsToDelete.Count - 1 - i], indexesToDelete[i]);
                    DataRow rowToRemove = rowsToDelete[rowsToDelete.Count - 1 - i];
                    if (skippedRows.Contains(rowToRemove)) continue;
                    RemoveRowDM(rowToRemove, indexesToDelete[i]);
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
        private void toggleIsActive_Toggled(object sender, EventArgs e)
        {

            toggleIsActive.Properties.Appearance.ForeColor = toggleIsActive.IsOn ? Color.ForestGreen : Color.DimGray;
            bool isActive = toggleIsActive.IsOn;
            string title = isActive ? "BOM NPL đã ban hành" : "BOM NPL đã hủy ban hành";
            SendNotify(title, "PKH");
            DoActive(isActive);

        }
        private void toggleIsActive_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            //bool isOn = Convert.ToBoolean(e.NewValue);
            //if (!_isDuyet & isOn)
            //{
            //    XtraMessageBox.Show($"Không thể active vì đợt chưa được duyệt hoàn toàn.Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    e.Cancel = true;
            //}
            bool isOn = Convert.ToBoolean(e.NewValue);

            if (isOn)
            {
                if (_isHuyDot)
                {
                    XtraMessageBox.Show(
                        "Đợt này đã hủy. Không thể Inactive!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    e.Cancel = true;
                    return;
                }
                if (!_isDuyet)
                {
                    XtraMessageBox.Show(
                        "Không thể active vì đợt chưa được duyệt hoàn toàn. Vui lòng kiểm tra lại!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    e.Cancel = true;
                    return;
                }

                DialogResult result = XtraMessageBox.Show(
                    "Bạn có chắc chắn muốn Active không?",
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

        private void CreateRepoSearchLookUpChungLoaiCT()
        {
            try
            {
                repoSearchCLCT.DisplayMember = "TenNhomChiTiet";
                repoSearchCLCT.ValueMember = "MaNhomChiTiet";
                searchLookUpEditCLCT.Properties.DisplayMember = "TenNhomChiTiet";
                searchLookUpEditCLCT.Properties.ValueMember = "MaNhomChiTiet";
                string url = $"{URL}KhoiTaoBOMV1/Get?action=GETCHUNGLOAICHITIET";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                tblChungLoaiChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
                repoSearchCLCT.DataSource = tblChungLoaiChiTiet;
                searchLookUpEditCLCT.Properties.DataSource = tblChungLoaiChiTiet;

            }
            catch (Exception ex)
            {
            }
        }

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
        private void DoActive(bool IsActive)
        {
            string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            if (madot.ToString() == "") return;
            int active = IsActive ? 1 : 0;

            string url = $"{URL}KhoiTaoBOMV1/Post1?action=DoActive&para1={active}&para2={makh}&para3={mahang}&para4={madot}&para5={GlobleData.UserName}";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
            if (msResult.ToLower() != "true") return;
            string urlXN = $"{URL}ERPDonHangTong/Get?action=UpdateIsActiveBOM&para={active}&para2={makh}&para3={mahang}&para4={madot}";
            string jsonXN = Task.Run(async () => await _clientExtension.GetAsnyc(urlXN)).Result;
            if (msResult.ToLower() != "true") return;
            else
            {
                //string userName = GlobleData.UserName ?? ""; 
                //string urlTS = $"{URL}KhoiTaoBOMV1/Get?action=GET_TSNEW&para1={makh}&para2={mahang}&para6={userName}&para7={madot}";
                //string jsonTS = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTS); }).Result;
                if (active == 1)
                {
                    string userName = GlobleData.UserName ?? "";
                    //string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                    //string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                    //string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                    string urlTS = $"{URL}KhoiTaoBOMV1/Get?action=GET_TSNEW&para1={makh}&para2={mahang}&para6={userName}&para7={madot}";
                    string jsonTS = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTS); }).Result;

                    string _Module = "PhanTichBOM";
                    string _Action = "Active BOM";
                    string _Contents = makh + "@" + mahang + "@" + madot;
                    string urls = $"{URL}CanDoiDinhMucNPLLog/GhiLogLenh?action=GhiLogLenh&para={_Action}&para1={_Module}&para2={_Contents}&para3={GlobleData.UserName}";
                    string jsons = Task.Run(async () => await _clientExtension.GetAsnyc(urls)).Result;
                }
                else
                {
                    string _Module1 = "PhanTichBOM";
                    string _Action1 = "Inactive BOM";
                    string _Contents1 = makh + "@" + mahang + "@" + madot;
                    string urls1 = $"{URL}CanDoiDinhMucNPLLog/GhiLogLenh?action=GhiLogLenh&para={_Action1}&para1={_Module1}&para2={_Contents1}&para3={GlobleData.UserName}";
                    string jsons1 = Task.Run(async () => await _clientExtension.GetAsnyc(urls1)).Result;
                }

                clsWaitForm.ShowSuccessForm(this, 3000);
            }
        }
        private void gridView7_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {

            DataRow dr = bandedGridView1.GetFocusedDataRow();
            if (dr == null) return;

            string manhonm = dr["MaNhom"].ToString();
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
        private void searchLookUpEditCLCT_QueryPopUp(object sender, CancelEventArgs e)
        {
            int[] selectedRowHandles = bandedGridView1.GetSelectedRows();
            if (selectedRowHandles.Length == 0) return;
            List<DataRow> rowsFromTbl = new List<DataRow>();
            foreach (int rowHandle in selectedRowHandles)
            {
                if (rowHandle < 0) continue;
                if (!bandedGridView1.IsValidRowHandle(rowHandle)) continue;
                DataRow drSL = bandedGridView1.GetDataRow(rowHandle);
                if (drSL != null)
                    rowsFromTbl.Add(drSL);
            }
            if (rowsFromTbl.Count == 0) return;
            string maNhomDau = rowsFromTbl[0].Field<string>("MaNhom");
            bool allSame = rowsFromTbl.All(r => string.Equals(r.Field<string>("MaNhom"), maNhomDau, StringComparison.OrdinalIgnoreCase));

            if (!allSame)
            {
                XtraMessageBox.Show(this, "Các dòng được chọn phải chung một chủng loại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

        }
        private void searchLookUpEditCLCT_EditValueChanged(object sender, EventArgs e)
        {
            int[] selectedRowHandles = bandedGridView1.GetSelectedRows();
            if (selectedRowHandles.Length == 0) return;

            foreach (int rowHandle in selectedRowHandles)
            {
                if (rowHandle < 0) continue;
                if (!bandedGridView1.IsValidRowHandle(rowHandle)) continue;
                DataRow drSL = bandedGridView1.GetDataRow(rowHandle);
                if (drSL != null)
                    drSL["MaNhomChiTiet"] = searchLookUpEditCLCT.EditValue.ToString();
                bandedGridView1.RefreshRow(rowHandle);
                bandedGridView1_CellValueChanged(bandedGridView1, new DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs(rowHandle, bandedGridView1.Columns["MaNhomChiTiet"], drSL["MaNhomChiTiet"]));

            }
            this.ActiveControl = button1;
        }

        private void createTableCT(DataTable tab)
        {
            DataTable tbl = gridControl1.DataSource as DataTable;
            GridBand parentBand = bandedGridView1.Bands["gridBandMauSP"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < bandedGridView1.Columns.Count;)
                {
                    if (bandedGridView1.Columns[i].FieldName.Contains("gridBandMauSPa"))
                    {
                        bandedGridView1.Columns.RemoveAt(i);
                    }
                    else
                    {
                        i += 1;
                    }
                }
            }

            foreach (DataRow column in tab.Rows)
            {
                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = column[4].ToString();
                col.FieldName = column[4].ToString() + "@Mau@" + column[1].ToString();
                col.Name = "col" + column[4].ToString();
                // col.OptionsColumn.AllowEdit = false; // REMOVE DÒNG NÀY!
                col.Visible = true;
                col.Width = 50;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

                // GÁN EDITOR TRƯỚC KHI ADD COLUMN
                col = CreateSearchLookUpMauVT(col);
                bandedGridView1.Columns.AddRange(new BandedGridColumn[] { col });

                GridBand gb = new GridBand();
                gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                gb.AppearanceHeader.Options.UseTextOptions = true;
                gb.Caption = column[4].ToString();
                gb.Columns.Add(col);
                gb.Name = "gridBandMauSPa" + col;
                gb.VisibleIndex = 0;
                gb.Width = 60;
                gridBandMauSP.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                if (tbl != null)
                {
                    tbl.Columns.Add(column[1].ToString(), typeof(string));
                }
                string caption = column[4].ToString();
                int estimatedWidth = Math.Max(50, caption.Length * 8 + 20); // 8px/char + padding
                col.Width = estimatedWidth;
                gb.Width = estimatedWidth;
            }
            gridControl1.DataSource = tbl;
            //bandedGridView1.BestFitColumns(true);
        }



        private BandedGridColumn CreateSearchLookUpMauVT(BandedGridColumn col)
        {
            try
            {
                // KIỂM TRA DATA SOURCE
                if (tblMauVT == null || tblMauVT.Rows.Count == 0)
                {
                    MessageBox.Show("Bảng màu chưa có dữ liệu!");
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
                    if (dr["KhoVaiID"].ToString() == "")
                    {
                        XtraMessageBox.Show("Chưa chọn Khổ/size nên không thể chọn màu vật tư. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }
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

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                using (var frm = new frmImportEXBOM())
                {
                    var result = frm.ShowDialog();

                    if (result == DialogResult.OK)
                    {

                        loadSizeChung();
                        CreateSearchLookUpDot();
                        CreateMauSPColumns();
                        LoadData();
                    }
                    else if (result == DialogResult.No)
                    {

                        this.Close();
                        return;
                    }
                    loadSizeChung();
                    CreateSearchLookUpDot();
                    CreateMauSPColumns();
                    LoadData();

                }
            }
            catch (Exception ex)
            {

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
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETMAUSPCOLUMNS&para1={makh}&para2={mahang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblMauSP = JsonConvert.DeserializeObject<DataTable>(json);
            createTableCT(_tblMauSP);
        }

        private void repositoryItemSearchLookUpEditSizeSP_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var repo = sender as DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit;
            var popupView = repo?.View as DevExpress.XtraGrid.Views.Grid.GridView;
            if (popupView == null) return;

            var rows = popupView.GetSelectedRows()
                .Select(handle => popupView.GetDataRow(handle))
                .Where(r => r != null)
                .ToList();
            if (rows.Count == 0) return;

            var text = string.Join("; ",
                rows.GroupBy(r => r["NhomSize"])
                    .Select(g => $"{g.Key}:{string.Join(",", g.Select(r => r["TenSize"]))}"));
            e.DisplayText = text;
        }

        private void LoadTVMauVT()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETMAUVTTV";
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

                        e.DisplayText = rows[0]["MaMauVT"].ToString() + "/" + rows[0]["MauVT"].ToString();
                        //if (rows[0]["MaMauVT"].ToString() == "")
                        //    e.DisplayText = "";
                    }
                }
            }

            if (e.Column.FieldName == "MaVTID")
            {
                if (e.Value != null && e.Value != DBNull.Value)
                {
                    string maVTID = e.Value.ToString();


                    DataRow[] rows = tblMaVT.Select($"MaVTID = '{maVTID}'");
                    if (rows.Length > 0)
                    {
                        e.DisplayText = rows[0]["MaVT"].ToString();
                    }
                }
            }
            if (e.Column.FieldName == "DinhMucChung" || e.Column.FieldName == "DinhMucKH") // đổi tên cột cho đúng
            {
                var isPS = bandedGridView1.GetRowCellValue(e.ListSourceRowIndex, "IsPS");

                if (isPS != null && isPS.ToString() == "1")
                {
                    string rawValue = e.Value?.ToString();


                    e.DisplayText = $"1:{rawValue}";
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
                    IsXetDuyet = row["IsXetDuyet"]?.ToString().ToLower() == "đã xác nhận";
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
            CreateRepoSearchLookUpChungLoaiCT();
            NapLai(!isAdd);
        }
        private void bandedGridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {

            var view = sender as BandedGridView;
            if (view == null) return;

            // KIỂM tra xem cell có đang được selected không
            bool isSelected = view.IsRowSelected(e.RowHandle);

            // NẾU ĐANG SELECTED thì KHÔNG APPLY custom color
            if (isSelected)
            {
                return; // Để GridView tự động apply màu selection mặc định
            }

            var maNhomChiTiet = view.GetRowCellValue(e.RowHandle, "MaNhomChiTiet") as string;
            bool isMaNhomChiTietEmpty = string.IsNullOrWhiteSpace(maNhomChiTiet);
            bool allMauColumnsEmpty = true;

            foreach (BandedGridColumn col in view.Columns)
            {
                if (col.FieldName != null && col.FieldName.Contains("@Mau@"))
                {
                    var value = view.GetRowCellValue(e.RowHandle, col);
                    string strValue = Convert.ToString(value);
                    if (!string.IsNullOrWhiteSpace(strValue) && strValue != "MAUVT_0")
                    {
                        allMauColumnsEmpty = false;
                        break;
                    }
                }
            }

            if (isMaNhomChiTietEmpty || allMauColumnsEmpty)
            {
                e.Appearance.BackColor = Color.MistyRose;
                e.Appearance.ForeColor = Color.DarkRed;
                return;
            }

            var maVT = view.GetRowCellValue(e.RowHandle, "MaVT") as string;
            if (string.IsNullOrWhiteSpace(maVT))
            {
                e.Appearance.BackColor = Color.MistyRose;
                e.Appearance.ForeColor = Color.DarkRed;
                return;
            }

            if (e.Column.FieldName == "IsXetDuyet")
            {
                object cellValue = e.CellValue;
                if (cellValue != null)
                {
                    if (cellValue.ToString() == "Đã xác nhận")
                    {
                        e.Appearance.ForeColor = Color.Green;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }
            }
            if (e.Column.FieldName == "DinhMucChung")
            {
                var dinhMucChungVal = view.GetRowCellValue(e.RowHandle, "DinhMucChung");
                var dinhMucKHVal = view.GetRowCellValue(e.RowHandle, "DinhMucKH");

                if (dinhMucChungVal != null && dinhMucKHVal != null
                    && decimal.TryParse(Convert.ToString(dinhMucChungVal), out decimal dinhMucChung)
                    && decimal.TryParse(Convert.ToString(dinhMucKHVal), out decimal dinhMucKH))
                {
                    if (dinhMucChung > dinhMucKH)
                    {
                        e.Appearance.BackColor = Color.Red;
                        e.Appearance.ForeColor = Color.White;
                    }
                }
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
                if (row.Table.Columns.Contains("IsXetDuyet"))
                {
                    bool IsXetDuyet = false;
                    IsXetDuyet = row["IsXetDuyet"]?.ToString().ToLower() == "đã xác nhận";
                    if (IsXetDuyet)
                    {
                        XtraMessageBox.Show("Đợt đã được xác nhận. Không thể chỉnh sửa. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        this.ActiveControl = button1;
                        return;
                    }

                }
                if ((bandedGridView1.FocusedColumn.FieldName == "Size" || bandedGridView1.FocusedColumn.FieldName.Contains("@Mau@")) && bandedGridView1.ActiveEditor is ButtonEdit buttonEdit)
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

            }
            catch (Exception ex)
            {


            }
        }


        private void btnXoaMauVT_Click(object sender, EventArgs e)
        {

            DataTable tbl = gridControl1.DataSource as DataTable;
            var view = bandedGridView1;
            GridCell[] selected = view.GetSelectedCells();
            if (selected.Length == 0) return;
            view.BeginUpdate();
            try
            {
                foreach (var cell in selected)
                {
                    if (!view.IsDataRow(cell.RowHandle)) continue;
                    if (!cell.Column.OptionsColumn.AllowEdit) continue;
                    view.SetRowCellValue(cell.RowHandle, cell.Column, string.Empty);
                }
            }
            finally
            {
                view.EndUpdate();
            }
        }
        private void loadSizeChung()
        {
            string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETSIZESP&para1={makh.ToString()}&para2={mahang.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                tblSizeChung = JsonConvert.DeserializeObject<DataTable>(json);

            }
        }



        private void btnXuatMauExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("BOM{0}", DateTime.Now.Millisecond.ToString(), DateTime.Now.Millisecond.ToString());
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "BOMTemplate.xlsx";
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

                string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETMAUSPCOLUMNS&para1={makh}&para2={mahang}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblMauSPExcel = JsonConvert.DeserializeObject<DataTable>(json);

                System.IO.FileInfo file = new System.IO.FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {

                        excelPackage.Workbook.Properties.Author = "NTB";
                        excelPackage.Workbook.Properties.Title = "BOM";

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

                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    int row = 7, column = 13;
                    if (tblMauSPExcel != null && tblMauSPExcel.Rows.Count > 0)
                    {
                        foreach (DataRow dr in tblMauSPExcel.Rows)
                        {
                            worksheet.Cells[row, column].Value = dr["TenMau"].ToString();
                            worksheet.Cells[row, column].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            worksheet.Cells[row, column].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            worksheet.Cells[row, column].Style.Font.Bold = false;
                            worksheet.Column(column).AutoFit();
                            column++;
                        }
                    }
                    excelPackage.Save();
                }
            }
            catch (Exception ex)
            {
            }

        }
        #endregion


        private void btnChonVatTu_Click(object sender, EventArgs e)
        {
            string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string dot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            if (dot == "" && textBox1.Text == "")
            {
                return;
            }
            if (isAdd)
            {
                dot = _madot;
            }
            if (_isActiveDot)
            {
                XtraMessageBox.Show($"Đợt đã được Active nên không thể thêm vật tư.Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DataTable tbl = gridControl1.DataSource as DataTable;
            frmPhanTichBOM_ChonVatTu frm = new frmPhanTichBOM_ChonVatTu(kh, mh, dot, tbl);
            if (frm.ShowDialog() == DialogResult.OK)
            {

                List<DataRow> lstSelect = frm.lstSelect.OrderBy(row => Convert.ToInt32(row["STT"])).ToList();
                foreach (DataRow row in lstSelect)
                {
                    AddRowGCNL(row);
                }
            }
        }
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
            newRow["DinhMucChung"] = 1;
            newRow["DinhMucHaoHut"] = 0;
            newRow["TachMau"] = false;
            newRow["IsNew"] = 1;
            newRow["MaDVCL"] = "DVVT_7";
            newRow["MaDVDM"] = "DVVT_7";
            newRow["STTCode"] = 0;


            bool isNL = firstRow["NPL"].ToString().ToUpper() == "TRUE" ? true : false;
            string random6 = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

            newRow["MaCode"] = firstRow["MaVTID"].ToString()
                             + "|" + random6
                             + "|" + (isNL ? _sttVT : _sttVTPL).ToString();

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

            string mauvtid = "";
            newRow["MaNhomChiTiet"] = maNhomChiTiet;
            newRow["MauVTIDChung"] = firstRow["MauVTIDChung"].ToString();
            if (firstRow["MauVTIDChung"].ToString().Split(',').Length == 1)
            {
                mauvtid = firstRow["MauVTIDChung"].ToString();
            }
            newRow["Size"] = "";
            foreach (DataColumn col in newRow.Table.Columns)
            {
                if (col.ColumnName.Contains("@Mau@"))
                {
                    newRow[col.ColumnName] = mauvtid;
                }
            }
            if (isNL)
            {
                _sttVT++;
            }
            else
            {
                _sttVTPL++;
            }
            //tbl.Rows.Add(newRow);

            // gridControl1.DataSource = tbl;

        }
        private void AddRowGCNL(DataRow rowAdd)
        {
            try
            {
                if (rowAdd == null) return;
                DataTable tbl = gridControl1.DataSource as DataTable;
                if (tbl == null)
                {
                    tbl = createTableSaveEdit();
                }
                string filter = string.Format("MaVTID = '{0}' AND KhoVaiID = '{1}' AND MaNhom = '{2}' AND MaDVVT = '{3}'",
                                              rowAdd["MaVTID"], rowAdd["KhoVaiID"], rowAdd["MaNhom"], rowAdd["MaDVVT"]);
                DataRow[] existingRows = tbl.Select(filter);
                if (existingRows.Length == 0)
                {
                    DataRow newRow = tbl.NewRow();
                    bool isNL = rowAdd["NPL"].ToString() == "True" ? true : false;
                    newRow["MaNhom"] = rowAdd["MaNhom"];
                    newRow["TenNhom"] = rowAdd["TenNhom"];
                    newRow["NPL"] = rowAdd["NPL"];
                    newRow["Sort"] = rowAdd["Sort"];
                    newRow["MaVTID"] = rowAdd["MaVTID"];
                    newRow["MaVT"] = rowAdd["MaVT"];
                    newRow["ChiTiet"] = rowAdd["VatTu"];
                    newRow["MaDVVT"] = rowAdd["MaDVVT"];
                    newRow["TenDVVT"] = rowAdd["TenDVVT"];
                    newRow["KhoVaiID"] = rowAdd["KhoVaiID"];
                    newRow["KhoVai"] = rowAdd["KhoVai"];
                    newRow["STT"] = isNL ? _sttVT : _sttVTPL;
                    newRow["DinhMucChung"] = 1;
                    newRow["DinhMucKH"] = 1;
                    newRow["DinhMucHaoHut"] = 0;
                    newRow["TachMau"] = false;
                    newRow["IsNew"] = 1;

                    newRow["MaDVCL"] = "DVVT_7";
                    newRow["MaDVDM"] = "DVVT_7";
                    newRow["IsPS"] = rowAdd["IsPS"];
                    newRow["STTCode"] = isNL ? _sttVT : _sttVTPL;

                    string random6 = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

                    newRow["MaCode"] = rowAdd["MaVTID"].ToString()
                                     + "|" + random6
                                     + "|" + (isNL ? _sttVT : _sttVTPL).ToString();
                    newRow["IsActive"] = false;

                    // Logic tìm kiếm và gán MaNhomChiTiet
                    string maNhomChiTiet = "";
                    bool nplValue = Convert.ToBoolean(rowAdd["NPL"]);

                    if (nplValue)
                    {
                        // NPL = true: Tìm theo MaNhom
                        string filterChungLoai = string.Format("MaNhom = '{0}'", rowAdd["MaNhom"]);
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
                        string filterChungLoai = string.Format("TenNhomChiTiet = '{0}'", rowAdd["TenNhom"].ToString().Replace("'", "''"));
                        DataRow[] foundRows = tblChungLoaiChiTiet.Select(filterChungLoai);

                        // Gán nếu tìm được ít nhất 1 dòng (lấy dòng đầu tiên)
                        if (foundRows.Length > 0)
                        {
                            maNhomChiTiet = foundRows[0]["MaNhomChiTiet"].ToString();
                        }
                    }

                    newRow["MaNhomChiTiet"] = maNhomChiTiet;
                    newRow["MauVTIDChung"] = rowAdd["MauVTIDChung"].ToString();
                    newRow["Size"] = "";
                    string[] arrmauvtid = rowAdd["MauVTIDChung"].ToString().Split(',');
                    if (arrmauvtid.Length == 1)
                    {
                        foreach (DataColumn dc in newRow.Table.Columns)
                        {
                            if (dc.ColumnName.Contains("@Mau@"))
                            {
                                newRow[dc.ColumnName] = arrmauvtid[0];
                            }
                        }
                    }
                    tbl.Rows.Add(newRow);
                    if (isNL)
                    {
                        _sttVT++;
                    }
                    else
                    {
                        _sttVTPL++;
                    }
                    gridControl1.DataSource = tbl;

                }
            }
            catch (Exception ex)
            {

            }
        }



        private void ExportEx_btn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel (2010) (.xlsx)|*.xlsx|Excel (2003)(.xls)|*.xls";
            Sfd.FileName = string.Format("{0}.BOM-{1}", searchLookUpEditMH.EditValue.ToString().Replace("_", ""), DateTime.Now.ToString("ddMMyyyy"));
            if (Sfd.ShowDialog() == DialogResult.OK)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất Excel");
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                string fileName = "BOMTemplate.xlsx";
                string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                string TemplateFileName = path;
                string ExportFileName = Sfd.FileName;
                ExportExcelToImport(TemplateFileName, ExportFileName);

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
        public void ExportExcelToImport(string TemplateFileName, string ExportFileName)
        {
            try
            {

                DataTable tblExport = gridControl1.DataSource as DataTable;
                if (tblExport == null || tblExport.Rows.Count == 0) return;
                System.IO.FileInfo file = new System.IO.FileInfo(ExportFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    if (TemplateFileName != string.Empty)
                    {

                        excelPackage.Workbook.Properties.Author = "NTB";
                        excelPackage.Workbook.Properties.Title = "BOM";

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

                    var worksheet = excelPackage.Workbook.Worksheets[0];
                    //khách hàng,mã hàng
                    List<string> colorFieldNames = new List<string>();
                    worksheet.Cells["C3"].Value = searchLookUpEditKH.Text.ToString();
                    worksheet.Cells["C4"].Value = searchLookUpEditMH.Text.ToString();
                    worksheet.Cells["D4"].Value = "ĐỢT";
                    worksheet.Cells["D4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells["D4"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells["D4"].Style.Font.Bold = true;
                    worksheet.Cells["E4"].Value = searchLookUpEditDot.Text.ToString();
                    worksheet.Cells["E4"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells["E4"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells["E4"].Style.Font.Bold = true;

                    int row = 7, column = 14; // in màu sản phẩm ở dòng 7, cột 15
                    int currentColHeader = 14; // cột 15 bắt đầu là màu sản phẩm
                    //gán màu
                    foreach (GridBand band in bandedGridView1.Bands)
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

                                    colorFieldNames.Add(fieldName);

                                    worksheet.Cells[row, currentColHeader].Value = headerText;
                                    worksheet.Cells[row, currentColHeader].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[row, currentColHeader].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                    worksheet.Cells[row, currentColHeader].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    worksheet.Cells[row, currentColHeader].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    worksheet.Cells[row, currentColHeader].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    worksheet.Cells[row, currentColHeader].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    worksheet.Cells[row, currentColHeader].Style.Font.Bold = true;

                                    worksheet.Cells[row + 1, currentColHeader].Value = "(" + currentColHeader + ")";
                                    worksheet.Cells[row + 1, currentColHeader].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[row + 1, currentColHeader].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                                    worksheet.Cells[row + 1, currentColHeader].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    worksheet.Cells[row + 1, currentColHeader].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    worksheet.Cells[row + 1, currentColHeader].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    worksheet.Cells[row + 1, currentColHeader].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                    worksheet.Cells[row + 1, currentColHeader].Style.Font.Bold = true;


                                }
                                currentColHeader++;
                            }
                        }

                    }
                    var range = worksheet.Cells[6, column, 6, currentColHeader - 1];

                    // Merge
                    range.Merge = true;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SkyBlue);
                    range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;



                    var rangeTop = worksheet.Cells[1, column, 2, currentColHeader - 1];

                    // Merge
                    rangeTop.Merge = true;
                    rangeTop.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    rangeTop.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    //rangeTop.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    //rangeTop.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    rangeTop.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    //rangeTop.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    rangeTop.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    //gán dữ liệu
                    int rowNumber = 9; // dòng 9 bắt đầu vật tư
                    int ColorCount = currentColHeader - 15;
                    int stt = 1;
                    tblExport.DefaultView.Sort = "NPL DESC,STT ASC";
                    tblExport = tblExport.DefaultView.ToTable();
                    foreach (DataRow dr in tblExport.Rows)
                    {
                        worksheet.Cells[rowNumber, 1].Value = stt;
                        stt++;
                        worksheet.Cells[rowNumber, 2].Value = dr["TenNhom"].ToString();
                        string maNhom = dr["MaNhomChiTiet"].ToString();

                        string tenNhom = tblChungLoaiChiTiet.AsEnumerable()
                            .Where(r => r["MaNhomChiTiet"].ToString() == maNhom)
                            .Select(r => r["TenNhomChiTiet"].ToString())
                            .FirstOrDefault();
                        worksheet.Cells[rowNumber, 3].Value = tenNhom;

                        //string maDVCL = dr["MaDVCL"].ToString();

                        //string tenDVCL = tblDV.AsEnumerable()
                        //    .Where(r => r["MaDVVT"].ToString() == maDVCL)
                        //    .Select(r => r["TenDVVT"].ToString())
                        //    .FirstOrDefault();

                        //worksheet.Cells[rowNumber, 4].Value = string.IsNullOrEmpty(tenDVCL) ? "": tenDVCL.ToString();


                        worksheet.Cells[rowNumber, 4].Value = dr["MaVT"].ToString();
                        worksheet.Cells[rowNumber, 5].Value = dr["ChiTiet"].ToString();
                        worksheet.Cells[rowNumber, 6].Value = dr["KhoVai"].ToString();
                        worksheet.Cells[rowNumber, 7].Value = dr["TenDVKV"].ToString();
                        worksheet.Cells[rowNumber, 8].Value = dr["TenDVVT"].ToString();
                        worksheet.Cells[rowNumber, 9].Value = dr["DinhMucKH"].ToString();
                        worksheet.Cells[rowNumber, 10].Value = dr["DinhMucChung"].ToString();

                        //string maDVDM = dr["MaDVDM"].ToString();

                        //string tenDVDM = tblDV.AsEnumerable()
                        //    .Where(r => r["MaDVVT"].ToString() == maDVDM)
                        //    .Select(r => r["TenDVVT"].ToString())
                        //    .FirstOrDefault();

                        //worksheet.Cells[rowNumber, 11].Value = string.IsNullOrEmpty(tenDVDM) ? "" : tenDVDM.ToString();
                        worksheet.Cells[rowNumber, 11].Value = dr["DinhMucHaoHut"].ToString();

                        worksheet.Cells[rowNumber, 12].Value = dr["Size"].ToString() == "All Size" ? "" : dr["Size"].ToString();
                        worksheet.Cells[rowNumber, 13].Value = dr["GhiChu"].ToString();




                        var range2 = worksheet.Cells[rowNumber, 1, rowNumber, 13];

                        range2.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range2.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range2.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        range2.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        //range2.Style.Border.Horizontal.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        //range2.Style.Border.Vertical.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                        int colorStartCol = 14;
                        for (int i = 0; i < colorFieldNames.Count; i++)
                        {
                            string field = colorFieldNames[i];

                            // Kiểm tra cột tồn tại trong DataTable
                            if (tblExport.Columns.Contains(field))
                            {
                                object value = dr[field];

                                string tenMau = tblMauVT.AsEnumerable()
                                    .Where(r => r["MauVTID"].ToString() == value.ToString())
                                    .Select(r => r["MaMauVT"].ToString() + "/" + r["MauVT"].ToString())
                                    .FirstOrDefault();


                                worksheet.Cells[rowNumber, colorStartCol + i].Value =
                                    string.IsNullOrEmpty(tenMau) ? "" : tenMau.ToString();


                            }


                        }

                        rowNumber++;

                    }
                    for (int col = 14; col < currentColHeader; col++)
                    {
                        worksheet.Column(col).AutoFit();
                    }

                    // Bước 2: Gán lại border sau khi autofit
                    int lastDataRow = rowNumber - 1; // rowNumber đã ++ rồi nên -1
                    for (int r = 9; r <= lastDataRow; r++)
                    {
                        for (int col = 14; col < currentColHeader; col++)
                        {
                            var cell = worksheet.Cells[r, col];
                            cell.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            cell.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            cell.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            cell.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }
                    }
                    excelPackage.Save();
                }
            }
            catch (Exception ex)
            {
            }

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

                BandedGridView view = bandedGridView1 as BandedGridView;
                GridBand colcl = view.Bands["gridBandMauSP"];

                Range headerRange = sheet.Rows[headerTableIndex];
                Range beginHeaderRange = sheet.Range.FromLTRB(rangeLeft, rangeTop, rangeLeft + colcl.VisibleIndex, rangeTop);
                Range colorRange = sheet.Range.FromLTRB(rangeLeft + colcl.VisibleIndex + 1, rangeTop, rangeLeft + colcl.VisibleIndex + 1, rangeTop);
                Range endHeaderRange = sheet.Range.FromLTRB(rangeLeft + colcl.VisibleIndex + 2, rangeTop, rangeRight, rangeTop);
                Range gridRange = sheet.Range.FromLTRB(rangeLeft, rangeTop, rangeRight, rangeBottom);

                beginHeaderRange.FillColor = Color.FromArgb(255, 228, 179);
                colorRange.FillColor = Color.FromArgb(0, 218, 218);
                endHeaderRange.FillColor = Color.FromArgb(255, 228, 179);
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
                else
                    sheet.Cells["E4"].Value = textBox1.Text;
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


                currentCol = colcl.VisibleIndex + 1;
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
        private void simpleButton1_Click_2(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(searchLookUpEditKH.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn khách hàng", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            if (string.IsNullOrEmpty(searchLookUpEditMH.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn mã hàng", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            if (string.IsNullOrEmpty(searchLookUpEditDot.EditValue.ToString()))
            {
                XtraMessageBox.Show("Chưa chọn đợt", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            string makh = searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue.ToString();
            string madot = searchLookUpEditDot.EditValue.ToString();
            frmXemDMLenh frm = new frmXemDMLenh(makh, mahang, madot);
            frm.ShowDialog();
        }
        private void loadKhoSizeBOM()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETKHOSIZEBOM";
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
                newRow["DinhMucChung"] = 1;
                newRow["DinhMucKH"] = 1;
                newRow["MaDVCL"] = "DVVT_7";

                newRow["MaDVDM"] = "DVVT_7";
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


        }



        private bool KiemTraDuLieuTuDataTable(DataTable dt, bool _isNPL)
        {
            if (dt == null || dt.Rows.Count == 0)
                return false;

            var mauColumns = dt.Columns
                .Cast<DataColumn>()
                .Where(c => c.ColumnName.Contains("@Mau@"))
                .ToList();

            // Lọc các dòng khớp với IsNPL = _isNPL
            var filteredRows = dt.Rows
                .Cast<DataRow>()
                .Where(r => Convert.ToBoolean(r["NPL"]) == _isNPL)
                .ToList();

            if (filteredRows.Count == 0)
                return false;

            foreach (DataRow row in filteredRows)
            {
                string maNhomChiTiet = Convert.ToString(row["MaNhomChiTiet"]);
                bool isMaNhomChiTietEmpty = string.IsNullOrWhiteSpace(maNhomChiTiet);

                bool allMauColumnsEmpty = true;
                foreach (var col in mauColumns)
                {
                    string strValue = Convert.ToString(row[col]);
                    if (!string.IsNullOrWhiteSpace(strValue) && strValue != "MAUVT_0")
                    {
                        allMauColumnsEmpty = false;
                        break;
                    }
                }

                if (isMaNhomChiTietEmpty || allMauColumnsEmpty)
                {
                    return false;
                }
            }

            return true;
        }

        private void loadSearchLookUpItemcode()
        {
            try
            {
                string kh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
                string mh = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
                string dot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
                string urlVT = $"{URL}KhoiTaoBOMV1/Get?action=GETITEMCODE_V1&para1={kh.ToString()}&para2={mh.ToString()}&para3={dot.ToString()}";
                string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
                if (jsonVT != "[]")
                {
                    tblItemCode = JsonConvert.DeserializeObject<DataTable>(jsonVT);

                    tblMaVT = tblItemCode
        .AsEnumerable()
        .GroupBy(r => new
        {
            MaVTID = r["MaVTID"].ToString(),
            MaVT = r["MaVT"].ToString(),
            VatTu = r["VatTu"].ToString()
        })
        .Select(g =>
        {
            DataRow row = tblItemCode.Clone().NewRow();
            row["MaVTID"] = g.Key.MaVTID;
            row["MaVT"] = g.Key.MaVT;
            row["VatTu"] = g.Key.VatTu;
            return row;
        })
        .CopyToDataTable();

                    repoSearchItemCode.DisplayMember = "MaVT";
                    repoSearchItemCode.ValueMember = "MaVTID";
                    repoSearchItemCode.DataSource = tblItemCode;
                }
            }
            catch (Exception ex)
            {

            }


        }

        private void btnThemDongNhanh_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable table = gridControl1.DataSource as DataTable;


                if (table == null)
                {
                    table = createTableSaveEdit();
                }


                DataRow newRow = table.NewRow();


                newRow["DinhMucHaoHut"] = 0;
                newRow["DinhMucChung"] = 1;
                newRow["DinhMucKH"] = 1;
                newRow["STT"] = _sttVT;
                newRow["NPL"] = true;
                newRow["MaDVCL"] = "DVVT_7";
                newRow["MaDVDM"] = "DVVT_7";
                _sttVT++;


                table.Rows.Add(newRow);

                gridControl1.DataSource = table;


                this.ActiveControl = button1;
            }
            catch (Exception ex)
            {

            }

        }

        private void btnThemDongNhanhPL_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable table = gridControl1.DataSource as DataTable;

                if (table == null)
                {
                    table = createTableSaveEdit();
                }




                DataRow newRow = table.NewRow();


                newRow["DinhMucHaoHut"] = 0;
                newRow["DinhMucChung"] = 1;
                newRow["DinhMucKH"] = 1;
                newRow["STT"] = _sttVTPL;
                newRow["NPL"] = false;
                newRow["MaDVCL"] = "DVVT_7";
                newRow["MaDVDM"] = "DVVT_7";
                _sttVTPL++;


                table.Rows.Add(newRow);

                gridControl1.DataSource = table;

                this.ActiveControl = button1;
            }
            catch (Exception ex)
            {


            }

        }



        private void gridView2_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();

            if (info.Column.FieldName == "NhomNPL")
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column.FieldName == "TenNhom")
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gridView2.IsGroupRow(e.RowHandle))
            {


                int groupIndex = view.GetRowLevel(e.RowHandle);
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

        private bool isUserSelected = false;
        private bool IsChanged = false;

        private void repoSearchItemCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsChanged) return;
                repoSearchItemCode.QueryCloseUp -= repoSearchItemCode_QueryCloseUp;
                SearchLookUpEdit editor = sender as SearchLookUpEdit;
                if (editor == null) return;

                object value = editor.EditValue;
                if (value == null) return;

                DataRowView drv = editor.Properties.GetRowByKeyValue(value) as DataRowView;
                if (drv == null) return;

                DataRow row = drv.Row;
                if (row != null)
                {
                    DataRow dr = bandedGridView1.GetFocusedDataRow();
                    if (dr == null) return;

                    dr["MaNhom"] = row["MaNhom"].ToString();
                    dr["TenNhom"] = row["TenNhom"].ToString();
                    dr["NPL"] = row["NPL"].ToString();
                    dr["Sort"] = row["Sort"].ToString();
                    dr["MaVTID"] = row["MaVTID"].ToString();
                    dr["MaVT"] = row["MaVT"].ToString();
                    dr["ChiTiet"] = row["VatTu"].ToString();
                    dr["MaDVVT"] = row["MaDVVT"].ToString();
                    dr["TenDVVT"] = row["TenDVVT"].ToString();
                    bool isNL = dr["NPL"].ToString().ToUpper() == "TRUE" ? true : false;

                    //xử lý macode

                    string random6 = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

                    dr["MaCode"] = row["MaVTID"].ToString()
                                     + "|" + random6
                                     + "|" + dr["STT"].ToString();




                    //xử lý chung loại chi tiết tự chọn
                    string maNhomChiTiet = "";


                    if (isNL)
                    {
                        // NPL = true: Tìm theo MaNhom
                        string filterChungLoai = string.Format("MaNhom = '{0}'", row["MaNhom"]);
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
                        string filterChungLoai = string.Format("TenNhomChiTiet = '{0}'", row["TenNhom"].ToString().Replace("'", "''"));
                        DataRow[] foundRows = tblChungLoaiChiTiet.Select(filterChungLoai);

                        // Gán nếu tìm được ít nhất 1 dòng (lấy dòng đầu tiên)
                        if (foundRows.Length > 0)
                        {
                            maNhomChiTiet = foundRows[0]["MaNhomChiTiet"].ToString();
                        }
                    }

                    dr["MaNhomChiTiet"] = maNhomChiTiet;


                    //xử lý màu vật tư
                    dr["MauVTIDChung"] = row["MauVTIDChung"].ToString();
                    string[] arrmauvtid = row["MauVTIDChung"].ToString().Split(',');
                    if (arrmauvtid.Length == 1)
                    {
                        foreach (DataColumn dc in dr.Table.Columns)
                        {
                            if (dc.ColumnName.Contains("@Mau@"))
                            {
                                dr[dc.ColumnName] = arrmauvtid[0];
                            }
                        }
                    }
                    else
                    {
                        foreach (DataColumn dc in dr.Table.Columns)
                        {
                            if (dc.ColumnName.Contains("@Mau@"))
                            {
                                dr[dc.ColumnName] = "";
                            }
                        }

                    }

                    //xử lý khổ vải ID

                    string[] khovaiidarr = row["KhoVaiIDChung"].ToString().Split(',');

                    // Kiểm tra tất cả phần tử có giống nhau không
                    if (khovaiidarr.Length == 1 || khovaiidarr.All(x => x == khovaiidarr[0]))
                    {
                        dr["KhoVaiID"] = khovaiidarr[0];
                    }
                    else
                    {
                        dr["KhoVaiID"] = "";
                    }

                }
                repoSearchItemCode.QueryCloseUp += repoSearchItemCode_QueryCloseUp;
                IsChanged = true;
            }
            catch (Exception ex)
            {


            }

        }



        private void repoSearchItemCode_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue != null && e.NewValue != DBNull.Value)
            {
                isUserSelected = true;
            }
        }

        private void repoSearchItemCode_Popup(object sender, EventArgs e)
        {
            isUserSelected = false;
            IsChanged = false;
        }

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void bandedGridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "DinhMucKH" && isAdd)
            {
                // Lấy giá trị mới của DinhMucKH
                decimal newValue = 0;
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal parsedValue))
                {
                    newValue = parsedValue;
                }

                // Cập nhật cột DinhMucChung cùng dòng
                bandedGridView1.SetRowCellValue(e.RowHandle, "DinhMucChung", newValue);
            }
        }


        private void gridView2_CustomRowFilter(object sender, RowFilterEventArgs e)
        {
            DataRow dr = bandedGridView1.GetFocusedDataRow();
            if (dr == null) return;

            bool isNL = dr["NPL"].ToString().ToUpper() == "TRUE" ? true : false;
            var view = sender as DevExpress.XtraGrid.Views.Base.ColumnView;
            bool rowNPL = Convert.ToBoolean(
                view.GetListSourceRowCellValue(e.ListSourceRow, "NPL")
            );

            if (rowNPL != isNL)
            {
                e.Visible = false;
                e.Handled = true;
            }
        }


        private void bandedGridView1_BeforeLeaveRow(object sender, RowAllowEventArgs e)
        {
            GridView view = sender as GridView;
            if (!view.IsDataRow(view.FocusedRowHandle))
            {
                return;
            }

            // Lấy giá trị KhoVaiID của dòng hiện tại
            string khoVaiID = view.GetFocusedRowCellValue("KhoVaiID")?.ToString() ?? "";
            string maVTID = view.GetFocusedRowCellValue("MaVTID")?.ToString() ?? "";

            if (string.IsNullOrEmpty(khoVaiID) && !string.IsNullOrEmpty(maVTID))
            {
                e.Allow = false;
                XtraMessageBox.Show("Vui lòng chọn Khổ/Size trước khi chuyển sang dòng khác!",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
            }
        }

        private void repoKhoSize_EditValueChanged(object sender, EventArgs e)
        {
            this.ActiveControl = button1;
            DataRow dr = bandedGridView1.GetFocusedDataRow();
            if (dr == null) return;

            string maNhom = dr["MaNhom"].ToString();
            string maVTID = dr["MaVTID"].ToString();
            string khovaiid = dr["KhoVaiID"].ToString();
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETCHECKKHOSIZE&para1={maNhom}&para2={maVTID}&para3={khovaiid}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblMauVTIDNew = JsonConvert.DeserializeObject<DataTable>(json);
            DataRow checkMau = tblMauVTIDNew.AsEnumerable().FirstOrDefault();
            if (checkMau == null) return;
            string mauVTIDChung = checkMau["MauVTIDChung"]?.ToString() ?? "";

            string[] arrmauvtid = checkMau["MauVTIDChung"].ToString().Split(',');
            if (arrmauvtid.Length == 1)
            {
                foreach (DataColumn dc in dr.Table.Columns)
                {
                    if (dc.ColumnName.Contains("@Mau@"))
                    {
                        dr[dc.ColumnName] = arrmauvtid[0];
                    }
                }
            }
            else
            {
                foreach (DataColumn dc in dr.Table.Columns)
                {
                    if (dc.ColumnName.Contains("@Mau@"))
                    {
                        dr[dc.ColumnName] = "";
                    }
                }

            }
            this.ActiveControl = button1;
        }
        private void bandedGridView1_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (e.Column == null) return;

            var repo = e.Column.ColumnEdit as DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit;
            if (repo == null) return;

            bandedGridView1.FocusedColumn = e.Column;
            bandedGridView1.FocusedRowHandle = e.RowHandle;

            bandedGridView1.ShowEditor();

            var editor = bandedGridView1.ActiveEditor as DevExpress.XtraEditors.SearchLookUpEdit;
            if (editor != null)
            {
                editor.ShowPopup();
            }
        }
        #region Sửa ngày 13.01.2026
        private DataRow AddNewRowAt(GridView view, int rowHandle)
        {
            DataTable table = gridControl1.DataSource as DataTable;
            if (table == null) return null;

            DataRow currentRow = null;

            if (rowHandle >= 0)
                currentRow = view.GetDataRow(rowHandle);

            bool isNL = false;
            int newSTT = 1;

            if (currentRow != null)
            {
                isNL = currentRow["NPL"].ToString() == "True";
                newSTT = Convert.ToInt32(currentRow["STT"]) + 1;
            }

            DataRow newRow = table.NewRow();

            newRow["DinhMucHaoHut"] = 0;
            newRow["DinhMucChung"] = 1;
            newRow["STT"] = newSTT;
            newRow["NPL"] = isNL;

            var rowsToUpdate = table.AsEnumerable()
                .Where(r => r["NPL"].ToString() == isNL.ToString()
                         && Convert.ToInt32(r["STT"]) >= newSTT)
                .ToList();

            foreach (DataRow row in rowsToUpdate)
            {
                row["STT"] = Convert.ToInt32(row["STT"]) + 1;
            }

            if (isNL)
                _sttVT++;
            else
                _sttVTPL++;

            int insertIndex = rowHandle >= 0
                ? view.GetDataSourceRowIndex(rowHandle) + 1
                : table.Rows.Count;

            table.Rows.InsertAt(newRow, insertIndex);

            view.RefreshData();

            int newRowHandle = view.GetRowHandle(insertIndex);
            return view.GetDataRow(newRowHandle);
        }



        private void repoSearchItemCode_QueryProcessKey(object sender, DevExpress.XtraEditors.Controls.QueryProcessKeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                // Xử lý khi ấn Enter
                var edit = sender as SearchLookUpEdit;
                if (edit != null && edit.IsPopupOpen)
                {
                    edit.ClosePopup();

                }
            }

        }


        private void repoSearchItemCode_QueryCloseUp(object sender, CancelEventArgs e)
        {

            RepositoryItemSearchLookUpEdit ri = sender as RepositoryItemSearchLookUpEdit;
            if (ri == null)
            {
                SearchLookUpEdit edit = sender as SearchLookUpEdit;
                if (edit != null)
                {
                    ri = edit.Properties as RepositoryItemSearchLookUpEdit;
                }
            }

            if (ri != null && ri.View is GridView view && view.RowCount == 3)
            {
                SearchLookUpEdit editControl = ri.OwnerEdit as SearchLookUpEdit;
                if (editControl != null)
                {
                    // Nếu user CHƯA chọn gì (ấn ESC hoặc click ra ngoài)
                    if (!isUserSelected)
                    {
                        object value = view.GetRowCellValue(0, ri.ValueMember);
                        editControl.EditValue = value;
                        this.ActiveControl = button1;
                    }

                    // Reset flag
                    isUserSelected = false;
                }
            }

        }



        #endregion

        #region Send To Notify
        private void SendNotify(string Title, string SendTo, string BoPhan = "ALL", int Status = -1)
        {
            bool isDuyetAcive = toggleIsActive.IsOn;


            string KhachHang = searchLookUpEditKH.Text == null ? "" : searchLookUpEditKH.Text.ToString();
            string MaHang = searchLookUpEditMH.Text == null ? "" : searchLookUpEditMH.Text.ToString();
            string Dot = searchLookUpEditDot.Text == null ? "" : searchLookUpEditDot.Text.ToString();



            string Detail = $"Đợt: {Dot}\nMã Hàng: {MaHang}\nKhách hàng: {KhachHang}";


            try
            {
                string url = $"{URL}SendToNotification/PushNotificationFrm?" +
                $"UserIDTao={HttpUtility.UrlEncode(GlobleData.UserName)}&" +
                $"FrmName={HttpUtility.UrlEncode("frmPhanTichBOM_TONG")}&" +
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
        private void btnConvertInch_Click(object sender, EventArgs e)
        {
            const double meterToInch = 39.3701;

            var selectedCells = bandedGridView1.GetSelectedCells();

            if (selectedCells == null || selectedCells.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một ô!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var cell in selectedCells)
            {
                var currentValue = bandedGridView1.GetRowCellValue(cell.RowHandle, cell.Column);

                if (currentValue != null && double.TryParse(currentValue.ToString(), out double numericValue))
                {
                    double convertedValue = Math.Round(numericValue * meterToInch, 4);
                    bandedGridView1.SetRowCellValue(cell.RowHandle, cell.Column, convertedValue);
                }
            }
        }


        private bool GetIsXacNhan()
        {
            try
            {
                string makh = searchLookUpEditKH.EditValue?.ToString() ?? "";
                string mahang = searchLookUpEditMH.EditValue?.ToString() ?? "";
                string madot = searchLookUpEditDot.EditValue?.ToString() ?? "";

                string urlGetLenh = $"{URL}ERPDonHangTong/Get?action=GetLenhByDot&para={makh}&para2={mahang}&para3={madot}";
                string jsonLenh = Task.Run(async () => await _clientExtension.GetAsnyc(urlGetLenh)).Result;

                if (string.IsNullOrEmpty(jsonLenh) || jsonLenh == "[]") return false;

                DataTable tblLenh = JsonConvert.DeserializeObject<DataTable>(jsonLenh);
                if (tblLenh == null || tblLenh.Rows.Count == 0) return false;

                foreach (DataRow dr in tblLenh.Rows)
                {
                    string maGop = dr["MaGop"].ToString();
                    string maLenhSanXuat = dr["MaLenhSanXuat"].ToString();
                    string urlXacNhan = $"{URL}ERPDonHangTong/Get?action=GetIsXacNhan&para={maLenhSanXuat}&para2={maGop}";
                    string result = Task.Run(async () => await _clientExtension.GetAsnyc(urlXacNhan)).Result;
                    if (string.IsNullOrEmpty(result) || result == "[]") return false;
                    DataTable tblXacNhan = JsonConvert.DeserializeObject<DataTable>(result);
                    if (tblXacNhan == null || tblXacNhan.Rows.Count == 0) return false;
                    int isXacNhan = Convert.ToInt32(tblXacNhan.Rows[0]["IsXacNhan"]);
                    if (isXacNhan != 1) return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void UpdateNguyenLieu()
        {
            try
            {
                string makh = searchLookUpEditKH.EditValue?.ToString() ?? "";
                string mahang = searchLookUpEditMH.EditValue?.ToString() ?? "";
                string madot = searchLookUpEditDot.EditValue?.ToString() ?? "";
                string urlGetLenh = $"{URL}ERPDonHangTong/Get?action=GetLenhByDot&para={makh}&para2={mahang}&para3={madot}";
                string jsonLenh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGetLenh); }).Result;

                if (jsonLenh == "[]") return;

                DataTable tblLenh = JsonConvert.DeserializeObject<DataTable>(jsonLenh);
                if (tblLenh == null || tblLenh.Rows.Count == 0) return;
                foreach (DataRow dr in tblLenh.Rows)
                {
                    string maGop = dr["MaGop"].ToString();
                    string maLenhSanXuat = dr["MaLenhSanXuat"].ToString();
                    string urlUpdate = $"{URL}ERPDonHangTong/Get?action=UpdateVTXNNguyenLieu&para={maGop}&para2={makh}&para3={mahang}&para4={madot}&para5={maLenhSanXuat}&para6={GlobleData.UserName}";
                    string result = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlUpdate); }).Result;
                }
            }
            catch (Exception ex)
            { }
        }
        private void DoBOMHangLoat()
        {
            try
            {
                string makh = searchLookUpEditKH.EditValue?.ToString() ?? "";
                string mahang = searchLookUpEditMH.EditValue?.ToString() ?? "";
                string urlUpdate = $"{URL}ERPDonHangTong/Get?action=CapNhatBOMHangLoat&para=&para2={makh}&para3={mahang}&para4=&para5=&para6={GlobleData.UserName}";
                string result = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlUpdate); }).Result;
            }
            catch (Exception ex)
            { }
        }
        private void UpdatePhuLieu()
        {
            try
            {
                string makh = searchLookUpEditKH.EditValue?.ToString() ?? "";
                string mahang = searchLookUpEditMH.EditValue?.ToString() ?? "";
                string madot = searchLookUpEditDot.EditValue?.ToString() ?? "";
                string urlGetLenh = $"{URL}ERPDonHangTong/Get?action=GetLenhByDot&para={makh}&para2={mahang}&para3={madot}";
                string jsonLenh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlGetLenh); }).Result;

                if (jsonLenh == "[]") return;

                DataTable tblLenh = JsonConvert.DeserializeObject<DataTable>(jsonLenh);
                if (tblLenh == null || tblLenh.Rows.Count == 0) return;
                foreach (DataRow dr in tblLenh.Rows)
                {
                    string maGop = dr["MaGop"].ToString();
                    string maLenhSanXuat = dr["MaLenhSanXuat"].ToString();
                    string urlUpdate = $"{URL}ERPDonHangTong/Get?action=UpdateVTXNPhuLieu&para={maGop}&para2={makh}&para3={mahang}&para4={madot}&para5={maLenhSanXuat}&para6={GlobleData.UserName}";
                    string result = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlUpdate); }).Result;
                }
            }
            catch (Exception ex)
            { }
        }
        private void loadDVChungLoai()
        {
            try
            {
                repoDVCL.DisplayMember = "TenDVVT";
                repoDVCL.ValueMember = "MaDVVT";
                repoDVDM.DisplayMember = "TenDVVT";
                repoDVDM.ValueMember = "MaDVVT";

                searchLookUpEditDVDM.Properties.DisplayMember = "TenDVVT";
                searchLookUpEditDVDM.Properties.ValueMember = "MaDVVT";
                string url = $"{URL}KhoiTaoBOMV1/Get?action=GETDONVICL";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                tblDV = JsonConvert.DeserializeObject<DataTable>(json);
                repoDVCL.DataSource = tblDV;
                repoDVDM.DataSource = tblDV;
                searchLookUpEditDVDM.Properties.DataSource = tblDV;

            }
            catch (Exception ex)
            {
            }
        }
        private void repoDVDM_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit editor = sender as SearchLookUpEdit;
            if (editor == null) return;

            object value = editor.EditValue;
            if (value == null) return;

            DataRowView drv = editor.Properties.GetRowByKeyValue(value) as DataRowView;
            if (drv == null) return;

            DataRow row = drv.Row;
            if (row != null)
            {
                decimal tile = 0;
                if (row["Tile"] != DBNull.Value)
                    tile = Convert.ToDecimal(row["Tile"]);

                if (tile == 0) return;
                DataRow dr = bandedGridView1.GetFocusedDataRow();
                if (dr == null) return;

                decimal dinhMucChung = 0;
                if (dr["DinhMucChung"] != DBNull.Value)
                    dinhMucChung = Convert.ToDecimal(dr["DinhMucChung"]);
                decimal ketQua = Math.Round(dinhMucChung * tile, 4);
                dr["DinhMucChung"] = ketQua;





                decimal dinhMucKH = 0;
                if (dr["DinhMucKH"] != DBNull.Value)
                    dinhMucKH = Convert.ToDecimal(dr["DinhMucKH"]);
                decimal ketQua2 = Math.Round(dinhMucKH * tile, 4);
                dr["DinhMucKH"] = ketQua2;
            }
        }
        private void btnConvertM_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < bandedGridView1.RowCount; i++)
            {

                var maDVVT = bandedGridView1.GetRowCellValue(i, "MaDVVT")?.ToString();

                if (string.IsNullOrEmpty(maDVVT)) continue;


                DataRow[] rows = tblDV.Select($"MaDVVT = '{maDVVT}'");

                if (rows.Length == 0) continue;

                if (!double.TryParse(rows[0]["Tile"].ToString(), out double tile)) continue;

                foreach (DevExpress.XtraGrid.Columns.GridColumn col in bandedGridView1.Columns)
                {
                    var currentValue = bandedGridView1.GetRowCellValue(i, col);
                    if (currentValue != null && double.TryParse(currentValue.ToString(), out double numericValue))
                    {
                        double convertedValue = Math.Round(numericValue * tile, 4);
                        bandedGridView1.SetRowCellValue(i, col, convertedValue);
                    }
                }
            }
        }
        private void searchLookUpEditDVDM_EditValueChanged(object sender, EventArgs e)
        {
            int[] selectedRowHandles = bandedGridView1.GetSelectedRows();
            if (selectedRowHandles.Length == 0) return;

            foreach (int rowHandle in selectedRowHandles)
            {
                if (rowHandle < 0) continue;
                if (!bandedGridView1.IsValidRowHandle(rowHandle)) continue;
                DataRow drSL = bandedGridView1.GetDataRow(rowHandle);
                if (drSL != null)
                    drSL["MaDVDM"] = searchLookUpEditDVDM.EditValue.ToString();
                bandedGridView1.RefreshRow(rowHandle);
                bandedGridView1_CellValueChanged(bandedGridView1, new DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs(rowHandle, bandedGridView1.Columns["MaDVDM"], drSL["MaDVDM"]));

            }
            this.ActiveControl = button1;
        }
        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Xác nhận PL
            if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                return;
            if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                return;
            string url = string.Format("{0}?username={1}", URL + "KhoiTaoDM/KiemTraDuyet", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                XtraMessageBox.Show("User không có quyền Xác nhận BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }

            DuyetPL(1);
            if (!GetIsXacNhan())
                UpdatePhuLieu();

        }
        private void btnHuyXNPL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Xác nhận PL
            if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                return;
            if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                return;
            string url = string.Format("{0}?username={1}", URL + "KhoiTaoDM/KiemTraDuyet", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                XtraMessageBox.Show("User không có quyền Xác nhận BOM. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }

            DuyetPL(0);
            //if (!GetIsXacNhan())
            //    UpdateVTXN();
            //string Title = "BOM NPL cần soát xét";
            //SendNotify(Title, "PKT", "ALL", 1);
        }

        private void DuyetPL(int isduyet)
        {
            try
            {
                if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                    return;
                if (searchLookUpEditMH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditMH.EditValue.ToString()))
                    return;

                if (isduyet == 0 && toggleIsActive.IsOn)
                {

                    XtraMessageBox.Show("Đợt này đang được Active. Vui lòng kiểm tra lại!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                    return;
                }
                DataTable tbl = gridControl1.DataSource as DataTable;

                var filteredRows = tbl.Rows
              .Cast<DataRow>()
              .Where(r => Convert.ToBoolean(r["NPL"]) == false)
              .ToList();

                if (filteredRows.Count == 0)
                    return;
                if (isduyet == 1)
                {
                    if (!KiemTraDuLieuTuDataTable(tbl, false))
                    {
                        XtraMessageBox.Show(
                            "Có dòng không hợp lệ (dòng đc tô màu).\nVui lòng kiểm tra lại!",
                            Properties.Resources.InfoTitle,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                }
                string _makh = searchLookUpEditKH.EditValue.ToString();
                string _mahang = searchLookUpEditMH.EditValue.ToString();
                string _madot = searchLookUpEditDot.EditValue.ToString();

                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang xử lý yêu cầu");
                string url = $"{URL}KhoiTaoBOMV1/Post1?Action=XacNhan&para1={_makh}&para2={_mahang}&para3={_madot}&para4={isduyet}&para5={GlobleData.UserName}&para6=0";
                string json = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
                if (json == "True")
                {
                    string title = "BOM PL đã bị hủy xác nhận " + searchLookUpEditDot.Text.ToString();
                    if (isduyet == 1)
                    {
                        title = "BOM PL đã xác nhận " + searchLookUpEditDot.Text.ToString();
                    }

                    SendNotify(title, "PKT", "ALL", 1);

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
        private void CancelBOM(bool IsCancel)
        {
            string makh = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            if (madot.ToString() == "") return;
            int cancel = IsCancel ? 2 : 0;

            string url = $"{URL}KhoiTaoBOMV1/Post1?action=CancelBOM&para1={cancel}&para2={makh}&para3={mahang}&para4={madot}&para5={GlobleData.UserName}";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
            if (msResult.ToLower() != "true") return;

            if (cancel == 2)
            {

                frmPhanTichBOMViewLenh frm = new frmPhanTichBOMViewLenh(makh, mahang, madot);
                if (frm.ShowDialog() != DialogResult.OK) return;

                DataTable selectedLenh = frm.SelectedTable;
                if (selectedLenh == null || selectedLenh.Rows.Count == 0) return;

                DataTable dtChuyenDot = CreateTypeCanDoiDonViSanXuat();

                foreach (DataRow dr in selectedLenh.Rows)
                {
                    DataRow newRow = dtChuyenDot.NewRow();
                    newRow["ID"] = DBNull.Value;
                    newRow["MaLenhSanXuat"] = dr["MaLenhSanXuat"]?.ToString() ?? "";
                    newRow["MaLenh"] = dr["MaLenh"]?.ToString() ?? "";
                    newRow["TenLenh"] = DBNull.Value;
                    newRow["MaDH"] = DBNull.Value;
                    newRow["MaDVSX"] = DBNull.Value;
                    newRow["DotSX"] = DBNull.Value;
                    newRow["MaQG"] = DBNull.Value;
                    newRow["POID"] = DBNull.Value;
                    newRow["PO"] = DBNull.Value;
                    newRow["MaMau"] = DBNull.Value;
                    newRow["DauSizeID"] = DBNull.Value;
                    newRow["DauSize"] = DBNull.Value;
                    newRow["SizeID"] = DBNull.Value;
                    newRow["Size"] = DBNull.Value;
                    newRow["SoLuong"] = DBNull.Value;
                    newRow["TrangThai"] = DBNull.Value;
                    newRow["STTLenh"] = DBNull.Value;
                    newRow["MaGop"] = dr["MaGop"]?.ToString() ?? "";
                    newRow["POID_T"] = DBNull.Value;
                    newRow["MaCu"] = DBNull.Value;
                    newRow["Line"] = DBNull.Value;
                    newRow["GhiChu"] = DBNull.Value;
                    dtChuyenDot.Rows.Add(newRow);
                }
                string urlUpdate = string.Format("{0}", URL + $"ERPDonHangTong/PostV1?action=ChuyenDot&type=@TypeTable&para={makh}&para2={mahang}&para3={madot}&para4={GlobleData.UserName}");
                var msUpdate = Task.Run(async () => { return await _clientExtension.PostAsync(urlUpdate, dtChuyenDot); }).Result;
                if (msUpdate.ToLower() != "true") return;

                string _Module = "PhanTichBOM";
                string _Action = "Hủy đợt BOM";
                string _Contents = makh + "@" + mahang + "@" + madot;
                string urls = $"{URL}CanDoiDinhMucNPLLog/GhiLogLenh?action=GhiLogLenh&para={_Action}&para1={_Module}&para2={_Contents}&para3={GlobleData.UserName}";
                string jsons = Task.Run(async () => await _clientExtension.GetAsnyc(urls)).Result;
            }
            else
            {
                string _Module = "PhanTichBOM";
                string _Action = "Bỏ hủy đợt BOM";
                string _Contents = makh + "@" + mahang + "@" + madot;
                string urls = $"{URL}CanDoiDinhMucNPLLog/GhiLogLenh?action=GhiLogLenh&para={_Action}&para1={_Module}&para2={_Contents}&para3={GlobleData.UserName}";
                string jsons = Task.Run(async () => await _clientExtension.GetAsnyc(urls)).Result;
            }

            clsWaitForm.ShowSuccessForm(this, 3000);
            NapLai(true);
        }

        private DataTable CreateTypeCanDoiDonViSanXuat()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("MaLenhSanXuat", typeof(string));
            dt.Columns.Add("MaLenh", typeof(string));
            dt.Columns.Add("TenLenh", typeof(string));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("MaDVSX", typeof(string));
            dt.Columns.Add("DotSX", typeof(string));
            dt.Columns.Add("MaQG", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("PO", typeof(string));
            dt.Columns.Add("MaMau", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("DauSize", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("Size", typeof(string));
            dt.Columns.Add("SoLuong", typeof(int));
            dt.Columns.Add("TrangThai", typeof(int));
            dt.Columns.Add("STTLenh", typeof(int));
            dt.Columns.Add("MaGop", typeof(string));
            dt.Columns.Add("POID_T", typeof(string));
            dt.Columns.Add("MaCu", typeof(string));
            dt.Columns.Add("Line", typeof(string));
            dt.Columns.Add("GhiChu", typeof(string));
            return dt;
        }
        //Xử lý nút hủy
        private void toggleIsHuy_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            bool isOn = Convert.ToBoolean(e.NewValue);
            if (isOn)
            {
                DialogResult result = XtraMessageBox.Show(
                    "Bạn có chắc chắn muốn hủy đợt này không?" +
                    "\n Lưu ý:Người dùng hoàn toàn chịu trách nhiệm về thao tác này!",
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
        private void toggleIsHuy_Toggled(object sender, EventArgs e)
        {
            toggleIsHuy.Properties.Appearance.ForeColor = toggleIsHuy.IsOn ? Color.Red : Color.DimGray;
            bool isCancel = toggleIsHuy.IsOn;
            //string title = isCancel ? "BOM NPL đã ban hành" : "BOM NPL đã hủy ban hành";
            //SendNotify(title, "PKH");
            CancelBOM(isCancel);
        }
    }
}
