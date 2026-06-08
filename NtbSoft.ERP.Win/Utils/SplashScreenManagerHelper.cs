using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraSplashScreen;
using DevExpress.Utils.Drawing;
using System.Drawing;
using DevExpress.Utils.Text;

namespace NtbSoft.ERP.Win.Utils
{
    class SplashScreenManagerHelper : ICustomImagePainter
    {
        static SplashScreenManagerHelper()
        {
            Paiter = new SplashScreenManagerHelper();
        }
        protected SplashScreenManagerHelper() { }

        public static SplashScreenManagerHelper Paiter { get; private set; }

        ViewInfo viewInfo = null;
        public ViewInfo ViewInfo
        {
            get
            {
                if (this.viewInfo == null) viewInfo = new ViewInfo();
                return this.viewInfo;
            }
        }

        public void Draw(GraphicsCache cache, Rectangle bounds)
        {
            PointF point = ViewInfo.CalcProgressLabelPoint(cache, bounds);
            cache.Graphics.DrawString(ViewInfo.Text, ViewInfo.ProgressLabelFont, ViewInfo.Brush, point);
        }

    }
    class ViewInfo
    {
        public ViewInfo()
        {
            Counter = 1;
            Stage = string.Empty;

        }
        public int Counter { get; set; }

        public string Stage { get; set; }

        public string Text
        {
            get
            {
                if (Stage == string.Empty) return string.Empty;
                return string.Format("{0}({1}%)", Stage, Counter.ToString("D2"));
            }
        }

        public System.Drawing.PointF CalcProgressLabelPoint(DevExpress.Utils.Drawing.GraphicsCache cache, System.Drawing.Rectangle bounds)
        {
            const int yOffset = 230;
            Size size = TextUtils.GetStringSize(cache.Graphics, Text, ProgressLabelFont);
            return new Point(bounds.Width / 2 - size.Width + 300 / 2, yOffset);
        }

        Font progressFont = null;

        public Font ProgressLabelFont
        {
            get
            {
                if (this.progressFont == null) this.progressFont = new Font("Consolas", 10);
                return this.progressFont;
            }
        }
        Brush brush = null;
        public Brush Brush
        {
            get
            {
                if (this.brush == null) this.brush = new SolidBrush(Color.Black);
                return this.brush;
            }
        }

    }
}
