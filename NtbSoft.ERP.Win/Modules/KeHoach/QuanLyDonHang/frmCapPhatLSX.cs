using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.CanDoiDonHang;
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
using System.Windows.Forms.VisualStyles;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmCapPhatLSX : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        string _magop = string.Empty, _nguoitao = string.Empty, _maLSX = string.Empty, _tenKH = string.Empty, _mahang = string.Empty, _madh = string.Empty, _malenh = string.Empty, _soluong = string.Empty, _tenhang = string.Empty, _dot = string.Empty, _makh = string.Empty, _madhgop = string.Empty;
        int _iskeove = 0;



        DataTable tblThongSo, tblChiTiet, tblDot, tblDM;
        public frmCapPhatLSX(string _magop, string _nguoitao, string _maLSX, string _tenKH, string _mahang, string _madh, string _malenh, string _soluong, string _tenhang, int _iskeove, string _dot, string _makh, string _madhgop)
        {
            this._magop = _magop;
            this._nguoitao = _nguoitao;
            this._maLSX = _maLSX;
            this._tenKH = _tenKH;
            this._mahang = _mahang;
            this._madh = _madh;
            this._malenh = _malenh;
            this._soluong = _soluong;
            this._tenhang = _tenhang;
            this._iskeove = _iskeove;
            this._dot = _dot;
            this._makh = _makh;
            this._madhgop = _madhgop;
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();

        }

        protected override void OnLoad(EventArgs e)
        {
            tblThongSo = new DataTable();
            tblChiTiet = CreateDatatable();
            tblDot = new DataTable();
            tblDM = new DataTable();
            txtLenhSX.Text = _malenh;
            txtDH.Text = _madhgop;
            txtKH.Text = _tenKH;
            txtMH.Text = _tenhang;

            InItChungLoaiChiTiet();
            loaddataCanDoi();
            CreateSearchLookUpDot();

            //loadGetCapPhatDotCanDoi();

            this.ActiveControl = btn1;
            if (!isTinh)
                layoutControlItem21.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            else
                layoutControlItem21.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            //LoadData();
        }

        private void loaddataCanDoi()
        {
            string urldvsx = string.Format("{0}?magop={1}&&malenhsanxuat={2}", URL + "DonHangTong/GetChiTietLenhSX", _magop, _maLSX);
            string jsondvsx = Task.Run(async () => { return await _clientExtension.GetAsnyc(urldvsx); }).Result;
            if (jsondvsx == "[]")
            {
                gridControl4.DataSource = null;
                return;
            }
            DataTable tbldvsx = JsonConvert.DeserializeObject<DataTable>(jsondvsx);
            if (!tbldvsx.Columns.Contains("Tong"))
            {
                tbldvsx.Columns.Add("Tong", typeof(string));

            }
            createTableCT(tbldvsx);


            gridControl4.DataSource = tbldvsx;
            this.ActiveControl = btn1;
        }
        private DataTable CreateDatatable()
        {
            DataTable tbl = new DataTable("tblChiTiet");
            tbl.Columns.Add("ID", typeof(string));
            tbl.Columns.Add("MaKH", typeof(string));
            tbl.Columns.Add("MaHang", typeof(string));
            tbl.Columns.Add("MaNPL", typeof(string));
            tbl.Columns.Add("MaVTID", typeof(string));
            tbl.Columns.Add("MaNhom", typeof(string));
            tbl.Columns.Add("TenNhom", typeof(string));
            tbl.Columns.Add("TenVT", typeof(string));
            tbl.Columns.Add("ChiTiet", typeof(string));
            tbl.Columns.Add("MaVT", typeof(string));
            tbl.Columns.Add("MaDot", typeof(string));
            tbl.Columns.Add("Dot", typeof(string));
            tbl.Columns.Add("MauID", typeof(string));
            tbl.Columns.Add("MaMau", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            tbl.Columns.Add("MaMauVT", typeof(string));
            tbl.Columns.Add("MauVT", typeof(string));
            tbl.Columns.Add("KhoVaiID", typeof(string));
            tbl.Columns.Add("KhoVai", typeof(string));
            tbl.Columns.Add("MaDVVT", typeof(string));
            tbl.Columns.Add("TenDVVT", typeof(string));
            tbl.Columns.Add("SoLuong", typeof(int));
            tbl.Columns.Add("DinhMuc", typeof(decimal));
            tbl.Columns.Add("CapPhat", typeof(decimal));
            tbl.Columns.Add("CapThem", typeof(decimal));
            tbl.Columns.Add("GhiChu", typeof(string));
            tbl.Columns.Add("Sort", typeof(int));
            tbl.Columns.Add("DinhMucHaoHut", typeof(float));
            tbl.Columns.Add("NPL", typeof(bool));
            tbl.Columns.Add("ThucXuat", typeof(decimal));
            tbl.Columns.Add("DinhMucChung", typeof(decimal));
            tbl.Columns.Add("CapPhatTK", typeof(decimal));
            tbl.Columns.Add("NhuCau", typeof(decimal));
            tbl.Columns.Add("TachMau", typeof(decimal));
            tbl.Columns.Add("MaVTGhep", typeof(string));
            tbl.Columns.Add("TachMauSPDH", typeof(int));
            tbl.Columns.Add("MaCode", typeof(string));
            tbl.Columns.Add("QDLe", typeof(bool));
            tbl.Columns.Add("MaCLCT", typeof(string));
            tbl.Columns.Add("DinhMucSX", typeof(decimal));
            return tbl;
        }
        private void CreateSearchLookUpDot()
        {
            searchLookUpEditDot.Properties.DisplayMember = "Dot";
            searchLookUpEditDot.Properties.ValueMember = "MaDot";

            string url = string.Format("{0}?makh={1}&&mahang={2}", URL + "DonHangTong/GetCapPhatThongSoDot", _makh.ToString(), _mahang.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            tblDot = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditDot.Properties.DataSource = tblDot;
            searchLookUpEditDot.EditValue = tblDot.Rows[0][0];
            isTinh = true;
            loadCapPhat();
        }
        private void LoaDM()
        {
            string madot = searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString();
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GETDMTONG&para1={_makh.ToString()}&para2={_mahang.ToString()}&para3={madot.ToString()}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tblDM = JsonConvert.DeserializeObject<DataTable>(json);
        }
        private void LoadData()
        {
            string url = string.Format("{0}?makh={1}&&mahang={2}&&madot={3}&&magop={4}&&malenh={5}&&madh={6}", URL + "DonHangTong/GetCapPhatThongSo", _makh, _mahang, searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(), _magop.ToString(), _maLSX.ToString(), _madh.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                gCThongSo.DataSource = null;
                return;
            }

            tblThongSo = JsonConvert.DeserializeObject<DataTable>(json);
            gCThongSo.DataSource = tblThongSo;
            // CheckAllRowsWithCheckTinh();

        }
        private void CheckAllRowsWithCheckTinh()
        {

            for (int i = 0; i < tblThongSo.Rows.Count; i++)
            {
                string checkMauValue = gVThongSo.GetRowCellValue(i, "CheckTinh").ToString();
                if (checkMauValue != null && checkMauValue.ToString() == "1")
                {
                    gVThongSo.SelectRow(i);
                }
                else
                {
                    gVThongSo.UnselectRow(i);
                }
            }
        }
        private void LoadChiTiet()
        {
            //try
            //{
            //    DataRow dr = gVThongSo.GetFocusedDataRow();
            //    if (dr == null) return;
            //    string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtid={3}&&mauid={4}&&malsx={5}&&madot={6}", URL + "DonHangTong/GetCapPhatThongSoChiTiet", _makh, _mahang, dr["MaVTID"].ToString(), dr["MauID"].ToString(), _maLSX, "DOT_1");
            //    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //    if (json == "[]") return;
            //    tblChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
            //    gCChiTiet.DataSource = tblChiTiet;
            //}
            //catch (Exception ex)
            //{

            //}


        }


        private DataTable AddColumnDataTableKho(DataTable dataTable)
        {
            DataTable dataTableTemp = dataTable;
            List<string> columnNamesWithSize = dataTable.Columns.Cast<DataColumn>()
                        .Where(column => column.ColumnName.Contains("@"))
                        .Select(column => column.ColumnName)
                        .Distinct()
                        .ToList();
            foreach (var item in columnNamesWithSize)
            {
                string[] lstName = item.Split('@');
                string NameColumnSLDM = $"Size_SLDM@{lstName[0]}@{lstName[1]}";
                DataColumn newNameColumnSLDM = new DataColumn(NameColumnSLDM, typeof(float));
                if (!dataTableTemp.Columns.Contains(NameColumnSLDM))
                    dataTableTemp.Columns.Add(newNameColumnSLDM);

                string NameColumnSLSL = $"Size_SLSL@{lstName[0]}@{lstName[1]}";
                DataColumn newNameColumnSLSL = new DataColumn(NameColumnSLSL, typeof(float));
                if (!dataTableTemp.Columns.Contains(NameColumnSLSL))
                    dataTableTemp.Columns.Add(newNameColumnSLSL);

                string NameColumnSLCP = $"Size_SLCP@{lstName[0]}@{lstName[1]}";
                DataColumn newNameColumnSLCP = new DataColumn(NameColumnSLCP, typeof(float));
                if (!dataTableTemp.Columns.Contains(NameColumnSLCP))
                    dataTableTemp.Columns.Add(newNameColumnSLCP);
            }

            foreach (DataRow row in dataTableTemp.Rows)
            {
                foreach (var item in columnNamesWithSize)
                {
                    string[] lstName = item.Split('@');
                    string[] value = row[$"{lstName[0]}@{lstName[1]}"].ToString().Split('@');
                    row[$"Size_SLDM@{lstName[0]}@{lstName[1]}"] = Math.Round(Convert.ToDouble(value[1]), 4);
                    row[$"Size_SLSL@{lstName[0]}@{lstName[1]}"] = value[0];
                    row[$"Size_SLCP@{lstName[0]}@{lstName[1]}"] = Math.Round(Convert.ToDouble(value[2]), 2);

                }

            }
            DataTable dataTablePivot = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(dataTableTemp));

            return dataTablePivot;
        }

        private void bandedGridviewSoLuong_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }


        List<string> lstSize = new List<string>();
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadChiTiet();
        }

        private void gVThongSo_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            Pen headerBorderPen = new Pen(Color.FromArgb(100, 128, 128, 128), 3);
            e.Graphics.DrawRectangle(headerBorderPen, e.Bounds);
            //if (e.Column == null) return;
            //Rectangle rect = e.Bounds;
            //ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            //Brush brush =
            //    e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Column.AppearanceHeader.GradientMode);
            //rect.Inflate(-1, -1);
            //e.Graphics.FillRectangle(brush, rect);
            //e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            //foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            //{
            //    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            //}
            //e.Handled = true;
        }

        private void gvChiTiet_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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


        private void splitContainerControl1_Paint(object sender, PaintEventArgs e)
        {
            //splitContainerControl1.SplitterPosition = splitContainerControl1.Height / 2;
        }

        private void searchLookUpEditDot_EditValueChanged(object sender, EventArgs e)
        {
            // clsWaitForm.ShowWaitForm(this, 3000);
            LoadData();
            LoaDM();
            LoadCanDoi();
        }

        private void gVThongSo_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            //GridView view = gVThongSo;
            //object MaVTID = null, MauID = null, _tachmau = null;
            //int levelGroup = gVThongSo.GetRowLevel(gVThongSo.FocusedRowHandle);
            //if (levelGroup == 0)
            //{
            //    int childHandle = levelGroup;
            //    MaVTID = view.GetRowCellValue(childHandle, colMaVTID);
            //    MauID = view.GetRowCellValue(childHandle, colMauID);
            //}
            //else
            //{
            //    if (view.IsGroupRow(view.FocusedRowHandle))
            //    {
            //        int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
            //        MaVTID = view.GetRowCellValue(childHandle, colMaVTID);
            //        MauID = view.GetRowCellValue(childHandle, colMauID);
            //    }
            //    else
            //    {
            //        MaVTID = view.GetFocusedRowCellValue(colMaVTID);
            //        MauID = view.GetFocusedRowCellValue(colMauID);
            //    }
            //}

            //if (view.IsGroupRow(view.FocusedRowHandle))
            //{
            //    int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
            //    MaVTID = view.GetRowCellValue(childHandle, colMaVTID);
            //    MauID = view.GetRowCellValue(childHandle, colMauID);
            //}
            //else
            //{
            //    MaVTID = view.GetFocusedRowCellValue(colMaVTID);
            //    MauID = view.GetFocusedRowCellValue(colMauID);
            //}
            //string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&madot={4}&&mauID={5}", URL + "DonHangTong/GetChiTietDM", _makh.ToString(),
            //           _mahang.ToString(), MaVTID == null ? "" : MaVTID.ToString(), searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(), MauID == null ? "" : MauID.ToString());
            //string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            //if (tbl.Rows.Count > 0)
            //{
            //    foreach (DataRow row in tbl.Rows)
            //    {
            //        Decimal totalForRow = 0;
            //        int countColumnsWithAt = 0;
            //        int columnCount = tbl.Columns.Count;

            //        for (int i = 0; i < columnCount; i++)
            //        {
            //            string columnName = tbl.Columns[i].ColumnName;
            //            if (columnName.Contains("@"))
            //            {
            //                var columnValue = row[columnName];
            //                if (columnValue != DBNull.Value && Convert.ToDecimal(columnValue) != 0)
            //                {
            //                    totalForRow += Convert.ToDecimal(columnValue) * 1;
            //                    countColumnsWithAt++;
            //                }
            //            }
            //            else if (i + 1 < columnCount && tbl.Columns[i + 1].ColumnName.Contains("@"))
            //            {
            //                var nextColumnValue = row[tbl.Columns[i + 1].ColumnName];
            //                if (nextColumnValue != DBNull.Value && Convert.ToDecimal(nextColumnValue) != 0)
            //                {
            //                    totalForRow += Convert.ToDecimal(nextColumnValue);
            //                    countColumnsWithAt++;
            //                }
            //            }
            //        }
            //        if (countColumnsWithAt > 0)
            //        {
            //            Decimal average = Math.Round(totalForRow / countColumnsWithAt, 4);
            //            row["DinhMucGY"] = average;
            //        }
            //    }
            //    if (tbl.Rows.Count > 0)
            //    {
            //        Decimal totalForAllRows = 0;
            //        int countColumnsWithAt = 0;
            //        int totalRows = tbl.Rows.Count;
            //        foreach (DataRow row in tbl.Rows)
            //        {
            //            int columnCount = tbl.Columns.Count;

            //            for (int i = 0; i < columnCount; i++)
            //            {
            //                string columnName = tbl.Columns[i].ColumnName;
            //                if (columnName.Contains("@"))
            //                {
            //                    var columnValue = row[columnName];
            //                    if (columnValue != DBNull.Value && Convert.ToDecimal(columnValue) != 0)
            //                    {
            //                        totalForAllRows += Convert.ToDecimal(columnValue) * 1;
            //                        countColumnsWithAt++;
            //                    }
            //                }
            //                else if (i + 1 < columnCount && tbl.Columns[i + 1].ColumnName.Contains("@"))
            //                {
            //                    var nextColumnValue = row[tbl.Columns[i + 1].ColumnName];
            //                    if (nextColumnValue != DBNull.Value && Convert.ToDecimal(nextColumnValue) != 0)
            //                    {
            //                        totalForAllRows += Convert.ToDecimal(nextColumnValue);
            //                        countColumnsWithAt++;
            //                    }
            //                }
            //            }
            //        }
            //        if (countColumnsWithAt > 0)
            //        {
            //            Decimal average = Math.Round(totalForAllRows / countColumnsWithAt, 4);
            //            foreach (DataRow row in tbl.Rows)
            //            {
            //                row["DinhMucGY"] = average;
            //            }
            //        }
            //    }
            //    createTable(tbl);
            //    gridControl1.DataSource = tbl;

            //}
            //else
            //{
            //    // gridControl1.DataSource = null;
            //}
        }



        private void createTable(DataTable tab)
        {
            //// bandedGridView1.OptionsView.AllowCellMerge = false;
            //GridBand parentBand = bandedGridView1.Bands["gridBandSizeDM"];
            //if (parentBand != null)
            //{
            //    parentBand.Children.Clear();
            //    parentBand.Columns.Clear();
            //    for (int i = 0; i < bandedGridView1.Columns.Count;)
            //    {
            //        if (bandedGridView1.Columns[i].FieldName.Contains("@Size@"))
            //        {
            //            bandedGridView1.Columns.RemoveAt(i);
            //        }
            //        else
            //        {
            //            i += 1;
            //        }
            //    }
            //}
            //int demColIndex = -1;

            //foreach (DataColumn column in tab.Columns)
            //{
            //    demColIndex++;
            //    if (demColIndex > 3 && demColIndex < tab.Columns.Count)
            //    {
            //        string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            //        string colName = arrName[0];
            //        BandedGridColumn col = new BandedGridColumn();
            //        col.AppearanceCell.Options.UseTextOptions = true;
            //        col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //        col.AppearanceHeader.Options.UseTextOptions = true;
            //        col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //        col.Caption = colName;
            //        col.FieldName = column.ColumnName;
            //        col.Name = "col" + colName;
            //        col.OptionsColumn.AllowEdit = false;
            //        col.Visible = true;
            //        col.Width = 50;
            //        //col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
            //        col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            //        //col.DisplayFormat.FormatString = "{0:##,0}";
            //        col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            //        bandedGridView1.Columns.AddRange(new BandedGridColumn[] { col });
            //        GridBand gb = new GridBand();
            //        gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //        gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
            //        gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
            //        gb.AppearanceHeader.Options.UseTextOptions = true;
            //        gb.Caption = colName;
            //        gb.Columns.Add(col);
            //        gb.Name = "gbColBandSize" + col;
            //        gb.VisibleIndex = 0;
            //        gb.Width = 60;
            //        gridBandSizeDM.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
            //    }
            //}
        }



        private void splitContainerControl2_Paint(object sender, PaintEventArgs e)
        {
            //splitContainerControl2.SplitterPosition = splitContainerControl1.Width/1;
        }
        private void LoadCanDoi()
        {
            try
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
                //  var groupedRows = tblThongSo.AsEnumerable()
                //.GroupBy(row => new
                //{
                //    MaVTID = row["MaVTID"].ToString(),
                //    KhoVaiID = row["KhoVaiID"].ToString(),
                //    MaDVVT = row["MaDVVT"].ToString(),
                //    MaNhom = row["MaNhom"].ToString()
                //});

                //  foreach (var group in groupedRows)
                //  {
                //      bool first = true;
                //      foreach (var row in group.OrderBy(r => r["MaCode"].ToString()))
                //      {
                //          if (first)
                //          {
                //              first = false;
                //          }
                //          else
                //          {
                //              row["TonKho"] = 0;
                //              row["SLCap"] = 0;
                //          }
                //      }
                //  }
                tblChiTiet.Clear();
                foreach (DataRow rowData in tblThongSo.Rows)
                {
                    if (Convert.ToInt32(rowData["Status"]) == 1)
                    {
                        string _mavtid = rowData["MaVTID"].ToString();
                        string _mauID = rowData["MauID"].ToString();
                        //  string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&madot={4}&&mauID={5}&&tachmau={6}&&manhom={7}&&KhoVaiID={8}&&macode={9}", URL + "DonHangTong/GetChiTietDM", _makh.ToString(),
                        //_mahang.ToString(), _mavtid == null ? "" : _mavtid, searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(), _mauID.ToString() == "" ? "" : _mauID.ToString(), rowData["TachMau"].ToString() == "" ? "False" : rowData["TachMau"].ToString(), rowData["MaNhom"].ToString(), rowData["KhoVaiID"].ToString(), rowData["MaCode"].ToString());
                        //  string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                        //  DataTable dtDM = JsonConvert.DeserializeObject<DataTable>(json);
                        // Lấy giá trị từ dòng dr
                        string maVTID = rowData["MaVTID"].ToString();
                        string mauVTID = rowData["MauID"].ToString();
                        string khoVaiID = rowData["KhoVaiID"].ToString();
                        string maNhom = rowData["MaNhom"].ToString();
                        string maCode = rowData["MaCode"].ToString();

                        // Lọc các dòng trùng khớp từ tblDM
                        var matchingRows = tblDM.AsEnumerable()
                            .Where(row =>
                                row["MaVTID"].ToString() == maVTID &&
                                row["MauVTID"].ToString() == mauVTID &&
                                row["KhoVaiID"].ToString() == khoVaiID &&
                                row["MaNhom"].ToString() == maNhom &&
                                row["MaCode"].ToString() == maCode
                            );

                        // Copy về một DataTable mới
                        DataTable dtDM = matchingRows.Any() ? matchingRows.CopyToDataTable() : tblDM.Clone();

                        DataTable dtSL = this.gridControl4.DataSource as DataTable;
                        var mausp = new HashSet<string>(rowData["MaMau"].ToString()
                                                             .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                                             .Select(m => m.Trim()));

                        var distinctDauSizeIds = new HashSet<string>(dtDM.AsEnumerable()
                                                                         .Select(row => row["DauSizeID"].ToString())
                                                                         .Distinct());

                        var duLieuDM = dtDM.AsEnumerable()
                            .ToDictionary(
                                row => $"{row["DauSizeID"]}_{row["MaMau"]}",
                                row => row
                            );

                        var sizeColumns = dtSL.Columns.Cast<DataColumn>()
                                              .Where(c => c.ColumnName.Contains("@"))
                                              .Select(c => c.ColumnName)
                                              .ToList();

                        decimal totalQuantity = 0;
                        if (rowData["TachMau"].ToString() == "False")
                        {
                            foreach (DataRow rowSL in dtSL.Rows)
                            {
                                string dauSizeIdSL = rowSL["DauSizeID"].ToString();
                                string maMauSL = rowSL["MaMau"].ToString();
                                if (distinctDauSizeIds.Contains(dauSizeIdSL) && mausp.Contains(maMauSL))
                                {
                                    string key = $"{dauSizeIdSL}_{rowData["MaMau"].ToString()}";
                                    if (duLieuDM.TryGetValue(key, out DataRow rowDM))
                                    {
                                        foreach (string columnName in sizeColumns)
                                        {
                                            if (dtDM.Columns.Contains(columnName))
                                            {
                                                var valueDM = rowDM[columnName];
                                                if (valueDM != DBNull.Value && Convert.ToDecimal(valueDM) > 0)
                                                {
                                                    var valueSL = rowSL[columnName];
                                                    if (valueSL != DBNull.Value)
                                                    {
                                                        totalQuantity += Convert.ToDecimal(valueSL);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (DataRow rowSL in dtSL.Rows)
                            {
                                string dauSizeIdSL = rowSL["DauSizeID"].ToString();
                                string maMauSL = rowSL["MaMau"].ToString();
                                if (distinctDauSizeIds.Contains(dauSizeIdSL) && mausp.Contains(maMauSL))
                                {
                                    string key = $"{dauSizeIdSL}_{maMauSL.ToString()}";
                                    if (duLieuDM.TryGetValue(key, out DataRow rowDM))
                                    {
                                        foreach (string columnName in sizeColumns)
                                        {
                                            if (dtDM.Columns.Contains(columnName))
                                            {
                                                var valueDM = rowDM[columnName];
                                                if (valueDM != DBNull.Value && Convert.ToDecimal(valueDM) > 0)
                                                {
                                                    var valueSL = rowSL[columnName];
                                                    if (valueSL != DBNull.Value)
                                                    {
                                                        totalQuantity += Convert.ToDecimal(valueSL);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        decimal totalAverage = TinhCapPhatTong(_mavtid, _mauID, rowData["MaMau"].ToString(), rowData["TachMau"].ToString(), dtDM);
                        Decimal finalAverage = totalQuantity == 0 ? 0 : Math.Round(totalAverage / totalQuantity, 4);

                        DataRow newRow = tblChiTiet.NewRow();
                        newRow["ID"] = 0;
                        newRow["MaKH"] = rowData["MaKH"];
                        newRow["MaHang"] = rowData["MaHang"];
                        newRow["MaNPL"] = 0;
                        newRow["MaVTID"] = rowData["MaVTID"];
                        newRow["MaNhom"] = rowData["MaNhom"];
                        newRow["TenNhom"] = rowData["TenNhom"];
                        newRow["TenVT"] = rowData["TenVT"];
                        newRow["ChiTiet"] = rowData["ChiTiet"];
                        newRow["MaVT"] = rowData["MaVT"];
                        newRow["MaDot"] = rowData["MaDot"];
                        newRow["Dot"] = rowData["Dot"];
                        newRow["MauID"] = rowData["MauID"];
                        newRow["MaMau"] = rowData["MaMau"];
                        newRow["TenMau"] = rowData["TenMau"];
                        newRow["MaMauVT"] = rowData["MaMauVT"];
                        newRow["MauVT"] = rowData["MauVT"];
                        newRow["KhoVaiID"] = rowData["KhoVaiID"];
                        newRow["KhoVai"] = rowData["KhoVai"];
                        newRow["MaDVVT"] = rowData["MaDVVT"];
                        newRow["TenDVVT"] = rowData["TenDVVT"];
                        newRow["DinhMucChung"] = finalAverage;
                        newRow["SoLuong"] = totalQuantity;
                        newRow["MaCLCT"] = rowData["MaCLCT"];
                        newRow["DinhMucSX"] = finalAverage;
                        int isCL = Convert.ToBoolean(rowData["QDLe"].ToString()) ? 2 : 0;
                        if (rowData["DinhMucHaoHut"].ToString() != "" && rowData["DinhMucHaoHut"].ToString() != "0")
                        {
                            decimal _dmhh = (Convert.ToDecimal(rowData["DinhMucHaoHut"]) / 100) + 1;
                            newRow["CapPhatTK"] = Math.Round(finalAverage * _dmhh * totalQuantity, isCL, MidpointRounding.AwayFromZero);
                            newRow["CapPhat"] = Math.Round(finalAverage * _dmhh * totalQuantity, isCL, MidpointRounding.AwayFromZero);
                            rowData["SLCap"] = Math.Round(finalAverage * _dmhh * totalQuantity, isCL, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            newRow["CapPhatTK"] = Math.Round(totalAverage, isCL, MidpointRounding.AwayFromZero);
                            newRow["CapPhat"] = Math.Round(totalAverage, isCL, MidpointRounding.AwayFromZero);
                            rowData["SLCap"] = Math.Round(totalAverage, isCL, MidpointRounding.AwayFromZero);
                        }
                        newRow["CapThem"] = 0;
                        newRow["Sort"] = rowData["Sort"];
                        newRow["DinhMucHaoHut"] = rowData["DinhMucHaoHut"];
                        newRow["NPL"] = rowData["NPL"];
                        newRow["ThucXuat"] = Math.Round(totalAverage, 2); //Convert.ToDecimal(rowData["SLCL"]) < 0 ? 0 : rowData["SLCL"];
                        newRow["DinhMuc"] = !Convert.ToBoolean(rowData["NPL"]) ? finalAverage : rowData["DinhMucSX"];
                        newRow["NhuCau"] = 0;
                        newRow["TachMau"] = rowData["TachMau"];
                        newRow["MaVTGhep"] = rowData["MaVTGhep"];
                        newRow["TachMauSPDH"] = 0;
                        newRow["MaCode"] = rowData["MaCode"];
                        newRow["QDLe"] = rowData["QDLe"];
                        tblChiTiet.Rows.Add(newRow);

                    }
                }
                
                var groupedRowss = tblThongSo.AsEnumerable()
                .GroupBy(row => new
                {
                    MaVTID = row["MaVTID"].ToString(),
                    MauID = row["MauID"].ToString(),
                    KhoVaiID = row["KhoVaiID"].ToString(),
                    MaDVVT = row["MaDVVT"].ToString(),
                    MaNhom = row["MaNhom"].ToString()
                });

                foreach (var group in groupedRowss)
                {
                    
                    var orderedRows = group.OrderBy(r => r["MaCode"].ToString()).ToList();

                    if (orderedRows.Count == 0)
                        continue;

                    var firstRow = orderedRows[0];
                    decimal tonKho = firstRow["TonKho"] != DBNull.Value ? Convert.ToDecimal(firstRow["TonKho"]) : 0;
                    decimal tongSLCap = orderedRows.Sum(r => r["SLCap"] != DBNull.Value ? Convert.ToDecimal(r["SLCap"]) : 0);

                    decimal slcl = tonKho - tongSLCap;
                    foreach (var row in orderedRows)
                    {
                        row["SLCL"] = slcl;
                    }

                }
                gridControl3.DataSource = tblChiTiet;
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            }
            catch (Exception ex)
            {
                if (DevExpress.XtraSplashScreen.SplashScreenManager.Default != null)
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                else
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(NtbSoft.ERP.Win.Modules.ResourceForm.frmWait));
            }
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < gVThongSo.RowCount; i++)
                {
                    object checkTinhValue = gVThongSo.GetRowCellValue(i, "CheckTinh");
                    if (gVThongSo.IsRowSelected(i) && checkTinhValue != null && checkTinhValue.ToString() == "0")
                    {
                        var rowData = gVThongSo.GetDataRow(i);
                        bool isDuplicate = tblChiTiet.AsEnumerable().Any(row =>
                         (row.Field<string>("MaVTID") ?? "") == (rowData["MaVTID"]?.ToString() ?? "") &&
                         (row.Field<string>("MaNhom") ?? "") == (rowData["MaNhom"]?.ToString() ?? "") &&
                         (row.Field<string>("MaVT") ?? "") == (rowData["MaVT"]?.ToString() ?? "") &&
                         (row.Field<string>("MaDot") ?? "") == (rowData["MaDot"]?.ToString() ?? "") &&
                         (row.Field<string>("MaMauVT") ?? "") == (rowData["MaMauVT"]?.ToString() ?? "") &&
                         (row.Field<string>("TenMau") ?? "") == (rowData["TenMau"]?.ToString() ?? "") &&
                         (row.Field<string>("KhoVai") ?? "") == (rowData["KhoVai"]?.ToString() ?? "")
                     );
                        if (!isDuplicate)
                        {
                            string _mavtid = rowData["MaVTID"].ToString();
                            string _mauID = rowData["MauID"].ToString();
                            string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&madot={4}&&mauID={5}", URL + "DonHangTong/GetChiTietDM", _makh.ToString(),
                          _mahang.ToString(), _mavtid == null ? "" : _mavtid, searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(), _mauID.ToString() == "" ? "" : _mauID.ToString());
                            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                            DataTable dtDM = JsonConvert.DeserializeObject<DataTable>(json);
                            DataTable dtSL = this.gridControl4.DataSource as DataTable;
                            int rowCount = 0;

                            var mausp = rowData["MaMau"].ToString()
                                     .Split('|')
                                     .Select(m => m.Trim())
                                     .ToList();
                            var distinctDauSizeIds = dtDM.AsEnumerable()
                                                         .Select(row => row["DauSizeID"].ToString())
                                                         .Distinct()
                                                         .ToList();
                            decimal totalQuantity = 0;

                            foreach (DataRow rowSL in dtSL.Rows)
                            {
                                string dauSizeIdSL = rowSL["DauSizeID"].ToString();
                                string maMauSL = rowSL["MaMau"].ToString();

                                if (distinctDauSizeIds.Contains(dauSizeIdSL) && mausp.Contains(maMauSL))
                                {
                                    foreach (DataColumn column in dtSL.Columns)
                                    {
                                        if (column.ColumnName.Contains("@"))
                                        {
                                            var columnValue = rowSL[column];
                                            if (columnValue != DBNull.Value && Convert.ToDecimal(columnValue) != 0)
                                            {
                                                totalQuantity += Convert.ToDecimal(columnValue);
                                            }
                                        }
                                    }
                                }
                            }
                            decimal totalAverage = TinhCapPhatTong(_mavtid, _mauID, rowData["MaMau"].ToString(), rowData["TachMau"].ToString(), dtDM);
                            Decimal finalAverage = totalQuantity == 0 ? 0 : Math.Round(totalAverage / totalQuantity, 4);

                            DataRow newRow = tblChiTiet.NewRow();
                            newRow["ID"] = 0;
                            newRow["MaKH"] = rowData["MaKH"];
                            newRow["MaHang"] = rowData["MaHang"];
                            newRow["MaNPL"] = 0;
                            newRow["MaVTID"] = rowData["MaVTID"];
                            newRow["MaNhom"] = rowData["MaNhom"];
                            newRow["TenNhom"] = rowData["TenNhom"];
                            newRow["TenVT"] = rowData["TenVT"];
                            newRow["ChiTiet"] = rowData["ChiTiet"];
                            newRow["MaVT"] = rowData["MaVT"];
                            newRow["MaDot"] = rowData["MaDot"];
                            newRow["Dot"] = rowData["Dot"];
                            newRow["MauID"] = rowData["MauID"];
                            newRow["MaMau"] = rowData["MaMau"];
                            newRow["TenMau"] = rowData["TenMau"];
                            newRow["MaMauVT"] = rowData["MaMauVT"];
                            newRow["MauVT"] = rowData["MauVT"];
                            newRow["KhoVaiID"] = rowData["KhoVaiID"];
                            newRow["KhoVai"] = rowData["KhoVai"];
                            newRow["MaDVVT"] = rowData["MaDVVT"];
                            newRow["TenDVVT"] = rowData["TenDVVT"];
                            newRow["DinhMucChung"] = finalAverage;
                            newRow["SoLuong"] = totalQuantity;
                            newRow["CapPhatTK"] = totalAverage;
                            newRow["CapThem"] = 0;
                            newRow["Sort"] = rowData["Sort"];
                            newRow["DinhMucHaoHut"] = rowData["DinhMucHaoHut"];
                            newRow["NPL"] = rowData["NPL"];
                            newRow["ThucXuat"] = Convert.ToDecimal(rowData["SLCL"]) < 0 ? 0 : rowData["SLCL"];
                            newRow["DinhMuc"] = 0;
                            newRow["CapPhat"] = 0;
                            newRow["NhuCau"] = 0;
                            tblChiTiet.Rows.Add(newRow);
                            gridControl3.DataSource = tblChiTiet;
                        }

                    }

                }
            }
            catch (Exception ex) { }
        }

        private void splitContainerControl3_Paint(object sender, PaintEventArgs e)
        {
            //splitContainerControl3.SplitterPosition = splitContainerControl3.Height / 2;
        }

        private void bandedGridView3_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "Tong" || e.Column.FieldName.Contains("@Size@"))
            {
                e.Appearance.ForeColor = Color.Red;
                //e.Appearance.Font = new Font("Tahoma", 10, FontStyle.Bold); 
            }
        }

        private void bandedGridView3_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            //if (e.Band == null) return;
            //Rectangle rect = e.Bounds;
            //ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            //Brush brush =
            //    e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Band.AppearanceHeader.GradientMode);
            //rect.Inflate(-1, -1);
            //e.Graphics.FillRectangle(brush, rect);
            //e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            //foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            //{
            //    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            //}
            //e.Handled = true;
        }

        private void bandedGridView1_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
        {
            //if (e.Band == null) return;
            //Rectangle rect = e.Bounds;
            //ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            //Brush brush =
            //    e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 212, 128), Color.FromArgb(255, 212, 128), e.Band.AppearanceHeader.GradientMode);
            //rect.Inflate(-1, -1);
            //e.Graphics.FillRectangle(brush, rect);
            //e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            //foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            //{
            //    DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            //}
            //e.Handled = true;
        }
        private void createTableCT(DataTable tab)
        {
            // bandedGridView3.OptionsView.AllowCellMerge = false;
            GridBand parentBand = bandedGridView3.Bands["gridBandSizeCT"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < bandedGridView3.Columns.Count;)
                {
                    if (bandedGridView3.Columns[i].FieldName.Contains("@Size@"))
                    {
                        bandedGridView3.Columns.RemoveAt(i);
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
                if (demColIndex > 3 && demColIndex < tab.Columns.Count - 1)
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[0];
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = colName;
                    col.FieldName = column.ColumnName;
                    col.Name = "col" + colName;
                    col.OptionsColumn.AllowEdit = false;
                    col.Visible = true;
                    col.Width = 50;
                    //col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    col.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    col.SummaryItem.DisplayFormat = "{0:N0}";

                    //col.DisplayFormat.FormatString = "{0:##,0}";
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedGridView3.Columns.AddRange(new BandedGridColumn[] { col });
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
                    gridBandSizeCT.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }

            foreach (DataRow row in tab.Rows)
            {
                int sum = 0;


                foreach (DataColumn col in tab.Columns)
                {
                    if (col.ColumnName.Contains("@"))
                    {
                        if (row[col] != DBNull.Value)
                        {
                            sum += Convert.ToInt32(row[col]);
                        }
                    }
                }
                row["Tong"] = sum;
            }

        }
        private void createTableCTCP(DataTable tab)
        {
            bandedGridView2.OptionsView.AllowCellMerge = false;
            GridBand parentBand = bandedGridView2.Bands["gridBandSizeCP"];
            if (parentBand != null)
            {
                parentBand.Children.Clear();
                parentBand.Columns.Clear();
                for (int i = 0; i < bandedGridView2.Columns.Count;)
                {
                    if (bandedGridView2.Columns[i].FieldName.Contains("@Size@"))
                    {
                        bandedGridView2.Columns.RemoveAt(i);
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
                if (demColIndex > 2 && demColIndex < tab.Columns.Count)
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[0];
                    BandedGridColumn col = new BandedGridColumn();
                    col.AppearanceCell.Options.UseTextOptions = true;
                    col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.AppearanceHeader.Options.UseTextOptions = true;
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    col.Caption = colName;
                    col.FieldName = column.ColumnName;
                    col.Name = "col" + colName;
                    col.OptionsColumn.AllowEdit = false;
                    col.Visible = true;
                    col.Width = 75;
                    //col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                    col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    //col.DisplayFormat.FormatString = "{0:##,0}";
                    col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedGridView2.Columns.AddRange(new BandedGridColumn[] { col });
                    GridBand gb = new GridBand();
                    gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                    gb.AppearanceHeader.ForeColor = System.Drawing.Color.Red;
                    gb.AppearanceHeader.Options.UseTextOptions = true;
                    gb.Caption = colName;
                    gb.Columns.Add(col);
                    gb.Name = "gbColBandSize" + col;
                    gb.VisibleIndex = 0;
                    gb.Width = 75;
                    gridBandSizeCP.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                }
            }
        }


        private void gridView1_FocusedRowChanged_1(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

            DataRow dr = gridView1.GetFocusedDataRow();
            if (dr == null) return;
            string _mavtid = dr["MaVTID"].ToString();
            string _mauID = dr["MauID"].ToString();
            string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&madot={4}&&mauID={5}", URL + "DonHangTong/GetChiTietDM", _makh.ToString(),
                     _mahang.ToString(), _mavtid == null ? "" : _mavtid, searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(), _mauID.ToString() == "" ? "" : _mauID.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dtDM = JsonConvert.DeserializeObject<DataTable>(json);

            //DataTable dtDM = this.gridControl1.DataSource as DataTable;
            DataTable dtSL = this.gridControl4.DataSource as DataTable;
            DataTable tbl = new DataTable();
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            foreach (DataColumn column in dtSL.Columns)
            {
                if (column.ColumnName.Contains("@"))
                {
                    tbl.Columns.Add(column.ColumnName, typeof(float));
                }
            }
            /*  foreach (DataRow rowSL in dtSL.Rows)
              {
                  string dauSizeID = rowSL["DauSizeID"].ToString();

                  bool dauSizeIDExists = tbl.AsEnumerable().Any(r => r["DauSizeID"].ToString() == dauSizeID);

                  if (!dauSizeIDExists)
                  {
                      DataRow newRow = tbl.NewRow();
                      newRow["DauSizeID"] = dauSizeID;
                      newRow["DauSize"] = rowSL["DauSize"];
                      foreach (DataColumn column in dtSL.Columns)
                      {
                          if (column.ColumnName.Contains("@"))
                          {
                              newRow[column.ColumnName] = 0;
                          }
                      }
                      tbl.Rows.Add(newRow);
                  }
              }*/
            // Lấy danh sách DauSizeID có trong dtDM
            var validDauSizeIDs = new HashSet<string>(
                             dtDM.AsEnumerable()
                                 .Select(row => row["DauSizeID"].ToString())
                                 .Distinct()
                         );

            foreach (DataRow rowSL in dtSL.Rows)
            {
                string dauSizeID = rowSL["DauSizeID"].ToString();

                // Chỉ thêm nếu DauSizeID có trong dtDM
                if (validDauSizeIDs.Contains(dauSizeID))
                {
                    bool dauSizeIDExists = tbl.AsEnumerable().Any(r => r["DauSizeID"].ToString() == dauSizeID);
                    if (!dauSizeIDExists)
                    {
                        DataRow newRow = tbl.NewRow();
                        newRow["DauSizeID"] = dauSizeID;
                        newRow["DauSize"] = rowSL["DauSize"];
                        foreach (DataColumn column in dtSL.Columns)
                        {
                            if (column.ColumnName.Contains("@"))
                            {
                                newRow[column.ColumnName] = 0;
                            }
                        }
                        tbl.Rows.Add(newRow);
                    }
                }
            }
            GridView view = gVThongSo;
            object MaMau = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                MaMau = view.GetRowCellValue(childHandle, gridColumn2);
            }
            else
            {
                MaMau = view.GetFocusedRowCellValue(gridColumn2);
            }
            if (MaMau == null) return;
            var mausp = MaMau.ToString()
                           .Split('|')
                           .Select(m => m.Trim())
                           .ToList();
            var distinctDauSizeIds = dtDM.AsEnumerable().Select(row => row["DauSizeID"].ToString())
                                                              .Distinct()
                                                              .ToList();
            var columnsWithAt = dtSL.Columns.Cast<DataColumn>()
                                            .Where(col => col.ColumnName.Contains("@"))
                                            .ToList();
            foreach (DataRow rowDM in dtDM.Rows)
            {
                foreach (var column in columnsWithAt)
                {

                    string dauSizeIDDM = rowDM["DauSizeID"].ToString();
                    decimal totalQuantityInSL = dtSL.AsEnumerable()
                                                       .Where(row => mausp.Contains(row["MaMau"]) &&
                                                                    row["DauSizeID"].ToString() == dauSizeIDDM.ToString())
                                                       .Sum(row =>
                                                           decimal.TryParse(row[column].ToString(), out decimal value)
                                                           ? Math.Round(value, 4)
                                                           : 0);
                    if (totalQuantityInSL > 0)
                    {
                        var rowDMs = dtDM.AsEnumerable()
                                        .FirstOrDefault(row => row["DauSizeID"].ToString() == dauSizeIDDM.ToString() && rowDM.Table.Columns.Contains(column.ColumnName));

                        if (rowDM != null && rowDM.Table.Columns.Contains(column.ColumnName))
                        {
                            decimal quantityInDM = Math.Round(Convert.ToDecimal(rowDM[column.ColumnName]), 4);
                            decimal totalQuantity = totalQuantityInSL * quantityInDM;
                            DataRow[] rowsInTbl = tbl.Select($"DauSizeID = '{dauSizeIDDM}'");

                            foreach (DataRow rowTbl in rowsInTbl)
                            {
                                foreach (DataColumn col in rowTbl.Table.Columns)
                                {
                                    if (col.ToString() == column.ToString())
                                    {

                                        rowTbl[column.ColumnName] = totalQuantity;
                                    }
                                }

                            }
                        }
                    }
                }

            }



            createTableCTCP(tbl);
            gridControl2.DataSource = tbl;
        }
        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LuuBang();
        }

        private void gridView1_FocusedRowChanged_2(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = gridView1.GetFocusedDataRow();
            if (dr == null) return;
            string _mavtid = dr["MaVTID"].ToString();
            string _mauID = dr["MauID"].ToString();
            string _mausp = dr["MaMau"].ToString();
            string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&madot={4}&&mauID={5}&&tachmau={6}&&manhom={7}&&KhoVaiID={8}&&macode={9}", URL + "DonHangTong/GetChiTietDM", _makh.ToString(),
                     _mahang.ToString(), _mavtid == null ? "" : _mavtid, searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(), _mauID.ToString() == "" ? "" : _mauID.ToString(), dr["TachMau"].ToString(), dr["MaNhom"].ToString(), dr["KhoVaiID"].ToString(), dr["MaCode"].ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dtDM = JsonConvert.DeserializeObject<DataTable>(json);

            //DataTable dtDM = this.gridControl1.DataSource as DataTable;
            DataTable dtSL = this.gridControl4.DataSource as DataTable;
            DataTable tbl = new DataTable();
            tbl.Columns.Add("DauSizeID", typeof(string));
            tbl.Columns.Add("DauSize", typeof(string));
            tbl.Columns.Add("TenMau", typeof(string));
            foreach (DataColumn column in dtSL.Columns)
            {
                if (column.ColumnName.Contains("@"))
                {
                    tbl.Columns.Add(column.ColumnName, typeof(float));
                }
            }

            var validDauSizeIDs = new HashSet<string>(
                             dtDM.AsEnumerable()
                                 .Select(row => row["DauSizeID"].ToString())
                                 .Distinct()
                         );

            foreach (DataRow rowSL in dtSL.Rows)
            {
                string dauSizeID = rowSL["DauSizeID"].ToString();

                // Chỉ thêm nếu DauSizeID có trong dtDM
                if (validDauSizeIDs.Contains(dauSizeID))
                {
                    if (dr["TachMau"].ToString() == "1")
                    {
                        bool dauSizeIDExists = tbl.AsEnumerable().Any(r => r["DauSizeID"].ToString() == dauSizeID && r["TenMau"].ToString() == rowSL["TenMau"].ToString());
                        if (!dauSizeIDExists)
                        {
                            DataRow newRow = tbl.NewRow();
                            newRow["DauSizeID"] = dauSizeID;
                            newRow["DauSize"] = rowSL["DauSize"];
                            if (dr["TachMau"].ToString() == "1")
                            {
                                newRow["TenMau"] = rowSL["TenMau"];
                            }
                            else
                            {
                                newRow["TenMau"] = "";
                            }
                            foreach (DataColumn column in dtSL.Columns)
                            {
                                if (column.ColumnName.Contains("@"))
                                {
                                    newRow[column.ColumnName] = 0;
                                }
                            }
                            tbl.Rows.Add(newRow);
                        }
                    }
                    else
                    {
                        bool dauSizeIDExists = tbl.AsEnumerable().Any(r => r["DauSizeID"].ToString() == dauSizeID);
                        if (!dauSizeIDExists)
                        {
                            DataRow newRow = tbl.NewRow();
                            newRow["DauSizeID"] = dauSizeID;
                            newRow["DauSize"] = rowSL["DauSize"];
                            if (dr["TachMau"].ToString() == "1")
                            {
                                newRow["TenMau"] = rowSL["TenMau"];
                            }
                            else
                            {
                                newRow["TenMau"] = "";
                            }
                            foreach (DataColumn column in dtSL.Columns)
                            {
                                if (column.ColumnName.Contains("@"))
                                {
                                    newRow[column.ColumnName] = 0;
                                }
                            }
                            tbl.Rows.Add(newRow);
                        }

                    }
                }
            }
            GridView view = gVThongSo;
            object MaMau = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                MaMau = view.GetRowCellValue(childHandle, gridColumn2);
            }
            else
            {
                MaMau = view.GetFocusedRowCellValue(gridColumn2);
            }
            if (_mausp == null) return;
            var mausp = _mausp.ToString()
                           .Split('|')
                           .Select(m => m.Trim())
                           .ToList();
            var distinctDauSizeIds = dtDM.AsEnumerable().Select(row => row["DauSizeID"].ToString())
                                                              .Distinct()
                                                              .ToList();
            var columnsWithAt = dtSL.Columns.Cast<DataColumn>()
                                            .Where(col => col.ColumnName.Contains("@"))
                                            .ToList();
            foreach (DataRow rowDM in dtDM.Rows)
            {
                foreach (var column in columnsWithAt)
                {

                    string dauSizeIDDM = rowDM["DauSizeID"].ToString();
                    decimal totalQuantityInSL = dtSL.AsEnumerable()
                                                       .Where(row => mausp.Contains(row["MaMau"]) &&
                                                                    row["DauSizeID"].ToString() == dauSizeIDDM.ToString())
                                                       .Sum(row =>
                                                           decimal.TryParse(row[column].ToString(), out decimal value)
                                                           ? Math.Round(value, 4)
                                                           : 0);
                    if (totalQuantityInSL > 0)
                    {
                        var rowDMs = dtDM.AsEnumerable()
                                        .FirstOrDefault(row => row["DauSizeID"].ToString() == dauSizeIDDM.ToString() && rowDM.Table.Columns.Contains(column.ColumnName));

                        if (rowDM != null && rowDM.Table.Columns.Contains(column.ColumnName))
                        {
                            decimal quantityInDM = Math.Round(Convert.ToDecimal(rowDM[column.ColumnName]), 4);
                            decimal totalQuantity = totalQuantityInSL * quantityInDM;
                            DataRow[] rowsInTbl = tbl.Select($"DauSizeID = '{dauSizeIDDM}'");

                            foreach (DataRow rowTbl in rowsInTbl)
                            {
                                foreach (DataColumn col in rowTbl.Table.Columns)
                                {
                                    if (col.ToString() == column.ToString())
                                    {

                                        rowTbl[column.ColumnName] = totalQuantity;
                                    }
                                }

                            }
                        }
                    }
                }

            }

            createTableCTCP(tbl);
            if (dr["TachMau"].ToString() == "1")
            {
                bandedGridColumn7.GroupIndex = 0;
            }
            else
            {
                bandedGridColumn7.GroupIndex = -1;
            }

            gridControl2.DataSource = tbl;
        }

        private void LuuBang()
        {
            try
            {
                this.ActiveControl = btn1;
                DataTable tblGrid = gridControl3.DataSource as DataTable;
                if (tblGrid == null || tblGrid.Rows.Count == 0)
                {
                    //
                    return;
                }
                //var rowsWithIssue = tblGrid.AsEnumerable()
                //                       .Where(row => row.IsNull("DinhMuc") || Convert.ToDecimal(row["DinhMuc"]) == 0)
                //                       .ToList();

                //if (rowsWithIssue.Any())
                //{
                //    MessageBox.Show("Định mức Sản xuất không bỏ trống. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}

                DataTable _dtSave = CreateTableSave(tblGrid);

                if (_dtSave == null || _dtSave.Rows.Count == 0)
                {
                    //
                    return;
                }

                List<string> duplicates = GetDuplicateRows(_dtSave);
                if (duplicates != null && duplicates.Count > 0)
                {
                    string message = "Các dòng sau bị trùng lặp:\n" + string.Join("\n", duplicates);
                    return;
                }

                clsWaitForm.ShowWaitForm(this, 2000);
                string url = string.Format("{0}", URL + "DonHangTong/PostCapPhatLSX");

                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtSave); }).Result;
                if (msResult.ToLower() == "true")
                {


                    clsWaitForm.ShowSuccessForm(this, 2000);
                    this.DialogResult = DialogResult.OK;
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private List<string> GetDuplicateRows(DataTable tblSave)
        {

            string url = string.Format("{0}?magop={1}&&malenhsanxuat={2}", URL + "DonHangTong/GetCapPhatCanDoi", _magop, _maLSX);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                return null;

            }
            DataTable tblExisting = JsonConvert.DeserializeObject<DataTable>(json);
            List<string> duplicateDetails = new List<string>();

            for (int i = 0; i < tblSave.Rows.Count; i++)
            {
                DataRow row = tblSave.Rows[i];

                string maVT = row["MaVT"].ToString();
                string maMau = row["MaMau"].ToString();
                string khoVai = row["KhoVai"].ToString();
                string maDot = row["MaDot"].ToString();

                // Tìm xem có dòng nào trong tblExisting trùng với thông tin trên không
                DataRow[] existingRows = tblExisting.Select(
                    $"MaVT = '{maVT}' AND MaMau = '{maMau}' AND KhoVai = '{khoVai}' AND MaDot = '{maDot}'");

                if (existingRows.Length > 0)
                {
                    string duplicateInfo = $"Dòng {i + 1}";
                    duplicateDetails.Add(duplicateInfo);
                }
            }

            return duplicateDetails;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    var rowData = gridView1.GetDataRow(i);
                    if (rowData == null) return;
                    string _mavtid = rowData["MaVTID"].ToString();
                    string _mauID = rowData["MauID"].ToString();
                    //  string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&madot={4}&&mauID={5}&&tachmau={6}&&manhom={7}&&KhoVaiID={8}&&macode={9}", URL + "DonHangTong/GetChiTietDM", _makh.ToString(),
                    //_mahang.ToString(), _mavtid == null ? "" : _mavtid, searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(), _mauID.ToString() == "" ? "" : _mauID.ToString(), "True", rowData["KhoVaiID"].ToString(), rowData["MaCode"].ToString());
                    //  string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    //  DataTable dtDM = JsonConvert.DeserializeObject<DataTable>(json);
                    string maVTID = rowData["MaVTID"].ToString();
                    string mauVTID = rowData["MauID"].ToString();
                    string khoVaiID = rowData["KhoVaiID"].ToString();
                    string maNhom = rowData["MaNhom"].ToString();
                    string maCode = rowData["MaCode"].ToString();

                    // Lọc các dòng trùng khớp từ tblDM
                    var matchingRows = tblDM.AsEnumerable()
                        .Where(row =>
                            row["MaVTID"].ToString() == maVTID &&
                            row["MauVTID"].ToString() == mauVTID &&
                            row["KhoVaiID"].ToString() == khoVaiID &&
                            row["MaNhom"].ToString() == maNhom &&
                            row["MaCode"].ToString() == maCode
                        );

                    // Copy về một DataTable mới
                    DataTable dtDM = matchingRows.Any() ? matchingRows.CopyToDataTable() : tblDM.Clone();
                    DataTable dtSL = this.gridControl4.DataSource as DataTable;
                    var mausp = new HashSet<string>(rowData["MaMau"].ToString()
                                                         .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                                         .Select(m => m.Trim()));

                    var distinctDauSizeIds = new HashSet<string>(dtDM.AsEnumerable()
                                                                     .Select(row => row["DauSizeID"].ToString())
                                                                     .Distinct());

                    var duLieuDM = dtDM.AsEnumerable()
                        .ToDictionary(
                            row => $"{row["DauSizeID"]}_{row["MaMau"]}",
                            row => row
                        );

                    var sizeColumns = dtSL.Columns.Cast<DataColumn>()
                                          .Where(c => c.ColumnName.Contains("@"))
                                          .Select(c => c.ColumnName)
                                          .ToList();

                    decimal totalQuantity = 0;
                    if (rowData["TachMau"].ToString() == "False")
                    {
                        foreach (DataRow rowSL in dtSL.Rows)
                        {
                            string dauSizeIdSL = rowSL["DauSizeID"].ToString();
                            string maMauSL = rowSL["MaMau"].ToString();
                            if (distinctDauSizeIds.Contains(dauSizeIdSL) && mausp.Contains(maMauSL))
                            {
                                string key = $"{dauSizeIdSL}_{rowData["MaMau"].ToString()}";
                                if (duLieuDM.TryGetValue(key, out DataRow rowDM))
                                {
                                    foreach (string columnName in sizeColumns)
                                    {
                                        if (dtDM.Columns.Contains(columnName))
                                        {
                                            var valueDM = rowDM[columnName];
                                            if (valueDM != DBNull.Value && Convert.ToDecimal(valueDM) > 0)
                                            {
                                                var valueSL = rowSL[columnName];
                                                if (valueSL != DBNull.Value)
                                                {
                                                    totalQuantity += Convert.ToDecimal(valueSL);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRow rowSL in dtSL.Rows)
                        {
                            string dauSizeIdSL = rowSL["DauSizeID"].ToString();
                            string maMauSL = rowSL["MaMau"].ToString();
                            if (distinctDauSizeIds.Contains(dauSizeIdSL) && mausp.Contains(maMauSL))
                            {
                                string key = $"{dauSizeIdSL}_{maMauSL.ToString()}";
                                if (duLieuDM.TryGetValue(key, out DataRow rowDM))
                                {
                                    foreach (string columnName in sizeColumns)
                                    {
                                        if (dtDM.Columns.Contains(columnName))
                                        {
                                            var valueDM = rowDM[columnName];
                                            if (valueDM != DBNull.Value && Convert.ToDecimal(valueDM) > 0)
                                            {
                                                var valueSL = rowSL[columnName];
                                                if (valueSL != DBNull.Value)
                                                {
                                                    totalQuantity += Convert.ToDecimal(valueSL);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    decimal totalAverage = TinhCapPhatTong(_mavtid, _mauID, rowData["MaMau"].ToString(), rowData["TachMau"].ToString(), dtDM);
                    Decimal finalAverage = totalQuantity == 0 ? 0 : Math.Round(totalAverage / totalQuantity, 4);
                    int isCL = Convert.ToBoolean(rowData["QDLe"].ToString()) ? 2 : 0;
                    if (rowData["DinhMucHaoHut"].ToString() != "" && rowData["DinhMucHaoHut"].ToString() != "0")
                    {
                        decimal _dmhh = (Convert.ToDecimal(rowData["DinhMucHaoHut"]) / 100) + 1;

                        gridView1.SetRowCellValue(i, "SoLuong", totalQuantity);
                        gridView1.SetRowCellValue(i, "DinhMucChung", finalAverage);
                        gridView1.SetRowCellValue(i, "CapPhatTK", Math.Round(finalAverage * _dmhh * totalQuantity, isCL, MidpointRounding.AwayFromZero));
                        gridView1.SetRowCellValue(i, "DinhMuc", finalAverage);
                        gridView1.SetRowCellValue(i, "CapPhat", Math.Round(finalAverage * _dmhh * totalQuantity, isCL, MidpointRounding.AwayFromZero));
                        gridView1.SetRowCellValue(i, "ThucXuat", Math.Round(totalAverage, isCL, MidpointRounding.AwayFromZero));
                        rowData["SLCap"] = Math.Round(finalAverage * _dmhh * totalQuantity, isCL, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        gridView1.SetRowCellValue(i, "SoLuong", totalQuantity);
                        gridView1.SetRowCellValue(i, "DinhMucChung", finalAverage);
                        gridView1.SetRowCellValue(i, "CapPhatTK", Math.Round(totalAverage, isCL, MidpointRounding.AwayFromZero));
                        gridView1.SetRowCellValue(i, "DinhMuc", finalAverage);
                        gridView1.SetRowCellValue(i, "CapPhat", Math.Round(totalAverage, isCL, MidpointRounding.AwayFromZero));
                        gridView1.SetRowCellValue(i, "ThucXuat", Math.Round(totalAverage, isCL, MidpointRounding.AwayFromZero));
                        rowData["SLCap"] = Math.Round(totalAverage, isCL, MidpointRounding.AwayFromZero);
                    }


                }
                var groupedRows = tblThongSo.AsEnumerable()
             .GroupBy(row => new
             {
                 MaVTID = row["MaVTID"].ToString(),
                 MauID = row["MauID"].ToString(),
                 KhoVaiID = row["KhoVaiID"].ToString(),
                 MaDVVT = row["MaDVVT"].ToString(),
                 MaNhom = row["MaNhom"].ToString()
             });

                foreach (var group in groupedRows)
                {
                    bool first = true;
                    foreach (var row in group.OrderBy(r => r["MaCode"].ToString()))
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            row["TonKho"] = 0;
                            row["SLCap"] = 0;
                        }
                    }
                }
                var groupedRowss = tblThongSo.AsEnumerable()
              .GroupBy(row => new
              {
                  MaVTID = row["MaVTID"].ToString(),
                  MauID = row["MauID"].ToString(),
                  KhoVaiID = row["KhoVaiID"].ToString(),
                  MaDVVT = row["MaDVVT"].ToString(),
                  MaNhom = row["MaNhom"].ToString()
              });

                foreach (var group in groupedRowss)
                {
                    var orderedRows = group.OrderBy(r => r["MaCode"].ToString()).ToList();

                    if (orderedRows.Count == 0)
                        continue;
                    var firstRow = orderedRows[0];
                    decimal tonKho = firstRow["TonKho"] != DBNull.Value ? Convert.ToDecimal(firstRow["TonKho"]) : 0;
                    decimal tongSLCap = orderedRows.Sum(r => r["SLCap"] != DBNull.Value ? Convert.ToDecimal(r["SLCap"]) : 0);
                    firstRow["SLCL"] = tonKho - tongSLCap;
                    for (int i = 1; i < orderedRows.Count; i++)
                    {
                        orderedRows[i]["TonKho"] = 0;
                        orderedRows[i]["SLCL"] = 0;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmCanDoiCapPhat frm = new frmCanDoiCapPhat();
            frm.ShowDialog();
            LoadData();
        }

        private DataTable CreateTableSave(DataTable tblGrid)
        {

            DataTable tblSave = new DataTable("tblSave");
            #region tạo cột 
            tblSave.Columns.Add("ID", typeof(long));
            tblSave.Columns.Add("MaDH", typeof(string));
            tblSave.Columns.Add("MaLenhSanXuat", typeof(string));
            tblSave.Columns.Add("MaNPL", typeof(string));
            tblSave.Columns.Add("MaVT", typeof(string));
            tblSave.Columns.Add("TenVT", typeof(string));
            tblSave.Columns.Add("MaMau", typeof(string));
            tblSave.Columns.Add("KhoVai", typeof(string));
            tblSave.Columns.Add("MaDV", typeof(string));
            tblSave.Columns.Add("DinhMuc", typeof(float));
            tblSave.Columns.Add("SoLuong", typeof(int));
            tblSave.Columns.Add("CapPhat", typeof(float));
            tblSave.Columns.Add("CapThem", typeof(float));
            tblSave.Columns.Add("ThuHoi", typeof(float));
            tblSave.Columns.Add("TrangThai", typeof(int));
            tblSave.Columns.Add("GhiChu", typeof(string));
            tblSave.Columns.Add("NguoiTao", typeof(string));
            tblSave.Columns.Add("NguoiSua", typeof(string));
            tblSave.Columns.Add("NgayTao", typeof(string));
            tblSave.Columns.Add("NgaySua", typeof(string));
            tblSave.Columns.Add("MaMauLenh", typeof(string));
            tblSave.Columns.Add("DauSizeLenh", typeof(string));
            tblSave.Columns.Add("SizeLenh", typeof(string));
            tblSave.Columns.Add("MaBom", typeof(string));
            tblSave.Columns.Add("IsXetDuyet", typeof(bool));
            tblSave.Columns.Add("NguoiXet", typeof(string));
            tblSave.Columns.Add("NguoiHuy", typeof(string));
            tblSave.Columns.Add("NgayXet", typeof(string));
            tblSave.Columns.Add("NgayHuy", typeof(string));
            tblSave.Columns.Add("MaVTMau", typeof(string));
            tblSave.Columns.Add("QtyES", typeof(float));
            tblSave.Columns.Add("CostES", typeof(float));
            tblSave.Columns.Add("NeedES", typeof(float));
            tblSave.Columns.Add("ToTalRec", typeof(float));
            tblSave.Columns.Add("ID_MaNPL", typeof(string));
            tblSave.Columns.Add("ThucNhan", typeof(float));
            tblSave.Columns.Add("TenTAVT", typeof(string));
            tblSave.Columns.Add("DinhMucHaoHut", typeof(float));
            tblSave.Columns.Add("MaNhomVT", typeof(string));
            tblSave.Columns.Add("MaVTTheoCayVai", typeof(string));
            tblSave.Columns.Add("TenLoaiVT", typeof(string));
            tblSave.Columns.Add("MaMauVT", typeof(string));
            tblSave.Columns.Add("MaKhoVT", typeof(string));
            tblSave.Columns.Add("MaDVVT", typeof(string));
            tblSave.Columns.Add("MaVTID", typeof(string));
            tblSave.Columns.Add("MaDot", typeof(string));
            tblSave.Columns.Add("ThucXuat", typeof(string));
            tblSave.Columns.Add("MauSP", typeof(string));
            tblSave.Columns.Add("DinhMucChung", typeof(decimal));
            tblSave.Columns.Add("CapPhatTK", typeof(decimal));
            tblSave.Columns.Add("NhuCau", typeof(decimal));
            tblSave.Columns.Add("MaVTGhep", typeof(string));
            tblSave.Columns.Add("TachMauSPDH", typeof(int));
            tblSave.Columns.Add("MaCode", typeof(string));
            #endregion
            foreach (DataRow row in tblGrid.Rows)
            {
                DataRow dr = tblSave.NewRow();
                if (((row["MaCLCT"] as string) ?? "") == "")
                {
                    XtraMessageBox.Show("Vui lòng chọn đầy đủ chủng loại chi tiết trước khi lưu!",
                           "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }
                dr["ID"] = row["ID"];
                dr["MaDH"] = _magop;
                dr["MaLenhSanXuat"] = _maLSX;
                dr["MaNPL"] = row["MaNPL"].ToString() == "" ? "0" : row["MaNPL"].ToString();
                dr["MaVT"] = row["MaVT"];
                dr["TenVT"] = row["ChiTiet"];
                dr["MaMau"] = row["MauVT"];
                dr["KhoVai"] = row["KhoVai"];
                dr["MaDV"] = row["TenDVVT"];
                dr["DinhMuc"] = row["DinhMuc"];
                dr["SoLuong"] = row["SoLuong"];
                dr["CapPhat"] = row["CapPhat"];
                dr["CapThem"] = row["CapThem"];
                dr["ThuHoi"] = 0;
                dr["TrangThai"] = 0;
                dr["GhiChu"] = row["GhiChu"];
                dr["NguoiTao"] = GlobleData.UserName;
                dr["NguoiSua"] = 0;
                dr["NgayTao"] = "";
                dr["NgaySua"] = "";
                dr["MaMauLenh"] = 0;
                dr["DauSizeLenh"] = 0;
                dr["SizeLenh"] = row["MaCLCT"];
                dr["MaBom"] = 0;
                dr["IsXetDuyet"] = false;
                dr["NguoiXet"] = 0;
                dr["NguoiHuy"] = 0;
                dr["NgayXet"] = "";
                dr["NgayHuy"] = "";
                dr["MaVTMau"] = 0;
                dr["QtyES"] = 0;
                dr["CostES"] = 0;
                dr["NeedES"] = 0;
                dr["ToTalRec"] = 0;
                dr["ID_MaNPL"] = 0;
                dr["ThucNhan"] = 0;
                dr["TenTAVT"] = 0;
                dr["DinhMucHaoHut"] = row["DinhMucHaoHut"];
                dr["MaNhomVT"] = row["MaNhom"];
                dr["MaVTTheoCayVai"] = 0;
                dr["TenLoaiVT"] = row["TenNhom"];
                dr["MaMauVT"] = row["MauID"];
                dr["MaKhoVT"] = row["KhoVaiID"];
                dr["MaDVVT"] = row["MaDVVT"];
                dr["MaVTID"] = row["MaVTID"];
                dr["MaDot"] = searchLookUpEditDot.EditValue.ToString();
                dr["ThucXuat"] = row["ThucXuat"];
                dr["MauSP"] = row["TenMau"];
                dr["DinhMucChung"] = row["DinhMucChung"];
                dr["CapPhatTK"] = row["CapPhatTK"];
                dr["NhuCau"] = row["NhuCau"];
                dr["MaVTGhep"] = row["MaVTGhep"];
                dr["TachMauSPDH"] = row["TachMauSPDH"];
                dr["MaCode"] = row["MaCode"];
                tblSave.Rows.Add(dr);
            }
            return tblSave;
        }


        private decimal TinhCapPhatTong(string _mavtid, string _mauID, string mausanpham, string tachmau, DataTable dtDM)
        {
            ////string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&madot={4}&&mauID={5}&&tachmau={6}", URL + "DonHangTong/GetChiTietDM", _makh.ToString(),
            ////         _mahang.ToString(), _mavtid == null ? "" : _mavtid, searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(), _mauID.ToString() == "" ? "" : _mauID.ToString(), tachmau.ToString() == "" ? "False" : tachmau.ToString());
            ////string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            //DataTable dtDM = JsonConvert.DeserializeObject<DataTable>(json);

            DataTable dtSL = this.gridControl4.DataSource as DataTable;

            DataTable tbl = new DataTable();

            var mausp = mausanpham.ToString()
                           .Split('|')
                           .Select(m => m.Trim())
                           .ToList();
            var filteredRows = dtSL.AsEnumerable()
                .Where(row => mausp.Contains(row["MaMau"].ToString()));
            if (tachmau.ToString() == "False")
            {
                tbl.Columns.Add("DauSizeID", typeof(string));
                tbl.Columns.Add("DauSize", typeof(string));
                foreach (DataColumn column in dtSL.Columns)
                {
                    if (column.ColumnName.Contains("@"))
                    {
                        tbl.Columns.Add(column.ColumnName, typeof(float));
                    }
                }
                var validDauSizeIDs = new HashSet<string>(
                                 dtDM.AsEnumerable()
                                     .Select(row => row["DauSizeID"].ToString())
                                     .Distinct()
                             );

                foreach (DataRow rowSL in dtSL.Rows)
                {
                    string dauSizeID = rowSL["DauSizeID"].ToString();


                    // Chỉ thêm nếu DauSizeID có trong dtDM
                    if (validDauSizeIDs.Contains(dauSizeID))
                    {
                        bool dauSizeIDExists = tbl.AsEnumerable().Any(r => r["DauSizeID"].ToString() == dauSizeID);
                        if (!dauSizeIDExists)
                        {
                            DataRow newRow = tbl.NewRow();
                            newRow["DauSizeID"] = dauSizeID;
                            newRow["DauSize"] = rowSL["DauSize"];
                            foreach (DataColumn column in dtSL.Columns)
                            {
                                if (column.ColumnName.Contains("@"))
                                {
                                    newRow[column.ColumnName] = 0;
                                }
                            }
                            tbl.Rows.Add(newRow);
                        }
                    }
                }
                DataTable dtSL_Filtered = filteredRows.Any()
                    ? filteredRows.CopyToDataTable()
                    : dtSL.Clone();

                var distinctDauSizeIds = dtDM.AsEnumerable().Select(row => row["DauSizeID"].ToString())
                                                                  .Distinct()
                                                                  .ToList();
                var columnsWithAt = dtSL_Filtered.Columns.Cast<DataColumn>()
                                                .Where(col => col.ColumnName.Contains("@"))
                                                .ToList();
                foreach (DataRow rowDM in dtDM.Rows)
                {
                    foreach (var column in columnsWithAt)
                    {

                        string dauSizeIDDM = rowDM["DauSizeID"].ToString();
                        decimal totalQuantityInSL = dtSL_Filtered.AsEnumerable()
                                                           .Where(row => mausp.Contains(row["MaMau"]) &&
                                                                        row["DauSizeID"].ToString() == dauSizeIDDM.ToString())
                                                           .Sum(row =>
                                                               decimal.TryParse(row[column].ToString(), out decimal value)
                                                               ? Math.Round(value, 4)
                                                               : 0);
                        if (totalQuantityInSL > 0)
                        {
                            var rowDMs = dtDM.AsEnumerable()
                                            .FirstOrDefault(row => row["DauSizeID"].ToString() == dauSizeIDDM.ToString() && rowDM.Table.Columns.Contains(column.ColumnName));

                            if (rowDM != null && rowDM.Table.Columns.Contains(column.ColumnName))
                            {
                                decimal quantityInDM = Math.Round(Convert.ToDecimal(rowDM[column.ColumnName]), 4);
                                decimal totalQuantity = totalQuantityInSL * quantityInDM;
                                DataRow[] rowsInTbl = tbl.Select($"DauSizeID = '{dauSizeIDDM}'");

                                foreach (DataRow rowTbl in rowsInTbl)
                                {
                                    foreach (DataColumn col in rowTbl.Table.Columns)
                                    {
                                        if (col.ToString() == column.ToString())
                                        {

                                            rowTbl[column.ColumnName] = totalQuantity;
                                        }
                                    }

                                }
                            }
                        }
                    }

                }
            }
            else
            {
                tbl.Columns.Add("DauSizeID", typeof(string));
                tbl.Columns.Add("DauSize", typeof(string));
                tbl.Columns.Add("MaMau", typeof(string));
                foreach (DataColumn column in dtSL.Columns)
                {
                    if (column.ColumnName.Contains("@"))
                    {
                        tbl.Columns.Add(column.ColumnName, typeof(float));
                    }
                }
                var validDauSizeIDs = new HashSet<string>(
                                 dtDM.AsEnumerable()
                                     .Select(row => row["DauSizeID"].ToString())
                                     .Distinct()
                             );

                foreach (DataRow rowSL in dtSL.Rows)
                {
                    string dauSizeID = rowSL["DauSizeID"].ToString();
                    string maMau = rowSL["MaMau"].ToString();


                    // Chỉ thêm nếu DauSizeID có trong dtDM
                    if (validDauSizeIDs.Contains(dauSizeID))
                    {
                        bool dauSizeIDExists = tbl.AsEnumerable().Any(r => r["DauSizeID"].ToString() == dauSizeID && r["MAMau"].ToString() == maMau);
                        if (!dauSizeIDExists)
                        {
                            DataRow newRow = tbl.NewRow();
                            newRow["DauSizeID"] = dauSizeID;
                            newRow["DauSize"] = rowSL["DauSize"];
                            newRow["MaMau"] = rowSL["MaMau"];
                            foreach (DataColumn column in dtSL.Columns)
                            {
                                if (column.ColumnName.Contains("@"))
                                {
                                    newRow[column.ColumnName] = 0;
                                }
                            }
                            tbl.Rows.Add(newRow);
                        }
                    }
                }
                foreach (var mamau in mausp)
                {
                    var filteredRowsByMau = dtSL.AsEnumerable()
                        .Where(row => row["MaMau"].ToString() == mamau)
                        .ToList();

                    DataTable dtSL_Filtered_ByMau = filteredRowsByMau.Any()
                        ? filteredRowsByMau.CopyToDataTable()
                        : dtSL.Clone();

                    var columnsWithAt = dtSL_Filtered_ByMau.Columns.Cast<DataColumn>()
                        .Where(col => col.ColumnName.Contains("@"))
                        .ToList();

                    foreach (DataRow rowDM in dtDM.AsEnumerable().Where(r => r["MaMau"].ToString() == mamau))
                    {
                        string dauSizeIDDM = rowDM["DauSizeID"].ToString();

                        foreach (var column in columnsWithAt)
                        {
                            decimal totalQuantityInSL = dtSL_Filtered_ByMau.AsEnumerable()
                                .Where(row => row["DauSizeID"].ToString() == dauSizeIDDM)
                                .Sum(row =>
                                    decimal.TryParse(row[column].ToString(), out decimal value)
                                        ? Math.Round(value, 4)
                                        : 0);

                            if (totalQuantityInSL > 0)
                            {
                                decimal quantityInDM = rowDM.Table.Columns.Contains(column.ColumnName)
                                    ? Math.Round(Convert.ToDecimal(rowDM[column.ColumnName]), 4)
                                    : 0;

                                decimal totalQuantity = totalQuantityInSL * quantityInDM;

                                DataRow[] rowsInTbl = tbl.Select($"DauSizeID = '{dauSizeIDDM}' AND MaMau = '{mamau}'");

                                foreach (DataRow rowTbl in rowsInTbl)
                                {
                                    if (rowTbl.Table.Columns.Contains(column.ColumnName))
                                    {
                                        rowTbl[column.ColumnName] = totalQuantity;
                                    }
                                }
                            }
                        }
                    }
                }

            }

            decimal sum = 0;
            foreach (DataColumn dc in tbl.Columns)
            {
                if (dc.ColumnName.Contains("@"))
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        if (decimal.TryParse(row[dc].ToString(), out decimal value))
                        {
                            sum += value;
                        }
                    }
                }
            }
            return sum;
        }

        private void gridView1_ShownEditor(object sender, EventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            var column = view.FocusedColumn;

            if (column.FieldName == "DinhMuc" || column.FieldName == "DinhMucHaoHut" || column.FieldName == "CapPhat" || column.FieldName == "CapPhatTK" || column.FieldName == "DinhMucChung" || column.FieldName == "ThucXuat" || column.FieldName == "CapThem" || column.FieldName == "NhuCau")
            {
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
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            NapLai();
        }
        private void NapLai()
        {
            if (!isTinh)
            {
                //tblChiTiet.Clear();
                //gridControl2.DataSource = null;
                //gridControl3.DataSource = null;
                CreateSearchLookUpDot();
                LoadData();
                // loadCapPhat();
                // loaddataCanDoi();
                LoadCanDoi();

            }
            else
            {
                LoadData();
                //loadGetCapPhatDotCanDoi();
                loadCapPhat();
                loaddataCanDoi();
            }

        }

        private void gridView1_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void bandedGridView3_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gridView1_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            e.Appearance.ForeColor = Color.Red;
        }

        #region select filer
        private HashSet<DataRow> selectedRows = new HashSet<DataRow>();
        private void gVThongSo_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                int rowHandle = e.ControllerRow;
                if (view == null) return;
                if (rowHandle < 0) return;
                object checkTinhValue = view.GetRowCellValue(rowHandle, "CheckTinh");
                if (checkTinhValue != null && Convert.ToInt32(checkTinhValue) == 1)
                {
                    if (!view.IsRowSelected(rowHandle))
                    {
                        view.SelectRow(rowHandle);
                        return;
                    }
                }
                if (rowHandle >= 0)
                {
                    DataRow row = view.GetDataRow(rowHandle);

                    if (view.IsRowSelected(rowHandle))
                    {
                        selectedRows.Add(row);
                    }
                    else
                    {
                        selectedRows.Remove(row);
                    }
                }

            }
            catch (Exception ex) { }
        }
        private void gVThongSo_ColumnFilterChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            for (int i = 0; i < view.RowCount; i++)
            {
                DataRow row = view.GetDataRow(i);
                if (row != null && selectedRows.Contains(row))
                {
                    view.SelectRow(i);
                }
            }
        }

        #endregion
        int quidoi = 1;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            if ((bool)checkBox1.Checked == true)
            {
                quidoi = 0;
                DataTable _dt = gridControl3.DataSource as DataTable;
                foreach (DataRow dr in _dt.Rows)
                {

                    dr["CapPhat"] = dr["CapPhat"] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(dr["CapPhat"]), quidoi, MidpointRounding.AwayFromZero);
                    dr["ThucXuat"] = dr["ThucXuat"] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(dr["ThucXuat"]), quidoi, MidpointRounding.AwayFromZero);

                }
            }
            else
            {
                quidoi = 2;
                DataTable _dt = gridControl3.DataSource as DataTable;
                foreach (DataRow dr in _dt.Rows)
                {
                    tinhCapPhat(dr);
                }


            }
            ganLaiNhuCau();
        }



        bool isTinh = false;
        private void loadGetCapPhatDotCanDoi()
        {
            string url = string.Format("{0}?magop={1}&&malenhsanxuat={2}", URL + "DonHangTong/GetCapPhatDotCanDoi", _magop.ToString(),
                    _maLSX.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable dtDot = JsonConvert.DeserializeObject<DataTable>(json);
            if (dtDot == null || dtDot.Rows.Count == 0)
            {
                return;
            }
            searchLookUpEditDot.EditValue = dtDot.Rows[0]["MaDot"].ToString();
            searchLookUpEditDot.Properties.ReadOnly = true;
            isTinh = true;
            loadCapPhat();
        }

        private void gVThongSo_RowStyle(object sender, RowStyleEventArgs e)
        {
            //GridView view = sender as GridView;
            //if (e.RowHandle >= 0)
            //{
            //    string Strflth = view.GetRowCellDisplayText(e.RowHandle, view.Columns["Status"]);
            //    if (Strflth == "1")
            //    {
            //        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#84e184");
            //        e.HighPriority = true;
            //    }
            if (e.RowHandle == gVThongSo.FocusedRowHandle)
            {
                e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ddd");
                e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                e.HighPriority = true;
            }

        }

        private void gVThongSo_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.RowHandle >= 0)
            {
                string Strflth = view.GetRowCellDisplayText(e.RowHandle, view.Columns["Status"]);
                string Strflths = view.GetRowCellDisplayText(e.RowHandle, view.Columns["CheckTinh"]);
                if (Strflth == "1" && Strflths == "0")
                {
                    if (e.Column.VisibleIndex >= 0 && e.Column.VisibleIndex <= 7)
                    {
                        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#FFE4E9");

                    }
                }
                if (Strflth == "1" && Strflths == "1")
                {
                    if (e.Column.VisibleIndex >= 0 && e.Column.VisibleIndex <= 7)
                    {
                        e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#F0FFF0");

                    }
                }
                if (Strflth == "0")
                {
                    if (e.Column.VisibleIndex >= 7 && e.Column.VisibleIndex <= 7)
                    {
                        e.Appearance.ForeColor = Color.Red;

                    }
                }

            }
        }

        private void bandedGridView2_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;


            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            if (info.Column == bandedGridColumn7)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
        }
        // private bool timAll = false;
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                gVThongSo.ActiveFilterString = "[Status] = '1'";
            }
            else
            {

                gVThongSo.ActiveFilterString = string.Empty;
            }
            //timAll = (bool)checkBox2.Checked;
            //gVThongSo.RefreshData();
        }

        private void loadCapPhat()
        {
            string url = string.Format("{0}?magop={1}&&malenhsanxuat={2}&&madot={3}", URL + "DonHangTong/GetCapPhatCanDoi", _magop.ToString(),
                   _maLSX.ToString(), searchLookUpEditDot.EditValue ?? "");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tblChiTiet = JsonConvert.DeserializeObject<DataTable>(json);
            // Danh sách các cột cần có
            string[] requiredColumns = {
                "ID", "MaKH", "MaHang", "MaVTID", "MaNhom", "TenNhom", "TenVT", "ChiTiet",
                "MaVT", "MaDot", "Dot", "MauID", "MaMau", "TenMau", "MaMauVT", "MauVT",
                "KhoVaiID", "KhoVai", "MaDVVT", "TenDVVT", "SoLuong", "DinhMuc", "CapPhat",
                "CapThem", "GhiChu", "Sort", "NhuCau", "MaVTGhep","MaCLCT"
            };

            // Kiểm tra và thêm cột nếu thiếu (mặc định kiểu string, riêng "SoLuong", "DinhMuc", "CapPhat", "CapThem" có kiểu số)
            foreach (string column in requiredColumns)
            {
                if (!tblChiTiet.Columns.Contains(column))
                    tblChiTiet.Columns.Add(column, column == "SoLuong" ? typeof(int) :
                                                 column == "DinhMuc" || column == "CapPhat" || column == "CapThem" ? typeof(decimal) :
                                                 typeof(string));
            }

            // Gán dữ liệu vào GridControl
            gridControl3.DataSource = tblChiTiet;
        }

        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (!isTinh)
            {
                DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
                if (gridView1.RowCount > 0)
                {
                    if (e.Menu == null)
                        return;
                    e.Menu.Items.Clear();

                    if (e.HitInfo.InRow)
                    {
                        if (e.HitInfo.Column != null)
                        {

                            DevExpress.Utils.Menu.DXMenuItem menuDeleteMauItem = new DevExpress.Utils.Menu.DXMenuItem("Xóa Dòng", ItemDelete_Click1);
                            e.Menu.Items.Add(menuDeleteMauItem);
                            DevExpress.Utils.Menu.DXMenuItem menuTachMau = new DevExpress.Utils.Menu.DXMenuItem("Tách màu sản phẩm", ItemTachMau_Click);
                            e.Menu.Items.Add(menuTachMau);
                        }

                    }
                }
            }

        }
        private void ShowColorSelector(GridView view, int rowHandle)
        {
            DataRow row = view.GetDataRow(rowHandle);
            if (row == null) return;
            string maVTID = row["MaVTID"].ToString();
            string mauVTID = row["MauID"].ToString();
            string khoVaiID = row["KhoVaiID"].ToString();
            string maNhom = row["MaNhom"].ToString();
            // Lấy DataRow trong tblThongSo thỏa điều kiện
            DataRow[] matchedRows = tblThongSo.Select(
                $"MaVTID = '{maVTID}' AND MauID = '{mauVTID}' AND KhoVaiID = '{khoVaiID}' AND MaNhom = '{maNhom}' AND MaCode = '{row["MaCode"].ToString()}'"
            );
            DataRow matchedRow = matchedRows.FirstOrDefault();

            if (matchedRow != null)
            {
                string[] maMaus = matchedRow["MaMau"].ToString().Split('|');
                string[] tenMaus = matchedRow["TenMau"].ToString().Split('|');

                int count = Math.Min(maMaus.Length, tenMaus.Length);
                DataTable colorTable = new DataTable();
                colorTable.Columns.Add("MaMau", typeof(string));
                colorTable.Columns.Add("TenMau", typeof(string));

                for (int i = 0; i < count; i++)
                {
                    colorTable.Rows.Add(maMaus[i].Trim(), tenMaus[i].Trim());
                }
                frmTachMauCapPhat frm = new frmTachMauCapPhat(colorTable, row);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (frm._mamau.ToString() != "" && frm._tenmau.ToString() != "")
                    {
                        row["MaMau"] = frm._mamau;
                        row["TenMau"] = frm._tenmau;
                        //row["TachMauSPDH"] = 1;
                        string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&madot={4}&&mauID={5}&&tachmau={6}&&manhom={7}&&KhoVaiID={8}&&macode={9}", URL + "DonHangTong/GetChiTietDM", _makh.ToString(),
                        _mahang.ToString(), maVTID == null ? "" : maVTID, searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(), mauVTID.ToString() == "" ? "" : mauVTID.ToString(), "True", matchedRow["MaNhom"].ToString(), matchedRow["KhoVaiID"].ToString(), matchedRow["MaCode"].ToString());
                        string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;


                        DataTable dtDM = JsonConvert.DeserializeObject<DataTable>(json);
                        DataTable dtSL = this.gridControl4.DataSource as DataTable;
                        var mausp = new HashSet<string>(frm._mamau.ToString()
                                                         .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                                         .Select(m => m.Trim()));

                        var distinctDauSizeIds = new HashSet<string>(
                            dtDM.AsEnumerable()
                                .Select(rows => rows["DauSizeID"].ToString())
                                .Distinct()
                        );

                        var duLieuDM = dtDM.AsEnumerable()
                                           .ToDictionary(
                                               rows => $"{rows["DauSizeID"]}_{rows["MaMau"]}",
                                               rows => rows
                                           );

                        var sizeColumns = dtSL.Columns.Cast<DataColumn>()
                                               .Where(c => c.ColumnName.Contains("@"))
                                               .Select(c => c.ColumnName)
                                               .ToList();

                        decimal totalQuantity = 0;
                        foreach (DataRow rowSL in dtSL.Rows)
                        {
                            string dauSizeIdSL = rowSL["DauSizeID"]?.ToString();
                            string maMauSL = rowSL["MaMau"]?.ToString();

                            if (distinctDauSizeIds.Contains(dauSizeIdSL) && mausp.Contains(maMauSL))
                            {
                                var rowDM = duLieuDM.Values.FirstOrDefault(rowss =>
                                {
                                    var maMauField = rowss["MaMau"]?.ToString();
                                    return rowss["DauSizeID"]?.ToString() == dauSizeIdSL &&
                                           maMauField != null &&
                                           maMauField.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Contains(maMauSL);
                                });

                                if (rowDM != null)
                                {
                                    foreach (string columnName in sizeColumns)
                                    {
                                        if (dtDM.Columns.Contains(columnName))
                                        {
                                            var valueDM = rowDM[columnName];
                                            if (valueDM != DBNull.Value && Convert.ToDecimal(valueDM) > 0)
                                            {
                                                var valueSL = rowSL[columnName];
                                                if (valueSL != DBNull.Value)
                                                {
                                                    totalQuantity += Convert.ToDecimal(valueSL);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        decimal totalAverage = TinhCapPhatTong(maVTID.ToString(), mauVTID.ToString(), frm._mamau.ToString(), frm._tenmau.ToString(), dtDM);
                        Decimal finalAverage = totalQuantity == 0 ? 0 : Math.Round(totalAverage / totalQuantity, 4);
                        row["DinhMuc"] = finalAverage;
                        row["DinhMucChung"] = finalAverage;
                        row["SoLuong"] = totalQuantity;
                        row["TachMauSPDH"] = 1;
                        if (row["DinhMucHaoHut"].ToString() != "" && row["DinhMucHaoHut"].ToString() != "0")
                        {
                            decimal _dmhh = (Convert.ToDecimal(row["DinhMucHaoHut"]) / 100) + 1;
                            row["CapPhatTK"] = Math.Round(finalAverage * _dmhh * totalQuantity, 2);
                            row["CapPhat"] = Math.Round(finalAverage * _dmhh * totalQuantity, 2);
                        }
                        else
                        {
                            row["CapPhatTK"] = Math.Round(totalAverage, 2);
                            row["CapPhat"] = Math.Round(totalAverage, 2);
                        }
                        view.RefreshRow(rowHandle);
                    }

                }
            }
        }
        private void ItemTachMau_Click(object sender, EventArgs e)
        {
            GridView view = gridView1;
            int rowHandle = view.FocusedRowHandle;

            if (rowHandle >= 0)
            {
                ShowColorSelector(view, rowHandle);

            }

        }
        private void ItemDelete_Click1(object sender, EventArgs e)
        {
            if (gridView1.SelectedRowsCount > 0)
            {
                GridView view = gridView1;
                object MaVTID = null, MauID = null;

                if (view.IsGroupRow(view.FocusedRowHandle))
                {
                    int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                    MaVTID = view.GetRowCellValue(childHandle, colMaVTID);
                    MauID = view.GetRowCellValue(childHandle, colMauID);
                }
                else
                {
                    MaVTID = view.GetFocusedRowCellValue(colMaVTID);
                    MauID = view.GetFocusedRowCellValue(colMauID);
                }
                int selectedIndex = gridView1.GetSelectedRows()[0];
                DataRow row = gridView1.GetDataRow(selectedIndex);
                DataTable dt = gCThongSo.DataSource as DataTable;


                // Lấy giá trị từ dòng dr
                string maVTID = row["MaVTID"].ToString();
                string mauVTID = row["MauID"].ToString();
                string khoVaiID = row["KhoVaiID"].ToString();
                string maNhom = row["MaNhom"].ToString();
                string maCode = row["MaCode"].ToString();
                foreach (DataRow row2 in dt.Rows)
                {
                    if (row2["MaVTID"].ToString() == maVTID &&
                        row2["MauID"].ToString() == mauVTID &&
                        row2["KhoVaiID"].ToString() == khoVaiID &&
                        row2["MaNhom"].ToString() == maNhom &&
                        row2["MaCode"].ToString() == maCode)
                    {
                        row2["SLCap"] = 0;
                    }
                }

                if (row != null)
                {
                    if (row["MaNPL"] != DBNull.Value && row["MaNPL"].ToString() == "0")
                    {
                        ((DataTable)gridControl3.DataSource).Rows.Remove(row);
                    }
                    else
                    {
                        MessageBox.Show("Vật tư đã được cấp phát sản xuất. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                gridView1.RefreshData();
            }
        }

        private void gVThongSo_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            if (info.Column != null && info.Column.FieldName == "NPL" && info.EditValue.ToString() != "")
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);

                if (isChecked)
                {
                    info.GroupText = "Nguyên liệu";
                }
                else
                {
                    info.GroupText = "Phụ liệu";
                }
            }

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            //if (info.Column == gridColumn24)
            //{
            //    info.GroupText = string.Format("{0}", info.GroupValueText);
            //}
            if (info.Column == colTenNhom)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gVThongSo.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gVThongSo.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }

                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }

        }

        private void gVThongSo_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            //if (!timAll)
            //{
            //    DataRowView rowView = (DataRowView)gVThongSo.GetRow(e.ListSourceRow);
            //    if (rowView == null) return;
            //    if (!rowView["Status"].Equals("0"))
            //    {
            //        e.Visible = false;
            //        e.Handled = true;
            //    }
            //}
        }

        private void gVThongSo_CellMerge(object sender, CellMergeEventArgs e)
        {
            if (e.Column.FieldName == "TonKho" || e.Column.FieldName == "SLCL")
            {
                var view = sender as GridView;

                var row1 = view.GetDataRow(e.RowHandle1);
                var row2 = view.GetDataRow(e.RowHandle2);
                if (row1["MaVTID"].ToString() == row2["MaVTID"].ToString() &&
                    row1["KhoVaiID"].ToString() == row2["KhoVaiID"].ToString() &&
                    row1["MaDVVT"].ToString() == row2["MaDVVT"].ToString() &&
                    row1["MaNhom"].ToString() == row2["MaNhom"].ToString() &&
                    row1["MauID"].ToString() == row2["MauID"].ToString())
                {
                    e.Merge = row1["TonKho"].Equals(row2["TonKho"]);
                    e.Merge = row1["SLCL"].Equals(row2["SLCL"]);
                    e.Handled = true;
                }
                else
                {
                    e.Merge = false;
                    e.Handled = true;
                }
            }
        }

        private void gridView1_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            if (info.Column != null && info.Column.FieldName == "NPL" && info.EditValue.ToString() != "")
            {
                bool isChecked = Convert.ToBoolean(info.EditValue);

                if (isChecked)
                {
                    info.GroupText = "Nguyên liệu";
                }
                else
                {
                    info.GroupText = "Phụ liệu";
                }
            }

            string caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
                caption = info.Column.ToString();
            //if (info.Column == gridColumn24)
            //{
            //    info.GroupText = string.Format("{0}", info.GroupValueText);
            //}
            if (info.Column == gridColumn17)
            {
                info.GroupText = string.Format("{0}", info.GroupValueText);
            }
            if (gridView1.IsGroupRow(e.RowHandle))
            {


                int groupIndex = gridView1.GetRowLevel(e.RowHandle);
                Color textColor = Color.White;
                Font font = e.Appearance.Font;
                if (groupIndex == 0)
                {
                    textColor = Color.MediumBlue;
                    font = new Font(e.Appearance.Font.FontFamily, 9, FontStyle.Bold);
                }
                else if (groupIndex == 1)
                {
                    textColor = Color.Red;
                    font = new Font(e.Appearance.Font.FontFamily, 8, FontStyle.Regular);
                }

                // Gán lại thuộc tính
                e.Appearance.ForeColor = textColor;
                e.Appearance.Font = font;
                e.DefaultDraw();

                e.Handled = true;

            }
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //reCalculateNhuCau();


        }
        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            GridView view = (GridView)sender;
            if (view.FocusedColumn.FieldName.ToString() == "DinhMuc" || view.FocusedColumn.FieldName.ToString() == "ThucXuat")
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

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_makh.ToString() == "" || _mahang.ToString() == "" || searchLookUpEditDot.EditValue == null) return;
            string madot = searchLookUpEditDot.EditValue.ToString() == "" ? "" : searchLookUpEditDot.EditValue.ToString();
            frmPhanTichBOMViewV1 frm = new frmPhanTichBOMViewV1(_makh.ToString(), _mahang.ToString(), madot.ToString());
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog();
            LoaDM();
            NapLai();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            frmCapThemLSX frm = new frmCapThemLSX(_magop.ToString() == "" ? "" : _magop.ToString(), GlobleData.UserName, _maLSX.ToString() == "" ? "" : _maLSX.ToString(), _tenKH.ToString() == "" ? "" : _tenKH.ToString(),
                _mahang.ToString() == "" ? "" : _mahang.ToString(), _madh.ToString() == "" ? "" : _madh.ToString(), _malenh.ToString() == "" ? "" : _malenh.ToString(), _soluong.ToString() == "" ? "" : _soluong.ToString(),
                _tenhang.ToString() == "" ? "" : _tenhang.ToString(), _iskeove.ToString() == "" ? 0 : _iskeove, _dot.ToString() == "" ? "" : _dot.ToString(),
                _makh.ToString() == "" ? "" : _makh.ToString(), _madhgop.ToString() == "" ? "" : _madhgop.ToString(), searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString());
            frm.ShowDialog();
            loadCapPhat();
        }
        private void gVThongSo_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            //if(!isTinh)
            //{
            DevExpress.XtraGrid.Menu.GridViewColumnMenu menu = e.Menu as DevExpress.XtraGrid.Menu.GridViewColumnMenu;
            if (gVThongSo.RowCount > 0)
            {
                if (e.Menu == null)
                    return;
                e.Menu.Items.Clear();

                if (e.HitInfo.InRow)
                {
                    if (e.HitInfo.Column != null)
                    {

                        DevExpress.Utils.Menu.DXMenuItem menuDeleteMauItem = new DevExpress.Utils.Menu.DXMenuItem("Cấp phát", ItemCapPhat_Click);
                        e.Menu.Items.Add(menuDeleteMauItem);
                    }

                }
            }
            //}    

        }



        private void ItemCapPhat_Click(object sender, EventArgs e)
        {
            try
            {

                DataTable tbl = gCThongSo.DataSource as DataTable;
                if (tbl == null || tbl.Rows.Count == 0) return;

                //for (int i = 0; i < tbl.Rows.Count; i++)
                //{
                //    if (gVThongSo.GetDataRow(i)["Status"].ToString() == "0") continue;
                //    object checkTinhValue = gVThongSo.GetRowCellValue(i, "CheckTinh");
                //if (gVThongSo.IsRowSelected(i))// && checkTinhValue != null && checkTinhValue.ToString() == "0")
                //{
                var rowData = gVThongSo.GetFocusedDataRow();
                if (rowData["Status"].ToString() == "0") return;
                bool isDuplicate = tblChiTiet.AsEnumerable().Any(row =>
                 (row.Field<string>("MaVTID") ?? "") == (rowData["MaVTID"]?.ToString() ?? "") &&
                 (row.Field<string>("MaNhom") ?? "") == (rowData["MaNhom"]?.ToString() ?? "") &&
                 (row.Field<string>("KhoVaiID") ?? "") == (rowData["KhoVaiID"]?.ToString() ?? "") &&
                 (row.Field<string>("MaDVVT") ?? "") == (rowData["MaDVVT"]?.ToString() ?? "") &&
                 (row.Field<string>("MauID") ?? "") == (rowData["MauID"]?.ToString() ?? "") &&
                 (row.Field<string>("MaDot") ?? "") == (rowData["MaDot"]?.ToString() ?? "") &&
                 (row.Field<string>("Macode") ?? "") == (rowData["Macode"]?.ToString() ?? "")
             );
                if (!isDuplicate)
                {
                    string _mavtid = rowData["MaVTID"].ToString();
                    string _mauID = rowData["MauID"].ToString();
                    string url = string.Format("{0}?makh={1}&&mahang={2}&&mavtID={3}&&madot={4}&&mauID={5}&&tachmau={6}&&manhom={7}&&KhoVaiID={8}&&macode={9}", URL + "DonHangTong/GetChiTietDM", _makh.ToString(),
                      _mahang.ToString(), _mavtid == null ? "" : _mavtid, searchLookUpEditDot.EditValue == null ? "" : searchLookUpEditDot.EditValue.ToString(), _mauID.ToString() == "" ? "" : _mauID.ToString(), rowData["TachMau"].ToString(), rowData["MaNhom"].ToString(), rowData["KhoVaiID"].ToString(), rowData["MaCode"].ToString());
                    string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                    DataTable dtDM = JsonConvert.DeserializeObject<DataTable>(json);
                    DataTable dtSL = this.gridControl4.DataSource as DataTable;
                    int rowCount = 0;

                    var mausp = new HashSet<string>(rowData["MaMau"].ToString()
                                                                          .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                                                          .Select(m => m.Trim()));

                    var distinctDauSizeIds = new HashSet<string>(dtDM.AsEnumerable()
                                                                     .Select(row => row["DauSizeID"].ToString())
                                                                     .Distinct());

                    var duLieuDM = dtDM.AsEnumerable()
                        .ToDictionary(
                            row => $"{row["DauSizeID"]}_{row["MaMau"]}",
                            row => row
                        );

                    var sizeColumns = dtSL.Columns.Cast<DataColumn>()
                                          .Where(c => c.ColumnName.Contains("@"))
                                          .Select(c => c.ColumnName)
                                          .ToList();

                    decimal totalQuantity = 0;
                    if (rowData["TachMau"].ToString() == "False")
                    {
                        foreach (DataRow rowSL in dtSL.Rows)
                        {
                            string dauSizeIdSL = rowSL["DauSizeID"].ToString();
                            string maMauSL = rowSL["MaMau"].ToString();
                            if (distinctDauSizeIds.Contains(dauSizeIdSL) && mausp.Contains(maMauSL))
                            {
                                string key = $"{dauSizeIdSL}_{rowData["MaMau"].ToString()}";
                                if (duLieuDM.TryGetValue(key, out DataRow rowDM))
                                {
                                    foreach (string columnName in sizeColumns)
                                    {
                                        if (dtDM.Columns.Contains(columnName))
                                        {
                                            var valueDM = rowDM[columnName];
                                            if (valueDM != DBNull.Value && Convert.ToDecimal(valueDM) > 0)
                                            {
                                                var valueSL = rowSL[columnName];
                                                if (valueSL != DBNull.Value)
                                                {
                                                    totalQuantity += Convert.ToDecimal(valueSL);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRow rowSL in dtSL.Rows)
                        {
                            string dauSizeIdSL = rowSL["DauSizeID"].ToString();
                            string maMauSL = rowSL["MaMau"].ToString();
                            if (distinctDauSizeIds.Contains(dauSizeIdSL) && mausp.Contains(maMauSL))
                            {
                                string key = $"{dauSizeIdSL}_{maMauSL.ToString()}";
                                if (duLieuDM.TryGetValue(key, out DataRow rowDM))
                                {
                                    foreach (string columnName in sizeColumns)
                                    {
                                        if (dtDM.Columns.Contains(columnName))
                                        {
                                            var valueDM = rowDM[columnName];
                                            if (valueDM != DBNull.Value && Convert.ToDecimal(valueDM) > 0)
                                            {
                                                var valueSL = rowSL[columnName];
                                                if (valueSL != DBNull.Value)
                                                {
                                                    totalQuantity += Convert.ToDecimal(valueSL);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    decimal totalAverage = TinhCapPhatTong(_mavtid, _mauID, rowData["MaMau"].ToString(), rowData["TachMau"].ToString(), dtDM);
                    Decimal finalAverage = totalQuantity == 0 ? 0 : Math.Round(totalAverage / totalQuantity, 4);

                    DataRow newRow = tblChiTiet.NewRow();
                    newRow["ID"] = 0;
                    newRow["MaKH"] = rowData["MaKH"];
                    newRow["MaHang"] = rowData["MaHang"];
                    newRow["MaNPL"] = 0;
                    newRow["MaVTID"] = rowData["MaVTID"];
                    newRow["MaNhom"] = rowData["MaNhom"];
                    newRow["TenNhom"] = rowData["TenNhom"];
                    newRow["TenVT"] = rowData["TenVT"];
                    newRow["ChiTiet"] = rowData["ChiTiet"];
                    newRow["MaVT"] = rowData["MaVT"];
                    newRow["MaDot"] = rowData["MaDot"];
                    newRow["Dot"] = rowData["Dot"];
                    newRow["MauID"] = rowData["MauID"];
                    newRow["MaMau"] = rowData["MaMau"];
                    newRow["TenMau"] = rowData["TenMau"];
                    newRow["MaMauVT"] = rowData["MaMauVT"];
                    newRow["MauVT"] = rowData["MauVT"];
                    newRow["KhoVaiID"] = rowData["KhoVaiID"];
                    newRow["KhoVai"] = rowData["KhoVai"];
                    newRow["MaDVVT"] = rowData["MaDVVT"];
                    newRow["TenDVVT"] = rowData["TenDVVT"];
                    newRow["DinhMucChung"] = finalAverage;
                    newRow["SoLuong"] = totalQuantity;
                    if (rowData["DinhMucHaoHut"].ToString() != "" && rowData["DinhMucHaoHut"].ToString() != "0")
                    {
                        decimal _dmhh = (Convert.ToDecimal(rowData["DinhMucHaoHut"]) / 100) + 1;
                        newRow["CapPhatTK"] = Math.Round(finalAverage * _dmhh * totalQuantity, 2);
                        newRow["CapPhat"] = Math.Round(finalAverage * _dmhh * totalQuantity, 2);
                    }
                    else
                    {
                        newRow["CapPhatTK"] = Math.Round(totalAverage, 2);
                        newRow["CapPhat"] = Math.Round(totalAverage, 2);
                    }
                    newRow["CapThem"] = 0;
                    newRow["Sort"] = rowData["Sort"];
                    newRow["DinhMucHaoHut"] = rowData["DinhMucHaoHut"];
                    newRow["NPL"] = rowData["NPL"];
                    newRow["ThucXuat"] = Convert.ToDecimal(rowData["SLCL"]) < 0 ? 0 : rowData["SLCL"];
                    newRow["DinhMuc"] = finalAverage;
                    newRow["NhuCau"] = 0;
                    newRow["TachMau"] = rowData["TachMau"];
                    newRow["MaVTGhep"] = rowData["MaVTGhep"];
                    newRow["TachMauSPDH"] = 0;
                    newRow["MaCode"] = rowData["MaCode"];
                    tblChiTiet.Rows.Add(newRow);
                    reCalculateNhuCau(newRow);
                    gridControl3.RefreshDataSource();
                }




            }
            catch (Exception ex) { }
        }
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            try
            {
                if (e.Value == null || e.Value == "") return;
                GridView view = (GridView)sender;
                DataRow drChange = view.GetFocusedDataRow();
                if (drChange == null) return;
                //Decimal _dmhh = 1, _nhucau = 1, _dm=1, sl=1;
                //if (drChange["DinhMucHaoHut"].ToString() != "0")
                //{
                //    _dmhh = Convert.ToDecimal(drChange["DinhMucHaoHut"]);
                //}
                //if (drChange["NhuCau"].ToString() != "0")
                //{
                //    _nhucau = Convert.ToDecimal(drChange["NhuCau"]);
                //}
                //if (drChange["DinhMuc"].ToString() != "0")
                //{
                //    _dm = Convert.ToDecimal(drChange["DinhMuc"]);
                //}
                //if (drChange["SoLuong"].ToString() != "0")
                //{
                //    sl = Convert.ToDecimal(drChange["SoLuong"]);
                //}



                //if (e.Column.FieldName == "DinhMuc")
                //{

                //    drChange["CapPhat"] = Math.Round(
                //        Convert.ToDecimal(e.Value) * _dmhh * Convert.ToInt32(drChange["SoLuong"]),
                //        quidoi
                //    );
                //}
                //else
                //{
                //    if (e.Column.FieldName == "DinhMucHaoHut")
                //    {
                //        drChange["CapPhat"] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(e.Value) * Convert.ToDecimal(drChange["DinhMuc"]) * _nhucau * Convert.ToInt32(drChange["SoLuong"])), quidoi);
                //    }
                //    else
                //    {
                //        if (e.Column.FieldName == "NhuCau")
                //        {

                //            drChange["CapPhat"] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(e.Value) * Convert.ToDecimal(drChange["DinhMuc"]) * _dmhh * Convert.ToInt32(drChange["SoLuong"])), quidoi);


                //        }
                //    }
                //}
                if (e.Column.FieldName != "CapPhat")
                {
                    tinhCapPhat(drChange);
                }
                else if (e.Column.FieldName == "CapPhat")
                {
                    if (drChange == null) return;
                    reCalculateNhuCau(drChange);
                }

            }
            catch (Exception ex) { }
        }
        private void tinhCapPhat(DataRow drChange)
        {

            try
            {
                Decimal _dmhh = 1, _nhucau = 0, _dm = 1, sl = 0;
                int isCL = Convert.ToBoolean(drChange["QDLe"].ToString()) ? 2 : 0;
                if (drChange["DinhMucHaoHut"].ToString() != "0" && drChange["DinhMucHaoHut"].ToString() != "")
                {
                    _dmhh += (Convert.ToDecimal(drChange["DinhMucHaoHut"]) / 100);
                }
                if (drChange["NhuCau"].ToString() != "0" && drChange["NhuCau"].ToString() != "")
                {
                    _nhucau += Convert.ToDecimal(drChange["NhuCau"]);
                }
                if (drChange["DinhMuc"].ToString() != "0" && drChange["DinhMuc"].ToString() != "")
                {
                    _dm = Convert.ToDecimal(drChange["DinhMuc"]);
                }
                if (drChange["SoLuong"].ToString() != "0" && drChange["SoLuong"].ToString() != "")
                {
                    sl = Convert.ToDecimal(drChange["SoLuong"]);
                }

                drChange["CapPhat"] = Math.Round(_dm * _dmhh * sl + _nhucau, isCL, MidpointRounding.AwayFromZero);
                if (_dmhh == 1 && _dm == 1 && sl == 1 && _nhucau == 0)
                {
                    drChange["CapPhat"] = 0;
                }
                else if (_dm == 0 && _nhucau == 0)
                {
                    drChange["CapPhat"] = 0;
                }
                reCalculateNhuCau(drChange);
            }
            catch (Exception ex) { }

        }
        private void tinhCapPhatHH(DataRow drChange)
        {

            try
            {
                Decimal _dmhh = 1, _nhucau = 0, _dm = 1, sl = 1;
                int isCL = Convert.ToBoolean(drChange["QDLe"].ToString()) ? 2 : 0;
                if (drChange["DinhMucHaoHut"].ToString() != "0" && drChange["DinhMucHaoHut"].ToString() != "")
                {
                    _dmhh += (Convert.ToDecimal(drChange["DinhMucHaoHut"]) / 100);
                }
                if (drChange["NhuCau"].ToString() != "0" && drChange["NhuCau"].ToString() != "")
                {
                    _nhucau += Convert.ToDecimal(drChange["NhuCau"]);
                }
                if (drChange["DinhMuc"].ToString() != "0" && drChange["DinhMuc"].ToString() != "")
                {
                    _dm = Convert.ToDecimal(drChange["DinhMuc"]);
                }
                if (drChange["SoLuong"].ToString() != "0" && drChange["SoLuong"].ToString() != "")
                {
                    sl = Convert.ToDecimal(drChange["SoLuong"]);
                }

                drChange["CapPhat"] = Math.Round(_dm * _dmhh * sl + _nhucau, isCL, MidpointRounding.AwayFromZero);
                reCalculateNhuCau(drChange);

            }
            catch (Exception ex) { }

        }


        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != 46)
            {
                e.Handled = true;  // Chặn ký tự không hợp lệ
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.ToString() == "") return;
                if ((bool)checkBox3.Checked == true)
                {
                    DataTable _dt = gridControl3.DataSource as DataTable;
                    if (_dt == null) return;
                    foreach (DataRow row in _dt.Rows)
                    {
                        row["DinhMucHaoHut"] = Convert.ToDecimal(textBox1.Text.ToString().Trim());
                        tinhCapPhatHH(row);

                    }
                    reCalculateCanDoi();
                }
            }
            catch (Exception ex) { }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.ToString() == "") return;

                if ((bool)checkBox3.Checked == true)
                {
                    DataTable _dt = gridControl3.DataSource as DataTable;
                    if (_dt == null) return;
                    foreach (DataRow row in _dt.Rows)
                    {
                        row["DinhMucHaoHut"] = Convert.ToDecimal(textBox1.Text.ToString().Trim());
                        tinhCapPhatHH(row);

                    }
                    reCalculateCanDoi();
                }

            }
            catch (Exception ex) { }
        }
        private void reCalculateNhuCau(DataRow dr)
        {

            //DataRow dr = gridView1.GetFocusedDataRow();
            //if (dr == null) return;
            DataTable dt = gCThongSo.DataSource as DataTable;


            // Lấy giá trị từ dòng dr
            string maVTID = dr["MaVTID"].ToString();
            string mauVTID = dr["MauID"].ToString();
            string khoVaiID = dr["KhoVaiID"].ToString();
            string maNhom = dr["MaNhom"].ToString();
            string maCode = dr["MaCode"].ToString();
            foreach (DataRow row in dt.Rows)
            {
                if (row["MaVTID"].ToString() == maVTID &&
                    row["MauID"].ToString() == mauVTID &&
                    row["KhoVaiID"].ToString() == khoVaiID &&
                    row["MaNhom"].ToString() == maNhom &&
                    row["MaCode"].ToString() == maCode)
                {
                    row["SLCap"] = Convert.ToDecimal(dr["CapPhat"]) + Convert.ToDecimal(dr["CapThem"]);
                }
            }

        }
        private void ganLaiNhuCau()
        {
            DataTable dt = gCThongSo.DataSource as DataTable;
            DataTable tbl = gridControl3.DataSource as DataTable;

            foreach (DataRow dr in tbl.Rows)
            {
                string maVTID = dr["MaVTID"].ToString();
                string mauVTID = dr["MauID"].ToString();
                string khoVaiID = dr["KhoVaiID"].ToString();
                string maNhom = dr["MaNhom"].ToString();
                string maCode = dr["MaCode"].ToString();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["MaVTID"].ToString() == maVTID &&
                        row["MauID"].ToString() == mauVTID &&
                        row["KhoVaiID"].ToString() == khoVaiID &&
                        row["MaNhom"].ToString() == maNhom &&
                        row["MaCode"].ToString() == maCode)
                    {
                        row["SLCap"] = Convert.ToDecimal(dr["CapPhat"]) + Convert.ToDecimal(dr["CapThem"]);
                    }
                }
            }
            reCalculateCanDoi();


        }
        private void reCalculateCanDoi()
        {
            DataTable tbl = gCThongSo.DataSource as DataTable;
            foreach (DataRow dr in tbl.Rows)
            {
                dr["SLCL"] = Math.Round((decimal.TryParse(dr["TonKho"]?.ToString(), out var tk) ? tk : 0) - (decimal.TryParse(dr["SLCap"]?.ToString(), out var sc) ? sc : 0), 2);

            }
        }
        #region Mạnh
        private DataTable _dtChungLoaiChiTiet; // Thêm biến toàn cục để lưu data
        private string firstSelectedTenNhom = null;
        private void InItChungLoaiChiTiet()
        {
            InIt();
            repoCL.ValueMember = "MaNhom";
            repoCL.DisplayMember = "TenNhom";
            repoCL.NullText = "[..Chủng loại CT..]";
            repoCL.Popup += RepoCL_Popup;

            GridView dvView = repoCL.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "TenNhom", Caption = "Tên nhóm chi tiết", Name = "colTenNhomCT", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "MaNhom", Caption = "Mã nhóm", Name = "colMaNhom", Visible = false }); // Thêm column này để filter
            }
            gridView1.SelectionChanged += gridView1_SelectionChanged;
            gridView1.OptionsSelection.MultiSelect = true;
            gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            LoadChungLoaiChiTiet();
        }

        private void LoadChungLoaiChiTiet()
        {
            string url = $"{URL}KhoiTaoBOMV1/Get?action=GetChiTietNhom";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            _dtChungLoaiChiTiet = JsonConvert.DeserializeObject<DataTable>(json);

            // Gán DataSource cho repoCL - THIẾU PHẦN NÀY
            repoCL.DataSource = _dtChungLoaiChiTiet;


            searchLookUpEditCLCT.Properties.DataSource = _dtChungLoaiChiTiet;
        }

        private void RepoCL_Popup(object sender, EventArgs e)
        {
            var editor = sender as SearchLookUpEdit;
            if (editor == null) return;

            var drRow = gridView1.GetFocusedDataRow();
            if (drRow == null) return;

            string tenNhom = drRow["TenNhom"]?.ToString();
            if (string.IsNullOrEmpty(tenNhom)) return;

            var popupView = editor.Properties.View;
            popupView.ActiveFilterString = $"[TenNhom] = '{tenNhom}'";
        }
        bool showedMessage = false;
        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {

            var view = sender as GridView;
            if (view == null) return;
            showedMessage = false;
            var selectedHandles = view.GetSelectedRows();
            if (firstSelectedTenNhom == null && selectedHandles.Length > 0)
            {
                DataRow firstRow = view.GetDataRow(selectedHandles[0]);
                if (firstRow != null)
                    firstSelectedTenNhom = firstRow["TenNhom"]?.ToString();
            }
            if ((Control.ModifierKeys & (Keys.Control | Keys.Shift)) == Keys.None)
            {
                DataRow firstRow = view.GetDataRow(selectedHandles[0]);
                if (firstRow != null)
                    firstSelectedTenNhom = firstRow["TenNhom"]?.ToString();
                return;
            }
            if (selectedHandles.Length == 0)
            {
                firstSelectedTenNhom = null;
                return;
            }

            // 🔴 Kiểm tra tất cả các dòng đã chọn
            foreach (int handle in selectedHandles)
            {
                DataRow row = view.GetDataRow(handle);
                if (row == null) continue;

                string tenNhom = row["TenNhom"]?.ToString();
                if (tenNhom != firstSelectedTenNhom)
                {
                    // ❌ Bỏ chọn dòng không đúng nhóm
                    view.UnselectRow(handle);

                    if (!showedMessage)
                    {
                        showedMessage = true;
                        XtraMessageBox.Show(
                            "Tất cả các dòng được chọn phải có cùng Tên Nhóm với dòng đầu tiên!",
                            "Cảnh báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                    }

                    // ❗ break để không lặp cảnh báo trong cùng lần xử lý
                    break;
                }
            }
        }
        private void InIt()
        {
            searchLookUpEditCLCT.Properties.ValueMember = "MaNhom";
            searchLookUpEditCLCT.Properties.DisplayMember = "TenNhom";
            searchLookUpEditCLCT.Properties.NullValuePrompt = "Chọn chủng loại CT";

            searchLookUpEditCLCT.Popup += SearchLookUpEditCLCT_Popup;
        }
        private void SearchLookUpEditCLCT_Popup(object sender, EventArgs e)
        {
            var editor = sender as SearchLookUpEdit;
            if (editor == null) return;
            var selectedHandles = gridView1.GetSelectedRows();
            DataRow firstRow = gridView1.GetDataRow(selectedHandles[0]);

            if (firstRow == null) return;

            string tenNhom = firstRow["TenNhom"]?.ToString();
            if (string.IsNullOrEmpty(tenNhom)) return;

            var popupView = editor.Properties.View;
            popupView.ActiveFilterString = $"[TenNhom] = '{tenNhom}'";
        }
        private void searchLookUpEditCLCT_Properties_EditValueChanged(object sender, EventArgs e)
        {
            int[] selectedHandles = gridView1.GetSelectedRows();
            string CLCT = (searchLookUpEditCLCT.EditValue as string) ?? "";
            if (CLCT == "") return;
            if (selectedHandles.Length == 0)
            {
                XtraMessageBox.Show("Vui lòng chọn ít nhất một dòng trước khi lưu!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (int handle in selectedHandles)
            {
                DataRow row = gridView1.GetDataRow(handle);
                if (row != null)
                {
                    row["MaCLCT"] = CLCT;
                }
            }

            gridView1.RefreshData();
            searchLookUpEditCLCT.EditValue = null;
        }

        #endregion
    }

}

