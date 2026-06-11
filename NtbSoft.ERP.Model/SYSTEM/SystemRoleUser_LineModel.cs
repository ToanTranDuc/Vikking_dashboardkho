using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.SYSTEM
{
    public class SystemRoleUser_LineModel
    {

        public DataTable GetRole(string user)
        {
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhanQuyen_Users", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "GetPQ_User");
                cmd.Parameters.AddWithValue("@UserName", user);
                cmd.Parameters.AddWithValue("@MaDVSX", "");
                cmd.Parameters.AddWithValue("@LineID", "");
                cmd.Parameters.AddWithValue("@TypeTable", null);
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
        public string Update(string user, DataTable dataUpdate)
        {
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PhanQuyen_Users", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@action", "Update");
                cmd.Parameters.AddWithValue("@UserName", user);
                cmd.Parameters.AddWithValue("@MaDVSX", "");
                cmd.Parameters.AddWithValue("@LineID", "");
                cmd.Parameters.AddWithValue("@TypeTable", dataUpdate);
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
