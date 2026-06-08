using NtbSoft.ERP.Win.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.WinForms;

namespace NtbSoft.ERP.Win.Modules.Erp.Kehoach
{
    public partial class MessageBoxDialogCanhBaoMaHang : DevExpress.XtraEditors.XtraForm
    {
        private Timer timer = new Timer();
        private float rotationAngle = 0.0f;
        public bool resulut { get; private set; }
        string _tenNPL = string.Empty, _tenmauNPL = string.Empty;
        public MessageBoxDialogCanhBaoMaHang(string tenNPL,string tenmauNPL)
        {
            InitializeComponent();
            this._tenNPL = tenNPL;
            this._tenmauNPL = tenmauNPL;
            InitializeUI();
            InitializeTimer();
            resulut = false;
         
            // Đặt StartPosition là Manual
            this.StartPosition = FormStartPosition.Manual;

            // Đặt vị trí cho form là giữa màn hình
            this.Location = new System.Drawing.Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                                                      (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // hoặc FormBorderStyle.Fixed3D hoặc FormBorderStyle.FixedToolWindow, tùy thuộc vào sở thích của bạn

            // Đặt kích thước cố định
            this.Size = new System.Drawing.Size(484, 150);
        }

        private void InitializeUI()
        {
            string encodedTenNPL = WebUtility.HtmlEncode(_tenNPL);
            string encodedTenmauNPL = WebUtility.HtmlEncode(_tenmauNPL);

            HtmlLabel htmlLabel2 = new HtmlLabel();
            htmlLabel2.AutoSize = true;
            htmlLabel2.Text = "Tên nguyên phụ liệu: <span style='color:green; font-weight: bold;font-size: 15px;'>" + encodedTenNPL + "</span>, <br/> và tên màu nguyên phụ liệu <span style='font-size: 15px; font-weight: bold;'>" + encodedTenmauNPL + "</span>  <br/> đã có trong danh sách .";
            htmlLabel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

            this.flowLayoutPanel2.Controls.Add(htmlLabel2);
        }


        private void InitializeTimer()
        {
            timer.Interval = 100; // Thay đổi giảm hoặc tăng độ nhanh chậm
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Di chuyển PictureBox theo đường chuyển động sine
            int x = 299 + (int)(Math.Sin(DateTime.Now.Ticks * 0.0001) * 10); // Điều chỉnh hệ số để thay đổi độ lắc
            int y = 12;

            this.pictureBox1.Location = new System.Drawing.Point(x, y);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            resulut = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            resulut = false;
            this.Close();
        }
        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    rotationAngle += 5.0f;
        //    pictureBox1.Image = RotateImage(Resources.canhbao, rotationAngle);
        //}

        //private Bitmap RotateImage(Image image, float angle)
        //{
        //    Bitmap rotatedImage = new Bitmap(image.Width, image.Height);

        //    using (Graphics g = Graphics.FromImage(rotatedImage))
        //    {
        //        g.TranslateTransform(image.Width / 2, image.Height / 2);
        //        g.RotateTransform(angle);
        //        g.TranslateTransform(-image.Width / 2, -image.Height / 2);
        //        g.DrawImage(image, new Point(0, 0));
        //    }

        //    return rotatedImage;
        //}
        //private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        //{

        //}
    }
}
