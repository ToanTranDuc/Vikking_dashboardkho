using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Configuration;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmQLSX_Test : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        // Hằng số
        const int GWL_STYLE = -16;
        const int WS_BORDER = 0x00800000;
        const int WS_DLGFRAME = 0x00400000;
        const int WS_CAPTION = 0x00C00000;
        const int WS_THICKFRAME = 0x00040000;
        string _URL = string.Empty;
        Process proc2;
        System.Configuration.AppSettingsReader settingsReader =
                                                  new AppSettingsReader();
        List<SystemUserModuleEntity> _listUserModule;
        SystemUserModuleService _serviceUserModule;
        public frmQLSX_Test()
        {
            InitializeComponent();
           
            _URL = (string)settingsReader.GetValue("URL", typeof(String));
            
            _serviceUserModule = new SystemUserModuleService();
            EmbedAppIntoPanel(@"D:\Phan mem Cty\Quanlysanxuat\Viking\Source\NtbSoft.ERP.Win\bin\QLDH\N-Garmentmanager.exe", panel1);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            try
            {
                if (proc2 != null && !proc2.HasExited)
                    proc2.Kill();
            }
            catch { }
        }
        private void EmbedAppIntoPanel(string exePath, Panel hostPanel)
        {
            try
            {
                proc2 = new Process();
                proc2.StartInfo.FileName = exePath;
                proc2.Start();
                proc2.WaitForInputIdle();

                // Chờ cửa sổ chính sẵn sàng
                while (proc2.MainWindowHandle == IntPtr.Zero)
                {
                    System.Threading.Thread.Sleep(100);
                    proc2.Refresh();
                }

                // Gán parent thật sự là panel1
                SetParent(proc2.MainWindowHandle, hostPanel.Handle);

                // Bỏ viền / caption / resize border    
                int style = GetWindowLong(proc2.MainWindowHandle, GWL_STYLE);
                style = style & ~(WS_BORDER | WS_DLGFRAME | WS_CAPTION | WS_THICKFRAME);
                SetWindowLong(proc2.MainWindowHandle, GWL_STYLE, style);

                // Dock vào panel
                MoveWindow(proc2.MainWindowHandle, 0, 0, hostPanel.Width, hostPanel.Height, true);

                // Cho nó resize theo panel1
                hostPanel.Resize += (s, e) =>
                {
                    MoveWindow(proc2.MainWindowHandle, 0, 0, hostPanel.Width, hostPanel.Height, true);
                };
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
           
        }
    }
}