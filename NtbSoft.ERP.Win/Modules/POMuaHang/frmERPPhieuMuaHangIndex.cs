using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Web.Api.ThuVien;
using NtbSoft.ERP.Win.Modules.ThuVien;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Spreadsheet;
using DevExpress.XtraPrinting;
using System.Drawing.Printing;
using DevExpress.XtraGrid;
using System.Diagnostics;
using System.Net.Http;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Controls;
using DevExpress.Pdf;
using System.Web;
using DevExpress.Utils.Menu;

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmERPPhieuMuaHangIndex : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        List<string> lstFormatFieldName = new List<string> { "ThanhTien", "SoLuongMuaThem", "DonGia" };
        private DataTable gridControl1Table;
        private DataTable vattuTable;
        decimal _gia;
        decimal _tygia;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        private const int CHECK_SIZE = 16;
        private const int CHECK_LEFT = 6;
        private DataTable _tblDetails;
        private DataTable _tblDetailsMMTB;
        Dictionary<string, string> _dicNhanVien;
        private bool isTab1Loaded = false;
        private bool isTab2Loaded = false;
        private bool isTab3Loaded = false;
        private string _focusMaPhieuMH = null;
        private DataTable _tblQCKiemCache;
        private DataTable _tblDetailsPYC;
        private Dictionary<string, string> tenNVHienThi = new Dictionary<string, string>();
        private bool _isMua = false;
        int pageIndex = 1;
        int pageSize = 20;

        public frmERPPhieuMuaHangIndex()
        {
            InitializeComponent();
            InitMasterDetail();
            InitMasterDetailMMTB();
            InitMasterDetailYC();
            

            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CheckPerminsion();

           
        }

        private void InitMasterDetail()
        {


            grcPhieuMHVatTu.ViewCollection.Add(gridView3);

            // 2. Tạo LevelNode
            GridLevelNode levelNode = new GridLevelNode();
            levelNode.RelationName = "CTVatTu";      // TÊN TÙY Ý – NHƯNG PHẢI TRÙNG EVENT
            levelNode.LevelTemplate = gridView3;

            grcPhieuMHVatTu.LevelTree.Nodes.Add(levelNode);

            // 3. Set option cho master
            grvPhieuMHVatTu.OptionsDetail.EnableMasterViewMode = true;
            grvPhieuMHVatTu.OptionsDetail.ShowDetailTabs = false;
            grvPhieuMHVatTu.OptionsDetail.AllowExpandEmptyDetails = true;

        }
        private void InitMasterDetailMMTB()
        {
            gridControl2.ViewCollection.Add(gridView4);

            // 2. Tạo LevelNode
            GridLevelNode levelNode = new GridLevelNode();
            levelNode.RelationName = "CTMMTB";
            levelNode.LevelTemplate = gridView4;

            gridControl2.LevelTree.Nodes.Add(levelNode);

            // 3. Set option cho master
            gridView5.OptionsDetail.EnableMasterViewMode = true;
            gridView5.OptionsDetail.ShowDetailTabs = false;
            gridView5.OptionsDetail.AllowExpandEmptyDetails = true;

        }
        private void InitMasterDetailYC()
        {
            gridControl3.ViewCollection.Add(gridView7);

            GridLevelNode levelNode = new GridLevelNode();
            levelNode.RelationName = "CTYC";
            levelNode.LevelTemplate = gridView7;

            gridControl3.LevelTree.Nodes.Add(levelNode);

            gridView6.OptionsDetail.EnableMasterViewMode = true;
            gridView6.OptionsDetail.ShowDetailTabs = false;
            gridView6.OptionsDetail.AllowExpandEmptyDetails = true;
        }
        protected override void OnLoad(EventArgs e)
        {
            LoadNhanVien();
            //LoadPhieuMuaHang();


            //grvPhieuMHVatTu.ColumnPanelRowHeight = 50;


            //grvPhieuMHVatTu.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            toDate.EditValue = DateTime.Now.ToString("dd/MM/yyyy");
            fromDate.EditValue = DateTime.Now.ToString("dd/MM/yyyy");

            tenNVHienThi = GetTenNhanVien();
            //VatTuCanMua

            //getVatTuCanMua();
            //calcVatTuCanMua();
            //loadGridControl1();
            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Maximized;
            barStaticItemPageIndex.EditValue = "1";
            LoadDataForCurrentTab();

        }

        private void LoadDataForCurrentTab()
        {
            // Giả sử xtraTabPage1 là tab đầu tiên, xtraTabPage2 là tab thứ hai
            if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
            {
                if (!isTab1Loaded)
                {
                    SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
                    //LoadNhanVien();
                    
                    try
                    {
                        LoadPhieuMuaHang();
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(ex.Message, "Lỗi",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        SplashScreenManager.CloseDefaultWaitForm();
                    }
                    finally
                    {
                        SplashScreenManager.CloseDefaultWaitForm();
                    }
                    //isTab1Loaded = true;
                }
            }
            else if (xtraTabControl1.SelectedTabPage == xtraTabPage2)
            {
                if (!isTab2Loaded)
                {
                    SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
                    try
                    {
                        getVatTuCanMua();
                        calcVatTuCanMua();
                        loadGridControl1();
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(ex.Message, "Lỗi",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        SplashScreenManager.CloseDefaultWaitForm();
                    }
                    finally
                    {
                        SplashScreenManager.CloseDefaultWaitForm();
                    }
                    //isTab2Loaded = true;
                }
            }
            else if (xtraTabControl1.SelectedTabPage == xtraTabPage3)
            {
                if (!isTab3Loaded)
                {
                    LoadPhieuYCMuaHangMMTB();

                    //isTab3Loaded = true;
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
                barSubItem1.Enabled = false;
                //barButtonItem7.Enabled = false;
            }
            if (!_allowEdit)
            {
                barButtonItem4.Enabled = false;
            }
            if (!_allowDelete)
            {
                barButtonItem5.Enabled = false;
                //barButtonItem10.Enabled = false;
            }

        }

        private void LoadNhanVien()
        {
            string urlNV = $"{URL}NhanVien/GetAllNV";
            string jsonNV = Task.Run(async () => await _clientExtension.GetAsnyc(urlNV)).Result;

            DataTable tblnv = JsonConvert.DeserializeObject<DataTable>(jsonNV);

            _dicNhanVien = tblnv.AsEnumerable()
                .Where(r => !string.IsNullOrEmpty(r.Field<string>("UserID")))
                .GroupBy(r => r.Field<string>("UserID").Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.First().Field<string>("Ten")
                );
        }

        private void LoadPhieuMuaHang()
        {
            try
            {
                //DateTime FromDate = clsForrmatUtils.ConvertDate(fromDate.EditValue);
                //DateTime ToDate = clsForrmatUtils.ConvertDate(toDate.EditValue);
                //Phieu MuaHang
                DataTable tblPhieuMuaHangVatTu = new DataTable();
                string url = $"{URL}ERPCanDoiNguyenPhuLieu/GET?action=GetPhieuMuaHang&para1={pageIndex}&para2={pageSize}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                string urldt = $"{URL}NhaCC/GePMH?action=GETPOALLDETAILS&para1=&para2=&para3=&para4=&para5=";
                string jsondt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldt); }).Result;
                _tblDetails = JsonConvert.DeserializeObject<DataTable>(jsondt);

                if (string.IsNullOrWhiteSpace(json) || json.Trim() == "[]") return;

                tblPhieuMuaHangVatTu = JsonConvert.DeserializeObject<DataTable>(json);

                if (!tblPhieuMuaHangVatTu.Columns.Contains("TongTien"))
                    tblPhieuMuaHangVatTu.Columns.Add("TongTien", typeof(decimal));

                if (!tblPhieuMuaHangVatTu.Columns.Contains("TongTienVND"))
                    tblPhieuMuaHangVatTu.Columns.Add("TongTienVND", typeof(decimal));

                if (!tblPhieuMuaHangVatTu.Columns.Contains("KetQuaKiem"))
                    tblPhieuMuaHangVatTu.Columns.Add("KetQuaKiem", typeof(string));

                //if (!tblPhieuMuaHangVatTu.Columns.Contains("KNL"))
                //    tblPhieuMuaHangVatTu.Columns.Add("KNL", typeof(string));

                //if (!tblPhieuMuaHangVatTu.Columns.Contains("KPL"))
                //    tblPhieuMuaHangVatTu.Columns.Add("KPL", typeof(string));

                if (!tblPhieuMuaHangVatTu.Columns.Contains("SoLoID"))
                    tblPhieuMuaHangVatTu.Columns.Add("SoLoID", typeof(string));

                if (!tblPhieuMuaHangVatTu.Columns.Contains("SoLo"))
                    tblPhieuMuaHangVatTu.Columns.Add("SoLo", typeof(string));

                if (!_tblDetails.Columns.Contains("KiemVT"))
                    _tblDetails.Columns.Add("KiemVT", typeof(string));

                if (!_tblDetails.Columns.Contains("SoLoID"))
                    _tblDetails.Columns.Add("SoLoID", typeof(string));

                if (!_tblDetails.Columns.Contains("SoLo"))
                    _tblDetails.Columns.Add("SoLo", typeof(string));

                if (!_tblDetails.Columns.Contains("GhiChuQC"))
                    _tblDetails.Columns.Add("GhiChuQC", typeof(string));

                if (!_tblDetails.Columns.Contains("MaNPL"))
                    _tblDetails.Columns.Add("MaNPL", typeof(string));

                if (!_tblDetails.Columns.Contains("qcShipment"))
                    _tblDetails.Columns.Add("qcShipment", typeof(string));

                if (!_tblDetails.Columns.Contains("NgayNK"))
                    _tblDetails.Columns.Add("NgayNK", typeof(string));
                //QCKiemVai
                DataTable tblqckiem = new DataTable();
                string urlqc = $"{URL}ERPCanDoiNguyenPhuLieu/GET?action=GetQCKIEM&para1=&para2=";
                string jsonqc = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlqc); }).Result;
                tblqckiem = JsonConvert.DeserializeObject<DataTable>(jsonqc);
                if (jsonqc != "[]")
                    tblqckiem = JsonConvert.DeserializeObject<DataTable>(jsonqc);

                var qcLookup = tblqckiem.AsEnumerable()
                   .GroupBy(x => x.Field<string>("MaPhieuMH"))
                   .ToDictionary(
                       g => g.Key,
                       g =>
                       {
                           bool allNull = g.All(x => x["KetQuaKiem"] == DBNull.Value);

                           bool hasNull = g.Any(x => x["KetQuaKiem"] == DBNull.Value);

                           bool hasFail = g.Any(x =>
                               x["KetQuaKiem"] != DBNull.Value &&
                               Convert.ToInt32(x["KetQuaKiem"]) == 0
                           );

                           bool allPass = g.All(x =>
                               x["KetQuaKiem"] != DBNull.Value &&
                               Convert.ToInt32(x["KetQuaKiem"]) == 1
                           );

                           string ketQuaKiem;
                           if (allNull)
                               ketQuaKiem = "";
                           else if (hasFail)
                               ketQuaKiem = "FAIL";
                           else if (allPass)
                               ketQuaKiem = "PASS";
                           else
                               ketQuaKiem = "Đang Kiểm";

                           string soloid = g.First()["SoLoID"]?.ToString() ?? "";
                           string solo = g.First()["SoLo"]?.ToString() ?? "";

                           return new
                           {
                               KetQuaKiem = ketQuaKiem,
                               SoLoID = soloid,
                               SoLo = solo
                           };
                       }
                   );

                foreach (DataRow r in tblPhieuMuaHangVatTu.Rows)
                {
                    //decimal tongCPPS = r["TongCPPS"] == DBNull.Value ? 0 : Convert.ToDecimal(r["TongCPPS"]);
                    //decimal tongCPVC = r["TongCPVC"] == DBNull.Value ? 0 : Convert.ToDecimal(r["TongCPVC"]);
                    //decimal tongCPVT = r["TongCPVT"] == DBNull.Value ? 0 : Convert.ToDecimal(r["TongCPVT"]);
                    string maPhieu = r["MaPhieuMH"]?.ToString();
                    string maTienTePhieu = r["MaTienTe"]?.ToString() ?? "VND";

                    decimal tongCPPS = GetTongCPPS_ByMaPhieu(maPhieu, maTienTePhieu);
                    decimal tongCPVC = GetTongCPVC_ByMaPhieu(maPhieu, maTienTePhieu);
                    decimal tongCPVT = GetTongCPVT_ByMaPhieu(maPhieu, maTienTePhieu);
                    r["TongCPPS"] = tongCPPS;
                    r["TongCPVC"] = tongCPVC;
                    r["TongCPVT"] = tongCPVT;
                    decimal tongTien = tongCPPS + tongCPVC + tongCPVT;
                    r["TongTien"] = tongTien;

                    if (!string.IsNullOrEmpty(maPhieu) && qcLookup.TryGetValue(maPhieu, out var qc))
                    {
                        r["KetQuaKiem"] = qc.KetQuaKiem;
                        //r["KPL"] = qcLookup[maPhieu].KPL;
                        //r["KNL"] = qcLookup[maPhieu].KNL;
                        r["SoLoID"] = qc.SoLoID;
                        r["SoLo"] = qc.SoLo;
                    }
                    else
                    {
                        r["KetQuaKiem"] = DBNull.Value;
                        //r["KPL"] = DBNull.Value;
                        //r["KNL"] = DBNull.Value;
                        r["SoLoID"] = DBNull.Value;
                        r["SoLo"] = DBNull.Value;
                    }

                    string mancc = r["MaNCC"]?.ToString() ?? "";

                    string maTienTe = r["MaTienTe"]?.ToString() ?? "VND";

                    decimal gia = 1;

                    if (!string.IsNullOrEmpty(mancc) && maTienTe != "VND")
                    {
                        string url1 = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={maTienTe}&para2={"VND"}&para3=&para4=&para5=";

                        string json1 = Task.Run(async () => await _clientExtension.GetAsnyc(url1)).Result;

                        DataTable tblTyGia = JsonConvert.DeserializeObject<DataTable>(json1);

                        if (tblTyGia != null && tblTyGia.Rows.Count > 0)
                        {
                            gia = Convert.ToDecimal(tblTyGia.Rows[0]["TyGia"]);
                        }

                    }
                    _gia = gia;
                    _tygia = gia;
                    // 6. Tính TongTienVND
                    if (maTienTe == "VND")
                        r["TongTienVND"] = tongTien;
                    else
                        r["TongTienVND"] = tongTien * gia;
                }

                grcPhieuMHVatTu.DataSource = tblPhieuMuaHangVatTu;
                grcPhieuMHVatTu.RefreshDataSource();
                RestoreFocusRow();

            }
            catch (Exception ex)
            {

            }
        }



        private decimal GetTongCPVT_ByMaPhieu(string maPhieu, string maTienTePhieu)
        {
            //if (string.IsNullOrEmpty(maPhieu)) return 0;

            //string url = $"{URL}NhaCC/GePMH?action=GETPMHChiTiet&para1={maPhieu}&para2=&para3=&para4=&para5=";
            //string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            //if (json == "[]" || string.IsNullOrEmpty(json))
            //    return 0;

            //DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            //if (dt == null || dt.Rows.Count == 0 || !dt.Columns.Contains("ChiPhiVT"))
            //    return 0;

            //return dt.AsEnumerable()
            //         .Where(r => r["ChiPhiVT"] != DBNull.Value)
            //         .Sum(r => Convert.ToDecimal(r["ChiPhiVT"]));

            if (string.IsNullOrEmpty(maPhieu)) return 0;

            string url = $"{URL}NhaCC/GePMH?action=GETPMHChiTiet&para1={maPhieu}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            if (json == "[]" || string.IsNullOrEmpty(json))
                return 0;

            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt == null || dt.Rows.Count == 0)
                return 0;

            // 🔹 Tỷ giá của TIỀN PHIẾU → VND (lấy 1 lần)
            decimal tyGiaPhieu = GetTyGia_ToVND(maTienTePhieu);

            decimal tongCPVT = 0;

            foreach (DataRow r in dt.Rows)
            {

                decimal chiPhiDong = r["ThanhTien"] == DBNull.Value ? 0 : Convert.ToDecimal(r["ThanhTien"]);
                if (chiPhiDong == 0) continue;

                string tienTeDong = r["TienTeID"]?.ToString() ?? "VND";

                // 🔹 Tỷ giá của DÒNG → VND
                decimal tyGiaDong = GetTyGia_ToVND(tienTeDong);

                // 🔹 Quy đổi: Dòng → VND → Tiền phiếu
                decimal chiPhiTheoPhieu = (chiPhiDong * tyGiaDong) / tyGiaPhieu;

                tongCPVT += chiPhiTheoPhieu;
            }

            return Math.Round(tongCPVT, 2);
        }
        private decimal GetTongCPPS_ByMaPhieu(string maPhieu, string maTienTePhieu)
        {
            //if (string.IsNullOrEmpty(maPhieu)) return 0;

            //string url = $"{URL}NhaCC/GePMH?action=GETPMHChiTiet&para1={maPhieu}&para2=&para3=&para4=&para5=";
            //string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            //if (json == "[]" || string.IsNullOrEmpty(json))
            //    return 0;

            //DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            //if (dt == null || dt.Rows.Count == 0 || !dt.Columns.Contains("ChiPhiVT"))
            //    return 0;

            //return dt.AsEnumerable()
            //         .Where(r => r["ChiPhiVT"] != DBNull.Value)
            //         .Sum(r => Convert.ToDecimal(r["ChiPhiVT"]));

            if (string.IsNullOrEmpty(maPhieu)) return 0;

            string url = $"{URL}NhaCC/GetCP?action=GETPMHChiPhiTong&para1={maPhieu}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            if (json == "[]" || string.IsNullOrEmpty(json))
                return 0;

            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt == null || dt.Rows.Count == 0)
                return 0;

            // 🔹 Tỷ giá của TIỀN PHIẾU → VND (lấy 1 lần)
            decimal tyGiaPhieu = GetTyGia_ToVND(maTienTePhieu);

            decimal tongCPPS = 0;

            foreach (DataRow r in dt.Rows)
            {
                decimal chiPhiDong = r["ThanhTienCPPS"] == DBNull.Value ? 0 : Convert.ToDecimal(r["ThanhTienCPPS"]);
                if (chiPhiDong == 0) continue;

                string tienTeDong = r["DonViTienTe"]?.ToString() ?? "VND";

                // 🔹 Tỷ giá của DÒNG → VND
                decimal tyGiaDong = GetTyGia_ToVND(tienTeDong);

                // 🔹 Quy đổi: Dòng → VND → Tiền phiếu
                decimal chiPhiTheoPhieu = (chiPhiDong * tyGiaDong) / tyGiaPhieu;

                tongCPPS += chiPhiTheoPhieu;
            }

            return Math.Round(tongCPPS, 2);
        }
        private decimal GetTongCPVC_ByMaPhieu(string maPhieu, string maTienTePhieu)
        {
            //if (string.IsNullOrEmpty(maPhieu)) return 0;

            //string url = $"{URL}NhaCC/GePMH?action=GETPMHChiTiet&para1={maPhieu}&para2=&para3=&para4=&para5=";
            //string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            //if (json == "[]" || string.IsNullOrEmpty(json))
            //    return 0;

            //DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            //if (dt == null || dt.Rows.Count == 0 || !dt.Columns.Contains("ChiPhiVT"))
            //    return 0;

            //return dt.AsEnumerable()
            //         .Where(r => r["ChiPhiVT"] != DBNull.Value)
            //         .Sum(r => Convert.ToDecimal(r["ChiPhiVT"]));

            if (string.IsNullOrEmpty(maPhieu)) return 0;
            string url = $"{URL}NhaCC/GetCP?action=GETPMHCPVC&para1={maPhieu}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            if (json == "[]" || string.IsNullOrEmpty(json))
                return 0;

            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt == null || dt.Rows.Count == 0)
                return 0;

            // 🔹 Tỷ giá của TIỀN PHIẾU → VND (lấy 1 lần)
            decimal tyGiaPhieu = GetTyGia_ToVND(maTienTePhieu);

            decimal tongCPVC = 0;

            foreach (DataRow r in dt.Rows)
            {
                decimal chiPhiDong = r["ThanhTienCPPS"] == DBNull.Value ? 0 : Convert.ToDecimal(r["ThanhTienCPPS"]);
                if (chiPhiDong == 0) continue;

                string tienTeDong = r["DonViTienTe"]?.ToString() ?? "VND";

                // 🔹 Tỷ giá của DÒNG → VND
                decimal tyGiaDong = GetTyGia_ToVND(tienTeDong);

                // 🔹 Quy đổi: Dòng → VND → Tiền phiếu
                decimal chiPhiTheoPhieu = (chiPhiDong * tyGiaDong) / tyGiaPhieu;

                tongCPVC += chiPhiTheoPhieu;
            }

            return Math.Round(tongCPVC, 2);
        }

        private decimal GetTongCPMMTB_ByMaPhieu(string maPhieu, string maTienTePhieu)
        {
            //if (string.IsNullOrEmpty(maPhieu)) return 0;

            //string url = $"{URL}NhaCC/GePMH?action=GETPMHChiTiet&para1={maPhieu}&para2=&para3=&para4=&para5=";
            //string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            //if (json == "[]" || string.IsNullOrEmpty(json))
            //    return 0;

            //DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            //if (dt == null || dt.Rows.Count == 0 || !dt.Columns.Contains("ChiPhiVT"))
            //    return 0;

            //return dt.AsEnumerable()
            //         .Where(r => r["ChiPhiVT"] != DBNull.Value)
            //         .Sum(r => Convert.ToDecimal(r["ChiPhiVT"]));

            if (string.IsNullOrEmpty(maPhieu)) return 0;

            string url = $"{URL}NhaCC/GePMHMMTB?action=GETPMHChiTiet&para1={maPhieu}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            if (json == "[]" || string.IsNullOrEmpty(json))
                return 0;

            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            if (dt == null || dt.Rows.Count == 0)
                return 0;

            // 🔹 Tỷ giá của TIỀN PHIẾU → VND (lấy 1 lần)
            decimal tyGiaPhieu = GetTyGia_ToVND(maTienTePhieu);

            decimal tongCPVT = 0;

            foreach (DataRow r in dt.Rows)
            {

                decimal chiPhiDong = r["ThanhTien"] == DBNull.Value ? 0 : Convert.ToDecimal(r["ThanhTien"]);
                if (chiPhiDong == 0) continue;

                string tienTeDong = r["TienTeID"]?.ToString() ?? "VND";

                // 🔹 Tỷ giá của DÒNG → VND
                decimal tyGiaDong = GetTyGia_ToVND(tienTeDong);

                // 🔹 Quy đổi: Dòng → VND → Tiền phiếu
                decimal chiPhiTheoPhieu = (chiPhiDong * tyGiaDong) / tyGiaPhieu;

                tongCPVT += chiPhiTheoPhieu;
            }

            return Math.Round(tongCPVT, 2);
        }
        private decimal GetTyGia_ToVND(string maTienTe)
        {
            if (string.IsNullOrEmpty(maTienTe) || maTienTe == "VND")
                return 1;

            string url = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={maTienTe}&para2=VND&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            if (json == "[]" || string.IsNullOrEmpty(json))
                return 1;

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
                return 1;

            return Convert.ToDecimal(tbl.Rows[0]["TyGia"]);
        }


        private void grvPhieuMHVatTu_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle == grvPhieuMHVatTu.FocusedRowHandle)
            {
                // Màu xanh dương nhạt - chuyên nghiệp cho bảng cha
                e.Appearance.BackColor = Color.FromArgb(220, 237, 252); // #DCEDFC
                e.HighPriority = true;
            }
        }

        // CustomColumnSort
        private void grvPhieuMHVatTu_CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            // Sort cho cột MaDH
            if (e.Column.FieldName == "MaPhieuMH")
            {
                string value1 = e.Value1?.ToString() ?? string.Empty;
                string value2 = e.Value2?.ToString() ?? string.Empty;

                e.Result = NaturalSort(value1, value2);
                e.Handled = true;
            }


        }

        private int NaturalSort(string x, string y)
        {
            if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y)) return 0;
            if (string.IsNullOrEmpty(x)) return -1;
            if (string.IsNullOrEmpty(y)) return 1;

            int i = 0, j = 0;

            while (i < x.Length && j < y.Length)
            {
                if (char.IsDigit(x[i]) && char.IsDigit(y[j]))
                {
                    string numX = string.Empty;
                    string numY = string.Empty;

                    while (i < x.Length && char.IsDigit(x[i]))
                    {
                        numX += x[i];
                        i++;
                    }

                    while (j < y.Length && char.IsDigit(y[j]))
                    {
                        numY += y[j];
                        j++;
                    }

                    int num1 = int.Parse(numX);
                    int num2 = int.Parse(numY);

                    if (num1 != num2)
                        return num1.CompareTo(num2);
                }
                else
                {
                    if (x[i] != y[j])
                        return x[i].CompareTo(y[j]);

                    i++;
                    j++;
                }
            }

            return x.Length.CompareTo(y.Length);
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadPhieuMuaHang();
        }

        private decimal GetDecimal(DataRow row, string fieldName)
        {
            if (row[fieldName] == null || row[fieldName] == DBNull.Value)
                return 0m;

            if (decimal.TryParse(row[fieldName].ToString(), out decimal result))
                return result;

            return 0m;
        }

        private void toDate_EditValueChanged(object sender, EventArgs e)
        {
            //LoadPhieuMuaHang();
        }

        private void fromDate_EditValueChanged(object sender, EventArgs e)
        {
            //loadPhieuMuaHang();
        }

        private void grvPhieuMHVatTu_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (lstFormatFieldName.Contains(e.Column.FieldName))
                {
                    if (e.Value == null || e.Value == DBNull.Value ||
                        string.IsNullOrWhiteSpace(e.Value?.ToString()) ||
                        !decimal.TryParse(e.Value.ToString(), out decimal value) ||
                        value == 0m)
                    {
                        e.DisplayText = "-";
                        return;
                    }


                    var nfi = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                    nfi.NumberGroupSeparator = ",";

                    int decimals = (decimal.GetBits(value)[3] >> 16) & 0x000000FF;
                    decimals = decimals == 0 ? 0 : Math.Min(decimals, 4);

                    string format = decimals == 0 ? "N0" : $"N{decimals}";

                    e.DisplayText = value.ToString(format, nfi);
                    return;
                }
                if (e.Column.FieldName == "MuaTheo")
                {
                    if (e.ListSourceRowIndex < 0)
                        return;
                    var maDHGop = grvPhieuMHVatTu.GetRowCellValue(e.ListSourceRowIndex, "MaDH_Gop");

                    if (maDHGop != null && maDHGop != DBNull.Value &&
                        !string.IsNullOrWhiteSpace(maDHGop?.ToString()))
                    {
                        e.DisplayText = "Đơn hàng ";
                    }
                    else
                    {
                        e.DisplayText = "Vật tư ";
                    }
                }


            }
            catch (Exception ex)
            {

            }
        }


        private void grvPhieuMHVatTu_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }
        private void focused(object sender)
        {
            try
            {


                DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                if (rowFocused == null) return;
                if (view == null) return;
                bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhan);
                bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                if (view.FocusedColumn == colIsXacNhan && IsDuyet)
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
                else if (view.FocusedColumn == colDuyet && !IsXacNhan)
                {

                }
                else
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                }
            }
            catch (Exception ex)
            {

            }


        }

        private void grvPhieuMHVatTu_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                if (rowFocused == null) return;

                if (grvPhieuMHVatTu.FocusedColumn == colNguoiTao)
                {
                    //if (e.Location.X > CHECK_LEFT + CHECK_SIZE)
                    //    return;

                    bool isAdmin = string.Equals(
                        GlobleData.UserName?.ToString(),
                        "admin",
                        StringComparison.OrdinalIgnoreCase
                    );
                    if (!isAdmin)
                    {
                        string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                        string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                        DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                        bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                            r["ModuleID"]?.ToString() == "M.12.03.00" &&
                            r["AllowDuyet"] != DBNull.Value &&
                            Convert.ToBoolean(r["AllowDuyet"]) == true
                        );

                        if (!coQuyenDuyet)
                        {
                            XtraMessageBox.Show("Bạn không có quyền duyệt phiếu mua hàng!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    //DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                    string trangThai = rowFocused["TrangThai"]?.ToString();
                    switch (trangThai)
                    {

                        //case "Đang xử lý":
                        //    XtraMessageBox.Show(
                        //        "Phiếu đã được duyệt, không được duyệt lại",
                        //        "Thông báo",
                        //        MessageBoxButtons.OK,
                        //        MessageBoxIcon.Warning
                        //    );
                        //    return;

                        case "Đã xác nhận":
                        case "Thành công":
                            XtraMessageBox.Show(
                                "Phiếu đã xác nhận hoặc đã mua hàng thành công, không được duyệt lại",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                    }


                    bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                    bool.TryParse(rowFocused["IsDuyet1"]?.ToString(), out bool IsDuyet1);
                    if (IsDuyet1)
                    {
                        XtraMessageBox.Show("Phiếu đã được trưởng bộ phận tick ký!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //LoadPhieuMuaHang();
                        return;
                    }
                    bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhan);
                    if (IsXacNhan)
                    {
                        XtraMessageBox.Show("Đã xác nhận không được hủy duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //LoadPhieuMuaHang();
                        return;
                    }
                    DialogResult messResult = MessageBox.Show(
                    $"Bạn có muốn {(!IsDuyet ? "duyệt" : "hủy duyệt")} {rowFocused["TenPhieu"]?.ToString()} không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                    _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                    string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                    if (messResult != DialogResult.Yes) return;
                    string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
                    bool check = !IsDuyet;
                    ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
                    objXetDuyet.Action = "DuyetMuaHang";
                    objXetDuyet.MaPhieu = maphieumh;
                    objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                    objXetDuyet.IsXacNhan = !IsDuyet;
                    objXetDuyet.NguoiXN = GlobleData.UserName;
                    objXetDuyet.GhiChu = "";

                    string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                    if (msResult.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        this.DialogResult = DialogResult.OK;
                        LoadDataForCurrentTab();

                        string Title = $"Purchase Order {(!IsDuyet ? "cần soát xét" : "đã hủy xác nhận")}";
                        string Detail = $"Mã Phiếu: {rowFocused["TenPhieu"]?.ToString()}\nNhà cung cấp: {rowFocused["TenNhaCC"]?.ToString()}\nPOMH : {rowFocused["POMua"]}";
                        SendNotify(Title, Detail, "PKH","ALL",1);

                    }
                    else
                    {
                        XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    //bool current = Convert.ToBoolean(
                    //    view.GetRowCellValue(e.RowHandle, "IsDuyet")
                    //);

                    //view.SetRowCellValue(e.RowHandle, "IsDuyet", !current);
                }
                else if (grvPhieuMHVatTu.FocusedColumn == gridColumn24)
                {
                    bool isAdmin = string.Equals(
                        GlobleData.UserName?.ToString(),
                        "admin",
                        StringComparison.OrdinalIgnoreCase
                    );
                    if (!isAdmin)
                    {
                        string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                        string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                        DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                        bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                            r["ModuleID"]?.ToString() == "M.12.03.00" &&
                            r["AllowDuyet2"] != DBNull.Value &&
                            Convert.ToBoolean(r["AllowDuyet2"]) == true
                        );

                        if (!coQuyenDuyet)
                        {
                            XtraMessageBox.Show("Bạn không có quyền tick duyệt giám đốc!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    if (grvPhieuMHVatTu.FocusedRowHandle >= 0)
                    {
                        //DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                        bool.TryParse(rowFocused["IsDuyet1"]?.ToString(), out bool IsDuyet1);
                        if (!IsDuyet1)
                        {
                            XtraMessageBox.Show(
                                "Trưởng bộ phận chưa tick ký!.",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }
                        //string trangThai = rowFocused["TrangThai"]?.ToString();
                        //switch (trangThai)
                        //{
                        //    case "Đã hủy":
                        //        XtraMessageBox.Show(
                        //            "Phiếu đã hủy",
                        //            "Thông báo",
                        //            MessageBoxButtons.OK,
                        //            MessageBoxIcon.Warning
                        //        );
                        //        return;

                        //    case "Đang xử lý":
                        //        XtraMessageBox.Show(
                        //            "Phiếu đã được duyệt, không được duyệt lại",
                        //            "Thông báo",
                        //            MessageBoxButtons.OK,
                        //            MessageBoxIcon.Warning
                        //        );
                        //        return;

                        //    case "Đã xác nhận":
                        //    case "Thành công":
                        //        XtraMessageBox.Show(
                        //            "Phiếu đã xác nhận hoặc đã mua hàng thành công, không được duyệt lại",
                        //            "Thông báo",
                        //            MessageBoxButtons.OK,
                        //            MessageBoxIcon.Warning
                        //        );
                        //        return;
                        //}


                        bool.TryParse(rowFocused["IsDuyet2"]?.ToString(), out bool IsDuyet);
                        if (IsDuyet)
                        {
                            XtraMessageBox.Show(
                                "Giám đốc đã ký, không được hủy ký!.",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }
                        //bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhan);
                        //if (IsXacNhan)
                        //{
                        //    XtraMessageBox.Show("Đã duyệt sẽ không hủy xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    LoadPhieuMuaHang();
                        //    return;
                        //}
                        DialogResult messResult = MessageBox.Show(
                        $"Bạn có muốn {(!IsDuyet ? "duyệt" : "hủy duyệt")} {rowFocused["TenPhieu"]?.ToString()} không?",
                        "Thông báo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                        _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                        string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                        if (messResult != DialogResult.Yes) return;
                        string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
                        bool check = !IsDuyet;
                        ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
                        objXetDuyet.Action = "DuyetMuaHang2";
                        objXetDuyet.MaPhieu = maphieumh;
                        objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                        objXetDuyet.IsXacNhan = !IsDuyet;
                        objXetDuyet.NguoiXN = GlobleData.UserName;
                        objXetDuyet.GhiChu = "";

                        string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                        if (msResult.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 2000);
                            this.DialogResult = DialogResult.OK;
                            //LoadPhieuMuaHang();
                            LoadDataForCurrentTab();
                            string Title = $"Purchase Order {(!IsDuyet ? "xác nhận mua hàng" : "đã hủy xác nhận mua hàng")}";
                            string Detail = $"Mã Phiếu: {rowFocused["TenPhieu"]?.ToString()}\nNhà cung cấp: {rowFocused["TenNhaCC"]?.ToString()}\nPOMH : {rowFocused["POMua"]}";
                            SendNotify(Title, Detail, "PKH");
                        }
                        else
                        {
                            XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else if (grvPhieuMHVatTu.FocusedColumn == gridColumn25)
                {
                    bool.TryParse(rowFocused["IsVatTuKH"]?.ToString(), out bool IsVatTuKH);
                    bool isAdmin = string.Equals(
                        GlobleData.UserName?.ToString(),
                        "admin",
                        StringComparison.OrdinalIgnoreCase
                    );
                    if (!isAdmin && !IsVatTuKH)
                    {
                        string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                        string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                        DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                        bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                            r["ModuleID"]?.ToString() == "M.12.03.00" &&
                            r["AllowXacNhan"] != DBNull.Value &&
                            Convert.ToBoolean(r["AllowXacNhan"]) == true
                        );

                        if (!coQuyenDuyet)
                        {
                            XtraMessageBox.Show("Bạn không có quyền xác nhận!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    if (grvPhieuMHVatTu.FocusedRowHandle >= 0)
                    {
                        //DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                        string trangThai = rowFocused["TrangThai"]?.ToString();
                        string NgayGHTT = "";
                        if (trangThai == "Đã hủy")
                        {
                            XtraMessageBox.Show(
                                "Phiếu đã hủy",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }

                        if (trangThai == "Đã xác nhận" || trangThai == "Thành công")
                        {
                            XtraMessageBox.Show(
                                "Phiếu đã xác nhận hoặc đã mua thành công, không thể xác nhận lại",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }
                        if (rowFocused != null)
                        {
                            string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                            if (string.IsNullOrEmpty(MaPhieuMH))
                            {
                                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                            if (!IsDuyet && !IsVatTuKH)
                            {
                                XtraMessageBox.Show("Vui lòng duyệt trước mới xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            bool.TryParse(rowFocused["IsDuyet1"]?.ToString(), out bool IsDuyet1);
                            bool.TryParse(rowFocused["IsDuyet2"]?.ToString(), out bool IsDuyet2);
                            if ((!IsDuyet1 || !IsDuyet2) && !IsVatTuKH)
                            {
                                XtraMessageBox.Show("Vui lòng trình ký đủ mới xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            DialogResult messResult = MessageBox.Show(
                            $"Bạn có muốn xác nhận {rowFocused["TenPhieu"]?.ToString()} không?",
                            "Thông báo",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                            _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                            string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                            if (messResult != DialogResult.Yes) return;

                            using (frmChonNgayGHTT frm = new frmChonNgayGHTT())
                            {
                                if (frm.ShowDialog() != DialogResult.OK)
                                {
                                    return;
                                }

                                NgayGHTT = frm.Ngay;
                            }
                            string url = $"{URL}NhaCC/GePMH?action=GETPMHXH&para1={MaPhieuMH}&para2=&para3=&para4=&para5=";
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                            DataTable _dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                            DataTable tblDG = CreateDatatableToSave();
                            DateTime today = DateTime.Today;
                            //_dtTable.Columns["NgayDuKienHV"].DataType = typeof(string);
                            foreach (DataRow dr in _dtTable.Rows)
                            {
                                dr["IsXacNhan"] = 1;
                                int soSom = int.TryParse(dr["SoNgayGHSom"]?.ToString(), out int s1) ? s1 : 0;
                                int soTre = int.TryParse(dr["SoNgayGHTre"]?.ToString(), out int s2) ? s2 : 0;

                                DateTime ngaySom = today.AddDays(soSom);
                                DateTime ngayTre = today.AddDays(soSom + soTre);

                                DataRow newRow = tblDG.NewRow();
                                newRow["ID"] = 0;
                                newRow["MaPhieuMH"] = dr["MaPhieuMH"];
                                newRow["TenPhieu"] = dr["TenPhieu"];
                                newRow["MaNCC"] = dr["MaNCC"];
                                newRow["MaVTID"] = dr["MaVTID"]?.ToString();
                                newRow["MaVT"] = dr["MaVT"]?.ToString();
                                newRow["ChiTiet"] = dr["ChiTiet"];
                                newRow["NPL"] = dr["NPL"];
                                newRow["SoLuongMuaThem"] = dr["SoLuongMuaThem"];
                                newRow["DonGia"] = dr["DonGia"];
                                newRow["ThanhTien"] = dr["ThanhTien"];
                                newRow["MaPhieuBG"] = dr["MaPhieuBG"];
                                newRow["MauVTID"] = dr["MauVTID"];
                                newRow["KhoVaiID"] = dr["KhoVaiID"];
                                newRow["MaDVVT"] = dr["MaDVVT"];
                                newRow["NgayTao"] = Convert.ToDateTime(dr["NgayTao"]).ToString("yyyy-MM-dd");
                                newRow["NguoiTao"] = dr["NguoiTao"];
                                newRow["IsDuyet"] = dr["IsDuyet"];
                                newRow["MaCLVT"] = dr["MaCLVT"];
                                newRow["TienTeID"] = dr["TienTeID"];
                                newRow["NgaySua"] = DateTime.Now.ToString("yyyy-MM-dd");
                                newRow["NguoiSua"] = GlobleData.UserName.ToString();
                                newRow["TrangThai"] = "";
                                newRow["NgayDuKienHV"] = $"{ngaySom:dd/MM/yyyy} - {ngayTre:dd/MM/yyyy}";
                                //object ngayXNObj = clsForrmatUtils.ConvertDate(dr["NgayXacNhan"]);

                                //if (ngayXNObj == null || ngayXNObj == DBNull.Value)
                                //{

                                //}
                                //else
                                //{
                                //    DateTime ngayXN = Convert.ToDateTime(ngayXNObj);
                                //    newRow["NgayXacNhan"] = ngayXN.Date;

                                //}
                                newRow["NgayXacNhan"] = DateTime.Now;
                                //object ngayXN = clsForrmatUtils.ConvertDate(dr["NgayXacNhan"]);
                                //newRow["NgayXacNhan"] = ngayXN ?? DBNull.Value;
                                newRow["GhiChu"] = dr["GhiChu"];
                                newRow["PTVanChuyen"] = dr["PTVanChuyen"];
                                newRow["PTThanhToan"] = dr["PTThanhToan"];
                                newRow["Thue"] = dr["Thue"];
                                newRow["CPVanChuyen"] = dr["CPVanChuyen"];
                                newRow["ChiPhiKhac"] = "";
                                newRow["Dot"] = dr["Dot"];
                                newRow["IsXacNhan"] = dr["IsXacNhan"];
                                object ngayHLObj = clsForrmatUtils.ConvertDate(dr["NgayHieuLuc"]);

                                if (ngayHLObj == null || ngayHLObj == DBNull.Value)
                                {
                                    newRow["NgayHieuLuc"] = DBNull.Value;
                                }
                                else
                                {
                                    DateTime ngayHL = Convert.ToDateTime(ngayHLObj);

                                    
                                    if (ngayHL == DateTime.MinValue)
                                        newRow["NgayHieuLuc"] = DBNull.Value;
                                    else
                                        newRow["NgayHieuLuc"] = ngayHL.Date; 
                                }
                                newRow["ChiPhiVT"] = dr["ChiPhiVT"];
                                newRow["POMua"] = dr["POMua"];
                                newRow["TyGiaThanhToan"] = _gia;
                                newRow["MaTienTePhieuMH"] = dr["MaTienTePhieuMH"];
                                newRow["ChietKhau"] = "";
                                newRow["TTChietKhau"] = "";
                                newRow["NgayGiaoHangYC"] = DBNull.Value;
                                tblDG.Rows.Add(newRow);
                            }


                            string url2 = string.Format("{0}?", URL + "NhaCC/PospXN");
                            string mss1 = Task.Run(async () =>
                            {
                                return await _clientExtension.PostAsync(url2, tblDG);
                            }).Result;
                            POSTNGUOIXN(NgayGHTT);
                            if (mss1.ToLower() == "true")
                            {
                                clsWaitForm.ShowSuccessForm(this, 2000);
                                this.DialogResult = DialogResult.OK;
                                //LoadPhieuMuaHang();
                                LoadDataForCurrentTab();
                                string Title = $"Purchase Order {(!IsDuyet ? "đã xác nhận hàng về" : "đã hủy xác nhận hàng về")}";
                                string Detail = $"Mã Phiếu: {rowFocused["TenPhieu"]?.ToString()}\nNhà cung cấp: {rowFocused["TenNhaCC"]?.ToString()}\nPOMH : {rowFocused["POMua"]}";
                                SendNotify(Title, Detail, "PKH");
                                SendNotify(Title, Detail, "KHO");
                            }
                            else
                            {
                                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }


                        }
                    }
                }
                else if (grvPhieuMHVatTu.FocusedColumn == gridColumn26)
                {
                    bool isAdmin = string.Equals(
                        GlobleData.UserName?.ToString(),
                        "admin",
                        StringComparison.OrdinalIgnoreCase
                    );
                    if (!isAdmin)
                    {
                        string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                        string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                        DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                        bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                            r["ModuleID"]?.ToString() == "M.12.03.00" &&
                            r["AllowXacNhan"] != DBNull.Value &&
                            Convert.ToBoolean(r["AllowXacNhan"]) == true
                        );

                        if (!coQuyenDuyet)
                        {
                            XtraMessageBox.Show("Bạn không có quyền xác nhận!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    if (grvPhieuMHVatTu.FocusedRowHandle >= 0)
                    {
                        //DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                        string trangThai = rowFocused["TrangThai"]?.ToString();
                        string ketquakiem = rowFocused["KetQuaKiem"]?.ToString();
                        string ghiChuKetThucSom = "";
                        if (trangThai == "Đã hủy")
                        {
                            XtraMessageBox.Show(
                                "Phiếu đã hủy",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }
                        bool.TryParse(rowFocused["IsDuyet2"]?.ToString(), out bool IsDuyet);
                        if (!IsDuyet)
                        {
                            XtraMessageBox.Show("Vui lòng duyệt trước mới xác nhận mua hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //LoadPhieuMuaHang();
                            //LoadDataForCurrentTab();
                            return;
                        }

                        bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhanngaygh);
                        if (!IsXacNhanngaygh)
                        {
                            XtraMessageBox.Show(
                                "Phiếu chưa xác nhận ngày giao hàng",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }
                        bool.TryParse(rowFocused["IsKetThuc"]?.ToString(), out bool IsKetThuc);
                        if (IsKetThuc)
                        {
                            XtraMessageBox.Show(
                                "Phiếu đã mua hàng thành công",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }

                        if (ketquakiem != "PASS")
                        {
                            DialogResult result = XtraMessageBox.Show(
                                "Phiếu hàng về chưa đủ, bạn có muốn kết thúc phiếu sớm không?",
                                "Thông báo",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Warning
                            );

                            if (result != DialogResult.OK)
                            {
                                return;
                            }

                            using (frmNGCKetThucPMHSom frm = new frmNGCKetThucPMHSom())
                            {
                                if (frm.ShowDialog() != DialogResult.OK)
                                {
                                    return;
                                }

                                ghiChuKetThucSom = frm.GhiChu;
                            }
                        }
                        bool.TryParse(rowFocused["IsKetThuc"]?.ToString(), out bool IsXacNhan);
                        DialogResult messResult = MessageBox.Show(
                        $"Bạn có muốn {(!IsXacNhan ? "xác nhận thành công" : "hủy xác nhận thành công")} {rowFocused["TenPhieu"]?.ToString()} không?",
                        "Thông báo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                        _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                        if (messResult != DialogResult.Yes) return;
                        string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
                        ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
                        objXetDuyet.Action = "XacNhanMuahang";
                        objXetDuyet.MaPhieu = rowFocused["MaPhieuMH"]?.ToString();
                        objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                        objXetDuyet.IsXacNhan = !IsXacNhan;
                        objXetDuyet.NguoiXN = GlobleData.UserName;
                        objXetDuyet.GhiChu = ghiChuKetThucSom;

                        string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                        POSTNGUOIKT();
                        if (msResult.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 2000);
                            this.DialogResult = DialogResult.OK;
                            //LoadPhieuMuaHang();
                            LoadDataForCurrentTab();
                            string Title = $"Purchase Order {(!IsDuyet ? "đã kết thúc mua hàng" : "đã hủy kết thúc mua hàng")}";
                            string Detail = $"Mã Phiếu: {rowFocused["TenPhieu"]?.ToString()}\nNhà cung cấp: {rowFocused["TenNhaCC"]?.ToString()}\nPOMH : {rowFocused["POMua"]}";
                            SendNotify(Title, Detail, "PKH");
                        }
                        else
                        {
                            XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else if (grvPhieuMHVatTu.FocusedColumn == gridColumn27)
                {
                    try
                    {
                        bool isAdmin = string.Equals(
                            GlobleData.UserName?.ToString(),
                            "admin",
                            StringComparison.OrdinalIgnoreCase
                        );
                        if (!isAdmin)
                        {
                            string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                            string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                            DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                            bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                                r["ModuleID"]?.ToString() == "M.12.03.00" &&
                                r["AllowDuyet1"] != DBNull.Value &&
                                Convert.ToBoolean(r["AllowDuyet1"]) == true
                            );

                            if (!coQuyenDuyet)
                            {
                                XtraMessageBox.Show("Bạn không có quyền tick ký trưởng bộ phận!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }

                        if (grvPhieuMHVatTu.FocusedRowHandle >= 0)
                        {
                            bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                            if (!IsDuyet)
                            {
                                XtraMessageBox.Show(
                                    "Người tạo chưa tick duyệt!. Người tạo vui lòng tick duyệt trước khi tick ký trưởng bộ phận",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                                return;
                            }



                            bool.TryParse(rowFocused["IsDuyet1"]?.ToString(), out bool IsDuyet1);
                            bool.TryParse(rowFocused["IsDuyet2"]?.ToString(), out bool IsDuye2);
                            if (IsDuye2)
                            {
                                XtraMessageBox.Show(
                                    "Giám đốc đã tick ký, không thể hủy ký",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                                return;
                            }
                            //bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhan);
                            //if (IsXacNhan)
                            //{
                            //    XtraMessageBox.Show("Đã duyệt sẽ không hủy xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //    LoadPhieuMuaHang();
                            //    return;
                            //}
                            DialogResult messResult = MessageBox.Show(
                            $"Bạn có muốn {(!IsDuyet1 ? "duyệt" : "hủy duyệt")} {rowFocused["TenPhieu"]?.ToString()} không?",
                            "Thông báo",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                            _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                            string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                            if (messResult != DialogResult.Yes) return;
                            string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
                            bool check = !IsDuyet;
                            ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
                            objXetDuyet.Action = "DuyetMuaHang1";
                            objXetDuyet.MaPhieu = maphieumh;
                            objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                            objXetDuyet.IsXacNhan = !IsDuyet1;
                            objXetDuyet.NguoiXN = GlobleData.UserName;
                            objXetDuyet.GhiChu = "";

                            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                            if (msResult.ToLower() == "true")
                            {
                                clsWaitForm.ShowSuccessForm(this, 2000);
                                this.DialogResult = DialogResult.OK;
                                //LoadPhieuMuaHang();
                                LoadDataForCurrentTab();
                                string Title = $"Purchase Order {(!IsDuyet ? "cần giám đốc xác nhận" : "trưởng bộ phận đã hủy soát xét")}";
                                string Detail = $"Mã Phiếu: {rowFocused["TenPhieu"]?.ToString()}\nNhà cung cấp: {rowFocused["TenNhaCC"]?.ToString()}\nPOMH : {rowFocused["POMua"]}";
                                SendNotify(Title, Detail, "PKH", "ALL", 0);
                            }
                            else
                            {
                                XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception exz)
                    {

                    }


                }
                #region
                //else if (grvPhieuMHVatTu.FocusedColumn == gridColumn31) 
                //{
                //    string kiemPL = rowFocused["KPL"]?.ToString();
                //    string SoLoID = rowFocused["SoLoID"]?.ToString();
                //    string SoLo = rowFocused["SoLo"]?.ToString();
                //    //if (kiemPL == "")
                //    //{
                //    //    return;
                //    //}

                //    frmViewKiemPL frm = new frmViewKiemPL(SoLoID, SoLo);
                //    frm.Show();
                //}
                //else if (grvPhieuMHVatTu.FocusedColumn == gridColumn30)
                //{
                //    string kiemNL = rowFocused["KNL"]?.ToString();
                //    string SoLoID = rowFocused["SoLoID"]?.ToString();
                //    string SoLo = rowFocused["SoLo"]?.ToString();
                //    //if (kiemNL == "")
                //    //{
                //    //    return;
                //    //}
                //    frmViewKiemNL frm = new frmViewKiemNL(SoLoID, SoLo);
                //    frm.Show();
                //}
                //else if (grvPhieuMHVatTu.FocusedColumn == gridColumn45)
                //{
                //    try
                //    {
                //        bool isAdmin = string.Equals(
                //            GlobleData.UserName?.ToString(),
                //            "admin",
                //            StringComparison.OrdinalIgnoreCase
                //        );
                //        if (!isAdmin)
                //        {
                //            string nguoitao = rowFocused["NguoiTao"]?.ToString();
                //            if (GlobleData.UserName.ToString().ToUpper() != nguoitao?.ToUpper())
                //            {
                //                XtraMessageBox.Show("bạn không phải là người tạo phiếu. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //                return;
                //            }
                //        }

                //        if (grvPhieuMHVatTu.FocusedRowHandle >= 0)
                //        {
                //            bool.TryParse(rowFocused["IsNhapKho"]?.ToString(), out bool IsDuyet);
                //            //bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhan);
                //            //if (IsXacNhan)
                //            //{
                //            //    XtraMessageBox.Show("Đã duyệt sẽ không hủy xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //            //    LoadPhieuMuaHang();
                //            //    return;
                //            //}
                //            DialogResult messResult = MessageBox.Show(
                //            $"Bạn có muốn {(!IsDuyet ? "nhập kho" : "hủy nhập kho")} {rowFocused["TenPhieu"]?.ToString()} không?",
                //            "Thông báo",
                //            MessageBoxButtons.YesNo,
                //            MessageBoxIcon.Question);
                //            string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                //            if (messResult != DialogResult.Yes) return;
                //            string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
                //            bool check = !IsDuyet;
                //            ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
                //            objXetDuyet.Action = "XNNhapKho";
                //            objXetDuyet.MaPhieu = maphieumh;
                //            objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                //            objXetDuyet.IsXacNhan = !IsDuyet;
                //            objXetDuyet.NguoiXN = GlobleData.UserName;
                //            objXetDuyet.GhiChu = "";

                //            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                //            if (msResult.ToLower() == "true")
                //            {
                //                clsWaitForm.ShowSuccessForm(this, 2000);
                //                this.DialogResult = DialogResult.OK;
                //                LoadPhieuMuaHang();
                //            }
                //            else
                //            {
                //                XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //            }
                //        }
                //    }
                //    catch (Exception exz)
                //    {

                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {

            }

        }

        //private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        //{
        //    //frmERPPhieuMuaHangV2 frm = new frmERPPhieuMuaHangV2(null,false,true,false);
        //    frmPhieuMuaHang frm = new frmPhieuMuaHang(null, false, true, false);
        //    frm.ShowDialog();
        //    frm.MaximizeBox = true;
        //    if (frm.DialogResult == DialogResult.OK)
        //    {
        //        LoadPhieuMuaHang();

        //        SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
        //        try
        //        {
        //            getVatTuCanMua();
        //            calcVatTuCanMua();
        //            loadGridControl1();
        //        }
        //        catch (Exception ex)
        //        {
        //            XtraMessageBox.Show(ex.Message, "Lỗi",
        //                            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            SplashScreenManager.CloseDefaultWaitForm();
        //        }
        //        finally
        //        {
        //            SplashScreenManager.CloseDefaultWaitForm();
        //        }
        //    }
        //}

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {


            if (grvPhieuMHVatTu.FocusedRowHandle >= 0 || gridView5.FocusedRowHandle >= 0)
            {

                if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                {
                    DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                    if (rowFocused != null)
                    {
                        string nguoitao = rowFocused["NguoiTao"]?.ToString();
                        if (GlobleData.UserName.ToString().ToUpper() == nguoitao?.ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
                        {
                            _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                            string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                            bool isduyet = Convert.ToBoolean(rowFocused["IsDuyet1"]);
                            bool isVTKH = Convert.ToBoolean(rowFocused["IsVatTuKh"]);
                            //if (isduyet)
                            //{
                            //    XtraMessageBox.Show("Phiếu đã được trưởng bộ phận tick ký, không thể chỉnh sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //    return;
                            //}
                            if (string.IsNullOrEmpty(MaPhieuMH))
                            {
                                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (isVTKH)
                            {
                                frmPhieuMuaHangPCDDH frm = new frmPhieuMuaHangPCDDH(rowFocused, true);
                                frm.ShowDialog();
                                frm.MaximizeBox = true;
                                if (frm.DialogResult == DialogResult.OK)
                                {
                                    //LoadPhieuMuaHang();
                                    LoadDataForCurrentTab();
                                    xtraTabControl1.SelectedTabPage = xtraTabPage1;
                                }
                            }
                            else
                            {
                                frmPhieuMuaHang frm = new frmPhieuMuaHang(rowFocused, true);
                                //frmERPPhieuMuaHangV2 frm = new frmERPPhieuMuaHangV2(rowFocused, true);
                                frm.ShowDialog();
                                frm.MaximizeBox = true;
                                if (frm.DialogResult == DialogResult.OK)
                                {
                                    //LoadPhieuMuaHang();
                                    LoadDataForCurrentTab();
                                    xtraTabControl1.SelectedTabPage = xtraTabPage1;
                                }
                            }

                        }
                        else
                        {
                            XtraMessageBox.Show("bạn không phải là người tạo, không có quyền sửa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                }
                else if (xtraTabControl1.SelectedTabPage == xtraTabPage3)
                {
                    DataRow rowFocused = gridView5.GetFocusedDataRow() as DataRow;
                    DataRow rowFocusedpyc = gridView6.GetFocusedDataRow() as DataRow;
                    if (rowFocused != null)
                    {
                        string nguoitao = rowFocused["NguoiTao"]?.ToString();
                        if (GlobleData.UserName.ToString().ToUpper() == nguoitao?.ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
                        {
                            _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                            string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                            bool isduyet = Convert.ToBoolean(rowFocused["IsDuyet"]);
                            if (isduyet)
                            {
                                XtraMessageBox.Show("Phiếu đã được duyệt, không thể chỉnh sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (string.IsNullOrEmpty(MaPhieuMH))
                            {
                                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            frmPhieuMuaMMTB frm = new frmPhieuMuaMMTB(rowFocused, rowFocusedpyc, true);
                            //frmERPPhieuMuaHangV2 frm = new frmERPPhieuMuaHangV2(rowFocused, true);
                            frm.ShowDialog();
                            frm.MaximizeBox = true;
                            if (frm.DialogResult == DialogResult.OK)
                            {
                                //LoadPhieuMuaHangMMTB();
                                LoadDataForCurrentTab();
                                xtraTabControl1.SelectedTabPage = xtraTabPage1;
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("bạn không phải là người tạo, không có quyền sửa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                }
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (grvPhieuMHVatTu.FocusedRowHandle >= 0 || gridView5.FocusedRowHandle >= 0)
            {

                if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                {
                    DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                    if (rowFocused != null)
                    {
                        string nguoitao = rowFocused["NguoiTao"]?.ToString();
                        if (GlobleData.UserName.ToString().ToUpper() == nguoitao?.ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
                        {
                            string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                            if (string.IsNullOrEmpty(MaPhieuMH))
                            {
                                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            //bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                            //if (IsDuyet)
                            //{
                            //    XtraMessageBox.Show("Phiếu mua hàng đã duyệt, hãy hủy trước khi xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //    LoadPhieuMuaHang();
                            //    return;
                            //}
                            DialogResult messResult = MessageBox.Show(
                               $"Bạn có muốn xóa {rowFocused["TenPhieu"]?.ToString()} không?",
                               "Thông báo",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question);
                            if (messResult != DialogResult.Yes) return;
                            string url = $"{URL}ERPCanDoiNguyenPhuLieu/Delete?action=DeletePhieuMuaHang&Para1={Uri.EscapeDataString(MaPhieuMH)}&Para2={GlobleData.UserName.ToString()}";
                            string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                            if (result.ToLower() == "true")
                            {
                                // LoadPhieuMuaHang();
                                LoadDataForCurrentTab();
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("bạn không phải là người tạo, không có quyền xóa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                }
                else if (xtraTabControl1.SelectedTabPage == xtraTabPage3)
                {
                    DataRow rowFocused = gridView5.GetFocusedDataRow() as DataRow;
                    if (rowFocused != null)
                    {
                        string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                        // Kiểm tra SoSeri trùng trên server
                        string urlCheck = $"{URL}ERP_POMH_PhieuMuaHangMMTB_Import_Seri/GET?action=CheckUseMMTB&para1={MaPhieuMH}&para2=NONE";
                        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCheck); }).Result;
                        if (json != "[]")
                        {
                            XtraMessageBox.Show($"Vật tư phiếu mua này đã phát sinh dữ liệu không thể xóa!",
                                "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            return;
                        }
                        string nguoitao = rowFocused["NguoiTao"]?.ToString();
                        if (GlobleData.UserName.ToString().ToUpper() == nguoitao?.ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
                        {

                            if (string.IsNullOrEmpty(MaPhieuMH))
                            {
                                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            //bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                            //if (IsDuyet)
                            //{
                            //    XtraMessageBox.Show("Phiếu mua hàng đã duyệt, hãy hủy trước khi xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //    LoadPhieuMuaHang();
                            //    return;
                            //}
                            DialogResult messResult = MessageBox.Show(
                               $"Bạn có muốn xóa {rowFocused["TenPhieu"]?.ToString()} không?",
                               "Thông báo",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question);
                            if (messResult != DialogResult.Yes) return;
                            string url = $"{URL}ERPPOMuaMMTB/Delete?action=DeletePhieuMuaHang&Para1={Uri.EscapeDataString(MaPhieuMH)}&Para2={GlobleData.UserName.ToString()}";
                            string result = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                            if (result.ToLower() == "true")
                            {
                                //LoadPhieuMuaHangMMTB();
                                LoadDataForCurrentTab();
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("bạn không phải là người tạo, không có quyền xóa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                }
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }


        private DataTable CreateDatatableToSave()
        {
            DataTable tbl = new DataTable("dtSize");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaPhieuMH", typeof(string));
            tbl.Columns.Add("TenPhieu", typeof(string));
            tbl.Columns.Add("MaNCC", typeof(string));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaVT", typeof(string));
            tbl.Columns.Add("ChiTiet", typeof(string));
            tbl.Columns.Add("NPL", typeof(string));
            tbl.Columns.Add("SoLuongMuaThem", typeof(float));
            tbl.Columns.Add("DonGia", typeof(float));
            tbl.Columns.Add("ThanhTien", typeof(float));
            tbl.Columns.Add("MaPhieuBG", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(string));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("IsDuyet", typeof(bool));
            tbl.Columns.Add("MaCLVT", typeof(string));
            tbl.Columns.Add("TienTeID", typeof(string));
            tbl.Columns.Add("NgaySua", typeof(DateTime));
            tbl.Columns.Add("NguoiSua", typeof(string));
            tbl.Columns.Add("TrangThai", typeof(string));
            tbl.Columns.Add("NgayDuKienHV", typeof(string));
            tbl.Columns.Add("NgayXacNhan", typeof(DateTime));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("PTVanChuyen", typeof(string));
            tbl.Columns.Add("PTThanhToan", typeof(string));
            tbl.Columns.Add("TGGiao", typeof(DateTime));
            tbl.Columns.Add("Thue", typeof(float));
            tbl.Columns.Add("CPVanChuyen", typeof(float));
            tbl.Columns.Add("ChiPhiKhac", typeof(string));
            tbl.Columns.Add("Dot", typeof(string));
            tbl.Columns.Add("IsXacNhan", typeof(bool));
            tbl.Columns.Add("NgayHieuLuc", typeof(DateTime));
            tbl.Columns.Add("ChiPhiVT", typeof(float));
            tbl.Columns.Add("POMua", typeof(string));
            tbl.Columns.Add("TyGiaThanhToan", typeof(decimal));
            tbl.Columns.Add("MaTienTePhieuMH", typeof(string));
            tbl.Columns.Add("ChietKhau", typeof(string));
            tbl.Columns.Add("TTChietKhau", typeof(string));
            tbl.Columns.Add("NgayGiaoHangYC", typeof(DateTime));
            tbl.Columns.Add("Action", typeof(string));
            tbl.Columns.Add("MaKhachHang", typeof(string));
            return tbl;
        }
        private DataTable CreateDatatableToSave1()
        {
            DataTable tbl = new DataTable("dtSize1");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("LoaiHang", typeof(string));
            tbl.Columns.Add("IsTB", typeof(bool));
            tbl.Columns.Add("MaPhieuMH", typeof(string));
            tbl.Columns.Add("MaPhieuBG", typeof(string));
            tbl.Columns.Add("TenPhieu", typeof(string));
            tbl.Columns.Add("MaNCC", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaCL", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("MaKho", typeof(string));
            tbl.Columns.Add("TenHang", typeof(string));
            tbl.Columns.Add("SoSeri", typeof(string));
            tbl.Columns.Add("DonVi", typeof(string));
            tbl.Columns.Add("MauMa", typeof(string));
            tbl.Columns.Add("XuatXu", typeof(string));
            tbl.Columns.Add("HangSX", typeof(string));
            tbl.Columns.Add("SoLuongMuaThem", typeof(decimal));
            tbl.Columns.Add("DonGia", typeof(decimal));
            tbl.Columns.Add("ThanhTien", typeof(decimal));
            tbl.Columns.Add("Thue", typeof(float));
            tbl.Columns.Add("CPVanChuyen", typeof(decimal));
            tbl.Columns.Add("ChiPhiTB", typeof(decimal));
            tbl.Columns.Add("ChiPhiKhac", typeof(string));
            tbl.Columns.Add("TienTeID", typeof(string));
            tbl.Columns.Add("MaTienTePhieuMH", typeof(string));
            tbl.Columns.Add("TyGiaThanhToan", typeof(decimal));
            tbl.Columns.Add("ChietKhau", typeof(float));
            tbl.Columns.Add("TTChietKhau", typeof(string));
            tbl.Columns.Add("POMua", typeof(string));
            tbl.Columns.Add("NgayDuKienHV", typeof(string));
            tbl.Columns.Add("NgayGiaoHangYC", typeof(DateTime));
            tbl.Columns.Add("NgayHieuLuc", typeof(DateTime));
            tbl.Columns.Add("NgayXacNhan", typeof(DateTime));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(DateTime));
            tbl.Columns.Add("NguoiSua", typeof(string));
            tbl.Columns.Add("NgaySua", typeof(DateTime));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("GhiChuTB", typeof(string));
            tbl.Columns.Add("IsDuyet", typeof(bool));
            tbl.Columns.Add("IsXacNhan", typeof(bool));
            tbl.Columns.Add("PhieuYC", typeof(string));
            //tbl.Columns.Add("ThanhTienVND", typeof(decimal));
            //tbl.Columns.Add("ThanhTienQD", typeof(decimal));
            tbl.Columns.Add("Action", typeof(string));
            tbl.Columns.Add("MaKhachHang", typeof(string));
            return tbl;
        }
        private DataTable CreateDatatableToSavePL()
        {
            DataTable tbl = new DataTable("dtpl");
            tbl.Columns.Add("SoLoID", typeof(string));
            tbl.Columns.Add("MaNPL", typeof(string));
            tbl.Columns.Add("KQ_GiaiQuyet", typeof(string));
            return tbl;
        }
        private DataTable CreateDatatableToSaveNL()
        {
            DataTable tbl = new DataTable("dtnl");
            tbl.Columns.Add("SoLoID", typeof(string));
            tbl.Columns.Add("MaNPL", typeof(string));
            tbl.Columns.Add("ResultQC", typeof(string));
            return tbl;
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool isAdmin = string.Equals(
                GlobleData.UserName?.ToString(),
                "admin",
                StringComparison.OrdinalIgnoreCase
            );
            if (!isAdmin)
            {
                string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                    r["ModuleID"]?.ToString() == "M.12.03.00" &&
                    r["AllowXacNhan"] != DBNull.Value &&
                    Convert.ToBoolean(r["AllowXacNhan"]) == true
                );

                if (!coQuyenDuyet)
                {
                    XtraMessageBox.Show("Bạn không có quyền xác nhận!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (grvPhieuMHVatTu.FocusedRowHandle >= 0)
            {
                DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                string trangThai = rowFocused["TrangThai"]?.ToString();
                if (trangThai == "Đã hủy")
                {
                    XtraMessageBox.Show(
                        "Phiếu đã hủy",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (trangThai == "Đã xác nhận" || trangThai == "Thành công")
                {
                    XtraMessageBox.Show(
                        "Phiếu đã xác nhận hoặc đã mua thành công, không thể xác nhận lại",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
                if (rowFocused != null)
                {
                    string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                    if (string.IsNullOrEmpty(MaPhieuMH))
                    {
                        XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                    if (!IsDuyet)
                    {
                        XtraMessageBox.Show("Vui lòng duyệt trước mới xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        LoadPhieuMuaHang();
                        return;
                    }
                    DialogResult messResult = MessageBox.Show(
                    $"Bạn có muốn xác nhận {rowFocused["TenPhieu"]?.ToString()} không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                    string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                    if (messResult != DialogResult.Yes) return;
                    string url = $"{URL}NhaCC/GePMH?action=GETPMHXH&para1={MaPhieuMH}&para2=&para3=&para4=&para5=";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    DataTable _dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                    DataTable tblDG = CreateDatatableToSave();
                    DateTime today = DateTime.Today;
                    //_dtTable.Columns["NgayDuKienHV"].DataType = typeof(string);
                    foreach (DataRow dr in _dtTable.Rows)
                    {
                        dr["IsXacNhan"] = 1;
                        int soSom = int.TryParse(dr["SoNgayGHSom"]?.ToString(), out int s1) ? s1 : 0;
                        int soTre = int.TryParse(dr["SoNgayGHTre"]?.ToString(), out int s2) ? s2 : 0;

                        DateTime ngaySom = today.AddDays(soSom);
                        DateTime ngayTre = today.AddDays(soSom + soTre);

                        DataRow newRow = tblDG.NewRow();
                        newRow["ID"] = 0;
                        newRow["MaPhieuMH"] = dr["MaPhieuMH"];
                        newRow["TenPhieu"] = dr["TenPhieu"];
                        newRow["MaNCC"] = dr["MaNCC"];
                        newRow["MaVTID"] = dr["MaVTID"]?.ToString();
                        newRow["MaVT"] = dr["MaVT"]?.ToString();
                        newRow["ChiTiet"] = dr["ChiTiet"];
                        newRow["NPL"] = dr["NPL"];
                        newRow["SoLuongMuaThem"] = dr["SoLuongMuaThem"];
                        newRow["DonGia"] = dr["DonGia"];
                        newRow["ThanhTien"] = dr["ThanhTien"];
                        newRow["MaPhieuBG"] = dr["MaPhieuBG"];
                        newRow["MauVTID"] = dr["MauVTID"];
                        newRow["KhoVaiID"] = dr["KhoVaiID"];
                        newRow["MaDVVT"] = dr["MaDVVT"];
                        newRow["NgayTao"] = Convert.ToDateTime(dr["NgayTao"]).ToString("yyyy-MM-dd");
                        newRow["NguoiTao"] = dr["NguoiTao"];
                        newRow["IsDuyet"] = dr["IsDuyet"];
                        newRow["MaCLVT"] = dr["MaCLVT"];
                        newRow["TienTeID"] = dr["TienTeID"];
                        newRow["NgaySua"] = DateTime.Now.ToString("yyyy-MM-dd");
                        newRow["NguoiSua"] = GlobleData.UserName.ToString();
                        newRow["TrangThai"] = "";
                        newRow["NgayDuKienHV"] = $"{ngaySom:dd/MM/yyyy} - {ngayTre:dd/MM/yyyy}";
                        object ngayXNObj = clsForrmatUtils.ConvertDate(dr["NgayXacNhan"]);

                        if (ngayXNObj == null || ngayXNObj == DBNull.Value)
                        {
                            newRow["NgayXacNhan"] = DateTime.Now;
                        }
                        else
                        {
                            DateTime ngayXN = Convert.ToDateTime(ngayXNObj);

                            // ❗ Chặn 01/01/0001
                            if (ngayXN == DateTime.MinValue)
                                newRow["NgayXacNhan"] = DBNull.Value;
                            else
                                newRow["NgayXacNhan"] = ngayXN.Date; // ✅ DATE SQL (yyyy-MM-dd)
                        }
                        //object ngayXN = clsForrmatUtils.ConvertDate(dr["NgayXacNhan"]);
                        //newRow["NgayXacNhan"] = ngayXN ?? DBNull.Value;
                        newRow["GhiChu"] = dr["GhiChu"];
                        newRow["PTVanChuyen"] = dr["PTVanChuyen"];
                        newRow["PTThanhToan"] = dr["PTThanhToan"];
                        newRow["Thue"] = dr["Thue"];
                        newRow["CPVanChuyen"] = dr["CPVanChuyen"];
                        newRow["ChiPhiKhac"] = "";
                        newRow["Dot"] = dr["Dot"];
                        newRow["IsXacNhan"] = dr["IsXacNhan"];
                        object ngayHLObj = clsForrmatUtils.ConvertDate(dr["NgayHieuLuc"]);

                        if (ngayHLObj == null || ngayHLObj == DBNull.Value)
                        {
                            newRow["NgayHieuLuc"] = DBNull.Value;
                        }
                        else
                        {
                            DateTime ngayHL = Convert.ToDateTime(ngayHLObj);

                            // ❗ Chặn 01/01/0001
                            if (ngayHL == DateTime.MinValue)
                                newRow["NgayHieuLuc"] = DBNull.Value;
                            else
                                newRow["NgayHieuLuc"] = ngayHL.Date; // ✅ DATE SQL (yyyy-MM-dd)
                        }
                        newRow["ChiPhiVT"] = dr["ChiPhiVT"];
                        newRow["POMua"] = "";
                        newRow["TyGiaThanhToan"] = _gia;
                        newRow["MaTienTePhieuMH"] = dr["MaTienTePhieuMH"];
                        newRow["ChietKhau"] = dr["ChietKhau"];
                        newRow["TTChietKhau"] = dr["TTChietKhau"];
                        newRow["NgayGiaoHangYC"] = DBNull.Value;
                        tblDG.Rows.Add(newRow);
                    }


                    string url2 = string.Format("{0}?", URL + "NhaCC/PospXN");
                    string mss1 = Task.Run(async () =>
                    {
                        return await _clientExtension.PostAsync(url2, tblDG);
                    }).Result;
                    if (mss1.ToLower() != "true")
                        XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadPhieuMuaHang();


                }
            }
        }


        private void grvPhieuMHVatTu_DoubleClick(object sender, EventArgs e)
        {
            if (grvPhieuMHVatTu.FocusedRowHandle >= 0)
            {
                DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                if (rowFocused != null)
                {
                    string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                    bool isVTKH = Convert.ToBoolean(rowFocused["IsVatTuKh"]);
                    if (string.IsNullOrEmpty(MaPhieuMH))
                    {
                        XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (isVTKH)
                    {
                        frmPhieuMuaHangPCDDH frm = new frmPhieuMuaHangPCDDH(rowFocused, false, false, true);
                        frm.ShowDialog();
                        frm.MaximizeBox = true;
                        if (frm.DialogResult == DialogResult.OK)
                        {
                            //LoadPhieuMuaHang();
                            LoadDataForCurrentTab();
                            xtraTabControl1.SelectedTabPage = xtraTabPage1;
                        }
                    }
                    else
                    {
                        frmPhieuMuaHang frm = new frmPhieuMuaHang(rowFocused, false, false, true);
                        frm.ShowDialog();
                        frm.MaximizeBox = true;
                        if (frm.DialogResult == DialogResult.OK)
                        {
                            //LoadPhieuMuaHang();
                            LoadDataForCurrentTab();
                            xtraTabControl1.SelectedTabPage = xtraTabPage1;
                        }
                    }



                }
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //LoadPhieuMuaHang();

            //SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
            //try
            //{
            //    getVatTuCanMua();
            //    calcVatTuCanMua();
            //    loadGridControl1();
            //}
            //catch (Exception ex)
            //{
            //    XtraMessageBox.Show(ex.Message, "Lỗi",
            //                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    SplashScreenManager.CloseDefaultWaitForm();
            //}
            //finally
            //{
            //    SplashScreenManager.CloseDefaultWaitForm();
            //}
            LoadDataForCurrentTab();
        }
        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool isAdmin = string.Equals(
                GlobleData.UserName?.ToString(),
                "admin",
                StringComparison.OrdinalIgnoreCase
            );
            if (!isAdmin)
            {
                string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                    r["ModuleID"]?.ToString() == "M.12.03.00" &&
                    r["AllowDuyet"] != DBNull.Value &&
                    Convert.ToBoolean(r["AllowDuyet"]) == true
                );

                if (!coQuyenDuyet)
                {
                    XtraMessageBox.Show("Bạn không có quyền duyệt phiếu mua hàng!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (grvPhieuMHVatTu.FocusedRowHandle >= 0)
            {
                DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                string trangThai = rowFocused["TrangThai"]?.ToString();
                switch (trangThai)
                {
                    case "Đã hủy":
                        XtraMessageBox.Show(
                            "Phiếu đã hủy",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;

                    case "Đang xử lý":
                        XtraMessageBox.Show(
                            "Phiếu đã được duyệt, không được duyệt lại",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;

                    case "Đã xác nhận":
                    case "Thành công":
                        XtraMessageBox.Show(
                            "Phiếu đã xác nhận hoặc đã mua hàng thành công, không được duyệt lại",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                }


                bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhan);
                if (IsXacNhan)
                {
                    XtraMessageBox.Show("Đã duyệt sẽ không hủy xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LoadPhieuMuaHang();
                    return;
                }
                DialogResult messResult = MessageBox.Show(
                $"Bạn có muốn {(!IsDuyet ? "duyệt" : "hủy duyệt")} {rowFocused["TenPhieu"]?.ToString()} không?",
                "Thông báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                if (messResult != DialogResult.Yes) return;
                string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
                bool check = !IsDuyet;
                ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
                objXetDuyet.Action = "DuyetMuaHang";
                objXetDuyet.MaPhieu = maphieumh;
                objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                objXetDuyet.IsXacNhan = !IsDuyet;
                objXetDuyet.NguoiXN = GlobleData.UserName;
                objXetDuyet.GhiChu = "";

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    this.DialogResult = DialogResult.OK;
                    LoadPhieuMuaHang();
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

        }

        private void barButtonItem9_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool isAdmin = string.Equals(
                GlobleData.UserName?.ToString(),
                "admin",
                StringComparison.OrdinalIgnoreCase
            );
            if (!isAdmin)
            {
                string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                    r["ModuleID"]?.ToString() == "M.12.03.00" &&
                    r["AllowXacNhan"] != DBNull.Value &&
                    Convert.ToBoolean(r["AllowXacNhan"]) == true
                );

                if (!coQuyenDuyet)
                {
                    XtraMessageBox.Show("Bạn không có quyền xác nhận!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (grvPhieuMHVatTu.FocusedRowHandle >= 0)
            {
                DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                string trangThai = rowFocused["TrangThai"]?.ToString();
                if (trangThai == "Đã hủy")
                {
                    XtraMessageBox.Show(
                        "Phiếu đã hủy",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
                bool.TryParse(rowFocused["IsDuyet2"]?.ToString(), out bool IsDuyet);
                if (!IsDuyet)
                {
                    XtraMessageBox.Show("Vui lòng duyệt trước mới xác nhận mua hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LoadPhieuMuaHang();
                    return;
                }
                if (trangThai != "Đã xác nhận")
                {
                    XtraMessageBox.Show(
                        "Phiếu chưa xác nhận hoặc đã mua hàng thành công",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
                bool.TryParse(rowFocused["IsKetThuc"]?.ToString(), out bool IsXacNhan);
                DialogResult messResult = MessageBox.Show(
                $"Bạn có muốn {(!IsXacNhan ? "xác nhận thành công" : "hủy xác nhận thành công")} {rowFocused["TenPhieu"]?.ToString()} không?",
                "Thông báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

                if (messResult != DialogResult.Yes) return;
                string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
                ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
                objXetDuyet.Action = "XacNhanMuahang";
                objXetDuyet.MaPhieu = rowFocused["MaPhieuMH"]?.ToString();
                objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                objXetDuyet.IsXacNhan = !IsXacNhan;
                objXetDuyet.NguoiXN = GlobleData.UserName;
                objXetDuyet.GhiChu = "";

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    this.DialogResult = DialogResult.OK;
                    LoadPhieuMuaHang();
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để xác nhận mua thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

        }

        private void grvPhieuMHVatTu_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {

            GridView view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;

            if (e.Column.FieldName == "TrangThai")
            {
                string trangThai = view.GetRowCellValue(e.RowHandle, "TrangThai")?.ToString();

                if (string.IsNullOrEmpty(trangThai)) return;
                switch (trangThai)
                {
                    case "Đang xử lý":
                        e.Appearance.BackColor = Color.Orange;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    case "Đã xác nhận":
                        e.Appearance.BackColor = Color.LightGreen;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    case "Thành công":
                        e.Appearance.BackColor = Color.LightSkyBlue;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    case "Đã hủy":
                        e.Appearance.BackColor = Color.Red;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    default:
                        break;
                }
            }

            if (e.Column.FieldName == "TongTien")
            {
                e.Appearance.ForeColor = Color.Red;
            }

            if (e.Column.FieldName == "MaTienTe")
            {
                e.Appearance.ForeColor = Color.Red;
            }

            if (e.Column.FieldName == "KetQuaKiem")
            {
                string ketqua = view.GetRowCellValue(e.RowHandle, e.Column.FieldName)?.ToString()?.Trim();

                if (string.IsNullOrEmpty(ketqua))
                    return;

                switch (ketqua.ToUpper())
                {
                    case "PASS":
                        e.Appearance.BackColor = Color.SkyBlue;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    case "FAIL":
                        e.Appearance.BackColor = Color.OrangeRed;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    default:
                        break;
                }
            }
        }


        #region DotGH

        #endregion

        private void grvPhieuMHVatTu_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            GridView gridView = sender as GridView;

            if (gridView == null) return;

            if (!e.HitInfo.InRow) return;

            DataRow rowFocused = (gridView.GetFocusedRow() as DataRowView).Row;

            if (rowFocused == null) return;

            if (e.Menu == null)
                return;
            e.Menu.Items.Clear();

            if (e.HitInfo.InRow)
            {
                DevExpress.Utils.Menu.DXMenuItem menuCoppyPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Xem theo dõi mua hàng", PhieuMHV2_Click);
                e.Menu.Items.Add(menuCoppyPasteItem);

                DevExpress.Utils.Menu.DXMenuItem menuSimpleSpiment = new DevExpress.Utils.Menu.DXMenuItem("Simple Shipment", menuSimpleSpiment_Click);
                e.Menu.Items.Add(menuSimpleSpiment);
            }
        }

        private void PhieuMHV2_Click(object sender, EventArgs e)
        {
            DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
            if (rowFocused != null)
            {
                string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                if (string.IsNullOrEmpty(MaPhieuMH))
                {
                    XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để xem", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                frmERPPhieuMuaHangV2 frm = new frmERPPhieuMuaHangV2(rowFocused, true);
                frm.ShowDialog();
                frm.MaximizeBox = true;
                if (frm.DialogResult == DialogResult.OK)
                {
                    LoadPhieuMuaHang();
                }
            }

        }

        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool isAdmin = string.Equals(
                GlobleData.UserName?.ToString(),
                "admin",
                StringComparison.OrdinalIgnoreCase
            );
            if (!isAdmin)
            {
                string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                    r["ModuleID"]?.ToString() == "M.12.03.00" &&
                    r["AllowDuyet"] != DBNull.Value &&
                    Convert.ToBoolean(r["AllowDuyet"]) == true
                );

                if (!coQuyenDuyet)
                {
                    XtraMessageBox.Show("Bạn không có quyền hủy duyệt phiếu mua hàng!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
            {
                if (grvPhieuMHVatTu.FocusedRowHandle >= 0)
                {
                    DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                    if (rowFocused != null)
                    {
                        _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                        string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                        string nguoitao = rowFocused["NguoiTao"]?.ToString();
                        string ghiChuKetThucSom = "";
                        if (GlobleData.UserName.ToString().ToUpper() == "QLDH_01" || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
                        {
                            if (string.IsNullOrEmpty(MaPhieuMH))
                            {
                                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            bool.TryParse(rowFocused["IsKetThuc"]?.ToString(), out bool Isketthuc);
                            if (Isketthuc)
                            {
                                XtraMessageBox.Show("Phiếu đã mua hàng thành công, không thể hủy", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                //LoadPhieuMuaHang();
                                return;
                            }
                            string trangThai = rowFocused["TrangThai"]?.ToString();
                            string user = GlobleData.UserName.ToString().ToUpper();
                            if (user != "ADMIN" && user != "QLDH_01")
                            {
                                if (trangThai == "Đã hủy")
                                {
                                    XtraMessageBox.Show(
                                        "Phiếu đã hủy",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning
                                    );
                                    return;
                                }

                                bool.TryParse(rowFocused["IsDuyet2"]?.ToString(), out bool IsDuyet2);
                                if (IsDuyet2)
                                {
                                    XtraMessageBox.Show("Phiếu đã được duyệt 2, không thể hủy", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    //LoadPhieuMuaHang();
                                    return;
                                }

                            }

                            //if (trangThai == "Thành công")
                            //{
                            //    XtraMessageBox.Show("Phiếu đã mua hàng thành công, không thể hủy", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //    LoadPhieuMuaHang();
                            //    return;
                            //}
                            bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);

                            if (IsDuyet)
                            {
                                DialogResult rs = XtraMessageBox.Show(
                                    "Phiếu đã duyệt, bạn có muốn hủy?",
                                    "Thông báo",
                                    MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Warning
                                );

                                if (rs != DialogResult.OK)
                                {
                                    //LoadPhieuMuaHang();
                                    return;
                                }
                            }
                            else
                            {
                                DialogResult rs = XtraMessageBox.Show(
                                    $"Bạn có muốn hủy {rowFocused["TenPhieu"]} không?",
                                    "Thông báo",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question
                                );

                                if (rs != DialogResult.Yes)
                                {
                                    return;
                                }

                            }
                            using (frmNGCKetThucPMHSom frm = new frmNGCKetThucPMHSom())
                            {
                                if (frm.ShowDialog() != DialogResult.OK)
                                {
                                    return;
                                }

                                ghiChuKetThucSom = frm.GhiChu;
                            }

                            string url = $"{URL}NhaCC/GePMH?action=GETPMHXH&para1={MaPhieuMH}&para2=&para3=&para4=&para5=";
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                            DataTable _dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                            DataTable tblDG = CreateDatatableToSave();
                            DateTime today = DateTime.Today;
                            //_dtTable.Columns["NgayDuKienHV"].DataType = typeof(string);
                            foreach (DataRow dr in _dtTable.Rows)
                            {
                                dr["IsXacNhan"] = 1;
                                int soSom = int.TryParse(dr["SoNgayGHSom"]?.ToString(), out int s1) ? s1 : 0;
                                int soTre = int.TryParse(dr["SoNgayGHTre"]?.ToString(), out int s2) ? s2 : 0;

                                DateTime ngaySom = today.AddDays(soSom);
                                DateTime ngayTre = today.AddDays(soSom + soTre);

                                DataRow newRow = tblDG.NewRow();
                                newRow["ID"] = 0;
                                newRow["MaPhieuMH"] = dr["MaPhieuMH"];
                                newRow["TenPhieu"] = dr["TenPhieu"];
                                newRow["MaNCC"] = dr["MaNCC"];
                                newRow["MaVTID"] = dr["MaVTID"]?.ToString();
                                newRow["MaVT"] = dr["MaVT"]?.ToString();
                                newRow["ChiTiet"] = dr["ChiTiet"];
                                newRow["NPL"] = dr["NPL"];
                                newRow["SoLuongMuaThem"] = dr["SoLuongMuaThem"];
                                newRow["DonGia"] = dr["DonGia"];
                                newRow["ThanhTien"] = dr["ThanhTien"];
                                newRow["MaPhieuBG"] = dr["MaPhieuBG"];
                                newRow["MauVTID"] = dr["MauVTID"];
                                newRow["KhoVaiID"] = dr["KhoVaiID"];
                                newRow["MaDVVT"] = dr["MaDVVT"];
                                newRow["NgayTao"] = Convert.ToDateTime(dr["NgayTao"]).ToString("yyyy-MM-dd");
                                newRow["NguoiTao"] = dr["NguoiTao"];
                                newRow["IsDuyet"] = 0;
                                newRow["MaCLVT"] = dr["MaCLVT"];
                                newRow["TienTeID"] = dr["TienTeID"];
                                newRow["NgaySua"] = DateTime.Now.ToString("yyyy-MM-dd");
                                newRow["NguoiSua"] = GlobleData.UserName.ToString();
                                newRow["TrangThai"] = "";
                                newRow["NgayDuKienHV"] = $"{ngaySom:dd/MM/yyyy} - {ngayTre:dd/MM/yyyy}";
                                object ngayXNObj = clsForrmatUtils.ConvertDate(dr["NgayXacNhan"]);

                                if (ngayXNObj == null || ngayXNObj == DBNull.Value)
                                {
                                    newRow["NgayXacNhan"] = DBNull.Value;
                                }
                                else
                                {
                                    DateTime ngayXN = Convert.ToDateTime(ngayXNObj);

                                    // ❗ Chặn 01/01/0001
                                    if (ngayXN == DateTime.MinValue)
                                        newRow["NgayXacNhan"] = DBNull.Value;
                                    else
                                        newRow["NgayXacNhan"] = ngayXN.Date; // ✅ DATE SQL (yyyy-MM-dd)
                                }
                                //object ngayXN = clsForrmatUtils.ConvertDate(dr["NgayXacNhan"]);
                                //newRow["NgayXacNhan"] = ngayXN ?? DBNull.Value;
                                newRow["GhiChu"] = dr["GhiChu"];
                                newRow["PTVanChuyen"] = dr["PTVanChuyen"];
                                newRow["PTThanhToan"] = dr["PTThanhToan"];
                                newRow["Thue"] = dr["Thue"];
                                newRow["CPVanChuyen"] = dr["CPVanChuyen"];
                                newRow["ChiPhiKhac"] = "";
                                newRow["Dot"] = dr["Dot"];
                                newRow["IsXacNhan"] = 0;
                                object ngayHLObj = clsForrmatUtils.ConvertDate(dr["NgayHieuLuc"]);

                                if (ngayHLObj == null || ngayHLObj == DBNull.Value)
                                {
                                    newRow["NgayHieuLuc"] = DBNull.Value;
                                }
                                else
                                {
                                    DateTime ngayHL = Convert.ToDateTime(ngayHLObj);

                                    // ❗ Chặn 01/01/0001
                                    if (ngayHL == DateTime.MinValue)
                                        newRow["NgayHieuLuc"] = DBNull.Value;
                                    else
                                        newRow["NgayHieuLuc"] = ngayHL.Date; // ✅ DATE SQL (yyyy-MM-dd)
                                }
                                newRow["ChiPhiVT"] = dr["ChiPhiVT"];
                                newRow["POMua"] = "";
                                newRow["TyGiaThanhToan"] = 0;
                                newRow["MaTienTePhieuMH"] = "";
                                newRow["ChietKhau"] = "";
                                newRow["TTChietKhau"] = "";
                                newRow["NgayGiaoHangYC"] = DBNull.Value;

                                tblDG.Rows.Add(newRow);
                            }


                            string url2 = string.Format("{0}?", URL + "NhaCC/PospHuy");
                            string mss1 = Task.Run(async () =>
                            {
                                return await _clientExtension.PostAsync(url2, tblDG);
                            }).Result;

                            POSTHUY(ghiChuKetThucSom);
                            if (mss1.ToLower() == "true")
                            {
                                clsWaitForm.ShowSuccessForm(this, 2000);
                                this.DialogResult = DialogResult.OK;
                                //LoadPhieuMuaHang();
                                LoadDataForCurrentTab();
                            }
                            else
                            {
                                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("bạn không phải là người được phân quyền hủy, không có quyền hủy. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                }
                else
                {
                    XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để hủy", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (xtraTabControl1.SelectedTabPage == xtraTabPage3)
            {
                if (gridView5.FocusedRowHandle >= 0)
                {
                    DataRow rowFocused = gridView5.GetFocusedDataRow() as DataRow;
                    if (rowFocused != null)
                    {
                        _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                        string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                        string urlCheck = $"{URL}ERP_POMH_PhieuMuaHangMMTB_Import_Seri/GET?action=CheckUseMMTB&para1={MaPhieuMH}&para2=NONE";
                        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCheck); }).Result;
                        if (json != "[]")
                        {
                            XtraMessageBox.Show($"Vật tư phiếu mua này đã phát sinh dữ liệu không thể hủy!",
                                "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            return;
                        }
                        string nguoitao = rowFocused["NguoiTao"]?.ToString();
                        if (GlobleData.UserName.ToString().ToUpper() == nguoitao?.ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
                        {
                            if (string.IsNullOrEmpty(MaPhieuMH))
                            {
                                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            string trangThai = rowFocused["TrangThai"]?.ToString();
                            if (GlobleData.UserName.ToString().ToUpper() != "ADMIN")
                            {
                                if (trangThai == "Đã hủy")
                                {
                                    XtraMessageBox.Show(
                                        "Phiếu đã hủy",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning
                                    );
                                    return;
                                }

                                bool.TryParse(rowFocused["IsDuyet2"]?.ToString(), out bool IsDuyet2);
                                if (IsDuyet2)
                                {
                                    XtraMessageBox.Show("Phiếu đã được duyệt 2, không thể hủy", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    //LoadPhieuMuaHang();
                                    return;
                                }
                                bool.TryParse(rowFocused["IsKetThuc"]?.ToString(), out bool Isketthuc);
                                if (Isketthuc)
                                {
                                    XtraMessageBox.Show("Phiếu đã mua hàng thành công, không thể hủy", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    //LoadPhieuMuaHang();
                                    return;
                                }
                            }

                            //if (trangThai == "Thành công")
                            //{
                            //    XtraMessageBox.Show("Phiếu đã mua hàng thành công, không thể hủy", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //    LoadPhieuMuaHang();
                            //    return;
                            //}
                            bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);

                            if (IsDuyet)
                            {
                                DialogResult rs = XtraMessageBox.Show(
                                    "Phiếu đã duyệt, bạn có muốn hủy?",
                                    "Thông báo",
                                    MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Warning
                                );

                                if (rs != DialogResult.OK)
                                {
                                    //LoadPhieuMuaHang();
                                    LoadDataForCurrentTab();
                                    return;
                                }
                            }
                            else
                            {
                                DialogResult rs = XtraMessageBox.Show(
                                    $"Bạn có muốn hủy {rowFocused["TenPhieu"]} không?",
                                    "Thông báo",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question
                                );

                                if (rs != DialogResult.Yes)
                                {
                                    return;
                                }
                            }

                            POSTHUYMMTB();
                        }
                        else
                        {
                            XtraMessageBox.Show("bạn không phải là người tạo, không có quyền hủy. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                }
                else
                {
                    XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để hủy", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            }



        }


        //vattucanmua
        #region vật tư cần mua
        private void loadGridControl1()
        {

            gridControl1.DataSource = gridControl1Table;
            foreach (GridColumn col in gridView1.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            gridView1.ExpandAllGroups();
        }
        private void getVatTuCanMua()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getvattucanmua";
            string urlGetListDataTable = URL + "VatTuMinMax/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            vattuTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!vattuTable.Columns.Contains("generatedID"))
                vattuTable.Columns.Add("generatedID");
            if (!vattuTable.Columns.Contains("IsNPLText"))
                vattuTable.Columns.Add("IsNPLText", typeof(string));
            if (!vattuTable.Columns.Contains("haveBought"))
                vattuTable.Columns.Add("haveBought", typeof(string));
            if (!vattuTable.Columns.Contains("Sort_ChungLoaiVatTu"))
                vattuTable.Columns.Add("Sort_ChungLoaiVatTu", typeof(string));
            foreach (DataRow row in vattuTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");

                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";

                row["Sort_ChungLoaiVatTu"] = padToThree(row["Sort"].ToString()) + "--" + row["ChungLoaiVatTu"].ToString();

                row["haveBought"] = row["MaPhieuMH"] == null || row["MaPhieuMH"].ToString() == "" ? "Chưa tạo phiếu mua hàng" : "Đã tạo phiếu mua hàng";
            }
        }
        private void calcVatTuCanMua()
        {
            DataTable dt = vattuTable.Copy();
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count == 0) return;
            foreach (DataRow row in dt.Rows)
            {
                row["TonToiThieu"] = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiThieu"].ToString());
                row["TonToiDa"] = XuLyVTUnits.SmartTryParse<decimal>(row["TonToiDa"].ToString());
                row["TonKho"] = XuLyVTUnits.SmartTryParse<decimal>(row["TonKho"].ToString());
            }
            gridControl1Table = dt.Copy();
        }
        public string padToThree(string input)
        {
            // Nếu input null thì trả về rỗng
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Dùng PadLeft để thêm '0' cho đủ 3 ký tự
            return input.PadLeft(3, '0');
        }
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }
        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 192, 128)))
            {
                e.Graphics.FillRectangle(brush, e.Info.Bounds);
            }

            // Vẽ border quanh cột header
            e.Graphics.DrawRectangle(Pens.Gray, e.Info.Bounds);

            // Vẽ text trong vùng caption
            TextRenderer.DrawText(e.Graphics, e.Info.Caption, e.Appearance.Font,
                                  e.Info.Bounds, Color.Black,
                                  TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

            e.Handled = true;
        }
        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.CellValue == null || e.CellValue == DBNull.Value) return;

            GridView view = sender as GridView;
            if (e.Column.FieldName == "TonToiThieu")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
                else
                {
                    e.Appearance.ForeColor = Color.Blue;
                }
            }
            else if (e.Column.FieldName == "TonToiDa")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "TonKho")
            {
                object objTonKho = view.GetRowCellValue(e.RowHandle, "TonKho");
                object objTonMin = view.GetRowCellValue(e.RowHandle, "TonToiThieu");

                if (objTonKho == null || objTonKho == DBNull.Value ||
                    objTonMin == null || objTonMin == DBNull.Value)
                    return;

                decimal tonkho = Convert.ToDecimal(objTonKho);
                decimal tonmin = Convert.ToDecimal(objTonMin);

                if (tonkho == 0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
                else if (tonkho < tonmin)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
                else
                {
                    e.Appearance.ForeColor = Color.Green;
                }
            }
        }
        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (info != null)
            {
                string originalText = info.GroupText;
                string[] parts = originalText.Split(new string[] { "--" }, StringSplitOptions.None);

                if (parts.Length > 1)
                {
                    info.GroupText = parts[1].Trim();
                }

                e.Painter.DrawObject(info);
                e.Handled = true;
            }
        }

        #endregion

        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //frmPhieuMuaHang frm = new frmPhieuMuaHang(null, false, true, false);
            //frm.ShowDialog();
            //frm.MaximizeBox = true;
            //if (frm.DialogResult == DialogResult.OK)
            //{
            //    LoadPhieuMuaHang();

            //    SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
            //    try
            //    {
            //        getVatTuCanMua();
            //        calcVatTuCanMua();
            //        loadGridControl1();
            //        xtraTabControl1.SelectedTabPage = xtraTabPage1;
            //    }
            //    catch (Exception ex)
            //    {
            //        XtraMessageBox.Show(ex.Message, "Lỗi",
            //                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        SplashScreenManager.CloseDefaultWaitForm();
            //    }
            //    finally
            //    {
            //        SplashScreenManager.CloseDefaultWaitForm();
            //    }
            //}
            btthemdh();
        }

        private void btthemdh()
        {
            frmPhieuMuaHang frm = new frmPhieuMuaHang(null, false, true, false);
            frm.Show();
            frm.MaximizeBox = true;
            frm.FormClosed += (s, e) =>
            {
                //LoadPhieuMuaHang();

                //SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
                //try
                //{
                //    getVatTuCanMua();
                //    calcVatTuCanMua();
                //    loadGridControl1();
                //}
                //catch (Exception ex)
                //{
                //    XtraMessageBox.Show(ex.Message, "Lỗi",
                //                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    SplashScreenManager.CloseDefaultWaitForm();
                //}
                //finally
                //{
                //    SplashScreenManager.CloseDefaultWaitForm();
                //}
                LoadDataForCurrentTab();
            };
        }

        private void barButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btthemvt();
        }

        private void btthemvt()
        {
            frmPhieuMuaHang frm = new frmPhieuMuaHang(null, false, true, false, true);
            //frm.ShowDialog();
            frm.Show();
            frm.FormClosed += (s, e) =>
            {
                //LoadPhieuMuaHang();

                //SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
                //try
                //{
                //    getVatTuCanMua();
                //    calcVatTuCanMua();
                //    loadGridControl1();
                //}
                //catch (Exception ex)
                //{
                //    XtraMessageBox.Show(ex.Message, "Lỗi",
                //                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    SplashScreenManager.CloseDefaultWaitForm();
                //}
                //finally
                //{
                //    SplashScreenManager.CloseDefaultWaitForm();
                //}
                LoadDataForCurrentTab();
            };

            //if (frm.DialogResult == DialogResult.OK)
            //{
            //    LoadPhieuMuaHang();

            //    SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
            //    try
            //    {
            //        getVatTuCanMua();
            //        calcVatTuCanMua();
            //        loadGridControl1();
            //    }
            //    catch (Exception ex)
            //    {
            //        XtraMessageBox.Show(ex.Message, "Lỗi",
            //                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        SplashScreenManager.CloseDefaultWaitForm();
            //    }
            //    finally
            //    {
            //        SplashScreenManager.CloseDefaultWaitForm();
            //    }
            //}
        }

        private void barButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

                DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                if (rowFocused == null) return;

                SaveFileDialog Sfd = new SaveFileDialog();
                Sfd.Title = "File To Save";
                //Sfd.Filter = "Excel | *.xlsx";
                Sfd.Filter = "PDF Files | *.pdf|Excel Files | *.xlsx";
                Sfd.FileName = string.Format("DonHang{0}_{1}" + DateTime.Now.ToString("ddMMyyyy"), rowFocused["MaPhieuMH"].ToString(), DateTime.Now.Millisecond.ToString());
                if (Sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (xtraTabControl1.SelectedTabPage == xtraTabPage1)
                        {
                            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                            DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
                            op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                            op.ShowGridLines = true;
                            op.SheetName = string.Format("Bao Cao");

                            string fileName = "MauPhieuMuaHang.xlsx";
                            string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                            string TemplateFileName = path;
                            string ExportFileName = Sfd.FileName;

                            string tempExcel = Path.Combine(
                                Path.GetTempPath(),
                                Guid.NewGuid().ToString("N") + ".xlsx"
                            );


                            ExportExcelLSX(TemplateFileName, tempExcel, rowFocused);

                            if (Path.GetExtension(Sfd.FileName).ToLower() == ".pdf")
                            {
                                ConvertExcelToPDF(tempExcel, Sfd.FileName);
                                File.Delete(tempExcel);
                            }
                            else
                            {
                                File.Copy(tempExcel, Sfd.FileName, true);
                                File.Delete(tempExcel);
                            }


                            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                            if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn mở tài liệu này không?", "Thông báo",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                            }
                        }
                        else if (xtraTabControl1.SelectedTabPage == xtraTabPage3)
                        {
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                    }
                }

            }
            catch (Exception ex)
            {
                return;
            }
        }


        public void ExportExcelLSX(string TemplateFileName, string ExportFileName, DataRow rows)
        {
            try
            {
                string MaPhieuMH = rows["MaPhieuMH"]?.ToString();
                bool IsDuyet1 = Convert.ToBoolean(rows["IsDuyet1"]);
                bool IsDuyet2 = Convert.ToBoolean(rows["IsDuyet2"]);
                string matiente = rows["MaTienTe"]?.ToString();
                string tenkh = rows["TenKH"]?.ToString();
                string diachi = rows["DiaChi"]?.ToString();
                string mail = rows["Mail"]?.ToString();
                string pomh = rows["POMua"]?.ToString();
                string ngaytao = rows["NgayTao"]?.ToString();
                string manhacc = rows["MaNCC"]?.ToString();
                string nguoitao = rows["NguoiTao"]?.ToString();

                //check tên
                string urlNV = string.Format("{0}?", URL + "NhanVien/GetAllNV");
                string jsonNV = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNV); }).Result;
                DataTable tblnv = JsonConvert.DeserializeObject<DataTable>(jsonNV);
                DataRow ten = tblnv.AsEnumerable().FirstOrDefault(r => string.Equals(r.Field<string>("UserID")?.Trim(), nguoitao.Trim(), StringComparison.OrdinalIgnoreCase));
                string hoten = ten != null ? ten["TenNV"].ToString() + "" + ten["Ten"].ToString() : nguoitao.ToString();

                //string urlMau = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                string url = $"{URL}NhaCC/GePMH?action=GETPMHExcel&para1={MaPhieuMH}&para2=&para3=&para4=&para5=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                string urlncc = $"{URL}NhaCC/GePMH?action=GETNCCExcel&para1={manhacc}&para2=&para3=&para4=&para5=";
                string jsonncc = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlncc); }).Result;
                DataTable tblncc = JsonConvert.DeserializeObject<DataTable>(jsonncc);

                string termOfdelivery = tblncc.Rows[0]["Deliveryterms"].ToString();
                string termOfpayment = tblncc.Rows[0]["TermsOfPayment"].ToString();
                string company = tblncc.Rows[0]["Company"].ToString();
                string adress = tblncc.Rows[0]["Address"].ToString();
                string sales = tblncc.Rows[0]["Sales"].ToString();
                string tax = tblncc.Rows[0]["TAX"].ToString();
                string chietkhau = tbl.Rows[0]["TTChietKhau"].ToString();
                DataRow _row = rows;
                System.IO.FileInfo file = new System.IO.FileInfo(TemplateFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    string templateFilePath = TemplateFileName;

                    string resultFilePath = ExportFileName;
                    FileInfo templateFile = new FileInfo(templateFilePath);
                    ExcelPackage templatePackage = new ExcelPackage(templateFile);
                    ExcelWorksheet worksheet = templatePackage.Workbook.Worksheets[0];
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.Font.Name = "Montserrat";
                    int dataStartRow = 16;
                    int templateStartRow = 16;

                    int dataCount = tbl.Rows.Count;
                    DateTime now = DateTime.Now;
                    // col Info
                    worksheet.Cells[11, 2].Value = tenkh;
                    worksheet.Cells[12, 2, 12, 4].Merge = true;

                    worksheet.Cells[12, 2].Value = sales;

                    worksheet.Cells[12, 2].Style.WrapText = true;
                    worksheet.Cells[12, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Row(12).Height = CalcRowHeight(sales, 80);
                    worksheet.Cells[13, 2, 13, 4].Merge = true;
                    worksheet.Cells[13, 2].Value = mail;
                    worksheet.Cells[13, 2].Style.WrapText = true;
                    worksheet.Cells[13, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Row(13).Height = CalcRowHeight(mail, 80);

                    // col POMH
                    worksheet.Cells[5, 8].Value = pomh.ToString();
                    worksheet.Cells[6, 8].Value = ngaytao.ToString();
                    worksheet.Cells[7, 8].Value = termOfdelivery;
                    worksheet.Cells[8, 8].Value = termOfpayment;
                    worksheet.Cells[9, 8].Value = chietkhau;
                    worksheet.Cells[10, 8].Value = company;
                    worksheet.Cells[11, 8, 11, 9].Merge = true;
                    worksheet.Cells[11, 8].Value = adress;
                    worksheet.Cells[11, 8].Style.WrapText = true;
                    worksheet.Cells[11, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    worksheet.Row(11).Height = CalcRowHeight(adress, 49);
                    worksheet.Cells[13, 8].Value = tax;

                    if (dataCount > 0)
                    {
                        worksheet.InsertRow(templateStartRow, dataCount);
                    }

                    int row = dataStartRow;
                    int stt = 1;
                    decimal tongtienvat = 0;

                    foreach (DataRow item in tbl.Rows)
                    {
                        string tienTephieu = item["MaTienTePhieuMH"]?.ToString() ?? "VND";
                        string tientedong = item["TienTeID"]?.ToString() ?? "VND";

                        // 🔹 Tỷ giá của DÒNG → VND
                        decimal tyGiaphieu = GetTyGia_ToVND(tienTephieu);
                        decimal tyGiadong = GetTyGia_ToVND(tientedong);
                        decimal thanhtien = Convert.ToDecimal(item["ThanhTien"]);
                        decimal thanhtienvnd = thanhtien * tyGiadong;
                        decimal thanhtienquydoi = thanhtienvnd / tyGiaphieu;

                        decimal chiphiVT = Convert.ToDecimal(item["ChiPhiVT"]);
                        decimal chiphivnd = chiphiVT * tyGiadong;
                        decimal chiphiquydoi = chiphivnd / tyGiaphieu;

                        worksheet.Cells[row, 1].Value = stt++;
                        worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[row, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, 2].Value = item["ItemCode"].ToString();
                        worksheet.Cells[row, 2].Style.WrapText = true;
                        //worksheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, 3, row, 4].Merge = true;
                        worksheet.Cells[row, 3].Value = item["Mota"].ToString();
                        worksheet.Cells[row, 3].Style.WrapText = true;
                        worksheet.Cells[row, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[row, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, 5].Value = item["ColorCode"].ToString();
                        worksheet.Cells[row, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, 5].Style.WrapText = true;
                        worksheet.Cells[row, 6].Value = item["KhoVai"].ToString();
                        worksheet.Cells[row, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, 7].Value = item["TenDVVT"].ToString();
                        worksheet.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[row, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, 8].Value = Convert.ToDecimal(item["TongSLMuaThem"]);
                        worksheet.Cells[row, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[row, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[row, 9].Value = Convert.ToDecimal(item["DonGia"]);
                        worksheet.Cells[row, 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[row, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[row, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, 10].Value = chiphiquydoi;//thanhtienquydoi;
                        worksheet.Cells[row, 10].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[row, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[row, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, 11].Value = "";
                        worksheet.Cells[row, 12].Value = item["GhiChu"].ToString();
                        worksheet.Cells[row, 12].Style.WrapText = true;
                        //worksheet.Cells[row, 16].Value = item["ThanhTienVND"].ToString();
                        tongtienvat += thanhtienquydoi;
                        row++;

                    }

                    int rowStart = dataStartRow;
                    //int rowStart = 11;
                    int rowEnd = row;
                    //int rowEnd = row;
                    int totalRow = rowEnd + 1;
                    int vatRow = totalRow + 1;
                    int paymentrow = vatRow + 3;

                    worksheet.Cells[totalRow, 8].Formula = $"SUM(H{rowStart}:H{rowEnd})";
                    worksheet.Cells[totalRow, 8].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[totalRow, 10].Formula = $"SUM(J{rowStart}:J{rowEnd})";
                    worksheet.Cells[totalRow, 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[vatRow, 10].Value = tongtienvat; //$"H{totalRow}*1.1";
                    worksheet.Cells[vatRow, 10].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[vatRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[totalRow, 11].Value = matiente;
                    worksheet.Cells[vatRow, 11].Value = matiente;
                    //worksheet.Cells[paymentrow, 2].Value = "2.Payment term: " +tbl.Rows[0]["ThanhToan"].ToString(); //$"H{totalRow}*1.1";
                    //worksheet.Cells[paymentrow, 6].Value = "2.Thanh toán: " + tbl.Rows[0]["ThanhToan"].ToString(); //$"H{totalRow}*1.1";
                    worksheet.Cells[totalRow, 8].Style.Font.Bold = true;
                    worksheet.Cells[totalRow, 10].Style.Font.Bold = true;
                    worksheet.Cells[vatRow, 10].Style.Font.Bold = true;

                    var dataRange = worksheet.Cells[rowStart, 1, rowEnd, 12];
                    dataRange.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    dataRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);


                    var totalRange = worksheet.Cells[totalRow, 1, totalRow, 11];
                    totalRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    totalRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    totalRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    totalRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    int signTitleRow = vatRow + 9;
                    int signContentRow = signTitleRow + 2;
                    int namerow = signContentRow + 1;

                    if (IsDuyet2)
                    {
                        DateTime ngayDuyet = Convert.ToDateTime(rows["NgayDuyet2"]);
                        string ngayKy1 = ngayDuyet.ToString("dd/MM/yyyy");
                        //worksheet.Cells[signContentRow, 3, signContentRow + 1, 4].Merge = true;
                        worksheet.Cells[signContentRow, 2].Value =
                            $"Ký bởi: CÔNG TY TNHH VIKING VIỆT NAM\nKý ngày: {ngayKy1}";
                        worksheet.Cells[signContentRow, 2].Style.WrapText = true;
                        worksheet.Cells[signContentRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[signContentRow, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[signContentRow, 2].Style.Font.Italic = true;
                        worksheet.Cells[signContentRow, 2].Style.Font.Color.SetColor(Color.DimGray);

                        worksheet.Cells[signContentRow - 1, 2].Value = "✔";
                        worksheet.Cells[signContentRow - 1, 2].Style.Font.Size = 18;
                        worksheet.Cells[signContentRow - 1, 2].Style.Font.Color.SetColor(Color.Green);
                        worksheet.Cells[signContentRow - 1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    if (IsDuyet1)
                    {
                        // ---- Approved (F:G)
                        //worksheet.Cells[signContentRow, 6, signContentRow + 1, 7].Merge = true;
                        DateTime ngayDuyet2 = Convert.ToDateTime(rows["NgayDuyet1"]);
                        string ngayKy2 = ngayDuyet2.ToString("dd/MM/yyyy");
                        worksheet.Cells[signContentRow, 4].Value =
                            $"Ký bởi: CÔNG TY TNHH \nVIKING VIỆT NAM\nKý ngày: {ngayKy2}";
                        worksheet.Cells[signContentRow, 4].Style.WrapText = true;
                        worksheet.Cells[signContentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[signContentRow, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[signContentRow, 4].Style.Font.Italic = true;
                        worksheet.Cells[signContentRow, 4].Style.Font.Color.SetColor(Color.DimGray);

                        worksheet.Cells[signContentRow - 1, 4].Value = "✔";
                        worksheet.Cells[signContentRow - 1, 4].Style.Font.Size = 18;
                        worksheet.Cells[signContentRow - 1, 4].Style.Font.Color.SetColor(Color.Green);
                        worksheet.Cells[signContentRow - 1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    worksheet.Cells[namerow, 10].Value = $"{hoten}";

                    worksheet.PrinterSettings.PaperSize = ePaperSize.A4;
                    worksheet.PrinterSettings.Orientation = eOrientation.Portrait;

                    worksheet.PrinterSettings.FitToPage = true;
                    worksheet.PrinterSettings.FitToWidth = 1;
                    worksheet.PrinterSettings.FitToHeight = 1;

                    worksheet.PrinterSettings.PrintArea = worksheet.Cells[
                        1,
                        1,
                        worksheet.Dimension.End.Row,
                        worksheet.Dimension.End.Column
                    ];

                    worksheet.PrinterSettings.TopMargin = 0.5M;
                    worksheet.PrinterSettings.BottomMargin = 0.5M;
                    worksheet.PrinterSettings.LeftMargin = 0.5M;
                    worksheet.PrinterSettings.RightMargin = 0.5M;

                    FileInfo resultFile = new FileInfo(resultFilePath);
                    templatePackage.SaveAs(resultFile);
                }
            }
            catch (Exception ex)
            {
                return;
            }


        }

        private double CalcRowHeight(string text, int approxCharsPerLine, double lineHeight = 18)
        {
            if (string.IsNullOrEmpty(text)) return lineHeight;

            int lines = (int)Math.Ceiling((double)text.Length / approxCharsPerLine);
            return Math.Max(lineHeight, lines * lineHeight);
        }

        //public void ConvertExcelToPDF(string excelPath, string pdfPath)
        //{
        //    Workbook workbook = new Workbook();
        //    workbook.LoadDocument(excelPath);

        //    PdfExportOptions options = new PdfExportOptions()
        //    {
        //        ImageQuality = PdfJpegImageQuality.High,
        //        ConvertImagesToJpeg = false
        //    };

        //    workbook.ExportToPdf(pdfPath, options);
        //}

        public void ConvertExcelToPDF(string excelPath, string pdfPath)
        {
            Workbook workbook = new Workbook();
            workbook.LoadDocument(excelPath);

            foreach (Worksheet sheet in workbook.Worksheets)
            {
                WorksheetView view = sheet.ActiveView;

                // CHIỀU NGANG
                view.Orientation = PageOrientation.Landscape;

                // Khổ giấy
                view.PaperKind = PaperKind.A4;
            }

            PdfExportOptions options = new PdfExportOptions()
            {
                ImageQuality = PdfJpegImageQuality.High,
                ConvertImagesToJpeg = false
            };

            workbook.ExportToPdf(pdfPath, options);

        }

        private void grvPhieuMHVatTu_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "NguoiTao")
            {
                bool isChecked = Convert.ToBoolean(
                    view.GetRowCellValue(e.RowHandle, "IsDuyet")
                );

                //string text = e.CellValue?.ToString();
                string text = GetTenNV(e.CellValue?.ToString());

                CheckEditViewInfo checkInfo =
                    new CheckEditViewInfo(rItemCheckEditDuyet);

                CheckEditPainter painter = new CheckEditPainter();

                checkInfo.EditValue = isChecked;

                checkInfo.Bounds = new Rectangle(
                    e.Bounds.X + CHECK_LEFT,
                    e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
                    CHECK_SIZE,
                    CHECK_SIZE
                );

                checkInfo.CalcViewInfo(e.Graphics);

                ControlGraphicsInfoArgs args =
                    new ControlGraphicsInfoArgs(
                        checkInfo,
                        new GraphicsCache(e.Graphics),
                        checkInfo.Bounds
                    );

                painter.Draw(args);

                // Text
                Rectangle textRect = new Rectangle(
                    e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                );

                e.Appearance.DrawString(e.Cache, text, textRect);
                e.Handled = true;
            }

            if (e.Column.FieldName == "NguoiDuyet")
            {
                bool isChecked = Convert.ToBoolean(
                   view.GetRowCellValue(e.RowHandle, "IsDuyet2")
               );

                //string text = e.CellValue?.ToString();
                string text = GetTenNV(e.CellValue?.ToString());

                CheckEditViewInfo checkInfo =
                    new CheckEditViewInfo(rItemCheckEditXN);

                CheckEditPainter painter = new CheckEditPainter();

                checkInfo.EditValue = isChecked;

                checkInfo.Bounds = new Rectangle(
                    e.Bounds.X + CHECK_LEFT,
                    e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
                    CHECK_SIZE,
                    CHECK_SIZE
                );

                checkInfo.CalcViewInfo(e.Graphics);

                ControlGraphicsInfoArgs args =
                    new ControlGraphicsInfoArgs(
                        checkInfo,
                        new GraphicsCache(e.Graphics),
                        checkInfo.Bounds
                    );

                painter.Draw(args);

                // Text
                Rectangle textRect = new Rectangle(
                    e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                );

                e.Appearance.DrawString(e.Cache, text, textRect);
                e.Handled = true;
            }

            if (e.Column.FieldName == "NguoiDuyet1")
            {
                bool isChecked = Convert.ToBoolean(
                   view.GetRowCellValue(e.RowHandle, "IsDuyet1")
               );

                //string text = e.CellValue?.ToString();
                string text = GetTenNV(e.CellValue?.ToString());

                CheckEditViewInfo checkInfo =
                    new CheckEditViewInfo(rItemCheckEditDuyet1);

                CheckEditPainter painter = new CheckEditPainter();

                checkInfo.EditValue = isChecked;

                checkInfo.Bounds = new Rectangle(
                    e.Bounds.X + CHECK_LEFT,
                    e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
                    CHECK_SIZE,
                    CHECK_SIZE
                );

                checkInfo.CalcViewInfo(e.Graphics);

                ControlGraphicsInfoArgs args =
                    new ControlGraphicsInfoArgs(
                        checkInfo,
                        new GraphicsCache(e.Graphics),
                        checkInfo.Bounds
                    );

                painter.Draw(args);

                // Text
                Rectangle textRect = new Rectangle(
                    e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                );

                e.Appearance.DrawString(e.Cache, text, textRect);
                e.Handled = true;
            }

            if (e.Column.FieldName == "NguoiXacNhan")
            {
                bool isChecked = Convert.ToBoolean(
                   view.GetRowCellValue(e.RowHandle, "IsXacNhan")
               );

                //string text = e.CellValue?.ToString();
                string text = GetTenNV(e.CellValue?.ToString());

                CheckEditViewInfo checkInfo =
                    new CheckEditViewInfo(rItemCheckEditXacNhan);

                CheckEditPainter painter = new CheckEditPainter();

                checkInfo.EditValue = isChecked;

                checkInfo.Bounds = new Rectangle(
                    e.Bounds.X + CHECK_LEFT,
                    e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
                    CHECK_SIZE,
                    CHECK_SIZE
                );

                checkInfo.CalcViewInfo(e.Graphics);

                ControlGraphicsInfoArgs args =
                    new ControlGraphicsInfoArgs(
                        checkInfo,
                        new GraphicsCache(e.Graphics),
                        checkInfo.Bounds
                    );

                painter.Draw(args);

                // Text
                Rectangle textRect = new Rectangle(
                    e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                );

                e.Appearance.DrawString(e.Cache, text, textRect);
                e.Handled = true;
            }

            if (e.Column.FieldName == "NguoiKetThuc")
            {
                bool isChecked = Convert.ToBoolean(
                   view.GetRowCellValue(e.RowHandle, "IsKetThuc")
               );

                //string text = e.CellValue?.ToString();
                string text = GetTenNV(e.CellValue?.ToString());

                CheckEditViewInfo checkInfo =
                    new CheckEditViewInfo(rItemCheckEditKetThuc);

                CheckEditPainter painter = new CheckEditPainter();

                checkInfo.EditValue = isChecked;

                checkInfo.Bounds = new Rectangle(
                    e.Bounds.X + CHECK_LEFT,
                    e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
                    CHECK_SIZE,
                    CHECK_SIZE
                );

                checkInfo.CalcViewInfo(e.Graphics);

                ControlGraphicsInfoArgs args =
                    new ControlGraphicsInfoArgs(
                        checkInfo,
                        new GraphicsCache(e.Graphics),
                        checkInfo.Bounds
                    );

                painter.Draw(args);

                // Text
                Rectangle textRect = new Rectangle(
                    e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                );

                e.Appearance.DrawString(e.Cache, text, textRect);
                e.Handled = true;
            }
            if (e.Column.FieldName == "NguoiXNNK")
            {
                bool isChecked = Convert.ToBoolean(
                   view.GetRowCellValue(e.RowHandle, "IsNhapKho")
               );

                //string text = e.CellValue?.ToString();
                string text = GetTenNV(e.CellValue?.ToString());

                CheckEditViewInfo checkInfo =
                    new CheckEditViewInfo(rItemCheckEditXNNK);

                CheckEditPainter painter = new CheckEditPainter();

                checkInfo.EditValue = isChecked;

                checkInfo.Bounds = new Rectangle(
                    e.Bounds.X + CHECK_LEFT,
                    e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
                    CHECK_SIZE,
                    CHECK_SIZE
                );

                checkInfo.CalcViewInfo(e.Graphics);

                ControlGraphicsInfoArgs args =
                    new ControlGraphicsInfoArgs(
                        checkInfo,
                        new GraphicsCache(e.Graphics),
                        checkInfo.Bounds
                    );

                painter.Draw(args);

                // Text
                Rectangle textRect = new Rectangle(
                    e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                );

                e.Appearance.DrawString(e.Cache, text, textRect);
                e.Handled = true;
            }
        }

        private string GetTenNV(object userId)
        {
            if (userId == null) return string.Empty;

            string key = userId.ToString().Trim();

            if (_dicNhanVien != null && _dicNhanVien.TryGetValue(key, out string ten))
                return ten;

            return key; // fallback: hiện UserID
        }

        private void barButtonItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bool isAdmin = string.Equals(
                GlobleData.UserName?.ToString(),
                "admin",
                StringComparison.OrdinalIgnoreCase
            );
            if (!isAdmin)
            {
                string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                    r["ModuleID"]?.ToString() == "M.12.03.00" &&
                    r["AllowDuyet"] != DBNull.Value &&
                    Convert.ToBoolean(r["AllowDuyet"]) == true
                );

                if (!coQuyenDuyet)
                {
                    XtraMessageBox.Show("Bạn không có quyền duyệt phiếu mua hàng!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (grvPhieuMHVatTu.FocusedRowHandle >= 0)
            {
                DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                //string trangThai = rowFocused["TrangThai"]?.ToString();
                //switch (trangThai)
                //{
                //    case "Đã hủy":
                //        XtraMessageBox.Show(
                //            "Phiếu đã hủy",
                //            "Thông báo",
                //            MessageBoxButtons.OK,
                //            MessageBoxIcon.Warning
                //        );
                //        return;

                //    case "Đang xử lý":
                //        XtraMessageBox.Show(
                //            "Phiếu đã được duyệt, không được duyệt lại",
                //            "Thông báo",
                //            MessageBoxButtons.OK,
                //            MessageBoxIcon.Warning
                //        );
                //        return;

                //    case "Đã xác nhận":
                //    case "Thành công":
                //        XtraMessageBox.Show(
                //            "Phiếu đã xác nhận hoặc đã mua hàng thành công, không được duyệt lại",
                //            "Thông báo",
                //            MessageBoxButtons.OK,
                //            MessageBoxIcon.Warning
                //        );
                //        return;
                //}


                bool.TryParse(rowFocused["IsDuyet2"]?.ToString(), out bool IsDuyet);
                bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhan);
                if (IsXacNhan)
                {
                    XtraMessageBox.Show("Đã duyệt sẽ không hủy xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LoadPhieuMuaHang();
                    return;
                }
                DialogResult messResult = MessageBox.Show(
                $"Bạn có muốn {(!IsDuyet ? "duyệt" : "hủy duyệt")} {rowFocused["TenPhieu"]?.ToString()} không?",
                "Thông báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                if (messResult != DialogResult.Yes) return;
                string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
                bool check = !IsDuyet;
                ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
                objXetDuyet.Action = "DuyetMuaHang2";
                objXetDuyet.MaPhieu = maphieumh;
                objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                objXetDuyet.IsXacNhan = !IsDuyet;
                objXetDuyet.NguoiXN = GlobleData.UserName;
                objXetDuyet.GhiChu = "";

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    this.DialogResult = DialogResult.OK;
                    LoadPhieuMuaHang();
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }


        private void POSTHUY(string ghichu)
        {
            if (grvPhieuMHVatTu.FocusedRowHandle >= 0)
            {
                DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
                ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
                objXetDuyet.Action = "HuyPhieu";
                objXetDuyet.MaPhieu = maphieumh;
                objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                objXetDuyet.IsXacNhan = false;
                objXetDuyet.NguoiXN = GlobleData.UserName;
                objXetDuyet.GhiChu = ghichu;

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                if (msResult.ToLower() == "true")
                {
                    //clsWaitForm.ShowSuccessForm(this, 2000);
                    //this.DialogResult = DialogResult.OK;
                    //LoadPhieuMuaHang();
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        private void POSTHUYMMTB()
        {
            if (gridView5.FocusedRowHandle >= 0)
            {
                DataRow rowFocused = gridView5.GetFocusedDataRow() as DataRow;
                string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                string url = string.Format("{0}", URL + "ERPPOMuaMMTB/Post");
                ERPXacNhanPOMuaMMTBEntity objXetDuyet = new ERPXacNhanPOMuaMMTBEntity();
                objXetDuyet.Action = "HuyPhieu";
                objXetDuyet.MaPhieu = maphieumh;
                objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                objXetDuyet.IsXacNhan = false;
                objXetDuyet.User = GlobleData.UserName;
                objXetDuyet.GhiChu = "";

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                if (msResult.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    this.DialogResult = DialogResult.OK;
                    //LoadPhieuMuaHangMMTB();
                    LoadDataForCurrentTab();
                }
                else
                {
                    XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void POSTNGUOIXN(string ngayghtt)
        {
            DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
            string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
            string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
            ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
            objXetDuyet.Action = "POSTNXN";
            objXetDuyet.MaPhieu = maphieumh;
            objXetDuyet.NgayXN = ngayghtt;
            objXetDuyet.IsXacNhan = false;
            objXetDuyet.NguoiXN = GlobleData.UserName;
            objXetDuyet.GhiChu = "";

            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
            if (msResult.ToLower() == "true")
            {
                //clsWaitForm.ShowSuccessForm(this, 2000);
                //this.DialogResult = DialogResult.OK;
                //LoadPhieuMuaHang();
            }
            else
            {
                XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void POSTNGUOIXNMMTB(string ngayghtt)
        {
            DataRow rowFocused = gridView5.GetFocusedDataRow() as DataRow;
            string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
            string url = string.Format("{0}", URL + "ERPPOMuaMMTB/Post");
            ERPXacNhanPOMuaMMTBEntity objXetDuyet = new ERPXacNhanPOMuaMMTBEntity();
            objXetDuyet.Action = "POSTNXN";
            objXetDuyet.MaPhieu = maphieumh;
            string ngayFormatted = DateTime.Now.ToString("yyyy-MM-dd");
            if (!string.IsNullOrEmpty(ngayghtt))
            {
                if (DateTime.TryParseExact(ngayghtt, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDate))
                {
                    ngayFormatted = parsedDate.ToString("yyyy-MM-dd");
                }
                else if (DateTime.TryParse(ngayghtt, out DateTime parsedDate2))
                {
                    ngayFormatted = parsedDate2.ToString("yyyy-MM-dd");
                }
            }

            objXetDuyet.NgayXN = ngayFormatted;
            objXetDuyet.IsXacNhan = false;
            objXetDuyet.User = GlobleData.UserName;
            objXetDuyet.GhiChu = "";

            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
            if (msResult.ToLower() == "true")
            {
                //clsWaitForm.ShowSuccessForm(this, 2000);
                //this.DialogResult = DialogResult.OK;
                //LoadPhieuMuaHang();
            }
            else
            {
                XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void POSTNGUOIKT()
        {
            DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
            string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
            string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
            ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
            objXetDuyet.Action = "POSTNKT";
            objXetDuyet.MaPhieu = maphieumh;
            objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            objXetDuyet.IsXacNhan = false;
            objXetDuyet.NguoiXN = GlobleData.UserName;
            objXetDuyet.GhiChu = "";

            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
            if (msResult.ToLower() == "true")
            {
                //clsWaitForm.ShowSuccessForm(this, 2000);
                //this.DialogResult = DialogResult.OK;
                //LoadPhieuMuaHang();
            }
            else
            {
                XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void grvPhieuMHVatTu_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gridView3_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            bool isNplGroup = (info.Column != null
                               && info.Column.FieldName == "NPL"
                               && info.EditValue != null);

            int groupIndex = gridView3.GetRowLevel(e.RowHandle);

            if (groupIndex == 0)
            {
                e.Appearance.ForeColor = Color.MediumBlue;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if (groupIndex == 1)
            {
                e.Appearance.ForeColor = Color.Maroon;
            }
            else if (groupIndex == 2)
            {
                e.Appearance.ForeColor = Color.Red;
            }

            if (isNplGroup)
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);
                info.GroupText = isChecked ? "Nguyên liệu" : "Phụ liệu";
                e.Handled = false;
                return;
            }

            if (info.Column == gridColumn3)
            {
                info.GroupText = info.GroupValueText;
                e.Handled = false;
                return;
            }


            e.Handled = false;
        }

        private void gridView3_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "KiemVT")
            {
                string ketqua = view.GetRowCellValue(e.RowHandle, e.Column.FieldName)?.ToString()?.Trim();

                if (string.IsNullOrEmpty(ketqua))
                    return;

                switch (ketqua.ToUpper())
                {
                    case "PASS":
                        e.Appearance.BackColor = Color.SkyBlue;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    case "FAIL":
                        e.Appearance.BackColor = Color.OrangeRed;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    default:
                        break;
                }
            }
            if (e.Column.FieldName == "qcShipment")
            {
                string ketqua = view.GetRowCellValue(e.RowHandle, e.Column.FieldName)?.ToString()?.Trim();

                if (string.IsNullOrEmpty(ketqua))
                    return;

                switch (ketqua.ToUpper())
                {
                    case "PASS":
                        e.Appearance.BackColor = Color.SkyBlue;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    case "FAIL":
                        e.Appearance.BackColor = Color.OrangeRed;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    default:
                        break;
                }
            }
            //if (e.Column.FieldName == "TrangThai")
            //{
            //    int ketqua = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, e.Column.FieldName));


            //    switch (ketqua)
            //    {
            //        case 1:
            //            e.Appearance.BackColor = Color.Green;
            //            e.Appearance.ForeColor = Color.Black;
            //            break;

            //        case 0:
            //            e.Appearance.BackColor = Color.SkyBlue;
            //            e.Appearance.ForeColor = Color.Black;
            //            break;

            //        default:
            //            break;
            //    }
            //}

            if (e.Column.FieldName == "TrangThai")
            {
                if (e.RowHandle < 0) return;
                int trangThai = Convert.ToInt32(
                    view.GetRowCellValue(e.RowHandle, "TrangThai")
                );

                string kiemVT = Convert.ToString(
                    view.GetRowCellValue(e.RowHandle, "KiemVT")
                )?.Trim();

                if (trangThai == 1)
                {
                    e.Appearance.BackColor = Color.Green;
                    e.Appearance.ForeColor = Color.Black;
                }
                else if (kiemVT == "PASS")
                {
                    e.Appearance.BackColor = Color.SkyBlue;
                    e.Appearance.ForeColor = Color.Black;
                }
                else
                {
                    e.Appearance.BackColor = Color.LightYellow;
                    e.Appearance.ForeColor = Color.Black;
                }
            }

            if (e.Column.FieldName == "SimpleShipment")
            {
                DataRow row = view.GetDataRow(e.RowHandle);
                if (row != null)
                {
                    if (row["SimpleShipment"]?.ToString() == "Đã nhập")
                    {
                        e.Appearance.ForeColor = Color.Olive;
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                }
            }

        }

        private async void gridView3_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            DataRow rowFocused = view.GetDataRow(e.RowHandle);
            if (rowFocused == null) return;
            int ISNL = Convert.ToInt32(rowFocused["NPL"]);
            string QCKiem = rowFocused["KiemVT"]?.ToString();
            if (e.Column.Name == "gridColumn44")
            {

                try
                {
                    bool isAdmin = string.Equals(
                            GlobleData.UserName?.ToString(),
                            "admin",
                            StringComparison.OrdinalIgnoreCase
                        );
                    if (!isAdmin)
                    {
                        string nguoitao = rowFocused["NguoiTao"]?.ToString();
                        if (GlobleData.UserName.ToString().ToUpper() != nguoitao?.ToUpper())
                        {
                            XtraMessageBox.Show("bạn không phải là người tạo phiếu. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter =
                        "Supported files (*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.jpg;*.jpeg;*.png;*.bmp)|" +
                        "*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.jpg;*.jpeg;*.png;*.bmp|" +
                        "PDF (*.pdf)|*.pdf|" +
                        "Word (*.doc;*.docx)|*.doc;*.docx|" +
                        "Excel (*.xls;*.xlsx)|*.xls;*.xlsx|" +
                        "Image (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                    if (ofd.ShowDialog() != DialogResult.OK) return;

                    string filePath = ofd.FileName;
                    string fileName = Path.GetFileName(filePath);
                    bool uploadOK = await UploadFile(filePath, fileName);
                    if (!uploadOK)
                    {
                        XtraMessageBox.Show("Upload thất bại!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    view.SetRowCellValue(
                    view.FocusedRowHandle,
                    "TaiLieuVT",
                    fileName);

                    string mavtid = rowFocused["MaVTID"]?.ToString();
                    string maclvt = rowFocused["MaCLVT"]?.ToString();
                    string mauvtid = rowFocused["MauVTID"]?.ToString();
                    string khovaiid = rowFocused["KhoVaiID"]?.ToString();
                    string tailieu = rowFocused["TaiLieuVT"]?.ToString();
                    bool IsDuyet = false;
                    //bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhan);
                    //if (IsXacNhan)
                    //{
                    //    XtraMessageBox.Show("Đã duyệt sẽ không hủy xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    LoadPhieuMuaHang();
                    //    return;
                    //}
                    DialogResult messResult = MessageBox.Show(
                    $"Bạn có muốn {("upload tài liệu này")} {rowFocused["TenPhieu"]?.ToString()} không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                    _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                    string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                    if (messResult != DialogResult.Yes) return;
                    string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
                    bool check = !IsDuyet;
                    ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
                    objXetDuyet.Action = "POSTTaiLieu";
                    objXetDuyet.MaPhieu = maphieumh;
                    objXetDuyet.MaVTID = mavtid;
                    objXetDuyet.MaCLVT = maclvt;
                    objXetDuyet.MauVTID = mauvtid;
                    objXetDuyet.KhoVaiID = khovaiid;
                    objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                    objXetDuyet.IsXacNhan = !IsDuyet;
                    objXetDuyet.NguoiXN = tailieu;
                    objXetDuyet.GhiChu = "";

                    string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                    if (msResult.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        this.DialogResult = DialogResult.OK;
                        LoadPhieuMuaHang();
                    }
                    else
                    {
                        XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {

                }
            }else if(view.FocusedColumn.FieldName == "SimpleShipment") 
            {
                rowFocused = grvPhieuMHVatTu.GetFocusedDataRow();
                if (rowFocused != null)
                {
                    string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                    if (string.IsNullOrEmpty(MaPhieuMH))
                    {
                        XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để xem", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    frmERPPOMHSimpleShipment frm = new frmERPPOMHSimpleShipment(rowFocused);
                    frm.ShowDialog();
                    frm.MaximizeBox = true;
                    if (frm.DialogResult == DialogResult.OK)
                    {
                        LoadPhieuMuaHang();
                    }
                }
            } 
        }

        private void gridView3_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            //if (e.Column.FieldName == "NguoiSample")
            //{
            //    bool isChecked = Convert.ToBoolean(
            //        view.GetRowCellValue(e.RowHandle, "IsSample")
            //    );

            //    string text = e.CellValue?.ToString();

            //    CheckEditViewInfo checkInfo =
            //        new CheckEditViewInfo(rItemCheckEditDuyet);

            //    CheckEditPainter painter = new CheckEditPainter();

            //    checkInfo.EditValue = isChecked;

            //    checkInfo.Bounds = new Rectangle(
            //        e.Bounds.X + CHECK_LEFT,
            //        e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
            //        CHECK_SIZE,
            //        CHECK_SIZE
            //    );

            //    checkInfo.CalcViewInfo(e.Graphics);

            //    ControlGraphicsInfoArgs args =
            //        new ControlGraphicsInfoArgs(
            //            checkInfo,
            //            new GraphicsCache(e.Graphics),
            //            checkInfo.Bounds
            //        );

            //    painter.Draw(args);

            //    // Text
            //    Rectangle textRect = new Rectangle(
            //        e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
            //        e.Bounds.Y,
            //        e.Bounds.Width,
            //        e.Bounds.Height
            //    );

            //    e.Appearance.DrawString(e.Cache, text, textRect);
            //    e.Handled = true;
            //}
            //if (e.Column.FieldName == "NguoiBulk")
            //{
            //    bool isChecked = Convert.ToBoolean(
            //        view.GetRowCellValue(e.RowHandle, "IsBulk")
            //    );

            //    string text = e.CellValue?.ToString();

            //    CheckEditViewInfo checkInfo =
            //        new CheckEditViewInfo(rItemCheckEditDuyet);

            //    CheckEditPainter painter = new CheckEditPainter();

            //    checkInfo.EditValue = isChecked;

            //    checkInfo.Bounds = new Rectangle(
            //        e.Bounds.X + CHECK_LEFT,
            //        e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
            //        CHECK_SIZE,
            //        CHECK_SIZE
            //    );

            //    checkInfo.CalcViewInfo(e.Graphics);

            //    ControlGraphicsInfoArgs args =
            //        new ControlGraphicsInfoArgs(
            //            checkInfo,
            //            new GraphicsCache(e.Graphics),
            //            checkInfo.Bounds
            //        );

            //    painter.Draw(args);

            //    // Text
            //    Rectangle textRect = new Rectangle(
            //        e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
            //        e.Bounds.Y,
            //        e.Bounds.Width,
            //        e.Bounds.Height
            //    );

            //    e.Appearance.DrawString(e.Cache, text, textRect);
            //    e.Handled = true;
            //}
            if (e.Column.FieldName == "UserMer")
            {
                bool isChecked = Convert.ToBoolean(
                    view.GetRowCellValue(e.RowHandle, "MerCheck")
                );

                //string text = e.CellValue?.ToString();
                string text = GetTenNV(e.CellValue?.ToString());

                CheckEditViewInfo checkInfo =
                    new CheckEditViewInfo(rItemCheckEditDuyet);

                CheckEditPainter painter = new CheckEditPainter();

                checkInfo.EditValue = isChecked;

                checkInfo.Bounds = new Rectangle(
                    e.Bounds.X + CHECK_LEFT,
                    e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
                    CHECK_SIZE,
                    CHECK_SIZE
                );

                checkInfo.CalcViewInfo(e.Graphics);

                ControlGraphicsInfoArgs args =
                    new ControlGraphicsInfoArgs(
                        checkInfo,
                        new GraphicsCache(e.Graphics),
                        checkInfo.Bounds
                    );

                painter.Draw(args);

                // Text
                Rectangle textRect = new Rectangle(
                    e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                );

                e.Appearance.DrawString(e.Cache, text, textRect);
                e.Handled = true;
            }

        }

        private async void gridView3_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {


            GridView view = sender as GridView;
            DataRow rowFocused = view.GetDataRow(e.RowHandle);
            if (rowFocused == null) return;
            //if (e.Column.Name == "gridColumn46")
            //{
            //    try
            //    {
            //        bool isAdmin = string.Equals(
            //                GlobleData.UserName?.ToString(),
            //                "admin",
            //                StringComparison.OrdinalIgnoreCase
            //            );
            //        if (!isAdmin)
            //        {
            //            string nguoitao = rowFocused["NguoiTao"]?.ToString();
            //            if (GlobleData.UserName.ToString().ToUpper() != nguoitao?.ToUpper())
            //            {
            //                XtraMessageBox.Show("bạn không phải là người tạo phiếu. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                return;
            //            }
            //        }

            //        bool.TryParse(rowFocused["IsSample"]?.ToString(), out bool IsDuyet);
            //        string mavtid = rowFocused["MaVTID"]?.ToString();
            //        string maclvt = rowFocused["MaCLVT"]?.ToString();
            //        string mauvtid = rowFocused["MauVTID"]?.ToString();
            //        string khovaiid = rowFocused["KhoVaiID"]?.ToString();
            //        //bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhan);
            //        //if (IsXacNhan)
            //        //{
            //        //    XtraMessageBox.Show("Đã duyệt sẽ không hủy xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        //    LoadPhieuMuaHang();
            //        //    return;
            //        //}
            //        DialogResult messResult = MessageBox.Show(
            //        $"Bạn có muốn {(!IsDuyet ? "duyệt Sample cho vật tư này của" : "hủy duyệt Sample vật tư này của")} {rowFocused["TenPhieu"]?.ToString()} không?",
            //        "Thông báo",
            //        MessageBoxButtons.YesNo,
            //        MessageBoxIcon.Question);
            //        string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
            //        if (messResult != DialogResult.Yes) return;
            //        string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
            //        bool check = !IsDuyet;
            //        ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
            //        objXetDuyet.Action = "POSTSAMPLE";
            //        objXetDuyet.MaPhieu = maphieumh;
            //        objXetDuyet.MaVTID = mavtid;
            //        objXetDuyet.MaCLVT = maclvt;
            //        objXetDuyet.MauVTID = mauvtid;
            //        objXetDuyet.KhoVaiID = khovaiid;
            //        objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            //        objXetDuyet.IsXacNhan = !IsDuyet;
            //        objXetDuyet.NguoiXN = GlobleData.UserName;
            //        objXetDuyet.GhiChu = "";

            //        string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
            //        if (msResult.ToLower() == "true")
            //        {
            //            clsWaitForm.ShowSuccessForm(this, 2000);
            //            this.DialogResult = DialogResult.OK;
            //            LoadPhieuMuaHang();
            //        }
            //        else
            //        {
            //            XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }

            //}
            //else if (e.Column.Name == "gridColumn47")
            //{
            //    try
            //    {
            //        bool isAdmin = string.Equals(
            //                GlobleData.UserName?.ToString(),
            //                "admin",
            //                StringComparison.OrdinalIgnoreCase
            //            );
            //        if (!isAdmin)
            //        {
            //            string nguoitao = rowFocused["NguoiTao"]?.ToString();
            //            if (GlobleData.UserName.ToString().ToUpper() != nguoitao?.ToUpper())
            //            {
            //                XtraMessageBox.Show("bạn không phải là người tạo phiếu. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                return;
            //            }
            //        }

            //        bool.TryParse(rowFocused["IsBulk"]?.ToString(), out bool IsDuyet);
            //        string mavtid = rowFocused["MaVTID"]?.ToString();
            //        string maclvt = rowFocused["MaCLVT"]?.ToString();
            //        string mauvtid = rowFocused["MauVTID"]?.ToString();
            //        string khovaiid = rowFocused["KhoVaiID"]?.ToString();
            //        //bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhan);
            //        //if (IsXacNhan)
            //        //{
            //        //    XtraMessageBox.Show("Đã duyệt sẽ không hủy xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        //    LoadPhieuMuaHang();
            //        //    return;
            //        //}
            //        DialogResult messResult = MessageBox.Show(
            //        $"Bạn có muốn {(!IsDuyet ? "duyệt bulk vật tư này của" : "hủy duyệt bulk vật tư này của")} {rowFocused["TenPhieu"]?.ToString()} không?",
            //        "Thông báo",
            //        MessageBoxButtons.YesNo,
            //        MessageBoxIcon.Question);
            //        string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
            //        if (messResult != DialogResult.Yes) return;
            //        string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
            //        bool check = !IsDuyet;
            //        ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
            //        objXetDuyet.Action = "POSTBULK";
            //        objXetDuyet.MaPhieu = maphieumh;
            //        objXetDuyet.MaVTID = mavtid;
            //        objXetDuyet.MaCLVT = maclvt;
            //        objXetDuyet.MauVTID = mauvtid;
            //        objXetDuyet.KhoVaiID = khovaiid;
            //        objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            //        objXetDuyet.IsXacNhan = !IsDuyet;
            //        objXetDuyet.NguoiXN = GlobleData.UserName;
            //        objXetDuyet.GhiChu = "";

            //        string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
            //        if (msResult.ToLower() == "true")
            //        {
            //            clsWaitForm.ShowSuccessForm(this, 2000);
            //            this.DialogResult = DialogResult.OK;
            //            LoadPhieuMuaHang();
            //        }
            //        else
            //        {
            //            XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }

            //}
            //else if (e.Column.Name == "gridColumn44")
            //{
            //    try
            //    {
            //        bool isAdmin = string.Equals(
            //                GlobleData.UserName?.ToString(),
            //                "admin",
            //                StringComparison.OrdinalIgnoreCase
            //            );
            //        if (!isAdmin)
            //        {
            //            string nguoitao = rowFocused["NguoiTao"]?.ToString();
            //            if (GlobleData.UserName.ToString().ToUpper() != nguoitao?.ToUpper())
            //            {
            //                XtraMessageBox.Show("bạn không phải là người tạo phiếu. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                return;
            //            }
            //        }

            //        OpenFileDialog ofd = new OpenFileDialog();
            //        ofd.Filter = "PDF files (*.pdf)|*.pdf";
            //        if (ofd.ShowDialog() != DialogResult.OK) return;

            //        string filePath = ofd.FileName;
            //        string fileName = Path.GetFileName(filePath);
            //        bool uploadOK = await UploadPDF(filePath, fileName);
            //        if (!uploadOK)
            //        {
            //            XtraMessageBox.Show("Upload thất bại!", "Lỗi",
            //                MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            return;
            //        }
            //        view.SetRowCellValue(
            //        view.FocusedRowHandle,
            //        "TaiLieuVT",
            //        fileName);

            //        string mavtid = rowFocused["MaVTID"]?.ToString();
            //        string maclvt = rowFocused["MaCLVT"]?.ToString();
            //        string mauvtid = rowFocused["MauVTID"]?.ToString();
            //        string khovaiid = rowFocused["KhoVaiID"]?.ToString();
            //        string tailieu = rowFocused["TaiLieuVT"]?.ToString();
            //        bool IsDuyet = false;
            //        //bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhan);
            //        //if (IsXacNhan)
            //        //{
            //        //    XtraMessageBox.Show("Đã duyệt sẽ không hủy xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        //    LoadPhieuMuaHang();
            //        //    return;
            //        //}
            //        DialogResult messResult = MessageBox.Show(
            //        $"Bạn có muốn {("upload tài liệu này")} {rowFocused["TenPhieu"]?.ToString()} không?",
            //        "Thông báo",
            //        MessageBoxButtons.YesNo,
            //        MessageBoxIcon.Question);
            //        string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
            //        if (messResult != DialogResult.Yes) return;
            //        string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
            //        bool check = !IsDuyet;
            //        ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
            //        objXetDuyet.Action = "POSTTaiLieu";
            //        objXetDuyet.MaPhieu = maphieumh;
            //        objXetDuyet.MaVTID = mavtid;
            //        objXetDuyet.MaCLVT = maclvt;
            //        objXetDuyet.MauVTID = mauvtid;
            //        objXetDuyet.KhoVaiID = khovaiid;
            //        objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            //        objXetDuyet.IsXacNhan = !IsDuyet;
            //        objXetDuyet.NguoiXN = tailieu;
            //        objXetDuyet.GhiChu = "";

            //        string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
            //        if (msResult.ToLower() == "true")
            //        {
            //            clsWaitForm.ShowSuccessForm(this, 2000);
            //            this.DialogResult = DialogResult.OK;
            //            LoadPhieuMuaHang();
            //        }
            //        else
            //        {
            //            XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }

            //}

            if (e.Column.Name == "gridColumn57")
            {
                try
                {
                    bool isAdmin = string.Equals(
                            GlobleData.UserName?.ToString(),
                            "admin",
                            StringComparison.OrdinalIgnoreCase
                        );
                    if (!isAdmin)
                    {
                        //string nguoitao = rowFocused["NguoiTao"]?.ToString();
                        //if (GlobleData.UserName.ToString().ToUpper() != nguoitao?.ToUpper())
                        //{
                        //    XtraMessageBox.Show("bạn không phải là người tạo phiếu. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    return;
                        //}
                    }

                    bool.TryParse(rowFocused["MerCheck"]?.ToString(), out bool IsDuyet);
                    string mavtid = rowFocused["MaVTID"]?.ToString();
                    string maclvt = rowFocused["MaCLVT"]?.ToString();
                    string mauvtid = rowFocused["MauVTID"]?.ToString();
                    string khovaiid = rowFocused["KhoVaiID"]?.ToString();
                    //bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhan);
                    //if (IsXacNhan)
                    //{
                    //    XtraMessageBox.Show("Đã duyệt sẽ không hủy xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    LoadPhieuMuaHang();
                    //    return;
                    //}
                    //DialogResult messResult = MessageBox.Show(
                    //$"Bạn có muốn {(!IsDuyet ? "duyệt Mer cho vật tư này của" : "hủy duyệt Mer vật tư này của")} {rowFocused["TenPhieu"]?.ToString()} không?",
                    //"Thông báo",
                    //MessageBoxButtons.YesNo,
                    //MessageBoxIcon.Question);
                    _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                    string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                    //if (messResult != DialogResult.Yes) return;
                    frmNhapKetLuanMer frm = new frmNhapKetLuanMer(maphieumh,$"{maclvt}@{mavtid}@{mauvtid}@{khovaiid}");
                    frm.ShowDialog();
                    if(frm.DialogResult == DialogResult.OK)
                    {
                        string url = string.Format("{0}", URL + "ERPCanDoiNguyenPhuLieu/Post");
                        bool check = !IsDuyet;
                        ERPXacNhanPOMuaEntity objXetDuyet = new ERPXacNhanPOMuaEntity();
                        objXetDuyet.Action = "POSTMerCheck";
                        objXetDuyet.MaPhieu = maphieumh;
                        objXetDuyet.MaVTID = mavtid;
                        objXetDuyet.MaCLVT = maclvt;
                        objXetDuyet.MauVTID = mauvtid;
                        objXetDuyet.KhoVaiID = khovaiid;
                        objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                        objXetDuyet.IsXacNhan = frm.result;
                        objXetDuyet.NguoiXN = GlobleData.UserName;
                        objXetDuyet.GhiChu = frm.KetLuan;

                        string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                        if (msResult.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 2000);
                            this.DialogResult = DialogResult.OK;
                            string resultDG = frm.result ? "Pass" : "Fail";
                            string Title = $"Merchandiser đánh giá vật tư {rowFocused["ItemCode"]} {resultDG}";
                            string Detail = $"Mã Phiếu: {rowFocused["TenPhieu"]?.ToString()}\nNhà cung cấp: {rowFocused["TenNhaCC"]?.ToString()}\nPOMH : {rowFocused["POMua"]}";
                            SendNotify(Title, Detail, "PKH");
                            SendNotify(Title, Detail, "KHO");

                        }
                        else
                        {
                            XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    //LoadPhieuMuaHang();

                }
                catch (Exception ex)
                {

                }

            }
        }

        private async void repositoryItemButtonEdit3_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            if (editor == null) return;

            GridControl grid = editor.Parent as GridControl;
            if (grid == null) return;

            GridView view = grid.FocusedView as GridView;
            if (view == null) return;

            int rowHandle = view.FocusedRowHandle;
            if (rowHandle < 0) return;

            DataRowView drv = view.GetRow(rowHandle) as DataRowView;
            if (drv == null) return;

            DataRow row = drv.Row;

            string fileName = row["TaiLieuVT"]?.ToString();
            if (string.IsNullOrEmpty(fileName))
            {
                XtraMessageBox.Show(
                    "Dòng này chưa có tài liệu!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string urlHost = settingsReader
                .GetValue("URLV2", typeof(string))
                .ToString()
                .TrimEnd('/');

            // Encode để tránh lỗi tên file có dấu / khoảng trắng
            string safeFileName = Uri.EscapeDataString(fileName);

            string fileUrl = $"{urlHost}/PMH/VatTu/{safeFileName}";

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = fileUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    ex.Message,
                    "Lỗi mở tài liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task<bool> UploadFile(string filePath, string fileName)
        {
            try
            {
                string url = URL + "ERPCanDoiNguyenPhuLieu/UploadFile";

                using (var client = new HttpClient())
                using (var content = new MultipartFormDataContent())
                {
                    byte[] data = File.ReadAllBytes(filePath);
                    var fileContent = new ByteArrayContent(data);

                    // Không fix cứng content-type
                    fileContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                    content.Add(fileContent, "file", fileName);

                    HttpResponseMessage res = await client.PostAsync(url, content);
                    return res.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        private void gridView3_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                //GridView view = sender as GridView;
                //if (e.Column.FieldName == "STT" && e.IsGetData)
                //    e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
                var urlHost = (string)settingsReader.GetValue("URLV2", typeof(String));
                //if (e.Column.FieldName == "STT" && e.IsGetData)
                //    e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
                if ((e.Column.FieldName == "UrlTaiLieu") && e.IsGetData)
                {
                    DataRow dr = gridView3.GetDataRow(e.ListSourceRowIndex);
                    if (dr == null) return;
                    string url = urlHost + "/PMHPDF/VatTu/" + dr["TaiLieuVT"].ToString();
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
                        catch
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void gridView3_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
            {
                GridView view = sender as GridView;

                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);

                // Chỉ đánh số cho dòng dữ liệu, không đánh số cho group row
                if (rowHandle >= 0)
                    e.DisplayText = (rowHandle + 1).ToString();
                else
                    e.DisplayText = "";
            }
            if (e.Column.FieldName == "TrangThai")
            {

                GridView view = sender as GridView;
                if (view == null) return;

                int rowHandle = view.GetRowHandle(e.ListSourceRowIndex);
                if (rowHandle < 0) return;

                int trangThai = Convert.ToInt32(
                    view.GetRowCellValue(rowHandle, "TrangThai")
                );

                string kiemVT = Convert.ToString(
                    view.GetRowCellValue(rowHandle, "KiemVT")
                )?.Trim();

                object ngayNkObj = view.GetRowCellValue(rowHandle, "NgayNK");
                bool hasNgayNK = ngayNkObj != null
                                 && ngayNkObj != DBNull.Value
                                 && !string.IsNullOrWhiteSpace(ngayNkObj.ToString());

                if (trangThai == 1)
                {
                    e.DisplayText = "Đã nhập kho";
                }
                else
                {
                    if (kiemVT == "PASS" || hasNgayNK)
                        e.DisplayText = "Đang nhập kho";
                    else
                        e.DisplayText = "Chưa nhập kho";
                }
            }


        }

        private void repositoryItemButtonEdit3_ButtonClick_1(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            if (editor == null) return;

            // GridControl chứa editor
            GridControl grid = editor.Parent as GridControl;
            if (grid == null) return;

            // LẤY VIEW ĐANG ACTIVE (detail view hiện tại)
            GridView view = grid.FocusedView as GridView;
            if (view == null) return;

            int rowHandle = view.FocusedRowHandle;
            if (rowHandle < 0) return;

            DataRowView drv = view.GetRow(rowHandle) as DataRowView;
            if (drv == null) return;

            DataRow row = drv.Row;
            int ISNL = Convert.ToInt32(row["NPL"]);
            string mavtid = row["MaVTID"]?.ToString();
            string maclvt = row["MaCLVT"]?.ToString();
            string mauvtid = row["MauVTID"]?.ToString();
            string khovaiid = row["KhoVaiID"]?.ToString();
            string SoLoID = row["SoLoID"]?.ToString();
            string SoLo = row["SoLo"]?.ToString();
            string manpl = row["MaNPL"]?.ToString();
            string ghichu = row["GhiChuQC"]?.ToString();
            string QCKiem = row["KiemVT"]?.ToString();
            string MaNPL = $"{maclvt}@{mavtid}@{mauvtid}@{khovaiid}";
            /**/
            if (ISNL == 1)
            {
                if (QCKiem == "")
                {
                    return;
                }
                frmViewKiemNL frm = new frmViewKiemNL(SoLoID, SoLo, MaNPL);
                frm.Show();
            }
            else if (ISNL == 0)
            {
                if (QCKiem == "")
                {
                    return;
                }
                frmViewKiemPL frm = new frmViewKiemPL(SoLoID, SoLo, MaNPL);
                frm.Show();
            }
        }

        private void repositoryItemButtonEdit5_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            ButtonEdit editor = sender as ButtonEdit;
            if (editor == null) return;

            // GridControl chứa editor
            GridControl grid = editor.Parent as GridControl;
            if (grid == null) return;

            // LẤY VIEW ĐANG ACTIVE (detail view hiện tại)
            GridView view = grid.FocusedView as GridView;
            if (view == null) return;

            if (view.RowCount == 0) return;
            view.CloseEditor();
            view.UpdateCurrentRow();
            //int rowHandle = view.FocusedRowHandle;
            //if (rowHandle < 0) return;

            //DataRowView drv = view.GetRow(rowHandle) as DataRowView;
            //if (drv == null) return;

            //DataRow row = drv.Row;
            //int ISNL = Convert.ToInt32(row["NPL"]);
            //string mavtid = row["MaVTID"]?.ToString();
            //string maclvt = row["MaCLVT"]?.ToString();
            //string mauvtid = row["MauVTID"]?.ToString();
            //string khovaiid = row["KhoVaiID"]?.ToString();
            //string SoLoID = row["SoLoID"]?.ToString();
            //string SoLo = row["SoLo"]?.ToString();
            //string manpl = row["MaNPL"]?.ToString();
            //string ghichu = row["GhiChuQC"]?.ToString();
            DataRowView firstDrv = view.GetRow(0) as DataRowView;
            if (firstDrv == null) return;

            int ISNL = Convert.ToInt32(firstDrv["NPL"]);

            if (ISNL == 1)
            {
                DataTable tblnl = CreateDatatableToSaveNL();

                //DataRow newRow = tblnl.NewRow();
                //newRow["SoLoID"] = SoLoID;
                //newRow["MaNPL"] = manpl;
                ////newRow["LoaiVai"] = maclvt;
                ////newRow["MaVTID"] = mavtid;
                ////newRow["MauVTID"] = mauvtid;
                //newRow["ResultQC"] = string.IsNullOrEmpty(ghichu) ? "" : ghichu;
                //tblnl.Rows.Add(newRow);

                for (int i = 0; i < view.RowCount; i++)
                {
                    DataRowView drv = view.GetRow(i) as DataRowView;
                    if (drv == null) continue;

                    DataRow row = drv.Row;

                    DataRow newRow = tblnl.NewRow();
                    newRow["SoLoID"] = row["SoLoID"]?.ToString();
                    newRow["MaNPL"] = row["MaNPL"]?.ToString();
                    newRow["ResultQC"] = row["GhiChuQC"]?.ToString() ?? "";
                    tblnl.Rows.Add(newRow);
                }

                string url2 = string.Format("{0}?", URL + "NhaCC/PostGhiChuNL");
                string mss1 = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url2, tblnl);
                }).Result;
                if (mss1.ToLower() != "true")
                    XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (ISNL == 0)
            {
                DataTable tblpl = CreateDatatableToSavePL();

                //DataRow newRow = tblpl.NewRow();
                //newRow["SoLoID"] = SoLoID;
                //newRow["MaNPL"] = manpl;
                //newRow["KQ_GiaiQuyet"] = string.IsNullOrEmpty(ghichu) ? "" : ghichu;
                //tblpl.Rows.Add(newRow);
                for (int i = 0; i < view.RowCount; i++)
                {
                    DataRowView drv = view.GetRow(i) as DataRowView;
                    if (drv == null) continue;

                    DataRow row = drv.Row;

                    DataRow newRow = tblpl.NewRow();
                    newRow["SoLoID"] = row["SoLoID"]?.ToString();
                    newRow["MaNPL"] = row["MaNPL"]?.ToString();
                    newRow["KQ_GiaiQuyet"] = row["GhiChuQC"]?.ToString() ?? "";
                    tblpl.Rows.Add(newRow);
                }

                string url2 = string.Format("{0}?", URL + "NhaCC/PostGhiChuPL");
                string mss1 = Task.Run(async () =>
                {
                    return await _clientExtension.PostAsync(url2, tblpl);
                }).Result;
                if (mss1.ToLower() != "true")
                    XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            LoadPhieuMuaHang();
        }


        private void grvPhieuMHVatTu_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "CTVatTu";
        }

        private void grvPhieuMHVatTu_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;

                DataRow masterRow = view.GetDataRow(e.RowHandle);
                if (masterRow == null) return;

                string maPhieuMH = masterRow["MaPhieuMH"]?.ToString();

                if (string.IsNullOrEmpty(maPhieuMH) || _tblDetails == null)
                {
                    e.ChildList = null;
                    return;
                }

                DataTable tblqckiem = new DataTable();
                string urlqc = $"{URL}ERPCanDoiNguyenPhuLieu/GET?action=GetQCKIEM&para1=&para2=";
                string jsonqc = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlqc); }).Result;
                if (jsonqc != "[]")
                    tblqckiem = JsonConvert.DeserializeObject<DataTable>(jsonqc);

                var qcLookup = tblqckiem.AsEnumerable()
                    .GroupBy(x => new
                    {
                        MaPhieuMH = x.Field<string>("MaPhieuMH"),
                        MaVTID = x.Field<string>("MaVTID"),
                        MaNhom = x.Field<string>("MaNhom"),
                        MauVTID = x.Field<string>("MauVTID"),
                        KhoVaiID = x.Field<string>("KhoVaiID"),
                        //MaDVVT = x.Field<string>("MaDVVT")
                    })
                    .ToDictionary(
                        g => g.Key,
                        g =>
                        {
                            bool hasKetQua = g.Any(x => x["KetQuaKiem"] != DBNull.Value);
                            bool failKetQua = hasKetQua && g.Any(x =>
                                x["KetQuaKiem"] != DBNull.Value &&
                                Convert.ToInt32(x["KetQuaKiem"]) == 0
                            );
                            string soloid = g.First()["SoLoID"]?.ToString() ?? "";
                            string solo = g.First()["SoLo"]?.ToString() ?? "";
                            string ghichu = g.First()["GhiChu"]?.ToString() ?? "";
                            string manpl = g.First()["MaNPL"]?.ToString() ?? "";
                            string ngaynk = g.First()["NgayNhapKho"]?.ToString() ?? "";
                        //int? qcShipmentValue = g
                        //    .Select(x => x["qcShipment"])
                        //    .Where(v => v != DBNull.Value)
                        //    .Select(v => Convert.ToInt32(v))
                        //    .FirstOrDefault();
                        int? qcShipmentValue = g
                                .Select(x => x["qcShipment"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(x["qcShipment"]))
                                .FirstOrDefault();

                            string qcShipmentResult = qcShipmentValue == 1
                                ? "PASS"
                                : qcShipmentValue == 0
                                    ? "FAIL"
                                    : "";
                            return new
                            {
                                KetQuaKiem = hasKetQua ? (failKetQua ? "FAIL" : "PASS") : "",
                                SoLoID = soloid,
                                SoLo = solo,
                                GhiChu = ghichu,
                                MaNPL = manpl,
                                qcShipment = qcShipmentResult,
                                NgayNK = ngaynk
                            };
                        }
                    );

                foreach (DataRow r in _tblDetails.Rows)
                {
                    var key = new
                    {
                        MaPhieuMH = r["MaPhieuMH"]?.ToString(),
                        MaVTID = r["MaVTID"]?.ToString(),
                        MaNhom = r["MaCLVT"]?.ToString(),
                        MauVTID = r["MauVTID"]?.ToString(),
                        KhoVaiID = r["KhoVaiID"]?.ToString(),
                        //MaDVVT = r["MaDVVT"]?.ToString()
                    };

                    if (qcLookup.TryGetValue(key, out var qc))
                    {
                        r["KiemVT"] = qc.KetQuaKiem;
                        r["SoLoID"] = qc.SoLoID;
                        r["SoLo"] = qc.SoLo;
                        r["GhiChuQC"] = qc.GhiChu;
                        r["MaNPL"] = qc.MaNPL;
                        r["qcShipment"] = qc.qcShipment;
                        r["NgayNK"] = qc.NgayNK;
                    }
                    else
                    {
                        r["KiemVT"] = DBNull.Value;
                        r["SoLoID"] = DBNull.Value;
                        r["GhiChuQC"] = DBNull.Value;
                        r["MaNPL"] = DBNull.Value;
                        r["qcShipment"] = DBNull.Value;
                        r["NgayNK"] = DBNull.Value;
                    }

                }

                DataView dv = new DataView(_tblDetails);
                dv.RowFilter = $"MaPhieuMH = '{maPhieuMH.Replace("'", "''")}'";

                e.ChildList = dv;
            }
            catch(Exception ex)
            {

            }
            

        }



        private void barButtonItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow row = grvPhieuMHVatTu.GetFocusedDataRow();
            if (row == null)
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu cần copy!");
                return;
            }

            //string maPhieuCu = row["MaPhieuMH"].ToString();

            CopyPhieuMuaHang(row);
        }

        #region coppy po
        private void CopyPhieuMuaHang(DataRow rowPhieuCu)
        {
            try
            {
                string maPhieuCu = rowPhieuCu["MaPhieuMH"].ToString();
                //string maPhieuBG = rowPhieuCu["MaPhieuBG"]?.ToString();
                string maNCC = rowPhieuCu["MaNCC"]?.ToString();
                // === LOAD FULL DATA ===
                DataTable dtChiTiet = LoadPMHChiTiet(maPhieuCu);
                DataTable dtChiPhi = LoadPMHChiPhi(maPhieuCu);
                DataTable dtDot = LoadPMHDot(maPhieuCu);
                DataTable dtHTTT = LoadPMHHTTT(maPhieuCu);

                // === MỞ FORM ADD ===
                frmPhieuMuaHang frm = new frmPhieuMuaHang(
                    rowFocused: rowPhieuCu,
                    IsEdit: false,
                    IsAdd: true,
                    IsView: false
                );

                frm.Show();

                //frm.LoadCopyData(
                //    dtChiTiet,
                //    dtChiPhi,
                //    dtDot,
                //    dtHTTT,
                //    maPhieuBG,
                //    maNCC
                //);

                frm.LoadCopyData(
                    dtChiTiet,
                    dtChiPhi,
                    dtDot,
                    dtHTTT,
                    maNCC
                );

                frm.FormClosed += (s, e) =>
                {
                    //LoadPhieuMuaHang();

                    //SplashScreenManager.ShowDefaultWaitForm("Đang tải dữ liệu", "Xin vui lòng chờ...");
                    //try
                    //{
                    //    getVatTuCanMua();
                    //    calcVatTuCanMua();
                    //    loadGridControl1();
                    //}
                    //catch (Exception ex)
                    //{
                    //    XtraMessageBox.Show(ex.Message, "Lỗi",
                    //                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    SplashScreenManager.CloseDefaultWaitForm();
                    //}
                    //finally
                    //{
                    //    SplashScreenManager.CloseDefaultWaitForm();
                    //}

                    LoadDataForCurrentTab();
                };
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "Lỗi Copy");
            }
        }

        private DataTable LoadPMHChiTiet(string maPhieu)
        {
            string url = $"{URL}NhaCC/GePMH?action=GETPMHChiTiet&para1={maPhieu}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            return JsonConvert.DeserializeObject<DataTable>(json);
        }

        private DataTable LoadPMHChiPhi(string maPhieu)
        {
            string url = $"{URL}NhaCC/GetCP?action=GETPMHChiPhi&para1={maPhieu}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            return JsonConvert.DeserializeObject<DataTable>(json);
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            LoadDataForCurrentTab();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Point mousePos = Control.MousePosition;

            frmSearchPMH frm = new frmSearchPMH(mousePos.X, mousePos.Y, xtraTabControl1.SelectedTabPageIndex);
            frm.StartPosition = FormStartPosition.Manual;

            if(xtraTabControl1.SelectedTabPage == xtraTabPage1)
            {
                frm.OnDataUpdate += (s, tblSearch) =>
                {
                  
                    if(tblSearch != null)
                    {

                        if (!tblSearch.Columns.Contains("TongTien"))
                            tblSearch.Columns.Add("TongTien", typeof(decimal));

                        if (!tblSearch.Columns.Contains("TongTienVND"))
                            tblSearch.Columns.Add("TongTienVND", typeof(decimal));

                        if (!tblSearch.Columns.Contains("KetQuaKiem"))
                            tblSearch.Columns.Add("KetQuaKiem", typeof(string));

                        //if (!tblPhieuMuaHangVatTu.Columns.Contains("KNL"))
                        //    tblPhieuMuaHangVatTu.Columns.Add("KNL", typeof(string));

                        //if (!tblPhieuMuaHangVatTu.Columns.Contains("KPL"))
                        //    tblPhieuMuaHangVatTu.Columns.Add("KPL", typeof(string));

                        if (!tblSearch.Columns.Contains("SoLoID"))
                            tblSearch.Columns.Add("SoLoID", typeof(string));

                        if (!tblSearch.Columns.Contains("SoLo"))
                            tblSearch.Columns.Add("SoLo", typeof(string));

                        if (!_tblDetails.Columns.Contains("KiemVT"))
                            _tblDetails.Columns.Add("KiemVT", typeof(string));

                        if (!_tblDetails.Columns.Contains("SoLoID"))
                            _tblDetails.Columns.Add("SoLoID", typeof(string));

                        if (!_tblDetails.Columns.Contains("SoLo"))
                            _tblDetails.Columns.Add("SoLo", typeof(string));

                        if (!_tblDetails.Columns.Contains("GhiChuQC"))
                            _tblDetails.Columns.Add("GhiChuQC", typeof(string));

                        if (!_tblDetails.Columns.Contains("MaNPL"))
                            _tblDetails.Columns.Add("MaNPL", typeof(string));

                        if (!_tblDetails.Columns.Contains("qcShipment"))
                            _tblDetails.Columns.Add("qcShipment", typeof(string));

                        if (!_tblDetails.Columns.Contains("NgayNK"))
                            _tblDetails.Columns.Add("NgayNK", typeof(string));

                        //QCKiemVai
                        DataTable tblqckiem = new DataTable();
                        string urlqc = $"{URL}ERPCanDoiNguyenPhuLieu/GET?action=GetQCKIEM&para1=&para2=&para3=";
                        string jsonqc = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlqc); }).Result;
                        tblqckiem = JsonConvert.DeserializeObject<DataTable>(jsonqc);
                        if (jsonqc != "[]")
                            tblqckiem = JsonConvert.DeserializeObject<DataTable>(jsonqc);

                        var qcLookup = tblqckiem.AsEnumerable()
                     .GroupBy(x => x.Field<string>("MaPhieuMH"))
                    .ToDictionary(
                     g => g.Key,
                     g =>
                     {
                         bool allNull = g.All(x => x["KetQuaKiem"] == DBNull.Value);

                         bool hasNull = g.Any(x => x["KetQuaKiem"] == DBNull.Value);

                         bool hasFail = g.Any(x =>
                             x["KetQuaKiem"] != DBNull.Value &&
                             Convert.ToInt32(x["KetQuaKiem"]) == 0
                         );

                         bool allPass = g.All(x =>
                             x["KetQuaKiem"] != DBNull.Value &&
                             Convert.ToInt32(x["KetQuaKiem"]) == 1
                         );

                         string ketQuaKiem;
                         if (allNull)
                             ketQuaKiem = "";
                         else if (hasFail)
                             ketQuaKiem = "FAIL";
                         else if (allPass)
                             ketQuaKiem = "PASS";
                         else
                             ketQuaKiem = "Đang Kiểm";

                         string soloid = g.First()["SoLoID"]?.ToString() ?? "";
                         string solo = g.First()["SoLo"]?.ToString() ?? "";

                         return new
                         {
                             KetQuaKiem = ketQuaKiem,
                             SoLoID = soloid,
                             SoLo = solo
                         };
                     }
                 );

                        foreach (DataRow r in tblSearch.Rows)
                        {

                            string maPhieu = r["MaPhieuMH"]?.ToString();


                            if (!string.IsNullOrEmpty(maPhieu) && qcLookup.TryGetValue(maPhieu, out var qc))
                            {
                                r["KetQuaKiem"] = qc.KetQuaKiem;
                                //r["KPL"] = qcLookup[maPhieu].KPL;
                                //r["KNL"] = qcLookup[maPhieu].KNL;
                                r["SoLoID"] = qc.SoLoID;
                                r["SoLo"] = qc.SoLo;
                            }
                            else
                            {
                                r["KetQuaKiem"] = DBNull.Value;
                                //r["KPL"] = DBNull.Value;
                                //r["KNL"] = DBNull.Value;
                                r["SoLoID"] = DBNull.Value;
                                r["SoLo"] = DBNull.Value;
                            }
                        }
                            
                    }

                   
                    grcPhieuMHVatTu.DataSource = tblSearch;
                    grcPhieuMHVatTu.RefreshDataSource();
                    grvPhieuMHVatTu.FocusedRowHandle = 0;
                };
        
            }
            else if (xtraTabControl1.SelectedTabPage == xtraTabPage3)
            {
                frm.OnDataUpdate += (s, tblSearch) =>
                {
                    gridControl3.DataSource = tblSearch;
                    gridControl3.RefreshDataSource();
                    
                    gridView6.FocusedRowHandle = 0;
                    if(tblSearch?.Rows?.Count == 0)
                    {
                        gridControl2.DataSource = new DataTable();
                    }
                };
            }


           frm.ShowDialog();
        }

        private void barButtonItem16_ItemClick(object sender, ItemClickEventArgs e)
        {
            //frmPhieuMuaMMTB frm = new frmPhieuMuaMMTB(null, false, true);
            ////frm.Show();
            //frm.ShowDialog();
            //LoadPhieuMuaHangMMTB();
            //frm.MaximizeBox = true;
            btthemvtmmtb();
        }

        private void btthemvtmmtb()
        {
            if (xtraTabControl1.SelectedTabPage != xtraTabPage3)
            {
                XtraMessageBox.Show("Vui lòng chuyển sang tab machinery and equipment để thực hiện tạo phiếu mua",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataRow rowFocusedPDK = gridView6.GetFocusedDataRow() as DataRow;
            int duyet = Convert.ToInt32(rowFocusedPDK["IsDuyet"]);
            if (duyet == 0)
            {
                XtraMessageBox.Show("Vui lòng duyệt phiếu trước khi tạo phiếu mua", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            frmPhieuMuaMMTB frm = new frmPhieuMuaMMTB(null, rowFocusedPDK, false, true);
            //frm.Show();
            frm.Show();
            frm.FormClosed += (s, e) =>
            {
                LoadDataForCurrentTab();
            };
        }


        private DataTable LoadPMHDot(string maPhieu)
        {
            string url = $"{URL}NhaCC/GetCP?action=GETPMHDOT&para1={maPhieu}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            return JsonConvert.DeserializeObject<DataTable>(json);
        }

        private DataTable LoadPMHHTTT(string maPhieu)
        {
            string url = $"{URL}NhaCC/GetCP?action=GETHTTT&para1={maPhieu}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            return JsonConvert.DeserializeObject<DataTable>(json);
        }
        #endregion

        private void grcPhieuMHVatTu_Click(object sender, EventArgs e)
        {

        }

        private void gridView5_DoubleClick(object sender, EventArgs e)
        {
            if (gridView5.FocusedRowHandle >= 0)
            {
                DataRow rowFocusedPDK = gridView6.GetFocusedDataRow() as DataRow;
                DataRow rowFocused = gridView5.GetFocusedDataRow() as DataRow;
                if (rowFocused != null)
                {
                    string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                    if (string.IsNullOrEmpty(MaPhieuMH))
                    {
                        XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    frmPhieuMuaMMTB frm = new frmPhieuMuaMMTB(rowFocused, rowFocusedPDK, false, false, true);
                    frm.ShowDialog();
                    frm.MaximizeBox = true;
                    if (frm.DialogResult == DialogResult.OK)
                    {
                        LoadPhieuMuaHang();
                    }


                }
            }
            else
            {
                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để sửa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void RestoreFocusRow()
        {
            if (string.IsNullOrEmpty(_focusMaPhieuMH)) return;

            GridView view = grvPhieuMHVatTu;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row == null) continue;

                if (row["MaPhieuMH"]?.ToString() == _focusMaPhieuMH)
                {
                    view.FocusedRowHandle = i;
                    view.MakeRowVisible(i);
                    if (view.IsMasterRow(i))
                    {
                        view.SetMasterRowExpanded(i, true);
                    }
                    break;
                }
            }
        }

        private void RestoreFocusRowMMTB()
        {
            if (string.IsNullOrEmpty(_focusMaPhieuMH)) return;

            GridView view = gridView5;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row == null) continue;

                if (row["MaPhieuMH"]?.ToString() == _focusMaPhieuMH)
                {
                    view.FocusedRowHandle = i;
                    view.MakeRowVisible(i);
                    if (view.IsMasterRow(i))
                    {
                        view.SetMasterRowExpanded(i, true);
                    }
                    break;
                }
            }
        }

        #region POMHMMTB
        private void LoadPhieuMuaHangMMTB()
        {
            try
            {
                DateTime FromDate = clsForrmatUtils.ConvertDate(fromDate.EditValue);
                DateTime ToDate = clsForrmatUtils.ConvertDate(toDate.EditValue);
                DataRow rowFocusedpyc = gridView6.GetFocusedDataRow() as DataRow;
                if (rowFocusedpyc == null) return;
                string maphieuyc = rowFocusedpyc["MaPhieu"]?.ToString();
                //Phieu MuaHang
                DataTable tblPhieuMuaHangMMTB = new DataTable();
                string url = $"{URL}ERPPOMuaMMTB/Get?action=GetPhieuMuaHang&para1={maphieuyc}&para2=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                string urldt = $"{URL}NhaCC/GePMHMMTB?action=GETPOALLDETAILS&para1=&para2=&para3=&para4=&para5=";
                string jsondt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldt); }).Result;
                _tblDetailsMMTB = JsonConvert.DeserializeObject<DataTable>(jsondt);

                if (json != "[]")
                {
                    tblPhieuMuaHangMMTB = JsonConvert.DeserializeObject<DataTable>(json);
                }
                else
                {
                    gridControl2.DataSource = null;
                    gridControl2.RefreshDataSource();
                }

                if (tblPhieuMuaHangMMTB == null || tblPhieuMuaHangMMTB.Rows.Count == 0)
                {
                    gridControl2.DataSource = tblPhieuMuaHangMMTB;
                    gridControl2.RefreshDataSource();
                    //RestoreFocusRow();
                    return;
                }

                if (!tblPhieuMuaHangMMTB.Columns.Contains("TongTien"))
                    tblPhieuMuaHangMMTB.Columns.Add("TongTien", typeof(decimal));

                if (!tblPhieuMuaHangMMTB.Columns.Contains("TongTienVND"))
                    tblPhieuMuaHangMMTB.Columns.Add("TongTienVND", typeof(decimal));

                //if (!tblPhieuMuaHangMMTB.Columns.Contains("KetQuaKiem"))
                //    tblPhieuMuaHangMMTB.Columns.Add("KetQuaKiem", typeof(string));

                //if (!tblPhieuMuaHangMMTB.Columns.Contains("SoLoID"))
                //    tblPhieuMuaHangMMTB.Columns.Add("SoLoID", typeof(string));

                //if (!tblPhieuMuaHangMMTB.Columns.Contains("SoLo"))
                //    tblPhieuMuaHangMMTB.Columns.Add("SoLo", typeof(string));

                //if (!_tblDetails.Columns.Contains("KiemVT"))
                //    _tblDetails.Columns.Add("KiemVT", typeof(string));

                //if (!_tblDetails.Columns.Contains("SoLoID"))
                //    _tblDetails.Columns.Add("SoLoID", typeof(string));

                //if (!_tblDetails.Columns.Contains("SoLo"))
                //    _tblDetails.Columns.Add("SoLo", typeof(string));

                //if (!_tblDetails.Columns.Contains("GhiChuQC"))
                //    _tblDetails.Columns.Add("GhiChuQC", typeof(string));

                //if (!_tblDetails.Columns.Contains("MaNPL"))
                //    _tblDetails.Columns.Add("MaNPL", typeof(string));

                //if (!_tblDetails.Columns.Contains("qcShipment"))
                //    _tblDetails.Columns.Add("qcShipment", typeof(int));

                //if (!_tblDetails.Columns.Contains("NgayNK"))
                //    _tblDetails.Columns.Add("NgayNK", typeof(string));
                /////QCKiemVai
                //DataTable tblqckiem = new DataTable();
                //string urlqc = $"{URL}ERPCanDoiNguyenPhuLieu/GET?action=GetQCKIEM&para1=&para2=";
                //string jsonqc = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlqc); }).Result;
                //tblqckiem = JsonConvert.DeserializeObject<DataTable>(jsonqc);
                //if (jsonqc != "[]")
                //    tblqckiem = JsonConvert.DeserializeObject<DataTable>(jsonqc);

                //var qcLookup = tblqckiem.AsEnumerable()
                //   .GroupBy(x => x.Field<string>("MaPhieuMH"))
                //   .ToDictionary(
                //       g => g.Key,
                //       g =>
                //       {
                //           bool allNull = g.All(x => x["KetQuaKiem"] == DBNull.Value);

                //           bool hasNull = g.Any(x => x["KetQuaKiem"] == DBNull.Value);

                //           bool hasFail = g.Any(x =>
                //               x["KetQuaKiem"] != DBNull.Value &&
                //               Convert.ToInt32(x["KetQuaKiem"]) == 0
                //           );

                //           bool allPass = g.All(x =>
                //               x["KetQuaKiem"] != DBNull.Value &&
                //               Convert.ToInt32(x["KetQuaKiem"]) == 1
                //           );

                //           string ketQuaKiem;
                //           if (allNull)
                //               ketQuaKiem = "";
                //           else if (hasNull || hasFail)
                //               ketQuaKiem = "FAIL";
                //           else if (allPass)
                //               ketQuaKiem = "PASS";
                //           else
                //               ketQuaKiem = "FAIL";

                //           string soloid = g.First()["SoLoID"]?.ToString() ?? "";
                //           string solo = g.First()["SoLo"]?.ToString() ?? "";

                //           return new
                //           {
                //               KetQuaKiem = ketQuaKiem,
                //               SoLoID = soloid,
                //               SoLo = solo
                //           };
                //       }
                //   );

                foreach (DataRow r in tblPhieuMuaHangMMTB.Rows)
                {
                    string maPhieu = r["MaPhieuMH"]?.ToString();
                    string maTienTePhieu = r["MaTienTe"]?.ToString() ?? "VND";

                    decimal tongCPPS = GetTongCPPS_ByMaPhieu(maPhieu, maTienTePhieu);
                    decimal tongCPVC = GetTongCPVC_ByMaPhieu(maPhieu, maTienTePhieu);
                    decimal tongCPVT = GetTongCPMMTB_ByMaPhieu(maPhieu, maTienTePhieu);
                    r["TongCPPS"] = tongCPPS;
                    r["TongCPVC"] = tongCPVC;
                    r["TongCPVT"] = tongCPVT;
                    decimal tongTien = tongCPPS + tongCPVC + tongCPVT;
                    r["TongTien"] = tongTien;

                    //if (!string.IsNullOrEmpty(maPhieu) && qcLookup.TryGetValue(maPhieu, out var qc))
                    //{
                    //    r["KetQuaKiem"] = qc.KetQuaKiem;
                    //    //r["KPL"] = qcLookup[maPhieu].KPL;
                    //    //r["KNL"] = qcLookup[maPhieu].KNL;
                    //    r["SoLoID"] = qc.SoLoID;
                    //    r["SoLo"] = qc.SoLo;
                    //}
                    //else
                    //{
                    //    r["KetQuaKiem"] = DBNull.Value;
                    //    //r["KPL"] = DBNull.Value;
                    //    //r["KNL"] = DBNull.Value;
                    //    r["SoLoID"] = DBNull.Value;
                    //    r["SoLo"] = DBNull.Value;
                    //}

                    string mancc = r["MaNCC"]?.ToString() ?? "";

                    string maTienTe = r["MaTienTe"]?.ToString() ?? "VND";

                    decimal gia = 1;

                    if (!string.IsNullOrEmpty(mancc) && maTienTe != "VND")
                    {
                        string url1 = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={maTienTe}&para2={"VND"}&para3=&para4=&para5=";

                        string json1 = Task.Run(async () => await _clientExtension.GetAsnyc(url1)).Result;

                        DataTable tblTyGia = JsonConvert.DeserializeObject<DataTable>(json1);

                        if (tblTyGia != null && tblTyGia.Rows.Count > 0)
                        {
                            gia = Convert.ToDecimal(tblTyGia.Rows[0]["TyGia"]);
                        }

                    }
                    _gia = gia;
                    _tygia = gia;
                    // 6. Tính TongTienVND
                    if (maTienTe == "VND")
                        r["TongTienVND"] = tongTien;
                    else
                        r["TongTienVND"] = tongTien * gia;
                }

                gridControl2.DataSource = tblPhieuMuaHangMMTB;
                gridControl2.RefreshDataSource();
                RestoreFocusRowMMTB();

            }
            catch (Exception ex)
            {

            }
        }

        private void gridView5_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gridView5_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "CTMMTB";
        }

        private void gridView5_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            DataRow masterRow = view.GetDataRow(e.RowHandle);
            if (masterRow == null) return;

            string maPhieuMH = masterRow["MaPhieuMH"]?.ToString();

            if (string.IsNullOrEmpty(maPhieuMH) || _tblDetails == null)
            {
                e.ChildList = null;
                return;
            }


            DataView dv = new DataView(_tblDetailsMMTB);
            dv.RowFilter = $"MaPhieuMH = '{maPhieuMH.Replace("'", "''")}'";

            e.ChildList = dv;

        }

        private void gridView5_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || e.RowHandle < 0) return;

            if (e.Column.FieldName == "TrangThai")
            {
                string trangThai = view.GetRowCellValue(e.RowHandle, "TrangThai")?.ToString();

                if (string.IsNullOrEmpty(trangThai)) return;
                switch (trangThai)
                {
                    case "Đang xử lý":
                        e.Appearance.BackColor = Color.Orange;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    case "Đã xác nhận":
                        e.Appearance.BackColor = Color.LightGreen;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    case "Thành công":
                        e.Appearance.BackColor = Color.LightSkyBlue;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    case "Đã hủy":
                        e.Appearance.BackColor = Color.Red;
                        e.Appearance.ForeColor = Color.Black;
                        break;

                    default:
                        break;
                }
            }

            if (e.Column.FieldName == "TongTien")
            {
                e.Appearance.ForeColor = Color.Red;
            }

            if (e.Column.FieldName == "MaTienTe")
            {
                e.Appearance.ForeColor = Color.Red;
            }
        }

        private void grcPhieuMHVatTu_Click_1(object sender, EventArgs e)
        {

        }

        private void gridView5_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                DataRow rowFocused = gridView5.GetFocusedDataRow() as DataRow;
                if (rowFocused == null) return;

                if (gridView5.FocusedColumn == gridColumn85)
                {

                    bool isAdmin = string.Equals(
                        GlobleData.UserName?.ToString(),
                        "admin",
                        StringComparison.OrdinalIgnoreCase
                    );
                    if (!isAdmin)
                    {
                        string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                        string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                        DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                        bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                            r["ModuleID"]?.ToString() == "M.12.03.00" &&
                            r["AllowDuyet"] != DBNull.Value &&
                            Convert.ToBoolean(r["AllowDuyet"]) == true
                        );

                        if (!coQuyenDuyet)
                        {
                            XtraMessageBox.Show("Bạn không có quyền duyệt phiếu mua hàng!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    //DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                    string trangThai = rowFocused["TrangThai"]?.ToString();
                    switch (trangThai)
                    {

                        case "Đang xử lý":
                            XtraMessageBox.Show(
                                "Phiếu đã được duyệt, không được duyệt lại",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;

                        case "Đã xác nhận":
                        case "Thành công":
                            XtraMessageBox.Show(
                                "Phiếu đã xác nhận hoặc đã mua hàng thành công, không được duyệt lại",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                    }


                    bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                    if (IsDuyet)
                    {
                        XtraMessageBox.Show("Phiếu đã được duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        LoadPhieuMuaHang();
                        return;
                    }
                    bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhan);
                    if (IsXacNhan)
                    {
                        XtraMessageBox.Show("Đã xác nhận không được hủy duyệt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        LoadPhieuMuaHang();
                        return;
                    }
                    DialogResult messResult = MessageBox.Show(
                    $"Bạn có muốn {(!IsDuyet ? "duyệt" : "hủy duyệt")} {rowFocused["TenPhieu"]?.ToString()} không?",
                    "Thông báo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                    _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                    string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                    if (messResult != DialogResult.Yes) return;
                    string url = string.Format("{0}", URL + "ERPPOMuaMMTB/Post");
                    bool check = !IsDuyet;
                    ERPXacNhanPOMuaMMTBEntity objXetDuyet = new ERPXacNhanPOMuaMMTBEntity();
                    objXetDuyet.Action = "DuyetMuaHang";
                    objXetDuyet.MaPhieu = maphieumh;
                    objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                    objXetDuyet.IsXacNhan = !IsDuyet;
                    objXetDuyet.User = GlobleData.UserName;
                    objXetDuyet.GhiChu = "";

                    string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                    if (msResult.ToLower() == "true")
                    {
                        clsWaitForm.ShowSuccessForm(this, 2000);
                        this.DialogResult = DialogResult.OK;
                        LoadPhieuMuaHangMMTB();

                        string Title = $"Purchase Order {(!IsDuyet ? "cần soát xét" : "đã hủy xác nhận")}";
                        string Detail = $"Mã Phiếu: {rowFocused["TenPhieu"]?.ToString()}\nNhà cung cấp: {rowFocused["TenNhaCC"]?.ToString()}\nPOMH : {rowFocused["POMua"]}";
                        SendNotify(Title, Detail, "PKH", "ALL", 1);
                    }
                    else
                    {
                        XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                else if (gridView5.FocusedColumn == gridColumn102)
                {
                    bool isAdmin = string.Equals(
                        GlobleData.UserName?.ToString(),
                        "admin",
                        StringComparison.OrdinalIgnoreCase
                    );
                    if (!isAdmin)
                    {
                        string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                        string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                        DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                        bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                            r["ModuleID"]?.ToString() == "M.12.03.00" &&
                            r["AllowDuyet2"] != DBNull.Value &&
                            Convert.ToBoolean(r["AllowDuyet2"]) == true
                        );

                        if (!coQuyenDuyet)
                        {
                            XtraMessageBox.Show("Bạn không có quyền tick duyệt giám đốc!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    if (gridView5.FocusedRowHandle >= 0)
                    {
                        //DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                        bool.TryParse(rowFocused["IsDuyet1"]?.ToString(), out bool IsDuyet1);
                        if (!IsDuyet1)
                        {
                            XtraMessageBox.Show(
                                "Trưởng bộ phận chưa tick ký!.",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }


                        bool.TryParse(rowFocused["IsDuyet2"]?.ToString(), out bool IsDuyet);
                        if (IsDuyet)
                        {
                            XtraMessageBox.Show(
                                "Giám đốc đã ký, không được hủy ký!.",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }
                        DialogResult messResult = MessageBox.Show(
                        $"Bạn có muốn {(!IsDuyet ? "duyệt" : "hủy duyệt")} {rowFocused["TenPhieu"]?.ToString()} không?",
                        "Thông báo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                        _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                        string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                        if (messResult != DialogResult.Yes) return;
                        string url = string.Format("{0}", URL + "ERPPOMuaMMTB/Post");
                        bool check = !IsDuyet;
                        ERPXacNhanPOMuaMMTBEntity objXetDuyet = new ERPXacNhanPOMuaMMTBEntity();
                        objXetDuyet.Action = "DuyetMuaHang2";
                        objXetDuyet.MaPhieu = maphieumh;
                        objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                        objXetDuyet.IsXacNhan = !IsDuyet;
                        objXetDuyet.User = GlobleData.UserName;
                        objXetDuyet.GhiChu = "";

                        string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                        if (msResult.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 2000);
                            this.DialogResult = DialogResult.OK;
                            string PhieuYC = gridView6.GetFocusedRowCellValue("MaPhieu")?.ToString();
                            UpdateDuyet2MuaMMTB(maphieumh, PhieuYC, !IsDuyet);
                            LoadPhieuMuaHangMMTB();

                            string Title = $"Purchase Order {(!IsDuyet ? "xác nhận mua hàng" : "đã hủy xác nhận mua hàng")}";
                            string Detail = $"Mã Phiếu: {rowFocused["TenPhieu"]?.ToString()}\nNhà cung cấp: {rowFocused["TenNhaCC"]?.ToString()}\nPOMH : {rowFocused["POMua"]}";
                            SendNotify(Title, Detail, "PKH");

                        }
                        else
                        {
                            XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else if (gridView5.FocusedColumn == gridColumn105)
                {
                    try
                    {
                        bool isAdmin = string.Equals(
                            GlobleData.UserName?.ToString(),
                            "admin",
                            StringComparison.OrdinalIgnoreCase
                        );
                        if (!isAdmin)
                        {
                            string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                            string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                            DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                            bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                                r["ModuleID"]?.ToString() == "M.12.03.00" &&
                                r["AllowDuyet1"] != DBNull.Value &&
                                Convert.ToBoolean(r["AllowDuyet1"]) == true
                            );

                            if (!coQuyenDuyet)
                            {
                                XtraMessageBox.Show("Bạn không có quyền tick ký trưởng bộ phận!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }

                        if (gridView5.FocusedRowHandle >= 0)
                        {
                            bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet1);
                            if (!IsDuyet1)
                            {
                                XtraMessageBox.Show(
                                    "Người tạo chưa tick duyệt!. Người tạo vui lòng tick duyệt trước khi tick ký trưởng bộ phận",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                                return;
                            }



                            bool.TryParse(rowFocused["IsDuyet1"]?.ToString(), out bool IsDuyet);
                            if (IsDuyet)
                            {
                                XtraMessageBox.Show(
                                    "Trưởng bộ phận đã tick ký, không thể hủy ký",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                                return;
                            }
                            DialogResult messResult = MessageBox.Show(
                            $"Bạn có muốn {(!IsDuyet ? "duyệt" : "hủy duyệt")} {rowFocused["TenPhieu"]?.ToString()} không?",
                            "Thông báo",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                            _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                            string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                            if (messResult != DialogResult.Yes) return;
                            string url = string.Format("{0}", URL + "ERPPOMuaMMTB/Post");
                            bool check = !IsDuyet;
                            ERPXacNhanPOMuaMMTBEntity objXetDuyet = new ERPXacNhanPOMuaMMTBEntity();
                            objXetDuyet.Action = "DuyetMuaHang1";
                            objXetDuyet.MaPhieu = maphieumh;
                            objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                            objXetDuyet.IsXacNhan = !IsDuyet;
                            objXetDuyet.User = GlobleData.UserName;
                            objXetDuyet.GhiChu = "";

                            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                            if (msResult.ToLower() == "true")
                            {
                                clsWaitForm.ShowSuccessForm(this, 2000);
                                this.DialogResult = DialogResult.OK;
                                LoadPhieuMuaHangMMTB();

                                string Title = $"Purchase Order {(!IsDuyet ? "cần giám đốc xác nhận" : "trưởng bộ phận đã hủy soát xét")}";
                                string Detail = $"Mã Phiếu: {rowFocused["TenPhieu"]?.ToString()}\nNhà cung cấp: {rowFocused["TenNhaCC"]?.ToString()}\nPOMH : {rowFocused["POMua"]}";
                                SendNotify(Title, Detail, "PKH", "ALL", 0);

                            }
                            else
                            {
                                XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception exz)
                    {

                    }


                }
                else if (gridView5.FocusedColumn == gridColumn103)
                {
                    bool isAdmin = string.Equals(
                        GlobleData.UserName?.ToString(),
                        "admin",
                        StringComparison.OrdinalIgnoreCase
                    );
                    if (!isAdmin)
                    {
                        string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                        string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                        DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                        bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                            r["ModuleID"]?.ToString() == "M.12.03.00" &&
                            r["AllowXacNhan"] != DBNull.Value &&
                            Convert.ToBoolean(r["AllowXacNhan"]) == true
                        );

                        if (!coQuyenDuyet)
                        {
                            XtraMessageBox.Show("Bạn không có quyền xác nhận!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    if (gridView5.FocusedRowHandle >= 0)
                    {
                        //DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                        string trangThai = rowFocused["TrangThai"]?.ToString();
                        string NgayGHTT = "";
                        if (trangThai == "Đã hủy")
                        {
                            XtraMessageBox.Show(
                                "Phiếu đã hủy",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }

                        if (trangThai == "Đã xác nhận" || trangThai == "Thành công")
                        {
                            XtraMessageBox.Show(
                                "Phiếu đã xác nhận hoặc đã mua thành công, không thể xác nhận lại",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }
                        if (rowFocused != null)
                        {
                            string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                            if (string.IsNullOrEmpty(MaPhieuMH))
                            {
                                XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            bool.TryParse(rowFocused["IsDuyet"]?.ToString(), out bool IsDuyet);
                            if (!IsDuyet)
                            {
                                XtraMessageBox.Show("Vui lòng duyệt trước mới xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            bool.TryParse(rowFocused["IsDuyet1"]?.ToString(), out bool IsDuyet1);
                            bool.TryParse(rowFocused["IsDuyet2"]?.ToString(), out bool IsDuyet2);
                            if (!IsDuyet1 || !IsDuyet2)
                            {
                                XtraMessageBox.Show("Vui lòng trình ký đủ mới xác nhận", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            DialogResult messResult = MessageBox.Show(
                            $"Bạn có muốn xác nhận {rowFocused["TenPhieu"]?.ToString()} không?",
                            "Thông báo",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                            _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                            string maphieumh = rowFocused["MaPhieuMH"]?.ToString();
                            if (messResult != DialogResult.Yes) return;

                            using (frmChonNgayGHTT frm = new frmChonNgayGHTT())
                            {
                                if (frm.ShowDialog() != DialogResult.OK)
                                {
                                    return;
                                }

                                NgayGHTT = frm.Ngay;
                            }
                            string PhieuYC = gridView5.GetFocusedRowCellValue("MaPhieu")?.ToString();
                            string url = $"{URL}NhaCC/GePMHMMTB?action=GETPMHXH&para1={MaPhieuMH}&para2=&para3=&para4=&para5=";
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                            DataTable _dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                            DataTable tblDG = CreateDatatableToSave1();
                            DateTime today = DateTime.Today;
                            //_dtTable.Columns["NgayDuKienHV"].DataType = typeof(string);
                            foreach (DataRow dr in _dtTable.Rows)
                            {
                                dr["IsXacNhan"] = 1;
                                int soSom = int.TryParse(dr["SoNgayGHSom"]?.ToString(), out int s1) ? s1 : 0;
                                int soTre = int.TryParse(dr["SoNgayGHTre"]?.ToString(), out int s2) ? s2 : 0;

                                DateTime ngaySom = today.AddDays(soSom);
                                DateTime ngayTre = today.AddDays(soSom + soTre);

                                DataRow newRow = tblDG.NewRow();
                                newRow["ID"] = 0;
                                newRow["LoaiHang"] = "";
                                newRow["IsTB"] = dr["IsTB"];
                                newRow["MaPhieuMH"] = dr["MaPhieuMH"];
                                newRow["MaPhieuBG"] = dr["MaPhieuBG"];
                                newRow["TenPhieu"] = dr["TenPhieu"];
                                newRow["MaNCC"] = dr["MaNCC"];
                                newRow["MaHang"] = dr["MaHang"]?.ToString();
                                newRow["MaCL"] = dr["MaCL"]?.ToString();
                                newRow["MaNhom"] = dr["MaNhom"];
                                newRow["MaKho"] = dr["MaKho"];
                                newRow["TenHang"] = dr["TenHang"];
                                newRow["SoSeri"] = "";
                                newRow["DonVi"] = dr["DonVi"];
                                newRow["MauMa"] = dr["MauMa"];
                                newRow["XuatXu"] = dr["XuatXu"];
                                newRow["HangSX"] = dr["HangSX"];
                                newRow["SoLuongMuaThem"] = dr["SoLuongMuaThem"];
                                newRow["DonGia"] = dr["DonGia"];
                                newRow["ThanhTien"] = dr["ThanhTien"];
                                newRow["Thue"] = dr["Thue"] == DBNull.Value ? 0 : dr["Thue"];
                                newRow["CPVanChuyen"] = 0;
                                newRow["ChiPhiTB"] = dr["ChiPhiTB"];
                                newRow["ChiPhiKhac"] = "";
                                newRow["TienTeID"] = dr["TienTeID"];
                                newRow["MaTienTePhieuMH"] = dr["MaTienTePhieuMH"];
                                newRow["TyGiaThanhToan"] = _gia;
                                newRow["ChietKhau"] = dr["ChietKhau"] == DBNull.Value ? 0 : dr["ChietKhau"];
                                newRow["TTChietKhau"] = dr["TTChietKhau"] == null ? "" : dr["TTChietKhau"];
                                newRow["POMua"] = dr["POMua"];
                                newRow["NgayDuKienHV"] = $"{ngaySom:dd/MM/yyyy} - {ngayTre:dd/MM/yyyy}";
                                newRow["NgayGiaoHangYC"] = dr["NgayGiaoHangYC"] == null ? DBNull.Value : dr["NgayGiaoHangYC"];
                                newRow["NgayHieuLuc"] = dr["NgayHieuLuc"] == null ? DBNull.Value : dr["NgayHieuLuc"];
                                newRow["NgayXacNhan"] = DBNull.Value;
                                newRow["NguoiTao"] = GlobleData.UserName.ToString();
                                newRow["NgayTao"] = DateTime.Now.ToString("yyyy-MM-dd");
                                newRow["NguoiSua"] = "";
                                newRow["NgaySua"] = DBNull.Value;
                                newRow["GhiChu"] = dr["GhiChu"];
                                newRow["GhiChuTB"] = "";
                                newRow["IsDuyet"] = 0;
                                newRow["IsXacNhan"] = 1;
                                newRow["PhieuYC"] = PhieuYC;
                                tblDG.Rows.Add(newRow);
                            }

                            string url2 = string.Format("{0}?", URL + "NhaCC/PospXNMMTB");
                            string mss1 = Task.Run(async () =>
                            {
                                return await _clientExtension.PostAsync(url2, tblDG);
                            }).Result;
                            POSTNGUOIXNMMTB(NgayGHTT);
                            if (mss1.ToLower() == "true")
                            {
                                clsWaitForm.ShowSuccessForm(this, 2000);
                                this.DialogResult = DialogResult.OK;

                                UpdateXNMuaMMTB(MaPhieuMH, PhieuYC, true);
                                LoadPhieuMuaHangMMTB();

                                string Title = $"Purchase Order {(!IsDuyet ? "đã xác nhận hàng về" : "đã hủy xác nhận hàng về")}";
                                string Detail = $"Mã Phiếu: {rowFocused["TenPhieu"]?.ToString()}\nNhà cung cấp: {rowFocused["TenNhaCC"]?.ToString()}\nPOMH : {rowFocused["POMua"]}";
                                SendNotify(Title, Detail, "PKH");
                                SendNotify(Title, Detail, "KHO");

                            }
                            else
                            {
                                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }


                        }
                    }
                }
                else if (gridView5.FocusedColumn == gridColumn104)
                {
                    bool isAdmin = string.Equals(
                        GlobleData.UserName?.ToString(),
                        "admin",
                        StringComparison.OrdinalIgnoreCase
                    );
                    if (!isAdmin)
                    {
                        string urpq = string.Format("{0}?userID={1}", URL + "PhanQuyenPOMH/Get", GlobleData.UserName.ToString());
                        string jsonpq = Task.Run(async () => { return await _clientExtension.GetAsnyc(urpq); }).Result;
                        DataTable tblpomh = JsonConvert.DeserializeObject<DataTable>(jsonpq);
                        bool coQuyenDuyet = tblpomh.AsEnumerable().Any(r =>
                            r["ModuleID"]?.ToString() == "M.12.03.00" &&
                            r["AllowXacNhan"] != DBNull.Value &&
                            Convert.ToBoolean(r["AllowXacNhan"]) == true
                        );

                        if (!coQuyenDuyet)
                        {
                            XtraMessageBox.Show("Bạn không có quyền xác nhận!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    if (gridView5.FocusedRowHandle >= 0)
                    {
                        //DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
                        string trangThai = rowFocused["TrangThai"]?.ToString();
                        //string ketquakiem = rowFocused["KetQuaKiem"]?.ToString();
                        string ghiChuKetThucSom = "";
                        if (trangThai == "Đã hủy")
                        {
                            XtraMessageBox.Show(
                                "Phiếu đã hủy",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }
                        bool.TryParse(rowFocused["IsDuyet2"]?.ToString(), out bool IsDuyet);
                        if (!IsDuyet)
                        {
                            XtraMessageBox.Show("Vui lòng duyệt trước mới xác nhận mua hàng thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            LoadPhieuMuaHang();
                            return;
                        }

                        bool.TryParse(rowFocused["IsXacNhan"]?.ToString(), out bool IsXacNhanngaygh);
                        if (!IsXacNhanngaygh)
                        {
                            XtraMessageBox.Show(
                                "Phiếu chưa xác nhận ngày giao hàng",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }
                        bool.TryParse(rowFocused["IsKetThuc"]?.ToString(), out bool IsKetThuc);
                        if (IsKetThuc)
                        {
                            XtraMessageBox.Show(
                                "Phiếu đã mua hàng thành công",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }

                        //if (ketquakiem != "PASS")
                        //{
                        //    DialogResult result = XtraMessageBox.Show(
                        //        "Phiếu hàng về chưa đủ, bạn có muốn kết thúc phiếu sớm không?",
                        //        "Thông báo",
                        //        MessageBoxButtons.OKCancel,
                        //        MessageBoxIcon.Warning
                        //    );

                        //    if (result != DialogResult.OK)
                        //    {
                        //        return;
                        //    }

                        //    using (frmNGCKetThucPMHSom frm = new frmNGCKetThucPMHSom())
                        //    {
                        //        if (frm.ShowDialog() != DialogResult.OK)
                        //        {
                        //            return;
                        //        }

                        //        ghiChuKetThucSom = frm.GhiChu;
                        //    }
                        //}
                        bool.TryParse(rowFocused["IsKetThuc"]?.ToString(), out bool IsXacNhan);
                        DialogResult messResult = MessageBox.Show(
                        $"Bạn có muốn {(!IsXacNhan ? "xác nhận thành công" : "hủy xác nhận thành công")} {rowFocused["TenPhieu"]?.ToString()} không?",
                        "Thông báo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                        _focusMaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                        if (messResult != DialogResult.Yes) return;
                        string url = string.Format("{0}", URL + "ERPPOMuaMMTB/Post");
                        ERPXacNhanPOMuaMMTBEntity objXetDuyet = new ERPXacNhanPOMuaMMTBEntity();
                        objXetDuyet.Action = "XacNhanMuahang";
                        objXetDuyet.MaPhieu = rowFocused["MaPhieuMH"]?.ToString();
                        objXetDuyet.NgayXN = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                        objXetDuyet.IsXacNhan = !IsXacNhan;
                        objXetDuyet.User = GlobleData.UserName;
                        objXetDuyet.GhiChu = ghiChuKetThucSom;

                        string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, objXetDuyet); }).Result;
                        //POSTNGUOIKT();
                        if (msResult.ToLower() == "true")
                        {
                            clsWaitForm.ShowSuccessForm(this, 2000);
                            this.DialogResult = DialogResult.OK;
                            LoadPhieuMuaHangMMTB();

                            string Title = $"Purchase Order {(!IsDuyet ? "đã kết thúc mua hàng" : "đã hủy kết thúc mua hàng")}";
                            string Detail = $"Mã Phiếu: {rowFocused["TenPhieu"]?.ToString()}\nNhà cung cấp: {rowFocused["TenNhaCC"]?.ToString()}\nPOMH : {rowFocused["POMua"]}";
                            SendNotify(Title, Detail, "PKH");

                        }
                        else
                        {
                            XtraMessageBox.Show(msResult, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void barButtonItem17_ItemClick(object sender, ItemClickEventArgs e)
        {
            btthemdhpcd();
        }

        private void btthemdhpcd()
        {
            frmPhieuMuaHangPCDDH frm = new frmPhieuMuaHangPCDDH(null, false, true, false);
            frm.Show();
            frm.MaximizeBox = true;
            frm.FormClosed += (s, e) =>
            {
                LoadDataForCurrentTab();
            };
        }

        private void gridView6_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadPhieuMuaHangMMTB();
        }

        private void gridView5_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "NguoiTao")
            {
                bool isChecked = Convert.ToBoolean(
                    view.GetRowCellValue(e.RowHandle, "IsDuyet")
                );

                //string text = e.CellValue?.ToString();
                string text = GetTenNV(e.CellValue?.ToString());

                CheckEditViewInfo checkInfo =
                    new CheckEditViewInfo(rItemCheckEditDuyet);

                CheckEditPainter painter = new CheckEditPainter();

                checkInfo.EditValue = isChecked;

                checkInfo.Bounds = new Rectangle(
                    e.Bounds.X + CHECK_LEFT,
                    e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
                    CHECK_SIZE,
                    CHECK_SIZE
                );

                checkInfo.CalcViewInfo(e.Graphics);

                ControlGraphicsInfoArgs args =
                    new ControlGraphicsInfoArgs(
                        checkInfo,
                        new GraphicsCache(e.Graphics),
                        checkInfo.Bounds
                    );

                painter.Draw(args);

                // Text
                Rectangle textRect = new Rectangle(
                    e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                );

                e.Appearance.DrawString(e.Cache, text, textRect);
                e.Handled = true;
            }
            if (e.Column.FieldName == "NguoiDuyet2")
            {
                bool isChecked = Convert.ToBoolean(
                   view.GetRowCellValue(e.RowHandle, "IsDuyet2")
               );

                //string text = e.CellValue?.ToString();
                string text = GetTenNV(e.CellValue?.ToString());

                CheckEditViewInfo checkInfo =
                    new CheckEditViewInfo(rItemCheckEditXN);

                CheckEditPainter painter = new CheckEditPainter();

                checkInfo.EditValue = isChecked;

                checkInfo.Bounds = new Rectangle(
                    e.Bounds.X + CHECK_LEFT,
                    e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
                    CHECK_SIZE,
                    CHECK_SIZE
                );

                checkInfo.CalcViewInfo(e.Graphics);

                ControlGraphicsInfoArgs args =
                    new ControlGraphicsInfoArgs(
                        checkInfo,
                        new GraphicsCache(e.Graphics),
                        checkInfo.Bounds
                    );

                painter.Draw(args);

                // Text
                Rectangle textRect = new Rectangle(
                    e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                );

                e.Appearance.DrawString(e.Cache, text, textRect);
                e.Handled = true;
            }

            if (e.Column.FieldName == "NguoiDuyet1")
            {
                bool isChecked = Convert.ToBoolean(
                   view.GetRowCellValue(e.RowHandle, "IsDuyet1")
               );

                //string text = e.CellValue?.ToString();
                string text = GetTenNV(e.CellValue?.ToString());

                CheckEditViewInfo checkInfo =
                    new CheckEditViewInfo(rItemCheckEditDuyet1);

                CheckEditPainter painter = new CheckEditPainter();

                checkInfo.EditValue = isChecked;

                checkInfo.Bounds = new Rectangle(
                    e.Bounds.X + CHECK_LEFT,
                    e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
                    CHECK_SIZE,
                    CHECK_SIZE
                );

                checkInfo.CalcViewInfo(e.Graphics);

                ControlGraphicsInfoArgs args =
                    new ControlGraphicsInfoArgs(
                        checkInfo,
                        new GraphicsCache(e.Graphics),
                        checkInfo.Bounds
                    );

                painter.Draw(args);

                // Text
                Rectangle textRect = new Rectangle(
                    e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                );

                e.Appearance.DrawString(e.Cache, text, textRect);
                e.Handled = true;
            }

            if (e.Column.FieldName == "NguoiXN")
            {
                bool isChecked = Convert.ToBoolean(
                   view.GetRowCellValue(e.RowHandle, "IsXacNhan")
               );

                //string text = e.CellValue?.ToString();
                string text = GetTenNV(e.CellValue?.ToString());

                CheckEditViewInfo checkInfo =
                    new CheckEditViewInfo(rItemCheckEditXacNhan);

                CheckEditPainter painter = new CheckEditPainter();

                checkInfo.EditValue = isChecked;

                checkInfo.Bounds = new Rectangle(
                    e.Bounds.X + CHECK_LEFT,
                    e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
                    CHECK_SIZE,
                    CHECK_SIZE
                );

                checkInfo.CalcViewInfo(e.Graphics);

                ControlGraphicsInfoArgs args =
                    new ControlGraphicsInfoArgs(
                        checkInfo,
                        new GraphicsCache(e.Graphics),
                        checkInfo.Bounds
                    );

                painter.Draw(args);

                // Text
                Rectangle textRect = new Rectangle(
                    e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                );

                e.Appearance.DrawString(e.Cache, text, textRect);
                e.Handled = true;
            }

            if (e.Column.FieldName == "NguoiKetThuc")
            {
                bool isChecked = Convert.ToBoolean(
                   view.GetRowCellValue(e.RowHandle, "IsKetThuc")
               );

                //string text = e.CellValue?.ToString();
                string text = GetTenNV(e.CellValue?.ToString());

                CheckEditViewInfo checkInfo =
                    new CheckEditViewInfo(rItemCheckEditKetThuc);

                CheckEditPainter painter = new CheckEditPainter();

                checkInfo.EditValue = isChecked;

                checkInfo.Bounds = new Rectangle(
                    e.Bounds.X + CHECK_LEFT,
                    e.Bounds.Y + (e.Bounds.Height - CHECK_SIZE) / 2,
                    CHECK_SIZE,
                    CHECK_SIZE
                );

                checkInfo.CalcViewInfo(e.Graphics);

                ControlGraphicsInfoArgs args =
                    new ControlGraphicsInfoArgs(
                        checkInfo,
                        new GraphicsCache(e.Graphics),
                        checkInfo.Bounds
                    );

                painter.Draw(args);

                // Text
                Rectangle textRect = new Rectangle(
                    e.Bounds.X + CHECK_LEFT + CHECK_SIZE + 6,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height
                );

                e.Appearance.DrawString(e.Cache, text, textRect);
                e.Handled = true;
            }
        }

        private void gridView6_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == gridColumn114)
            {
                string username = e.Value as string;
                if (!string.IsNullOrEmpty(username) && tenNVHienThi != null)
                {
                    string upperUser = username.Trim().ToUpper();
                    if (tenNVHienThi.ContainsKey(upperUser))
                    {
                        e.DisplayText = tenNVHienThi[upperUser];
                    }
                }
            }
            if (e.Column.FieldName == "NgayLap")
            {
                if (e.Value == null || e.Value == DBNull.Value) return;

                if (DateTime.TryParse(e.Value.ToString(), out DateTime dt))
                    e.DisplayText = dt.ToString("dd/MM/yyyy");
            }
            if (e.Column.FieldName == "IsDuyet")
            {
                if (e.Value == null || e.Value == DBNull.Value) { e.DisplayText = "Chưa duyệt"; return; }

                int isMua = 0;
                int.TryParse(e.Value.ToString(), out isMua);
                e.DisplayText = isMua == 1 ? "Đã duyệt" : "Chưa duyệt";
            }
        }

        private void LoadPhieuYCMuaHangMMTB()
        {
            try
            {
                DateTime FromDate = clsForrmatUtils.ConvertDate(fromDate.EditValue);
                DateTime ToDate = clsForrmatUtils.ConvertDate(toDate.EditValue);
                if (ToDate != DateTime.MinValue && FromDate != DateTime.MinValue)
                {
                    //Phieu MuaHang
                    DataTable tblPhieuMuaHangMMTB = new DataTable();
                    string url = $"{URL}NhaCC/GePMHMMTB?action=GetPYC&para1={pageIndex}&para2={pageSize}&para3=&para4=&para5=";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                    //string urldt = $"{URL}NhaCC/GePMHMMTB?action=GETPOALLDETAILS&para1=&para2=&para3=&para4=&para5=";
                    //string jsondt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldt); }).Result;
                    //_tblDetailsMMTB = JsonConvert.DeserializeObject<DataTable>(jsondt);

                    if (json != "[]")
                    {
                        tblPhieuMuaHangMMTB = JsonConvert.DeserializeObject<DataTable>(json);
                    }

                    if (tblPhieuMuaHangMMTB == null || tblPhieuMuaHangMMTB.Rows.Count == 0)
                    {
                        gridControl2.DataSource = tblPhieuMuaHangMMTB;
                        gridControl2.RefreshDataSource();
                        //RestoreFocusRow();
                        return;
                    }

                   

                    gridControl3.DataSource = tblPhieuMuaHangMMTB;
                    gridControl3.RefreshDataSource();
                    //RestoreFocusRowMMTB();

                }
                else
                {
                    gridControl3.DataSource = null;
                    gridControl3.RefreshDataSource();
                }

            }
            catch (Exception ex)
            {
            }
        }
        #endregion
        private void gridView6_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "CTYC";
        }

        private void gridView6_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            GridView view = sender as GridView;

            object isMuaVal = view.GetRowCellValue(e.RowHandle, "IsMua");
            int isMua = 0;
            int.TryParse(isMuaVal?.ToString(), out isMua);

            e.RelationCount = isMua == 1 ? 1 : 0;
            e.RelationCount = 1;
        }

        private void gridView6_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

           
            object isMuaVal = view.GetRowCellValue(e.RowHandle, "IsMua");
            int isMua = 0;
            int.TryParse(isMuaVal?.ToString(), out isMua);
            //if (isMua != 1) { e.ChildList = null; return; }

            object maPhieuVal = view.GetRowCellValue(e.RowHandle, "MaPhieu");
            string maPhieu = maPhieuVal?.ToString();
            if (string.IsNullOrEmpty(maPhieu)) { e.ChildList = null; return; }

            object loaiPhieuVal = view.GetRowCellValue(e.RowHandle, "LoaiPhieu");
            string loaiPhieu = loaiPhieuVal?.ToString() ?? "1";

            string url = $"{URL}NhaCC/GePMHMMTB?action=GetDetailsPYC&para1={maPhieu}&para2={loaiPhieu}&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            if (json == "[]" || string.IsNullOrEmpty(json)) { e.ChildList = null; return; }

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            e.ChildList = tbl.DefaultView;
        }

        private void CheckIsMua(string maPhieu)
        {
            _isMua = false;
            string url = $"{URL}NhaCC/GePMHMMTB?action=CheckIsMua&para1={maPhieu}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (string.IsNullOrEmpty(json)) return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0) return;
            int isMua = 0;
            int.TryParse(tbl.Rows[0]["IsMua"]?.ToString(), out isMua);
            _isMua = isMua == 1;
        }
        private Dictionary<string, string> GetTenNhanVien()
        {
            try
            {
                string url = string.Format("{0}?", URL + "GetTenNV/GetTenNV");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (!string.IsNullOrEmpty(json))
                {
                    DataTable dtNhanVien = JsonConvert.DeserializeObject<DataTable>(json);
                    string userNameCol = "UserName";
                    string tenNVCol = "TenNV";
                    return dtNhanVien.AsEnumerable()
                        .Where(row => row[userNameCol] != DBNull.Value && row[userNameCol] != null && !string.IsNullOrWhiteSpace(row.Field<string>(userNameCol))) // Lọc row null/rỗng
                        .GroupBy(row => row.Field<string>(userNameCol).Trim().ToUpper())
                        .ToDictionary(
                            g => g.Key,
                            g => g.First().Field<string>(tenNVCol) ?? g.Key
                        );
                }

            }
            catch (Exception ex)
            {
                clsWaitForm.ShowErrorForm(this, 2000);
            }
            return new Dictionary<string, string>();
        }


        #region Import/ Nhập số seri MMTB && Tự Động update MMTB

        private void gridView5_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DataRow drFocus = gridView5.GetFocusedDataRow();
            if (drFocus == null || e.Menu == null) return;
            if (drFocus["IsXacNhan"]?.ToString()?.ToLower() == "true")
            {
                DXMenuItem menudelete = new DXMenuItem();
                menudelete.Caption = "Số Seri vật tư";
                menudelete.Click += MenuEditSeri_Click;
                e.Menu.Items.Add(menudelete);
            }

        }

        private void MenuEditSeri_Click(object sender, EventArgs e)
        {
            DataRow drFocus = gridView5.GetFocusedDataRow();
            if (drFocus == null) return;
            DataRow rowFocusedpyc = gridView6.GetFocusedDataRow() as DataRow;
            if (rowFocusedpyc == null) return;
            ImportSeriMMTB(drFocus["MaPhieuMH"]?.ToString(), rowFocusedpyc["MaPhieu"]?.ToString());
        }
        private void ImportSeriMMTB(string PhieuMH_MMTB, string MaPhieuYC)
        {
            try
            {
                DataRow rowFocused = gridView5.GetFocusedDataRow() as DataRow;
                if (rowFocused != null)
                {
                    frmPOMH_PhieuMuaHangMMTB_Import_Seri frm = new frmPOMH_PhieuMuaHangMMTB_Import_Seri(PhieuMH_MMTB, MaPhieuYC, rowFocused);
                    frm.ShowDialog();
                    LoadPhieuMuaHangMMTB();
                }

            }
            catch (Exception ex)
            {

            }

        }

        private void UpdateXNMuaMMTB(string PhieuMH_MMTB, string MaPhieuYC, bool isMua)
        {
            try
            {
                string url = $"{URL}/ERP_POMH_PhieuMuaHangMMTB_Import_Seri/UpdateXNMuaMMTB?" +
                $"action=UpdateXNMuaMMTB&" +
                $"para1={HttpUtility.UrlEncode(PhieuMH_MMTB)}&" +
                $"para2={HttpUtility.UrlEncode(MaPhieuYC)}&" +
                $"para3={isMua}&";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
                //if (isMua)
                //{
                //    ImportSeriMMTB(PhieuMH_MMTB, MaPhieuYC);
                //}


            }
            catch (Exception ex)
            {

            }
             
        }

      

        private void UpdateDuyet2MuaMMTB(string PhieuMH_MMTB, string MaPhieuYC, bool IsDuyet2)
        {
            try
            {
                string url = $"{URL}/ERP_POMH_PhieuMuaHangMMTB_Import_Seri/UpdateXNMuaMMTB?" +
                $"action=UpdateDuyet2MuaMMTB&" +
                $"para1={HttpUtility.UrlEncode(PhieuMH_MMTB)}&" +
                $"para2={HttpUtility.UrlEncode(MaPhieuYC)}&" +
                $"para3={GlobleData.UserName}&" +
                $"para4={IsDuyet2}&";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
            }
            catch (Exception ex)
            {

            }

        }

        #endregion
     
        private void menuSimpleSpiment_Click(object sender, EventArgs e)
        {
            DataRow rowFocused = grvPhieuMHVatTu.GetFocusedDataRow() as DataRow;
            if (rowFocused != null)
            {
                string MaPhieuMH = rowFocused["MaPhieuMH"]?.ToString();
                if (string.IsNullOrEmpty(MaPhieuMH))
                {
                    XtraMessageBox.Show("Vui lòng chọn phiếu mua hàng để xem", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                frmERPPOMHSimpleShipment frm = new frmERPPOMHSimpleShipment(rowFocused);
                frm.ShowDialog();
                frm.MaximizeBox = true;
                if (frm.DialogResult == DialogResult.OK)
                {
                    LoadPhieuMuaHang();
                }
            }

        }


        #region Phân Trang

        private void btnPre_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (pageIndex > 1)
            {
                pageIndex = pageIndex - 1;
                barStaticItemPageIndex.EditValue = pageIndex.ToString();
                LoadDataForCurrentTab();
            }
            
        }

        

        private void btnNext_ItemClick(object sender, ItemClickEventArgs e)
        {
            pageIndex = pageIndex + 1;
            barStaticItemPageIndex.EditValue = pageIndex.ToString();
            LoadDataForCurrentTab();
        }

        private void txtPage_EditValueChanged(object sender, EventArgs e)
        {

        }
        #endregion
        private void grvPhieuMHVatTu_RowCellDefaultAlignment(object sender, DevExpress.XtraGrid.Views.Base.RowCellAlignmentEventArgs e)
        {

        }

        private void gridView6_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime FromDate = clsForrmatUtils.ConvertDate(fromDate.EditValue);
                DateTime ToDate = clsForrmatUtils.ConvertDate(toDate.EditValue);
                if(gridView6?.RowCount == 0)
                {
                    gridControl2.DataSource = new DataTable();
                    return;
                }
                DataRow rowFocusedpyc = gridView6.GetDataRow(0) as DataRow;
                if (rowFocusedpyc == null) return;
                string maphieuyc = rowFocusedpyc["MaPhieu"]?.ToString();
                //Phieu MuaHang
                DataTable tblPhieuMuaHangMMTB = new DataTable();
                string url = $"{URL}ERPPOMuaMMTB/Get?action=GetPhieuMuaHang&para1={maphieuyc}&para2=";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

                string urldt = $"{URL}NhaCC/GePMHMMTB?action=GETPOALLDETAILS&para1=&para2=&para3=&para4=&para5=";
                string jsondt = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldt); }).Result;
                _tblDetailsMMTB = JsonConvert.DeserializeObject<DataTable>(jsondt);

                if (json != "[]")
                {
                    tblPhieuMuaHangMMTB = JsonConvert.DeserializeObject<DataTable>(json);
                }
                else
                {
                    gridControl2.DataSource = null;
                    gridControl2.RefreshDataSource();
                }

                if (tblPhieuMuaHangMMTB == null || tblPhieuMuaHangMMTB.Rows.Count == 0)
                {
                    gridControl2.DataSource = tblPhieuMuaHangMMTB;
                    gridControl2.RefreshDataSource();
                    //RestoreFocusRow();
                    return;
                }

                if (!tblPhieuMuaHangMMTB.Columns.Contains("TongTien"))
                    tblPhieuMuaHangMMTB.Columns.Add("TongTien", typeof(decimal));

                if (!tblPhieuMuaHangMMTB.Columns.Contains("TongTienVND"))
                    tblPhieuMuaHangMMTB.Columns.Add("TongTienVND", typeof(decimal));

                //if (!tblPhieuMuaHangMMTB.Columns.Contains("KetQuaKiem"))
                //    tblPhieuMuaHangMMTB.Columns.Add("KetQuaKiem", typeof(string));

                //if (!tblPhieuMuaHangMMTB.Columns.Contains("SoLoID"))
                //    tblPhieuMuaHangMMTB.Columns.Add("SoLoID", typeof(string));

                //if (!tblPhieuMuaHangMMTB.Columns.Contains("SoLo"))
                //    tblPhieuMuaHangMMTB.Columns.Add("SoLo", typeof(string));

                //if (!_tblDetails.Columns.Contains("KiemVT"))
                //    _tblDetails.Columns.Add("KiemVT", typeof(string));

                //if (!_tblDetails.Columns.Contains("SoLoID"))
                //    _tblDetails.Columns.Add("SoLoID", typeof(string));

                //if (!_tblDetails.Columns.Contains("SoLo"))
                //    _tblDetails.Columns.Add("SoLo", typeof(string));

                //if (!_tblDetails.Columns.Contains("GhiChuQC"))
                //    _tblDetails.Columns.Add("GhiChuQC", typeof(string));

                //if (!_tblDetails.Columns.Contains("MaNPL"))
                //    _tblDetails.Columns.Add("MaNPL", typeof(string));

                //if (!_tblDetails.Columns.Contains("qcShipment"))
                //    _tblDetails.Columns.Add("qcShipment", typeof(int));

                //if (!_tblDetails.Columns.Contains("NgayNK"))
                //    _tblDetails.Columns.Add("NgayNK", typeof(string));
                /////QCKiemVai
                //DataTable tblqckiem = new DataTable();
                //string urlqc = $"{URL}ERPCanDoiNguyenPhuLieu/GET?action=GetQCKIEM&para1=&para2=";
                //string jsonqc = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlqc); }).Result;
                //tblqckiem = JsonConvert.DeserializeObject<DataTable>(jsonqc);
                //if (jsonqc != "[]")
                //    tblqckiem = JsonConvert.DeserializeObject<DataTable>(jsonqc);

                //var qcLookup = tblqckiem.AsEnumerable()
                //   .GroupBy(x => x.Field<string>("MaPhieuMH"))
                //   .ToDictionary(
                //       g => g.Key,
                //       g =>
                //       {
                //           bool allNull = g.All(x => x["KetQuaKiem"] == DBNull.Value);

                //           bool hasNull = g.Any(x => x["KetQuaKiem"] == DBNull.Value);

                //           bool hasFail = g.Any(x =>
                //               x["KetQuaKiem"] != DBNull.Value &&
                //               Convert.ToInt32(x["KetQuaKiem"]) == 0
                //           );

                //           bool allPass = g.All(x =>
                //               x["KetQuaKiem"] != DBNull.Value &&
                //               Convert.ToInt32(x["KetQuaKiem"]) == 1
                //           );

                //           string ketQuaKiem;
                //           if (allNull)
                //               ketQuaKiem = "";
                //           else if (hasNull || hasFail)
                //               ketQuaKiem = "FAIL";
                //           else if (allPass)
                //               ketQuaKiem = "PASS";
                //           else
                //               ketQuaKiem = "FAIL";

                //           string soloid = g.First()["SoLoID"]?.ToString() ?? "";
                //           string solo = g.First()["SoLo"]?.ToString() ?? "";

                //           return new
                //           {
                //               KetQuaKiem = ketQuaKiem,
                //               SoLoID = soloid,
                //               SoLo = solo
                //           };
                //       }
                //   );

                foreach (DataRow r in tblPhieuMuaHangMMTB.Rows)
                {
                    string maPhieu = r["MaPhieuMH"]?.ToString();
                    string maTienTePhieu = r["MaTienTe"]?.ToString() ?? "VND";

                    decimal tongCPPS = GetTongCPPS_ByMaPhieu(maPhieu, maTienTePhieu);
                    decimal tongCPVC = GetTongCPVC_ByMaPhieu(maPhieu, maTienTePhieu);
                    decimal tongCPVT = GetTongCPMMTB_ByMaPhieu(maPhieu, maTienTePhieu);
                    r["TongCPPS"] = tongCPPS;
                    r["TongCPVC"] = tongCPVC;
                    r["TongCPVT"] = tongCPVT;
                    decimal tongTien = tongCPPS + tongCPVC + tongCPVT;
                    r["TongTien"] = tongTien;

                    //if (!string.IsNullOrEmpty(maPhieu) && qcLookup.TryGetValue(maPhieu, out var qc))
                    //{
                    //    r["KetQuaKiem"] = qc.KetQuaKiem;
                    //    //r["KPL"] = qcLookup[maPhieu].KPL;
                    //    //r["KNL"] = qcLookup[maPhieu].KNL;
                    //    r["SoLoID"] = qc.SoLoID;
                    //    r["SoLo"] = qc.SoLo;
                    //}
                    //else
                    //{
                    //    r["KetQuaKiem"] = DBNull.Value;
                    //    //r["KPL"] = DBNull.Value;
                    //    //r["KNL"] = DBNull.Value;
                    //    r["SoLoID"] = DBNull.Value;
                    //    r["SoLo"] = DBNull.Value;
                    //}

                    string mancc = r["MaNCC"]?.ToString() ?? "";

                    string maTienTe = r["MaTienTe"]?.ToString() ?? "VND";

                    decimal gia = 1;

                    if (!string.IsNullOrEmpty(mancc) && maTienTe != "VND")
                    {
                        string url1 = $"{URL}NhaCC/GePMH?action=GETPTyGia&para1={maTienTe}&para2={"VND"}&para3=&para4=&para5=";

                        string json1 = Task.Run(async () => await _clientExtension.GetAsnyc(url1)).Result;

                        DataTable tblTyGia = JsonConvert.DeserializeObject<DataTable>(json1);

                        if (tblTyGia != null && tblTyGia.Rows.Count > 0)
                        {
                            gia = Convert.ToDecimal(tblTyGia.Rows[0]["TyGia"]);
                        }

                    }
                    _gia = gia;
                    _tygia = gia;
                    // 6. Tính TongTienVND
                    if (maTienTe == "VND")
                        r["TongTienVND"] = tongTien;
                    else
                        r["TongTienVND"] = tongTien * gia;
                }

                gridControl2.DataSource = tblPhieuMuaHangMMTB;
                gridControl2.RefreshDataSource();
                RestoreFocusRowMMTB();

            }
            catch (Exception ex)
            {

            }
        }

        #region Send To Notify
        private void SendNotify(string Title, string Detail, string SendTo, string BoPhan = "ALL", int Status = -1)
        {

            try
            {
                string url = $"{URL}SendToNotification/PushNotificationFrm?" +
                $"UserIDTao={HttpUtility.UrlEncode(GlobleData.UserName)}&" +
                $"FrmName={HttpUtility.UrlEncode(this.Name)}&" +
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
    }


}