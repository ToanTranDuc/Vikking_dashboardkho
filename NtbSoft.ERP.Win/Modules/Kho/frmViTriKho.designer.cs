
namespace NtbSoft.ERP.Win.Modules.ViTriKho
{
    partial class frmViTriKho
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmViTriKho));
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.btnThem = new DevExpress.XtraBars.BarButtonItem();
            this.btnLuu = new DevExpress.XtraBars.BarButtonItem();
            this.btnXoa = new DevExpress.XtraBars.BarButtonItem();
            this.btnImportExcel = new DevExpress.XtraBars.BarButtonItem();
            this.btnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.btnQrCode = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
            this.tlKho = new DevExpress.XtraTreeList.TreeList();
            this.tlcolTenKho = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tlcolMaKho = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tlcolSTT = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repoColChon = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.tlViTriKho = new DevExpress.XtraTreeList.TreeList();
            this.tlcolTenVT = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tlcolCBM = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tlcolGhiChu = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tlcolMaVT = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgrViTriKho = new DevExpress.XtraGrid.GridControl();
            this.gridViewViTriKho = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMaKho = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaVT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenVT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colParentMaVT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCBM = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDai = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoTxtNumber = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.colRong = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCao = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGhiChu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNguyenLieu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoCheckNL = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.colPhuLieu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoCheckPL = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repoCboParentMaVT = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).BeginInit();
            this.splitContainerControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tlKho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoColChon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tlViTriKho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrViTriKho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewViTriKho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoTxtNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheckNL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheckPL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCboParentMaVT)).BeginInit();
            this.SuspendLayout();
            // 
            // barAndDockingController1
            // 
            this.barAndDockingController1.LookAndFeel.SkinMaskColor = System.Drawing.Color.DodgerBlue;
            this.barAndDockingController1.LookAndFeel.SkinName = "Office 2010 Blue";
            this.barAndDockingController1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.Controller = this.barAndDockingController1;
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnThem,
            this.btnLuu,
            this.btnXoa,
            this.btnRefresh,
            this.barButtonItem1,
            this.btnQrCode,
            this.btnImportExcel,
            this.barButtonItem3});
            this.barManager1.MaxItemId = 16;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnThem),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnLuu),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnXoa),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnImportExcel),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnRefresh),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnQrCode)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // btnThem
            // 
            this.btnThem.Caption = "Thêm";
            this.btnThem.Id = 0;
            this.btnThem.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnThem.ImageOptions.Image")));
            this.btnThem.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnThem.ImageOptions.LargeImage")));
            this.btnThem.Name = "btnThem";
            this.btnThem.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnThem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnThem_ItemClick);
            // 
            // btnLuu
            // 
            this.btnLuu.Caption = "Lưu";
            this.btnLuu.Id = 1;
            this.btnLuu.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnLuu.ImageOptions.Image")));
            this.btnLuu.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnLuu.ImageOptions.LargeImage")));
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnLuu.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnLuu_ItemClick);
            // 
            // btnXoa
            // 
            this.btnXoa.Caption = "Xóa";
            this.btnXoa.Id = 2;
            this.btnXoa.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnXoa.ImageOptions.Image")));
            this.btnXoa.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnXoa.ImageOptions.LargeImage")));
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnXoa.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnXoa_ItemClick);
            // 
            // btnImportExcel
            // 
            this.btnImportExcel.Caption = "Excel";
            this.btnImportExcel.Id = 14;
            this.btnImportExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImportExcel.ImageOptions.Image")));
            this.btnImportExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnImportExcel.ImageOptions.LargeImage")));
            this.btnImportExcel.Name = "btnImportExcel";
            this.btnImportExcel.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnImportExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnImportExcel_ItemClick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Caption = "Làm Mới";
            this.btnRefresh.Id = 4;
            this.btnRefresh.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.ImageOptions.Image")));
            this.btnRefresh.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnRefresh.ImageOptions.LargeImage")));
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRefresh_ItemClick);
            // 
            // btnQrCode
            // 
            this.btnQrCode.Caption = "QR Code";
            this.btnQrCode.Id = 8;
            this.btnQrCode.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnQrCode.ImageOptions.Image")));
            this.btnQrCode.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnQrCode.ImageOptions.LargeImage")));
            this.btnQrCode.Name = "btnQrCode";
            this.btnQrCode.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnQrCode.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnQrCode_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlTop.Size = new System.Drawing.Size(1967, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 684);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(1967, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 648);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1967, 36);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 648);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Id = 7;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // barButtonItem3
            // 
            this.barButtonItem3.Caption = "barButtonItem3";
            this.barButtonItem3.Id = 15;
            this.barButtonItem3.Name = "barButtonItem3";
            // 
            // splitContainerControl2
            // 
            this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl2.Location = new System.Drawing.Point(0, 36);
            this.splitContainerControl2.LookAndFeel.SkinName = "Office 2010 Blue";
            this.splitContainerControl2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.splitContainerControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainerControl2.Name = "splitContainerControl2";
            this.splitContainerControl2.Panel1.Controls.Add(this.tlKho);
            this.splitContainerControl2.Panel1.Text = "Panel1";
            this.splitContainerControl2.Panel2.Controls.Add(this.tlViTriKho);
            this.splitContainerControl2.Panel2.Controls.Add(this.groupBox1);
            this.splitContainerControl2.Panel2.Text = "Panel2";
            this.splitContainerControl2.Size = new System.Drawing.Size(1967, 648);
            this.splitContainerControl2.SplitterPosition = 520;
            this.splitContainerControl2.TabIndex = 6;
            // 
            // tlKho
            // 
            this.tlKho.Appearance.EvenRow.BackColor = System.Drawing.Color.White;
            this.tlKho.Appearance.EvenRow.Options.UseBackColor = true;
            this.tlKho.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(232)))), ((int)(((byte)(255)))));
            this.tlKho.Appearance.FocusedCell.Options.UseBackColor = true;
            this.tlKho.Appearance.OddRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.tlKho.Appearance.OddRow.Options.UseBackColor = true;
            this.tlKho.ColumnPanelRowHeight = 37;
            this.tlKho.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.tlcolTenKho,
            this.tlcolMaKho,
            this.tlcolSTT});
            this.tlKho.Cursor = System.Windows.Forms.Cursors.Default;
            this.tlKho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlKho.Location = new System.Drawing.Point(0, 0);
            this.tlKho.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tlKho.MinWidth = 23;
            this.tlKho.Name = "tlKho";
            this.tlKho.OptionsView.EnableAppearanceOddRow = true;
            this.tlKho.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoColChon});
            this.tlKho.RowHeight = 31;
            this.tlKho.Size = new System.Drawing.Size(520, 648);
            this.tlKho.TabIndex = 1;
            this.tlKho.TreeLevelWidth = 21;
            this.tlKho.CustomUnboundColumnData += new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.tlKho_CustomUnboundColumnData);
            this.tlKho.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tlKho_FocusedNodeChanged);
            // 
            // tlcolTenKho
            // 
            this.tlcolTenKho.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(109)))), ((int)(((byte)(177)))));
            this.tlcolTenKho.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tlcolTenKho.AppearanceHeader.Options.UseBackColor = true;
            this.tlcolTenKho.AppearanceHeader.Options.UseFont = true;
            this.tlcolTenKho.AppearanceHeader.Options.UseTextOptions = true;
            this.tlcolTenKho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tlcolTenKho.Caption = "Tên kho";
            this.tlcolTenKho.FieldName = "TenKho";
            this.tlcolTenKho.MaxWidth = 350;
            this.tlcolTenKho.MinWidth = 23;
            this.tlcolTenKho.Name = "tlcolTenKho";
            this.tlcolTenKho.OptionsColumn.AllowEdit = false;
            this.tlcolTenKho.Visible = true;
            this.tlcolTenKho.VisibleIndex = 2;
            this.tlcolTenKho.Width = 304;
            // 
            // tlcolMaKho
            // 
            this.tlcolMaKho.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(109)))), ((int)(((byte)(177)))));
            this.tlcolMaKho.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tlcolMaKho.AppearanceHeader.Options.UseBackColor = true;
            this.tlcolMaKho.AppearanceHeader.Options.UseFont = true;
            this.tlcolMaKho.AppearanceHeader.Options.UseTextOptions = true;
            this.tlcolMaKho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tlcolMaKho.Caption = "Mã kho";
            this.tlcolMaKho.FieldName = "Id";
            this.tlcolMaKho.MinWidth = 23;
            this.tlcolMaKho.Name = "tlcolMaKho";
            this.tlcolMaKho.OptionsColumn.AllowEdit = false;
            this.tlcolMaKho.Visible = true;
            this.tlcolMaKho.VisibleIndex = 1;
            this.tlcolMaKho.Width = 139;
            // 
            // tlcolSTT
            // 
            this.tlcolSTT.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tlcolSTT.AppearanceCell.Options.UseFont = true;
            this.tlcolSTT.AppearanceCell.Options.UseTextOptions = true;
            this.tlcolSTT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tlcolSTT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(109)))), ((int)(((byte)(177)))));
            this.tlcolSTT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tlcolSTT.AppearanceHeader.Options.UseBackColor = true;
            this.tlcolSTT.AppearanceHeader.Options.UseFont = true;
            this.tlcolSTT.AppearanceHeader.Options.UseTextOptions = true;
            this.tlcolSTT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tlcolSTT.Caption = "STT";
            this.tlcolSTT.FieldName = "STT";
            this.tlcolSTT.MinWidth = 23;
            this.tlcolSTT.Name = "tlcolSTT";
            this.tlcolSTT.OptionsColumn.AllowEdit = false;
            this.tlcolSTT.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Integer;
            this.tlcolSTT.Visible = true;
            this.tlcolSTT.VisibleIndex = 0;
            this.tlcolSTT.Width = 56;
            // 
            // repoColChon
            // 
            this.repoColChon.AutoHeight = false;
            this.repoColChon.Name = "repoColChon";
            this.repoColChon.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // tlViTriKho
            // 
            this.tlViTriKho.Appearance.EvenRow.BackColor = System.Drawing.Color.White;
            this.tlViTriKho.Appearance.EvenRow.Options.UseBackColor = true;
            this.tlViTriKho.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.tlViTriKho.Appearance.FocusedCell.Options.UseBackColor = true;
            this.tlViTriKho.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.tlViTriKho.Appearance.FocusedRow.Options.UseBackColor = true;
            this.tlViTriKho.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.tlViTriKho.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.tlViTriKho.Appearance.OddRow.BackColor = System.Drawing.Color.White;
            this.tlViTriKho.Appearance.OddRow.Options.UseBackColor = true;
            this.tlViTriKho.ColumnPanelRowHeight = 37;
            this.tlViTriKho.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.tlcolTenVT,
            this.tlcolCBM,
            this.tlcolGhiChu,
            this.tlcolMaVT});
            this.tlViTriKho.Cursor = System.Windows.Forms.Cursors.Default;
            this.tlViTriKho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlViTriKho.KeyFieldName = "MaVT";
            this.tlViTriKho.Location = new System.Drawing.Point(0, 102);
            this.tlViTriKho.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tlViTriKho.MinWidth = 23;
            this.tlViTriKho.Name = "tlViTriKho";
            this.tlViTriKho.OptionsView.EnableAppearanceOddRow = true;
            this.tlViTriKho.ParentFieldName = "ParentMaVT";
            this.tlViTriKho.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.tlViTriKho.RowHeight = 31;
            this.tlViTriKho.Size = new System.Drawing.Size(1440, 546);
            this.tlViTriKho.TabIndex = 2;
            this.tlViTriKho.TreeLevelWidth = 21;
            this.tlViTriKho.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tlViTriKho_FocusedNodeChanged);
            this.tlViTriKho.CustomDrawRow += new DevExpress.XtraTreeList.CustomDrawRowEventHandler(this.tlViTriKho_CustomDrawRow);
            // 
            // tlcolTenVT
            // 
            this.tlcolTenVT.AppearanceCell.Options.UseBackColor = true;
            this.tlcolTenVT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(109)))), ((int)(((byte)(177)))));
            this.tlcolTenVT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tlcolTenVT.AppearanceHeader.Options.UseBackColor = true;
            this.tlcolTenVT.AppearanceHeader.Options.UseFont = true;
            this.tlcolTenVT.AppearanceHeader.Options.UseTextOptions = true;
            this.tlcolTenVT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tlcolTenVT.Caption = "Tên vị trí";
            this.tlcolTenVT.FieldName = "TenVT";
            this.tlcolTenVT.MaxWidth = 350;
            this.tlcolTenVT.MinWidth = 23;
            this.tlcolTenVT.Name = "tlcolTenVT";
            this.tlcolTenVT.OptionsColumn.AllowEdit = false;
            this.tlcolTenVT.Visible = true;
            this.tlcolTenVT.VisibleIndex = 1;
            this.tlcolTenVT.Width = 350;
            // 
            // tlcolCBM
            // 
            this.tlcolCBM.AppearanceCell.Options.UseBackColor = true;
            this.tlcolCBM.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(109)))), ((int)(((byte)(177)))));
            this.tlcolCBM.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tlcolCBM.AppearanceHeader.Options.UseBackColor = true;
            this.tlcolCBM.AppearanceHeader.Options.UseFont = true;
            this.tlcolCBM.AppearanceHeader.Options.UseTextOptions = true;
            this.tlcolCBM.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tlcolCBM.Caption = "CBM";
            this.tlcolCBM.FieldName = "CBM";
            this.tlcolCBM.MaxWidth = 117;
            this.tlcolCBM.MinWidth = 23;
            this.tlcolCBM.Name = "tlcolCBM";
            this.tlcolCBM.OptionsColumn.AllowEdit = false;
            this.tlcolCBM.Visible = true;
            this.tlcolCBM.VisibleIndex = 2;
            this.tlcolCBM.Width = 117;
            // 
            // tlcolGhiChu
            // 
            this.tlcolGhiChu.AppearanceCell.Options.UseBackColor = true;
            this.tlcolGhiChu.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(109)))), ((int)(((byte)(177)))));
            this.tlcolGhiChu.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tlcolGhiChu.AppearanceHeader.Options.UseBackColor = true;
            this.tlcolGhiChu.AppearanceHeader.Options.UseFont = true;
            this.tlcolGhiChu.AppearanceHeader.Options.UseTextOptions = true;
            this.tlcolGhiChu.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tlcolGhiChu.Caption = "Ghi chú";
            this.tlcolGhiChu.FieldName = "GhiChu";
            this.tlcolGhiChu.MaxWidth = 350;
            this.tlcolGhiChu.MinWidth = 23;
            this.tlcolGhiChu.Name = "tlcolGhiChu";
            this.tlcolGhiChu.OptionsColumn.AllowEdit = false;
            this.tlcolGhiChu.Visible = true;
            this.tlcolGhiChu.VisibleIndex = 3;
            this.tlcolGhiChu.Width = 302;
            // 
            // tlcolMaVT
            // 
            this.tlcolMaVT.AppearanceCell.Options.UseBackColor = true;
            this.tlcolMaVT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(109)))), ((int)(((byte)(177)))));
            this.tlcolMaVT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tlcolMaVT.AppearanceHeader.Options.UseBackColor = true;
            this.tlcolMaVT.AppearanceHeader.Options.UseFont = true;
            this.tlcolMaVT.AppearanceHeader.Options.UseTextOptions = true;
            this.tlcolMaVT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.tlcolMaVT.Caption = "Mã vị trí";
            this.tlcolMaVT.FieldName = "MaVT";
            this.tlcolMaVT.MaxWidth = 233;
            this.tlcolMaVT.MinWidth = 23;
            this.tlcolMaVT.Name = "tlcolMaVT";
            this.tlcolMaVT.OptionsColumn.AllowEdit = false;
            this.tlcolMaVT.Visible = true;
            this.tlcolMaVT.VisibleIndex = 0;
            this.tlcolMaVT.Width = 233;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgrViTriKho);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(1440, 102);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // dgrViTriKho
            // 
            this.dgrViTriKho.Cursor = System.Windows.Forms.Cursors.Default;
            this.dgrViTriKho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgrViTriKho.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgrViTriKho.Location = new System.Drawing.Point(3, 20);
            this.dgrViTriKho.LookAndFeel.SkinName = "Office 2010 Blue";
            this.dgrViTriKho.LookAndFeel.UseDefaultLookAndFeel = false;
            this.dgrViTriKho.MainView = this.gridViewViTriKho;
            this.dgrViTriKho.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgrViTriKho.MenuManager = this.barManager1;
            this.dgrViTriKho.Name = "dgrViTriKho";
            this.dgrViTriKho.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoCboParentMaVT,
            this.repoTxtNumber,
            this.repoCheckPL,
            this.repoCheckNL});
            this.dgrViTriKho.Size = new System.Drawing.Size(1434, 78);
            this.dgrViTriKho.TabIndex = 0;
            this.dgrViTriKho.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewViTriKho});
            // 
            // gridViewViTriKho
            // 
            this.gridViewViTriKho.Appearance.FocusedRow.BackColor = System.Drawing.Color.Transparent;
            this.gridViewViTriKho.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewViTriKho.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.Transparent;
            this.gridViewViTriKho.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gridViewViTriKho.ColumnPanelRowHeight = 37;
            this.gridViewViTriKho.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMaKho,
            this.colMaVT,
            this.colTenVT,
            this.colParentMaVT,
            this.colCBM,
            this.colDai,
            this.colRong,
            this.colCao,
            this.colGhiChu,
            this.colNguyenLieu,
            this.colPhuLieu});
            this.gridViewViTriKho.DetailHeight = 431;
            this.gridViewViTriKho.GridControl = this.dgrViTriKho;
            this.gridViewViTriKho.Name = "gridViewViTriKho";
            this.gridViewViTriKho.OptionsView.ShowGroupPanel = false;
            this.gridViewViTriKho.RowHeight = 37;
            this.gridViewViTriKho.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewViTriKho_CellValueChanged);
            this.gridViewViTriKho.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewViTriKho_CustomColumnDisplayText);
            // 
            // colMaKho
            // 
            this.colMaKho.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.colMaKho.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.colMaKho.AppearanceCell.Options.UseBackColor = true;
            this.colMaKho.AppearanceCell.Options.UseFont = true;
            this.colMaKho.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(117)))), ((int)(((byte)(181)))));
            this.colMaKho.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colMaKho.AppearanceHeader.Options.UseBackColor = true;
            this.colMaKho.AppearanceHeader.Options.UseFont = true;
            this.colMaKho.AppearanceHeader.Options.UseTextOptions = true;
            this.colMaKho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaKho.Caption = "Tên kho";
            this.colMaKho.FieldName = "MaKho";
            this.colMaKho.MinWidth = 23;
            this.colMaKho.Name = "colMaKho";
            this.colMaKho.OptionsColumn.AllowEdit = false;
            this.colMaKho.Visible = true;
            this.colMaKho.VisibleIndex = 0;
            this.colMaKho.Width = 87;
            // 
            // colMaVT
            // 
            this.colMaVT.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.colMaVT.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.colMaVT.AppearanceCell.Options.UseBackColor = true;
            this.colMaVT.AppearanceCell.Options.UseFont = true;
            this.colMaVT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(117)))), ((int)(((byte)(181)))));
            this.colMaVT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colMaVT.AppearanceHeader.Options.UseBackColor = true;
            this.colMaVT.AppearanceHeader.Options.UseFont = true;
            this.colMaVT.AppearanceHeader.Options.UseTextOptions = true;
            this.colMaVT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaVT.Caption = "Mã vị trí";
            this.colMaVT.FieldName = "MaVT";
            this.colMaVT.MinWidth = 23;
            this.colMaVT.Name = "colMaVT";
            this.colMaVT.OptionsColumn.AllowEdit = false;
            this.colMaVT.Visible = true;
            this.colMaVT.VisibleIndex = 2;
            this.colMaVT.Width = 87;
            // 
            // colTenVT
            // 
            this.colTenVT.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.colTenVT.AppearanceCell.Options.UseFont = true;
            this.colTenVT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(117)))), ((int)(((byte)(181)))));
            this.colTenVT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colTenVT.AppearanceHeader.Options.UseBackColor = true;
            this.colTenVT.AppearanceHeader.Options.UseFont = true;
            this.colTenVT.AppearanceHeader.Options.UseTextOptions = true;
            this.colTenVT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colTenVT.Caption = "Tên vị trí";
            this.colTenVT.FieldName = "TenVT";
            this.colTenVT.MinWidth = 175;
            this.colTenVT.Name = "colTenVT";
            this.colTenVT.Visible = true;
            this.colTenVT.VisibleIndex = 3;
            this.colTenVT.Width = 175;
            // 
            // colParentMaVT
            // 
            this.colParentMaVT.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.colParentMaVT.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.colParentMaVT.AppearanceCell.Options.UseBackColor = true;
            this.colParentMaVT.AppearanceCell.Options.UseFont = true;
            this.colParentMaVT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(117)))), ((int)(((byte)(181)))));
            this.colParentMaVT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colParentMaVT.AppearanceHeader.Options.UseBackColor = true;
            this.colParentMaVT.AppearanceHeader.Options.UseFont = true;
            this.colParentMaVT.AppearanceHeader.Options.UseTextOptions = true;
            this.colParentMaVT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colParentMaVT.Caption = "VT trực thuộc";
            this.colParentMaVT.FieldName = "ParentMaVT";
            this.colParentMaVT.MinWidth = 23;
            this.colParentMaVT.Name = "colParentMaVT";
            this.colParentMaVT.OptionsColumn.AllowEdit = false;
            this.colParentMaVT.Visible = true;
            this.colParentMaVT.VisibleIndex = 1;
            this.colParentMaVT.Width = 87;
            // 
            // colCBM
            // 
            this.colCBM.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(254)))), ((int)(((byte)(255)))));
            this.colCBM.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.colCBM.AppearanceCell.Options.UseBackColor = true;
            this.colCBM.AppearanceCell.Options.UseFont = true;
            this.colCBM.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(117)))), ((int)(((byte)(181)))));
            this.colCBM.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colCBM.AppearanceHeader.Options.UseBackColor = true;
            this.colCBM.AppearanceHeader.Options.UseFont = true;
            this.colCBM.AppearanceHeader.Options.UseTextOptions = true;
            this.colCBM.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colCBM.Caption = "CBM";
            this.colCBM.FieldName = "CBM";
            this.colCBM.MaxWidth = 87;
            this.colCBM.MinWidth = 23;
            this.colCBM.Name = "colCBM";
            this.colCBM.OptionsColumn.AllowEdit = false;
            this.colCBM.Visible = true;
            this.colCBM.VisibleIndex = 7;
            this.colCBM.Width = 64;
            // 
            // colDai
            // 
            this.colDai.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.colDai.AppearanceCell.Options.UseFont = true;
            this.colDai.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(117)))), ((int)(((byte)(181)))));
            this.colDai.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colDai.AppearanceHeader.Options.UseBackColor = true;
            this.colDai.AppearanceHeader.Options.UseFont = true;
            this.colDai.AppearanceHeader.Options.UseTextOptions = true;
            this.colDai.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colDai.Caption = "Chiều dài";
            this.colDai.ColumnEdit = this.repoTxtNumber;
            this.colDai.FieldName = "Dai";
            this.colDai.MaxWidth = 87;
            this.colDai.MinWidth = 23;
            this.colDai.Name = "colDai";
            this.colDai.Visible = true;
            this.colDai.VisibleIndex = 4;
            this.colDai.Width = 87;
            // 
            // repoTxtNumber
            // 
            this.repoTxtNumber.AutoHeight = false;
            this.repoTxtNumber.Mask.EditMask = "n2";
            this.repoTxtNumber.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.repoTxtNumber.Name = "repoTxtNumber";
            // 
            // colRong
            // 
            this.colRong.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.colRong.AppearanceCell.Options.UseFont = true;
            this.colRong.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(117)))), ((int)(((byte)(181)))));
            this.colRong.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colRong.AppearanceHeader.Options.UseBackColor = true;
            this.colRong.AppearanceHeader.Options.UseFont = true;
            this.colRong.AppearanceHeader.Options.UseTextOptions = true;
            this.colRong.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colRong.Caption = "Chiều rộng";
            this.colRong.ColumnEdit = this.repoTxtNumber;
            this.colRong.FieldName = "Rong";
            this.colRong.MaxWidth = 87;
            this.colRong.MinWidth = 23;
            this.colRong.Name = "colRong";
            this.colRong.Visible = true;
            this.colRong.VisibleIndex = 5;
            this.colRong.Width = 87;
            // 
            // colCao
            // 
            this.colCao.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.colCao.AppearanceCell.Options.UseFont = true;
            this.colCao.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(117)))), ((int)(((byte)(181)))));
            this.colCao.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colCao.AppearanceHeader.Options.UseBackColor = true;
            this.colCao.AppearanceHeader.Options.UseFont = true;
            this.colCao.AppearanceHeader.Options.UseTextOptions = true;
            this.colCao.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colCao.Caption = "Chiều cao";
            this.colCao.ColumnEdit = this.repoTxtNumber;
            this.colCao.FieldName = "Cao";
            this.colCao.MaxWidth = 87;
            this.colCao.MinWidth = 23;
            this.colCao.Name = "colCao";
            this.colCao.Visible = true;
            this.colCao.VisibleIndex = 6;
            this.colCao.Width = 87;
            // 
            // colGhiChu
            // 
            this.colGhiChu.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.colGhiChu.AppearanceCell.Options.UseFont = true;
            this.colGhiChu.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(117)))), ((int)(((byte)(181)))));
            this.colGhiChu.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colGhiChu.AppearanceHeader.Options.UseBackColor = true;
            this.colGhiChu.AppearanceHeader.Options.UseFont = true;
            this.colGhiChu.AppearanceHeader.Options.UseTextOptions = true;
            this.colGhiChu.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colGhiChu.Caption = "Ghi chú";
            this.colGhiChu.FieldName = "GhiChu";
            this.colGhiChu.MinWidth = 23;
            this.colGhiChu.Name = "colGhiChu";
            this.colGhiChu.Visible = true;
            this.colGhiChu.VisibleIndex = 10;
            this.colGhiChu.Width = 87;
            // 
            // colNguyenLieu
            // 
            this.colNguyenLieu.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(117)))), ((int)(((byte)(181)))));
            this.colNguyenLieu.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colNguyenLieu.AppearanceHeader.Options.UseBackColor = true;
            this.colNguyenLieu.AppearanceHeader.Options.UseFont = true;
            this.colNguyenLieu.AppearanceHeader.Options.UseTextOptions = true;
            this.colNguyenLieu.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colNguyenLieu.Caption = "Nguyên liệu";
            this.colNguyenLieu.ColumnEdit = this.repoCheckNL;
            this.colNguyenLieu.FieldName = "NguyenLieu";
            this.colNguyenLieu.MaxWidth = 87;
            this.colNguyenLieu.MinWidth = 23;
            this.colNguyenLieu.Name = "colNguyenLieu";
            this.colNguyenLieu.Visible = true;
            this.colNguyenLieu.VisibleIndex = 8;
            this.colNguyenLieu.Width = 82;
            // 
            // repoCheckNL
            // 
            this.repoCheckNL.AutoHeight = false;
            this.repoCheckNL.Name = "repoCheckNL";
            this.repoCheckNL.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repoCheckNL.CheckedChanged += new System.EventHandler(this.repoCheckNL_CheckedChanged);
            // 
            // colPhuLieu
            // 
            this.colPhuLieu.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(117)))), ((int)(((byte)(181)))));
            this.colPhuLieu.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colPhuLieu.AppearanceHeader.Options.UseBackColor = true;
            this.colPhuLieu.AppearanceHeader.Options.UseFont = true;
            this.colPhuLieu.AppearanceHeader.Options.UseTextOptions = true;
            this.colPhuLieu.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colPhuLieu.Caption = "Phụ liệu";
            this.colPhuLieu.ColumnEdit = this.repoCheckPL;
            this.colPhuLieu.FieldName = "PhuLieu";
            this.colPhuLieu.MaxWidth = 87;
            this.colPhuLieu.MinWidth = 23;
            this.colPhuLieu.Name = "colPhuLieu";
            this.colPhuLieu.Visible = true;
            this.colPhuLieu.VisibleIndex = 9;
            this.colPhuLieu.Width = 87;
            // 
            // repoCheckPL
            // 
            this.repoCheckPL.AutoHeight = false;
            this.repoCheckPL.Name = "repoCheckPL";
            this.repoCheckPL.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repoCheckPL.CheckedChanged += new System.EventHandler(this.repoCheckPL_CheckedChanged);
            // 
            // repoCboParentMaVT
            // 
            this.repoCboParentMaVT.AutoHeight = false;
            this.repoCboParentMaVT.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoCboParentMaVT.Name = "repoCboParentMaVT";
            this.repoCboParentMaVT.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // frmViTriKho
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1967, 684);
            this.Controls.Add(this.splitContainerControl2);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmViTriKho";
            this.Text = "Vị trí kho";
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).EndInit();
            this.splitContainerControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tlKho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoColChon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tlViTriKho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrViTriKho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewViTriKho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoTxtNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheckNL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCheckPL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoCboParentMaVT)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem btnThem;
        private DevExpress.XtraBars.BarButtonItem btnLuu;
        private DevExpress.XtraBars.BarButtonItem btnXoa;
        private DevExpress.XtraBars.BarButtonItem btnImportExcel;
        private DevExpress.XtraBars.BarButtonItem btnRefresh;
        private DevExpress.XtraBars.BarButtonItem btnQrCode;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl2;
        private DevExpress.XtraTreeList.TreeList tlKho;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlcolTenKho;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlcolMaKho;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repoColChon;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlcolSTT;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraGrid.GridControl dgrViTriKho;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewViTriKho;
        private DevExpress.XtraGrid.Columns.GridColumn colMaKho;
        private DevExpress.XtraGrid.Columns.GridColumn colMaVT;
        private DevExpress.XtraGrid.Columns.GridColumn colTenVT;
        private DevExpress.XtraGrid.Columns.GridColumn colParentMaVT;
        private DevExpress.XtraGrid.Columns.GridColumn colCBM;
        private DevExpress.XtraGrid.Columns.GridColumn colDai;
        private DevExpress.XtraGrid.Columns.GridColumn colRong;
        private DevExpress.XtraGrid.Columns.GridColumn colCao;
        private DevExpress.XtraGrid.Columns.GridColumn colGhiChu;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repoCboParentMaVT;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repoTxtNumber;
        private DevExpress.XtraTreeList.TreeList tlViTriKho;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlcolTenVT;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlcolCBM;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlcolGhiChu;
        private DevExpress.XtraTreeList.Columns.TreeListColumn tlcolMaVT;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn colNguyenLieu;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repoCheckPL;
        private DevExpress.XtraGrid.Columns.GridColumn colPhuLieu;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repoCheckNL;
    }
}