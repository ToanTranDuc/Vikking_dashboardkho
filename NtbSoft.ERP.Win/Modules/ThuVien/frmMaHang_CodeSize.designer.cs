
namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    partial class frmMaHang_CodeSize
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMaHang_CodeSize));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.grcMaHang = new DevExpress.XtraGrid.GridControl();
            this.grvMaHang = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMaHang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenHang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.grcMaHangCT = new DevExpress.XtraGrid.GridControl();
            this.grvMaHangCT = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMHCT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaMau = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenMau = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSizeID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCodeSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCodeMau = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.Them = new DevExpress.XtraBars.BarButtonItem();
            this.Sua = new DevExpress.XtraBars.BarButtonItem();
            this.Xoa = new DevExpress.XtraBars.BarButtonItem();
            this.Luu = new DevExpress.XtraBars.BarButtonItem();
            this.Naplai = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grcMaHang)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvMaHang)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grcMaHangCT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvMaHangCT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 37);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.layoutControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.layoutControl2);
            this.splitContainer1.Size = new System.Drawing.Size(928, 942);
            this.splitContainer1.SplitterDistance = 221;
            this.splitContainer1.TabIndex = 0;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.grcMaHang);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(221, 942);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // grcMaHang
            // 
            this.grcMaHang.Location = new System.Drawing.Point(12, 12);
            this.grcMaHang.MainView = this.grvMaHang;
            this.grcMaHang.Name = "grcMaHang";
            this.grcMaHang.Size = new System.Drawing.Size(197, 918);
            this.grcMaHang.TabIndex = 4;
            this.grcMaHang.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvMaHang});
            // 
            // grvMaHang
            // 
            this.grvMaHang.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMaHang,
            this.colTenHang});
            this.grvMaHang.GridControl = this.grcMaHang;
            this.grvMaHang.Name = "grvMaHang";
            this.grvMaHang.OptionsView.ShowAutoFilterRow = true;
            this.grvMaHang.OptionsView.ShowGroupPanel = false;
            this.grvMaHang.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridView_CustomDrawColumnHeader);
            this.grvMaHang.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.grvMaHang_RowStyle);
            this.grvMaHang.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grvMaHang_FocusedRowChanged);
            // 
            // colMaHang
            // 
            this.colMaHang.Caption = "Mã Hàng";
            this.colMaHang.FieldName = "MaHang";
            this.colMaHang.MinWidth = 25;
            this.colMaHang.Name = "colMaHang";
            this.colMaHang.Width = 94;
            // 
            // colTenHang
            // 
            this.colTenHang.Caption = "Tên Hàng";
            this.colTenHang.FieldName = "TenHang";
            this.colTenHang.MinWidth = 25;
            this.colTenHang.Name = "colTenHang";
            this.colTenHang.OptionsColumn.AllowEdit = false;
            this.colTenHang.OptionsColumn.ReadOnly = true;
            this.colTenHang.Visible = true;
            this.colTenHang.VisibleIndex = 0;
            this.colTenHang.Width = 177;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(221, 942);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.grcMaHang;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(201, 922);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.grcMaHangCT);
            this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl2.Location = new System.Drawing.Point(0, 0);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.layoutControlGroup1;
            this.layoutControl2.Size = new System.Drawing.Size(703, 942);
            this.layoutControl2.TabIndex = 0;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // grcMaHangCT
            // 
            this.grcMaHangCT.Location = new System.Drawing.Point(12, 12);
            this.grcMaHangCT.MainView = this.grvMaHangCT;
            this.grcMaHangCT.Name = "grcMaHangCT";
            this.grcMaHangCT.Size = new System.Drawing.Size(679, 918);
            this.grcMaHangCT.TabIndex = 4;
            this.grcMaHangCT.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvMaHangCT});
            // 
            // grvMaHangCT
            // 
            this.grvMaHangCT.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMHCT,
            this.colMaMau,
            this.colTenMau,
            this.colSizeID,
            this.colTenSize,
            this.colCodeSize,
            this.colCodeMau});
            this.grvMaHangCT.GridControl = this.grcMaHangCT;
            this.grvMaHangCT.Name = "grvMaHangCT";
            this.grvMaHangCT.OptionsView.ColumnAutoWidth = false;
            this.grvMaHangCT.OptionsView.ShowAutoFilterRow = true;
            this.grvMaHangCT.OptionsView.ShowGroupPanel = false;
            this.grvMaHangCT.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridView_CustomDrawColumnHeader);
            this.grvMaHangCT.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView_CustomDrawRowIndicator);
            this.grvMaHangCT.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.grvMaHangCT_RowStyle);
            this.grvMaHangCT.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grvMaHangCT_FocusedRowChanged);
            this.grvMaHangCT.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.grvMaHangCT_FocusedColumnChanged);
            this.grvMaHangCT.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.grvMaHangCT_CellValueChanging);
            this.grvMaHangCT.RowCountChanged += new System.EventHandler(this.gridView_RowCountChanged);
            this.grvMaHangCT.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.grvMaHangCT_InvalidValueException);
            // 
            // colMHCT
            // 
            this.colMHCT.Caption = "Mã Hàng";
            this.colMHCT.FieldName = "MaHang";
            this.colMHCT.MinWidth = 25;
            this.colMHCT.Name = "colMHCT";
            this.colMHCT.Width = 94;
            // 
            // colMaMau
            // 
            this.colMaMau.Caption = "Mã Màu";
            this.colMaMau.FieldName = "MaMau";
            this.colMaMau.MinWidth = 25;
            this.colMaMau.Name = "colMaMau";
            this.colMaMau.Width = 94;
            // 
            // colTenMau
            // 
            this.colTenMau.Caption = "Màu";
            this.colTenMau.FieldName = "TenMau";
            this.colTenMau.MinWidth = 25;
            this.colTenMau.Name = "colTenMau";
            this.colTenMau.Visible = true;
            this.colTenMau.VisibleIndex = 0;
            this.colTenMau.Width = 127;
            // 
            // colSizeID
            // 
            this.colSizeID.Caption = "Mã Size";
            this.colSizeID.FieldName = "MaSize";
            this.colSizeID.MinWidth = 25;
            this.colSizeID.Name = "colSizeID";
            this.colSizeID.Width = 94;
            // 
            // colTenSize
            // 
            this.colTenSize.Caption = "Size";
            this.colTenSize.FieldName = "TenSize";
            this.colTenSize.MinWidth = 25;
            this.colTenSize.Name = "colTenSize";
            this.colTenSize.Visible = true;
            this.colTenSize.VisibleIndex = 1;
            this.colTenSize.Width = 140;
            // 
            // colCodeSize
            // 
            this.colCodeSize.Caption = "Code Size";
            this.colCodeSize.FieldName = "CodeSize";
            this.colCodeSize.MinWidth = 25;
            this.colCodeSize.Name = "colCodeSize";
            this.colCodeSize.Visible = true;
            this.colCodeSize.VisibleIndex = 2;
            this.colCodeSize.Width = 182;
            // 
            // colCodeMau
            // 
            this.colCodeMau.Caption = "Code Màu";
            this.colCodeMau.FieldName = "CodeMau";
            this.colCodeMau.MinWidth = 25;
            this.colCodeMau.Name = "colCodeMau";
            this.colCodeMau.Width = 94;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2});
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(703, 942);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.grcMaHangCT;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(683, 922);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
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
            this.Naplai});
            this.barManager1.MaxItemId = 5;
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
            new DevExpress.XtraBars.LinkPersistInfo(this.Luu),
            new DevExpress.XtraBars.LinkPersistInfo(this.Naplai)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // Them
            // 
            this.Them.Caption = "Thêm (Ctrl + N)";
            this.Them.Id = 0;
            this.Them.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Them.ImageOptions.SvgImage")));
            this.Them.Name = "Them";
            this.Them.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
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
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlTop.Size = new System.Drawing.Size(928, 37);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 979);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(928, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 37);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 942);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl1.Location = new System.Drawing.Point(928, 37);
            this.barDockControl1.Manager = this.barManager1;
            this.barDockControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControl1.Size = new System.Drawing.Size(0, 942);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(687, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(116, 25);
            this.button1.TabIndex = 5;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // frmMaHang_CodeSize
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 979);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControl1);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmMaHang_CodeSize";
            this.Text = "Khai báo mã hàng CodeSize";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grcMaHang)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvMaHang)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grcMaHangCT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvMaHangCT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraGrid.GridControl grcMaHang;
        private DevExpress.XtraGrid.Views.Grid.GridView grvMaHang;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraGrid.GridControl grcMaHangCT;
        private DevExpress.XtraGrid.Views.Grid.GridView grvMaHangCT;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraGrid.Columns.GridColumn colMaHang;
        private DevExpress.XtraGrid.Columns.GridColumn colTenHang;
        private DevExpress.XtraGrid.Columns.GridColumn colMHCT;
        private DevExpress.XtraGrid.Columns.GridColumn colMaMau;
        private DevExpress.XtraGrid.Columns.GridColumn colTenMau;
        private DevExpress.XtraGrid.Columns.GridColumn colSizeID;
        private DevExpress.XtraGrid.Columns.GridColumn colTenSize;
        private DevExpress.XtraGrid.Columns.GridColumn colCodeSize;
        private DevExpress.XtraGrid.Columns.GridColumn colCodeMau;
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
        private System.Windows.Forms.Button button1;
    }
}