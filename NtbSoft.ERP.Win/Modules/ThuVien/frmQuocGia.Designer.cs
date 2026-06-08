
using System.Drawing;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    partial class frmQuocGia
    {
        private const bool V = false;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmQuocGia));
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.Them = new DevExpress.XtraBars.BarButtonItem();
            this.Sua = new DevExpress.XtraBars.BarButtonItem();
            this.Xoa = new DevExpress.XtraBars.BarButtonItem();
            this.Luu = new DevExpress.XtraBars.BarButtonItem();
            this.Naplai = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.gridQuocGia = new DevExpress.XtraGrid.GridControl();
            this.gridViewQuocGia = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaQG = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenQG = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColMaBuuChinh = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColMaDienThoai = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColMaQK = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemImageComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.colGhiChu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaCang = new DevExpress.XtraGrid.Columns.GridColumn();
            #region Thoai
            this.colNguoiTao = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNgayTao = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNguoiSua = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNgaySua = new DevExpress.XtraGrid.Columns.GridColumn();
            #endregion
            this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridQuocGia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewQuocGia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).BeginInit();
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
            this.barButtonItem1});
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
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1)});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.RotateWhenVertical = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Tools";
            // 
            // Them
            // 
            this.Them.Caption = "Thêm (Ctrl + Shift + N)";
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
            this.Sua.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Sua_ItemClick);
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
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Import Excel";
            this.barButtonItem1.Id = 5;
            this.barButtonItem1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.Image")));
            this.barButtonItem1.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.LargeImage")));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlTop.Size = new System.Drawing.Size(1009, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 490);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(1009, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 454);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl1.Location = new System.Drawing.Point(1009, 36);
            this.barDockControl1.Manager = this.barManager1;
            this.barDockControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControl1.Size = new System.Drawing.Size(0, 454);
            // 
            // gridQuocGia
            // 
            this.gridQuocGia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridQuocGia.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.gridQuocGia.Location = new System.Drawing.Point(0, 36);
            this.gridQuocGia.LookAndFeel.SkinName = "Office 2010 Blue";
            this.gridQuocGia.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridQuocGia.MainView = this.gridViewQuocGia;
            this.gridQuocGia.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridQuocGia.MenuManager = this.barManager1;
            this.gridQuocGia.Name = "gridQuocGia";
            this.gridQuocGia.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemImageComboBox1,
            this.repositoryItemButtonEdit1});
            this.gridQuocGia.Size = new System.Drawing.Size(1009, 454);
            this.gridQuocGia.TabIndex = 9;
            this.gridQuocGia.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewQuocGia});
            this.gridQuocGia.EditorKeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gridControlQuocGia_EditorKeyPress);
            // 
            // gridViewQuocGia
            // 
            this.gridViewQuocGia.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(209)))));
            this.gridViewQuocGia.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewQuocGia.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9.75F);
            this.gridViewQuocGia.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewQuocGia.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridViewQuocGia.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridViewQuocGia.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridViewQuocGia.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colID,
            this.colMaQG,
            this.colTenQG,
            this.gridColMaBuuChinh,
            this.gridColMaDienThoai,
            this.gridColMaQK,
            this.colGhiChu,
            this.gridColumn1,
            this.colCang,
            this.colMaCang,
            //Thoai thêm 4 cột mới
            this.colNguoiTao,
            this.colNgayTao,
            this.colNguoiSua,
            this.colNgaySua});
            this.gridViewQuocGia.DetailHeight = 431;
            this.gridViewQuocGia.GridControl = this.gridQuocGia;
            this.gridViewQuocGia.IndicatorWidth = 47;
            this.gridViewQuocGia.Name = "gridViewQuocGia";
            this.gridViewQuocGia.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewQuocGia.OptionsCustomization.AllowFilter = false;
            this.gridViewQuocGia.OptionsCustomization.AllowSort = false;
            this.gridViewQuocGia.OptionsView.ColumnAutoWidth = false;
            this.gridViewQuocGia.OptionsView.ShowAutoFilterRow = true;
            this.gridViewQuocGia.OptionsView.ShowGroupPanel = false;
            this.gridViewQuocGia.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridViewQuocGia_RowCellClick);
            this.gridViewQuocGia.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewQuocGia_CustomDrawColumnHeader);
            this.gridViewQuocGia.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridViewQuocGia_CustomDrawRowIndicator);
            this.gridViewQuocGia.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewQuocGia_FocusedRowChanged);
            this.gridViewQuocGia.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.gridViewQuocGia_FocusedColumnChanged);
            this.gridViewQuocGia.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewQuocGia_CellValueChanged);
            this.gridViewQuocGia.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewQuocGia_CellValueChanging);
            this.gridViewQuocGia.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridViewQuocGia_CustomColumnDisplayText);
            this.gridViewQuocGia.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gridViewQuocGia_KeyPress_1);
            this.gridViewQuocGia.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gridViewQuocGia_ValidatingEditor);
            // 
            // colID
            // 
            this.colID.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colID.AppearanceHeader.Options.UseFont = true;
            this.colID.Caption = "ID";
            this.colID.FieldName = "ID";
            this.colID.MinWidth = 23;
            this.colID.Name = "colID";
            this.colID.Width = 87;
            // 
            // colMaQG
            // 
            this.colMaQG.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaQG.AppearanceHeader.Options.UseFont = true;
            this.colMaQG.Caption = "Mã quốc gia";
            this.colMaQG.FieldName = "MaQG";
            this.colMaQG.MinWidth = 23;
            this.colMaQG.Name = "colMaQG";
            this.colMaQG.Width = 143;
            // 
            // colTenQG
            // 
            this.colTenQG.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colTenQG.AppearanceHeader.Options.UseFont = true;
            this.colTenQG.Caption = "Quốc gia";
            this.colTenQG.FieldName = "TenQG";
            this.colTenQG.MinWidth = 23;
            this.colTenQG.Name = "colTenQG";
            this.colTenQG.Visible = true;
            this.colTenQG.VisibleIndex = 0;
            this.colTenQG.Width = 149;
            // 
            // gridColMaBuuChinh
            // 
            this.gridColMaBuuChinh.Caption = "Mã bưu chính";
            this.gridColMaBuuChinh.FieldName = "MaBuuChinh";
            this.gridColMaBuuChinh.MinWidth = 24;
            this.gridColMaBuuChinh.Name = "gridColMaBuuChinh";
            this.gridColMaBuuChinh.Visible = true;
            this.gridColMaBuuChinh.VisibleIndex = 2;
            this.gridColMaBuuChinh.Width = 120;
            // 
            // gridColMaDienThoai
            // 
            this.gridColMaDienThoai.Caption = "Mã điện thoại";
            this.gridColMaDienThoai.FieldName = "MaDienThoai";
            this.gridColMaDienThoai.MinWidth = 24;
            this.gridColMaDienThoai.Name = "gridColMaDienThoai";
            this.gridColMaDienThoai.Visible = true;
            this.gridColMaDienThoai.VisibleIndex = 3;
            this.gridColMaDienThoai.Width = 120;
            // 
            // gridColMaQK
            // 
            this.gridColMaQK.Caption = "Quốc kỳ";
            this.gridColMaQK.ColumnEdit = this.repositoryItemImageComboBox1;
            this.gridColMaQK.FieldName = "MaQuocKy";
            this.gridColMaQK.MinWidth = 24;
            this.gridColMaQK.Name = "gridColMaQK";
            this.gridColMaQK.Visible = true;
            this.gridColMaQK.VisibleIndex = 4;
            this.gridColMaQK.Width = 125;
            // 
            // repositoryItemImageComboBox1
            // 
            this.repositoryItemImageComboBox1.AutoHeight = false;
            this.repositoryItemImageComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemImageComboBox1.Name = "repositoryItemImageComboBox1";
            this.repositoryItemImageComboBox1.NullText = "Chọn quốc kỳ";
            this.repositoryItemImageComboBox1.ShowToolTipForTrimmedText = DevExpress.Utils.DefaultBoolean.False;
            this.repositoryItemImageComboBox1.EditValueChanged += new System.EventHandler(this.repositoryItemImageComboBox1_EditValueChanged);
            // 
            // colGhiChu
            // 
            this.colGhiChu.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colGhiChu.AppearanceHeader.Options.UseFont = true;
            this.colGhiChu.Caption = "Ghi chú";
            this.colGhiChu.FieldName = "GhiChu";
            this.colGhiChu.MinWidth = 23;
            this.colGhiChu.Name = "colGhiChu";
            this.colGhiChu.Visible = true;
            this.colGhiChu.VisibleIndex = 6;
            this.colGhiChu.Width = 150;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Tên tiếng Anh";
            this.gridColumn1.FieldName = "TenTiengAnh";
            this.gridColumn1.MinWidth = 24;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            this.gridColumn1.Width = 153;
            // 
            // colCang
            // 
            this.colCang.Caption = "Cảng";
            this.colCang.FieldName = "TenCang";
            this.colCang.MinWidth = 23;
            this.colCang.Name = "colCang";
            this.colCang.Visible = true;
            this.colCang.VisibleIndex = 5;
            this.colCang.Width = 301;
            // 
            // colMaCang
            // 
            this.colMaCang.Caption = "Mã Cảng";
            this.colMaCang.FieldName = "MaCang";
            this.colMaCang.MinWidth = 23;
            this.colMaCang.Name = "colMaCang";
            this.colMaCang.Width = 87;
            #region Thoai thêm 4 cột mới
            // 
            // colNguoiTao
            // 
            this.colNguoiTao.Caption = "Người Tạo";
            this.colNguoiTao.FieldName = "NguoiTao";
            this.colNguoiTao.MinWidth = 23;
            this.colNguoiTao.Name = "colNguoiTao";
            this.colNguoiTao.OptionsColumn.AllowEdit = false;
            this.colNguoiTao.OptionsColumn.AllowFocus = false;
            this.colNguoiTao.Visible = true;
            this.colNguoiTao.VisibleIndex = 7;
            this.colNguoiTao.Width = 100;
            // 
            // colNgayTao
            // 
            this.colNgayTao.Caption = "Ngày Tạo";
            this.colNgayTao.FieldName = "NgayTao";
            this.colNgayTao.MinWidth = 23;
            this.colNgayTao.Name = "colNgayTao";
            this.colNgayTao.Width = 87;
            // 
            // colNguoiSua
            // 
            this.colNguoiSua.Caption = "Người Sửa";
            this.colNguoiSua.FieldName = "NguoiSua";
            this.colNguoiSua.MinWidth = 23;
            this.colNguoiSua.Name = "colNguoiSua";
            this.colNguoiSua.OptionsColumn.AllowEdit = false;
            this.colNguoiSua.OptionsColumn.AllowFocus = false;
            this.colNguoiSua.Visible = true;
            this.colNguoiSua.VisibleIndex = 8;
            this.colNguoiSua.Width = 100;
            // 
            // colNgaySua
            // 
            this.colNgaySua.Caption = "Ngày Sửa";
            this.colNgaySua.FieldName = "NgaySua";
            this.colNgaySua.MinWidth = 23;
            this.colNgaySua.Name = "colNgaySua";
            this.colNgaySua.Width = 87;
            #endregion
            // 
            // repositoryItemButtonEdit1
            // 
            this.repositoryItemButtonEdit1.AutoHeight = false;
            this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(881, 6);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // frmQuocGia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1009, 490);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.gridQuocGia);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControl1);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.LookAndFeel.SkinName = "Office 2010 Blue";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmQuocGia";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quốc Gia";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridQuocGia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewQuocGia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).EndInit();
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
        private DevExpress.XtraGrid.GridControl gridQuocGia;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewQuocGia;
        private DevExpress.XtraGrid.Columns.GridColumn colID;
        private DevExpress.XtraGrid.Columns.GridColumn colMaQG;
        private DevExpress.XtraGrid.Columns.GridColumn colTenQG;
        private DevExpress.XtraGrid.Columns.GridColumn colGhiChu;
        private System.Windows.Forms.Button button1;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox repositoryItemImageComboBox1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColMaBuuChinh;
        private DevExpress.XtraGrid.Columns.GridColumn gridColMaDienThoai;
        private DevExpress.XtraGrid.Columns.GridColumn gridColMaQK;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraGrid.Columns.GridColumn colCang;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn colMaCang;
        #region Thoai
        private DevExpress.XtraGrid.Columns.GridColumn colNguoiTao;
        private DevExpress.XtraGrid.Columns.GridColumn colNgayTao;
        private DevExpress.XtraGrid.Columns.GridColumn colNguoiSua;
        private DevExpress.XtraGrid.Columns.GridColumn colNgaySua;
        #endregion
    }
}