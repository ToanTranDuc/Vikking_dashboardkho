using DevExpress.XtraEditors;
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
using Newtonsoft.Json;
namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPNhapKhoNPL_KhoiLuong : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        string _dv = string.Empty, _mdv = string.Empty;

        public frmERPNhapKhoNPL_KhoiLuong(string mdv, string dv, string tileNW="", string tileGW="")
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _dv = dv;
            layoutControlItem1.Text = "Khối lượng(kg)/" + _dv;
            _mdv = mdv;
            if(!string.IsNullOrWhiteSpace(tileNW))
            {
                textEdit1.Text = tileNW;
            }
            if (!string.IsNullOrWhiteSpace(tileGW))
            {
                textEdit2.Text = tileGW;
            }

        }

        private void setTypeTxtEdit(object sender, KeyPressEventArgs e)
        {

            TextEdit textEdit = sender as TextEdit;
            string currentText = textEdit.Text;
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Ngăn không cho nhập ký tự không hợp lệ
                return;
            }
            if (currentText.Contains("."))
            {
                int indexOfDot = currentText.IndexOf('.');
                string decimalPart = currentText.Substring(indexOfDot + 1);
                if (decimalPart.Length >= 2 && e.KeyChar != '\b') // '\b' là phím Backspace
                {
                    e.Handled = true;
                    return;
                }
            }
            if (e.KeyChar == '.' && currentText.Contains("."))
            {
                e.Handled = true;
            }
        }

        private void textEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void textEdit2_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textEdit1.Text))
            {
                MessageBox.Show("Vui lòng nhập Khối lượng(kg)/" + _dv, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(textEdit2.Text))
            {
                MessageBox.Show("Vui lòng nhập Khối lượng bao bì", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
        }
        public List<decimal> GetKhoiLuong()
        {
            List<decimal> _lst = new List<decimal>();
            decimal _kl1 = decimal.TryParse(textEdit1.Text, out decimal temp1) ? temp1 : 0;
            decimal _kl2 = decimal.TryParse(textEdit2.Text, out decimal temp2) ? temp2 : 0;
            _lst.Add(_kl1);
            _lst.Add(_kl2);
            return _lst;
        }
    }
}