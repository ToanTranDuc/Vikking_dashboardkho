
namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    partial class frmKHDongThungPCB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmKHDongThungPCB));
            this.repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.btSave = new DevExpress.XtraBars.BarButtonItem();
            this.btCancel = new DevExpress.XtraBars.BarButtonItem();
            this.barEditItem1 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.btAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btDelete = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.barEditItem2 = new DevExpress.XtraBars.BarEditItem();
            this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
            this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
            this.dgrMaHang = new DevExpress.XtraGrid.GridControl();
            this.gridViewMaHang = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.StyleID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaHang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.dgrKHDongThung = new DevExpress.XtraGrid.GridControl();
            this.bandedGridView1 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridView();
            this.gridBandColorID = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandSize = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBand_QC_DongThung = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.gridBandGC = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
            this.splitContainerControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrMaHang)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMaHang)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrKHDongThung)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bandedGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // repositoryItemCheckEdit2
            // 
            this.repositoryItemCheckEdit2.AutoHeight = false;
            this.repositoryItemCheckEdit2.Caption = "Chỉnh sửa cho tất cả";
            this.repositoryItemCheckEdit2.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
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
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btSave,
            this.btCancel,
            this.btAdd,
            this.btDelete,
            this.barEditItem1});
            this.barManager1.MaxItemId = 8;
            this.barManager1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.btCancel),
            new DevExpress.XtraBars.LinkPersistInfo(this.barEditItem1)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
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
            // btCancel
            // 
            this.btCancel.Caption = "Nạp lại (F5)";
            this.btCancel.Id = 4;
            this.btCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btCancel.ImageOptions.Image")));
            this.btCancel.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btCancel.ImageOptions.LargeImage")));
            this.btCancel.Name = "btCancel";
            this.btCancel.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barEditItem1
            // 
            this.barEditItem1.Edit = this.repositoryItemCheckEdit1;
            this.barEditItem1.Id = 6;
            this.barEditItem1.Name = "barEditItem1";
            this.barEditItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption;
            this.barEditItem1.Size = new System.Drawing.Size(130, 0);
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Caption = "Chỉnh sửa cho tất cả";
            this.repositoryItemCheckEdit1.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlTop.Size = new System.Drawing.Size(1198, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 450);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(1198, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 414);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1198, 36);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 414);
            // 
            // btAdd
            // 
            this.btAdd.Caption = "Thêm (F1)";
            this.btAdd.Id = 3;
            this.btAdd.Name = "btAdd";
            this.btAdd.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btDelete
            // 
            this.btDelete.Caption = "Xóa (F3)";
            this.btDelete.Id = 4;
            this.btDelete.ItemAppearance.Disabled.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold);
            this.btDelete.ItemAppearance.Disabled.Options.UseFont = true;
            this.btDelete.Name = "btDelete";
            this.btDelete.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Lưu (F4)";
            this.barButtonItem1.Id = 3;
            this.barButtonItem1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.Image")));
            this.barButtonItem1.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.LargeImage")));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Caption = "Nạp lại (F5)";
            this.barButtonItem2.Id = 4;
            this.barButtonItem2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.ImageOptions.Image")));
            this.barButtonItem2.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.ImageOptions.LargeImage")));
            this.barButtonItem2.Name = "barButtonItem2";
            this.barButtonItem2.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barEditItem2
            // 
            this.barEditItem2.Edit = this.repositoryItemCheckEdit2;
            this.barEditItem2.Id = 6;
            this.barEditItem2.Name = "barEditItem2";
            this.barEditItem2.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption;
            this.barEditItem2.Size = new System.Drawing.Size(130, 0);
            // 
            // barDockControl4
            // 
            this.barDockControl4.CausesValidation = false;
            this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl4.Location = new System.Drawing.Point(1198, 36);
            this.barDockControl4.Manager = null;
            this.barDockControl4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControl4.Size = new System.Drawing.Size(0, 414);
            // 
            // splitContainerControl
            // 
            this.splitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl.Location = new System.Drawing.Point(0, 36);
            this.splitContainerControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainerControl.Name = "splitContainerControl";
            this.splitContainerControl.Panel1.Controls.Add(this.dgrMaHang);
            this.splitContainerControl.Panel1.Text = "Panel1";
            this.splitContainerControl.Panel2.Controls.Add(this.dgrKHDongThung);
            this.splitContainerControl.Panel2.Text = "Panel2";
            this.splitContainerControl.Size = new System.Drawing.Size(1198, 414);
            this.splitContainerControl.SplitterPosition = 205;
            this.splitContainerControl.TabIndex = 8;
            // 
            // dgrMaHang
            // 
            this.dgrMaHang.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgrMaHang.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgrMaHang.Location = new System.Drawing.Point(0, 0);
            this.dgrMaHang.MainView = this.gridViewMaHang;
            this.dgrMaHang.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgrMaHang.MenuManager = this.barManager1;
            this.dgrMaHang.Name = "dgrMaHang";
            this.dgrMaHang.Size = new System.Drawing.Size(205, 414);
            this.dgrMaHang.TabIndex = 0;
            this.dgrMaHang.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewMaHang});
            // 
            // gridViewMaHang
            // 
            this.gridViewMaHang.Appearance.FocusedCell.BackColor = System.Drawing.Color.RoyalBlue;
            this.gridViewMaHang.Appearance.FocusedCell.ForeColor = System.Drawing.Color.White;
            this.gridViewMaHang.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gridViewMaHang.Appearance.FocusedCell.Options.UseForeColor = true;
            this.gridViewMaHang.Appearance.FocusedRow.BackColor = System.Drawing.Color.RoyalBlue;
            this.gridViewMaHang.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            this.gridViewMaHang.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewMaHang.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gridViewMaHang.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.RoyalBlue;
            this.gridViewMaHang.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gridViewMaHang.Appearance.SelectedRow.ForeColor = System.Drawing.Color.White;
            this.gridViewMaHang.Appearance.SelectedRow.Options.UseForeColor = true;
            this.gridViewMaHang.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.StyleID,
            this.colMaHang});
            this.gridViewMaHang.DetailHeight = 431;
            this.gridViewMaHang.GridControl = this.dgrMaHang;
            this.gridViewMaHang.Name = "gridViewMaHang";
            this.gridViewMaHang.OptionsView.ShowAutoFilterRow = true;
            this.gridViewMaHang.OptionsView.ShowGroupPanel = false;
            this.gridViewMaHang.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewMaHang_FocusedRowChanged);
            // 
            // StyleID
            // 
            this.StyleID.AppearanceHeader.Options.UseTextOptions = true;
            this.StyleID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.StyleID.Caption = "StyleID";
            this.StyleID.FieldName = "StyleID";
            this.StyleID.MinWidth = 23;
            this.StyleID.Name = "StyleID";
            this.StyleID.OptionsColumn.AllowEdit = false;
            this.StyleID.Width = 117;
            // 
            // colMaHang
            // 
            this.colMaHang.Caption = "Mã hàng";
            this.colMaHang.FieldName = "MaHang";
            this.colMaHang.MinWidth = 24;
            this.colMaHang.Name = "colMaHang";
            this.colMaHang.OptionsColumn.AllowEdit = false;
            this.colMaHang.Visible = true;
            this.colMaHang.VisibleIndex = 0;
            this.colMaHang.Width = 94;
            // 
            // dgrKHDongThung
            // 
            this.dgrKHDongThung.Dock = System.Windows.Forms.DockStyle.Left;
            this.dgrKHDongThung.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgrKHDongThung.Location = new System.Drawing.Point(0, 0);
            this.dgrKHDongThung.MainView = this.bandedGridView1;
            this.dgrKHDongThung.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgrKHDongThung.MenuManager = this.barManager1;
            this.dgrKHDongThung.Name = "dgrKHDongThung";
            this.dgrKHDongThung.Size = new System.Drawing.Size(1249, 414);
            this.dgrKHDongThung.TabIndex = 0;
            this.dgrKHDongThung.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.bandedGridView1});
            // 
            // bandedGridView1
            // 
            this.bandedGridView1.Appearance.FocusedRow.BackColor = System.Drawing.Color.Transparent;
            this.bandedGridView1.Appearance.FocusedRow.Options.UseBackColor = true;
            this.bandedGridView1.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] {
            this.gridBandColorID,
            this.gridBandSize,
            this.gridBand_QC_DongThung,
            this.gridBandGC});
            this.bandedGridView1.DetailHeight = 431;
            this.bandedGridView1.GridControl = this.dgrKHDongThung;
            this.bandedGridView1.Name = "bandedGridView1";
            this.bandedGridView1.OptionsView.ShowAutoFilterRow = true;
            this.bandedGridView1.OptionsView.ShowGroupPanel = false;
            this.bandedGridView1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.bandedGridView1_KeyPress);
            // 
            // gridBandColorID
            // 
            this.gridBandColorID.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandColorID.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandColorID.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandColorID.Caption = "Màu";
            this.gridBandColorID.MinWidth = 136;
            this.gridBandColorID.Name = "gridBandColorID";
            this.gridBandColorID.Visible = false;
            this.gridBandColorID.VisibleIndex = -1;
            this.gridBandColorID.Width = 408;
            // 
            // gridBandSize
            // 
            this.gridBandSize.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandSize.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandSize.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandSize.Caption = "Size";
            this.gridBandSize.MinWidth = 476;
            this.gridBandSize.Name = "gridBandSize";
            this.gridBandSize.VisibleIndex = 0;
            this.gridBandSize.Width = 602;
            // 
            // gridBand_QC_DongThung
            // 
            this.gridBand_QC_DongThung.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBand_QC_DongThung.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBand_QC_DongThung.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBand_QC_DongThung.Caption = "Qui cách đóng thùng (dài x rộng x cao)";
            this.gridBand_QC_DongThung.MinWidth = 199;
            this.gridBand_QC_DongThung.Name = "gridBand_QC_DongThung";
            this.gridBand_QC_DongThung.VisibleIndex = 1;
            this.gridBand_QC_DongThung.Width = 199;
            // 
            // gridBandGC
            // 
            this.gridBandGC.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBandGC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBandGC.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridBandGC.Caption = "Ghi chú";
            this.gridBandGC.MinWidth = 341;
            this.gridBandGC.Name = "gridBandGC";
            this.gridBandGC.VisibleIndex = 2;
            this.gridBandGC.Width = 341;
            // 
            // frmKHDongThungPCB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1198, 450);
            this.Controls.Add(this.splitContainerControl);
            this.Controls.Add(this.barDockControl4);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmKHDongThungPCB";
            this.Text = "PCB đóng thùng";
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
            this.splitContainerControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrMaHang)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewMaHang)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgrKHDongThung)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bandedGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem btSave;
        private DevExpress.XtraBars.BarButtonItem btCancel;
        private DevExpress.XtraBars.BarEditItem barEditItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem btAdd;
        private DevExpress.XtraBars.BarButtonItem btDelete;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl;
        private DevExpress.XtraGrid.GridControl dgrMaHang;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewMaHang;
        private DevExpress.XtraGrid.Columns.GridColumn StyleID;
        private DevExpress.XtraGrid.GridControl dgrKHDongThung;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridView bandedGridView1;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandColorID;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandSize;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand_QC_DongThung;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBandGC;
        private DevExpress.XtraBars.BarDockControl barDockControl4;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.BarEditItem barEditItem2;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit2;
        private DevExpress.XtraGrid.Columns.GridColumn colMaHang;
    }
}