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
using System.Web;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmNhapKhoTP : DevExpress.XtraEditors.XtraForm
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
        private string _maKH = string.Empty, _madhdtct = string.Empty, _mahangdtct = string.Empty, _poiddtct = string.Empty, _mapkldtct = string.Empty, _tendvsxdtct = string.Empty, _tensxdtct = string.Empty, _malenhdtct = string.Empty, _tenlenhdtct = string.Empty, _podtct = string.Empty,
            _dausizeiddtct = string.Empty, _maudtct = string.Empty, _malenhsxct = string.Empty, _madvsxdtct = string.Empty, _makhodtct = string.Empty, _madvsxHT = string.Empty;
        private int _sttthung = 0, _slkhdt = 0, _slthdt = 0, _slcldt = 0, _tuthung = 0, _denthung = 0, _minsttthung = 0, _maxsttthung = 0, _isTonKho = 0;
        DataTable tblCT;
        DataTable tbl;
        bool indicatorIcon = true;
        RepositoryItemCheckEdit repositoryItemCheckEdit;
        int lcsx = 0;
        decimal _cbm = 0;
        int rowHandel = 0;
        int rowHandel_1 = 0;
        int rowHandel_2 = 0;
        int statuschuyen = 1;
        int pageIndex = 1;
        int pageSize = 100;
        DataTable tblKho;
        DataTable _dtRowSave;
        bool _kiemtra = false, _sms = false, _kt = false;
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        public frmNhapKhoTP()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            lstDSNhapKho = new List<DongThungThongTinEntity>();
            lstCTNhapKho = new List<DongThungEntity>();
            lstDTPOCT = new List<DongThungThongTinPOCTEntity>();
            tblCT = new DataTable();
            tbl = new DataTable();
            tblKho = new DataTable();
            _dtRowSave = new DataTable();
            repositoryItemCheckEdit = new RepositoryItemCheckEdit();
        }

        protected override void OnLoad(EventArgs e)
        {
            CheckPerminsion();
            CreateDatatable();
            barEditItemSpinPageSizee.EditValue = pageSize;
            //CreateDefault();
            if ((bool)barCheckItemlcxh.Checked == true)
            {
                lcsx = 1;
                colNTenDVSXD.Visible = true;
                colTenDVSX.Caption = "Từ đơn vị SX";
                gridBand12.Visible = true;
                gridBand16.Visible = true;
                gridBand4.Caption = "Từ đơn vị SX";
                gridBand17.Caption = "Từ Kho";
                LoadData();
            }
            else
            {
                lcsx = 0;
                colNTenDVSXD.Visible = false;
                colTenDVSX.Caption = "Đơn vị SX";
                gridBand12.Visible = false;
                gridBand16.Visible = false;
                gridBand4.Caption = "Đơn vị SX";
                gridBand17.Caption = "Kho";
                LoadData();
            }
            if ((bool)chkNhap.Checked == false)
            {

                layoutControlItem15.Enabled = false;
                layoutControlItem16.Enabled = false;
                layoutControlItem17.Enabled = false;
                layoutControlItem18.Enabled = false;
                layoutControlItem19.Enabled = false;
                searchLookUpEditO.EditValue = null;
                searchLookUpEditTang.EditValue = null;
                searchLookUpEditKe.EditValue = null;
                searchLookUpEditDay.EditValue = null;
                txtCBM.Text = "";
            }

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
                layoutControlItem11.Enabled = false;
                layoutControlItem12.Enabled = false;
                layoutControlItem13.Enabled = false;
                layoutControlItem14.Enabled = false;
                layoutControlItem15.Enabled = false;
                layoutControlItem16.Enabled = false;
                layoutControlItem17.Enabled = false;
                layoutControlItem18.Enabled = false;
                layoutControlItem19.Enabled = false;
            }
        }
        private DataTable CreateDatatable()
        {
            _dtRowSave = new DataTable();
            DataTable tbl = new DataTable("dtRowSave");

            _dtRowSave.Columns.Add("MaDH", typeof(string));
            _dtRowSave.Columns.Add("MaPKL", typeof(string));
            _dtRowSave.Columns.Add("MaDVSX", typeof(string));
            _dtRowSave.Columns.Add("MaLenh", typeof(string));
            _dtRowSave.Columns.Add("POID", typeof(string));
            _dtRowSave.Columns.Add("MaKho", typeof(string));
            _dtRowSave.Columns.Add("MaKhoDen", typeof(string));
            _dtRowSave.Columns.Add("TuThung", typeof(int));
            _dtRowSave.Columns.Add("DenThung", typeof(int));
            _dtRowSave.Columns.Add("MinSttThung", typeof(int));
            _dtRowSave.Columns.Add("MaxSttThung", typeof(int));
            _dtRowSave.Columns.Add("SttThungDecat", typeof(int));
            _dtRowSave.Columns.Add("NgayNhapKho_TC", typeof(string));
            _dtRowSave.Columns.Add("CheckDT", typeof(bool));
            return tbl;
        }
        private void LoadCheckNhap()
        {

            if ((bool)chkNhap.Checked == true)
            {

                layoutControlItem15.Enabled = true;
                layoutControlItem16.Enabled = true;
                layoutControlItem17.Enabled = true;
                layoutControlItem18.Enabled = true;
                layoutControlItem19.Enabled = true;
            }
            else
            {
                layoutControlItem15.Enabled = false;
                layoutControlItem16.Enabled = false;
                layoutControlItem17.Enabled = false;
                layoutControlItem18.Enabled = false;
                layoutControlItem19.Enabled = false;
            }
        }
        private void LoadData()
        {
            searchLookUpEditKho.EditValue = null;
            if (searchLookUpEditKho.EditValue == null)
                chkNhap.Enabled = false;
            else
                chkNhap.Enabled = true;
            if (!_sms)
            {
                string urlTT = string.Format("{0}?lcsx={1}&&pageIndex={2}&&pageSize={3}&&madvsx={4}", URL + "NhapKho/GetThongTin", lcsx, pageIndex, pageSize, string.Join(";", GlobleData.lstDVSX));
                string jsonTT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTT); }).Result;
                tbl = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                colMaDH.Caption = "Đơn hàng";
                colTenHang.Caption = "Tên hàng";
                colTenCL.Visible = true;
                //colDot.Visible = true;
                colGhiChu.Visible = true;
               // colSoLuong.Visible = true;
            }
            else
            {
                string urlTT = string.Format("{0}?lcsx={1}&&pageIndex={2}&&pageSize={3}&&madvsx={4}", URL + "NhapKho/GetThongTinSMS", lcsx, pageIndex, pageSize, string.Join(";", GlobleData.lstDVSX));
                string jsonTT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTT); }).Result;
                tbl = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                colMaDH.Caption = "Mã SMS";
                colTenHang.Caption = "Tên SMS";
                colTenCL.Visible = false;
               // colDot.Visible = false;
                colGhiChu.Visible = false;
                //colSoLuong.Visible = false;
            }

            if (tbl !=null && tbl.Rows.Count > 0)
            {
                gridControl1.DataSource = tbl;
                if (gridView2.FocusedRowHandle != rowHandel)
                {
                    gridView2.FocusedRowHandle = rowHandel;
                }

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
                NtbSoft.ERP.Libs.clsConvert<DongThungThongTinEntity> convert = new Libs.clsConvert<DongThungThongTinEntity>();
                DataTable tblnull = convert.ToDataTable(lstDSNhapKho);
                gridControl1.DataSource = tblnull;


            }
            chkNhap.Checked = false;
            searchLookUpEditDay.EditValue = null;
            searchLookUpEditKe.EditValue = null;
            searchLookUpEditTang.EditValue = null;
            searchLookUpEditO.EditValue = null;
            txtCBM.Text = "";

        }

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
                                // Sử dụng biểu thức chính quy để tìm số trong chuỗi
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
        private void LoadNKPO()
        {
            searchLookUpEditKho.EditValue = null;
            searchLookUpEditKho.EditValue = null;
            chkNhap.Checked = false;
            GridView view = (GridView)gridView2;
            object _objMaDH = null, _objKhachHang = null;
            if (view.IsGroupRow(view.FocusedRowHandle))
            {
                int childHandle = view.GetChildRowHandle(view.FocusedRowHandle, 0);
                _objMaDH = view.GetRowCellValue(childHandle, colMaGop);
                _objKhachHang = view.GetRowCellValue(childHandle, colMaKH);
            }
            else
            {
                _objMaDH = view.GetFocusedRowCellValue(colMaGop);
                _objKhachHang = view.GetFocusedRowCellValue(colMaKH);
            }
            if (_objMaDH != null)
                _madh = _objMaDH.ToString();
            else
                _madh = "";
            if (_objKhachHang != null)
                _maKH = _objKhachHang.ToString();
            if (_madh == null || _madh == "")
            {
                NtbSoft.ERP.Libs.clsConvert<DongThungEntity> convert = new Libs.clsConvert<DongThungEntity>();
                DataTable tblnull = convert.ToDataTable(lstCTNhapKho);
                gridControlDongThung.DataSource = tblnull;

                NtbSoft.ERP.Libs.clsConvert<DongThungThongTinPOCTEntity> _convert = new Libs.clsConvert<DongThungThongTinPOCTEntity>();
                DataTable tblnulll = _convert.ToDataTable(lstDTPOCT);
                gridControl2.DataSource = tblnulll;
                return;
            }
            if (!_sms)
            {
                string urlTT = string.Format("{0}?madh={1}&&lcsx={2}&&madvsx={3}", URL + "NhapKho/GetThongTinPO", _madh, lcsx, string.Join(";", GlobleData.lstDVSX));
                string jsonTT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTT); }).Result;
                tblCT = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                colMaLenh.Visible = true;
                colTenLenh.Visible = true;
            }
            else
            {
                string urlTT = string.Format("{0}?madh={1}&&lcsx={2}&&madvsx={3}", URL + "NhapKho/GetThongTinPOSMS", _madh, lcsx, string.Join(";", GlobleData.lstDVSX));
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
            LoadNKPO();
        }

        private void bandedGridView1_DataSourceChanged(object sender, EventArgs e)
        {
            GridView view = (GridView)sender;
            object _objmadh = null, _objmapkl = null, _objmadvsx = null, _objtendvsx = null, _objmalenh = null, _objpoid = null,
                _objdausizeid = null, _objmau = null, _objpo = null, _objtenlenh = null, _objmalenhsx = null, _objmakho = null;
            int _objtuthung = 0, _objdenthung = 0, _objsttthung = 0, _objMinSttThung = 0, _objMaxSttThung = 0;
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
            _objmakho = focusedRow["MaKho"];
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
            if (_objmakho != null)
                _makhodtct = _objmakho.ToString();
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
            LoadNKPO();
        }

        private void gridView1_DataSourceChanged(object sender, EventArgs e)
        {
            LoadNhapKho();
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
        private void LoadNhapKho()
        {
            GridView view = (GridView)gridView1;
            object _objmadh = null, _objmapkl = null, _objmadvsx = null, _objtendvsx = null, _objmalenh = null, _objpoid = null, _objpo = null,
                _objtenlenh = null, _objmalenhsx = null, _objmadvsxd = null, _makho = string.Empty, _objMaDVSXHoanThanh = null;
            int _objslkh = 0, _objslth = 0, _objslcl = 0;
            DataRow focusedRow = view.GetDataRow(view.FocusedRowHandle);
            if (focusedRow == null) return;
            _objmadh = focusedRow["MaDH"];
            _objmapkl = focusedRow["MaPKL"];
            _objmadvsx = focusedRow["MaDVSX"];
            if (lcsx == 1)
                _objmadvsxd = focusedRow["MaDVSXD"];
            _objmalenhsx = focusedRow["MaLenh"];
            _objpoid = focusedRow["POID"];
            _objpo = focusedRow["PO"];
            _objtenlenh = focusedRow["TenLenh"];
            _objMaDVSXHoanThanh = focusedRow["MaDVSXHoanThanh"];
            _objslkh = 0;
            _objslth = 0;
            _objslcl = 0;
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
            if (_objMaDVSXHoanThanh != null)
                _madvsxHT = _objMaDVSXHoanThanh.ToString();
            if (!_sms)
            {
                string urlTT = string.Format("{0}?mapkl={1}&&madvsx={2}&&malenh={3}&&MaDH={4}&&poid={5}&&malenhsanxuat={6}&&lcsx={7}", URL + "NhapKho/GetThongTinPOCT", _objmapkl, _objmadvsx, _objmalenh, _madh, _objpoid, _objmalenhsx, lcsx);
                string jsonTT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTT); }).Result;
                DataTable tblPOCT = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                if (tblPOCT.Rows.Count > 0)
                {
                    bandedGridView1 = CreateBandSize(tblPOCT);
                    gridControl2.DataSource = tblPOCT;
                    if (bandedGridView1.FocusedRowHandle != rowHandel_2)
                    {
                        bandedGridView1.FocusedRowHandle = rowHandel_2;
                    }
                    gridControl2.RefreshDataSource();
                }
                else
                {
                    NtbSoft.ERP.Libs.clsConvert<DongThungThongTinPOCTEntity> convert = new Libs.clsConvert<DongThungThongTinPOCTEntity>();
                    DataTable tblnull = convert.ToDataTable(lstDTPOCT);
                    gridControl2.DataSource = tblnull;
                    gridControl2.RefreshDataSource();
                }
            }
            else
            {
                string urlTT = string.Format("{0}?mapkl={1}&&madvsx={2}&&malenh={3}&&MaDH={4}&&poid={5}&&malenhsanxuat={6}&&lcsx={7}", URL + "NhapKho/GetThongTinPOCTSMS", _objmapkl, _objmadvsx, _objmalenh, _madh, _objpoid, _objmalenhsx, lcsx);
                string jsonTT = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlTT); }).Result;
                DataTable tblPOCT = JsonConvert.DeserializeObject<DataTable>(jsonTT);
                if (tblPOCT.Rows.Count > 0)
                {
                    KHDongThungLibP.ProcessSttTrung(tblPOCT);
                    bandedGridView1 = CreateBandSize(tblPOCT);
                    gridControl2.DataSource = tblPOCT;
                    if (bandedGridView1.FocusedRowHandle != rowHandel_2)
                    {
                        bandedGridView1.FocusedRowHandle = rowHandel_2;
                    }
                    gridControl2.RefreshDataSource();
                }
                else
                {
                    NtbSoft.ERP.Libs.clsConvert<DongThungThongTinPOCTEntity> convert = new Libs.clsConvert<DongThungThongTinPOCTEntity>();
                    DataTable tblnull = convert.ToDataTable(lstDTPOCT);
                    gridControl2.DataSource = tblnull;
                    gridControl2.RefreshDataSource();
                }

            }

            if (lcsx == 1)
                _makho = _objmadvsxd;
            else
                _makho = _objmadvsx;
            string urlKho = string.Format("{0}?madvsx={1}&&lcsx={2}", URL + "NhapKho/GetMaKho", _objMaDVSXHoanThanh, lcsx);
            string jsonKho = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKho); }).Result;
            tblKho = JsonConvert.DeserializeObject<DataTable>(jsonKho);
            searchLookUpEditKho.Properties.DataSource = tblKho;
            searchLookUpEditKho.Properties.DisplayMember = "TenKho";
            searchLookUpEditKho.Properties.ValueMember = "MaKho";
            searchLookUpEditKho.Properties.ShowClearButton = false;
            searchLookUpEditKho.Properties.NullText = "[Chọn kho]";

            GridView dvView = searchLookUpEditKho.Properties.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaKho", Caption = "Mã kho", Name = "colMaKho", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenKho", Caption = "Kho", Name = "colTenKho", Visible = true });

            }

        }
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            LoadNhapKho();
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
            LoadNKPO();
            LoadNhapKho();
        }

        private void barButtonItemNextt_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            rowHandel = 0;
            rowHandel_1 = 0;
            rowHandel_2 = 0;
            pageIndex = pageIndex + 1;
            barStaticItemPageIndexx.Caption = pageIndex.ToString();

            LoadData();
            LoadNKPO();
            LoadNhapKho();
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
                LoadNKPO();
                LoadNhapKho();
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

        private void btcNhapKhoBarcode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmScanBarcodeDT_NK frm = new frmScanBarcodeDT_NK("NK");
            frm.ShowDialog();
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
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            if (searchLookUpEditKho.EditValue == null && (bool)e.Value == true)
            {
                MessageBox.Show("Vui lòng chọn kho.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                return;
            }

            GridView view = sender as GridView;
            if (view != null)
            {
                DataRow focusedRow = view.GetDataRow(view.FocusedRowHandle);
                if (focusedRow == null) return;
                string _Sizeheader = "SoLuong";
                string filterExpression = "";
                if (!_sms)
                {
                    filterExpression = $"MaDH = '{_madhdt}' AND MaDVSX = '{view.GetFocusedRowCellValue("MaDVSX")}' AND " +
                   $"MaPKL = '{view.GetFocusedRowCellValue("MaPKL")}' AND MaLenh = '{_malenhdt}' AND " +
                   $"POID = '{view.GetFocusedRowCellValue("POID")}' AND TuThung = '{view.GetFocusedRowCellValue("TuThung")}'" +
                   $"AND DenThung = '{view.GetFocusedRowCellValue("DenThung")}'";
                }
                else
                {
                    filterExpression = $"MaDH = '{_madhdt}' AND MaDVSX = '{view.GetFocusedRowCellValue("MaDVSX")}' AND " +
                   $"MaPKL = '{view.GetFocusedRowCellValue("MaPKL")}' AND " +
                   $"POID = '{view.GetFocusedRowCellValue("POID")}' AND TuThung = '{view.GetFocusedRowCellValue("TuThung")}'" +
                   $"AND DenThung = '{view.GetFocusedRowCellValue("DenThung")}'";
                }
                DataRow[] filteredRows = _dtRowSave.Select(filterExpression);
                if (filteredRows.Length == 0)
                {
                    DataRow _dr = _dtRowSave.NewRow();
                    _dr["MaDH"] = _madhdt;
                    _dr["MaPKL"] = _mapkldt;
                    _dr["MaDVSX"] = _madvsxdt;
                    _dr["MaLenh"] = _malenhdt;
                    _dr["POID"] = _podt;
                    if ((bool)e.Value == false)
                    {
                        if (lcsx == 0)
                            _dr["MaKho"] = "";
                        else
                            _dr["MaKhoDen"] = "";
                    }
                    else
                    {
                        if (lcsx == 0)
                            _dr["MaKho"] = searchLookUpEditKho.EditValue.ToString();
                        else
                            _dr["MaKhoDen"] = searchLookUpEditKho.EditValue.ToString();
                    }
                    _dr["TuThung"] = focusedRow["TuThung"];
                    _dr["DenThung"] = focusedRow["DenThung"];
                    _dr["MinSttThung"] = focusedRow["MinSttThung"];
                    _dr["MaxSttThung"] = focusedRow["MaxSttThung"];
                    _dr["SttThungDecat"] = focusedRow["SttThung"];
                    _dr["CheckDT"] = e.Value;
                    _dtRowSave.Rows.Add(_dr);
                }
                else
                {
                    DataRow foundRow = filteredRows[0];
                    if ((bool)e.Value == false)
                    {
                        if (lcsx == 0)
                            foundRow["MaKho"] = "";
                        else
                            foundRow["MaKhoDen"] = "";
                    }
                    else
                    {
                        if (lcsx == 0)
                            foundRow["MaKho"] = searchLookUpEditKho.EditValue.ToString();
                        else
                            foundRow["MaKhoDen"] = searchLookUpEditKho.EditValue.ToString();
                    }
                    foundRow["CheckDT"] = e.Value;
                }
                DataTable dtDongThung = this.gridControl2.DataSource as DataTable;
                if (focusedRow == null) return;
                foreach (DataRow _dr in dtDongThung.Rows)
                {
                    if (_dr["SttThung"].ToString() == focusedRow["SttThung"].ToString())
                        _dr["CheckDT"] = e.Value;
                }
            }
        }


        private void ChiTietDT_Click(object sender, EventArgs e)
        {
            //frmChiTietNhapKho frm = new frmChiTietNhapKho(_mapkldtct, _malenhdtct, _madvsxdtct, _poiddtct, _dausizeiddtct, _maudtct, _tuthung, _denthung, lcsx, _sttthung, _minsttthung, _maxsttthung, tblKho, _isTonKho, _makhodtct, _sms);
            //frm.ShowDialog();
        }

        private void bandedGridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = (GridView)sender;
            object _objmadh = null, _objmapkl = null, _objmadvsx = null, _objtendvsx = null, _objmalenh = null, _objpoid = null,
                _objdausizeid = null, _objmau = null, _objpo = null, _objtenlenh = null, _objmalenhsx = null, _objmakho = null;
            int _objtuthung = 0, _objdenthung = 0, _objsttthung = 0, _objMinSttThung = 0, _objMaxSttThung = 0;
            DataRow focusedRow = view.GetDataRow(view.FocusedRowHandle);
            if (focusedRow == null) return;
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
            _objmakho = focusedRow["MaKho"];
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
            if (_objmakho != null)
                _makhodtct = _objmakho.ToString();
        }
        private void barCheckItemlcxh_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if ((bool)barCheckItemlcxh.Checked == true)
            {
                lcsx = 1;
                colNTenDVSXD.Visible = true;
                colTenDVSX.Caption = "Từ đơn vị SX";
                gridBand12.Visible = true;
                gridBand16.Visible = true;
                gridBand4.Caption = "Từ đơn vị SX";
                gridBand17.Caption = "Từ Kho";
                LoadData();
                searchLookUpEditKho.EditValue = null;
                layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
               
            }
            else
            {
                lcsx = 0;
                colNTenDVSXD.Visible = false;
                colTenDVSX.Caption = "Đơn vị SX";
                gridBand12.Visible = false;
                gridBand16.Visible = false;
                gridBand4.Caption = "Đơn vị SX";
                gridBand17.Caption = "Kho";
                LoadData();
                searchLookUpEditKho.EditValue = null;
                layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem15.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem18.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem19.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
               
            }

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
        private void simpleButtonAll_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel_1 = gridView2.FocusedRowHandle;
            rowHandel = gridView1.FocusedRowHandle;
            DataTable dtNhapKho = this.gridControl2.DataSource as DataTable;
            if (searchLookUpEditKho.EditValue == null)
            {
                MessageBox.Show("Vui lòng chọn kho.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (chkNhap.Checked == true)
            {
                if (searchLookUpEditDay.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn Dãy.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    if (searchLookUpEditKe.EditValue == null)
                    {
                        XtraMessageBox.Show("Vui lòng chọn Kệ.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        if (searchLookUpEditTang.EditValue == null)
                        {
                            XtraMessageBox.Show("Vui lòng chọn Tầng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            if (searchLookUpEditO.EditValue == null)
                            {
                                XtraMessageBox.Show("Vui lòng chọn Ô.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }
                }
            }
            frmNhapKhoAll frm = new frmNhapKhoAll();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                DataTable _dtVTK = new DataTable();
                string ngaynhapkho = frm.NgayNhapKho;
                DataTable dtSave = new DataTable();
                dtSave.Columns.Add("MaDH", typeof(string));
                dtSave.Columns.Add("MaPKL", typeof(string));
                dtSave.Columns.Add("MaDVSX", typeof(string));
                dtSave.Columns.Add("MaLenh", typeof(string));
                dtSave.Columns.Add("POID", typeof(string));
                dtSave.Columns.Add("MaKho", typeof(string));
                dtSave.Columns.Add("MaKhoDen", typeof(string));
                dtSave.Columns.Add("TuThung", typeof(int));
                dtSave.Columns.Add("DenThung", typeof(int));
                dtSave.Columns.Add("MinSttThung", typeof(int));
                dtSave.Columns.Add("MaxSttThung", typeof(int));
                dtSave.Columns.Add("SttThungDecat", typeof(int));
                dtSave.Columns.Add("NgayNhapKho_TC", typeof(string));
                foreach (DataRow dr in dtNhapKho.Rows)
                {
                    if ((bool)dr["CheckDT"] == false)
                    {
                        DataRow _dr = dtSave.NewRow();
                        _dr["MaDH"] = _madhdt;
                        _dr["MaPKL"] = _mapkldt;
                        _dr["MaDVSX"] = _madvsxdt;
                        if (_sms)
                            _dr["MaLenh"] = dr["MaLenh"];
                        else
                            _dr["MaLenh"] = _malenhdtct;
                        _dr["POID"] = _podt;
                        _dr["MaKho"] = _makhodtct.ToString() == "" ? searchLookUpEditKho.EditValue.ToString() : _makhodtct;
                        _dr["MaKhoDen"] = searchLookUpEditKho.EditValue.ToString();
                        _dr["TuThung"] = dr["TuThung"];
                        _dr["DenThung"] = dr["DenThung"];
                        _dr["MinSttThung"] = dr["MinSttThung"];
                        _dr["MaxSttThung"] = dr["MaxSttThung"];
                        _dr["NgayNhapKho_TC"] = ngaynhapkho;
                        dtSave.Rows.Add(_dr);
                    }
                }
                if (dtSave.Rows.Count <= 0)
                    return;

                if (chkNhap.Checked == true)
                {
                    string urlVTK = string.Format("{0}?", URL + "NhapKho/GetVTK");
                    string json = Task.Run(async () => { return await _clientExtension.PostAsync(urlVTK, dtSave); }).Result;
                    _dtVTK = JsonConvert.DeserializeObject<DataTable>(json);
                    bool isEmpty = !_dtVTK.AsEnumerable()
                               .Any(row => row["CBM"] != null &&
                                            row["CBM"] != "" &&
                                            row["CBM"] != DBNull.Value.ToString());

                    if (isEmpty)
                    {
                        XtraMessageBox.Show("Vui lòng khai báo CBM cho thùng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    decimal total = Math.Round(_dtVTK.AsEnumerable().Sum(row => Convert.ToDecimal(row["CBM"])), 4);
                    if (total > _cbm)
                    {
                        XtraMessageBox.Show("CBM các thùng vượt quá sức chứa trong Ô. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    foreach (DataRow _dr in _dtVTK.Rows)
                    {
                        _dr["MaKhuVuc"] = searchLookUpEditDay.EditValue.ToString();
                        _dr["MaKe"] = searchLookUpEditKe.EditValue.ToString();
                        _dr["MaTang"] = searchLookUpEditTang.EditValue.ToString();
                        _dr["MaO"] = searchLookUpEditO.EditValue.ToString();
                        _dr["MaKho"] = searchLookUpEditKho.EditValue.ToString();
                        _dr["NVNhap"] = GlobleData.UserName;
                    }
                }
                string url = "";
                if (!_sms)
                    url = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}&&mahang={4}&&po={5}", URL + "NhapKho/PostALL", true, lcsx, 2, _mahang, _poiddt);
                else
                    url = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}&&mahang={4}&&po={5}", URL + "NhapKho/PostALLSMS", true, lcsx, 2, _mahang, _poiddt);
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
                if (result.ToLower() != "true")
                    XtraMessageBox.Show(result);
                else
                {
                    if (chkNhap.Checked == true)
                    {
                        if (_dtVTK != null && _dtVTK.Rows.Count > 0)
                        {
                            string urlsaveVTK = string.Format("{0}?", URL + "NhapKho/PostVTK");
                            string resultsaveVTK = Task.Run(async () => { return await _clientExtension.PostAsync(urlsaveVTK, _dtVTK); }).Result;
                            if (resultsaveVTK.ToLower() != "true")
                                XtraMessageBox.Show(resultsaveVTK);
                        }
                    }

                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadNhapKho();
                    LoadCBM();
                }
            }
        }

        private void simpleButtonHuy_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            DataTable dtNhapKho = this.gridControl2.DataSource as DataTable;
            DataTable dtSave = new DataTable();
            dtSave.Columns.Add("MaDH", typeof(string));
            dtSave.Columns.Add("MaPKL", typeof(string));
            dtSave.Columns.Add("MaDVSX", typeof(string));
            dtSave.Columns.Add("MaLenh", typeof(string));
            dtSave.Columns.Add("POID", typeof(string));
            dtSave.Columns.Add("MaKho", typeof(string));
            dtSave.Columns.Add("MaKhoDen", typeof(string));
            dtSave.Columns.Add("TuThung", typeof(int));
            dtSave.Columns.Add("DenThung", typeof(int));
            dtSave.Columns.Add("MinSttThung", typeof(int));
            dtSave.Columns.Add("MaxSttThung", typeof(int));
            dtSave.Columns.Add("SttThungDecat", typeof(int));
            dtSave.Columns.Add("NgayNhapKho_TC", typeof(DateTime));
            foreach (DataRow dr in dtNhapKho.Rows)
            {
                DataRow _dr = dtSave.NewRow();
                _dr["MaDH"] = _madhdt;
                _dr["MaPKL"] = _mapkldt;
                _dr["MaDVSX"] = _madvsxdt;
                if (_sms)
                    _dr["MaLenh"] = dr["MaLenh"];
                else
                    _dr["MaLenh"] = _malenhdtct;
                _dr["POID"] = _podt;
                _dr["MaKho"] = "";
                _dr["MaKhoDen"] = "";
                _dr["TuThung"] = dr["TuThung"];
                _dr["DenThung"] = dr["DenThung"];
                _dr["MinSttThung"] = dr["MinSttThung"];
                _dr["MaxSttThung"] = dr["MaxSttThung"];
                dtSave.Rows.Add(_dr);

            }
            DataTable dtKT = new DataTable();
            dtKT.Columns.Add("MaDH", typeof(string));
            dtKT.Columns.Add("MaPKL", typeof(string));
            dtKT.Columns.Add("MaDVSX", typeof(string));
            dtKT.Columns.Add("MaLenh", typeof(string));
            dtKT.Columns.Add("POID", typeof(string));
            dtKT.Columns.Add("MaKho", typeof(string));
            dtKT.Columns.Add("MaKhoDen", typeof(string));
            dtKT.Columns.Add("TuThung", typeof(int));
            dtKT.Columns.Add("DenThung", typeof(int));
            dtKT.Columns.Add("MinSttThung", typeof(int));
            dtKT.Columns.Add("MaxSttThung", typeof(int));
            dtKT.Columns.Add("SttThungDecat", typeof(int));
            foreach (DataRow _drow in dtSave.Rows)
            {
                DataRow _dr = dtKT.NewRow();
                _dr["MaDH"] = _drow["MaDH"];
                _dr["MaPKL"] = _drow["MaDH"];
                _dr["MaDVSX"] = _drow["MaDVSX"];
                _dr["MaLenh"] = _drow["MaLenh"];
                _dr["POID"] = _drow["POID"];
                _dr["TuThung"] = _drow["TuThung"];
                _dr["DenThung"] = _drow["DenThung"];
                _dr["MinSttThung"] = _drow["MinSttThung"];
                _dr["MaxSttThung"] = _drow["MaxSttThung"];
                _dr["SttThungDecat"] = _drow["SttThungDecat"];
                dtKT.Rows.Add(_dr);
            }
            string urlKT = string.Format("{0}?", URL + "NhapKho/GetKiemTra");
            string jsonKT = Task.Run(async () => { return await _clientExtension.PostAsync(urlKT, dtKT); }).Result;
            DataTable _dtKT = JsonConvert.DeserializeObject<DataTable>(jsonKT);
            if (_dtKT != null && _dtKT.Rows.Count > 0)
            {
                DialogResult messResult = MessageBox.Show("Một số thùng đã được xác nhận giao. Chỉ hủy thùng chưa được xác nhận giao.!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.No)
                {
                    return;
                }
            }
            string url = "";
            if (!_sms)
                url = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}&&mahang={4}&&po={5}", URL + "NhapKho/PostALL", false, lcsx, 1, _mahang, _poiddt);
            else
                url = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}&&mahang={4}&&po={5}", URL + "NhapKho/PostALLSMS", false, lcsx, 1, _mahang, _poiddt);
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, dtSave); }).Result;
            if (result.ToLower() != "true")
                XtraMessageBox.Show(result);
            else
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadNKPO();
                // gridView1.FocusedRowHandle = rowHandel_1;
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
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            if (searchLookUpEditKho.EditValue == null)
            {
                MessageBox.Show("Vui lòng chọn kho.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (chkNhap.Checked == true)
            {
                if (searchLookUpEditDay.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn Dãy.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    if (searchLookUpEditKe.EditValue == null)
                    {
                        XtraMessageBox.Show("Vui lòng chọn Kệ.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        if (searchLookUpEditTang.EditValue == null)
                        {
                            XtraMessageBox.Show("Vui lòng chọn Tầng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            if (searchLookUpEditO.EditValue == null)
                            {
                                XtraMessageBox.Show("Vui lòng chọn Ô.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }
                }
            }
            DataTable _tblCT = new DataTable();
            if (!_sms)
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&lcsx={9}&&minsttthung={10}&&maxsttthung={11}", URL + "NhapKho/GetThongTinChiTiet", _mapkldtct, _malenhdtct, _madvsxdtct, _podt, _dausizeiddtct, _madh, _tuthung, _denthung, lcsx, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblCT = JsonConvert.DeserializeObject<DataTable>(json);
            }
            else
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&lcsx={9}&&minsttthung={10}&&maxsttthung={11}", URL + "NhapKho/GetThongTinChiTietSMS", _mapkldtct, _malenhdtct, _madvsxdtct, _podt, _dausizeiddtct, _madh, _tuthung, _denthung, lcsx, _minsttthung, _maxsttthung);
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

            frmNhapKhoSoLuong frm = new frmNhapKhoSoLuong(lstDongThung, string.Join(";", lstSize.Distinct()), false, _mapkldt, _tenlenhdt, 1, _poiddt);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                DataTable _dtVTK = new DataTable();
                int i = 0;
                int soluong = frm.SoLuong;
                string ngaynhapkho = frm.NgayNhapKho;
                if (soluong <= 0) return;
                bool isdongthung = true;
                DataTable dtSave = new DataTable();
                dtSave.Columns.Add("MaDH", typeof(string));
                dtSave.Columns.Add("MaPKL", typeof(string));
                dtSave.Columns.Add("MaDVSX", typeof(string));
                dtSave.Columns.Add("MaLenh", typeof(string));
                dtSave.Columns.Add("POID", typeof(string));
                dtSave.Columns.Add("MaKho", typeof(string));
                dtSave.Columns.Add("MaKhoDen", typeof(string));
                dtSave.Columns.Add("TuThung", typeof(int));
                dtSave.Columns.Add("DenThung", typeof(int));
                dtSave.Columns.Add("MinSttThung", typeof(int));
                dtSave.Columns.Add("MaxSttThung", typeof(int));
                dtSave.Columns.Add("SttThungDecat", typeof(int));
                dtSave.Columns.Add("NgayNhapKho_TC", typeof(string));
                if (!_sms)
                {
                    foreach (DataRow row in _tblCT.Rows)
                    {

                        if (i == soluong)
                            break;
                        if (Convert.ToInt32(row["CheckDT"]) == 0)
                        {
                            DataRow _dr = dtSave.NewRow();
                            _dr["MaDH"] = _madhdt;
                            _dr["MaPKL"] = row["MaPKL"];
                            _dr["MaLenh"] = row["MaLenh"];
                            _dr["MaDVSX"] = row["MaDVSX"];
                            _dr["POID"] = _podt;//row["POID"];
                            _dr["MaKho"] = _makhodtct.ToString() == "" ? searchLookUpEditKho.EditValue.ToString() : _makhodtct;
                            _dr["MaKhoDen"] = searchLookUpEditKho.EditValue.ToString();
                            _dr["TuThung"] = _tuthung;
                            _dr["DenThung"] = _denthung;
                            _dr["MinSttThung"] = _minsttthung;
                            _dr["MaxSttThung"] = _maxsttthung;
                            _dr["SttThungDecat"] = row["SttDT"];
                            _dr["NgayNhapKho_TC"] = ngaynhapkho;
                            dtSave.Rows.Add(_dr);
                            i++;
                        }
                    }
                }
                else
                {
                    foreach (DataRow row in _tblCT.Rows)
                    {
                        if (Convert.ToInt32(row["CheckDT"]) == 0)
                        {
                            DataRow _dr = dtSave.NewRow();
                            _dr["MaDH"] = _madhdt;
                            _dr["MaPKL"] = row["MaPKL"];
                            _dr["MaLenh"] = row["MaLenh"];
                            _dr["MaDVSX"] = row["MaDVSX"];
                            _dr["POID"] = _podt; //row["POID"];
                            _dr["MaKho"] = _makhodtct.ToString() == "" ? searchLookUpEditKho.EditValue.ToString() : _makhodtct;
                            _dr["MaKhoDen"] = searchLookUpEditKho.EditValue.ToString();
                            _dr["TuThung"] = _tuthung;
                            _dr["DenThung"] = _denthung;
                            _dr["MinSttThung"] = _minsttthung;
                            _dr["MaxSttThung"] = _maxsttthung;
                            _dr["SttThungDecat"] = row["SttThung"];
                            _dr["NgayNhapKho_TC"] = ngaynhapkho;
                            dtSave.Rows.Add(_dr);
                        }
                    }
                }
                if ((bool)barCheckItemlcxh.Checked == true)
                {
                    lcsx = 1;
                    statuschuyen = 2;
                }
                if (dtSave.Rows.Count <= 0) return;
                if (chkNhap.Checked == true)
                {
                    string urlVTK = string.Format("{0}?", URL + "NhapKho/GetVTKV1");
                    string jsonVTK = Task.Run(async () => { return await _clientExtension.PostAsync(urlVTK, dtSave); }).Result;
                    _dtVTK = JsonConvert.DeserializeObject<DataTable>(jsonVTK);
                    bool isEmpty = !_dtVTK.AsEnumerable()
                              .Any(row => row["CBM"] != null &&
                                           row["CBM"] != "" &&
                                           row["CBM"] != DBNull.Value.ToString());

                    if (isEmpty)
                    {
                        XtraMessageBox.Show("Vui lòng khai báo CBM cho thùng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    decimal total = Math.Round(_dtVTK.AsEnumerable().Sum(row => Convert.ToDecimal(row["CBM"])), 4);
                    if (total > _cbm)
                    {
                        XtraMessageBox.Show("CBM các thùng vượt quá sức chứa trong Ô. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    foreach (DataRow _dr in _dtVTK.Rows)
                    {
                        _dr["MaKhuVuc"] = searchLookUpEditDay.EditValue.ToString();
                        _dr["MaKe"] = searchLookUpEditKe.EditValue.ToString();
                        _dr["MaTang"] = searchLookUpEditTang.EditValue.ToString();
                        _dr["MaO"] = searchLookUpEditO.EditValue.ToString();
                        _dr["MaKho"] = searchLookUpEditKho.EditValue.ToString();
                        _dr["NVNhap"] = GlobleData.UserName;
                    }
                }
                string urlSave = "";
                if (!_sms)
                    urlSave = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}", URL + "NhapKho/PostDT", isdongthung, lcsx, statuschuyen);
                else
                    urlSave = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}", URL + "NhapKho/PostDTSMS", isdongthung, lcsx, statuschuyen);
                string resultSave = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, dtSave); }).Result;
                if (resultSave.ToLower() != "true")
                    XtraMessageBox.Show(resultSave);
                else
                {
                    if (chkNhap.Checked == true)
                    {
                        if (_dtVTK != null && _dtVTK.Rows.Count > 0)
                        {
                            string urlsaveVTK = string.Format("{0}?", URL + "NhapKho/PostVTK");
                            string resultsaveVTK = Task.Run(async () => { return await _clientExtension.PostAsync(urlsaveVTK, _dtVTK); }).Result;
                            if (resultsaveVTK.ToLower() != "true")
                                XtraMessageBox.Show(resultsaveVTK);
                        }
                    }
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    LoadNKPO();
                    LoadCBM();
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            //if (searchLookUpEditKho.EditValue == null)
            //{
            //    MessageBox.Show("Vui lòng chọn kho.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            DataTable _tblCT = new DataTable();
            if (!_sms)
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&lcsx={9}&&minsttthung={10}&&maxsttthung={11}", URL + "NhapKho/GetThongTinChiTiet", _mapkldtct, _malenhdtct, _madvsxdtct, _podt, _dausizeiddtct, _madh, _tuthung, _denthung, lcsx, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblCT = JsonConvert.DeserializeObject<DataTable>(json);
            }
            else
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&lcsx={9}&&minsttthung={10}&&maxsttthung={11}", URL + "NhapKho/GetThongTinChiTietSMS", _mapkldtct, _malenhdtct, _madvsxdtct, _podt, _dausizeiddtct, _madh, _tuthung, _denthung, lcsx, _minsttthung, _maxsttthung);
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
            //frmNhapKhoSoLuong frm = new frmNhapKhoSoLuong();
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
                dtSave.Columns.Add("MaKho", typeof(string));
                dtSave.Columns.Add("MaKhoDen", typeof(string));
                dtSave.Columns.Add("TuThung", typeof(int));
                dtSave.Columns.Add("DenThung", typeof(int));
                dtSave.Columns.Add("MinSttThung", typeof(int));
                dtSave.Columns.Add("MaxSttThung", typeof(int));
                dtSave.Columns.Add("SttThungDecat", typeof(int));
                dtSave.Columns.Add("NgayNhapKho_TC", typeof(string));
                if (!_sms)
                {
                    foreach (DataRow row in _tblCT.Rows)
                    {
                        if (i == soluong)
                            break;
                        if (Convert.ToInt32(row["CheckDT"]) == 1)
                        {
                            DataRow _dr = dtSave.NewRow();
                            _dr["MaDH"] = _madh; 
                            _dr["MaPKL"] = row["MaPKL"];
                            _dr["MaLenh"] = row["MaLenh"];
                            _dr["MaDVSX"] = row["MaDVSX"];
                            _dr["POID"] = _podt; //row["POID"];
                            _dr["MaKho"] = "";
                            _dr["MaKhoDen"] = "";
                            _dr["TuThung"] = _tuthung;
                            _dr["DenThung"] = _denthung;
                            _dr["MinSttThung"] = _minsttthung;
                            _dr["MaxSttThung"] = _maxsttthung;
                            _dr["SttThungDecat"] = row["SttDT"];
                            _dr["NgayNhapKho_TC"] = "";
                            dtSave.Rows.Add(_dr);
                            i++;
                        }
                    }
                }
                else
                {
                    foreach (DataRow row in _tblCT.Rows)
                    {
                        if (Convert.ToInt32(row["CheckDT"]) == 1)
                        {
                            DataRow _dr = dtSave.NewRow();
                            _dr["MaDH"] = _madh;//row["MaDH"];
                            _dr["MaPKL"] = row["MaPKL"];
                            _dr["MaLenh"] = row["MaLenh"];
                            _dr["MaDVSX"] = row["MaDVSX"];
                            _dr["POID"] = _podt; //row["POID"];
                            _dr["MaKho"] = "";
                            _dr["MaKhoDen"] = "";
                            _dr["TuThung"] = _tuthung;
                            _dr["DenThung"] = _denthung;
                            _dr["MinSttThung"] = _minsttthung;
                            _dr["MaxSttThung"] = _maxsttthung;
                            _dr["SttThungDecat"] = row["SttDT"];
                            _dr["NgayNhapKho_TC"] = "";
                            dtSave.Rows.Add(_dr);
                        }
                    }

                }

                if ((bool)barCheckItemlcxh.Checked == true)
                {
                    lcsx = 1;
                    statuschuyen = 1;
                }
                if (dtSave == null || dtSave.Rows.Count == 0) return;
                DataTable dtKT = new DataTable();
                dtKT.Columns.Add("MaDH", typeof(string));
                dtKT.Columns.Add("MaPKL", typeof(string));
                dtKT.Columns.Add("MaDVSX", typeof(string));
                dtKT.Columns.Add("MaLenh", typeof(string));
                dtKT.Columns.Add("POID", typeof(string));
                dtKT.Columns.Add("MaKho", typeof(string));
                dtKT.Columns.Add("MaKhoDen", typeof(string));
                dtKT.Columns.Add("TuThung", typeof(int));
                dtKT.Columns.Add("DenThung", typeof(int));
                dtKT.Columns.Add("MinSttThung", typeof(int));
                dtKT.Columns.Add("MaxSttThung", typeof(int));
                dtKT.Columns.Add("SttThungDecat", typeof(int));
                foreach (DataRow _drow in dtSave.Rows)
                {
                    DataRow _dr = dtKT.NewRow();
                    _dr["MaDH"] = _drow["MaDH"];
                    _dr["MaPKL"] = _drow["MaDH"];
                    _dr["MaDVSX"] = _drow["MaDVSX"];
                    _dr["MaLenh"] = _drow["MaLenh"];
                    _dr["POID"] = _drow["POID"];
                    _dr["TuThung"] = _drow["TuThung"];
                    _dr["DenThung"] = _drow["DenThung"];
                    _dr["MinSttThung"] = _drow["MinSttThung"];
                    _dr["MaxSttThung"] = _drow["MaxSttThung"];
                    _dr["SttThungDecat"] = _drow["SttThungDecat"];
                    dtKT.Rows.Add(_dr);
                }
                string urlKT = string.Format("{0}?", URL + "NhapKho/GetKiemTra");
                string jsonKT = Task.Run(async () => { return await _clientExtension.PostAsync(urlKT, dtKT); }).Result;
                DataTable _dtKT = JsonConvert.DeserializeObject<DataTable>(jsonKT);
                if (_dtKT != null && _dtKT.Rows.Count > 0)
                {
                    DialogResult messResult = MessageBox.Show("Một số thùng đã được xác nhận giao. Chỉ hủy thùng chưa được xác nhận giao.!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (messResult == DialogResult.No)
                    {
                        return;
                    }
                }
                string urlSave = "";
                if (!_sms)
                    urlSave = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}", URL + "NhapKho/PostHuy", isdongthung, lcsx, statuschuyen);
                else
                    urlSave = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}", URL + "NhapKho/PostHuySMS", isdongthung, lcsx, statuschuyen);
                string resultSave = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, dtSave); }).Result;
                if (resultSave.ToLower() != "true")
                    XtraMessageBox.Show(resultSave);
                else
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    LoadNKPO();
                    //gridView1.FocusedRowHandle = rowHandel_1;
                }
            }
        }

        private void bandedGridView1_DoubleClick(object sender, EventArgs e)
        {
            rowHandel_1 = gridView1.FocusedRowHandle;
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            GridView view = (GridView)sender;
            object _objmadh = null, _objmapkl = null, _objmadvsx = null, _objtendvsx = null, _objmalenh = null, _objpoid = null,
                _objdausizeid = null, _objmau = null, _objpo = null, _objtenlenh = null, _objmalenhsx = null, _objmakho = null;
            int _objtuthung = 0, _objdenthung = 0, _objsttthung = 0, _objMinSttThung = 0, _objMaxSttThung = 0;
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
            _objmakho = focusedRow["MaKho"];
            //if (_objmadh != null)
            //    _madhdtct = _objmadh.ToString();
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
            if (_objmakho != null)
                _makhodtct = _objmakho.ToString();
            //frmChiTietNhapKho frm = new frmChiTietNhapKho(_mapkldtct, _malenhdtct, _madvsxdtct, _podt, _dausizeiddtct, _maudtct, _tuthung, _denthung, lcsx, _sttthung, _minsttthung, _maxsttthung, tblKho, _isTonKho, _makhodtct, _sms, _maKH, _madh);
            //frm.ShowDialog();
            //LoadNKPO();
            //gridView1.FocusedRowHandle = rowHandel_1;
            //bandedGridView1.FocusedRowHandle = rowHandel_2;
        }

        private void btnNVT_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            if (searchLookUpEditKho.EditValue == null)
            {
                MessageBox.Show("Vui lòng chọn kho.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (chkNhap.Checked == true)
            {
                if (searchLookUpEditDay.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn Dãy.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    if (searchLookUpEditKe.EditValue == null)
                    {
                        XtraMessageBox.Show("Vui lòng chọn Kệ.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        if (searchLookUpEditTang.EditValue == null)
                        {
                            XtraMessageBox.Show("Vui lòng chọn Tầng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            if (searchLookUpEditO.EditValue == null)
                            {
                                XtraMessageBox.Show("Vui lòng chọn Ô.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }
                }
            }
            DataTable _tblCT = new DataTable();
            if (!_sms)
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&lcsx={9}&&minsttthung={10}&&maxsttthung={11}", URL + "NhapKho/GetThongTinChiTiet", _mapkldtct, _malenhdtct, _madvsxdtct, _podt, _dausizeiddtct, _madh, _tuthung, _denthung, lcsx, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblCT = JsonConvert.DeserializeObject<DataTable>(json);
            }
            else
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&lcsx={9}&&minsttthung={10}&&maxsttthung={11}", URL + "NhapKho/GetThongTinChiTietSMS", _mapkldtct, _malenhdtct, _madvsxdtct, _podt, _dausizeiddtct, _madh, _tuthung, _denthung, lcsx, _minsttthung, _maxsttthung);
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

            frmNhapViTri frm = new frmNhapViTri(lstDongThung, string.Join(";", lstSize.Distinct()), true, _mapkldt, _tenlenhdt, 1, _poiddt);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                DataTable _dtVTK = new DataTable();
                int TuThung = frm.TuThung;
                int DenThung = frm.DenThung;
                string ngaynhapkho = frm.NgayNhapKho;
                DataTable dtSave = new DataTable();
                dtSave.Columns.Add("MaDH", typeof(string));
                dtSave.Columns.Add("MaPKL", typeof(string));
                dtSave.Columns.Add("MaDVSX", typeof(string));
                dtSave.Columns.Add("MaLenh", typeof(string));
                dtSave.Columns.Add("POID", typeof(string));
                dtSave.Columns.Add("MaKho", typeof(string));
                dtSave.Columns.Add("MaKhoDen", typeof(string));
                dtSave.Columns.Add("TuThung", typeof(int));
                dtSave.Columns.Add("DenThung", typeof(int));
                dtSave.Columns.Add("MinSttThung", typeof(int));
                dtSave.Columns.Add("MaxSttThung", typeof(int));
                dtSave.Columns.Add("SttThungDecat", typeof(int));
                dtSave.Columns.Add("NgayNhapKho_TC", typeof(string));
                int _tuThung = _tblCT.AsEnumerable().Where(row => Convert.ToInt32(row["SttThung"]) == Convert.ToInt32(TuThung)).Sum(row => Convert.ToInt32(row["SttDT"]));
                int _denThung = _tblCT.AsEnumerable().Where(row => Convert.ToInt32(row["SttThung"]) == Convert.ToInt32(DenThung)).Sum(row => Convert.ToInt32(row["SttDT"]));
                DataRow _dr = dtSave.NewRow();
                _dr["MaDH"] = _madhdt;
                _dr["MaPKL"] = _mapkldtct;
                _dr["MaLenh"] = _malenhdtct;
                _dr["MaDVSX"] = _madvsxdtct;
                _dr["POID"] = _podt;
                _dr["MaKho"] = _makhodtct.ToString() == "" ? searchLookUpEditKho.EditValue.ToString() : _makhodtct;
                _dr["MaKhoDen"] = searchLookUpEditKho.EditValue.ToString();
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
                _dr["MinSttThung"] = _minsttthung;
                _dr["MaxSttThung"] = _maxsttthung;
                _dr["SttThungDecat"] = 0;
                _dr["NgayNhapKho_TC"] = ngaynhapkho;
                dtSave.Rows.Add(_dr);
                if ((bool)barCheckItemlcxh.Checked == true)
                {
                    lcsx = 1;
                    statuschuyen = 2;
                }
                if (dtSave == null || dtSave.Rows.Count == 0) return;
                if (chkNhap.Checked == true)
                {
                    string urlVTK = string.Format("{0}?", URL + "NhapKho/GetVTKV2");
                    string jsonVTK = Task.Run(async () => { return await _clientExtension.PostAsync(urlVTK, dtSave); }).Result;
                    _dtVTK = JsonConvert.DeserializeObject<DataTable>(jsonVTK);
                    bool isEmpty = _dtVTK.AsEnumerable()
                              .Any(row => row["CBM"] == null ||
                                           Convert.ToDecimal(row["CBM"]) == 0 ||
                                           row["CBM"] == "" ||
                                           row["CBM"] == DBNull.Value.ToString());

                    if (isEmpty)
                    {
                        XtraMessageBox.Show("Vui lòng khai báo CBM cho thùng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    decimal total = Math.Round(_dtVTK.AsEnumerable().Sum(row => Convert.ToDecimal(row["CBM"])), 4);
                    if (total > _cbm)
                    {
                        XtraMessageBox.Show("CBM các thùng vượt quá sức chứa trong Ô. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    foreach (DataRow row in _dtVTK.Rows)
                    {
                        row["MaKhuVuc"] =  searchLookUpEditDay.EditValue.ToString();
                        row["MaKe"] = searchLookUpEditKe.EditValue.ToString();
                        row["MaTang"] = searchLookUpEditTang.EditValue.ToString();
                        row["MaO"] = searchLookUpEditO.EditValue.ToString();
                        row["MaKho"] = searchLookUpEditKho.EditValue.ToString();
                        row["NVNhap"] = GlobleData.UserName;
                    }

                }
                string urlSave = "";
                if (!_sms)
                    urlSave = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}", URL + "NhapKho/PostViTri", true, lcsx, statuschuyen);
                else
                    urlSave = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}", URL + "NhapKho/PostViTriSMS", true, lcsx, statuschuyen);
                string resultSave = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, dtSave); }).Result;
                if (resultSave.ToLower() != "true")
                    XtraMessageBox.Show(resultSave);
                else
                {
                    if (chkNhap.Checked == true)
                    {
                        if (_dtVTK != null && _dtVTK.Rows.Count > 0)
                        {
                            string urlsaveVTK = string.Format("{0}?", URL + "NhapKho/PostVTK");
                            string resultsaveVTK = Task.Run(async () => { return await _clientExtension.PostAsync(urlsaveVTK, _dtVTK); }).Result;
                            if (resultsaveVTK.ToLower() != "true")
                                XtraMessageBox.Show(resultsaveVTK);
                        }
                    }
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    LoadNKPO();
                    LoadCBM();
                }
            }
        }

        private void btnHUYVT_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            DataTable _tblCT = new DataTable();
            if (!_sms)
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&lcsx={9}&&minsttthung={10}&&maxsttthung={11}", URL + "NhapKho/GetThongTinChiTiet", _mapkldtct, _malenhdtct, _madvsxdtct, _poiddtct, _dausizeiddtct, _maudtct, _tuthung, _denthung, lcsx, _minsttthung, _maxsttthung);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                _tblCT = JsonConvert.DeserializeObject<DataTable>(json);
            }
            else
            {
                string url = string.Format("{0}?mapkl={1}&&malenh={2}&&madvsx={3}&&poid={4}&&dausizeid={5}&&mau={6}&&tuthung={7}&&denthung={8}&&lcsx={9}&&minsttthung={10}&&maxsttthung={11}", URL + "NhapKho/GetThongTinChiTietSMS", _mapkldtct, _malenhdtct, _madvsxdtct, _poiddtct, _dausizeiddtct, _maudtct, _tuthung, _denthung, lcsx, _minsttthung, _maxsttthung);
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
            frmNhapViTri frm = new frmNhapViTri(lstDongThung, string.Join(";", lstSize.Distinct()), true, _mapkldt, _tenlenhdt, 0, _poiddt);
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
                dtSave.Columns.Add("MaKho", typeof(string));
                dtSave.Columns.Add("MaKhoDen", typeof(string));
                dtSave.Columns.Add("TuThung", typeof(int));
                dtSave.Columns.Add("DenThung", typeof(int));
                dtSave.Columns.Add("MinSttThung", typeof(int));
                dtSave.Columns.Add("MaxSttThung", typeof(int));
                dtSave.Columns.Add("SttThungDecat", typeof(int));
                dtSave.Columns.Add("NgayNhapKho_TC", typeof(string));
                int _tuThung = _tblCT.AsEnumerable().Where(row => Convert.ToInt32(row["SttThung"]) == Convert.ToInt32(TuThung)).Sum(row => Convert.ToInt32(row["SttDT"]));
                int _denThung = _tblCT.AsEnumerable().Where(row => Convert.ToInt32(row["SttThung"]) == Convert.ToInt32(DenThung)).Sum(row => Convert.ToInt32(row["SttDT"]));
                DataRow _dr = dtSave.NewRow();
                _dr["MaDH"] = _madhdt;
                _dr["MaPKL"] = _mapkldtct;
                _dr["MaLenh"] = _malenhdtct;
                _dr["MaDVSX"] = _madvsxdtct;
                _dr["POID"] = _podt; 
                _dr["MaKho"] = "";
                _dr["MaKhoDen"] = "";
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
                _dr["NgayNhapKho_TC"] = "";
                dtSave.Rows.Add(_dr);
                if ((bool)barCheckItemlcxh.Checked == true)
                {
                    lcsx = 1;
                    statuschuyen = 1;
                }
                if (dtSave == null || dtSave.Rows.Count == 0) return;
                DataTable dtKT = new DataTable();
                dtKT.Columns.Add("MaDH", typeof(string));
                dtKT.Columns.Add("MaPKL", typeof(string));
                dtKT.Columns.Add("MaDVSX", typeof(string));
                dtKT.Columns.Add("MaLenh", typeof(string));
                dtKT.Columns.Add("POID", typeof(string));
                dtKT.Columns.Add("MaKho", typeof(string));
                dtKT.Columns.Add("MaKhoDen", typeof(string));
                dtKT.Columns.Add("TuThung", typeof(int));
                dtKT.Columns.Add("DenThung", typeof(int));
                dtKT.Columns.Add("MinSttThung", typeof(int));
                dtKT.Columns.Add("MaxSttThung", typeof(int));
                dtKT.Columns.Add("SttThungDecat", typeof(int));
                foreach (DataRow _drow in dtSave.Rows)
                {
                    DataRow dr = dtKT.NewRow();
                    dr["MaDH"] = _drow["MaDH"];
                    dr["MaPKL"] = _drow["MaDH"];
                    dr["MaDVSX"] = _drow["MaDVSX"];
                    dr["MaLenh"] = _drow["MaLenh"];
                    dr["POID"] = _drow["POID"];
                    dr["TuThung"] = _drow["TuThung"];
                    dr["DenThung"] = _drow["DenThung"];
                    dr["MinSttThung"] = _drow["MinSttThung"];
                    dr["MaxSttThung"] = _drow["MaxSttThung"];
                    dr["SttThungDecat"] = _drow["SttThungDecat"];
                    dtKT.Rows.Add(dr);
                }
                string urlKT = string.Format("{0}?", URL + "NhapKho/GetKiemTraViTri");
                string jsonKT = Task.Run(async () => { return await _clientExtension.PostAsync(urlKT, dtKT); }).Result;
                DataTable _dtKT = JsonConvert.DeserializeObject<DataTable>(jsonKT);
                if (_dtKT != null && _dtKT.Rows.Count > 0)
                {
                    DialogResult messResult = MessageBox.Show("Một số thùng đã được xác nhận giao. Chỉ hủy thùng chưa được xác nhận giao.!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (messResult == DialogResult.No)
                    {
                        return;
                    }
                }
                string urlSave = "";
                if (!_sms)
                    urlSave = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}", URL + "NhapKho/PostViTri", false, lcsx, statuschuyen);
                else
                    urlSave = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}", URL + "NhapKho/PostViTriSMS", false, lcsx, statuschuyen);
                string resultSave = Task.Run(async () => { return await _clientExtension.PostAsync(urlSave, dtSave); }).Result;
                if (resultSave.ToLower() != "true")
                    XtraMessageBox.Show(resultSave);
                else
                {
                    clsWaitForm.ShowSuccessForm(this, 3000);
                    LoadNKPO();
                }
            }
        }
        private void CheckVTK()
        {
            if (chkNhap.Checked == true)
            {
                if (searchLookUpEditDay.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn Dãy.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    if (searchLookUpEditKe.EditValue == null)
                    {
                        XtraMessageBox.Show("Vui lòng chọn Kệ.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        if (searchLookUpEditTang.EditValue == null)
                        {
                            XtraMessageBox.Show("Vui lòng chọn Tầng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            if (searchLookUpEditO.EditValue == null)
                            {
                                XtraMessageBox.Show("Vui lòng chọn Ô.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }
                }
            }
        }

        public static DataTable CloneAndModify(DataTable originalTable)
        {
            DataTable newTable = originalTable.Clone();
            foreach (DataRow row in originalTable.Rows)
            {
                newTable.ImportRow(row);
            }
            if (newTable.Columns.Contains("CheckDT"))
            {
                newTable.Columns.Remove("CheckDT");
            }

            return newTable;
        }

        private void btnNKChon_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            if (chkNhap.Checked == true)
            {
                if (searchLookUpEditDay.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn Dãy.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    if (searchLookUpEditKe.EditValue == null)
                    {
                        XtraMessageBox.Show("Vui lòng chọn Kệ.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        if (searchLookUpEditTang.EditValue == null)
                        {
                            XtraMessageBox.Show("Vui lòng chọn Tầng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            if (searchLookUpEditO.EditValue == null)
                            {
                                XtraMessageBox.Show("Vui lòng chọn Ô.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }
                }
            }
            frmNhapKhoAll frm = new frmNhapKhoAll();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                DataTable _dtVTK = new DataTable();
                string ngaynhapkho = frm.NgayNhapKho.ToString();
                List<DataRow> rowsToDelete = new List<DataRow>();
                foreach (DataRow row in _dtRowSave.Rows)
                {
                    row["NgayNhapKho_TC"] = ngaynhapkho;

                    if ((bool)row["CheckDT"] == false)
                    {
                        rowsToDelete.Add(row);
                    }
                }
                foreach (DataRow rowToDelete in rowsToDelete)
                {
                    _dtRowSave.Rows.Remove(rowToDelete);
                }
                if (_dtRowSave.Rows.Count <= 0)
                {
                    XtraMessageBox.Show("Chưa chọn dữ liệu nhập kho.! Vui lòng kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (chkNhap.Checked == true)
                {
                    DataTable _dtNK = CloneAndModify(_dtRowSave);
                    string urlVTK = string.Format("{0}?", URL + "NhapKho/GetVTK");
                    string json = Task.Run(async () => { return await _clientExtension.PostAsync(urlVTK, _dtNK); }).Result;
                    _dtVTK = JsonConvert.DeserializeObject<DataTable>(json);
                    bool isEmpty = _dtVTK.AsEnumerable()
                               .Any(row => row["CBM"] == null ||
                                            Convert.ToDecimal(row["CBM"]) == 0 ||
                                            row["CBM"] == "" ||
                                            row["CBM"] == DBNull.Value.ToString());

                    if (isEmpty)
                    {
                        XtraMessageBox.Show("Vui lòng khai báo CBM cho thùng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    decimal total = Math.Round(_dtVTK.AsEnumerable().Sum(row => Convert.ToDecimal(row["CBM"])), 4);
                    if (total > _cbm)
                    {
                        XtraMessageBox.Show("CBM các thùng vượt quá sức chứa trong Ô. Vui lòng kiểm tra lại.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    foreach (DataRow _dr in _dtVTK.Rows)
                    {
                        _dr["MaKhuVuc"] = searchLookUpEditDay.EditValue.ToString();
                        _dr["MaKe"] = searchLookUpEditKe.EditValue.ToString();
                        _dr["MaTang"] = searchLookUpEditTang.EditValue.ToString();
                        _dr["MaO"] = searchLookUpEditO.EditValue.ToString();
                        _dr["MaKho"] = searchLookUpEditKho.EditValue.ToString();
                        _dr["NVNhap"] = GlobleData.UserName;
                    }
                }
                _dtRowSave.Columns.Remove("CheckDT");
                string url = "";
                if (!_sms)
                {
                    url = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}&&mahang={4}&&po={5}", URL + "NhapKho/Post", true, lcsx, 2, _mahang, _poiddt);
                }
                else
                {
                    url = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}&&mahang={4}&&po={5}", URL + "NhapKho/PostSMS", true, lcsx, 2, _mahang, _poiddt);
                }
                string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtRowSave); }).Result;
                if (result.ToLower() != "true")
                    XtraMessageBox.Show(result);
                else
                {
                    if (chkNhap.Checked == true)
                    {
                        if(_dtVTK != null && _dtVTK.Rows.Count > 0)
                        {
                            string urlsaveVTK = string.Format("{0}?", URL + "NhapKho/PostVTK");
                            string resultsaveVTK = Task.Run(async () => { return await _clientExtension.PostAsync(urlsaveVTK, _dtVTK); }).Result;
                            if (resultsaveVTK.ToLower() != "true")
                                XtraMessageBox.Show(resultsaveVTK);
                        }    
                        //string urlsaveVTK = string.Format("{0}?", URL + "NhapKho/PostVTK");
                        //string resultsaveVTK = Task.Run(async () => { return await _clientExtension.PostAsync(urlsaveVTK, _dtVTK); }).Result;
                        //if (resultsaveVTK.ToLower() != "true")
                        //    XtraMessageBox.Show(resultsaveVTK);
                    }
                    clsWaitForm.ShowSuccessForm(this, 2000);
                    LoadNKPO();
                    _dtRowSave.Clear();
                    CreateDatatable();
                    LoadCBM();
                }
            }

        }
        public static object[][] DataTableToArray(DataTable dataTable)
        {
            int rowCount = dataTable.Rows.Count;
            int columnCount = dataTable.Columns.Count;

            object[][] array = new object[rowCount][];

            for (int i = 0; i < rowCount; i++)
            {
                DataRow row = dataTable.Rows[i];
                object[] rowArray = new object[columnCount];

                for (int j = 0; j < columnCount; j++)
                {
                    rowArray[j] = row[j];
                }

                array[i] = rowArray;
            }

            return array;
        }

        private void btHuyChon_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            List<DataRow> rowsToDelete = new List<DataRow>();
            foreach (DataRow row in _dtRowSave.Rows)
            {
                row["NgayNhapKho_TC"] = "";
                if ((bool)row["CheckDT"] == true)
                {
                    rowsToDelete.Add(row);
                }

            }
            foreach (DataRow rowToDelete in rowsToDelete)
            {
                _dtRowSave.Rows.Remove(rowToDelete);
            }
            if (_dtRowSave.Rows.Count <= 0)
            {
                XtraMessageBox.Show("Chưa chọn dữ liệu hủy nhập kho.! Vui lòng kiểm tra lại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DataTable dtKT = new DataTable();
            dtKT.Columns.Add("MaDH", typeof(string));
            dtKT.Columns.Add("MaPKL", typeof(string));
            dtKT.Columns.Add("MaDVSX", typeof(string));
            dtKT.Columns.Add("MaLenh", typeof(string));
            dtKT.Columns.Add("POID", typeof(string));
            dtKT.Columns.Add("MaKho", typeof(string));
            dtKT.Columns.Add("MaKhoDen", typeof(string));
            dtKT.Columns.Add("TuThung", typeof(int));
            dtKT.Columns.Add("DenThung", typeof(int));
            dtKT.Columns.Add("MinSttThung", typeof(int));
            dtKT.Columns.Add("MaxSttThung", typeof(int));
            dtKT.Columns.Add("SttThungDecat", typeof(int));
            foreach(DataRow _drow in _dtRowSave.Rows)
            {
                DataRow _dr = dtKT.NewRow();
                _dr["MaDH"] = _drow["MaDH"];
                _dr["MaPKL"] = _drow["MaDH"];
                _dr["MaDVSX"] = _drow["MaDVSX"];
                _dr["MaLenh"] = _drow["MaLenh"];
                _dr["POID"] = _drow["POID"];
                _dr["TuThung"] = _drow["TuThung"];
                _dr["DenThung"] = _drow["DenThung"];
                _dr["MinSttThung"] = _drow["MinSttThung"];
                _dr["MaxSttThung"] = _drow["MaxSttThung"];
                _dr["SttThungDecat"] = _drow["SttThungDecat"];
                dtKT.Rows.Add(_dr);
            }    
            string urlKT = string.Format("{0}?", URL + "NhapKho/GetKiemTra");
            string jsonKT = Task.Run(async () => { return await _clientExtension.PostAsync(urlKT, dtKT); }).Result;
            DataTable _dtKT = JsonConvert.DeserializeObject<DataTable>(jsonKT);
            if (_dtKT != null && _dtKT.Rows.Count > 0)
            {
                DialogResult messResult = MessageBox.Show("Một số thùng đã được xác nhận giao. Chỉ hủy thùng chưa được xác nhận giao.!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.No)
                {
                    return;
                }
            }
            _dtRowSave.Columns.Remove("CheckDT");

            string url = "";
            if (!_sms)
            {
                url = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}&&mahang={4}&&po={5}", URL + "NhapKho/Post", false, lcsx, 1, _mahang, _poiddt);
            }
            else
            {
                url = string.Format("{0}?isdongthung={1}&&lcsx={2}&&statuschuyen={3}&&mahang={4}&&po={5}", URL + "NhapKho/PostSMS", false, lcsx, 1, _mahang, _poiddt);

            }
            string result = Task.Run(async () => { return await _clientExtension.PostAsync(url, _dtRowSave); }).Result;
            if (result.ToLower() != "true")
                XtraMessageBox.Show(result);
            else
            {
                clsWaitForm.ShowSuccessForm(this, 2000);
                LoadNKPO();
                _dtRowSave.Clear();
                CreateDatatable();
            }
        }
        #region Manh
        private void bandedGridView1_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
        {
            var dt = gridControl2.DataSource as DataTable;
            KHDongThungLibP.SumGroup(dt, e);
        }

        private void bandedGridView1_CellMerge(object sender, CellMergeEventArgs e)
        {
            KHDongThungLibP.Merge(sender, e);
        }

        private void btnEX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Title = "File To Save";
            Sfd.Filter = "Excel | *.xlsx";
            if (tblCT is null || tblCT.Rows.Count == 0) return;
            _mahang = tbl.AsEnumerable().Where(x => x["MaGop"] == _madh.ToString()).FirstOrDefault()["MaHang"].ToString();
            DataTable dtTbl = gridControl2.DataSource as DataTable;
            Sfd.FileName = string.Format("NhapKho_{0}_{1}" + DateTime.Now.ToString("ddMMyyyy"), _mahang, _poiddt);
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
                    range = worksheet.Cells["D2:N3"]; range.Merge = true; range.Value = "NHẬP KHO"; range.Style.Font.Bold = true;
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
                    worksheet.Cells[9, columSttThungDong].Value = "SL Nhập kho";
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
        #endregion

        private void barCheckItemSMS_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _sms = (bool)barCheckItemSMS.Checked;
            LoadData();
        }

        private void btnBarCode_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            if (searchLookUpEditKho.EditValue == null)
            {
                MessageBox.Show("Vui lòng chọn kho.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (chkNhap.Checked == true)
            {
                if (searchLookUpEditDay.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn Dãy.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    if (searchLookUpEditKe.EditValue == null)
                    {
                        XtraMessageBox.Show("Vui lòng chọn Kệ.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        if (searchLookUpEditTang.EditValue == null)
                        {
                            XtraMessageBox.Show("Vui lòng chọn Tầng.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            if (searchLookUpEditO.EditValue == null)
                            {
                                XtraMessageBox.Show("Vui lòng chọn Ô.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }
                }
            }
            DataTable _tblCT = new DataTable();

            DataTable dtNhapKho = this.gridControl2.DataSource as DataTable;
            frmNhapKhoBarCode frm = new frmNhapKhoBarCode(dtNhapKho, _sms, (bool)barCheckItemlcxh.Checked, searchLookUpEditKho.EditValue.ToString(), true, (bool)chkNhap.Checked,
                searchLookUpEditDay.EditValue == null ? "" : searchLookUpEditDay.EditValue.ToString(), searchLookUpEditKe.EditValue == null ? "" : searchLookUpEditKe.EditValue.ToString(),
                searchLookUpEditTang.EditValue == null ? "" : searchLookUpEditTang.EditValue.ToString(), searchLookUpEditO.EditValue == null ? "" : searchLookUpEditO.EditValue.ToString(), _cbm, _madvsxdt);
            frm.ShowDialog();
            LoadNKPO();
        }

        private void btnHuyBarCode_Click(object sender, EventArgs e)
        {
            rowHandel_2 = bandedGridView1.FocusedRowHandle;
            rowHandel = gridView2.FocusedRowHandle;
            rowHandel_1 = gridView1.FocusedRowHandle;
            DataTable _tblCT = new DataTable();
            DataTable dtNhapKho = this.gridControl2.DataSource as DataTable;
            frmNhapKhoBarCode frm = new frmNhapKhoBarCode(dtNhapKho, _sms, (bool)barCheckItemlcxh.Checked, "", false, (bool)chkNhap.Checked,
                 searchLookUpEditDay.EditValue == null ? "" : searchLookUpEditDay.EditValue.ToString(), searchLookUpEditKe.EditValue == null ? "" : searchLookUpEditKe.EditValue.ToString(),
                searchLookUpEditTang.EditValue == null ? "" : searchLookUpEditTang.EditValue.ToString(), searchLookUpEditO.EditValue == null ? "" : searchLookUpEditO.EditValue.ToString(), _cbm, _madvsxdt);
            frm.ShowDialog();
            LoadNKPO();
            LoadCBM();
        }
        private void chkNhap_CheckedChanged(object sender, EventArgs e)
        {
            LoadCheckNhap();
            if ((bool)chkNhap.Checked == false)
            {
                searchLookUpEditO.EditValue = null;
                searchLookUpEditTang.EditValue = null;
                searchLookUpEditKe.EditValue = null;
                searchLookUpEditDay.EditValue = null;
                txtCBM.Text = "";
            }

        }
        private void searchLookUpEditKho_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEditKho.EditValue == null)
                chkNhap.Enabled = false;
            else
                chkNhap.Enabled = true;

            string url = string.Format("{0}?madvsx={1}&&makho={2}&&makh={3}", URL + "NhapKho/GetDay", _madvsxHT, searchLookUpEditKho.EditValue == null ? "" : searchLookUpEditKho.EditValue.ToString(), _maKH);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _dtDay = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditDay.Properties.DataSource = _dtDay;
            searchLookUpEditDay.Properties.DisplayMember = "TenDay";
            searchLookUpEditDay.Properties.ValueMember = "MaDay";
            searchLookUpEditDay.Properties.ShowClearButton = false;

            GridView dvView = searchLookUpEditDay.Properties.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaDay", Caption = "Mã Dãy", Name = "colMaDay", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenDay", Caption = "Dãy", Name = "colTenDay", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "CBM", Caption = "CBM", Name = "colCBM", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "TenKH", Caption = "Khách hàng", Name = "colTenKH", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "CheckStatus", Caption = "CheckStatus", Name = "colCheckStatus", Visible = false });
            }
            dvView.RowStyle += DvView_RowStyle;
        }

        private void DvView_RowStyle(object sender, RowStyleEventArgs e)
        {
            var view = sender as GridView;
            var checkStatus = view.GetRowCellValue(e.RowHandle, view.Columns["CheckStatus"]);

            if (checkStatus != null && checkStatus.ToString() == "1")
            {
                e.Appearance.BackColor = Color.Yellow;
            }
        }

        private void searchLookUpEditDay_EditValueChanged(object sender, EventArgs e)
        {
            string url = string.Format("{0}?madvsx={1}&&makho={2}&&maday={3}", URL + "NhapKho/GetKe", _madvsxHT,
                searchLookUpEditKho.EditValue == null ? "" : searchLookUpEditKho.EditValue.ToString(),
                searchLookUpEditDay.EditValue == null ? "" : searchLookUpEditDay.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _dtKe = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditKe.Properties.DataSource = _dtKe;
            searchLookUpEditKe.Properties.DisplayMember = "TenKe";
            searchLookUpEditKe.Properties.ValueMember = "MaKe";
            searchLookUpEditKe.Properties.ShowClearButton = false;

            GridView dvView = searchLookUpEditKe.Properties.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaKe", Caption = "Mã kệ", Name = "colMaKe", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenKe", Caption = "Kệ", Name = "colTenKe", Visible = true });

            }
            searchLookUpEditKe.EditValue = null;
            searchLookUpEditTang.EditValue = null;
            searchLookUpEditO.EditValue = null;
            txtCBM.Text = "";
        }
        private void searchLookUpEditKe_EditValueChanged(object sender, EventArgs e)
        {
            string url = string.Format("{0}?madvsx={1}&&makho={2}&&maday={3}&&maKe={4}", URL + "NhapKho/GetTang", _madvsxHT, searchLookUpEditKho.EditValue == null ? "" : searchLookUpEditKho.EditValue.ToString(),
                searchLookUpEditDay.EditValue == null ? "" : searchLookUpEditDay.EditValue.ToString(),
                searchLookUpEditKe.EditValue == null ? "" : searchLookUpEditKe.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _dtTang = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditTang.Properties.DataSource = _dtTang;
            searchLookUpEditTang.Properties.DisplayMember = "TenTang";
            searchLookUpEditTang.Properties.ValueMember = "MaTang";
            searchLookUpEditTang.Properties.ShowClearButton = false;

            GridView dvView = searchLookUpEditTang.Properties.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaTang", Caption = "Mã tầng", Name = "colMaTang", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenTang", Caption = "Tầng", Name = "colTenTang", Visible = true });

            }
        }

        private void searchLookUpEditTang_EditValueChanged(object sender, EventArgs e)
        {
            string url = string.Format("{0}?madvsx={1}&&makho={2}&&maday={3}&&make={4}&&matang={5}", URL + "NhapKho/GetO", _madvsxHT, searchLookUpEditKho.EditValue == null ? "" : searchLookUpEditKho.EditValue.ToString(),
                searchLookUpEditDay.EditValue == null ? "" : searchLookUpEditDay.EditValue.ToString(),
                searchLookUpEditKe.EditValue == null ? "" : searchLookUpEditKe.EditValue.ToString(),
                searchLookUpEditTang.EditValue == null ? "" : searchLookUpEditTang.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _dtO = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditO.Properties.DataSource = _dtO;
            searchLookUpEditO.Properties.DisplayMember = "TenO";
            searchLookUpEditO.Properties.ValueMember = "MaO";
            searchLookUpEditO.Properties.ShowClearButton = false;

            GridView dvView = searchLookUpEditO.Properties.View;
            if (dvView.Columns.Count == 0)
            {
                dvView.Appearance.HeaderPanel.Font = new Font("Tahoma", 9.75F);
                dvView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dvView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                dvView.Appearance.HeaderPanel.Options.UseTextOptions = true;
                dvView.Columns.Add(new GridColumn { FieldName = "MaO", Caption = "Mã Ô", Name = "colMaO", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "TenO", Caption = "Ô", Name = "colTenO", Visible = true });
                dvView.Columns.Add(new GridColumn { FieldName = "CBM", Caption = "CBM", Name = "colCBM", Visible = false });
                dvView.Columns.Add(new GridColumn { FieldName = "CBMTotal", Caption = "CBMToal", Name = "colCBMTotal", Visible = false });
            }
        }
        public void LoadCBM()
        {
            DataRow focusedRow = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            string url = string.Format("{0}?madvsx={1}&&makho={2}&&maday={3}&&make={4}&&matang={5}&&mao={6}&&madh={7}&&mapkl={8}&&poid={9}", URL + "NhapKho/GetCBM", _madvsxHT, searchLookUpEditKho.EditValue == null ? "" : searchLookUpEditKho.EditValue.ToString(),
               searchLookUpEditDay.EditValue == null ? "" : searchLookUpEditDay.EditValue.ToString(),
               searchLookUpEditKe.EditValue == null ? "" : searchLookUpEditKe.EditValue.ToString(),
               searchLookUpEditTang.EditValue == null ? "" : searchLookUpEditTang.EditValue.ToString(),
               searchLookUpEditO.EditValue == null ? "" : searchLookUpEditO.EditValue.ToString(), _madhdt, _mapkldt, _podt);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable _dtCBM = JsonConvert.DeserializeObject<DataTable>(json);
            //var selectedValue = searchLookUpEditO.EditValue;
            //var view = searchLookUpEditO.Properties.View;
            //var selectedRow = view.GetDataRow(view.FocusedRowHandle);

            if (_dtCBM.Rows.Count > 0)
            {
                txtCBM.Text = Convert.ToDecimal(_dtCBM.Rows[0][1]).ToString() + "/" + Convert.ToDecimal(_dtCBM.Rows[0][0]).ToString();
                _cbm = Convert.ToInt32(_dtCBM.Rows[0][1]);

            }
        }
        private void searchLookUpEditO_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEditO.EditValue != null)
            {
                LoadCBM();
            }
        }
    }
}
