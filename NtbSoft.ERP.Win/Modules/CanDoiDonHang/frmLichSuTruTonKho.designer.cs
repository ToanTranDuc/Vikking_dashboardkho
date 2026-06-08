
namespace NtbSoft.ERP.Win.Modules.CanDoiDonHang
{
    partial class frmLichSuTruTonKho
    {
        ///// <summary>
        ///// Required designer variable.
        ///// </summary>
        //private System.ComponentModel.IContainer components = null;

        ///// <summary>
        ///// Clean up any resources being used.
        ///// </summary>
        ///// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        //#region Windows Form Designer generated code

        ///// <summary>
        ///// Required method for Designer support - do not modify
        ///// the contents of this method with the code editor.
        ///// </summary>
        //private void InitializeComponent()
        //{
        //    this.components = new System.ComponentModel.Container();
        //    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        //    this.ClientSize = new System.Drawing.Size(800, 450);
        //    this.Text = "frmLichSuTruTonKho";
        //}

        //#endregion

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLichSuTruTonKho));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.Them = new DevExpress.XtraBars.BarButtonItem();
            this.Sua = new DevExpress.XtraBars.BarButtonItem();
            this.Xoa = new DevExpress.XtraBars.BarButtonItem();
            this.Luu = new DevExpress.XtraBars.BarButtonItem();
            this.Naplai = new DevExpress.XtraBars.BarButtonItem();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.gridLichSuTruTK = new DevExpress.XtraGrid.GridControl();
            this.gridViewLichSuTruTK = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaHang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaDH = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDauSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDauSizeID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaMau = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSizeID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSoLuong = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNgayTru = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGhiChu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLichSuTruTK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLichSuTruTK)).BeginInit();
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.None, false, this.Xoa, false),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.None, false, this.Luu, false),
            new DevExpress.XtraBars.LinkPersistInfo(this.Naplai)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // Them
            // 
            this.Them.Caption = "Thêm (F1)";
            this.Them.Id = 0;
            this.Them.Name = "Them";
            this.Them.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // Sua
            // 
            this.Sua.Caption = "Sửa (F2)";
            this.Sua.Id = 1;
            this.Sua.Name = "Sua";
            this.Sua.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // Xoa
            // 
            this.Xoa.Caption = "Xóa (F3)";
            this.Xoa.Id = 2;
            this.Xoa.Name = "Xoa";
            this.Xoa.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // Luu
            // 
            this.Luu.Caption = "Lưu (F4)";
            this.Luu.Id = 3;
            this.Luu.Name = "Luu";
            this.Luu.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
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
            this.barDockControlTop.Size = new System.Drawing.Size(1581, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 965);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(1581, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 929);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl1.Location = new System.Drawing.Point(1581, 36);
            this.barDockControl1.Manager = this.barManager1;
            this.barDockControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControl1.Size = new System.Drawing.Size(0, 929);
            // 
            // gridLichSuTruTK
            // 
            this.gridLichSuTruTK.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLichSuTruTK.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.gridLichSuTruTK.Location = new System.Drawing.Point(0, 36);
            this.gridLichSuTruTK.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridLichSuTruTK.MainView = this.gridViewLichSuTruTK;
            this.gridLichSuTruTK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridLichSuTruTK.MenuManager = this.barManager1;
            this.gridLichSuTruTK.Name = "gridLichSuTruTK";
            this.gridLichSuTruTK.Size = new System.Drawing.Size(1581, 929);
            this.gridLichSuTruTK.TabIndex = 10;
            this.gridLichSuTruTK.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewLichSuTruTK});
            // 
            // gridViewLichSuTruTK
            // 
            this.gridViewLichSuTruTK.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(209)))));
            this.gridViewLichSuTruTK.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewLichSuTruTK.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridViewLichSuTruTK.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewLichSuTruTK.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridViewLichSuTruTK.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridViewLichSuTruTK.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridViewLichSuTruTK.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colID,
            this.colMaHang,
            this.colMaDH,
            this.colDauSize,
            this.colDauSizeID,
            this.colMaMau,
            this.colSize,
            this.colSizeID,
            this.colSoLuong,
            this.colNgayTru,
            this.colGhiChu});
            this.gridViewLichSuTruTK.DetailHeight = 431;
            this.gridViewLichSuTruTK.GridControl = this.gridLichSuTruTK;
            this.gridViewLichSuTruTK.Name = "gridViewLichSuTruTK";
            this.gridViewLichSuTruTK.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewLichSuTruTK.OptionsCustomization.AllowFilter = false;
            this.gridViewLichSuTruTK.OptionsCustomization.AllowSort = false;
            this.gridViewLichSuTruTK.OptionsView.ColumnAutoWidth = false;
            this.gridViewLichSuTruTK.OptionsView.ShowAutoFilterRow = true;
            this.gridViewLichSuTruTK.OptionsView.ShowGroupPanel = false;
            this.gridViewLichSuTruTK.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewTruTK_CustomDrawColumnHeader);
            this.gridViewLichSuTruTK.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridViewLichSuTruTK_CustomDrawRowIndicator);
            this.gridViewLichSuTruTK.RowCountChanged += new System.EventHandler(this.gridViewLichSuTruTK_RowCountChanged);
            // 
            // colID
            // 
            this.colID.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colID.AppearanceCell.Options.UseFont = true;
            this.colID.AppearanceCell.Options.UseTextOptions = true;
            this.colID.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colID.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colID.AppearanceHeader.Options.UseFont = true;
            this.colID.Caption = "ID";
            this.colID.FieldName = "ID";
            this.colID.MinWidth = 24;
            this.colID.Name = "colID";
            this.colID.Width = 87;
            // 
            // colMaHang
            // 
            this.colMaHang.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaHang.AppearanceCell.Options.UseFont = true;
            this.colMaHang.AppearanceCell.Options.UseTextOptions = true;
            this.colMaHang.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaHang.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colMaHang.AppearanceHeader.Options.UseFont = true;
            this.colMaHang.Caption = "Mã hàng";
            this.colMaHang.FieldName = "MaHang";
            this.colMaHang.MinWidth = 24;
            this.colMaHang.Name = "colMaHang";
            this.colMaHang.Width = 143;
            // 
            // colMaDH
            // 
            this.colMaDH.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaDH.AppearanceCell.Options.UseFont = true;
            this.colMaDH.AppearanceCell.Options.UseTextOptions = true;
            this.colMaDH.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaDH.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colMaDH.AppearanceHeader.Options.UseFont = true;
            this.colMaDH.Caption = "Mã đơn hàng";
            this.colMaDH.FieldName = "MaDH";
            this.colMaDH.MinWidth = 24;
            this.colMaDH.Name = "colMaDH";
            this.colMaDH.Visible = true;
            this.colMaDH.VisibleIndex = 0;
            this.colMaDH.Width = 150;
            // 
            // colDauSize
            // 
            this.colDauSize.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colDauSize.AppearanceCell.Options.UseFont = true;
            this.colDauSize.AppearanceCell.Options.UseTextOptions = true;
            this.colDauSize.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colDauSize.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colDauSize.AppearanceHeader.Options.UseFont = true;
            this.colDauSize.Caption = "Nhóm size";
            this.colDauSize.FieldName = "DauSize";
            this.colDauSize.MinWidth = 24;
            this.colDauSize.Name = "colDauSize";
            this.colDauSize.Visible = true;
            this.colDauSize.VisibleIndex = 1;
            this.colDauSize.Width = 199;
            // 
            // colDauSizeID
            // 
            this.colDauSizeID.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colDauSizeID.AppearanceCell.Options.UseFont = true;
            this.colDauSizeID.AppearanceCell.Options.UseTextOptions = true;
            this.colDauSizeID.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colDauSizeID.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colDauSizeID.AppearanceHeader.Options.UseFont = true;
            this.colDauSizeID.Caption = "Đầu size ID";
            this.colDauSizeID.FieldName = "DauSizeID";
            this.colDauSizeID.MinWidth = 24;
            this.colDauSizeID.Name = "colDauSizeID";
            this.colDauSizeID.Width = 343;
            // 
            // colMaMau
            // 
            this.colMaMau.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaMau.AppearanceCell.Options.UseFont = true;
            this.colMaMau.AppearanceCell.Options.UseTextOptions = true;
            this.colMaMau.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaMau.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colMaMau.AppearanceHeader.Options.UseFont = true;
            this.colMaMau.Caption = "Mã màu";
            this.colMaMau.FieldName = "MaMau";
            this.colMaMau.MinWidth = 24;
            this.colMaMau.Name = "colMaMau";
            this.colMaMau.Visible = true;
            this.colMaMau.VisibleIndex = 2;
            this.colMaMau.Width = 120;
            // 
            // colSize
            // 
            this.colSize.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colSize.AppearanceCell.Options.UseFont = true;
            this.colSize.AppearanceCell.Options.UseTextOptions = true;
            this.colSize.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSize.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colSize.AppearanceHeader.Options.UseFont = true;
            this.colSize.Caption = "Size";
            this.colSize.FieldName = "Size";
            this.colSize.MinWidth = 24;
            this.colSize.Name = "colSize";
            this.colSize.Visible = true;
            this.colSize.VisibleIndex = 3;
            this.colSize.Width = 101;
            // 
            // colSizeID
            // 
            this.colSizeID.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colSizeID.AppearanceCell.Options.UseFont = true;
            this.colSizeID.AppearanceCell.Options.UseTextOptions = true;
            this.colSizeID.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSizeID.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colSizeID.AppearanceHeader.Options.UseFont = true;
            this.colSizeID.Caption = "Size ID";
            this.colSizeID.FieldName = "SizeID";
            this.colSizeID.MinWidth = 24;
            this.colSizeID.Name = "colSizeID";
            this.colSizeID.Width = 343;
            // 
            // colSoLuong
            // 
            this.colSoLuong.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colSoLuong.AppearanceCell.Options.UseFont = true;
            this.colSoLuong.AppearanceCell.Options.UseTextOptions = true;
            this.colSoLuong.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSoLuong.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colSoLuong.AppearanceHeader.Options.UseFont = true;
            this.colSoLuong.Caption = "Số lượng";
            this.colSoLuong.FieldName = "SoLuong";
            this.colSoLuong.MinWidth = 24;
            this.colSoLuong.Name = "colSoLuong";
            this.colSoLuong.Visible = true;
            this.colSoLuong.VisibleIndex = 4;
            this.colSoLuong.Width = 90;
            // 
            // colNgayTru
            // 
            this.colNgayTru.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colNgayTru.AppearanceCell.Options.UseFont = true;
            this.colNgayTru.AppearanceCell.Options.UseTextOptions = true;
            this.colNgayTru.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colNgayTru.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colNgayTru.AppearanceHeader.Options.UseFont = true;
            this.colNgayTru.Caption = "Ngày trừ";
            this.colNgayTru.DisplayFormat.FormatString = "HH:mm dd/MM/yyyy";
            this.colNgayTru.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colNgayTru.FieldName = "NgayTru";
            this.colNgayTru.MinWidth = 24;
            this.colNgayTru.Name = "colNgayTru";
            this.colNgayTru.Visible = true;
            this.colNgayTru.VisibleIndex = 5;
            this.colNgayTru.Width = 199;
            // 
            // colGhiChu
            // 
            this.colGhiChu.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colGhiChu.AppearanceCell.Options.UseFont = true;
            this.colGhiChu.AppearanceCell.Options.UseTextOptions = true;
            this.colGhiChu.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colGhiChu.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colGhiChu.AppearanceHeader.Options.UseFont = true;
            this.colGhiChu.Caption = "Ghi chú";
            this.colGhiChu.FieldName = "GhiChu";
            this.colGhiChu.MinWidth = 24;
            this.colGhiChu.Name = "colGhiChu";
            this.colGhiChu.Visible = true;
            this.colGhiChu.VisibleIndex = 6;
            this.colGhiChu.Width = 199;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(563, 2);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(73, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // frmLichSuTruTonKho
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1581, 965);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.gridLichSuTruTK);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControl1);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.LookAndFeel.SkinName = "Office 2019 Colorful";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmLichSuTruTonKho";
            this.ShowIcon = false;
            this.Text = "Lịch sử trừ tồn kho";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLichSuTruTK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLichSuTruTK)).EndInit();
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
        private DevExpress.XtraGrid.GridControl gridLichSuTruTK;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewLichSuTruTK;
        private DevExpress.XtraGrid.Columns.GridColumn colID;
        private DevExpress.XtraGrid.Columns.GridColumn colMaHang;
        private DevExpress.XtraGrid.Columns.GridColumn colMaDH;
        private DevExpress.XtraGrid.Columns.GridColumn colDauSize;
        private DevExpress.XtraGrid.Columns.GridColumn colDauSizeID;

        private DevExpress.XtraGrid.Columns.GridColumn colMaMau;
        private DevExpress.XtraGrid.Columns.GridColumn colSize;
        private DevExpress.XtraGrid.Columns.GridColumn colSizeID;      
        private DevExpress.XtraGrid.Columns.GridColumn colSoLuong;
        private DevExpress.XtraGrid.Columns.GridColumn colNgayTru;
        private DevExpress.XtraGrid.Columns.GridColumn colGhiChu;
        private System.Windows.Forms.Button button1;

    }
}