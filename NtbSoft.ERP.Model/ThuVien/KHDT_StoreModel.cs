using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.ThuVien
{
   public class KHDT_StoreModel
    {
        public DataTable Get(string Action, string Para1,string Para2)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("Sp_Dic_KHDT_Store", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", Action);
                cmd.Parameters.AddWithValue("@Para1", string.IsNullOrEmpty(Para1) ? DBNull.Value : (object)Para1);
                cmd.Parameters.AddWithValue("@Para2", string.IsNullOrEmpty(Para2) ? DBNull.Value : (object)Para2);
                cmd.Parameters.AddWithValue("@Para3", DBNull.Value);
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


        public string Delete(string Para1,string Para2,string Para3)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("Sp_Dic_KHDT_Store", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "Delete");
                cmd.Parameters.AddWithValue("@Para1", Para1);
                cmd.Parameters.AddWithValue("@Para2", Para2);
                cmd.Parameters.AddWithValue("@Para3", Para3);

                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }


        public string Post(object objSave)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("Sp_Dic_KHDT_Store", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POST");
                cmd.Parameters.AddWithValue("@Para1", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para2", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para3", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable", objSave);
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
