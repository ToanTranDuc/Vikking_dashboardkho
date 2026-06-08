using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.SYSTEM
{
    public class SystemUserModel
    {
        public DataTable GetAll()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_SYS_USER", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET");
                cmd.Parameters.AddWithValue("@ID", 0);
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

        public DataTable Get(string action)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_SYS_USER", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@ID", 0);
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

        public DataTable GetKhoUser()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_SYS_USER", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GETKHOUSER");
                cmd.Parameters.AddWithValue("@ID", 0);
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

        public string Post(DataTable tb)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_SYS_USER", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@ID", 0);
                cmd.Parameters.AddWithValue("@TypeTable", tb);
                cmd.ExecuteNonQuery();
                return "True";
            }catch(SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string UpdateMaKho(DataTable tb)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_SYS_USER", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "UpdateMaKho");
                cmd.Parameters.AddWithValue("@ID", 0);
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

        public string Delete(string id)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_SYS_USER", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETE");
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.ExecuteNonQuery();
                return "True";
            }catch(SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
    }
}
