using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using Newtonsoft.Json;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Modules.Kho;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NtbSoft.ERP.Win.Modules.ThuVien
{
    public partial class frmERP_VatTuCopyTS : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        bool checkMau = false;
        string _makh = string.Empty, _tenkh = string.Empty, _mahang = string.Empty, _tenhang = string.Empty;

        public frmERP_VatTuCopyTS(string makh, string kh)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            _makh = makh;
            _tenkh = kh;
           
            textEdit1.Text = _tenkh;
          
            loadKH();
            this.ActiveControl = simpleButton1;
        }

      

       
      
        private void loadKH()
        {
            try
            {
               
                searchLookUpEditKH.Properties.ValueMember = "MaKH";
                searchLookUpEditKH.Properties.DisplayMember = "TenKH";
                string url = $"{URL}ERPVatTuBOM/GetChung?Action=GETKHCOPY&para={_makh}";
                string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                if (json == "[]") return;
                DataTable tblKH = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblKH == null || tblKH.Rows.Count == 0)
                    return;
                searchLookUpEditKH.Properties.DataSource = tblKH;

            }
            catch (Exception ex)
            {

            }
        }
       
        private void btnXacNhan_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchLookUpEditKH.EditValue == null || string.IsNullOrWhiteSpace(searchLookUpEditKH.EditValue.ToString()))
                {
                    XtraMessageBox.Show("Vui lòng chọn khách hàng copy.!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DialogResult result = XtraMessageBox.Show(
                    "Bạn có muốn cập nhật lại mã vật tư theo mã hàng mới hay không?",
                    "Xác nhận",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question
                );
                string para4 = string.Empty;
                string _makhcopy = searchLookUpEditKH.EditValue.ToString();
                if (result == DialogResult.Yes)
                {
                    para4 = "1";
                }
                else if (result == DialogResult.No)
                {
                    para4 = "0";
                }
                else 
                {
                    return;
                }

                string action = "POSTCOPYNOMAU";
                string url = $"{URL}ERPVatTuBOM/PostT1?Action={action}&para={_makh}&para2=&para3={_makhcopy}&para4={para4}";
                string msResult = Task.Run(async () => { return await _clientExtension.PostAsync(url, null); }).Result;
                if (msResult.ToLower() == "true") this.DialogResult = DialogResult.OK;

            }
            catch (Exception ex)
            {

            }
           
        }

    }
}