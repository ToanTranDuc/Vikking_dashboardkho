using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
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
    public partial class frmCoppyMaHang : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private List<string> _lstSize;
        private HttpClientExtension _clientExtension;
        private HashSet<DataRow> selectedRowsSize = new HashSet<DataRow>();
        private List<string> _preselectedSizeIds = new List<string>();
        DataTable dtbdh;
        DataTable dtbctdh;
        public event Action<DataTable, DataTable> OnCopyConfirmed;
        int sum = 0, sort = 0;
        bool indicatorIcon = true;
        public frmCoppyMaHang()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            dtbdh = new DataTable();
            dtbctdh = new DataTable();
        }


        protected override void OnLoad(EventArgs e) 
        {
            string urlKH = string.Format("{0}?", URL + "DonHangTong/GetKhachHang");
            string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
            DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
            searchLookUpEditKH.Properties.DataSource = tblKH;
            searchLookUpEditKH.Properties.ValueMember = "MaKH";
            searchLookUpEditKH.Properties.DisplayMember = "TenKH";
        }

        private void LoadDSDonHangTong()
        {
            string url = string.Format("{0}?makh={1}", URL + "DonHangTong/Getdhcoppy", searchLookUpEditKH.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtbdh = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit1.Properties.DataSource = dtbdh;
            searchLookUpEdit1.Properties.DisplayMember = "MaDH";
            searchLookUpEdit1.Properties.ValueMember = "MaDH";
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn mã đơn hàng cần sao chép!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedMaDH = searchLookUpEdit1.EditValue.ToString();

                // Lọc ra dòng tương ứng trong dtbdh
                DataRow[] selectedRows = dtbdh.Select($"MaDH = '{selectedMaDH}'");

                DataTable selectedHeader = dtbdh.Clone(); // giữ cấu trúc
                foreach (var row in selectedRows)
                    selectedHeader.ImportRow(row);

                // Gửi selectedHeader và dtbctdh về form cha
                OnCopyConfirmed?.Invoke(selectedHeader, dtbctdh);

                this.Close();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void searchLookUpEdit2_Properties_EditValueChanged(object sender, EventArgs e)
        {
            LoadDSDonHangTong();
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            string url = string.Format("{0}?madh={1}", URL + "DonHangTong/Getctdhcoppy", searchLookUpEdit1.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtbctdh = JsonConvert.DeserializeObject<DataTable>(json);

            gridBand10.Children.Clear();
            createTable2(dtbctdh);
            gridControl1.MainView = GetBandGridViewAmount(dtbctdh);
            BandedGridView mainView = (BandedGridView)gridControl1.MainView;
            mainView.CellValueChanging += mainView_CellValueChanging;
            mainView.CustomSummaryCalculate += mainView_CustomSummaryCalculate;
            gridControl1.DataSource = dtbctdh;
        }


        private void mainView_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if (e.RowHandle >= 0 && (e.Column.FieldName == "MaQG" || e.Column.FieldName == "NgayGH" || e.Column.FieldName == "NgayDKVC" || e.Column.FieldName == "NgayThucTeVC"
                || e.Column.FieldName == "NgayXuatHang"))
            {
                GridView gridView = sender as GridView;

                DataRow rowFocused = (gridView.GetFocusedRow() as DataRowView).Row;
                DataTable dataTable = (gridView.DataSource as DataView).Table as DataTable;
                if (rowFocused != null && rowFocused["PO"] != null && dataTable != null && dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if (dataTable.Rows[i]["PO"] != null && (rowFocused["PO"].Equals(dataTable.Rows[i]["PO"])))
                        {
                            gridView.SetRowCellValue(i, e.Column.FieldName, e.Value);
                        }
                    }
                }
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
                    if (col.FieldName != "ID" && col.FieldName != "POID" && col.FieldName != "PO" && col.FieldName != "MaMau" && col.FieldName != "DauSize" && col.FieldName != "MaQG" && col.FieldName != "NgayGH" && col.FieldName != "GhiChu" && col.UnboundType == UnboundColumnType.Bound)
                    {
                        sum += Convert.ToInt32(col.Summary[0].SummaryValue);
                    }
                }
                e.TotalValue = sum;
            }
        }
        private void createTable2(DataTable tab)
        {
            //gridViewThongTinDonHang.OptionsView.AllowCellMerge = false;

            gridBand10.Children.Clear();
            GridBand parentBand = bandedGridView1.Bands["gridBandSize"];
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
                if (demColIndex > 10 && demColIndex < tab.Columns.Count)
                {
                    string[] arrName = column.ColumnName.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = arrName[2];
                    String colName1 = arrName[0];
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
                    gridBand10.Children.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] { gb });
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
            bandedView.KeyPress += gridviewThongTinDonHang_KeyPress;
            bandedView.ValidatingEditor += BandedView_ValidatingEditor;
            bandedView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false;
            bandedView.OptionsNavigation.EnterMoveNextColumn = true;
            return bandedView;
        }

        private void BandedView_CustomDrawFooter(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, Color.FromArgb(255, 239, 204), Color.FromArgb(255, 239, 204), 90);
            e.Graphics.FillRectangle(brush, e.Bounds);
            ControlPaint.DrawBorder3D(e.Graphics, e.Bounds, Border3DStyle.RaisedInner);
            e.Handled = true;
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
            if (e.Column.FieldName == "DauSize")
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

        private void gridviewThongTinDonHang_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ngăn không cho ký tự khi đang giữ nút control
            // Khi giữ nút control thực hiện các chức năng trên gridview
            if (ModifierKeys == Keys.Control)
            {
                e.Handled = true;
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
                    //if (Convert.ToInt32(e.Value) < 0)
                    //{
                    //    e.Valid = false;
                    //    e.ErrorText = "Số lượng phải lớn hơn 0";
                    //}
                }
            }

        }
    }
}