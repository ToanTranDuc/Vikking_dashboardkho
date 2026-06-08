using DevExpress.XtraEditors;
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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Utils;
using DevExpress.XtraGrid.Views.Grid;
using System.Globalization;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPVatTu : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tblVatTu;
        public frmERPVatTu()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            tblVatTu = new DataTable();
            this.ActiveControl = button1;
        }
        protected override void OnLoad(EventArgs e)
        {
            CreateSearchLookUpEditLocKH();
            CreateSearchLookup();
            CreateTableVatTu();
            loadVatTu();
            this.ActiveControl = button1;

        }
        private void CreateTableVatTu()
        {
            tblVatTu = new DataTable("tblVatTu");
            tblVatTu.Columns.Add("ID", typeof(int));
            tblVatTu.Columns.Add("MaVTID", typeof(string));
            tblVatTu.Columns.Add("MaVT", typeof(string));
            tblVatTu.Columns.Add("TenVT", typeof(string));
            tblVatTu.Columns.Add("ChiTiet", typeof(string));
        }
        private void loadVatTu()
        {
            string url = string.Format("{0}", URL + $"ERPThongSoVatTu/GetVatTu");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                gCVatTu.DataSource = null;
                return;
            }
            tblVatTu = JsonConvert.DeserializeObject<DataTable>(json);
            if (tblVatTu == null || tblVatTu.Rows.Count == 0)
            {
                gCVatTu.DataSource = null;
                return;
            } 
            gCVatTu.DataSource = tblVatTu;

        }

        private void gVVatTu_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void NapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CreateSearchLookup();
            loadVatTu();
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataRow dr = gVVatTu.GetFocusedDataRow();
            if (dr == null) return;
            DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (messResult == DialogResult.Yes)
            {
                string url = string.Format("{0}?id={1}", URL + "ERPThongSoVatTu/DeleteVatTu", dr["ID"].ToString());
                string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                loadVatTu();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaVT.Text) && string.IsNullOrWhiteSpace(txtChiTiet.Text)) return;
            string[] arrMaVT = txtMaVT.Text.Trim().Split('|');
            string[] arrChiTiet=txtChiTiet.Text.Trim().Split('|');
            if(txtMaVT.Text==""||txtChiTiet.Text=="")
            {
                XtraMessageBox.Show(" Vui lòng điền đủ thông tin vật tư!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }    
            if(arrMaVT.Length!=arrChiTiet.Length)
            {
                XtraMessageBox.Show(" Vui lòng điền đủ thông tin vật tư!!!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }    
           
            for(int i=0;i<arrMaVT.Length;i++)
            {
                DataRow dr = tblVatTu.NewRow();
                dr["ID"] = 0;
                dr["MaVTID"] = "";
                dr["MaVT"] = arrMaVT[i];
                dr["TenVT"] = "";
                dr["ChiTiet"] = arrChiTiet[i];
                dr["MaHang"] = searchLookUpEditMaHang.EditValue.ToString();
                dr["MaKH"] = searchLookUpEditLocKH.EditValue.ToString();
                dr["MaDVVT"] = searchLookUpEditDVVT.EditValue.ToString();
                dr["MaNhom"] = searchLookUpEditNhom.EditValue.ToString();
                tblVatTu.Rows.InsertAt(dr,0);
            }
            gCVatTu.DataSource = tblVatTu;
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.ActiveControl = button1;
                var duplicates = GetDuplicateMaVTWithRows(tblVatTu);

                if (duplicates.Any())
                {
                    string message = "Có mã vật tư bị trùng:\n";
                    foreach (var item in duplicates)
                    {
                        message += $"- Mã VT: {item.Key} (Dòng: {string.Join(" và ", item.Value)})\n";
                    }

                    XtraMessageBox.Show(message, "Thông báo", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    return;
                }
                clsWaitForm.ShowWaitForm(this, 2000);
                string url = string.Format("{0}", URL + "ERPThongSoVatTu/PostVatTu");

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblVatTu); }).Result;
                if (msResult.ToLower() == "true")
                {

                    loadVatTu();
                    clsWaitForm.ShowSuccessForm(this, 2000);

                }
            }
            catch (Exception ex)
            {

            }
        }
        private Dictionary<string, List<int>> GetDuplicateMaVTWithRows(DataTable table)
        {
            return table.AsEnumerable()
                .GroupBy(row => row.Field<string>("MaVT"))
                .Where(group => group.Count() > 1)  // Chỉ lấy nhóm có nhiều hơn 1 phần tử (trùng)
                .ToDictionary(group => group.Key, group => group.Select(row => table.Rows.IndexOf(row) + 1).ToList());
        }

        private void gVVatTu_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }

        private void txtMaVT_Properties_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar)) // Chỉ xử lý nếu là chữ cái
            {
                string convertedChar = RemoveDiacritics(e.KeyChar.ToString());
                if (!string.IsNullOrEmpty(convertedChar))
                {
                    e.Handled = true; // Ngăn chặn ký tự gốc có dấu
                    txtMaVT.Text += convertedChar; // Thêm ký tự không dấu
                    txtMaVT.SelectionStart = txtMaVT.Text.Length; // Đặt con trỏ về cuối
                }
            }
        }

        private string RemoveDiacritics(string text)
        {
            return string.Concat(text.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
                .Normalize(NormalizationForm.FormC);
        }

        private void gVVatTu_ShownEditor(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            if (view.FocusedColumn.FieldName == "MaVT") // Chỉ áp dụng cho cột "MaVT"
            {
                TextEdit editor = view.ActiveEditor as TextEdit;
                if (editor != null)
                {
                    // Xử lý khi người dùng nhập ký tự
                    editor.KeyPress += (s, ke) =>
                    {
                        if (char.IsLetter(ke.KeyChar)) // Kiểm tra nếu là chữ cái
                        {
                            string convertedChar = RemoveDiacritics(ke.KeyChar.ToString());
                            if (!string.IsNullOrEmpty(convertedChar))
                            {
                                ke.Handled = true; // Ngăn chặn ký tự gốc có dấu
                                editor.Text += convertedChar; // Ghi ký tự không dấu vào
                                editor.SelectionStart = editor.Text.Length; // Đưa con trỏ về cuối
                            }
                        }
                    };
                }
            }
        }
        private void RCountryEdit_EditValueChanged(object sender, EventArgs e)
        {
            var rCountryEdit = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (rCountryEdit == null) return;
            DevExpress.XtraGrid.Views.Grid.GridView gridView = (DevExpress.XtraGrid.Views.Grid.GridView)rCountryEdit.Properties.View;
            int focusedRowHandle = gridView.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                DevExpress.XtraGrid.Views.Grid.GridView gridViews = (DevExpress.XtraGrid.Views.Grid.GridView)gCVatTu.MainView;
                DataRow focusedRow = gridView.GetDataRow(focusedRowHandle);
                if (focusedRow != null)
                {
                    gridViews.SetRowCellValue(gridViews.FocusedRowHandle, "MaKH", focusedRow["MaKH"].ToString());
                }
            }
        }

        private void searchLookUpEditLocKH_EditValueChanged(object sender, EventArgs e)
        {
            gVVatTu.ActiveFilterString = string.Empty;
            ApplyFilter();
            CreateSearchLookUpEditLocMH();
            CreateSearchLookUpEditMH();
        }
        private void ApplyFilter()
        {
            // Lấy giá trị từ SearchLookUpEdit
            string selectedKH = searchLookUpEditLocKH.EditValue?.ToString();
            string selectedMH = searchLookUpEditLocMH.EditValue?.ToString();

            // Nếu giá trị là "All", thì bỏ qua bộ lọc cho giá trị đó
            string filterKH = (!string.IsNullOrEmpty(selectedKH) && selectedKH != "All") ? $"[MaKH] = '{selectedKH}'" : "";
            string filterMH = (!string.IsNullOrEmpty(selectedMH) && selectedMH != "All") ? $"[MaHang] = '{selectedMH}'" : "";

            // Kết hợp cả 2 điều kiện nếu đều có giá trị hợp lệ
            if (!string.IsNullOrEmpty(filterKH) && !string.IsNullOrEmpty(filterMH))
            {
                gVVatTu.ActiveFilterString = $"{filterKH} AND {filterMH}";
            }
            else if (!string.IsNullOrEmpty(filterKH))
            {
                gVVatTu.ActiveFilterString = filterKH;
            }
            else if (!string.IsNullOrEmpty(filterMH))
            {
                gVVatTu.ActiveFilterString = filterMH;
            }
            else
            {
                gVVatTu.ActiveFilterString = string.Empty; // Xóa bộ lọc nếu không có điều kiện nào
            }
        }

        private void searchLookUpEditLocMH_EditValueChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void CreateSearchLookUpEditLocMH()
        {
            searchLookUpEditLocMH.Properties.DataSource = null;
            searchLookUpEditLocMH.EditValue = null;
            searchLookUpEditLocMH.Properties.ValueMember = "MaHang";
            searchLookUpEditLocMH.Properties.DisplayMember = "MaHang";

            string khachhang = searchLookUpEditLocKH.EditValue?.ToString() ?? "All";
            string url2 = $"{URL}KhaiBaoAll/Get?action=GetHangHoaBangMau&para1={khachhang}";
            string json2 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
            DataTable tbl2 = JsonConvert.DeserializeObject<DataTable>(json2);

            searchLookUpEditLocMH.Properties.DataSource = tbl2;
            //searchLookUpEditMH.Properties.DataSource = tbl2;
        }

        private void CreateSearchLookUpEditMH()
        {
            searchLookUpEditMaHang.Properties.ValueMember = "MaHang";
            searchLookUpEditMaHang.Properties.DisplayMember = "MaHang";

            string khachhang = searchLookUpEditLocKH.EditValue?.ToString() ?? "All";

            string url2 = $"{URL}ERPThongSoVatTu/GetMHVT?&makh={khachhang}";
            string json2 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url2); }).Result;
            DataTable tbl2 = JsonConvert.DeserializeObject<DataTable>(json2);
            searchLookUpEditMaHang.Properties.DataSource = tbl2;
        }
        private void CreateSearchLookUpEditLocKH()
        {
            searchLookUpEditLocKH.Properties.ValueMember = "MaKH";
            searchLookUpEditLocKH.Properties.DisplayMember = "TenKH";

            string url = $"{URL}KhaiBaoAll/Get?action=GetKHBangSize";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            searchLookUpEditLocKH.Properties.DataSource = tbl;

        }

        private void CreateSearchLookup()
        {
            string url = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
            rCountryEdit.DataSource = tbl;
            rCountryEdit.DisplayMember = "TenHang";
            rCountryEdit.ValueMember = "MaHang";
            rCountryEdit.ShowClearButton = false;
            rCountryEdit.NullText = "[Chọn giá trị]";

            GridView dvView = rCountryEdit.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaHang", Caption = "Mã hàng", Name = "colMaHang", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenHang", Caption = "Tên hàng", Name = "colTenHang", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "MaKH", Caption = "Mã khách hàng", Name = "colMaKH", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Khách hàng", Name = "colTenKH", Visible = true });

            }
            colMaHang.ColumnEdit = rCountryEdit;
            rCountryEdit.EditValueChanged += RCountryEdit_EditValueChanged;

            //searchLookUpEditMaHang.Properties.DataSource = tbl;

            /// kh
            string urlkh = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string jsonkh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlkh); }).Result;
            DataTable tblkh = JsonConvert.DeserializeObject<DataTable>(jsonkh);
            RepositoryItemSearchLookUpEdit rCountryEditkh = new RepositoryItemSearchLookUpEdit();
            rCountryEditkh.DataSource = tblkh;
            rCountryEditkh.DisplayMember = "TenKH";
            rCountryEditkh.ValueMember = "MaKH";
            rCountryEditkh.ShowClearButton = false;
            rCountryEditkh.NullText = "[Chọn giá trị]";

            GridView dvViewkh = rCountryEditkh.View;
            if (dvViewkh.Columns.Count == 0)
            {
                dvViewkh.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewkh.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewkh.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewkh.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewkh.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Tên khách hàng", Name = "colMaHang", Visible = true });

            }
            colMaKH.ColumnEdit = rCountryEditkh;
            //// dv
            string urldv = string.Format("{0}?", URL + "ERPThuVienVT/GetDVVT");
            string jsondv = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldv); }).Result;
            DataTable tbldv = JsonConvert.DeserializeObject<DataTable>(jsondv);
            RepositoryItemSearchLookUpEdit rCountryEditdv = new RepositoryItemSearchLookUpEdit();
            rCountryEditdv.DataSource = tbldv;
            rCountryEditdv.DisplayMember = "TenDVVT";
            rCountryEditdv.ValueMember = "MaDVVT";
            rCountryEditdv.ShowClearButton = false;
            rCountryEditdv.NullText = "[Chọn giá trị]";

            GridView dvViewdv = rCountryEditdv.View;
            if (dvViewdv.Columns.Count == 0)
            {
                dvViewdv.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewdv.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewdv.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewdv.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewdv.Columns.Add(new GridColumn { FieldName = "TenDVVT", Caption = "Tên Đơn Vị Vật Tư", Name = "colMaDVVT", Visible = true });

            }
            colMaDVVT.ColumnEdit = rCountryEditdv;
            searchLookUpEditDVVT.Properties.DataSource = tbldv;
            ///nhom
            string urln = string.Format("{0}?", URL + "NhomNPL/Get");
            string jsonn = Task.Run(async () => { return await _clientExtension.GetAsnyc(urln); }).Result;
            DataTable tbln = JsonConvert.DeserializeObject<DataTable>(jsonn);
            RepositoryItemSearchLookUpEdit rCountryEditn = new RepositoryItemSearchLookUpEdit();
            rCountryEditn.DataSource = tbln;
            rCountryEditn.DisplayMember = "TenNhom";
            rCountryEditn.ValueMember = "MaNhom";
            rCountryEditn.ShowClearButton = false;
            rCountryEditn.NullText = "[Chọn giá trị]";

            GridView dvViewn = rCountryEditn.View;
            if (dvViewn.Columns.Count == 0)
            {
                dvViewn.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewn.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewn.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewn.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewn.Columns.Add(new GridColumn { FieldName = "TenNhom", Caption = "Tên Nhóm", Name = "colMaNhom", Visible = true });

            }
            colMaNhom.ColumnEdit = rCountryEditn;

            searchLookUpEditNhom.Properties.DataSource = tbln;
        }
        string _maKH = "";
        private void searchLookUpEditMaHang_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gridView = (DevExpress.XtraGrid.Views.Grid.GridView)searchLookUpEditMaHang.Properties.View;
            int focusedRowHandle = gridView.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                DataRow focusedRow = gridView.GetDataRow(focusedRowHandle);
                if (focusedRow != null)
                {
                    _maKH = focusedRow["MaKH"].ToString();
                }
            }
        }
    }
}