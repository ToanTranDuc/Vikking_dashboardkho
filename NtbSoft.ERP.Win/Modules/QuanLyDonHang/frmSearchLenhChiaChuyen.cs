using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
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
    public partial class frmSearchLenhChiaChuyen : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        public DataTable ResultTable { get; private set; } = null;
        public string ResultMaLenh { get; private set; } = string.Empty;
        public frmSearchLenhChiaChuyen(int x, int y)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            SetPosition(x, y);
            txtMaLenh.Text = string.Empty;
            txtMaLenh.Focus();
        }
        private void SetPosition(int x, int y)
        {
            this.Location = new Point(x,y);
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string maLenhNhap = txtMaLenh.Text.Trim().ToString();
            if (string.IsNullOrWhiteSpace(maLenhNhap))
            {
                XtraMessageBox.Show("Vui lòng nhập lệnh cần tìm!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaLenh.Focus();
                return;
            }
            string url = $"{URL}ERPDonHangTong/Get?action=GetLenhChiaSX&para=all&para2={maLenhNhap}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl == null || tbl.Rows.Count == 0)
            {
                XtraMessageBox.Show($"Không tìm thấy lệnh '{maLenhNhap}'.",
                "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaLenh.Focus();
                txtMaLenh.SelectAll();
                return;
            }
            DataTable filtered = tbl.AsEnumerable()
            .Where(row =>
            {
                string ml = row["MaLenh"]?.ToString()?.Trim().ToUpper();
                string mlsx = row["MaLenhSanXuat"]?.ToString()?.Trim().ToUpper();
                return ml == maLenhNhap || mlsx == maLenhNhap;
            })
            .CopyToDataTable();
            ResultMaLenh = filtered.Rows[0]["MaLenh"]?.ToString() ?? "";
            ResultTable = filtered;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtMaLenh.Text = "";
            this.Close();
        }

        private void txtMaLenh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLower(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
            }
        }
    }
}