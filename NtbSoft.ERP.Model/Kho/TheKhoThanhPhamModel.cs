using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.Kho
{
    public class TheKhoThanhPhamModel
    {
        public DataTable Get(string action, string para, string para2, string para3, string para4, string para5, string para6, string para7, string para8)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("Sp_TheKhoThanhPham", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", action);
                cmd.Parameters.AddWithValue("@Para", para ?? "");
                cmd.Parameters.AddWithValue("@Para2", para2 ?? "");
                cmd.Parameters.AddWithValue("@Para3", para3 ?? "");
                cmd.Parameters.AddWithValue("@Para4", para4 ?? "");
                cmd.Parameters.AddWithValue("@Para5", para5 ?? "");
                cmd.Parameters.AddWithValue("@Para6", para6 ?? "");
                cmd.Parameters.AddWithValue("@Para7", para7 ?? "");
                cmd.Parameters.AddWithValue("@Para8", para8 ?? "");
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
