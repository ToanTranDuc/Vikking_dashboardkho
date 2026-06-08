using System;
using System.Data;
using System.Data.SqlClient;

namespace NtbSoft.ERP.Model.KeHoach
{
    public class PackageListChuyenKhoModel
    {
        public DataTable Get(string Action, string Para1, string Para2,string Para3)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PackageListChuyenKho", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@Para1", Para1 ?? "");
                cmd.Parameters.AddWithValue("@Para2", Para2 ?? "");
                cmd.Parameters.AddWithValue("@Para3", Para3 ?? "");
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
        public string Post(string action, object dtXuatHang, DataTable dtDongThung, DataTable dtTonKho)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PackageListChuyenKho", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", "");
                cmd.Parameters.AddWithValue("@Para2", "");
                cmd.Parameters.AddWithValue("@Para3", "");
                cmd.Parameters.AddWithValue("@TypePackageList", dtXuatHang);
                //cmd.Parameters.AddWithValue("@TypeTableKHDongThung", dtDongThung);
                if (dtDongThung != null && dtDongThung.Rows.Count > 0)
                    cmd.Parameters.AddWithValue("@TypeTableKHDongThung", dtDongThung);
                if (dtTonKho != null && dtTonKho.Rows.Count > 0)
                    cmd.Parameters.AddWithValue("@TypeTableKHDongThungTonKho", dtTonKho);
                cmd.ExecuteNonQuery();
                return "True";
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
        public string Delete(string action, string Para, object dtDongThung)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_PackageListChuyenKho", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", Para);
                cmd.Parameters.AddWithValue("@Para2", "");
                cmd.Parameters.AddWithValue("@Para3", "");
                if (action == "DeletePKLXH_Row" || action == "DeletePKLXH_Row_Ton")
                    cmd.Parameters.AddWithValue("@TypeTableKHDongThung", dtDongThung);
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
