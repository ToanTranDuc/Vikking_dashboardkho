using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Net.Http;
using Newtonsoft.Json;
using System.Configuration;
using NtbSoft.ERP.Win.Utils;
using NtbSoft.ERP.Entity.SYSTEM;
using NtbSoft.ERP.Win.Service.SYSTEM;
using NtbSoft.ERP.Win.Account;
using System.Threading.Tasks;
using NtbSoft.ERP.Win;

namespace NtbSoft.HRM.Win.Account
{
    public partial class frmLogin : DevExpress.XtraEditors.XtraForm
    {

        private bool isWrongPassword = false;
        internal string _User = string.Empty;
        MD5StringCrypt Md5Crypt;
        SystemUserService _serviceUser;
        HttpClientExtension _clientExtension = new HttpClientExtension();
        Security _objSecurity = new Security();
        System.Configuration.AppSettingsReader settingsReader =
                                               new System.Configuration.AppSettingsReader();
        string URL = string.Empty;

        public frmLogin()
        {

            InitializeComponent();

            this.Load += new EventHandler(frmLogin_Load);
            //this.FormClosed += new FormClosedEventHandler(frmLogin_FormClosed);
            SplashScreenManagerHelper.Paiter.ViewInfo.Stage = string.Empty;
            SplashScreenManagerHelper.Paiter.ViewInfo.Counter = 0;
            //DevExpress.XtraSplashScreen.SplashScreenManager.ShowImage(NtbSoft.ERP.Win.Properties.Resources.img1, true, true, SplashScreenManagerHelper.Paiter);
            this.FormClosing += new FormClosingEventHandler(frmLogin_FormClosing);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            Md5Crypt = new MD5StringCrypt();
            //_sysUser = new clsSystemUserMethod();
            URL = (string)settingsReader.GetValue("URL", typeof(String));
            _serviceUser = new SystemUserService();

            //this.btConfig.Visible = false;
        }

        void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isWrongPassword)
                e.Cancel = true;
        }
        void frmLogin_Load(object sender, EventArgs e)
        {
            this.AcceptButton = this.btLogin;
            this.CancelButton = this.btCancel;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //this.btConfig.Enabled = false;
            string _Path = _objSecurity.mRegKey + "\\" + _objSecurity.mSessionID.ToString("0000");
            object obj = _objSecurity.get_RegistryKey(_Path, "IsRememberQLDH");
            if (obj != null)
            {
                this.ceRememberMe.Checked = Convert.ToBoolean(obj);
                if (this.ceRememberMe.Checked)
                {
                    object objPass = _objSecurity.get_RegistryKey(_Path, "PasswordQLDH");
                    if (objPass != null)
                    {
                        string deCrypt = Md5Crypt.Decrypt(objPass.ToString(), true);
                        textEdit2.Text = deCrypt;
                    }
                }
            }

            object objUser = _objSecurity.get_RegistryKey(_Path, "UserQLDH");
            if (objUser != null)
                textEdit1.Text = objUser.ToString();
            this.AcceptButton = this.btLogin;
            this.CancelButton = this.btCancel;

        }
        //private void LoadTextured()
        //{
        //    SplashScreenManagerHelper.Paiter.ViewInfo.Stage = "Load dữ liệu...";
        //    for (int i = 1; i <= 100; i++)
        //    {
        //        System.Threading.Thread.Sleep(20);
        //        SplashScreenManagerHelper.Paiter.ViewInfo.Counter = i;
        //        DevExpress.XtraSplashScreen.SplashScreenManager.Default.Invalidate();
        //    }
        //}

        ////private void LoadFonts()
        ////{
        ////    SplashScreenManagerHelper.Paiter.ViewInfo.Stage = "Loading Fonts";
        ////    for (int i = 1; i <= 100; i++)
        ////    {
        ////        System.Threading.Thread.Sleep(20);
        ////        SplashScreenManagerHelper.Paiter.ViewInfo.Counter = i;
        ////        DevExpress.XtraSplashScreen.SplashScreenManager.Default.Invalidate();
        ////    }
        ////}

        private void BaseInitialize()
        {
            SplashScreenManagerHelper.Paiter.ViewInfo.Stage = "Khởi tạo giá trị ban đầu...";
            for (int i = 1; i <= 100; i++)
            {
                System.Threading.Thread.Sleep(20);
                SplashScreenManagerHelper.Paiter.ViewInfo.Counter = i;
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.Invalidate();
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (!GlobleData.IsUserLoggedIn)
            {
                this.Dispose();
                Application.ExitThread();
            }

        }

        private void LongInitializeComponent()
        {
            //BaseInitialize();
            //LoadFonts();
            //LoadTextured();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (GlobleData.IsUserLoggedIn)
            {
                this.Hide();
                LongInitializeComponent();
                //DevExpress.XtraSplashScreen.SplashScreenManager.HideImage();
            }

        }

        private async void btLogin_Click(object sender, EventArgs e)
        {
            string _UserName = textEdit1.Text.Trim();
            string _Password = textEdit2.Text.Trim(); //Admin
            //try  
            //{
            //DicDonHangService _serviceDonHang = new DicDonHangService();
            //string urls = URL + ResourceURL.UrlDicDonHang + "/Get";
            //List<DicDonHangEntity> _listData = Task.Run(async () => { return await _serviceDonHang.Get(urls); }).Result;

            string url = URL + ResourceURL.UrlUser + "/Get";
            List<SystemUserEntity> listUser = Task.Run(async () => { return await _serviceUser.UserGet(url); }).Result;

            SystemUserEntity objEntiAdmin = (from l in listUser
                                             where l.UserName == "admin"
                                             select l).FirstOrDefault();

            url = URL + "SystemUser/GetKhoUser";
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblKho = JsonConvert.DeserializeObject<DataTable>(json);




            if (objEntiAdmin == null)
            {
                XtraMessageBox.Show(NtbSoft.ERP.Win.Properties.Resources.GetDataError, NtbSoft.ERP.Win.Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (listUser.Count == 0) return;
            SystemUserEntity objUserLock = (from l in listUser
                                            where l.UserName == _UserName
                                            select l).FirstOrDefault();
            bool isLock = false;
            if (objUserLock == null)
            {
                XtraMessageBox.Show("User không hợp lệ!", NtbSoft.ERP.Win.Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (objUserLock != null)
                isLock = objUserLock.IsLock;
            //MessageBox.Show("GetAll");
            if (isLock)
            {
                XtraMessageBox.Show(NtbSoft.ERP.Win.Properties.Resources.AccountLock, NtbSoft.ERP.Win.Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return;
            }

            string password = objUserLock.Password;
            string deCrypt = Md5Crypt.Decrypt(password, true);

            if (_Password == deCrypt)
            {
                url = URL + $"SystemUserModule/GetKhoUser?action=GetKhoUser&userID={_UserName}";
                json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
                DataTable tblKhoUser = JsonConvert.DeserializeObject<DataTable>(json);
                if (tblKhoUser != null)
                    GlobleData.lstDVSX = tblKhoUser.AsEnumerable().Select(x => x["MaDVSX"].ToString()).ToList();
                GlobleData.IsUserLoggedIn = true;
                GlobleData.UserName = _UserName;
                GlobleData.MaKho = objUserLock.MaKho == null ? "" : objUserLock.MaKho;
                DataRow drCheck = tblKho.AsEnumerable().Where(x => x["MaKho"].ToString() == GlobleData.MaKho).FirstOrDefault();
                if (drCheck != null && drCheck["ParentMaKho"].ToString() == "") GlobleData.IsKhoTong = true;

                string _Path = _objSecurity.mRegKey + "\\" + _objSecurity.mSessionID.ToString("0000");
                _objSecurity.set_RegistryKey(_Path, "UserQLDH", _UserName);

                if (this.ceRememberMe.Checked)
                {
                    _objSecurity.set_RegistryKey(_Path, "IsRememberQLDH", 1);
                    _objSecurity.set_RegistryKey(_Path, "PasswordQLDH", password);
                }
                else
                {
                    _objSecurity.set_RegistryKey(_Path, "IsRememberQLDH", 0);
                    _objSecurity.detele_RegistryKey(_Path, "PasswordQLDH");
                }

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                isWrongPassword = false;



                SystemUserEntity row = (from l in listUser
                                        where l.UserName == _UserName
                                        select l).SingleOrDefault();
                if (row != null)
                {
                    SystemUserEntity sysUser = new SystemUserEntity();
                    sysUser.Active = row.Active;
                    sysUser.Descriptions = row.Descriptions;
                    sysUser.EmplyeeID = row.EmplyeeID;
                    sysUser.IsAdmin = row.IsAdmin;
                    sysUser.IsLeader = row.IsLeader;
                    sysUser.IsLock = row.IsLock;
                    sysUser.LastView = DateTime.Now;
                    sysUser.UserName = row.UserName;
                    sysUser.GroupID = row.GroupID;
                    sysUser.ViewCount = row.ViewCount + 1;
                    sysUser.Password = row.Password;
                    sysUser.AddDate = DateTime.Now;
                    sysUser.MaKho = row.MaKho;
                    string mg = Task.Run(async () => { return await _serviceUser.UpdateData(sysUser, URL + ResourceURL.UrlUser); }).Result;
                    if (mg != "True")
                    {
                        XtraMessageBox.Show("Cập nhập số lần đăng nhập thất bại!", NtbSoft.ERP.Win.Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    }
                }
            }
            else
            {
                isWrongPassword = true;
                XtraMessageBox.Show("Người dùng hoặc mật khẩu không đúng!", NtbSoft.ERP.Win.Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }

        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

        private void btConfig_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmActiveKey frm = new frmActiveKey();
            frm.ShowDialog();
            this.Show();
        }

        private void frmLogin_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            NtbSoft.ERP.Win.frmConfigConnection frm = new ERP.Win.frmConfigConnection();
            frm.ShowDialog();
            this.Show();
        }
        private async void LoadUserNV()
        {
            string url = string.Format("{0}?", URL + "PhanQuyenDH/GetUserNV");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblNV = JsonConvert.DeserializeObject<DataTable>(json);
        }
        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {
            string url = string.Format("{0}?", URL + "PhanQuyenDH/GetUserNV");
            string json = Task.Run(async () => { return await _clientExtension.GetAsnyc(url); }).Result;
            DataTable tblNV = JsonConvert.DeserializeObject<DataTable>(json);

            string _UserName = textEdit1.Text.Trim();
            string _Password = textEdit2.Text.Trim();

            DataRow row = tblNV.AsEnumerable()
           .FirstOrDefault(r => r["UserName"]?.ToString() == _UserName);

            if (row != null)
            {
                textEdit3.Text = row["TenNV"]?.ToString() +""+ row["Ten"]?.ToString();
                GlobleData.NhanVien = row["Ten"].ToString() == "" ? _UserName.ToString() : row["Ten"].ToString();
            }
            else
            {
                textEdit3.Text = string.Empty;
                GlobleData.NhanVien = _UserName.ToString();
            }
        }
    }
}