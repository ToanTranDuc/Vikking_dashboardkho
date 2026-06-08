using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;  // For XtraForm
namespace NtbSoft.ERP.Win.Utils
{
    class clsCMYkColorLib
    {
        public float C { get; }
        public float M { get; }
        public float Y { get; }
        public float K { get; }

        public clsCMYkColorLib(float c, float m, float y, float k)
        {
            C = ClampCmyk(c);
            M = ClampCmyk(m);
            Y = ClampCmyk(y);
            K = ClampCmyk(k);
        }

        private static float ClampCmyk(float value)
        {
            if (value < 0 || float.IsNaN(value))
            {
                value = 0;
            }
            if (value > 1)
            {
                value = 1;
            }
            return value;
        }

        // Convert CMYK (0-1) sang RGB
        public Color ToRgb()
        {
            int r = (int)(255 * (1 - C) * (1 - K));
            int g = (int)(255 * (1 - M) * (1 - K));
            int b = (int)(255 * (1 - Y) * (1 - K));
            return Color.FromArgb(r, g, b);
        }

        // Convert RGB sang CMYK (0-1)
        public static clsCMYkColorLib FromRgb(Color rgb)
        {
            float rf = rgb.R / 255f;
            float gf = rgb.G / 255f;
            float bf = rgb.B / 255f;

            float k = 1 - Math.Max(Math.Max(rf, gf), bf);
            float c = (1 - rf - k) / (1 - k);
            float m = (1 - gf - k) / (1 - k);
            float y = (1 - bf - k) / (1 - k);

            // Xử lý trường hợp K=1 (trắng/đen, tránh chia 0)
            if (k == 1) { c = m = y = 0; }
            else { c = Math.Max(0, c); m = Math.Max(0, m); y = Math.Max(0, y); }

            return new clsCMYkColorLib(c, m, y, k);
        }

        // Tạo string hiển thị CMYK
        public override string ToString()
        {
            return $"({Math.Round(C * 100)}, {Math.Round(M * 100)}, {Math.Round(Y * 100)}, {Math.Round(K * 100)})";
        }
    }


    // ===== ProgressForm.cs =====
    public partial class ProgressForm : XtraForm
    {
        public ProgressForm()
        {
            this.Size = new Size(300, 120);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.TopMost = true;
            this.BackColor = Color.White;
            this.Padding = new Padding(15);
            this.ShowInTaskbar = false;
            this.ControlBox = false;

            Label lbl = new Label
            {
                Text = "Đang tải dữ liệu...",
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            System.Windows.Forms.ProgressBar pb = new System.Windows.Forms.ProgressBar
            {
                Dock = DockStyle.Fill,
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30
            };

            this.Controls.Add(pb);
            this.Controls.Add(lbl);
        }
    }

}
