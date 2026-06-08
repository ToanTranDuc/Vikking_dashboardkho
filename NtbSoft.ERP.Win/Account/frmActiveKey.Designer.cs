namespace NtbSoft.ERP.Win.Account
{
    partial class frmActiveKey
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmActiveKey));
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btPasteKeyActive = new DevExpress.XtraEditors.SimpleButton();
            this.btCopyKeySource = new DevExpress.XtraEditors.SimpleButton();
            this.btActive = new DevExpress.XtraEditors.SimpleButton();
            this.btGetKey = new DevExpress.XtraEditors.SimpleButton();
            this.tbKeyActive = new DevExpress.XtraEditors.TextEdit();
            this.tbKeySource = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbKeyActive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbKeySource.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.ForeColor = System.Drawing.Color.Red;
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Appearance.Options.UseForeColor = true;
            this.labelControl1.Location = new System.Drawing.Point(14, 15);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(87, 18);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Key source:";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.labelControl2.Appearance.ForeColor = System.Drawing.Color.Green;
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Appearance.Options.UseForeColor = true;
            this.labelControl2.Location = new System.Drawing.Point(14, 49);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(83, 18);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "Key active:";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btPasteKeyActive);
            this.panelControl1.Controls.Add(this.btCopyKeySource);
            this.panelControl1.Controls.Add(this.btActive);
            this.panelControl1.Controls.Add(this.btGetKey);
            this.panelControl1.Controls.Add(this.tbKeyActive);
            this.panelControl1.Controls.Add(this.tbKeySource);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(516, 132);
            this.panelControl1.TabIndex = 2;
            // 
            // btPasteKeyActive
            // 
            this.btPasteKeyActive.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btPasteKeyActive.Appearance.Options.UseFont = true;
            this.btPasteKeyActive.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btPasteKeyActive.ImageOptions.Image")));
            this.btPasteKeyActive.Location = new System.Drawing.Point(468, 43);
            this.btPasteKeyActive.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btPasteKeyActive.Name = "btPasteKeyActive";
            this.btPasteKeyActive.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btPasteKeyActive.Size = new System.Drawing.Size(27, 28);
            this.btPasteKeyActive.TabIndex = 8;
            this.btPasteKeyActive.Click += new System.EventHandler(this.btPasteKeyActive_Click);
            // 
            // btCopyKeySource
            // 
            this.btCopyKeySource.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btCopyKeySource.Appearance.Options.UseFont = true;
            this.btCopyKeySource.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btCopyKeySource.ImageOptions.Image")));
            this.btCopyKeySource.Location = new System.Drawing.Point(468, 10);
            this.btCopyKeySource.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btCopyKeySource.Name = "btCopyKeySource";
            this.btCopyKeySource.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btCopyKeySource.Size = new System.Drawing.Size(27, 28);
            this.btCopyKeySource.TabIndex = 7;
            this.btCopyKeySource.Click += new System.EventHandler(this.btCopyKeySource_Click);
            // 
            // btActive
            // 
            this.btActive.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btActive.Appearance.ForeColor = System.Drawing.Color.DarkGreen;
            this.btActive.Appearance.Options.UseFont = true;
            this.btActive.Appearance.Options.UseForeColor = true;
            this.btActive.Enabled = false;
            this.btActive.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btActive.ImageOptions.Image")));
            this.btActive.Location = new System.Drawing.Point(231, 87);
            this.btActive.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btActive.Name = "btActive";
            this.btActive.Size = new System.Drawing.Size(105, 28);
            this.btActive.TabIndex = 6;
            this.btActive.Text = "Active";
            this.btActive.Click += new System.EventHandler(this.btActive_Click);
            // 
            // btGetKey
            // 
            this.btGetKey.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.btGetKey.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.btGetKey.Appearance.Options.UseFont = true;
            this.btGetKey.Appearance.Options.UseForeColor = true;
            this.btGetKey.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btGetKey.ImageOptions.Image")));
            this.btGetKey.Location = new System.Drawing.Point(103, 87);
            this.btGetKey.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btGetKey.Name = "btGetKey";
            this.btGetKey.Size = new System.Drawing.Size(105, 28);
            this.btGetKey.TabIndex = 4;
            this.btGetKey.Text = "Get Key";
            this.btGetKey.Click += new System.EventHandler(this.btGetKey_Click);
            // 
            // tbKeyActive
            // 
            this.tbKeyActive.Enabled = false;
            this.tbKeyActive.Location = new System.Drawing.Point(103, 46);
            this.tbKeyActive.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbKeyActive.Name = "tbKeyActive";
            this.tbKeyActive.Size = new System.Drawing.Size(358, 23);
            this.tbKeyActive.TabIndex = 3;
            this.tbKeyActive.EditValueChanged += new System.EventHandler(this.tbKeyActive_EditValueChanged);
            // 
            // tbKeySource
            // 
            this.tbKeySource.Location = new System.Drawing.Point(103, 11);
            this.tbKeySource.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbKeySource.Name = "tbKeySource";
            this.tbKeySource.Size = new System.Drawing.Size(358, 23);
            this.tbKeySource.TabIndex = 2;
            // 
            // frmActiveKey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 132);
            this.Controls.Add(this.panelControl1);
            this.LookAndFeel.SkinName = "The Asphalt World";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "frmActiveKey";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Active";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbKeyActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbKeySource.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.TextEdit tbKeyActive;
        private DevExpress.XtraEditors.TextEdit tbKeySource;
        private DevExpress.XtraEditors.SimpleButton btActive;
        private DevExpress.XtraEditors.SimpleButton btGetKey;
        private DevExpress.XtraEditors.SimpleButton btCopyKeySource;
        private DevExpress.XtraEditors.SimpleButton btPasteKeyActive;
    }
}