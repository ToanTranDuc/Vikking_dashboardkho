using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.SYSTEM
{
    public class SystemGETsqlModel
    {
        public DataTable Get(string columns, string tableName, string whereClause)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("sp_Select", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Columns", string.Format(@"{0}", columns));
                cmd.Parameters.AddWithValue("@TableName", string.Format(@"{0}", tableName));
                cmd.Parameters.AddWithValue("@WhereClause", string.Format(@"{0}", whereClause));
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable ds = new DataTable();
                    adt.Fill(ds);
                    return ds;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public DataTable Get(string query)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                string msql = string.Format(@"{0}", query);
                SqlCommand cmd = new SqlCommand(msql, conn);
                cmd.CommandType = CommandType.Text;
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable ds = new DataTable();
                    adt.Fill(ds);
                    return ds;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public DataTable GetIP()
        {
            System.Configuration.AppSettingsReader _appReader = new System.Configuration.AppSettingsReader();
            string dsIP = _appReader.GetValue("IPGetSql", typeof(string)).ToString();
            if (dsIP == "") return new DataTable();
            List<string> lst = dsIP.Split('@').ToList();
            DataTable tblDsIP = new DataTable();
            tblDsIP.Columns.Add("IP", typeof(string));
            foreach(string ip in lst)
            {
                DataRow drIP = tblDsIP.NewRow();
                drIP["IP"] = ip;
                tblDsIP.Rows.Add(drIP);
            }
            return tblDsIP;
        }
    }
}
