
namespace NtbSoft.ERP.Win.Modules.Kho
{
    partial class frmERPDonViVT
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmERPDonViVT));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.Them = new DevExpress.XtraBars.BarButtonItem();
            this.Sua = new DevExpress.XtraBars.BarButtonItem();
            this.Xoa = new DevExpress.XtraBars.BarButtonItem();
            this.btnSubmit = new DevExpress.XtraBars.BarButtonItem();
            this.Luu = new DevExpress.XtraBars.BarButtonItem();
            this.Naplai = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.btnXacNhan = new DevExpress.XtraEditors.SimpleButton();
            this.gridDonViVT = new DevExpress.XtraGrid.GridControl();
            this.gridViewDonViVT = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaDVVT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenDVVT = new DevExpress.XtraGrid.Columns.GridColumn();
            #region Thoai
            this.colNguoiTao = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNgayTao = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNguoiSua = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNgaySua = new DevExpress.XtraGrid.Columns.GridColumn();
            #endregion
            this.colGhiChu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTiLe = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQDSoLe = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ItemChkLTSoLe = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.txtTendvvt = new DevExpress.XtraEditors.TextEdit();
            this.txtTiLe = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.button1 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDonViVT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDonViVT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemChkLTSoLe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTendvvt.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTiLe.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).BeginInit();
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
            this.barManager1.DockControls.Add(this.barDockControl1);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.Them,
            this.Sua,
            this.Xoa,
            this.Luu,
            this.Naplai,
            this.barButtonItem1,
            this.btnSubmit});
            this.barManager1.MaxItemId = 7;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.None, false, this.Them, false),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.None, false, this.Sua, false),
            new DevExpress.XtraBars.LinkPersistInfo(this.Xoa),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.None, false, this.btnSubmit, false),
            new DevExpress.XtraBars.LinkPersistInfo(this.Luu),
            new DevExpress.XtraBars.LinkPersistInfo(this.Naplai),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.None, false, this.barButtonItem1, false)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // Them
            // 
            this.Them.Caption = "Thêm (Ctrl  + Shift+ N)";
            this.Them.Id = 0;
            this.Them.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Them.ImageOptions.SvgImage")));
            this.Them.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.N));
            this.Them.Name = "Them";
            this.Them.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // Sua
            // 
            this.Sua.Caption = "Sửa (Ctrl + E)";
            this.Sua.Id = 1;
            this.Sua.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Sua.ImageOptions.SvgImage")));
            this.Sua.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E));
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
            // btnSubmit
            // 
            this.btnSubmit.Caption = "Xác Nhận";
            this.btnSubmit.CausesValidation = true;
            this.btnSubmit.Id = 6;
            this.btnSubmit.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSubmit.ImageOptions.Image")));
            this.btnSubmit.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnSubmit.ImageOptions.LargeImage")));
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnSubmit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSubmit_ItemClick);
            // 
            // Luu
            // 
            this.Luu.Caption = "Lưu (Ctrl + S)";
            this.Luu.Id = 3;
            this.Luu.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Luu.ImageOptions.SvgImage")));
            this.Luu.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
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
            this.Naplai.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F5);
            this.Naplai.Name = "Naplai";
            this.Naplai.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Naplai.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Naplai_ItemClick);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Import Excel";
            this.barButtonItem1.Id = 5;
            this.barButtonItem1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.Image")));
            this.barButtonItem1.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.LargeImage")));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlTop.Size = new System.Drawing.Size(1136, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 547);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(1136, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 511);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl1.Location = new System.Drawing.Point(1136, 36);
            this.barDockControl1.Manager = this.barManager1;
            this.barDockControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControl1.Size = new System.Drawing.Size(0, 511);
            // 
            // barAndDockingController1
            // 
            this.barAndDockingController1.LookAndFeel.SkinMaskColor = System.Drawing.Color.Lime;
            this.barAndDockingController1.LookAndFeel.SkinName = "Office 2010 Blue";
            this.barAndDockingController1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.btnXacNhan);
            this.layoutControl2.Controls.Add(this.gridDonViVT);
            this.layoutControl2.Controls.Add(this.txtTendvvt);
            this.layoutControl2.Controls.Add(this.txtTiLe);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 36);
            this.layoutControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3385, 566, 812, 500);
            this.layoutControl2.Root = this.layoutControlGroup1;
            this.layoutControl2.Size = new System.Drawing.Size(1136, 511);
            this.layoutControl2.TabIndex = 11;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // btnXacNhan
            // 
            this.btnXacNhan.Appearance.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnXacNhan.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnXacNhan.Appearance.ForeColor = System.Drawing.Color.OrangeRed;
            this.btnXacNhan.Appearance.Options.UseBackColor = true;
            this.btnXacNhan.Appearance.Options.UseFont = true;
            this.btnXacNhan.Appearance.Options.UseForeColor = true;
            this.btnXacNhan.Appearance.Options.UseTextOptions = true;
            this.btnXacNhan.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.btnXacNhan.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.btnXacNhan.AppearancePressed.Font = new System.Drawing.Font("Tahoma", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnXacNhan.AppearancePressed.ForeColor = System.Drawing.Color.Navy;
            this.btnXacNhan.AppearancePressed.Options.UseFont = true;
            this.btnXacNhan.AppearancePressed.Options.UseForeColor = true;
            this.btnXacNhan.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnXacNhan.ImageOptions.Image")));
            this.btnXacNhan.Location = new System.Drawing.Point(12, 61);
            this.btnXacNhan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnXacNhan.Name = "btnXacNhan";
            this.btnXacNhan.Size = new System.Drawing.Size(144, 27);
            this.btnXacNhan.StyleController = this.layoutControl2;
            this.btnXacNhan.TabIndex = 53;
            this.btnXacNhan.Text = "Xác Nhận Thêm";
            this.btnXacNhan.Click += new System.EventHandler(this.btnXacNhan_Click);
            // 
            // gridDonViVT
            // 
            this.gridDonViVT.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.gridDonViVT.Location = new System.Drawing.Point(12, 92);
            this.gridDonViVT.LookAndFeel.SkinName = "Office 2010 Blue";
            this.gridDonViVT.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridDonViVT.MainView = this.gridViewDonViVT;
            this.gridDonViVT.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridDonViVT.MenuManager = this.barManager1;
            this.gridDonViVT.Name = "gridDonViVT";
            this.gridDonViVT.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.ItemChkLTSoLe});
            this.gridDonViVT.Size = new System.Drawing.Size(1112, 407);
            this.gridDonViVT.TabIndex = 9;
            this.gridDonViVT.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDonViVT});
            this.gridDonViVT.EditorKeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gC_EditorKeyPress);
            // 
            // gridViewDonViVT
            // 
            this.gridViewDonViVT.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(209)))));
            this.gridViewDonViVT.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewDonViVT.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9F);
            this.gridViewDonViVT.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewDonViVT.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridViewDonViVT.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridViewDonViVT.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridViewDonViVT.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.gridViewDonViVT.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gridViewDonViVT.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colID,
            this.colMaDVVT,
            this.colTenDVVT,
            this.colGhiChu,
            this.colTiLe,
            this.colQDSoLe,
            //Thoai thêm 4 cột
            this.colNguoiTao,
            this.colNgayTao,
            this.colNguoiSua,
            this.colNgaySua});
            this.gridViewDonViVT.DetailHeight = 431;
            this.gridViewDonViVT.GridControl = this.gridDonViVT;
            this.gridViewDonViVT.IndicatorWidth = 47;
            this.gridViewDonViVT.Name = "gridViewDonViVT";
            this.gridViewDonViVT.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewDonViVT.OptionsCustomization.AllowFilter = false;
            this.gridViewDonViVT.OptionsCustomization.AllowSort = false;
            this.gridViewDonViVT.OptionsSelection.MultiSelect = true;
            this.gridViewDonViVT.OptionsView.ColumnAutoWidth = false;
            this.gridViewDonViVT.OptionsView.ShowAutoFilterRow = true;
            this.gridViewDonViVT.OptionsView.ShowGroupPanel = false;
            this.gridViewDonViVT.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewDonViVT_CellValueChanged);
            this.gridViewDonViVT.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewDonViVT_CustomColumnDisplayText);
            this.gridViewDonViVT.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.grv_ValidatingEditor);
            this.gridViewDonViVT.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.grv_InvalidValueException);
            // 
            // colID
            // 
            this.colID.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colID.AppearanceHeader.Options.UseFont = true;
            this.colID.Caption = "ID";
            this.colID.FieldName = "ID";
            this.colID.Name = "colID";
            this.colID.Width = 76;
            // 
            // colMaDVVT
            // 
            this.colMaDVVT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaDVVT.AppearanceHeader.Options.UseFont = true;
            this.colMaDVVT.Caption = "Mã Đơn Vị Vật Tư";
            this.colMaDVVT.FieldName = "MaDVVT";
            this.colMaDVVT.Name = "colMaDVVT";
            this.colMaDVVT.Width = 125;
            // 
            // colTenDVVT
            // 
            this.colTenDVVT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.colTenDVVT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colTenDVVT.AppearanceHeader.Options.UseBackColor = true;
            this.colTenDVVT.AppearanceHeader.Options.UseFont = true;
            this.colTenDVVT.Caption = "Đơn Vị Vật Tư";
            this.colTenDVVT.FieldName = "TenDVVT";
            this.colTenDVVT.MinWidth = 24;
            this.colTenDVVT.Name = "colTenDVVT";
            this.colTenDVVT.Visible = true;
            this.colTenDVVT.VisibleIndex = 0;
            this.colTenDVVT.Width = 198;
            #region Thoai
            // 
            // colNguoiTao
            // 
            this.colNguoiTao.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.colNguoiTao.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colNguoiTao.AppearanceHeader.Options.UseBackColor = true;
            this.colNguoiTao.AppearanceHeader.Options.UseFont = true;
            this.colNguoiTao.Caption = "Người tạo";
            this.colNguoiTao.FieldName = "NguoiTao";
            this.colNguoiTao.MinWidth = 25;
            this.colNguoiTao.Name = "colNguoiTao";
            this.colNguoiTao.Visible = true;
            this.colNguoiTao.VisibleIndex = 3;
            this.colNguoiTao.Width = 239;
            this.colNguoiTao.OptionsColumn.AllowEdit = false;
            this.colNguoiTao.OptionsColumn.AllowFocus = false;
            // 
            // colNgayTao
            // 
            this.colNgayTao.Caption = "Ngày tạo";
            this.colNgayTao.FieldName = "NgayTao";
            this.colNgayTao.MinWidth = 25;
            this.colNgayTao.Name = "colNgayTao";
            // 
            // colNguoiSua
            // 
            this.colNguoiSua.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.colNguoiSua.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colNguoiSua.AppearanceHeader.Options.UseBackColor = true;
            this.colNguoiSua.AppearanceHeader.Options.UseFont = true;
            this.colNguoiSua.Caption = "Người sửa";
            this.colNguoiSua.FieldName = "NguoiSua";
            this.colNguoiSua.MinWidth = 25;
            this.colNguoiSua.Name = "colNguoiSua";
            this.colNguoiSua.Visible = true;
            this.colNguoiSua.VisibleIndex = 5;
            this.colNguoiSua.Width = 239;
            this.colNguoiSua.OptionsColumn.AllowEdit = false;
            this.colNguoiSua.OptionsColumn.AllowFocus = false;
            // 
            // colNgaySua
            // 
            this.colNgaySua.Caption = "Ngày sửa";
            this.colNgaySua.FieldName = "NgaySua";
            this.colNgaySua.MinWidth = 25;
            this.colNgaySua.Name = "colNgaySua";
            #endregion
            // 
            // colGhiChu
            // 
            this.colGhiChu.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colGhiChu.AppearanceHeader.Options.UseFont = true;
            this.colGhiChu.Caption = "Ghi chú";
            this.colGhiChu.FieldName = "GhiChu";
            this.colGhiChu.Name = "colGhiChu";
            this.colGhiChu.Width = 146;
            // 
            // colTiLe
            // 
            this.colTiLe.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.colTiLe.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colTiLe.AppearanceHeader.Options.UseBackColor = true;
            this.colTiLe.AppearanceHeader.Options.UseFont = true;
            this.colTiLe.AppearanceHeader.Options.UseTextOptions = true;
            this.colTiLe.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colTiLe.Caption = "Tỉ Lệ (Đổi Sang Mét)";
            this.colTiLe.FieldName = "Tile";
            this.colTiLe.MinWidth = 125;
            this.colTiLe.Name = "colTiLe";
            this.colTiLe.OptionsEditForm.ColumnSpan = 2;
            this.colTiLe.OptionsEditForm.RowSpan = 2;
            this.colTiLe.Visible = true;
            this.colTiLe.VisibleIndex = 1;
            this.colTiLe.Width = 160;
            // 
            // colQDSoLe
            // 
            this.colQDSoLe.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.colQDSoLe.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colQDSoLe.AppearanceHeader.Options.UseBackColor = true;
            this.colQDSoLe.AppearanceHeader.Options.UseFont = true;
            this.colQDSoLe.AppearanceHeader.Options.UseTextOptions = true;
            this.colQDSoLe.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colQDSoLe.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colQDSoLe.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colQDSoLe.Caption = "Qui đổi Số Lẻ";
            this.colQDSoLe.ColumnEdit = this.ItemChkLTSoLe;
            this.colQDSoLe.FieldName = "QDLamTronLe";
            this.colQDSoLe.MinWidth = 25;
            this.colQDSoLe.Name = "colQDSoLe";
            this.colQDSoLe.OptionsEditForm.RowSpan = 2;
            this.colQDSoLe.Visible = true;
            this.colQDSoLe.VisibleIndex = 2;
            this.colQDSoLe.Width = 107;
            // 
            // ItemChkLTSoLe
            // 
            this.ItemChkLTSoLe.AutoHeight = false;
            this.ItemChkLTSoLe.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.SvgCheckBox1;
            this.ItemChkLTSoLe.CheckBoxOptions.SvgColorChecked = System.Drawing.Color.Lime;
            this.ItemChkLTSoLe.CheckBoxOptions.SvgColorGrayed = System.Drawing.Color.Silver;
            this.ItemChkLTSoLe.CheckBoxOptions.SvgColorUnchecked = System.Drawing.Color.Silver;
            this.ItemChkLTSoLe.Name = "ItemChkLTSoLe";
            // 
            // txtTendvvt
            // 
            this.txtTendvvt.Location = new System.Drawing.Point(12, 35);
            this.txtTendvvt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtTendvvt.MenuManager = this.barManager1;
            this.txtTendvvt.Name = "txtTendvvt";
            this.txtTendvvt.Properties.NullValuePrompt = "M1:M2";
            this.txtTendvvt.Properties.ShowNullValuePromptWhenFocused = true;
            this.txtTendvvt.Size = new System.Drawing.Size(504, 22);
            this.txtTendvvt.StyleController = this.layoutControl2;
            this.txtTendvvt.TabIndex = 4;
            this.txtTendvvt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_KeyPress);
            // 
            // txtTiLe
            // 
            this.txtTiLe.EditValue = "";
            this.txtTiLe.Location = new System.Drawing.Point(530, 33);
            this.txtTiLe.MenuManager = this.barManager1;
            this.txtTiLe.Name = "txtTiLe";
            this.txtTiLe.Size = new System.Drawing.Size(117, 22);
            this.txtTiLe.StyleController = this.layoutControl2;
            this.txtTiLe.TabIndex = 54;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.AppearanceGroup.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlGroup1.AppearanceGroup.ForeColor = System.Drawing.Color.Red;
            this.layoutControlGroup1.AppearanceGroup.Options.UseFont = true;
            this.layoutControlGroup1.AppearanceGroup.Options.UseForeColor = true;
            this.layoutControlGroup1.CustomizationFormText = "Nhập thông tin";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3,
            this.layoutControlItem2,
            this.layoutControlItem7,
            this.emptySpaceItem3,
            this.layoutControlItem1,
            this.emptySpaceItem5,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1136, 511);
            this.layoutControlGroup1.Text = "Nhập thông tin";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.layoutControlItem3.AppearanceItemCaption.ForeColor = System.Drawing.Color.Blue;
            this.layoutControlItem3.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem3.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem3.Control = this.txtTendvvt;
            this.layoutControlItem3.CustomizationFormText = "ĐƠN VỊ VẬT TƯ";
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(508, 49);
            this.layoutControlItem3.Text = "ĐƠN VỊ VẬT TƯ";
            this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.layoutControlItem3.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem3.TextSize = new System.Drawing.Size(122, 18);
            this.layoutControlItem3.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gridDonViVT;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 80);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(1116, 411);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnXacNhan;
            this.layoutControlItem7.Location = new System.Drawing.Point(0, 49);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(148, 31);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // emptySpaceItem3
            // 
            this.emptySpaceItem3.AllowHotTrack = false;
            this.emptySpaceItem3.Location = new System.Drawing.Point(148, 49);
            this.emptySpaceItem3.Name = "emptySpaceItem3";
            this.emptySpaceItem3.Size = new System.Drawing.Size(968, 31);
            this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem1.AppearanceItemCaption.ForeColor = System.Drawing.Color.Blue;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem1.Control = this.txtTiLe;
            this.layoutControlItem1.Location = new System.Drawing.Point(518, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(121, 49);
            this.layoutControlItem1.Text = "Tỉ Lệ";
            this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(38, 18);
            // 
            // emptySpaceItem5
            // 
            this.emptySpaceItem5.AllowHotTrack = false;
            this.emptySpaceItem5.Location = new System.Drawing.Point(508, 0);
            this.emptySpaceItem5.Name = "emptySpaceItem5";
            this.emptySpaceItem5.Size = new System.Drawing.Size(10, 49);
            this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(639, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(477, 49);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1268, 4);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 28);
            this.button1.TabIndex = 16;
            this.button1.Text = "Button1";
            this.button1.Visible = false;
            // 
            // frmERPDonViVT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1136, 547);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.layoutControl2);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControl1);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.LookAndFeel.SkinName = "Office 2010 Blue";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmERPDonViVT";
            this.Text = "Đơn Vị Vật Tư";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridDonViVT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDonViVT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemChkLTSoLe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTendvvt.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTiLe.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).EndInit();
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
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraEditors.SimpleButton btnXacNhan;
        private DevExpress.XtraGrid.GridControl gridDonViVT;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDonViVT;
        private DevExpress.XtraGrid.Columns.GridColumn colID;
        private DevExpress.XtraGrid.Columns.GridColumn colMaDVVT;
        private DevExpress.XtraGrid.Columns.GridColumn colTenDVVT;
        private DevExpress.XtraGrid.Columns.GridColumn colGhiChu;
        private DevExpress.XtraEditors.TextEdit txtTendvvt;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem5;
        private DevExpress.XtraEditors.SimpleButton button1;
        private DevExpress.XtraGrid.Columns.GridColumn colTiLe;
        private DevExpress.XtraEditors.TextEdit txtTiLe;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Columns.GridColumn colQDSoLe;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit ItemChkLTSoLe;
        private DevExpress.XtraBars.BarButtonItem btnSubmit;
        #region Thoai
        private DevExpress.XtraGrid.Columns.GridColumn colNguoiTao;
        private DevExpress.XtraGrid.Columns.GridColumn colNgayTao;
        private DevExpress.XtraGrid.Columns.GridColumn colNguoiSua;
        private DevExpress.XtraGrid.Columns.GridColumn colNgaySua;
        #endregion
    }
}