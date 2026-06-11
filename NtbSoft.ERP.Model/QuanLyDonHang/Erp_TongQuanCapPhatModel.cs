using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
   public class Erp_TongQuanCapPhatModel
    {
        public DataTable Get(string action, string para1, string para2, string para3, string para4, string para5)
        {
            
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_TongQuanCapPhat", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@para1", para1=="NONE"? DBNull.Value : (object)para1);
                cmd.Parameters.AddWithValue("@para2", para2 == "NONE" ? DBNull.Value : (object)para2);
                cmd.Parameters.AddWithValue("@para3", para3 == "NONE" ? DBNull.Value : (object)para3);
                cmd.Parameters.AddWithValue("@para4", para4 == "NONE" ? DBNull.Value : (object)para4);
                cmd.Parameters.AddWithValue("@para5", para5 == "NONE" ? DBNull.Value : (object)para5);
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
