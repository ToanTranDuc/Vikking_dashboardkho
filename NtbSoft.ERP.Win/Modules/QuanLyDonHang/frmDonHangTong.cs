using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
//using NtbSoft.ERP.Entity.XuLyVT;
using NtbSoft.ERP.Win.Modules.CanDoiDonHang;
using NtbSoft.ERP.Win.Modules.Erp.Kehoach;
using NtbSoft.ERP.Win.Modules.POMuaHang;
using NtbSoft.ERP.Win.Properties;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmDonHangTong : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        string _maDonHang = string.Empty, _mahang = string.Empty, _khachhang = string.Empty, _chungloai = string.Empty, _soluong = string.Empty, _ngaytao = string.Empty, _nguoitao = string.Empty;
        int _rowAdd = -1;
        List<DonHangTongEntity> lstDonHangTong;
        string json = string.Empty;
        int pageIndex = 1;
        int pageSize = 200;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool _allowViewCDDH = false, _allowAddCDDH = false, _allowEditCDDH = false, _allowDeleteCDDH = false;
        bool _allowViewDM = false, _allowAddDM = false, _allowEditDM = false, _allowDeleteDM = false;
        bool _allowViewCTLENH = false, _allowAddCTLENH = false, _allowEditCTLENH = false, _allowDeleteCTLENH = false;
        bool _ChkCT = false, _ChkTH = false;
        int sum = 0;
        string maHangSearchLookup = string.Empty;
        //bool isFocusedRowChanged = false;
        private int _rightClickedRowHandle = -1;
        bool indicatorIcon = true;
        KeyDownControlHandler keyDownControlHandler;
        private string _focusedMaDH = string.Empty, _focusedMaHang = string.Empty;
        private int _focusedRowHandle = -1;
        int indexFocusedRow;
        int rowhandle = 0;
        int _rowhandle = 0;
        DataTable tblCanDoiLog = new DataTable();
        DataTable tblToolTip = new DataTable();
        DataTable tblCanDoiLogStyle = new DataTable();
        DataTable _dtbCcCache = new DataTable();
        //Series series;

        //tam
        private string maDH = "";
        private string lenhSX = "";
        private DataTable vattuTable;
        private DataTable lenhTable;
        private DataTable ngayDBTable;
        private DataTable gridControl3Table;
        //tam
        public frmDonHangTong()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstDonHangTong = new List<DonHangTongEntity>();
            tblCanDoiLog = CreateCanDoiDinhMucNPL_LogTable();
        }
        protected override void OnLoad(EventArgs e)
        {
            indexFocusedRow = 0;
            barEditItemSpinPageSize.EditValue = pageSize;
            InitChart();
            barEditItemcbLoc.EditValue = repositoryItemComboBox1.Items[0].ToString();
            LoadDSDonHangTong();
            CreateSearchLookUp();
            CheckPerminsion();
            CheckPerminsionDH();
            InitChartV1();

            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            gridView1.RowCellStyle += gridView1_RowCellStyle;
        }
        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var view = sender as GridView; // hoặc BandedGridView nếu bạn dùng BandedGrid
            if (view == null) return;

            DataRow row = null;
            try { row = view.GetDataRow(e.RowHandle); } catch { row = null; }
            if (row == null) return;

            // Lấy giá trị CheckDuyet và Status
            int checkDuyet = Convert.ToInt32(row["CheckDuyetNPL"]);
            int status = 0;
            e.Appearance.BackColor = Color.White;
            e.Appearance.ForeColor = Color.Black;

            //// ✅ ÁP DỤNG MÀU CHO CẢ DÒNG DỰA TRÊN TRẠNG THÁI
            //if (checkDuyet == 0)
            //{
            //    // 🔸 Chưa duyệt (CheckDuyet = 0) → Màu trắng/mặc định
            //    e.Appearance.BackColor = Color.White;
            //    e.Appearance.ForeColor = Color.Black;
            //}
            //else if (checkDuyet == 1)
            //{
            //    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#43BDF9"); // Vàng nhạt
            //    e.Appearance.ForeColor = Color.Black;
            //}
            e.Appearance.Options.UseBackColor = true;
            e.Appearance.Options.UseForeColor = true;
        }
        private void GetDataChart(string maDH)
        {
            // CanDoi
            DataTable tblDsBaoCao;
            if (!string.IsNullOrEmpty(maDH))
            {
                List<BaoCaoEntity> lstBaoCao = new List<BaoCaoEntity>();
                string urlBaoCao = string.Format("{0}?maDH={1}", URL + "DonHangTong/GetDsBaoCao", maDH);
                string jsonBaoCao = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlBaoCao); }).Result;
                tblDsBaoCao = JsonConvert.DeserializeObject<DataTable>(jsonBaoCao);
            }
            else
            {
                tblDsBaoCao = null;
            }

            DrawChart(tblDsBaoCao);
        }

        private Color FillColor(double soLuong)
        {
            const double soLuongBlue = 70.0;
            Color colorBlue = Color.FromArgb(104, 156, 247);
            const double soLuongYellow = 50.0;
            Color colorYellow = Color.FromArgb(247, 245, 104);
            const double soLuongRed = 0.0;
            Color colorRed = Color.FromArgb(247, 104, 104);
            Color colorWhite = Color.White;

            if (soLuong >= soLuongBlue)
            {
                return colorBlue;
            }
            else if (soLuong >= soLuongYellow)
            {
                return colorYellow;
            }
            else if (soLuong > soLuongRed)
            {
                return colorRed;
            }
            return colorWhite;
        }

        private void InitChart()
        {
            DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel1 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
            sideBySideBarSeriesLabel1.Position = DevExpress.XtraCharts.BarSeriesLabelPosition.Center;

            // code cũ
            //series1.CrosshairLabelVisibility = DefaultBoolean.True;
            //series1.CrosshairEnabled = DefaultBoolean.True;
            //series1.CrosshairLabelPattern = "{A}: {V}%";
            //((BarSeriesView)series1.View).BarWidth = 0.3; // Đặt kích thước ngang của cột
            //series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;

            // Tùy chỉnh biểu đồ
            XYDiagram diagram = (XYDiagram)chartControl1.Diagram;
            //diagram.AxisX.Label.TextPattern = "{A}";


            // Gán nhãn giá trị cho trục Y
            diagram.AxisY.CustomLabels.AddRange(new DevExpress.XtraCharts.CustomAxisLabel[] {
            new CustomAxisLabel("10%","10"),
            new CustomAxisLabel("20%","20"),
            new CustomAxisLabel("30%","30"),
            new CustomAxisLabel("40%","40"),
            new CustomAxisLabel("50%","50"),
            new CustomAxisLabel("60%","60"),
            new CustomAxisLabel("70%","70"),
            new CustomAxisLabel("80%","80"),
            new CustomAxisLabel("90%","90"),
            new CustomAxisLabel("100%","100"),
            });
            diagram.AxisY.LabelVisibilityMode = AxisLabelVisibilityMode.Default;
            diagram.AxisY.VisibleInPanesSerializable = "-1";
            //diagram.AxisY.WholeRange.Auto = false; // Ngăn trục Y tự động điều chỉnh phạm vi giá trị toàn bộ
            //diagram.AxisY.VisualRange.Auto = false; // Ngăn trục Y tự động điều chỉnh phạm vi giá trị hiển thị

            // Thiết lập phạm vi giá trị toàn bộ và hiển thị cho trục Y
            diagram.AxisY.WholeRange.SetMinMaxValues(0, 100);
            diagram.AxisY.WholeRange.AutoSideMargins = false;
            diagram.AxisY.VisualRange.SetMinMaxValues(0, 100);

            // Xoay biểu đồ ngang
            diagram.Rotated = true;
        }

        private void DrawChart(DataTable tblBaoCao)
        {
            // Tạo và thêm dữ liệu mẫu vào biểu đồ

            DevExpress.XtraCharts.SeriesPoint seriesPoint1 = new DevExpress.XtraCharts.SeriesPoint()
                , seriesPoint2 = new DevExpress.XtraCharts.SeriesPoint()
                , seriesPoint3 = new DevExpress.XtraCharts.SeriesPoint()
                , seriesPoint4 = new DevExpress.XtraCharts.SeriesPoint()
                , seriesPoint5 = new DevExpress.XtraCharts.SeriesPoint()
                , seriesPoint6 = new DevExpress.XtraCharts.SeriesPoint();
            double soLuong = -1;
            if (tblBaoCao != null && tblBaoCao.Rows.Count > 0)
            {
                foreach (DataColumn column in tblBaoCao.Columns)
                {
                    switch (column.ColumnName)
                    {
                        case "CanDoi":
                            soLuong = double.Parse(tblBaoCao.Rows[0][column].ToString());
                            seriesPoint1 = new DevExpress.XtraCharts.SeriesPoint("Chia sản xuất", new object[] { (soLuong) });
                            seriesPoint1.Color = FillColor(soLuong);

                            break;
                        case "PackageList":
                            soLuong = double.Parse(tblBaoCao.Rows[0][column].ToString());
                            seriesPoint2 = new DevExpress.XtraCharts.SeriesPoint("Package List", new object[] { (soLuong) });
                            seriesPoint2.Color = FillColor(soLuong);
                            break;
                        case "DaDongThung":
                            soLuong = double.Parse(tblBaoCao.Rows[0][column].ToString());
                            seriesPoint3 = new DevExpress.XtraCharts.SeriesPoint("Đóng thùng", new object[] { (soLuong) });

                            seriesPoint3.Color = FillColor(soLuong);

                            break;
                        case "NhapKho":
                            soLuong = double.Parse(tblBaoCao.Rows[0][column].ToString());
                            seriesPoint4 = new DevExpress.XtraCharts.SeriesPoint("Nhập kho", new object[] { (soLuong) });
                            seriesPoint4.Color = FillColor(soLuong);

                            break;
                        case "LuanChuyen":
                            soLuong = double.Parse(tblBaoCao.Rows[0][column].ToString());
                            seriesPoint5 = new DevExpress.XtraCharts.SeriesPoint("Luân chuyển", new object[] { (soLuong) });
                            seriesPoint5.Color = FillColor(soLuong);

                            break;
                        case "XuatHang":
                            soLuong = double.Parse(tblBaoCao.Rows[0][column].ToString());
                            seriesPoint6 = new DevExpress.XtraCharts.SeriesPoint("Xuất hàng", new object[] { (soLuong) });

                            seriesPoint6.Color = FillColor(soLuong);

                            break;
                    }
                }
            }
            else
            {
                soLuong = double.Parse("0");
                seriesPoint1 = new DevExpress.XtraCharts.SeriesPoint("Chia sản xuất", new object[] { (soLuong) });
                seriesPoint1.Color = FillColor(soLuong);

                seriesPoint2 = new DevExpress.XtraCharts.SeriesPoint("Pakcage List", new object[] { (soLuong) });
                seriesPoint2.Color = FillColor(soLuong);

                seriesPoint3 = new DevExpress.XtraCharts.SeriesPoint("Đóng thùng", new object[] { (soLuong) });
                seriesPoint3.Color = FillColor(soLuong);

                seriesPoint4 = new DevExpress.XtraCharts.SeriesPoint("Nhập kho", new object[] { (soLuong) });
                seriesPoint4.Color = FillColor(soLuong);

                seriesPoint5 = new DevExpress.XtraCharts.SeriesPoint("Luân chuyển", new object[] { (soLuong) });
                seriesPoint5.Color = FillColor(soLuong);

                seriesPoint6 = new DevExpress.XtraCharts.SeriesPoint("Xuất hàng", new object[] { (soLuong) });
                seriesPoint6.Color = FillColor(soLuong);
            }

            // code cũ
            //if (series1.Points != null || (series1.Points != null && series1.Points.Count > 0))
            //{
            //    series1.Points.Clear();
            //}

            //series1.Points.AddRange(new DevExpress.XtraCharts.SeriesPoint[] {
            //seriesPoint6,
            //seriesPoint5,
            //seriesPoint4,
            //seriesPoint3,
            //seriesPoint2,
            //seriesPoint1});

            // Code mới
            if (chartControl1 != null && chartControl1.Series != null && chartControl1.Series.Count > 0)
            {
                Series seriesChartControl = chartControl1.Series[0];
                if (seriesChartControl != null)
                {
                    seriesChartControl.Points.Clear();
                    if (seriesChartControl.Points != null || (seriesChartControl.Points != null && seriesChartControl.Points.Count > 0))
                    {
                        seriesChartControl.Points.Clear();
                    }

                    seriesChartControl.Points.AddRange(new DevExpress.XtraCharts.SeriesPoint[] {
            seriesPoint6,
            seriesPoint5,
            seriesPoint4,
            seriesPoint3,
            seriesPoint2,
            seriesPoint1});
                }
            }
        }

        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, BtThem, true, ActionType.Add);
            AddActionControl(_lstActionControl, BtSua, true, ActionType.Edit);
            AddActionControl(_lstActionControl, BtXoa, true, ActionType.Delete);
            AddActionControl(_lstActionControl, NapLai, true, ActionType.Refresh);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private void CreateSearchLookUp()
        {
            string urlHH = string.Format("{0}?", URL + "HangHoa/GetHangHoa");
            string jsonHH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHH); }).Result;
            DataTable tblHH = JsonConvert.DeserializeObject<DataTable>(jsonHH);
            this.repositoryMaHangSearchLookUpEdit.DataSource = tblHH;
            //searchLookUpEditMH.Properties.DataSource = tblHH;

        }

        private void repositorySearchLookMaHang_EditValuedChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Edit_valued_changed");

            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;

            if (changingEventArgs != null)
            {
                maHangSearchLookup = changingEventArgs.NewValue as string;
            }
            else
            {
                maHangSearchLookup = string.Empty;
            }
            FilterData(maHangSearchLookup);
        }

        private void FilterData(string _maHang)
        {
            try
            {
                string url = string.Format("{0}", URL + "DonHangTong/GetALLDH");
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                //isRowChanged = false;
                if (!string.IsNullOrEmpty(json))
                {
                    DataTable _tblFilter = JsonConvert.DeserializeObject<DataTable>(json);
                    if (!string.IsNullOrEmpty(_maHang))
                    {
                        DataRow row = _tblFilter.AsEnumerable().FirstOrDefault(x => x["MaHang"].Equals(_maHang));
                        if (row != null)
                        {
                            _tblFilter = _tblFilter.AsEnumerable().Where(x => x["MaHang"].Equals(_maHang)).CopyToDataTable();
                        }
                        else
                        {
                            _tblFilter = null;
                            gridControlChiTietDonHang.DataSource = null;
                        }
                    }
                    gridDonHangTong.DataSource = _tblFilter;
                    if (_tblFilter != null && _tblFilter.Rows.Count > 0)
                    {
                        HandleLoadDataChiTiet();
                    }
                    else
                    {
                        gridControlChiTietDonHang.MainView = XoaCotSize();
                        gridControlChiTietDonHang.DataSource = null;
                        GetDataChart(null);
                    }

                    //if (!isRowChanged && _tblFilter != null)
                    //{
                    //    GetChiTietDonHangTongPO();
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private DevExpress.XtraGrid.Views.Base.BaseView XoaCotSize()
        {
            //bandedGridViewChiTietDonHang.Bands.Last().Children.Clear();
            bandedGridViewChiTietDonHang.Bands.Where(band => band.Name.Equals("gridBandSize")).FirstOrDefault().Children.Clear();
            return bandedGridViewChiTietDonHang;
        }

        private void splitContainerControl1_SizeChanged(object sender, EventArgs e)
        {
            //splitContainerControl1.SizeChanged -= splitContainerControl1_SizeChanged;
            splitContainerControl1.SplitterPosition = splitContainerControl1.Height / 2;
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
            }
            if (!_allowDelete)
                Xoa.Enabled = false;
        }
        private void CheckPerminsionDH()
        {
            string url = string.Format("{0}?userId={1}", URL + "PhanQuyenDH/GetUser", GlobleData.UserName);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _dtUserDH = JsonConvert.DeserializeObject<DataTable>(json);

            if (_dtUserDH == null || _dtUserDH.Rows.Count <= 0) return;
            foreach (DataRow _dr in _dtUserDH.Rows)
            {
                if (_dr["ModuleID"].ToString() == "M.1.00.00")
                {
                    _allowViewCDDH = (bool)_dr["AllowView"];
                    _allowAddCDDH = (bool)_dr["AllowAdd"];
                    _allowEditCDDH = (bool)_dr["AllowEdit"];
                    _allowDeleteCDDH = (bool)_dr["AllowDelete"];
                    if (!_allowViewCDDH)
                    {
                        xtraTabPage2.PageVisible = false;
                    }
                    if (!_allowAddCDDH)
                    {
                        btnChiaSX.Enabled = false;
                        // btnImport.Enabled = false;
                        simpleButton2.Enabled = false;
                        btnNhapCapPhat.Enabled = false;
                    }
                    if (!_allowEditCDDH)
                    {
                        btnXacNhan.Enabled = false;
                    }
                    if (!_allowDeleteCDDH)
                        simpleButton1.Enabled = false;
                }
                else
                {
                    if (_dr["ModuleID"].ToString() == "M.2.00.00")
                    {
                        _allowViewDM = (bool)_dr["AllowView"];
                        _allowAddDM = (bool)_dr["AllowAdd"];
                        _allowEditDM = (bool)_dr["AllowEdit"];
                        _allowDeleteDM = (bool)_dr["AllowDelete"];
                        if (!_allowViewDM)
                        {
                            xtraTabPage3.PageVisible = false;
                        }
                        //if (!_allowAddCDDH)
                        //{
                        //    btnChiaSX.Enabled = false;
                        //    btnImport.Enabled = false;
                        //}
                        //if (!_allowEditCDDH)
                        //{
                        //    btnSuaLenh.Enabled = false;
                        //}
                        if (!_allowDeleteDM)
                            btnXNXoa.Enabled = false;
                    }
                    else
                    {
                        if (_dr["ModuleID"].ToString() == "M.3.00.00")
                        {
                            _allowViewCTLENH = (bool)_dr["AllowView"];
                            _allowAddCTLENH = (bool)_dr["AllowAdd"];
                            _allowEditCTLENH = (bool)_dr["AllowEdit"];
                            _allowDeleteCTLENH = (bool)_dr["AllowDelete"];
                            if (!_allowViewCTLENH)
                            {
                                xtraTabPage4.PageVisible = false;
                            }
                            //if (!_allowAddCDDH)
                            //{
                            //    btnChiaSX.Enabled = false;
                            //    btnImport.Enabled = false;
                            //}
                            //if (!_allowEditCDDH)
                            //{
                            //    btnSuaLenh.Enabled = false;
                            //}
                            //if (!_allowDeleteCDDH)
                            //    btnXoaLenh.Enabled = false;
                        }
                    }
                }
            }

        }
        private void LoadDSDonHangTong(bool isRefresh = true)
        {
            try
            {
                if (isRefresh)
                {
                    maHangSearchLookup = string.Empty;
                    barEditItem1.EditValue = null;
                }
                string url = string.Format("{0}?pageIndex={1}&&pageSize={2}", URL + "DonHangTong/GetDonHangTong", pageIndex, pageSize);
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    if (!isRefresh && maHangSearchLookup != null && !string.IsNullOrEmpty(maHangSearchLookup))
                    {
                        FilterData(maHangSearchLookup);
                    }
                    else
                    {
                        if (barEditItemcbLoc.EditValue == null)
                        {
                            FilterData(maHangSearchLookup);
                        }
                        else
                        {
                            if (RemoveVietnameseTone(barEditItemcbLoc.EditValue.ToString().Trim().ToUpper().Replace(" ", "")).ToString().ToUpper() == "CHUACANDOISANXUAT")
                            {
                                var selectedRows = tbl.AsEnumerable().Where(row => Convert.ToInt32(row["SLTH"]) == 0 && Convert.ToInt32(row["SLCL"]) != 0);
                                if (selectedRows.Any())
                                    tbl = selectedRows.CopyToDataTable();
                                else
                                {
                                    MessageBox.Show("Dữ liệu lọc không tìm thấy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                var sortedRows = tbl.AsEnumerable().OrderBy(row => row["MaDH"].ToString().Substring(4, row["MaDH"].ToString().Length));

                            }
                            if (RemoveVietnameseTone(barEditItemcbLoc.EditValue.ToString().Trim().ToUpper().Replace(" ", "")).ToString().ToUpper() == "DACANDOISANXUATXONG")
                            {
                                var selectedRows = tbl.AsEnumerable().Where(row => Convert.ToInt32(row["SLCL"]) == 0);
                                if (selectedRows.Any())
                                    tbl = selectedRows.CopyToDataTable();
                                else
                                {
                                    MessageBox.Show("Dữ liệu lọc không tìm thấy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            if (RemoveVietnameseTone(barEditItemcbLoc.EditValue.ToString().Trim().ToUpper().Replace(" ", "")).ToString().ToUpper() == "CANDOISANXUATCHUAXONG")
                            {
                                var selectedRows = tbl.AsEnumerable().Where(row => Convert.ToInt32(row["SLTH"]) != 0 && Convert.ToInt32(row["SLCL"]) != 0);
                                if (selectedRows.Any())
                                    tbl = selectedRows.CopyToDataTable();
                                else
                                {
                                    MessageBox.Show("Dữ liệu lọc không tìm thấy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            int rowfocus = 0;
                            if (gridViewDonHangTong.FocusedRowHandle >= 0)
                                rowfocus = gridViewDonHangTong.FocusedRowHandle;
                            gridDonHangTong.DataSource = tbl;
                            gridViewDonHangTong.FocusedRowHandle = rowfocus;
                            gridDonHangTong.RefreshDataSource();
                            //gridViewDonHangTong.FocusedRowHandle = rowhandle;
                            HandleLoadDataChiTiet();
                        }
                    }
                }
                else
                {
                    NtbSoft.ERP.Libs.clsConvert<DonHangTongEntity> convert = new Libs.clsConvert<DonHangTongEntity>();
                    DataTable tblnull = convert.ToDataTable(lstDonHangTong);
                    gridDonHangTong.DataSource = tblnull;
                    GetDataChart(null);
                    //this.bandedGridViewChiTietDonHang.Bands.RemoveAt(bandedGridViewChiTietDonHang.Bands.Count - 1);
                    gridControlChiTietDonHang.MainView = XoaCotSize();
                    this.gridControlChiTietDonHang.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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
        private void gridViewDonHangTong_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gridViewDonHangTong.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InGroupRow || e.HitInfo.InRow)
                {
                    _rightClickedRowHandle = e.HitInfo.RowHandle;
                    if (_allowEdit)
                    {
                        DXMenuItem suaDH = new DXMenuItem();
                        suaDH.Caption = "Sửa đơn hàng";
                        suaDH.Click += SuaDH_Click;
                        e.Menu.Items.Add(suaDH);
                    }
                }
            }
        }
        private void SuaDH_Click(object sender, EventArgs e)
        {
            if (_rightClickedRowHandle < 0) return;
            object donhang = null;
            if (gridViewDonHangTong.IsGroupRow(gridViewDonHangTong.FocusedRowHandle))
            {
                int childRowHandle = gridViewDonHangTong.GetChildRowHandle(gridViewDonHangTong.FocusedRowHandle, 0);
                donhang = gridViewDonHangTong.GetRowCellValue(childRowHandle, colMaDH);
            }
            else
            {
                donhang = gridViewDonHangTong.GetFocusedRowCellValue(colMaDH);
            }
            //List<DonHangTongEntity> lstDonHangTong = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(json);
            //int rowFocused = gridViewDonHangTong.GetFocusedDataSourceRowIndex();
            //if (rowFocused >= 0)
            //{
            //    frmSuaDonHangTong frm = new frmSuaDonHangTong(lstDonHangTong[rowFocused], (gridControlChiTietDonHang.DataSource as DataTable), _allowAdd, _allowEdit, _allowDelete, _nguoitao);
            //    frm.WindowState = FormWindowState.Maximized;
            //    frm.ShowDialog();
            //    LoadDSDonHangTong();
            //}
            int rowFocused = gridViewDonHangTong.GetDataSourceRowIndex(_rightClickedRowHandle);
            if (rowFocused >= 0)
            {
                DataTable dt = gridDonHangTong.DataSource as DataTable;
                string json = JsonConvert.SerializeObject(dt);
                List<DonHangTongEntity> lstDonHangTong = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(json);

                frmSuaDonHangTong frm = new frmSuaDonHangTong(
                    lstDonHangTong[rowFocused],
                    (gridControlChiTietDonHang.DataSource as DataTable),
                    _allowAdd, _allowEdit, _allowDelete, _nguoitao);
                frm.WindowState = FormWindowState.Maximized;
                frm.Show();
                frm.FormClosed += (s, ev) => { LoadDSDonHangTong(); };
            }
        }

        private void gridViewDonHangTong_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
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
            catch
            {

            }

        }

        private void gridViewDonHangTong_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            GridView view = (GridView)sender;
            if (e.ListSourceRowIndex < 0) return;
            if (e.Column == colMaDH)
            {
                int chilHandle = view.GetChildRowHandle(e.GroupRowHandle, 0);
                if (chilHandle < 0) chilHandle = 0;
                e.DisplayText = e.DisplayText + string.Format(" (Mã hàng: {0}) - SeaSon: {1} - Số lượng:{2}", view.GetRowCellValue(chilHandle, colTenHang), view.GetRowCellValue(chilHandle, colDot), view.GetRowCellValue(chilHandle, colSLTong));
            }
        }

        private void GetChiTietDonHangTongPO()
        {
            try
            {
                GridView view = gridViewDonHangTong;
                string _maDH = view.GetFocusedRowCellValue(this.colMaDH) as string;
                if (!string.IsNullOrEmpty(_maDH))
                {
                    string url = string.Format("{0}?maDH={1}", URL + "DonHangTong/GetPivotDonHangTongPOChiTiet", _maDH);
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    //createColBandsSize(tbl);
                    gridControlChiTietDonHang.MainView = GetBandGridViewAmount(tbl);

                    gridControlChiTietDonHang.DataSource = tbl;

                    AddColumnSumRow(tbl);
                }
                else
                {
                    gridControlChiTietDonHang.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private DataTable AddColumnSumRow(DataTable tbl)
        {
            tbl.Columns.Add("TongSL", typeof(int));
            foreach (DataRow dataRow in tbl.Rows)
            {
                int tong = 0;
                foreach (DataColumn dataColumn in tbl.Columns)
                {
                    if (dataColumn.ColumnName.Contains("Size@"))
                    {
                        int sl = 0;
                        bool parse = int.TryParse(dataRow[dataColumn.ColumnName].ToString(), out sl);
                        if (parse)
                        {
                            tong += sl;
                        }
                    }
                }
                dataRow["TongSL"] = tong;
            }
            return tbl;
        }

        private DevExpress.XtraGrid.Views.Base.BaseView GetBandGridViewAmount(DataTable tab)
        {
            BandedGridView bandedView = new BandedGridView();
            bandedView.OptionsView.ShowColumnHeaders = false;
            bandedView.OptionsView.ShowGroupPanel = false;
            bandedView.OptionsView.ColumnAutoWidth = false;
            bandedView.OptionsView.ShowFooter = true;

            bandedView.OptionsBehavior.Editable = false;

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
                    SetGridBandedViewAmount(bandedView, "", "InSeam ID", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "DauSize")
                {
                    _ListString.Add("DauSize");
                    SetGridBandedViewAmount(bandedView, "", "Nhóm size", _ListString);
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
                    SetGridBandedViewAmount(bandedView, "", "Ngày DK vào Chuyền", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "NgayXuatHang")
                {
                    _ListString.Add("NgayXuatHang");
                    SetGridBandedViewAmount(bandedView, "", "Ngày Xuất Hàng", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "NgayThucTeVC")
                {
                    _ListString.Add("NgayThucTeVC");
                    SetGridBandedViewAmount(bandedView, "", "Ngày TT vào Chuyền", _ListString);
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
                // lstSize.Add(colName.Replace("@Size@", ""));
                //lstSize.Add(tab.Columns[j].ColumnName.Split('@').Last());
                string[] arrName = tab.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                maHang = arrName[1];
                listHeader.Add(tab.Columns[j].ColumnName);
                j++;

            }

            GridBand gridBand = new GridBand();
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            BandedGridColumn colAmount = bandedView.Columns.AddField("TongSL");
            SetGridBandedViewAmount(bandedView, maHang, "", listHeader);

            colAmount.OptionsColumn.AllowEdit = false;
            colAmount.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            colAmount.Caption = "Tổng";
            colAmount.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            colAmount.Summary.Add(DevExpress.Data.SummaryItemType.Sum, colAmount.FieldName, "{0:n0}");
            //colAmount.UnboundExpression = exp;
            colAmount.Visible = true;
            colAmount.AppearanceCell.ForeColor = Color.Blue;
            colAmount.AppearanceCell.Options.UseBackColor = true;
            colAmount.AppearanceCell.Options.UseForeColor = true;
            colAmount.OwnerBand = gridBand;
            gridBand.Visible = true;
            gridBand.Caption = "Tổng";

            bandedView.CustomDrawBandHeader += BandedView_CustomDrawBandHeader;
            bandedView.CustomDrawFooter += BandedView_CustomDrawFooter;
            bandedView.CustomDrawFooterCell += BandedView_CustomDrawFooterCell;
            bandedView.CustomUnboundColumnData += BandedView_CustomUnboundColumnData_1;
            bandedView.CustomDrawCell += BandedView_CustomDrawCell;
            bandedView.CustomDrawRowIndicator += BandedView_CustomDrawRowIndicator;
            bandedView.RowCountChanged += BandedView_RowCountChanged;
            bandedView.CustomColumnDisplayText += BandedView_CustomColumnDisplayText;
            bandedView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            bandedView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bandedView.OptionsNavigation.EnterMoveNextColumn = true;
            bandedView.OptionsView.AllowCellMerge = true;
            return bandedView;
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

        private void BandedView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            //BandedGridView view = (BandedGridView)sender;

            //if (view.IsRowSelected(e.RowHandle))
            //{
            //    // Thiết lập màu sắc cho dòng được chọn
            //    e.Appearance.BackColor = Color.FromArgb(255, 251, 209);
            //}
        }

        private void BandedView_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                sum = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "ID" && col.FieldName != "MaDH" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSizeID" && col.FieldName != "DauSize" && col.FieldName != "MaQG" && col.FieldName != "NgayGH" && col.FieldName != "GhiChu" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
            }
        }

        private void BandedView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            BandedGridView view = bandedGridViewChiTietDonHang;
            if (e.IsGetData)
            {
                int sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "ID" && col.FieldName != "MaDH" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSizeID" && col.FieldName != "DauSize" && col.FieldName != "MaQG" && col.FieldName != "NgayGH" && col.FieldName != "GhiChu" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        try
                        {
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

        private void BandedView_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;
            //e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            e.Appearance.ForeColor = Color.Red;
        }


        //private void BandedView_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        //{
        //    GridView view = sender as GridView;
        //    if (e.Column == null || e.RowHandle != GridControl.InvalidRowHandle)
        //        return;

        //    if (e.Column.FieldName == "YourFieldName") // Thay YourFieldName bằng tên cột của bạn
        //    {
        //        // Đặt màu cho văn bản
        //        e.Appearance.ForeColor = Color.Red; // Màu chữ
        //        e.Appearance.BackColor = Color.Yellow; // Màu nền

        //        // Các thiết lập khác như Font, Border, Alignment, v.v. có thể được thực hiện tại đây
        //    }
        //}

        private void BandedView_CustomDrawFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void BandedView_CustomUnboundColumnData_1(object sender, CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
        }


        private void BandedView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            if (e.Band == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Band.AppearanceHeader.GradientMode);
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
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;



            if (nrOfColumns == 1 && (columnNames[0] == "MaDH" || columnNames[0] == "POID" || columnNames[0] == "PO" || columnNames[0] == "ColorCode" || columnNames[0] == "MaMau" ||
                columnNames[0] == "DauSizeID" || columnNames[0] == "DauSize" || columnNames[0] == "MaQG" || columnNames[0] == "NgayGH" || columnNames[0] == "NgayDKVC" || columnNames[0] == "NgayThucTeVC"
                 || columnNames[0] == "NgayXuatHang" || columnNames[0] == "GhiChu"))
            {
                BandedGridColumn bandedColumns = (BandedGridColumn)bandedView.Columns.AddField(columnNames[0]);

                if (columnNames[0] == "PO" || columnNames[0] == "ColorCode" || columnNames[0] == "MaMau" || columnNames[0] == "DauSize"
                    || columnNames[0] == "MaQG" || columnNames[0] == "NgayGH" || columnNames[0] == "NgayDKVC" || columnNames[0] == "NgayThucTeVC" || columnNames[0] == "NgayXuatHang")
                {
                    bandedColumns.Width = 80;
                }
                else if (columnNames[0] == "GhiChu")
                {
                    bandedColumns.Width = 120;
                }

                if (columnNames[0] == "PO")
                {
                    bandedColumns.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                }
                else
                {
                    bandedColumns.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                }

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
                        dvView.Columns.Add(new GridColumn { FieldName = "MaQG", Caption = "Mã quốc gia", Name = "colMaQG", Visible = true });
                        dvView.Columns.Add(new GridColumn { FieldName = "TenQG", Caption = "Tên quốc gia", Name = "colTenQG", Visible = true });

                    }
                    bandedColumns.ColumnEdit = rCountryEdit;
                }
                else if (columnNames[0] == "MaMau")
                {
                    //string url = string.Format("{0}?", URL + "BangMau/GetBangMau");
                    DataRow dr = gridViewDonHangTong.GetFocusedDataRow();
                    if (dr == null) return;

                    string url = $"{URL}ERPDonHangTong/Get?Action=GETBANGMAU&para={dr["MaKH"].ToString()}&para2={dr["MaHang"].ToString()}";
                    //string url = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                    RepositoryItemSearchLookUpEdit rCountryEdit = new RepositoryItemSearchLookUpEdit();
                    rCountryEdit.DataSource = tbl;
                    rCountryEdit.DisplayMember = "TenMau";
                    rCountryEdit.ValueMember = "MaMau";
                    rCountryEdit.ShowClearButton = false;
                    rCountryEdit.NullText = "[Chọn giá trị]";
                    //rCountryEdit.EditValueChanged += new System.EventHandler(this.searchLookUpEditMau_EditValueChanged);

                    GridView dvView = rCountryEdit.View;
                    if (dvView.Columns.Count == 0)
                    {
                        dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                        dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                        dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                        dvView.Columns.Add(new GridColumn { FieldName = "MaMau", Caption = "Mã màu", Name = "colMaMau", Visible = true });
                        dvView.Columns.Add(new GridColumn { FieldName = "TenMau", Caption = "Tên màu", Name = "colTenMau", Visible = true });

                    }
                    bandedColumns.ColumnEdit = rCountryEdit;
                }
                else if (columnNames[0] == "NgayGH")
                {
                    bandedColumns.DisplayFormat.FormatString = "dd/MM/yyyy";
                    bandedColumns.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                }
                else if (columnNames[0] == "NgayDKVC")
                {
                    bandedColumns.DisplayFormat.FormatString = "dd/MM/yyyy";
                    bandedColumns.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                }
                else if (columnNames[0] == "NgayThucTeVC")
                {
                    bandedColumns.DisplayFormat.FormatString = "dd/MM/yyyy";
                    bandedColumns.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                }
                else if (columnNames[0] == "NgayXuatHang")
                {
                    bandedColumns.DisplayFormat.FormatString = "dd/MM/yyyy";
                    bandedColumns.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                }
                else if (columnNames[0] == "MaDH" || columnNames[0] == "POID" || columnNames[0] == "PO" || columnNames[0] == "MaMau" || columnNames[0] == "DauSizeID" || columnNames[0] == "DauSize")
                {
                    bandedColumns.OptionsColumn.AllowEdit = false;
                }

                String CaptionName = columnNames[0];
                bandedColumns.OwnerBand = gridBand;
                bandedColumns.Caption = GridBandCaption;
                if (columnNames[0] == "MaDH" || columnNames[0] == "POID" || columnNames[0] == "DauSizeID")
                {
                    gridBand.Visible = false;

                }
                bandedColumns.Visible = true;
                //bandedColumns.Width = 120;

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
                    bandedColumns[i].Width = 50;
                    bandedColumns[i].Summary.Add(DevExpress.Data.SummaryItemType.Sum, bandedColumns[i].FieldName, "{0:n0}");
                    gridband3.Columns.Add(bandedColumns[i]);
                    bandedColumns[i].OwnerBand = gridband3;
                    bandedColumns[i].Visible = true;
                    gridband3.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    gridband3.AppearanceHeader.Options.UseFont = true;
                    gridband3.AppearanceHeader.Options.UseTextOptions = true;
                    gridband3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    grHeader[i] = gridband3;

                }
                gridBand.Children.AddRange(grHeader);
                bandedView.Bands.Add(gridBand);
            }
        }

        private void BtThem()
        {
            string urlID = string.Format("{0}?", URL + "DonHangTong/GetDonHangTongID");
            string jsonID = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlID); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonID);
            frmNhapDonHangTong frm = new frmNhapDonHangTong(Convert.ToInt32(tbl.Rows[0][0]), _allowAdd, _allowEdit, _allowDelete);
            frm.WindowState = FormWindowState.Maximized;
            //frm.ShowDialog();
            //LoadDSDonHangTong();

            frm.Show();
            //LoadDSDonHangTong();
            frm.FormClosed += (s, e) =>
            {
                LoadDSDonHangTong();
            };
        }

        private void btThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtThem();
        }

        private void BtSua()
        {
            //List<DonHangTongEntity> lstDonHangTong = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(json);
            List<DonHangTongEntity> lstDonHangTong = new List<DonHangTongEntity>();
            if (!string.IsNullOrEmpty(maHangSearchLookup))
            {
                DataTable dt = gridDonHangTong.DataSource as DataTable;
                string jsonsua = JsonConvert.SerializeObject(dt);
                lstDonHangTong = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(jsonsua);
            }
            else
            {
                DataTable dt = gridDonHangTong.DataSource as DataTable;
                string jsonsua = JsonConvert.SerializeObject(dt);
                lstDonHangTong = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(jsonsua);
                //lstDonHangTong = JsonConvert.DeserializeObject<List<DonHangTongEntity>>(json);
            }
            int rowFocused = gridViewDonHangTong.GetFocusedDataSourceRowIndex();
            if (rowFocused >= 0)
            {
                frmSuaDonHangTong frm = new frmSuaDonHangTong(lstDonHangTong[rowFocused], (gridControlChiTietDonHang.DataSource as DataTable), _allowAdd, _allowEdit, _allowDelete, _nguoitao);
                frm.WindowState = FormWindowState.Maximized;
                //frm.ShowDialog();
                //LoadDSDonHangTong();

                frm.Show();
                //LoadDSDonHangTong();
                frm.FormClosed += (s, e) =>
                {
                    LoadDSDonHangTong();
                };
            }
        }

        private void btSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            rowhandle = gridViewDonHangTong.FocusedRowHandle;
            BtSua();
        }

        private void gridViewDonHangTong_GroupRowCollapsed(object sender, DevExpress.XtraGrid.Views.Base.RowEventArgs e)
        {
            gridViewDonHangTong.ExpandAllGroups();
        }

        private void NapLai()
        {
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            barEditItem1.EditValue = null;
            LoadDSDonHangTong();
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }

        private void btNaplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barEditItemcbLoc.EditValue = repositoryItemComboBox1.Items[0].ToString();
            NapLai();
        }

        private void barButtonItemPrev_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (pageIndex > 1)
            {
                pageIndex = pageIndex - 1;
                barStaticItemPageIndex.Caption = pageIndex.ToString();
            }
            LoadDSDonHangTong(false);
        }

        private void gridViewDonHangTong_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
                    int nNewSize = Convert.ToInt32(size.Width) + GridPainter.Indicator.ImageSize.Width + 15;
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

        private void gridViewDonHangTong_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void chartControl1_CustomDrawAxisLabel(object sender, CustomDrawAxisLabelEventArgs e)
        {

        }

        private void chartControl1_CustomDrawCrosshair(object sender, CustomDrawCrosshairEventArgs e)
        {

        }

        private void chartControl1_CustomDrawSeries(object sender, CustomDrawSeriesEventArgs e)
        {

            Console.WriteLine("CustomDrawSeries");
            //if (e.Object is ChartToolTipController)
            //{

            //}
            //// Kiểm tra nếu đây là sự kiện CustomDrawSeries cho Tooltip
            //if (e.Element == DevExpress.XtraCharts.ViewElement.ToolTip)
            //{
            //    // Lấy thông tin điểm dữ liệu đang được di chuột
            //    SeriesPoint point = e.SeriesPoint;

            //    // Tạo nội dung Tooltip tùy chỉnh
            //    string tooltipText = $"Value: {point.Values[0]}";

            //    // Vẽ Tooltip tùy chỉnh
            //    e.DrawOptions.Content = tooltipText;

            //    // Đặt màu nền và màu chữ cho Tooltip
            //    e.DrawOptions.BackColor = Color.LightGray;
            //    e.DrawOptions.ForeColor = Color.Black;
            //}
        }

        private void HandleLoadDataChiTiet()
        {
            if (gridViewDonHangTong.FocusedRowHandle >= 0)
            {
                indexFocusedRow = gridViewDonHangTong.FocusedRowHandle;
            }
            GridView view = this.gridViewDonHangTong;
            //if (view.IsGroupRow(view.FocusedRowHandle))
            //{
            int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
            string donhang = view.GetFocusedRowCellValue(colMaDH) as string;
            object mahang = view.GetRowCellValue(childHandle, colMaHang);
            object soluong = view.GetRowCellValue(childHandle, colSLTong);
            object ngaytao = view.GetRowCellValue(childHandle, colNgayTao);
            object chungloai = view.GetRowCellValue(childHandle, colMaCL);
            object maGop = view.GetFocusedRowCellValue(colMaGop);
            //object nguoitao = view.GetRowCellValue(childHandle, colNguoiTao);


            if (donhang != null) _maDonHang = donhang.ToString();
            if (mahang != null) _mahang = mahang.ToString();
            if (soluong != null) _soluong = soluong.ToString();
            if (ngaytao != null) _ngaytao = ngaytao.ToString();
            if (chungloai != null) _chungloai = chungloai.ToString();
            // if (nguoitao != null) _nguoitao = nguoitao.ToString();
            GetChiTietDonHangTongPO();

            if (maGop != null)
            {
                GetDataChart(maGop.ToString());
            }

        }

        private void gridViewDonHangTong_RowClick(object sender, RowClickEventArgs e)
        {
            //HandleLoadDataChiTiet();
        }

        private void barButtonItemNext_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pageIndex = pageIndex + 1;
            barStaticItemPageIndex.Caption = pageIndex.ToString();
            LoadDSDonHangTong(false);
        }
        private void ProcessFocusedRow(int rowHandle)
        {
            if (rowHandle < 0)
            {
                _focusedMaDH = string.Empty;
                _focusedMaHang = string.Empty;
                _focusedRowHandle = -1;
                return;
            }

            DataTable dt = gridDonHangTong.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0) return;

            DataRow focusedRow = gridViewDonHangTong.GetDataRow(rowHandle);
            if (focusedRow == null) return;

            _focusedMaDH = focusedRow["MaDH"]?.ToString() ?? string.Empty;
            _focusedMaHang = focusedRow["MaHang"]?.ToString() ?? string.Empty;
            _focusedRowHandle = rowHandle;
        }

        private void gridViewDonHangTong_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            ProcessFocusedRow(e.FocusedRowHandle);
            object nguoitao = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                nguoitao = view.GetRowCellValue(childHandle, colNguoiTao);
            }
            else
            {
                nguoitao = view.GetFocusedRowCellValue(colNguoiTao);
            }
            if (nguoitao != null) _nguoitao = nguoitao.ToString();
            _rowhandle = gridViewDonHangTong.FocusedRowHandle;
            HandleLoadDataChiTiet();
            LoadTab();
            loadDataToolTip();
            loadDataCanDoiLogStyle();
            ////tam
            //loadCanDoiNPLTab(e.FocusedRowHandle);
            ////tam
        }

        private void gridViewDonHangTong_DataSourceChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            object nguoitao = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                nguoitao = view.GetRowCellValue(childHandle, colNguoiTao);
            }
            else
            {
                nguoitao = view.GetFocusedRowCellValue(colNguoiTao);
            }
            if (nguoitao != null) _nguoitao = nguoitao.ToString();
            _rowhandle = gridViewDonHangTong.FocusedRowHandle;
            LoadTab();
        }

        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BarButtonItemLink link = (BarButtonItemLink)e.Link;
            if (link != null)
            {
                // Lấy BarManager từ BarButtonItem
                BarManager barManager = link.Item.Manager;
                // Lấy tọa độ trên màn hình của BarButtonItem
                Point barButtonLocation = link.Bounds.Location;
                // Chuyển đổi tọa độ từ thanh công cụ sang màn hình
                Point screenLocation = barManager.Form.PointToScreen(barButtonLocation);

                frmSearchDonHang frm = new frmSearchDonHang(screenLocation.X, screenLocation.Y + link.Bounds.Height);
                // Đăng ký lắng nghe sự kiện OnDataUpdate
                frm.OnDataUpdate += (s, tblSearch) =>
                {
                    Console.WriteLine("tblSearch");
                    gridDonHangTong.DataSource = tblSearch;
                    gridDonHangTong.RefreshDataSource();
                    gridViewDonHangTong.FocusedRowHandle = 0;
                    ProcessFocusedRow(0);
                    HandleLoadDataChiTiet();//**
                };
                frm.ShowDialog();
            }
        }

        private void barEditItemcbLoc_EditValueChanged(object sender, EventArgs e)
        {
            if (barEditItem1.EditValue != null)
            {
                maHangSearchLookup = barEditItem1.EditValue.ToString();
            }
            else
            {
                maHangSearchLookup = string.Empty;
            }

            LoadDSDonHangTong();
        }

        private void repositoryItemSpinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            SpinEdit spinEdit = sender as SpinEdit;
            if (spinEdit != null && spinEdit.EditValue != null && Convert.ToInt32(spinEdit.EditValue) >= 0)
            {
                pageSize = Convert.ToInt32(spinEdit.EditValue);
                LoadDSDonHangTong();
            }
        }

        private void gridViewDonHangTong_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                DataRow row = null;
                try { row = view.GetDataRow(e.RowHandle); } catch { row = null; }
                if (e.RowHandle >= 0)
                {
                    string Strflth = view.GetRowCellDisplayText(e.RowHandle, view.Columns["SLTH"]);
                    string Strfcl = view.GetRowCellDisplayText(e.RowHandle, view.Columns["SLCL"]);
                    string strCheckDuyet = row["CheckDuyetNPL"].ToString();
                    string strCheckBom = row["CheckBom"].ToString();
                    //if (strCheckBom == "0")
                    //{
                    //    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcad4");
                    //    e.HighPriority = true;
                    //    return;
                    //}
                    //if (strCheckDuyet == "1")
                    //{
                    //    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#43BDF9");
                    //    e.HighPriority = true;
                    //    return;
                    //}
                    if (Strfcl == "0")
                    {
                        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#84e184");
                        e.HighPriority = true;
                    }
                    if (Strflth != "0" && Strfcl != "0")
                    {
                        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffff99");
                        e.HighPriority = true;
                    }
                    if (e.RowHandle == gridViewDonHangTong.FocusedRowHandle)
                    {
                        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ddd");
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                        e.HighPriority = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridDonHangTong.DataSource != null)
            {
                if (GlobleData.UserName.ToString().ToUpper() == _nguoitao.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
                    BtXoa();
                else
                {
                    XtraMessageBox.Show("User này không phải là người tạo, không có quyền xóa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


        }
        private bool CheckCanDoi(string _maDH)
        {
            string url = string.Format("{0}?maDH={1}", URL + "DonHangTong/CheckCanDoi", _maDH);
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            DataTable tblCheckCanDoi = JsonConvert.DeserializeObject<DataTable>(json);

            if (tblCheckCanDoi != null && tblCheckCanDoi.Rows.Count > 0)
            {
                int isKeoVe = int.Parse(tblCheckCanDoi.Rows[0]["IsKeoVe"].ToString());
                switch (isKeoVe)
                {
                    case 0:
                        DialogResult result = MessageBox.Show(string.Format("Đơn hàng: {0} đã được chia sản xuất. Bạn có chắc muốn xóa Không?", _maDH),
                            "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            // Xóa DonHangTongPOChiTiet
                            //tbl.Rows.RemoveAt(foucsedDelete);
                            //XoaDonHangTongPOChiTiet(tblXoa, _poID, _maMau, _dauSizeID);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case 1:
                        XtraMessageBox.Show(string.Format("Đơn hàng: {0} đã được lấy về sản xuất, không thể thực hiện xóa!", _maDH),
                            Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        return false;
                        break;
                }
            }
            return true;
        }
        private void BtXoa()
        {
            try
            {
                bool isCheckCanDoi = CheckCanDoi(_maDonHang);
                if (isCheckCanDoi)
                {
                    getCTTong();
                    if (_maDonHang == "" || _maDonHang == null)
                    {
                        return;
                    }
                    DialogResult result = MessageBox.Show("Bạn có muốn xóa mã hàng " + _maDonHang + " này không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string url = string.Format("{0}/?madonhang={1}&&username={2}", URL + "DonHangTong/Delete", _maDonHang, GlobleData.UserName);
                        string json = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                        LoadDSDonHangTong();
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridViewDonHangTong_DoubleClick(object sender, EventArgs e)
        {
            //getCTTong();
            //frmDonHangChiTiet frm = new frmDonHangChiTiet(_maDonHang, _mahang, _soluong, _ngaytao, _chungloai);
            //frm.ShowDialog();
            //LoadDSDonHangTong();

        }
        private void getCTTong()
        {

            DataRowView row = gridViewDonHangTong.GetFocusedRow() as DataRowView;
            if (row == null)
            {
                return;
            }
            _maDonHang = row.Row.ItemArray[1].ToString();
            _mahang = row.Row.ItemArray[4].ToString();
            _soluong = row.Row.ItemArray[10].ToString();
            _ngaytao = row.Row.ItemArray[9].ToString();
            _chungloai = row.Row.ItemArray[6].ToString();
        }
        private void LoadTab()
        {
            if (xtraTabControl1.SelectedTabPageIndex == 0)
            {
                GridView view = gridViewDonHangTong;
                HandleLoadDataChiTiet();
            }
            if (xtraTabControl1.SelectedTabPageIndex == 2)
            {
                GridView view = gridViewDonHangTong;
                GetSearchLookUpDVSX();
                //InitChartV1();
                LoadLenhSXV1(view);
                GetDataChartV1();
            }
            if (xtraTabControl1.SelectedTabPageIndex == 3)
            {
                GridView view = gridViewDonHangTong;
                DataRowView dataRowView = view.GetFocusedRow() as DataRowView;
                if (dataRowView != null)
                {
                    DataRow row = dataRowView.Row;
                    string parameter = string.Format("{0}", row["MaGop"].ToString());
                    LoadDSDinhMucDonHang(parameter);
                }
            }
            if (xtraTabControl1.SelectedTabPageIndex == 4)
            {
                GridView view = gridViewDonHangTong;
                LoadLenhSX(this.gridViewDonHangTong);

            }
            if (xtraTabControl1.SelectedTabPageIndex == 1)
            {
                GridView view = gridViewDonHangTong;
                DataRowView dataRowView = view.GetFocusedRow() as DataRowView;
                if (dataRowView != null)
                {
                    DataRow row = dataRowView.Row;
                    string parameter = string.Format("{0}", row["MaGop"].ToString());
                    string parameter1 = string.Format("{0}", row["MaKH"].ToString());
                    string paramete2 = string.Format("{0}", row["MaHang"].ToString());
                    //LoadDSDinhMucKH(parameter, parameter1, paramete2);
                    loadCanDoiNPLTab(gridViewDonHangTong.FocusedRowHandle);
                }

            }
        }
        private void xtraTabControl1_Click(object sender, EventArgs e)
        {
            LoadTab();
            loadDataToolTip();
            loadDataCanDoiLogStyle();

        }
        private void LoadLenhSXV1(GridView view)
        {
            object _objMaDH = null, _objMaHang = null, _objTenHang = null, _objDot = null, _objMaLenh = null, _objPOID = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaDH = view.GetRowCellValue(childHandle, colMaDH);
                _objMaHang = view.GetRowCellValue(childHandle, colMaHang);
                _objTenHang = view.GetRowCellValue(childHandle, colTenHang);
                _objDot = view.GetRowCellValue(childHandle, colDot);
                _objPOID = view.GetRowCellValue(childHandle, colPOID);
            }
            else
            {
                _objMaDH = view.GetFocusedRowCellValue(colMaDH);
                _objMaHang = view.GetFocusedRowCellValue(colMaHang);
                _objTenHang = view.GetFocusedRowCellValue(colTenHang);
                _objDot = view.GetFocusedRowCellValue(colDot);
                _objPOID = view.GetFocusedRowCellValue(colPOID);
            }
            string urlLSX = string.Format("{0}?madh={1}&&poid={2}", URL + "CanDoiDonHangTong/GetLenhSX", _objMaDH, _objPOID);
            string jsonLSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlLSX); }).Result;
            DataTable _dtDataLSX = JsonConvert.DeserializeObject<DataTable>(jsonLSX);
            gridControl1.DataSource = _dtDataLSX;
            if (gridView1.FocusedRowHandle < 0)
                gridView1.FocusedRowHandle = 1;
            GetDataChartV1();

        }

        private void GetDataChartV1()
        {
            string maLenhSX = string.Empty;
            if (gridView1.GetFocusedRow() != null)
            {
                DataRow row = (gridView1.GetFocusedRow() as DataRowView).Row;

                if (row != null && row["MaLenhSanXuat"] != null && !string.IsNullOrEmpty(row["MaLenhSanXuat"].ToString()))
                {
                    Console.WriteLine("GetDataChart");
                    maLenhSX = row["MaLenhSanXuat"].ToString();

                }
            }
            else
            {
                maLenhSX = "";
            }

            GetDataChartV1(maLenhSX);
        }

        private void GetDataChartV1(string maLenhSX)
        {
            // CanDoi
            if (!string.IsNullOrEmpty(maLenhSX))
            {
                List<BaoCaoEntity> lstBaoCao = new List<BaoCaoEntity>();
                string urlBaoCao = string.Format("{0}?maLenhSX={1}", URL + "CanDoiDonHangTong/GetDsBaoCao", maLenhSX);
                string jsonBaoCao = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlBaoCao); }).Result;
                DataTable tblDsBaoCao = JsonConvert.DeserializeObject<DataTable>(jsonBaoCao);
                DrawChartV1(tblDsBaoCao);
            }
            else
            {
                DrawChartV1(null);
            }


        }
        private void DrawChartV1(DataTable tblBaoCao)
        {
            // Tạo và thêm dữ liệu mẫu vào biểu đồ

            DevExpress.XtraCharts.SeriesPoint seriesPoint2 = new DevExpress.XtraCharts.SeriesPoint()
                , seriesPoint3 = new DevExpress.XtraCharts.SeriesPoint()
                , seriesPoint4 = new DevExpress.XtraCharts.SeriesPoint()
                , seriesPoint5 = new DevExpress.XtraCharts.SeriesPoint()
                , seriesPoint6 = new DevExpress.XtraCharts.SeriesPoint();
            double soLuong = -1;
            if (tblBaoCao != null && tblBaoCao.Rows.Count > 0)
            {
                foreach (DataColumn column in tblBaoCao.Columns)
                {
                    switch (column.ColumnName)
                    {
                        case "PackageList":
                            soLuong = double.Parse(tblBaoCao.Rows[0][column].ToString());
                            seriesPoint2 = new DevExpress.XtraCharts.SeriesPoint("Package List", new object[] { (soLuong) });
                            seriesPoint2.Color = FillColor(soLuong);
                            break;
                        case "DaDongThung":
                            soLuong = double.Parse(tblBaoCao.Rows[0][column].ToString());
                            seriesPoint3 = new DevExpress.XtraCharts.SeriesPoint("Đóng thùng", new object[] { (soLuong) });
                            seriesPoint3.Color = FillColor(soLuong);

                            break;
                        case "NhapKho":
                            soLuong = double.Parse(tblBaoCao.Rows[0][column].ToString());
                            seriesPoint4 = new DevExpress.XtraCharts.SeriesPoint("Nhập kho", new object[] { (soLuong) });
                            seriesPoint4.Color = FillColor(soLuong);

                            break;
                        case "LuanChuyen":
                            soLuong = double.Parse(tblBaoCao.Rows[0][column].ToString());
                            seriesPoint5 = new DevExpress.XtraCharts.SeriesPoint("Luân chuyển", new object[] { (soLuong) });
                            seriesPoint5.Color = FillColor(soLuong);

                            break;
                        case "XuatHang":
                            soLuong = double.Parse(tblBaoCao.Rows[0][column].ToString());
                            seriesPoint6 = new DevExpress.XtraCharts.SeriesPoint("Xuất hàng", new object[] { (soLuong) });
                            seriesPoint6.Color = FillColor(soLuong);

                            break;
                    }
                }
            }
            else
            {
                soLuong = double.Parse("0");

                seriesPoint2 = new DevExpress.XtraCharts.SeriesPoint("Package List", new object[] { (soLuong) });
                seriesPoint2.Color = FillColor(soLuong);

                seriesPoint3 = new DevExpress.XtraCharts.SeriesPoint("Đóng thùng", new object[] { (soLuong) });
                seriesPoint3.Color = FillColor(soLuong);

                seriesPoint4 = new DevExpress.XtraCharts.SeriesPoint("Nhập kho", new object[] { (soLuong) });
                seriesPoint4.Color = FillColor(soLuong);

                seriesPoint5 = new DevExpress.XtraCharts.SeriesPoint("Luân chuyển", new object[] { (soLuong) });
                seriesPoint5.Color = FillColor(soLuong);

                seriesPoint6 = new DevExpress.XtraCharts.SeriesPoint("Xuất hàng", new object[] { (soLuong) });
                seriesPoint6.Color = FillColor(soLuong);
            }

            // Code mới
            if (chartControl2 != null && chartControl2.Series != null && chartControl2.Series.Count > 0)
            {
                Series seriesChartControl = chartControl2.Series[0];
                if (seriesChartControl != null)
                {
                    seriesChartControl.Points.Clear();
                    if (seriesChartControl.Points != null || (seriesChartControl.Points != null && seriesChartControl.Points.Count > 0))
                    {
                        seriesChartControl.Points.Clear();
                    }

                    seriesChartControl.Points.AddRange(new DevExpress.XtraCharts.SeriesPoint[] {
            seriesPoint6,
            seriesPoint5,
            seriesPoint4,
            seriesPoint3,
            seriesPoint2});
                }
            }
        }
        private void LoadLenhSX(GridView view)
        {
            object _objMaDH = null, _objMaHang = null, _objTenHang = null, _objDot = null, _objMaLenh = null, _objPOID = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaDH = view.GetRowCellValue(childHandle, colMaGop);
            }
            else
            {
                _objMaDH = view.GetFocusedRowCellValue(colMaGop);
            }

            string urlLSX = string.Format("{0}?madh={1}&&madvsx={2}", URL + "CanDoiDonHangTong/GetChiTietLenhSX", _objMaDH, "");
            string jsonLSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlLSX); }).Result;
            DataTable _dtDataLSX = JsonConvert.DeserializeObject<DataTable>(jsonLSX);


            string urlLcc = string.Format("{0}?madh={1}", URL + "CanDoiDonHangTong/GetChuyenChinh", _objMaDH);
            string jsonLcc = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlLcc); }).Result;
            _dtbCcCache = JsonConvert.DeserializeObject<DataTable>(jsonLcc);

            gridControl2.MainView = GetBandGridViewAmountV2(_dtDataLSX);
            //gridControl2.MainView = GetBandGridViewAmountV1(_dtDataLSX);
            BandedGridView mainView = (BandedGridView)gridControl2.MainView;
            mainView.CustomUnboundColumnData += MainView_CustomUnboundColumnData;
            mainView.CustomSummaryCalculate += MainView_CustomSummaryCalculate;
            mainView.CustomDrawBandHeader += MainView_CustomDrawBandHeader;
            mainView.CustomDrawFooter += MainView_CustomDrawFooter;
            mainView.CustomDrawFooterCell += MainView_CustomDrawFooterCell;
            mainView.CustomColumnDisplayText += MainView_CustomColumnDisplayText;
            mainView.CustomDrawGroupRow += MainView_CustomDrawGroupRow;
            mainView.CustomDrawRowFooter += MainView_CustomDrawRowFooter;
            mainView.RowCellStyle += gridView2_RowCellStyle;
            //mainView.RowClick += gridView1_RowClick;
            gridControl2.DataSource = _dtDataLSX;

        }
        public void LoadDSDinhMucDonHang(string parameter)
        {
            GridView views = gridView1;
            object _objMaLenhSanXuat = null;
            if (views.IsGroupRow(views.FocusedRowHandle))
            {
                int childHandle = views.GetChildRowHandle(views.FocusedRowHandle, 0);
                _objMaLenhSanXuat = views.GetRowCellValue(childHandle, colMaLenhSanXuat);
            }
            else
            {
                if (views.FocusedRowHandle < 0) gridView1.FocusedRowHandle = 0;
                _objMaLenhSanXuat = views.GetFocusedRowCellValue(colMaLenhSanXuat);
            }
            string url = string.Format("{0}?parameter={1}&&malenhsanxuat={2}", URL + "CanDoiDonHangTong/GetGomNPL", parameter, _objMaLenhSanXuat);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0 || tbl == null)
            {
                NtbSoft.ERP.Libs.clsConvert<DinhMucEntity> convert = new Libs.clsConvert<DinhMucEntity>();
                List<DinhMucEntity> lstDinhMuc = new List<DinhMucEntity>();
                DataTable tblnull = convert.ToDataTable(lstDinhMuc);
                gridDinhMucNguyenPhuLieu.DataSource = tblnull;
            }
            //else
            //    gridDinhMucNguyenPhuLieu.DataSource = tbl;
            DataTable dtAllSizes = GetBangSize(parameter);
            DataTable dtERPSizes = GetERPSIZESP(parameter);
            tbl = XuLyCotSize(tbl, dtAllSizes, dtERPSizes);

            gridDinhMucNguyenPhuLieu.DataSource = tbl;
        }
        public DataTable GetBangSize(string parameter)
        {
            string url = string.Format("{0}?parameter={1}&action=GetBangSize", URL + "CanDoiDonHangTong/GetSize", parameter);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0 || tbl == null)
            {
                return null;
            }
            else
                return tbl;
        }
        public DataTable GetERPSIZESP(string parameter)
        {
            string url = string.Format("{0}?parameter={1}&action=GetERPSIZESP", URL + "CanDoiDonHangTong/GetSize", parameter);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0 || tbl == null)
            {
                return null;
            }
            else
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
        public void LoadDSDinhMucKH(string parameter, string parameter1, string parameter2)
        {
            GridView views = gridView1;
            object _objMaLenhSanXuat = null;
            if (views.IsGroupRow(views.FocusedRowHandle))
            {
                int childHandle = views.GetChildRowHandle(views.FocusedRowHandle, 0);
                _objMaLenhSanXuat = views.GetRowCellValue(childHandle, colMaLenhSanXuat);
            }
            else
            {
                if (views.FocusedRowHandle < 0) gridView1.FocusedRowHandle = 0;
                _objMaLenhSanXuat = views.GetFocusedRowCellValue(colMaLenhSanXuat);
            }
            string url = string.Format("{0}?parameter={1}&&parameter1={2}&&parameter2={3}", URL + "CanDoiDonHangTong/GetGomNPLKH", parameter, parameter1, parameter2);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl.Rows.Count == 0 || tbl == null)
            {
                NtbSoft.ERP.Libs.clsConvert<DinhMucEntity> convert = new Libs.clsConvert<DinhMucEntity>();
                List<DinhMucEntity> lstDinhMuc = new List<DinhMucEntity>();
                DataTable tblnull = convert.ToDataTable(lstDinhMuc);
                gridControl3.DataSource = tblnull;
            }
            else
                gridControl3.DataSource = tbl;
        }
        private void gridViewDinhMucNguyenPhuLieu_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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


        private DevExpress.XtraGrid.Views.Base.BaseView GetBandGridViewAmountV1(DataTable tab)
        {
            BandedGridView bandedView = new BandedGridView();
            bandedView.OptionsView.ShowColumnHeaders = false;
            bandedView.OptionsView.ShowGroupPanel = false;
            bandedView.OptionsView.ColumnAutoWidth = false;
            bandedView.OptionsView.ShowFooter = true;
            bandedView.OptionsView.AllowCellMerge = true;
            bandedView.OptionsBehavior.AutoExpandAllGroups = true;
            bandedView.OptionsCustomization.AllowBandMoving = false;
            //bandedView.OptionsView.GroupFooterShowMode = GroupFooterShowMode.VisibleAlways;
            if (tab.Columns.Count == 0) return bandedView;
            for (int i = 0; i < 17; i++)
            {
                List<string> _ListString = new List<string>();
                if (tab.Columns[i].ColumnName == "TenDVSX")
                {
                    _ListString.Add("TenDVSX");
                    SetGridBandedViewAmountV1(bandedView, "", "ĐƠN VỊ SX", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "MaLenh")
                {
                    _ListString.Add("MaLenh");
                    SetGridBandedViewAmountV1(bandedView, "", "Mã lệnh SX", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "STUFFED_MaDH")
                {
                    _ListString.Add("STUFFED_MaDH");
                    SetGridBandedViewAmountV1(bandedView, "", "Đơn Hàng Gộp", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "PO")
                {
                    _ListString.Add("PO");
                    SetGridBandedViewAmountV1(bandedView, "", "PO", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "TenMau")
                {
                    _ListString.Add("TenMau");
                    SetGridBandedViewAmountV1(bandedView, "", "Màu", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "DauSize")
                {
                    _ListString.Add("DauSize");
                    SetGridBandedViewAmountV1(bandedView, "", "InSeam", _ListString);
                    _ListString.Clear();
                }
            }
            List<string> listHeader = new List<string>();
            int j = 17;
            string maHang = string.Empty;
            while (j < tab.Columns.Count)
            {
                string[] arrName = tab.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                maHang = arrName[1];
                listHeader.Add(tab.Columns[j].ColumnName);
                j++;

            }
            GridBand gridBand = new GridBand();
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            BandedGridColumn colAmount = bandedView.Columns.AddField("Amount");
            SetGridBandedViewAmountV1(bandedView, maHang, "", listHeader);

            colAmount.OptionsColumn.AllowEdit = false;
            colAmount.Caption = "Tổng";
            colAmount.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            //colAmount.UnboundExpression = exp;
            colAmount.Visible = true;
            colAmount.OwnerBand = gridBand;
            colAmount.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            gridBand.Visible = true;
            gridBand.Caption = "Tổng";
            GridColumnSummaryItem item = colAmount.Summary.Add(SummaryItemType.Custom);
            item.FieldName = "Amount";
            item.DisplayFormat = "{0:n0}";
            bandedView.Bands.Add(gridBand);
            foreach (BandedGridColumn col in bandedView.Columns)
            {

                if (col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "TenMau" && col.FieldName != "DauSize" && col.FieldName != "MaKH"
                    && col.FieldName != "MaDH" && col.FieldName != "MaHang" && col.FieldName != "TenHang" && col.FieldName != "DotSX" && col.FieldName != "MaQG"
                    && col.FieldName != "DauSizeID" && col.FieldName != "MaDVSX" && col.FieldName != "TenDVSX" && col.FieldName != "MaLenh")
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
            bandedView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            bandedView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bandedView.OptionsNavigation.EnterMoveNextColumn = true;
            return bandedView;
        }

        private void SetGridBandedViewAmountV1(BandedGridView bandedView, string GridBandHeader, string GridBandCaption, List<string> columnNames)
        {

            GridBand gridBand = new GridBand();
            if (GridBandHeader == "")
                gridBand.Caption = GridBandCaption;
            else
                gridBand.Caption = GridBandHeader;

            int nrOfColumns = columnNames.Count;
            gridBand.Fixed = FixedStyle.None;
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            if (columnNames[0] == "TenDVSX")
            {
                gridBand.Visible = false;

            }

            if (nrOfColumns == 1 && (columnNames[0] == "POID" || columnNames[0] == "PO" || columnNames[0] == "TenMau" || columnNames[0] == "DauSize" || columnNames[0] == "MaQG" || columnNames[0] == "MaDH" || columnNames[0] == "DauSizeID")
                || columnNames[0] == "MaKH" || columnNames[0] == "MaHang" || columnNames[0] == "TenHang" || columnNames[0] == "DauSizeID" || columnNames[0] == "TenDVSX" || columnNames[0] == "MaLenh" || columnNames[0] == "STUFFED_MaDH" || columnNames[0] == "Name" || columnNames[0] == "LC")
            {
                BandedGridColumn bandedColumns = (BandedGridColumn)bandedView.Columns.AddField(columnNames[0]);
                bandedColumns.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

                if (columnNames[0] == "PO" || columnNames[0] == "MaLenh" || columnNames[0] == "STUFFED_MaDH" || columnNames[0] == "Name" || columnNames[0] == "LC")
                {
                    bandedColumns.Visible = false;
                    bandedColumns.OptionsColumn.AllowEdit = false;
                    bandedColumns.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                }
                if (columnNames[0] == "TenDVSX")
                {
                    bandedColumns.Visible = false;
                    bandedColumns.GroupIndex = 0;

                }

                String CaptionName = columnNames[0];
                bandedColumns.OwnerBand = gridBand;

                bandedColumns.Caption = GridBandCaption;
                bandedColumns.Visible = true;
                bandedColumns.Width = 90;

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
                    if (_colName[0].ToString() != "TenDVSX")
                    {
                        GridBand gridband3 = new GridBand();
                        gridband3.Caption = _colName[2];
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

                }

                gridBand.Children.AddRange(grHeader);
                bandedView.Bands.Add(gridBand);

            }
        }

        private void MainView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {

            if (e.Band == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Band.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }

        private void MainView_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                sum = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "TenMau" && col.FieldName != "DauSize" && col.FieldName != "MaKH"
                    && col.FieldName != "MaDH" && col.FieldName != "MaHang" && col.FieldName != "TenHang" && col.FieldName != "DotSX" && col.FieldName != "MaQG"
                    && col.FieldName != "DauSizeID" && col.FieldName != "MaDVSX" && col.FieldName != "TenDVSX" && col.FieldName != "MaLenh" && col.FieldName != "STUFFED_MaDH" && col.FieldName != "Name" && col.FieldName != "LC" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
            }
        }

        private void MainView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.IsGetData)
            {
                int sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "TenMau" && col.FieldName != "DauSize" && col.FieldName != "MaKH"
                    && col.FieldName != "MaDH" && col.FieldName != "MaHang" && col.FieldName != "TenHang" && col.FieldName != "DotSX" && col.FieldName != "MaQG"
                    && col.FieldName != "DauSizeID" && col.FieldName != "MaDVSX" && col.FieldName != "TenDVSX" && col.FieldName != "MaLenh" && col.FieldName != "STUFFED_MaDH" && col.FieldName != "Name" && col.FieldName != "LC" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        try
                        {
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

        private void MainView_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void MainView_CustomDrawRowFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            if (view.IsGroupRow(e.RowHandle))
            {
                e.Appearance.BackColor = Color.Yellow;
            }
        }

        //private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        //{
        //    DataRow _rowFocus = gridView1.GetFocusedDataRow();
        //    if (_rowFocus == null || e.Menu == null) return;
        //    DXMenuItem menuCanDoiDMNL = new DXMenuItem();
        //    menuCanDoiDMNL.Caption = "Xóa cấp phát Lệnh SX";
        //    menuCanDoiDMNL.Appearance.Font = new Font("Arial", 9); // Đặt size là 12
        //    menuCanDoiDMNL.Click += xoaLenhCapPhatMenu;
        //    e.Menu.Items.Add(menuCanDoiDMNL);
        //}
        private void btnXoaCPLenh_Click(object sender, EventArgs e)
        {
            DataRow focusedRow = gridView1.GetFocusedDataRow();
            if (focusedRow == null) return;
            string magop = focusedRow["MaGop"]?.ToString() ?? "";
            //string madot = focusedRow["MaDot"]?.ToString() ?? "";
            string maLenhSX = focusedRow["MaLenhSanXuat"]?.ToString() ?? "";
            bool hasDataInSDKH = false;
            string urlCheckTNC = $"{URL}KhoiTaoBOMV1/Get?action=CheckBOMTNCDELCP&para4={magop}&para5={maLenhSX}";
            string jsonCheckTNC = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCheckTNC); }).Result;
            if (!string.IsNullOrEmpty(jsonCheckTNC))
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonCheckTNC);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    int hasDataTNC = 0;
                    int.TryParse(tbl.Rows[0]["HasDataTNC"]?.ToString(), out hasDataTNC);
                    if (hasDataTNC == 1)
                    {
                        hasDataInSDKH = true;
                    }
                    else
                    {
                        hasDataInSDKH = false;
                    }
                }
            }
            var meg = "";
            if (hasDataInSDKH)
            {
                meg = meg + "Lệnh này đã được tác nghiệp cắt!\n";
            }
            var confirm = XtraMessageBox.Show(meg +
                    "Bạn có chắc chắn xóa cấp phát của lệnh này không?",
                    "Xác nhận xóa cấp phát",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

            if (confirm != DialogResult.Yes)
                return;
            xoaLenhCapPhatMenu();
        }
        private void xoaLenhCapPhatMenu()
        {
            DataRow _rowFocus = gridView1.GetFocusedDataRow();
            DataRow _rowFocusTong = gridViewDonHangTong.GetFocusedDataRow();
            string _malenhsx = _rowFocus["MaLenhSanXuat"].ToString();
            string _madh = _rowFocusTong["MaGop"].ToString();
            string url = string.Format("{0}?madh={1}&&malenhsanxuat={2}&&username={3}", URL + "CanDoiDonHangTong/DeleteNPL", _madh == null ? "" : _madh.ToString(),
                      _malenhsx == null ? "" : _malenhsx.ToString(), GlobleData.UserName);
            string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
            string url2 = string.Format("{0}?madh={1}&&malenhsanxuat={2}&&username={3}", URL + "CanDoiDonHangTong/DeleteAllCapThem", _madh == null ? "" : _madh.ToString(),
                _malenhsx == null ? "" : _malenhsx.ToString(), GlobleData.UserName);
            string result2 = Task.Run(async () => { return await _clientExtension.DeletedAsync(url2); }).Result;
            if (result.ToLower() == "true")
                LoadDSDonHangTong();
            else XtraMessageBox.Show(result);

        }
        private void MenuCanDoiDMNL(object sender, EventArgs e)
        {
            DataRow _rowFocus = gridView1.GetFocusedDataRow();
            frmCanDoiDinhMucNguyenLieu frm = new frmCanDoiDinhMucNguyenLieu(_rowFocus);
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
        }

        private void MainView_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }
        private void MainView_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
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

        private void MainView_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (info != null)
            {
                if (info.Column.FieldName == "PO" && view.IsGroupRow(e.RowHandle))
                {
                    e.Appearance.BackColor = Color.Yellow;
                }
            }
            e.Appearance.ForeColor = Color.Red;
        }

        private void btnNhapCapPhat_Click(object sender, EventArgs e)
        {
            //GridView view = gridView1;
            //if (view.FocusedRowHandle < 0)
            //{

            //    XtraMessageBox.Show("Không có lệnh được cấp phát. Vui lòng chọn lệnh!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);

            //    return;
            //}
            //object _objMaLenhSanXuat = null, _objMaGop = null, _objTenKH = null, _objMaHang = null, _objTenHang = null, _objMaDH = null, _objMaLenh = null, _objSoLuong = null, _objDot;
            //int _objIsKeoVe = 0;
            //GridView views = gridViewDonHangTong;
            //if (views.IsGroupRow(view.FocusedRowHandle))
            //{
            //    int childHandle = views.GetChildRowHandle(view.FocusedRowHandle, 0);
            //    _objTenKH = views.GetRowCellValue(childHandle, gridColumnKH);
            //    _objMaDH = views.GetRowCellValue(childHandle, colMaDH);
            //    _objMaHang = views.GetRowCellValue(childHandle, colMaHang);
            //    _objTenHang = views.GetRowCellValue(childHandle, colTenHang);
            //}
            //else
            //{
            //    _objTenKH = views.GetFocusedRowCellValue(gridColumnKH);
            //    _objMaDH = views.GetFocusedRowCellValue(colMaDH);
            //    _objMaHang = views.GetFocusedRowCellValue(colMaHang);
            //    _objTenHang = views.GetFocusedRowCellValue(colTenHang);
            //}
            //if (view.IsGroupRow(view.FocusedRowHandle))
            //{
            //    int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
            //    _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
            //    _objMaLenh = view.GetRowCellValue(childHandle, colMaLenh);
            //    _objMaGop = view.GetRowCellValue(childHandle, colNMaGop);
            //    _objDot = view.GetRowCellValue(childHandle, colNDot);
            //    _objSoLuong = view.GetRowCellValue(childHandle, colSoLuong);
            //    _objIsKeoVe = Convert.ToInt32(view.GetRowCellValue(childHandle, colIsKeoVe));

            //}
            //else
            //{
            //    _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
            //    _objMaLenh = view.GetFocusedRowCellValue(colMaLenh);
            //    _objMaGop = view.GetFocusedRowCellValue(colNMaGop);
            //    _objSoLuong = view.GetFocusedRowCellValue(colSoLuong);
            //    _objDot = view.GetFocusedRowCellValue(colNDot);
            //    _objIsKeoVe = Convert.ToInt32(view.GetFocusedRowCellValue(colIsKeoVe));
            //}

            //frmNhapCapPhatDHT frm = new frmNhapCapPhatDHT(_objMaGop == null ? "" : _objMaGop.ToString(), GlobleData.UserName, _objMaLenhSanXuat == null ? "" : _objMaLenhSanXuat.ToString(), _objTenKH == null ? "" : _objTenKH.ToString(), _objMaHang == null ? "" : _objMaHang.ToString(), _objMaDH == null ? "" : _objMaDH.ToString(), _objMaLenh == null ? "" : _objMaLenh.ToString(), _objSoLuong == null ? "" : _objSoLuong.ToString(), _objTenHang == null ? "" : _objTenHang.ToString(), _objIsKeoVe == null ? 0 : _objIsKeoVe, _objDot == null ? "" : _objDot.ToString());
            //frm.WindowState = FormWindowState.Maximized;
            //frm.ShowDialog();

            GridView view = gridView1;
            if (view.FocusedRowHandle < 0)
            {

                XtraMessageBox.Show("Không có lệnh được cấp phát. Vui lòng chọn lệnh!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            object _objMaLenhSanXuat = null, _objMaGop = null, _objTenKH = null, _objMaHang = null, _objTenHang = null, _objMaDH = null, _objMaLenh = null, _objSoLuong = null, _objDot = null, _objMaKH = null, _objMaDHGop = null;
            int _objIsKeoVe = 0;
            GridView views = gridViewDonHangTong;
            DataTable tblview = gridControl1.DataSource as DataTable;
            if (views.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = views.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objTenKH = views.GetRowCellValue(childHandle, gridColumnKH);
                _objMaDH = views.GetRowCellValue(childHandle, colMaDH);
                _objMaHang = views.GetRowCellValue(childHandle, colMaHang);
                _objTenHang = views.GetRowCellValue(childHandle, colTenHang);
            }
            else
            {
                _objTenKH = views.GetFocusedRowCellValue(gridColumnKH);
                _objMaDH = views.GetFocusedRowCellValue(colMaDH);
                _objMaHang = views.GetFocusedRowCellValue(colMaHang);
                _objTenHang = views.GetFocusedRowCellValue(colTenHang);
            }
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
                _objMaLenh = view.GetRowCellValue(childHandle, colMaLenh);
                _objMaGop = view.GetRowCellValue(childHandle, colNMaGop);
                _objDot = view.GetRowCellValue(childHandle, colNDot);
                _objSoLuong = view.GetRowCellValue(childHandle, colSoLuong);
                _objIsKeoVe = Convert.ToInt32(view.GetRowCellValue(childHandle, colIsKeoVe));
                _objMaKH = view.GetRowCellValue(childHandle, gridColumn16);
                _objMaDHGop = view.GetRowCellValue(childHandle, gridColumn2);

            }
            else
            {
                _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
                _objMaLenh = view.GetFocusedRowCellValue(colMaLenh);
                _objMaGop = view.GetFocusedRowCellValue(colNMaGop);
                _objSoLuong = view.GetFocusedRowCellValue(colSoLuong);
                _objDot = view.GetFocusedRowCellValue(colNDot);
                _objIsKeoVe = Convert.ToInt32(view.GetFocusedRowCellValue(colIsKeoVe));
                _objMaKH = view.GetFocusedRowCellValue(gridColumn16);
                _objMaDHGop = view.GetFocusedRowCellValue(gridColumn2);
            }

            frmCapPhatLSX frm = new frmCapPhatLSX(_objMaGop == null ? "" : _objMaGop.ToString(), GlobleData.UserName, _objMaLenhSanXuat == null ? "" : _objMaLenhSanXuat.ToString(), _objTenKH == null ? "" : _objTenKH.ToString(), _objMaHang == null ? "" : _objMaHang.ToString(), _objMaDH == null ? "" : _objMaDH.ToString(), _objMaLenh == null ? "" : _objMaLenh.ToString(), _objSoLuong == null ? "" : _objSoLuong.ToString(), _objTenHang == null ? "" : _objTenHang.ToString(), _objIsKeoVe == null ? 0 : _objIsKeoVe, _objDot == null ? "" : _objDot.ToString(), _objMaKH == null ? "" : _objMaKH.ToString(), _objMaDHGop == null ? "" : _objMaDHGop.ToString());
            frm.WindowState = FormWindowState.Maximized;
            frm.FormClosing += (s, args) =>
            {
                HandleLoadDataChiTiet();

            };

            frm.ShowDialog();

        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            BtImportCapThem();
        }

        private void InitChartV1()
        {
            DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel1 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
            sideBySideBarSeriesLabel1.Position = DevExpress.XtraCharts.BarSeriesLabelPosition.Center;

            // Code cũ
            //series1.CrosshairLabelVisibility = DefaultBoolean.True;
            //series1.CrosshairEnabled = DefaultBoolean.True;
            //series1.CrosshairLabelPattern = "{A}: {V}%";
            //((BarSeriesView)series1.View).BarWidth = 0.3; // Đặt kích thước ngang của cột
            //series1.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;

            // Tùy chỉnh biểu đồ
            XYDiagram diagram = (XYDiagram)chartControl2.Diagram;
            //diagram.AxisX.Label.TextPattern = "{A}";


            // Gán nhãn giá trị cho trục Y
            diagram.AxisY.CustomLabels.AddRange(new DevExpress.XtraCharts.CustomAxisLabel[] {
            new CustomAxisLabel("10%","10"),
            new CustomAxisLabel("20%","20"),
            new CustomAxisLabel("30%","30"),
            new CustomAxisLabel("40%","40"),
            new CustomAxisLabel("50%","50"),
            new CustomAxisLabel("60%","60"),
            new CustomAxisLabel("70%","70"),
            new CustomAxisLabel("80%","80"),
            new CustomAxisLabel("90%","90"),
            new CustomAxisLabel("100%","100"),
            });
            diagram.AxisY.LabelVisibilityMode = AxisLabelVisibilityMode.Default;
            diagram.AxisY.VisibleInPanesSerializable = "-1";
            //diagram.AxisY.WholeRange.Auto = false; // Ngăn trục Y tự động điều chỉnh phạm vi giá trị toàn bộ
            //diagram.AxisY.VisualRange.Auto = false; // Ngăn trục Y tự động điều chỉnh phạm vi giá trị hiển thị

            // Thiết lập phạm vi giá trị toàn bộ và hiển thị cho trục Y
            diagram.AxisY.WholeRange.SetMinMaxValues(0, 100);
            diagram.AxisY.WholeRange.AutoSideMargins = false;
            diagram.AxisY.VisualRange.SetMinMaxValues(0, 100);

            // Xoay biểu đồ ngang
            diagram.Rotated = true;
        }

        private void GetSearchLookUpDVSX()
        {
            string urlDVSX = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
            string jsonDVSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDVSX); }).Result;
            DataTable dtDVSX = JsonConvert.DeserializeObject<DataTable>(jsonDVSX);
            repositoryItemSearchLookUpDVSX.DataSource = dtDVSX;

        }

        private void btnChiaSX_ItemClick(object sender, ItemClickEventArgs e)
        {
            btchiasx();
        }

        private void btchiasx()
        {
            GridView view = gridViewDonHangTong;
            if (view == null) return;
            object _objMaDH = null, _objMaHang = null, _objTenHang = null, _objDot = null, _objMaLenh = null, _objPOID = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaDH = view.GetRowCellValue(childHandle, colMaDH);
                _objMaHang = view.GetRowCellValue(childHandle, colMaHang);
                _objTenHang = view.GetRowCellValue(childHandle, colTenHang);
                _objDot = view.GetRowCellValue(childHandle, colDot);
                _objPOID = view.GetRowCellValue(childHandle, colPOID);
            }
            else
            {
                _objMaDH = view.GetFocusedRowCellValue(colMaDH);
                _objMaHang = view.GetFocusedRowCellValue(colMaHang);
                _objTenHang = view.GetFocusedRowCellValue(colTenHang);
                _objDot = view.GetFocusedRowCellValue(colDot);
                _objPOID = view.GetFocusedRowCellValue(colPOID);
            }
            frmCanDoiDonHangChiTiet frm = new frmCanDoiDonHangChiTiet(false, _objMaDH == null ? "" : _objMaDH.ToString(), _objMaHang == null ? "" : _objMaHang.ToString(),
                _objTenHang == null ? "" : _objTenHang.ToString(), _objDot == null ? "" : _objDot.ToString(), _objPOID == null ? "" : _objPOID.ToString(), "", "", "", "", "", "", "", 0);
            frm.WindowState = FormWindowState.Maximized;
            //frm.ShowDialog();
            //LoadDSDonHangTong();

            frm.Show();
            //LoadDSDonHangTong();
            frm.FormClosed += (s, e) =>
            {
                LoadDSDonHangTong();
            };
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmGhepDonHangBo frm = new frmGhepDonHangBo();
            frm.ShowDialog();
        }

        private void gridView1_CustomDrawFooter_1(object sender, RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gridView1_CustomDrawFooterCell_1(object sender, FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;
            e.Appearance.ForeColor = Color.Red;
        }

        private void gridViewDinhMucNguyenPhuLieu_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == gridColumn19)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }

        }



        private void btnSuaLenh_Click(object sender, EventArgs e)
        {
            btnSuaLenh();
        }

        private void btnSuaLenh()
        {
            GridView view = gridView1;
            if (view.FocusedRowHandle < 0) return;
            object _objMaDH = null, _objMaHang = null, _objTenHang = null, _objDot = null, _objMaLenh = null, _objPOID = null,
                 _objMaDVSX = null, _objMaLenhSanXuat = null, _objNguoiTao = null, _objMaGop = null, _objTenLenh = null, _objSTTLenh = null, _objMaDHGop = null, _objGhiChu = null;
            int _objIsKeoVe = 0;
            GridView views = gridView1;//gridViewDonHangTong;
            if (views.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = views.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaDH = views.GetRowCellValue(childHandle, colMaDH);
                _objMaHang = views.GetRowCellValue(childHandle, colMaHang);
                _objTenHang = views.GetRowCellValue(childHandle, colTenHang);
                _objGhiChu = views.GetRowCellValue(childHandle, gridColumn57);

            }
            else
            {
                _objMaDH = views.GetFocusedRowCellValue(colMaDH);
                _objMaHang = views.GetFocusedRowCellValue(colMaHang);
                _objTenHang = views.GetFocusedRowCellValue(colTenHang);
                _objGhiChu = views.GetFocusedRowCellValue(gridColumn57);
            }
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                //_objMaDH = view.GetRowCellValue(childHandle, colMaDH);
                //_objMaHang = view.GetRowCellValue(childHandle, colMaHang);
                //_objTenHang = view.GetRowCellValue(childHandle, colTenHang);
                _objDot = view.GetRowCellValue(childHandle, colNDot);
                _objPOID = view.GetRowCellValue(childHandle, colPOID);
                _objMaDVSX = view.GetRowCellValue(childHandle, colMaDVSX);
                _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
                _objMaLenh = view.GetRowCellValue(childHandle, colMaLenh);
                _objTenLenh = view.GetRowCellValue(childHandle, colTenLenh);
                _objNguoiTao = view.GetRowCellValue(childHandle, colEmployeeID);
                _objSTTLenh = view.GetRowCellValue(childHandle, colSTTLenh);
                _objMaGop = view.GetRowCellValue(childHandle, colNMaGop);
                _objIsKeoVe = Convert.ToInt32(view.GetRowCellValue(childHandle, colIsKeoVe));//P:gán giá trị cho _iskeove 
                _objMaDHGop = view.GetRowCellValue(childHandle, gridColumn2);
                _objGhiChu = views.GetRowCellValue(childHandle, gridColumn57);

            }
            else
            {
                //_objMaDH = view.GetFocusedRowCellValue(colMaDH);
                //_objMaHang = view.GetFocusedRowCellValue(colMaHang);
                //_objTenHang = view.GetFocusedRowCellValue(colTenHang);
                _objDot = view.GetFocusedRowCellValue(colNDot);
                _objPOID = view.GetFocusedRowCellValue(colPOID);
                _objMaDVSX = view.GetFocusedRowCellValue(colMaDVSX);
                _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
                _objMaLenh = view.GetFocusedRowCellValue(colMaLenh);
                _objTenLenh = view.GetFocusedRowCellValue(colTenLenh);
                _objNguoiTao = view.GetFocusedRowCellValue(colEmployeeID);
                _objSTTLenh = view.GetFocusedRowCellValue(colSTTLenh);
                _objMaGop = view.GetFocusedRowCellValue(colNMaGop);
                _objIsKeoVe = Convert.ToInt32(view.GetFocusedRowCellValue(colIsKeoVe));//P:gán giá trị cho _iskeove 
                _objMaDHGop = view.GetFocusedRowCellValue(gridColumn2);
                _objGhiChu = views.GetFocusedRowCellValue(gridColumn57);
            }
            frmCanDoiDinhMucNPLChiTiet frm = new frmCanDoiDinhMucNPLChiTiet(_objMaDH == null ? "" : _objMaDH.ToString(), _objMaHang == null ? "" : _objMaHang.ToString(), _objTenHang == null ? "" : _objTenHang.ToString(),
                _objMaLenh == null ? "" : _objMaLenh.ToString(), _objDot == null ? "" : _objDot.ToString(), _objMaDVSX == null ? "" : _objMaDVSX.ToString(),
                _objMaLenhSanXuat == null ? "" : _objMaLenhSanXuat.ToString(), _objPOID == null ? "" : _objPOID.ToString(), _objIsKeoVe == null ? 0 : _objIsKeoVe,
                _objNguoiTao == null ? "" : _objNguoiTao.ToString(), _objMaGop == null ? "" : _objMaGop.ToString(), _objTenLenh == null ? "" : _objTenLenh.ToString(), _objSTTLenh == null ? "" : _objSTTLenh.ToString(),
                 _objMaDHGop == null ? "" : _objMaDHGop.ToString(), _objGhiChu == null ? "" : _objGhiChu.ToString());
            frm.WindowState = FormWindowState.Maximized;
            //frm.ShowDialog();
            //LoadDSDonHangTong();
            frm.Show();
            frm.FormClosed += (s, e) =>
            {
                LoadDSDonHangTong();
            };
        }

        private void btnXoaLenh_Click(object sender, EventArgs e)
        {

            GridView view = gridView1;
            if (view == null || view.FocusedRowHandle < 0) return;
            object objMaLenh = null;
            object objMaLenhSX = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int chilHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                objMaLenh = view.GetRowCellValue(chilHandle, colMaLenh);
            }
            else
                objMaLenh = view.GetFocusedRowCellValue(colMaLenh);
            objMaLenhSX = view.GetFocusedRowCellValue(colMaLenhSanXuat);
            if (objMaLenh == null || string.IsNullOrEmpty(objMaLenh.ToString()))
            {
                return;
            }
            string maLenh = objMaLenh.ToString();
            string maLenhSX = objMaLenhSX.ToString();
            bool isDuyetLenh = false;
            string url = string.Format("{0}?para={1}", URL + "DonHangTong/CheckDuyetLenh", maLenh);
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
            if (!string.IsNullOrEmpty(json))
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    int isDuyet = 0;
                    int.TryParse(tbl.Rows[0]["Status"]?.ToString(), out isDuyet);
                    if (isDuyet == 1)
                    {
                        isDuyetLenh = true;
                    }
                    else
                    {
                        isDuyetLenh = false;
                    }
                }
            }
            bool hasDataInSDKH = false;
            string urlCheckTNC = $"{URL}KhoiTaoBOMV1/Get?action=CheckBOMTNCDELCP&para4=&para5={maLenhSX}";
            string jsonCheckTNC = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCheckTNC); }).Result;
            if (!string.IsNullOrEmpty(jsonCheckTNC))
            {
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonCheckTNC);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    int hasDataTNC = 0;
                    int.TryParse(tbl.Rows[0]["HasDataTNC"]?.ToString(), out hasDataTNC);
                    if (hasDataTNC == 1)
                    {
                        hasDataInSDKH = true;
                    }
                    else
                    {
                        hasDataInSDKH = false;
                    }
                }
            }
            if (hasDataInSDKH)
            {
                XtraMessageBox.Show($"Lệnh SX {maLenh} đã có TNC. Không thể xóa lệnh!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (isDuyetLenh)
            {
                XtraMessageBox.Show($"Lệnh SX {maLenh} đã được duyệt. Không thể xóa lệnh!\n Nếu bạn muốn xóa vui lòng hủy duyệt trước khi xóa lệnh!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (GlobleData.UserName.ToString().ToUpper() == _nguoitao.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
                Delete();
            else
            {
                XtraMessageBox.Show("User này không phải là người tạo, không có quyền xóa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void Delete()
        {
            try
            {
                GridView view = (GridView)gridView1;
                if (view.FocusedRowHandle < 0) return;
                int _objIsKeoVe = 0;
                if (view.IsGroupRow(view.FocusedRowHandle))
                {
                    int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                    _objIsKeoVe = Convert.ToInt32(view.GetRowCellValue(childHandle, colIsKeoVe));
                }
                else
                {
                    _objIsKeoVe = Convert.ToInt32(view.GetFocusedRowCellValue(colIsKeoVe));

                }
                //if (_objIsKeoVe == 1)
                //{
                //    XtraMessageBox.Show("Lệnh đã được xét duyệt. Vui lòng không được xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                object _objMaLenh = null, _objMaLenhSanXuat = null, _objMaGop = null;
                if (view.IsGroupRow(view.FocusedRowHandle))
                {
                    int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                    _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
                    _objMaLenh = view.GetRowCellValue(childHandle, colMaLenh);
                    _objMaGop = view.GetRowCellValue(childHandle, colNMaGop);
                }
                else
                {

                    _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
                    _objMaLenh = view.GetFocusedRowCellValue(colMaLenh);
                    _objMaGop = view.GetFocusedRowCellValue(colNMaGop);
                }

                string urlLc = string.Format("{0}", URL + "CanDoiDonHangTong/GetIsLenChuyen");
                string jsonLc = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlLc); }).Result;
                DataTable tblLC = JsonConvert.DeserializeObject<DataTable>(jsonLc);
                bool IsLenChuyen = false;
                if (tblLC != null && tblLC.Rows.Count > 0)
                {
                    DataRow[] foundRows = tblLC.Select($"PoId = '{_objMaLenh}'");
                    if (foundRows.Length > 0)
                    {
                        IsLenChuyen = true;
                    }
                }
                if (IsLenChuyen == true)
                {
                    XtraMessageBox.Show("Lệnh này đã lên chuyền sản xuất. Vui lòng không được xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult messResult = MessageBox.Show("Bạn có chắc chắn muốn xóa lệnh này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    //string url = string.Format("{0}?madh={1}&&malenh={2}&&malenhsanxuat={3}&&username={4}", URL + "CanDoiDonHangTong/Delete", _objMaGop == null ? "" : _objMaGop.ToString(),
                    //    _objMaLenh == null ? "" : _objMaLenh.ToString(), _objMaLenhSanXuat == null ? "" : _objMaLenhSanXuat.ToString(), GlobleData.UserName);
                    string url = string.Format("{0}?madh={1}&&malenh={2}&&malenhsanxuat={3}&&username={4}", URL + "CanDoiDonHangTong/DeleteLenhSX", _objMaGop == null ? "" : _objMaGop.ToString(),
                    _objMaLenh == null ? "" : _objMaLenh.ToString(), _objMaLenhSanXuat == null ? "" : _objMaLenhSanXuat.ToString(), GlobleData.UserName);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadDSDonHangTong();
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa dòng đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void gridView1_CustomDrawFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gridView1_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;
            e.Appearance.ForeColor = Color.Red;
        }

        private void btnCapPhat_Click(object sender, EventArgs e)
        {
            BtImportCapPhat();
        }



        private void BtImportCapPhat()
        {
            _ChkTH = false;
            _ChkCT = false;
            addEventClick(_ChkCT, _ChkTH);
        }



        private void addEventClick(bool _ChkCT, bool _ChkTH)
        {

            GridView view = (GridView)gridView1;
            GridView view2 = (GridView)gridViewDonHangTong;
            if (view == null) return;
            object _objMaGop = null, _objMaLenhSanXuat = null, _objMaKH = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaGop = view.GetRowCellValue(childHandle, colNMaGop);
                _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
                _objMaKH = view2.GetRowCellValue(childHandle, gridColumnKH);
            }
            else
            {
                _objMaGop = view.GetFocusedRowCellValue(colNMaGop);
                _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
                _objMaKH = view2.GetFocusedRowCellValue(gridColumnKH);
            }


            if (_objMaGop == null || _objMaLenhSanXuat == null)
            {

                MessageBox.Show("Vui lòng chọn đơn hàng và lệnh sản xuất.! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_ChkCT || !_ChkTH)
            {
                //string url = string.Format("{0}/?madh={1}&&malenhsanxuat={2}", URL + "CanDoiDonHangTong/GetKiemTraDinhMuc", _objMaGop.ToString(), _objMaLenhSanXuat.ToString());
                string url = string.Format("{0}?madh={1}&&malenhsx={2}", URL + "CanDoiDonHangTong/GetVatTuNPL", _objMaGop.ToString(), _objMaLenhSanXuat.ToString());
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                if (tbl == null || tbl.Rows.Count == 0)
                {
                    frmErpImport_DMNL frm = new frmErpImport_DMNL(_objMaGop.ToString(), _objMaLenhSanXuat.ToString(), _ChkCT, _ChkTH, _objMaKH == null ? "" : _objMaKH.ToString());
                    frm.ShowDialog();
                }
                else
                {
                    if (_ChkCT || _ChkTH)
                    {
                        frmErpImport_DMNL frm = new frmErpImport_DMNL(_objMaGop.ToString(), _objMaLenhSanXuat.ToString(), _ChkCT, _ChkTH, _objMaKH == null ? "" : _objMaKH.ToString());
                        frm.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Lệnh sản xuât của đơn hàng này đã có cấp phát, hãy xóa đi trước khi muốn thực hiện cấp phát! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Lệnh sản xuất của đơn hàng đã được sản xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        private void btnCapThem_Click(object sender, EventArgs e)
        {
            BtImportCapThem();
        }







        //private void BtImportCapThem()
        //{
        //    GridView view = (GridView)gridView1;
        //    if (view == null) return;
        //    object _objMaGop = null, _objMaLenhSanXuat = null;
        //    if (view.IsGroupRow(view.FocusedRowHandle))
        //    {
        //        int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
        //        _objMaGop = view.GetRowCellValue(childHandle, colNMaGop);
        //        _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
        //    }
        //    else
        //    {
        //        _objMaGop = view.GetFocusedRowCellValue(colNMaGop);
        //        _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
        //    }

        //    if (_objMaGop == null || _objMaLenhSanXuat == null)
        //    {
        //        MessageBox.Show("Vui lòng chọn đơn hàng và lệnh sản xuất.! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }
        //    _ChkCT = true;
        //    _ChkTH = false;
        //    string url = string.Format("{0}/?madh={1}&&malenhsanxuat={2}", URL + "CanDoiDonHangTong/GetKiemTraDinhMuc", _objMaGop.ToString(), _objMaLenhSanXuat.ToString());
        //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
        //    List<DinhMucEntity> listDMNPL = JsonConvert.DeserializeObject<List<DinhMucEntity>>(json);
        //    if (listDMNPL == null || listDMNPL.Count == 0)
        //    {
        //        MessageBox.Show("Chưa có cấp phát", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }
        //    else
        //    {
        //        addEventClick(_ChkCT, _ChkTH);
        //    }
        //}

        private void btnThuHoi_Click(object sender, EventArgs e)
        {
            BtImportThuHoi();
        }
        private void BtImportThuHoi()
        {
            GridView view = (GridView)gridView1;
            if (view == null) return;
            object _objMaGop = null, _objMaLenhSanXuat = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaGop = view.GetRowCellValue(childHandle, colNMaGop);
                _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
            }
            else
            {
                _objMaGop = view.GetFocusedRowCellValue(colNMaGop);
                _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
            }

            if (_objMaGop == null || _objMaLenhSanXuat == null)
            {

                MessageBox.Show("Vui lòng chọn đơn hàng và lệnh sản xuất.! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _ChkCT = false;
            _ChkTH = true;
            string url = string.Format("{0}/?madh={1}&&malenhsanxuat={2}", URL + "CanDoiDonHangTong/GetKiemTraDinhMuc", _objMaGop.ToString(), _objMaLenhSanXuat.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<DinhMucEntity> listDMNPL = JsonConvert.DeserializeObject<List<DinhMucEntity>>(json);
            if (listDMNPL == null || listDMNPL.Count == 0)
            {
                MessageBox.Show("Chưa có cấp phát", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                addEventClick(_ChkCT, _ChkTH);
            }
        }



        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DataRow _rowFocus = gridView1.GetFocusedDataRow();
            //DataTable _dtData = gridControl1.DataSource as DataTable;
            //frmCanDoiDinhMucNPL frm = new frmCanDoiDinhMucNPL(_rowFocus, _dtData);
            //frm.ShowDialog();
            //LoadDSDonHangTong();
            frmCanDoiDinhMucNguyenLieu frm = new frmCanDoiDinhMucNguyenLieu(_rowFocus);
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
            LoadDSDonHangTong();
        }
        private void btnXNXoa_Click(object sender, EventArgs e)
        {
            try
            {
                GridView views = gridViewDonHangTong;
                if (views.FocusedRowHandle < 0) return;
                object _objMaGop = null;
                int _objIsKeoVe = 0;

                if (views.IsGroupRow(views.FocusedRowHandle))
                {
                    int childHandle = views.GetChildRowHandle(views.FocusedRowHandle, 0);
                    _objMaGop = views.GetRowCellValue(childHandle, colMaGop);

                }
                else
                {
                    _objMaGop = views.GetFocusedRowCellValue(colMaGop);
                }
                DialogResult result = MessageBox.Show("Bạn có muốn xóa định mức đơn hàng " + _objMaGop + " này không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string url = string.Format("{0}/?madonhang={1}", URL + "DonHangTong/DeleteNPL", _objMaGop == null ? "" : _objMaGop.ToString());
                    string json = Task.Run(async () => await _clientExtension.DeletedAsync(url)).Result;
                    LoadDSDonHangTong();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa đã xảy ra lỗi. Vui lòng thử lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void simpleButton3_Click(object sender, EventArgs e)
        {
            BtImportCapPhat();
        }

        #region quan
        private void BtImportCapThem()
        {
            GridView view = (GridView)gridView1;
            if (view == null) return;
            object _objMaGop = null, _objMaLenhSanXuat = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaGop = view.GetRowCellValue(childHandle, colNMaGop);
                _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
            }
            else
            {
                _objMaGop = view.GetFocusedRowCellValue(colNMaGop);
                _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
            }

            if (_objMaGop == null || _objMaLenhSanXuat == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng và lệnh sản xuất.! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _ChkCT = true;
            _ChkTH = false;
            string url = string.Format("{0}/?madh={1}&&malenhsanxuat={2}", URL + "CanDoiDonHangTong/GetKiemTraDinhMuc", _objMaGop.ToString(), _objMaLenhSanXuat.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            List<DinhMucEntity> listDMNPL = JsonConvert.DeserializeObject<List<DinhMucEntity>>(json);
            if (listDMNPL == null || listDMNPL.Count == 0)
            {
                MessageBox.Show("Chưa có cấp phát", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                addEventClick2(_ChkCT, _ChkTH);
            }
        }
        private void addEventClick2(bool _ChkCT, bool _ChkTH)
        {

            GridView view = (GridView)gridView1;
            GridView view2 = (GridView)gridViewDonHangTong;
            if (view == null) return;
            object _objMaGop = null, _objMaLenhSanXuat = null, _objMaKH = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaGop = view.GetRowCellValue(childHandle, colNMaGop);
                _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
                _objMaKH = view2.GetRowCellValue(childHandle, gridColumnKH);
            }
            else
            {
                _objMaGop = view.GetFocusedRowCellValue(colNMaGop);
                _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
                _objMaKH = view2.GetFocusedRowCellValue(gridColumnKH);
            }


            if (_objMaGop == null || _objMaLenhSanXuat == null)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng và lệnh sản xuất.! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_ChkCT || !_ChkTH)
            {
                //string url = string.Format("{0}/?madh={1}&&malenhsanxuat={2}", URL + "CanDoiDonHangTong/GetKiemTraDinhMuc", _objMaGop.ToString(), _objMaLenhSanXuat.ToString());
                string url = string.Format("{0}?madh={1}&&malenhsx={2}", URL + "CanDoiDonHangTong/GetVatTuNPL", _objMaGop.ToString(), _objMaLenhSanXuat.ToString());
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                if (tbl != null || tbl.Rows.Count > 0)
                {
                    frmErpImport_DMNL frm = new frmErpImport_DMNL(_objMaGop.ToString(), _objMaLenhSanXuat.ToString(), _ChkCT, _ChkTH, _objMaKH == null ? "" : _objMaKH.ToString());
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Lệnh sản xuất của đơn hàng đã được sản xuất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        #endregion



        private void simpleButton4_Click_1(object sender, EventArgs e)
        {
            BtImportCapThem();
        }


        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            BarButtonItemLink link = (BarButtonItemLink)e.Link;
            if (link != null)
            {
                BarManager barManager = link.Item.Manager;
                Point barButtonLocation = link.Bounds.Location;
                Point screenLocation = barManager.Form.PointToScreen(barButtonLocation);

                frmSearchOrderProduction frm = new frmSearchOrderProduction(screenLocation.X, screenLocation.Y + link.Bounds.Height);
                frm.ShowDialog();
                string resultDH = frm._resultDonHang;
                if (resultDH == "Clean")
                {
                    LoadDSDonHangTong();
                    return;
                }
                if (string.IsNullOrEmpty(resultDH)) return;
                xtraTabControl1.SelectedTabPageIndex = 1;
                string url = string.Format("{0}?maDH={1}", URL + "CanDoiDonHangTong/GetDonHangTong_MaDH", resultDH);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                gridDonHangTong.DataSource = tbl;
                gridDonHangTong.RefreshDataSource();
                gridViewDonHangTong.FocusedRowHandle = _rowhandle;
                LoadLenhSX(this.gridViewDonHangTong);
                GetDataChartV1();

            }
        }
        #region
        private DataTable CreateCanDoiDinhMucNPL_LogTable()
        {
            DataTable dt = new DataTable("TYPE_CanDoiDinhMucNPL_Log");

            dt.Columns.Add("ID_CanDoi", typeof(long));
            dt.Columns.Add("MaDH", typeof(string));
            dt.Columns.Add("MaLenhSanXuat", typeof(string));
            dt.Columns.Add("MaNPL", typeof(string));
            dt.Columns.Add("DinhMuc_Old", typeof(double));
            dt.Columns.Add("DinhMuc_New", typeof(double));
            dt.Columns.Add("SoLuong_Old", typeof(int));
            dt.Columns.Add("SoLuong_New", typeof(int));
            dt.Columns.Add("CapPhat_Old", typeof(double));
            dt.Columns.Add("CapPhat_New", typeof(double));
            dt.Columns.Add("DinhMucHaoHut_Old", typeof(double));
            dt.Columns.Add("DinhMucHaoHut_New", typeof(double));
            dt.Columns.Add("CapThem_Old", typeof(double));
            dt.Columns.Add("CapThem_New", typeof(double));
            dt.Columns.Add("ThuHoi_Old", typeof(double));
            dt.Columns.Add("ThuHoi_New", typeof(double));
            dt.Columns.Add("UserID", typeof(string));
            dt.Columns.Add("Time", typeof(DateTime));
            dt.Columns.Add("IP", typeof(string));

            return dt;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Kiểm tra tổ hợp phím Ctrl + S
            if (keyData == (Keys.Control | Keys.S))
            {

                if (gridDinhMucNguyenPhuLieu.ContainsFocus)
                {

                    LuuCapPhat();


                    return true;
                }
            }

            // Gọi phương thức cơ sở để xử lý các phím khác bình thường
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void gridViewDinhMucNguyenPhuLieu_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {

            GridView view = sender as GridView;
            if (view == null) return;
            string tenCotThayDoi = view.FocusedColumn.FieldName;
            if (tenCotThayDoi != "CapPhat" && tenCotThayDoi != "SoLuong" && tenCotThayDoi != "DinhMuc" && tenCotThayDoi != "DinhMucHaoHut") return;
            if (e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
            {
                e.Valid = true;
                return;
            }

            bool isValid = false;
            if (tenCotThayDoi == "SoLuong")
            {
                isValid = int.TryParse(e.Value.ToString(), out _);
            }
            else
            {
                isValid = double.TryParse(e.Value.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out _);
            }

            if (!isValid)
            {
                e.Valid = false;
                string _eText = "Giá trị nhập không đúng định dạng số thập phân";
                if (tenCotThayDoi == "SoLuong")
                    _eText = "Giá trị nhập không đúng định dạng số nguyên";
                e.ErrorText = _eText;
                return;
            }
            object oldValue = view.GetRowCellValue(view.FocusedRowHandle, view.FocusedColumn);
            object newValue = e.Value;
            if (object.Equals(newValue, oldValue))
            {
                return;
            }
            DataRow currentRow = view.GetDataRow(view.FocusedRowHandle);
            if (currentRow == null) return;
            object idCanDoi = currentRow["ID"];

            // Tìm kiếm xem dòng này đã có trong bảng log hay chưa
            DataRow[] logRows = tblCanDoiLog.Select($"ID_CanDoi = {idCanDoi}");
            DataRow logRow;

            if (logRows.Length == 0)
            {
                logRow = tblCanDoiLog.NewRow();
                logRow["ID_CanDoi"] = idCanDoi;
                logRow["MaDH"] = currentRow["MaDH"].ToString().Trim();
                logRow["MaLenhSanXuat"] = currentRow["MaLenhSanXuat"].ToString().Trim();
                logRow["MaNPL"] = currentRow["MaNPL"].ToString().Trim();
                logRow["DinhMuc_Old"] = currentRow["DinhMuc"];
                logRow["DinhMuc_New"] = currentRow["DinhMuc"];
                logRow["SoLuong_Old"] = currentRow["SoLuong"];
                logRow["SoLuong_New"] = currentRow["SoLuong"];
                logRow["CapPhat_Old"] = currentRow["CapPhat"];
                logRow["CapPhat_New"] = currentRow["CapPhat"];
                logRow["ThuHoi_Old"] = currentRow["ThuHoi"];
                logRow["ThuHoi_New"] = currentRow["ThuHoi"];
                logRow["DinhMucHaoHut_Old"] = currentRow["DinhMucHaohut"];
                logRow["DinhMucHaoHut_New"] = currentRow["DinhMucHaohut"];
                logRow["CapThem_Old"] = currentRow["CapThem"];
                logRow["CapThem_New"] = currentRow["CapThem"];
                logRow["UserID"] = GlobleData.UserName;
                logRow["Time"] = DateTime.Now;
                logRow["IP"] = GetLocalIPAddress();
            }
            else
            {
                logRow = logRows[0];

            }

            switch (tenCotThayDoi)
            {
                case "DinhMuc":

                    logRow["DinhMuc_Old"] = oldValue;
                    logRow["DinhMuc_New"] = newValue;

                    break;
                case "CapPhat":

                    logRow["CapPhat_Old"] = oldValue;


                    logRow["CapPhat_New"] = newValue;


                    break;
                case "SoLuong":

                    logRow["SoLuong_Old"] = oldValue;

                    logRow["SoLuong_New"] = newValue;

                    break;
                case "DinhMucHaoHut":
                    logRow["DinhMucHaoHut_Old"] = oldValue;
                    logRow["DinhMucHaoHut_New"] = newValue;

                    break;
            }
            if (logRow.RowState == DataRowState.Detached)
            {
                tblCanDoiLog.Rows.Add(logRow);
            }

        }


        private void LuuCapPhat()
        {
            if (tblCanDoiLog.Rows.Count == 0) return;



            string url = $"{URL}CanDoiDinhMucNPLLog/Post?Action=POSTCANDOINPL";
            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblCanDoiLog); }).Result;
            if (msResult.ToLower() == "true")
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                loadDataToolTip();
                loadDataCanDoiLogStyle();
            }
        }
        private string GetLocalIPAddress()
        {
            string localIP = "Không xác định";
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        private void loadDataToolTip()
        {
            DataRow dr = gridViewDonHangTong.GetFocusedDataRow();
            if (dr == null) return;
            string url = $"{URL}CanDoiDinhMucNPLLog/Get?Action=GETCANDOILOG&para={dr["MaDH"].ToString().Trim()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            if (json == "[]") return;
            tblToolTip.Clear();
            tblToolTip = JsonConvert.DeserializeObject<DataTable>(json);
            // Thực hiện một lần sau khi đổ dữ liệu vào tblToolTip
            if (tblToolTip != null && tblToolTip.PrimaryKey.Length == 0)
            {
                tblToolTip.PrimaryKey = new DataColumn[] { tblToolTip.Columns["ID_CanDoi"] };
            }
            gridViewDinhMucNguyenPhuLieu.RefreshData();

        }
        private void loadDataCanDoiLogStyle()
        {
            DataRow dr = gridViewDonHangTong.GetFocusedDataRow();
            if (dr == null) return;
            string url = $"{URL}CanDoiDinhMucNPLLog/Get?Action=GETCANDOILOGSTYLE&para={dr["MaDH"].ToString().Trim()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;

            if (json == "[]") return;
            tblCanDoiLogStyle.Clear();
            tblCanDoiLogStyle = JsonConvert.DeserializeObject<DataTable>(json);
            // Thực hiện một lần sau khi đổ dữ liệu vào tblToolTip
            if (tblCanDoiLogStyle != null && tblCanDoiLogStyle.PrimaryKey.Length == 0)
            {
                tblCanDoiLogStyle.PrimaryKey = new DataColumn[] { tblCanDoiLogStyle.Columns["ID_CanDoi"] };
            }
            gridViewDinhMucNguyenPhuLieu.RefreshData();

        }
        private void toolTipControllerCapPhat_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {

            if (e.SelectedControl != gridDinhMucNguyenPhuLieu) return;

            var hitInfo = gridViewDinhMucNguyenPhuLieu.CalcHitInfo(e.ControlMousePosition);

            if (hitInfo.InRowCell)
            {
                string currentColumnFieldName = hitInfo.Column.FieldName;
                if (currentColumnFieldName == "DinhMuc" || currentColumnFieldName == "DinhMucHaoHut" || currentColumnFieldName == "CapPhat" || currentColumnFieldName == "SoLuong")
                {
                    object focusedRowKeyValue = gridViewDinhMucNguyenPhuLieu.GetRowCellValue(hitInfo.RowHandle, "ID");
                    if (tblToolTip == null || tblToolTip.Rows.Count == 0) return;
                    if (focusedRowKeyValue != null)
                    {
                        // Sử dụng Rows.Find() để truy xuất siêu nhanh
                        DataRow toolTipRow = tblToolTip.Rows.Find(focusedRowKeyValue);

                        if (toolTipRow != null)
                        {
                            string old_Field = currentColumnFieldName + "_Old";
                            // Kiểm tra xem cột có tồn tại không trước khi truy cập
                            if (tblToolTip.Columns.Contains(old_Field))
                            {
                                string toolTipText = toolTipRow[old_Field].ToString();

                                var info = new DevExpress.Utils.ToolTipControlInfo(hitInfo.RowHandle, toolTipText);
                                info.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
                                e.Info = info;
                            }
                        }
                    }
                }
            }
        }
        private void gridViewDinhMucNguyenPhuLieu_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.Column.FieldName != "DinhMuc" && e.Column.FieldName != "CapPhat" && e.Column.FieldName != "SoLuong" && e.Column.FieldName != "DinhMucHaoHut")
                {
                    return;
                }

                GridView view = sender as GridView;
                if (view == null) return;

                object currentIdObj = view.GetRowCellValue(e.RowHandle, "ID");
                if (currentIdObj == null || currentIdObj == DBNull.Value)
                {
                    return;
                }
                int currentId = Convert.ToInt32(currentIdObj);
                object currentDinhMucObj = e.CellValue;
                if (currentDinhMucObj == null || currentDinhMucObj == DBNull.Value)
                {
                    return;
                }
                decimal currentDinhMuc = Convert.ToDecimal(currentDinhMucObj);

                //if (tblCanDoiLogStyle == null || tblCanDoiLogStyle.Rows.Count == 0) return;
                //DataRow[] foundRows = tblCanDoiLogStyle.Select($"ID_CanDoi = {currentId}");
                //if (foundRows.Length > 0)
                //{
                //    DataRow toolTipRow = foundRows[0];
                //    string field_Old = e.Column.FieldName + "_Old";
                //    object toolTipDinhMucObj = toolTipRow[field_Old];

                //    if (toolTipDinhMucObj != null && toolTipDinhMucObj != DBNull.Value)
                //    {
                //        decimal toolTipDinhMuc = Convert.ToDecimal(toolTipDinhMucObj);

                //        // 6. So sánh DinhMuc và tô màu nếu khác nhau
                //        if (currentDinhMuc != toolTipDinhMuc)
                //        {
                //            e.Appearance.BackColor = Color.Yellow;
                //            e.Appearance.ForeColor = Color.Red;
                //        }
                //    }
                //}
                bool hasChanged = false; // Cờ để xác định có sự thay đổi hay không

                // --- BƯỚC 1: So sánh với tblCanDoiLogStyle ---
                if (tblCanDoiLogStyle != null && tblCanDoiLogStyle.Rows.Count > 0)
                {
                    // Tìm dòng log tương ứng
                    DataRow[] foundRows = tblCanDoiLogStyle.Select($"ID_CanDoi = {currentId}");

                    if (foundRows.Length > 0)
                    {
                        DataRow logRow = foundRows[0];
                        string oldField = e.Column.FieldName + "_Old"; // Ví dụ: "DinhMuc_Old"

                        // Kiểm tra xem cột giá trị cũ có tồn tại trong bảng log không
                        if (logRow.Table.Columns.Contains(oldField))
                        {
                            object oldValueObj = logRow[oldField];

                            // So sánh giá trị
                            if (oldValueObj != DBNull.Value)
                            {
                                // Chuyển đổi cả hai giá trị về cùng một kiểu để so sánh chính xác
                                // Thay decimal bằng kiểu dữ liệu phù hợp của bạn (string, int, etc.)
                                try
                                {
                                    decimal oldValue = Convert.ToDecimal(oldValueObj);
                                    decimal currentValue = Convert.ToDecimal(e.CellValue);

                                    if (currentValue != oldValue)
                                    {
                                        hasChanged = true; // Đánh dấu là đã thay đổi
                                    }
                                }
                                catch
                                {
                                    // Xử lý lỗi nếu không thể chuyển đổi kiểu dữ liệu
                                    // Có thể so sánh như chuỗi nếu cần
                                    if (!e.CellValue.ToString().Equals(oldValueObj.ToString()))
                                    {
                                        hasChanged = true;
                                    }
                                }
                            }
                        }
                    }
                }

                // --- BƯỚC 2: Nếu chưa thay đổi, tiếp tục so sánh với tblToolTip ---
                if (!hasChanged && tblToolTip != null && tblToolTip.Rows.Count > 0)
                {
                    // Tìm dòng tooltip tương ứng (giả sử cũng dùng ID_CanDoi)
                    DataRow[] foundRows = tblToolTip.Select($"ID_CanDoi = {currentId}");

                    if (foundRows.Length > 0)
                    {
                        DataRow toolTipRow = foundRows[0];
                        string oldField = e.Column.FieldName + "_Old"; // Giả sử tblToolTip cũng có cấu trúc tương tự

                        // Kiểm tra xem cột giá trị cũ có tồn tại trong bảng tooltip không
                        if (toolTipRow.Table.Columns.Contains(oldField))
                        {
                            object oldValueObj = toolTipRow[oldField];

                            if (oldValueObj != DBNull.Value)
                            {
                                try
                                {
                                    decimal oldValue = Convert.ToDecimal(oldValueObj);
                                    decimal currentValue = Convert.ToDecimal(e.CellValue);

                                    if (currentValue != oldValue)
                                    {
                                        hasChanged = true; // Đánh dấu là đã thay đổi
                                    }
                                }
                                catch
                                {
                                    if (!e.CellValue.ToString().Equals(oldValueObj.ToString()))
                                    {
                                        hasChanged = true;
                                    }
                                }
                            }
                        }
                    }
                }


                // --- BƯỚC 3: Dựa vào kết quả cuối cùng để tô màu ---
                if (hasChanged)
                {
                    e.Appearance.BackColor = Color.Yellow;
                    e.Appearance.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {


            }

        }
        #endregion
        #region Mạnh
        private void btnChiaChuyenSanXuat_Click(object sender, EventArgs e)
        {
            GetChiTietlenh();
        }
        private void GetChiTietlenh()
        {
            //using (var frm = new frmCanDoiDHChiaSXChiTiet(_focusedMaDH, _focusedMaHang, "", _allowAdd, _allowEdit, _allowDelete))
            //{
            //    frm.ShowDialog();
            //    //frm.Show();
            //    if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
            //        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            //    else
            //        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            //    barEditItem1.EditValue = null;

            //    LoadDSDonHangTong();
            //    //frm.FormClosed += (s, e) =>
            //    //{
            //    //    LoadDSDonHangTong();
            //    //};
            //    if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
            //        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            //}
            var frm = new frmCanDoiDHChiaSXChiTiet(_focusedMaDH, _focusedMaHang, "", _allowAdd, _allowEdit, _allowDelete);

            frm.FormClosed += (s, e) =>
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));

                barEditItem1.EditValue = null;
                LoadDSDonHangTong();
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            };

            frm.Show();
        }

        private void gridControl3_Click(object sender, EventArgs e)
        {

        }

        private void barButtonItem9_ItemClick(object sender, ItemClickEventArgs e)
        {
            GetChiTietlenh();
        }
        #endregion

        #region Quân
        private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                GridView view = gridViewDonHangTong;
                GridView views = gridViewDonHangTong;
                object _objMaDH = null, _objMaHang = null, _objTenHang = null, _objDot = null, _objMaLenh = null, _objPOID = null,
                _objMaDVSX = null, _objMaLenhSanXuat = null, _objNguoiTao = null, _objMaGop = null, _objTenLenh = null, _objSTTLenh = null, _objMaDHGop = null;
                int _objIsKeoVe = 0;
                if (views.IsGroupRow(view.FocusedRowHandle))
                {
                    int childHandle = views.GetChildRowHandle(view.FocusedRowHandle, 0);
                    _objMaDH = views.GetRowCellValue(childHandle, colMaDH);
                    _objMaHang = views.GetRowCellValue(childHandle, colMaHang);
                    _objTenHang = views.GetRowCellValue(childHandle, colTenHang);

                }
                else
                {
                    _objMaDH = views.GetFocusedRowCellValue(colMaDH);
                    _objMaHang = views.GetFocusedRowCellValue(colMaHang);
                    _objTenHang = views.GetFocusedRowCellValue(colTenHang);
                }
                if (view.IsGroupRow(view.FocusedRowHandle))
                {
                    int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                    _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
                    _objMaLenh = view.GetRowCellValue(childHandle, colMaLenh);
                }
                else
                {
                    _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
                    _objMaLenh = view.GetFocusedRowCellValue(colMaLenh);
                }
                int selectedHandle = view.FocusedRowHandle;
                if (selectedHandle < 0) return;
                SaveFileDialog Sfd = new SaveFileDialog();
                Sfd.Title = "File To Save";
                Sfd.Filter = "Excel | *.xlsx";

                DataTable tbl = gridControlChiTietDonHang.DataSource as DataTable;
                //string url = string.Format("{0}", URL + $"TheoDoiDonHang/Get?Action=GETEXCEL1&Para1=''&Para2={_objMaDH}&Para3=A");
                //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                //DataTable dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                DataRow row = view.GetDataRow(selectedHandle);
                DataTable dtblr = gridDonHangTong.DataSource as DataTable;
                Sfd.FileName = string.Format("DonHang{0}_{1}" + DateTime.Now.ToString("ddMMyyyy"), row["MaHang"].ToString(), row["MaCty"].ToString());
                if (tbl.Rows.Count == 0 && tbl == null) return;
                if (Sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(Modules.ResourceForm.frmWait));
                        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption("Xuất B/C");
                        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Đang thực hiện...");
                        DevExpress.XtraPrinting.XlsxExportOptionsEx op = new DevExpress.XtraPrinting.XlsxExportOptionsEx();
                        op.ExportType = DevExpress.Export.ExportType.WYSIWYG;
                        op.ShowGridLines = true;
                        op.SheetName = string.Format("Bao Cao");

                        string fileName = "LSX_ROBE 4.xlsx";
                        string path = Path.Combine(Application.StartupPath, @"Templates\", fileName);
                        string TemplateFileName = path;
                        string ExportFileName = Sfd.FileName;


                        ExportExcelLSX(TemplateFileName, ExportFileName, tbl, row);
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn muốn mở tài liệu này không?", "Thông báo",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start("explorer.exe", Sfd.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm();
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xuất Excel đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ExportExcelLSX(string TemplateFileName, string ExportFileName, DataTable tbl, DataRow rows)
        {
            try
            {
                //string urlMau = string.Format("{0}", URL + $"ERPBangMau/Get?action=GetMauv1");
                DataRow dr = gridViewDonHangTong.GetFocusedDataRow();
                if (dr == null) return;

                string urlMau = $"{URL}ERPDonHangTong/Get?Action=GETBANGMAU&para={dr["MaKH"].ToString()}&para2={dr["MaHang"].ToString()}";
                string jsonmau = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
                DataTable tblmau = JsonConvert.DeserializeObject<DataTable>(jsonmau);

                string urlQG = string.Format("{0}?", URL + "QuocGia/GetQuocGia");
                string jsonQG = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlQG); }).Result;
                DataTable tblqg = JsonConvert.DeserializeObject<DataTable>(jsonQG);

                string urlcty = string.Format("{0}?", URL + "PhapDanhCty/Get");
                string jsoncty = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcty); }).Result;
                DataTable tblcty = JsonConvert.DeserializeObject<DataTable>(jsoncty);

                DataRow _row = rows;
                System.IO.FileInfo file = new System.IO.FileInfo(TemplateFileName);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
                Dictionary<string, System.Drawing.Color> dictionaryTextToColour = new Dictionary<string, System.Drawing.Color>();
                using (OfficeOpenXml.ExcelPackage excelPackage = new OfficeOpenXml.ExcelPackage(file))
                {
                    string templateFilePath = TemplateFileName;

                    string resultFilePath = ExportFileName;
                    FileInfo templateFile = new FileInfo(templateFilePath);
                    ExcelPackage templatePackage = new ExcelPackage(templateFile);
                    ExcelWorksheet worksheet = templatePackage.Workbook.Worksheets[0];
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    decimal sumSL_CapPhap = 0;
                    var rowcty = tblcty.AsEnumerable().FirstOrDefault(x => x["MaCty"].ToString() == _row["MaCty"].ToString());

                    worksheet.Cells["c4"].Value = _row["TenKH"].ToString();
                    worksheet.Cells["c5"].Value = _row["TenHang"].ToString();
                    worksheet.Cells["c6"].Value = _row["Dot"].ToString();
                    worksheet.Cells["c7"].Value = rowcty == null ? "" : rowcty["TenCty"].ToString();
                    worksheet.Cells["c8"].Value = _row["TenCL"].ToString();
                    worksheet.Cells["c9"].Value = _row["BookingMaHang"].ToString();

                    List<string> columnNamesWithSize = tbl.Columns.Cast<DataColumn>()
                       .Where(column => column.ColumnName.Contains("@"))
                       .Select(column => column.ColumnName)
                       .Distinct()
                       .ToList();

                    int colums = 7;
                    range = worksheet.Cells[11, colums, 11, colums + columnNamesWithSize.Count - 1]; range.Value = "Size"; range.Merge = true; range.Style.Font.Color.SetColor(Color.Red); range.Style.Font.Bold = true;
                    foreach (var item in columnNamesWithSize)
                    {
                        string[] _split = item.Split('@');
                        string _size = _split[0].ToString();
                        worksheet.Cells[12, colums].Value = _size;
                        worksheet.Cells[12, colums].Style.Font.Bold = true;
                        colums++;
                    }
                    range = worksheet.Cells[11, colums, 12, colums]; range.Value = "Grand Total"; range.Style.WrapText = true; range.Merge = true; range.Style.Font.Color.SetColor(Color.Red); range.Style.Font.Bold = true;
                    int row = 13;
                    foreach (DataRow item in tbl.Rows)
                    {
                        var rowmau = tblmau.AsEnumerable().FirstOrDefault(r => r["MaMau"].ToString().Trim().ToUpper() == item["MaMau"].ToString().Trim().ToUpper());
                        var rowqg = tblqg.AsEnumerable().FirstOrDefault(r => r["MaQG"].ToString().Trim().ToUpper() == item["MaQG"].ToString().Trim().ToUpper());
                        worksheet.Cells[row, 1].Value = item["ColorCode"].ToString(); worksheet.Cells[row, 1].Style.WrapText = true;
                        worksheet.Cells[row, 2].Value = rowmau["TenMau"].ToString(); worksheet.Cells[row, 2].Style.WrapText = true;
                        worksheet.Cells[row, 3].Value = item["PO"].ToString();
                        worksheet.Cells[row, 4].Value = item["DauSize"].ToString();
                        worksheet.Cells[row, 5].Value = rowqg["TenQG"].ToString();
                        worksheet.Cells[row, 6].Value = Convert.ToDateTime(item["NgayGH"].ToString()).ToString("dd/MM/yyyy");
                        int sumSize = 0;
                        colums = 7;
                        foreach (var lstSize in columnNamesWithSize)
                        {
                            worksheet.Cells[row, colums].Value = Convert.ToInt32(item[lstSize]);
                            worksheet.Cells[row, colums].Style.Font.Color.SetColor(Color.FromArgb(255, 0, 0));
                            sumSize += Convert.ToInt32(item[lstSize]);
                            colums++;
                        }
                        worksheet.Cells[row, colums].Value = sumSize;
                        row++;
                    }
                    range = worksheet.Cells[row, 1]; range.Value = "Grand Total"; range.Style.Font.Color.SetColor(Color.Red);
                    for (int i = 7; i < colums + 1; i++)
                    {
                        worksheet.Cells[row, i].Formula = "=SUM(" + worksheet.Cells[13, i].Address + ":" + worksheet.Cells[row - 1, i].Address + ")";
                        worksheet.Cells[row, i].Style.Font.Bold = true;
                    }
                    range = worksheet.Cells[row, 1, row, colums];
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(218, 150, 148));
                    var borderData = worksheet.Cells[11, 1, row, colums].Style.Border;
                    borderData.Bottom.Style =
                        borderData.Top.Style =
                        borderData.Left.Style =
                        borderData.Right.Style = ExcelBorderStyle.Thin;
                    ///////
                    ///
                    try
                    {
                        // 1️⃣ Lấy danh sách nhân viên từ API
                        string urlNV = string.Format("{0}?", URL + "NhanVien/GetAllNV");
                        string jsonNV = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlNV); }).Result;
                        DataTable dttb = JsonConvert.DeserializeObject<DataTable>(jsonNV);

                        if (dttb != null && dttb.Rows.Count > 0)
                        {
                            // 2️⃣ Tìm nhân viên hiện tại
                            string username = (GlobleData.UserName ?? "").Trim();
                            DataRow[] found = dttb.Select($"UserID = '{username.Replace("'", "''")}'");

                            if (found == null || found.Length == 0)
                            {
                                found = dttb.AsEnumerable()
                                            .Where(r => r["UserID"] != DBNull.Value &&
                                                        r["UserID"].ToString().Trim().Equals(username, StringComparison.OrdinalIgnoreCase))
                                            .ToArray();
                            }

                            if (found != null && found.Length > 0)
                            {
                                string fileName = found[0]["HinhAnh"] == DBNull.Value ? string.Empty : found[0]["HinhAnh"].ToString();
                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    // 3️⃣ Tải file chữ ký từ server
                                    string urlHost = (string)settingsReader.GetValue("HostDH", typeof(String));
                                    if (string.IsNullOrEmpty(urlHost)) urlHost = URL;

                                    string imgUrl = string.Format("{0}/Images/NhanVien/{1}", urlHost.TrimEnd('/'), fileName);
                                    byte[] imgBytes = null;

                                    using (var wc = new WebClient())
                                    {
                                        imgBytes = wc.DownloadData(imgUrl);
                                    }

                                    if (imgBytes != null && imgBytes.Length > 0)
                                    {
                                        using (var ms = new MemoryStream(imgBytes))
                                        using (var img = System.Drawing.Image.FromStream(ms))
                                        {
                                            // 🖋️ 4️⃣ Chèn chữ ký dưới hàng tổng, cách 3 dòng
                                            var picture = worksheet.Drawings.AddPicture("Signature", img);

                                            // Xác định vị trí cột Grand Total (chính là cột cuối cùng)
                                            int colGrandTotal = colums - 1;  // colums đang là vị trí Grand Total
                                            int rowSign = row + 2;       // Cách hàng tổng 1 dòng

                                            // Thêm chữ "Chữ Ký:" ngay bên dưới ô tổng Grand Total
                                            worksheet.Cells[rowSign, colGrandTotal].Value = "Chữ Ký:";
                                            worksheet.Cells[rowSign, colGrandTotal].Style.Font.Bold = true;
                                            worksheet.Cells[rowSign, colGrandTotal].Style.HorizontalAlignment =
                                                OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                                            double cellWidthPx = worksheet.Column(colGrandTotal).Width * 7; // Quy đổi sang px tương đối
                                            double cellHeightPx = 20;

                                            int offsetY = 5;  // khoảng cách giữa chữ và hình
                                            int imageWidth = 150;
                                            int imageHeight = 100;
                                            int offsetX = (int)((cellWidthPx - imageWidth) / 2);

                                            picture.SetPosition(rowSign, (int)(cellHeightPx + offsetY), colGrandTotal - 1, offsetX);
                                            picture.SetSize(imageWidth, imageHeight);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exSign)
                    {
                        System.Diagnostics.Debug.WriteLine("Lỗi chèn chữ ký: " + exSign.Message);
                    }


                    FileInfo resultFile = new FileInfo(resultFilePath);
                    templatePackage.SaveAs(resultFile);
                }
            }
            catch (Exception ex)
            {
                return;
            }


        }

        private DevExpress.XtraGrid.Views.Base.BaseView GetBandGridViewAmountV2(DataTable tab)
        {
            BandedGridView bandedView = new BandedGridView();
            bandedView.OptionsView.ShowColumnHeaders = false;
            bandedView.OptionsView.ShowGroupPanel = false;
            bandedView.OptionsView.ColumnAutoWidth = false;
            bandedView.OptionsView.ShowFooter = true;
            bandedView.OptionsView.AllowCellMerge = true;
            bandedView.OptionsBehavior.AutoExpandAllGroups = true;
            bandedView.OptionsCustomization.AllowBandMoving = false;
            //bandedView.OptionsView.GroupFooterShowMode = GroupFooterShowMode.VisibleAlways;
            if (tab.Columns.Count == 0) return bandedView;
            for (int i = 0; i < 19; i++)
            {
                List<string> _ListString = new List<string>();
                if (tab.Columns[i].ColumnName == "TenDVSX")
                {
                    _ListString.Add("TenDVSX");
                    SetGridBandedViewAmountV1(bandedView, "", "ĐƠN VỊ SX", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "MaLenh")
                {
                    _ListString.Add("MaLenh");
                    SetGridBandedViewAmountV1(bandedView, "", "Mã lệnh SX", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "Name")
                {
                    _ListString.Add("Name");
                    SetGridBandedViewAmountV1(bandedView, "", "Chuyền", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "STUFFED_MaDH")
                {
                    _ListString.Add("STUFFED_MaDH");
                    SetGridBandedViewAmountV1(bandedView, "", "Đơn Hàng Gộp", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "PO")
                {
                    _ListString.Add("PO");
                    SetGridBandedViewAmountV1(bandedView, "", "PO", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "TenMau")
                {
                    _ListString.Add("TenMau");
                    SetGridBandedViewAmountV1(bandedView, "", "Màu", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "DauSize")
                {
                    _ListString.Add("DauSize");
                    SetGridBandedViewAmountV1(bandedView, "", "Nhóm size", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "LC")
                {
                    _ListString.Add("LC");
                    SetGridBandedViewAmountV1(bandedView, "", "Trạng thái", _ListString);
                    _ListString.Clear();
                }
            }
            List<string> listHeader = new List<string>();
            int j = 19;
            string maHang = string.Empty;
            while (j < tab.Columns.Count)
            {
                string[] arrName = tab.Columns[j].ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                maHang = arrName[1];
                listHeader.Add(tab.Columns[j].ColumnName);
                j++;

            }
            GridBand gridBand = new GridBand();
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            BandedGridColumn colAmount = bandedView.Columns.AddField("Amount");
            SetGridBandedViewAmountV1(bandedView, maHang, "", listHeader);

            colAmount.OptionsColumn.AllowEdit = false;
            colAmount.Caption = "Tổng";
            colAmount.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            //colAmount.UnboundExpression = exp;
            colAmount.Visible = true;
            colAmount.OwnerBand = gridBand;
            colAmount.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            gridBand.Visible = true;
            gridBand.Caption = "Tổng";
            GridColumnSummaryItem item = colAmount.Summary.Add(SummaryItemType.Custom);
            item.FieldName = "Amount";
            item.DisplayFormat = "{0:n0}";
            bandedView.Bands.Add(gridBand);
            foreach (BandedGridColumn col in bandedView.Columns)
            {

                if (col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "TenMau" && col.FieldName != "DauSize" && col.FieldName != "MaKH"
                    && col.FieldName != "MaDH" && col.FieldName != "MaHang" && col.FieldName != "TenHang" && col.FieldName != "DotSX" && col.FieldName != "MaQG"
                    && col.FieldName != "DauSizeID" && col.FieldName != "MaDVSX" && col.FieldName != "TenDVSX" && col.FieldName != "MaLenh" && col.FieldName != "LC")
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
            bandedView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            bandedView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bandedView.OptionsNavigation.EnterMoveNextColumn = true;
            return bandedView;
        }

        private void gridView2_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || _dtbCcCache == null || _dtbCcCache.Rows.Count == 0)
                return;

            // Chỉ xử lý nếu đang vẽ cột "Name"
            if (e.Column.FieldName == "Name")
            {
                // Lấy giá trị chuyền hiện tại (ô Name)
                object objLine = view.GetRowCellValue(e.RowHandle, "Name");
                if (objLine == null || objLine == DBNull.Value)
                    return;

                // Kiểm tra xem chuyền này có nằm trong danh sách chuyền chính không
                bool laChuyenChinh = _dtbCcCache.AsEnumerable().Any(r =>
                    r["Name"] != DBNull.Value &&
                    r["Name"].ToString().Trim().Equals(objLine.ToString().Trim(), StringComparison.OrdinalIgnoreCase)
                );

                // Nếu là chuyền chính => tô màu xanh nhạt cho ô Name
                if (laChuyenChinh)
                {
                    e.Appearance.BackColor = ColorTranslator.FromHtml("#84e184"); // xanh nhạt
                    e.Appearance.ForeColor = Color.Black;
                }
            }
            if (e.Column.FieldName == "LC")
            {
                string val = e.CellValue == null ? "" : e.CellValue.ToString();

                if (val == "Đã lên chuyền")
                {
                    // e.Appearance.BackColor = Color.LightGreen;   // xanh lá nhạt
                    e.Appearance.ForeColor = Color.DarkGreen;
                }
                else
                {
                    //e.Appearance.BackColor = Color.LightCoral;   // đỏ nhạt
                    e.Appearance.ForeColor = Color.DarkRed;
                }
            }
        }

        private void gridViewDonHangTong_CustomColumnDisplayText_1(object sender, CustomColumnDisplayTextEventArgs e)
        {
            //if (e.Column.FieldName != "TrangThai") return;

            //var view = sender as GridView;
            //if (view == null) return;

            //int listIndex = e.ListSourceRowIndex;
            //if (listIndex < 0) { e.DisplayText = ""; return; }

            //DataRow dr = view.GetDataRow(listIndex);
            //if (dr == null) { e.DisplayText = ""; return; }

            //int checkDuyet = dr["CheckDuyetNPL"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CheckDuyetNPL"]);
            //int checkBOM = dr["CheckBom"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CheckBom"]);
            //int xacnhan = dr["XacNhan"] == DBNull.Value ? 0 : Convert.ToInt32(dr["XacNhan"]);

            //if (checkDuyet == 1 && checkBOM == 0 && xacnhan == 0)
            //    e.DisplayText = "Đã cấp phát";
            //else if (checkDuyet == 0 && checkBOM == 0 && xacnhan == 0)
            //    e.DisplayText = "Đã có BOM";
            //else if (checkDuyet == 1 && checkBOM == 0 && xacnhan == 1)
            //    e.DisplayText = "Đã ban hành";
            //else
            //    e.DisplayText = "";
        }

        private void gridViewDonHangTong_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            //if (e.Column.FieldName == "TrangThaiText")  // đúng tên FieldName cột trạng thái
            //{
            //    var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

            //    // Lấy giá trị 2 cột nguồn
            //    object cd = view.GetRowCellValue(e.RowHandle, "CheckDuyetNPL");
            //    object cb = view.GetRowCellValue(e.RowHandle, "CheckBom");
            //    object xn = view.GetRowCellValue(e.RowHandle, "XacNhanBH");
            //    object isduyet = view.GetRowCellValue(e.RowHandle, "IsDuyet");
            //    object isactive = view.GetRowCellValue(e.RowHandle, "IsActive");

            //    int checkDuyet = cd == null || cd == DBNull.Value ? 0 : Convert.ToInt32(cd);
            //    int checkBOM = cb == null || cb == DBNull.Value ? 0 : Convert.ToInt32(cb);
            //    int xacnhan = xn == null || xn == DBNull.Value ? 0 : Convert.ToInt32(xn);
            //    int isDuyet = isduyet == null || isduyet == DBNull.Value ? 0 : Convert.ToInt32(isduyet);
            //    int isActive = isactive == null || isactive == DBNull.Value ? 0 : Convert.ToInt32(isactive);

            //    // Tô màu
            //    if (checkDuyet == 1 && checkBOM == 0 && xacnhan == 0 && isDuyet == 1)// Đã ss cấp phát
            //    {
            //        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcad4");
            //        e.Appearance.ForeColor = Color.DarkGreen;
            //    }
            //    else if ( xacnhan == 1) // Đã cp
            //    {
            //        e.Appearance.BackColor = Color.LightYellow;
            //        e.Appearance.ForeColor = Color.OrangeRed;
            //    }
            //    else if (checkDuyet == 0 && checkBOM == 0 && xacnhan == 0 && isDuyet == 1) // Đã có BOM
            //    {
            //        e.Appearance.BackColor = Color.LightSkyBlue;
            //        e.Appearance.ForeColor = Color.DarkBlue;
            //    }
            //}
            if (e.Column.FieldName == "TrangThaiText")  // đúng tên FieldName cột trạng thái
            {
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                // Lấy giá trị 2 cột nguồn
                object cd = view.GetRowCellValue(e.RowHandle, "CheckDuyetNPL");
                object cb = view.GetRowCellValue(e.RowHandle, "CheckBom");
                object xn = view.GetRowCellValue(e.RowHandle, "XacNhanBH");
                object isduyet = view.GetRowCellValue(e.RowHandle, "IsDuyet");
                object isactive = view.GetRowCellValue(e.RowHandle, "IsActive");
                object isduyetlsx = view.GetRowCellValue(e.RowHandle, "IsDuyetLSX");

                int checkDuyet = cd == null || cd == DBNull.Value ? 0 : Convert.ToInt32(cd);
                int checkBOM = cb == null || cb == DBNull.Value ? 0 : Convert.ToInt32(cb);
                int xacnhan = xn == null || xn == DBNull.Value ? 0 : Convert.ToInt32(xn);
                int isDuyet = isduyet == null || isduyet == DBNull.Value ? 0 : Convert.ToInt32(isduyet);
                int isActive = isactive == null || isactive == DBNull.Value ? 0 : Convert.ToInt32(isactive);
                int isDuyetLSX = isduyetlsx == null || isduyetlsx == DBNull.Value ? 0 : Convert.ToInt32(isduyetlsx);
                // Tô màu
                if (isDuyetLSX == 1)// Đã ss cấp phát
                {
                    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffcad4");
                    e.Appearance.ForeColor = Color.DarkGreen;
                }
                //else if ( xacnhan == 1) // Đã cp
                //{
                //    e.Appearance.BackColor = Color.LightYellow;
                //    e.Appearance.ForeColor = Color.OrangeRed;
                //}
                else if (isActive == 1) // Đã có BOM
                {
                    e.Appearance.BackColor = Color.LightSkyBlue;
                    e.Appearance.ForeColor = Color.DarkBlue;
                }
            }
        }
        #endregion


        #region Tam
        private async void loadCanDoiNPLTab(int rh)
        {
            gridView3.ShowLoadingPanel();
            try
            {
                await Task.Run(() =>
                {
                    handleRowAction(rh);
                    getVatTu();
                    calcVatTu();
                    getNgayDongBoDK();
                });
                loadGridControl3();
                loadNgayDongBoDK();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                gridView3.HideLoadingPanel();
            }
        }
        private void loadGridControl3()
        {
            gridControl3.DataSource = gridControl3Table;

            foreach (GridColumn col in gridView3.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();
            textEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            textEdit.Mask.EditMask = "n2";
            textEdit.Mask.UseMaskAsDisplayFormat = true;
            gridView3.Columns["SLNhuCau"].ColumnEdit = textEdit;
            gridView3.Columns["TonKho"].ColumnEdit = textEdit;
            gridView3.Columns["SLCanDoiKho"].ColumnEdit = textEdit;
            gridView3.Columns["SLMuaThem"].ColumnEdit = textEdit;
            gridView3.Columns["TyLeMuaThem"].ColumnEdit = textEdit;
            gridView3.Columns["SLMuaThemTong"].ColumnEdit = textEdit;
            gridView3.Columns["TyLeDongBo"].ColumnEdit = textEdit;
        }
        private void getVatTu()
        {
            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "getvattucancandoi";
            DataRow newRow = request.TypeTable.NewRow();
            newRow["MaDH"] = maDH;
            request.TypeTable.Rows.Add(newRow);
            string urlGetListDataTable = URL + "CanDoiNPL/GetByTypeTable";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            vattuTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!vattuTable.Columns.Contains("generatedID"))
                vattuTable.Columns.Add("generatedID");
            if (!vattuTable.Columns.Contains("SLNhuCau"))
                vattuTable.Columns.Add("SLNhuCau", typeof(decimal));
            if (!vattuTable.Columns.Contains("TyLeMuaThem"))
                vattuTable.Columns.Add("TyLeMuaThem", typeof(decimal));
            if (!vattuTable.Columns.Contains("SLCanDoiKho"))
                vattuTable.Columns.Add("SLCanDoiKho", typeof(decimal));
            if (!vattuTable.Columns.Contains("IsNPLText"))
                vattuTable.Columns.Add("IsNPLText", typeof(string));
            foreach (DataRow row in vattuTable.Rows)
            {
                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");
            }
        }
        private void calcVatTu()
        {
            DataTable dt = vattuTable.Copy();
            gridControl3Table = vattuTable.Clone();
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count == 0) return;
            //foreach (DataRow row in dt.Rows)
            //{
            //    //if (row["DinhMucHaoHut"] == DBNull.Value || row["DinhMucHaoHut"].ToString() == "") row["DinhMucHaoHut"] = 0;
            //    //if (row["DinhMucChung"] == DBNull.Value || row["DinhMucChung"].ToString() == "") row["DinhMucChung"] = 0;
            //    //float slCapPhat = XuLyVTUnits.SmartTryParse<float>(row["DinhMucChung"].ToString()) *
            //    //    XuLyVTUnits.SmartTryParse<float>(row["SoLuong"].ToString()) *
            //    //    (1 + XuLyVTUnits.SmartTryParse<float>(row["DinhMucHaoHut"].ToString()) / 100);
            //    //float slCanDoi = 0;
            //    //slCapPhat = (float)Math.Round(slCapPhat, 2, MidpointRounding.AwayFromZero);
            //    //row["SLNhuCau"] = slCapPhat;

            //}
            gridControl3Table = dt.Copy();
        }


        private void loadNgayDongBoDK()
        {
            if (ngayDBTable == null || ngayDBTable.Columns.Count <= 0 || ngayDBTable.Rows.Count <= 0) return;
            string ngayStr = ngayDBTable.Rows[0]["NgayDKDBo"]?.ToString(); // lấy từ DataTable
            DateTime? ngayValue = null;

            if (!string.IsNullOrWhiteSpace(ngayStr))
            {
                // Parse theo format dd/MM/yyyy
                if (DateTime.TryParseExact(
                    ngayStr,
                    "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDate))
                {
                    ngayValue = parsedDate;
                }
            }
            ngayDBDKEdit.EditValue = ngayValue;
        }
        private void getNgayDongBoDK()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getngaydongbo";
            request.Parameter = maDH;
            string urlGetListDataTable = URL + "CanDoiNPL/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            ngayDBTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);
        }

        private void candoiBtn_Click(object sender, EventArgs e)
        {
            using (frmCanDoiNPLTheoDH frm = new frmCanDoiNPLTheoDH(maDH))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    getVatTu();
                    calcVatTu();
                    loadGridControl3();
                }
            }
        }
        private void handleRowAction(int rowHandle)
        {
            var row = gridViewDonHangTong.GetDataRow(rowHandle);
            if (row == null) return;
            maDH = row["MaDH"]?.ToString();
        }

        private void gridView3_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.CellValue == null || e.CellValue == DBNull.Value) return;

            GridView view = sender as GridView;
            if (e.Column.FieldName == "TyLeMuaThem")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "TonKho")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
            }
            else if (e.Column.FieldName == "SLMuaThemTong")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
                else
                {
                    DataRow row = view.GetDataRow(e.RowHandle);
                    if (row != null && row.Table.Columns.Contains("TrangThaiDH"))
                    {
                        bool trueValue = row["TrangThaiDH"] != DBNull.Value && row["TrangThaiDH"].ToString() == "chinh_5";
                        if (trueValue)
                        {
                            e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                            e.Appearance.BackColor = Color.PaleGreen;
                        }
                    }
                }
            }
            else if (e.Column.FieldName == "SLCanDoiKho")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }
                else
                {
                    DataRow row = view.GetDataRow(e.RowHandle);
                    if (row != null && row.Table.Columns.Contains("IsDuyet"))
                    {
                        bool trueValue = row["IsDuyet"] != DBNull.Value && XuLyVTUnits.SmartTryParse<bool>(row["IsDuyet"].ToString());
                        if (trueValue)
                        {
                            e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                            e.Appearance.BackColor = Color.PaleGreen;
                        }
                    }
                }
            }
            else if (e.Column.FieldName == "SLMuaThem")
            {
                float zeroValue = XuLyVTUnits.SmartTryParse<float>(e.CellValue.ToString());
                if (zeroValue == 0.0)
                {
                    e.Appearance.ForeColor = Color.DarkGray;
                }

            }
        }
        private void gridView3_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "SLMuaThemTong" && e.IsGetData)
            {


                DataRow row = ((DataRowView)e.Row).Row;
                decimal slMuaThemTong = XuLyVTUnits.SmartTryParse<decimal>(row["SLMuaThem"].ToString()) *
                    (1 + XuLyVTUnits.SmartTryParse<decimal>(row["TyLeMuaThem"].ToString()) / 100m);
                e.Value = slMuaThemTong;//a * (1 + b / 100m);
            }
            else if (e.Column.FieldName == "TyLeDongBo" && e.IsGetData)
            {


                DataRow row = ((DataRowView)e.Row).Row;
                decimal slMuaThemTong = XuLyVTUnits.SmartTryParse<decimal>(row["SLMuaThem"].ToString()) *
                    (1 + XuLyVTUnits.SmartTryParse<decimal>(row["TyLeMuaThem"].ToString()) / 100m);
                decimal slCanDoi = XuLyVTUnits.SmartTryParse<decimal>(row["SLCanDoiKho"].ToString());
                decimal slNhuCau = XuLyVTUnits.SmartTryParse<decimal>(row["SLNhuCau"].ToString());
                decimal slDaCap = XuLyVTUnits.SmartTryParse<decimal>(row["SLNhap"].ToString());
                decimal isDuyet = row["IsDuyet"] != DBNull.Value && XuLyVTUnits.SmartTryParse<bool>(row["IsDuyet"].ToString()) ? 1 : 0;
                decimal muaThanhCong = 0;
                e.Value = (slDaCap) * 100m / slNhuCau;
            }
        }
        private void gridView3_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "TyLeDongBo")
            {
                object val = view.GetRowCellValue(e.RowHandle, e.Column);
                if (val == null || val == DBNull.Value) return;

                decimal percent;
                if (!decimal.TryParse(val.ToString(), out percent)) return;
                if (percent == 0) return;
                percent = percent / 100;
                // Giới hạn từ 0 đến 1 (0% đến 100%)
                if (percent < 0) percent = 0;
                if (percent > 1) percent = 1;

                int fillWidth = (int)(e.Bounds.Width * percent);
                Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, fillWidth, e.Bounds.Height);
                e.Graphics.FillRectangle(Brushes.LightGreen, rect);

                // Vẽ viền (tuỳ chọn)
                //e.Graphics.DrawRectangle(Pens.DarkGreen, rect);

                // Vẽ chữ sau cùng để không bị đè
                e.Appearance.DrawString(e.Cache, e.DisplayText, e.Bounds);

                // Đánh dấu đã custom draw
                e.Handled = true;
            }
        }
        private void gridView3_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;

            if (view.IsGroupRow(e.RowHandle))
            {
                // Chỉnh font chữ
                //e.Appearance.Font = new Font("Tahoma", 10, FontStyle.Bold);

                // Chỉnh màu chữ
                e.Appearance.ForeColor = Color.Black;

                // Nếu muốn phân biệt theo level group
                //int level = view.GetRowLevel(e.RowHandle);
                //if (level == 0)
                //{
                //    e.Appearance.ForeColor = Color.DarkBlue;
                //    e.Appearance.Font = new Font("Tahoma", 11, FontStyle.Bold | FontStyle.Underline);
                //}
                //else if (level == 1)
                //{
                //    e.Appearance.ForeColor = Color.Green;
                //    e.Appearance.Font = new Font("Tahoma", 10, FontStyle.Italic);
                //}
            }
        }
        #endregion


        #region Thành phần vật tư
        private void PopupDinMucThanhPhanVatTu(object sender, EventArgs e)
        {

            DataRow rowDH_focused = gridViewDonHangTong.GetFocusedDataRow();
            if (rowDH_focused != null)
            {
                frmERPDinhMucVatTuThanhPhan frm = new frmERPDinhMucVatTuThanhPhan(rowDH_focused);
                frm.Show();

            }
        }
        /*Cân đối*/
        private void gridView3_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;

                DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
                if (view.RowCount > 0)
                {
                    if (e.Menu == null)
                        return;
                    e.Menu.Items.Clear();

                    if (e.HitInfo.InRow)
                    {
                        if (e.HitInfo.Column != null)
                        {

                            DevExpress.Utils.Menu.DXMenuItem menuSpecificationPDFItem = new DevExpress.Utils.Menu.DXMenuItem("Định mức thành phần vật tư", PopupDinMucThanhPhanVatTu);
                            e.Menu.Items.Add(menuSpecificationPDFItem);


                        }

                    }
                }
            }
            catch (Exception ex)
            {


            }
        }
        /*Cấp phát*/
        private void gridViewDinhMucNguyenPhuLieu_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                if (view == null) return;

                DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
                if (view.RowCount > 0)
                {
                    if (e.Menu == null)
                        return;
                    e.Menu.Items.Clear();

                    if (e.HitInfo.InRow)
                    {
                        if (e.HitInfo.Column != null)
                        {

                            DevExpress.Utils.Menu.DXMenuItem menuSpecificationPDFItem = new DevExpress.Utils.Menu.DXMenuItem("Định mức thành phần vật tư", PopupDinMucThanhPhanVatTu);
                            e.Menu.Items.Add(menuSpecificationPDFItem);


                        }

                    }
                }
            }
            catch (Exception ex)
            {


            }
        }
        #endregion
    }
}