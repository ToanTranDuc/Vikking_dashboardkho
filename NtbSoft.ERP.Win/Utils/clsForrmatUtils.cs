using DevExpress.XtraEditors;
using NtbSoft.ERP.Win.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NtbSoft.ERP.Win.Utils
{
   public static class clsForrmatUtils
    {
        public static bool IsValueEmpty(string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }

      
        public static void ShowMessger(string message)
        {
            XtraMessageBoxArgs args = new XtraMessageBoxArgs();
            args.Caption = Resources.Warning;
            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Text = $"{message}";
            args.Buttons = new DialogResult[] { DialogResult.OK };
            args.Icon = SystemIcons.Warning;
            args.MessageBeepSound = MessageBeepSound.Warning;
            XtraMessageBox.Show(args);
        }

        private static string[] GenerateDateFormats(string baseFormat)
        {
            List<string> formats = new List<string>();


            formats.Add(baseFormat);


            formats.Add(baseFormat.Replace("/", "-"));
            formats.Add(baseFormat.Replace("-", "/"));
            formats.Add(baseFormat.Replace("/", "."));


            string[] components = baseFormat.Split('/');
            if (components.Length == 3)
            {
                string reorderedFormat = $"{components[1]}/{components[0]}/{components[2]}";
                formats.Add(reorderedFormat);
                formats.Add(reorderedFormat.Replace("/", "-"));
                formats.Add(reorderedFormat.Replace("-", "/"));
                formats.Add(reorderedFormat.Replace("/", "."));
            }

            return formats.ToArray();
        }

        private static string[] GetDateFormats()
        {
            string baseFormat = "d/M/yyyy";
            return GenerateDateFormats(baseFormat);
        }

        private static string[] GetDateParten()
        {
            DateTimeFormatInfo formatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
            string[] datePatterns = formatInfo.GetAllDateTimePatterns();
            return datePatterns;
        }

        public static DateTime ConvertDate(object strDate)
        {
            DateTime result = DateTime.MinValue;
            if (strDate == null || strDate == DBNull.Value) return result;

           
            if (strDate is DateTime dt)
                return dt;

            string raw = strDate.ToString().Trim();
            if (string.IsNullOrWhiteSpace(raw)) return result;

          
            if (raw.Contains(" "))
                raw = raw.Split(' ')[0].Trim();

                        string[] sqlFormats = new[]
                        {
                    "yyyy-MM-dd",
                    "yyyy/MM/dd",
                    "yyyy.MM.dd"
                };

            if (DateTime.TryParseExact(raw, sqlFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime sqlDate))
                return sqlDate;

          
            string[] formats1 = GetDateFormats();
            if (DateTime.TryParseExact(raw, formats1, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date1))
                return date1;

  
            string[] formats2 = GetDateParten();
            if (DateTime.TryParseExact(raw, formats2, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date2))
                return date2;

            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date3))
                return date3;

            return result;
        }

        public static string ParseMinutesFromTime(string timeString)
        {
            if (IsValueEmpty(timeString)) return null;

            var parts = timeString.Split(':');
            if (parts.Length == 3 && int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes))
            {
                return $"{hours}:{minutes.ToString("D2")}";
            }

            return null;
        }
        public static int? TryParseInt(object value)
        {
            if (value != DBNull.Value && int.TryParse(value.ToString(), out var result))
            {
                return result;
            }
            return null;
        }
    }
}
