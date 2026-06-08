using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.ThuVien;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LicenseContext = OfficeOpenXml.LicenseContext;
namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmImportEXBOM : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _maHang = string.Empty, _maKH = string.Empty, _tenhang = string.Empty, _tenkhachhang = string.Empty;
        private HttpClientExtension _clientExtension;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _checkUserName = false;
        bool _allowDuyet = false, _allowHuyDuyet = false, _allowActive = false;
        private string selectedFilePath = "";
        DataTable _tblMauSP;
        DataTable tblMauVT = new DataTable();
        DataTable tblCheck = new DataTable();
        DataTable tblHangHoa = new DataTable();
        DataTable tblMauSP = new DataTable();
        DataTable tblCLCT = new DataTable();
        DataTable tblSizeSP = new DataTable();
        DataTable tblBOM = new DataTable();
        DataTable tblSizeChung = new DataTable();
        public frmImportEXBOM()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _tblMauSP = new DataTable();
            tblCheck = new DataTable();
            tblMauSP = new DataTable();
            tblCLCT = new DataTable();
            tblSizeSP = new DataTable();
            tblBOM = new DataTable();
            CreateRepoSearchLookUpChungLoaiCT();
            LoadTVMauVT();
            LoadCheckData();
            LoadChecHang();
            LoadCLCT();
            loadSizeChung();
            ImportEX();
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
            return dt;
        }




        private void CreateRepoSearchLookUpChungLoaiCT()
        {
            try
            {
                repoSearchCLCT.DisplayMember = "TenNhomChiTiet";
                repoSearchCLCT.ValueMember = "MaNhomChiTiet";
                string url = $"{URL}KhoiTaoBOMV1/Get?action=GETCHUNGLOAICHITIET";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                repoSearchCLCT.DataSource = tbl;

            }
            catch (Exception ex)
            {
            }
        }

        public List<string> OpenExcelAndShowSheets()
        {
            var sheetNames = new List<string>();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls;*.xlsm|All Files|*.*";
                openFileDialog.Title = "Chọn file Excel";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName; // LƯU ĐƯỜNG DẪN FILE

                    try
                    {
                        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                        using (var package = new ExcelPackage(new FileInfo(selectedFilePath)))
                        {
                            foreach (var worksheet in package.Workbook.Worksheets)
                            {
                                sheetNames.Add(worksheet.Name);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show($"Lỗi khi đọc file Excel:\n{ex.Message}",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            return sheetNames;
        }
        private void ReadExcelToDataTable(string filePath, string sheetName)
        {
            DataTable dt = new DataTable();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[sheetName];
                if (worksheet == null)
                {
                    XtraMessageBox.Show($"Không tìm thấy sheet '{sheetName}'", "Lỗi");
                    return;
                }

                int row = 2; // Bắt đầu từ row 2
                var tenkhachhang = worksheet.Cells["c3"].Value?.ToString()?.Trim() ?? "";
                var tenmahang = worksheet.Cells["c4"].Value?.ToString()?.Trim() ?? "";

                // Kiểm tra khách hàng
                if (!CheckHang(tenkhachhang, out _maKH, true))
                {
                    XtraMessageBox.Show($"Không tìm thấy khách hàng '{tenkhachhang}' trong hệ thống!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }

                // Kiểm tra mã hàng
                if (!CheckHang(tenmahang, out _maHang, false))
                {
                    XtraMessageBox.Show($"Không tìm thấy mã hàng '{tenmahang}' trong hệ thống!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.No;
                    this.Close();
                    return;
                }
                _tenhang = tenmahang.ToString();
                _tenkhachhang = tenkhachhang.ToString();
                List<string> danhSachTenMauSP = new List<string>();
                int startRow = 7;
                int startCol = 12; // Cột L = 12 (A=1, B=2,..., L=12)
                int colIndex = startCol;
                while (true)
                {
                    var cellValue = worksheet.Cells[startRow, colIndex].Value?.ToString()?.Trim();

                    if (string.IsNullOrEmpty(cellValue))
                    {
                        break; // Dừng khi gặp ô trống
                    }

                    danhSachTenMauSP.Add(cellValue);
                    colIndex++; // Sang cột tiếp theo
                }
                var mauTrung = danhSachTenMauSP.GroupBy(x => x)
                                .Where(g => g.Count() > 1)
                                .Select(g => g.Key)
                                .ToList();

                if (mauTrung.Any())
                {
                    XtraMessageBox.Show($"Có màu sản phẩm bị trùng: {string.Join(", ", mauTrung)}\n\nVui lòng nhập lại!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.No;

                    return;
                }
                LoadMauSP();
                LoadSize();
                if (!KiemTraDuLieuExcel(worksheet, tblCheck))
                {
                    this.DialogResult = DialogResult.No;
                    this.Close();
                    return;
                }
                gridControl1.DataSource = tblBOM;
            }

        }
        private bool KiemTraDuLieuExcel(ExcelWorksheet worksheet, DataTable tblCheck)
        {
            tblBOM = createTableSaveEdit();
            bool checkAddColumn = false;

            // Kiểm tra các cột có tồn tại không
            var requiredColumns = new Dictionary<string, int>
                    {
                        { "CHỦNG LOẠI", 2 },
                        { "ITEMCODE", 4 },
                        { "VẬT TƯ", 5 },
                        { "SIZE/KHÔ", 9 },
                        { "Đơn vị của size khô", 10 },
                        { "ĐVT", 11 },
                    };

            // Kiểm tra header tồn tại (row 8)
            foreach (var col in requiredColumns)
            {
                var headerValue = worksheet.Cells[8, col.Value].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(headerValue))
                {
                    XtraMessageBox.Show(
                        $"Không tìm thấy cột '{col.Key}' ở vị trí mong đợi (cột {GetColumnName(col.Value)})",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return false;
                }
            }

            // Bắt đầu kiểm tra dữ liệu từ row 9
            int startRow = 9;
            int rowIndex = startRow;

            while (true)
            {
                // Kiểm tra nếu STT rỗng thì dừng
                var stt = worksheet.Cells[rowIndex, 1].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(stt))
                    break;

                // Lấy dữ liệu từ Excel
                string chungLoai = worksheet.Cells[rowIndex, 2].Value?.ToString()?.Trim() ?? "";
                string tenNhomChiTiet = worksheet.Cells[rowIndex, 3].Value?.ToString()?.Trim() ?? "";
                string itemCode = worksheet.Cells[rowIndex, 4].Value?.ToString()?.Trim() ?? "";
                string vatTu = worksheet.Cells[rowIndex, 5].Value?.ToString()?.Trim() ?? "";
                string sizeKho = worksheet.Cells[rowIndex, 6].Value?.ToString()?.Trim() ?? "";
                string donViSizeKho = worksheet.Cells[rowIndex, 7].Value?.ToString()?.Trim() ?? "";
                string dvt = worksheet.Cells[rowIndex, 8].Value?.ToString()?.Trim() ?? "";
                string sizechung = worksheet.Cells[rowIndex, 11].Value?.ToString()?.Trim() ?? "";
                string dinhmucchung = worksheet.Cells[rowIndex, 9].Value?.ToString()?.Trim() ?? "";
                string dinhmuchaohut = worksheet.Cells[rowIndex, 10].Value?.ToString()?.Trim() ?? "";

                if (new[] { chungLoai, itemCode, vatTu, sizeKho, donViSizeKho }.All(string.IsNullOrEmpty))
                {
                    break;
                }

                // Parse Size
                string[] parsInSeam = sizechung.Split(';');
                string MaSizeChung = "";

                foreach (var item in parsInSeam)
                {
                    string nhomSize = item.Split(':')[0].ToString().Trim().ToUpper();
                    string isPars = item.Split(':')[1];
                    string[] sizePars = isPars.Split(',');

                    DataRow drIS = tblSizeSP.AsEnumerable()
                        .FirstOrDefault(x => x["NhomSize"].ToString().Trim().ToUpper() == nhomSize);
                    if (drIS == null)
                    {
                        XtraMessageBox.Show(
                           $"Row {rowIndex}: Không tìm thấy InSeam '{item.Split(':')[0]}' trong thẻ size!",
                           "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    string masizeJoin = "";
                    foreach (var itemsize in sizePars)
                    {
                        string sizeName = itemsize.Trim().ToUpper();
                        DataRow drSize = tblSizeSP.AsEnumerable()
                            .FirstOrDefault(x =>
                                x["NhomSize"].ToString().Trim().ToUpper() == nhomSize &&
                                x["TenSize"].ToString().Trim().ToUpper() == sizeName
                            );

                        if (drSize == null)
                        {
                            XtraMessageBox.Show(
                               $"Row {rowIndex}: Không tìm thấy size '{itemsize}' của inseam {item.Split(':')[0]} trong thẻ size!",
                               "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        masizeJoin = string.IsNullOrEmpty(masizeJoin)
                            ? drSize["MaSize"].ToString()
                            : $"{masizeJoin},{drSize["MaSize"]}";
                    }

                    MaSizeChung = string.IsNullOrEmpty(MaSizeChung)
                        ? $"{drIS["MaNhomSize"]}:{masizeJoin}"
                        : $"{MaSizeChung};{drIS["MaNhomSize"]}:{masizeJoin}";
                }

                if (!decimal.TryParse(dinhmucchung, out _))
                {
                    MessageBox.Show($"Giá trị '{dinhmucchung}' tại dòng {rowIndex} - cột 9 không phải kiểu số hợp lệ (decimal)!");
                    return false;
                }

                if (!decimal.TryParse(dinhmuchaohut, out _))
                {
                    MessageBox.Show($"Giá trị '{dinhmuchaohut}' tại dòng {rowIndex} - cột 10 không phải kiểu số hợp lệ (decimal)!");
                    return false;
                }

                // Kiểm tra Chủng loại
                var matchedByChungLoai = tblCheck.AsEnumerable()
                    .Where(x => x["TenNhom"].ToString().Trim().Equals(chungLoai, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!matchedByChungLoai.Any())
                {
                    XtraMessageBox.Show(
                        $"Row {rowIndex}: Không tìm thấy Chủng loại '{chungLoai}' trong thư viện vật tư!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                DataRow checkChungLoai = tblCLCT.AsEnumerable()
                     .FirstOrDefault(x =>
                         x["MaCLVT"].ToString().Trim().ToUpper() == matchedByChungLoai.First()["MaNhom"].ToString().Trim().ToUpper() &&
                         x["TenNhom"].ToString().Trim().ToUpper() == tenNhomChiTiet.ToString().Trim().ToUpper()
                     );

                if (checkChungLoai == null)
                {
                    XtraMessageBox.Show(
                       $"Row {rowIndex}: Không tìm thấy Chủng loại chi tiết '{tenNhomChiTiet}' trong thư viện {matchedByChungLoai.First()["TenNhom"]}!",
                       "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Kiểm tra ItemCode
                var matchedByItemCode = matchedByChungLoai
                    .Where(x => x["MaVT"].ToString().Trim().Equals(itemCode, StringComparison.OrdinalIgnoreCase)
                     && x["ChiTiet"].ToString().Trim().Equals(vatTu, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!matchedByItemCode.Any())
                {
                    XtraMessageBox.Show(
                        $"Row {rowIndex}: Không tìm thấy ItemCode '{itemCode}' trong thư viện vật tư!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                var row = matchedByItemCode.Where(x => x["KhoVai"].ToString().Trim().Equals(sizeKho, StringComparison.OrdinalIgnoreCase)
                        && x["DVKhoVai"].ToString().Trim().Equals(donViSizeKho, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!row.Any())
                {
                    XtraMessageBox.Show(
                        $"Row {rowIndex}: Không tìm thấy Size/Khổ '{sizeKho}' trong thư viện vật tư",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Kiểm tra ĐVT
                var rowDVTinh = row.Where(x => x["TenDVVT"].ToString().Trim().Equals(dvt, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                if (!rowDVTinh.Any())
                {
                    XtraMessageBox.Show(
                        $"Row {rowIndex}: Không tìm thấy đơn vị tính '{dvt}' trong thư viện vật tư!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // ✅ TẠO DATAROW Ở ĐÂY - TRƯỚC KHI VÀO VÒNG LẶP MÀU
                DataRow dr = tblBOM.NewRow();

                // Gán các giá trị chung
                dr["MaNhom"] = rowDVTinh.First()["MaNhom"].ToString();
                dr["TenNhom"] = rowDVTinh.First()["TenNhom"].ToString();
                dr["NPL"] = rowDVTinh.First()["NPL"].ToString();
                dr["Sort"] = rowDVTinh.First()["Sort"].ToString();
                dr["MaVTID"] = rowDVTinh.First()["MaVTID"].ToString();
                dr["MaVT"] = rowDVTinh.First()["MaVT"].ToString();
                dr["ChiTiet"] = rowDVTinh.First()["ChiTiet"].ToString();
                dr["MaDVVT"] = rowDVTinh.First()["MaDVVT"].ToString();
                dr["TenDVVT"] = rowDVTinh.First()["TenDVVT"].ToString();
                dr["KhoVaiID"] = rowDVTinh.First()["KhoVaiID"].ToString();
                dr["KhoVai"] = rowDVTinh.First()["KhoVai"].ToString();
                dr["DinhMucChung"] = dinhmucchung;
                dr["DinhMucHaoHut"] = dinhmuchaohut;
                dr["MaCode"] = $"{rowDVTinh.First()["MaVTID"].ToString()}|{rowIndex - 8}";
                dr["MaNhomChiTiet"] = checkChungLoai["MaNhom"];
                dr["Size"] = sizechung;
                dr["MaSizeChung"] = MaSizeChung;

                // Thêm cột màu sản phẩm nếu chưa có
                if (!checkAddColumn)
                {
                    foreach (DataRow item in tblMauSP.Rows)
                    {
                        var value = $"{item["TenMau"]}@Mau@{item["MaMau"]}";
                        if (!tblBOM.Columns.Contains(value))
                        {
                            tblBOM.Columns.Add(value, typeof(string));
                        }
                    }
                    checkAddColumn = true;
                }

                // Kiểm tra màu sản phẩm từ cột L trở đi
                int mauColIndex = 12; // Bắt đầu từ cột L (12)
                List<string> danhSachMauVTID = new List<string>();

                while (true)
                {
                    var mauValue = worksheet.Cells[rowIndex, mauColIndex].Value?.ToString()?.Trim();

                    if (string.IsNullOrEmpty(mauValue))
                        break; // Dừng khi gặp ô trống

                    // Kiểm tra màu có trong rowDVTinh không
                    var mauExists = rowDVTinh.Where(x => x["MaMauVT"].ToString().Trim().Equals(mauValue, StringComparison.OrdinalIgnoreCase)).ToList();

                    if (!mauExists.Any())
                    {
                        XtraMessageBox.Show(
                            $"Row {rowIndex}, Cột {GetColumnName(mauColIndex)}: Màu '{mauValue}' không tồn tại trong hệ thống!",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    // ✅ SỬA: Lấy đúng MauVTID
                    danhSachMauVTID.Add(mauExists.First()["MauVTID"].ToString().Trim());

                    // ✅ SỬA: Lấy tên màu SP từ row 7
                    string tenmauSP = worksheet.Cells[7, mauColIndex].Value?.ToString()?.Trim() ?? "";

                    // Gán giá trị màu vào cột tương ứng
                    foreach (DataColumn col in tblBOM.Columns)
                    {
                        if (col.ColumnName.Contains("@MAU@"))
                        {
                            string tenMauCol = col.ColumnName.Split(new[] { "@MAU@" }, StringSplitOptions.None)[0].Trim();

                            if (tenMauCol.Equals(tenmauSP, StringComparison.OrdinalIgnoreCase))
                            {
                                dr[col.ColumnName] = mauExists.First()["MauVTID"];
                            }
                        }
                    }

                    mauColIndex++; 
                }

                dr["MauVTIDChung"] = string.Join(",", danhSachMauVTID);

                tblBOM.Rows.Add(dr);

                rowIndex++;
            }

            return true;
        }

        // Helper method để lấy tên cột (A, B, C,...)
        private string GetColumnName(int columnNumber)
        {
            string columnName = "";
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }
        private bool CheckHang(string giaTri, out string maCode, bool isKhachHang)
        {
            maCode = string.Empty;

            try
            {
                DataRow row = null;

                if (isKhachHang)
                {
                    // Kiểm tra khách hàng
                    row = tblHangHoa.AsEnumerable()
                        .FirstOrDefault(x => x["TenKH"].ToString().Trim().Equals(giaTri, StringComparison.OrdinalIgnoreCase));

                    if (row != null)
                    {
                        maCode = row["MaKH"].ToString();
                        return true;
                    }
                }
                else
                {
                    // Kiểm tra mã hàng
                    row = tblHangHoa.AsEnumerable()
                        .FirstOrDefault(x => x["TenHang"].ToString().Trim().Equals(giaTri, StringComparison.OrdinalIgnoreCase));

                    if (row != null)
                    {
                        maCode = row["MaHang"].ToString();
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi kiểm tra:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private void ImportEX()
        {
            var sheetNames = OpenExcelAndShowSheets();
            if (sheetNames.Count > 0)
            {
                using (var frm = new frmERP_BangSizeImport(sheetNames))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        string selectedSheet = frm.SelectedSheet;
                        ReadExcelToDataTable(selectedFilePath, selectedSheet);

                    }

                }
            }
            else
            {
                XtraMessageBox.Show("Không có sheet nào được chọn hoặc file rỗng.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImportEX();
        }

        private async void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(this, typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait), true, true, false);
                SplashScreenManager.Default.SetWaitFormCaption("Đang xử lý yêu cầu");
                SplashScreenManager.Default.SetWaitFormDescription("Đang chuẩn bị dữ liệu... (10%)");
                string url = $"{URL}KhoiTaoBOMV1/Get?action=GETSTTDOT&para1={_maKH.ToString()}&para2={_maHang.ToString()}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]")
                {
                    return;
                }
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                int _stt = Convert.ToInt32(tbl.Rows[0][0]) + 1;
                string tendotpost = _tenkhachhang.ToString() + '-' + _tenhang.ToString() + '-' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "|" + _stt;
                string madotpost = _maKH.ToString() + '_' + _maHang.ToString() + '_' + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + "|" + _stt;

                this.ActiveControl = button1;

                if (string.IsNullOrWhiteSpace(_maKH?.ToString()) ||
                    string.IsNullOrWhiteSpace(_maHang?.ToString()))
                {
                    SplashScreenManager.CloseForm();
                    return;
                }

                var tblGC1 = gridControl1.DataSource as DataTable;
                bool tonTaiNull = tblGC1 != null && tblGC1.AsEnumerable().Any(r => string.IsNullOrWhiteSpace(r["MaNhomChiTiet"].ToString()));
                if (tonTaiNull)
                {
                    XtraMessageBox.Show(this, "Vui lòng chọn đầy đủ chủng loại chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (tblGC1 == null || tblGC1.Rows.Count == 0)
                {
                    SplashScreenManager.CloseForm();
                    return;
                }
                bool success = await Task.Run(async () =>
                {
                    DataTable dtMauSP = createTypeMauSP();
                    foreach (DataRow _dr in tblGC1.Rows)
                    {
                        foreach (DataColumn _cl in tblGC1.Columns)
                        {
                            if (_cl.ColumnName.Contains("@MAU@") && !string.IsNullOrWhiteSpace(_dr[_cl.ColumnName].ToString()))
                            {
                                string mamau = _cl.ColumnName.ToString().Split('@')[2];
                                DataRow _nr = dtMauSP.NewRow();
                                _nr["ID"] = 0;
                                _nr["MaKH"] = _maKH;
                                _nr["MaHang"] = _maHang;
                                _nr["MaNhom"] = _dr["MaNhom"];
                                _nr["MaVTID"] = _dr["MaVTID"];
                                _nr["MauVTID"] = _dr[_cl.ColumnName];
                                _nr["KhoVaiID"] = _dr["KhoVaiID"];
                                _nr["MaMau"] = mamau.Trim();
                                _nr["MaDot"] = madotpost;
                                _nr["MaCode"] = _dr["MaCode"];
                                _nr["IsActive"] = _dr["IsActive"];
                                _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
                                dtMauSP.Rows.Add(_nr);
                            }
                        }

                    }

                    DataTable dtSizeSP = createTypeSizeSP();
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
                        foreach (DataColumn _cl in tblGC1.Columns)
                        {
                            if (_cl.ColumnName.Contains("@MAU@") && !string.IsNullOrWhiteSpace(_dr[_cl.ColumnName].ToString()))
                            {
                                string mamau = _cl.ColumnName.ToString().Split('@')[2];
                                //string mamau = _cl.ColumnName.Replace("@Mau@", "");
                                foreach (var kv in sizeMap)
                                {
                                    foreach (string size in kv.Value)
                                    {
                                        DataRow _nr = dtSizeSP.NewRow();
                                        _nr["MaKH"] = _maKH;
                                        _nr["MaHang"] = _maHang;
                                        _nr["MaNhom"] = _dr["MaNhom"];
                                        _nr["MaVTID"] = _dr["MaVTID"];
                                        _nr["MauVTID"] = _dr[_cl.ColumnName];
                                        _nr["KhoVaiID"] = _dr["KhoVaiID"];
                                        _nr["MaNhomSize"] = kv.Key;
                                        _nr["MaSize"] = size;
                                        _nr["DinhMuc"] = _dr["DinhMucChung"];
                                        _nr["MaDot"] = madotpost;
                                        _nr["Dot"] = tendotpost;
                                        _nr["NguoiTao"] = GlobleData.UserName;
                                        _nr["MaMau"] = mamau.Trim();
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

                    DataTable dtVatTuSP = createTypeVattuSP();
                    foreach (DataRow _dr in tblGC1.Rows)
                    {
                        foreach (DataColumn _cl in tblGC1.Columns)
                        {
                            if (_cl.ColumnName.Contains("@MAU@") && !string.IsNullOrWhiteSpace(_dr[_cl.ColumnName].ToString()))
                            {

                                DataRow _nr = dtVatTuSP.NewRow();
                                _nr["ID"] = 0;
                                _nr["MaKH"] = _maKH;
                                _nr["MaHang"] = _maHang;
                                _nr["MaNhom"] = _dr["MaNhom"];
                                _nr["MaVTID"] = _dr["MaVTID"];
                                _nr["MauVTID"] = _dr[_cl.ColumnName];
                                _nr["KhoVaiID"] = _dr["KhoVaiID"];
                                _nr["MaDot"] = madotpost;
                                _nr["Dot"] = tendotpost;
                                _nr["NguoiTao"] = GlobleData.UserName;
                                _nr["DinhMucChung"] = _dr["DinhMucChung"];
                                _nr["DinhMucChiTiet"] = _dr["DinhMucHaoHut"];
                                _nr["TachMau"] = _dr["TachMau"];
                                _nr["STT"] = _stt;
                                _nr["MaCode"] = _dr["MaCode"];
                                _nr["STTCode"] = _dr["STTCode"];
                                _nr["IsActive"] = _dr["IsActive"];
                                _nr["MaNhomChiTiet"] = _dr["MaNhomChiTiet"];
                                dtVatTuSP.Rows.Add(_nr);
                            }
                        }
                    }
                    SplashScreenManager.Default.SetWaitFormDescription("Đang lưu màu sản phẩm... (40%)");
                    string url1 = $"{URL}KhoiTaoBOMV1/Post1?Action=POSTMAUSP&para1={GlobleData.UserName}";
                    string result1 = await _clientExtension.PostAsync(url1, dtMauSP);
                    if (result1 != "True") return false;
                    SplashScreenManager.Default.SetWaitFormDescription("Đang lưu định mức size... (70%)");
                    string url2 = $"{URL}KhoiTaoBOMV1/Post2?Action=POSTDMSIZE&para1={GlobleData.UserName}";
                    string result2 = await _clientExtension.PostAsync(url2, dtSizeSP);
                    if (result2 != "True") return false;
                    SplashScreenManager.Default.SetWaitFormDescription("Đang lưu vật tư... (90%)");
                    string url3 = $"{URL}KhoiTaoBOMV1/Post3?Action=POSTVTSP&para1={GlobleData.UserName}";
                    string result3 = await _clientExtension.PostAsync(url3, dtVatTuSP);
                    if (result3 != "True") return false;
                    SplashScreenManager.Default.SetWaitFormDescription("Hoàn tất... (100%)");
                    return true;
                });
                SplashScreenManager.CloseForm(false);

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);
            }
         
        }

        private void bandedGridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
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

            }
            gridControl1.DataSource = tbl;
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

                // Thiết lập hình ảnh cho button
                // Cách 1: Từ Resources
                button.ImageOptions.Image = Properties.Resources.group_16x16;


                btnEdit.Buttons.Add(button);


                gridControl1.RepositoryItems.Add(btnEdit);

                // Xử lý click button
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

        private void LoadTVMauVT()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETMAUVTTV";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblMauVT = JsonConvert.DeserializeObject<DataTable>(json);
        }
        private void LoadCheckData()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GetCheckEX";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblCheck = JsonConvert.DeserializeObject<DataTable>(json);
        }
        private void LoadChecHang()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GetHangCheck&para1={_maKH}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblHangHoa = JsonConvert.DeserializeObject<DataTable>(json);
        }
        private void LoadMauSP()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETMAUSPCOLUMNS&para1={_maKH}&para2={_maHang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblMauSP = JsonConvert.DeserializeObject<DataTable>(json);
            createTableCT(tblMauSP);
        }
        private void LoadCLCT()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=CheckChungLoaiCT";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblCLCT = JsonConvert.DeserializeObject<DataTable>(json);
        }
        private void LoadSize()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?Action=GETSIZESP&para1={_maKH}&para2={_maHang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblSizeSP = JsonConvert.DeserializeObject<DataTable>(json);
        }
   
        private void repositoryItemSearchLookUpEditMauVTView_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            DataRow dr = bandedGridView1.GetFocusedDataRow();
            if (dr == null)
            {
                return;
            }
            string MauVTIDChung = dr["MauVTIDChung"].ToString();
            if (string.IsNullOrEmpty(MauVTIDChung))
                return; // Không có điều kiện ⇒ hiển thị toàn bộ

            var mauVTIDs = dr["MauVTIDChung"].ToString()
                       .Split(',')
                       .Select(id => id.Trim())
                       .ToList();
            var view = sender as DevExpress.XtraGrid.Views.Base.ColumnView;
            var mauVTID = Convert.ToString(
                view.GetListSourceRowCellValue(e.ListSourceRow, "MauVTID")
            );
            if (!mauVTIDs.Contains(mauVTID, StringComparer.OrdinalIgnoreCase))
            {
                e.Visible = false;
                e.Handled = true;
            }
        }
        private void btnChonSize_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
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
                        XtraMessageBox.Show("Vật tư đã được xét duyêt. Không thể chỉnh sửa Size sản phẩm. Vui lòng kiểm tra lại!!!", Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Question);
                        return;
                    }

                }
                frmPhanTichBOM_ChonSize frm = new frmPhanTichBOM_ChonSize(_maKH,_maHang, row["MaSizeChung"].ToString());
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
                if (frm.DialogResult != DialogResult.OK) return;
                string result = frm.SelectedResult;
                row["Size"] = frm.SelectedResultName;
                row["MaSizeChung"] = frm.SelectedResult;
                this.ActiveControl = button1;
            }
            catch (Exception ex)
            {


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
            return tbl;
        }
        private void loadSizeChung()
        {
          
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETSIZESP&para1={_maKH.ToString()}&para2={_maHang.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                tblSizeChung = JsonConvert.DeserializeObject<DataTable>(json);

            }
        }
    }
}