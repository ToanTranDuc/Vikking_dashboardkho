
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NtbSoft.ERP.Utils
{
   public static class clsForrmatUtils
    {
        public static bool IsValueEmpty(string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
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
            if (strDate == null) return result;

            string raw = strDate.ToString().Trim();

            // Nếu chuỗi chứa 2 thời điểm, chỉ lấy phần đầu tiên
            if (raw.Contains(" ") && raw.Split(' ').Length >= 2)
            {
                raw = raw.Substring(0, raw.IndexOf(" ", raw.IndexOf(" ") + 1));
            }

            string[] formats1 = GetDateFormats();
            string[] formats2 = GetDateParten();

            if (DateTime.TryParseExact(raw, formats1, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date1))
            {
                result = date1;
            }
            else if (DateTime.TryParseExact(raw, formats2, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date2))
            {
                result = date2;
            }

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
