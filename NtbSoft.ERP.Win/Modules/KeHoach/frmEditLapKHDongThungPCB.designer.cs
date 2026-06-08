
namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    partial class frmEditLapKHDongThungPCB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditLapKHDongThungPCB));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.btnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btEdit = new DevExpress.XtraBars.BarButtonItem();
            this.btDelete = new DevExpress.XtraBars.BarButtonItem();
            this.btSave = new DevExpress.XtraBars.BarButtonItem();
            this.btRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.btnLapKeHoach = new DevExpress.XtraBars.BarButtonItem();
            this.btnGopThung = new DevExpress.XtraBars.BarButtonItem();
            this.btnAddorEdit = new DevExpress.XtraBars.BarButtonItem();
            this.btnPKLPO = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportPKLToTal = new DevExpress.XtraBars.BarButtonItem();
            this.barSubItem1 = new DevExpress.XtraBars.BarSubItem();
            this.btnLapKeHoachN = new DevExpress.XtraBars.BarButtonItem();
            this.btnPKLPON = new DevExpress.XtraBars.BarButtonItem();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.grcDetail = new DevExpress.XtraGrid.GridControl();
            this.gridviewDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSLKH = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSLLapPKL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSLDaLap = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridviewDetail)).BeginInit();
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
            this.btnLapKeHoach,
            this.btnRefresh,
            this.btnGopThung,
            this.btnAddorEdit,
            this.btnPKLPO,
            this.btnExportPKLToTal,
            this.barSubItem1,
            this.btnLapKeHoachN,
            this.btnPKLPON});
            this.barManager1.MaxItemId = 40;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnRefresh)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Caption = "Nạp lại (F5)";
            this.btnRefresh.Id = 6;
            this.btnRefresh.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.ImageOptions.Image")));
            this.btnRefresh.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnRefresh.ImageOptions.LargeImage")));
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlTop.Size = new System.Drawing.Size(864, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 444);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(864, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 408);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(864, 36);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 408);
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
            this.btSave.Caption = "Lưu (Ctrl + S )";
            this.btSave.Id = 3;
            this.btSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btSave.ImageOptions.Image")));
            this.btSave.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btSave.ImageOptions.LargeImage")));
            this.btSave.Name = "btSave";
            this.btSave.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btRefresh
            // 
            this.btRefresh.Caption = "Nạp lại (F5)";
            this.btRefresh.Id = 4;
            this.btRefresh.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btRefresh.ImageOptions.Image")));
            this.btRefresh.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btRefresh.ImageOptions.LargeImage")));
            this.btRefresh.Name = "btRefresh";
            this.btRefresh.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btnLapKeHoach
            // 
            this.btnLapKeHoach.Caption = "Lập PKL mới";
            this.btnLapKeHoach.Id = 5;
            this.btnLapKeHoach.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnLapKeHoach.ImageOptions.Image")));
            this.btnLapKeHoach.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnLapKeHoach.ImageOptions.LargeImage")));
            this.btnLapKeHoach.Name = "btnLapKeHoach";
            this.btnLapKeHoach.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btnGopThung
            // 
            this.btnGopThung.Caption = "Gọp thùng(Ctrl+G)";
            this.btnGopThung.Id = 7;
            this.btnGopThung.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnGopThung.ImageOptions.Image")));
            this.btnGopThung.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnGopThung.ImageOptions.LargeImage")));
            this.btnGopThung.Name = "btnGopThung";
            this.btnGopThung.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btnAddorEdit
            // 
            this.btnAddorEdit.Caption = "Sửa PKL(Ctrl +E)";
            this.btnAddorEdit.Id = 8;
            this.btnAddorEdit.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAddorEdit.ImageOptions.Image")));
            this.btnAddorEdit.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnAddorEdit.ImageOptions.LargeImage")));
            this.btnAddorEdit.Name = "btnAddorEdit";
            this.btnAddorEdit.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
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
            // btnExportPKLToTal
            // 
            this.btnExportPKLToTal.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Left;
            this.btnExportPKLToTal.Caption = "Xuất EX(Ctrl+P)";
            this.btnExportPKLToTal.Id = 10;
            this.btnExportPKLToTal.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnExportPKLToTal.ImageOptions.Image")));
            this.btnExportPKLToTal.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnExportPKLToTal.ImageOptions.LargeImage")));
            this.btnExportPKLToTal.Name = "btnExportPKLToTal";
            this.btnExportPKLToTal.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barSubItem1
            // 
            this.barSubItem1.Caption = "Lập kế hoạch";
            this.barSubItem1.Id = 11;
            this.barSubItem1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barSubItem1.ImageOptions.Image")));
            this.barSubItem1.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barSubItem1.ImageOptions.LargeImage")));
            this.barSubItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnLapKeHoachN),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnPKLPON)});
            this.barSubItem1.Name = "barSubItem1";
            this.barSubItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btnLapKeHoachN
            // 
            this.btnLapKeHoachN.Caption = "Lập PKL PO đợt(Ctrl+Shift+N)";
            this.btnLapKeHoachN.Id = 12;
            this.btnLapKeHoachN.Name = "btnLapKeHoachN";
            // 
            // btnPKLPON
            // 
            this.btnPKLPON.Caption = "Lập PKL PO PCB(Ctrl+Shift+M)";
            this.btnPKLPON.Id = 13;
            this.btnPKLPON.Name = "btnPKLPON";
            // 
            // barAndDockingController1
            // 
            this.barAndDockingController1.LookAndFeel.SkinMaskColor = System.Drawing.Color.Lime;
            this.barAndDockingController1.LookAndFeel.SkinName = "Office 2010 Blue";
            this.barAndDockingController1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
            // 
            // grcDetail
            // 
            this.grcDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grcDetail.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grcDetail.Location = new System.Drawing.Point(0, 36);
            this.grcDetail.MainView = this.gridviewDetail;
            this.grcDetail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grcDetail.Name = "grcDetail";
            this.grcDetail.Size = new System.Drawing.Size(864, 408);
            this.grcDetail.TabIndex = 37;
            this.grcDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridviewDetail});
            // 
            // gridviewDetail
            // 
            this.gridviewDetail.Appearance.FooterPanel.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold);
            this.gridviewDetail.Appearance.FooterPanel.ForeColor = System.Drawing.Color.Red;
            this.gridviewDetail.Appearance.FooterPanel.Options.UseFont = true;
            this.gridviewDetail.Appearance.FooterPanel.Options.UseForeColor = true;
            this.gridviewDetail.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9F);
            this.gridviewDetail.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridviewDetail.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridviewDetail.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridviewDetail.Appearance.HeaderPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridviewDetail.ColumnPanelRowHeight = 34;
            this.gridviewDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10,
            this.colSLKH,
            this.gridColumn14,
            this.gridColumn15,
            this.colSLLapPKL,
            this.colSLDaLap});
            this.gridviewDetail.GridControl = this.grcDetail;
            this.gridviewDetail.Name = "gridviewDetail";
            this.gridviewDetail.OptionsView.ShowAutoFilterRow = true;
            this.gridviewDetail.OptionsView.ShowFooter = true;
            this.gridviewDetail.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridColumn2.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn2.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumn2.AppearanceHeader.Options.UseFont = true;
            this.gridColumn2.Caption = "Nhóm size";
            this.gridColumn2.FieldName = "DauSize";
            this.gridColumn2.MinWidth = 24;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 2;
            this.gridColumn2.Width = 113;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "SizeTypeID";
            this.gridColumn3.FieldName = "DauSizeID";
            this.gridColumn3.MinWidth = 24;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Width = 94;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "POID";
            this.gridColumn4.FieldName = "POID";
            this.gridColumn4.MinWidth = 24;
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.Width = 94;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridColumn5.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold);
            this.gridColumn5.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumn5.AppearanceHeader.Options.UseFont = true;
            this.gridColumn5.Caption = "PO";
            this.gridColumn5.FieldName = "PO";
            this.gridColumn5.MinWidth = 24;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 0;
            this.gridColumn5.Width = 108;
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridColumn6.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn6.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumn6.AppearanceHeader.Options.UseFont = true;
            this.gridColumn6.Caption = "Màu";
            this.gridColumn6.FieldName = "TenMau";
            this.gridColumn6.MinWidth = 24;
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 3;
            this.gridColumn6.Width = 94;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "ColorID";
            this.gridColumn7.FieldName = "ColorID";
            this.gridColumn7.MinWidth = 24;
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Width = 94;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Từ thùng - Đến thùng";
            this.gridColumn8.FieldName = "TuThung_Denthung";
            this.gridColumn8.MinWidth = 24;
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.OptionsColumn.AllowEdit = false;
            this.gridColumn8.Width = 176;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "SizeID";
            this.gridColumn9.FieldName = "SizeID";
            this.gridColumn9.MinWidth = 24;
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.OptionsColumn.AllowEdit = false;
            this.gridColumn9.Width = 94;
            // 
            // gridColumn10
            // 
            this.gridColumn10.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridColumn10.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn10.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumn10.AppearanceHeader.Options.UseFont = true;
            this.gridColumn10.Caption = "Size";
            this.gridColumn10.FieldName = "Size";
            this.gridColumn10.MinWidth = 24;
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.OptionsColumn.AllowEdit = false;
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 4;
            this.gridColumn10.Width = 138;
            // 
            // colSLKH
            // 
            this.colSLKH.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colSLKH.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colSLKH.AppearanceHeader.Options.UseBackColor = true;
            this.colSLKH.AppearanceHeader.Options.UseFont = true;
            this.colSLKH.Caption = "SLKH";
            this.colSLKH.FieldName = "SLKH";
            this.colSLKH.MinWidth = 24;
            this.colSLKH.Name = "colSLKH";
            this.colSLKH.OptionsColumn.AllowEdit = false;
            this.colSLKH.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SLKH", "{0:0}")});
            this.colSLKH.Visible = true;
            this.colSLKH.VisibleIndex = 5;
            this.colSLKH.Width = 92;
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "MaKhachHang";
            this.gridColumn14.FieldName = "MaKhachHang";
            this.gridColumn14.MinWidth = 24;
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.Width = 94;
            // 
            // gridColumn15
            // 
            this.gridColumn15.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridColumn15.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn15.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumn15.AppearanceHeader.Options.UseFont = true;
            this.gridColumn15.Caption = "Đơn vị";
            this.gridColumn15.FieldName = "TenDVSX";
            this.gridColumn15.MinWidth = 24;
            this.gridColumn15.Name = "gridColumn15";
            this.gridColumn15.OptionsColumn.AllowEdit = false;
            this.gridColumn15.Visible = true;
            this.gridColumn15.VisibleIndex = 1;
            this.gridColumn15.Width = 120;
            // 
            // colSLLapPKL
            // 
            this.colSLLapPKL.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colSLLapPKL.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colSLLapPKL.AppearanceHeader.Options.UseBackColor = true;
            this.colSLLapPKL.AppearanceHeader.Options.UseFont = true;
            this.colSLLapPKL.Caption = "SL lập";
            this.colSLLapPKL.FieldName = "SLDT";
            this.colSLLapPKL.MinWidth = 24;
            this.colSLLapPKL.Name = "colSLLapPKL";
            this.colSLLapPKL.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SLDT", "{0:0.##}")});
            this.colSLLapPKL.Visible = true;
            this.colSLLapPKL.VisibleIndex = 7;
            this.colSLLapPKL.Width = 83;
            // 
            // colSLDaLap
            // 
            this.colSLDaLap.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colSLDaLap.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colSLDaLap.AppearanceHeader.Options.UseBackColor = true;
            this.colSLDaLap.AppearanceHeader.Options.UseFont = true;
            this.colSLDaLap.Caption = "SL đã lập";
            this.colSLDaLap.FieldName = "SLDaLapPKL";
            this.colSLDaLap.MinWidth = 24;
            this.colSLDaLap.Name = "colSLDaLap";
            this.colSLDaLap.OptionsColumn.AllowEdit = false;
            this.colSLDaLap.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SLDaLapPKL", "{0:0.##}")});
            this.colSLDaLap.Visible = true;
            this.colSLDaLap.VisibleIndex = 6;
            this.colSLDaLap.Width = 94;
            // 
            // frmEditLapKHDongThungPCB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 444);
            this.Controls.Add(this.grcDetail);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmEditLapKHDongThungPCB";
            this.Text = "Chỉnh sửa số lượng lập kế hoạch";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridviewDetail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem btnRefresh;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraGrid.GridControl grcDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gridviewDetail;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn colSLKH;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
        private DevExpress.XtraGrid.Columns.GridColumn colSLLapPKL;
        private DevExpress.XtraGrid.Columns.GridColumn colSLDaLap;
        private DevExpress.XtraBars.BarButtonItem btAdd;
        private DevExpress.XtraBars.BarButtonItem btEdit;
        private DevExpress.XtraBars.BarButtonItem btDelete;
        private DevExpress.XtraBars.BarButtonItem btSave;
        private DevExpress.XtraBars.BarButtonItem btRefresh;
        private DevExpress.XtraBars.BarButtonItem btnLapKeHoach;
        private DevExpress.XtraBars.BarButtonItem btnGopThung;
        private DevExpress.XtraBars.BarButtonItem btnAddorEdit;
        private DevExpress.XtraBars.BarButtonItem btnPKLPO;
        private DevExpress.XtraBars.BarButtonItem btnExportPKLToTal;
        private DevExpress.XtraBars.BarSubItem barSubItem1;
        private DevExpress.XtraBars.BarButtonItem btnLapKeHoachN;
        private DevExpress.XtraBars.BarButtonItem btnPKLPON;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
    }
}