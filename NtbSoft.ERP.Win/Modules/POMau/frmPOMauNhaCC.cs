using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.QuanLyDonHang;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;

namespace NtbSoft.ERP.Win.Modules.POMau
{
    public partial class frmPOMauNhaCC : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private int currentPhieuNumber = 1;
        private string formattedPhieuNumber = string.Empty;
        private HttpClientExtension _clientExtension;
        private DataTable copiedData;
        string _maKH, _maHang, _maPhieu = string.Empty;
        private int _rightClickRowHandle = -1;
        private ContextMenuStrip _contextMenuNCC;
        private ContextMenuStrip _contextMenuPBG;
        private DataTable _nccTable;
        private DataTable _phieuTable = new DataTable();
        public frmPOMauNhaCC(string maKH = "", string maHang = "", string maPhieu = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _maKH = maKH ?? "";
            _maHang = maHang ?? "";
            _maPhieu = maPhieu ?? "";
        }
        protected override void OnLoad(EventArgs e)
        {
            if (!string.IsNullOrEmpty(_maKH))
            {
                searchLookUpEditKH.EditValue = _maKH;
            }
            if (!string.IsNullOrEmpty(_maHang) && !string.IsNullOrEmpty(_maKH))
            {
                searchLookUpEditKH_EditValueChanged(null, null);
                searchLookUpEditMH.EditValue = _maHang;
                searchLookUpEditMH_EditValueChanged(null, null);
            }

            CreateSearchLookup();
            loadRepoChungLoai();
            BuildNCCContextMenu();
            BuildPBGContextMenu();
            InitPhieuBaoGiaRepo();
            loaddata();
            CreateSearchLookUpNCC();
            CheckDuyetNCC();
            gridView1.RefreshData();
        }
        private void InitPhieuBaoGiaRepo()
        {
            _phieuTable = new DataTable();
            _phieuTable.Columns.Add("MaPhieuBG", typeof(string));
            _phieuTable.Columns.Add("TenPhieu", typeof(string));

            repoPhieuBaoGia.DataSource = _phieuTable;
            repoPhieuBaoGia.ValueMember = "MaPhieuBG";
            repoPhieuBaoGia.DisplayMember = "TenPhieu";
            repoPhieuBaoGia.NullText = "";
        }
        private void loaddata()
        {
            if (string.IsNullOrEmpty(_maPhieu)) return;

            string urlID = $"{URL}POMau/Get?action=GetThongTin&para1={_maPhieu}";
            string jsonID = Task.Run(async () => await _clientExtension.GetAsnyc(urlID)).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonID);
            if (tbl == null || tbl.Rows.Count == 0) return;

            string urlCT = $"{URL}POMau/Get?action=GetCTNCC&para1={_maPhieu}";
            string jsonCT = Task.Run(async () => await _clientExtension.GetAsnyc(urlCT)).Result;
            DataTable tblCT = JsonConvert.DeserializeObject<DataTable>(jsonCT);
            if (tblCT == null || tblCT.Rows.Count == 0) return;

            foreach (DataRow row in tblCT.Rows)
            {
                if (tblCT.Columns.Contains("MaCLVT") && tblCT.Columns.Contains("ChungLoaiVatTu"))
                    row["ChungLoaiVatTu"] = row["MaCLVT"]?.ToString() ?? "";
            }

            DataRow firstRow = tbl.Rows[0];
            textBoxPosition.Text = firstRow["Position"]?.ToString() ?? "";

            textEditForecastqty.Text = firstRow["SoLuong"]?.ToString() ?? "0";
            textBoxSizeRatio.Text = firstRow["TyLeKichThuoc"]?.ToString() ?? "";
            textBoxColorways.Text = firstRow["MauSac"]?.ToString() ?? "";
            textBoxGhiChu.Text = firstRow["GhiChu"]?.ToString() ?? "";

            if (tbl.Columns.Contains("KichThuocBaoGia"))
                searchLookUpEditQuotainsize.EditValue = firstRow["KichThuocBaoGia"]?.ToString() ?? "";

            if (!tblCT.Columns.Contains("MaPhieuBG"))
                tblCT.Columns.Add("MaPhieuBG", typeof(string));
            if (!tblCT.Columns.Contains("LoaiVT"))
                tblCT.Columns.Add("LoaiVT", typeof(string));

            foreach (DataRow row in tblCT.Rows)
            {
                bool isNPL = row["IsNPL"] != null && row["IsNPL"] != DBNull.Value
                             && Convert.ToBoolean(row["IsNPL"]);
                row["LoaiVT"] = isNPL ? "Nguyên liệu" : "Phụ liệu";
            }
            LoadPhieuBaoGia(tblCT);
            gridControl1.DataSource = tblCT;
            gridView1.RefreshData();
            gridView1.ExpandAllGroups();
        }
        private void LoadPhieuBaoGia(DataTable tblCT)
        {
            try
            {
                if (!tblCT.Columns.Contains("MaPhieuBG")) return;

                foreach (DataRow row in tblCT.Rows)
                {
                    string ma = row["MaPhieuBG"]?.ToString() ?? "";
                    string ten = row["TenPhieu"]?.ToString() ?? "";
                    if (string.IsNullOrEmpty(ma)) continue;

                    bool exists = _phieuTable.AsEnumerable()
                        .Any(r => r["MaPhieuBG"]?.ToString() == ma);
                    if (!exists)
                    {
                        DataRow nr = _phieuTable.NewRow();
                        nr["MaPhieuBG"] = ma;
                        nr["TenPhieu"] = ten;
                        _phieuTable.Rows.Add(nr);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadPhieuBaoGia error: " + ex.Message);
            }
        }
        private void CheckDuyetNCC()
        {
            DataRow dr = gridView1.GetFocusedDataRow();
            if (dr == null) return;
            string maPhieu = dr["MaPhieu"].ToString();
            string url = $"{URL}POMau/Get?action=GetDuyetNCC&para1={maPhieu}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            if (!string.IsNullOrEmpty(json))
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    int duyetncc = 0;
                    int.TryParse(tbl.Rows[0]["DuyetNCC"]?.ToString(), out duyetncc);

                    if (duyetncc == 1 && gridView1.DataSource != null)
                    {
                        gridView1.OptionsBehavior.Editable = false;
                        layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }

                }
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
        private void simpleButton1_Click(object sender, EventArgs e)
        {

            DataTable tbl = gridControl1.DataSource as DataTable;
            if (tbl == null)
            {
                tbl = createTableGridControl();
            }
            frmPhanTichBOM_ChonVatTuV1 frm = new frmPhanTichBOM_ChonVatTuV1(tbl);
            if (frm.ShowDialog() == DialogResult.OK)
            {

                List<DataRow> lstSelect = frm.lstSelect.OrderBy(row => Convert.ToInt32(row["STT"])).ToList();

                foreach (DataRow row in lstSelect)
                {
                    DataRow _nr = tbl.NewRow();
                    _nr["MaCLVT"] = row["MaNhom"].ToString();
                    _nr["ChungLoaiVatTu"] = row["MaNhom"].ToString();
                    _nr["MaVTID"] = row["MaVTID"].ToString();
                    _nr["MaVT"] = row["MaVT"].ToString();
                    _nr["ChiTiet"] = row["VatTu"].ToString();
                    _nr["KhoVaiID"] = row["KhoVaiID"].ToString();
                    _nr["KhoVai"] = row["KhoVai"].ToString();
                    _nr["MaDVVT"] = row["MaDVVT"].ToString();
                    _nr["TenDVVT"] = row["TenDVVT"].ToString();
                    _nr["MauVTID"] = row["MauVTID"].ToString();
                    _nr["MaMauVT"] = row["MaMauVT"].ToString();
                    _nr["MauVT"] = row["MauVT"].ToString();
                    _nr["DinhMuc"] = 0;
                    _nr["HaoHut"] = 0;
                    _nr["GhiChu"] = "";
                    _nr["LoaiVT"] = row["NhomNPL"].ToString();
                    _nr["IsNPL"] = row["NPL"].ToString();
                    _nr["TenDVKV"] = row["TenDVKV"].ToString();
                    _nr["MaNCC"] = "";
                    _nr["TenNCC"] = "";
                    tbl.Rows.Add(_nr);
                }
                gridControl1.DataSource = tbl;
            }
        }

        private DataTable createTableGridControl()
        {
            DataTable tbl = new DataTable("tbl");
            tbl.Columns.Add("MaCLVT", typeof(string));
            tbl.Columns.Add("ChungLoaiVatTu", typeof(string));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaVT", typeof(string));
            tbl.Columns.Add("ChiTiet", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("MaMauVT", typeof(string));
            tbl.Columns.Add("MauVT", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("KhoVai", typeof(string));
            tbl.Columns.Add("TenDVKV", typeof(string));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("TenDVVT", typeof(string));
            tbl.Columns.Add("DinhMucKH", typeof(string));
            tbl.Columns.Add("HaoHut", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("LoaiVT", typeof(string));
            tbl.Columns.Add("IsNPL", typeof(bool));
            tbl.Columns.Add("MaNCC", typeof(string));
            tbl.Columns.Add("TenNCC", typeof(string));
            tbl.Columns.Add("MaPhieuBG", typeof(string));
            tbl.Columns.Add("TenPhieu", typeof(string));
            tbl.Columns.Add("DonGia", typeof(decimal));
            return tbl;
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


            }
            catch (Exception ex)
            {

            }
        }

        private void repositoryItemTextEditDM_KeyPress(object sender, KeyPressEventArgs e)
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

        private void repositoryItemTextEditHH_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnXoaDong_Click(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetFocusedDataRow();
            if (dr == null) return;

            DataTable tbl = gridControl1.DataSource as DataTable;
            if (tbl == null) return;

            tbl.Rows.Remove(dr);
            gridControl1.DataSource = tbl;
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (searchLookUpEditKH.EditValue == null)
            {
                XtraMessageBox.Show("Vui lòng chọn Khách hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                if (searchLookUpEditMH.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn Mã hàng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            if (gridView1.IsEditing)
            {
                gridView1.CloseEditor();
                gridView1.UpdateCurrentRow();
            }
            DataTable tbl = gridControl1.DataSource as DataTable;
            if (tbl == null) return;
            DataTable dtSave = new DataTable();
            dtSave.Columns.Add("MaPhieu", typeof(string));
            dtSave.Columns.Add("STT", typeof(int));
            dtSave.Columns.Add("MaKH", typeof(string));
            dtSave.Columns.Add("MaHang", typeof(string));
            dtSave.Columns.Add("SMV", typeof(float));
            dtSave.Columns.Add("SoLuong", typeof(int));
            dtSave.Columns.Add("TyLeKichThuoc", typeof(string));
            dtSave.Columns.Add("MauSac", typeof(string));
            dtSave.Columns.Add("KichThuocBaoGia", typeof(string));
            dtSave.Columns.Add("Position", typeof(string));
            dtSave.Columns.Add("GhiChu", typeof(string));
            dtSave.Columns.Add("HinhAnh", typeof(string));
            dtSave.Columns.Add("TenFile", typeof(string));

            DataTable dtSaveCT = new DataTable();
            dtSaveCT.Columns.Add("ID", typeof(int));
            dtSaveCT.Columns.Add("MaPhieu", typeof(string));
            dtSaveCT.Columns.Add("MaNhom", typeof(string));
            dtSaveCT.Columns.Add("MaVTID", typeof(string));
            dtSaveCT.Columns.Add("MaVT", typeof(string));
            dtSaveCT.Columns.Add("ChiTiet", typeof(string));
            dtSaveCT.Columns.Add("MauVTID", typeof(string));
            dtSaveCT.Columns.Add("MaMauVT", typeof(string));
            dtSaveCT.Columns.Add("MauVT", typeof(string));
            dtSaveCT.Columns.Add("KhoVaiID", typeof(string));
            dtSaveCT.Columns.Add("KhoVai", typeof(string));
            dtSaveCT.Columns.Add("TenDVKV", typeof(string));
            dtSaveCT.Columns.Add("MaDVVT", typeof(string));
            dtSaveCT.Columns.Add("TenDVVT", typeof(string));
            dtSaveCT.Columns.Add("MaNCC", typeof(string));
            dtSaveCT.Columns.Add("PhieuBaoGia", typeof(string));
            dtSaveCT.Columns.Add("DonGia", typeof(float));
            dtSaveCT.Columns.Add("DinhMuc", typeof(float));
            dtSaveCT.Columns.Add("HaoHut", typeof(float));
            dtSaveCT.Columns.Add("GhiChu", typeof(string));
            dtSaveCT.Columns.Add("Position", typeof(string));

            DataRow _dr = dtSave.NewRow();
            _dr["MaPhieu"] = _maPhieu;
            _dr["STT"] = 0;
            _dr["MaKH"] = searchLookUpEditKH.EditValue.ToString();
            _dr["MaHang"] = searchLookUpEditMH.EditValue.ToString();
            _dr["SMV"] = 0;
            _dr["SoLuong"] = string.IsNullOrWhiteSpace(textEditForecastqty.Text) ? 0 : int.Parse(textEditForecastqty.Text.Trim());
            _dr["TyLeKichThuoc"] = textBoxSizeRatio.Text.Trim();
            _dr["MauSac"] = textBoxColorways.Text.Trim();
            _dr["KichThuocBaoGia"] = searchLookUpEditQuotainsize.EditValue?.ToString()?.Trim() ?? "";
            _dr["Position"] = textBoxPosition.Text?.Trim() ?? "";
            _dr["GhiChu"] = textBoxGhiChu.Text;

            dtSave.Rows.Add(_dr);


            foreach (DataRow dr in tbl.Rows)
            {
                DataRow _drow = dtSaveCT.NewRow();
                _drow["ID"] = dr["ID"].ToString() == "" ? 0 : dr["ID"];
                _drow["MaNhom"] = dr["ChungLoaiVatTu"]?.ToString() ?? null;
                _drow["MaPhieu"] = _maPhieu;
                _drow["MaVTID"] = dr["MaVTID"]?.ToString() ?? null;
                _drow["MaVT"] = dr["MaVT"]?.ToString() ?? null;
                _drow["ChiTiet"] = dr["ChiTiet"]?.ToString() ?? null;
                _drow["MauVTID"] = dr["MauVTID"]?.ToString() ?? null;
                _drow["MaMauVT"] = dr["MaMauVT"]?.ToString() ?? null;
                _drow["MauVT"] = dr["MauVT"]?.ToString() ?? null;
                _drow["KhoVaiID"] = dr["KhoVaiID"]?.ToString() ?? null;
                _drow["KhoVai"] = dr["KhoVai"]?.ToString() ?? null;
                _drow["TenDVKV"] = dr["TenDVKV"]?.ToString() ?? null;
                _drow["MaDVVT"] = dr["MaDVVT"]?.ToString() ?? null;
                _drow["TenDVVT"] = dr["TenDVVT"]?.ToString() ?? null;
                _drow["MaNCC"] = dr["MaNCC"]?.ToString() ?? null;
                _drow["PhieuBaoGia"] = dr["MaPhieuBG"]?.ToString() ?? null;
                _drow["DonGia"] = dr["DonGia"] ?? 0;
                _drow["DinhMuc"] = dr["DinhMuc"] ?? 0;
                _drow["HaoHut"] = dr["HaoHut"] ?? 0;
                _drow["GhiChu"] = dr["GhiChu"]?.ToString() ?? null;
                _drow["Position"] = dr["Position"]?.ToString() ?? null;
                dtSaveCT.Rows.Add(_drow);
            }
            string url = $"{URL}POMau/PostNCC?Action=PostNCC&para1={GlobleData.UserName}";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToLower() == "true")
            {
                string urlCT = $"{URL}POMau/PostNCCCT?Action=PostNCCCT&para1={GlobleData.UserName}";
                string msResultCT = Task.Run(async () => { return await _clientExtension.PostAsync(urlCT, dtSaveCT); }).Result;
                if (msResultCT.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    this.Close();
                }
            }
        }
        private void CreateSearchLookUpNCC()
        {
            try
            {
                string maclvt = gridView1.GetRowCellValue(0, "MaCLVT")?.ToString() ?? "";
                string urlNCC = $"{URL}POMau/Get?action=GetNCC&para1={maclvt}";
                string jsonNCC = Task.Run(async () => await _clientExtension.GetAsnyc(urlNCC)).Result;
                _nccTable = JsonConvert.DeserializeObject<DataTable>(jsonNCC);

                repoNCC.DataSource = _nccTable;
                repoNCC.ValueMember = "MaNCC";
                repoNCC.DisplayMember = "TenNCC";
                repoNCC.NullText = "";

                var view = repoNCC.View;
                view.Columns.Clear();
                repoNCC.PopulateViewColumns();

                if (view.Columns["MaNCC"] != null)
                {
                    view.Columns["MaNCC"].Visible = false;
                    view.Columns["MaNCC"].VisibleIndex = -1;
                }
                if (view.Columns["TenNCC"] != null)
                {
                    view.Columns["TenNCC"].Caption = "Tên nhà cung cấp";
                    view.Columns["TenNCC"].VisibleIndex = 0;
                    view.Columns["TenNCC"].Width = 280;
                }

                view.OptionsBehavior.Editable = false;
                view.OptionsSelection.EnableAppearanceFocusedCell = true;

                repoNCC.QueryPopUp += (s, e) =>
                {
                    int rowHandle = gridView1.FocusedRowHandle;
                    if (rowHandle < 0 || gridView1.IsGroupRow(rowHandle)) return;

                    string mc = gridView1.GetRowCellValue(rowHandle, "MaCLVT")?.ToString() ?? "";
                    string url = $"{URL}POMau/Get?action=GetNCC&para1={mc}";
                    string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                    DataTable dtNew = JsonConvert.DeserializeObject<DataTable>(json);
                    _nccTable.Clear();
                    if (dtNew != null)
                    {
                        foreach (DataRow r in dtNew.Rows)
                            _nccTable.ImportRow(r);
                    }
                };
                GridColumn colMaNCC = gridView1.Columns["MaNCC"];
                if (colMaNCC != null)
                {
                    colMaNCC.ColumnEdit = repoNCC;
                    colMaNCC.OptionsColumn.AllowEdit = true;
                    colMaNCC.Visible = true;
                    colMaNCC.Caption = "Nhà cung cấp";
                }
                repoPhieuBaoGia.DataSource = _phieuTable;
                repoPhieuBaoGia.ValueMember = "MaPhieuBG";
                repoPhieuBaoGia.DisplayMember = "TenPhieu";
                repoPhieuBaoGia.NullText = "";
                gridView1.LayoutChanged();
                repoNCC.EditValueChanged += (s, ev) =>
                {
                    this.ActiveControl = simpleButton2;
                };
            }
            catch { }
        }
        private void BuildPBGContextMenu()
        {
            _contextMenuPBG = new ContextMenuStrip();

            var mnuFillAllPhieu = new ToolStripMenuItem("Áp dụng Phiếu Báo Giá cho tất cả dòng");
            mnuFillAllPhieu.Click += MnuFillAllPhieu_Click;

            _contextMenuPBG.Items.Add(mnuFillAllPhieu);
        }
        private void BuildNCCContextMenu()
        {
            _contextMenuNCC = new ContextMenuStrip();
            var mnuFillAll = new ToolStripMenuItem("Áp dụng cho tất cả dòng");
            mnuFillAll.Click += (sender, e) =>
            {
                if (_rightClickRowHandle < 0) return;
                object maNCC = gridView1.GetRowCellValue(_rightClickRowHandle, "MaNCC");
                DataTable tbl = gridControl1.DataSource as DataTable;
                if (tbl == null || maNCC == null) return;
                string maNCC_str = maNCC.ToString().Trim();
                if (string.IsNullOrEmpty(maNCC_str)) return;
                gridView1.BeginDataUpdate();
                try
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        string maclvt = row["MaCLVT"]?.ToString() ?? "";
                        bool nccCungCap = KiemTraNCCCungCapCLVT(maNCC_str, maclvt);
                        if (nccCungCap)
                            row["MaNCC"] = maNCC_str;
                    }
                }
                finally { gridView1.EndDataUpdate(); }
            };
            _contextMenuNCC.Items.Add(mnuFillAll);
        }
        private void MnuFillAllPhieu_Click(object sender, EventArgs e)
        {
            DataRow focusedRow = gridView1.GetFocusedDataRow();
            if (focusedRow == null) return;

            string maPhieuBG = focusedRow["MaPhieuBG"]?.ToString() ?? "";
            string maNCCFocused = focusedRow["MaNCC"]?.ToString() ?? "";

            if (string.IsNullOrEmpty(maPhieuBG) || string.IsNullOrEmpty(maNCCFocused))
            {
                XtraMessageBox.Show("Chưa có thông tin Phiếu Báo Giá hoặc Nhà cung cấp.", "Thông báo");
                return;
            }

            //if (XtraMessageBox.Show($"Áp dụng cho tất cả dòng.",
            //    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            //    return;

            DataTable tbl = gridControl1.DataSource as DataTable;
            if (tbl == null) return;

            int successCount = 0;
            int skipCount = 0;

            gridView1.BeginDataUpdate();
            try
            {
                foreach (DataRow row in tbl.Rows)
                {
                    string maNCC = row["MaNCC"]?.ToString() ?? "";
                    string maclvt = row["MaCLVT"]?.ToString() ?? "";
                    string maVTID = row["MaVTID"]?.ToString() ?? "";
                    string mauVTID = row["MauVTID"]?.ToString() ?? "";
                    string khoVaiID = row["KhoVaiID"]?.ToString() ?? "";
                    if (string.IsNullOrEmpty(maNCC))
                    {
                        skipCount++;
                        continue;
                    }
                    if (KiemTraNCCPhieuBaoGia(maNCC, maPhieuBG, maclvt, maVTID, mauVTID, khoVaiID))
                    {
                        LoadDonGiaFromPhieuBaoGia(maNCC, maPhieuBG, row);
                        successCount++;
                    }
                    else
                    {
                        skipCount++;
                    }
                }
            }
            finally
            {
                gridView1.EndDataUpdate();
                gridView1.RefreshData();
            }

        }
        private bool KiemTraNCCPhieuBaoGia(string maNCC, string maPhieuBG, string maclvt, string maVTID, string mauVTID, string khoVaiID)
        {
            try
            {
                string url = $"{URL}POMau/Get?action=GetPhieuBaoGia&para1={maNCC}&para2={maPhieuBG}&para3={maclvt}&para4={maVTID}&para5={mauVTID}&para6={khoVaiID}";

                string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

                if (string.IsNullOrWhiteSpace(json) || json.Trim() == "false" || json.Trim() == "0")
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool KiemTraNCCCungCapCLVT(string maNCC, string maclvt)
        {
            try
            {
                string urlNCC = $"{URL}POMau/Get?action=GetNCC&para1={maclvt}";
                string jsonNCC = Task.Run(async () => await _clientExtension.GetAsnyc(urlNCC)).Result;
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonNCC);
                return dt != null && dt.AsEnumerable()
                    .Any(r => r["MaNCC"]?.ToString().Trim() == maNCC);
            }
            catch { return false; }
        }
        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            GridGroupRowInfo info = e.Info as GridGroupRowInfo;

            if (view == null || info == null) return;

            int groupLevel = view.GetRowLevel(e.RowHandle);

            GridColumn groupColumn = info.Column;


            if (groupColumn == gridColumn11)

            {

                info.GroupText = string.Format("{0}", info.GroupValueText);

            }

            //if (groupColumn == gridColumn1)

            //{

            //    info.GroupText = string.Format("{0}", info.GroupValueText);

            //}

            if (view.IsGroupRow(e.RowHandle))

            {

                Color textColor = Color.Black;

                switch (groupLevel)

                {

                    case 0: textColor = Color.MediumBlue; break;

                    case 1: textColor = Color.Maroon; break;

                }

                e.Appearance.ForeColor = textColor;

                e.DefaultDraw();

                e.Handled = true;

            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            //try
            //{
            //    if (e.Column == null) return;
            //    Rectangle rect = e.Bounds;
            //    ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            //    Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(47, 84, 150), Color.FromArgb(47, 84, 150), e.Column.AppearanceHeader.GradientMode);
            //    rect.Inflate(-1, -1);
            //    e.Graphics.FillRectangle(brush, rect);

            //    // Thiết lập font và màu chữ
            //    Font font = new Font(e.Appearance.Font, FontStyle.Bold);
            //    StringFormat stringFormat = new StringFormat();
            //    stringFormat.Alignment = StringAlignment.Center;
            //    stringFormat.LineAlignment = StringAlignment.Center;
            //    SolidBrush textColorBrush = new SolidBrush(Color.White);

            //    // Vẽ chữ
            //    e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);

            //    foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            //    {
            //        DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            //    }
            //    e.Handled = true;
            //}
            //catch (Exception ex)
            //{

            //}
        }

        private void textBoxColorways_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void textBoxQuotainsize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void searchLookUpEditMH_EditValueChanged(object sender, EventArgs e)
        {
            string makh = searchLookUpEditKH.EditValue.ToString();
            string mahang = searchLookUpEditMH.EditValue.ToString();
            string url = $"{URL}POMau/Get?Action=GetSize&para1={makh}&para2={mahang}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditQuotainsize.Properties.DataSource = tbl;
            searchLookUpEditQuotainsize.Properties.DisplayMember = "TenSize";
            searchLookUpEditQuotainsize.Properties.ValueMember = "MaSize";

            string urlT = $"{URL}POMau/Get?Action=GetSizeChuoi&para1={makh}&para2={mahang}";
            string jsonT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlT); }).Result;
            DataTable tblT = JsonConvert.DeserializeObject<DataTable>(jsonT);
            textBoxSizeRatio.Text = tblT.Rows[0][0].ToString();
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

        private void simpleButton11_Click(object sender, EventArgs e)
        {

            addRowNPL(true);

        }

        private void simpleButton12_Click(object sender, EventArgs e)
        {
            addRowNPL(false);
        }
        private void addRowNPL(bool NPL)
        {
            try
            {
                DataTable tbl = gridControl1.DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    tbl = createTableGridControl();
                }
                DataRow dr = tbl.NewRow();

                dr["LoaiVT"] = NPL ? "Nguyên liệu" : "Phụ liệu";
                dr["IsNPL"] = NPL;
                tbl.Rows.Add(dr);
                gridControl1.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }


        }

        private void repoKhoSize_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DataRow row = gridView1.GetFocusedDataRow();
            if (row == null) return;

            bool loaiVT = row["IsNPL"] != null && Convert.ToBoolean(row["IsNPL"]);
            string maKSHienTai = row["KhoVaiID"]?.ToString() ?? "";

            frmPOMau_ChonKS frm = new frmPOMau_ChonKS(loaiVT, maKSHienTai);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                DataRow selectedRow = frm.SelectedRow;
                if (selectedRow == null) return;

                row["KhoVaiID"] = selectedRow["KhoVaiID"];
                row["KhoVai"] = selectedRow["KhoVai"];
                row["TenDVKV"] = selectedRow["TenDVKV"];
                gridView1.RefreshData();
            }
            this.ActiveControl = simpleButton2;
        }

        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
                if (e.HitInfo.InRowCell && !gridView1.IsGroupRow(e.HitInfo.RowHandle))
                {
                    if (gridView1.IsEditing)
                        gridView1.CloseEditor();

                    _rightClickRowHandle = e.HitInfo.RowHandle;
                    gridView1.FocusedRowHandle = e.HitInfo.RowHandle;

                    if (e.HitInfo.Column?.FieldName == "MaNCC")
                    {
                        e.Allow = false;
                        _rightClickRowHandle = e.HitInfo.RowHandle;
                        gridView1.FocusedRowHandle = e.HitInfo.RowHandle;
                        _contextMenuNCC.Show(Cursor.Position);
                        return;
                    }
                    if (e.HitInfo.Column?.FieldName == "MaPhieuBG")
                    {
                        e.Allow = false;
                        _rightClickRowHandle = e.HitInfo.RowHandle;
                        gridView1.FocusedRowHandle = e.HitInfo.RowHandle;
                        _contextMenuPBG.Show(Cursor.Position);
                        return;
                    }
                }
                if (gridView1.RowCount > 0)
                {
                    if (e.Menu == null)
                        return;
                    e.Menu.Items.Clear();

                    if (e.HitInfo.InRow)
                    {
                        if (e.HitInfo.Column != null)
                        {

                            bool IsXetDuyet = false;
                            DataRow row_focused = gridView1.GetFocusedDataRow();
                            if (row_focused == null) return;


                            DevExpress.Utils.Menu.DXMenuItem menuThemDongItem = new DevExpress.Utils.Menu.DXMenuItem("Thêm dòng", ThemDong);
                            e.Menu.Items.Add(menuThemDongItem);

                            DevExpress.Utils.Menu.DXMenuItem menuCopyCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy dòng", CopyDong);
                            e.Menu.Items.Add(menuCopyCopyItem);

                            DevExpress.Utils.Menu.DXMenuItem menuCopyNhieuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy nhiều dòng", CopyNhieuDong);
                            e.Menu.Items.Add(menuCopyNhieuCopyItem);

                            DevExpress.Utils.Menu.DXMenuItem menuCopyPasteCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Paste nhiều dòng", PasteNhieuDong);
                            e.Menu.Items.Add(menuCopyPasteCopyItem);
                            //DevExpress.Utils.Menu.DXMenuItem menuCopyPasteExcelCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Paste Excel Itemcode", PasteExcel);
                            //e.Menu.Items.Add(menuCopyPasteExcelCopyItem);

                            //DevExpress.Utils.Menu.DXMenuItem menuCopyPasteExcelCopyKhoSize = new DevExpress.Utils.Menu.DXMenuItem("Paste Khổ/Size", PasteKhoSizeExcel);
                            //e.Menu.Items.Add(menuCopyPasteExcelCopyKhoSize);

                            DevExpress.Utils.Menu.DXMenuItem menuCopyPasteDMExcelCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Paste Excel", PasteExcelDinhMuc);
                            e.Menu.Items.Add(menuCopyPasteDMExcelCopyItem);

                            DevExpress.Utils.Menu.DXMenuItem menuRemoveVatTuItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng", RemoveVatTu);
                            e.Menu.Items.Add(menuRemoveVatTuItem);



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
        private void ThemDong(object sender, EventArgs e)
        {
            GridView view = gridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                DataTable table = gridControl1.DataSource as DataTable;
                if (table == null) return;

                DataRow currentRow = view.GetDataRow(rowHandle);
                if (currentRow == null) return;

                string currentMaVTID = currentRow["MaVTID"].ToString();

                string currentKhoVaiID = currentRow["KhoVaiID"].ToString();
                bool isNL = currentRow["IsNPL"].ToString() == "True" ? true : false;

                // Tạo dòng mới
                DataRow newRow = table.NewRow();
                newRow["IsNPL"] = currentRow["IsNPL"];
                newRow["LoaiVT"] = currentRow["IsNPL"].ToString() == "True" ? "Nguyên liệu" : "Phụ liệu";

                int insertIndex = view.GetDataSourceRowIndex(rowHandle) + 1;
                table.Rows.InsertAt(newRow, insertIndex);
                view.RefreshData();

            }
            //  this.ActiveControl = button1;
        }
        private void CopyDong(object sender, EventArgs e)
        {
            GridView view = gridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                DataTable table = gridControl1.DataSource as DataTable;
                if (table == null) return;

                DataRow currentRow = view.GetDataRow(rowHandle);
                if (currentRow == null) return;
                // Tạo dòng mới
                DataRow newRow = table.NewRow();
                foreach (DataColumn col in table.Columns)
                {
                    newRow[col.ColumnName] = currentRow[col.ColumnName];
                }
                int insertIndex = view.GetDataSourceRowIndex(rowHandle) + 1;
                table.Rows.InsertAt(newRow, insertIndex);
                view.RefreshData();
                int newRowHandle = view.GetRowHandle(insertIndex);
                view.FocusedRowHandle = newRowHandle;
                view.MakeRowVisible(newRowHandle);
            }
        }
        private void CopyNhieuDong(object sender, EventArgs e)
        {

            GridView view = gridView1;
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
                int[] selectedRows = gridView1.GetSelectedRows();
                if (selectedRows.Length == 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn dòng cần copy!", "Thông báo");
                    return;
                }
                copiedData = (gridView1.GridControl.DataSource as DataTable).Clone();

                foreach (int rowHandle in selectedRows)
                {
                    if (rowHandle >= 0)
                    {
                        DataRow sourceRow = gridView1.GetDataRow(rowHandle);
                        DataRow newRow = copiedData.NewRow();

                        foreach (DataColumn col in (gridView1.GridControl.DataSource as DataTable).Columns)
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

            GridView view = gridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                PasteCopiedRows();
            }
        }
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

                int currentRowHandle = gridView1.FocusedRowHandle;

                // LẤY CỘT ĐANG ĐƯỢC FOCUS
                GridColumn focusedColumn = gridView1.FocusedColumn;
                string focusedColumnName = focusedColumn?.FieldName;

                // KIỂM TRA GROUP ROW
                if (gridView1.IsGroupRow(currentRowHandle))
                {
                    XtraMessageBox.Show("Vui lòng chọn một dòng dữ liệu, không phải dòng nhóm!", "Thông báo");
                    return;
                }

                if (currentRowHandle < 0)
                {
                    XtraMessageBox.Show("Vui lòng chọn vị trí để paste!", "Thông báo");
                    return;
                }

                DataTable dt = gridView1.GridControl.DataSource as DataTable;
                if (dt == null)
                {
                    XtraMessageBox.Show("DataSource bị null!", "Lỗi");
                    return;
                }

                gridView1.BeginUpdate();

                try
                {
                    int currentHandle = currentRowHandle;
                    int copiedRowCount = 0;

                    while (copiedRowCount < copiedData.Rows.Count)
                    {
                        // BỎ QUA GROUP ROWS
                        while (currentHandle < gridView1.RowCount &&
                               gridView1.IsGroupRow(currentHandle))
                        {
                            currentHandle++;
                        }

                        // Nếu hết dòng visible, thoát
                        if (currentHandle >= gridView1.RowCount)
                        {
                            break;
                        }

                        try
                        {
                            DataRow copiedRow = copiedData.Rows[copiedRowCount];
                            DataRow targetRow = gridView1.GetDataRow(currentHandle);

                            if (targetRow != null)
                            {

                                // PASTE TẤT CẢ CÁC CỘT (CODE GỐC)
                                foreach (DataColumn col in dt.Columns)
                                {
                                    if (copiedData.Columns.Contains(col.ColumnName))
                                    {
                                        targetRow[col.ColumnName] = copiedRow[col.ColumnName];
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
                    gridView1.EndUpdate();
                    gridView1.RefreshData();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi paste: {ex.Message}", "Lỗi");
            }
        }
        private void PasteExcelDinhMuc(object sender, EventArgs e)
        {

            GridView view = gridView1;
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

                int startRow = view.FocusedRowHandle;

                foreach (string row in data)
                {
                    if (!view.IsValidRowHandle(startRow)) break;

                    PasteCell(row, startRow++, view);
                    //DataRow dr = gridView1.GetDataRow(startRow);
                    //if (dr == null) return;

                    //startRow++;
                }

                view.CloseEditor();

            }
            catch (Exception ex)
            {
            }
        }
        private void PasteCell(string data, int rowHandle, GridView view)
        {
            if (string.IsNullOrEmpty(data)) return;

            string[] rowData = data.Split('\t');
            int columnIndex = view.FocusedColumn.VisibleIndex;

            int originalRowHandle = view.GetDataSourceRowIndex(rowHandle);

            for (int i = 0; i < rowData.Length; i++)
            {
                if (i >= view.VisibleColumns.Count) break;

                try
                {
                    GridColumn targetColumn = view.VisibleColumns[columnIndex + i];
                    if (targetColumn == null) break;
                    if (targetColumn.FieldName == "DinhMucKH" || targetColumn.FieldName == "HaoHut")
                    {
                        if (!float.TryParse(rowData[i].Replace(",", "."),
                            System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out float numValue))
                        {
                            continue;
                        }
                        view.SetRowCellValue(rowHandle, targetColumn, numValue);
                        continue;
                    }
                    Type fieldType = view.Columns[targetColumn.FieldName].ColumnType;


                    if (fieldType == typeof(int))
                    {
                        if (int.TryParse(rowData[i].Replace(",", ""), out int intValue))
                        {
                            view.SetRowCellValue(rowHandle, targetColumn, intValue);
                        }
                        else
                        {

                            this.ActiveControl = simpleButton1;
                        }
                    }
                    else if (fieldType == typeof(float))
                    {
                        if (float.TryParse(rowData[i].Replace(",", ""), out float floatValue))
                        {
                            view.SetRowCellValue(rowHandle, targetColumn, floatValue);
                        }
                        else
                        {

                            this.ActiveControl = simpleButton1;
                        }
                    }
                    else
                    {

                        view.SetRowCellValue(rowHandle, targetColumn, rowData[i]);
                        this.ActiveControl = simpleButton1;
                    }
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }
        private void RemoveVatTu(object sender, EventArgs e)
        {
            DataRow dr = gridView1.GetFocusedDataRow();
            if (dr == null) return;

            DataTable tbl = gridControl1.DataSource as DataTable;
            if (tbl == null) return;

            tbl.Rows.Remove(dr);
            gridControl1.DataSource = tbl;
        }
        private void loadRepoChungLoai()
        {
            repoChungLoai.DisplayMember = "ChungLoaiVatTu";
            repoChungLoai.ValueMember = "MaCLVT";
            string url = $"{URL}POMau/Get?action=GETCHUNGLOAI";

            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblChungLoaiChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
            repoChungLoai.DataSource = tblChungLoaiChiTiet;
        }
        private void repoPhieuBaoGia_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                SearchLookUpEdit edit = sender as SearchLookUpEdit;
                if (edit == null || edit.EditValue == null) return;

                DataRow row = gridView1.GetFocusedDataRow();
                if (row == null) return;

                string maPhieuBG = edit.EditValue.ToString();
                if (string.IsNullOrEmpty(maPhieuBG)) return;

                var found = _phieuTable.AsEnumerable().FirstOrDefault(r => r["MaPhieuBG"]?.ToString() == maPhieuBG);

                row["MaPhieuBG"] = maPhieuBG;
                //row["TenPhieu"] = found?["TenPhieu"]?.ToString() ?? "";/1111

                string maNCC = row["MaNCC"]?.ToString() ?? "";
                if (string.IsNullOrEmpty(maNCC))
                {
                    XtraMessageBox.Show("Vui lòng chọn Nhà cung cấp trước!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                LoadDonGiaFromPhieuBaoGia(maNCC, maPhieuBG, row);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                this.ActiveControl = simpleButton2;
            }
        }
        private void LoadDonGiaFromPhieuBaoGia(string maNCC, string maPhieuBG, DataRow currentRow)
        {
            try
            {
                string url = $"{URL}POMau/Get?action=GetDonGiaVT&para1={maPhieuBG}";
                string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                if (dt == null || dt.Rows.Count == 0)
                {
                    gridView1.RefreshRow(gridView1.FocusedRowHandle);
                    return;
                }
                string maclvt = currentRow["MaCLVT"]?.ToString() ?? "";
                string maVTID = currentRow["MaVTID"]?.ToString() ?? "";
                string mauVTID = currentRow["MauVTID"]?.ToString() ?? "";
                string khoSizeID = currentRow["KhoVaiID"]?.ToString() ?? "";
                string maDVVT = currentRow["MaDVVT"]?.ToString() ?? "";

                DataRow found = dt.AsEnumerable().FirstOrDefault(r =>
                    r["ChungLoaiCC"]?.ToString() == maclvt &&
                    r["MaVTID"]?.ToString() == maVTID &&
                    r["MauVTID"]?.ToString() == mauVTID &&
                    r["KhoSizeID"]?.ToString() == khoSizeID &&
                    r["MaDVVT"]?.ToString() == maDVVT);

                if (found != null)
                {
                    currentRow["MaPhieuBG"] = maPhieuBG;
                    currentRow["DonGia"] = Convert.ToDecimal(found["DonGiaPBG"]);
                }
                gridView1.RefreshRow(gridView1.FocusedRowHandle);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi LoadDonGia: " + ex.Message);
            }
        }
        private void repoPhieuBaoGia_QueryPopUp(object sender, CancelEventArgs e)
        {
            DataRow row = gridView1.GetFocusedDataRow();
            if (row == null) { e.Cancel = true; return; }

            string maNCC = row["MaNCC"]?.ToString() ?? "";
            string maclvt = row["MaCLVT"]?.ToString() ?? "";

            if (string.IsNullOrEmpty(maNCC))
            {
                e.Cancel = true;
                this.BeginInvoke(new Action(() => XtraMessageBox.Show("Vui lòng chọn Nhà cung cấp trước!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                return;
            }

            try
            {
                string maVTID = row["MaVTID"]?.ToString() ?? "";
                string mauVTID = row["MauVTID"]?.ToString() ?? "";
                string khoVaiID = row["KhoVaiID"]?.ToString() ?? "";
                string url = $"{URL}POMau/Get?action=GetPhieuBaoGia&para1={maNCC}&para2=&para3={maclvt}&para4={maVTID}&para5={mauVTID}&para6={khoVaiID}";
                string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                DataTable dtNew = JsonConvert.DeserializeObject<DataTable>(json);

                if (dtNew != null)
                {
                    foreach (DataRow r in dtNew.Rows)
                    {
                        string ma = r["MaPhieuBG"]?.ToString() ?? "";
                        bool exists = _phieuTable.AsEnumerable()
                            .Any(x => x["MaPhieuBG"]?.ToString() == ma);
                        if (!exists)
                            _phieuTable.ImportRow(r);
                    }
                }

                if (dtNew == null || dtNew.Rows.Count == 0)
                {
                    e.Cancel = true;
                    this.BeginInvoke(new Action(() =>
                        XtraMessageBox.Show("Nhà cung cấp này không có phiếu báo giá đang có hiệu lực!",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                this.BeginInvoke(new Action(() =>
                    XtraMessageBox.Show("Lỗi: " + ex.Message, "Lỗi")));
            }
        }

        private void gridView1_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (gridView1.FocusedColumn == null)
                        return;
                    DataRow row = gridView1.GetFocusedDataRow();
                    if (row == null) return;
                    string fieldName = gridView1.FocusedColumn.FieldName;

                    if ((fieldName == "DinhMucKH" || fieldName == "HaoHut")
                        && gridView1.ActiveEditor is TextEdit editor)
                    {
                        editor.KeyDown += (s, ke) =>
                        {
                            if (ke.Control && ke.KeyCode == Keys.V)
                            {
                                string clipText = Clipboard.GetText().Trim();
                                if (!float.TryParse(clipText.Replace(",", "."),
                                    System.Globalization.NumberStyles.Float,
                                    System.Globalization.CultureInfo.InvariantCulture, out _))
                                {
                                    ke.Handled = true;
                                    ke.SuppressKeyPress = true;
                                }
                            }
                        };
                    }

                    if (fieldName == "ChungLoaiVatTu")
                    {
                        gridControl1.BeginInvoke(new Action(() =>
                        {
                            if (gridView1.ActiveEditor is SearchLookUpEdit searchLookUpEdit)
                                if (!searchLookUpEdit.IsPopupOpen)
                                    searchLookUpEdit.ShowPopup();
                        }));
                    }

                }
                catch (Exception ex)
                {


                }
            }
            catch (Exception ex)
            { }
        }

        private void repoMauVT_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DataRow row = gridView1.GetFocusedDataRow();
            if (row == null || IsRowSaved(row)) return;
            string maMau = row["MaMauVT"]?.ToString() ?? "";

            frmPOMau_ChonMau frm = new frmPOMau_ChonMau(maMau);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                DataRow selectedRow = frm.SelectedRow;
                if (selectedRow == null) return;
                row["MauVTID"] = selectedRow["MauVTID"];
                row["MaMauVT"] = selectedRow["MaMauVT"];
                row["MauVT"] = selectedRow["MauVT"];
                gridView1.RefreshData();
            }
            this.ActiveControl = simpleButton2;
        }
        private void repoItemCode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DataRow row = gridView1.GetFocusedDataRow();
            if (row == null || IsRowSaved(row)) return;

            frmPOMau_ChonItemCode frm = new frmPOMau_ChonItemCode();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                DataRow selectedRow = frm.SelectedRow;
                if (selectedRow == null) return;
                row["MaVTID"] = selectedRow["MaVTID"];
                row["MaVT"] = selectedRow["MaVT"];
                row["ChiTiet"] = selectedRow["ChiTiet"];
                gridView1.RefreshData();
            }
            this.ActiveControl = simpleButton2;
        }

        private void repoDonViVT_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DataRow row = gridView1.GetFocusedDataRow();
            if (row == null || IsRowSaved(row)) return;
            frmPOMau_ChonDVVT frm = new frmPOMau_ChonDVVT();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                DataRow selectedRow = frm.SelectedRow;
                if (selectedRow == null) return;
                row["MaDVVT"] = selectedRow["MaDVVT"];
                row["TenDVVT"] = selectedRow["TenDVVT"];
                gridView1.RefreshData();
            }
            this.ActiveControl = simpleButton2;
        }
        private void repositoryItemSearchLookUpEdit1View_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            DataRow dr = gridView1.GetFocusedDataRow();
            if (dr == null)
                return;
            string manhom = dr["ChungLoaiVatTu"].ToString();
            bool isNPL = dr["IsNPL"] != DBNull.Value && Convert.ToBoolean(dr["IsNPL"]);

            var view = sender as DevExpress.XtraGrid.Views.Base.ColumnView;
            //string listMaNhom = Convert.ToString(view.GetListSourceRowCellValue(e.ListSourceRow, "ChungLoaiVatTu"));
            int listIsNPL = Convert.ToInt32(view.GetListSourceRowCellValue(e.ListSourceRow, "IsNPL"));
            if (listIsNPL != (isNPL ? 1 : 0))
            {
                e.Visible = false;
                e.Handled = true;
            }
        }
        private bool IsRowSaved(DataRow row)
        {
            return row != null && row.Table.Columns.Contains("ID") && row["ID"] != DBNull.Value && Convert.ToInt32(row["ID"]) > 0;
        }
        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            DataRow row = gridView1.GetFocusedDataRow();
            if (row == null) return;

            string fieldName = gridView1.FocusedColumn?.FieldName;
            if (fieldName == null) return;

            bool isSaved = row.Table.Columns.Contains("ID") && row["ID"] != DBNull.Value && Convert.ToInt32(row["ID"]) > 0;

            if (fieldName == "ChiTiet")
            {
                string maVTID = row["MaVTID"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(maVTID))
                {
                    e.Cancel = true;
                }
            }
            if (isSaved)
            {
                var blockedColumns = new[] { "ChungLoaiVatTu", "MaVT", "ChiTiet", "MauVT", "MaMauVT", "KhoVai", "TenDVVT", "MaDVVT" };
                if (blockedColumns.Contains(fieldName))
                {
                    e.Cancel = true;
                }
            }
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "MaVT")
            {
                DataRow row = gridView1.GetFocusedDataRow();
                if (row == null) return;
                row["MaVTID"] = "";
                row["ChiTiet"] = "";
                gridView1.RefreshData();
            }
        }
    }
}
