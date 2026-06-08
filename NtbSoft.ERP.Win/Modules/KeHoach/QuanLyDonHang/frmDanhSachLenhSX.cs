using DevExpress.Data;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.CanDoiDonHang
{
    public partial class frmDanhSachLenhSX : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        DataTable tblDsChiTietLenhSX;
        List<string> lstSize;
        int pageIndex = 0;
        int pageSize = 0;
        string maDVSX;
        string maHang;
        bool isRowChanged = false;
        bool indicatorIcon = true;
        string json = string.Empty;
        KeyDownControlHandler keyDownControlHandler;
        public frmDanhSachLenhSX()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_KeyDown);
            // Lấy chiều dài màn hình chính
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            this.splitContainerControl1.SplitterPosition = screenWidth / 4;

            lstSize = new List<string>();
            _clientExtension = new HttpClientExtension();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            init();
            // SearchLookup
            CreateSearchLookup();
            keyDownControlHandler = new KeyDownControlHandler(InitActionKeyDown());
        }

        private void frm_KeyDown(object sender, KeyEventArgs e)
        {
            keyDownControlHandler.PressKeyDown(e);
        }

        private List<ActionControl> InitActionKeyDown()
        {
            List<ActionControl> _lstActionControl = new List<ActionControl>();

            // Thêm các ActionControl cần thiết vào danh sách
            AddActionControl(_lstActionControl, BtNapLai, true, ActionType.Refresh);

            return _lstActionControl;
        }
        private void AddActionControl(List<ActionControl> list, Action action, bool permission, ActionType actionType)
        {
            ActionControl actionControl = new ActionControl(action, permission, actionType, true);
            list.Add(actionControl);
        }
        private void init()
        {
            isRowChanged = false;
            // phân trang
            pageIndex = 1;
            pageSize = 100;
            barEditItemSpinPageSize.EditValue = pageSize;
            maDVSX = string.Empty;
            maHang = string.Empty;

            //MapDataToView(maDVSX, maHang);

            DataTable tblDSThongTin = LoadDsThongTinLenhSX();
            gridControlThongTin.DataSource = tblDSThongTin;
            if (tblDSThongTin != null && tblDSThongTin.Rows.Count > 0)
            {
                if (!isRowChanged)
                {
                    LoadDsChiTietLenhSX();
                }
            }
            else
            {
                gridControlChiTiet.DataSource = null;
            }
        }

        // Map dữ liệu vào SearchLookup cho DonViSanXuat và HangHoa
        private void CreateSearchLookup()
        {
            // DonViSanXuat
            string urlDVSX = string.Format("{0}?", URL + "DonViSanXuat/GetDonViSanXuat");
            string jsonDVSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDVSX); }).Result;
            DataTable tblDVSX = JsonConvert.DeserializeObject<DataTable>(jsonDVSX);
            repositorySearchLookUpEditMaDVSX.DataSource = tblDVSX;
            this.repositorySearchLookUpEditMaDVSX.DisplayMember = "TenDVSX";
            this.repositorySearchLookUpEditMaDVSX.ValueMember = "MaDVSX";

            // HangHoa
            string urlHangHoa = string.Format("{0}?", URL + "CanDoiDonHangTong/GetDSHangHoa");
            string jsonHangHoa = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlHangHoa); }).Result;
            DataTable tblHangHoa = JsonConvert.DeserializeObject<DataTable>(jsonHangHoa);
            repositorySearchLookUpEditHangHoa.DataSource = tblHangHoa;
            //repositorySearchLookUpEditHangHoa.View.PopulateColumns();
        }

        private void repositorySearchLookUpEditMaDVSX_EditValuedChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Edit_valued_changed");

            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;

            maDVSX = changingEventArgs.NewValue.ToString();
            //MapDataToView(maDVSX, maHang);
            FilterData(maDVSX, maHang);
        }

        private void repositorySearchLookUpEditHangHoa_EditValuedChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Edit_valued_changed");

            ChangingEventArgs changingEventArgs = e as ChangingEventArgs;
            maHang = changingEventArgs.NewValue.ToString();
            //MapDataToView(maDVSX, maHang);
            FilterData(maDVSX, maHang);
        }

        private void FilterData(string _maDVSX, string _maHang)
        {
            try
            {
                DataTable _tblFilter = JsonConvert.DeserializeObject<DataTable>(json);
                isRowChanged = false;
                if (!string.IsNullOrEmpty(json))
                {
                    
                    if (!string.IsNullOrEmpty(_maDVSX))
                    {
                        DataRow row = _tblFilter.AsEnumerable().FirstOrDefault(x => x["MaDVSX"].Equals(_maDVSX));
                        if (row != null)
                        {
                            _tblFilter = _tblFilter.AsEnumerable().Where(x => x["MaDVSX"].Equals(_maDVSX)).CopyToDataTable();
                        }
                        else
                        {
                            _tblFilter = null;
                            gridControlChiTiet.DataSource = null;
                        }
                    }
                    if (!string.IsNullOrEmpty(_maHang))
                    {
                        DataRow row = _tblFilter.AsEnumerable().FirstOrDefault(x => x["MaHang"].Equals(_maHang));
                        if (row != null)
                        {
                            _tblFilter = _tblFilter.AsEnumerable().Where(x => x["MaHang"].Equals(_maHang)).CopyToDataTable();
                        }
                        else
                        {
                            _tblFilter = null;
                            gridControlChiTiet.DataSource = null;
                        }
                    }
                    gridControlThongTin.DataSource = _tblFilter;
                }
                if (!isRowChanged&& _tblFilter!=null)
                {
                    LoadDsChiTietLenhSX();
                }
            }
            catch (Exception ex)
            {
                gridControlThongTin.DataSource = null;
            }
        }

        private DataTable LoadDsThongTinLenhSX()
        {
            // Tăng pageSize lên 1( Để kiểm tra)
            // Ví dụ pageIndex=1, pageSize = 100, tăng paegSize lên 1 => pageSize = 101
            // Nếu DataTable trả về có đúng 101 row => thì pageIndex = 2 sẽ có dữ liệu
            //pageSize += 1;
            //string urlDSThongTinLenhSX = string.Format("{0}", URL + "CanDoiDonHangTong/GetDSThongTinLenhSX");

            string urlDSThongTinLenhSX = string.Format("{0}?pageIndex={1}&&pageSize={2}", URL + "CanDoiDonHangTong/GetDSThongTinLenhSX", pageIndex, pageSize);
            string jsonDSThongTinLenhSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSThongTinLenhSX); }).Result;
            json = jsonDSThongTinLenhSX;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonDSThongTinLenhSX);
            return tbl;
        }

        private void LoadDsChiTietLenhSX()
        {
            DataRow _focusedRow = (gridViewThongTin.GetFocusedRow() as DataRowView).Row;
            if (_focusedRow != null)
            {
                string _id = _focusedRow["ID"].ToString();
                string urlDSChiTietLenhSX = string.Format("{0}?id={1}&&pageIndex={2}&&pageSize={3}", URL + "CanDoiDonHangTong/GetDSChiTietLenhSX", _id, pageIndex, pageSize);
                string jsonDSChiTietLenhSX = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlDSChiTietLenhSX); }).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(jsonDSChiTietLenhSX);
                if (tbl != null && tbl.Rows.Count > 0)
                {

                    bandedGridColDauSize.Width = 200;
                    bandedGridViewChiTiet = CreateBandSize(tbl);

                    AddColumnSumRow(tbl);
                }
                gridControlChiTiet.DataSource = tbl;
            }
        }

        private DataTable AddColumnSumRow(DataTable tbl)
        {
            tbl.Columns.Add("TongSL", typeof(int));
            foreach (DataRow dataRow in tbl.Rows)
            {
                int tong = 0;
                foreach (DataColumn dataColumn in tbl.Columns)
                {
                    if (dataColumn.ColumnName.Contains("Size@"))
                    {
                        int sl = 0;
                        bool parse = int.TryParse(dataRow[dataColumn.ColumnName].ToString(), out sl);
                        if (parse)
                        {
                            tong += sl;
                        }
                    }
                }
                dataRow["TongSL"] = tong;
            }
            return tbl;
        }

        private BandedGridView CreateBandSize(DataTable tblDS)
        {
            BandedGridView bandedGridview = new BandedGridView();
            bandedGridview = bandedGridViewChiTiet;
            //bandedGridview.CustomUnboundColumnData += BandedGridview_CustomUnboundColumnData; //new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.bandedGridview_CustomUnboundColumnData);
            try
            {
                if (lstSize != null && lstSize.Count > 0)
                {
                    lstSize.Clear();
                }
                gridBandSize.Children.Clear();
                // bandedGridview.Columns.Clear();
                foreach (DataColumn dc in tblDS.Columns)
                {
                    if (dc.ColumnName.Contains("Size@"))
                    {
                        string colName = dc.ColumnName;
                        lstSize.Add(colName.Replace("Size@", ""));

                        BandedGridColumn col = new BandedGridColumn();
                        col.AppearanceHeader.Options.UseTextOptions = true;
                        col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        col.Caption = colName.Replace("Size@", "");
                        col.FieldName = dc.ColumnName;
                        col.Name = "col" + colName;
                        col.OptionsColumn.AllowEdit = false;
                        col.Visible = true;
                        col.Width = 50;
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
                        gb.Caption = colName.Replace("Size@", "");
                        gb.Columns.Add(col);
                        gb.Name = "gb" + "Size_" + col;
                        gb.VisibleIndex = 0;
                        gb.Width = 60;
                        gridBandSize.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return bandedGridview;
        }

        private void barButtonPrevItemClick(object sender, ItemClickEventArgs e)
        {
            if (pageIndex > 1)
            {
                pageIndex -= 1;
                this.barStaticItemPageIndex.Caption = pageIndex.ToString();
            }
            //MapDataToView(maDVSX, maHang);
            LoadDsThongTinLenhSX();
        }

        private void repositoryItemSpinEdit1_EditValueChanged(object sender, EventArgs e)
        {
            SpinEdit spinEdit = sender as SpinEdit;
            if (spinEdit != null && spinEdit.EditValue != null && Convert.ToInt32(spinEdit.EditValue) >= 0)
            {
                pageSize = Convert.ToInt32(spinEdit.EditValue);
                //MapDataToView(maDVSX, maHang);
            }
        }

        private void barButtonNextItemClick(object sender, ItemClickEventArgs e)
        {
            pageIndex += 1;
            this.barStaticItemPageIndex.Caption = pageIndex.ToString();
            //MapDataToView(maDVSX, maHang);
        }

        private void BandedView_CustomDrawFooter(object sender, RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
        }

        private void bandedGridviewSoLuong_CustomDrawFooterCell(object sender, DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventArgs e)
        {
            if (e.Column == null)
                return;
            //e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            e.Appearance.ForeColor = Color.Red;
        }

        //private void BandedView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        //{
        //    GridView view = sender as GridView;
        //    if (e.Column.FieldName == "STT" && e.IsGetData)
        //        e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
        //}
        private void gridViewThongTin_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            isRowChanged = true;
            LoadDsChiTietLenhSX();
        }
        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            BtNapLai();
        }

        private void Gridview_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridView view = (GridView)sender;
            if (e.Column.FieldName == "STT")
            {
                if (e.RowHandle < 0)
                {
                    e.DisplayText = "";
                }
                else
                {
                    e.DisplayText = Convert.ToString(e.RowHandle + 1);
                }

            }


            // Kiểm tra xem dòng này có được chọn không
            if (view.IsRowSelected(e.RowHandle))
            {
                // Thiết lập màu sắc cho dòng được chọn
                e.Appearance.BackColor = Color.FromArgb(255, 251, 209);
            }
        }

        private void BtNapLai()
        {
            barEditItem1.EditValue = null;
            barEditItem2.EditValue = null;
            maHang = string.Empty;
            maDVSX = string.Empty;
            //MapDataToView(maDVSX, maHang);
            init();
        }

        private void gridViewThongTin_ProcessGridKey(object sender, KeyEventArgs e)
        {
            //switch (e.KeyCode)
            //{
            //    case Keys.F5:
            //        barEditItem1.EditValue = null;
            //        BtNapLai();
            //        break;
            //}
        }

        private void gridViewThongTin_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
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

        private void Gridview_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Bounds == null) return;
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

        private void bandedGridViewChiTiet_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            try
            {
                BandedGridView view = bandedGridViewChiTiet;
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {
                    string sText = (e.RowHandle + 1).ToString();
                    Graphics gr = e.Info.Graphics;
                    gr.PageUnit = GraphicsUnit.Pixel;
                    BandedGridView gridView = ((BandedGridView)sender);
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
                    BandedGridView gridView = bandedGridViewChiTiet;
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

        private void bandedGridViewChiTiet_RowCountChanged(object sender, EventArgs e)
        {
            BandedGridView gridview = bandedGridViewChiTiet; ;
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void gridViewThongTin_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gridViewThongTin_RowCountChanged(object sender, EventArgs e)
        {
            GridView gridview = ((GridView)sender);
            if (!gridview.GridControl.IsHandleCreated) return;
            Graphics gr = Graphics.FromHwnd(gridview.GridControl.Handle);
            SizeF size = gr.MeasureString(gridview.RowCount.ToString(), gridview.PaintAppearance.Row.GetFont());
            gridview.IndicatorWidth = Convert.ToInt32(size.Width + 0.999f) + GridPainter.Indicator.ImageSize.Width + 15;
        }

        private void bandedGridviewChiTiet_CustomDrawBandheader(object sender, BandHeaderCustomDrawEventArgs e)
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
    }
}
