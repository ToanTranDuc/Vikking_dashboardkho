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

namespace NtbSoft.ERP.Win.Modules.POMuaHang
{
    public partial class frmNhapKetLuanMer : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;

        public string KetLuan = string.Empty;
        public bool result;
        string _MaPhieuMH = string.Empty;
        string _MaNPL = string.Empty;
        public frmNhapKetLuanMer(string MaPhieuMH,string MaNPL)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _MaPhieuMH = MaPhieuMH;
            _MaNPL = MaNPL;
            LoadQCMerCheck();
        }


        private void simpleButton1_Click(object sender, EventArgs e)
        {
          
            if (ckFail.Checked)
            {
                result = false;
            }
            else if (ckPass.Checked)
            {
                result = true;
            }
            else if(!ckPass.Checked && !ckFail.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtKetLuanMer.Text))
                {
                    XtraMessageBox.Show(
                        "Vui lòng kết luận cho vật tư.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
            }

           
            KetLuan = txtKetLuanMer.EditValue?.ToString();
            this.DialogResult = DialogResult.OK;
            ////this.Close();
        }

        private void LoadQCMerCheck()
        {
            try
            {

                txtKetLuanMer.EditValue = string.Empty;
                ckFail.Checked = false;
                ckPass.Checked = false;
                string url = string.Empty;
                DataTable tbl = new DataTable();
                url = $"{URL}ERPCanDoiNguyenPhuLieu/GET?action=GetMerDanhGia&para1={_MaPhieuMH}&para2={_MaNPL}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json != "[]")
                {
                    tbl = JsonConvert.DeserializeObject<DataTable>(json);
                }

                if(tbl!=null && tbl?.Rows?.Count > 0)
                {
                    bool.TryParse(tbl.Rows[0]["Result"]?.ToString(), out bool Result);
                    if (Result)
                    {
                        ckPass.Checked = true;
                        ckFail.Checked = false;
                    }
                    else if(!Result)
                    {
                        ckFail.Checked = true;
                        ckPass.Checked = false;
                    }
                    txtKetLuanMer.EditValue = tbl.Rows[0]["KetLuanMer"]?.ToString();
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void frmNhapKetLuanMer_Load(object sender, EventArgs e)
        {

        }
    }
}