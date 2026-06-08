using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.Kho
{
  public  class Erp_Export_BBMoKien_NPLModel
    {
        public DataTable Get(string Action,string para1, string para2, string para3, string para4)
        {
            SqlConnection conn = null;
            try
            {

                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_Export_BB_MoKien_NPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@Para1", para1 == "NONE" ? DBNull.Value : (object)para1);
                cmd.Parameters.AddWithValue("@Para2", para2 == "NONE" ? DBNull.Value : (object)para2);
                cmd.Parameters.AddWithValue("@Para3", para3 == "NONE" ? DBNull.Value : (object)para3);
                cmd.Parameters.AddWithValue("@Para4", para4 == "NONE" ? DBNull.Value : (object)para4);
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
