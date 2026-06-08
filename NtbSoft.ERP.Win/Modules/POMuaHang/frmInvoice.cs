using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmInvoice : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private ResourceURL.EventStatus _status;
        int _rowAdd = -1;
        List<int> lstRowUpdate = new List<int>();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        bool _allowAdd = false, _allowEdit = false, _allowDelete = false;
        bool indicatorIcon = true;
        int FocusedIndex = 0;
        KeyDownControlHandler keyDownControlHandler;
        ActionControl actionControlAdd;
        ActionControl actionControlEdit;
        ActionControl actionControlDelete;
        ActionControl actionControlSave;
        ActionControl actionControlRefresh;
        List<ActionControl> lstActionControls;
        Helper helper = new Helper();
        DataTable tbl = new DataTable();
        public DataTable _tbl = new DataTable();
        public List<DataRow> lstSelect = new List<DataRow>();
        public DataTable ListExistingCP = new DataTable();
        public frmInvoice()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _serviceUserModule = new SystemUserModuleService();
        }

        protected override void OnLoad(EventArgs e)
        {
            CreateTableThuVien();
            LoadData();
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (gridControl1.DataSource != null)
                {
                    tbl = gridControl1.DataSource as DataTable;
                }
                this.ActiveControl = simpleButton1;
                DataRow dr = tbl.NewRow();
                tbl.Rows.InsertAt(dr, 0);
                gridControl1.DataSource = tbl;
            }

            catch (Exception ex)
            {


            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ActiveControl = simpleButton1;
            if (tbl.Rows.Count > 0 && tbl != null)
            {

                //foreach (DataRow row in tbl.Rows)
                //{
                //    if (row["ChiPhi"] == DBNull.Value || string.IsNullOrWhiteSpace(row["ChiPhi"].ToString()))
                //    {
                //        int rowIndex = tbl.Rows.IndexOf(row) + 1;
                //        XtraMessageBox.Show($"Có dòng bị thiếu thông tin Chi phí!. Vui lòng kiểm tra lại",
                //                            "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //        return;
                //    }
                //}

                string url = string.Format("{0}?", URL + "NhaCC/PostInvoice");
                string mss = Task.Run(async () => { return await _clientExtension.PostAsync(url, tbl); }).Result;
                if (mss.ToLower() != "true")
                    XtraMessageBox.Show("Lỗi Lưu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    clsWaitForm.ShowSuccessForm(this, 3000);
                LoadData();
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XoaDong();
        }

        private async void XoaDong()
        {
            try
            {
                DialogResult messResult = MessageBox.Show("Bạn có muốn xóa không? ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (messResult == DialogResult.Yes)
                {

                    DataRow dr = gridView1.GetFocusedDataRow();
                    if (dr == null) return;
                    int id = Convert.ToInt32(dr["ID"].ToString());
                    //if (id >= 1 && id <= 6)
                    //{
                    //    XtraMessageBox.Show(
                    //        "Chi phí gốc không được phép xóa.",
                    //        "Thông báo",
                    //        MessageBoxButtons.OK,
                    //        MessageBoxIcon.Warning);
                    //    return;
                    //}
                    ////string macl = dr["MaLoaiNCC"].ToString();
                    //string urlcheckpmh = $"{URL}NhaCC/GetCP?action=GETCPCKPMH&para1={id}&para2=&para3=&para4=&para5=";
                    //string jsoncheckpmh = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheckpmh); }).Result;
                    //DataTable tblcheck = JsonConvert.DeserializeObject<DataTable>(jsoncheckpmh);

                    //if (tblcheck != null && tblcheck.Rows.Count > 0)
                    //{
                    //    XtraMessageBox.Show("Chi phí này đã có trong phiếu mua hàng. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return;
                    //}

                    //string urlcheckpbg = $"{URL}NhaCC/GetCP?action=GETCPCKPBG&para1={id}&para2=&para3=&para4=&para5=";
                    //string jsoncheckpbg = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlcheckpbg); }).Result;
                    //DataTable tblcheckpbg = JsonConvert.DeserializeObject<DataTable>(jsoncheckpbg);

                    //if (tblcheckpbg != null && tblcheckpbg.Rows.Count > 0)
                    //{
                    //    XtraMessageBox.Show("Chi phí này đã được gán cho phiếu báo giá. Không thể xóa.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return;
                    //}

                    string url = string.Format("{0}?Parameter={1}", URL + "NhaCC/DeletecP", id);
                    string result = Task.Run(async () => { return await _clientExtension.DeletedAsync(url); }).Result;
                    if (result.ToLower() == "true")
                        LoadData();
                    else XtraMessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string url = $"{URL}NhaCC/GetInvoice?action=GETINVOICE&para=&para1=&para2=&para3=&para4=&para5=";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            tbl = JsonConvert.DeserializeObject<DataTable>(json);
            if (tbl != null && tbl.Rows.Count > 0)
            {
                gridControl1.DataSource = tbl;
            }
            else
            {
                CreateTableThuVien();
                gridControl1.DataSource = null;
            }

        }

        private void CreateTableThuVien()
        {
            tbl = new DataTable("tbl");
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("CompanyID", typeof(string));
            tbl.Columns.Add("Company", typeof(string));
            tbl.Columns.Add("Address", typeof(string));
            tbl.Columns.Add("TAX", typeof(string));
            tbl.Columns.Add("VAT", typeof(string));
            tbl.Columns.Add("GhiChu", typeof(string));
        }
    }
}