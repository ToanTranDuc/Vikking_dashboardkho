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
    public partial class frmERPNhapKhoNPL_DVCD : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        string _dv = string.Empty,_mdv=string.Empty;
        public frmERPNhapKhoNPL_DVCD()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }
        public frmERPNhapKhoNPL_DVCD(string mdv,string dv)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _dv = dv;
            textEdit1.Text = _dv;
            _mdv = mdv;


            loadDonVi();
        }
        private void loadDonVi()
        {
            searchLookUpEdit1.Properties.DisplayMember = "TenDVVT";
            searchLookUpEdit1.Properties.ValueMember = "MaDVVT";
            string url = $"{URL}ERPNhapKhoNPL/GET?Action=GETDVCD&para={_dv}";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;


            if(json =="[]")
            {
                searchLookUpEdit1.EditValue = null;
                return; 
            }    
            DataTable tbl= JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEdit1.Properties.DataSource = tbl;

            
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        public List<string> GetDVCD() //_lst[0] là mã đơn vị, _lst[1] là tên đơn vị
        {
            List<string> _lst = new List<string>();
            if (searchLookUpEdit1.EditValue == null||string.IsNullOrWhiteSpace(searchLookUpEdit1.EditValue.ToString()))
            {
                _lst.Add(_mdv);
                _lst.Add(_dv);
                return _lst;
            }
            _lst.Add(searchLookUpEdit1.EditValue.ToString());
            _lst.Add(searchLookUpEdit1.Text.ToString());
            return _lst;
        }
    }
}