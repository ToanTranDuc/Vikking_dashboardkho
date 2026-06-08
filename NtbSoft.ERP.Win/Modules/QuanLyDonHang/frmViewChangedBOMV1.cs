using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmViewChangedBOMV1 : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _maDH = string.Empty, _maLenhSX = string.Empty, _maKH = string.Empty, _maHang = string.Empty, _maDot = string.Empty, _maGop = string.Empty;
        int _isSua = 0;
        DataTable tblMaVT = new DataTable();
        private bool _isUpdatingFocus = false;
        DataTable tblAllSize = new DataTable();

        public frmViewChangedBOMV1(string maDH, string maLenhSX, string maKH, string maHang, string maDot, string maGop)
        {
            InitializeComponent();
            _clientExtension = new HttpClientExtension();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _maDH = maDH;
            _maLenhSX = maLenhSX;
            _maKH = maKH;
            _maHang = maHang;
            _maDot = maDot;
            _maGop = maGop;
        }

        private void frmViewChangedBOM_Load(object sender, EventArgs e)
        {
            loadSearchLookUpItemcode();
            CreateRepoSearchLookUpChungLoaiCT();
            loadKhoSizeBOM();
            LoadCanDoi();
            LoadChangedBOMData();

            var canDoiView = grcCanDoi.MainView as GridView;
            if (canDoiView != null)
            {
                canDoiView.RowCellStyle += grvCanDoi_RowCellStyle;
                canDoiView.FocusedRowChanged += grvCanDoi_FocusedRowChanged;
            }

            bandedGridView1.FocusedRowChanged += bandedGridView1_FocusedRowChanged;
        }

        private void LoadCanDoi()
        {
            try
            {
                string url = $"{URL}ERPDonHangTong/Get?action=GetNPL&para={_maDH}&para5={_maLenhSX}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                if (tbl == null || tbl.Rows.Count == 0)
                {
                    grcCanDoi.DataSource = null;
                    return;
                }
                DataTable dtAllSizes = GetBangSize(_maGop);
                DataTable dtERPSizes = GetERPSIZESP(_maGop);
                tbl = XuLyCotSize(tbl, dtAllSizes, dtERPSizes);
                grcCanDoi.DataSource = tbl;

                
                
                if (tbl != null && tbl.Rows.Count > 0 && tbl.Columns.Contains("Dot"))
                {
                    string dotValue = null;

                    foreach (DataRow row in tbl.Rows)
                    {
                        if (row["Dot"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["Dot"].ToString()))
                        {
                            dotValue = row["Dot"].ToString();
                            break;
                        }
                    }

                    labelControl3.Text = dotValue != null ? "Đợt: " + dotValue : "Đợt: ";
                }
                else
                {
                    labelControl3.Text = "";
                }

            }
            catch (Exception ex)
            {
                return;
            }
        }
        public DataTable GetBangSize(string parameter)
        {
            string url = string.Format("{0}?parameter={1}&action=GetBangSize", URL + "CanDoiDonHangTong/GetSize", parameter);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            if (tbl == null || tbl.Rows.Count == 0)
            {
                return null;
            }
            return tbl;
        }

        public DataTable GetERPSIZESP(string parameter)
        {
            string url = string.Format("{0}?parameter={1}&action=GetERPSIZESP", URL + "CanDoiDonHangTong/GetSize", parameter);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            if (tbl == null || tbl.Rows.Count == 0)
            {
                return null;
            }
            return tbl;
        }

        private DataTable XuLyCotSize(DataTable tblMain, DataTable dtAllSizes, DataTable dtERPSizes)
        {
            if (!tblMain.Columns.Contains("Size"))
            {
                tblMain.Columns.Add("Size", typeof(string));
            }

            // Dictionary chứa danh sách size theo NhomSize (Bảng 1 - BangSize)
            Dictionary<string, List<string>> dictAllSizes = new Dictionary<string, List<string>>();

            if (dtAllSizes != null && dtAllSizes.Rows.Count > 0)
            {
                var groupedAllSizes = dtAllSizes.AsEnumerable()
                    .GroupBy(r => r.Field<string>("NhomSize"))
                    .Select(g => new
                    {
                        NhomSize = g.Key,
                        Sizes = g.Select(r => r.Field<object>("TenSize")?.ToString() ?? "")
                                 .Distinct()
                                 .OrderBy(s => int.TryParse(s, out int num) ? 0 : 1)
                                 .ThenBy(s => int.TryParse(s, out int num) ? num : 0)
                                 .ThenBy(s => s)
                                 .ToList()
                    });

                foreach (var item in groupedAllSizes)
                {
                    dictAllSizes[item.NhomSize] = item.Sizes;
                }
            }
            Dictionary<string, Dictionary<string, List<string>>> dictVTSizes = new Dictionary<string, Dictionary<string, List<string>>>();

            if (dtERPSizes != null && dtERPSizes.Rows.Count > 0)
            {
                var groupedVTSizes = dtERPSizes.AsEnumerable()
                    .GroupBy(r => new
                    {
                        ID = r.Field<object>("ID")?.ToString(),
                        MaVTID = r.Field<object>("MaVTID")?.ToString(),
                        MaMauVT = r.Field<object>("MaMauVT")?.ToString(),
                        MaKhoVT = r.Field<object>("MaKhoVT")?.ToString(),
                        MaNhomVT = r.Field<object>("MaNhomVT")?.ToString(),
                        MaNhomChiTiet = r.Field<object>("MaNhomChiTiet")?.ToString(),
                        MaCode = r.Field<object>("MaCode")?.ToString()
                    })
                    .Select(g => new
                    {
                        Key = g.Key,
                        SizesByNhom = g.GroupBy(r => r.Field<string>("NhomSize"))
                                       .Select(ng => new
                                       {
                                           NhomSize = ng.Key,
                                           Sizes = ng.Select(r => r.Field<object>("TenSize")?.ToString() ?? "")
                                                     .Distinct()
                                                     .OrderBy(s => int.TryParse(s, out int num) ? 0 : 1)
                                                     .ThenBy(s => int.TryParse(s, out int num) ? num : 0)
                                                     .ThenBy(s => s)
                                                     .ToList()
                                       })
                                       .ToDictionary(x => x.NhomSize, x => x.Sizes)
                    });

                foreach (var item in groupedVTSizes)
                {
                    string key = item.Key.ID;
                    dictVTSizes[key] = item.SizesByNhom;
                }
            }
            foreach (DataRow row in tblMain.Rows)
            {
                string id = row["ID"]?.ToString();
                string sizeValue = "";

                if (!string.IsNullOrEmpty(id) && dictVTSizes.ContainsKey(id))
                {
                    var vtSizesByNhom = dictVTSizes[id];
                    bool isAllSize = true;
                    List<string> sizeStrings = new List<string>();
                    var sortedNhoms = vtSizesByNhom
                        .Select(kv => new
                        {
                            Key = kv.Key,
                            Value = kv.Value,
                            IsNum = int.TryParse(kv.Key, out int n),
                            NumVal = int.TryParse(kv.Key, out int n2) ? n2 : int.MaxValue
                        })
                        .OrderBy(x => x.IsNum ? 0 : 1)
                        .ThenBy(x => x.NumVal)
                        .ThenBy(x => x.Key);

                    foreach (var nhom in sortedNhoms)
                    {
                        string nhomSize = nhom.Key;
                        List<string> vtSizes = nhom.Value;
                        if (dictAllSizes.ContainsKey(nhomSize))
                        {
                            List<string> allSizes = dictAllSizes[nhomSize];
                            bool containsAllSizes = allSizes.All(s => vtSizes.Contains(s));

                            if (!containsAllSizes)
                            {
                                isAllSize = false;
                            }
                        }
                        else
                        {
                            isAllSize = false;
                        }
                        string sizeString = nhomSize + ":" + string.Join(",", vtSizes);
                        sizeStrings.Add(sizeString);
                    }
                    if (isAllSize && sizeStrings.Count > 0)
                    {
                        sizeValue = "AllSize";
                    }
                    else
                    {
                        sizeValue = string.Join("; ", sizeStrings);
                    }
                }

                row["Size"] = sizeValue;
            }

            return tblMain;
        }
        private void bandedGridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
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
        }

        private void LoadChangedBOMData()
        {
            try
            {
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang tải dữ liệu");

                // Query 1: Main BOM data
                string urlMain = $"{URL}KhoiTaoBOMV1/Get?action=GETVTSP_V1&para1={_maKH}&para2={_maHang}&para3={_maDot}";
                string jsonMain = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMain); }).Result;
                DataTable dtMain = JsonConvert.DeserializeObject<DataTable>(jsonMain);

                if (dtMain.Rows.Count == 0)
                {
                    gridControl1.DataSource = null;
                    SplashScreenManager.CloseForm(false);
                    return;
                }

                _isSua = dtMain.Rows.Count == 0
                    ? 0
                    : dtMain.AsEnumerable()
                        .Where(r => r["IsSua"] != DBNull.Value)
                        .Select(r => Convert.ToInt32(r["IsSua"]))
                        .DefaultIfEmpty(0)
                        .Max();

                // Query 2: Size data
                string urlSize = $"{URL}KhoiTaoBOMV1/Get?action=GETSIZESP_V1&para1={_maKH}&para2={_maHang}&para3={_maDot}";
                string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);

                // Query 3: Color mapping
                string urlColorMap = $"{URL}KhoiTaoBOMV1/Get?action=GETMAUSP_V1&para1={_maKH}&para2={_maHang}&para3={_maDot}";
                string jsonColorMap = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlColorMap); }).Result;
                DataTable dtColorMap = JsonConvert.DeserializeObject<DataTable>(jsonColorMap);

                // Query 4: All colors
                string urlColors = $"{URL}KhoiTaoBOMV1/Get?action=GETBANGMAU_V1&para1={_maKH}&para2={_maHang}&para3={_maDot}";
                string jsonColors = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlColors); }).Result;
                DataTable dtColors = JsonConvert.DeserializeObject<DataTable>(jsonColors);

                DataTable dtResult = new DataTable();

                // Add fixed columns
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

                // Group data
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
                        MaMauVT = r["MaMauVT"]?.ToString() ?? "",
                        MauVT = r["MauVT"]?.ToString() ?? "",
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
                    .OrderBy(g => g.Key.NPL ? 0 : 1)
                    .ThenBy(g => g.Key.Sort)
                    .ThenBy(g => g.Key.STT);

                int id = 1;

                foreach (var group in groupedData)
                {
                    DataRow resultRow = dtResult.NewRow();

                    // Fill fixed columns
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
                    resultRow["MauVTIDChung"] = group.First()["MauVTIDChung"]?.ToString() ?? "";
                    resultRow["MaMauVT"] = group.Key.MaMauVT;
                    resultRow["MauVT"] = group.Key.MauVT;
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

                    // Process size data
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

                    // Process KhoVai/Size
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

                    // Initialize color columns
                    foreach (var kvp in colorColumnMapping)
                    {
                        resultRow[kvp.Key] = "";
                    }

                    // Fill color columns
                    var colorMappingsForGroup = dtColorMap.AsEnumerable()
                        .Where(r => r["MaVTID"].ToString() == group.Key.MaVTID
                            && r["MaNhom"].ToString() == group.Key.MaNhom
                            && r["KhoVaiID"].ToString() == group.Key.KhoVaiID
                            && r["MaCode"].ToString() == group.Key.MaCode)
                        .ToList();

                    foreach (DataRow colorMapping in colorMappingsForGroup)
                    {
                        string mauVTID = colorMapping["MauVTID"].ToString();
                        string maMau = colorMapping["MaMau"].ToString();

                        var matchedColumn = colorColumnMapping.FirstOrDefault(x => x.Value == maMau);

                        if (!string.IsNullOrEmpty(matchedColumn.Key))
                        {
                            string currentValue = resultRow[matchedColumn.Key].ToString();
                            if (string.IsNullOrEmpty(currentValue))
                            {
                                resultRow[matchedColumn.Key] = mauVTID;
                            }
                        }
                    }

                    dtResult.Rows.Add(resultRow);
                }
                loadAllSize(_maKH, _maHang); //em có makh, mahnag k

                ProcessSizeData(dtResult);
                gridControl1.DataSource = dtResult;
                var mauColumns = dtResult.Columns
                    .Cast<DataColumn>()
                    .Where(c => c.ColumnName.Contains("@Mau@"))
                    .ToList();

                var rowsToRemove = new List<DataRow>();
                var seenColorCombinations = new Dictionary<string, DataRow>();

                foreach (DataRow r in dtResult.Rows)
                {
                    string vtKey = $"{r["MaVTID"]}|{r["MaNhom"]}|{r["KhoVaiID"]}|{r["MaCode"]}";
                    string colorKey = string.Join("|", mauColumns.Select(c => r[c]?.ToString() ?? ""));
                    string uniqueKey = $"{vtKey}||{colorKey}";
                    if (!seenColorCombinations.ContainsKey(uniqueKey))
                    {
                        seenColorCombinations.Add(uniqueKey, r);
                    }
                    else
                    {
                        rowsToRemove.Add(r);
                    }
                }
                
                foreach (DataRow row in rowsToRemove)
                {
                    dtResult.Rows.Remove(row);
                }
                
                dtResult.AcceptChanges();
                ApplyHighlighting();

                SplashScreenManager.CloseForm(false);
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (info.Column == gridColumn5)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (info.Column == gridColumn6)
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
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
        }

        
        private void loadSearchLookUpItemcode()
        {
            try
            {
                string urlVT = $"{URL}KhoiTaoBOMV1/Get?action=GETITEMCODE_V1&para1={_maKH.ToString()}&para2={_maHang.ToString()}&para3={_maDot.ToString()}";
                string jsonVT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlVT); }).Result;
                if (jsonVT != "[]")
                {
                    DataTable tblItemCode = JsonConvert.DeserializeObject<DataTable>(jsonVT);

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

        private void CreateRepoSearchLookUpChungLoaiCT()
        {
            try
            {
                repoSearchCLCT.DisplayMember = "TenNhomChiTiet";
                repoSearchCLCT.ValueMember = "MaNhomChiTiet";
                string url = $"{URL}KhoiTaoBOMV1/Get?action=GETCHUNGLOAICHITIET";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblChungLoaiChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
                repoSearchCLCT.DataSource = tblChungLoaiChiTiet;
            }
            catch (Exception ex)
            {
            }
        }

        private void loadKhoSizeBOM()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETKHOSIZEBOM";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _tblKhoSize = JsonConvert.DeserializeObject<DataTable>(json);
            repoKhoSize.DisplayMember = "KhoVai";
            repoKhoSize.ValueMember = "KhoVaiID";
            repoKhoSize.DataSource = _tblKhoSize;
        }

        private void ApplyHighlighting()
        {
            try
            {
                DataTable dtBOM = gridControl1.DataSource as DataTable;
                DataTable dtCanDoi = grcCanDoi.DataSource as DataTable;

                if (dtBOM == null || dtCanDoi == null) return;

                // Dictionary cho BOM - đầy đủ key
                var bomDict = new Dictionary<string, DataRow>();
                foreach (DataRow bomRow in dtBOM.Rows)
                {
                    string key = $"{bomRow["MaVTID"]}|{bomRow["MaNhom"]}|{bomRow["KhoVaiID"]}|{bomRow["MaCode"]}";
                    if (!bomDict.ContainsKey(key))
                    {
                        bomDict.Add(key, bomRow);
                    }
                }

                // Dictionary cho BOM - chỉ theo MaVTID và MaNhom (cho CanDoi check)
                var bomSimpleDict = new HashSet<string>();
                foreach (DataRow bomRow in dtBOM.Rows)
                {
                    string simpleKey = $"{bomRow["MaVTID"]}|{bomRow["MaNhom"]}|{bomRow["KhoVaiID"]}|{bomRow["MaCode"]}";
                    bomSimpleDict.Add(simpleKey);
                }

                // Dictionary cho CanDoi - đầy đủ key
                var canDoiDict = new Dictionary<string, DataRow>();
                foreach (DataRow canDoiRow in dtCanDoi.Rows)
                {
                    string key = $"{canDoiRow["MaVTID"]}|{canDoiRow["MaNhom"]}|{canDoiRow["KhoVaiID"]}|{canDoiRow["MaCode"]}";
                    if (!canDoiDict.ContainsKey(key))
                    {
                        canDoiDict.Add(key, canDoiRow);
                    }
                }

                // Thêm cột đánh dấu cho BOM
                if (!dtBOM.Columns.Contains("IsRowMissing"))
                    dtBOM.Columns.Add("IsRowMissing", typeof(bool));
                if (!dtBOM.Columns.Contains("IsDinhMucDifferent"))
                    dtBOM.Columns.Add("IsDinhMucDifferent", typeof(bool));

                // Thêm cột đánh dấu cho CanDoi
                if (!dtCanDoi.Columns.Contains("IsRowMissingInBOM"))
                    dtCanDoi.Columns.Add("IsRowMissingInBOM", typeof(bool));

                // Xử lý BOM - check đầy đủ key
                foreach (DataRow bomRow in dtBOM.Rows)
                {
                    string key = $"{bomRow["MaVTID"]}|{bomRow["MaNhom"]}|{bomRow["KhoVaiID"]}|{bomRow["MaCode"]}";

                    if (!canDoiDict.ContainsKey(key))
                    {
                        bomRow["IsRowMissing"] = true;
                        bomRow["IsDinhMucDifferent"] = false;
                    }
                    else
                    {
                        bomRow["IsRowMissing"] = false;
                        DataRow canDoiRow = canDoiDict[key];

                        decimal bomDinhMuc = 0;
                        decimal canDoiDinhMuc = 0;
                        decimal.TryParse(bomRow["DinhMucChung"]?.ToString(), out bomDinhMuc);
                        decimal.TryParse(canDoiRow["DMKH"]?.ToString(), out canDoiDinhMuc);

                        bomRow["IsDinhMucDifferent"] = (bomDinhMuc != canDoiDinhMuc);
                    }
                }

                foreach (DataRow canDoiRow in dtCanDoi.Rows)
                {
                    string simpleKey = $"{canDoiRow["MaVTID"]}|{canDoiRow["MaNhom"]}|{canDoiRow["KhoVaiID"]}|{canDoiRow["MaCode"]}";

                    // Chỉ check xem VT có tồn tại trong BOM không
                    if (!bomSimpleDict.Contains(simpleKey))
                    {
                        canDoiRow["IsRowMissingInBOM"] = true;
                    }
                    else
                    {
                        canDoiRow["IsRowMissingInBOM"] = false;
                    }
                }

                gridControl1.RefreshDataSource();
                grcCanDoi.RefreshDataSource();
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void grvCanDoi_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            info.GroupText = string.Format("{0}", info.GroupValueText);

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
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();
                e.Handled = true;
            }
            //if (info.Column != null && info.Column.FieldName == "NPL" && info.EditValue.ToString() != "")
            //{
            //     bool isChecked = Convert.ToBoolean(info.EditValue);

            //    if (isChecked)
            //    {
            //        info.GroupText = "Nguyên liệu";
            //    }
            //    else
            //    {
            //        info.GroupText = "Phụ liệu";
            //    }
            //}

            //string caption = info.Column.Caption;
            //if (info.Column.Caption == string.Empty)
            //    caption = info.Column.ToString();

            //if (info.Column == gridColumn15)
            //{
            //    info.GroupText = string.Format("{0}", info.GroupValueText);
            //}
            ////if (info.Column == gridColumn6)
            ////{
            ////    info.GroupText = string.Format("{0}", info.GroupValueText);
            ////}
            //if (bandedGridView1.IsGroupRow(e.RowHandle))
            //{
            //    int groupIndex = bandedGridView1.GetRowLevel(e.RowHandle);
            //    Color textColor = Color.White;
            //    Font font = e.Appearance.Font;
            //    if (groupIndex == 0)
            //    {
            //        textColor = Color.MediumBlue;
            //        font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
            //    }
            //    else if (groupIndex == 1)
            //    {
            //        textColor = Color.Red;
            //        font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
            //    }
            //    else if (groupIndex == 2)
            //    {
            //        textColor = Color.Maroon;
            //        font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
            //    }
            //    e.Appearance.ForeColor = textColor;
            //    e.Appearance.Font = font;
            //    e.DefaultDraw();
            //    e.Handled = true;

        }

        private void bandedGridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;

            DataRow row = null;
            try { row = view.GetDataRow(e.RowHandle); } catch { row = null; }
            if (row == null) return;

            bool isRowMissing = false;
            bool isDinhMucDifferent = false;

            if (row.Table.Columns.Contains("IsRowMissing"))
                bool.TryParse(row["IsRowMissing"]?.ToString(), out isRowMissing);

            if (row.Table.Columns.Contains("IsDinhMucDifferent"))
                bool.TryParse(row["IsDinhMucDifferent"]?.ToString(), out isDinhMucDifferent);

            // Tô xanh cyan nếu VT không có trong CanDoi
            if (isRowMissing)
            {
                e.Appearance.BackColor = Color.LightCyan;
                e.Appearance.ForeColor = Color.DarkBlue;
                e.Appearance.Options.UseBackColor = true;
                e.Appearance.Options.UseForeColor = true;
                return;
            }

            // Tô vàng cam nếu định mức khác nhau
            if (isDinhMucDifferent && e.Column.FieldName == "DinhMucChung")
            {
                e.Appearance.BackColor = Color.LightGoldenrodYellow;
                e.Appearance.ForeColor = Color.DarkOrange;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                e.Appearance.Options.UseBackColor = true;
                e.Appearance.Options.UseForeColor = true;
                e.Appearance.Options.UseFont = true;
                return;
            }
            if (e.RowHandle == view.FocusedRowHandle)
            {
                e.Appearance.BackColor = Color.FromArgb(202, 232, 189);
                e.Appearance.Options.UseBackColor = true;
            }
        }

        private void grvCanDoi_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var view = sender as GridView;
            if (view == null) return;

            DataRow row = null;
            try { row = view.GetDataRow(e.RowHandle); } catch { row = null; }
            if (row == null) return;

            bool isRowMissingInBOM = false;

            if (row.Table.Columns.Contains("IsRowMissingInBOM"))
                bool.TryParse(row["IsRowMissingInBOM"]?.ToString(), out isRowMissingInBOM);

            // Tô đỏ nếu VT không có trong BOM
            if (isRowMissingInBOM)
            {
                e.Appearance.BackColor = Color.LightCoral;
                e.Appearance.ForeColor = Color.DarkRed;
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                e.Appearance.Options.UseBackColor = true;
                e.Appearance.Options.UseForeColor = true;
                e.Appearance.Options.UseFont = true;
                return;
            }
            if (e.RowHandle == view.FocusedRowHandle)
            {
                e.Appearance.BackColor = Color.FromArgb(202, 232, 189);
                e.Appearance.Options.UseBackColor = true;
            }
        }

        private void grvCanDoi_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (_isUpdatingFocus) return;

            try
            {
                _isUpdatingFocus = true;

                var canDoiView = sender as GridView;
                if (canDoiView == null || e.FocusedRowHandle < 0) return;

                DataRow canDoiRow = canDoiView.GetDataRow(e.FocusedRowHandle);
                if (canDoiRow == null) return;

                string maVTID = canDoiRow["MaVTID"]?.ToString();
                string maNhom = canDoiRow["MaNhom"]?.ToString();
                string khoVaiID = canDoiRow["KhoVaiID"]?.ToString();
                string maCode = canDoiRow["MaCode"]?.ToString();

                if (bandedGridView1 != null)
                {
                    DataTable dtBOM = gridControl1.DataSource as DataTable;
                    if (dtBOM != null)
                    {
                        for (int i = 0; i < bandedGridView1.DataRowCount; i++)
                        {
                            DataRow bomRow = bandedGridView1.GetDataRow(i);
                            if (bomRow != null &&
                                bomRow["MaVTID"]?.ToString() == maVTID &&
                                bomRow["MaNhom"]?.ToString() == maNhom &&
                                bomRow["KhoVaiID"]?.ToString() == khoVaiID &&
                                bomRow["MaCode"]?.ToString() == maCode)
                            {
                                bandedGridView1.FocusedRowHandle = i;
                                bandedGridView1.MakeRowVisible(i);
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                _isUpdatingFocus = false;
            }
        }

        private void bandedGridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (_isUpdatingFocus) return;

            try
            {
                _isUpdatingFocus = true;

                var bomView = sender as BandedGridView;
                if (bomView == null || e.FocusedRowHandle < 0) return;

                DataRow bomRow = bomView.GetDataRow(e.FocusedRowHandle);
                if (bomRow == null) return;

                string maVTID = bomRow["MaVTID"]?.ToString();
                string maNhom = bomRow["MaNhom"]?.ToString();
                string khoVaiID = bomRow["KhoVaiID"]?.ToString();
                string maCode = bomRow["MaCode"]?.ToString();

                var canDoiView = grcCanDoi.MainView as GridView;
                if (canDoiView != null)
                {
                    DataTable dtCanDoi = grcCanDoi.DataSource as DataTable;
                    if (dtCanDoi != null)
                    {
                        for (int i = 0; i < canDoiView.DataRowCount; i++)
                        {
                            DataRow canDoiRow = canDoiView.GetDataRow(i);
                            if (canDoiRow != null &&
                                canDoiRow["MaVTID"]?.ToString() == maVTID &&
                                canDoiRow["MaNhom"]?.ToString() == maNhom &&
                                canDoiRow["KhoVaiID"]?.ToString() == khoVaiID &&
                                canDoiRow["MaCode"]?.ToString() == maCode)
                            {
                                canDoiView.FocusedRowHandle = i;
                                canDoiView.MakeRowVisible(i);
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                _isUpdatingFocus = false;
            }
        }
    }
}