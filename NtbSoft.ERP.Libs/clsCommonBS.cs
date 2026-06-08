
using System;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NtbSoft.ERP.Libs
{
    public class clsCommonBS : MarshalByRefObject
    {
        public static string sConnectionString = string.Empty;
        private static bool m_IsUserAdmin;
        private static string m_CurrentLanguageID;
        private static string m_DefaultLanguageID;

        public static bool IsAdminUser
        {
            get { return m_IsUserAdmin; }
            set { m_IsUserAdmin = value; }
        }
        public static string CurrentLanguageID
        {
            get { return m_CurrentLanguageID; }
            set { m_CurrentLanguageID = value; }
        }
        public static string DefaultLanguageID
        {
            get { return m_DefaultLanguageID; }
            set { m_DefaultLanguageID = value; }
        }
        public static NumberFormatInfo NumberFormat
        {
            get { return CultureInfo.CurrentCulture.NumberFormat; }
        }

        private static System.Data.DataTable m_dtLanguageList = null;
        public static System.Data.DataTable dtLanguageList
        {
            get
            {
                //if (m_dtLanguageList == null)
                //m_dtLanguageList = new clsLanguagesBS().GetListInUseLanguages();
                return m_dtLanguageList;
            }
            set { m_dtLanguageList = value; }
        }

        /// <summary>
        /// GetServerTime
        /// </summary>
        /// <returns></returns>
        public static DateTime GetServerTime()
        {
            DbAccess db = new DbAccess();
            db.CreateNewSqlCommand();
            DateTime dtServer = Convert.ToDateTime(db.ExecuteScalar("spGetSystemDate"));
            return dtServer;
        }


        private static string VALID_CHARACTERS_EMAIL = @"^[-a-zA-Z0-9][-.a-zA-Z0-9]*@[-.a-zA-Z0-9]+(\.[-.a-zA-Z0-9]+)*\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$";

        /// <summary>        
        /// Check inputed string is right email format
        /// </summary>
        /// <param name="strEmailAddress">Email Address</param>
        /// <returns>true/false</returns>
        public static bool CheckEmailAddress(string strEmailAddress)
        {
            Match emailAddressMatch = Regex.Match(strEmailAddress, VALID_CHARACTERS_EMAIL);

            if (emailAddressMatch.Success)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Save Successfully
        /// </summary>
        public static void SaveSuccessfully()
        {
            MessageBox.Show("Lưu thành công.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Save Not Successfully
        /// </summary>
        public static void SaveNotSuccessfully()
        {
            MessageBox.Show("Lưu không thành công.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Delete Confirmation
        /// </summary>
        public static DialogResult ConfirmDeletion()
        {
            return MessageBox.Show("Bạn có chắc chắn muốn xoá hay không ?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Delete Confirmation
        /// </summary>
        public static DialogResult ConfirmDeletion(object Item)
        {
            string _msg = string.Format("Bạn có chắc chắn xóa \"{0}\" hay không?", Item);
            return MessageBox.Show(_msg, "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public static DialogResult DeleteSuccessfully()
        {
            return MessageBox.Show("Xoá thành công.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult DeleteNotSuccessfully()
        {
            return MessageBox.Show("Xoá không thành công.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Delete Confirmation
        /// </summary>
        public static DialogResult ConfirmChangedData()
        {
            return MessageBox.Show("Dữ liệu đã thay đổi, bạn có muốn lưu hay không ?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Delete Confirmation
        /// </summary>
        public static DialogResult ConfirmError(string ex)
        {
            return MessageBox.Show(ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Delete Confirmation
        /// </summary>
        public static DialogResult ConfirmError(string ex, MessageBoxIcon icon)
        {
            return MessageBox.Show(ex, "LineName", MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Confirmation
        /// </summary>
        public static DialogResult Confirm(string msg)
        {
            return MessageBox.Show(msg, "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
