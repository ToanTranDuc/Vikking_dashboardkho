using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
    public class BCCapThem_ThuHoi_NPL
    {
        public DataTable Get(string action, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("USP_QuanLy_CapVaThuHoi_NPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Param1", param1);
                cmd.Parameters.AddWithValue("@Param2", param2);
                cmd.Parameters.AddWithValue("@Param3", param3);
                cmd.Parameters.AddWithValue("@Param4", param4);
                cmd.Parameters.AddWithValue("@Param5", param5);
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
