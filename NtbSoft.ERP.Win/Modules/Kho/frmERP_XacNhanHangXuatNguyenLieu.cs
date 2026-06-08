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
using CefSharp;
using CefSharp.WinForms;
using NtbSoft.ERP.Win.Utils;
using System.IO;
namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERP_XacNhanHangXuatNguyenLieu : DevExpress.XtraEditors.XtraForm
    { 
        System.Configuration.AppSettingsReader settingsReader =
                                               new System.Configuration.AppSettingsReader();
        string URL = string.Empty;

        private ChromiumWebBrowser chromiumWebBrowser;
        private DataGridView dataGridView;
        private DataTable dt;
        public frmERP_XacNhanHangXuatNguyenLieu()
        {
            URL = (string)settingsReader.GetValue("URL_XacNhanHangXuatNL", typeof(String));
            InitializeComponent();
            InitializeChromiumBrowser();
            InitializeDataGridView();
            this.Load += frmKTXuatHang;
            this.WindowState = FormWindowState.Maximized;
            InitializeComponent();
        }
        private void frmKTXuatHang(object sender, EventArgs e)
        {
            if (dataGridView.Rows.Count > 0)
            {
                string userName = GlobleData.UserName;
                int selectedRowIndex = dataGridView.CurrentCell.RowIndex;
                string selectedURL = dataGridView.Rows[selectedRowIndex].Cells["URL"].Value.ToString();
                string requestURL = $"{selectedURL}?userName={userName}";

                chromiumWebBrowser.Load(requestURL);
            }
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


        private void InitializeChromiumBrowser()
        {
            chromiumWebBrowser = new ChromiumWebBrowser();
            chromiumWebBrowser.Dock = DockStyle.Fill;
            Controls.Add(chromiumWebBrowser);
            chromiumWebBrowser.DownloadHandler = new DownloadHandler();
            chromiumWebBrowser.FrameLoadEnd += ChromiumWebBrowser_FrameLoadEnd;
        }
        private async void ChromiumWebBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            // Chỉ chạy ở frame chính
            if (!e.Frame.IsMain) return;

            string jsCode = @"
                    (function() {
                        var el = document.getElementById('home');
                        if (el) {
                            el.style.display = 'none';
                            return true;
                        }
                        return false;
                    })();
                ";

            await e.Frame.EvaluateScriptAsync(jsCode);
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
        public class InitCefChorme
        {
            private static InitCefChorme _instance;
            public static InitCefChorme Instance => _instance ?? (
                _instance = new InitCefChorme());
        }
    }
}