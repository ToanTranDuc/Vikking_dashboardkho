using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace NtbSoft.ERP.Model.Kho
{
    public class ERPNhapKhoNPLPOMUAModel
    {
        public DataTable Get(string action, string para1, string para2, string para3, string para4, string para5, string para6, string para7,string para8, string para9, string para10)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_NHAPKHONPLPOMUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", para4 ?? "");
                cmd.Parameters.AddWithValue("@parameter5", para5 ?? "");
                cmd.Parameters.AddWithValue("@parameter6", para6 ?? "");
                cmd.Parameters.AddWithValue("@parameter7", para7 ?? "");
                cmd.Parameters.AddWithValue("@parameter8", para8 ?? "");
                cmd.Parameters.AddWithValue("@parameter9", para9 ?? "");
                cmd.Parameters.AddWithValue("@parameter10", para10 ?? "");
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
        public string Post(string action, string para1, string para2, string para3, string para4, string para5, string para6, string para7, string para8, string para9, string para10, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_NHAPKHONPLPOMUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", para4 ?? "");
                cmd.Parameters.AddWithValue("@parameter5", para5 ?? "");
                cmd.Parameters.AddWithValue("@parameter6", para6 ?? "");
                cmd.Parameters.AddWithValue("@parameter7", para7 ?? "");
                cmd.Parameters.AddWithValue("@parameter8", para8 ?? "");
                cmd.Parameters.AddWithValue("@parameter9", para9 ?? "");
                cmd.Parameters.AddWithValue("@parameter10", para10 ?? "");
                //cmd.Parameters.AddWithValue("@TypeTable", tbl);
                //cmd.Parameters.AddWithValue("@TypeTable2", null);
                //cmd.Parameters.AddWithValue("@TypeTable3", null);
                cmd.ExecuteNonQuery();
                return "True";

            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        //public string PostT2(string action, string para1, string para2, string para3, string para4, string para5, string para6, string para7, string para8, string para9, string para10, DataTable tbl)
        //{
        //    SqlConnection conn = null;
        //    try
        //    {
        //        conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
        //        SqlCommand cmd = new SqlCommand("SP_ERP_NHAPKHONPLPOMUA", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@Action", action);
        //        cmd.Parameters.AddWithValue("@parameter1", para1 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter2", para2 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter3", para3 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter4", para4 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter5", para5 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter6", para6 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter7", para7 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter8", para8 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter9", para9 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter10", para10 ?? "");
        //        cmd.Parameters.AddWithValue("@TypeTable", null);
        //        cmd.Parameters.AddWithValue("@TypeTable2", tbl);
        //        cmd.Parameters.AddWithValue("@TypeTable3", null);
        //        cmd.ExecuteNonQuery();
        //        return "True";

        //    }
        //    catch (SqlException ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        //}
        //public string PostT3(string action, string para1, string para2, string para3, string para4, string para5, string para6, string para7, string para8, string para9, string para10, DataTable tbl)
        //{
        //    SqlConnection conn = null;
        //    try
        //    {
        //        conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
        //        SqlCommand cmd = new SqlCommand("SP_ERP_NHAPKHONPLPOMUA", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@Action", action);
        //        cmd.Parameters.AddWithValue("@parameter1", para1 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter2", para2 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter3", para3 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter4", para4 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter5", para5 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter6", para6 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter7", para7 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter8", para8 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter9", para9 ?? "");
        //        cmd.Parameters.AddWithValue("@parameter10", para10 ?? "");
        //        cmd.Parameters.AddWithValue("@TypeTable", null);
        //        cmd.Parameters.AddWithValue("@TypeTable2", null);
        //        cmd.Parameters.AddWithValue("@TypeTable3", tbl);
        //        cmd.ExecuteNonQuery();
        //        return "True";

        //    }
        //    catch (SqlException ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //    finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        //}
        public string Delete(string action, string para1, string para2, string para3, string para4, string para5, string para6, string para7, string para8, string para9, string para10)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_NHAPKHONPLPOMUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@parameter1", para1 ?? "");
                cmd.Parameters.AddWithValue("@parameter2", para2 ?? "");
                cmd.Parameters.AddWithValue("@parameter3", para3 ?? "");
                cmd.Parameters.AddWithValue("@parameter4", para4 ?? "");
                cmd.Parameters.AddWithValue("@parameter5", para5 ?? "");
                cmd.Parameters.AddWithValue("@parameter6", para6 ?? "");
                cmd.Parameters.AddWithValue("@parameter7", para7 ?? "");
                cmd.Parameters.AddWithValue("@parameter8", para8 ?? "");
                cmd.Parameters.AddWithValue("@parameter9", para9 ?? "");
                cmd.Parameters.AddWithValue("@parameter10", para10 ?? "");
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
