using DevExpress.XtraEditors;
using Newtonsoft.Json;
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
    public partial class frmPhanTichBOM_Copy : DevExpress.XtraEditors.XtraForm
    {
        public DataTable ResultTable { get; private set; }
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        string _maKH_root = "", _maHang_root = "", _maKH_copy = "", _maHang_copy = "", _maDot_copy = "", _dot_copy = "";
        DataTable _tblGrid = new DataTable();
        DataTable _tblSizeRoot = new DataTable(), _tblSizeCopy = new DataTable();
        DataTable tblAllSize = new DataTable();
        public frmPhanTichBOM_Copy(string maKH, string maHang, string maDot, DataTable tbl)
        {
            InitializeComponent();
            _maKH_root = maKH;
            _maHang_root = maHang;
            _maDot_copy = maDot;
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            CreateSearchLookupKhachHang();

            _tblSizeRoot = loadSizeRoot(_maKH_root, _maHang_root);
            _tblGrid = tbl;
          

        }

        private DataTable loadSizeRoot(string makh, string mahang)
        {
            string url = $"{URL}KHOITAOBOMV1/Get?Action=GETSIZECOPY&para1={makh}&para2={mahang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
                return null;

            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
                return null;

            return tbl;
        }
        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (_maKH_root == "" || _maHang_root == "" || _maKH_copy == "" || _maHang_copy == "" || _maDot_copy == "" || _dot_copy == "")
            {
                return;
            }
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            //string urlKT = $"{URL}KhoiTaoBOMV1/Get?action=GETCHECKSIZEBOM&para1={_maKH_root.ToString()}&para2={_maHang_root.ToString()}&para3={_maKH_copy.ToString()}&para4={_maHang_copy}&para5={_maDot_copy}&para6={_dot_copy}";
            //string msResult = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKT); }).Result;
            //DataTable tblKT = JsonConvert.DeserializeObject<DataTable>(msResult);

            //if (tblKT != null && tblKT.Rows.Count > 0)
            //{

            //    string text = GroupSizesByNhomSize(tblKT);
            //    XtraMessageBox.Show(this, $"Các size sau không có trong khách hàng {searchLookUpEditKH.Text}, mã hàng {searchLookUpEditMH.Text}:\n" + text, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            //    return;
            //}
            //string urlKT2 = $"{URL}KhoiTaoBOMV1/Get?action=GETCHECKMAUBOM&para1={_maKH_root.ToString()}&para2={_maHang_root.ToString()}&para3={_maKH_copy.ToString()}&para4={_maHang_copy}&para5={_maDot_copy}&para6={_dot_copy}";
            //string msResult2 = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKT2); }).Result;
            //DataTable tblKT2 = JsonConvert.DeserializeObject<DataTable>(msResult2);

            //if (tblKT2 != null && tblKT2.Rows.Count > 0)
            //{

            //    string text = GetColorDataString(tblKT2);
            //    XtraMessageBox.Show(this, $"Các màu sau không có trong khách hàng {searchLookUpEditKH.Text}, mã hàng {searchLookUpEditMH.Text}:\n" + text, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            //    return;
            //}


            _tblSizeCopy = loadSizeRoot(searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString());


            var keySet = new HashSet<string>(
         _tblSizeRoot.AsEnumerable()
             .Select(r => $"{r.Field<string>("MaNhomSize")}|{r.Field<string>("MaSize")}"));

            var filteredRows = _tblSizeCopy.AsEnumerable()
                .Where(r => keySet.Contains($"{r.Field<string>("MaNhomSize")}|{r.Field<string>("MaSize")}"));

            DataTable tblSizeChild;
            if (filteredRows.Any())
            {
                tblSizeChild = filteredRows.CopyToDataTable();
            }
            else
            {
                // Create empty DataTable with same structure
                tblSizeChild = _tblSizeCopy.Clone();
            }

            //string url = string.Format("{0}?makh={1}&&mahang={2}&&madot={3}", URL + "KhoiTaoDM/GetKTBom", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(),
            //    searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString());
            //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //if (json == "[]")
            //    return;
            //DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            //if (tbl == null || tbl.Rows.Count == 0)
            //    return;
            string maKH = searchLookUpEditKH.EditValue?.ToString() ?? "";
            string maHang = searchLookUpEditMH.EditValue?.ToString() ?? "";
            string maDot = searchLookUpEditDot.EditValue?.ToString() ?? "";
            string urlMain = $"{URL}KhoiTaoBOMV1/Get?action=GETVTSP_V1&para1={maKH.ToString()}&para2={maHang.ToString()}&para3={maDot.ToString()}";

            string jsonMain = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMain); }).Result;
            DataTable dtMain = JsonConvert.DeserializeObject<DataTable>(jsonMain);

            if (dtMain.Rows.Count == 0)
            {
              
              
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
            dtResult.Columns.Add("IsAllSize", typeof(string));
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
     MaDot = r["MaDot"]?.ToString() ?? "",
     IsAllSize = r["IsAllSize"]?.ToString() ?? ""
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
                resultRow["IsAllSize"] = group.Key.IsAllSize;
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



            var mauColumns = dtResult.Columns
                .Cast<DataColumn>()
                .Where(col => col.ColumnName.IndexOf("@Mau@", StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(col => col.ColumnName)
                .ToList();

            bool IsOnetbl = mauColumns.Count == 1;

            var mauColumnsGrid = _tblGrid.Columns
                .Cast<DataColumn>()
                .Where(col => col.ColumnName.IndexOf("@Mau@", StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(col => col.ColumnName)
                .ToList();

            bool IsOne = mauColumnsGrid.Count == 1;
            bool isSameCount = mauColumns.Count == mauColumnsGrid.Count;
            foreach (DataRow dr in dtResult.Rows)
            {
                DataRow _nr = _tblGrid.NewRow();

                //_nr["MaKH"] = dr["MaKH"];
                //_nr["MaHang"] = dr["MaHang"];
                _nr["MaNhom"] = dr["MaNhom"];
                _nr["TenNhom"] = dr["TenNhom"];
                _nr["NPL"] = dr["NPL"];
                _nr["Sort"] = dr["Sort"];
                _nr["MaVTID"] = dr["MaVTID"];
                _nr["MaVT"] = dr["MaVT"];
                _nr["ChiTiet"] = dr["ChiTiet"];
                _nr["MaDVVT"] = dr["MaDVVT"];
                _nr["TenDVVT"] = dr["TenDVVT"];
                _nr["KhoVaiID"] = dr["KhoVaiID"];
                _nr["KhoVai"] = dr["KhoVai"];
                _nr["GhiChu"] = dr["GhiChu"];
                _nr["TachMau"] = dr["TachMau"];
                _nr["DinhMucHaoHut"] = dr["DinhMucHaoHut"];
                _nr["DinhMucChung"] = dr["DinhMucChung"];
                //_nr["IsXetDuyet"] = "";
                _nr["IsNew"] = dr["IsNew"];
                _nr["MaCode"] = dr["MaCode"];
                _nr["STTCode"] = dr["STTCode"];
                //_nr["IsActive"] = 0;
                _nr["MaNhomChiTiet"] = dr["MaNhomChiTiet"];
                _nr["STT"] = dr["STT"];
                //size
                string maSizeChung = string.Empty;
                string size = string.Empty;
                if (dr["IsAllSize"].ToString().ToLower()=="true")
                {
                    maSizeChung ="";
                    size ="";
                }
                else
                {
                    maSizeChung = dr["MaSizeChung"].ToString();
                    size = dr["Size"].ToString();
                }
               
                string filteredMaSizeChung = FilterMaSizeChung(maSizeChung, tblSizeChild);
                string filteredSize = FilterSize(size, tblSizeChild);
                _nr["MaSizeChung"] = filteredMaSizeChung;
                _nr["Size"] = filteredSize;

                //tới khúc màu
                _nr["MauVTIDChung"] = dr["MauVTIDChung"];
                //if (IsOne && IsOne == IsOnetbl)
                //{
                foreach(DataColumn dc in _nr.Table.Columns)
                {
                    if(dc.ColumnName.Contains("@Mau@"))
                    {
                        //if (mauColumns == mauColumnsGrid)
                        //{
                        //    int idx = mauColumnsGrid.IndexOf(dc.ColumnName);
                        //    if (idx >= 0 && idx < mauColumns.Count)
                        //    {
                        //        _nr[dc.ColumnName] = dr[mauColumns[idx]];
                        //    }
                        //}
                        //else
                        //{
                        //    if (mauColumns.Count > 0)
                        //    {
                        //        _nr[dc.ColumnName] = dr[mauColumns[0]];
                        //    }
                        //}
                        if (mauColumns.SequenceEqual(mauColumnsGrid))
                        {
                            // Source và Grid có cùng tập cột → map 1-1 theo tên
                            foreach (var colName in mauColumnsGrid)
                            {
                                if (mauColumns.Contains(colName))
                                    _nr[colName] = dr[colName];
                            }
                        }
                        else
                        {
                            // Số cột khác nhau → dồn tất cả vào cột đầu tiên của grid
                            if (mauColumns.Count > 0 && mauColumnsGrid.Count > 0)
                                _nr[mauColumnsGrid[0]] = dr[mauColumns[0]];
                        }
                    }    
                 
                }    
                    string sourceMauColumn = mauColumns[0];
                    string targetMauColumn = mauColumnsGrid[0];
                    _nr[targetMauColumn] = dr[sourceMauColumn];
                //}
                _tblGrid.Rows.Add(_nr);
            }
            if (_tblGrid.Columns.Contains("IsAllSize"))
                _tblGrid.Columns.Remove("IsAllSize");

            ResultTable = _tblGrid;
            this.DialogResult = DialogResult.OK;
            this.Close();
            

            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            this.Close();
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
        private string FilterSize(string size, DataTable tblGrid)
        {
            if (string.IsNullOrEmpty(size))
                return string.Empty;

            List<string> validGroups = new List<string>();

            string[] groups = size.Split(';');

            foreach (string group in groups)
            {
                if (string.IsNullOrEmpty(group.Trim()))
                    continue;

                string[] parts = group.Split(':');
                if (parts.Length != 2)
                    continue;

                string nhomSize = parts[0].Trim();
                string[] tenSizes = parts[1].Split(',');

                // Check NhomSize có trong tblGrid không
                bool nhomSizeExists = tblGrid.AsEnumerable()
                    .Any(row => row["NhomSize"].ToString().Trim() == nhomSize);

                if (!nhomSizeExists)
                    continue;

                // Filter các TenSize có trong tblGrid
                List<string> validSizes = new List<string>();
                foreach (string tenSize in tenSizes)
                {
                    string cleanSize = tenSize.Trim();
                    bool sizeExists = tblGrid.AsEnumerable()
                        .Any(row => row["TenSize"].ToString().Trim() == cleanSize);

                    if (sizeExists)
                        validSizes.Add(cleanSize);
                }

                if (validSizes.Count > 0)
                {
                    validGroups.Add($"{nhomSize}:{string.Join(",", validSizes)}");
                }
            }

            return string.Join(";", validGroups);
        }
        private string FilterMaSizeChung(string maSizeChung, DataTable tblGrid)
        {
            if (string.IsNullOrEmpty(maSizeChung))
                return string.Empty;

            List<string> validGroups = new List<string>();

            // Split theo dấu ; để lấy từng nhóm
            string[] groups = maSizeChung.Split(';');

            foreach (string group in groups)
            {
                if (string.IsNullOrEmpty(group.Trim()))
                    continue;

                // Split theo dấu : để tách MaNhomSize và danh sách MaSize
                string[] parts = group.Split(':');
                if (parts.Length != 2)
                    continue;

                string maNhomSize = parts[0].Trim();
                string[] maSizes = parts[1].Split(',');

                // Check xem MaNhomSize có trong tblGrid không
                bool nhomSizeExists = tblGrid.AsEnumerable()
                    .Any(row => row["MaNhomSize"].ToString().Trim() == maNhomSize);

                if (!nhomSizeExists)
                    continue;

                // Filter các MaSize có trong tblGrid
                List<string> validSizes = new List<string>();
                foreach (string maSize in maSizes)
                {
                    string cleanSize = maSize.Trim();
                    bool sizeExists = tblGrid.AsEnumerable()
                        .Any(row => row["MaSize"].ToString().Trim() == cleanSize);

                    if (sizeExists)
                        validSizes.Add(cleanSize);
                }

                // Nếu nhóm này còn size hợp lệ thì thêm vào
                if (validSizes.Count > 0)
                {
                    validGroups.Add($"{maNhomSize}:{string.Join(",", validSizes)}");
                }
            }

            return string.Join(";", validGroups);
        }
        public string GetColorDataString(DataTable dt)
        {
            var result = new System.Text.StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                string codeMau = row["CodeMau"].ToString();
                string tenMau = row["TenMau"].ToString();

                if (!string.IsNullOrEmpty(codeMau))
                {
                    result.AppendLine($"Code màu: {codeMau} - Tên màu: {tenMau}");
                }
            }
            return result.ToString().Trim();
        }
        public static string GroupSizesByNhomSize(DataTable dt)
        {
            // Nhóm theo NhomSize và lấy các TenSize duy nhất
            var grouped = dt.AsEnumerable()
                .Where(row => row["NhomSize"] != DBNull.Value && row["TenSize"] != DBNull.Value)
                .GroupBy(row => "Inseam " + row["NhomSize"].ToString())
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    NhomSize = g.Key,
                    TenSizes = g.Select(row => row["TenSize"].ToString())
                               .Distinct()

                               .ToList()
                });

            // Format: NhomSize1:TenSize1,TenSize2;NhomSize2:TenSize3,...
            return string.Join("\n", grouped.Select(g => $"{g.NhomSize}: {string.Join(",", g.TenSizes)}"));
        }
        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CreateSearchLookupKhachHang()
        {
            try
            {
                searchLookUpEditKH.Properties.ValueMember = "MaKH";
                searchLookUpEditKH.Properties.DisplayMember = "TenKH";

                string urlKH = string.Format("{0}?", URL + "PhanTichBom/GetKH");
                string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
                DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
                searchLookUpEditKH.Properties.DataSource = tblKH;
                if (tblKH != null && tblKH.Rows.Count > 0) searchLookUpEditKH.EditValue = tblKH.Rows[0]["MaKH"];
            }
            catch (Exception ex)
            {

            }
        }

        private void CreateSearchLookUpDot()
        {
            string maKH = searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString();
            string maHang = searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString();
            searchLookUpEditDot.Properties.DisplayMember = "Dot";
            searchLookUpEditDot.Properties.ValueMember = "MaDot";

            //string url = string.Format("{0}?makh={1}&&mahang={2}", URL + "KhoiTaoDM/GetThongSoDot", maKH, maHang);
            string url = $"{URL}KHOITAOBOMV1/Get?Action=GETDOTCOPY&para1={maKH}&para2={maHang}";

            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                searchLookUpEditDot.Properties.DataSource = null;
                return;
            }

            DataTable tblDot = JsonConvert.DeserializeObject<DataTable>(json);
            if (tblDot == null || tblDot.Rows.Count == 0)
            {
                searchLookUpEditDot.Properties.DataSource = null;
                return;
            }
            searchLookUpEditDot.Properties.DataSource = tblDot;
            searchLookUpEditDot.EditValue = tblDot.Rows[0]["MaDot"].ToString();
        }

        private void CreateSearchLookUpMaHang()
        {
            try
            {
                searchLookUpEditMH.Properties.ValueMember = "MaHang";
                searchLookUpEditMH.Properties.DisplayMember = "TenHang";

                string urlMH = string.Format("{0}?makh={1}", URL + "PhanTichBom/GetMH", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
                string jsonMH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMH); }).Result;
                DataTable tblMH = JsonConvert.DeserializeObject<DataTable>(jsonMH);
                searchLookUpEditMH.Properties.DataSource = tblMH;
                if (tblMH != null && tblMH.Rows.Count > 0) searchLookUpEditMH.EditValue = tblMH.Rows[0]["MaHang"];
            }
            catch (Exception ex)
            {

            }
        }
        private void searchLookUpEditDot_EditValueChanged(object sender, EventArgs e)
        {
            _maDot_copy = searchLookUpEditDot.EditValue.ToString();
            _dot_copy = searchLookUpEditDot.Text.ToString();
        }
        private void searchLookUpEditKH_EditValueChanged(object sender, EventArgs e)
        {
            _maKH_copy = searchLookUpEditKH.EditValue.ToString();
            CreateSearchLookUpMaHang();
            CreateSearchLookUpDot();
        }
        private void searchLookUpEditMH_EditValueChanged(object sender, EventArgs e)
        {
            _maHang_copy = searchLookUpEditMH.EditValue.ToString();
            CreateSearchLookUpDot();
        }


      
    }
}
