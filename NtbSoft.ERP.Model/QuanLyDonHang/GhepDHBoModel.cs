using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.KeHoach
{
    public static class GhepDHBoModel
    {
        public static DataTable GET_DH()
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_GhepDHBo", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_DH");
                cmd.Parameters.AddWithValue("@sqlQuery", "");
                cmd.Parameters.AddWithValue("@Parameter", "");
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
        public static DataTable GET_DH_GHEP(string donHang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_GhepDHBo", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GET_DH_GHEP");
                cmd.Parameters.AddWithValue("@sqlQuery", "");
                cmd.Parameters.AddWithValue("@Parameter", donHang);
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
        public static string POST(string donHang, DataTable tb)
        {
            string sqlExect = "insert into #TypeTablePost(MaDHGoc, MaDHGhep) values ";
            if(tb.Rows.Count > 0)
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                sqlExect += string.Format(@"('{0}', '{1}')", tb.Rows[i]["MaDHGoc"].ToString(), tb.Rows[i]["MaDHGhep"].ToString());
                if (i < tb.Rows.Count - 1)
                    sqlExect += ",";
                else
                    sqlExect += ";";
                }
            else
            {
                sqlExect = "print('true')";
            }
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_GhepDHBo", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@Parameter", donHang);
                cmd.Parameters.AddWithValue("@sqlQuery", sqlExect);
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
