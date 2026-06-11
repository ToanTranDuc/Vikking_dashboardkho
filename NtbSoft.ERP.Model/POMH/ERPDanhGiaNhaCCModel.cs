using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtbSoft.ERP.Model.POMuaHang
{
  public class ERPDanhGiaNhaCCModel
    {
        public async Task<DataTable> Get(string action, string para1, string para2, string para3, string para4, string para5)
        {
            using (SqlConnection _cnn = NtbSoft.ERP.Libs.SqlHelper.GetConnection())
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SP_ERP_POMH_DANHGIA_NHACC", _cnn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@Para1", para1 == "NONE" ? DBNull.Value : (object)para1);
                        cmd.Parameters.AddWithValue("@Para2", para2 == "NONE" ? DBNull.Value : (object)para2);
                        cmd.Parameters.AddWithValue("@Para3", para3 == "NONE" ? DBNull.Value : (object)para3);
                        cmd.Parameters.AddWithValue("@Para4", para4 == "NONE" ? DBNull.Value : (object)para4);
                        cmd.Parameters.AddWithValue("@Para5", para5 == "NONE" ? DBNull.Value : (object)para5);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            var tb = new DataTable();
                            tb.Load(reader);
                            return tb;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (_cnn != null) { _cnn.Close(); _cnn.Dispose(); }
                }
            }
        }

        public async Task<string> Delete(string Action, string Para1, string Para2,string Para3)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_POMH_DANHGIA_NHACC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@Para1", Para1);
                cmd.Parameters.AddWithValue("@Para2", Para2);
                cmd.Parameters.AddWithValue("@Para3", Para3);
                cmd.Parameters.AddWithValue("@Para4", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para5", DBNull.Value);

                await cmd.ExecuteNonQueryAsync();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public async Task<string> PostPhieuDG_TQ(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_POMH_DANHGIA_NHACC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "PostPhieuDG_TQ");
                cmd.Parameters.AddWithValue("@Para1", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para2", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para3", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para4", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para5", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable_PhieuTQ", tbl);

                await cmd.ExecuteNonQueryAsync();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public async Task<string> ImportTCNoiLV(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_POMH_DANHGIA_NHACC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "ImportTCNoiLV");
                cmd.Parameters.AddWithValue("@Para1", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para2", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para3", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para4", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para5", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable_Dic_NoiLV", tbl);

                await cmd.ExecuteNonQueryAsync();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
/**/
        public async Task<string> PostPhieuDG(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_POMH_DANHGIA_NHACC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "PostPhieuDG");
                cmd.Parameters.AddWithValue("@Para1", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para2", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para3", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para4", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para5", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable", tbl);

                await cmd.ExecuteNonQueryAsync();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public async Task<string> PostPhieuDGNoiLV(DataTable tbl)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_POMH_DANHGIA_NHACC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "PostPhieuDGNoiLV");
                cmd.Parameters.AddWithValue("@Para1", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para2", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para3", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para4", DBNull.Value);
                cmd.Parameters.AddWithValue("@Para5", DBNull.Value);
                cmd.Parameters.AddWithValue("@TypeTable_PhieuDG_NoiLV", tbl);

                await cmd.ExecuteNonQueryAsync();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }

        public async Task<string> UpdateKetLuan(string action, string para1, string para2, string para3, string para4, string para5)
        {
            SqlConnection conn = null;
            try
            {
                conn = NtbSoft.ERP.Libs.SqlHelper.GetConnection();
                SqlCommand cmd = new SqlCommand("SP_ERP_POMH_DANHGIA_NHACC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@Para1", para1 == "NONE" ? DBNull.Value : (object)para1);
                cmd.Parameters.AddWithValue("@Para2", para2 == "NONE" ? DBNull.Value : (object)para2);
                cmd.Parameters.AddWithValue("@Para3", para3 == "NONE" ? DBNull.Value : (object)para3);
                cmd.Parameters.AddWithValue("@Para4", para4 == "NONE" ? DBNull.Value : (object)para4);
                cmd.Parameters.AddWithValue("@Para5", para5 == "NONE" ? DBNull.Value : (object)para5);
                await cmd.ExecuteNonQueryAsync();
                return "True";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
            finally { if (conn != null) { conn.Close(); conn.Dispose(); } }
        }
    }
}
