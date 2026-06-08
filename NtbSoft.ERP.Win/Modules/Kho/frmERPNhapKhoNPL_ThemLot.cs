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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmERPNhapKhoNPL_ThemLot : DevExpress.XtraEditors.XtraForm
    {
        private string _tukien = "0", _denkien = "0";
        private string _lot = "";
        private string _kienMin = "0", _kienMax = "0";
        public frmERPNhapKhoNPL_ThemLot()
        {
            _tukien = "0";
            _denkien = "0";
            _lot = "";
            _kienMin = "0";
            _kienMax = "0";
            InitializeComponent();
        }
        public frmERPNhapKhoNPL_ThemLot(string kienMin, string kienMax)
        {
            _tukien = "0";
            _denkien = "0";
            _lot = "";
            _kienMin = kienMin;
            _kienMax = kienMax;
            InitializeComponent();
        }

        private void Luu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                _tukien = txtTuKien.Text;
                _denkien = txtDenKien.Text;
                _lot = txtLOT.Text;
                if (_tukien == "0")
                {
                    MessageBox.Show("Vui lòng nhập Từ kiện lớn hơn 0", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_denkien == "0")
                {
                    MessageBox.Show("Vui lòng nhập Đến kiện  lớn hơn 0.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Convert.ToDouble(_tukien) >= Convert.ToDouble(_denkien))
                {
                    MessageBox.Show("Vui lòng nhập Từ kiện bé hơn Đến kiện.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_lot == "")
                {
                    MessageBox.Show("Vui lòng nhập LOT.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if(Convert.ToDouble(_tukien) < Convert.ToDouble(_kienMin))
                {
                    MessageBox.Show($"Vui lòng nhập Từ kiện lớn hơn {_kienMin}.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Convert.ToDouble(_denkien) > Convert.ToDouble(_kienMax))
                {
                    MessageBox.Show($"Vui lòng nhập Đến kiện bé hơn {_kienMax}.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Convert.ToDouble(_tukien) >= Convert.ToDouble(_kienMax))
                {
                    MessageBox.Show($"Vui lòng nhập Từ kiện bé hơn  {_kienMax}.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (Convert.ToDouble(_denkien) <= Convert.ToDouble(_kienMin))
                {
                    MessageBox.Show($"Vui lòng nhập Đến kiện lớn hơn {_kienMin}.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {

                
            }
            
        }

        private void txtTuKien_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
        }

        private void txtDenKien_KeyPress(object sender, KeyPressEventArgs e)
        {
            setTypeTxtEdit(sender, e);
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
        public List<string> GetValues()
        {
            List<string> _lst = new List<string>();
            _lst.Add(_tukien);
            _lst.Add(_denkien);
            _lst.Add(_lot);
            return _lst;
        }
    }
}