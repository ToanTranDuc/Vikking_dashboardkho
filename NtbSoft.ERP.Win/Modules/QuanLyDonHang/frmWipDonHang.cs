using CefSharp;
using CefSharp.WinForms;
using DevExpress.XtraEditors;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{

    public partial class frmWipDonHang : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                              new System.Configuration.AppSettingsReader();
        string URL = string.Empty;

        private DataGridView dataGridView;
        private DataTable dt;
        private ChromiumWebBrowser chromiumWebBrowser;
        private double _zoomLevel = 0;
        public frmWipDonHang()
        {
            URL = (string)settingsReader.GetValue("URL_WIP", typeof(String));
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += frmWipDonHang_KeyDown;
            InitializeChromiumBrowser();
            InitializeDataGridView();
            this.Load += frmWip;
            this.WindowState = FormWindowState.Maximized;
        }
        private void frmWip(object sender, EventArgs e)
        {
            if (dataGridView.Rows.Count > 0)
            {
                string userName = GlobleData.UserName;
                int selectedRowIndex = dataGridView.CurrentCell.RowIndex;
                string selectedURL = dataGridView.Rows[selectedRowIndex].Cells["URL"].Value.ToString();
                string requestURL = AppendQueryString(selectedURL, $"userName={Uri.EscapeDataString(userName)}");
                requestURL = AppendQueryString(requestURL, "host=win");

                chromiumWebBrowser.Load(requestURL);
            }
        }
        private static string AppendQueryString(string url, string queryPart)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return url;
            }

            string separator = url.Contains("?")
                ? (url.EndsWith("?") || url.EndsWith("&") ? string.Empty : "&")
                : "?";

            return url + separator + queryPart;
        }
        private void InitializeChromiumBrowser()
        {
            chromiumWebBrowser = new ChromiumWebBrowser();
            chromiumWebBrowser.Dock = DockStyle.Fill;
            chromiumWebBrowser.KeyboardHandler = new BrowserZoomKeyboardHandler(HandleBrowserZoomShortcut);
            chromiumWebBrowser.FrameLoadEnd += (s, e) =>
            {
                if (e.Frame.IsMain)
                {
                    ApplyZoom();
                }
            };
            chromiumWebBrowser.LifeSpanHandler = new PopupLifeSpanHandler();
            Controls.Add(chromiumWebBrowser);
            chromiumWebBrowser.DownloadHandler = new DownloadHandler();
        }
        private void frmWipDonHang_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control)
            {
                return;
            }

            if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus)
            {
                AdjustZoom(0.25);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus)
            {
                AdjustZoom(-0.25);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.D0 || e.KeyCode == Keys.NumPad0)
            {
                ResetZoom();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
        private void HandleBrowserZoomShortcut(Keys keyCode)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => HandleBrowserZoomShortcut(keyCode)));
                return;
            }

            if (keyCode == Keys.Add || keyCode == Keys.Oemplus)
            {
                AdjustZoom(0.25);
            }
            else if (keyCode == Keys.Subtract || keyCode == Keys.OemMinus)
            {
                AdjustZoom(-0.25);
            }
            else if (keyCode == Keys.D0 || keyCode == Keys.NumPad0)
            {
                ResetZoom();
            }
        }
        private void AdjustZoom(double delta)
        {
            _zoomLevel = Math.Max(-2, Math.Min(4, _zoomLevel + delta));
            ApplyZoom();
        }
        private void ResetZoom()
        {
            _zoomLevel = 0;
            ApplyZoom();
        }
        private void ApplyZoom()
        {
            if (chromiumWebBrowser == null || chromiumWebBrowser.IsDisposed || !chromiumWebBrowser.IsBrowserInitialized)
            {
                return;
            }

            chromiumWebBrowser.GetBrowserHost().SetZoomLevel(_zoomLevel);
        }
        private void InitializeDataGridView()
        {
            dataGridView = new DataGridView();
            dataGridView.Width = 100;

            dt = new DataTable();
            dt.Columns.Add("URL", typeof(string));


            string dynamicURL = URL;
            dt.Rows.Add(dynamicURL);

            dataGridView.DataSource = dt;
            Controls.Add(dataGridView);
        }
        class DownloadHandler : IDownloadHandler
        {
            public event EventHandler<DownloadItem> OnBeforeDownloadFired;

            public event EventHandler<DownloadItem> OnDownloadUpdatedFired;
            private string pathFile;

            public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
            {
                string directoryPath;
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    // Set the initial directory (optional)
                    folderDialog.SelectedPath = @"C:\";

                    // Show the folder browser dialog
                    DialogResult result = folderDialog.ShowDialog();

                    // Check if the user selected a folder and clicked OK
                    if (result == DialogResult.OK)
                    {
                        directoryPath = folderDialog.SelectedPath;
                    }
                    else return;
                }
                pathFile = directoryPath + "\\" + downloadItem.SuggestedFileName;

                if (downloadItem.IsValid)
                {
                    Console.WriteLine("== File information ========================");
                    Console.WriteLine(" File URL: {0}", downloadItem.Url);
                    Console.WriteLine(" Suggested FileName: {0}", downloadItem.SuggestedFileName);
                    Console.WriteLine(" MimeType: {0}", downloadItem.MimeType);
                    Console.WriteLine(" Content Disposition: {0}", downloadItem.ContentDisposition);
                    Console.WriteLine(" Total Size: {0}", downloadItem.TotalBytes);
                    Console.WriteLine("============================================");
                }

                OnBeforeDownloadFired?.Invoke(this, downloadItem);

                if (!callback.IsDisposed)
                {
                    using (callback)
                    {
                        // Define the Downloads Directory Path
                        // You can use a different one, in this example we will hard-code it

                        string DownloadsDirectoryPath = directoryPath;

                        callback.Continue(
                            Path.Combine(
                                DownloadsDirectoryPath,
                                downloadItem.SuggestedFileName
                            ),
                            showDialog: false
                        );
                    }
                }
            }

            /// https://cefsharp.github.io/api/51.0.0/html/T_CefSharp_DownloadItem.htm
            public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
            {
                OnDownloadUpdatedFired?.Invoke(this, downloadItem);

                if (downloadItem.IsValid)
                {
                    // Show progress of the download
                    if (downloadItem.IsInProgress && (downloadItem.PercentComplete != 0))
                    {
                        Console.WriteLine(
                            "Current Download Speed: {0} bytes ({1}%)",
                            downloadItem.CurrentSpeed,
                            downloadItem.PercentComplete
                        );
                    }

                    if (downloadItem.IsComplete)
                    {
                        DialogResult res = MessageBox.Show("Bạn có muốn mở file không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (res == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start("explorer.exe", pathFile);
                        }

                        Console.WriteLine("The download has been finished !");
                    }
                }
            }
            public bool CanDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, string url, string requestMethod)
            {
                return true;
            }

        }
        class BrowserZoomKeyboardHandler : IKeyboardHandler
        {
            private readonly Action<Keys> handleZoomShortcut;

            public BrowserZoomKeyboardHandler(Action<Keys> handleZoomShortcut)
            {
                this.handleZoomShortcut = handleZoomShortcut;
            }

            public bool OnPreKeyEvent(
                IWebBrowser chromiumWebBrowser,
                IBrowser browser,
                KeyType type,
                int windowsKeyCode,
                int nativeKeyCode,
                CefEventFlags modifiers,
                bool isSystemKey,
                ref bool isKeyboardShortcut)
            {
                if (type != KeyType.RawKeyDown || !modifiers.HasFlag(CefEventFlags.ControlDown))
                {
                    return false;
                }

                Keys keyCode = (Keys)windowsKeyCode;
                if (keyCode == Keys.Add || keyCode == Keys.Oemplus ||
                    keyCode == Keys.Subtract || keyCode == Keys.OemMinus ||
                    keyCode == Keys.D0 || keyCode == Keys.NumPad0)
                {
                    handleZoomShortcut(keyCode);
                    isKeyboardShortcut = true;
                    return true;
                }

                return false;
            }

            public bool OnKeyEvent(
                IWebBrowser chromiumWebBrowser,
                IBrowser browser,
                KeyType type,
                int windowsKeyCode,
                int nativeKeyCode,
                CefEventFlags modifiers,
                bool isSystemKey)
            {
                return false;
            }
        }
        public class InitCefChorme
        {
            private static InitCefChorme _instance;
            public static InitCefChorme Instance => _instance ?? (
                _instance = new InitCefChorme());
        }
        public class PopupLifeSpanHandler : ILifeSpanHandler
        {
            public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
                return false;
            }

            public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
            }

            public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
            }

            public bool OnBeforePopup(
                IWebBrowser chromiumWebBrowser,
                IBrowser browser,
                IFrame frame,
                string targetUrl,
                string targetFrameName,
                WindowOpenDisposition targetDisposition,
                bool userGesture,
                IPopupFeatures popupFeatures,
                IWindowInfo windowInfo,
                IBrowserSettings browserSettings,
                ref bool noJavascriptAccess,
                out IWebBrowser newBrowser)
            {
                // BẮT BUỘC
                newBrowser = null;

                var frm = new XtraForm
                {
                    WindowState = FormWindowState.Maximized,
                    StartPosition = FormStartPosition.CenterScreen,
                    Text = "Thư viện WIP"
                };

                var browserPopup = new ChromiumWebBrowser(targetUrl)
                {
                    Dock = DockStyle.Fill,
                    LifeSpanHandler = new PopupLifeSpanHandler()
                };

                frm.Controls.Add(browserPopup);
                frm.Show();

                // Tự xử lý popup bằng form riêng
                return true;
            }
        }
    }

}
