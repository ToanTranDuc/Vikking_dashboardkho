using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmERPPackageListXHSP : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty, _maDH = string.Empty, _maLenhSX = string.Empty, _lenhSX = string.Empty;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _checkCapPhat = false;
        private string _madh = string.Empty, _maKH = string.Empty, _poid = string.Empty, _maPKL = string.Empty, _maPhieuXH = string.Empty
            , _maKHEdit = string.Empty, _joinedMaDH = string.Empty, _joinedGopPO = string.Empty, _maDHEdit = string.Empty, _POEdit = string.Empty, _maPKLEdit = string.Empty;
        private bool _checkEdit;
        private DataTable tblChiTietTongFull = null;
        private int checkPO = 0, checkPKL = 0, checkDH = 0;
        private HttpClientExtension _clientExtension;
        public frmERPPackageListXHSP(string maPhieuXH = "", string maDH = "", string maKH = "", string maPKL = "", string po = "", bool checkEdit = false)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            grvTong.Appearance.FocusedCell.Options.UseBackColor = true;
            grvTong.Appearance.FocusedCell.BackColor = Color.FromArgb(234, 234, 234);
            grvTong.CustomColumnDisplayText += bandedGridView1_CustomColumnDisplayText;
            _maPhieuXH = maPhieuXH;
            _maDHEdit = maDH;
            _maKHEdit = maKH;
            _maPKLEdit = maPKL;
            _POEdit = po;
            tblChiTietTongFull = null;
            _checkEdit = checkEdit;
            checkPO = 0;
            checkPKL = 0;
            checkDH = 0;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InIt();
            if (_checkEdit) loadDanhSachXuatHang();
            LoadKhachHang();

        }
        private void loadDanhSachXuatHang()
        {
            string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/Get?Action=GetPhieuXHEdit&Para1={_maDHEdit}&Para2={_POEdit}&Para3={_maPKLEdit}&Para4={_maPhieuXH}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            tblChiTietTongFull = dt;
            DataTable tblView = CreateSummaryView(tblChiTietTongFull);

            // Gán vào Grid
            grcChiTietTong.DataSource = tblView;
            grcChiTietTong.RefreshDataSource();

        }
        private void InIt()
        {
            searchLookUpEdit1.Properties.ValueMember = "MaKH";
            searchLookUpEdit1.Properties.DisplayMember = "TenKH";

            searchLookUpEditPO.Properties.ValueMember = "POID";
            searchLookUpEditPO.Properties.DisplayMember = "PO";

            searchLookUpEditMaDH.Properties.ValueMember = "MaDH";
            searchLookUpEditMaDH.Properties.DisplayMember = "GopDH";

            searchLookUpEditMaPKL.Properties.ValueMember = "MaPKL";
            searchLookUpEditMaPKL.Properties.DisplayMember = "MaPKL";

        }
        private void LoadKhachHang()
        {
            string url = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit1.Properties.DataSource = dt;
            if (!_checkEdit) return;
            if (!string.IsNullOrEmpty(_maKHEdit))
            {
                bool exists = dt.AsEnumerable().Any(r => r["MaKH"].ToString() == _maKHEdit);
                if (exists)
                {
                    searchLookUpEdit1.EditValue = _maKHEdit;
                }
            }
        }
        private void LoadDonHang()
        {
            try
            {
                if (searchLookUpEdit1.EditValue is null) return;

                string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/Get?Action=GetDH&Para1={searchLookUpEdit1.EditValue.ToString()}&Para2=Para");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable dtDonHang = JsonConvert.DeserializeObject<DataTable>(json);
                if (dtDonHang == null || dtDonHang.Rows.Count == 0)
                {
                    searchLookUpEditMaDH.Properties.DataSource = null;
                    searchLookUpEditPO.Properties.DataSource = null;
                    searchLookUpEditMaPKL.Properties.DataSource = null;
                    grcTong.DataSource = null;
                    grcChiTietThung.DataSource = null;
                    return;
                }
                searchLookUpEditMaDH.Properties.DataSource = dtDonHang;
                if (!_checkEdit || _maKHEdit == "") return;
                // BẮT BUỘC
                if (checkDH == 0)
                {
                    searchLookUpEditMaDH.ShowPopup();
                    // Cho UI xử lý
                    Application.DoEvents();
                    searchLookUpEditMaDH.ClosePopup();
                    checkDH++;
                }
                var view = searchLookUpEditMaDH.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
                AutoCheckDonHang(view);
                _maKHEdit = "";
            }
            catch (Exception ex)
            {

            }
        }
        private void AutoCheckDonHang(GridView view)
        {
            string donhang = _maDHEdit;
            if (string.IsNullOrEmpty(donhang)) return;

            string[] arr = donhang.Split(';');


            for (int i = 0; i < view.DataRowCount; i++)
            {
                string madh = view.GetRowCellValue(i, searchLookUpEditMaDH.Properties.ValueMember)?.ToString();
                if (arr.Contains(madh))
                {
                    view.SelectRow(i);
                }
            }

        }
        private void LoadPO()
        {
            if (_madh is null) return;
            string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/Get?Action=GetPO&Para1={_madh}&Para2=Para");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dtPO = JsonConvert.DeserializeObject<DataTable>(json);
            if (dtPO == null | dtPO.Rows.Count == 0)
            {
                searchLookUpEditPO.Properties.DataSource = null;
                searchLookUpEditMaPKL.Properties.DataSource = null;
                grcTong.DataSource = null;
                grcChiTietThung.DataSource = null;
                return;
            };
            searchLookUpEditPO.Properties.DataSource = dtPO;
            if (!_checkEdit || _POEdit == "") return;
            // BẮT BUỘC
            if (checkPO == 0)
            {
                searchLookUpEditPO.ShowPopup();
                // Cho UI xử lý
                Application.DoEvents();
                searchLookUpEditPO.ClosePopup();
                checkPO++;
            }


            var view = searchLookUpEditPO.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
            AutoCheckPO(view);
        }
        private void AutoCheckPO(GridView view)
        {
            string po = _POEdit;
            if (string.IsNullOrEmpty(po)) return;

            string[] arr = po.Split(';');


            for (int i = 0; i < view.DataRowCount; i++)
            {
                string poText = view.GetRowCellValue(i, searchLookUpEditPO.Properties.ValueMember)?.ToString();
                if (arr.Contains(poText))
                {
                    view.SelectRow(i);
                }
            }
        }
        private void LoadMaPKL()
        {
            string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/Get?Action=GetMaPKL&Para1={_madh}&Para2={_poid}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dtMaPKL = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditMaPKL.Properties.DataSource = dtMaPKL;
            if (dtMaPKL.Rows.Count > 0)
            {
                if (!_checkEdit) return;
                // BẮT BUỘC
                if (checkPKL == 0)
                {
                    searchLookUpEditMaPKL.ShowPopup();
                    // Cho UI xử lý
                    Application.DoEvents();
                    searchLookUpEditMaPKL.ClosePopup();
                    checkPKL++;
                }


                var view = searchLookUpEditMaPKL.Properties.View as DevExpress.XtraGrid.Views.Grid.GridView;
                AutoCheckPKL(view);
            }
            else
            {
                searchLookUpEditMaPKL.Properties.DataSource = null;
                grcTong.DataSource = null;
                grcChiTietThung.DataSource = null;
                return;
            }


        }
        private void AutoCheckPKL(GridView view)
        {
            string mapkl = _maPKLEdit;
            if (string.IsNullOrEmpty(mapkl)) return;

            string[] arr = mapkl.Split(';');


            for (int i = 0; i < view.DataRowCount; i++)
            {
                string mapklText = view.GetRowCellValue(i, "Value")?.ToString();
                if (arr.Contains(mapklText))
                {
                    view.SelectRow(i);
                }
            }
        }
        private void LoadData()
        {

            string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/Get?Action=GetDanhSachThung&Para1={_joinedMaDH}&Para2={_joinedGopPO}&Para3={_maPKL}");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                grcTong.DataSource = null;
                grcChiTietThung.DataSource = null;
                return;
            }
            CreateBandSize(tbl, grvTong, gbSize, repotxtN0);
            KHDongThungLib.ProcessSttTrung1(tbl);

            grcTong.DataSource = tbl;

        }
        private void bandedGridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;

            if (e.Column == null || !e.Column.FieldName.Contains("@")) return;

            DataRow row = null;
            try { row = view.GetDataRow(e.ListSourceRowIndex); } catch { row = null; }
            if (row == null) return;


            int value;
            if (int.TryParse(e.DisplayText, out value))
            {
                if (value == 0)
                {
                    e.DisplayText = "-";
                }
            }
        }
        private async void LoadChiTietThung(string madh, string poid, string mapkl, string tuthung, string denthung)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(madh) || string.IsNullOrWhiteSpace(poid) ||
                    string.IsNullOrWhiteSpace(mapkl))
                {
                    XtraMessageBox.Show("Thông tin không hợp lệ.", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hiển thị loading
                this.Cursor = Cursors.WaitCursor;

                // Gọi API
                string url = $"{URL}KeHoachXuatHangThanhPham/Get?Action=DanhSachChiTietThung" +
                             $"&Para1={madh}&Para2={poid}&Para3={mapkl}&Para4={tuthung}&Para5={denthung}";

                string json = await _clientExtension.GetAsnyc(url);

                if (string.IsNullOrWhiteSpace(json))
                    return;

                // Deserialize
                DataTable tblChiTiet = JsonConvert.DeserializeObject<DataTable>(json);

                if (tblChiTiet == null || tblChiTiet.Rows.Count == 0)
                {
                    grcChiTietThung.DataSource = null;
                    XtraMessageBox.Show("Không có dữ liệu chi tiết thùng.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // Trừ đi số lượng đã xuất
                DataTable tblDaXuat = tblChiTietTongFull;
                if (tblDaXuat != null && tblDaXuat.Rows.Count > 0)
                {
                    DeductExportedQuantity(tblChiTiet, tblDaXuat);
                }

                // Gán vào Grid
                grcChiTietThung.DataSource = tblChiTiet;
                grcChiTietThung.RefreshDataSource();
            }
            catch (JsonException jsonEx)
            {
                XtraMessageBox.Show($"Lỗi phân tích dữ liệu: {jsonEx.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        private void DeductExportedQuantity(DataTable tblChiTiet, DataTable tblDaXuatFull)
        {
            foreach (DataRow row in tblChiTiet.Rows)
            {
                string maDH = row["MaDH"]?.ToString() ?? string.Empty;
                string poID = row["POID"]?.ToString() ?? string.Empty;
                string maPKL = row["MaPKL"]?.ToString() ?? string.Empty;
                int sttThung = Convert.ToInt32(row["SttThung"]);

                // Tìm dòng trùng khóa trong bảng đã xuất FULL
                DataRow rowDaXuat = tblDaXuatFull.AsEnumerable().FirstOrDefault(r =>
                    (r.Field<string>("MaDH") ?? string.Empty) == maDH &&
                    (r.Field<string>("POID") ?? string.Empty) == poID &&
                    (r.Field<string>("MaPKL") ?? string.Empty) == maPKL &&
                    (Convert.ToInt32(r["SttThung"]) == sttThung)
                );

                if (rowDaXuat != null)
                {
                    // Lấy số lượng đã xuất
                    int daXuat = rowDaXuat["SLXuat"] != DBNull.Value
                        ? Convert.ToInt32(rowDaXuat["SLXuat"])
                        : 0;

                    // Lấy số lượng sản phẩm hiện tại
                    int soLuongSP = row["SoLuongSP"] != DBNull.Value
                        ? Convert.ToInt32(row["SoLuongSP"])
                        : 0;

                    // Tính số lượng còn lại
                    int slConLai = soLuongSP - daXuat;

                    // Đảm bảo không âm
                    row["SLSPCL"] = Math.Max(0, slConLai);
                }
            }
        }


        private void grvTong_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = grvTong.GetFocusedDataRow();
            if (dr == null) return;
            var madh = dr["MaDH"].ToString();
            var poid = dr["POID"].ToString();
            var mapkl = dr["MaPKL"].ToString();
            var tuthung = dr["TuThung"].ToString();
            var denthung = dr["DenThung"].ToString();
            LoadChiTietThung(madh, poid, mapkl, tuthung, denthung);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

            DataTable tbl = grcChiTietThung.DataSource as DataTable;
            tbl.AsEnumerable().ToList().ForEach(r => r["SLXuat"] = 0);

        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSLXuat1.Text)) return;

            int soNhap;
            if (!int.TryParse(txtSLXuat1.Text, out soNhap)) return;

            DataTable tbl = grcChiTietThung.DataSource as DataTable;
            if (tbl == null) return;

            // Lấy các dòng selected
            int[] selectedHandles = grvChiTietThung.GetSelectedRows();

            foreach (int handle in selectedHandles)
            {
                DataRow row = grvChiTietThung.GetDataRow(handle);
                if (row == null) continue;

                int soLuongSP = Convert.ToInt32(row["SoLuongSP"]);

                row["SLXuat"] = (soNhap > soLuongSP) ? soLuongSP : soNhap;
            }
            txtSLXuat1.Text = "";
            // Refresh hiển thị
            grcChiTietThung.RefreshDataSource();
        }


        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy DataTable nguồn
                DataTable tblChiTiet = grcChiTietThung.DataSource as DataTable;
                if (tblChiTiet == null || tblChiTiet.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu để xử lý.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Lọc các dòng có SLXuat > 0
                var rowsToExport = tblChiTiet.AsEnumerable()
                    .Where(r => r.Field<object>("SLXuat") != null &&
                               Convert.ToInt32(r["SLXuat"]) > 0)
                    .ToList();

                if (!rowsToExport.Any())
                {
                    XtraMessageBox.Show("Không có dòng nào có số lượng xuất > 0.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Khởi tạo bảng chi tiết full nếu chưa có
                if (tblChiTietTongFull == null || tblChiTietTongFull.Rows.Count == 0)
                {
                    tblChiTietTongFull = tblChiTiet.Clone();
                }

                // Xử lý từng dòng xuất vào bảng full
                foreach (DataRow sourceRow in rowsToExport)
                {
                    ProcessExportRow(sourceRow, tblChiTietTongFull);
                }

                // Tạo bảng view (sum theo size, bỏ SttThung)
                DataTable tblView = CreateSummaryView(tblChiTietTongFull);

                // Gán vào Grid
                grcChiTietTong.DataSource = tblView;
                grcChiTietTong.RefreshDataSource();

                // Reset SLXuat về 0 và cập nhật SLSPCL
                ResetAndRecalculateSLSPCL(tblChiTiet, tblChiTietTongFull);

                // Refresh grid chi tiết thùng
                grcChiTietThung.RefreshDataSource();

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private DataTable CreateSummaryView(DataTable tblFull)
        {
            if (tblFull == null || tblFull.Rows.Count == 0)
                return new DataTable();

            // Clone cấu trúc
            DataTable tblView = tblFull.Clone();

            // Group theo MaDH, POID, MaPKL (bỏ SttThung)
            var grouped = tblFull.AsEnumerable()
                .GroupBy(r => new
                {
                    MaDH = r.Field<string>("MaDH") ?? "",
                    POID = r.Field<string>("POID") ?? "",
                    MaPKL = r.Field<string>("MaPKL") ?? "",
                    SizeID = r.Field<string>("SizeID") ?? ""
                });

            foreach (var group in grouped)
            {
                DataRow newRow = tblView.NewRow();

                // Copy các trường cơ bản
                newRow["MaDH"] = group.Key.MaDH;
                newRow["POID"] = group.Key.POID;
                newRow["MaPKL"] = group.Key.MaPKL;
                newRow["SizeID"] = group.Key.SizeID;
                // Copy các trường khác từ dòng đầu tiên
                DataRow firstRow = group.First();
                foreach (DataColumn col in tblView.Columns)
                {
                    string colName = col.ColumnName;

                    // Bỏ qua SttThung
                    if (colName == "SttThung") continue;

                    // Đã xử lý các khóa ở trên
                    if (colName == "MaDH" || colName == "POID" || colName == "MaPKL" || colName == "SizeID") continue;

                    // Sum các cột size (chứa @) và SLXuat
                    if (colName.Contains("@") || colName == "SLXuat")
                    {
                        int sum = group.Sum(r =>
                            r[colName] != DBNull.Value ? Convert.ToInt32(r[colName]) : 0);
                        newRow[colName] = sum;
                    }
                    else
                    {
                        // Copy giá trị từ dòng đầu tiên
                        if (firstRow[colName] != DBNull.Value)
                        {
                            newRow[colName] = firstRow[colName];
                        }
                    }
                }

                tblView.Rows.Add(newRow);
            }

            return tblView;
        }



        private void ProcessExportRow(DataRow sourceRow, DataTable targetTable)
        {
            // Lấy thông tin khóa
            string maDH = sourceRow["MaDH"]?.ToString() ?? string.Empty;
            string poID = sourceRow["POID"]?.ToString() ?? string.Empty;
            string maPKL = sourceRow["MaPKL"]?.ToString() ?? string.Empty;
            int sttThung = Convert.ToInt32(sourceRow["SttThung"]);
            int slXuatMoi = Convert.ToInt32(sourceRow["SLXuat"]);

            // Tìm dòng trùng khóa trong bảng đích
            DataRow existingRow = targetTable.AsEnumerable().FirstOrDefault(t =>
                (t.Field<string>("MaDH") ?? string.Empty) == maDH &&
                (t.Field<string>("POID") ?? string.Empty) == poID &&
                (t.Field<string>("MaPKL") ?? string.Empty) == maPKL &&
                (Convert.ToInt32(t["SttThung"]) == sttThung)
            );

            if (existingRow != null)
            {
                // Cộng dồn số lượng xuất
                int slCu = Convert.ToInt32(existingRow["SLXuat"]);
                existingRow["SLXuat"] = slCu + slXuatMoi;
            }
            else
            {
                // Thêm dòng mới
                DataRow newRow = targetTable.NewRow();
                newRow.ItemArray = (object[])sourceRow.ItemArray.Clone();
                targetTable.Rows.Add(newRow);
            }
        }
        private void ResetAndRecalculateSLSPCL(DataTable tblChiTiet, DataTable tblDaXuatFull)
        {
            // Duyệt qua từng dòng trong bảng chi tiết thùng
            foreach (DataRow row in tblChiTiet.Rows)
            {
                string maDH = row["MaDH"]?.ToString() ?? string.Empty;
                string poID = row["POID"]?.ToString() ?? string.Empty;
                string maPKL = row["MaPKL"]?.ToString() ?? string.Empty;
                int sttThung = Convert.ToInt32(row["SttThung"]);

                // Reset SLXuat về 0
                row["SLXuat"] = 0;

                // Tìm dòng tương ứng trong bảng đã xuất FULL (có SttThung)
                DataRow rowDaXuat = tblDaXuatFull.AsEnumerable().FirstOrDefault(r =>
                    (r.Field<string>("MaDH") ?? string.Empty) == maDH &&
                    (r.Field<string>("POID") ?? string.Empty) == poID &&
                    (r.Field<string>("MaPKL") ?? string.Empty) == maPKL &&
                    (Convert.ToInt32(r["SttThung"]) == sttThung)
                );

                if (rowDaXuat != null)
                {
                    // Lấy SoLuongSP gốc (từ SLSPCLBD hoặc SoLuongSP)
                    int soLuongGoc = row["SoLuongSP"] != DBNull.Value
                        ? Convert.ToInt32(row["SoLuongSP"])
                        : 0;

                    int slDaXuat = rowDaXuat["SLXuat"] != DBNull.Value
                        ? Convert.ToInt32(rowDaXuat["SLXuat"])
                        : 0;

                    // Tính SLSPCL còn lại = SoLuongSP - SLXuat đã xuất
                    int slspclConLai = soLuongGoc - slDaXuat;

                    // Đảm bảo không âm
                    row["SLSPCL"] = Math.Max(0, slspclConLai);
                }
            }
        }

        private void grvChiTietThung_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            var view = sender as BandedGridView;
            if (view == null) return;

            if (e.Column == null || !e.Column.FieldName.Contains("SLSPCL") || !e.Column.FieldName.Contains("SLXuat")) return;

            DataRow row = null;
            try { row = view.GetDataRow(e.ListSourceRowIndex); } catch { row = null; }
            if (row == null) return;


            int value;
            if (int.TryParse(e.DisplayText, out value))
            {
                if (value == 0)
                {
                    e.DisplayText = "-";
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy DataTable nguồn
                DataTable tblChiTiet = grcChiTietThung.DataSource as DataTable;

                if (tblChiTiet == null || tblChiTiet.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu để xử lý.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                int[] selectedHandles = grvChiTietThung.GetSelectedRows();
                if (selectedHandles.Length > 0)
                {
                    foreach (int handle in selectedHandles)
                    {
                        DataRow row = grvChiTietThung.GetDataRow(handle);
                        if (row == null) continue;

                        row["SLXuat"] = row["SLSPCL"];
                    }
                }
                else
                {
                    foreach (DataRow row in tblChiTiet.Rows)
                    {
                        row["SLXuat"] = row["SLSPCL"];
                    }
                }

                grcChiTietThung.RefreshDataSource();


            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void grvChiTietTong_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DXMenuItem DeleteMau = new DXMenuItem();
            DeleteMau.Caption = "Xóa";
            DeleteMau.Click += DeleteS;
            e.Menu.Items.Add(DeleteMau);
        }
        private void DeleteS(object sender, EventArgs e)
        {
            try
            {
                GridView view = grvChiTietTong;

                if (view == null || view.FocusedRowHandle < 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn dòng cần xóa.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy dòng đang chọn từ View
                DataRow selectedViewRow = view.GetDataRow(view.FocusedRowHandle);
                if (selectedViewRow == null)
                    return;

                // Lấy thông tin khóa
                string maDH = selectedViewRow["MaDH"]?.ToString() ?? string.Empty;
                string poID = selectedViewRow["POID"]?.ToString() ?? string.Empty;
                string maPKL = selectedViewRow["MaPKL"]?.ToString() ?? string.Empty;
                string SizeID = selectedViewRow["SizeID"]?.ToString() ?? string.Empty;

                // Xóa tất cả các dòng có cùng khóa trong bảng Full (bao gồm các SttThung khác nhau)
                if (tblChiTietTongFull != null)
                {
                    var rowsToDelete = tblChiTietTongFull.AsEnumerable()
                        .Where(r =>
                            (r.Field<string>("MaDH") ?? string.Empty) == maDH &&
                            (r.Field<string>("POID") ?? string.Empty) == poID &&
                            (r.Field<string>("MaPKL") ?? string.Empty) == maPKL &&
                            (r.Field<string>("SizeID") ?? string.Empty) == SizeID)
                        .ToList();

                    foreach (DataRow row in rowsToDelete)
                    {
                        row.Delete();
                    }

                    tblChiTietTongFull.AcceptChanges();
                }

                // Tạo lại view
                DataTable tblView = CreateSummaryView(tblChiTietTongFull);
                grcChiTietTong.DataSource = tblView;
                grcChiTietTong.RefreshDataSource();

                DataTable tblChiTiet = grcChiTietThung.DataSource as DataTable;

                int countUpdated = 0;

                foreach (DataRow row in tblChiTiet.Rows)
                {
                    if (row["SLSPCLBD"] == DBNull.Value)
                    {
                        row["SLSPCL"] = 0;
                        continue;
                    }

                    int slspclbd = Convert.ToInt32(row["SLSPCLBD"]);

                    if (slspclbd > 0)
                    {
                        row["SLSPCL"] = slspclbd;
                        countUpdated++;
                    }
                    else
                    {
                        row["SLSPCL"] = 0;
                    }
                }
                string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/PostXH?Action=DeletePhieuSize&Para1={_maPhieuXH}&Para2={maDH}&Para3={poID}&Para4={maPKL}&Para5={SizeID}");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json.ToUpper() == "TRUE")
                {
                   
                }

                grcChiTietThung.RefreshDataSource();
                
                XtraMessageBox.Show("Xóa thành công.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Có lỗi xảy ra khi xóa: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void simpleButton5_Click(object sender, EventArgs e)
        {
            // Reset tất cả
            tblChiTietTongFull = null;
            grcChiTietTong.DataSource = new DataTable();

            DataTable tblChiTiet = grcChiTietThung.DataSource as DataTable;

            if (tblChiTiet == null || tblChiTiet.Rows.Count == 0)
            {
                XtraMessageBox.Show("Không có dữ liệu để xử lý.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int countUpdated = 0;

            foreach (DataRow row in tblChiTiet.Rows)
            {
                if (row["SLSPCLBD"] == DBNull.Value)
                {
                    row["SLSPCL"] = 0;
                    continue;
                }

                int slspclbd = Convert.ToInt32(row["SLSPCLBD"]);

                if (slspclbd > 0)
                {
                    row["SLSPCL"] = slspclbd;
                    countUpdated++;
                }
                else
                {
                    row["SLSPCL"] = 0;
                }
            }

            grcChiTietThung.RefreshDataSource();
        }

        public static void CreateBandSize(DataTable dt, BandedGridView grvShared, GridBand gbSizeA, DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repotxtN0, bool IsCheckSumSize = true)
        {
            ClearBand(gbSizeA);
            foreach (DataColumn dc in dt.Columns)
            {
                string colName = dc.ColumnName;
                if (!colName.Contains('@')) continue;
                var _sizeID = colName.Split('@')[1];
                var _size = colName.Split('@')[0];


                BandedGridColumn col = new BandedGridColumn();
                col.AppearanceCell.Options.UseTextOptions = true;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.Options.UseTextOptions = true;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.Caption = _size;
                col.FieldName = colName;
                col.Name = "col" + _sizeID;
                col.OptionsColumn.AllowEdit = true;
                col.ColumnEdit = repotxtN0;
                col.Visible = true;
                col.Width = 60;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                if (IsCheckSumSize)
                {
                    if (col.FieldName.Contains("@"))
                    {
                        GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                        itemSize.FieldName = col.FieldName;
                        itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                        itemSize.DisplayFormat = "{0:n0}";
                        itemSize.ShowInGroupColumnFooter = col;
                        grvShared.GroupSummary.Add(itemSize);
                    }
                    string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrName.Length > 1)
                    {
                        col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    }
                }

                // Thêm cột vào grid
                grvShared.Columns.AddRange(new BandedGridColumn[] { col });

                // Tạo và thêm GridBand vào grid
                GridBand gb = new GridBand();
                gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
                gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
                gb.AppearanceHeader.ForeColor = System.Drawing.Color.White;
                gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                gb.AppearanceHeader.Options.UseBackColor = true;
                gb.AppearanceHeader.Options.UseFont = true;
                gb.AppearanceHeader.Options.UseForeColor = true;
                gb.AppearanceHeader.Options.UseTextOptions = true;
                gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gb.Caption = _size;
                gb.Columns.Add(col);
                gb.Name = "gb" + "Size@" + _sizeID;
                gb.VisibleIndex = 0;
                gb.Width = 60;
                gbSizeA.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            }
        }



        public static void ClearBand(GridBand gbSizeA)
        {
            gbSizeA.Children.Clear();
        }
        string selectValueDH = "";
        private void grvSDH_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            string selectedValues = string.Join(";", grvSDH.GetSelectedRows().Select(rowHandle2 =>
                    grvSDH.GetRowCellValue(rowHandle2, searchLookUpEditMaDH.Properties.ValueMember)));
            searchLookUpEditMaDH.EditValue = selectedValues;
            if (searchLookUpEditMaDH.EditValue is null) return;
            _madh = searchLookUpEditMaDH.EditValue.ToString();
            LoadPO();


        }
        private void searchLookUpEditMaDH_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedRows = grvSDH.GetSelectedRows();
            var selectedValues = selectedRows
                .Select(rowHandle => grvSDH.GetRowCellValue(rowHandle, searchLookUpEditMaDH.Properties.DisplayMember)?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();

            selectValueDH = selectedValues.Any() ? string.Join("; ", selectedValues) : "Chọn đơn hàng";
            e.DisplayText = selectValueDH;
        }
        string selectValuePO = "";
        private void grvSPO_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            string selectedValues = string.Join(";", grvSPO.GetSelectedRows().Select(rowHandle2 =>
                   grvSPO.GetRowCellValue(rowHandle2, searchLookUpEditPO.Properties.ValueMember)));
            searchLookUpEditPO.EditValue = selectedValues;
            if (searchLookUpEditPO.EditValue is null) return;
            _poid = searchLookUpEditPO.EditValue.ToString();

            LoadMaPKL();
        }
        private void searchLookUpEditPO_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedRows = grvSPO.GetSelectedRows();
            var selectedValues = selectedRows
                .Select(rowHandle => grvSPO.GetRowCellValue(rowHandle, searchLookUpEditPO.Properties.DisplayMember)?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();

            selectValuePO = selectedValues.Any() ? string.Join("; ", selectedValues) : "Chọn po";
            e.DisplayText = selectValuePO;
        }
        string selectValuePKL = "";
        private void grvSMaPKL_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            string selectedValues = string.Join(";", grvSMaPKL.GetSelectedRows().Select(rowHandle2 =>
                   grvSMaPKL.GetRowCellValue(rowHandle2, searchLookUpEditMaPKL.Properties.ValueMember)));
            searchLookUpEditMaPKL.EditValue = selectedValues;
            if (searchLookUpEditMaPKL.EditValue is null) return;
            _maPKL = searchLookUpEditMaPKL.EditValue.ToString();
            _joinedMaDH = string.Join(";",
                   grvSMaPKL.GetSelectedRows()
                       .Select(h => grvSMaPKL.GetDataRow(h)?["MaDHKH"]?.ToString())
                       .Where(x => !string.IsNullOrEmpty(x))
               );
            _joinedGopPO = string.Join(";",
                   grvSMaPKL.GetSelectedRows()
                       .Select(h => grvSMaPKL.GetDataRow(h)?["POID"]?.ToString())
                       .Where(x => !string.IsNullOrEmpty(x))
               );
            LoadData();
        }

        private void searchLookUpEditMaPKL_Properties_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            var selectedRows = grvSMaPKL.GetSelectedRows();
            var selectedValues = selectedRows
                .Select(rowHandle => grvSMaPKL.GetRowCellValue(rowHandle, searchLookUpEditMaPKL.Properties.DisplayMember)?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();

            selectValuePKL = selectedValues.Any() ? string.Join("; ", selectedValues) : "Chọn PKL";
            e.DisplayText = selectValuePKL;
        }


        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            LoadDonHang();
        }
        private void btSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable tbl = CreatePackageListDataTable();
            string userName = GlobleData.UserName;
            foreach (DataRow item in tblChiTietTongFull.Rows)
            {
                DataRow row = tbl.NewRow();
                row["PhieuXH"] = _maPhieuXH;
                row["MaDH"] = item["MaDH"];
                row["MaKH"] = searchLookUpEdit1.EditValue.ToString();
                row["POID"] = item["POID"];
                row["PO"] = item["PO"];
                row["MaPKL"] = item["MaPKL"];
                row["DauSizeID"] = item["DauSizeID"];
                row["DauSize"] = item["DauSize"];
                row["ColorID"] = item["ColorID"];
                row["TenMau"] = item["TenMau"];
                row["SizeID"] = item["SizeID"];
                row["Size"] = item["Size"];
                row["SttThung"] = item["SttThung"];
                row["SLXuat"] = item["SLXuat"];
                row["Sort"] = 0;
                row["IsXuatHang"] = 0;
                row["UserKiem"] = userName;

                tbl.Rows.Add(row);
            }
            string url = string.Format("{0}", URL + $"KeHoachXuatHangThanhPham/Post?action=Save");
            var msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl); }).Result;

            if (msResult.ToUpper() == "TRUE")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        public DataTable CreatePackageListDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("PhieuXH", typeof(string));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("MaKH", typeof(string));
            dt.Columns.Add("POID", typeof(string));
            dt.Columns.Add("PO", typeof(string));
            dt.Columns.Add("MaPKL", typeof(string));
            dt.Columns.Add("DauSizeID", typeof(string));
            dt.Columns.Add("DauSize", typeof(string));
            dt.Columns.Add("ColorID", typeof(string));
            dt.Columns.Add("TenMau", typeof(string));
            dt.Columns.Add("SizeID", typeof(string));
            dt.Columns.Add("Size", typeof(string));
            dt.Columns.Add("SttThung", typeof(int));
            dt.Columns.Add("SLXuat", typeof(int));
            dt.Columns.Add("Sort", typeof(int));
            dt.Columns.Add("IsXuatHang", typeof(int));
            dt.Columns.Add("NgayXuatHang", typeof(DateTime));
            dt.Columns.Add("NgayXacNhanXH", typeof(DateTime));
            dt.Columns.Add("UserKiem", typeof(string));

            return dt;
        }

    }
}