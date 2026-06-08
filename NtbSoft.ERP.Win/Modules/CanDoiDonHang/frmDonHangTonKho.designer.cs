using DevExpress.XtraGrid.Columns;
using System.Drawing;

namespace NtbSoft.ERP.Win.Modules.CanDoiDonHang
{
    partial class frmDonHangTonKho
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDonHangTonKho));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.Them = new DevExpress.XtraBars.BarButtonItem();
            this.Sua = new DevExpress.XtraBars.BarButtonItem();
            this.Xoa = new DevExpress.XtraBars.BarButtonItem();
            this.Luu = new DevExpress.XtraBars.BarButtonItem();
            this.Naplai = new DevExpress.XtraBars.BarButtonItem();
            this.LichSu = new DevExpress.XtraBars.BarButtonItem();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.barCheckItem1 = new DevExpress.XtraBars.BarCheckItem();
            this.layoutControl_KHSX = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlDanhSachSoLuong_KHSX = new DevExpress.XtraGrid.GridControl();
            this.bandedGridViewDanhSachSoLuong_KHSX = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridView();
            this.gridBandSTTKHSX = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandPOID = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandPO = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandMaMau = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandTenMau = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandDauSize = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandDauSizeID = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandMaDH = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandMaQuocGia = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandNgayGH = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandGhiChu = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandSize = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.searchLookUpEditMaDH = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.gridviewLookupEditMH = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMaDH = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenHang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDot = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSoLuong = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup_KHSX = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem_KHSX = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlGroupThongTinDonHang = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItemThongTinDonHang = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.colMaHang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridBandPOID_TTK = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandPO_TTK = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandMaMau_TTK = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandDauSize_TTK = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandDauSizeID_TTK = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandMaDH_TTK = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandMaQuocGia_TTK = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandNgayGH_TTK = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandGhiChu_TTK = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandSize_TTK = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
            this.layoutControl_Kho = new DevExpress.XtraLayout.LayoutControl();
            this.gridControlDanhSachSoLuongTonKho = new DevExpress.XtraGrid.GridControl();
            this.bandedGridViewDanhSachSoLuongTonKho = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridView();
            this.gridBandSTT_Kho = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandPOID_Kho = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandPO_Kho = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandMaMau_Kho = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandTenMau_Kho = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandDauSize_Kho = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandDauSizeID_Kho = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandMaDH_Kho = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandMaQuocGia_Kho = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandNgayGH_Kho = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandGhiChu_Kho = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandSize_Kho = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.layoutControlGroup_Kho = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem_Kho = new DevExpress.XtraLayout.LayoutControlItem();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl_KHSX)).BeginInit();
            this.layoutControl_KHSX.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDanhSachSoLuong_KHSX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bandedGridViewDanhSachSoLuong_KHSX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditMaDH.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridviewLookupEditMH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup_KHSX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem_KHSX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupThongTinDonHang)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemThongTinDonHang)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
            this.splitContainerControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl_Kho)).BeginInit();
            this.layoutControl_Kho.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDanhSachSoLuongTonKho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bandedGridViewDanhSachSoLuongTonKho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup_Kho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem_Kho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
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
            this.LichSu,
            this.barCheckItem1});
            this.barManager1.MaxItemId = 6;
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
            new DevExpress.XtraBars.LinkPersistInfo(this.Naplai),
            new DevExpress.XtraBars.LinkPersistInfo(this.LichSu)});
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
            this.Them.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Them.ImageOptions.SvgImage")));
            this.Them.Name = "Them";
            this.Them.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // Sua
            // 
            this.Sua.Caption = "Sửa (F2)";
            this.Sua.Id = 1;
            this.Sua.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Sua.ImageOptions.SvgImage")));
            this.Sua.Name = "Sua";
            this.Sua.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // Xoa
            // 
            this.Xoa.Caption = "Xóa (F3)";
            this.Xoa.Id = 2;
            this.Xoa.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Xoa.ImageOptions.SvgImage")));
            this.Xoa.Name = "Xoa";
            this.Xoa.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // Luu
            // 
            this.Luu.Caption = "Lưu (F4)";
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
            this.Naplai.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btNapLai);
            // 
            // LichSu
            // 
            this.LichSu.Caption = "Lịch sử (F6)";
            this.LichSu.Enabled = false;
            this.LichSu.Id = 4;
            this.LichSu.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("LichSu.ImageOptions.Image")));
            this.LichSu.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("LichSu.ImageOptions.LargeImage")));
            this.LichSu.ItemAppearance.Disabled.Font = new System.Drawing.Font("Tahoma", 8.1F);
            this.LichSu.ItemAppearance.Disabled.Options.UseFont = true;
            this.LichSu.ItemAppearance.Hovered.Font = new System.Drawing.Font("Tahoma", 8.1F);
            this.LichSu.ItemAppearance.Hovered.Options.UseFont = true;
            this.LichSu.ItemAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.1F);
            this.LichSu.ItemAppearance.Normal.Options.UseFont = true;
            this.LichSu.ItemAppearance.Pressed.Font = new System.Drawing.Font("Tahoma", 8.1F);
            this.LichSu.ItemAppearance.Pressed.Options.UseFont = true;
            this.LichSu.Name = "LichSu";
            this.LichSu.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.LichSu.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btLichSu);
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
            this.barDockControlTop.Size = new System.Drawing.Size(1174, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 783);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(1174, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 747);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl1.Location = new System.Drawing.Point(1174, 36);
            this.barDockControl1.Manager = this.barManager1;
            this.barDockControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControl1.Size = new System.Drawing.Size(0, 747);
            // 
            // barCheckItem1
            // 
            this.barCheckItem1.Caption = "Nhập/Import";
            this.barCheckItem1.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
            this.barCheckItem1.Id = 5;
            this.barCheckItem1.Name = "barCheckItem1";
            this.barCheckItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // layoutControl_KHSX
            // 
            this.layoutControl_KHSX.Controls.Add(this.gridControlDanhSachSoLuong_KHSX);
            this.layoutControl_KHSX.Controls.Add(this.searchLookUpEditMaDH);
            this.layoutControl_KHSX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl_KHSX.Location = new System.Drawing.Point(0, 0);
            this.layoutControl_KHSX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.layoutControl_KHSX.Name = "layoutControl_KHSX";
            this.layoutControl_KHSX.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(790, 447, 812, 500);
            this.layoutControl_KHSX.Root = this.layoutControlGroup_KHSX;
            this.layoutControl_KHSX.Size = new System.Drawing.Size(1174, 554);
            this.layoutControl_KHSX.TabIndex = 5;
            this.layoutControl_KHSX.Text = "layoutControl_KHSX";
            // 
            // gridControlDanhSachSoLuong_KHSX
            // 
            this.gridControlDanhSachSoLuong_KHSX.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridControlDanhSachSoLuong_KHSX.Location = new System.Drawing.Point(8, 104);
            this.gridControlDanhSachSoLuong_KHSX.MainView = this.bandedGridViewDanhSachSoLuong_KHSX;
            this.gridControlDanhSachSoLuong_KHSX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridControlDanhSachSoLuong_KHSX.MenuManager = this.barManager1;
            this.gridControlDanhSachSoLuong_KHSX.Name = "gridControlDanhSachSoLuong_KHSX";
            this.gridControlDanhSachSoLuong_KHSX.Size = new System.Drawing.Size(1158, 442);
            this.gridControlDanhSachSoLuong_KHSX.TabIndex = 5;
            this.gridControlDanhSachSoLuong_KHSX.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.bandedGridViewDanhSachSoLuong_KHSX});
            // 
            // bandedGridViewDanhSachSoLuong_KHSX
            // 
            this.bandedGridViewDanhSachSoLuong_KHSX.Appearance.FocusedCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(88)))), ((int)(((byte)(219)))));
            this.bandedGridViewDanhSachSoLuong_KHSX.Appearance.FocusedCell.Options.UseForeColor = true;
            this.bandedGridViewDanhSachSoLuong_KHSX.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(209)))));
            this.bandedGridViewDanhSachSoLuong_KHSX.Appearance.FocusedRow.Options.UseBackColor = true;
            this.bandedGridViewDanhSachSoLuong_KHSX.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bandedGridViewDanhSachSoLuong_KHSX.Appearance.HeaderPanel.Options.UseFont = true;
            this.bandedGridViewDanhSachSoLuong_KHSX.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.bandedGridViewDanhSachSoLuong_KHSX.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.bandedGridViewDanhSachSoLuong_KHSX.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.bandedGridViewDanhSachSoLuong_KHSX.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] {
            this.gridBandSTTKHSX,
            this.gridBandPOID,
            this.gridBandPO,
            this.gridBandMaMau,
            this.gridBandTenMau,
            this.gridBandDauSize,
            this.gridBandDauSizeID,
            this.gridBandMaDH,
            this.gridBandMaQuocGia,
            this.gridBandNgayGH,
            this.gridBandGhiChu,
            this.gridBandSize});
            this.bandedGridViewDanhSachSoLuong_KHSX.DetailHeight = 431;
            this.bandedGridViewDanhSachSoLuong_KHSX.GridControl = this.gridControlDanhSachSoLuong_KHSX;
            this.bandedGridViewDanhSachSoLuong_KHSX.Name = "bandedGridViewDanhSachSoLuong_KHSX";
            this.bandedGridViewDanhSachSoLuong_KHSX.OptionsCustomization.AllowBandMoving = false;
            this.bandedGridViewDanhSachSoLuong_KHSX.OptionsNavigation.EnterMoveNextColumn = true;
            this.bandedGridViewDanhSachSoLuong_KHSX.OptionsView.ColumnAutoWidth = false;
            this.bandedGridViewDanhSachSoLuong_KHSX.OptionsView.ShowColumnHeaders = false;
            this.bandedGridViewDanhSachSoLuong_KHSX.OptionsView.ShowFooter = true;
            this.bandedGridViewDanhSachSoLuong_KHSX.OptionsView.ShowGroupPanel = false;
            this.bandedGridViewDanhSachSoLuong_KHSX.CustomDrawBandHeader += new DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventHandler(this.bandedGridViewDanhSachSoLuong_KHSX_CustomDrawBandHeader);
            this.bandedGridViewDanhSachSoLuong_KHSX.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.bandedGridViewDanhSachSoLuong_KHSX_CustomDrawRowIndicator);
            this.bandedGridViewDanhSachSoLuong_KHSX.CustomDrawFooterCell += new DevExpress.XtraGrid.Views.Grid.FooterCellCustomDrawEventHandler(this.bandedGridviewSoLuong_CustomDrawFooterCell);
            this.bandedGridViewDanhSachSoLuong_KHSX.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.BandedView_CustomUnboundColumnData);
            this.bandedGridViewDanhSachSoLuong_KHSX.DoubleClick += new System.EventHandler(this.bandedGridviewSoLuong_KHSX_DoubleClick);
            this.bandedGridViewDanhSachSoLuong_KHSX.RowCountChanged += new System.EventHandler(this.bandedGridViewDanhSachSoLuong_KHSX_RowCountChanged);
            // 
            // gridBandSTTKHSX
            // 
            this.gridBandSTTKHSX.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandSTTKHSX.AppearanceHeader.Options.UseFont = true;
            this.gridBandSTTKHSX.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandSTTKHSX.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandSTTKHSX.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandSTTKHSX.Caption = "STT";
            this.gridBandSTTKHSX.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandSTTKHSX.MinWidth = 71;
            this.gridBandSTTKHSX.Name = "gridBandSTTKHSX";
            this.gridBandSTTKHSX.Visible = false;
            this.gridBandSTTKHSX.VisibleIndex = -1;
            this.gridBandSTTKHSX.Width = 71;
            // 
            // gridBandPOID
            // 
            this.gridBandPOID.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandPOID.AppearanceHeader.Options.UseFont = true;
            this.gridBandPOID.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandPOID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandPOID.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandPOID.Caption = "POID";
            this.gridBandPOID.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandPOID.MinWidth = 159;
            this.gridBandPOID.Name = "gridBandPOID";
            this.gridBandPOID.Visible = false;
            this.gridBandPOID.VisibleIndex = -1;
            this.gridBandPOID.Width = 198;
            // 
            // gridBandPO
            // 
            this.gridBandPO.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandPO.AppearanceHeader.Options.UseFont = true;
            this.gridBandPO.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandPO.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandPO.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandPO.Caption = "PO";
            this.gridBandPO.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandPO.MinWidth = 159;
            this.gridBandPO.Name = "gridBandPO";
            this.gridBandPO.VisibleIndex = 0;
            this.gridBandPO.Width = 198;
            // 
            // gridBandMaMau
            // 
            this.gridBandMaMau.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandMaMau.AppearanceHeader.Options.UseFont = true;
            this.gridBandMaMau.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandMaMau.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandMaMau.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandMaMau.Caption = "Mã Màu";
            this.gridBandMaMau.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandMaMau.MinWidth = 159;
            this.gridBandMaMau.Name = "gridBandMaMau";
            this.gridBandMaMau.Visible = false;
            this.gridBandMaMau.VisibleIndex = -1;
            this.gridBandMaMau.Width = 198;
            // 
            // gridBandTenMau
            // 
            this.gridBandTenMau.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandTenMau.AppearanceHeader.Options.UseFont = true;
            this.gridBandTenMau.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandTenMau.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandTenMau.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandTenMau.Caption = "Màu";
            this.gridBandTenMau.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandTenMau.MinWidth = 159;
            this.gridBandTenMau.Name = "gridBandTenMau";
            this.gridBandTenMau.VisibleIndex = 1;
            this.gridBandTenMau.Width = 198;
            // 
            // gridBandDauSize
            // 
            this.gridBandDauSize.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandDauSize.AppearanceHeader.Options.UseFont = true;
            this.gridBandDauSize.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandDauSize.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandDauSize.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandDauSize.Caption = "Nhóm size";
            this.gridBandDauSize.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandDauSize.MinWidth = 159;
            this.gridBandDauSize.Name = "gridBandDauSize";
            this.gridBandDauSize.VisibleIndex = 2;
            this.gridBandDauSize.Width = 198;
            // 
            // gridBandDauSizeID
            // 
            this.gridBandDauSizeID.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandDauSizeID.AppearanceHeader.Options.UseFont = true;
            this.gridBandDauSizeID.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandDauSizeID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandDauSizeID.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandDauSizeID.Caption = "Đầu size ID";
            this.gridBandDauSizeID.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandDauSizeID.MinWidth = 159;
            this.gridBandDauSizeID.Name = "gridBandDauSizeID";
            this.gridBandDauSizeID.Visible = false;
            this.gridBandDauSizeID.VisibleIndex = -1;
            this.gridBandDauSizeID.Width = 198;
            // 
            // gridBandMaDH
            // 
            this.gridBandMaDH.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridBandMaDH.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandMaDH.AppearanceHeader.Options.UseBackColor = true;
            this.gridBandMaDH.AppearanceHeader.Options.UseFont = true;
            this.gridBandMaDH.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandMaDH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandMaDH.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandMaDH.Caption = "Mã đơn hàng";
            this.gridBandMaDH.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandMaDH.MinWidth = 159;
            this.gridBandMaDH.Name = "gridBandMaDH";
            this.gridBandMaDH.Visible = false;
            this.gridBandMaDH.VisibleIndex = -1;
            this.gridBandMaDH.Width = 198;
            // 
            // gridBandMaQuocGia
            // 
            this.gridBandMaQuocGia.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridBandMaQuocGia.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandMaQuocGia.AppearanceHeader.Options.UseBackColor = true;
            this.gridBandMaQuocGia.AppearanceHeader.Options.UseFont = true;
            this.gridBandMaQuocGia.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandMaQuocGia.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandMaQuocGia.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandMaQuocGia.Caption = "Quốc gia";
            this.gridBandMaQuocGia.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandMaQuocGia.MinWidth = 159;
            this.gridBandMaQuocGia.Name = "gridBandMaQuocGia";
            this.gridBandMaQuocGia.Visible = false;
            this.gridBandMaQuocGia.VisibleIndex = -1;
            this.gridBandMaQuocGia.Width = 198;
            // 
            // gridBandNgayGH
            // 
            this.gridBandNgayGH.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandNgayGH.AppearanceHeader.Options.UseFont = true;
            this.gridBandNgayGH.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandNgayGH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandNgayGH.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandNgayGH.Caption = "Ngày GH";
            this.gridBandNgayGH.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandNgayGH.MinWidth = 159;
            this.gridBandNgayGH.Name = "gridBandNgayGH";
            this.gridBandNgayGH.Visible = false;
            this.gridBandNgayGH.VisibleIndex = -1;
            this.gridBandNgayGH.Width = 198;
            // 
            // gridBandGhiChu
            // 
            this.gridBandGhiChu.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandGhiChu.AppearanceHeader.Options.UseFont = true;
            this.gridBandGhiChu.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandGhiChu.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandGhiChu.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandGhiChu.Caption = "Ghi chú";
            this.gridBandGhiChu.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandGhiChu.MinWidth = 159;
            this.gridBandGhiChu.Name = "gridBandGhiChu";
            this.gridBandGhiChu.VisibleIndex = 3;
            this.gridBandGhiChu.Width = 198;
            // 
            // gridBandSize
            // 
            this.gridBandSize.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandSize.AppearanceHeader.Options.UseFont = true;
            this.gridBandSize.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandSize.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandSize.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandSize.Caption = "Size";
            this.gridBandSize.MinWidth = 476;
            this.gridBandSize.Name = "gridBandSize";
            this.gridBandSize.VisibleIndex = 4;
            this.gridBandSize.Width = 765;
            // 
            // searchLookUpEditMaDH
            // 
            this.searchLookUpEditMaDH.Location = new System.Drawing.Point(155, 44);
            this.searchLookUpEditMaDH.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.searchLookUpEditMaDH.Name = "searchLookUpEditMaDH";
            this.searchLookUpEditMaDH.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.searchLookUpEditMaDH.Properties.DisplayMember = "MaDH";
            this.searchLookUpEditMaDH.Properties.NullText = "Chọn mã đơn hàng";
            this.searchLookUpEditMaDH.Properties.PopupView = this.gridviewLookupEditMH;
            this.searchLookUpEditMaDH.Properties.ShowClearButton = false;
            this.searchLookUpEditMaDH.Properties.ShowFooter = false;
            this.searchLookUpEditMaDH.Properties.ValueMember = "MaDH";
            this.searchLookUpEditMaDH.Size = new System.Drawing.Size(372, 22);
            this.searchLookUpEditMaDH.StyleController = this.layoutControl_KHSX;
            this.searchLookUpEditMaDH.TabIndex = 6;
            this.searchLookUpEditMaDH.EditValueChanged += new System.EventHandler(this.searchLookUpEditMaDH_Properties_EditValueChanged);
            this.searchLookUpEditMaDH.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.SearchLookupEditMaDH_CustomDisplayText);
            // 
            // gridviewLookupEditMH
            // 
            this.gridviewLookupEditMH.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMaDH,
            this.colTenHang,
            this.colDot,
            this.colSoLuong});
            this.gridviewLookupEditMH.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridviewLookupEditMH.Name = "gridviewLookupEditMH";
            this.gridviewLookupEditMH.OptionsCustomization.AllowColumnMoving = false;
            this.gridviewLookupEditMH.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridviewLookupEditMH.OptionsView.ShowGroupPanel = false;
            this.gridviewLookupEditMH.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridviewLookupEditMH_CustomDrawColumnHeader);
            // 
            // colMaDH
            // 
            this.colMaDH.AppearanceCell.Options.UseTextOptions = true;
            this.colMaDH.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaDH.AppearanceHeader.Options.UseTextOptions = true;
            this.colMaDH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaDH.Caption = "Mã đơn hàng";
            this.colMaDH.FieldName = "MaDH";
            this.colMaDH.Name = "colMaDH";
            this.colMaDH.Visible = true;
            this.colMaDH.VisibleIndex = 0;
            // 
            // colTenHang
            // 
            this.colTenHang.AppearanceCell.Options.UseTextOptions = true;
            this.colTenHang.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colTenHang.AppearanceHeader.Options.UseTextOptions = true;
            this.colTenHang.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colTenHang.Caption = "Tên hàng";
            this.colTenHang.FieldName = "TenHang";
            this.colTenHang.Name = "colTenHang";
            this.colTenHang.Visible = true;
            this.colTenHang.VisibleIndex = 1;
            // 
            // colDot
            // 
            this.colDot.AppearanceCell.Options.UseTextOptions = true;
            this.colDot.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colDot.AppearanceHeader.Options.UseTextOptions = true;
            this.colDot.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colDot.Caption = "Đợt";
            this.colDot.FieldName = "Dot";
            this.colDot.Name = "colDot";
            this.colDot.Visible = true;
            this.colDot.VisibleIndex = 2;
            // 
            // colSoLuong
            // 
            this.colSoLuong.AppearanceCell.Options.UseTextOptions = true;
            this.colSoLuong.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSoLuong.AppearanceHeader.Options.UseTextOptions = true;
            this.colSoLuong.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colSoLuong.Caption = "Số lượng";
            this.colSoLuong.FieldName = "SoLuong";
            this.colSoLuong.Name = "colSoLuong";
            this.colSoLuong.Visible = true;
            this.colSoLuong.VisibleIndex = 3;
            // 
            // layoutControlGroup_KHSX
            // 
            this.layoutControlGroup_KHSX.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup_KHSX.GroupBordersVisible = false;
            this.layoutControlGroup_KHSX.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem_KHSX,
            this.layoutControlGroupThongTinDonHang});
            this.layoutControlGroup_KHSX.Name = "Root";
            this.layoutControlGroup_KHSX.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 6, 6, 6);
            this.layoutControlGroup_KHSX.Size = new System.Drawing.Size(1174, 554);
            this.layoutControlGroup_KHSX.TextVisible = false;
            // 
            // layoutControlItem_KHSX
            // 
            this.layoutControlItem_KHSX.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem_KHSX.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem_KHSX.Control = this.gridControlDanhSachSoLuong_KHSX;
            this.layoutControlItem_KHSX.Location = new System.Drawing.Point(0, 74);
            this.layoutControlItem_KHSX.Name = "layoutControlItem_KHSX";
            this.layoutControlItem_KHSX.Size = new System.Drawing.Size(1162, 468);
            this.layoutControlItem_KHSX.Text = "Số lượng kế hoạch";
            this.layoutControlItem_KHSX.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem_KHSX.TextSize = new System.Drawing.Size(132, 19);
            // 
            // layoutControlGroupThongTinDonHang
            // 
            this.layoutControlGroupThongTinDonHang.AppearanceGroup.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlGroupThongTinDonHang.AppearanceGroup.Options.UseFont = true;
            this.layoutControlGroupThongTinDonHang.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemThongTinDonHang,
            this.emptySpaceItem1});
            this.layoutControlGroupThongTinDonHang.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroupThongTinDonHang.Name = "layoutControlGroupThongTinDonHang";
            this.layoutControlGroupThongTinDonHang.Size = new System.Drawing.Size(1162, 74);
            this.layoutControlGroupThongTinDonHang.Text = "Thông tin đơn hàng";
            // 
            // layoutControlItemThongTinDonHang
            // 
            this.layoutControlItemThongTinDonHang.AllowHtmlStringInCaption = true;
            this.layoutControlItemThongTinDonHang.Control = this.searchLookUpEditMaDH;
            this.layoutControlItemThongTinDonHang.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemThongTinDonHang.Name = "layoutControlItemThongTinDonHang";
            this.layoutControlItemThongTinDonHang.Size = new System.Drawing.Size(511, 26);
            this.layoutControlItemThongTinDonHang.Text = "Mã đơn hàng <color=red>(*)</color>:";
            this.layoutControlItemThongTinDonHang.TextSize = new System.Drawing.Size(132, 16);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(511, 0);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(627, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // colMaHang
            // 
            this.colMaHang.AppearanceCell.Options.UseTextOptions = true;
            this.colMaHang.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaHang.AppearanceHeader.Options.UseTextOptions = true;
            this.colMaHang.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaHang.Caption = "Mã hàng";
            this.colMaHang.FieldName = "MaHang";
            this.colMaHang.Name = "colMaHang";
            // 
            // gridBandPOID_TTK
            // 
            this.gridBandPOID_TTK.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandPOID_TTK.AppearanceHeader.Options.UseFont = true;
            this.gridBandPOID_TTK.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandPOID_TTK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandPOID_TTK.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandPOID_TTK.Caption = "PO";
            this.gridBandPOID_TTK.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandPOID_TTK.MinWidth = 100;
            this.gridBandPOID_TTK.Name = "gridBandPOID_TTK";
            this.gridBandPOID_TTK.Visible = false;
            this.gridBandPOID_TTK.VisibleIndex = -1;
            this.gridBandPOID_TTK.Width = 125;
            // 
            // gridBandPO_TTK
            // 
            this.gridBandPO_TTK.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandPO_TTK.AppearanceHeader.Options.UseFont = true;
            this.gridBandPO_TTK.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandPO_TTK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandPO_TTK.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandPO_TTK.Caption = "PO";
            this.gridBandPO_TTK.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandPO_TTK.MinWidth = 100;
            this.gridBandPO_TTK.Name = "gridBandPO_TTK";
            this.gridBandPO_TTK.VisibleIndex = -1;
            this.gridBandPO_TTK.Width = 125;
            // 
            // gridBandMaMau_TTK
            // 
            this.gridBandMaMau_TTK.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandMaMau_TTK.AppearanceHeader.Options.UseFont = true;
            this.gridBandMaMau_TTK.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandMaMau_TTK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandMaMau_TTK.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandMaMau_TTK.Caption = "Màu";
            this.gridBandMaMau_TTK.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandMaMau_TTK.MinWidth = 100;
            this.gridBandMaMau_TTK.Name = "gridBandMaMau_TTK";
            this.gridBandMaMau_TTK.VisibleIndex = -1;
            this.gridBandMaMau_TTK.Width = 125;
            // 
            // gridBandDauSize_TTK
            // 
            this.gridBandDauSize_TTK.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandDauSize_TTK.AppearanceHeader.Options.UseFont = true;
            this.gridBandDauSize_TTK.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandDauSize_TTK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandDauSize_TTK.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandDauSize_TTK.Caption = "Đầu size";
            this.gridBandDauSize_TTK.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandDauSize_TTK.MinWidth = 100;
            this.gridBandDauSize_TTK.Name = "gridBandDauSize_TTK";
            this.gridBandDauSize_TTK.VisibleIndex = -1;
            this.gridBandDauSize_TTK.Width = 125;
            // 
            // gridBandDauSizeID_TTK
            // 
            this.gridBandDauSizeID_TTK.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandDauSizeID_TTK.AppearanceHeader.Options.UseFont = true;
            this.gridBandDauSizeID_TTK.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandDauSizeID_TTK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandDauSizeID_TTK.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandDauSizeID_TTK.Caption = "Đầu size ID";
            this.gridBandDauSizeID_TTK.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandDauSizeID_TTK.MinWidth = 100;
            this.gridBandDauSizeID_TTK.Name = "gridBandDauSizeID_TTK";
            this.gridBandDauSizeID_TTK.Visible = false;
            this.gridBandDauSizeID_TTK.VisibleIndex = -1;
            this.gridBandDauSizeID_TTK.Width = 125;
            // 
            // gridBandMaDH_TTK
            // 
            this.gridBandMaDH_TTK.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandMaDH_TTK.AppearanceHeader.Options.UseFont = true;
            this.gridBandMaDH_TTK.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandMaDH_TTK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandMaDH_TTK.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandMaDH_TTK.Caption = "Mã đơn hàng";
            this.gridBandMaDH_TTK.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandMaDH_TTK.MinWidth = 100;
            this.gridBandMaDH_TTK.Name = "gridBandMaDH_TTK";
            this.gridBandMaDH_TTK.Visible = false;
            this.gridBandMaDH_TTK.VisibleIndex = -1;
            this.gridBandMaDH_TTK.Width = 125;
            // 
            // gridBandMaQuocGia_TTK
            // 
            this.gridBandMaQuocGia_TTK.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandMaQuocGia_TTK.AppearanceHeader.Options.UseFont = true;
            this.gridBandMaQuocGia_TTK.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandMaQuocGia_TTK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandMaQuocGia_TTK.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandMaQuocGia_TTK.Caption = "Quốc gia";
            this.gridBandMaQuocGia_TTK.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandMaQuocGia_TTK.MinWidth = 100;
            this.gridBandMaQuocGia_TTK.Name = "gridBandMaQuocGia_TTK";
            this.gridBandMaQuocGia_TTK.Visible = false;
            this.gridBandMaQuocGia_TTK.VisibleIndex = -1;
            this.gridBandMaQuocGia_TTK.Width = 125;
            // 
            // gridBandNgayGH_TTK
            // 
            this.gridBandNgayGH_TTK.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandNgayGH_TTK.AppearanceHeader.Options.UseFont = true;
            this.gridBandNgayGH_TTK.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandNgayGH_TTK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandNgayGH_TTK.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandNgayGH_TTK.Caption = "Ngày GH";
            this.gridBandNgayGH_TTK.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandNgayGH_TTK.MinWidth = 100;
            this.gridBandNgayGH_TTK.Name = "gridBandNgayGH_TTK";
            this.gridBandNgayGH_TTK.Visible = false;
            this.gridBandNgayGH_TTK.VisibleIndex = -1;
            this.gridBandNgayGH_TTK.Width = 125;
            // 
            // gridBandGhiChu_TTK
            // 
            this.gridBandGhiChu_TTK.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandGhiChu_TTK.AppearanceHeader.Options.UseFont = true;
            this.gridBandGhiChu_TTK.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandGhiChu_TTK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandGhiChu_TTK.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandGhiChu_TTK.Caption = "Ghi chú";
            this.gridBandGhiChu_TTK.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandGhiChu_TTK.MinWidth = 100;
            this.gridBandGhiChu_TTK.Name = "gridBandGhiChu_TTK";
            this.gridBandGhiChu_TTK.VisibleIndex = -1;
            this.gridBandGhiChu_TTK.Width = 125;
            // 
            // gridBandSize_TTK
            // 
            this.gridBandSize_TTK.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridBandSize_TTK.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandSize_TTK.AppearanceHeader.Options.UseFont = true;
            this.gridBandSize_TTK.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandSize_TTK.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandSize_TTK.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandSize_TTK.Caption = "Size";
            this.gridBandSize_TTK.MinWidth = 300;
            this.gridBandSize_TTK.Name = "gridBandSize_TTK";
            this.gridBandSize_TTK.VisibleIndex = -1;
            this.gridBandSize_TTK.Width = 482;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem4.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem4.Control = this.gridControlDanhSachSoLuong_KHSX;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(1006, 230);
            this.layoutControlItem4.Text = "Số lượng kế hoạch";
            this.layoutControlItem4.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(106, 16);
            // 
            // splitContainerControl
            // 
            this.splitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl.Horizontal = false;
            this.splitContainerControl.Location = new System.Drawing.Point(0, 36);
            this.splitContainerControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitContainerControl.Name = "splitContainerControl";
            this.splitContainerControl.Panel1.Controls.Add(this.layoutControl_KHSX);
            this.splitContainerControl.Panel1.Text = "Panel1";
            this.splitContainerControl.Panel2.Controls.Add(this.layoutControl_Kho);
            this.splitContainerControl.Panel2.Text = "Panel2";
            this.splitContainerControl.Size = new System.Drawing.Size(1174, 747);
            this.splitContainerControl.SplitterPosition = 554;
            this.splitContainerControl.TabIndex = 2;
            // 
            // layoutControl_Kho
            // 
            this.layoutControl_Kho.Controls.Add(this.gridControlDanhSachSoLuongTonKho);
            this.layoutControl_Kho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl_Kho.Location = new System.Drawing.Point(0, 0);
            this.layoutControl_Kho.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.layoutControl_Kho.Name = "layoutControl_Kho";
            this.layoutControl_Kho.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(792, 172, 812, 500);
            this.layoutControl_Kho.Root = this.layoutControlGroup_Kho;
            this.layoutControl_Kho.Size = new System.Drawing.Size(1174, 178);
            this.layoutControl_Kho.TabIndex = 5;
            this.layoutControl_Kho.Text = "layoutControl_Kho";
            // 
            // gridControlDanhSachSoLuongTonKho
            // 
            this.gridControlDanhSachSoLuongTonKho.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridControlDanhSachSoLuongTonKho.Location = new System.Drawing.Point(8, 30);
            this.gridControlDanhSachSoLuongTonKho.MainView = this.bandedGridViewDanhSachSoLuongTonKho;
            this.gridControlDanhSachSoLuongTonKho.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridControlDanhSachSoLuongTonKho.MenuManager = this.barManager1;
            this.gridControlDanhSachSoLuongTonKho.Name = "gridControlDanhSachSoLuongTonKho";
            this.gridControlDanhSachSoLuongTonKho.Size = new System.Drawing.Size(1158, 140);
            this.gridControlDanhSachSoLuongTonKho.TabIndex = 5;
            this.gridControlDanhSachSoLuongTonKho.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.bandedGridViewDanhSachSoLuongTonKho});
            this.gridControlDanhSachSoLuongTonKho.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.gridViewDanhSachSoLuong_ProcessGridKey);
            // 
            // bandedGridViewDanhSachSoLuongTonKho
            // 
            this.bandedGridViewDanhSachSoLuongTonKho.Appearance.FocusedCell.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(88)))), ((int)(((byte)(219)))));
            this.bandedGridViewDanhSachSoLuongTonKho.Appearance.FocusedCell.Options.UseForeColor = true;
            this.bandedGridViewDanhSachSoLuongTonKho.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(209)))));
            this.bandedGridViewDanhSachSoLuongTonKho.Appearance.FocusedRow.Options.UseBackColor = true;
            this.bandedGridViewDanhSachSoLuongTonKho.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bandedGridViewDanhSachSoLuongTonKho.Appearance.HeaderPanel.Options.UseFont = true;
            this.bandedGridViewDanhSachSoLuongTonKho.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.bandedGridViewDanhSachSoLuongTonKho.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.bandedGridViewDanhSachSoLuongTonKho.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.bandedGridViewDanhSachSoLuongTonKho.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] {
            this.gridBandSTT_Kho,
            this.gridBandPOID_Kho,
            this.gridBandPO_Kho,
            this.gridBandMaMau_Kho,
            this.gridBandTenMau_Kho,
            this.gridBandDauSize_Kho,
            this.gridBandDauSizeID_Kho,
            this.gridBandMaDH_Kho,
            this.gridBandMaQuocGia_Kho,
            this.gridBandNgayGH_Kho,
            this.gridBandGhiChu_Kho,
            this.gridBandSize_Kho});
            this.bandedGridViewDanhSachSoLuongTonKho.DetailHeight = 431;
            this.bandedGridViewDanhSachSoLuongTonKho.GridControl = this.gridControlDanhSachSoLuongTonKho;
            this.bandedGridViewDanhSachSoLuongTonKho.Name = "bandedGridViewDanhSachSoLuongTonKho";
            this.bandedGridViewDanhSachSoLuongTonKho.OptionsNavigation.EnterMoveNextColumn = true;
            this.bandedGridViewDanhSachSoLuongTonKho.OptionsView.ColumnAutoWidth = false;
            this.bandedGridViewDanhSachSoLuongTonKho.OptionsView.ShowColumnHeaders = false;
            this.bandedGridViewDanhSachSoLuongTonKho.OptionsView.ShowFooter = true;
            this.bandedGridViewDanhSachSoLuongTonKho.OptionsView.ShowGroupPanel = false;
            this.bandedGridViewDanhSachSoLuongTonKho.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.BandedView_CustomUnboundColumnData);
            // 
            // gridBandSTT_Kho
            // 
            this.gridBandSTT_Kho.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridBandSTT_Kho.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandSTT_Kho.AppearanceHeader.Options.UseFont = true;
            this.gridBandSTT_Kho.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandSTT_Kho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandSTT_Kho.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandSTT_Kho.Caption = "STT";
            this.gridBandSTT_Kho.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandSTT_Kho.MinWidth = 61;
            this.gridBandSTT_Kho.Name = "gridBandSTT_Kho";
            this.gridBandSTT_Kho.VisibleIndex = 0;
            this.gridBandSTT_Kho.Width = 61;
            // 
            // gridBandPOID_Kho
            // 
            this.gridBandPOID_Kho.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandPOID_Kho.AppearanceHeader.Options.UseFont = true;
            this.gridBandPOID_Kho.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandPOID_Kho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandPOID_Kho.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandPOID_Kho.Caption = "PO";
            this.gridBandPOID_Kho.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandPOID_Kho.MinWidth = 136;
            this.gridBandPOID_Kho.Name = "gridBandPOID_Kho";
            this.gridBandPOID_Kho.Visible = false;
            this.gridBandPOID_Kho.VisibleIndex = -1;
            this.gridBandPOID_Kho.Width = 170;
            // 
            // gridBandPO_Kho
            // 
            this.gridBandPO_Kho.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridBandPO_Kho.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandPO_Kho.AppearanceHeader.Options.UseBackColor = true;
            this.gridBandPO_Kho.AppearanceHeader.Options.UseFont = true;
            this.gridBandPO_Kho.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandPO_Kho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandPO_Kho.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandPO_Kho.Caption = "PO";
            this.gridBandPO_Kho.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandPO_Kho.MinWidth = 136;
            this.gridBandPO_Kho.Name = "gridBandPO_Kho";
            this.gridBandPO_Kho.Visible = false;
            this.gridBandPO_Kho.VisibleIndex = -1;
            this.gridBandPO_Kho.Width = 170;
            // 
            // gridBandMaMau_Kho
            // 
            this.gridBandMaMau_Kho.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandMaMau_Kho.AppearanceHeader.Options.UseFont = true;
            this.gridBandMaMau_Kho.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandMaMau_Kho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandMaMau_Kho.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandMaMau_Kho.Caption = "Mã Màu";
            this.gridBandMaMau_Kho.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandMaMau_Kho.MinWidth = 136;
            this.gridBandMaMau_Kho.Name = "gridBandMaMau_Kho";
            this.gridBandMaMau_Kho.Visible = false;
            this.gridBandMaMau_Kho.VisibleIndex = -1;
            this.gridBandMaMau_Kho.Width = 170;
            // 
            // gridBandTenMau_Kho
            // 
            this.gridBandTenMau_Kho.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridBandTenMau_Kho.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandTenMau_Kho.AppearanceHeader.Options.UseBackColor = true;
            this.gridBandTenMau_Kho.AppearanceHeader.Options.UseFont = true;
            this.gridBandTenMau_Kho.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandTenMau_Kho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandTenMau_Kho.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandTenMau_Kho.Caption = "Màu";
            this.gridBandTenMau_Kho.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandTenMau_Kho.MinWidth = 136;
            this.gridBandTenMau_Kho.Name = "gridBandTenMau_Kho";
            this.gridBandTenMau_Kho.VisibleIndex = 1;
            this.gridBandTenMau_Kho.Width = 170;
            // 
            // gridBandDauSize_Kho
            // 
            this.gridBandDauSize_Kho.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridBandDauSize_Kho.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandDauSize_Kho.AppearanceHeader.Options.UseBackColor = true;
            this.gridBandDauSize_Kho.AppearanceHeader.Options.UseFont = true;
            this.gridBandDauSize_Kho.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandDauSize_Kho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandDauSize_Kho.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandDauSize_Kho.Caption = "Nhóm size";
            this.gridBandDauSize_Kho.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandDauSize_Kho.MinWidth = 136;
            this.gridBandDauSize_Kho.Name = "gridBandDauSize_Kho";
            this.gridBandDauSize_Kho.VisibleIndex = 2;
            this.gridBandDauSize_Kho.Width = 170;
            // 
            // gridBandDauSizeID_Kho
            // 
            this.gridBandDauSizeID_Kho.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandDauSizeID_Kho.AppearanceHeader.Options.UseFont = true;
            this.gridBandDauSizeID_Kho.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandDauSizeID_Kho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandDauSizeID_Kho.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandDauSizeID_Kho.Caption = "Đầu size ID";
            this.gridBandDauSizeID_Kho.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandDauSizeID_Kho.MinWidth = 136;
            this.gridBandDauSizeID_Kho.Name = "gridBandDauSizeID_Kho";
            this.gridBandDauSizeID_Kho.Visible = false;
            this.gridBandDauSizeID_Kho.VisibleIndex = -1;
            this.gridBandDauSizeID_Kho.Width = 170;
            // 
            // gridBandMaDH_Kho
            // 
            this.gridBandMaDH_Kho.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandMaDH_Kho.AppearanceHeader.Options.UseFont = true;
            this.gridBandMaDH_Kho.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandMaDH_Kho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandMaDH_Kho.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandMaDH_Kho.Caption = "Mã đơn hàng";
            this.gridBandMaDH_Kho.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandMaDH_Kho.MinWidth = 136;
            this.gridBandMaDH_Kho.Name = "gridBandMaDH_Kho";
            this.gridBandMaDH_Kho.Visible = false;
            this.gridBandMaDH_Kho.VisibleIndex = -1;
            this.gridBandMaDH_Kho.Width = 170;
            // 
            // gridBandMaQuocGia_Kho
            // 
            this.gridBandMaQuocGia_Kho.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandMaQuocGia_Kho.AppearanceHeader.Options.UseFont = true;
            this.gridBandMaQuocGia_Kho.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandMaQuocGia_Kho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandMaQuocGia_Kho.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandMaQuocGia_Kho.Caption = "Quốc gia";
            this.gridBandMaQuocGia_Kho.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandMaQuocGia_Kho.MinWidth = 136;
            this.gridBandMaQuocGia_Kho.Name = "gridBandMaQuocGia_Kho";
            this.gridBandMaQuocGia_Kho.Visible = false;
            this.gridBandMaQuocGia_Kho.VisibleIndex = -1;
            this.gridBandMaQuocGia_Kho.Width = 170;
            // 
            // gridBandNgayGH_Kho
            // 
            this.gridBandNgayGH_Kho.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandNgayGH_Kho.AppearanceHeader.Options.UseFont = true;
            this.gridBandNgayGH_Kho.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandNgayGH_Kho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandNgayGH_Kho.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandNgayGH_Kho.Caption = "Ngày GH";
            this.gridBandNgayGH_Kho.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandNgayGH_Kho.MinWidth = 136;
            this.gridBandNgayGH_Kho.Name = "gridBandNgayGH_Kho";
            this.gridBandNgayGH_Kho.Visible = false;
            this.gridBandNgayGH_Kho.VisibleIndex = -1;
            this.gridBandNgayGH_Kho.Width = 170;
            // 
            // gridBandGhiChu_Kho
            // 
            this.gridBandGhiChu_Kho.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridBandGhiChu_Kho.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandGhiChu_Kho.AppearanceHeader.Options.UseBackColor = true;
            this.gridBandGhiChu_Kho.AppearanceHeader.Options.UseFont = true;
            this.gridBandGhiChu_Kho.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandGhiChu_Kho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandGhiChu_Kho.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandGhiChu_Kho.Caption = "Ghi chú";
            this.gridBandGhiChu_Kho.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridBandGhiChu_Kho.MinWidth = 136;
            this.gridBandGhiChu_Kho.Name = "gridBandGhiChu_Kho";
            this.gridBandGhiChu_Kho.VisibleIndex = 3;
            this.gridBandGhiChu_Kho.Width = 170;
            // 
            // gridBandSize_Kho
            // 
            this.gridBandSize_Kho.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(212)))), ((int)(((byte)(128)))));
            this.gridBandSize_Kho.AppearanceHeader.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridBandSize_Kho.AppearanceHeader.Options.UseBackColor = true;
            this.gridBandSize_Kho.AppearanceHeader.Options.UseFont = true;
            this.gridBandSize_Kho.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandSize_Kho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandSize_Kho.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandSize_Kho.Caption = "Size";
            this.gridBandSize_Kho.MinWidth = 408;
            this.gridBandSize_Kho.Name = "gridBandSize_Kho";
            this.gridBandSize_Kho.VisibleIndex = 4;
            this.gridBandSize_Kho.Width = 656;
            // 
            // layoutControlGroup_Kho
            // 
            this.layoutControlGroup_Kho.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup_Kho.GroupBordersVisible = false;
            this.layoutControlGroup_Kho.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem_Kho});
            this.layoutControlGroup_Kho.Name = "Root";
            this.layoutControlGroup_Kho.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 6, 6, 6);
            this.layoutControlGroup_Kho.Size = new System.Drawing.Size(1174, 178);
            this.layoutControlGroup_Kho.TextVisible = false;
            // 
            // layoutControlItem_Kho
            // 
            this.layoutControlItem_Kho.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem_Kho.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem_Kho.Control = this.gridControlDanhSachSoLuongTonKho;
            this.layoutControlItem_Kho.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem_Kho.Name = "layoutControlItem_Kho";
            this.layoutControlItem_Kho.Size = new System.Drawing.Size(1162, 166);
            this.layoutControlItem_Kho.Text = "Số lượng kho";
            this.layoutControlItem_Kho.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem_Kho.TextSize = new System.Drawing.Size(95, 19);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(180, 120);
            // 
            // frmDonHangTonKho
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1174, 783);
            this.Controls.Add(this.splitContainerControl);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControl1);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.LookAndFeel.SkinName = "Office 2013 Light Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDonHangTonKho";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đơn hàng tồn kho";
            this.Load += new System.EventHandler(this.frmDonHangTonKho_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl_KHSX)).EndInit();
            this.layoutControl_KHSX.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDanhSachSoLuong_KHSX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bandedGridViewDanhSachSoLuong_KHSX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditMaDH.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridviewLookupEditMH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup_KHSX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem_KHSX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupThongTinDonHang)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemThongTinDonHang)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
            this.splitContainerControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl_Kho)).EndInit();
            this.layoutControl_Kho.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlDanhSachSoLuongTonKho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bandedGridViewDanhSachSoLuongTonKho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup_Kho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem_Kho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
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
        private DevExpress.XtraBars.BarButtonItem LichSu;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraLayout.LayoutControl layoutControl_KHSX;

        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandPOID_TTK;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandPO_TTK;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandMaMau_TTK;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandDauSize_TTK;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandDauSizeID_TTK;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandMaDH_TTK;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandMaQuocGia_TTK;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandNgayGH_TTK;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandGhiChu_TTK;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandSize_TTK;

        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;

        private DevExpress.XtraGrid.GridControl gridControlDanhSachSoLuong_KHSX;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridView bandedGridViewDanhSachSoLuong_KHSX;
        // Số lượng Tồn kho và còn lại kho
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup_Kho;
        private DevExpress.XtraLayout.LayoutControl layoutControl_Kho;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem_Kho;

        private DevExpress.XtraGrid.GridControl gridControlDanhSachSoLuongTonKho;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridView bandedGridViewDanhSachSoLuongTonKho;
        //
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup_KHSX;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem_KHSX;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupThongTinDonHang;
        private DevExpress.XtraBars.BarCheckItem barCheckItem1;
        private DevExpress.XtraGrid.Columns.GridColumn colMaHang;
        private DevExpress.XtraGrid.Columns.GridColumn colMaDH;
        private DevExpress.XtraGrid.Columns.GridColumn colTenHang;
        private DevExpress.XtraGrid.Columns.GridColumn colSoLuong;
        private DevExpress.XtraGrid.Columns.GridColumn colDot;
        private DevExpress.XtraEditors.SearchLookUpEdit searchLookUpEditMaDH;
        private DevExpress.XtraGrid.Views.Grid.GridView gridviewLookupEditMH;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemThongTinDonHang;

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl;

        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandSTTKHSX;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandPOID;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandPO;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandMaMau;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandTenMau;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandDauSize;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandDauSizeID;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandMaDH;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandMaQuocGia;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandNgayGH;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandGhiChu;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandSize;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandSTT_Kho;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandPOID_Kho;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandPO_Kho;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandMaMau_Kho;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandTenMau_Kho;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandDauSize_Kho;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandDauSizeID_Kho;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandMaDH_Kho;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandMaQuocGia_Kho;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandNgayGH_Kho;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandGhiChu_Kho;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandSize_Kho;
    }
}