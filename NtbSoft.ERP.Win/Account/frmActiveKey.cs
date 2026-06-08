using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using NtbSoft.ERP.Libs;
using System.Net.Http;
using Newtonsoft.Json;

namespace NtbSoft.ERP.Win.Account
{
    public partial class frmActiveKey : DevExpress.XtraEditors.XtraForm
    {
        NtbSoft.ERP.Libs.Security _objSecurity;
        System.Configuration.AppSettingsReader settingsReader =
                                            new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        string controler = "SystemLogged";
        public frmActiveKey()
        {
            InitializeComponent();
            _objSecurity = new Libs.Security();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
        }

        private async void btActive_Click(object sender, EventArgs e)
        {
            KeyActive keyObj = new KeyActive();
            keyObj.ID = "1";
            keyObj.Key = tbKeyActive.Text;
            string urlPost = URL + controler + "/SetKey";
            string mg = await CreatedKey(urlPost, keyObj);
            if (string.Compare(mg, "True") == 0)
                XtraMessageBox.Show("OK!", "Active", MessageBoxButtons.OK, MessageBoxIcon.Information);

            else
                XtraMessageBox.Show(mg, "Active", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private async void btGetKey_Click(object sender, EventArgs e)
        {
            string url = URL + controler + "/GetKey";
            string key = await GetKey(url);
            tbKeySource.Text = key;
        }
        public async Task<string> GetKey(string url)
        {

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseString = await client.GetAsync(url);
                if (responseString.IsSuccessStatusCode)
                {
                    string jsonString = responseString.Content.ReadAsStringAsync().Result.Replace("\\", "")
                                            .Trim(new char[1] { '"' });
                    if (!string.IsNullOrEmpty(jsonString))
                        return jsonString;
                    else
                        return string.Empty;
                }
                throw new Exception("Lỗi kết nối Server!");
            }

        }
        public async Task<string> CreatedKey(string url, KeyActive keyObj)
        {

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                string serializedObj = JsonConvert.SerializeObject(keyObj);

                HttpContent content = new StringContent(serializedObj, Encoding.UTF8, "application/json");
                HttpResponseMessage result = await client.PostAsync(url, content);
                if (result.IsSuccessStatusCode)
                    return result.Content.ReadAsStringAsync().Result.Replace("\\", "")
                                           .Trim(new char[1] { '"' });
                else
                    throw new Exception("Lỗi kết nối server!");

            }

        }

        private void tbKeyActive_EditValueChanged(object sender, EventArgs e)
        {
            if (tbKeyActive.Text == "") btActive.Enabled = false;
            btActive.Enabled = true;
        }

        private void btCopyKeySource_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(tbKeySource.Text);
        }

        private void btPasteKeyActive_Click(object sender, EventArgs e)
        {
            tbKeyActive.Text = "";
            string keyActive = Clipboard.GetText();
            if (keyActive != null)
                tbKeyActive.Text = keyActive;
        }
    }
    public class KeyActive
    {
        public string ID { get; set; }
        public string Key { get; set; }
    }
}