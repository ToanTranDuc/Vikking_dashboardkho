using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NtbSoft.ERP.Entity.QuanLyDonHang.ChiTietCayVai_Entity;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmChiTietChonItemCode : Form
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        private string maDonHang = string.Empty;
        private string maLenhSanXuat = string.Empty;

        private string maDH = string.Empty;
        private string maLenhSX = string.Empty;
        private string maGop = string.Empty;
        private string maVTID = string.Empty;
        private string mauVTID = string.Empty;
        private string khoVaiID = string.Empty;
        private string maCLVT = string.Empty;
        private string tenLenhSX = string.Empty;
        private string PO = string.Empty;
        private string Mau = string.Empty;
        private string Line = string.Empty;

        #region binding data
        BindingList<CayVaiPOMua> selectedItems = new BindingList<CayVaiPOMua>();
        #endregion

        #region flag
        bool isLoadingSelection = false;
        #endregion

        public frmChiTietChonItemCode(string _MaVTID, string _MauVTID, string _KhoVaiID, string _MaCLVT, string _MaDH_Send, string _MaLenhSX_Send, string _MaGop, string _TenLenhSX, string _PO, string _Mau, string _Line)
        {
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

            maDH = _MaDH_Send;
            maLenhSX = _MaLenhSX_Send;
            maGop = _MaGop;
            maVTID = _MaVTID;
            mauVTID = _MauVTID;
            khoVaiID = _KhoVaiID;
            maCLVT = _MaCLVT;
            tenLenhSX = _TenLenhSX;

            PO = _PO;
            Mau = _Mau;
            Line = _Line;

            InitializeComponent();
            onStart();
            createFilter();

            this.KeyDown += frmChiTietChonItemCode_KeyDown;

            var gvDetail = gridControlDetailCayVai.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
            if (gvDetail != null)
            {
                gvDetail.KeyDown += GridViewDetail_KeyDown;
            }
        }

        private void frmChiTietChonItemCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                btnSave_ItemClick(null, null);

                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void GridViewDetail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                btnDeleteMany_ItemClick(null, null);

                e.Handled = true;
            }
        }

        protected void onStart()
        {
            gridControlDetailCayVai.DataSource = selectedItems;
            repositoryItemSearchLookUpCayVai.CustomDisplayText += repositoryItemSearchLookUpCayVai_CustomDisplayText;

            barStaticItem1.Caption = tenLenhSX;

            summaryItemC();

            loadDataCanDoiDH();
            loadGridChiTietCanDoi();

            grcCanDoi.ForceInitialize();
            grvCanDoi.Focus();

            FocusDefaultRow();
        }

        protected void summaryItemC()
        {
            var gvDetail = gridControlDetailCayVai.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
            if (gvDetail != null)
            {
                gvDetail.OptionsView.ShowFooter = true;

                var colSoLuong = gvDetail.Columns["SoLuongThucTe"];
                if (colSoLuong != null)
                {
                    colSoLuong.Summary.Clear();
                    colSoLuong.Summary.Add(DevExpress.Data.SummaryItemType.Sum, "SoLuongThucTe", "{0:n2}");

                    colSoLuong.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    colSoLuong.DisplayFormat.FormatString = "n2";
                }
            }
        }

        //protected void onBrand() { 
        //}

        #region create filter
        protected async Task createFilter()
        {
            await filterChonCayVai();
        }

        private async Task filterChonCayVai()
        {
            using (HttpClient client = new HttpClient())
            {
                string urlGetCayVai = URL + $"chitiet/get?action=cayvaitrongkho&para1={maVTID}&para2={mauVTID}&para3={khoVaiID}&para4={maCLVT}";
                var responeCayVai = await client.GetAsync(urlGetCayVai);
                if (responeCayVai.IsSuccessStatusCode)
                {
                    var json = await responeCayVai.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<CayVaiPOMua>>(json);

                    repositoryItemSearchLookUpCayVai.DataSource = data;
                    repositoryItemSearchLookUpCayVai.DisplayMember = "POMua";
                    repositoryItemSearchLookUpCayVai.ValueMember = "POMua";

                    var view = repositoryItemSearchLookUpCayVai.View;
                }
            }    
        }

        private void repositoryItemSearchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (isLoadingSelection) return;

            var viewLookup = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (viewLookup == null) return;

            selectedItems.RaiseListChangedEvents = false;
            selectedItems.Clear();

            int[] selectedRowHandles = viewLookup.GetSelectedRows();
            foreach (int rowHandle in selectedRowHandles)
            {
                if (rowHandle >= 0)
                {
                    var entity = viewLookup.GetRow(rowHandle) as CayVaiPOMua;
                    if (entity != null) selectedItems.Add(entity);
                }
            }
            selectedItems.RaiseListChangedEvents = true;
            selectedItems.ResetBindings();

            string chuoiPOMua = string.Join(", ", selectedItems.Select(x => x.POMua).Distinct());

            DataRow dr = grvCanDoi.GetFocusedDataRow();
            if (dr != null)
            {
                dr["POMua"] = chuoiPOMua;
                dr["IsEdited"] = true;
            }

            if (this.ActiveControl is DevExpress.XtraEditors.SearchLookUpEdit editor)
            {
                editor.Refresh();
            }
            barEditChonCayVai.EditValue = Guid.NewGuid();
        }

        private void repositoryItemSearchLookUpCayVai_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            if (selectedItems != null && selectedItems.Count > 0)
            {
                e.DisplayText = string.Join(", ", selectedItems.Select(x => x.POMua).Distinct());
            }
            else
            {
                e.DisplayText = "Chọn cây vải";
            }
        }
        #endregion

        #region dataGrid
        private void loadDataCanDoiDH()
        {
            string url = $"{URL}ERPDonHangTong/Get?action=GetNPL&para={maDH}&para5={maLenhSX}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (!tbl.Columns.Contains("POMua"))
            {
                tbl.Columns.Add("POMua", typeof(string));
            }

            if (tbl == null || tbl.Rows.Count == 0)
            {
                grcCanDoi.DataSource = null;
                return;
            }
            if (!tbl.Columns.Contains("IsEdited"))
            {
                tbl.Columns.Add("IsEdited", typeof(bool));
                foreach (DataRow r in tbl.Rows)
                    r["IsEdited"] = false;
            }
            if (!tbl.Columns.Contains("IsCapPhatEdited"))
            {
                tbl.Columns.Add("IsCapPhatEdited", typeof(bool));
                foreach (DataRow r in tbl.Rows)
                    r["IsCapPhatEdited"] = false;
            }
            DataTable dtAllSizes = GetBangSize(maGop);
            DataTable dtERPSizes = GetERPSIZESP(maGop);
            tbl = XuLyCotSize(tbl, dtAllSizes, dtERPSizes);
            grcCanDoi.DataSource = tbl;
        }

        public DataTable GetBangSize(string parameter)
        {
            string url = string.Format("{0}?parameter={1}&action=GetBangSize", URL + "CanDoiDonHangTong/GetSize", parameter);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            if (tbl == null || tbl.Rows.Count == 0)
            {
                return null;
            }
            return tbl;
        }

        public DataTable GetERPSIZESP(string parameter)
        {
            string url = string.Format("{0}?parameter={1}&action=GetERPSIZESP", URL + "CanDoiDonHangTong/GetSize", parameter);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

            if (tbl == null || tbl.Rows.Count == 0)
            {
                return null;
            }
            return tbl;
        }

        private DataTable XuLyCotSize(DataTable tblMain, DataTable dtAllSizes, DataTable dtERPSizes)
        {
            if (!tblMain.Columns.Contains("Size"))
            {
                tblMain.Columns.Add("Size", typeof(string));
            }

            // Dictionary chứa danh sách size theo NhomSize (Bảng 1 - BangSize)
            Dictionary<string, List<string>> dictAllSizes = new Dictionary<string, List<string>>();

            if (dtAllSizes != null && dtAllSizes.Rows.Count > 0)
            {
                var groupedAllSizes = dtAllSizes.AsEnumerable()
                    .GroupBy(r => r.Field<string>("NhomSize"))
                    .Select(g => new
                    {
                        NhomSize = g.Key,
                        Sizes = g.Select(r => r.Field<object>("TenSize")?.ToString() ?? "")
                                 .Distinct()
                                 .OrderBy(s => int.TryParse(s, out int num) ? 0 : 1)
                                 .ThenBy(s => int.TryParse(s, out int num) ? num : 0)
                                 .ThenBy(s => s)
                                 .ToList()
                    });

                foreach (var item in groupedAllSizes)
                {
                    dictAllSizes[item.NhomSize] = item.Sizes;
                }
            }
            Dictionary<string, Dictionary<string, List<string>>> dictVTSizes = new Dictionary<string, Dictionary<string, List<string>>>();

            if (dtERPSizes != null && dtERPSizes.Rows.Count > 0)
            {
                var groupedVTSizes = dtERPSizes.AsEnumerable()
                    .GroupBy(r => new
                    {
                        ID = r.Field<object>("ID")?.ToString(),
                        MaVTID = r.Field<object>("MaVTID")?.ToString(),
                        MaMauVT = r.Field<object>("MaMauVT")?.ToString(),
                        MaKhoVT = r.Field<object>("MaKhoVT")?.ToString(),
                        MaNhomVT = r.Field<object>("MaNhomVT")?.ToString(),
                        MaNhomChiTiet = r.Field<object>("MaNhomChiTiet")?.ToString(),
                        MaCode = r.Field<object>("MaCode")?.ToString()
                    })
                    .Select(g => new
                    {
                        Key = g.Key,
                        SizesByNhom = g.GroupBy(r => r.Field<string>("NhomSize"))
                                       .Select(ng => new
                                       {
                                           NhomSize = ng.Key,
                                           Sizes = ng.Select(r => r.Field<object>("TenSize")?.ToString() ?? "")
                                                     .Distinct()
                                                     .OrderBy(s => int.TryParse(s, out int num) ? 0 : 1)
                                                     .ThenBy(s => int.TryParse(s, out int num) ? num : 0)
                                                     .ThenBy(s => s)
                                                     .ToList()
                                       })
                                       .ToDictionary(x => x.NhomSize, x => x.Sizes)
                    });

                foreach (var item in groupedVTSizes)
                {
                    string key = item.Key.ID;
                    dictVTSizes[key] = item.SizesByNhom;
                }
            }
            foreach (DataRow row in tblMain.Rows)
            {
                string id = row["ID"]?.ToString();
                string sizeValue = "";

                if (!string.IsNullOrEmpty(id) && dictVTSizes.ContainsKey(id))
                {
                    var vtSizesByNhom = dictVTSizes[id];
                    bool isAllSize = true;
                    List<string> sizeStrings = new List<string>();
                    var sortedNhoms = vtSizesByNhom
                        .Select(kv => new
                        {
                            Key = kv.Key,
                            Value = kv.Value,
                            IsNum = int.TryParse(kv.Key, out int n),
                            NumVal = int.TryParse(kv.Key, out int n2) ? n2 : int.MaxValue
                        })
                        .OrderBy(x => x.IsNum ? 0 : 1)
                        .ThenBy(x => x.NumVal)
                        .ThenBy(x => x.Key);

                    foreach (var nhom in sortedNhoms)
                    {
                        string nhomSize = nhom.Key;
                        List<string> vtSizes = nhom.Value;
                        if (dictAllSizes.ContainsKey(nhomSize))
                        {
                            List<string> allSizes = dictAllSizes[nhomSize];
                            bool containsAllSizes = allSizes.All(s => vtSizes.Contains(s));

                            if (!containsAllSizes)
                            {
                                isAllSize = false;
                            }
                        }
                        else
                        {
                            isAllSize = false;
                        }
                        string sizeString = nhomSize + ":" + string.Join(",", vtSizes);
                        sizeStrings.Add(sizeString);
                    }
                    if (isAllSize && sizeStrings.Count > 0)
                    {
                        sizeValue = "AllSize";
                    }
                    else
                    {
                        sizeValue = string.Join("; ", sizeStrings);
                    }
                }

                row["Size"] = sizeValue;
            }

            return tblMain;
        }

        private void loadGridChiTietCanDoi()
        {
            string url = $"{URL}ERPDonHangTong/Get?action=GetChiTietLenh&para={maDH}&para2={PO}&para3={Mau}&para4={Line}&para5={maLenhSX}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    grcChiTietCanDoi.DataSource = null;
                    return;
                }
            grcChiTietCanDoi.DataSource = tbl;
            CreateBandSize(tbl, bandedGridView1, gbSize, repotxtN0);
            AmoutRow(tbl);
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

        private void AmoutRow(DataTable _data)
        {
            if (!_data.Columns.Contains("Amount"))
                _data.Columns.Add("Amount", typeof(int));

            foreach (DataRow row in _data.Rows)
            {
                int total = 0;
                foreach (DataColumn column in _data.Columns)
                {
                    if (column.ColumnName.Contains("@"))
                    {
                        total += Convert.ToInt32(row[column]);
                    }
                }
                row["Amount"] = total;
            }

            // 🔸 Nếu cột Amount đã tồn tại trong grid → chỉnh lại thuộc tính
            BandedGridColumn colAmount = bandedGridView1.Columns["Amount"] as BandedGridColumn;
            if (colAmount == null)
            {
                // Nếu chưa có, tạo mới
                colAmount = new BandedGridColumn
                {
                    FieldName = "Amount",
                    Caption = "Tổng",
                    Visible = true,
                    Width = 80,
                    OptionsColumn = { AllowEdit = false }
                };
                bandedGridView1.Columns.Add(colAmount);
            }
            else
            {
                // Nếu có, chỉnh caption/format nếu cần
                colAmount.Caption = "Tổng số lượng";
                colAmount.Width = 100;
            }

            // 🔸 Format số
            colAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colAmount.DisplayFormat.FormatString = "n0";

            // 🔸 Xóa summary cũ nếu có
            colAmount.Summary.Clear();

            // 🔸 Thêm summary mới
            colAmount.Summary.Add(DevExpress.Data.SummaryItemType.Sum, "Amount", "{0:n0}");
            bandedGridView1.OptionsView.ShowFooter = true;
        }

        #endregion

        #region dataGridMain
        private async Task loadCayVaiDaChon()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = URL + $"chitiet/get?action=cayvaidachon&para1={maVTID}&para2={mauVTID}&para3={khoVaiID}&para4={maCLVT}";

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<CayVaiPOMua>>(json);

                    selectedItems.RaiseListChangedEvents = false;
                    selectedItems.Clear();

                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            selectedItems.Add(item);
                        }
                    }

                    selectedItems.RaiseListChangedEvents = true;
                    selectedItems.ResetBindings();

                    UpdateDisplayAfterLoading();
                }
            }
        }

        private void UpdateDisplayAfterLoading()
        {
            string chuoiPOMua = string.Join(", ", selectedItems.Select(x => x.POMua).Distinct());
            DataRow dr = grvCanDoi.GetFocusedDataRow();
            if (dr != null)
            {
                dr["POMua"] = chuoiPOMua;
            }
            if (this.ActiveControl is DevExpress.XtraEditors.SearchLookUpEdit editor)
            {
                editor.Refresh();
            }
        }
        // repositoryItemSearchLookUpCayVai.Popup += repositoryItemSearchLookUpCayVai_Popup;

        private void repositoryItemSearchLookUpCayVai_Popup(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.SearchLookUpEdit edit = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (edit == null) return;

            var view = edit.Properties.View;
            isLoadingSelection = true;

            try
            {
                view.ClearSelection();

                for (int i = 0; i < view.RowCount; i++)
                {
                    var row = view.GetRow(i) as CayVaiPOMua;
                    if (row != null && selectedItems.Any(x => x.BarCode == row.BarCode))
                    {
                        view.SelectRow(i);
                    }
                }
            }
            finally
            {
                isLoadingSelection = false;
            }
        }

        #endregion

        #region function
        private async void gridViewCanDoi_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null || e.FocusedRowHandle < 0) return;

            DataRow dr = grvCanDoi.GetFocusedDataRow();
            string newMaVTID = dr["MaVTID"].ToString();
            string newMauVTID = dr["MauID"].ToString();
            string newKhoVaiID = dr["KhoVaiID"].ToString();
            string newMaCLVT = dr["MaNhom"].ToString();

            maVTID = newMaVTID;
            mauVTID = newMauVTID;
            khoVaiID = newKhoVaiID;
            maCLVT = newMaCLVT;

            maLenhSanXuat = dr["MaLenhSanXuat"].ToString();
            maDonHang = dr["MaDH"].ToString();

            selectedItems.Clear();
            await filterChonCayVai();
            await loadCayVaiDaChon();
        }

        private void FocusDefaultRow()
        {
            var view = grvCanDoi;
            if (view.RowCount == 0) return;

            for (int i = 0; i < view.RowCount; i++)
            {
                string vMaVT = view.GetRowCellValue(i, "MaVTID")?.ToString();
                string vMau = view.GetRowCellValue(i, "MauID")?.ToString();
                string vKho = view.GetRowCellValue(i, "KhoVaiID")?.ToString();

                if (vMaVT == maVTID && vMau == mauVTID && vKho == khoVaiID)
                {
                    view.FocusedRowHandle = i;
                    view.SelectRow(i);
                    view.MakeRowVisible(i);
                    break;
                }
            }
        }

        private async void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (selectedItems == null || selectedItems.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một cây vải trước khi lưu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dtPost = new DataTable("DataType1");
                dtPost.Columns.Add("MaVTID");
                dtPost.Columns.Add("MaVT");
                dtPost.Columns.Add("MauVTID");
                dtPost.Columns.Add("MauVT");
                dtPost.Columns.Add("KhoVaiID");
                dtPost.Columns.Add("KhoVai");
                dtPost.Columns.Add("MaNhom");
                dtPost.Columns.Add("TenCL");
                dtPost.Columns.Add("MaDVVT");
                dtPost.Columns.Add("TenDVVT");
                dtPost.Columns.Add("SoLoID");
                dtPost.Columns.Add("MaNPL");
                dtPost.Columns.Add("SoKienHienThi");
                dtPost.Columns.Add("SoLuongThucTe", typeof(decimal));
                dtPost.Columns.Add("BarCode");
                dtPost.Columns.Add("SoLoT");
                dtPost.Columns.Add("Batch");
                dtPost.Columns.Add("NgayNhap", typeof(DateTime));
                dtPost.Columns.Add("MaONPL");
                dtPost.Columns.Add("POMua");
                dtPost.Columns.Add("NguoiTao");
                dtPost.Columns.Add("NgayTao", typeof(DateTime));
                dtPost.Columns.Add("MaLenhSX");
                dtPost.Columns.Add("MaDH");
                dtPost.Columns.Add("IsNPL");

                foreach (var item in selectedItems)
                {
                    DataRow row = dtPost.NewRow();
                    row["MaVTID"] = item.MaVTID;
                    row["MaVT"] = item.MaVT;
                    row["MauVTID"] = item.MauVTID;
                    row["MauVT"] = item.MauVT;
                    row["KhoVaiID"] = item.KhoVaiID;
                    row["KhoVai"] = item.KhoVai;
                    row["MaNhom"] = item.MaNhom;
                    row["TenCL"] = item.TenCL;
                    row["MaDVVT"] = item.MaDVVT;
                    row["TenDVVT"] = item.TenDVVT;
                    row["SoLoID"] = item.SoLoID;
                    row["MaNPL"] = item.MaNPL;
                    row["SoKienHienThi"] = item.SoKienHienThi;
                    row["SoLuongThucTe"] = item.SoLuongThucTe;
                    row["BarCode"] = item.BarCode;
                    row["SoLoT"] = item.SoLoT;
                    row["Batch"] = item.Batch;
                    row["NgayNhap"] = item.NgayTaoNhapKho;
                    row["MaONPL"] = item.MaONPL;
                    row["POMua"] = item.POMua;
                    row["NguoiTao"] = GlobleData.UserName; // Lấy từ thông tin user đăng nhập
                    row["NgayTao"] = DBNull.Value;
                    row["MaLenhSX"] = maLenhSanXuat;
                    row["MaDH"] = maDonHang;
                    row["IsNPL"] = item.IsNPL;

                    dtPost.Rows.Add(row);
                }

                using (HttpClient client = new HttpClient())
                {
                    string urlPost = URL + "chitiet/postcayvaicapphat";

                    string jsonPayload = JsonConvert.SerializeObject(dtPost);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(urlPost, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        if (result.ToLower().Contains("true") || result.Contains("1"))
                        {
                            MessageBox.Show("Cập nhật cấp phát cây vải thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = DialogResult.OK;
                        }
                        else
                        {
                            MessageBox.Show("Lưu thất bại: " + result, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không thể kết nối đến máy chủ API.", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            var view = gridControlDetailCayVai.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null) return;

            var itemToRemove = view.GetFocusedRow() as CayVaiPOMua;

            if (itemToRemove != null)
            {
                //if (MessageBox.Show($"Bạn có chắc chắn muốn bỏ chọn cây vải {itemToRemove.BarCode}?", "Xác nhận",
                //    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                //{

                //}

                selectedItems.Remove(itemToRemove);
                UpdateDisplayAfterLoading();

                DataRow drMain = grvCanDoi.GetFocusedDataRow();
                if (drMain != null)
                {
                    drMain["IsEdited"] = true;
                }
                if (this.ActiveControl is DevExpress.XtraEditors.SearchLookUpEdit editor)
                {
                    editor.Refresh();
                }
            }
        }

        private void btnDeleteMany_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var view = gridControlDetailCayVai.MainView as DevExpress.XtraGrid.Views.Grid.GridView;

            if (view == null || view.SelectedRowsCount == 0)
            {
                return;
            }

            string message = $"Bạn có chắc chắn muốn bỏ chọn {view.SelectedRowsCount} cây vải đang chọn?";
            if (MessageBox.Show(message, "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                List<CayVaiPOMua> itemsToDelete = new List<CayVaiPOMua>();
                int[] selectedRowHandles = view.GetSelectedRows();

                foreach (int rowHandle in selectedRowHandles)
                {
                    if (rowHandle >= 0)
                    {
                        var item = view.GetRow(rowHandle) as CayVaiPOMua;
                        if (item != null) itemsToDelete.Add(item);
                    }
                }

                selectedItems.RaiseListChangedEvents = false;
                try
                {
                    foreach (var item in itemsToDelete)
                    {
                        selectedItems.Remove(item);
                    }
                }
                finally
                {
                    selectedItems.RaiseListChangedEvents = true;
                    selectedItems.ResetBindings();
                }
                UpdateDisplayAfterLoading();
                DataRow drMain = grvCanDoi.GetFocusedDataRow();
                if (drMain != null)
                {
                    drMain["IsEdited"] = true;
                }

                if (this.ActiveControl is DevExpress.XtraEditors.SearchLookUpEdit editor)
                {
                    editor.Refresh();
                }

                MessageBox.Show($"Đã bỏ chọn {itemsToDelete.Count} dòng.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion
    }
}
