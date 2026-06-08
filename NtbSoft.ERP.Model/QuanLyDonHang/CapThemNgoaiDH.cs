using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
    public class CapThemNgoaiDH
    {
        public DataTable Get(string action, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null,
                        string param6 = null, string param7 = null, string param8 = null, string param9 = null)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("USP_ERP_CapThemNgoaiDH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Param1", param1);
                cmd.Parameters.AddWithValue("@Param2", param2);
                cmd.Parameters.AddWithValue("@Param3", param3);
                cmd.Parameters.AddWithValue("@Param4", param4);
                cmd.Parameters.AddWithValue("@Param5", param5);
                cmd.Parameters.AddWithValue("@Param6", param6);
                cmd.Parameters.AddWithValue("@Param7", param7);
                cmd.Parameters.AddWithValue("@Param8", param8);
                cmd.Parameters.AddWithValue("@Param9", param9);
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adt.Fill(dt);
                    return dt;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string Post(string action, DataTable tbl, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null,
                        string param6 = null, string param7 = null, string param8 = null, string param9 = null)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("USP_ERP_CapThemNgoaiDH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Param1", param1);
                cmd.Parameters.AddWithValue("@Param2", param2);
                cmd.Parameters.AddWithValue("@Param3", param3);
                cmd.Parameters.AddWithValue("@Param4", param4);
                cmd.Parameters.AddWithValue("@Param5", param5);
                cmd.Parameters.AddWithValue("@Param6", param6);
                cmd.Parameters.AddWithValue("@Param7", param7);
                cmd.Parameters.AddWithValue("@Param8", param8);
                cmd.Parameters.AddWithValue("@Param9", param9);
                cmd.Parameters.AddWithValue("@DataType1", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string PostChiTiet(string action, DataTable tbl, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null,
                        string param6 = null, string param7 = null, string param8 = null, string param9 = null)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("USP_ERP_CapThemNgoaiDH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Param1", param1);
                cmd.Parameters.AddWithValue("@Param2", param2);
                cmd.Parameters.AddWithValue("@Param3", param3);
                cmd.Parameters.AddWithValue("@Param4", param4);
                cmd.Parameters.AddWithValue("@Param5", param5);
                cmd.Parameters.AddWithValue("@Param6", param6);
                cmd.Parameters.AddWithValue("@Param7", param7);
                cmd.Parameters.AddWithValue("@Param8", param8);
                cmd.Parameters.AddWithValue("@Param9", param9);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
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
