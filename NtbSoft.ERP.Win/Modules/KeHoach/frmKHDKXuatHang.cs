using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmKHDKXuatHang : DevExpress.XtraEditors.XtraForm
    {
        #region Constants / Colors
        private static class Clr
        {
            public static readonly Color[] DotColors = new Color[]
            {
                Color.FromArgb(245, 245, 245),  Color.FromArgb(255, 255, 255), 
            };
            public static readonly Color[] DotColorDask = new Color[]
            {
            Color.FromArgb(220, 220, 220),  Color.FromArgb(240, 240, 240),  
            };
            public static readonly Color BandFixed = Color.FromArgb(255, 212, 128);
            public static readonly Color BandEditable = Color.FromArgb(255, 230, 100);
            public static readonly Color BandSizeParent = Color.FromArgb(180, 220, 255);
            public static readonly Color BandSizeChild = Color.FromArgb(200, 235, 255);
            public static readonly Color BandKHParent = Color.FromArgb(255, 200, 100);
            public static readonly Color BandKHChild = Color.FromArgb(255, 220, 120);
            public static readonly Color CellEditable = Color.FromArgb(255, 255, 210);
            public static readonly Color RowKH = Color.FromArgb(255, 255, 230);
            public static readonly Color IndicatorGOC = Color.FromArgb(220, 230, 255);
            public static readonly Color IndicatorKH = Color.FromArgb(255, 255, 220);
            public static readonly Color FooterBg = Color.FromArgb(255, 239, 204);
            public static readonly Color EditModeBg = Color.FromArgb(255, 255, 220);
        }

        private static readonly HashSet<string> _fixedCols = new HashSet<string>
            {
                "KHDKID","POID","MaDH","MaMau","TenMauHienThi","DauSize","DauSizeID","RowType",
                "PO","ColorCode","MaQG","MaBrand",
                "NgayGH","Dot","NgayDKGiao",
                "ThucXuat","NgayXuatThucTe","SoKien","GhiChu",
                "TongKH","TongSL","NgayGioKTDongHang"
            };
        #endregion

        #region Fields
        private readonly System.Configuration.AppSettingsReader _settingsReader =
            new System.Configuration.AppSettingsReader();
        private readonly HttpClientExtension _clientExtension = new HttpClientExtension();

        private readonly Dictionary<string, int> _dotColorIndex = new Dictionary<string, int>();
        private string _url = string.Empty;
        private string _currentMaDH = string.Empty;
        private bool _isEditMode = false;
        private bool _isSyncing = false;

        private DataTable _tblChiTiet = null;
        private DataView _dvChiTiet = null;
        private BandedGridView _rightView = null;
        private readonly HashSet<string> _expandedGroups = new HashSet<string>();
        #endregion

        #region Helpers
        private void RebuildDotColorrIndex()
        {
            _dotColorIndex.Clear();
            if (_tblChiTiet == null) return;
            var dots = _tblChiTiet.AsEnumerable()
                .Where(r => GetRowType(r) == "KH")
                .Select(r => r["Dot"]?.ToString() ?? "")
                .Where(d => !string.IsNullOrEmpty(d))
                .Distinct()
                .OrderBy(d => d)
                .ToList();
            for (int i = 0; i < dots.Count; i++)
                _dotColorIndex[dots[i]] = i % Clr.DotColors.Length;
        }

        private string GetGroupKey(DataRow r) =>
              $"{r["POID"]?.ToString().Trim().ToUpper()}|" +
              $"{r["MaMau"]?.ToString().Trim().ToUpper()}|" +
              $"{r["ColorCode"]?.ToString().Trim().ToUpper()}|" +
              $"{r["DauSize"]?.ToString().Trim()}";

        private static string GetRowType(DataRow r) => r["RowType"]?.ToString() ?? "";

        private string GetRowTypeByDvIdx(int i) =>
            (i >= 0 && _dvChiTiet != null && i < _dvChiTiet.Count)
                ? GetRowType(_dvChiTiet[i].Row) : "";

        private T RunSync<T>(Func<Task<T>> fn) => Task.Run(fn).Result;

        private static string GetSizeOnly(string s) =>
            s.StartsWith("SIZE_") ? s.Substring(5) : (s.Contains("_") ? s : s);

        private static int ParseInt(object v) =>
            int.TryParse(v?.ToString(), out int r) ? r : 0;

        private static bool TryParseDate(object v, out DateTime r)
        {
            r = DateTime.MinValue;
            if (v == null || v == DBNull.Value || string.IsNullOrEmpty(v.ToString())) return false;
            return DateTime.TryParse(v.ToString(), out r);
        }

        private static void ShowError(string prefix, Exception ex) =>
            XtraMessageBox.Show($"{prefix}: {ex.Message}", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Error);


        #endregion

        #region Init
        public frmKHDKXuatHang()
        {
            InitializeComponent();
            _url = (string)_settingsReader.GetValue("URL", typeof(string));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            barEditItem1.EditValue = null;
            barEditItem2.EditValue = null;
            gridViewDonHangTong.ColumnFilterChanged += (s, _) => _currentMaDH = string.Empty;
            LoadDSDonHang();
        }
        #endregion

        #region Load Data
        private bool TryGetBarDate(BarEditItem item, out DateTime result)
        {
            result = DateTime.MinValue;
            if (item.EditValue is DateTime dt) { result = dt; return true; }
            return item.EditValue != null && DateTime.TryParse(item.EditValue.ToString(), out result);
        }

        private void LoadDSDonHang()
        {
            try
            {
                string json = RunSync(() => _clientExtension.GetAsnyc(_url + "DonHangTong/GetALLDH"));
                if (string.IsNullOrEmpty(json)) return;

                var tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    gridDonHangTong.DataSource = null;
                    gridControlChiTietDonHang.DataSource = null;
                    return;
                }

                tbl = FilterByDateRange(tbl);
                gridDonHangTong.DataSource = tbl;
                gridViewDonHangTong.FocusedRowHandle = 0;
                _currentMaDH = string.Empty;

                if (tbl.Rows.Count > 0)
                {
                    _currentMaDH = gridViewDonHangTong.GetRowCellValue(0, colMaDH) as string ?? "";
                    LoadChiTiet();
                }
                else
                    gridControlChiTietDonHang.DataSource = null;
            }
            catch (Exception ex) { ShowError("Lỗi tải dữ liệu", ex); }
        }

        private DataTable FilterByDateRange(DataTable tbl)
        {
            if (!TryGetBarDate(barEditItem1, out DateTime from) ||
                !TryGetBarDate(barEditItem2, out DateTime to)) return tbl;

            string urlF = $"{_url}KHDKXuatHang/Get?maDH=&parameter={from:yyyy-MM-dd}&parameter2={to:yyyy-MM-dd}";
            string jsonF = RunSync(() => _clientExtension.GetAsnyc(urlF));
            if (string.IsNullOrEmpty(jsonF) || jsonF == "[]") return tbl.Clone();

            var tblMaDH = JsonConvert.DeserializeObject<DataTable>(jsonF);
            if (tblMaDH == null || tblMaDH.Rows.Count == 0) return tbl.Clone();

            var valid = new HashSet<string>(tblMaDH.AsEnumerable()
                .Select(r => r["MaDH"]?.ToString() ?? "").Where(s => !string.IsNullOrEmpty(s)));

            var rows = tbl.AsEnumerable()
                .Where(r => valid.Contains(r["MaDH"]?.ToString() ?? "")).ToList();

            return rows.Count > 0 ? rows.CopyToDataTable() : tbl.Clone();
        }

        private void LoadChiTiet()
        {
            try
            {
                string maDH = gridViewDonHangTong.GetFocusedRowCellValue(colMaDH) as string;
                if (string.IsNullOrEmpty(maDH)) return;

                string json = RunSync(() => _clientExtension.GetAsnyc(
                    $"{_url}KHDKXuatHang/Get?maDH={maDH}"));
                if (string.IsNullOrEmpty(json)) return;

                var tbl = JsonConvert.DeserializeObject<DataTable>(json);
                if (tbl == null || tbl.Rows.Count == 0)
                {
                    gridControlChiTietDonHang.DataSource = null;
                    gridControlKHoach.DataSource = null;
                    return;
                }

                PrepareTable(tbl);
                _tblChiTiet = tbl;
                _dvChiTiet = new DataView(_tblChiTiet);
                RecalcKHSizeCols();
                RebuildDotColorrIndex();

                // ── Left grid (GOC rows) ──────────────────────────────────
                var dvGoc = new DataView(_tblChiTiet) { RowFilter = "RowType = 'GOC'" };
                var leftBv = BuildLeftView(tbl);
                leftBv.OptionsBehavior.Editable = false;

                gridControlChiTietDonHang.ViewCollection.Clear();
                gridControlChiTietDonHang.ViewCollection.Add(leftBv);
                gridControlChiTietDonHang.MainView = leftBv;
                gridControlChiTietDonHang.DataSource = dvGoc;

                // ── Right grid (KH rows) ──────────────────────────────────
                var dvKH = new DataView(_tblChiTiet) { RowFilter = "RowType = 'KH'" };
                _rightView = BuildRightView(tbl, maDH);
                _rightView.OptionsBehavior.Editable = _isEditMode;

                if (_rightView.Columns["Dot"] != null)
                    _rightView.Columns["Dot"].GroupIndex = 0;

                _rightView.OptionsView.ShowGroupedColumns = false;
                _rightView.OptionsBehavior.AutoExpandAllGroups = true;

                gridControlKHoach.ViewCollection.Clear();
                gridControlKHoach.ViewCollection.Add(_rightView);
                gridControlKHoach.MainView = _rightView;
                gridControlKHoach.DataSource = dvKH;
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
        #endregion

        #region PrepareTable / RecalcKH
        private void PrepareTable(DataTable tbl)
        {
            foreach (DataColumn col in tbl.Columns.Cast<DataColumn>().ToList())
            {
                if (_fixedCols.Contains(col.ColumnName)) continue;
                if (col.ColumnName.StartsWith("KH@") || col.ColumnName.StartsWith("Size@")) continue;
                string n = "Size@" + col.ColumnName;
                if (!tbl.Columns.Contains(n)) col.ColumnName = n;
            }

            EnsureEditableColumns(tbl);

            if (tbl.Columns.Contains("NgayGioKTDongHang") && tbl.Columns.Contains("NgayXuatThucTe"))
                foreach (DataRow r in tbl.Rows)
                    if (GetRowType(r) == "GOC"
                        && (r["NgayXuatThucTe"] == DBNull.Value || string.IsNullOrEmpty(r["NgayXuatThucTe"]?.ToString()))
                        && r["NgayGioKTDongHang"] != DBNull.Value)
                        r["NgayXuatThucTe"] = r["NgayGioKTDongHang"];

            // Ensure TongSL column
            if (!tbl.Columns.Contains("TongSL")) tbl.Columns.Add("TongSL", typeof(int));
            foreach (DataRow row in tbl.Rows)
                row["TongSL"] = tbl.Columns.Cast<DataColumn>()
                    .Where(c => c.ColumnName.StartsWith("Size@"))
                    .Sum(c => ParseInt(row[c]));
        }

        private void EnsureEditableColumns(DataTable tbl)
        {
            var cols = new List<Tuple<string, Type>>
                {
                    Tuple.Create("KHDKID", typeof(int)),
                    Tuple.Create("Dot", typeof(string)),
                    Tuple.Create("NgayDKGiao", typeof(DateTime)),
                    Tuple.Create("ThucXuat", typeof(int)),
                    Tuple.Create("NgayXuatThucTe", typeof(DateTime)),
                    Tuple.Create("SoKien", typeof(int)),
                    Tuple.Create("GhiChu", typeof(string)),
                    Tuple.Create("MaBrand", typeof(string))
                };

            foreach (var item in cols)
            {
                string name = item.Item1;
                Type type = item.Item2;

                if (!tbl.Columns.Contains(name))
                    tbl.Columns.Add(name, type);
            }
        }

        private void RecalcKHSizeCols()
        {
            if (_tblChiTiet == null) return;

            var sizeCols = _tblChiTiet.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName.StartsWith("Size@")).ToList();
            var khCols = _tblChiTiet.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName.StartsWith("KH@")).ToList();

            foreach (string gk in _tblChiTiet.AsEnumerable()
                .Where(r => GetRowType(r) == "GOC").Select(r => GetGroupKey(r)).Distinct())
            {
                var gocRow = _tblChiTiet.AsEnumerable()
                    .FirstOrDefault(r => GetRowType(r) == "GOC" && GetGroupKey(r) == gk);
                if (gocRow == null) continue;

                var khRows = _tblChiTiet.AsEnumerable()
                    .Where(r => GetRowType(r) == "KH" && GetGroupKey(r) == gk).ToList();

                foreach (DataRow khRow in khRows)
                {
                    foreach (DataColumn sc in sizeCols)
                    {
                        string khField = "KH@" + GetSizeOnly(sc.ColumnName.Substring("Size@".Length));
                        if (!_tblChiTiet.Columns.Contains(khField)) continue;
                        int gocSL = ParseInt(gocRow[sc]);
                        int tongKH = khRows.Sum(r => ParseInt(r[khField]));
                        if (sc.ReadOnly) sc.ReadOnly = false;
                        khRow[sc] = Math.Max(0, gocSL - tongKH);
                    }
                    khRow["TongKH"] = khCols.Sum(c => ParseInt(khRow[c]));
                    khRow["TongSL"] = sizeCols.Sum(c => ParseInt(khRow[c]));
                }

                foreach (DataColumn kc in khCols)
                    gocRow[kc] = khRows.Sum(r => ParseInt(r[kc]));
                gocRow["TongKH"] = khCols.Sum(c => ParseInt(gocRow[c]));
            }
        }
        #endregion

        #region Build Views
        private BandedGridView BuildLeftView(DataTable tab)
        {
            var bv = new BandedGridView { IndicatorWidth = 25 };
            bv.OptionsView.ShowIndicator = false;
            bv.OptionsView.ShowColumnHeaders = false;
            bv.OptionsView.ShowGroupPanel = false;
            bv.OptionsView.ColumnAutoWidth = false;
            bv.OptionsView.ShowFooter = true;
            bv.OptionsView.AllowCellMerge = true;
            bv.OptionsBehavior.Editable = false;
            bv.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bv.OptionsNavigation.EnterMoveNextColumn = true;
            bv.OptionsSelection.MultiSelectMode =
                DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;

            if (tab.Columns.Count == 0) return bv;

            BuildFixedBands(bv, tab);
            if (bv.Columns["MaQG"] is BandedGridColumn maQG) maQG.OptionsColumn.AllowEdit = false;
            BuildSizeBand(bv, tab);

            //bv.RowStyle += BandedView_RowStyle;
            bv.CustomSummaryCalculate += LeftView_CustomSummaryCalculate;
            bv.CustomDrawBandHeader += BandedView_CustomDrawBandHeader;
            bv.CustomDrawFooter += BandedView_CustomDrawFooter;
            bv.CustomDrawFooterCell += BandedView_CustomDrawFooterCell;
            bv.CustomColumnDisplayText += BandedView_CustomColumnDisplayText;
            bv.RowCountChanged += BandedView_RowCountChanged;
            //AttachExpandEvents(bv);

            return bv;
        }

        private BandedGridView BuildRightView(DataTable tab, string maDH)
        {
            var bv = new BandedGridView();
            bv.OptionsView.ShowIndicator = false;
            bv.OptionsView.ShowColumnHeaders = false;
            bv.OptionsView.ShowGroupPanel = false;
            bv.OptionsView.ColumnAutoWidth = false;
            bv.OptionsView.ShowFooter = true;
            bv.OptionsView.AllowCellMerge = true;
            bv.OptionsBehavior.Editable = _isEditMode;

            if (tab.Columns.Count == 0) return bv;

            // Fixed left columns
            if (tab.Columns.Contains("PO"))
            {
                var b = MakeBand("PO", Clr.BandFixed, 9F); b.Fixed = FixedStyle.Left;
                AddColToBand(bv, b, "PO", "PO", 80).OptionsColumn.AllowEdit = false;
            }
            if (tab.Columns.Contains("ColorCode"))
            {
                var b = MakeBand("Color Code", Clr.BandFixed, 9F); b.Fixed = FixedStyle.Left;
                var c = AddColToBand(bv, b, "ColorCode", "Color Code", 80);
                c.OptionsColumn.AllowEdit = false;
                c.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
            }
            if (tab.Columns.Contains("TenMauHienThi"))
            {
                var b = MakeBand("Màu", Clr.BandFixed, 9F); b.Fixed = FixedStyle.Left;
                var c = AddColToBand(bv, b, "TenMauHienThi", "Màu", 100);
                c.OptionsColumn.AllowEdit = false;
                c.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
            }
            if (tab.Columns.Contains("MaMau"))
            {
                var c = (BandedGridColumn)bv.Columns.AddField("MaMau");
                c.Visible = false;
            }
            if (tab.Columns.Contains("DauSize"))
            {
                var b = MakeBand("InSeam", Clr.BandFixed, 9F); b.Fixed = FixedStyle.Left;
                var c = AddColToBand(bv, b, "DauSize", "InSeam", 70);
                c.OptionsColumn.AllowEdit = false;
                c.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
            }

            if (tab.Columns.Contains("MaBrand"))
            {
                var b = MakeBand("Brand", Clr.BandEditable, 9F);
                var c = AddColToBand(bv, b, "MaBrand", "Brand", 80);
                c.OptionsColumn.AllowEdit = true;
                c.AppearanceCell.BackColor = Clr.CellEditable;
                c.AppearanceCell.Options.UseBackColor = true;
            }

            BuildEditableCols(bv);
            BuildBrandLookup(bv, maDH);
            BuildKHBand(bv, tab);

            if (bv.Columns["Dot"] != null) bv.Columns["Dot"].GroupIndex = 0;
            bv.OptionsView.ShowGroupedColumns = false;
            bv.OptionsBehavior.AutoExpandAllGroups = true;
            AddGroupSummaries(bv, tab);
            bv.CustomColumnDisplayText += (s, ev) =>
            {
                string fn = ev.Column.FieldName;
                if (fn != "NgayDKGiao" && fn != "NgayXuatThucTe") return;
                if (ev.Value == null || ev.Value == DBNull.Value) return;
                if (ev.Value is DateTime dt && dt > DateTime.MinValue)
                    ev.DisplayText = dt.ToString("dd/MM/yyyy");
                else if (DateTime.TryParse(ev.Value.ToString(), out DateTime dt2) && dt2 > DateTime.MinValue)
                    ev.DisplayText = dt2.ToString("dd/MM/yyyy");
            };
            bv.CellValueChanged += RightView_CellValueChanged;
            bv.RowStyle += (s, ev) =>
            {
                if (ev.RowHandle < 0) return;

                var dv = gridControlKHoach.DataSource as DataView;
                int idx = (s as BandedGridView)?.GetDataSourceRowIndex(ev.RowHandle) ?? -1;
                if (dv == null || idx < 0 || idx >= dv.Count) return;

                DataRow row = dv[idx].Row;
                string dot = row["Dot"]?.ToString() ?? "";

                if (!_dotColorIndex.TryGetValue(dot, out int ci)) return;

             
                Color rowColor = (ci % 2 == 0)
                    ? Color.FromArgb(225, 225, 225)   
                    : Color.FromArgb(255, 255, 255);  

                ev.Appearance.BackColor = rowColor;
                ev.HighPriority = true;
            };

            bv.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
            bv.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;

            bv.Appearance.VertLine.BackColor = Color.FromArgb(160, 160, 160);
            bv.Appearance.VertLine.Options.UseBackColor = true;
            bv.Appearance.HorzLine.BackColor = Color.FromArgb(160, 160, 160);
            bv.Appearance.HorzLine.Options.UseBackColor = true;

            bv.CustomSummaryCalculate += RightView_CustomSummaryCalculate;
            bv.CustomDrawFooter += BandedView_CustomDrawFooter;
            bv.CustomDrawFooterCell += RightView_CustomDrawFooterCell;
            bv.CustomDrawGroupRow += RightView_CustomDrawGroupRow;
            return bv;

        }
        #endregion

        #region Build Bands / Columns
        private void BuildFixedBands(BandedGridView bv, DataTable tab)
        {
            var defs = new List<FixedCol>
            {
                new FixedCol("MaDH",           "Đơn hàng",       true,  true,  false, 0),
                new FixedCol("POID",           "POID",            true,  true,  false, 0),
                new FixedCol("MaMau",          "MaMau",           true,  true,  false, 0),  
                new FixedCol("PO",             "PO",              false, true,  false, 80),
                new FixedCol("ColorCode",      "Color Code",      false, true,  false, 80),
                new FixedCol("TenMauHienThi",  "Màu",             false, true,  false, 90),  
                new FixedCol("DauSizeID",      "InSeam ID",       true,  true,  false, 0),
                new FixedCol("DauSize",        "InSeam",          false, true,  false, 70),
                new FixedCol("MaQG",           "Quốc gia",        false, false, false, 70),
                new FixedCol("NgayGH",         "Ngày giao hàng",  false, true,  true,  120),
            };

            foreach (var def in defs)
            {
                if (!tab.Columns.Contains(def.Field)) continue;

                var band = MakeBand(def.Caption, Clr.BandFixed, 9F);
                band.Fixed = FixedStyle.Left;
                band.Visible = !def.Hidden;

                var col = AddColToBand(bv, band, def.Field, def.Caption, def.W > 0 ? def.W : 60);
                col.OptionsColumn.AllowEdit = !def.RO;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;

                if (def.Date) ApplyDateFormat(col);
            }
        }

        private void BuildEditableCols(BandedGridView bv)
        {
            AddEditableCol(bv, "Dot", "Đợt", 50, false);
            AddEditableCol(bv, "NgayDKGiao", "KH ngày giao", 130, true);
            AddEditableCol(bv, "NgayXuatThucTe", "Ngày xuất thực tế", 150, true);
            AddEditableCol(bv, "ThucXuat", "Thực xuất", 100, false, hasSum: true);
            AddEditableCol(bv, "SoKien", "Số kiện", 70, false, hasSum: true);
            AddEditableCol(bv, "GhiChu", "Ghi chú", 150, false);
        }

        private void BuildSizeBand(BandedGridView bv, DataTable tab)
        {
            var cols = tab.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName.StartsWith("Size@")).ToList();
            if (cols.Count == 0) return;

            var parent = MakeBand("Số lượng", Clr.BandSizeParent, 9F);
            foreach (var sc in cols)
            {
                string cap = GetSizeOnly(sc.ColumnName.Substring("Size@".Length));
                var child = MakeBand(cap, Clr.BandSizeChild, 8F);
                var col = AddColToBand(bv, child, sc.ColumnName, cap, 55);
                col.OptionsColumn.AllowEdit = false;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                ApplyNumericFormat(col);
                col.Summary.Add(DevExpress.Data.SummaryItemType.Custom, sc.ColumnName, "{0:n0}");
                parent.Children.Add(child);
            }
            bv.Bands.Add(parent);
            AddSummaryCol(bv, "TongSL", "Tổng SL", Color.SteelBlue);
        }

        private void BuildKHBand(BandedGridView bv, DataTable tab)
        {
            var khCols = tab.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName.StartsWith("KH@"))
                .ToList();
            if (khCols.Count == 0) return;
            var sizeOrder = tab.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName.StartsWith("Size@"))
                .Select((c, i) => new {
                    SizeOnly = GetSizeOnly(c.ColumnName.Substring("Size@".Length)),
                    Index = i
                })
                .ToDictionary(x => x.SizeOnly, x => x.Index);

            khCols = khCols
                .OrderBy(c => {
                    string sizeOnly = GetSizeOnly(c.ColumnName.Substring("KH@".Length));
                    return sizeOrder.TryGetValue(sizeOnly, out int idx) ? idx : int.MaxValue;
                })
                .ToList();

            var parent = MakeBand("Kế hoạch số lượng giao", Clr.BandKHParent, 9F);
            foreach (var kc in khCols)
            {
                string cap = GetSizeOnly(kc.ColumnName.Substring("KH@".Length));
                var child = MakeBand(cap, Clr.BandKHChild, 8F);
                var col = AddColToBand(bv, child, kc.ColumnName, cap, 60);
                col.OptionsColumn.AllowEdit = true;
                col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                col.AppearanceCell.BackColor = Clr.CellEditable;
                col.AppearanceCell.Options.UseBackColor = true;
                ApplyNumericFormat(col);
                col.Summary.Add(DevExpress.Data.SummaryItemType.Custom, kc.ColumnName, "{0:n0}");
                parent.Children.Add(child);
            }
            bv.Bands.Add(parent);
            AddSummaryCol(bv, "TongKH", "Tổng KH", Color.DarkOrange);
        }

        private void BuildBrandLookup(BandedGridView bv, string maDH)
        {
            if (!(bv.Columns["MaBrand"] is BandedGridColumn col)) return;
            col.OptionsColumn.AllowEdit = true;

            var repo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit
            {
                NullText = "",
                DisplayMember = "TenBrand",
                ValueMember = "MaBrand",
                ShowHeader = false,
                ShowLines = false,
                SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoFilter
            };
            try
            {
                string j = RunSync(() => _clientExtension.GetAsnyc(
                    $"{_url}KHDKXuatHang/GetBrand?maDH={maDH}"));
                if (!string.IsNullOrEmpty(j))
                {
                    var t = JsonConvert.DeserializeObject<DataTable>(j);
                    if (t?.Rows.Count > 0)
                    {
                        repo.DataSource = t;
                        repo.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TenBrand", "Brand", 150));
                    }
                }
            }
            catch { }
            col.ColumnEdit = repo;

            bv.ShowingEditor += (s, ev) =>
            {
                var view = s as BandedGridView;
                string fn = view?.FocusedColumn?.FieldName;
                if (fn != "MaQG" && fn != "NgayXuatThucTe") return;
                string rt = GetRowTypeByDvIdx(view.GetDataSourceRowIndex(view.FocusedRowHandle));
                if (fn == "MaQG" && rt != "KH") ev.Cancel = true;
                if (fn == "NgayXuatThucTe" && rt == "GOC") ev.Cancel = true;
            };
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        private static GridBand MakeBand(string caption, Color bg, float fontSize)
        {
            var band = new GridBand { Caption = caption };
            band.AppearanceHeader.BackColor = bg;
            band.AppearanceHeader.Options.UseBackColor = true;
            band.AppearanceHeader.Font = new Font("Tahoma", fontSize, FontStyle.Bold);
            band.AppearanceHeader.Options.UseFont = true;
            band.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            return band;
        }

        private static BandedGridColumn AddColToBand(
            BandedGridView view, GridBand band, string field, string caption, int width)
        {
            var col = (BandedGridColumn)view.Columns.AddField(field);
            col.Caption = caption;
            col.Width = width;
            col.Visible = true;
            col.OwnerBand = band;
            band.Columns.Add(col);
            view.Bands.Add(band);
            return col;
        }

        private void AddEditableCol(BandedGridView view, string field, string caption,
            int width, bool isDate, bool hasSum = false)
        {
            var band = MakeBand(caption, Clr.BandEditable, 9F);
            band.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            var col = AddColToBand(view, band, field, caption, width);
            col.OptionsColumn.AllowEdit = true;
            col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            col.AppearanceCell.BackColor = Clr.CellEditable;
            col.AppearanceCell.Options.UseBackColor = true;
            if (isDate) ApplyDateFormat(col);
            if (hasSum) col.Summary.Add(DevExpress.Data.SummaryItemType.Custom, field, "{0:n0}");
        }

        private void AddGroupSummaries(BandedGridView bv, DataTable tab)
        {
            var khCols = tab.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName.StartsWith("KH@")).ToList();

            foreach (var kc in khCols)
            {
                bv.GroupSummary.Add(new DevExpress.XtraGrid.GridGroupSummaryItem(
                    DevExpress.Data.SummaryItemType.Sum,
                    kc.ColumnName,
                    bv.Columns[kc.ColumnName],
                    "{0:##,0}"));
            }

            if (bv.Columns["TongKH"] != null)
                bv.GroupSummary.Add(new DevExpress.XtraGrid.GridGroupSummaryItem(
                    DevExpress.Data.SummaryItemType.Sum,
                    "TongKH",
                    bv.Columns["TongKH"],
                    "{0:##,0}"));

            foreach (string fn in new[] { "ThucXuat", "SoKien" })
            {
                if (bv.Columns[fn] != null)
                    bv.GroupSummary.Add(new DevExpress.XtraGrid.GridGroupSummaryItem(
                        DevExpress.Data.SummaryItemType.Sum,
                        fn,
                        bv.Columns[fn],
                        "{0:##,0}"));
            }

            bv.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
        }

        private void AddSummaryCol(BandedGridView view, string field, string caption, Color color)
        {
            if (!_tblChiTiet.Columns.Contains(field))
                _tblChiTiet.Columns.Add(field, typeof(int));

            var col = AddColToBand(view, MakeBand(caption, color, 9F), field, caption, 80);
            col.OptionsColumn.AllowEdit = false;
            col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            col.AppearanceCell.ForeColor = color;
            col.AppearanceCell.Options.UseForeColor = true;
            col.AppearanceCell.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            col.AppearanceCell.Options.UseFont = true;
            ApplyNumericFormat(col);
            col.Summary.Add(DevExpress.Data.SummaryItemType.Custom, field, "{0:n0}");
        }

        private static void ApplyDateFormat(BandedGridColumn col)
        {
            col.DisplayFormat.FormatString = "dd/MM/yyyy";
            col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;

            var repo = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            repo.DisplayFormat.FormatString = "dd/MM/yyyy";
            repo.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            repo.EditFormat.FormatString = "dd/MM/yyyy";
            repo.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            repo.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTime;
            repo.Mask.EditMask = "dd/MM/yyyy";
            col.ColumnEdit = repo;
        }

        private static void ApplyNumericFormat(BandedGridColumn col)
        {
            col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            col.DisplayFormat.FormatString = "{0:##,0}";
        }
        #endregion

        #region Grid Events — Left (GOC)
        private void LeftView_CustomSummaryCalculate(object sender,
            DevExpress.Data.CustomSummaryEventArgs ev)
        {
            var view = sender as BandedGridView;
            if (ev.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
                ev.TotalValue = 0;
            else if (ev.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
            {
                if (ev.RowHandle < 0) return;
                if (GetRowTypeByDvIdx(view.GetDataSourceRowIndex(ev.RowHandle)) != "GOC") return;
                ev.TotalValue = Convert.ToInt32(ev.TotalValue) + ParseInt(ev.FieldValue);
            }
        }
        #endregion

        #region Grid Events — Right (KH)
        private void RightView_CellValueChanged(object sender, CellValueChangedEventArgs ev)
        {
            if (!ev.Column.FieldName.StartsWith("KH@")) return;
            var view = sender as BandedGridView;

            int idx = view.GetDataSourceRowIndex(ev.RowHandle);
            var dvKH = gridControlKHoach.DataSource as DataView;
            if (dvKH == null || idx < 0 || idx >= dvKH.Count) return;

            DataRow changedRow = dvKH[idx].Row;
            string gKey = GetGroupKey(changedRow);
            var gocRow = _tblChiTiet.AsEnumerable()
                .FirstOrDefault(r => GetRowType(r) == "GOC" && GetGroupKey(r) == gKey);
            if (gocRow == null) return;

            string khSuffix = ev.Column.FieldName.Substring("KH@".Length);
            string sizeField = _tblChiTiet.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName.StartsWith("Size@"))
                .Select(c => c.ColumnName)
                .FirstOrDefault(cn => GetSizeOnly(cn.Substring("Size@".Length)) == khSuffix);

            if (sizeField == null) return;

            int maxSL = ParseInt(gocRow[sizeField]);
            int tongOther = _tblChiTiet.AsEnumerable()
                .Where(r => GetRowType(r) == "KH" && GetGroupKey(r) == gKey && r != changedRow)
                .Sum(r => ParseInt(r[ev.Column.FieldName]));
            int newVal = ParseInt(ev.Value);

            if (tongOther + newVal > maxSL)
            {
                int allowed = Math.Max(0, maxSL - tongOther);
                changedRow[ev.Column.FieldName] = allowed;
                XtraMessageBox.Show(
                    $"Size {ev.Column.Caption}: Tổng KH không được vượt quá {maxSL}!\n" +
                    $"Đã giao các đợt khác: {tongOther} | Còn được nhập: {allowed}",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            RecalcKHSizeCols();
            view.RefreshData();
            (gridControlChiTietDonHang.MainView as BandedGridView)?.RefreshData();
        }

        private void RightView_CustomSummaryCalculate(object sender,
            DevExpress.Data.CustomSummaryEventArgs ev)
        {
            if (ev.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
            { ev.TotalValue = 0; return; }
            if (ev.SummaryProcess != DevExpress.Data.CustomSummaryProcess.Calculate || ev.RowHandle < 0) return;

            var view = sender as BandedGridView;
            int idx = view.GetDataSourceRowIndex(ev.RowHandle);
            var dv = gridControlKHoach.DataSource as DataView;
            if (dv == null || idx < 0 || idx >= dv.Count) return;
            if (GetRowType(dv[idx].Row) != "KH") return;
            ev.TotalValue = Convert.ToInt32(ev.TotalValue) + ParseInt(ev.FieldValue);
        }

        private void RightView_CustomDrawFooterCell(object sender,
            FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null || _tblChiTiet == null) return;
            string fn = e.Column.FieldName;


            if (fn == "PO" || fn == "MaMau")
            {
                if (fn == "TongKH")
                {
                    DrawFooter2Row(e,
                        "Đã KH", "Còn lại",
                        Color.FromArgb(255, 230, 150), Color.FromArgb(200, 255, 200),
                        Color.DarkOrange, Color.DarkGreen);
                    return;
                }
            }

            if (!fn.StartsWith("KH@") && fn != "TongKH") return;

            int tongGoc;
            if (fn.StartsWith("KH@"))
            {
                string sizeOnly = GetSizeOnly(fn.Substring("KH@".Length));
                string sf = _tblChiTiet.Columns.Cast<DataColumn>()
                    .Where(c => c.ColumnName.StartsWith("Size@"))
                    .Select(c => c.ColumnName)
                    .FirstOrDefault(cn => GetSizeOnly(cn.Substring("Size@".Length)) == sizeOnly);
                tongGoc = sf != null
                    ? _tblChiTiet.AsEnumerable().Where(r => GetRowType(r) == "GOC").Sum(r => ParseInt(r[sf]))
                    : 0;
            }
            else 
            {
                tongGoc = _tblChiTiet.AsEnumerable()
                    .Where(r => GetRowType(r) == "GOC").Sum(r => ParseInt(r["TongSL"]));
            }

            int tongKH = _tblChiTiet.AsEnumerable()
                .Where(r => GetRowType(r) == "KH").Sum(r => ParseInt(r[fn]));
            int conLai = tongGoc - tongKH;

            Color botBg = conLai <= 0 ? Color.FromArgb(255, 180, 180) : Color.FromArgb(200, 255, 200);
            Color botFg = conLai <= 0 ? Color.DarkRed : Color.DarkGreen;

            DrawFooter2Row(e,
                tongKH.ToString("##,0"), conLai.ToString("##,0"),
                Color.FromArgb(255, 230, 150), botBg,
                Color.DarkOrange, botFg);

            using (Pen pen = new Pen(Color.Gray, 1))
            {
            
                //e.Graphics.DrawRectangle(pen, e.Bounds);

                int midY = e.Bounds.Top + e.Bounds.Height / 2;
                e.Graphics.DrawLine(pen,
                    e.Bounds.Left, midY,
                    e.Bounds.Right, midY);
            }
        }

        private void RightView_CustomDrawGroupRow(object sender,
       DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            var view = sender as BandedGridView;
            if (e.RowHandle >= 0) return; 

            string dot = view?.GetGroupRowValue(e.RowHandle)?.ToString() ?? "";
            if (string.IsNullOrEmpty(dot)) return;

            if (!_dotColorIndex.TryGetValue(dot, out int ci)) return;


            Color groupColor = (ci % 2 == 0)
                ? Color.FromArgb(225, 225, 225)  
                : Color.FromArgb(255, 255, 255);  

            e.Appearance.BackColor = groupColor;
            e.Appearance.BackColor2 = groupColor;
            e.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            e.Appearance.Options.UseBackColor = true;
        }

        private static void DrawFooter2Row(FooterCellCustomDrawEventArgs e,
            string top, string bot, Color topBg, Color botBg, Color topFg, Color botFg)
        {
            int half = e.Bounds.Height / 2;
            var topRect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, half);
            var botRect = new Rectangle(e.Bounds.X, e.Bounds.Y + half, e.Bounds.Width, e.Bounds.Height - half);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var font = new Font("Tahoma", 8.5F, FontStyle.Bold);

            e.Graphics.FillRectangle(new SolidBrush(topBg), topRect);
            e.Graphics.DrawString(top, font, new SolidBrush(topFg), topRect, sf);
            e.Graphics.FillRectangle(new SolidBrush(botBg), botRect);
            e.Graphics.DrawString(bot, font, new SolidBrush(botFg), botRect, sf);
            e.Handled = true;
        }
        #endregion

        #region Grid Events — Shared
        private void BandedView_RowStyle(object sender, RowStyleEventArgs ev)
        {
            var view = sender as BandedGridView;
            if (ev.RowHandle < 0) return;
            if (GetRowTypeByDvIdx(view.GetDataSourceRowIndex(ev.RowHandle)) != "KH") return;
            ev.Appearance.BackColor = Clr.RowKH;
            ev.Appearance.ForeColor = Color.DarkBlue;
            ev.HighPriority = true;
        }

        private void BandedView_CustomDrawBandHeader(object sender,
            DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventArgs e)
        {
            if (e.Band == null) return;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Color c = e.Band.AppearanceHeader.Options.UseBackColor
                ? e.Band.AppearanceHeader.BackColor : Clr.BandFixed;
            var r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
            e.Graphics.FillRectangle(e.Cache.GetGradientBrush(e.Bounds, c, c,
                e.Band.AppearanceHeader.GradientMode), r);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            e.Handled = true;
        }

        private void BandedView_CustomDrawFooter(object sender,
            DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            e.Graphics.FillRectangle(new System.Drawing.Drawing2D.LinearGradientBrush(
                e.Bounds, Clr.FooterBg, Clr.FooterBg, 90f), e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void BandedView_CustomDrawFooterCell(object sender,
            FooterCellCustomDrawEventArgs e)
        { if (e.Column != null) e.Appearance.ForeColor = Color.Red; }

        private void BandedView_CustomColumnDisplayText(object sender,
            DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            string fn = e.Column.FieldName;
            bool empty = e.Value == null || string.IsNullOrEmpty(e.Value.ToString());
            if (fn == "DauSize") { if (empty) e.DisplayText = "0"; return; }
            if (empty) { e.DisplayText = "-"; return; }
            if (!fn.Contains("@")) return;
            if (ParseInt(e.Value) == 0)
                e.DisplayText = fn.StartsWith("Size@") ? "0" : "-";
        }

        private void BandedView_RowCountChanged(object sender, EventArgs e)
        {
            var view = sender as GridView;
            if (!view.GridControl.IsHandleCreated) return;
            using (var gr = Graphics.FromHwnd(view.GridControl.Handle))
            {
                var sz = gr.MeasureString(view.RowCount.ToString(), view.PaintAppearance.Row.GetFont());
                view.IndicatorWidth = (int)(sz.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
            }
        }
        #endregion

        #region Expand / Collapse (Left grid indicator)
        //private void AttachExpandEvents(BandedGridView bv)
        //{
        //    bv.CustomDrawRowIndicator += (s, ev) =>
        //    {
        //        try
        //        {
        //            if (!ev.Info.IsRowIndicator || ev.RowHandle < 0) return;
        //            var view = s as BandedGridView;
        //            int idx = view.GetDataSourceRowIndex(ev.RowHandle);
        //            if (idx < 0 || _dvChiTiet == null || idx >= _dvChiTiet.Count) return;

        //            DataRow row = _dvChiTiet[idx].Row;
        //            string rowType = GetRowType(row);

        //            if (rowType == "GOC")
        //            {
        //                bool expanded = _expandedGroups.Contains(GetGroupKey(row));
        //                ev.Graphics.FillRectangle(new SolidBrush(Clr.IndicatorGOC), ev.Bounds);
        //                using (var font = new Font("Times New Roman", 9F, FontStyle.Bold))
        //                using (var brush = new SolidBrush(expanded
        //                    ? Color.FromArgb(140, 160, 190) : Color.FromArgb(0, 90, 180)))
        //                {
        //                    ev.Graphics.DrawString(expanded ? "▼" : "▶", font, brush, ev.Bounds,
        //                        new StringFormat
        //                        {
        //                            Alignment = StringAlignment.Center,
        //                            LineAlignment = StringAlignment.Center
        //                        });
        //                }
        //                ev.Handled = true;
        //            }
        //            else if (rowType == "KH")
        //            {
        //                ev.Graphics.FillRectangle(new SolidBrush(Clr.IndicatorKH), ev.Bounds);
        //                using (var pen = new Pen(Color.SteelBlue, 2f)
        //                {
        //                    StartCap = System.Drawing.Drawing2D.LineCap.Round,
        //                    EndCap = System.Drawing.Drawing2D.LineCap.Round,
        //                    LineJoin = System.Drawing.Drawing2D.LineJoin.Round
        //                })
        //                {
        //                    int cx = ev.Bounds.Left + ev.Bounds.Width / 2;
        //                    int cy = ev.Bounds.Top + ev.Bounds.Height / 2;
        //                    ev.Graphics.DrawLine(pen, cx - 3, ev.Bounds.Top + 4, cx - 3, cy);
        //                    ev.Graphics.DrawLine(pen, cx - 3, cy, cx + 4, cy);
        //                    ev.Graphics.DrawLine(pen, cx + 1, cy - 4, cx + 5, cy);
        //                    ev.Graphics.DrawLine(pen, cx + 1, cy + 4, cx + 5, cy);
        //                }
        //                ev.Handled = true;
        //            }
        //        }
        //        catch { }
        //    };

        //    bv.MouseDown += (s, ev) =>
        //    {
        //        if (ev.Button != MouseButtons.Left) return;
        //        var view = s as BandedGridView;
        //        var hitInfo = view.CalcHitInfo(ev.X, ev.Y);
        //        if (ev.X > view.IndicatorWidth || hitInfo.RowHandle < 0) return;

        //        int idx = view.GetDataSourceRowIndex(hitInfo.RowHandle);
        //        if (idx < 0 || _dvChiTiet == null || idx >= _dvChiTiet.Count) return;
        //        DataRow row = _dvChiTiet[idx].Row;
        //        if (GetRowType(row) != "GOC") return;

        //        string key = GetGroupKey(row);
        //        if (_expandedGroups.Contains(key)) _expandedGroups.Remove(key);
        //        else _expandedGroups.Add(key);
        //        view.FocusedRowHandle = hitInfo.RowHandle;
        //    };
        //}
        #endregion

        #region DonHangTong grid events
        private void gridViewDonHangTong_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0) return;
            if (e.FocusedRowHandle == DevExpress.XtraGrid.GridControl.AutoFilterRowHandle) return;
            if (gridViewDonHangTong.ActiveEditor != null) return;

            string maDH = gridViewDonHangTong.GetRowCellValue(e.FocusedRowHandle, colMaDH) as string ?? "";
            if (string.IsNullOrEmpty(maDH) || maDH == _currentMaDH) return;
            _currentMaDH = maDH;
            LoadChiTiet();
        }

        private void gridViewDonHangTong_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                var view = sender as GridView;
                if (e.RowHandle < 0 || view.GetDataRow(e.RowHandle) == null) return;
                string slcl = view.GetRowCellDisplayText(e.RowHandle, view.Columns["SLCL"]);
                string slth = view.GetRowCellDisplayText(e.RowHandle, view.Columns["SLTH"]);
                if (slcl == "0")
                { e.Appearance.BackColor = ColorTranslator.FromHtml("#84e184"); e.HighPriority = true; }
                else if (slth != "0")
                { e.Appearance.BackColor = ColorTranslator.FromHtml("#ffff99"); e.HighPriority = true; }
                if (e.RowHandle == view.FocusedRowHandle)
                { e.Appearance.BackColor = ColorTranslator.FromHtml("#dddddd"); e.Appearance.FontStyleDelta = FontStyle.Bold; e.HighPriority = true; }
            }
            catch { }
        }

        private void gridViewDonHangTong_CustomDrawColumnHeader(object sender,
            DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            var r = new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2);
            e.Graphics.FillRectangle(e.Cache.GetGradientBrush(e.Bounds, Clr.BandFixed, Clr.BandFixed,
                e.Column.AppearanceHeader.GradientMode), r);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            e.Handled = true;
        }

        private void gridViewDonHangTong_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            try
            {
                var view = (GridView)sender;
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {
                    string txt = (e.RowHandle + 1).ToString();
                    int nNew = (int)e.Info.Graphics.MeasureString(txt, e.Info.Appearance.Font).Width
                                  + GridPainter.Indicator.ImageSize.Width + 15;
                    if (view.IndicatorWidth < nNew) view.IndicatorWidth = nNew;
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = txt;
                }
                else if (e.RowHandle == DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                {
                    int nNew = (int)e.Info.Graphics.MeasureString("STT", e.Info.Appearance.Font).Width
                               + GridPainter.Indicator.ImageSize.Width + 20;
                    if (view.IndicatorWidth < nNew) view.IndicatorWidth = nNew;
                    e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    e.Info.DisplayText = "";
                }
            }
            catch { }
        }

        private void gridViewDonHangTong_RowCountChanged(object sender, EventArgs e)
        {
            var view = (GridView)sender;
            if (!view.GridControl.IsHandleCreated) return;
            using (var gr = Graphics.FromHwnd(view.GridControl.Handle))
            {
                var sz = gr.MeasureString(view.RowCount.ToString(), view.PaintAppearance.Row.GetFont());
                view.IndicatorWidth = (int)(sz.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
            }
        }
        #endregion

        #region Edit Mode / Save / Delete / Add
        private void SetEditMode(bool isEdit)
        {
            _isEditMode = isEdit;
            if (gridControlChiTietDonHang.MainView is BandedGridView leftBv)
                leftBv.OptionsBehavior.Editable = false;
            if (_rightView != null) _rightView.OptionsBehavior.Editable = isEdit;
            gridControlKHoach.BackColor = isEdit ? Clr.EditModeBg : SystemColors.Control;
            gridControlChiTietDonHang.BackColor = SystemColors.Control;
        }

        private void LuuChiTiet()
        {
            try
            {
                if (_tblChiTiet == null || _tblChiTiet.Rows.Count == 0) return;
                (gridControlChiTietDonHang.MainView as BandedGridView)?.CloseEditor();
                (gridControlChiTietDonHang.MainView as BandedGridView)?.UpdateCurrentRow();
                _rightView?.CloseEditor();
                _rightView?.UpdateCurrentRow();

                string maDH = _currentMaDH;
                if (string.IsNullOrEmpty(maDH))
                    maDH = gridViewDonHangTong.GetFocusedRowCellValue(colMaDH) as string ?? "";

                if (string.IsNullOrEmpty(maDH))
                {
                    XtraMessageBox.Show("Không xác định được đơn hàng!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var khCols = _tblChiTiet.Columns.Cast<DataColumn>()
                    .Where(c => c.ColumnName.StartsWith("KH@")).ToList();
                var tblSave = CreateSaveTable();

                foreach (DataRowView drv in _dvChiTiet)
                {
                    DataRow row = drv.Row;
                    if (GetRowType(row) != "KH") continue;
                    if (!TryParseDate(row["NgayDKGiao"], out DateTime ngayDK)) continue;

                    int khdkid = ParseInt(row["KHDKID"]);
                    string poid = row["POID"]?.ToString() ?? "";
                    string dauSize = row["DauSize"]?.ToString() ?? "";
                    string dot = row["Dot"]?.ToString() ?? "";
                    string ghiChu = row["GhiChu"]?.ToString() ?? "";
                    int thucXuat = ParseInt(row["ThucXuat"]);
                    int soKien = ParseInt(row["SoKien"]);
                    object ngayXT = row["NgayXuatThucTe"] == DBNull.Value
                                     ? (object)DBNull.Value : row["NgayXuatThucTe"];

                    string maMau = row["MaMau"]?.ToString() ?? "";
                    
                    // Lấy MaBrand
                    string maBrand = row["MaBrand"]?.ToString() ?? "";
                    if (khdkid > 0 && string.IsNullOrEmpty(maBrand))
                        maBrand = _tblChiTiet.AsEnumerable()
                            .FirstOrDefault(r => ParseInt(r["KHDKID"]) == khdkid)
                            ?["MaBrand"]?.ToString() ?? "";

                    foreach (DataColumn khCol in khCols)
                    {
                        int sl = ParseInt(row[khCol]);
                        if (sl == 0) continue;
                        string raw = khCol.ColumnName.Substring("KH@".Length); 
                        string sizeName = GetSizeOnly(raw);                   
                        string sizeID = "SIZE_" + sizeName;   
                        tblSave.Rows.Add(khdkid, poid, maDH, maMau, dauSize,
                            sizeID,     
                            sizeName, 
                            ngayDK, dot, sl.ToString(),
                            thucXuat, ngayXT, soKien, ghiChu, maBrand,
                            GlobleData.UserName, GlobleData.UserName);
                    }
                }

                if (tblSave.Rows.Count == 0)
                {
                    XtraMessageBox.Show("Không có dữ liệu để lưu!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string result = RunSync(() => _clientExtension.PostAsync($"{_url}KHDKXuatHang/Post?maDH={maDH}",  tblSave));
                if (result?.ToLower() == "true")
                {
                    XtraMessageBox.Show("Lưu thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadChiTiet();
                    SetEditMode(false);
                }
                else
                    XtraMessageBox.Show("Lưu thất bại: " + result, "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) { ShowError("Lỗi lưu", ex); }
        }

        private static DataTable CreateSaveTable()
        {
            DataTable t = new DataTable();

            var cols = new List<Tuple<string, Type>>
                    {
                        Tuple.Create("KHDKID", typeof(int)),
                        Tuple.Create("POID", typeof(string)),
                        Tuple.Create("MaDH", typeof(string)),
                        Tuple.Create("MaMau", typeof(string)),
                        Tuple.Create("DauSize", typeof(string)),
                        Tuple.Create("SizeID", typeof(string)),
                        Tuple.Create("Size", typeof(string)),
                        Tuple.Create("NgayDKGiao", typeof(DateTime)),
                        Tuple.Create("Dot", typeof(string)),
                        Tuple.Create("SL_Size", typeof(string)),
                        Tuple.Create("ThucXuat", typeof(int)),
                        Tuple.Create("NgayXuatThucTe", typeof(DateTime)),
                        Tuple.Create("SoKien", typeof(int)),
                        Tuple.Create("GhiChu", typeof(string)),
                        Tuple.Create("MaBrand", typeof(string)),
                        Tuple.Create("NguoiTao", typeof(string)),
                        Tuple.Create("NguoiSua", typeof(string))
                    };

            foreach (var item in cols)
            {
                t.Columns.Add(item.Item1, item.Item2);
            }
            return t;
        }

        private void ThemDotKHoach()
        {
            if (_tblChiTiet == null || _dvChiTiet == null) return;
            string maDH = gridViewDonHangTong.GetFocusedRowCellValue(colMaDH) as string ?? "";
            if (string.IsNullOrEmpty(maDH)) return;

            DataTable tblBrands = new DataTable();
            try
            {
                string j = RunSync(() => _clientExtension.GetAsnyc($"{_url}KHDKXuatHang/GetBrand?maDH={maDH}"));
                if (!string.IsNullOrEmpty(j))
                    tblBrands = JsonConvert.DeserializeObject<DataTable>(j) ?? new DataTable();
            }
            catch { }

            var sizeCols = _tblChiTiet.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName.StartsWith("Size@"))
                .Select(c => c.ColumnName.Substring("Size@".Length)).ToList();

            using (var frm = new frmNhapDotKHoach(maDH, _tblChiTiet, sizeCols, tblBrands))
            {
                frm.Text = "Nhập kế hoạch đợt xuất - " + maDH;
                frm.StartPosition = FormStartPosition.CenterParent;
                if (frm.ShowDialog(this) != DialogResult.OK || !frm.Saved) return;

                var inputTable = frm.ResultTable;
                if (inputTable == null || inputTable.Rows.Count == 0) return;

                foreach (DataRow inputRow in inputTable.Rows)
                {
                    string poid = inputRow["POID"]?.ToString() ?? "";
                    string maMau = inputRow["MaMau"]?.ToString() ?? "";
                    string dauSize = inputRow["DauSize"]?.ToString() ?? "";

                    var gocRow = _tblChiTiet.AsEnumerable()
                        .FirstOrDefault(r => r["RowType"]?.ToString() == "GOC"
                                          && r["POID"]?.ToString() == poid
                                          && r["MaMau"]?.ToString() == maMau
                                            && r["DauSize"]?.ToString() == dauSize);
                    if (gocRow == null) continue;

                    DataRow newRow = _tblChiTiet.NewRow();
                    foreach (DataColumn col in _tblChiTiet.Columns)
                        newRow[col.ColumnName] = gocRow[col.ColumnName];

                    newRow["KHDKID"] = 0;
                    newRow["RowType"] = "KH";
                    newRow["Dot"] = inputRow["Dot"]?.ToString() ?? "1";
                    newRow["MaBrand"] = inputRow.Table.Columns.Contains("MaBrand") ? inputRow["MaBrand"]?.ToString() ?? "" : "";
                    newRow["GhiChu"] = inputRow["GhiChu"]?.ToString() ?? "";
                    newRow["ThucXuat"] = DBNull.Value;
                    newRow["NgayXuatThucTe"] = DBNull.Value;
                    newRow["SoKien"] = DBNull.Value;
                    newRow["TongKH"] = 0;
                    newRow["NgayDKGiao"] = inputRow["NgayDKGiao"] != DBNull.Value
                        ? inputRow["NgayDKGiao"] : DBNull.Value;

                    foreach (DataColumn col in _tblChiTiet.Columns)
                    {
                        if (!col.ColumnName.StartsWith("KH@")) continue;
                        string sizeOnly = GetSizeOnly(col.ColumnName.Substring("KH@".Length));
                        string inputField = inputTable.Columns.Cast<DataColumn>()
                            .Where(c => c.ColumnName.StartsWith("KH@"))
                            .Select(c => c.ColumnName)
                            .FirstOrDefault(cn => GetSizeOnly(cn.Substring("KH@".Length)) == sizeOnly);
                        if (inputField != null) newRow[col.ColumnName] = ParseInt(inputRow[inputField]);
                    }

                    _tblChiTiet.Rows.Add(newRow);
                    _expandedGroups.Add(GetGroupKey(newRow));
                }

                RecalcKHSizeCols();
                SetEditMode(true);
                LuuChiTiet();
                _rightView?.RefreshData();
                (gridControlChiTietDonHang.MainView as BandedGridView)?.RefreshData();
            }
        }
        private void AutoLoadIfBothDates(object sender, EventArgs e)
        {
            try
            {
                DateTime from, to;

                if (TryGetBarDate(barEditItem1, out from) &&
                    TryGetBarDate(barEditItem2, out to))
                {
                    LoadDSDonHang();
                }
            }
            catch { }
        }
        private void XoaChiTiet()
        {
            try
            {
                if (!(gridControlChiTietDonHang.MainView is BandedGridView view)) return;
                int handle = view.FocusedRowHandle;
                if (handle < 0) return;

                int khdkid = ParseInt(view.GetFocusedRowCellValue("KHDKID"));
                if (khdkid == 0)
                {
                    int dvIdx = view.GetDataSourceRowIndex(handle);
                    if (dvIdx >= 0 && dvIdx < _dvChiTiet.Count)
                        _tblChiTiet.Rows.Remove(_dvChiTiet[dvIdx].Row);
                    return;
                }

                if (XtraMessageBox.Show("Bạn có chắc muốn xóa dòng kế hoạch này?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                string result = RunSync(() => _clientExtension.DeletedAsync(
                    $"{_url}KHDKXuatHang/Delete?id={khdkid}"));

                if (result?.ToLower() == "true") LoadChiTiet();
                else XtraMessageBox.Show("Xóa thất bại: " + result, "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) { ShowError("Lỗi xóa", ex); }
        }
        #endregion

        #region Button / Keyboard handlers
        private void btnThem_ItemClick(object sender, ItemClickEventArgs e) => ThemDotKHoach();
        private void btnLuu_ItemClick(object sender, ItemClickEventArgs e) => LuuChiTiet();
        private void btnXoa_ItemClick(object sender, ItemClickEventArgs e) => XoaChiTiet();
        private void Sua_ItemClick(object sender, ItemClickEventArgs e) => SetEditMode(true);
        private void btnFilter_ItemClick(object sender, ItemClickEventArgs e) => LoadDSDonHang();
        private void btnNapLai_ItemClick(object sender, ItemClickEventArgs e) { SetEditMode(false); LoadDSDonHang(); }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S)) { LuuChiTiet(); return true; }
            if (keyData == (Keys.Control | Keys.E)) { SetEditMode(true); return true; }
            if (keyData == Keys.F5) { LoadDSDonHang(); return true; }
            if (keyData == Keys.Delete) { XoaChiTiet(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region Inner types
        private class FixedCol
        {
            public string Field, Caption; public bool Hidden, RO, Date; public int W;
            public FixedCol(string f, string c, bool h, bool ro, bool d, int w)
            { Field = f; Caption = c; Hidden = h; RO = ro; Date = d; W = w; }
        }
        #endregion

        private void SafeMerge(OfficeOpenXml.ExcelWorksheet ws, int r1, int c1, int r2, int c2)
        {
            var toUnmerge = new HashSet<string>();
            for (int r = r1; r <= r2; r++)
                for (int c = c1; c <= c2; c++)
                {
                    string m = ws.MergedCells[r, c];
                    if (!string.IsNullOrEmpty(m)) toUnmerge.Add(m);
                }
            foreach (var addr in toUnmerge)
                try { ws.Cells[addr].Merge = false; } catch { }
            ws.Cells[r1, c1, r2, c2].Merge = true;
        }

        private void XuatExcel_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_rightView == null) return;
            using (var dlg = new SaveFileDialog
            {
                Filter = "Excel 2007+ (*.xlsx)|*.xlsx",
                FileName = $"KH_XuatHang_{_currentMaDH}_{DateTime.Now:yyyyMMdd}"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                // 1. Xuất ra file TẠM
                string tmpFile = System.IO.Path.GetTempFileName() + ".xlsx";
                var options = new DevExpress.XtraPrinting.XlsxExportOptions
                {
                    SheetName = "KH Xuất Hàng",
                    ShowGridLines = true,
                    TextExportMode = DevExpress.XtraPrinting.TextExportMode.Text,
                };
                _rightView.ExportToXlsx(tmpFile, options);
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                var dataRows = new List<List<string>>();
                int srcLastCol = 0;
                using (var srcPkg = new OfficeOpenXml.ExcelPackage(new System.IO.FileInfo(tmpFile)))
                {
                    var srcWs = srcPkg.Workbook.Worksheets["KH Xuất Hàng"];
                    srcLastCol = srcWs.Dimension.End.Column;
                    int srcLastRow = srcWs.Dimension.End.Row;
                    for (int r = 1; r <= srcLastRow; r++)
                    {
                        var rowData = new List<string>();
                        for (int c = 1; c <= srcLastCol; c++)
                            rowData.Add(srcWs.Cells[r, c].Text ?? "");
                        dataRows.Add(rowData);
                    }
                }
                try { System.IO.File.Delete(tmpFile); } catch { }

   
                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("KH Xuất Hàng");
                    ws.View.ShowGridLines = true;

                    int lastCol = srcLastCol;
                    const int HEADER_ROWS = 5;

                    ws.Row(1).Height = 18;
                    ws.Row(2).Height = 28;
                    ws.Row(3).Height = 18;
                    ws.Row(4).Height = 16;
                    ws.Row(5).Height = 8;

                    SafeMerge(ws, 1, 1, 4, 3);
                    ws.Cells[1, 1].Value = "VIKING VIETNAM CO., LTD";
                    ws.Cells[1, 1].Style.Font.Bold = true;
                    ws.Cells[1, 1].Style.Font.Size = 11;
                    ws.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    ws.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[1, 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                    var lbc = System.Drawing.Color.Black;
                    var lbs = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    for (int c = 1; c <= 3; c++)
                    {
                        ws.Cells[1, c].Style.Border.Top.Style = lbs;
                        ws.Cells[1, c].Style.Border.Top.Color.SetColor(lbc);
                        ws.Cells[4, c].Style.Border.Bottom.Style = lbs;
                        ws.Cells[4, c].Style.Border.Bottom.Color.SetColor(lbc);
                    }
                    for (int r = 1; r <= 4; r++)
                    {
                        ws.Cells[r, 1].Style.Border.Left.Style = lbs;
                        ws.Cells[r, 1].Style.Border.Left.Color.SetColor(lbc);
                        ws.Cells[r, 3].Style.Border.Right.Style = lbs;
                        ws.Cells[r, 3].Style.Border.Right.Color.SetColor(lbc);
                    }
                    var range = ws.Cells[1, 1, 4, lastCol];

                    range.Style.Border.Top.Style = lbs;
                    range.Style.Border.Bottom.Style = lbs;
                    range.Style.Border.Left.Style = lbs;
                    range.Style.Border.Right.Style = lbs;

                    range.Style.Border.Top.Color.SetColor(lbc);
                    range.Style.Border.Bottom.Color.SetColor(lbc);
                    range.Style.Border.Left.Color.SetColor(lbc);
                    range.Style.Border.Right.Color.SetColor(lbc);

                    SafeMerge(ws, 1, 4, 3, lastCol);
                    ws.Cells[1, 4].Value = "KE HOACH DANG KY XUAT HANG";
                 
                    ws.Cells[1, 4].Value = "KẾ HOẠCH ĐĂNG KÝ XUẤT HÀNG";
                    ws.Cells[1, 4].Style.Font.Bold = true;
                    ws.Cells[1, 4].Style.Font.Size = 18;
                    ws.Cells[1, 4].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    ws.Cells[1, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[1, 4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Cells[1, 4, 3, lastCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[1, 4, 3, lastCol].Style.Fill.BackgroundColor
                        .SetColor(System.Drawing.Color.FromArgb(255, 255, 255));

                    int mid = 4 + (lastCol - 4) / 2;
                    SafeMerge(ws, 4, 4, 4, mid);
                    ws.Cells[4, 4].Value = "Đơn hàng: " + _currentMaDH;
                    ws.Cells[4, 4].Style.Font.Bold = true;
                    ws.Cells[4, 4].Style.Font.Size = 10;
                    ws.Cells[4, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    ws.Cells[4, 4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Cells[4, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[4, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 255));

                    SafeMerge(ws, 4, mid + 1, 4, lastCol);
                    ws.Cells[4, mid + 1].Value = "Ngày xuất: " + DateTime.Now.ToString("dd/MM/yyyy");
                    ws.Cells[4, mid + 1].Style.Font.Italic = true;
                    ws.Cells[4, mid + 1].Style.Font.Size = 9;
                    ws.Cells[4, mid + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    ws.Cells[4, mid + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Cells[4, mid + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[4, mid + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 255));

                    // Logo
                    string logoPath = string.Empty;
                    try { logoPath = (string)_settingsReader.GetValue("LogoPath", typeof(string)); } catch { }
                    if (string.IsNullOrEmpty(logoPath) || !System.IO.File.Exists(logoPath))
                    {
                        string sd = System.IO.Path.GetFullPath(
                            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
                        logoPath = System.IO.Path.Combine(sd, @"NtbSoft.ERP.Web\Content\Image\LOGOVIKING.png");
                    }
                    if (!System.IO.File.Exists(logoPath))
                        logoPath = @"D:\viking\C.Ming_QLDH\start\NtbSoft.ERP.Web\Content\Image\LOGOVIKING.png";
                    if (System.IO.File.Exists(logoPath))
                    {
                        var pic = ws.Drawings.AddPicture("Logo", new System.IO.FileInfo(logoPath));
                        pic.SetPosition(0, 20, 0, 44);
                        pic.SetSize(100, 52);
                    }

                    for (int r = 0; r < dataRows.Count; r++)
                    {
                        int targetRow = r + HEADER_ROWS + 1;
                        for (int c = 0; c < dataRows[r].Count; c++)
                        {
                            string val = dataRows[r][c];
                            if (string.IsNullOrEmpty(val)) continue;
                            if (int.TryParse(val.Replace(",", "").Replace(".", ""), out int iv))
                                ws.Cells[targetRow, c + 1].Value = iv;
                            else
                                ws.Cells[targetRow, c + 1].Value = val;
                        }
                    }

              
                    int startCol = 11;
                    for (int c = startCol + 1; c <= lastCol - 1; c++)   
                        ws.Cells[6, c].Value = null;

                    SafeMerge(ws, 6, startCol, 6, lastCol - 1);        

                    ws.Cells[6, startCol].Value = "Kế hoạch số lượng giao";
                    ws.Cells[6, startCol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[6, startCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    var hBg1 = System.Drawing.Color.FromArgb(68, 114, 196);
                    var hBg2 = System.Drawing.Color.FromArgb(189, 215, 238);
                    var hFg1 = System.Drawing.Color.White;
                    var hFg2 = System.Drawing.Color.FromArgb(31, 56, 100);
                    var borderClr = System.Drawing.Color.FromArgb(50, 50, 50);   
                    var thinClr = System.Drawing.Color.FromArgb(180, 180, 180);

                    for (int col = 1; col <= lastCol; col++)
                    {
                        var c6 = ws.Cells[6, col];
                        c6.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        c6.Style.Fill.BackgroundColor.SetColor(hBg1);
                        c6.Style.Font.Bold = true;
                        c6.Style.Font.Color.SetColor(hFg1);
                        c6.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        c6.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        c6.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        c6.Style.Border.Top.Color.SetColor(borderClr);
                        c6.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        c6.Style.Border.Bottom.Color.SetColor(borderClr);
                        c6.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        c6.Style.Border.Left.Color.SetColor(borderClr);
                        c6.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        c6.Style.Border.Right.Color.SetColor(borderClr);

                        var c7 = ws.Cells[7, col];
                        c7.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        c7.Style.Fill.BackgroundColor.SetColor(hBg2);
                        c7.Style.Font.Bold = true;
                        c7.Style.Font.Color.SetColor(hFg2);
                        c7.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        c7.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        c7.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        c7.Style.Border.Bottom.Color.SetColor(borderClr);
                        c7.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        c7.Style.Border.Left.Color.SetColor(borderClr);
                        c7.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        c7.Style.Border.Right.Color.SetColor(borderClr);

                        var c8 = ws.Cells[8, col];
                        c8.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        c8.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                        c8.Style.Font.Italic = true;
                        c8.Style.Font.Size = 8;
                        c8.Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(128, 128, 128));
                        c8.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        c8.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(160, 160, 160));
                        c8.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        c8.Style.Border.Left.Color.SetColor(thinClr);
                        c8.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        c8.Style.Border.Right.Color.SetColor(thinClr);
                    }
                    ws.Row(5).Hidden = true;
                    ws.Row(6).Height = 22;
                    ws.Row(7).Height = 20;
                    ws.Row(8).Hidden = true;
                    //ws.Row(8).Height = 14;

                    for (int headerRow = 7; headerRow <= 7; headerRow++)
                    {
                        int mergeStart = 1;
                        string mergeVal = ws.Cells[headerRow, 1].Text ?? "";
                        for (int col = 2; col <= lastCol + 1; col++)
                        {
                            string colVal = col <= lastCol
                                ? (ws.Cells[headerRow, col].Text ?? "")
                                : "__END__";
                            if (colVal != mergeVal)
                            {
                                if (col - 1 > mergeStart)
                                    SafeMerge(ws, headerRow, mergeStart, headerRow, col - 1);
                                mergeStart = col;
                                mergeVal = colVal;
                            }
                        }
                    }

                    int totalRows = ws.Dimension?.End.Row ?? 0;
                    int colorToggle = 0;

                    var footerBg = System.Drawing.Color.FromArgb(255, 214, 132); 
                    var footerFg = System.Drawing.Color.FromArgb(80, 40, 0);      
                    var footerBdr = System.Drawing.Color.FromArgb(180, 100, 0);    

                    for (int row = 9; row <= totalRows; row++)
                    {
                        string cellVal = ws.Cells[row, 1].Text?.Trim() ?? "";
                        bool isFooter = (row == totalRows); 

                        if (isFooter)
                        {
                            for (int col = 1; col <= lastCol; col++)
                            {
                                var cell = ws.Cells[row, col];
                                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(footerBg);
                                cell.Style.Font.Bold = true;
                                cell.Style.Font.Color.SetColor(footerFg);
                                cell.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                                cell.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                                cell.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                cell.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                cell.Style.Border.Top.Color.SetColor(footerBdr);
                                cell.Style.Border.Bottom.Color.SetColor(footerBdr);
                                cell.Style.Border.Left.Color.SetColor(footerBdr);
                                cell.Style.Border.Right.Color.SetColor(footerBdr);
                            }
                            continue; 
                        }

                        System.Drawing.Color rowBg;
                        if (cellVal.StartsWith("Đợt"))
                        {
                            colorToggle++;
                            rowBg = (colorToggle % 2 == 0)
                                ? System.Drawing.Color.FromArgb(200, 200, 200)
                                : System.Drawing.Color.FromArgb(250, 250, 250);
                            ws.Cells[row, 1, row, lastCol].Style.Font.Bold = true;
                            ws.Cells[row, 1, row, lastCol].Style.Font.Color
                                .SetColor(System.Drawing.Color.FromArgb(50, 50, 50));
                        }
                        else
                        {
                            bool hasData = false;
                            for (int col = 1; col <= lastCol; col++)
                                if (!string.IsNullOrEmpty(ws.Cells[row, col].Text)) { hasData = true; break; }
                            rowBg = (!hasData || colorToggle == 0)
                                ? System.Drawing.Color.White
                                : (colorToggle % 2 == 0)
                                    ? System.Drawing.Color.FromArgb(210, 210, 210)
                                    : System.Drawing.Color.White;
                        }

                        for (int col = 1; col <= lastCol; col++)
                        {
                            var cell = ws.Cells[row, col];
                            cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(rowBg);
                            cell.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            cell.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            cell.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            cell.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            cell.Style.Border.Top.Color.SetColor(thinClr);
                            cell.Style.Border.Bottom.Color.SetColor(thinClr);
                            cell.Style.Border.Left.Color.SetColor(thinClr);
                            cell.Style.Border.Right.Color.SetColor(thinClr);
                        }
                    }

                    if (totalRows >= 6)
                    {
                        var outer = ws.Cells[6, 1, totalRows, lastCol];
                        outer.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        outer.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        outer.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        outer.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        outer.Style.Border.Top.Color.SetColor(borderClr);
                        outer.Style.Border.Bottom.Color.SetColor(borderClr);
                        outer.Style.Border.Left.Color.SetColor(borderClr);
                        outer.Style.Border.Right.Color.SetColor(borderClr);
                    }

                    ws.View.FreezePanes(9, 1);
                    package.SaveAs(new System.IO.FileInfo(dlg.FileName));
                }

                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo(dlg.FileName) { UseShellExecute = true });
            }
        }
    }
}