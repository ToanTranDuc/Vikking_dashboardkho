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
    public partial class frmPOMauSMV : Form
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
        private DataTable _nccTable;
        public frmPOMauSMV(string maKH = "", string maHang = "", string maPhieu = "")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _maKH = maKH ?? "";
            _maHang = maHang ?? "";
            _maPhieu = maPhieu ?? "";
        }
        private string GeneratePhieuNumber(int sophieu)
        {
            currentPhieuNumber = currentPhieuNumber + sophieu;
            formattedPhieuNumber = "PHIEU_" + currentPhieuNumber;//SoDonHang.ToString("D8");
            return formattedPhieuNumber;
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
            loaddata();
            CreateSearchLookUpNCC();
        }
        private void loaddata()
        {
            if (string.IsNullOrEmpty(_maPhieu)) return;

            string urlID = $"{URL}POMau/Get?action=GetChiTiet&para1={_maPhieu}";
            string jsonID = Task.Run(async () => await _clientExtension.GetAsnyc(urlID)).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonID);

            if (tbl == null || tbl.Rows.Count == 0) return;

            if (!tbl.Columns.Contains("MaNCC"))
                tbl.Columns.Add("MaNCC", typeof(string));
            if (!tbl.Columns.Contains("TenNCC"))
                tbl.Columns.Add("TenNCC", typeof(string));

            foreach (DataRow row in tbl.Rows)
            {
                if (row["MaNCC"] == DBNull.Value || row["MaNCC"] == null)
                    row["MaNCC"] = "";
                if (tbl.Columns.Contains("MaCLVT") && tbl.Columns.Contains("ChungLoaiVatTu"))
                    row["ChungLoaiVatTu"] = row["MaCLVT"]?.ToString() ?? "";
            }

            DataRow firstRow = tbl.Rows[0];
            textBoxPosition.Text = firstRow["Position"]?.ToString() ?? "";

            textEditForecastqty.Text = firstRow["SoLuong"]?.ToString() ?? "0";
            textBoxSizeRatio.Text = firstRow["TyLeKichThuoc"]?.ToString() ?? "";
            textBoxColorways.Text = firstRow["MauSac"]?.ToString() ?? "";

            if (tbl.Columns.Contains("KichThuocBaoGia"))
                searchLookUpEditQuotainsize.EditValue = firstRow["KichThuocBaoGia"]?.ToString() ?? "";
            txtTotalCPSPHP.Text = firstRow["TotalCPSPHP"]?.ToString() ?? "0";
            txtTotalHCost.Text = firstRow["TotalHCost"]?.ToString() ?? "0";
            txtManuCost.Text = firstRow["ManuCost"]?.ToString() ?? "";
            txtProfCM.Text = firstRow["ProfitfCM"]?.ToString() ?? "";
            txtCSpeTool.Text = firstRow["CostSpeTool"]?.ToString() ?? "";
            txtIECost.Text = firstRow["IECost"]?.ToString() ?? "";
            txtCGMSP.Text = firstRow["CostGMSP"]?.ToString() ?? "";
            txtC3P.Text = firstRow["Cost3P"]?.ToString() ?? "";
            if (!tbl.Columns.Contains("MaNCC")) tbl.Columns.Add("MaNCC", typeof(string));
            if (!tbl.Columns.Contains("TenNCC")) tbl.Columns.Add("TenNCC", typeof(string));
            foreach (DataRow row in tbl.Rows)
            {
                if (row["MaNCC"] == DBNull.Value || row["MaNCC"] == null)
                    row["MaNCC"] = "";

                if (tbl.Columns.Contains("MaCLVT") && tbl.Columns.Contains("ChungLoaiVatTu"))
                    row["ChungLoaiVatTu"] = row["MaCLVT"]?.ToString() ?? "";
            }
            gridControl1.DataSource = tbl;
            gridView1.RefreshData();
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
                    _nr["DinhMucKH"] = 0;
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
            dtSave.Columns.Add("MaNhom", typeof(string));
            dtSave.Columns.Add("MaVTID", typeof(string));
            dtSave.Columns.Add("MauVTID", typeof(string));
            dtSave.Columns.Add("KhoVaiID", typeof(string));
            dtSave.Columns.Add("MaDVVT", typeof(string));
            dtSave.Columns.Add("MaNCC", typeof(string));
            dtSave.Columns.Add("DinhMuc", typeof(float));
            dtSave.Columns.Add("HaoHut", typeof(float));
            dtSave.Columns.Add("SMV", typeof(float));
            dtSave.Columns.Add("SoLuong", typeof(int));
            dtSave.Columns.Add("TyLeKichThuoc", typeof(string));
            dtSave.Columns.Add("MauSac", typeof(string));
            dtSave.Columns.Add("KichThuocBaoGia", typeof(string));
            dtSave.Columns.Add("GhiChu", typeof(float));
            foreach (DataRow dr in tbl.Rows)
            {
                DataRow _dr = dtSave.NewRow();
                _dr["MaPhieu"] = formattedPhieuNumber;
                _dr["STT"] = currentPhieuNumber;
                _dr["MaKH"] = searchLookUpEditKH.EditValue.ToString();
                _dr["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                _dr["MaNhom"] = dr["MaCLVT"];
                _dr["MaVTID"] = dr["MaVTID"];
                _dr["MaVT"] = dr["MaVT"];
                _dr["ChiTiet"] = dr["ChiTiet"];
                _dr["MauVTID"] = dr["MauVTID"];
                _dr["MaMauVT"] = dr["MaMauVT"];
                _dr["MauVT"] = dr["MauVT"];
                _dr["KhoVaiID"] = dr["KhoVaiID"];
                _dr["KhoVai"] = dr["KhoVai"];
                _dr["TenDVKV"] = dr["TenDVKV"];
                _dr["MaDVVT"] = dr["MaDVVT"];
                _dr["TenDVVT"] = dr["TenDVVT"];
                _dr["DinhMuc"] = dr["DinhMucKH"];
                _dr["HaoHut"] = dr["HaoHut"];
                _dr["SMV"] = 0;
                _dr["SoLuong"] = string.IsNullOrWhiteSpace(textEditForecastqty.Text) ? 0 : int.Parse(textEditForecastqty.Text.Trim());
                _dr["TyLeKichThuoc"] = textBoxSizeRatio.Text.Trim();
                _dr["MauSac"] = textBoxColorways.Text.Trim();
                _dr["KichThuocBaoGia"] = searchLookUpEditQuotainsize.EditValue.ToString().Trim();
                _dr["GhiChu"] = dr["GhiChu"].ToString();
                _dr["Position"] = textBoxPosition.Text.ToString();
                _dr["TotalCPSPHP"] = string.IsNullOrWhiteSpace(txtTotalCPSPHP.Text) ? 0 : float.Parse(txtTotalCPSPHP.Text.Trim());
                _dr["TotalHCost"] = string.IsNullOrWhiteSpace(txtTotalHCost.Text) ? 0 : float.Parse(txtTotalHCost.Text.Trim()); ToString();
                _dr["ManuCost"] = txtManuCost.Text.ToString();
                _dr["ProfitfCM"] = txtProfCM.Text.ToString();
                _dr["CostSpeTool"] = txtCSpeTool.Text.ToString();
                _dr["IECost"] = txtIECost.Text.ToString();
                _dr["CostGMSP"] = txtCGMSP.Text.ToString();
                _dr["Cost3P"] = txtC3P.Text.ToString();
                dtSave.Rows.Add(_dr);
            }
            string url = $"{URL}POMau/Post?Action=Post&para1={GlobleData.UserName}";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                this.Close();
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

                repositoryItemSearchLookUpEditNCC.DataSource = _nccTable;
                repositoryItemSearchLookUpEditNCC.ValueMember = "MaNCC";
                repositoryItemSearchLookUpEditNCC.DisplayMember = "TenNCC";
                repositoryItemSearchLookUpEditNCC.NullText = "";

                var view = repositoryItemSearchLookUpEditNCC.View;
                view.Columns.Clear();
                repositoryItemSearchLookUpEditNCC.PopulateViewColumns();

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

                // Khi mở popup: load lại NCC theo chủng loại vật tư của dòng đang focus
                repositoryItemSearchLookUpEditNCC.QueryPopUp += (s, e) =>
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

                // Gán repository vào cột MaNCC của grid
                GridColumn colMaNCC = gridView1.Columns["MaNCC"];
                if (colMaNCC != null)
                {
                    colMaNCC.ColumnEdit = repositoryItemSearchLookUpEditNCC;
                    colMaNCC.OptionsColumn.AllowEdit = true;
                    colMaNCC.Visible = true;
                    colMaNCC.Caption = "Nhà cung cấp";
                }
            }
            catch { }
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
                finally
                {
                    gridView1.EndDataUpdate();
                }
            };

            _contextMenuNCC.Items.Add(mnuFillAll);
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
            try
            {
                if (e.Column == null) return;
                Rectangle rect = e.Bounds;
                ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
                Brush brush = e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 192, 128), Color.FromArgb(255, 192, 128), e.Column.AppearanceHeader.GradientMode);
                rect.Inflate(-1, -1);
                e.Graphics.FillRectangle(brush, rect);

                // Thiết lập font và màu chữ
                Font font = new Font(e.Appearance.Font, FontStyle.Bold);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                SolidBrush textColorBrush = new SolidBrush(Color.FromArgb(0, 17, 102)); // Màu chữ #001166

                // Vẽ chữ
                e.Graphics.DrawString(e.Info.Caption, font, textColorBrush, e.Info.CaptionRect, stringFormat);

                foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                {
                    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

            }
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
                    e.Allow = false;
                    _contextMenuNCC.Show(Cursor.Position);
                    return;
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
                    //MessageBox.Show($"Đã xảy ra lỗi khi dán dữ liệu vào cột '{view.VisibleColumns[columnIndex + i].FieldName}': {ex.Message}",
                    //                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

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

        private void gridView1_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                if (gridView1.FocusedColumn == null)
                    return;
                DataRow row = gridView1.GetFocusedDataRow();
                if (row == null) return;
                if (gridView1.FocusedColumn.FieldName == "ChungLoaiVatTu")
                {

                    gridControl1.BeginInvoke(new Action(() =>
                    {
                        if (gridView1.ActiveEditor is SearchLookUpEdit searchLookUpEdit)
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

        private void repoMauVT_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DataRow row = gridView1.GetFocusedDataRow();
            if (row == null) return;
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
        }
    }
}
