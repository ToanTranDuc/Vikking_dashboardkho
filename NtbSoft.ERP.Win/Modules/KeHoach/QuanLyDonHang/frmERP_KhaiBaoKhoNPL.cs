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
    public partial class frmERP_KhaiBaoKhoNPL : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                            new System.Configuration.AppSettingsReader();
        string URL = string.Empty;

        private ChromiumWebBrowser chromiumWebBrowser;
        private DataGridView dataGridView;
        private DataTable dt;
        public frmERP_KhaiBaoKhoNPL()
        {
            URL = (string)settingsReader.GetValue("URL_InItKhoNPL", typeof(String));
            InitializeComponent();
            InitializeChromiumBrowser();
            InitializeDataGridView();
            this.Load += frmKhaiBaoKhoNPL;
            this.WindowState = FormWindowState.Maximized;
            InitializeComponent();
        }
        private void frmKhaiBaoKhoNPL(object sender, EventArgs e)
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
            //chromiumWebBrowser.DownloadHandler = new DownloadHandler();
        }
    }
}