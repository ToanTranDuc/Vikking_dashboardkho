namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    partial class frmChonNoiDen_KHDTV4
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.searchLookUpEdit_NoiDen = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.gridView_NoiDen = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem_NoiDen = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem_OK = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem_Cancel = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.gridColumn_NoiDen = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit_NoiDen.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_NoiDen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem_NoiDen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem_OK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem_Cancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.searchLookUpEdit_NoiDen);
            this.layoutControl1.Controls.Add(this.btnOK);
            this.layoutControl1.Controls.Add(this.btnCancel);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(647, 0, 812, 500);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(412, 98);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // searchLookUpEdit_NoiDen
            // 
            this.searchLookUpEdit_NoiDen.AllowDrop = true;
            this.searchLookUpEdit_NoiDen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchLookUpEdit_NoiDen.Location = new System.Drawing.Point(89, 12);
            this.searchLookUpEdit_NoiDen.Name = "searchLookUpEdit_NoiDen";
            this.searchLookUpEdit_NoiDen.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.searchLookUpEdit_NoiDen.Properties.DisplayMember = "NoiDen";
            this.searchLookUpEdit_NoiDen.Properties.NullText = "Chọn nơi đến";
            this.searchLookUpEdit_NoiDen.Properties.PopupView = this.gridView_NoiDen;
            this.searchLookUpEdit_NoiDen.Properties.ValueMember = "MaNoiDen";
            this.searchLookUpEdit_NoiDen.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.searchLookUpEdit_NoiDen.Size = new System.Drawing.Size(311, 22);
            this.searchLookUpEdit_NoiDen.StyleController = this.layoutControl1;
            this.searchLookUpEdit_NoiDen.TabIndex = 0;
            // 
            // gridView_NoiDen
            // 
            this.gridView_NoiDen.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_NoiDen});
            this.gridView_NoiDen.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView_NoiDen.Name = "gridView_NoiDen";
            this.gridView_NoiDen.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView_NoiDen.OptionsView.ShowGroupPanel = false;
            // 
            // btnOK
            // 
            this.btnOK.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(168, 59);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(113, 27);
            this.btnOK.StyleController = this.layoutControl1;
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "Xác nhận";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(285, 59);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(105, 27);
            this.btnCancel.StyleController = this.layoutControl1;
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem_NoiDen,
            this.layoutControlItem_OK,
            this.layoutControlItem_Cancel,
            this.emptySpaceItem2,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(412, 98);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem_NoiDen
            // 
            this.layoutControlItem_NoiDen.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem_NoiDen.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem_NoiDen.Control = this.searchLookUpEdit_NoiDen;
            this.layoutControlItem_NoiDen.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem_NoiDen.MinSize = new System.Drawing.Size(131, 26);
            this.layoutControlItem_NoiDen.Name = "layoutControlItem_NoiDen";
            this.layoutControlItem_NoiDen.Size = new System.Drawing.Size(392, 26);
            this.layoutControlItem_NoiDen.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem_NoiDen.Text = "Nơi đến:";
            this.layoutControlItem_NoiDen.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem_NoiDen.TextSize = new System.Drawing.Size(72, 18);
            this.layoutControlItem_NoiDen.TextToControlDistance = 5;
            // 
            // layoutControlItem_OK
            // 
            this.layoutControlItem_OK.Control = this.btnOK;
            this.layoutControlItem_OK.Location = new System.Drawing.Point(156, 47);
            this.layoutControlItem_OK.MaxSize = new System.Drawing.Size(117, 31);
            this.layoutControlItem_OK.MinSize = new System.Drawing.Size(117, 31);
            this.layoutControlItem_OK.Name = "layoutControlItem_OK";
            this.layoutControlItem_OK.Size = new System.Drawing.Size(117, 31);
            this.layoutControlItem_OK.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem_OK.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem_OK.TextVisible = false;
            // 
            // layoutControlItem_Cancel
            // 
            this.layoutControlItem_Cancel.Control = this.btnCancel;
            this.layoutControlItem_Cancel.Location = new System.Drawing.Point(273, 47);
            this.layoutControlItem_Cancel.MaxSize = new System.Drawing.Size(109, 31);
            this.layoutControlItem_Cancel.MinSize = new System.Drawing.Size(109, 31);
            this.layoutControlItem_Cancel.Name = "layoutControlItem_Cancel";
            this.layoutControlItem_Cancel.Size = new System.Drawing.Size(119, 31);
            this.layoutControlItem_Cancel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem_Cancel.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem_Cancel.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 47);
            this.emptySpaceItem2.MaxSize = new System.Drawing.Size(156, 31);
            this.emptySpaceItem2.MinSize = new System.Drawing.Size(156, 31);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(156, 31);
            this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 26);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(392, 21);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // gridColumn_NoiDen
            // 
            this.gridColumn_NoiDen.Caption = "Nơi đến";
            this.gridColumn_NoiDen.FieldName = "NoiDen";
            this.gridColumn_NoiDen.Name = "gridColumn_NoiDen";
            this.gridColumn_NoiDen.Visible = true;
            this.gridColumn_NoiDen.VisibleIndex = 0;
            // 
            // frmChonNoiDen_KHDTV4
            // 
            this.AcceptButton = this.btnOK;
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(412, 98);
            this.Controls.Add(this.layoutControl1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmChonNoiDen_KHDTV4";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chọn qui cách đóng thùng";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit_NoiDen.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView_NoiDen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem_NoiDen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem_OK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem_Cancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.SearchLookUpEdit searchLookUpEdit_NoiDen;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView_NoiDen;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_NoiDen;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem_NoiDen;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem_OK;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem_Cancel;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}