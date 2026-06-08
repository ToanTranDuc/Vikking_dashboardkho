using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.Kho;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.KeHoach;
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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmDongThung : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        List<DongThungThongTinEntity> lstDSNhapKho;
        List<DongThungEntity> lstCTNhapKho;
        List<DongThungThongTinPOCTEntity> lstDTPOCT;
        int sum = 0;
        private string _madh = string.Empty, _mahang = string.Empty, _poid = string.Empty, _dausizeid = string.Empty, _mamau = string.Empty, _mapkl = string.Empty, _madvsxdt = string.Empty;
        private string _madhdt = string.Empty, _mahangdt = string.Empty, _poiddt = string.Empty, _mapkldt = string.Empty, _tendvsxdt = string.Empty, _tensxdt = string.Empty, _malenhdt = string.Empty, _tenlenhdt = string.Empty, _podt = string.Empty, _malenhsx = string.Empty;
        private string _madhdtct = string.Empty, _mahangdtct = string.Empty, _poiddtct = string.Empty, _mapkldtct = string.Empty, _tendvsxdtct = string.Empty, _tensxdtct = string.Empty, _malenhdtct = string.Empty, _tenlenhdtct = string.Empty, _podtct = string.Empty,
            _dausizeiddtct = string.Empty, _maudtct = string.Empty, _malenhsxct = string.Empty, _madvsxdtct = string.Empty;
        private int _sttthung = 0, _slkhdt = 0, _slthdt = 0, _slcldt = 0, _tuthung = 0, _denthung = 0, _minsttthung = 0, _maxsttthung = 0;
        DataTable tblCT;
        DataTable tbl;
        bool indicatorIcon = true;
        int rowHandel = 0;
        int rowHandel_1 = 0;
        int rowHandel_2 = 0;
        int pageIndex = 1;
        int pageSize = 100;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool _kiemtra = false, _sms = false;
        RepositoryItemCheckEdit repositoryItemCheckEdit;
        public frmDongThung()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstDSNhapKho = new List<DongThungThongTinEntity>();
            lstCTNhapKho = new List<DongThungEntity>();
            lstDTPOCT = new List<DongThungThongTinPOCTEntity>();
            tblCT = new DataTable();
            tbl = new DataTable();
            repositoryItemCheckEdit = new RepositoryItemCheckEdit();
        }
        protected override void OnLoad(EventArgs e)
        {
            barEditItemSpinPageSizee.EditValue = pageSize;
            CheckPerminsion();
            LoadData();
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
                layoutControlItem3.Enabled = false;
                layoutControlItem4.Enabled = false;
                layoutControlItem5.Enabled = false;
                layoutControlItem6.Enabled = false;
                layoutControlItem7.Enabled = false;
                layoutControlItem8.Enabled = false;
                layoutControlItem9.Enabled = false;
                layoutControlItem10.Enabled = false;
            } 
        }


        private void LoadData()
        {
            if (!_sms)
            {
                string urlTT = string.Format("{0}?madh={1}&&pageIndex={2}&&pageSize={3}&&madvsx={4}", URL + "DongThung/GetThongTin", "", pageIndex, pageSize, string.Join(";", GlobleData.lstDVSX));
                string jsonTT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTT); }).Result;
                tbl = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                colMaDH.Caption = "Đơn hàng";
                colTenHang.Caption = "Tên hàng";
                colTenCL.Visible = true;
                //colDot.Visible = true;
                colGhiChu.Visible = true;
                //colSoLuong.Visible = true;
            }
            else
            {
                string urlTT = string.Format("{0}?madh={1}&&pageIndex={2}&&pageSize={3}&&madvsx={4}", URL + "DongThung/GetThongTinSMS", "", pageIndex, pageSize, string.Join(";", GlobleData.lstDVSX));
                string jsonTT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTT); }).Result;
                tbl = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                colMaDH.Caption = "Mã SMS";
                colTenHang.Caption = "Tên SMS";
                colTenCL.Visible = false;
               // colDot.Visible = false;
                colGhiChu.Visible = false;
                //colSoLuong.Visible = false;
            }
            if (tbl !=null  && tbl.Rows.Count > 0)
            {
                gridControl1.DataSource = tbl;
              
            }
            else
            {
                NtbSoft.ERP.Libs.clsConvert<DongThungThongTinEntity> convert = new Libs.clsConvert<DongThungThongTinEntity>();
                DataTable tblnull = convert.ToDataTable(lstDSNhapKho);
                gridControl1.DataSource = tblnull;
            }
        }
        private

        void mainView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.IsGetData)
            {
                int sum = 0;
                bool isEmpty = true;
                DataRow row = (e.Row as DataRowView).Row;
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "ID" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "DauSizeID" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        try
                        {
                            if (row[col.FieldName] != null && !string.IsNullOrEmpty(row[col.FieldName].ToString()))
                            {
                                Match match = Regex.Match(row[col.FieldName].ToString(), @"\d+");

                                if (match.Success)
                                {
                                    // Lấy giá trị số từ kết quả Match
                                    int number = int.Parse(match.Value);
                                    int value = Convert.ToInt32(number);
                                    sum += value;

                                }

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
        void mainView_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            if (e.SummaryProcess == CustomSummaryProcess.Start)
                sum = 0;
            if (e.SummaryProcess == CustomSummaryProcess.Finalize)
            {
                foreach (BandedGridColumn col in view.Columns)
                {
                    if (col.FieldName != "ID" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "DauSizeID" && col.UnboundType == UnboundColumnType.Bound)
                    {

                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
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

            if (nrOfColumns == 1 && (columnNames[0] == "POID" || columnNames[0] == "PO" || columnNames[0] == "MaMau" || columnNames[0] == "DauSize" || columnNames[0] == "DauSizeID" || columnNames[0] == "NgayGH" || columnNames[0] == "GhiChu"))
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
                    gridband3.Caption = _colName[2];
                    bandedColumns[i] = (BandedGridColumn)bandedView.Columns.AddField(columnNames[i]);
                    bandedColumns[i].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    bandedColumns[i].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                    bandedColumns[i].DisplayFormat.FormatString = "{0:##,0}";
                    bandedColumns[i].Width = 70;
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
        private void gridView2_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column == null) return;
            Rectangle rect = e.Bounds;
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds);
            Brush brush =
                e.Cache.GetGradientBrush(rect, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), e.Column.AppearanceHeader.GradientMode);
            rect.Inflate(-1, -1);
            e.Graphics.FillRectangle(brush, rect);
            e.Appearance.DrawString(e.Cache, e.Info.Caption, e.Info.CaptionRect);
            foreach (DevExpress.Utils.Drawing.DrawElementInfo info in e.Info.InnerElements)
            {
                DevExpress.Utils.Drawing.ObjectPainter.DrawObject(e.Cache, info.ElementPainter, info.ElementInfo);
            }
            e.Handled = true;
        }
        private void bandedGridView1_DataSourceChanged(object sender, EventArgs e)
        {
            GridView view = (GridView)sender;
            object _objmadh = null, _objmapkl = null, _objmadvsx = null, _objtendvsx = null, _objmalenh = null, _objpoid = null,
                _objdausizeid = null, _objmau = null, _objpo = null, _objtenlenh = null, _objmalenhsx = null;
            int _objtuthung = 0, _objdenthung = 0;

            // Lấy dữ liệu từ hàng đã chọn (FocusedRow)
            DataRow focusedRow = view.GetDataRow(view.FocusedRowHandle);
            if (focusedRow == null) return;
            //_objmadh = focusedRow["MaDH"];
            _objmapkl = focusedRow["MaPKL"];
            _objmadvsx = focusedRow["MaDVSX"];
            _objmalenh = focusedRow["MaLenh"];
            _objpoid = focusedRow["POID"];
            _objdausizeid = focusedRow["DauSizeID"];
            _objmau = focusedRow["MaMau"];
            _objpoid = focusedRow["POID"];
            _objtuthung = Convert.ToInt32(focusedRow["TuThung"]);
            _objdenthung = Convert.ToInt32(focusedRow["DenThung"]);
            if (_objmapkl != null)
                _mapkldtct = _objmapkl.ToString();
            if (_objmadvsx != null)
                _madvsxdtct = _objmadvsx.ToString();
            if (_objmalenh != null)
                _malenhdtct = _objmalenh.ToString();
            if (_objdausizeid != null)
                _dausizeiddtct = _objdausizeid.ToString();
            if (_objmau != null)
                _maudtct = _objmau.ToString();
            if (_objpoid != null)
                _poiddtct = _objpoid.ToString();
            if (_objtuthung != null)
                _tuthung = _objtuthung;
            if (_objdenthung != null)
                _denthung = _objdenthung;
        }
        private void gridView2_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            rowHandel_1 = 0;
            rowHandel_2 = 0;
            LoadDTPO();
        }

        private void bandedGridView1_CellMerge(object sender, CellMergeEventArgs e)
        {
            KHDongThungLibP.Merge(sender, e);
        }

        private void bandedGridView1_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            var dt = gridControl2.DataSource as DataTable;
            KHDongThungLibP.SumGroup(dt, e);
        }
        private void LoadDTPO()
        {
            GridView view = (GridView)gridView2;
            object _objMaDH = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaDH = view.GetRowCellValue(childHandle, colMaGop);
            }
            else
            {
                _objMaDH = view.GetFocusedRowCellValue(colMaGop);
            }
            if (_objMaDH != null)
                _madh = _objMaDH.ToString();
            else
                _madh = "";
            if (_madh == null)
                return;
            if (!_sms)
            {
                string urlTT = string.Format("{0}?madh={1}&&madvsx={2}", URL + "DongThung/GetThongTinPO", _madh, string.Join(";", GlobleData.lstDVSX));
                string jsonTT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTT); }).Result;
                tblCT = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                colMaLenh.Visible = true;
                colTenLenh.Visible = true;
            }
            else
            {
                string urlTT = string.Format("{0}?madh={1}&&madvsx={2}", URL + "DongThung/GetThongTinPOSMS", _madh, string.Join(";", GlobleData.lstDVSX));
                string jsonTT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTT); }).Result;
                tblCT = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                colMaLenh.Visible = false;
                colTenLenh.Visible = false;
            }
            if (tblCT.Rows.Count > 0)
            {
                gridControlDongThung.DataSource = tblCT;
                if (gridView1.FocusedRowHandle != rowHandel_1)
                {
                    gridView1.FocusedRowHandle = rowHandel_1;
                }
                if (bandedGridView1.FocusedRowHandle != rowHandel_2)
                {
                    bandedGridView1.FocusedRowHandle = rowHandel_2;
                }
            }
            else
            {
                NtbSoft.ERP.Libs.clsConvert<DongThungEntity> convert = new Libs.clsConvert<DongThungEntity>();
                DataTable tblnull = convert.ToDataTable(lstCTNhapKho);
                gridControlDongThung.DataSource = tblnull;

                NtbSoft.ERP.Libs.clsConvert<DongThungThongTinPOCTEntity> _convert = new Libs.clsConvert<DongThungThongTinPOCTEntity>();
                DataTable tblnulll = _convert.ToDataTable(lstDTPOCT);
                gridControl2.DataSource = tblnulll;
            }
        }
        private void gridView2_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            rowHandel_1 = 0;
            rowHandel_2 = 0;
            LoadDTPO();
        }

        private void bandedGridView1_DoubleClick(object sender, EventArgs e)
        {
            rowHandel_1 = gridView1.FocusedRowHandle;
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            GridView view = (GridView)sender;
            object _objmadh = null, _objmapkl = null, _objmadvsx = null, _objtendvsx = null, _objmalenh = null, _objpoid = null,
                _objdausizeid = null, _objmau = null, _objpo = null, _objtenlenh = null, _objmalenhsx = null;
            int _objtuthung = 0, _objdenthung = 0;
            DataRow focusedRow = view.GetDataRow(view.FocusedRowHandle);
            if (focusedRow == null) return;
            //_objmadh = focusedRow["MaDH"];
            _objmapkl = focusedRow["MaPKL"];
            _objmadvsx = focusedRow["MaDVSX"];
            _objmalenh = focusedRow["MaLenh"];
            _objpoid = focusedRow["POID"];
            _objdausizeid = focusedRow["DauSizeID"];
            _objmau = focusedRow["MaMau"];
            _objpoid = focusedRow["POID"];
            _objtuthung = Convert.ToInt32(focusedRow["TuThung"]);
            _objdenthung = Convert.ToInt32(focusedRow["DenThung"]);
            if (_objmapkl != null)
                _mapkldtct = _objmapkl.ToString();
            if (_objmadvsx != null)
                _madvsxdtct = _objmadvsx.ToString();
            if (_objmalenh != null)
                _malenhdtct = _objmalenh.ToString();
            if (_objdausizeid != null)
                _dausizeiddtct = _objdausizeid.ToString();
            if (_objmau != null)
                _maudtct = _objmau.ToString();
            if (_objpoid != null)
                _poiddtct = _objpoid.ToString();
            if (_objtuthung != null)
                _tuthung = _objtuthung;
            if (_objdenthung != null)
                _denthung = _objdenthung;
            frmChiTietDongThung frm = new frmChiTietDongThung(_mapkldtct, _malenhdtct, _madvsxdtct, _podt, _dausizeiddtct, _maudtct, _tuthung, _denthung, _sttthung, _minsttthung, _maxsttthung, _madh, _sms);
            frm.ShowDialog();
            LoadDTPO();
            gridView1.FocusedRowHandle = rowHandel_1;
            bandedGridView1.FocusedRowHandle = rowHandel_2;
        }

        private void bandedGridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
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
        private void MainView_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void MainView_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
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

        private void gridView2_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void gridView2_DataSourceChanged(object sender, EventArgs e)
        {
            rowHandel_1 = 0;
            rowHandel_2 = 0;
            LoadDTPO();
        }


        private void gridView1_DataSourceChanged(object sender, EventArgs e)
        {
            LoadDongThung();
        }
        private void bandedGridView1_CustomDrawBandHeader(object sender, BandHeaderCustomDrawEventArgs e)
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

        private void bandedGridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            e.Handled = KHDongThungLibP.MergeAllowChangeValue(bandedGridView1, e);
        }

        private void btCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            rowHandel = 0;
            rowHandel_1 = 0;
            rowHandel_2 = 0;
            LoadData();
        }

        private void MainView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            BandedGridView view = (BandedGridView)sender;
            object _objpoid = null, _objdausize = null, _objmamau = null;

            // Lấy dữ liệu từ hàng đã chọn (FocusedRow)
            DataRow focusedRow = view.GetDataRow(view.FocusedRowHandle);
            if (focusedRow == null) return;
            _objpoid = focusedRow["POID"];
            _objdausize = focusedRow["DauSizeID"];
            _objmamau = focusedRow["MaMau"];
            if (_objpoid != null)
                _poid = _objpoid.ToString();
            if (_objdausize != null)
                _dausizeid = _objdausize.ToString();
            if (_objmamau != null)
                _mamau = _objmamau.ToString();
        }

        private void barButtonItemPrevv_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            rowHandel = 0;
            rowHandel_1 = 0;
            rowHandel_2 = 0;
            if (pageIndex > 1)
            {
                pageIndex = pageIndex - 1;
                barStaticItemPageIndexx.Caption = pageIndex.ToString();
            }
            LoadData();
            LoadDTPO();
            LoadDongThung();
        }

        private void btnDTBarcode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmScanBarcodeDT_NK frm = new frmScanBarcodeDT_NK("DT");
            frm.ShowDialog();
        }

        private void barButtonItemNextt_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            rowHandel = 0;
            rowHandel_1 = 0;
            rowHandel_2 = 0;
            pageIndex = pageIndex + 1;
            barStaticItemPageIndexx.Caption = pageIndex.ToString();
            LoadData();
            LoadDTPO();
            LoadDongThung();
        }

        private void repositoryItemSpinEdit2_EditValueChanged(object sender, EventArgs e)
        {
            SpinEdit spinEdit = sender as SpinEdit;
            if (spinEdit != null && spinEdit.EditValue != null && Convert.ToInt32(spinEdit.EditValue) >= 0)
            {
                rowHandel = 0;
                rowHandel_1 = 0;
                rowHandel_2 = 0;
                pageSize = Convert.ToInt32(spinEdit.EditValue);
                LoadData();
                LoadDTPO();
                LoadDongThung();
            }
        }

        private void gridView2_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.RowHandle >= 0)
            {
                if (e.RowHandle == gridView2.FocusedRowHandle)
                {
                    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ddd");
                    e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    e.HighPriority = true;
                }
            }
        }

        private void gridView1_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.RowHandle >= 0)
            {
                if (e.RowHandle == gridView1.FocusedRowHandle)
                {
                    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ddd");
                    e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    e.HighPriority = true;
                }
            }
        }

        private void bandedGridView1_RowStyle(object sender, RowStyleEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.RowHandle >= 0)
            {
                if (e.RowHandle == bandedGridView1.FocusedRowHandle)
                {
                    e.Appearance.BackColor = System.Drawing.ColorTranslator.FromHtml("#ddd");
                    e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    e.HighPriority = true;
                }
            }

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
        private void bandedGridView1_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void gridView3_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void LoadDongThung()
        {
            GridView view = (GridView)gridView1;
            object _objmadh = null, _objmapkl = null, _objmadvsx = null, _objtendvsx = null, _objmalenh = null, _objpoid = null, _objpo = null, _objtenlenh = null, _objmalenhsx = null;
            int _objslkh = 0, _objslth = 0, _objslcl = 0;

            // Lấy dữ liệu từ hàng đã chọn (FocusedRow)
            DataRow focusedRow = view.GetDataRow(view.FocusedRowHandle);
            if (focusedRow == null) return;
            _objmadh = focusedRow["MaDH"];
            _objmapkl = focusedRow["MaPKL"];
            _objmadvsx = focusedRow["MaDVSX"];
            _objmalenhsx = focusedRow["MaLenh"];
            _objpoid = focusedRow["POID"];
            _objpo = focusedRow["PO"];
            _objtenlenh = focusedRow["TenLenh"];
            _objslkh = Convert.ToInt32(focusedRow["SLKH"]);
            _objslth = Convert.ToInt32(focusedRow["SLTH"]);
            _objslcl = Convert.ToInt32(focusedRow["SLCL"]);
            if (_objmadh != null)
                _madhdt = _objmadh.ToString();
            if (_objmapkl != null)
                _mapkldt = _objmapkl.ToString();
            if (_objmadvsx != null)
                _madvsxdt = _objmadvsx.ToString();
            if (_objtendvsx != null)
                _tendvsxdt = _objtendvsx.ToString();
            if (_objmalenhsx != null)
                _malenhdt = _objmalenhsx.ToString();
            if (_objtenlenh != null)
                _tenlenhdt = _objtenlenh.ToString();
            if (_objpo != null)
                _poiddt = _objpo.ToString();
            if (_objmalenh != null)
                _malenhsx = _objmalenh.ToString();
            if (_objpoid != null)
                _podt = _objpoid.ToString();
            if (_objslkh != null)
                _slkhdt = _objslkh;
            if (_objslth != null)
                _slthdt = _objslth;
            if (_objslcl != null)
                _slcldt = _objslcl;
            if (!_sms)
            {
                string urlTT = string.Format("{0}?mapkl={1}&&madvsx={2}&&malenh={3}&&MaDH={4}&&poid={5}&&malenhsanxuat={6}", URL + "DongThung/GetThongTinPOCT", _objmapkl, _objmadvsx, _objmalenh, _madh, _objpoid, _objmalenhsx);
                string jsonTT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTT); }).Result;
                DataTable tblPOCT = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                if (tblPOCT.Rows.Count > 0)
                {
                    bandedGridView1 = CreateBandSize(tblPOCT);
                    //KHDongThungLibP.ProcessSttTrung1(tblPOCT);
                    gridControl2.DataSource = tblPOCT;
                    bandedGridView1.FocusedRowHandle = rowHandel_2;
                }
                else
                {
                    NtbSoft.ERP.Libs.clsConvert<DongThungThongTinPOCTEntity> convert = new Libs.clsConvert<DongThungThongTinPOCTEntity>();
                    DataTable tblnull = convert.ToDataTable(lstDTPOCT);
                    gridControlDongThung.DataSource = tblnull;
                   
                }
            }
            else
            {
                string urlTT = string.Format("{0}?mapkl={1}&&madvsx={2}&&malenh={3}&&MaDH={4}&&poid={5}&&malenhsanxuat={6}", URL + "DongThung/GetThongTinPOCTSMS", _objmapkl, _objmadvsx, _objmalenh, _madh, _objpoid, _objmalenhsx);
                string jsonTT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTT); }).Result;
                DataTable tblPOCT = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                if (tblPOCT.Rows.Count > 0)
                {
                    KHDongThungLibP.ProcessSttTrung(tblPOCT);
                    bandedGridView1 = CreateBandSize(tblPOCT);
                    gridControl2.DataSource = tblPOCT;
                    bandedGridView1.FocusedRowHandle = rowHandel_2;
                }
                else
                {
                    NtbSoft.ERP.Libs.clsConvert<DongThungThongTinPOCTEntity> convert = new Libs.clsConvert<DongThungThongTinPOCTEntity>();
                    DataTable tblnull = convert.ToDataTable(lstDTPOCT);
                    gridControlDongThung.DataSource = tblnull;
                }
            }

        }
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadDongThung();
        }
        private void gridView3_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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
        private void gridView1_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }
        private void gridView3_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = (GridView)sender;
            object _objmapkl = null, _objmadh = null, _objpoid = null;
            int _objsttthung = 0;
            DataRow focusedRow = view.GetDataRow(view.FocusedRowHandle);
            if (focusedRow == null) return;
            _objmadh = focusedRow["MaDH"];
            _objmapkl = focusedRow["MaPKL"];
            _objsttthung = Convert.ToInt32(focusedRow["SttThung"]);
            _objpoid = focusedRow["POID"];
            if (_objmadh != null)
                _madh = _objmadh.ToString();
            if (_objmapkl != null)
                _mapkl = _objmapkl.ToString();
            if (_objsttthung != null)
                _sttthung = _objsttthung;
            if (_objpoid != null)
                _poid = _objpoid.ToString();
        }
        private void gridView2_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
        private void gridView2_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
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
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }
        private void gridView3_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
        private void gridView3_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }
        private BandedGridView CreateBandSize(DataTable tblDS)
        {
            BandedGridView bandedGridview = new BandedGridView();
            bandedGridview = bandedGridView1;
            try
            {

                gridBand7.Children.Clear();
                foreach (DataColumn dc in tblDS.Columns)
                {
                    if (dc.ColumnName.Contains("@Size@"))
                    {
                        string[] arrNewHeader = dc.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        string colName = arrNewHeader[2];
                        BandedGridColumn col = new BandedGridColumn();
                        col.AppearanceHeader.Options.UseTextOptions = true;
                        col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        col.Caption = colName.Replace("@Size@", "");
                        col.FieldName = dc.ColumnName;
                        col.Name = "col" + colName;
                        col.OptionsColumn.AllowEdit = false;
                        col.Visible = true;
                        col.MinWidth = 45;
                        col.Width = 45;
                        col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                        col.Summary.Add(DevExpress.Data.SummaryItemType.Sum, col.FieldName, "{0:n0}");
                        col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                        col.DisplayFormat.FormatString = "{0:##,0}";
                        col.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                        bandedGridview.Columns.AddRange(new BandedGridColumn[] { col });

                        GridBand gb = new GridBand();
                        gb.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        gb.AppearanceHeader.Options.UseFont = true;
                        gb.AppearanceHeader.Options.UseTextOptions = true;
                        gb.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        // 255, 212, 128 => Màu cam dùng cho header
                        gb.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
                        gb.AppearanceHeader.BackColor2 = System.Drawing.Color.White;
                        gb.AppearanceHeader.ForeColor = System.Drawing.Color.Black;
                        gb.AppearanceHeader.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
                        gb.AppearanceHeader.Options.UseBackColor = true;
                        gb.AppearanceHeader.Options.UseForeColor = true;
                        gb.Caption = colName.Replace("@Size@", "");
                        gb.Columns.Add(col);
                        gb.Name = "gb" + "Size_" + col;
                        gb.VisibleIndex = 0;
                        gb.Width = 65;
                        gridBand7.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return bandedGridview;
        }
        private void bandedGridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            BandedGridView view = sender as BandedGridView;
            if (view != null)
            {
                DataRow focusedRow = view.GetDataRow(view.FocusedRowHandle);
                DataTable dtDongThung = this.gridControl2.DataSource as DataTable;
                if (focusedRow == null) return;
                foreach (DataRow _dr in dtDongThung.Rows)
                {
                    if (_dr["SttThung"].ToString() == focusedRow["SttThung"].ToString())
                        _dr["CheckDT"] = e.Value;
                }
            }
        }
        private void bandedGridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = (GridView)sender;
            object _objmadh = null, _objmapkl = null, _objmadvsx = null, _objtendvsx = null, _objmalenh = null, _objpoid = null,
                _objdausizeid = null, _objmau = null, _objpo = null, _objtenlenh = null, _objmalenhsx = null;
            int _objtuthung = 0, _objdenthung = 0, _objsttthung = 0, _objMinSttThung = 0, _objMaxSttThung = 0;

            // Lấy dữ liệu từ hàng đã chọn (FocusedRow)
            DataRow focusedRow = view.GetDataRow(view.FocusedRowHandle);
            if (focusedRow == null) return;
            //_objmadh = focusedRow["MaDH"];
            _objmapkl = focusedRow["MaPKL"];
            _objmadvsx = focusedRow["MaDVSX"];
            _objmalenh = focusedRow["MaLenh"];
            _objpoid = focusedRow["POID"];
            _objdausizeid = focusedRow["DauSizeID"];
            _objmau = focusedRow["MaMau"];
            _objpoid = focusedRow["POID"];
            _objtuthung = Convert.ToInt32(focusedRow["TuThung"]);
            _objdenthung = Convert.ToInt32(focusedRow["DenThung"]);
            _objsttthung = Convert.ToInt32(focusedRow["SttThung"]);
            _objMinSttThung = Convert.ToInt32(focusedRow["MinSttThung"]);
            _objMaxSttThung = Convert.ToInt32(focusedRow["MaxSttThung"]);
            if (_objmapkl != null)
                _mapkldtct = _objmapkl.ToString();
            if (_objmadvsx != null)
                _madvsxdtct = _objmadvsx.ToString();
            if (_objmalenh != null)
                _malenhdtct = _objmalenh.ToString();
            if (_objdausizeid != null)
                _dausizeiddtct = _objdausizeid.ToString();
            if (_objmau != null)
                _maudtct = _objmau.ToString();
            if (_objpoid != null)
                _poiddtct = _objpoid.ToString();
            if (_objtuthung != null)
                _tuthung = _objtuthung;
            if (_objdenthung != null)
                _denthung = _objdenthung;
            if (_objsttthung != null)
                _sttthung = _objsttthung;
            if (_objMinSttThung != null)
                _minsttthung = _objMinSttThung;
            if (_objMaxSttThung != null)
                _maxsttthung = _objMaxSttThung;
        }

        private void simpleButtonAll_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
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
            DataRow _dr = dtSave.NewRow();
            _dr["MaDH"] = _madhdt;
            _dr["MaPKL"] = _mapkldt;
            _dr["MaDVSX"] = _madvsxdt;
            _dr["MaLenh"] = _malenhdt;
            _dr["POID"] = _podt;
            dtSave.Rows.Add(_dr);
            string url = "";
            if (!_sms)
                url = string.Format("{0}?value={1}", URL + "DongThung/PostALL", 1);
            else
                url = string.Format("{0}?value={1}", URL + "DongThung/PostALLSMS", 1);
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (result.ToLower() != "true")
                XtraMessageBox.Show(result);
            else
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadDTPO();
            }
        }

        private void simpleButtonHuy_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
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
            DataRow _dr = dtSave.NewRow();
            _dr["MaDH"] = _madhdt;
            _dr["MaPKL"] = _mapkldt;
            _dr["MaDVSX"] = _madvsxdt;
            _dr["MaLenh"] = _malenhdt;
            _dr["POID"] = _podt;
            dtSave.Rows.Add(_dr);
            if (dtSave.Rows.Count <= 0) return;
            string urlKT = string.Format("{0}?", URL + "DongThung/GetKiemTraAll");
            string jsonKT = Task.Run(async () => { return await _clientExtension.PostAsync(urlKT, dtSave); }).Result;
            DataTable _dtKT = JsonConvert.DeserializeObject<DataTable>(jsonKT);
            if (_dtKT != null && _dtKT.Rows.Count > 0)
            {
                DialogResult messResult = MessageBox.Show("Một số thùng đã được nhập kho. Chỉ hủy thùng chưa được nhập kho.!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.No)
                {
                    return;
                }
            }
            string url = "";
            if (_sms)
                url = string.Format("{0}?value={1}", URL + "DongThung/PostALLSMS", 0);
            else
                url = string.Format("{0}?value={1}", URL + "DongThung/PostALL", 0);
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (result.ToLower() != "true")
                XtraMessageBox.Show(result);
            else
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadDTPO();
            }
        }

        private void splitContainerControl1_SizeChanged(object sender, EventArgs e)
        {
            splitContainerControl1.SplitterPosition = splitContainerControl1.Height / 2;
        }

        private void gridView2_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    LoadData();
                    break;
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    LoadData();
                    break;
            }
        }

        private void bandedGridView1_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.F5:
                    LoadData();
                    break;
            }
        }

        private void simpleButtonSL_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            DataTable _tblCT = new DataTable();
            if (!_sms)
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&minsttthung={9}&&maxsttthung={10}", URL + "DongThung/GetThongTinChiTiet", _mapkldt, _malenhdt, _madvsxdt, _podt, _madh, _maudtct, _tuthung, _denthung, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblCT = JsonConvert.DeserializeObject<DataTable>(json);
            }
            else
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&minsttthung={9}&&maxsttthung={10}", URL + "DongThung/GetThongTinChiTietSMS", _mapkldt, _malenhdt, _madvsxdt, _podt, _madh, _maudtct, _tuthung, _denthung, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblCT = JsonConvert.DeserializeObject<DataTable>(json);
            }

            List<string> lstSize = _tblCT.AsEnumerable()
                       .Select(x => x["Size"].ToString())
                       .ToList();

            DataTable dtDongThung = this.gridControl2.DataSource as DataTable;
            DataRow rowDT = bandedGridView1.GetDataRow(bandedGridView1.FocusedRowHandle);

            List<DataRow> lstDongThung = dtDongThung.AsEnumerable()
                .Where(x => x["SttThung"].ToString() == rowDT["SttThung"].ToString()
                            && x["POID"].ToString() == rowDT["POID"].ToString()
                            && x["MaHang"].ToString() == rowDT["MaHang"].ToString())
                .ToList();

            frmNhapKhoSoLuong frm = new frmNhapKhoSoLuong(lstDongThung, string.Join(";", lstSize.Distinct()), true, _mapkldt, _tenlenhdt, 0, _poiddt);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                int i = 0;
                int soluong = frm.SoLuong;
                if (soluong <= 0) return;
                bool isdongthung = true;
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
                foreach (DataRow row in _tblCT.Rows)
                {
                    if (i == soluong)
                        break;
                    if (Convert.ToInt32(row["CheckDT"]) == 0)
                    {
                        DataRow _dr = dtSave.NewRow();
                        _dr["MaDH"] = row["MaGop"];
                        _dr["MaPKL"] = row["MaPKL"];
                        _dr["MaLenh"] = row["MaLenh"];
                        _dr["MaDVSX"] = row["MaDVSX"];
                        _dr["POID"] = _podt; //row["POID"];
                        _dr["TuThung"] = _tuthung;
                        _dr["DenThung"] = _denthung;
                        _dr["MinSttThung"] = _minsttthung;
                        _dr["MaxSttThung"] = _maxsttthung;
                        _dr["SttThungDecat"] = row["SttDT"];
                        dtSave.Rows.Add(_dr);
                        i++;
                    }
                }
                if (dtSave == null || dtSave.Rows.Count == 0) return;
                string urlSave = "";
                if (_sms)
                    urlSave = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostDTSMS", isdongthung);
                else
                    urlSave = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostDT", isdongthung);

                string resultSave = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, dtSave); }).Result;
                if (resultSave.ToLower() != "true")
                    XtraMessageBox.Show(resultSave);
                else
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    LoadDTPO();
                }
            }

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            DataTable _tblCT = new DataTable();
            if (!_sms)
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&minsttthung={9}&&maxsttthung={10}", URL + "DongThung/GetThongTinChiTiet", _mapkldt, _malenhdt, _madvsxdt, _podt, _madh, _maudtct, _tuthung, _denthung, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblCT = JsonConvert.DeserializeObject<DataTable>(json);

            }
            else
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&minsttthung={9}&&maxsttthung={10}", URL + "DongThung/GetThongTinChiTietSMS", _mapkldt, _malenhdt, _madvsxdt, _podt, _madh, _maudtct, _tuthung, _denthung, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblCT = JsonConvert.DeserializeObject<DataTable>(json);
            }

            List<string> lstSize = _tblCT.AsEnumerable()
                       .Select(x => x["Size"].ToString())
                       .ToList();

            DataTable dtDongThung = this.gridControl2.DataSource as DataTable;
            DataRow rowDT = bandedGridView1.GetDataRow(bandedGridView1.FocusedRowHandle);

            List<DataRow> lstDongThung = dtDongThung.AsEnumerable()
                .Where(x => x["SttThung"].ToString() == rowDT["SttThung"].ToString()
                            && x["POID"].ToString() == rowDT["POID"].ToString()
                            && x["MaHang"].ToString() == rowDT["MaHang"].ToString())
                .ToList();

            frmNhapKhoSoLuong frm = new frmNhapKhoSoLuong(lstDongThung, string.Join(";", lstSize.Distinct()), true, _mapkldt, _tenlenhdt, 0, _poiddt);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                int i = 0;
                int soluong = frm.SoLuong;
                if (soluong <= 0) return;
                bool isdongthung = false;
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
                foreach (DataRow row in _tblCT.Rows)
                {
                    if (i == soluong)
                        break;
                    if (Convert.ToInt32(row["CheckDT"]) == 1)
                    {
                        DataRow _dr = dtSave.NewRow();
                        _dr["MaDH"] = row["MaGop"];
                        _dr["MaPKL"] = row["MaPKL"];
                        _dr["MaLenh"] = row["MaLenh"];
                        _dr["MaDVSX"] = row["MaDVSX"];
                        _dr["POID"] = _podt;//row["POID"];
                        _dr["TuThung"] = _tuthung;
                        _dr["DenThung"] = _denthung;
                        _dr["MinSttThung"] = _minsttthung;
                        _dr["MaxSttThung"] = _maxsttthung;
                        _dr["SttThungDecat"] = row["SttDT"];
                        dtSave.Rows.Add(_dr);
                        i++;
                    }
                }
                if (dtSave == null || dtSave.Rows.Count == 0) return;
                string urlKT = string.Format("{0}?", URL + "DongThung/GetKiemTra");
                string jsonKT = Task.Run(async () => { return await _clientExtension.PostAsync(urlKT, dtSave); }).Result;
                DataTable _dtKT = JsonConvert.DeserializeObject<DataTable>(jsonKT);
                if (_dtKT != null && _dtKT.Rows.Count > 0)
                {
                    DialogResult messResult = MessageBox.Show("Một số thùng đã được nhập kho. Chỉ hủy thùng chưa được nhập kho.!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (messResult == DialogResult.No)
                    {
                        return;
                    }
                }
                string urlSave = "";
                if (_sms)
                    urlSave = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostHuySMS", isdongthung);
                else
                    urlSave = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostHuy", isdongthung);

                string resultSave = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, dtSave); }).Result;
                if (resultSave.ToLower() != "true")
                    XtraMessageBox.Show(resultSave);
                else
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    LoadDTPO();
                }
            }
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            if (tblCT is null || tblCT.Rows.Count == 0) return;
            _mahang = tbl.AsEnumerable().Where(x => x["MaGop"] == _madh.ToString()).FirstOrDefault()["MaHang"].ToString();
            DataTable dtTbl = gridControl2.DataSource as DataTable;
            Sfd.FileName = string.Format("DongThung_{0}_{1}" + DateTime.Now.ToString("ddMMyyyy"), _mahang, _poiddt);
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
                    ExportExcel(Sfd.FileName, dtTbl);
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
        private void ExportExcel(string path, DataTable dtPKLXuatHang)
        {
            try
            {
                string PathLoGo = KHDongThungLibP.getImgPath("texgiang.jpg");
                FileInfo file = new FileInfo(path);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

                string fileName = "";
                string pathLogo = Path.Combine(Application.StartupPath, @"Resources\", fileName);
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    int Height = 100;
                    int Width = 150;
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add($"{_mahang}_{_poiddt}");
                    ExcelRange range = worksheet.Cells;
                    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    worksheet.Cells.Style.Font.Name = "Times New Roman";
                    worksheet.Cells.Style.Font.Size = 13;
                    range = worksheet.Cells["D1:N1"]; range.Merge = true; range.Value = "TEX-GIANG JOINT STOCK COMPANY"; range.Style.Font.Bold = true;
                    range = worksheet.Cells["D2:N3"]; range.Merge = true; range.Value = "ĐÓNG THÙNG"; range.Style.Font.Bold = true;
                    int colum = 6;
                    range = worksheet.Cells["A1:B3"]; range.Merge = true;
                    Image image = Image.FromFile(PathLoGo);
                    OfficeOpenXml.Drawing.ExcelPicture picture = worksheet.Drawings.AddPicture("pic", image);
                    picture.SetPosition(0, 0, 0, 0);
                    picture.SetSize(105, 55);
                    worksheet.Cells.Style.Font.Size = 11;
                    range = worksheet.Cells["D4"]; range.Value = "STYLE :";
                    range = worksheet.Cells["e4:f4"]; range.Merge = true; range.Value = _mahang;
                    range = worksheet.Cells["D5"]; range.Value = "Shipper: :";
                    range = worksheet.Cells["D6"]; range.Value = "Invoice No : ";
                    range = worksheet.Cells["D7"]; range.Value = "Consignee : ";
                    KHDongThungLibP.dtXuatEX(dtPKLXuatHang, worksheet);
                    int columSttThungDong = KHDongThungLibP._columns;
                    int row = 10;
                    worksheet.Cells[9, columSttThungDong].Value = "SL Đóng thùng";
                    worksheet.Column(columSttThungDong).AutoFit();
                    foreach (DataRow item in dtPKLXuatHang.Rows)
                    {
                        worksheet.Cells[row, columSttThungDong].Value = item["SLNhap"].ToString();
                        row++;
                    }
                    var borderData1 = worksheet.Cells[9, 5, row - 1, columSttThungDong].Style.Border;
                    borderData1.Bottom.Style =
                        borderData1.Top.Style =
                        borderData1.Left.Style =
                        borderData1.Right.Style = ExcelBorderStyle.Thin;
                    excelPackage.SaveAs(file);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xảy ra tại dòng: " + ex.StackTrace);
                throw;
            }
        }

        private void btnDVT_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            DataTable _tblCT = new DataTable();
            if (!_sms)
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&minsttthung={9}&&maxsttthung={10}", URL + "DongThung/GetThongTinChiTiet", _mapkldt, _malenhdt, _madvsxdt, _podt, _madh, _maudtct, _tuthung, _denthung, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblCT = JsonConvert.DeserializeObject<DataTable>(json);

            }
            else
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&minsttthung={9}&&maxsttthung={10}", URL + "DongThung/GetThongTinChiTietSMS", _mapkldt, _malenhdt, _madvsxdt, _podt, _madh, _maudtct, _tuthung, _denthung, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblCT = JsonConvert.DeserializeObject<DataTable>(json);
            }

            List<string> lstSize = _tblCT.AsEnumerable()
                       .Select(x => x["Size"].ToString())
                       .ToList();

            DataTable dtDongThung = this.gridControl2.DataSource as DataTable;
            DataRow rowDT = bandedGridView1.GetDataRow(bandedGridView1.FocusedRowHandle);

            List<DataRow> lstDongThung = dtDongThung.AsEnumerable()
                .Where(x => x["SttThung"].ToString() == rowDT["SttThung"].ToString()
                            && x["POID"].ToString() == rowDT["POID"].ToString()
                            && x["MaHang"].ToString() == rowDT["MaHang"].ToString())
                .ToList();

            frmNhapViTri frm = new frmNhapViTri(lstDongThung, string.Join(";", lstSize.Distinct()), true, _mapkldt, _tenlenhdt, 0, _podt);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                int i = 0;
                int TuThung = frm.TuThung;
                int DenThung = frm.DenThung;
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
                int _tuThung = _tblCT.AsEnumerable().Where(row => Convert.ToInt32(row["SttThung"]) == Convert.ToInt32(TuThung)).Sum(row => Convert.ToInt32(row["SttDT"]));
                int _denThung = _tblCT.AsEnumerable().Where(row => Convert.ToInt32(row["SttThung"]) == Convert.ToInt32(DenThung)).Sum(row => Convert.ToInt32(row["SttDT"]));

                DataRow _dr = dtSave.NewRow();
                _dr["MaDH"] = _madh;
                _dr["MaPKL"] = _mapkldtct;
                _dr["MaLenh"] = _malenhdtct;
                _dr["MaDVSX"] = _madvsxdtct;
                _dr["POID"] = _podt;
                if (!_sms)
                {
                    _dr["TuThung"] = TuThung;
                    _dr["DenThung"] = DenThung;
                }  
                else
                {
                    _dr["TuThung"] = TuThung;
                    _dr["DenThung"] = DenThung;
                }    
                _dr["MinSttThung"] = 0;
                _dr["MaxSttThung"] = 0;
                _dr["SttThungDecat"] = 0;
                dtSave.Rows.Add(_dr);
                if (dtSave == null || dtSave.Rows.Count == 0) return;
                string urlSave = "";
                if (_sms)
                    urlSave = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostViTriSMS", true);
                else
                    urlSave = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostViTri", true);
                string resultSave = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, dtSave); }).Result;
                if (resultSave.ToLower() != "true")
                    XtraMessageBox.Show(resultSave);
                else
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    LoadDTPO();
                }
            }
        }

        private void btnHVT_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            DataTable _tblCT = new DataTable();
            if (!_sms)
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&minsttthung={9}&&maxsttthung={10}", URL + "DongThung/GetThongTinChiTiet", _mapkldt, _malenhdt, _madvsxdt, _podt, _madh, _maudtct, _tuthung, _denthung, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblCT = JsonConvert.DeserializeObject<DataTable>(json);

            }
            else
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&minsttthung={9}&&maxsttthung={10}", URL + "DongThung/GetThongTinChiTietSMS", _mapkldt, _malenhdt, _madvsxdt, _podt, _madh, _maudtct, _tuthung, _denthung, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblCT = JsonConvert.DeserializeObject<DataTable>(json);
            }

            List<string> lstSize = _tblCT.AsEnumerable()
                       .Select(x => x["Size"].ToString())
                       .ToList();

            DataTable dtDongThung = this.gridControl2.DataSource as DataTable;
            DataRow rowDT = bandedGridView1.GetDataRow(bandedGridView1.FocusedRowHandle);

            List<DataRow> lstDongThung = dtDongThung.AsEnumerable()
                .Where(x => x["SttThung"].ToString() == rowDT["SttThung"].ToString()
                            && x["POID"].ToString() == rowDT["POID"].ToString()
                            && x["MaHang"].ToString() == rowDT["MaHang"].ToString())
                .ToList();

            frmNhapViTri frm = new frmNhapViTri(lstDongThung, string.Join(";", lstSize.Distinct()), true, _mapkldt, _tenlenhdt, 0, _podt);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                int TuThung = frm.TuThung;
                int DenThung = frm.DenThung;
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
                int _tuThung = _tblCT.AsEnumerable().Where(row => Convert.ToInt32(row["SttThung"]) == Convert.ToInt32(TuThung)).Sum(row => Convert.ToInt32(row["SttDT"]));
                int _denThung = _tblCT.AsEnumerable().Where(row => Convert.ToInt32(row["SttThung"]) == Convert.ToInt32(DenThung)).Sum(row => Convert.ToInt32(row["SttDT"]));

                DataRow _dr = dtSave.NewRow();
                _dr["MaDH"] = _madh;
                _dr["MaPKL"] = _mapkldtct;
                _dr["MaLenh"] = _malenhdtct;
                _dr["MaDVSX"] = _madvsxdtct;
                _dr["POID"] = _podt;
                if (!_sms)
                {
                    _dr["TuThung"] = TuThung;
                    _dr["DenThung"] = DenThung;
                }
                else
                {
                    _dr["TuThung"] = TuThung;
                    _dr["DenThung"] = DenThung;
                }
                _dr["MinSttThung"] = 0;
                _dr["MaxSttThung"] = 0;
                _dr["SttThungDecat"] = 0;
                dtSave.Rows.Add(_dr);

                if (dtSave == null || dtSave.Rows.Count == 0) return;
                string urlKT = string.Format("{0}?", URL + "DongThung/GetKiemTraViTri");
                string jsonKT = Task.Run(async () => { return await _clientExtension.PostAsync(urlKT, dtSave); }).Result;
                DataTable _dtKT = JsonConvert.DeserializeObject<DataTable>(jsonKT);
                if (_dtKT != null && _dtKT.Rows.Count > 0)
                {
                    DialogResult messResult = MessageBox.Show("Một số thùng đã được nhập kho. Chỉ hủy thùng chưa được nhập kho.!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (messResult == DialogResult.No)
                    {
                        return;
                    }
                }
                string urlSave = "";
                if (_sms)
                    urlSave = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostViTriSMS", false);
                else
                    urlSave = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostViTri", false);
                string resultSave = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, dtSave); }).Result;
                if (resultSave.ToLower() != "true")
                    XtraMessageBox.Show(resultSave);
                else
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    LoadDTPO();
                }
            }
        }

        private void btnDongThungChon_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            DataTable dtDongThung = this.gridControl2.DataSource as DataTable;
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
            foreach (DataRow dr in dtDongThung.Rows)
            {
                if ((bool)dr["CheckDT"] == true)
                {
                    DataRow _dr = dtSave.NewRow();
                    _dr["MaDH"] = _madhdt;
                    _dr["MaPKL"] = _mapkldt;
                    _dr["MaDVSX"] = _madvsxdt;
                    if (_sms)
                        _dr["MaLenh"] = dr["MaLenh"];
                    else
                        _dr["MaLenh"] = dr["MaLenh"];
                    _dr["POID"] = _podt;
                    _dr["TuThung"] = dr["TuThung"];
                    _dr["DenThung"] = dr["DenThung"];
                    _dr["MinSttThung"] = dr["MinSttThung"];
                    _dr["MaxSttThung"] = dr["MaxSttThung"];
                    dtSave.Rows.Add(_dr);
                }

            }
            if (dtSave.Rows.Count <= 0) return;
            string url = "";
            if (_sms)
                url = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostSMS", true);
            else
                url = string.Format("{0}?isdongthung={1}", URL + "DongThung/Post", true);
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (result.ToLower() != "true")
                XtraMessageBox.Show(result);
            else
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadDTPO();
            }
        }
        private void btDongThungHuy_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            DataTable dtDongThung = this.gridControl2.DataSource as DataTable;
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
            foreach (DataRow dr in dtDongThung.Rows)
            {
                if ((bool)dr["CheckDT"] == false)
                {
                    DataRow _dr = dtSave.NewRow();
                    _dr["MaDH"] = _madhdt;
                    _dr["MaPKL"] = _mapkldt;
                    _dr["MaDVSX"] = _madvsxdt;
                    _dr["MaLenh"] = dr["MaLenh"];
                    _dr["POID"] = _podt;
                    _dr["TuThung"] = dr["TuThung"];
                    _dr["DenThung"] = dr["DenThung"];
                    _dr["MinSttThung"] = dr["MinSttThung"];
                    _dr["MaxSttThung"] = dr["MaxSttThung"];
                    dtSave.Rows.Add(_dr);
                }

            }
            if (dtSave.Rows.Count <= 0) return;
            string urlKT = string.Format("{0}?", URL + "DongThung/GetKiemTra");
            string jsonKT = Task.Run(async () => { return await _clientExtension.PostAsync(urlKT, dtSave); }).Result;
            DataTable _dtKT = JsonConvert.DeserializeObject<DataTable>(jsonKT);
            if (_dtKT != null && _dtKT.Rows.Count > 0)
            {
                DialogResult messResult = MessageBox.Show("Một số thùng đã được nhập kho. Chỉ hủy thùng chưa được nhập kho.!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.No)
                {
                    return;
                }
            }
            string url = "";
            if (_sms)
                url = string.Format("{0}?isdongthung={1}", URL + "DongThung/PostSMS", false);
            else
                url = string.Format("{0}?isdongthung={1}", URL + "DongThung/Post", false);
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (result.ToLower() != "true")
                XtraMessageBox.Show(result);
            else
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadDTPO();
            }

        }

        private void barCheckItemSMS_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {  
            _sms = (bool)barCheckItemSMS.Checked;
            LoadData();
        }

    }
}
