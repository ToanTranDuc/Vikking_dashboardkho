
namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    partial class frmChiTietScanThungXuatHang
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChiTietScanThungXuatHang));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.btRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.btnScan = new DevExpress.XtraBars.BarButtonItem();
            this.btnSave = new DevExpress.XtraBars.BarButtonItem();
            this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btEdit = new DevExpress.XtraBars.BarButtonItem();
            this.btDelete = new DevExpress.XtraBars.BarButtonItem();
            this.btSave = new DevExpress.XtraBars.BarButtonItem();
            this.btnGopThung = new DevExpress.XtraBars.BarButtonItem();
            this.btnPKLPO = new DevExpress.XtraBars.BarButtonItem();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.dgcTong = new DevExpress.XtraGrid.GridControl();
            this.grvTong = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.bandedGridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.bandedGridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.bandedGridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTongThung = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPOA = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaHang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBarcodeA = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStoreA = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSttThung = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTinhTrang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSTDaQuet = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBarcode2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtScanQR = new System.Windows.Forms.TextBox();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblScan = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgcTong)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvTong)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblScan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btAdd,
            this.btEdit,
            this.btDelete,
            this.btSave,
            this.btRefresh,
            this.btnGopThung,
            this.btnPKLPO,
            this.btnScan,
            this.btnSave,
            this.btnDelete});
            this.barManager1.MaxItemId = 17;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btRefresh),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnScan),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnDelete)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // btRefresh
            // 
            this.btRefresh.Caption = "Nạp lại (F5)";
            this.btRefresh.Id = 4;
            this.btRefresh.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btRefresh.ImageOptions.Image")));
            this.btRefresh.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btRefresh.ImageOptions.LargeImage")));
            this.btRefresh.Name = "btRefresh";
            this.btRefresh.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btRefresh_ItemClick);
            // 
            // btnScan
            // 
            this.btnScan.Caption = "ScanQR";
            this.btnScan.Id = 12;
            this.btnScan.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnScan.ImageOptions.Image")));
            this.btnScan.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnScan.ImageOptions.LargeImage")));
            this.btnScan.Name = "btnScan";
            this.btnScan.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnScan.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // btnSave
            // 
            this.btnSave.Caption = "Lưu";
            this.btnSave.Id = 15;
            this.btnSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.ImageOptions.Image")));
            this.btnSave.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnSave.ImageOptions.LargeImage")));
            this.btnSave.Name = "btnSave";
            this.btnSave.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSave_ItemClick);
            // 
            // btnDelete
            // 
            this.btnDelete.Caption = "Xóa";
            this.btnDelete.Id = 16;
            this.btnDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.Image")));
            this.btnDelete.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.LargeImage")));
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.btnDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnDelete_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlTop.Size = new System.Drawing.Size(1535, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 676);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(1535, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 640);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1535, 36);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 640);
            // 
            // btAdd
            // 
            this.btAdd.Caption = "Thêm (F1)";
            this.btAdd.Id = 0;
            this.btAdd.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btAdd.ImageOptions.Image")));
            this.btAdd.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btAdd.ImageOptions.LargeImage")));
            this.btAdd.Name = "btAdd";
            this.btAdd.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btEdit
            // 
            this.btEdit.Caption = "Sửa (F2)";
            this.btEdit.Id = 1;
            this.btEdit.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btEdit.ImageOptions.Image")));
            this.btEdit.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btEdit.ImageOptions.LargeImage")));
            this.btEdit.Name = "btEdit";
            this.btEdit.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btDelete
            // 
            this.btDelete.Caption = "Xóa (F3)";
            this.btDelete.Id = 2;
            this.btDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btDelete.ImageOptions.Image")));
            this.btDelete.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btDelete.ImageOptions.LargeImage")));
            this.btDelete.Name = "btDelete";
            this.btDelete.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btSave
            // 
            this.btSave.Caption = "Lưu (F4)";
            this.btSave.Id = 3;
            this.btSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btSave.ImageOptions.Image")));
            this.btSave.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btSave.ImageOptions.LargeImage")));
            this.btSave.Name = "btSave";
            this.btSave.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btnGopThung
            // 
            this.btnGopThung.Caption = "Gọp thùng";
            this.btnGopThung.Id = 7;
            this.btnGopThung.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGopThung.ImageOptions.Image")));
            this.btnGopThung.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnGopThung.ImageOptions.LargeImage")));
            this.btnGopThung.Name = "btnGopThung";
            this.btnGopThung.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btnPKLPO
            // 
            this.btnPKLPO.Caption = "Lập PKL PO New";
            this.btnPKLPO.Id = 9;
            this.btnPKLPO.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPKLPO.ImageOptions.Image")));
            this.btnPKLPO.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnPKLPO.ImageOptions.LargeImage")));
            this.btnPKLPO.Name = "btnPKLPO";
            this.btnPKLPO.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barAndDockingController1
            // 
            this.barAndDockingController1.LookAndFeel.SkinMaskColor = System.Drawing.Color.Lime;
            this.barAndDockingController1.LookAndFeel.SkinName = "Office 2010 Blue";
            this.barAndDockingController1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
            // 
            // dgcTong
            // 
            this.dgcTong.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgcTong.Location = new System.Drawing.Point(14, 43);
            this.dgcTong.MainView = this.grvTong;
            this.dgcTong.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgcTong.MenuManager = this.barManager1;
            this.dgcTong.Name = "dgcTong";
            this.dgcTong.Size = new System.Drawing.Size(1507, 583);
            this.dgcTong.TabIndex = 4;
            this.dgcTong.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvTong,
            this.gridView1});
            this.dgcTong.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.dgcTong_ProcessGridKey);
            // 
            // grvTong
            // 
            this.grvTong.Appearance.FocusedRow.BackColor = System.Drawing.Color.LightSkyBlue;
            this.grvTong.Appearance.FocusedRow.Options.UseBackColor = true;
            this.grvTong.Appearance.FooterPanel.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold);
            this.grvTong.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Red;
            this.grvTong.Appearance.FooterPanel.Options.UseFont = true;
            this.grvTong.Appearance.FooterPanel.Options.UseForeColor = true;
            this.grvTong.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.bandedGridColumn1,
            this.bandedGridColumn2,
            this.bandedGridColumn3,
            this.colTongThung,
            this.colPOA,
            this.colMaHang,
            this.colBarcodeA,
            this.colStoreA,
            this.colSttThung,
            this.colTinhTrang,
            this.colSTDaQuet,
            this.colBarcode2});
            this.grvTong.DetailHeight = 431;
            this.grvTong.GridControl = this.dgcTong;
            this.grvTong.Name = "grvTong";
            this.grvTong.OptionsView.ColumnAutoWidth = false;
            this.grvTong.OptionsView.ShowFooter = true;
            this.grvTong.OptionsView.ShowGroupPanel = false;
            this.grvTong.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.grvTong_RowStyle);
            this.grvTong.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.grvTong_CellValueChanging);
            // 
            // bandedGridColumn1
            // 
            this.bandedGridColumn1.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bandedGridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.bandedGridColumn1.AppearanceHeader.Options.UseBackColor = true;
            this.bandedGridColumn1.AppearanceHeader.Options.UseFont = true;
            this.bandedGridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.bandedGridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.bandedGridColumn1.Caption = "Nhóm size";
            this.bandedGridColumn1.FieldName = "DauSize";
            this.bandedGridColumn1.MinWidth = 27;
            this.bandedGridColumn1.Name = "bandedGridColumn1";
            this.bandedGridColumn1.OptionsColumn.AllowEdit = false;
            this.bandedGridColumn1.Visible = true;
            this.bandedGridColumn1.VisibleIndex = 3;
            this.bandedGridColumn1.Width = 128;
            // 
            // bandedGridColumn2
            // 
            this.bandedGridColumn2.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bandedGridColumn2.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.bandedGridColumn2.AppearanceHeader.Options.UseBackColor = true;
            this.bandedGridColumn2.AppearanceHeader.Options.UseFont = true;
            this.bandedGridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.bandedGridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.bandedGridColumn2.Caption = "Màu";
            this.bandedGridColumn2.FieldName = "TenMau";
            this.bandedGridColumn2.MinWidth = 27;
            this.bandedGridColumn2.Name = "bandedGridColumn2";
            this.bandedGridColumn2.OptionsColumn.AllowEdit = false;
            this.bandedGridColumn2.Visible = true;
            this.bandedGridColumn2.VisibleIndex = 4;
            this.bandedGridColumn2.Width = 94;
            // 
            // bandedGridColumn3
            // 
            this.bandedGridColumn3.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bandedGridColumn3.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.bandedGridColumn3.AppearanceHeader.Options.UseBackColor = true;
            this.bandedGridColumn3.AppearanceHeader.Options.UseFont = true;
            this.bandedGridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.bandedGridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.bandedGridColumn3.Caption = "Size";
            this.bandedGridColumn3.FieldName = "Size";
            this.bandedGridColumn3.MinWidth = 27;
            this.bandedGridColumn3.Name = "bandedGridColumn3";
            this.bandedGridColumn3.OptionsColumn.AllowEdit = false;
            this.bandedGridColumn3.Visible = true;
            this.bandedGridColumn3.VisibleIndex = 5;
            this.bandedGridColumn3.Width = 73;
            // 
            // colTongThung
            // 
            this.colTongThung.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.colTongThung.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colTongThung.AppearanceHeader.Options.UseBackColor = true;
            this.colTongThung.AppearanceHeader.Options.UseFont = true;
            this.colTongThung.AppearanceHeader.Options.UseTextOptions = true;
            this.colTongThung.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colTongThung.Caption = "SL Thùng";
            this.colTongThung.FieldName = "SLThung";
            this.colTongThung.MinWidth = 29;
            this.colTongThung.Name = "colTongThung";
            this.colTongThung.OptionsColumn.AllowEdit = false;
            this.colTongThung.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SLThung", "{0:N0}")});
            this.colTongThung.Visible = true;
            this.colTongThung.VisibleIndex = 6;
            this.colTongThung.Width = 80;
            // 
            // colPOA
            // 
            this.colPOA.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.colPOA.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colPOA.AppearanceHeader.Options.UseBackColor = true;
            this.colPOA.AppearanceHeader.Options.UseFont = true;
            this.colPOA.AppearanceHeader.Options.UseTextOptions = true;
            this.colPOA.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colPOA.Caption = "PO";
            this.colPOA.FieldName = "PO";
            this.colPOA.MinWidth = 24;
            this.colPOA.Name = "colPOA";
            this.colPOA.OptionsColumn.AllowEdit = false;
            this.colPOA.Visible = true;
            this.colPOA.VisibleIndex = 1;
            this.colPOA.Width = 96;
            // 
            // colMaHang
            // 
            this.colMaHang.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.colMaHang.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colMaHang.AppearanceHeader.Options.UseBackColor = true;
            this.colMaHang.AppearanceHeader.Options.UseFont = true;
            this.colMaHang.AppearanceHeader.Options.UseTextOptions = true;
            this.colMaHang.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaHang.Caption = "Mã hàng";
            this.colMaHang.FieldName = "TenHang";
            this.colMaHang.MinWidth = 24;
            this.colMaHang.Name = "colMaHang";
            this.colMaHang.OptionsColumn.AllowEdit = false;
            this.colMaHang.Visible = true;
            this.colMaHang.VisibleIndex = 0;
            this.colMaHang.Width = 121;
            // 
            // colBarcodeA
            // 
            this.colBarcodeA.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.colBarcodeA.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colBarcodeA.AppearanceHeader.Options.UseBackColor = true;
            this.colBarcodeA.AppearanceHeader.Options.UseFont = true;
            this.colBarcodeA.AppearanceHeader.Options.UseTextOptions = true;
            this.colBarcodeA.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colBarcodeA.Caption = "Barcode";
            this.colBarcodeA.FieldName = "Barcode";
            this.colBarcodeA.MinWidth = 24;
            this.colBarcodeA.Name = "colBarcodeA";
            this.colBarcodeA.Visible = true;
            this.colBarcodeA.VisibleIndex = 9;
            this.colBarcodeA.Width = 230;
            // 
            // colStoreA
            // 
            this.colStoreA.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.colStoreA.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colStoreA.AppearanceHeader.Options.UseBackColor = true;
            this.colStoreA.AppearanceHeader.Options.UseFont = true;
            this.colStoreA.AppearanceHeader.Options.UseTextOptions = true;
            this.colStoreA.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colStoreA.Caption = "Store";
            this.colStoreA.FieldName = "Store";
            this.colStoreA.MinWidth = 24;
            this.colStoreA.Name = "colStoreA";
            this.colStoreA.OptionsColumn.AllowEdit = false;
            this.colStoreA.Visible = true;
            this.colStoreA.VisibleIndex = 2;
            this.colStoreA.Width = 94;
            // 
            // colSttThung
            // 
            this.colSttThung.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.colSttThung.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold);
            this.colSttThung.AppearanceHeader.Options.UseBackColor = true;
            this.colSttThung.AppearanceHeader.Options.UseFont = true;
            this.colSttThung.AppearanceHeader.Options.UseTextOptions = true;
            this.colSttThung.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSttThung.Caption = "Stt Thùng";
            this.colSttThung.FieldName = "SttThungDisplay";
            this.colSttThung.MinWidth = 24;
            this.colSttThung.Name = "colSttThung";
            this.colSttThung.OptionsColumn.AllowEdit = false;
            this.colSttThung.Visible = true;
            this.colSttThung.VisibleIndex = 8;
            this.colSttThung.Width = 94;
            // 
            // colTinhTrang
            // 
            this.colTinhTrang.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.colTinhTrang.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold);
            this.colTinhTrang.AppearanceHeader.Options.UseBackColor = true;
            this.colTinhTrang.AppearanceHeader.Options.UseFont = true;
            this.colTinhTrang.AppearanceHeader.Options.UseTextOptions = true;
            this.colTinhTrang.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colTinhTrang.Caption = "Tình trạng";
            this.colTinhTrang.FieldName = "IsScan";
            this.colTinhTrang.MinWidth = 24;
            this.colTinhTrang.Name = "colTinhTrang";
            this.colTinhTrang.OptionsColumn.AllowEdit = false;
            this.colTinhTrang.Visible = true;
            this.colTinhTrang.VisibleIndex = 11;
            this.colTinhTrang.Width = 107;
            // 
            // colSTDaQuet
            // 
            this.colSTDaQuet.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.colSTDaQuet.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold);
            this.colSTDaQuet.AppearanceHeader.Options.UseBackColor = true;
            this.colSTDaQuet.AppearanceHeader.Options.UseFont = true;
            this.colSTDaQuet.AppearanceHeader.Options.UseTextOptions = true;
            this.colSTDaQuet.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSTDaQuet.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colSTDaQuet.Caption = "ST đã quét";
            this.colSTDaQuet.FieldName = "STDaQuet";
            this.colSTDaQuet.MinWidth = 24;
            this.colSTDaQuet.Name = "colSTDaQuet";
            this.colSTDaQuet.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "STDaQuet", "{0:N0}")});
            this.colSTDaQuet.Visible = true;
            this.colSTDaQuet.VisibleIndex = 7;
            this.colSTDaQuet.Width = 87;
            // 
            // colBarcode2
            // 
            this.colBarcode2.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.colBarcode2.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colBarcode2.AppearanceHeader.Options.UseBackColor = true;
            this.colBarcode2.AppearanceHeader.Options.UseFont = true;
            this.colBarcode2.AppearanceHeader.Options.UseTextOptions = true;
            this.colBarcode2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colBarcode2.Caption = "Barcode 2";
            this.colBarcode2.FieldName = "Barcode2";
            this.colBarcode2.MinWidth = 24;
            this.colBarcode2.Name = "colBarcode2";
            this.colBarcode2.Visible = true;
            this.colBarcode2.VisibleIndex = 10;
            this.colBarcode2.Width = 210;
            // 
            // gridView1
            // 
            this.gridView1.DetailHeight = 431;
            this.gridView1.GridControl = this.dgcTong;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtScanQR);
            this.layoutControl1.Controls.Add(this.dgcTong);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 36);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1535, 640);
            this.layoutControl1.TabIndex = 9;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtScanQR
            // 
            this.txtScanQR.Location = new System.Drawing.Point(95, 14);
            this.txtScanQR.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtScanQR.Name = "txtScanQR";
            this.txtScanQR.Size = new System.Drawing.Size(454, 25);
            this.txtScanQR.TabIndex = 5;
            this.txtScanQR.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtScanQR_KeyPress);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.lblScan,
            this.emptySpaceItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1535, 640);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.dgcTong;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 29);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1511, 587);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // lblScan
            // 
            this.lblScan.Control = this.txtScanQR;
            this.lblScan.Location = new System.Drawing.Point(0, 0);
            this.lblScan.Name = "lblScan";
            this.lblScan.Size = new System.Drawing.Size(539, 29);
            this.lblScan.Text = "Scan Barcode";
            this.lblScan.TextSize = new System.Drawing.Size(78, 16);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(539, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(972, 29);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // frmChiTietScanThungXuatHang
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1535, 676);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmChiTietScanThungXuatHang";
            this.Text = "Scanbarcode thùng";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgcTong)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvTong)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblScan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem btRefresh;
        private DevExpress.XtraBars.BarButtonItem btnScan;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem btAdd;
        private DevExpress.XtraBars.BarButtonItem btEdit;
        private DevExpress.XtraBars.BarButtonItem btDelete;
        private DevExpress.XtraBars.BarButtonItem btSave;
        private DevExpress.XtraBars.BarButtonItem btnGopThung;
        private DevExpress.XtraBars.BarButtonItem btnPKLPO;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraGrid.GridControl dgcTong;
        private DevExpress.XtraGrid.Views.Grid.GridView grvTong;
        private DevExpress.XtraGrid.Columns.GridColumn bandedGridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn bandedGridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn bandedGridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn colTongThung;
        private DevExpress.XtraGrid.Columns.GridColumn colPOA;
        private DevExpress.XtraGrid.Columns.GridColumn colMaHang;
        private DevExpress.XtraGrid.Columns.GridColumn colBarcodeA;
        private DevExpress.XtraGrid.Columns.GridColumn colStoreA;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colSttThung;
        private DevExpress.XtraGrid.Columns.GridColumn colTinhTrang;
        private DevExpress.XtraBars.BarButtonItem btnSave;
        private DevExpress.XtraGrid.Columns.GridColumn colSTDaQuet;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private System.Windows.Forms.TextBox txtScanQR;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem lblScan;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Columns.GridColumn colBarcode2;
        private DevExpress.XtraBars.BarButtonItem btnDelete;
    }
}