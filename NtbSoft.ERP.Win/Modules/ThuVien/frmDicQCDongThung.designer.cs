
namespace NtbSoft.ERP.Win.Modules.DicForm
{
    partial class frmDicQCDongThung
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDicQCDongThung));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.btAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btEdit = new DevExpress.XtraBars.BarButtonItem();
            this.btDelete = new DevExpress.XtraBars.BarButtonItem();
            this.btSave = new DevExpress.XtraBars.BarButtonItem();
            this.btRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.btnImportExcel = new DevExpress.XtraBars.BarButtonItem();
            this.btnTQCartonList = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.button1 = new System.Windows.Forms.Button();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMaQC_DongThung = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenQC_DongThung = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colChieuDai = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colChieuRong = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colChieuCao = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCanNang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDonVi = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repoDonVi = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.repositoryItemSearchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSTT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSoLop = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ColLoaiThung = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGhiChu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGhiChu2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNguoiTao = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNgayTao = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNguoiSua = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNgaySua = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
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
            this.barManager1.Controller = this.barAndDockingController1;
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
            this.btnImportExcel,
            this.barButtonItem1,
            this.btnTQCartonList});
            this.barManager1.MaxItemId = 9;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btAdd),
            new DevExpress.XtraBars.LinkPersistInfo(this.btEdit),
            new DevExpress.XtraBars.LinkPersistInfo(this.btDelete),
            new DevExpress.XtraBars.LinkPersistInfo(this.btSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.btRefresh),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnImportExcel, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnTQCartonList, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // btAdd
            // 
            this.btAdd.Caption = "Thêm (Ctrl + Shift+ N)";
            this.btAdd.Id = 0;
            this.btAdd.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btAdd.ImageOptions.Image")));
            this.btAdd.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btAdd.ImageOptions.LargeImage")));
            this.btAdd.Name = "btAdd";
            this.btAdd.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btAdd_ItemClick);
            // 
            // btEdit
            // 
            this.btEdit.Caption = "Sửa (Ctrl + E)";
            this.btEdit.Id = 1;
            this.btEdit.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btEdit.ImageOptions.Image")));
            this.btEdit.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btEdit.ImageOptions.LargeImage")));
            this.btEdit.Name = "btEdit";
            this.btEdit.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btEdit_ItemClick);
            // 
            // btDelete
            // 
            this.btDelete.Caption = "Xóa (Delete)";
            this.btDelete.Id = 2;
            this.btDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btDelete.ImageOptions.Image")));
            this.btDelete.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btDelete.ImageOptions.LargeImage")));
            this.btDelete.Name = "btDelete";
            this.btDelete.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btDelete_ItemClick);
            // 
            // btSave
            // 
            this.btSave.Caption = "Lưu (Ctrl + S)";
            this.btSave.Id = 3;
            this.btSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btSave.ImageOptions.Image")));
            this.btSave.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btSave.ImageOptions.LargeImage")));
            this.btSave.Name = "btSave";
            this.btSave.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btSave_ItemClick);
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
            // btnImportExcel
            // 
            this.btnImportExcel.Caption = "Import Excel";
            this.btnImportExcel.Id = 5;
            this.btnImportExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImportExcel.ImageOptions.Image")));
            this.btnImportExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnImportExcel.ImageOptions.LargeImage")));
            this.btnImportExcel.Name = "btnImportExcel";
            this.btnImportExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnImportExcel_ItemClick);
            // 
            // btnTQCartonList
            // 
            this.btnTQCartonList.Id = 8;
            this.btnTQCartonList.Name = "btnTQCartonList";
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Id = 6;
            this.barButtonItem1.Name = "barButtonItem1";
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
            this.barDockControlTop.Size = new System.Drawing.Size(1444, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 676);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(1444, 0);
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
            this.barDockControlRight.Location = new System.Drawing.Point(1444, 36);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 640);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.button1);
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2});
            this.layoutControl1.Location = new System.Drawing.Point(0, 36);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1444, 640);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 289);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(755, 28);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // gridControl1
            // 
            this.gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.gridControl1.Location = new System.Drawing.Point(8, 8);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridControl1.MenuManager = this.barManager1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repoDonVi});
            this.gridControl1.Size = new System.Drawing.Size(1428, 624);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.gridControl1.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.gridControl1_ProcessGridKey);
            this.gridControl1.EditorKeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gridControl1_EditorKeyPress);
            // 
            // gridView1
            // 
            this.gridView1.Appearance.FocusedRow.BackColor = System.Drawing.Color.RoyalBlue;
            this.gridView1.Appearance.FocusedRow.ForeColor = System.Drawing.Color.Transparent;
            this.gridView1.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridView1.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gridView1.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.gridView1.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView1.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridView1.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridView1.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridView1.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.RoyalBlue;
            this.gridView1.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.White;
            this.gridView1.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gridView1.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMaQC_DongThung,
            this.colTenQC_DongThung,
            this.colChieuDai,
            this.colChieuRong,
            this.colChieuCao,
            this.colCanNang,
            this.colDonVi,
            this.colSTT,
            this.colSoLop,
            this.ColLoaiThung,
            this.colGhiChu,
            this.colGhiChu2,
            this.colNguoiTao,
            this.colNgayTao,
            this.colNguoiSua,
            this.colNgaySua});
            this.gridView1.DetailHeight = 431;
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsCustomization.AllowColumnMoving = false;
            this.gridView1.OptionsCustomization.AllowFilter = false;
            this.gridView1.OptionsCustomization.AllowSort = false;
            this.gridView1.OptionsSelection.MultiSelect = true;
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.ShowAutoFilterRow = true;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gridView1_PopupMenuShowing);
            this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView1_FocusedRowChanged);
            this.gridView1.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.gridView1_FocusedColumnChanged);
            this.gridView1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanging);
            this.gridView1.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridView1_CustomUnboundColumnData);
            this.gridView1.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridView1_CustomColumnDisplayText);
            // 
            // colMaQC_DongThung
            // 
            this.colMaQC_DongThung.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colMaQC_DongThung.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colMaQC_DongThung.AppearanceHeader.Options.UseBackColor = true;
            this.colMaQC_DongThung.AppearanceHeader.Options.UseFont = true;
            this.colMaQC_DongThung.Caption = "Mã qui cách";
            this.colMaQC_DongThung.FieldName = "MaQuiCach";
            this.colMaQC_DongThung.MinWidth = 23;
            this.colMaQC_DongThung.Name = "colMaQC_DongThung";
            this.colMaQC_DongThung.OptionsColumn.AllowEdit = false;
            this.colMaQC_DongThung.Width = 143;
            // 
            // colTenQC_DongThung
            // 
            this.colTenQC_DongThung.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colTenQC_DongThung.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colTenQC_DongThung.AppearanceHeader.Options.UseBackColor = true;
            this.colTenQC_DongThung.AppearanceHeader.Options.UseFont = true;
            this.colTenQC_DongThung.Caption = "Qui cách thùng";
            this.colTenQC_DongThung.FieldName = "TenQuiCach";
            this.colTenQC_DongThung.MinWidth = 23;
            this.colTenQC_DongThung.Name = "colTenQC_DongThung";
            this.colTenQC_DongThung.OptionsColumn.ReadOnly = true;
            this.colTenQC_DongThung.Visible = true;
            this.colTenQC_DongThung.VisibleIndex = 2;
            this.colTenQC_DongThung.Width = 143;
            // 
            // colChieuDai
            // 
            this.colChieuDai.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colChieuDai.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colChieuDai.AppearanceHeader.Options.UseBackColor = true;
            this.colChieuDai.AppearanceHeader.Options.UseFont = true;
            this.colChieuDai.Caption = "Chiều dài";
            this.colChieuDai.FieldName = "ChieuDai";
            this.colChieuDai.MinWidth = 23;
            this.colChieuDai.Name = "colChieuDai";
            this.colChieuDai.Visible = true;
            this.colChieuDai.VisibleIndex = 5;
            this.colChieuDai.Width = 88;
            // 
            // colChieuRong
            // 
            this.colChieuRong.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colChieuRong.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colChieuRong.AppearanceHeader.Options.UseBackColor = true;
            this.colChieuRong.AppearanceHeader.Options.UseFont = true;
            this.colChieuRong.Caption = "Chiều rộng";
            this.colChieuRong.FieldName = "ChieuRong";
            this.colChieuRong.MinWidth = 23;
            this.colChieuRong.Name = "colChieuRong";
            this.colChieuRong.Visible = true;
            this.colChieuRong.VisibleIndex = 6;
            this.colChieuRong.Width = 98;
            // 
            // colChieuCao
            // 
            this.colChieuCao.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colChieuCao.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colChieuCao.AppearanceHeader.Options.UseBackColor = true;
            this.colChieuCao.AppearanceHeader.Options.UseFont = true;
            this.colChieuCao.Caption = "Chiều cao";
            this.colChieuCao.FieldName = "ChieuCao";
            this.colChieuCao.MinWidth = 23;
            this.colChieuCao.Name = "colChieuCao";
            this.colChieuCao.Visible = true;
            this.colChieuCao.VisibleIndex = 7;
            this.colChieuCao.Width = 88;
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
            this.colCanNang.Width = 171;
            // 
            // colDonVi
            // 
            this.colDonVi.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colDonVi.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold);
            this.colDonVi.AppearanceHeader.Options.UseBackColor = true;
            this.colDonVi.AppearanceHeader.Options.UseFont = true;
            this.colDonVi.Caption = "Đơn vị";
            this.colDonVi.ColumnEdit = this.repoDonVi;
            this.colDonVi.FieldName = "MaDV";
            this.colDonVi.MinWidth = 24;
            this.colDonVi.Name = "colDonVi";
            this.colDonVi.Visible = true;
            this.colDonVi.VisibleIndex = 8;
            this.colDonVi.Width = 61;
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
            // colSTT
            // 
            this.colSTT.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colSTT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colSTT.AppearanceHeader.Options.UseBackColor = true;
            this.colSTT.AppearanceHeader.Options.UseFont = true;
            this.colSTT.Caption = "STT";
            this.colSTT.FieldName = "STT";
            this.colSTT.MinWidth = 23;
            this.colSTT.Name = "colSTT";
            this.colSTT.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.colSTT.Visible = true;
            this.colSTT.VisibleIndex = 0;
            this.colSTT.Width = 42;
            // 
            // colSoLop
            // 
            this.colSoLop.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colSoLop.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colSoLop.AppearanceHeader.Options.UseBackColor = true;
            this.colSoLop.AppearanceHeader.Options.UseFont = true;
            this.colSoLop.AppearanceHeader.Options.UseTextOptions = true;
            this.colSoLop.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSoLop.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colSoLop.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colSoLop.Caption = "Số lớp";
            this.colSoLop.FieldName = "SoLop";
            this.colSoLop.MinWidth = 24;
            this.colSoLop.Name = "colSoLop";
            this.colSoLop.Visible = true;
            this.colSoLop.VisibleIndex = 3;
            this.colSoLop.Width = 67;
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
            this.ColLoaiThung.Caption = "Loại thùng";
            this.ColLoaiThung.FieldName = "LoaiThung";
            this.ColLoaiThung.MinWidth = 24;
            this.ColLoaiThung.Name = "ColLoaiThung";
            this.ColLoaiThung.Visible = true;
            this.ColLoaiThung.VisibleIndex = 4;
            this.ColLoaiThung.Width = 109;
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
            this.colGhiChu.FieldName = "GhiChu";
            this.colGhiChu.MinWidth = 24;
            this.colGhiChu.Name = "colGhiChu";
            this.colGhiChu.Visible = true;
            this.colGhiChu.VisibleIndex = 1;
            this.colGhiChu.Width = 225;
            // 
            // colGhiChu2
            // 
            this.colGhiChu2.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colGhiChu2.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colGhiChu2.AppearanceHeader.Options.UseBackColor = true;
            this.colGhiChu2.AppearanceHeader.Options.UseFont = true;
            this.colGhiChu2.AppearanceHeader.Options.UseTextOptions = true;
            this.colGhiChu2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colGhiChu2.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colGhiChu2.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colGhiChu2.Caption = "Ghi Chú";
            this.colGhiChu2.FieldName = "GhiChu2";
            this.colGhiChu2.MinWidth = 24;
            this.colGhiChu2.Name = "colGhiChu2";
            this.colGhiChu2.Visible = true;
            this.colGhiChu2.VisibleIndex = 9;
            this.colGhiChu2.Width = 353;
            // 
            // colNguoiTao
            // 
            this.colNguoiTao.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colNguoiTao.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colNguoiTao.AppearanceHeader.Options.UseBackColor = true;
            this.colNguoiTao.AppearanceHeader.Options.UseFont = true;
            this.colNguoiTao.AppearanceHeader.Options.UseTextOptions = true;
            this.colNguoiTao.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colNguoiTao.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colNguoiTao.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colNguoiTao.Caption = "Người tạo";
            this.colNguoiTao.FieldName = "NguoiTao";
            this.colNguoiTao.MinWidth = 24;
            this.colNguoiTao.Name = "colNguoiTao";
            this.colNguoiTao.Visible = true;
            this.colNguoiTao.VisibleIndex = 10;
            this.colNguoiTao.Width = 94;
            // 
            // colNgayTao
            // 
            this.colNgayTao.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colNgayTao.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colNgayTao.AppearanceHeader.Options.UseBackColor = true;
            this.colNgayTao.AppearanceHeader.Options.UseFont = true;
            this.colNgayTao.AppearanceHeader.Options.UseTextOptions = true;
            this.colNgayTao.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colNgayTao.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colNgayTao.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colNgayTao.Caption = "Ngày tạo";
            this.colNgayTao.FieldName = "NgayTao";
            this.colNgayTao.MinWidth = 24;
            this.colNgayTao.Name = "colNgayTao";
            this.colNgayTao.Width = 94;
            // 
            // colNguoiSua
            // 
            this.colNguoiSua.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colNguoiSua.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colNguoiSua.AppearanceHeader.Options.UseBackColor = true;
            this.colNguoiSua.AppearanceHeader.Options.UseFont = true;
            this.colNguoiSua.AppearanceHeader.Options.UseTextOptions = true;
            this.colNguoiSua.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colNguoiSua.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colNguoiSua.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colNguoiSua.Caption = "Người sửa";
            this.colNguoiSua.FieldName = "NguoiSua";
            this.colNguoiSua.MinWidth = 24;
            this.colNguoiSua.Name = "colNguoiSua";
            this.colNguoiSua.Visible = true;
            this.colNguoiSua.VisibleIndex = 11;
            this.colNguoiSua.Width = 94;
            // 
            // colNgaySua
            // 
            this.colNgaySua.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colNgaySua.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colNgaySua.AppearanceHeader.Options.UseBackColor = true;
            this.colNgaySua.AppearanceHeader.Options.UseFont = true;
            this.colNgaySua.AppearanceHeader.Options.UseTextOptions = true;
            this.colNgaySua.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colNgaySua.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colNgaySua.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colNgaySua.Caption = "Ngày sửa";
            this.colNgaySua.FieldName = "NgaySua";
            this.colNgaySua.MinWidth = 24;
            this.colNgaySua.Name = "colNgaySua";
            this.colNgaySua.Width = 94;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.button1;
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
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 6, 6, 6);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1444, 640);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem1.Control = this.gridControl1;
            this.layoutControlItem1.CustomizationFormText = " ";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1432, 628);
            this.layoutControlItem1.Text = "Danh mục qui cách đóng thùng";
            this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // frmDicQCDongThung
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1444, 676);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.LookAndFeel.SkinMaskColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmDicQCDongThung";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Carton List";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
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
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem btAdd;
        private DevExpress.XtraBars.BarButtonItem btEdit;
        private DevExpress.XtraBars.BarButtonItem btDelete;
        private DevExpress.XtraBars.BarButtonItem btSave;
        private DevExpress.XtraBars.BarButtonItem btRefresh;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn colMaQC_DongThung;
        private DevExpress.XtraGrid.Columns.GridColumn colTenQC_DongThung;
        private DevExpress.XtraGrid.Columns.GridColumn colChieuDai;
        private DevExpress.XtraGrid.Columns.GridColumn colChieuRong;
        private DevExpress.XtraGrid.Columns.GridColumn colChieuCao;
        private DevExpress.XtraGrid.Columns.GridColumn colCanNang;
        private System.Windows.Forms.Button button1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraGrid.Columns.GridColumn colDonVi;
        private DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit repoDonVi;
        private DevExpress.XtraGrid.Views.Grid.GridView repositoryItemSearchLookUpEdit1View;
        private DevExpress.XtraGrid.Columns.GridColumn colSTT;
        private DevExpress.XtraBars.BarButtonItem btnImportExcel;
        private DevExpress.XtraGrid.Columns.GridColumn colSoLop;
        private DevExpress.XtraGrid.Columns.GridColumn ColLoaiThung;
        private DevExpress.XtraGrid.Columns.GridColumn colGhiChu;
        private DevExpress.XtraGrid.Columns.GridColumn colGhiChu2;
        private DevExpress.XtraGrid.Columns.GridColumn colNguoiTao;
        private DevExpress.XtraGrid.Columns.GridColumn colNgayTao;
        private DevExpress.XtraGrid.Columns.GridColumn colNguoiSua;
        private DevExpress.XtraGrid.Columns.GridColumn colNgaySua;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem btnTQCartonList;
        //private DevExpress.XtraGrid.Columns.GridColumn colCLHID;
    }
}