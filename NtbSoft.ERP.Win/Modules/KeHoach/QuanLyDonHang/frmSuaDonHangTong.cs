using DevExpress.Data;
using DevExpress.DataAccess.Excel;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Entity.ThuVien;
using NtbSoft.ERP.Win.Modules.Erp.Kehoach;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmSuaDonHangTong : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        DonHangTongEntity donHangTong;
        DataTable tblDonHangChiTiet;
        DataTable _dtSizeNhom;
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        List<string> lstSize;
        int sum = 0;
        List<BangSizeEntity> lstBangSize;
        List<DonHangTongEntity> dhTong;
        List<DonHangTongPOEntity> dhTongPO;
        List<DonHangTongPOChiTietEntity> dhTongPOChiTiet;
        // tạo unsave để phân biệt size chưa lưu vào database 10/10/2025
        private List<string> _unsavedSelectedSizes = new List<string>();
        // end
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;

        bool indicatorIcon = true;

        KeyDownControlHandler keyDownControlHandler;

        ActionControl actionControlAdd;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        string _nguoiTao = string.Empty;

        public frmSuaDonHangTong(DonHangTongEntity _donHangTong, DataTable tblDHTChiTiet, bool allowAdd, bool allowEdit, bool allowDelete, string nguoitao)
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);

            layoutControlItem1.Enabled = false;
            layoutControlItem3.Enabled = false;
            layoutControlItem15.Enabled = false;
            layoutControlItem4.Enabled = false;
            layoutControlItem8.Enabled = false;

            tblDonHangChiTiet = tblDHTChiTiet.Copy();

            lstSize = new List<string>();
            _clientExtension = new HttpClientExtension();
            donHangTong = _donHangTong;
            dhTong = new List<DonHangTongEntity>();
            dhTongPO = new List<DonHangTongPOEntity>();
            dhTongPOChiTiet = new List<DonHangTongPOChiTietEntity>();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _allowAdd = allowAdd;
            _allowEdit = allowEdit;
            _allowDelete = allowDelete;
            this._nguoiTao = nguoitao;
        }

        private void BtLoad(bool isReload)
        {
            try
            {
                SetData();
                CreateSearchLookup(donHangTong.MaHang);
                GetChiTietDonHangTongPO(isReload);
                CreateSearchLookupHT();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        protected override void OnLoad(EventArgs e)
        {
            BtLoad(false);
            CheckPerminsion();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        private List<ActionControl> InitActionKeyDown()
        {
            actionControlAdd = new ActionControl(BtThem, _allowAdd, ActionType.Add, this.Them.Enabled);
            actionControlSave = new ActionControl(LuuDong, (_allowAdd || _allowEdit), ActionType.Save, this.Luu.Enabled);
            actionControlRefresh = new ActionControl(BtNapLai, true, ActionType.Refresh, this.Naplai.Enabled);

            lstActionControls = new List<ActionControl> {
                actionControlAdd, actionControlSave,actionControlRefresh };
            return lstActionControls;
        }
        private void gridViewThongTinDonHang_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }
        private void CheckPerminsion()
        {
            string url = string.Format("{0}/GetPer?userID={1}", URL + ResourceURL.UrlUserModule, GlobleData.UserName);
            List<SystemUserModuleEntity> List = Task.Run(async () => { return await _serviceUserModule.UserModuleGet(url); }).Result;
            SystemUserModuleEntity obj = (from m in List
                                          where m.FormShow == this.Name
                                          select m).FirstOrDefault();
            if (obj == null) return;
            _allowAdd = obj.AllowAdd;
            _allowEdit = obj.AllowEdit;
            _allowDelete = obj.AllowDelete;
            if (!_allowAdd)
            {
                Them.Enabled = false;
            }
            if (!_allowEdit)
            {
                Sua.Enabled = false;
                Luu.Enabled = false;
            }
            else
            {
                Luu.Enabled = false;
            }
            if (!_allowDelete)
                Xoa.Enabled = false;
        }
        private void CreateSearchLookup(string maHang)
        {
            string urlHH = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            DataTable tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
            searchLookUpEditMH.Properties.DataSource = tblHH;
            searchLookUpEditMH.Properties.ValueMember = "MaHang";
            searchLookUpEditMH.Properties.DisplayMember = "TenHang";

            string urlKH = string.Format("{0}?", URL + "KhachHang/GetKhachHang");
            string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
            DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
            searchLookUpEditKH.Properties.DataSource = tblKH;
            searchLookUpEditKH.Properties.ValueMember = "MaKH";
            searchLookUpEditKH.Properties.DisplayMember = "TenKH";

            string urlCL = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetChungLoai", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
            string jsonCL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCL); }).Result;
            DataTable tblCL = JsonConvert.DeserializeObject<DataTable>(jsonCL);
            searchLookUpEditCL.Properties.DataSource = tblCL;
            searchLookUpEditCL.Properties.ValueMember = "MaCL";
            searchLookUpEditCL.Properties.DisplayMember = "TenCL";

            // Người tạo
            string urlNV = string.Format("{0}?", URL + "NhanVien/GetAllNV");
            string jsonNV = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNV); }).Result;
            DataTable tblnv = JsonConvert.DeserializeObject<DataTable>(jsonNV);
            string username = GlobleData.UserName.ToString();
            DataRow ten = tblnv.AsEnumerable().FirstOrDefault(r => string.Equals(r.Field<string>("UserID")?.Trim(), donHangTong.NguoiTao, StringComparison.OrdinalIgnoreCase));
            txtNguoiTao.Text = ten != null ? ten["Ten"].ToString() : username.ToString();

            // searchlookupedit màu
            //string urlMau = string.Format("{0}?maHang={1}", URL + "BangMau/GetBangMauAllowMaHang", maHang);
            //string jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
            //DataTable tblMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);
            //this.searchLookUpEditMau1.DataSource = tblMau;

            // searchlookupedit Quốc gia
            string urlQG = string.Format("{0}", URL + "QuocGia/GetQuocGia");
            string jsonQG = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlQG); }).Result;
            DataTable tblQG = JsonConvert.DeserializeObject<DataTable>(jsonQG);
            this.searchLookUpEditQuocGia1.DataSource = tblQG;

            string urlSize = string.Format("{0}", URL + "BangSize/GetBangSize");
            string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
            lstBangSize = JsonConvert.DeserializeObject<List<BangSizeEntity>>(jsonSize);

            string urlPD = string.Format("{0}?", URL + "PhapDanhCty/Get");
            string jsonPD = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlPD); }).Result;
            DataTable tblPD = JsonConvert.DeserializeObject<DataTable>(jsonPD);
            searchLookUpEditCTY.Properties.DataSource = tblPD;
            searchLookUpEditCTY.Properties.ValueMember = "MaCty";
            searchLookUpEditCTY.Properties.DisplayMember = "TenCty";
        }

        private void SetData()
        {
            this.Them.Enabled = true;
            txtMaDH.Text = donHangTong.MaDH;
            searchLookUpEditKH.Text = donHangTong.MaKH;
            searchLookUpEditMH.Text = donHangTong.MaHang;
            searchLookUpEditCL.EditValue = donHangTong.MaCL;
            txtDot.Text = donHangTong.Dot;
            txtNguoiTao.Text = donHangTong.NguoiTao;
            dateEditNgayTao.EditValue = donHangTong.NgayTao;
            spinEditVND.EditValue = donHangTong.VND;
            spinEditUSD.EditValue = donHangTong.CM;
            txtGhiChu.Text = donHangTong.GhiChu;
            txtMaHS.Text = donHangTong.MaHS;
            txtSoVoice.Text = donHangTong.SoVoice;
            searchLookUpEditCTY.EditValue = donHangTong.MaCty;
            textEdit1.EditValue = tblDonHangChiTiet.Rows[0]["FOB"];
            textEdit2.EditValue = tblDonHangChiTiet.Rows[0]["HeSoDH"];
            serachlookupeditHT.EditValue = donHangTong.HinhThucXuatHang;
        }

        private void GetChiTietDonHangTongPO(bool isReload)
        {
            _status = ResourceURL.EventStatus.View;


            DataTable tbl = tblDonHangChiTiet.Copy();
            if (tbl.Columns.Contains("CheckColumn"))
            {
                tbl.Columns.Remove("CheckColumn");
            }
            bool flagRemoveTongSL = false;
            // Xóa cột Tổng số lượng
            foreach (DataColumn column in tbl.Columns)
            {
                if (column.ColumnName.Contains("TongSL"))
                {
                    flagRemoveTongSL = true;
                }
            }
            if (flagRemoveTongSL)
            {
                tbl.Columns.Remove("TongSL");
            }

            //if (isReload)
            //{
            string url = string.Format("{0}?maDH={1}", URL + "DonHangTong/GetPivotDonHangTongPOChiTiet", donHangTong.MaDH);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tbl = JsonConvert.DeserializeObject<DataTable>(json);
            ConvertColumnsToDateTime(tbl, "NgayDKVC", "NgayThucTeVC", "NgayXuatHang");
            tblDonHangChiTiet = tbl.Copy();
            //}
            //createColBandsSize(tbl);
            gridControlThongTinDonHang.MainView = GetBandGridViewAmount(tbl);
            gridControlThongTinDonHang.DataSource = tbl;
            string SoBooking = tbl?.Rows?.Count == 0 ? string.Empty : tbl.Rows[0]["BookingMaHang"]?.ToString();
            txtSoBooing.Text = string.IsNullOrEmpty(SoBooking)? string.Empty : SoBooking;

            DataTable dataSource = gridControlThongTinDonHang.DataSource as DataTable;
            if (!dataSource.Columns.Contains("CheckColumn"))
            {
                dataSource.Columns.Add("CheckColumn", typeof(bool));
            }

            // Gán giá trị mặc định true cho cột "CheckColumn" cho tất cả các dòng trong DataTable
            foreach (DataRow row in dataSource.Rows)
            {
                row["CheckColumn"] = true;
            }
        }

        private DevExpress.XtraGrid.Views.Base.BaseView GetBandGridViewAmount(DataTable tab)
        {
            BandedGridView bandedView = new BandedGridView();
            bandedView.OptionsView.ShowColumnHeaders = false;
            bandedView.OptionsView.ShowGroupPanel = false;
            bandedView.OptionsView.ColumnAutoWidth = false;
            bandedView.OptionsView.ShowFooter = true;
            if (tab.Columns.Count == 0) return bandedView;
            for (int i = 0; i < 17; i++)
            {
                List<string> _ListString = new List<string>();

                if (tab.Columns[i].ColumnName == "MaDH")
                {
                    _ListString.Add("MaDH");
                    SetGridBandedViewAmount(bandedView, "", "Đơn hàng", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "POID")
                {
                    _ListString.Add("POID");
                    SetGridBandedViewAmount(bandedView, "", "POID", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "PO")
                {
                    _ListString.Add("PO");
                    SetGridBandedViewAmount(bandedView, "", "PO", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "ColorCode")
                {
                    _ListString.Add("ColorCode");
                    SetGridBandedViewAmount(bandedView, "", "Color Code", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "MaMau")
                {
                    _ListString.Add("MaMau");
                    SetGridBandedViewAmount(bandedView, "", "Màu", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "DauSizeID")
                {
                    _ListString.Add("DauSizeID");
                    SetGridBandedViewAmount(bandedView, "", "InSeam", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "DauSize")
                {
                    _ListString.Add("DauSize");
                    SetGridBandedViewAmount(bandedView, "", "InSeam", _ListString);
                    _ListString.Clear();
                }

                if (tab.Columns[i].ColumnName == "MaQG")
                {
                    _ListString.Add("MaQG");
                    SetGridBandedViewAmount(bandedView, "", "Quốc gia", _ListString);
                    _ListString.Clear();
                }

                if (tab.Columns[i].ColumnName == "NgayGH")
                {
                    _ListString.Add("NgayGH");
                    SetGridBandedViewAmount(bandedView, "", "Ngày giao hàng", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "NgayDKVC")
                {
                    _ListString.Add("NgayDKVC");
                    SetGridBandedViewAmount(bandedView, "", "Ngày Đăng Ký VC", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "NgayThucTeVC")
                {
                    _ListString.Add("NgayThucTeVC");
                    SetGridBandedViewAmount(bandedView, "", "Ngày Thực Tế VC", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "NgayXuatHang")
                {
                    _ListString.Add("NgayXuatHang");
                    SetGridBandedViewAmount(bandedView, "", "Ngày Xuất Hàng", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "GhiChu")
                {
                    _ListString.Add("GhiChu");
                    SetGridBandedViewAmount(bandedView, "", "Ghi chú", _ListString);
                    _ListString.Clear();
                }
            }
            List<string> listHeader = new List<string>();
            int j = 17;
            string maHang = string.Empty;
            while (j < tab.Columns.Count)
            {
                if (tab.Columns[j].ColumnName == "CheckColumn" || tab.Columns[j].ColumnName == "IsNew")
                {
                    j++;
                    continue;
                }
                // lstSize.Add(colName.Replace("@Size@", ""));
                lstSize.Add(tab.Columns[j].ColumnName.Split('@').Last());
                string[] arrName = tab.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                maHang = arrName[1];
                listHeader.Add(tab.Columns[j].ColumnName);
                j++;
            }

            GridBand gridBand = new GridBand();
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            BandedGridColumn colAmount = bandedView.Columns.AddField("Amount");
            SetGridBandedViewAmount(bandedView, maHang, "", listHeader);

            colAmount.OptionsColumn.AllowEdit = false;
            colAmount.OptionsColumn.ReadOnly = true;
            colAmount.Caption = "Tổng";
            colAmount.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            //colAmount.UnboundExpression = exp;
            colAmount.Visible = true;
            colAmount.OwnerBand = gridBand;
            gridBand.Visible = true;
            gridBand.Caption = "Tổng";
            GridColumnSummaryItem item = colAmount.Summary.Add(SummaryItemType.Custom);
            item.FieldName = "Amount";
            item.DisplayFormat = "{0:n0}";
            bandedView.Bands.Add(gridBand);
            foreach (BandedGridColumn col in bandedView.Columns)
            {
                if (col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "MaQG"
                && col.FieldName != "NgayGH")
                {
                    GridGroupSummaryItem itemSize = new GridGroupSummaryItem();
                    itemSize.FieldName = col.FieldName;
                    itemSize.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    itemSize.DisplayFormat = "{0:n0}";
                    itemSize.ShowInGroupColumnFooter = col;
                    bandedView.GroupSummary.Add(itemSize);
                }
                string[] arrName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrName.Length > 1)
                {
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                }
            }

            bandedView.CustomDrawBandHeader += BandedView_CustomDrawBandHeader;
            bandedView.CustomColumnDisplayText += BandedView_CustomColumnDisplayText;
            bandedView.CustomDrawFooter += BandedView_CustomDrawFooter;
            bandedView.CustomDrawFooterCell += BandedView_CustomDrawFooterCell;
            bandedView.CustomUnboundColumnData += BandedView_CustomUnboundColumnData;
            bandedView.CustomSummaryCalculate += BandedView_CustomSummaryCalculate;
            bandedView.CellValueChanged += BandedView_CellValueChanged;
            bandedView.CellValueChanging += BandedView_CellValueChanging;
            bandedView.ShowingEditor += bandedGridView2_ShowingEditor;
            bandedView.PopupMenuShowing += BandedView_PopupMenuShowing;
            bandedView.FocusedColumnChanged += BandedView_FocusedColumnChanged;
            bandedView.FocusedRowChanged += BandedView_FocusedRowChanged;
            bandedView.ValidatingEditor += BandedGridViewThongTinDonHang_ValidatingEditor;
            bandedView.CustomDrawRowIndicator += BandedView_CustomDrawRowIndicator;
            bandedView.RowCountChanged += BandedView_RowCountChanged;
            bandedView.RowStyle += BandedView_CustomDrawCell;


            bandedView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            bandedView.OptionsSelection.MultiSelect = true;
            bandedView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bandedView.OptionsNavigation.EnterMoveNextColumn = true;

            #region Luân -Thêm Sự Kiện Mới
            bandedView.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.bandedGridViewThongTinDonHang_RowCellClick);

            #endregion
            return bandedView;
        }

        private void bandedGridView2_ShowingEditor(object sender, CancelEventArgs e)
        {
            Console.WriteLine("bandedGridView2_ShowingEditor");
            GridView gridView = sender as GridView;
            DataRow rowFocused = (gridView.GetFocusedRow() as DataRowView).Row;
            int parse = -1;
            
            if (gridView.FocusedColumn.FieldName.Contains("@Size@"))
            {
                return;
            }
            if (gridView.FocusedColumn.FieldName.Contains("Amount"))
            {
                return;
            }
            if (rowFocused != null && rowFocused["IsCD"] != null
                && int.TryParse(rowFocused["IsCD"].ToString(), out parse))
            {
                int SLTH = int.Parse(rowFocused["IsCD"].ToString());
                if (SLTH == 1 && bool.Parse(rowFocused["CheckColumn"].ToString()) == false) 
                {
                    return;
                }
                if (SLTH ==1 && gridView.FocusedColumn.FieldName != "NgayGH" && gridView.FocusedColumn.FieldName != "NgayDKVC" && gridView.FocusedColumn.FieldName != "NgayThucTeVC" && gridView.FocusedColumn.FieldName != "NgayXuatHang"
                    && !gridView.FocusedColumn.FieldName.Contains("@Size@"))
                {
                    XtraMessageBox.Show("PO - Màu - Đầu size này đã được cân đối sản xuất. Không thể thay đổi dữ liệu.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Cancel = true;
                }

            }
        }

        private void BandedView_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {

            GridView gridView = sender as GridView;
            if (e.RowHandle >= 0 && (e.Column.FieldName == "NgayGH" || e.Column.FieldName == "MaQG" || e.Column.FieldName == "NgayDKVC" || e.Column.FieldName == "NgayThucTeVC"
               || e.Column.FieldName == "NgayXuatHang"))
            {
                DataRow rowFocused = (gridView.GetFocusedRow() as DataRowView).Row;
                if (rowFocused == null) return;
                DataTable dataTable = (gridView.DataSource as DataView).Table as DataTable;
                //if (rowFocused["IsCD"].ToString() != "")
                //{
                //    if (Convert.ToInt32(rowFocused["IsCD"]) == 1)
                //    {
                //        MessageBox.Show("PO đã được chia sản xuất không được phép sửa. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //        //GetChiTietDonHangTongPO(false);
                //        return;
                //    }
                //}
                if (e.RowHandle >= 0 && (e.Column.FieldName == "MaQG" || e.Column.FieldName == "NgayGH" || e.Column.FieldName == "NgayDKVC" || e.Column.FieldName == "NgayThucTeVC"
                    || e.Column.FieldName == "NgayXuatHang"))
                {
                    if (rowFocused != null && rowFocused["PO"] != null && dataTable != null && dataTable.Rows.Count > 0)
                    {
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            if (dataTable.Rows[i]["PO"] != null && (rowFocused["PO"].Equals(dataTable.Rows[i]["PO"])))
                            {
                                gridView.SetRowCellValue(i, e.Column.FieldName, e.Value);
                            }
                        }
                    }
                }

            }

        }

        private void BandedView_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;
            //e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            e.Appearance.ForeColor = Color.Red;
        }

     

        private void BandedView_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void BandedView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            try
            {
                GridView view = (GridView)sender;
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {
                    string sText = (e.RowHandle + 1).ToString();
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString(sText, e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = sText;
                }
                if (!indicatorIcon)
                    e.Info.ImageIndex = -1;

                if (e.RowHandle == GridControl.InvalidRowHandle)
                {
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    GridView gridView = ((GridView)sender);
                    SizeF size = gr.MeasureString("STT", e.Info.Appearance.Font);
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 20;
                    if (gridView.IndicatorWidth < nNewSize)
                    {
                        gridView.IndicatorWidth = nNewSize;
                    }

                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void BandedView_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            GridView gridView = sender as GridView;
            DataRow rowFocused = (gridView.GetFocusedRow() as DataRowView).Row;
            if (e.Menu == null)
                return;
            e.Menu.Items.Clear();

            if (e.HitInfo.InRow)
            {
                if (e.HitInfo.Column != null)
                {

                    // Example of column names where you want the context menu to appear
                    List<string> allowedColumns = new List<string> { "ID", "MaDH", "POID", "PO", "MaQG", "DauSizeID", "DauSize", "NgayGH", "GhiChu", "MaMau" };

                    if (!allowedColumns.Contains(e.HitInfo.Column.FieldName))
                    {
                        // Add the "Copy" item to the context menu
                        DevExpress.Utils.Menu.DXMenuItem menuCopyItem = new DevExpress.Utils.Menu.DXMenuItem("Copy", ItemCopy_Click);
                        e.Menu.Items.Add(menuCopyItem);

                        // Add the "Paste" item to the context menu
                        DevExpress.Utils.Menu.DXMenuItem menuPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Paste", ItemPaste_Click);
                        e.Menu.Items.Add(menuPasteItem);
                    }
                }
                DevExpress.Utils.Menu.DXMenuItem menuCoppyPasteItem = new DevExpress.Utils.Menu.DXMenuItem("Copy dòng", ItemCoppyPaste_Click);
                e.Menu.Items.Add(menuCoppyPasteItem);
                int SLTH = 0;
                if (rowFocused["IsCD"] != DBNull.Value && !string.IsNullOrWhiteSpace(rowFocused["IsCD"].ToString()))
                {
                    SLTH = Convert.ToInt32(rowFocused["IsCD"]);
                }
                if (SLTH == 1 && bool.Parse(rowFocused["CheckColumn"].ToString()) == true)
                {
                    return;
                }
                DevExpress.Utils.Menu.DXMenuItem menuCoppyDeleteItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa dòng thêm mới", ItemDelete_Click);
                e.Menu.Items.Add(menuCoppyDeleteItem);
            }
        }
        private void ItemCopy_Click(object sender, EventArgs e)
        {
            BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            view.CopyToClipboard();

        }
        private void ItemPaste_Click(object sender, EventArgs e)
        {
            BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            string[] data = ClipboardData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length < 1) return;
            int startRow = view.FocusedRowHandle;
            int endRow = startRow + data.Length - 1;
            foreach (string row in data)
            {
                AddRow(row, startRow++);
                if (!view.IsValidRowHandle(startRow)) break;
            }
            SelectRows(view, startRow - data.Length, startRow - 1);
        }
        private void SelectRows(BandedGridView view, int startRow, int endRow)
        {
            view.ClearSelection(); // Xóa các dòng được chọn trước đó
            for (int i = startRow; i <= endRow; i++)
            {
                if (view.IsValidRowHandle(i))
                {
                    view.SelectRow(i);
                }
            }
        }
        private void ItemCoppyPaste_Click(object sender, EventArgs e)
        {
            // Lấy view từ GridControl
            BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;

            // Lấy vị trí của dòng được chọn
            //int focusedRowHandle = view.FocusedRowHandle;
            //if (focusedRowHandle < 0) return;
            int[] selectedRowHandles = view.GetSelectedRows();
            if (selectedRowHandles.Length == 0) return;
            // Tạo một từ điển chứa dữ liệu của dòng được chọn
            //var rowData = new Dictionary<string, object>();
            //foreach (DevExpress.XtraGrid.Columns.GridColumn column in view.VisibleColumns)
            //{
            //    string fieldName = column.FieldName;
            //    if (!string.IsNullOrEmpty(fieldName))
            //    {
            //        rowData[fieldName] = view.GetRowCellValue(focusedRowHandle, column);
            //    }
            //}


            DataTable dataSource = gridControlThongTinDonHang.DataSource as DataTable;
            if (dataSource == null) return;

            //DataRow newRow = view.GetFocusedDataRow();


            //// Tạo một dòng mới và thêm vào dataSource
            //DataRow insertedRow = dataSource.NewRow();

            //// Gán giá trị cho các cột trong dòng mới 
            //foreach (DataColumn column in dataSource.Columns)
            //{
            //    if (newRow.Table.Columns.Contains(column.ColumnName))
            //    {
            //        insertedRow[column.ColumnName] = newRow[column.ColumnName];
            //    }
            //}

            //// Đặt giá trị mặc định cho "CheckColumn" trong dòng mới là false
            //insertedRow["CheckColumn"] = false;

            //// Chèn dòng mới vào DataTable (sau dòng hiện tại)
            ////dataSource.Rows.InsertAt(insertedRow, focusedRowHandle + 1);
            Array.Sort(selectedRowHandles);
            Array.Reverse(selectedRowHandles);

            foreach (int rowHandle in selectedRowHandles)
            {
                // Lấy dữ liệu của từng dòng được chọn
                DataRow selectedRow = view.GetDataRow(rowHandle);
                if (selectedRow == null) continue;

                // Tạo một dòng mới hoàn toàn
                DataRow newRow = dataSource.NewRow();

                // Sao chép dữ liệu từ dòng được chọn vào dòng mới
                foreach (DataColumn column in dataSource.Columns)
                {
                    if (selectedRow.Table.Columns.Contains(column.ColumnName))
                    {
                        newRow[column.ColumnName] = selectedRow[column.ColumnName];
                    }
                }

                // Đặt giá trị mặc định cho "CheckColumn" là false
                if (dataSource.Columns.Contains("CheckColumn"))
                {
                    newRow["CheckColumn"] = false;
                }

                // Xác định vị trí chèn
                int insertIndex = rowHandle + 1;
                if (insertIndex > dataSource.Rows.Count)
                {
                    insertIndex = dataSource.Rows.Count;
                }

                // Chèn dòng mới vào DataTable
                dataSource.Rows.InsertAt(newRow, insertIndex);
            }

            // Cập nhật lại GridControl
            view.RefreshData();

            // Cập nhật lại GridControl
            gridControlThongTinDonHang.DataSource = dataSource;
            gridControlThongTinDonHang.RefreshDataSource();
            view.RowStyle += (s, z) =>
            {
                var gridView = s as BandedGridView;
                if (gridView == null) return;

                // Lấy dữ liệu của dòng hiện tại
                DataRow row = gridView.GetDataRow(z.RowHandle);
                if (row == null) return;

                // Kiểm tra nếu dòng được đánh dấu là dòng mới
                if (row.Table.Columns.Contains("CheckColumn") && row["CheckColumn"] != DBNull.Value && (bool)row["CheckColumn"] == false)
                {
                    z.Appearance.BackColor = System.Drawing.Color.Yellow; // Tô màu nền
                    z.Appearance.BackColor2 = System.Drawing.Color.White;    // Hiệu ứng gradient
                }
            };

        }

        private void ItemDelete_Click(object sender, EventArgs e)
        {
            BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            if (view.SelectedRowsCount > 0)
            {
                int selectedIndex = view.GetSelectedRows()[0];
                DataRow row = view.GetDataRow(selectedIndex);
                if (row != null)
                {
                    ((DataTable)gridControlThongTinDonHang.DataSource).Rows.Remove(row);
                }
                view.RefreshData();
            }
        }
        private void AddRow(string data, int rowHandle)
        {
            //BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            //if (data == string.Empty) return;
            //string[] rowData = data.Split('\t');
            //int column = view.FocusedColumn.VisibleIndex;
            //for (int i = 0; i < rowData.Length; i++)
            //{
            //    if (i >= view.VisibleColumns.Count) break;
            //    if (rowData[i] == "-")
            //        rowData[i] = "0";
            //    view.SetRowCellValue(rowHandle, view.VisibleColumns[column + i], rowData[i].Replace(",", ""));
            //}
            BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            if (data == string.Empty) return;
            string[] rowData = data.Split('\t');
            int column = view.FocusedColumn.VisibleIndex;
            bool hasShownError = false;
            for (int i = 0; i < rowData.Length; i++)
            {
                if (i >= view.VisibleColumns.Count) break;
                if (rowData[i] == "-")
                    rowData[i] = "0";

                var currentColumn = view.VisibleColumns[column + i];

                if (currentColumn.FieldName.Contains("@Size@"))
                {
                    string cellValue = rowData[i].Replace(",", "");

                    if (decimal.TryParse(cellValue, out decimal numericValue))
                    {
                        view.SetRowCellValue(rowHandle, currentColumn, cellValue);
                    }
                }
                else
                {
                    view.SetRowCellValue(rowHandle, currentColumn, rowData[i].Replace(",", ""));
                }
            }
        }
        private string ClipboardData
        {
            get
            {
                IDataObject iData = Clipboard.GetDataObject();
                if (iData == null) return "";

                if (iData.GetDataPresent(DataFormats.Text))
                    return (string)iData.GetData(DataFormats.Text);
                return "";
            }
            set
            {
                Clipboard.SetDataObject(value);
            }
        }
        private void BandedView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (e.Value == null) return;
            if (string.IsNullOrEmpty(e.Value.ToString()) && e.Column.FieldName != "GhiChu") return;
            if (_status == ResourceURL.EventStatus.View)
            {
                BandedGridView view = (BandedGridView)sender;
                GridView views = (GridView)sender;
                string POID = view.GetFocusedRowCellValue(view.Columns["POID"]).ToString();
                string PO = view.GetFocusedRowCellValue(view.Columns["PO"]).ToString();
                string DauSize = view.GetFocusedRowCellValue(view.Columns["DauSize"]).ToString();
                string DauSizeID = view.GetFocusedRowCellValue(view.Columns["DauSizeID"]).ToString();
                string ColorCode = view.GetFocusedRowCellValue(view.Columns["ColorCode"]).ToString();
                string MaMau = view.GetFocusedRowCellValue(view.Columns["MaMau"]).ToString();
                int amount = 0;
                if (e.Column.FieldName == "NgayGH" || e.Column.FieldName == "NgayDKVC" || e.Column.FieldName == "NgayThucTeVC" || e.Column.FieldName == "NgayXuatHang" || e.Column.FieldName == "MaQG" || e.Column.FieldName == "GhiChu" || e.Column.FieldName == "ColorCode")
                {
                    bool allowMota = true;

                    if (dhTongPOChiTiet.Count > 0)
                    {
                        foreach (BandedGridColumn col in view.Columns)
                        {
                            string[] arrMTName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                            if (arrMTName.Length == 1) continue;
                            string sizeMTID = arrMTName[0];

                            BangSizeEntity bangsizeMT = (from l in lstBangSize
                                                         where l.MaHang == donHangTong.MaHang.ToString() && l.SizeSanXuat == sizeMTID.ToString() && l.MaNhomSize == DauSizeID
                                                         select l).FirstOrDefault();
                            if (bangsizeMT == null) continue;

                            foreach (DonHangTongPOChiTietEntity editObj in dhTongPOChiTiet)
                            {
                                if (editObj.POID == POID && editObj.MaMau == MaMau && editObj.DauSizeID == DauSizeID)
                                {
                                    if (e.Column.FieldName == "GhiChu")
                                        editObj.GhiChu = e.Value.ToString();
                                    allowMota = false;
                                }

                            }
                        }
                    }
                    DonHangTongPOEntity addDHTPO = new DonHangTongPOEntity();
                    addDHTPO.MaDH = txtMaDH.Text.ToString();
                    addDHTPO.POID = POID;
                    addDHTPO.PO = PO;
                    addDHTPO.ColorCode = ColorCode;
                    addDHTPO.MaMau = MaMau;
                    addDHTPO.DauSizeID = DauSizeID;
                    addDHTPO.DauSize = DauSize;
                    addDHTPO.MaQG = view.GetFocusedRowCellValue(view.Columns["MaQG"]).ToString();
                    addDHTPO.NgayGH = Convert.ToDateTime(view.GetFocusedRowCellValue(view.Columns["NgayGH"]));
                    addDHTPO.GhiChu = view.GetFocusedRowCellValue(view.Columns["GhiChu"]).ToString();
                    if (dhTongPO.Count > 0)
                    {
                        foreach (DonHangTongPOEntity editDHTPO in dhTongPO)
                        {
                            if (editDHTPO.MaDH == view.GetFocusedRowCellValue(view.Columns["MaDH"]).ToString() && editDHTPO.POID == view.GetFocusedRowCellValue(view.Columns["POID"]).ToString() && editDHTPO.ColorCode == view.GetFocusedRowCellValue(view.Columns["ColorCode"]).ToString()
                                && editDHTPO.MaMau == view.GetFocusedRowCellValue(view.Columns["MaMau"]).ToString() && editDHTPO.DauSizeID == view.GetFocusedRowCellValue(view.Columns["DauSizeID"]).ToString())
                            {
                                editDHTPO.MaQG = view.GetFocusedRowCellValue(view.Columns["MaQG"]).ToString();
                                editDHTPO.NgayGH = Convert.ToDateTime(view.GetFocusedRowCellValue(view.Columns["NgayGH"]));
                                editDHTPO.GhiChu = view.GetFocusedRowCellValue(view.Columns["GhiChu"]).ToString();
                            }
                        }
                    }
                    for (int i = 17; i < view.Columns.Count; i++)
                    {
                        if (view.GetRowCellValue(e.RowHandle, view.Columns[i]).ToString() != "")
                            addDHTPO.SoLuong += Convert.ToInt32(view.GetRowCellValue(e.RowHandle, view.Columns[i]));
                    }
                    DonHangTongPOEntity CheckAddListPOT = dhTongPO.Where(item => (item.MaDH + item.POID + item.ColorCode + item.MaMau + item.DauSizeID).ToUpper().Trim().ToString() == (txtMaDH.Text + POID + MaMau + DauSizeID).ToUpper().Trim().ToString()).FirstOrDefault();
                    if (CheckAddListPOT == null)
                    {
                        dhTongPO.Add(addDHTPO);
                    }
                    if (!allowMota) return;
                    foreach (BandedGridColumn col in view.Columns)
                    {
                        string[] arrMTName = col.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrMTName.Length == 1) continue;
                        string sizeMTID = arrMTName[0];

                        BangSizeEntity bangsizeMT = (from l in lstBangSize
                                                     where l.MaHang.ToString().Trim().ToUpper() == donHangTong.MaHang.ToString().Trim().ToUpper() && l.SizeSanXuat == sizeMTID.ToString() && l.MaNhomSize == DauSizeID
                                                     select l).FirstOrDefault();
                        if (bangsizeMT == null) continue;

                        DonHangTongPOChiTietEntity addMTObj = new DonHangTongPOChiTietEntity();
                        addMTObj.POID = POID;
                        addMTObj.PO = PO;
                        addMTObj.MaMau = MaMau;
                        addMTObj.DauSizeID = DauSizeID;
                        addMTObj.DauSize = DauSize;
                        addMTObj.SizeID = bangsizeMT.MaSize;
                        addMTObj.Size = bangsizeMT.SizeSanXuat;
                        int soLuong = 0;
                        if (int.TryParse(view.GetFocusedRowCellValue(view.Columns[col.FieldName]).ToString(), out soLuong))
                        {
                            addMTObj.SoLuong = soLuong;
                        }
                        else
                        {
                            addMTObj.SoLuong = soLuong;
                        }

                        object ghichu = view.GetFocusedRowCellValue(view.Columns["GhiChu"]);
                        if (ghichu != null && !string.IsNullOrEmpty(ghichu.ToString()))
                            addMTObj.GhiChu = ghichu.ToString();
                        DonHangTongPOChiTietEntity CheckAddListGC = dhTongPOChiTiet.Where(item => (item.POID + item.MaMau + item.DauSizeID + item.SizeID).ToUpper().Trim().ToString() == (POID + MaMau + DauSizeID + bangsizeMT.MaSize).ToUpper().Trim().ToString()).FirstOrDefault();
                        if (CheckAddListGC == null)
                        {
                            dhTongPOChiTiet.Add(addMTObj);
                        }
                    }
                    return;
                }

                string[] arrName = e.Column.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arrName.Length == 1 && e.Column.FieldName != "MaQG" && e.Column.FieldName != "NgayGH" && e.Column.FieldName != "GhiChu") return;
                string sizeID = arrName[0];

                BangSizeEntity bangsize = lstBangSize?
                     .FirstOrDefault(x =>
                         string.Equals((x.MaHang ?? "").Trim(), (donHangTong.MaHang ?? "").Trim(), StringComparison.OrdinalIgnoreCase)
                         && (x.SizeSanXuat ?? "") == (sizeID?.ToString() ?? "")
                         && (x.MaNhomSize ?? "") == (DauSizeID ?? "") && 
                         string.Equals((x.MaKH ?? "").Trim(),
                         (donHangTong.MaKH ?? "").Trim(), StringComparison.OrdinalIgnoreCase)
                     );

                if (bangsize == null) return;
                if (e.Value.ToString() != "")
                    amount = Convert.ToInt32(e.Value);


                bool allowEditAdd = true;
                if (!allowEditAdd) return;
                DonHangTongPOEntity addDHTPOCheck = new DonHangTongPOEntity();
                addDHTPOCheck.MaDH = txtMaDH.Text.ToString();
                addDHTPOCheck.POID = POID;
                addDHTPOCheck.PO = PO;
                addDHTPOCheck.ColorCode = ColorCode;
                addDHTPOCheck.MaMau = MaMau;
                addDHTPOCheck.DauSizeID = DauSizeID;
                addDHTPOCheck.DauSize = DauSize;
                addDHTPOCheck.MaQG = view.GetFocusedRowCellValue(view.Columns["MaQG"]).ToString();
                addDHTPOCheck.NgayGH = Convert.ToDateTime(view.GetFocusedRowCellValue(view.Columns["NgayGH"]));
                addDHTPOCheck.GhiChu = view.GetFocusedRowCellValue(view.Columns["GhiChu"]).ToString();
                if (dhTongPO.Count > 0)
                {
                    foreach (DonHangTongPOEntity editDHTPO in dhTongPO)
                    {
                        if (editDHTPO.MaDH == view.GetFocusedRowCellValue(view.Columns["MaDH"]).ToString() && editDHTPO.POID == view.GetFocusedRowCellValue(view.Columns["POID"]).ToString() && editDHTPO.ColorCode == view.GetFocusedRowCellValue(view.Columns["ColorCode"]).ToString()
                            && editDHTPO.MaMau == view.GetFocusedRowCellValue(view.Columns["MaMau"]).ToString() && editDHTPO.DauSizeID == view.GetFocusedRowCellValue(view.Columns["DauSizeID"]).ToString())
                        {
                            addDHTPOCheck.MaQG = view.GetFocusedRowCellValue(view.Columns["MaQG"]).ToString();
                            addDHTPOCheck.NgayGH = Convert.ToDateTime(view.GetFocusedRowCellValue(view.Columns["NgayGH"]));
                            addDHTPOCheck.GhiChu = view.GetFocusedRowCellValue(view.Columns["GhiChu"]).ToString();
                        }
                    }
                }
                for (int i = 17; i < view.Columns.Count; i++)
                {
                    if (view.GetRowCellValue(e.RowHandle, view.Columns[i]).ToString() != "")
                        addDHTPOCheck.SoLuong += Convert.ToInt32(view.GetRowCellValue(e.RowHandle, view.Columns[i]));
                }
                DonHangTongPOEntity CheckAddListPOGC = dhTongPO.Where(item => (item.MaDH + item.POID + item.ColorCode + item.MaMau + item.DauSizeID).ToUpper().Trim().ToString() == (txtMaDH.Text + POID + MaMau + DauSizeID).ToUpper().Trim().ToString()).FirstOrDefault();
                if (CheckAddListPOGC == null)
                {
                    dhTongPO.Add(addDHTPOCheck);
                }
                DonHangTongPOChiTietEntity addDHTPOCT = new DonHangTongPOChiTietEntity();
                addDHTPOCT.POID = view.GetFocusedRowCellValue(view.Columns["POID"]).ToString();
                addDHTPOCT.PO = view.GetFocusedRowCellValue(view.Columns["PO"]).ToString();
                addDHTPOCT.MaMau = view.GetFocusedRowCellValue(view.Columns["MaMau"]).ToString();
                addDHTPOCT.DauSizeID = view.GetFocusedRowCellValue(view.Columns["DauSizeID"]).ToString();
                addDHTPOCT.DauSize = view.GetFocusedRowCellValue(view.Columns["DauSize"]).ToString();
                addDHTPOCT.SizeID = bangsize.MaSize;
                addDHTPOCT.Size = bangsize.SizeSanXuat;
                if (dhTongPOChiTiet.Count > 0)
                {
                    foreach (DonHangTongPOChiTietEntity editDHTPOCT in dhTongPOChiTiet)
                    {
                        if (editDHTPOCT.POID == view.GetFocusedRowCellValue(view.Columns["POID"]).ToString() && editDHTPOCT.MaMau == view.GetFocusedRowCellValue(view.Columns["MaMau"]).ToString()
                            && editDHTPOCT.DauSizeID == view.GetFocusedRowCellValue(view.Columns["DauSizeID"]).ToString() && editDHTPOCT.SizeID == bangsize.MaSize)
                        {
                            editDHTPOCT.SoLuong = amount;
                            allowEditAdd = false;
                        }


                    }
                }
                addDHTPOCT.GhiChu = string.IsNullOrEmpty(view.GetFocusedRowCellValue(view.Columns["GhiChu"]).ToString()) ? null :
                    view.GetFocusedRowCellValue(view.Columns["GhiChu"]).ToString();
                DonHangTongPOChiTietEntity CheckAddList = dhTongPOChiTiet.Where(item => (item.POID + item.MaMau + item.DauSizeID + item.SizeID).ToUpper().Trim().ToString() == (POID + MaMau + DauSizeID + bangsize.MaSize).ToUpper().Trim().ToString()).FirstOrDefault();
                if (CheckAddList == null)
                {
                    addDHTPOCT.SoLuong = Convert.ToInt32(amount);
                    dhTongPOChiTiet.Add(addDHTPOCT);
                }
                if (!allowEditAdd) return;


            }

        }

        private void BandedView_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            try
            {
                BandedGridView view = (BandedGridView)sender;
                if (e.SummaryProcess == CustomSummaryProcess.Start)
                    sum = 0;
                if (e.SummaryProcess == CustomSummaryProcess.Finalize)
                {
                    foreach (BandedGridColumn col in view.Columns)
                    {
                        if (col.FieldName != "ID" && col.FieldName != "MaDH" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "ColorCode" && col.FieldName != "MaMau" && col.FieldName != "DauSizeID" && col.FieldName != "DauSize" && col.FieldName != "MaQG" && col.FieldName != "NgayGH" && col.FieldName != "NgayDKVC" && col.FieldName != "NgayThucTeVC" && col.FieldName != "NgayXuatHang" && col.FieldName != "GhiChu" && col.UnboundType == UnboundColumnType.Bound)
                        {
                            sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                        }
                    }
                    e.TotalValue = sum;
                }
            }
            catch (Exception ex)
            {

               
            }
            
        }

        private void BandedView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.IsGetData)
            {
                int sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                DataTable dt = row.Table;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "ID" && col.FieldName != "MaDH" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "ColorCode" && col.FieldName != "MaMau" && col.FieldName != "DauSizeID" && col.FieldName != "DauSize" && col.FieldName != "MaQG" && col.FieldName != "NgayGH" && col.FieldName != "NgayDKVC" && col.FieldName != "NgayThucTeVC" && col.FieldName != "NgayXuatHang" && col.FieldName != "GhiChu" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        try
                        {
                            if (!dt.Columns.Contains(col.FieldName))
                                continue;

                            if (row[col.FieldName] != null && !string.IsNullOrEmpty(row[col.FieldName].ToString()))
                            {
                                int value = Convert.ToInt32(row[col.FieldName]);
                                sum += value;
                            }
                            isEmpty = false;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
                if (!isEmpty)
                    e.Value = sum;
            }
        }

        private void BandedView_CustomDrawFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void BandedView_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "DauSize")
            {
                if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                {
                    e.DisplayText = "0";
                }

            }
            else
            {
                if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                {
                    e.DisplayText = "-";
                }
                else
                {
                    string[] arr = e.Column.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Count() > 1)
                    {
                        if (Convert.ToInt32(e.Value.ToString()) == 0)
                            e.DisplayText = "-";
                    }
                }
            }
        }

        private void BandedView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (e.Band == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), e.Band.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void SetGridBandedViewAmount(BandedGridView bandedView, string GridBandHeader, string GridBandCaption, List<string> columnNames)
        {
            GridBand gridBand = new GridBand();
            if (GridBandHeader == "")
                gridBand.Caption = GridBandCaption;
            else
                gridBand.Caption = GridBandHeader;
            int nrOfColumns = columnNames.Count;
            gridBand.Fixed = FixedStyle.None;
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            if (nrOfColumns == 1 && (columnNames[0] == "MaDH" || columnNames[0] == "POID" || columnNames[0] == "PO" || columnNames[0] == "ColorCode" || columnNames[0] == "MaMau" || columnNames[0] == "DauSizeID" || columnNames[0] == "DauSize" || columnNames[0] == "MaQG" || columnNames[0] == "NgayGH" || columnNames[0] == "NgayDKVC" || columnNames[0] == "NgayThucTeVC"|| columnNames[0] == "NgayXuatHang" || columnNames[0] == "GhiChu"))
            {
                BandedGridColumn bandedColumns = (BandedGridColumn)bandedView.Columns.AddField(columnNames[0]);

                if (columnNames[0] == "MaQG")
                {
                    string url = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
                    rCountryEdit.DataSource = tbl;
                    rCountryEdit.DisplayMember = "TenQG";
                    rCountryEdit.ValueMember = "MaQG";
                    rCountryEdit.ShowClearButton = false;
                    rCountryEdit.NullText = "[Chọn giá trị]";

                    GridView dvView = rCountryEdit.View;
                    if (dvView.Columns.Count == 0)
                    {
                        dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                        dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                        dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                        dvView.Columns.Add(new GridColumn { FieldName = "MaQG", Caption = "Mã quốc gia", Name = "colMaQG", Visible = false });
                        dvView.Columns.Add(new GridColumn { FieldName = "TenQG", Caption = "Tên quốc gia", Name = "colTenQG", Visible = true });

                    }
                    bandedColumns.ColumnEdit = rCountryEdit;
                }
                else if (columnNames[0] == "NgayGH")
                {
                    bandedColumns.DisplayFormat.FormatString = "dd/MM/yyyy";
                    bandedColumns.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;

                    // Thêm phần EditMask để khi edit vẫn là dd/MM/yyyy
                    bandedColumns.ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
                    var dateEdit = (DevExpress.XtraEditors.Repository.RepositoryItemDateEdit)bandedColumns.ColumnEdit;
                    dateEdit.Mask.EditMask = "dd/MM/yyyy";
                    dateEdit.Mask.UseMaskAsDisplayFormat = true;
                    dateEdit.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dateEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    dateEdit.EditFormat.FormatString = "dd/MM/yyyy";
                }
                else if (columnNames[0] == "NgayDKVC")
                {
                    bandedColumns.DisplayFormat.FormatString = "dd/MM/yyyy";
                    bandedColumns.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    bandedColumns.ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
                    var dateEdit = (DevExpress.XtraEditors.Repository.RepositoryItemDateEdit)bandedColumns.ColumnEdit;
                    dateEdit.Mask.EditMask = "dd/MM/yyyy";
                    dateEdit.Mask.UseMaskAsDisplayFormat = true;
                    dateEdit.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dateEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    dateEdit.EditFormat.FormatString = "dd/MM/yyyy";

                }
                else if (columnNames[0] == "NgayThucTeVC")
                {
                    bandedColumns.DisplayFormat.FormatString = "dd/MM/yyyy";
                    bandedColumns.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    bandedColumns.ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
                    var dateEdit = (DevExpress.XtraEditors.Repository.RepositoryItemDateEdit)bandedColumns.ColumnEdit;
                    dateEdit.Mask.EditMask = "dd/MM/yyyy";
                    dateEdit.Mask.UseMaskAsDisplayFormat = true;
                    dateEdit.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dateEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    dateEdit.EditFormat.FormatString = "dd/MM/yyyy";
                }
                else if (columnNames[0] == "NgayXuatHang")
                {
                    bandedColumns.DisplayFormat.FormatString = "dd/MM/yyyy";
                    bandedColumns.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    bandedColumns.ColumnEdit = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
                    var dateEdit = (DevExpress.XtraEditors.Repository.RepositoryItemDateEdit)bandedColumns.ColumnEdit;
                    dateEdit.Mask.EditMask = "dd/MM/yyyy";
                    dateEdit.Mask.UseMaskAsDisplayFormat = true;
                    dateEdit.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dateEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                    dateEdit.EditFormat.FormatString = "dd/MM/yyyy";
                }
                else
                {
                    if (columnNames[0] == "MaMau")
                    {
                        string urlMau = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                        DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                        RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
                        rCountryEdit.DataSource = tbl;
                        rCountryEdit.DisplayMember = "TenMau";
                        rCountryEdit.ValueMember = "MaMau";
                        rCountryEdit.ShowClearButton = false;
                        rCountryEdit.NullText = "[Chọn giá trị]";
                        rCountryEdit.EditValueChanged += new System.EventHandler(this.searchLookUpEditMau_EditValueChanged);

                        GridView dvView = rCountryEdit.View;
                        if (dvView.Columns.Count == 0)
                        {
                            dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                            dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                            dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                            dvView.Columns.Add(new GridColumn { FieldName = "MaMau", Caption = "Mã màu", Name = "colMaMau", Visible = false });
                            dvView.Columns.Add(new GridColumn { FieldName = "TenMau", Caption = "Tên màu", Name = "colTenMau", Visible = true });
                            dvView.Columns.Add(new GridColumn { FieldName = "CodeMau", Caption = "Code màu", Name = "colTenMau", Visible = true });
                            GridColumn colGroup = new GridColumn
                            {
                                FieldName = "TheMau",
                                Caption = "",
                                Name = "colTheMau",
                                Visible = true
                            };
                            dvView.Columns.Add(colGroup);

                            colGroup.GroupIndex = 0;
                            dvView.OptionsView.ShowGroupPanel = false;
                            dvView.OptionsBehavior.AutoExpandAllGroups = true;
                            dvView.GroupFormat = "{1}";
                        }
                        bandedColumns.ColumnEdit = rCountryEdit;
                        //bandedColumns.OptionsColumn.AllowEdit = false;
                    }
                    if (columnNames[0] == "DauSizeID")
                    {
                        string urlDSize = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeamv2");
                        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSize); }).Result;
                        DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                        RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
                        rCountryEdit.DataSource = tbl;
                        rCountryEdit.DisplayMember = "DauSize";
                        rCountryEdit.ValueMember = "DauSizeID";
                        rCountryEdit.ShowClearButton = false;
                        rCountryEdit.NullText = "[Chọn giá trị]";
                        rCountryEdit.EditValueChanged += new System.EventHandler(this.searchLookUpEditDauSize_EditValueChanged);

                        GridView dvView = rCountryEdit.View;
                        if (dvView.Columns.Count == 0)
                        {
                            dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                            dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                            dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                            dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                            dvView.Columns.Add(new GridColumn { FieldName = "DauSizeID", Caption = "Đầu size", Name = "colDauSize", Visible = false });
                            dvView.Columns.Add(new GridColumn { FieldName = "DauSize", Caption = "InSeam", Name = "colDauSizeTen", Visible = true });
                            GridColumn colGroup = new GridColumn
                            {
                                FieldName = "TheInSeam",
                                Caption = "",
                                Name = "colTheInSeam",
                                Visible = false
                            };
                            dvView.Columns.Add(colGroup);

                            colGroup.GroupIndex = 0;
                            dvView.OptionsView.ShowGroupPanel = false;
                            dvView.OptionsBehavior.AutoExpandAllGroups = true;
                            dvView.GroupFormat = "{1}";
                        }
                        bandedColumns.ColumnEdit = rCountryEdit;
                        //bandedColumns.OptionsColumn.AllowEdit = false;
                    }
                    if ( columnNames[0] == "MaDH" || columnNames[0] == "POID" || columnNames[0] == "PO" || columnNames[0] == "ColorCode" || columnNames[0] == "MaMau" || columnNames[0] == "DauSizeID" || columnNames[0] == "DauSize")
                    {
                        bandedColumns.OptionsColumn.AllowEdit = false;
                    }
                }

                String CaptionName = columnNames[0];
                bandedColumns.OwnerBand = gridBand;
                bandedColumns.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                bandedColumns.Caption = GridBandCaption;
                if (columnNames[0] == "MaDH" || columnNames[0] == "POID" || columnNames[0] == "DauSize")
                {
                    gridBand.Visible = false;
                }
                bandedColumns.Visible = true;
                bandedColumns.Width = 120;

                gridBand.Fixed = FixedStyle.Left;
                gridBand.RowCount = 1;
                bandedView.Bands.Add(gridBand);

            }
            else
            {
                BandedGridColumn[] bandedColumns = new BandedGridColumn[nrOfColumns];
                GridBand[] grHeader = new GridBand[nrOfColumns];
                for (int i = 0; i < nrOfColumns; i++)
                {
                    String[] _colName = columnNames[i].Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    GridBand gridband3 = new GridBand();
                    gridband3.Caption = _colName[0];
                    bandedColumns[i] = (BandedGridColumn)bandedView.Columns.AddField(columnNames[i]);
                    bandedColumns[i].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    bandedColumns[i].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedColumns[i].DisplayFormat.FormatString = "{0:##,0}";
                    bandedColumns[i].Width = 60;
                    gridband3.Columns.Add(bandedColumns[i]);
                    bandedColumns[i].OwnerBand = gridband3;
                    bandedColumns[i].Visible = true;
                    gridband3.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    gridband3.AppearanceHeader.Options.UseFont = true;
                    gridband3.AppearanceHeader.Options.UseTextOptions = true;
                    gridband3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    grHeader[i] = gridband3;

                }
                gridBand.Children.AddRange(grHeader);
                bandedView.Bands.Add(gridBand);
            }
        }
        private void searchLookUpEditDauSize_EditValueChanged(object sender, EventArgs e)
        {
            var rCountryEdit = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (rCountryEdit == null) return;
            DevExpress.XtraGrid.Views.Grid.GridView gridView = (DevExpress.XtraGrid.Views.Grid.GridView)rCountryEdit.Properties.View;
            int focusedRowHandle = gridView.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                DevExpress.XtraGrid.Views.Grid.GridView gridViews = (DevExpress.XtraGrid.Views.Grid.GridView)gridControlThongTinDonHang.MainView;
                DataRow focusedRow = gridView.GetDataRow(focusedRowHandle);
                if (focusedRow != null)
                {
                    gridViews.SetRowCellValue(gridViews.FocusedRowHandle, "DauSize", focusedRow["DauSize"].ToString());
                }
            }
        }
        private void searchLookUpEditMau_EditValueChanged(object sender, EventArgs e)
        {
            //SendKeys.SendWait("{ENTER}");
            //this.ActiveControl = this.txtMaDH;

            //string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
            string urlMau = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
            string jsonmau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
            DataTable tblmau = JsonConvert.DeserializeObject<DataTable>(jsonmau);
            var rCountryEdit = sender as DevExpress.XtraEditors.SearchLookUpEdit;
            if (rCountryEdit == null) return;
            DevExpress.XtraGrid.Views.Grid.GridView gridView = (DevExpress.XtraGrid.Views.Grid.GridView)rCountryEdit.Properties.View;
            int focusedRowHandle = gridView.FocusedRowHandle;
            if (focusedRowHandle >= 0)
            {
                DevExpress.XtraGrid.Views.Grid.GridView gridViews = (DevExpress.XtraGrid.Views.Grid.GridView)gridControlThongTinDonHang.MainView;
                DataRow focusedRow = gridView.GetDataRow(focusedRowHandle);
                if (focusedRow != null)
                {
                    gridViews.SetRowCellValue(gridViews.FocusedRowHandle, "MaMau", focusedRow["MaMau"].ToString());
                    var rowctmau = tblmau.AsEnumerable().FirstOrDefault(x => x["MaMau"].ToString() == focusedRow["MaMau"].ToString());
                    if (rowctmau != null) 
                    {
                        gridViews.SetRowCellValue(gridViews.FocusedRowHandle, "ColorCode", rowctmau["CodeMau"].ToString());
                    }
                    
                }
            }
        }

        private void BandedGridViewThongTinDonHang_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            try
            {
                GridView gridView = sender as GridView;
                int num = 0;
                Console.WriteLine("ValidatingEditor");
                if (gridView.GetFocusedDataSourceRowIndex() >= 0)
                {
                    List<string> lstCheckDuplicate = new List<string>() { "PO", "DauSize", "MaMau" };
                    if (lstCheckDuplicate.IndexOf(gridView.FocusedColumn.FieldName) >= 0)
                    {
                        if (string.IsNullOrEmpty(e.Value.ToString()))
                        {
                            e.Valid = false;
                            e.ErrorText = "Thông tin không được để trống.!";
                        }
                        else if (CheckDuplicateData(gridView, e, gridView.FocusedColumn.FieldName))
                        {
                            //e.Valid = false;
                            //e.ErrorText = "Dữ liệu bị trùng PO, DauSize, MaMau.!";
                            this.Luu.Enabled = false;
                            MessageBox.Show("Dữ liệu nhập vào trùng thông tin: PO, Màu, Đầu size !", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            this.Luu.Enabled = true;
                        }
                    }

                }
                if (gridView.FocusedColumn.FieldName.Contains("@"))
                {
                    string _filename = gridView.FocusedColumn.FieldName.ToString();
                    string _poid = gridView.GetRowCellValue(gridView.FocusedRowHandle, "POID").ToString();
                    string _sizeId = _filename.Split(new string[] { "@Size@" }, StringSplitOptions.RemoveEmptyEntries)[1].ToString();
                    string _dauSize = gridView.GetRowCellValue(gridView.FocusedRowHandle, "DauSizeID").ToString();
                    string _mamau = gridView.GetRowCellValue(gridView.FocusedRowHandle, "MaMau").ToString();
                    string urlcd = string.Format("{0}?maDH={1}", URL + "DonHangTong/GetCD", donHangTong.MaDH);
                    string jsondcd = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcd); }).Result;
                    DataTable tbcd = JsonConvert.DeserializeObject<DataTable>(jsondcd);
                    DataTable tblsua = UnPivot(gridControlThongTinDonHang.DataSource as DataTable);
                    DataRow rowFocused = (gridView.GetFocusedRow() as DataRowView).Row;

                    if (!Int32.TryParse(e.Value as String, out num))
                    {
                        e.Valid = false;
                        e.ErrorText = "Hãy nhập số nguyên";
                    }
                    else
                    {
                        if (Convert.ToInt32(e.Value) < 0)
                        {
                            e.Valid = false;
                            e.ErrorText = "Số lượng phải lớn hơn 0";
                        }

                        if (tbcd != null && tbcd.Rows.Count > 0)
                        {
                            foreach (DataRow rowCd in tbcd.Rows)
                            {
                                string poidCd = rowCd["POID"].ToString();
                                string sizeIdCd = rowCd["SizeID"].ToString();
                                string dauSizeCd = rowCd["DauSizeID"].ToString();
                                string mamauCd = rowCd["MaMau"].ToString();
                                int soLuongCd = Convert.ToInt32(rowCd["SoLuong"]);

                                if (poidCd == _poid && sizeIdCd == _sizeId && dauSizeCd == _dauSize && mamauCd == _mamau)
                                {
                                    if (Convert.ToInt32(e.Value) < soLuongCd)
                                    {
                                        e.Valid = false;
                                        e.ErrorText = "Không được nhập nhỏ hơn số lượng đã chia sx: " + soLuongCd;
                                        return;
                                    }
                                    break;
                                }

                            }

                        }
                        //string filterExpression = $"POID = '{_poid}' AND MaMau = '{_mamau}' AND DauSizeID = '{_dauSize} AND SizeID = '{_sizeId}'";
                        //string filterExpression = $"POID = '{_poid}' AND MaMau = '{_mamau}' AND DauSizeID = '{_dauSize}'";
                        //DataRow[] filteredRows = tbcd.Select(filterExpression);
                        //DataRow foundRow = filteredRows[0];
                        //int SLKT = Convert.ToInt32(foundRow[_filename]);

                    }
                }
            }
            catch(Exception ex)
            {

            }
            
        }

        // Thông tin: PO, Đầu size, Mã màu không được đồng thời trùng
        private bool CheckDuplicateData(GridView gridView, BaseContainerValidateEditorEventArgs e, string fieldName)
        {
            //lstCheckDuplicate.Remove(fieldName);
            //bool flagCheck = false;
            DataTable tbl = (gridView.DataSource as DataView).Table;
            DataRow dataRowFocused = (gridView.GetFocusedRow() as DataRowView).Row;
            DataTable tblTemp = tbl.Copy();

            tblTemp.Rows.RemoveAt(tblTemp.Rows.Count - 1);

            dataRowFocused[fieldName] = e.Value;
            DataRow dr = tblTemp.AsEnumerable().Where(x =>
                x["PO"].ToString().Equals(dataRowFocused["PO"].ToString()) &&
                x["DauSize"].ToString().Equals(dataRowFocused["DauSize"].ToString()) &&
                x["MaMau"].ToString().Equals(dataRowFocused["MaMau"].ToString())
                ).FirstOrDefault();

            if (dr != null)
            {
                return true;
            }
            else
            {
                return false;
            }


            //for (int i = 0; i < tbl.Rows.Count - 1; i++)
            //{
            //    bool flagCheckColumn = true;
            //    DataRow dataRow = tbl.Rows[i];
            //    if (!dataRow[fieldName].ToString().Equals(e.Value.ToString()))
            //    {
            //        flagCheckColumn = false;
            //    }
            //    else
            //    {
            //        foreach (string columnName in lstCheckDuplicate)
            //        {
            //            bool checkDup = dataRow[columnName].ToString().Equals(dataRowFocused[columnName].ToString());
            //            if (!checkDup)
            //            {
            //                flagCheckColumn = false;
            //                break;
            //            }
            //        }
            //    }

            //    if (flagCheckColumn)
            //    {
            //        flagCheck = true;
            //    }
            //}
            //return flagCheck;
        }
        private void BandedGridViewThongTinDonHang_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
            {
                e.DisplayText = "-";
            }
            else
            {
                string[] arr = e.Column.FieldName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Count() > 1)
                {
                    if (Convert.ToInt32(e.Value.ToString()) == 0)
                        e.DisplayText = "-";
                }
            }
        }
        #region
        //private void createColBandsSize(DataTable _thongTinDonHang)
        //{
        //    try
        //    {
        //        if (lstSize != null && lstSize.Count > 0)
        //        {
        //            lstSize.Clear();
        //        }
        //        gridBandSize.Children.Clear();
        //        bandedGridViewThongTinDonHang.Columns.Clear();
        //        foreach (DataColumn dc in _thongTinDonHang.Columns)
        //        {
        //            if (dc.ColumnName.Contains("@Size@"))
        //            {
        //                string[] arrName = dc.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
        //                string colName = arrName[2];
        //                lstSize.Add(colName.Replace("@Size@", ""));
        //                //colName.Replace("@Size", "");
        //                BandedGridColumn col = new BandedGridColumn();
        //                col.AppearanceCell.Options.UseTextOptions = true;
        //                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                col.AppearanceHeader.Options.UseTextOptions = true;
        //                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                col.Caption = colName.Replace("@Size@", "");
        //                col.FieldName = dc.ColumnName;
        //                col.Name = "col" + colName;
        //                col.OptionsColumn.AllowEdit = true;
        //                col.Visible = true;
        //                col.Width = 70;
        //                col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
        //                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        //                col.DisplayFormat.FormatString = "{0:##,0}";
        //                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
        //                bandedGridViewThongTinDonHang.Columns.AddRange(new BandedGridColumn[] { col });
        //                GridBand gb = new GridBand();
        //                gb.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        //                gb.AppearanceHeader.Options.UseFont = true;
        //                gb.AppearanceHeader.Options.UseTextOptions = true;
        //                gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                //
        //                gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(223)))), ((int)(((byte)(253)))));
        //                gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
        //                gb.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
        //                gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
        //                gb.AppearanceHeader.Options.UseBackColor = true;
        //                gb.AppearanceHeader.Options.UseForeColor = true;
        //                gb.Caption = colName.Replace("@Size@", "");
        //                gb.Columns.Add(col);
        //                gb.Name = "gb" + "Size_" + col;
        //                gb.VisibleIndex = 0;
        //                gb.Width = 60;
        //                gridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });


        //            }
        //            else if (dc.ColumnName.ToString().ToLower() == "poid")
        //            {
        //                BandedGridColumn colPoID = new BandedGridColumn();
        //                colPoID.AppearanceCell.Options.UseTextOptions = true;
        //                colPoID.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colPoID.AppearanceHeader.Options.UseTextOptions = true;
        //                colPoID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colPoID.Caption = dc.ColumnName.Replace("@Size@", "");
        //                colPoID.FieldName = dc.ColumnName;
        //                colPoID.Name = "col" + dc.ColumnName;
        //                colPoID.OptionsColumn.AllowEdit = false;
        //                colPoID.Visible = true;
        //                colPoID.Width = 50;
        //                bandedGridViewThongTinDonHang.Columns.AddRange(new BandedGridColumn[] { colPoID });
        //                gridBandPOID.Columns.Add(colPoID);
        //            }
        //            else if (dc.ColumnName.ToString().ToLower() == "po")
        //            {
        //                BandedGridColumn colPo = new BandedGridColumn();
        //                colPo.AppearanceCell.Options.UseTextOptions = true;
        //                colPo.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colPo.AppearanceHeader.Options.UseTextOptions = true;
        //                colPo.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colPo.Caption = dc.ColumnName.Replace("@Size@", "");
        //                colPo.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
        //                colPo.FieldName = dc.ColumnName;
        //                colPo.Name = "col" + dc.ColumnName;
        //                colPo.OptionsColumn.AllowEdit = false;
        //                colPo.Visible = true;
        //                colPo.Width = 50;
        //                bandedGridViewThongTinDonHang.Columns.AddRange(new BandedGridColumn[] { colPo });
        //                gridBandPO.Columns.Add(colPo);
        //            }
        //            else if (dc.ColumnName.ToString().ToLower() == "mamau")
        //            {
        //                BandedGridColumn colMaMau = new BandedGridColumn();
        //                colMaMau.AppearanceCell.Options.UseTextOptions = true;
        //                colMaMau.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colMaMau.AppearanceHeader.Options.UseTextOptions = true;
        //                colMaMau.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colMaMau.Caption = dc.ColumnName.Replace("@Size@", "");
        //                colMaMau.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
        //                colMaMau.FieldName = dc.ColumnName;
        //                colMaMau.Name = "col" + dc.ColumnName;
        //                colMaMau.OptionsColumn.AllowEdit = false;
        //                colMaMau.Visible = true;
        //                colMaMau.Width = 50;
        //                colMaMau.ColumnEdit = searchLookUpEditMau1;
        //                bandedGridViewThongTinDonHang.Columns.AddRange(new BandedGridColumn[] { colMaMau });
        //                gridBandMaMau.Columns.Add(colMaMau);
        //            }
        //            else if (dc.ColumnName.ToString().ToLower() == "dausize")
        //            {
        //                BandedGridColumn colDauSize = new BandedGridColumn();
        //                colDauSize.AppearanceCell.Options.UseTextOptions = true;
        //                colDauSize.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colDauSize.AppearanceHeader.Options.UseTextOptions = true;
        //                colDauSize.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colDauSize.Caption = dc.ColumnName.Replace("@Size@", "");
        //                colDauSize.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
        //                colDauSize.FieldName = dc.ColumnName;
        //                colDauSize.Name = "col" + dc.ColumnName;
        //                colDauSize.OptionsColumn.AllowEdit = false;
        //                colDauSize.Visible = true;
        //                colDauSize.Width = 50;
        //                bandedGridViewThongTinDonHang.Columns.AddRange(new BandedGridColumn[] { colDauSize });
        //                gridBandDauSize.Columns.Add(colDauSize);
        //            }
        //            else if (dc.ColumnName.ToString().ToLower() == "dausizeid")
        //            {
        //                BandedGridColumn colDauSizeID = new BandedGridColumn();
        //                colDauSizeID.AppearanceCell.Options.UseTextOptions = true;
        //                colDauSizeID.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colDauSizeID.AppearanceHeader.Options.UseTextOptions = true;
        //                colDauSizeID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colDauSizeID.Caption = dc.ColumnName.Replace("@Size@", "");
        //                colDauSizeID.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
        //                colDauSizeID.FieldName = dc.ColumnName;
        //                colDauSizeID.Name = "col" + dc.ColumnName;
        //                colDauSizeID.OptionsColumn.AllowEdit = false;
        //                colDauSizeID.Visible = true;
        //                colDauSizeID.Width = 50;
        //                bandedGridViewThongTinDonHang.Columns.AddRange(new BandedGridColumn[] { colDauSizeID });
        //                gridBandDauSizeID.Columns.Add(colDauSizeID);
        //            }
        //            else if (dc.ColumnName.ToString().ToLower() == "madh")
        //            {
        //                BandedGridColumn colMaDH = new BandedGridColumn();
        //                colMaDH.AppearanceCell.Options.UseTextOptions = true;
        //                colMaDH.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colMaDH.AppearanceHeader.Options.UseTextOptions = true;
        //                colMaDH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colMaDH.Caption = dc.ColumnName.Replace("@Size@", "");
        //                colMaDH.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
        //                colMaDH.FieldName = dc.ColumnName;
        //                colMaDH.Name = "col" + dc.ColumnName;
        //                colMaDH.OptionsColumn.AllowEdit = false;
        //                colMaDH.Visible = true;
        //                colMaDH.Width = 50;
        //                bandedGridViewThongTinDonHang.Columns.AddRange(new BandedGridColumn[] { colMaDH });
        //                gridBandMaDH.Columns.Add(colMaDH);
        //            }
        //            else if (dc.ColumnName.ToString().ToLower() == "maqg")
        //            {
        //                BandedGridColumn colMaQG = new BandedGridColumn();
        //                colMaQG.AppearanceCell.Options.UseTextOptions = true;
        //                colMaQG.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colMaQG.AppearanceHeader.Options.UseTextOptions = true;
        //                colMaQG.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colMaQG.Caption = dc.ColumnName.Replace("@Size@", "");
        //                colMaQG.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
        //                colMaQG.FieldName = dc.ColumnName;
        //                colMaQG.Name = "col" + dc.ColumnName;
        //                colMaQG.OptionsColumn.AllowEdit = true;
        //                colMaQG.Visible = true;
        //                colMaQG.Width = 50;
        //                colMaQG.ColumnEdit = searchLookUpEditQuocGia1;
        //                bandedGridViewThongTinDonHang.Columns.AddRange(new BandedGridColumn[] { colMaQG });
        //                gridBandMaQuocGia.Columns.Add(colMaQG);
        //            }
        //            else if (dc.ColumnName.ToString().ToLower() == "ngaygh")
        //            {
        //                BandedGridColumn colNgayGH = new BandedGridColumn();
        //                colNgayGH.AppearanceCell.Options.UseTextOptions = true;
        //                colNgayGH.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colNgayGH.AppearanceHeader.Options.UseTextOptions = true;
        //                colNgayGH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colNgayGH.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        //                colNgayGH.DisplayFormat.FormatString = "dd/MM/yyyy";
        //                colNgayGH.Caption = dc.ColumnName.Replace("@Size@", "");
        //                colNgayGH.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
        //                colNgayGH.OptionsColumn.AllowEdit = true;
        //                colNgayGH.FieldName = dc.ColumnName;
        //                colNgayGH.Name = "col" + dc.ColumnName;
        //                colNgayGH.Visible = true;
        //                colNgayGH.Width = 50;
        //                bandedGridViewThongTinDonHang.Columns.AddRange(new BandedGridColumn[] { colNgayGH });
        //                gridBandNgayGH.Columns.Add(colNgayGH);
        //            }
        //            else if (dc.ColumnName.ToString().ToLower() == "ghichu")
        //            {
        //                BandedGridColumn colGhiChu = new BandedGridColumn();
        //                colGhiChu.AppearanceCell.Options.UseTextOptions = true;
        //                colGhiChu.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colGhiChu.AppearanceHeader.Options.UseTextOptions = true;
        //                colGhiChu.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //                colGhiChu.Caption = dc.ColumnName.Replace("@Size@", "");
        //                colGhiChu.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
        //                colGhiChu.FieldName = dc.ColumnName;
        //                colGhiChu.Name = "col" + dc.ColumnName;
        //                colGhiChu.OptionsColumn.AllowEdit = true;
        //                colGhiChu.Visible = true;
        //                colGhiChu.Width = 50;
        //                bandedGridViewThongTinDonHang.Columns.AddRange(new BandedGridColumn[] { colGhiChu });
        //                gridBandGhiChu.Columns.Add(colGhiChu);
        //            }
        //        }
        //        GridBand gridBand = new GridBand();
        //        gridBand.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        //        gridBand.AppearanceHeader.Options.UseFont = true;
        //        gridBand.AppearanceHeader.Options.UseTextOptions = true;
        //        gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        //        gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        //        BandedGridColumn colAmount = bandedGridViewThongTinDonHang.Columns.AddField("Amount");

        //        colAmount.OptionsColumn.AllowEdit = false;
        //        colAmount.Caption = "Tổng";
        //        colAmount.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
        //        //colAmount.UnboundExpression = exp;
        //        colAmount.Visible = true;
        //        colAmount.OwnerBand = gridBand;
        //        gridBand.Visible = true;
        //        gridBand.Caption = "Tổng";
        //        GridColumnSummaryItem item = colAmount.Summary.Add(SummaryItemType.Custom);
        //        item.FieldName = "Amount";
        //        item.DisplayFormat = "{0:n0}";
        //        bandedGridViewThongTinDonHang.Bands.Add(gridBand);

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString());
        //    }
        //}
        #endregion
        private DataTable createTablePOChiTiet()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("Size", typeof(string));
            tbl.Columns.Add("SoLuong", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            return tbl;
        }

        private DataTable UnPivot(DataTable tblThongTonDonHang)
        {
            DataTable tblSave = createTablePOChiTiet();
            if (!tblSave.Columns.Contains("MaHang"))
            {
                tblSave.Columns.Add("MaHang", typeof(string));
            }
            if (tblThongTonDonHang == null || tblThongTonDonHang.Rows.Count == 0) return null;
            foreach (DataRow dataRows in tblThongTonDonHang.Rows)
            {
                foreach (DataColumn column in tblThongTonDonHang.Columns)
                {
                    if (column.ColumnName.Contains("Size@"))
                    {
                        Console.WriteLine("dataRows");
                        //tblSave[""]
                        DataRow dataRowSave = tblSave.NewRow();
                        dataRowSave["MaHang"] =searchLookUpEditMH.EditValue.ToString();
                        dataRowSave["POID"] = dataRows["POID"];
                        dataRowSave["PO"] = dataRows["PO"];
                        dataRowSave["MaMau"] = dataRows["MaMau"];
                        dataRowSave["DauSize"] = string.IsNullOrEmpty(dataRows["DauSize"].ToString())?"0": dataRows["DauSize"].ToString();
                        dataRowSave["DauSizeID"] = string.IsNullOrEmpty(dataRows["DauSizeID"].ToString()) ? "0" : dataRows["DauSizeID"].ToString() ;
                        dataRowSave["GhiChu"] = dataRows["GhiChu"];
                        dataRowSave["SizeID"] = column.ColumnName.Split(new string[] { "@Size@" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        dataRowSave["Size"] = column.ColumnName.Split(new string[] { "@Size@" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        dataRowSave["SoLuong"] = dataRows[column];
                        tblSave.Rows.Add(dataRowSave);
                    }
                }
            }
            Console.WriteLine("end for");
            return tblSave;
        }

        private void BtSave()
        {
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            this.ActiveControl = this.txtMaDH;
            DataTable tblTTDonHangTong = gridControlThongTinDonHang.DataSource as DataTable;
            DataTable tblunit = UnPivot(gridControlThongTinDonHang.DataSource as DataTable);
            bool checkDuplicateData = tblunit.AsEnumerable().GroupBy(row => new
            {
                DauSize = row.Field<string>("DauSize"),
                POID = row.Field<string>("POID"),
                PO = row.Field<string>("PO"),
                MaMau = row.Field<string>("MaMau"),
                Size = row.Field<string>("Size"),
                SizeID = row.Field<string>("SizeID")
            }).Any(g => g.Count() > 1);
            if (checkDuplicateData)
            {
                XtraMessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (tblunit.Columns.Contains("CheckColumn"))
            {
                tblunit.Columns.Remove("CheckColumn");
            }

            if (tblunit.Columns.Contains("IsNew"))
            {
                tblunit.Columns.Remove("IsNew");
            }
          
            //string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
            //string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
            //DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);

            //var missingRows = from row1 in tblunit.AsEnumerable()
            //                  where !dtSize.AsEnumerable().Any(row2 =>
            //                      row1["MaHang"].ToString() == row2["MaHang"].ToString() &&
            //                      row1["Size"].ToString() == row2["SizeSanXuat"].ToString() &&
            //                      row1["DauSize"].ToString() == row2["NhomSize"].ToString())
            //                  select row1;
            //if (missingRows.Any())
            //{
            //    // Nhóm các dòng bị thiếu theo NhomSize
            //    var groupedMissing = missingRows
            //        .GroupBy(row => row["DauSize"].ToString())
            //        .Select(group => new
            //        {
            //            NhomSize = group.Key, // Tên nhóm size
            //                Sizes = group.Select(row => row["Size"].ToString()).Distinct() // Các size bị thiếu
            //            });

            //    // Tạo chuỗi thông báo
            //    var message = string.Join("\n", groupedMissing.Select(group =>
            //        $"Nhóm {group.NhomSize} : thiếu các size:  {string.Join(", ", group.Sizes)}  thuộc mã hàng {searchLookUpEditMH.EditValue.ToString()}"));

            //    // Hiển thị thông báo
            //    MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            if (_status == ResourceURL.EventStatus.View)
            {
                DataTable tbl1 = gridControlThongTinDonHang.DataSource as DataTable;
                LuuDong();
                SaveDHTongPOChiTietAdd(tbl1);
                SaveDonHangTongPOAdd(tbl1);
            }
            else if (_status == ResourceURL.EventStatus.Add)
            {
                //string SoBooking = txtSoBooing.Text?.ToString();
                //if (string.IsNullOrEmpty(SoBooking))
                //{
                //    XtraMessageBox.Show("Chưa nhập Số booking. Vui lòng nhập Số booking.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                //    return;
                //}

                // Check chưa nhập: MaQG, PO ,MaMau, DauSize => Thông báo
                //DataRow newDataRow = tbl.Rows[tbl.Rows.Count -1];
                DataTable tbl = gridControlThongTinDonHang.DataSource as DataTable;
                List<DataRow> newDataRow = new List<DataRow>();
                
                if (tbl.Columns.Contains("IsNew")) 
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        if (row["IsNew"] != DBNull.Value && (bool)row["IsNew"] == true)
                        {
                            newDataRow.Add(row);
                        }
                    }
                }

                // Chắc chắn newDataRow đã có 4 cột: MaQG, PO, MaMau, DauSize
                // => Để không bị lỗi khi lấy ra giá trị tại các cột
                //if (string.IsNullOrEmpty(newDataRow["PO"].ToString()))
                //{
                //    XtraMessageBox.Show("Chưa nhập PO .Vui lòng nhập PO.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                //    return;
                //}
                //else if (string.IsNullOrEmpty(newDataRow["MaMau"].ToString()))
                //{
                //    XtraMessageBox.Show("Chưa chọn Màu. Vui lòng chọn Màu.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                //    return;
                //}
                //else if (string.IsNullOrEmpty(newDataRow["DauSize"].ToString()))
                //{
                //    XtraMessageBox.Show("Chưa nhập Đầu Size. Vui lòng nhập Đầu Size.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                //    return;
                //}
                if (string.IsNullOrEmpty(newDataRow[0]["MaQG"].ToString()))
                {
                    XtraMessageBox.Show("Chưa chọn Quốc Gia. Vui lòng chọn Quốc Gia.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    return;
                }
                //string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
                //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                //List<BangMauEntity> lstBangMau = JsonConvert.DeserializeObject<List<BangMauEntity>>(json);
                //List<string> lstSaveMau = new List<string>();
                //BangMauEntity bangMauKT = lstBangMau.Where(item => item.MaMau.ToString().Trim().Replace(" ", "").ToUpper().Trim() == (newDataRow["MaMau"].ToString().Trim().Replace(" ", "")).ToUpper().Trim()).FirstOrDefault();
                //BangMauEntity bangMau = lstBangMau.Where(item => item.MaMau.ToString().Trim().Replace(" ", "") + item.MaHang.ToUpper().Trim() == (newDataRow["MaMau"].ToString().Trim().Replace(" ", "") + searchLookUpEditMH.EditValue.ToString().Trim()).ToUpper().Trim()).FirstOrDefault();
                //if (bangMau == null)
                //{
                //    lstSaveMau.Add(bangMauKT.TenMau);
                //}

                //if (lstSaveMau.Count > 0)
                //{
                //    AutoImportMau(lstSaveMau);
                //}

                List<DataRow> allRows = tbl.Rows.Cast<DataRow>().ToList();
                //bool flagSave = SaveDataTongPOChiTietAdd(tbl, newDataRow);
                bool flagSave = SaveDataTongPOChiTietAdd(tbl, allRows);
                if (flagSave)
                {
                    SaveDonHangTongPOAdd(tbl);
                    SaveDonHangTongAdd(tbl);
                }
                else
                {
                    return;
                }    
            }
            BtNapLai();
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }
        private void AutoImportMau(List<string> lstSaveMau)
        {
            List<BangMauEntity> lstAutoImportMau = new List<BangMauEntity>();
            BangSizeEntity autoImportSizeItem;
            string maHang = searchLookUpEditMH.EditValue.ToString().Trim();

            if (lstSaveMau != null && lstSaveMau.Count > 0)
            {
                lstSaveMau = lstSaveMau.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstSaveMau.Count; i++)
                {
                    BangMauEntity itemBangMau = new BangMauEntity(searchLookUpEditMH.EditValue.ToString(), lstSaveMau[i],"","");
                    lstAutoImportMau.Add(itemBangMau);
                }
                if (lstAutoImportMau.Count > 0)
                {
                    string urlAutoImportMau = string.Format("{0}?", URL + "BangMau/PostAutoImportBangMau");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlAutoImportMau, lstAutoImportMau); }).Result;
                }
            }
        }
        private void Luu_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (GlobleData.UserName.ToString().ToUpper() == donHangTong.NguoiTao.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
            {
                
                BtSave();
                DisabledLayoutControl(true);
            }
            else
            {
                XtraMessageBox.Show("User này không phải là người tạo, không có quyền sửa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void DisabledLayoutControl(bool value)
        {
            layoutControlItem7.Enabled = value;
            layoutControlItem14.Enabled = value;
            layoutControlItem10.Enabled = value;
            layoutControlItem9.Enabled = value;
            layoutControlItem12.Enabled = value;
            layoutControlItem13.Enabled = value;
            layoutControlItem11.Enabled = value;
        }

        private void BtThem()
        {
            DataTable tblthem = gridControlThongTinDonHang.DataSource as DataTable;
            // Disabled các thông tin của đơn hàng tổng và đơn hàng tổng PO
            // Khi đang thêm đơn hàng tổng PO chi tiết
            DisabledLayoutControl(false);
            //LuuDong();
            //this.Them.Enabled = false;

            if (!tblthem.Columns.Contains("IsNew"))
            {
                tblthem.Columns.Add("IsNew", typeof(bool));
            }
            _status = ResourceURL.EventStatus.Add;
            DataTable tbl = gridControlThongTinDonHang.DataSource as DataTable;
            DataRow newDataRow = tbl.Rows.Add();
            if (tbl != null && tbl.Rows.Count > 0)
            {
                foreach (DataColumn column in tbl.Columns)
                {
                    if (column.ColumnName.Contains("NgayGH"))
                    {
                        newDataRow["NgayGH"] = DateTime.Now.Date;
                    }
                    else if (column.ColumnName.Contains("PO"))
                    {
                        //newDataRow["PO"] = tbl.Rows[0]["PO"];
                    }
                }
            }
            newDataRow["IsNew"] = true;
            _rowAdd = tbl.Rows.Count - 1;
            //tbl.Rows.Add();
            gridControlThongTinDonHang.DataSource = tbl;
            gridControlThongTinDonHang.RefreshDataSource();
            tblDonHangChiTiet = gridControlThongTinDonHang.DataSource as DataTable;
        }

        private void Them_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (GlobleData.UserName.ToString().ToUpper() == donHangTong.NguoiTao.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
            {
                BtThem();
            }
            else
            {
                XtraMessageBox.Show("User này không phải là người tạo, không có quyền Thêm. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
          
        }

        private void BandedView_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            focused(sender);
        }

        private void BandedView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            focused(sender);
        }
        // Hàm focused dùng để ràng buộc focused vào cell nào và không được focused vào cell nào
        private void focused(object sender)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
            if (_status == ResourceURL.EventStatus.Add)
            {
                bool isNewRow = false;
                object isNewValue = view.GetRowCellValue(view.FocusedRowHandle, "IsNew");
                isNewRow = isNewValue != null && isNewValue != DBNull.Value && Convert.ToBoolean(isNewValue);

                //if (_rowAdd != -1 && (view.FocusedRowHandle == _rowAdd) && (!view.FocusedColumn.FieldName.Equals("Amount")) &&!(view.FocusedColumn.FieldName.Equals("ColorCode"))
                //    view.FocusedColumn.OptionsColumn.AllowEdit = true;
                //else
                //    view.FocusedColumn.OptionsColumn.AllowEdit = false;


                if (_rowAdd != -1 && (view.FocusedRowHandle == _rowAdd) || isNewRow)
                {
                    if ((!view.FocusedColumn.FieldName.Equals("Amount")) && !(view.FocusedColumn.FieldName.Equals("ColorCode")))
                    {
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;
                    }
                }
                else 
                {
                    view.FocusedColumn.OptionsColumn.AllowEdit = false;
                }
                    
            }
            else
            {
                if (_status == ResourceURL.EventStatus.View)
                {
                    if (view.FocusedColumn.FieldName.Equals("PO") || view.FocusedColumn.FieldName.Equals("DauSize") || view.FocusedColumn.FieldName.Equals("ColorCode"))
                        view.FocusedColumn.OptionsColumn.AllowEdit = false;
                    else
                        view.FocusedColumn.OptionsColumn.AllowEdit = true;
                }
            }
        }


        private List<DonHangTongEntity> GetCurrentData(int soLuong)
        {
            List<DonHangTongEntity> _lstDonHangTong = new List<DonHangTongEntity>();
            DonHangTongEntity _donHangTong = new DonHangTongEntity();
            _donHangTong.MaDH = txtMaDH.Text;
            if (searchLookUpEditMH.EditValue != null)
            _donHangTong.MaHang = searchLookUpEditMH.EditValue.ToString();
            _donHangTong.MaKH = searchLookUpEditKH.EditValue.ToString();
            _donHangTong.MaCL = searchLookUpEditCL.EditValue == null ? "" : searchLookUpEditCL.EditValue.ToString();
            _donHangTong.Dot = txtDot.Text;
            _donHangTong.NguoiTao = txtNguoiTao.Text;
            _donHangTong.NgayTao = dateEditNgayTao.DateTime;
            _donHangTong.VND = spinEditVND.Value;
            _donHangTong.CM = spinEditUSD.Value;
            _donHangTong.GhiChu = txtGhiChu.Text;

            _donHangTong.ID = donHangTong.ID;
            _donHangTong.SoLuong = soLuong;
            _donHangTong.TrangThai = donHangTong.TrangThai;

            _donHangTong.NguoiSua = GlobleData.UserName;
            _donHangTong.NgaySua = DateTime.Now;
            _donHangTong.MaCty = searchLookUpEditCTY.EditValue == null ? "" : searchLookUpEditCTY.EditValue.ToString();
            string BookingMaHang = txtSoBooing?.Text?.ToString();
            _donHangTong.BookingMaHang = string.IsNullOrEmpty(BookingMaHang) ? "" : BookingMaHang;
            _donHangTong.FOB = textEdit1.Text.ToString() == "" ? 0 : Convert.ToDecimal(textEdit1.Text);
            _donHangTong.HeSoDH = textEdit2.Text.ToString() == "" ? 0 : Convert.ToDecimal(textEdit2.Text);
            _donHangTong.HinhThucXuatHang = serachlookupeditHT.EditValue == null ? "" : serachlookupeditHT.EditValue.ToString();
            _lstDonHangTong.Add(_donHangTong);

            return _lstDonHangTong;
        }

        private DataTable createTableDonHangTong()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaQG", typeof(string));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("NgayGH", typeof(DateTime));
            tbl.Columns.Add("SoLuong", typeof(int));
            tbl.Columns.Add("GhiChu", typeof(string));
            return tbl;
        }

        private DataTable createTablePO()
        {
            DataTable tbl = new DataTable();
            tbl.Columns.Add("MaDH", typeof(string));
            tbl.Columns.Add("MaQG", typeof(string));
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("NgayGH", typeof(DateTime));
            tbl.Columns.Add("SoLuong", typeof(int));
            tbl.Columns.Add("GhiChu", typeof(string));
            return tbl;
        }

        private bool SaveDataTongPOChiTietAdd(DataTable tbl, List<DataRow> newDataRow)
        {
            bool flagSave = false;


            DataTable tblAddRow = tbl.Clone();

            //DataRow newataRow = tbl.Rows[tbl.Rows.Count - 1];

            //tblAddRow.ImportRow(newDataRow);

            foreach (DataRow row in newDataRow)
            {
                tblAddRow.ImportRow(row);
            }

            // Bảng tblAddRow đã chắc chắn có các cột PO, DauSize, MaQG

            if (!string.IsNullOrEmpty(tblAddRow.Rows[0]["PO"].ToString())
                && !string.IsNullOrEmpty(tblAddRow.Rows[0]["MaQG"].ToString())
                )
            {
                for (int i = 0; i < tblAddRow.Rows.Count; i++)
                {
                    string strDauSizeID = replaceSpecialCharacter((string.IsNullOrEmpty(tblAddRow.Rows[i]["DauSize"].ToString())?"0": tblAddRow.Rows[i]["DauSize"]) as string);
                    string strPOID = string.Format("{0}|{1}", txtMaDH.Text.ToString(), replaceSpecialCharacter(RemoveVietnameseTone((tblAddRow.Rows[i]["PO"]) as string)));
                    tblAddRow.Rows[i]["DauSizeID"] = strDauSizeID;
                    tblAddRow.Rows[i]["POID"] = strPOID;
                }


                DataTable tblUnpivot = UnPivot(tblAddRow);
                //DataColumn dataColumn = (tblUnpivot[0] as DataRow)
                if (tblUnpivot.Columns.Contains("MaHang"))
                {
                    tblUnpivot.Columns.Remove("MaHang");
                }
                foreach (DataRow row in tblUnpivot.Rows)
                {
                    if (string.IsNullOrEmpty(row["SoLuong"].ToString()))
                    {
                        row["SoLuong"] = 0;
                    }
                }
                //string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                //string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                //DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
                //var missingRows = from row1 in tblUnpivot.AsEnumerable()
                //                  where !dtSize.AsEnumerable().Any(row2 =>
                //                      searchLookUpEditMH.EditValue.ToString() == row2["MaHang"].ToString() &&
                //                      row1["Size"].ToString() == row2["SizeSanXuat"].ToString() &&
                //                      row1["DauSize"].ToString() == row2["NhomSize"].ToString())
                //                  select row1;
                //if (missingRows.Any())
                //{
                //    string sizesanxuatMissing = string.Join("; ", missingRows.Select(r => r["Size"].ToString()));
                //    string nhomsizeMissing = string.Join("; ", missingRows.Select(r => r["DauSize"].ToString()).Distinct().ToArray());
                //    MessageBox.Show("Size " + sizesanxuatMissing + " không có trong nhóm  " + nhomsizeMissing + " thuộc mã hàng  " + searchLookUpEditMH.EditValue.ToString() + " trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                //    return false;
                //}
                List<DonHangTongPOChiTietEntity> lst = JsonConvert.DeserializeObject<List<DonHangTongPOChiTietEntity>>(JsonConvert.SerializeObject(tblUnpivot));
                string urlSort = string.Format("{0}?madh={1}", URL + "DonHangTong/GetSort", txtMaDH.EditValue.ToString());
                string jsonSort = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSort); }).Result;
                DataTable dtSort = JsonConvert.DeserializeObject<DataTable>(jsonSort);
                int maxSort = 0;
                if (dtSort != null && dtSort.Rows.Count > 0)
                {
                    var sortValues = dtSort.AsEnumerable()
                        .Select(r =>
                        {
                            var val = r["Sort"];
                            if (val == null || val == DBNull.Value) return 0;
                            return Convert.ToInt32(val); // ép cứng sang int
                        });
                    maxSort = sortValues.Max();
                }

                // Biến dùng để tăng Sort cho các dòng mới
                int nextSort = maxSort + 1;
                foreach (var item in lst)
                {
                    var rowMatch = dtSort.AsEnumerable().FirstOrDefault(r =>
                            r["POID"].ToString() == item.POID &&
                            r["MaMau"].ToString() == item.MaMau &&
                            r["DauSizeID"].ToString() == item.DauSizeID &&
                            r["SizeID"].ToString() == item.SizeID
                        );
                    if (rowMatch != null)
                    {
                        item.Sort = Convert.ToInt32(rowMatch["Sort"]);
                    }
                    else
                    {
                        item.Sort = nextSort;
                        nextSort++;
                    }
                    item.MaKiemTra = txtMaDH.Text.ToString();

                }
                if (lst != null && lst.Count > 0)
                {
                    var distinctSizeRows = lst
                   .GroupBy(x => new { x.SizeID, x.DauSizeID })
                   .Select(g => g.First())
                   .ToList();
                    string urlDsTheSizect = string.Format("{0}", URL + $"ERPBangSize/Get?action=Getizev1");
                    string jsonDsTheSizect = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDsTheSizect); }).Result;

                    string urlDSTheInSeamct = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeamv1");
                    string jsonDSTheInSeamct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSTheInSeamct); }).Result;
                    //bangsize
                    string urlsizecheck = $"{URL}BangSize/GetBangSizebom?para={searchLookUpEditMH.EditValue.ToString()}&para1={searchLookUpEditKH.EditValue.ToString()}";
                    string jsonsizecheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlsizecheck); }).Result;
                    DataTable tblSizecheck = JsonConvert.DeserializeObject<DataTable>(jsonsizecheck);
                    DataTable TheSize = JsonConvert.DeserializeObject<DataTable>(jsonDsTheSizect);
                    DataTable TheInSeam = JsonConvert.DeserializeObject<DataTable>(jsonDSTheInSeamct);

                    var dictTheSize = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);
                    if (TheSize != null)
                    {
                        foreach (DataRow r in TheSize.Rows)
                        {
                            // cố gắng map bằng SizeSanXuat (hoặc MaSize nếu bạn có)
                            string key = (r["SizeSanXuat"]?.ToString() ?? "").Trim();
                            if (!dictTheSize.ContainsKey(key)) dictTheSize[key] = r;
                        }
                    }
                    DataTable tblsizeSave = CreateDatatableTheSize();
                    foreach (var rows in distinctSizeRows)
                    {
                        string sizeID = rows.SizeID.ToString();
                        string dauSizeID = rows.DauSizeID.ToString();
                        string sizeText = rows.Size?.ToString() ?? "";
                        string dauSizeText = rows.DauSize?.ToString() ?? "";
                        bool exists = tblSizecheck.AsEnumerable().Any(r =>
                            r["MaSize"].ToString() == sizeID &&
                            r["MaNhomSize"].ToString() == dauSizeID
                        );
                        if (!exists)
                        {

                            DataRow rowSize = tblsizeSave.NewRow();
                            rowSize["ID"] = 0;
                            rowSize["MaSize"] = sizeID;
                            rowSize["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                            rowSize["CodeSize"] = sizeText;
                            rowSize["TenSize"] = sizeText;
                            rowSize["SizeSanXuat"] = sizeText;
                            rowSize["GhiChu"] = "";
                            rowSize["XacNhan"] = "";
                            rowSize["SizeXacNhan"] = 0;
                            rowSize["MaNhomSize"] = dauSizeID;
                            rowSize["NhomSize"] = dauSizeText;
                            rowSize["MaKH"] = searchLookUpEditKH.EditValue.ToString();
                            rowSize["Sort"] = rows.Sort.ToString();


                            // ✅ lấy MaTheSize từ thư viện size
                            var rsTheSize = TheSize.AsEnumerable().FirstOrDefault(r =>
                                (r["SizeSanXuat"]?.ToString() ?? "").Trim().Equals(sizeText.Trim(), StringComparison.OrdinalIgnoreCase)
                            );

                            rowSize["MaTheSize"] = rsTheSize?["MaTheSize"]?.ToString() ?? "";

                            // ✅ lấy MaTheInSeam từ thư viện inseam (ưu tiên MaInSeam → InSeam)
                            var rsInSeam = TheInSeam.AsEnumerable().FirstOrDefault(r =>
                                (r["MaInSeam"]?.ToString() ?? "").Trim().Equals(dauSizeID.Trim(), StringComparison.OrdinalIgnoreCase)
                                || (r["InSeam"]?.ToString() ?? "").Trim().Equals(dauSizeText.Trim(), StringComparison.OrdinalIgnoreCase)
                            );

                            rowSize["MaTheInSeam"] = rsInSeam?["MaTheInSeam"]?.ToString() ?? "";

                            tblsizeSave.Rows.Add(rowSize);
                        }
                    }

                    if (tblsizeSave != null && tblsizeSave.Rows.Count > 0)
                    {
                        string urlpostsize = string.Format("{0}", URL + "DonHangTong/PostSize");
                        string msResultsize = Task.Run(async () => { return await _clientExtension.PostAsync(urlpostsize, tblsizeSave); }).Result;
                        if (msResultsize.ToLower() != "true")
                            XtraMessageBox.Show(msResultsize);
                    }


                    flagSave = true;
                    string url = string.Format("{0}", URL + "DonHangTong/PostDonHangTongPOChiTiet");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, lst); }).Result;
                    if (result == "true") // hoặc tùy biến server trả về
                    {
                        _unsavedSelectedSizes.Clear();
                    }
                }
            }

            return flagSave;

            //bool flagSave = false;
            //if (dhTongPOChiTiet != null && dhTongPOChiTiet.Count > 0)
            //{
            //    flagSave = true;
            //    string url = string.Format("{0}", URL + "DonHangTong/PostDonHangTongPOChiTiet");
            //    string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dhTongPOChiTiet); }).Result;
            //    Console.WriteLine(result);
            //}
            //return flagSave;
        }

        private void SaveDHTongPOChiTietAdd(DataTable tbl)
        {

            DataTable tblAddRow = tbl.Copy();
            string strPOID = string.Empty;

            // Bảng tblAddRow đã chắc chắn có các cột PO, DauSize, MaQG
            if (!string.IsNullOrEmpty(tblAddRow.Rows[0]["PO"].ToString())
                && !string.IsNullOrEmpty(tblAddRow.Rows[0]["DauSize"].ToString())
                && !string.IsNullOrEmpty(tblAddRow.Rows[0]["MaQG"].ToString())
                )
            {
                string strDauSizeID = replaceSpecialCharacter((string.IsNullOrEmpty(tblAddRow.Rows[0]["DauSize"].ToString())?"0": tblAddRow.Rows[0]["DauSize"]) as string);
                if (string.IsNullOrEmpty(tblAddRow.Rows[0]["POID"].ToString()) || tblAddRow.Rows[0]["POID"] == null)
                {
                    strPOID = string.Format("{0}|{1}", txtMaDH.Text.ToString(), replaceSpecialCharacter(RemoveVietnameseTone((tblAddRow.Rows[0]["PO"]) as string)));
                }
                else
                {
                    strPOID = tblAddRow.Rows[0]["POID"].ToString();
                }

                tblAddRow.Rows[0]["DauSizeID"] = strDauSizeID;
                tblAddRow.Rows[0]["POID"] = strPOID;

                DataTable tblUnpivot = UnPivot(tblAddRow);
                //DataColumn dataColumn = (tblUnpivot[0] as DataRow)

                foreach (DataRow row in tblUnpivot.Rows)
                {
                    if (string.IsNullOrEmpty(row["SoLuong"].ToString()))
                    {
                        row["SoLuong"] = 0;
                    }
                }
                //string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                //string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                //DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
                //var missingRows = from row1 in tblUnpivot.AsEnumerable()
                //                  where !dtSize.AsEnumerable().Any(row2 =>
                //                      searchLookUpEditMH.EditValue.ToString() == row2["MaHang"].ToString() &&
                //                      row1["Size"].ToString() == row2["SizeSanXuat"].ToString() &&
                //                      row1["DauSize"].ToString() == row2["NhomSize"].ToString())
                //                  select row1;
                //if (missingRows.Any())
                //{
                //    string sizesanxuatMissing = string.Join("; ", missingRows.Select(r => r["Size"].ToString()));
                //    string nhomsizeMissing = string.Join("; ", missingRows.Select(r => r["DauSize"].ToString()).Distinct().ToArray());
                //    MessageBox.Show("Size " + sizesanxuatMissing + " không có trong nhóm  " + nhomsizeMissing + " thuộc mã hàng  " + searchLookUpEditMH.EditValue.ToString() + " trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                //}

                List<DonHangTongPOChiTietEntity> lst = JsonConvert.DeserializeObject<List<DonHangTongPOChiTietEntity>>(JsonConvert.SerializeObject(tblUnpivot));
                string urlSort = string.Format("{0}?madh={1}", URL + "DonHangTong/GetSort", txtMaDH.EditValue.ToString());
                string jsonSort = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSort); }).Result;
                DataTable dtSort = JsonConvert.DeserializeObject<DataTable>(jsonSort);

                int maxSort = 0;
                if (dtSort != null && dtSort.Rows.Count > 0)
                {
                    var sortValues = dtSort.AsEnumerable()
                        .Select(r =>
                        {
                            var val = r["Sort"];
                            if (val == null || val == DBNull.Value) return 0;
                            return Convert.ToInt32(val); // ép cứng sang int
                        });
                    maxSort = sortValues.Max();
                }

                // Biến dùng để tăng Sort cho các dòng mới
                int nextSort = maxSort + 1;

                foreach (var item in lst)
                {
                    var rowMatch = dtSort.AsEnumerable().FirstOrDefault(r =>
                                    r["POID"].ToString() == item.POID &&
                                    r["MaMau"].ToString() == item.MaMau &&
                                    r["DauSizeID"].ToString() == item.DauSizeID &&
                                    r["SizeID"].ToString() == item.SizeID
                                );
                    if (rowMatch != null)
                    {
                        item.Sort = Convert.ToInt32(rowMatch["Sort"]);
                    }
                    else
                    {
                        item.Sort = nextSort;
                        nextSort++;
                    }
                    item.MaKiemTra = txtMaDH.Text.ToString();
                }
                if (lst != null && lst.Count > 0)
                {
                    var distinctSizeRows = lst
                       .GroupBy(x => new { x.SizeID, x.DauSizeID })
                       .Select(g => g.First())
                       .ToList();
                    string urlDsTheSizect = string.Format("{0}", URL + $"ERPBangSize/Get?action=Getizev1");
                    string jsonDsTheSizect = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDsTheSizect); }).Result;

                    string urlDSTheInSeamct = string.Format("{0}", URL + $"ERPBangInSeam/Get?action=GetInSeamv1");
                    string jsonDSTheInSeamct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSTheInSeamct); }).Result;
                    //bangsize
                    string urlsizecheck = $"{URL}BangSize/GetBangSizebom?para={searchLookUpEditMH.EditValue.ToString()}&para1={searchLookUpEditKH.EditValue.ToString()}";
                    string jsonsizecheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlsizecheck); }).Result;
                    DataTable tblSizecheck = JsonConvert.DeserializeObject<DataTable>(jsonsizecheck);
                    DataTable TheSize = JsonConvert.DeserializeObject<DataTable>(jsonDsTheSizect);
                    DataTable TheInSeam = JsonConvert.DeserializeObject<DataTable>(jsonDSTheInSeamct);

                    var dictTheSize = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);
                    if (TheSize != null)
                    {
                        foreach (DataRow r in TheSize.Rows)
                        {
                            // cố gắng map bằng SizeSanXuat (hoặc MaSize nếu bạn có)
                            string key = (r["SizeSanXuat"]?.ToString() ?? "").Trim();
                            if (!dictTheSize.ContainsKey(key)) dictTheSize[key] = r;
                        }
                    }
                    DataTable tblsizeSave = CreateDatatableTheSize();
                    foreach (var rows in distinctSizeRows)
                    {
                        string sizeID = rows.SizeID.ToString();
                        string dauSizeID = rows.DauSizeID.ToString();
                        string sizeText = rows.Size?.ToString() ?? "";
                        string dauSizeText = rows.DauSize?.ToString() ?? "";
                        bool exists = tblSizecheck.AsEnumerable().Any(r =>
                            r["MaSize"].ToString() == sizeID &&
                            r["MaNhomSize"].ToString() == dauSizeID
                        );
                        if (!exists)
                        {

                            DataRow rowSize = tblsizeSave.NewRow();
                            rowSize["ID"] = 0;
                            rowSize["MaSize"] = sizeID;
                            rowSize["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                            rowSize["CodeSize"] = sizeText;
                            rowSize["TenSize"] = sizeText;
                            rowSize["SizeSanXuat"] = sizeText;
                            rowSize["GhiChu"] = "";
                            rowSize["XacNhan"] = "";
                            rowSize["SizeXacNhan"] = 0;
                            rowSize["MaNhomSize"] = dauSizeID;
                            rowSize["NhomSize"] = dauSizeText;
                            rowSize["MaKH"] = searchLookUpEditKH.EditValue.ToString();
                            rowSize["Sort"] = rows.Sort.ToString();


                            // ✅ lấy MaTheSize từ thư viện size
                            var rsTheSize = TheSize.AsEnumerable().FirstOrDefault(r =>
                                (r["SizeSanXuat"]?.ToString() ?? "").Trim().Equals(sizeText.Trim(), StringComparison.OrdinalIgnoreCase)
                            );

                            rowSize["MaTheSize"] = rsTheSize?["MaTheSize"]?.ToString() ?? "";

                            // ✅ lấy MaTheInSeam từ thư viện inseam (ưu tiên MaInSeam → InSeam)
                            var rsInSeam = TheInSeam.AsEnumerable().FirstOrDefault(r =>
                                (r["MaInSeam"]?.ToString() ?? "").Trim().Equals(dauSizeID.Trim(), StringComparison.OrdinalIgnoreCase)
                                || (r["InSeam"]?.ToString() ?? "").Trim().Equals(dauSizeText.Trim(), StringComparison.OrdinalIgnoreCase)
                            );

                            rowSize["MaTheInSeam"] = rsInSeam?["MaTheInSeam"]?.ToString() ?? "";

                            tblsizeSave.Rows.Add(rowSize);
                        }
                    }

                    if (tblsizeSave != null && tblsizeSave.Rows.Count > 0)
                    {
                        string urlpostsize = string.Format("{0}", URL + "DonHangTong/PostSize");
                        string msResultsize = Task.Run(async () => { return await _clientExtension.PostAsync(urlpostsize, tblsizeSave); }).Result;
                        if (msResultsize.ToLower() != "true")
                            XtraMessageBox.Show(msResultsize);
                    }


                    string url = string.Format("{0}", URL + "DonHangTong/PostDonHangTongPOChiTiet");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, lst); }).Result;
                    if (result == "true") // hoặc tùy biến server trả về
                    {
                        _unsavedSelectedSizes.Clear();
                    }
                }
            }
        }

        private string replaceSpecialCharacter(string str)
        {
            char[] charArray = str.ToCharArray();
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsLetterOrDigit(charArray[i]))
                {
                    charArray[i] = '_';
                }
            }
            string newString = new string(charArray);
            return Regex.Replace(newString, @"_+", "_");
        }

        private void SaveDonHangTongPOAdd(DataTable tbl)
        {
            //DataTable tbl = gridControlThongTinDonHang.DataSource as DataTable;

            //DataTable tbl = gridControlThongTinDonHang.DataSource as DataTable;
            // tblNew được tạo ra để lưu cho bảng dbo.DonHangTongPO
            //DataTable tblNew = new DataTable();
            string urlMau = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
            string jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
            DataTable dtMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);

            if (tbl != null && tbl.Rows.Count > 0)
            {
                if (!tbl.Columns.Contains("SoLuong"))
                {
                    tbl.Columns.Add("SoLuong", typeof(int));
                }
                

                DataTable saveDonHangTongPO = tbl.Copy();

                saveDonHangTongPO.Rows[0]["MaDH"] = txtMaDH.Text.ToString();
                int i = 0;
                foreach (DataRow dataRow in saveDonHangTongPO.Rows)
                {
                    int soLuong = 0;
                    string strPOID = string.Empty;
                    foreach (DataColumn dataColumn in saveDonHangTongPO.Columns)
                    {
                        var clcode = dtMau.AsEnumerable().FirstOrDefault(f => f["MaMau"].ToString() == dataRow["MaMau"].ToString());
                        if (dataColumn.ColumnName.Contains("Size@"))
                        {
                            if (!string.IsNullOrEmpty(dataRow[dataColumn.ColumnName].ToString())
                                && dataColumn.ColumnName.Contains("Size@"))
                            {
                                soLuong += int.Parse(dataRow[dataColumn.ColumnName].ToString());
                            }
                        }
                        else if (dataColumn.ColumnName.Contains("MaDH"))
                        {
                            dataRow["MaDH"] = txtMaDH.Text.ToString();
                        }
                        else if (dataColumn.ColumnName.Contains("POID"))
                        {
                            //string strPOID = string.Format("{0}|{1}", txtMaDH.Text.ToString(), replaceSpecialCharacter((dataRow["PO"]) as string));//replaceSpecialCharacter((dataRow["PO"]) as string));
                            if (string.IsNullOrEmpty(dataRow["POID"].ToString()) || dataRow["PO"] == null)
                            {
                                strPOID = string.Format("{0}|{1}", txtMaDH.Text.ToString(), replaceSpecialCharacter(RemoveVietnameseTone((dataRow["PO"]) as string)));//replaceSpecialCharacter((dataRow["PO"]) as string));
                            }
                            else
                            {
                                strPOID = dataRow["POID"].ToString();
                            }
                            dataRow["POID"] = strPOID;
                        }
                        else if (dataColumn.ColumnName.Contains("DauSizeID"))
                        {
                            
                            string strDauSizeID = replaceSpecialCharacter((string.IsNullOrEmpty(dataRow["DauSize"].ToString()) ?"0": dataRow["DauSize"]) as string);
                            dataRow["DauSizeID"] = strDauSizeID;
                        }
                        //else if (dataColumn.ColumnName.Contains("ColorCode"))
                        //{
                        //    dataRow["ColorCode"] = string.IsNullOrEmpty(dataRow["ColorCode"].ToString())? clcode["TenMau"].ToString() : dataRow["ColorCode"];
                        //}
                    }
                    dataRow["SoLuong"] = soLuong;
                    i += 1;
                }

                ConvertColumnsToDateTime(saveDonHangTongPO, "NgayDKVC", "NgayThucTeVC", "NgayXuatHang");

                List<DonHangTongPOEntity> lstTongPO = JsonConvert.DeserializeObject<List<DonHangTongPOEntity>>(JsonConvert.SerializeObject(saveDonHangTongPO));
                //Console.WriteLine("lst: " + lstTongPO);
                if (lstTongPO != null && lstTongPO.Count > 0)
                {
                    var MauRows = lstTongPO
                     .GroupBy(row => new { row.MaMau, row.ColorCode })
                     .Select(group => group.First())
                     .ToList();

                    string urlDSTheMauct = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                    string jsonDSTheMauct = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSTheMauct); }).Result;

                    string urlmaucheck = $"{URL}BangMau/GetBangMau3?para={searchLookUpEditMH.EditValue.ToString()}&para2={searchLookUpEditKH.EditValue.ToString()}";
                    string jsonmaucheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlmaucheck); }).Result;

                    DataTable tblmaucheck = JsonConvert.DeserializeObject<DataTable>(jsonmaucheck);
                    DataTable themau = JsonConvert.DeserializeObject<DataTable>(jsonDSTheMauct);
                    DataTable tbmauSave = CreateDatatableTheMau();
                    //bangmau
                    foreach (var rowm in MauRows)
                    {
                        var tenmau = themau.AsEnumerable().FirstOrDefault(r => r["MaMau"].ToString() == rowm.MaMau.ToString());
                        string mamau = rowm.MaMau.ToString();
                        string colorCode = rowm.ColorCode?.ToString().Trim() ?? mamau;

                        bool existsm = tblmaucheck.AsEnumerable().Any(r =>
                            r["MaMau"].ToString() == mamau
                        );
                        if (!existsm)
                        {
                            DataRow newRow = tbmauSave.NewRow();
                            newRow["ID"] = 0;
                            newRow["MaMau"] = mamau;
                            newRow["MaHang"] = searchLookUpEditMH.EditValue.ToString();
                            newRow["TenMau"] = tenmau["TenMau"];
                            newRow["CodeMau"] = tenmau["CodeMau"];
                            newRow["GhiChu"] = "";
                            newRow["MaKH"] = searchLookUpEditKH.EditValue.ToString();

                            var thuVienMauRow = themau.AsEnumerable()
                                .FirstOrDefault(r => (r["MaMau"]?.ToString() ?? "").Trim()
                                    .Equals(mamau, StringComparison.OrdinalIgnoreCase));

                            if (thuVienMauRow != null)
                            {
                                newRow["MaTheMau"] =
                                    thuVienMauRow["MaTheMau"]?.ToString() ??
                                    thuVienMauRow["TheMau"]?.ToString() ??
                                    "";
                            }
                            else
                            {
                                newRow["MaTheMau"] = "";
                            }
                            tbmauSave.Rows.Add(newRow);
                        }

                    }

                    if (tbmauSave != null && tbmauSave.Rows.Count > 0)
                    {
                        string urlpostmau = string.Format("{0}?", URL + "DonHangTong/PostMau");
                        string msResultmau = Task.Run(async () => { return await _clientExtension.PostAsync(urlpostmau, tbmauSave); }).Result;
                        if (msResultmau.ToLower() != "true")
                            XtraMessageBox.Show(msResultmau);
                    }


                    string url = string.Format("{0}", URL + "DonHangTong/PostDonHangTongPO");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, lstTongPO); }).Result;
                    Console.WriteLine(result);
                }
            }
        }

        private void SaveDonHangTongAdd(DataTable tbl, bool isDelete = false, string _poID = "", string _maMau = "", string _dauSizeID = "")
        {

            //DataTable tbl = gridControlThongTinDonHang.DataSource as DataTable;

            if (isDelete && !tbl.Columns.Contains("SoLuong"))
            {
                tbl.Columns.Add("SoLuong", typeof(int));
            }

            if (tbl != null && tbl.Rows.Count > 0)
            {
                string SoBooking = txtSoBooing.Text?.ToString();
                //if (string.IsNullOrEmpty(SoBooking))
                //{
                //    XtraMessageBox.Show("Chưa nhập Số booking. Vui lòng nhập Số booking.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                //    return;
                //}

                DataTable saveDonHangTong = tbl.Copy();

                saveDonHangTong.Rows[0]["MaDH"] = txtMaDH.Text.ToString();
                saveDonHangTong.Rows[0]["BookingMaHang"] = SoBooking;
                int i = 0;
                foreach (DataRow dataRow in saveDonHangTong.Rows)
                {
                    int soLuong = 0;
                    foreach (DataColumn dataColumn in saveDonHangTong.Columns)
                    {
                        if (dataColumn.ColumnName.Contains("Size@"))
                        {
                            if (!string.IsNullOrEmpty(dataRow[dataColumn.ColumnName].ToString())
                                && dataColumn.ColumnName.Contains("@Size@"))
                            {
                                soLuong += int.Parse(dataRow[dataColumn.ColumnName].ToString());
                            }

                        }
                    }
                    dataRow["SoLuong"] = soLuong;
                    i += 1;
                }


                ConvertColumnsToDateTime(saveDonHangTong, "NgayDKVC", "NgayThucTeVC", "NgayXuatHang");

                List<DonHangTongPOEntity> lstDonHangTongPO = JsonConvert.DeserializeObject<List<DonHangTongPOEntity>>(JsonConvert.SerializeObject(saveDonHangTong));
                int soLuongTong = lstDonHangTongPO.Sum(item => item.SoLuong);
                List<DonHangTongEntity> _lstDonHangTong = GetCurrentData(soLuongTong);
                if (_lstDonHangTong != null && _lstDonHangTong.Count > 0)
                {
                    string url = string.Format("{0}?", URL + "DonHangTong/PostDonHangTong");
                    string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _lstDonHangTong); }).Result;
                }
            }
            if ((tbl == null || (tbl != null && tbl.Rows.Count == 0)) &&
                !(string.IsNullOrEmpty(_poID) && string.IsNullOrEmpty(_maMau) && string.IsNullOrEmpty(_dauSizeID)))
            {
                // Xóa DonHangTong
                string _maDH = txtMaDH.Text.ToString();
                string url = string.Format("{0}/?madonhang={1}", URL + "DonHangTong/Delete", _maDH);
                string json = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                this.Close();
            }
        }

        private void LuuDong()
        {
            try
            {
                string SoBooking = txtSoBooing.Text?.ToString();
                //if (string.IsNullOrEmpty(SoBooking))
                //{
                //    XtraMessageBox.Show("Chưa nhập Số booking. Vui lòng nhập Số booking.!", Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                //    return;
                //}

                int TotalSL = 0;
                GridView gridView = (GridView)gridControlThongTinDonHang.MainView;
                DataTable dtb = gridControlThongTinDonHang.DataSource as DataTable;
                for (int i = 0; i < gridView.RowCount; i++)
                {
                    int sum = 0;
                    for (int j = 0; j < gridView.Columns.Count; j++)
                    {
                        if (gridView.Columns[j].FieldName.Contains('@'))
                        {
                            if (gridView.GetRowCellValue(i, gridView.Columns[j]).ToString() != "")
                                sum += Convert.ToInt32(gridView.GetRowCellValue(i, gridView.Columns[j]));
                        }

                    }
                    TotalSL += sum;
                }
                DonHangTongEntity addDHT = new DonHangTongEntity();
                addDHT.MaDH = txtMaDH.Text.ToString();
                addDHT.MaHang = searchLookUpEditMH.EditValue.ToString();
                addDHT.MaKH = searchLookUpEditKH.EditValue.ToString();
                addDHT.MaCL = searchLookUpEditCL.EditValue == null ? "" : searchLookUpEditCL.EditValue.ToString();
                addDHT.MaHS = txtMaHS.Text;
                addDHT.NguoiTao = txtNguoiTao.Text.ToString();

                addDHT.NgayTao = DateTime.Now;
                addDHT.NgaySua = DateTime.Now;
                addDHT.NguoiSua = GlobleData.UserName;
                addDHT.VND = spinEditVND.Value;
                addDHT.CM = spinEditUSD.Value;
                addDHT.Dot = txtDot.Text;
                addDHT.TrangThai = donHangTong.TrangThai;
                addDHT.SoLuong = TotalSL;
                addDHT.SoVoice = txtSoVoice.Text;
                addDHT.GhiChu = txtGhiChu.Text;
                addDHT.MaCty = searchLookUpEditCTY.EditValue == null ? "" : searchLookUpEditCTY.EditValue.ToString();
                addDHT.BookingMaHang = SoBooking;
                addDHT.FOB = string.IsNullOrEmpty(textEdit1.Text.ToString()) ? 0 : Convert.ToDecimal(textEdit1.Text.ToString());
                addDHT.HeSoDH = string.IsNullOrEmpty(textEdit2.Text.ToString()) ? 0 : Convert.ToDecimal(textEdit2.Text.ToString());
                addDHT.HinhThucXuatHang = serachlookupeditHT.EditValue == null ? "" : serachlookupeditHT.EditValue.ToString();
                dhTong.Add(addDHT);

                string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
                if (dtSize == null && dtSize.Rows.Count < 0)
                {
                    MessageBox.Show("Size thuộc mã hàng  " + searchLookUpEditMH.EditValue.ToString().ToString() + " chưa được khai báo trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                    return;
                }

                

                if (dhTong.Count > 0)
                {
                    string urlDonHangTong = string.Format("{0}?", URL + "DonHangTong/PostDonHangTong");
                    string msDonHangTong = Task.Run(async () => { return await _clientExtension.PostAsync(urlDonHangTong, dhTong); }).Result;
                    if (msDonHangTong.ToLower() != "true")
                        XtraMessageBox.Show(msDonHangTong);
                }
                //if (dhTongPO.Count > 0)
                //{
                //    string urlDonHangTongPO = string.Format("{0}?", URL + "DonHangTong/PostDonHangTongPO");
                //    string msDonHangTongPO = Task.Run(async () => { return await _clientExtension.PostAsync(urlDonHangTongPO, dhTongPO); }).Result;
                //    if (msDonHangTongPO.ToLower() != "true")
                //        XtraMessageBox.Show(msDonHangTongPO);
                //}
                //if (dhTongPOChiTiet.Count > 0)
                //{
                //    string urlDonHangTongPOChiTiet = string.Format("{0}?", URL + "DonHangTong/PostDonHangTongPOChiTiet");
                //    string msDonHangTongPOChiTiet = Task.Run(async () => { return await _clientExtension.PostAsync(urlDonHangTongPOChiTiet, dhTongPOChiTiet); }).Result;
                //    if (msDonHangTongPOChiTiet.ToLower() != "true")
                //        XtraMessageBox.Show(msDonHangTongPOChiTiet);
                //}

                dhTong.Clear();
                dhTongPO.Clear();
                dhTongPOChiTiet.Clear();
                clsWaitForm.ShowSuccessForm(this, 3000);
                this.Close();

                this.Them.Enabled = true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lưu đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void txtGhiChu_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtSoVoice_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtMaHS_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void txtDot_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Kiểm tra nếu ký tự là chữ cái thì chuyển thành chữ hoa
            if (char.IsLetter(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }

        private void Xoa_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (GlobleData.UserName.ToString().ToUpper() == donHangTong.NguoiTao.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
            {
                try
                {
                    Console.WriteLine("Xoa");
                    DataTable tbl = gridControlThongTinDonHang.DataSource as DataTable;
                    BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
                    int foucsedDelete = view.FocusedRowHandle;
                    if (foucsedDelete >= 0)
                    {
                        DataRow row = view.GetFocusedDataRow();




                        if (row != null)
                        {
                            string _po = row["PO"].ToString();
                            string _poID = row["POID"].ToString();
                            string _maMau = row["MaMau"].ToString();
                            string _dauSizeID = row["DauSizeID"].ToString();

                            string urlislsx = string.Format("{0}?madh={1}", URL + "CanDoiDonHangTong/GetIsLSX", txtMaDH.Text.ToString());
                            string jsonislsx = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlislsx); }).Result;
                            DataTable tblIsLSX = JsonConvert.DeserializeObject<DataTable>(jsonislsx);
                            if (tblIsLSX != null && tblIsLSX.Rows.Count > 0)
                            {
                                XtraMessageBox.Show(string.Format("PO: {0} đã được lên lệnh sản xuất, không thể thực hiện xóa!", _po),
                                Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                                return;
                            }

                            DialogResult result = MessageBox.Show("Xác nhận xóa PO: " + _po, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {

                                // Tạo DataTable Xóa
                                List<DonHangTongPOChiTietEntity> lstDonHangTongPOChiTiet = new List<DonHangTongPOChiTietEntity>();
                                lstDonHangTongPOChiTiet.Add(new DonHangTongPOChiTietEntity(_poID, _maMau, _dauSizeID));

                                DataTable tblXoa = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(lstDonHangTongPOChiTiet));

                                List<DonHangTongPOEntity> lstDeleteTongPO = new List<DonHangTongPOEntity>();

                                string _maDH = txtMaDH.Text.ToString();

                                lstDeleteTongPO.Add(new DonHangTongPOEntity(_maDH, _poID, _maMau, _dauSizeID));

                                DataTable tblDHPO = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(lstDeleteTongPO));
                                // GetCanDoiDonHang
                                string urlGetCanDoi = string.Format("{0}", URL + "DonHangTong/GetPOCanDoi");
                                string jsonCanDoi = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetCanDoi, tblDHPO); }).Result;
                                DataTable tblCanDoi = JsonConvert.DeserializeObject<DataTable>(jsonCanDoi);
                                //Console.WriteLine("GetCanDoiDonHang");
                                if (tblCanDoi != null && tblCanDoi.Rows.Count > 0)
                                {
                                    int isKeoVe = int.Parse(tblCanDoi.Rows[0]["IsKeoVe"].ToString());
                                    switch (isKeoVe)
                                    {
                                        case 0:
                                            result = MessageBox.Show(string.Format("PO: {0} đã được chia sản xuất. Bạn có chắc muốn xóa Không?", _po),
                                                "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                            if (result == DialogResult.Yes)
                                            {
                                                // Xóa DonHangTongPOChiTiet
                                                tbl.Rows.RemoveAt(foucsedDelete);
                                                XoaDonHangTongPOChiTiet(tblXoa, _poID, _maMau, _dauSizeID);
                                            }
                                            break;
                                        case 1:
                                            XtraMessageBox.Show(string.Format("PO: {0} đã được lấy về sản xuất, không thể thực hiện xóa!", _po),
                                                Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                                            break;
                                    }
                                    return;
                                }

                                // Xóa DonHangTongPOChiTiet
                                tbl.Rows.RemoveAt(foucsedDelete);
                                XoaDonHangTongPOChiTiet(tblXoa, _poID, _maMau, _dauSizeID);
                            }
                        }
                        else
                        {
                            XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("User này không phải là người tạo, không có quyền Xóa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
          
        }

        private void XoaDonHangTongPOChiTiet(DataTable tblXoa, string _poID, string _maMau, string _dauSizeID)
        {
            string madh = txtMaDH.Text.ToString();
            string url = string.Format("{0}/?", URL + "DonHangTong/DeleteDonHangTongPOChiTiet");
            string json = Task.Run(async () => await _clientExtension.PostAsync(url, tblXoa)).Result;
            DeleteDonHangTongPO(_poID, _maMau, _dauSizeID);

            string urldeletePOCD = string.Format("{0}?madh={1}&&poid={2}", URL + "CanDoiDonHangTong/DeletePOCD", madh, _poID);
            string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(urldeletePOCD); }).Result;

            DataTable tbl = gridControlThongTinDonHang.DataSource as DataTable;
            SaveDonHangTongAdd(tbl, true, _poID, _maMau, _dauSizeID);
        }

        private void DeleteDonHangTongPO(string _poID, string _maMau, string _dauSizeID)
        {
            // Xóa DonHangTongPO
            if (!(string.IsNullOrEmpty(_poID) && string.IsNullOrEmpty(_maMau) && string.IsNullOrEmpty(_dauSizeID)))
            {

                string _maDH = txtMaDH.Text.ToString();
                List<DonHangTongPOEntity> lstDeleteTongPO = new List<DonHangTongPOEntity>();
                lstDeleteTongPO.Add(new DonHangTongPOEntity(_maDH, _poID, _maMau, _dauSizeID));

                DataTable tblDHPO = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(lstDeleteTongPO));
                // Xóa DongHangTongPO
                string url = string.Format("{0}", URL + "DonHangTong/DeleteDonHangTongPO");
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblDHPO); }).Result;
                Console.WriteLine(result);
            }
        }



        private void BtNapLai()
        {

            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            SetData();
            GetChiTietDonHangTongPO(true);
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }

        private void Sua_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void bandedGridViewThongTinDonHang_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            DataRow row = view.GetDataRow(e.RowHandle);
            if (row != null && row.Table.Columns.Contains("IsNew") && row["IsNew"] is bool isNew && isNew)
            {
                e.Appearance.BackColor = Color.LightGreen;
                e.Appearance.BackColor2 = Color.LightYellow;
            }

        }

        private void BandedView_CustomDrawCell(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;

            DataRow row = view.GetDataRow(e.RowHandle);
            if (row != null && row.Table.Columns.Contains("IsNew") && row["IsNew"] is bool isNew && isNew)
            {
                e.Appearance.BackColor = Color.LightGreen;
                e.Appearance.BackColor2 = Color.LightYellow;
            }

        }

        private void bandedGridViewThongTinDonHang_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                GridView gridView = sender as GridView;


                DataTable checkdauszie = gridControlThongTinDonHang.DataSource as DataTable;
                if (e.Column.FieldName == "DauSize")
                {
                    foreach (DataRow dr in tblDonHangChiTiet.Rows)
                    {
                        dr["NhomSize"] = e.Value?.ToString();
                    }
                    string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", searchLookUpEditMH.EditValue.ToString(), searchLookUpEditKH.EditValue.ToString());
                    string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                    DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
                    if (dtSize == null && dtSize.Rows.Count < 0)
                    {
                        MessageBox.Show("Size thuộc mã hàng  " + searchLookUpEditMH.EditValue.ToString().ToString() + " chưa được khai báo trong thư viện size.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);// Chuỗi không đúng định dạng, xử lý tại đây
                        return;
                    }
                    var missingRows = from row1 in tblDonHangChiTiet.AsEnumerable()
                                      where !dtSize.AsEnumerable().Any(row2 =>
                                          row1["MaHang"].ToString() == row2["MaHang"].ToString() &&
                                          row1["SizeSanXuat"].ToString() == row2["SizeSanXuat"].ToString() &&
                                          row1["DauSize"].ToString() == row2["NhomSize"].ToString())
                                      select row1;
                    if (missingRows.Any())
                    {
                        // Nhóm các dòng bị thiếu theo NhomSize
                        var groupedMissing = missingRows
                            .GroupBy(row => row["NhomSize"].ToString())
                            .Select(group => new
                            {
                                NhomSize = group.Key, // Tên nhóm size
                            Sizes = group.Select(row => row["SizeSanXuat"].ToString()).Distinct() // Các size bị thiếu
                        });

                        // Tạo chuỗi thông báo
                        var message = string.Join("\n", groupedMissing.Select(group =>
                            $"Nhóm {group.NhomSize} : thiếu các size:  {string.Join(", ", group.Sizes)}  thuộc mã hàng {searchLookUpEditMH.EditValue.ToString()}"));

                        // Hiển thị thông báo
                        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;

                    }
                }
            }
            catch (Exception ex)
            {

                
            }
           

        }

        private void bandedGridViewThongTinDonHang_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {

        }

     

        private void Naplai_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            DisabledLayoutControl(true);
            BtNapLai();
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }

        private void AddRow1(string data, int rowHandle)
        {
            try
            {

                BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
                if (data == string.Empty) return;
                string[] rowData = data.Split('\t');
                int column = view.FocusedColumn.VisibleIndex;
                for (int i = 0; i < rowData.Length; i++)
                {
                    if (i >= view.VisibleColumns.Count) break;
                    if (rowData[i] == "-")
                        rowData[i] = "0";
                    List<string> allowedColumns = new List<string> { "ID", "MaDH", "POID", "PO", "MaQG", "DauSizeID", "DauSize", "NgayGH", "GhiChu", "MaMau" };
                    if (view.VisibleColumns[column + i] != null)
                    {
                        if (!allowedColumns.Contains(view.VisibleColumns[column + i].ToString()))
                        {
                            view.SetRowCellValue(rowHandle, view.VisibleColumns[column + i], rowData[i].Replace(",", ""));
                        }
                    }

                }
            }
            catch(Exception ex)
            { }
            
        }

        private void gridControlThongTinDonHang_ProcessGridKey(object sender, KeyEventArgs e)
        {
            //if (e.Control && e.KeyCode == Keys.C)
            //{
            //    BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            //    view.CopyToClipboard();
            //}
            //else if (e.Control && e.KeyCode == Keys.V)
            //{
            //    BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            //    string[] data = ClipboardData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //    if (data.Length < 1) return;
            //    int startRow = view.FocusedRowHandle;
            //    foreach (string row in data)
            //    {
            //        AddRow(row, startRow++);
            //        if (!view.IsValidRowHandle(startRow)) break;
            //    }

            //}
            BandedGridView view = gridControlThongTinDonHang.MainView as BandedGridView;
            if (e.Control && e.KeyCode == Keys.C)
            {
                view.CopyToClipboard();
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                string[] data = ClipboardData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length < 1) return;
                int startRow = view.FocusedRowHandle;
                bool hasError = false;
                foreach (string row in data)
                {
                    AddRow(row, startRow++);
                    if (!view.IsValidRowHandle(startRow)) break;
                }
                e.SuppressKeyPress = true;
                e.Handled = true;

            }
        }

        private DataTable CreateDatatable()
        {
            DataTable tbl = new DataTable("dtTable");
            tbl.Columns.Add("POID", typeof(string));
            tbl.Columns.Add("PO", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("MaQG", typeof(string));
            tbl.Columns.Add("NgayGH", typeof(DateTime));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("ColorCode", typeof(string));


            return tbl;
        }

        private DataTable CreateDatatableSizeNhom()
        {
            DataTable tbl = new DataTable("dtSizeNhom");
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("SizeSanXuat", typeof(string));
            tbl.Columns.Add("MaNhomSize", typeof(string));
            tbl.Columns.Add("NhomSize", typeof(string));
            return tbl;
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {

                string filePath = string.Empty;
                string sheetIndex = string.Empty;
                bool resultFrm = false;
                // btnXacNhan.BackColor = Color.Navy;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                openFileDialog.Title = "Select an Excel File";

                frmErpImport_DHT frm = new frmErpImport_DHT();
                frm.ShowDialog();
                filePath = frm.resultFilePath;
                sheetIndex = frm.resultIndex;
                resultFrm = frm.result;
                if (resultFrm == false) return;
                string pathExecel = filePath;
                string conString = "", sheet1 = string.Empty;
                int totalAmount = 0, Amount = 0;
                using (OleDbConnection connection = new OleDbConnection(conString))
                {
                    var source = new ExcelDataSource();
                    source.FileName = pathExecel;
                    string worksheetName = "";
                    using (DevExpress.SpreadsheetSource.ISpreadsheetSource spreadsheetSource = DevExpress.SpreadsheetSource.SpreadsheetSourceFactory.CreateSource(pathExecel))
                    {
                        DevExpress.SpreadsheetSource.IWorksheetCollection worksheetCollection = spreadsheetSource.Worksheets;
                        worksheetName = worksheetCollection[sheetIndex].Name;
                    }
                    var worksheetSettingss = new ExcelWorksheetSettings(worksheetName, "$A1:D9");
                    source.SourceOptions = new ExcelSourceOptions(worksheetSettingss);
                    source.Fill();
                    DataTable tbl_Styles = new DataTable();
                    tbl_Styles = source.ToDataTable();

                    string pp = string.Empty;
                    string style = tbl_Styles.Rows[1][2].ToString().TrimStart().TrimEnd();
                    var worksheetSettings = new ExcelWorksheetSettings(worksheetName, "$A11:BZ500");
                    source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                    source.Fill();
                    DataTable tbColorPO = new DataTable();
                    tbColorPO = source.ToDataTable();
                    DataTable _dtTable = CreateDatatable();
                    //_dtTable = CreateDatatable();
                    DataTable _dtSizeNhom = CreateDatatableSizeNhom();
                    string styleID = "";
                    string cusName = "";
                    string chungloai = "";
                    string seasonex = "";
                    string bookingnum = "";
                    string _styleID = style.ToString().TrimStart().TrimEnd();
                    if (string.IsNullOrEmpty(_styleID))
                    {
                        XtraMessageBox.Show("Mã hàng không được bỏ trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string urlmhang = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
                    string jsonmhang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlmhang); }).Result;
                    DataTable _dtMaHang = JsonConvert.DeserializeObject<DataTable>(jsonmhang);
                    if (_dtMaHang == null)
                    {
                        XtraMessageBox.Show("Mã hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var rowmh = _dtMaHang.AsEnumerable().FirstOrDefault(r => r["TenHang"].ToString().Trim() == _styleID.ToString().Trim());
                    if (rowmh != null)
                    {
                        styleID = rowmh["MaHang"].ToString();
                    }
                    else
                    {
                        XtraMessageBox.Show("Mã hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // kiểm tra khách hàng
                    string _cusName = tbl_Styles.Rows[0][2].ToString().TrimStart().TrimEnd();
                    if (string.IsNullOrEmpty(_cusName))
                    {
                        XtraMessageBox.Show("Không được bỏ trống khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string urlKH = string.Format("{0}?", URL + "DonHangTong/GetKhachHang");
                    string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
                    DataTable _dtKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
                    if (_dtKH == null)
                    {
                        XtraMessageBox.Show("Khách hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var rowkh = _dtKH.AsEnumerable().FirstOrDefault(r => r["TenKH"].ToString().Trim() == _cusName.ToString().Trim());
                    if (rowkh != null)
                    {
                        cusName = rowkh["MaKH"].ToString();
                    }
                    else
                    {
                        XtraMessageBox.Show("Khách hàng chưa được khai báo trong thư viện. Vui lòng khai báo trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if(styleID.ToString() != searchLookUpEditMH.EditValue.ToString() && cusName.ToString() != searchLookUpEditKH.EditValue.ToString())
                    {
                        XtraMessageBox.Show("Khách hàng và Mã hàng không giống với Đơn hàng. Vui lòng kiểm tra lại trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (cusName.ToString() != searchLookUpEditKH.EditValue.ToString())
                    {
                        XtraMessageBox.Show("Khách hàng không giống với Đơn hàng. Vui lòng kiểm tra lại trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (styleID.ToString() != searchLookUpEditMH.EditValue.ToString())
                    {
                        XtraMessageBox.Show("Khách hàng không giống với Đơn hàng. Vui lòng kiểm tra lại trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    string urlCL = string.Format("{0}?chungloai={1}", URL + "DonHangTong/GetChungLoai", tbl_Styles.Rows[4][2].ToString().TrimStart().TrimEnd());
                    string jsonCL = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCL); }).Result;
                    DataTable _dtCL = JsonConvert.DeserializeObject<DataTable>(jsonCL);
                    if (_dtCL != null && _dtCL.Rows.Count > 0)
                    {
                        var rowcl = _dtCL.AsEnumerable().FirstOrDefault(r => r["TenCL"].ToString().Trim().ToUpper() == tbl_Styles.Rows[4][2].ToString().TrimStart().TrimEnd().ToUpper());
                        if (rowcl != null)
                        {
                            chungloai = rowcl["MaCL"].ToString();
                        }

                    }

                    seasonex = tbl_Styles.Rows[2][2].ToString().TrimStart().TrimEnd().ToUpper();
                    bookingnum = tbl_Styles.Rows[5][2].ToString().TrimStart().TrimEnd().ToUpper();
                    if (chungloai.ToString() != searchLookUpEditCL.EditValue.ToString()) 
                    {
                        XtraMessageBox.Show("Chủng loại không giống với Đơn hàng. Vui lòng kiểm tra lại trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (seasonex.ToString() != txtDot.Text.ToString()) 
                    {
                        XtraMessageBox.Show("Đợt khác với Đơn hàng. Vui lòng kiểm tra lại trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (bookingnum.ToString() != txtSoBooing.Text.ToString())
                    {
                        XtraMessageBox.Show("Số Booking khác với Đơn hàng. Vui lòng kiểm tra lại trước khi Import Excel.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string urlSize = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetSize", styleID.ToString(), cusName.ToString());
                    string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                    DataTable dtSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
                    //Mau
                    string urlMau = string.Format("{0}?mahang={1}&&makh={2}", URL + "DonHangTong/GetMau", styleID.ToString(), cusName.ToString());
                    string jsonMau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                    DataTable dtMau = JsonConvert.DeserializeObject<DataTable>(jsonMau);
                    int Grand = 0;
                    for (int j = 1; j < tbColorPO.Rows.Count; j++)
                    {
                        if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("TOTAL"))
                        {
                            if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                                break;
                            continue;
                        }
                        else
                        {
                            for (int c = 6; c < tbColorPO.Columns.Count - 1; c++)
                            {
                                if (tbColorPO.Columns[c].ToString().Trim().ToUpper().Contains("GRAND"))
                                    break;

                                if (KiemTraChuoiLaChu(tbColorPO.Rows[j][c].ToString().Trim()))
                                {
                                    MessageBox.Show("Số lượng size không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                    return;
                                }
                                else
                                {
                                    if (tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "") != "")
                                        totalAmount = totalAmount + (tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "") == "" ? 0 : Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", ""), "[^0-9a-zA-Z]+", "")));//row[col].ToString().Replace());
                                }

                            }
                        }
                    }

                    //string cleanedStringktmh = Regex.Replace(styleID, @"[^\w\d-]", "_");
                    //cleanedStringktmh = cleanedStringktmh.Replace(" ", "_");
                    string urlktdonhang = string.Format("{0}?mahang={1}", URL + "DonHangTong/GetDonHangTongMaHang", styleID);
                    string jsonktdonhang = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlktdonhang); }).Result;
                    List<DonHangTongEntity> dohanglist = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(jsonktdonhang);
                    bool isMessageShow = false;
                    foreach (DonHangTongEntity donhang in dohanglist)
                    {

                        if (donhang.MaKH == cusName.ToString() && donhang.MaHang == styleID.ToString() && RemoveVietnameseTone(donhang.Dot).Trim().ToUpper().ToString().Replace(" ", "") == RemoveVietnameseTone(tbl_Styles.Rows[1][2].ToString().TrimStart().TrimEnd()).Trim().ToUpper().ToString().Replace(" ", "")
                           && donhang.SoLuong == totalAmount)
                        {
                            if (!isMessageShow)
                            {
                                DialogResult messResult = MessageBox.Show("Đã có đơn hàng cho mã hàng và khách hàng này. Vui lòng kiểm tra lại. Nếu muốn tiếp tục hãy đổi Đợt!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (messResult == DialogResult.Yes)
                                {
                                    return;
                                }
                            }
                            isMessageShow = true;
                        }
                    }

                    string sizetrung = "", _inseam = "";
                    for (int j = 1; j < tbColorPO.Columns.Count - 1; j++)
                    {
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][3].ToString()))
                        {
                            //_inseam = tbColorPO.Rows[j][3].ToString().Trim();
                            _inseam = Regex.Replace(tbColorPO.Rows[j][3].ToString().Trim().ToUpper(), @"[^\w\d]+", "_");
                        }
                        if (_inseam == "" && !_inseam.ToUpper().Contains("GRAND"))
                        {
                            _inseam = "0";
                        }

                        if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("TOTAL"))
                        {
                            if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                                break;
                        }
                        for (int i = 6; i < tbColorPO.Columns.Count - 1; i++)
                        {

                            if (!string.IsNullOrEmpty(tbColorPO.Columns[i].ColumnName.ToString()))
                            {
                                pp = tbColorPO.Columns[i].ColumnName;
                            }

                            if (pp.Trim().ToUpper().Contains("TOTAL"))
                            {
                                if (pp.Trim().ToUpper().Contains("GRAND"))
                                    break;
                                continue;
                            }

                            string sizeID = tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper() + "@Size@" + tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper();
                            if (sizeID.ToString() == "")
                            {
                                XtraMessageBox.Show("Size không được bỏ trống. Vui lòng kiểm tra lai.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            if (sizetrung.ToString() == sizeID.ToString())
                            {
                                XtraMessageBox.Show("Size bị trùng. Vui lòng kiểm tra lai.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            var rowsize = _dtSizeNhom.AsEnumerable().FirstOrDefault(r => r["SizeSanXuat"].ToString().Trim() == tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper()
                            && r["MaNhomSize"].ToString().Trim() == _inseam.ToString().Trim());
                            if (rowsize != null) break;
                            DataRow _dr = _dtSizeNhom.NewRow();
                            _dr["MaHang"] = styleID;
                            _dr["SizeSanXuat"] = tbColorPO.Rows[0][i].ToString().Replace(" ", "").Trim().ToUpper();
                            _dr["MaNhomSize"] = _inseam.ToString().Replace(" ", "").Trim().ToUpper();
                            _dr["NhomSize"] = string.IsNullOrEmpty(tbColorPO.Rows[j][3].ToString()) ? "0" : tbColorPO.Rows[j][3].ToString().Trim().ToUpper();
                            _dtSizeNhom.Rows.Add(_dr);
                            sizetrung = sizeID;

                        }
                    }
                    //// Đối chiếu dữ liệu và kiểm tra
                    var missingRows = from row1 in _dtSizeNhom.AsEnumerable()
                                      where !dtSize.AsEnumerable().Any(row2 =>
                                          row1["MaHang"].ToString() == row2["MaHang"].ToString() &&
                                          row1["SizeSanXuat"].ToString() == row2["SizeSanXuat"].ToString() &&
                                          row1["NhomSize"].ToString() == row2["NhomSize"].ToString())
                                      select row1;

                    if (missingRows.Any())
                    {
                        // Nhóm các dòng bị thiếu theo NhomSize
                        var groupedMissing = missingRows
                            .GroupBy(row => row["NhomSize"].ToString())
                            .Select(group => new
                            {
                                NhomSize = group.Key, // Tên nhóm size
                                Sizes = group.Select(row => row["SizeSanXuat"].ToString()).Distinct() // Các size bị thiếu
                            });

                        // Tạo chuỗi thông báo
                        var message = string.Join("\n", groupedMissing.Select(group =>
                            $"InSeam {group.NhomSize} : thiếu các size:  {string.Join(", ", group.Sizes)}  thuộc mã hàng {styleID.ToString()}"));

                        // Hiển thị thông báo
                        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    var uniqueValues = _dtSizeNhom.AsEnumerable().Select(x => new { MaHang = x["MaHang"], SizeSanXuat = x["SizeSanXuat"] }).Distinct().ToList();
                    foreach (var dr in uniqueValues)
                    {
                        var rowmasize = dtSize.AsEnumerable().FirstOrDefault(r => r["SizeSanXuat"].ToString().Trim() == dr.SizeSanXuat.ToString().Trim());
                        if (rowmasize != null)
                            _dtTable.Columns.Add(rowmasize["MaSize"].ToString() + "@Size@" + dr.SizeSanXuat.ToString(), typeof(string));
                    }
                    string _strdausize = string.Empty, _sizetrung = string.Empty;
                    string newHeader = string.Empty;
                    string dausize = string.Empty;
                    string _colorID = string.Empty, _colorName = string.Empty, _po = string.Empty, _maQG = string.Empty, _dausize = string.Empty, _colorCode = string.Empty;
                    DateTime _ngayGH;
                    string error = string.Empty, _potrung = string.Empty;

                    List<string> duplicatedRows = new List<string>();
                    string ngayBHStr = string.Empty;
                    for (int j = 1; j < tbColorPO.Rows.Count; j++)
                    {
                        if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("TOTAL"))
                        {
                            if (tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                                break;
                            continue;
                        }
                        _colorCode = tbColorPO.Rows[j][0].ToString();
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][0].ToString()))
                        {
                            _colorCode = tbColorPO.Rows[j][0].ToString().Trim();
                        }
                        if (_colorCode == "" && !tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                        {
                            _colorCode = "";
                        }

                        _colorID = tbColorPO.Rows[j][1].ToString().Trim();
                        var _rowcolorID = dtMau.AsEnumerable().FirstOrDefault(x => Regex.Replace(x["TenMau"].ToString().Trim(), @"\s+", "").ToUpper() == Regex.Replace(tbColorPO.Rows[j][1].ToString().Trim(), @"\s+", "").ToUpper());
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][1].ToString().ToUpper()))
                        {
                            _colorName = _rowcolorID == null ? "" : _rowcolorID["MaMau"].ToString();

                        }
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][2].ToString()))
                        {
                            _po = tbColorPO.Rows[j][2].ToString().Trim();
                        }
                        if (_po == "" && !tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                        {
                            MessageBox.Show("PO không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        _dausize = string.IsNullOrEmpty(tbColorPO.Rows[j][3].ToString()) ? "0" : tbColorPO.Rows[j][3].ToString().Trim();

                        bool isDuplicate = tblDonHangChiTiet.AsEnumerable().Any(r =>
                            r.Field<string>("PO")?.Trim() == _po &&
                            r.Field<string>("MaMau")?.Trim() == _colorName &&
                            r.Field<string>("DauSize")?.Trim() == _dausize);

                        if (isDuplicate)
                        {
                            duplicatedRows.Add($"- PO: {_po}, Màu: {_colorID}, InSeam: {_dausize}");
                        }
                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][4].ToString()))
                        {
                            _maQG = tbColorPO.Rows[j][4].ToString().Trim();
                        }

                        if (!string.IsNullOrEmpty(tbColorPO.Rows[j][5].ToString()))
                        {

                            DateTime date_ngayBH;
                            if (DateTime.TryParseExact(tbColorPO.Rows[j][5].ToString().Trim().Replace("-", "/"), new[]
                            { "MM/dd/yyyy", "MM/dd/yyyy h:mm:ss", "MM/dd/yyyy h:mm:ss tt", "dd/MM/yyyy h:mm:ss", "dd/MM/yyyy h:mm:ss tt", "dd/MM/yyyy", "M/d/yyyy h:mm:ss tt", "M/d/yyyy", "M/d/yy", "yy/MM/dd", "yyyy-MM-dd", "dd-MMM-yy", "MM/yyyy/dd h:mm:ss","M/d/yyyy h:mm:ss","M/d/yy h:mm:ss","MM/dd/yy h:mm:ss", "yy/MM/dd h:mm:ss", "yyyy/MM/dd h:mm:ss","dd-MMM-yy h:mm:ss","yyyy-MM-dd h:mm:ss", "dd.MM.yyyy", "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy h:mm:ss tt", "dd\\MM\\yyyy", "dd\\MM\\yyyy HH:mm:ss","dd\\MM\\yyyy h:mm:ss tt","dd-MMM-yyyy","dd-MMMM-yyyy","dd-MMM-yyyy HH:mm:ss","dd-MMMM-yyyy HH:mm:ss","yyyy/MM/dd","yyyy-MM-dd","yyyy.MM.dd","yyyy\\MM\\dd","yyyyMMdd","dd/MM/yyyy HH:mm:ss","dd.MM.yyyy HH:mm:ss","dd\\MM\\yyyy HH:mm:ss","dd/MM/yyyy hh:mm:ss tt","dd-MM-yyyy hh:mm:ss tt","dd.MM.yyyy hh:mm:ss tt","dd\\MM\\yyyy hh:mm:ss tt","yyyy-MM-ddTHH:mm:ssZ","yyyy-MM-ddTHH:mm:ss.fffZ","yyyy-MM-ddTHH:mm:sszzz","yyyy-MM-ddTHH:mm:ss.fffzzz","dd/MM/yy","dd-MM-yy","dd.MM.yy","dd\\MM\\yy","MM/dd/yy","MM-dd-yy","MM.dd.yy","MM\\dd\\yy","yyyy/MM/dd HH:mm:ss","yyyy-MM-dd HH:mm:ss","yyyy.MM.dd HH:mm:ss","yyyy\\MM\\dd HH:mm:ss","yyyy/MM/dd hh:mm:ss tt","yyyy-MM-dd hh:mm:ss tt","yyyy.MM.dd hh:mm:ss tt","yyyy\\MM\\dd hh:mm:ss tt"
                                }, CultureInfo.InvariantCulture, DateTimeStyles.None, out date_ngayBH))
                            {
                                ngayBHStr = date_ngayBH.ToString("dd/MM/yyyy");
                            }
                            else
                            {
                                MessageBox.Show("Kiểm tra lại Ngày Giao Hàng chưa đúng định dạng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                        }
                        if (ngayBHStr == "" && !tbColorPO.Rows[j][0].ToString().Trim().ToUpper().Contains("GRAND"))
                        {
                            MessageBox.Show("Ngày giao hàng không được bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (_colorID.Trim().ToUpper().Contains("TOTAL"))
                        {
                            _po = "";
                            _colorName = "";
                            ngayBHStr = "";
                            _maQG = "";
                            if (_colorID.Trim().ToUpper().Contains("GRAND"))
                                break;
                            continue;
                        }
                        else
                        {
                            DataRow _dr = _dtTable.NewRow();
                            int n = 8;
                            for (int c = 0; c < tbColorPO.Columns.Count - 1; c++)
                            {
                                if (tbColorPO.Columns[c].ToString().Trim().ToUpper().Contains("GRAND"))
                                    break;
                                if (c <= 5)
                                {
                                    _dr[0] = txtMaDH.Text + "|" + Regex.Replace(_po.Trim(), @"[^\w\s']", "_").Replace(" ", "_").Replace("'", "_");
                                    _dr[1] = _po.Trim();
                                    _dr[2] = _colorName.Trim();
                                    _dr[3] = _dausize.ToString().Trim();
                                    _dr[4] = _maQG.Trim();
                                    DateTime dateValue;

                                    try
                                    {
                                        dateValue = DateTime.ParseExact(ngayBHStr, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                        _dr[5] = dateValue;
                                        _dr[7] = _colorCode.Trim() == "" ? "" : _colorCode;
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine("Chuỗi không có định dạng hợp lệ.");
                                    }

                                }
                                else
                                {
                                    if (KiemTraChuoiLaChu(tbColorPO.Rows[j][c].ToString().Trim()))
                                    {
                                        MessageBox.Show("Số lượng size không đúng định dạng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);// Chuỗi không đúng định dạng, xử lý tại đây
                                        return;
                                    }
                                    else
                                    {
                                        _dr[n] = tbColorPO.Rows[j][c].ToString().Trim().Replace(",", "").Replace(".", "").Replace(" ", "");
                                        n++;
                                    }



                                }

                            }

                            _dtTable.Rows.Add(_dr);
                        }

                    }

                    if (duplicatedRows.Any())
                    {
                        string message = "Các dòng sau đã tồn tại trong đơn hàng:\n" + string.Join("\n", duplicatedRows) + "\n\nVui lòng kiểm tra lại trước khi Import Excel.";
                        MessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        _dtTable.Clear();
                        return;
                    }

                    foreach (DataRow _drmau in _dtTable.Rows)
                    {
                        var rowmau = dtMau.AsEnumerable().FirstOrDefault(r => r["TenMau"].ToString().Trim().ToUpper() == _drmau["MaMau"].ToString().Trim().ToUpper());
                        if (rowmau != null)
                            _drmau["MaMau"] = rowmau["TenMau"];
                    }

                    string urlQG = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                    string jsonQG = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlQG); }).Result;
                    List<QuocGiaEntity> lstQG = JsonConvert.DeserializeObject<List<QuocGiaEntity>>(jsonQG);

                    List<string> lstSaveQG = new List<string>();

                    for (int j = 0; j < _dtTable.Rows.Count; j++)
                    {

                        string KTQG = string.Empty;
                        QuocGiaEntity qg = lstQG.Where(item => RemoveVietnameseTone(item.TenQG).ToString().ToUpper().Trim() == RemoveVietnameseTone(_dtTable.Rows[j][4].ToString().ToUpper().Trim())).FirstOrDefault();
                        if (qg == null)
                        {
                            string hasDuplicates = lstSaveQG.Where(x => x.Equals(_dtTable.Rows[j][4].ToString())).FirstOrDefault();
                            if (string.IsNullOrEmpty(hasDuplicates))
                            {
                                if (_dtTable.Rows[j][4].ToString() != "")
                                    lstSaveQG.Add(_dtTable.Rows[j][4].ToString());
                            }
                        }
                    }
                    if (lstSaveQG.Count > 0)
                    {
                        AutoImportQGNew(lstSaveQG);
                    }
                    _dtTable = ChangeTenQGToMaQGNew(_dtTable);
                    _dtTable = _dtTable.AsEnumerable().OrderBy(x => x["MaMau"]).CopyToDataTable();

                    bool checkDuplicateData = _dtTable.AsEnumerable().GroupBy(row => new
                    {
                        DauSize = row.Field<string>("DauSize"),
                        POID = row.Field<string>("POID"),
                        MaMau = row.Field<string>("MaMau")
                    }).Any(g => g.Count() > 1);
                    if (checkDuplicateData)
                    {
                        XtraMessageBox.Show("Dữ liệu bị trùng. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _dtTable.Clear();
                        return;
                    }
                    DataTable tblcheck = tblDonHangChiTiet.Copy();
                    if (tblDonHangChiTiet != null && tblDonHangChiTiet.Rows.Count > 0)
                    {
                        tblcheck = MapData(_dtTable, tblcheck);
                    }
                    gridBandSize.Children.Clear();
                    createTable(tblcheck);
                    gridControlThongTinDonHang.MainView = GetBandGridViewAmount(tblcheck);
                    //BandedGridView mainView = (BandedGridView)gridControlThongTinDonHang.MainView;
                    //mainView.CellValueChanging += mainView_CellValueChanging;
                    //mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
                    gridControlThongTinDonHang.DataSource = tblcheck;
                    DataTable dataSource = gridControlThongTinDonHang.DataSource as DataTable;
                    if (!dataSource.Columns.Contains("CheckColumn"))
                    {
                        dataSource.Columns.Add("CheckColumn", typeof(bool));
                    }
                    if (!dataSource.Columns.Contains("IsNew"))
                    {
                        dataSource.Columns.Add("IsNew", typeof(bool));
                    }
                    foreach (DataRow row in dataSource.Rows)
                    {
                        row["CheckColumn"] = true;
                        bool istontai = tblDonHangChiTiet.AsEnumerable().Any(r =>
                                            r.Field<string>("PO")?.Trim() == row["PO"].ToString() &&
                                            r.Field<string>("MaMau")?.Trim() == row["MaMau"].ToString() &&
                                            r.Field<string>("DauSize")?.Trim() == row["DauSize"].ToString());
                        if (istontai)
                        {
                            row["IsNew"] = false;
                        }
                        else 
                        {
                            row["IsNew"] = true;
                        }
                        
                    }
                }

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng kiểm tra file excel và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }


        private DataTable MapData(DataTable _dtTable, DataTable tblChanged)
        {
            var existingRows = new HashSet<string>();

            string urlKT = $"{URL}BangSize/GetBangSize";
            string jsonKT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKT); }).Result;
            DataTable tblbs = JsonConvert.DeserializeObject<DataTable>(jsonKT);

            foreach (DataRow row in _dtTable.Rows)
            {
                var rowinseam = tblbs.AsEnumerable().FirstOrDefault(r =>
                                r["NhomSize"].ToString() == row["DauSize"].ToString().Trim());

                DataRow newRow = tblChanged.NewRow();
                newRow["MaDH"] = txtMaDH.Text;
                newRow["POID"] = row["POID"];
                newRow["PO"] = row["PO"];
                newRow["ColorCode"] = row["ColorCode"];
                newRow["MaMau"] = row["MaMau"];
                if (rowinseam != null && rowinseam["NhomSize"] != DBNull.Value)
                {
                    newRow["DauSizeID"] = rowinseam["MaNhomSize"];
                }
                else
                {
                    newRow["DauSizeID"] = ""; // Giá trị mặc định nếu có lỗi
                }
                newRow["DauSize"] = row["DauSize"];
                newRow["MaQG"] = row["MaQG"];
                newRow["NgayGH"] = row["NgayGH"];
                newRow["GhiChu"] = row["GhiChu"];
                newRow["IsCD"] = 0;
                newRow["BookingMaHang"] = txtSoBooing.Text;
                tblChanged.Rows.Add(newRow);
            }

            int index = 8;
            foreach (DataColumn column in tblChanged.Columns)
            {
                if (column.ColumnName.Contains("@Size@"))
                {
                    for (int i = 0; i < tblChanged.Rows.Count; i++)
                    {
                        DataRow rowChanged = tblChanged.Rows[i];

                        DataRow[] matchingRows = _dtTable.Select(
                            $"POID = '{rowChanged["POID"]}' AND " +
                            $"PO = '{rowChanged["PO"]}' AND " +
                            $"DauSize = '{rowChanged["DauSize"]}' AND " +
                            $"MaMau = '{rowChanged["MaMau"]}'");

                        if (matchingRows.Length > 0)
                        {
                            // Lấy dòng đầu tiên khớp
                            DataRow sourceRow = matchingRows[0];
                            rowChanged[column] = sourceRow[index];
                        }
                    }
                    index += 1;
                }
            }

            return tblChanged;
        }


        static bool KiemTraChuoiLaChu(string chuoi)
        {
            if (string.IsNullOrEmpty(chuoi))
            {
                return false; // Chuỗi rỗng không được coi là chứa toàn ký tự chữ
            }

            foreach (char kyTu in chuoi)
            {
                if (!char.IsLetter(kyTu))
                {
                    return false; // Nếu có ít nhất một ký tự không phải là chữ, trả về false
                }
            }

            return true; // Nếu không có ký tự nào không phải là chữ, trả về true
        }

        public string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }

        private void AutoImportQGNew(List<string> lstSaveQG)
        {
            List<QuocGiaEntity> lstAutoImportQG = new List<QuocGiaEntity>();
            QuocGiaEntity autoImportSizeItem;

            if (lstSaveQG != null && lstSaveQG.Count > 0)
            {
                lstSaveQG = lstSaveQG.Distinct().OrderBy(x => x).ToList();
                for (int i = 0; i < lstSaveQG.Count; i++)
                {
                    QuocGiaEntity itemQG = new QuocGiaEntity(lstSaveQG[i]);
                    lstAutoImportQG.Add(itemQG);
                }
                if (lstAutoImportQG.Count > 0)
                {
                    string urlAutoImportQG = string.Format("{0}?", URL + "QuocGia/PostAutoQG");
                    string result = Task.Run(async () => { return await _clientExtension.PostAsync(urlAutoImportQG, lstAutoImportQG); }).Result;
                }
            }
        }

        public DataTable ChangeTenQGToMaQGNew(DataTable tbl)
        {
            try
            {
                DataTable tblChange = tbl.Copy();
                // lấy danh sách bảng QG
                string urlQG = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlQG); }).Result;
                List<QuocGiaEntity> _lstQG = JsonConvert.DeserializeObject<List<QuocGiaEntity>>(json);
                string columnNameQG = "MAQG";

                // Cập nhật field Màu PO
                for (int i = 0; i < tblChange.Rows.Count; i++)
                {
                    foreach (DataColumn dataColumn in tblChange.Columns)
                    {
                        if (RemoveDiacritics(dataColumn.ColumnName).Replace(" ", "").ToUpper().Equals(columnNameQG))
                        {
                            //Console.WriteLine("ChangedTenMauToMaMau");
                            if (tblChange.Rows[i][dataColumn] != null)
                            {
                                string tenQG = tblChange.Rows[i][dataColumn].ToString();
                                // Lấy ra mã màu từ _lstBangMau bằng field TenMau và MaHang
                                QuocGiaEntity QGEntity = _lstQG.Where(x => x.TenQG.ToString().ToUpper().Trim() == tenQG.ToString().ToUpper().Trim()).FirstOrDefault();

                                if (QGEntity != null)
                                {
                                    // Đổi DataTable
                                    tblChange.Rows[i][dataColumn] = QGEntity.MaQG;
                                }
                                else
                                {
                                    Console.WriteLine("Import QG không thành công => Nên không lấy được mã QG sau khi import");
                                }
                            }
                        }
                    }
                }

                //Console.WriteLine("Change PO complete");
                return tblChange;
            }
            catch (Exception ex)
            {
                return new DataTable();
            }

        }

        static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private void createTable(DataTable tab)
        {
            //gridViewThongTinDonHang.OptionsView.AllowCellMerge = false;
            gridBandSize.Children.Clear();
            GridBand parentBand = bandedGridViewThongTinDonHang.Bands["gridBandSize"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < bandedGridViewThongTinDonHang.Columns.Count;)
                {
                    if (bandedGridViewThongTinDonHang.Columns[i].FieldName.Contains("@Size@"))
                    {
                        bandedGridViewThongTinDonHang.Columns.RemoveAt(i);
                    }
                    else
                    {
                        i += 1;
                    }
                }
            }


            int demColIndex = -1;

            foreach (DataColumn column in tab.Columns)
            {
                demColIndex++;
                if (demColIndex > 11 && demColIndex < tab.Columns.Count)
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[0];
                    String colName1 = arrName[2];
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = colName;
                    col.FieldName = column.ColumnName;
                    col.Name = "col" + colName;
                    col.OptionsColumn.AllowEdit = true;
                    col.Visible = true;
                    col.Width = 65;
                    col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    col.DisplayFormat.FormatString = "{0:##,0}";
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedGridViewThongTinDonHang.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.AppearanceHeader.Options.UseForeColor = true;
                    gb.Caption = colName;
                    gb.Columns.Add(col);
                    gb.Name = "gbColBandSize" + col;
                    gb.VisibleIndex = 0;
                    gb.Width = 65;
                    gridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }

        }


        private void mainView_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if (e.RowHandle >= 0 && (e.Column.FieldName == "MaQG" || e.Column.FieldName == "NgayGH" || e.Column.FieldName == "NgayDKVC" || e.Column.FieldName == "NgayThucTeVC"
                || e.Column.FieldName == "NgayXuatHang"))
            {
                GridView gridView = sender as GridView;

                DataRow rowFocused = (gridView.GetFocusedRow() as DataRowView).Row;
                DataTable dataTable = (gridView.DataSource as DataView).Table as DataTable;
                if (rowFocused != null && rowFocused["PO"] != null && dataTable != null && dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if (dataTable.Rows[i]["PO"] != null && (rowFocused["PO"].Equals(dataTable.Rows[i]["PO"])))
                        {
                            gridView.SetRowCellValue(i, e.Column.FieldName, e.Value);
                        }
                    }
                }
            }
        }

        void mainView_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                sum = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "ID" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "MaQG" && col.FieldName != "NgayGH" && col.FieldName != "GhiChu" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
            }
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            string mkh = searchLookUpEditKH.EditValue.ToString();
            string mh = searchLookUpEditMH.EditValue.ToString();
            var bandedView = gridControlThongTinDonHang.MainView as BandedGridView;
            if (bandedView == null) return;
            var inseamList = new HashSet<string>();
            for (int i = 0; i < bandedView.RowCount; i++)
            {
                var value = bandedView.GetRowCellValue(i, "DauSize");
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    inseamList.Add(value.ToString().Trim());
                }
            }

            List<string> existedSizes = new List<string>();
            foreach (DataColumn col in tblDonHangChiTiet.Columns)
            {
                if (col.ColumnName.Contains("@"))
                {
                    string sizeName = col.ColumnName.Split('@')[2];
                    existedSizes.Add(sizeName);
                }
            }

            string inseam = string.Join(":", inseamList);
            using (frmImportSize frm = new frmImportSize(existedSizes, mkh, mh, inseam, _unsavedSelectedSizes))
            {
                frm.StartPosition = FormStartPosition.CenterScreen;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    string urlSizecheck = string.Format("{0}?mahang={1}&&makh={2}&&dausize={3}", URL + "DonHangTong/GetSizeEdit", mh, mkh, inseam);
                    string jsonSizecheck = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSizecheck); }).Result;
                    DataTable dtSizeCheck = JsonConvert.DeserializeObject<DataTable>(jsonSizecheck);

                    var newSelectedIds = frm.SelectedSizes.Select(s => s.MaSize).ToList();
                    var oldUnsaved = _unsavedSelectedSizes.ToList();
                    // Lấy danh sách size đã chọn
                    foreach (var size in frm.SelectedSizes)
                    {
                        // Tạo tên cột dạng Size@MaHang
                        string colName = size.SizeSanXuat + "@Size@" + size.MaSize;

                        if (!tblDonHangChiTiet.Columns.Contains(colName))
                        {
                            tblDonHangChiTiet.Columns.Add(colName, typeof(int));
                            foreach (DataRow row in tblDonHangChiTiet.Rows)
                            {
                                row[colName] = 0; // mặc định số lượng = 0
                            }
                        }

                        if (!_unsavedSelectedSizes.Contains(size.MaSize))
                            _unsavedSelectedSizes.Add(size.MaSize);

                    }


                    // lấy chuỗi X:L:M từ form con
                    //string selectedSizeString = frm.Tag?.ToString();
                    // (selectedSizeString có dạng "X:L:M")
                    var toRemove = oldUnsaved
                        .Where(id => !newSelectedIds.Contains(id))
                        .ToList();

                    foreach (var id in toRemove)
                    {
                        var colToRemove = tblDonHangChiTiet.Columns
                            .Cast<DataColumn>()
                            .FirstOrDefault(c => c.ColumnName.Contains("@Size@" + id));

                        if (colToRemove != null)
                        {
                            tblDonHangChiTiet.Columns.Remove(colToRemove);
                        }

                        _unsavedSelectedSizes.Remove(id);
                    }

                    if (dtSizeCheck != null && dtSizeCheck.Rows.Count > 0)
                    {
                        // B1: map MaSize -> Sort
                        //Dictionary<string, int> sizeSortMap = dtSizeCheck.AsEnumerable()
                        //    .ToDictionary(r => r["MaSize"].ToString(),
                        //                  r => r.Table.Columns.Contains("Sort") ? Convert.ToInt32(r["Sort"]) : 9999);
                        Dictionary<string, int> sizeSortMap = new Dictionary<string, int>();
                        HashSet<int> usedSorts = new HashSet<int>();

                        foreach (DataRow r in dtSizeCheck.Rows)
                        {
                            string maSize = r["MaSize"].ToString();
                            int sort = 9999;

                            if (r.Table.Columns.Contains("Sort") && int.TryParse(r["Sort"].ToString(), out int s))
                                sort = s;

                            // Nếu Sort bị trùng, tự động +1 cho tới khi không trùng
                            while (usedSorts.Contains(sort))
                            {
                                sort++;
                            }

                            usedSorts.Add(sort);
                            sizeSortMap[maSize] = sort;
                        }

                        // B2: Lấy cột size hiện có
                        var sizeCols = tblDonHangChiTiet.Columns.Cast<DataColumn>()
                            .Where(c => c.ColumnName.Contains("@Size@"))
                            .ToList();

                        // B3: Sắp xếp lại theo Sort
                        var orderedCols = sizeCols
                            .OrderBy(c =>
                            {
                                string maSize = c.ColumnName.Split('@').Last();
                                return sizeSortMap.ContainsKey(maSize) ? sizeSortMap[maSize] : int.MaxValue;
                            })
                            .ToList();

                        // B4: Tạo DataTable mới theo thứ tự cột mới
                        DataTable newTbl = new DataTable();
                        foreach (DataColumn col in tblDonHangChiTiet.Columns)
                        {
                            if (!col.ColumnName.Contains("@Size@"))
                                newTbl.Columns.Add(col.ColumnName, col.DataType);
                        }
                        foreach (DataColumn col in orderedCols)
                        {
                            newTbl.Columns.Add(col.ColumnName, col.DataType);
                        }

                        // B5: Copy dữ liệu từng dòng theo đúng cột
                        foreach (DataRow oldRow in tblDonHangChiTiet.Rows)
                        {
                            DataRow newRow = newTbl.NewRow();
                            foreach (DataColumn col in newTbl.Columns)
                            {
                                if (tblDonHangChiTiet.Columns.Contains(col.ColumnName))
                                    newRow[col.ColumnName] = oldRow[col.ColumnName];
                            }
                            newTbl.Rows.Add(newRow);
                        }

                        tblDonHangChiTiet = newTbl;
                    }

                    gridControlThongTinDonHang.MainView = null;
                    gridControlThongTinDonHang.ViewCollection.Clear();
                    gridControlThongTinDonHang.MainView = GetBandGridViewAmount(tblDonHangChiTiet);
                    gridControlThongTinDonHang.DataSource = tblDonHangChiTiet;
                }
            }

        }


        private void ConvertColumnsToDateTime(DataTable dt, params string[] columnNames)
        {
            if (dt == null) return;

            // Những format hay gặp (bổ sung nếu bạn dùng format khác)
            string[] formats = new[] {
                    "dd/MM/yyyy",
                    "d/M/yyyy",
                    "dd/MM/yyyy HH:mm:ss",
                    "dd/MM/yyyy H:mm",
                    "yyyy-MM-ddTHH:mm:ss",
                    "yyyy-MM-dd"
                };

            var vi = CultureInfo.GetCultureInfo("vi-VN");

            foreach (var colName in columnNames)
            {
                if (!dt.Columns.Contains(colName)) continue;
                if (dt.Columns[colName].DataType == typeof(DateTime)) continue;

                // tạo cột tạm kiểu DateTime
                DataColumn tmp = new DataColumn(colName + "_dt", typeof(DateTime));
                dt.Columns.Add(tmp);

                foreach (DataRow r in dt.Rows)
                {
                    var v = r[colName];
                    if (v == null || v == DBNull.Value || string.IsNullOrWhiteSpace(v.ToString()))
                    {
                        r[tmp] = DBNull.Value;
                        continue;
                    }

                    var s = v.ToString().Trim();
                    DateTime parsed;

                    // thử nhiều cách: TryParseExact với culture vi-VN, TryParse với vi-VN, TryParseInvariant (dự phòng)
                    bool ok =
                        DateTime.TryParseExact(s, formats, vi, DateTimeStyles.None, out parsed)
                        || DateTime.TryParse(s, vi, DateTimeStyles.None, out parsed)
                        || DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed);

                    if (ok)
                        r[tmp] = parsed;
                    else
                        r[tmp] = DBNull.Value; // hoặc bạn muốn giữ nguyên chuỗi thì để r[tmp] = DBNull.Value hoặc xử lý khác
                }

                // thay thế cột cũ bằng cột tmp, giữ vị trí
                int ord = dt.Columns[colName].Ordinal;
                dt.Columns.Remove(colName);
                tmp.ColumnName = colName;
                tmp.SetOrdinal(ord);
            }
        }

        private void CreateSearchLookupHT()
        {
            string url = string.Format("{0}?", URL + "HinhThuc/Get");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbht = JsonConvert.DeserializeObject<DataTable>(json);
            tbht.Columns.Add("Chon", typeof(bool));
            serachlookupeditHT.Properties.DataSource = tbht;
            serachlookupeditHT.Properties.ValueMember = "MaHT";
            serachlookupeditHT.Properties.DisplayMember = "TenHT";
        }

        private DataTable CreateDatatableTheMau()
        {
            DataTable tbl = new DataTable("dtMauthe");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("CodeMau", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaTheMau", typeof(string));
            return tbl;
        }

        private DataTable CreateDatatableTheSize()
        {
            DataTable tbl = new DataTable("dtSizethe");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaSize", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("CodeSize", typeof(string));
            tbl.Columns.Add("TenSize", typeof(string));
            tbl.Columns.Add("SizeSanXuat", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("XacNhan", typeof(string));
            tbl.Columns.Add("SizeXacNhan", typeof(int));
            tbl.Columns.Add("MaNhomSize", typeof(string));
            tbl.Columns.Add("NhomSize", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("Sort", typeof(string));
            tbl.Columns.Add("MaTheSize", typeof(string));
            tbl.Columns.Add("MaTheInSeam", typeof(string));
            return tbl;
        }



        #region Đổ dữ liệu cân đối sản xuất


        private void bandedGridViewThongTinDonHang_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.Column.FieldName.Contains("@Size@") && e.RowHandle >= 0)
            {
                DataRow row_focused = view.GetFocusedDataRow();
                if (row_focused != null)
                {
                    DataTable tblSX = new DataTable();
                    DataTable tblSize = new DataTable();
                    string urlSize = $"{URL}SaveOnCanDoiDonViSX/Get?Action=GetSizeCanDoi&para={txtMaDH.Text}&para2={row_focused["POID"]}&para3={row_focused["DauSizeID"]}&para4={row_focused["MaMau"]}";
                    string jsonSize = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlSize); }).Result;
                    if(jsonSize != "[]")
                    {
                        tblSize = JsonConvert.DeserializeObject<DataTable>(jsonSize);
                    }
                    string url = $"{URL}SaveOnCanDoiDonViSX/Get?Action=GetLSX&para={txtMaDH.Text}&para2={row_focused["POID"]}&para3={row_focused["DauSizeID"]}&para4={row_focused["MaMau"]}";
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    if (json == "[]")
                    {
                        return;
                    }
                    tblSX = JsonConvert.DeserializeObject<DataTable>(json);
                    if (tblSX?.Rows?.Count > 0)
                    {
                        string[] arrSize = e.Column.FieldName.Split(new string[] { "@Size@" }, StringSplitOptions.RemoveEmptyEntries);
                        var Query = tblSize.AsEnumerable().FirstOrDefault(x => x["SizeID"]?.ToString()?.ToUpper()?.Trim() == arrSize[1]?.ToUpper()?.Trim() && x["Size"]?.ToString()?.ToUpper()?.Trim() == arrSize[0]?.ToUpper()?.Trim());
                        Int32.TryParse(tblSX.Rows[0]["SoLuongCanDoi"]?.ToString(), out int SoLuongCanDoi);
                        Int32.TryParse(tblSX.Rows[0]["SoLuongPO"]?.ToString(), out int SoLuongPO);
                        if (SoLuongCanDoi == SoLuongPO && Query != null)
                        {
                            frmSaveOnCanDoiDonViSX frm = new frmSaveOnCanDoiDonViSX(tblSX, txtMaDH.Text);
                            frm.StartPosition = FormStartPosition.CenterScreen;
                            frm.WindowState = FormWindowState.Maximized;
                            frm.FormBorderStyle = FormBorderStyle.Sizable;
                            frm.MaximizeBox = true;
                            frm.ShowDialog();
                            Naplai.PerformClick();
                        }

                    }

                }
            }

        }
        #endregion
    }
}
