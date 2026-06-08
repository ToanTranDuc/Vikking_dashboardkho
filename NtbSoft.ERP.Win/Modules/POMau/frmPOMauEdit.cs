using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
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
    public partial class frmPOMauEdit : Form
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private DataTable copiedData;
        string _maKH, _maHang, _maPhieu = string.Empty;
        private Image _originalImage = null;
        private float _zoomFactor = 1.0f;
        private Point _panOffset = Point.Empty;
        private Point _lastMousePos = Point.Empty;
        private bool _isPanning = false;
        private Image _sourceImage = null;
        public frmPOMauEdit(string maKH = "", string maHang = "", string maPhieu = "")
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
            loaddata();
            CheckDuyet();
            SetupPictureEditZoom();
        }
        private void SetupPictureEditZoom()
        {
            pictureEdit1.MouseWheel += (s, e) =>
            {
                if (pictureEdit1.Image == null) return;
                float oldZoom = _zoomFactor;
                if (e.Delta > 0) _zoomFactor = Math.Min(_zoomFactor * 1.15f, 10f);
                else _zoomFactor = Math.Max(_zoomFactor / 1.15f, 0.1f);
                RedrawZoomedImage();
            };

            pictureEdit1.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Middle)
                {
                    _zoomFactor = 1f;
                    _panOffset = Point.Empty;
                    pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
                    RedrawZoomedImage();
                }
                if (e.Button == MouseButtons.Left)
                {
                    _isPanning = true;
                    _lastMousePos = e.Location;
                    pictureEdit1.Cursor = Cursors.Hand;
                }
            };
            pictureEdit1.MouseMove += (s, e) =>
            {
                if (!_isPanning) return;
                _panOffset.X += e.X - _lastMousePos.X;
                _panOffset.Y += e.Y - _lastMousePos.Y;
                _lastMousePos = e.Location;
                RedrawZoomedImage();
            };

            pictureEdit1.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Left) { _isPanning = false; pictureEdit1.Cursor = Cursors.Default; }
            };
        }

        private void RedrawZoomedImage()
        {
            if (_sourceImage == null) return;

            int w = pictureEdit1.Width;
            int h = pictureEdit1.Height;
            float drawZoom = _zoomFactor == 1f
                ? Math.Min((float)w / _sourceImage.Width, (float)h / _sourceImage.Height)
                : _zoomFactor;

            int newW = (int)(_sourceImage.Width * drawZoom);
            int newH = (int)(_sourceImage.Height * drawZoom);

            Bitmap bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                int x = (w - newW) / 2 + _panOffset.X;
                int y = (h - newH) / 2 + _panOffset.Y;
                g.DrawImage(_sourceImage, x, y, newW, newH);
            }
            pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
            pictureEdit1.Image = bmp;
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
        private void loaddata()
        {
            if (string.IsNullOrEmpty(_maPhieu)) return;

            string urlID = $"{URL}POMau/Get?action=GetThongTin&para1={_maPhieu}";
            string jsonID = Task.Run(async () => await _clientExtension.GetAsnyc(urlID)).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonID);
            if (tbl == null || tbl.Rows.Count == 0) return;
            try
            {
                string relativePath = "";

                if (tbl.Columns.Contains("HinhAnh") && tbl.Rows.Count > 0)
                {
                    object val = tbl.Rows[0]["HinhAnh"];

                    if (val != DBNull.Value && val != null)
                        relativePath = val.ToString();
                }

                //if (!string.IsNullOrWhiteSpace(relativePath))
                //{
                //    // =========================
                //    // FIX URL ẢNH
                //    // =========================
                //    string baseUrl = URL.Replace("api/", ""); // bỏ api nếu có

                //    string fullUrl = baseUrl + relativePath;

                //    // debug
                //    System.Diagnostics.Debug.WriteLine("IMG URL: " + fullUrl);

                //    pictureEdit1.LoadAsync(fullUrl);
                //    pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;


                //    _fileName = Path.GetFileName(relativePath);
                //}
                if (!string.IsNullOrWhiteSpace(relativePath))
                {
                    string baseUrl = URL.Replace("api/", "");
                    string fullUrl = baseUrl + relativePath;
                    _fileName = Path.GetFileName(relativePath);

                    // Load ảnh async để lấy _sourceImage
                    Task.Run(async () =>
                    {
                        try
                        {
                            using (var client = new HttpClient())
                            {
                                byte[] bytes = await client.GetByteArrayAsync(fullUrl);
                                using (var ms = new MemoryStream(bytes))
                                {
                                    Image img = new Bitmap(Image.FromStream(ms));
                                    this.Invoke(new Action(() =>
                                    {
                                        _sourceImage = img;
                                        _originalImage = img;
                                        _zoomFactor = 1f;
                                        _panOffset = Point.Empty;
                                        RedrawZoomedImage();
                                    }));
                                }
                            }
                        }
                        catch { }
                    });
                }
                else
                {
                    pictureEdit1.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Load ảnh lỗi: " + ex.Message);
            }
            string urlCT = $"{URL}POMau/Get?action=GetCT&para1={_maPhieu}";
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
             if (!tblCT.Columns.Contains("HinhAnh"))
                tblCT.Columns.Add("HinhAnh", typeof(string));
            foreach (DataRow row in tblCT.Rows)
            {
                bool isNPL = row["IsNPL"] != null && row["IsNPL"] != DBNull.Value
                             && Convert.ToBoolean(row["IsNPL"]);
                row["LoaiVT"] = isNPL ? "Nguyên liệu" : "Phụ liệu";
            }
            gridControl1.DataSource = tblCT;
      
     
            gridView1.RefreshData();
            gridView1.ExpandAllGroups();
        }
        private void CheckDuyet()
        {
            DataRow dr = gridView1.GetFocusedDataRow();
            if (dr == null) return;
            string maPhieu = dr["MaPhieu"].ToString();
            string url = $"{URL}POMau/Get?action=GetDuyet&para1={maPhieu}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            if (!string.IsNullOrEmpty(json))
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    int duyettao = 0;
                    int.TryParse(tbl.Rows[0]["DuyetTao"]?.ToString(), out duyettao);

                    if (duyettao == 1 && gridView1.DataSource != null)
                    {
                        gridView1.OptionsBehavior.Editable = false;
                        textEditForecastqty.ReadOnly = true;
                        textBoxColorways.ReadOnly = true;
                        textBoxPosition.ReadOnly = true;
                        searchLookUpEditQuotainsize.ReadOnly = true;
                        layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }

                }
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
                    tbl.Rows.Add(_nr);
                }
                gridControl1.DataSource = tbl;
            }
        }

        private DataTable createTableGridControl()
        {
            DataTable tbl = new DataTable("tbl");
            tbl.Columns.Add("ID", typeof(int));
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
            tbl.Columns.Add("DinhMuc", typeof(string));
            tbl.Columns.Add("HaoHut", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("LoaiVT", typeof(string));
            tbl.Columns.Add("IsNPL", typeof(bool));
            tbl.Columns.Add("Position", typeof(string));
            tbl.Columns.Add("HinhAnh", typeof(byte[]));
            tbl.Columns.Add("TenFile", typeof(string));
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
            _zoomFactor = 1f;
            _panOffset = Point.Empty;
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
            _dr["GhiChu"] = textBoxGhiChu.Text?.Trim() ?? "";
            string relativePath = string.IsNullOrEmpty(_fileName)
                ? ""
                : "Content/Image/POMau/" + _fileName;

            _dr["HinhAnh"] = relativePath;
            _dr["TenFile"] = _fileName ?? "";
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
                _drow["DinhMuc"] = dr["DinhMuc"] ?? 0;
                _drow["HaoHut"] = dr["HaoHut"] ?? 0;
                _drow["GhiChu"] = dr["GhiChu"]?.ToString() ?? null;
                _drow["DonGia"] = 0;
                _drow["Position"] = dr["Position"]?.ToString() ?? null;
                dtSaveCT.Rows.Add(_drow);
            }
            string url = $"{URL}POMau/Post?Action=Post&para1={GlobleData.UserName}";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (msResult.ToLower() == "true")
            {
                string urlCT = $"{URL}POMau/PostCT?Action=PostCT&para1={GlobleData.UserName}";
                string msResultCT = Task.Run(async () => { return await _clientExtension.PostAsync(urlCT, dtSaveCT); }).Result;
                if (msResultCT.ToLower() == "true")
                {
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    this.Close();
                }
            }
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
                dr["HaoHut"] = 0;
                dr["DinhMuc"] = 0;
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
            Console.WriteLine(listIsNPL + " - " + isNPL);
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

            if(fieldName == "ChiTiet")
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

        //Minh Thong
        // ======================
        // IMAGE STATE
        // ======================
        private byte[] _imageBytes = null;
        private string _fileName = "";
        private byte[] ResizeImage(Image img, int targetWidth = 800, int targetHeight = 800)
        {
            double ratioX = (double)targetWidth / img.Width;
            double ratioY = (double)targetHeight / img.Height;
            double ratio = Math.Max(ratioX, ratioY);

            int newWidth = (int)(img.Width * ratio);
            int newHeight = (int)(img.Height * ratio);

            using (Bitmap bmp = new Bitmap(targetWidth, targetHeight))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                int posX = (targetWidth - newWidth) / 2;
                int posY = (targetHeight - newHeight) / 2;

                g.DrawImage(img, posX, posY, newWidth, newHeight);

                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return ms.ToArray();
                }
            }
        }

        private byte[] ResizeImageToFitControl(Image img)
        {
            int targetWidth = pictureEdit1.Width;
            int targetHeight = pictureEdit1.Height;

            double ratioX = (double)targetWidth / img.Width;
            double ratioY = (double)targetHeight / img.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(img.Width * ratio);
            int newHeight = (int)(img.Height * ratio);

            using (Bitmap bmp = new Bitmap(targetWidth, targetHeight))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                int posX = (targetWidth - newWidth) / 2;
                int posY = (targetHeight - newHeight) / 2;

                g.DrawImage(img, posX, posY, newWidth, newHeight);

                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return ms.ToArray();
                }
            }
        }

        private async void pictureEdit1_DoubleClick(object sender, EventArgs e)
        {
            Image imgToShow = null;

            if (_originalImage != null)
                imgToShow = _originalImage;
            else if (!string.IsNullOrEmpty(_fileName))
            {
                try
                {
                    string imageUrl = URL + "Content/Image/POMau/" + _fileName;
                    using (HttpClient client = new HttpClient())
                    {
                        byte[] bytes = await client.GetByteArrayAsync(imageUrl);
                        using (MemoryStream ms = new MemoryStream(bytes))
                            imgToShow = new Bitmap(Image.FromStream(ms));
                    }
                }
                catch { imgToShow = pictureEdit1.Image; }
            }
            else imgToShow = pictureEdit1.Image;

            if (imgToShow == null) return;

            Form frmView = new Form();
            frmView.Text = "Xem hình (scroll=zoom, kéo=di chuyển, giữa=reset)";
            frmView.StartPosition = FormStartPosition.CenterScreen;
            frmView.Size = new Size(900, 700);
            frmView.BackColor = Color.DarkGray;
            frmView.KeyPreview = true;
            frmView.KeyDown += (s, ke) => { if (ke.KeyCode == Keys.Escape) frmView.Close(); };

            PictureBox pb = new PictureBox();
            pb.Dock = DockStyle.Fill;
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = imgToShow;
            pb.BackColor = Color.DarkGray;
            frmView.Controls.Add(pb);

            // --- ZOOM STATE ---
            float zoom = 1f;
            Point offset = Point.Empty;
            Point lastMouse = Point.Empty;
            bool panning = false;

            Action redraw = () =>
            {
                int cw = pb.ClientSize.Width, ch = pb.ClientSize.Height;
                Bitmap bmp = new Bitmap(cw, ch);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.FromArgb(50, 50, 50));
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    int nw = (int)(imgToShow.Width * zoom);
                    int nh = (int)(imgToShow.Height * zoom);
                    int x = (cw - nw) / 2 + offset.X;
                    int y = (ch - nh) / 2 + offset.Y;
                    g.DrawImage(imgToShow, x, y, nw, nh);
                }
                pb.Image = bmp;
            };

            pb.MouseWheel += (s, me) =>
            {
                if (me.Delta > 0) zoom = Math.Min(zoom * 1.15f, 20f);
                else zoom = Math.Max(zoom / 1.15f, 0.05f);
                redraw();
            };

            pb.MouseDown += (s, me) =>
            {
                if (me.Button == MouseButtons.Middle) { zoom = 1f; offset = Point.Empty; redraw(); }
                if (me.Button == MouseButtons.Left) { panning = true; lastMouse = me.Location; pb.Cursor = Cursors.Hand; }
            };

            pb.MouseMove += (s, me) =>
            {
                if (!panning) return;
                offset.X += me.X - lastMouse.X;
                offset.Y += me.Y - lastMouse.Y;
                lastMouse = me.Location;
                redraw();
            };

            pb.MouseUp += (s, me) => { panning = false; pb.Cursor = Cursors.Default; };

            pb.Resize += (s, re) => redraw();
            pb.DoubleClick += (s, ev) => frmView.Close();

            frmView.Shown += (s, se) => redraw();
            frmView.Show();
        }
        private async void btnAddPicture_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    FileInfo file = new FileInfo(ofd.FileName);

                    // 🔥 CHECK SIZE
                    if (file.Length > 50 * 1024 * 1024)
                    {
                        XtraMessageBox.Show("Ảnh vượt quá 50MB!");
                        return;
                    }

                    // ======================
                    // 1. LOAD + RESIZE
                    // ======================
                    byte[] originalBytes = File.ReadAllBytes(ofd.FileName);
                    using (Image original = Image.FromFile(ofd.FileName))
                    {
                        _originalImage = new Bitmap(original);
                        _imageBytes = ResizeImageToFitControl(original);
                    }
                    using (Image original = Image.FromFile(ofd.FileName))
                    {
                        _originalImage = new Bitmap(original);
                        _sourceImage = new Bitmap(original);
                        _imageBytes = ResizeImageToFitControl(original);
                    }
                    _zoomFactor = 1f;
                    _panOffset = Point.Empty;
                    RedrawZoomedImage();

                    // ======================
                    // 2. HIỂN THỊ ẢNH
                    // ======================
                    //using (MemoryStream ms = new MemoryStream(_imageBytes))
                    //{
                    //    pictureEdit1.EditValue = Image.FromStream(ms);
                    //}

                    //pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;

                    // ======================
                    // 3. UPLOAD SERVER
                    // ======================
                    string uploadedFileName = await UploadImageResized(originalBytes);

                    if (string.IsNullOrEmpty(uploadedFileName))
                    {
                        XtraMessageBox.Show("Upload ảnh thất bại!");
                        return;
                    }

                    // ======================
                    // 4. LƯU BIẾN TẠM
                    // ======================
                    _fileName = uploadedFileName;

                    // ======================
                    // DEBUG
                    // ======================
                    System.Diagnostics.Debug.WriteLine("IMAGE SIZE: " + _imageBytes.Length);
                    System.Diagnostics.Debug.WriteLine("FILE NAME: " + _fileName);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Lỗi ảnh: " + ex.Message);
                }
            }
        }


        private async Task<string> UploadImageResized(byte[] imageBytes)
        {
            using (var client = new HttpClient())
            using (var content = new MultipartFormDataContent())
            {
                var fileContent = new ByteArrayContent(imageBytes);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                content.Add(fileContent, "file", "upload.jpg");

                var response = await client.PostAsync($"{URL}POMau/UploadImage", content);

                if (!response.IsSuccessStatusCode)
                    return null;

                string fileName = await response.Content.ReadAsStringAsync();

                return fileName.Replace("\"", ""); // remove quote nếu có
            }

        }

    }
}
