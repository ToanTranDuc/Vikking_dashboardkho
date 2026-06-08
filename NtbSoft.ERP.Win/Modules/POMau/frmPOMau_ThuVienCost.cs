using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Modules.QuanLyDonHang;
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

namespace NtbSoft.ERP.Win.Modules.Kho
{
    public partial class frmPOMau_ThuVienCost : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        private string _tenTau="";
        public frmPOMau_ThuVienCost()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
         
        }
        protected override void OnLoad(EventArgs e)
        {
            CreateSearchLookupKH();
            base.OnLoad(e);
        }
        private void CreateSearchLookupKH()
        {
            try
            {
                string urlKH = string.Format("{0}?", URL + "PhanTichBom/GetKH");
                string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
                DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
                searchLookUpEditKH.Properties.DataSource = tblKH;
                searchLookUpEditKH.Properties.ValueMember = "MaKH";
                searchLookUpEditKH.Properties.DisplayMember = "TenKH";

            }
            catch (Exception ex)
            {

            }
        }
        private void LoadTVCosting(string maKH, string maHang)
        {
            try
            {
                string url = $"{URL}POMau/Get?action=GetTVCosting&para1={maKH}&para2={maHang}";
                string json = Task.Run(async () => await _clientExtension.GetAsnyc(url)).Result;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

                if (tbl == null || tbl.Rows.Count == 0) return;

                DataRow row = tbl.Rows[0];
                TotalCPSPHP.Text = row["TotalCPSPHP"] == DBNull.Value ? "" : row["TotalCPSPHP"].ToString();
                TotalHCost.Text = row["TotalHCost"] == DBNull.Value ? "" : row["TotalHCost"].ToString();
                ManuCost.Text = row["ManuCost"] == DBNull.Value ? "" : row["ManuCost"].ToString();
                ProfitfCM.Text = row["ProfitfCM"] == DBNull.Value ? "" : row["ProfitfCM"].ToString();
                CostSpeTool.Text = row["CostSpeTool"] == DBNull.Value ? "" : row["CostSpeTool"].ToString();
                IECost.Text = row["IECost"] == DBNull.Value ? "" : row["IECost"].ToString();
                CostGMSP.Text = row["CostGMSP"] == DBNull.Value ? "" : row["CostGMSP"].ToString();
                Cost3P.Text = row["Cost3P"] == DBNull.Value ? "" : row["Cost3P"].ToString();
            }
            catch (Exception ex) { }
        }
        private void btnLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(searchLookUpEditKH.EditValue == null)
            {
                XtraMessageBox.Show("Vui lòng chọn khách hàng và mã hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (searchLookUpEditMH.EditValue == null)
            {
                XtraMessageBox.Show("Vui lòng chọn mã hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataTable dtSave = new DataTable();
            dtSave.Columns.Add("MaKH", typeof(string));
            dtSave.Columns.Add("MaHang", typeof(string));
            dtSave.Columns.Add("TotalCPSPHP", typeof(float));
            dtSave.Columns.Add("TotalHCost", typeof(float));
            dtSave.Columns.Add("ManuCost", typeof(float));
            dtSave.Columns.Add("ProfitfCM", typeof(float));
            dtSave.Columns.Add("CostSpeTool", typeof(float));
            dtSave.Columns.Add("IECost", typeof(float));
            dtSave.Columns.Add("CostGMSP", typeof(float));
            dtSave.Columns.Add("Cost3P", typeof(float));

            DataRow r = dtSave.NewRow();
            r["MaKH"] = searchLookUpEditKH.EditValue.ToString();
            r["MaHang"] = searchLookUpEditMH.EditValue.ToString();
            r["TotalCPSPHP"] = ParseFloat(TotalCPSPHP.Text);
            r["TotalHCost"] = ParseFloat(TotalHCost.Text);
            r["ManuCost"] = ParseFloat(ManuCost.Text);
            r["ProfitfCM"] = ParseFloat(ProfitfCM.Text);
            r["CostSpeTool"] = ParseFloat(CostSpeTool.Text);
            r["IECost"] = ParseFloat(IECost.Text);
            r["CostGMSP"] = ParseFloat(CostGMSP.Text);
            r["Cost3P"] = ParseFloat(Cost3P.Text);
            dtSave.Rows.Add(r);
            try
            {
                string url = $"{URL}POMau/PostTV?Action=PostTVCosting&para1={GlobleData.UserName}";
                string result = Task.Run(async () => await _clientExtension.PostAsync(url, dtSave)).Result;
                if (result.ToLower() != "true")
                {
                    return;
                }
                clsWaitForm.ShowSuccessForm(this, 2000);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private object ParseFloat(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            return float.TryParse(text.Trim(), out float val) ? (object)val : 0;
        }

        private void searchLookUpEditMH_EditValueChanged(object sender, EventArgs e)
        {
            if (searchLookUpEditKH.EditValue == null || searchLookUpEditMH.EditValue == null) return;
            TotalCPSPHP.Text = "";
            TotalHCost.Text = "";
            ManuCost.Text = "";
            ProfitfCM.Text = "";
            CostSpeTool.Text = "";
            IECost.Text = "";
            CostGMSP.Text = "";
            Cost3P.Text = "";
            LoadTVCosting(searchLookUpEditKH.EditValue.ToString(), searchLookUpEditMH.EditValue.ToString());
        }

        private void searchLookUpEditKH_EditValueChanged(object sender, EventArgs e)
        {
            TotalCPSPHP.Text = "";
            TotalHCost.Text = "";
            ManuCost.Text = "";
            ProfitfCM.Text = "";
            CostSpeTool.Text = "";
            IECost.Text = "";
            CostGMSP.Text = "";
            Cost3P.Text = "";
            try
            {
                string urlMH = string.Format("{0}?makh={1}", URL + "PhanTichBom/GetMH", searchLookUpEditKH.EditValue == null ? "" : searchLookUpEditKH.EditValue.ToString());
                string jsonMH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlMH); }).Result;
                DataTable tblMH = JsonConvert.DeserializeObject<DataTable>(jsonMH);
                searchLookUpEditMH.Properties.DataSource = tblMH;
                searchLookUpEditMH.Properties.ValueMember = "MaHang";
                searchLookUpEditMH.Properties.DisplayMember = "TenHang";
                if (tblMH.Rows.Count == 1)
                {
                    searchLookUpEditMH.EditValue = tblMH.Rows[0]["MaHang"];
                }
                else
                    searchLookUpEditMH.EditValue = null;


            }
            catch (Exception ex)
            {

            }
        }

        private void TotalCPSPHP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && TotalCPSPHP.Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void TotalHCost_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && TotalHCost.Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void ManuCost_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && ManuCost.Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void ProfitfCM_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && ProfitfCM.Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void CostSpeTool_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && CostSpeTool.Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void IECost_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && IECost.Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void CostGMSP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && CostGMSP.Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void Cost3P_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' && Cost3P.Text.Contains('.'))
            {
                e.Handled = true;
            }
        }
    }
}
