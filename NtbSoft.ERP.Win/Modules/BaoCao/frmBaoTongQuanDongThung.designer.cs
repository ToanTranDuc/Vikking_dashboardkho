
using DevExpress.Utils;

namespace NtbSoft.ERP.Win.Modules.BaoCao
{
    partial class frmBaoTongQuanDongThung
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBaoTongQuanDongThung));
            this.cardView1 = new DevExpress.XtraGrid.Views.Card.CardView();
            this.gridColumnSTTThung = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnSoLuongThung = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnTuThung = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnDenThung = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnSoLuongSP = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnDauSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnTenMau = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnMaPKL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barEditItem1 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemHHFilter = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.gridViewMaHangSearchLookup = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMaHangSearchLookup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenHangSearchLookup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.barEditItem2 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemDateFilter = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.barEditItem3 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemDVSXFilter = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.gridViewMaDVSXSearchLookup = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMaDVSXSearchLookup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenDVSXSearchLookup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Naplai = new DevExpress.XtraBars.BarButtonItem();
            this.XuatExcel = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.gridColPO = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.cardView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemHHFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMaHangSearchLookup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateFilter.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDVSXFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMaDVSXSearchLookup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            this.SuspendLayout();
            // 
            // cardView1
            // 
            this.cardView1.Appearance.Card.Options.UseTextOptions = true;
            this.cardView1.Appearance.Card.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.cardView1.CardWidth = 130;
            this.cardView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnSTTThung,
            this.gridColumnSoLuongThung,
            this.gridColumnTuThung,
            this.gridColumnDenThung,
            this.gridColPO,
            this.gridColumnSize,
            this.gridColumnSoLuongSP,
            this.gridColumnDauSize,
            this.gridColumnTenMau,
            this.gridColumnMaPKL});
            this.cardView1.GridControl = this.gridControl1;
            this.cardView1.Name = "cardView1";
            this.cardView1.OptionsBehavior.Editable = false;
            this.cardView1.OptionsBehavior.ReadOnly = true;
            this.cardView1.CustomDrawCardCaption += new DevExpress.XtraGrid.Views.Card.CardCaptionCustomDrawEventHandler(this.cardView1_CustomDrawCardCaption);
            // 
            // gridColumnSTTThung
            // 
            this.gridColumnSTTThung.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnSTTThung.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridColumnSTTThung.Caption = "STT thùng";
            this.gridColumnSTTThung.FieldName = "SttThung";
            this.gridColumnSTTThung.Name = "gridColumnSTTThung";
            this.gridColumnSTTThung.OptionsColumn.ReadOnly = true;
            // 
            // gridColumnSoLuongThung
            // 
            this.gridColumnSoLuongThung.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnSoLuongThung.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridColumnSoLuongThung.Caption = "SL thùng";
            this.gridColumnSoLuongThung.FieldName = "SoLuongThung";
            this.gridColumnSoLuongThung.Name = "gridColumnSoLuongThung";
            this.gridColumnSoLuongThung.OptionsColumn.ReadOnly = true;
            // 
            // gridColumnTuThung
            // 
            this.gridColumnTuThung.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnTuThung.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridColumnTuThung.Caption = "Từ thùng";
            this.gridColumnTuThung.FieldName = "TuThung";
            this.gridColumnTuThung.Name = "gridColumnTuThung";
            this.gridColumnTuThung.OptionsColumn.ReadOnly = true;
            // 
            // gridColumnDenThung
            // 
            this.gridColumnDenThung.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnDenThung.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridColumnDenThung.Caption = "Đến thùng";
            this.gridColumnDenThung.FieldName = "DenThung";
            this.gridColumnDenThung.Name = "gridColumnDenThung";
            this.gridColumnDenThung.OptionsColumn.ReadOnly = true;
            // 
            // gridColumnSize
            // 
            this.gridColumnSize.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumnSize.AppearanceCell.Options.UseFont = true;
            this.gridColumnSize.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnSize.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridColumnSize.Caption = "Size";
            this.gridColumnSize.FieldName = "Size";
            this.gridColumnSize.Name = "gridColumnSize";
            this.gridColumnSize.OptionsColumn.ReadOnly = true;
            this.gridColumnSize.Visible = true;
            this.gridColumnSize.VisibleIndex = 1;
            // 
            // gridColumnSoLuongSP
            // 
            this.gridColumnSoLuongSP.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumnSoLuongSP.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridColumnSoLuongSP.Caption = "SL Sản phẩm";
            this.gridColumnSoLuongSP.FieldName = "SoLuongSP";
            this.gridColumnSoLuongSP.Name = "gridColumnSoLuongSP";
            this.gridColumnSoLuongSP.OptionsColumn.ReadOnly = true;
            this.gridColumnSoLuongSP.Visible = true;
            this.gridColumnSoLuongSP.VisibleIndex = 4;
            // 
            // gridColumnDauSize
            // 
            this.gridColumnDauSize.Caption = "Đầu size";
            this.gridColumnDauSize.FieldName = "DauSize";
            this.gridColumnDauSize.MinWidth = 21;
            this.gridColumnDauSize.Name = "gridColumnDauSize";
            this.gridColumnDauSize.Visible = true;
            this.gridColumnDauSize.VisibleIndex = 2;
            this.gridColumnDauSize.Width = 81;
            // 
            // gridColumnTenMau
            // 
            this.gridColumnTenMau.Caption = "Màu";
            this.gridColumnTenMau.FieldName = "TenMau";
            this.gridColumnTenMau.MinWidth = 21;
            this.gridColumnTenMau.Name = "gridColumnTenMau";
            this.gridColumnTenMau.Visible = true;
            this.gridColumnTenMau.VisibleIndex = 3;
            this.gridColumnTenMau.Width = 81;
            // 
            // gridColumnMaPKL
            // 
            this.gridColumnMaPKL.Caption = "Mã PKL";
            this.gridColumnMaPKL.FieldName = "MaPKL";
            this.gridColumnMaPKL.MinWidth = 21;
            this.gridColumnMaPKL.Name = "gridColumnMaPKL";
            this.gridColumnMaPKL.Visible = true;
            this.gridColumnMaPKL.VisibleIndex = 5;
            this.gridColumnMaPKL.Width = 93;
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            gridLevelNode1.LevelTemplate = this.cardView1;
            gridLevelNode1.RelationName = "Level1";
            this.gridControl1.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
            this.gridControl1.Location = new System.Drawing.Point(0, 29);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gridControl1.MenuManager = this.barManager1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(920, 337);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1,
            this.cardView1});
            this.gridControl1.ViewRegistered += new DevExpress.XtraGrid.ViewOperationEventHandler(this.gridControl1_ViewRegistered);
            // 
            // gridView1
            // 
            this.gridView1.ColumnPanelRowHeight = 32;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10,
            this.gridColumn11,
            this.gridColumn12,
            this.gridColumn13,
            this.gridColumn14});
            this.gridView1.DetailHeight = 284;
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.IndicatorWidth = 40;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsCustomization.AllowColumnMoving = false;
            this.gridView1.OptionsCustomization.AllowFilter = false;
            this.gridView1.OptionsCustomization.AllowSort = false;
            this.gridView1.OptionsView.AllowCellMerge = true;
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsView.ShowAutoFilterRow = true;
            this.gridView1.OptionsView.ShowFooter = true;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.CellMerge += new DevExpress.XtraGrid.Views.Grid.CellMergeEventHandler(this.gridView1_CellMerge);
            this.gridView1.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridview1_CustomDrawColumnHeader);
            this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
            this.gridView1.CustomDrawFooterCell += new DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventHandler(this.GridView_CustomDrawFooterCell);
            this.gridView1.CustomDrawFooter += new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.GridView_CustomDrawFooter);
            this.gridView1.MasterRowGetChildList += new DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventHandler(this.gridView1_MasterRowGetChildList);
            this.gridView1.MasterRowGetRelationName += new DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventHandler(this.gridView1_MasterRowGetRelationName);
            this.gridView1.MasterRowGetRelationDisplayCaption += new DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventHandler(this.gridView1_MasterRowGetRelationDisplayCaption);
            this.gridView1.MasterRowGetRelationCount += new DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventHandler(this.gridView1_MasterRowGetRelationCount);
            this.gridView1.CustomSummaryCalculate += new DevExpress.Data.CustomSummaryEventHandler(this.gridView_CustomSummaryCalculate);
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Tên hàng";
            this.gridColumn2.FieldName = "TenHang";
            this.gridColumn2.MinWidth = 19;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 2;
            this.gridColumn2.Width = 86;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "POID";
            this.gridColumn3.FieldName = "POID";
            this.gridColumn3.MinWidth = 19;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Width = 70;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "PO";
            this.gridColumn4.FieldName = "PO";
            this.gridColumn4.MinWidth = 19;
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 86;
            // 
            // gridColumn8
            // 
            this.gridColumn8.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn8.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn8.Caption = "Ngày đóng thùng";
            this.gridColumn8.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.gridColumn8.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn8.FieldName = "NgayDongThung";
            this.gridColumn8.MinWidth = 19;
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 0;
            this.gridColumn8.Width = 120;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Đơn vị sản xuất";
            this.gridColumn9.FieldName = "TenDVSX";
            this.gridColumn9.MinWidth = 19;
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 1;
            this.gridColumn9.Width = 111;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "Số thùng đã đóng";
            this.gridColumn10.FieldName = "SoThungDaDong";
            this.gridColumn10.MinWidth = 19;
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SoThungDaDong", "{0:n0}")});
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 4;
            this.gridColumn10.Width = 103;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "Số thùng chưa đóng";
            this.gridColumn11.FieldName = "SoThungChuaDong";
            this.gridColumn11.MinWidth = 19;
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Custom, "SoThungChuaDong", "{0:n0}")});
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 5;
            this.gridColumn11.Width = 103;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "Tổng số thùng";
            this.gridColumn12.FieldName = "TongSoThung";
            this.gridColumn12.MinWidth = 19;
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Custom, "TongSoThung", "{0:n0}")});
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 6;
            this.gridColumn12.Width = 103;
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "MaDVSX";
            this.gridColumn13.FieldName = "MaDVSX";
            this.gridColumn13.MinWidth = 21;
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.Width = 81;
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "MaHang";
            this.gridColumn14.FieldName = "MaHang";
            this.gridColumn14.MinWidth = 21;
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.Width = 81;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.Controller = this.barAndDockingController1;
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControl1);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barEditItem1,
            this.barEditItem2,
            this.barEditItem3,
            this.Naplai,
            this.XuatExcel,
            this.barButtonItem1});
            this.barManager1.MaxItemId = 9;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barEditItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.barEditItem2),
            new DevExpress.XtraBars.LinkPersistInfo(this.barEditItem3),
            new DevExpress.XtraBars.LinkPersistInfo(this.Naplai),
            new DevExpress.XtraBars.LinkPersistInfo(this.XuatExcel),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // barEditItem1
            // 
            this.barEditItem1.Edit = this.repositoryItemHHFilter;
            this.barEditItem1.Id = 6;
            this.barEditItem1.Name = "barEditItem1";
            this.barEditItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption;
            this.barEditItem1.Size = new System.Drawing.Size(150, 0);
            // 
            // repositoryItemHHFilter
            // 
            this.repositoryItemHHFilter.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemHHFilter.DisplayMember = "TenHang";
            this.repositoryItemHHFilter.Name = "repositoryItemHHFilter";
            this.repositoryItemHHFilter.NullText = "[Chọn mã hàng]";
            this.repositoryItemHHFilter.PopupView = this.gridViewMaHangSearchLookup;
            this.repositoryItemHHFilter.ShowFooter = false;
            this.repositoryItemHHFilter.ValueMember = "MaHang";
            this.repositoryItemHHFilter.EditValueChanged += new System.EventHandler(this.repositoryItemSearchHH_EditValueChanged);
            // 
            // gridViewMaHangSearchLookup
            // 
            this.gridViewMaHangSearchLookup.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridViewMaHangSearchLookup.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewMaHangSearchLookup.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridViewMaHangSearchLookup.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridViewMaHangSearchLookup.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridViewMaHangSearchLookup.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMaHangSearchLookup,
            this.colTenHangSearchLookup});
            this.gridViewMaHangSearchLookup.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewMaHangSearchLookup.Name = "gridViewMaHangSearchLookup";
            this.gridViewMaHangSearchLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewMaHangSearchLookup.OptionsView.ShowGroupPanel = false;
            // 
            // colMaHangSearchLookup
            // 
            this.colMaHangSearchLookup.AppearanceCell.BackColor = System.Drawing.Color.White;
            this.colMaHangSearchLookup.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(43)))), ((int)(((byte)(48)))));
            this.colMaHangSearchLookup.AppearanceCell.Options.UseBackColor = true;
            this.colMaHangSearchLookup.AppearanceCell.Options.UseForeColor = true;
            this.colMaHangSearchLookup.AppearanceCell.Options.UseTextOptions = true;
            this.colMaHangSearchLookup.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaHangSearchLookup.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colMaHangSearchLookup.AppearanceHeader.Options.UseBackColor = true;
            this.colMaHangSearchLookup.Caption = "MaHang";
            this.colMaHangSearchLookup.FieldName = "MaHang";
            this.colMaHangSearchLookup.Name = "colMaHangSearchLookup";
            this.colMaHangSearchLookup.OptionsColumn.AllowEdit = false;
            this.colMaHangSearchLookup.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            // 
            // colTenHangSearchLookup
            // 
            this.colTenHangSearchLookup.AppearanceCell.BackColor = System.Drawing.Color.White;
            this.colTenHangSearchLookup.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(43)))), ((int)(((byte)(48)))));
            this.colTenHangSearchLookup.AppearanceCell.Options.UseBackColor = true;
            this.colTenHangSearchLookup.AppearanceCell.Options.UseForeColor = true;
            this.colTenHangSearchLookup.AppearanceCell.Options.UseTextOptions = true;
            this.colTenHangSearchLookup.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colTenHangSearchLookup.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colTenHangSearchLookup.AppearanceHeader.Options.UseBackColor = true;
            this.colTenHangSearchLookup.Caption = "Hàng hóa";
            this.colTenHangSearchLookup.FieldName = "TenHang";
            this.colTenHangSearchLookup.Name = "colTenHangSearchLookup";
            this.colTenHangSearchLookup.OptionsColumn.AllowEdit = false;
            this.colTenHangSearchLookup.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colTenHangSearchLookup.Visible = true;
            this.colTenHangSearchLookup.VisibleIndex = 0;
            this.colTenHangSearchLookup.Width = 86;
            // 
            // barEditItem2
            // 
            this.barEditItem2.Edit = this.repositoryItemDateFilter;
            this.barEditItem2.Id = 6;
            this.barEditItem2.Name = "barEditItem2";
            this.barEditItem2.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption;
            this.barEditItem2.Size = new System.Drawing.Size(150, 0);
            // 
            // repositoryItemDateFilter
            // 
            this.repositoryItemDateFilter.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemDateFilter.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemDateFilter.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.repositoryItemDateFilter.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.repositoryItemDateFilter.Name = "repositoryItemDateFilter";
            this.repositoryItemDateFilter.NullText = "[Chọn ngày đóng thùng]";
            this.repositoryItemDateFilter.EditValueChanged += new System.EventHandler(this.repositoryItemDateEdit_EditValueChanged);
            // 
            // barEditItem3
            // 
            this.barEditItem3.Edit = this.repositoryItemDVSXFilter;
            this.barEditItem3.Id = 7;
            this.barEditItem3.Name = "barEditItem3";
            this.barEditItem3.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption;
            this.barEditItem3.Size = new System.Drawing.Size(150, 0);
            // 
            // repositoryItemDVSXFilter
            // 
            this.repositoryItemDVSXFilter.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemDVSXFilter.DisplayMember = "TenDVSX";
            this.repositoryItemDVSXFilter.Name = "repositoryItemDVSXFilter";
            this.repositoryItemDVSXFilter.NullText = "[Chọn đơn vị sản xuất]";
            this.repositoryItemDVSXFilter.PopupView = this.gridViewMaDVSXSearchLookup;
            this.repositoryItemDVSXFilter.ShowFooter = false;
            this.repositoryItemDVSXFilter.ValueMember = "MaDVSX";
            this.repositoryItemDVSXFilter.EditValueChanged += new System.EventHandler(this.repositoryItemDVSXFilter_EditValueChanged);
            // 
            // gridViewMaDVSXSearchLookup
            // 
            this.gridViewMaDVSXSearchLookup.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridViewMaDVSXSearchLookup.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewMaDVSXSearchLookup.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridViewMaDVSXSearchLookup.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridViewMaDVSXSearchLookup.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridViewMaDVSXSearchLookup.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMaDVSXSearchLookup,
            this.colTenDVSXSearchLookup});
            this.gridViewMaDVSXSearchLookup.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewMaDVSXSearchLookup.Name = "gridViewMaDVSXSearchLookup";
            this.gridViewMaDVSXSearchLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewMaDVSXSearchLookup.OptionsView.ShowGroupPanel = false;
            // 
            // colMaDVSXSearchLookup
            // 
            this.colMaDVSXSearchLookup.AppearanceCell.BackColor = System.Drawing.Color.White;
            this.colMaDVSXSearchLookup.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(43)))), ((int)(((byte)(48)))));
            this.colMaDVSXSearchLookup.AppearanceCell.Options.UseBackColor = true;
            this.colMaDVSXSearchLookup.AppearanceCell.Options.UseForeColor = true;
            this.colMaDVSXSearchLookup.AppearanceCell.Options.UseTextOptions = true;
            this.colMaDVSXSearchLookup.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaDVSXSearchLookup.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colMaDVSXSearchLookup.AppearanceHeader.Options.UseBackColor = true;
            this.colMaDVSXSearchLookup.Caption = "MaDVSX";
            this.colMaDVSXSearchLookup.FieldName = "MaDVSX";
            this.colMaDVSXSearchLookup.Name = "colMaDVSXSearchLookup";
            this.colMaDVSXSearchLookup.OptionsColumn.AllowEdit = false;
            this.colMaDVSXSearchLookup.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            // 
            // colTenDVSXSearchLookup
            // 
            this.colTenDVSXSearchLookup.AppearanceCell.BackColor = System.Drawing.Color.White;
            this.colTenDVSXSearchLookup.AppearanceCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(43)))), ((int)(((byte)(48)))));
            this.colTenDVSXSearchLookup.AppearanceCell.Options.UseBackColor = true;
            this.colTenDVSXSearchLookup.AppearanceCell.Options.UseForeColor = true;
            this.colTenDVSXSearchLookup.AppearanceCell.Options.UseTextOptions = true;
            this.colTenDVSXSearchLookup.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colTenDVSXSearchLookup.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colTenDVSXSearchLookup.AppearanceHeader.Options.UseBackColor = true;
            this.colTenDVSXSearchLookup.Caption = "Đơn vị sản xuất";
            this.colTenDVSXSearchLookup.FieldName = "TenDVSX";
            this.colTenDVSXSearchLookup.Name = "colTenDVSXSearchLookup";
            this.colTenDVSXSearchLookup.OptionsColumn.AllowEdit = false;
            this.colTenDVSXSearchLookup.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colTenDVSXSearchLookup.Visible = true;
            this.colTenDVSXSearchLookup.VisibleIndex = 0;
            this.colTenDVSXSearchLookup.Width = 86;
            // 
            // Naplai
            // 
            this.Naplai.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.Naplai.Caption = "Nạp lại (F5)";
            this.Naplai.Id = 9;
            this.Naplai.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("Naplai.ImageOptions.Image")));
            this.Naplai.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("Naplai.ImageOptions.LargeImage")));
            this.Naplai.ItemAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.Naplai.ItemAppearance.Normal.Options.UseFont = true;
            this.Naplai.Name = "Naplai";
            this.Naplai.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Naplai.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Naplai_ItemClick);
            // 
            // XuatExcel
            // 
            this.XuatExcel.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.XuatExcel.Caption = "Xuất Excel (Ctrl + P)";
            this.XuatExcel.Id = 9;
            this.XuatExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("XuatExcel.ImageOptions.Image")));
            this.XuatExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("XuatExcel.ImageOptions.LargeImage")));
            this.XuatExcel.ItemAppearance.Disabled.Font = new System.Drawing.Font("Tahoma", 7.9F);
            this.XuatExcel.ItemAppearance.Disabled.Options.UseFont = true;
            this.XuatExcel.ItemAppearance.Hovered.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.XuatExcel.ItemAppearance.Hovered.Options.UseFont = true;
            this.XuatExcel.ItemAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.XuatExcel.ItemAppearance.Normal.Options.UseFont = true;
            this.XuatExcel.ItemAppearance.Pressed.Font = new System.Drawing.Font("Tahoma", 7.9F);
            this.XuatExcel.ItemAppearance.Pressed.Options.UseFont = true;
            this.XuatExcel.Name = "XuatExcel";
            this.XuatExcel.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.XuatExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.XuatExcel_ItemClick);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Lọc dữ liệu";
            this.barButtonItem1.Id = 8;
            this.barButtonItem1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.Image")));
            this.barButtonItem1.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.LargeImage")));
            this.barButtonItem1.ItemAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.barButtonItem1.ItemAppearance.Normal.Options.UseFont = true;
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // barAndDockingController1
            // 
            this.barAndDockingController1.LookAndFeel.SkinMaskColor = System.Drawing.Color.Lime;
            this.barAndDockingController1.LookAndFeel.SkinName = "Office 2010 Blue";
            this.barAndDockingController1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(920, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 366);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(920, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 337);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl1.Location = new System.Drawing.Point(920, 29);
            this.barDockControl1.Manager = this.barManager1;
            this.barDockControl1.Size = new System.Drawing.Size(0, 337);
            // 
            // gridColPO
            // 
            this.gridColPO.Caption = "PO";
            this.gridColPO.FieldName = "PO";
            this.gridColPO.Name = "gridColPO";
            this.gridColPO.Visible = true;
            this.gridColPO.VisibleIndex = 0;
            this.gridColPO.Width = 64;
            // 
            // frmBaoTongQuanDongThung
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 366);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControl1);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmBaoTongQuanDongThung";
            this.Text = "Báo cáo tổng quan đóng thùng";
            ((System.ComponentModel.ISupportInitialize)(this.cardView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemHHFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMaHangSearchLookup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateFilter.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDateFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDVSXFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMaDVSXSearchLookup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControl1;

        private DevExpress.XtraBars.BarButtonItem Naplai;
        private DevExpress.XtraBars.BarButtonItem XuatExcel;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;

        private DevExpress.XtraBars.BarEditItem barEditItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit repositoryItemHHFilter;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMaHangSearchLookup;
        private DevExpress.XtraGrid.Columns.GridColumn colMaHangSearchLookup;
        private DevExpress.XtraGrid.Columns.GridColumn colTenHangSearchLookup;

        private DevExpress.XtraBars.BarEditItem barEditItem2;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repositoryItemDateFilter;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;

        private DevExpress.XtraBars.BarEditItem barEditItem3;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit repositoryItemDVSXFilter;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMaDVSXSearchLookup;
        private DevExpress.XtraGrid.Columns.GridColumn colMaDVSXSearchLookup;
        private DevExpress.XtraGrid.Columns.GridColumn colTenDVSXSearchLookup;

        private DevExpress.XtraGrid.Views.Card.CardView cardView1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSTTThung;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSoLuongThung;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnTuThung;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDenThung;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSize;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnSoLuongSP;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnDauSize;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnTenMau;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnMaPKL;
        private DevExpress.XtraGrid.Columns.GridColumn gridColPO;
    }
}