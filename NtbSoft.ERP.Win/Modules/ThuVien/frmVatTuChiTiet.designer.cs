
namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    partial class frmVatTuChiTiet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVatTuChiTiet));
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.Them = new DevExpress.XtraBars.BarButtonItem();
            this.Sua = new DevExpress.XtraBars.BarButtonItem();
            this.Xoa = new DevExpress.XtraBars.BarButtonItem();
            this.Luu = new DevExpress.XtraBars.BarButtonItem();
            this.NapLai = new DevExpress.XtraBars.BarButtonItem();
            this.btnImportExcel = new DevExpress.XtraBars.BarButtonItem();
            this.btnXuatExcel = new DevExpress.XtraBars.BarButtonItem();
            this.btnXuatFileMau = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl11 = new DevExpress.XtraBars.BarDockControl();
            this.barButtonItem27 = new DevExpress.XtraBars.BarButtonItem();
            this.barCheckItem2 = new DevExpress.XtraBars.BarCheckItem();
            this.gCVatTuChiTiet = new DevExpress.XtraGrid.GridControl();
            this.gVVatTuChiTiet = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaVT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenVT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaHienThi = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtMaVT = new DevExpress.XtraEditors.TextEdit();
            this.txtTenVatTu = new DevExpress.XtraEditors.TextEdit();
            this.textID = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.Button1 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gCVatTuChiTiet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gVVatTuChiTiet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaVT.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTenVatTu.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textID.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // barAndDockingController1
            // 
            this.barAndDockingController1.LookAndFeel.SkinMaskColor = System.Drawing.Color.Lime;
            this.barAndDockingController1.LookAndFeel.SkinName = "Office 2010 Blue";
            this.barAndDockingController1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2});
            this.barManager1.Controller = this.barAndDockingController1;
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControl11);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.Them,
            this.Sua,
            this.Xoa,
            this.Luu,
            this.NapLai,
            this.barButtonItem27,
            this.barCheckItem2,
            this.btnImportExcel,
            this.btnXuatExcel,
            this.btnXuatFileMau});
            this.barManager1.MaxItemId = 9;
            // 
            // bar2
            // 
            this.bar2.BarName = "Tools";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.Them),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.None, false, this.Sua, false),
            new DevExpress.XtraBars.LinkPersistInfo(this.Xoa),
            new DevExpress.XtraBars.LinkPersistInfo(this.Luu),
            new DevExpress.XtraBars.LinkPersistInfo(this.NapLai),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnImportExcel),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnXuatExcel),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnXuatFileMau)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.RotateWhenVertical = false;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Tools";
            // 
            // Them
            // 
            this.Them.Caption = "Thêm (Ctrl + Shift + N)";
            this.Them.Id = 0;
            this.Them.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Them.ImageOptions.SvgImage")));
            this.Them.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.N));
            this.Them.Name = "Them";
            this.Them.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Them.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Them_ItemClick);
            // 
            // Sua
            // 
            this.Sua.Caption = "Sửa (F2)";
            this.Sua.Enabled = false;
            this.Sua.Id = 1;
            this.Sua.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Sua.ImageOptions.SvgImage")));
            this.Sua.Name = "Sua";
            this.Sua.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Sua.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Sua_ItemClick);
            // 
            // Xoa
            // 
            this.Xoa.Caption = "Xóa (Delete)";
            this.Xoa.Id = 2;
            this.Xoa.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Xoa.ImageOptions.SvgImage")));
            this.Xoa.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.Delete);
            this.Xoa.Name = "Xoa";
            this.Xoa.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Xoa.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Xoa_ItemClick);
            // 
            // Luu
            // 
            this.Luu.Caption = "Lưu (Ctrl + S)";
            this.Luu.Enabled = false;
            this.Luu.Id = 3;
            this.Luu.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Luu.ImageOptions.SvgImage")));
            this.Luu.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.Luu.Name = "Luu";
            this.Luu.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Luu.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Luu_ItemClick);
            // 
            // NapLai
            // 
            this.NapLai.Caption = "Nạp lại (F5)";
            this.NapLai.Id = 4;
            this.NapLai.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("NapLai.ImageOptions.Image")));
            this.NapLai.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("NapLai.ImageOptions.LargeImage")));
            this.NapLai.Name = "NapLai";
            this.NapLai.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.NapLai.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.NapLai_ItemClick);
            // 
            // btnImportExcel
            // 
            this.btnImportExcel.Caption = "Nhập Excel(F6)";
            this.btnImportExcel.Id = 6;
            this.btnImportExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImportExcel.ImageOptions.Image")));
            this.btnImportExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnImportExcel.ImageOptions.LargeImage")));
            this.btnImportExcel.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F6);
            this.btnImportExcel.Name = "btnImportExcel";
            this.btnImportExcel.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnImportExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnImportExcel_ItemClick);
            // 
            // btnXuatExcel
            // 
            this.btnXuatExcel.Caption = "Xuất Excel(F7)";
            this.btnXuatExcel.Enabled = false;
            this.btnXuatExcel.Id = 7;
            this.btnXuatExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnXuatExcel.ImageOptions.Image")));
            this.btnXuatExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnXuatExcel.ImageOptions.LargeImage")));
            this.btnXuatExcel.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F7);
            this.btnXuatExcel.Name = "btnXuatExcel";
            this.btnXuatExcel.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnXuatExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnXuatExcel_ItemClick);
            // 
            // btnXuatFileMau
            // 
            this.btnXuatFileMau.Caption = "Xuất File Mẫu(F8)";
            this.btnXuatFileMau.Id = 8;
            this.btnXuatFileMau.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnXuatFileMau.ImageOptions.Image")));
            this.btnXuatFileMau.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnXuatFileMau.ImageOptions.LargeImage")));
            this.btnXuatFileMau.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F8);
            this.btnXuatFileMau.Name = "btnXuatFileMau";
            this.btnXuatFileMau.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnXuatFileMau.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnXuatFileMau_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(916, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 754);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(916, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 725);
            // 
            // barDockControl11
            // 
            this.barDockControl11.CausesValidation = false;
            this.barDockControl11.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl11.Location = new System.Drawing.Point(916, 29);
            this.barDockControl11.Manager = this.barManager1;
            this.barDockControl11.Size = new System.Drawing.Size(0, 725);
            // 
            // barButtonItem27
            // 
            this.barButtonItem27.Caption = "chose file";
            this.barButtonItem27.Enabled = false;
            this.barButtonItem27.Id = 4;
            this.barButtonItem27.ItemAppearance.Disabled.Font = new System.Drawing.Font("Tahoma", 8.1F);
            this.barButtonItem27.ItemAppearance.Disabled.Options.UseFont = true;
            this.barButtonItem27.ItemAppearance.Hovered.Font = new System.Drawing.Font("Tahoma", 8.1F);
            this.barButtonItem27.ItemAppearance.Hovered.Options.UseFont = true;
            this.barButtonItem27.ItemAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.1F);
            this.barButtonItem27.ItemAppearance.Normal.Options.UseFont = true;
            this.barButtonItem27.ItemAppearance.Pressed.Font = new System.Drawing.Font("Tahoma", 8.1F);
            this.barButtonItem27.ItemAppearance.Pressed.Options.UseFont = true;
            this.barButtonItem27.Name = "barButtonItem27";
            this.barButtonItem27.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barButtonItem27.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem27_ItemClick);
            // 
            // barCheckItem2
            // 
            this.barCheckItem2.Caption = "Nhập/Import";
            this.barCheckItem2.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
            this.barCheckItem2.Id = 5;
            this.barCheckItem2.Name = "barCheckItem2";
            this.barCheckItem2.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // gCVatTuChiTiet
            // 
            this.gCVatTuChiTiet.Location = new System.Drawing.Point(12, 12);
            this.gCVatTuChiTiet.MainView = this.gVVatTuChiTiet;
            this.gCVatTuChiTiet.MenuManager = this.barManager1;
            this.gCVatTuChiTiet.Name = "gCVatTuChiTiet";
            this.gCVatTuChiTiet.Size = new System.Drawing.Size(892, 658);
            this.gCVatTuChiTiet.TabIndex = 4;
            this.gCVatTuChiTiet.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gVVatTuChiTiet});
            this.gCVatTuChiTiet.EditorKeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gCVatTuChiTiet_EditorKeyPress);
            this.gCVatTuChiTiet.Click += new System.EventHandler(this.gridControl1_Click);
            // 
            // gVVatTuChiTiet
            // 
            this.gVVatTuChiTiet.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colID,
            this.colSTT,
            this.colMaVT,
            this.colTenVT,
            this.colMaHienThi});
            this.gVVatTuChiTiet.GridControl = this.gCVatTuChiTiet;
            this.gVVatTuChiTiet.Name = "gVVatTuChiTiet";
            this.gVVatTuChiTiet.OptionsBehavior.Editable = false;
            this.gVVatTuChiTiet.OptionsView.ColumnAutoWidth = false;
            this.gVVatTuChiTiet.OptionsView.ShowAutoFilterRow = true;
            this.gVVatTuChiTiet.OptionsView.ShowGroupPanel = false;
            this.gVVatTuChiTiet.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridNhomNPL_CustomDrawColumnHeader_1);
            this.gVVatTuChiTiet.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gVThuVienKhoSize_FocusedRowChanged);
            this.gVVatTuChiTiet.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gVVatTuChiTiet_CellValueChanged);
            this.gVVatTuChiTiet.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridNhomNPL_CustomUnboundColumnData);
            this.gVVatTuChiTiet.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gVVatTuChiTiet_KeyPress);
            this.gVVatTuChiTiet.Click += new System.EventHandler(this.gridNhomNPL_Click);
            // 
            // colID
            // 
            this.colID.Caption = "ID";
            this.colID.FieldName = "ID";
            this.colID.Name = "colID";
            // 
            // colSTT
            // 
            this.colSTT.Caption = "STT";
            this.colSTT.FieldName = "STT";
            this.colSTT.MinWidth = 21;
            this.colSTT.Name = "colSTT";
            this.colSTT.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.colSTT.Visible = true;
            this.colSTT.VisibleIndex = 0;
            this.colSTT.Width = 37;
            // 
            // colMaVT
            // 
            this.colMaVT.AppearanceHeader.Options.UseTextOptions = true;
            this.colMaVT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaVT.Caption = "Mã vật tư";
            this.colMaVT.FieldName = "MaVT";
            this.colMaVT.Name = "colMaVT";
            this.colMaVT.OptionsColumn.ReadOnly = true;
            this.colMaVT.Width = 77;
            // 
            // colTenVT
            // 
            this.colTenVT.AppearanceHeader.Options.UseTextOptions = true;
            this.colTenVT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colTenVT.Caption = "Tên vật tư";
            this.colTenVT.FieldName = "TenVT";
            this.colTenVT.MinWidth = 21;
            this.colTenVT.Name = "colTenVT";
            this.colTenVT.Visible = true;
            this.colTenVT.VisibleIndex = 2;
            this.colTenVT.Width = 341;
            // 
            // colMaHienThi
            // 
            this.colMaHienThi.Caption = "Mã hiển thị";
            this.colMaHienThi.FieldName = "MaHienThi";
            this.colMaHienThi.MinWidth = 21;
            this.colMaHienThi.Name = "colMaHienThi";
            this.colMaHienThi.Visible = true;
            this.colMaHienThi.VisibleIndex = 1;
            this.colMaHienThi.Width = 135;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtMaVT);
            this.layoutControl1.Controls.Add(this.txtTenVatTu);
            this.layoutControl1.Controls.Add(this.textID);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.layoutControl1.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem7});
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-1449, 168, 896, 809);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(916, 43);
            this.layoutControl1.TabIndex = 15;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtMaVT
            // 
            this.txtMaVT.Location = new System.Drawing.Point(91, 12);
            this.txtMaVT.Name = "txtMaVT";
            this.txtMaVT.Properties.ReadOnly = true;
            this.txtMaVT.Size = new System.Drawing.Size(211, 20);
            this.txtMaVT.StyleController = this.layoutControl1;
            this.txtMaVT.TabIndex = 4;
            // 
            // txtTenVatTu
            // 
            this.txtTenVatTu.Location = new System.Drawing.Point(385, 12);
            this.txtTenVatTu.Name = "txtTenVatTu";
            this.txtTenVatTu.Properties.ReadOnly = true;
            this.txtTenVatTu.Size = new System.Drawing.Size(175, 20);
            this.txtTenVatTu.StyleController = this.layoutControl1;
            this.txtTenVatTu.TabIndex = 5;
            // 
            // textID
            // 
            this.textID.Location = new System.Drawing.Point(294, 36);
            this.textID.Name = "textID";
            this.textID.Size = new System.Drawing.Size(86, 20);
            this.textID.StyleController = this.layoutControl1;
            this.textID.TabIndex = 7;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.textID;
            this.layoutControlItem7.CustomizationFormText = "layoutControlItem4";
            this.layoutControlItem7.Location = new System.Drawing.Point(185, 24);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(187, 76);
            this.layoutControlItem7.Text = "layoutControlItem4";
            this.layoutControlItem7.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItem7.TextSize = new System.Drawing.Size(93, 13);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(899, 44);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AllowHtmlStringInCaption = true;
            this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem2.Control = this.txtMaVT;
            this.layoutControlItem2.CustomizationFormText = "Mã vật tư";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(294, 24);
            this.layoutControlItem2.Text = "Mã hiển thị";
            this.layoutControlItem2.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(76, 13);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AllowHtmlStringInCaption = true;
            this.layoutControlItem3.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.layoutControlItem3.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutControlItem3.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem3.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem3.Control = this.txtTenVatTu;
            this.layoutControlItem3.CustomizationFormText = "Khổ/Size";
            this.layoutControlItem3.Location = new System.Drawing.Point(294, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(258, 24);
            this.layoutControlItem3.Text = "Tên vật tư<color=Red>(*)</color>";
            this.layoutControlItem3.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(76, 13);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(552, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(327, 24);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.gCVatTuChiTiet);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 72);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup1;
            this.layoutControl2.Size = new System.Drawing.Size(916, 682);
            this.layoutControl2.TabIndex = 16;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(916, 682);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gCVatTuChiTiet;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(896, 662);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(829, 0);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(75, 23);
            this.Button1.TabIndex = 21;
            this.Button1.Text = "simpleButton1";
            this.Button1.Visible = false;
            // 
            // frmVatTuChiTiet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 754);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.layoutControl2);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControl11);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmVatTuChiTiet";
            this.Text = "Vật tư chi tiết";
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gCVatTuChiTiet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gVVatTuChiTiet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtMaVT.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTenVatTu.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textID.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem Them;
        private DevExpress.XtraBars.BarButtonItem Sua;
        private DevExpress.XtraBars.BarButtonItem Xoa;
        private DevExpress.XtraBars.BarButtonItem Luu;
        private DevExpress.XtraBars.BarButtonItem NapLai;
        private DevExpress.XtraBars.BarButtonItem barButtonItem27;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControl11;
        private DevExpress.XtraBars.BarCheckItem barCheckItem2;
        private DevExpress.XtraGrid.GridControl gCVatTuChiTiet;
        private DevExpress.XtraGrid.Views.Grid.GridView gVVatTuChiTiet;
        private DevExpress.XtraGrid.Columns.GridColumn colID;
        private DevExpress.XtraGrid.Columns.GridColumn colMaVT;
        private DevExpress.XtraBars.BarButtonItem btnImportExcel;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit txtMaVT;
        private DevExpress.XtraEditors.TextEdit txtTenVatTu;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.TextEdit textID;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraBars.BarButtonItem btnXuatExcel;
        private DevExpress.XtraBars.BarButtonItem btnXuatFileMau;
        private DevExpress.XtraGrid.Columns.GridColumn colTenVT;
        private DevExpress.XtraGrid.Columns.GridColumn colSTT;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Columns.GridColumn colMaHienThi;
        private DevExpress.XtraEditors.SimpleButton Button1;
    }
}