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
using System.Net.NetworkInformation;
using NtbSoft.ERP.Libs;
using System.Net.Http;
using Newtonsoft.Json;

namespace NtbSoft.ERP.Log
{
    public partial class frmGetRegisterKey : DevExpress.XtraEditors.XtraForm
    {
        System.Configuration.AppSettingsReader settingsReader =
                                             new System.Configuration.AppSettingsReader();
        string URL = string.Empty;
        string controler = "SystemLogged";
        public frmGetRegisterKey()
        {
            InitializeComponent();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //LoadData();
        }

        private void LoadData()
        {
            string macAddr =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            string rendomStr = CryptorEnginePro.GetRendomValue();
            string keySend = CryptorEnginePro.Encrypt(rendomStr + macAddr).Replace("/","_");
            //tbKeySend.Text = keySend;
        }
        //public async Task<string> CreatedKey(string url,KeyActive keyObj)
        //{
           
        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(url);
        //        string serializedObj = JsonConvert.SerializeObject(keyObj);

        //        HttpContent content = new StringContent(serializedObj, Encoding.UTF8, "application/json");
        //        HttpResponseMessage result = await client.PostAsync(url, content);
        //        if (result.IsSuccessStatusCode)
        //            return result.Content.ReadAsStringAsync().Result.Replace("\\", "")
        //                                   .Trim(new char[1] { '"' });
        //        else
        //            throw new Exception("Lỗi kết nối server!");
                
        //    }
            
        //}
        //public async Task<string> GetKey(string url)
        //{

        //    using (HttpClient client = new HttpClient())
        //    {
        //        HttpResponseMessage responseString = await client.GetAsync(url);
        //        if (responseString.IsSuccessStatusCode)
        //        {
        //            string jsonString = responseString.Content.ReadAsStringAsync().Result.Replace("\\", "")
        //                                    .Trim(new char[1] { '"' });
        //            if (!string.IsNullOrEmpty(jsonString))
        //                return jsonString;
        //            else
        //                return string.Empty;
        //        }
        //        throw new Exception("Lỗi kết nối Server!");
        //    }

        //}
        private void btGetkey_Click(object sender, EventArgs e)
        {
            //string url = URL + controler + "/GetKey";
            //string key = await GetKey(url);
            //tbKeyActive.Text = key;

            string keyDecrypt = CryptorEnginePro.Decrypt(tbKeySource.Text);
            string realKey = keyDecrypt.Remove(0, 20);//.Remove(keyDecrypt.Length-7,6);
            string rKey = realKey.Remove(realKey.Length - 6, 6);
            string randomCode = CryptorEnginePro.GetRendomValue();
            string keyEncrypt = CryptorEnginePro.Encrypt(randomCode + rKey +"Client");
            tbKeyActive.Text = keyEncrypt;
        }

        private void btActive_Click(object sender, EventArgs e)
        {
          
            //KeyActive keyObj = new KeyActive();
            //keyObj.ID = "1";
            //keyObj.Key = keyEncrypt;
            //string urlPost = URL + controler + "/SetKey";
            //string mg = await CreatedKey(urlPost, keyObj);
            //if (string.Compare(mg, "True") == 0)
            //    XtraMessageBox.Show("OK!","Active",MessageBoxButtons.OK,MessageBoxIcon.Information);
            //else
            //    XtraMessageBox.Show(mg,"Active",MessageBoxButtons.OK,MessageBoxIcon.Error);

        }

      

      
    }
    public class KeyActive
    {
        public string ID { get; set; }
        public string Key { get; set; }
    }
}