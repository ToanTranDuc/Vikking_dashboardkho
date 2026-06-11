using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
    public class DeNghiCapThemNPLModel
    {
        public DataTable Get(string action, string Para1 = "", string Para2 = "", string Para3 = "", string Para4 = "", string Para5 = "", string Para6 = "",string Para7 = "", string Para8= "", string Para9 = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DeNghiCapThemVT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para", Para1 ?? "");
                cmd.Parameters.AddWithValue("@Para2", Para2 ?? "");
                cmd.Parameters.AddWithValue("@Para3", Para3 ?? "");
                cmd.Parameters.AddWithValue("@Para4", Para4 ?? "");
                cmd.Parameters.AddWithValue("@Para5", Para5 ?? "");
                cmd.Parameters.AddWithValue("@Para6", Para6 ?? "");
                cmd.Parameters.AddWithValue("@Para7", Para7 ?? "");
                cmd.Parameters.AddWithValue("@Para8", Para8 ?? "");
                cmd.Parameters.AddWithValue("@Para9", Para9 ?? "");
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

        public string Post(string action, DataTable tbl, string Para1 = "", string Para2 = "", string Para3 = "", string Para4 = "", string Para5 = "", string Para6 = "", string Para7 = "", string Para8 = "", string Para9 = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DeNghiCapThemVT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para", Para1 ?? "");
                cmd.Parameters.AddWithValue("@Para2", Para2 ?? "");
                cmd.Parameters.AddWithValue("@Para3", Para3 ?? "");
                cmd.Parameters.AddWithValue("@Para4", Para4 ?? "");
                cmd.Parameters.AddWithValue("@Para5", Para5 ?? "");
                cmd.Parameters.AddWithValue("@Para6", Para6 ?? "");
                cmd.Parameters.AddWithValue("@Para7", Para7 ?? "");
                cmd.Parameters.AddWithValue("@Para8", Para8 ?? "");
                cmd.Parameters.AddWithValue("@Para9", Para9 ?? "");
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
        public string PostChiTiet(string action, DataTable tbl, string Para1 = "", string Para2 = "", string Para3 = "", string Para4 = "", string Para5 = "", string Para6 = "", string Para7 = "", string Para8 = "", string Para9 = "")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_DeNghiCapThemVT", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para", Para1 ?? "");
                cmd.Parameters.AddWithValue("@Para2", Para2 ?? "");
                cmd.Parameters.AddWithValue("@Para3", Para3 ?? "");
                cmd.Parameters.AddWithValue("@Para4", Para4 ?? "");
                cmd.Parameters.AddWithValue("@Para5", Para5 ?? "");
                cmd.Parameters.AddWithValue("@Para6", Para6 ?? "");
                cmd.Parameters.AddWithValue("@Para7", Para7 ?? "");
                cmd.Parameters.AddWithValue("@Para8", Para8 ?? "");
                cmd.Parameters.AddWithValue("@Para9", Para9 ?? "");
                cmd.Parameters.AddWithValue("@TypeTableCT", tbl);
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
