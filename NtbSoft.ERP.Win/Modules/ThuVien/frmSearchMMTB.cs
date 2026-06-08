using DevExpress.XtraEditors;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmSearchMMTB : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        System.Configuration.AppSettingsReader settingsReader =
                                     new System.Configuration.AppSettingsReader();
        public event EventHandler<DataTable> OnDataUpdate;
        private HttpClientExtension _clientExtension;

        public frmSearchMMTB(int x, int y)
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

        private void frmSearchPMH_Load(object sender, EventArgs e)
        {
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            string valMMTB = txtMMTB.EditValue?.ToString().Trim() ?? "";
            string valLinhKien = txtLinhKien.EditValue?.ToString().Trim() ?? "";

            if (string.IsNullOrEmpty(valMMTB) && string.IsNullOrEmpty(valLinhKien))
            {
                XtraMessageBox.Show("Vui lòng nhập ít nhất một điều kiện tìm kiếm!",
                                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable tblResult = SearchMMTBLK(valMMTB, valLinhKien);
            OnDataUpdate?.Invoke(this, tblResult);
        }

        private DataTable SearchMMTBLK(string mmtb, string linhKien)
        {
            string url = $"{URL}NhaCC/Get?action=SearchMMTBLK&para={mmtb}&para1={linhKien}&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

            if (string.IsNullOrEmpty(json) || json == "[]")
                return null;

            return JsonConvert.DeserializeObject<DataTable>(json);
        }
    }
}