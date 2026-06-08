using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.SYSTEM
{
    public class SystemLogModel
    {
        public DataTable Get(DateTime fromDate,DateTime toDate)
        {
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_SYS_LOG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET");
                cmd.Parameters.AddWithValue("@ID", 0);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
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
        public string ExeSQL(string content)
        {
            SqlConnection conn = null;
            try
            {
                IEnumerable<string> commandStrings = Regex.Split(content, @"^\s*GO\s*$",
                           RegexOptions.Multiline | RegexOptions.IgnoreCase);

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();

                foreach (string commandString in commandStrings)
                {
                    if (commandString.Trim() != "")
                    {
                        using (var command = new SqlCommand(commandString, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Post(DataTable tb)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_SYS_LOG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@ID", 0);
                cmd.Parameters.AddWithValue("@FromDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@ToDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@TypeTable", tb);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Delete(long id)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_SYS_LOG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@FromDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@ToDate", DateTime.Now);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
    }
}
