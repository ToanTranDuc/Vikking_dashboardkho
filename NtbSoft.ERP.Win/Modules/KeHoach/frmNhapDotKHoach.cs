using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmNhapDotKHoach : DevExpress.XtraEditors.XtraForm
    {
        private readonly HttpClientExtension _clientExtension = new HttpClientExtension();
        private readonly System.Configuration.AppSettingsReader _settingsReader =
            new System.Configuration.AppSettingsReader();
        private string _url = string.Empty;
        private string _maDH = string.Empty;
        private DataTable _tblChiTietGoc;
        private DataTable _tblData = null;
        private DataTable _tblBrands = new DataTable();
        private List<string> _sizeCols = new List<string>();
     

        public DataTable ResultTable
        {
            get
            {
                var rows = _tblData.AsEnumerable()
                    .Where(r => _tblData.Columns.Cast<DataColumn>()
                        .Where(c => c.ColumnName.StartsWith("KH@"))
                        .Any(c => ParseInt(r[c.ColumnName]) > 0))
                    .ToList();

                return rows.Count > 0
                    ? rows.CopyToDataTable()
                    : _tblData.Clone();
            }
        }

        private static int ParseInt(object val)
        { int v; return int.TryParse(val?.ToString(), out v) ? v : 0; }
        public bool Saved { get; private set; } = false;

        public frmNhapDotKHoach(string maDH, DataTable tblChiTiet, List<string> sizeCols, DataTable tblBrands = null)
        {
            InitializeComponent();
            _url = (string)_settingsReader.GetValue("URL", typeof(string));
            _maDH = maDH;
            _sizeCols = sizeCols ?? new List<string>();
            _tblChiTietGoc = tblChiTiet;

            // Dùng tblBrands được truyền vào, không cần gọi API nữa
            _tblBrands = tblBrands ?? new DataTable();
            if (!_tblBrands.Columns.Contains("MaBrand"))
                _tblBrands.Columns.Add("MaBrand", typeof(string));
            if (!_tblBrands.Columns.Contains("TenBrand"))
                _tblBrands.Columns.Add("TenBrand", typeof(string));
            BuildInputTable(tblChiTiet);
            BuildSummaryPanel();
        }

       

        private T RunSync<T>(Func<Task<T>> fn) => Task.Run(fn).Result;

        private void BuildInputTable(DataTable tblChiTiet)
        {
            _tblData = new DataTable();

            _tblData.Columns.Add("POID", typeof(string));
            _tblData.Columns.Add("PO", typeof(string));
            _tblData.Columns.Add("ColorCode", typeof(string));
            _tblData.Columns.Add("Mau", typeof(string));
            _tblData.Columns.Add("MaMau", typeof(string));
            _tblData.Columns.Add("DauSize", typeof(string));
            _tblData.Columns.Add("Dot", typeof(string));
            _tblData.Columns.Add("MaBrand", typeof(string));
            _tblData.Columns.Add("NgayDKGiao", typeof(DateTime));
            _tblData.Columns.Add("GhiChu", typeof(string));

            var sizeColNames = new List<string>();
            foreach (string sc in _sizeCols) 
            {
                string colName = "KH@" + sc;
                if (!_tblData.Columns.Contains(colName))
                {
                    _tblData.Columns.Add(colName, typeof(int));
                    sizeColNames.Add(colName);
                }
            }

            if (tblChiTiet == null) { BuildGrid(sizeColNames); return; }

            var gocRows = tblChiTiet.AsEnumerable()
                .Where(r => r["RowType"]?.ToString() == "GOC")
                .ToList();

            foreach (DataRow gr in gocRows)
            {
                string poid = gr["POID"]?.ToString() ?? "";
                string maMau = gr["MaMau"]?.ToString() ?? "";
                string dauSize = gr["DauSize"]?.ToString() ?? ""; 

                int nextDot = tblChiTiet.AsEnumerable()
                    .Where(r => r["RowType"]?.ToString() == "KH"
                             && r["POID"]?.ToString() == poid
                             && r["MaMau"]?.ToString() == maMau
                             && r["DauSize"]?.ToString() == dauSize) 
                    .Select(r => { int d; return int.TryParse(r["Dot"]?.ToString(), out d) ? d : 0; })
                    .DefaultIfEmpty(0).Max() + 1;

                DataRow nr = _tblData.NewRow();
                nr["POID"] = poid;
                nr["PO"] = gr["PO"]?.ToString() ?? "";
                nr["ColorCode"] = gr["ColorCode"]?.ToString() ?? "";
                nr["Mau"] = gr["MaMau"]?.ToString() ?? "";
                nr["MaMau"] = maMau;
                nr["DauSize"] = dauSize;
                nr["Dot"] = nextDot.ToString();

                var lastKH = tblChiTiet.AsEnumerable()
                    .Where(r => r["RowType"]?.ToString() == "KH"
                             && r["POID"]?.ToString() == poid
                             && r["MaMau"]?.ToString() == maMau
                             && r["DauSize"]?.ToString() == dauSize) 
                    .OrderByDescending(r => ParseInt(r["Dot"]))
                    .FirstOrDefault();

                nr["MaBrand"] = lastKH?["MaBrand"]?.ToString() ?? "";
                nr["NgayDKGiao"] = DBNull.Value;
                nr["GhiChu"] = "";

                foreach (string colName in sizeColNames)
                {
                    string sizeOnly = colName.Substring("KH@".Length);
                    string sizeField = tblChiTiet.Columns.Cast<DataColumn>()
                        .Where(c => c.ColumnName.StartsWith("Size@"))
                        .Select(c => c.ColumnName)
                        .FirstOrDefault(cn =>
                        {
                            string s = cn.Substring("Size@".Length);
                            string sh = s.Contains("_") ? s.Substring(s.LastIndexOf('_') + 1) : s;
                            return s == sizeOnly || sh == sizeOnly;
                        });

                    int gocSL = sizeField != null ? ParseInt(gr[sizeField]) : 0;

                    int daKH = tblChiTiet.AsEnumerable()
                        .Where(r => r["RowType"]?.ToString() == "KH"
                                 && r["POID"]?.ToString() == poid
                                 && r["MaMau"]?.ToString() == maMau
                                 && r["DauSize"]?.ToString() == dauSize
                                 && tblChiTiet.Columns.Contains(colName))
                        .Sum(r => ParseInt(r[colName]));

                    int conLai = Math.Max(0, gocSL - daKH);
                    nr[colName] = conLai; 
                }

                _tblData.Rows.Add(nr);
            }

            BuildGrid(sizeColNames);
        }
        private DataTable _tblSummary = new DataTable();
        private DevExpress.XtraGrid.GridControl _gridSummary;
        private void BuildSummaryPanel()
        {
            _tblSummary = new DataTable();
            _tblSummary.Columns.Add("Loại", typeof(string));

            foreach (string sc in _sizeCols)
            {
                string cap = sc.Contains("_") ? sc.Substring(sc.LastIndexOf('_') + 1) : sc;
                if (!_tblSummary.Columns.Contains(cap))
                    _tblSummary.Columns.Add(cap, typeof(int));
            }

            _tblSummary.Rows.Add(CreateSummaryRow("Gốc"));
            _tblSummary.Rows.Add(CreateSummaryRow("Đã kế hoạch giao hàng "));
            _tblSummary.Rows.Add(CreateSummaryRow("Còn lại"));

            _gridSummary = new DevExpress.XtraGrid.GridControl();
            _gridSummary.Dock = DockStyle.Bottom;
            _gridSummary.Height = 120;
            _gridSummary.DataSource = _tblSummary;

            var view = new GridView();
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsView.ShowIndicator = false;
            view.OptionsView.ColumnAutoWidth = false;
            view.OptionsBehavior.Editable = false;
            view.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;

            _gridSummary.ViewCollection.Add(view);
            _gridSummary.ViewCollection.Add(view);
            _gridSummary.MainView = view;
            view.PopulateColumns();

            var colLoai = view.Columns["Loại"];
            if (colLoai != null)
            {
                colLoai.Caption = "";
                colLoai.Width = 80;
                colLoai.AppearanceCell.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                colLoai.AppearanceCell.Options.UseFont = true;
                colLoai.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }

            foreach (DataColumn dc in _tblSummary.Columns)
            {
                if (dc.ColumnName == "Loại") continue;
                var col = view.Columns[dc.ColumnName];
                if (col == null) continue;
                col.Width = 55;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceCell.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                col.AppearanceCell.Options.UseFont = true;
                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                col.DisplayFormat.FormatString = "{0:##,0}";
            }

            view.RowStyle += (s, e) =>
            {
                if (e.RowHandle == 0)
                { e.Appearance.BackColor = Color.FromArgb(198, 224, 180); e.HighPriority = true; }
                else if (e.RowHandle == 1)
                { e.Appearance.BackColor = Color.FromArgb(255, 230, 150); e.HighPriority = true; }
                else if (e.RowHandle == 2)
                { e.Appearance.BackColor = Color.FromArgb(200, 255, 200); e.HighPriority = true; }
            };

            view.CustomDrawCell += (s, e) =>
            {
                if (e.RowHandle != 2 || e.Column.FieldName == "Loại") return;
                if (e.CellValue is int v && v == 0)
                {
                    e.Appearance.BackColor = Color.FromArgb(255, 180, 180);
                    e.Appearance.ForeColor = Color.DarkRed;
                }
            };

            _lblSummaryTitle = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 22,
                Font = new Font("Tahoma", 9F, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                BackColor = Color.FromArgb(220, 235, 255),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
                Text = "--- Chọn một dòng để xem chi tiết ---"
            };

            this.Controls.Add(_lblSummaryTitle);
            this.Controls.Add(_gridSummary);

            _tblData.ColumnChanged += (s, e) => RefreshSummary();
            _tblData.RowDeleted += (s, e) => RefreshSummary();

            var mainView = gridKH.MainView as GridView;
            if (mainView != null)
            {
                mainView.FocusedRowChanged += (s, e) =>
                {
                    var row = mainView.GetDataRow(e.FocusedRowHandle);
                    if (row == null) return;
                    string po = row["PO"]?.ToString() ?? "";
                    string maMau = row["MaMau"]?.ToString() ?? "";
                    string poid = row["POID"]?.ToString() ?? "";
                    string dauSize = row["DauSize"]?.ToString() ?? "";
                    _lblSummaryTitle.Text = $"📋  PO: {po}  |  Màu: {maMau}  |  InSeam:{dauSize}";
                    RefreshSummary(po, maMau, poid, dauSize);
                };
            }

            if (_tblData.Rows.Count > 0)
            {
                var first = _tblData.Rows[0];
                string po0 = first["PO"]?.ToString() ?? "";
                string maMau0 = first["MaMau"]?.ToString() ?? "";
                string poid0 = first["POID"]?.ToString() ?? "";
                string dauSize0 = first["DauSize"]?.ToString() ?? "";  
                _lblSummaryTitle.Text = $"📋  PO: {po0}  |  Màu: {maMau0}  |  InSeam: {dauSize0}";  
                RefreshSummary(po0, maMau0, poid0, dauSize0); 
            }
            else
            {
                RefreshSummary();
            }
        }


        private DataRow CreateSummaryRow(string label)
        {
            var r = _tblSummary.NewRow();
            r["Loại"] = label;
            foreach (DataColumn c in _tblSummary.Columns)
                if (c.ColumnName != "Loại") r[c.ColumnName] = 0;
            return r;
        }

        private Label _lblSummaryTitle;

        public void RefreshSummary(string po = "", string maMau = "", string poid = "", string dauSize ="")
        {
            if (_tblSummary == null || _tblChiTietGoc == null) return;
            var mainView = gridKH?.MainView as GridView;
            if (mainView != null && string.IsNullOrEmpty(po))
            {
                var row = mainView.GetDataRow(mainView.FocusedRowHandle);
                po = row?["PO"]?.ToString() ?? "";
                maMau = row?["MaMau"]?.ToString() ?? "";
                poid = row?["POID"]?.ToString() ?? "";
            }

            var gocRows = _tblChiTietGoc.AsEnumerable()
                .Where(r => r["RowType"]?.ToString() == "GOC"
                         && (string.IsNullOrEmpty(po) || r["PO"]?.ToString() == po)
                         && (string.IsNullOrEmpty(maMau) || r["MaMau"]?.ToString() == maMau)
                         && (string.IsNullOrEmpty(dauSize) || r["DauSize"]?.ToString() == dauSize))
                .ToList();

            var khRows = _tblChiTietGoc.AsEnumerable()
                .Where(r => r["RowType"]?.ToString() == "KH"
                         && (string.IsNullOrEmpty(poid) || r["POID"]?.ToString() == poid)
                         && (string.IsNullOrEmpty(maMau) || r["MaMau"]?.ToString() == maMau)
                         && (string.IsNullOrEmpty(dauSize) || r["DauSize"]?.ToString() == dauSize)) 
                .ToList();

            foreach (DataColumn col in _tblSummary.Columns)
            {
                if (col.ColumnName == "Loại") continue;
                string caption = col.ColumnName;

                string sizeField = FindField(_tblChiTietGoc, "Size@", caption);
                string khField = FindField(_tblChiTietGoc, "KH@", caption);
                string formField = FindField(_tblData, "KH@", caption);

                int goc = sizeField != null ? gocRows.Sum(r => ParseInt(r[sizeField])) : 0;
                int daKH = khField != null ? khRows.Sum(r => ParseInt(r[khField])) : 0;
                int trongForm = formField != null
                           ? _tblData.AsEnumerable()
                               .Where(r => (string.IsNullOrEmpty(po) || r["PO"]?.ToString() == po)
                                        && (string.IsNullOrEmpty(maMau) || r["MaMau"]?.ToString() == maMau)
                                        && (string.IsNullOrEmpty(dauSize) || r["DauSize"]?.ToString() == dauSize)) 
                               .Sum(r => ParseInt(r[formField]))
                           : 0;

                _tblSummary.Rows[0][caption] = goc;
                _tblSummary.Rows[1][caption] = daKH + trongForm;
                _tblSummary.Rows[2][caption] = goc - daKH - trongForm;
            }

            _gridSummary?.Refresh();
        }

        private string FindField(DataTable tbl, string prefix, string caption)
        {
            return tbl.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName.StartsWith(prefix))
                .Select(c => c.ColumnName)
                .FirstOrDefault(cn =>
                {
                    string s = cn.Substring(prefix.Length);
                    string sh = s.Contains("_") ? s.Substring(s.LastIndexOf('_') + 1) : s;
                    return sh == caption || s == caption;
                });
        }

        private void BuildGrid(List<string> sizeColNames)
        {
            gridKH.DataSource = null;
            gridKH.DataSource = _tblData;

            var view = gridKH.MainView as DevExpress.XtraGrid.Views.BandedGrid.BandedGridView;
            if (view == null) return;


            view.Columns.Clear();
            view.Bands.Clear();

            view.OptionsView.ShowGroupPanel = false;
            view.OptionsBehavior.Editable = true;
            view.OptionsView.ColumnAutoWidth = false;

            view.ValidatingEditor -= gridView1_ValidatingEditor;
            view.ValidatingEditor += gridView1_ValidatingEditor;

            var bandInfo = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            bandInfo.Caption = "Thông tin";
            bandInfo.OptionsBand.FixedWidth = true;
            bandInfo.RowCount = 2;
          
            var bandSize = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            bandSize.Caption = "SIZE";
            bandSize.RowCount = 2;
            view.Bands.Add(bandInfo);
            view.Bands.Add(bandSize);

            DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn AddCol(
                string field, string caption, int width, bool editable,
                bool isDate = false)
            {
                var col = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                col.FieldName = field;
                col.Caption = caption;
                col.Visible = true;
                col.Width = width;
                col.OptionsColumn.AllowEdit = editable;

                col.AppearanceCell.BackColor = editable
                    ? Color.FromArgb(255, 255, 210)
                    : Color.WhiteSmoke;
                col.AppearanceCell.Options.UseBackColor = true;

                if (isDate)
                {
                    col.DisplayFormat.FormatString = "dd/MM/yyyy";
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;

                    var repo = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
                    repo.DisplayFormat.FormatString = "dd/MM/yyyy";
                    repo.EditFormat.FormatString = "dd/MM/yyyy";
                    col.ColumnEdit = repo;
                }

                view.Columns.Add(col);
                return col;
            }

            bandInfo.Columns.Add(AddCol("PO", "PO", 50, false));
            bandInfo.Columns.Add(AddCol("ColorCode", "Color Code", 50, false));
            bandInfo.Columns.Add(AddCol("Mau", "Màu", 80, false));
            bandInfo.Columns.Add(AddCol("DauSize", "InSeam", 70, false));
            var colDot = AddCol("Dot", "Đợt", 25, true);
            colDot.MinWidth = 25;
            colDot.MaxWidth = 30;  
            bandInfo.Columns.Add(colDot);
            var repoLookup = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            repoLookup.DataSource = _tblBrands;
            repoLookup.ValueMember = "MaBrand";
            repoLookup.DisplayMember = "TenBrand";
            repoLookup.NullText = "";
            repoLookup.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
            var popupView = repoLookup.View;
            popupView.OptionsView.ShowGroupPanel = false;
            popupView.OptionsView.ShowIndicator = false;
            popupView.OptionsView.ColumnAutoWidth = true;
            popupView.Columns.Clear();

            var colTenBrand = new DevExpress.XtraGrid.Columns.GridColumn();
            colTenBrand.FieldName = "TenBrand";
            colTenBrand.Caption = "Tên Brand";
            colTenBrand.Visible = true;
            colTenBrand.Width = 180;
            popupView.Columns.Add(colTenBrand);

            var colBrand = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            colBrand.FieldName = "MaBrand";
            colBrand.Caption = "Brand";
            colBrand.Visible = true;
            colBrand.Width = 120;
            colBrand.OptionsColumn.AllowEdit = true;
            colBrand.AppearanceCell.BackColor = Color.FromArgb(255, 255, 210);
            colBrand.AppearanceCell.Options.UseBackColor = true;
            colBrand.ColumnEdit = repoLookup;

            view.Columns.Add(colBrand);
            bandInfo.Columns.Add(colBrand);
            bandInfo.Columns.Add(AddCol("NgayDKGiao", "Ngày giao hàng KH", 400, true, true));
            bandInfo.Columns.Add(AddCol("GhiChu", "Ghi chú", 150, true));

            foreach (string hideField in new[] { "POID", "MaMau" })
            {
                var col = view.Columns[hideField];
                if (col != null)
                {
                    col.Visible = false;
                    col.Width = 0;
                }
            }

            foreach (string colName in sizeColNames)
            {
                string raw = colName.Substring("KH@".Length);
                string caption = raw.Contains("_")
                    ? raw.Substring(raw.LastIndexOf('_') + 1)
                    : raw;

                var col = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                col.FieldName = colName;
                col.Caption = caption;
                col.Visible = true;
                col.Width = 70;
                col.OptionsColumn.AllowEdit = true;

                col.AppearanceCell.BackColor = Color.FromArgb(255, 230, 100);
                col.AppearanceCell.Options.UseBackColor = true;

                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                col.DisplayFormat.FormatString = "{0:##,0}";

                view.Columns.Add(col);
                bandSize.Columns.Add(col); 
            }
            view.RowStyle += (s, e) =>
            {
                if (e.RowHandle >= 0)
                    e.Appearance.BackColor = Color.FromArgb(255, 255, 230);
            };
            view.CellValueChanged += (s, e) =>
            {
                string field = e.Column.FieldName;
                if (field != "MaBrand" && field != "NgayDKGiao") return;

                var changedRow = (gridKH.MainView as GridView)?.GetDataRow(e.RowHandle);
                if (changedRow == null) return;

                object newValue = changedRow[field];
                string changedDot = changedRow["Dot"]?.ToString() ?? ""; 

                foreach (DataRow row in _tblData.Rows)
                {
                    if (row == changedRow) continue;

                    string rowDot = row["Dot"]?.ToString() ?? "";

                    if (field == "NgayDKGiao")
                    {
                        if (rowDot == changedDot)
                            row[field] = newValue;
                    }
                    else if (field == "MaBrand")
                    {
                        row[field] = newValue;
                    }
                }

    (gridKH.MainView as GridView)?.RefreshData();
                RefreshSummary();
            };

            bandSize.AppearanceHeader.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            bandSize.AppearanceHeader.Options.UseFont = true;
            bandSize.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandSize.AppearanceHeader.Options.UseTextOptions = true;
            bandInfo.AppearanceHeader.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            bandInfo.AppearanceHeader.Options.UseFont = true;
            bandInfo.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandInfo.AppearanceHeader.Options.UseTextOptions = true;
            view.Appearance.HeaderPanel.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            view.Appearance.HeaderPanel.Options.UseFont = true;
            view.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            view.Appearance.HeaderPanel.Options.UseTextOptions = true;
            //view.Appearance.Row.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            //view.Appearance.Row.Options.UseFont = true;
            //view.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //view.Appearance.Row.Options.UseTextOptions = true;
            view.Appearance.FocusedRow.BackColor = Color.FromArgb(255, 255, 200);
            view.Appearance.FocusedRow.Options.UseBackColor = true;
            bandInfo.OptionsBand.FixedWidth = true;
            bandSize.OptionsBand.FixedWidth = true;
        }

        private void HideBandCol(DevExpress.XtraGrid.Views.BandedGrid.BandedGridView view, string field)
        {
            var col = view.Columns[field];
            if (col != null)
                col.Visible = false;
        }
        private static void HideCol(GridView v, string field)
        {
            var c = v.Columns[field];
            if (c != null) c.Visible = false;
        }

        private static void SetCol(GridView v, string field, string caption, int width,
            bool editable, bool isDate = false, bool isNum = false)
        {
            var c = v.Columns[field];
            if (c == null) return;
            c.Caption = caption;
            c.Width = width;
            c.OptionsColumn.AllowEdit = editable;
            c.AppearanceCell.BackColor = editable
                ? Color.FromArgb(255, 255, 210) : Color.WhiteSmoke;
            c.AppearanceCell.Options.UseBackColor = true;

            if (isDate)
            {
                c.DisplayFormat.FormatString = "dd/MM/yyyy";
                c.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                var repo = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
                repo.DisplayFormat.FormatString = "dd/MM/yyyy";
                repo.EditFormat.FormatString = "dd/MM/yyyy";
                c.ColumnEdit = repo;
            }
            if (isNum)
            {
                c.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                c.DisplayFormat.FormatString = "{0:##,0}";
            }
        }

        private void AddRow()
        {
            var mainView = gridKH.MainView as GridView;
            DataRow sourceRow = null;
            int insertIndex = _tblData.Rows.Count; 

            if (mainView != null && mainView.FocusedRowHandle >= 0)
            {
                sourceRow = mainView.GetDataRow(mainView.FocusedRowHandle);
                insertIndex = mainView.FocusedRowHandle + 1;
            }

            DataRow nr = _tblData.NewRow();

            if (sourceRow != null)
            {
                foreach (DataColumn col in _tblData.Columns)
                    nr[col.ColumnName] = sourceRow[col.ColumnName];

                int currentDot = ParseInt(sourceRow["Dot"]);
                nr["Dot"] = (currentDot + 1).ToString();

                foreach (DataColumn col in _tblData.Columns)
                    if (col.ColumnName.StartsWith("KH@"))
                        nr[col.ColumnName] = 0;
                nr["NgayDKGiao"] = DBNull.Value;
                nr["GhiChu"] = "";
            }
            else
            {
                nr["Dot"] = "1";
                nr["NgayDKGiao"] = DBNull.Value;
                foreach (DataColumn col in _tblData.Columns)
                    if (col.ColumnName.StartsWith("KH@")) nr[col.ColumnName] = 0;
            }

            _tblData.Rows.InsertAt(nr, insertIndex);
            if (mainView != null)
            {
                mainView.RefreshData();
                mainView.FocusedRowHandle = insertIndex;
            }
        }

        private void Save()
        {
            try
            {
                (gridKH.MainView as GridView)?.CloseEditor();
                (gridKH.MainView as GridView)?.UpdateCurrentRow();

                var activeRows = _tblData.AsEnumerable()
                    .Where(r => _tblData.Columns.Cast<DataColumn>()
                        .Where(c => c.ColumnName.StartsWith("KH@"))
                        .Any(c => ParseInt(r[c.ColumnName]) > 0))
                    .ToList();

                if (activeRows.Count == 0)
                {
                    XtraMessageBox.Show("Vui lòng nhập số lượng kế hoạch!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var invalid = activeRows
                    .Where(r => r["NgayDKGiao"] == DBNull.Value
                             || string.IsNullOrEmpty(r["NgayDKGiao"]?.ToString()))
                    .ToList();

                if (invalid.Count > 0)
                {
                    XtraMessageBox.Show("Vui lòng nhập đầy đủ Kế hoạch ngày giao!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Saved = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi: " + ex.Message, "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) => Save();
        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var view = gridKH.MainView as GridView;
            if (view == null || view.FocusedRowHandle < 0) return;
            view.DeleteRow(view.FocusedRowHandle);
        }
        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) => AddRow();
        private void btnNapLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                (gridKH.MainView as GridView)?.CloseEditor();
                _tblData.Clear();

                var sizeColNames = _tblData.Columns.Cast<DataColumn>()
                    .Where(c => c.ColumnName.StartsWith("KH@"))
                    .Select(c => c.ColumnName).ToList();

                var gocRows = _tblChiTietGoc.AsEnumerable()
                    .Where(r => r["RowType"]?.ToString() == "GOC").ToList();

                foreach (DataRow gr in gocRows)
                {
                    string poid = gr["POID"]?.ToString() ?? "";
                    string maMau = gr["MaMau"]?.ToString() ?? "";

                    int nextDot = _tblChiTietGoc.AsEnumerable()
                        .Where(r => r["RowType"]?.ToString() == "KH"
                                 && r["POID"]?.ToString() == poid
                                 && r["MaMau"]?.ToString() == maMau)
                        .Select(r => { int d; return int.TryParse(r["Dot"]?.ToString(), out d) ? d : 0; })
                        .DefaultIfEmpty(0).Max() + 1;

                    DataRow nr = _tblData.NewRow();
                    nr["POID"] = poid;
                    nr["PO"] = gr["PO"]?.ToString() ?? "";
                    nr["ColorCode"] = gr["ColorCode"]?.ToString() ?? "";
                    nr["Mau"] = gr["MaMau"]?.ToString() ?? "";
                    nr["MaMau"] = maMau;
                    nr["DauSize"] = gr["DauSize"]?.ToString() ?? "";
                    nr["Dot"] = nextDot.ToString();
                    nr["MaBrand"] = "";
                    nr["NgayDKGiao"] = DBNull.Value;
                    nr["GhiChu"] = "";

                    foreach (string colName in sizeColNames)
                        nr[colName] = 0;

                    _tblData.Rows.Add(nr);
                }

                (gridKH.MainView as GridView)?.RefreshData();

                if (_tblData.Rows.Count > 0)
                {
                    var first = _tblData.Rows[0];
                    string po0 = first["PO"]?.ToString() ?? "";
                    string maMau0 = first["MaMau"]?.ToString() ?? "";
                    string poid0 = first["POID"]?.ToString() ?? "";
                    string dauSize0 = first["DauSize"]?.ToString() ?? "";
                    _lblSummaryTitle.Text = $"📋  PO: {po0}  |  Màu: {maMau0}  |  InSeam: {dauSize0}";
                    RefreshSummary(po0, maMau0, poid0, dauSize0);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi: " + ex.Message, "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S)) { Save(); return true; }
            if (keyData == Keys.Delete)
            {
                var view = gridKH.MainView as GridView;
                if (view != null && view.FocusedRowHandle >= 0) view.DeleteRow(view.FocusedRowHandle);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            var view = sender as GridView;
            string field = view.FocusedColumn.FieldName;
            if (!field.StartsWith("KH@")) return;

            var row = view.GetDataRow(view.FocusedRowHandle);
            if (row == null) return;

            string poid = row["POID"]?.ToString() ?? "";
            string maMau = row["MaMau"]?.ToString() ?? "";
            string sizeOnly = field.Substring("KH@".Length); 

            string sizeCaption = sizeOnly.Contains("_")
                ? sizeOnly.Substring(sizeOnly.LastIndexOf('_') + 1)
                : sizeOnly;
            string sizeField = _tblChiTietGoc.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName.StartsWith("Size@"))
                .Select(c => c.ColumnName)
                .FirstOrDefault(cn =>
                {
                    string s = cn.Substring("Size@".Length);
                    string sShort = s.Contains("_") ? s.Substring(s.LastIndexOf('_') + 1) : s;
                    return s == sizeOnly || sShort == sizeOnly || sShort == sizeCaption;
                });

            if (sizeField == null) return;

            string khFieldInGoc = _tblChiTietGoc.Columns.Cast<DataColumn>()
                .Where(c => c.ColumnName.StartsWith("KH@"))
                .Select(c => c.ColumnName)
                .FirstOrDefault(cn =>
                {
                    string s = cn.Substring("KH@".Length);
                    string sShort = s.Contains("_") ? s.Substring(s.LastIndexOf('_') + 1) : s;
                    return s == sizeOnly || sShort == sizeOnly || sShort == sizeCaption;
                });

            string dauSize = row["DauSize"]?.ToString() ?? "";

            var gocRow = _tblChiTietGoc.AsEnumerable()
                .FirstOrDefault(r => r["RowType"]?.ToString() == "GOC"
                                  && r["POID"]?.ToString() == poid
                                  && r["MaMau"]?.ToString() == maMau
                                  && r["DauSize"]?.ToString() == dauSize);
            if (gocRow == null) return;

            int maxSL = ParseInt(gocRow[sizeField]);

            int tongDaLuu = 0;

            if (khFieldInGoc != null && _tblChiTietGoc.Columns.Contains(khFieldInGoc))
            {
                tongDaLuu = _tblChiTietGoc.AsEnumerable()
                    .Where(r => r["RowType"]?.ToString() == "KH"
                             && r["POID"]?.ToString() == poid
                             && r["MaMau"]?.ToString() == maMau
                             && r["DauSize"]?.ToString() == dauSize)  
                    .Sum(r => ParseInt(r[khFieldInGoc]));
            }

            int tongTrongForm = _tblData.AsEnumerable()
                .Where(r => r != row
                         && r["POID"]?.ToString() == poid
                         && r["MaMau"]?.ToString() == maMau
                         && r["DauSize"]?.ToString() == dauSize   
                         && _tblData.Columns.Contains(field))
                .Sum(r => ParseInt(r[field]));

            int newVal = ParseInt(e.Value);
            int conLai = maxSL - tongDaLuu - tongTrongForm; 

            if (newVal > conLai)
            {
                int allowed = Math.Max(0, conLai);
                e.Valid = false;
                e.ErrorText = $"Size {sizeCaption}: Gốc {maxSL} | Đã KH {tongDaLuu} | Còn lại {allowed}";
            }
        }
    }
}