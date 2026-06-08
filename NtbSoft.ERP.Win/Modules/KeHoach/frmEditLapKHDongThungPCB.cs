using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Modules.KeHoach
{
    public partial class frmEditLapKHDongThungPCB : DevExpress.XtraEditors.XtraForm
    {
        public static DataTable dtEdit = new DataTable();
        public frmEditLapKHDongThungPCB(DataTable dt)
        {
            InitializeComponent();
            dtEdit = dt;
            BindingData();
        }
        private void BindingData()
        {
            grcDetail.DataSource = dtEdit;
        }
    }
}
