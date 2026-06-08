using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmPhieuBaoGia : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<int> lstRowUpdate = new List<int>();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;
        KeyDownControlHandler keyDownControlHandler;
        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        Helper helper = new Helper();
        string _mancc = string.Empty;

        SearchCheckSelection gridCheckMarksColor;
        SearchCheckSelection gridCheckMarksSize;
        SearchCheckSelection gridCheckMarksDauSize;
        SearchCheckSelection gridCheckMarksKho;
        SearchCheckSelection gridCheckMarksVattu;
        string mau = string.Empty, size = string.Empty, dausize = string.Empty, kho = string.Empty,vt = string.Empty;
        int sum = 0;
        DataTable _dtTable;
        DataTable grid;
        DataTable _tblMau;
        DataTable _tblKho;
        public frmPhieuBaoGia()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
        }

        protected override void OnLoad(EventArgs e)
        {
            CreateSearchlookupNCC();
        }

        private void CreateSearchlookupNCC()
        {
            string url = $"{URL}NhaCC/GetPhieuBG?action=GETNCC&para=&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookupEditNCC.Properties.DataSource = tbl;
            //searchLookUpEditCL.Properties.DataSource = tblCL;
            searchLookupEditNCC.Properties.ValueMember = "MaNCC";
            searchLookupEditNCC.Properties.DisplayMember = "TenNCC";

        }


        private void CreateSearchlookupcolvt()
        {
            string url = $"{URL}NhaCC/GetPhieuBG?action=GETVATTU&para=&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEditvt = new RepositoryItemSearchLookUpEdit();
            rCountryEditvt.DataSource = tbl;
            rCountryEditvt.DisplayMember = "ChiTiet";
            rCountryEditvt.ValueMember = "MaVTID";
            rCountryEditvt.ShowClearButton = false;
            rCountryEditvt.NullText = "[Chọn giá trị]";

            GridView dvViewvt = rCountryEditvt.View;
            if (dvViewvt.Columns.Count == 0)
            {
                dvViewvt.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewvt.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewvt.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewvt.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewvt.Columns.Add(new GridColumn { FieldName = "MaVTID", Caption = "Mã khổ id", Name = "colMaHang", Visible = false });
                dvViewvt.Columns.Add(new GridColumn { FieldName = "ChiTiet", Caption = "Chi tiết", Name = "colTenHang", Visible = true });

            }
            bandedGridColumn6.ColumnEdit = rCountryEditvt;

        }

        private void CreateSearchlookupcolMau()
        {
            //string mavtid = searchLookupEditVT.EditValue == null ? "" : searchLookupEditVT.EditValue.ToString();
            string mavtid = vt;
            string url = $"{URL}NhaCC/GetPhieuBG?action=GETMAU&para=&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblMau = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEditmau = new RepositoryItemSearchLookUpEdit();
            rCountryEditmau.DataSource = _tblMau;
            rCountryEditmau.DisplayMember = "MauVT";
            rCountryEditmau.ValueMember = "MauVTID";
            rCountryEditmau.ShowClearButton = false;
            rCountryEditmau.NullText = "[Chọn giá trị]";

            GridView dvViewmau = rCountryEditmau.View;
            if (dvViewmau.Columns.Count == 0)
            {
                dvViewmau.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewmau.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewmau.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewmau.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewmau.Columns.Add(new GridColumn { FieldName = "MauVTID", Caption = "Mã Màu ID", Name = "colMaHang", Visible = false });
                dvViewmau.Columns.Add(new GridColumn { FieldName = "MauVT", Caption = "Tên Màu", Name = "colTenHang", Visible = true });

            }
            bandedGridColumn7.ColumnEdit = rCountryEditmau;

        }



        private void CreateSearchlookupcolKho()
        {
            //string mavtid = searchLookupEditVT.EditValue == null ? "" : searchLookupEditVT.EditValue.ToString();
            string mavtid = vt;
            string url = $"{URL}NhaCC/GetPhieuBG?action=GETKHO&para=&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _tblKho = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEditkho = new RepositoryItemSearchLookUpEdit();
            rCountryEditkho.DataSource = _tblKho;
            rCountryEditkho.DisplayMember = "KhoVai";
            rCountryEditkho.ValueMember = "KhoVaiID";
            rCountryEditkho.ShowClearButton = false;
            rCountryEditkho.NullText = "[Chọn giá trị]";

            GridView dvViewkho = rCountryEditkho.View;
            if (dvViewkho.Columns.Count == 0)
            {
                dvViewkho.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewkho.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewkho.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewkho.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewkho.Columns.Add(new GridColumn { FieldName = "KhoVaiID", Caption = "Mã khổ id", Name = "colMaHang", Visible = false });
                dvViewkho.Columns.Add(new GridColumn { FieldName = "MauVT", Caption = "Tên Màu", Name = "colTenHang", Visible = true });

            }
            bandedGridColumn11.ColumnEdit = rCountryEditkho;

        }

        private void CreateSearchlookupcoldonvi()
        {
            //string mavtid = searchLookupEditVT.EditValue == null ? "" : searchLookupEditVT.EditValue.ToString();
            string mavtid = vt;
            string url = $"{URL}NhaCC/GetPhieuBG?action=GETDONVI&para=&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _tbldv = JsonConvert.DeserializeObject<DataTable>(json);
            RepositoryItemSearchLookUpEdit rCountryEditkho = new RepositoryItemSearchLookUpEdit();
            rCountryEditkho.DataSource = _tbldv;
            rCountryEditkho.DisplayMember = "TenDVVT";
            rCountryEditkho.ValueMember = "MaDVVT";
            rCountryEditkho.ShowClearButton = false;
            rCountryEditkho.NullText = "[Chọn giá trị]";

            GridView dvViewkho = rCountryEditkho.View;
            if (dvViewkho.Columns.Count == 0)
            {
                dvViewkho.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvViewkho.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvViewkho.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvViewkho.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvViewkho.Columns.Add(new GridColumn { FieldName = "KhoVaiID", Caption = "Mã khổ id", Name = "colMaHang", Visible = false });
                dvViewkho.Columns.Add(new GridColumn { FieldName = "MauVT", Caption = "Tên Màu", Name = "colTenHang", Visible = true });

            }
            bandedGridColumn12.ColumnEdit = rCountryEditkho;

        }

        private DataTable CreateDatatable()
        {
            DataTable tbl = new DataTable("dtTable");
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("MaVT", typeof(string));
            tbl.Columns.Add("SoLuong", typeof(int));
            tbl.Columns.Add("DonGiaMM", typeof(int));
            return tbl;
        }

        private DataTable CreateDatatableToSave()
        {
            DataTable tbl = new DataTable("dtSize");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("MaPhieu", typeof(string));
            tbl.Columns.Add("MaNCC", typeof(string));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("SizeID", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("SoLuong", typeof(int));
            tbl.Columns.Add("DonGiaMM", typeof(int));
            tbl.Columns.Add("UngTruoc", typeof(string));
            tbl.Columns.Add("PhuongThucTT", typeof(string));
            tbl.Columns.Add("ThoiDiem", typeof(DateTime));
            tbl.Columns.Add("Tong", typeof(float));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("Phieu", typeof(int));
            return tbl;
        }


        private DataTable CreateColumnNhap()
        {
            DataTable _dataTable = new DataTable();
            _dataTable.Columns.Add("MaVTID", typeof(string));
            _dataTable.Columns.Add("MauVTID", typeof(string));
            _dataTable.Columns.Add("Kho", typeof(string));
            //for (int i = 0; i < _lstSize.Count; i++)
            //{
            //    _dataTable.Columns.Add(string.Format("Size {0}", _lstSize[i]), typeof(int));
            //}
            return _dataTable;
        }

        private DataTable CreateRowNhap(DataTable _dtTable, List<string> _lstPO, List<string> _lstMau, List<string> _lstDauSize)
        {
            Console.WriteLine("CreateRowNhap");
            foreach (string _po in _lstPO)
            {
                foreach (string _dauSize in _lstDauSize)
                {
                    foreach (string _mau in _lstMau)
                    {
                        DataRow row = _dtTable.NewRow();
                        row["MaVTID"] = _po;
                        row["MauVTID"] = _mau;
                        row["Kho"] = _dauSize;
                        _dtTable.Rows.Add(row);
                    }
                }
            }
            return _dtTable;
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

        private DataTable MapData(DataTable _dtTable, DataTable tblChanged)
        {
            DataTable _tblTempSLSize = tblChanged.Copy();
            string urlMau = string.Format("{0}?", URL + "BangMau/GetBangMau");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMau); }).Result;
            DataTable tblmau = JsonConvert.DeserializeObject<DataTable>(json);

            // Xóa 3 cột đầu tiên
            //for (int i = 0; i < 3; i++)
            //{
            //    if (_tblTempSLSize.Columns.Count > 0)
            //    {
            //        _tblTempSLSize.Columns.RemoveAt(0);
            //    }
            //}


            // Xóa cột cuối cùng nếu có tên là "SỐ LƯỢNG"
            if (_tblTempSLSize.Columns.Count > 0)
            {
                DataColumn lastColumn = _tblTempSLSize.Columns[_tblTempSLSize.Columns.Count - 1];
                if (RemoveDiacritics(lastColumn.ColumnName.ToUpper()).Replace(" ", "").Trim() == "SOLUONG")
                {
                    _tblTempSLSize.Columns.Remove(lastColumn);
                }
            }

            // POID, PO, MaMau, DauSize, MaQG, NgayGH, GhiChu
            foreach (DataRow row in tblChanged.Rows)
            {
                if (row[0].ToString() == "***")
                {
                    break;
                }
                DataRow _rowAdd = _dtTable.NewRow();
                _rowAdd["MaVTID"] = row[0].ToString();
                _rowAdd["MauVTID"] = row[1].ToString().ToUpper().Trim();
                _rowAdd["Kho"] = row[2].ToString().ToUpper().Trim();
                _rowAdd["SoLuong"] = row[3].ToString().ToUpper().Trim();
                _dtTable.Rows.Add(_rowAdd);

            }


            // Map danh sách số lượng của từng size
            int index = 0;
            foreach (DataColumn column in _dtTable.Columns)
            {
                if (column.ColumnName.Contains("@Size@"))
                {
                    for (int i = 0; i < _dtTable.Rows.Count; i++)
                    {
                        _dtTable.Rows[i][column] = _tblTempSLSize.Rows[i][index];
                    }
                    index += 1;
                }
            }

            if (grid != null)
            {

                foreach (DataRow dtRow in _dtTable.Rows)
                {
                    string mavtid = dtRow["MaVTID"].ToString();
                    string mauvtid = dtRow["MauVTID"].ToString();
                    string manhomsize = dtRow["Kho"].ToString();
                    //string size = dtRow["MaSize"].ToString();

                    //DataRow[] matchingRows = grid.Select($"MaVTID = '{poid}'  AND MauVTID ='{po}'  AND MaNhomSize ='{mamau}' AND MaNhomSize = '{dausize}'");


                    DataRow[] matchingRows = grid.Select($"MaVTID = '{mavtid}'  AND MauVTID ='{mauvtid}'  AND MaNhomSize ='{manhomsize}'");

                    if (matchingRows.Length > 0)
                    {
                        // Cập nhật dữ liệu của dòng trùng lặp
                        DataRow gridRow = matchingRows[0];
                        dtRow.BeginEdit();

                        foreach (DataColumn column in grid.Columns)
                        {
                            if (dtRow.Table.Columns.Contains(column.ColumnName))
                            {
                                // Ghi đè dữ liệu từ gridRow sang dtRow
                                dtRow[column.ColumnName] = gridRow[column.ColumnName];
                            }
                            //dtRow[column.ColumnName] = gridRow[column.ColumnName];
                        }
                        dtRow.EndEdit();
                    }
                }
            }

            return _dtTable;
        }

        private void createTable(DataTable tab)
        {
            //gridViewThongTinDonHang.OptionsView.AllowCellMerge = false;

            //gridbandSize.Children.Clear();
            GridBand parentBand = bandedGridView1.Bands["gridbandSize"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < bandedGridView1.Columns.Count;)
                {
                    if (bandedGridView1.Columns[i].FieldName.Contains("@Size@"))
                    {
                        bandedGridView1.Columns.RemoveAt(i);
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
                if (demColIndex > 3 && demColIndex < tab.Columns.Count)
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[2];
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
                    bandedGridView1.Columns.AddRange(new BandedGridColumn[] { col });
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
                    //gridbandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }

        }

        private DevExpress.XtraGrid.Views.Base.BaseView GetBandGridViewAmount(DataTable tab)
        {
            BandedGridView bandedView = bandedGridView1;
            bandedView.OptionsView.ShowColumnHeaders = false;
            bandedView.OptionsView.ShowGroupPanel = false;
            bandedView.OptionsView.ColumnAutoWidth = false;
            bandedView.OptionsView.ShowFooter = true;
            // bandedView.OptionsView.AllowCellMerge = true;
            bandedView.OptionsCustomization.AllowBandMoving = false;
            if (tab.Columns.Count == 0) return bandedView;

            bandedView.CustomDrawBandHeader += bandedView_CustomDrawBandHeader;
            bandedView.CustomColumnDisplayText += bandedView_CustomColumnDisplayText;
            bandedView.CustomDrawFooter += BandedView_CustomDrawFooter;
            bandedView.CustomDrawFooterCell += BandedView_CustomDrawFooterCell;
            //bandedView.PopupMenuShowing += BandedView_PopupMenuShowing;
            bandedView.OptionsSelection.MultiSelect = true;
            bandedView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            bandedView.RowCountChanged += BandedView_RowCountChanged;
            bandedView.CustomDrawRowIndicator += BandedView_CustomDrawRowIndicator;
            bandedView.KeyPress += gridview1_KeyPress;
            bandedView.ValidatingEditor += BandedView_ValidatingEditor;
            bandedView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bandedView.OptionsNavigation.EnterMoveNextColumn = true;
            return bandedView;
        }

        void bandedView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
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

        void bandedView_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "NhomSize")
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
                        string valueStr = e.Value.ToString();
                        int parsedValue;
                        bool isNumeric = Int32.TryParse(valueStr, out parsedValue);

                        if (!isNumeric)
                        {
                            e.DisplayText = "-";
                        }
                        else
                        {
                            if (Convert.ToInt32(e.Value.ToString()) == 0)
                                e.DisplayText = "-";
                        }

                    }
                }
            }

        }

        private void BandedView_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
        {
            GridView view = (GridView)sender;
            if (view.FocusedColumn.FieldName.Contains("@"))
            {
                int outParse = -1;
                if (e != null && e.Value != null)
                {
                    bool flagParse = int.TryParse(e.Value.ToString(), out outParse);
                    if (string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Value = 0;
                    }
                    else if (flagParse && outParse < 0)
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập > 0!.";
                        return;
                    }
                    else if (!flagParse)
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập số!.";
                        return;
                    }
                }
            }

        }
        private void BandedView_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
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

        private void searchLookUpEdit1_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            foreach (DataRowView rv in gridCheckMark.Selection)
            {

                if (sb.ToString().Length > 0) { sb.Append(":"); }
                sb.Append(rv["KhoVai"].ToString());
            }
            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chọn Màu----";
            }
            else
            {
                e.DisplayText = sb.ToString();
            }
        }

        private void bandedGridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

        }


        private void bandedGridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.IsGetData)
            {
                sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName.Contains("@Size@") && col.UnboundType == UnboundColumnType.Bound)
                    {
                        try
                        {
                            if (row.Table.Columns.Contains(col.FieldName) && row[col.FieldName] != null && !string.IsNullOrEmpty(row[col.FieldName].ToString()))
                            {
                                int value;
                                bool isValidNumber = Int32.TryParse(row[col.FieldName].ToString(), out value);

                                if (!isValidNumber)
                                {
                                    value = 0;
                                }
                                else
                                {
                                    Convert.ToInt32(row[col.FieldName]);
                                }
                                //int value = Convert.ToInt32(row[col.FieldName]);
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
                {
                    e.Value = sum;
                    //textEditTong.EditValue = sum * ;
                }

            }
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

        private void gridview1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
            }
        }

        private void bandedGridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
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
                    if (col.FieldName != "MaVTID" && col.FieldName != "MauVTID" && col.FieldName != "MaNhomSize" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            //frmChonVTPhieu frm = new frmChonVTPhieu();
            //if(_dtTable != null && _dtTable.Rows.Count >0)
            //{
            //    frm.ListExistingVT = _dtTable.Copy();
            //}
            
            //if (frm.ShowDialog() == DialogResult.OK)
            //{
            //    DataTable selectedTable = frm._tbl;
            //    if (selectedTable == null || selectedTable.Rows.Count == 0)
            //        return;
            //    _dtTable = CreateDatatable();
            //    foreach (DataRow row in selectedTable.Rows)
            //    {
            //        DataRow newRow = _dtTable.NewRow();
            //        newRow["MaVTID"] = row["MaVTID"];
            //        newRow["MauVTID"] = row["MauVTID"];
            //        newRow["KhoVaiID"] = row["KhoVaiID"];
            //        newRow["MaDVVT"] = row["MaDVVT"];
            //        newRow["MaVT"] = row["MaVT"];
            //        newRow["SoLuong"] = 0;
            //        newRow["DonGiaMM"] = 0;
            //        _dtTable.Rows.Add(newRow);
            //    }

            //    CreateSearchlookupcolvt();
            //    CreateSearchlookupcolMau();
            //    CreateSearchlookupcolKho();
            //    CreateSearchlookupcoldonvi();
            //    gridControl1.DataSource = _dtTable;
            //}
        }

        private void searchLookupEditVT_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            SearchCheckSelection gridCheckMark = sender is SearchLookUpEdit ? (sender as SearchLookUpEdit).Properties.Tag as SearchCheckSelection : (sender as RepositoryItemSearchLookUpEdit).Tag as SearchCheckSelection;
            if (gridCheckMark == null) return;
            foreach (DataRowView rv in gridCheckMark.Selection)
            {

                if (sb.ToString().Length > 0) { sb.Append(":"); }
                sb.Append(rv["ChiTiet"].ToString());
            }
            if (string.IsNullOrEmpty(sb.ToString()))
            {
                e.DisplayText = "----Chọn Vật Tư----";
            }
            else
            {
                e.DisplayText = sb.ToString();
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Xóa dữ liệu grid
            gridControl1.DataSource = null;
            _dtTable = null;
            // Refresh giao diện
            gridControl1.RefreshDataSource();
            gridControl1.Refresh();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dt = gridControl1.DataSource as DataTable;
            if (searchLookupEditNCC.EditValue == null || searchLookupEditNCC.EditValue == "")
            {
                MessageBox.Show("Vui lòng chọn nhà cung cấp.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            SaveData(dt);

            this.DialogResult = DialogResult.OK;
        }


        public void SaveData(DataTable dt)
        {
            string url1 = $"{URL}NhaCC/GetPhieuBG?action=GETPHIEUBG&para=&para1=&para2=&para3=&para4=&para5=";
            string json1 = Task.Run(async () => { return await _clientExtension.GetAsnyc(url1); }).Result;
            DataTable _tbldv = JsonConvert.DeserializeObject<DataTable>(json1);

            DataTable tblDG = CreateDatatableToSave();

            int maxPhieu = 0;
            if (_tbldv != null && _tbldv.Rows.Count > 0 && _tbldv.Columns.Contains("Phieu"))
            {
                var validValues = _tbldv.AsEnumerable()
                                        .Where(r => r["Phieu"] != DBNull.Value &&
                                                    int.TryParse(r["Phieu"].ToString(), out _))
                                        .Select(r => Convert.ToInt32(r["Phieu"]));

                if (validValues.Any())
                    maxPhieu = validValues.Max();
            }

            int newPhieu = maxPhieu + 1;

            foreach (DataRow dr in dt.Rows)
            {
                DataRow newRow = tblDG.NewRow();
                newRow["ID"] = 0;
                newRow["MaPhieu"] = "";
                newRow["MaNCC"] = searchLookupEditNCC.EditValue?.ToString();

                newRow["MaVTID"] = dr["MaVTID"]?.ToString();
                newRow["MaMau"] = dr["MauVTID"]?.ToString();
                newRow["DauSizeID"] = "";
                newRow["SizeID"] = "";
                newRow["KhoVaiID"] = dr["KhoVaiID"]?.ToString();

                newRow["SoLuong"] = Convert.ToInt32(dr["SoLuong"]);

                newRow["DonGiaMM"] = dr["DonGiaMM"]?.ToString();
                newRow["UngTruoc"] = "";
                newRow["PhuongThucTT"] =  string.IsNullOrEmpty(textEditPTTT.Text) ? "" : textEditPTTT.Text;
                newRow["ThoiDiem"] = DBNull.Value;

                newRow["Tong"] = 0;
                newRow["MaDVVT"] = dr["MaDVVT"]?.ToString();
                newRow["Phieu"] = newPhieu;
                tblDG.Rows.Add(newRow);
            }

            // GỌI API ĐỂ LƯU PHIẾU
            string url = string.Format("{0}?", URL + "NhaCC/Postphieu");
            string mss = Task.Run(async () =>
            {
                return await _clientExtension.PostAsync(url, tblDG);
            }).Result;

            if (mss.ToLower() != "true")
                XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                clsWaitForm.ShowSuccessForm(this, 3000);

        }
    }
}