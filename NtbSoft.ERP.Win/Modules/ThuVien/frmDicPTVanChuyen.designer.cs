
namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    partial class frmDicPTVanChuyen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDicPTVanChuyen));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.Them = new DevExpress.XtraBars.BarButtonItem();
            this.Xoa = new DevExpress.XtraBars.BarButtonItem();
            this.Luu = new DevExpress.XtraBars.BarButtonItem();
            this.Naplai = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
            this.Sua = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
            this.grcPhuongThucVanChuyen = new DevExpress.XtraGrid.GridControl();
            this.grvPhuongThucVanChuyen = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMaVanChuyen = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colVanChuyen = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGhiChu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIsUse = new DevExpress.XtraGrid.Columns.GridColumn();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcPhuongThucVanChuyen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvPhuongThucVanChuyen)).BeginInit();
            this.SuspendLayout();
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
            this.Naplai,
            this.barButtonItem1,
            this.barButtonItem2,
            this.barButtonItem3});
            this.barManager1.MaxItemId = 8;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.Them),
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
            this.Them.Caption = "Thêm(Ctrl+ Shift+ N)";
            this.Them.Id = 0;
            this.Them.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Them.ImageOptions.SvgImage")));
            this.Them.ItemShortcut = new DevExpress.XtraBars.BarShortcut(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                | System.Windows.Forms.Keys.N));
            this.Them.Name = "Them";
            this.Them.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Them.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Them_ItemClick);
            // 
            // Xoa
            // 
            this.Xoa.Caption = "Xóa (Delete)";
            this.Xoa.Id = 2;
            this.Xoa.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Xoa.ImageOptions.SvgImage")));
            this.Xoa.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.Delete);
            this.Xoa.Name = "Xoa";
            this.Xoa.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Xoa.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Xoa_ItemClick);
            // 
            // Luu
            // 
            this.Luu.Caption = "Lưu (Ctrl + S)";
            this.Luu.Id = 3;
            this.Luu.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Luu.ImageOptions.SvgImage")));
            this.Luu.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.Luu.Name = "Luu";
            this.Luu.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.Luu.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Luu_ItemClick);
            // 
            // Naplai
            // 
            this.Naplai.Caption = "Nạp lại(F5)";
            this.Naplai.Id = 4;
            this.Naplai.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("Naplai.ImageOptions.Image")));
            this.Naplai.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("Naplai.ImageOptions.LargeImage")));
            this.Naplai.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F5);
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
            this.barDockControlTop.Size = new System.Drawing.Size(887, 36);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 398);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlBottom.Size = new System.Drawing.Size(887, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 36);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 362);
            // 
            // barDockControl1
            // 
            this.barDockControl1.CausesValidation = false;
            this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControl1.Location = new System.Drawing.Point(887, 36);
            this.barDockControl1.Manager = this.barManager1;
            this.barDockControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.barDockControl1.Size = new System.Drawing.Size(0, 362);
            // 
            // Sua
            // 
            this.Sua.Caption = "Sửa ";
            this.Sua.Id = 1;
            this.Sua.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("Sua.ImageOptions.SvgImage")));
            this.Sua.Name = "Sua";
            this.Sua.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Import Excel";
            this.barButtonItem1.Id = 5;
            this.barButtonItem1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.Image")));
            this.barButtonItem1.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.LargeImage")));
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.Caption = "Up";
            this.barButtonItem2.Id = 6;
            this.barButtonItem2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.ImageOptions.Image")));
            this.barButtonItem2.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem2.ImageOptions.LargeImage")));
            this.barButtonItem2.Name = "barButtonItem2";
            this.barButtonItem2.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barButtonItem3
            // 
            this.barButtonItem3.Caption = "Down";
            this.barButtonItem3.Id = 7;
            this.barButtonItem3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem3.ImageOptions.Image")));
            this.barButtonItem3.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem3.ImageOptions.LargeImage")));
            this.barButtonItem3.Name = "barButtonItem3";
            this.barButtonItem3.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barAndDockingController1
            // 
            this.barAndDockingController1.LookAndFeel.SkinMaskColor = System.Drawing.Color.Lime;
            this.barAndDockingController1.LookAndFeel.SkinName = "Office 2010 Blue";
            this.barAndDockingController1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
            // 
            // grcPhuongThucVanChuyen
            // 
            this.grcPhuongThucVanChuyen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grcPhuongThucVanChuyen.Location = new System.Drawing.Point(0, 36);
            this.grcPhuongThucVanChuyen.MainView = this.grvPhuongThucVanChuyen;
            this.grcPhuongThucVanChuyen.Name = "grcPhuongThucVanChuyen";
            this.grcPhuongThucVanChuyen.Size = new System.Drawing.Size(887, 362);
            this.grcPhuongThucVanChuyen.TabIndex = 6;
            this.grcPhuongThucVanChuyen.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvPhuongThucVanChuyen});
            // 
            // grvPhuongThucVanChuyen
            // 
            this.grvPhuongThucVanChuyen.Appearance.TopNewRow.BackColor = System.Drawing.Color.White;
            this.grvPhuongThucVanChuyen.Appearance.TopNewRow.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grvPhuongThucVanChuyen.Appearance.TopNewRow.Options.UseBackColor = true;
            this.grvPhuongThucVanChuyen.Appearance.TopNewRow.Options.UseFont = true;
            this.grvPhuongThucVanChuyen.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colID,
            this.colMaVanChuyen,
            this.colVanChuyen,
            this.colGhiChu,
            this.colIsUse});
            this.grvPhuongThucVanChuyen.GridControl = this.grcPhuongThucVanChuyen;
            this.grvPhuongThucVanChuyen.Name = "grvPhuongThucVanChuyen";
            this.grvPhuongThucVanChuyen.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
            this.grvPhuongThucVanChuyen.OptionsSelection.MultiSelect = true;
            this.grvPhuongThucVanChuyen.OptionsView.ColumnAutoWidth = false;
            this.grvPhuongThucVanChuyen.OptionsView.ShowAutoFilterRow = true;
            this.grvPhuongThucVanChuyen.OptionsView.ShowGroupPanel = false;
            this.grvPhuongThucVanChuyen.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.grvPhuongThucVanChuyen_CustomDrawColumnHeader);
            this.grvPhuongThucVanChuyen.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.grvPhuongThucVanChuyen_InitNewRow);
            this.grvPhuongThucVanChuyen.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.grvPhuongThucVanChuyen_FocusedRowChanged);
            this.grvPhuongThucVanChuyen.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.grvPhuongThucVanChuyen_ValidatingEditor);
            this.grvPhuongThucVanChuyen.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.grvPhuongThucVanChuyen_InvalidValueException);
            // 
            // colID
            // 
            this.colID.Caption = "ID";
            this.colID.FieldName = "ID";
            this.colID.MinWidth = 25;
            this.colID.Name = "colID";
            this.colID.Width = 94;
            // 
            // colMaVanChuyen
            // 
            this.colMaVanChuyen.Caption = "MaCDTS";
            this.colMaVanChuyen.FieldName = "MaPTVanChuyen";
            this.colMaVanChuyen.MinWidth = 25;
            this.colMaVanChuyen.Name = "colMaVanChuyen";
            this.colMaVanChuyen.Width = 94;
            // 
            // colVanChuyen
            // 
            this.colVanChuyen.Caption = "Phương Thức Vận Chuyển";
            this.colVanChuyen.FieldName = "VanChuyen";
            this.colVanChuyen.MinWidth = 25;
            this.colVanChuyen.Name = "colVanChuyen";
            this.colVanChuyen.Visible = true;
            this.colVanChuyen.VisibleIndex = 0;
            this.colVanChuyen.Width = 279;
            // 
            // colGhiChu
            // 
            this.colGhiChu.Caption = "Ghi Chú";
            this.colGhiChu.FieldName = "GhiChu";
            this.colGhiChu.MinWidth = 25;
            this.colGhiChu.Name = "colGhiChu";
            this.colGhiChu.Visible = true;
            this.colGhiChu.VisibleIndex = 1;
            this.colGhiChu.Width = 167;
            // 
            // colIsUse
            // 
            this.colIsUse.Caption = "IsUse";
            this.colIsUse.FieldName = "IsUse";
            this.colIsUse.MinWidth = 25;
            this.colIsUse.Name = "colIsUse";
            this.colIsUse.Width = 94;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(584, 1);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(94, 29);
            this.simpleButton1.TabIndex = 11;
            this.simpleButton1.Text = "simpleButton1";
            this.simpleButton1.Visible = false;
            // 
            // frmDicPTVanChuyen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 398);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.grcPhuongThucVanChuyen);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControl1);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmDicPTVanChuyen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Phương Thức Vận Chuyển";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grcPhuongThucVanChuyen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvPhuongThucVanChuyen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem Them;
        private DevExpress.XtraBars.BarButtonItem Xoa;
        private DevExpress.XtraBars.BarButtonItem Luu;
        private DevExpress.XtraBars.BarButtonItem Naplai;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControl1;
        private DevExpress.XtraBars.BarButtonItem Sua;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
        private DevExpress.XtraGrid.GridControl grcPhuongThucVanChuyen;
        private DevExpress.XtraGrid.Views.Grid.GridView grvPhuongThucVanChuyen;
        private DevExpress.XtraGrid.Columns.GridColumn colID;
        private DevExpress.XtraGrid.Columns.GridColumn colMaVanChuyen;
        private DevExpress.XtraGrid.Columns.GridColumn colVanChuyen;
        private DevExpress.XtraGrid.Columns.GridColumn colGhiChu;
        private DevExpress.XtraGrid.Columns.GridColumn colIsUse;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
    }
}