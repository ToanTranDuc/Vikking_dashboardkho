using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
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
    public partial class frmPhanTichBom_XinNPL_History : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _khachHang = string.Empty, _maHang = string.Empty, _maPhieu = string.Empty, _Dot = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tblMauVT = new DataTable();
        DataTable tblAllSize = new DataTable();
        private int _rowhandle = 0, _stt = 0;
        private Dictionary<string, bool> cellsToHighlight = new Dictionary<string, bool>();
        public frmPhanTichBom_XinNPL_History(string khachHang,string maHang,string maPhieu,string Dot,string tenPhieu)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            tblMauVT = new DataTable();
            this._khachHang = khachHang;
            this._maHang = maHang;
            this._maPhieu = maPhieu;
            this._Dot = Dot;
            CreateRepoSearchLookUpChungLoaiCT();
            LoadTVMauVT();
            //LoadData();
            loadMauPhieu();
            //load dữ liệu phiếu
            loadDataPhieu();
            LockAllColumns();
            bandedGridView1.CustomColumnDisplayText += bandedGridView1_CustomColumnDisplayText;
            bandedGridView1.CustomDrawGroupRow += bandedGridView1_CustomDrawGroupRow;
            Dictionary<string, bool> cellsToHighlight = new Dictionary<string, bool>();
            bandedGridView1.RowCellStyle += bandedGridView1_RowCellStyle;
            bandedGridView1.OptionsBehavior.Editable = false;
            textEdit1.Text = tenPhieu;
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

        private void loadMauPhieu()
        {
            try
            {
                string makh = _khachHang;
                string mahang = _maHang;
                string maphieu = _maPhieu;
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


        private void loadDataPhieu()
        {
            try
            {
                string makh = _khachHang;
                string mahang = _maHang;
                string madot = _Dot;
                string maphieu = _maPhieu;

                if (string.IsNullOrEmpty(makh) || string.IsNullOrEmpty(mahang) ||
                    string.IsNullOrEmpty(madot) || string.IsNullOrEmpty(maphieu))
                    return;

                // BƯỚC 1: Lấy 4 bảng dữ liệu
                var url1 = string.Format("{0}XinNPLMayMau/Get?action=GETERP_NPLMayMau_V1Edit&para1={1}&para2={2}&para3={3}&para4={4}", URL, makh, mahang, madot, maphieu);
                var url2 = string.Format("{0}XinNPLMayMau/Get?action=GETBangMau_V1&para1={1}&para2={2}&para4={3}", URL, makh, mahang, maphieu);
                var url3 = string.Format("{0}XinNPLMayMau/Get?action=GETERP_NPLMayMauChiTiet_V1&para4={1}", URL, maphieu);
                var url4 = string.Format("{0}XinNPLMayMau/Get?action=GETERP_NPLMayMauChiTiet_V1Edit&para4={1}", URL, maphieu);

                string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
                string json2 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
                string json3 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url3); }).Result;
                string json4 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url4); }).Result;

                if (json1 == "[]") return;

                DataTable dtMain = JsonConvert.DeserializeObject<DataTable>(json1); // Có cột DotEdit
                DataTable dtMau = json2 == "[]" ? null : JsonConvert.DeserializeObject<DataTable>(json2);
                DataTable dtChiTietHistory = json3 == "[]" ? null : JsonConvert.DeserializeObject<DataTable>(json3); // History - data gốc
                DataTable dtChiTiet = json4 == "[]" ? null : JsonConvert.DeserializeObject<DataTable>(json4); // Data Edit - có nhiều đợt với cột MaDot

                // SO SÁNH TỪNG ĐỢT VỚI HISTORY ĐỂ TÌM Ô KHÁC NHAU
                cellsToHighlight.Clear();
                if (dtChiTiet != null && dtChiTietHistory != null && dtChiTiet.Columns.Contains("MaDot"))
                {
                    // Tạo dictionary cho History (KHÔNG có MaDot)
                    var historyDict = new Dictionary<string, decimal>();
                    foreach (DataRow rowHistory in dtChiTietHistory.Rows)
                    {
                        string keyHistory = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}",
                            rowHistory["MaPhieu"],
                            rowHistory["MaVTID"],
                            rowHistory["MauVTID"],
                            rowHistory["KhoVaiID"],
                            rowHistory["MaNhom"],
                            rowHistory["MaCode"],
                            rowHistory["MaMau"],
                            rowHistory["MaPhong"]);

                        decimal soLuongHistory = rowHistory["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(rowHistory["SoLuong"]);
                        historyDict[keyHistory] = soLuongHistory;
                    }

                    // Lấy danh sách các đợt
                    var dotList = dtChiTiet.AsEnumerable()
                        .Select(r => r["MaDot"]?.ToString() ?? "")
                        .Where(d => !string.IsNullOrEmpty(d))
                        .Distinct()
                        .OrderBy(d => d)
                        .ToList();

                    // So sánh TỪNG ĐỢT với History
                    foreach (string dot in dotList)
                    {
                        // Tạo dictionary cho đợt này
                        var dotDict = new Dictionary<string, decimal>();

                        var rowsOfDot = dtChiTiet.AsEnumerable()
                            .Where(r => r["MaDot"]?.ToString() == dot)
                            .ToList();

                        foreach (DataRow rowDot in rowsOfDot)
                        {
                            string keyDot = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}",
                                rowDot["MaPhieu"],
                                rowDot["MaVTID"],
                                rowDot["MauVTID"],
                                rowDot["KhoVaiID"],
                                rowDot["MaNhom"],
                                rowDot["MaCode"],
                                rowDot["MaMau"],
                                rowDot["MaPhong"]);

                            decimal soLuongDot = rowDot["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(rowDot["SoLuong"]);
                            dotDict[keyDot] = soLuongDot;
                        }

                        // So sánh đợt này với History
                        foreach (var kvp in dotDict)
                        {
                            string keyCompare = kvp.Key;
                            decimal soLuongDot = kvp.Value;

                            // Tạo key để highlight (CÓ thêm MaDot)
                            string keyHighlight = $"{keyCompare}_{dot}";

                            // Kiểm tra với History
                            if (historyDict.ContainsKey(keyCompare))
                            {
                                decimal soLuongHistory = historyDict[keyCompare];

                                // Nếu số lượng khác nhau thì đánh dấu tô màu
                                if (soLuongDot != soLuongHistory)
                                {
                                    cellsToHighlight[keyHighlight] = true;
                                }
                            }
                            else
                            {
                                // Key không tồn tại trong history (dòng mới) - tô màu
                                cellsToHighlight[keyHighlight] = true;
                            }
                        }
                    }
                }

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

                // BƯỚC 3: Tạo DataTable kết quả
                DataTable result = dtMain.Clone();

                if (result.Columns.Contains("MauVTID"))
                    result.Columns.Remove("MauVTID");
                if (!result.Columns.Contains("MauVTIDChung"))
                    result.Columns.Add("MauVTIDChung", typeof(string));

                // Thêm cột màu
                for (int i = 0; i < mauMaList.Count; i++)
                {
                    string colName = string.Format("{0}@Mau@{1}", mauTenList[i], mauMaList[i]);
                    if (!result.Columns.Contains(colName))
                        result.Columns.Add(colName, typeof(string));
                }

                // Thêm cột phòng+màu
                foreach (var phong in phongList)
                {
                    for (int i = 0; i < mauMaList.Count; i++)
                    {
                        string colName = string.Format("{0}@{1}@{2}", mauTenList[i], phong, mauMaList[i]);
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

                // BƯỚC 4: Group dữ liệu theo DotEdit
                var groupedData = new Dictionary<string, List<DataRow>>();
                foreach (DataRow mainRow in dtMain.Rows)
                {
                    string dotEdit = mainRow.Table.Columns.Contains("DotEdit") ? mainRow["DotEdit"]?.ToString() ?? "" : "";

                    string groupKey = string.Format("{0}_{1}_{2}_{3}_{4}_{5}",
                        mainRow["MaPhieu"],
                        mainRow["MaVTID"],
                        mainRow["KhoVaiID"],
                        mainRow["MaNhom"],
                        mainRow["MaCode"],
                        dotEdit); // Thêm DotEdit vào groupKey

                    if (!groupedData.ContainsKey(groupKey))
                        groupedData[groupKey] = new List<DataRow>();

                    groupedData[groupKey].Add(mainRow);
                }

                // BƯỚC 5: Xử lý từng nhóm
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

                    // Lấy DotEdit từ row hiện tại
                    string currentDotEdit = firstRow.Table.Columns.Contains("DotEdit") ? firstRow["DotEdit"]?.ToString() ?? "" : "";

                    // Gộp MauVTID
                    var mauVTIDList = new List<string>();
                    foreach (DataRow row in group.Value)
                    {
                        string mauVTID = row["MauVTID"].ToString();
                        if (!string.IsNullOrEmpty(mauVTID) && !mauVTIDList.Contains(mauVTID))
                            mauVTIDList.Add(mauVTID);
                    }
                    newRow["MauVTIDChung"] = string.Join(", ", mauVTIDList);

                    // Tạo map MaMau -> MauVTID từ chi tiết (LỌC THEO DotEdit = MaDot)
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
                            // Kiểm tra MaDot == DotEdit
                            string maDot = ctRow.Table.Columns.Contains("MaDot") ? ctRow["MaDot"]?.ToString() ?? "" : "";

                            if (ctRow["MaPhieu"].ToString() == searchMaPhieu &&
                                ctRow["MaVTID"].ToString() == searchMaVTID &&
                                ctRow["KhoVaiID"].ToString() == searchKhoVaiID &&
                                ctRow["MaNhom"].ToString() == searchMaNhom &&
                                ctRow["MaCode"].ToString() == searchMaCode &&
                                maDot == currentDotEdit) // SO SÁNH DotEdit với MaDot
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

                    // Fill cột phòng+màu (số lượng) - LỌC THEO DotEdit = MaDot
                    foreach (DataRow mainRow in group.Value)
                    {
                        string mauVTID = mainRow["MauVTID"].ToString();

                        foreach (var phong in phongList)
                        {
                            for (int i = 0; i < mauMaList.Count; i++)
                            {
                                string colName = string.Format("{0}@{1}@{2}", mauTenList[i], phong, mauMaList[i]);

                                // Tìm trong dtChiTiet với điều kiện MaDot == currentDotEdit
                                if (dtChiTiet != null)
                                {
                                    decimal totalSoLuong = 0;

                                    foreach (DataRow ctRow in dtChiTiet.Rows)
                                    {
                                        string maDot = ctRow.Table.Columns.Contains("MaDot") ? ctRow["MaDot"]?.ToString() ?? "" : "";

                                        if (ctRow["MaPhieu"].ToString() == maPhieu &&
                                            ctRow["MaVTID"].ToString() == maVTID &&
                                            ctRow["MauVTID"].ToString() == mauVTID &&
                                            ctRow["KhoVaiID"].ToString() == khoVaiID &&
                                            ctRow["MaNhom"].ToString() == maNhom &&
                                            ctRow["MaCode"].ToString() == maCode &&
                                            ctRow["MaMau"].ToString() == mauMaList[i] &&
                                            ctRow["MaPhong"].ToString() == phong &&
                                            maDot == currentDotEdit) // SO SÁNH DotEdit với MaDot
                                        {
                                            decimal soLuong = ctRow["SoLuong"] == DBNull.Value ? 0 : Convert.ToDecimal(ctRow["SoLuong"]);
                                            totalSoLuong += soLuong;
                                        }
                                    }

                                    if (totalSoLuong != 0)
                                    {
                                        newRow[colName] = Math.Round(totalSoLuong, 2).ToString("0.##");
                                    }
                                }
                            }
                        }
                    }

                    result.Rows.Add(newRow);
                }

                gridControl1.DataSource = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Lỗi: {0}", ex.Message), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện RowCellStyle để tô màu VÀNG
        private void bandedGridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
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
                string dotEdit = row.Table.Columns.Contains("DotEdit") ? row["DotEdit"]?.ToString() ?? "" : "";

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
                    string key = $"{maPhieu}_{maVTID}_{mauVTID}_{khoVaiID}_{maNhom}_{maCode}_{maMau}_{maPhong}_{dotEdit}";

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

                    // Luôn tạo column mới
                    BandedGridColumn column = new BandedGridColumn
                    {
                        Caption = caption,
                        FieldName = fieldName,
                        Name = columnName,
                        Visible = true,
                        MinWidth = 60,
                        Width = 60,
                        MaxWidth = 60
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
                        Width = 60  // Đặt cố định
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

                //btnEdit.ButtonClick += (s, e) =>
                //{
                //    DataRow dr = bandedGridView1.GetFocusedDataRow();
                //    if (dr == null) return;
                //    frmPhanTichBOM_ChonMau frm = new frmPhanTichBOM_ChonMau(dr);
                //    if (frm.ShowDialog() == DialogResult.OK)
                //    {
                //        dr["MauVTIDChung"] = frm._mauvtidchung;
                //        LoadTVMauVT();
                //        DataTable tbl = frm.tblGrid;
                //        if (tbl == null || tbl.Rows.Count == 0) return;
                //        foreach (DataRow r in tbl.Rows)
                //        {
                //            string columnName = r["MauSPID"].ToString();
                //            string mauVTID = r["MauVTID"].ToString();
                //            foreach (DataColumn dc in dr.Table.Columns)
                //            {
                //                if (dc.ColumnName == columnName)
                //                {
                //                    dr[columnName] = mauVTID;
                //                }
                //            }

                //        }

                //    }
                //    this.ActiveControl = button1;
                //};

                //col.ColumnEdit = btnEdit;

                return col;
            }
            catch (Exception ex)
            {

                return col;
            }
        }
        private void LoadTVMauVT()
        {
            string url = $"{URL}XinNPLMayMau/Get?Action=GETMAUVTTV";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblMauVT = JsonConvert.DeserializeObject<DataTable>(json);
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
                    if (decimalPart.Length > 2)
                    {
                        e.Handled = true;
                        return;
                    }
                }
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
                DataTable tblChungLoaiChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
                repoSearchCLCT.DataSource = tblChungLoaiChiTiet;


            }
            catch (Exception ex)
            {
            }
        }
        private void LockAllColumns()
        {
            foreach (BandedGridColumn col in bandedGridView1.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
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
    }
}