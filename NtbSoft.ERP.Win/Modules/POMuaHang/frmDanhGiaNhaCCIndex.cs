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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmDanhGiaNhaCCIndex : DevExpress.XtraEditors.XtraForm
    {
        public frmDanhGiaNhaCCIndex()
        {
            InitializeComponent();
        }

        private void Them_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmDanhGiaNhaCungCap frm = new frmDanhGiaNhaCungCap("add");
            frm.ShowDialog();

        }
    }
}