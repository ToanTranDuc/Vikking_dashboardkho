
namespace NtbSoft.ERP.Win.Modules.Kho
{
    partial class frmPhieuNhapKho
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPhieuNhapKho));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.btnAddNew = new DevExpress.XtraBars.BarButtonItem();
            this.btnAddorEdit = new DevExpress.XtraBars.BarButtonItem();
            this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
            this.btRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.tuNgay = new DevExpress.XtraEditors.DateEdit();
            this.denNgay = new DevExpress.XtraEditors.DateEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.grcSoPhieu = new DevExpress.XtraGrid.GridControl();
            this.grvSoPhieu = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSoTo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNgayLap = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSoPhieu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNgayNhap = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grcChiTietSoPhieu = new DevExpress.XtraGrid.GridControl();
            this.grwChiTietSoPhieu = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colTenHang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDotSX = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPO = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colKH = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSLThung = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSLSP = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tuNgay.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tuNgay.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.denNgay.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.denNgay.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grcSoPhieu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvSoPhieu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcChiTietSoPhieu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grwChiTietSoPhieu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
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
            this.btnAddNew,
            this.btnDelete,
            this.btnGopThung,
            this.btnAddorEdit,
            this.btnPKLPO});
            this.barManager1.MaxItemId = 11;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAddNew),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAddorEdit),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnDelete),
            new DevExpress.XtraBars.LinkPersistInfo(this.btRefresh)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // btnAddNew
            // 
            this.btnAddNew.Caption = "Thêm Phiếu(Ctrl+Shift+N)";
            this.btnAddNew.Id = 5;
            this.btnAddNew.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAddNew.ImageOptions.Image")));
            this.btnAddNew.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnAddNew.ImageOptions.LargeImage")));
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnAddNew.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAddNew_ItemClick);
            // 
            // btnAddorEdit
            // 
            this.btnAddorEdit.Caption = "Sửa Phiếu(Ctrl+E)";
            this.btnAddorEdit.Id = 8;
            this.btnAddorEdit.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAddorEdit.ImageOptions.Image")));
            this.btnAddorEdit.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnAddorEdit.ImageOptions.LargeImage")));
            this.btnAddorEdit.Name = "btnAddorEdit";
            this.btnAddorEdit.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnAddorEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAddorEdit_ItemClick);
            // 
            // btnDelete
            // 
            this.btnDelete.Caption = "Xóa (Delete)";
            this.btnDelete.Id = 6;
            this.btnDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.Image")));
            this.btnDelete.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.LargeImage")));
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnDelete_ItemClick);
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
            this.barDockControlTop.Size = new System.Drawing.Size(1060, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 702);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(1060, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 666);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1060, 36);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 666);
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
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.tuNgay);
            this.layoutControl1.Controls.Add(this.denNgay);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.layoutControl1.Location = new System.Drawing.Point(0, 36);
            this.layoutControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1060, 54);
            this.layoutControl1.TabIndex = 4;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // tuNgay
            // 
            this.tuNgay.EditValue = null;
            this.tuNgay.Location = new System.Drawing.Point(88, 12);
            this.tuNgay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tuNgay.MaximumSize = new System.Drawing.Size(175, 0);
            this.tuNgay.MenuManager = this.barManager1;
            this.tuNgay.MinimumSize = new System.Drawing.Size(140, 0);
            this.tuNgay.Name = "tuNgay";
            this.tuNgay.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.tuNgay.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.tuNgay.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.tuNgay.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.tuNgay.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.tuNgay.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.tuNgay.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.tuNgay.Properties.EditValueChanged += new System.EventHandler(this.dateEdit1_Properties_EditValueChanged);
            this.tuNgay.Size = new System.Drawing.Size(174, 22);
            this.tuNgay.StyleController = this.layoutControl1;
            this.tuNgay.TabIndex = 4;
            // 
            // denNgay
            // 
            this.denNgay.EditValue = null;
            this.denNgay.Location = new System.Drawing.Point(342, 12);
            this.denNgay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.denNgay.MaximumSize = new System.Drawing.Size(175, 0);
            this.denNgay.MenuManager = this.barManager1;
            this.denNgay.MinimumSize = new System.Drawing.Size(140, 0);
            this.denNgay.Name = "denNgay";
            this.denNgay.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.denNgay.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.denNgay.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.denNgay.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.denNgay.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.denNgay.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.denNgay.Properties.Mask.EditMask = "dd/MM/yyyy";
            this.denNgay.Properties.EditValueChanged += new System.EventHandler(this.dateEdit2_Properties_EditValueChanged);
            this.denNgay.Size = new System.Drawing.Size(150, 22);
            this.denNgay.StyleController = this.layoutControl1;
            this.denNgay.TabIndex = 5;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem2});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1060, 54);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.layoutControlItem1.AppearanceItemCaption.ForeColor = System.Drawing.Color.Green;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem1.Control = this.tuNgay;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(254, 34);
            this.layoutControlItem1.Text = "Từ ngày";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(73, 18);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.Green;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem2.Control = this.denNgay;
            this.layoutControlItem2.Location = new System.Drawing.Point(254, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(230, 34);
            this.layoutControlItem2.Text = "Đến ngày";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(73, 18);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(484, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(556, 34);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.splitContainerControl1);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 90);
            this.layoutControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup1;
            this.layoutControl2.Size = new System.Drawing.Size(1060, 612);
            this.layoutControl2.TabIndex = 5;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Location = new System.Drawing.Point(12, 12);
            this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.grcSoPhieu);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.grcChiTietSoPhieu);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(1036, 588);
            this.splitContainerControl1.SplitterPosition = 409;
            this.splitContainerControl1.TabIndex = 4;
            // 
            // grcSoPhieu
            // 
            this.grcSoPhieu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grcSoPhieu.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grcSoPhieu.Location = new System.Drawing.Point(0, 0);
            this.grcSoPhieu.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grcSoPhieu.MainView = this.grvSoPhieu;
            this.grcSoPhieu.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grcSoPhieu.MenuManager = this.barManager1;
            this.grcSoPhieu.Name = "grcSoPhieu";
            this.grcSoPhieu.Size = new System.Drawing.Size(409, 588);
            this.grcSoPhieu.TabIndex = 0;
            this.grcSoPhieu.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvSoPhieu});
            // 
            // grvSoPhieu
            // 
            this.grvSoPhieu.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSoTo,
            this.colNgayLap,
            this.colSoPhieu,
            this.colNgayNhap});
            this.grvSoPhieu.DetailHeight = 431;
            this.grvSoPhieu.GridControl = this.grcSoPhieu;
            this.grvSoPhieu.Name = "grvSoPhieu";
            this.grvSoPhieu.OptionsBehavior.Editable = false;
            this.grvSoPhieu.OptionsView.ColumnAutoWidth = false;
            this.grvSoPhieu.OptionsView.ShowGroupPanel = false;
            this.grvSoPhieu.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grvSoPhieu_FocusedRowChanged);
            // 
            // colSoTo
            // 
            this.colSoTo.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colSoTo.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colSoTo.AppearanceHeader.Options.UseBackColor = true;
            this.colSoTo.AppearanceHeader.Options.UseFont = true;
            this.colSoTo.AppearanceHeader.Options.UseTextOptions = true;
            this.colSoTo.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSoTo.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colSoTo.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colSoTo.Caption = "Tờ số .No";
            this.colSoTo.FieldName = "SoTo";
            this.colSoTo.MinWidth = 23;
            this.colSoTo.Name = "colSoTo";
            this.colSoTo.Visible = true;
            this.colSoTo.VisibleIndex = 1;
            this.colSoTo.Width = 107;
            // 
            // colNgayLap
            // 
            this.colNgayLap.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colNgayLap.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colNgayLap.AppearanceHeader.Options.UseBackColor = true;
            this.colNgayLap.AppearanceHeader.Options.UseFont = true;
            this.colNgayLap.AppearanceHeader.Options.UseTextOptions = true;
            this.colNgayLap.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colNgayLap.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colNgayLap.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colNgayLap.Caption = "Ngày lập phiếu";
            this.colNgayLap.FieldName = "NgayLapPhieu";
            this.colNgayLap.MinWidth = 23;
            this.colNgayLap.Name = "colNgayLap";
            this.colNgayLap.Visible = true;
            this.colNgayLap.VisibleIndex = 2;
            this.colNgayLap.Width = 140;
            // 
            // colSoPhieu
            // 
            this.colSoPhieu.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colSoPhieu.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colSoPhieu.AppearanceHeader.Options.UseBackColor = true;
            this.colSoPhieu.AppearanceHeader.Options.UseFont = true;
            this.colSoPhieu.AppearanceHeader.Options.UseTextOptions = true;
            this.colSoPhieu.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSoPhieu.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colSoPhieu.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colSoPhieu.Caption = "Số phiếu";
            this.colSoPhieu.FieldName = "MaPhieu";
            this.colSoPhieu.MinWidth = 23;
            this.colSoPhieu.Name = "colSoPhieu";
            this.colSoPhieu.Visible = true;
            this.colSoPhieu.VisibleIndex = 0;
            this.colSoPhieu.Width = 127;
            // 
            // colNgayNhap
            // 
            this.colNgayNhap.Caption = "Ngày nhâp kho";
            this.colNgayNhap.FieldName = "NgayNhapKho";
            this.colNgayNhap.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            this.colNgayNhap.MinWidth = 23;
            this.colNgayNhap.Name = "colNgayNhap";
            this.colNgayNhap.Width = 87;
            // 
            // grcChiTietSoPhieu
            // 
            this.grcChiTietSoPhieu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grcChiTietSoPhieu.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grcChiTietSoPhieu.Location = new System.Drawing.Point(0, 0);
            this.grcChiTietSoPhieu.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grcChiTietSoPhieu.MainView = this.grwChiTietSoPhieu;
            this.grcChiTietSoPhieu.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grcChiTietSoPhieu.MenuManager = this.barManager1;
            this.grcChiTietSoPhieu.Name = "grcChiTietSoPhieu";
            this.grcChiTietSoPhieu.Size = new System.Drawing.Size(618, 588);
            this.grcChiTietSoPhieu.TabIndex = 0;
            this.grcChiTietSoPhieu.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grwChiTietSoPhieu});
            // 
            // grwChiTietSoPhieu
            // 
            this.grwChiTietSoPhieu.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colTenHang,
            this.colDotSX,
            this.colPO,
            this.colKH,
            this.colSLThung,
            this.colSLSP});
            this.grwChiTietSoPhieu.DetailHeight = 431;
            this.grwChiTietSoPhieu.GridControl = this.grcChiTietSoPhieu;
            this.grwChiTietSoPhieu.Name = "grwChiTietSoPhieu";
            this.grwChiTietSoPhieu.OptionsBehavior.Editable = false;
            this.grwChiTietSoPhieu.OptionsView.AllowCellMerge = true;
            this.grwChiTietSoPhieu.OptionsView.ColumnAutoWidth = false;
            this.grwChiTietSoPhieu.OptionsView.ShowFooter = true;
            this.grwChiTietSoPhieu.OptionsView.ShowGroupPanel = false;
            // 
            // colTenHang
            // 
            this.colTenHang.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colTenHang.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colTenHang.AppearanceHeader.Options.UseBackColor = true;
            this.colTenHang.AppearanceHeader.Options.UseFont = true;
            this.colTenHang.AppearanceHeader.Options.UseTextOptions = true;
            this.colTenHang.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colTenHang.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colTenHang.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colTenHang.Caption = "Tên hàng";
            this.colTenHang.FieldName = "MaHang";
            this.colTenHang.MinWidth = 23;
            this.colTenHang.Name = "colTenHang";
            this.colTenHang.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colTenHang.Visible = true;
            this.colTenHang.VisibleIndex = 0;
            this.colTenHang.Width = 87;
            // 
            // colDotSX
            // 
            this.colDotSX.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colDotSX.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colDotSX.AppearanceHeader.Options.UseBackColor = true;
            this.colDotSX.AppearanceHeader.Options.UseFont = true;
            this.colDotSX.AppearanceHeader.Options.UseTextOptions = true;
            this.colDotSX.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colDotSX.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colDotSX.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colDotSX.Caption = "Đợt SX";
            this.colDotSX.FieldName = "DotSX";
            this.colDotSX.MinWidth = 23;
            this.colDotSX.Name = "colDotSX";
            this.colDotSX.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colDotSX.Visible = true;
            this.colDotSX.VisibleIndex = 3;
            this.colDotSX.Width = 87;
            // 
            // colPO
            // 
            this.colPO.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colPO.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colPO.AppearanceHeader.Options.UseBackColor = true;
            this.colPO.AppearanceHeader.Options.UseFont = true;
            this.colPO.AppearanceHeader.Options.UseTextOptions = true;
            this.colPO.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colPO.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colPO.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colPO.Caption = "PO";
            this.colPO.FieldName = "PO";
            this.colPO.MinWidth = 23;
            this.colPO.Name = "colPO";
            this.colPO.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colPO.Visible = true;
            this.colPO.VisibleIndex = 2;
            this.colPO.Width = 87;
            // 
            // colKH
            // 
            this.colKH.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colKH.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colKH.AppearanceHeader.Options.UseBackColor = true;
            this.colKH.AppearanceHeader.Options.UseFont = true;
            this.colKH.AppearanceHeader.Options.UseTextOptions = true;
            this.colKH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colKH.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colKH.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colKH.Caption = "Khách hàng";
            this.colKH.FieldName = "KhachHang";
            this.colKH.MinWidth = 23;
            this.colKH.Name = "colKH";
            this.colKH.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colKH.Visible = true;
            this.colKH.VisibleIndex = 1;
            this.colKH.Width = 96;
            // 
            // colSLThung
            // 
            this.colSLThung.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colSLThung.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colSLThung.AppearanceHeader.Options.UseBackColor = true;
            this.colSLThung.AppearanceHeader.Options.UseFont = true;
            this.colSLThung.AppearanceHeader.Options.UseTextOptions = true;
            this.colSLThung.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSLThung.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colSLThung.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colSLThung.Caption = "SL Thùng";
            this.colSLThung.FieldName = "SLThung";
            this.colSLThung.MinWidth = 23;
            this.colSLThung.Name = "colSLThung";
            this.colSLThung.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colSLThung.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SLThung", "{0.n0}")});
            this.colSLThung.Visible = true;
            this.colSLThung.VisibleIndex = 4;
            this.colSLThung.Width = 87;
            // 
            // colSLSP
            // 
            this.colSLSP.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.colSLSP.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.colSLSP.AppearanceHeader.Options.UseBackColor = true;
            this.colSLSP.AppearanceHeader.Options.UseFont = true;
            this.colSLSP.AppearanceHeader.Options.UseTextOptions = true;
            this.colSLSP.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSLSP.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colSLSP.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.colSLSP.Caption = "SL Sản Phẩm";
            this.colSLSP.FieldName = "SLSP";
            this.colSLSP.MinWidth = 23;
            this.colSLSP.Name = "colSLSP";
            this.colSLSP.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colSLSP.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "SLSP", "{0.n0}")});
            this.colSLSP.Visible = true;
            this.colSLSP.VisibleIndex = 5;
            this.colSLSP.Width = 111;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem6});
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(1060, 612);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.splitContainerControl1;
            this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(1040, 592);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // frmPhieuNhapKho
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 702);
            this.Controls.Add(this.layoutControl2);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.LookAndFeel.SkinName = "London Liquid Sky";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmPhieuNhapKho";
            this.ShowIcon = false;
            this.Text = "Phiếu nhập kho";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tuNgay.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tuNgay.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.denNgay.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.denNgay.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grcSoPhieu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvSoPhieu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcChiTietSoPhieu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grwChiTietSoPhieu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem btRefresh;
        private DevExpress.XtraBars.BarButtonItem btnAddNew;
        private DevExpress.XtraBars.BarButtonItem btnAddorEdit;
        private DevExpress.XtraBars.BarButtonItem btnDelete;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
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
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.DateEdit tuNgay;
        private DevExpress.XtraEditors.DateEdit denNgay;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraGrid.GridControl grcSoPhieu;
        private DevExpress.XtraGrid.Views.Grid.GridView grvSoPhieu;
        private DevExpress.XtraGrid.Columns.GridColumn colSoTo;
        private DevExpress.XtraGrid.Columns.GridColumn colNgayLap;
        private DevExpress.XtraGrid.Columns.GridColumn colSoPhieu;
        private DevExpress.XtraGrid.GridControl grcChiTietSoPhieu;
        private DevExpress.XtraGrid.Views.Grid.GridView grwChiTietSoPhieu;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraGrid.Columns.GridColumn colTenHang;
        private DevExpress.XtraGrid.Columns.GridColumn colDotSX;
        private DevExpress.XtraGrid.Columns.GridColumn colPO;
        private DevExpress.XtraGrid.Columns.GridColumn colKH;
        private DevExpress.XtraGrid.Columns.GridColumn colSLThung;
        private DevExpress.XtraGrid.Columns.GridColumn colSLSP;
        private DevExpress.XtraGrid.Columns.GridColumn colNgayNhap;
    }
}