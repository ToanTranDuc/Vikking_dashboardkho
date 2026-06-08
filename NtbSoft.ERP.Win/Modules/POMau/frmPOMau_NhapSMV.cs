using DevExpress.XtraEditors;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.POMau
{
    public partial class frmPOMau_NhapSMV : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty;
        System.Configuration.AppSettingsReader settingsReader =
                                     new System.Configuration.AppSettingsReader();
        private HttpClientExtension _clientExtension;
        private string _maPhieu;
        public decimal SMVValue { get; private set; } = 0;
        public frmPOMau_NhapSMV(string maPhieu, decimal curSMV)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            SMVValue = curSMV;
            _maPhieu = maPhieu;
            txtEditSMV.EditValue = curSMV.ToString();
        }

        private void btnXNSMV_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEditSMV.Text))
            {
                XtraMessageBox.Show("Vui lòng nhập giá trị SMV!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtEditSMV.Text.Trim(), out decimal smv))
            {
                XtraMessageBox.Show("SMV phải là số hợp lệ!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string url = $"{URL}POMau/Get?action=PostSMV&para1={_maPhieu}&para2={smv.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
                string result = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;

                SMVValue = smv;
                this.DialogResult = DialogResult.OK;
                this.Close();
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi lưu SMV: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtEditSMV_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && txtEditSMV.Text.Contains('.'))
            {
                e.Handled = true;
            }
        }
    }
}
