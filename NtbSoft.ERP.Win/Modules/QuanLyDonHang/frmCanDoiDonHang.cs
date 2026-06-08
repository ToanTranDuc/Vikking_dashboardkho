using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.QuanLyDonHang;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.CanDoiDonHang;
using NtbSoft.ERP.Win.Modules.Erp.Kehoach;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmCanDoiDonHang : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                             new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        List<CanDoiDonHangTongEntity> lstDonHangTong;
        bool indicatorIcon = true;
        private string _madh = string.Empty, _magop = string.Empty, _mahang = string.Empty, _tenhang = string.Empty, _malenh = string.Empty, _dot = string.Empty, _madvsx = string.Empty, _malenhsanxuat = string.Empty, _poid = string.Empty, _tenlenh = string.Empty, _tendvsx = string.Empty, _po = string.Empty, _nguoitao = string.Empty, _sttlenh = string.Empty;
        private int rowhandel = 0, rowhandel1 = 0, _IsKeoVe = 0;
        DataTable _dtDataLSX;
        List<string> lstPO;
        KeyDownControlHandler keyDownControlHandler;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false, _ChkCT = false, _ChkTH = false;
        int pageIndex;
        int pageSize;
        DataTable tbl;
        public frmCanDoiDonHang()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstDonHangTong = new List<CanDoiDonHangTongEntity>();
            _dtDataLSX = new DataTable();
            lstPO = new List<string>();
            tbl = new DataTable();
        }
        protected override void OnLoad(EventArgs e)
        {
            pageIndex = 1;
            pageSize = 100;
            barEditItemSpinPageSize.EditValue = pageSize;
            barEditItemcbLoc.EditValue = repositoryItemComboBox1.Items[0].ToString();
            LoadData();
            CheckPerminsion();
            InitChart();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
            gridView1.OptionsView.ShowAutoFilterRow = true;
            gridView1.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            GetSearchLookUpDVSX();
        }

        private void GetSearchLookUpDVSX()
        {
            string urlDVSX = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
            string jsonDVSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDVSX); }).Result;
            DataTable dtDVSX = JsonConvert.DeserializeObject<DataTable>(jsonDVSX);
            repositoryItemSearchLookUpDVSX.DataSource = dtDVSX;

        }

        private void CheckPerminsion()
        {
            SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
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
                btChiaSX.Enabled = false;
                barSubItem1.Enabled = false;
            }
            if (!_allowEdit)
            {
                btEdit.Enabled = false;
                barButtonItemNPL.Enabled = false;
            }
            if (!_allowDelete)
                btDelete.Enabled = false;
        }

        private void GetDataChart()
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

            GetDataChart(maLenhSX);
        }

        private void GetDataChart(string maLenhSX)
        {
            // CanDoi
            if (!string.IsNullOrEmpty(maLenhSX))
            {
                List<BaoCaoEntity> lstBaoCao = new List<BaoCaoEntity>();
                string urlBaoCao = string.Format("{0}?maLenhSX={1}", URL + "CanDoiDonHangTong/GetDsBaoCao", maLenhSX);
                string jsonBaoCao = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlBaoCao); }).Result;
                DataTable tblDsBaoCao = JsonConvert.DeserializeObject<DataTable>(jsonBaoCao);
                DrawChart(tblDsBaoCao);
            }
            else
            {
                DrawChart(null);
            }


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

            // Code cũ
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
            seriesPoint2});
                }
            }
        }
        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }
        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, ChiaSX, _allowAdd, ActionType.Add);
            AddActionControl(_lstActionControl, SuaSX, _allowEdit, ActionType.Edit);
            AddActionControl(_lstActionControl, Delete, _allowDelete, ActionType.Delete);
            AddActionControl(_lstActionControl, LoadData, true, ActionType.Refresh);
            AddActionControl(_lstActionControl, CanDoiDonHang, (_allowAdd || _allowEdit), ActionType.Balance);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }

        private void LoadData()
        {
            //rowhandel = gridViewDonHangTong.FocusedRowHandle;
            //rowhandel1 = gridView1.FocusedRowHandle;
            string url = string.Format("{0}?pageIndex={1}&&pageSize={2}", URL + "CanDoiDonHangTong/GetDonHangTong", pageIndex, pageSize);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || (tbl != null && tbl.Rows.Count <= 0))
            {
                NtbSoft.ERP.Libs.clsConvert<CanDoiDonHangTongEntity> convert = new Libs.clsConvert<CanDoiDonHangTongEntity>();
                DataTable tblnull = convert.ToDataTable(lstDonHangTong);
                gridDonHangTong.DataSource = tblnull;
                gridControl1.DataSource = null;
                GetDataChart();
            }
            else
            {
                if (barEditItemcbLoc.EditValue != null)
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
                }
                gridDonHangTong.DataSource = tbl;
                gridViewDonHangTong.FocusedRowHandle = rowhandel;
                LoadLenhSX(this.gridViewDonHangTong);
                if (rowhandel1 < 0)
                    rowhandel1 = 0;
                gridView1.FocusedRowHandle = rowhandel1;
                GetDataChart();

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
        private void ChiaSX()
        {
            rowhandel = gridViewDonHangTong.FocusedRowHandle;
            rowhandel1 = gridView1.FocusedRowHandle;
            frmCanDoiDonHangChiTiet frm = new frmCanDoiDonHangChiTiet(false, _madh, _mahang, _tenhang, _dot, _poid, "", "", "", "", "", "", "", 0);
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
            LoadData();
        }
        private void btChiaSX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ChiaSX();
        }
        private void LoadLenhSX(GridView view)
        {
            object _objMaDH = null, _objMaHang = null, _objTenHang = null, _objDot = null, _objMaLenh = null, _objPOID = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaDH = view.GetRowCellValue(childHandle, colNMaDH);
                _objMaHang = view.GetRowCellValue(childHandle, colNMaHang);
                _objTenHang = view.GetRowCellValue(childHandle, colNTenHang);
                _objDot = view.GetRowCellValue(childHandle, colNDot);
                //_objMaLenh = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
                _objPOID = view.GetRowCellValue(childHandle, colNPOID);
            }
            else
            {
                _objMaDH = view.GetFocusedRowCellValue(colNMaDH);
                _objMaHang = view.GetFocusedRowCellValue(colNMaHang);
                _objTenHang = view.GetFocusedRowCellValue(colNTenHang);
                _objDot = view.GetFocusedRowCellValue(colNDot);
                //_objMaLenh = view.GetFocusedRowCellValue(colMaLenhSanXuat);
                _objPOID = view.GetFocusedRowCellValue(colPOID);
            }
            if (_objMaDH != null)
                _madh = _objMaDH.ToString();
            if (_objMaHang != null)
                _mahang = _objMaHang.ToString();
            if (_objTenHang != null)
                _tenhang = _objTenHang.ToString();
            if (_objDot != null)
                _dot = _objDot.ToString();
            //if (_objMaLenh != null)
            //    _malenhsanxuat = _objMaLenh.ToString();
            if (_objPOID != null)
                _poid = _objPOID.ToString();

            string urlLSX = string.Format("{0}?madh={1}&&poid={2}", URL + "CanDoiDonHangTong/GetLenhSX", _madh, _poid);
            string jsonLSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlLSX); }).Result;
            _dtDataLSX = JsonConvert.DeserializeObject<DataTable>(jsonLSX);
            gridControl1.DataSource = _dtDataLSX;
            if (gridView1.FocusedRowHandle < 0)
                gridView1.FocusedRowHandle = 1;
            GetDataChart();
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
        private void gridView1_CustomDrawRowFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridView view = (GridView)sender;

            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds,
            Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gridView1.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InGroupRow || e.HitInfo.InRow)
                {
                    DevExpress.Utils.Menu.DXMenuItem menuChiTietCanDoiDMNPL = new DevExpress.Utils.Menu.DXMenuItem("Sửa", ItemsDMNPL_Click);
                    e.Menu.Items.Add(menuChiTietCanDoiDMNPL);

                }
            }
        }

        private void ItemsDMNPL_Click(object sender, EventArgs e)
        {
            //frmCanDoiDinhMucNPLChiTiet frm = new frmCanDoiDinhMucNPLChiTiet(_madh, _mahang, _tenhang, _malenh, _dot, _madvsx, _malenhsanxuat, _poid);
            //frm.ShowDialog();
            //frm.WindowState = FormWindowState.Maximized;
            //LoadData();
        }

        private void btCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            else
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            rowhandel = gridViewDonHangTong.FocusedRowHandle;
            rowhandel1 = gridView1.FocusedRowHandle;
            barEditItemcbLoc.EditValue = repositoryItemComboBox1.Items[0].ToString();
            LoadData();
            if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
        }

        private void gridView1_RowChanged()
        {
            GridView view = this.gridView1;
            object _objMaDH = null, _objMaHang = null, _objTenHang = null, _objMaLenh = null, _objDot = null, _objMaDVSX = null, _objTenLenh = null, _objMaLenhSanXuat = null, _objPoid = null, _objTenDVSX = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaDH = view.GetRowCellValue(childHandle, colNMaDH);
                _objMaHang = view.GetRowCellValue(childHandle, colMaHang);
                _objTenHang = view.GetRowCellValue(childHandle, colTenHang);
                _objMaLenh = view.GetRowCellValue(childHandle, colMaLenh);
                _objDot = view.GetRowCellValue(childHandle, colDot);
                _objMaDVSX = view.GetRowCellValue(childHandle, colMaDVSX);
                _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
                _objPoid = view.GetRowCellValue(childHandle, colNPOID);
                _objTenLenh = view.GetRowCellValue(childHandle, colTenLenh);
                _objTenDVSX = view.GetRowCellValue(childHandle, colTenDVSX);
            }
            else
            {
                _objMaDH = view.GetFocusedRowCellValue(colNMaDH);
                _objMaHang = view.GetFocusedRowCellValue(colMaHang);
                _objTenHang = view.GetFocusedRowCellValue(colTenHang);
                _objMaLenh = view.GetFocusedRowCellValue(colMaLenh);
                _objDot = view.GetFocusedRowCellValue(colDot);
                _objMaDVSX = view.GetFocusedRowCellValue(colMaDVSX);
                _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
                _objPoid = view.GetFocusedRowCellValue(colNPOID);
                _objTenLenh = view.GetFocusedRowCellValue(colTenLenh);
                _objTenDVSX = view.GetFocusedRowCellValue(colTenDVSX);
            }
            if (_objMaDH != null)
                _madh = _objMaDH.ToString();

            if (_objMaHang != null)
                _mahang = _objMaHang.ToString();

            if (_objTenHang != null)
                _tenhang = _objTenHang.ToString();

            if (_objMaLenh != null)
                _malenh = _objMaLenh.ToString();

            if (_objDot != null)
                _dot = _objDot.ToString();

            if (_objMaDVSX != null)
                _madvsx = _objMaDVSX.ToString();

            if (_objMaLenhSanXuat != null)
                _malenhsanxuat = _objMaLenhSanXuat.ToString();

            if (_objPoid != null)
                _poid = _objPoid.ToString();

            if (_objTenLenh != null)
                _tenlenh = _objTenLenh.ToString();

            if (_objTenDVSX != null)
                _tendvsx = _objTenDVSX.ToString();
            if (gridView1.FocusedRowHandle >= 0)
            {
                DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                List<DataRow> lst = _dtDataLSX.AsEnumerable()
                    .Where(x => x["MaLenh"].ToString() == row["MaLenh"].ToString())
                    .ToList();
                lstPO = lst.AsEnumerable()
                                .Select(x => x["PO"].ToString())
                                .ToList();
            }
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = (GridView)sender;
            object _objMaDH = null, _objMaHang = null, _objTenHang = null, _objMaLenh = null, _objDot = null, _objMaDVSX = null, _objTenLenh = null,
                   _objMaLenhSanXuat = null, _objPoid = null, _objTenDVSX = null, _objMaGop = null, _objNguoiTao = null, _objSttLenh = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaDH = view.GetRowCellValue(childHandle, colNMaDH);
                _objMaGop = view.GetRowCellValue(childHandle, colMaGop);
                _objMaHang = view.GetRowCellValue(childHandle, colMaHang);
                _objTenHang = view.GetRowCellValue(childHandle, colTenHang);
                _objMaLenh = view.GetRowCellValue(childHandle, colMaLenh);
                _objDot = view.GetRowCellValue(childHandle, colDot);
                _objMaDVSX = view.GetRowCellValue(childHandle, colMaDVSX);
                _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
                _objPoid = view.GetRowCellValue(childHandle, colNPOID);
                _objTenLenh = view.GetRowCellValue(childHandle, colTenLenh);
                _objTenDVSX = view.GetRowCellValue(childHandle, colTenDVSX);
                _objNguoiTao = view.GetRowCellValue(childHandle, colEmployeeID);
                _objSttLenh = view.GetRowCellValue(childHandle, colSTTLenh);
            }
            else
            {
                _objMaDH = view.GetFocusedRowCellValue(colNMaDH);
                _objMaGop = view.GetFocusedRowCellValue(colMaGop);
                _objMaHang = view.GetFocusedRowCellValue(colMaHang);
                _objTenHang = view.GetFocusedRowCellValue(colTenHang);
                _objMaLenh = view.GetFocusedRowCellValue(colMaLenh);
                _objDot = view.GetFocusedRowCellValue(colDot);
                _objMaDVSX = view.GetFocusedRowCellValue(colMaDVSX);
                _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
                _objPoid = view.GetFocusedRowCellValue(colNPOID);
                _objTenLenh = view.GetFocusedRowCellValue(colTenLenh);
                _objTenDVSX = view.GetFocusedRowCellValue(colTenDVSX);
                _objNguoiTao = view.GetFocusedRowCellValue(colEmployeeID);
                _objSttLenh = view.GetFocusedRowCellValue(colSTTLenh);
            }
            if (_objMaDH != null)
                _madh = _objMaDH.ToString();

            if (_objMaGop != null)
                _magop = _objMaGop.ToString();

            if (_objMaHang != null)
                _mahang = _objMaHang.ToString();

            if (_objTenHang != null)
                _tenhang = _objTenHang.ToString();

            if (_objMaLenh != null)
                _malenh = _objMaLenh.ToString();

            if (_objDot != null)
                _dot = _objDot.ToString();

            if (_objMaDVSX != null)
                _madvsx = _objMaDVSX.ToString();

            if (_objMaLenhSanXuat != null)
                _malenhsanxuat = _objMaLenhSanXuat.ToString();

            if (_objPoid != null)
                _poid = _objPoid.ToString();

            if (_objTenLenh != null)
                _tenlenh = _objTenLenh.ToString();

            if (_objTenDVSX != null)
                _tendvsx = _objTenDVSX.ToString();

            if (_objNguoiTao != null)
                _nguoitao = _objNguoiTao.ToString();

            if (_objSttLenh != null)
                _sttlenh = _objSttLenh.ToString();

            if (gridView1.FocusedRowHandle >= 0)
            {
                DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                List<DataRow> lst = _dtDataLSX.AsEnumerable()
                    .Where(x => x["MaLenh"].ToString() == row["MaLenh"].ToString())
                    .ToList();
                lstPO = lst.AsEnumerable()
                                .Select(x => x["PO"].ToString())
                                .ToList();
            }
        }

        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            GridView view = (GridView)sender;
            if (e.ListSourceRowIndex < 0) return;
            if (e.Column == colMaLenh)
            {
                int chilHandle = view.GetChildRowHandle(e.GroupRowHandle, 0);
                if (chilHandle < 0) chilHandle = 0;
                e.DisplayText = e.DisplayText + string.Format(" Đơn vị sản xuất: {0} - Đợt: {1} - Số lượng:{2}", view.GetRowCellValue(chilHandle, colTenDVSX), view.GetRowCellValue(chilHandle, colDot), view.GetRowCellValue(chilHandle, colSoLuong));
            }
        }
        private void SuaSX()
        {
            rowhandel = gridViewDonHangTong.FocusedRowHandle;
            rowhandel1 = gridView1.FocusedRowHandle;
            if (_dtDataLSX == null || _dtDataLSX.Rows.Count == 0) return;
            frmCanDoiDinhMucNPLChiTiet frm = new frmCanDoiDinhMucNPLChiTiet(_madh, _mahang, _tenhang, _malenh, _dot, _madvsx, _malenhsanxuat, _poid, _IsKeoVe, _nguoitao, _magop, _tenlenh, _sttlenh);
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
            LoadData();
        }
        private void btEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SuaSX();
        }
        private void Delete()
        {
            try
            {
                GridView view = (GridView)gridView1;
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
                if (_objIsKeoVe == 1)
                {
                    XtraMessageBox.Show("Lệnh đã lấy về sản xuất. Vui lòng không được xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa lệnh này không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {
                    string url = string.Format("{0}?madh={1}&&malenh={2}&&malenhsanxuat={3}&&username={4}", URL + "CanDoiDonHangTong/Delete", _magop, _malenh, _malenhsanxuat, GlobleData.UserName);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadData();
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Xóa dòng đã xảy ra lỗi.Vui lòng kiểm tra và thực hiện lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (GlobleData.UserName.ToString().ToUpper() == _nguoitao.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
                Delete();
            else
            {
                XtraMessageBox.Show("User này không phải là người tạo, không có quyền xóa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void gridViewDonHangTong_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void gridViewDonHangTong_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = (GridView)sender;
            LoadLenhSX(view);
            GetDataChart();
        }


        private void gridViewDonHangTong_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gridView1_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gridView1_RowClick(object sender, RowClickEventArgs e)
        {
            GridView view = (GridView)sender;
            object _objMaDH = null, _objMaHang = null, _objTenHang = null, _objMaLenh = null, _objDot = null, _objMaDVSX = null, _objMaLenhSanXuat = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaDH = view.GetRowCellValue(childHandle, colNMaDH);
                _objMaHang = view.GetRowCellValue(childHandle, colMaHang);
                _objTenHang = view.GetRowCellValue(childHandle, colTenHang);
                _objMaLenh = view.GetRowCellValue(childHandle, colMaLenh);
                _objDot = view.GetRowCellValue(childHandle, colDot);
                _objMaDVSX = view.GetRowCellValue(childHandle, colMaDVSX);
                _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
            }
            else
            {
                _objMaDH = view.GetFocusedRowCellValue(colNMaDH);
                _objMaHang = view.GetFocusedRowCellValue(colMaHang);
                _objTenHang = view.GetFocusedRowCellValue(colTenHang);
                _objMaLenh = view.GetFocusedRowCellValue(colMaLenh);
                _objDot = view.GetFocusedRowCellValue(colDot);
                _objMaDVSX = view.GetFocusedRowCellValue(colMaDVSX);
                _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
            }
            if (_objMaDH != null)
                _madh = _objMaDH.ToString();

            if (_objMaHang != null)
                _mahang = _objMaHang.ToString();

            if (_objTenHang != null)
                _tenhang = _objTenHang.ToString();

            if (_objMaLenh != null)
                _malenh = _objMaLenh.ToString();
            if (_objMaLenhSanXuat != null)
                _malenhsanxuat = _objMaLenhSanXuat.ToString();

            if (_objDot != null)
                _dot = _objDot.ToString();

            if (_objMaDVSX != null)
                _madvsx = _objMaDVSX.ToString();
        }


        private void gridViewDonHangTong_RowClick(object sender, RowClickEventArgs e)
        {
            GridView view = (GridView)sender;
            LoadLenhSX(view);
            GetDataChart();
        }

        private void barButtonItemPrev_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (pageIndex > 1)
            {
                pageIndex = pageIndex - 1;
                barStaticItemPageIndex.Caption = pageIndex.ToString();
            }
            LoadData();
        }

        private void barButtonItemNext_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pageIndex = pageIndex + 1;
            barStaticItemPageIndex.Caption = pageIndex.ToString();
            LoadData();
        }

        private void repositoryItemSpinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            SpinEdit spinEdit = sender as SpinEdit;
            if (spinEdit != null && spinEdit.EditValue != null && Convert.ToInt32(spinEdit.EditValue) >= 0)
            {
                pageSize = Convert.ToInt32(spinEdit.EditValue);
                LoadData();
            }
        }

        private void gridView1_RowClick_1(object sender, RowClickEventArgs e)
        {
            GetDataChart();
        }



        private void gridView1_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {

            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red; // Màu đỏ
        }


        private void gridViewDonHangTong_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {

            GridView view = sender as GridView;
            if (e.Column.FieldName == "SLKH")
            {
                e.Appearance.ForeColor = Color.Green;
            }
            else
            {
                if (e.Column.FieldName == "SLTH")
                {
                    e.Appearance.ForeColor = Color.Navy;
                }
                else
                {
                    if (e.Column.FieldName == "SLCL")
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                }

            }

        }

        private void splitContainerControl1_SizeChanged(object sender, EventArgs e)
        {
            splitContainerControl1.SplitterPosition = splitContainerControl1.Height / 2;
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
            //GridView gridview = ((GridView)sender);
            //if (!gridview.GridControl.IsHandleCreated) return;
            //Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            //SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            //gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }


        private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridView1_RowCountChanged(object sender, EventArgs e)
        {
            //GridView gridview = ((GridView)sender);
            //if (!gridview.GridControl.IsHandleCreated) return;
            //Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            //SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            //gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void gridViewDonHangTong_ShowingEditor(object sender, CancelEventArgs e)
        {
            //GridView gridView = sender as GridView;
            //if (gridView == null)
            //    return;

            //string filterString = gridView.RowCount.ToString();
            //ApplyFilterToGridView2(filterString);
        }

        private void ApplyFilterToGridView2(string filterString)
        {
            //gridView1.ActiveFilterString = filterString;
        }

        private void barEditItemcbLoc_EditValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void gridView1_PopupMenuShowing_1(object sender, PopupMenuShowingEventArgs e)
        {
            DataRow _rowFocus = gridView1.GetFocusedDataRow();
            if (_rowFocus == null || e.Menu == null) return;
            DXMenuItem menuCanDoiDMNL = new DXMenuItem();
            menuCanDoiDMNL.Caption = "Cân đối định mức nguyên liệu";
            menuCanDoiDMNL.Appearance.Font = new Font("Arial", 9); // Đặt size là 12
            menuCanDoiDMNL.Click += MenuCanDoiDMNL;
            e.Menu.Items.Add(menuCanDoiDMNL);
        }

        private void MenuCanDoiDMNL(object sender, EventArgs e)
        {
            DataRow _rowFocus = gridView1.GetFocusedDataRow();
            frmCanDoiDinhMucNguyenLieu frm = new frmCanDoiDinhMucNguyenLieu(_rowFocus);
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
        }

        private void barBtnSearchLenhSX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

                frmSearchOrderProduction frm = new frmSearchOrderProduction(screenLocation.X, screenLocation.Y + link.Bounds.Height);
                frm.ShowDialog();
                string resultDH = frm._resultDonHang;
                if (resultDH == "Clean")
                {
                    LoadData();
                    return;
                }
                if (string.IsNullOrEmpty(resultDH)) return;
                //SetSearchDHString(this.gridViewDonHangTong, resultDH);
                string url = string.Format("{0}?maDH={1}", URL + "CanDoiDonHangTong/GetDonHangTong_MaDH", resultDH);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                tbl = JsonConvert.DeserializeObject<DataTable>(json);
                gridDonHangTong.DataSource = tbl;
                gridViewDonHangTong.FocusedRowHandle = rowhandel;
                LoadLenhSX(this.gridViewDonHangTong);
                if (rowhandel1 < 0)
                    rowhandel1 = 0;
                gridView1.FocusedRowHandle = rowhandel1;
                GetDataChart();
            }
        }

        private void gridViewDonHangTong_DataSourceChanged(object sender, EventArgs e)
        {
            //rowhandel = gridViewDonHangTong.FocusedRowHandle;
            GridView view = (GridView)sender;
            //LoadLenhSX(view);
        }

        private void gridView1_RowStyle(object sender, RowStyleEventArgs e)
        {

            //GridView view = sender as GridView;
            //if (e.RowHandle >= 0)
            //{
            //    if (e.RowHandle == gridView1.FocusedRowHandle)
            //    {
            //        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ddd");
            //        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
            //        e.HighPriority = true;
            //    }
            //}
        }

        private void gridView1_DataSourceChanged(object sender, EventArgs e)
        {
            GridView view = (GridView)sender;
            object _objMaDH = null, _objMaGop = null, _objMaHang = null, _objTenHang = null, _objMaLenh = null, _objDot = null, _objMaDVSX = null, _objTenLenh = null, _objMaLenhSanXuat = null, _objPoid = null, _objTenDVSX = null, _objNguoiTao = null, _objSttLenh = null;
            int _objIsKeoVe = 0;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaDH = view.GetRowCellValue(childHandle, colNMaDH);
                _objMaGop = view.GetRowCellValue(childHandle, colMaGop);
                _objMaHang = view.GetRowCellValue(childHandle, colMaHang);
                _objTenHang = view.GetRowCellValue(childHandle, colTenHang);
                _objMaLenh = view.GetRowCellValue(childHandle, colMaLenh);
                _objDot = view.GetRowCellValue(childHandle, colDot);
                _objMaDVSX = view.GetRowCellValue(childHandle, colMaDVSX);
                _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
                _objPoid = view.GetRowCellValue(childHandle, colNPOID);
                _objTenLenh = view.GetRowCellValue(childHandle, colTenLenh);
                _objTenDVSX = view.GetRowCellValue(childHandle, colTenDVSX);
                _objIsKeoVe = Convert.ToInt32(view.GetRowCellValue(childHandle, colIsKeoVe));
                _objNguoiTao = view.GetRowCellValue(childHandle, colEmployeeID);
                _objSttLenh = view.GetRowCellValue(childHandle, colSTTLenh);
            }
            else
            {
                _objMaDH = view.GetFocusedRowCellValue(colNMaDH);
                _objMaGop = view.GetFocusedRowCellValue(colMaGop);
                _objMaHang = view.GetFocusedRowCellValue(colMaHang);
                _objTenHang = view.GetFocusedRowCellValue(colTenHang);
                _objMaLenh = view.GetFocusedRowCellValue(colMaLenh);
                _objDot = view.GetFocusedRowCellValue(colDot);
                _objMaDVSX = view.GetFocusedRowCellValue(colMaDVSX);
                _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
                _objPoid = view.GetFocusedRowCellValue(colNPOID);
                _objTenLenh = view.GetFocusedRowCellValue(colTenLenh);
                _objTenDVSX = view.GetFocusedRowCellValue(colTenDVSX);
                _objIsKeoVe = Convert.ToInt32(view.GetFocusedRowCellValue(colIsKeoVe));
                _objNguoiTao = view.GetFocusedRowCellValue(colEmployeeID);
                _objSttLenh = view.GetFocusedRowCellValue(colSTTLenh);
            }
            if (_objMaDH != null)
                _madh = _objMaDH.ToString();

            if (_objMaGop != null)
                _magop = _objMaGop.ToString();

            if (_objMaHang != null)
                _mahang = _objMaHang.ToString();

            if (_objTenHang != null)
                _tenhang = _objTenHang.ToString();

            if (_objMaLenh != null)
                _malenh = _objMaLenh.ToString();

            if (_objDot != null)
                _dot = _objDot.ToString();

            if (_objMaDVSX != null)
                _madvsx = _objMaDVSX.ToString();

            if (_objMaLenhSanXuat != null)
                _malenhsanxuat = _objMaLenhSanXuat.ToString();

            if (_objPoid != null)
                _poid = _objPoid.ToString();

            if (_objTenLenh != null)
                _tenlenh = _objTenLenh.ToString();

            if (_objTenDVSX != null)
                _tendvsx = _objTenDVSX.ToString();
            if (_objIsKeoVe != null)
                _IsKeoVe = _objIsKeoVe;

            if (_objNguoiTao != null)
                _nguoitao = _objNguoiTao.ToString();

            if (_objSttLenh != null)
                _sttlenh = _objSttLenh.ToString();

            //DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            //if (_dtDataLSX.Rows.Count == 0) return;
            //List<DataRow> lst = _dtDataLSX.AsEnumerable()
            //    .Where(x => x["MaLenh"].ToString() == row["MaLenh"].ToString())
            //    .ToList();
            //lstPO = lst.AsEnumerable()
            //                .Select(x => x["PO"].ToString())
            //                .ToList();

        }

        private void gridViewDonHangTong_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void gridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void CanDoiDonHang()
        {
            rowhandel = gridViewDonHangTong.FocusedRowHandle;
            rowhandel1 = gridView1.FocusedRowHandle;
            int total = _dtDataLSX.AsEnumerable().Sum(x => Convert.ToInt32(x["SoLuong"]));
            //frmCanDoiDinhMucNPL frm = new frmCanDoiDinhMucNPL(_madh, _mahang, _tenhang, _malenh, _tenlenh, _malenhsanxuat, _dot, _madvsx, _tendvsx, total, string.Join(";", lstPO.Distinct()),_magop);
            //frm.ShowDialog();
            //LoadData();
        }

        private void barButtonItemNPL_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (GlobleData.UserName.ToString().ToUpper() == _nguoitao.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
            //{
            CanDoiDonHang();
            //}
            //else
            //{
            //    XtraMessageBox.Show("User này không phải là người tạo, không có quyền sửa. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

        }
        private void btCapPhat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (GlobleData.UserName.ToString().ToUpper() == _nguoitao.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
            //{
            rowhandel = gridViewDonHangTong.FocusedRowHandle;
            rowhandel1 = gridView1.FocusedRowHandle;
            BtImportCapPhat();
            //}   
            //else
            //{
            //    XtraMessageBox.Show("User này không phải là người tạo, không có quyền cấp phát NPL. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

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
            if (view == null) return;
            object _objMaDH = null, _objMaLenhSanXuat = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
            }
            else
            {
                _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
            }

            if (_objMaLenhSanXuat != null)
                _malenhsanxuat = _objMaLenhSanXuat.ToString();
            else
                _malenhsanxuat = "";

            if (_magop.ToString() == "" || _malenhsanxuat.ToString() == "")
            {

                MessageBox.Show("Vui lòng chọn đơn hàng và lệnh sản xuất.! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_ChkCT || !_ChkTH)
            {
                string url = string.Format("{0}/?madh={1}&&malenhsanxuat={2}", URL + "CanDoiDonHangTong/GetKiemTraDinhMuc", _magop, _malenhsanxuat);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                List<DinhMucEntity> listDMNPL = JsonConvert.DeserializeObject<List<DinhMucEntity>>(json);

                if (listDMNPL == null || listDMNPL.Count == 0)
                {
                    frmErpImport_DMNL frm = new frmErpImport_DMNL(_magop, _malenhsanxuat, _ChkCT, _ChkTH,"");
                    frm.ShowDialog();
                }
                else
                {
                    if (_ChkCT || _ChkTH)
                    {
                        frmErpImport_DMNL frm = new frmErpImport_DMNL(_magop, _malenhsanxuat, _ChkCT, _ChkTH,"");
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
        private void btCapThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (GlobleData.UserName.ToString().ToUpper() == _nguoitao.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
            //{
            rowhandel = gridViewDonHangTong.FocusedRowHandle;
            rowhandel1 = gridView1.FocusedRowHandle;
            BtImportCapThem();
            //}
            //else
            //{
            //    XtraMessageBox.Show("User này không phải là người tạo, không có quyền cấp thêm NPL. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

        }
        private void BtImportCapThem()
        {
            GridView view = (GridView)gridView1;
            if (view == null) return;
            object _objMaDH = null, _objMaLenhSanXuat = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
            }
            else
            {
                _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
            }

            if (_objMaLenhSanXuat != null)
                _malenhsanxuat = _objMaLenhSanXuat.ToString();
            else
                _malenhsanxuat = "";

            if (_magop.ToString() == "" || _malenhsanxuat.ToString() == "")
            {

                MessageBox.Show("Vui lòng chọn đơn hàng và lệnh sản xuất.! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _ChkCT = true;
            _ChkTH = false;
            string url = string.Format("{0}/?madh={1}&&malenhsanxuat={2}", URL + "CanDoiDonHangTong/GetKiemTraDinhMuc", _magop, _malenhsanxuat);
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
        private void btThuHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (GlobleData.UserName.ToString().ToUpper() == _nguoitao.ToString().ToUpper() || GlobleData.UserName.ToString().ToUpper() == "ADMIN")
            //{
            rowhandel = gridViewDonHangTong.FocusedRowHandle;
            rowhandel1 = gridView1.FocusedRowHandle;
            BtImportThuHoi();
            //}
            //else
            //{
            //    XtraMessageBox.Show("User này không phải là người tạo, không có quyền thu hồi NPL. Vui lòng kiểm tra lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

        }
        private void BtImportThuHoi()
        {
            GridView view = (GridView)gridView1;
            if (view == null) return;
            object _objMaDH = null, _objMaLenhSanXuat = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaLenhSanXuat = view.GetRowCellValue(childHandle, colMaLenhSanXuat);
            }
            else
            {
                _objMaLenhSanXuat = view.GetFocusedRowCellValue(colMaLenhSanXuat);
            }

            if (_objMaLenhSanXuat != null)
                _malenhsanxuat = _objMaLenhSanXuat.ToString();
            else
                _malenhsanxuat = "";

            if (_magop.ToString() == "" && _malenhsanxuat.ToString() == "")
            {

                MessageBox.Show("Vui lòng chọn đơn hàng và lệnh sản xuất.! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _ChkCT = false;
            _ChkTH = true;
            string url = string.Format("{0}/?madh={1}&&malenhsanxuat={2}", URL + "CanDoiDonHangTong/GetKiemTraDinhMuc", _magop, _malenhsanxuat);
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
        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;

            // Kiểm tra nếu đây là ô cần được tùy chỉnh
            if (e.Column.FieldName == "MaLenh")
            {
                if (e.RowHandle >= 0)
                {
                    string Strflagkv = view.GetRowCellDisplayText(e.RowHandle, view.Columns["IsKeoVe"]);
                    string Strflagtt = view.GetRowCellDisplayText(e.RowHandle, view.Columns["TrangThai"]);
                    string Strflagc = view.GetRowCellDisplayText(e.RowHandle, view.Columns["IsChangeData"]);
                    if (Strflagkv == "1" && Strflagc == "0")
                    {
                        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#00FF00"); //Color.LightCyan;
                    }
                    if (Strflagtt == "1" && Strflagc == "1")
                    {
                        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#FF6666");
                        //e.Appearance.BackColor2 = Color.SeaShell;
                    }
                }
            }
            if (e.Column.FieldName == "SoLuong")
            {
                if (e.RowHandle >= 0)
                {
                    string Strflagkv = view.GetRowCellDisplayText(e.RowHandle, view.Columns["IsKeoVe"]);
                    string Strflagtt = view.GetRowCellDisplayText(e.RowHandle, view.Columns["TrangThai"]);
                    string Strflagc = view.GetRowCellDisplayText(e.RowHandle, view.Columns["IsChangeData"]);
                    if (Strflagtt == "1" && Strflagc == "1")
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }

                }
            }
        }

        private void btCapThemNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //rowhandel = gridViewDonHangTong.FocusedRowHandle;
            //rowhandel1 = gridView1.FocusedRowHandle;
            //BtImportCapThem();
        }

        private void gridViewDonHangTong_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.RowHandle >= 0)
            {
                string Strflth = view.GetRowCellDisplayText(e.RowHandle, view.Columns["SLTH"]);
                string Strfcl = view.GetRowCellDisplayText(e.RowHandle, view.Columns["SLCL"]);
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

        private void SetSearchDHString(GridView gridView, string searchString)
        {
            // Thiết lập chuỗi tìm kiếm vào ô tìm kiếm của GridView
            gridView.ActiveFilterString = string.Format("[MaDH] LIKE '%{0}%'", searchString);
        }

    }
}
