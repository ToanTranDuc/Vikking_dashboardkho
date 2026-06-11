using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.ThuVien
{
    public class BrandModel
    {
        public DataTable Get(string action = "",string para = "", string para1 = "", string para2 = "", string para3 = "", string para4 = "", string para5 = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BRAND", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", action);
                cmd.Parameters.AddWithValue("@Parameter", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", para4 ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", para5 ?? "");
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
        public string Post(object ojb)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BRAND", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojb);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Delete(string parameter)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_BRAND", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@Parameter", parameter);
                cmd.Parameters.AddWithValue("@Parameter1", 0);
                cmd.Parameters.AddWithValue("@Parameter2", 0);
                cmd.Parameters.AddWithValue("@Parameter3", 0);
                cmd.Parameters.AddWithValue("@Parameter4", 0);
                cmd.Parameters.AddWithValue("@Parameter5", 0);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
    }
}
