
namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    partial class frmERPCangNK
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmERPCangNK));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gCThuVien = new DevExpress.XtraGrid.GridControl();
            this.gVThuVien = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.btnThem = new DevExpress.XtraBars.BarButtonItem();
            this.btnLuu = new DevExpress.XtraBars.BarButtonItem();
            this.btnXoa = new DevExpress.XtraBars.BarButtonItem();
            this.btnNapLai = new DevExpress.XtraBars.BarButtonItem();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl11 = new DevExpress.XtraBars.BarDockControl();
            this.barButtonItem27 = new DevExpress.XtraBars.BarButtonItem();
            this.barCheckItem2 = new DevExpress.XtraBars.BarCheckItem();
            this.btnTVKH = new DevExpress.XtraBars.BarButtonItem();
            this.btnTVMH = new DevExpress.XtraBars.BarButtonItem();
            this.btnTVVT = new DevExpress.XtraBars.BarButtonItem();
            this.btnTVMVT = new DevExpress.XtraBars.BarButtonItem();
            this.btnTVKV = new DevExpress.XtraBars.BarButtonItem();
            this.btnTVDV = new DevExpress.XtraBars.BarButtonItem();
            this.btnTVNhomNPL = new DevExpress.XtraBars.BarButtonItem();
            this.checkEditCopyMH = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gCThuVien)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gVThuVien)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditCopyMH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gCThuVien);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 36);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(926, 510);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gCThuVien
            // 
            this.gCThuVien.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.gCThuVien.Location = new System.Drawing.Point(12, 12);
            this.gCThuVien.MainView = this.gVThuVien;
            this.gCThuVien.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gCThuVien.MenuManager = this.barManager1;
            this.gCThuVien.Name = "gCThuVien";
            this.gCThuVien.Size = new System.Drawing.Size(902, 486);
            this.gCThuVien.TabIndex = 4;
            this.gCThuVien.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gVThuVien});
            this.gCThuVien.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.gC_ProcessGridKey);
            // 
            // gVThuVien
            // 
            this.gVThuVien.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6});
            this.gVThuVien.DetailHeight = 431;
            this.gVThuVien.GridControl = this.gCThuVien;
            this.gVThuVien.Name = "gVThuVien";
            this.gVThuVien.OptionsView.ColumnAutoWidth = false;
            this.gVThuVien.OptionsView.ShowAutoFilterRow = true;
            this.gVThuVien.OptionsView.ShowGroupPanel = false;
            this.gVThuVien.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gVThuVien_CellValueChanged);
            this.gVThuVien.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gVThuVien_CustomUnboundColumnData);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.gridColumn1.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumn1.AppearanceHeader.Options.UseFont = true;
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn1.Caption = "ID";
            this.gridColumn1.FieldName = "ID";
            this.gridColumn1.MinWidth = 23;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Width = 87;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.gridColumn2.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.gridColumn2.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumn2.AppearanceHeader.Options.UseFont = true;
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn2.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn2.Caption = "MaCang";
            this.gridColumn2.FieldName = "MaCang";
            this.gridColumn2.MinWidth = 23;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Width = 87;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.gridColumn3.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.gridColumn3.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumn3.AppearanceHeader.Options.UseFont = true;
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn3.Caption = "Cảng";
            this.gridColumn3.FieldName = "TenCang";
            this.gridColumn3.MinWidth = 23;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 1;
            this.gridColumn3.Width = 196;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.gridColumn4.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.gridColumn4.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumn4.AppearanceHeader.Options.UseFont = true;
            this.gridColumn4.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn4.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn4.Caption = "Địa chỉ";
            this.gridColumn4.FieldName = "DiaChi";
            this.gridColumn4.MinWidth = 23;
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 2;
            this.gridColumn4.Width = 240;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.gridColumn5.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumn5.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn5.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn5.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn5.Caption = "Ghi chú";
            this.gridColumn5.FieldName = "GhiChu";
            this.gridColumn5.MinWidth = 24;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 3;
            this.gridColumn5.Width = 141;
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.gridColumn6.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.gridColumn6.AppearanceHeader.Options.UseBackColor = true;
            this.gridColumn6.AppearanceHeader.Options.UseFont = true;
            this.gridColumn6.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridColumn6.Caption = "STT";
            this.gridColumn6.FieldName = "STT";
            this.gridColumn6.MinWidth = 24;
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 0;
            this.gridColumn6.Width = 108;
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
            this.barButtonItem27,
            this.barCheckItem2,
            this.btnTVKH,
            this.btnTVMH,
            this.btnTVVT,
            this.btnTVMVT,
            this.btnTVKV,
            this.btnTVDV,
            this.btnTVNhomNPL,
            this.btnThem,
            this.btnLuu,
            this.btnXoa,
            this.btnNapLai});
            this.barManager1.MaxItemId = 24;
            this.barManager1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.checkEditCopyMH});
            // 
            // bar2
            // 
            this.bar2.BarName = "Tools";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnThem),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnLuu),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnXoa),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnNapLai)});
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.RotateWhenVertical = false;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Tools";
            // 
            // btnThem
            // 
            this.btnThem.Caption = "Thêm(Ctrl+ Shift+ N)";
            this.btnThem.Id = 20;
            this.btnThem.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnThem.ImageOptions.Image")));
            this.btnThem.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnThem.ImageOptions.LargeImage")));
            this.btnThem.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.N));
            this.btnThem.Name = "btnThem";
            this.btnThem.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnThem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnThem_ItemClick);
            // 
            // btnLuu
            // 
            this.btnLuu.Caption = "Lưu(Ctrl+ S)";
            this.btnLuu.Id = 21;
            this.btnLuu.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnLuu.ImageOptions.Image")));
            this.btnLuu.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnLuu.ImageOptions.LargeImage")));
            this.btnLuu.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnLuu.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnLuu_ItemClick);
            // 
            // btnXoa
            // 
            this.btnXoa.Caption = "Xóa(Delete)";
            this.btnXoa.Id = 22;
            this.btnXoa.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnXoa.ImageOptions.Image")));
            this.btnXoa.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnXoa.ImageOptions.LargeImage")));
            this.btnXoa.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.Delete);
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnXoa.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnXoa_ItemClick);
            // 
            // btnNapLai
            // 
            this.btnNapLai.Caption = "Nạp lại(F5)";
            this.btnNapLai.Id = 23;
            this.btnNapLai.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNapLai.ImageOptions.Image")));
            this.btnNapLai.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnNapLai.ImageOptions.LargeImage")));
            this.btnNapLai.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F5);
            this.btnNapLai.Name = "btnNapLai";
            this.btnNapLai.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnNapLai.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnNapLai_ItemClick);
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
            this.barDockControlTop.Size = new System.Drawing.Size(926, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 546);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(926, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 510);
            // 
            // barDockControl11
            // 
            this.barDockControl11.CausesValidation = false;
            this.barDockControl11.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl11.Location = new System.Drawing.Point(926, 36);
            this.barDockControl11.Manager = this.barManager1;
            this.barDockControl11.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControl11.Size = new System.Drawing.Size(0, 510);
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
            // 
            // barCheckItem2
            // 
            this.barCheckItem2.Caption = "Nhập/Import";
            this.barCheckItem2.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
            this.barCheckItem2.Id = 5;
            this.barCheckItem2.Name = "barCheckItem2";
            this.barCheckItem2.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btnTVKH
            // 
            this.btnTVKH.Caption = "Khách hàng";
            this.btnTVKH.Id = 12;
            this.btnTVKH.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnTVKH.ImageOptions.Image")));
            this.btnTVKH.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnTVKH.ImageOptions.LargeImage")));
            this.btnTVKH.Name = "btnTVKH";
            // 
            // btnTVMH
            // 
            this.btnTVMH.Caption = "Mã hàng";
            this.btnTVMH.Id = 13;
            this.btnTVMH.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnTVMH.ImageOptions.Image")));
            this.btnTVMH.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnTVMH.ImageOptions.LargeImage")));
            this.btnTVMH.Name = "btnTVMH";
            // 
            // btnTVVT
            // 
            this.btnTVVT.Caption = "Vật tư";
            this.btnTVVT.Id = 14;
            this.btnTVVT.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnTVVT.ImageOptions.Image")));
            this.btnTVVT.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnTVVT.ImageOptions.LargeImage")));
            this.btnTVVT.Name = "btnTVVT";
            // 
            // btnTVMVT
            // 
            this.btnTVMVT.Caption = "Màu vật tư";
            this.btnTVMVT.Id = 15;
            this.btnTVMVT.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnTVMVT.ImageOptions.Image")));
            this.btnTVMVT.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnTVMVT.ImageOptions.LargeImage")));
            this.btnTVMVT.Name = "btnTVMVT";
            // 
            // btnTVKV
            // 
            this.btnTVKV.Caption = "Khổ";
            this.btnTVKV.Id = 16;
            this.btnTVKV.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnTVKV.ImageOptions.Image")));
            this.btnTVKV.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnTVKV.ImageOptions.LargeImage")));
            this.btnTVKV.Name = "btnTVKV";
            // 
            // btnTVDV
            // 
            this.btnTVDV.Caption = "Đơn vị";
            this.btnTVDV.Id = 17;
            this.btnTVDV.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnTVDV.ImageOptions.Image")));
            this.btnTVDV.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnTVDV.ImageOptions.LargeImage")));
            this.btnTVDV.Name = "btnTVDV";
            // 
            // btnTVNhomNPL
            // 
            this.btnTVNhomNPL.Caption = "Nhóm NPL";
            this.btnTVNhomNPL.Id = 18;
            this.btnTVNhomNPL.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnTVNhomNPL.ImageOptions.Image")));
            this.btnTVNhomNPL.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnTVNhomNPL.ImageOptions.LargeImage")));
            this.btnTVNhomNPL.Name = "btnTVNhomNPL";
            // 
            // checkEditCopyMH
            // 
            this.checkEditCopyMH.AutoHeight = false;
            this.checkEditCopyMH.Name = "checkEditCopyMH";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(926, 510);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gCThuVien;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(906, 490);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(583, 0);
            this.simpleButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(87, 28);
            this.simpleButton1.TabIndex = 5;
            this.simpleButton1.Text = "simpleButton1";
            this.simpleButton1.Visible = false;
            // 
            // frmERPCangNK
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 546);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControl11);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.LookAndFeel.SkinName = "Office 2010 Blue";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmERPCangNK";
            this.Text = "Cảng nhập kho";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gCThuVien)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gVThuVien)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditCopyMH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarButtonItem btnTVKH;
        private DevExpress.XtraBars.BarButtonItem btnTVMH;
        private DevExpress.XtraBars.BarButtonItem btnTVNhomNPL;
        private DevExpress.XtraBars.BarButtonItem btnTVVT;
        private DevExpress.XtraBars.BarButtonItem btnTVMVT;
        private DevExpress.XtraBars.BarButtonItem btnTVKV;
        private DevExpress.XtraBars.BarButtonItem btnTVDV;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit checkEditCopyMH;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControl11;
        private DevExpress.XtraBars.BarButtonItem barButtonItem27;
        private DevExpress.XtraBars.BarCheckItem barCheckItem2;
        private DevExpress.XtraGrid.GridControl gCThuVien;
        private DevExpress.XtraGrid.Views.Grid.GridView gVThuVien;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.BarButtonItem btnThem;
        private DevExpress.XtraBars.BarButtonItem btnLuu;
        private DevExpress.XtraBars.BarButtonItem btnXoa;
        private DevExpress.XtraBars.BarButtonItem btnNapLai;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
    }
}