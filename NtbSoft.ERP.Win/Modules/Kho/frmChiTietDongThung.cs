using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmChiTietDongThung : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _madh = string.Empty, _malenh = string.Empty, _mapkl = string.Empty, _poid = string.Empty, _madvsx = string.Empty, _dausizeID = string.Empty, _mau = string.Empty;
        private int _tuthung = 0, _denthung = 0, sum = 0, _sttthung = 0, _minsttthung = 0, _maxsttthung = 0;
        DataTable tbl;
        bool _sms = false;
        public frmChiTietDongThung(string mapkl, string malenh, string madvsx, string poid, string dausizeid, string mau, int tuthung, int denthung, int sttthung, int minsttthung, int maxsttthung, string madh, bool sms)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._madh = madh;
            this._malenh = malenh;
            this._mapkl = mapkl;
            this._madvsx = madvsx;
            this._tuthung = tuthung;
            this._denthung = denthung;
            this._poid = poid;
            this._dausizeID = dausizeid;
            this._mau = mau;
            this._sttthung = sttthung;
            this._minsttthung = minsttthung;
            this._maxsttthung = maxsttthung;
            this._sms = sms;
            tbl = new DataTable();
        }


        protected override void OnLoad(EventArgs e)
        {
            barEditItemcbLoc.EditValue = repositoryItemComboBox1.Items[0].ToString();
            LoadData();
        }

        private void LoadData()
        {
            if(!_sms)
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&minsttthung={9}&&maxsttthung={10}", URL + "DongThung/GetThongTinChiTiet", _mapkl, _malenh, _madvsx, _poid, _madh, _mau, _tuthung, _denthung, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                tbl = JsonConvert.DeserializeObject<DataTable>(json);
            }   
            else
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&minsttthung={9}&&maxsttthung={10}", URL + "DongThung/GetThongTinChiTietSMS", _mapkl, _malenh, _madvsx, _poid, _madh, _mau, _tuthung, _denthung, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                tbl = JsonConvert.DeserializeObject<DataTable>(json);
            }    
            if (tbl.Rows.Count == 0)
            {
                DataTable tbnull = new DataTable();
                gridControlDongThung.DataSource = tbnull;
            }
            else
            {

                if (barEditItemcbLoc.EditValue.ToString().Trim().ToUpper().Replace(" ", "") == "TẤTCẢ")
                {
                    gridControlDongThung.DataSource = tbl;
                }
                if (barEditItemcbLoc.EditValue.ToString().Trim().ToUpper().Replace(" ", "") == "ĐÃĐÓNGTHÙNG")
                {
                    var selectedRows = tbl.AsEnumerable().Where(row => (bool)row["IsDongThung"] == true);
                    if (selectedRows.Any())
                        tbl = selectedRows.CopyToDataTable();
                    else
                    {
                        MessageBox.Show("Dữ liệu lọc không tìm thấy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    gridControlDongThung.DataSource = tbl;

                }
                if (barEditItemcbLoc.EditValue.ToString().Trim().ToUpper().Replace(" ", "") == "CHƯAĐÓNGTHÙNG")
                {
                    var selectedRows = tbl.AsEnumerable().Where(row => (bool)row["IsDongThung"] == false);
                    if (selectedRows.Any())
                        tbl = selectedRows.CopyToDataTable();
                    else
                    {
                        MessageBox.Show("Dữ liệu lọc không tìm thấy. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    gridControlDongThung.DataSource = tbl;
                }         
            }

        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {

                foreach (DataRow _dr in tbl.Rows)
                {
                    _dr["CheckDT"] = true;
                }
                gridControlDongThung.RefreshDataSource();
                e.Handled = true;
            }
            if (e.Control && e.KeyCode == Keys.Z)
            {

                foreach (DataRow _dr in tbl.Rows)
                {
                    _dr["CheckDT"] = false;
                }
                gridControlDongThung.RefreshDataSource();
                e.Handled = true;
            }
            switch (e.KeyCode)
            {
                case Keys.F5:
                    LoadData();
                    break;
            }
        }
        private void btHuyDongThung_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = this.button1;
            DataTable dtDongThung = this.gridControlDongThung.DataSource as DataTable;
            DataTable dtSave = new DataTable();
            dtSave.Columns.Add("MaDH", typeof(string));
            dtSave.Columns.Add("MaPKL", typeof(string));
            dtSave.Columns.Add("MaDVSX", typeof(string));
            dtSave.Columns.Add("MaLenh", typeof(string));
            dtSave.Columns.Add("POID", typeof(string));
            dtSave.Columns.Add("TuThung", typeof(int));
            dtSave.Columns.Add("DenThung", typeof(int));
            dtSave.Columns.Add("MinSttThung", typeof(int));
            dtSave.Columns.Add("MaxSttThung", typeof(int));
            dtSave.Columns.Add("SttThungDecat", typeof(int));
            foreach (DataRow dr in tbl.Rows)
            {
                if ((bool)dr["CheckDT"] == false)
                {
                    DataRow _dr = dtSave.NewRow();
                    _dr["MaDH"] = dr["MaGop"];
                    _dr["MaPKL"] = dr["MaPKL"];
                    _dr["MaDVSX"] = dr["MaDVSX"];
                    _dr["MaLenh"] = dr["MaLenh"];
                    _dr["POID"] = _poid;//dr["POID"];
                    _dr["TuThung"] = 0;
                    _dr["DenThung"] = 0;
                    _dr["MinSttThung"] = 0;
                    _dr["MaxSttThung"] = 0;
                    _dr["SttThungDecat"] = dr["SttDT"];
                    dtSave.Rows.Add(_dr);
                }

            }
            if (dtSave.Rows.Count <= 0) return;
            string url = "";
            if(!_sms)
                url = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostChiTiet", false);
            else
                url = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostChiTietSMS", false);
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (result.ToLower() != "true")
                XtraMessageBox.Show(result);
            else
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadData();
            }

        }

        private void btCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            barEditItemcbLoc.EditValue = repositoryItemComboBox1.Items[0].ToString();
            LoadData();
        }

        private void barEditItemcbLoc_EditValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
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
        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view != null)
            {
                DataRow focusedRow = view.GetDataRow(view.FocusedRowHandle);
                DataTable dtDongThung = this.gridControlDongThung.DataSource as DataTable;
                if (focusedRow == null) return;
                foreach (DataRow _dr in dtDongThung.Rows)
                {
                    if (_dr["SttThung"].ToString() == focusedRow["SttThung"].ToString())
                        _dr["CheckDT"] = e.Value;
                }
            }
        }

        private void btDongThung_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = this.button1;
            DataTable dtDongThung = this.gridControlDongThung.DataSource as DataTable;
            DataTable dtSave = new DataTable();
            dtSave.Columns.Add("MaDH", typeof(string));
            dtSave.Columns.Add("MaPKL", typeof(string));
            dtSave.Columns.Add("MaDVSX", typeof(string));
            dtSave.Columns.Add("MaLenh", typeof(string));
            dtSave.Columns.Add("POID", typeof(string));
            dtSave.Columns.Add("TuThung", typeof(int));
            dtSave.Columns.Add("DenThung", typeof(int));
            dtSave.Columns.Add("MinSttThung", typeof(int));
            dtSave.Columns.Add("MaxSttThung", typeof(int));
            dtSave.Columns.Add("SttThungDecat", typeof(int));
            foreach (DataRow dr in tbl.Rows)
            {
                if ((bool)dr["CheckDT"] == true)
                {
                    DataRow _dr = dtSave.NewRow();
                    _dr["MaDH"] = dr["MaGop"];
                    _dr["MaPKL"] = dr["MaPKL"];
                    _dr["MaDVSX"] = dr["MaDVSX"];
                    _dr["MaLenh"] = dr["MaLenh"];
                    _dr["POID"] = _poid;//dr["POID"];
                    _dr["TuThung"] = 0;
                    _dr["DenThung"] = 0;
                    _dr["MinSttThung"] = 0;
                    _dr["MaxSttThung"] = 0;
                    _dr["SttThungDecat"] = dr["SttDT"];
                    dtSave.Rows.Add(_dr);
                }

            }
            if (dtSave.Rows.Count <= 0) return;
            string url = "";
            if(!_sms)
                url = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostChiTiet", true);
            else
                url = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostChiTietSMS", true);
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (result.ToLower() != "true")
                XtraMessageBox.Show(result);
            else
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadData();
            }

        }


        #region
        private DevExpress.XtraGrid.Views.Base.BaseView GetBandGridViewAmount(DataTable tab)
        {
            BandedGridView bandedView = new BandedGridView();
            bandedView.OptionsView.ShowColumnHeaders = false;
            bandedView.OptionsView.ShowGroupPanel = false;
            bandedView.OptionsView.ColumnAutoWidth = false;
            bandedView.OptionsView.ShowFooter = true;
            if (tab.Columns.Count == 0) return bandedView;
            for (int i = 0; i < 12; i++)
            {
                List<string> _ListString = new List<string>();
                if (tab.Columns[i].ColumnName == "PO")
                {
                    _ListString.Add("PO");
                    SetGridBandedViewAmount(bandedView, "", "PO", _ListString);
                    _ListString.Clear();
                }
                if (tab.Columns[i].ColumnName == "DauSize")
                {
                    _ListString.Add("DauSize");
                    SetGridBandedViewAmount(bandedView, "", "Đầu Size", _ListString);
                    _ListString.Clear();
                }

                if (tab.Columns[i].ColumnName == "TenMau")
                {
                    _ListString.Add("TenMau");
                    SetGridBandedViewAmount(bandedView, "", "Màu", _ListString);
                    _ListString.Clear();
                }
            }
            List<string> listHeader = new List<string>();
            int j = 12;
            string maHang = string.Empty;
            while (j < tab.Columns.Count)
            {
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
            colAmount.Caption = "Tổng";
            colAmount.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
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

                if (col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "DauSizeID"
                    && col.FieldName != "MaLenh" && col.FieldName != "MaPKL" && col.FieldName != "MaHang" && col.FieldName != "MaDVSX" && col.FieldName != "SttThung" && col.FieldName != "TenMau")
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
            bandedView.CustomUnboundColumnData += BandedView_CustomUnboundColumnData;
            bandedView.CustomDrawFooter += BandedView_CustomDrawFooter;
            bandedView.CustomSummaryCalculate += BandedView_CustomSummaryCalculate;
            bandedView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bandedView.OptionsNavigation.EnterMoveNextColumn = true;
            bandedView.OptionsBehavior.Editable = false;

            return bandedView;
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
                    if (col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "DauSizeID"
                    && col.FieldName != "MaLenh" && col.FieldName != "MaPKL" && col.FieldName != "MaHang" && col.FieldName != "MaDVSX" && col.FieldName != "SttThung" && col.FieldName != "TenMau" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
            }
        }

        private void BandedView_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void BandedView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.IsGetData)
            {
                int sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "DauSizeID"
                    && col.FieldName != "MaLenh" && col.FieldName != "MaPKL" && col.FieldName != "MaHang" && col.FieldName != "MaDVSX" && col.FieldName != "SttThung" && col.FieldName != "TenMau" && col.UnboundType == UnboundColumnType.Bound)
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

        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            // Kiểm tra nếu đây là ô cần được tùy chỉnh
            if (e.Column.FieldName == "TrangThai")
            {
                if (e.RowHandle >= 0)
                {
                    string Strflagkv = view.GetRowCellDisplayText(e.RowHandle, view.Columns["TrangThai"]);
                   
                    if (RemoveVietnameseTone(Strflagkv).ToString().Trim().ToUpper().Replace(" ","") == "DADONGTHUNG")
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }   
                }
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
            gridBand.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridBand.AppearanceHeader.Options.UseFont = true;
            gridBand.AppearanceHeader.Options.UseTextOptions = true;
            gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridBand.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            if (nrOfColumns == 1 && (columnNames[0] == "POID" || columnNames[0] == "PO" || columnNames[0] == "MaMau" || columnNames[0] == "DauSize" || columnNames[0] == "DauSizeID" || columnNames[0] == "MaPKL" || columnNames[0] == "MaDVSX")
                || columnNames[0] == "MaLenh" || columnNames[0] == "MaDH" || columnNames[0] == "MaHang" || columnNames[0] == "MaDVSX" || columnNames[0] == "SttThung" || columnNames[0] == "TenMau")
            {
                BandedGridColumn bandedColumns = (BandedGridColumn)bandedView.Columns.AddField(columnNames[0]);


                String CaptionName = columnNames[0];
                bandedColumns.OwnerBand = gridBand;
                bandedColumns.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
                bandedColumns.Caption = GridBandCaption;
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
                    bandedColumns[i].Width = 100;
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
        #endregion
    }
}
