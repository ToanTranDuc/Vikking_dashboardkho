using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Newtonsoft.Json;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Utils;

namespace NtbSoft.ERP.Win.Modules.QuanLyDonHang
{
    public partial class frmXoaSize : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader = new System.Configuration.AppSettingsReader();
        SystemUserModuleService _serviceUserModule = new SystemUserModuleService();
        string URL = string.Empty;
        private HttpClientExtension _clientExtension;
        public string _madh = string.Empty;
        public frmXoaSize(string madh)
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _clientExtension = new HttpClientExtension();
            this._madh = madh;
        }
        protected override void OnLoad(EventArgs e)
        {
            loadSize();

        }
        private void loadSize()
        {
            string url = string.Format("{0}?madh={1}", URL + "DonHangTong/GetDeleteSize", _madh);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json == "[]")
            {
                gridControl1.DataSource = null;
                return;
            }
            DataTable tbld = JsonConvert.DeserializeObject<DataTable>(json);
            gridControl1.DataSource = tbld;
        }

        private void Xoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int _yes = 0;
            this.ActiveControl = button1;
            DataTable dt = gridControl1.DataSource as DataTable;
            if (dt == null) return;

            string _sizeid = string.Join(";",
                dt.AsEnumerable()
                  .Where(r => r.Field<bool>("Chon") == true)
                  .Select(r => r["SizeID"].ToString())
            );
            string url = string.Format("{0}?madh={1}&&sizeid={2}", URL + "DonHangTong/GetKTSize", _madh, _sizeid);
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            if (json != "[]")
            {
                DataTable tbld = JsonConvert.DeserializeObject<DataTable>(json);
              
                string size = string.Join(";",
                tbld.AsEnumerable()
                                   .Select(r => r["Size"].ToString()));

                DialogResult result = MessageBox.Show("SIZE " + size.ToString() + " ĐÃ LÊN CHUYỀN. BẠN CÓ MUỐN XÓA SIZE NÀY KHÔNG?", "THÔNG BÁO", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string urlV2 = string.Format("{0}/?madh={1}&&sizeid={2}&&username={3}", URL + "DonHangTong/DeleteSize", _madh, _sizeid, GlobleData.UserName);
                    string jsonV2 = Task.Run(async () => await _clientExtension.DeletedAsync(urlV2)).Result;
                    if (jsonV2.ToLower() != "true")
                    {
                        XtraMessageBox.Show(jsonV2);
                        return;
                    }

                    clsWaitForm.ShowSuccessForm(this, 3000);
                    loadSize();
                    return;

                }
                else
                    return;

            }
            string urlV1 = string.Format("{0}/?madh={1}&&sizeid={2}&&username={3}", URL + "DonHangTong/DeleteSize", _madh, _sizeid, GlobleData.UserName);
            string jsonV1 = Task.Run(async () => await _clientExtension.DeletedAsync(urlV1)).Result;
            if (jsonV1.ToLower() != "true")
            {
                XtraMessageBox.Show(jsonV1);
                return;
            }

            clsWaitForm.ShowSuccessForm(this, 3000);
            loadSize();
        }

        private void Naplai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            loadSize();
        }
    }

}
