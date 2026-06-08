
namespace NtbSoft.ERP.Win.Modules.ViTriKho
{
    partial class frmSoDoKho
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSoDoKho));
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.btnThem = new DevExpress.XtraBars.BarButtonItem();
            this.btnLuu = new DevExpress.XtraBars.BarButtonItem();
            this.btnXoa = new DevExpress.XtraBars.BarButtonItem();
            this.btnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.dgrViTriKho = new DevExpress.XtraGrid.GridControl();
            this.gridViewViTriKho = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMaKho = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaVT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenVT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCBM = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colChon = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.dgrSoDoKho = new DevExpress.XtraGrid.GridControl();
            this.gridViewSoDoKho = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrViTriKho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewViTriKho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.xtraScrollableControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrSoDoKho)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSoDoKho)).BeginInit();
            this.SuspendLayout();
            // 
            // barAndDockingController1
            // 
            this.barAndDockingController1.LookAndFeel.SkinMaskColor = System.Drawing.Color.DodgerBlue;
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
            this.btnThem,
            this.btnLuu,
            this.btnXoa,
            this.btnRefresh,
            this.barButtonItem1,
            this.barButtonItem3});
            this.barManager1.MaxItemId = 16;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnThem),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnLuu),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnXoa),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnRefresh)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // btnThem
            // 
            this.btnThem.Caption = "Thêm";
            this.btnThem.Id = 0;
            this.btnThem.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnThem.ImageOptions.Image")));
            this.btnThem.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnThem.ImageOptions.LargeImage")));
            this.btnThem.Name = "btnThem";
            this.btnThem.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btnLuu
            // 
            this.btnLuu.Caption = "Lưu";
            this.btnLuu.Id = 1;
            this.btnLuu.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnLuu.ImageOptions.Image")));
            this.btnLuu.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnLuu.ImageOptions.LargeImage")));
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btnXoa
            // 
            this.btnXoa.Caption = "Xóa";
            this.btnXoa.Id = 2;
            this.btnXoa.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnXoa.ImageOptions.Image")));
            this.btnXoa.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnXoa.ImageOptions.LargeImage")));
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Caption = "Làm Mới";
            this.btnRefresh.Id = 4;
            this.btnRefresh.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.ImageOptions.Image")));
            this.btnRefresh.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnRefresh.ImageOptions.LargeImage")));
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(1421, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 764);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(1421, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 735);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1421, 29);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 735);
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Id = 7;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // barButtonItem3
            // 
            this.barButtonItem3.Caption = "barButtonItem3";
            this.barButtonItem3.Id = 15;
            this.barButtonItem3.Name = "barButtonItem3";
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 29);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.dgrViTriKho);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.xtraScrollableControl1);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(1421, 735);
            this.splitContainerControl1.SplitterPosition = 428;
            this.splitContainerControl1.TabIndex = 0;
            // 
            // dgrViTriKho
            // 
            this.dgrViTriKho.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgrViTriKho.Location = new System.Drawing.Point(0, 0);
            this.dgrViTriKho.MainView = this.gridViewViTriKho;
            this.dgrViTriKho.MenuManager = this.barManager1;
            this.dgrViTriKho.Name = "dgrViTriKho";
            this.dgrViTriKho.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.dgrViTriKho.Size = new System.Drawing.Size(428, 735);
            this.dgrViTriKho.TabIndex = 0;
            this.dgrViTriKho.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewViTriKho});
            // 
            // gridViewViTriKho
            // 
            this.gridViewViTriKho.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(226)))), ((int)(((byte)(255)))));
            this.gridViewViTriKho.Appearance.FocusedCell.BackColor2 = System.Drawing.Color.White;
            this.gridViewViTriKho.Appearance.FocusedCell.ForeColor = System.Drawing.Color.Black;
            this.gridViewViTriKho.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gridViewViTriKho.Appearance.FocusedCell.Options.UseForeColor = true;
            this.gridViewViTriKho.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(226)))), ((int)(((byte)(255)))));
            this.gridViewViTriKho.Appearance.FocusedRow.BackColor2 = System.Drawing.Color.White;
            this.gridViewViTriKho.Appearance.FocusedRow.ForeColor = System.Drawing.Color.Black;
            this.gridViewViTriKho.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewViTriKho.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gridViewViTriKho.Appearance.HideSelectionRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(226)))), ((int)(((byte)(255)))));
            this.gridViewViTriKho.Appearance.HideSelectionRow.BackColor2 = System.Drawing.Color.White;
            this.gridViewViTriKho.Appearance.HideSelectionRow.ForeColor = System.Drawing.Color.Black;
            this.gridViewViTriKho.Appearance.HideSelectionRow.Options.UseBackColor = true;
            this.gridViewViTriKho.Appearance.HideSelectionRow.Options.UseForeColor = true;
            this.gridViewViTriKho.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(226)))), ((int)(((byte)(255)))));
            this.gridViewViTriKho.Appearance.SelectedRow.BackColor2 = System.Drawing.Color.White;
            this.gridViewViTriKho.Appearance.SelectedRow.ForeColor = System.Drawing.Color.Black;
            this.gridViewViTriKho.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gridViewViTriKho.Appearance.SelectedRow.Options.UseForeColor = true;
            this.gridViewViTriKho.ColumnPanelRowHeight = 30;
            this.gridViewViTriKho.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMaKho,
            this.colMaVT,
            this.colTenVT,
            this.colCBM,
            this.colChon});
            this.gridViewViTriKho.GridControl = this.dgrViTriKho;
            this.gridViewViTriKho.GroupCount = 1;
            this.gridViewViTriKho.Name = "gridViewViTriKho";
            this.gridViewViTriKho.OptionsMenu.ShowConditionalFormattingItem = true;
            this.gridViewViTriKho.OptionsView.ColumnAutoWidth = false;
            this.gridViewViTriKho.OptionsView.ShowGroupPanel = false;
            this.gridViewViTriKho.RowHeight = 30;
            this.gridViewViTriKho.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colMaKho, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.gridViewViTriKho.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewViTriKho_CustomColumnDisplayText);
            // 
            // colMaKho
            // 
            this.colMaKho.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colMaKho.AppearanceCell.Options.UseFont = true;
            this.colMaKho.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colMaKho.AppearanceHeader.Options.UseFont = true;
            this.colMaKho.AppearanceHeader.Options.UseTextOptions = true;
            this.colMaKho.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaKho.Caption = "Kho";
            this.colMaKho.FieldName = "MaKho";
            this.colMaKho.Name = "colMaKho";
            this.colMaKho.OptionsColumn.AllowEdit = false;
            this.colMaKho.Visible = true;
            this.colMaKho.VisibleIndex = 0;
            // 
            // colMaVT
            // 
            this.colMaVT.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colMaVT.AppearanceCell.Options.UseFont = true;
            this.colMaVT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colMaVT.AppearanceHeader.Options.UseFont = true;
            this.colMaVT.AppearanceHeader.Options.UseTextOptions = true;
            this.colMaVT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaVT.Caption = "Mã vị trí";
            this.colMaVT.FieldName = "MaVT";
            this.colMaVT.MaxWidth = 120;
            this.colMaVT.MinWidth = 80;
            this.colMaVT.Name = "colMaVT";
            this.colMaVT.OptionsColumn.AllowEdit = false;
            this.colMaVT.Visible = true;
            this.colMaVT.VisibleIndex = 0;
            this.colMaVT.Width = 118;
            // 
            // colTenVT
            // 
            this.colTenVT.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colTenVT.AppearanceCell.Options.UseFont = true;
            this.colTenVT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colTenVT.AppearanceHeader.Options.UseFont = true;
            this.colTenVT.AppearanceHeader.Options.UseTextOptions = true;
            this.colTenVT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colTenVT.Caption = "Tên vị trí";
            this.colTenVT.FieldName = "TenVT";
            this.colTenVT.MaxWidth = 180;
            this.colTenVT.MinWidth = 100;
            this.colTenVT.Name = "colTenVT";
            this.colTenVT.OptionsColumn.AllowEdit = false;
            this.colTenVT.Visible = true;
            this.colTenVT.VisibleIndex = 1;
            this.colTenVT.Width = 150;
            // 
            // colCBM
            // 
            this.colCBM.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colCBM.AppearanceCell.Options.UseFont = true;
            this.colCBM.AppearanceCell.Options.UseTextOptions = true;
            this.colCBM.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colCBM.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colCBM.AppearanceHeader.Options.UseFont = true;
            this.colCBM.AppearanceHeader.Options.UseTextOptions = true;
            this.colCBM.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colCBM.Caption = "CBM";
            this.colCBM.FieldName = "CBM";
            this.colCBM.MaxWidth = 100;
            this.colCBM.MinWidth = 50;
            this.colCBM.Name = "colCBM";
            this.colCBM.OptionsColumn.AllowEdit = false;
            this.colCBM.Visible = true;
            this.colCBM.VisibleIndex = 2;
            // 
            // colChon
            // 
            this.colChon.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colChon.AppearanceHeader.Options.UseFont = true;
            this.colChon.AppearanceHeader.Options.UseTextOptions = true;
            this.colChon.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colChon.Caption = "Chọn";
            this.colChon.ColumnEdit = this.repositoryItemCheckEdit1;
            this.colChon.FieldName = "Chon";
            this.colChon.MaxWidth = 50;
            this.colChon.MinWidth = 50;
            this.colChon.Name = "colChon";
            this.colChon.Visible = true;
            this.colChon.VisibleIndex = 3;
            this.colChon.Width = 50;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.repositoryItemCheckEdit1.CheckedChanged += new System.EventHandler(this.repositoryItemCheckEdit1_CheckedChanged);
            // 
            // xtraScrollableControl1
            // 
            this.xtraScrollableControl1.AllowTouchScroll = true;
            this.xtraScrollableControl1.Controls.Add(this.dgrSoDoKho);
            this.xtraScrollableControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraScrollableControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraScrollableControl1.Name = "xtraScrollableControl1";
            this.xtraScrollableControl1.Size = new System.Drawing.Size(985, 735);
            this.xtraScrollableControl1.TabIndex = 1;
            // 
            // dgrSoDoKho
            // 
            this.dgrSoDoKho.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgrSoDoKho.Location = new System.Drawing.Point(0, 0);
            this.dgrSoDoKho.LookAndFeel.SkinName = "Office 2019 Colorful";
            this.dgrSoDoKho.LookAndFeel.UseDefaultLookAndFeel = false;
            this.dgrSoDoKho.LookAndFeel.UseWindowsXPTheme = true;
            this.dgrSoDoKho.MainView = this.gridViewSoDoKho;
            this.dgrSoDoKho.MenuManager = this.barManager1;
            this.dgrSoDoKho.Name = "dgrSoDoKho";
            this.dgrSoDoKho.Size = new System.Drawing.Size(985, 157);
            this.dgrSoDoKho.TabIndex = 0;
            this.dgrSoDoKho.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSoDoKho});
            this.dgrSoDoKho.Visible = false;
            // 
            // gridViewSoDoKho
            // 
            this.gridViewSoDoKho.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(226)))), ((int)(((byte)(255)))));
            this.gridViewSoDoKho.Appearance.FocusedCell.Options.UseBackColor = true;
            this.gridViewSoDoKho.Appearance.Row.BorderColor = System.Drawing.Color.Navy;
            this.gridViewSoDoKho.Appearance.Row.Options.UseBorderColor = true;
            this.gridViewSoDoKho.ColumnPanelRowHeight = 30;
            this.gridViewSoDoKho.GridControl = this.dgrSoDoKho;
            this.gridViewSoDoKho.Name = "gridViewSoDoKho";
            this.gridViewSoDoKho.OptionsMenu.ShowConditionalFormattingItem = true;
            this.gridViewSoDoKho.OptionsView.AllowCellMerge = true;
            this.gridViewSoDoKho.OptionsView.ShowColumnHeaders = false;
            this.gridViewSoDoKho.OptionsView.ShowGroupPanel = false;
            this.gridViewSoDoKho.OptionsView.ShowIndicator = false;
            this.gridViewSoDoKho.RowHeight = 30;
            this.gridViewSoDoKho.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gridViewSoDoKho_PopupMenuShowing);
            this.gridViewSoDoKho.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewSoDoKho_CustomColumnDisplayText);
            // 
            // frmSoDoKho
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1421, 764);
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmSoDoKho";
            this.Text = "Sơ đồ kho";
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrViTriKho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewViTriKho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.xtraScrollableControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgrSoDoKho)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSoDoKho)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem btnThem;
        private DevExpress.XtraBars.BarButtonItem btnLuu;
        private DevExpress.XtraBars.BarButtonItem btnXoa;
        private DevExpress.XtraBars.BarButtonItem btnRefresh;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraGrid.GridControl dgrViTriKho;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewViTriKho;
        private DevExpress.XtraGrid.Columns.GridColumn colMaKho;
        private DevExpress.XtraGrid.Columns.GridColumn colMaVT;
        private DevExpress.XtraGrid.Columns.GridColumn colTenVT;
        private DevExpress.XtraGrid.Columns.GridColumn colCBM;
        private DevExpress.XtraGrid.GridControl dgrSoDoKho;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewSoDoKho;
        private DevExpress.XtraGrid.Columns.GridColumn colChon;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
    }
}