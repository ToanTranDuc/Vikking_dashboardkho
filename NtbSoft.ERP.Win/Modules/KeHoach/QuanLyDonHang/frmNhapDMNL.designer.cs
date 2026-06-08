
using DevExpress.XtraEditors.Repository;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    partial class frmNhapDMNL
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNhapDMNL));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.Them = new DevExpress.XtraBars.BarButtonItem();
            this.Sua = new DevExpress.XtraBars.BarButtonItem();
            this.Xoa = new DevExpress.XtraBars.BarButtonItem();
            this.Luu = new DevExpress.XtraBars.BarButtonItem();
            this.Naplai = new DevExpress.XtraBars.BarButtonItem();
            this.Export = new DevExpress.XtraBars.BarButtonItem();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.gridControlNhapDMNL = new DevExpress.XtraGrid.GridControl();
            this.gridViewNhapDMNL = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaNPL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.searchLookUpEditNPL = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.gridViewLookUpNPL = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMaNPLSearchLookup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaVTSearchLookup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenVTSearchLookup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaMauNPLSearchLookup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenMauNPLSearchLookup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaVT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenVT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaMau = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemSearchLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.repositoryItemSearchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colKhoVai = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDonVi = new DevExpress.XtraGrid.Columns.GridColumn();
            this.searchLookUpEditMaDonVi = new DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit();
            this.gridViewLookUpMaDonVi = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colMaDonVi = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTenDonVi = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDinhMuc = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSoLuong = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCapPhat = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCapThem = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colThuHoi = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGhiChu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MaMauNPL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.TenMauNPL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlNhapDMNL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewNhapDMNL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditNPL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLookUpNPL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditMaDonVi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLookUpMaDonVi)).BeginInit();
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
            this.Export});
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
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.None, false, this.Export, false)});
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
            // Export
            // 
            this.Export.Alignment = DevExpress.XtraBars.BarItemLinkAlignment.Right;
            this.Export.Caption = "Xuất excel";
            this.Export.Id = 5;
            this.Export.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("Export.ImageOptions.Image")));
            this.Export.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("Export.ImageOptions.LargeImage")));
            this.Export.ItemAppearance.Disabled.Font = new System.Drawing.Font("Tahoma", 7.9F);
            this.Export.ItemAppearance.Disabled.Options.UseFont = true;
            this.Export.ItemAppearance.Hovered.Font = new System.Drawing.Font("Tahoma", 7.9F);
            this.Export.ItemAppearance.Hovered.Options.UseFont = true;
            this.Export.ItemAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 7.9F);
            this.Export.ItemAppearance.Normal.Options.UseFont = true;
            this.Export.ItemAppearance.Pressed.Font = new System.Drawing.Font("Tahoma", 7.9F);
            this.Export.ItemAppearance.Pressed.Options.UseFont = true;
            this.Export.Name = "Export";
            this.Export.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Export.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btExcel_ItemClick);
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
            this.barDockControlTop.Size = new System.Drawing.Size(1558, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 484);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(1558, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 448);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl1.Location = new System.Drawing.Point(1558, 36);
            this.barDockControl1.Manager = this.barManager1;
            this.barDockControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControl1.Size = new System.Drawing.Size(0, 448);
            // 
            // gridControlNhapDMNL
            // 
            this.gridControlNhapDMNL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlNhapDMNL.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.gridControlNhapDMNL.Location = new System.Drawing.Point(0, 36);
            this.gridControlNhapDMNL.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlNhapDMNL.MainView = this.gridViewNhapDMNL;
            this.gridControlNhapDMNL.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gridControlNhapDMNL.MenuManager = this.barManager1;
            this.gridControlNhapDMNL.Name = "gridControlNhapDMNL";
            this.gridControlNhapDMNL.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemSearchLookUpEdit1});
            this.gridControlNhapDMNL.Size = new System.Drawing.Size(1558, 448);
            this.gridControlNhapDMNL.TabIndex = 9;
            this.gridControlNhapDMNL.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewNhapDMNL});
            this.gridControlNhapDMNL.EditorKeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gridControlNhapDMNL_EditorKeyPress);
            // 
            // gridViewNhapDMNL
            // 
            this.gridViewNhapDMNL.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(251)))), ((int)(((byte)(209)))));
            this.gridViewNhapDMNL.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gridViewNhapDMNL.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridViewNhapDMNL.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridViewNhapDMNL.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridViewNhapDMNL.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridViewNhapDMNL.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.gridViewNhapDMNL.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colID,
            this.colMaNPL,
            this.colMaVT,
            this.colTenVT,
            this.colMaMau,
            this.colKhoVai,
            this.colDonVi,
            this.colDinhMuc,
            this.colSoLuong,
            this.colCapPhat,
            this.colCapThem,
            this.colThuHoi,
            this.colGhiChu,
            this.MaMauNPL,
            this.TenMauNPL});
            this.gridViewNhapDMNL.DetailHeight = 431;
            this.gridViewNhapDMNL.GridControl = this.gridControlNhapDMNL;
            this.gridViewNhapDMNL.Name = "gridViewNhapDMNL";
            this.gridViewNhapDMNL.OptionsCustomization.AllowColumnMoving = false;
            this.gridViewNhapDMNL.OptionsCustomization.AllowFilter = false;
            this.gridViewNhapDMNL.OptionsCustomization.AllowSort = false;
            this.gridViewNhapDMNL.OptionsView.ColumnAutoWidth = false;
            this.gridViewNhapDMNL.OptionsView.ShowAutoFilterRow = true;
            this.gridViewNhapDMNL.OptionsView.ShowGroupPanel = false;
            this.gridViewNhapDMNL.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridViewNhapDMNL_CustomDrawColumnHeader);
            this.gridViewNhapDMNL.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridViewNhapDMNL_CustomDrawRowIndicator);
            this.gridViewNhapDMNL.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewNhapDMNL_FocusedRowChanged);
            this.gridViewNhapDMNL.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.gridViewNhapDMNL_FocusedColumnChanged);
            this.gridViewNhapDMNL.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewNhapDMNL_CellValueChanging);
            this.gridViewNhapDMNL.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gridViewNhapDMNL_KeyPress);
            this.gridViewNhapDMNL.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gridViewNhapDMNL_ValidatingEditor);
            this.gridViewNhapDMNL.RowCountChanged += new System.EventHandler(this.gridViewNhapDMNL_RowCountChanged);
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
            this.colID.MinWidth = 23;
            this.colID.Name = "colID";
            this.colID.Width = 87;
            // 
            // colMaNPL
            // 
            this.colMaNPL.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaNPL.AppearanceCell.Options.UseFont = true;
            this.colMaNPL.AppearanceCell.Options.UseTextOptions = true;
            this.colMaNPL.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaNPL.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colMaNPL.AppearanceHeader.Options.UseFont = true;
            this.colMaNPL.Caption = "Mã nguyên phụ liệu";
            this.colMaNPL.ColumnEdit = this.searchLookUpEditNPL.Properties;
            this.colMaNPL.FieldName = "MaNPL";
            this.colMaNPL.MinWidth = 23;
            this.colMaNPL.Name = "colMaNPL";
            this.colMaNPL.Visible = true;
            this.colMaNPL.VisibleIndex = 0;
            this.colMaNPL.Width = 163;
            // 
            // searchLookUpEditNPL
            // 
            this.searchLookUpEditNPL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.searchLookUpEditNPL.DisplayMember = "MaNPL";
            this.searchLookUpEditNPL.Name = "searchLookUpEditNPL";
            this.searchLookUpEditNPL.NullText = "Chọn mã vật tư";
            this.searchLookUpEditNPL.PopupView = this.gridViewLookUpNPL;
            this.searchLookUpEditNPL.ShowAddNewButton = true;
            this.searchLookUpEditNPL.ShowClearButton = false;
            this.searchLookUpEditNPL.ShowFooter = false;
            this.searchLookUpEditNPL.ValueMember = "MaNPL";
            this.searchLookUpEditNPL.EditValueChanged += new System.EventHandler(this.searchLookUpEditNPL_EditValueChanged);
            // 
            // gridViewLookUpNPL
            // 
            this.gridViewLookUpNPL.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMaNPLSearchLookup,
            this.colMaVTSearchLookup,
            this.colTenVTSearchLookup,
            this.colMaMauNPLSearchLookup,
            this.colTenMauNPLSearchLookup});
            this.gridViewLookUpNPL.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewLookUpNPL.Name = "gridViewLookUpNPL";
            this.gridViewLookUpNPL.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewLookUpNPL.OptionsView.ShowGroupPanel = false;
            // 
            // colMaNPLSearchLookup
            // 
            this.colMaNPLSearchLookup.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaNPLSearchLookup.AppearanceCell.Options.UseFont = true;
            this.colMaNPLSearchLookup.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colMaNPLSearchLookup.AppearanceHeader.Options.UseFont = true;
            this.colMaNPLSearchLookup.Caption = "Mã NPL";
            this.colMaNPLSearchLookup.FieldName = "MaNPL";
            this.colMaNPLSearchLookup.MinWidth = 23;
            this.colMaNPLSearchLookup.Name = "colMaNPLSearchLookup";
            this.colMaNPLSearchLookup.Visible = true;
            this.colMaNPLSearchLookup.VisibleIndex = 0;
            this.colMaNPLSearchLookup.Width = 123;
            // 
            // colMaVTSearchLookup
            // 
            this.colMaVTSearchLookup.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaVTSearchLookup.AppearanceCell.Options.UseFont = true;
            this.colMaVTSearchLookup.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colMaVTSearchLookup.AppearanceHeader.Options.UseFont = true;
            this.colMaVTSearchLookup.Caption = "Mã vật tư";
            this.colMaVTSearchLookup.FieldName = "MaVatTu";
            this.colMaVTSearchLookup.MinWidth = 23;
            this.colMaVTSearchLookup.Name = "colMaVTSearchLookup";
            this.colMaVTSearchLookup.Visible = true;
            this.colMaVTSearchLookup.VisibleIndex = 1;
            this.colMaVTSearchLookup.Width = 123;
            // 
            // colTenVTSearchLookup
            // 
            this.colTenVTSearchLookup.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colTenVTSearchLookup.AppearanceCell.Options.UseFont = true;
            this.colTenVTSearchLookup.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colTenVTSearchLookup.AppearanceHeader.Options.UseFont = true;
            this.colTenVTSearchLookup.Caption = "Tên vật tư";
            this.colTenVTSearchLookup.FieldName = "TenNPL";
            this.colTenVTSearchLookup.MinWidth = 23;
            this.colTenVTSearchLookup.Name = "colTenVTSearchLookup";
            this.colTenVTSearchLookup.Visible = true;
            this.colTenVTSearchLookup.VisibleIndex = 2;
            this.colTenVTSearchLookup.Width = 123;
            // 
            // colMaMauNPLSearchLookup
            // 
            this.colMaMauNPLSearchLookup.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaMauNPLSearchLookup.AppearanceCell.Options.UseFont = true;
            this.colMaMauNPLSearchLookup.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colMaMauNPLSearchLookup.AppearanceHeader.Options.UseFont = true;
            this.colMaMauNPLSearchLookup.Caption = "Mã màu";
            this.colMaMauNPLSearchLookup.FieldName = "MaMauNPL";
            this.colMaMauNPLSearchLookup.MinWidth = 23;
            this.colMaMauNPLSearchLookup.Name = "colMaMauNPLSearchLookup";
            this.colMaMauNPLSearchLookup.Width = 123;
            // 
            // colTenMauNPLSearchLookup
            // 
            this.colTenMauNPLSearchLookup.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colTenMauNPLSearchLookup.AppearanceCell.Options.UseFont = true;
            this.colTenMauNPLSearchLookup.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colTenMauNPLSearchLookup.AppearanceHeader.Options.UseFont = true;
            this.colTenMauNPLSearchLookup.Caption = "Màu";
            this.colTenMauNPLSearchLookup.FieldName = "TenMauNPL";
            this.colTenMauNPLSearchLookup.MinWidth = 23;
            this.colTenMauNPLSearchLookup.Name = "colTenMauNPLSearchLookup";
            this.colTenMauNPLSearchLookup.Visible = true;
            this.colTenMauNPLSearchLookup.VisibleIndex = 3;
            this.colTenMauNPLSearchLookup.Width = 123;
            // 
            // colMaVT
            // 
            this.colMaVT.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaVT.AppearanceCell.Options.UseFont = true;
            this.colMaVT.AppearanceCell.Options.UseTextOptions = true;
            this.colMaVT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaVT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colMaVT.AppearanceHeader.Options.UseFont = true;
            this.colMaVT.Caption = "Mã vật tư";
            this.colMaVT.FieldName = "MaVT";
            this.colMaVT.MinWidth = 23;
            this.colMaVT.Name = "colMaVT";
            this.colMaVT.Visible = true;
            this.colMaVT.VisibleIndex = 1;
            this.colMaVT.Width = 143;
            // 
            // colTenVT
            // 
            this.colTenVT.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colTenVT.AppearanceCell.Options.UseFont = true;
            this.colTenVT.AppearanceCell.Options.UseTextOptions = true;
            this.colTenVT.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colTenVT.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colTenVT.AppearanceHeader.Options.UseFont = true;
            this.colTenVT.Caption = "Tên vật tư";
            this.colTenVT.FieldName = "TenVT";
            this.colTenVT.MinWidth = 23;
            this.colTenVT.Name = "colTenVT";
            this.colTenVT.OptionsColumn.AllowEdit = false;
            this.colTenVT.Visible = true;
            this.colTenVT.VisibleIndex = 2;
            this.colTenVT.Width = 190;
            // 
            // colMaMau
            // 
            this.colMaMau.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaMau.AppearanceCell.Options.UseFont = true;
            this.colMaMau.AppearanceCell.Options.UseTextOptions = true;
            this.colMaMau.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colMaMau.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colMaMau.AppearanceHeader.Options.UseFont = true;
            this.colMaMau.Caption = "Màu";
            this.colMaMau.ColumnEdit = this.repositoryItemSearchLookUpEdit1;
            this.colMaMau.FieldName = "MaMau";
            this.colMaMau.MinWidth = 24;
            this.colMaMau.Name = "colMaMau";
            this.colMaMau.OptionsColumn.AllowEdit = false;
            this.colMaMau.Visible = true;
            this.colMaMau.VisibleIndex = 3;
            this.colMaMau.Width = 112;
            // 
            // repositoryItemSearchLookUpEdit1
            // 
            this.repositoryItemSearchLookUpEdit1.AutoHeight = false;
            this.repositoryItemSearchLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSearchLookUpEdit1.DisplayMember = "TenMauNPL";
            this.repositoryItemSearchLookUpEdit1.Name = "repositoryItemSearchLookUpEdit1";
            this.repositoryItemSearchLookUpEdit1.PopupView = this.repositoryItemSearchLookUpEdit1View;
            this.repositoryItemSearchLookUpEdit1.ValueMember = "MaMauNPL";
            // 
            // repositoryItemSearchLookUpEdit1View
            // 
            this.repositoryItemSearchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.repositoryItemSearchLookUpEdit1View.Name = "repositoryItemSearchLookUpEdit1View";
            this.repositoryItemSearchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.repositoryItemSearchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // colKhoVai
            // 
            this.colKhoVai.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colKhoVai.AppearanceCell.Options.UseFont = true;
            this.colKhoVai.AppearanceCell.Options.UseTextOptions = true;
            this.colKhoVai.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colKhoVai.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colKhoVai.AppearanceHeader.Options.UseFont = true;
            this.colKhoVai.Caption = "Khổ vải";
            this.colKhoVai.FieldName = "KhoVai";
            this.colKhoVai.MinWidth = 23;
            this.colKhoVai.Name = "colKhoVai";
            this.colKhoVai.Visible = true;
            this.colKhoVai.VisibleIndex = 4;
            this.colKhoVai.Width = 122;
            // 
            // colDonVi
            // 
            this.colDonVi.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colDonVi.AppearanceCell.Options.UseFont = true;
            this.colDonVi.AppearanceCell.Options.UseTextOptions = true;
            this.colDonVi.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colDonVi.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colDonVi.AppearanceHeader.Options.UseFont = true;
            this.colDonVi.Caption = "Đơn vị";
            this.colDonVi.ColumnEdit = this.searchLookUpEditMaDonVi.Properties;
            this.colDonVi.FieldName = "MaDV";
            this.colDonVi.MinWidth = 23;
            this.colDonVi.Name = "colDonVi";
            this.colDonVi.Visible = true;
            this.colDonVi.VisibleIndex = 5;
            this.colDonVi.Width = 128;
            // 
            // searchLookUpEditMaDonVi
            // 
            this.searchLookUpEditMaDonVi.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.searchLookUpEditMaDonVi.DisplayMember = "TenDV";
            this.searchLookUpEditMaDonVi.Name = "searchLookUpEditMaDonVi";
            this.searchLookUpEditMaDonVi.NullText = "Chọn mã đơn vị";
            this.searchLookUpEditMaDonVi.PopupView = this.gridViewLookUpMaDonVi;
            this.searchLookUpEditMaDonVi.ValueMember = "MaDV";
            // 
            // gridViewLookUpMaDonVi
            // 
            this.gridViewLookUpMaDonVi.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colMaDonVi,
            this.colTenDonVi});
            this.gridViewLookUpMaDonVi.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridViewLookUpMaDonVi.Name = "gridViewLookUpMaDonVi";
            this.gridViewLookUpMaDonVi.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewLookUpMaDonVi.OptionsView.ShowGroupPanel = false;
            // 
            // colMaDonVi
            // 
            this.colMaDonVi.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colMaDonVi.AppearanceCell.Options.UseFont = true;
            this.colMaDonVi.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colMaDonVi.AppearanceHeader.Options.UseFont = true;
            this.colMaDonVi.Caption = "Mã đơn vị";
            this.colMaDonVi.FieldName = "MaDV";
            this.colMaDonVi.MinWidth = 23;
            this.colMaDonVi.Name = "colMaDonVi";
            this.colMaDonVi.Width = 123;
            // 
            // colTenDonVi
            // 
            this.colTenDonVi.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colTenDonVi.AppearanceCell.Options.UseFont = true;
            this.colTenDonVi.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colTenDonVi.AppearanceHeader.Options.UseFont = true;
            this.colTenDonVi.Caption = "Đơn vị";
            this.colTenDonVi.FieldName = "TenDV";
            this.colTenDonVi.MinWidth = 23;
            this.colTenDonVi.Name = "colTenDonVi";
            this.colTenDonVi.Visible = true;
            this.colTenDonVi.VisibleIndex = 0;
            this.colTenDonVi.Width = 123;
            // 
            // colDinhMuc
            // 
            this.colDinhMuc.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colDinhMuc.AppearanceCell.Options.UseFont = true;
            this.colDinhMuc.AppearanceCell.Options.UseTextOptions = true;
            this.colDinhMuc.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colDinhMuc.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colDinhMuc.AppearanceHeader.Options.UseFont = true;
            this.colDinhMuc.Caption = "Định mức";
            this.colDinhMuc.FieldName = "DinhMuc";
            this.colDinhMuc.MinWidth = 23;
            this.colDinhMuc.Name = "colDinhMuc";
            this.colDinhMuc.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            this.colDinhMuc.Visible = true;
            this.colDinhMuc.VisibleIndex = 6;
            this.colDinhMuc.Width = 113;
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
            this.colSoLuong.MinWidth = 23;
            this.colSoLuong.Name = "colSoLuong";
            this.colSoLuong.Visible = true;
            this.colSoLuong.VisibleIndex = 7;
            this.colSoLuong.Width = 108;
            // 
            // colCapPhat
            // 
            this.colCapPhat.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colCapPhat.AppearanceCell.Options.UseFont = true;
            this.colCapPhat.AppearanceCell.Options.UseTextOptions = true;
            this.colCapPhat.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colCapPhat.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colCapPhat.AppearanceHeader.Options.UseFont = true;
            this.colCapPhat.Caption = "Cấp phát";
            this.colCapPhat.FieldName = "CapPhat";
            this.colCapPhat.MinWidth = 23;
            this.colCapPhat.Name = "colCapPhat";
            this.colCapPhat.Visible = true;
            this.colCapPhat.VisibleIndex = 8;
            this.colCapPhat.Width = 106;
            // 
            // colCapThem
            // 
            this.colCapThem.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colCapThem.AppearanceCell.Options.UseFont = true;
            this.colCapThem.AppearanceCell.Options.UseTextOptions = true;
            this.colCapThem.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colCapThem.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colCapThem.AppearanceHeader.Options.UseFont = true;
            this.colCapThem.Caption = "Cấp thêm";
            this.colCapThem.FieldName = "CapThem";
            this.colCapThem.MinWidth = 23;
            this.colCapThem.Name = "colCapThem";
            this.colCapThem.Visible = true;
            this.colCapThem.VisibleIndex = 9;
            this.colCapThem.Width = 106;
            // 
            // colThuHoi
            // 
            this.colThuHoi.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colThuHoi.AppearanceCell.Options.UseFont = true;
            this.colThuHoi.AppearanceCell.Options.UseTextOptions = true;
            this.colThuHoi.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colThuHoi.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colThuHoi.AppearanceHeader.Options.UseFont = true;
            this.colThuHoi.Caption = "Thu hồi";
            this.colThuHoi.FieldName = "ThuHoi";
            this.colThuHoi.MinWidth = 23;
            this.colThuHoi.Name = "colThuHoi";
            this.colThuHoi.Visible = true;
            this.colThuHoi.VisibleIndex = 10;
            this.colThuHoi.Width = 106;
            // 
            // colGhiChu
            // 
            this.colGhiChu.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.colGhiChu.AppearanceCell.Options.UseFont = true;
            this.colGhiChu.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.colGhiChu.AppearanceHeader.Options.UseFont = true;
            this.colGhiChu.Caption = "Ghi chú";
            this.colGhiChu.FieldName = "GhiChu";
            this.colGhiChu.MinWidth = 23;
            this.colGhiChu.Name = "colGhiChu";
            this.colGhiChu.Visible = true;
            this.colGhiChu.VisibleIndex = 11;
            this.colGhiChu.Width = 400;
            // 
            // MaMauNPL
            // 
            this.MaMauNPL.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.MaMauNPL.AppearanceCell.Options.UseFont = true;
            this.MaMauNPL.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.MaMauNPL.AppearanceHeader.Options.UseFont = true;
            this.MaMauNPL.Caption = "MaMauNPL";
            this.MaMauNPL.FieldName = "MaMauNPL";
            this.MaMauNPL.MinWidth = 23;
            this.MaMauNPL.Name = "MaMauNPL";
            this.MaMauNPL.Width = 400;
            // 
            // TenMauNPL
            // 
            this.TenMauNPL.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 9F);
            this.TenMauNPL.AppearanceCell.Options.UseFont = true;
            this.TenMauNPL.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            this.TenMauNPL.AppearanceHeader.Options.UseFont = true;
            this.TenMauNPL.Caption = "TenMauNPL";
            this.TenMauNPL.FieldName = "TenMauNPL";
            this.TenMauNPL.MinWidth = 23;
            this.TenMauNPL.Name = "TenMauNPL";
            this.TenMauNPL.Width = 60;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(665, 12);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // frmNhapDMNL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1558, 484);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.gridControlNhapDMNL);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControl1);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.LookAndFeel.SkinName = "London Liquid Sky";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmNhapDMNL";
            this.ShowIcon = false;
            this.Text = "Nhập định mức nguyên liệu";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlNhapDMNL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewNhapDMNL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditNPL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLookUpNPL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSearchLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditMaDonVi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewLookUpMaDonVi)).EndInit();
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
        private DevExpress.XtraGrid.GridControl gridControlNhapDMNL;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewNhapDMNL;
        private DevExpress.XtraGrid.Columns.GridColumn colID;
        // NPL
        private RepositoryItemSearchLookUpEdit searchLookUpEditNPL;
        private DevExpress.XtraGrid.Columns.GridColumn colMaNPLSearchLookup;
        private DevExpress.XtraGrid.Columns.GridColumn colMaVTSearchLookup;
        private DevExpress.XtraGrid.Columns.GridColumn colTenVTSearchLookup;
        private DevExpress.XtraGrid.Columns.GridColumn colMaMauNPLSearchLookup;
        private DevExpress.XtraGrid.Columns.GridColumn colTenMauNPLSearchLookup;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewLookUpNPL;
        private DevExpress.XtraGrid.Columns.GridColumn colMaNPL;
        private DevExpress.XtraGrid.Columns.GridColumn colMaVT;
        private DevExpress.XtraGrid.Columns.GridColumn colTenVT;
        private DevExpress.XtraGrid.Columns.GridColumn colMaMau;

        private RepositoryItemSearchLookUpEdit searchLookUpEditMaDonVi;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewLookUpMaDonVi;
        private DevExpress.XtraGrid.Columns.GridColumn colMaDonVi;
        private DevExpress.XtraGrid.Columns.GridColumn colTenDonVi;

        private DevExpress.XtraGrid.Columns.GridColumn colKhoVai;
        private DevExpress.XtraGrid.Columns.GridColumn colDonVi;
        private DevExpress.XtraGrid.Columns.GridColumn colDinhMuc;
        private DevExpress.XtraGrid.Columns.GridColumn colSoLuong;
        private DevExpress.XtraGrid.Columns.GridColumn colCapPhat;
        private DevExpress.XtraGrid.Columns.GridColumn colCapThem;
        private DevExpress.XtraGrid.Columns.GridColumn colThuHoi;
        private DevExpress.XtraGrid.Columns.GridColumn colGhiChu;
        private System.Windows.Forms.Button button1;
        private DevExpress.XtraBars.BarButtonItem Export;
        private RepositoryItemSearchLookUpEdit repositoryItemSearchLookUpEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn MaMauNPL;
        private DevExpress.XtraGrid.Columns.GridColumn TenMauNPL;
        private DevExpress.XtraGrid.Views.Grid.GridView repositoryItemSearchLookUpEdit1View;
    }
}