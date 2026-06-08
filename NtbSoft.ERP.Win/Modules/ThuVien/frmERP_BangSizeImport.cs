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

namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERP_BangSizeImport : DevExpress.XtraEditors.XtraForm
    {
        List<string> _listSheet;
        public string SelectedSheet { get; private set; }
        public frmERP_BangSizeImport(List<string> listSheet)
        {
            InitializeComponent();
            this._listSheet = listSheet;
            ComboxSheet();
        }
        private void ComboxSheet()
        {
            comboBoxEdit1.Properties.Items.Clear();

            // Thêm danh sách sheet vào ComboBoxEdit
            comboBoxEdit1.Properties.Items.AddRange(_listSheet);

            // (Tùy chọn) chọn sẵn sheet đầu tiên
            comboBoxEdit1.SelectedIndex = 0;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            SelectedSheet = comboBoxEdit1.Text;

            // Đóng form và báo là OK
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}