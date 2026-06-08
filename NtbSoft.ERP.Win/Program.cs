using NtbSoft.ERP.Win.Modules.KeHoach;
using NtbSoft.ERP.Win.Modules.ThuVien;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
namespace NtbSoft.ERP.Win
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            var settings = new CefSettings();
            settings.CefCommandLineArgs.Add("disable-gpu");
            settings.CefCommandLineArgs.Add("disable-gpu-compositing");
            settings.CefCommandLineArgs.Add("disable-gpu-vsync");
            settings.CefCommandLineArgs.Add("disable-d3d11");
            Cef.Initialize(settings);
            DevExpress.XtraEditors.WindowsFormsSettings.SetDPIAware();
            bool isWindow = IsWindows11();
            if (isWindow)
                DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont = new System.Drawing.Font("Tahoma", 7.5f);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
            //Application.Run(new frmThuVienMauSP());
        }
        static bool IsWindows11()
        {
            try
            {
                // Đọc trực tiếp từ Registry - luôn trả về đúng
                var key = Microsoft.Win32.Registry.LocalMachine
                          .OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

                if (key != null)
                {
                    var buildNumber = int.Parse(key.GetValue("CurrentBuildNumber").ToString());
                    // Windows 11 build >= 22000
                    return buildNumber >= 22000;
                }
            }
            catch { }

            return false;
        }
    }
}
