
namespace NtbSoft.ERP.Win.Modules.MuaHang
{
    partial class frmTonKho
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
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.indentID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.supplyID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rmpoID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.indentName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.supplyName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prpoName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prpoSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prpoColor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prpoProcess = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prpoUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prpoPredictTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prpoCompleteTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prpoStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prpoLocation = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prpoRFQ = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prpoGRN = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prpoRemarks = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.prpoQty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.prpoContractor = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.gridControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1504, 258);
            this.layoutControl1.TabIndex = 1;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // gridControl1
            // 
            this.gridControl1.Location = new System.Drawing.Point(12, 12);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.Size = new System.Drawing.Size(1480, 234);
            this.gridControl1.TabIndex = 4;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.indentID,
            this.supplyID,
            this.rmpoID,
            this.indentName,
            this.supplyName,
            this.prpoName,
            this.prpoSize,
            this.prpoColor,
            this.prpoProcess,
            this.prpoUnit,
            this.prpoPredictTime,
            this.prpoCompleteTime,
            this.prpoStatus,
            this.prpoLocation,
            this.prpoRFQ,
            this.prpoGRN,
            this.prpoRemarks,
            this.prpoQty,
            this.prpoContractor});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // indentID
            // 
            this.indentID.Caption = "indentID";
            this.indentID.FieldName = "indentID";
            this.indentID.MinWidth = 25;
            this.indentID.Name = "indentID";
            this.indentID.Width = 94;
            // 
            // supplyID
            // 
            this.supplyID.Caption = "supplyID";
            this.supplyID.FieldName = "supplyID";
            this.supplyID.MinWidth = 25;
            this.supplyID.Name = "supplyID";
            this.supplyID.Width = 94;
            // 
            // rmpoID
            // 
            this.rmpoID.Caption = "rmpoID";
            this.rmpoID.FieldName = "rmpoID";
            this.rmpoID.MinWidth = 25;
            this.rmpoID.Name = "rmpoID";
            this.rmpoID.Width = 94;
            // 
            // indentName
            // 
            this.indentName.Caption = "Tên phiếu XLVT";
            this.indentName.FieldName = "indentName";
            this.indentName.MinWidth = 25;
            this.indentName.Name = "indentName";
            this.indentName.Visible = true;
            this.indentName.VisibleIndex = 1;
            this.indentName.Width = 94;
            // 
            // supplyName
            // 
            this.supplyName.Caption = "Tên vật tư";
            this.supplyName.FieldName = "supplyName";
            this.supplyName.MinWidth = 25;
            this.supplyName.Name = "supplyName";
            this.supplyName.Visible = true;
            this.supplyName.VisibleIndex = 2;
            this.supplyName.Width = 94;
            // 
            // prpoName
            // 
            this.prpoName.Caption = "Tên đơn hàng";
            this.prpoName.FieldName = "prpoName";
            this.prpoName.MinWidth = 25;
            this.prpoName.Name = "prpoName";
            this.prpoName.Visible = true;
            this.prpoName.VisibleIndex = 3;
            this.prpoName.Width = 94;
            // 
            // prpoSize
            // 
            this.prpoSize.Caption = "Size";
            this.prpoSize.FieldName = "prpoSize";
            this.prpoSize.MinWidth = 25;
            this.prpoSize.Name = "prpoSize";
            this.prpoSize.Visible = true;
            this.prpoSize.VisibleIndex = 4;
            this.prpoSize.Width = 94;
            // 
            // prpoColor
            // 
            this.prpoColor.Caption = "Màu";
            this.prpoColor.FieldName = "prpoColor";
            this.prpoColor.MinWidth = 25;
            this.prpoColor.Name = "prpoColor";
            this.prpoColor.Visible = true;
            this.prpoColor.VisibleIndex = 5;
            this.prpoColor.Width = 94;
            // 
            // prpoProcess
            // 
            this.prpoProcess.Caption = "Tác vụ gia công";
            this.prpoProcess.FieldName = "prpoProcess";
            this.prpoProcess.MinWidth = 25;
            this.prpoProcess.Name = "prpoProcess";
            this.prpoProcess.Visible = true;
            this.prpoProcess.VisibleIndex = 9;
            this.prpoProcess.Width = 94;
            // 
            // prpoUnit
            // 
            this.prpoUnit.Caption = "Đơn vị";
            this.prpoUnit.FieldName = "prpoUnit";
            this.prpoUnit.MinWidth = 25;
            this.prpoUnit.Name = "prpoUnit";
            this.prpoUnit.Visible = true;
            this.prpoUnit.VisibleIndex = 6;
            this.prpoUnit.Width = 94;
            // 
            // prpoPredictTime
            // 
            this.prpoPredictTime.Caption = "Thời gian nhận hàng dự kiến";
            this.prpoPredictTime.FieldName = "prpoPredictTime";
            this.prpoPredictTime.MinWidth = 25;
            this.prpoPredictTime.Name = "prpoPredictTime";
            this.prpoPredictTime.Visible = true;
            this.prpoPredictTime.VisibleIndex = 10;
            this.prpoPredictTime.Width = 94;
            // 
            // prpoCompleteTime
            // 
            this.prpoCompleteTime.Caption = "Thời gian nhận hàng";
            this.prpoCompleteTime.FieldName = "prpoCompleteTime";
            this.prpoCompleteTime.MinWidth = 25;
            this.prpoCompleteTime.Name = "prpoCompleteTime";
            this.prpoCompleteTime.Visible = true;
            this.prpoCompleteTime.VisibleIndex = 11;
            this.prpoCompleteTime.Width = 94;
            // 
            // prpoStatus
            // 
            this.prpoStatus.Caption = "Trạng thái";
            this.prpoStatus.FieldName = "prpoStatus";
            this.prpoStatus.MinWidth = 25;
            this.prpoStatus.Name = "prpoStatus";
            this.prpoStatus.Visible = true;
            this.prpoStatus.VisibleIndex = 0;
            this.prpoStatus.Width = 94;
            // 
            // prpoLocation
            // 
            this.prpoLocation.Caption = "Vị trí lưu kho";
            this.prpoLocation.FieldName = "prpoLocation";
            this.prpoLocation.MinWidth = 25;
            this.prpoLocation.Name = "prpoLocation";
            this.prpoLocation.Visible = true;
            this.prpoLocation.VisibleIndex = 12;
            this.prpoLocation.Width = 94;
            // 
            // prpoRFQ
            // 
            this.prpoRFQ.Caption = "Phiếu báo giá";
            this.prpoRFQ.FieldName = "prpoRFQ";
            this.prpoRFQ.MinWidth = 25;
            this.prpoRFQ.Name = "prpoRFQ";
            this.prpoRFQ.Visible = true;
            this.prpoRFQ.VisibleIndex = 13;
            this.prpoRFQ.Width = 94;
            // 
            // prpoGRN
            // 
            this.prpoGRN.Caption = "Phiếu nhập kho";
            this.prpoGRN.FieldName = "prpoGRN";
            this.prpoGRN.MinWidth = 25;
            this.prpoGRN.Name = "prpoGRN";
            this.prpoGRN.Visible = true;
            this.prpoGRN.VisibleIndex = 14;
            this.prpoGRN.Width = 94;
            // 
            // prpoRemarks
            // 
            this.prpoRemarks.Caption = "Ghi chú";
            this.prpoRemarks.FieldName = "prpoRemarks";
            this.prpoRemarks.MinWidth = 25;
            this.prpoRemarks.Name = "prpoRemarks";
            this.prpoRemarks.Visible = true;
            this.prpoRemarks.VisibleIndex = 15;
            this.prpoRemarks.Width = 94;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1504, 258);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1484, 238);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // prpoQty
            // 
            this.prpoQty.Caption = "Số lượng";
            this.prpoQty.FieldName = "prpoQty";
            this.prpoQty.MinWidth = 25;
            this.prpoQty.Name = "prpoQty";
            this.prpoQty.Visible = true;
            this.prpoQty.VisibleIndex = 7;
            this.prpoQty.Width = 94;
            // 
            // prpoContractor
            // 
            this.prpoContractor.Caption = "Bên thực hiện";
            this.prpoContractor.FieldName = "prpoContractor";
            this.prpoContractor.MinWidth = 25;
            this.prpoContractor.Name = "prpoContractor";
            this.prpoContractor.Visible = true;
            this.prpoContractor.VisibleIndex = 8;
            this.prpoContractor.Width = 94;
            // 
            // frmTonKho
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1504, 258);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmTonKho";
            this.Text = "frmTonKho";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn indentID;
        private DevExpress.XtraGrid.Columns.GridColumn supplyID;
        private DevExpress.XtraGrid.Columns.GridColumn rmpoID;
        private DevExpress.XtraGrid.Columns.GridColumn indentName;
        private DevExpress.XtraGrid.Columns.GridColumn supplyName;
        private DevExpress.XtraGrid.Columns.GridColumn prpoName;
        private DevExpress.XtraGrid.Columns.GridColumn prpoSize;
        private DevExpress.XtraGrid.Columns.GridColumn prpoColor;
        private DevExpress.XtraGrid.Columns.GridColumn prpoProcess;
        private DevExpress.XtraGrid.Columns.GridColumn prpoUnit;
        private DevExpress.XtraGrid.Columns.GridColumn prpoPredictTime;
        private DevExpress.XtraGrid.Columns.GridColumn prpoCompleteTime;
        private DevExpress.XtraGrid.Columns.GridColumn prpoStatus;
        private DevExpress.XtraGrid.Columns.GridColumn prpoLocation;
        private DevExpress.XtraGrid.Columns.GridColumn prpoRFQ;
        private DevExpress.XtraGrid.Columns.GridColumn prpoGRN;
        private DevExpress.XtraGrid.Columns.GridColumn prpoRemarks;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraGrid.Columns.GridColumn prpoQty;
        private DevExpress.XtraGrid.Columns.GridColumn prpoContractor;
    }
}