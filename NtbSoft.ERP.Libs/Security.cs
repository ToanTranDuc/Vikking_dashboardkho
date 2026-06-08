using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Libs
{
    public class Security
    {
        public string mRegKey = "";
        public int mSessionID = 0;

        public string mSessk = "Session";

        public bool isSetup = false;

        public string mRootKey = "";

        public string mFolder = "";

        public Security()
        {
            mRootKey = "Software\\";
            mFolder = "LineName";

            getReg();

            mRegKey = mRootKey + mFolder;
        }
        public bool isRegOk = false;
        private void getReg()
        {
            isRegOk = true;
            if (!File.Exists("regedit.txt"))
            {
                isRegOk = false;
                return;
            }

            StreamReader sr = new StreamReader("regedit.txt");
            String line = sr.ReadLine();
            mFolder = line;

            return;
        }
        public object get_RegistryKey(string _key)
        {
            object obj;
            RegistryKey key;

            key = Registry.CurrentUser.OpenSubKey(mRegKey);
            obj = key.GetValue(_key);
            key.Close();
            return obj;
        }


        public void set_RegistryKey(string objkeyname, object valueobjkey)
        {
            RegistryKey key;
            try
            {
                key = Registry.CurrentUser.CreateSubKey(mRegKey);
                key.SetValue(objkeyname, valueobjkey);
                key.Close();
            }
            catch
            {
            }
        }
        public object get_RegistryKey(string path, string _key)
        {
            object obj;
            RegistryKey key;

            key = Registry.CurrentUser.OpenSubKey(path);
            if (key == null) return null;
            obj = key.GetValue(_key);
            key.Close();
            return obj;
        }

        public void set_RegistryKey(string path, string objkeyname, object valueobjkey)
        {
            RegistryKey key;
            try
            {
                key = Registry.CurrentUser.CreateSubKey(path);
                key.SetValue(objkeyname, valueobjkey);
                key.Close();
            }
            catch
            {
            }
        }
        public bool is_exist_key()
        {
            Microsoft.Win32.RegistryKey regkeyApp;
            regkeyApp = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(mRegKey);
            if (regkeyApp == null)
                return false;
            else
                return true;
        }

        public bool is_exist_key(string Path, string objKeyName)
        {
            Microsoft.Win32.RegistryKey regkeyApp;
            regkeyApp = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Path);
            if (regkeyApp == null)
                return false;
            else
            {
                if (regkeyApp.GetValue(objKeyName) == null)
                    return false;
                return true;
            }

        }

        public void detele_RegistryAllKey(string path, string skey)
        {
            Microsoft.Win32.RegistryKey regkeyApp;
            regkeyApp = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path,
                RegistryKeyPermissionCheck.ReadWriteSubTree,
                System.Security.AccessControl.RegistryRights.FullControl);
            if (regkeyApp != null)
                try
                {
                    regkeyApp.DeleteSubKeyTree(skey);
                }
                catch { }
            regkeyApp.Close();
        }

        public void detele_RegistryKey(string path, string value)
        {
            Microsoft.Win32.RegistryKey regkeyApp;
            regkeyApp = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path,
                RegistryKeyPermissionCheck.ReadWriteSubTree,
                System.Security.AccessControl.RegistryRights.FullControl);
            if (regkeyApp != null)
                try
                {
                    if (this.is_exist_key(path, value))
                        regkeyApp.DeleteValue(value);
                }
                catch { }
            regkeyApp.Close();
        }
        public void create_RegistryKey()
        {
            Microsoft.Win32.RegistryKey regkeyApp;
            regkeyApp = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(mRegKey);

            if (regkeyApp == null)
            {
                regkeyApp = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(mRegKey);
                regkeyApp = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(mRegKey + "\\" + mSessk);

                //mSessionID = 1;
                regkeyApp.SetValue(mSessk, mSessionID);

                regkeyApp = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(mRegKey + "\\" + mSessionID.ToString("0000"));

                createKeySession(regkeyApp);
            }

            regkeyApp.Close();
        }

        private void createKeySession(Microsoft.Win32.RegistryKey regkeyApp)
        {
            regkeyApp.SetValue("m_DirApp", "");

            regkeyApp.SetValue("m_administrator", "");
            regkeyApp.SetValue("m_password", "");
            regkeyApp.SetValue("m_isRemember", 0);

            //regkeyApp.SetValue("m_ServerName", "");
            //regkeyApp.SetValue("m_AppDataName", "");
            //regkeyApp.SetValue("m_Sa", "");
            //regkeyApp.SetValue("m_Pwd", "");
            //regkeyApp.SetValue("m_TimeOut", 600);
        }
        //  `
        // Mã hóa theo chuỗi "khóa mã" Trả về chuỗi mã hóa không bao gồm "khóa mã"
        // Ghi đoạn mã hóa kèm theo "khóa mã" vào file tạm "SystemConfig.Sys"
        public string EndcodeSqlString(string StrSql, string FILE_NAMEs, string keycodding)
        {
            StrSql = StrSql.Trim();
            keycodding = keycodding.Trim();
            string un_keycode = "";
            string StrCodeResult = "";
            string StrCodeView = "";
            int numbercode = 30;
            string FILE_NAME = "";

            if (FILE_NAMEs == "")
                FILE_NAME = "SystemConfig.Sys";

            StreamWriter strw = new StreamWriter(FILE_NAME);
            strw.Write((string)(StrSql));
            strw.Flush();
            strw.Close();


            if (keycodding != "")
            {
                for (int s = 0; s < keycodding.Length; s++)
                {
                    un_keycode = keycodding.Substring(s, 1) + un_keycode;
                    numbercode = numbercode + KeyCodingNow(un_keycode);
                }
                StrCodeResult = "#" + un_keycode + "#";
            }

            StreamReader strfr = new StreamReader(FILE_NAME);
            char[] c = null;

            while (strfr.Peek() >= 0)
            {
                c = new char[1];
                strfr.Read(c, 0, 1);
                c[0] = (char)((int)c[0] + numbercode);
                StrCodeView = StrCodeView + c[0].ToString();
                StrCodeResult = StrCodeResult + c[0].ToString();
            }
            strfr.Close();

            StreamWriter strfw = new StreamWriter(FILE_NAME);
            strfw.Write((string)(StrCodeResult));

            strfw.Flush();
            strfw.Close();
            return StrCodeView;
        }

        // Giải mã theo chuỗi "khóa mã" trả về chuỗi ký tự gốc 
        public string DecodeSqlString(string FILE_NAME)
        {
            string StrSql = "";
            string STRKEY = "";
            string i = "0";
            int _keycode = 30;
            if (FILE_NAME == "")
                FILE_NAME = "SystemConfig.Sys";
            StreamReader strr = new StreamReader(FILE_NAME);

            string _tmp_info = "";
            String[] _srr_str = new String[128];
            int idx = 0;

            char[] c = null;
            c = new char[1];
            //Kiểm tra ký tự đầu tiên của chuỗi
            // Nếu là "#" sẽ có khóa mã (khóa mã là một chuỗi đảo ngược)
            // Nếu không phải là "#" thì file được mã hóa với khóa là 30
            // dấu "#" không bao giờ xuất hiện trong vùng bị mã hóa vì mã ASCI (35) 
            char[] cp = null;
            cp = new char[1];
            strr.Read(cp, 0, 1);
            if (cp[0].ToString() == "#")
                i = "0";
            else
            {
                i = "1";
                cp[0] = (char)((int)cp[0] - _keycode);
                StrSql = StrSql + cp[0].ToString();


            }
            // Bắt đầu kiểm tra ký tự thứ 2 của chuỗi
            while (strr.Peek() >= 0)
            {
                // Kiểm tra khóa
                // Nếu gặp dấu "#" kết thúc đọan khóa mã, cờ hiệu i="1"
                c = new char[1];
                strr.Read(c, 0, 1);
                if (c[0].ToString().Contains("#") && i == "0")
                {
                    i = "1";
                }
                else
                {
                    // Kết thúc kiểm tra
                    if (i == "0")
                    {
                        STRKEY = c[0].ToString() + STRKEY;
                        _keycode = _keycode + KeyCodingNow(STRKEY);
                    }
                    else
                    {
                        c[0] = (char)((int)c[0] - _keycode);
                        StrSql = StrSql + c[0].ToString();

                        _tmp_info = _tmp_info + c[0].ToString();
                    }
                }

                if (c[0].ToString().Contains("#"))
                {
                    _srr_str[idx] = _tmp_info;
                    idx += 1;
                    _tmp_info = "";
                }
            }
            strr.Close();
            return StrSql;
        }

        public String[] DecodeSql_Array_String(string FILE_NAME)
        {
            string StrSql = "";
            string STRKEY = "";
            string i = "0";
            int _keycode = 30;
            if (FILE_NAME == "")
                FILE_NAME = "SystemConfig.Sys";
            StreamReader strr = new StreamReader(FILE_NAME);

            string _tmp_info = "";
            String[] _srr_str = new String[128];
            int idx = 0;
            char[] c = null;
            c = new char[1];


            //Kiểm tra ký tự đầu tiên của chuỗi
            // Nếu là "#" sẽ có khóa mã (khóa mã là một chuỗi đảo ngược)
            // Nếu không phải là "#" thì file được mã hóa với khóa là 30
            // dấu "#" không bao giờ xuất hiện trong vùng bị mã hóa vì mã ASCI (35) 
            char[] cp = null;
            cp = new char[1];
            strr.Read(cp, 0, 1);
            if (cp[0].ToString() == "#")
                i = "0";
            else
            {
                i = "1";
                cp[0] = (char)((int)cp[0] - _keycode);
                StrSql = StrSql + cp[0].ToString();
            }
            // Bắt đầu kiểm tra ký tự thứ 2 của chuỗi
            while (strr.Peek() >= 0)
            {
                // Kiểm tra khóa
                // Nếu gặp dấu "#" kết thúc đọan khóa mã, cờ hiệu i="1"
                c = new char[1];
                strr.Read(c, 0, 1);
                if (c[0].ToString().Contains("#") && i == "0")
                {
                    i = "1";
                }
                else
                {
                    // Kết thúc kiểm tra
                    if (i == "0")
                    {
                        STRKEY = c[0].ToString() + STRKEY;
                        _keycode = _keycode + KeyCodingNow(STRKEY);
                    }
                    else
                    {
                        c[0] = (char)((int)c[0] - _keycode);
                        StrSql = StrSql + c[0].ToString();

                        _tmp_info = _tmp_info + c[0].ToString();
                    }
                }

                if (c[0].ToString().Contains("#"))
                {
                    _srr_str[idx] = _tmp_info;
                    idx += 1;
                    _tmp_info = "";
                }
            }
            strr.Close();
            return _srr_str;
        }

        // Trả về chuỗi mã hóa nguyên trong file tạm (bao gồm cả khóa) 
        // Nằn trong thư mục chính của dự án
        public string ResultAfter_EndcodeSqlString()
        {
            string StrSql = "";
            string FILE_NAME = "SystemConfig.Sys";

            StreamReader strr = new StreamReader(FILE_NAME);
            StrSql = strr.ReadToEnd();
            strr.Close();
            return StrSql;
        }

        public string feadstring(string sValue, int Id)
        {
            Encoding ascii = Encoding.ASCII;
            String unicodeString = sValue;
            Byte[] encodedBytes = ascii.GetBytes(unicodeString);
            String decodedString = ascii.GetString(encodedBytes);

            sValue = decodedString;

            int i = 0;
            string sReturn = "", st = "";
            sValue = sValue.Trim();
            for (i = 1; i <= 64; i++)
            {
                st = st + ((char)i).ToString();
            }
            if (sValue == "")
                sReturn = st;
            if (sValue == st)
                sReturn = "";
            else
            {
                for (i = 0; i < sValue.Length; i++)
                {
                    //string c = sValue.Substring(i, 1);
                    //j = (int)c[0] + Id;
                    //j = (((char)((int)sValue[i] + Id))).ToString();
                    //sReturn = sReturn + j;
                    sReturn = sReturn + (((char)((int)sValue[i] + Id))).ToString();
                }
            }

            return sReturn;
        }

        // Công thức tính khóa mã
        public int KeyCodingNow(string _STRKEY)
        {
            int subskey = 0;
            subskey = subskey + (int)_STRKEY[0] % 2;
            return subskey;
        }

        // Trả về chuỗi khóa mã
        public string DecodeStrKey(string FILE_NAME)
        {
            string _StrKey = "";
            string StrSql = "";
            int i = 0;


            if (FILE_NAME == "")
                FILE_NAME = "SystemConfig.Sys";
            StreamReader strr = new StreamReader(FILE_NAME);
            StrSql = strr.ReadToEnd();
            strr.Close();


            for (i = 0; i < 16; i++)
            {
                string c = "";
                c = StrSql.Substring(i, 1);
                if (c == "#" && i > 1)
                    break;
                if (c == "#")
                {
                }
                else
                {
                    if (i == 0)
                    {
                        break;
                    }
                    else
                    {
                        _StrKey = c + _StrKey;
                    }
                }

            }

            return _StrKey;
        }
    }
}
