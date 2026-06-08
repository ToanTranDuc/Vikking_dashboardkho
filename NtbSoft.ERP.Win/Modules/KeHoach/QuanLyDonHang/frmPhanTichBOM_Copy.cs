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

            DataTable tblSizeChild = _tblSizeCopy.AsEnumerable()
                .Where(r => keySet.Contains($"{r.Field<string>("MaNhomSize")}|{r.Field<string>("MaSize")}"))
                .CopyToDataTable();

            string url = string.Format("{0}?makh={1}&&mahang={2}&&madot={3}", URL + "KhoiTaoDM/GetKTBom", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString(),
                searchLookUpEditMH.EditValue == null ? "" : searchLookUpEditMH.EditValue.ToString(), searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
                return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
                return;

            var mauColumns = tbl.Columns
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

            foreach (DataRow dr in tbl.Rows)
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
                string maSizeChung = dr["MaSizeChung"].ToString();
                string size = dr["Size"].ToString();
                string filteredMaSizeChung = FilterMaSizeChung(maSizeChung, tblSizeChild);
                string filteredSize = FilterSize(size, tblSizeChild);
                _nr["MaSizeChung"] = filteredMaSizeChung;
                _nr["Size"] = filteredSize;

                //tới khúc màu
                _nr["MauVTIDChung"] = dr["MauVTIDChung"];
                if (IsOne && IsOne == IsOnetbl)
                {
                    string sourceMauColumn = mauColumns[0];
                    string targetMauColumn = mauColumnsGrid[0];
                    _nr[targetMauColumn] = dr[sourceMauColumn];
                }
                _tblGrid.Rows.Add(_nr);
            }    


            ResultTable = _tblGrid;
            this.DialogResult = DialogResult.OK;
            this.Close();


            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            this.Close();
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
