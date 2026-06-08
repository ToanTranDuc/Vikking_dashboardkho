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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmViewKiemNL : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                       new System.Configuration.AppSettingsReader();
        string URL = string.Empty;

        private ChromiumWebBrowser chromiumWebBrowser;
        private DataGridView dataGridView;
        private DataTable dt;
        string _SoLoID = string.Empty, _SoLo = string.Empty, _MaNPL= string.Empty;
        public frmViewKiemNL(string soloid, string solo, string manpl)
        {
            _SoLoID = soloid;
            _SoLo = solo;
            _MaNPL = manpl;
            URL = (string)settingsReader.GetValue("URL_KiemVTNL", typeof(String));
            InitializeComponent();
            InitializeChromiumBrowser();
            InitializeDataGridView();
            this.Load += frmKTXuatHang;
            this.WindowState = FormWindowState.Maximized;
        }

        private void frmKTXuatHang(object sender, EventArgs e)
        {
            if (dataGridView.Rows.Count > 0)
            {
                string userName = GlobleData.UserName;
                int selectedRowIndex = dataGridView.CurrentCell.RowIndex;
                string selectedURL = dataGridView.Rows[selectedRowIndex].Cells["URL"].Value.ToString();
                string requestURL = $"{selectedURL}?userName={userName}&soLoID={_SoLoID}&soLo={Uri.EscapeDataString(_SoLo)}&maNPL={Uri.EscapeDataString(_MaNPL)}";

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
            chromiumWebBrowser.FrameLoadEnd += ChromiumWebBrowser_FrameLoadEnd;
            chromiumWebBrowser.DownloadHandler = new DownloadHandler();
            Controls.Add(chromiumWebBrowser);
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


        private void ChromiumWebBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            //if (!e.Frame.IsMain) return;

            //string js = $@"
            //(function () {{

            //    // 1. Ẩn nút Excel
            //    var btnExport = document.querySelector('.btn-export');
            //    if (btnExport) {{
            //        btnExport.style.display = 'none';
            //    }}

            //    // 2. Auto chọn Số lô
            //    var soLoId = '{_SoLoID}';

            //    var timerSoLo = setInterval(function () {{
            //        var select = document.getElementById('SoLo');
            //        if (!select) return;

            //        if (select.options.length > 1) {{
            //            select.value = soLoId;
            //            select.dispatchEvent(new Event('change', {{ bubbles: true }}));
            //            clearInterval(timerSoLo);

            //            // Sau khi set Số lô xong → xử lý batch
            //            autoCheckAllBatch();
            //        }}
            //    }}, 300);

            //    function autoCheckAllBatch() {{
            //        var retry = 0;
            //        var timerBatch = setInterval(function () {{
            //            retry++;
            
            //            var batchInput = document.getElementById('batchInput');
            //            if (!batchInput) return;

            //            // Mở dropdown batch
            //            batchInput.click();

            //            var checkboxes = document.querySelectorAll('#itemList input[type=""checkbox""]');
            //            if (checkboxes.length > 0) {{
            //                checkboxes.forEach(function (cb) {{
            //                    cb.checked = true;
            //                    cb.dispatchEvent(new Event('change', {{ bubbles: true }}));
            //                }});

            //                // cập nhật checkbox Check All nếu có
            //                var checkAll = document.getElementById('checkAll');
            //                if (checkAll) {{
            //                    checkAll.checked = true;
            //                }}

            //                clearInterval(timerBatch);
            //            }}

            //            if (retry > 20) {{
            //                clearInterval(timerBatch);
            //            }}

            //        }}, 300);
            //    }}

            //}})();
            //";

            //e.Frame.ExecuteJavaScriptAsync(js);
        }
    }
}