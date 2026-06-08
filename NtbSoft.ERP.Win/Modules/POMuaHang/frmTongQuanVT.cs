using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.POMuaHang;
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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmTongQuanVT : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        KeyDownControlHandler keyDownControlHandler;

        private DataTable gridControl1Table;
        private DataTable gridControl2Table;
        private DataTable gridControl3Table;
        private DataTable candoiTable;
        private DataTable muahangTable;
        private DataTable vattuTable;
        private DataTable nhacungcapTable;

        private string maNhaCC;

        public frmTongQuanVT()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            loadComboBoxEdit1();
            layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Maximized;
            chartControl1.Dock = DockStyle.Fill;
            chartControl2.Dock = DockStyle.Fill;
        }


        // lọc theo
        private void loadComboBoxEdit1()
        {
            comboBoxEdit1.Properties.Items.Add("Vật tư");
            comboBoxEdit1.Properties.Items.Add("Nhà cung cấp");

            // Chọn item mặc định
            comboBoxEdit1.SelectedIndex = 0;
        }
        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEdit combo = sender as ComboBoxEdit;
            int index = combo.SelectedIndex;

            switch (index)
            {
                case 0:
                    layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    gridControl1Table = null;
                    gridControl1.DataSource = null;
                    gridControl2Table = null;
                    gridControl3Table = null;
                    gridControl2.DataSource = null;
                    gridControl3.DataSource = null;
                    maNhaCC = null;
                    loadChart();
                    loadPieChart();
                    getVatTu();
                    calcVatTu();
                    loadGridControl1();

                    break;
                case 1:
                    layoutControlItem10.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    gridControl1Table = null;
                    gridControl1.DataSource = null;
                    gridControl2Table = null;
                    gridControl3Table = null;
                    gridControl2.DataSource = null;
                    gridControl3.DataSource = null;
                    maNhaCC = null;
                    loadChart();
                    loadPieChart();
                    loadSearchLookupEdit1();
                    break;
                default:
                    break;
            }
        }



        // nhà cung cấp
        private void loadSearchLookupEdit1()
        {
            getNCC();
            DataTable dt = nhacungcapTable;
            if (dt == null || dt.Columns.Count == 0 || dt.Rows.Count == 0) return;
            searchLookUpEdit1.Properties.DataSource = dt;
            searchLookUpEdit1.Properties.DisplayMember = "TenKH";
            searchLookUpEdit1.Properties.ValueMember = "MaNhaCC";
            searchLookUpEdit1.Properties.PopulateViewColumns();

            GridView view = searchLookUpEdit1.Properties.View;

            view.Columns["TenKH"].Caption = "Nhà cung cấp";
            view.Columns["DiaChi"].Caption = "Địa chỉ";
            view.Columns["SoDienThoai"].Caption = "Số điện thoại";

            view.Columns["MaNhaCC"].Visible = false;
            view.Columns["generatedID"].Visible = false;
            view.Columns["MaLoaiNCC"].Visible = false;

            view.OptionsFind.AlwaysVisible = true;
        }
        private void getNCC()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getnhacc";
            string urlGetListDataTable = URL + "CanDoiNPL/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            nhacungcapTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!nhacungcapTable.Columns.Contains("generatedID"))
                nhacungcapTable.Columns.Add("generatedID");
            foreach (DataRow row in vattuTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");
            }
        }
        private void getVatTuTheoNCC()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getvattusptheonhacc";
            request.Parameter = maNhaCC;
            string urlGetListDataTable = URL + "CanDoiNPL/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            vattuTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!vattuTable.Columns.Contains("generatedID"))
                vattuTable.Columns.Add("generatedID");
            if (!vattuTable.Columns.Contains("IsNPLText"))
                vattuTable.Columns.Add("IsNPLText", typeof(string));
            if (!vattuTable.Columns.Contains("Sort_ChungLoaiVatTu"))
                vattuTable.Columns.Add("Sort_ChungLoaiVatTu", typeof(string));
            foreach (DataRow row in vattuTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");

                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";
                row["Sort_ChungLoaiVatTu"] = padToThree(row["Sort"].ToString()) + "--" + row["ChungLoaiVatTu"].ToString();
            }
        }
        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            gridControl1Table = null;
            gridControl1.DataSource = null;
            gridControl2Table = null;
            gridControl3Table = null;
            gridControl2.DataSource = null;
            gridControl3.DataSource = null;
            loadChart();
            loadPieChart();
            maNhaCC = searchLookUpEdit1.EditValue.ToString();
            getVatTuTheoNCC();
            calcVatTu();
            loadGridControl1();
            
        }

        // danh sách vật tư
        private void loadGridControl1()
        {

            gridControl1.DataSource = gridControl1Table;
            foreach (GridColumn col in gridView1.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            gridView1.ExpandAllGroups();
        }
        private void getVatTu()
        {
            var request = XuLyVTRequestGet.createDefault();
            request.Action = "getvattusp";
            string urlGetListDataTable = URL + "CanDoiNPL/Get";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            vattuTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!vattuTable.Columns.Contains("generatedID"))
                vattuTable.Columns.Add("generatedID");
            if (!vattuTable.Columns.Contains("IsNPLText"))
                vattuTable.Columns.Add("IsNPLText", typeof(string));
            if (!vattuTable.Columns.Contains("Sort_ChungLoaiVatTu"))
                vattuTable.Columns.Add("Sort_ChungLoaiVatTu", typeof(string));
            foreach (DataRow row in vattuTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");

                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";
                row["Sort_ChungLoaiVatTu"] = padToThree(row["Sort"].ToString()) + "--" + row["ChungLoaiVatTu"].ToString();
            }
        }
        private void calcVatTu()
        {
            if (vattuTable == null || vattuTable.Columns.Count <= 0 || vattuTable.Rows.Count == 0) return;
            DataTable dt = vattuTable.Copy();
            foreach (DataRow row in dt.Rows)
            {
                row["TonKho"] = XuLyVTUnits.SmartTryParse<decimal>(row["TonKho"].ToString());
            }
            gridControl1Table = dt.Copy();
        }
        public string padToThree(string input)
        {
            // Nếu input null thì trả về rỗng
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Dùng PadLeft để thêm '0' cho đủ 3 ký tự
            return input.PadLeft(3, '0');
        }
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
            {
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
            }
        }
        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (info != null)
            {
                string originalText = info.GroupText;
                string[] parts = originalText.Split(new string[] { "--" }, StringSplitOptions.None);

                if (parts.Length > 1)
                {
                    info.GroupText = parts[1].Trim();
                }

                e.Painter.DrawObject(info);
                e.Handled = true;
            }
        }
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            gridControl2Table = null;
            gridControl3Table = null;
            gridControl2.DataSource = null;
            gridControl3.DataSource = null;

            getCanDoi();
            calcCanDoi();
            loadGridControl2();

            getMuaHang();
            calcMuaHang();
            loadGridControl3();

            loadChart();
            loadPieChart();
        }
        private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 192, 128)))
            {
                e.Graphics.FillRectangle(brush, e.Info.Bounds);
            }

            e.Graphics.DrawRectangle(Pens.Gray, e.Info.Bounds);

            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisWord,
                    FormatFlags = StringFormatFlags.LineLimit
                };

                e.Graphics.DrawString(e.Info.Caption, e.Appearance.Font, textBrush, e.Info.Bounds, sf);
            }

            e.Handled = true;
        }



        // lịch sử cân đối
        private void loadGridControl2()
        {

            gridControl2.DataSource = gridControl2Table;
            foreach (GridColumn col in gridView2.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            gridView2.ExpandAllGroups();
        }
        private void getCanDoi()
        {
            GridView view = gridView1;
            int focusedHandle = view.FocusedRowHandle;

            DataRow focusedRow;

            if (focusedHandle >= 0)
            {
                focusedRow = view.GetDataRow(focusedHandle);
            }
            else return;

            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "getlichsucandoitheovattu";
            DataRow newRow = request.TypeTable.NewRow();
            newRow["MaVTID"] = focusedRow["MaVTID"];
            newRow["MaNhomVT"] = focusedRow["MaNhom"];
            newRow["MaMauVT"] = focusedRow["MauVTID"];
            newRow["MaKhoVT"] = focusedRow["KhoVaiID"];
            request.TypeTable.Rows.Add(newRow);
            string urlGetListDataTable = URL + "CanDoiNPL/GetByTypeTable";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            candoiTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!candoiTable.Columns.Contains("generatedID"))
                candoiTable.Columns.Add("generatedID");
            if (!candoiTable.Columns.Contains("IsNPLText"))
                candoiTable.Columns.Add("IsNPLText", typeof(string));
            if (!candoiTable.Columns.Contains("Sort_ChungLoaiVatTu"))
                candoiTable.Columns.Add("Sort_ChungLoaiVatTu", typeof(string));
            foreach (DataRow row in candoiTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");

                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";

                row["Sort_ChungLoaiVatTu"] = padToThree(row["Sort"].ToString()) + "--" + row["ChungLoaiVatTu"].ToString();
            }
        }
        private void calcCanDoi()
        {
            if (candoiTable == null || candoiTable.Columns.Count <= 0 || candoiTable.Rows.Count == 0) return;
            DataTable dt = candoiTable.Copy();
            foreach (DataRow row in dt.Rows)
            {
                row["SLCanDoiKho"] = XuLyVTUnits.SmartTryParse<decimal>(row["SLCanDoiKho"].ToString());
            }
            gridControl2Table = dt.Copy();
        }
        private void gridView2_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 192, 128)))
            {
                e.Graphics.FillRectangle(brush, e.Info.Bounds);
            }

            e.Graphics.DrawRectangle(Pens.Gray, e.Info.Bounds);

            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisWord,
                    FormatFlags = StringFormatFlags.LineLimit
                };

                e.Graphics.DrawString(e.Info.Caption, e.Appearance.Font, textBrush, e.Info.Bounds, sf);
            }

            e.Handled = true;
        }


        // lịch sử mua hàng
        private void loadGridControl3()
        {

            gridControl3.DataSource = gridControl3Table;
            foreach (GridColumn col in gridView3.Columns)
            {
                col.OptionsColumn.AllowEdit = false;
            }
            gridView3.ExpandAllGroups();
        }
        private void getMuaHang()
        {
            GridView view = gridView1;
            int focusedHandle = view.FocusedRowHandle;

            DataRow focusedRow;

            if (focusedHandle >= 0)
            {
                focusedRow = view.GetDataRow(focusedHandle);
            }
            else return;

            var request = XuLyVTRequestPost.createDefault("CanDoiNPL");
            request.Action = "getlichsumuahangtheovattu";
            request.Parameter = maNhaCC;
            DataRow newRow = request.TypeTable.NewRow();
            newRow["MaVTID"] = focusedRow["MaVTID"];
            newRow["MaNhomVT"] = focusedRow["MaNhom"];
            newRow["MaMauVT"] = focusedRow["MauVTID"];
            newRow["MaKhoVT"] = focusedRow["KhoVaiID"];
            request.TypeTable.Rows.Add(newRow);
            string urlGetListDataTable = URL + "CanDoiNPL/GetByTypeTable";
            string jsonLstDataTable = Task.Run(async () => { return await _clientExtension.PostAsync(urlGetListDataTable, request); }).Result;
            muahangTable = JsonConvert.DeserializeObject<DataTable>(jsonLstDataTable);

            if (!muahangTable.Columns.Contains("generatedID"))
                muahangTable.Columns.Add("generatedID");
            if (!muahangTable.Columns.Contains("IsNPLText"))
                muahangTable.Columns.Add("IsNPLText", typeof(string));
            if (!muahangTable.Columns.Contains("Sort_ChungLoaiVatTu"))
                muahangTable.Columns.Add("Sort_ChungLoaiVatTu", typeof(string));
            if (!muahangTable.Columns.Contains("MuaTheo"))
                muahangTable.Columns.Add("MuaTheo", typeof(string));
            foreach (DataRow row in muahangTable.Rows)
            {
                row["generatedID"] = XuLyVTUnits.generatedTimeKey("id");

                if ((bool)row["IsNPL"] == true) row["IsNPLText"] = "Nguyên liệu";
                else row["IsNPLText"] = "Phụ liệu";

                row["Sort_ChungLoaiVatTu"] = padToThree(row["Sort"].ToString()) + "--" + row["ChungLoaiVatTu"].ToString();

                if (row["MaDH"] == DBNull.Value || row["MaDH"].ToString() == "")
                {
                    row["MuaTheo"] = "Bổ sung";
                    row["SoLuongMuaThem"] = XuLyVTUnits.SmartTryParse<decimal>(row["SLCungPO"].ToString());
                }
                else row["MuaTheo"] = "Đơn hàng";
            }
        }
        private void calcMuaHang()
        {
            if (muahangTable == null || muahangTable.Columns.Count <= 0 || muahangTable.Rows.Count == 0) return;
            DataTable dt = muahangTable.Copy();
            foreach (DataRow row in dt.Rows)
            {
                row["SoLuongMuaThem"] = XuLyVTUnits.SmartTryParse<decimal>(row["SoLuongMuaThem"].ToString());
                row["SLMuaThem"] = XuLyVTUnits.SmartTryParse<decimal>(row["SLMuaThem"].ToString());
                row["TyLeMuaThem"] = XuLyVTUnits.SmartTryParse<decimal>(row["TyLeMuaThem"].ToString());
            }
            gridControl3Table = dt.Copy();
        }
        private void gridView3_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 192, 128)))
            {
                e.Graphics.FillRectangle(brush, e.Info.Bounds);
            }

            e.Graphics.DrawRectangle(Pens.Gray, e.Info.Bounds);

            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisWord,
                    FormatFlags = StringFormatFlags.LineLimit
                };

                e.Graphics.DrawString(e.Info.Caption, e.Appearance.Font, textBrush, e.Info.Bounds, sf);
            }

            e.Handled = true;

        }


        // chart
        private void loadChart()
        {
            groupData();
            if (gridControl2Table == null || gridControl3Table == null)
            {
                chartControl1.Series.Clear();
                chartControl2.Series.Clear();
                return;
            }
            Series series1 = new Series("Sử dụng kho", ViewType.Bar);
            series1.DataSource = gridControl2Table;
            series1.ArgumentDataMember = "Ngay";
            series1.ValueDataMembers.AddRange(new string[] { "SLCanDoiKho" });
            series1.Label.TextPattern = "{V:n2}";
            series1.LabelsVisibility = (DevExpress.Utils.DefaultBoolean)DevExpress.XtraCharts.ChartElementVisibility.Visible;

            // Tạo Series 2
            Series series2 = new Series("Đã mua", ViewType.Bar);
            series2.DataSource = gridControl3Table;
            series2.ArgumentDataMember = "Ngay";
            series2.ValueDataMembers.AddRange(new string[] { "SoLuongMuaThem" });
            series2.Label.TextPattern = "{V:n2}";
            series2.LabelsVisibility = (DevExpress.Utils.DefaultBoolean)DevExpress.XtraCharts.ChartElementVisibility.Visible;

            // Thêm vào ChartControl
            chartControl1.Series.Clear();
            chartControl1.Series.Add(series1);
            chartControl1.Series.Add(series2);

            // Định dạng trục X là thời gian
            XYDiagram diagram = chartControl1.Diagram as XYDiagram;
            if (diagram != null)
            {
                diagram.PaneDistance = 50;
                // Trục X: ngày dd/MM/yyyy
                diagram.AxisX.Label.TextPattern = "{A}";
                diagram.AxisX.DateTimeScaleOptions.ScaleMode = ScaleMode.Continuous;
                diagram.AxisX.DateTimeOptions.Format = DateTimeFormat.Custom;
                diagram.AxisX.DateTimeOptions.FormatString = "dd/MM/yyyy";
                diagram.AxisX.Label.ResolveOverlappingOptions.AllowRotate = true;
                diagram.AxisX.Label.ResolveOverlappingOptions.AllowStagger = true;;

                // Trục Y: số lượng kiểu n2
                diagram.AxisY.Label.TextPattern = "{V:n2}";
            }
            chartControl1.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
            chartControl1.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.Right;
            chartControl1.Legend.AlignmentVertical = LegendAlignmentVertical.TopOutside;
            chartControl1.Legend.Direction = LegendDirection.LeftToRight;
        }
        private void loadPieChart()
        {
           
            if (gridControl2Table == null || gridControl2Table.Rows.Count == 0)
            {
                chartControl2.Series.Clear();
                return;
            }
            DateTime today = DateTime.Now.Date;
            string ngaytk = "01/01/2026";
            decimal tongCanDoi = 0;
            decimal tongMuaHang = 0;

            // Tính tổng đến ngày hiện tại từ string date
            foreach (DataRow row in gridControl2Table.Rows)
            {
                try
                {
                    DateTime ngay = DateTime.ParseExact(row["Ngay"].ToString(), "dd/MM/yyyy", null);
                    DateTime NgayTK = DateTime.ParseExact(ngaytk, "dd/MM/yyyy", null);
                    if (ngay.Date <= (NgayTK))
                    {
                        tongCanDoi += XuLyVTUnits.SmartTryParse<decimal>(row["SLCanDoiKho"].ToString());
                        tongMuaHang += XuLyVTUnits.SmartTryParse<decimal>(row["SoLuongMuaThem"].ToString());
                    }
                }
                catch { }
            }
            DataTable pieTable = new DataTable();
            pieTable.Columns.Add("Loai", typeof(string));
            pieTable.Columns.Add("SoLuong", typeof(decimal));

            if (tongCanDoi > 0)
                pieTable.Rows.Add("Cân đối", tongCanDoi);

            if (tongMuaHang > 0)
                pieTable.Rows.Add("Mua hàng", tongMuaHang);

            if (pieTable.Rows.Count == 0)
            {
                chartControl2.Series.Clear();
                return;
            }
            Series pieSeries = new Series("Tổng số lượng", ViewType.Pie3D);
            pieSeries.DataSource = pieTable;
            pieSeries.ArgumentDataMember = "Loai";
            pieSeries.ValueDataMembers.AddRange(new string[] { "SoLuong" });
            pieSeries.Label.TextPattern = "{A}: {V:n2}";
            pieSeries.LabelsVisibility = (DevExpress.Utils.DefaultBoolean)DevExpress.XtraCharts.ChartElementVisibility.Visible;

            chartControl2.Series.Clear();
            chartControl2.Series.Add(pieSeries);

            chartControl2.Legend.Visibility = DevExpress.Utils.DefaultBoolean.True;
            chartControl2.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.Right;
            chartControl2.Legend.AlignmentVertical = LegendAlignmentVertical.TopOutside;

            chartControl2.Titles.Clear();
            ChartTitle pieTitle = new ChartTitle();
            pieTitle.Text = "Tổng số lượng đến " + ngaytk;
            pieTitle.Font = new Font("Segoe UI",11, FontStyle.Bold);
            pieTitle.Alignment = StringAlignment.Center;
            chartControl2.Titles.Add(pieTitle);

            chartControl2.BackColor = Color.White;
        }
        private void groupData()
        {
            var groupCanDoi = new List<dynamic>();
            var groupMuaHang = new List<dynamic>();

            if (gridControl2Table != null && gridControl2Table.Rows.Count > 0)
            {
                groupCanDoi = gridControl2Table.AsEnumerable()
                    .GroupBy(row => ((DateTime)row["NgayTao"]).Date)
                    .Select(g => new
                    {
                        NgayTao = g.Key,
                        TongSLCanDoiKho = g.Sum(row =>
                            XuLyVTUnits.SmartTryParse<decimal>(row["SLCanDoiKho"].ToString()))
                    })
                    .OrderBy(x => x.NgayTao)
                    .ToList<dynamic>();
            }

            if (gridControl3Table != null && gridControl3Table.Rows.Count > 0)
            {
                groupMuaHang = gridControl3Table.AsEnumerable()
                .GroupBy(row => ((DateTime)row["NgayMua"]).Date)
                .Select(g => new
                {
                    NgayMua = g.Key,
                    TongSoLuongMuaThem = g.Sum(row =>
                        XuLyVTUnits.SmartTryParse<decimal>(row["SoLuongMuaThem"].ToString()))
                })
                .OrderBy(x => x.NgayMua)
                .ToList<dynamic>();
            }
            var allDates = new List<DateTime>();
            foreach(var item in groupCanDoi)
            {
                DateTime ngay = Convert.ToDateTime(item.NgayTao);
                if (!allDates.Contains(ngay))
                    allDates.Add(ngay);
            }
            foreach (var item in groupMuaHang)
            {
                DateTime ngay = Convert.ToDateTime(item.NgayMua);
                if (!allDates.Contains(ngay))
                    allDates.Add(ngay);
            }
            allDates.Sort();

            DataTable merTable = new DataTable();
            merTable.Columns.Add("Ngay", typeof(string));
            merTable.Columns.Add("SLCanDoiKho", typeof(decimal));
            merTable.Columns.Add("SoLuongMuaThem", typeof(decimal));
            foreach(var date in allDates)
            {
                var canDoiItem = groupCanDoi.FirstOrDefault(x => Convert.ToDateTime(x.NgayTao).Date == date);
                var muaHangItem = groupMuaHang.FirstOrDefault(x => Convert.ToDateTime(x.NgayMua).Date == date);

                decimal slCanDoi = canDoiItem != null ? canDoiItem.TongSLCanDoiKho : 0;
                decimal slMuaThem = muaHangItem != null ? muaHangItem.TongSoLuongMuaThem : 0;

                merTable.Rows.Add(date.ToString("dd/MM/yyyy"), slCanDoi, slMuaThem);
            }
            gridControl2Table = merTable;
            gridControl3Table = merTable;
        }

       
    }
}