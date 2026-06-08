
namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    partial class frmTongQuanKiemDauChuyen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTongQuanKiemDauChuyen));
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.btnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btnExportExcel = new DevExpress.XtraBars.BarButtonItem();
            this.btnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.grcTongQuan = new DevExpress.XtraGrid.GridControl();
            this.grvTongQuan = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMaQC_DongThung = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaLenh = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStyle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colChuyen = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCanNang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPO = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSoLop = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ColLoaiThung = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGhiChu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNguoiKhacPhuc = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNgay = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNgayTao = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNguoiSua = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colChiTiet = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoChiTietQC = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.repoDonVi = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.repositoryItemSearchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grcTongQuan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvTongQuan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoChiTietQC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDonVi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
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
            this.btnExportExcel,
            this.btnAdd,
            this.btnRefresh});
            this.barManager1.MaxItemId = 9;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnAdd, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnExportExcel, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnRefresh)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // btnAdd
            // 
            this.btnAdd.Caption = "Thêm (Ctrl + Shift + N)";
            this.btnAdd.Id = 7;
            this.btnAdd.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnAdd.ImageOptions.SvgImage")));
            this.btnAdd.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.N));
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAdd_ItemClick);
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Caption = "Export Excel";
            this.btnExportExcel.Id = 5;
            this.btnExportExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnExportExcel.ImageOptions.Image")));
            this.btnExportExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnExportExcel.ImageOptions.LargeImage")));
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExportExcel_ItemClick);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Caption = "Nạp lại";
            this.btnRefresh.Id = 8;
            this.btnRefresh.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.ImageOptions.Image")));
            this.btnRefresh.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnRefresh.ImageOptions.LargeImage")));
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRefresh_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlTop.Size = new System.Drawing.Size(1503, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 597);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(1503, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 561);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1503, 36);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 561);
            // 
            // barAndDockingController1
            // 
            this.barAndDockingController1.LookAndFeel.SkinMaskColor = System.Drawing.Color.Lime;
            this.barAndDockingController1.LookAndFeel.SkinName = "Office 2010 Blue";
            this.barAndDockingController1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.grcTongQuan);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2});
            this.layoutControl1.Location = new System.Drawing.Point(0, 36);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1503, 561);
            this.layoutControl1.TabIndex = 5;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // grcTongQuan
            // 
            this.grcTongQuan.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.grcTongQuan.Location = new System.Drawing.Point(8, 8);
            this.grcTongQuan.MainView = this.grvTongQuan;
            this.grcTongQuan.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grcTongQuan.MenuManager = this.barManager1;
            this.grcTongQuan.Name = "grcTongQuan";
            this.grcTongQuan.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoDonVi,
            this.repoChiTietQC});
            this.grcTongQuan.Size = new System.Drawing.Size(1487, 545);
            this.grcTongQuan.TabIndex = 4;
            this.grcTongQuan.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvTongQuan});
            // 
            // grvTongQuan
            // 
            this.grvTongQuan.Appearance.FocusedRow.BackColor = System.Drawing.Color.DodgerBlue;
            this.grvTongQuan.Appearance.FocusedRow.ForeColor = System.Drawing.Color.Transparent;
            this.grvTongQuan.Appearance.FocusedRow.Options.UseBackColor = true;
            this.grvTongQuan.Appearance.FocusedRow.Options.UseForeColor = true;
            this.grvTongQuan.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.grvTongQuan.Appearance.HeaderPanel.Options.UseFont = true;
            this.grvTongQuan.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.grvTongQuan.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grvTongQuan.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.grvTongQuan.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.RoyalBlue;
            this.grvTongQuan.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.White;
            this.grvTongQuan.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.grvTongQuan.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.grvTongQuan.ColumnPanelRowHeight = 30;
            this.grvTongQuan.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMaQC_DongThung,
            this.colMaLenh,
            this.colStyle,
            this.colChuyen,
            this.colSize,
            this.colCanNang,
            this.colPO,
            this.colSTT,
            this.colSoLop,
            this.ColLoaiThung,
            this.colGhiChu,
            this.colNguoiKhacPhuc,
            this.colNgay,
            this.colNgayTao,
            this.colNguoiSua,
            this.colChiTiet});
            this.grvTongQuan.DetailHeight = 431;
            this.grvTongQuan.GridControl = this.grcTongQuan;
            this.grvTongQuan.Name = "grvTongQuan";
            this.grvTongQuan.OptionsCustomization.AllowColumnMoving = false;
            this.grvTongQuan.OptionsCustomization.AllowFilter = false;
            this.grvTongQuan.OptionsCustomization.AllowSort = false;
            this.grvTongQuan.OptionsDetail.SmartDetailHeight = true;
            this.grvTongQuan.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
            this.grvTongQuan.OptionsView.ShowAutoFilterRow = true;
            this.grvTongQuan.OptionsView.ShowGroupPanel = false;
            this.grvTongQuan.RowHeight = 35;
            // 
            // colMaQC_DongThung
            // 
            this.colMaQC_DongThung.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colMaQC_DongThung.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colMaQC_DongThung.AppearanceHeader.Options.UseBackColor = true;
            this.colMaQC_DongThung.AppearanceHeader.Options.UseFont = true;
            this.colMaQC_DongThung.Caption = "Người tạo";
            this.colMaQC_DongThung.FieldName = "CreateUser";
            this.colMaQC_DongThung.MinWidth = 23;
            this.colMaQC_DongThung.Name = "colMaQC_DongThung";
            this.colMaQC_DongThung.OptionsColumn.AllowEdit = false;
            this.colMaQC_DongThung.OptionsColumn.ReadOnly = true;
            this.colMaQC_DongThung.Visible = true;
            this.colMaQC_DongThung.VisibleIndex = 5;
            this.colMaQC_DongThung.Width = 133;
            // 
            // colMaLenh
            // 
            this.colMaLenh.AppearanceCell.Options.UseTextOptions = true;
            this.colMaLenh.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaLenh.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colMaLenh.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colMaLenh.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colMaLenh.AppearanceHeader.Options.UseBackColor = true;
            this.colMaLenh.AppearanceHeader.Options.UseFont = true;
            this.colMaLenh.AppearanceHeader.Options.UseTextOptions = true;
            this.colMaLenh.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaLenh.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colMaLenh.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colMaLenh.Caption = "Mã lệnh";
            this.colMaLenh.FieldName = "LenhSX";
            this.colMaLenh.MinWidth = 23;
            this.colMaLenh.Name = "colMaLenh";
            this.colMaLenh.OptionsColumn.AllowEdit = false;
            this.colMaLenh.OptionsColumn.ReadOnly = true;
            this.colMaLenh.Visible = true;
            this.colMaLenh.VisibleIndex = 2;
            this.colMaLenh.Width = 74;
            // 
            // colStyle
            // 
            this.colStyle.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colStyle.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colStyle.AppearanceHeader.Options.UseBackColor = true;
            this.colStyle.AppearanceHeader.Options.UseFont = true;
            this.colStyle.Caption = "Style";
            this.colStyle.FieldName = "MaHang";
            this.colStyle.MinWidth = 23;
            this.colStyle.Name = "colStyle";
            this.colStyle.OptionsColumn.AllowEdit = false;
            this.colStyle.OptionsColumn.ReadOnly = true;
            this.colStyle.Visible = true;
            this.colStyle.VisibleIndex = 1;
            this.colStyle.Width = 207;
            // 
            // colChuyen
            // 
            this.colChuyen.AppearanceCell.Options.UseTextOptions = true;
            this.colChuyen.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colChuyen.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colChuyen.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colChuyen.AppearanceHeader.Options.UseBackColor = true;
            this.colChuyen.AppearanceHeader.Options.UseFont = true;
            this.colChuyen.AppearanceHeader.Options.UseTextOptions = true;
            this.colChuyen.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colChuyen.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colChuyen.Caption = "Chuyền";
            this.colChuyen.FieldName = "Name";
            this.colChuyen.MinWidth = 23;
            this.colChuyen.Name = "colChuyen";
            this.colChuyen.OptionsColumn.AllowEdit = false;
            this.colChuyen.OptionsColumn.ReadOnly = true;
            this.colChuyen.Visible = true;
            this.colChuyen.VisibleIndex = 4;
            this.colChuyen.Width = 81;
            // 
            // colSize
            // 
            this.colSize.AppearanceCell.Options.UseTextOptions = true;
            this.colSize.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colSize.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colSize.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colSize.AppearanceHeader.Options.UseBackColor = true;
            this.colSize.AppearanceHeader.Options.UseFont = true;
            this.colSize.Caption = "Người tạo";
            this.colSize.FieldName = "CreateUser";
            this.colSize.MinWidth = 23;
            this.colSize.Name = "colSize";
            this.colSize.OptionsColumn.AllowEdit = false;
            this.colSize.OptionsColumn.ReadOnly = true;
            this.colSize.Width = 158;
            // 
            // colCanNang
            // 
            this.colCanNang.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colCanNang.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colCanNang.AppearanceHeader.Options.UseBackColor = true;
            this.colCanNang.AppearanceHeader.Options.UseFont = true;
            this.colCanNang.Caption = "Trọng lượng thùng ";
            this.colCanNang.FieldName = "CanNang";
            this.colCanNang.MinWidth = 23;
            this.colCanNang.Name = "colCanNang";
            this.colCanNang.OptionsColumn.AllowEdit = false;
            this.colCanNang.OptionsColumn.ReadOnly = true;
            this.colCanNang.Width = 171;
            // 
            // colPO
            // 
            this.colPO.AppearanceCell.Options.UseTextOptions = true;
            this.colPO.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colPO.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colPO.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colPO.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold);
            this.colPO.AppearanceHeader.Options.UseBackColor = true;
            this.colPO.AppearanceHeader.Options.UseFont = true;
            this.colPO.AppearanceHeader.Options.UseTextOptions = true;
            this.colPO.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colPO.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colPO.Caption = "PO";
            this.colPO.FieldName = "PO";
            this.colPO.MinWidth = 24;
            this.colPO.Name = "colPO";
            this.colPO.OptionsColumn.AllowEdit = false;
            this.colPO.OptionsColumn.ReadOnly = true;
            this.colPO.Visible = true;
            this.colPO.VisibleIndex = 3;
            this.colPO.Width = 136;
            // 
            // colSTT
            // 
            this.colSTT.AppearanceCell.Options.UseTextOptions = true;
            this.colSTT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSTT.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colSTT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colSTT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colSTT.AppearanceHeader.Options.UseBackColor = true;
            this.colSTT.AppearanceHeader.Options.UseFont = true;
            this.colSTT.AppearanceHeader.Options.UseTextOptions = true;
            this.colSTT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSTT.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colSTT.Caption = "STT";
            this.colSTT.FieldName = "STT";
            this.colSTT.MinWidth = 23;
            this.colSTT.Name = "colSTT";
            this.colSTT.OptionsColumn.AllowEdit = false;
            this.colSTT.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colSTT.OptionsColumn.ReadOnly = true;
            this.colSTT.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.colSTT.Width = 52;
            // 
            // colSoLop
            // 
            this.colSoLop.AppearanceCell.Options.UseTextOptions = true;
            this.colSoLop.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSoLop.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colSoLop.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colSoLop.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colSoLop.AppearanceHeader.Options.UseBackColor = true;
            this.colSoLop.AppearanceHeader.Options.UseFont = true;
            this.colSoLop.AppearanceHeader.Options.UseTextOptions = true;
            this.colSoLop.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSoLop.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colSoLop.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colSoLop.Caption = "Ngày tạo";
            this.colSoLop.FieldName = "CreateDate";
            this.colSoLop.MinWidth = 24;
            this.colSoLop.Name = "colSoLop";
            this.colSoLop.OptionsColumn.AllowEdit = false;
            this.colSoLop.OptionsColumn.ReadOnly = true;
            this.colSoLop.Visible = true;
            this.colSoLop.VisibleIndex = 6;
            this.colSoLop.Width = 135;
            // 
            // ColLoaiThung
            // 
            this.ColLoaiThung.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.ColLoaiThung.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.ColLoaiThung.AppearanceHeader.Options.UseBackColor = true;
            this.ColLoaiThung.AppearanceHeader.Options.UseFont = true;
            this.ColLoaiThung.AppearanceHeader.Options.UseTextOptions = true;
            this.ColLoaiThung.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.ColLoaiThung.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.ColLoaiThung.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.ColLoaiThung.Caption = "Trạng thái";
            this.ColLoaiThung.FieldName = "StatusKT";
            this.ColLoaiThung.MinWidth = 24;
            this.ColLoaiThung.Name = "ColLoaiThung";
            this.ColLoaiThung.OptionsColumn.AllowEdit = false;
            this.ColLoaiThung.OptionsColumn.ReadOnly = true;
            this.ColLoaiThung.Visible = true;
            this.ColLoaiThung.VisibleIndex = 7;
            this.ColLoaiThung.Width = 132;
            // 
            // colGhiChu
            // 
            this.colGhiChu.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colGhiChu.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colGhiChu.AppearanceHeader.Options.UseBackColor = true;
            this.colGhiChu.AppearanceHeader.Options.UseFont = true;
            this.colGhiChu.AppearanceHeader.Options.UseTextOptions = true;
            this.colGhiChu.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colGhiChu.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colGhiChu.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colGhiChu.Caption = "Khách hàng";
            this.colGhiChu.FieldName = "KhachHang";
            this.colGhiChu.MinWidth = 24;
            this.colGhiChu.Name = "colGhiChu";
            this.colGhiChu.OptionsColumn.AllowEdit = false;
            this.colGhiChu.OptionsColumn.ReadOnly = true;
            this.colGhiChu.Visible = true;
            this.colGhiChu.VisibleIndex = 0;
            this.colGhiChu.Width = 154;
            // 
            // colNguoiKhacPhuc
            // 
            this.colNguoiKhacPhuc.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colNguoiKhacPhuc.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colNguoiKhacPhuc.AppearanceHeader.Options.UseBackColor = true;
            this.colNguoiKhacPhuc.AppearanceHeader.Options.UseFont = true;
            this.colNguoiKhacPhuc.AppearanceHeader.Options.UseTextOptions = true;
            this.colNguoiKhacPhuc.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colNguoiKhacPhuc.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colNguoiKhacPhuc.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colNguoiKhacPhuc.Caption = "Người khắc phục";
            this.colNguoiKhacPhuc.FieldName = "QCInLine";
            this.colNguoiKhacPhuc.MinWidth = 24;
            this.colNguoiKhacPhuc.Name = "colNguoiKhacPhuc";
            this.colNguoiKhacPhuc.OptionsColumn.AllowEdit = false;
            this.colNguoiKhacPhuc.OptionsColumn.ReadOnly = true;
            this.colNguoiKhacPhuc.Visible = true;
            this.colNguoiKhacPhuc.VisibleIndex = 8;
            this.colNguoiKhacPhuc.Width = 155;
            // 
            // colNgay
            // 
            this.colNgay.AppearanceCell.Options.UseTextOptions = true;
            this.colNgay.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colNgay.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colNgay.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colNgay.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colNgay.AppearanceHeader.Options.UseBackColor = true;
            this.colNgay.AppearanceHeader.Options.UseFont = true;
            this.colNgay.AppearanceHeader.Options.UseTextOptions = true;
            this.colNgay.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colNgay.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colNgay.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colNgay.Caption = "Ngày";
            this.colNgay.MinWidth = 24;
            this.colNgay.Name = "colNgay";
            this.colNgay.OptionsColumn.AllowEdit = false;
            this.colNgay.OptionsColumn.ReadOnly = true;
            this.colNgay.Width = 124;
            // 
            // colNgayTao
            // 
            this.colNgayTao.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colNgayTao.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colNgayTao.AppearanceHeader.Options.UseBackColor = true;
            this.colNgayTao.AppearanceHeader.Options.UseFont = true;
            this.colNgayTao.AppearanceHeader.Options.UseTextOptions = true;
            this.colNgayTao.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colNgayTao.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colNgayTao.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colNgayTao.Caption = "Ngày";
            this.colNgayTao.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.colNgayTao.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colNgayTao.FieldName = "NgayTao";
            this.colNgayTao.MinWidth = 24;
            this.colNgayTao.Name = "colNgayTao";
            this.colNgayTao.OptionsColumn.AllowEdit = false;
            this.colNgayTao.OptionsColumn.ReadOnly = true;
            this.colNgayTao.Width = 94;
            // 
            // colNguoiSua
            // 
            this.colNguoiSua.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colNguoiSua.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colNguoiSua.AppearanceHeader.Options.UseBackColor = true;
            this.colNguoiSua.AppearanceHeader.Options.UseFont = true;
            this.colNguoiSua.AppearanceHeader.Options.UseTextOptions = true;
            this.colNguoiSua.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colNguoiSua.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colNguoiSua.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colNguoiSua.Caption = "Ngày khắc phục";
            this.colNguoiSua.FieldName = "NgayTH";
            this.colNguoiSua.MinWidth = 24;
            this.colNguoiSua.Name = "colNguoiSua";
            this.colNguoiSua.OptionsColumn.AllowEdit = false;
            this.colNguoiSua.OptionsColumn.ReadOnly = true;
            this.colNguoiSua.Visible = true;
            this.colNguoiSua.VisibleIndex = 9;
            this.colNguoiSua.Width = 140;
            // 
            // colChiTiet
            // 
            this.colChiTiet.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colChiTiet.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colChiTiet.AppearanceHeader.Options.UseBackColor = true;
            this.colChiTiet.AppearanceHeader.Options.UseFont = true;
            this.colChiTiet.AppearanceHeader.Options.UseTextOptions = true;
            this.colChiTiet.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colChiTiet.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colChiTiet.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colChiTiet.Caption = "Chi tiết";
            this.colChiTiet.ColumnEdit = this.repoChiTietQC;
            this.colChiTiet.FieldName = "btnChiTietQC";
            this.colChiTiet.MinWidth = 25;
            this.colChiTiet.Name = "colChiTiet";
            this.colChiTiet.OptionsColumn.ReadOnly = true;
            this.colChiTiet.Visible = true;
            this.colChiTiet.VisibleIndex = 10;
            this.colChiTiet.Width = 120;
            // 
            // repoChiTietQC
            // 
            this.repoChiTietQC.AutoHeight = false;
            editorButtonImageOptions1.Image = ((System.Drawing.Image)(resources.GetObject("editorButtonImageOptions1.Image")));
            this.repoChiTietQC.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.repoChiTietQC.Name = "repoChiTietQC";
            this.repoChiTietQC.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repoChiTietQC.Click += new System.EventHandler(this.repoChiTietQC_Click);
            // 
            // repoDonVi
            // 
            this.repoDonVi.AutoHeight = false;
            this.repoDonVi.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repoDonVi.Name = "repoDonVi";
            this.repoDonVi.NullText = "";
            this.repoDonVi.PopupView = this.repositoryItemSearchLookUpEdit1View;
            // 
            // repositoryItemSearchLookUpEdit1View
            // 
            this.repositoryItemSearchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemSearchLookUpEdit1View.Name = "repositoryItemSearchLookUpEdit1View";
            this.repositoryItemSearchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemSearchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 281);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(759, 32);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 6, 6, 6);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1503, 561);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem1.Control = this.grcTongQuan;
            this.layoutControlItem1.CustomizationFormText = " ";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1491, 549);
            this.layoutControlItem1.Text = "Danh mục qui cách đóng thùng";
            this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // frmTongQuanKiemDauChuyen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1503, 597);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmTongQuanKiemDauChuyen";
            this.Text = "Tổng quan duyệt kiểm đầu chuyền";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grcTongQuan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvTongQuan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoChiTietQC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repoDonVi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem btnAdd;
        private DevExpress.XtraBars.BarButtonItem btnExportExcel;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl grcTongQuan;
        private DevExpress.XtraGrid.Views.Grid.GridView grvTongQuan;
        private DevExpress.XtraGrid.Columns.GridColumn colMaQC_DongThung;
        private DevExpress.XtraGrid.Columns.GridColumn colMaLenh;
        private DevExpress.XtraGrid.Columns.GridColumn colStyle;
        private DevExpress.XtraGrid.Columns.GridColumn colChuyen;
        private DevExpress.XtraGrid.Columns.GridColumn colSize;
        private DevExpress.XtraGrid.Columns.GridColumn colCanNang;
        private DevExpress.XtraGrid.Columns.GridColumn colPO;
        private DevExpress.XtraGrid.Columns.GridColumn colSTT;
        private DevExpress.XtraGrid.Columns.GridColumn colSoLop;
        private DevExpress.XtraGrid.Columns.GridColumn ColLoaiThung;
        private DevExpress.XtraGrid.Columns.GridColumn colGhiChu;
        private DevExpress.XtraGrid.Columns.GridColumn colNguoiKhacPhuc;
        private DevExpress.XtraGrid.Columns.GridColumn colNgay;
        private DevExpress.XtraGrid.Columns.GridColumn colNgayTao;
        private DevExpress.XtraGrid.Columns.GridColumn colNguoiSua;
        private DevExpress.XtraGrid.Columns.GridColumn colChiTiet;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repoChiTietQC;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit repoDonVi;
        private DevExpress.XtraGrid.Views.Grid.GridView repositoryItemSearchLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarButtonItem btnRefresh;
    }
}