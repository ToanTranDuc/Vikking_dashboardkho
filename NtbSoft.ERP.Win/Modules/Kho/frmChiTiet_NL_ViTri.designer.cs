
namespace NtbSoft.ERP.Win.Modules.ViTriKho
{
    partial class frmChiTiet_NL_ViTri
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
            DevExpress.XtraGrid.GridFormatRule gridFormatRule1 = new DevExpress.XtraGrid.GridFormatRule();
            DevExpress.XtraEditors.FormatConditionRuleDataBar formatConditionRuleDataBar1 = new DevExpress.XtraEditors.FormatConditionRuleDataBar();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.dgrCTNL = new DevExpress.XtraGrid.GridControl();
            this.gridViewCTNL = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStatusXuat = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbConLai = new DevExpress.XtraEditors.LabelControl();
            this.lbDaChua = new DevExpress.XtraEditors.LabelControl();
            this.lbSucChua = new DevExpress.XtraEditors.LabelControl();
            this.lbTenVT = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.dgrCTNL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCTNL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn3.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn3.AppearanceHeader.Options.UseFont = true;
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.Caption = "Số lượng";
            this.gridColumn3.FieldName = "SoLuong";
            this.gridColumn3.MaxWidth = 100;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 100;
            // 
            // gridColumn4
            // 
            this.gridColumn4.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn4.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn4.AppearanceHeader.Options.UseFont = true;
            this.gridColumn4.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn4.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn4.Caption = "CBM";
            this.gridColumn4.FieldName = "CBM";
            this.gridColumn4.MaxWidth = 100;
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 5;
            this.gridColumn4.Width = 100;
            // 
            // dgrCTNL
            // 
            this.dgrCTNL.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgrCTNL.Location = new System.Drawing.Point(0, 0);
            this.dgrCTNL.LookAndFeel.SkinName = "Office 2010 Blue";
            this.dgrCTNL.LookAndFeel.UseDefaultLookAndFeel = false;
            this.dgrCTNL.MainView = this.gridViewCTNL;
            this.dgrCTNL.Name = "dgrCTNL";
            this.dgrCTNL.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1,
            this.repositoryItemTextEdit1});
            this.dgrCTNL.Size = new System.Drawing.Size(1009, 355);
            this.dgrCTNL.TabIndex = 0;
            this.dgrCTNL.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewCTNL});
            // 
            // gridViewCTNL
            // 
            this.gridViewCTNL.ColumnPanelRowHeight = 30;
            this.gridViewCTNL.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn7,
            this.gridColumn6,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.colStatusXuat});
            this.gridViewCTNL.CustomizationFormBounds = new System.Drawing.Rectangle(585, 257, 266, 340);
            gridFormatRule1.Name = "Format0";
            formatConditionRuleDataBar1.PredefinedName = null;
            gridFormatRule1.Rule = formatConditionRuleDataBar1;
            this.gridViewCTNL.FormatRules.Add(gridFormatRule1);
            this.gridViewCTNL.GridControl = this.dgrCTNL;
            this.gridViewCTNL.Name = "gridViewCTNL";
            this.gridViewCTNL.OptionsMenu.ShowConditionalFormattingItem = true;
            this.gridViewCTNL.OptionsView.ShowGroupPanel = false;
            this.gridViewCTNL.RowHeight = 25;
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn1.AppearanceHeader.Options.UseFont = true;
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn1.Caption = "Mã phiếu nhập";
            this.gridColumn1.FieldName = "MaPhieuNhap";
            this.gridColumn1.MaxWidth = 250;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 183;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn2.AppearanceHeader.Options.UseFont = true;
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn2.Caption = "Mã CTVT";
            this.gridColumn2.FieldName = "MaCTVT";
            this.gridColumn2.MaxWidth = 250;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 169;
            // 
            // gridColumn7
            // 
            this.gridColumn7.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn7.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn7.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn7.AppearanceHeader.Options.UseFont = true;
            this.gridColumn7.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn7.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn7.Caption = "Trọng lượng";
            this.gridColumn7.FieldName = "TrongLuong";
            this.gridColumn7.MaxWidth = 100;
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 3;
            // 
            // gridColumn6
            // 
            this.gridColumn6.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn6.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn6.AppearanceHeader.Options.UseFont = true;
            this.gridColumn6.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn6.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn6.Caption = "Khối lượng";
            this.gridColumn6.FieldName = "KhoiLuong";
            this.gridColumn6.MaxWidth = 100;
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 4;
            // 
            // gridColumn5
            // 
            this.gridColumn5.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn5.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn5.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn5.AppearanceHeader.Options.UseFont = true;
            this.gridColumn5.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn5.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn5.Caption = "Ngày nhập kho";
            this.gridColumn5.FieldName = "NgayNhapKho";
            this.gridColumn5.MaxWidth = 150;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 6;
            this.gridColumn5.Width = 150;
            // 
            // colStatusXuat
            // 
            this.colStatusXuat.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.colStatusXuat.AppearanceHeader.Options.UseFont = true;
            this.colStatusXuat.AppearanceHeader.Options.UseTextOptions = true;
            this.colStatusXuat.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colStatusXuat.Caption = "Đã xuất";
            this.colStatusXuat.ColumnEdit = this.repositoryItemCheckEdit1;
            this.colStatusXuat.FieldName = "Status_PX";
            this.colStatusXuat.MaxWidth = 80;
            this.colStatusXuat.Name = "colStatusXuat";
            this.colStatusXuat.OptionsColumn.AllowEdit = false;
            this.colStatusXuat.Visible = true;
            this.colStatusXuat.VisibleIndex = 7;
            this.colStatusXuat.Width = 80;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbConLai);
            this.panel1.Controls.Add(this.lbDaChua);
            this.panel1.Controls.Add(this.lbSucChua);
            this.panel1.Controls.Add(this.lbTenVT);
            this.panel1.Controls.Add(this.labelControl4);
            this.panel1.Controls.Add(this.labelControl3);
            this.panel1.Controls.Add(this.labelControl2);
            this.panel1.Controls.Add(this.labelControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 355);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1009, 110);
            this.panel1.TabIndex = 1;
            // 
            // lbConLai
            // 
            this.lbConLai.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lbConLai.Appearance.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lbConLai.Appearance.Options.UseFont = true;
            this.lbConLai.Appearance.Options.UseForeColor = true;
            this.lbConLai.Location = new System.Drawing.Point(87, 76);
            this.lbConLai.Name = "lbConLai";
            this.lbConLai.Size = new System.Drawing.Size(33, 16);
            this.lbConLai.TabIndex = 7;
            this.lbConLai.Text = ">>>";
            // 
            // lbDaChua
            // 
            this.lbDaChua.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lbDaChua.Appearance.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lbDaChua.Appearance.Options.UseFont = true;
            this.lbDaChua.Appearance.Options.UseForeColor = true;
            this.lbDaChua.Location = new System.Drawing.Point(87, 53);
            this.lbDaChua.Name = "lbDaChua";
            this.lbDaChua.Size = new System.Drawing.Size(33, 16);
            this.lbDaChua.TabIndex = 6;
            this.lbDaChua.Text = ">>>";
            // 
            // lbSucChua
            // 
            this.lbSucChua.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lbSucChua.Appearance.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lbSucChua.Appearance.Options.UseFont = true;
            this.lbSucChua.Appearance.Options.UseForeColor = true;
            this.lbSucChua.Location = new System.Drawing.Point(87, 30);
            this.lbSucChua.Name = "lbSucChua";
            this.lbSucChua.Size = new System.Drawing.Size(33, 16);
            this.lbSucChua.TabIndex = 5;
            this.lbSucChua.Text = ">>>";
            // 
            // lbTenVT
            // 
            this.lbTenVT.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.lbTenVT.Appearance.ForeColor = System.Drawing.Color.DodgerBlue;
            this.lbTenVT.Appearance.Options.UseFont = true;
            this.lbTenVT.Appearance.Options.UseForeColor = true;
            this.lbTenVT.Location = new System.Drawing.Point(87, 7);
            this.lbTenVT.Name = "lbTenVT";
            this.lbTenVT.Size = new System.Drawing.Size(33, 16);
            this.lbTenVT.TabIndex = 4;
            this.lbTenVT.Text = ">>>";
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.labelControl4.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.Appearance.Options.UseForeColor = true;
            this.labelControl4.Location = new System.Drawing.Point(12, 75);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(55, 17);
            this.labelControl4.TabIndex = 3;
            this.labelControl4.Text = "Còn lại: ";
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.labelControl3.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Appearance.Options.UseForeColor = true;
            this.labelControl3.Location = new System.Drawing.Point(12, 52);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(62, 17);
            this.labelControl3.TabIndex = 2;
            this.labelControl3.Text = "Đã chứa:";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.labelControl2.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Appearance.Options.UseForeColor = true;
            this.labelControl2.Location = new System.Drawing.Point(12, 29);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(73, 17);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "Sức chứa: ";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Appearance.Options.UseForeColor = true;
            this.labelControl1.Location = new System.Drawing.Point(12, 6);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(69, 17);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Tên vị trí: ";
            // 
            // frmChiTiet_NL_ViTri
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1009, 465);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dgrCTNL);
            this.Name = "frmChiTiet_NL_ViTri";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chi tiết năng lực vị trí";
            ((System.ComponentModel.ISupportInitialize)(this.dgrCTNL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewCTNL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl dgrCTNL;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewCTNL;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn colStatusXuat;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lbConLai;
        private DevExpress.XtraEditors.LabelControl lbDaChua;
        private DevExpress.XtraEditors.LabelControl lbSucChua;
        private DevExpress.XtraEditors.LabelControl lbTenVT;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
    }
}