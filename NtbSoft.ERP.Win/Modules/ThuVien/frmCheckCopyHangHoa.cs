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
    public partial class frmCheckCopyHangHoa : DevExpress.XtraEditors.XtraForm
    {
        public List<int> SelectedSteps { get; private set; } = new List<int>();
        public frmCheckCopyHangHoa()
        {
            InitializeComponent();
        }

        private void checkEdit5_CheckedChanged(object sender, EventArgs e)
        {
            if(Convert.ToBoolean(checkEdit5.EditValue) == true)
                CheckBoxItem(true);
            else
                CheckBoxItem(false);
            
        }
        private void CheckBoxItem(bool check)
        {
            checkEdit1.EditValue = check;
            checkEdit2.EditValue = check;
            checkEdit3.EditValue = check;
            checkEdit4.EditValue = check;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            SelectedSteps.Clear();

            if (Convert.ToBoolean(checkEdit1.EditValue))
                SelectedSteps.Add(1);

            if (Convert.ToBoolean(checkEdit2.EditValue))
                SelectedSteps.Add(2);

            if (Convert.ToBoolean(checkEdit3.EditValue))
                SelectedSteps.Add(3);

            if (Convert.ToBoolean(checkEdit4.EditValue))
                SelectedSteps.Add(4);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}