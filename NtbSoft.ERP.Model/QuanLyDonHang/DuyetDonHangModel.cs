using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
    public class DuyetDonHangModel
    {
        public DataTable GetEx(string madh, string maLenhSX)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DUYET", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETEXCEL");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", madh ?? "");
                cmd.Parameters.AddWithValue("@parameter1", maLenhSX ?? "");
                cmd.Parameters.AddWithValue("@parameter2", 0);
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
        public string PostDuyet(object ojDinhMuc)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DUYET", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDUYET");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojDinhMuc);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string PostHuy(object ojDinhMuc)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DUYET", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTHUY");
                cmd.Parameters.AddWithValue("@MaDH", 0);
                cmd.Parameters.AddWithValue("@parameter", 0);
                cmd.Parameters.AddWithValue("@parameter1", 0);
                cmd.Parameters.AddWithValue("@parameter2", 0);
                cmd.Parameters.AddWithValue("@TypeTable", ojDinhMuc);
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
