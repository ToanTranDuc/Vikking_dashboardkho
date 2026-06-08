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
    public partial class frmBienBanMoKienExcel : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        string URL = string.Empty, selectedValuesMH = string.Empty;
        private HttpClientExtension _clientExtension;
        

        public frmBienBanMoKienExcel()
        {
            InitializeComponent();
          
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
        }

        private void CreateSearchLookupKhachHang()
        {
            try
            {
                searchLookUpEditMH.Properties.ValueMember = "MaKH";
                searchLookUpEditMH.Properties.DisplayMember = "TenKH";

                string urlKH = string.Format("{0}?", URL + "PhanTichBom/GetKH");
                string jsonKH = Task.Run(async () => { return await _clientExtension.GetAsnyc(urlKH); }).Result;
                DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(jsonKH);
                searchLookUpEditMH.Properties.DataSource = tblKH;
                if (tblKH != null && tblKH.Rows.Count > 0) searchLookUpEditMH.EditValue = tblKH.Rows[0]["MaKH"];
            }
            catch (Exception ex)
            {

            }
        }

        private void CreateSearchLookUpSoLo(int npl)
        {
            searchLookUpEditSoLo.Properties.DisplayMember = "SoLo";
            searchLookUpEditSoLo.Properties.ValueMember = "SoLoID";

            string url = string.Format("{0}?npl={1}", URL + "BienBanMoKien/GetSoLoExcel", npl);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
            searchLookUpEditSoLo.Properties.DataSource = tbl;
        }

        private void CreateSearchLookUpMaHang(int npl)
        {
            try
            {
                searchLookUpEditMH.Properties.ValueMember = "MaHang";
                searchLookUpEditMH.Properties.DisplayMember = "TenHang";
                string url = string.Format("{0}?solo={1}&&npl={2}", URL + "BienBanMoKien/GetMaHangExcel", searchLookUpEditSoLo.EditValue.ToString() == "" ? "" : searchLookUpEditSoLo.EditValue.ToString(), npl);
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);
                searchLookUpEditMH.Properties.DataSource = tbl;
            }
            catch (Exception ex)
            {

            }
        }

        private void searchLookUpEditSoLo_EditValueChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked == true)
                CreateSearchLookUpMaHang(1);
            else
                CreateSearchLookUpMaHang(0);
        }

        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            int npl = 2;
            if (searchLookUpEditSoLo.EditValue == null) return;
            if (radioButton1.Checked == false && radioButton2.Checked == false)
                return;
            if (radioButton1.Checked == true)
            {
                npl = 1;
                radioButton2.Checked = false;
            }    
               
            if (radioButton2.Checked == true)
            {
                npl = 0;
                radioButton1.Checked = false;
            }    
               
            string url = string.Format("{0}?solo={1}&&mahang={2}&&npl={3}", URL + "BienBanMoKien/GetExcel", searchLookUpEditSoLo.EditValue.ToString() == "" ? "" : searchLookUpEditSoLo.EditValue.ToString(),
            selectedValuesMH.ToString() == "" ? "" : selectedValuesMH.ToString(), npl);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]") return;
            DataTable tbl = JsonConvert.DeserializeObject<DataTable>(json);

        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void searchLookUpEditMH_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            string selectedValuessMH = string.Join("; ", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEditMH.Properties.DisplayMember)));
            if (string.IsNullOrEmpty(selectedValuessMH.ToString()))
            {
               
                    e.DisplayText = "---Chưa chọn Số lô---";
               
            }
            else
            {
                e.DisplayText = selectedValuessMH.ToString();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            CreateSearchLookUpSoLo(1);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

            CreateSearchLookUpSoLo(0);
        }

        private void searchLookUpEdit1View_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            selectedValuesMH = string.Join(";", searchLookUpEdit1View.GetSelectedRows().Select(rowHandle => searchLookUpEdit1View.GetRowCellValue(rowHandle, searchLookUpEditMH.Properties.ValueMember)));
            searchLookUpEditMH.EditValue = selectedValuesMH;
        }
    }
}
