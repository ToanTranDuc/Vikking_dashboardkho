
namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    partial class frmDonViSanXuatDetail
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDonViSanXuatDetail));
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
            this.gridDonViSanXuat = new DevExpress.XtraGrid.GridControl();
            this.gridViewDonViSanXuat = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaDVSX = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenDVSX = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGiaCong = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CheckGiaCong = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDonViSanXuat = new DevExpress.XtraGrid.Columns.GridColumn();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDonViSanXuat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDonViSanXuat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckGiaCong)).BeginInit();
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
            this.barDockControlTop.Size = new System.Drawing.Size(978, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 570);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(978, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 534);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl1.Location = new System.Drawing.Point(978, 36);
            this.barDockControl1.Manager = this.barManager1;
            this.barDockControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControl1.Size = new System.Drawing.Size(0, 534);
            // 
            // gridDonViSanXuat
            // 
            this.gridDonViSanXuat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDonViSanXuat.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.gridDonViSanXuat.Location = new System.Drawing.Point(0, 36);
            this.gridDonViSanXuat.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridDonViSanXuat.MainView = this.gridViewDonViSanXuat;
            this.gridDonViSanXuat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridDonViSanXuat.MenuManager = this.barManager1;
            this.gridDonViSanXuat.Name = "gridDonViSanXuat";
            this.gridDonViSanXuat.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.CheckGiaCong});
            this.gridDonViSanXuat.Size = new System.Drawing.Size(978, 534);
            this.gridDonViSanXuat.TabIndex = 16;
            this.gridDonViSanXuat.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDonViSanXuat});
            this.gridDonViSanXuat.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.gridDonViSanXuat_ProcessGridKey);
            // 
            // gridViewDonViSanXuat
            // 
            this.gridViewDonViSanXuat.Appearance.ColumnFilterButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(209)))));
            this.gridViewDonViSanXuat.Appearance.ColumnFilterButton.Options.UseBackColor = true;
            this.gridViewDonViSanXuat.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(209)))));
            this.gridViewDonViSanXuat.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewDonViSanXuat.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.gridViewDonViSanXuat.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewDonViSanXuat.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridViewDonViSanXuat.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridViewDonViSanXuat.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridViewDonViSanXuat.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.colMaDVSX,
            this.colTenDVSX,
            this.colGiaCong,
            this.gridColumn2,
            this.colDonViSanXuat});
            this.gridViewDonViSanXuat.DetailHeight = 431;
            this.gridViewDonViSanXuat.GridControl = this.gridDonViSanXuat;
            this.gridViewDonViSanXuat.IndicatorWidth = 47;
            this.gridViewDonViSanXuat.Name = "gridViewDonViSanXuat";
            this.gridViewDonViSanXuat.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewDonViSanXuat.OptionsCustomization.AllowFilter = false;
            this.gridViewDonViSanXuat.OptionsCustomization.AllowSort = false;
            this.gridViewDonViSanXuat.OptionsView.ColumnAutoWidth = false;
            this.gridViewDonViSanXuat.OptionsView.ShowAutoFilterRow = true;
            this.gridViewDonViSanXuat.OptionsView.ShowGroupPanel = false;
            this.gridViewDonViSanXuat.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewDonViSanXuat_CustomDrawColumnHeader);
            this.gridViewDonViSanXuat.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridViewDonViSanXuat_CustomDrawRowIndicator);
            this.gridViewDonViSanXuat.RowCountChanged += new System.EventHandler(this.gridViewDonViSanXuat_RowCountChanged);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.gridColumn1.AppearanceHeader.Options.UseFont = true;
            this.gridColumn1.Caption = "ID";
            this.gridColumn1.FieldName = "ID";
            this.gridColumn1.MinWidth = 23;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Width = 87;
            // 
            // colMaDVSX
            // 
            this.colMaDVSX.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaDVSX.AppearanceHeader.Options.UseFont = true;
            this.colMaDVSX.Caption = "Mã đơn vị sản xuất";
            this.colMaDVSX.FieldName = "MaDVSX";
            this.colMaDVSX.MinWidth = 23;
            this.colMaDVSX.Name = "colMaDVSX";
            this.colMaDVSX.OptionsColumn.ReadOnly = true;
            this.colMaDVSX.Width = 181;
            // 
            // colTenDVSX
            // 
            this.colTenDVSX.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colTenDVSX.AppearanceHeader.Options.UseFont = true;
            this.colTenDVSX.Caption = "Ngày tạo";
            this.colTenDVSX.DisplayFormat.FormatString = "dd/MM/yyyy hh:mm";
            this.colTenDVSX.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colTenDVSX.FieldName = "NgayTao";
            this.colTenDVSX.MinWidth = 23;
            this.colTenDVSX.Name = "colTenDVSX";
            this.colTenDVSX.OptionsColumn.ReadOnly = true;
            this.colTenDVSX.Visible = true;
            this.colTenDVSX.VisibleIndex = 2;
            this.colTenDVSX.Width = 222;
            // 
            // colGiaCong
            // 
            this.colGiaCong.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colGiaCong.AppearanceHeader.Options.UseFont = true;
            this.colGiaCong.Caption = "Gia công";
            this.colGiaCong.ColumnEdit = this.CheckGiaCong;
            this.colGiaCong.FieldName = "GiaCong";
            this.colGiaCong.MinWidth = 24;
            this.colGiaCong.Name = "colGiaCong";
            this.colGiaCong.OptionsColumn.ReadOnly = true;
            this.colGiaCong.Visible = true;
            this.colGiaCong.VisibleIndex = 1;
            this.colGiaCong.Width = 94;
            // 
            // CheckGiaCong
            // 
            this.CheckGiaCong.AutoHeight = false;
            this.CheckGiaCong.Name = "CheckGiaCong";
            this.CheckGiaCong.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.gridColumn2.AppearanceHeader.Options.UseFont = true;
            this.gridColumn2.Caption = "Người tạo";
            this.gridColumn2.FieldName = "NguoiTao";
            this.gridColumn2.MinWidth = 23;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.ReadOnly = true;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 3;
            this.gridColumn2.Width = 343;
            // 
            // colDonViSanXuat
            // 
            this.colDonViSanXuat.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colDonViSanXuat.AppearanceHeader.Options.UseFont = true;
            this.colDonViSanXuat.Caption = "Đơn vị sản xuất";
            this.colDonViSanXuat.FieldName = "DonViSanXuat";
            this.colDonViSanXuat.MinWidth = 24;
            this.colDonViSanXuat.Name = "colDonViSanXuat";
            this.colDonViSanXuat.OptionsColumn.ReadOnly = true;
            this.colDonViSanXuat.Visible = true;
            this.colDonViSanXuat.VisibleIndex = 0;
            this.colDonViSanXuat.Width = 208;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(537, 0);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 21;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // frmDonViSanXuatDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 570);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.gridDonViSanXuat);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControl1);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.LookAndFeel.SkinName = "London Liquid Sky";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDonViSanXuatDetail";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đơn vị sản xuất chi tiết";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDonViSanXuat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewDonViSanXuat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckGiaCong)).EndInit();
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
        private DevExpress.XtraGrid.GridControl gridDonViSanXuat;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewDonViSanXuat;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn colMaDVSX;
        private DevExpress.XtraGrid.Columns.GridColumn colTenDVSX;
        private DevExpress.XtraGrid.Columns.GridColumn colGiaCong;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit CheckGiaCong;
        private System.Windows.Forms.Button button1;
        private DevExpress.XtraGrid.Columns.GridColumn colDonViSanXuat;
    }
}