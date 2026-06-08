
using NtbSoft.ERP.Win.Modules.ResourceForm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Utils
{
    public class HelperExtensions
    {
        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            ImageConverter converter = new ImageConverter();
            byte[] imgArray = (byte[])converter.ConvertTo(imageIn, typeof(byte[]));
            return imgArray;
        }

        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static string GetNewID(string keyCode, string maxId)
        {
            if (!string.IsNullOrEmpty(maxId))
            {
                //string id = maxId.Substring(maxId.Length - 6);
                string id = maxId.Replace(keyCode, "");
                if (System.Text.RegularExpressions.Regex.IsMatch(id, "[a-zA-Z]"))
                    return string.Empty;

                int nextID = Convert.ToInt32(id) + 1;
                //string nextStr = maxId.Remove(maxId.Length - 6);
                return keyCode + nextID.ToString("D3");
            }
            return string.Empty;
        }

        static object locker = new object();
        public static string Generate15UniqueDigits()
        {
            lock (locker)
            {
                System.Threading.Thread.Sleep(100);
                return DateTime.Now.ToString("yyyyMMddHHmmssf");
            }
        }
    }

    public class clsWaitForm
    {
        public static void ShowSuccessFormCustom(Form Parent, int closingDelay,string text)
        {
            if (Parent.InvokeRequired)
            {
                Parent.Invoke(new Action<Form, int,string>(ShowSuccessFormCustom), new object[] { Parent, closingDelay,text });
                return;
            }
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(Parent, typeof(frmCustomSucess), true, true, false, true);
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, closingDelay, Parent);
        }
        public static void ShowWaningFormCustom(Form Parent, int closingDelay, string text)
        {
            if (Parent.InvokeRequired)
            {
                Parent.Invoke(new Action<Form, int, string>(ShowWaningFormCustom), new object[] { Parent, closingDelay, text });
                return;
            }
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(Parent, typeof(frmCustomWarning), true, true, false, true);
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, closingDelay, Parent);
        }
        public static void ShowWaningFormCustomV2(Form Parent, int closingDelay, string text)
        {
            if (Parent.InvokeRequired)
            {
                Parent.Invoke(new Action<Form, int, string>(ShowWaningFormCustomV2), new object[] { Parent, closingDelay, text });
                return;
            }
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(Parent, typeof(frmCustomWarningV2), true, true, false, true);
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, closingDelay, Parent);
        }
        public static void ShowWaningFormCustomV3(Form Parent, int closingDelay, string text)
        {
            if (Parent.InvokeRequired)
            {
                Parent.Invoke(new Action<Form, int, string>(ShowWaningFormCustomV3), new object[] { Parent, closingDelay, text });
                return;
            }
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(Parent, typeof(frmCustomWarningV3), true, true, false, true);
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, closingDelay, Parent);
        }
        public static void ShowWaningFormCustomV4(Form Parent, int closingDelay, string text)
        {
            if (Parent.InvokeRequired)
            {
                Parent.Invoke(new Action<Form, int, string>(ShowWaningFormCustomV4), new object[] { Parent, closingDelay, text });
                return;
            }
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(Parent, typeof(frmCustomWarningV4), true, true, false, true);
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, closingDelay, Parent);
        }
        public static void ShowSuccessForm(Form Parent)
        {
            if (Parent.InvokeRequired)
            {
                Parent.Invoke(new Action<Form>(ShowSuccessForm), new object[] { Parent });
                return;
            }
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(Parent, typeof(frmExcSuccess), true, true, false, true);
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 4000, Parent);
        }

        public static void ShowSuccessForm(Form Parent, int closingDelay)
        {
            if (Parent.InvokeRequired)
            {
                Parent.Invoke(new Action<Form, int>(ShowSuccessForm), new object[] { Parent, closingDelay });
                return;
            }
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(Parent, typeof(frmExcSuccess), true, true, false, true);
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, closingDelay, Parent);
        }

        public static void ShowWaitForm(Form Parent, int closingDelay)
        {
            if (Parent.InvokeRequired)
            {
                Parent.Invoke(new Action<Form, int>(ShowWaitForm), new object[] { Parent, closingDelay });
                return;
            }
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(Parent, typeof(frmWait), true, true, false, true);
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, closingDelay, Parent);
        }
        //public static void ShowErrorForm(Form parent,string caption,int delaytime)
        //{
        //    if(parent.InvokeRequired)
        //    {
        //        parent.Invoke(new Action<Form, string, int>(ShowErrorForm), new object[] { parent, caption, delaytime });
        //        return;
        //    }
        //    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm()
        //}

        //public static void ShowWaitForm(Form Parent, string Caption)
        //{
        //    if (Parent.InvokeRequired)
        //    {
        //        Parent.Invoke(new Action<Form, string>(ShowWaitForm), new object[] { Parent, Caption });
        //        return;
        //    }
        //    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(Parent, typeof(frmSplashScreen), true, true, false, true);
        //    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption(Caption);
        //    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 4000, Parent);
        //}

        //public static void ShowWaitForm(Form Parent, string Caption, int closingDelay)
        //{
        //    if (Parent.InvokeRequired)
        //    {
        //        Parent.Invoke(new Action<Form, string, int>(ShowWaitForm), new object[] { Parent, Caption, closingDelay });
        //        return;
        //    }
        //    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(Parent, typeof(frmSplashScreen), true, true, false, true);
        //    DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormCaption(Caption);
        //    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, closingDelay, Parent);
        //}
        public static void ShowErrorForm(Form Parent)
        {
            if (Parent.InvokeRequired)
            {
                Parent.Invoke(new Action<Form>(ShowErrorForm), new object[] { Parent });
                return;
            }
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(Parent, typeof(frmExcError), true, true, false, true);
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, 4000, Parent);
        }

        public static void ShowErrorForm(Form Parent, int closingDelay)
        {
            if (Parent.InvokeRequired)
            {
                Parent.Invoke(new Action<Form, int>(ShowErrorForm), new object[] { Parent, closingDelay });
                return;
            }
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(Parent, typeof(frmExcError), true, true, false, true);
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false, closingDelay, Parent);
        }
    }
    public class BooleanFormatter : IFormatProvider, ICustomFormatter
    {
        string _trueString, _falseString;
        public BooleanFormatter(string trueString, string falseString)
        {
            this._trueString = trueString;
            this._falseString = falseString;
        }
        public object GetFormat(Type formatType)
        {
            return this;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            bool formatValue = Convert.ToBoolean(arg);
            if (formatValue)
                return _trueString;
            else
                return _falseString;
        }
    }

    public class clsLogFile
    {
        public static void WriteFileLogInClient(string content)
        {
            try
            {
                string folder = System.Windows.Forms.Application.StartupPath + @"\LogFile\";
                string path = folder + DateTime.Today.ToString("dd-MM-yyyy") + ".txt";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }
                using (StreamWriter w = File.AppendText(path))
                {
                    string ip = GetLocalIPAddress();
                    w.WriteLine("\r\nLog Entry : ");
                    w.WriteLine("{0}", DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    string err = string.Format("Account: {0} IP: {1} Content: {2}", GlobleData.UserName, ip, content);
                    w.WriteLine(err);
                    w.WriteLine("__________________________");
                    w.Flush();
                    w.Close();
                }
            }
            catch (Exception ex)
            {
                WriteFileLogInClient(ex.Message);
                return;
            }
        }

        public static string GetContentFileLog(string content)
        {
            string ip = GetLocalIPAddress();
            string fullContent = string.Format("Account: {0} \r\nIP: {1} \r\nContent: {2}", GlobleData.UserName, ip, content);
            return fullContent;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "No network adapters with an IPv4 address in the system!";
        }
    }
}
