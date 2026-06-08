using DevExpress.XtraEditors;
using Newtonsoft.Json;
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

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmCoppyMaHang : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private List<string> _lstSize;
        private HttpClientExtension _clientExtension;
        private HashSet<DataRow> selectedRowsSize = new HashSet<DataRow>();
        private List<string> _preselectedSizeIds = new List<string>();
        DataTable dtbdh;
        DataTable dtbctdh;
        public event Action<DataTable, DataTable> OnCopyConfirmed;
        public frmCoppyMaHang()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            dtbdh = new DataTable();
            dtbctdh = new DataTable();
        }


        protected override void OnLoad(EventArgs e) 
        {
            string urlKH = string.Format("{0}?", URL + "DonHangTong/GetKhachHang");
            string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
            DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
            searchLookUpEditKH.Properties.DataSource = tblKH;
            searchLookUpEditKH.Properties.ValueMember = "MaKH";
            searchLookUpEditKH.Properties.DisplayMember = "TenKH";
        }

        private void LoadDSDonHangTong()
        {
            string url = string.Format("{0}?makh={1}", URL + "DonHangTong/Getdhcoppy", searchLookUpEditKH.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtbdh = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit1.Properties.DataSource = dtbdh;
            searchLookUpEdit1.Properties.DisplayMember = "MaDH";
            searchLookUpEdit1.Properties.ValueMember = "MaDH";
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEdit1.EditValue == null)
                {
                    XtraMessageBox.Show("Vui lòng chọn mã đơn hàng cần sao chép!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string selectedMaDH = searchLookUpEdit1.EditValue.ToString();

                // Lọc ra dòng tương ứng trong dtbdh
                DataRow[] selectedRows = dtbdh.Select($"MaDH = '{selectedMaDH}'");

                DataTable selectedHeader = dtbdh.Clone(); // giữ cấu trúc
                foreach (var row in selectedRows)
                    selectedHeader.ImportRow(row);

                // Gửi selectedHeader và dtbctdh về form cha
                OnCopyConfirmed?.Invoke(selectedHeader, dtbctdh);

                this.Close();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void searchLookUpEdit2_Properties_EditValueChanged(object sender, EventArgs e)
        {
            LoadDSDonHangTong();
        }

        private void searchLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {
            string url = string.Format("{0}?madh={1}", URL + "DonHangTong/Getctdhcoppy", searchLookUpEdit1.EditValue.ToString());
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            dtbctdh = JsonConvert.DeserializeObject<DataTable>(json);
        }
    }
}