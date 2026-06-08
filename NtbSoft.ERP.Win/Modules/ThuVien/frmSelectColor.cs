using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmSelectColor : DevExpress.XtraEditors.XtraForm
    {
        public string Hex { get; set; }
        public string CMYK { get; set; }

        string _defaultHex = string.Empty;
        private System.Threading.CancellationTokenSource _updateToken;
        public frmSelectColor(string defaultHex)
        {
            InitializeComponent();
            _defaultHex = defaultHex;
            SetupUI();
        }

        private void SetupUI()
        {
           
            //this.Size = new Size(600, 250);  // Kích thước nhỏ gọn
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(248, 248, 255);  // Nền sáng nhẹ, dễ nhìn
            // C
          
            spinC.Properties.MinValue = 0;
            spinC.Properties.MaxValue = 100;
            spinC.Properties.Increment = 1;
            spinC.Width = 30;
            //spinC.EditValue = 0;
            // M
         
            spinM.Properties.MinValue = 0;
            spinM.Properties.MaxValue = 100;
            spinM.Properties.Increment = 1;
            spinM.Width = 30;
            //spinM.EditValue = 0;
            // Y
          
            spinY.Properties.MinValue = 0;
            spinY.Properties.MaxValue = 100;
            spinY.Properties.Increment = 1;
            spinY.Width = 30;
            //spinY.EditValue = 0;

            // Y
    
            spinK.Properties.MinValue = 0;
            spinK.Properties.MaxValue = 100;
            spinK.Properties.Increment = 1;
            spinK.Width = 30;
            //spinK.EditValue = 0;

            colorPickEdit1.EditValue = _defaultHex;



        }
 





        private void bntSubmit_Click(object sender, EventArgs e)
        {
            //this.ActiveControl = txtDot;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        

        private void colorPickEdit1_EditValueChanged(object sender, EventArgs e)
        {
            ColorPickEdit edit = sender as ColorPickEdit;
            if (edit != null && edit.Color != default(Color))
            {
                clsCMYkColorLib cmyk = clsCMYkColorLib.FromRgb(edit.Color);
                // Round và gán giá trị nguyên (phù hợp IsFloatValue = false)
                spinC.EditValue = (decimal)Math.Round(cmyk.C * 100);
                spinM.EditValue = (decimal)Math.Round(cmyk.M * 100);
                spinY.EditValue = (decimal)Math.Round(cmyk.Y * 100);
                spinK.EditValue = (decimal)Math.Round(cmyk.K * 100);

                //Hex =$"Hệ Màu Hexa(RGB): #{edit.Color.Name.ToString().ToUpper()}";
                //CMYK = cmyk.ToString();

                //string text = $"Hệ Màu CMYK : {CMYK}";
              
                //lblCmykText.Text = text;
                //lblHexText.Text = Hex;
            }
        }

        private decimal _currentC = 0;
        private decimal _currentM = 0;
        private decimal _currentY = 0;
        private decimal _currentK = 0;

        private System.Threading.Timer _timerC;
        private System.Threading.Timer _timerM;
        private System.Threading.Timer _timerY;
        private System.Threading.Timer _timerK;

        private readonly object _lockObj = new object();

        // Method cập nhật cuối cùng
        private void UpdateColorPreview()
        {
            lock (_lockObj)
            {
                try
                {
                    float c = (float)_currentC / 100f;
                    float m = (float)_currentM / 100f;
                    float y = (float)_currentY / 100f;
                    float k = (float)_currentK / 100f;

                    clsCMYkColorLib cmyk = new clsCMYkColorLib(c, m, y, k);
                    Color color = cmyk.ToRgb();
                    string text = $"Hệ Màu CMYK : {cmyk}";
                    string cmykStr = cmyk.ToString();

                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            lblCmykText.Text = text;
                       
                            colorPickEdit1.Color = color;
                            CMYK = cmykStr;
                            Hex = $"#{color.Name.ToString().ToUpper()}";
                            lblHexText.Text = $"Hệ Màu Hexa(RGB):{Hex}" ;
                        }));
                    }
                    else
                    {
                        lblCmykText.Text = text;                    
                        colorPickEdit1.Color = color;
                        CMYK = cmykStr;
                        Hex = $"#{color.Name.ToString().ToUpper()}";
                        lblHexText.Text = $"Hệ Màu Hexa(RGB):{Hex}";
                    }
                }
                catch { }
            }
        }

        private void spinC_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue == null) return;
            _currentC = Convert.ToDecimal(e.NewValue); // ✅ C → _currentC
            UpdateColorPreview();
        }

        private void spinM_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue == null) return;
            _currentM = Convert.ToDecimal(e.NewValue); // ✅ M → _currentM
            UpdateColorPreview();
        }

        private void spinY_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue == null) return;
            _currentY = Convert.ToDecimal(e.NewValue); // ✅ Y → _currentY
            UpdateColorPreview();
        }

        private void spinK_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue == null) return;
            _currentK = Convert.ToDecimal(e.NewValue); // ✅ K → _currentK
            UpdateColorPreview();
        }






    }

}