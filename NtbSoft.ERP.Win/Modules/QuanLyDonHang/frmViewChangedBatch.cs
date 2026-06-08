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
    public partial class frmViewChangedBatch : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _maDH = string.Empty, _maLenhSX = string.Empty, _maKH = string.Empty, _maHang = string.Empty, _maDot = string.Empty, _maGop = string.Empty,_preBatch = string.Empty;
        int _isSua = 0;
        DataTable tblMaVT = new DataTable();
        private bool _isUpdatingFocus = false;
        DataTable tblAllSize = new DataTable();
        DataTable tblMauVT = new DataTable();

        public frmViewChangedBatch(string maDH, string maLenhSX, string maKH, string maHang, string maDot, string maGop,string preBatch)
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
            _preBatch = preBatch;
        }

        private void frmViewChangedBOM_Load(object sender, EventArgs e)
        {
            loadSearchLookUpItemcode();
            CreateRepoSearchLookUpChungLoaiCT();
            loadKhoSizeBOM();
            LoadTVMauVT();
            loadAllSize(_maKH, _maHang);
            labelControl3.Text = _preBatch;
            labelControl4.Text = _maDot;
            LoadOldBatch();
            LoadChangedBOMData();
            bandedGridViewNewBatch.FocusedRowChanged += bandedGridView1_FocusedRowChanged;
        }
        private DataTable BuildBOMDataTable(DataTable dtMain, DataTable dtSize, DataTable dtColorMap, DataTable dtColors)
        {
            DataTable dtResult = new DataTable();
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
            //dtResult.Columns.Add("Dot", typeof(string));

            var colorColumnMapping = new Dictionary<string, string>();
            if (dtColors != null)
            {
                foreach (DataRow colorRow in dtColors.Rows)
                {
                    string maMau = colorRow["MaMau"].ToString();
                    string tenMau = colorRow["TenMau"].ToString();
                    string colName = $"{tenMau}@Mau@{maMau}";
                    if (!dtResult.Columns.Contains(colName))
                    {
                        dtResult.Columns.Add(colName, typeof(string));
                        colorColumnMapping[colName] = maMau;
                    }
                }
            }

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
                    TachMau = r["TachMau"] != DBNull.Value ? Convert.ToBoolean(r["TachMau"]) : false,
                    DinhMucHaoHut = r["DinhMucHaoHut"] != DBNull.Value ? Convert.ToDecimal(r["DinhMucHaoHut"]) : 0m,
                    DinhMucChung = r["DinhMucChung"] != DBNull.Value ? Convert.ToDecimal(r["DinhMucChung"]) : 0m,
                    IsXetDuyet = r["IsXetDuyet"]?.ToString() ?? "",
                    MaCode = r["MaCode"]?.ToString() ?? "",
                    STTCode = r["STTCode"] != DBNull.Value ? Convert.ToInt32(r["STTCode"]) : 0,
                    IsActive = r["IsActive"] != DBNull.Value ? Convert.ToBoolean(r["IsActive"]) : false,
                    MaNhomChiTiet = r["MaNhomChiTiet"]?.ToString() ?? "",
                    MaDot = r["MaDot"]?.ToString() ?? ""
                    //Dot = r["Dot"]?.ToString() ?? ""
                })
                .OrderBy(g => g.Key.NPL ? 0 : 1)
                .ThenBy(g => g.Key.Sort)
                .ThenBy(g => g.Key.STT);

            int id = 1;
            foreach (var group in groupedData)
            {
                DataRow resultRow = dtResult.NewRow();
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
                //resultRow["Dot"] = group.Key.Dot;

                if (dtSize != null)
                {
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
                    foreach (var sg in sizeGroups)
                    {
                        var sizes = sg.OrderBy(r => Convert.ToInt32(r["Sort"])).ThenBy(r => r["MaSize"].ToString())
                            .Select(r => r["MaSize"].ToString()).Distinct();
                        var tenSizes = sg.OrderBy(r => Convert.ToInt32(r["Sort"]))
                            .Select(r => r["TenSize"] != DBNull.Value ? r["TenSize"].ToString() : r["MaSize"].ToString()).Distinct();
                        maSizeChungParts.Add($"{sg.Key.MaNhomSize}: {string.Join(", ", sizes)}");
                        sizeChungParts.Add($"{sg.Key.NhomSize}: {string.Join(", ", tenSizes)}");
                    }
                    resultRow["MaSizeChung"] = string.Join("; ", maSizeChungParts);
                    resultRow["Size"] = string.Join("; ", sizeChungParts);

                    if (!group.Key.IsKV)
                    {
                        var sizeInfo = dtSize.AsEnumerable()
                            .FirstOrDefault(r => r["MaSize"].ToString() == group.Key.KhoVaiID);
                        if (sizeInfo != null)
                        {
                            resultRow["KhoVaiID"] = sizeInfo["MaSize"].ToString();
                            resultRow["KhoVai"] = sizeInfo["TenSize"] != DBNull.Value ? sizeInfo["TenSize"].ToString() : group.Key.KhoVai;
                        }
                    }
                }

                foreach (var kvp in colorColumnMapping) resultRow[kvp.Key] = "";
                if (dtColorMap != null)
                {
                    var colorMappings = dtColorMap.AsEnumerable()
                        .Where(r => r["MaVTID"].ToString() == group.Key.MaVTID
                            && r["MaNhom"].ToString() == group.Key.MaNhom
                            && r["KhoVaiID"].ToString() == group.Key.KhoVaiID
                            && r["MaCode"].ToString() == group.Key.MaCode);
                    foreach (DataRow cm in colorMappings)
                    {
                        string maMau = cm["MaMau"].ToString();
                        var matched = colorColumnMapping.FirstOrDefault(x => x.Value == maMau);
                        if (!string.IsNullOrEmpty(matched.Key) && string.IsNullOrEmpty(resultRow[matched.Key].ToString()))
                            resultRow[matched.Key] = cm["MauVTID"].ToString();
                    }
                }

                dtResult.Rows.Add(resultRow);
            }

            return dtResult;
        }
        private void LoadOldBatch()
        {
            try
            {
                string urlMain = $"{URL}KhoiTaoBOMV1/Get?action=GETVTSP_V1&para1={_maKH}&para2={_maHang}&para3={_preBatch}";
                string urlSize = $"{URL}KhoiTaoBOMV1/Get?action=GETSIZESP_V1&para1={_maKH}&para2={_maHang}&para3={_preBatch}";
                string urlColorMap = $"{URL}KhoiTaoBOMV1/Get?action=GETMAUSP_V1&para1={_maKH}&para2={_maHang}&para3={_preBatch}";
                string urlColors = $"{URL}KhoiTaoBOMV1/Get?action=GETBANGMAU_V1&para1={_maKH}&para2={_maHang}&para3={_preBatch}";

                DataTable dtMain = JsonConvert.DeserializeObject<DataTable>(Task.Run(async () => await _clientExtension.GetAsnyc(urlMain)).Result);
                DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(Task.Run(async () => await _clientExtension.GetAsnyc(urlSize)).Result);
                DataTable dtColorMap = JsonConvert.DeserializeObject<DataTable>(Task.Run(async () => await _clientExtension.GetAsnyc(urlColorMap)).Result);
                DataTable dtColors = JsonConvert.DeserializeObject<DataTable>(Task.Run(async () => await _clientExtension.GetAsnyc(urlColors)).Result);

                if (dtMain == null || dtMain.Rows.Count == 0)
                {
                    gridControlOldBatch.DataSource = null;
                    return;
                }

                DataTable dtResult = BuildBOMDataTable(dtMain, dtSize, dtColorMap, dtColors);
                ProcessSizeData(dtResult);
                gridControlOldBatch.DataSource = dtResult;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi LoadOldBatch: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadChangedBOMData()
        {
            try
            {
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang tải dữ liệu");

                string urlMain = $"{URL}KhoiTaoBOMV1/Get?action=GETVTSP_V1&para1={_maKH}&para2={_maHang}&para3={_maDot}";
                string urlSize = $"{URL}KhoiTaoBOMV1/Get?action=GETSIZESP_V1&para1={_maKH}&para2={_maHang}&para3={_maDot}";
                string urlColorMap = $"{URL}KhoiTaoBOMV1/Get?action=GETMAUSP_V1&para1={_maKH}&para2={_maHang}&para3={_maDot}";
                string urlColors = $"{URL}KhoiTaoBOMV1/Get?action=GETBANGMAU_V1&para1={_maKH}&para2={_maHang}&para3={_maDot}";

                DataTable dtMain = JsonConvert.DeserializeObject<DataTable>(Task.Run(async () => await _clientExtension.GetAsnyc(urlMain)).Result);
                DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(Task.Run(async () => await _clientExtension.GetAsnyc(urlSize)).Result);
                DataTable dtColorMap = JsonConvert.DeserializeObject<DataTable>(Task.Run(async () => await _clientExtension.GetAsnyc(urlColorMap)).Result);
                DataTable dtColors = JsonConvert.DeserializeObject<DataTable>(Task.Run(async () => await _clientExtension.GetAsnyc(urlColors)).Result);

                if (dtMain == null || dtMain.Rows.Count == 0)
                {
                    gridControlNewBatch.DataSource = null;
                    SplashScreenManager.CloseForm(false);
                    return;
                }

                _isSua = dtMain.AsEnumerable()
                    .Where(r => r["IsSua"] != DBNull.Value)
                    .Select(r => Convert.ToInt32(r["IsSua"]))
                    .DefaultIfEmpty(0)
                    .Max();

                DataTable dtResult = BuildBOMDataTable(dtMain, dtSize, dtColorMap, dtColors);
                ProcessSizeData(dtResult);
                var mauColumns = dtResult.Columns
                    .Cast<DataColumn>()
                    .Where(c => c.ColumnName.Contains("@Mau@"))
                    .ToList();

                var rowsToRemove = new List<DataRow>();
                var seenKeys = new Dictionary<string, DataRow>();
                foreach (DataRow r in dtResult.Rows)
                {
                    string vtKey = $"{r["MaVTID"]}|{r["MaNhom"]}|{r["KhoVaiID"]}|{r["MaCode"]}";
                    string colorKey = string.Join("|", mauColumns.Select(c => r[c]?.ToString() ?? ""));
                    string key = $"{vtKey}||{colorKey}";
                    if (!seenKeys.ContainsKey(key)) seenKeys[key] = r;
                    else rowsToRemove.Add(r);
                }
                foreach (DataRow row in rowsToRemove) dtResult.Rows.Remove(row);
                dtResult.AcceptChanges();

                gridControlNewBatch.DataSource = dtResult;
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
            if (bandedGridViewNewBatch.IsGroupRow(e.RowHandle))
            {
                int groupIndex = bandedGridViewNewBatch.GetRowLevel(e.RowHandle);
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
                repoSearchCLCT1.DisplayMember = "TenNhomChiTiet";
                repoSearchCLCT1.ValueMember = "MaNhomChiTiet";
                string url = $"{URL}KhoiTaoBOMV1/Get?action=GETCHUNGLOAICHITIET";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblChungLoaiChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
                repoSearchCLCT.DataSource = tblChungLoaiChiTiet;
                repoSearchCLCT1.DataSource = tblChungLoaiChiTiet;
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
            repoKhoSize1.DisplayMember = "KhoVai";
            repoKhoSize1.ValueMember = "KhoVaiID";
            repoKhoSize1.DataSource = _tblKhoSize;
        }

        private void bandedGridViewNewBatch_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
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
        }

        private void bandedGridViewOldBatch_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
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
            if (bandedGridViewOldBatch.IsGroupRow(e.RowHandle))
            {
                int groupIndex = bandedGridViewNewBatch.GetRowLevel(e.RowHandle);
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

        private void bandedGridViewOldBatch_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
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

                var canDoiView = gridControlOldBatch.MainView as GridView;
                if (canDoiView != null)
                {
                    DataTable dtCanDoi = gridControlOldBatch.DataSource as DataTable;
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

        private void bandedGridViewOldBatch_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;

            DataRow row = null;
            try { row = view.GetDataRow(e.RowHandle); } catch { row = null; }
            if (row == null) return;

            bool isRowMissing = false;
            bool isDinhMucDifferent = false;
            bool isRowMissingInBOM = false;

            if (row.Table.Columns.Contains("IsRowMissing"))
                bool.TryParse(row["IsRowMissing"]?.ToString(), out isRowMissing);

            if (row.Table.Columns.Contains("IsDinhMucDifferent"))
                bool.TryParse(row["IsDinhMucDifferent"]?.ToString(), out isDinhMucDifferent);

            if (row.Table.Columns.Contains("IsRowMissingInBOM"))
                bool.TryParse(row["IsRowMissingInBOM"]?.ToString(), out isRowMissingInBOM);
            if (e.RowHandle == view.FocusedRowHandle)
            {
                e.Appearance.BackColor = Color.FromArgb(202, 232, 189);
                e.Appearance.Options.UseBackColor = true;
                return;
            }
            if (isRowMissingInBOM)
            {
                e.Appearance.BackColor = Color.FromArgb(255, 180, 180);
                e.Appearance.ForeColor = Color.DarkRed;
                e.Appearance.Options.UseBackColor = true;
                e.Appearance.Options.UseForeColor = true;
                return;
            }

            if (isRowMissing)
            {
                e.Appearance.BackColor = Color.LightCyan;
                e.Appearance.ForeColor = Color.DarkBlue;
                e.Appearance.Options.UseBackColor = true;
                e.Appearance.Options.UseForeColor = true;
                return;
            }

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
        }
        private void LoadTVMauVT()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETMAUVTTV";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblMauVT = JsonConvert.DeserializeObject<DataTable>(json);
        }
        private void bandedGridViewOldBatch_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
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
        }

        private void ApplyHighlighting()
        {
            try
            {
                DataTable dtNewBOM = gridControlNewBatch.DataSource as DataTable;
                DataTable dtOldBOM = gridControlOldBatch.DataSource as DataTable;

                if (dtNewBOM == null || dtOldBOM == null) return;

                var newBOMDict = new Dictionary<string, DataRow>();
                var newBOMSimple = new HashSet<string>();
                foreach (DataRow r in dtNewBOM.Rows)
                {
                    string key = $"{r["MaVTID"]}|{r["MaNhom"]}|{r["KhoVaiID"]}|{r["MaCode"]}";
                    if (!newBOMDict.ContainsKey(key)) newBOMDict.Add(key, r);
                    newBOMSimple.Add(key);
                }

                var oldBOMDict = new Dictionary<string, DataRow>();
                foreach (DataRow r in dtOldBOM.Rows)
                {
                    string key = $"{r["MaVTID"]}|{r["MaNhom"]}|{r["KhoVaiID"]}|{r["MaCode"]}";
                    if (!oldBOMDict.ContainsKey(key)) oldBOMDict.Add(key, r);
                }

                if (!dtNewBOM.Columns.Contains("IsRowMissing"))
                    dtNewBOM.Columns.Add("IsRowMissing", typeof(bool));
                if (!dtNewBOM.Columns.Contains("IsDinhMucDifferent"))
                    dtNewBOM.Columns.Add("IsDinhMucDifferent", typeof(bool));
                if (!dtOldBOM.Columns.Contains("IsRowMissingInBOM"))
                    dtOldBOM.Columns.Add("IsRowMissingInBOM", typeof(bool));

                foreach (DataRow newRow in dtNewBOM.Rows)
                {
                    string key = $"{newRow["MaVTID"]}|{newRow["MaNhom"]}|{newRow["KhoVaiID"]}|{newRow["MaCode"]}";
                    if (!oldBOMDict.ContainsKey(key))
                    {
                        newRow["IsRowMissing"] = true;
                        newRow["IsDinhMucDifferent"] = false;
                    }
                    else
                    {
                        newRow["IsRowMissing"] = false;
                        decimal.TryParse(newRow["DinhMucChung"]?.ToString(), out decimal newDM);
                        decimal.TryParse(oldBOMDict[key]["DinhMucChung"]?.ToString(), out decimal oldDM);
                        newRow["IsDinhMucDifferent"] = (newDM != oldDM);
                    }
                }

                foreach (DataRow oldRow in dtOldBOM.Rows)
                {
                    string key = $"{oldRow["MaVTID"]}|{oldRow["MaNhom"]}|{oldRow["KhoVaiID"]}|{oldRow["MaCode"]}";
                    oldRow["IsRowMissingInBOM"] = !newBOMSimple.Contains(key);
                }

                gridControlNewBatch.RefreshDataSource();
                gridControlOldBatch.RefreshDataSource();
            }
            catch (Exception ex)
            {
                return;
            }
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
            if (e.RowHandle == view.FocusedRowHandle)
            {
                e.Appearance.BackColor = Color.FromArgb(202, 232, 189);
                e.Appearance.Options.UseBackColor = true;
                return;
            }
            if (isRowMissing)
            {
                e.Appearance.BackColor = Color.LightCyan;
                e.Appearance.ForeColor = Color.DarkBlue;
                e.Appearance.Options.UseBackColor = true;
                e.Appearance.Options.UseForeColor = true;
                return;
            }
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

                var canDoiView = gridControlOldBatch.MainView as GridView;
                if (canDoiView != null)
                {
                    DataTable dtCanDoi = gridControlOldBatch.DataSource as DataTable;
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