using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
  public  class ERP_DeleteBOM_DMModel
    {
        public string Post(object tb)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_Delete_DM_BOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "WriteLog");
                cmd.Parameters.AddWithValue("@para1", DBNull.Value);
                cmd.Parameters.AddWithValue("@para2", DBNull.Value);
                cmd.Parameters.AddWithValue("@para3", DBNull.Value);
                cmd.Parameters.AddWithValue("@para4", DBNull.Value);
                cmd.Parameters.AddWithValue("@para5", DBNull.Value);
                cmd.Parameters.AddWithValue("@para6", DBNull.Value);
                cmd.Parameters.AddWithValue("@para7", DBNull.Value);
                cmd.Parameters.AddWithValue("@para8", DBNull.Value);
                cmd.Parameters.AddWithValue("@para9", DBNull.Value);
                cmd.Parameters.AddWithValue("@para10", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable", tb);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
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
        public string Delete(string para1,string para2, string para3, string para4, string para5, string para6="", string para7="", string para8="", string para9="", string para10="")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_Delete_DM_BOM", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Delete");       
                cmd.Parameters.AddWithValue("@para1", para1);
                cmd.Parameters.AddWithValue("@para2", para2);
                cmd.Parameters.AddWithValue("@para3", para3);
                cmd.Parameters.AddWithValue("@para4", para4);
                cmd.Parameters.AddWithValue("@para5", para5);
                cmd.Parameters.AddWithValue("@para6", para6);
                cmd.Parameters.AddWithValue("@para7", para7);
                cmd.Parameters.AddWithValue("@para8", para8);
                cmd.Parameters.AddWithValue("@para9", para9);
                cmd.Parameters.AddWithValue("@para10", para10);

                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
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
    }
}
