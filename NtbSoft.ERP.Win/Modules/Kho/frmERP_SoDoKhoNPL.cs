using CefSharp;
using CefSharp.WinForms;
using DevExpress.XtraEditors;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERP_SoDoKhoNPL : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                              new System.Configuration.AppSettingsReader();
        string URL = string.Empty;

        private ChromiumWebBrowser chromiumWebBrowser;
        private DataGridView dataGridView;
        private DataTable dt;
        public frmERP_SoDoKhoNPL()
        {
            URL = (string)settingsReader.GetValue("URL_SDK", typeof(String));
            InitializeComponent();
            InitializeChromiumBrowser();
            InitializeDataGridView();
            this.Load += frmSoDoKho;
            this.WindowState = FormWindowState.Maximized;
            InitializeComponent();
        }
        private void frmSoDoKho(object sender, EventArgs e)
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

            chromiumWebBrowser.FrameLoadEnd += ChromiumWebBrowser_FrameLoadEnd;
        }

        private void ChromiumWebBrowser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            // Chỉ chạy khi là main frame
            if (e.Frame.IsMain)
            {
                HideElementByIdKeepSpace("home");
            }
        }

        // CÁCH 2: Ẩn element bằng visibility: hidden (giữ nguyên không gian)
        private void HideElementByIdKeepSpace(string elementId)
        {
            string script = $@"
                    var el = document.getElementById('{elementId}');
                    if (el) {{
                        el.style.visibility = 'hidden';
                        'OK';
                    }} else {{
                        'Not Found';
                    }}
                ";
            chromiumWebBrowser.EvaluateScriptAsync(script);
        }

    }
}