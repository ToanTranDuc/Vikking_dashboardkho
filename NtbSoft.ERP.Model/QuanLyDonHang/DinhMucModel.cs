using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace NtbSoft.ERP.Model.QuanLyDonHang
{
    public class DinhMucModel
    {
        public DataTable Get(int pageIndex, int pageSize,string parameter)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DINHMUC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GET");
                cmd.Parameters.AddWithValue("Parameter",parameter);
                //cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                //cmd.Parameters.AddWithValue("@PageSize", pageSize);
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
        }
        public DataTable GetKiemTraDinhMuc(string donhang) 
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DINHMUC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Action", "GETKIEMTRADINHMUC");
                cmd.Parameters.AddWithValue("Parameter", donhang);
                //cmd.Parameters.AddWithValue("@PageIndex", 0);
                //cmd.Parameters.AddWithValue("@PageSize", 0);
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
        }
        public string PostDinhMuc(DataTable ojDinhMuc)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DINHMUC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "POSTDINHMUC");
                cmd.Parameters.AddWithValue("@Parameter", 0);
                //cmd.Parameters.AddWithValue("@PageIndex", 0);
                //cmd.Parameters.AddWithValue("@PageSize", 0);
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
        public string UpdateNPL(DataTable ojDinhMuc)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_DinhMucNPL", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "UPDATENPL");
                cmd.Parameters.AddWithValue("@Param", 0);
                //cmd.Parameters.AddWithValue("@PageIndex", 0);
                //cmd.Parameters.AddWithValue("@PageSize", 0);
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
