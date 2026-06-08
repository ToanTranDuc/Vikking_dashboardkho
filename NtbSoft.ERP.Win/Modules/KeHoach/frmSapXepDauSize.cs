using NtbSoft.ERP.Entity.KeHoach;
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
    public partial class frmSapXepDauSize : DevExpress.XtraEditors.XtraForm
    {

        DauSizeEntity drfocus = new DauSizeEntity();
        List<DauSizeEntity> lstData = new List<DauSizeEntity>();

        public frmSapXepDauSize()
        {
            InitializeComponent();
            loadData();
        }
        private void loadData()
        {
            lstData = frmERPKeHoachDongThungV4.lstDauSizeCheckN;
            grcDauSize.DataSource = lstData;
        }

        private void grvSize_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            drfocus = grvSize.GetFocusedRow() as DauSizeEntity;
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            var index = lstData.IndexOf(drfocus);
            if (index == 0) return;
            lstData.RemoveAt(index);
            lstData.Insert(index - 1,drfocus);
            grcDauSize.DataSource = lstData;
            grvSize.FocusedRowHandle = index - 1;
            grvSize.RefreshData();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var index = lstData.IndexOf(drfocus);
            if (index == lstData.Count - 1) return;
            lstData.RemoveAt(index);
            lstData.Insert(index + 1, drfocus);
            grcDauSize.DataSource = lstData;
            grvSize.FocusedRowHandle = index + 1;
            grvSize.RefreshData();
        }

        private void btSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmERPKeHoachDongThungV4.lstDauSizeCheckN = lstData;
            this.Close();
        }
    }
}
