using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.ThuVien
{
    public class ERP_ThuVienMauModel
    {
        public DataTable Get(string action, string para, string para2, string para3, string para4, string para5, string para6, string para7)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_MauVTTV", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@parameter", para ?? "");
                cmd.Parameters.AddWithValue("@parameter1", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para4 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", para5 ?? "");
                cmd.Parameters.AddWithValue("@parameter5", para6 ?? "");
                cmd.Parameters.AddWithValue("@parameter6", para7 ?? "");
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
        public DataTable Get(string action, DataTable tbl, string para = "", string para2 = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_MauVTTV", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@parameter", para);
                cmd.Parameters.AddWithValue("@parameter1", para2);
                cmd.Parameters.AddWithValue("@parameter2", DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter3", DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter4", DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter5", DBNull.Value);
                cmd.Parameters.AddWithValue("@parameter6", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
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
        public string Post(string action, string para, string para2, string para3, string para4, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_MauVTTV", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@parameter", para ?? "");
                cmd.Parameters.AddWithValue("@parameter1", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para4 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
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
     
     
        public string Delete(string action, string para, string para2, string para3, string para4)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_MauVTTV", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@parameter", para ?? "");
                cmd.Parameters.AddWithValue("@parameter1", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para4 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@parameter6", "");
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
