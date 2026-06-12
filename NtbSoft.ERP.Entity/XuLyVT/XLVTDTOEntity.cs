using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Entity.XuLyVT
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
            if (dataTable == "XLVT")
            {
                dt = XLVTTable.create();
            }
            else if (dataTable == "XLVTVT")
            {
                dt = XLVTVTTable.create();
            }
            else if (dataTable == "XLVTMUAHANG")
            {
                dt = XLVTMUAHANGTable.create();
            }
            else
            {
                dt = XLVTPRPOTable.create();
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
            return header + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + rd.Next(1000, 10000).ToString();
        }
        public static T SmartTryParse<T>(object input, T defaultValue = default(T))
        {
            if (input == null || input == DBNull.Value)
                return defaultValue;

            string value = input.ToString().Trim();

            if (string.IsNullOrEmpty(value))
                return defaultValue;

            Type targetType = typeof(T);

            try
            {
                if (targetType == typeof(int))
                {
                    int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int result);
                    return (T)(object)result;
                }
                else if (targetType == typeof(long))
                {
                    long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out long result);
                    return (T)(object)result;
                }
                else if (targetType == typeof(float))
                {
                    float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out float result);
                    return (T)(object)result;
                }
                else if (targetType == typeof(double))
                {
                    double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result);
                    return (T)(object)result;
                }
                else if (targetType == typeof(decimal))
                {
                    decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result);
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
                    // Thử Convert cho kiểu khác nếu có thể
                    return (T)Convert.ChangeType(value, targetType);
                }
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}