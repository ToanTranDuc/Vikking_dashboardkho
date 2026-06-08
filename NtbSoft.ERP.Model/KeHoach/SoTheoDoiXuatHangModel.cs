using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.KeHoach
{
    public class SoTheoDoiXuatHangModel
    {
        public DataTable Get(string Action, string para1, string para2, string para3, string para4)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_SOTHEODOIXH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@Para1", para1);
                cmd.Parameters.AddWithValue("@Para2", para2);
                cmd.Parameters.AddWithValue("@Para3", para3);
                cmd.Parameters.AddWithValue("@Para4", para4);
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
        public string Post(DataTable table)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_SOTHEODOIXH", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Post");
                cmd.Parameters.AddWithValue("@Para1", "");
                cmd.Parameters.AddWithValue("@Para2", "");
                cmd.Parameters.AddWithValue("@Para3", "");
                cmd.Parameters.AddWithValue("@Para4", "");
                cmd.Parameters.AddWithValue("@TypePackageList", table);
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
