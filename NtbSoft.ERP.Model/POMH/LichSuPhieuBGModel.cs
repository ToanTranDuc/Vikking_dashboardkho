using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.POMuaHang
{
    public class LichSuPhieuBGModel
    {
        public DataTable Get(string action, string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_POMH_LichSuPhieuBaoGia", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@para1", para1 ?? "");
                cmd.Parameters.AddWithValue("@para2", para2 ?? "");
                cmd.Parameters.AddWithValue("@para3", para3 ?? "");
                cmd.Parameters.AddWithValue("@para4", para4 ?? "");
                cmd.Parameters.AddWithValue("@para5", para5 ?? "");
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


        public DataTable GetNCP(string action, string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_POMH_NhomCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@para1", para1 ?? "");
                cmd.Parameters.AddWithValue("@para2", para2 ?? "");
                cmd.Parameters.AddWithValue("@para3", para3 ?? "");
                cmd.Parameters.AddWithValue("@para4", para4 ?? "");
                cmd.Parameters.AddWithValue("@para5", para5 ?? "");
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
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public string Post(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_POMH_NhomCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTNHOMCP");
                cmd.Parameters.AddWithValue("@para1",  "");
                cmd.Parameters.AddWithValue("@para2", "");
                cmd.Parameters.AddWithValue("@para3", "");
                cmd.Parameters.AddWithValue("@para4", "");
                cmd.Parameters.AddWithValue("@para5", "");
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
        public string Delete(string para1)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_POMH_NhomCP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETENHOMCP");
                cmd.Parameters.AddWithValue("@para1", para1 ?? "");
                cmd.Parameters.AddWithValue("@para2", "");
                cmd.Parameters.AddWithValue("@para3", "");
                cmd.Parameters.AddWithValue("@para4", "");
                cmd.Parameters.AddWithValue("@para5", "");
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
