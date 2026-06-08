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
using NtbSoft.ERP.Libs;
using System.Configuration;
using NtbSoft.ERP.Win.Utils;
using NtbSoft.ERP.Entity.SYSTEM;
using System.Threading.Tasks;
using NtbSoft.ERP.Win.Service.SYSTEM;

namespace NtbSoft.HRM.Win.Account
{
    public partial class frmChangePassword : DevExpress.XtraEditors.XtraForm
    {
        public string _userID = string.Empty;
        MD5Password _md5Pass;
        //clsSystemUserMethod _sysUser;
        System.Configuration.AppSettingsReader settingsReader =
                                               new AppSettingsReader();
        string URL = string.Empty;
        SystemUserService _serviceUser;
        public frmChangePassword()
        {
            InitializeComponent();
            _md5Pass = new MD5Password();
            //_sysUser = new clsSystemUserMethod();
            _serviceUser = new SystemUserService();

            URL = (string)settingsReader.GetValue("URL", typeof(String));
        }

        private void btSave_Click(object sender, EventArgs e)
        {

            //List<clsSystemUserEntity> userTable = GetAll(NtbSoft.ERP.Libs.Action.GETALL.ToString());
            //string password = (from tb in userTable
            //                   where tb.UserID == _userID
            //                   select tb.Password).First<string>();
            string url = URL + ResourceURL.UrlUser + "/Get";
            List<SystemUserEntity> listUser = Task.Run(async () => { return await _serviceUser.UserGet(url); }).Result;
            SystemUserEntity objCurrentUser = (from l in listUser
                                        where l.UserName == GlobleData.UserName
                                        select l).FirstOrDefault();
            if(objCurrentUser==null)return;
            string decryptPass = _md5Pass.Decrypt(objCurrentUser.Password, true);
            if (decryptPass != tbCurrentPass.Text)
            {
                tbCurrentPass.Focus();
                tbCurrentPass.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
                tbCurrentPass.ErrorText = NtbSoft.ERP.Win.Properties.Resources.WrongPassword;
            }
            else
            {
                if (string.Compare(tbNewPass.Text, tbConfirmPass.Text) == 0)
                {
                    SystemUserEntity sysUser = new SystemUserEntity();
                    sysUser.Descriptions = objCurrentUser.Descriptions;
                    sysUser.EmplyeeID = objCurrentUser.EmplyeeID;
                    sysUser.IsAdmin = objCurrentUser.IsAdmin;
                    sysUser.IsLock = objCurrentUser.IsLock;
                    sysUser.LastView = objCurrentUser.LastView;
                    sysUser.AddDate = objCurrentUser.AddDate;
                    sysUser.Active = objCurrentUser.Active;
                    sysUser.GroupID = objCurrentUser.GroupID;
                    sysUser.IsLeader = objCurrentUser.IsLeader;
                    sysUser.ViewCount = objCurrentUser.ViewCount;

                    NtbSoft.ERP.Libs.MD5Password md5Pass = new ERP.Libs.MD5Password();
                    string newPass = md5Pass.Encrypt(tbNewPass.Text, true);
                    sysUser.Password = newPass;
                    sysUser.UserName = objCurrentUser.UserName;
                    string ms = Task.Run(async () => { return await _serviceUser.UpdateData(sysUser, URL + ResourceURL.UrlUser); }).Result;
                    if (string.Compare(ms, "True") != 0)
                    {
                        XtraMessageBox.Show(ms, NtbSoft.ERP.Win.Properties.Resources.InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    this.Close();
                }
                else
                {
                    tbConfirmPass.Focus();
                    tbConfirmPass.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
                    tbConfirmPass.ErrorText = NtbSoft.ERP.Win.Properties.Resources.ConfirmNewPasswordError;
                }
                
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}