using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.ThuVien
{
    public class ERPHangHoaModel
    {
        public DataTable Get(string action, string para, string para2, string para3, string para4, string para5, string para6)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPHangHoa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", action);
                cmd.Parameters.AddWithValue("@Parameter", para ?? "");
                cmd.Parameters.AddWithValue("@Parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", para4 ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", para5 ?? "");
                cmd.Parameters.AddWithValue("@Parameter6", para6 ?? "");
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

        // Lấy danh sách màu theo mã hàng

        public string Post(string action, string type, object tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPHangHoa", conn);
                cmd.CommandTimeout = 100000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
                cmd.Parameters.AddWithValue(type, tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostEdit(string action, string type, object tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPHangHoaEdit", conn);
                cmd.CommandTimeout = 100000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
                cmd.Parameters.AddWithValue(type, tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string DeleteSize(string action, string type, object tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPDeleteSize", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
                cmd.Parameters.AddWithValue(type, tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public string Delete(string action, string parameter,string para2, string para3, string para4)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPHangHoa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter", parameter);
                cmd.Parameters.AddWithValue("@Parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@Parameter4", para4 ?? "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable GetKiemTra(string action, string type, object tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERPDeleteSize", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Parameter", "");
                cmd.Parameters.AddWithValue("@Parameter2", "");
                cmd.Parameters.AddWithValue("@Parameter3", "");
                cmd.Parameters.AddWithValue("@Parameter4", "");
                cmd.Parameters.AddWithValue("@Parameter5", "");
                cmd.Parameters.AddWithValue("@Parameter6", "");
                cmd.Parameters.AddWithValue(type, tbl);
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
    }
}
