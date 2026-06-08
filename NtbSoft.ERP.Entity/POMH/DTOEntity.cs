using NtbSoft.ERP.Entity.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.POMuaHang
{
    public class XuLyVTRequestPost
    {
        public string Action { get; set; }
        public string Parameter { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
        public int Parameter3 { get; set; }
        public float Parameter4 { get; set; }
        public float Parameter5 { get; set; }
        public DateTime? Parameter6 { get; set; }
        public DateTime? Parameter7 { get; set; }
        public DataTable TypeTable { get; set; }

        public static XuLyVTRequestPost createDefault(string dataTable)
        {

            DataTable dt;
            if (dataTable == "CanDoiNPL")
            {
                dt = CanDoiNPLTable.create();
            }
            else if (dataTable == "VatTuMinMax")
            {
                dt = VatTuMinMaxTable.create();
            }
            else if (dataTable == "TyGia")
            {
                dt = TyGiaTable.create();
            }
            else if (dataTable == "DanhGiaNLV")
            {
                dt = DanhGiaNoiLamBViecTable.create();
            }
            else
            {
                dt = CanDoiNPLTable.create();
            }
            return new XuLyVTRequestPost
            {
                Action = "",
                Parameter = "",
                Parameter1 = "",
                Parameter2 = "",
                Parameter3 = 0,
                Parameter4 = 0.0f,
                Parameter5 = 0.0f,
                Parameter6 = null,
                Parameter7 = null,
                TypeTable = dt
            };
        }
    }
    public class XuLyVTRequestGet
    {
        public string Action { get; set; }
        public string Parameter { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
        public int Parameter3 { get; set; }
        public float Parameter4 { get; set; }
        public float Parameter5 { get; set; }
        public DateTime? Parameter6 { get; set; }
        public DateTime? Parameter7 { get; set; }

        public static XuLyVTRequestGet createDefault()
        {
            return new XuLyVTRequestGet
            {
                Action = "",
                Parameter = "",
                Parameter1 = "",
                Parameter2 = "",
                Parameter3 = 0,
                Parameter4 = 0.0f,
                Parameter5 = 0.0f,
                Parameter6 = null,
                Parameter7 = null,
            };
        }
    }
    public class XuLyVTUnits
    {
        private static readonly Random rd = new Random();

        public static string generatedTimeKey(string header)
        {
            return header + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + rd.Next(100000, 1000000).ToString();
        }
        private static string NormalizeNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.Trim();

            int commaCount = input.Count(c => c == ',');
            int dotCount = input.Count(c => c == '.');

            // Có cả , và .
            if (commaCount > 0 && dotCount > 0)
            {
                int lastComma = input.LastIndexOf(',');
                int lastDot = input.LastIndexOf('.');

                if (lastComma > lastDot)
                {
                    // 3.308,44
                    input = input.Replace(".", "");
                    input = input.Replace(",", ".");
                }
                else
                {
                    // 3,308.44
                    input = input.Replace(",", "");
                }
            }
            // Chỉ có ,
            else if (commaCount > 0)
            {
                if (commaCount > 1)
                {
                    // 12,234,234 → thousand
                    input = input.Replace(",", "");
                }
                else
                {
                    // 123456,78 → decimal
                    input = input.Replace(",", ".");
                }
            }
            // Chỉ có .
            else if (dotCount > 0)
            {
                if (dotCount > 1)
                {
                    // 12.234.234 → thousand
                    input = input.Replace(".", "");
                }
                // else: 3308.44 → OK
            }

            return input;
        }
        public static T SmartTryParse<T>(object input, T defaultValue = default)
        {
            if (input == null || input == DBNull.Value)
                return defaultValue;

            string value = input.ToString().Trim();
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            Type targetType = typeof(T);

            try
            {
                if (targetType == typeof(float))
                {
                    value = NormalizeNumber(value);
                    float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out float result);
                    return (T)(object)result;
                }
                else if (targetType == typeof(double))
                {
                    value = NormalizeNumber(value);
                    double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result);
                    return (T)(object)result;
                }
                else if (targetType == typeof(decimal))
                {
                    value = NormalizeNumber(value);
                    decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result);
                    return (T)(object)result;
                }
                else if (targetType == typeof(int))
                {
                    int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int result);
                    return (T)(object)result;
                }
                else if (targetType == typeof(long))
                {
                    long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out long result);
                    return (T)(object)result;
                }
                else if (targetType == typeof(bool))
                {
                    bool.TryParse(value, out bool result);
                    return (T)(object)result;
                }
                else if (targetType == typeof(DateTime))
                {
                    DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
                    return (T)(object)result;
                }
                else if (targetType == typeof(Guid))
                {
                    Guid.TryParse(value, out Guid result);
                    return (T)(object)result;
                }
                else if (targetType == typeof(string))
                {
                    return (T)(object)value;
                }
                else
                {
                    return (T)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime? ParseDateByCulture(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var culture = CultureInfo.CurrentCulture; // lấy culture hiện tại của thread
            DateTime dt;

            // Nếu đang chạy vi-VN thì parse theo dd/MM/yyyy
            if (culture.Name == "vi-VN")
            {
                if (DateTime.TryParseExact(input,
                                           new[] { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss" },
                                           culture,
                                           DateTimeStyles.None,
                                           out dt))
                {
                    return dt;
                }
            }
            // Nếu đang chạy en-US thì parse theo MM/dd/yyyy
            else if (culture.Name == "en-US")
            {
                if (DateTime.TryParseExact(input,
                                           new[] { "MM/dd/yyyy", "MM/dd/yyyy HH:mm:ss" },
                                           culture,
                                           DateTimeStyles.None,
                                           out dt))
                {
                    return dt;
                }
            }

            // fallback: thử invariant với vài format phổ biến
            string[] formats = { "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss" };
            if (DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out dt))
            {
                return dt;
            }

            return null;
        }
    }
}

