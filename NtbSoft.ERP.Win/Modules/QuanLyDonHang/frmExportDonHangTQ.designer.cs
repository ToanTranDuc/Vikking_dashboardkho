
namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    partial class frmExportDonHangTQ
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.searchLookUpEdit_PO = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.searchLookUpEditView_PO = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.rColTenHang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rColMaHang = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rColPOID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rColPO = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtTenHang = new DevExpress.XtraEditors.TextEdit();
            this.txtKhachHang = new DevExpress.XtraEditors.TextEdit();
            this.textEdit3 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit6 = new DevExpress.XtraEditors.TextEdit();
            this.btnXacNhan = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit_PO.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditView_PO)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTenHang.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKhachHang.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit6.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AutoScroll = false;
            this.layoutControl1.Controls.Add(this.searchLookUpEdit_PO);
            this.layoutControl1.Controls.Add(this.txtTenHang);
            this.layoutControl1.Controls.Add(this.txtKhachHang);
            this.layoutControl1.Controls.Add(this.textEdit3);
            this.layoutControl1.Controls.Add(this.textEdit6);
            this.layoutControl1.Controls.Add(this.btnXacNhan);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem3,
            this.layoutControlItem6});
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3135, 0, 812, 500);
            this.layoutControl1.OptionsView.AlwaysScrollActiveControlIntoView = false;
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(613, 107);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // searchLookUpEdit_PO
            // 
            this.searchLookUpEdit_PO.Location = new System.Drawing.Point(104, 38);
            this.searchLookUpEdit_PO.Name = "searchLookUpEdit_PO";
            this.searchLookUpEdit_PO.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.searchLookUpEdit_PO.Properties.NullText = "Chọn PO";
            this.searchLookUpEdit_PO.Properties.PopupView = this.searchLookUpEditView_PO;
            this.searchLookUpEdit_PO.Size = new System.Drawing.Size(497, 22);
            this.searchLookUpEdit_PO.StyleController = this.layoutControl1;
            this.searchLookUpEdit_PO.TabIndex = 12;
            this.searchLookUpEdit_PO.Popup += new System.EventHandler(this.searchLookUpEdit_PO_Popup);
            this.searchLookUpEdit_PO.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.searchLookUpEditViewPO_CustomDisplayText);
            // 
            // searchLookUpEditView_PO
            // 
            this.searchLookUpEditView_PO.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.rColTenHang,
            this.rColMaHang,
            this.rColPOID,
            this.rColPO});
            this.searchLookUpEditView_PO.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchLookUpEditView_PO.GroupCount = 1;
            this.searchLookUpEditView_PO.Name = "searchLookUpEditView_PO";
            this.searchLookUpEditView_PO.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpEditView_PO.OptionsSelection.MultiSelect = true;
            this.searchLookUpEditView_PO.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.searchLookUpEditView_PO.OptionsView.ShowGroupPanel = false;
            this.searchLookUpEditView_PO.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.rColTenHang, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.searchLookUpEditView_PO.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.gridView_CustomDrawColumnHeader);
            this.searchLookUpEditView_PO.CustomDrawGroupRow += new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(this.gVtong_CustomDrawGroupRow);
            this.searchLookUpEditView_PO.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.searchLookUpEditViewPO_SelectionChanged);
            this.searchLookUpEditView_PO.ColumnFilterChanged += new System.EventHandler(this.searchLookUpEditViewPO_ColumnFilterChanged);
            // 
            // rColTenHang
            // 
            this.rColTenHang.Caption = "Mã Hàng";
            this.rColTenHang.FieldName = "TenHang";
            this.rColTenHang.Name = "rColTenHang";
            this.rColTenHang.Visible = true;
            this.rColTenHang.VisibleIndex = 0;
            // 
            // rColMaHang
            // 
            this.rColMaHang.Caption = "MaHang";
            this.rColMaHang.FieldName = "MaHang";
            this.rColMaHang.Name = "rColMaHang";
            // 
            // rColPOID
            // 
            this.rColPOID.Caption = "POID";
            this.rColPOID.FieldName = "POID";
            this.rColPOID.Name = "rColPOID";
            // 
            // rColPO
            // 
            this.rColPO.Caption = "PO";
            this.rColPO.FieldName = "PO";
            this.rColPO.Name = "rColPO";
            this.rColPO.Visible = true;
            this.rColPO.VisibleIndex = 1;
            // 
            // txtTenHang
            // 
            this.txtTenHang.Location = new System.Drawing.Point(348, 12);
            this.txtTenHang.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtTenHang.Name = "txtTenHang";
            this.txtTenHang.Properties.ReadOnly = true;
            this.txtTenHang.Size = new System.Drawing.Size(253, 22);
            this.txtTenHang.StyleController = this.layoutControl1;
            this.txtTenHang.TabIndex = 4;
            // 
            // txtKhachHang
            // 
            this.txtKhachHang.Location = new System.Drawing.Point(104, 12);
            this.txtKhachHang.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtKhachHang.Name = "txtKhachHang";
            this.txtKhachHang.Properties.ReadOnly = true;
            this.txtKhachHang.Size = new System.Drawing.Size(148, 22);
            this.txtKhachHang.StyleController = this.layoutControl1;
            this.txtKhachHang.TabIndex = 5;
            // 
            // textEdit3
            // 
            this.textEdit3.Location = new System.Drawing.Point(106, 40);
            this.textEdit3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textEdit3.Name = "textEdit3";
            this.textEdit3.Size = new System.Drawing.Size(110, 22);
            this.textEdit3.StyleController = this.layoutControl1;
            this.textEdit3.TabIndex = 6;
            // 
            // textEdit6
            // 
            this.textEdit6.Location = new System.Drawing.Point(330, 40);
            this.textEdit6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textEdit6.Name = "textEdit6";
            this.textEdit6.Size = new System.Drawing.Size(116, 22);
            this.textEdit6.StyleController = this.layoutControl1;
            this.textEdit6.TabIndex = 9;
            // 
            // btnXacNhan
            // 
            this.btnXacNhan.Appearance.BackColor = System.Drawing.Color.Teal;
            this.btnXacNhan.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.btnXacNhan.Appearance.Options.UseBackColor = true;
            this.btnXacNhan.Appearance.Options.UseFont = true;
            this.btnXacNhan.Location = new System.Drawing.Point(407, 64);
            this.btnXacNhan.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnXacNhan.Name = "btnXacNhan";
            this.btnXacNhan.Size = new System.Drawing.Size(194, 27);
            this.btnXacNhan.StyleController = this.layoutControl1;
            this.btnXacNhan.TabIndex = 11;
            this.btnXacNhan.Text = "Xuất Excel";
            this.btnXacNhan.Click += new System.EventHandler(this.btnXacNhan_Click);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.layoutControlItem3.AppearanceItemCaption.ForeColor = System.Drawing.Color.Blue;
            this.layoutControlItem3.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem3.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem3.Control = this.textEdit3;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(206, 26);
            this.layoutControlItem3.Text = "Chủng loại:";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(82, 16);
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.layoutControlItem6.AppearanceItemCaption.ForeColor = System.Drawing.Color.Blue;
            this.layoutControlItem6.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem6.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem6.Control = this.textEdit6;
            this.layoutControlItem6.Location = new System.Drawing.Point(224, 26);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Size = new System.Drawing.Size(212, 26);
            this.layoutControlItem6.Text = "Số lượng:";
            this.layoutControlItem6.TextSize = new System.Drawing.Size(82, 16);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.emptySpaceItem2,
            this.layoutControlItem2});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(613, 107);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.layoutControlItem1.AppearanceItemCaption.ForeColor = System.Drawing.Color.Blue;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem1.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem1.Control = this.txtTenHang;
            this.layoutControlItem1.Location = new System.Drawing.Point(244, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(349, 26);
            this.layoutControlItem1.Text = "Mã hàng:";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(89, 17);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnXacNhan;
            this.layoutControlItem4.Location = new System.Drawing.Point(395, 52);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(198, 35);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem5.AppearanceItemCaption.ForeColor = System.Drawing.Color.Crimson;
            this.layoutControlItem5.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem5.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem5.Control = this.searchLookUpEdit_PO;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 26);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(593, 26);
            this.layoutControlItem5.Text = "Chọn PO : ";
            this.layoutControlItem5.TextSize = new System.Drawing.Size(89, 17);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 52);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(395, 35);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.layoutControlItem2.AppearanceItemCaption.ForeColor = System.Drawing.Color.Blue;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem2.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutControlItem2.Control = this.txtKhachHang;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(244, 26);
            this.layoutControlItem2.Text = "Khách hàng:";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(89, 17);
            // 
            // frmExportDonHangTQ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 107);
            this.Controls.Add(this.layoutControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.LookAndFeel.SkinName = "Office 2010 Blue";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmExportDonHangTQ";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Xuất Excel";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit_PO.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEditView_PO)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTenHang.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtKhachHang.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit6.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit txtTenHang;
        private DevExpress.XtraEditors.TextEdit txtKhachHang;
        private DevExpress.XtraEditors.TextEdit textEdit3;
        private DevExpress.XtraEditors.TextEdit textEdit6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraEditors.SimpleButton btnXacNhan;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.SearchLookUpEdit searchLookUpEdit_PO;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEditView_PO;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraGrid.Columns.GridColumn rColTenHang;
        private DevExpress.XtraGrid.Columns.GridColumn rColMaHang;
        private DevExpress.XtraGrid.Columns.GridColumn rColPOID;
        private DevExpress.XtraGrid.Columns.GridColumn rColPO;
    }
}