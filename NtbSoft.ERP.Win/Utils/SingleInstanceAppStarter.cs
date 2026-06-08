using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.ApplicationServices;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Utils
{
    public class SingleInstanceAppStarter
    {
        static SingleInstanceApp app = null;

        // Construct SingleInstanceApp object, and invoke its run method.
        public static void Start(Form f, StartupNextInstanceEventHandler handler)
        {
            if (app == null && f != null)
                app = new SingleInstanceApp(f);

            // Wire up StartupNextInstance event handler.
            app.StartupNextInstance += handler;
            app.Run(Environment.GetCommandLineArgs());
        }
    }
    public class GlobleData
    {
        public static List<string> lstDVSX { get; set; }
        public static string NhanVien { get; set; }
        static bool isUserLoggedIn;
        public static bool IsUserLoggedIn
        {
            get { return isUserLoggedIn; }
            set { isUserLoggedIn = value; }
        }

        static string userName;
        public static string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        static string maKho;
        public static string MaKho
        {
            get { return maKho; }
            set { maKho = value; }
        }

        static bool isKhoTong;
        public static bool IsKhoTong
        {
            get { return isKhoTong; }
            set { isKhoTong = value; }
        }


    }
}
