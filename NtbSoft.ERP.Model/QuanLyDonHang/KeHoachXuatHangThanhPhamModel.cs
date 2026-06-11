using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.ThuVien
{
    public class KeHoachXuatHangThanhPhamModel
    {
        public DataTable Get(string action, string para1, string para2, string para3, string para4, string para5,string para6)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_XuatHangNTP_SP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", action);
                cmd.Parameters.AddWithValue("@Para", para1 ?? "");
                cmd.Parameters.AddWithValue("@Para2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Para3", para3 ?? "");
                cmd.Parameters.AddWithValue("@Para4", para4 ?? "");
                cmd.Parameters.AddWithValue("@Para5", para5 ?? "");
                cmd.Parameters.AddWithValue("@Para6", para6 ?? "");
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
        public string PostXH(string action, string para1, string para2, string para3, string para4, string para5, string para6)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_XuatHangNTP_SP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", action);
                cmd.Parameters.AddWithValue("@Para", para1 ?? "");
                cmd.Parameters.AddWithValue("@Para2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Para3", para3 ?? "");
                cmd.Parameters.AddWithValue("@Para4", para4 ?? "");
                cmd.Parameters.AddWithValue("@Para5", para5 ?? "");
                cmd.Parameters.AddWithValue("@Para6", para6 ?? "");
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Post(string action, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_XuatHangNTP_SP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", action);
                cmd.Parameters.AddWithValue("@Para", "");
                cmd.Parameters.AddWithValue("@Para2", "");
                cmd.Parameters.AddWithValue("@Para3", "");
                cmd.Parameters.AddWithValue("@Para4", "");
                cmd.Parameters.AddWithValue("@Para5", "");
                cmd.Parameters.AddWithValue("@Para6", "");
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
