
namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    partial class frmArtCode_MaHang
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmArtCode_MaHang));
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.Them = new DevExpress.XtraBars.BarButtonItem();
            this.Sua = new DevExpress.XtraBars.BarButtonItem();
            this.Xoa = new DevExpress.XtraBars.BarButtonItem();
            this.Luu = new DevExpress.XtraBars.BarButtonItem();
            this.Naplai = new DevExpress.XtraBars.BarButtonItem();
            this.btnImportExcel = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.grcArtSize = new DevExpress.XtraGrid.GridControl();
            this.grvArt = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaMau = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaHang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCodeMau = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenMau = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGhiChu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSizeID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaterial = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStyleID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEANCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colArtSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colVCD = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colArtStyle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.grcMaHang = new DevExpress.XtraGrid.GridControl();
            this.grvMaHang = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcArtSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvArt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grcMaHang)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvMaHang)).BeginInit();
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
            this.btnImportExcel});
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
            new DevExpress.XtraBars.LinkPersistInfo(this.btnImportExcel)});
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
            // btnImportExcel
            // 
            this.btnImportExcel.Caption = "Import excel";
            this.btnImportExcel.Id = 5;
            this.btnImportExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImportExcel.ImageOptions.Image")));
            this.btnImportExcel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnImportExcel.ImageOptions.LargeImage")));
            this.btnImportExcel.Name = "btnImportExcel";
            this.btnImportExcel.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnImportExcel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnImportExcel_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(903, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 452);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(903, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 423);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl1.Location = new System.Drawing.Point(903, 29);
            this.barDockControl1.Manager = this.barManager1;
            this.barDockControl1.Size = new System.Drawing.Size(0, 423);
            // 
            // grcArtSize
            // 
            this.grcArtSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grcArtSize.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grcArtSize.Location = new System.Drawing.Point(0, 0);
            this.grcArtSize.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grcArtSize.MainView = this.grvArt;
            this.grcArtSize.MenuManager = this.barManager1;
            this.grcArtSize.Name = "grcArtSize";
            this.grcArtSize.Size = new System.Drawing.Size(698, 423);
            this.grcArtSize.TabIndex = 10;
            this.grcArtSize.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvArt});
            // 
            // grvArt
            // 
            this.grvArt.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(209)))));
            this.grvArt.Appearance.FocusedRow.Options.UseBackColor = true;
            this.grvArt.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grvArt.Appearance.HeaderPanel.Options.UseFont = true;
            this.grvArt.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.grvArt.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grvArt.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.grvArt.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colID,
            this.colMaMau,
            this.colMaHang,
            this.colCodeMau,
            this.colTenMau,
            this.colGhiChu,
            this.colSizeID,
            this.colSize,
            this.colMaterial,
            this.colStyleID,
            this.colEANCode,
            this.colArtSize,
            this.colVCD,
            this.colArtStyle});
            this.grvArt.GridControl = this.grcArtSize;
            this.grvArt.Name = "grvArt";
            this.grvArt.OptionsCustomization.AllowColumnMoving = false;
            this.grvArt.OptionsCustomization.AllowFilter = false;
            this.grvArt.OptionsCustomization.AllowSort = false;
            this.grvArt.OptionsView.ColumnAutoWidth = false;
            this.grvArt.OptionsView.ShowAutoFilterRow = true;
            this.grvArt.OptionsView.ShowGroupPanel = false;
            // 
            // colID
            // 
            this.colID.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colID.AppearanceHeader.Options.UseFont = true;
            this.colID.Caption = "ID";
            this.colID.FieldName = "ID";
            this.colID.MinWidth = 17;
            this.colID.Name = "colID";
            this.colID.Width = 65;
            // 
            // colMaMau
            // 
            this.colMaMau.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaMau.AppearanceHeader.Options.UseFont = true;
            this.colMaMau.Caption = "Mã màu";
            this.colMaMau.FieldName = "MaMau";
            this.colMaMau.MinWidth = 17;
            this.colMaMau.Name = "colMaMau";
            this.colMaMau.Width = 107;
            // 
            // colMaHang
            // 
            this.colMaHang.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaHang.AppearanceHeader.Options.UseFont = true;
            this.colMaHang.Caption = "Mã hàng";
            this.colMaHang.FieldName = "MaHang";
            this.colMaHang.MinWidth = 21;
            this.colMaHang.Name = "colMaHang";
            this.colMaHang.Width = 125;
            // 
            // colCodeMau
            // 
            this.colCodeMau.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colCodeMau.AppearanceHeader.Options.UseFont = true;
            this.colCodeMau.Caption = "Code màu";
            this.colCodeMau.FieldName = "MaMau";
            this.colCodeMau.MinWidth = 21;
            this.colCodeMau.Name = "colCodeMau";
            this.colCodeMau.Width = 111;
            // 
            // colTenMau
            // 
            this.colTenMau.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colTenMau.AppearanceHeader.Options.UseFont = true;
            this.colTenMau.Caption = "Màu";
            this.colTenMau.FieldName = "TenMau";
            this.colTenMau.MinWidth = 17;
            this.colTenMau.Name = "colTenMau";
            this.colTenMau.Visible = true;
            this.colTenMau.VisibleIndex = 1;
            this.colTenMau.Width = 123;
            // 
            // colGhiChu
            // 
            this.colGhiChu.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colGhiChu.AppearanceHeader.Options.UseFont = true;
            this.colGhiChu.Caption = "Ghi chú";
            this.colGhiChu.FieldName = "GhiChu";
            this.colGhiChu.MinWidth = 17;
            this.colGhiChu.Name = "colGhiChu";
            this.colGhiChu.Visible = true;
            this.colGhiChu.VisibleIndex = 7;
            this.colGhiChu.Width = 108;
            // 
            // colSizeID
            // 
            this.colSizeID.Caption = "SizeID";
            this.colSizeID.FieldName = "SizeID";
            this.colSizeID.MinWidth = 21;
            this.colSizeID.Name = "colSizeID";
            this.colSizeID.Width = 71;
            // 
            // colSize
            // 
            this.colSize.Caption = "Size";
            this.colSize.FieldName = "Size";
            this.colSize.MinWidth = 21;
            this.colSize.Name = "colSize";
            this.colSize.Visible = true;
            this.colSize.VisibleIndex = 2;
            this.colSize.Width = 81;
            // 
            // colMaterial
            // 
            this.colMaterial.Caption = "Material";
            this.colMaterial.FieldName = "Material";
            this.colMaterial.MinWidth = 21;
            this.colMaterial.Name = "colMaterial";
            this.colMaterial.Visible = true;
            this.colMaterial.VisibleIndex = 0;
            this.colMaterial.Width = 103;
            // 
            // colStyleID
            // 
            this.colStyleID.Caption = "StyleID";
            this.colStyleID.FieldName = "StyleID";
            this.colStyleID.MinWidth = 21;
            this.colStyleID.Name = "colStyleID";
            this.colStyleID.Width = 81;
            // 
            // colEANCode
            // 
            this.colEANCode.Caption = "EAN/UPC-Code";
            this.colEANCode.FieldName = "EANCode";
            this.colEANCode.MinWidth = 21;
            this.colEANCode.Name = "colEANCode";
            this.colEANCode.Visible = true;
            this.colEANCode.VisibleIndex = 3;
            this.colEANCode.Width = 104;
            // 
            // colArtSize
            // 
            this.colArtSize.Caption = "Art size";
            this.colArtSize.FieldName = "ArtSize";
            this.colArtSize.MinWidth = 21;
            this.colArtSize.Name = "colArtSize";
            this.colArtSize.Visible = true;
            this.colArtSize.VisibleIndex = 4;
            this.colArtSize.Width = 93;
            // 
            // colVCD
            // 
            this.colVCD.Caption = "VCD";
            this.colVCD.FieldName = "VCD";
            this.colVCD.MinWidth = 21;
            this.colVCD.Name = "colVCD";
            this.colVCD.Visible = true;
            this.colVCD.VisibleIndex = 6;
            this.colVCD.Width = 81;
            // 
            // colArtStyle
            // 
            this.colArtStyle.Caption = "Art theo Style";
            this.colArtStyle.FieldName = "ArtStyle";
            this.colArtStyle.MinWidth = 21;
            this.colArtStyle.Name = "colArtStyle";
            this.colArtStyle.Visible = true;
            this.colArtStyle.VisibleIndex = 5;
            this.colArtStyle.Width = 96;
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 29);
            this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.grcMaHang);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.grcArtSize);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(903, 423);
            this.splitContainerControl1.SplitterPosition = 200;
            this.splitContainerControl1.TabIndex = 15;
            // 
            // grcMaHang
            // 
            this.grcMaHang.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grcMaHang.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grcMaHang.Location = new System.Drawing.Point(0, 0);
            this.grcMaHang.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grcMaHang.MainView = this.grvMaHang;
            this.grcMaHang.MenuManager = this.barManager1;
            this.grcMaHang.Name = "grcMaHang";
            this.grcMaHang.Size = new System.Drawing.Size(200, 423);
            this.grcMaHang.TabIndex = 11;
            this.grcMaHang.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvMaHang});
            // 
            // grvMaHang
            // 
            this.grvMaHang.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(209)))));
            this.grvMaHang.Appearance.FocusedRow.Options.UseBackColor = true;
            this.grvMaHang.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grvMaHang.Appearance.HeaderPanel.Options.UseFont = true;
            this.grvMaHang.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.grvMaHang.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grvMaHang.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.grvMaHang.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn3});
            this.grvMaHang.GridControl = this.grcMaHang;
            this.grvMaHang.Name = "grvMaHang";
            this.grvMaHang.OptionsCustomization.AllowColumnMoving = false;
            this.grvMaHang.OptionsCustomization.AllowFilter = false;
            this.grvMaHang.OptionsCustomization.AllowSort = false;
            this.grvMaHang.OptionsView.ColumnAutoWidth = false;
            this.grvMaHang.OptionsView.ShowAutoFilterRow = true;
            this.grvMaHang.OptionsView.ShowGroupPanel = false;
            this.grvMaHang.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grvMaHang_FocusedRowChanged);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.gridColumn1.AppearanceHeader.Options.UseFont = true;
            this.gridColumn1.Caption = "ID";
            this.gridColumn1.FieldName = "ID";
            this.gridColumn1.MinWidth = 17;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Width = 65;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.gridColumn3.AppearanceHeader.Options.UseFont = true;
            this.gridColumn3.Caption = "Mã hàng";
            this.gridColumn3.FieldName = "MaHang";
            this.gridColumn3.MinWidth = 21;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 0;
            this.gridColumn3.Width = 170;
            // 
            // frmArtCode_MaHang
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(903, 452);
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControl1);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmArtCode_MaHang";
            this.Text = "Art mã hàng";
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcArtSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvArt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grcMaHang)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvMaHang)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
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
        private DevExpress.XtraGrid.GridControl grcArtSize;
        private DevExpress.XtraGrid.Views.Grid.GridView grvArt;
        private DevExpress.XtraGrid.Columns.GridColumn colID;
        private DevExpress.XtraGrid.Columns.GridColumn colMaMau;
        private DevExpress.XtraGrid.Columns.GridColumn colMaHang;
        private DevExpress.XtraGrid.Columns.GridColumn colCodeMau;
        private DevExpress.XtraGrid.Columns.GridColumn colTenMau;
        private DevExpress.XtraGrid.Columns.GridColumn colGhiChu;
        private DevExpress.XtraGrid.Columns.GridColumn colSizeID;
        private DevExpress.XtraGrid.Columns.GridColumn colSize;
        private DevExpress.XtraGrid.Columns.GridColumn colMaterial;
        private DevExpress.XtraGrid.Columns.GridColumn colStyleID;
        private DevExpress.XtraGrid.Columns.GridColumn colEANCode;
        private DevExpress.XtraGrid.Columns.GridColumn colArtSize;
        private DevExpress.XtraGrid.Columns.GridColumn colVCD;
        private DevExpress.XtraGrid.Columns.GridColumn colArtStyle;
        private DevExpress.XtraBars.BarButtonItem btnImportExcel;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraGrid.GridControl grcMaHang;
        private DevExpress.XtraGrid.Views.Grid.GridView grvMaHang;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
    }
}