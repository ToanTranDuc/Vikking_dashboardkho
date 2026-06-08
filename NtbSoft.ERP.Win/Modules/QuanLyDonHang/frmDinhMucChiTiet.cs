using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmDinhMucChiTiet : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _makh = string.Empty, _mahang = string.Empty, _mavtID = string.Empty, _mauvtID = string.Empty, _mauVT = string.Empty, _nhomVT = string.Empty, _khovaiID = string.Empty,
                       _khachhang = string.Empty, _tenhang = string.Empty, _chitiet = string.Empty, _mausp = string.Empty, _mavt = string.Empty, _madot = string.Empty, _dot = string.Empty, _manhom = string.Empty, _SLDinhMuc = string.Empty,_isneww=string.Empty;
        string MaMauID = string.Empty, _mavtGhep = string.Empty;

        public bool _isCheck = false;

        bool _checkSave;

        DataTable tblSize = new DataTable();
        DataTable tblSave = new DataTable();
        DataTable _dt = new DataTable();
        public static DataTable _tblSize = new DataTable();
        public static bool checkOutView = false;


        public frmDinhMucChiTiet(string makh, string khachhang, string mahang, string nhomvt, string mavtid, string mavt, string chitiet, string mauvt, string mausp, string madot, string dot, string manhom, string mauvtid, string khovaiid, bool tachmau,string isneww, string mavtghep, string SLDinhMuc = "", string MaMauID = "", bool checkSave = false, bool isEdit = true)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._makh = makh;
            this._khachhang = khachhang;
            this._mahang = mahang;
            this._nhomVT = nhomvt;
            this._mavtID = mavtid;
            this._mavt = mavt;
            this._chitiet = chitiet;
            this._mauVT = mauvt;
            this._mausp = mausp;
            this._madot = madot;
            this._dot = dot;
            this._manhom = manhom;
            this._mauvtID = mauvtid;
            this._khovaiID = khovaiid;
            this._SLDinhMuc = SLDinhMuc;
            this.MaMauID = MaMauID;
            this._checkSave = checkSave;
            this._isneww = isneww;
            _isCheck = tachmau;
            this._mavtGhep = _mavtGhep;
            checkOutView = false;
            if (!isEdit)
            {
                bandedGVSize.OptionsBehavior.Editable = false;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            txtKH.Text = _khachhang;
            txtMaHang.Text = _mahang;
            txtNhom.Text = _nhomVT;
            txtMaVT.Text = _mavt;
            txtChiTiet.Text = _chitiet;
            txtMauVT.Text = _mauVT;
            txtMauSP.Text = _mausp;
            txtDot.Text = _dot;
            checkBox1.Checked = _isCheck;
           
            loadSize();
            //LoadTachMau();
            tblSave = createTableSave();

            tinhDMChung();
       
        }

        private void bandedGVSize_ShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.BandedGrid.BandedGridView;
            var column = view.FocusedColumn;
            var value = view.GetFocusedValue();

            if (value != null &&
                ((value is int && (int)value == 0) ||
                 (value is decimal && (decimal)value == 0) ||
                 (value is double && (double)value == 0) ||
                 (value is float && (float)value == 0)))
            {
                view.SetFocusedValue(null);
            }

        }

        private void bandedGVSize_MouseUp(object sender, MouseEventArgs e)
        {
            //try
            //{
            //    GridHitInfo hitInfo = bandedGVSize.CalcHitInfo(e.Location);

            //    if (hitInfo.InRowCell && hitInfo.Column.FieldName == "CheckAll" && e.Button == MouseButtons.Left)
            //    {
            //        var currentValue = Convert.ToBoolean(bandedGVSize.GetRowCellValue(hitInfo.RowHandle, hitInfo.Column));
            //        bandedGVSize.SetRowCellValue(hitInfo.RowHandle, hitInfo.Column, !currentValue);
            //    }

            //    if (hitInfo.InRowCell && hitInfo.Column.FieldName == "AllSize" && e.Button == MouseButtons.Left)
            //    {
            //        bandedGVSize.FocusedColumn = hitInfo.Column;
            //        bandedGVSize.FocusedRowHandle = hitInfo.RowHandle;
            //        bandedGVSize.ShowEditor();
            //    }
            //}
            //catch (Exception ex)
            //{


            //}

        }

        private DataTable createTableSave()
        {

            DataTable tbl = new DataTable("tblSave");
            tbl.Columns.Add("ID", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MauVTID", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("MaNhomSize", typeof(string));
            tbl.Columns.Add("MaSize", typeof(string));
            tbl.Columns.Add("DinhMuc", typeof(Decimal));
            tbl.Columns.Add("MaDot", typeof(string));
            tbl.Columns.Add("Dot", typeof(string));
            tbl.Columns.Add("NguoiTao", typeof(string));
            tbl.Columns.Add("NgayTao", typeof(string));
            tbl.Columns.Add("MaMauID", typeof(string));
            tbl.Columns.Add("TachMau", typeof(bool));
            tbl.Columns.Add("MaVTGhep", typeof(string));
            return tbl;
        }

        private void loadSize()
        {

            try
            {
                string urlCT = string.Format("{0}?makh={1}&mahang={2}&manhom={3}&mavtid={4}&mauvtid={5}&khovaiid={6}&madot={7}&isnew={8}", URL + $"KhoiTaoDM/GetSizeDinhMucChung", _makh, _mahang, _madot, MaMauID, _mauvtID, _mavtID, _khovaiID,_isneww);
                string jsonCT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlCT); }).Result;

                tblSize = JsonConvert.DeserializeObject<DataTable>(jsonCT);
                if (tblSize == null || tblSize.Rows.Count == 0)
                {
                    gCSize.DataSource = null;
                    return;
                }
                createTable(tblSize);
                if (!tblSize.Columns.Contains("AllSize"))
                {
                    tblSize.Columns.Add("AllSize", typeof(float));
                }
                if (!tblSize.Columns.Contains("CheckAll"))
                {
                    tblSize.Columns.Add("CheckAll", typeof(bool)).DefaultValue = true;
                }
                var targetColumns = tblSize.Columns
                .Cast<DataColumn>()
                .Where(col => col.ColumnName.Contains("@"))
                .Select(col => col.ColumnName)
                .ToList();
                //double dmchung = tinhDMChung();
                if (!_checkSave)
                {
                    tblSize.AsEnumerable()
                 .ToList()
                 .ForEach(row =>
                     targetColumns.ForEach(column => row[column] = Convert.ToDouble(_SLDinhMuc.ToString() == "" ? "0" : _SLDinhMuc))
                 );
                }
                tblSize.AsEnumerable().ToList().ForEach(r => r["CheckAll"] = true);
                gCSize.DataSource = tblSize;
                _dt = tblSize.Copy();
            }
            catch (Exception ex)
            {

            }

        }

        private void repositoryItemCheckEdit11_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void repositoryItemCheckEdit11_Click(object sender, EventArgs e)
        {

        }

        private void bandedGVSize_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void bandedGVSize_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Location);

            if (hitInfo.InRowCell && hitInfo.Column.FieldName == "CheckAll") // Thay bằng field thật
            {
                view.FocusedColumn = hitInfo.Column;
                view.FocusedRowHandle = hitInfo.RowHandle;

                // Bắt buộc hiển thị editor
                if (!view.IsEditing)
                {
                    view.ShowEditor();
                }

                // Bắt checkbox và toggle
                CheckEdit edit = view.ActiveEditor as CheckEdit;
                if (edit != null)
                {
                    // Toggle giá trị
                    edit.Checked = !Convert.ToBoolean(view.GetRowCellValue(hitInfo.RowHandle, hitInfo.Column));

                    // Cập nhật vào data source
                    view.PostEditor();
                }

                // Ngăn chọn cell nếu cần — không dùng e.Handled ở đây
            }
        }

        private void createTable(DataTable tab)
        {
            bandedGVSize.OptionsView.AllowCellMerge = false;
            GridBand parentBand = bandedGVSize.Bands["gridBandSize"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < bandedGVSize.Columns.Count;)
                {
                    if (bandedGVSize.Columns[i].FieldName.Contains("@"))
                    {
                        bandedGVSize.Columns.RemoveAt(i);
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
                if (demColIndex > 6 && demColIndex < tab.Columns.Count)
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[1];
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
                    col.Width = 60;
                    // col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    col.DisplayFormat.FormatString = "{0:0.####}";
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedGVSize.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.Caption = colName;
                    gb.Columns.Add(col);
                    gb.Name = "gbColBandSize" + col;
                    gb.VisibleIndex = 0;
                    gb.Width = 60;
                    gridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }

        }
        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = txtDot;
            DataTable tblGrid = gCSize.DataSource as DataTable;
            DataTable tblPost = setupTableSave(tblGrid);

            string url = $"{URL}KhoiTaoDM/PostDMChiTiet?para={MaMauID}";

            string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, tblPost); }).Result;
            if (msResult.ToLower() == "true")
            {
                checkOutView = true;
                this.DialogResult = DialogResult.OK;
            }

        }
        private DataTable setupTableSave(DataTable tbl)
        {
            tblSave.Clear();
            DataTable _dt = new DataTable();
            _dt = tblSize.Copy();
            if ((bool)checkBox1.Checked == false)
            {

                string[] maMauArr = MaMauID.Split('|');
                string[] tenMauArr = _mausp.Split('|');
                var distinctGroups = _dt.AsEnumerable()
                                            .Select(r => r["MaNhomSize"].ToString())
                                            .Distinct()
                                            .ToList();

                int groupCount = distinctGroups.Count;
                int colorCount = Math.Min(maMauArr.Length, tenMauArr.Length);

                if (colorCount == 1)
                {
                    foreach (DataRow row in _dt.Rows)
                    {
                        row["MaMau"] = maMauArr[0].ToString().Trim();
                        row["TenMau"] = tenMauArr[0].ToString().Trim();
                    }
                }
                else
                {
                    foreach (DataRow row in _dt.Rows)
                    {
                        row["MaMau"] = maMauArr[0].ToString().Trim();
                        row["TenMau"] = tenMauArr[0].ToString().Trim();
                    }

                    List<DataRow> originalRows = _dt.AsEnumerable()
                                                        .Where(r => r["MaMau"].ToString() == maMauArr[0].ToString().Trim())
                                                        .ToList();

                    for (int colorIndex = 1; colorIndex < colorCount; colorIndex++)
                    {
                        foreach (DataRow baseRow in originalRows)
                        {
                            DataRow newRow = _dt.NewRow();
                            newRow.ItemArray = baseRow.ItemArray.Clone() as object[];
                            newRow["MaMau"] = maMauArr[colorIndex].ToString().Trim();
                            newRow["TenMau"] = tenMauArr[colorIndex].ToString().Trim();
                            _dt.Rows.Add(newRow);
                        }
                    }
                }
            }
            int columnsCount = _dt.Columns.Count;

            foreach (DataRow dr in _dt.Rows)
            {
                for (int i = 7; i < columnsCount - 2; i++)
                {
                    DataRow _newRow = tblSave.NewRow();
                    _newRow["ID"] = 0;
                    _newRow["MaKH"] = dr["MaKH"];
                    _newRow["MaHang"] = dr["MaHang"];
                    _newRow["MaNhom"] = _manhom;
                    _newRow["MaVTID"] = _mavtID;
                    _newRow["MauVTID"] = _mauvtID;
                    _newRow["KhoVaiID"] = _khovaiID;
                    _newRow["MaNhomSize"] = dr["MaNhomSize"];
                    _newRow["MaDot"] = _madot;
                    _newRow["Dot"] = _dot;
                    _newRow["NguoiTao"] = GlobleData.UserName;
                    _newRow["NgayTao"] = null;
                    _newRow["MaMauID"] = dr["MaMau"].ToString().Trim();
                    string[] arr = tbl.Columns[i].ToString().Split('@');
                    _newRow["MaSize"] = arr[0];
                    _newRow["DinhMuc"] = (dr[i] == null || string.IsNullOrWhiteSpace(dr[i].ToString())) ? 0 : dr[i];
                    _newRow["TachMau"] = _isCheck;
                    _newRow["MaVTGhep"] = _mavtGhep;
                    tblSave.Rows.Add(_newRow);
                }

            }
            return tblSave;
        }
        private void bandedGVSize_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                object oldValue = bandedGVSize.GetRowCellValue(e.RowHandle, e.Column);


                if (e.Column.FieldName == "AllSize")
                {
                    bool isChecked = false;
                    bool.TryParse(bandedGVSize.GetRowCellValue(e.RowHandle, "CheckAll")?.ToString(), out isChecked);

                    if (isChecked && !string.IsNullOrEmpty(e.Value?.ToString()))
                    {
                        decimal soLuong;
                        if (decimal.TryParse(e.Value.ToString(), out soLuong) && soLuong >= 0)
                        {
                            foreach (GridColumn column in bandedGVSize.Columns)
                            {
                                if (column.FieldName.Contains("@"))
                                {
                                    bandedGVSize.SetRowCellValue(e.RowHandle, column, soLuong.ToString());
                                }
                            }
                        }
                    }
                }
                else if (e.Column.FieldName == "CheckAll")
                {
                    bool isChecked = false;
                    bool.TryParse(e.Value?.ToString(), out isChecked);

                    string allSizeValue = bandedGVSize.GetRowCellValue(e.RowHandle, "AllSize")?.ToString();
                    decimal soLuong;
                    bool hasAllSize = decimal.TryParse(allSizeValue, out soLuong);

                    if (isChecked)
                    {
                        foreach (GridColumn column in bandedGVSize.Columns)
                        {
                            if (column.FieldName.Contains("@"))
                            {
                                object currentValue = bandedGVSize.GetRowCellValue(e.RowHandle, column);
                                decimal currentSoLuong;

                                // Trường hợp AllSize có giá trị => đặt hết
                                if (hasAllSize && soLuong >= 0)
                                {
                                    bandedGVSize.SetRowCellValue(e.RowHandle, column, soLuong.ToString());
                                }
                                // Trường hợp AllSize == null => chỉ set giá trị cho các ô null hoặc = 0
                                else if (!hasAllSize)
                                {
                                    if (currentValue == null ||
                                        !decimal.TryParse(currentValue.ToString(), out currentSoLuong) ||
                                        currentSoLuong == 0)
                                    {
                                        bandedGVSize.SetRowCellValue(e.RowHandle, column, "0");
                                    }
                                }
                            }
                        }
                    }
                }

                if (e.Column.FieldName.Contains("@"))
                {
                    DataRow row = bandedGVSize.GetFocusedDataRow();
                    if (Convert.ToBoolean(row["CheckAll"]))
                    {
                        bandedGVSize.SetRowCellValue(e.RowHandle, e.Column, oldValue);
                    };

                }
            }
            catch (Exception ex)
            {

            }
        }
        private void bandedGVSize_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {

            GridView view = (GridView)sender;
            if (view.FocusedColumn.FieldName.Contains("@") || view.FocusedColumn.FieldName.ToString() == "AllSize")
            {
                decimal outParse = -1;

                if (e != null && e.Value != null)
                {
                    string inputValue = e.Value.ToString();
                    bool isValidDecimal = decimal.TryParse(inputValue, out outParse);
                    if (string.IsNullOrEmpty(inputValue))
                    {
                        e.Value = 0;
                    }
                    else if (isValidDecimal)
                    {
                        if (outParse < 0)
                        {
                            e.Valid = false;
                            e.ErrorText = "Vui lòng nhập > 0!";
                            return;
                        }
                        int decimalPlaces = BitConverter.GetBytes(decimal.GetBits(outParse)[3])[2];
                        if (decimalPlaces > 4)
                        {
                            e.Valid = false;
                            e.ErrorText = "Vui lòng nhập tối đa 4 chữ số sau dấu thập phân!";
                            return;
                        }
                    }
                    else
                    {
                        e.Valid = false;
                        e.ErrorText = "Vui lòng nhập số!";
                        return;
                    }
                }

            }
        }
        private double tinhDMChung()
        {
            DataTable tbl = gCSize.DataSource as DataTable;
            if (tbl == null || tbl.Rows.Count == 0) return 0;

            double dmCT = 0;
            double sum = 0;
            int index = 0;

            foreach (DataRow dr in tbl.Rows)
            {
                foreach (DataColumn col in tbl.Columns)
                {
                    if (col.ColumnName.Contains("@"))
                    {
                        if (double.TryParse(dr[col].ToString(), out double temp))
                        {
                            sum += temp;
                            index++;
                        }
                    }
                }
            }
            if (index == 0) index = 1;
            dmCT = Math.Round(sum / index, 4);
            return dmCT;
        }
        private void bandedGVSize_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (bandedGVSize.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {

                        DevExpress.Utils.Menu.DXMenuItem menuCopyMauItem = new DevExpress.Utils.Menu.DXMenuItem("Copy ", ItemCopy_Click1);
                        e.Menu.Items.Add(menuCopyMauItem);

                        DevExpress.Utils.Menu.DXMenuItem menuPasteMauItem = new DevExpress.Utils.Menu.DXMenuItem("Paste ", ItemPaste_Click1);
                        e.Menu.Items.Add(menuPasteMauItem);

                    }

                }
            }
        }
        private void ItemCopy_Click1(object sender, EventArgs e)
        {
            try
            {
                BandedGridView view = bandedGVSize as BandedGridView;
                if (view == null) return;
                view.CopyToClipboard();
            }
            catch (Exception ex)
            {


            }

        }
        private void ItemPaste_Click1(object sender, EventArgs e)
        {
            Paste();
        }
        private void Paste()
        {
            try
            {
                BandedGridView view = bandedGVSize as BandedGridView;
                string clipboardData = Clipboard.GetText();
                byte[] bytes = Encoding.UTF8.GetBytes(clipboardData);
                string decodedClipboardData = Encoding.UTF8.GetString(bytes);
                string[] data = decodedClipboardData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 0) return;

                string checkDataRow = string.Join("\t",
                    bandedGVSize.Columns
                        .Cast<DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn>()
                        .Select(col => col.Caption)
                );
                List<string> allCaptions = GetBandCaptions(bandedGVSize.Bands);
                string text = string.Join("\t", allCaptions);
                //string checkDataRow = "Mã vật tư\tChi tiết\tĐơn vị\tKhổ/Size\tMã màu vật tư\tMàu vật tư\tMàu";
                if (checkDataRow.Contains(data[0]))
                {
                    data = data.Skip(1).ToArray();
                }
                int startRow = view.FocusedRowHandle;
                foreach (string row in data)
                {
                    AddRow(row, startRow++, view);
                    if (!view.IsValidRowHandle(startRow)) break;
                }
                if (view.IsFocusedView)
                {
                    // Chỉ gửi phím ESC khi GridView đang được focus
                    // SendKeys.SendWait("{ESC}");
                }


            }
            catch (Exception ex)
            {
            }
        }
        private List<string> GetBandCaptions(GridBandCollection bands)
        {
            List<string> captions = new List<string>();
            foreach (GridBand band in bands)
            {
                captions.Add(band.Caption);
                if (band.Children.Count > 0)
                    captions.AddRange(GetBandCaptions(band.Children));
            }
            return captions;
        }

        // Sử dụng:

        private void gCSize_ProcessGridKey(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                Paste();
            }
        }

        private void AddRow(string data, int rowHandle, GridView view)
        {
            try
            {
                if (string.IsNullOrEmpty(data)) return;

                string[] rowData = data.Split('\t');
                int columnIndex = view.FocusedColumn.VisibleIndex;

                int originalRowHandle = view.GetDataSourceRowIndex(rowHandle); // Đảm bảo lấy dòng gốc khi merge

                for (int i = 0; i < rowData.Length; i++)
                {
                    if (i >= view.VisibleColumns.Count) break;
                    if (i + columnIndex >= view.VisibleColumns.Count) break;
                    try
                    {
                        GridColumn targetColumn = view.VisibleColumns[columnIndex + i];

                        if (targetColumn.ToString() == "bandedGridColumn3") break;
                        Type fieldType = view.Columns[targetColumn.FieldName].ColumnType;

                        if (fieldType == typeof(int))
                        {
                            if (int.TryParse(rowData[i].Replace(",", ""), out int intValue))
                            {
                                view.SetRowCellValue(rowHandle, targetColumn, intValue);
                            }
                            else
                            {

                                this.ActiveControl = button1;
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

                                this.ActiveControl = button1;
                            }
                        }
                        else
                        {
                            view.SetRowCellValue(rowHandle, targetColumn, rowData[i]);
                            this.ActiveControl = button1;
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
            catch (Exception ex)
            {


            }

        }
        private void LoadTachMau()
        {

            if ((bool)checkBox1.Checked == true)
            {
                _isCheck = true;

                string[] maMauArr = MaMauID.Split('|');
                string[] tenMauArr = _mausp.Split('|');
                var distinctGroups = tblSize.AsEnumerable()
                                            .Select(r => r["MaNhomSize"].ToString())
                                            .Distinct()
                                            .ToList();

                int groupCount = distinctGroups.Count;
                int colorCount = Math.Min(maMauArr.Length, tenMauArr.Length);

                if (colorCount == 1)
                {
                    foreach (DataRow row in tblSize.Rows)
                    {
                        row["MaMau"] = maMauArr[0].ToString().Trim();
                        row["TenMau"] = tenMauArr[0].ToString().Trim();
                    }
                }
                else
                {
                    foreach (DataRow row in tblSize.Rows)
                    {
                        row["MaMau"] = maMauArr[0].ToString().Trim();
                        row["TenMau"] = tenMauArr[0].ToString().Trim();
                    }

                    List<DataRow> originalRows = tblSize.AsEnumerable()
                                                        .Where(r => r["MaMau"].ToString() == maMauArr[0].ToString().Trim())
                                                        .ToList();

                    for (int colorIndex = 1; colorIndex < colorCount; colorIndex++)
                    {
                        foreach (DataRow baseRow in originalRows)
                        {
                            DataRow newRow = tblSize.NewRow();
                            newRow.ItemArray = baseRow.ItemArray.Clone() as object[];
                            newRow["MaMau"] = maMauArr[colorIndex].ToString().Trim();
                            newRow["TenMau"] = tenMauArr[colorIndex].ToString().Trim();
                            tblSize.Rows.Add(newRow);
                        }
                    }
                }

                bandedGridColumn5.GroupIndex = 0;
                gCSize.DataSource = tblSize;
            }
            else
            {
                _isCheck = false;

                foreach (DataRow dr in _dt.Rows)
                {
                    dr["MaMau"] = "";
                    dr["TenMau"] = "";
                    foreach (DataColumn col in _dt.Columns)
                    {
                        if (col.ColumnName.Contains("@"))
                        {
                            dr[col.ColumnName] = 0;
                        }
                    }
                }
                DataTable distinctTblSize = _dt.AsEnumerable()
                    .Distinct(DataRowComparer.Default) 
                    .CopyToDataTable();
                tblSize = distinctTblSize.Copy();
                bandedGridColumn5.GroupIndex = -1;
                gCSize.DataSource = tblSize;
            }

        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            LoadTachMau();
        }
        private void bandedGVSize_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == bandedGridColumn5)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
          
        }
    }
}