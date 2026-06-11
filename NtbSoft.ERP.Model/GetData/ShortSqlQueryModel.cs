using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model
{
    public class ShortSqlQueryModel
    {
        public string GetStr(string query)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();


                cmd = new SqlCommand(query, conn); 
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return reader["result"].ToString();
                }
                return "true";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetDataTable(string query)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataAdapter adapter = null;
            DataTable dt = new DataTable();
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                cmd = new SqlCommand(query, conn);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt); 
                return dt; 
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
    }
}
