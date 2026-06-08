using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win
{
    public partial class frmConfigConnection : XtraForm
    {
        public frmConfigConnection()
        {
            InitializeComponent();
        }

        private static string _strConnectionString = string.Empty;

        private void frmConfigConnection_Load(object sender, EventArgs e)
        {
            txtServerName.Text = _strConnectionString;

            SqlConnection connection = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void btnTestConnect_Click(object sender, EventArgs e)
        {

        }
    }
}
