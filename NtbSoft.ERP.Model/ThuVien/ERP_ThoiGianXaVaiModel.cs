using System;
using System.Data;
using System.Data.SqlClient;

namespace NtbSoft.ERP.Model.ThuVien
{
    public class ERP_ThoiGianXaVaiModel
    {
        #region Get

        public DataTable GetChung(string action, string para, string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();

                SqlCommand cmd = new SqlCommand("SP_ERP_ThoiGianXaVai", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("Action", action ?? "");
                cmd.Parameters.AddWithValue("@parameter", para ?? "");
                cmd.Parameters.AddWithValue("@parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", para4 ?? "");
                cmd.Parameters.AddWithValue("@parameter5", para5 ?? "");
                cmd.Parameters.AddWithValue("@TypeTable1", null);

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
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        #endregion
        public DataTable Get(string makh, string mahang)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_ThoiGianXaVai", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET");
                cmd.Parameters.AddWithValue("@parameter", makh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", mahang ?? "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");

                cmd.Parameters.AddWithValue("@TypeTable1", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable2", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable3", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable4", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable5", DBNull.Value);

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


        #region Post

        public string Post(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();

                SqlCommand cmd = new SqlCommand("SP_ERP_ThoiGianXaVai", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@parameter", "");
                cmd.Parameters.AddWithValue("@parameter1", "");
                cmd.Parameters.AddWithValue("@parameter2", "");
                cmd.Parameters.AddWithValue("@parameter3", "");
                cmd.Parameters.AddWithValue("@parameter4", "");
                cmd.Parameters.AddWithValue("@parameter5", "");
                cmd.Parameters.AddWithValue("@TypeTable5", tbl);

                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        #endregion

    }
}