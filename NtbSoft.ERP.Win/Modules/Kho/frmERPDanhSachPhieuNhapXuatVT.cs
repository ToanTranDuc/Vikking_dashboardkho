using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
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
    public partial class frmERPDanhSachPhieuNhapXuatVT : DevExpress.XtraEditors.XtraForm
    {
        string URL = string.Empty, _maDVVT = "";
        int _rowAdd = -1, FocusedIndex = 0;
        private HttpClientExtension _clientExtension;
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        DateTime _tuNgay = DateTime.Now, _toiNgay = DateTime.Now;
        public frmERPDanhSachPhieuNhapXuatVT()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            dtTuNgay.EditValue = _tuNgay;
            dtToiNgay.EditValue = _toiNgay;
        }

        private void LoadDSPhieuNhap()
        {
            string url = string.Format("{0}?action={1}&&para={2}&&para2={3}", URL + "KhoVatTu/GetKhoVatTu", "GetDSPN", _tuNgay.ToString("yyyy-MM-dd"), _toiNgay.ToString("yyyy-MM-dd"));
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            DataRow dr = tbl.AsEnumerable().Where(x => x["ID"].ToString() == "").FirstOrDefault();
            if (dr != null)
                tbl.Rows.Remove(dr);
            dgrDSPhieuNhap.DataSource = tbl;
            gridViewDSPhieuNhap.OptionsBehavior.AutoExpandAllGroups = true;
        }

        private void LoadDSPhieuXuat()
        {
            string url = string.Format("{0}?action={1}&&para={2}&&para2={3}", URL + "KhoVatTu/GetKhoVatTu", "GetDSPX", _tuNgay.ToString("yyyy-MM-dd"), _toiNgay.ToString("yyyy-MM-dd"));
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            DataRow dr = tbl.AsEnumerable().Where(x => x["ID"].ToString() == "").FirstOrDefault();
            if (dr != null)
                tbl.Rows.Remove(dr);
            dgrDSPhieuXuat.DataSource = tbl;
            gridViewDSPhieuXuat.OptionsBehavior.AutoExpandAllGroups = true;
        }

        private void DeletePhieuNhap()
        {

        }

        private void DeletePhieuXuat()
        {

        }

        private void LoadData()
        {
            if (xtraTabNhapXuat.SelectedTabPageIndex == 0)
                LoadDSPhieuNhap();
            else LoadDSPhieuXuat();
        }

        private void btCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void frmERPDanhSachPhieuNhapXuatVT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                //DeleteKhoVT();
            }
            else if (e.KeyCode == Keys.F5)
            {
                LoadData();
            }
        }

        private void gridViewDSPhieuNhap_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
        }

        private void gridViewDSPhieuXuat_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Column.FieldName == "STT" && e.IsGetData)
                e.Value = view.GetRowHandle(e.ListSourceRowIndex) + 1;
        }

        private void dtTuNgay_EditValueChanged(object sender, EventArgs e)
        {
            if(dtTuNgay.EditValue != null && dtToiNgay.EditValue != null)
            {
                _tuNgay = DateTime.Parse(dtTuNgay.EditValue.ToString());
                _toiNgay = DateTime.Parse(dtToiNgay.EditValue.ToString());
                LoadData();
            }    
        }

        private void xtraTabNhapXuat_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            LoadData();
        }

        private void dtToiNgay_EditValueChanged(object sender, EventArgs e)
        {
            if (dtTuNgay.EditValue != null && dtToiNgay.EditValue != null)
            {
                _tuNgay = DateTime.Parse(dtTuNgay.EditValue.ToString());
                _toiNgay = DateTime.Parse(dtToiNgay.EditValue.ToString());
                LoadData();
            }
        }

        private void xtraTabNhapXuat_Selected(object sender, DevExpress.XtraTab.TabPageEventArgs e)
        {
            
        }

        

        private void xtraTabNhapXuat_CustomHeaderButtonClick(object sender, DevExpress.XtraTab.ViewInfo.CustomHeaderButtonEventArgs e)
        {
            LoadData();
        }
    }
}
