using System;
using System.Data;
using System.Data.SqlClient;

namespace NtbSoft.ERP.Model.ThuVien
{
    public class QTY_MaHang_HinhModel
    {
        public DataTable GetCaiDatSeason(string action, string mahang, string sizeTypeid, string season, string version)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("spQtyCaiDatThongSoMaHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@MaHang", mahang ?? "");
                cmd.Parameters.AddWithValue("@SizeTypeID", sizeTypeid ?? "");
                cmd.Parameters.AddWithValue("@Season", season ?? "");
                cmd.Parameters.AddWithValue("@Version", version ?? "");
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
        public string PostCaiDatSeason(string action, DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("spQtyCaiDatThongSoMaHang", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@MaHang", "");
                cmd.Parameters.AddWithValue("@SizeTypeID", "");
                cmd.Parameters.AddWithValue("@Season", "");
                cmd.Parameters.AddWithValue("@Version", "");
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public DataTable Get(string action, string para1)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("spQTY_MaHang_Hinh", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", para1 ?? "");
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
        public string Post(string action, DataTable tbl,string para1 ="")
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnectionSX();
                SqlCommand cmd = new SqlCommand("spQTY_MaHang_Hinh", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", para1);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);
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
