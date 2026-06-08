
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    partial class frmDonViSanXuat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDonViSanXuat));
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.Them = new DevExpress.XtraBars.BarButtonItem();
            this.Sua = new DevExpress.XtraBars.BarButtonItem();
            this.Xoa = new DevExpress.XtraBars.BarButtonItem();
            this.Luu = new DevExpress.XtraBars.BarButtonItem();
            this.Naplai = new DevExpress.XtraBars.BarButtonItem();
            this.btnQLKho = new DevExpress.XtraBars.BarButtonItem();
            this.btnSaveKho = new DevExpress.XtraBars.BarButtonItem();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.gridDonViSanXuat = new DevExpress.XtraGrid.GridControl();
            this.gridViewDonViSanXuat = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaDVSX = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenDVSX = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGiaCong = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CheckGiaCong = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.colSort = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTrangThai = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaHienThi = new DevExpress.XtraGrid.Columns.GridColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.pnlQLKho = new DevExpress.XtraEditors.PanelControl();
            this.grcQLKho = new DevExpress.XtraGrid.GridControl();
            this.gridViewDSKho = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colQLKhoID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQLKhoMaKho = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQLKhoTenKho = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQLKhoStatusVT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQLKhoParentMaKho = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQLKhoCBM = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQLKhoGhichu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQLKhoDVSX = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoLookUpDVSX = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
            this.layoutView1 = new DevExpress.XtraGrid.Views.Layout.LayoutView();
            this.grcKho = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colChon = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoChon = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManager2 = new DevExpress.XtraBars.BarManager(this.components);
            this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutViewCard1 = new DevExpress.XtraGrid.Views.Layout.LayoutViewCard();
            this.btnThemKho = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnXoaKho = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnNapLaiKho = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnLuuKho = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnCloseKho = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDonViSanXuat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDonViSanXuat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckGiaCong)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlQLKho)).BeginInit();
            this.pnlQLKho.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grcQLKho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDSKho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoLookUpDVSX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcKho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoChon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewCard1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
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
            this.Them,
            this.Sua,
            this.Xoa,
            this.Luu,
            this.Naplai,
            this.btnQLKho,
            this.btnSaveKho});
            this.barManager1.MaxItemId = 6;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.Them),
            new DevExpress.XtraBars.LinkPersistInfo(this.Sua),
            new DevExpress.XtraBars.LinkPersistInfo(this.Xoa),
            new DevExpress.XtraBars.LinkPersistInfo(this.Luu),
            new DevExpress.XtraBars.LinkPersistInfo(this.Naplai),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnQLKho),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnSaveKho)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // Them
            // 
            this.Them.Caption = "Thêm (Ctrl + Shift+ N)";
            this.Them.Id = 0;
            this.Them.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Them.ImageOptions.SvgImage")));
            this.Them.Name = "Them";
            this.Them.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Them.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Them_ItemClick);
            // 
            // Sua
            // 
            this.Sua.Caption = "Sửa (Ctrl + E)";
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
            this.Xoa.Name = "Xoa";
            this.Xoa.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Xoa.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Xoa_ItemClick);
            // 
            // Luu
            // 
            this.Luu.Caption = "Lưu (Ctrl + S)";
            this.Luu.Id = 3;
            this.Luu.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Luu.ImageOptions.SvgImage")));
            this.Luu.Name = "Luu";
            this.Luu.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Luu.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Luu_ItemClick);
            // 
            // Naplai
            // 
            this.Naplai.Caption = "Nạp lại (F5)";
            this.Naplai.Id = 4;
            this.Naplai.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("Naplai.ImageOptions.Image")));
            this.Naplai.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("Naplai.ImageOptions.LargeImage")));
            this.Naplai.Name = "Naplai";
            this.Naplai.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Naplai.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Naplai_ItemClick);
            // 
            // btnQLKho
            // 
            this.btnQLKho.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnQLKho.Caption = "Quản Lý Kho";
            this.btnQLKho.Id = 5;
            this.btnQLKho.ImageOptions.Image = global::NtbSoft.ERP.Win.Properties.Resources.listwork16x16;
            this.btnQLKho.Name = "btnQLKho";
            this.btnQLKho.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnQLKho.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnQLKho_ItemClick);
            // 
            // btnSaveKho
            // 
            this.btnSaveKho.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.btnSaveKho.Caption = "Lưu Kho";
            this.btnSaveKho.Id = 6;
            this.btnSaveKho.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveKho.ImageOptions.Image")));
            this.btnSaveKho.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnSaveKho.ImageOptions.LargeImage")));
            this.btnSaveKho.Name = "btnSaveKho";
            this.btnSaveKho.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnSaveKho.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSaveKho_ItemClick);
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
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlTop.Size = new System.Drawing.Size(1582, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 881);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(1582, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 845);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl1.Location = new System.Drawing.Point(1582, 36);
            this.barDockControl1.Manager = this.barManager1;
            this.barDockControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControl1.Size = new System.Drawing.Size(0, 845);
            // 
            // gridDonViSanXuat
            // 
            this.gridDonViSanXuat.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.gridDonViSanXuat.Location = new System.Drawing.Point(12, 12);
            this.gridDonViSanXuat.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridDonViSanXuat.MainView = this.gridViewDonViSanXuat;
            this.gridDonViSanXuat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridDonViSanXuat.MenuManager = this.barManager1;
            this.gridDonViSanXuat.Name = "gridDonViSanXuat";
            this.gridDonViSanXuat.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.CheckGiaCong});
            this.gridDonViSanXuat.Size = new System.Drawing.Size(761, 821);
            this.gridDonViSanXuat.TabIndex = 16;
            this.gridDonViSanXuat.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDonViSanXuat});
            this.gridDonViSanXuat.EditorKeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gridDonViSanXuat_EditorKeyPress);
            // 
            // gridViewDonViSanXuat
            // 
            this.gridViewDonViSanXuat.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(209)))));
            this.gridViewDonViSanXuat.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewDonViSanXuat.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridViewDonViSanXuat.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewDonViSanXuat.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridViewDonViSanXuat.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridViewDonViSanXuat.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridViewDonViSanXuat.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.colMaDVSX,
            this.colTenDVSX,
            this.colGiaCong,
            this.colSort,
            this.gridColumn2,
            this.colTrangThai,
            this.colMaHienThi});
            this.gridViewDonViSanXuat.DetailHeight = 431;
            this.gridViewDonViSanXuat.GridControl = this.gridDonViSanXuat;
            this.gridViewDonViSanXuat.IndicatorWidth = 47;
            this.gridViewDonViSanXuat.Name = "gridViewDonViSanXuat";
            this.gridViewDonViSanXuat.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewDonViSanXuat.OptionsCustomization.AllowFilter = false;
            this.gridViewDonViSanXuat.OptionsCustomization.AllowSort = false;
            this.gridViewDonViSanXuat.OptionsView.ColumnAutoWidth = false;
            this.gridViewDonViSanXuat.OptionsView.ShowAutoFilterRow = true;
            this.gridViewDonViSanXuat.OptionsView.ShowGroupPanel = false;
            this.gridViewDonViSanXuat.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridViewDonViSanXuat_RowCellClick);
            this.gridViewDonViSanXuat.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewDonViSanXuat_CustomDrawColumnHeader);
            this.gridViewDonViSanXuat.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridViewDonViSanXuat_CustomDrawRowIndicator);
            this.gridViewDonViSanXuat.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gridViewDonViSanXuat_PopupMenuShowing);
            this.gridViewDonViSanXuat.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewDonViSanXuat_FocusedRowChanged);
            this.gridViewDonViSanXuat.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.gridViewDonViSanXuat_FocusedColumnChanged);
            this.gridViewDonViSanXuat.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewDonViSanXuat_CellValueChanged);
            this.gridViewDonViSanXuat.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewDonViSanXuat_CellValueChanging);
            this.gridViewDonViSanXuat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gridViewDonViSanXuat_KeyPress_1);
            this.gridViewDonViSanXuat.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gridViewDonViSanXuat_ValidatingEditor);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumn1.AppearanceHeader.Options.UseFont = true;
            this.gridColumn1.Caption = "ID";
            this.gridColumn1.FieldName = "ID";
            this.gridColumn1.MinWidth = 23;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Width = 87;
            // 
            // colMaDVSX
            // 
            this.colMaDVSX.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaDVSX.AppearanceHeader.Options.UseFont = true;
            this.colMaDVSX.Caption = "Mã đơn vị sản xuất";
            this.colMaDVSX.FieldName = "MaDVSX";
            this.colMaDVSX.MinWidth = 23;
            this.colMaDVSX.Name = "colMaDVSX";
            this.colMaDVSX.Width = 181;
            // 
            // colTenDVSX
            // 
            this.colTenDVSX.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colTenDVSX.AppearanceHeader.Options.UseFont = true;
            this.colTenDVSX.Caption = "Đơn vị sản xuất";
            this.colTenDVSX.FieldName = "TenDVSX";
            this.colTenDVSX.MinWidth = 23;
            this.colTenDVSX.Name = "colTenDVSX";
            this.colTenDVSX.Visible = true;
            this.colTenDVSX.VisibleIndex = 0;
            this.colTenDVSX.Width = 222;
            // 
            // colGiaCong
            // 
            this.colGiaCong.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colGiaCong.AppearanceHeader.Options.UseFont = true;
            this.colGiaCong.Caption = "Gia công";
            this.colGiaCong.ColumnEdit = this.CheckGiaCong;
            this.colGiaCong.FieldName = "GiaCong";
            this.colGiaCong.MinWidth = 24;
            this.colGiaCong.Name = "colGiaCong";
            this.colGiaCong.Visible = true;
            this.colGiaCong.VisibleIndex = 1;
            this.colGiaCong.Width = 94;
            // 
            // CheckGiaCong
            // 
            this.CheckGiaCong.AutoHeight = false;
            this.CheckGiaCong.Name = "CheckGiaCong";
            this.CheckGiaCong.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // colSort
            // 
            this.colSort.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colSort.AppearanceHeader.Options.UseFont = true;
            this.colSort.Caption = "Thứ tự";
            this.colSort.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colSort.FieldName = "Sort";
            this.colSort.MinWidth = 24;
            this.colSort.Name = "colSort";
            this.colSort.OptionsColumn.AllowEdit = false;
            this.colSort.OptionsColumn.ReadOnly = true;
            this.colSort.Visible = true;
            this.colSort.VisibleIndex = 2;
            this.colSort.Width = 94;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.gridColumn2.AppearanceHeader.Options.UseFont = true;
            this.gridColumn2.Caption = "Ghi chú";
            this.gridColumn2.FieldName = "GhiChu";
            this.gridColumn2.MinWidth = 23;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 4;
            this.gridColumn2.Width = 343;
            // 
            // colTrangThai
            // 
            this.colTrangThai.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colTrangThai.AppearanceHeader.Options.UseFont = true;
            this.colTrangThai.Caption = "Trạng thái";
            this.colTrangThai.FieldName = "TrangThai";
            this.colTrangThai.MinWidth = 23;
            this.colTrangThai.Name = "colTrangThai";
            // 
            // colMaHienThi
            // 
            this.colMaHienThi.Caption = "Mã hiện thị";
            this.colMaHienThi.FieldName = "MaHienThi";
            this.colMaHienThi.MinWidth = 24;
            this.colMaHienThi.Name = "colMaHienThi";
            this.colMaHienThi.Visible = true;
            this.colMaHienThi.VisibleIndex = 3;
            this.colMaHienThi.Width = 134;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(808, 6);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 21;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridDonViSanXuat);
            this.layoutControl1.Controls.Add(this.pnlQLKho);
            this.layoutControl1.Controls.Add(this.grcKho);
            this.layoutControl1.Controls.Add(this.btnThemKho);
            this.layoutControl1.Controls.Add(this.btnXoaKho);
            this.layoutControl1.Controls.Add(this.btnNapLaiKho);
            this.layoutControl1.Controls.Add(this.btnLuuKho);
            this.layoutControl1.Controls.Add(this.btnCloseKho);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 36);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3802, 675, 812, 500);
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1582, 845);
            this.layoutControl1.TabIndex = 26;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // pnlQLKho
            // 
            this.pnlQLKho.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.pnlQLKho.Controls.Add(this.grcQLKho);
            this.pnlQLKho.Location = new System.Drawing.Point(786, 372);
            this.pnlQLKho.LookAndFeel.SkinName = "Office 2010 Blue";
            this.pnlQLKho.LookAndFeel.UseDefaultLookAndFeel = false;
            this.pnlQLKho.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlQLKho.Name = "pnlQLKho";
            this.pnlQLKho.Size = new System.Drawing.Size(784, 461);
            this.pnlQLKho.TabIndex = 0;
            this.pnlQLKho.Visible = false;
            // 
            // grcQLKho
            // 
            this.grcQLKho.AllowDrop = true;
            this.grcQLKho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grcQLKho.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            gridLevelNode1.RelationName = "Level1";
            this.grcQLKho.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
            this.grcQLKho.Location = new System.Drawing.Point(2, 2);
            this.grcQLKho.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grcQLKho.MainView = this.gridViewDSKho;
            this.grcQLKho.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grcQLKho.Name = "grcQLKho";
            this.grcQLKho.Size = new System.Drawing.Size(780, 457);
            this.grcQLKho.TabIndex = 0;
            this.grcQLKho.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDSKho,
            this.layoutView1});
            // 
            // gridViewDSKho
            // 
            this.gridViewDSKho.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(209)))));
            this.gridViewDSKho.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewDSKho.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridViewDSKho.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewDSKho.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridViewDSKho.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridViewDSKho.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridViewDSKho.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colQLKhoID,
            this.colQLKhoMaKho,
            this.colQLKhoTenKho,
            this.colQLKhoStatusVT,
            this.colQLKhoParentMaKho,
            this.colQLKhoCBM,
            this.colQLKhoGhichu,
            this.colQLKhoDVSX});
            this.gridViewDSKho.GridControl = this.grcQLKho;
            this.gridViewDSKho.IndicatorWidth = 47;
            this.gridViewDSKho.Name = "gridViewDSKho";
            this.gridViewDSKho.OptionsSelection.MultiSelect = true;
            this.gridViewDSKho.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.gridViewDSKho.OptionsView.ShowAutoFilterRow = true;
            this.gridViewDSKho.OptionsView.ShowGroupPanel = false;
            // 
            // colQLKhoID
            // 
            this.colQLKhoID.Caption = "ID";
            this.colQLKhoID.FieldName = "ID";
            this.colQLKhoID.Name = "colQLKhoID";
            this.colQLKhoID.Visible = true;
            this.colQLKhoID.VisibleIndex = 1;
            this.colQLKhoID.Width = 199;
            // 
            // colQLKhoMaKho
            // 
            this.colQLKhoMaKho.Caption = "Mã Kho";
            this.colQLKhoMaKho.FieldName = "MaKho";
            this.colQLKhoMaKho.Name = "colQLKhoMaKho";
            this.colQLKhoMaKho.Visible = true;
            this.colQLKhoMaKho.VisibleIndex = 2;
            this.colQLKhoMaKho.Width = 199;
            // 
            // colQLKhoTenKho
            // 
            this.colQLKhoTenKho.Caption = "Tên Kho";
            this.colQLKhoTenKho.FieldName = "TenKho";
            this.colQLKhoTenKho.Name = "colQLKhoTenKho";
            this.colQLKhoTenKho.Visible = true;
            this.colQLKhoTenKho.VisibleIndex = 3;
            this.colQLKhoTenKho.Width = 199;
            // 
            // colQLKhoStatusVT
            // 
            this.colQLKhoStatusVT.Caption = "Status_VT";
            this.colQLKhoStatusVT.FieldName = "Status_VT";
            this.colQLKhoStatusVT.Name = "colQLKhoStatusVT";
            this.colQLKhoStatusVT.Visible = true;
            this.colQLKhoStatusVT.VisibleIndex = 6;
            this.colQLKhoStatusVT.Width = 300;
            // 
            // colQLKhoParentMaKho
            // 
            this.colQLKhoParentMaKho.Caption = "Mã DVSX";
            this.colQLKhoParentMaKho.FieldName = "ParentMaKho";
            this.colQLKhoParentMaKho.Name = "colQLKhoParentMaKho";
            this.colQLKhoParentMaKho.Visible = true;
            this.colQLKhoParentMaKho.VisibleIndex = 4;
            this.colQLKhoParentMaKho.Width = 199;
            // 
            // colQLKhoCBM
            // 
            this.colQLKhoCBM.Caption = "CBM";
            this.colQLKhoCBM.FieldName = "CBM";
            this.colQLKhoCBM.Name = "colQLKhoCBM";
            this.colQLKhoCBM.Visible = true;
            this.colQLKhoCBM.VisibleIndex = 7;
            this.colQLKhoCBM.Width = 300;
            // 
            // colQLKhoGhichu
            // 
            this.colQLKhoGhichu.Caption = "Ghi Chú";
            this.colQLKhoGhichu.FieldName = "Ghichu";
            this.colQLKhoGhichu.Name = "colQLKhoGhichu";
            this.colQLKhoGhichu.Visible = true;
            this.colQLKhoGhichu.VisibleIndex = 8;
            this.colQLKhoGhichu.Width = 300;
            // 
            // colQLKhoDVSX
            // 
            this.colQLKhoDVSX.Caption = "Đơn vị sản xuất";
            this.colQLKhoDVSX.ColumnEdit = this.repoLookUpDVSX;
            this.colQLKhoDVSX.FieldName = "ParentMaKho";
            this.colQLKhoDVSX.Name = "colQLKhoDVSX";
            this.colQLKhoDVSX.Visible = true;
            this.colQLKhoDVSX.VisibleIndex = 5;
            this.colQLKhoDVSX.Width = 199;
            // 
            // repoLookUpDVSX
            // 
            this.repoLookUpDVSX.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.repoLookUpDVSX.AutoHeight = false;
            this.repoLookUpDVSX.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoLookUpDVSX.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TenKho", "Tên đơn vị sản xuất")});
            this.repoLookUpDVSX.DisplayMember = "TenKho";
            this.repoLookUpDVSX.Name = "repoLookUpDVSX";
            this.repoLookUpDVSX.NullText = "";
            this.repoLookUpDVSX.ShowHeader = false;
            this.repoLookUpDVSX.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.repoLookUpDVSX.ValueMember = "MaKho";
            // 
            // layoutView1
            // 
            this.layoutView1.GridControl = this.grcQLKho;
            this.layoutView1.Name = "layoutView1";
            this.layoutView1.TemplateCard = null;
            // 
            // grcKho
            // 
            this.grcKho.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.grcKho.Location = new System.Drawing.Point(786, 12);
            this.grcKho.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grcKho.MainView = this.gridView1;
            this.grcKho.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grcKho.MenuManager = this.barManager1;
            this.grcKho.Name = "grcKho";
            this.grcKho.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoChon});
            this.grcKho.Size = new System.Drawing.Size(784, 325);
            this.grcKho.TabIndex = 17;
            this.grcKho.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(209)))));
            this.gridView1.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridView1.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridView1.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView1.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridView1.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridView1.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn3,
            this.gridColumn4,
            this.colChon});
            this.gridView1.DetailHeight = 431;
            this.gridView1.GridControl = this.grcKho;
            this.gridView1.IndicatorWidth = 47;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsCustomization.AllowColumnMoving = false;
            this.gridView1.OptionsCustomization.AllowFilter = false;
            this.gridView1.OptionsCustomization.AllowSort = false;
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.ShowAutoFilterRow = true;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridColumn3.AppearanceHeader.Options.UseFont = true;
            this.gridColumn3.Caption = "ID";
            this.gridColumn3.FieldName = "ID";
            this.gridColumn3.MinWidth = 23;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Width = 87;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.gridColumn4.AppearanceHeader.Options.UseFont = true;
            this.gridColumn4.Caption = "Kho";
            this.gridColumn4.FieldName = "TenKho";
            this.gridColumn4.MinWidth = 23;
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 1;
            this.gridColumn4.Width = 181;
            // 
            // colChon
            // 
            this.colChon.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colChon.AppearanceHeader.Options.UseFont = true;
            this.colChon.Caption = "Chọn";
            this.colChon.ColumnEdit = this.repoChon;
            this.colChon.FieldName = "Chon";
            this.colChon.MinWidth = 24;
            this.colChon.Name = "colChon";
            this.colChon.Visible = true;
            this.colChon.VisibleIndex = 0;
            this.colChon.Width = 94;
            // 
            // repoChon
            // 
            this.repoChon.AutoHeight = false;
            this.repoChon.Name = "repoChon";
            this.repoChon.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.splitterItem1,
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.layoutControlItem3,
            this.layoutControlItem5,
            this.layoutControlItem8,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.emptySpaceItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1582, 845);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridDonViSanXuat;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(765, 825);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // splitterItem1
            // 
            this.splitterItem1.AllowHotTrack = true;
            this.splitterItem1.Location = new System.Drawing.Point(765, 0);
            this.splitterItem1.Name = "splitterItem1";
            this.splitterItem1.Size = new System.Drawing.Size(9, 825);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.grcKho;
            this.layoutControlItem2.Location = new System.Drawing.Point(774, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(788, 329);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.pnlQLKho;
            this.layoutControlItem4.Location = new System.Drawing.Point(774, 360);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(0, 465);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(5, 465);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(788, 465);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            this.layoutControlItem4.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // barManager2
            // 
            this.barManager2.Controller = this.barAndDockingController1;
            this.barManager2.DockControls.Add(this.barDockControl2);
            this.barManager2.DockControls.Add(this.barDockControl3);
            this.barManager2.DockControls.Add(this.barDockControl4);
            this.barManager2.DockControls.Add(this.barDockControlRight);
            this.barManager2.Form = this;
            // 
            // barDockControl2
            // 
            this.barDockControl2.CausesValidation = false;
            this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControl2.Location = new System.Drawing.Point(0, 0);
            this.barDockControl2.Manager = this.barManager2;
            this.barDockControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.barDockControl2.Size = new System.Drawing.Size(1582, 0);
            // 
            // barDockControl3
            // 
            this.barDockControl3.CausesValidation = false;
            this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControl3.Location = new System.Drawing.Point(0, 881);
            this.barDockControl3.Manager = this.barManager2;
            this.barDockControl3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.barDockControl3.Size = new System.Drawing.Size(1582, 0);
            // 
            // barDockControl4
            // 
            this.barDockControl4.CausesValidation = false;
            this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControl4.Location = new System.Drawing.Point(0, 0);
            this.barDockControl4.Manager = this.barManager2;
            this.barDockControl4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.barDockControl4.Size = new System.Drawing.Size(0, 881);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1582, 0);
            this.barDockControlRight.Manager = this.barManager2;
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 881);
            // 
            // layoutViewCard1
            // 
            this.layoutViewCard1.HeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
            this.layoutViewCard1.Name = "layoutViewCard1";
            // 
            // btnThemKho
            // 
            this.btnThemKho.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnThemKho.ImageOptions.Image")));
            this.btnThemKho.Location = new System.Drawing.Point(786, 341);
            this.btnThemKho.Name = "btnThemKho";
            this.btnThemKho.Size = new System.Drawing.Size(141, 27);
            this.btnThemKho.StyleController = this.layoutControl1;
            this.btnThemKho.TabIndex = 18;
            this.btnThemKho.Text = "Thêm(Ctrl+Shift+N)";
            this.btnThemKho.Click += new System.EventHandler(this.btnThemKho_Click);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnThemKho;
            this.layoutControlItem3.Location = new System.Drawing.Point(774, 329);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(145, 31);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            this.layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // btnXoaKho
            // 
            this.btnXoaKho.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnXoaKho.ImageOptions.Image")));
            this.btnXoaKho.Location = new System.Drawing.Point(931, 341);
            this.btnXoaKho.Name = "btnXoaKho";
            this.btnXoaKho.Size = new System.Drawing.Size(96, 27);
            this.btnXoaKho.StyleController = this.layoutControl1;
            this.btnXoaKho.TabIndex = 19;
            this.btnXoaKho.Text = "Xóa(Delete)";
            this.btnXoaKho.Click += new System.EventHandler(this.btnXoaKho_Click);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnXoaKho;
            this.layoutControlItem5.Location = new System.Drawing.Point(919, 329);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(100, 31);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            this.layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // btnNapLaiKho
            // 
            this.btnNapLaiKho.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNapLaiKho.ImageOptions.Image")));
            this.btnNapLaiKho.Location = new System.Drawing.Point(1130, 341);
            this.btnNapLaiKho.Name = "btnNapLaiKho";
            this.btnNapLaiKho.Size = new System.Drawing.Size(92, 27);
            this.btnNapLaiKho.StyleController = this.layoutControl1;
            this.btnNapLaiKho.TabIndex = 20;
            this.btnNapLaiKho.Text = "Nạp lại(F5)";
            this.btnNapLaiKho.Click += new System.EventHandler(this.btnNapLaiKho_Click);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.btnNapLaiKho;
            this.layoutControlItem6.Location = new System.Drawing.Point(1118, 329);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(96, 31);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            this.layoutControlItem6.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // btnLuuKho
            // 
            this.btnLuuKho.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnLuuKho.ImageOptions.Image")));
            this.btnLuuKho.Location = new System.Drawing.Point(1031, 341);
            this.btnLuuKho.Name = "btnLuuKho";
            this.btnLuuKho.Size = new System.Drawing.Size(95, 27);
            this.btnLuuKho.StyleController = this.layoutControl1;
            this.btnLuuKho.TabIndex = 21;
            this.btnLuuKho.Text = "Lưu(Ctrl+S)";
            this.btnLuuKho.Click += new System.EventHandler(this.btnLuuKho_Click);
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnLuuKho;
            this.layoutControlItem7.Location = new System.Drawing.Point(1019, 329);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(99, 31);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            this.layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // btnCloseKho
            // 
            this.btnCloseKho.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCloseKho.ImageOptions.Image")));
            this.btnCloseKho.Location = new System.Drawing.Point(1509, 341);
            this.btnCloseKho.Name = "btnCloseKho";
            this.btnCloseKho.Size = new System.Drawing.Size(61, 27);
            this.btnCloseKho.StyleController = this.layoutControl1;
            this.btnCloseKho.TabIndex = 22;
            this.btnCloseKho.Text = "Đóng";
            this.btnCloseKho.Click += new System.EventHandler(this.btnCloseKho_Click_1);
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.btnCloseKho;
            this.layoutControlItem8.Location = new System.Drawing.Point(1497, 329);
            this.layoutControlItem8.MaxSize = new System.Drawing.Size(65, 31);
            this.layoutControlItem8.MinSize = new System.Drawing.Size(65, 31);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(65, 31);
            this.layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem8.Text = "xóa";
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            this.layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(1214, 329);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(283, 31);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            this.emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // frmDonViSanXuat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1582, 881);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControl1);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Controls.Add(this.barDockControl4);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControl3);
            this.Controls.Add(this.barDockControl2);
            this.LookAndFeel.SkinName = "London Liquid Sky";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmDonViSanXuat";
            this.ShowIcon = false;
            this.Text = "Đơn vị sản xuất";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDonViSanXuat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDonViSanXuat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckGiaCong)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlQLKho)).EndInit();
            this.pnlQLKho.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grcQLKho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDSKho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoLookUpDVSX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcKho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoChon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutViewCard1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
       
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem Them;
        private DevExpress.XtraBars.BarButtonItem Sua;
        private DevExpress.XtraBars.BarButtonItem Xoa;
        private DevExpress.XtraBars.BarButtonItem Luu;
        private DevExpress.XtraBars.BarButtonItem Naplai;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraGrid.GridControl gridDonViSanXuat;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDonViSanXuat;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn colMaDVSX;
        private DevExpress.XtraGrid.Columns.GridColumn colTenDVSX;
        private DevExpress.XtraGrid.Columns.GridColumn colGiaCong;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckGiaCong;
        private System.Windows.Forms.Button button1;
        private DevExpress.XtraGrid.Columns.GridColumn colTrangThai;
        private DevExpress.XtraGrid.Columns.GridColumn colSort;
        private DevExpress.XtraGrid.Columns.GridColumn colMaHienThi;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl grcKho;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn colChon;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repoChon;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.SplitterItem splitterItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraBars.BarButtonItem btnSaveKho;
        #region QLKho Thoai
        private DevExpress.XtraBars.BarButtonItem btnQLKho;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
        private DevExpress.XtraBars.BarManager barManager2;
        private DevExpress.XtraBars.BarDockControl barDockControl2;
        private DevExpress.XtraBars.BarDockControl barDockControl3;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.PanelControl pnlQLKho;
        private DevExpress.XtraGrid.GridControl grcQLKho;

        private DevExpress.XtraGrid.Views.Layout.LayoutView layoutView1;
        private DevExpress.XtraGrid.Views.Layout.LayoutViewCard layoutViewCard1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDSKho;

        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;

        private DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit repoLookUpDVSX;
        private DevExpress.XtraGrid.Columns.GridColumn colQLKhoTenKho;
        private DevExpress.XtraGrid.Columns.GridColumn colQLKhoGhichu;
        private DevExpress.XtraGrid.Columns.GridColumn colQLKhoDVSX;

        private DevExpress.XtraGrid.Columns.GridColumn colQLKhoID;
        private DevExpress.XtraGrid.Columns.GridColumn colQLKhoMaKho;
        private DevExpress.XtraGrid.Columns.GridColumn colQLKhoStatusVT;
        private DevExpress.XtraGrid.Columns.GridColumn colQLKhoParentMaKho;
        private DevExpress.XtraGrid.Columns.GridColumn colQLKhoCBM;
        #endregion QLKho Thoai
        private DevExpress.XtraEditors.SimpleButton btnThemKho;
        private DevExpress.XtraEditors.SimpleButton btnXoaKho;
        private DevExpress.XtraEditors.SimpleButton btnNapLaiKho;
        private DevExpress.XtraEditors.SimpleButton btnLuuKho;
        private DevExpress.XtraEditors.SimpleButton btnCloseKho;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}