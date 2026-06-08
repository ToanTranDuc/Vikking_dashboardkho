using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
namespace NtbSoft.ERP.Model.Kho
{
    public class NguyenLieuModel
    {
        public DataTable Get()
        {   
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KhoNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetNL");

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
                SqlCommand cmd = new SqlCommand("SP_KhoNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTNL");
                cmd.Parameters.AddWithValue("@TypeTable2", tb);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Delete(int id)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_KhoNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "DELETENL");
                cmd.Parameters.AddWithValue("@Parameter", id);

                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
    }
}
