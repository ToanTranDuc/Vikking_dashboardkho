using DevExpress.XtraEditors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmSearchChiaChuyen : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        System.Configuration.AppSettingsReader settingsReader =
                                     new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        public DataTable ResultTable { get; private set; } = null;
        public string maHang { get; private set; } = string.Empty;
        public string maLenh { get; private set; } = string.Empty;
        public string chungLoai { get; private set; } = string.Empty;
        public string khachHang { get; private set; } = string.Empty;
        public string dot { get; private set; } = string.Empty;
        public string po { get; private set; } = string.Empty;
        public frmSearchChiaChuyen(int x, int y)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            SetPosition(x, y);
            _clientExtension = new HttpClientExtension();
        }
        private void SetPosition(int x, int y)
        {
            this.Location = new Point(x, y);
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            maHang = (textEdit1.EditValue != null) ? textEdit1.EditValue.ToString() : "";
            maLenh = (txtLenhSX.EditValue != null) ? txtLenhSX.EditValue.ToString() : "";
            chungLoai = (textEdit3.EditValue != null) ? textEdit3.EditValue.ToString() : "";
            khachHang = (textEdit2.EditValue != null) ? textEdit2.EditValue.ToString() : "";
            dot = (textEdit5.EditValue != null) ? textEdit5.EditValue.ToString() : "";
            po = (txtPO.EditValue != null) ? txtPO.EditValue.ToString() : "";

            DataTable tbl = SearchDonHang();

            if (tbl == null || tbl.Rows.Count == 0)
            {
                XtraMessageBox.Show("Không tìm thấy dữ liệu", "Thông báo");
                return;
            }

            ResultTable = tbl;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private DataTable SearchDonHang()
        {
            string url = $"{URL}ERPDonHangTong/Get?action=GetLenhChiaSX&para={maHang}&para2={maLenh}&para3={chungLoai}&para4={khachHang}&para5={dot}&para6={po}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json); 
            return tbl;
        }
    }
}
