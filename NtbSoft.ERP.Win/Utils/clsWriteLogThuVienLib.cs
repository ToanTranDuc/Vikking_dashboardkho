using NtbSoft.ERP.Entity.ThuVien;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Win.Utils
{
   public static class clsWriteLogThuVienLib
    {
     
        public static string FormatRow(DataRow row, string separator = " | ")
        {
            if (row == null) return string.Empty;

            var list = new List<string>();

            foreach (DataColumn col in row.Table.Columns)
            {
                var value = row[col];
                string displayValue = value == DBNull.Value ? "null" : value.ToString();
                list.Add($"{col.ColumnName}: {displayValue}");
            }

            return string.Join(separator, list);
        }
        public static string FormatRow(object obj, string separator = " | ")
        {
            if (obj == null)
                return string.Empty;

            var list = new List<string>();

           
            if (obj is IDictionary<string, object> dict)
            {
                foreach (var kvp in dict)
                {
                    string displayValue = kvp.Value?.ToString() ?? "null";
                    list.Add($"{kvp.Key}: {displayValue}");
                }
            }
            else
            {
               
                var props = obj.GetType().GetProperties();

                foreach (var prop in props)
                {
                    object value = prop.GetValue(obj, null);
                    string displayValue = value?.ToString() ?? "null";
                    list.Add($"{prop.Name}: {displayValue}");
                }
            }

            return string.Join(separator, list);
        }

        public static void WriteLog(string URL, HttpClientExtension clientExtension, List<LogThuvienEntity> lstLog,string TableName,string KeyColumn ="ID")
        {
            try
            {
                if(lstLog.Count > 0)
                {
                    string url = string.Format("{0}?TableName={1}&KeyColumn={2}", URL + "WriteLogThuVien/WriteLog", TableName, KeyColumn);
                    string msg = Task.Run(async () => { return await clientExtension.PostAsync(url, lstLog); }).Result;
                }
                
            }
            catch (Exception ex)
            {

            }
        }

        public static void WriteLog_BOM_DM(string URL, HttpClientExtension clientExtension, List<Log_Erp_KhoiTaoBom_Delete> lstLog)
        {
            try
            {
                if (lstLog.Count > 0)
                {
                    string url = string.Format("{0}", URL + "DeleteBOM_DM/WriteLog");
                    string msg = Task.Run(async () => { return await clientExtension.PostAsync(url, lstLog); }).Result;
                }
            }
            catch(Exception ex)
            {

            }
        }
    }

    public class LogThuvienEntity
    {
        public int ID { get; set; }
        public string Action { get; set; }
        public string Module { get; set; }
        public string Content { get; set; }
        public string UserID { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
    public class Log_Erp_KhoiTaoBom_Delete
    {
        //public int ID { get; set; }
        public string Action { get; set; }
        //public string TableName { get; set; }
        public string Modules { get; set; }
        public string MaVTID { get; set; }
        public string MauVTID { get; set; }
        public string MaNhom { get; set; }
        public string KhoVaiID { get; set; }
        public string MaCode { get; set; }
        public string Content { get; set; }
        public string IP { get; set; }
        public string UserID { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

    }
}
