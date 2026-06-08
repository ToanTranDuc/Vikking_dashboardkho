
namespace NtbSoft.ERP.Win.Modules.MuaHang
{
    partial class grpGiaCong
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.MaVTID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.LoaiVatTu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.MaVT2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Size2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Color = new DevExpress.XtraGrid.Columns.GridColumn();
            this.TenDVVT2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.DinhMucChung2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.supplySizeID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.supplyColorID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.DinhMucHaoHut2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.TVGiaCong = new DevExpress.XtraGrid.Columns.GridColumn();
            this.SoLuong = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ChiTiet2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1418, 558);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(12, 12);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1394, 534);
            this.gridControl1.TabIndex = 13;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.MaVTID,
            this.LoaiVatTu,
            this.MaVT2,
            this.Size2,
            this.Color,
            this.TenDVVT2,
            this.DinhMucChung2,
            this.supplySizeID,
            this.supplyColorID,
            this.DinhMucHaoHut2,
            this.TVGiaCong,
            this.SoLuong,
            this.ChiTiet2});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsFind.AllowFindPanel = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // MaVTID
            // 
            this.MaVTID.Caption = "MaVTID";
            this.MaVTID.FieldName = "MaVTID";
            this.MaVTID.MinWidth = 25;
            this.MaVTID.Name = "MaVTID";
            this.MaVTID.Width = 94;
            // 
            // LoaiVatTu
            // 
            this.LoaiVatTu.Caption = "Loại vật tư";
            this.LoaiVatTu.FieldName = "LoaiVatTu";
            this.LoaiVatTu.MinWidth = 25;
            this.LoaiVatTu.Name = "LoaiVatTu";
            this.LoaiVatTu.Visible = true;
            this.LoaiVatTu.VisibleIndex = 0;
            this.LoaiVatTu.Width = 94;
            // 
            // MaVT2
            // 
            this.MaVT2.Caption = "Tên vật tư";
            this.MaVT2.FieldName = "MaVT";
            this.MaVT2.MinWidth = 25;
            this.MaVT2.Name = "MaVT2";
            this.MaVT2.Visible = true;
            this.MaVT2.VisibleIndex = 1;
            this.MaVT2.Width = 94;
            // 
            // Size2
            // 
            this.Size2.Caption = "Size";
            this.Size2.FieldName = "TenSize";
            this.Size2.MinWidth = 25;
            this.Size2.Name = "Size2";
            this.Size2.Visible = true;
            this.Size2.VisibleIndex = 3;
            this.Size2.Width = 94;
            // 
            // Color
            // 
            this.Color.Caption = "Màu";
            this.Color.FieldName = "TenMau";
            this.Color.MinWidth = 25;
            this.Color.Name = "Color";
            this.Color.Visible = true;
            this.Color.VisibleIndex = 4;
            this.Color.Width = 94;
            // 
            // TenDVVT2
            // 
            this.TenDVVT2.Caption = "Đơn vị";
            this.TenDVVT2.FieldName = "TenDVVT";
            this.TenDVVT2.MinWidth = 25;
            this.TenDVVT2.Name = "TenDVVT2";
            this.TenDVVT2.Visible = true;
            this.TenDVVT2.VisibleIndex = 5;
            this.TenDVVT2.Width = 94;
            // 
            // DinhMucChung2
            // 
            this.DinhMucChung2.Caption = "Định mức";
            this.DinhMucChung2.FieldName = "DinhMucChung";
            this.DinhMucChung2.MinWidth = 25;
            this.DinhMucChung2.Name = "DinhMucChung2";
            this.DinhMucChung2.Visible = true;
            this.DinhMucChung2.VisibleIndex = 6;
            this.DinhMucChung2.Width = 94;
            // 
            // supplySizeID
            // 
            this.supplySizeID.Caption = "SizeID";
            this.supplySizeID.FieldName = "SizeID";
            this.supplySizeID.MinWidth = 25;
            this.supplySizeID.Name = "supplySizeID";
            this.supplySizeID.Width = 94;
            // 
            // supplyColorID
            // 
            this.supplyColorID.Caption = "MauVTID";
            this.supplyColorID.FieldName = "MauVTID";
            this.supplyColorID.MinWidth = 25;
            this.supplyColorID.Name = "supplyColorID";
            this.supplyColorID.Width = 94;
            // 
            // DinhMucHaoHut2
            // 
            this.DinhMucHaoHut2.Caption = "% Hao hụt";
            this.DinhMucHaoHut2.FieldName = "DinhMucHaoHut";
            this.DinhMucHaoHut2.MinWidth = 25;
            this.DinhMucHaoHut2.Name = "DinhMucHaoHut2";
            this.DinhMucHaoHut2.Visible = true;
            this.DinhMucHaoHut2.VisibleIndex = 7;
            this.DinhMucHaoHut2.Width = 94;
            // 
            // TVGiaCong
            // 
            this.TVGiaCong.Caption = "Tác vụ gia công";
            this.TVGiaCong.FieldName = "TVGiaCong";
            this.TVGiaCong.MinWidth = 25;
            this.TVGiaCong.Name = "TVGiaCong";
            this.TVGiaCong.Visible = true;
            this.TVGiaCong.VisibleIndex = 9;
            this.TVGiaCong.Width = 94;
            // 
            // SoLuong
            // 
            this.SoLuong.Caption = "Số lượng";
            this.SoLuong.FieldName = "SoLuong";
            this.SoLuong.MinWidth = 25;
            this.SoLuong.Name = "SoLuong";
            this.SoLuong.Visible = true;
            this.SoLuong.VisibleIndex = 8;
            this.SoLuong.Width = 94;
            // 
            // ChiTiet2
            // 
            this.ChiTiet2.Caption = "Chi tiết";
            this.ChiTiet2.FieldName = "ChiTiet";
            this.ChiTiet2.MinWidth = 25;
            this.ChiTiet2.Name = "ChiTiet2";
            this.ChiTiet2.Visible = true;
            this.ChiTiet2.VisibleIndex = 2;
            this.ChiTiet2.Width = 94;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem4});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1418, 558);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.gridControl1;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(1398, 538);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // grpGiaCong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "grpGiaCong";
            this.Size = new System.Drawing.Size(1418, 558);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn MaVTID;
        private DevExpress.XtraGrid.Columns.GridColumn LoaiVatTu;
        private DevExpress.XtraGrid.Columns.GridColumn MaVT2;
        private DevExpress.XtraGrid.Columns.GridColumn Size2;
        private DevExpress.XtraGrid.Columns.GridColumn Color;
        private DevExpress.XtraGrid.Columns.GridColumn TenDVVT2;
        private DevExpress.XtraGrid.Columns.GridColumn DinhMucChung2;
        private DevExpress.XtraGrid.Columns.GridColumn supplySizeID;
        private DevExpress.XtraGrid.Columns.GridColumn supplyColorID;
        private DevExpress.XtraGrid.Columns.GridColumn DinhMucHaoHut2;
        private DevExpress.XtraGrid.Columns.GridColumn TVGiaCong;
        private DevExpress.XtraGrid.Columns.GridColumn SoLuong;
        private DevExpress.XtraGrid.Columns.GridColumn ChiTiet2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
    }
}
